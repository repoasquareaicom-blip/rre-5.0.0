/*
  Purchase Receipt quantity is decimal(18,3).
  PurchaseReceiptDetailstype stored quantity as nvarchar, and
  SavePurchaseReceipt_direct compared that text to an integer.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.SavePurchaseReceipt_direct', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SavePurchaseReceipt_direct;
GO

IF OBJECT_ID('dbo.SavePurchaseReceipt', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SavePurchaseReceipt;
GO

IF TYPE_ID('dbo.PurchaseReceiptDetailstype') IS NOT NULL
    DROP TYPE dbo.PurchaseReceiptDetailstype;
GO

CREATE TYPE dbo.PurchaseReceiptDetailstype AS TABLE
(
    OrderQuantity DECIMAL(18,3) NULL,
    RecievedQuantity DECIMAL(18,3) NULL,
    Productid INT NULL
);
GO

IF EXISTS (
    SELECT 1
    FROM sys.columns c
    INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.PurchaseOrderDetails')
      AND c.name = 'Quantity'
      AND ty.name = 'decimal'
      AND c.scale < 3
)
BEGIN
    ALTER TABLE dbo.PurchaseOrderDetails ALTER COLUMN Quantity DECIMAL(18,3) NULL;
END
GO

CREATE PROCEDURE dbo.SavePurchaseReceipt
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
        SET @PO0utid = 'RR PO ' + @POvalue + '/' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
        SET @POId = (SELECT ISNULL(MAX(PurchaseId), 0) + 1 FROM PurchaseOrderHeader);
        INSERT INTO PurchaseOrderHeader (PurchaseId, OrderNumber, OrderDate, VendorId, Status, EnteredBy, EnteredOn, Remarks, IsDeleted, ApprovedBy, ApproverOn)
        VALUES (@POId, @PO0utid, @OrderDate, @VendorId, @Status, @EnteredBy, GETDATE(), @Remarks, 0, @EnteredBy, GETDATE());

        SELECT @i = (SELECT ISNULL(MAX(Id), 0) FROM PurchaseOrderDetails);
        INSERT INTO PurchaseOrderDetails (Id, PurchaseId, OrderNumber, Quantity, ProductId)
        SELECT ROW_NUMBER() OVER (ORDER BY @i) + @i AS Id, @POId, @PO0utid, RecievedQuantity, Productid
        FROM @PurchaseDetails;

        SET @PRmaxid = (SELECT COUNT(*) FROM PurchaseReceiptHeader);
        SET @PRvalue = @PRmaxid + 1;
        SET @PR0utid = 'RR PR ' + @PRvalue + '/' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
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
        SET @PR0utid = 'RR PR ' + @PRvalue + '/' + CAST(YEAR(GETDATE()) AS VARCHAR(10));
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
END
GO

IF OBJECT_ID('dbo.SavePurchaseReceipt_direct', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.SavePurchaseReceipt_direct AS BEGIN SET NOCOUNT ON; END');
GO
