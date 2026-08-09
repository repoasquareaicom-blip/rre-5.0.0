using System.ComponentModel;

namespace RRE_To_Tally;

public enum CashSalesLedgerBehaviour
{
    UseCustomerLedger,
    UseRreCashLedger
}

public sealed class UserSession
{
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string UserFullName { get; set; } = "";
    public string Role { get; set; } = "";
    public bool IsAdmin { get { return string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase); } }
}

public sealed class UserAccessRow : INotifyPropertyChanged
{
    private bool _hasAccess;
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string UserFullName { get; set; } = "";
    public string Role { get; set; } = "";
    public bool HasAccess
    {
        get { return _hasAccess; }
        set { _hasAccess = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasAccess")); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public sealed class TallyExportOptions
{
    public bool IncludeCustomerMasters { get; set; } = true;
    public bool IncludeProductMasters { get; set; } = true;
    public bool IncludeSalesLedgers { get; set; } = true;
    public bool IncludeGstLedgers { get; set; } = true;
    public bool OpenFolderAfterExport { get; set; } = true;
    public CashSalesLedgerBehaviour CashSalesLedgerBehaviour { get; set; } = CashSalesLedgerBehaviour.UseCustomerLedger;
    public bool AggregateDuplicateProducts { get; set; }
    public TallyCompanySettings CompanySettings { get; set; } = TallyCompanySettings.Load();
}

public sealed class TallyCompanySettings
{
    public string CompanyName { get; set; } = "";
    public string CompanyGSTIN { get; set; } = "";
    public string CompanyState { get; set; } = "Tamil Nadu";
    public string CompanyGSTRegistrationName { get; set; } = "Tamil Nadu Registration";
    public string DispatchFromName { get; set; } = "";
    public string DispatchFromAddress1 { get; set; } = "";
    public string DispatchFromAddress2 { get; set; } = "";
    public string DispatchFromAddress3 { get; set; } = "";
    public string DispatchFromPlace { get; set; } = "";
    public string DispatchFromPincode { get; set; } = "";
    public string MainGodownName { get; set; } = "Main Location";
    public string PrimaryBatchName { get; set; } = "Primary Batch";
    public string CGSTLedgerName { get; set; } = "CGST";
    public string SGSTLedgerName { get; set; } = "SGST";
    public string IGSTLedgerName { get; set; } = "IGST";
    public string RoundOffLedgerName { get; set; } = "Round Off";
    public string CashLedgerName { get; set; } = "CASH";
    public string SalesLedgerPrefix { get; set; } = "SALES";
    public string MasterApplicableFrom { get; set; } = "20260401";

    public static TallyCompanySettings Load()
    {
        TallyCompanySettings settings = new TallyCompanySettings();
        System.Collections.Specialized.NameValueCollection app = System.Configuration.ConfigurationManager.AppSettings;
        settings.CompanyName = Read(app, "TallyCompany.CompanyName", settings.CompanyName);
        settings.CompanyGSTIN = Read(app, "TallyCompany.CompanyGSTIN", settings.CompanyGSTIN).Trim().ToUpperInvariant();
        settings.CompanyState = Read(app, "TallyCompany.CompanyState", settings.CompanyState);
        settings.CompanyGSTRegistrationName = Read(app, "TallyCompany.CompanyGSTRegistrationName", settings.CompanyGSTRegistrationName);
        settings.DispatchFromName = Read(app, "TallyCompany.DispatchFromName", settings.DispatchFromName);
        settings.DispatchFromAddress1 = Read(app, "TallyCompany.DispatchFromAddress1", settings.DispatchFromAddress1);
        settings.DispatchFromAddress2 = Read(app, "TallyCompany.DispatchFromAddress2", settings.DispatchFromAddress2);
        settings.DispatchFromAddress3 = Read(app, "TallyCompany.DispatchFromAddress3", settings.DispatchFromAddress3);
        settings.DispatchFromPlace = Read(app, "TallyCompany.DispatchFromPlace", settings.DispatchFromPlace);
        settings.DispatchFromPincode = Read(app, "TallyCompany.DispatchFromPincode", settings.DispatchFromPincode);
        settings.MainGodownName = Read(app, "TallyCompany.MainGodownName", settings.MainGodownName);
        settings.PrimaryBatchName = Read(app, "TallyCompany.PrimaryBatchName", settings.PrimaryBatchName);
        settings.CGSTLedgerName = Read(app, "TallyCompany.CGSTLedgerName", settings.CGSTLedgerName);
        settings.SGSTLedgerName = Read(app, "TallyCompany.SGSTLedgerName", settings.SGSTLedgerName);
        settings.IGSTLedgerName = Read(app, "TallyCompany.IGSTLedgerName", settings.IGSTLedgerName);
        settings.RoundOffLedgerName = Read(app, "TallyCompany.RoundOffLedgerName", settings.RoundOffLedgerName);
        settings.CashLedgerName = Read(app, "TallyCompany.CashLedgerName", settings.CashLedgerName);
        settings.SalesLedgerPrefix = Read(app, "TallyCompany.SalesLedgerPrefix", settings.SalesLedgerPrefix);
        settings.MasterApplicableFrom = Read(app, "TallyCompany.MasterApplicableFrom", settings.MasterApplicableFrom);
        return settings;
    }

    private static string Read(System.Collections.Specialized.NameValueCollection values, string key, string fallback)
    {
        string? value = values[key];
        return string.IsNullOrWhiteSpace(value) ? fallback : TallyNameHelper.CleanXmlText(value).Trim();
    }
}

public sealed class SalesExportRow
{
    public int Sino { get; set; }
    public string SalesId { get; set; } = "";
    public string ReferenceId { get; set; } = "";
    public DateTime TransactionDate { get; set; }
    public string CustomerId { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string CustomerAddress1 { get; set; } = "";
    public string CustomerAddress2 { get; set; } = "";
    public string CustomerCity { get; set; } = "";
    public string CustomerState { get; set; } = "";
    public string District { get; set; } = "";
    public string Pincode { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public string Email { get; set; } = "";
    public string CustomerGSTIN { get; set; } = "";
    public string PaymentMode { get; set; } = "";
    public string TotalAmount { get; set; } = "";
    public string LessAmount { get; set; } = "";
    public string GrandTotal { get; set; } = "";
    public string OtherCharges { get; set; } = "";
    public string GstText { get; set; } = "";
    public string ProductId { get; set; } = "";
    public string ItemCode { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string Category { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Uom { get; set; } = "";
    public string Hsn { get; set; } = "";
    public string ProductGst { get; set; } = "";
    public string Sgst { get; set; } = "";
    public string Igst { get; set; } = "";
    public string Tax { get; set; } = "";
    public string Rate { get; set; } = "";
    public string Quantity { get; set; } = "";
    public string Amount { get; set; } = "";
    public decimal SalesDetailGst { get; set; }
    public bool MissingProductMaster { get; set; }
}

public sealed class SalesExportInvoice : INotifyPropertyChanged
{
    private bool _export = true;
    public bool Export
    {
        get { return _export; }
        set { _export = value; OnPropertyChanged("Export"); }
    }

    public string Status { get; set; } = "Ready";
    public string SalesId { get; set; } = "";
    public string ReferenceId { get; set; } = "";
    public DateTime Date { get; set; }
    public string CustomerId { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string CustomerLedgerName { get; set; } = "";
    public string CustomerAddress1 { get; set; } = "";
    public string CustomerAddress2 { get; set; } = "";
    public string CustomerCity { get; set; } = "";
    public string CustomerState { get; set; } = "";
    public string Pincode { get; set; } = "";
    public string CustomerGSTIN { get; set; } = "";
    public string GstRegistrationType { get; set; } = "Unregistered/Consumer";
    public string PaymentMode { get; set; } = "";
    public bool IsInterstate { get; set; }
    public string SaleType { get { return IsInterstate ? "Interstate" : "Local"; } }
    public int ItemCount { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal Cgst { get; set; }
    public decimal Sgst { get; set; }
    public decimal Igst { get; set; }
    public decimal Discount { get; set; }
    public decimal OtherCharges { get; set; }
    public decimal CalculatedTotal { get; set; }
    public decimal StoredGrandTotal { get; set; }
    public decimal Difference { get; set; }
    public decimal RoundOff { get; set; }
    public string ErrorMessage { get; set; } = "";
    public List<SalesExportItem> Items { get; } = new List<SalesExportItem>();

    public bool IsValid { get { return Status == "Ready" || Status == "Warning"; } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}

public sealed class SalesExportItem
{
    public string ProductId { get; set; } = "";
    public string ItemCode { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string ProductTallyName { get; set; } = "";
    public string Category { get; set; } = "";
    public string StockGroupName { get; set; } = "";
    public string Uom { get; set; } = "NOS";
    public string Hsn { get; set; } = "";
    public decimal Rate { get; set; }
    public decimal Quantity { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal GstRate { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public string SalesLedgerName { get; set; } = "";
    public bool IsInterstate { get; set; }
}

public sealed class CustomerMasterExport
{
    public string Name { get; set; } = "";
    public string State { get; set; } = "";
    public string Pincode { get; set; } = "";
    public string Gstin { get; set; } = "";
    public string GstRegistrationType { get; set; } = "";
    public List<string> AddressLines { get; } = new List<string>();
}

public sealed class ProductMasterExport
{
    public string Name { get; set; } = "";
    public string StockGroupName { get; set; } = "";
    public string BaseUnit { get; set; } = "NOS";
    public string Hsn { get; set; } = "";
    public decimal GstRate { get; set; }
    public decimal OpeningBalance { get; set; }
}

public sealed class UnitMasterExport
{
    public string Name { get; set; } = "";
    public int DecimalPlaces { get; set; }
}

public sealed class StockGroupMasterExport
{
    public string Name { get; set; } = "";
}

public sealed class LedgerMasterExport
{
    public string Name { get; set; } = "";
    public string Parent { get; set; } = "";
    public string TaxType { get; set; } = "";
    public string DutyHead { get; set; } = "";
}

public sealed class ExportValidationResult
{
    public bool IsValid { get; set; } = true;
    public List<string> Warnings { get; } = new List<string>();
    public List<string> Errors { get; } = new List<string>();
}

public sealed class ExportSummary
{
    public int InvoicesSelected { get; set; }
    public int InvoicesExported { get; set; }
    public int InvoicesSkipped { get; set; }
    public int CustomersExported { get; set; }
    public int ProductsExported { get; set; }
    public int Warnings { get; set; }
    public int Errors { get; set; }
    public string MastersXmlPath { get; set; } = "";
    public string SalesXmlPath { get; set; } = "";
    public string ErrorCsvPath { get; set; } = "";
    public string LogPath { get; set; } = "";
}

public sealed class TallyExportPackage
{
    public List<SalesExportInvoice> Invoices { get; } = new List<SalesExportInvoice>();
    public List<CustomerMasterExport> Customers { get; } = new List<CustomerMasterExport>();
    public List<ProductMasterExport> Products { get; } = new List<ProductMasterExport>();
    public List<UnitMasterExport> Units { get; } = new List<UnitMasterExport>();
    public List<StockGroupMasterExport> StockGroups { get; } = new List<StockGroupMasterExport>();
    public List<LedgerMasterExport> Ledgers { get; } = new List<LedgerMasterExport>();
    public List<string> Warnings { get; } = new List<string>();
    public List<string> Errors { get; } = new List<string>();
}
