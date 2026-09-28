/*
  Read-only Material Transaction Report.
  Does not change stock posting. MaterialTranscation remains the source.
*/
IF OBJECT_ID('dbo.GetMaterialTransactionReport', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.GetMaterialTransactionReport AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.GetMaterialTransactionReport
    @FromDate DATETIME,
    @ToDateExclusive DATETIME,
    @ProductId INT = NULL,
    @LocationId VARCHAR(100) = NULL,
    @RackId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        mt.ID,
        mt.TranscationDate,
        mt.MaterailId,
        ISNULL(pm.ItemCode, '') AS ItemCode,
        ISNULL(pm.DisplayName, '') AS ProductName,
        mt.LocationId,
        CASE
            WHEN lm.LocationName IS NOT NULL AND LTRIM(RTRIM(lm.LocationName)) <> '' THEN lm.LocationName
            ELSE 'Unknown Location'
        END AS LocationName,
        mt.RackId,
        CASE
            WHEN rm.RackCaption IS NOT NULL AND LTRIM(RTRIM(rm.RackCaption)) <> '' THEN rm.RackCaption
            ELSE 'No Rack / Unassigned'
        END AS RackCaption,
        mt.Type AS MovementType,
        mt.Quantity,
        mt.TranscationType,
        mt.TransId,
        mt.Updatedby,
        ISNULL(u.UserName, '') AS UserName
    FROM dbo.MaterialTranscation mt
    LEFT JOIN dbo.ProductMaster pm
        ON pm.id = mt.MaterailId
    LEFT JOIN dbo.LocationMaster lm
        ON CONVERT(VARCHAR(100), lm.LocationId) = CONVERT(VARCHAR(100), mt.LocationId)
    LEFT JOIN dbo.RackMaster rm
        ON rm.RackId = mt.RackId
    LEFT JOIN dbo.Users u
        ON u.UserId = mt.Updatedby
    WHERE mt.TranscationDate >= @FromDate
      AND mt.TranscationDate < @ToDateExclusive
      AND (@ProductId IS NULL OR mt.MaterailId = @ProductId)
      AND (@LocationId IS NULL OR CONVERT(VARCHAR(100), mt.LocationId) = @LocationId)
      AND (@RackId IS NULL OR mt.RackId = @RackId)
    ORDER BY mt.TranscationDate, mt.ID;

    SELECT
        CONVERT(VARCHAR(100), mt.LocationId) AS LocationId,
        CASE
            WHEN lm.LocationName IS NOT NULL AND LTRIM(RTRIM(lm.LocationName)) <> '' THEN lm.LocationName
            ELSE 'Unknown Location'
        END AS LocationName,
        mt.RackId,
        CASE
            WHEN rm.RackCaption IS NOT NULL AND LTRIM(RTRIM(rm.RackCaption)) <> '' THEN rm.RackCaption
            ELSE 'No Rack / Unassigned'
        END AS RackCaption,
        SUM(CASE WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'IN' THEN mt.Quantity ELSE 0 END)
            - SUM(CASE WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'OUT' THEN mt.Quantity ELSE 0 END) AS OpeningQty
    FROM dbo.MaterialTranscation mt
    LEFT JOIN dbo.LocationMaster lm
        ON CONVERT(VARCHAR(100), lm.LocationId) = CONVERT(VARCHAR(100), mt.LocationId)
    LEFT JOIN dbo.RackMaster rm
        ON rm.RackId = mt.RackId
    WHERE (@ProductId IS NULL OR mt.MaterailId = @ProductId)
      AND mt.TranscationDate < @FromDate
      AND (@LocationId IS NULL OR CONVERT(VARCHAR(100), mt.LocationId) = @LocationId)
      AND (@RackId IS NULL OR mt.RackId = @RackId)
    GROUP BY
        CONVERT(VARCHAR(100), mt.LocationId),
        CASE
            WHEN lm.LocationName IS NOT NULL AND LTRIM(RTRIM(lm.LocationName)) <> '' THEN lm.LocationName
            ELSE 'Unknown Location'
        END,
        mt.RackId,
        CASE
            WHEN rm.RackCaption IS NOT NULL AND LTRIM(RTRIM(rm.RackCaption)) <> '' THEN rm.RackCaption
            ELSE 'No Rack / Unassigned'
        END;

    SELECT
        CONVERT(VARCHAR(100), mt.LocationId) AS LocationId,
        CASE
            WHEN lm.LocationName IS NOT NULL AND LTRIM(RTRIM(lm.LocationName)) <> '' THEN lm.LocationName
            ELSE 'Unknown Location'
        END AS LocationName,
        mt.RackId,
        CASE
            WHEN rm.RackCaption IS NOT NULL AND LTRIM(RTRIM(rm.RackCaption)) <> '' THEN rm.RackCaption
            ELSE 'No Rack / Unassigned'
        END AS RackCaption,
        SUM(CASE WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'IN' THEN mt.Quantity ELSE 0 END) AS InQty,
        SUM(CASE WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'OUT' THEN mt.Quantity ELSE 0 END) AS OutQty
    FROM dbo.MaterialTranscation mt
    LEFT JOIN dbo.LocationMaster lm
        ON CONVERT(VARCHAR(100), lm.LocationId) = CONVERT(VARCHAR(100), mt.LocationId)
    LEFT JOIN dbo.RackMaster rm
        ON rm.RackId = mt.RackId
    WHERE (@ProductId IS NULL OR mt.MaterailId = @ProductId)
      AND mt.TranscationDate >= @FromDate
      AND mt.TranscationDate < @ToDateExclusive
      AND (@LocationId IS NULL OR CONVERT(VARCHAR(100), mt.LocationId) = @LocationId)
      AND (@RackId IS NULL OR mt.RackId = @RackId)
    GROUP BY
        CONVERT(VARCHAR(100), mt.LocationId),
        CASE
            WHEN lm.LocationName IS NOT NULL AND LTRIM(RTRIM(lm.LocationName)) <> '' THEN lm.LocationName
            ELSE 'Unknown Location'
        END,
        mt.RackId,
        CASE
            WHEN rm.RackCaption IS NOT NULL AND LTRIM(RTRIM(rm.RackCaption)) <> '' THEN rm.RackCaption
            ELSE 'No Rack / Unassigned'
        END;
END
GO
