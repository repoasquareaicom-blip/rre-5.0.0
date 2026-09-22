/*
  Simple rack-to-rack Material Movement save path.
  One input row represents one product movement from one rack to one rack.
  Legacy procedures and dbo.MaterialTranscation TVP are intentionally untouched.
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

IF TYPE_ID('dbo.MaterialMovementSimpleRackDetailType') IS NULL
BEGIN
    CREATE TYPE dbo.MaterialMovementSimpleRackDetailType AS TABLE
    (
        [LineNo] INT NOT NULL,
        ProductId INT NOT NULL,
        FromRackId INT NOT NULL,
        ToRackId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL
    );
END;
GO

IF OBJECT_ID('dbo.SaveMaterialMovement_RackWise', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.SaveMaterialMovement_RackWise AS BEGIN SET NOCOUNT ON; END');
END;
GO

ALTER PROCEDURE dbo.SaveMaterialMovement_RackWise
(
    @MoveBy VARCHAR(20),
    @Estid VARCHAR(100) = NULL,
    @RackDetails dbo.MaterialMovementSimpleRackDetailType READONLY,
    @Message VARCHAR(500) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Message = '';

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM @RackDetails)
        BEGIN
            RAISERROR('Please enter at least one rack movement.', 16, 1);
        END;

        IF EXISTS (SELECT 1 FROM @RackDetails WHERE Quantity <= 0)
        BEGIN
            RAISERROR('Rack movement quantity must be greater than zero.', 16, 1);
        END;

        IF EXISTS (SELECT 1 FROM @RackDetails WHERE FromRackId = ToRackId)
        BEGIN
            RAISERROR('Source and destination rack cannot be the same.', 16, 1);
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
            RAISERROR('Invalid product in rack movement.', 16, 1);
        END;

        IF EXISTS
        (
            SELECT 1
            FROM @RackDetails rd
            LEFT JOIN dbo.RackMaster fromRack
                ON fromRack.RackId = rd.FromRackId
            LEFT JOIN dbo.LocationMaster fromLocation
                ON fromLocation.LocationId = fromRack.LocationId
            LEFT JOIN dbo.ProductRackMapping fromMap
                ON fromMap.ProductId = rd.ProductId
               AND fromMap.RackId = rd.FromRackId
            WHERE fromRack.RackId IS NULL
               OR ISNULL(fromRack.IsActive, 0) <> 1
               OR fromLocation.LocationId IS NULL
               OR ISNULL(fromLocation.IsActive, 0) <> 1
               OR fromMap.ProductRackMappingId IS NULL
        )
        BEGIN
            RAISERROR('Invalid or inactive source rack mapping in material movement.', 16, 1);
        END;

        IF EXISTS
        (
            SELECT 1
            FROM @RackDetails rd
            LEFT JOIN dbo.RackMaster toRack
                ON toRack.RackId = rd.ToRackId
            LEFT JOIN dbo.LocationMaster toLocation
                ON toLocation.LocationId = toRack.LocationId
            LEFT JOIN dbo.ProductRackMapping toMap
                ON toMap.ProductId = rd.ProductId
               AND toMap.RackId = rd.ToRackId
            WHERE toRack.RackId IS NULL
               OR ISNULL(toRack.IsActive, 0) <> 1
               OR toLocation.LocationId IS NULL
               OR ISNULL(toLocation.IsActive, 0) <> 1
               OR toMap.ProductRackMappingId IS NULL
        )
        BEGIN
            RAISERROR('Invalid or inactive destination rack mapping in material movement.', 16, 1);
        END;

        BEGIN TRANSACTION;

        DECLARE @RequestedOut TABLE
        (
            ProductId INT NOT NULL,
            RackId INT NOT NULL,
            RequestedQuantity DECIMAL(18,3) NOT NULL
        );

        INSERT INTO @RequestedOut (ProductId, RackId, RequestedQuantity)
        SELECT ProductId, FromRackId, SUM(Quantity)
        FROM @RackDetails
        GROUP BY ProductId, FromRackId;

        DECLARE @Available TABLE
        (
            ProductId INT NOT NULL,
            RackId INT NOT NULL,
            RequestedQuantity DECIMAL(18,3) NOT NULL,
            AvailableQuantity DECIMAL(18,3) NOT NULL
        );

        INSERT INTO @Available (ProductId, RackId, RequestedQuantity, AvailableQuantity)
        SELECT
            ro.ProductId,
            ro.RackId,
            ro.RequestedQuantity,
            CAST(ISNULL(SUM(CASE
                WHEN UPPER(ISNULL(mt.Type, '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
                WHEN UPPER(ISNULL(mt.Type, '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
                ELSE 0
            END), 0) AS DECIMAL(18,3)) AS AvailableQuantity
        FROM @RequestedOut ro
        LEFT JOIN dbo.MaterialTranscation mt WITH (UPDLOCK, HOLDLOCK)
            ON mt.MaterailId = ro.ProductId
           AND mt.RackId = ro.RackId
        GROUP BY ro.ProductId, ro.RackId, ro.RequestedQuantity;

        IF EXISTS
        (
            SELECT 1
            FROM @Available
            WHERE RequestedQuantity > AvailableQuantity
        )
        BEGIN
            RAISERROR('Insufficient stock in one or more source racks. Please refresh and try again.', 16, 1);
        END;

        DECLARE @MainId INT;

        INSERT INTO dbo.MaterialTranscationMain
        VALUES (@MoveBy, GETDATE());

        SET @MainId = SCOPE_IDENTITY();

        DECLARE @LineMap TABLE
        (
            [LineNo] INT NOT NULL,
            ProductId INT NOT NULL,
            TransId INT NOT NULL
        );

        DECLARE
            @LineNo INT,
            @ProductId INT,
            @FromRackId INT,
            @ToRackId INT,
            @Quantity DECIMAL(18,3),
            @FromLocationId INT,
            @ToLocationId INT,
            @Stock VARCHAR(20),
            @TransId INT;

        DECLARE line_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT [LineNo], ProductId, FromRackId, ToRackId, Quantity
            FROM @RackDetails
            ORDER BY [LineNo];

        OPEN line_cursor;
        FETCH NEXT FROM line_cursor INTO @LineNo, @ProductId, @FromRackId, @ToRackId, @Quantity;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SELECT @FromLocationId = LocationId
            FROM dbo.RackMaster
            WHERE RackId = @FromRackId;

            SELECT @ToLocationId = LocationId
            FROM dbo.RackMaster
            WHERE RackId = @ToRackId;

            SET @Stock = CONVERT(VARCHAR(20), CAST(@Quantity AS DECIMAL(18,3)));

            INSERT INTO dbo.MaterialTranscationDetails
            VALUES (@ProductId, @Quantity, @FromLocationId, @ToLocationId, @MoveBy, GETDATE(), @MainId, @Stock, 1);

            SET @TransId = SCOPE_IDENTITY();

            INSERT INTO @LineMap ([LineNo], ProductId, TransId)
            VALUES (@LineNo, @ProductId, @TransId);

            FETCH NEXT FROM line_cursor INTO @LineNo, @ProductId, @FromRackId, @ToRackId, @Quantity;
        END;

        CLOSE line_cursor;
        DEALLOCATE line_cursor;

        IF ISNULL(@Estid, '') <> ''
        BEGIN
            UPDATE dbo.QuotationEstimation
            SET ismove = 1
            WHERE Estimationid = @Estid;
        END;

        INSERT INTO dbo.MaterialTranscation
            (TransId, TranscationType, TranscationDate, MaterailId, Quantity, LocationId, Type, Updatedby, RackId)
        SELECT
            lm.TransId,
            'Product Movement',
            GETDATE(),
            rd.ProductId,
            rd.Quantity,
            fromRack.LocationId,
            'OUT',
            CASE WHEN ISNUMERIC(@MoveBy) = 1 THEN CONVERT(INT, @MoveBy) ELSE NULL END,
            rd.FromRackId
        FROM @RackDetails rd
        INNER JOIN @LineMap lm
            ON lm.[LineNo] = rd.[LineNo]
           AND lm.ProductId = rd.ProductId
        INNER JOIN dbo.RackMaster fromRack
            ON fromRack.RackId = rd.FromRackId;

        INSERT INTO dbo.MaterialTranscation
            (TransId, TranscationType, TranscationDate, MaterailId, Quantity, LocationId, Type, Updatedby, RackId)
        SELECT
            lm.TransId,
            'Product Movement',
            GETDATE(),
            rd.ProductId,
            rd.Quantity,
            toRack.LocationId,
            'IN',
            CASE WHEN ISNUMERIC(@MoveBy) = 1 THEN CONVERT(INT, @MoveBy) ELSE NULL END,
            rd.ToRackId
        FROM @RackDetails rd
        INNER JOIN @LineMap lm
            ON lm.[LineNo] = rd.[LineNo]
           AND lm.ProductId = rd.ProductId
        INNER JOIN dbo.RackMaster toRack
            ON toRack.RackId = rd.ToRackId;

        COMMIT TRANSACTION;
        SET @Message = 'Successfully Product Moved';
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local', 'line_cursor') >= -1
        BEGIN
            CLOSE line_cursor;
            DEALLOCATE line_cursor;
        END;

        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        SET @Message = ERROR_MESSAGE();
        RAISERROR(@Message, 16, 1);
    END CATCH;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.DbDeploymentHistory
    WHERE ScriptName = '2026_09_18_material_movement_simple_rackwise.sql'
)
BEGIN
    INSERT INTO dbo.DbDeploymentHistory (ScriptName)
    VALUES ('2026_09_18_material_movement_simple_rackwise.sql');
END;
GO
