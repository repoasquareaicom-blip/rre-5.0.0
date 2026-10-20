/*
  Day Close V2 reconciliation layer.

  Does not change Cash, UPI, or Card expected-amount formulas.
  A closed day returns the saved summary.
  Opening cash for a later day remains the previous ActualCashClosing.
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

IF OBJECT_ID('dbo.DayCloseV2DenominationLine', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DayCloseV2DenominationLine
    (
        Id int IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        DayCloseId int NOT NULL,
        FaceValue int NOT NULL,
        OpeningQty int NOT NULL,
        InQty int NOT NULL,
        OutQty int NOT NULL,
        ExpectedQty int NOT NULL,
        ActualQty int NOT NULL,
        CONSTRAINT FK_DayCloseV2DenominationLine_Header FOREIGN KEY (DayCloseId) REFERENCES dbo.DayCloseV2(Id)
    );
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetExpectedSummary', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetExpectedSummary AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetExpectedSummary
(
    @BusinessDate date
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.DayCloseV2 WHERE BusinessDate = @BusinessDate AND Status = 'Completed')
    BEGIN
        SELECT
            1 AS HasPreviousClose,
            1 AS IsClosed,
            h.OpeningCash,
            cash.TotalIn AS CashIn,
            cash.TotalOut AS CashOut,
            cash.ExpectedNet AS ExpectedCash,
            upi.TotalIn AS UpiIn,
            upi.TotalOut AS UpiOut,
            upi.ExpectedNet AS ExpectedUpi,
            card.TotalIn AS CardIn,
            card.TotalOut AS CardOut,
            card.ExpectedNet AS ExpectedCard,
            h.ActualCashClosing,
            h.ActualUpiNet,
            h.ActualCardNet,
            h.Remarks
        FROM dbo.DayCloseV2 h
        INNER JOIN dbo.DayCloseV2PaymentSummary cash ON cash.DayCloseId = h.Id AND cash.PaymentMode = 'CASH'
        INNER JOIN dbo.DayCloseV2PaymentSummary upi ON upi.DayCloseId = h.Id AND upi.PaymentMode = 'UPI'
        INNER JOIN dbo.DayCloseV2PaymentSummary card ON card.DayCloseId = h.Id AND card.PaymentMode = 'CARD'
        WHERE h.BusinessDate = @BusinessDate AND h.Status = 'Completed';
        RETURN;
    END

    DECLARE @opening decimal(18, 2);
    DECLARE @hasPrevious bit;
    DECLARE @cashIn decimal(18, 2);
    DECLARE @cashOut decimal(18, 2);
    DECLARE @upiIn decimal(18, 2);
    DECLARE @upiOut decimal(18, 2);
    DECLARE @cardIn decimal(18, 2);
    DECLARE @cardOut decimal(18, 2);
    DECLARE @start datetime;
    DECLARE @end datetime;

    SET @start = @BusinessDate;
    SET @end = DATEADD(day, 1, @BusinessDate);

    SELECT TOP 1 @opening = ActualCashClosing
    FROM dbo.DayCloseV2
    WHERE Status = 'Completed' AND BusinessDate < @BusinessDate
    ORDER BY BusinessDate DESC;

    SET @hasPrevious = CASE WHEN @opening IS NULL THEN 0 ELSE 1 END;
    IF @opening IS NULL SET @opening = 0;

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

    SELECT
        @hasPrevious AS HasPreviousClose,
        0 AS IsClosed,
        @opening AS OpeningCash,
        @cashIn AS CashIn,
        @cashOut AS CashOut,
        @opening + @cashIn - @cashOut AS ExpectedCash,
        @upiIn AS UpiIn,
        @upiOut AS UpiOut,
        @upiIn - @upiOut AS ExpectedUpi,
        @cardIn AS CardIn,
        @cardOut AS CardOut,
        @cardIn - @cardOut AS ExpectedCard,
        CAST(NULL AS decimal(18, 2)) AS ActualCashClosing,
        CAST(NULL AS decimal(18, 2)) AS ActualUpiNet,
        CAST(NULL AS decimal(18, 2)) AS ActualCardNet,
        CAST(NULL AS nvarchar(500)) AS Remarks;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetPaymentLedger', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetPaymentLedger AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetPaymentLedger
(
    @BusinessDate date,
    @Filter varchar(20),
    @OpeningCash decimal(18, 2)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @start datetime;
    DECLARE @end datetime;
    SET @start = @BusinessDate;
    SET @end = DATEADD(day, 1, @BusinessDate);
    IF @Filter IS NULL OR @Filter = '' SET @Filter = 'ALL';
    IF @OpeningCash IS NULL SET @OpeningCash = 0;

    ;WITH raw AS
    (
        SELECT
            c.TransDate AS TransTime,
            'CASH' AS Channel,
            CASE WHEN UPPER(c.Type) = 'D' THEN 'OUT' ELSE 'IN' END AS Direction,
            c.EntityId AS EntityReference,
            c.EntitySource AS EntityType,
            ISNULL(q.customername, '') AS Description,
            CASE WHEN UPPER(c.Type) = 'D' THEN CONVERT(decimal(18, 2), 0) ELSE CONVERT(decimal(18, 2), REPLACE(c.Amount, ',', '')) END AS AmountIn,
            CASE WHEN UPPER(c.Type) = 'D' THEN CONVERT(decimal(18, 2), REPLACE(c.Amount, ',', '')) ELSE CONVERT(decimal(18, 2), 0) END AS AmountOut
        FROM CashTransaction c
        LEFT JOIN QuotationEstimation q ON q.Estimationid = c.EntityId
        WHERE c.TransDate >= @start AND c.TransDate < @end

        UNION ALL

        SELECT
            t.TransDate,
            CASE WHEN t.Mode = 'UPI' THEN 'UPI' ELSE 'CARD' END,
            CASE WHEN t.Type = 'D' THEN 'OUT' ELSE 'IN' END,
            t.Entity,
            t.EntitySource,
            ISNULL(q.customername, ISNULL(t.Reference, '')),
            CASE WHEN t.Type = 'D' THEN CONVERT(decimal(18, 2), 0) ELSE CONVERT(decimal(18, 2), t.Amount) END,
            CASE WHEN t.Type = 'D' THEN CONVERT(decimal(18, 2), t.Amount) ELSE CONVERT(decimal(18, 2), 0) END
        FROM CardTransactions t
        LEFT JOIN QuotationEstimation q ON q.Estimationid = t.Entity
        WHERE t.TransDate >= @start AND t.TransDate < @end
          AND t.Mode IN ('UPI', 'Card')
    ),
    numbered AS
    (
        SELECT raw.*, ROW_NUMBER() OVER (ORDER BY TransTime, Channel, EntityReference) AS Seq
        FROM raw
    ),
    balanced AS
    (
        SELECT numbered.*,
            @OpeningCash + SUM(CASE WHEN Channel = 'CASH' THEN AmountIn - AmountOut ELSE 0 END)
                OVER (ORDER BY TransTime, Seq ROWS UNBOUNDED PRECEDING) AS CashRunning,
            SUM(CASE WHEN Channel = 'UPI' THEN AmountIn - AmountOut ELSE 0 END)
                OVER (ORDER BY TransTime, Seq ROWS UNBOUNDED PRECEDING) AS UpiRunning,
            SUM(CASE WHEN Channel = 'CARD' THEN AmountIn - AmountOut ELSE 0 END)
                OVER (ORDER BY TransTime, Seq ROWS UNBOUNDED PRECEDING) AS CardRunning
        FROM numbered
    )
    SELECT
        TransTime,
        Channel,
        Direction,
        EntityReference,
        EntityType,
        Description,
        AmountIn,
        AmountOut,
        CASE Channel WHEN 'CASH' THEN CashRunning WHEN 'UPI' THEN UpiRunning ELSE CardRunning END AS RunningBalance
    FROM balanced
    WHERE @Filter = 'ALL' OR (Channel + ' ' + Direction) = @Filter
    ORDER BY TransTime, Seq;
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

    SELECT TOP 1 @prevId = Id
    FROM dbo.DayCloseV2
    WHERE Status = 'Completed' AND BusinessDate < @BusinessDate
    ORDER BY BusinessDate DESC;

    ;WITH notes AS
    (
        SELECT UPPER(LTRIM(RTRIM(c.Type))) AS PayType, 2000 AS FaceValue,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.TwoThousand)), '')), 0)) AS Qty
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 1000,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Thousand)), '')), 0))
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 500,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.FiveHundred)), '')), 0))
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 200,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.TwoHundred)), '')), 0))
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 100,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Hundred)), '')), 0))
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 50,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Fifty)), '')), 0))
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 20,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Twenty)), '')), 0))
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 10,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Ten)), '')), 0))
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 5,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Five)), '')), 0))
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 2,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Two)), '')), 0))
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 1,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.One)), '')), 0))
        FROM CashTransactionDenomination d
        INNER JOIN CashTransaction c ON c.TransId = d.TransId
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
    opening AS
    (
        SELECT 2000 AS FaceValue, TwoThousand AS Qty FROM dbo.DayCloseV2Denomination WHERE DayCloseId = @prevId
        UNION ALL SELECT 1000, Thousand FROM dbo.DayCloseV2Denomination WHERE DayCloseId = @prevId
        UNION ALL SELECT 500, FiveHundred FROM dbo.DayCloseV2Denomination WHERE DayCloseId = @prevId
        UNION ALL SELECT 200, TwoHundred FROM dbo.DayCloseV2Denomination WHERE DayCloseId = @prevId
        UNION ALL SELECT 100, Hundred FROM dbo.DayCloseV2Denomination WHERE DayCloseId = @prevId
        UNION ALL SELECT 50, Fifty FROM dbo.DayCloseV2Denomination WHERE DayCloseId = @prevId
        UNION ALL SELECT 20, Twenty FROM dbo.DayCloseV2Denomination WHERE DayCloseId = @prevId
        UNION ALL SELECT 10, Ten FROM dbo.DayCloseV2Denomination WHERE DayCloseId = @prevId
        UNION ALL SELECT 5, Five FROM dbo.DayCloseV2Denomination WHERE DayCloseId = @prevId
        UNION ALL SELECT 2, Two FROM dbo.DayCloseV2Denomination WHERE DayCloseId = @prevId
        UNION ALL SELECT 1, One FROM dbo.DayCloseV2Denomination WHERE DayCloseId = @prevId
    )
    SELECT
        f.FaceValue,
        ISNULL(o.Qty, 0) AS OpeningQty,
        ISNULL(m.InQty, 0) AS InQty,
        ISNULL(m.OutQty, 0) AS OutQty,
        ISNULL(o.Qty, 0) + ISNULL(m.InQty, 0) - ISNULL(m.OutQty, 0) AS ExpectedQty,
        CAST(NULL AS int) AS ActualQty
    FROM faces f
    LEFT JOIN movement m ON m.FaceValue = f.FaceValue
    LEFT JOIN opening o ON o.FaceValue = f.FaceValue
    ORDER BY f.FaceValue DESC;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetStockMovement', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetStockMovement AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetStockMovement
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

    SELECT
        mt.TranscationDate AS TransTime,
        mt.MaterailId AS ProductId,
        ISNULL(pm.DisplayName, '') AS Product,
        CASE
            WHEN lm.LocationName IS NOT NULL AND LTRIM(RTRIM(lm.LocationName)) <> '' THEN lm.LocationName
            ELSE 'Unknown Location'
        END AS Location,
        CASE
            WHEN rm.RackCaption IS NOT NULL AND LTRIM(RTRIM(rm.RackCaption)) <> '' THEN rm.RackCaption
            ELSE 'No Rack / Unassigned'
        END AS Rack,
        UPPER(LTRIM(RTRIM(mt.Type))) AS Movement,
        mt.Quantity,
        mt.TransId AS EntityReference,
        mt.TranscationType AS EntityType
    FROM dbo.MaterialTranscation mt
    LEFT JOIN dbo.ProductMaster pm ON pm.id = mt.MaterailId
    LEFT JOIN dbo.LocationMaster lm ON CONVERT(varchar(100), lm.LocationId) = CONVERT(varchar(100), mt.LocationId)
    LEFT JOIN dbo.RackMaster rm ON rm.RackId = mt.RackId
    WHERE mt.TranscationDate >= @start
      AND mt.TranscationDate < @end
    ORDER BY mt.TranscationDate, mt.ID;
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
    @OpenOne int = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Result = 0;
    SET @Message = '';

    IF EXISTS (SELECT 1 FROM dbo.DayCloseV2 WHERE BusinessDate = @BusinessDate AND Status = 'Completed')
    BEGIN
        SET @Message = 'This business date is already closed.';
        RETURN;
    END

    DECLARE @hasPrevious bit;
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

    SET @start = @BusinessDate;
    SET @end = DATEADD(day, 1, @BusinessDate);

    SELECT TOP 1 @opening = ActualCashClosing, @prevId = Id
    FROM dbo.DayCloseV2
    WHERE Status = 'Completed' AND BusinessDate < @BusinessDate
    ORDER BY BusinessDate DESC;

    SET @hasPrevious = CASE WHEN @opening IS NULL THEN 0 ELSE 1 END;
    IF @hasPrevious = 0
    BEGIN
        IF @OpeningCash IS NULL OR @OpeningCash < 0
        BEGIN
            SET @Message = 'Enter the opening cash for the first Day Close.';
            RETURN;
        END
        SET @opening = @OpeningCash;
        SET @openTotal =
            (ISNULL(@OpenTwoThousand, 0) * 2000) + (ISNULL(@OpenThousand, 0) * 1000) + (ISNULL(@OpenFiveHundred, 0) * 500)
            + (ISNULL(@OpenTwoHundred, 0) * 200) + (ISNULL(@OpenHundred, 0) * 100) + (ISNULL(@OpenFifty, 0) * 50)
            + (ISNULL(@OpenTwenty, 0) * 20) + (ISNULL(@OpenTen, 0) * 10) + (ISNULL(@OpenFive, 0) * 5)
            + (ISNULL(@OpenTwo, 0) * 2) + ISNULL(@OpenOne, 0);
        IF ABS(@openTotal - @opening) > 0.009
        BEGIN
            SET @Message = 'Opening denomination total must equal the opening cash amount.';
            RETURN;
        END
    END
    ELSE
    BEGIN
        SELECT
            @OpenTwoThousand = TwoThousand, @OpenThousand = Thousand, @OpenFiveHundred = FiveHundred,
            @OpenTwoHundred = TwoHundred, @OpenHundred = Hundred, @OpenFifty = Fifty,
            @OpenTwenty = Twenty, @OpenTen = Ten, @OpenFive = Five, @OpenTwo = Two, @OpenOne = One
        FROM dbo.DayCloseV2Denomination
        WHERE DayCloseId = @prevId;
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
        SELECT UPPER(LTRIM(RTRIM(c.Type))) AS PayType, 2000 AS FaceValue,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.TwoThousand)), '')), 0)) AS Qty
        FROM CashTransactionDenomination d INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 1000, CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Thousand)), '')), 0))
        FROM CashTransactionDenomination d INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 500, CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.FiveHundred)), '')), 0))
        FROM CashTransactionDenomination d INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 200, CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.TwoHundred)), '')), 0))
        FROM CashTransactionDenomination d INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 100, CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Hundred)), '')), 0))
        FROM CashTransactionDenomination d INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 50, CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Fifty)), '')), 0))
        FROM CashTransactionDenomination d INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 20, CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Twenty)), '')), 0))
        FROM CashTransactionDenomination d INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 10, CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Ten)), '')), 0))
        FROM CashTransactionDenomination d INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 5, CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Five)), '')), 0))
        FROM CashTransactionDenomination d INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 2, CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.Two)), '')), 0))
        FROM CashTransactionDenomination d INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
        UNION ALL
        SELECT UPPER(LTRIM(RTRIM(c.Type))), 1, CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(d.One)), '')), 0))
        FROM CashTransactionDenomination d INNER JOIN CashTransaction c ON c.TransId = d.TransId
        WHERE c.TransDate >= @start AND c.TransDate < @end
    ) s
    GROUP BY FaceValue;

    BEGIN TRAN;
        INSERT INTO dbo.DayCloseV2
            (BusinessDate, OpeningCash, ExpectedCashClosing, ActualCashClosing, CashDifference,
             ExpectedUpiNet, ActualUpiNet, UpiDifference, ExpectedCardNet, ActualCardNet, CardDifference,
             ExpectedTotal, ActualTotal, TotalDifference, Status, Remarks, EnteredBy, EnteredOn)
        VALUES
            (@BusinessDate, @opening, @expectedCash, @actualCash, @actualCash - @expectedCash,
             @expectedUpi, ISNULL(@ActualUpi, 0), ISNULL(@ActualUpi, 0) - @expectedUpi,
             @expectedCard, ISNULL(@ActualCard, 0), ISNULL(@ActualCard, 0) - @expectedCard,
             @expectedCash + @expectedUpi + @expectedCard,
             @actualCash + ISNULL(@ActualUpi, 0) + ISNULL(@ActualCard, 0),
             (@actualCash + ISNULL(@ActualUpi, 0) + ISNULL(@ActualCard, 0)) - (@expectedCash + @expectedUpi + @expectedCard),
             'Completed', @Remarks, @EnteredBy, GETDATE());

        DECLARE @closeId int;
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
        SELECT @closeId, f.FaceValue,
            CASE f.FaceValue
                WHEN 2000 THEN ISNULL(@OpenTwoThousand, 0) WHEN 1000 THEN ISNULL(@OpenThousand, 0)
                WHEN 500 THEN ISNULL(@OpenFiveHundred, 0) WHEN 200 THEN ISNULL(@OpenTwoHundred, 0)
                WHEN 100 THEN ISNULL(@OpenHundred, 0) WHEN 50 THEN ISNULL(@OpenFifty, 0)
                WHEN 20 THEN ISNULL(@OpenTwenty, 0) WHEN 10 THEN ISNULL(@OpenTen, 0)
                WHEN 5 THEN ISNULL(@OpenFive, 0) WHEN 2 THEN ISNULL(@OpenTwo, 0)
                ELSE ISNULL(@OpenOne, 0) END,
            ISNULL(m.InQty, 0),
            ISNULL(m.OutQty, 0),
            CASE f.FaceValue
                WHEN 2000 THEN ISNULL(@OpenTwoThousand, 0) WHEN 1000 THEN ISNULL(@OpenThousand, 0)
                WHEN 500 THEN ISNULL(@OpenFiveHundred, 0) WHEN 200 THEN ISNULL(@OpenTwoHundred, 0)
                WHEN 100 THEN ISNULL(@OpenHundred, 0) WHEN 50 THEN ISNULL(@OpenFifty, 0)
                WHEN 20 THEN ISNULL(@OpenTwenty, 0) WHEN 10 THEN ISNULL(@OpenTen, 0)
                WHEN 5 THEN ISNULL(@OpenFive, 0) WHEN 2 THEN ISNULL(@OpenTwo, 0)
                ELSE ISNULL(@OpenOne, 0) END + ISNULL(m.InQty, 0) - ISNULL(m.OutQty, 0),
            CASE f.FaceValue
                WHEN 2000 THEN ISNULL(@TwoThousand, 0) WHEN 1000 THEN ISNULL(@Thousand, 0)
                WHEN 500 THEN ISNULL(@FiveHundred, 0) WHEN 200 THEN ISNULL(@TwoHundred, 0)
                WHEN 100 THEN ISNULL(@Hundred, 0) WHEN 50 THEN ISNULL(@Fifty, 0)
                WHEN 20 THEN ISNULL(@Twenty, 0) WHEN 10 THEN ISNULL(@Ten, 0)
                WHEN 5 THEN ISNULL(@Five, 0) WHEN 2 THEN ISNULL(@Two, 0)
                ELSE ISNULL(@One, 0) END
        FROM (SELECT 2000 AS FaceValue UNION ALL SELECT 1000 UNION ALL SELECT 500 UNION ALL SELECT 200
              UNION ALL SELECT 100 UNION ALL SELECT 50 UNION ALL SELECT 20 UNION ALL SELECT 10
              UNION ALL SELECT 5 UNION ALL SELECT 2 UNION ALL SELECT 1) f
        LEFT JOIN @move m ON m.FaceValue = f.FaceValue;
    COMMIT TRAN;

    SET @Result = 1;
    SET @Message = 'Day Close saved.';
END
GO
