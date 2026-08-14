using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace RRE_To_Tally;

public sealed class TallyExportService
{
    private readonly TallyDataRepository _repository;
    private readonly TallyExportValidator _validator = new TallyExportValidator();

    public TallyExportService(TallyDataRepository repository)
    {
        _repository = repository;
    }

    public async Task<TallyExportPackage> LoadPackageAsync(DateTime fromDate, DateTime toDate, string billNumber, TallyExportOptions options)
    {
        List<SalesExportRow> rows = await _repository.LoadSalesRowsAsync(fromDate, toDate, billNumber).ConfigureAwait(false);
        return BuildPackage(rows, options);
    }

    public TallyExportPackage BuildPackage(IList<SalesExportRow> rows, TallyExportOptions options)
    {
        TallyExportPackage package = new TallyExportPackage();
        TallyNumericHelper numeric = new TallyNumericHelper(package.Warnings);
        TallyCompanySettings settings = options.CompanySettings ?? TallyCompanySettings.Load();
        Dictionary<string, string> customerNameByIdentity = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, string> productNameByIdentity = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> usedCustomerNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> usedProductNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (IGrouping<string, SalesExportRow> group in rows.GroupBy(r => (r.DivisionKey ?? "") + "|" + (r.SalesId ?? ""), StringComparer.OrdinalIgnoreCase))
        {
            List<SalesExportRow> invoiceRows = group.ToList();
            SalesExportRow first = invoiceRows[0];
            SalesExportInvoice invoice = new SalesExportInvoice
            {
                DivisionKey = first.DivisionKey,
                DivisionName = first.DivisionName,
                DivisionCompanyName = first.DivisionCompanyName,
                SalesId = first.SalesId,
                ReferenceId = first.ReferenceId,
                Date = first.TransactionDate,
                CustomerId = first.CustomerId,
                CustomerName = TallyNameHelper.GetTallyCustomerName(first),
                CustomerAddress1 = TallyNameHelper.CleanXmlText(first.CustomerAddress1),
                CustomerAddress2 = TallyNameHelper.CleanXmlText(first.CustomerAddress2),
                CustomerCity = TallyNameHelper.CleanXmlText(first.CustomerCity),
                CustomerState = string.IsNullOrWhiteSpace(first.CustomerState) ? settings.CompanyState : TallyNameHelper.CleanTallyName(first.CustomerState),
                Pincode = TallyNameHelper.CleanXmlText(first.Pincode),
                PaymentMode = TallyNameHelper.CleanXmlText(first.PaymentMode),
                Discount = numeric.ParseDecimal(first.LessAmount),
                OtherCharges = numeric.ParseDecimal(first.OtherCharges)
            };
            decimal storedGrandTotal = numeric.ParseDecimal(first.GrandTotal);
            invoice.StoredGrandTotal = storedGrandTotal > 0m ? Round(storedGrandTotal) : 0m;

            invoice.CustomerLedgerName = ResolveUniqueName(customerNameByIdentity, usedCustomerNames, first.DivisionKey + "|" + first.CustomerId, invoice.CustomerName, first.CustomerId, package.Warnings, "Duplicate normalized customer name");
            string gstin = (first.CustomerGSTIN ?? "").Trim().ToUpperInvariant();
            if (gstin.Length > 0 && TallyNameHelper.IsBasicValidGstin(gstin))
            {
                invoice.CustomerGSTIN = gstin;
                invoice.GstRegistrationType = "Regular";
            }
            else
            {
                if (gstin.Length > 0) package.Warnings.Add("Invalid GSTIN for " + invoice.CustomerLedgerName + ": " + gstin);
                invoice.GstRegistrationType = "Unregistered/Consumer";
            }

            bool isInterstate = IsInterstateSale(first, settings);
            invoice.IsInterstate = isInterstate;
            foreach (SalesExportRow row in invoiceRows)
            {
                if (row.MissingProductMaster)
                {
                    invoice.Status = "Error";
                    invoice.ErrorMessage = AppendMessage(invoice.ErrorMessage, "Missing product master for product id " + row.ProductId);
                    package.Errors.Add(invoice.SalesId + ": missing product master " + row.ProductId);
                    continue;
                }

                string productName = TallyNameHelper.GetTallyProductName(row);
                if (string.IsNullOrWhiteSpace(productName))
                {
                    invoice.Status = "Error";
                    invoice.ErrorMessage = AppendMessage(invoice.ErrorMessage, "Missing product name for product id " + row.ProductId);
                    package.Errors.Add(invoice.SalesId + ": missing product name " + row.ProductId);
                    continue;
                }

                decimal gstRate = ResolveGstRate(row, numeric);
                decimal grossAmount = Round(numeric.ParseDecimal(row.Amount));
                decimal taxable = CalculateTaxableFromInclusive(grossAmount, gstRate);
                decimal gstAmount = Round(grossAmount - taxable);
                decimal quantity = numeric.ParseDecimal(row.Quantity);
                decimal rate = numeric.ParseDecimal(row.Rate);
                if (quantity != 0m)
                {
                    rate = Round(taxable / quantity);
                }
                else if (rate == 0m && taxable != 0m)
                {
                    package.Warnings.Add(invoice.SalesId + ": quantity missing for " + productName + ", cannot derive taxable unit rate");
                }
                decimal cgst = isInterstate ? 0m : Round(gstAmount / 2m);
                decimal sgst = isInterstate ? 0m : Round(gstAmount - cgst);
                decimal igst = isInterstate ? gstAmount : 0m;
                string unit = TallyNameHelper.NormalizeUom(row.Uom);
                if (string.IsNullOrWhiteSpace(row.Uom)) package.Warnings.Add(invoice.SalesId + ": missing UOM for " + productName + ", using NOS");
                if (string.IsNullOrWhiteSpace(row.Hsn)) package.Warnings.Add(invoice.SalesId + ": missing HSN for " + productName);
                if (!IsSupportedGstRate(gstRate)) package.Warnings.Add(invoice.SalesId + ": unsupported GST rate " + gstRate.ToString("0.##", CultureInfo.InvariantCulture));

                string productKey = row.DivisionKey + "|" + (!string.IsNullOrWhiteSpace(row.ProductId) ? row.ProductId : row.ItemCode);
                string productTallyName = ResolveUniqueName(productNameByIdentity, usedProductNames, productKey, productName, row.ItemCode.Length > 0 ? row.ItemCode : row.ProductId, package.Warnings, "Duplicate normalized product name");

                SalesExportItem item = new SalesExportItem
                {
                    ProductId = row.ProductId,
                    ItemCode = row.ItemCode,
                    ProductName = productName,
                    ProductTallyName = productTallyName,
                    Category = row.Category,
                    StockGroupName = string.IsNullOrWhiteSpace(row.Category) ? "RRE PRODUCTS" : TallyNameHelper.CleanTallyName(row.Category),
                    Uom = unit,
                    Hsn = TallyNameHelper.CleanTallyName(row.Hsn),
                    Rate = Round(rate),
                    Quantity = quantity,
                    TaxableAmount = taxable,
                    GstRate = gstRate,
                    CgstAmount = cgst,
                    SgstAmount = sgst,
                    IgstAmount = igst,
                    IsInterstate = isInterstate,
                    SalesLedgerName = TallyNameHelper.GetSalesLedgerName(gstRate, settings.SalesLedgerPrefix)
                };
                invoice.Items.Add(item);
                invoice.TaxableAmount += taxable;
                invoice.Cgst += cgst;
                invoice.Sgst += sgst;
                invoice.Igst += igst;
            }

            invoice.ItemCount = invoice.Items.Count;
            invoice.TaxableAmount = Round(invoice.TaxableAmount);
            invoice.Cgst = Round(invoice.Cgst);
            invoice.Sgst = Round(invoice.Sgst);
            invoice.Igst = Round(invoice.Igst);
            decimal grossDetailsTotal = Round(invoice.TaxableAmount + invoice.Cgst + invoice.Sgst + invoice.Igst);
            decimal unroundedVoucherTotal = Round(grossDetailsTotal - invoice.Discount + invoice.OtherCharges);
            decimal targetGrandTotal = invoice.StoredGrandTotal > 0m ? invoice.StoredGrandTotal : RoundWhole(unroundedVoucherTotal);
            invoice.RoundOff = Round(targetGrandTotal - unroundedVoucherTotal);
            invoice.CalculatedTotal = Round(unroundedVoucherTotal + invoice.RoundOff);
            invoice.Difference = Round(invoice.CalculatedTotal - targetGrandTotal);

            if (invoice.RoundOff != 0m)
            {
                invoice.Status = "Warning";
                package.Warnings.Add(invoice.SalesId + ": round off posted " + TallyNumericHelper.FormatAmount(invoice.RoundOff));
            }
            if (invoice.StoredGrandTotal == 0m) invoice.StoredGrandTotal = targetGrandTotal;

            ExportValidationResult validation = _validator.ValidateInvoice(invoice);
            foreach (string warning in validation.Warnings) package.Warnings.Add(invoice.SalesId + ": " + warning);
            foreach (string error in validation.Errors) package.Errors.Add(invoice.SalesId + ": " + error);
            if (!validation.IsValid)
            {
                invoice.Status = "Error";
                invoice.ErrorMessage = AppendMessage(invoice.ErrorMessage, string.Join(" ", validation.Errors.ToArray()));
            }

            invoice.Export = invoice.IsValid;
            package.Invoices.Add(invoice);
        }

        PrepareMasters(package, options, settings);
        return package;
    }

    public async Task<ExportSummary> ExportAsync(TallyExportPackage package, DateTime fromDate, DateTime toDate, string exportFolder, TallyExportOptions options, bool writeMasters, bool writeSales)
    {
        Directory.CreateDirectory(exportFolder);
        ExportSummary summary = new ExportSummary();
        List<SalesExportInvoice> selected = package.Invoices.Where(i => i.Export && i.IsValid).ToList();
        summary.InvoicesSelected = package.Invoices.Count(i => i.Export);
        summary.InvoicesSkipped = package.Invoices.Count - selected.Count;

        string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        summary.ErrorCsvPath = Path.Combine(exportFolder, "TallyExportErrors_" + stamp + ".csv");
        summary.LogPath = Path.Combine(exportFolder, "TallyExportLog_" + stamp + ".txt");
        TallyXmlWriter writer = new TallyXmlWriter();

        await Task.Run(delegate
        {
            if (writeMasters)
            {
                string mastersPath = GetExportPath(exportFolder, "RRE_Tally_Masters", fromDate, toDate);
                writer.WriteMastersXml(mastersPath, package, options);
                summary.MastersXmlPath = mastersPath;
            }

            if (writeSales)
            {
                foreach (IGrouping<string, SalesExportInvoice> divisionGroup in selected.GroupBy(i => i.DivisionKey, StringComparer.OrdinalIgnoreCase))
                {
                    SalesDivisionConfig division = SalesDivisionConfig.Find(divisionGroup.Key);
                    string salesPath = GetExportPath(exportFolder, division.FilePrefix + "_Sales", fromDate, toDate);
                    writer.WriteSalesXml(salesPath, divisionGroup.ToList(), options);
                    summary.SalesXmlPaths.Add(salesPath);
                }

                summary.SalesXmlPath = string.Join(Environment.NewLine, summary.SalesXmlPaths.ToArray());
                summary.InvoicesExported = selected.Count;
            }

            WriteLogs(summary, package, selected);
            summary.CustomersExported = package.Customers.Count;
            summary.ProductsExported = package.Products.Count;
            summary.Warnings = package.Warnings.Count;
            summary.Errors = package.Errors.Count;
        }).ConfigureAwait(false);

        return summary;
    }

    // SalesDetails.Amount is GST-inclusive. Tally inventory allocation receives the extracted taxable value.
    public decimal ResolveGstRate(SalesExportRow row, TallyNumericHelper numeric)
    {
        if (row.SalesDetailGst > 0m) return row.SalesDetailGst;
        decimal productGst = numeric.ParseGstRate(row.ProductGst);
        if (productGst > 0m) return productGst;
        decimal tax = numeric.ParseGstRate(row.Tax);
        if (tax > 0m) return tax;
        decimal igst = numeric.ParseGstRate(row.Igst);
        if (igst > 0m) return igst;
        return 0m;
    }

    private static void PrepareMasters(TallyExportPackage package, TallyExportOptions options, TallyCompanySettings settings)
    {
        HashSet<string> units = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> groups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> customers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> products = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> ledgers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        bool needsCgst = false;
        bool needsSgst = false;
        bool needsIgst = false;
        bool needsRoundOff = false;
        bool needsDiscount = false;
        bool needsOtherCharges = false;
        bool needsCash = false;

        foreach (SalesExportInvoice invoice in package.Invoices)
        {
            if (invoice.Items.Count == 0) continue;
            if (options.IncludeCustomerMasters && customers.Add(invoice.CustomerLedgerName))
            {
                CustomerMasterExport customer = new CustomerMasterExport
                {
                    Name = invoice.CustomerLedgerName,
                    State = invoice.CustomerState,
                    Pincode = invoice.Pincode,
                    Gstin = invoice.CustomerGSTIN,
                    GstRegistrationType = invoice.GstRegistrationType
                };
                AddAddress(customer, invoice.CustomerAddress1);
                AddAddress(customer, invoice.CustomerAddress2);
                AddAddress(customer, ((invoice.CustomerCity + " - " + invoice.Pincode).Trim(' ', '-')));
                package.Customers.Add(customer);
            }

            if (invoice.RoundOff != 0m) needsRoundOff = true;
            if (invoice.Discount != 0m) needsDiscount = true;
            if (invoice.OtherCharges != 0m) needsOtherCharges = true;
            if (options.CashSalesLedgerBehaviour == CashSalesLedgerBehaviour.UseRreCashLedger && invoice.PaymentMode.IndexOf("cash", StringComparison.OrdinalIgnoreCase) >= 0) needsCash = true;

            foreach (SalesExportItem item in invoice.Items)
            {
                if (units.Add(item.Uom)) package.Units.Add(new UnitMasterExport { Name = item.Uom, DecimalPlaces = TallyNameHelper.GetUnitDecimalPlaces(item.Uom) });
                if (groups.Add(item.StockGroupName)) package.StockGroups.Add(new StockGroupMasterExport { Name = item.StockGroupName });
                if (options.IncludeProductMasters && products.Add(item.ProductTallyName))
                {
                    package.Products.Add(new ProductMasterExport { Name = item.ProductTallyName, BaseUnit = item.Uom, StockGroupName = item.StockGroupName, Hsn = item.Hsn, GstRate = item.GstRate });
                }

                if (options.IncludeSalesLedgers && ledgers.Add(item.SalesLedgerName))
                {
                    package.Ledgers.Add(new LedgerMasterExport { Name = item.SalesLedgerName, Parent = "Sales Accounts" });
                }

                if (item.CgstAmount != 0m) needsCgst = true;
                if (item.SgstAmount != 0m) needsSgst = true;
                if (item.IgstAmount != 0m) needsIgst = true;
            }
        }

        if (options.IncludeGstLedgers)
        {
            if (needsCgst) package.Ledgers.Add(new LedgerMasterExport { Name = settings.CGSTLedgerName, Parent = "Duties & Taxes", TaxType = "GST", DutyHead = "Central Tax" });
            if (needsSgst) package.Ledgers.Add(new LedgerMasterExport { Name = settings.SGSTLedgerName, Parent = "Duties & Taxes", TaxType = "GST", DutyHead = "State Tax" });
            if (needsIgst) package.Ledgers.Add(new LedgerMasterExport { Name = settings.IGSTLedgerName, Parent = "Duties & Taxes", TaxType = "GST", DutyHead = "Integrated Tax" });
        }
        if (needsRoundOff) package.Ledgers.Add(new LedgerMasterExport { Name = settings.RoundOffLedgerName, Parent = "Indirect Expenses" });
        if (needsDiscount) package.Ledgers.Add(new LedgerMasterExport { Name = settings.DiscountLedgerName, Parent = "Indirect Expenses" });
        if (needsOtherCharges) package.Ledgers.Add(new LedgerMasterExport { Name = settings.OtherChargesLedgerName, Parent = "Indirect Incomes" });
        if (needsCash) package.Ledgers.Add(new LedgerMasterExport { Name = settings.CashLedgerName, Parent = "Cash-in-Hand" });
    }

    private static string ResolveUniqueName(Dictionary<string, string> byIdentity, HashSet<string> usedNames, string identity, string baseName, string suffix, IList<string> warnings, string warningPrefix)
    {
        string id = string.IsNullOrWhiteSpace(identity) ? baseName : identity;
        if (byIdentity.ContainsKey(id)) return byIdentity[id];
        string name = TallyNameHelper.CleanTallyName(baseName);
        if (string.IsNullOrWhiteSpace(name)) name = "UNKNOWN";
        if (usedNames.Contains(name))
        {
            string decorated = TallyNameHelper.CleanTallyName(name + " [" + suffix + "]");
            warnings.Add(warningPrefix + ": " + name + " renamed to " + decorated);
            name = decorated;
        }
        usedNames.Add(name);
        byIdentity[id] = name;
        return name;
    }

    private static string GetExportPath(string folder, string prefix, DateTime fromDate, DateTime toDate)
    {
        string baseName = prefix + "_" + fromDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "_" + toDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".xml";
        string path = Path.Combine(folder, baseName);
        if (!File.Exists(path)) return path;
        string timed = prefix + "_" + fromDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "_" + toDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "_" + DateTime.Now.ToString("HHmmss", CultureInfo.InvariantCulture) + ".xml";
        return Path.Combine(folder, timed);
    }

    private static void WriteLogs(ExportSummary summary, TallyExportPackage package, List<SalesExportInvoice> selected)
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("Type,Message");
        foreach (string warning in package.Warnings) csv.AppendLine("Warning,\"" + warning.Replace("\"", "\"\"") + "\"");
        foreach (string error in package.Errors) csv.AppendLine("Error,\"" + error.Replace("\"", "\"\"") + "\"");
        File.WriteAllText(summary.ErrorCsvPath, csv.ToString(), Encoding.UTF8);

        StringBuilder log = new StringBuilder();
        log.AppendLine("RRE Tally Sales Export");
        log.AppendLine("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        log.AppendLine("Invoices selected: " + summary.InvoicesSelected);
        log.AppendLine("Invoices exported: " + selected.Count);
        log.AppendLine("Warnings: " + package.Warnings.Count);
        log.AppendLine("Errors: " + package.Errors.Count);
        File.WriteAllText(summary.LogPath, log.ToString(), Encoding.UTF8);
    }

    private static void AddAddress(CustomerMasterExport customer, string text)
    {
        string cleaned = TallyNameHelper.CleanXmlText(text).Trim();
        if (cleaned.Length > 0) customer.AddressLines.Add(cleaned);
    }

    private static bool IsSupportedGstRate(decimal rate)
    {
        return rate == 0m || rate == 5m || rate == 12m || rate == 18m || rate == 28m;
    }

    private static decimal Round(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal RoundWhole(decimal value)
    {
        return Math.Round(value, 0, MidpointRounding.AwayFromZero);
    }

    private static decimal CalculateTaxableFromInclusive(decimal grossAmount, decimal gstRate)
    {
        if (gstRate <= 0m) return grossAmount;
        return Round(grossAmount / (1m + (gstRate / 100m)));
    }

    private static bool IsInterstateSale(SalesExportRow row, TallyCompanySettings settings)
    {
        string customerState = string.IsNullOrWhiteSpace(row.CustomerState) ? settings.CompanyState : row.CustomerState.Trim();
        return !string.Equals(customerState, settings.CompanyState, StringComparison.OrdinalIgnoreCase);
    }

    private static string AppendMessage(string existing, string message)
    {
        return string.IsNullOrWhiteSpace(existing) ? message : existing + "; " + message;
    }
}
