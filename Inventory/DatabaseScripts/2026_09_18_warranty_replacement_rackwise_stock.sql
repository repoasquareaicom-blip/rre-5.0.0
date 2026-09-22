/*
  Rack-wise stock entry for Warranty Replacement New tab.
  Existing warranty header/detail procedures are untouched.
*/

IF OBJECT_ID('dbo.DbDeploymentHistory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DbDeploymentHistory
    (
        ScriptName VARCHAR(255) NOT NULL PRIMARY KEY,
        AppliedOn DATETIME NOT NULL DEFAULT (GETDATE())
    );
END;
GO

IF TYPE_ID('dbo.WarrantyReplacementRackDetailType') IS NULL
BEGIN
    CREATE TYPE dbo.WarrantyReplacementRackDetailType AS TABLE
    (
        TransId VARCHAR(50) NOT NULL,
        ProductId INT NOT NULL,
        RackId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL
    );
END;
GO

IF OBJECT_ID('dbo.SaveWarrantyReplacement_RackWiseStock', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.SaveWarrantyReplacement_RackWiseStock AS BEGIN SET NOCOUNT ON; END');
END;
GO

ALTER PROCEDURE dbo.SaveWarrantyReplacement_RackWiseStock
(
    @RackDetails dbo.WarrantyReplacementRackDetailType READONLY,
    @UpdatedBy VARCHAR(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM @RackDetails)
        BEGIN
            RETURN;
        END;

        IF COL_LENGTH('dbo.MaterialTranscation', 'RackId') IS NULL
        BEGIN
            RAISERROR('MaterialTranscation.RackId is required for rack-wise warranty stock.', 16, 1);
        END;

        IF EXISTS (SELECT 1 FROM @RackDetails WHERE Quantity <= 0)
        BEGIN
            RAISERROR('Warranty rack quantity must be greater than zero.', 16, 1);
        END;

        IF EXISTS
        (
            SELECT 1
            FROM @RackDetails rd
            LEFT JOIN dbo.ProductMaster pm
                ON pm.id = rd.ProductId
            WHERE pm.id IS NULL
               OR ISNULL(pm.IsDeleted, '0') = '1'
        )
        BEGIN
            RAISERROR('Invalid product in warranty rack stock.', 16, 1);
        END;

        IF EXISTS
        (
            SELECT 1
            FROM @RackDetails rd
            LEFT JOIN dbo.RackMaster rm
                ON rm.RackId = rd.RackId
            LEFT JOIN dbo.LocationMaster lm
                ON lm.LocationId = rm.LocationId
            LEFT JOIN dbo.ProductRackMapping prm
                ON prm.ProductId = rd.ProductId
               AND prm.RackId = rd.RackId
            WHERE rm.RackId IS NULL
               OR ISNULL(rm.IsActive, 0) <> 1
               OR lm.LocationId IS NULL
               OR ISNULL(lm.IsActive, 0) <> 1
               OR prm.ProductRackMappingId IS NULL
        )
        BEGIN
            RAISERROR('Invalid or inactive rack mapping in warranty rack stock.', 16, 1);
        END;

        BEGIN TRANSACTION;

        DELETE mt
        FROM dbo.MaterialTranscation mt
        INNER JOIN
        (
            SELECT DISTINCT TransId
            FROM @RackDetails
        ) rd
            ON CONVERT(VARCHAR(50), mt.TransId) = rd.TransId
        WHERE mt.TranscationType = 'Warranty Replacement';

        INSERT INTO dbo.MaterialTranscation
        (
            TransId,
            TranscationType,
            TranscationDate,
            MaterailId,
            Quantity,
            LocationId,
            RackId,
            Type
        )
        SELECT
            rd.TransId,
            'Warranty Replacement',
            GETDATE(),
            rd.ProductId,
            rd.Quantity,
            rm.LocationId,
            rd.RackId,
            'IN'
        FROM @RackDetails rd
        INNER JOIN dbo.RackMaster rm
            ON rm.RackId = rd.RackId
        WHERE rd.Quantity > 0;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.DbDeploymentHistory
    WHERE ScriptName = '2026_09_18_warranty_replacement_rackwise_stock.sql'
)
BEGIN
    INSERT INTO dbo.DbDeploymentHistory (ScriptName)
    VALUES ('2026_09_18_warranty_replacement_rackwise_stock.sql');
END;
GO
