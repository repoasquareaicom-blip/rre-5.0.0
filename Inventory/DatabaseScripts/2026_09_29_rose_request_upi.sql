/*
  Rose Request UPI payment.

  Cash Rose payments stay on SavecommisionPayment_Direct:
  CashTransaction Type D, EntitySource COMMISSION.

  This procedure adds the UPI channel only.
  Mode is UPI. GPay is not stored as a separate mode.
  No CashTransaction and no denomination row.
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

IF OBJECT_ID('dbo.SaveRoseRequestUpi', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.SaveRoseRequestUpi AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.SaveRoseRequestUpi
(
    @CommissionId nvarchar(50),
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

    UPDATE Commission
    SET CommissionPaid = CAST(CAST(@Amount AS numeric(18)) AS varchar(10)),
        CommissionBalance = '0'
    WHERE Commissionid = @CommissionId;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Rose Request was not found.', 16, 1);
        RETURN;
    END

    INSERT INTO CardTransactions
        (TransId, Entity, EntitySource, TransDate, Amount, Type, Mode, InstrumentNumber, InstrumentDate, InstrumentBank, InstrumentSource, Reference)
    VALUES
        (NEWID(), @CommissionId, 'COMMISSION', GETDATE(), @Amount, 'D', 'UPI', '', GETDATE(), '', @Reference, @Reference);

    SET @Result = @CommissionId;
END
GO
