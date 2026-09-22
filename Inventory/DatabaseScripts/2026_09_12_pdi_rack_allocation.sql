/*
  PDI rack-wise allocation deployment script.
  Keeps dbo.SaveQuotationPdi_Direct_ProductNo unchanged for old EXE compatibility.
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

IF TYPE_ID('dbo.PdiRackAllocationType') IS NULL
BEGIN
    CREATE TYPE dbo.PdiRackAllocationType AS TABLE
    (
        ProductId INT NOT NULL,
        RackId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.PdiRackAllocation', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PdiRackAllocation
    (
        PdiRackAllocationId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PdiRackAllocation PRIMARY KEY,
        QuotationId VARCHAR(100) NOT NULL,
        ProductId INT NOT NULL,
        LocationId INT NOT NULL,
        RackId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL,
        CreatedOn DATETIME NOT NULL CONSTRAINT DF_PdiRackAllocation_CreatedOn DEFAULT (GETDATE()),
        UpdatedOn DATETIME NULL,
        UpdatedBy VARCHAR(100) NULL
    );
END
GO

IF OBJECT_ID('dbo.PdiRackAllocation', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.PdiRackAllocation') AND name = 'UX_PdiRackAllocation_Quotation_Product_Rack')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_PdiRackAllocation_Quotation_Product_Rack
    ON dbo.PdiRackAllocation (QuotationId, ProductId, RackId);
END
GO

IF OBJECT_ID('dbo.PdiRackAllocation', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.PdiRackAllocation') AND name = 'IX_PdiRackAllocation_Product_Rack')
BEGIN
    CREATE NONCLUSTERED INDEX IX_PdiRackAllocation_Product_Rack
    ON dbo.PdiRackAllocation (ProductId, RackId);
END
GO

IF OBJECT_ID('dbo.PdiRackAllocation', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PdiRackAllocation_RackMaster')
BEGIN
    ALTER TABLE dbo.PdiRackAllocation
    ADD CONSTRAINT FK_PdiRackAllocation_RackMaster FOREIGN KEY (RackId)
    REFERENCES dbo.RackMaster (RackId);
END
GO

IF OBJECT_ID('dbo.SaveQuotationPdi_RackWise', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.SaveQuotationPdi_RackWise AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.SaveQuotationPdi_RackWise
(
    @isnew INT,
    @Quotationid VARCHAR(100) = NULL,
    @Customerid VARCHAR(100) = NULL,
    @date DATETIME = NULL,
    @Referenceid VARCHAR(100) = NULL,
    @Assist VARCHAR(100) = NULL,
    @status VARCHAR(100) = NULL,
    @Updatedby VARCHAR(100) = NULL,
    @AssistName VARCHAR(100) = NULL,
    @Customername VARCHAR(250) = NULL,
    @City VARCHAR(100) = NULL,
    @QuotationDetails dbo.QuotationPditype_2 READONLY,
    @RackDetails dbo.PdiRackAllocationType READONLY,
    @output INT OUTPUT,
    @result VARCHAR(100) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @output = 0;
    SET @result = '';

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM @RackDetails WHERE Quantity <= 0)
        BEGIN
            RAISERROR('Rack allocation quantity should be greater than zero.', 16, 1);
        END

        IF EXISTS (
            SELECT ProductId, RackId
            FROM @RackDetails
            GROUP BY ProductId, RackId
            HAVING COUNT(*) > 1
        )
        BEGIN
            RAISERROR('Duplicate rack allocation found for product and rack.', 16, 1);
        END

        IF EXISTS (
            SELECT 1
            FROM @RackDetails rd
            LEFT JOIN dbo.RackMaster rm
                ON rm.RackId = rd.RackId
            LEFT JOIN dbo.LocationMaster lm
                ON lm.LocationId = rm.LocationId
            WHERE rm.RackId IS NULL
               OR ISNULL(rm.IsActive, 0) = 0
               OR ISNULL(lm.IsActive, 0) = 0
               OR ISNULL(lm.AllowForSales, 0) = 0
        )
        BEGIN
            RAISERROR('Invalid active sales rack allocation found.', 16, 1);
        END

        IF EXISTS (
            SELECT 1
            FROM @RackDetails rd
            WHERE NOT EXISTS (
                SELECT 1
                FROM dbo.ProductRackMapping prm
                WHERE prm.ProductId = rd.ProductId
                  AND prm.RackId = rd.RackId
            )
        )
        BEGIN
            RAISERROR('Rack allocation is not assigned to product.', 16, 1);
        END

        BEGIN TRANSACTION;

        EXEC dbo.SaveQuotationPdi_Direct_ProductNo
            @isnew = @isnew,
            @Quotationid = @Quotationid,
            @Customerid = @Customerid,
            @date = @date,
            @Referenceid = @Referenceid,
            @Assist = @Assist,
            @status = @status,
            @Updatedby = @Updatedby,
            @AssistName = @AssistName,
            @Customername = @Customername,
            @City = @City,
            @QuotationDetails = @QuotationDetails,
            @output = @output OUTPUT,
            @result = @result OUTPUT;

        DECLARE @SavedQuotationId VARCHAR(100);
        SET @SavedQuotationId = NULLIF(@result, '');
        IF @SavedQuotationId IS NULL
        BEGIN
            SET @SavedQuotationId = @Quotationid;
        END

        DELETE FROM dbo.PdiRackAllocation
        WHERE QuotationId = @SavedQuotationId;

        INSERT INTO dbo.PdiRackAllocation
        (
            QuotationId,
            ProductId,
            LocationId,
            RackId,
            Quantity,
            CreatedOn,
            UpdatedBy
        )
        SELECT
            @SavedQuotationId,
            rd.ProductId,
            rm.LocationId,
            rd.RackId,
            rd.Quantity,
            GETDATE(),
            @Updatedby
        FROM @RackDetails rd
        INNER JOIN dbo.RackMaster rm
            ON rm.RackId = rd.RackId
        WHERE rd.Quantity > 0;

        IF OBJECT_ID('dbo.MaterialTranscation', 'U') IS NOT NULL
           AND COL_LENGTH('dbo.MaterialTranscation', 'RackId') IS NOT NULL
           AND EXISTS (SELECT 1 FROM @RackDetails)
        BEGIN
            DECLARE @TransactionTypeColumn SYSNAME =
                CASE
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'TranscationType') IS NOT NULL THEN 'TranscationType'
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'TransactionType') IS NOT NULL THEN 'TransactionType'
                    ELSE NULL
                END;

            DECLARE @TransactionDateColumn SYSNAME =
                CASE
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'TranscationDate') IS NOT NULL THEN 'TranscationDate'
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'TransactionDate') IS NOT NULL THEN 'TransactionDate'
                    ELSE NULL
                END;

            DECLARE @MaterialIdColumn SYSNAME =
                CASE
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'MaterailId') IS NOT NULL THEN 'MaterailId'
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'MaterialId') IS NOT NULL THEN 'MaterialId'
                    ELSE NULL
                END;

            IF @TransactionTypeColumn IS NULL OR @TransactionDateColumn IS NULL OR @MaterialIdColumn IS NULL
            BEGIN
                RAISERROR('MaterialTranscation table does not have the expected transaction/product columns.', 16, 1);
            END

            CREATE TABLE #PdiRackMaterialTranscation
            (
                ProductId INT NOT NULL,
                LocationId INT NOT NULL,
                RackId INT NOT NULL,
                Quantity DECIMAL(18,3) NOT NULL
            );

            INSERT INTO #PdiRackMaterialTranscation (ProductId, LocationId, RackId, Quantity)
            SELECT
                rd.ProductId,
                rm.LocationId,
                rd.RackId,
                rd.Quantity
            FROM @RackDetails rd
            INNER JOIN dbo.RackMaster rm
                ON rm.RackId = rd.RackId
            WHERE rd.Quantity > 0;

            DECLARE @MaterialSql NVARCHAR(MAX) = N'
DELETE mt
FROM dbo.MaterialTranscation mt
INNER JOIN #PdiRackMaterialTranscation rd
    ON mt.' + QUOTENAME(@MaterialIdColumn) + N' = rd.ProductId
WHERE mt.TransId = @SavedQuotationId;

INSERT INTO dbo.MaterialTranscation
(
    TransId,
    ' + QUOTENAME(@TransactionTypeColumn) + N',
    ' + QUOTENAME(@TransactionDateColumn) + N',
    ' + QUOTENAME(@MaterialIdColumn) + N',
    Quantity,
    LocationId,
    RackId,
    Type,
    Updatedby
)
SELECT
    @SavedQuotationId,
    ''PDI'',
    GETDATE(),
    rd.ProductId,
    rd.Quantity,
    rd.LocationId,
    rd.RackId,
    ''OUT'',
    @Updatedby
FROM #PdiRackMaterialTranscation rd;';

            EXEC sp_executesql
                @MaterialSql,
                N'@SavedQuotationId VARCHAR(100), @Updatedby VARCHAR(100)',
                @SavedQuotationId,
                @Updatedby;
        END

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

IF NOT EXISTS (SELECT 1 FROM dbo.DbDeploymentHistory WHERE ScriptName = '2026_09_12_pdi_rack_allocation.sql')
BEGIN
    INSERT INTO dbo.DbDeploymentHistory (ScriptName, AppliedBy)
    VALUES ('2026_09_12_pdi_rack_allocation.sql', SUSER_SNAME());
END
GO
