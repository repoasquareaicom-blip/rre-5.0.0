using System.ComponentModel;
using System.Diagnostics;

namespace RRE_To_Tally;

public partial class FrmTallySalesExport : Form
{
    private readonly TallyDataRepository _repository = new TallyDataRepository();
    private readonly TallyExportService _service;
    private readonly UserSession _currentUser;
    private TallyExportPackage _package = new TallyExportPackage();
    private BindingList<SalesExportInvoice> _invoiceBinding = new BindingList<SalesExportInvoice>();
    private bool _loadingCompanies;

    public FrmTallySalesExport(UserSession currentUser)
    {
        _service = new TallyExportService(_repository);
        _currentUser = currentUser;
        InitializeComponent();
        dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        dtpTo.Value = DateTime.Today;
        txtExportFolder.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RRE Tally Exports");
        lblLoggedIn.Text = "User: " + (_currentUser.UserFullName.Length > 0 ? _currentUser.UserFullName : _currentUser.UserName);
        btnUserAccess.Visible = _currentUser.IsAdmin;
        dgvInvoices.CurrentCellDirtyStateChanged += dgvInvoices_CurrentCellDirtyStateChanged;
        BindGrid();
        SetStatus("Ready");
        Load += FrmTallySalesExport_Load;
    }

    private async void FrmTallySalesExport_Load(object? sender, EventArgs e)
    {
        await LoadCompaniesAsync().ConfigureAwait(true);
    }

    private async void btnLoadData_Click(object sender, EventArgs e)
    {
        if (!ValidateDates()) return;
        SalesDivisionConfig? selectedDivision = GetSelectedDivision();
        if (selectedDivision == null)
        {
            MessageBox.Show(this, "Select a company before loading data.", "Tally Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await RunBusyAsync("Loading sales data...", async delegate
        {
            string billNumber = txtBillNumber.Text.Trim();
            ClearGrid();
            _package = await _service.LoadPackageAsync(dtpFrom.Value.Date, dtpTo.Value.Date, billNumber, GetOptions()).ConfigureAwait(true);
            _invoiceBinding = new BindingList<SalesExportInvoice>(_package.Invoices);
            BindGrid();
            ApplyRowColours();
            UpdateSummary();
            SetStatus("Loaded " + _package.Invoices.Count + " " + selectedDivision.CompanyName + " invoices" + (billNumber.Length > 0 ? " for bill search '" + billNumber + "'." : "."));
        }).ConfigureAwait(true);
    }

    private async void btnGenerateMasters_Click(object sender, EventArgs e)
    {
        await ExportAsync(true, false).ConfigureAwait(true);
    }

    private async void btnGenerateSales_Click(object sender, EventArgs e)
    {
        await ExportAsync(false, true).ConfigureAwait(true);
    }

    private async void btnGenerateBoth_Click(object sender, EventArgs e)
    {
        await ExportAsync(true, true).ConfigureAwait(true);
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog dialog = new FolderBrowserDialog())
        {
            dialog.Description = "Select Tally XML export folder";
            dialog.SelectedPath = Directory.Exists(txtExportFolder.Text) ? txtExportFolder.Text : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                txtExportFolder.Text = dialog.SelectedPath;
            }
        }
    }

    private void btnOpenFolder_Click(object sender, EventArgs e)
    {
        OpenFolder();
    }

    private void btnUserAccess_Click(object sender, EventArgs e)
    {
        using (FrmUserAccess form = new FrmUserAccess(_currentUser))
        {
            form.ShowDialog(this);
        }
    }

    private async Task ExportAsync(bool masters, bool sales)
    {
        if (!ValidateDates()) return;
        if (_package.Invoices.Count == 0)
        {
            MessageBox.Show(this, "Load data before generating XML.", "Tally Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!_package.Invoices.Any(i => i.Export && i.IsValid))
        {
            MessageBox.Show(this, "Select at least one valid invoice before generating XML.", "Tally Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        List<SalesExportInvoice> selectedInvalid = _package.Invoices.Where(i => i.Export && !i.IsValid).ToList();
        if (selectedInvalid.Count > 0)
        {
            MessageBox.Show(this, "Some selected invoices have errors and will be skipped. Fix or uncheck them before export.", "Tally Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        await RunBusyAsync("Generating XML...", async delegate
        {
            ExportSummary summary = await _service.ExportAsync(_package, dtpFrom.Value.Date, dtpTo.Value.Date, txtExportFolder.Text.Trim(), GetOptions(), masters, sales).ConfigureAwait(true);
            string message = "Invoices selected: " + summary.InvoicesSelected + Environment.NewLine +
                "Invoices exported: " + summary.InvoicesExported + Environment.NewLine +
                "Invoices skipped: " + summary.InvoicesSkipped + Environment.NewLine +
                "Customers exported: " + summary.CustomersExported + Environment.NewLine +
                "Products exported: " + summary.ProductsExported + Environment.NewLine +
                "Warnings: " + summary.Warnings + Environment.NewLine +
                "Errors: " + summary.Errors + Environment.NewLine +
                "Masters XML path: " + summary.MastersXmlPath + Environment.NewLine +
                "Sales XML path: " + summary.SalesXmlPath;
            SetStatus("Export complete.");
            MessageBox.Show(this, message, "Tally Export Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (chkOpenAfter.Checked) OpenFolder();
        }).ConfigureAwait(true);
    }

    private async Task RunBusyAsync(string message, Func<Task> work)
    {
        SetBusy(true);
        progressBar.Style = ProgressBarStyle.Marquee;
        SetStatus(message);
        try
        {
            await work().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            SetStatus("Error: " + ex.Message);
            MessageBox.Show(this, ex.Message, "Tally Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = 0;
            SetBusy(false);
        }
    }

    private bool ValidateDates()
    {
        if (dtpFrom.Value.Date > dtpTo.Value.Date)
        {
            MessageBox.Show(this, "From Date cannot be greater than To Date.", "Tally Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtExportFolder.Text))
        {
            MessageBox.Show(this, "Select an export folder.", "Tally Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private TallyExportOptions GetOptions()
    {
        return new TallyExportOptions
        {
            IncludeCustomerMasters = chkCustomerMasters.Checked,
            IncludeProductMasters = chkProductMasters.Checked,
            IncludeSalesLedgers = chkSalesLedgers.Checked,
            IncludeGstLedgers = chkGstLedgers.Checked,
            OpenFolderAfterExport = chkOpenAfter.Checked,
            CashSalesLedgerBehaviour = CashSalesLedgerBehaviour.UseCustomerLedger,
            SelectedDivision = GetSelectedDivision()
        };
    }

    private void BindGrid()
    {
        dgvInvoices.AutoGenerateColumns = false;
        dgvInvoices.AllowUserToAddRows = false;
        dgvInvoices.AllowUserToDeleteRows = false;
        dgvInvoices.ReadOnly = false;
        foreach (DataGridViewColumn column in dgvInvoices.Columns)
        {
            column.ReadOnly = column.DataPropertyName != "Export";
        }

        dgvInvoices.DataSource = _invoiceBinding;
    }

    private async Task LoadCompaniesAsync()
    {
        await RunBusyAsync("Loading companies...", async delegate
        {
            _loadingCompanies = true;
            try
            {
                List<SalesDivisionConfig> companies = await _service.LoadCompanyConfigsAsync().ConfigureAwait(true);
                cboCompany.DataSource = companies;
                cboCompany.DisplayMember = "CompanyName";
                cboCompany.ValueMember = "Key";
                if (companies.Count > 0)
                {
                    cboCompany.SelectedIndex = GetDefaultCompanyIndex(companies);
                    SetStatus("Ready. Company field loaded from ReportAddressDetails.CompanyName.");
                }
                else
                {
                    SetStatus("No mapped companies found in ReportAddressDetails.CompanyName.");
                }
            }
            finally
            {
                _loadingCompanies = false;
            }
        }).ConfigureAwait(true);
    }

    private int GetDefaultCompanyIndex(List<SalesDivisionConfig> companies)
    {
        string configuredCompany = TallyCompanySettings.Load().CompanyName;
        if (!string.IsNullOrWhiteSpace(configuredCompany))
        {
            int configuredIndex = companies.FindIndex(c => string.Equals(SalesDivisionConfig.NormalizeCompanyName(c.CompanyName), SalesDivisionConfig.NormalizeCompanyName(configuredCompany), StringComparison.OrdinalIgnoreCase));
            if (configuredIndex >= 0) return configuredIndex;
        }

        return 0;
    }

    private SalesDivisionConfig? GetSelectedDivision()
    {
        return cboCompany.SelectedItem as SalesDivisionConfig;
    }

    private void cboCompany_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_loadingCompanies) return;
        ClearGrid();
        SalesDivisionConfig? selected = GetSelectedDivision();
        SetStatus(selected == null ? "Select a company." : "Company changed to " + selected.CompanyName + ". Click Load Data.");
    }

    private void btnSelectAll_Click(object? sender, EventArgs e)
    {
        SetAllExports(true);
    }

    private void btnDeselectAll_Click(object? sender, EventArgs e)
    {
        SetAllExports(false);
    }

    private void SetAllExports(bool export)
    {
        foreach (SalesExportInvoice invoice in _invoiceBinding)
        {
            invoice.Export = export;
        }

        dgvInvoices.Refresh();
    }

    private void dgvInvoices_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (dgvInvoices.IsCurrentCellDirty)
        {
            dgvInvoices.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void ClearGrid()
    {
        _package = new TallyExportPackage();
        _invoiceBinding = new BindingList<SalesExportInvoice>();
        BindGrid();
        UpdateSummary();
    }

    private void ApplyRowColours()
    {
        foreach (DataGridViewRow row in dgvInvoices.Rows)
        {
            SalesExportInvoice? invoice = row.DataBoundItem as SalesExportInvoice;
            if (invoice == null) continue;
            if (invoice.Status == "Error")
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(127, 29, 29);
            }
            else if (invoice.Status == "Warning")
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(254, 249, 195);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(113, 63, 18);
            }
        }
    }

    private void UpdateSummary()
    {
        lblTotalInvoicesValue.Text = _package.Invoices.Count.ToString();
        lblTotalCustomersValue.Text = _package.Customers.Count.ToString();
        lblTotalProductsValue.Text = _package.Products.Count.ToString();
        lblTaxableValue.Text = _package.Invoices.Sum(i => i.TaxableAmount).ToString("0.00");
        lblGstValue.Text = _package.Invoices.Sum(i => i.Cgst + i.Sgst + i.Igst).ToString("0.00");
        lblInvoiceValue.Text = _package.Invoices.Sum(i => i.CalculatedTotal).ToString("0.00");
    }

    private void SetBusy(bool busy)
    {
        btnLoadData.Enabled = !busy;
        btnGenerateMasters.Enabled = !busy;
        btnGenerateSales.Enabled = !busy;
        btnGenerateBoth.Enabled = !busy;
        btnOpenFolder.Enabled = !busy;
        btnBrowse.Enabled = !busy;
        btnSelectAll.Enabled = !busy;
        btnDeselectAll.Enabled = !busy;
        cboCompany.Enabled = !busy;
    }

    private void SetStatus(string message)
    {
        lblStatus.Text = message;
    }

    private void OpenFolder()
    {
        string folder = txtExportFolder.Text.Trim();
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        ProcessStartInfo info = new ProcessStartInfo
        {
            FileName = folder,
            UseShellExecute = true
        };
        Process.Start(info);
    }
}
