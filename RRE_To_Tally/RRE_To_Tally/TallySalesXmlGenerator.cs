using System.Xml;

namespace RRE_To_Tally;

public sealed class TallySalesXmlGenerator
{
    public void WriteSalesEnvelope(XmlWriter writer, IList<SalesExportInvoice> invoices, TallyCompanySettings settings, TallyExportOptions options)
    {
        string companyName = GetCompanyName(invoices, settings);

        writer.WriteStartElement("ENVELOPE");
        writer.WriteStartElement("HEADER");
        writer.WriteElementString("TALLYREQUEST", "Import Data");
        writer.WriteEndElement();
        writer.WriteStartElement("BODY");
        writer.WriteStartElement("IMPORTDATA");
        writer.WriteStartElement("REQUESTDESC");
        writer.WriteElementString("REPORTNAME", "Vouchers");
        writer.WriteStartElement("STATICVARIABLES");
        writer.WriteElementString("SVCURRENTCOMPANY", companyName);
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteStartElement("REQUESTDATA");

        foreach (SalesExportInvoice invoice in invoices)
        {
            WriteTallyMessage(writer, invoice, settings, options);
        }

        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteTallyMessage(XmlWriter writer, SalesExportInvoice invoice, TallyCompanySettings settings, TallyExportOptions options)
    {
        writer.WriteStartElement("TALLYMESSAGE");
        writer.WriteAttributeString("xmlns", "UDF", null, "TallyUDF");
        writer.WriteStartElement("VOUCHER");
        writer.WriteAttributeString("VCHTYPE", "Sales");
        writer.WriteAttributeString("ACTION", "Create");
        writer.WriteAttributeString("OBJVIEW", "Invoice Voucher View");

        string partyLedger = GetPartyLedger(invoice, settings, options);
        WriteVoucherHeader(writer, invoice, partyLedger, settings);

        foreach (SalesExportItem item in invoice.Items)
        {
            WriteInventoryEntry(writer, item, settings);
        }

        WritePartyLedgerEntry(writer, invoice, partyLedger);
        if (invoice.Cgst != 0m) WriteTaxLedgerEntry(writer, settings.CGSTLedgerName, invoice.Cgst);
        if (invoice.Sgst != 0m) WriteTaxLedgerEntry(writer, settings.SGSTLedgerName, invoice.Sgst);
        if (invoice.Igst != 0m) WriteTaxLedgerEntry(writer, settings.IGSTLedgerName, invoice.Igst);
        if (invoice.Discount != 0m) WriteAdjustmentLedgerEntry(writer, settings.DiscountLedgerName, -invoice.Discount);
        if (invoice.OtherCharges != 0m) WriteAdjustmentLedgerEntry(writer, settings.OtherChargesLedgerName, invoice.OtherCharges);
        if (invoice.RoundOff != 0m) WriteAdjustmentLedgerEntry(writer, settings.RoundOffLedgerName, invoice.RoundOff);

        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteVoucherHeader(XmlWriter writer, SalesExportInvoice invoice, string partyLedger, TallyCompanySettings settings)
    {
        string date = invoice.Date.ToString("yyyyMMdd");
        string billPlace = FirstNonEmpty(invoice.CustomerCity, invoice.CustomerState, settings.CompanyState);

        writer.WriteElementString("DATE", date);
        writer.WriteElementString("VCHSTATUSDATE", date);
        writer.WriteElementString("GSTREGISTRATIONTYPE", invoice.GstRegistrationType);
        writer.WriteElementString("VATDEALERTYPE", invoice.GstRegistrationType);
        writer.WriteElementString("STATENAME", invoice.CustomerState);
        writer.WriteElementString("COUNTRYOFRESIDENCE", "India");
        writer.WriteElementString("PARTYGSTIN", invoice.CustomerGSTIN);
        writer.WriteElementString("PLACEOFSUPPLY", invoice.CustomerState);
        writer.WriteElementString("PARTYNAME", partyLedger);
        writer.WriteElementString("VOUCHERTYPENAME", "Sales");
        writer.WriteElementString("PARTYLEDGERNAME", partyLedger);
        writer.WriteElementString("VOUCHERNUMBER", invoice.SalesId);
        writer.WriteElementString("BASICBUYERNAME", partyLedger);
        writer.WriteElementString("PARTYMAILINGNAME", partyLedger);
        writer.WriteElementString("CONSIGNEEMAILINGNAME", partyLedger);
        writer.WriteElementString("PARTYPINCODE", invoice.Pincode);
        writer.WriteElementString("BILLTOPLACE", billPlace);
        writer.WriteElementString("SHIPTOPLACE", billPlace);
        writer.WriteElementString("CONSIGNEEGSTIN", invoice.CustomerGSTIN);
        writer.WriteElementString("CONSIGNEEPINCODE", invoice.Pincode);
        writer.WriteElementString("CONSIGNEESTATENAME", invoice.CustomerState);
        writer.WriteElementString("CONSIGNEECOUNTRYNAME", "India");
        writer.WriteElementString("PERSISTEDVIEW", "Invoice Voucher View");
        writer.WriteElementString("VCHENTRYMODE", "Item Invoice");
        writer.WriteElementString("ISINVOICE", "Yes");
        WriteAddressList(writer, "BASICBUYERADDRESS.LIST", "BASICBUYERADDRESS", invoice);
        WriteAddressList(writer, "ADDRESS.LIST", "ADDRESS", invoice);
    }

    private static void WriteInventoryEntry(XmlWriter writer, SalesExportItem item, TallyCompanySettings settings)
    {
        string quantity = TallyNumericHelper.FormatQuantity(item.Quantity) + " " + item.Uom;

        writer.WriteStartElement("ALLINVENTORYENTRIES.LIST");
        writer.WriteElementString("STOCKITEMNAME", item.ProductTallyName);
        writer.WriteElementString("GSTOVRDNISREVCHARGEAPPL", "Not Applicable");
        writer.WriteElementString("GSTOVRDNTAXABILITY", item.GstRate > 0m ? "Taxable" : "Exempt");
        writer.WriteElementString("GSTSOURCETYPE", "Stock Item");
        writer.WriteElementString("GSTITEMSOURCE", item.ProductTallyName);
        writer.WriteElementString("HSNSOURCETYPE", "Stock Item");
        writer.WriteElementString("HSNITEMSOURCE", item.ProductTallyName);
        writer.WriteElementString("GSTOVRDNTYPEOFSUPPLY", "Goods");
        writer.WriteElementString("GSTRATEINFERAPPLICABILITY", "As per Masters/Company");
        writer.WriteElementString("GSTHSNNAME", item.Hsn);
        writer.WriteElementString("GSTHSNINFERAPPLICABILITY", "As per Masters/Company");
        writer.WriteElementString("ISDEEMEDPOSITIVE", "No");
        writer.WriteElementString("RATE", TallyNumericHelper.FormatAmount(item.Rate) + "/" + item.Uom);
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(item.TaxableAmount));
        writer.WriteElementString("ACTUALQTY", quantity);
        writer.WriteElementString("BILLEDQTY", quantity);

        WriteBatchAllocation(writer, item, settings, quantity);
        WriteAccountingAllocation(writer, item);
        WriteRateDetails(writer, item);
        writer.WriteEndElement();
    }

    private static void WriteBatchAllocation(XmlWriter writer, SalesExportItem item, TallyCompanySettings settings, string quantity)
    {
        writer.WriteStartElement("BATCHALLOCATIONS.LIST");
        writer.WriteElementString("GODOWNNAME", settings.MainGodownName);
        writer.WriteElementString("BATCHNAME", settings.PrimaryBatchName);
        writer.WriteElementString("INDENTNO", "Not Applicable");
        writer.WriteElementString("ORDERNO", "Not Applicable");
        writer.WriteElementString("TRACKINGNUMBER", "Not Applicable");
        writer.WriteElementString("DYNAMICCSTISCLEARED", "No");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(item.TaxableAmount));
        writer.WriteElementString("ACTUALQTY", quantity);
        writer.WriteElementString("BILLEDQTY", quantity);
        writer.WriteEndElement();
    }

    private static void WriteAccountingAllocation(XmlWriter writer, SalesExportItem item)
    {
        writer.WriteStartElement("ACCOUNTINGALLOCATIONS.LIST");
        writer.WriteElementString("LEDGERNAME", item.SalesLedgerName);
        writer.WriteElementString("GSTCLASS", "Not Applicable");
        writer.WriteElementString("ISDEEMEDPOSITIVE", "No");
        writer.WriteElementString("LEDGERFROMITEM", "No");
        writer.WriteElementString("REMOVEZEROENTRIES", "No");
        writer.WriteElementString("ISPARTYLEDGER", "No");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(item.TaxableAmount));
        writer.WriteEndElement();
    }

    private static void WriteRateDetails(XmlWriter writer, SalesExportItem item)
    {
        WriteRateDetail(writer, "CGST", item.GstRate / 2m);
        WriteRateDetail(writer, "SGST/UTGST", item.GstRate / 2m);
        WriteRateDetail(writer, "IGST", item.GstRate);
    }

    private static void WriteRateDetail(XmlWriter writer, string dutyHead, decimal rate)
    {
        writer.WriteStartElement("RATEDETAILS.LIST");
        writer.WriteElementString("GSTRATEDUTYHEAD", dutyHead);
        writer.WriteElementString("GSTRATEVALUATIONTYPE", "Based on Value");
        writer.WriteElementString("GSTRATE", TallyNumericHelper.FormatGstRate(rate));
        writer.WriteEndElement();
    }

    private static void WritePartyLedgerEntry(XmlWriter writer, SalesExportInvoice invoice, string partyLedger)
    {
        decimal amount = -invoice.CalculatedTotal;
        writer.WriteStartElement("LEDGERENTRIES.LIST");
        writer.WriteElementString("LEDGERNAME", partyLedger);
        writer.WriteElementString("GSTCLASS", "Not Applicable");
        writer.WriteElementString("ISDEEMEDPOSITIVE", "Yes");
        writer.WriteElementString("ISPARTYLEDGER", "Yes");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(amount));
        writer.WriteStartElement("BILLALLOCATIONS.LIST");
        writer.WriteElementString("NAME", invoice.SalesId);
        writer.WriteElementString("BILLTYPE", "New Ref");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(amount));
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteTaxLedgerEntry(XmlWriter writer, string ledgerName, decimal amount)
    {
        writer.WriteStartElement("LEDGERENTRIES.LIST");
        writer.WriteElementString("LEDGERNAME", ledgerName);
        writer.WriteElementString("ROUNDTYPE", "Not Applicable");
        writer.WriteElementString("ISDEEMEDPOSITIVE", "No");
        writer.WriteElementString("ISPARTYLEDGER", "No");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(amount));
        writer.WriteElementString("VATEXPAMOUNT", TallyNumericHelper.FormatAmount(amount));
        writer.WriteEndElement();
    }

    private static void WriteAdjustmentLedgerEntry(XmlWriter writer, string ledgerName, decimal amount)
    {
        writer.WriteStartElement("LEDGERENTRIES.LIST");
        writer.WriteElementString("LEDGERNAME", ledgerName);
        writer.WriteElementString("ROUNDTYPE", "Not Applicable");
        writer.WriteElementString("ISDEEMEDPOSITIVE", amount < 0m ? "Yes" : "No");
        writer.WriteElementString("ISPARTYLEDGER", "No");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(amount));
        writer.WriteEndElement();
    }

    private static void WriteAddressList(XmlWriter writer, string listName, string itemName, SalesExportInvoice invoice)
    {
        writer.WriteStartElement(listName);
        writer.WriteAttributeString("TYPE", "String");
        WriteOptionalRepeated(writer, itemName, invoice.CustomerAddress1);
        WriteOptionalRepeated(writer, itemName, invoice.CustomerAddress2);
        WriteOptionalRepeated(writer, itemName, FirstNonEmpty(invoice.CustomerCity, invoice.CustomerState));
        writer.WriteFullEndElement();
    }

    private static void WriteOptionalRepeated(XmlWriter writer, string name, string value)
    {
        string cleaned = TallyNameHelper.CleanXmlText(value).Trim();
        if (cleaned.Length > 0) writer.WriteElementString(name, cleaned);
    }

    private static string FirstNonEmpty(params string[] values)
    {
        foreach (string value in values)
        {
            string cleaned = TallyNameHelper.CleanXmlText(value).Trim();
            if (cleaned.Length > 0) return cleaned;
        }

        return "";
    }

    private static string GetCompanyName(IList<SalesExportInvoice> invoices, TallyCompanySettings settings)
    {
        if (!string.IsNullOrWhiteSpace(settings.CompanyName)) return settings.CompanyName;
        SalesExportInvoice? first = invoices.FirstOrDefault();
        if (first != null && !string.IsNullOrWhiteSpace(first.DivisionCompanyName)) return first.DivisionCompanyName;
        return SalesDivisionConfig.All[0].CompanyName;
    }

    private static string GetPartyLedger(SalesExportInvoice invoice, TallyCompanySettings settings, TallyExportOptions options)
    {
        if (options.CashSalesLedgerBehaviour == CashSalesLedgerBehaviour.UseRreCashLedger &&
            invoice.PaymentMode.IndexOf("cash", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return settings.CashLedgerName;
        }

        return invoice.CustomerLedgerName;
    }
}
