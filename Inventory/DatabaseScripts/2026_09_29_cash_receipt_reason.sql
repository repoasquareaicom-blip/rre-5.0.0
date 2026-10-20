/*
  Stores the Cash Receipt narration.

  The existing header insert (SaveTransactionCashReceipt) is unchanged.
  Reason is written after the receipt id is created.
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

IF COL_LENGTH('dbo.CashReceipt', 'Reason') IS NULL
    ALTER TABLE dbo.CashReceipt ADD Reason nvarchar(500) NULL;
GO

IF OBJECT_ID('dbo.SaveCashReceiptReason', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.SaveCashReceiptReason AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.SaveCashReceiptReason
(
    @ReceiptId varchar(50),
    @Reason nvarchar(500)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE CashReceipt
    SET Reason = @Reason
    WHERE ReceiptId = @ReceiptId;
END
GO
