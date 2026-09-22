/*
  Product Master rack-wise stock movement flag deployment script.

  This only adds/saves/loads the product configuration flag. It does not
  implement rack-wise stock movement or stock calculation.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.ProductMaster', 'RackWiseStockMovement') IS NULL
BEGIN
    ALTER TABLE dbo.ProductMaster
    ADD RackWiseStockMovement BIT NOT NULL
        CONSTRAINT DF_ProductMaster_RackWiseStockMovement DEFAULT (0);
END
GO

IF OBJECT_ID('dbo.sp_Product_RackWiseStockMovement_Get', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_Product_RackWiseStockMovement_Get AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_Product_RackWiseStockMovement_Get
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ISNULL(RackWiseStockMovement, 0) AS RackWiseStockMovement
    FROM dbo.ProductMaster
    WHERE id = @ProductId;
END
GO

IF OBJECT_ID('dbo.sp_Product_RackWiseStockMovement_Save', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_Product_RackWiseStockMovement_Save AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_Product_RackWiseStockMovement_Save
    @ProductId INT,
    @RackWiseStockMovement BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ProductMaster
    SET RackWiseStockMovement = ISNULL(@RackWiseStockMovement, 0)
    WHERE id = @ProductId;
END
GO
