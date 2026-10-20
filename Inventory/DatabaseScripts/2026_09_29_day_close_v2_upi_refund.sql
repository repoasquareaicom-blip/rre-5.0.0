/*
  Day Close V2 and UPI refund.

  Does not drop or alter the existing Day Close tables or procedures,
  except GetRRECash, which keeps counting Card/UPI receipts only.
  Existing CardTransactions.Type C rows stay receipts.
  A UPI refund is a new Type D row with a positive amount.
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

IF COL_LENGTH('dbo.CardTransactions', 'Type') IS NULL
BEGIN
    RAISERROR('CardTransactions.Type is missing. No change was made.', 16, 1);
    RETURN;
END
GO

ALTER PROCEDURE dbo.GetRRECash
AS
BEGIN
    SELECT EntitySource AS Type, CONVERT(numeric(18,2), ROUND(CONVERT(nvarchar(250), Amount), 0)) AS Amount, TransId
    INTO #1
    FROM CashTransaction
    WHERE IsDayEndClosed IS NULL AND DayEndCloseId IS NULL AND UPPER(Type) = 'C'
    ORDER BY TransDate DESC;

    SELECT EntitySource AS Type, CONVERT(numeric(18,2), ROUND(CONVERT(nvarchar(250), Amount), 0)) AS Amount, TransId
    INTO #2
    FROM CashTransaction
    WHERE IsDayEndClosed IS NULL AND DayEndCloseId IS NULL AND UPPER(Type) = 'D'
    ORDER BY TransDate DESC;

    SELECT [Type] 'CashType', SUM(Amount) Amount, 'In' 'Type' FROM #1 GROUP BY [Type]
    UNION ALL
    SELECT [Type] 'CashType', SUM(Amount) Amount, 'Out' 'Type' FROM #2 GROUP BY [Type]
    UNION ALL
    SELECT 'SALES' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), GrandTotal)), 0) 'Amount', 'Sales' 'Type'
    FROM Sales
    WHERE CONVERT(varchar(10), Updatedon, 120) = CONVERT(varchar(10), GETDATE(), 120)
    UNION ALL
    SELECT 'CASH' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), c.Amount)), 0) 'Amount', 'CashBill' 'Type'
    FROM QuotationEstimation q
    LEFT JOIN CashTransaction c ON q.Estimationid = c.EntityId
    WHERE Paymentmode = 'Cash Bill'
      AND CONVERT(varchar(10), date, 120) = CONVERT(varchar(10), GETDATE(), 120)
      AND CAST(Balance AS decimal) = 0
    UNION ALL
    SELECT 'CARD' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), Amount)), 0) 'Amount', 'CardBill' 'Type'
    FROM CardTransactions
    WHERE CONVERT(varchar(10), TransDate, 120) = CONVERT(varchar(10), GETDATE(), 120)
      AND ISNULL(Mode, '') <> 'UPI'
      AND ISNULL(Type, 'C') = 'C'
    UNION ALL
    SELECT 'UPI' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), Amount)), 0) 'Amount', 'UpiBill' 'Type'
    FROM CardTransactions
    WHERE CONVERT(varchar(10), TransDate, 120) = CONVERT(varchar(10), GETDATE(), 120)
      AND Mode = 'UPI'
      AND ISNULL(Type, 'C') = 'C'
    UNION ALL
    SELECT 'PARTIAL' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), c.Amount)), 0) 'Amount', 'CashBill' 'Type'
    FROM QuotationEstimation q
    LEFT JOIN CashTransaction c ON q.Estimationid = c.EntityId
    WHERE Paymentmode = 'Partial Credit Bill'
      AND CONVERT(varchar(10), date, 120) = CONVERT(varchar(10), GETDATE(), 120)
      AND CAST(Balance AS decimal) = 0
    UNION ALL
    SELECT 'CREDIT' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), GrnandTotal)), 0) 'Amount', 'CreditBill' 'Type'
    FROM QuotationEstimation
    WHERE Paymentmode = 'Credit Bill'
      AND CONVERT(varchar(10), date, 120) = CONVERT(varchar(10), GETDATE(), 120);
END
GO

IF OBJECT_ID('dbo.SaveUpiRefund', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.SaveUpiRefund AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.SaveUpiRefund
(
    @RequestId nvarchar(20),
    @Amount decimal(18, 2),
    @Reference nvarchar(30),
    @Result int OUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = 0;

    IF @Amount IS NULL OR @Amount <= 0
    BEGIN
        RAISERROR('UPI refund amount must be greater than zero.', 16, 1);
        RETURN;
    END
    IF @Reference IS NULL OR LTRIM(RTRIM(@Reference)) = ''
    BEGIN
        RAISERROR('UPI reference is required.', 16, 1);
        RETURN;
    END

    DECLARE @grand decimal(18, 2);
    DECLARE @signed bigint;
    DECLARE @balance numeric(18, 2);
    DECLARE @grandText varchar(max);

    IF NOT EXISTS (SELECT 1 FROM QuotationEstimation WHERE Estimationid = @RequestId)
    BEGIN
        RAISERROR('Sales bill was not found.', 16, 1);
        RETURN;
    END

    SELECT @grandText = GrnandTotal FROM QuotationEstimation WHERE Estimationid = @RequestId;
    SET @grandText = REPLACE(@grandText, ',', '');
    SET @grand = CAST(ROUND(@grandText, 0, 1) AS decimal(18, 0));
    IF @grand >= 0
    BEGIN
        RAISERROR('UPI refund is only for a negative sales bill.', 16, 1);
        RETURN;
    END
    IF ABS(@grand) <> CAST(ROUND(@Amount, 0) AS decimal(18, 0))
    BEGIN
        RAISERROR('UPI refund must equal the sales bill amount.', 16, 1);
        RETURN;
    END
    SET @signed = -CAST(ROUND(@Amount, 0) AS bigint);
    SET @balance = CONVERT(numeric(18, 2), @grand - @signed);

    INSERT INTO CardTransactions
        (TransId, Entity, EntitySource, TransDate, Amount, Type, Mode, InstrumentNumber, InstrumentBank, InstrumentDate, InstrumentSource, Reference)
    VALUES
        (NEWID(), @RequestId, 'ESTIMATION', GETDATE(), @Amount, 'D', 'UPI', '', '', GETDATE(), @Reference, @Reference);

    UPDATE QuotationEstimation
    SET Paid = CONVERT(varchar(20), @signed),
        Balance = CONVERT(varchar(20), @balance),
        IsBilled = 1
    WHERE Estimationid = @RequestId;

    UPDATE Estimation
    SET Paid = CONVERT(varchar(20), @signed),
        Balance = CONVERT(varchar(20), @balance)
    WHERE Entityid = @RequestId;

    SET @Result = 1;
END
GO

IF OBJECT_ID('dbo.DayCloseV2', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DayCloseV2
    (
        Id int IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        BusinessDate date NOT NULL,
        OpeningCash decimal(18, 2) NOT NULL,
        ExpectedCashClosing decimal(18, 2) NOT NULL,
        ActualCashClosing decimal(18, 2) NOT NULL,
        CashDifference decimal(18, 2) NOT NULL,
        ExpectedUpiNet decimal(18, 2) NOT NULL,
        ActualUpiNet decimal(18, 2) NOT NULL,
        UpiDifference decimal(18, 2) NOT NULL,
        ExpectedCardNet decimal(18, 2) NOT NULL,
        ActualCardNet decimal(18, 2) NOT NULL,
        CardDifference decimal(18, 2) NOT NULL,
        ExpectedTotal decimal(18, 2) NOT NULL,
        ActualTotal decimal(18, 2) NOT NULL,
        TotalDifference decimal(18, 2) NOT NULL,
        Status varchar(20) NOT NULL,
        Remarks nvarchar(500) NULL,
        EnteredBy int NULL,
        EnteredOn datetime NOT NULL,
        ReviewedBy int NULL,
        ReviewedOn datetime NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_DayCloseV2_CompletedDate' AND object_id = OBJECT_ID('dbo.DayCloseV2'))
    CREATE UNIQUE INDEX UX_DayCloseV2_CompletedDate ON dbo.DayCloseV2(BusinessDate) WHERE Status = 'Completed';
GO

IF OBJECT_ID('dbo.DayCloseV2PaymentSummary', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DayCloseV2PaymentSummary
    (
        Id int IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        DayCloseId int NOT NULL,
        PaymentMode varchar(10) NOT NULL,
        OpeningAmount decimal(18, 2) NOT NULL,
        TotalIn decimal(18, 2) NOT NULL,
        TotalOut decimal(18, 2) NOT NULL,
        ExpectedNet decimal(18, 2) NOT NULL,
        ActualAmount decimal(18, 2) NOT NULL,
        Difference decimal(18, 2) NOT NULL,
        CONSTRAINT FK_DayCloseV2PaymentSummary_Header FOREIGN KEY (DayCloseId) REFERENCES dbo.DayCloseV2(Id)
    );
END
GO

IF OBJECT_ID('dbo.DayCloseV2Denomination', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DayCloseV2Denomination
    (
        Id int IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        DayCloseId int NOT NULL,
        TwoThousand int NOT NULL,
        Thousand int NOT NULL,
        FiveHundred int NOT NULL,
        TwoHundred int NOT NULL,
        Hundred int NOT NULL,
        Fifty int NOT NULL,
        Twenty int NOT NULL,
        Ten int NOT NULL,
        Five int NOT NULL,
        Two int NOT NULL,
        One int NOT NULL,
        DenominationTotal decimal(18, 2) NOT NULL,
        CONSTRAINT FK_DayCloseV2Denomination_Header FOREIGN KEY (DayCloseId) REFERENCES dbo.DayCloseV2(Id)
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

    DECLARE @opening decimal(18, 2);
    DECLARE @hasPrevious bit;
    DECLARE @cashIn decimal(18, 2);
    DECLARE @cashOut decimal(18, 2);
    DECLARE @upiIn decimal(18, 2);
    DECLARE @upiOut decimal(18, 2);
    DECLARE @cardIn decimal(18, 2);
    DECLARE @cardOut decimal(18, 2);

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
    WHERE CONVERT(date, TransDate) = @BusinessDate;

    SELECT
        @upiIn = ISNULL(SUM(CASE WHEN ISNULL(Type, 'C') = 'C' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0),
        @upiOut = ISNULL(SUM(CASE WHEN Type = 'D' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0)
    FROM CardTransactions
    WHERE CONVERT(date, TransDate) = @BusinessDate AND Mode = 'UPI';

    SELECT
        @cardIn = ISNULL(SUM(CASE WHEN ISNULL(Type, 'C') = 'C' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0),
        @cardOut = ISNULL(SUM(CASE WHEN Type = 'D' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0)
    FROM CardTransactions
    WHERE CONVERT(date, TransDate) = @BusinessDate AND Mode = 'Card';

    SELECT
        @hasPrevious AS HasPreviousClose,
        @opening AS OpeningCash,
        @cashIn AS CashIn,
        @cashOut AS CashOut,
        @opening + @cashIn - @cashOut AS ExpectedCash,
        @upiIn AS UpiIn,
        @upiOut AS UpiOut,
        @upiIn - @upiOut AS ExpectedUpi,
        @cardIn AS CardIn,
        @cardOut AS CardOut,
        @cardIn - @cardOut AS ExpectedCard;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetTransactions', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetTransactions AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetTransactions
(
    @BusinessDate date,
    @PaymentMode varchar(10),
    @Direction varchar(3)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @PaymentMode = 'CASH'
    BEGIN
        SELECT c.TransDate, CONVERT(varchar(36), c.TransId) AS TransId, c.EntitySource,
               c.EntityId AS EstimationId, q.customername AS CustomerName,
               'CASH' AS PaymentMode,
               CASE WHEN UPPER(c.Type) = 'D' THEN 'OUT' ELSE 'IN' END AS Direction,
               CONVERT(decimal(18, 2), REPLACE(c.Amount, ',', '')) AS Amount,
               '' AS ReferenceNo
        FROM CashTransaction c
        LEFT JOIN QuotationEstimation q ON q.Estimationid = c.EntityId
        WHERE CONVERT(date, c.TransDate) = @BusinessDate
          AND ((@Direction = 'IN' AND UPPER(c.Type) = 'C') OR (@Direction = 'OUT' AND UPPER(c.Type) = 'D'))
        ORDER BY c.TransDate;
        RETURN;
    END

    SELECT t.TransDate, CONVERT(varchar(36), t.TransId) AS TransId, t.EntitySource,
           t.Entity AS EstimationId, q.customername AS CustomerName,
           CASE WHEN t.Mode = 'UPI' THEN 'UPI' ELSE 'CARD' END AS PaymentMode,
           CASE WHEN t.Type = 'D' THEN 'OUT' ELSE 'IN' END AS Direction,
           CONVERT(decimal(18, 2), t.Amount) AS Amount,
           ISNULL(t.Reference, ISNULL(t.InstrumentSource, '')) AS ReferenceNo
    FROM CardTransactions t
    LEFT JOIN QuotationEstimation q ON q.Estimationid = t.Entity
    WHERE CONVERT(date, t.TransDate) = @BusinessDate
      AND ((@PaymentMode = 'UPI' AND t.Mode = 'UPI') OR (@PaymentMode = 'CARD' AND t.Mode = 'Card'))
      AND ((@Direction = 'IN' AND ISNULL(t.Type, 'C') = 'C') OR (@Direction = 'OUT' AND t.Type = 'D'))
    ORDER BY t.TransDate;
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
    @Message nvarchar(400) OUT
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

    SELECT TOP 1 @opening = ActualCashClosing
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
    END

    SELECT
        @cashIn = ISNULL(SUM(CASE WHEN UPPER(Type) = 'C' THEN CONVERT(decimal(18, 2), REPLACE(Amount, ',', '')) ELSE 0 END), 0),
        @cashOut = ISNULL(SUM(CASE WHEN UPPER(Type) = 'D' THEN CONVERT(decimal(18, 2), REPLACE(Amount, ',', '')) ELSE 0 END), 0)
    FROM CashTransaction
    WHERE CONVERT(date, TransDate) = @BusinessDate;

    SELECT
        @upiIn = ISNULL(SUM(CASE WHEN ISNULL(Type, 'C') = 'C' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0),
        @upiOut = ISNULL(SUM(CASE WHEN Type = 'D' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0)
    FROM CardTransactions
    WHERE CONVERT(date, TransDate) = @BusinessDate AND Mode = 'UPI';

    SELECT
        @cardIn = ISNULL(SUM(CASE WHEN ISNULL(Type, 'C') = 'C' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0),
        @cardOut = ISNULL(SUM(CASE WHEN Type = 'D' THEN CONVERT(decimal(18, 2), Amount) ELSE 0 END), 0)
    FROM CardTransactions
    WHERE CONVERT(date, TransDate) = @BusinessDate AND Mode = 'Card';

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

        DECLARE @id int;
        SET @id = SCOPE_IDENTITY();

        INSERT INTO dbo.DayCloseV2PaymentSummary
            (DayCloseId, PaymentMode, OpeningAmount, TotalIn, TotalOut, ExpectedNet, ActualAmount, Difference)
        VALUES
            (@id, 'CASH', @opening, @cashIn, @cashOut, @expectedCash, @actualCash, @actualCash - @expectedCash),
            (@id, 'UPI', 0, @upiIn, @upiOut, @expectedUpi, ISNULL(@ActualUpi, 0), ISNULL(@ActualUpi, 0) - @expectedUpi),
            (@id, 'CARD', 0, @cardIn, @cardOut, @expectedCard, ISNULL(@ActualCard, 0), ISNULL(@ActualCard, 0) - @expectedCard);

        INSERT INTO dbo.DayCloseV2Denomination
            (DayCloseId, TwoThousand, Thousand, FiveHundred, TwoHundred, Hundred, Fifty, Twenty, Ten, Five, Two, One, DenominationTotal)
        VALUES
            (@id, ISNULL(@TwoThousand, 0), ISNULL(@Thousand, 0), ISNULL(@FiveHundred, 0), ISNULL(@TwoHundred, 0),
             ISNULL(@Hundred, 0), ISNULL(@Fifty, 0), ISNULL(@Twenty, 0), ISNULL(@Ten, 0), ISNULL(@Five, 0),
             ISNULL(@Two, 0), ISNULL(@One, 0), @actualCash);
    COMMIT TRAN;

    SET @Result = 1;
    SET @Message = 'Day Close saved.';
END
GO
