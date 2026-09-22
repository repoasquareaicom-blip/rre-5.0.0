/*
  Pending Issued rack-wise stock save.
  Keeps dbo.SaveIssued and all legacy Checkout/PDI/Delivery procedures unchanged.
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

IF TYPE_ID('dbo.PendingIssuedRackDetailType') IS NULL
BEGIN
    CREATE TYPE dbo.PendingIssuedRackDetailType AS TABLE
    (
        ProductId INT NOT NULL,
        RackId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.SaveIssued_RackWise', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.SaveIssued_RackWise AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.SaveIssued_RackWise
(
    @receiveid VARCHAR(50) = NULL,
    @QuotationDetails dbo.Issuedtype READONLY,
    @RackDetails dbo.PendingIssuedRackDetailType READONLY,
    @Updatedby VARCHAR(10) = NULL,
    @outid VARCHAR(100) OUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @issuedId VARCHAR(40);
    DECLARE @maxid INT;
    DECLARE @value VARCHAR(15);
    DECLARE @pendingDifferentCount INT;

    SET @outid = NULL;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @receiveid IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.Issuedreceived WITH (UPDLOCK, HOLDLOCK) WHERE ReceivedID = @receiveid AND Status = 'Approved')
        BEGIN
            RAISERROR('Invalid or completed pending issued order. Please refresh and try again.', 16, 1);
        END

        CREATE TABLE #IssueDetails
        (
            ProductId INT NOT NULL,
            Quantity DECIMAL(18,3) NOT NULL,
            Location VARCHAR(50) NULL
        );

        INSERT INTO #IssueDetails (ProductId, Quantity, Location)
        SELECT
            CONVERT(INT, Productid),
            SUM(CONVERT(DECIMAL(18,3), Quantity)),
            MAX(Location)
        FROM @QuotationDetails
        GROUP BY CONVERT(INT, Productid);

        IF NOT EXISTS (SELECT 1 FROM #IssueDetails)
        BEGIN
            RAISERROR('Issue quantity is required.', 16, 1);
        END

        IF EXISTS (SELECT 1 FROM #IssueDetails WHERE Quantity <= 0)
        BEGIN
            RAISERROR('Issue quantity should be greater than zero.', 16, 1);
        END

        IF EXISTS (
            SELECT 1
            FROM #IssueDetails id
            WHERE NOT EXISTS (
                SELECT 1
                FROM dbo.Issuedreceiveddetails ird WITH (UPDLOCK, HOLDLOCK)
                WHERE ird.ReceivedID = @receiveid
                  AND CONVERT(INT, ird.Productid) = id.ProductId
            )
        )
        BEGIN
            RAISERROR('Invalid product found for pending issued order.', 16, 1);
        END

        IF EXISTS (
            SELECT 1
            FROM #IssueDetails id
            INNER JOIN dbo.Issuedreceiveddetails ird WITH (UPDLOCK, HOLDLOCK)
                ON ird.ReceivedID = @receiveid
               AND CONVERT(INT, ird.Productid) = id.ProductId
            WHERE id.Quantity > ISNULL(ird.Receiveqty, 0) - ISNULL(ird.IssueQty, 0)
        )
        BEGIN
            RAISERROR('Issue quantity exceeds pending quantity. Please refresh and try again.', 16, 1);
        END

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
            WHERE NOT EXISTS (
                SELECT 1
                FROM #IssueDetails id
                WHERE id.ProductId = rd.ProductId
            )
        )
        BEGIN
            RAISERROR('Rack allocation includes a product that is not being issued.', 16, 1);
        END

        IF EXISTS (
            SELECT 1
            FROM #IssueDetails id
            LEFT JOIN (
                SELECT ProductId, SUM(Quantity) AS RackQuantity
                FROM @RackDetails
                GROUP BY ProductId
            ) rd
                ON rd.ProductId = id.ProductId
            WHERE ISNULL(rd.RackQuantity, 0) <> id.Quantity
        )
        BEGIN
            RAISERROR('Rack allocation total must equal issue quantity for every product.', 16, 1);
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
               OR lm.LocationId IS NULL
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

        CREATE TABLE #RackRequest
        (
            ProductId INT NOT NULL,
            RackId INT NOT NULL,
            Quantity DECIMAL(18,3) NOT NULL
        );

        INSERT INTO #RackRequest (ProductId, RackId, Quantity)
        SELECT ProductId, RackId, SUM(Quantity)
        FROM @RackDetails
        GROUP BY ProductId, RackId;

        CREATE TABLE #RackAvailable
        (
            ProductId INT NOT NULL,
            RackId INT NOT NULL,
            AvailableQuantity DECIMAL(18,3) NOT NULL
        );

        INSERT INTO #RackAvailable (ProductId, RackId, AvailableQuantity)
        SELECT
            rr.ProductId,
            rr.RackId,
            ISNULL(SUM(
                CASE
                    WHEN UPPER(ISNULL(mt.Type, '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
                    WHEN UPPER(ISNULL(mt.Type, '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
                    ELSE 0
                END
            ), 0)
        FROM #RackRequest rr
        LEFT JOIN dbo.MaterialTranscation mt WITH (UPDLOCK, HOLDLOCK)
            ON mt.MaterailId = rr.ProductId
           AND mt.RackId = rr.RackId
        GROUP BY rr.ProductId, rr.RackId;

        IF EXISTS (
            SELECT 1
            FROM #RackRequest rr
            INNER JOIN #RackAvailable ra
                ON ra.ProductId = rr.ProductId
               AND ra.RackId = rr.RackId
            WHERE rr.Quantity > ra.AvailableQuantity
        )
        BEGIN
            RAISERROR('Insufficient rack stock for one or more products. Please refresh and try again.', 16, 1);
        END

        SET @maxid = (
            SELECT COUNT(*)
            FROM dbo.Issued WITH (UPDLOCK, HOLDLOCK)
            WHERE CONVERT(VARCHAR(10), Updatedon, 101) = CONVERT(VARCHAR(10), GETDATE(), 101)
        );
        SET @value = @maxid + 1;
        SET @issuedId = 'RR IS ' + @value + '/' + CAST(YEAR(GETDATE()) AS VARCHAR(10))
            + CAST(RIGHT('0' + RTRIM(MONTH(GETDATE())), 2) AS VARCHAR(10))
            + CAST(RIGHT('0' + RTRIM(DAY(GETDATE())), 2) AS VARCHAR(10));

        INSERT INTO dbo.Issued
        (
            Issuedid,
            ReceivedID,
            CustomerName,
            customerid,
            RefNo,
            Updatedon,
            UpdatedBy,
            Status
        )
        SELECT
            @issuedId,
            ReceivedID,
            CustomerName,
            customerid,
            RefNo,
            GETDATE(),
            @Updatedby,
            'ISuued Completed'
        FROM dbo.Issuedreceived
        WHERE ReceivedID = @receiveid;

        INSERT INTO dbo.Issueddetails
        (
            issuedid,
            ReceivedID,
            Productid,
            Location,
            Quantity
        )
        SELECT
            @issuedId,
            @receiveid,
            ProductId,
            Location,
            Quantity
        FROM #IssueDetails;

        DECLARE @TProduct TABLE(Productid VARCHAR(100), Quantity DECIMAL(18,3));

        INSERT INTO @TProduct
        SELECT id.Productid, SUM(Quantity)
        FROM dbo.Issueddetails id
        INNER JOIN dbo.Issuedreceived ir
            ON id.Receivedid = ir.ReceivedID
        WHERE id.Receivedid = @receiveid
        GROUP BY id.Productid;

        UPDATE ar
        SET IssueQty = sa.Quantity
        FROM dbo.Issuedreceiveddetails ar
        INNER JOIN @TProduct sa
            ON ar.Productid = sa.Productid
        WHERE ar.Receivedid = @receiveid;

        SET @pendingDifferentCount = (
            SELECT COUNT(*)
            FROM dbo.Issuedreceiveddetails
            WHERE Receiveqty <> IssueQty
              AND ReceivedID = @receiveid
        );

        IF (@pendingDifferentCount = 0)
        BEGIN
            UPDATE dbo.Issuedreceived
            SET Status = 'ISuued Completed'
            WHERE ReceivedID = @receiveid;
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
            @receiveid,
            'IssuedReceived',
            GETDATE(),
            rr.ProductId,
            rr.Quantity,
            rm.LocationId,
            rr.RackId,
            'OUT',
            CASE WHEN ISNUMERIC(@Updatedby) = 1 THEN CONVERT(INT, @Updatedby) ELSE NULL END
        FROM #RackRequest rr
        INNER JOIN dbo.RackMaster rm
            ON rm.RackId = rr.RackId;

        SET @outid = @issuedId;

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

IF NOT EXISTS (SELECT 1 FROM dbo.DbDeploymentHistory WHERE ScriptName = '2026_09_15_pending_issued_rackwise.sql')
BEGIN
    INSERT INTO dbo.DbDeploymentHistory (ScriptName, AppliedBy)
    VALUES ('2026_09_15_pending_issued_rackwise.sql', SUSER_SNAME());
END
GO
