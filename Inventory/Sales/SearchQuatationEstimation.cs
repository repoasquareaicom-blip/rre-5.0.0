
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;

using System.Windows.Forms;


using InvBal;
using Inventory.Sales;
using System.Globalization;
using System.Reflection;
using EstimationReport;
using System.Diagnostics;

namespace Inventory
{
    public partial class SearchQuatationEstimation : Form
    {
        QuotationBal objQuotationbal = new QuotationBal();
        TextBox tb;
        public bool edit = false;
        int userid = 0;
        string cas = string.Empty;
        int ProdSelRowvalue = 0;
        bool res = false;
        bool all = false;
        int page;
        string fileName;
        string firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue;
        public SearchQuatationEstimation()
        {
            InitializeComponent();
            userid = Convert.ToInt32(Program.userid);
            this.WindowState = FormWindowState.Maximized;



            //SearchPurchaselistOrder();
            SearchPurchaseOrder();
            lblperare.Text = Program.UserName; ;
            bindAssist();
            bindreference();
            bindcustomer();

            LoadPortsNew();
            Payment.SelectedItem = "All";
            Globeimage();
            cmbIscombined.Checked = true;
            GetSearchSalesOrder();

            pnlprodsearch.Visible = false;


            //panel9.Enabled = false;

        }
        private void LoadPortsNew()
        {

            dgvNew.Rows.Clear();
            dgvNew.ColumnCount = 7;
            //dgvOrder.RowCount = 16;

            dgvNew.Columns[0].Name = "S.NO";
            dgvNew.Columns[1].Name = "Items";
            dgvNew.Columns[2].Name = "UOM";
            dgvNew.Columns[5].Name = "Quantity";
            dgvNew.Columns[3].Name = "productid";
            dgvNew.Columns[4].Name = "Rate";
            dgvNew.Columns[6].Name = "Amount";

            dgvNew.Columns[3].Visible = false;

            this.dgvNew.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvNew.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvNew.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvNew.Columns[0].ReadOnly = true;
            this.dgvNew.Columns[1].ReadOnly = true;
            this.dgvNew.Columns[2].ReadOnly = true;
            this.dgvNew.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvNew.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvNew.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            this.dgvNew.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvNew.Columns[4].ReadOnly = true;

            this.dgvNew.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvNew.Columns[5].ReadOnly = true;

            dgvNew.Columns[4].DefaultCellStyle.Format = "N2";
            dgvNew.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvNew.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvNew.Columns[6].ReadOnly = true;


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvNew.Columns[0].Width = 12;
                this.dgvNew.Columns[1].Width = 100;
                this.dgvNew.Columns[2].Width = 15;
                this.dgvNew.Columns[4].Width = 15;
                this.dgvNew.Columns[5].Width = 20;
                this.dgvNew.Columns[6].Width = 100;

            }
            else
            {
                this.dgvNew.Columns[0].Width = 12;
                this.dgvNew.Columns[1].Width = 100;
                this.dgvNew.Columns[2].Width = 15;
                this.dgvNew.Columns[4].Width = 15;
                this.dgvNew.Columns[5].Width = 15;
                this.dgvNew.Columns[6].Width = 100;

            }


            dgvNew.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);



            dgvNew.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvNew.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvNew.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Yellow, 0, 0, 100, 100);
        }
        public void Globeimage()
        {
            //string pathname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            ////string pathname = Path.Combine(Environment.CurrentDirectory);
            ////string a = pathname.Replace("\\bin\\Debug", "");
            ////string path = a + "\\Resources\\Light Globe.gif";
            //string path = pathname + "\\Loading.gif";
            //using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            //{
            //    var ms = new System.IO.MemoryStream();
            //    fs.CopyTo(ms);
            //    ms.Position = 0;                               // <=== here
            //    if (pcloading.Image != null) pcloading.Image.Dispose();
            //    pcloading.Image = Image.FromStream(ms);
            //    pcloading.SizeMode = PictureBoxSizeMode.Zoom;
            //}
        }

        public void bindcustomer()
        {

            cmbcustomername.DataSource = objQuotationbal.Getcustomer();
            cmbcustomername.DisplayMember = "Name";
            cmbcustomername.ValueMember = "CustomerID";

            //cmbcustomer.DataSource = objQuotationbal.Getcustomer();
            //cmbcustomer.DisplayMember = "Name";
            //cmbcustomer.ValueMember = "CustomerID";




        }



        public void bindreference()
        {
            cmbreference.DataSource = objQuotationbal.Getreference();
            cmbreference.DisplayMember = "Name";
            cmbreference.ValueMember = "ReferencesID";
        }

        public void bindAssist()
        {
            cmbassistby.DataSource = objQuotationbal.GetProductsusername();
            cmbassistby.DisplayMember = "Name";
            cmbassistby.ValueMember = "employeeid";
        }

        private void pbxCollapse_Click(object sender, EventArgs e)
        {

            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;

            }
        }
        private void RefScrollGrid()
        {

        }
        private void vLabel2_Click(object sender, EventArgs e)
        {
            if (pnlOrder.Visible == true)
            {
                pnlOrder.Visible = false;
                vLabel2.Visible = false;
                pnlCollapse2.Visible = true;
                splitContainer1.Panel2Collapsed = false;
                pbxCollapse.Visible = true;
                pbxRightCollapse.Visible = true;
                this.dgvSearch.Columns[0].Visible = false;
                this.dgvSearch.Columns[2].Visible = false;
                this.dgvSearch.Columns[3].Visible = false;


            }
        }




        private void pbxRightCollapse_Click(object sender, EventArgs e)
        {
            if (pnlCollapse2.Visible == true)
            {
                pnlOrder.Visible = true;
                vLabel2.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;
                this.dgvSearch.Columns[1].Visible = true;
                this.dgvSearch.Columns[2].Visible = true;
                this.dgvSearch.Columns[3].Visible = true;
                this.dgvSearch.Columns[4].Visible = true;


            }
        }

        private void SalesQuotation1_Load(object sender, EventArgs e)
        {
            this.ActiveControl = cmbcustomername;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F3))
            {
                txtOrderNo.Focus();
                this.ActiveControl = txtOrderNo;
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
                        SearchPurchaselistOrder();
                    }
                }
            }
            if (keyData == (Keys.A | Keys.Alt))
            {
                all = true;
                GetSearchSalesOrder();

                return true;
            }

            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }






            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SearchPurchaselistOrder()
        {
            DataTable dt = objQuotationbal.SearchPurchaselistOrder(txtprodsearch.Text);
            DataTable SearchResult = new DataTable();
            if (dt.Rows.Count > 0)
            {
                try
                {
                    if (textSearchQty.Text == "")
                        SearchResult = dt.Select("Products like '%" + txtSearchProduct.Text + "%'").CopyToDataTable();
                    else
                        SearchResult = dt.Select("Products like '%" + txtSearchProduct.Text + "%' and QTY like '%|" + textSearchQty.Text + "|%'").CopyToDataTable();
                }
                catch (Exception ex)
                {
                    SearchResult = new DataTable();
                }
            }

            lblItemCount.Text = SearchResult.Rows.Count.ToString();
            total1();
            try
            {
                AlphanumComparator<string> comparer = new AlphanumComparator<string>();
                //DataTable dtNew = dv.Table;
                DataTable dtNew = SearchResult.AsEnumerable().OrderBy(x => x.Field<string>("Estimation No"), comparer).CopyToDataTable();
                //dtNew.TableName = "NaturalSort";
                SearchResult = dtNew;
            }
            catch (Exception ex)
            {

            }

            dgvSearch.Rows.Clear();
            for (int i = 0; i < SearchResult.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(SearchResult.Rows[i]["Order No"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(SearchResult.Rows[i]["Estimation No"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(SearchResult.Rows[i]["Customer"]);
                dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(SearchResult.Rows[i]["Order Date"]);
                dgvSearch.Rows[i].Cells[4].Value = Convert.ToString(SearchResult.Rows[i]["Status"]);
                dgvSearch.Rows[i].Cells[5].Value = Convert.ToDecimal(SearchResult.Rows[i]["Amount"]);
            }



            dgvSearch.Columns["Estimation No"].Visible = true;
            dgvSearch.Columns["Status"].Visible = true;
            dgvSearch.Columns["Amount"].Visible = true;
            dgvSearch.Columns["Order No"].Visible = false;
            //dgvSearch.Columns["Order Date"].Visible = false;
            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //panel9.Enabled = false;
            all = false;

            //if (dgvSearch.Rows.Count>0)
            //{
            //    dgvSearch.Focus();
            //    dgvSearch.CurrentCell=dgvSearch[0,0];
            //}

        }
        private void SearchPurchaseOrder()
        {
            dgvSearch.Rows.Clear();
            dgvSearch.Columns.Clear();
            dgvSearch.ColumnCount = 6;
            //dgvSearch.RowCount = 16;

            dgvSearch.Columns[0].Name = "Order No";
            dgvSearch.Columns[1].Name = "Estimation No";
            dgvSearch.Columns[2].Name = "CustomerName";
            dgvSearch.Columns[3].Name = "Order Date";
            dgvSearch.Columns[4].Name = "Status";
            dgvSearch.Columns[5].Name = "Amount";


            this.dgvSearch.Columns[0].Width = 40;
            this.dgvSearch.Columns[1].Width = 25;
            this.dgvSearch.Columns[4].Width = 60;
            this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[5].SortMode = DataGridViewColumnSortMode.Automatic;

            this.dgvSearch.Columns[2].Width = 100;
            //this.dgvSearch.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvSearch.Columns[3].Width = 30;


            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn c in dgvSearch.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
            this.dgvSearch.Columns[2].Visible = false;
            this.dgvSearch.Columns[3].Visible = false;
            this.dgvSearch.Columns[4].Visible = false;
            this.dgvSearch.Columns[0].Visible = false;
            //this.dgvSearch.Columns[3].Visible = false;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        }

        public void AutoCompleteLoad(string s, int t)
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.itemauto(s, t);

            if (st.Rows.Count > 0)
            {

            }
            else
            {

            }




        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnSavePending_Click(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }



        private void txtCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;

        }

        private void txtContactNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }




        private void btnNew_Click(object sender, EventArgs e)
        {

        }

        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrls => GetAll(ctrls, type)).Concat(controls).Where(c => c.GetType() == type);
        }








        private void cmbcustomername_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        public DataTable DataGridView2DataTable(DataGridView dgv, int minRow = 0)
        {

            DataTable dt = new DataTable();

            // Header columns
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                DataColumn dc = new DataColumn(column.Name.ToString());
                dt.Columns.Add(dc);
            }

            // Data cells
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                DataGridViewRow row = dgv.Rows[i];
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    dr[j] = (row.Cells[j].Value == null) ? "" : row.Cells[j].Value.ToString();
                }
                dt.Rows.Add(dr);
            }

            // Related to the bug arround min size when using ExcelLibrary for export
            for (int i = dgv.Rows.Count; i < minRow; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dr[j] = "  ";
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }




        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            ProdSelRowvalue = 0;

        }



        public void RemoveNullColumnFromDataTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][0])))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }


        private void textbox_Change(object sender, EventArgs e)
        {

        }

        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void button_click(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;

            if (btn.Checked)
            {

            }

        }





        private void btnSearch_Click(object sender, EventArgs e)
        {

        }

        public void bindorderno(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = objQuotationbal.ordernoval();
            cmb.DataSource = dt;
            cmb.DisplayMember = "Quotationid";
            cmb.ValueMember = "Quotationid";
        }

        public void bindcustomer(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = objQuotationbal.Getcustomer();
            cmb.DataSource = dt;
            cmb.DisplayMember = "Name";
            cmb.ValueMember = "CustomerID";
        }

        public void bindreference(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = objQuotationbal.Getreference();
            cmb.DataSource = dt;
            cmb.DisplayMember = "Name";
            cmb.ValueMember = "ReferencesID";
        }





        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, int userid)
        {

            DataTable dt = objQuotationbal.search(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Quotationid"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Customer"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Reference"]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            total1();
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[1].Value);
                if (!string.IsNullOrEmpty(s))
                {
                    double totalamount = 0.00;

                    getEstimation(s);
                    total();
                    //totalamount = Convert.ToDouble(lbltotalamount.Text) - Convert.ToDouble(txtless.Text);

                   // lblTotal.Text = String.Format("{0:00.00}", totalamount);
                    Txtcustomername.Focus();
                }
                else
                {
                    clear();
                }

            }
        }
        public void clear()
        {
            // pnsearch.Visible = false;
            Txtcustomername.Text = "--Select--";
            cmdcity.Text = string.Empty;
            cmbassistby.SelectedIndex = 0;
            cmbreference.SelectedIndex = 0;

            dgvNew.Rows.Clear();
            lblperare.Text = Program.UserName;
            lbltotalamount.Text = "0.00";
            txtless.Text = "0.00";
            lblTotal.Text = "0.00";
            tatQuotationno.Text = string.Empty;
            Txtcustomername.Focus();
            txtOrdNo.Text = string.Empty;

            txtRemarks.Text = string.Empty;
            // cmbloaction.SelectedIndex = 0;
            cmbpaymode.SelectedIndex = 0;
            //rbCash.Checked = true;
            lblless.Visible = false;
            txtless.Visible = false;

            // btnLess.Enabled = false;
        }
        public void total()
        {
            double totalamount = 0.0D, totalquantity = 0.0D;
            double value = 0.0, value1 = 0.0;
            for (int i = 0; i < dgvNew.Rows.Count; i++)
            {

                if (string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[i].Cells[6].Value)))
                {
                    value = 0.0;
                }
                else
                {
                    value = Convert.ToDouble(dgvNew.Rows[i].Cells[6].Value);
                }

                if (string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[i].Cells[5].Value)))
                {
                    value1 = 0.0;
                }
                else
                {
                    value1 = Convert.ToDouble(dgvNew.Rows[i].Cells[5].Value);
                }

                totalamount = totalamount + Convert.ToInt32(dgvNew.Rows[i].Cells[6].Value);
                //totalquantity = totalquantity + Convert.ToInt32(dgvNew.Rows[i].Cells[5].Value);
            }

            //lbltotalquantity.Text = Convert.ToString(totalquantity);
            lbltotalamount.Text = String.Format("{0:00.00}", totalamount);

        }
        public void getEstimation(string s)
        {
            DataSet ds = objQuotationbal.GetEstimation_search(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                txtOrdNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
                Txtcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                cmdcity.Text = Convert.ToString(ds.Tables[0].Rows[0]["city"]);
                cmbreference.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Referenceid"]);
                cmbassistby.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Assist"]);
                lblperare.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);
                tatQuotationno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);
                date1.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);

                txtless.Text = Convert.ToString(ds.Tables[0].Rows[0]["LessAmount"]);
                lblTotal.Text = Convert.ToString(ds.Tables[0].Rows[0]["GrnandTotal"]);
                TxtOthers.Text = Convert.ToString(ds.Tables[0].Rows[0]["Others"]);
                txtRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);

            }

            if (ds.Tables[1].Rows.Count > 0)
            {
                dgvNew.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvNew.Rows.Add();
                    dgvNew.Rows[i].Cells[0].Value = i + 1;
                    dgvNew.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvNew.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvNew.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

                    double qty;

                    if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[i]["Rate"])))
                    {
                        qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    }
                    else
                    {
                        qty = 0;
                    }
                    dgvNew.Rows[i].Cells[4].Value = qty;
                    dgvNew.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    dgvNew.Rows[i].Cells[5].ReadOnly = false;

                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvNew.Rows[i].Cells[6].Value = amt;
                    button4.Visible = true;
                    //Btncancel.Visible = true;
                }

            }
            else
            {
                dgvNew.Rows.Clear();

            }

        }








        private void vLabel1_Click(object sender, EventArgs e)
        {
            if (pnlLabelSearch.Visible == true)
            {
                pnlLabelSearch.Visible = false;
                vLabel1.Visible = false;
                pnlSearch.Visible = true;
                splitContainer1.Panel1Collapsed = false;
            }
        }

        private void SalesQuotation1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }






        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            GetSearchSalesOrder();
            total1();
        }
        public void total1()
        {
            double totalamount = 0.0D;
            double value = 0.0;
            for (int i = 0; i < dgvSearch.Rows.Count; i++)
            {

                if (string.IsNullOrEmpty(Convert.ToString(dgvSearch.Rows[i].Cells["Amount"].Value)))
                {
                    value = 0.0;
                }
                else
                {
                    value = Convert.ToDouble(dgvSearch.Rows[i].Cells["Amount"].Value);
                }


                totalamount = totalamount + value;
                //totalquantity = totalquantity + Convert.ToInt32(dgvNew.Rows[i].Cells[5].Value);
            }

            //lbltotalquantity.Text = Convert.ToString(totalquantity);
            Totalamt.Text = String.Format("{0:0.00}", totalamount);

        }
        private void GetSearchSalesOrder()
        {
            string OrderNo = txtOrderNo.Text.Trim();
            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);
            string Vendorid;
            if (string.IsNullOrEmpty(cmbcustomer.Text))
            {
                Vendorid = null;
            }
            else
            {
                Vendorid = cmbcustomer.Text;
            }

            string Iscombined = string.Empty;
            if (all == true)
            {
                Iscombined = null;
            }
            else
            {
                if (cmbIscombined.Checked)
                {
                    Iscombined = "1";
                }
                else
                {
                    Iscombined = "0";
                }
            }
            string Paymentmode = Convert.ToString(Payment.Text);
            DataTable dt = objQuotationbal.searchQuotEstimationwithDate(OrderNo, FromDate, ToDate, Vendorid, Convert.ToInt32(Iscombined), Paymentmode);
          
            DataTable SearchResult = new DataTable();
           
            if (dt.Rows.Count > 0)
            {
                try
                {
                    if (textSearchQty.Text == "")
                        SearchResult = dt.Select("Products like '%" + txtSearchProduct.Text + "%'", "SiNo ASC").CopyToDataTable();
                    else
                        SearchResult = dt.Select("Products like '%" + txtSearchProduct.Text + "%' and QTY like '%|" + textSearchQty.Text + "|%'", "SiNo ASC").CopyToDataTable();
                }
                catch (Exception ex)
                {
                    SearchResult = new DataTable();
                }


              
               
               

            }


            lblItemCount.Text = dt.Rows.Count.ToString();
            
          //  Totalamt.Text = String.Format("{0:0,0.00}",totalamount + value);
            //try
            //{
            //    AlphanumComparator<string> comparer = new AlphanumComparator<string>();
            //    //DataTable dtNew = dv.Table;
            //    DataTable dtNew = SearchResult.AsEnumerable().OrderBy(x => x.Field<string>("Estimation No"), comparer).CopyToDataTable();
            //    //dtNew.TableName = "NaturalSort";
            //    SearchResult = dtNew;
            //}
            //catch (Exception ex)
            //{

            //}

            dgvSearch.Rows.Clear();
            for (int i = 0; i < SearchResult.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(SearchResult.Rows[i]["Order No"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(SearchResult.Rows[i]["Estimation No"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(SearchResult.Rows[i]["Customer"]);
                dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(SearchResult.Rows[i]["Order Date"]);
                dgvSearch.Rows[i].Cells[4].Value = Convert.ToString(SearchResult.Rows[i]["Status"]);
                dgvSearch.Rows[i].Cells[5].Value = Convert.ToDecimal(SearchResult.Rows[i]["Amount"]);
            }
                     


            dgvSearch.Columns["Estimation No"].Visible = true;
            dgvSearch.Columns["Status"].Visible = true;
            dgvSearch.Columns["Amount"].Visible = true;
            dgvSearch.Columns["Order No"].Visible = false;
            //dgvSearch.Columns["Order Date"].Visible = false;
            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //panel9.Enabled = false;
            all = false;
            total1();
            //if (dgvSearch.Rows.Count>0)
            //{
            //    dgvSearch.Focus();
            //    dgvSearch.CurrentCell=dgvSearch[0,0];
            //}

        }

        private void dgvSearch_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
           preview();
           
           
        }
        public void GetReport(string QuotationId)
        {
            try
            {
                if (!string.IsNullOrEmpty(QuotationId))
                {
                    DialogResult result = MessageBox.Show("Do you want to Print View "  + QuotationId + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Quotationreport rpt = new Quotationreport(txtorder.Text);
                        //rpt.ShowDialog();

                        using (SqlConnection con = new SqlConnection(Program.connection))
                        {
                            DataSet ds = new DataSet();
                            con.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.AddWithValue("@id", QuotationId);
                            cmd.Parameters.AddWithValue("@companyname", Program.Company);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "GetQuotationEstimationreport_print";
                            cmd.Connection = con;
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(ds);

                            EstimationReportDAL Obj = new EstimationReportDAL();
                            Obj.dsMain = ds;
                            if (Obj.GenerateQuoationReport())
                            {




                                frmPrintPreview objfrmpreview = new frmPrintPreview();
                                objfrmpreview.fileName = Obj.fileName;
                                objfrmpreview.Show();

                            }

                            //System.Diagnostics.Process myProc = new System.Diagnostics.Process();
                            //myProc.StartInfo.FileName = "type " + Obj.fileName + " >prn";  //Attempting to start a non-existing executable
                            //myProc.Start();    //Start the application and assign it to the process component.    
                            //ExecuteCommandSync("type " + Obj.fileName + " >prn");



                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please Select the Estimation");
                }

            }
            catch (Exception e)
            {

            }
        }
        //public void GetReport(string QuotationId)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(QuotationId))
        //        {
        //            DialogResult result = MessageBox.Show("Do you want to Print " + QuotationId + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        //            if (result == DialogResult.Yes)
        //            {
        //                // Quotationreport rpt = new Quotationreport(txtorder.Text);
        //                //rpt.ShowDialog();

        //                using (SqlConnection con = new SqlConnection(Program.connection))
        //                {
        //                    DataSet ds = new DataSet();
        //                    con.Open();
        //                    SqlCommand cmd = new SqlCommand();
        //                    cmd.Parameters.AddWithValue("@id", QuotationId);
        //                    cmd.Parameters.AddWithValue("@companyname", Program.Company);
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.CommandText = "GetQuotationEstimationreport_print";
        //                    cmd.Connection = con;
        //                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
        //                    ad.Fill(ds);

        //                    EstimationReportDAL Obj = new EstimationReportDAL();
        //                    Obj.dsMain = ds;
        //                    Obj.pagenumber = 1;
        //                    Obj.status = true;
        //                    if (Obj.GenerateQuoation())
        //                    {

        //                        page = Obj.pagenumber;

        //                        lblqid.Text = txtOrdNo.Text;
        //                        panel4.Visible = true;
        //                        rdrange.Checked = false;
        //                        rdall.Checked = true;
        //                        rdrange.Text = "Range(" + page + ")";
        //                        fileName = Obj.fileName;

        //                        textBox2.Text = string.Empty;
        //                        textBox2.Text = string.Empty;


        //                        //frmPrintPreview objfrmpreview = new frmPrintPreview();
        //                        //objfrmpreview.fileName = Obj.fileName;
        //                        //objfrmpreview.Show();

        //                    }

        //                    //System.Diagnostics.Process myProc = new System.Diagnostics.Process();
        //                    //myProc.StartInfo.FileName = "type " + Obj.fileName + " >prn";  //Attempting to start a non-existing executable
        //                    //myProc.Start();    //Start the application and assign it to the process component.    
        //                    //ExecuteCommandSync("type " + Obj.fileName + " >prn");



        //                }
        //            }
        //        }


        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}
        public void preview()
        {
            try
            {

                if (!string.IsNullOrEmpty(txtOrdNo.Text))
                {

                    //  Pnloading.Visible = false;

                    //MessageBox.Show("save successfully");



                    DialogResult result = MessageBox.Show("Do you want to Print" + txtOrdNo.Text + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        QuotationEstimationreport rpt = new QuotationEstimationreport(txtOrdNo.Text);
                        rpt.ShowDialog();
                    }

                }


                else
                {
                    MessageBox.Show("Please Select The Item");
                }
            }
            catch (Exception e)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOrdNo.Text))
            {
                DialogResult result = MessageBox.Show("Do you want to Cancel Estimation?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int v = objQuotationbal.savecancel(txtOrdNo.Text);
                    if (v == 1)
                    {
                        MessageBox.Show("Estimation Cancelled .");
                        clear();
                        GetSearchSalesOrder();

                    }
                    if (v == 2)
                    {
                        MessageBox.Show("Estimation Can't Cancel .");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Select Estimation .");
            }
        }


        //public void GetReport(string QuotationId)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(QuotationId))
        //        {
        //            DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        //            if (result == DialogResult.Yes)
        //            {
        //                // Quotationreport rpt = new Quotationreport(txtorder.Text);
        //                //rpt.ShowDialog();

        //                using (SqlConnection con = new SqlConnection(Program.connection))
        //                {
        //                    DataSet ds = new DataSet();
        //                    con.Open();
        //                    SqlCommand cmd = new SqlCommand();
        //                    cmd.Parameters.AddWithValue("@id", QuotationId);
        //                    cmd.Parameters.AddWithValue("@companyname", Program.Company);
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.CommandText = "GetQuotationEstimationreport_print";
        //                    cmd.Connection = con;
        //                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
        //                    ad.Fill(ds);

        //                    EstimationReportDAL Obj = new EstimationReportDAL();
        //                    Obj.dsMain = ds;
        //                    Obj.pagenumber = 1;
        //                    Obj.status = true;
        //                    if (Obj.GenerateQuoation())
        //                    {

        //                        page = Obj.pagenumber;

        //                        lblqid.Text = txtOrdNo.Text;
        //                        panel4.Visible = true;
        //                        rdrange.Checked = false;
        //                        rdall.Checked = true;
        //                        rdrange.Text = "Range(" + page + ")";
        //                        fileName = Obj.fileName;

        //                        textBox2.Text = string.Empty;
        //                        textBox2.Text = string.Empty;


        //                        //frmPrintPreview objfrmpreview = new frmPrintPreview();
        //                        //objfrmpreview.fileName = Obj.fileName;
        //                        //objfrmpreview.Show();

        //                    }



        //                    //System.Diagnostics.Process myProc = new System.Diagnostics.Process();
        //                    //myProc.StartInfo.FileName = "type " + Obj.fileName + " >prn";  //Attempting to start a non-existing executable
        //                    //myProc.Start();    //Start the application and assign it to the process component.    
        //                    //ExecuteCommandSync("type " + Obj.fileName + " >prn");



        //                }
        //            }
        //        }


        //    }
        //    catch(Exception e)
        //    {
        //        MessageBox.Show(e.Message.ToString());
        //    }
        //}
        public void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            GetReport(txtOrdNo.Text);//GetReport(txtOrdNo.Text);
        }

        private void dgvSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Down)
                {
                    if (dgvSearch.CurrentCell.RowIndex >= 0)
                    {
                        string s = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex + 1].Cells[1].Value);
                        if (!string.IsNullOrEmpty(s))
                        {
                            double totalamount = 0.00;

                            getEstimation(s);
                            total();
                            //totalamount = Convert.ToDouble(lbltotalamount.Text) - Convert.ToDouble(txtless.Text);

                            //lblTotal.Text = String.Format("{0:00.00}", totalamount);
                            Txtcustomername.Focus();
                        }
                        else
                        {
                            clear();
                        }

                    }
                }

                if (e.KeyData == Keys.Up)
                {
                    if (dgvSearch.CurrentCell.RowIndex >= 0)
                    {
                        string s = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex - 1].Cells[1].Value);
                        if (!string.IsNullOrEmpty(s))
                        {
                            double totalamount = 0.00;

                            getEstimation(s);
                            total();
                            //totalamount = Convert.ToDouble(lbltotalamount.Text) - Convert.ToDouble(txtless.Text);

                            //lblTotal.Text = String.Format("{0:00.00}", totalamount);
                            Txtcustomername.Focus();
                        }
                        else
                        {
                            clear();
                        }

                    }
                }
            }
            catch
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (rdrange.Checked)
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please Enter From Page");
                }
                else if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("Please Enter To Page");
                }

                else if (Convert.ToInt32(textBox1.Text) > Convert.ToInt32(textBox2.Text))
                {
                    MessageBox.Show("From Page Should Not Be Greater Than To Page");
                }
                else
                {
                    print();
                    clearprint();
                }
            }
            else
            {
                print();
                clearprint();
            }
        }

        public void clearprint()
        {
            panel4.Visible = false;
            rdall.Checked = false;
            rdrange.Checked = false;
            //lblfrom.Visible = false;
            //lblto.Visible = false;
            textBox2.Text = string.Empty;
            textBox2.Text = string.Empty;
            //textBox1.Visible = false;
            //textBox2.Visible = false;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            clearprint();
        }

        public void PrintBuffer()
        {

            Process cmd = new Process();
            cmd.StartInfo.FileName = "d:\\bill.bat";

            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            cmd.Start();


        }

        public void print()
        {

            if (rdall.Checked)
            {
                PrintBuffer();
                //frmPrintPreview objfrmpreview = new frmPrintPreview();
                //objfrmpreview.fileName = fileName;
                //objfrmpreview.Show();
            }

            else if (rdrange.Checked)
            {

                using (SqlConnection con = new SqlConnection(Program.connection))
                {
                    DataSet ds = new DataSet();
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.AddWithValue("@id", lblqid.Text);
                    cmd.Parameters.AddWithValue("@companyname", Program.Company);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetQuotationEstimationreport_print";
                    cmd.Connection = con;
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.Fill(ds);

                    EstimationReportDAL Obj = new EstimationReportDAL();
                    Obj.dsMain = ds;
                    Obj.pagenumber = 1;

                    Obj.status = false;
                    Obj.fpage = Convert.ToInt32(textBox1.Text);
                    Obj.lpage = Convert.ToInt32(textBox2.Text);
                    if (Obj.GenerateQuoation())
                    {


                        PrintBuffer();

                        //frmPrintPreview objfrmpreview = new frmPrintPreview();
                        //objfrmpreview.fileName = Obj.fileName;
                        //objfrmpreview.Show();

                    }

                }
            }
        }


        private bool Validation1()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(txtOrdNo.Text))
            {
                i++;

                message = message + "* Please Select Estimation" + "\n";
                
            }


            if (cmbreference.SelectedIndex == 0)
            {
                i++;
                message = message + "* Please Select Reference Name" + "\n";

            }



            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

        private void btnsave_Click_1(object sender, EventArgs e)
        {

            bool vali = Validation1();
            if (vali)
            {

                objQuotationbal.txtordno = txtOrdNo.Text;
                objQuotationbal.cmbreference = Convert.ToString(cmbreference.SelectedValue);
                string output = objQuotationbal.updaterefferenceid(objQuotationbal);

                if (output == "1")
                {

                    MessageBox.Show("Updated Sucessfully");
                    clear();
                }
            }
        }

        private void textSearchQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && (e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GetEstimationReport(txtOrdNo.Text);
        }
        public void GetEstimationReport(string QuotationId)
        {
            try
            {
                if (!string.IsNullOrEmpty(QuotationId))
                {
                    DialogResult result = MessageBox.Show("Do you want to Print " + QuotationId + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Quotationreport rpt = new Quotationreport(txtorder.Text);
                        //rpt.ShowDialog();

                        using (SqlConnection con = new SqlConnection(Program.connection))
                        {
                            DataSet ds = new DataSet();
                            con.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.AddWithValue("@id", QuotationId);
                            cmd.Parameters.AddWithValue("@companyname", Program.Company);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "GetQuotationEstimationreport_print_Direct";
                            cmd.Connection = con;
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(ds);

                            EstimationReportDAL Obj = new EstimationReportDAL();
                            Obj.dsMain = ds;
                            Obj.pagenumber = 1;
                            Obj.status = true;
                            if (Obj.GenerateQuoation())
                            {

                                page = Obj.pagenumber;

                                lblqid.Text = QuotationId;
                                panel4.Visible = true;
                                rdrange.Checked = false;
                                rdall.Checked = true;
                                rdrange.Text = "Range(" + page + ")";
                                fileName = Obj.fileName;

                                textBox2.Text = string.Empty;
                                textBox2.Text = string.Empty;


                                //frmPrintPreview objfrmpreview = new frmPrintPreview();
                                //objfrmpreview.fileName = Obj.fileName;
                                //objfrmpreview.Show();

                            }

                            //System.Diagnostics.Process myProc = new System.Diagnostics.Process();
                            //myProc.StartInfo.FileName = "type " + Obj.fileName + " >prn";  //Attempting to start a non-existing executable
                            //myProc.Start();    //Start the application and assign it to the process component.    
                            //ExecuteCommandSync("type " + Obj.fileName + " >prn");



                        }
                    }
                }


            }
            catch (Exception e)
            {

            }
        }
    }
}
