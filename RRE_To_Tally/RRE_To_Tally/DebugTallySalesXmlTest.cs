using System.Xml.Linq;

namespace RRE_To_Tally;

internal static class DebugTallySalesXmlTest
{
    public static void WriteLocalGstSplitTest(string outputPath)
    {
        SalesExportInvoice invoice = new SalesExportInvoice
        {
            SalesId = "DEBUG-LOCAL-GST",
            ReferenceId = "DEBUG-LOCAL-GST",
            Date = new DateTime(2026, 5, 1),
            CustomerId = "1",
            CustomerName = "Debug Tamil Nadu Customer",
            CustomerLedgerName = "Debug Tamil Nadu Customer",
            CustomerCity = "Namakkal",
            CustomerState = "Tamil Nadu",
            GstRegistrationType = "Unregistered/Consumer",
            PaymentMode = "Cash",
            ItemCount = 3,
            TaxableAmount = 6000m,
            Cgst = 415m,
            Sgst = 415m,
            Igst = 0m,
            CalculatedTotal = 6830m,
            StoredGrandTotal = 6830m,
            IsInterstate = false
        };

        invoice.Items.Add(MakeItem("Product A", 1000m, 1m, 1000m, 5m, 25m, 25m));
        invoice.Items.Add(MakeItem("Product B", 2000m, 1m, 2000m, 12m, 120m, 120m));
        invoice.Items.Add(MakeItem("Product C", 3000m, 1m, 3000m, 18m, 270m, 270m));

        TallyExportOptions options = new TallyExportOptions
        {
            CompanySettings = new TallyCompanySettings
            {
                CompanyState = "Tamil Nadu",
                MainGodownName = "Main Location",
                PrimaryBatchName = "Primary Batch",
                CGSTLedgerName = "CGST",
                SGSTLedgerName = "SGST",
                IGSTLedgerName = "IGST",
                RoundOffLedgerName = "Round Off",
                CashLedgerName = "CASH",
                SalesLedgerPrefix = "Sales"
            }
        };

        if (File.Exists(outputPath)) File.Delete(outputPath);
        new TallyXmlWriter().WriteSalesXml(outputPath, new List<SalesExportInvoice> { invoice }, options);
        XDocument document = XDocument.Load(outputPath);
        int cgstLedgerBlocks = document.Descendants("LEDGERENTRIES.LIST").Count(e => (string?)e.Element("LEDGERNAME") == "CGST");
        int sgstLedgerBlocks = document.Descendants("LEDGERENTRIES.LIST").Count(e => (string?)e.Element("LEDGERNAME") == "SGST");
        bool hasCgstAmount = document.Descendants("LEDGERENTRIES.LIST").Any(e => (string?)e.Element("LEDGERNAME") == "CGST" && (string?)e.Element("AMOUNT") == "415.00" && (string?)e.Element("VATEXPAMOUNT") == "415.00");
        bool hasSgstAmount = document.Descendants("LEDGERENTRIES.LIST").Any(e => (string?)e.Element("LEDGERNAME") == "SGST" && (string?)e.Element("AMOUNT") == "415.00" && (string?)e.Element("VATEXPAMOUNT") == "415.00");
        bool hasPartyAmount = document.Descendants("LEDGERENTRIES.LIST").Any(e => (string?)e.Element("ISPARTYLEDGER") == "Yes" && (string?)e.Element("AMOUNT") == "-6830.00");
        bool hasEffectiveDate = document.Descendants("VOUCHER").Any(e => (string?)e.Element("EFFECTIVEDATE") == "20260401");
        if (cgstLedgerBlocks != 1 || sgstLedgerBlocks != 1 || !hasCgstAmount || !hasSgstAmount || !hasPartyAmount || !hasEffectiveDate)
        {
            throw new InvalidOperationException("Debug GST split XML validation failed.");
        }
    }

    public static void WriteMastersStructureTest(string outputPath)
    {
        TallyCompanySettings settings = new TallyCompanySettings
        {
            CompanyState = "Tamil Nadu",
            MainGodownName = "Main Location",
            PrimaryBatchName = "Primary Batch",
            MasterApplicableFrom = "20260401",
            CGSTLedgerName = "CGST",
            SGSTLedgerName = "SGST",
            IGSTLedgerName = "IGST",
            RoundOffLedgerName = "Round Off",
            SalesLedgerPrefix = "Sales"
        };
        TallyExportPackage package = new TallyExportPackage();
        package.Units.Add(new UnitMasterExport { Name = "NOS" });
        package.StockGroups.Add(new StockGroupMasterExport { Name = "FROZEN & MILKY MIST&VEG PRODUCTS" });
        package.Ledgers.Add(new LedgerMasterExport { Name = "SALES 5%", Parent = "Sales Accounts" });
        package.Ledgers.Add(new LedgerMasterExport { Name = "CGST", Parent = "Duties & Taxes", TaxType = "GST", DutyHead = "Central Tax" });
        package.Ledgers.Add(new LedgerMasterExport { Name = "SGST", Parent = "Duties & Taxes", TaxType = "GST", DutyHead = "State Tax" });
        package.Ledgers.Add(new LedgerMasterExport { Name = "Round Off", Parent = "Indirect Expenses" });
        CustomerMasterExport customer = new CustomerMasterExport
        {
            Name = "THE FALOODA SHOP & TANDOORI TRIBES",
            State = "Tamil Nadu",
            Pincode = "637001",
            GstRegistrationType = "Regular",
            Gstin = "33ARWPA5570E1ZS"
        };
        customer.AddressLines.Add("56, S.P. PUDUR");
        customer.AddressLines.Add("KARUPPANNAN STREET,");
        customer.AddressLines.Add("PARAMATHI ROAD");
        customer.AddressLines.Add("NAMAKKAL");
        package.Customers.Add(customer);
        package.Products.Add(new ProductMasterExport
        {
            Name = "SLICE CHEESE 765 GM",
            StockGroupName = "FROZEN & MILKY MIST&VEG PRODUCTS",
            BaseUnit = "NOS",
            Hsn = "04063000",
            GstRate = 5m,
            OpeningBalance = 13m
        });

        if (File.Exists(outputPath)) File.Delete(outputPath);
        new TallyXmlWriter().WriteMastersXml(outputPath, package, new TallyExportOptions { CompanySettings = settings });
        XDocument document = XDocument.Load(outputPath);
        XElement stockItem = document.Descendants("STOCKITEM").Single();
        XElement ledger = document.Descendants("LEDGER").Single(e => (string?)e.Attribute("NAME") == "THE FALOODA SHOP & TANDOORI TRIBES");
        bool hasStockShape = (string?)stockItem.Attribute("ACTION") == "Create" &&
            (string?)stockItem.Element("GSTDETAILS.LIST")?.Element("STATEWISEDETAILS.LIST")?.Elements("RATEDETAILS.LIST").FirstOrDefault(e => (string?)e.Element("GSTRATEDUTYHEAD") == "Central Tax")?.Element("GSTRATE") == "2.5" &&
            (string?)stockItem.Element("GSTDETAILS.LIST")?.Element("STATEWISEDETAILS.LIST")?.Elements("RATEDETAILS.LIST").FirstOrDefault(e => (string?)e.Element("GSTRATEDUTYHEAD") == "State Tax")?.Element("GSTRATE") == "2.5" &&
            (string?)stockItem.Element("GSTDETAILS.LIST")?.Element("STATEWISEDETAILS.LIST")?.Elements("RATEDETAILS.LIST").FirstOrDefault(e => (string?)e.Element("GSTRATEDUTYHEAD") == "Integrated Tax")?.Element("GSTRATE") == "5" &&
            (string?)stockItem.Element("HSNDETAILS.LIST")?.Element("HSNCODE") == "04063000" &&
            stockItem.Elements("BATCHALLOCATIONS.LIST").Any() == false;
        bool hasLedgerShape = (string?)ledger.Attribute("ACTION") == "Create" &&
            (string?)ledger.Element("GSTIN") == "33ARWPA5570E1ZS" &&
            (string?)ledger.Element("PARENT") == "Sundry Debtors";
        if (!hasStockShape || !hasLedgerShape)
        {
            throw new InvalidOperationException("Debug masters XML validation failed.");
        }
    }

    private static SalesExportItem MakeItem(string name, decimal rate, decimal quantity, decimal amount, decimal gstRate, decimal cgst, decimal sgst)
    {
        return new SalesExportItem
        {
            ProductId = name,
            ItemCode = name,
            ProductName = name,
            ProductTallyName = name,
            StockGroupName = "RRE PRODUCTS",
            Uom = "NOS",
            Hsn = "0000",
            Rate = rate,
            Quantity = quantity,
            TaxableAmount = amount,
            GstRate = gstRate,
            CgstAmount = cgst,
            SgstAmount = sgst,
            SalesLedgerName = TallyNameHelper.GetSalesLedgerName(gstRate, "Sales")
        };
    }
}
