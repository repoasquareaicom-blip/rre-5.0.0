/*
  SalesBillNew card and UPI saves call SaveQuotationsales_Direct_GST.
  Combined modes such as UPI and Cash need more than 10 characters.
*/
DECLARE @def nvarchar(max);
SET @def = OBJECT_DEFINITION(OBJECT_ID('dbo.SaveQuotationsales_Direct_GST'));
IF CHARINDEX('@paymentmode varchar(10)', @def) = 0
    RAISERROR('paymentmode parameter was not found on SaveQuotationsales_Direct_GST', 16, 1);
SET @def = STUFF(@def, CHARINDEX('CREATE', @def), 6, 'ALTER');
SET @def = REPLACE(@def, '@paymentmode varchar(10)', '@paymentmode varchar(50)');
EXEC(@def);
GO
