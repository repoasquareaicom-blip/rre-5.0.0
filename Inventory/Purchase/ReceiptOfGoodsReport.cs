using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Inventory.Purchase
{
    public class ReceiptOfGoodsReport : Form
    {
        private static readonly string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

        private readonly DateTimePicker fromDate = new DateTimePicker();
        private readonly DateTimePicker toDate = new DateTimePicker();
        private readonly ComboBox cmbSupplier = new ComboBox();
        private readonly ComboBox cmbRows = new ComboBox();
        private readonly TextBox txtProduct = new TextBox();
        private readonly TextBox txtDocument = new TextBox();
        private readonly Button btnSearch = new Button();
        private readonly Button btnClear = new Button();
        private readonly DataGridView dgvReceipts = new DataGridView();
        private readonly Label lblRows = new Label();
        private readonly Label lblQty = new Label();
        private readonly Label lblAmount = new Label();
        private readonly Label lblStatus = new Label();
        private readonly BackgroundWorker reportWorker = new BackgroundWorker();

        public ReceiptOfGoodsReport()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Receipt Of Goods Report";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1180, 680);
            this.MinimumSize = new Size(980, 560);
            this.Font = new Font("Calibri", 9.75F);
            this.BackColor = Color.FromArgb(245, 247, 250);

            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 86;
            header.Padding = new Padding(18, 12, 18, 10);
            header.BackColor = Color.FromArgb(248, 250, 252);

            Label title = new Label();
            title.Text = "Receipt Of Goods";
            title.Font = new Font("Calibri", 15F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(31, 41, 55);
            title.AutoSize = true;
            title.Location = new Point(18, 12);

            Label subTitle = new Label();
            subTitle.Text = "Purchase receipt details by supplier, product, quantity, UOM and price";
            subTitle.Font = new Font("Calibri", 9.5F);
            subTitle.ForeColor = Color.FromArgb(100, 116, 139);
            subTitle.AutoSize = true;
            subTitle.Location = new Point(20, 40);

            header.Controls.Add(title);
            header.Controls.Add(subTitle);

            Panel filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 96;
            filterPanel.Padding = new Padding(14, 12, 14, 10);
            filterPanel.BackColor = Color.White;

            TableLayoutPanel filterLayout = new TableLayoutPanel();
            filterLayout.Dock = DockStyle.Fill;
            filterLayout.ColumnCount = 8;
            filterLayout.RowCount = 2;
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 122F));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 122F));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 245F));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 245F));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 205F));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88F));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 178F));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filterLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            filterLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));

            ConfigureDatePicker(fromDate);
            ConfigureDatePicker(toDate);
            fromDate.Value = DateTime.Today.AddDays(-7);
            toDate.Value = DateTime.Today;

            ConfigureCombo(cmbSupplier);
            ConfigureCombo(cmbRows);
            cmbRows.Items.AddRange(new object[] { "50", "100", "200", "500" });
            cmbRows.SelectedIndex = 0;

            ConfigureTextBox(txtProduct);
            ConfigureTextBox(txtDocument);

            btnSearch.Text = "Search";
            btnSearch.Size = new Size(84, 28);
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            btnSearch.BackColor = Color.FromArgb(37, 99, 235);
            btnSearch.ForeColor = Color.White;
            btnSearch.Font = new Font("Calibri", 9.75F, FontStyle.Bold);
            btnSearch.Click += btnSearch_Click;

            btnClear.Text = "Clear";
            btnClear.Size = new Size(76, 28);
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnClear.BackColor = Color.White;
            btnClear.ForeColor = Color.FromArgb(51, 65, 85);
            btnClear.Click += btnClear_Click;

            Panel actionPanel = new Panel();
            actionPanel.Dock = DockStyle.Fill;
            btnSearch.Location = new Point(0, 1);
            btnClear.Location = new Point(90, 1);
            actionPanel.Controls.Add(btnSearch);
            actionPanel.Controls.Add(btnClear);

            lblStatus.Dock = DockStyle.Fill;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus.ForeColor = Color.FromArgb(100, 116, 139);

            AddFilter(filterLayout, "From", fromDate, 0);
            AddFilter(filterLayout, "To", toDate, 1);
            AddFilter(filterLayout, "Supplier", cmbSupplier, 2);
            AddFilter(filterLayout, "Product", txtProduct, 3);
            AddFilter(filterLayout, "Receipt / PO / Invoice", txtDocument, 4);
            AddFilter(filterLayout, "Rows", cmbRows, 5);
            filterLayout.Controls.Add(actionPanel, 6, 1);
            filterLayout.Controls.Add(lblStatus, 7, 1);
            filterPanel.Controls.Add(filterLayout);

            dgvReceipts.AllowUserToAddRows = false;
            dgvReceipts.AllowUserToDeleteRows = false;
            dgvReceipts.AllowUserToOrderColumns = true;
            dgvReceipts.AllowUserToResizeRows = false;
            dgvReceipts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReceipts.BackgroundColor = Color.White;
            dgvReceipts.BorderStyle = BorderStyle.None;
            dgvReceipts.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvReceipts.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvReceipts.ColumnHeadersHeight = 34;
            dgvReceipts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvReceipts.Dock = DockStyle.Fill;
            dgvReceipts.EnableHeadersVisualStyles = false;
            dgvReceipts.GridColor = Color.FromArgb(226, 232, 240);
            dgvReceipts.MultiSelect = false;
            dgvReceipts.ReadOnly = true;
            dgvReceipts.RowHeadersVisible = false;
            dgvReceipts.RowTemplate.Height = 29;
            dgvReceipts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReceipts.DataBindingComplete += dgvReceipts_DataBindingComplete;

            Panel footer = new Panel();
            footer.Dock = DockStyle.Bottom;
            footer.Height = 38;
            footer.Padding = new Padding(14, 8, 14, 8);
            footer.BackColor = Color.FromArgb(248, 250, 252);

            lblRows.Dock = DockStyle.Left;
            lblRows.Width = 190;
            lblRows.TextAlign = ContentAlignment.MiddleLeft;

            lblQty.Dock = DockStyle.Left;
            lblQty.Width = 190;
            lblQty.TextAlign = ContentAlignment.MiddleLeft;

            lblAmount.Dock = DockStyle.Fill;
            lblAmount.TextAlign = ContentAlignment.MiddleLeft;

            footer.Controls.Add(lblAmount);
            footer.Controls.Add(lblQty);
            footer.Controls.Add(lblRows);

            this.Controls.Add(dgvReceipts);
            this.Controls.Add(footer);
            this.Controls.Add(filterPanel);
            this.Controls.Add(header);
            this.Load += ReceiptOfGoodsReport_Load;

            reportWorker.DoWork += reportWorker_DoWork;
            reportWorker.RunWorkerCompleted += reportWorker_RunWorkerCompleted;
        }

        private void ConfigureDatePicker(DateTimePicker picker)
        {
            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = "dd-MM-yyyy";
            picker.Width = 105;
        }

        private void ConfigureCombo(ComboBox combo)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.FlatStyle = FlatStyle.Popup;
            combo.Font = new Font("Calibri", 9.75F);
        }

        private void ConfigureTextBox(TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Calibri", 9.75F);
            textBox.KeyDown += textBox_KeyDown;
        }

        private void AddFilter(TableLayoutPanel panel, string caption, Control control, int column)
        {
            Label label = new Label();
            label.Text = caption;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.BottomLeft;
            label.ForeColor = Color.FromArgb(71, 85, 105);

            control.Dock = DockStyle.Fill;
            panel.Controls.Add(label);
            panel.SetCellPosition(label, new TableLayoutPanelCellPosition(column, 0));
            panel.Controls.Add(control);
            panel.SetCellPosition(control, new TableLayoutPanelCellPosition(column, 1));
        }

        private void ReceiptOfGoodsReport_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            LoadReport();
        }

        private void LoadSuppliers()
        {
            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT 0 AS SuppliersID, '-- All Suppliers --' AS Name
UNION ALL
SELECT SuppliersID, Name
FROM Suppliers
WHERE ISNULL(IsDelete, 0) = 0
ORDER BY Name", con))
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                cmbSupplier.DataSource = dt;
                cmbSupplier.ValueMember = "SuppliersID";
                cmbSupplier.DisplayMember = "Name";
            }
        }

        private void LoadReport()
        {
            if (reportWorker.IsBusy)
            {
                lblStatus.Text = "Already loading...";
                return;
            }

            SetLoadingState(true);
            reportWorker.RunWorkerAsync(GetRequest());
        }

        private void reportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = GetReportData((ReportRequest)e.Argument);
        }

        private void reportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetLoadingState(false);

            if (e.Error != null)
            {
                lblStatus.Text = "Unable to load report";
                MessageBox.Show(e.Error.Message, "Receipt Of Goods Report", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable dt = (DataTable)e.Result;
            dgvReceipts.DataSource = dt;
            ApplyGridColumns();
            UpdateTotals(dt);
            lblStatus.Text = "Loaded " + dt.Rows.Count.ToString(CultureInfo.InvariantCulture) + " rows";
        }

        private void SetLoadingState(bool loading)
        {
            btnSearch.Enabled = !loading;
            btnClear.Enabled = !loading;
            cmbRows.Enabled = !loading;
            cmbSupplier.Enabled = !loading;
            fromDate.Enabled = !loading;
            toDate.Enabled = !loading;
            txtProduct.Enabled = !loading;
            txtDocument.Enabled = !loading;
            this.Cursor = loading ? Cursors.WaitCursor : Cursors.Default;
            lblStatus.Text = loading ? "Loading..." : string.Empty;
        }

        private ReportRequest GetRequest()
        {
            ReportRequest request = new ReportRequest();
            request.Rows = GetSelectedRows();
            request.FromDate = fromDate.Value.Date;
            request.ToDate = toDate.Value.Date;
            request.SupplierId = GetSelectedSupplierId();
            request.Product = txtProduct.Text.Trim();
            request.Document = txtDocument.Text.Trim();
            return request;
        }

        private DataTable GetReportData(ReportRequest request)
        {
            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP (@Rows)
    CONVERT(varchar(10), prh.OrderDate, 105) AS ReceiptDate,
    prh.OrderNumber AS ReceiptNo,
    ISNULL(poh.OrderNumber, '') AS PurchaseOrderNo,
    ISNULL(prh.InvoiceNo, '') AS InvoiceNo,
    ISNULL(s.Name, '') AS Supplier,
    ISNULL(pm.ItemCode, '') AS ItemCode,
    ISNULL(pm.ItemName, '') AS ProductName,
    ISNULL(prd.Price, '') AS ProductPrice,
    prd.ReceivedQuantity AS QtyReceived,
    ISNULL(um.UOM, pm.UOM) AS UOM,
    ISNULL(prd.Amount, '') AS Amount,
    ISNULL(prh.Status, '') AS Status,
    prh.OrderDate
FROM PurchaseReceiptHeader prh
INNER JOIN PurchaseReceiptDetails prd ON prd.PurchaseId = prh.PurchaseId
LEFT JOIN PurchaseOrderHeader poh ON poh.PurchaseId = prh.PurchaseOrderId
LEFT JOIN Suppliers s ON s.SuppliersID = prh.VendorId
LEFT JOIN ProductMaster pm ON pm.id = prd.ProductId
LEFT JOIN UOM um ON CONVERT(varchar(50), um.Uomid) = LTRIM(RTRIM(pm.UOM)) AND ISNULL(um.IsDeleted, 0) = 0
WHERE ISNULL(prh.IsDeleted, 0) = 0
  AND prh.OrderDate >= @FromDate
  AND prh.OrderDate < DATEADD(day, 1, @ToDate)
  AND (@SupplierId = 0 OR prh.VendorId = @SupplierId)
  AND ISNULL(pm.ItemName, '') LIKE '%' + @Product + '%'
  AND (
        @Document = ''
        OR ISNULL(prh.OrderNumber, '') LIKE '%' + @Document + '%'
        OR ISNULL(poh.OrderNumber, '') LIKE '%' + @Document + '%'
        OR ISNULL(prh.InvoiceNo, '') LIKE '%' + @Document + '%'
      )
ORDER BY prh.OrderDate DESC, prh.PurchaseId DESC, pm.ItemName", con))
            {
                cmd.CommandTimeout = 60;
                cmd.Parameters.Add("@Rows", SqlDbType.Int).Value = request.Rows;
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = request.FromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = request.ToDate;
                cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = request.SupplierId;
                cmd.Parameters.Add("@Product", SqlDbType.VarChar, 500).Value = request.Product;
                cmd.Parameters.Add("@Document", SqlDbType.VarChar, 100).Value = request.Document;

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        private void ApplyGridColumns()
        {
            if (dgvReceipts.Columns.Count == 0)
            {
                return;
            }

            dgvReceipts.Columns["OrderDate"].Visible = false;
            SetColumn("ReceiptDate", "Receipt Date", 95, 95);
            SetColumn("ReceiptNo", "Receipt No", 120, 120);
            SetColumn("PurchaseOrderNo", "PO No", 115, 115);
            SetColumn("InvoiceNo", "Invoice No", 110, 110);
            SetColumn("Supplier", "Supplier", 170, 160);
            SetColumn("ItemCode", "Code", 90, 80);
            SetColumn("ProductName", "Product", 260, 230);
            SetColumn("ProductPrice", "Price", 85, 85);
            SetColumn("QtyReceived", "Received Qty", 95, 95);
            SetColumn("UOM", "UOM", 70, 70);
            SetColumn("Amount", "Amount", 90, 90);
            SetColumn("Status", "Status", 90, 90);
        }

        private void SetColumn(string name, string header, int fillWeight, int minWidth)
        {
            if (!dgvReceipts.Columns.Contains(name))
            {
                return;
            }

            dgvReceipts.Columns[name].HeaderText = header;
            dgvReceipts.Columns[name].FillWeight = fillWeight;
            dgvReceipts.Columns[name].MinimumWidth = minWidth;
        }

        private void UpdateTotals(DataTable dt)
        {
            decimal qty = 0;
            decimal amount = 0;

            foreach (DataRow row in dt.Rows)
            {
                qty += ToDecimal(row["QtyReceived"]);
                amount += ToDecimal(row["Amount"]);
            }

            lblRows.Text = "Rows : " + dt.Rows.Count.ToString(CultureInfo.InvariantCulture);
            lblQty.Text = "Received Qty : " + qty.ToString("0.###", CultureInfo.InvariantCulture);
            lblAmount.Text = "Amount : " + amount.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private decimal ToDecimal(object value)
        {
            decimal result;
            if (value != null && decimal.TryParse(Convert.ToString(value), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            if (value != null && decimal.TryParse(Convert.ToString(value), NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }

            return 0;
        }

        private int GetSelectedRows()
        {
            int rows;
            return int.TryParse(Convert.ToString(cmbRows.SelectedItem), out rows) ? rows : 50;
        }

        private int GetSelectedSupplierId()
        {
            if (cmbSupplier.SelectedValue == null)
            {
                return 0;
            }

            int supplierId;
            return int.TryParse(Convert.ToString(cmbSupplier.SelectedValue), out supplierId) ? supplierId : 0;
        }

        private void dgvReceipts_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvReceipts.ClearSelection();
            dgvReceipts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvReceipts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReceipts.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 9.75F, FontStyle.Bold);
            dgvReceipts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvReceipts.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvReceipts.DefaultCellStyle.Font = new Font("Calibri", 9.5F);
            dgvReceipts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fromDate.Value = DateTime.Today.AddDays(-7);
            toDate.Value = DateTime.Today;
            txtProduct.Text = string.Empty;
            txtDocument.Text = string.Empty;
            cmbRows.SelectedIndex = 0;
            if (cmbSupplier.Items.Count > 0)
            {
                cmbSupplier.SelectedIndex = 0;
            }

            LoadReport();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadReport();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private class ReportRequest
        {
            public int Rows;
            public DateTime FromDate;
            public DateTime ToDate;
            public int SupplierId;
            public string Product;
            public string Document;
        }
    }
}
