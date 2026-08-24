/*
Optimize Estimation -> Quotation List initial load/search.

The required supporting index already exists in current RRE_NEW:
IX_QuotationHeader_UpdatedOn_QuotationId ON dbo.QuotationHeader (Updatedon, Quotationid)

This procedure change keeps the same parameters and output columns, but removes
CAST(CONVERT(... Updatedon ...)) from the WHERE clause so SQL Server can seek/range-scan
the Updatedon index.
*/

IF OBJECT_ID('dbo.searchQuotationNew_v2', 'P') IS NOT NULL
    DROP PROCEDURE dbo.searchQuotationNew_v2;
GO

CREATE PROCEDURE dbo.searchQuotationNew_v2
(
    @OrderNo varchar(50) = NULL,
    @FromDate datetime = NULL,
    @ToDate datetime = NULL,
    @Iscombined bit = NULL,
    @customername varchar(500) = NULL,
    @product varchar(255) = NULL,
    @qty nvarchar(255) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FromDateStart datetime;
    DECLARE @ToDateExclusive datetime;

    SET @FromDateStart = CASE WHEN @FromDate IS NULL THEN NULL ELSE DATEADD(day, DATEDIFF(day, 0, @FromDate), 0) END;
    SET @ToDateExclusive = CASE WHEN @ToDate IS NULL THEN NULL ELSE DATEADD(day, DATEDIFF(day, 0, @ToDate) + 1, 0) END;

    SELECT
        qh.Quotationid AS [Order No],
        qh.customername AS Customer,
        CONVERT(varchar(10), qh.Updatedon, 120) AS [Order Date],
        qh.sino
    INTO #QuotationList
    FROM dbo.QuotationHeader qh
    WHERE (@OrderNo IS NULL OR @OrderNo = '' OR qh.Quotationid LIKE '%' + @OrderNo + '%')
      AND (@customername IS NULL OR @customername = '' OR qh.customername LIKE '%' + @customername + '%')
      AND (@FromDateStart IS NULL OR qh.Updatedon >= @FromDateStart)
      AND (@ToDateExclusive IS NULL OR qh.Updatedon < @ToDateExclusive);

    ALTER TABLE #QuotationList ADD Products varchar(1) NOT NULL DEFAULT('');
    ALTER TABLE #QuotationList ADD QTY int NOT NULL DEFAULT(0);
    ALTER TABLE #QuotationList ADD isproduct bit NOT NULL DEFAULT(1);
    ALTER TABLE #QuotationList ADD isqty bit NOT NULL DEFAULT(1);

    IF ISNULL(@product, '') <> ''
    BEGIN
        UPDATE q
        SET q.isproduct = 0
        FROM #QuotationList q
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.QuotationDetails qd
            INNER JOIN dbo.ProductMaster pm ON qd.Productid = pm.id
            WHERE qd.Quotationid = q.[Order No]
              AND pm.DisplayName LIKE '%' + @product + '%'
        );
    END;

    IF ISNULL(@qty, '') <> '' AND ISNULL(@qty, '') <> '-1'
    BEGIN
        UPDATE q
        SET q.isqty = 0
        FROM #QuotationList q
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.QuotationDetails qd
            WHERE qd.Quotationid = q.[Order No]
              AND CONVERT(nvarchar(255), qd.Quantity) = @qty
        );
    END;

    SELECT *
    FROM #QuotationList
    WHERE isproduct = 1
      AND isqty = 1
    ORDER BY sino DESC;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_QuotationHeader_UpdatedOn_QuotationId'
      AND object_id = OBJECT_ID('dbo.QuotationHeader')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_QuotationHeader_UpdatedOn_QuotationId
    ON dbo.QuotationHeader (Updatedon, Quotationid)
    INCLUDE (customername, City, AssistName, Status, Iscancel, isdelete, sino);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_QuotationDetails_QuotationId'
      AND object_id = OBJECT_ID('dbo.QuotationDetails')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_QuotationDetails_QuotationId
    ON dbo.QuotationDetails (Quotationid)
    INCLUDE (Productid, Rate, Quantity, Amount);
END;
GO

UPDATE STATISTICS dbo.QuotationHeader;
UPDATE STATISTICS dbo.QuotationDetails;
GO
