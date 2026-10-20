-- Rack Wise Stock Upload (Tools -> Rack Wise Stock Upload)
-- The Excel is the FINAL PHYSICAL RACK STOCK. For every product present in the Excel,
-- every active rack allocated to it (ProductRackMapping + RackMaster + LocationMaster)
-- is reset: racks in the Excel to the Excel stock, allocated racks omitted from the
-- Excel to zero. Products not in the Excel are untouched.
-- The reset inserts MaterialTranscation adjustment rows only:
--     TranscationType = 'STOCK RESET', Type = 'IN' / 'OUT', Quantity = ABS(adjustment),
--     TransId = upload reference RSU-xxxxxx.
-- Existing MaterialTranscation rows are never updated or deleted.

SET NOCOUNT ON;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Menus WHERE MenuName = 'RackWiseStockUpload')
    INSERT INTO dbo.Menus (MenuName) VALUES ('RackWiseStockUpload');
GO

IF OBJECT_ID('dbo.RackWiseStockUpload', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RackWiseStockUpload
    (
        UploadId int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_RackWiseStockUpload PRIMARY KEY,
        UploadRef varchar(25) NULL,
        FileName nvarchar(260) NULL,
        UploadedOn datetime NOT NULL CONSTRAINT DF_RackWiseStockUpload_UploadedOn DEFAULT (GETDATE()),
        UploadedBy varchar(50) NULL,
        UploadedByUserId int NULL,
        TotalRows int NOT NULL,
        ProductCount int NOT NULL,
        Status varchar(20) NOT NULL
    );
END
GO

IF COL_LENGTH('dbo.RackWiseStockUpload', 'ZeroResetRacks') IS NULL
    ALTER TABLE dbo.RackWiseStockUpload ADD ZeroResetRacks int NULL;
IF COL_LENGTH('dbo.RackWiseStockUpload', 'AffectedRacks') IS NULL
    ALTER TABLE dbo.RackWiseStockUpload ADD AffectedRacks int NULL;
GO

IF OBJECT_ID('dbo.RackWiseStockUploadDetail', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RackWiseStockUploadDetail
    (
        UploadDetailId int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_RackWiseStockUploadDetail PRIMARY KEY,
        UploadId int NOT NULL CONSTRAINT FK_RackWiseStockUploadDetail_Upload REFERENCES dbo.RackWiseStockUpload (UploadId),
        RowNo int NULL,
        ProductId int NOT NULL,
        RackId int NOT NULL,
        RackCaption varchar(200) NULL,
        Result varchar(20) NOT NULL
    );
    CREATE INDEX IX_RackWiseStockUploadDetail_UploadId ON dbo.RackWiseStockUploadDetail (UploadId);
    CREATE INDEX IX_RackWiseStockUploadDetail_ProductRack ON dbo.RackWiseStockUploadDetail (ProductId, RackId);
END
GO

-- Upgrade of the earlier opening-stock audit shape: existing audit rows are kept,
-- the old columns become optional and the stock reset columns are added.
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.RackWiseStockUploadDetail') AND name = 'RowNo' AND is_nullable = 0)
    ALTER TABLE dbo.RackWiseStockUploadDetail ALTER COLUMN RowNo int NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.RackWiseStockUploadDetail') AND name = 'PreviousOpening' AND is_nullable = 0)
    ALTER TABLE dbo.RackWiseStockUploadDetail ALTER COLUMN PreviousOpening decimal(18, 3) NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.RackWiseStockUploadDetail') AND name = 'NewOpening' AND is_nullable = 0)
    ALTER TABLE dbo.RackWiseStockUploadDetail ALTER COLUMN NewOpening decimal(18, 3) NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.RackWiseStockUploadDetail') AND name = 'PreviousBalance' AND is_nullable = 0)
    ALTER TABLE dbo.RackWiseStockUploadDetail ALTER COLUMN PreviousBalance decimal(18, 3) NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.RackWiseStockUploadDetail') AND name = 'NewBalance' AND is_nullable = 0)
    ALTER TABLE dbo.RackWiseStockUploadDetail ALTER COLUMN NewBalance decimal(18, 3) NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.RackWiseStockUploadDetail') AND name = 'Action' AND is_nullable = 0)
    ALTER TABLE dbo.RackWiseStockUploadDetail ALTER COLUMN Action varchar(20) NULL;
GO

IF COL_LENGTH('dbo.RackWiseStockUploadDetail', 'LocationId') IS NULL
    ALTER TABLE dbo.RackWiseStockUploadDetail ADD LocationId int NULL;
IF COL_LENGTH('dbo.RackWiseStockUploadDetail', 'Source') IS NULL
    ALTER TABLE dbo.RackWiseStockUploadDetail ADD Source varchar(20) NULL;
IF COL_LENGTH('dbo.RackWiseStockUploadDetail', 'PreviousStock') IS NULL
    ALTER TABLE dbo.RackWiseStockUploadDetail ADD PreviousStock decimal(18, 3) NULL;
IF COL_LENGTH('dbo.RackWiseStockUploadDetail', 'TargetStock') IS NULL
    ALTER TABLE dbo.RackWiseStockUploadDetail ADD TargetStock decimal(18, 3) NULL;
IF COL_LENGTH('dbo.RackWiseStockUploadDetail', 'Adjustment') IS NULL
    ALTER TABLE dbo.RackWiseStockUploadDetail ADD Adjustment decimal(18, 3) NULL;
IF COL_LENGTH('dbo.RackWiseStockUploadDetail', 'MovementType') IS NULL
    ALTER TABLE dbo.RackWiseStockUploadDetail ADD MovementType varchar(10) NULL;
IF COL_LENGTH('dbo.RackWiseStockUploadDetail', 'ResultingStock') IS NULL
    ALTER TABLE dbo.RackWiseStockUploadDetail ADD ResultingStock decimal(18, 3) NULL;
GO

IF OBJECT_ID('dbo.Proc_RackWiseStockUpload_Apply', 'P') IS NOT NULL
    DROP PROCEDURE dbo.Proc_RackWiseStockUpload_Apply;
GO
IF OBJECT_ID('dbo.Proc_RackWiseStockUpload_Preview', 'P') IS NOT NULL
    DROP PROCEDURE dbo.Proc_RackWiseStockUpload_Preview;
GO
IF OBJECT_ID('dbo.Proc_RackWiseStockUpload_Validate', 'P') IS NOT NULL
    DROP PROCEDURE dbo.Proc_RackWiseStockUpload_Validate;
GO
IF TYPE_ID('dbo.RackWiseStockUploadRow') IS NOT NULL
    DROP TYPE dbo.RackWiseStockUploadRow;
GO

CREATE TYPE dbo.RackWiseStockUploadRow AS TABLE
(
    RowNo int NOT NULL PRIMARY KEY,
    ProductId int NULL,
    ProductName nvarchar(max) NULL,
    UOM nvarchar(400) NULL,
    RackName nvarchar(400) NULL,
    Stock decimal(18, 3) NULL
);
GO

-- Row-level validation of the Excel rows.
-- ProductId / Stock are NULL when the Excel value is blank or not a valid number.
CREATE PROCEDURE dbo.Proc_RackWiseStockUpload_Validate
    @Rows dbo.RackWiseStockUploadRow READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v TABLE
    (
        RowNo int NOT NULL PRIMARY KEY,
        ProductId int NULL,
        ProductFound bit NOT NULL,
        NameOk bit NOT NULL,
        UomOk bit NOT NULL,
        RackKey nvarchar(400) NOT NULL,
        RackMatches int NOT NULL,
        RackId int NULL,
        RackActive bit NOT NULL,
        Mapped bit NOT NULL,
        Stock decimal(18, 3) NULL,
        Remarks nvarchar(1000) NOT NULL
    );

    INSERT INTO @v (RowNo, ProductId, ProductFound, NameOk, UomOk, RackKey, RackMatches, RackId, RackActive, Mapped, Stock, Remarks)
    SELECT
        r.RowNo,
        r.ProductId,
        CASE WHEN p.id IS NULL THEN 0 ELSE 1 END,
        CASE WHEN p.id IS NOT NULL
                  AND LTRIM(RTRIM(ISNULL(r.ProductName, N''))) <> N''
                  AND (LTRIM(RTRIM(ISNULL(r.ProductName, N''))) = LTRIM(RTRIM(ISNULL(p.DisplayName, '')))
                       OR LTRIM(RTRIM(ISNULL(r.ProductName, N''))) = LTRIM(RTRIM(ISNULL(p.ItemName, ''))))
             THEN 1 ELSE 0 END,
        CASE WHEN u.UOM IS NOT NULL AND LTRIM(RTRIM(ISNULL(r.UOM, N''))) = LTRIM(RTRIM(u.UOM)) THEN 1 ELSE 0 END,
        UPPER(LTRIM(RTRIM(ISNULL(r.RackName, N'')))),
        0, NULL, 0, 0,
        r.Stock,
        N''
    FROM @Rows r
    LEFT JOIN dbo.ProductMaster p ON p.id = r.ProductId
    LEFT JOIN dbo.UOM u ON CONVERT(varchar(50), u.Uomid) = LTRIM(RTRIM(p.UOM));

    UPDATE v
    SET RackMatches = x.Matches,
        RackId = CASE WHEN x.Matches = 1 THEN x.RackId END
    FROM @v v
    CROSS APPLY (SELECT COUNT(*) AS Matches, MIN(rm.RackId) AS RackId
                 FROM dbo.RackMaster rm
                 WHERE v.RackKey <> N'' AND LTRIM(RTRIM(rm.RackCaption)) = v.RackKey) x;

    UPDATE v
    SET RackActive = CASE WHEN ISNULL(rm.IsActive, 0) = 1 AND ISNULL(lm.IsActive, 0) = 1 THEN 1 ELSE 0 END,
        Mapped = CASE WHEN EXISTS (SELECT 1 FROM dbo.ProductRackMapping prm WHERE prm.ProductId = v.ProductId AND prm.RackId = v.RackId) THEN 1 ELSE 0 END
    FROM @v v
    INNER JOIN dbo.RackMaster rm ON rm.RackId = v.RackId
    LEFT JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId;

    UPDATE @v SET Remarks = Remarks + N'Product ID not found. ' WHERE ProductFound = 0;
    UPDATE @v SET Remarks = Remarks + N'Product Name does not match Product Master. ' WHERE ProductFound = 1 AND NameOk = 0;
    UPDATE @v SET Remarks = Remarks + N'UOM does not match Product Master. ' WHERE ProductFound = 1 AND UomOk = 0;
    UPDATE @v SET Remarks = Remarks + N'Rack not found. ' WHERE RackMatches = 0;
    UPDATE @v SET Remarks = Remarks + N'Rack Name exists in more than one location. Location is required to identify the rack. ' WHERE RackMatches > 1;
    UPDATE @v SET Remarks = Remarks + N'Rack is inactive. ' WHERE RackMatches = 1 AND RackActive = 0;
    UPDATE @v SET Remarks = Remarks + N'Rack is not allocated to this product. ' WHERE RackMatches = 1 AND ProductFound = 1 AND Mapped = 0;
    UPDATE @v SET Remarks = Remarks + N'Invalid stock quantity. ' WHERE Stock IS NULL;
    UPDATE @v SET Remarks = Remarks + N'Stock cannot be negative. ' WHERE Stock < 0;
    UPDATE v SET Remarks = Remarks + N'Duplicate Product/Rack in Excel. '
    FROM @v v
    WHERE v.ProductId IS NOT NULL AND v.RackKey <> N''
      AND (SELECT COUNT(*) FROM @v d WHERE d.ProductId = v.ProductId AND d.RackKey = v.RackKey) > 1;

    SELECT
        v.RowNo,
        v.ProductId,
        v.RackId,
        rm.RackCaption,
        lm.LocationName,
        CAST(ISNULL(s.Stock, 0) AS decimal(18, 3)) AS CurrentStock,
        CASE WHEN v.Remarks = N'' THEN 'VALID' ELSE 'ERROR' END AS Result,
        RTRIM(v.Remarks) AS Remarks
    FROM @v v
    LEFT JOIN dbo.RackMaster rm ON rm.RackId = v.RackId
    LEFT JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId
    OUTER APPLY (SELECT SUM(CASE WHEN mt.Type = 'IN' THEN mt.Quantity WHEN mt.Type = 'OUT' THEN -mt.Quantity ELSE 0 END) AS Stock
                 FROM dbo.MaterialTranscation mt
                 WHERE v.RackId IS NOT NULL AND mt.MaterailId = v.ProductId AND mt.RackId = v.RackId) s
    ORDER BY v.RowNo;
END
GO

-- Complete reset plan: every active allocated rack of every product present in the Excel.
-- Rack in the Excel -> Target = Excel stock (Source EXCEL).
-- Allocated rack omitted from the Excel -> Target = 0 (Source RESET TO ZERO).
-- Call only after every row passed Proc_RackWiseStockUpload_Validate.
CREATE PROCEDURE dbo.Proc_RackWiseStockUpload_Preview
    @Rows dbo.RackWiseStockUploadRow READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @excel TABLE
    (
        RowNo int NOT NULL PRIMARY KEY,
        ProductId int NOT NULL,
        RackKey nvarchar(400) NOT NULL,
        Stock decimal(18, 3) NULL
    );
    INSERT INTO @excel (RowNo, ProductId, RackKey, Stock)
    SELECT RowNo, ProductId, UPPER(LTRIM(RTRIM(ISNULL(RackName, N'')))), Stock
    FROM @Rows
    WHERE ProductId IS NOT NULL;

    DECLARE @racks TABLE
    (
        ProductId int NOT NULL,
        RackId int NOT NULL,
        PRIMARY KEY (ProductId, RackId)
    );
    INSERT INTO @racks (ProductId, RackId)
    SELECT DISTINCT prm.ProductId, prm.RackId
    FROM dbo.ProductRackMapping prm
    INNER JOIN dbo.RackMaster rm ON rm.RackId = prm.RackId AND rm.IsActive = 1
    INNER JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId AND lm.IsActive = 1
    WHERE prm.ProductId IN (SELECT ProductId FROM @excel);

    SELECT
        e.RowNo,
        k.ProductId,
        p.ItemName AS ProductName,
        ISNULL(u.UOM, ISNULL(p.UOM, '')) AS UOM,
        k.RackId,
        rm.RackCaption,
        rm.LocationId,
        lm.LocationName,
        CASE WHEN e.RowNo IS NULL THEN 'RESET TO ZERO' ELSE 'EXCEL' END AS Source,
        CAST(ISNULL(s.Stock, 0) AS decimal(18, 3)) AS CurrentStock,
        CAST(ISNULL(e.Stock, 0) AS decimal(18, 3)) AS TargetStock,
        CAST(ISNULL(e.Stock, 0) - ISNULL(s.Stock, 0) AS decimal(18, 3)) AS Adjustment
    FROM @racks k
    INNER JOIN dbo.RackMaster rm ON rm.RackId = k.RackId
    INNER JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId
    INNER JOIN dbo.ProductMaster p ON p.id = k.ProductId
    LEFT JOIN dbo.UOM u ON CONVERT(varchar(50), u.Uomid) = LTRIM(RTRIM(p.UOM))
    LEFT JOIN @excel e ON e.ProductId = k.ProductId AND e.RackKey = UPPER(LTRIM(RTRIM(rm.RackCaption)))
    OUTER APPLY (SELECT MIN(f.RowNo) AS FirstRow FROM @excel f WHERE f.ProductId = k.ProductId) o
    OUTER APPLY (SELECT SUM(CASE WHEN mt.Type = 'IN' THEN mt.Quantity WHEN mt.Type = 'OUT' THEN -mt.Quantity ELSE 0 END) AS Stock
                 FROM dbo.MaterialTranscation mt
                 WHERE mt.MaterailId = k.ProductId AND mt.RackId = k.RackId) s
    ORDER BY o.FirstRow, CASE WHEN e.RowNo IS NULL THEN 1 ELSE 0 END, e.RowNo, rm.RackCaption;
END
GO

CREATE PROCEDURE dbo.Proc_RackWiseStockUpload_Apply
    @Rows dbo.RackWiseStockUploadRow READONLY,
    @FileName nvarchar(260),
    @UploadedBy varchar(50),
    @UserId int
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    CREATE TABLE #v
    (
        RowNo int NOT NULL PRIMARY KEY,
        ProductId int NULL,
        RackId int NULL,
        RackCaption varchar(200) NULL,
        LocationName varchar(200) NULL,
        CurrentStock decimal(18, 3) NULL,
        Result varchar(10) NOT NULL,
        Remarks nvarchar(1000) NULL
    );

    CREATE TABLE #p
    (
        RowNo int NULL,
        ProductId int NOT NULL,
        ProductName varchar(200) NULL,
        UOM varchar(200) NULL,
        RackId int NOT NULL,
        RackCaption varchar(200) NULL,
        LocationId int NOT NULL,
        LocationName varchar(200) NULL,
        Source varchar(20) NOT NULL,
        CurrentStock decimal(18, 3) NOT NULL,
        TargetStock decimal(18, 3) NOT NULL,
        Adjustment decimal(18, 3) NOT NULL,
        PrevRowCount int NOT NULL DEFAULT (0),
        PRIMARY KEY (ProductId, RackId)
    );

    CREATE TABLE #ins (ID int NOT NULL PRIMARY KEY);

    DECLARE @uploadId int, @ref varchar(25), @total int, @products int, @errors int, @lock int,
            @zeroRacks int, @affected int, @expected int, @inserted int, @now datetime;

    BEGIN TRY
        BEGIN TRAN;

        EXEC @lock = sp_getapplock @Resource = 'RackWiseStockUpload', @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 30000;
        IF @lock < 0
            RAISERROR('Another Rack Wise Stock Upload is in progress. Please try again.', 16, 1);

        -- 1. Revalidate every Excel row on the server.
        INSERT INTO #v EXEC dbo.Proc_RackWiseStockUpload_Validate @Rows = @Rows;

        SELECT @total = COUNT(*), @errors = SUM(CASE WHEN Result = 'VALID' THEN 0 ELSE 1 END) FROM #v;
        IF ISNULL(@total, 0) = 0
            RAISERROR('The Excel file has no rows to upload.', 16, 1);
        IF @errors > 0
            RAISERROR('Validation failed for %d row(s). Correct all errors before uploading stock.', 16, 1, @errors);

        -- 2/3. Resolve all allocated racks of the Excel products and their target stock.
        INSERT INTO #p (RowNo, ProductId, ProductName, UOM, RackId, RackCaption, LocationId, LocationName, Source, CurrentStock, TargetStock, Adjustment)
        EXEC dbo.Proc_RackWiseStockUpload_Preview @Rows = @Rows;

        IF (SELECT COUNT(*) FROM #p WHERE Source = 'EXCEL') <> @total
           OR EXISTS (SELECT 1 FROM #v v WHERE NOT EXISTS (SELECT 1 FROM #p p WHERE p.RowNo = v.RowNo AND p.ProductId = v.ProductId AND p.RackId = v.RackId))
            RAISERROR('Could not resolve every Excel row to an allocated active rack.', 16, 1);

        -- 4. Latest current stock, locked until commit. The preview adjustment is not used.
        UPDATE p
        SET CurrentStock = ISNULL(x.Stock, 0),
            Adjustment = p.TargetStock - ISNULL(x.Stock, 0),
            PrevRowCount = ISNULL(x.RowCnt, 0)
        FROM #p p
        OUTER APPLY (SELECT SUM(CASE WHEN mt.Type = 'IN' THEN mt.Quantity WHEN mt.Type = 'OUT' THEN -mt.Quantity ELSE 0 END) AS Stock,
                            COUNT(*) AS RowCnt
                     FROM dbo.MaterialTranscation mt WITH (UPDLOCK, HOLDLOCK)
                     WHERE mt.MaterailId = p.ProductId AND mt.RackId = p.RackId) x;

        SELECT @products = COUNT(DISTINCT ProductId),
               @zeroRacks = SUM(CASE WHEN Source = 'RESET TO ZERO' THEN 1 ELSE 0 END),
               @affected = COUNT(*),
               @expected = SUM(CASE WHEN Adjustment <> 0 THEN 1 ELSE 0 END)
        FROM #p;

        INSERT INTO dbo.RackWiseStockUpload (FileName, UploadedOn, UploadedBy, UploadedByUserId, TotalRows, ProductCount, ZeroResetRacks, AffectedRacks, Status)
        VALUES (@FileName, GETDATE(), @UploadedBy, @UserId, @total, @products, @zeroRacks, @affected, 'IN PROGRESS');
        SET @uploadId = SCOPE_IDENTITY();
        SET @ref = 'RSU-' + RIGHT('000000' + CAST(@uploadId AS varchar(10)), 6);
        UPDATE dbo.RackWiseStockUpload SET UploadRef = @ref WHERE UploadId = @uploadId;

        -- 5. New STOCK RESET adjustment rows only; zero adjustments create no row.
        SET @now = GETDATE();
        INSERT INTO dbo.MaterialTranscation (TransId, TranscationType, TranscationDate, MaterailId, Quantity, LocationId, Type, Updatedby, RackId)
        OUTPUT inserted.ID INTO #ins (ID)
        SELECT @ref, 'STOCK RESET', @now, p.ProductId, ABS(p.Adjustment), CONVERT(varchar(100), p.LocationId),
               CASE WHEN p.Adjustment > 0 THEN 'IN' ELSE 'OUT' END, @UserId, p.RackId
        FROM #p p
        WHERE p.Adjustment <> 0;
        SET @inserted = @@ROWCOUNT;
        IF @inserted <> @expected
            RAISERROR('Stock reset inserted an unexpected number of rows.', 16, 1);

        -- 6. Rack-level verification: final stock = target, existing rows unchanged.
        IF EXISTS (
            SELECT 1
            FROM #p p
            OUTER APPLY (SELECT SUM(CASE WHEN mt.Type = 'IN' THEN mt.Quantity WHEN mt.Type = 'OUT' THEN -mt.Quantity ELSE 0 END) AS FinalStock,
                                SUM(CASE WHEN i.ID IS NULL THEN 1 ELSE 0 END) AS OldCount,
                                SUM(CASE WHEN i.ID IS NOT NULL THEN 0 WHEN mt.Type = 'IN' THEN mt.Quantity WHEN mt.Type = 'OUT' THEN -mt.Quantity ELSE 0 END) AS OldStock
                         FROM dbo.MaterialTranscation mt
                         LEFT JOIN #ins i ON i.ID = mt.ID
                         WHERE mt.MaterailId = p.ProductId AND mt.RackId = p.RackId) n
            WHERE ISNULL(n.FinalStock, 0) - p.TargetStock <> 0
               OR ISNULL(n.OldCount, 0) <> p.PrevRowCount
               OR ISNULL(n.OldStock, 0) <> p.CurrentStock)
            RAISERROR('Rack-level stock verification failed. The upload was rolled back.', 16, 1);

        -- Product-level verification: database rack total = Excel product total.
        IF EXISTS (
            SELECT 1
            FROM (SELECT ProductId, SUM(Stock) AS ExcelTotal FROM @Rows GROUP BY ProductId) e
            OUTER APPLY (SELECT SUM(CASE WHEN mt.Type = 'IN' THEN mt.Quantity WHEN mt.Type = 'OUT' THEN -mt.Quantity ELSE 0 END) AS RackTotal
                         FROM #p p
                         INNER JOIN dbo.MaterialTranscation mt ON mt.MaterailId = p.ProductId AND mt.RackId = p.RackId
                         WHERE p.ProductId = e.ProductId) d
            WHERE ISNULL(d.RackTotal, 0) - e.ExcelTotal <> 0)
            RAISERROR('Product-level stock verification failed. The upload was rolled back.', 16, 1);

        -- 7. Audit every affected rack, including racks reset to zero.
        INSERT INTO dbo.RackWiseStockUploadDetail (UploadId, RowNo, ProductId, RackId, RackCaption, LocationId, Source, PreviousStock, TargetStock, Adjustment, MovementType, ResultingStock, Result)
        SELECT @uploadId, p.RowNo, p.ProductId, p.RackId, p.RackCaption, p.LocationId, p.Source, p.CurrentStock, p.TargetStock, p.Adjustment,
               CASE WHEN p.Adjustment > 0 THEN 'IN' WHEN p.Adjustment < 0 THEN 'OUT' END,
               p.TargetStock, 'SUCCESS'
        FROM #p p;

        UPDATE dbo.RackWiseStockUpload SET Status = 'SUCCESS' WHERE UploadId = @uploadId;

        -- 8. Commit.
        COMMIT TRAN;

        SELECT d.RowNo, d.ProductId, d.RackId, d.RackCaption, d.Source, d.PreviousStock, d.TargetStock, d.Adjustment,
               d.MovementType, d.ResultingStock, d.Result, @ref AS UploadRef
        FROM dbo.RackWiseStockUploadDetail d
        WHERE d.UploadId = @uploadId
        ORDER BY d.UploadDetailId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;
        DECLARE @error nvarchar(2048);
        SET @error = ERROR_MESSAGE();
        RAISERROR('%s', 16, 1, @error);
    END CATCH
END
GO
