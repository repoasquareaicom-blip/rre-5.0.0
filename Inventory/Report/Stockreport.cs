using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private string stockAlertMode = "All";

        public Stockreport()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            AddStockAlertButtons();
            txtproductname.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtproductname.AutoCompleteCustomSource = AutoCompleteLoad();
            txtproductname.AutoCompleteSource = AutoCompleteSource.CustomSource;
            Bind(null, "All");
            lights.Checked = true;
            electiclas.Checked = true;
        }

        private void AddStockAlertButtons()
        {
            groupBox1.Height = 78;
            dgvStockrpt.Top = dgvStockrpt.Top + 30;
            dgvStockrpt.Height = Math.Max(100, dgvStockrpt.Height - 30);

            btnAllStock = CreateAlertButton("All Stock", 99, 48);
            btnMinStockAlert = CreateAlertButton("Min Stock Alert", 185, 48);
            btnReorderPointAlert = CreateAlertButton("Reorder Point Alert", 305, 48);

            btnAllStock.Click += new EventHandler(btnAllStock_Click);
            btnMinStockAlert.Click += new EventHandler(btnMinStockAlert_Click);
            btnReorderPointAlert.Click += new EventHandler(btnReorderPointAlert_Click);

            groupBox1.Controls.Add(btnAllStock);
            groupBox1.Controls.Add(btnMinStockAlert);
            groupBox1.Controls.Add(btnReorderPointAlert);
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
            DataTable stock = StockReportBAL.GetSearchProductName(ProductName);
            dgvStockrpt.DataSource = ApplyStockAlertFilter(stock);
            FormatStockGrid();
        }

        public void Bind(string product,string status)
        {
            DataTable stock = StockReportBAL.GetStockNew(product, status);
            dgvStockrpt.DataSource = ApplyStockAlertFilter(stock);
            FormatStockGrid();
        }

        private DataTable ApplyStockAlertFilter(DataTable stock)
        {
            if (stockAlertMode == "All")
                return stock;

            if (!stock.Columns.Contains("Stock"))
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
                if (!decimal.TryParse(Convert.ToString(row["Stock"]), out currentStock))
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

            dgvStockrpt.Columns["ProductId"].Visible = false;

            dgvStockrpt.Columns["SNo"].Width = 80;
            dgvStockrpt.Columns["Price"].Width = 70;


            //dgvStockrpt.Columns["ItemCode"].Width = 100; dgvStockrpt.Columns["Sno"].Width = 50;

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;
            if (w == 1024 && h == 768)
            {

                dgvStockrpt.Columns["ProductName"].Width = 350;


            }
            else
            {
                dgvStockrpt.Columns["ProductName"].Width = 600;
            }

            dgvStockrpt.Columns["Category"].Width = 100;
            dgvStockrpt.Columns["Brand"].Width = 100;

            dgvStockrpt.Columns["TotalStock"].Width = 100;
            dgvStockrpt.Columns["Damage"].Width = 100;
            dgvStockrpt.Columns["Stock"].Width = 100;
            dgvStockrpt.Columns["TotalAmount"].Width = 100;


            this.dgvStockrpt.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);



            foreach (DataGridViewColumn c in dgvStockrpt.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvStockrpt.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvStockrpt.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
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
                if (dgvStockrpt.Columns[dgvStockrpt.CurrentCell.ColumnIndex].HeaderText == "Stock")
                {
                    if (e.RowIndex >= 0)
                    {
                        var coordinates = this.PointToClient(Cursor.Position);
                        int x = coordinates.X;
                        int y = coordinates.Y;
                        x = x - 200;
                        y = y - 100;
                        panelIn.Visible = false;
                        dgvInBills.DataSource = null;
                        string productname = Convert.ToString(dgvStockrpt.Rows[dgvStockrpt.CurrentCell.RowIndex].Cells["Productid"].Value);
                        dgvInBills.DataSource = StockReportBAL.GetStockNew1(productname);

                        dgvInBills.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);



                        foreach (DataGridViewColumn c in dgvInBills.Columns)
                        {
                            c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
                        }

                        dgvInBills.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                        dgvInBills.DefaultCellStyle.BackColor = Color.Gainsboro;
                        dgvInBills.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                        panelIn.Visible = true;
                        panelIn.Location = new Point(x, y);
                    }
                }
            }
           
            catch(Exception ex)
            {

            }
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
