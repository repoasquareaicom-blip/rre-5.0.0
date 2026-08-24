/*
  Product Analysis report for Reporting.Web.
  Source: completed QuotationEstimation transaction lines only.
  SQL Server 2012 compatible. Do not execute directly against production without review.
*/

IF OBJECT_ID('dbo.Proc_ProductSalesAnalysis', 'P') IS NOT NULL
    DROP PROCEDURE dbo.Proc_ProductSalesAnalysis;
GO

CREATE PROCEDURE dbo.Proc_ProductSalesAnalysis
    @FromDate DATE,
    @ToDate DATE,
    @ProductSearch VARCHAR(200) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 500,
    @SortBy VARCHAR(30) = 'TRANSDATE',
    @SortDirection VARCHAR(4) = 'DESC'
AS
BEGIN
    SET NOCOUNT ON;

    IF @FromDate IS NULL OR @ToDate IS NULL
    BEGIN
        RAISERROR('FromDate and ToDate are required.', 16, 1);
        RETURN;
    END;

    IF @ToDate < @FromDate
    BEGIN
        RAISERROR('ToDate must be greater than or equal to FromDate.', 16, 1);
        RETURN;
    END;

    IF DATEDIFF(DAY, @FromDate, @ToDate) > 34
    BEGIN
        RAISERROR('Maximum allowed date range is 35 days.', 16, 1);
        RETURN;
    END;

    IF @PageNumber IS NULL OR @PageNumber < 1
        SET @PageNumber = 1;

    IF @PageSize IS NULL OR @PageSize < 1
        SET @PageSize = 500;

    IF @PageSize > 500
        SET @PageSize = 500;

    SET @ProductSearch = NULLIF(LTRIM(RTRIM(@ProductSearch)), '');
    SET @SortBy = UPPER(LTRIM(RTRIM(ISNULL(@SortBy, 'TRANSDATE'))));
    SET @SortDirection = UPPER(LTRIM(RTRIM(ISNULL(@SortDirection, 'DESC'))));

    IF @SortBy NOT IN ('TRANSID', 'PRODUCTNAME', 'CUSTOMERNAME', 'CITY', 'BRAND', 'CATEGORY', 'TRANSDATE', 'TRANSQTY', 'PRICE', 'TOTALPRICE')
        SET @SortBy = 'TRANSDATE';

    IF @SortDirection NOT IN ('ASC', 'DESC')
        SET @SortDirection = 'DESC';

    ;WITH TransactionLines AS
    (
        SELECT
            LTRIM(RTRIM(CONVERT(VARCHAR(50), qe.Estimationid))) AS TransId,
            LTRIM(RTRIM(CONVERT(VARCHAR(50), qed.Productid))) AS ProductId,
            ISNULL(pm.DisplayName, '') AS ProductName,
            ISNULL(qe.customername, '') AS CustomerName,
            ISNULL(qe.City, '') AS City,
            ISNULL(br.Brandname, '') AS Brand,
            ISNULL(cat.categoryname, '') AS Category,
            qe.[date] AS TransDate,
            ISNULL(TRY_CONVERT(DECIMAL(18, 3), NULLIF(LTRIM(RTRIM(CONVERT(VARCHAR(50), qed.Quantity))), '')), 0) AS TransQty,
            ISNULL(TRY_CONVERT(DECIMAL(18, 2), NULLIF(LTRIM(RTRIM(CONVERT(VARCHAR(50), qed.Rate))), '')), 0) AS Price,
            ISNULL(TRY_CONVERT(DECIMAL(18, 2), NULLIF(LTRIM(RTRIM(CONVERT(VARCHAR(50), qed.Amount))), '')), 0) AS TotalPrice
        FROM dbo.QuotationEstimation qe
        INNER JOIN dbo.QuotationEstimationDetails qed
            ON LTRIM(RTRIM(CONVERT(VARCHAR(50), qed.Estimationid))) = LTRIM(RTRIM(CONVERT(VARCHAR(50), qe.Estimationid)))
        LEFT JOIN dbo.ProductMaster pm
            ON LTRIM(RTRIM(CONVERT(VARCHAR(50), pm.id))) = LTRIM(RTRIM(CONVERT(VARCHAR(50), qed.Productid)))
        LEFT JOIN dbo.Brand br
            ON pm.Brand = br.id
        LEFT JOIN dbo.Category cat
            ON pm.Category = cat.id
        WHERE LTRIM(RTRIM(CONVERT(VARCHAR(50), qe.Status))) = 'Estimation Completed'
          AND qe.[date] >= @FromDate
          AND qe.[date] < DATEADD(DAY, 1, @ToDate)
          AND ISNULL(qe.isdelete, '0') <> '1'
          AND (
                @ProductSearch IS NULL
                OR pm.DisplayName LIKE '%' + @ProductSearch + '%'
                OR br.Brandname LIKE '%' + @ProductSearch + '%'
                OR cat.categoryname LIKE '%' + @ProductSearch + '%'
              )
    ),
    Summary AS
    (
        SELECT
            COUNT(DISTINCT ProductId) AS SummaryProducts,
            COUNT(DISTINCT TransId) AS SummaryTransactions,
            SUM(TransQty) AS SummaryQuantity,
            SUM(TotalPrice) AS SummaryTotalPrice
        FROM TransactionLines
    ),
    NumberedRows AS
    (
        SELECT
            TransId,
            ProductId,
            ProductName,
            CustomerName,
            City,
            Brand,
            Category,
            TransDate,
            TransQty,
            Price,
            TotalPrice,
            COUNT(1) OVER() AS TotalRows,
            ROW_NUMBER() OVER
            (
                ORDER BY
                    CASE WHEN @SortBy = 'TRANSID' AND @SortDirection = 'ASC' THEN TransId END ASC,
                    CASE WHEN @SortBy = 'TRANSID' AND @SortDirection = 'DESC' THEN TransId END DESC,
                    CASE WHEN @SortBy = 'PRODUCTNAME' AND @SortDirection = 'ASC' THEN ProductName END ASC,
                    CASE WHEN @SortBy = 'PRODUCTNAME' AND @SortDirection = 'DESC' THEN ProductName END DESC,
                    CASE WHEN @SortBy = 'CUSTOMERNAME' AND @SortDirection = 'ASC' THEN CustomerName END ASC,
                    CASE WHEN @SortBy = 'CUSTOMERNAME' AND @SortDirection = 'DESC' THEN CustomerName END DESC,
                    CASE WHEN @SortBy = 'CITY' AND @SortDirection = 'ASC' THEN City END ASC,
                    CASE WHEN @SortBy = 'CITY' AND @SortDirection = 'DESC' THEN City END DESC,
                    CASE WHEN @SortBy = 'BRAND' AND @SortDirection = 'ASC' THEN Brand END ASC,
                    CASE WHEN @SortBy = 'BRAND' AND @SortDirection = 'DESC' THEN Brand END DESC,
                    CASE WHEN @SortBy = 'CATEGORY' AND @SortDirection = 'ASC' THEN Category END ASC,
                    CASE WHEN @SortBy = 'CATEGORY' AND @SortDirection = 'DESC' THEN Category END DESC,
                    CASE WHEN @SortBy = 'TRANSDATE' AND @SortDirection = 'ASC' THEN TransDate END ASC,
                    CASE WHEN @SortBy = 'TRANSDATE' AND @SortDirection = 'DESC' THEN TransDate END DESC,
                    CASE WHEN @SortBy = 'TRANSQTY' AND @SortDirection = 'ASC' THEN TransQty END ASC,
                    CASE WHEN @SortBy = 'TRANSQTY' AND @SortDirection = 'DESC' THEN TransQty END DESC,
                    CASE WHEN @SortBy = 'PRICE' AND @SortDirection = 'ASC' THEN Price END ASC,
                    CASE WHEN @SortBy = 'PRICE' AND @SortDirection = 'DESC' THEN Price END DESC,
                    CASE WHEN @SortBy = 'TOTALPRICE' AND @SortDirection = 'ASC' THEN TotalPrice END ASC,
                    CASE WHEN @SortBy = 'TOTALPRICE' AND @SortDirection = 'DESC' THEN TotalPrice END DESC,
                    TransDate DESC,
                    TransId DESC,
                    ProductName ASC
            ) AS RowNumber
        FROM TransactionLines
    )
    SELECT
        nr.TransId,
        nr.ProductId,
        nr.ProductName,
        nr.CustomerName,
        nr.City,
        nr.Brand,
        nr.Category,
        nr.TransDate,
        CAST(nr.TransQty AS DECIMAL(18, 3)) AS TransQty,
        CAST(nr.Price AS DECIMAL(18, 2)) AS Price,
        CAST(nr.TotalPrice AS DECIMAL(18, 2)) AS TotalPrice,
        nr.TotalRows,
        ISNULL(s.SummaryProducts, 0) AS SummaryProducts,
        ISNULL(s.SummaryTransactions, 0) AS SummaryTransactions,
        CAST(ISNULL(s.SummaryQuantity, 0) AS DECIMAL(18, 3)) AS SummaryQuantity,
        CAST(ISNULL(s.SummaryTotalPrice, 0) AS DECIMAL(18, 2)) AS SummaryTotalPrice
    FROM NumberedRows nr
    CROSS JOIN Summary s
    WHERE nr.RowNumber BETWEEN ((@PageNumber - 1) * @PageSize) + 1 AND (@PageNumber * @PageSize)
    ORDER BY nr.RowNumber;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuotationEstimation_ProductSalesAnalysis' AND object_id = OBJECT_ID('dbo.QuotationEstimation'))
    CREATE NONCLUSTERED INDEX IX_QuotationEstimation_ProductSalesAnalysis
    ON dbo.QuotationEstimation (Status, [date], Estimationid)
    INCLUDE (isdelete);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuotationEstimationDetails_ProductSalesAnalysis' AND object_id = OBJECT_ID('dbo.QuotationEstimationDetails'))
    CREATE NONCLUSTERED INDEX IX_QuotationEstimationDetails_ProductSalesAnalysis
    ON dbo.QuotationEstimationDetails (Estimationid, Productid)
    INCLUDE (Quantity, Rate, Amount);
GO
