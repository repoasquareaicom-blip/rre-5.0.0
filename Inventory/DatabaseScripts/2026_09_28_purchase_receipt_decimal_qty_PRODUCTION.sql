/*
  Production deployment: Purchase Receipt decimal quantity.

  Applies only:
    1. dbo.PurchaseReceiptDetailstype
         OrderQuantity     decimal(18,3)
         RecievedQuantity  decimal(18,3)
         Productid         int
    2. dbo.PurchaseOrderDetails.Quantity decimal(18,3)
       Existing values are preserved. Scale 2 widens to scale 3.
    3. dbo.SavePurchaseReceipt
    4. dbo.SavePurchaseReceipt_direct
       These are the procedure definitions tested on RRE_NEW.

  Does not recreate dbo.PurchaseReceiptRackDetailType.
  Does not alter PurchaseReceiptDetails or MaterialTranscation.
  Does not insert, update, or delete transaction rows.

  Run in the production application database, not in master.
  Take a backup first. Run during a maintenance window.
  If validation fails, no change is committed.

  sqlcmd -S <server> -d <production-database> -E -b -i 2026_09_28_purchase_receipt_decimal_qty_PRODUCTION.sql
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET LOCK_TIMEOUT 15000;

PRINT '=== PRE-DEPLOYMENT TYPES ===';

SELECT
    DB_NAME() AS DatabaseName,
    t.name AS TypeName,
    c.column_id AS ColumnOrder,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.max_length AS MaxLength,
    c.precision AS PrecisionValue,
    c.scale AS ScaleValue,
    c.is_nullable AS IsNullable
FROM sys.table_types t
INNER JOIN sys.columns c
    ON c.object_id = t.type_table_object_id
INNER JOIN sys.types ty
    ON ty.user_type_id = c.user_type_id
WHERE t.name IN ('PurchaseReceiptDetailstype', 'PurchaseReceiptRackDetailType')
ORDER BY t.name, c.column_id;

SELECT
    'PurchaseOrderDetails' AS TableName,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.precision AS PrecisionValue,
    c.scale AS ScaleValue,
    c.is_nullable AS IsNullable
FROM sys.columns c
INNER JOIN sys.types ty
    ON ty.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.PurchaseOrderDetails')
  AND c.name = 'Quantity'
UNION ALL
SELECT
    'PurchaseReceiptDetails',
    c.name,
    ty.name,
    c.precision,
    c.scale,
    c.is_nullable
FROM sys.columns c
INNER JOIN sys.types ty
    ON ty.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.PurchaseReceiptDetails')
  AND c.name IN ('OrderQuantity', 'ReceivedQuantity')
UNION ALL
SELECT
    'MaterialTranscation',
    c.name,
    ty.name,
    c.precision,
    c.scale,
    c.is_nullable
FROM sys.columns c
INNER JOIN sys.types ty
    ON ty.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.MaterialTranscation')
  AND c.name = 'Quantity';

SELECT
    OBJECT_SCHEMA_NAME(d.referencing_id) AS DependentSchema,
    OBJECT_NAME(d.referencing_id) AS DependentObject
FROM sys.sql_expression_dependencies d
WHERE d.referenced_id = TYPE_ID('dbo.PurchaseReceiptDetailstype')
ORDER BY DependentObject;

DECLARE @Error NVARCHAR(4000);

IF DB_NAME() IN ('master', 'model', 'msdb', 'tempdb')
    SET @Error = 'Refusing to run in a system database.';

IF @Error IS NULL AND TYPE_ID('dbo.PurchaseReceiptDetailstype') IS NULL
    SET @Error = 'dbo.PurchaseReceiptDetailstype was not found.';

IF @Error IS NULL AND TYPE_ID('dbo.PurchaseReceiptRackDetailType') IS NULL
    SET @Error = 'dbo.PurchaseReceiptRackDetailType was not found.';

IF @Error IS NULL AND OBJECT_ID('dbo.SavePurchaseReceipt', 'P') IS NULL
    SET @Error = 'dbo.SavePurchaseReceipt was not found.';

IF @Error IS NULL AND OBJECT_ID('dbo.SavePurchaseReceipt_direct', 'P') IS NULL
    SET @Error = 'dbo.SavePurchaseReceipt_direct was not found.';

IF @Error IS NULL AND OBJECT_ID('dbo.PurchaseOrderDetails', 'U') IS NULL
    SET @Error = 'dbo.PurchaseOrderDetails was not found.';

IF @Error IS NULL AND OBJECT_ID('dbo.PurchaseReceiptDetails', 'U') IS NULL
    SET @Error = 'dbo.PurchaseReceiptDetails was not found.';

IF @Error IS NULL AND OBJECT_ID('dbo.MaterialTranscation', 'U') IS NULL
    SET @Error = 'dbo.MaterialTranscation was not found.';

IF @Error IS NULL
BEGIN
    DECLARE @DetailShape NVARCHAR(200);
    DECLARE @Column1 NVARCHAR(80);
    DECLARE @Column2 NVARCHAR(80);
    DECLARE @Column3 NVARCHAR(80);

    SELECT @Column1 = c.name + ':' + ty.name + ':' + CONVERT(VARCHAR(10), c.precision) + ':' + CONVERT(VARCHAR(10), c.scale)
    FROM sys.table_types t
    INNER JOIN sys.columns c ON c.object_id = t.type_table_object_id AND c.column_id = 1
    INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
    WHERE t.name = 'PurchaseReceiptDetailstype';

    SELECT @Column2 = c.name + ':' + ty.name + ':' + CONVERT(VARCHAR(10), c.precision) + ':' + CONVERT(VARCHAR(10), c.scale)
    FROM sys.table_types t
    INNER JOIN sys.columns c ON c.object_id = t.type_table_object_id AND c.column_id = 2
    INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
    WHERE t.name = 'PurchaseReceiptDetailstype';

    SELECT @Column3 = c.name + ':' + ty.name + ':' + CONVERT(VARCHAR(10), c.precision) + ':' + CONVERT(VARCHAR(10), c.scale)
    FROM sys.table_types t
    INNER JOIN sys.columns c ON c.object_id = t.type_table_object_id AND c.column_id = 3
    INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
    WHERE t.name = 'PurchaseReceiptDetailstype';

    SET @DetailShape = @Column1 + '|' + @Column2 + '|' + @Column3;

    IF @DetailShape NOT IN (
        'OrderQuantity:nvarchar:0:0|RecievedQuantity:nvarchar:0:0|Productid:nvarchar:0:0',
        'OrderQuantity:decimal:18:3|RecievedQuantity:decimal:18:3|Productid:int:10:0'
    )
        SET @Error = 'Unexpected PurchaseReceiptDetailstype shape: ' + ISNULL(@DetailShape, '(none)');

    IF @Error IS NULL AND (
        SELECT COUNT(*)
        FROM sys.columns c
        WHERE c.object_id = (SELECT type_table_object_id FROM sys.table_types WHERE name = 'PurchaseReceiptDetailstype')
    ) <> 3
        SET @Error = 'PurchaseReceiptDetailstype does not have exactly 3 columns.';
END

DECLARE @OrderQuantityScale TINYINT;

SELECT @OrderQuantityScale = c.scale
FROM sys.columns c
INNER JOIN sys.types ty
    ON ty.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.PurchaseOrderDetails')
  AND c.name = 'Quantity'
  AND ty.name = 'decimal'
  AND c.precision = 18;

IF @Error IS NULL
BEGIN
    IF EXISTS (
        SELECT 1
        FROM sys.sql_expression_dependencies d
        WHERE d.referenced_entity_name = 'PurchaseReceiptDetailstype'
          AND OBJECT_NAME(d.referencing_id) NOT IN ('SavePurchaseReceipt', 'SavePurchaseReceipt_direct')
    )
        SET @Error = 'An unexpected object depends on PurchaseReceiptDetailstype. No change was made.';
END

DECLARE @AlreadyApplied BIT;
SET @AlreadyApplied = 0;

IF @Error IS NULL
   AND @DetailShape = 'OrderQuantity:decimal:18:3|RecievedQuantity:decimal:18:3|Productid:int:10:0'
   AND @OrderQuantityScale = 3
   AND EXISTS (
        SELECT 1
        FROM sys.parameters par
        INNER JOIN sys.types ty
            ON ty.user_type_id = par.user_type_id
        WHERE par.object_id = OBJECT_ID('dbo.SavePurchaseReceipt_direct')
          AND par.name = '@PurchaseDetails'
          AND ty.name = 'PurchaseReceiptDetailstype'
   )
   AND EXISTS (
        SELECT 1
        FROM sys.parameters par
        INNER JOIN sys.types ty
            ON ty.user_type_id = par.user_type_id
        WHERE par.object_id = OBJECT_ID('dbo.SavePurchaseReceipt_direct')
          AND par.name = '@RackDetails'
          AND ty.name = 'PurchaseReceiptRackDetailType'
   )
    SET @AlreadyApplied = 1;

IF @Error IS NULL
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM sys.columns c
        INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
        WHERE c.object_id = OBJECT_ID('dbo.PurchaseOrderDetails')
          AND c.name = 'Quantity'
          AND ty.name = 'decimal'
          AND c.precision = 18
          AND c.scale IN (2, 3)
    )
        SET @Error = 'PurchaseOrderDetails.Quantity is not decimal(18,2) or decimal(18,3). No change was made.';
END

IF @Error IS NULL
BEGIN
    IF (
        SELECT COUNT(*)
        FROM sys.columns c
        INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
        WHERE c.object_id = OBJECT_ID('dbo.PurchaseReceiptDetails')
          AND (
                (c.name = 'OrderQuantity' AND ty.name = 'decimal' AND c.precision = 18 AND c.scale = 3)
             OR (c.name = 'ReceivedQuantity' AND ty.name = 'decimal' AND c.precision = 18 AND c.scale = 3)
          )
    ) <> 2
        SET @Error = 'PurchaseReceiptDetails quantity columns are not already decimal(18,3). They were not altered.';
END

IF @Error IS NULL
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM sys.columns c
        INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
        WHERE c.object_id = OBJECT_ID('dbo.MaterialTranscation')
          AND c.name = 'Quantity'
          AND ty.name = 'decimal'
          AND c.precision = 18
          AND c.scale = 3
    )
        SET @Error = 'MaterialTranscation.Quantity is not already decimal(18,3). It was not altered.';
END

IF @Error IS NULL
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM sys.table_types t
        INNER JOIN sys.columns c ON c.object_id = t.type_table_object_id
        INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
        WHERE t.name = 'PurchaseReceiptRackDetailType'
          AND c.name = 'Quantity'
          AND ty.name = 'decimal'
          AND c.precision = 18
          AND c.scale = 3
    )
        SET @Error = 'PurchaseReceiptRackDetailType.Quantity is not already decimal(18,3). It was not recreated.';
END

IF @Error IS NOT NULL
BEGIN
    RAISERROR(@Error, 16, 1);
    RETURN;
END

IF @AlreadyApplied = 1
BEGIN
    PRINT 'Purchase receipt decimal quantity is already applied. No objects were changed.';
END
ELSE
BEGIN

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('dbo.SavePurchaseReceipt_direct', 'P') IS NOT NULL
        DROP PROCEDURE dbo.SavePurchaseReceipt_direct;

    IF OBJECT_ID('dbo.SavePurchaseReceipt', 'P') IS NOT NULL
        DROP PROCEDURE dbo.SavePurchaseReceipt;

    IF TYPE_ID('dbo.PurchaseReceiptDetailstype') IS NOT NULL
        DROP TYPE dbo.PurchaseReceiptDetailstype;

    CREATE TYPE dbo.PurchaseReceiptDetailstype AS TABLE
    (
        OrderQuantity DECIMAL(18,3) NULL,
        RecievedQuantity DECIMAL(18,3) NULL,
        Productid INT NULL
    );

    DECLARE @QuantityNullable BIT;
    DECLARE @QuantityScale TINYINT;

    SELECT
        @QuantityNullable = c.is_nullable,
        @QuantityScale = c.scale
    FROM sys.columns c
    WHERE c.object_id = OBJECT_ID('dbo.PurchaseOrderDetails')
      AND c.name = 'Quantity';

    IF @QuantityScale < 3
    BEGIN
        DECLARE @AlterSql NVARCHAR(400);
        SET @AlterSql =
            N'ALTER TABLE dbo.PurchaseOrderDetails ALTER COLUMN Quantity DECIMAL(18,3) '
            + CASE WHEN @QuantityNullable = 1 THEN N'NULL' ELSE N'NOT NULL' END;
        EXEC sp_executesql @AlterSql;
    END

    DECLARE @CreateSave NVARCHAR(MAX);
    DECLARE @CreateDirect NVARCHAR(MAX);

    SET @CreateSave = N'CREATE PROCEDURE dbo.SavePurchaseReceipt
(
    @isnew INT,
    @OrderNumber VARCHAR(100) = NULL,
    @OrderDate DATETIME = NULL,
    @VendorId INT = NULL,
    @Status VARCHAR(50) = NULL,
    @EnteredBy VARCHAR(50) = NULL,
    @PurchaseDetails dbo.PurchaseReceiptDetailstype READONLY,
    @Remarks VARCHAR(MAX) = NULL,
    @Partial VARCHAR(MAX) = NULL,
    @out INT OUTPUT,
    @result VARCHAR(50) OUTPUT
)
AS
BEGIN
    DECLARE @POmaxid INT, @POvalue VARCHAR(15), @PO0utid VARCHAR(100), @POId INT;
    DECLARE @PRmaxid INT, @PRvalue VARCHAR(15), @PR0utid VARCHAR(100), @PRId INT;
    DECLARE @i INT;

    IF (@isnew = 0)
    BEGIN
        SET @POmaxid = (SELECT COUNT(*) FROM PurchaseOrderHeader);
        SET @POvalue = @POmaxid + 1;
        SET @PO0utid = ''RR PO '' + @POvalue + ''/'' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
        SET @POId = (SELECT ISNULL(MAX(PurchaseId), 0) + 1 FROM PurchaseOrderHeader);
        INSERT INTO PurchaseOrderHeader (PurchaseId, OrderNumber, OrderDate, VendorId, Status, EnteredBy, EnteredOn, Remarks, IsDeleted, ApprovedBy, ApproverOn)
        VALUES (@POId, @PO0utid, @OrderDate, @VendorId, @Status, @EnteredBy, GETDATE(), @Remarks, 0, @EnteredBy, GETDATE());

        SELECT @i = (SELECT ISNULL(MAX(Id), 0) FROM PurchaseOrderDetails);
        INSERT INTO PurchaseOrderDetails (Id, PurchaseId, OrderNumber, Quantity, ProductId)
        SELECT ROW_NUMBER() OVER (ORDER BY @i) + @i AS Id, @POId, @PO0utid, RecievedQuantity, Productid
        FROM @PurchaseDetails;

        SET @PRmaxid = (SELECT COUNT(*) FROM PurchaseReceiptHeader);
        SET @PRvalue = @PRmaxid + 1;
        SET @PR0utid = ''RR PR '' + @PRvalue + ''/'' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
        SET @PRId = (SELECT ISNULL(MAX(PurchaseId), 0) + 1 FROM PurchaseReceiptHeader);

        INSERT INTO PurchaseReceiptHeader (PurchaseId, OrderNumber, OrderDate, VendorId, Status, EnteredBy, EnteredOn, Remarks, IsDeleted, PurchaseOrderId, IsEntryCompleted)
        VALUES (@PRId, @PR0utid, @OrderDate, @VendorId, @Status, @EnteredBy, GETDATE(), @Remarks, 0, @POId, 1);

        SELECT @i = (SELECT ISNULL(MAX(Id), 0) FROM PurchaseReceiptDetails);
        INSERT INTO PurchaseReceiptDetails (Id, PurchaseId, OrderNumber, OrderQuantity, ReceivedQuantity, ProductId)
        SELECT ROW_NUMBER() OVER (ORDER BY @i) + @i AS Id, @PRId, @PR0utid, RecievedQuantity, RecievedQuantity, Productid
        FROM @PurchaseDetails;
        UPDATE PurchaseOrderHeader SET IsReceipt = 1, Partialval = @Partial WHERE PurchaseId = @POId;
        SET @out = 1;
        SET @result = @PR0utid;
    END
    ELSE
    BEGIN
        SET @PRmaxid = (SELECT COUNT(*) FROM PurchaseReceiptHeader);
        SET @PRvalue = @PRmaxid + 1;
        SET @PR0utid = ''RR PR '' + @PRvalue + ''/'' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
        SET @PRId = (SELECT ISNULL(MAX(PurchaseId), 0) + 1 FROM PurchaseReceiptHeader);
        SET @POId = (SELECT PurchaseId FROM PurchaseOrderHeader WHERE OrderNumber = @OrderNumber);
        INSERT INTO PurchaseReceiptHeader (PurchaseId, OrderNumber, OrderDate, VendorId, Status, EnteredBy, EnteredOn, Remarks, IsDeleted, PurchaseOrderId, IsEntryCompleted)
        VALUES (@PRId, @PR0utid, @OrderDate, @VendorId, @Status, @EnteredBy, GETDATE(), @Remarks, 0, @POId, 1);

        SELECT @i = (SELECT ISNULL(MAX(Id), 0) FROM PurchaseReceiptDetails);
        INSERT INTO PurchaseReceiptDetails (Id, PurchaseId, OrderNumber, OrderQuantity, ReceivedQuantity, ProductId)
        SELECT ROW_NUMBER() OVER (ORDER BY @i) + @i AS Id, @PRId, @PR0utid, OrderQuantity, RecievedQuantity, Productid
        FROM @PurchaseDetails;
        UPDATE PurchaseOrderHeader SET IsReceipt = 1, Partialval = @Partial WHERE PurchaseId = @POId;
        SET @out = 2;
        SET @result = @PR0utid;
    END
END';
    SET @CreateDirect = N'CREATE PROCEDURE dbo.SavePurchaseReceipt_direct
(
    @isnew INT,
    @OrderNumber VARCHAR(100) = NULL,
    @OrderDate DATETIME = NULL,
    @VendorId INT = NULL,
    @Status VARCHAR(50) = NULL,
    @EnteredBy VARCHAR(50) = NULL,
    @PurchaseDetails dbo.PurchaseReceiptDetailstype READONLY,
    @Remarks VARCHAR(MAX) = NULL,
    @Partial VARCHAR(MAX) = NULL,
    @RackDetails dbo.PurchaseReceiptRackDetailType READONLY,
    @out INT OUTPUT,
    @result VARCHAR(50) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @POmaxid INT;
    DECLARE @POvalue VARCHAR(15);
    DECLARE @PO0utid VARCHAR(100);
    DECLARE @POId INT;
    DECLARE @PRmaxid INT;
    DECLARE @PRvalue VARCHAR(15);
    DECLARE @PR0utid VARCHAR(100);
    DECLARE @PRId INT;
    DECLARE @i INT;

    SET @out = 0;
    SET @result = '''';

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM @PurchaseDetails WHERE ISNULL(RecievedQuantity, 0) > 0)
        BEGIN
            RAISERROR(''Enter received quantity for at least one product.'', 16, 1);
        END

        IF EXISTS (SELECT 1 FROM @RackDetails WHERE Quantity <= 0)
        BEGIN
            RAISERROR(''Rack quantity should be greater than zero.'', 16, 1);
        END

        IF EXISTS (
            SELECT ProductId, RackId
            FROM @RackDetails
            GROUP BY ProductId, RackId
            HAVING COUNT(*) > 1
        )
        BEGIN
            RAISERROR(''Duplicate rack allocation found for product and rack.'', 16, 1);
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
        )
        BEGIN
            RAISERROR(''Invalid active rack allocation found.'', 16, 1);
        END

        IF EXISTS (
            SELECT 1
            FROM @RackDetails rd
            WHERE NOT EXISTS (
                SELECT 1
                FROM @PurchaseDetails pd
                WHERE pd.Productid = rd.ProductId
            )
        )
        BEGIN
            RAISERROR(''Rack allocation exists for product not in purchase details.'', 16, 1);
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
            RAISERROR(''Rack allocation is not assigned to product.'', 16, 1);
        END

        IF EXISTS (
            SELECT 1
            FROM @PurchaseDetails pd
            WHERE ISNULL(pd.RecievedQuantity, 0) > 0
              AND NOT EXISTS (
                  SELECT 1
                  FROM @RackDetails rd
                  WHERE rd.ProductId = pd.Productid
              )
        )
        BEGIN
            RAISERROR(''Rack allocation missing for received product.'', 16, 1);
        END

        IF EXISTS (
            SELECT 1
            FROM @RackDetails rd
            WHERE NOT EXISTS (
                SELECT 1
                FROM @PurchaseDetails pd
                WHERE pd.Productid = rd.ProductId
                  AND ISNULL(pd.RecievedQuantity, 0) > 0
            )
        )
        BEGIN
            RAISERROR(''Rack allocation should not exist for zero received quantity.'', 16, 1);
        END

        IF EXISTS (
            SELECT 1
            FROM @PurchaseDetails pd
            OUTER APPLY (
                SELECT SUM(rd.Quantity) AS RackQty
                FROM @RackDetails rd
                WHERE rd.ProductId = pd.Productid
            ) x
            WHERE ISNULL(pd.RecievedQuantity, 0) > 0
              AND ISNULL(x.RackQty, 0) <> pd.RecievedQuantity
        )
        BEGIN
            RAISERROR(''Rack allocation total must match received quantity.'', 16, 1);
        END

        IF @isnew <> 0
        BEGIN
            SELECT @POId = PurchaseId
            FROM dbo.PurchaseOrderHeader
            WHERE OrderNumber = @OrderNumber
              AND ISNULL(IsDeleted, 0) = 0;

            IF ISNULL(@POId, 0) = 0
            BEGIN
                RAISERROR(''Purchase order not found.'', 16, 1);
            END

            IF EXISTS (
                SELECT 1
                FROM @PurchaseDetails pd
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM dbo.PurchaseOrderDetails pod
                    WHERE pod.PurchaseId = @POId
                      AND pod.Productid = pd.Productid
                )
            )
            BEGIN
                RAISERROR(''Purchase detail product is not part of selected purchase order.'', 16, 1);
            END

            IF EXISTS (
                SELECT 1
                FROM @PurchaseDetails pd
                INNER JOIN dbo.PurchaseOrderDetails pod
                    ON pod.PurchaseId = @POId
                   AND pod.Productid = pd.Productid
                OUTER APPLY (
                    SELECT SUM(prd.ReceivedQuantity) AS AlreadyReceivedQty
                    FROM dbo.PurchaseReceiptHeader prh
                    INNER JOIN dbo.PurchaseReceiptDetails prd
                        ON prd.PurchaseId = prh.PurchaseId
                    WHERE ISNULL(prh.IsDeleted, 0) = 0
                      AND prh.PurchaseOrderId = @POId
                      AND prd.ProductId = pd.Productid
                ) received
                WHERE ISNULL(pd.RecievedQuantity, 0) >
                      (ISNULL(pod.Quantity, 0) - ISNULL(received.AlreadyReceivedQty, 0))
            )
            BEGIN
                RAISERROR(''Received quantity cannot be greater than remaining purchase order quantity.'', 16, 1);
            END
        END

        BEGIN TRANSACTION;

        IF (@isnew = 0)
        BEGIN
            SET @POmaxid = (SELECT COUNT(*) FROM dbo.PurchaseOrderHeader);
            SET @POvalue = @POmaxid + 1;
            SET @PO0utid = ''RR PO '' + @POvalue + ''/'' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
            SET @POId = (SELECT ISNULL(MAX(PurchaseId), 0) + 1 FROM dbo.PurchaseOrderHeader);

            INSERT INTO dbo.PurchaseOrderHeader
            (
                PurchaseId,
                OrderNumber,
                OrderDate,
                VendorId,
                Status,
                EnteredBy,
                EnteredOn,
                Remarks,
                IsDeleted,
                ApprovedBy,
                ApproverOn
            )
            VALUES
            (
                @POId,
                @PO0utid,
                @OrderDate,
                @VendorId,
                ''Closed'',
                @EnteredBy,
                GETDATE(),
                @Remarks,
                0,
                @EnteredBy,
                GETDATE()
            );

            SELECT @i = (SELECT ISNULL(MAX(Id), 0) FROM dbo.PurchaseOrderDetails);

            INSERT INTO dbo.PurchaseOrderDetails
            (
                Id,
                PurchaseId,
                OrderNumber,
                Quantity,
                ProductId
            )
            SELECT
                ROW_NUMBER() OVER (ORDER BY Productid) + @i AS Id,
                @POId,
                @PO0utid,
                RecievedQuantity,
                Productid
            FROM @PurchaseDetails;

            SET @PRmaxid = (SELECT COUNT(*) FROM dbo.PurchaseReceiptHeader);
            SET @PRvalue = @PRmaxid + 1;
            SET @PR0utid = ''RR PR '' + @PRvalue + ''/'' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
            SET @PRId = (SELECT ISNULL(MAX(PurchaseId), 0) + 1 FROM dbo.PurchaseReceiptHeader);

            INSERT INTO dbo.PurchaseReceiptHeader
            (
                PurchaseId,
                OrderNumber,
                OrderDate,
                VendorId,
                Status,
                EnteredBy,
                EnteredOn,
                Remarks,
                IsDeleted,
                PurchaseOrderId,
                IsEntryCompleted,
                CheckedBy,
                CheckedOn,
                IsCheckingCompleted,
                BarcodeBy,
                BarcodeOn,
                IsBarcodedCompleted,
                FloorCheckinBy,
                FloorCheckinOn,
                IsFloorCheckedCompleted,
                DamageBy,
                DamageOn,
                IsDamagedCompleted,
                InvoiceBy,
                InvoiceOn,
                IsInvoiceCompleted
            )
            VALUES
            (
                @PRId,
                @PR0utid,
                @OrderDate,
                @VendorId,
                @Status,
                @EnteredBy,
                GETDATE(),
                @Remarks,
                0,
                @POId,
                1,
                @EnteredBy,
                GETDATE(),
                1,
                @EnteredBy,
                GETDATE(),
                1,
                @EnteredBy,
                GETDATE(),
                1,
                @EnteredBy,
                GETDATE(),
                1,
                @EnteredBy,
                GETDATE(),
                1
            );

            SELECT @i = (SELECT ISNULL(MAX(Id), 0) FROM dbo.PurchaseReceiptDetails);

            INSERT INTO dbo.PurchaseReceiptDetails
            (
                Id,
                PurchaseId,
                OrderNumber,
                OrderQuantity,
                ReceivedQuantity,
                ProductId
            )
            SELECT
                ROW_NUMBER() OVER (ORDER BY Productid) + @i AS Id,
                @PRId,
                @PR0utid,
                RecievedQuantity,
                RecievedQuantity,
                Productid
            FROM @PurchaseDetails;

            UPDATE dbo.PurchaseOrderHeader
            SET IsReceipt = 1,
                Partialval = @Partial
            WHERE PurchaseId = @POId;

            SET @out = 1;
            SET @result = @PR0utid;
        END
        ELSE
        BEGIN
            SET @PRmaxid = (SELECT COUNT(*) FROM dbo.PurchaseReceiptHeader);
            SET @PRvalue = @PRmaxid + 1;
            SET @PR0utid = ''RR PR '' + @PRvalue + ''/'' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
            SET @PRId = (SELECT ISNULL(MAX(PurchaseId), 0) + 1 FROM dbo.PurchaseReceiptHeader);
            SET @POId = (SELECT PurchaseId FROM dbo.PurchaseOrderHeader WHERE OrderNumber = @OrderNumber);

            INSERT INTO dbo.PurchaseReceiptHeader
            (
                PurchaseId,
                OrderNumber,
                OrderDate,
                VendorId,
                Status,
                EnteredBy,
                EnteredOn,
                Remarks,
                IsDeleted,
                PurchaseOrderId,
                IsEntryCompleted,
                CheckedBy,
                CheckedOn,
                IsCheckingCompleted,
                BarcodeBy,
                BarcodeOn,
                IsBarcodedCompleted,
                FloorCheckinBy,
                FloorCheckinOn,
                IsFloorCheckedCompleted,
                DamageBy,
                DamageOn,
                IsDamagedCompleted,
                InvoiceBy,
                InvoiceOn,
                IsInvoiceCompleted
            )
            VALUES
            (
                @PRId,
                @PR0utid,
                @OrderDate,
                @VendorId,
                @Status,
                @EnteredBy,
                GETDATE(),
                @Remarks,
                0,
                @POId,
                1,
                @EnteredBy,
                GETDATE(),
                1,
                @EnteredBy,
                GETDATE(),
                1,
                @EnteredBy,
                GETDATE(),
                1,
                @EnteredBy,
                GETDATE(),
                1,
                @EnteredBy,
                GETDATE(),
                1
            );

            SELECT @i = (SELECT ISNULL(MAX(Id), 0) FROM dbo.PurchaseReceiptDetails);

            INSERT INTO dbo.PurchaseReceiptDetails
            (
                Id,
                PurchaseId,
                OrderNumber,
                OrderQuantity,
                ReceivedQuantity,
                ProductId
            )
            SELECT
                ROW_NUMBER() OVER (ORDER BY Productid) + @i AS Id,
                @PRId,
                @PR0utid,
                OrderQuantity,
                RecievedQuantity,
                Productid
            FROM @PurchaseDetails;

            UPDATE dbo.PurchaseOrderHeader
            SET IsReceipt = 1,
                Partialval = @Partial,
                Status = ''Closed''
            WHERE PurchaseId = @POId;

            SET @out = 2;
            SET @result = @PR0utid;
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
            Type
        )
        SELECT
            @PR0utid,
            ''Purchase'',
            GETDATE(),
            rd.ProductId,
            rd.Quantity,
            rm.LocationId,
            rd.RackId,
            ''IN''
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
END';

    EXEC sp_executesql @CreateSave;
    EXEC sp_executesql @CreateDirect;

    IF NOT EXISTS (
        SELECT 1
        FROM sys.table_types t
        INNER JOIN sys.columns c ON c.object_id = t.type_table_object_id
        INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
        WHERE t.name = 'PurchaseReceiptDetailstype'
          AND c.column_id = 1
          AND c.name = 'OrderQuantity'
          AND ty.name = 'decimal'
          AND c.precision = 18
          AND c.scale = 3
    )
    OR NOT EXISTS (
        SELECT 1
        FROM sys.table_types t
        INNER JOIN sys.columns c ON c.object_id = t.type_table_object_id
        INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
        WHERE t.name = 'PurchaseReceiptDetailstype'
          AND c.column_id = 2
          AND c.name = 'RecievedQuantity'
          AND ty.name = 'decimal'
          AND c.precision = 18
          AND c.scale = 3
    )
    OR NOT EXISTS (
        SELECT 1
        FROM sys.table_types t
        INNER JOIN sys.columns c ON c.object_id = t.type_table_object_id
        INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
        WHERE t.name = 'PurchaseReceiptDetailstype'
          AND c.column_id = 3
          AND c.name = 'Productid'
          AND ty.name = 'int'
    )
    OR NOT EXISTS (
        SELECT 1
        FROM sys.columns c
        INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
        WHERE c.object_id = OBJECT_ID('dbo.PurchaseOrderDetails')
          AND c.name = 'Quantity'
          AND ty.name = 'decimal'
          AND c.precision = 18
          AND c.scale = 3
    )
    OR OBJECT_ID('dbo.SavePurchaseReceipt', 'P') IS NULL
    OR OBJECT_ID('dbo.SavePurchaseReceipt_direct', 'P') IS NULL
    BEGIN
        RAISERROR('Post-change verification failed inside the transaction. All changes were rolled back.', 16, 1);
    END

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @Fail NVARCHAR(4000);
    SET @Fail = ERROR_MESSAGE();
    RAISERROR(@Fail, 16, 1);
    RETURN;
END CATCH

END

PRINT '=== POST-DEPLOYMENT TYPES ===';

SELECT
    DB_NAME() AS DatabaseName,
    t.name AS TypeName,
    c.column_id AS ColumnOrder,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.precision AS PrecisionValue,
    c.scale AS ScaleValue
FROM sys.table_types t
INNER JOIN sys.columns c
    ON c.object_id = t.type_table_object_id
INNER JOIN sys.types ty
    ON ty.user_type_id = c.user_type_id
WHERE t.name IN ('PurchaseReceiptDetailstype', 'PurchaseReceiptRackDetailType')
ORDER BY t.name, c.column_id;

SELECT
    'PurchaseOrderDetails' AS TableName,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.precision AS PrecisionValue,
    c.scale AS ScaleValue
FROM sys.columns c
INNER JOIN sys.types ty
    ON ty.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.PurchaseOrderDetails')
  AND c.name = 'Quantity'
UNION ALL
SELECT
    'PurchaseReceiptDetails',
    c.name,
    ty.name,
    c.precision,
    c.scale
FROM sys.columns c
INNER JOIN sys.types ty
    ON ty.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.PurchaseReceiptDetails')
  AND c.name IN ('OrderQuantity', 'ReceivedQuantity')
UNION ALL
SELECT
    'MaterialTranscation',
    c.name,
    ty.name,
    c.precision,
    c.scale
FROM sys.columns c
INNER JOIN sys.types ty
    ON ty.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.MaterialTranscation')
  AND c.name = 'Quantity';

SELECT
    OBJECT_NAME(object_id) AS ProcedureName,
    CASE
        WHEN OBJECT_NAME(object_id) = 'SavePurchaseReceipt_direct'
         AND definition LIKE '%PurchaseReceiptRackDetailType%'
         AND definition LIKE '%DECIMAL(18,3)%'
        THEN 'Present'
        WHEN OBJECT_NAME(object_id) = 'SavePurchaseReceipt'
         AND definition LIKE '%PurchaseReceiptDetailstype%'
        THEN 'Present'
        ELSE 'Unexpected definition'
    END AS DefinitionCheck
FROM sys.sql_modules
WHERE object_id IN (OBJECT_ID('dbo.SavePurchaseReceipt'), OBJECT_ID('dbo.SavePurchaseReceipt_direct'))
ORDER BY ProcedureName;
