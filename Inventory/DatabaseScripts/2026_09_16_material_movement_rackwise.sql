/*
  Rack-aware Material Movement save path.
  Keeps legacy Material Movement procedures untouched for old EXE compatibility.
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

IF TYPE_ID('dbo.MaterialMovementRackDetailType') IS NULL
BEGIN
    CREATE TYPE dbo.MaterialMovementRackDetailType AS TABLE
    (
        LineNo INT NOT NULL,
        ProductId INT NOT NULL,
        RackId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL,
        MovementType VARCHAR(3) NOT NULL
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
    @RackDetails dbo.MaterialMovementRackDetailType READONLY,
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
            RAISERROR('Please allocate racks before saving.', 16, 1);
        END;

        IF EXISTS
        (
            SELECT 1
            FROM @RackDetails
            WHERE Quantity <= 0
               OR MovementType NOT IN ('OUT', 'IN')
        )
        BEGIN
            RAISERROR('Rack movement quantity must be greater than zero.', 16, 1);
        END;

        IF EXISTS
        (
            SELECT rd.ProductId
            FROM @RackDetails rd
            LEFT JOIN dbo.ProductMaster pm
                ON pm.id = rd.ProductId
            WHERE pm.id IS NULL
            GROUP BY rd.ProductId
        )
        BEGIN
            RAISERROR('Invalid product in rack movement.', 16, 1);
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
            RAISERROR('Invalid or inactive rack mapping in material movement.', 16, 1);
        END;

        IF EXISTS
        (
            SELECT 1
            FROM @RackDetails src
            INNER JOIN @RackDetails dst
                ON dst.LineNo = src.LineNo
               AND dst.ProductId = src.ProductId
               AND dst.RackId = src.RackId
            WHERE src.MovementType = 'OUT'
              AND dst.MovementType = 'IN'
        )
        BEGIN
            RAISERROR('Source and destination rack cannot be the same.', 16, 1);
        END;

        IF EXISTS
        (
            SELECT LineNo, ProductId
            FROM @RackDetails
            GROUP BY LineNo, ProductId
            HAVING SUM(CASE WHEN MovementType = 'OUT' THEN Quantity ELSE 0 END) <= 0
                OR SUM(CASE WHEN MovementType = 'IN' THEN Quantity ELSE 0 END) <= 0
                OR SUM(CASE WHEN MovementType = 'OUT' THEN Quantity ELSE 0 END)
                   <> SUM(CASE WHEN MovementType = 'IN' THEN Quantity ELSE 0 END)
        )
        BEGIN
            RAISERROR('Source and destination totals must be equal.', 16, 1);
        END;

        BEGIN TRANSACTION;

        ;WITH RequestedOut AS
        (
            SELECT ProductId, RackId, SUM(Quantity) AS RequestedQuantity
            FROM @RackDetails
            WHERE MovementType = 'OUT'
            GROUP BY ProductId, RackId
        ),
        Available AS
        (
            SELECT
                ro.ProductId,
                ro.RackId,
                ro.RequestedQuantity,
                CAST(ISNULL(SUM(CASE
                    WHEN UPPER(ISNULL(mt.Type, '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
                    WHEN UPPER(ISNULL(mt.Type, '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
                    ELSE 0
                END), 0) AS DECIMAL(18,3)) AS AvailableQuantity
            FROM RequestedOut ro
            LEFT JOIN dbo.MaterialTranscation mt WITH (UPDLOCK, HOLDLOCK)
                ON mt.MaterailId = ro.ProductId
               AND mt.RackId = ro.RackId
            GROUP BY ro.ProductId, ro.RackId, ro.RequestedQuantity
        )
        IF EXISTS
        (
            SELECT 1
            FROM Available
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
            LineNo INT NOT NULL,
            ProductId INT NOT NULL,
            TransId INT NOT NULL
        );

        DECLARE @Lines TABLE
        (
            LineNo INT NOT NULL,
            ProductId INT NOT NULL,
            Quantity DECIMAL(18,3) NOT NULL,
            FromLocation INT NOT NULL,
            ToLocation INT NOT NULL,
            Stock VARCHAR(20) NOT NULL
        );

        INSERT INTO @Lines (LineNo, ProductId, Quantity, FromLocation, ToLocation, Stock)
        SELECT
            rd.LineNo,
            rd.ProductId,
            SUM(CASE WHEN rd.MovementType = 'OUT' THEN rd.Quantity ELSE 0 END) AS Quantity,
            MIN(CASE WHEN rd.MovementType = 'OUT' THEN rm.LocationId END) AS FromLocation,
            MIN(CASE WHEN rd.MovementType = 'IN' THEN rm.LocationId END) AS ToLocation,
            CONVERT(VARCHAR(20), CAST(SUM(CASE
                WHEN rd.MovementType = 'OUT' THEN rd.Quantity
                ELSE 0
            END) AS DECIMAL(18,3))) AS Stock
        FROM @RackDetails rd
        INNER JOIN dbo.RackMaster rm
            ON rm.RackId = rd.RackId
        GROUP BY rd.LineNo, rd.ProductId;

        DECLARE
            @LineNo INT,
            @ProductId INT,
            @Quantity DECIMAL(18,3),
            @FromLocation INT,
            @ToLocation INT,
            @Stock VARCHAR(20),
            @TransId INT;

        DECLARE line_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT LineNo, ProductId, Quantity, FromLocation, ToLocation, Stock
            FROM @Lines
            ORDER BY LineNo;

        OPEN line_cursor;
        FETCH NEXT FROM line_cursor INTO @LineNo, @ProductId, @Quantity, @FromLocation, @ToLocation, @Stock;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            INSERT INTO dbo.MaterialTranscationDetails
            VALUES (@ProductId, @Quantity, @FromLocation, @ToLocation, @MoveBy, GETDATE(), @MainId, @Stock, 1);

            SET @TransId = SCOPE_IDENTITY();

            INSERT INTO @LineMap (LineNo, ProductId, TransId)
            VALUES (@LineNo, @ProductId, @TransId);

            FETCH NEXT FROM line_cursor INTO @LineNo, @ProductId, @Quantity, @FromLocation, @ToLocation, @Stock;
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
            rm.LocationId,
            rd.MovementType,
            CASE WHEN ISNUMERIC(@MoveBy) = 1 THEN CONVERT(INT, @MoveBy) ELSE NULL END,
            rd.RackId
        FROM @RackDetails rd
        INNER JOIN @LineMap lm
            ON lm.LineNo = rd.LineNo
           AND lm.ProductId = rd.ProductId
        INNER JOIN dbo.RackMaster rm
            ON rm.RackId = rd.RackId;

        COMMIT TRANSACTION;
        SET @Message = 'Successfully Product Moved';
    END TRY
    BEGIN CATCH
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
    WHERE ScriptName = '2026_09_16_material_movement_rackwise.sql'
)
BEGIN
    INSERT INTO dbo.DbDeploymentHistory (ScriptName)
    VALUES ('2026_09_16_material_movement_rackwise.sql');
END;
GO
