/*
  SalesReport payment modes need more than 10 characters.
  Sales.Paymentmode is already varchar(200).
  Only the three procedures this screen calls are changed.
*/
DECLARE @name sysname;
DECLARE @def nvarchar(max);
DECLARE @procs TABLE (name sysname);
INSERT INTO @procs (name) VALUES ('SaveQuotationsales_1'), ('SaveQuotationsales_2'), ('SaveQuotationsales_3');

DECLARE proc_cursor CURSOR LOCAL FAST_FORWARD FOR SELECT name FROM @procs;
OPEN proc_cursor;
FETCH NEXT FROM proc_cursor INTO @name;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @def = OBJECT_DEFINITION(OBJECT_ID('dbo.' + @name));
    SET @def = STUFF(@def, CHARINDEX('CREATE', @def), 6, 'ALTER');
    SET @def = REPLACE(@def, '@paymentmode varchar(10)', '@paymentmode varchar(50)');
    EXEC(@def);
    FETCH NEXT FROM proc_cursor INTO @name;
END
CLOSE proc_cursor;
DEALLOCATE proc_cursor;
GO
