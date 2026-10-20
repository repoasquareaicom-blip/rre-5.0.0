/*
  Rs. 200 denomination and sales-bill cash out.

  Live sales-bill path:
    SalesBillNew -> QuotationBal.Savepayment -> SavePayment_Direct_Quotation
    SalesBillNew -> QuotationBal.SavepaymentDenomination -> SavePaymentDenomination

  Day close:
    Proc_GetSystemDenomination
    Proc_CashEndClose

  Adds TwoHundred only when the column is missing.
  Does not drop denomination tables and does not delete existing rows.
  Proc_CashEndClose copies denomination history with an explicit column list.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET LOCK_TIMEOUT 15000;

IF DB_NAME() IN ('master', 'model', 'msdb', 'tempdb')
BEGIN
    RAISERROR('Refusing to change a system database.', 16, 1);
    RETURN;
END

BEGIN TRAN;

IF COL_LENGTH('dbo.CashTransactionDenomination', 'TwoHundred') IS NULL
    ALTER TABLE dbo.CashTransactionDenomination ADD TwoHundred nvarchar(255) NULL;

IF COL_LENGTH('dbo.CashTransactionDenomination_History', 'TwoHundred') IS NULL
    ALTER TABLE dbo.CashTransactionDenomination_History ADD TwoHundred nvarchar(255) NULL;

IF COL_LENGTH('dbo.DayEndClose', 'TwoHundred') IS NULL
    ALTER TABLE dbo.DayEndClose ADD TwoHundred int NULL;

COMMIT TRAN;
GO

ALTER PROCEDURE dbo.SavePayment_Direct_Quotation
(
    @RequestId nvarchar(20) = null,
    @Amount nvarchar(100) = null,
    @OAmount nvarchar(100) = null,
    @OAmounts nvarchar(100) = null,
    @Thousand nvarchar(255) = null,
    @TwoThousand nvarchar(255) = null,
    @FiveHundred nvarchar(255) = null,
    @Hundred nvarchar(255) = null,
    @Fifty nvarchar(255) = null,
    @Twenty nvarchar(255) = null,
    @Ten nvarchar(255) = null,
    @Five nvarchar(255) = null,
    @Two nvarchar(255) = null,
    @One nvarchar(255) = null,
    @Result varchar(100) out,
    @TwoHundred nvarchar(255) = null
)
AS
BEGIN
    DECLARE @transid varchar(200);
    DECLARE @val varchar(max);
    DECLARE @val1 decimal;
    DECLARE @EN varchar(max);
    DECLARE @CashType nvarchar(20);
    DECLARE @StoredAmount nvarchar(100);

    SELECT @EN = GrnandTotal FROM QuotationEstimation WHERE Estimationid = @RequestId;
    SELECT @EN = REPLACE(@EN, ',', '');
    SELECT @EN = CAST(ROUND(@EN, 0, 1) AS DECIMAL(18, 0));
    SELECT @val1 = CAST(@EN AS bigint);

    SELECT @OAmount = REPLACE(@OAmount, ',', '');
    SELECT @OAmount = CAST(ROUND(@OAmount, 0, 1) AS DECIMAL(18, 0));
    SELECT @OAmount = CAST(@OAmount AS bigint);

    SELECT @val = CONVERT(numeric(18, 2), @val1 - @OAmount);

    SET @CashType = 'C';
    SET @StoredAmount = @OAmount;
    IF (CAST(@OAmount AS bigint) < 0)
    BEGIN
        SET @CashType = 'D';
        SET @StoredAmount = CAST(ABS(CAST(@OAmount AS bigint)) AS nvarchar(100));
    END

    INSERT INTO CashTransaction (TransId, EntityId, EntitySource, TransDate, Amount, Type)
    VALUES (NEWID(), @RequestId, 'ESTIMATION', GETDATE(), @StoredAmount, @CashType);

    SET @transid = (SELECT TOP 1 TransId FROM CashTransaction ORDER BY TransDate DESC);

    INSERT INTO CashTransactionDenomination
        (Id, TransId, Thousand, FiveHundred, Hundred, Fifty, Twenty, Ten, Five, Two, One, Amount, TwoThousand, TwoHundred)
    VALUES
        (NEWID(), @transid, @Thousand, @FiveHundred, @Hundred, @Fifty, @Twenty, @Ten, @Five, @Two, @One, @Amount, @TwoThousand, @TwoHundred);

    INSERT INTO Receipt (TransactionId, Transdate, EntityId, CustomerName, Amount, Mode)
    SELECT c.TransId, c.TransDate, c.EntityId, q.customername + '-' + q.City, c.Amount, 'Cash'
    FROM CashTransaction c
    JOIN QuotationEstimation q ON c.EntityId = q.Estimationid
    WHERE TransId = @transid;

    IF (@OAmounts = '0')
    BEGIN
        UPDATE QuotationEstimation SET Paid = @OAmount, Balance = CONVERT(varchar(20), @val), IsBilled = 1 WHERE Estimationid = @RequestId;
        UPDATE Estimation SET Paid = @OAmount, Balance = CONVERT(varchar(20), @val) WHERE Entityid = @RequestId;
        SET @Result = @transid;
    END
    ELSE
    BEGIN
        UPDATE QuotationEstimation SET Paid = @OAmounts, Balance = 0, IsBilled = 1 WHERE Estimationid = @RequestId;
        UPDATE Estimation SET Paid = @OAmounts, Balance = 0 WHERE Entityid = @RequestId;
        SET @Result = @transid;
    END
END
GO

ALTER PROCEDURE dbo.SavePaymentDenomination
(
    @TransId nvarchar(100),
    @TwoThousand nvarchar(255) = null,
    @Thousand nvarchar(255) = null,
    @FiveHundred nvarchar(255) = null,
    @Hundred nvarchar(255) = null,
    @Fifty nvarchar(255) = null,
    @Twenty nvarchar(255) = null,
    @Ten nvarchar(255) = null,
    @Five nvarchar(255) = null,
    @Coin nvarchar(255) = null,
    @one nvarchar(255) = null,
    @Amount nvarchar(255) = null,
    @Result int out,
    @TwoHundred nvarchar(255) = null
)
AS
BEGIN
    INSERT INTO CashTransactionDenomination
        (Id, TransId, Thousand, FiveHundred, Hundred, Fifty, Twenty, Ten, Five, Two, One, Amount, Twothousand, TwoHundred)
    VALUES
        (NEWID(), @TransId, @Thousand, @FiveHundred, @Hundred, @Fifty, @Twenty, @Ten, @Five, @Coin, @one, @Amount, @TwoThousand, @TwoHundred);
    SET @Result = 1;
END
GO

ALTER PROCEDURE dbo.Proc_GetSystemDenomination
(
    @TransId uniqueidentifier = null
)
AS
BEGIN
    DECLARE @CashTransaction TABLE
    (
        Transid varchar(max),
        EntityId varchar(100),
        Transdate date,
        Amount varchar(100),
        Type varchar(100),
        TwoThousand varchar(100),
        Thousand varchar(100),
        FiveHundred varchar(100),
        TwoHundred varchar(100),
        Hundred varchar(100),
        Fifty varchar(100),
        Twenty varchar(100),
        Ten varchar(100),
        Five varchar(100),
        Two varchar(100),
        One varchar(100)
    );

    DECLARE @CashTransaction1 TABLE
    (
        Id int,
        Type varchar(100),
        TwoThousand varchar(100),
        Thousand varchar(100),
        FiveHundred varchar(100),
        TwoHundred varchar(100),
        Hundred varchar(100),
        Fifty varchar(100),
        Twenty varchar(100),
        Ten varchar(100),
        Five varchar(100),
        Two varchar(100),
        One varchar(100)
    );

    INSERT INTO @CashTransaction (TransId, EntityId, TransDate, Amount, Type)
    SELECT TransId, EntityId, TransDate, Amount, Type FROM CashTransaction WHERE Type = 'C';

    INSERT INTO @CashTransaction (TransId, EntityId, TransDate, Amount, Type)
    SELECT TransId, EntityId, TransDate, Amount, Type FROM CashTransaction WHERE Type = 'D';

    UPDATE a SET TwoThousand = c.somePrice
    FROM @CashTransaction a
    INNER JOIN (
        SELECT TransId, SUM(ISNULL(CONVERT(decimal(18, 2), TwoThousand), 0)) somePrice
        FROM CashTransactionDenomination GROUP BY TransId
    ) c ON a.Transid = c.TransId;

    UPDATE a SET Thousand = c.somePrice
    FROM @CashTransaction a
    INNER JOIN (
        SELECT TransId, SUM(ISNULL(CONVERT(decimal(18, 2), Thousand), 0)) somePrice
        FROM CashTransactionDenomination GROUP BY TransId
    ) c ON a.Transid = c.TransId;

    UPDATE a SET FiveHundred = c.somePrice
    FROM @CashTransaction a
    INNER JOIN (
        SELECT TransId, SUM(ISNULL(CONVERT(decimal(18, 2), FiveHundred), 0)) somePrice
        FROM CashTransactionDenomination GROUP BY TransId
    ) c ON a.Transid = c.TransId;

    UPDATE a SET TwoHundred = c.somePrice
    FROM @CashTransaction a
    INNER JOIN (
        SELECT TransId, SUM(ISNULL(CONVERT(decimal(18, 2), TwoHundred), 0)) somePrice
        FROM CashTransactionDenomination GROUP BY TransId
    ) c ON a.Transid = c.TransId;

    UPDATE a SET Hundred = c.somePrice
    FROM @CashTransaction a
    INNER JOIN (
        SELECT TransId, SUM(ISNULL(CONVERT(decimal(18, 2), Hundred), 0)) somePrice
        FROM CashTransactionDenomination GROUP BY TransId
    ) c ON a.Transid = c.TransId;

    UPDATE a SET Fifty = c.somePrice
    FROM @CashTransaction a
    INNER JOIN (
        SELECT TransId, SUM(ISNULL(CONVERT(decimal(18, 2), Fifty), 0)) somePrice
        FROM CashTransactionDenomination GROUP BY TransId
    ) c ON a.Transid = c.TransId;

    UPDATE a SET Twenty = c.somePrice
    FROM @CashTransaction a
    INNER JOIN (
        SELECT TransId, SUM(ISNULL(CONVERT(decimal(18, 2), Twenty), 0)) somePrice
        FROM CashTransactionDenomination GROUP BY TransId
    ) c ON a.Transid = c.TransId;

    UPDATE a SET Ten = c.somePrice
    FROM @CashTransaction a
    INNER JOIN (
        SELECT TransId, SUM(ISNULL(CONVERT(decimal(18, 2), Ten), 0)) somePrice
        FROM CashTransactionDenomination GROUP BY TransId
    ) c ON a.Transid = c.TransId;

    UPDATE a SET Five = c.somePrice
    FROM @CashTransaction a
    INNER JOIN (
        SELECT TransId, SUM(ISNULL(CONVERT(decimal(18, 2), Five), 0)) somePrice
        FROM CashTransactionDenomination GROUP BY TransId
    ) c ON a.Transid = c.TransId;

    UPDATE a SET Two = c.somePrice
    FROM @CashTransaction a
    INNER JOIN (
        SELECT TransId, SUM(ISNULL(CONVERT(decimal(18, 2), Two), 0)) somePrice
        FROM CashTransactionDenomination GROUP BY TransId
    ) c ON a.Transid = c.TransId;

    UPDATE a SET One = c.somePrice
    FROM @CashTransaction a
    INNER JOIN (
        SELECT TransId, SUM(ISNULL(CONVERT(decimal(18, 2), One), 0)) somePrice
        FROM CashTransactionDenomination GROUP BY TransId
    ) c ON a.Transid = c.TransId;

    INSERT INTO @CashTransaction1
    SELECT ROW_NUMBER() OVER (ORDER BY type DESC),
           type,
           SUM(CONVERT(decimal(18, 2), TwoThousand)),
           SUM(CONVERT(decimal(18, 2), Thousand)),
           SUM(CONVERT(decimal(18, 2), FiveHundred)),
           SUM(CONVERT(decimal(18, 2), TwoHundred)),
           SUM(CONVERT(decimal(18, 2), Hundred)),
           SUM(CONVERT(decimal(18, 2), Fifty)),
           SUM(CONVERT(decimal(18, 2), Twenty)),
           SUM(CONVERT(decimal(18, 2), Ten)),
           SUM(CONVERT(decimal(18, 2), Five)),
           SUM(CONVERT(decimal(18, 2), Two)),
           SUM(CONVERT(decimal(18, 2), One))
    FROM @CashTransaction
    GROUP BY type;

    DECLARE @rowcount int;
    SET @rowcount = (SELECT COUNT(*) FROM @CashTransaction1);

    IF (@rowcount = 1)
    BEGIN
        SELECT * FROM @CashTransaction1;
    END
    ELSE
    BEGIN
        SELECT
            (SUM(CONVERT(decimal(18, 2), TableA.TwoThousand)) - SUM(CONVERT(decimal(18, 2), TableB.TwoThousand))) AS TwoThousand,
            (SUM(CONVERT(decimal(18, 2), TableA.Thousand)) - SUM(CONVERT(decimal(18, 2), TableB.Thousand))) AS Thousand,
            (SUM(CONVERT(decimal(18, 2), TableA.FiveHundred)) - SUM(CONVERT(decimal(18, 2), TableB.FiveHundred))) AS FiveHundred,
            (SUM(CONVERT(decimal(18, 2), TableA.TwoHundred)) - SUM(CONVERT(decimal(18, 2), TableB.TwoHundred))) AS TwoHundred,
            (SUM(CONVERT(decimal(18, 2), TableA.Hundred)) - SUM(CONVERT(decimal(18, 2), TableB.Hundred))) AS Hundred,
            (SUM(CONVERT(decimal(18, 2), TableA.Fifty)) - SUM(CONVERT(decimal(18, 2), TableB.Fifty))) AS Fifty,
            (SUM(CONVERT(decimal(18, 2), TableA.Twenty)) - SUM(CONVERT(decimal(18, 2), TableB.Twenty))) AS Twenty,
            (SUM(CONVERT(decimal(18, 2), TableA.Ten)) - SUM(CONVERT(decimal(18, 2), TableB.Ten))) AS Ten,
            (SUM(CONVERT(decimal(18, 2), TableA.Five)) - SUM(CONVERT(decimal(18, 2), TableB.Five))) AS Five,
            (SUM(CONVERT(decimal(18, 2), TableA.Two)) - SUM(CONVERT(decimal(18, 2), TableB.Two))) AS Two,
            (SUM(CONVERT(decimal(18, 2), TableA.One)) - SUM(CONVERT(decimal(18, 2), TableB.One))) AS One
        FROM @CashTransaction1 TableA
        INNER JOIN @CashTransaction1 TableB ON TableB.ID = TableA.ID - 1;
    END
END
GO

ALTER PROCEDURE dbo.Proc_CashEndClose
(
    @Id varchar = null,
    @AccountingDate datetime,
    @GrossBalance decimal,
    @Type char(1),
    @Adjustment nvarchar(250),
    @ClosingBalance decimal,
    @TwoThousand int,
    @Thousand int,
    @FiveHundred int,
    @Hundred int,
    @Fifty int,
    @Twenty int,
    @Ten int,
    @Five int,
    @Two int,
    @One int,
    @DTwoThousand int,
    @DThousand int,
    @DFiveHundred int,
    @DHundred int,
    @DFifty int,
    @DTwenty int,
    @DTen int,
    @DFive int,
    @DTwo int,
    @DOne int,
    @Remarks varchar(max),
    @UserId int,
    @OutId int OUT,
    @Result int OUT,
    @TwoHundred int = NULL,
    @DTwoHundred int = NULL
)
AS
BEGIN
    INSERT INTO DayEndClose
        (AccountingDate, GrossBalance, Type, Adjustment, ClosingBalance, Thousand, FiveHundred, Hundred, Fifty, Twenty, Ten, Five, Two, One, Remarks, EnteredBy, EnteredOn, TwoThousand, TwoHundred)
    VALUES
        (@AccountingDate, @GrossBalance, @Type, @Adjustment, @ClosingBalance, @Thousand, @FiveHundred, @Hundred, @Fifty, @Twenty, @Ten, @Five, @Two, @One, @Remarks, @UserId, GETDATE(), @TwoThousand, @TwoHundred);

    IF EXISTS (SELECT TOP 1 Id FROM CashTransactionDenomination)
    BEGIN
        BEGIN TRAN
            INSERT INTO CashTransactionDenomination_History
                (Id, TransId, Amount, Thousand, FiveHundred, Hundred, Fifty, Twenty, Ten, Five, Two, One, TwoThousand, TwoHundred)
            SELECT
                Id, TransId, Amount, Thousand, FiveHundred, Hundred, Fifty, Twenty, Ten, Five, Two, One, TwoThousand, TwoHundred
            FROM CashTransactionDenomination;

            DELETE FROM CashTransactionDenomination
            WHERE TransId IN (SELECT TransId FROM CashTransactionDenomination_History);
        COMMIT TRAN
    END

    INSERT INTO dbo.CashTransactionDenomination
        (Id, TransId, Amount, Thousand, FiveHundred, Hundred, Fifty, Twenty, Ten, Five, Two, One, TwoThousand, TwoHundred)
    VALUES
        (NEWID(), NEWID(), @ClosingBalance, @DThousand, @DFiveHundred, @DHundred, @DFifty, @DTwenty, @DTen, @DFive, @DTwo, @DOne, @DTwoThousand, @DTwoHundred);

    SET @OutId = (SELECT MAX(Id) FROM DayEndClose);
    SET @Result = 1;
END
GO
