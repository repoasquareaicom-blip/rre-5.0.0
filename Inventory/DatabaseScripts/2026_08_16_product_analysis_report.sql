/*
  Product Analysis report for Reporting.Web.
  SQL Server 2012 compatible. Do not execute directly against production without review.
*/

IF OBJECT_ID('dbo.sp_report_product_analysis_detail', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_report_product_analysis_detail;
GO

IF OBJECT_ID('dbo.sp_report_product_analysis', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_report_product_analysis;
GO

IF OBJECT_ID('dbo.sp_report_product_lookup', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_report_product_lookup;
GO

CREATE PROCEDURE dbo.sp_report_product_lookup
    @SearchText NVARCHAR(200) = NULL,
    @Top INT = 50
AS
BEGIN
    SET NOCOUNT ON;

    IF @Top IS NULL OR @Top < 1 SET @Top = 50;
    IF @Top > 100 SET @Top = 100;

    DECLARE @Search NVARCHAR(200) = NULLIF(LTRIM(RTRIM(@SearchText)), '');

    SELECT TOP (@Top)
        CONVERT(VARCHAR(50), pm.id) AS ProductId,
        COALESCE(NULLIF(pm.DisplayName, ''), NULLIF(pm.ItemName, ''), CONVERT(VARCHAR(50), pm.id)) AS ProductName
    FROM dbo.ProductMaster pm
    WHERE ISNULL(pm.IsDeleted, '0') <> '1'
      AND (
            @Search IS NULL
            OR pm.DisplayName LIKE '%' + @Search + '%'
            OR pm.ItemName LIKE '%' + @Search + '%'
            OR CONVERT(VARCHAR(50), pm.id) LIKE '%' + @Search + '%'
          )
    ORDER BY COALESCE(NULLIF(pm.DisplayName, ''), NULLIF(pm.ItemName, ''), CONVERT(VARCHAR(50), pm.id));
END;
GO

CREATE PROCEDURE dbo.sp_report_product_analysis
    @AnalysisType VARCHAR(20),
    @ProductId VARCHAR(50) = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchText NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @AnalysisType = UPPER(LTRIM(RTRIM(ISNULL(@AnalysisType, 'SALES'))));
    IF @AnalysisType NOT IN ('QUOTATION', 'ESTIMATION', 'SALES')
    BEGIN
        RAISERROR('Invalid AnalysisType. Use QUOTATION, ESTIMATION, or SALES.', 16, 1);
        RETURN;
    END;

    IF @FromDate IS NULL SET @FromDate = CAST(GETDATE() AS DATE);
    IF @ToDate IS NULL SET @ToDate = @FromDate;
    IF @ToDate < @FromDate SET @ToDate = @FromDate;
    IF @PageNumber IS NULL OR @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize IS NULL OR @PageSize < 1 SET @PageSize = 10;
    IF @PageSize > 500 SET @PageSize = 500;

    DECLARE @Search NVARCHAR(200) = NULLIF(LTRIM(RTRIM(@SearchText)), '');

    CREATE TABLE #Lines
    (
        ProductId VARCHAR(50) NOT NULL,
        ProductName NVARCHAR(300) NOT NULL,
        DocumentNo VARCHAR(50) NOT NULL,
        Division VARCHAR(30) NULL,
        Quantity DECIMAL(18, 6) NOT NULL,
        Rate DECIMAL(18, 6) NOT NULL,
        Amount DECIMAL(18, 6) NOT NULL,
        TaxableAmount DECIMAL(18, 6) NOT NULL,
        GSTAmount DECIMAL(18, 6) NOT NULL
    );

    IF @AnalysisType = 'QUOTATION'
    BEGIN
        INSERT #Lines
        SELECT
            CONVERT(VARCHAR(50), qd.Productid),
            COALESCE(NULLIF(pm.DisplayName, ''), NULLIF(pm.ItemName, ''), CONVERT(VARCHAR(50), qd.Productid)),
            qh.Quotationid,
            NULL,
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Quantity), 0),
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Rate), 0),
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0),
            CASE WHEN ISNULL(COALESCE(TRY_CONVERT(DECIMAL(18, 6), qd.GSTAtQuote), TRY_CONVERT(DECIMAL(18, 6), pm.GST)), 0) > 0
                 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0) / (1 + (COALESCE(TRY_CONVERT(DECIMAL(18, 6), qd.GSTAtQuote), TRY_CONVERT(DECIMAL(18, 6), pm.GST)) / 100.0))
                 ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0) END,
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0) -
            CASE WHEN ISNULL(COALESCE(TRY_CONVERT(DECIMAL(18, 6), qd.GSTAtQuote), TRY_CONVERT(DECIMAL(18, 6), pm.GST)), 0) > 0
                 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0) / (1 + (COALESCE(TRY_CONVERT(DECIMAL(18, 6), qd.GSTAtQuote), TRY_CONVERT(DECIMAL(18, 6), pm.GST)) / 100.0))
                 ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0) END
        FROM dbo.QuotationHeader qh
        INNER JOIN dbo.QuotationDetails qd ON qd.Quotationid = qh.Quotationid
        LEFT JOIN dbo.ProductMaster pm ON CONVERT(VARCHAR(50), pm.id) = CONVERT(VARCHAR(50), qd.Productid)
        WHERE qh.Updatedon >= @FromDate
          AND qh.Updatedon < DATEADD(DAY, 1, @ToDate)
          AND (@ProductId IS NULL OR CONVERT(VARCHAR(50), qd.Productid) = @ProductId);
    END;

    IF @AnalysisType = 'ESTIMATION'
    BEGIN
        INSERT #Lines
        SELECT
            CONVERT(VARCHAR(50), qed.Productid),
            COALESCE(NULLIF(pm.DisplayName, ''), NULLIF(pm.ItemName, ''), CONVERT(VARCHAR(50), qed.Productid)),
            qe.Estimationid,
            NULL,
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Quantity), 0),
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Rate), 0),
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0),
            CASE WHEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), pm.GST), 0) > 0
                 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0) / (1 + (TRY_CONVERT(DECIMAL(18, 6), pm.GST) / 100.0))
                 ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0) END,
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0) -
            CASE WHEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), pm.GST), 0) > 0
                 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0) / (1 + (TRY_CONVERT(DECIMAL(18, 6), pm.GST) / 100.0))
                 ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0) END
        FROM dbo.QuotationEstimation qe
        INNER JOIN dbo.QuotationEstimationDetails qed ON qed.Estimationid = qe.Estimationid
        LEFT JOIN dbo.ProductMaster pm ON CONVERT(VARCHAR(50), pm.id) = CONVERT(VARCHAR(50), qed.Productid)
        WHERE qe.Updatedon >= @FromDate
          AND qe.Updatedon < DATEADD(DAY, 1, @ToDate)
          AND ISNULL(qe.isdelete, '0') <> '1'
          AND (@ProductId IS NULL OR CONVERT(VARCHAR(50), qed.Productid) = @ProductId);
    END;

    IF @AnalysisType = 'SALES'
    BEGIN
        ;WITH SalesLines AS
        (
            SELECT 'Electricals' AS Division, s.Salesid, s.Updatedon, sd.Productid, sd.Rate, sd.Quantity, sd.Amount, sd.gst FROM dbo.Sales s INNER JOIN dbo.SalesDetails sd ON sd.Salesid = s.Salesid
            UNION ALL
            SELECT 'Pipes', s.Salesid, s.Updatedon, sd.ProductId, sd.Rate, sd.Quantity, sd.Amount, sd.gst FROM dbo.SalesPipes s INNER JOIN dbo.SalesPipesDetails sd ON sd.Salesid = s.Salesid
            UNION ALL
            SELECT 'Traders', s.Salesid, s.Updatedon, sd.ProductId, sd.Rate, sd.Quantity, sd.Amount, sd.gst FROM dbo.SalesTraders s INNER JOIN dbo.SalesTradersDetails sd ON sd.Salesid = s.Salesid
        )
        INSERT #Lines
        SELECT
            CONVERT(VARCHAR(50), sl.Productid),
            COALESCE(NULLIF(pm.DisplayName, ''), NULLIF(pm.ItemName, ''), CONVERT(VARCHAR(50), sl.Productid)),
            sl.Salesid,
            sl.Division,
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Quantity), 0),
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Rate), 0),
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0),
            CASE WHEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.gst), 0) > 0
                 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0) / (1 + (TRY_CONVERT(DECIMAL(18, 6), sl.gst) / 100.0))
                 ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0) END,
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0) -
            CASE WHEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.gst), 0) > 0
                 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0) / (1 + (TRY_CONVERT(DECIMAL(18, 6), sl.gst) / 100.0))
                 ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0) END
        FROM SalesLines sl
        LEFT JOIN dbo.ProductMaster pm ON CONVERT(VARCHAR(50), pm.id) = CONVERT(VARCHAR(50), sl.Productid)
        WHERE sl.Updatedon >= @FromDate
          AND sl.Updatedon < DATEADD(DAY, 1, @ToDate)
          AND (@ProductId IS NULL OR CONVERT(VARCHAR(50), sl.Productid) = @ProductId);
    END;

    ;WITH FilteredLines AS
    (
        SELECT *
        FROM #Lines
        WHERE @Search IS NULL OR ProductName LIKE '%' + @Search + '%' OR ProductId LIKE '%' + @Search + '%'
    ),
    Aggregated AS
    (
        SELECT
            ProductId,
            ProductName,
            COUNT(DISTINCT DocumentNo) AS TransactionCount,
            SUM(Quantity) AS TotalQuantity,
            SUM(Rate * Quantity) / NULLIF(SUM(Quantity), 0) AS AverageRate,
            MIN(Rate) AS MinimumRate,
            MAX(Rate) AS MaximumRate,
            SUM(TaxableAmount) AS TaxableAmount,
            SUM(GSTAmount) AS GSTAmount,
            SUM(Amount) AS TotalValue,
            SUM(CASE WHEN Division = 'Electricals' THEN Amount ELSE 0 END) AS ElectricalsValue,
            SUM(CASE WHEN Division = 'Pipes' THEN Amount ELSE 0 END) AS PipesValue,
            SUM(CASE WHEN Division = 'Traders' THEN Amount ELSE 0 END) AS TradersValue
        FROM FilteredLines
        GROUP BY ProductId, ProductName
    ),
    Summary AS
    (
        SELECT COUNT(1) AS SummaryProducts, SUM(TransactionCount) AS SummaryTransactions, SUM(TotalQuantity) AS SummaryQuantity, SUM(TotalValue) AS SummaryValue
        FROM Aggregated
    ),
    NumberedRows AS
    (
        SELECT *, COUNT(1) OVER() AS TotalRows, ROW_NUMBER() OVER (ORDER BY TotalValue DESC, ProductName) AS RowNumber
        FROM Aggregated
    )
    SELECT
        nr.ProductId,
        nr.ProductName,
        nr.TransactionCount,
        nr.TotalQuantity,
        nr.AverageRate,
        nr.MinimumRate,
        nr.MaximumRate,
        nr.TaxableAmount,
        nr.GSTAmount,
        nr.TotalValue,
        nr.ElectricalsValue,
        nr.PipesValue,
        nr.TradersValue,
        nr.TotalRows,
        ISNULL(s.SummaryProducts, 0) AS SummaryProducts,
        ISNULL(s.SummaryTransactions, 0) AS SummaryTransactions,
        ISNULL(s.SummaryQuantity, 0) AS SummaryQuantity,
        ISNULL(s.SummaryValue, 0) AS SummaryValue
    FROM NumberedRows nr
    CROSS JOIN Summary s
    WHERE nr.RowNumber BETWEEN ((@PageNumber - 1) * @PageSize) + 1 AND (@PageNumber * @PageSize)
    ORDER BY nr.RowNumber;
END;
GO

CREATE PROCEDURE dbo.sp_report_product_analysis_detail
    @AnalysisType VARCHAR(20),
    @ProductId VARCHAR(50),
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchText NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @AnalysisType = UPPER(LTRIM(RTRIM(ISNULL(@AnalysisType, 'SALES'))));
    IF @AnalysisType NOT IN ('QUOTATION', 'ESTIMATION', 'SALES')
    BEGIN
        RAISERROR('Invalid AnalysisType. Use QUOTATION, ESTIMATION, or SALES.', 16, 1);
        RETURN;
    END;

    IF @ProductId IS NULL OR LTRIM(RTRIM(@ProductId)) = ''
    BEGIN
        RAISERROR('ProductId is required.', 16, 1);
        RETURN;
    END;

    IF @FromDate IS NULL SET @FromDate = CAST(GETDATE() AS DATE);
    IF @ToDate IS NULL SET @ToDate = @FromDate;
    IF @ToDate < @FromDate SET @ToDate = @FromDate;
    IF @PageNumber IS NULL OR @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize IS NULL OR @PageSize < 1 SET @PageSize = 10;
    IF @PageSize > 500 SET @PageSize = 500;

    DECLARE @Search NVARCHAR(200) = NULLIF(LTRIM(RTRIM(@SearchText)), '');

    CREATE TABLE #Detail
    (
        TransactionDate DATETIME NOT NULL,
        DocumentNo VARCHAR(50) NOT NULL,
        CustomerName NVARCHAR(300) NULL,
        Division VARCHAR(30) NULL,
        Quantity DECIMAL(18, 6) NOT NULL,
        Rate DECIMAL(18, 6) NOT NULL,
        GSTPercent DECIMAL(18, 6) NOT NULL,
        TaxableAmount DECIMAL(18, 6) NOT NULL,
        GSTAmount DECIMAL(18, 6) NOT NULL,
        Amount DECIMAL(18, 6) NOT NULL
    );

    IF @AnalysisType = 'QUOTATION'
        INSERT #Detail
        SELECT qh.Updatedon, qh.Quotationid, qh.customername, NULL,
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Quantity), 0),
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Rate), 0),
               ISNULL(COALESCE(TRY_CONVERT(DECIMAL(18, 6), qd.GSTAtQuote), TRY_CONVERT(DECIMAL(18, 6), pm.GST)), 0),
               CASE WHEN ISNULL(COALESCE(TRY_CONVERT(DECIMAL(18, 6), qd.GSTAtQuote), TRY_CONVERT(DECIMAL(18, 6), pm.GST)), 0) > 0 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0) / (1 + (COALESCE(TRY_CONVERT(DECIMAL(18, 6), qd.GSTAtQuote), TRY_CONVERT(DECIMAL(18, 6), pm.GST)) / 100.0)) ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0) END,
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0) - CASE WHEN ISNULL(COALESCE(TRY_CONVERT(DECIMAL(18, 6), qd.GSTAtQuote), TRY_CONVERT(DECIMAL(18, 6), pm.GST)), 0) > 0 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0) / (1 + (COALESCE(TRY_CONVERT(DECIMAL(18, 6), qd.GSTAtQuote), TRY_CONVERT(DECIMAL(18, 6), pm.GST)) / 100.0)) ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0) END,
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), qd.Amount), 0)
        FROM dbo.QuotationHeader qh
        INNER JOIN dbo.QuotationDetails qd ON qd.Quotationid = qh.Quotationid
        LEFT JOIN dbo.ProductMaster pm ON CONVERT(VARCHAR(50), pm.id) = CONVERT(VARCHAR(50), qd.Productid)
        WHERE qh.Updatedon >= @FromDate AND qh.Updatedon < DATEADD(DAY, 1, @ToDate) AND CONVERT(VARCHAR(50), qd.Productid) = @ProductId;

    IF @AnalysisType = 'ESTIMATION'
        INSERT #Detail
        SELECT qe.Updatedon, qe.Estimationid, qe.customername, NULL,
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Quantity), 0),
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Rate), 0),
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), pm.GST), 0),
               CASE WHEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), pm.GST), 0) > 0 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0) / (1 + (TRY_CONVERT(DECIMAL(18, 6), pm.GST) / 100.0)) ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0) END,
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0) - CASE WHEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), pm.GST), 0) > 0 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0) / (1 + (TRY_CONVERT(DECIMAL(18, 6), pm.GST) / 100.0)) ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0) END,
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), qed.Amount), 0)
        FROM dbo.QuotationEstimation qe
        INNER JOIN dbo.QuotationEstimationDetails qed ON qed.Estimationid = qe.Estimationid
        LEFT JOIN dbo.ProductMaster pm ON CONVERT(VARCHAR(50), pm.id) = CONVERT(VARCHAR(50), qed.Productid)
        WHERE qe.Updatedon >= @FromDate AND qe.Updatedon < DATEADD(DAY, 1, @ToDate) AND ISNULL(qe.isdelete, '0') <> '1' AND CONVERT(VARCHAR(50), qed.Productid) = @ProductId;

    IF @AnalysisType = 'SALES'
    BEGIN
        ;WITH SalesLines AS
        (
            SELECT 'Electricals' AS Division, s.Salesid, s.Updatedon, s.customername, sd.Productid, sd.Rate, sd.Quantity, sd.Amount, sd.gst FROM dbo.Sales s INNER JOIN dbo.SalesDetails sd ON sd.Salesid = s.Salesid
            UNION ALL
            SELECT 'Pipes', s.Salesid, s.Updatedon, s.customername, sd.ProductId, sd.Rate, sd.Quantity, sd.Amount, sd.gst FROM dbo.SalesPipes s INNER JOIN dbo.SalesPipesDetails sd ON sd.Salesid = s.Salesid
            UNION ALL
            SELECT 'Traders', s.Salesid, s.Updatedon, s.customername, sd.ProductId, sd.Rate, sd.Quantity, sd.Amount, sd.gst FROM dbo.SalesTraders s INNER JOIN dbo.SalesTradersDetails sd ON sd.Salesid = s.Salesid
        )
        INSERT #Detail
        SELECT sl.Updatedon, sl.Salesid, sl.customername, sl.Division,
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Quantity), 0),
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Rate), 0),
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.gst), 0),
               CASE WHEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.gst), 0) > 0 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0) / (1 + (TRY_CONVERT(DECIMAL(18, 6), sl.gst) / 100.0)) ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0) END,
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0) - CASE WHEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.gst), 0) > 0 THEN ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0) / (1 + (TRY_CONVERT(DECIMAL(18, 6), sl.gst) / 100.0)) ELSE ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0) END,
               ISNULL(TRY_CONVERT(DECIMAL(18, 6), sl.Amount), 0)
        FROM SalesLines sl
        WHERE sl.Updatedon >= @FromDate AND sl.Updatedon < DATEADD(DAY, 1, @ToDate) AND CONVERT(VARCHAR(50), sl.Productid) = @ProductId;
    END;

    ;WITH Filtered AS
    (
        SELECT *
        FROM #Detail
        WHERE @Search IS NULL OR DocumentNo LIKE '%' + @Search + '%' OR CustomerName LIKE '%' + @Search + '%'
    ),
    NumberedRows AS
    (
        SELECT *, COUNT(1) OVER() AS TotalRows, ROW_NUMBER() OVER (ORDER BY TransactionDate DESC, DocumentNo DESC) AS RowNumber
        FROM Filtered
    )
    SELECT
        TransactionDate,
        DocumentNo,
        CustomerName,
        Division,
        Quantity,
        Rate,
        GSTPercent,
        TaxableAmount,
        GSTAmount,
        Amount,
        TotalRows
    FROM NumberedRows
    WHERE RowNumber BETWEEN ((@PageNumber - 1) * @PageSize) + 1 AND (@PageNumber * @PageSize)
    ORDER BY RowNumber;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductMaster_ProductLookup' AND object_id = OBJECT_ID('dbo.ProductMaster'))
    CREATE NONCLUSTERED INDEX IX_ProductMaster_ProductLookup ON dbo.ProductMaster (id) INCLUDE (IsDeleted);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuotationDetails_Productid_Quotationid' AND object_id = OBJECT_ID('dbo.QuotationDetails'))
    CREATE NONCLUSTERED INDEX IX_QuotationDetails_Productid_Quotationid ON dbo.QuotationDetails (Productid, Quotationid) INCLUDE (Rate, Quantity, Amount, GSTAtQuote);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuotationEstimationDetails_Productid_Estimationid' AND object_id = OBJECT_ID('dbo.QuotationEstimationDetails'))
    CREATE NONCLUSTERED INDEX IX_QuotationEstimationDetails_Productid_Estimationid ON dbo.QuotationEstimationDetails (Productid, Estimationid) INCLUDE (Rate, Quantity, Amount);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SalesDetails_Productid_Salesid' AND object_id = OBJECT_ID('dbo.SalesDetails'))
    CREATE NONCLUSTERED INDEX IX_SalesDetails_Productid_Salesid ON dbo.SalesDetails (Productid, Salesid) INCLUDE (Rate, Quantity, Amount, gst);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SalesPipesDetails_Productid_Salesid' AND object_id = OBJECT_ID('dbo.SalesPipesDetails'))
    CREATE NONCLUSTERED INDEX IX_SalesPipesDetails_Productid_Salesid ON dbo.SalesPipesDetails (ProductId, Salesid) INCLUDE (Rate, Quantity, Amount, gst);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SalesTradersDetails_Productid_Salesid' AND object_id = OBJECT_ID('dbo.SalesTradersDetails'))
    CREATE NONCLUSTERED INDEX IX_SalesTradersDetails_Productid_Salesid ON dbo.SalesTradersDetails (ProductId, Salesid) INCLUDE (Rate, Quantity, Amount, gst);
GO
