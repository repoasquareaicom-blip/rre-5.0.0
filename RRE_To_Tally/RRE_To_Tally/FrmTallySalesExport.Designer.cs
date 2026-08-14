namespace RRE_To_Tally;

partial class FrmTallySalesExport
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlHeader;
    private Label lblTitle;
    private Label lblLoggedIn;
    private Button btnUserAccess;
    private DateTimePicker dtpFrom;
    private DateTimePicker dtpTo;
    private TextBox txtBillNumber;
    private Button btnLoadData;
    private Button btnGenerateMasters;
    private Button btnGenerateSales;
    private Button btnGenerateBoth;
    private Button btnOpenFolder;
    private ProgressBar progressBar;
    private Label lblStatus;
    private DataGridView dgvInvoices;
    private TextBox txtExportFolder;
    private Button btnBrowse;
    private CheckBox chkCustomerMasters;
    private CheckBox chkProductMasters;
    private CheckBox chkSalesLedgers;
    private CheckBox chkGstLedgers;
    private CheckBox chkOpenAfter;
    private ComboBox cboCashBehaviour;
    private Label lblTotalInvoicesValue;
    private Label lblTotalCustomersValue;
    private Label lblTotalProductsValue;
    private Label lblTaxableValue;
    private Label lblGstValue;
    private Label lblInvoiceValue;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblTitle = new Label();
        lblLoggedIn = new Label();
        btnUserAccess = new Button();
        dtpFrom = new DateTimePicker();
        dtpTo = new DateTimePicker();
        txtBillNumber = new TextBox();
        btnLoadData = new Button();
        btnGenerateMasters = new Button();
        btnGenerateSales = new Button();
        btnGenerateBoth = new Button();
        btnOpenFolder = new Button();
        progressBar = new ProgressBar();
        lblStatus = new Label();
        dgvInvoices = new DataGridView();
        txtExportFolder = new TextBox();
        btnBrowse = new Button();
        chkCustomerMasters = new CheckBox();
        chkProductMasters = new CheckBox();
        chkSalesLedgers = new CheckBox();
        chkGstLedgers = new CheckBox();
        chkOpenAfter = new CheckBox();
        cboCashBehaviour = new ComboBox();
        lblTotalInvoicesValue = new Label();
        lblTotalCustomersValue = new Label();
        lblTotalProductsValue = new Label();
        lblTaxableValue = new Label();
        lblGstValue = new Label();
        lblInvoiceValue = new Label();
        pnlHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvInvoices).BeginInit();
        SuspendLayout();
        pnlHeader.BackColor = Color.FromArgb(12, 74, 110);
        pnlHeader.Controls.Add(btnUserAccess);
        pnlHeader.Controls.Add(lblLoggedIn);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1280, 56);
        pnlHeader.TabIndex = 0;
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(18, 13);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(245, 30);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Tally Sales XML Export";
        lblLoggedIn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblLoggedIn.AutoEllipsis = true;
        lblLoggedIn.ForeColor = Color.FromArgb(224, 242, 254);
        lblLoggedIn.Location = new Point(776, 19);
        lblLoggedIn.Name = "lblLoggedIn";
        lblLoggedIn.Size = new Size(305, 20);
        lblLoggedIn.TextAlign = ContentAlignment.MiddleRight;
        btnUserAccess.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnUserAccess.BackColor = Color.FromArgb(250, 204, 21);
        btnUserAccess.FlatStyle = FlatStyle.Flat;
        btnUserAccess.ForeColor = Color.FromArgb(15, 23, 42);
        btnUserAccess.Location = new Point(1098, 14);
        btnUserAccess.Name = "btnUserAccess";
        btnUserAccess.Size = new Size(164, 30);
        btnUserAccess.Text = "User Access";
        btnUserAccess.UseVisualStyleBackColor = false;
        btnUserAccess.Visible = false;
        btnUserAccess.Click += btnUserAccess_Click;
        Label lblFrom = MakeLabel("From Date", 18, 76);
        Label lblTo = MakeLabel("To Date", 194, 76);
        Label lblBillNumber = MakeLabel("Bill Number", 370, 76);
        dtpFrom.Format = DateTimePickerFormat.Short;
        dtpFrom.Location = new Point(18, 99);
        dtpFrom.Name = "dtpFrom";
        dtpFrom.Size = new Size(150, 23);
        dtpTo.Format = DateTimePickerFormat.Short;
        dtpTo.Location = new Point(194, 99);
        dtpTo.Name = "dtpTo";
        dtpTo.Size = new Size(150, 23);
        txtBillNumber.Location = new Point(370, 99);
        txtBillNumber.Name = "txtBillNumber";
        txtBillNumber.PlaceholderText = "Optional";
        txtBillNumber.Size = new Size(145, 23);
        btnLoadData = MakeButton("Load Data", 540, 96, 130);
        btnLoadData.Click += btnLoadData_Click;
        btnGenerateMasters = MakeButton("Generate Masters XML", 690, 96, 170);
        btnGenerateMasters.Click += btnGenerateMasters_Click;
        btnGenerateSales = MakeButton("Generate Sales XML", 876, 96, 160);
        btnGenerateSales.Click += btnGenerateSales_Click;
        btnGenerateBoth = MakeButton("Generate Both", 1052, 96, 130);
        btnGenerateBoth.Click += btnGenerateBoth_Click;
        btnOpenFolder = MakeButton("Open Export Folder", 1002, 137, 160);
        btnOpenFolder.Click += btnOpenFolder_Click;
        Label lblFolder = MakeLabel("Export folder", 18, 142);
        txtExportFolder.Location = new Point(112, 139);
        txtExportFolder.Name = "txtExportFolder";
        txtExportFolder.Size = new Size(775, 23);
        btnBrowse = MakeButton("Browse", 900, 137, 90);
        btnBrowse.Click += btnBrowse_Click;
        chkCustomerMasters = MakeCheck("Include customer masters", 18, 182);
        chkProductMasters = MakeCheck("Include product masters", 214, 182);
        chkSalesLedgers = MakeCheck("Include sales ledgers", 402, 182);
        chkGstLedgers = MakeCheck("Include GST ledgers", 568, 182);
        chkOpenAfter = MakeCheck("Open folder after export", 728, 182);
        chkCustomerMasters.Checked = true;
        chkProductMasters.Checked = true;
        chkSalesLedgers.Checked = true;
        chkGstLedgers.Checked = true;
        chkOpenAfter.Checked = true;
        Label lblCash = MakeLabel("Cash sales ledger behaviour", 930, 183);
        cboCashBehaviour.DropDownStyle = ComboBoxStyle.DropDownList;
        cboCashBehaviour.Items.AddRange(new object[] { "Use customer ledger", "Use configured cash ledger" });
        cboCashBehaviour.Location = new Point(930, 204);
        cboCashBehaviour.Name = "cboCashBehaviour";
        cboCashBehaviour.Size = new Size(210, 23);
        AddSummaryLabel("Total invoices", lblTotalInvoicesValue, 18, 224);
        AddSummaryLabel("Total customers", lblTotalCustomersValue, 210, 224);
        AddSummaryLabel("Total products", lblTotalProductsValue, 402, 224);
        AddSummaryLabel("Total taxable value", lblTaxableValue, 594, 224);
        AddSummaryLabel("Total GST", lblGstValue, 786, 224);
        AddSummaryLabel("Total invoice value", lblInvoiceValue, 978, 224);
        dgvInvoices.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvInvoices.BackgroundColor = Color.White;
        dgvInvoices.BorderStyle = BorderStyle.None;
        dgvInvoices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvInvoices.Location = new Point(18, 294);
        dgvInvoices.Name = "dgvInvoices";
        dgvInvoices.RowHeadersVisible = false;
        dgvInvoices.Size = new Size(1244, 345);
        dgvInvoices.TabIndex = 26;
        dgvInvoices.Columns.Add(MakeCheckColumn("Export", "Export", 55));
        dgvInvoices.Columns.Add(MakeTextColumn("Status", "Status", 80));
        dgvInvoices.Columns.Add(MakeTextColumn("Division", "DivisionName", 90));
        dgvInvoices.Columns.Add(MakeTextColumn("Sales ID", "SalesId", 90));
        dgvInvoices.Columns.Add(MakeTextColumn("Date", "Date", 90, "d"));
        dgvInvoices.Columns.Add(MakeTextColumn("Customer", "CustomerLedgerName", 180));
        dgvInvoices.Columns.Add(MakeTextColumn("State", "CustomerState", 110));
        dgvInvoices.Columns.Add(MakeTextColumn("Local/Interstate", "SaleType", 115));
        dgvInvoices.Columns.Add(MakeTextColumn("GSTIN", "CustomerGSTIN", 130));
        dgvInvoices.Columns.Add(MakeTextColumn("Item count", "ItemCount", 75));
        dgvInvoices.Columns.Add(MakeTextColumn("Taxable amount", "TaxableAmount", 110, "0.00"));
        dgvInvoices.Columns.Add(MakeTextColumn("CGST", "Cgst", 80, "0.00"));
        dgvInvoices.Columns.Add(MakeTextColumn("SGST", "Sgst", 80, "0.00"));
        dgvInvoices.Columns.Add(MakeTextColumn("IGST", "Igst", 80, "0.00"));
        dgvInvoices.Columns.Add(MakeTextColumn("Discount", "Discount", 90, "0.00"));
        dgvInvoices.Columns.Add(MakeTextColumn("Other charges", "OtherCharges", 105, "0.00"));
        dgvInvoices.Columns.Add(MakeTextColumn("Round off", "RoundOff", 90, "0.00"));
        dgvInvoices.Columns.Add(MakeTextColumn("Calculated total", "CalculatedTotal", 115, "0.00"));
        dgvInvoices.Columns.Add(MakeTextColumn("Stored grand total", "StoredGrandTotal", 125, "0.00"));
        dgvInvoices.Columns.Add(MakeTextColumn("Balance difference", "Difference", 120, "0.00"));
        dgvInvoices.Columns.Add(MakeTextColumn("Error message", "ErrorMessage", 260));
        progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        progressBar.Location = new Point(18, 652);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(1000, 12);
        lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblStatus.AutoEllipsis = true;
        lblStatus.Location = new Point(18, 670);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(1244, 23);
        lblStatus.Text = "Ready";
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(236, 240, 245);
        ClientSize = new Size(1280, 706);
        Controls.Add(lblFrom);
        Controls.Add(lblTo);
        Controls.Add(lblBillNumber);
        Controls.Add(dtpFrom);
        Controls.Add(dtpTo);
        Controls.Add(txtBillNumber);
        Controls.Add(btnLoadData);
        Controls.Add(btnGenerateMasters);
        Controls.Add(btnGenerateSales);
        Controls.Add(btnGenerateBoth);
        Controls.Add(btnOpenFolder);
        Controls.Add(lblFolder);
        Controls.Add(txtExportFolder);
        Controls.Add(btnBrowse);
        Controls.Add(chkCustomerMasters);
        Controls.Add(chkProductMasters);
        Controls.Add(chkSalesLedgers);
        Controls.Add(chkGstLedgers);
        Controls.Add(chkOpenAfter);
        Controls.Add(lblCash);
        Controls.Add(cboCashBehaviour);
        Controls.Add(dgvInvoices);
        Controls.Add(progressBar);
        Controls.Add(lblStatus);
        Controls.Add(pnlHeader);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(1100, 650);
        Name = "FrmTallySalesExport";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Tally Sales XML Export";
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvInvoices).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private static Label MakeLabel(string text, int x, int y)
    {
        return new Label { AutoSize = true, ForeColor = Color.FromArgb(30, 41, 59), Location = new Point(x, y), Text = text };
    }

    private static Button MakeButton(string text, int x, int y, int width)
    {
        Button button = new Button { Text = text, Location = new Point(x, y), Size = new Size(width, 29), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(20, 184, 166), ForeColor = Color.White };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private static CheckBox MakeCheck(string text, int x, int y)
    {
        return new CheckBox { AutoSize = true, Checked = true, Location = new Point(x, y), Text = text };
    }

    private void AddSummaryLabel(string caption, Label valueLabel, int x, int y)
    {
        Label captionLabel = new Label { AutoSize = true, ForeColor = Color.FromArgb(71, 85, 105), Location = new Point(x, y), Text = caption };
        valueLabel.AutoSize = true;
        valueLabel.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        valueLabel.ForeColor = Color.FromArgb(15, 23, 42);
        valueLabel.Location = new Point(x, y + 22);
        valueLabel.Text = "0";
        Controls.Add(captionLabel);
        Controls.Add(valueLabel);
    }

    private static DataGridViewTextBoxColumn MakeTextColumn(string header, string property, int width, string format = null)
    {
        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn { HeaderText = header, DataPropertyName = property, Width = width, ReadOnly = true };
        if (!string.IsNullOrWhiteSpace(format)) column.DefaultCellStyle.Format = format;
        return column;
    }

    private static DataGridViewCheckBoxColumn MakeCheckColumn(string header, string property, int width)
    {
        return new DataGridViewCheckBoxColumn { HeaderText = header, DataPropertyName = property, Width = width };
    }
}
