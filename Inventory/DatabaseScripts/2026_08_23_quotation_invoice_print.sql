IF OBJECT_ID('dbo.Proc_QuotationInvoicePrint', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_QuotationInvoicePrint AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_QuotationInvoicePrint
    @id varchar(100),
    @company varchar(100)
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #temptable
    (
        Tin varchar(100),
        Cst varchar(100),
        GST varchar(100),
        Billno varchar(100),
        Party varchar(100),
        CustomerName varchar(100),
        Customeraddress1 varchar(max),
        Customercity varchar(max),
        Customerphone varchar(100),
        CustomerTin varchar(100),
        Total varchar(100),
        Address1 varchar(max),
        Address2 varchar(max),
        City varchar(max),
        District varchar(max),
        State varchar(max),
        NumberInwords varchar(max),
        Phoneno varchar(20),
        salesdate varchar(100),
        Billtype varchar(25),
        GSTText varchar(max)
    );

    INSERT INTO #temptable
    SELECT
        (SELECT tin FROM dbo.ReportAddressDetails WHERE CompanyName = @company) AS Tin,
        (SELECT cst FROM dbo.ReportAddressDetails WHERE CompanyName = @company) AS Cst,
        (SELECT GST FROM dbo.ReportAddressDetails WHERE CompanyName = @company) AS GST,
        qh.Quotationid AS BillNo,
        c.Tin AS Party,
        UPPER(ISNULL(qh.customername, c.Name)) AS CustomerName,
        REPLACE((UPPER(ISNULL(c.Address1, '')) + CASE WHEN NULLIF(UPPER(ISNULL(c.Address2, '')), '') IS NULL THEN '' ELSE ',' + UPPER(c.Address2) END), ',,', ',') AS Customeraddress1,
        ISNULL(qh.City, c.City) AS Customercity,
        c.Phone AS Customerphone,
        c.Tin AS CustomerTin,
        ROUND(SUM(CONVERT(decimal(18, 2), ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(qd.Amount, '')), 0))), 0) AS Total,
        c.Address1,
        c.Address2,
        c.City AS CITY,
        ds.DISTRICT,
        (st.State + '-' + c.Pincode) AS State,
        (SELECT dbo.IndianCurrencyInWords1(SUM(CONVERT(decimal(18, 2), ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(qd.Amount, '')), 0)))) + '  Only. )') AS NumberInwords,
        c.Phone,
        CONVERT(varchar(11), qh.[date], 103) AS salesdate,
        'QUOTATION' AS BillType,
        '' AS GSTText
    FROM dbo.QuotationHeader qh
    INNER JOIN dbo.QuotationDetails qd ON qh.Quotationid = qd.Quotationid
    LEFT JOIN dbo.ProductMaster p ON CONVERT(varchar(50), p.id) = CONVERT(varchar(50), qd.Productid)
    LEFT JOIN dbo.Customers c ON CONVERT(varchar(50), c.CustomerID) = CONVERT(varchar(50), qh.Customerid)
    LEFT JOIN dbo.state st ON st.State = c.State
    LEFT JOIN dbo.district ds ON ds.DistrictId = c.District
    WHERE qh.Quotationid = @id
    GROUP BY qh.Quotationid, qh.customername, qh.City, qh.[date], c.Address1, c.Address2, c.City, ds.DISTRICT, st.State, c.State,
             c.Pincode, c.Phone, c.Name, c.Tin;

    ALTER TABLE #temptable ADD Others numeric(18, 2);
    UPDATE #temptable SET Others = 0;

    ALTER TABLE #temptable ADD Valdata1 varchar(max), Valdata2 varchar(max);
    UPDATE #temptable SET Valdata1 = '', Valdata2 = '';

    ALTER TABLE #temptable ADD
        CompanyName nvarchar(50),
        DoorNo nvarchar(50),
        Address11 nvarchar(50),
        Address22 nvarchar(50),
        City1 nvarchar(50),
        State1 nvarchar(50),
        Country1 nvarchar(50),
        Pincode nvarchar(50),
        Phone1 nvarchar(50),
        Phone2 nvarchar(50),
        Fax nvarchar(50),
        EmailId nvarchar(max),
        BillHeader nvarchar(max);

    UPDATE #temptable
    SET BillHeader = 'QUOTATION';

    UPDATE #temptable
    SET CompanyName = REPLACE(UPPER(r.CompanyName), 'R.R', 'R R'),
        DoorNo = r.DoorNo,
        Address11 = r.Address1,
        Address22 = r.Address2,
        City1 = (r.City + '-' + r.Pincode),
        State1 = r.State,
        Country1 = r.Country,
        Pincode = r.Pincode,
        Phone1 = r.Phone1 + ',' + REPLACE(r.Phone2, '0427-', ''),
        Phone2 = r.Phone2,
        Fax = r.Fax,
        EmailId = r.EmailId
    FROM dbo.ReportAddressDetails r
    WHERE r.IsDeleted = 0
      AND r.CompanyName = @company;

    ALTER TABLE #temptable ADD LessAmount varchar(100), GrandTotal varchar(100), ReturnSales nvarchar(250);

    UPDATE #temptable
    SET LessAmount = '0',
        GrandTotal = Total,
        ReturnSales = NULL;

    DECLARE @FinalGrandTotal decimal(18, 2);
    SELECT @FinalGrandTotal = SUM(CONVERT(decimal(18, 2), ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(qd.Amount, '')), 0)))
    FROM dbo.QuotationDetails qd
    WHERE qd.Quotationid = @id;

    UPDATE #temptable
    SET GrandTotal = ROUND(ISNULL(@FinalGrandTotal, 0), 0),
        NumberInwords = (SELECT dbo.IndianCurrencyInWords1(SUM(CONVERT(decimal(18, 2), ISNULL(@FinalGrandTotal, 0)))) + ' Only. )');

    SELECT
        CAST(ROW_NUMBER() OVER (ORDER BY qe.sino) AS varchar) AS Sno,
        p.DisplayName AS ItemName,
        p.HSN,
        CONVERT(numeric(18, 2), ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(qe.Rate, '')), 0)) AS Rate,
        CAST(ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(qe.Rate, '')), 0) / (1 + ISNULL(COALESCE(qe.GSTAtQuote, TRY_CONVERT(decimal(18, 2), p.GST)), 0) / 100.0) AS numeric(18, 2)) AS TRate,
        qe.Quantity AS Quantity,
        CAST(NULL AS numeric(18, 2)) AS T5,
        CAST(NULL AS numeric(18, 2)) AS T12,
        CAST(NULL AS numeric(18, 2)) AS T18,
        CAST(NULL AS numeric(18, 2)) AS T28,
        ROUND(CONVERT(decimal(16, 2), ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(qe.Amount, '')), 0)), 0) AS Amount,
        ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(qe.Quantity, '')), 0)
            * CAST(ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(qe.Rate, '')), 0) / (1 + ISNULL(COALESCE(qe.GSTAtQuote, TRY_CONVERT(decimal(18, 2), p.GST)), 0) / 100.0) AS numeric(18, 2)) AS TAXABLEVALUE,
        ISNULL(COALESCE(qe.GSTAtQuote, TRY_CONVERT(decimal(18, 2), p.GST)), 0) AS Vat,
        u.UOM AS UOM
    INTO #Products
    FROM dbo.QuotationDetails qe
    LEFT JOIN dbo.ProductMaster p ON CONVERT(varchar(50), qe.Productid) = CONVERT(varchar(50), p.id)
    LEFT JOIN dbo.UOM u ON CONVERT(varchar(50), u.Uomid) = CONVERT(varchar(50), p.UOM)
    WHERE qe.Quotationid = @id;

    UPDATE #Products SET T5 = TRate * TRY_CONVERT(decimal(18, 2), Quantity) * CAST(VAT AS numeric(18, 2)) / 100.0 WHERE VAT = 5;
    UPDATE #Products SET T12 = TRate * TRY_CONVERT(decimal(18, 2), Quantity) * CAST(VAT AS numeric(18, 2)) / 100.0 WHERE VAT = 12;
    UPDATE #Products SET T18 = TRate * TRY_CONVERT(decimal(18, 2), Quantity) * CAST(VAT AS numeric(18, 2)) / 100.0 WHERE VAT = 18;
    UPDATE #Products SET T28 = TRate * TRY_CONVERT(decimal(18, 2), Quantity) * CAST(VAT AS numeric(18, 2)) / 100.0 WHERE VAT = 28;

    ALTER TABLE #Products ADD TAX numeric(18, 2);
    UPDATE #Products SET TAX = ISNULL(T5, 0) + ISNULL(T12, 0) + ISNULL(T18, 0) + ISNULL(T28, 0);

    ALTER TABLE #temptable ADD
        TOTAL5 numeric(18, 2),
        TOTAL12 numeric(18, 2),
        TOTAL18 numeric(18, 2),
        TOTAL28 numeric(18, 2),
        TOTAL5TAXABLE numeric(18, 2),
        TOTAL12TAXABLE numeric(18, 2),
        TOTAL18TAXABLE numeric(18, 2),
        TOTAL28TAXABLE numeric(18, 2),
        TOTALTAXABLE numeric(18, 2),
        TOTALGST numeric(18, 2),
        CGST numeric(18, 2),
        SGST numeric(18, 2),
        IGST numeric(18, 2),
        TOTALAMOUNT numeric(18, 2),
        NETAMOUNT numeric(18, 2);

    UPDATE #temptable SET TOTAL5 = (SELECT SUM(T5) FROM #Products);
    UPDATE #temptable SET TOTAL12 = (SELECT SUM(T12) FROM #Products);
    UPDATE #temptable SET TOTAL18 = (SELECT SUM(T18) FROM #Products);
    UPDATE #temptable SET TOTAL28 = (SELECT SUM(T28) FROM #Products);

    UPDATE #temptable SET TOTAL5TAXABLE = (SELECT SUM(TAXABLEVALUE) FROM #Products WHERE VAT = 5);
    UPDATE #temptable SET TOTAL12TAXABLE = (SELECT SUM(TAXABLEVALUE) FROM #Products WHERE VAT = 12);
    UPDATE #temptable SET TOTAL18TAXABLE = (SELECT SUM(TAXABLEVALUE) FROM #Products WHERE VAT = 18);
    UPDATE #temptable SET TOTAL28TAXABLE = (SELECT SUM(TAXABLEVALUE) FROM #Products WHERE VAT = 28);
    UPDATE #temptable SET TOTALTAXABLE = (SELECT SUM(TAXABLEVALUE) FROM #Products);

    UPDATE #temptable
    SET TOTALGST = ISNULL(TOTAL5, 0) + ISNULL(TOTAL12, 0) + ISNULL(TOTAL18, 0) + ISNULL(TOTAL28, 0),
        CGST = (ISNULL(TOTAL5, 0) + ISNULL(TOTAL12, 0) + ISNULL(TOTAL18, 0) + ISNULL(TOTAL28, 0)) / 2,
        SGST = (ISNULL(TOTAL5, 0) + ISNULL(TOTAL12, 0) + ISNULL(TOTAL18, 0) + ISNULL(TOTAL28, 0)) / 2,
        IGST = 0;

    UPDATE #temptable SET TOTALAMOUNT = ROUND(TOTALTAXABLE + TOTALGST, 0);
    UPDATE #temptable SET NETAMOUNT = ROUND(CAST(Total AS numeric(18, 2)) - ISNULL(LESSAMOUNT, 0), 0);

    SELECT *,
        DOORNO + ADDRESS11 AS COMPANYADDRESS,
        'ACFPR4217D' AS PAN,
        'HDFC BANK' AS Bank,
        '50200001121289' AS ACNO,
        'HDFC0001281' AS IFSC,
        'BRINDHAVAN ROAD,SALEM' AS BRANCH,
        CAST(ISNULL(Total5, 0) / 2 AS numeric(18, 2)) AS CGST5,
        CAST(ISNULL(Total5, 0) / 2 AS numeric(18, 2)) AS SGSTT5,
        CAST(ISNULL(Total12, 0) / 2 AS numeric(18, 2)) AS CGST12,
        CAST(ISNULL(Total12, 0) / 2 AS numeric(18, 2)) AS SGSTT12,
        CAST(ISNULL(Total18, 0) / 2 AS numeric(18, 2)) AS CGST18,
        CAST(ISNULL(Total18, 0) / 2 AS numeric(18, 2)) AS SGSTT18,
        CAST(ISNULL(Total28, 0) / 2 AS numeric(18, 2)) AS CGST28,
        CAST(ISNULL(Total28, 0) / 2 AS numeric(18, 2)) AS SGSTT28,
        CAST(0 AS numeric(18, 2)) AS IGST5,
        CAST(0 AS numeric(18, 2)) AS IGST12,
        CAST(0 AS numeric(18, 2)) AS IGST18,
        CAST(0 AS numeric(18, 2)) AS IGST28,
        CAST(0 AS numeric(18, 2)) AS RoundOff,
        CAST(0 AS numeric(18, 2)) AS DiscountPer
    FROM #temptable;

    UPDATE #Products SET Quantity = LEFT(Quantity + ' ' + ISNULL(UOM, ''), 10);
    SELECT * FROM #Products;
END;
GO
