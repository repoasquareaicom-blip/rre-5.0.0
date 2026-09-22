/*
  Product Rack Mapping deployment script.

  ProductMaster.id currently has no primary key or unique constraint in the live database,
  so ProductRackMapping.ProductId intentionally has no foreign key to ProductMaster.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.ProductRackMapping', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductRackMapping
    (
        ProductRackMappingId INT IDENTITY(1,1) NOT NULL,
        ProductId INT NOT NULL,
        RackId INT NOT NULL,
        CreatedOn DATETIME NOT NULL CONSTRAINT DF_ProductRackMapping_CreatedOn DEFAULT (GETDATE()),
        UpdatedOn DATETIME NULL,
        UpdatedBy INT NULL,
        CONSTRAINT PK_ProductRackMapping PRIMARY KEY CLUSTERED (ProductRackMappingId)
    );
END
GO

IF OBJECT_ID('dbo.ProductRackMapping', 'U') IS NOT NULL
   AND OBJECT_ID('dbo.RackMaster', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ProductRackMapping_RackMaster')
BEGIN
    ALTER TABLE dbo.ProductRackMapping
    ADD CONSTRAINT FK_ProductRackMapping_RackMaster FOREIGN KEY (RackId)
        REFERENCES dbo.RackMaster (RackId);
END
GO

IF OBJECT_ID('dbo.ProductRackMapping', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.ProductRackMapping') AND name = 'UX_ProductRackMapping_ProductId_RackId')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_ProductRackMapping_ProductId_RackId
    ON dbo.ProductRackMapping (ProductId, RackId);
END
GO

IF OBJECT_ID('dbo.sp_ProductRackMapping_ListByProduct', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_ProductRackMapping_ListByProduct AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_ProductRackMapping_ListByProduct
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        prm.ProductRackMappingId,
        prm.ProductId,
        prm.RackId,
        rm.RackCaption,
        lm.LocationId,
        lm.LocationName,
        lm.DisplayOrder AS LocationDisplayOrder
    FROM dbo.ProductRackMapping prm
    INNER JOIN dbo.RackMaster rm
        ON rm.RackId = prm.RackId
    INNER JOIN dbo.LocationMaster lm
        ON lm.LocationId = rm.LocationId
    WHERE prm.ProductId = @ProductId
    ORDER BY
        lm.DisplayOrder,
        rm.DisplayOrder,
        rm.RackId;
END
GO

IF OBJECT_ID('dbo.sp_ProductRackMapping_Save', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_ProductRackMapping_Save AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_ProductRackMapping_Save
    @ProductId INT,
    @RackId INT,
    @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF ISNULL(@ProductId, 0) <= 0
    BEGIN
        RAISERROR('Product id should be a positive number', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.RackMaster rm
        INNER JOIN dbo.LocationMaster lm
            ON lm.LocationId = rm.LocationId
        WHERE rm.RackId = @RackId
          AND rm.IsActive = 1
          AND lm.IsActive = 1
    )
    BEGIN
        RAISERROR('Select valid active rack', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM dbo.ProductRackMapping
        WHERE ProductId = @ProductId
          AND RackId = @RackId
    )
    BEGIN
        RETURN;
    END

    INSERT INTO dbo.ProductRackMapping (ProductId, RackId, CreatedOn, UpdatedBy)
    VALUES (@ProductId, @RackId, GETDATE(), @UpdatedBy);
END
GO

IF OBJECT_ID('dbo.sp_ProductRackMapping_Delete', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_ProductRackMapping_Delete AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_ProductRackMapping_Delete
    @ProductId INT,
    @RackId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.ProductRackMapping
    WHERE ProductId = @ProductId
      AND RackId = @RackId;
END
GO

IF OBJECT_ID('dbo.sp_ProductRackMapping_DeleteByProduct', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_ProductRackMapping_DeleteByProduct AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_ProductRackMapping_DeleteByProduct
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.ProductRackMapping
    WHERE ProductId = @ProductId;
END
GO
