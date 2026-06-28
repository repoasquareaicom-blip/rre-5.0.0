using InvBal;
using RRLightsSaleBill;
using SalesReportDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Inventory.Sales
{
    public partial class SalesReportForm : Form
    {
        static string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

        QuotationBal objQuotationbal = new QuotationBal();
        TextBox tb;
        public bool edit = false;
        //string userid = "";
        int page;
        int hdnComis;
        string fileName;
        string test;
        string company = string.Empty;
        string firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue;
        public SalesReportForm()
        {
            InitializeComponent();
           
            bindLocation();
            cmbloaction.SelectedIndex = 0;
            cmbpaymode.SelectedIndex = 0;
            cmbstatus.SelectedIndex = 0;
            lblperare.Text = Program.UserName;
            LoadPorts();
            bindAssist();
            label36.Visible = false;
            textBox6.Visible = false;
            bindreference();
            label35.Visible = false;
            bindcustomer();
            Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            Txtitem.AutoCompleteCustomSource = AutoCompleteLoad();
            Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;



            btnSearch.PerformClick();
            SearchPurchaseOrder();
            this.WindowState = FormWindowState.Maximized;



            textBox6.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox6.AutoCompleteCustomSource = AutoCompleteLoads();
            textBox6.AutoCompleteSource = AutoCompleteSource.CustomSource;

            lblShopList.Text = Program.ShopName;
            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);

            int shop = Convert.ToInt32(lblShopList.Text);

            Bindcompany();
            cmbcompanychange.SelectedIndex = Convert.ToInt32(lblShopList.Text);

            getsearchdata();
        }


        public AutoCompleteStringCollection AutoCompleteLoads()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.getHSNautocomplteonly();
            string[] arr = new string[st.Rows.Count];
            for (int i = 0; i < st.Rows.Count; i++)
            {
                arr[i] = st.Rows[i]["HSN"].ToString();
            }

            for (int i = 0; i < arr.Length; i++)
            {
                //var combined = string.Join(", ", arr);
                var combined = arr[i];
                str.Add(combined);
            }

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}

            return str;
        }
        public void Bindcompany()
        {
            try
            {
                
                DataTable dt = LoginBAL.Getcompanyname();
                DataRow dr = dt.NewRow();
                dr["CompanyName"] = "0";
                dr["CompanyName"] = "--Select--";
                dt.Rows.InsertAt(dr, 0);
                string com = dt.Rows[1][1].ToString();
                if (com == "R.R.LIGHTS")
                {
                    hdnComis = 1;
                    foreach (DataRow dr1 in dt.Rows)
                    {
                        if (dr1["CompanyName"].ToString() == "R.R. PIPES")
                            dr1.Delete();
                    }
                }
                else
                {
                    hdnComis = 2;
                }
                cmbcompanychange.DataSource = dt;

                cmbcompanychange.ValueMember = "CompanyName";
                cmbcompanychange.DisplayMember = "CompanyName";
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message.ToString());
            }
        }
        public void recal()
        {
            lblShopList.Text = Program.ShopName;
            cmbcompanychange.SelectedIndex = Convert.ToInt32(lblShopList.Text);
            int shop = Convert.ToInt32(lblShopList.Text);
            bindLocation();
            cmbloaction.SelectedIndex = 0;
            cmbpaymode.SelectedIndex = 0;
            cmbstatus.SelectedIndex = 0;
            lblperare.Text = Program.UserName;
            LoadPorts();
            bindAssist();
            bindreference();
            bindcustomer();
            Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            Txtitem.AutoCompleteCustomSource = AutoCompleteLoad();
            Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;



            textBox6.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox6.AutoCompleteCustomSource = AutoCompleteLoads();
            textBox6.AutoCompleteSource = AutoCompleteSource.CustomSource;

            SearchPurchaseOrder();
            this.WindowState = FormWindowState.Maximized;
            btnSearch.PerformClick();
           
            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);



            getsearchdata();
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


            }
        }

        public void bindcustomer()
        {
            cmbcustomername.DataSource = objQuotationbal.Getcustomer();
            cmbcustomername.DisplayMember = "Name";
            cmbcustomername.ValueMember = "CustomerID";
        }

        public void bindLocation()
        {
            cmbloaction.DataSource = objQuotationbal.getLocation();
            cmbloaction.DisplayMember = "LocationName";
            cmbloaction.ValueMember = "LocationID";
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

        private void LoadPorts()
        {
            dgvOrder.Rows.Clear();
            dgvOrder.ColumnCount = 10;
            //dgvOrder.RowCount = 16;

            dgvOrder.Columns[0].Name = "S.NO";
            dgvOrder.Columns[1].Name = "Items";
            dgvOrder.Columns[3].Name = "UOM";
            dgvOrder.Columns[2].Name = "HSN";
            dgvOrder.Columns[7].Name = "Quantity";
            dgvOrder.Columns[4].Name = "productid";
            dgvOrder.Columns[5].Name = "Rate";
            dgvOrder.Columns[6].Name = "GST";
            dgvOrder.Columns[8].Name = "Amount";
            dgvOrder.Columns[9].Name = "orderQty";

            dgvOrder.Columns[4].Visible = false;

            this.dgvOrder.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[0].Width = 25;
            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;

           // dgvOrder.Columns[5].HeaderText = "GST";
            this.dgvOrder.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[1].Width = 150;
            this.dgvOrder.Columns[2].Width = 30;
            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns[1].ReadOnly = true;
            this.dgvOrder.Columns[2].ReadOnly = true;
            this.dgvOrder.Columns[5].ReadOnly = false;
            this.dgvOrder.Columns[6].ReadOnly = true;
            this.dgvOrder.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrder.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrder.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvOrder.Columns[5].DefaultCellStyle.Format = "F2";
            dgvOrder.Columns[7].DefaultCellStyle.Format = "F2";

            this.dgvOrder.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[5].Width = 30;
            //this.dgvOrder.Columns[4].ReadOnly = true;

            this.dgvOrder.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[2].Width = 35;

            this.dgvOrder.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[3].Width = 30;
            this.dgvOrder.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[6].Width = 30;

            this.dgvOrder.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[7].Width = 35;



            this.dgvOrder.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[8].Width = 90;

            this.dgvOrder.Columns[8].ReadOnly = true;

            this.dgvOrder.Columns[9].Visible = false;



            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvOrder.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvOrder.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }
        private void SearchPurchaseOrder()
        {
            dgvSearch.Rows.Clear();
            dgvSearch.Columns.Clear();
            dgvSearch.ColumnCount = 1;
            //dgvSearch.RowCount = 16;

            dgvSearch.Columns[0].Name = "Order No";



            this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;



            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn c in dgvSearch.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }


            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        }

        private void cmbcustomername_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = "";
            if (cmbcustomername.SelectedIndex > 0)
            {
                s = Convert.ToString(cmbcustomername.SelectedValue);
            }

            DataTable dt = objQuotationbal.getcityvalbyCustomerid(s);
            if (dt.Rows.Count > 0)
            {
                txtAddressLine1.Text = Convert.ToString(dt.Rows[0]["Address1"]);
                txtAddressLine2.Text = Convert.ToString(dt.Rows[0]["Address2"]);
                txtmobile.Text = Convert.ToString(dt.Rows[0]["Phone"]);
                cmdcity.Text = Convert.ToString(dt.Rows[0]["CITY"]);
                txttin.Text = Convert.ToString(dt.Rows[0]["Tin"]);
            }
            else
            {
                txtAddressLine1.Clear();
                txtAddressLine2.Clear();
                txtmobile.Clear();
                cmdcity.Clear();
                txttin.Clear();
            }


        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            getsearchdata();
        }


        public void getsearchdata()
        {
            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);
            int shop = Convert.ToInt32(lblShopList.Text);
            string Quty = string.Empty;
            Quty = textSearchQty.Text.Trim();
            if (Quty != "")
            {
                Quty = textSearchQty.Text.Trim();
            }
            else
            {
                Quty = null;
            }

            string ProductName = string.Empty;
            ProductName = txtSearchProduct.Text.Trim();
            search(FromDate, ToDate, shop, Quty,ProductName);



        }
        public AutoCompleteStringCollection AutoCompleteLoad()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.getproductautocomplteonly();
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

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}

            return str;
        }



        public void search(DateTime f, DateTime t, int shop, string Quty, string ProductName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlCommand cmd = null;

                if (shop == 1)
                {

                    cmd = new SqlCommand("searchsale_Direct1", con);
                }

                if (shop == 2)
                {
                    cmd = new SqlCommand("searchsalepipes_Direct1", con);

                }

                //SqlCommand cmd = new SqlCommand("searchsale", con);
                // SqlCommand cmd = new SqlCommand("searchsalepipes", con);
                cmd.Parameters.AddWithValue("@FromDate", f);
                cmd.Parameters.AddWithValue("@ToDate", t);
                cmd.Parameters.AddWithValue("@CustomerName", textBox4.Text.Trim());
                cmd.Parameters.AddWithValue("@Qty", Quty);
                cmd.Parameters.AddWithValue("@Product", ProductName);
                cmd.Parameters.AddWithValue("@OrderNumber", textBox5.Text.Trim());
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);

                ad.Fill(dt);

            }
           // DataTable dt = objQuotationbal.searchsale(f, t,shop);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Salesid"]);

            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            if(lblItemCount.Text=="0")
            {
                clear();
            }
        }

        public void getsaleamount(int shop)
        {
            string values = string.Empty;
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlCommand cmd = null;

                if (shop == 1)
                {

                    cmd = new SqlCommand("getsaleamount", con);
                }

                if (shop == 2)
                {
                    cmd = new SqlCommand("getsaleamountPipes_Direct1", con);

                }
                //SqlCommand cmd = new SqlCommand("getsaleamountPipes", con);
                cmd.CommandType = CommandType.StoredProcedure;
                values = Convert.ToString(cmd.ExecuteScalar());
                con.Close();
            }
            string s = values;
            //string s = objQuotationbal.getsaleamount(shop);
            if (!string.IsNullOrEmpty(s))
            {
                lbltodaysales.Text = s;
            }
            else
            {
                lbltodaysales.Text = "0";
            }
        }

        private void SalesReportForm_Load(object sender, EventArgs e)
        {
            int shop = Convert.ToInt32(lblShopList.Text);
            getsaleamount(shop);
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string  aa=Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                int shop = Convert.ToInt32(lblShopList.Text);
                getEstimation(aa,shop);
            }
        }

        public void getEstimation(string s,int shop)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlCommand cmd = null;

                if (shop == 1)
                {

                    cmd = new SqlCommand("Getsaleslist_Direct2", con);
                }

                if (shop == 2)
                {
                    cmd = new SqlCommand("Getsaleslistpipes_Direct2", con);

                }
                //SqlCommand cmd = new SqlCommand("Getsaleslistpipes", con);
                // SqlCommand cmd = new SqlCommand("Getsaleslist", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@id", val);
                cmd.Parameters.AddWithValue("@id", s);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(ds);
                con.Close();
            }
           // DataSet ds = objQuotationbal.Getsaleslist(s,shop);
            if (ds.Tables[0].Rows.Count > 0)
            {


                txtorder.Text = Convert.ToString(ds.Tables[0].Rows[0]["Salesid"]);
                cmbcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                cmdcity.Text = Convert.ToString(ds.Tables[0].Rows[0]["city"]);
                if (Convert.ToInt32(ds.Tables[0].Rows[0]["Referenceid"]) > 0)
                {
                    cmbreference.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Referenceid"]);
                }
                if (Convert.ToInt32(ds.Tables[0].Rows[0]["Assist"]) > 0)
                {
                    cmbassistby.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Assist"]);
                }


                lblperare.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);
                cmbpaymode.Text = Convert.ToString(ds.Tables[0].Rows[0]["Paymentmode"]);
                txtdate.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["Updatedon"]);

                lbltotalamount.Text = Convert.ToString(ds.Tables[0].Rows[0]["TotalAmount"]);
                txtless.Text = Convert.ToString(ds.Tables[0].Rows[0]["LessAmount"]);
                // lblTotal.Text = Convert.ToString(ds.Tables[0].Rows[0]["GrandTotal"]);
                txtAddressLine1.Text = Convert.ToString(ds.Tables[0].Rows[0]["Address1"]);
                txtAddressLine2.Text = Convert.ToString(ds.Tables[0].Rows[0]["Address2"]);
                txttin.Text = Convert.ToString(ds.Tables[0].Rows[0]["Tin"]);
                txtmobile.Text = Convert.ToString(ds.Tables[0].Rows[0]["Mobile"]);
                Txtothers.Text = Convert.ToString(ds.Tables[0].Rows[0]["others"]);




            }
            else
            {

                // dgvOrder.Enabled = false;


            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                // dgvOrder.Enabled = false;
                dgvOrder.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {

                    dgvOrder.Rows.Add();
                    dgvOrder.Rows[i].Cells[0].Value = i + 1;
                    dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["HSN"]);
                    dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvOrder.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

                    double qty;

                    if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[i]["Rate"])))
                    {
                        qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    }
                    else
                    {
                        qty = 0;
                    }
                    dgvOrder.Rows[i].Cells[5].Value = qty;
                    dgvOrder.Rows[i].Cells[6].Value = Convert.ToString(ds.Tables[1].Rows[i]["GST"]);
                    dgvOrder.Rows[i].Cells[7].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    dgvOrder.Rows[i].Cells[7].ReadOnly = false;

                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvOrder.Rows[i].Cells[8].Value = amt;
                }

            }
            else
            {
                // dgvOrder.Enabled = false;
                dgvOrder.Rows.Clear();

                clear();
                // dgvOrder.Enabled = false;
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }
        private void clear()
        {
            pnsearch.Visible = false;
            lblHSNCode.Text = "0";
            btnSave.Enabled = true;
            btnPrint.Enabled = true;
            cmbcustomername.Text = "--Select--";
            cmdcity.Text = string.Empty;
            cmbassistby.SelectedIndex = 0;
            cmbreference.SelectedIndex = 0;
            txtorder.Clear();
            dgvOrder.Rows.Clear();
            lblperare.Text = Program.UserName;
            lbltotalquantity.Text = "0";
            lbltotalamount.Text = "0.00";
            cmbcustomername.Focus();
            cmbloaction.SelectedIndex = 0;
            panel2.Enabled = true;
            cmbpaymode.SelectedIndex = 0;
            lblgrandtotal.Text = "0.00";
            txtless.Text = "0.00";
            txtAddressLine1.Clear();
            txtAddressLine2.Clear();
            txttin.Clear();
            txtmobile.Clear();
            txtdate.Value = DateTime.Now.Date;
            Txtothers.Text = "0";

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {


            if (!string.IsNullOrEmpty(txtorder.Text))
            {
                DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                  //  if (txtorder.Text.IndexOf("GST") >= 0)
                   // {

                        Program.PrintInvoiceNumber = txtorder.Text;
                        frmInvoice frmInvoice = new frmInvoice();
                        frmInvoice.Show();
                   // }
                    //else
                    //{


                       // Salesbillreport rpt = new Salesbillreport(txtorder.Text);
                       // rpt.ShowDialog();
                  //  }


                   
                }
            }
            else
            {
                MessageBox.Show("Please Select Sales Item");
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool val = Validation();



            if (val)
            {
                save();

                if(test=="1")
                {

                }
                else{
                    clear();
                }
                
            }
        }

        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (cmbcustomername.Text == "--Select--")
            {
                i++;
                message = message + "* Please Enter Customer Name" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbcustomername;
            }



            if (cmbpaymode.SelectedIndex <= 0)
            {
                i++;
                message = message + "* Please select Paymentmode" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbpaymode;
            }


            if (dgvOrder.Rows.Count > 0)
            {
                i++;
                string Items = Convert.ToString(dgvOrder.Rows[0].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvOrder.Rows[0].Cells["Items"].Value);
                if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                {
                    message = message + "* Please Enter Product" + "\n";
                }

                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }
            else if (dgvOrder.Rows.Count == 0)
            {
                i++;
                message = message + "* Please Enter Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }

            else if (string.IsNullOrEmpty(txtorder.Text))
            {
                i++;
                message = message + "* Please Select Sales Order " + "\n";
                if (i == 1)
                    this.ActiveControl = dgvSearch;
            }

            bool sas = false;

            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                string Items = Convert.ToString(dgvOrder.Rows[k].Cells["Quantity"].Value);

                //string Received = Convert.ToString(dgvOrder.Rows[k].Cells["Items"].Value);
                //string rate = Convert.ToString(dgvOrder.Rows[k].Cells["Rate"].Value);






                if ((!string.IsNullOrEmpty(Items) && Items == "." || Items == "-" || Items == ".-" || Items == "-." || Items == "0"))
                {
                    sas = true;
                    break;
                }
                else
                {
                }



            }


            if (sas == true)
            {
                i++;
                message = message + "*Quantity should not be empty." + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }




            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
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

        public void RemoveNullColumnFromDataTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][3])))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }

        public void save()
        {




            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(txtorder.Text))
            {
                objQuotationbal.isnew = 0;
            }
            else
            {
                objQuotationbal.isnew = 1;
            }

            objQuotationbal.salesid = txtorder.Text;
            objQuotationbal.Customerid = Convert.ToString(cmbcustomername.SelectedValue);
            objQuotationbal.Customername = Convert.ToString(cmbcustomername.Text);
            objQuotationbal.City = Convert.ToString(cmdcity.Text);
            objQuotationbal.Referenceid = Convert.ToString(cmbreference.SelectedValue);
            objQuotationbal.Assist = Convert.ToString(cmbassistby.SelectedValue);
            objQuotationbal.Paymentmode = Convert.ToString(cmbpaymode.Text);
            objQuotationbal.Total = Convert.ToString(lbltotalamount.Text);
            if (string.IsNullOrEmpty(txtless.Text))
            {
                objQuotationbal.Lessamount = "0";
            }
            else
            {
                objQuotationbal.Lessamount = Convert.ToString(txtless.Text);
            }
            if (string.IsNullOrEmpty(Txtothers.Text))
            {
                Txtothers.Text = "0";
            }

            objQuotationbal.Grandtotal = Convert.ToString(Convert.ToDouble(lbltotalamount.Text) + Convert.ToDouble(Txtothers.Text) - Convert.ToDouble(objQuotationbal.Lessamount));
            objQuotationbal.Updatedby = Program.userid;
            objQuotationbal.Address1 = txtAddressLine1.Text;
            objQuotationbal.Address2 = txtAddressLine2.Text;
            objQuotationbal.Tin = txttin.Text;
            objQuotationbal.others = Txtothers.Text;
            objQuotationbal.Mobile = txtmobile.Text;
            DateTime date = new DateTime(txtdate.Value.Year, txtdate.Value.Month, txtdate.Value.Day);
            objQuotationbal.date = date;
            dt = DataGridView2DataTable(dgvOrder);
            for (int i = 0; i < 4; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            dt.Columns.RemoveAt(2);
            dt.Columns.RemoveAt(4);
            RemoveNullColumnFromDataTable(dt);
            //foreach (DataRow row in dt.Rows)
            //{
            //    if (row["Quantity"].ToString() == "0")
            //    {
            //        //MessageBox.Show("Quanityy");

            //       test = "1";
            //    }
            //    else
            //    {
            //        test = "0";
            //    }
            //}

            //if(test=="1")
            //{
            //    MessageBox.Show("Quantity Should not be Zero");
            //}
            //else
            //{
                int shop = Convert.ToInt32(lblShopList.Text);
                string value;
                using (SqlConnection con = new SqlConnection(Conn))
                {
                    con.Open();
                    SqlCommand cmd = null;

                    if (shop == 1)
                    {

                        cmd = new SqlCommand("updateQuotationsales", con);
                    }

                    if (shop == 2)
                    {
                        cmd = new SqlCommand("updateQuotationsalespipes_Direct", con);

                    }

                    //SqlCommand cmd = new SqlCommand("updateQuotationsalespipes", con);
                    //SqlCommand cmd = new SqlCommand("updateQuotationsales", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@salesid", txtorder.Text);
                    cmd.Parameters.AddWithValue("@Customerid", Convert.ToString(cmbcustomername.SelectedValue));
                    cmd.Parameters.AddWithValue("@Customername", Convert.ToString(cmbcustomername.Text));
                    cmd.Parameters.AddWithValue("@City", Convert.ToString(cmdcity.Text));
                    cmd.Parameters.AddWithValue("@Referenceid", Convert.ToString(cmbreference.SelectedValue));
                    cmd.Parameters.AddWithValue("@Assist", Convert.ToString(cmbassistby.SelectedValue));
                    cmd.Parameters.AddWithValue("@Updatedby", Program.userid);
                    cmd.Parameters.AddWithValue("@paymentmode", Convert.ToString(cmbpaymode.Text));
                    cmd.Parameters.AddWithValue("@TotalAmount", Convert.ToString(lbltotalamount.Text));
                    if (string.IsNullOrEmpty(txtless.Text))
                    {
                        cmd.Parameters.AddWithValue("@LessAmount", "0");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@LessAmount", Convert.ToString(txtless.Text));
                    }

                    cmd.Parameters.AddWithValue("@GrandTotal", Convert.ToString(Convert.ToDouble(lbltotalamount.Text) + Convert.ToDouble(Txtothers.Text) - Convert.ToDouble(objQuotationbal.Lessamount)));
                    cmd.Parameters.AddWithValue("@address1", txtAddressLine1.Text);
                    cmd.Parameters.AddWithValue("@date", date);
                    if (string.IsNullOrEmpty(Txtothers.Text))
                    {
                        cmd.Parameters.AddWithValue("@others", "0");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@others", Convert.ToString(Txtothers.Text));
                    }

                    if (string.IsNullOrEmpty(txtAddressLine2.Text))
                    {
                        cmd.Parameters.AddWithValue("@address2", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@address2", txtAddressLine2.Text);
                    }
                    cmd.Parameters.AddWithValue("@tin", txttin.Text);
                    cmd.Parameters.AddWithValue("@mobile", txtmobile.Text);
                    cmd.Parameters.AddWithValue("@QuotationDetails", dt);
                    cmd.Parameters.Add("@result", SqlDbType.VarChar, 100);
                    cmd.Parameters["@result"].Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    value = Convert.ToString(cmd.Parameters["@result"].Value);
                    con.Close();
                }



                string output = value;
                //string output = objQuotationbal.updateQuotationsales(objQuotationbal, dt,shop);
                if (!string.IsNullOrEmpty(output))
                {

                    getsaleamount(shop);

                    DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {

                        // int dateVal = 7;
                        if (txtorder.Text.IndexOf("GST") >= 0)
                        {

                            Program.PrintInvoiceNumber = txtorder.Text;
                            frmInvoice frmInvoice = new frmInvoice();
                            frmInvoice.Show();
                        }
                        else
                        {


                            int shops = Convert.ToInt32(lblShopList.Text);
                            string bb = txtorder.Text;
                            GetReport(bb, shops);
                        }
                    }

               // }
                btnSearch.PerformClick();
            }
           

        }

        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            int aa = e.ColumnIndex;

            try
            {
                if (e.ColumnIndex == 7)
                {

                    double ordqty = Convert.ToDouble(dgvOrder.Rows[e.RowIndex].Cells[7].Value);
                    double qty = Convert.ToDouble(tb.Text);
                    if (ordqty > 0)
                    {
                        if (ordqty < qty)
                        {
                            MessageBox.Show("Quantity should  not be greater than original quantity");
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder[6, e.RowIndex];
                            dgvOrder.Rows[e.RowIndex].Cells[6].Value = ordqty;

                        }
                        else
                        {
                            dgvOrder.Focus();
                            //edit = true;
                            dgvOrder.CurrentCell = dgvOrder[6, e.RowIndex];
                        }
                    }
                }
                dgvOrder.CurrentCell = dgvOrder[1, e.RowIndex];
            }
            catch
            {

            }
        }

        private void dgvOrder_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (edit == true)
                {
                    if (dgvOrder.CurrentCell.RowIndex >= 1)
                    {
                        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex - 1];
                        edit = false;
                    }
                    else if (dgvOrder.CurrentCell.RowIndex == 0)
                    {
                        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex - 1];
                    }

                }
            }
            catch
            {

            }
        }


        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //total();
            //if (e.ColumnIndex == 1)
            //{
            //    pnsearch.Visible = true;
            //    this.ActiveControl = Txtitem;
         
               
            //    lblrowindex.Text = e.RowIndex.ToString();
            //    lblpageno.Text = Convert.ToString(Convert.ToInt32(lblpageno.Text) + 1);
            //}
            //else
            //{
            //    pnsearch.Visible = false; 
            //}



            total();
            if (e.ColumnIndex == 1)
            {
                //rdbStartsWith.Checked = true;
                if (dgvOrder.ReadOnly == false)
                {
                    pnsearch.Visible = true;
                }

                Txtitem.SelectionStart = 0;
                Txtitem.SelectionLength = Txtitem.Text.Length;
                dgvOrder.Focus();
                if (radioButton1.Checked)
                {
                    this.ActiveControl = Txtitem;
                }
                else
                {
                    this.ActiveControl = textBox6;
                }



            
                // dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
              
                lblrowindex.Text = e.RowIndex.ToString();
                lblpageno.Text = Convert.ToString(Convert.ToInt32(lblpageno.Text) + 1);
            }
        }


        private void dgvOrder_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double rate = Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value);
                if (!String.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value)))
                {
                    double amt = 0;
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value)))
                    {
                        amt = rate * Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value);
                        if (amt > 0)
                        {
                            string[] str = amt.ToString().Split('.');
                            if (str.Length > 1)
                            {
                                double num1 = Convert.ToDouble("0." + str[1]);

                                if (num1 >= 0.5)
                                {
                                    amt = Math.Ceiling(amt);
                                }
                                else
                                {
                                    amt = Math.Floor(amt);
                                }

                            }
                        }
                        else
                        {
                            string[] str = amt.ToString().Split('.');
                            if (str.Length > 1)
                            {
                                double num1 = Convert.ToDouble("0." + str[1]);

                                if (num1 >= 0.5)
                                {
                                    amt = Math.Floor(amt);
                                }
                                else
                                {
                                    amt = Math.Ceiling(amt);
                                }

                            }
                        }

                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value = amt;
                    }
                }

            }
            catch
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value)))
                    {
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value = 0.00;
                    }
                }
            }
            total();

        }

        private void dgvOrder_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double rate = Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value);
                if (!String.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value)))
                {
                    double amt = 0;
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value)))
                    {
                        amt = rate * Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value);
                        if (amt > 0)
                        {
                            string[] str = amt.ToString().Split('.');
                            if (str.Length > 1)
                            {
                                double num1 = Convert.ToDouble("0." + str[1]);

                                if (num1 >= 0.5)
                                {
                                    amt = Math.Ceiling(amt);
                                }
                                else
                                {
                                    amt = Math.Floor(amt);
                                }

                            }
                        }
                        else
                        {
                            string[] str = amt.ToString().Split('.');
                            if (str.Length > 1)
                            {
                                double num1 = Convert.ToDouble("0." + str[1]);

                                if (num1 >= 0.5)
                                {
                                    amt = Math.Floor(amt);
                                }
                                else
                                {
                                    amt = Math.Ceiling(amt);
                                }

                            }
                        }

                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value = amt;
                    }
                }

            }
            catch
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value)))
                    {
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value = 0.00;
                    }
                }
            }
            total();
        }

        private void dgvOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvOrder.CurrentCell.ColumnIndex;
            string headerText = dgvOrder.Columns[column].HeaderText;

            if (headerText.Equals("Quantity"))
            {
                tb = e.Control as TextBox;


                if (tb != null)
                {
                    tb.TextChanged += new EventHandler(textbox_Change);
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.MaxLength = 10;
                }
            }
        }

        private void dgvOrder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                
                    e.SuppressKeyPress = true;
                    if (dgvOrder.CurrentCell.ColumnIndex == 0)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 1)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 2)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[4, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 4)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[5, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 5)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[6, dgvOrder.CurrentCell.RowIndex];

                    }

                    //else if (dgvOrder.CurrentCell.ColumnIndex == 6)
                    //{
                    //    dgvOrder.Focus();
                    //    dgvOrder.CurrentCell = dgvOrder[7, dgvOrder.CurrentCell.RowIndex];

                    //}
                    else if (dgvOrder.CurrentCell.ColumnIndex == 8)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];

                    }



                }
           

            catch
            {

            }
        }

        public void total()
        {
            try
            {
                double totalamount = 0, totalquantity = 0;
                for (int i = 0; i < dgvOrder.Rows.Count; i++)
                {
                    totalamount = totalamount + Convert.ToDouble(dgvOrder.Rows[i].Cells[8].Value);
                    totalquantity = totalquantity + Convert.ToDouble(dgvOrder.Rows[i].Cells[7].Value);
                }


                lbltotalquantity.Text = Convert.ToString(totalquantity);

                string[] str = totalamount.ToString().Split('.');
                if (str.Length > 1)
                {
                    double num1 = Convert.ToDouble("0." + str[1]);

                    if (num1 >= 0.5)
                    {
                        totalamount = Math.Ceiling(totalamount);
                    }
                    else
                    {
                        totalamount = Math.Floor(totalamount);
                    }

                }


                // totalamount = Math.Round(totalamount);

                lbltotalamount.Text = totalamount.ToString("F2");
                totalamount = totalamount + Convert.ToDouble(Txtothers.Text) - Convert.ToDouble(txtless.Text);

                totalamount = totalamount - Convert.ToDouble(Txtothers.Text);

                lblgrandtotal.Text = totalamount.ToString("F2");
            }
            catch
            {

            }

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
            try
            {
                if (textSearchQty.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        btnSearch.Focus();
                        return true;
                    }
                }
                if (btnSearch.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        cmbcustomername.Focus();
                        return true;
                    }
                }
                if (txtmobile.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        txtdate.Focus();
                        return true;
                    }
                }
                //if (txtdate.Focused)
                //{
                //    if (keyData == (Keys.Tab))
                //    {
                //        txtdate.Focus();
                //        return true;
                //    }
                //}

                if (radioButton1.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = radioButton2;
                        return true;


                        // return true;
                    }
                }

                if (radioButton2.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = Txtitem;
                        return true;


                        // return true;
                    }
                }

                if (transactionclose.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        cmbpaymode.Focus();
                        return true;
                    }
                }
                if (cmbpaymode.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        btnSave.Focus();
                        return true;
                    }
                }
                if (btnSave.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        btnPrint.Focus();
                        return true;
                    }
                }
                if (btnPrint.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        button1.Focus();
                        return true;
                    }
                }
                if (button1.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        dtfromdate.Focus();
                        return true;
                    }
                }
                if (keyData == (Keys.Alt | Keys.I))
                {
                    radioButton1.Checked = true;
                    return true;
                }
                if (keyData == (Keys.Alt | Keys.H))
                {
                    radioButton2.Checked = true;
                    return true;
                }

                try
                {
                    if (keyData == Keys.Tab)
                    {
                        if(radioButton1.Checked)
                        {
                            if (dgvOrder.CurrentCell.ColumnIndex == 8)
                            {
                                dgvOrder.Focus();
                                //edit = true;
                                dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                                Txtitem.Focus();
                               
                            }
                        }

                        else if (radioButton2.Checked)
                        {
                            
                            
                                if (dgvOrder.CurrentCell.ColumnIndex == 8)
                                {
                                    dgvOrder.Focus();
                                    //edit = true;
                                    dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                                    textBox6.Focus();

                                }
                            
                        }
                       // textBox6.Focus();
                    }
                }
                catch
                {

                }
                //if (keyData == Keys.Tab)
                //{
                //    if (dgvOrder.CurrentCell.ColumnIndex ==6)
                //    {
                //        dgvOrder.Focus();
                //        //edit = true;
                //        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                //        return true;
                //    }
                //}


                
               // 

                //if (transactionclose.Focused)
                //{
                //    if (keyData == (Keys.Tab))
                //    {

                //        txtdate.Focus();

                //        this.ActiveControl = txtdate;

                //        return true;
                //    }
                //}

                //if (txtdate.Focused)
                //{
                //    if (keyData == Keys.Tab)
                //    {
                //        if (dgvOrder.CurrentCell.ColumnIndex == 1)
                //        {
                //            dgvOrder.Focus();
                //            //edit = true;
                //            dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                //        }
                //    }
                //}
                if (txtdate.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {


                        if (radioButton1.Checked)
                        {
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex + 1, dgvOrder.CurrentCell.RowIndex];
                            this.ActiveControl = dgvOrder;
                            //dgvOrder.BeginEdit(true);
                            Txtitem.Focus();
                           // textBox6.Focus();
                            pnsearch.Visible = true;
                            return true;
                        }

                        else if (radioButton2.Checked)
                        {

                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex + 1, dgvOrder.CurrentCell.RowIndex];
                            this.ActiveControl = dgvOrder;
                            //dgvOrder.BeginEdit(true);
                            //Txtitem.Focus();
                            textBox6.Focus();
                            pnsearch.Visible = true;
                            return true;

                        }




                        //if (dgvOrder.CurrentCell.ColumnIndex == 1)
                        //{
                        //this.ActiveControl = txtRemarks;
                       
                        //}
                    }
                }
                }
        
            
            catch(Exception ex)
            {
               // MessageBox.Show(ex.ToString());
            }
           
                try
                {

                    if (keyData == (Keys.Alt | Keys.Delete))
                    {
                        DialogResult result = MessageBox.Show("Do you want to Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            if (dgvOrder.Rows.Count > 0)
                            {
                                try
                                {
                                    int rowindex = dgvOrder.CurrentRow.Index;
                                    int colindex = dgvOrder.CurrentCell.ColumnIndex;
                                    dgvOrder.Rows.RemoveAt(rowindex);
                                }
                                catch
                                {
                                    if (dgvOrder.Rows.Count - 1 == dgvOrder.CurrentCell.RowIndex)
                                    {
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[0].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = "";
                                       // dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value = "";

                                    }
                                }

                            }
                            pnsearch.Visible = false;
                           
                            return true;
                        }


                    }

                //    if (keyData == (Keys.Control | Keys.Delete))
                //    {
                //        DialogResult result = MessageBox.Show("Do you want to Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //        if (result == DialogResult.Yes)
                //        {
                //            string s = txtorder.Text;
                //            int shop = Convert.ToInt32(lblShopList.Text);
                            
                //            using (SqlConnection con = new SqlConnection(Conn))
                //            {
                //                con.Open();
                //                SqlCommand cmd = null;
                //                if (shop == 1)
                //                {
                //                    cmd = new SqlCommand("deletesaleslistbill", con);
                //                }
                //                if (shop == 2)
                //                {
                //                    cmd = new SqlCommand("deletesalespipeslistbill", con);

                //                }                               
                //                cmd.CommandType = CommandType.StoredProcedure;
                //                cmd.Parameters.AddWithValue("@requestid", s);
                //                cmd.Parameters.AddWithValue("@outid", SqlDbType.Int).Direction = ParameterDirection.Output;
                //                cmd.ExecuteNonQuery();
                //                int val = Convert.ToInt32(cmd.Parameters["@outid"].Value);
                //                con.Close();

                //            }
                //            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
                //            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);

                //            getsaleamount(shop);
                //            search(FromDate, ToDate, shop);
                //            clear();
                //            return true;
                //        }


                //    }
                }
                catch
                {

                }
               

            



           
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void textbox_Change(object sender, EventArgs e)
        {
            //try
            //{

            //    double rate = Convert.ToInt32(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
            //    double amt = rate * Convert.ToDouble(tb.Text);
            //    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = amt;

            //}
            //catch
            //{
            //    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = 0.00;
            //}
        }
        Regex reg = new Regex(@"^-?\d+[.]?\d*$");
        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (!reg.IsMatch(tb.Text.Insert(tb.SelectionStart, e.KeyChar.ToString()) + "1")) e.Handled = true;
        }


        private void Txtitem_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(Txtitem.Text))
                    {
                        if (Convert.ToInt32(lblproductid.Text) != 0)
                        {
                            dgvOrder.Columns[6].DefaultCellStyle.Format = "F2";
                            int rowindex = Convert.ToInt32(lblrowindex.Text);
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7];
                            dgvOrder.Rows[rowindex].Cells[2].Value = lbelhsncode.Text;
                            dgvOrder.Rows[rowindex].Cells[4].Value = lblproductid.Text;
                            dgvOrder.Rows[rowindex].Cells[1].Value = Txtitem.Text.ToUpper();
                            dgvOrder.Rows[rowindex].Cells[3].Value = lblitemcode.Text;
                            double val = Convert.ToDouble(lblprice.Text);
                            dgvOrder.Rows[rowindex].Cells[5].Value = val;
                            dgvOrder.Rows[rowindex].Cells[6].Value = lblVat.Text;
                            dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                            pnsearch.Visible = false;
                            lblproductid.Text = string.Empty;
                            Txtitem.Text = string.Empty;
                            lblitemcode.Text = "0";
                            lblHSNCode.Text = "0";
                            lbldisplay.Text = "0";
                            lbldemo.Text = "0";
                            lblservice.Text = "0";
                            lbldamage.Text = "0";
                            lblprice.Text = "0";
                            lbelhsncode.Text = "0";
                            lblVat.Text = "0";
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7];
                        }
                        else
                        {
                            MessageBox.Show("Please Enter Correct Product Name");
                        }
                    }
                    else
                    {

                        this.ActiveControl = btnSave;
                        pnsearch.Visible = false;

                        //MessageBox.Show("Please Enter Product Name");
                        //Txtitem.Focus();
                    }
                }
            }

            catch (Exception ex)
            {

            }
            
            
        }

        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox6.Visible = false;
                Txtitem.Visible = true;
                label35.Visible = false;
                label34.Visible = true;
                textBox6.Text = "";
                itemdetails();
            }
            else if (radioButton2.Checked)
            {
                textBox6.Visible = true;
                Txtitem.Visible = false;
                label35.Visible = true;
                Txtitem.Text = "";
                label34.Visible = false;
                itemdetailsHSN();
            }
        }
        public void itemdetails()
        {

            try
            {
                lblproductid.Text = "";
                string s1 = Txtitem.Text.Trim();
                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s1.Replace("'", "''");


                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                //DataTable st = objQuotationbal.itemdetails1(name, s2);
                DataTable st = objQuotationbal.itemdetailssales(name, s2);

                if (st.Rows.Count > 0)
                {
                    lblitem.Text = name;
                    //pnitemdetails.Visible = true;
                    lblitemcode.Text = Convert.ToString(st.Rows[0]["UOM"]);
                    if (lblitemcode.Text == "")
                    {
                        lblitemcode.Text = "0";
                    }


                    lblproductid.Text = Convert.ToString(st.Rows[0]["ProductId"]);
                    if (lblproductid.Text == "")
                    {
                        lblproductid.Text = "0";
                    }
                    lblHSNCode.Text = Convert.ToString(st.Rows[0]["HSN"]);
                    if (lblHSNCode.Text == "")
                    {
                        lblHSNCode.Text = "0";
                    }
                    lblprice.Text = Convert.ToString(st.Rows[0]["Price"]);
                    if (lblprice.Text == "")
                    {
                        lblprice.Text = "0";
                    }

                    lblVat.Text = Convert.ToString(st.Rows[0]["GST"]);
                    if (lblVat.Text == "")
                    {
                        lblVat.Text = "0";
                    }

                    lbelhsncode.Text = Convert.ToString(st.Rows[0]["HSN"]);
                    if (lbelhsncode.Text == "")
                    {
                        lbelhsncode.Text = "0";
                    }
                    //DefaultFloor.Text = Convert.ToString(st.Rows[0]["DefaultFloor"]);
                    //if (DefaultFloor.Text == "")
                    //{
                    //    DefaultFloor.Text = "0";
                    //}

                    //Checking.Text = Convert.ToString(st.Rows[0]["Checking"]);
                    //if (Checking.Text == "")
                    //{
                    //    Checking.Text = "0";
                    //}


                    //Display.Text = Convert.ToString(st.Rows[0]["Display"]);

                    //if (Display.Text == "")
                    //{
                    //    Display.Text = "0";
                    //}


                    //Damage.Text = Convert.ToString(st.Rows[0]["Damage"]);
                    //if (Damage.Text == "")
                    //{
                    //    Damage.Text = "0";
                    //}

                    //Delivery.Text = Convert.ToString(st.Rows[0]["Delivery"]);
                    //if (Delivery.Text == "")
                    //{
                    //    Delivery.Text = "0";
                    //}





                    //pictureBox1.ImageLocation = Path.GetFullPath("131353W24J150-43D6.jpg");
                    //pictureBox1.ImageLocation = itemdetails.ToList()[0].imagepath;




                }
                else
                {
                    //pnitemdetails.Visible = false;
                    //lblitemcode.Text = "0";
                    //lblproductid.Text = "0";
                    lblprice.Text = "0";
                    lblHSNCode.Text = "0";
                    //lbldisplay.Text = "0";


                }

            }
            catch (Exception e)
            {

            }

        }
        private void transactionclose_Click(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
            lblproductid.Text = string.Empty;
            Txtitem.Text = string.Empty;
            lblitemcode.Text = "0";
            label36.Text = string.Empty;
            radioButton1.Checked = true;
            radioButton2.Checked = false; ;
            lbldisplay.Text = "0";
            lbldemo.Text = "0";
            lblservice.Text = "0";
            lbldamage.Text = "0";
            lblprice.Text = "0";
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            lblHSNCode.Text = "0";
            cmbpaymode.Focus();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtless_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtless.Text))
                {

                    double total = Convert.ToDouble(lbltotalamount.Text);
                    if (string.IsNullOrEmpty(txtless.Text))
                    {
                        txtless.Text = "0";
                    }
                    if (string.IsNullOrEmpty(Txtothers.Text))
                    {
                        Txtothers.Text = "0";
                    }
                    double less = Convert.ToDouble(txtless.Text);

                    if (total < less)
                    {
                        MessageBox.Show("Less amount shold not be greater than actual amount");
                        txtless.Text = "0";
                        txtless.SelectionStart = 0;
                        txtless.SelectionLength = 1;
                        txtless.Focus();
                        lblgrandtotal.Text = Convert.ToDouble(lbltotalamount.Text).ToString("F2");

                    }

                    else
                    {
                        double others = Convert.ToDouble(Txtothers.Text);
                        double grandtotal = total + others - less;
                        lblgrandtotal.Text = grandtotal.ToString("F2");
                    }
                }
                else
                {
                    lblgrandtotal.Text = Convert.ToDouble(lbltotalamount.Text).ToString("F2");
                }
            }
            catch
            {

            }
        }

        private void Txtothers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtless.Text))
                {

                    double total = Convert.ToDouble(lbltotalamount.Text);
                    if (string.IsNullOrEmpty(txtless.Text))
                    {
                        txtless.Text = "0";
                    }
                    if (string.IsNullOrEmpty(Txtothers.Text))
                    {
                        Txtothers.Text = "0";
                    }

                    double less = Convert.ToDouble(txtless.Text);
                    double others = Convert.ToDouble(Txtothers.Text);
                    double grandtotal = total + others - less;
                    lblgrandtotal.Text = grandtotal.ToString("F2");
                }
                else
                {
                    lblgrandtotal.Text = Convert.ToDouble(lbltotalamount.Text).ToString("F2");
                }
            }
            catch
            {

            }
        }

        private void txtless_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar))
                              && (e.KeyChar != '.'))
                e.Handled = true;


            //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            //{
            //    e.Handled = true;
            //}

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            // only allow minus sign at the beginning
            if (e.KeyChar == '-' && (sender as TextBox).Text.Length > 0)
            {
                e.Handled = true;
            }
        }


        public void GetReport(string QuotationId, int shop)
        {
            try
            {
                if (!string.IsNullOrEmpty(QuotationId))
                {
                    DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                             shop = Convert.ToInt32(Program.ShopName);

                            if (shop == 1)
                            { 
                                if(hdnComis==1)
                                {
                                    company = "R.R.LIGHTS";
                                }
                                else
                                {
                                    company = "R.R. ELECTRICAL AGENCIES";
                                }



                                
                            }

                            if (shop == 2)
                            {
                                company = "R.R. PIPES";

                            }
                            cmd.Parameters.AddWithValue("@company", company);
                            cmd.CommandType = CommandType.StoredProcedure;
                            //cmd.CommandText = "GetQuotationsalesreport_print";
                           

                            if (shop == 1)
                            {

                                cmd.CommandText = "GetQuotationsalesreport_print";
                            }

                            if (shop == 2)
                            {
                                cmd.CommandText = "GetQuotationsalesreport_printPipes";

                            }
                           // cmd.CommandText = "GetQuotationsalesreport_printPipes";
                            cmd.Connection = con;
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(ds);

                            SalesReports Obj = new SalesReports();
                            Obj.dsMain = ds;
                            Obj.pagenumber = 1;
                            Obj.status = true;
                            if (Obj.GenerateQuoation())
                            {
                                page = Obj.pagenumber;

                                lblqid.Text = txtorder.Text;
                                panel4.Visible = true;
                                rdrange.Checked = false;
                                rdall.Checked = true;
                                rdrange.Text = "Range(" + page + ")";
                                fileName = Obj.fileName;

                                textBox2.Text = string.Empty;
                                textBox2.Text = string.Empty;

                                // print(QuotationId,page);

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


        public void print()
        {

            if (lblqid.Text.IndexOf("GST") >= 0)
            {
                Program.PrintInvoiceNumber = lblqid.Text;
                frmInvoice frmInvoice = new frmInvoice();
                frmInvoice.Show();

            }
            else
            {
                if (rdall.Checked)
                {

                    PrintBuffer();
                    frmPrintPreview objfrmpreview = new frmPrintPreview();
                    objfrmpreview.fileName = fileName;
                    objfrmpreview.Show();

                }

                else if (rdrange.Checked)
                {

                    using (SqlConnection con = new SqlConnection(Program.connection))
                    {
                        DataSet ds = new DataSet();
                        con.Open();
                        SqlCommand cmd = new SqlCommand();
                        int shop = Convert.ToInt32(lblShopList.Text);
                        cmd.Parameters.AddWithValue("@id", lblqid.Text);
                        if (shop == 1)
                        {
                            if (hdnComis == 1)
                            {
                                company = "R.R.LIGHTS";
                            }
                            else
                            {
                                company = "R.R. ELECTRICAL AGENCIES";
                            }
                        }

                        if (shop == 2)
                        {
                            company = "R.R. PIPES";

                        }


                        cmd.Parameters.AddWithValue("@company", company);
                        cmd.CommandType = CommandType.StoredProcedure;
                        //int shop = Convert.ToInt32(lblShopList.Text);
                        if (shop == 1)
                        {
                            cmd.CommandText = "GetQuotationsalesreport_print";

                        }

                        if (shop == 2)
                        {
                            cmd.CommandText = "GetQuotationsalesreport_printPipes";

                        }
                        // cmd.CommandText = "GetQuotationsalesreport_print";
                        cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.Fill(ds);

                        SlaesReportDALPage Obj = new SlaesReportDALPage();
                        Obj.dsMain = ds;
                        Obj.pagenumber = 1;

                        Obj.status = false;
                        Obj.fpage = Convert.ToInt32(txtfrmpage.Text);
                        Obj.lpage = Convert.ToInt32(txttppage.Text);

                        if (Obj.GenerateQuoation())
                        {
                            PrintBuffer();
                            frmPrintPreview objfrmpreview = new frmPrintPreview();
                            objfrmpreview.fileName = Obj.fileName;
                            objfrmpreview.Show();
                        }

                    }
                }
            }
        }


        public void PrintBuffer()
        {

            Process cmd = new Process();
            cmd.StartInfo.FileName = "d:\\bill.bat";

            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            cmd.Start();


        }
        public void GetrrlightsReport(string QuotationId)
        {
            try
            {
                if (!string.IsNullOrEmpty(QuotationId))
                {
                    DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                            int shop = Convert.ToInt32(Program.ShopName);

                            if (shop == 1)
                            {

                                if (hdnComis == 1)
                                {
                                    company = "R.R.LIGHTS";
                                }
                                else
                                {
                                    company = "R.R. ELECTRICAL AGENCIES";
                                }
                            }

                            if (shop == 2)
                            {
                                company = "R.R. PIPES";

                            }

                            cmd.Parameters.AddWithValue("@company", company);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "GetRRLightsQuotationsalesreport_print";
                            cmd.Connection = con;
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(ds);

                            DalRRlightsSalesBill Obj = new DalRRlightsSalesBill();
                            Obj.dsMain = ds;
                            if (Obj.GenerateQuoation())
                            {
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
                MessageBox.Show(e.Message);
            }
        }

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

        private void button1_Click(object sender, EventArgs e)
        {
            if( txtorder.Text.IndexOf("GST")>=0)
            {
                Program.PrintInvoiceNumber = txtorder.Text;
                frmInvoice frminvoic = new frmInvoice();
                frminvoic.Show();
            }
            else
            {
                string configvalue2 = ConfigurationManager.AppSettings["PrePrinted"];
                if (configvalue2 == "Yes")
                {
                    GetrrlightsReport(txtorder.Text);
                }
                else
                {
                    int shop = Convert.ToInt32(lblShopList.Text);
                    string bb = txtorder.Text;
                    GetReport(bb, shop);
                }
            }
           

            //if()
            //{
            //GetReport(txtorder.Text);
            //}

        }

        private void dgvSearch_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyData == Keys.Down)
                {

                    if (dgvSearch.CurrentCell.RowIndex >= 0)
                    {
                        string bb = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex + 1].Cells[0].Value);
                        int shop = Convert.ToInt32(lblShopList.Text);
                        getEstimation(bb,shop);
                    }
                }
                else if (e.KeyData == Keys.Up)
                {
                    if (dgvSearch.CurrentCell.RowIndex >= 0)
                    {
                        string cc = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex - 1].Cells[0].Value);
                        int shop = Convert.ToInt32(lblShopList.Text);
                        getEstimation(cc,shop);
                    }
                }
            }
            catch
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clearprint();
        }


        public void clearprint()
        {
            panel4.Visible = false;
            rdall.Checked = false;
            rdrange.Checked = false;
            //lblfrom.Visible = false;
            //lblto.Visible = false;


            txttppage.Text = string.Empty;
            txtfrmpage.Text = string.Empty;
            //textBox1.Visible = false;
            //textBox2.Visible = false;

        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (rdrange.Checked)
            {
                if (string.IsNullOrEmpty(txtfrmpage.Text))
                {
                    MessageBox.Show("Please Enter From Page");
                }
                else if (string.IsNullOrEmpty(txttppage.Text))
                {
                    MessageBox.Show("Please Enter To Page");
                }

                else if (Convert.ToInt32(txtfrmpage.Text) > Convert.ToInt32(txttppage.Text))
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

        public void chages()
        {
            Program.ShopName = Convert.ToString(cmbcompanychange.SelectedIndex);
            
            recal();
           

        }
        private void button4_Click(object sender, EventArgs e)
        {
            
            if (cmbcompanychange.SelectedIndex==0)
            {
                MessageBox.Show("Please Select Company name");
            }
            else
            {
                chages();
                chages();
                int shop = Convert.ToInt32(lblShopList.Text);
                Program.Company = cmbcompanychange.Text;
                getsaleamount(shop);
                clear();
                MessageBox.Show("Shop Changed Successfully");
            }
            
               
        }

        private void dgvOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Txtitem_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label36.Visible = false;          
            label35.Visible = false;
            lblHSNCode.Visible = true;
            label34.Visible = true;
            textBox6.Visible = false;
            label36.Text = "";
            Txtitem.Visible = true;
            textBox6.Text = "";
            Txtitem.Focus();
            ItemName.Visible = false;
            lblHSNCode.Text = "0";
            lblprice.Text = "0";

            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
         
            label36.Visible = true;
            label35.Visible = true;
            lblHSNCode.Visible = false;
            label34.Visible = false;

           // label36.Visible = true;
            label36.Text = "";
            Txtitem.Visible = false;
            Txtitem.Text = "";
           // label35.Visible = false;
            textBox6.Visible = true;
           // lblHSNCode.Visible = false;
            ItemName.Visible = true;
            textBox6.Focus();
            ItemName.Text = "";
            lblprice.Text = "0";

           
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {




            if (radioButton1.Checked)
            {
                textBox6.Visible = false;
                textBox6.Text = "";
                lblprice.Text = "0";
                Txtitem.Visible = true;
                Txtitem.Focus();
                lblHSNCode.Visible = true;
                ItemName.Visible = false;
                itemdetails();
            }
            else if (radioButton2.Checked)
            {
                textBox6.Visible = true;
                Txtitem.Text = "";
                Txtitem.Visible = false;
                lblHSNCode.Visible = false;
                ItemName.Visible = true;
                textBox6.Focus();
                itemdetailsHSN();
               
            }

            //if (radioButton1.Checked)
            //{
            //{
            //    textBox6.Visible = false;
            //    Txtitem.Visible = true;
            //    itemdetails();
            //}
            //else if (radioButton2.Checked)
            //{
            //    textBox6.Visible = true;
            //    Txtitem.Visible = false;
            //    itemdetailsHSN();
            //}
            
        }
        public void itemdetailsHSN()
        {

            try
            {


                string s1 = textBox6.Text.Trim();
                string[] s = s1.Split('-');
                HSNPrice.Text = "";

                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s[0].ToString();
                string name1 = s[1].ToString();

                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                //DataTable st = objQuotationbal.itemdetails1(name, s2);
                DataTable st = objQuotationbal.itemdetailssalesHSN(name, s2, name1);

                if (st.Rows.Count > 0)
                {
                    lblitem.Text = name;
                    //pnitemdetails.Visible = true;
                    lblitemcode.Text = Convert.ToString(st.Rows[0]["UOM"]);
                    if (lblitemcode.Text == "")
                    {
                        lblitemcode.Text = "0";
                    }


                    HSNPrice.Text = Convert.ToString(st.Rows[0]["ProductId"]);
                    if (HSNPrice.Text == "")
                    {
                        HSNPrice.Text = "0";
                    }
                    lblHSNCode.Text = Convert.ToString(st.Rows[0]["HSN"]);
                    if (lblHSNCode.Text == "")
                    {
                        lblHSNCode.Text = "0";
                    }
                    lblprice.Text = Convert.ToString(st.Rows[0]["Price"]);
                    if (lblprice.Text == "")
                    {
                        lblprice.Text = "0";
                    }

                    lblVat.Text = Convert.ToString(st.Rows[0]["GST"]);
                    if (lblVat.Text == "")
                    {
                        lblVat.Text = "0";
                    }

                    lbelhsncode.Text = Convert.ToString(st.Rows[0]["HSN1"]);
                    if (lbelhsncode.Text == "")
                    {
                        lbelhsncode.Text = "0";
                    }


                    label36.Text = Convert.ToString(st.Rows[0]["ItemName"]);
                    if (label36.Text == "")
                    {
                        label36.Text = "";
                    }
                    

                }
                else
                {
                  
                    lblprice.Text = "0";
                    label36.Text = "";
                    //lblprice.Text = "0";
                 
                }

            }
            catch (Exception e)
            {
                lblprice.Text = "0";
                label36.Text = "";
            }

        }

        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(textBox6.Text))
                    {
                        if (Convert.ToInt32(HSNPrice.Text) != 0)
                        {
                            dgvOrder.Columns[6].DefaultCellStyle.Format = "F2";
                            int rowindex = Convert.ToInt32(lblrowindex.Text);
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7];
                            dgvOrder.Rows[rowindex].Cells[2].Value = lbelhsncode.Text;
                            dgvOrder.Rows[rowindex].Cells[4].Value = HSNPrice.Text;
                            dgvOrder.Rows[rowindex].Cells[1].Value = label36.Text.ToUpper(); 
                            dgvOrder.Rows[rowindex].Cells[3].Value = lblitemcode.Text;
                            double val = Convert.ToDouble(lblprice.Text);
                            dgvOrder.Rows[rowindex].Cells[5].Value = val;
                            dgvOrder.Rows[rowindex].Cells[6].Value = lblVat.Text;
                            dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                            pnsearch.Visible = false;
                            HSNPrice.Text = string.Empty;
                            textBox6.Text = string.Empty;
                            label36.Text = "";
                            lblitemcode.Text = "0";
                            lblHSNCode.Text = "0";
                            lbldisplay.Text = "0";
                            lbldemo.Text = "0";
                            lblservice.Text = "0";
                            lbldamage.Text = "0";
                            lblprice.Text = "0";
                            lbelhsncode.Text = "0";
                            lblVat.Text = "0";
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7];
                        }
                        else
                        {
                            MessageBox.Show("Please Enter Correct Product Name");
                        }
                    }
                    else
                    {

                        this.ActiveControl = btnSave;
                        pnsearch.Visible = false;

                        //MessageBox.Show("Please Enter Product Name");
                        //Txtitem.Focus();
                    }
                }
            }

            catch (Exception ex)
            {

            }
        }
    }
}
