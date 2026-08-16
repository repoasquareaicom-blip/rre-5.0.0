using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace RRE_To_Tally;

public sealed class TallyMinimalMasterXmlWriter
{
    private static readonly XmlWriterSettings Settings = new XmlWriterSettings
    {
        Indent = true,
        Encoding = new UTF8Encoding(false),
        OmitXmlDeclaration = false,
        NewLineHandling = NewLineHandling.Entitize
    };

    public void WriteMastersXml(string finalPath, TallyExportPackage package, TallyCompanySettings settings)
    {
        string tempPath = finalPath + ".tmp";
        if (File.Exists(tempPath)) File.Delete(tempPath);

        using (XmlWriter writer = XmlWriter.Create(tempPath, Settings))
        {
            WriteEnvelope(writer, delegate
            {
                foreach (UnitMasterExport unit in package.Units) WriteUnit(writer, unit);
                foreach (StockGroupMasterExport group in package.StockGroups) WriteStockGroup(writer, group);
                foreach (CustomerMasterExport customer in package.Customers) WriteCustomerLedger(writer, customer);
                foreach (LedgerMasterExport ledger in package.Ledgers) WriteLedger(writer, ledger);
                foreach (ProductMasterExport product in package.Products) WriteStockItem(writer, product, settings);
            });
        }

        ValidateMastersXml(tempPath);

        if (File.Exists(finalPath)) throw new IOException("Export file already exists: " + finalPath);
        File.Move(tempPath, finalPath);
    }

    private static void WriteEnvelope(XmlWriter writer, Action writeData)
    {
        writer.WriteStartElement("ENVELOPE");
        writer.WriteStartElement("HEADER");
        writer.WriteElementString("TALLYREQUEST", "Import Data");
        writer.WriteEndElement();
        writer.WriteStartElement("BODY");
        writer.WriteStartElement("IMPORTDATA");
        writer.WriteStartElement("REQUESTDESC");
        writer.WriteElementString("REPORTNAME", "All Masters");
        writer.WriteEndElement();
        writer.WriteStartElement("REQUESTDATA");
        writeData();
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void StartTallyMessage(XmlWriter writer)
    {
        writer.WriteStartElement("TALLYMESSAGE");
        writer.WriteAttributeString("xmlns", "UDF", null, "TallyUDF");
    }

    private static void WriteUnit(XmlWriter writer, UnitMasterExport unit)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("UNIT");
        writer.WriteAttributeString("NAME", unit.Name);
        writer.WriteAttributeString("ACTION", "Create");
        writer.WriteElementString("NAME", unit.Name);
        writer.WriteElementString("ISSIMPLEUNIT", "Yes");
        writer.WriteElementString("DECIMALPLACES", unit.DecimalPlaces.ToString(CultureInfo.InvariantCulture));
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteStockGroup(XmlWriter writer, StockGroupMasterExport group)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("STOCKGROUP");
        writer.WriteAttributeString("NAME", group.Name);
        writer.WriteAttributeString("ACTION", "Create");
        writer.WriteElementString("NAME", group.Name);
        writer.WriteElementString("PARENT", "");
        writer.WriteElementString("ISADDABLE", "Yes");
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteCustomerLedger(XmlWriter writer, CustomerMasterExport customer)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("LEDGER");
        writer.WriteAttributeString("NAME", customer.Name);
        writer.WriteAttributeString("ACTION", "Create");
        writer.WriteElementString("NAME", customer.Name);
        writer.WriteElementString("PARENT", "Sundry Debtors");
        writer.WriteElementString("COUNTRYOFRESIDENCE", "India");
        writer.WriteElementString("STATENAME", customer.State);
        writer.WriteElementString("PLACEOFSUPPLY", customer.State);
        writer.WriteElementString("GSTREGISTRATIONTYPE", customer.GstRegistrationType);
        if (!string.IsNullOrWhiteSpace(customer.Gstin))
        {
            writer.WriteElementString("GSTIN", customer.Gstin);
        }

        WriteAddressLines(writer, customer.AddressLines);
        writer.WriteElementString("ISBILLWISEON", "Yes");
        writer.WriteElementString("OPENINGBALANCE", "0");
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteAddressLines(XmlWriter writer, IList<string> addressLines)
    {
        List<string> cleanLines = addressLines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        if (cleanLines.Count == 0) return;

        writer.WriteStartElement("ADDRESS.LIST");
        writer.WriteAttributeString("TYPE", "String");
        foreach (string line in cleanLines)
        {
            writer.WriteElementString("ADDRESS", line);
        }

        writer.WriteEndElement();
    }

    private static void WriteLedger(XmlWriter writer, LedgerMasterExport ledger)
    {
        if (string.Equals(ledger.Parent, "Duties & Taxes", StringComparison.OrdinalIgnoreCase))
        {
            WriteGstLedger(writer, ledger);
            return;
        }

        if (string.Equals(ledger.Parent, "Sales Accounts", StringComparison.OrdinalIgnoreCase))
        {
            WriteSalesLedger(writer, ledger);
            return;
        }

        WriteSimpleLedger(writer, ledger);
    }

    private static void WriteSalesLedger(XmlWriter writer, LedgerMasterExport ledger)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("LEDGER");
        writer.WriteAttributeString("NAME", ledger.Name);
        writer.WriteAttributeString("ACTION", "Create");
        writer.WriteElementString("NAME", ledger.Name);
        writer.WriteElementString("PARENT", "Sales Accounts");
        writer.WriteElementString("GSTAPPLICABLE", "Applicable");
        writer.WriteElementString("GSTTYPEOFSUPPLY", "Goods");
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteGstLedger(XmlWriter writer, LedgerMasterExport ledger)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("LEDGER");
        writer.WriteAttributeString("NAME", ledger.Name);
        writer.WriteAttributeString("ACTION", "Create");
        writer.WriteElementString("NAME", ledger.Name);
        writer.WriteElementString("PARENT", "Duties & Taxes");
        writer.WriteElementString("TAXTYPE", "GST");
        writer.WriteElementString("GSTDUTYHEAD", GetGstDutyHead(ledger));
        writer.WriteElementString("GSTAPPLICABLE", "Applicable");
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteSimpleLedger(XmlWriter writer, LedgerMasterExport ledger)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("LEDGER");
        writer.WriteAttributeString("NAME", ledger.Name);
        writer.WriteAttributeString("ACTION", "Create");
        writer.WriteElementString("NAME", ledger.Name);
        writer.WriteElementString("PARENT", string.IsNullOrWhiteSpace(ledger.Parent) ? "Indirect Expenses" : ledger.Parent);
        writer.WriteElementString("OPENINGBALANCE", "0");
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static string GetGstDutyHead(LedgerMasterExport ledger)
    {
        if (string.Equals(ledger.DutyHead, "Central Tax", StringComparison.OrdinalIgnoreCase)) return "Central Tax";
        if (string.Equals(ledger.DutyHead, "State Tax", StringComparison.OrdinalIgnoreCase)) return "State Tax";
        if (string.Equals(ledger.DutyHead, "Integrated Tax", StringComparison.OrdinalIgnoreCase)) return "Integrated Tax";
        if (ledger.Name.IndexOf("IGST", StringComparison.OrdinalIgnoreCase) >= 0) return "Integrated Tax";
        if (ledger.Name.IndexOf("SGST", StringComparison.OrdinalIgnoreCase) >= 0) return "State Tax";
        return "Central Tax";
    }

    private static void WriteStockItem(XmlWriter writer, ProductMasterExport item, TallyCompanySettings settings)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("STOCKITEM");
        writer.WriteAttributeString("NAME", item.Name);
        writer.WriteAttributeString("ACTION", "Create");
        writer.WriteElementString("NAME", item.Name);
        writer.WriteElementString("PARENT", item.StockGroupName);
        writer.WriteElementString("BASEUNITS", item.BaseUnit);
        writer.WriteElementString("GSTAPPLICABLE", "Applicable");
        writer.WriteElementString("GSTTYPEOFSUPPLY", "Goods");
        writer.WriteElementString("COSTINGMETHOD", "Avg. Cost");
        writer.WriteElementString("VALUATIONMETHOD", "Avg. Price");
        writer.WriteElementString("OPENINGBALANCE", FormatQuantity(item.OpeningBalance, item.BaseUnit));
        writer.WriteElementString("OPENINGRATE", "0/" + item.BaseUnit);
        writer.WriteElementString("OPENINGVALUE", "0");
        WriteStockItemGstDetails(writer, item, settings);
        WriteStockItemHsnDetails(writer, item, settings);
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteStockItemGstDetails(XmlWriter writer, ProductMasterExport item, TallyCompanySettings settings)
    {
        writer.WriteStartElement("GSTDETAILS.LIST");
        writer.WriteElementString("APPLICABLEFROM", settings.MasterApplicableFrom);
        writer.WriteElementString("TAXABILITY", item.GstRate > 0m ? "Taxable" : "Exempt");
        writer.WriteElementString("GSTINELIGIBLEITC", "No");
        writer.WriteStartElement("STATEWISEDETAILS.LIST");
        writer.WriteElementString("STATENAME", "Any");
        WriteRateDetail(writer, "Central Tax", item.GstRate / 2m);
        WriteRateDetail(writer, "State Tax", item.GstRate / 2m);
        WriteRateDetail(writer, "Integrated Tax", item.GstRate);
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteRateDetail(XmlWriter writer, string dutyHead, decimal rate)
    {
        writer.WriteStartElement("RATEDETAILS.LIST");
        writer.WriteElementString("GSTRATEDUTYHEAD", dutyHead);
        writer.WriteElementString("GSTRATEVALUATIONTYPE", "Based on Value");
        writer.WriteElementString("GSTRATE", TallyNumericHelper.FormatGstRate(rate));
        writer.WriteEndElement();
    }

    private static void WriteStockItemHsnDetails(XmlWriter writer, ProductMasterExport item, TallyCompanySettings settings)
    {
        writer.WriteStartElement("HSNDETAILS.LIST");
        writer.WriteElementString("APPLICABLEFROM", settings.MasterApplicableFrom);
        writer.WriteElementString("HSNCODE", item.Hsn);
        writer.WriteElementString("SRCOFHSNDETAILS", "Specify Details Here");
        writer.WriteEndElement();
    }

    private static string FormatQuantity(decimal quantity, string unit)
    {
        return TallyNumericHelper.FormatQuantity(quantity) + " " + unit;
    }

    private static void ValidateMastersXml(string path)
    {
        string xml = File.ReadAllText(path);
        string[] forbiddenTokens =
        {
            "CATEGORY",
            "TAXCLASSIFICATION",
            "TAXCLASSIFICATIONNAME",
            ">Not Applicable<",
            "STOCKCATEGORY",
            "TCSCATEGORYDETAILS",
            "TDSCATEGORYDETAILS",
            "VATCLASSIFICATIONDETAILS",
            "BATCHALLOCATIONS",
            "OLDAUDITENTRYIDS",
            "VATAPPLICABLE",
            "VATBASEUNIT",
            "RATEOFVAT",
            "VATBASENO",
            "VATTRAILNO",
            "VATACTUALRATIO",
            "SALESTAXCESSAPPLICABLE",
            "REPORTINGUOMDETAILS",
            "GSTCLASSFNIGSTRATES",
            "EXTARIFFDUTYHEADDETAILS",
            "TEMPGSTITEMSLABRATES"
        };

        foreach (string token in forbiddenTokens)
        {
            int count = CountOccurrences(xml, token);
            if (count != 0)
            {
                throw new InvalidOperationException("Minimal Masters XML validation failed: found " + count.ToString(CultureInfo.InvariantCulture) + " forbidden token(s): " + token + ".");
            }
        }

        XDocument document = XDocument.Load(path);
        foreach (XElement stockItem in document.Descendants("STOCKITEM"))
        {
            string itemName = ((string?)stockItem.Element("NAME") ?? "").Trim();
            string parent = ((string?)stockItem.Element("PARENT") ?? "").Trim();
            string baseUnit = ((string?)stockItem.Element("BASEUNITS") ?? "").Trim();
            string hsn = ((string?)stockItem.Element("HSNDETAILS.LIST")?.Element("HSNCODE") ?? "").Trim();
            if (itemName.Length == 0 || parent.Length == 0 || baseUnit.Length == 0 || hsn.Length == 0)
            {
                throw new InvalidOperationException("Minimal Masters XML validation failed: stock item '" + itemName + "' is missing NAME, PARENT, BASEUNITS, or HSN.");
            }

            List<string> dutyHeads = stockItem.Descendants("RATEDETAILS.LIST")
                .Select(e => ((string?)e.Element("GSTRATEDUTYHEAD") ?? "").Trim())
                .Where(v => v.Length > 0)
                .ToList();

            string[] expected = { "Central Tax", "State Tax", "Integrated Tax" };
            foreach (string dutyHead in expected)
            {
                if (dutyHeads.Count(v => string.Equals(v, dutyHead, StringComparison.OrdinalIgnoreCase)) != 1)
                {
                    throw new InvalidOperationException("Minimal Masters XML validation failed: stock item '" + itemName + "' must contain exactly one " + dutyHead + " rate detail.");
                }
            }

            if (dutyHeads.Count != expected.Length)
            {
                throw new InvalidOperationException("Minimal Masters XML validation failed: stock item '" + itemName + "' contains unexpected GST rate detail entries.");
            }
        }
    }

    private static int CountOccurrences(string text, string value)
    {
        int count = 0;
        int index = 0;
        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }
}
