/*
  Delivery Note / Receipt Note rack-wise stock.

  Products are rack-wise when ProductMaster.RackWiseStockMovement = 1 (the same product-level switch used by PDI).
  - Rack-wise products need a rack allocation on the note (DeliveryNoteRackAllocation / ReceiptNoteRackAllocation)
    on racks mapped to the product in ProductRackMapping. The allocated total must equal the line quantity.
  - Other products keep the existing non-rack posting (MaterialTranscation LocationId '6', no RackId).
  - Saving a note posts no stock. Proc_DeliveryNote_Approve / Proc_ReceiptNote_Approve validate, post
    MaterialTranscation (DELIVERY NOTE OUT / RECEIPT NOTE IN, dated with the note date) and mark the note
    APPROVED in one transaction. Delivery approval re-checks source rack stock under UPDLOCK/HOLDLOCK.
  - Approved notes, their details and their rack allocations are locked by triggers.
  - Existing notes are not changed. Approved notes without allocations stay as they are; a pending note with a
    rack-wise product must be given a rack allocation before it can be approved.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF DB_NAME() IN ('master', 'model', 'msdb', 'tempdb')
BEGIN
    RAISERROR('Refusing to change a system database.', 16, 1);
    RETURN;
END
GO

IF OBJECT_ID('dbo.DeliveryNoteRackAllocation', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DeliveryNoteRackAllocation
    (
        DeliveryNoteRackAllocationId int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_DeliveryNoteRackAllocation PRIMARY KEY,
        DeliveryNoteId int NOT NULL
            CONSTRAINT FK_DeliveryNoteRackAllocation_DeliveryNote REFERENCES dbo.DeliveryNote (DeliveryNoteId),
        DeliveryNoteDetailId int NOT NULL
            CONSTRAINT FK_DeliveryNoteRackAllocation_Detail REFERENCES dbo.DeliveryNoteDetail (DeliveryNoteDetailId),
        ProductId int NOT NULL,
        RackId int NOT NULL
            CONSTRAINT FK_DeliveryNoteRackAllocation_Rack REFERENCES dbo.RackMaster (RackId),
        Quantity decimal(18, 3) NOT NULL CONSTRAINT CK_DeliveryNoteRackAllocation_Quantity CHECK (Quantity > 0),
        CreatedBy varchar(20) NULL,
        CreatedOn datetime NOT NULL CONSTRAINT DF_DeliveryNoteRackAllocation_CreatedOn DEFAULT (GETDATE()),
        CONSTRAINT UQ_DeliveryNoteRackAllocation_DetailRack UNIQUE (DeliveryNoteDetailId, RackId)
    );
    CREATE INDEX IX_DeliveryNoteRackAllocation_Note ON dbo.DeliveryNoteRackAllocation (DeliveryNoteId);
    CREATE INDEX IX_DeliveryNoteRackAllocation_ProductRack ON dbo.DeliveryNoteRackAllocation (ProductId, RackId);
    CREATE INDEX IX_DeliveryNoteRackAllocation_Rack ON dbo.DeliveryNoteRackAllocation (RackId);
END
GO

IF OBJECT_ID('dbo.ReceiptNoteRackAllocation', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ReceiptNoteRackAllocation
    (
        ReceiptNoteRackAllocationId int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ReceiptNoteRackAllocation PRIMARY KEY,
        ReceiptNoteId int NOT NULL
            CONSTRAINT FK_ReceiptNoteRackAllocation_ReceiptNote REFERENCES dbo.ReceiptNote (ReceiptNoteId),
        ReceiptNoteDetailId int NOT NULL
            CONSTRAINT FK_ReceiptNoteRackAllocation_Detail REFERENCES dbo.ReceiptNoteDetail (ReceiptNoteDetailId),
        ProductId int NOT NULL,
        RackId int NOT NULL
            CONSTRAINT FK_ReceiptNoteRackAllocation_Rack REFERENCES dbo.RackMaster (RackId),
        Quantity decimal(18, 3) NOT NULL CONSTRAINT CK_ReceiptNoteRackAllocation_Quantity CHECK (Quantity > 0),
        CreatedBy varchar(20) NULL,
        CreatedOn datetime NOT NULL CONSTRAINT DF_ReceiptNoteRackAllocation_CreatedOn DEFAULT (GETDATE()),
        CONSTRAINT UQ_ReceiptNoteRackAllocation_DetailRack UNIQUE (ReceiptNoteDetailId, RackId)
    );
    CREATE INDEX IX_ReceiptNoteRackAllocation_Note ON dbo.ReceiptNoteRackAllocation (ReceiptNoteId);
    CREATE INDEX IX_ReceiptNoteRackAllocation_ProductRack ON dbo.ReceiptNoteRackAllocation (ProductId, RackId);
    CREATE INDEX IX_ReceiptNoteRackAllocation_Rack ON dbo.ReceiptNoteRackAllocation (RackId);
END
GO

/* Approved notes are permanent: header, details and rack allocations cannot change after approval. */
IF OBJECT_ID('dbo.trg_DeliveryNote_LockApproved', 'TR') IS NOT NULL DROP TRIGGER dbo.trg_DeliveryNote_LockApproved;
GO
CREATE TRIGGER dbo.trg_DeliveryNote_LockApproved ON dbo.DeliveryNote AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted WHERE UPPER(ISNULL(Status, '')) = 'APPROVED')
    BEGIN
        RAISERROR('An approved Delivery Note cannot be changed.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

IF OBJECT_ID('dbo.trg_DeliveryNoteDetail_LockApproved', 'TR') IS NOT NULL DROP TRIGGER dbo.trg_DeliveryNoteDetail_LockApproved;
GO
CREATE TRIGGER dbo.trg_DeliveryNoteDetail_LockApproved ON dbo.DeliveryNoteDetail AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM (SELECT DeliveryNoteId FROM inserted UNION SELECT DeliveryNoteId FROM deleted) x
        INNER JOIN dbo.DeliveryNote h ON h.DeliveryNoteId = x.DeliveryNoteId
        WHERE UPPER(ISNULL(h.Status, '')) = 'APPROVED')
    BEGIN
        RAISERROR('Products of an approved Delivery Note cannot be changed.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

IF OBJECT_ID('dbo.trg_DeliveryNoteRackAllocation_LockApproved', 'TR') IS NOT NULL DROP TRIGGER dbo.trg_DeliveryNoteRackAllocation_LockApproved;
GO
CREATE TRIGGER dbo.trg_DeliveryNoteRackAllocation_LockApproved ON dbo.DeliveryNoteRackAllocation AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM (SELECT DeliveryNoteId FROM inserted UNION SELECT DeliveryNoteId FROM deleted) x
        INNER JOIN dbo.DeliveryNote h ON h.DeliveryNoteId = x.DeliveryNoteId
        WHERE UPPER(ISNULL(h.Status, '')) = 'APPROVED')
    BEGIN
        RAISERROR('Rack allocation of an approved Delivery Note cannot be changed.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

IF OBJECT_ID('dbo.trg_ReceiptNote_LockApproved', 'TR') IS NOT NULL DROP TRIGGER dbo.trg_ReceiptNote_LockApproved;
GO
CREATE TRIGGER dbo.trg_ReceiptNote_LockApproved ON dbo.ReceiptNote AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted WHERE UPPER(ISNULL(Status, '')) = 'APPROVED')
    BEGIN
        RAISERROR('An approved Receipt Note cannot be changed.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

IF OBJECT_ID('dbo.trg_ReceiptNoteDetail_LockApproved', 'TR') IS NOT NULL DROP TRIGGER dbo.trg_ReceiptNoteDetail_LockApproved;
GO
CREATE TRIGGER dbo.trg_ReceiptNoteDetail_LockApproved ON dbo.ReceiptNoteDetail AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM (SELECT ReceiptNoteId FROM inserted UNION SELECT ReceiptNoteId FROM deleted) x
        INNER JOIN dbo.ReceiptNote h ON h.ReceiptNoteId = x.ReceiptNoteId
        WHERE UPPER(ISNULL(h.Status, '')) = 'APPROVED')
    BEGIN
        RAISERROR('Products of an approved Receipt Note cannot be changed.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

IF OBJECT_ID('dbo.trg_ReceiptNoteRackAllocation_LockApproved', 'TR') IS NOT NULL DROP TRIGGER dbo.trg_ReceiptNoteRackAllocation_LockApproved;
GO
CREATE TRIGGER dbo.trg_ReceiptNoteRackAllocation_LockApproved ON dbo.ReceiptNoteRackAllocation AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM (SELECT ReceiptNoteId FROM inserted UNION SELECT ReceiptNoteId FROM deleted) x
        INNER JOIN dbo.ReceiptNote h ON h.ReceiptNoteId = x.ReceiptNoteId
        WHERE UPPER(ISNULL(h.Status, '')) = 'APPROVED')
    BEGIN
        RAISERROR('Rack allocation of an approved Receipt Note cannot be changed.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

/*
  Raises one error listing every rack allocation problem of a Delivery Note.
  @ForApproval = 1 reads source rack stock WITH (UPDLOCK, HOLDLOCK) so the caller's transaction keeps it until commit.
*/
IF OBJECT_ID('dbo.Proc_DeliveryNote_ValidateRackAllocation', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DeliveryNote_ValidateRackAllocation AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DeliveryNote_ValidateRackAllocation
    @DeliveryNoteId int,
    @ForApproval bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @lines TABLE
    (
        DetailId int PRIMARY KEY,
        ProductId int NOT NULL,
        ProductName nvarchar(300) NOT NULL,
        Quantity decimal(18, 3) NOT NULL,
        RackWise bit NOT NULL,
        Allocated decimal(18, 3) NOT NULL,
        AllocationCount int NOT NULL
    );
    DECLARE @problems TABLE (Seq int IDENTITY(1, 1) PRIMARY KEY, Message nvarchar(400) NOT NULL);
    DECLARE @stock TABLE (ProductId int NOT NULL, RackId int NOT NULL, Required decimal(18, 3) NOT NULL, Available decimal(18, 3) NULL,
        PRIMARY KEY (ProductId, RackId));

    INSERT INTO @lines (DetailId, ProductId, ProductName, Quantity, RackWise, Allocated, AllocationCount)
    SELECT d.DeliveryNoteDetailId, d.MaterialId,
        ISNULL(NULLIF(p.DisplayName, ''), ISNULL(p.ItemName, CONVERT(varchar(20), d.MaterialId))),
        d.Quantity, ISNULL(p.RackWiseStockMovement, 0), ISNULL(a.Quantity, 0), ISNULL(a.Cnt, 0)
    FROM dbo.DeliveryNoteDetail d
    LEFT JOIN dbo.ProductMaster p ON p.id = d.MaterialId
    OUTER APPLY (SELECT SUM(r.Quantity) AS Quantity, COUNT(*) AS Cnt
                 FROM dbo.DeliveryNoteRackAllocation r WHERE r.DeliveryNoteDetailId = d.DeliveryNoteDetailId) a
    WHERE d.DeliveryNoteId = @DeliveryNoteId;

    INSERT INTO @problems (Message)
    SELECT N'No rack is allocated for this rack-wise product. Please allocate a rack first. (' + l.ProductName + N')'
    FROM @lines l
    WHERE l.RackWise = 1
      AND NOT EXISTS (SELECT 1 FROM dbo.ProductRackMapping prm
                      INNER JOIN dbo.RackMaster rm ON rm.RackId = prm.RackId
                      INNER JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId
                      WHERE prm.ProductId = l.ProductId AND ISNULL(rm.IsActive, 0) = 1 AND ISNULL(lm.IsActive, 0) = 1)
    ORDER BY l.DetailId;

    INSERT INTO @problems (Message)
    SELECT N'Rack allocation for ' + l.ProductName + N' is ' + CONVERT(nvarchar(30), l.Allocated) + N' of '
        + CONVERT(nvarchar(30), l.Quantity) + N'. Allocate the full quantity to racks.'
    FROM @lines l
    WHERE l.RackWise = 1 AND l.Allocated <> l.Quantity
      AND EXISTS (SELECT 1 FROM dbo.ProductRackMapping prm WHERE prm.ProductId = l.ProductId)
    ORDER BY l.DetailId;

    INSERT INTO @problems (Message)
    SELECT l.ProductName + N' is not a rack-wise product but has a rack allocation. Open the Delivery Note and save it again.'
    FROM @lines l
    WHERE l.RackWise = 0 AND l.AllocationCount > 0
    ORDER BY l.DetailId;

    INSERT INTO @problems (Message)
    SELECT N'Rack ' + ISNULL(rm.RackCaption, CONVERT(varchar(20), r.RackId)) + N' is not an active rack mapped to ' + l.ProductName + N'.'
    FROM dbo.DeliveryNoteRackAllocation r
    INNER JOIN @lines l ON l.DetailId = r.DeliveryNoteDetailId
    LEFT JOIN dbo.RackMaster rm ON rm.RackId = r.RackId
    LEFT JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId
    WHERE l.RackWise = 1
      AND (r.ProductId <> l.ProductId
           OR ISNULL(rm.IsActive, 0) = 0
           OR ISNULL(lm.IsActive, 0) = 0
           OR NOT EXISTS (SELECT 1 FROM dbo.ProductRackMapping prm WHERE prm.ProductId = l.ProductId AND prm.RackId = r.RackId));

    INSERT INTO @stock (ProductId, RackId, Required)
    SELECT r.ProductId, r.RackId, SUM(r.Quantity)
    FROM dbo.DeliveryNoteRackAllocation r
    INNER JOIN @lines l ON l.DetailId = r.DeliveryNoteDetailId
    WHERE l.RackWise = 1
    GROUP BY r.ProductId, r.RackId;

    IF @ForApproval = 1
        UPDATE s SET Available = ISNULL(x.Available, 0)
        FROM @stock s
        OUTER APPLY (SELECT SUM(CASE WHEN UPPER(ISNULL(mt.Type, '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
                                     WHEN UPPER(ISNULL(mt.Type, '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0) ELSE 0 END) AS Available
                     FROM dbo.MaterialTranscation mt WITH (UPDLOCK, HOLDLOCK)
                     WHERE mt.MaterailId = s.ProductId AND mt.RackId = s.RackId) x;
    ELSE
        UPDATE s SET Available = ISNULL(x.Available, 0)
        FROM @stock s
        OUTER APPLY (SELECT SUM(CASE WHEN UPPER(ISNULL(mt.Type, '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
                                     WHEN UPPER(ISNULL(mt.Type, '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0) ELSE 0 END) AS Available
                     FROM dbo.MaterialTranscation mt
                     WHERE mt.MaterailId = s.ProductId AND mt.RackId = s.RackId) x;

    INSERT INTO @problems (Message)
    SELECT N'Insufficient stock in Rack ' + ISNULL(rm.RackCaption, CONVERT(varchar(20), s.RackId))
        + N' for ' + ISNULL(NULLIF(p.DisplayName, ''), ISNULL(p.ItemName, CONVERT(varchar(20), s.ProductId)))
        + N'. Required: ' + CONVERT(nvarchar(30), s.Required) + N' Available: ' + CONVERT(nvarchar(30), CAST(s.Available AS decimal(18, 3))) + N'.'
    FROM @stock s
    LEFT JOIN dbo.RackMaster rm ON rm.RackId = s.RackId
    LEFT JOIN dbo.ProductMaster p ON p.id = s.ProductId
    WHERE s.Available < s.Required;

    IF EXISTS (SELECT 1 FROM @problems)
    BEGIN
        DECLARE @message nvarchar(2000);
        SET @message = LEFT(STUFF((SELECT NCHAR(10) + Message FROM @problems ORDER BY Seq
                                   FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 1, N''), 2000);
        RAISERROR('%s', 16, 1, @message);
    END
END
GO

IF OBJECT_ID('dbo.Proc_ReceiptNote_ValidateRackAllocation', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_ReceiptNote_ValidateRackAllocation AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_ReceiptNote_ValidateRackAllocation
    @ReceiptNoteId int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @lines TABLE
    (
        DetailId int PRIMARY KEY,
        ProductId int NOT NULL,
        ProductName nvarchar(300) NOT NULL,
        Quantity decimal(18, 3) NOT NULL,
        RackWise bit NOT NULL,
        Allocated decimal(18, 3) NOT NULL,
        AllocationCount int NOT NULL
    );
    DECLARE @problems TABLE (Seq int IDENTITY(1, 1) PRIMARY KEY, Message nvarchar(400) NOT NULL);

    INSERT INTO @lines (DetailId, ProductId, ProductName, Quantity, RackWise, Allocated, AllocationCount)
    SELECT d.ReceiptNoteDetailId, d.MaterialId,
        ISNULL(NULLIF(p.DisplayName, ''), ISNULL(p.ItemName, CONVERT(varchar(20), d.MaterialId))),
        d.Quantity, ISNULL(p.RackWiseStockMovement, 0), ISNULL(a.Quantity, 0), ISNULL(a.Cnt, 0)
    FROM dbo.ReceiptNoteDetail d
    LEFT JOIN dbo.ProductMaster p ON p.id = d.MaterialId
    OUTER APPLY (SELECT SUM(r.Quantity) AS Quantity, COUNT(*) AS Cnt
                 FROM dbo.ReceiptNoteRackAllocation r WHERE r.ReceiptNoteDetailId = d.ReceiptNoteDetailId) a
    WHERE d.ReceiptNoteId = @ReceiptNoteId;

    INSERT INTO @problems (Message)
    SELECT N'No rack is allocated for this rack-wise product. Please allocate a rack first and then continue the Receipt Note. ('
        + l.ProductName + N')'
    FROM @lines l
    WHERE l.RackWise = 1
      AND NOT EXISTS (SELECT 1 FROM dbo.ProductRackMapping prm
                      INNER JOIN dbo.RackMaster rm ON rm.RackId = prm.RackId
                      INNER JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId
                      WHERE prm.ProductId = l.ProductId AND ISNULL(rm.IsActive, 0) = 1 AND ISNULL(lm.IsActive, 0) = 1)
    ORDER BY l.DetailId;

    INSERT INTO @problems (Message)
    SELECT N'Rack allocation for ' + l.ProductName + N' is ' + CONVERT(nvarchar(30), l.Allocated) + N' of '
        + CONVERT(nvarchar(30), l.Quantity) + N'. Allocate the full received quantity to racks.'
    FROM @lines l
    WHERE l.RackWise = 1 AND l.Allocated <> l.Quantity
      AND EXISTS (SELECT 1 FROM dbo.ProductRackMapping prm WHERE prm.ProductId = l.ProductId)
    ORDER BY l.DetailId;

    INSERT INTO @problems (Message)
    SELECT l.ProductName + N' is not a rack-wise product but has a rack allocation. Open the Receipt Note and save it again.'
    FROM @lines l
    WHERE l.RackWise = 0 AND l.AllocationCount > 0
    ORDER BY l.DetailId;

    INSERT INTO @problems (Message)
    SELECT N'Rack ' + ISNULL(rm.RackCaption, CONVERT(varchar(20), r.RackId)) + N' is not an active rack mapped to ' + l.ProductName + N'.'
    FROM dbo.ReceiptNoteRackAllocation r
    INNER JOIN @lines l ON l.DetailId = r.ReceiptNoteDetailId
    LEFT JOIN dbo.RackMaster rm ON rm.RackId = r.RackId
    LEFT JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId
    WHERE l.RackWise = 1
      AND (r.ProductId <> l.ProductId
           OR ISNULL(rm.IsActive, 0) = 0
           OR ISNULL(lm.IsActive, 0) = 0
           OR NOT EXISTS (SELECT 1 FROM dbo.ProductRackMapping prm WHERE prm.ProductId = l.ProductId AND prm.RackId = r.RackId));

    IF EXISTS (SELECT 1 FROM @problems)
    BEGIN
        DECLARE @message nvarchar(2000);
        SET @message = LEFT(STUFF((SELECT NCHAR(10) + Message FROM @problems ORDER BY Seq
                                   FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 1, N''), 2000);
        RAISERROR('%s', 16, 1, @message);
    END
END
GO

IF OBJECT_ID('dbo.Proc_DeliveryNote_Approve', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DeliveryNote_Approve AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DeliveryNote_Approve
    @DeliveryNoteId int,
    @ApprovedBy varchar(20),
    @UserId int
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @status varchar(20);
    DECLARE @noteNo varchar(30);
    DECLARE @noteDate datetime;

    BEGIN TRY
        BEGIN TRAN;

        SELECT @status = Status, @noteNo = DeliveryNoteNo, @noteDate = DATEADD(day, DATEDIFF(day, 0, DeliveryNoteDate), 0)
        FROM dbo.DeliveryNote WITH (UPDLOCK, HOLDLOCK)
        WHERE DeliveryNoteId = @DeliveryNoteId AND ISNULL(IsDeleted, 0) = 0;

        IF @status IS NULL
            RAISERROR('Delivery Note not found.', 16, 1);
        IF UPPER(@status) = 'APPROVED'
            RAISERROR('Delivery Note already approved.', 16, 1);
        IF UPPER(@status) <> 'PENDING'
            RAISERROR('Delivery Note is %s and cannot be approved.', 16, 1, @status);
        IF NOT EXISTS (SELECT 1 FROM dbo.DeliveryNoteDetail WHERE DeliveryNoteId = @DeliveryNoteId)
            RAISERROR('Delivery Note has no products.', 16, 1);
        IF EXISTS (SELECT 1 FROM dbo.MaterialTranscation WHERE TranscationType = 'DELIVERY NOTE' AND TransId = @noteNo)
            RAISERROR('Stock is already posted for Delivery Note %s.', 16, 1, @noteNo);

        EXEC dbo.Proc_DeliveryNote_ValidateRackAllocation @DeliveryNoteId = @DeliveryNoteId, @ForApproval = 1;

        INSERT INTO dbo.MaterialTranscation (TransId, TranscationType, TranscationDate, MaterailId, Quantity, LocationId, Type, Updatedby, RackId)
        SELECT @noteNo, 'DELIVERY NOTE', @noteDate, r.ProductId, r.Quantity, CONVERT(varchar(100), rm.LocationId), 'OUT', @UserId, r.RackId
        FROM dbo.DeliveryNoteRackAllocation r
        INNER JOIN dbo.DeliveryNoteDetail d ON d.DeliveryNoteDetailId = r.DeliveryNoteDetailId
        INNER JOIN dbo.ProductMaster p ON p.id = d.MaterialId AND ISNULL(p.RackWiseStockMovement, 0) = 1
        INNER JOIN dbo.RackMaster rm ON rm.RackId = r.RackId
        WHERE r.DeliveryNoteId = @DeliveryNoteId
        ORDER BY d.DeliveryNoteDetailId, r.DeliveryNoteRackAllocationId;

        INSERT INTO dbo.MaterialTranscation (TransId, TranscationType, TranscationDate, MaterailId, Quantity, LocationId, Type, Updatedby, RackId)
        SELECT @noteNo, 'DELIVERY NOTE', @noteDate, d.MaterialId, d.Quantity, '6', 'OUT', @UserId, NULL
        FROM dbo.DeliveryNoteDetail d
        LEFT JOIN dbo.ProductMaster p ON p.id = d.MaterialId
        WHERE d.DeliveryNoteId = @DeliveryNoteId AND ISNULL(p.RackWiseStockMovement, 0) = 0
        ORDER BY d.DeliveryNoteDetailId;

        UPDATE dbo.DeliveryNote
        SET Status = 'APPROVED', ApprovedBy = @ApprovedBy, ApprovedOn = GETDATE()
        WHERE DeliveryNoteId = @DeliveryNoteId AND Status = 'PENDING';
        IF @@ROWCOUNT <> 1
            RAISERROR('Delivery Note could not be approved.', 16, 1);

        COMMIT TRAN;
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

IF OBJECT_ID('dbo.Proc_ReceiptNote_Approve', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_ReceiptNote_Approve AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_ReceiptNote_Approve
    @ReceiptNoteId int,
    @ApprovedBy varchar(20),
    @UserId int
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @status varchar(20);
    DECLARE @noteNo varchar(30);
    DECLARE @noteDate datetime;

    BEGIN TRY
        BEGIN TRAN;

        SELECT @status = Status, @noteNo = ReceiptNoteNo, @noteDate = DATEADD(day, DATEDIFF(day, 0, ReceiptNoteDate), 0)
        FROM dbo.ReceiptNote WITH (UPDLOCK, HOLDLOCK)
        WHERE ReceiptNoteId = @ReceiptNoteId AND ISNULL(IsDeleted, 0) = 0;

        IF @status IS NULL
            RAISERROR('Receipt Note not found.', 16, 1);
        IF UPPER(@status) = 'APPROVED'
            RAISERROR('Receipt Note already approved.', 16, 1);
        IF UPPER(@status) <> 'PENDING'
            RAISERROR('Receipt Note is %s and cannot be approved.', 16, 1, @status);
        IF NOT EXISTS (SELECT 1 FROM dbo.ReceiptNoteDetail WHERE ReceiptNoteId = @ReceiptNoteId)
            RAISERROR('Receipt Note has no products.', 16, 1);
        IF EXISTS (SELECT 1 FROM dbo.MaterialTranscation WHERE TranscationType = 'RECEIPT NOTE' AND TransId = @noteNo)
            RAISERROR('Stock is already posted for Receipt Note %s.', 16, 1, @noteNo);

        EXEC dbo.Proc_ReceiptNote_ValidateRackAllocation @ReceiptNoteId = @ReceiptNoteId;

        INSERT INTO dbo.MaterialTranscation (TransId, TranscationType, TranscationDate, MaterailId, Quantity, LocationId, Type, Updatedby, RackId)
        SELECT @noteNo, 'RECEIPT NOTE', @noteDate, r.ProductId, r.Quantity, CONVERT(varchar(100), rm.LocationId), 'IN', @UserId, r.RackId
        FROM dbo.ReceiptNoteRackAllocation r
        INNER JOIN dbo.ReceiptNoteDetail d ON d.ReceiptNoteDetailId = r.ReceiptNoteDetailId
        INNER JOIN dbo.ProductMaster p ON p.id = d.MaterialId AND ISNULL(p.RackWiseStockMovement, 0) = 1
        INNER JOIN dbo.RackMaster rm ON rm.RackId = r.RackId
        WHERE r.ReceiptNoteId = @ReceiptNoteId
        ORDER BY d.ReceiptNoteDetailId, r.ReceiptNoteRackAllocationId;

        INSERT INTO dbo.MaterialTranscation (TransId, TranscationType, TranscationDate, MaterailId, Quantity, LocationId, Type, Updatedby, RackId)
        SELECT @noteNo, 'RECEIPT NOTE', @noteDate, d.MaterialId, d.Quantity, '6', 'IN', @UserId, NULL
        FROM dbo.ReceiptNoteDetail d
        LEFT JOIN dbo.ProductMaster p ON p.id = d.MaterialId
        WHERE d.ReceiptNoteId = @ReceiptNoteId AND ISNULL(p.RackWiseStockMovement, 0) = 0
        ORDER BY d.ReceiptNoteDetailId;

        UPDATE dbo.ReceiptNote
        SET Status = 'APPROVED', ApprovedBy = @ApprovedBy, ApprovedOn = GETDATE()
        WHERE ReceiptNoteId = @ReceiptNoteId AND Status = 'PENDING';
        IF @@ROWCOUNT <> 1
            RAISERROR('Receipt Note could not be approved.', 16, 1);

        COMMIT TRAN;
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
