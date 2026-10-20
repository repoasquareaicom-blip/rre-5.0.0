/* Read-only report. Deploy to each branch database before opening the screen.
   Source/rules: dbo.GetStockRackWiseDetail (2026_09_10_stock_rackwise_detail.sql).
   MaterialTranscation already includes opening/adjustment IN/OUT, purchase receipts,
   PDI/sales, reversals/returns and posted delivery/receipt notes. Do NOT join their
   source detail tables: doing so would count the same movement twice.
   NULL RackId (Previous / No Rack) is deliberately excluded; never allocate it here.
   Duplicate-looking ledger entries are NOT removed: separate postings can be valid.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
IF TYPE_ID(N'dbo.RackStockReportRackIds') IS NULL
    EXEC(N'CREATE TYPE dbo.RackStockReportRackIds AS TABLE (RackId INT NOT NULL PRIMARY KEY)');
GO
IF OBJECT_ID('dbo.sp_RackWiseProductStockReport', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.sp_RackWiseProductStockReport AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE dbo.sp_RackWiseProductStockReport
    @ProductSearch NVARCHAR(200) = NULL,
    @RackId INT = NULL,
    @ShowZeroStock BIT = 0,
    @RackIds dbo.RackStockReportRackIds READONLY
AS
BEGIN
    SET NOCOUNT ON;
    -- @RackId remains available for existing single-rack callers. An omitted or
    -- empty TVP means no multi-rack restriction; IDs are never concatenated into SQL.
    DECLARE @HasRacks BIT;
    SET @HasRacks = CASE WHEN EXISTS (SELECT 1 FROM @RackIds) THEN 1 ELSE 0 END;
    DECLARE @Search NVARCHAR(200), @Pattern NVARCHAR(810);
    SET @Search = LTRIM(RTRIM(ISNULL(@ProductSearch, N'')));
    -- Treat LIKE metacharacters as literal user input.
    SET @Pattern = N'%' + REPLACE(REPLACE(REPLACE(REPLACE(@Search, N'~', N'~~'), N'%', N'~%'), N'_', N'~_'), N'[', N'~[') + N'%';

    ;WITH Products AS
    (
        SELECT pm.id AS ProductId, pm.ItemName, pm.DisplayName, pm.ItemCode, pm.UOM,
            ROW_NUMBER() OVER (PARTITION BY pm.id ORDER BY pm.ItemName, pm.DisplayName, pm.ItemCode, pm.UOM) AS rn
        FROM dbo.ProductMaster pm
        WHERE ISNULL(pm.IsDeleted, '0') <> '1'
          AND (@Search = N'' OR CONVERT(NVARCHAR(20), pm.id) LIKE @Pattern ESCAPE N'~'
            OR pm.ItemCode LIKE @Pattern ESCAPE N'~'
            OR pm.DisplayName LIKE @Pattern ESCAPE N'~'
            OR pm.ItemName LIKE @Pattern ESCAPE N'~')
    )
    SELECT p.ProductId, ISNULL(NULLIF(p.DisplayName, ''), p.ItemName) AS ProductName,
           ISNULL(p.ItemCode, '') AS ItemCode,
           ISNULL(u.UOM, ISNULL(CONVERT(NVARCHAR(200), p.UOM), '')) AS UOM
    INTO #Products
    FROM Products p
    LEFT JOIN dbo.UOM u ON CONVERT(VARCHAR(50), u.Uomid) = LTRIM(RTRIM(CONVERT(VARCHAR(50), p.UOM)))
    WHERE p.rn = 1;
    CREATE UNIQUE CLUSTERED INDEX IX_Products ON #Products(ProductId);

    -- Aggregate once, before joining mappings. Location remains part of the
    -- balance key, exactly as in GetStockRackWiseDetail.
    SELECT mt.MaterailId AS ProductId, mt.LocationId, mt.RackId,
        CAST(SUM(CASE WHEN UPPER(ISNULL(mt.[Type], '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
                      WHEN UPPER(ISNULL(mt.[Type], '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
                      ELSE 0 END) AS DECIMAL(18,3)) AS Quantity
    INTO #Balances
    FROM dbo.MaterialTranscation mt
    INNER JOIN #Products p ON p.ProductId = mt.MaterailId
    WHERE mt.RackId IS NOT NULL AND (@RackId IS NULL OR mt.RackId = @RackId)
      AND (@HasRacks = 0 OR EXISTS (SELECT 1 FROM @RackIds ids WHERE ids.RackId = mt.RackId))
    GROUP BY mt.MaterailId, mt.LocationId, mt.RackId;

    ;WITH Allocations AS
    (
        SELECT DISTINCT prm.ProductId, rm.RackId, rm.LocationId
        FROM dbo.ProductRackMapping prm
        INNER JOIN #Products p ON p.ProductId = prm.ProductId
        INNER JOIN dbo.RackMaster rm ON rm.RackId = prm.RackId AND rm.IsActive = 1
        INNER JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId AND lm.IsActive = 1
        WHERE @ShowZeroStock = 1 AND (@RackId IS NULL OR rm.RackId = @RackId)
          AND (@HasRacks = 0 OR EXISTS (SELECT 1 FROM @RackIds ids WHERE ids.RackId = rm.RackId))
    ), Keys AS
    (
        SELECT ProductId, RackId FROM #Balances WHERE Quantity <> 0
        UNION
        SELECT ProductId, RackId FROM Allocations
    ), RackBalances AS
    (
        SELECT k.ProductId, k.RackId, CAST(ISNULL(SUM(b.Quantity), 0) AS DECIMAL(18,3)) AS CurrentStockQuantity
        FROM Keys k
        LEFT JOIN #Balances b ON b.ProductId = k.ProductId AND b.RackId = k.RackId
        GROUP BY k.ProductId, k.RackId
    )
    SELECT rm.RackId, rm.RackCaption AS RackName, lm.LocationId, lm.LocationName,
           p.ProductId, p.ProductName, p.ItemCode, p.UOM, b.CurrentStockQuantity
    FROM RackBalances b
    INNER JOIN #Products p ON p.ProductId = b.ProductId
    INNER JOIN dbo.RackMaster rm ON rm.RackId = b.RackId
    INNER JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId
    WHERE b.CurrentStockQuantity > 0
       OR (@ShowZeroStock = 1 AND b.CurrentStockQuantity = 0
           AND EXISTS (SELECT 1 FROM Allocations a WHERE a.ProductId = b.ProductId AND a.RackId = b.RackId))
    ORDER BY lm.LocationName, rm.RackCaption, p.ProductName, p.ProductId
    OPTION (RECOMPILE);
END
GO
-- Optional DBA-reviewed covering index (do not create redundant indexes).
-- CREATE NONCLUSTERED INDEX IX_MaterialTranscation_ProductRack_Report
-- ON dbo.MaterialTranscation(MaterailId, RackId, LocationId) INCLUDE ([Type], Quantity);
-- Existing UX_ProductRackMapping_ProductId_RackId covers the allocation lookup.
