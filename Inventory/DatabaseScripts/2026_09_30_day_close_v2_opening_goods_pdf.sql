/*
  Day Close V2: editable opening, goods movement summary, saved-close report data.

  Requires 2026_09_29_day_close_v2_reconciliation.sql.
  Does not delete or rewrite existing Day Close rows.
  Adds one nullable column: DayCloseV2.OpeningDefaultCash (previous actual closing offered as the default).
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;

IF DB_NAME() IN ('master', 'model', 'msdb', 'tempdb')
BEGIN
    RAISERROR('Refusing to change a system database.', 16, 1);
    RETURN;
END
GO

IF COL_LENGTH('dbo.DayCloseV2', 'OpeningDefaultCash') IS NULL
    ALTER TABLE dbo.DayCloseV2 ADD OpeningDefaultCash decimal(18, 2) NULL;
GO

IF OBJECT_ID('dbo.fn_DayCloseV2_GoodsCategory', 'FN') IS NULL
    EXEC('CREATE FUNCTION dbo.fn_DayCloseV2_GoodsCategory (@TranscationType varchar(250)) RETURNS nvarchar(60) AS BEGIN RETURN NULL; END');
GO

ALTER FUNCTION dbo.fn_DayCloseV2_GoodsCategory
(
    @TranscationType varchar(250)
)
RETURNS nvarchar(60)
AS
BEGIN
    DECLARE @t varchar(250);
    SET @t = UPPER(LTRIM(RTRIM(ISNULL(@TranscationType, ''))));
    RETURN CASE
        WHEN @t = 'PURCHASE' THEN N'Purchase'
        WHEN @t = 'PDI' THEN N'Sales / PDI'
        WHEN @t = 'PDI-REVERSE' THEN N'PDI Reverse'
        WHEN @t = 'SALES RETURN' THEN N'Sales Return'
        WHEN @t = 'SALES RETURN-REVERSE' THEN N'Sales Return Reverse'
        WHEN @t = 'PRODUCT MOVEMENT' THEN N'Stock Movement'
        WHEN @t = 'WARRANTY REPLACEMENT' THEN N'Warranty Replacement'
        WHEN @t IN ('ISSUEDRECEIVED', 'ISSUED', 'RECEIVED') THEN N'Issued / Received'
        WHEN @t = 'RECEIPT NOTE' THEN N'Receipt Note'
        WHEN @t = 'DELIVERY NOTE' THEN N'Delivery Note'
        WHEN @t = 'STOCK ADJUSTMENT' THEN N'Stock Adjustment'
        WHEN @t = 'OPENING STOCK' THEN N'Opening Stock'
        WHEN @t = '' THEN N'Unspecified'
        ELSE N'Other: ' + CONVERT(nvarchar(52), LTRIM(RTRIM(@TranscationType)))
    END;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetGoodsSummary', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetGoodsSummary AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetGoodsSummary
(
    @BusinessDate date
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @start datetime;
    DECLARE @end datetime;
    SET @start = @BusinessDate;
    SET @end = DATEADD(day, 1, @BusinessDate);

    CREATE TABLE #moves
    (
        ProductId int NULL,
        Category nvarchar(60) NOT NULL,
        GoodsIn decimal(18, 3) NOT NULL,
        GoodsOut decimal(18, 3) NOT NULL
    );

    INSERT INTO #moves (ProductId, Category, GoodsIn, GoodsOut)
    SELECT
        mt.MaterailId,
        dbo.fn_DayCloseV2_GoodsCategory(mt.TranscationType),
        CASE WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'IN' THEN ISNULL(mt.Quantity, 0) ELSE 0 END,
        CASE WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'OUT' THEN ISNULL(mt.Quantity, 0) ELSE 0 END
    FROM dbo.MaterialTranscation mt
    WHERE mt.TranscationDate >= @start
      AND mt.TranscationDate < @end;

    SELECT
        m.ProductId,
        ISNULL(pm.ItemCode, '') AS ProductCode,
        ISNULL(pm.DisplayName, '') AS ProductName,
        m.Category,
        SUM(m.GoodsIn) AS GoodsIn,
        SUM(m.GoodsOut) AS GoodsOut,
        SUM(m.GoodsIn) - SUM(m.GoodsOut) AS Net
    FROM #moves m
    LEFT JOIN dbo.ProductMaster pm ON pm.id = m.ProductId
    GROUP BY m.ProductId, pm.ItemCode, pm.DisplayName, m.Category
    ORDER BY m.Category, pm.DisplayName;

    SELECT
        Category,
        SUM(GoodsIn) AS GoodsIn,
        SUM(GoodsOut) AS GoodsOut,
        SUM(GoodsIn) - SUM(GoodsOut) AS Net,
        COUNT(DISTINCT ProductId) AS Products
    FROM #moves
    GROUP BY Category
    ORDER BY Category;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetGoodsDetail', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetGoodsDetail AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetGoodsDetail
(
    @BusinessDate date,
    @ProductId int,
    @Category nvarchar(60)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @start datetime;
    DECLARE @end datetime;
    SET @start = @BusinessDate;
    SET @end = DATEADD(day, 1, @BusinessDate);

    SELECT
        mt.TranscationDate AS TransTime,
        mt.TransId AS Reference,
        mt.TranscationType AS StoredType,
        dbo.fn_DayCloseV2_GoodsCategory(mt.TranscationType) AS Category,
        UPPER(LTRIM(RTRIM(mt.Type))) AS Movement,
        mt.Quantity,
        ISNULL(lm.LocationName, '') AS Location,
        ISNULL(rm.RackCaption, '') AS Rack,
        ISNULL(u.UserName, '') AS UpdatedBy
    FROM dbo.MaterialTranscation mt
    LEFT JOIN dbo.LocationMaster lm ON CONVERT(varchar(100), lm.LocationId) = CONVERT(varchar(100), mt.LocationId)
    LEFT JOIN dbo.RackMaster rm ON rm.RackId = mt.RackId
    LEFT JOIN dbo.Users u ON CONVERT(varchar(50), u.UserId) = CONVERT(varchar(50), mt.Updatedby)
    WHERE mt.TranscationDate >= @start
      AND mt.TranscationDate < @end
      AND ((@ProductId IS NULL AND mt.MaterailId IS NULL) OR mt.MaterailId = @ProductId)
      AND (@Category IS NULL OR dbo.fn_DayCloseV2_GoodsCategory(mt.TranscationType) = @Category)
    ORDER BY mt.TranscationDate, mt.ID;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetDenominationRecon', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetDenominationRecon AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetDenominationRecon
(
    @BusinessDate date
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @id int;
    SELECT @id = Id
    FROM dbo.DayCloseV2
    WHERE BusinessDate = @BusinessDate AND Status = 'Completed';

    IF @id IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.DayCloseV2DenominationLine WHERE DayCloseId = @id)
    BEGIN
        SELECT FaceValue, OpeningQty, InQty, OutQty, ExpectedQty, ActualQty
        FROM dbo.DayCloseV2DenominationLine
        WHERE DayCloseId = @id
        ORDER BY FaceValue DESC;
        RETURN;
    END

    DECLARE @start datetime;
    DECLARE @end datetime;
    DECLARE @prevId int;
    SET @start = @BusinessDate;
    SET @end = DATEADD(day, 1, @BusinessDate);

    IF @id IS NULL
    BEGIN
        SELECT TOP 1 @prevId = Id
        FROM dbo.DayCloseV2
        WHERE Status = 'Completed' AND BusinessDate < @BusinessDate
        ORDER BY BusinessDate DESC;
    END

    ;WITH notes AS
    (
        SELECT UPPER(LTRIM(RTRIM(c.Type))) AS PayType, v.FaceValue,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(v.Qty)), '')), 0)) AS Qty
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        CROSS APPLY (VALUES
            (2000, d.TwoThousand), (1000, d.Thousand), (500, d.FiveHundred), (200, d.TwoHundred),
            (100, d.Hundred), (50, d.Fifty), (20, d.Twenty), (10, d.Ten), (5, d.Five), (2, d.Two), (1, d.One)
        ) v (FaceValue, Qty)
        WHERE c.TransDate >= @start AND c.TransDate < @end
    ),
    movement AS
    (
        SELECT FaceValue,
            SUM(CASE WHEN PayType = 'C' AND Qty > 0 THEN Qty WHEN PayType = 'D' AND Qty < 0 THEN -Qty ELSE 0 END) AS InQty,
            SUM(CASE WHEN PayType = 'C' AND Qty < 0 THEN -Qty WHEN PayType = 'D' AND Qty > 0 THEN Qty ELSE 0 END) AS OutQty
        FROM notes
        GROUP BY FaceValue
    ),
    faces AS
    (
        SELECT 2000 AS FaceValue UNION ALL SELECT 1000 UNION ALL SELECT 500 UNION ALL SELECT 200
        UNION ALL SELECT 100 UNION ALL SELECT 50 UNION ALL SELECT 20 UNION ALL SELECT 10
        UNION ALL SELECT 5 UNION ALL SELECT 2 UNION ALL SELECT 1
    ),
    counts AS
    (
        SELECT v.FaceValue, v.Qty, d.DayCloseId
        FROM dbo.DayCloseV2Denomination d
        CROSS APPLY (VALUES
            (2000, d.TwoThousand), (1000, d.Thousand), (500, d.FiveHundred), (200, d.TwoHundred),
            (100, d.Hundred), (50, d.Fifty), (20, d.Twenty), (10, d.Ten), (5, d.Five), (2, d.Two), (1, d.One)
        ) v (FaceValue, Qty)
        WHERE d.DayCloseId IN (ISNULL(@prevId, -1), ISNULL(@id, -1))
    )
    SELECT
        f.FaceValue,
        ISNULL(o.Qty, 0) AS OpeningQty,
        ISNULL(m.InQty, 0) AS InQty,
        ISNULL(m.OutQty, 0) AS OutQty,
        ISNULL(o.Qty, 0) + ISNULL(m.InQty, 0) - ISNULL(m.OutQty, 0) AS ExpectedQty,
        a.Qty AS ActualQty
    FROM faces f
    LEFT JOIN movement m ON m.FaceValue = f.FaceValue
    LEFT JOIN counts o ON o.FaceValue = f.FaceValue AND o.DayCloseId = @prevId
    LEFT JOIN counts a ON a.FaceValue = f.FaceValue AND a.DayCloseId = @id
    ORDER BY f.FaceValue DESC;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetReport', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetReport AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetReport
(
    @BusinessDate date
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @id int;
    SELECT @id = Id
    FROM dbo.DayCloseV2
    WHERE BusinessDate = @BusinessDate AND Status = 'Completed';

    SELECT
        h.Id, h.BusinessDate, h.OpeningCash, h.OpeningDefaultCash,
        h.ExpectedCashClosing, h.ActualCashClosing, h.CashDifference,
        h.ExpectedUpiNet, h.ActualUpiNet, h.UpiDifference,
        h.ExpectedCardNet, h.ActualCardNet, h.CardDifference,
        h.ExpectedTotal, h.ActualTotal, h.TotalDifference,
        h.Status, h.Remarks,
        ISNULL(NULLIF(eu.UserFullName, ''), ISNULL(eu.UserName, CONVERT(varchar(20), h.EnteredBy))) AS ClosedBy,
        h.EnteredOn AS ClosedOn,
        COALESCE(NULLIF(LTRIM(RTRIM(eu.UserFullName)), ''), NULLIF(LTRIM(RTRIM(eu.UserName)), '')) AS PreparedBy,
        ISNULL(NULLIF(ru.UserFullName, ''), ru.UserName) AS ReviewedBy,
        h.ReviewedOn
    FROM dbo.DayCloseV2 h
    LEFT JOIN dbo.Users eu ON eu.UserId = h.EnteredBy
    LEFT JOIN dbo.Users ru ON ru.UserId = h.ReviewedBy
    WHERE h.Id = @id;

    SELECT PaymentMode, OpeningAmount, TotalIn, TotalOut, ExpectedNet, ActualAmount, Difference
    FROM dbo.DayCloseV2PaymentSummary
    WHERE DayCloseId = @id
    ORDER BY CASE PaymentMode WHEN 'CASH' THEN 1 WHEN 'UPI' THEN 2 ELSE 3 END;

    IF EXISTS (SELECT 1 FROM dbo.DayCloseV2DenominationLine WHERE DayCloseId = @id)
    BEGIN
        SELECT FaceValue, OpeningQty, InQty, OutQty, ExpectedQty, ActualQty
        FROM dbo.DayCloseV2DenominationLine
        WHERE DayCloseId = @id
        ORDER BY FaceValue DESC;
    END
    ELSE
    BEGIN
        SELECT v.FaceValue, CAST(NULL AS int) AS OpeningQty, CAST(NULL AS int) AS InQty, CAST(NULL AS int) AS OutQty,
            CAST(NULL AS int) AS ExpectedQty, v.Qty AS ActualQty
        FROM dbo.DayCloseV2Denomination d
        CROSS APPLY (VALUES
            (2000, d.TwoThousand), (1000, d.Thousand), (500, d.FiveHundred), (200, d.TwoHundred),
            (100, d.Hundred), (50, d.Fifty), (20, d.Twenty), (10, d.Ten), (5, d.Five), (2, d.Two), (1, d.One)
        ) v (FaceValue, Qty)
        WHERE d.DayCloseId = @id
        ORDER BY v.FaceValue DESC;
    END
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_Submit', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_Submit AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_Submit
(
    @BusinessDate date,
    @OpeningCash decimal(18, 2),
    @ActualUpi decimal(18, 2),
    @ActualCard decimal(18, 2),
    @Remarks nvarchar(500),
    @EnteredBy int,
    @TwoThousand int,
    @Thousand int,
    @FiveHundred int,
    @TwoHundred int,
    @Hundred int,
    @Fifty int,
    @Twenty int,
    @Ten int,
    @Five int,
    @Two int,
    @One int,
    @Result int OUT,
    @Message nvarchar(400) OUT,
    @OpenTwoThousand int = NULL,
    @OpenThousand int = NULL,
    @OpenFiveHundred int = NULL,
    @OpenTwoHundred int = NULL,
    @OpenHundred int = NULL,
    @OpenFifty int = NULL,
    @OpenTwenty int = NULL,
    @OpenTen int = NULL,
    @OpenFive int = NULL,
    @OpenTwo int = NULL,
    @OpenOne int = NULL,
    @DayCloseId int = NULL OUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Result = 0;
    SET @Message = '';
    SET @DayCloseId = NULL;

    IF EXISTS (SELECT 1 FROM dbo.DayCloseV2 WHERE BusinessDate = @BusinessDate AND Status = 'Completed')
    BEGIN
        SET @Message = 'This business date is already closed.';
        RETURN;
    END

    DECLARE @defaultOpening decimal(18, 2);
    DECLARE @opening decimal(18, 2);
    DECLARE @cashIn decimal(18, 2);
    DECLARE @cashOut decimal(18, 2);
    DECLARE @upiIn decimal(18, 2);
    DECLARE @upiOut decimal(18, 2);
    DECLARE @cardIn decimal(18, 2);
    DECLARE @cardOut decimal(18, 2);
    DECLARE @expectedCash decimal(18, 2);
    DECLARE @expectedUpi decimal(18, 2);
    DECLARE @expectedCard decimal(18, 2);
    DECLARE @actualCash decimal(18, 2);
    DECLARE @start datetime;
    DECLARE @end datetime;
    DECLARE @prevId int;
    DECLARE @openTotal decimal(18, 2);
    DECLARE @openSupplied bit;

    SET @start = @BusinessDate;
    SET @end = DATEADD(day, 1, @BusinessDate);

    SELECT TOP 1 @defaultOpening = ActualCashClosing, @prevId = Id
    FROM dbo.DayCloseV2
    WHERE Status = 'Completed' AND BusinessDate < @BusinessDate
    ORDER BY BusinessDate DESC;

    SET @openSupplied = CASE WHEN COALESCE(@OpenTwoThousand, @OpenThousand, @OpenFiveHundred, @OpenTwoHundred, @OpenHundred,
        @OpenFifty, @OpenTwenty, @OpenTen, @OpenFive, @OpenTwo, @OpenOne) IS NULL THEN 0 ELSE 1 END;

    IF @OpeningCash IS NULL
    BEGIN
        IF @prevId IS NULL
        BEGIN
            SET @Message = 'Enter the opening cash for the first Day Close.';
            RETURN;
        END
        SET @opening = @defaultOpening;
    END
    ELSE
        SET @opening = @OpeningCash;

    IF @opening < 0
    BEGIN
        SET @Message = 'Opening cash cannot be negative.';
        RETURN;
    END

    IF @openSupplied = 0 AND @prevId IS NOT NULL
    BEGIN
        SELECT
            @OpenTwoThousand = TwoThousand, @OpenThousand = Thousand, @OpenFiveHundred = FiveHundred,
            @OpenTwoHundred = TwoHundred, @OpenHundred = Hundred, @OpenFifty = Fifty,
            @OpenTwenty = Twenty, @OpenTen = Ten, @OpenFive = Five, @OpenTwo = Two, @OpenOne = One
        FROM dbo.DayCloseV2Denomination
        WHERE DayCloseId = @prevId;
    END

    IF ISNULL(@OpenTwoThousand, 0) < 0 OR ISNULL(@OpenThousand, 0) < 0 OR ISNULL(@OpenFiveHundred, 0) < 0
        OR ISNULL(@OpenTwoHundred, 0) < 0 OR ISNULL(@OpenHundred, 0) < 0 OR ISNULL(@OpenFifty, 0) < 0
        OR ISNULL(@OpenTwenty, 0) < 0 OR ISNULL(@OpenTen, 0) < 0 OR ISNULL(@OpenFive, 0) < 0
        OR ISNULL(@OpenTwo, 0) < 0 OR ISNULL(@OpenOne, 0) < 0
    BEGIN
        SET @Message = 'Opening denomination counts cannot be negative.';
        RETURN;
    END

    SET @openTotal =
        (ISNULL(@OpenTwoThousand, 0) * 2000) + (ISNULL(@OpenThousand, 0) * 1000) + (ISNULL(@OpenFiveHundred, 0) * 500)
        + (ISNULL(@OpenTwoHundred, 0) * 200) + (ISNULL(@OpenHundred, 0) * 100) + (ISNULL(@OpenFifty, 0) * 50)
        + (ISNULL(@OpenTwenty, 0) * 20) + (ISNULL(@OpenTen, 0) * 10) + (ISNULL(@OpenFive, 0) * 5)
        + (ISNULL(@OpenTwo, 0) * 2) + ISNULL(@OpenOne, 0);
    IF ABS(@openTotal - @opening) > 0.009
    BEGIN
        SET @Message = 'Opening denomination total does not match Opening Cash.';
        RETURN;
    END

    SELECT
        @cashIn = ISNULL(SUM(CASE WHEN UPPER(Type) = 'C' THEN CONVERT(decimal(18, 2), REPLACE(Amount, ',', '')) ELSE 0 END), 0),
        @cashOut = ISNULL(SUM(CASE WHEN UPPER(Type) = 'D' THEN CONVERT(decimal(18, 2), REPLACE(Amount, ',', '')) ELSE 0 END), 0)
    FROM CashTransaction
    WHERE TransDate >= @start AND TransDate < @end;

    SELECT
        @upiIn = ISNULL(SUM(CASE WHEN ISNULL(Type, 'C') = 'C' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0),
        @upiOut = ISNULL(SUM(CASE WHEN Type = 'D' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0)
    FROM CardTransactions
    WHERE TransDate >= @start AND TransDate < @end AND Mode = 'UPI';

    SELECT
        @cardIn = ISNULL(SUM(CASE WHEN ISNULL(Type, 'C') = 'C' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0),
        @cardOut = ISNULL(SUM(CASE WHEN Type = 'D' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0)
    FROM CardTransactions
    WHERE TransDate >= @start AND TransDate < @end AND Mode = 'Card';

    SET @expectedCash = @opening + @cashIn - @cashOut;
    SET @expectedUpi = @upiIn - @upiOut;
    SET @expectedCard = @cardIn - @cardOut;
    SET @actualCash =
        (ISNULL(@TwoThousand, 0) * 2000) + (ISNULL(@Thousand, 0) * 1000) + (ISNULL(@FiveHundred, 0) * 500)
        + (ISNULL(@TwoHundred, 0) * 200) + (ISNULL(@Hundred, 0) * 100) + (ISNULL(@Fifty, 0) * 50)
        + (ISNULL(@Twenty, 0) * 20) + (ISNULL(@Ten, 0) * 10) + (ISNULL(@Five, 0) * 5)
        + (ISNULL(@Two, 0) * 2) + ISNULL(@One, 0);

    IF ABS(@expectedCash - @actualCash) > 0.009
        OR ABS(@expectedUpi - ISNULL(@ActualUpi, 0)) > 0.009
        OR ABS(@expectedCard - ISNULL(@ActualCard, 0)) > 0.009
    BEGIN
        IF @Remarks IS NULL OR LTRIM(RTRIM(@Remarks)) = ''
        BEGIN
            SET @Message = 'Enter a variance reason. Actual closing does not match the expected total.';
            RETURN;
        END
    END

    DECLARE @move TABLE (FaceValue int PRIMARY KEY, InQty int, OutQty int);
    INSERT INTO @move (FaceValue, InQty, OutQty)
    SELECT FaceValue,
        SUM(CASE WHEN PayType = 'C' AND Qty > 0 THEN Qty WHEN PayType = 'D' AND Qty < 0 THEN -Qty ELSE 0 END),
        SUM(CASE WHEN PayType = 'C' AND Qty < 0 THEN -Qty WHEN PayType = 'D' AND Qty > 0 THEN Qty ELSE 0 END)
    FROM
    (
        SELECT UPPER(LTRIM(RTRIM(c.Type))) AS PayType, v.FaceValue,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(v.Qty)), '')), 0)) AS Qty
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        CROSS APPLY (VALUES
            (2000, d.TwoThousand), (1000, d.Thousand), (500, d.FiveHundred), (200, d.TwoHundred),
            (100, d.Hundred), (50, d.Fifty), (20, d.Twenty), (10, d.Ten), (5, d.Five), (2, d.Two), (1, d.One)
        ) v (FaceValue, Qty)
        WHERE c.TransDate >= @start AND c.TransDate < @end
    ) s
    GROUP BY FaceValue;

    DECLARE @closeId int;

    BEGIN TRAN;
        IF EXISTS (SELECT 1 FROM dbo.DayCloseV2 WITH (UPDLOCK, HOLDLOCK) WHERE BusinessDate = @BusinessDate AND Status = 'Completed')
        BEGIN
            ROLLBACK TRAN;
            SET @Message = 'This business date is already closed.';
            RETURN;
        END

        INSERT INTO dbo.DayCloseV2
            (BusinessDate, OpeningCash, OpeningDefaultCash, ExpectedCashClosing, ActualCashClosing, CashDifference,
             ExpectedUpiNet, ActualUpiNet, UpiDifference, ExpectedCardNet, ActualCardNet, CardDifference,
             ExpectedTotal, ActualTotal, TotalDifference, Status, Remarks, EnteredBy, EnteredOn)
        VALUES
            (@BusinessDate, @opening, @defaultOpening, @expectedCash, @actualCash, @actualCash - @expectedCash,
             @expectedUpi, ISNULL(@ActualUpi, 0), ISNULL(@ActualUpi, 0) - @expectedUpi,
             @expectedCard, ISNULL(@ActualCard, 0), ISNULL(@ActualCard, 0) - @expectedCard,
             @expectedCash + @expectedUpi + @expectedCard,
             @actualCash + ISNULL(@ActualUpi, 0) + ISNULL(@ActualCard, 0),
             (@actualCash + ISNULL(@ActualUpi, 0) + ISNULL(@ActualCard, 0)) - (@expectedCash + @expectedUpi + @expectedCard),
             'Completed', @Remarks, @EnteredBy, GETDATE());

        SET @closeId = SCOPE_IDENTITY();

        INSERT INTO dbo.DayCloseV2PaymentSummary
            (DayCloseId, PaymentMode, OpeningAmount, TotalIn, TotalOut, ExpectedNet, ActualAmount, Difference)
        VALUES
            (@closeId, 'CASH', @opening, @cashIn, @cashOut, @expectedCash, @actualCash, @actualCash - @expectedCash),
            (@closeId, 'UPI', 0, @upiIn, @upiOut, @expectedUpi, ISNULL(@ActualUpi, 0), ISNULL(@ActualUpi, 0) - @expectedUpi),
            (@closeId, 'CARD', 0, @cardIn, @cardOut, @expectedCard, ISNULL(@ActualCard, 0), ISNULL(@ActualCard, 0) - @expectedCard);

        INSERT INTO dbo.DayCloseV2Denomination
            (DayCloseId, TwoThousand, Thousand, FiveHundred, TwoHundred, Hundred, Fifty, Twenty, Ten, Five, Two, One, DenominationTotal)
        VALUES
            (@closeId, ISNULL(@TwoThousand, 0), ISNULL(@Thousand, 0), ISNULL(@FiveHundred, 0), ISNULL(@TwoHundred, 0),
             ISNULL(@Hundred, 0), ISNULL(@Fifty, 0), ISNULL(@Twenty, 0), ISNULL(@Ten, 0), ISNULL(@Five, 0),
             ISNULL(@Two, 0), ISNULL(@One, 0), @actualCash);

        INSERT INTO dbo.DayCloseV2DenominationLine
            (DayCloseId, FaceValue, OpeningQty, InQty, OutQty, ExpectedQty, ActualQty)
        SELECT @closeId, f.FaceValue, f.OpenQty, ISNULL(m.InQty, 0), ISNULL(m.OutQty, 0),
            f.OpenQty + ISNULL(m.InQty, 0) - ISNULL(m.OutQty, 0), f.ActualQty
        FROM (VALUES
            (2000, ISNULL(@OpenTwoThousand, 0), ISNULL(@TwoThousand, 0)),
            (1000, ISNULL(@OpenThousand, 0), ISNULL(@Thousand, 0)),
            (500, ISNULL(@OpenFiveHundred, 0), ISNULL(@FiveHundred, 0)),
            (200, ISNULL(@OpenTwoHundred, 0), ISNULL(@TwoHundred, 0)),
            (100, ISNULL(@OpenHundred, 0), ISNULL(@Hundred, 0)),
            (50, ISNULL(@OpenFifty, 0), ISNULL(@Fifty, 0)),
            (20, ISNULL(@OpenTwenty, 0), ISNULL(@Twenty, 0)),
            (10, ISNULL(@OpenTen, 0), ISNULL(@Ten, 0)),
            (5, ISNULL(@OpenFive, 0), ISNULL(@Five, 0)),
            (2, ISNULL(@OpenTwo, 0), ISNULL(@Two, 0)),
            (1, ISNULL(@OpenOne, 0), ISNULL(@One, 0))
        ) f (FaceValue, OpenQty, ActualQty)
        LEFT JOIN @move m ON m.FaceValue = f.FaceValue;
    COMMIT TRAN;

    SET @DayCloseId = @closeId;
    SET @Result = 1;
    SET @Message = 'Day Close saved.';
END
GO
