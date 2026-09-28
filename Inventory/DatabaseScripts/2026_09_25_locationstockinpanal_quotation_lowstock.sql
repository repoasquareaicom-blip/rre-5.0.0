/*
  Quotation low-stock print uses LocationstockinPanal.
  Sales stock is IN minus OUT on racks mapped to the product where AllowForSales = 1.
  Damage is the same calculation where AllowForSales = 0.
*/
IF OBJECT_ID('dbo.LocationstockinPanal', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.LocationstockinPanal AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE [dbo].[LocationstockinPanal]
(
    @Productname VARCHAR(MAX) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ProductId INT;

    SELECT @ProductId = id
    FROM ProductMaster
    WHERE DisplayName = @Productname;

    ;WITH AllocatedRackStock AS
    (
        SELECT
            RM.RackId,
            RM.LocationId,
            LM.AllowForSales,

            ISNULL(SUM(
                CASE
                    WHEN UPPER(MT.Type) = 'IN'
                        THEN ISNULL(MT.Quantity, 0)
                    WHEN UPPER(MT.Type) = 'OUT'
                        THEN -ISNULL(MT.Quantity, 0)
                    ELSE 0
                END
            ), 0) AS StockQty

        FROM ProductRackMapping PRM

        INNER JOIN RackMaster RM
            ON RM.RackId = PRM.RackId

        INNER JOIN LocationMaster LM
            ON LM.LocationId = RM.LocationId

        LEFT JOIN MaterialTranscation MT
            ON MT.MaterailId = PRM.ProductId
           AND MT.RackId = PRM.RackId

        WHERE PRM.ProductId = @ProductId

        GROUP BY
            RM.RackId,
            RM.LocationId,
            LM.AllowForSales
    )

    SELECT
        CAST(ISNULL(SUM(
            CASE
                WHEN ISNULL(AllowForSales, 0) = 1
                    THEN StockQty
                ELSE 0
            END
        ), 0) AS NUMERIC(18,3)) AS Quantity,
        'Total Stock' AS LocationId
    FROM AllocatedRackStock

    UNION ALL

    SELECT
        CAST(ISNULL(SUM(
            CASE
                WHEN ISNULL(AllowForSales, 0) = 0
                    THEN StockQty
                ELSE 0
            END
        ), 0) AS NUMERIC(18,3)) AS Quantity,
        'Damage' AS LocationId
    FROM AllocatedRackStock;

END
GO
