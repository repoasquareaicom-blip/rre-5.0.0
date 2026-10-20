/*
  Day Close V2 ledger performance: lookup indexes.

  fn_DayCloseV2_SourceRows looks up a description for each payment row by
  QuotationEstimation.Estimationid, CashRequest.RequestId and CashReceipt.ReceiptId.
  These tables are heaps without an index on those columns, so every lookup scans the table.

  Adds three nonclustered indexes only. No data, procedure, function or calculation is changed.
  Safe to run more than once.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
GO

IF OBJECT_ID('dbo.QuotationEstimation', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.QuotationEstimation', 'Estimationid') IS NOT NULL
   AND COL_LENGTH('dbo.QuotationEstimation', 'customername') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.QuotationEstimation') AND name = 'IX_QuotationEstimation_Estimationid')
BEGIN
    CREATE NONCLUSTERED INDEX IX_QuotationEstimation_Estimationid
        ON dbo.QuotationEstimation (Estimationid)
        INCLUDE (customername);
END
GO

IF OBJECT_ID('dbo.CashRequest', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.CashRequest', 'RequestId') IS NOT NULL
   AND COL_LENGTH('dbo.CashRequest', 'Reason') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.CashRequest') AND name = 'IX_CashRequest_RequestId')
BEGIN
    CREATE NONCLUSTERED INDEX IX_CashRequest_RequestId
        ON dbo.CashRequest (RequestId)
        INCLUDE (Reason);
END
GO

IF OBJECT_ID('dbo.CashReceipt', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.CashReceipt', 'ReceiptId') IS NOT NULL
   AND COL_LENGTH('dbo.CashReceipt', 'Reason') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.CashReceipt') AND name = 'IX_CashReceipt_ReceiptId')
BEGIN
    CREATE NONCLUSTERED INDEX IX_CashReceipt_ReceiptId
        ON dbo.CashReceipt (ReceiptId)
        INCLUDE (Reason);
END
GO

SELECT OBJECT_NAME(i.object_id) AS TableName, i.name AS IndexName, i.type_desc
FROM sys.indexes i
WHERE i.name IN ('IX_QuotationEstimation_Estimationid', 'IX_CashRequest_RequestId', 'IX_CashReceipt_ReceiptId')
ORDER BY TableName;
GO
