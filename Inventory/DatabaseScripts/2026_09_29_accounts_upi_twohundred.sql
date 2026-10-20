/*
  Accounts UPI channel and Rs. 200 denomination follow-up.

  Does not change Account Receivable, Payment Request, or Cash Receipt
  accounting procedures. Does not recreate Day Close V2 or TwoHundred columns.
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

IF OBJECT_ID('dbo.SaveAccountsTwoHundred', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.SaveAccountsTwoHundred AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.SaveAccountsTwoHundred
(
    @TransId varchar(100),
    @TwoHundred nvarchar(50) = NULL,
    @TwoHundredReturn nvarchar(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @TwoHundred IS NOT NULL
    BEGIN
        UPDATE CashTransactionDenomination
        SET TwoHundred = @TwoHundred
        WHERE Id = (
            SELECT TOP 1 d.Id
            FROM CashTransactionDenomination d
            WHERE d.TransId = @TransId
              AND d.TwoHundred IS NULL
              AND LEFT(LTRIM(ISNULL(d.Amount, '0')), 1) <> '-'
            ORDER BY d.Id
        );
    END

    IF @TwoHundredReturn IS NOT NULL
    BEGIN
        UPDATE CashTransactionDenomination
        SET TwoHundred = @TwoHundredReturn
        WHERE Id = (
            SELECT TOP 1 d.Id
            FROM CashTransactionDenomination d
            WHERE d.TransId = @TransId
              AND d.TwoHundred IS NULL
              AND LEFT(LTRIM(ISNULL(d.Amount, '0')), 1) = '-'
            ORDER BY d.Id
        );
    END
END
GO

IF OBJECT_ID('dbo.SaveAccountReceivableTwoHundred', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.SaveAccountReceivableTwoHundred AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.SaveAccountReceivableTwoHundred
(
    @EnteredBy int,
    @PaidAmount nvarchar(50),
    @TwoHundred nvarchar(50) = NULL,
    @TwoHundredReturn nvarchar(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @receipt varchar(100);
    DECLARE @cashId varchar(100);

    SELECT TOP 1 @receipt = TransId
    FROM RecipeHeader
    WHERE EntityType = 'Estimation AR' AND EnteredBy = @EnteredBy AND PaidAmount = @PaidAmount
    ORDER BY EnteredOn DESC;

    SELECT TOP 1 @cashId = CONVERT(varchar(100), TransId)
    FROM CashTransaction
    WHERE EntityId = @receipt AND EntitySource = 'Estimation AR' AND Type = 'C'
    ORDER BY TransDate DESC;

    EXEC dbo.SaveAccountsTwoHundred @TransId = @cashId, @TwoHundred = @TwoHundred, @TwoHundredReturn = @TwoHundredReturn;
END
GO

IF OBJECT_ID('dbo.SaveAccountReceivableUpi', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.SaveAccountReceivableUpi AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.SaveAccountReceivableUpi
(
    @BillId nvarchar(max),
    @CustomerId varchar(50),
    @IsRecord bit,
    @PaidAmount nvarchar(50),
    @Reference nvarchar(30),
    @EnteredBy int,
    @Result int OUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Result = 0;

    IF @Reference IS NULL OR LTRIM(RTRIM(@Reference)) = ''
    BEGIN
        RAISERROR('Enter UPI Payment Reference', 16, 1);
        RETURN;
    END
    IF @PaidAmount IS NULL OR CONVERT(decimal(18, 2), @PaidAmount) <= 0
    BEGIN
        RAISERROR('UPI amount must be greater than zero.', 16, 1);
        RETURN;
    END

    BEGIN TRAN;
        DECLARE @remaining varchar(50);
        DECLARE @inner int;
        EXEC dbo.Proc_PayReceiveBalance_Direct2
            @BillId = @BillId,
            @CustomerId = @CustomerId,
            @IsRecord = @IsRecord,
            @PaidAmount = @PaidAmount,
            @IsReturnAmount = 0,
            @Flag = 'UPI',
            @Total = @PaidAmount,
            @DenominationTotal = '0.00',
            @Reference = @Reference,
            @EnteredBy = @EnteredBy,
            @RemainingAmount = @remaining OUT,
            @Result = @inner OUT;

        DECLARE @receipt varchar(100);
        SELECT TOP 1 @receipt = TransId
        FROM RecipeHeader
        WHERE EntityType = 'Estimation AR' AND EnteredBy = @EnteredBy AND PaidAmount = @PaidAmount
        ORDER BY EnteredOn DESC;

        INSERT INTO CardTransactions
            (TransId, Entity, EntitySource, TransDate, Amount, Type, Mode, InstrumentNumber, InstrumentDate, InstrumentBank, InstrumentSource, Reference)
        VALUES
            (NEWID(), @receipt, 'PENDING AMOUNT RECEIVED', GETDATE(), CONVERT(decimal(18, 2), @PaidAmount), 'C', 'UPI', '', GETDATE(), '', @Reference, @Reference);
    COMMIT TRAN;

    SET @Result = 1;
END
GO

IF OBJECT_ID('dbo.SavePaymentRequestUpi', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.SavePaymentRequestUpi AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.SavePaymentRequestUpi
(
    @RequestId varchar(100),
    @Amount decimal(18, 2),
    @Reference nvarchar(30),
    @CashPaidBy int,
    @AccountHead nvarchar(250),
    @Result varchar(100) OUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = NULL;

    IF @Reference IS NULL OR LTRIM(RTRIM(@Reference)) = ''
    BEGIN
        RAISERROR('Enter UPI Payment Reference', 16, 1);
        RETURN;
    END
    IF @Amount IS NULL OR @Amount <= 0
    BEGIN
        RAISERROR('UPI amount must be greater than zero.', 16, 1);
        RETURN;
    END

    UPDATE CashRequest
    SET CashPaidBy = @CashPaidBy,
        CashPaidOn = GETDATE(),
        PaymentMode = 'UPI',
        Status = 'Closed'
    WHERE RequestId = @RequestId;

    INSERT INTO CardTransactions
        (TransId, Entity, EntitySource, TransDate, Amount, Type, Mode, InstrumentNumber, InstrumentDate, InstrumentBank, InstrumentSource, Reference)
    VALUES
        (NEWID(), @RequestId, 'CASH REQUEST', GETDATE(), @Amount, 'D', 'UPI', '', GETDATE(), '', @Reference, @Reference);

    SET @Result = @RequestId;
END
GO

IF OBJECT_ID('dbo.SaveCashReceiptUpi', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.SaveCashReceiptUpi AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.SaveCashReceiptUpi
(
    @RequestId varchar(50),
    @Amount decimal(18, 2),
    @Reference nvarchar(30),
    @Result varchar(100) OUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = NULL;

    IF @Reference IS NULL OR LTRIM(RTRIM(@Reference)) = ''
    BEGIN
        RAISERROR('Enter UPI Payment Reference', 16, 1);
        RETURN;
    END
    IF @Amount IS NULL OR @Amount <= 0
    BEGIN
        RAISERROR('UPI amount must be greater than zero.', 16, 1);
        RETURN;
    END

    DECLARE @channelId uniqueidentifier;
    SET @channelId = NEWID();

    INSERT INTO CardTransactions
        (TransId, Entity, EntitySource, TransDate, Amount, Type, Mode, InstrumentNumber, InstrumentDate, InstrumentBank, InstrumentSource, Reference)
    VALUES
        (@channelId, @RequestId, 'CASH RECEIPT', GETDATE(), @Amount, 'C', 'UPI', '', GETDATE(), '', @Reference, @Reference);

    INSERT INTO Receipt (TransactionId, Transdate, EntityId, CustomerName, Amount, Mode)
    SELECT @channelId, GETDATE(), q.ReceiptId, q.ReceiptFrom + '-' + q.AccountHead, @Amount, 'UPI'
    FROM CashReceipt q
    WHERE q.ReceiptId = @RequestId;

    SET @Result = @RequestId;
END
GO
