/*
  Block ProductRackMapping removal when that product still has a non-zero
  rack balance in MaterialTranscation. Recalculate LocationstockinPanal
  from the transaction ledger, classified by LocationMaster.AllowForSales.
*/
IF OBJECT_ID('dbo.sp_ProductRackMapping_Delete', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.sp_ProductRackMapping_Delete AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.sp_ProductRackMapping_Delete
    @ProductId INT,
    @RackId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Balance DECIMAL(18,3);

    SELECT @Balance = CAST(ISNULL(SUM(
        CASE
            WHEN UPPER(LTRIM(RTRIM(Type))) = 'IN' THEN ISNULL(Quantity, 0)
            WHEN UPPER(LTRIM(RTRIM(Type))) = 'OUT' THEN -ISNULL(Quantity, 0)
            ELSE 0
        END
    ), 0) AS DECIMAL(18,3))
    FROM dbo.MaterialTranscation
    WHERE MaterailId = @ProductId
      AND RackId = @RackId;

    IF ISNULL(@Balance, 0) <> 0
    BEGIN
        RAISERROR('This rack currently contains stock for this product. Move the stock to another rack before removing the rack allocation.', 16, 1);
        RETURN;
    END

    DELETE FROM dbo.ProductRackMapping
    WHERE ProductId = @ProductId
      AND RackId = @RackId;
END
GO

IF OBJECT_ID('dbo.sp_ProductRackMapping_DeleteByProduct', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.sp_ProductRackMapping_DeleteByProduct AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.sp_ProductRackMapping_DeleteByProduct
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE prm
    FROM dbo.ProductRackMapping prm
    WHERE prm.ProductId = @ProductId
      AND CAST(ISNULL((
            SELECT SUM(
                CASE
                    WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'IN' THEN ISNULL(mt.Quantity, 0)
                    WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
                    ELSE 0
                END
            )
            FROM dbo.MaterialTranscation mt
            WHERE mt.MaterailId = prm.ProductId
              AND mt.RackId = prm.RackId
      ), 0) AS DECIMAL(18,3)) = 0;
END
GO

IF OBJECT_ID('dbo.LocationstockinPanal', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.LocationstockinPanal AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE [dbo].[LocationstockinPanal]
    @Productname VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ProductId INT;

    SELECT @ProductId = id
    FROM dbo.ProductMaster
    WHERE DisplayName = @Productname;

    ;WITH RackStock AS
    (
        SELECT
            lm.AllowForSales,
            CAST(SUM(
                CASE
                    WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'IN' THEN ISNULL(mt.Quantity, 0)
                    WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
                    ELSE 0
                END
            ) AS NUMERIC(18,3)) AS StockQty
        FROM dbo.MaterialTranscation mt
        INNER JOIN dbo.RackMaster rm
            ON rm.RackId = mt.RackId
        INNER JOIN dbo.LocationMaster lm
            ON lm.LocationId = rm.LocationId
        WHERE mt.MaterailId = @ProductId
          AND mt.RackId IS NOT NULL
        GROUP BY lm.AllowForSales
    )
    SELECT
        CAST(ISNULL(SUM(CASE WHEN AllowForSales = 1 THEN StockQty ELSE 0 END), 0) AS NUMERIC(18,3)) AS Quantity,
        'Total Stock' AS LocationId
    FROM RackStock
    UNION ALL
    SELECT
        CAST(ISNULL(SUM(CASE WHEN AllowForSales = 0 THEN StockQty ELSE 0 END), 0) AS NUMERIC(18,3)) AS Quantity,
        'Damage' AS LocationId
    FROM RackStock;
END
GO
