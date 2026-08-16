SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.QuotationDetails', 'MasterSalesPrice') IS NULL
BEGIN
    ALTER TABLE dbo.QuotationDetails ADD MasterSalesPrice decimal(18, 2) NULL;
END
GO

IF COL_LENGTH('dbo.QuotationDetails', 'GSTAtQuote') IS NULL
BEGIN
    ALTER TABLE dbo.QuotationDetails ADD GSTAtQuote decimal(18, 2) NULL;
END
GO

IF OBJECT_ID('dbo.trg_QuotationDetails_SetPriceSnapshot', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_QuotationDetails_SetPriceSnapshot;
GO

CREATE TRIGGER dbo.trg_QuotationDetails_SetPriceSnapshot
ON dbo.QuotationDetails
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE qd
       SET MasterSalesPrice = TRY_CONVERT(decimal(18, 2), pm.SalesPrice),
           GSTAtQuote = TRY_CONVERT(decimal(18, 2), pm.GST)
    FROM dbo.QuotationDetails qd
    INNER JOIN inserted i
      ON qd.Quotationid = i.Quotationid
     AND qd.Productid = i.Productid
     AND ISNULL(qd.sino, -1) = ISNULL(i.sino, -1)
    LEFT JOIN dbo.ProductMaster pm
      ON CONVERT(varchar(50), pm.id) = CONVERT(varchar(50), i.Productid);
END
GO

/*
SaveQuotation_Direct deployment note and optional direct-procedure change:

The current SaveQuotation_Direct source was not present in this repository. Keep the
existing procedure body, quotation number generation, customer/update/followup logic,
and TVP parameter unchanged. In the detail INSERT/RE-INSERT section only, add the two
new target columns and populate them from ProductMaster by Productid.

The trigger above already captures these snapshots for INSERT statements without
changing SaveQuotation_Direct. If your deployment standards prefer no trigger, remove
the trigger and apply the INSERT pattern below directly inside SaveQuotation_Direct.

Required INSERT pattern:

    INSERT dbo.QuotationDetails
    (
        Quotationid,
        Productid,
        Rate,
        Quantity,
        Amount,
        sino,
        Status,
        pqty,
        Productserialno,
        MasterSalesPrice,
        GSTAtQuote
    )
    SELECT
        @Quotationid,
        q.Productid,
        q.Rate,
        q.Quantity,
        q.Amount,
        q.sino,
        q.Status,
        q.pqty,
        q.Productserialno,
        TRY_CONVERT(decimal(18, 2), pm.SalesPrice),
        TRY_CONVERT(decimal(18, 2), pm.GST)
    FROM @QuotationDetails q
    LEFT JOIN dbo.ProductMaster pm
      ON CONVERT(varchar(50), pm.id) = CONVERT(varchar(50), q.Productid);

Do not add MasterSalesPrice or GSTAtQuote to dbo.QuotationDetailstype. The application
must not send these values; they must be captured from ProductMaster at save time.
*/
GO

IF OBJECT_ID('dbo.sp_report_quotation', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_report_quotation;
GO

CREATE PROCEDURE dbo.sp_report_quotation
    @FromDate date = NULL,
    @ToDate date = NULL,
    @PageNumber int = 1,
    @PageSize int = 10,
    @SearchText nvarchar(200) = NULL,
    @PriceAlteredOnly bit = NULL,
    @Status varchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber IS NULL OR @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize IS NULL OR @PageSize < 1 SET @PageSize = 10;
    IF @PageSize > 100 SET @PageSize = 100;
    IF @FromDate IS NULL SET @FromDate = CAST(GETDATE() AS date);
    IF @ToDate IS NULL SET @ToDate = @FromDate;
    IF @ToDate < @FromDate SET @ToDate = @FromDate;

    DECLARE @Search nvarchar(210);
    SET @Search = NULLIF(LTRIM(RTRIM(@SearchText)), '');
    IF @Search IS NOT NULL SET @Search = '%' + @Search + '%';

    ;WITH FilteredHeaders AS
    (
        SELECT
            qh.Quotationid,
            qh.Updatedon,
            qh.customername,
            qh.City,
            qh.AssistName,
            qh.Status,
            qh.Iscancel
        FROM dbo.QuotationHeader qh
        WHERE qh.Updatedon >= @FromDate
          AND qh.Updatedon < DATEADD(DAY, 1, @ToDate)
          AND (@Status IS NULL OR @Status = '' OR qh.Status = @Status)
          AND (ISNULL(qh.isdelete, '') NOT IN ('1', 'true', 'TRUE', 'Yes', 'YES'))
          AND (
                @Search IS NULL
                OR qh.Quotationid LIKE @Search
                OR qh.customername LIKE @Search
                OR qh.City LIKE @Search
                OR qh.AssistName LIKE @Search
              )
    ),
    DetailAgg AS
    (
        SELECT
            qd.Quotationid,
            COUNT(1) AS ItemCount,
            SUM(ISNULL(TRY_CONVERT(decimal(18, 3), NULLIF(qd.Quantity, '')), 0)) AS TotalQuantity,
            SUM(ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(qd.Amount, '')), 0)) AS QuotationValue,
            SUM(CASE
                    WHEN qd.MasterSalesPrice IS NOT NULL
                     AND TRY_CONVERT(decimal(18, 2), NULLIF(qd.Rate, '')) IS NOT NULL
                     AND TRY_CONVERT(decimal(18, 2), NULLIF(qd.Rate, '')) <> qd.MasterSalesPrice
                    THEN 1 ELSE 0
                END) AS AlteredItemCount
        FROM dbo.QuotationDetails qd
        INNER JOIN FilteredHeaders fh ON fh.Quotationid = qd.Quotationid
        GROUP BY qd.Quotationid
    ),
    Filtered AS
    (
        SELECT
            qh.Quotationid AS QuotationId,
            qh.Updatedon AS UpdatedOn,
            qh.customername AS CustomerName,
            qh.City,
            qh.AssistName,
            qh.Status,
            ISNULL(da.ItemCount, 0) AS ItemCount,
            ISNULL(da.TotalQuantity, 0) AS TotalQuantity,
            ISNULL(da.QuotationValue, 0) AS QuotationValue,
            ISNULL(da.AlteredItemCount, 0) AS AlteredItemCount,
            CASE WHEN ISNULL(da.AlteredItemCount, 0) > 0 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS HasPriceAltered,
            CASE
                WHEN ISNULL(qh.Iscancel, 0) = 1 OR UPPER(ISNULL(qh.Status, '')) LIKE '%CANCEL%' THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END AS IsCancelled
        FROM FilteredHeaders qh
        LEFT JOIN DetailAgg da ON da.Quotationid = qh.Quotationid
    ),
    PriceFiltered AS
    (
        SELECT *
        FROM Filtered
        WHERE (@PriceAlteredOnly IS NULL OR @PriceAlteredOnly = 0 OR HasPriceAltered = 1)
    ),
    Summary AS
    (
        SELECT
            COUNT(1) AS SummaryTotalQuotations,
            ISNULL(SUM(QuotationValue), 0) AS SummaryQuotationValue,
            ISNULL(SUM(CASE WHEN HasPriceAltered = 1 THEN 1 ELSE 0 END), 0) AS SummaryPriceAltered,
            ISNULL(SUM(CASE WHEN IsCancelled = 1 THEN 1 ELSE 0 END), 0) AS SummaryCancelled
        FROM PriceFiltered
    ),
    Numbered AS
    (
        SELECT
            pf.*,
            COUNT(1) OVER() AS TotalRows,
            ROW_NUMBER() OVER (ORDER BY pf.UpdatedOn DESC, pf.QuotationId DESC) AS RowNumber
        FROM PriceFiltered pf
    )
    SELECT
        n.QuotationId,
        n.UpdatedOn,
        n.CustomerName,
        n.City,
        n.AssistName,
        n.Status,
        n.ItemCount,
        n.TotalQuantity,
        n.QuotationValue,
        n.AlteredItemCount,
        n.HasPriceAltered,
        n.IsCancelled,
        n.TotalRows,
        s.SummaryTotalQuotations,
        s.SummaryQuotationValue,
        s.SummaryPriceAltered,
        s.SummaryCancelled
    FROM Numbered n
    CROSS JOIN Summary s
    WHERE n.RowNumber BETWEEN ((@PageNumber - 1) * @PageSize) + 1 AND (@PageNumber * @PageSize)
    ORDER BY n.RowNumber;
END
GO

IF OBJECT_ID('dbo.sp_report_quotation_detail', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_report_quotation_detail;
GO

CREATE PROCEDURE dbo.sp_report_quotation_detail
    @QuotationId varchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH DetailLines AS
    (
        SELECT
            qd.sino,
            qd.Productid AS ProductId,
            pm.ItemName AS ProductName,
            pm.DisplayName,
            pm.UOM,
            COALESCE(pm.HSN, pm.HSNCODE) AS HSN,
            TRY_CONVERT(decimal(18, 2), NULLIF(qd.Rate, '')) AS QuotedRate,
            qd.MasterSalesPrice,
            TRY_CONVERT(decimal(18, 3), NULLIF(qd.Quantity, '')) AS Quantity,
            TRY_CONVERT(decimal(18, 2), NULLIF(qd.Amount, '')) AS Amount,
            COALESCE(qd.GSTAtQuote, TRY_CONVERT(decimal(18, 2), pm.GST)) AS GST,
            CASE WHEN qd.GSTAtQuote IS NULL THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsGSTFallback,
            CASE
                WHEN qd.MasterSalesPrice IS NOT NULL
                 AND TRY_CONVERT(decimal(18, 2), NULLIF(qd.Rate, '')) IS NOT NULL
                 AND TRY_CONVERT(decimal(18, 2), NULLIF(qd.Rate, '')) <> qd.MasterSalesPrice
                THEN CAST(1 AS bit) ELSE CAST(0 AS bit)
            END AS IsPriceAltered
        FROM dbo.QuotationDetails qd
        LEFT JOIN dbo.ProductMaster pm
          ON CONVERT(varchar(50), pm.id) = CONVERT(varchar(50), qd.Productid)
        WHERE qd.Quotationid = @QuotationId
    ),
    Calculated AS
    (
        SELECT
            *,
            CASE
                WHEN Amount IS NULL THEN NULL
                WHEN GST IS NULL THEN Amount
                ELSE ROUND(Amount / (1 + (GST / 100)), 2)
            END AS TaxableValue,
            CASE
                WHEN Amount IS NULL THEN NULL
                WHEN GST IS NULL THEN 0
                ELSE Amount - ROUND(Amount / (1 + (GST / 100)), 2)
            END AS GSTAmount
        FROM DetailLines
    ),
    GstBreakup AS
    (
        SELECT
            GST AS GSTRate,
            SUM(ISNULL(TaxableValue, 0)) AS TaxableValue,
            SUM(ISNULL(GSTAmount, 0)) AS GSTAmount,
            SUM(ISNULL(Amount, 0)) AS GrandTotal
        FROM Calculated
        GROUP BY GST
    ),
    GrandTotal AS
    (
        SELECT
            SUM(ISNULL(TaxableValue, 0)) AS TotalTaxable,
            SUM(ISNULL(GSTAmount, 0)) AS TotalGST,
            SUM(ISNULL(Amount, 0)) AS GrandTotal
        FROM Calculated
    )
    SELECT
        'DETAIL' AS RowType,
        c.sino AS SortOrder,
        c.ProductId,
        c.ProductName,
        c.DisplayName,
        c.UOM,
        c.HSN,
        c.QuotedRate,
        c.MasterSalesPrice,
        c.Quantity,
        c.Amount,
        c.GST,
        c.IsGSTFallback,
        c.IsPriceAltered,
        c.TaxableValue,
        c.GSTAmount,
        CAST(NULL AS decimal(18, 2)) AS GSTRate,
        gt.TotalTaxable,
        gt.TotalGST,
        gt.GrandTotal
    FROM Calculated c
    CROSS JOIN GrandTotal gt

    UNION ALL

    SELECT
        'GST_BREAKUP' AS RowType,
        100000 + ROW_NUMBER() OVER (ORDER BY gb.GSTRate),
        NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL,
        gb.GSTRate,
        CAST(0 AS bit),
        CAST(0 AS bit),
        gb.TaxableValue,
        gb.GSTAmount,
        gb.GSTRate,
        gt.TotalTaxable,
        gt.TotalGST,
        gt.GrandTotal
    FROM GstBreakup gb
    CROSS JOIN GrandTotal gt

    UNION ALL

    SELECT
        'TOTAL' AS RowType,
        200000,
        NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, gt.GrandTotal,
        NULL,
        CAST(0 AS bit),
        CAST(0 AS bit),
        gt.TotalTaxable,
        gt.TotalGST,
        NULL,
        gt.TotalTaxable,
        gt.TotalGST,
        gt.GrandTotal
    FROM GrandTotal gt
    ORDER BY SortOrder;
END
GO


/* ============================================================
   QUOTATION REPORT PERFORMANCE INDEXES
   SQL Server 2012 compatible
   ============================================================ */

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_QuotationHeader_UpdatedOn_QuotationId'
      AND object_id = OBJECT_ID('dbo.QuotationHeader')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_QuotationHeader_UpdatedOn_QuotationId
    ON dbo.QuotationHeader
    (
        UpdatedOn,
        Quotationid
    )
    INCLUDE
    (
        CustomerName,
        City,
        AssistName,
        Status,
        Iscancel,
        isdelete
    );
END;
GO


IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_QuotationDetails_QuotationId'
      AND object_id = OBJECT_ID('dbo.QuotationDetails')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_QuotationDetails_QuotationId
    ON dbo.QuotationDetails
    (
        Quotationid
    )
    INCLUDE
    (
        Productid,
        Rate,
        Quantity,
        Amount,
        MasterSalesPrice,
        GSTAtQuote
    );
END;
GO


IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_QuotationDetails_ProductId'
      AND object_id = OBJECT_ID('dbo.QuotationDetails')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_QuotationDetails_ProductId
    ON dbo.QuotationDetails
    (
        Productid
    )
    INCLUDE
    (
        Quotationid,
        Rate,
        Quantity,
        Amount
    );
END;
GO


/* Refresh optimizer statistics after deployment */
UPDATE STATISTICS dbo.QuotationHeader;
UPDATE STATISTICS dbo.QuotationDetails;
GO