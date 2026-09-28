/*
  Sales return PDI stock.
  Positive quotation quantity remains PDI OUT.
  Negative quotation quantity posts Sales Return IN for ABS(quantity).
  Reversal of a Sales Return posts Sales Return-REVERSE OUT.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.DbDeploymentHistory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DbDeploymentHistory
    (
        DeploymentId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DbDeploymentHistory PRIMARY KEY,
        ScriptName VARCHAR(255) NOT NULL CONSTRAINT UX_DbDeploymentHistory_ScriptName UNIQUE,
        AppliedOn DATETIME NOT NULL CONSTRAINT DF_DbDeploymentHistory_AppliedOn DEFAULT (GETDATE()),
        AppliedBy VARCHAR(100) NULL
    );
END
GO

IF TYPE_ID('dbo.PdiRackAllocationType') IS NULL
BEGIN
    CREATE TYPE dbo.PdiRackAllocationType AS TABLE
    (
        ProductId INT NOT NULL,
        RackId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.PdiRackAllocation', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PdiRackAllocation
    (
        PdiRackAllocationId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PdiRackAllocation PRIMARY KEY,
        QuotationId VARCHAR(100) NOT NULL,
        ProductId INT NOT NULL,
        LocationId INT NOT NULL,
        RackId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL,
        CreatedOn DATETIME NOT NULL CONSTRAINT DF_PdiRackAllocation_CreatedOn DEFAULT (GETDATE()),
        UpdatedOn DATETIME NULL,
        UpdatedBy VARCHAR(100) NULL
    );
END
GO

IF OBJECT_ID('dbo.PdiRackAllocation', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.PdiRackAllocation') AND name = 'UX_PdiRackAllocation_Quotation_Product_Rack')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_PdiRackAllocation_Quotation_Product_Rack
    ON dbo.PdiRackAllocation (QuotationId, ProductId, RackId);
END
GO

IF OBJECT_ID('dbo.PdiRackAllocation', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.PdiRackAllocation') AND name = 'IX_PdiRackAllocation_Product_Rack')
BEGIN
    CREATE NONCLUSTERED INDEX IX_PdiRackAllocation_Product_Rack
    ON dbo.PdiRackAllocation (ProductId, RackId);
END
GO

IF OBJECT_ID('dbo.PdiRackAllocation', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PdiRackAllocation_RackMaster')
BEGIN
    ALTER TABLE dbo.PdiRackAllocation
    ADD CONSTRAINT FK_PdiRackAllocation_RackMaster FOREIGN KEY (RackId)
    REFERENCES dbo.RackMaster (RackId);
END
GO

IF OBJECT_ID('dbo.SaveQuotationPdi_RackWise', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.SaveQuotationPdi_RackWise AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.SaveQuotationPdi_RackWise
(
    @isnew INT,
    @Quotationid VARCHAR(100) = NULL,
    @Customerid VARCHAR(100) = NULL,
    @date DATETIME = NULL,
    @Referenceid VARCHAR(100) = NULL,
    @Assist VARCHAR(100) = NULL,
    @status VARCHAR(100) = NULL,
    @Updatedby VARCHAR(100) = NULL,
    @AssistName VARCHAR(100) = NULL,
    @Customername VARCHAR(250) = NULL,
    @City VARCHAR(100) = NULL,
    @QuotationDetails dbo.QuotationPditype_2 READONLY,
    @RackDetails dbo.PdiRackAllocationType READONLY,
    @output INT OUTPUT,
    @result VARCHAR(100) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @output = 0;
    SET @result = '';

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM @RackDetails WHERE Quantity <= 0)
        BEGIN
            RAISERROR('Rack allocation quantity should be greater than zero.', 16, 1);
        END

        IF EXISTS (
            SELECT ProductId, RackId
            FROM @RackDetails
            GROUP BY ProductId, RackId
            HAVING COUNT(*) > 1
        )
        BEGIN
            RAISERROR('Duplicate rack allocation found for product and rack.', 16, 1);
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
               OR ISNULL(lm.AllowForSales, 0) = 0
        )
        BEGIN
            RAISERROR('Invalid active sales rack allocation found.', 16, 1);
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
            RAISERROR('Rack allocation is not assigned to product.', 16, 1);
        END

        BEGIN TRANSACTION;

        EXEC dbo.SaveQuotationPdi_Direct_ProductNo
            @isnew = @isnew,
            @Quotationid = @Quotationid,
            @Customerid = @Customerid,
            @date = @date,
            @Referenceid = @Referenceid,
            @Assist = @Assist,
            @status = @status,
            @Updatedby = @Updatedby,
            @AssistName = @AssistName,
            @Customername = @Customername,
            @City = @City,
            @QuotationDetails = @QuotationDetails,
            @output = @output OUTPUT,
            @result = @result OUTPUT;

        DECLARE @SavedQuotationId VARCHAR(100);
        SET @SavedQuotationId = NULLIF(@result, '');
        IF @SavedQuotationId IS NULL
        BEGIN
            SET @SavedQuotationId = @Quotationid;
        END

        DELETE FROM dbo.PdiRackAllocation
        WHERE QuotationId = @SavedQuotationId;

        INSERT INTO dbo.PdiRackAllocation
        (
            QuotationId,
            ProductId,
            LocationId,
            RackId,
            Quantity,
            CreatedOn,
            UpdatedBy
        )
        SELECT
            @SavedQuotationId,
            rd.ProductId,
            rm.LocationId,
            rd.RackId,
            rd.Quantity,
            GETDATE(),
            @Updatedby
        FROM @RackDetails rd
        INNER JOIN dbo.RackMaster rm
            ON rm.RackId = rd.RackId
        WHERE rd.Quantity > 0;

        IF OBJECT_ID('dbo.MaterialTranscation', 'U') IS NOT NULL
           AND COL_LENGTH('dbo.MaterialTranscation', 'RackId') IS NOT NULL
           AND EXISTS (SELECT 1 FROM @RackDetails)
        BEGIN
            DECLARE @TransactionTypeColumn SYSNAME =
                CASE
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'TranscationType') IS NOT NULL THEN 'TranscationType'
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'TransactionType') IS NOT NULL THEN 'TransactionType'
                    ELSE NULL
                END;

            DECLARE @TransactionDateColumn SYSNAME =
                CASE
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'TranscationDate') IS NOT NULL THEN 'TranscationDate'
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'TransactionDate') IS NOT NULL THEN 'TransactionDate'
                    ELSE NULL
                END;

            DECLARE @MaterialIdColumn SYSNAME =
                CASE
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'MaterailId') IS NOT NULL THEN 'MaterailId'
                    WHEN COL_LENGTH('dbo.MaterialTranscation', 'MaterialId') IS NOT NULL THEN 'MaterialId'
                    ELSE NULL
                END;

            IF @TransactionTypeColumn IS NULL OR @TransactionDateColumn IS NULL OR @MaterialIdColumn IS NULL
            BEGIN
                RAISERROR('MaterialTranscation table does not have the expected transaction/product columns.', 16, 1);
            END

            CREATE TABLE #PdiRackMaterialTranscation
            (
                ProductId INT NOT NULL,
                LocationId INT NOT NULL,
                RackId INT NOT NULL,
                Quantity DECIMAL(18,3) NOT NULL,
                IsReturn BIT NOT NULL
            );

            INSERT INTO #PdiRackMaterialTranscation (ProductId, LocationId, RackId, Quantity, IsReturn)
            SELECT
                rd.ProductId,
                rm.LocationId,
                rd.RackId,
                rd.Quantity,
                CASE WHEN ISNULL(q.LineQty, 0) < 0 THEN 1 ELSE 0 END
            FROM @RackDetails rd
            INNER JOIN dbo.RackMaster rm
                ON rm.RackId = rd.RackId
            LEFT JOIN
            (
                SELECT
                    CONVERT(INT, CONVERT(DECIMAL(18,3), qd.Productid)) AS ProductId,
                    SUM(CONVERT(DECIMAL(18,3), qd.Quantity)) AS LineQty
                FROM @QuotationDetails qd
                WHERE ISNUMERIC(qd.Productid) = 1
                  AND ISNUMERIC(qd.Quantity) = 1
                GROUP BY CONVERT(INT, CONVERT(DECIMAL(18,3), qd.Productid))
            ) q
                ON q.ProductId = rd.ProductId
            WHERE rd.Quantity > 0;

            DECLARE @MaterialSql NVARCHAR(MAX) = N'
DELETE mt
FROM dbo.MaterialTranscation mt
INNER JOIN #PdiRackMaterialTranscation rd
    ON mt.' + QUOTENAME(@MaterialIdColumn) + N' = rd.ProductId
WHERE mt.TransId = @SavedQuotationId;

INSERT INTO dbo.MaterialTranscation
(
    TransId,
    ' + QUOTENAME(@TransactionTypeColumn) + N',
    ' + QUOTENAME(@TransactionDateColumn) + N',
    ' + QUOTENAME(@MaterialIdColumn) + N',
    Quantity,
    LocationId,
    RackId,
    Type,
    Updatedby
)
SELECT
    @SavedQuotationId,
    CASE WHEN rd.IsReturn = 1 THEN ''Sales Return'' ELSE ''PDI'' END,
    GETDATE(),
    rd.ProductId,
    ABS(rd.Quantity),
    rd.LocationId,
    rd.RackId,
    CASE WHEN rd.IsReturn = 1 THEN ''IN'' ELSE ''OUT'' END,
    @Updatedby
FROM #PdiRackMaterialTranscation rd;';

            EXEC sp_executesql
                @MaterialSql,
                N'@SavedQuotationId VARCHAR(100), @Updatedby VARCHAR(100)',
                @SavedQuotationId,
                @Updatedby;
        END

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
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DbDeploymentHistory WHERE ScriptName = '2026_09_26_sales_return_pdi.sql')
BEGIN
    INSERT INTO dbo.DbDeploymentHistory (ScriptName, AppliedBy)
    VALUES ('2026_09_26_sales_return_pdi.sql', SUSER_SNAME());
END
GO
IF OBJECT_ID('dbo.ReverseQuotationPdi_RackWise', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.ReverseQuotationPdi_RackWise AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.ReverseQuotationPdi_RackWise
(
    @QuotationId VARCHAR(100),
    @UpdatedBy VARCHAR(100),
    @RackDetails dbo.PdiReverseRackDetailType READONLY,
    @ReturnRackDetails dbo.PdiReverseRackDetailType READONLY,
    @output INT OUTPUT,
    @result VARCHAR(200) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @output = 0;
    SET @result = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM dbo.QuotationHeader WHERE Quotationid = @QuotationId)
        BEGIN
            RAISERROR('Quotation does not exist.', 16, 1);
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.QuotationHeader WHERE Quotationid = @QuotationId AND ISNULL(IsPDI, 0) = 1)
        BEGIN
            RAISERROR('Quotation is not in PDI state.', 16, 1);
        END

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.MaterialTranscation
            WHERE TransId = @QuotationId
              AND (
                    (TranscationType = 'PDI' AND Type = 'OUT')
                    OR (TranscationType = 'Sales Return' AND UPPER(LTRIM(RTRIM(Type))) = 'IN')
                  )
        )
        BEGIN
            RAISERROR('Original PDI stock transaction was not found.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM dbo.MaterialTranscation
            WHERE TransId = @QuotationId
              AND TranscationType = 'PDI-REVERSE'
              AND Type = 'IN'
        )
        OR EXISTS
        (
            SELECT 1
            FROM dbo.MaterialTranscation
            WHERE TransId = @QuotationId
              AND TranscationType = 'Sales Return-REVERSE'
              AND UPPER(LTRIM(RTRIM(Type))) = 'OUT'
        )
        BEGIN
            RAISERROR('PDI has already been reversed.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM dbo.QuotationEstimation
            WHERE Quotationid = @QuotationId
              AND ISNULL(IsBilled, 0) = 1
        )
        BEGIN
            RAISERROR('PDI cannot be reversed because payment/billing has already been completed.', 16, 1);
        END

        IF EXISTS (SELECT 1 FROM @RackDetails WHERE Quantity <= 0)
        BEGIN
            RAISERROR('Reverse PDI rack quantity should be greater than zero.', 16, 1);
        END

        IF EXISTS
        (
            SELECT ProductId, RackId
            FROM @RackDetails
            GROUP BY ProductId, RackId
            HAVING COUNT(*) > 1
        )
        BEGIN
            RAISERROR('Duplicate reverse rack allocation found for product and rack.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM @RackDetails rd
            LEFT JOIN dbo.RackMaster rm
                ON rm.RackId = rd.RackId
            WHERE rm.RackId IS NULL
        )
        BEGIN
            RAISERROR('Invalid rack allocation found for Reverse PDI.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM @RackDetails rd
            INNER JOIN dbo.ProductMaster pm
                ON pm.id = rd.ProductId
            INNER JOIN dbo.RackMaster rm
                ON rm.RackId = rd.RackId
            INNER JOIN dbo.LocationMaster lm
                ON lm.LocationId = rm.LocationId
            WHERE ISNULL(pm.RackWiseStockMovement, 0) = 1
              AND
              (
                  ISNULL(rm.IsActive, 0) = 0
                  OR ISNULL(lm.IsActive, 0) = 0
                  OR NOT EXISTS
                  (
                      SELECT 1
                      FROM dbo.ProductRackMapping prm
                      WHERE prm.ProductId = rd.ProductId
                        AND prm.RackId = rd.RackId
                  )
              )
        )
        BEGIN
            RAISERROR('Reverse PDI rack allocation is not an active assigned rack for rack-wise product.', 16, 1);
        END

        CREATE TABLE #OriginalPdiOut
        (
            ProductId INT NOT NULL,
            Quantity DECIMAL(18,3) NOT NULL
        );

        INSERT INTO #OriginalPdiOut (ProductId, Quantity)
        SELECT
            mt.MaterailId,
            CAST(SUM(ISNULL(mt.Quantity, 0)) AS DECIMAL(18,3))
        FROM dbo.MaterialTranscation mt
        WHERE mt.TransId = @QuotationId
          AND mt.TranscationType = 'PDI'
          AND mt.Type = 'OUT'
        GROUP BY mt.MaterailId;

        CREATE TABLE #ReverseRackDetails
        (
            ProductId INT NOT NULL,
            LocationId INT NOT NULL,
            RackId INT NOT NULL,
            Quantity DECIMAL(18,3) NOT NULL
        );

        INSERT INTO #ReverseRackDetails (ProductId, LocationId, RackId, Quantity)
        SELECT
            rd.ProductId,
            rm.LocationId,
            rd.RackId,
            CAST(SUM(rd.Quantity) AS DECIMAL(18,3))
        FROM @RackDetails rd
        INNER JOIN dbo.RackMaster rm
            ON rm.RackId = rd.RackId
        GROUP BY rd.ProductId, rm.LocationId, rd.RackId;

        IF EXISTS
        (
            SELECT 1
            FROM #OriginalPdiOut o
            FULL OUTER JOIN
            (
                SELECT ProductId, CAST(SUM(Quantity) AS DECIMAL(18,3)) AS Quantity
                FROM #ReverseRackDetails
                GROUP BY ProductId
            ) r
                ON r.ProductId = o.ProductId
            WHERE o.ProductId IS NULL
               OR r.ProductId IS NULL
               OR ISNULL(o.Quantity, 0) <> ISNULL(r.Quantity, 0)
        )
        BEGIN
            RAISERROR('Reverse PDI quantities must exactly match original PDI OUT quantities by product.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM #ReverseRackDetails rd
            INNER JOIN dbo.ProductMaster pm
                ON pm.id = rd.ProductId
            WHERE ISNULL(pm.RackWiseStockMovement, 0) = 0
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM dbo.ProductRackMapping prm
                  INNER JOIN dbo.RackMaster rm
                      ON rm.RackId = prm.RackId
                  INNER JOIN dbo.LocationMaster lm
                      ON lm.LocationId = rm.LocationId
                  WHERE prm.ProductId = rd.ProductId
                    AND prm.RackId = rd.RackId
                    AND ISNULL(rm.IsActive, 0) = 1
                    AND ISNULL(lm.IsActive, 0) = 1
              )
        )
        BEGIN
            RAISERROR('Reverse PDI rack is not an active assigned rack.', 16, 1);
        END

        IF EXISTS (SELECT 1 FROM @ReturnRackDetails WHERE Quantity <= 0)
        BEGIN
            RAISERROR('Sales return reversal quantity should be greater than zero.', 16, 1);
        END

        CREATE TABLE #OriginalSalesReturn
        (
            ProductId INT NOT NULL,
            Quantity DECIMAL(18,3) NOT NULL
        );

        INSERT INTO #OriginalSalesReturn (ProductId, Quantity)
        SELECT
            mt.MaterailId,
            CAST(SUM(ISNULL(mt.Quantity, 0)) AS DECIMAL(18,3))
        FROM dbo.MaterialTranscation mt
        WHERE mt.TransId = @QuotationId
          AND mt.TranscationType = 'Sales Return'
          AND UPPER(LTRIM(RTRIM(mt.Type))) = 'IN'
        GROUP BY mt.MaterailId;

        CREATE TABLE #ReturnRackDetails
        (
            ProductId INT NOT NULL,
            LocationId INT NOT NULL,
            RackId INT NOT NULL,
            Quantity DECIMAL(18,3) NOT NULL
        );

        INSERT INTO #ReturnRackDetails (ProductId, LocationId, RackId, Quantity)
        SELECT
            rd.ProductId,
            rm.LocationId,
            rd.RackId,
            CAST(SUM(rd.Quantity) AS DECIMAL(18,3))
        FROM @ReturnRackDetails rd
        INNER JOIN dbo.RackMaster rm
            ON rm.RackId = rd.RackId
        GROUP BY rd.ProductId, rm.LocationId, rd.RackId;

        IF EXISTS
        (
            SELECT 1
            FROM #OriginalSalesReturn o
            FULL OUTER JOIN
            (
                SELECT ProductId, CAST(SUM(Quantity) AS DECIMAL(18,3)) AS Quantity
                FROM #ReturnRackDetails
                GROUP BY ProductId
            ) r
                ON r.ProductId = o.ProductId
            WHERE o.ProductId IS NULL
               OR r.ProductId IS NULL
               OR ISNULL(o.Quantity, 0) <> ISNULL(r.Quantity, 0)
        )
        BEGIN
            RAISERROR('Sales return reversal quantities must exactly match the original sales return quantities by product.', 16, 1);
        END

        IF EXISTS
        (
            SELECT 1
            FROM #ReturnRackDetails rd
            WHERE NOT EXISTS
            (
                SELECT 1
                FROM dbo.ProductRackMapping prm
                INNER JOIN dbo.RackMaster rm
                    ON rm.RackId = prm.RackId
                INNER JOIN dbo.LocationMaster lm
                    ON lm.LocationId = rm.LocationId
                WHERE prm.ProductId = rd.ProductId
                  AND prm.RackId = rd.RackId
                  AND ISNULL(rm.IsActive, 0) = 1
                  AND ISNULL(lm.IsActive, 0) = 1
            )
        )
        BEGIN
            RAISERROR('Sales return reversal rack is not an active assigned rack.', 16, 1);
        END

        DECLARE @ReturnStockMessage NVARCHAR(4000);

        SELECT TOP 1
            @ReturnStockMessage =
                'Insufficient stock to reverse sales return. Product: '
                + ISNULL(NULLIF(pm.DisplayName, ''), ISNULL(pm.ItemName, CONVERT(VARCHAR(20), req.ProductId)))
                + ', Rack: ' + ISNULL(rm.RackCaption, CONVERT(VARCHAR(20), req.RackId))
                + ', Required Qty: ' + CONVERT(VARCHAR(30), req.RequiredQty)
                + ', Available Qty: ' + CONVERT(VARCHAR(30), req.AvailableQty)
        FROM
        (
            SELECT
                rd.ProductId,
                rd.RackId,
                rd.Quantity AS RequiredQty,
                CAST(ISNULL((
                    SELECT SUM(
                        CASE
                            WHEN UPPER(LTRIM(RTRIM(b.Type))) = 'IN' THEN ISNULL(b.Quantity, 0)
                            WHEN UPPER(LTRIM(RTRIM(b.Type))) = 'OUT' THEN -ISNULL(b.Quantity, 0)
                            ELSE 0
                        END)
                    FROM dbo.MaterialTranscation b
                    WHERE b.MaterailId = rd.ProductId
                      AND b.RackId = rd.RackId
                ), 0) AS DECIMAL(18,3)) AS AvailableQty
            FROM #ReturnRackDetails rd
        ) req
        LEFT JOIN dbo.ProductMaster pm
            ON pm.id = req.ProductId
        LEFT JOIN dbo.RackMaster rm
            ON rm.RackId = req.RackId
        WHERE req.AvailableQty < req.RequiredQty;

        IF @ReturnStockMessage IS NOT NULL
        BEGIN
            RAISERROR(@ReturnStockMessage, 16, 1);
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
            Type,
            Updatedby
        )
        SELECT
            @QuotationId,
            'PDI-REVERSE',
            GETDATE(),
            rd.ProductId,
            rd.Quantity,
            rd.LocationId,
            rd.RackId,
            'IN',
            @UpdatedBy
        FROM #ReverseRackDetails rd
        WHERE rd.Quantity > 0;

        INSERT INTO dbo.MaterialTranscation
        (
            TransId,
            TranscationType,
            TranscationDate,
            MaterailId,
            Quantity,
            LocationId,
            RackId,
            Type,
            Updatedby
        )
        SELECT
            @QuotationId,
            'Sales Return-REVERSE',
            GETDATE(),
            rd.ProductId,
            rd.Quantity,
            rd.LocationId,
            rd.RackId,
            'OUT',
            @UpdatedBy
        FROM #ReturnRackDetails rd
        WHERE rd.Quantity > 0;

        UPDATE dbo.QuotationHeader
        SET IsPDI = 0
        WHERE Quotationid = @QuotationId;

        SET @output = 1;
        SET @result = 'PDI reversed successfully.';

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
END
GO



