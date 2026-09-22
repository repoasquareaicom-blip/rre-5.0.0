/*
  Rack-wise stock report detail deployment script.

  This creates only dbo.GetStockRackWiseDetail. It does not alter
  dbo.GetStockNew_Direct1 or dbo.GetStockRackWiseReport.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.GetStockRackWiseDetail', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.GetStockRackWiseDetail AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.GetStockRackWiseDetail
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Balances AS
    (
        SELECT
            mt.MaterailId AS ProductId,
            mt.LocationId,
            mt.RackId,
            CAST(SUM(
                CASE
                    WHEN UPPER(ISNULL(mt.[Type], '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
                    WHEN UPPER(ISNULL(mt.[Type], '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
                    ELSE 0
                END
            ) AS DECIMAL(18,3)) AS Quantity
        FROM dbo.MaterialTranscation mt
        WHERE mt.MaterailId = @ProductId
        GROUP BY mt.MaterailId, mt.LocationId, mt.RackId
    )
    SELECT
        b.LocationId,
        ISNULL(lm.LocationName, 'Location ' + CAST(ISNULL(b.LocationId, 0) AS VARCHAR(20))) AS LocationName,
        b.RackId,
        CASE
            WHEN b.RackId IS NULL THEN 'Previous / No Rack'
            ELSE ISNULL(rm.RackCaption, 'Rack ' + CAST(b.RackId AS VARCHAR(20)))
        END AS RackCaption,
        b.Quantity
    FROM Balances b
    LEFT JOIN dbo.LocationMaster lm
        ON lm.LocationId = b.LocationId
    LEFT JOIN dbo.RackMaster rm
        ON rm.RackId = b.RackId
    WHERE b.Quantity <> 0
    ORDER BY
        ISNULL(lm.DisplayOrder, 2147483647),
        ISNULL(rm.DisplayOrder, 2147483647),
        b.LocationId,
        b.RackId;
END
GO
