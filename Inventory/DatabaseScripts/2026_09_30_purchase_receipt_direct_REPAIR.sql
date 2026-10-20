/*
  PRODUCTION REPAIR: dbo.SavePurchaseReceipt_direct

  Symptom in Purchase Receipt save:
    The procedure "SavePurchaseReceipt_direct" has no parameter named "@PurchaseDetails".

  Cause:
    2026_09_28_purchase_receipt_decimal_qty.sql (development script) drops
    SavePurchaseReceipt_direct and leaves an empty stub with no parameters.

  This script restores the full procedure exactly as tested on RRE_NEW.
  It only alters the procedure. It does not change tables, types or data.
  Safe to run more than once.

  Run in the production application database (not master):
  sqlcmd -S <server> -d <production-database> -E -b -i 2026_09_30_purchase_receipt_direct_REPAIR.sql
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

PRINT '=== BEFORE ===';
SELECT DB_NAME() AS DatabaseName, p.parameter_id, p.name, TYPE_NAME(p.user_type_id) AS TypeName
FROM sys.parameters p
WHERE p.object_id = OBJECT_ID('dbo.SavePurchaseReceipt_direct')
ORDER BY p.parameter_id;
GO

IF DB_NAME() IN ('master', 'model', 'msdb', 'tempdb')
    RAISERROR('Refusing to run in a system database.', 20, 1) WITH LOG;
IF TYPE_ID('dbo.PurchaseReceiptDetailstype') IS NULL
    RAISERROR('dbo.PurchaseReceiptDetailstype was not found. Run the purchase receipt decimal quantity script first.', 20, 1) WITH LOG;
IF TYPE_ID('dbo.PurchaseReceiptRackDetailType') IS NULL
    RAISERROR('dbo.PurchaseReceiptRackDetailType was not found. Run the rack-wise purchase receipt script first.', 20, 1) WITH LOG;
GO

IF OBJECT_ID('dbo.SavePurchaseReceipt_direct', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.SavePurchaseReceipt_direct AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE dbo.SavePurchaseReceipt_direct
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
    SET @result = '';

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM @PurchaseDetails WHERE ISNULL(RecievedQuantity, 0) > 0)
        BEGIN
            RAISERROR('Enter received quantity for at least one product.', 16, 1);
        END

        IF EXISTS (SELECT 1 FROM @RackDetails WHERE Quantity <= 0)
        BEGIN
            RAISERROR('Rack quantity should be greater than zero.', 16, 1);
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
        )
        BEGIN
            RAISERROR('Invalid active rack allocation found.', 16, 1);
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
            RAISERROR('Rack allocation exists for product not in purchase details.', 16, 1);
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
            RAISERROR('Rack allocation missing for received product.', 16, 1);
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
            RAISERROR('Rack allocation should not exist for zero received quantity.', 16, 1);
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
            RAISERROR('Rack allocation total must match received quantity.', 16, 1);
        END

        IF @isnew <> 0
        BEGIN
            SELECT @POId = PurchaseId
            FROM dbo.PurchaseOrderHeader
            WHERE OrderNumber = @OrderNumber
              AND ISNULL(IsDeleted, 0) = 0;

            IF ISNULL(@POId, 0) = 0
            BEGIN
                RAISERROR('Purchase order not found.', 16, 1);
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
                RAISERROR('Purchase detail product is not part of selected purchase order.', 16, 1);
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
                RAISERROR('Received quantity cannot be greater than remaining purchase order quantity.', 16, 1);
            END
        END

        BEGIN TRANSACTION;

        IF (@isnew = 0)
        BEGIN
            SET @POmaxid = (SELECT COUNT(*) FROM dbo.PurchaseOrderHeader);
            SET @POvalue = @POmaxid + 1;
            SET @PO0utid = 'RR PO ' + @POvalue + '/' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
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
                'Closed',
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
            SET @PR0utid = 'RR PR ' + @PRvalue + '/' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
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
            SET @PR0utid = 'RR PR ' + @PRvalue + '/' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
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
                Status = 'Closed'
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
            'Purchase',
            GETDATE(),
            rd.ProductId,
            rd.Quantity,
            rm.LocationId,
            rd.RackId,
            'IN'
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
END
GO

PRINT '=== AFTER ===';
SELECT p.parameter_id, p.name, TYPE_NAME(p.user_type_id) AS TypeName, p.is_output
FROM sys.parameters p
WHERE p.object_id = OBJECT_ID('dbo.SavePurchaseReceipt_direct')
ORDER BY p.parameter_id;

IF (SELECT COUNT(*) FROM sys.parameters
    WHERE object_id = OBJECT_ID('dbo.SavePurchaseReceipt_direct')
      AND name IN ('@PurchaseDetails', '@RackDetails', '@out', '@result')) = 4
    PRINT 'SavePurchaseReceipt_direct repaired: 12 parameters present.';
ELSE
    RAISERROR('SavePurchaseReceipt_direct is still missing parameters.', 16, 1);
GO