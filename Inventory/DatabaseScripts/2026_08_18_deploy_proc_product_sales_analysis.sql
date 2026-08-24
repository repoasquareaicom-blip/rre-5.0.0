/*
  Deploy Product Analysis stored procedure to each branch database.
  This procedure is consumed by Reporting.Web Product Analysis via BranchApi /api/getdata.
  Main report output is transaction-detail rows, not product-wise grouped rows.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.Proc_ProductSalesAnalysis', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.Proc_ProductSalesAnalysis AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.Proc_ProductSalesAnalysis
(
    @FromDate       DATE,
    @ToDate         DATE,
    @ProductSearch  VARCHAR(200) = NULL,
    @PageNumber     INT = 1,
    @PageSize       INT = 500,
    @SortBy         VARCHAR(30) = 'TRANSDATE',
    @SortDirection  VARCHAR(4) = 'DESC'
)
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
            LTRIM(RTRIM(CONVERT(VARCHAR(50), e.Estimationid))) AS TransId,
            LTRIM(RTRIM(CONVERT(VARCHAR(50), d.Productid))) AS ProductId,
            ISNULL(p.DisplayName, '') AS ProductName,
            ISNULL(e.customername, '') AS CustomerName,
            ISNULL(e.City, '') AS City,
            ISNULL(br.Brandname, '') AS Brand,
            ISNULL(cat.categoryname, '') AS Category,
            e.[date] AS TransDate,
            ISNULL(TRY_CONVERT(DECIMAL(18, 3), NULLIF(LTRIM(RTRIM(CONVERT(VARCHAR(50), d.Quantity))), '')), 0) AS TransQty,
            ISNULL(TRY_CONVERT(DECIMAL(18, 2), NULLIF(LTRIM(RTRIM(CONVERT(VARCHAR(50), d.Rate))), '')), 0) AS Price,
            ISNULL(TRY_CONVERT(DECIMAL(18, 2), NULLIF(LTRIM(RTRIM(CONVERT(VARCHAR(50), d.Amount))), '')), 0) AS TotalPrice
        FROM dbo.QuotationEstimation e
        INNER JOIN dbo.QuotationEstimationDetails d
            ON LTRIM(RTRIM(CONVERT(VARCHAR(50), e.Estimationid))) = LTRIM(RTRIM(CONVERT(VARCHAR(50), d.Estimationid)))
        LEFT JOIN dbo.ProductMaster p
            ON LTRIM(RTRIM(CONVERT(VARCHAR(50), d.Productid))) = LTRIM(RTRIM(CONVERT(VARCHAR(50), p.id)))
        LEFT JOIN dbo.Brand br
            ON p.Brand = br.id
        LEFT JOIN dbo.Category cat
            ON p.Category = cat.id
        WHERE LTRIM(RTRIM(CONVERT(VARCHAR(50), e.Status))) = 'Estimation Completed'
          AND e.[date] >= @FromDate
          AND e.[date] < DATEADD(DAY, 1, @ToDate)
          AND (
                @ProductSearch IS NULL
                OR p.DisplayName LIKE '%' + @ProductSearch + '%'
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
