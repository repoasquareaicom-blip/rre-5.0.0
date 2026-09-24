/*
  SalesReport payment amounts stay on the company sales header.
  RR Electrical -> Sales
  RR Pipes -> SalesPipes
  RR Traders -> SalesTraders
  Existing bills are left null. Run once on each database.
*/
DECLARE @tables TABLE (name sysname);
INSERT INTO @tables (name) VALUES ('Sales'), ('SalesPipes'), ('SalesTraders');

DECLARE @table sysname;
DECLARE table_cursor CURSOR LOCAL FAST_FORWARD FOR SELECT name FROM @tables;
OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @table;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF COL_LENGTH('dbo.' + @table, 'CashAmount') IS NULL
        EXEC('ALTER TABLE dbo.' + @table + ' ADD CashAmount decimal(18, 2) NULL');
    IF COL_LENGTH('dbo.' + @table, 'CardAmount') IS NULL
        EXEC('ALTER TABLE dbo.' + @table + ' ADD CardAmount decimal(18, 2) NULL');
    IF COL_LENGTH('dbo.' + @table, 'UpiAmount') IS NULL
        EXEC('ALTER TABLE dbo.' + @table + ' ADD UpiAmount decimal(18, 2) NULL');
    IF COL_LENGTH('dbo.' + @table, 'CardBank') IS NULL
        EXEC('ALTER TABLE dbo.' + @table + ' ADD CardBank varchar(200) NULL');
    IF COL_LENGTH('dbo.' + @table, 'CardNumber') IS NULL
        EXEC('ALTER TABLE dbo.' + @table + ' ADD CardNumber varchar(30) NULL');
    IF COL_LENGTH('dbo.' + @table, 'CardTransId') IS NULL
        EXEC('ALTER TABLE dbo.' + @table + ' ADD CardTransId varchar(50) NULL');
    IF COL_LENGTH('dbo.' + @table, 'UpiReference') IS NULL
        EXEC('ALTER TABLE dbo.' + @table + ' ADD UpiReference varchar(30) NULL');
    FETCH NEXT FROM table_cursor INTO @table;
END
CLOSE table_cursor;
DEALLOCATE table_cursor;
GO
