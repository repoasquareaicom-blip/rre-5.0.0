/*
Recommended indexes for the quotation report.

Review existing branch indexes before deployment. These are intentionally narrow and
deployment-safe for SQL Server 2012; apply only where the indexes do not already exist.
*/

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_QuotationHeader_Updatedon_Quotationid'
      AND object_id = OBJECT_ID('dbo.QuotationHeader')
)
BEGIN
    CREATE INDEX IX_QuotationHeader_Updatedon_Quotationid
    ON dbo.QuotationHeader (Updatedon, Quotationid)
    INCLUDE (customername, City, AssistName, Status, Iscancel, isdelete);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_QuotationDetails_Quotationid'
      AND object_id = OBJECT_ID('dbo.QuotationDetails')
)
BEGIN
    CREATE INDEX IX_QuotationDetails_Quotationid
    ON dbo.QuotationDetails (Quotationid)
    INCLUDE (Productid, Rate, Quantity, Amount, MasterSalesPrice, GSTAtQuote);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_QuotationDetails_Productid'
      AND object_id = OBJECT_ID('dbo.QuotationDetails')
)
BEGIN
    CREATE INDEX IX_QuotationDetails_Productid
    ON dbo.QuotationDetails (Productid);
END
GO
