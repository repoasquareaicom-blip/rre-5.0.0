/*
  Product Rack Allocation page support.

  Creates set-based search and add-only save procedures. Save validates all
  comma-separated rack captions for a product before inserting anything.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.sp_ProductRackAllocation_Search', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_ProductRackAllocation_Search AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_ProductRackAllocation_Search
    @SearchText NVARCHAR(200) = NULL,
    @RackAllocatedOnly BIT = 0,
    @IncludeZeroStock BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Search NVARCHAR(200);

    SET @Search = LTRIM(RTRIM(ISNULL(@SearchText, '')));

    ;WITH OverallStock AS
    (
        SELECT
            mt.MaterailId AS ProductId,
            CAST(SUM(
                CASE
                    WHEN UPPER(LTRIM(RTRIM(ISNULL(mt.[Type], '')))) = 'IN' THEN ISNULL(mt.Quantity, 0)
                    WHEN UPPER(LTRIM(RTRIM(ISNULL(mt.[Type], '')))) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
                    ELSE 0
                END
            ) AS DECIMAL(18,3)) AS CurrentStock
        FROM dbo.MaterialTranscation mt
        GROUP BY mt.MaterailId
    ),
    ProductRows AS
    (
        SELECT
            pm.id AS ProductId,
            ISNULL(NULLIF(pm.DisplayName, ''), pm.ItemName) AS ProductName,
            ISNULL(CONVERT(NVARCHAR(200), pm.Category), '') AS Category,
            pm.SalesPrice AS BrandPrice,
            ISNULL(u.UOM, ISNULL(CONVERT(NVARCHAR(200), pm.UOM), '')) AS UOM,
            CAST(ISNULL(os.CurrentStock, 0) AS DECIMAL(18,3)) AS CurrentStock,
            CASE
                WHEN EXISTS (SELECT 1 FROM dbo.ProductRackMapping prm WHERE prm.ProductId = pm.id) THEN 1
                ELSE 0
            END AS IsRackAllocated
        FROM dbo.ProductMaster pm
        LEFT JOIN OverallStock os
            ON os.ProductId = pm.id
        LEFT JOIN dbo.UOM u
            ON CONVERT(VARCHAR(50), u.Uomid) = LTRIM(RTRIM(CONVERT(VARCHAR(50), pm.UOM)))
        WHERE
            ISNULL(pm.IsDeleted, '0') <> '1'
            AND
            (
                @Search = ''
                OR ISNULL(NULLIF(pm.DisplayName, ''), pm.ItemName) LIKE '%' + @Search + '%'
            )
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY pr.ProductName, pr.ProductId) AS SNo,
        pr.ProductId,
        pr.ProductName,
        pr.Category,
        pr.BrandPrice,
        pr.UOM,
        pr.CurrentStock,
        ISNULL(STUFF((
            SELECT ', ' + rm.RackCaption
            FROM dbo.ProductRackMapping prm
            INNER JOIN dbo.RackMaster rm
                ON rm.RackId = prm.RackId
            LEFT JOIN dbo.LocationMaster lm
                ON lm.LocationId = rm.LocationId
            WHERE prm.ProductId = pr.ProductId
            ORDER BY
                ISNULL(lm.DisplayOrder, 2147483647),
                ISNULL(rm.DisplayOrder, 2147483647),
                rm.RackId
            FOR XML PATH(''), TYPE
        ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), 'Not Allocated') AS InRacks,
        CAST('' AS NVARCHAR(MAX)) AS AddRack
    FROM ProductRows pr
    WHERE
        (@IncludeZeroStock = 1 OR pr.CurrentStock > 0)
        AND (@RackAllocatedOnly = 0 OR pr.IsRackAllocated = 1)
    ORDER BY pr.ProductName, pr.ProductId;
END
GO

IF OBJECT_ID('dbo.sp_ProductRackAllocation_AddRacks', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_ProductRackAllocation_AddRacks AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_ProductRackAllocation_AddRacks
    @ProductId INT,
    @RackCaptions NVARCHAR(MAX),
    @UpdatedBy INT = NULL,
    @InsertedCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @InsertedCount = 0;

    IF ISNULL(@ProductId, 0) <= 0
    BEGIN
        RAISERROR('Product id should be a positive number', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.ProductMaster WHERE id = @ProductId)
    BEGIN
        RAISERROR('Product not found', 16, 1);
        RETURN;
    END

    DECLARE @Requested TABLE
    (
        RackCaption NVARCHAR(200) NOT NULL,
        NormalizedCaption NVARCHAR(200) NOT NULL PRIMARY KEY
    );

    DECLARE @Work NVARCHAR(MAX);
    DECLARE @Token NVARCHAR(200);
    DECLARE @Pos INT;

    SET @Work = REPLACE(ISNULL(@RackCaptions, ''), '+', ',') + ',';

    WHILE LEN(@Work) > 0
    BEGIN
        SET @Pos = CHARINDEX(',', @Work);
        IF @Pos = 0 BREAK;

        SET @Token = LTRIM(RTRIM(SUBSTRING(@Work, 1, @Pos - 1)));
        SET @Work = SUBSTRING(@Work, @Pos + 1, LEN(@Work));

        IF @Token <> '' AND NOT EXISTS (SELECT 1 FROM @Requested WHERE NormalizedCaption = UPPER(@Token))
        BEGIN
            INSERT INTO @Requested (RackCaption, NormalizedCaption)
            VALUES (@Token, UPPER(@Token));
        END
    END

    IF NOT EXISTS (SELECT 1 FROM @Requested)
    BEGIN
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

    BEGIN TRANSACTION;

    INSERT INTO dbo.ProductRackMapping (ProductId, RackId, CreatedOn, UpdatedBy)
    SELECT
        @ProductId,
        rm.RackId,
        GETDATE(),
        @UpdatedBy
    FROM @Requested req
    INNER JOIN dbo.RackMaster rm
        ON UPPER(LTRIM(RTRIM(rm.RackCaption))) = req.NormalizedCaption
       AND ISNULL(rm.IsActive, 0) = 1
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.ProductRackMapping prm
        WHERE prm.ProductId = @ProductId
          AND prm.RackId = rm.RackId
    );

    SET @InsertedCount = @@ROWCOUNT;

    COMMIT TRANSACTION;
END
GO
