SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.sp_product_sync_full_product', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_product_sync_full_product AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_product_sync_full_product
    @ProductId VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.ProductMaster
    WHERE CONVERT(VARCHAR(50), id) = @ProductId;
END;
GO

IF OBJECT_ID('dbo.sp_product_sync_pending_by_branch', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_product_sync_pending_by_branch AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_product_sync_pending_by_branch
    @TargetBranchCode VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @mrpExpression NVARCHAR(200) =
        CASE
            WHEN COL_LENGTH('dbo.ProductMaster', 'MRP') IS NULL THEN 'CAST(NULL AS DECIMAL(18,2))'
            ELSE 'p.[MRP]'
        END;

    DECLARE @gstExpression NVARCHAR(200) =
        CASE
            WHEN COL_LENGTH('dbo.ProductMaster', 'GST') IS NOT NULL THEN 'p.[GST]'
            WHEN COL_LENGTH('dbo.ProductMaster', 'Tax') IS NOT NULL THEN 'p.[Tax]'
            WHEN COL_LENGTH('dbo.ProductMaster', 'SGST') IS NOT NULL THEN 'p.[SGST]'
            ELSE 'CAST(NULL AS VARCHAR(50))'
        END;

    DECLARE @sql NVARCHAR(MAX) = N'
SELECT
    q.QueueId,
    q.ProductId,
    COALESCE(NULLIF(CONVERT(VARCHAR(255), p.DisplayName), ''''), CONVERT(VARCHAR(255), p.ItemName), q.ItemName) AS DisplayName,
    p.SalesPrice,
    ' + @mrpExpression + N' AS MRP,
    ' + @gstExpression + N' AS GST,
    q.Status,
    q.ChangeType,
    q.AttemptCount,
    q.LastError,
    q.LastTriedOn,
    q.TargetBranchCode
FROM dbo.ProductMasterCloudQueue q
INNER JOIN dbo.ProductMaster p
    ON CONVERT(VARCHAR(50), p.id) = q.ProductId
WHERE q.TargetBranchCode = @TargetBranchCode
  AND q.Status IN (''Pending'', ''Failed'')
ORDER BY q.ModifiedOn DESC, q.QueueId DESC';

    EXEC sp_executesql @sql, N'@TargetBranchCode VARCHAR(30)', @TargetBranchCode;
END;
GO

IF OBJECT_ID('dbo.sp_report_product_sync_pending', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_report_product_sync_pending AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_report_product_sync_pending
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        q.QueueId,
        q.ProductId,
        q.ItemName,
        q.SalesPrice,
        pm.MRP,
        pm.GST,
        q.ChangeType,
        q.Status,
        q.AttemptCount,
        q.LastError,
        q.CreatedOn,
        q.ModifiedOn,
        q.LastTriedOn,
        q.SyncedOn,
        q.TargetBranchCode
    FROM dbo.ProductMasterCloudQueue q
    LEFT JOIN dbo.ProductMaster pm
        ON TRY_CONVERT(INT, q.ProductId) = pm.Id
    WHERE
        UPPER(ISNULL(q.Status, '')) <> 'SYNCED'
    ORDER BY
        q.TargetBranchCode,
        q.CreatedOn DESC,
        q.QueueId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_report_stock', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_report_stock AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_report_stock
(
    @PageNumber INT,
    @PageSize INT,
    @SearchText NVARCHAR(200),
    @StockOperator VARCHAR(10) = NULL,
    @StockValue DECIMAL(18,3) = NULL,
    @Gst DECIMAL(18,2) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber IS NULL OR @PageNumber < 1
        SET @PageNumber = 1;

    IF @PageSize IS NULL OR @PageSize < 1
        SET @PageSize = 10;

    SET @SearchText =
        NULLIF(LTRIM(RTRIM(@SearchText)), '');

    SET @StockOperator =
        NULLIF(LTRIM(RTRIM(@StockOperator)), '');

    ;WITH StockData AS
    (
        SELECT
            pm.Id AS ProductId,
            pm.ItemName AS ProductName,
            pm.DisplayName,
            u.UOM,
            pm.HSN,

            TRY_CONVERT(
                DECIMAL(18,2),
                NULLIF(LTRIM(RTRIM(pm.SalesPrice)), '')
            ) AS SalePrice,

            pm.MRP,

            pm.GST AS GST,

            TRY_CONVERT(
                DECIMAL(18,3),
                NULLIF(LTRIM(RTRIM(pm.MinStock)), '')
            ) AS MinStock,

            CAST(
                ISNULL(
                    SUM(
                        CASE
                            WHEN UPPER(ISNULL(mt.[Type], '')) = 'IN'
                                THEN ISNULL(mt.Quantity, 0)

                            WHEN UPPER(ISNULL(mt.[Type], '')) = 'OUT'
                                THEN -ISNULL(mt.Quantity, 0)

                            ELSE 0
                        END
                    ),
                    0
                )
                AS DECIMAL(18,3)
            ) AS AvailableStock

        FROM dbo.ProductMaster pm

        LEFT JOIN dbo.UOM u
            ON pm.UOM = u.Uomid

        LEFT JOIN dbo.MaterialTranscation mt
            ON mt.MaterailId = pm.Id
           AND mt.LocationId = 6

        WHERE
            ISNULL(pm.IsDeleted, 0) = 0

            AND
            (
                @SearchText IS NULL
                OR pm.ItemName LIKE '%' + @SearchText + '%'
                OR pm.DisplayName LIKE '%' + @SearchText + '%'
                OR pm.HSN LIKE '%' + @SearchText + '%'
            )

            AND
            (
                @Gst IS NULL
                OR TRY_CONVERT(
                    DECIMAL(18,2),
                    NULLIF(LTRIM(RTRIM(pm.GST)), '')
                ) = @Gst
            )

        GROUP BY
            pm.Id,
            pm.ItemName,
            pm.DisplayName,
            u.UOM,
            pm.HSN,
            pm.SalesPrice,
            pm.MRP,
            pm.GST,
            pm.MinStock
    ),

    FilteredStock AS
    (
        SELECT *
        FROM StockData
        WHERE
               @StockValue IS NULL
            OR @StockOperator IS NULL
            OR (@StockOperator = 'EQ'  AND AvailableStock =  @StockValue)
            OR (@StockOperator = 'LTE' AND AvailableStock <= @StockValue)
            OR (@StockOperator = 'GTE' AND AvailableStock >= @StockValue)
    )

    SELECT
        ProductId,
        ProductName,
        DisplayName,
        UOM,
        HSN,
        SalePrice,
        MRP,
        GST,
        ISNULL(MinStock, 0) AS MinStock,
        AvailableStock,
        COUNT(*) OVER() AS TotalRows

    FROM FilteredStock

    ORDER BY
        DisplayName,
        ProductId

    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO
