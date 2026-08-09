SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.MaterialTransaction', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MaterialTransaction
    (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MaterialTransaction PRIMARY KEY,
        TransId INT NULL,
        TransactionType VARCHAR(50) NULL,
        TransactionDate DATETIME NULL,
        MaterialId INT NULL,
        Quantity DECIMAL(18,3) NULL,
        LocationId INT NULL,
        Type VARCHAR(10) NULL,
        Updatedby VARCHAR(20) NULL
    );
END
GO

IF OBJECT_ID('dbo.InventoryLocationMaster', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.InventoryLocationMaster
    (
        LocationId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_InventoryLocationMaster PRIMARY KEY,
        LocationCode VARCHAR(20) NOT NULL,
        LocationName VARCHAR(150) NOT NULL,
        LocationType VARCHAR(50) NOT NULL,
        Address VARCHAR(500) NULL,
        Reference VARCHAR(250) NULL,
        Remarks VARCHAR(500) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_InventoryLocationMaster_IsActive DEFAULT (1),
        EnteredBy VARCHAR(20) NULL,
        EnteredOn DATETIME NOT NULL CONSTRAINT DF_InventoryLocationMaster_EnteredOn DEFAULT (GETDATE()),
        UpdatedBy VARCHAR(20) NULL,
        UpdatedOn DATETIME NULL
    );

    CREATE UNIQUE INDEX UX_InventoryLocationMaster_LocationCode
        ON dbo.InventoryLocationMaster(LocationCode);
END
GO

IF OBJECT_ID('dbo.DeliveryNote', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DeliveryNote
    (
        DeliveryNoteId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DeliveryNote PRIMARY KEY,
        DeliveryNoteNo VARCHAR(30) NOT NULL,
        DeliveryNoteDate DATETIME NOT NULL,
        FromLocationId INT NOT NULL,
        ToLocationId INT NOT NULL,
        ReferenceNo VARCHAR(100) NULL,
        ReferenceDate DATETIME NULL,
        Remarks VARCHAR(500) NULL,
        Status VARCHAR(20) NOT NULL CONSTRAINT DF_DeliveryNote_Status DEFAULT ('PENDING'),
        EnteredBy VARCHAR(20) NULL,
        EnteredOn DATETIME NOT NULL CONSTRAINT DF_DeliveryNote_EnteredOn DEFAULT (GETDATE()),
        ApprovedBy VARCHAR(20) NULL,
        ApprovedOn DATETIME NULL,
        RejectedBy VARCHAR(20) NULL,
        RejectedOn DATETIME NULL,
        RejectionRemarks VARCHAR(500) NULL,
        IsDeleted BIT NOT NULL CONSTRAINT DF_DeliveryNote_IsDeleted DEFAULT (0),
        CONSTRAINT FK_DeliveryNote_FromLocation FOREIGN KEY (FromLocationId) REFERENCES dbo.InventoryLocationMaster(LocationId),
        CONSTRAINT FK_DeliveryNote_ToLocation FOREIGN KEY (ToLocationId) REFERENCES dbo.InventoryLocationMaster(LocationId),
        CONSTRAINT CK_DeliveryNote_Status CHECK (Status IN ('PENDING', 'APPROVED', 'REJECTED')),
        CONSTRAINT CK_DeliveryNote_DifferentLocations CHECK (FromLocationId <> ToLocationId)
    );

    CREATE UNIQUE INDEX UX_DeliveryNote_DeliveryNoteNo
        ON dbo.DeliveryNote(DeliveryNoteNo);
    CREATE INDEX IX_DeliveryNote_StatusDate
        ON dbo.DeliveryNote(Status, DeliveryNoteDate);
END
GO

IF OBJECT_ID('dbo.DeliveryNoteDetail', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DeliveryNoteDetail
    (
        DeliveryNoteDetailId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DeliveryNoteDetail PRIMARY KEY,
        DeliveryNoteId INT NOT NULL,
        MaterialId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL,
        Remarks VARCHAR(250) NULL,
        CONSTRAINT FK_DeliveryNoteDetail_DeliveryNote FOREIGN KEY (DeliveryNoteId) REFERENCES dbo.DeliveryNote(DeliveryNoteId),
        CONSTRAINT CK_DeliveryNoteDetail_Quantity CHECK (Quantity > 0)
    );

    CREATE INDEX IX_DeliveryNoteDetail_DeliveryNoteId
        ON dbo.DeliveryNoteDetail(DeliveryNoteId);
    CREATE INDEX IX_DeliveryNoteDetail_MaterialId
        ON dbo.DeliveryNoteDetail(MaterialId);
END
GO

IF OBJECT_ID('dbo.ReceiptNote', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ReceiptNote
    (
        ReceiptNoteId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ReceiptNote PRIMARY KEY,
        ReceiptNoteNo VARCHAR(30) NOT NULL,
        ReceiptNoteDate DATETIME NOT NULL,
        FromLocationId INT NOT NULL,
        ToLocationId INT NOT NULL,
        ReferenceNo VARCHAR(100) NULL,
        ReferenceDate DATETIME NULL,
        Remarks VARCHAR(500) NULL,
        Status VARCHAR(20) NOT NULL CONSTRAINT DF_ReceiptNote_Status DEFAULT ('PENDING'),
        EnteredBy VARCHAR(20) NULL,
        EnteredOn DATETIME NOT NULL CONSTRAINT DF_ReceiptNote_EnteredOn DEFAULT (GETDATE()),
        ApprovedBy VARCHAR(20) NULL,
        ApprovedOn DATETIME NULL,
        RejectedBy VARCHAR(20) NULL,
        RejectedOn DATETIME NULL,
        RejectionRemarks VARCHAR(500) NULL,
        IsDeleted BIT NOT NULL CONSTRAINT DF_ReceiptNote_IsDeleted DEFAULT (0),
        CONSTRAINT FK_ReceiptNote_FromLocation FOREIGN KEY (FromLocationId) REFERENCES dbo.InventoryLocationMaster(LocationId),
        CONSTRAINT FK_ReceiptNote_ToLocation FOREIGN KEY (ToLocationId) REFERENCES dbo.InventoryLocationMaster(LocationId),
        CONSTRAINT CK_ReceiptNote_Status CHECK (Status IN ('PENDING', 'APPROVED', 'REJECTED')),
        CONSTRAINT CK_ReceiptNote_DifferentLocations CHECK (FromLocationId <> ToLocationId)
    );

    CREATE UNIQUE INDEX UX_ReceiptNote_ReceiptNoteNo
        ON dbo.ReceiptNote(ReceiptNoteNo);
    CREATE INDEX IX_ReceiptNote_StatusDate
        ON dbo.ReceiptNote(Status, ReceiptNoteDate);
END
GO

IF OBJECT_ID('dbo.ReceiptNoteDetail', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ReceiptNoteDetail
    (
        ReceiptNoteDetailId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ReceiptNoteDetail PRIMARY KEY,
        ReceiptNoteId INT NOT NULL,
        MaterialId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL,
        Remarks VARCHAR(250) NULL,
        CONSTRAINT FK_ReceiptNoteDetail_ReceiptNote FOREIGN KEY (ReceiptNoteId) REFERENCES dbo.ReceiptNote(ReceiptNoteId),
        CONSTRAINT CK_ReceiptNoteDetail_Quantity CHECK (Quantity > 0)
    );

    CREATE INDEX IX_ReceiptNoteDetail_ReceiptNoteId
        ON dbo.ReceiptNoteDetail(ReceiptNoteId);
    CREATE INDEX IX_ReceiptNoteDetail_MaterialId
        ON dbo.ReceiptNoteDetail(MaterialId);
END
GO

IF OBJECT_ID('dbo.ProductMaster', 'U') IS NOT NULL
   AND OBJECT_ID('dbo.DeliveryNoteDetail', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_DeliveryNoteDetail_ProductMaster')
   AND EXISTS
   (
       SELECT 1
       FROM sys.indexes i
       INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
       INNER JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
       WHERE i.object_id = OBJECT_ID('dbo.ProductMaster')
         AND i.is_unique = 1
         AND c.name = 'id'
         AND NOT EXISTS
         (
             SELECT 1
             FROM sys.index_columns ic2
             WHERE ic2.object_id = i.object_id
               AND ic2.index_id = i.index_id
               AND ic2.key_ordinal > 1
         )
   )
BEGIN
    ALTER TABLE dbo.DeliveryNoteDetail
        ADD CONSTRAINT FK_DeliveryNoteDetail_ProductMaster FOREIGN KEY (MaterialId) REFERENCES dbo.ProductMaster(id);
END
GO

IF OBJECT_ID('dbo.ProductMaster', 'U') IS NOT NULL
   AND OBJECT_ID('dbo.ReceiptNoteDetail', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ReceiptNoteDetail_ProductMaster')
   AND EXISTS
   (
       SELECT 1
       FROM sys.indexes i
       INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
       INNER JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
       WHERE i.object_id = OBJECT_ID('dbo.ProductMaster')
         AND i.is_unique = 1
         AND c.name = 'id'
         AND NOT EXISTS
         (
             SELECT 1
             FROM sys.index_columns ic2
             WHERE ic2.object_id = i.object_id
               AND ic2.index_id = i.index_id
               AND ic2.key_ordinal > 1
         )
   )
BEGIN
    ALTER TABLE dbo.ReceiptNoteDetail
        ADD CONSTRAINT FK_ReceiptNoteDetail_ProductMaster FOREIGN KEY (MaterialId) REFERENCES dbo.ProductMaster(id);
END
GO

IF OBJECT_ID('dbo.MaterialTransaction', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_MaterialTransaction_DeliveryNote_Post' AND object_id = OBJECT_ID('dbo.MaterialTransaction'))
BEGIN
    CREATE UNIQUE INDEX UX_MaterialTransaction_DeliveryNote_Post
        ON dbo.MaterialTransaction(TransactionType, TransId, MaterialId, LocationId, Type)
        WHERE TransactionType = 'DELIVERY NOTE';
END
GO

IF OBJECT_ID('dbo.MaterialTransaction', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_MaterialTransaction_ReceiptNote_Post' AND object_id = OBJECT_ID('dbo.MaterialTransaction'))
BEGIN
    CREATE UNIQUE INDEX UX_MaterialTransaction_ReceiptNote_Post
        ON dbo.MaterialTransaction(TransactionType, TransId, MaterialId, LocationId, Type)
        WHERE TransactionType = 'RECEIPT NOTE';
END
GO

IF OBJECT_ID('dbo.MaterialTransaction', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MaterialTransaction_StockLookup' AND object_id = OBJECT_ID('dbo.MaterialTransaction'))
BEGIN
    CREATE INDEX IX_MaterialTransaction_StockLookup
        ON dbo.MaterialTransaction(MaterialId, LocationId, Type)
        INCLUDE (Quantity);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.InventoryLocationMaster)
   AND OBJECT_ID('dbo.Location', 'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.InventoryLocationMaster (LocationCode, LocationName, LocationType, EnteredBy, EnteredOn)
    SELECT 'LOC' + RIGHT('00000' + CAST(LocationID AS VARCHAR(5)), 5),
           LocationName,
           'BRANCH',
           'SYSTEM',
           GETDATE()
    FROM dbo.Location
    WHERE ISNULL(LocationName, '') <> '';
END
GO
