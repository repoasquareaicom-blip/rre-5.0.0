using System.Text;
using System.Xml;

namespace RRE_To_Tally;

public sealed class TallyXmlWriter
{
    private static readonly XmlWriterSettings Settings = new XmlWriterSettings
    {
        Indent = true,
        Encoding = new UTF8Encoding(false),
        OmitXmlDeclaration = true,
        NewLineHandling = NewLineHandling.Entitize
    };

    public void WriteMastersXml(string finalPath, TallyExportPackage package, TallyExportOptions options)
    {
        TallyCompanySettings settings = options.CompanySettings ?? TallyCompanySettings.Load();
        WriteValidated(finalPath, delegate(XmlWriter writer)
        {
            WriteMastersEnvelope(writer, delegate
            {
                foreach (AccountGroupDefinition group in GetRequiredAccountGroups(package, settings)) WriteAccountGroup(writer, group);
                foreach (LedgerMasterExport ledger in package.Ledgers.Where(l => string.Equals(l.Parent, "Sales Accounts", StringComparison.OrdinalIgnoreCase))) WriteLedger(writer, ledger, settings);
                foreach (LedgerMasterExport ledger in package.Ledgers.Where(l => string.Equals(l.Parent, "Duties & Taxes", StringComparison.OrdinalIgnoreCase))) WriteLedger(writer, ledger, settings);
                foreach (LedgerMasterExport ledger in package.Ledgers.Where(l => string.Equals(l.Name, settings.RoundOffLedgerName, StringComparison.OrdinalIgnoreCase))) WriteLedger(writer, ledger, settings);
                foreach (LedgerMasterExport ledger in package.Ledgers.Where(l => !string.Equals(l.Parent, "Sales Accounts", StringComparison.OrdinalIgnoreCase) && !string.Equals(l.Parent, "Duties & Taxes", StringComparison.OrdinalIgnoreCase) && !string.Equals(l.Name, settings.RoundOffLedgerName, StringComparison.OrdinalIgnoreCase))) WriteLedger(writer, ledger, settings);
                if (options.IncludeCustomerMasters) foreach (CustomerMasterExport customer in package.Customers) WriteCustomerLedger(writer, customer, settings);
                foreach (UnitMasterExport unit in package.Units) WriteUnit(writer, unit, settings);
                foreach (StockGroupMasterExport group in package.StockGroups) WriteStockGroup(writer, group, settings);
                if (options.IncludeProductMasters) foreach (ProductMasterExport product in package.Products) WriteStockItem(writer, product, settings);
            });
        });
    }

    public void WriteSalesXml(string finalPath, IList<SalesExportInvoice> invoices, TallyExportOptions options)
    {
        TallyCompanySettings settings = options.CompanySettings ?? TallyCompanySettings.Load();
        ReferenceTallySalesXmlWriter salesWriter = new ReferenceTallySalesXmlWriter();
        WriteValidated(finalPath, delegate(XmlWriter writer)
        {
            salesWriter.WriteEnvelopeStart(writer);
            foreach (SalesExportInvoice invoice in invoices)
            {
                salesWriter.WriteVoucher(writer, invoice, settings, options);
            }
            salesWriter.WriteEnvelopeEnd(writer);
        });
    }

    private static void WriteMastersEnvelope(XmlWriter writer, Action writeData)
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
    }

    private sealed class AccountGroupDefinition
    {
        public string Name { get; set; } = "";
        public string Parent { get; set; } = "";
    }

    private static IEnumerable<AccountGroupDefinition> GetRequiredAccountGroups(TallyExportPackage package, TallyCompanySettings settings)
    {
        Dictionary<string, AccountGroupDefinition> groups = new Dictionary<string, AccountGroupDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (LedgerMasterExport ledger in package.Ledgers)
        {
            string parent = GetMasterLedgerParent(ledger);
            if (parent == "SALES") AddGroup(groups, "SALES", "Sales Accounts");
            if (parent == "TAX AND DUTIES") AddGroup(groups, "TAX AND DUTIES", "Duties & Taxes");
            if (parent == "INDIRECT EXPENSES") AddGroup(groups, "INDIRECT EXPENSES", "Indirect Expenses");
            if (parent == "CASH A/C") AddGroup(groups, "CASH A/C", "Cash-in-Hand");
        }
        if (package.Customers.Count > 0) AddGroup(groups, "SUNDRY DEBTORS", "Sundry Debtors");
        return groups.Values;
    }

    private static void AddGroup(Dictionary<string, AccountGroupDefinition> groups, string name, string parent)
    {
        if (!groups.ContainsKey(name)) groups.Add(name, new AccountGroupDefinition { Name = name, Parent = parent });
    }

    private static void WriteAccountGroup(XmlWriter writer, AccountGroupDefinition group)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("GROUP");
        writer.WriteAttributeString("NAME", group.Name);
        writer.WriteAttributeString("RESERVEDNAME", "");
        writer.WriteElementString("PARENT", group.Parent);
        writer.WriteElementString("BASICGROUPISCALCULABLE", "No");
        writer.WriteElementString("ADDLALLOCTYPE", "");
        writer.WriteElementString("GRPDEBITPARENT", "");
        writer.WriteElementString("GRPCREDITPARENT", "");
        writer.WriteElementString("ISBILLWISEON", "No");
        writer.WriteElementString("ISCOSTCENTRESON", "No");
        writer.WriteElementString("ISADDABLE", "No");
        writer.WriteElementString("ISUPDATINGTARGETID", "No");
        writer.WriteElementString("ASORIGINAL", "Yes");
        writer.WriteElementString("ISSUBLEDGER", "No");
        writer.WriteElementString("ISREVENUE", "No");
        writer.WriteElementString("AFFECTSGROSSPROFIT", "No");
        writer.WriteElementString("ISDEEMEDPOSITIVE", "No");
        writer.WriteElementString("TRACKNEGATIVEBALANCES", "Yes");
        writer.WriteElementString("ISCONDENSED", "No");
        writer.WriteElementString("AFFECTSSTOCK", "No");
        writer.WriteElementString("ISGROUPFORLOANRCPT", "No");
        writer.WriteElementString("ISGROUPFORLOANPYMNT", "No");
        writer.WriteElementString("ISRATEINCLUSIVEVAT", "No");
        writer.WriteElementString("ISINVDETAILSENABLE", "No");
        WriteEmptyLists(writer, "SERVICETAXDETAILS.LIST", "VATDETAILS.LIST", "SALESTAXCESSDETAILS.LIST", "GSTDETAILS.LIST");
        WriteLanguageName(writer, group.Name);
        WriteEmptyLists(writer, "XBRLDETAIL.LIST", "AUDITDETAILS.LIST", "SCHVIDETAILS.LIST", "EXCISETARIFFDETAILS.LIST", "TCSCATEGORYDETAILS.LIST", "TDSCATEGORYDETAILS.LIST", "GSTCLASSFNIGSTRATES.LIST", "EXTARIFFDUTYHEADDETAILS.LIST");
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteUnit(XmlWriter writer, UnitMasterExport unit, TallyCompanySettings settings)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("UNIT");
        writer.WriteAttributeString("NAME", unit.Name);
        writer.WriteAttributeString("RESERVEDNAME", "");
        writer.WriteElementString("NAME", unit.Name);
        writer.WriteElementString("ISUPDATINGTARGETID", "No");
        writer.WriteElementString("ISDELETED", "No");
        writer.WriteElementString("ISSECURITYONWHENENTERED", "No");
        writer.WriteElementString("ASORIGINAL", "Yes");
        writer.WriteElementString("ISGSTEXCLUDED", "No");
        writer.WriteElementString("ISSIMPLEUNIT", "Yes");
        writer.WriteStartElement("REPORTINGUQCDETAILS.LIST");
        writer.WriteElementString("APPLICABLEFROM", settings.MasterApplicableFrom);
        writer.WriteElementString("REPORTINGUQCNAME", GetReportingUqcName(unit.Name));
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteStockGroup(XmlWriter writer, StockGroupMasterExport group, TallyCompanySettings settings)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("STOCKGROUP");
        writer.WriteAttributeString("NAME", group.Name);
        writer.WriteAttributeString("RESERVEDNAME", "");
        writer.WriteElementString("PARENT", "");
        writer.WriteElementString("COSTINGMETHOD", "Avg. Cost");
        writer.WriteElementString("VALUATIONMETHOD", "Avg. Price");
        writer.WriteElementString("BASEUNITS", "NOS");
        writer.WriteElementString("ADDITIONALUNITS", "");
        writer.WriteElementString("ISBATCHWISEON", "No");
        writer.WriteElementString("ISPERISHABLEON", "No");
        writer.WriteElementString("ISADDABLE", "No");
        writer.WriteElementString("ISUPDATINGTARGETID", "No");
        writer.WriteElementString("ISDELETED", "No");
        writer.WriteElementString("ISSECURITYONWHENENTERED", "No");
        writer.WriteElementString("ASORIGINAL", "Yes");
        writer.WriteElementString("IGNOREPHYSICALDIFFERENCE", "No");
        writer.WriteElementString("IGNORENEGATIVESTOCK", "No");
        writer.WriteElementString("TREATSALESASMANUFACTURED", "No");
        writer.WriteElementString("TREATPURCHASESASCONSUMED", "No");
        writer.WriteElementString("TREATREJECTSASSCRAP", "No");
        writer.WriteElementString("HASMFGDATE", "No");
        writer.WriteElementString("ALLOWUSEOFEXPIREDITEMS", "No");
        writer.WriteElementString("IGNOREBATCHES", "No");
        writer.WriteElementString("IGNOREGODOWNS", "No");
        WriteEmptyLists(writer, "SERVICETAXDETAILS.LIST", "VATDETAILS.LIST", "SALESTAXCESSDETAILS.LIST");
        WriteStockGroupGstDetails(writer, settings);
        writer.WriteStartElement("HSNDETAILS.LIST");
        writer.WriteElementString("APPLICABLEFROM", settings.MasterApplicableFrom);
        writer.WriteEndElement();
        WriteLanguageName(writer, group.Name);
        WriteEmptyLists(writer, "SCHVIDETAILS.LIST", "EXCISETARIFFDETAILS.LIST", "TCSCATEGORYDETAILS.LIST", "TDSCATEGORYDETAILS.LIST", "GSTCLASSFNIGSTRATES.LIST", "EXTARIFFDUTYHEADDETAILS.LIST", "TEMPGSTITEMSLABRATES.LIST");
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteCustomerLedger(XmlWriter writer, CustomerMasterExport customer, TallyCompanySettings settings)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("LEDGER");
        writer.WriteAttributeString("NAME", customer.Name);
        writer.WriteAttributeString("RESERVEDNAME", "");
        WriteLedgerBody(writer, customer.Name, "SUNDRY DEBTORS", customer.State, customer.Pincode, customer.GstRegistrationType, settings.MasterApplicableFrom, customer.Gstin, "", customer.AddressLines);
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private static void WriteLedger(XmlWriter writer, LedgerMasterExport ledger, TallyCompanySettings settings)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("LEDGER");
        writer.WriteAttributeString("NAME", ledger.Name);
        writer.WriteAttributeString("RESERVEDNAME", "");
        WriteLedgerBody(writer, ledger.Name, GetMasterLedgerParent(ledger), settings.CompanyState, "", "Unregistered/Consumer", settings.MasterApplicableFrom, "", "", new List<string>());
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    public void WriteStockItem(XmlWriter writer, ProductMasterExport item, TallyCompanySettings settings)
    {
        StartTallyMessage(writer);
        writer.WriteStartElement("STOCKITEM");
        writer.WriteAttributeString("NAME", item.Name);
        writer.WriteAttributeString("RESERVEDNAME", "");
        WriteOldAudit(writer);
        writer.WriteElementString("PARENT", item.StockGroupName);
        writer.WriteElementString("CATEGORY", "Not Applicable");
        writer.WriteElementString("GSTAPPLICABLE", item.GstRate > 0m ? "Applicable" : "Not Applicable");
        writer.WriteElementString("TAXCLASSIFICATIONNAME", "Not Applicable");
        writer.WriteElementString("GSTTYPEOFSUPPLY", "Goods");
        writer.WriteElementString("EXCISEAPPLICABILITY", "Not Applicable");
        writer.WriteElementString("SALESTAXCESSAPPLICABLE", "");
        writer.WriteElementString("VATAPPLICABLE", "Applicable");
        writer.WriteElementString("COSTINGMETHOD", "Avg. Cost");
        writer.WriteElementString("VALUATIONMETHOD", "Avg. Price");
        writer.WriteElementString("BASEUNITS", item.BaseUnit);
        writer.WriteElementString("ADDITIONALUNITS", "Not Applicable");
        writer.WriteElementString("EXCISEITEMCLASSIFICATION", "Not Applicable");
        writer.WriteElementString("VATBASEUNIT", item.BaseUnit);
        WriteStockItemFlags(writer);
        writer.WriteElementString("DENOMINATOR", "1");
        writer.WriteElementString("RATEOFVAT", "0");
        writer.WriteElementString("VATBASENO", "1");
        writer.WriteElementString("VATTRAILNO", "1");
        writer.WriteElementString("VATACTUALRATIO", "1");
        writer.WriteElementString("OPENINGBALANCE", FormatMasterQuantity(item.OpeningBalance, item.BaseUnit));
        writer.WriteElementString("OPENINGVALUE", "-0");
        writer.WriteElementString("OPENINGRATE", "0/" + item.BaseUnit);
        WriteEmptyLists(writer, "SERVICETAXDETAILS.LIST", "VATDETAILS.LIST", "SALESTAXCESSDETAILS.LIST");
        WriteStockItemGstDetails(writer, item, settings);
        WriteStockItemHsnDetails(writer, item, settings);
        WriteLanguageName(writer, item.Name);
        WriteEmptyLists(writer, "SCHVIDETAILS.LIST", "EXCISETARIFFDETAILS.LIST", "TCSCATEGORYDETAILS.LIST", "TDSCATEGORYDETAILS.LIST", "EXCLUDEDTAXATIONS.LIST", "OLDAUDITENTRIES.LIST", "ACCOUNTAUDITENTRIES.LIST", "AUDITENTRIES.LIST", "OLDMRPDETAILS.LIST", "VATCLASSIFICATIONDETAILS.LIST", "MRPDETAILS.LIST", "REPORTINGUOMDETAILS.LIST", "COMPONENTLIST.LIST", "ADDITIONALLEDGERS.LIST", "SALESLIST.LIST", "PURCHASELIST.LIST", "FULLPRICELIST.LIST");
        WriteStockItemBatchAllocation(writer, item, settings);
        WriteEmptyLists(writer, "TRADEREXCISEDUTIES.LIST", "STANDARDCOSTLIST.LIST", "STANDARDPRICELIST.LIST", "EXCISEITEMGODOWN.LIST", "MULTICOMPONENTLIST.LIST", "LBTDETAILS.LIST", "PRICELEVELLIST.LIST", "GSTCLASSFNIGSTRATES.LIST", "EXTARIFFDUTYHEADDETAILS.LIST", "TEMPGSTITEMSLABRATES.LIST");
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

    private static void WriteLedgerBody(XmlWriter writer, string name, string parent, string state, string pincode, string gstRegistrationType, string applicableFrom, string gstin, string openingBalance, IList<string> addressLines)
    {
        string balance = string.IsNullOrWhiteSpace(openingBalance) ? "0 " : openingBalance;
        WriteOldAudit(writer);
        writer.WriteElementString("PRIORSTATENAME", "\"" + name + "\"");
        writer.WriteElementString("VATDEALERTYPE", gstRegistrationType);
        writer.WriteElementString("PARENT", parent);
        writer.WriteElementString("TAXCLASSIFICATIONNAME", "Not Applicable");
        writer.WriteElementString("TAXTYPE", "Others");
        writer.WriteElementString("COUNTRYOFRESIDENCE", "India");
        writer.WriteElementString("OPENINGBALANCE", balance);
        writer.WriteElementString("GSTTYPE", "Not Applicable");
        writer.WriteElementString("APPROPRIATEFOR", "Not Applicable");
        writer.WriteElementString("GSTNATUREOFSUPPLY", "Not Applicable");
        writer.WriteElementString("SERVICECATEGORY", "Not Applicable");
        writer.WriteElementString("EXCISELEDGERCLASSIFICATION", "Not Applicable");
        writer.WriteElementString("EXCISEDUTYTYPE", "Not Applicable");
        writer.WriteElementString("EXCISENATUREOFPURCHASE", "Not Applicable");
        writer.WriteElementString("LEDGERFBTCATEGORY", "Not Applicable");
        WriteCustomerLedgerFlags(writer);
        writer.WriteElementString("SORTPOSITION", "1000");
        WriteEmptyLists(writer, "SERVICETAXDETAILS.LIST", "LBTREGNDETAILS.LIST", "VATDETAILS.LIST", "SALESTAXCESSDETAILS.LIST", "GSTDETAILS.LIST", "HSNDETAILS.LIST");
        WriteLanguageName(writer, name);
        writer.WriteElementString("APPLICABLEFROM", applicableFrom);
        writer.WriteElementString("GSTREGISTRATIONTYPE", gstRegistrationType);
        writer.WriteElementString("PLACEOFSUPPLY", state);
        writer.WriteElementString("GSTIN", gstin);
        WriteEmptyLists(writer, "XBRLDETAIL.LIST", "AUDITDETAILS.LIST", "SCHVIDETAILS.LIST", "EXCISETARIFFDETAILS.LIST", "TCSCATEGORYDETAILS.LIST", "TDSCATEGORYDETAILS.LIST", "SLABPERIOD.LIST", "GRATUITYPERIOD.LIST", "ADDITIONALCOMPUTATIONS.LIST", "EXCISEJURISDICTIONDETAILS.LIST", "EXCLUDEDTAXATIONS.LIST", "BANKALLOCATIONS.LIST", "PAYMENTDETAILS.LIST", "BANKEXPORTFORMATS.LIST", "BILLALLOCATIONS.LIST", "INTERESTCOLLECTION.LIST", "LEDGERCLOSINGVALUES.LIST", "LEDGERAUDITCLASS.LIST", "OLDAUDITENTRIES.LIST", "TDSEXEMPTIONRULES.LIST", "DEDUCTINSAMEVCHRULES.LIST", "LOWERDEDUCTION.LIST", "STXABATEMENTDETAILS.LIST", "LEDMULTIADDRESSLIST.LIST", "STXTAXDETAILS.LIST", "CHEQUERANGE.LIST", "DEFAULTVCHCHEQUEDETAILS.LIST", "ACCOUNTAUDITENTRIES.LIST", "AUDITENTRIES.LIST", "BRSIMPORTEDINFO.LIST", "AUTOBRSCONFIGS.LIST", "BANKURENTRIES.LIST", "DEFAULTCHEQUEDETAILS.LIST", "DEFAULTOPENINGCHEQUEDETAILS.LIST", "CANCELLEDPAYALLOCATIONS.LIST", "ECHEQUEPRINTLOCATION.LIST", "ECHEQUEPAYABLELOCATION.LIST", "EDDPRINTLOCATION.LIST", "EDDPAYABLELOCATION.LIST", "AVAILABLETRANSACTIONTYPES.LIST", "LEDPAYINSCONFIGS.LIST", "TYPECODEDETAILS.LIST", "FIELDVALIDATIONDETAILS.LIST", "INPUTCRALLOCS.LIST", "TCSMETHODOFCALCULATION.LIST");
        writer.WriteStartElement("LEDGSTREGDETAILS.LIST");
        writer.WriteElementString("APPLICABLEFROM", applicableFrom);
        writer.WriteElementString("GSTREGISTRATIONTYPE", gstRegistrationType);
        writer.WriteElementString("PLACEOFSUPPLY", state);
        writer.WriteElementString("GSTIN", gstin);
        writer.WriteElementString("ISOTHTERRITORYASSESSEE", "No");
        writer.WriteElementString("CONSIDERPURCHASEFOREXPORT", "No");
        writer.WriteElementString("ISTRANSPORTER", "No");
        writer.WriteElementString("ISCOMMONPARTY", "No");
        writer.WriteEndElement();
        writer.WriteStartElement("LEDMAILINGDETAILS.LIST");
        writer.WriteStartElement("ADDRESS.LIST");
        writer.WriteAttributeString("TYPE", "String");
        foreach (string address in addressLines)
        {
            writer.WriteElementString("ADDRESS", address);
        }
        writer.WriteFullEndElement();
        writer.WriteElementString("APPLICABLEFROM", applicableFrom);
        writer.WriteElementString("PINCODE", pincode);
        writer.WriteElementString("MAILINGNAME", name);
        writer.WriteElementString("STATE", state);
        writer.WriteElementString("COUNTRY", "India");
        writer.WriteEndElement();
        WriteEmptyLists(writer, "GSTRECONPREFIXSUFFIXDETAILS.LIST", "GSTCLASSFNIGSTRATES.LIST", "EXTARIFFDUTYHEADDETAILS.LIST", "TEMPGSTITEMSLABRATES.LIST", "VOUCHERTYPEPRODUCTCODES.LIST", "LEDADDRESS.LIST");
    }

    private static string GetMasterLedgerParent(LedgerMasterExport ledger)
    {
        if (string.Equals(ledger.Parent, "Sales Accounts", StringComparison.OrdinalIgnoreCase)) return "SALES";
        if (string.Equals(ledger.Parent, "Duties & Taxes", StringComparison.OrdinalIgnoreCase)) return "TAX AND DUTIES";
        if (string.Equals(ledger.Parent, "Indirect Expenses", StringComparison.OrdinalIgnoreCase)) return "INDIRECT EXPENSES";
        if (string.Equals(ledger.Parent, "Cash-in-Hand", StringComparison.OrdinalIgnoreCase)) return "CASH A/C";
        return TallyNameHelper.CleanTallyName(ledger.Parent).ToUpperInvariant();
    }

    private static void WriteStockItemGstDetails(XmlWriter writer, ProductMasterExport item, TallyCompanySettings settings)
    {
        writer.WriteStartElement("GSTDETAILS.LIST");
        writer.WriteElementString("APPLICABLEFROM", settings.MasterApplicableFrom);
        writer.WriteElementString("CALCULATIONTYPE", "On Value");
        writer.WriteElementString("TAXABILITY", item.GstRate > 0m ? "Taxable" : "Exempt");
        writer.WriteElementString("SRCOFGSTDETAILS", "Specify Details Here");
        writer.WriteElementString("GSTCALCSLABONMRP", "No");
        writer.WriteElementString("ISREVERSECHARGEAPPLICABLE", "No");
        writer.WriteElementString("ISNONGSTGOODS", "No");
        writer.WriteElementString("GSTINELIGIBLEITC", "Yes");
        writer.WriteElementString("INCLUDEEXPFORSLABCALC", "No");
        writer.WriteStartElement("STATEWISEDETAILS.LIST");
        writer.WriteElementString("STATENAME", "Any");
        WriteMasterItemRateDetail(writer, "CGST", item.GstRate / 2m, true);
        WriteMasterItemRateDetail(writer, "SGST/UTGST", item.GstRate / 2m, true);
        WriteMasterItemRateDetail(writer, "IGST", item.GstRate, true);
        WriteMasterItemRateDetail(writer, "Cess", 0m, false);
        WriteMasterItemRateDetail(writer, "State Cess", 0m, true);
        WriteEmptyList(writer, "GSTSLABRATES.LIST");
        writer.WriteEndElement();
        WriteEmptyList(writer, "TEMPGSTITEMSLABRATES.LIST");
        WriteEmptyList(writer, "TEMPGSTDETAILSLABRATES.LIST");
        writer.WriteEndElement();
    }

    private static void WriteStockGroupGstDetails(XmlWriter writer, TallyCompanySettings settings)
    {
        writer.WriteStartElement("GSTDETAILS.LIST");
        writer.WriteElementString("APPLICABLEFROM", settings.MasterApplicableFrom);
        writer.WriteElementString("SRCOFGSTDETAILS", "Specify Details Here");
        writer.WriteElementString("GSTCALCSLABONMRP", "No");
        writer.WriteElementString("ISREVERSECHARGEAPPLICABLE", "No");
        writer.WriteElementString("ISNONGSTGOODS", "No");
        writer.WriteElementString("GSTINELIGIBLEITC", "Yes");
        writer.WriteElementString("INCLUDEEXPFORSLABCALC", "No");
        writer.WriteStartElement("STATEWISEDETAILS.LIST");
        WriteStockGroupRateDetail(writer, "CGST", true);
        WriteStockGroupRateDetail(writer, "SGST/UTGST", true);
        WriteStockGroupRateDetail(writer, "IGST", true);
        WriteStockGroupRateDetail(writer, "Cess", false);
        WriteStockGroupRateDetail(writer, "State Cess", true);
        WriteEmptyList(writer, "GSTSLABRATES.LIST");
        writer.WriteEndElement();
        WriteEmptyList(writer, "TEMPGSTITEMSLABRATES.LIST");
        WriteEmptyList(writer, "TEMPGSTDETAILSLABRATES.LIST");
        writer.WriteEndElement();
    }

    private static void WriteStockGroupRateDetail(XmlWriter writer, string dutyHead, bool includeValuation)
    {
        writer.WriteStartElement("RATEDETAILS.LIST");
        writer.WriteElementString("GSTRATEDUTYHEAD", dutyHead);
        if (includeValuation) writer.WriteElementString("GSTRATEVALUATIONTYPE", "Based on Value");
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

    private static void WriteStockItemBatchAllocation(XmlWriter writer, ProductMasterExport item, TallyCompanySettings settings)
    {
        writer.WriteStartElement("BATCHALLOCATIONS.LIST");
        writer.WriteElementString("GODOWNNAME", settings.MainGodownName);
        writer.WriteElementString("BATCHNAME", settings.PrimaryBatchName);
        writer.WriteElementString("OPENINGBALANCE", FormatMasterQuantity(item.OpeningBalance, item.BaseUnit));
        writer.WriteElementString("OPENINGVALUE", "-0");
        writer.WriteElementString("OPENINGRATE", "0/" + item.BaseUnit);
        writer.WriteEndElement();
    }

    private static void WriteMasterItemRateDetail(XmlWriter writer, string dutyHead, decimal rate, bool basedOnValue)
    {
        writer.WriteStartElement("RATEDETAILS.LIST");
        writer.WriteElementString("GSTRATEDUTYHEAD", dutyHead);
        if (dutyHead == "IGST" && basedOnValue)
        {
            writer.WriteElementString("GSTRATE", TallyNumericHelper.FormatGstRate(rate));
            writer.WriteElementString("GSTRATEVALUATIONTYPE", "Based on Value");
        }
        else
        {
            writer.WriteElementString("GSTRATEVALUATIONTYPE", basedOnValue ? "Based on Value" : "Not Applicable");
            if (basedOnValue) writer.WriteElementString("GSTRATE", TallyNumericHelper.FormatGstRate(rate));
        }
        writer.WriteEndElement();
    }

    private static void WriteLanguageName(XmlWriter writer, string name)
    {
        writer.WriteStartElement("LANGUAGENAME.LIST");
        writer.WriteStartElement("NAME.LIST");
        writer.WriteAttributeString("TYPE", "String");
        writer.WriteElementString("NAME", name);
        writer.WriteEndElement();
        writer.WriteElementString("LANGUAGEID", "1033");
        writer.WriteEndElement();
    }

    private static void WriteLedGstRegDetails(XmlWriter writer, CustomerMasterExport customer, TallyCompanySettings settings)
    {
        writer.WriteStartElement("LEDGSTREGDETAILS.LIST");
        writer.WriteElementString("APPLICABLEFROM", settings.MasterApplicableFrom);
        writer.WriteElementString("GSTREGISTRATIONTYPE", customer.GstRegistrationType);
        writer.WriteElementString("PLACEOFSUPPLY", customer.State);
        writer.WriteElementString("GSTIN", customer.Gstin);
        writer.WriteElementString("ISOTHTERRITORYASSESSEE", "No");
        writer.WriteElementString("CONSIDERPURCHASEFOREXPORT", "No");
        writer.WriteElementString("ISTRANSPORTER", "No");
        writer.WriteElementString("ISCOMMONPARTY", "No");
        writer.WriteEndElement();
    }

    private static void WriteLedMailingDetails(XmlWriter writer, CustomerMasterExport customer, TallyCompanySettings settings)
    {
        writer.WriteStartElement("LEDMAILINGDETAILS.LIST");
        writer.WriteStartElement("ADDRESS.LIST");
        writer.WriteAttributeString("TYPE", "String");
        foreach (string address in customer.AddressLines)
        {
            writer.WriteElementString("ADDRESS", address);
        }
        writer.WriteFullEndElement();
        writer.WriteElementString("APPLICABLEFROM", settings.MasterApplicableFrom);
        writer.WriteElementString("PINCODE", customer.Pincode);
        writer.WriteElementString("MAILINGNAME", customer.Name);
        writer.WriteElementString("STATE", customer.State);
        writer.WriteElementString("COUNTRY", "India");
        writer.WriteEndElement();
    }

    private static void WriteStockItemFlags(XmlWriter writer)
    {
        string[] firstNo =
        {
            "ISCOSTCENTRESON", "ISBATCHWISEON", "ISPERISHABLEON", "ISENTRYTAXAPPLICABLE", "ISCOSTTRACKINGON", "ISUPDATINGTARGETID", "ISDELETED",
            "ISSECURITYONWHENENTERED"
        };
        foreach (string flag in firstNo) writer.WriteElementString(flag, "No");
        writer.WriteElementString("ASORIGINAL", "Yes");
        string[] remainingNo =
        {
            "ISRATEINCLUSIVEVAT", "IGNOREPHYSICALDIFFERENCE", "IGNORENEGATIVESTOCK", "TREATSALESASMANUFACTURED",
            "TREATPURCHASESASCONSUMED", "TREATREJECTSASSCRAP", "HASMFGDATE", "ALLOWUSEOFEXPIREDITEMS", "IGNOREBATCHES", "IGNOREGODOWNS",
            "ADJDIFFINFIRSTSALELEDGER", "ADJDIFFINFIRSTPURCLEDGER", "CALCONMRP", "EXCLUDEJRNLFORVALUATION", "ISMRPINCLOFTAX", "ISADDLTAXEXEMPT",
            "ISSUPPLEMENTRYDUTYON", "GVATISEXCISEAPPL", "ISADDITIONALTAX", "ISCESSEXEMPTED", "REORDERASHIGHER", "MINORDERASHIGHER",
            "ISEXCISECALCULATEONMRP", "INCLUSIVETAX", "GSTCALCSLABONMRP", "MODIFYMRPRATE"
        };
        foreach (string flag in remainingNo) writer.WriteElementString(flag, "No");
    }

    private static void WriteCustomerLedgerFlags(XmlWriter writer)
    {
        writer.WriteElementString("ISBILLWISEON", "Yes");
        string[] firstNo =
        {
            "ISCOSTCENTRESON", "ISINTERESTON", "ALLOWINMOBILE", "ISCOSTTRACKINGON", "ISBENEFICIARYCODEON", "ISEXPORTONVCHCREATE",
            "PLASINCOMEEXPENSE", "ISUPDATINGTARGETID", "ISDELETED", "ISSECURITYONWHENENTERED"
        };
        foreach (string flag in firstNo) writer.WriteElementString(flag, "No");
        writer.WriteElementString("ASORIGINAL", "Yes");
        string[] moreNo =
        {
            "ISCONDENSED", "AFFECTSSTOCK", "ISRATEINCLUSIVEVAT", "FORPAYROLL", "ISABCENABLED", "ISCREDITDAYSCHKON", "INTERESTONBILLWISE",
            "OVERRIDEINTEREST", "OVERRIDEADVINTEREST", "USEFORVAT", "IGNORETDSEXEMPT", "ISTCSAPPLICABLE", "ISTDSAPPLICABLE", "ISFBTAPPLICABLE",
            "ISGSTAPPLICABLE", "ISEXCISEAPPLICABLE", "ISTDSEXPENSE", "ISEDLIAPPLICABLE", "ISRELATEDPARTY", "USEFORESIELIGIBILITY",
            "ISINTERESTINCLLASTDAY", "APPROPRIATETAXVALUE", "ISBEHAVEASDUTY", "INTERESTINCLDAYOFADDITION", "INTERESTINCLDAYOFDEDUCTION",
            "ISOTHTERRITORYASSESSEE", "IGNOREMISMATCHWITHWARNING", "USEASNOTIONALBANK", "BEHAVEASPAYMENTGATEWAY", "OVERRIDECREDITLIMIT",
            "ISAGAINSTFORMC"
        };
        foreach (string flag in moreNo) writer.WriteElementString(flag, "No");
        writer.WriteElementString("ISCHEQUEPRINTINGENABLED", "Yes");
        string[] tailNo =
        {
            "ISPAYUPLOAD", "ISPAYBATCHONLYSAL", "ISBNFCODESUPPORTED", "ALLOWEXPORTWITHERRORS", "CONSIDERPURCHASEFOREXPORT", "ISTRANSPORTER",
            "ISECASHLEDGER", "USEFORNOTIONALITC", "ISECOMMOPERATOR", "OVERRIDEBASEDONREALIZATION", "ISECDIFFINSDATE", "SHOWINPAYSLIP",
            "USEFORGRATUITY", "ISTDSPROJECTED", "ISSALARYMULFILE", "FORSERVICETAX", "ISINPUTCREDIT", "ISEXEMPTED", "ISABATEMENTAPPLICABLE",
            "ISSTXPARTY", "ISSTXNONREALIZEDTYPE", "USEFORKKC", "USEFORSBC", "ISUSEDFORCVD", "LEDBELONGSTONONTAXABLE",
            "ISEXCISEMERCHANTEXPORTER", "ISPARTYEXEMPTED", "ISSEZPARTY", "TDSDEDUCTEEISSPECIALRATE", "ISECHEQUESUPPORTED", "ISEDDSUPPORTED",
            "HASECHEQUEDELIVERYMODE", "HASECHEQUEDELIVERYTO", "HASECHEQUEPRINTLOCATION", "HASECHEQUEPAYABLELOCATION", "HASECHEQUEBANKLOCATION",
            "HASEDDDELIVERYMODE", "HASEDDDELIVERYTO", "HASEDDPRINTLOCATION", "HASEDDPAYABLELOCATION", "HASEDDBANKLOCATION", "ISEBANKINGENABLED",
            "ISEXPORTFILEENCRYPTED", "ISBATCHENABLED", "ISPRODUCTCODEBASED", "HASEDDCITY", "HASECHEQUECITY", "ISFILENAMEFORMATSUPPORTED",
            "HASCLIENTCODE", "PAYINSISBATCHAPPLICABLE", "PAYINSISFILENUMAPP", "ISSALARYTRANSGROUPEDFORBRS", "ISEBANKINGSUPPORTED",
            "ISSCBUAE", "ISBANKSTATUSAPP", "ISSALARYGROUPED", "USEFORPURCHASETAX"
        };
        foreach (string flag in tailNo) writer.WriteElementString(flag, "No");
        writer.WriteElementString("AUDITED", "No");
    }

    private static void WriteOldAudit(XmlWriter writer)
    {
        writer.WriteStartElement("OLDAUDITENTRYIDS.LIST");
        writer.WriteAttributeString("TYPE", "Number");
        writer.WriteElementString("OLDAUDITENTRYIDS", "-1");
        writer.WriteEndElement();
    }

    private static void WriteEmptyList(XmlWriter writer, string name)
    {
        writer.WriteStartElement(name);
        writer.WriteFullEndElement();
    }

    private static void WriteEmptyLists(XmlWriter writer, params string[] names)
    {
        foreach (string name in names) WriteEmptyList(writer, name);
    }

    private static string FormatMasterQuantity(decimal quantity, string unit)
    {
        if (quantity == 0m) return "0 " + unit + " ";
        return TallyNumericHelper.FormatQuantity(quantity) + " " + unit + " ";
    }

    private static string GetReportingUqcName(string unit)
    {
        string normalized = TallyNameHelper.NormalizeUom(unit);
        if (normalized == "NOS") return "NOS-NUMBERS";
        if (normalized == "KGS") return "KGS-KILOGRAMS";
        if (normalized == "GMS") return "GMS-GRAMMES";
        if (normalized == "LTR") return "LTR-LITRES";
        if (normalized == "MTR") return "MTR-METERS";
        if (normalized == "BAG") return "BAG-BAGS";
        if (normalized == "BOX") return "BOX-BOX";
        if (normalized == "TON") return "TON-TONNES";
        if (normalized == "SET") return "SET-SETS";
        return normalized;
    }

    private static void StartTallyMessage(XmlWriter writer)
    {
        writer.WriteStartElement("TALLYMESSAGE");
        writer.WriteAttributeString("xmlns", "UDF", null, "TallyUDF");
    }

    private static void WriteValidated(string finalPath, Action<XmlWriter> write)
    {
        string tempPath = finalPath + ".tmp";
        if (File.Exists(tempPath)) File.Delete(tempPath);
        using (XmlWriter writer = XmlWriter.Create(tempPath, Settings))
        {
            write(writer);
        }

        using (XmlReader reader = XmlReader.Create(tempPath))
        {
            while (reader.Read())
            {
            }
        }

        if (File.Exists(finalPath)) throw new IOException("Export file already exists: " + finalPath);
        File.Move(tempPath, finalPath);
    }
}

public sealed class ReferenceTallySalesXmlWriter
{
    public void WriteEnvelopeStart(XmlWriter writer)
    {
        writer.WriteStartElement("ENVELOPE");
        writer.WriteStartElement("HEADER");
        writer.WriteElementString("TALLYREQUEST", "Import Data");
        writer.WriteEndElement();
        writer.WriteStartElement("BODY");
    }

    public void WriteEnvelopeEnd(XmlWriter writer)
    {
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    public void WriteVoucher(XmlWriter writer, SalesExportInvoice invoice, TallyCompanySettings settings, TallyExportOptions options)
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
        WriteVoucherTailBeforeLedgers(writer);
        WritePartyLedgerEntry(writer, invoice, partyLedger);
        if (invoice.Cgst != 0m) WriteTaxLedgerEntry(writer, settings.CGSTLedgerName, invoice.Cgst);
        if (invoice.Sgst != 0m) WriteTaxLedgerEntry(writer, settings.SGSTLedgerName, invoice.Sgst);
        if (invoice.Igst != 0m) WriteTaxLedgerEntry(writer, settings.IGSTLedgerName, invoice.Igst);
        if (invoice.RoundOff != 0m) WriteRoundOffLedgerEntry(writer, settings.RoundOffLedgerName, invoice.RoundOff);
        WriteVoucherTail(writer);

        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    public void WriteVoucherHeader(XmlWriter writer, SalesExportInvoice invoice, string partyLedger, TallyCompanySettings settings)
    {
        string date = invoice.Date.ToString("yyyyMMdd");
        string gstin = invoice.CustomerGSTIN;
        string billPlace = FirstNonEmpty(invoice.CustomerCity, invoice.CustomerState, settings.CompanyState);

        writer.WriteStartElement("OLDAUDITENTRYIDS.LIST");
        writer.WriteAttributeString("TYPE", "Number");
        writer.WriteElementString("OLDAUDITENTRYIDS", "-1");
        writer.WriteEndElement();
        writer.WriteElementString("DATE", date);
        writer.WriteElementString("VCHSTATUSDATE", date);
        writer.WriteElementString("GSTREGISTRATIONTYPE", invoice.GstRegistrationType);
        writer.WriteElementString("VATDEALERTYPE", invoice.GstRegistrationType);
        writer.WriteElementString("STATENAME", invoice.CustomerState);
        writer.WriteElementString("NARRATION", "Generated from RRE sales " + invoice.SalesId + " " + invoice.PaymentMode);
        writer.WriteElementString("COUNTRYOFRESIDENCE", "India");
        writer.WriteElementString("PARTYGSTIN", gstin);
        writer.WriteElementString("PLACEOFSUPPLY", invoice.CustomerState);
        writer.WriteElementString("PARTYNAME", partyLedger);
        writer.WriteStartElement("GSTREGISTRATION");
        writer.WriteAttributeString("TAXTYPE", "GST");
        writer.WriteAttributeString("TAXREGISTRATION", settings.CompanyGSTIN);
        writer.WriteString(settings.CompanyGSTRegistrationName);
        writer.WriteEndElement();
        writer.WriteElementString("CMPGSTIN", settings.CompanyGSTIN);
        writer.WriteElementString("VOUCHERTYPENAME", "Sales");
        writer.WriteElementString("PARTYLEDGERNAME", partyLedger);
        writer.WriteElementString("VOUCHERNUMBER", invoice.SalesId);
        writer.WriteElementString("BASICBUYERNAME", partyLedger);
        writer.WriteElementString("CMPGSTREGISTRATIONTYPE", string.IsNullOrWhiteSpace(settings.CompanyGSTIN) ? "" : "Regular");
        writer.WriteElementString("PARTYMAILINGNAME", partyLedger);
        writer.WriteElementString("PARTYPINCODE", invoice.Pincode);
        writer.WriteElementString("BILLTOPLACE", billPlace);
        WriteDispatchAddress(writer, settings);
        writer.WriteElementString("DISPATCHFROMNAME", settings.DispatchFromName);
        writer.WriteElementString("DISPATCHFROMSTATENAME", settings.CompanyState);
        writer.WriteElementString("DISPATCHFROMPINCODE", settings.DispatchFromPincode);
        writer.WriteElementString("DISPATCHFROMPLACE", settings.DispatchFromPlace);
        WriteAddressList(writer, "BASICBUYERADDRESS.LIST", "BASICBUYERADDRESS", invoice);
        WriteAddressList(writer, "ADDRESS.LIST", "ADDRESS", invoice);
        writer.WriteElementString("SHIPTOPLACE", billPlace);
        writer.WriteElementString("CONSIGNEEGSTIN", gstin);
        writer.WriteElementString("CONSIGNEEMAILINGNAME", partyLedger);
        writer.WriteElementString("CONSIGNEEPINCODE", invoice.Pincode);
        writer.WriteElementString("CONSIGNEESTATENAME", invoice.CustomerState);
        writer.WriteElementString("CMPGSTSTATE", settings.CompanyState);
        writer.WriteElementString("CONSIGNEECOUNTRYNAME", "India");
        writer.WriteElementString("BASICBASEPARTYNAME", partyLedger);
        writer.WriteElementString("NUMBERINGSTYLE", "Auto Retain");
        writer.WriteElementString("CSTFORMISSUETYPE", "Not Applicable");
        writer.WriteElementString("CSTFORMRECVTYPE", "Not Applicable");
        writer.WriteElementString("FBTPAYMENTTYPE", "Default");
        writer.WriteElementString("PERSISTEDVIEW", "Invoice Voucher View");
        writer.WriteElementString("VCHSTATUSTAXADJUSTMENT", "Default");
        writer.WriteElementString("VCHSTATUSVOUCHERTYPE", "Sales");
        writer.WriteElementString("VCHSTATUSTAXUNIT", settings.CompanyGSTRegistrationName);
        writer.WriteElementString("VCHGSTCLASS", "Not Applicable");
        writer.WriteElementString("VCHENTRYMODE", "Item Invoice");
        WriteVoucherFlags(writer, date);
        WriteEwayDetails(writer, invoice, settings);
        WriteEmptyList(writer, "EXCLUDEDTAXATIONS.LIST");
        WriteEmptyList(writer, "OLDAUDITENTRIES.LIST");
        WriteEmptyList(writer, "ACCOUNTAUDITENTRIES.LIST");
        WriteEmptyList(writer, "AUDITENTRIES.LIST");
        WriteEmptyList(writer, "DUTYHEADDETAILS.LIST");
        WriteEmptyList(writer, "GSTADVADJDETAILS.LIST");
    }

    public void WriteInventoryEntry(XmlWriter writer, SalesExportItem item, TallyCompanySettings settings)
    {
        writer.WriteStartElement("ALLINVENTORYENTRIES.LIST");
        writer.WriteElementString("STOCKITEMNAME", item.ProductTallyName);
        writer.WriteElementString("GSTOVRDNINELIGIBLEITC", "Not Applicable");
        writer.WriteElementString("GSTOVRDNISREVCHARGEAPPL", "Not Applicable");
        writer.WriteElementString("GSTOVRDNTAXABILITY", item.GstRate > 0m ? "Taxable" : "Exempt");
        writer.WriteElementString("GSTSOURCETYPE", "Stock Item");
        writer.WriteElementString("GSTITEMSOURCE", item.ProductTallyName);
        writer.WriteElementString("HSNSOURCETYPE", "Stock Item");
        writer.WriteElementString("HSNITEMSOURCE", item.ProductTallyName);
        writer.WriteElementString("GSTOVRDNSTOREDNATURE", "");
        writer.WriteElementString("GSTOVRDNTYPEOFSUPPLY", "Goods");
        writer.WriteElementString("GSTRATEINFERAPPLICABILITY", "As per Masters/Company");
        writer.WriteElementString("GSTHSNNAME", item.Hsn);
        writer.WriteElementString("GSTHSNINFERAPPLICABILITY", "As per Masters/Company");
        writer.WriteElementString("ISDEEMEDPOSITIVE", "No");
        writer.WriteElementString("ISGSTASSESSABLEVALUEOVERRIDDEN", "No");
        writer.WriteElementString("STRDISGSTAPPLICABLE", "No");
        writer.WriteElementString("CONTENTNEGISPOS", "No");
        writer.WriteElementString("ISLASTDEEMEDPOSITIVE", "No");
        writer.WriteElementString("ISAUTONEGATE", "No");
        writer.WriteElementString("ISCUSTOMSCLEARANCE", "No");
        writer.WriteElementString("ISTRACKCOMPONENT", "No");
        writer.WriteElementString("ISTRACKPRODUCTION", "No");
        writer.WriteElementString("ISPRIMARYITEM", "No");
        writer.WriteElementString("ISSCRAP", "No");
        writer.WriteElementString("RATE", TallyNumericHelper.FormatAmount(item.Rate) + "/" + item.Uom);
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(item.TaxableAmount));
        writer.WriteElementString("ACTUALQTY", TallyNumericHelper.FormatQuantity(item.Quantity) + " " + item.Uom);
        writer.WriteElementString("BILLEDQTY", TallyNumericHelper.FormatQuantity(item.Quantity) + " " + item.Uom);
        WriteBatchAllocation(writer, item, settings);
        WriteAccountingAllocation(writer, item);
        WriteEmptyList(writer, "DUTYHEADDETAILS.LIST");
        WriteItemRateDetails(writer, item);
        WriteEmptyList(writer, "SUPPLEMENTARYDUTYHEADDETAILS.LIST");
        WriteEmptyList(writer, "TAXOBJECTALLOCATIONS.LIST");
        WriteEmptyList(writer, "REFVOUCHERDETAILS.LIST");
        WriteEmptyList(writer, "EXCISEALLOCATIONS.LIST");
        WriteEmptyList(writer, "EXPENSEALLOCATIONS.LIST");
        writer.WriteEndElement();
    }

    public void WriteBatchAllocation(XmlWriter writer, SalesExportItem item, TallyCompanySettings settings)
    {
        writer.WriteStartElement("BATCHALLOCATIONS.LIST");
        writer.WriteElementString("GODOWNNAME", settings.MainGodownName);
        writer.WriteElementString("BATCHNAME", settings.PrimaryBatchName);
        writer.WriteElementString("INDENTNO", "Not Applicable");
        writer.WriteElementString("ORDERNO", "Not Applicable");
        writer.WriteElementString("TRACKINGNUMBER", "Not Applicable");
        writer.WriteElementString("DYNAMICCSTISCLEARED", "No");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(item.TaxableAmount));
        writer.WriteElementString("ACTUALQTY", TallyNumericHelper.FormatQuantity(item.Quantity) + " " + item.Uom);
        writer.WriteElementString("BILLEDQTY", TallyNumericHelper.FormatQuantity(item.Quantity) + " " + item.Uom);
        WriteEmptyList(writer, "ADDITIONALDETAILS.LIST");
        WriteEmptyList(writer, "VOUCHERCOMPONENTLIST.LIST");
        writer.WriteEndElement();
    }

    public void WriteAccountingAllocation(XmlWriter writer, SalesExportItem item)
    {
        writer.WriteStartElement("ACCOUNTINGALLOCATIONS.LIST");
        writer.WriteStartElement("OLDAUDITENTRYIDS.LIST");
        writer.WriteAttributeString("TYPE", "Number");
        writer.WriteElementString("OLDAUDITENTRYIDS", "-1");
        writer.WriteEndElement();
        writer.WriteElementString("LEDGERNAME", item.SalesLedgerName);
        writer.WriteElementString("GSTCLASS", "Not Applicable");
        writer.WriteElementString("ISDEEMEDPOSITIVE", "No");
        writer.WriteElementString("LEDGERFROMITEM", "No");
        writer.WriteElementString("REMOVEZEROENTRIES", "No");
        writer.WriteElementString("ISPARTYLEDGER", "No");
        writer.WriteElementString("GSTOVERRIDDEN", "No");
        writer.WriteElementString("ISGSTASSESSABLEVALUEOVERRIDDEN", "No");
        writer.WriteElementString("STRDISGSTAPPLICABLE", "No");
        writer.WriteElementString("STRDGSTISPARTYLEDGER", "No");
        writer.WriteElementString("STRDGSTISDUTYLEDGER", "No");
        writer.WriteElementString("CONTENTNEGISPOS", "No");
        writer.WriteElementString("ISLASTDEEMEDPOSITIVE", "No");
        writer.WriteElementString("ISCAPVATTAXALTERED", "No");
        writer.WriteElementString("ISCAPVATNOTCLAIMED", "No");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(item.TaxableAmount));
        WriteLedgerEntryEmptyLists(writer);
        writer.WriteEndElement();
    }

    public void WriteItemRateDetails(XmlWriter writer, SalesExportItem item)
    {
        WriteItemRateDetail(writer, "CGST", item.GstRate / 2m, true);
        WriteItemRateDetail(writer, "SGST/UTGST", item.GstRate / 2m, true);
        WriteItemRateDetail(writer, "IGST", item.GstRate, true);
        WriteItemRateDetail(writer, "Cess", 0m, false);
        WriteItemRateDetail(writer, "State Cess", 0m, true);
    }

    public void WritePartyLedgerEntry(XmlWriter writer, SalesExportInvoice invoice, string partyLedger)
    {
        decimal amount = -invoice.CalculatedTotal;
        writer.WriteStartElement("LEDGERENTRIES.LIST");
        WriteOldAudit(writer);
        writer.WriteElementString("LEDGERNAME", partyLedger);
        writer.WriteElementString("GSTCLASS", "Not Applicable");
        writer.WriteElementString("ISDEEMEDPOSITIVE", "Yes");
        writer.WriteElementString("LEDGERFROMITEM", "No");
        writer.WriteElementString("REMOVEZEROENTRIES", "No");
        writer.WriteElementString("ISPARTYLEDGER", "Yes");
        WriteCommonLedgerFlags(writer, "Yes");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(amount));
        WriteEmptyList(writer, "SERVICETAXDETAILS.LIST");
        WriteEmptyList(writer, "BANKALLOCATIONS.LIST");
        writer.WriteStartElement("BILLALLOCATIONS.LIST");
        writer.WriteElementString("NAME", invoice.SalesId);
        writer.WriteElementString("BILLTYPE", "New Ref");
        writer.WriteElementString("TDSDEDUCTEEISSPECIALRATE", "No");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(amount));
        WriteEmptyList(writer, "INTERESTCOLLECTION.LIST");
        WriteEmptyList(writer, "STBILLCATEGORIES.LIST");
        writer.WriteEndElement();
        WriteLedgerEntryEmptyListsAfterBills(writer);
        writer.WriteEndElement();
    }

    public void WriteTaxLedgerEntry(XmlWriter writer, string ledgerName, decimal amount)
    {
        writer.WriteStartElement("LEDGERENTRIES.LIST");
        WriteOldAudit(writer);
        writer.WriteElementString("APPROPRIATEFOR", "Not Applicable");
        writer.WriteElementString("ROUNDTYPE", "Not Applicable");
        writer.WriteElementString("LEDGERNAME", ledgerName);
        writer.WriteElementString("GSTCLASS", "Not Applicable");
        writer.WriteElementString("ISDEEMEDPOSITIVE", "No");
        writer.WriteElementString("LEDGERFROMITEM", "No");
        writer.WriteElementString("REMOVEZEROENTRIES", "No");
        writer.WriteElementString("ISPARTYLEDGER", "No");
        WriteCommonLedgerFlags(writer, "No");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(amount));
        writer.WriteElementString("VATEXPAMOUNT", TallyNumericHelper.FormatAmount(amount));
        WriteLedgerEntryEmptyLists(writer);
        writer.WriteEndElement();
    }

    public void WriteRoundOffLedgerEntry(XmlWriter writer, string ledgerName, decimal amount)
    {
        writer.WriteStartElement("LEDGERENTRIES.LIST");
        WriteOldAudit(writer);
        writer.WriteElementString("LEDGERNAME", ledgerName);
        writer.WriteElementString("ISDEEMEDPOSITIVE", "No");
        writer.WriteElementString("LEDGERFROMITEM", "No");
        writer.WriteElementString("REMOVEZEROENTRIES", "No");
        writer.WriteElementString("ISPARTYLEDGER", "No");
        WriteCommonLedgerFlags(writer, "Yes");
        writer.WriteElementString("AMOUNT", TallyNumericHelper.FormatAmount(amount));
        writer.WriteElementString("VATEXPAMOUNT", TallyNumericHelper.FormatAmount(amount));
        WriteLedgerEntryEmptyLists(writer);
        writer.WriteEndElement();
    }

    public void WriteVoucherTail(XmlWriter writer)
    {
        writer.WriteStartElement("GST.LIST");
        writer.WriteElementString("PURPOSETYPE", "GST");
        writer.WriteStartElement("STAT.LIST");
        writer.WriteElementString("PURPOSETYPE", "GST");
        writer.WriteElementString("ISFETCHEDONLY", "No");
        writer.WriteElementString("ISDELETED", "No");
        WriteEmptyList(writer, "TALLYCONTENTUSER.LIST");
        writer.WriteEndElement();
        writer.WriteEndElement();
        WriteEmptyList(writer, "PAYROLLMODEOFPAYMENT.LIST");
        WriteEmptyList(writer, "ATTDRECORDS.LIST");
        WriteEmptyList(writer, "GSTEWAYCONSIGNORADDRESS.LIST");
        WriteEmptyList(writer, "GSTEWAYCONSIGNEEADDRESS.LIST");
        WriteEmptyList(writer, "TEMPGSTRATEDETAILS.LIST");
        WriteEmptyList(writer, "TEMPGSTADVADJUSTED.LIST");
    }

    private static void WriteVoucherTailBeforeLedgers(XmlWriter writer)
    {
        WriteEmptyList(writer, "CONTRITRANS.LIST");
        WriteEmptyList(writer, "EWAYBILLERRORLIST.LIST");
        WriteEmptyList(writer, "IRNERRORLIST.LIST");
        WriteEmptyList(writer, "HARYANAVAT.LIST");
        WriteEmptyList(writer, "SUPPLEMENTARYDUTYHEADDETAILS.LIST");
        WriteEmptyList(writer, "INVOICEDELNOTES.LIST");
        WriteEmptyList(writer, "INVOICEORDERLIST.LIST");
        WriteEmptyList(writer, "INVOICEINDENTLIST.LIST");
        WriteEmptyList(writer, "ATTENDANCEENTRIES.LIST");
        WriteEmptyList(writer, "ORIGINVOICEDETAILS.LIST");
        WriteEmptyList(writer, "INVOICEEXPORTLIST.LIST");
    }

    private static void WriteVoucherFlags(XmlWriter writer, string date)
    {
        string[] names =
        {
            "DIFFACTUALQTY", "ISMSTFROMSYNC", "ISDELETED", "ISSECURITYONWHENENTERED", "ASORIGINAL", "AUDITED", "ISCOMMONPARTY", "FORJOBCOSTING", "ISOPTIONAL"
        };
        foreach (string name in names) writer.WriteElementString(name, "No");
        writer.WriteElementString("EFFECTIVEDATE", date);
        string[] moreNo =
        {
            "USEFOREXCISE", "ISFORJOBWORKIN", "ALLOWCONSUMPTION", "USEFORINTEREST", "USEFORGAINLOSS", "USEFORGODOWNTRANSFER", "USEFORCOMPOUND",
            "USEFORSERVICETAX", "ISREVERSECHARGEAPPLICABLE", "ISSYSTEM", "ISFETCHEDONLY", "ISGSTOVERRIDDEN", "ISCANCELLED", "ISONHOLD",
            "ISSUMMARY", "ISECOMMERCESUPPLY", "ISBOENOTAPPLICABLE", "ISGSTSECSEVENAPPLICABLE", "IGNOREEINVVALIDATION",
            "CMPGSTISOTHTERRITORYASSESSEE", "PARTYGSTISOTHTERRITORYASSESSEE", "IRNJSONEXPORTED", "IRNCANCELLED", "IGNOREGSTCONFLICTINMIG",
            "ISOPBALTRANSACTION", "IGNOREGSTFORMATVALIDATION"
        };
        foreach (string name in moreNo) writer.WriteElementString(name, "No");
        writer.WriteElementString("ISELIGIBLEFORITC", "Yes");
        string[] tailNo =
        {
            "UPDATESUMMARYVALUES", "ISEWAYBILLAPPLICABLE", "ISDELETEDRETAINED", "ISNULL", "ISEXCISEVOUCHER", "EXCISETAXOVERRIDE",
            "USEFORTAXUNITTRANSFER", "ISEXER1NOPOVERWRITE", "ISEXF2NOPOVERWRITE", "ISEXER3NOPOVERWRITE", "IGNOREPOSVALIDATION",
            "EXCISEOPENING", "USEFORFINALPRODUCTION", "ISTDSOVERRIDDEN", "ISTCSOVERRIDDEN", "ISTDSTCSCASHVCH", "INCLUDEADVPYMTVCH",
            "ISSUBWORKSCONTRACT", "ISVATOVERRIDDEN", "IGNOREORIGVCHDATE", "ISVATPAIDATCUSTOMS", "ISDECLAREDTOCUSTOMS", "VATADVANCEPAYMENT",
            "VATADVPAY", "ISCSTDELCAREDGOODSSALES", "ISVATRESTAXINV", "ISSERVICETAXOVERRIDDEN", "ISISDVOUCHER", "ISEXCISEOVERRIDDEN",
            "ISEXCISESUPPLYVCH", "GSTNOTEXPORTED", "IGNOREGSTINVALIDATION", "ISGSTREFUND", "OVRDNEWAYBILLAPPLICABILITY",
            "ISVATPRINCIPALACCOUNT", "VCHSTATUSISVCHNUMUSED", "VCHGSTSTATUSISINCLUDED"
        };
        foreach (string name in tailNo) writer.WriteElementString(name, "No");
        writer.WriteElementString("VCHGSTSTATUSISUNCERTAIN", "Yes");
        writer.WriteElementString("VCHGSTSTATUSISEXCLUDED", "No");
        writer.WriteElementString("VCHGSTSTATUSISAPPLICABLE", "Yes");
        string[] finalNo =
        {
            "VCHGSTSTATUSISGSTR2BRECONCILED", "VCHGSTSTATUSISGSTR2BONLYINPORTAL", "VCHGSTSTATUSISGSTR2BONLYINBOOKS", "VCHGSTSTATUSISGSTR2BMISMATCH",
            "VCHGSTSTATUSISGSTR2BINDIFFPERIOD", "VCHGSTSTATUSISRETEFFDATEOVERRDN", "VCHGSTSTATUSISOVERRDN", "VCHGSTSTATUSISSTATINDIFFDATE",
            "VCHGSTSTATUSISRETINDIFFDATE", "VCHGSTSTATUSMAINSECTIONEXCLUDED", "VCHGSTSTATUSISBRANCHTRANSFEROUT", "VCHGSTSTATUSISSYSTEMSUMMARY",
            "VCHSTATUSISUNREGISTEREDRCM", "VCHSTATUSISOPTIONAL", "VCHSTATUSISCANCELLED", "VCHSTATUSISDELETED", "VCHSTATUSISOPENINGBALANCE",
            "VCHSTATUSISFETCHEDONLY", "PAYMENTLINKHASMULTIREF", "ISSHIPPINGWITHINSTATE", "ISOVERSEASTOURISTTRANS", "ISDESIGNATEDZONEPARTY",
            "HASCASHFLOW", "ISPOSTDATED", "USETRACKINGNUMBER"
        };
        foreach (string name in finalNo) writer.WriteElementString(name, "No");
        writer.WriteElementString("ISINVOICE", "Yes");
        string[] invoiceNo =
        {
            "MFGJOURNAL", "HASDISCOUNTS", "ASPAYSLIP", "ISCOSTCENTRE", "ISSTXNONREALIZEDVCH", "ISEXCISEMANUFACTURERON", "ISBLANKCHEQUE",
            "ISVOID", "ORDERLINESTATUS", "VATISAGNSTCANCSALES", "VATISPURCEXEMPTED", "ISVATRESTAXINVOICE", "VATISASSESABLECALCVCH"
        };
        foreach (string name in invoiceNo) writer.WriteElementString(name, "No");
        writer.WriteElementString("ISVATDUTYPAID", "Yes");
        writer.WriteElementString("ISDELIVERYSAMEASCONSIGNEE", "No");
        writer.WriteElementString("ISDISPATCHSAMEASCONSIGNOR", "No");
        writer.WriteElementString("ISDELETEDVCHRETAINED", "No");
        writer.WriteElementString("CHANGEVCHMODE", "No");
        writer.WriteElementString("RESETIRNQRCODE", "No");
        writer.WriteElementString("VOUCHERNUMBERSERIES", "Default");
    }

    private static void WriteEwayDetails(XmlWriter writer, SalesExportInvoice invoice, TallyCompanySettings settings)
    {
        writer.WriteStartElement("EWAYBILLDETAILS.LIST");
        writer.WriteElementString("DOCUMENTTYPE", "Tax Invoice");
        writer.WriteElementString("CONSIGNEEPINCODE", invoice.Pincode);
        writer.WriteElementString("SUBTYPE", "Supply");
        writer.WriteElementString("CONSIGNORPINCODE", settings.DispatchFromPincode);
        writer.WriteElementString("SHIPPEDFROMSTATE", settings.CompanyState);
        writer.WriteElementString("SHIPPEDTOSTATE", invoice.CustomerState);
        string[] flags = { "ISCANCELLED", "IGNOREGSTINVALIDATION", "ISCANCELPENDING", "IGNOREGENERATIONVALIDATION", "ISEXPORTEDFORGENERATION", "INTRASTATEAPPLICABILITY" };
        foreach (string flag in flags) writer.WriteElementString(flag, "No");
        writer.WriteStartElement("TRANSPORTDETAILS.LIST");
        writer.WriteElementString("OLDVEHICLETYPE", "Not Applicable");
        string[] transport = { "IGNOREVEHICLENOVALIDATION", "ISTRANSIDPENDING", "ISTRANSIDUPDATED", "IGNORETRANSIDVALIDATION", "ISEXPORTEDFORTRANSPORTERID", "ISPARTBPENDING", "ISPARTBUPDATED", "IGNOREPARTBVALIDATION", "ISEXPORTEDFORPARTB" };
        foreach (string flag in transport) writer.WriteElementString(flag, "No");
        writer.WriteEndElement();
        WriteEmptyList(writer, "EXTENSIONDETAILS.LIST");
        WriteEmptyList(writer, "MULTIVEHICLEDETAILS.LIST");
        WriteEmptyList(writer, "STATEWISETHRESHOLD.LIST");
        writer.WriteEndElement();
    }

    private static void WriteDispatchAddress(XmlWriter writer, TallyCompanySettings settings)
    {
        writer.WriteStartElement("DISPATCHFROMADDRESS.LIST");
        writer.WriteAttributeString("TYPE", "String");
        WriteOptionalRepeated(writer, "DISPATCHFROMADDRESS", settings.DispatchFromAddress1);
        WriteOptionalRepeated(writer, "DISPATCHFROMADDRESS", settings.DispatchFromAddress2);
        WriteOptionalRepeated(writer, "DISPATCHFROMADDRESS", settings.DispatchFromAddress3);
        writer.WriteFullEndElement();
    }

    private static void WriteAddressList(XmlWriter writer, string listName, string itemName, SalesExportInvoice invoice)
    {
        writer.WriteStartElement(listName);
        writer.WriteAttributeString("TYPE", "String");
        WriteOptionalRepeated(writer, itemName, invoice.CustomerAddress1);
        WriteOptionalRepeated(writer, itemName, invoice.CustomerAddress2);
        string cityLine = FirstNonEmpty(invoice.CustomerCity, invoice.CustomerState);
        WriteOptionalRepeated(writer, itemName, cityLine);
        writer.WriteFullEndElement();
    }

    private static void WriteCommonLedgerFlags(XmlWriter writer, string isLastDeemedPositive)
    {
        writer.WriteElementString("GSTOVERRIDDEN", "No");
        writer.WriteElementString("ISGSTASSESSABLEVALUEOVERRIDDEN", "No");
        writer.WriteElementString("STRDISGSTAPPLICABLE", "No");
        writer.WriteElementString("STRDGSTISPARTYLEDGER", "No");
        writer.WriteElementString("STRDGSTISDUTYLEDGER", "No");
        writer.WriteElementString("CONTENTNEGISPOS", "No");
        writer.WriteElementString("ISLASTDEEMEDPOSITIVE", isLastDeemedPositive);
        writer.WriteElementString("ISCAPVATTAXALTERED", "No");
        writer.WriteElementString("ISCAPVATNOTCLAIMED", "No");
    }

    private static void WriteLedgerEntryEmptyLists(XmlWriter writer)
    {
        WriteEmptyList(writer, "SERVICETAXDETAILS.LIST");
        WriteEmptyList(writer, "BANKALLOCATIONS.LIST");
        WriteEmptyList(writer, "BILLALLOCATIONS.LIST");
        WriteLedgerEntryEmptyListsAfterBills(writer);
    }

    private static void WriteLedgerEntryEmptyListsAfterBills(XmlWriter writer)
    {
        string[] names =
        {
            "INTERESTCOLLECTION.LIST", "OLDAUDITENTRIES.LIST", "ACCOUNTAUDITENTRIES.LIST", "AUDITENTRIES.LIST", "INPUTCRALLOCS.LIST",
            "DUTYHEADDETAILS.LIST", "EXCISEDUTYHEADDETAILS.LIST", "RATEDETAILS.LIST", "SUMMARYALLOCS.LIST", "CENVATDUTYALLOCATIONS.LIST",
            "STPYMTDETAILS.LIST", "EXCISEPAYMENTALLOCATIONS.LIST", "TAXBILLALLOCATIONS.LIST", "TAXOBJECTALLOCATIONS.LIST",
            "TDSEXPENSEALLOCATIONS.LIST", "VATSTATUTORYDETAILS.LIST", "COSTTRACKALLOCATIONS.LIST", "REFVOUCHERDETAILS.LIST",
            "INVOICEWISEDETAILS.LIST", "VATITCDETAILS.LIST", "ADVANCETAXDETAILS.LIST", "TAXTYPEALLOCATIONS.LIST"
        };
        foreach (string name in names) WriteEmptyList(writer, name);
    }

    private static void WriteItemRateDetail(XmlWriter writer, string dutyHead, decimal rate, bool basedOnValue)
    {
        writer.WriteStartElement("RATEDETAILS.LIST");
        writer.WriteElementString("GSTRATEDUTYHEAD", dutyHead);
        writer.WriteElementString("GSTRATEVALUATIONTYPE", basedOnValue ? "Based on Value" : "Not Applicable");
        if (basedOnValue && rate != 0m) writer.WriteElementString("GSTRATE", TallyNumericHelper.FormatGstRate(rate));
        if (basedOnValue && rate == 0m) writer.WriteElementString("GSTRATE", "0");
        writer.WriteEndElement();
    }

    private static void WriteOldAudit(XmlWriter writer)
    {
        writer.WriteStartElement("OLDAUDITENTRYIDS.LIST");
        writer.WriteAttributeString("TYPE", "Number");
        writer.WriteElementString("OLDAUDITENTRYIDS", "-1");
        writer.WriteEndElement();
    }

    private static void WriteEmptyList(XmlWriter writer, string name)
    {
        writer.WriteStartElement(name);
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
