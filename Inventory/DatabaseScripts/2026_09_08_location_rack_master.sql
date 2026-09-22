/*
  Location & Rack Master deployment script.

  Final deployment model:
  - dbo.LocationMaster
  - dbo.RackMaster with LocationId

  DisplayOrder on LocationMaster is used as the UI display sequence and future
  sales stock-adjustment priority.

  Migration section:
  If an existing development database still has FloorMaster/RackMaster.FloorId
  and does not yet have LocationMaster/RackMaster.LocationId, this script uses
  metadata renames to preserve data. Review before running in production.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* Migration from the previous development Floor/Rack names. */
IF OBJECT_ID('dbo.FloorMaster', 'U') IS NOT NULL
   AND OBJECT_ID('dbo.LocationMaster', 'U') IS NULL
BEGIN
    EXEC sp_rename 'dbo.FloorMaster', 'LocationMaster';
END
GO

IF OBJECT_ID('dbo.LocationMaster', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.LocationMaster', 'FloorId') IS NOT NULL
   AND COL_LENGTH('dbo.LocationMaster', 'LocationId') IS NULL
BEGIN
    EXEC sp_rename 'dbo.LocationMaster.FloorId', 'LocationId', 'COLUMN';
END
GO

IF OBJECT_ID('dbo.LocationMaster', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.LocationMaster', 'FloorName') IS NOT NULL
   AND COL_LENGTH('dbo.LocationMaster', 'LocationName') IS NULL
BEGIN
    EXEC sp_rename 'dbo.LocationMaster.FloorName', 'LocationName', 'COLUMN';
END
GO

IF OBJECT_ID('dbo.RackMaster', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.RackMaster', 'FloorId') IS NOT NULL
   AND COL_LENGTH('dbo.RackMaster', 'LocationId') IS NULL
BEGIN
    EXEC sp_rename 'dbo.RackMaster.FloorId', 'LocationId', 'COLUMN';
END
GO

IF OBJECT_ID('dbo.PK_FloorMaster', 'PK') IS NOT NULL
   AND OBJECT_ID('dbo.PK_LocationMaster', 'PK') IS NULL
BEGIN
    EXEC sp_rename 'dbo.PK_FloorMaster', 'PK_LocationMaster', 'OBJECT';
END
GO

IF OBJECT_ID('dbo.FK_RackMaster_FloorMaster', 'F') IS NOT NULL
   AND OBJECT_ID('dbo.FK_RackMaster_LocationMaster', 'F') IS NULL
BEGIN
    EXEC sp_rename 'dbo.FK_RackMaster_FloorMaster', 'FK_RackMaster_LocationMaster', 'OBJECT';
END
GO

IF OBJECT_ID('dbo.LocationMaster', 'U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.LocationMaster') AND name = 'UX_FloorMaster_FloorName')
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.LocationMaster') AND name = 'UX_LocationMaster_LocationName')
BEGIN
    EXEC sp_rename 'dbo.LocationMaster.UX_FloorMaster_FloorName', 'UX_LocationMaster_LocationName', 'INDEX';
END
GO

IF OBJECT_ID('dbo.RackMaster', 'U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.RackMaster') AND name = 'UX_RackMaster_FloorId_RackCaption')
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.RackMaster') AND name = 'UX_RackMaster_LocationId_RackCaption')
BEGIN
    EXEC sp_rename 'dbo.RackMaster.UX_RackMaster_FloorId_RackCaption', 'UX_RackMaster_LocationId_RackCaption', 'INDEX';
END
GO

IF OBJECT_ID('dbo.sp_Floor_List', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_Floor_List;
END
GO

IF OBJECT_ID('dbo.sp_Floor_Insert', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_Floor_Insert;
END
GO

IF OBJECT_ID('dbo.sp_Floor_Update', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_Floor_Update;
END
GO

IF OBJECT_ID('dbo.sp_Rack_ListByFloor', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_Rack_ListByFloor;
END
GO

IF OBJECT_ID('dbo.LocationMaster', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LocationMaster
    (
        LocationId INT IDENTITY(1,1) NOT NULL,
        LocationName VARCHAR(100) NOT NULL,
        DisplayOrder INT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_LocationMaster_IsActive DEFAULT ((1)),
        AllowForSales BIT NOT NULL CONSTRAINT DF_LocationMaster_AllowForSales DEFAULT (1),
        CreatedOn DATETIME NOT NULL CONSTRAINT DF_LocationMaster_CreatedOn DEFAULT (GETDATE()),
        UpdatedOn DATETIME NULL,
        UpdatedBy INT NULL,
        CONSTRAINT PK_LocationMaster PRIMARY KEY CLUSTERED (LocationId)
    );
END
GO

IF COL_LENGTH('dbo.LocationMaster', 'AllowForSales') IS NULL
BEGIN
    ALTER TABLE dbo.LocationMaster
    ADD AllowForSales BIT NOT NULL
        CONSTRAINT DF_LocationMaster_AllowForSales DEFAULT (1);
END
GO

IF OBJECT_ID('dbo.RackMaster', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RackMaster
    (
        RackId INT IDENTITY(1,1) NOT NULL,
        LocationId INT NOT NULL,
        RackCaption VARCHAR(100) NOT NULL,
        DisplayOrder INT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_RackMaster_IsActive DEFAULT ((1)),
        CreatedOn DATETIME NOT NULL CONSTRAINT DF_RackMaster_CreatedOn DEFAULT (GETDATE()),
        UpdatedOn DATETIME NULL,
        UpdatedBy INT NULL,
        CONSTRAINT PK_RackMaster PRIMARY KEY CLUSTERED (RackId),
        CONSTRAINT FK_RackMaster_LocationMaster FOREIGN KEY (LocationId)
            REFERENCES dbo.LocationMaster (LocationId)
    );
END
GO

IF OBJECT_ID('dbo.RackMaster', 'U') IS NOT NULL
   AND OBJECT_ID('dbo.LocationMaster', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.RackMaster', 'LocationId') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_RackMaster_LocationMaster')
BEGIN
    ALTER TABLE dbo.RackMaster
    ADD CONSTRAINT FK_RackMaster_LocationMaster FOREIGN KEY (LocationId)
        REFERENCES dbo.LocationMaster (LocationId);
END
GO

IF OBJECT_ID('dbo.LocationMaster', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.LocationMaster') AND name = 'UX_LocationMaster_LocationName')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_LocationMaster_LocationName
    ON dbo.LocationMaster (LocationName);
END
GO

IF OBJECT_ID('dbo.RackMaster', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.RackMaster') AND name = 'UX_RackMaster_LocationId_RackCaption')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_RackMaster_LocationId_RackCaption
    ON dbo.RackMaster (LocationId, RackCaption);
END
GO

IF OBJECT_ID('dbo.sp_Location_Insert', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_Location_Insert AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_Location_Insert
    @LocationName VARCHAR(100),
    @DisplayOrder INT,
    @AllowForSales BIT,
    @UpdatedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    SET @LocationName = LTRIM(RTRIM(ISNULL(@LocationName, '')));

    IF @LocationName = ''
    BEGIN
        RAISERROR('Location name should not be empty', 16, 1);
        RETURN;
    END

    IF ISNULL(@DisplayOrder, 0) <= 0
    BEGIN
        RAISERROR('Sales priority should be a positive number', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.LocationMaster WHERE LocationName = @LocationName)
    BEGIN
        RAISERROR('Location already exist', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.LocationMaster WHERE IsActive = 1 AND DisplayOrder = @DisplayOrder)
    BEGIN
        RAISERROR('Sales priority already exist', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.LocationMaster (LocationName, DisplayOrder, IsActive, AllowForSales, CreatedOn, UpdatedBy)
    VALUES (@LocationName, @DisplayOrder, 1, ISNULL(@AllowForSales, 1), GETDATE(), @UpdatedBy);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS LocationId;
END
GO

IF OBJECT_ID('dbo.sp_Location_List', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_Location_List AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_Location_List
AS
BEGIN
    SET NOCOUNT ON;

    SELECT LocationId, LocationName, DisplayOrder, IsActive, AllowForSales, CreatedOn, UpdatedOn, UpdatedBy
    FROM dbo.LocationMaster
    WHERE IsActive = 1
    ORDER BY DisplayOrder, LocationName;
END
GO

IF OBJECT_ID('dbo.sp_Location_Update', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_Location_Update AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_Location_Update
    @LocationId INT,
    @LocationName VARCHAR(100),
    @DisplayOrder INT,
    @AllowForSales BIT,
    @UpdatedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    SET @LocationName = LTRIM(RTRIM(ISNULL(@LocationName, '')));

    IF @LocationName = ''
    BEGIN
        RAISERROR('Location name should not be empty', 16, 1);
        RETURN;
    END

    IF ISNULL(@DisplayOrder, 0) <= 0
    BEGIN
        RAISERROR('Sales priority should be a positive number', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.LocationMaster WHERE LocationId = @LocationId AND IsActive = 1)
    BEGIN
        RAISERROR('Location not found', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.LocationMaster WHERE LocationName = @LocationName AND LocationId <> @LocationId)
    BEGIN
        RAISERROR('Location already exist', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM dbo.LocationMaster
        WHERE IsActive = 1
          AND DisplayOrder = @DisplayOrder
          AND LocationId <> @LocationId
    )
    BEGIN
        RAISERROR('Sales priority already exist', 16, 1);
        RETURN;
    END

    UPDATE dbo.LocationMaster
    SET LocationName = @LocationName,
        DisplayOrder = @DisplayOrder,
        AllowForSales = ISNULL(@AllowForSales, 1),
        UpdatedOn = GETDATE(),
        UpdatedBy = @UpdatedBy
    WHERE LocationId = @LocationId;
END
GO

IF OBJECT_ID('dbo.sp_Rack_Insert', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_Rack_Insert AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_Rack_Insert
    @LocationId INT,
    @RackCaption VARCHAR(100),
    @UpdatedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    SET @RackCaption = LTRIM(RTRIM(ISNULL(@RackCaption, '')));

    IF NOT EXISTS (SELECT 1 FROM dbo.LocationMaster WHERE LocationId = @LocationId AND IsActive = 1)
    BEGIN
        RAISERROR('Select valid location', 16, 1);
        RETURN;
    END

    IF @RackCaption = ''
    BEGIN
        RAISERROR('Rack caption should not be empty', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.RackMaster WHERE LocationId = @LocationId AND RackCaption = @RackCaption)
    BEGIN
        RAISERROR('Rack already exist for this location', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.RackMaster (LocationId, RackCaption, DisplayOrder, IsActive, CreatedOn, UpdatedBy)
    VALUES
    (
        @LocationId,
        @RackCaption,
        ISNULL((SELECT MAX(DisplayOrder) FROM dbo.RackMaster WHERE LocationId = @LocationId), 0) + 1,
        1,
        GETDATE(),
        @UpdatedBy
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS RackId;
END
GO

IF OBJECT_ID('dbo.sp_Rack_ListByLocation', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_Rack_ListByLocation AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_Rack_ListByLocation
    @LocationId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        rm.RackId,
        rm.LocationId,
        rm.RackCaption,
        rm.DisplayOrder,
        rm.IsActive,
        rm.CreatedOn,
        rm.UpdatedOn,
        rm.UpdatedBy,
        ProductCount = (
            SELECT COUNT(*)
            FROM dbo.ProductRackMapping prm
            WHERE prm.RackId = rm.RackId
        ),
        CAST(
            CASE
                WHEN EXISTS (
                    SELECT 1
                    FROM dbo.ProductRackMapping prm
                    WHERE prm.RackId = rm.RackId
                ) THEN 1
                ELSE 0
            END AS BIT
        ) AS IsAssigned
    FROM dbo.RackMaster rm
    WHERE rm.LocationId = @LocationId
      AND rm.IsActive = 1
    ORDER BY ISNULL(rm.DisplayOrder, 2147483647), rm.RackId;
END
GO

IF OBJECT_ID('dbo.sp_Rack_ProductList', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_Rack_ProductList AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_Rack_ProductList
    @RackId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pm.id AS ProductId,
        pm.ItemName AS ProductName
    FROM dbo.ProductRackMapping prm
    INNER JOIN dbo.ProductMaster pm
        ON pm.id = prm.ProductId
    WHERE prm.RackId = @RackId
    ORDER BY pm.ItemName;
END
GO

IF OBJECT_ID('dbo.sp_Rack_Update', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_Rack_Update AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_Rack_Update
    @RackId INT,
    @RackCaption VARCHAR(100),
    @UpdatedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LocationId INT;
    SET @RackCaption = LTRIM(RTRIM(ISNULL(@RackCaption, '')));

    SELECT @LocationId = LocationId
    FROM dbo.RackMaster
    WHERE RackId = @RackId
      AND IsActive = 1;

    IF @LocationId IS NULL
    BEGIN
        RAISERROR('Rack not found', 16, 1);
        RETURN;
    END

    IF @RackCaption = ''
    BEGIN
        RAISERROR('Rack caption should not be empty', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM dbo.RackMaster
        WHERE LocationId = @LocationId
          AND RackCaption = @RackCaption
          AND RackId <> @RackId
    )
    BEGIN
        RAISERROR('Rack already exist for this location', 16, 1);
        RETURN;
    END

    UPDATE dbo.RackMaster
    SET RackCaption = @RackCaption,
        UpdatedOn = GETDATE(),
        UpdatedBy = @UpdatedBy
    WHERE RackId = @RackId;
END
GO
