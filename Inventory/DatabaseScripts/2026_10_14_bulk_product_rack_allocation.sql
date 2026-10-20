/*
  Bulk Product Rack Allocation support.

  Adds an add-only bulk allocation procedure for the Product Rack Allocation
  screen. The procedure validates all product ids and rack captions before
  inserting anything, preserves existing mappings, and does not touch stock
  transactions or balances.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.sp_ProductRackAllocation_BulkAddRacks', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_ProductRackAllocation_BulkAddRacks AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_ProductRackAllocation_BulkAddRacks
    @ProductIds NVARCHAR(MAX),
    @RackCaptions NVARCHAR(MAX),
    @UpdatedBy INT = NULL,
    @SelectedProductCount INT OUTPUT,
    @RackCount INT OUTPUT,
    @InsertedCount INT OUTPUT,
    @SkippedCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @SelectedProductCount = 0;
    SET @RackCount = 0;
    SET @InsertedCount = 0;
    SET @SkippedCount = 0;

    DECLARE @Products TABLE
    (
        ProductId INT NOT NULL PRIMARY KEY
    );

    DECLARE @ProductWork NVARCHAR(MAX);
    DECLARE @IdToken NVARCHAR(50);
    DECLARE @ProductPos INT;

    SET @ProductWork = ISNULL(@ProductIds, '') + ',';

    WHILE LEN(@ProductWork) > 0
    BEGIN
        SET @ProductPos = CHARINDEX(',', @ProductWork);
        IF @ProductPos = 0 BREAK;

        SET @IdToken = LTRIM(RTRIM(SUBSTRING(@ProductWork, 1, @ProductPos - 1)));
        SET @ProductWork = SUBSTRING(@ProductWork, @ProductPos + 1, LEN(@ProductWork));

        IF @IdToken <> ''
           AND @IdToken NOT LIKE '%[^0-9]%'
           AND CAST(@IdToken AS INT) > 0
           AND NOT EXISTS (SELECT 1 FROM @Products WHERE ProductId = CAST(@IdToken AS INT))
        BEGIN
            INSERT INTO @Products (ProductId)
            VALUES (CAST(@IdToken AS INT));
        END
    END

    IF NOT EXISTS (SELECT 1 FROM @Products)
    BEGIN
        RAISERROR('Select one or more valid products', 16, 1);
        RETURN;
    END

    DECLARE @InvalidProducts NVARCHAR(MAX);

    SELECT @InvalidProducts = STUFF((
        SELECT ', ' + CONVERT(NVARCHAR(20), p.ProductId)
        FROM @Products p
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.ProductMaster pm
            WHERE pm.id = p.ProductId
        )
        ORDER BY p.ProductId
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '');

    IF ISNULL(@InvalidProducts, '') <> ''
    BEGIN
        RAISERROR('Product not found: %s', 16, 1, @InvalidProducts);
        RETURN;
    END

    DECLARE @Requested TABLE
    (
        RackCaption NVARCHAR(200) NOT NULL,
        NormalizedCaption NVARCHAR(200) NOT NULL PRIMARY KEY
    );

    DECLARE @RackWork NVARCHAR(MAX);
    DECLARE @RackToken NVARCHAR(200);
    DECLARE @RackPos INT;

    SET @RackWork = REPLACE(ISNULL(@RackCaptions, ''), '+', ',') + ',';

    WHILE LEN(@RackWork) > 0
    BEGIN
        SET @RackPos = CHARINDEX(',', @RackWork);
        IF @RackPos = 0 BREAK;

        SET @RackToken = LTRIM(RTRIM(SUBSTRING(@RackWork, 1, @RackPos - 1)));
        SET @RackWork = SUBSTRING(@RackWork, @RackPos + 1, LEN(@RackWork));

        IF @RackToken <> ''
           AND NOT EXISTS (SELECT 1 FROM @Requested WHERE NormalizedCaption = UPPER(@RackToken))
        BEGIN
            INSERT INTO @Requested (RackCaption, NormalizedCaption)
            VALUES (@RackToken, UPPER(@RackToken));
        END
    END

    IF NOT EXISTS (SELECT 1 FROM @Requested)
    BEGIN
        RAISERROR('Enter one or more rack names', 16, 1);
        RETURN;
    END

    DECLARE @InvalidRacks NVARCHAR(MAX);

    SELECT @InvalidRacks = STUFF((
        SELECT ', ' + req.RackCaption
        FROM @Requested req
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.RackMaster rm
            WHERE UPPER(LTRIM(RTRIM(rm.RackCaption))) = req.NormalizedCaption
              AND ISNULL(rm.IsActive, 0) = 1
        )
        ORDER BY req.RackCaption
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '');

    IF ISNULL(@InvalidRacks, '') <> ''
    BEGIN
        RAISERROR('Invalid Rack(s): %s', 16, 1, @InvalidRacks);
        RETURN;
    END

    DECLARE @DuplicateRackCaptions NVARCHAR(MAX);

    SELECT @DuplicateRackCaptions = STUFF((
        SELECT ', ' + req.RackCaption
        FROM @Requested req
        WHERE
        (
            SELECT COUNT(1)
            FROM dbo.RackMaster rm
            WHERE UPPER(LTRIM(RTRIM(rm.RackCaption))) = req.NormalizedCaption
              AND ISNULL(rm.IsActive, 0) = 1
        ) > 1
        ORDER BY req.RackCaption
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '');

    IF ISNULL(@DuplicateRackCaptions, '') <> ''
    BEGIN
        RAISERROR('Rack caption exists more than once: %s', 16, 1, @DuplicateRackCaptions);
        RETURN;
    END

    SELECT @SelectedProductCount = COUNT(1)
    FROM @Products;

    SELECT @RackCount = COUNT(1)
    FROM @Requested;

    BEGIN TRANSACTION;

    INSERT INTO dbo.ProductRackMapping (ProductId, RackId, CreatedOn, UpdatedBy)
    SELECT
        p.ProductId,
        rm.RackId,
        GETDATE(),
        @UpdatedBy
    FROM @Products p
    CROSS JOIN @Requested req
    INNER JOIN dbo.RackMaster rm
        ON UPPER(LTRIM(RTRIM(rm.RackCaption))) = req.NormalizedCaption
       AND ISNULL(rm.IsActive, 0) = 1
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.ProductRackMapping prm
        WHERE prm.ProductId = p.ProductId
          AND prm.RackId = rm.RackId
    );

    SET @InsertedCount = @@ROWCOUNT;
    SET @SkippedCount = (@SelectedProductCount * @RackCount) - @InsertedCount;

    COMMIT TRANSACTION;
END
GO
