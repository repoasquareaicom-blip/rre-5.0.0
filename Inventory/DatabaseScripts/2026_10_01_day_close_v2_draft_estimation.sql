/*
  Day Close V2: draft auto-save and Estimation excluded from Goods Movement.

  Adds:
    dbo.DayCloseV2Draft                 one row per business date, user-entered values only
    dbo.DayCloseV2DraftDenomination     opening and physical counts per face value
    dbo.DayCloseV2DraftDenominationRow  table type for the save procedure
    dbo.Proc_DayCloseV2_SaveDraft
    dbo.Proc_DayCloseV2_GetDraft

  Changes:
    dbo.Proc_DayCloseV2_GetGoodsSummary  ignores MaterialTranscation rows with TranscationType 'Estimation'.
                                         Report only; no MaterialTranscation row is changed.

  Does not change DayCloseV2, DayCloseV2PaymentSummary, DayCloseV2Denomination,
  DayCloseV2DenominationLine, DayCloseV2Transaction, Proc_DayCloseV2_Submit or any completed close.
  Safe to run more than once.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.DayCloseV2Draft', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DayCloseV2Draft
    (
        BusinessDate date NOT NULL CONSTRAINT PK_DayCloseV2Draft PRIMARY KEY,
        OpeningCash decimal(18, 2) NULL,
        ActualUpi decimal(18, 2) NOT NULL CONSTRAINT DF_DayCloseV2Draft_ActualUpi DEFAULT (0),
        ActualCard decimal(18, 2) NOT NULL CONSTRAINT DF_DayCloseV2Draft_ActualCard DEFAULT (0),
        Remarks nvarchar(500) NULL,
        CreatedBy int NULL,
        CreatedOn datetime NOT NULL CONSTRAINT DF_DayCloseV2Draft_CreatedOn DEFAULT (GETDATE()),
        UpdatedBy int NULL,
        UpdatedOn datetime NOT NULL CONSTRAINT DF_DayCloseV2Draft_UpdatedOn DEFAULT (GETDATE())
    );
END
GO

IF OBJECT_ID('dbo.DayCloseV2DraftDenomination', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DayCloseV2DraftDenomination
    (
        BusinessDate date NOT NULL,
        FaceValue int NOT NULL,
        OpeningQty int NOT NULL,
        PhysicalQty int NOT NULL,
        CONSTRAINT PK_DayCloseV2DraftDenomination PRIMARY KEY (BusinessDate, FaceValue),
        CONSTRAINT FK_DayCloseV2DraftDenomination_Draft FOREIGN KEY (BusinessDate) REFERENCES dbo.DayCloseV2Draft (BusinessDate)
    );
END
GO

IF TYPE_ID('dbo.DayCloseV2DraftDenominationRow') IS NULL
BEGIN
    CREATE TYPE dbo.DayCloseV2DraftDenominationRow AS TABLE
    (
        FaceValue int NOT NULL PRIMARY KEY,
        OpeningQty int NOT NULL,
        PhysicalQty int NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_SaveDraft', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_SaveDraft AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_SaveDraft
(
    @BusinessDate date,
    @OpeningCash decimal(18, 2) = NULL,
    @ActualUpi decimal(18, 2),
    @ActualCard decimal(18, 2),
    @Remarks nvarchar(500) = NULL,
    @UpdatedBy int = NULL,
    @Denominations dbo.DayCloseV2DraftDenominationRow READONLY,
    @Result int OUT,
    @Message nvarchar(400) OUT,
    @SavedOn datetime = NULL OUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Result = 0;
    SET @Message = '';
    SET @SavedOn = NULL;

    IF EXISTS (SELECT 1 FROM @Denominations WHERE OpeningQty < 0 OR PhysicalQty < 0)
    BEGIN
        SET @Message = 'Denomination counts cannot be negative.';
        RETURN;
    END

    DECLARE @now datetime;
    SET @now = GETDATE();

    BEGIN TRANSACTION;

    IF EXISTS (SELECT 1 FROM dbo.DayCloseV2 WITH (UPDLOCK, HOLDLOCK) WHERE BusinessDate = @BusinessDate AND Status = 'Completed')
    BEGIN
        COMMIT TRANSACTION;
        SET @Message = 'This business date is already closed. The draft was not saved.';
        RETURN;
    END

    UPDATE dbo.DayCloseV2Draft WITH (UPDLOCK, HOLDLOCK)
    SET OpeningCash = @OpeningCash,
        ActualUpi = ISNULL(@ActualUpi, 0),
        ActualCard = ISNULL(@ActualCard, 0),
        Remarks = @Remarks,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = @now
    WHERE BusinessDate = @BusinessDate;

    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO dbo.DayCloseV2Draft (BusinessDate, OpeningCash, ActualUpi, ActualCard, Remarks, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn)
        VALUES (@BusinessDate, @OpeningCash, ISNULL(@ActualUpi, 0), ISNULL(@ActualCard, 0), @Remarks, @UpdatedBy, @now, @UpdatedBy, @now);
    END

    DELETE FROM dbo.DayCloseV2DraftDenomination WHERE BusinessDate = @BusinessDate;

    INSERT INTO dbo.DayCloseV2DraftDenomination (BusinessDate, FaceValue, OpeningQty, PhysicalQty)
    SELECT @BusinessDate, FaceValue, OpeningQty, PhysicalQty
    FROM @Denominations;

    COMMIT TRANSACTION;

    SET @Result = 1;
    SET @SavedOn = @now;
    SET @Message = 'Draft saved.';
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetDraft', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetDraft AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetDraft
(
    @BusinessDate date
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.BusinessDate,
        d.OpeningCash,
        d.ActualUpi,
        d.ActualCard,
        d.Remarks,
        d.UpdatedBy,
        d.UpdatedOn,
        COALESCE(NULLIF(LTRIM(RTRIM(u.UserFullName)), ''), NULLIF(LTRIM(RTRIM(u.UserName)), ''), CONVERT(varchar(20), d.UpdatedBy)) AS UpdatedByName
    FROM dbo.DayCloseV2Draft d
    LEFT JOIN dbo.Users u ON u.UserId = d.UpdatedBy
    WHERE d.BusinessDate = @BusinessDate;

    SELECT FaceValue, OpeningQty, PhysicalQty
    FROM dbo.DayCloseV2DraftDenomination
    WHERE BusinessDate = @BusinessDate
    ORDER BY FaceValue DESC;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetGoodsSummary', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetGoodsSummary AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetGoodsSummary
(
    @BusinessDate date
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @start datetime;
    DECLARE @end datetime;
    SET @start = @BusinessDate;
    SET @end = DATEADD(day, 1, @BusinessDate);

    CREATE TABLE #moves
    (
        ProductId int NULL,
        Category nvarchar(60) NOT NULL,
        GoodsIn decimal(18, 3) NOT NULL,
        GoodsOut decimal(18, 3) NOT NULL
    );

    INSERT INTO #moves (ProductId, Category, GoodsIn, GoodsOut)
    SELECT
        mt.MaterailId,
        dbo.fn_DayCloseV2_GoodsCategory(mt.TranscationType),
        CASE WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'IN' THEN ISNULL(mt.Quantity, 0) ELSE 0 END,
        CASE WHEN UPPER(LTRIM(RTRIM(mt.Type))) = 'OUT' THEN ISNULL(mt.Quantity, 0) ELSE 0 END
    FROM dbo.MaterialTranscation mt
    WHERE mt.TranscationDate >= @start
      AND mt.TranscationDate < @end
      AND UPPER(LTRIM(RTRIM(ISNULL(mt.TranscationType, '')))) <> 'ESTIMATION';

    SELECT
        m.ProductId,
        ISNULL(pm.ItemCode, '') AS ProductCode,
        ISNULL(pm.DisplayName, '') AS ProductName,
        m.Category,
        SUM(m.GoodsIn) AS GoodsIn,
        SUM(m.GoodsOut) AS GoodsOut,
        SUM(m.GoodsIn) - SUM(m.GoodsOut) AS Net
    FROM #moves m
    LEFT JOIN dbo.ProductMaster pm ON pm.id = m.ProductId
    GROUP BY m.ProductId, pm.ItemCode, pm.DisplayName, m.Category
    ORDER BY m.Category, pm.DisplayName;

    SELECT
        Category,
        SUM(GoodsIn) AS GoodsIn,
        SUM(GoodsOut) AS GoodsOut,
        SUM(GoodsIn) - SUM(GoodsOut) AS Net,
        COUNT(DISTINCT ProductId) AS Products
    FROM #moves
    GROUP BY Category
    ORDER BY Category;
END
GO
