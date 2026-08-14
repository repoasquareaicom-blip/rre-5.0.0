SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.GetAvailableStockByProductId', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.GetAvailableStockByProductId AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.GetAvailableStockByProductId
    @ProductId INT,
    @ItemName VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.ProductMaster
        WHERE id = @ProductId
          AND LTRIM(RTRIM(ISNULL(ItemName, ''))) = LTRIM(RTRIM(ISNULL(@ItemName, '')))
    )
    BEGIN
        SELECT
            'NO' AS ReturnCode,
            'item name not matched' AS Message,
            CAST(NULL AS INT) AS ProductId,
            CAST(NULL AS DECIMAL(18, 3)) AS AvailableStock;
        RETURN;
    END

    SELECT
        'YES' AS ReturnCode,
        'Success' AS Message,
        @ProductId AS ProductId,
        CAST(
            ISNULL(SUM(
                CASE
                    WHEN UPPER(ISNULL([Type], '')) = 'IN'
                        THEN ISNULL(Quantity, 0)
                    WHEN UPPER(ISNULL([Type], '')) = 'OUT'
                        THEN -ISNULL(Quantity, 0)
                    ELSE 0
                END
            ), 0)
        AS DECIMAL(18, 3)) AS AvailableStock
    FROM dbo.MaterialTranscation
    WHERE MaterailId = @ProductId
      AND LocationId = 6;
END
GO
