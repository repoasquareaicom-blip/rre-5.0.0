/*
  Sales invoice heading is always TAX INVOICE.
  BillHeader keeps the selected payment mode for the Payment Terms line.
  Electrical, Pipes, and Traders print procedures are all updated.
*/
DECLARE @name sysname;
DECLARE @def nvarchar(max);
DECLARE @procs TABLE (name sysname);
INSERT INTO @procs (name) VALUES ('SalesBillPrint_1'), ('SalesPipesBillPrint_1'), ('SalesTradersBillPrint_1');

DECLARE proc_cursor CURSOR LOCAL FAST_FORWARD FOR SELECT name FROM @procs;
OPEN proc_cursor;
FETCH NEXT FROM proc_cursor INTO @name;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @def = OBJECT_DEFINITION(OBJECT_ID('dbo.' + @name));
    IF CHARINDEX('case s.Paymentmode when ''CreditBill'' then ''CREDIT BILL'' else ''CASH BILL'' end BillType', @def) = 0
        RAISERROR('Bill type text was not found in %s', 16, 1, @name);
    IF CHARINDEX('set @Flag=''CASH BILL''', @def) = 0
        RAISERROR('Cash bill flag was not found in %s', 16, 1, @name);
    IF CHARINDEX('set @Flag=''CREDIT BILL''', @def) = 0
        RAISERROR('Credit bill flag was not found in %s', 16, 1, @name);

    SET @def = STUFF(@def, CHARINDEX('CREATE', @def), 6, 'ALTER');
    SET @def = REPLACE(@def, 'case s.Paymentmode when ''CreditBill'' then ''CREDIT BILL'' else ''CASH BILL'' end BillType', '''TAX INVOICE'' BillType');
    SET @def = REPLACE(@def, 'set @Flag=''CASH BILL''', 'set @Flag=@Flag');
    SET @def = REPLACE(@def, 'set @Flag=''CREDIT BILL''', 'set @Flag=''Credit Bill''');
    EXEC(@def);
    FETCH NEXT FROM proc_cursor INTO @name;
END
CLOSE proc_cursor;
DEALLOCATE proc_cursor;
GO
