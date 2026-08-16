/*
  Estimation and Sales reports for Reporting.Web.
  SQL Server 2012 compatible: no CREATE OR ALTER, no STRING_AGG.
*/

IF OBJECT_ID('dbo.sp_report_estimation_detail', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_report_estimation_detail;
GO

IF OBJECT_ID('dbo.sp_report_estimation', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_report_estimation;
GO

IF OBJECT_ID('dbo.sp_report_sales_detail', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_report_sales_detail;
GO

IF OBJECT_ID('dbo.sp_report_sales', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_report_sales;
GO

CREATE PROCEDURE dbo.sp_report_estimation
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchText NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @FromDate IS NULL
        SET @FromDate = CAST(GETDATE() AS DATE);

    IF @ToDate IS NULL
        SET @ToDate = @FromDate;

    IF @ToDate < @FromDate
        SET @ToDate = @FromDate;

    IF @PageNumber IS NULL OR @PageNumber < 1
        SET @PageNumber = 1;

    IF @PageSize IS NULL OR @PageSize < 1
        SET @PageSize = 10;

    IF @PageSize > 500
        SET @PageSize = 500;

    DECLARE @Search NVARCHAR(200) = NULLIF(LTRIM(RTRIM(@SearchText)), '');

    ;WITH FilteredHeaders AS
    (
        SELECT
            qe.Estimationid,
            qe.Quotationid,
            qe.Updatedon,
            qe.customername,
            qe.City,
            qe.AssistName,
            qe.Iscancel,
            TRY_CONVERT(DECIMAL(18, 2), qe.GrnandTotal) AS HeaderGrandTotal
        FROM dbo.QuotationEstimation qe
        WHERE qe.Updatedon >= @FromDate
          AND qe.Updatedon < DATEADD(DAY, 1, @ToDate)
          AND (ISNULL(qe.isdelete, '0') <> '1')
          AND (
                @Search IS NULL
                OR qe.Estimationid LIKE '%' + @Search + '%'
                OR qe.Quotationid LIKE '%' + @Search + '%'
                OR qe.customername LIKE '%' + @Search + '%'
                OR qe.City LIKE '%' + @Search + '%'
                OR qe.AssistName LIKE '%' + @Search + '%'
              )
    ),
    LineFacts AS
    (
        SELECT
            fh.Estimationid,
            qed.Productid AS ProductId,
            TRY_CONVERT(DECIMAL(18, 3), qed.Quantity) AS Quantity,
            TRY_CONVERT(DECIMAL(18, 2), qed.Amount) AS Amount,
            ISNULL(TRY_CONVERT(DECIMAL(18, 2), pm.GST), 0) AS GST
        FROM FilteredHeaders fh
        LEFT JOIN dbo.QuotationEstimationDetails qed
            ON qed.Estimationid = fh.Estimationid
        LEFT JOIN dbo.ProductMaster pm
            ON CONVERT(VARCHAR(50), pm.id) = CONVERT(VARCHAR(50), qed.Productid)
    ),
    HeaderAgg AS
    (
        SELECT
            Estimationid,
            COUNT(ProductId) AS ItemCount,
            SUM(ISNULL(Quantity, 0)) AS TotalQuantity,
            SUM(CASE WHEN GST > 0 THEN ISNULL(Amount, 0) * 100 / (100 + GST) ELSE ISNULL(Amount, 0) END) AS TaxableValue,
            SUM(CASE WHEN GST > 0 THEN ISNULL(Amount, 0) - (ISNULL(Amount, 0) * 100 / (100 + GST)) ELSE 0 END) AS GSTAmount,
            SUM(ISNULL(Amount, 0)) AS LineTotal
        FROM LineFacts
        GROUP BY Estimationid
    ),
    Summary AS
    (
        SELECT
            COUNT(1) AS SummaryTotalEstimations,
            SUM(ISNULL(ha.TaxableValue, 0)) AS SummaryTaxableValue,
            SUM(ISNULL(ha.GSTAmount, 0)) AS SummaryGSTAmount,
            SUM(ISNULL(fh.HeaderGrandTotal, ISNULL(ha.LineTotal, 0))) AS SummaryEstimationValue,
            SUM(CASE WHEN ISNULL(fh.Iscancel, 0) <> 0 THEN 1 ELSE 0 END) AS SummaryCancelled
        FROM FilteredHeaders fh
        LEFT JOIN HeaderAgg ha
            ON ha.Estimationid = fh.Estimationid
    ),
    NumberedRows AS
    (
        SELECT
            fh.Estimationid AS EstimationId,
            fh.Quotationid AS QuotationId,
            fh.Updatedon AS UpdatedOn,
            fh.customername AS CustomerName,
            fh.City,
            fh.AssistName,
            ISNULL(ha.ItemCount, 0) AS ItemCount,
            ISNULL(ha.TotalQuantity, 0) AS TotalQuantity,
            ISNULL(ha.TaxableValue, 0) AS TaxableValue,
            ISNULL(ha.GSTAmount, 0) AS GSTAmount,
            ISNULL(fh.HeaderGrandTotal, ISNULL(ha.LineTotal, 0)) AS EstimationValue,
            CASE WHEN ISNULL(fh.Iscancel, 0) <> 0 THEN 1 ELSE 0 END AS IsCancelled,
            COUNT(1) OVER() AS TotalRows,
            ROW_NUMBER() OVER (ORDER BY fh.Updatedon DESC, fh.Estimationid DESC) AS RowNumber
        FROM FilteredHeaders fh
        LEFT JOIN HeaderAgg ha
            ON ha.Estimationid = fh.Estimationid
    )
    SELECT
        nr.EstimationId,
        nr.QuotationId,
        nr.UpdatedOn,
        nr.CustomerName,
        nr.City,
        nr.AssistName,
        nr.ItemCount,
        nr.TotalQuantity,
        nr.TaxableValue,
        nr.GSTAmount,
        nr.EstimationValue,
        nr.IsCancelled,
        nr.TotalRows,
        s.SummaryTotalEstimations,
        ISNULL(s.SummaryTaxableValue, 0) AS SummaryTaxableValue,
        ISNULL(s.SummaryGSTAmount, 0) AS SummaryGSTAmount,
        ISNULL(s.SummaryEstimationValue, 0) AS SummaryEstimationValue,
        ISNULL(s.SummaryCancelled, 0) AS SummaryCancelled
    FROM NumberedRows nr
    CROSS JOIN Summary s
    WHERE nr.RowNumber BETWEEN ((@PageNumber - 1) * @PageSize) + 1 AND (@PageNumber * @PageSize)
    ORDER BY nr.RowNumber;
END;
GO

CREATE PROCEDURE dbo.sp_report_estimation_detail
    @EstimationId VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH HeaderRow AS
    (
        SELECT TOP (1)
            qe.Estimationid,
            qe.customername,
            qe.City,
            qe.Updatedon,
            TRY_CONVERT(DECIMAL(18, 2), qe.GrnandTotal) AS HeaderGrandTotal
        FROM dbo.QuotationEstimation qe
        WHERE qe.Estimationid = @EstimationId
    ),
    LineFacts AS
    (
        SELECT
            qed.Productid AS ProductId,
            COALESCE(NULLIF(pm.ItemName, ''), NULLIF(pm.DisplayName, ''), qed.Productid) AS ProductName,
            TRY_CONVERT(DECIMAL(18, 2), qed.Rate) AS Rate,
            TRY_CONVERT(DECIMAL(18, 3), qed.Quantity) AS Quantity,
            TRY_CONVERT(DECIMAL(18, 2), qed.Amount) AS Amount,
            ISNULL(TRY_CONVERT(DECIMAL(18, 2), pm.GST), 0) AS GST
        FROM dbo.QuotationEstimationDetails qed
        INNER JOIN HeaderRow h
            ON h.Estimationid = qed.Estimationid
        LEFT JOIN dbo.ProductMaster pm
            ON CONVERT(VARCHAR(50), pm.id) = CONVERT(VARCHAR(50), qed.Productid)
    ),
    CalculatedLines AS
    (
        SELECT
            ProductId,
            ProductName,
            Rate,
            Quantity,
            Amount,
            GST,
            CASE WHEN GST > 0 THEN ISNULL(Amount, 0) * 100 / (100 + GST) ELSE ISNULL(Amount, 0) END AS TaxableValue,
            CASE WHEN GST > 0 THEN ISNULL(Amount, 0) - (ISNULL(Amount, 0) * 100 / (100 + GST)) ELSE 0 END AS GSTAmount
        FROM LineFacts
    )
    SELECT
        'DETAIL' AS RowType,
        h.Estimationid AS EstimationId,
        h.customername AS CustomerName,
        h.City,
        h.Updatedon AS UpdatedOn,
        cl.ProductId,
        cl.ProductName,
        cl.Rate,
        cl.Quantity,
        cl.GST,
        cl.TaxableValue,
        cl.GSTAmount,
        cl.Amount,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalTaxable,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalGST,
        CAST(NULL AS DECIMAL(18, 2)) AS GrandTotal
    FROM HeaderRow h
    INNER JOIN CalculatedLines cl
        ON 1 = 1

    UNION ALL

    SELECT
        'TOTAL' AS RowType,
        h.Estimationid AS EstimationId,
        h.customername AS CustomerName,
        h.City,
        h.Updatedon AS UpdatedOn,
        NULL AS ProductId,
        NULL AS ProductName,
        NULL AS Rate,
        NULL AS Quantity,
        NULL AS GST,
        NULL AS TaxableValue,
        NULL AS GSTAmount,
        NULL AS Amount,
        SUM(cl.TaxableValue) AS TotalTaxable,
        SUM(cl.GSTAmount) AS TotalGST,
        ISNULL(h.HeaderGrandTotal, SUM(cl.Amount)) AS GrandTotal
    FROM HeaderRow h
    LEFT JOIN CalculatedLines cl
        ON 1 = 1
    GROUP BY h.Estimationid, h.customername, h.City, h.Updatedon, h.HeaderGrandTotal;
END;
GO

CREATE PROCEDURE dbo.sp_report_sales
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchText NVARCHAR(200) = NULL,
    @DivisionCode VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @FromDate IS NULL
        SET @FromDate = CAST(GETDATE() AS DATE);

    IF @ToDate IS NULL
        SET @ToDate = @FromDate;

    IF @ToDate < @FromDate
        SET @ToDate = @FromDate;

    IF @PageNumber IS NULL OR @PageNumber < 1
        SET @PageNumber = 1;

    IF @PageSize IS NULL OR @PageSize < 1
        SET @PageSize = 10;

    IF @PageSize > 500
        SET @PageSize = 500;

    DECLARE @Search NVARCHAR(200) = NULLIF(LTRIM(RTRIM(@SearchText)), '');
    DECLARE @Division VARCHAR(20) = NULLIF(UPPER(LTRIM(RTRIM(@DivisionCode))), '');

    ;WITH AllHeaders AS
    (
        SELECT 'MAIN' AS DivisionCode, 'Electricals' AS DivisionName, Salesid, Updatedon, customername, City, Paymentmode, GstText, TRY_CONVERT(DECIMAL(18, 2), LessAmount) AS LessAmount, TRY_CONVERT(DECIMAL(18, 2), others) AS Others, TRY_CONVERT(DECIMAL(18, 2), GrandTotal) AS HeaderGrandTotal FROM dbo.Sales
        UNION ALL
        SELECT 'PIPES' AS DivisionCode, 'Pipes' AS DivisionName, Salesid, Updatedon, customername, City, Paymentmode, GstText, TRY_CONVERT(DECIMAL(18, 2), LessAmount) AS LessAmount, TRY_CONVERT(DECIMAL(18, 2), others) AS Others, TRY_CONVERT(DECIMAL(18, 2), GrandTotal) AS HeaderGrandTotal FROM dbo.SalesPipes
        UNION ALL
        SELECT 'TRADERS' AS DivisionCode, 'Traders' AS DivisionName, Salesid, Updatedon, customername, City, Paymentmode, GstText, TRY_CONVERT(DECIMAL(18, 2), LessAmount) AS LessAmount, TRY_CONVERT(DECIMAL(18, 2), others) AS Others, TRY_CONVERT(DECIMAL(18, 2), GrandTotal) AS HeaderGrandTotal FROM dbo.SalesTraders
    ),
    FilteredHeaders AS
    (
        SELECT *
        FROM AllHeaders ah
        WHERE ah.Updatedon >= @FromDate
          AND ah.Updatedon < DATEADD(DAY, 1, @ToDate)
          AND (@Division IS NULL OR ah.DivisionCode = @Division)
          AND (
                @Search IS NULL
                OR ah.Salesid LIKE '%' + @Search + '%'
                OR ah.customername LIKE '%' + @Search + '%'
                OR ah.City LIKE '%' + @Search + '%'
              )
    ),
    AllDetails AS
    (
        SELECT 'MAIN' AS DivisionCode, Salesid, Productid, Rate, Quantity, Amount, gst FROM dbo.SalesDetails
        UNION ALL
        SELECT 'PIPES' AS DivisionCode, Salesid, ProductId, Rate, Quantity, Amount, gst FROM dbo.SalesPipesDetails
        UNION ALL
        SELECT 'TRADERS' AS DivisionCode, Salesid, ProductId, Rate, Quantity, Amount, gst FROM dbo.SalesTradersDetails
    ),
    LineFacts AS
    (
        SELECT
            fh.DivisionCode,
            fh.Salesid,
            ad.Productid AS ProductId,
            TRY_CONVERT(DECIMAL(18, 3), ad.Quantity) AS Quantity,
            TRY_CONVERT(DECIMAL(18, 6), ad.Amount) AS GrossAmount,
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), ad.gst), 0) AS GSTRate,
            CASE WHEN UPPER(ISNULL(fh.GstText, '')) LIKE '%IGST%' THEN 1 ELSE 0 END AS IsIGST
        FROM FilteredHeaders fh
        LEFT JOIN AllDetails ad
            ON ad.DivisionCode = fh.DivisionCode
           AND ad.Salesid = fh.Salesid
    ),
    CalculatedLines AS
    (
        SELECT
            DivisionCode,
            Salesid,
            ProductId,
            Quantity,
            GrossAmount,
            GSTRate,
            IsIGST,
            CASE WHEN GSTRate > 0 THEN ISNULL(GrossAmount, 0) / (1 + (GSTRate / 100.0)) ELSE ISNULL(GrossAmount, 0) END AS TaxableAmount,
            ISNULL(GrossAmount, 0) - CASE WHEN GSTRate > 0 THEN ISNULL(GrossAmount, 0) / (1 + (GSTRate / 100.0)) ELSE ISNULL(GrossAmount, 0) END AS GSTAmount
        FROM LineFacts
    ),
    HeaderAgg AS
    (
        SELECT
            DivisionCode,
            Salesid,
            COUNT(ProductId) AS ItemCount,
            SUM(ISNULL(Quantity, 0)) AS TotalQuantity,
            SUM(TaxableAmount) AS TaxableAmount,
            SUM(CASE WHEN IsIGST = 1 THEN 0 ELSE GSTAmount / 2 END) AS CGSTAmount,
            SUM(CASE WHEN IsIGST = 1 THEN 0 ELSE GSTAmount / 2 END) AS SGSTAmount,
            SUM(CASE WHEN IsIGST = 1 THEN GSTAmount ELSE 0 END) AS IGSTAmount,
            SUM(GSTAmount) AS GSTAmount,
            SUM(ISNULL(GrossAmount, 0)) AS LineTotal
        FROM CalculatedLines
        GROUP BY DivisionCode, Salesid
    ),
    Summary AS
    (
        SELECT
            COUNT(1) AS SummaryTotalSales,
            SUM(ISNULL(ha.TaxableAmount, 0)) AS SummaryTaxableValue,
            SUM(ISNULL(ha.CGSTAmount, 0)) AS SummaryCGSTAmount,
            SUM(ISNULL(ha.SGSTAmount, 0)) AS SummarySGSTAmount,
            SUM(ISNULL(ha.IGSTAmount, 0)) AS SummaryIGSTAmount,
            SUM(ISNULL(ha.GSTAmount, 0)) AS SummaryGSTAmount,
            SUM(ISNULL(fh.HeaderGrandTotal, ISNULL(ha.LineTotal, 0))) AS SummarySalesValue,
            SUM(CASE WHEN UPPER(ISNULL(fh.GstText, '')) LIKE '%IGST%' THEN 1 ELSE 0 END) AS SummaryIGSTInvoices
        FROM FilteredHeaders fh
        LEFT JOIN HeaderAgg ha
            ON ha.DivisionCode = fh.DivisionCode
           AND ha.Salesid = fh.Salesid
    ),
    NumberedRows AS
    (
        SELECT
            fh.DivisionCode,
            fh.DivisionName,
            fh.Salesid AS SalesId,
            fh.Updatedon AS UpdatedOn,
            fh.customername AS CustomerName,
            fh.City,
            fh.Paymentmode AS PaymentMode,
            fh.GstText,
            ISNULL(ha.ItemCount, 0) AS ItemCount,
            ISNULL(ha.TotalQuantity, 0) AS TotalQuantity,
            ISNULL(ha.TaxableAmount, 0) AS TaxableAmount,
            ISNULL(ha.CGSTAmount, 0) AS CGSTAmount,
            ISNULL(ha.SGSTAmount, 0) AS SGSTAmount,
            ISNULL(ha.IGSTAmount, 0) AS IGSTAmount,
            ISNULL(ha.GSTAmount, 0) AS GSTAmount,
            ISNULL(fh.HeaderGrandTotal, ISNULL(ha.LineTotal, 0)) AS SalesValue,
            CASE WHEN UPPER(ISNULL(fh.GstText, '')) LIKE '%IGST%' THEN 1 ELSE 0 END AS IsIGST,
            COUNT(1) OVER() AS TotalRows,
            ROW_NUMBER() OVER (ORDER BY fh.Updatedon DESC, fh.DivisionCode, fh.Salesid DESC) AS RowNumber
        FROM FilteredHeaders fh
        LEFT JOIN HeaderAgg ha
            ON ha.DivisionCode = fh.DivisionCode
           AND ha.Salesid = fh.Salesid
    )
    SELECT
        nr.DivisionCode,
        nr.DivisionName,
        nr.SalesId,
        nr.UpdatedOn,
        nr.CustomerName,
        nr.City,
        nr.PaymentMode,
        nr.GstText,
        nr.ItemCount,
        nr.TotalQuantity,
        nr.TaxableAmount,
        nr.CGSTAmount,
        nr.SGSTAmount,
        nr.IGSTAmount,
        nr.GSTAmount,
        nr.SalesValue,
        nr.IsIGST,
        nr.TotalRows,
        s.SummaryTotalSales,
        ISNULL(s.SummaryTaxableValue, 0) AS SummaryTaxableValue,
        ISNULL(s.SummaryCGSTAmount, 0) AS SummaryCGSTAmount,
        ISNULL(s.SummarySGSTAmount, 0) AS SummarySGSTAmount,
        ISNULL(s.SummaryIGSTAmount, 0) AS SummaryIGSTAmount,
        ISNULL(s.SummaryGSTAmount, 0) AS SummaryGSTAmount,
        ISNULL(s.SummarySalesValue, 0) AS SummarySalesValue,
        ISNULL(s.SummaryIGSTInvoices, 0) AS SummaryIGSTInvoices
    FROM NumberedRows nr
    CROSS JOIN Summary s
    WHERE nr.RowNumber BETWEEN ((@PageNumber - 1) * @PageSize) + 1 AND (@PageNumber * @PageSize)
    ORDER BY nr.RowNumber;
END;
GO

CREATE PROCEDURE dbo.sp_report_sales_detail
    @SalesId VARCHAR(50),
    @DivisionCode VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Division VARCHAR(20) = ISNULL(NULLIF(UPPER(LTRIM(RTRIM(@DivisionCode))), ''), 'MAIN');

    ;WITH HeaderRow AS
    (
        SELECT 'MAIN' AS DivisionCode, 'Electricals' AS DivisionName, Salesid, Updatedon, customername, City, Paymentmode, GstText, TRY_CONVERT(DECIMAL(18, 2), LessAmount) AS LessAmount, TRY_CONVERT(DECIMAL(18, 2), others) AS Others, TRY_CONVERT(DECIMAL(18, 2), GrandTotal) AS HeaderGrandTotal FROM dbo.Sales WHERE @Division = 'MAIN' AND Salesid = @SalesId
        UNION ALL
        SELECT 'PIPES' AS DivisionCode, 'Pipes' AS DivisionName, Salesid, Updatedon, customername, City, Paymentmode, GstText, TRY_CONVERT(DECIMAL(18, 2), LessAmount) AS LessAmount, TRY_CONVERT(DECIMAL(18, 2), others) AS Others, TRY_CONVERT(DECIMAL(18, 2), GrandTotal) AS HeaderGrandTotal FROM dbo.SalesPipes WHERE @Division = 'PIPES' AND Salesid = @SalesId
        UNION ALL
        SELECT 'TRADERS' AS DivisionCode, 'Traders' AS DivisionName, Salesid, Updatedon, customername, City, Paymentmode, GstText, TRY_CONVERT(DECIMAL(18, 2), LessAmount) AS LessAmount, TRY_CONVERT(DECIMAL(18, 2), others) AS Others, TRY_CONVERT(DECIMAL(18, 2), GrandTotal) AS HeaderGrandTotal FROM dbo.SalesTraders WHERE @Division = 'TRADERS' AND Salesid = @SalesId
    ),
    AllDetails AS
    (
        SELECT 'MAIN' AS DivisionCode, Salesid, Productid, Rate, Quantity, Amount, gst FROM dbo.SalesDetails WHERE @Division = 'MAIN'
        UNION ALL
        SELECT 'PIPES' AS DivisionCode, Salesid, ProductId, Rate, Quantity, Amount, gst FROM dbo.SalesPipesDetails WHERE @Division = 'PIPES'
        UNION ALL
        SELECT 'TRADERS' AS DivisionCode, Salesid, ProductId, Rate, Quantity, Amount, gst FROM dbo.SalesTradersDetails WHERE @Division = 'TRADERS'
    ),
    LineFacts AS
    (
        SELECT
            ad.Productid AS ProductId,
            COALESCE(NULLIF(pm.ItemName, ''), NULLIF(pm.DisplayName, ''), ad.Productid) AS ProductName,
            TRY_CONVERT(DECIMAL(18, 2), ad.Rate) AS Rate,
            TRY_CONVERT(DECIMAL(18, 3), ad.Quantity) AS Quantity,
            TRY_CONVERT(DECIMAL(18, 6), ad.Amount) AS Amount,
            ISNULL(TRY_CONVERT(DECIMAL(18, 6), ad.gst), 0) AS GST,
            CASE WHEN UPPER(ISNULL(h.GstText, '')) LIKE '%IGST%' THEN 1 ELSE 0 END AS IsIGST
        FROM HeaderRow h
        INNER JOIN AllDetails ad
            ON ad.DivisionCode = h.DivisionCode
           AND ad.Salesid = h.Salesid
        LEFT JOIN dbo.ProductMaster pm
            ON CONVERT(VARCHAR(50), pm.id) = CONVERT(VARCHAR(50), ad.Productid)
    ),
    CalculatedLines AS
    (
        SELECT
            ProductId,
            ProductName,
            Rate,
            Quantity,
            Amount,
            GST,
            IsIGST,
            CASE WHEN GST > 0 THEN ISNULL(Amount, 0) / (1 + (GST / 100.0)) ELSE ISNULL(Amount, 0) END AS TaxableValue,
            ISNULL(Amount, 0) - CASE WHEN GST > 0 THEN ISNULL(Amount, 0) / (1 + (GST / 100.0)) ELSE ISNULL(Amount, 0) END AS GSTAmount
        FROM LineFacts
    ),
    TaxLines AS
    (
        SELECT
            ProductId,
            ProductName,
            Rate,
            Quantity,
            Amount,
            GST,
            IsIGST,
            TaxableValue,
            CASE WHEN IsIGST = 1 THEN 0 ELSE GSTAmount / 2 END AS CGSTAmount,
            CASE WHEN IsIGST = 1 THEN 0 ELSE GSTAmount / 2 END AS SGSTAmount,
            CASE WHEN IsIGST = 1 THEN GSTAmount ELSE 0 END AS IGSTAmount,
            GSTAmount
        FROM CalculatedLines
    )
    SELECT
        'DETAIL' AS RowType,
        h.DivisionCode,
        h.DivisionName,
        h.Salesid AS SalesId,
        h.customername AS CustomerName,
        h.City,
        h.Paymentmode AS PaymentMode,
        h.GstText,
        CASE WHEN UPPER(ISNULL(h.GstText, '')) LIKE '%IGST%' THEN 1 ELSE 0 END AS IsIGST,
        h.Updatedon AS UpdatedOn,
        tl.ProductId,
        tl.ProductName,
        tl.Rate,
        tl.Quantity,
        tl.GST,
        tl.TaxableValue,
        tl.CGSTAmount,
        tl.SGSTAmount,
        tl.IGSTAmount,
        tl.GSTAmount,
        tl.Amount,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalTaxable,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalCGST,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalSGST,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalIGST,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalGST,
        CAST(NULL AS DECIMAL(18, 2)) AS LessAmount,
        CAST(NULL AS DECIMAL(18, 2)) AS Others,
        CAST(NULL AS DECIMAL(18, 2)) AS GrandTotal
    FROM HeaderRow h
    INNER JOIN TaxLines tl
        ON 1 = 1

    UNION ALL

    SELECT
        'GST_SUMMARY' AS RowType,
        h.DivisionCode,
        h.DivisionName,
        h.Salesid AS SalesId,
        h.customername AS CustomerName,
        h.City,
        h.Paymentmode AS PaymentMode,
        h.GstText,
        CASE WHEN UPPER(ISNULL(h.GstText, '')) LIKE '%IGST%' THEN 1 ELSE 0 END AS IsIGST,
        h.Updatedon AS UpdatedOn,
        NULL AS ProductId,
        NULL AS ProductName,
        NULL AS Rate,
        NULL AS Quantity,
        tl.GST,
        SUM(tl.TaxableValue) AS TaxableValue,
        SUM(tl.CGSTAmount) AS CGSTAmount,
        SUM(tl.SGSTAmount) AS SGSTAmount,
        SUM(tl.IGSTAmount) AS IGSTAmount,
        SUM(tl.GSTAmount) AS GSTAmount,
        SUM(tl.Amount) AS Amount,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalTaxable,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalCGST,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalSGST,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalIGST,
        CAST(NULL AS DECIMAL(18, 2)) AS TotalGST,
        CAST(NULL AS DECIMAL(18, 2)) AS LessAmount,
        CAST(NULL AS DECIMAL(18, 2)) AS Others,
        CAST(NULL AS DECIMAL(18, 2)) AS GrandTotal
    FROM HeaderRow h
    INNER JOIN TaxLines tl
        ON 1 = 1
    GROUP BY h.DivisionCode, h.DivisionName, h.Salesid, h.customername, h.City, h.Paymentmode, h.GstText, h.Updatedon, tl.GST

    UNION ALL

    SELECT
        'TOTAL' AS RowType,
        h.DivisionCode,
        h.DivisionName,
        h.Salesid AS SalesId,
        h.customername AS CustomerName,
        h.City,
        h.Paymentmode AS PaymentMode,
        h.GstText,
        CASE WHEN UPPER(ISNULL(h.GstText, '')) LIKE '%IGST%' THEN 1 ELSE 0 END AS IsIGST,
        h.Updatedon AS UpdatedOn,
        NULL AS ProductId,
        NULL AS ProductName,
        NULL AS Rate,
        NULL AS Quantity,
        NULL AS GST,
        NULL AS TaxableValue,
        NULL AS CGSTAmount,
        NULL AS SGSTAmount,
        NULL AS IGSTAmount,
        NULL AS GSTAmount,
        NULL AS Amount,
        SUM(tl.TaxableValue) AS TotalTaxable,
        SUM(tl.CGSTAmount) AS TotalCGST,
        SUM(tl.SGSTAmount) AS TotalSGST,
        SUM(tl.IGSTAmount) AS TotalIGST,
        SUM(tl.GSTAmount) AS TotalGST,
        ISNULL(h.LessAmount, 0) AS LessAmount,
        ISNULL(h.Others, 0) AS Others,
        ISNULL(h.HeaderGrandTotal, SUM(tl.Amount)) AS GrandTotal
    FROM HeaderRow h
    LEFT JOIN TaxLines tl
        ON 1 = 1
    GROUP BY h.DivisionCode, h.DivisionName, h.Salesid, h.customername, h.City, h.Paymentmode, h.GstText, h.Updatedon, h.LessAmount, h.Others, h.HeaderGrandTotal;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuotationEstimation_Updatedon' AND object_id = OBJECT_ID('dbo.QuotationEstimation'))
    CREATE NONCLUSTERED INDEX IX_QuotationEstimation_Updatedon
    ON dbo.QuotationEstimation (Updatedon)
    INCLUDE (Estimationid, Quotationid, customername, City, AssistName, Iscancel, isdelete, GrnandTotal);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuotationEstimationDetails_Estimationid' AND object_id = OBJECT_ID('dbo.QuotationEstimationDetails'))
    CREATE NONCLUSTERED INDEX IX_QuotationEstimationDetails_Estimationid
    ON dbo.QuotationEstimationDetails (Estimationid)
    INCLUDE (Productid, Rate, Quantity, Amount);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Sales_Updatedon' AND object_id = OBJECT_ID('dbo.Sales'))
    CREATE NONCLUSTERED INDEX IX_Sales_Updatedon ON dbo.Sales (Updatedon) INCLUDE (Salesid, customername, City, Paymentmode, GstText, LessAmount, others, GrandTotal);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SalesPipes_Updatedon' AND object_id = OBJECT_ID('dbo.SalesPipes'))
    CREATE NONCLUSTERED INDEX IX_SalesPipes_Updatedon ON dbo.SalesPipes (Updatedon) INCLUDE (Salesid, customername, City, Paymentmode, GstText, LessAmount, others, GrandTotal);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SalesTraders_Updatedon' AND object_id = OBJECT_ID('dbo.SalesTraders'))
    CREATE NONCLUSTERED INDEX IX_SalesTraders_Updatedon ON dbo.SalesTraders (Updatedon) INCLUDE (Salesid, customername, City, Paymentmode, GstText, LessAmount, others, GrandTotal);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SalesDetails_Salesid' AND object_id = OBJECT_ID('dbo.SalesDetails'))
    CREATE NONCLUSTERED INDEX IX_SalesDetails_Salesid ON dbo.SalesDetails (Salesid) INCLUDE (Productid, Rate, Quantity, Amount, gst);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SalesPipesDetails_Salesid' AND object_id = OBJECT_ID('dbo.SalesPipesDetails'))
    CREATE NONCLUSTERED INDEX IX_SalesPipesDetails_Salesid ON dbo.SalesPipesDetails (Salesid) INCLUDE (ProductId, Rate, Quantity, Amount, gst);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SalesTradersDetails_Salesid' AND object_id = OBJECT_ID('dbo.SalesTradersDetails'))
    CREATE NONCLUSTERED INDEX IX_SalesTradersDetails_Salesid ON dbo.SalesTradersDetails (Salesid) INCLUDE (ProductId, Rate, Quantity, Amount, gst);
GO

UPDATE STATISTICS dbo.QuotationEstimation;
UPDATE STATISTICS dbo.QuotationEstimationDetails;
UPDATE STATISTICS dbo.Sales;
UPDATE STATISTICS dbo.SalesDetails;
UPDATE STATISTICS dbo.SalesPipes;
UPDATE STATISTICS dbo.SalesPipesDetails;
UPDATE STATISTICS dbo.SalesTraders;
UPDATE STATISTICS dbo.SalesTradersDetails;
GO
