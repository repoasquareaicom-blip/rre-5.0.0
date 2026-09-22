using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Report
{
    public partial class Stockreport : Form
    {
        private Button btnAllStock;
        private Button btnMinStockAlert;
        private Button btnReorderPointAlert;
        private CheckBox chkIncludeZeroStock;
        private string stockAlertMode = "All";

        public Stockreport()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            AddStockAlertButtons();
            txtproductname.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtproductname.AutoCompleteCustomSource = AutoCompleteLoad();
            txtproductname.AutoCompleteSource = AutoCompleteSource.CustomSource;
            dgvStockrpt.CellMouseMove += new DataGridViewCellMouseEventHandler(dgvStockrpt_CellMouseMove);
            dgvStockrpt.MouseLeave += new EventHandler(dgvStockrpt_MouseLeave);
            lights.Checked = true;
            electiclas.Checked = true;
            Bind(null, "All");
        }

        private void AddStockAlertButtons()
        {
            groupBox1.Height = 78;
            dgvStockrpt.Top = dgvStockrpt.Top + 30;
            dgvStockrpt.Height = Math.Max(100, dgvStockrpt.Height - 30);

            btnAllStock = CreateAlertButton("All Stock", 99, 48);
            btnMinStockAlert = CreateAlertButton("Min Stock Alert", 185, 48);
            btnReorderPointAlert = CreateAlertButton("Reorder Point Alert", 305, 48);
            chkIncludeZeroStock = new CheckBox();
            chkIncludeZeroStock.AutoSize = true;
            chkIncludeZeroStock.Checked = false;
            chkIncludeZeroStock.Location = new Point(430, 50);
            chkIncludeZeroStock.Name = "chkIncludeZeroStock";
            chkIncludeZeroStock.Size = new Size(130, 20);
            chkIncludeZeroStock.Text = "Include Zero Stock";
            chkIncludeZeroStock.UseVisualStyleBackColor = true;

            btnAllStock.Click += new EventHandler(btnAllStock_Click);
            btnMinStockAlert.Click += new EventHandler(btnMinStockAlert_Click);
            btnReorderPointAlert.Click += new EventHandler(btnReorderPointAlert_Click);
            chkIncludeZeroStock.CheckedChanged += new EventHandler(chkIncludeZeroStock_CheckedChanged);

            groupBox1.Controls.Add(btnAllStock);
            groupBox1.Controls.Add(btnMinStockAlert);
            groupBox1.Controls.Add(btnReorderPointAlert);
            groupBox1.Controls.Add(chkIncludeZeroStock);
        }

        private Button CreateAlertButton(string text, int x, int y)
        {
            Button button = new Button();
            button.FlatStyle = FlatStyle.Popup;
            button.Location = new Point(x, y);
            button.Name = "btn" + text.Replace(" ", "");
            button.Size = new Size(text.Length > 10 ? 115 : 80, 23);
            button.Text = text;
            button.UseVisualStyleBackColor = true;
            return button;
        }

        public AutoCompleteStringCollection AutoCompleteLoad()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = ProductMovementBal.itemauto();
            string[] arr = new string[st.Rows.Count];
            for (int i = 0; i < st.Rows.Count; i++)
            {
                arr[i] = st.Rows[i]["DisplayName"].ToString();
            }
            for (int i = 0; i < arr.Length; i++)
            {
                //var combined = string.Join(", ", arr);
                var combined = arr[i];
                str.Add(combined);
            }
            //for (int i = 0; i < arr.Length; i++)
            //{
            //  var combined = string.Join(", ", arr);
            //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //   str.Add(combined);
            //}

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}

            return str;
        }

        string category = null;
        string product = null;
        string Status="";
        private void btnsearch_Click(object sender, EventArgs e)
        {
            stockAlertMode = "All";
            if (!string.IsNullOrEmpty(txtproductname.Text))
            {
                product = txtproductname.Text;
            }
            else
            {
                product = null;
            }

            if (lights.Checked == true && electiclas.Checked==true)
            {
                Status = "All";
            }
            else if (lights.Checked == false && electiclas.Checked == true) {
                Status = "No";
            }
            else if (lights.Checked == true && electiclas.Checked == false)
            {
                Status = "Yes";
            }
            else if (lights.Checked == false && electiclas.Checked == false)
            {
                Status = "All";
            }

            Bind(product, Status);
        }

        private void btnAllStock_Click(object sender, EventArgs e)
        {
            stockAlertMode = "All";
            Bind(GetSelectedProductFilter(), GetSelectedStatusFilter());
        }

        private void btnMinStockAlert_Click(object sender, EventArgs e)
        {
            stockAlertMode = "MinStock";
            Bind(GetSelectedProductFilter(), GetSelectedStatusFilter());
        }

        private void btnReorderPointAlert_Click(object sender, EventArgs e)
        {
            stockAlertMode = "ReorderPoint";
            Bind(GetSelectedProductFilter(), GetSelectedStatusFilter());
        }

        private void chkIncludeZeroStock_CheckedChanged(object sender, EventArgs e)
        {
            Bind(GetSelectedProductFilter(), GetSelectedStatusFilter());
        }

        private string GetSelectedProductFilter()
        {
            return string.IsNullOrEmpty(txtproductname.Text) ? null : txtproductname.Text;
        }

        private string GetSelectedStatusFilter()
        {
            if (lights.Checked == true && electiclas.Checked == true)
                return "All";
            if (lights.Checked == false && electiclas.Checked == true)
                return "No";
            if (lights.Checked == true && electiclas.Checked == false)
                return "Yes";

            return "All";
        }

        public void GetProductSearch(string ProductName)
        {
            DataTable stock = LoadStockRackWiseReport(ProductName, GetSelectedStatusFilter());
            stock = ApplyIncludeZeroStockFilter(stock);
            dgvStockrpt.DataSource = ApplyStockAlertFilter(stock);
            FormatStockGrid();
        }

        public void Bind(string product,string status)
        {
            DataTable stock = LoadStockRackWiseReport(product, status);
            stock = ApplyIncludeZeroStockFilter(stock);
            dgvStockrpt.DataSource = ApplyStockAlertFilter(stock);
            FormatStockGrid();
        }

        private DataTable LoadStockRackWiseReport(string productName, string status)
        {
            DataTable stock = new DataTable();
            using (SqlConnection con = new SqlConnection(Program.connection))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.GetStockRackWiseReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProductName", SqlDbType.VarChar, -1).Value = string.IsNullOrEmpty(productName) ? (object)DBNull.Value : productName;
                    cmd.Parameters.Add("@status", SqlDbType.VarChar, 15).Value = string.IsNullOrEmpty(status) ? "All" : status;
                    using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(stock);
                    }
                }
            }
            return stock;
        }

        private DataTable ApplyIncludeZeroStockFilter(DataTable stock)
        {
            if (stock == null)
                return stock;

            if (chkIncludeZeroStock == null || chkIncludeZeroStock.Checked)
                return stock;

            if (!stock.Columns.Contains("TotalStock"))
                return stock;

            DataTable filtered = stock.Clone();
            foreach (DataRow row in stock.Rows)
            {
                decimal currentStock;
                if (!decimal.TryParse(Convert.ToString(row["TotalStock"]), out currentStock))
                {
                    filtered.ImportRow(row);
                    continue;
                }

                if (currentStock != 0)
                    filtered.ImportRow(row);
            }

            return filtered;
        }

        private DataTable ApplyStockAlertFilter(DataTable stock)
        {
            if (stockAlertMode == "All")
                return stock;

            if (!stock.Columns.Contains("TotalStock"))
                return stock;

            string thresholdColumn = stockAlertMode == "MinStock" ? "MinStock" : "ReorderPoint";
            if (!stock.Columns.Contains(thresholdColumn))
            {
                MessageBox.Show(thresholdColumn + " column is not available in this stock report.");
                return stock;
            }

            DataTable filtered = stock.Clone();
            foreach (DataRow row in stock.Rows)
            {
                decimal currentStock;
                decimal threshold;
                if (!decimal.TryParse(Convert.ToString(row["TotalStock"]), out currentStock))
                    continue;
                if (!decimal.TryParse(Convert.ToString(row[thresholdColumn]), out threshold))
                    continue;
                if (threshold <= 0)
                    continue;
                if (currentStock <= threshold)
                    filtered.ImportRow(row);
            }

            return filtered;
        }

        private void FormatStockGrid()
        {
            if (!dgvStockrpt.Columns.Contains("ProductId"))
                return;

            dgvStockrpt.AutoGenerateColumns = true;
            dgvStockrpt.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvStockrpt.ScrollBars = ScrollBars.Both;
            dgvStockrpt.Columns["ProductId"].Visible = false;

            SetColumn("SNo", 0, 60, "SNo");
            SetColumn("ProductName", 1, 300, "Product Name");
            SetColumn("Category", 2, 100, "Category");
            SetColumn("Brand", 3, 100, "Brand");
            SetColumn("Price", 4, 80, "Price");
            SetColumn("TotalStock", 5, 90, "Total Stock");
            SetColumn("InLocations", 6, 260, "In Locations");
            SetColumn("InRacks", 7, 350, "In Racks");
            SetColumn("TotalAmount", 8, 110, "Stock Value");
            SetColumn("LightProduct", 9, 90, "Light Product");
            SetColumn("HSN", 10, 100, "HSN");
            SetColumn("MinStock", 11, 90, "Min Stock");
            SetColumn("MaxStock", 12, 90, "Max Stock");
            SetColumn("ReorderQty", 13, 90, "Reorder Qty");
            SetColumn("ReorderPoint", 14, 105, "Reorder Point");

            AlignNumericColumn("Price");
            AlignNumericColumn("TotalStock");
            AlignNumericColumn("TotalAmount");
            AlignNumericColumn("MinStock");
            AlignNumericColumn("MaxStock");
            AlignNumericColumn("ReorderQty");
            AlignNumericColumn("ReorderPoint");

            StyleClickableColumn("TotalStock");
            StyleClickableColumn("InLocations");
            StyleClickableColumn("InRacks");


            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);



            foreach (DataGridViewColumn c in dgvStockrpt.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvStockrpt.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvStockrpt.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }

        private void SetColumn(string columnName, int displayIndex, int width, string headerText)
        {
            if (!dgvStockrpt.Columns.Contains(columnName))
                return;

            dgvStockrpt.Columns[columnName].DisplayIndex = displayIndex;
            dgvStockrpt.Columns[columnName].Width = width;
            dgvStockrpt.Columns[columnName].HeaderText = headerText;
        }

        private void AlignNumericColumn(string columnName)
        {
            if (dgvStockrpt.Columns.Contains(columnName))
                dgvStockrpt.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void StyleClickableColumn(string columnName)
        {
            if (!dgvStockrpt.Columns.Contains(columnName))
                return;

            dgvStockrpt.Columns[columnName].DefaultCellStyle.ForeColor = Color.Blue;
            dgvStockrpt.Columns[columnName].DefaultCellStyle.SelectionForeColor = Color.Blue;
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr GetFocus();
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {

                IntPtr wndHandle = GetFocus();
                Control focusedControl = FromChildHandle(wndHandle);
                
            }


            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            if (keyData == Keys.F3)
            {
                pnlprodsearch.Visible = true;
                txtprodsearch.SelectionStart = 0;
                txtprodsearch.SelectionLength = txtprodsearch.Text.Length;
                txtprodsearch.Text = "";
                txtprodsearch.Focus();
                return true;
            }
            if (txtprodsearch.Focused)
            {
                if (keyData == (Keys.Enter))
                {
                    if (!string.IsNullOrEmpty(txtprodsearch.Text))
                    {
                        GetProductSearch(txtprodsearch.Text);
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void dgvStockrpt_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && IsStockDetailColumn(dgvStockrpt.Columns[e.ColumnIndex]))
                {
                    ShowStockDetails(e.RowIndex, dgvStockrpt.Columns[e.ColumnIndex].Name);
                }
            }
           
            catch(Exception ex)
            {

            }
        }

        private void dgvStockrpt_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && IsStockDetailColumn(dgvStockrpt.Columns[e.ColumnIndex]))
                dgvStockrpt.Cursor = Cursors.Hand;
            else
                dgvStockrpt.Cursor = Cursors.Default;
        }

        private void dgvStockrpt_MouseLeave(object sender, EventArgs e)
        {
            dgvStockrpt.Cursor = Cursors.Default;
        }

        private bool IsStockDetailColumn(DataGridViewColumn column)
        {
            if (column == null)
                return false;

            string columnName = !string.IsNullOrEmpty(column.DataPropertyName) ? column.DataPropertyName : column.Name;
            return columnName == "TotalStock" || columnName == "InLocations" || columnName == "InRacks";
        }

        private void ShowStockDetails(int rowIndex, string clickedColumnName)
        {
            var coordinates = this.PointToClient(Cursor.Position);
            int x = coordinates.X - 200;
            int y = coordinates.Y - 100;
            panelIn.Visible = false;
            dgvInBills.DataSource = null;

            int productId = Convert.ToInt32(dgvStockrpt.Rows[rowIndex].Cells["ProductId"].Value);
            string productName = dgvStockrpt.Columns.Contains("ProductName") ? Convert.ToString(dgvStockrpt.Rows[rowIndex].Cells["ProductName"].Value) : "";
            string serialNoText = GetSerialNoTitleText(rowIndex);
            string totalStockText = GetTotalStockTitleText(rowIndex);
            DataTable details = LoadStockRackWiseDetail(productId);
            if (clickedColumnName == "InLocations")
                dgvInBills.DataSource = BuildLocationSummary(details);
            else
                dgvInBills.DataSource = BuildRackSummary(details);

            label6.RightToLeft = RightToLeft.No;
            label6.TextAlign = ContentAlignment.BottomLeft;
            label6.Width = Math.Max(280, panelIn.Width - 40);
            label6.Text = "Stock Details | SNo: " + serialNoText + " | Total Stock: " + totalStockText + " - " + productName;
            FormatDetailGrid();
            panelIn.Visible = true;
            panelIn.Location = new Point(Math.Max(0, x), Math.Max(0, y));
        }

        private string GetSerialNoTitleText(int rowIndex)
        {
            if (!dgvStockrpt.Columns.Contains("SNo"))
                return "";

            return Convert.ToString(dgvStockrpt.Rows[rowIndex].Cells["SNo"].Value);
        }

        private string GetTotalStockTitleText(int rowIndex)
        {
            if (!dgvStockrpt.Columns.Contains("TotalStock"))
                return "";

            object totalStockValue = dgvStockrpt.Rows[rowIndex].Cells["TotalStock"].Value;
            decimal totalStock;
            if (decimal.TryParse(Convert.ToString(totalStockValue), out totalStock))
                return totalStock.ToString("0.000");

            return Convert.ToString(totalStockValue);
        }

        private DataTable LoadStockRackWiseDetail(int productId)
        {
            DataTable details = new DataTable();
            using (SqlConnection con = new SqlConnection(Program.connection))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.GetStockRackWiseDetail", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(details);
                    }
                }
            }
            return details;
        }

        private DataTable BuildRackSummary(DataTable details)
        {
            DataTable result = new DataTable();
            result.Columns.Add("Location", typeof(string));
            result.Columns.Add("Rack", typeof(string));
            result.Columns.Add("Qty", typeof(decimal));

            foreach (DataRow row in details.Rows)
            {
                DataRow newRow = result.NewRow();
                newRow["Location"] = Convert.ToString(row["LocationName"]);
                newRow["Rack"] = Convert.ToString(row["RackCaption"]);
                newRow["Qty"] = Convert.ToDecimal(row["Quantity"]);
                result.Rows.Add(newRow);
            }

            return result;
        }

        private DataTable BuildLocationSummary(DataTable details)
        {
            DataTable result = new DataTable();
            result.Columns.Add("Location", typeof(string));
            result.Columns.Add("Qty", typeof(decimal));

            Dictionary<string, decimal> quantities = new Dictionary<string, decimal>();
            Dictionary<string, string> locations = new Dictionary<string, string>();
            foreach (DataRow row in details.Rows)
            {
                string locationId = Convert.ToString(row["LocationId"]);
                string location = Convert.ToString(row["LocationName"]);
                string key = locationId + "|" + location;
                decimal quantity = Convert.ToDecimal(row["Quantity"]);
                if (!quantities.ContainsKey(key))
                {
                    quantities.Add(key, 0);
                    locations.Add(key, location);
                }
                quantities[key] = quantities[key] + quantity;
            }

            foreach (KeyValuePair<string, decimal> item in quantities)
            {
                DataRow newRow = result.NewRow();
                newRow["Location"] = locations[item.Key];
                newRow["Qty"] = item.Value;
                result.Rows.Add(newRow);
            }

            return result;
        }

        private void FormatDetailGrid()
        {
            dgvInBills.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvInBills.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvInBills.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            foreach (DataGridViewColumn c in dgvInBills.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            if (dgvInBills.Columns.Contains("Qty"))
                dgvInBills.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void InClose_Click(object sender, EventArgs e)
        {
            panelIn.Visible = false;
            dgvInBills.DataSource = null;
        }

        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
        }
    }
}
