/*
  Reverse PDI rack-wise stock restoration.
  Does not modify dbo.SaveQuotationPdi_Direct_ProductNo or dbo.SaveQuotationPdi_RackWise.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.DbDeploymentHistory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DbDeploymentHistory
    (
        DeploymentId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DbDeploymentHistory PRIMARY KEY,
        ScriptName VARCHAR(255) NOT NULL CONSTRAINT UX_DbDeploymentHistory_ScriptName UNIQUE,
        AppliedOn DATETIME NOT NULL CONSTRAINT DF_DbDeploymentHistory_AppliedOn DEFAULT (GETDATE()),
        AppliedBy VARCHAR(100) NULL
    );
END
GO

IF TYPE_ID('dbo.PdiReverseRackDetailType') IS NULL
BEGIN
    CREATE TYPE dbo.PdiReverseRackDetailType AS TABLE
    (
        ProductId INT NOT NULL,
        RackId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.ReverseQuotationPdi_RackWise', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.ReverseQuotationPdi_RackWise AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.ReverseQuotationPdi_RackWise
(
    @QuotationId VARCHAR(100),
    @UpdatedBy VARCHAR(100),
    @RackDetails dbo.PdiReverseRackDetailType READONLY,
    @output INT OUTPUT,
    @result VARCHAR(200) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @output = 0;
    SET @result = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM dbo.QuotationHeader WHERE Quotationid = @QuotationId)
        BEGIN
            RAISERROR('Quotation does not exist.', 16, 1);
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.QuotationHeader WHERE Quotationid = @QuotationId AND ISNULL(IsPDI, 0) = 1)
        BEGIN
            RAISERROR('Quotation is not in PDI state.', 16, 1);
        END

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.MaterialTranscation
            WHERE TransId = @QuotationId
              AND TranscationType = 'PDI'
              AND Type = 'OUT'
        )
        BEGIN
            RAISERROR('Original PDI stock transaction was not found.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM dbo.MaterialTranscation
            WHERE TransId = @QuotationId
              AND TranscationType = 'PDI-REVERSE'
              AND Type = 'IN'
        )
        BEGIN
            RAISERROR('PDI has already been reversed.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM dbo.QuotationEstimation
            WHERE Quotationid = @QuotationId
              AND ISNULL(IsBilled, 0) = 1
        )
        BEGIN
            RAISERROR('PDI cannot be reversed because payment/billing has already been completed.', 16, 1);
        END

        IF EXISTS (SELECT 1 FROM @RackDetails WHERE Quantity <= 0)
        BEGIN
            RAISERROR('Reverse PDI rack quantity should be greater than zero.', 16, 1);
        END

        IF EXISTS
        (
            SELECT ProductId, RackId
            FROM @RackDetails
            GROUP BY ProductId, RackId
            HAVING COUNT(*) > 1
        )
        BEGIN
            RAISERROR('Duplicate reverse rack allocation found for product and rack.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM @RackDetails rd
            LEFT JOIN dbo.RackMaster rm
                ON rm.RackId = rd.RackId
            WHERE rm.RackId IS NULL
        )
        BEGIN
            RAISERROR('Invalid rack allocation found for Reverse PDI.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM @RackDetails rd
            INNER JOIN dbo.ProductMaster pm
                ON pm.id = rd.ProductId
            INNER JOIN dbo.RackMaster rm
                ON rm.RackId = rd.RackId
            INNER JOIN dbo.LocationMaster lm
                ON lm.LocationId = rm.LocationId
            WHERE ISNULL(pm.RackWiseStockMovement, 0) = 1
              AND
              (
                  ISNULL(rm.IsActive, 0) = 0
                  OR ISNULL(lm.IsActive, 0) = 0
                  OR NOT EXISTS
                  (
                      SELECT 1
                      FROM dbo.ProductRackMapping prm
                      WHERE prm.ProductId = rd.ProductId
                        AND prm.RackId = rd.RackId
                  )
              )
        )
        BEGIN
            RAISERROR('Reverse PDI rack allocation is not an active assigned rack for rack-wise product.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM @RackDetails rd
            INNER JOIN dbo.ProductMaster pm
                ON pm.id = rd.ProductId
            WHERE ISNULL(pm.RackWiseStockMovement, 0) = 0
              AND NOT EXISTS
            (
                SELECT 1
                FROM dbo.MaterialTranscation mt
                WHERE mt.TransId = @QuotationId
                  AND mt.TranscationType = 'PDI'
                  AND mt.Type = 'OUT'
                  AND mt.MaterailId = rd.ProductId
                  AND mt.RackId = rd.RackId
            )
        )
        BEGIN
            RAISERROR('Reverse PDI automatic rack allocation must use original PDI OUT rack.', 16, 1);
        END

        CREATE TABLE #OriginalPdiOut
        (
            ProductId INT NOT NULL,
            Quantity DECIMAL(18,3) NOT NULL
        );

        INSERT INTO #OriginalPdiOut (ProductId, Quantity)
        SELECT
            mt.MaterailId,
            CAST(SUM(ISNULL(mt.Quantity, 0)) AS DECIMAL(18,3))
        FROM dbo.MaterialTranscation mt
        WHERE mt.TransId = @QuotationId
          AND mt.TranscationType = 'PDI'
          AND mt.Type = 'OUT'
        GROUP BY mt.MaterailId;

        CREATE TABLE #ReverseRackDetails
        (
            ProductId INT NOT NULL,
            LocationId INT NOT NULL,
            RackId INT NOT NULL,
            Quantity DECIMAL(18,3) NOT NULL
        );

        INSERT INTO #ReverseRackDetails (ProductId, LocationId, RackId, Quantity)
        SELECT
            rd.ProductId,
            rm.LocationId,
            rd.RackId,
            CAST(SUM(rd.Quantity) AS DECIMAL(18,3))
        FROM @RackDetails rd
        INNER JOIN dbo.RackMaster rm
            ON rm.RackId = rd.RackId
        GROUP BY rd.ProductId, rm.LocationId, rd.RackId;

        IF EXISTS
        (
            SELECT 1
            FROM #OriginalPdiOut o
            FULL OUTER JOIN
            (
                SELECT ProductId, CAST(SUM(Quantity) AS DECIMAL(18,3)) AS Quantity
                FROM #ReverseRackDetails
                GROUP BY ProductId
            ) r
                ON r.ProductId = o.ProductId
            WHERE o.ProductId IS NULL
               OR r.ProductId IS NULL
               OR ISNULL(o.Quantity, 0) <> ISNULL(r.Quantity, 0)
        )
        BEGIN
            RAISERROR('Reverse PDI quantities must exactly match original PDI OUT quantities by product.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM
            (
                SELECT
                    mt.MaterailId AS ProductId,
                    mt.RackId,
                    CAST(SUM(ISNULL(mt.Quantity, 0)) AS DECIMAL(18,3)) AS Quantity
                FROM dbo.MaterialTranscation mt
                INNER JOIN dbo.ProductMaster pm
                    ON pm.id = mt.MaterailId
                WHERE mt.TransId = @QuotationId
                  AND mt.TranscationType = 'PDI'
                  AND mt.Type = 'OUT'
                  AND ISNULL(pm.RackWiseStockMovement, 0) = 0
                GROUP BY mt.MaterailId, mt.RackId
            ) o
            FULL OUTER JOIN
            (
                SELECT
                    rd.ProductId,
                    rd.RackId,
                    CAST(SUM(rd.Quantity) AS DECIMAL(18,3)) AS Quantity
                FROM #ReverseRackDetails rd
                INNER JOIN dbo.ProductMaster pm
                    ON pm.id = rd.ProductId
                WHERE ISNULL(pm.RackWiseStockMovement, 0) = 0
                GROUP BY rd.ProductId, rd.RackId
            ) r
                ON r.ProductId = o.ProductId
               AND r.RackId = o.RackId
            WHERE o.ProductId IS NULL
               OR r.ProductId IS NULL
               OR ISNULL(o.Quantity, 0) <> ISNULL(r.Quantity, 0)
        )
        BEGIN
            RAISERROR('Non-rack-wise Reverse PDI quantities must match original PDI OUT quantities by rack.', 16, 1);
        END

        INSERT INTO dbo.MaterialTranscation
        (
            TransId,
            TranscationType,
            TranscationDate,
            MaterailId,
            Quantity,
            LocationId,
            RackId,
            Type,
            Updatedby
        )
        SELECT
            @QuotationId,
            'PDI-REVERSE',
            GETDATE(),
            rd.ProductId,
            rd.Quantity,
            rd.LocationId,
            rd.RackId,
            'IN',
            @UpdatedBy
        FROM #ReverseRackDetails rd
        WHERE rd.Quantity > 0;

        UPDATE dbo.QuotationHeader
        SET IsPDI = 0
        WHERE Quotationid = @QuotationId;

        SET @output = 1;
        SET @result = 'PDI reversed successfully.';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END

        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DbDeploymentHistory WHERE ScriptName = '2026_09_15_reverse_pdi_rackwise.sql')
BEGIN
    INSERT INTO dbo.DbDeploymentHistory (ScriptName, AppliedBy)
    VALUES ('2026_09_15_reverse_pdi_rackwise.sql', SUSER_SNAME());
END
GO
