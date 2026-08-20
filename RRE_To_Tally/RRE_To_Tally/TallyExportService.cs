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
        SalesDivisionConfig division = options.SelectedDivision ?? throw new InvalidOperationException("Select a company before loading sales data.");
        List<SalesExportRow> rows = await _repository.LoadSalesRowsAsync(fromDate, toDate, billNumber, division).ConfigureAwait(false);
        return BuildPackage(rows, options);
    }

    public Task<List<SalesDivisionConfig>> LoadCompanyConfigsAsync()
    {
        return _repository.LoadCompanyConfigsAsync();
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
            ResolvedTallyCustomer resolvedCustomer = ResolveCustomer(first, settings, package.Warnings);
            SalesExportInvoice invoice = new SalesExportInvoice
            {
                DivisionKey = first.DivisionKey,
                DivisionName = first.DivisionName,
                DivisionCompanyName = first.DivisionCompanyName,
                SalesId = first.SalesId,
                ReferenceId = first.ReferenceId,
                Date = first.TransactionDate,
                CustomerId = first.CustomerId,
                CustomerName = resolvedCustomer.LedgerName,
                CustomerAddress1 = resolvedCustomer.Address1,
                CustomerAddress2 = resolvedCustomer.Address2,
                CustomerCity = resolvedCustomer.City,
                CustomerDistrict = resolvedCustomer.District,
                CustomerState = resolvedCustomer.State,
                Pincode = resolvedCustomer.Pincode,
                CustomerGSTIN = resolvedCustomer.Gstin,
                GstRegistrationType = resolvedCustomer.RegistrationType,
                RawMasterGSTIN = resolvedCustomer.RawMasterGstin,
                RawSalesGSTIN = resolvedCustomer.RawSalesGstin,
                RawCustomerState = resolvedCustomer.RawState,
                CustomerContactName = resolvedCustomer.ContactName,
                CustomerPhone = resolvedCustomer.Phone,
                CustomerEmail = resolvedCustomer.Email,
                PaymentMode = TallyNameHelper.CleanXmlText(first.PaymentMode),
                Discount = numeric.ParseDecimal(first.LessAmount),
                OtherCharges = numeric.ParseDecimal(first.OtherCharges)
            };
            decimal storedGrandTotal = numeric.ParseDecimal(first.GrandTotal);
            invoice.StoredGrandTotal = storedGrandTotal > 0m ? Round(storedGrandTotal) : 0m;

            invoice.CustomerLedgerName = ResolveUniqueName(customerNameByIdentity, usedCustomerNames, first.DivisionKey + "|" + first.CustomerId, resolvedCustomer.LedgerName, first.CustomerId, package.Warnings, "Duplicate normalized customer name");
            invoice.CustomerName = invoice.CustomerLedgerName;

            bool isInterstate = IsInterstateSale(invoice.CustomerState, settings);
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
                    ProductVat = Clean(row.ProductVat),
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
        TallyCompanySettings settings = options.CompanySettings ?? TallyCompanySettings.Load();
        TallyExportPackage exportPackage = BuildSelectedPackage(package, selected, options, settings);

        string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        summary.ErrorCsvPath = Path.Combine(exportFolder, "TallyExportErrors_" + stamp + ".csv");
        summary.LogPath = Path.Combine(exportFolder, "TallyExportLog_" + stamp + ".txt");
        TallyXmlWriter writer = new TallyXmlWriter();

        await Task.Run(delegate
        {
            if (writeMasters)
            {
                string mastersPath = GetExportPath(exportFolder, "RRE_Tally_Masters", fromDate, toDate);
                writer.WriteMastersXml(mastersPath, exportPackage, options);
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

            WriteLogs(summary, exportPackage, selected, settings);
            summary.CustomersExported = exportPackage.Customers.Count;
            summary.ProductsExported = exportPackage.Products.Count;
            summary.Warnings = package.Warnings.Count;
            summary.Errors = package.Errors.Count;
        }).ConfigureAwait(false);

        return summary;
    }

    private static TallyExportPackage BuildSelectedPackage(TallyExportPackage source, List<SalesExportInvoice> selected, TallyExportOptions options, TallyCompanySettings settings)
    {
        TallyExportPackage package = new TallyExportPackage();
        foreach (SalesExportInvoice invoice in selected) package.Invoices.Add(invoice);
        foreach (string warning in source.Warnings) package.Warnings.Add(warning);
        foreach (string error in source.Errors) package.Errors.Add(error);
        PrepareMasters(package, options, settings);
        return package;
    }

    // SalesDetails.Amount is GST-inclusive. Tally inventory allocation receives the extracted taxable value.
    public decimal ResolveGstRate(SalesExportRow row, TallyNumericHelper numeric)
    {
        decimal productVat = numeric.ParseGstRate(row.ProductVat);
        if (productVat > 0m) return productVat;
        decimal productGst = numeric.ParseGstRate(row.ProductGst);
        if (productGst > 0m) return productGst;
        decimal tax = numeric.ParseGstRate(row.Tax);
        if (tax > 0m) return tax;
        decimal igst = numeric.ParseGstRate(row.Igst);
        if (igst > 0m) return igst;
        if (row.SalesDetailGst > 0m) return row.SalesDetailGst;
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
            if (customers.Add(invoice.CustomerLedgerName))
            {
                CustomerMasterExport customer = new CustomerMasterExport
                {
                    Name = invoice.CustomerLedgerName,
                    MailingName = invoice.CustomerLedgerName,
                    State = invoice.CustomerState,
                    Pincode = invoice.Pincode,
                    Gstin = invoice.CustomerGSTIN,
                    GstRegistrationType = invoice.GstRegistrationType,
                    ContactName = invoice.CustomerContactName,
                    Phone = invoice.CustomerPhone,
                    Email = invoice.CustomerEmail,
                    CustomerId = invoice.CustomerId,
                    RawTin = FirstCleanText(invoice.RawMasterGSTIN, invoice.RawSalesGSTIN),
                    RawState = invoice.RawCustomerState
                };
                foreach (string addressLine in BuildCustomerAddressLines(invoice))
                {
                    AddAddress(customer, addressLine);
                }
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
                if (products.Add(item.ProductTallyName))
                {
                    package.Products.Add(new ProductMasterExport { Name = item.ProductTallyName, ProductId = item.ProductId, BaseUnit = item.Uom, StockGroupName = item.StockGroupName, Hsn = item.Hsn, ProductVat = item.ProductVat, GstRate = item.GstRate });
                }

                if (ledgers.Add(item.SalesLedgerName))
                {
                    package.Ledgers.Add(new LedgerMasterExport { Name = item.SalesLedgerName, Parent = "Sales Accounts" });
                }

                if (item.CgstAmount != 0m) needsCgst = true;
                if (item.SgstAmount != 0m) needsSgst = true;
                if (item.IgstAmount != 0m) needsIgst = true;
            }
        }

        if (needsCgst) package.Ledgers.Add(new LedgerMasterExport { Name = settings.CGSTLedgerName, Parent = "Duties & Taxes", TaxType = "GST", DutyHead = "Central Tax" });
        if (needsSgst) package.Ledgers.Add(new LedgerMasterExport { Name = settings.SGSTLedgerName, Parent = "Duties & Taxes", TaxType = "GST", DutyHead = "State Tax" });
        if (needsIgst) package.Ledgers.Add(new LedgerMasterExport { Name = settings.IGSTLedgerName, Parent = "Duties & Taxes", TaxType = "GST", DutyHead = "Integrated Tax" });
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

    private static ResolvedTallyCustomer ResolveCustomer(SalesExportRow row, TallyCompanySettings settings, IList<string> warnings)
    {
        ResolvedTallyCustomer customer = new ResolvedTallyCustomer
        {
            CustomerId = Clean(row.CustomerId),
            LedgerName = FirstCleanTallyName(row.MasterCustomerName, row.SalesCustomerName, "CASH CUSTOMER"),
            Address1 = FirstCleanText(row.MasterAddress1, row.SalesAddress1),
            Address2 = FirstCleanText(row.MasterAddress2, row.SalesAddress2),
            City = FirstCleanText(row.MasterCity, row.SalesCity),
            District = Clean(row.MasterDistrict),
            State = FirstCleanTallyName(row.MasterStateResolved, row.SalesStateResolved, row.MasterState, row.SalesState, settings.CompanyState),
            Pincode = Clean(row.MasterPincode),
            ContactName = Clean(row.MasterContactName),
            Phone = FirstCleanText(row.MasterPhone, row.SalesMobile),
            Email = Clean(row.MasterEmail),
            RawMasterGstin = row.MasterGSTIN ?? "",
            RawSalesGstin = row.SalesGSTIN ?? "",
            RawState = FirstCleanText(row.MasterState, row.SalesState)
        };

        string gstin = FirstValidGstin(warnings, customer.LedgerName, row.MasterGSTIN ?? "", row.SalesGSTIN ?? "");
        if (gstin.Length > 0 && TallyNameHelper.IsBasicValidGstin(gstin))
        {
            customer.Gstin = gstin;
            customer.RegistrationType = "Regular";
        }
        else
        {
            customer.RegistrationType = "Unregistered/Consumer";
        }

        foreach (string line in BuildAddressLines(customer.Address1, customer.Address2, customer.City, customer.District, customer.State, customer.Pincode))
        {
            customer.AddressLines.Add(line);
        }

        return customer;
    }

    private static List<string> BuildCustomerAddressLines(SalesExportInvoice invoice)
    {
        return BuildAddressLines(invoice.CustomerAddress1, invoice.CustomerAddress2, invoice.CustomerCity, invoice.CustomerDistrict, invoice.CustomerState, invoice.Pincode);
    }

    private static List<string> BuildAddressLines(string address1, string address2, string city, string district, string state, string pincode)
    {
        List<string> lines = new List<string>();
        AddAddressLine(lines, address1, "");
        AddAddressLine(lines, address2, "");
        AddAddressLine(lines, city, address1 + " " + address2);
        AddAddressLine(lines, district, address1 + " " + address2 + " " + city);

        string stateLine = Clean(state);
        string cleanPincode = Clean(pincode);
        if (stateLine.Length > 0 && cleanPincode.Length > 0)
        {
            stateLine += " - " + cleanPincode;
        }
        else if (stateLine.Length == 0)
        {
            stateLine = cleanPincode;
        }

        AddAddressLine(lines, stateLine, string.Join(" ", lines.ToArray()));
        return lines;
    }

    private static void AddAddressLine(List<string> lines, string value, string previousText)
    {
        string cleaned = Clean(value);
        if (cleaned.Length == 0) return;
        if (ContainsWholeText(previousText, cleaned)) return;
        if (lines.Any(line => string.Equals(line, cleaned, StringComparison.OrdinalIgnoreCase))) return;
        lines.Add(cleaned);
    }

    private static bool ContainsWholeText(string text, string value)
    {
        string cleanText = Clean(text);
        string cleanValue = Clean(value);
        return cleanText.Length > 0 &&
            cleanValue.Length > 0 &&
            cleanText.IndexOf(cleanValue, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static string FirstCleanText(params string[] values)
    {
        foreach (string value in values)
        {
            string cleaned = Clean(value);
            if (cleaned.Length > 0) return cleaned;
        }

        return "";
    }

    private static string FirstCleanTallyName(params string[] values)
    {
        foreach (string value in values)
        {
            string cleaned = TallyNameHelper.CleanTallyName(value);
            if (cleaned.Length > 0) return cleaned;
        }

        return "";
    }

    private static string FirstValidGstin(IList<string> warnings, string ledgerName, params string[] values)
    {
        string firstInvalid = "";
        foreach (string value in values)
        {
            string gstin = NormalizeGstin(value);
            if (gstin.Length == 0) continue;
            if (TallyNameHelper.IsBasicValidGstin(gstin)) return gstin;
            if (firstInvalid.Length == 0) firstInvalid = gstin;
        }

        if (firstInvalid.Length > 0) warnings.Add("Invalid GSTIN for " + ledgerName + ": " + firstInvalid);
        return "";
    }

    private static string NormalizeGstin(string value)
    {
        string cleaned = TallyNameHelper.CleanXmlText(value).Trim();
        if (cleaned.Length == 0) return "";
        return new string(cleaned.Where(ch => !char.IsWhiteSpace(ch)).ToArray()).ToUpperInvariant();
    }

    private static string Clean(string value)
    {
        return TallyNameHelper.CleanXmlText(value).Trim();
    }

    private static string GetExportPath(string folder, string prefix, DateTime fromDate, DateTime toDate)
    {
        string baseName = prefix + "_" + fromDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "_" + toDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".xml";
        string path = Path.Combine(folder, baseName);
        if (!File.Exists(path)) return path;
        string timed = prefix + "_" + fromDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "_" + toDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "_" + DateTime.Now.ToString("HHmmss", CultureInfo.InvariantCulture) + ".xml";
        return Path.Combine(folder, timed);
    }

    private static void WriteLogs(ExportSummary summary, TallyExportPackage package, List<SalesExportInvoice> selected, TallyCompanySettings settings)
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
        log.AppendLine("Sales XML voucher dates:");
        foreach (SalesExportInvoice invoice in selected)
        {
            log.AppendLine("  " + invoice.DivisionName + " " + invoice.SalesId + " -> " + FormatVoucherDate(settings));
        }
        log.AppendLine("");
        log.AppendLine("Resolved customers:");
        foreach (CustomerMasterExport customer in package.Customers.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
        {
            log.AppendLine("  CustomerID = " + customer.CustomerId);
            log.AppendLine("  Resolved Name = " + customer.Name);
            log.AppendLine("  Customers.Tin raw value = " + customer.RawTin);
            log.AppendLine("  Resolved GSTIN = " + customer.Gstin);
            log.AppendLine("  GST Registration Type = " + customer.GstRegistrationType);
            log.AppendLine("  Raw State = " + customer.RawState);
            log.AppendLine("  Resolved State = " + customer.State);
        }
        log.AppendLine("");
        log.AppendLine("Resolved products:");
        foreach (ProductMasterExport product in package.Products.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
        {
            decimal cgst = Round(product.GstRate / 2m);
            decimal sgst = Round(product.GstRate / 2m);
            decimal igst = Round(product.GstRate);
            log.AppendLine("  ProductId = " + product.ProductId);
            log.AppendLine("  Product = " + product.Name);
            log.AppendLine("  ProductMaster.vat = " + product.ProductVat);
            log.AppendLine("  Resolved total GST = " + product.GstRate.ToString("0.##", CultureInfo.InvariantCulture));
            log.AppendLine("  CGST = " + cgst.ToString("0.##", CultureInfo.InvariantCulture));
            log.AppendLine("  SGST = " + sgst.ToString("0.##", CultureInfo.InvariantCulture));
            log.AppendLine("  IGST = " + igst.ToString("0.##", CultureInfo.InvariantCulture));
            log.AppendLine("  HSN = " + product.Hsn);
        }
        log.AppendLine("Warnings: " + package.Warnings.Count);
        log.AppendLine("Errors: " + package.Errors.Count);
        File.WriteAllText(summary.LogPath, log.ToString(), Encoding.UTF8);
    }

    private static string FormatVoucherDate(TallyCompanySettings settings)
    {
        return settings.ExportVoucherDate;
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

    private static bool IsInterstateSale(string customerStateValue, TallyCompanySettings settings)
    {
        string customerState = string.IsNullOrWhiteSpace(customerStateValue) ? settings.CompanyState : customerStateValue.Trim();
        return !string.Equals(customerState, settings.CompanyState, StringComparison.OrdinalIgnoreCase);
    }

    private static string AppendMessage(string existing, string message)
    {
        return string.IsNullOrWhiteSpace(existing) ? message : existing + "; " + message;
    }
}
