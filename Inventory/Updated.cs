using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory
{
    public partial class Updated : Form
    {
        private static readonly string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        private readonly TabControl tabControl = new TabControl();
        private readonly TabPage tabProductPrice = new TabPage("Product Price");
        private readonly TabPage tabReceiptGoods = new TabPage("Receipt of Goods");
        private readonly DataGridView dgvReceiptGoods = new DataGridView();
        private readonly ComboBox cmbDays = new ComboBox();
        private readonly Label lblDays = new Label();
        private readonly Panel headerPanel = new Panel();
        private readonly Panel filterPanel = new Panel();
        private readonly Panel contentPanel = new Panel();
        private readonly Label lblTitle = new Label();
        private readonly Label lblSubtitle = new Label();
        private readonly Label lblProductSearch = new Label();
        private readonly Button btnApplySearch = new Button();
        private readonly Button btnCancelFilter = new Button();

        public Updated()
        {
            InitializeComponent();
            ConfigureLayout();
            LoadData();
        }

        private void ConfigureLayout()
        {
            this.Text = "Product Updates";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(960, 600);
            this.MinimumSize = new Size(860, 520);
            this.BackColor = Color.FromArgb(246, 248, 251);
            this.Font = new Font("Calibri", 9.75F, FontStyle.Regular);
            this.KeyPreview = true;

            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 76;
            headerPanel.Padding = new Padding(20, 12, 20, 12);
            headerPanel.BackColor = Color.White;

            filterPanel.Dock = DockStyle.Right;
            filterPanel.Width = 655;
            filterPanel.BackColor = Color.Transparent;

            lblTitle.Text = "Product Updates";
            lblTitle.Font = new Font("Calibri", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(24, 37, 57);
            lblTitle.Location = new Point(20, 12);
            lblTitle.AutoSize = true;

            lblSubtitle.Text = "Recent price changes and receipt-of-goods activity";
            lblSubtitle.Font = new Font("Calibri", 9.75F, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.FromArgb(102, 112, 133);
            lblSubtitle.Location = new Point(22, 42);
            lblSubtitle.AutoSize = true;

            lblDays.Text = "Last updated days";
            lblDays.Font = new Font("Calibri", 9.75F, FontStyle.Regular);
            lblDays.ForeColor = Color.FromArgb(52, 64, 84);
            lblDays.Location = new Point(0, 11);
            lblDays.AutoSize = true;

            cmbDays.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDays.FlatStyle = FlatStyle.Flat;
            cmbDays.Font = new Font("Calibri", 9.75F);
            cmbDays.Items.AddRange(new object[] { "7", "15", "30" });
            cmbDays.Location = new Point(118, 7);
            cmbDays.Width = 70;
            cmbDays.SelectedIndex = 0;
            cmbDays.SelectedIndexChanged += cmbDays_SelectedIndexChanged;

            this.Controls.Remove(dgvOrder);
            this.Controls.Remove(label25);

            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Padding = new Padding(14);
            contentPanel.BackColor = Color.FromArgb(246, 248, 251);

            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Calibri", 10F, FontStyle.Regular);
            tabControl.Padding = new Point(18, 6);
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.ItemSize = new Size(170, 34);
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.Appearance = TabAppearance.Normal;
            tabControl.DrawItem += tabControl_DrawItem;
            tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;

            ConfigureGrid(dgvOrder);
            dgvOrder.Dock = DockStyle.Fill;

            ConfigureGrid(dgvReceiptGoods);
            dgvReceiptGoods.Dock = DockStyle.Fill;
            dgvReceiptGoods.Columns.Add("Sino", "Sino");
            dgvReceiptGoods.Columns.Add("ProductName", "ProductName");
            dgvReceiptGoods.Columns.Add("Price", "Price");
            dgvReceiptGoods.Columns.Add("ReceivedQuantity", "Received Qty");
            dgvReceiptGoods.Columns.Add("UOM", "UOM");
            dgvReceiptGoods.Columns.Add("ReceiptDate", "Receipt Date");

            tabProductPrice.Padding = new Padding(10);
            tabProductPrice.BackColor = Color.White;
            tabReceiptGoods.Padding = new Padding(10);
            tabReceiptGoods.BackColor = Color.White;

            tabProductPrice.Controls.Add(dgvOrder);
            tabReceiptGoods.Controls.Add(dgvReceiptGoods);
            tabControl.TabPages.Add(tabProductPrice);
            tabControl.TabPages.Add(tabReceiptGoods);

            contentPanel.Controls.Add(tabControl);
            this.Controls.Add(contentPanel);

            lblProductSearch.Text = "Product name";
            lblProductSearch.Font = new Font("Calibri", 9.75F, FontStyle.Regular);
            lblProductSearch.ForeColor = Color.FromArgb(52, 64, 84);
            lblProductSearch.Location = new Point(205, 11);
            lblProductSearch.AutoSize = true;

            txtprodsearch.BorderStyle = BorderStyle.FixedSingle;
            txtprodsearch.Font = new Font("Calibri", 10F, FontStyle.Regular);
            txtprodsearch.Location = new Point(292, 7);
            txtprodsearch.Size = new Size(160, 24);

            btnApplySearch.Text = "Search";
            btnApplySearch.Font = new Font("Calibri", 9.75F, FontStyle.Bold);
            btnApplySearch.ForeColor = Color.White;
            btnApplySearch.BackColor = Color.FromArgb(21, 112, 239);
            btnApplySearch.FlatStyle = FlatStyle.Flat;
            btnApplySearch.FlatAppearance.BorderSize = 0;
            btnApplySearch.Cursor = Cursors.Hand;
            btnApplySearch.Location = new Point(458, 6);
            btnApplySearch.Size = new Size(78, 26);
            btnApplySearch.Click += btnApplySearch_Click;

            btnCancelFilter.Text = "Cancel Filter";
            btnCancelFilter.Font = new Font("Calibri", 9.75F, FontStyle.Bold);
            btnCancelFilter.ForeColor = Color.FromArgb(52, 64, 84);
            btnCancelFilter.BackColor = Color.FromArgb(234, 236, 240);
            btnCancelFilter.FlatStyle = FlatStyle.Flat;
            btnCancelFilter.FlatAppearance.BorderSize = 0;
            btnCancelFilter.Cursor = Cursors.Hand;
            btnCancelFilter.Location = new Point(542, 6);
            btnCancelFilter.Size = new Size(100, 26);
            btnCancelFilter.Click += btnCancelFilter_Click;

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);
            filterPanel.Controls.Add(lblDays);
            filterPanel.Controls.Add(cmbDays);
            filterPanel.Controls.Add(lblProductSearch);
            filterPanel.Controls.Add(txtprodsearch);
            filterPanel.Controls.Add(btnApplySearch);
            filterPanel.Controls.Add(btnCancelFilter);
            headerPanel.Controls.Add(filterPanel);
            this.Controls.Add(headerPanel);
        }

        private void ConfigureGrid(DataGridView grid)
        {
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToOrderColumns = true;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.BackgroundColor = SystemColors.Window;
            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = Color.FromArgb(191, 191, 191);
            grid.RowTemplate.Height = 30;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
        }

        public void getupdate()
        {
            LoadData();
        }

        private void LoadData()
        {
            LoadProductPrice();
            LoadReceiptGoods();
        }

        private void LoadProductPrice()
        {
            DataTable dt = GetProductPriceData();
            dgvOrder.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvOrder.Rows.Add();
                dgvOrder.Rows[i].Cells[0].Value = i + 1;
                dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["ProductName"]);
                dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Price"]);
                dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i]["Date"]);
            }


            ApplyGridStyle(dgvOrder);
            dgvOrder.Columns["Sino"].FillWeight = 45;
            dgvOrder.Columns["Sino"].MinimumWidth = 45;
            dgvOrder.Columns["Sino"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvOrder.Columns["ProductName"].FillWeight = 300;
            dgvOrder.Columns["ProductName"].MinimumWidth = 300;
            dgvOrder.Columns["ProductName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvOrder.Columns["ProductName"].HeaderText = "Product Name";
            dgvOrder.Columns["Price"].FillWeight = 100;
            dgvOrder.Columns["Price"].MinimumWidth = 90;
            dgvOrder.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvOrder.Columns["Date"].FillWeight = 100; 
            dgvOrder.Columns["Date"].MinimumWidth = 100;
            dgvOrder.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private DataTable GetProductPriceData()
        {
            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    ItemName AS ProductName,
    SalesPrice AS Price,
    CONVERT(varchar(10), UpdatedOn, 105) AS Date,
    UpdatedOn
FROM ProductMaster
WHERE ISNULL(IsDeleted, 0) = 0
  AND ItemName LIKE '%' + ISNULL(@ProductName, ItemName) + '%'
  AND UpdatedOn >= @FromDate
  AND UpdatedOn < DATEADD(day, DATEDIFF(day, 0, GETDATE()) + 1, 0)
ORDER BY UpdatedOn DESC, ItemName", con))
            {
                cmd.Parameters.Add("@ProductName", SqlDbType.VarChar, 500).Value = string.IsNullOrEmpty(txtprodsearch.Text.Trim()) ? (object)DBNull.Value : txtprodsearch.Text.Trim();
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = DateTime.Today.AddDays(-GetSelectedDays());
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        private void LoadReceiptGoods()
        {
            DataTable dt = GetReceiptGoodsData();
            dgvReceiptGoods.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvReceiptGoods.Rows.Add();
                dgvReceiptGoods.Rows[i].Cells[0].Value = i + 1;
                dgvReceiptGoods.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["ProductName"]);
                dgvReceiptGoods.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Price"]);
                dgvReceiptGoods.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i]["ReceivedQuantity"]);
                dgvReceiptGoods.Rows[i].Cells[4].Value = Convert.ToString(dt.Rows[i]["UOM"]);
                dgvReceiptGoods.Rows[i].Cells[5].Value = Convert.ToString(dt.Rows[i]["ReceiptDate"]);
            }

            ApplyGridStyle(dgvReceiptGoods);
            dgvReceiptGoods.Columns["Sino"].FillWeight = 45;
            dgvReceiptGoods.Columns["Sino"].MinimumWidth = 45;
            dgvReceiptGoods.Columns["Sino"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvReceiptGoods.Columns["ProductName"].FillWeight = 260;
            dgvReceiptGoods.Columns["ProductName"].MinimumWidth = 300;
            dgvReceiptGoods.Columns["ProductName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvReceiptGoods.Columns["ProductName"].HeaderText = "Product Name";
            dgvReceiptGoods.Columns["Price"].FillWeight = 90;
            dgvReceiptGoods.Columns["Price"].MinimumWidth = 90;
            dgvReceiptGoods.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvReceiptGoods.Columns["ReceivedQuantity"].FillWeight = 100;
            dgvReceiptGoods.Columns["ReceivedQuantity"].MinimumWidth = 110;
            dgvReceiptGoods.Columns["ReceivedQuantity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvReceiptGoods.Columns["UOM"].FillWeight = 70;
            dgvReceiptGoods.Columns["UOM"].MinimumWidth = 80;
            dgvReceiptGoods.Columns["UOM"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvReceiptGoods.Columns["ReceiptDate"].FillWeight = 100;
            dgvReceiptGoods.Columns["ReceiptDate"].MinimumWidth = 110;
            dgvReceiptGoods.Columns["ReceiptDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private DataTable GetReceiptGoodsData()
        {
            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    pm.ItemName AS ProductName,
    ISNULL(prd.Price, pm.SalesPrice) AS Price,
    prd.ReceivedQuantity AS ReceivedQuantity,
    ISNULL(um.UOM, pm.UOM) AS UOM,
    CONVERT(varchar(10), prh.OrderDate, 105) AS ReceiptDate,
    prh.OrderDate
FROM PurchaseReceiptHeader prh
INNER JOIN PurchaseReceiptDetails prd ON prd.PurchaseId = prh.PurchaseId
LEFT JOIN ProductMaster pm ON pm.id = prd.ProductId
LEFT JOIN UOM um ON CONVERT(varchar(50), um.Uomid) = LTRIM(RTRIM(pm.UOM)) AND ISNULL(um.IsDeleted, 0) = 0
WHERE ISNULL(prh.IsDeleted, 0) = 0
  AND ISNULL(pm.ItemName, '') LIKE '%' + ISNULL(@ProductName, ISNULL(pm.ItemName, '')) + '%'
  AND prh.OrderDate >= @FromDate
  AND prh.OrderDate < DATEADD(day, DATEDIFF(day, 0, GETDATE()) + 1, 0)
ORDER BY prh.OrderDate DESC, prh.PurchaseId DESC, pm.ItemName", con))
            {
                cmd.Parameters.Add("@ProductName", SqlDbType.VarChar, 500).Value = string.IsNullOrEmpty(txtprodsearch.Text.Trim()) ? (object)DBNull.Value : txtprodsearch.Text.Trim();
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = DateTime.Today.AddDays(-GetSelectedDays());
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        private void ApplyGridStyle(DataGridView grid)
        {
            if (grid.Columns.Count > 2)
            {
                grid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(217, 225, 242);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(32, 41, 57);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(32, 41, 57);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(198, 224, 180);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(16, 24, 40);
            grid.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            foreach (DataGridViewColumn c in grid.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Calibri", 9.75F, FontStyle.Regular);
            }
        }

        private int GetSelectedDays()
        {
            int days;
            if (cmbDays.SelectedItem == null || !int.TryParse(cmbDays.SelectedItem.ToString(), out days))
            {
                return 7;
            }

            return days;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
  
            if (keyData == (Keys.Escape))
            {
                this.Close();
                return true;
            }

            if (txtprodsearch.Focused)
            {
                if (keyData == (Keys.Enter))
                {
                    //if (!string.IsNullOrEmpty(txtprodsearch.Text))
                    //{
                        LoadData();
                    //}
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnApplySearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnCancelFilter_Click(object sender, EventArgs e)
        {
            txtprodsearch.Clear();
            LoadData();
        }

        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
        }

        private void cmbDays_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.IsHandleCreated)
            {
                LoadData();
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = tabControl.TabPages[e.Index];
            bool selected = e.Index == tabControl.SelectedIndex;
            Rectangle bounds = e.Bounds;
            Color backColor = selected ? Color.White : Color.FromArgb(238, 242, 247);
            Color foreColor = selected ? Color.FromArgb(21, 112, 239) : Color.FromArgb(71, 84, 103);

            using (SolidBrush backBrush = new SolidBrush(backColor))
            using (SolidBrush foreBrush = new SolidBrush(foreColor))
            using (Pen borderPen = new Pen(Color.FromArgb(208, 213, 221)))
            {
                e.Graphics.FillRectangle(backBrush, bounds);
                e.Graphics.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                if (selected)
                {
                    using (SolidBrush accentBrush = new SolidBrush(Color.FromArgb(21, 112, 239)))
                    {
                        e.Graphics.FillRectangle(accentBrush, bounds.X + 10, bounds.Bottom - 4, bounds.Width - 20, 3);
                    }
                }

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                Font font = selected ? new Font(tabControl.Font, FontStyle.Bold) : tabControl.Font;
                e.Graphics.DrawString(page.Text, font, foreBrush, bounds, format);
                if (selected)
                {
                    font.Dispose();
                }
            }
        }

    }
}
