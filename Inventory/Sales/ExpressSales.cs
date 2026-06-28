
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
using System.Reflection;
using System.Text.RegularExpressions;
using Inventory.Report;
using EstimationReport;
using Receipt;
using System.Collections;



namespace Inventory.Sales
{
    public partial class ExpressSales : Form
    {
        string role1 = string.Empty;
        string srole = string.Empty;
        string ReceiptId = string.Empty;
        TextBox tb, tbrate;
        bool savevads = false;
        int cusbool = 0;
        PurchaseReceiptBAL ObjPurchaseReceiptBAL = new PurchaseReceiptBAL();

        QuotationBal objQuotationbal = new QuotationBal();
        DataTable dtreceivedbalance, dtpaidbalance;
        ComboBox cmblocation;
        string cas = string.Empty;

        int ProdSelRowvalue = 0;
        DataTable dtitems;
        bool res = false;

        TextBox  tbamount, tbbaalanceanount, tborderquantoty, tbdeliveredqty;
        public bool edit = false;
        string clickstatus = string.Empty;
        string selectedtab = string.Empty;
        public ExpressSales()
        {
            InitializeComponent();
            Cmbless.SelectedIndex = 0;

            srole = Program.Userrole;
            if (srole != "Admin")
            {
                role1 = "Emp";
            }
            else
            {
                role1 = "Admin";
            }

            this.WindowState = FormWindowState.Maximized;
            
            LoadPortsNew();
            LoadPortsFloorCheckOut();
            LoadPortsCreditApproval();
            LoadPortsPayment();
            bindAccountno();
            LoadPortsChecking();
            LoadPortsDelivery();
            LoadPortsReturnApproval();
            LoadPortsreturnChecking();
            bindLocation();
            cmbpaymode.SelectedIndex = 0;
            rbCash.Checked = false;
            cmbloaction.SelectedIndex = 0;
            Globeimage();
            groupBox3.Enabled = false;
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            DataTable dtitems = Program.dtitems;
            if (dtitems == null)
            {
                itemdetails("");
            }
            ddlpaymenttype.SelectedIndex = 0;
            ddlpaymode.SelectedIndex = 0;

            cmbbank.SelectedIndex = 0;


            paymentdenobind();
            paymentDenotoCustomerbind();

            ddlpaymode.SelectedIndex = 0;


            // SearchPurchaseOrder();


            lblperare.Text = Program.UserName;
            bindAssist();
            bindreference();
            bindcustomer();

            //Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //Txtitem.AutoCompleteCustomSource = AutoCompleteLoad();
            //Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;



            txtsearch1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtsearch1.AutoCompleteCustomSource = Autocusomer();
            txtsearch1.AutoCompleteSource = AutoCompleteSource.CustomSource;


            txtsearch2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtsearch2.AutoCompleteCustomSource = Autocusomer();
            txtsearch2.AutoCompleteSource = AutoCompleteSource.CustomSource;


            txtsearch3.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtsearch3.AutoCompleteCustomSource = Autocusomer();
            txtsearch3.AutoCompleteSource = AutoCompleteSource.CustomSource;







            //if (Program.Userrole == "Estimation")
            //{
            //    MainTabSalesBill.TabPages.Remove(TabCreditApproval);
            //    MainTabSalesBill.TabPages.Remove(TabPayment);

            //    MainTabSalesBill.TabPages.Remove(TabFloorCheckOut);
            //    MainTabSalesBill.TabPages.Remove(TabChecking);
            //    MainTabSalesBill.TabPages.Remove(TabDelivery);
            //    btnPrint.Enabled = true;
            //    btnNew.Enabled = true;
            //    btnsave.Enabled = true;
            //    btnSavePending.Enabled = true;
            //    btnClear.Enabled = true;

            //}
            //else if (Program.Userrole == "Checkout")
            //{
            //    MainTabSalesBill.TabPages.Remove(TabCreditApproval);
            //    MainTabSalesBill.TabPages.Remove(TabPayment);

            //    MainTabSalesBill.TabPages.Remove(TabNew);
            //    MainTabSalesBill.TabPages.Remove(TabChecking);
            //    MainTabSalesBill.TabPages.Remove(TabDelivery);
            //    btnsave.Enabled = true;
            //    btnClear.Enabled = true;
            //}

            //else if (Program.Userrole == "credit")
            //{
            //    MainTabSalesBill.TabPages.Remove(TabFloorCheckOut);
            //    MainTabSalesBill.TabPages.Remove(TabNew);
            //    MainTabSalesBill.TabPages.Remove(TabChecking);
            //    MainTabSalesBill.TabPages.Remove(TabDelivery);

            //    MainTabSalesBill.TabPages.Remove(TabPayment);
            //    btnPrint.Enabled = false;
            //    btnNew.Enabled = false;
            //    btnsave.Enabled = false;
            //    btnSavePending.Enabled = false;
            //    btnClear.Enabled = true;
            //}
            //else if (Program.Userrole == "Payment")
            //{
            //    MainTabSalesBill.TabPages.Remove(TabFloorCheckOut);
            //    MainTabSalesBill.TabPages.Remove(TabNew);
            //    MainTabSalesBill.TabPages.Remove(TabChecking);
            //    MainTabSalesBill.TabPages.Remove(TabDelivery);

            //    MainTabSalesBill.TabPages.Remove(TabCreditApproval);
            //    btnPrint.Enabled = false;
            //    btnNew.Enabled = false;
            //    btnsave.Enabled = false;
            //    btnSavePending.Enabled = false;
            //    btnClear.Enabled = true;
            //}
            //else if (Program.Userrole == "Check")
            //{
            //    MainTabSalesBill.TabPages.Remove(TabFloorCheckOut);
            //    MainTabSalesBill.TabPages.Remove(TabNew);
            //    MainTabSalesBill.TabPages.Remove(TabPayment);
            //    MainTabSalesBill.TabPages.Remove(TabDelivery);

            //    MainTabSalesBill.TabPages.Remove(TabCreditApproval);
            //    btnPrint.Enabled = false;
            //    btnNew.Enabled = false;
            //    btnsave.Enabled = true;
            //    btnSavePending.Enabled = false;
            //    btnClear.Enabled = true;
            //}
            //else if (Program.Userrole == "Delivered")
            //{
            //    MainTabSalesBill.TabPages.Remove(TabFloorCheckOut);
            //    MainTabSalesBill.TabPages.Remove(TabNew);
            //    MainTabSalesBill.TabPages.Remove(TabPayment);
            //    MainTabSalesBill.TabPages.Remove(TabChecking);
            //    btnPrint.Enabled = false;
            //    btnNew.Enabled = false;
            //    btnsave.Enabled = true;
            //    btnSavePending.Enabled = false;
            //    btnClear.Enabled = true;
            //}



            selectedtab = MainTabSalesBill.SelectedTab.Name;

            if (selectedtab == "TabNew")
            {
                //search("Quotationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                DataTable dt = bindEstimation();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Quotationid";
                cmbstatus1.ValueMember = "Quotationid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Quotationid";
                cmbstatus2.ValueMember = "Quotationid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Quotationid";
                cmbstatus3.ValueMember = "Quotationid";

                btnSavePending.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = true;
            }
            else if (selectedtab == "TabFloorCheckOut")
            {
                searchcheckout("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);

                DataTable dt = bindcheckout();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Estimationid";
                cmbstatus1.ValueMember = "Estimationid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Estimationid";
                cmbstatus2.ValueMember = "Estimationid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Estimationid";
                cmbstatus3.ValueMember = "Estimationid";


                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
            }

            else if (selectedtab == "TabCreditApproval")
            {

                searchcreditapproved("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);

                DataTable dt = bindcheckout();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Estimationid";
                cmbstatus1.ValueMember = "Estimationid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Estimationid";
                cmbstatus2.ValueMember = "Estimationid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Estimationid";
                cmbstatus3.ValueMember = "Estimationid";


                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
            }
            else if (selectedtab == "TabPayment")
            {

                searchpay("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);

                DataTable dt = bindcheckout();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Estimationid";
                cmbstatus1.ValueMember = "Estimationid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Estimationid";
                cmbstatus2.ValueMember = "Estimationid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Estimationid";
                cmbstatus3.ValueMember = "Estimationid";


                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
            }

            else if (selectedtab == "TabChecking")
            {

                searchcheck("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);

                DataTable dt = bindcheckout();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Estimationid";
                cmbstatus1.ValueMember = "Estimationid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Estimationid";
                cmbstatus2.ValueMember = "Estimationid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Estimationid";
                cmbstatus3.ValueMember = "Estimationid";


                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
            }
            else if (selectedtab == "TabDelivery")
            {

                searchdelivery("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);

                DataTable dt = bindcheckout();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Estimationid";
                cmbstatus1.ValueMember = "Estimationid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Estimationid";
                cmbstatus2.ValueMember = "Estimationid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Estimationid";
                cmbstatus3.ValueMember = "Estimationid";


                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
            }


            else if (selectedtab == "TapReturnCheckin")
            {

                searchreturncheck("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);

                DataTable dt = bindcheckout();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Estimationid";
                cmbstatus1.ValueMember = "Estimationid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Estimationid";
                cmbstatus2.ValueMember = "Estimationid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Estimationid";
                cmbstatus3.ValueMember = "Estimationid";


                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
            }

            else if (selectedtab == "TapReturnApproval")
            {

                searchReturnapproved("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);

                DataTable dt = bindcheckout();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Estimationid";
                cmbstatus1.ValueMember = "Estimationid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Estimationid";
                cmbstatus2.ValueMember = "Estimationid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Estimationid";
                cmbstatus3.ValueMember = "Estimationid";


                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
            }

            if (Program.UserName != "Admin")
            {


                DataTable dt1 = Program.Dtmenu;
                bool contains = dt1.AsEnumerable()
                    .Any(row => "ExpressSalesCheckout" == row.Field<String>("Data"));
                if (contains == false)
                {
                    MainTabSalesBill.TabPages.Remove(TabFloorCheckOut);
                    btnPrint.Enabled = true;
                    btnNew.Enabled = true;
                    btnsave.Enabled = true;
                    btnSavePending.Enabled = true;
                    btnClear.Enabled = true;

                }
                bool contains5 = dt1.AsEnumerable()
           .Any(row => "ExpressSalesReturnApproval" == row.Field<String>("Data"));
                if (contains5 == false)
                {
                    MainTabSalesBill.TabPages.Remove(TapReturnApproval);
                    btnPrint.Enabled = false;
                    btnNew.Enabled = false;
                    btnsave.Enabled = false;
                    btnSavePending.Enabled = false;
                    btnClear.Enabled = true;
                }


                bool contains6 = dt1.AsEnumerable()
                .Any(row => "ExpressSalesReturnChecking" == row.Field<String>("Data"));
                if (contains6 == false)
                {
                    MainTabSalesBill.TabPages.Remove(TapReturnCheckin);
                    btnPrint.Enabled = false;
                    btnNew.Enabled = false;
                    btnsave.Enabled = true;
                    btnSavePending.Enabled = false;
                    btnClear.Enabled = true;
                }




                bool contains1 = dt1.AsEnumerable()
                   .Any(row => "ExpressSalesCreditapproval" == row.Field<String>("Data"));
                if (contains1 == false)
                {
                    MainTabSalesBill.TabPages.Remove(TabCreditApproval);
                    btnPrint.Enabled = false;
                    btnNew.Enabled = false;
                    btnsave.Enabled = false;
                    btnSavePending.Enabled = false;
                    btnClear.Enabled = true;
                }

                bool contains2 = dt1.AsEnumerable()
                   .Any(row => "ExpressSalesPayment" == row.Field<String>("Data"));
                if (contains2 == false)
                {
                    MainTabSalesBill.TabPages.Remove(TabPayment);
                    btnPrint.Enabled = false;
                    btnNew.Enabled = false;
                    btnsave.Enabled = false;
                    btnSavePending.Enabled = false;
                    btnClear.Enabled = true;
                }

                bool contains3 = dt1.AsEnumerable()
                 .Any(row => "ExpressSalesChecking" == row.Field<String>("Data"));
                if (contains3 == false)
                {
                    MainTabSalesBill.TabPages.Remove(TabChecking);
                    btnPrint.Enabled = false;
                    btnNew.Enabled = false;
                    btnsave.Enabled = true;
                    btnSavePending.Enabled = false;
                    btnClear.Enabled = true;
                }

                bool contains4 = dt1.AsEnumerable()
                .Any(row => "ExpressSalesDelivery" == row.Field<String>("Data"));
                if (contains4 == false)
                {
                    MainTabSalesBill.TabPages.Remove(TabDelivery);
                    btnPrint.Enabled = false;
                    btnNew.Enabled = false;
                    btnsave.Enabled = true;
                    btnSavePending.Enabled = false;
                    btnClear.Enabled = true;
                }



            }


        }

        public bool getreturns()
        {
            bool returns = false;
            for (int i = 0; i < dgvNew.Rows.Count; i++)
            {
                double v = Convert.ToDouble(dgvNew.Rows[i].Cells["Quantity"].Value);
                if (v > 0)
                {
                    returns = true;
                    break;
                }
                else
                {
                    returns = false;
                }
            }
            return returns;
        }

        public void searchreturncheck(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchreturncheck(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void LoadPortsReturnApproval()
        {

            dgvReturnApproval.Rows.Clear();
            dgvReturnApproval.ColumnCount = 7;
            //dgvOrder.RowCount = 16;

            dgvReturnApproval.Columns[0].Name = "S.NO";
            dgvReturnApproval.Columns[1].Name = "Items";
            dgvReturnApproval.Columns[2].Name = "UOM";
            dgvReturnApproval.Columns[5].Name = "Quantity";
            dgvReturnApproval.Columns[3].Name = "productid";
            dgvReturnApproval.Columns[4].Name = "Rate";
            dgvReturnApproval.Columns[6].Name = "Amount";

            dgvReturnApproval.Columns[3].Visible = false;

            this.dgvReturnApproval.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvReturnApproval.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvReturnApproval.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvReturnApproval.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvReturnApproval.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvReturnApproval.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvReturnApproval.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvReturnApproval.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvReturnApproval.Columns[0].ReadOnly = true;
            this.dgvReturnApproval.Columns[1].ReadOnly = true;
            this.dgvReturnApproval.Columns[2].ReadOnly = true;
            this.dgvReturnApproval.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvReturnApproval.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvReturnApproval.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            this.dgvReturnApproval.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvReturnApproval.Columns[4].ReadOnly = true;

            this.dgvReturnApproval.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            dgvReturnApproval.Columns[4].DefaultCellStyle.Format = "N2";
            dgvReturnApproval.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvReturnApproval.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvReturnApproval.Columns[6].ReadOnly = true;

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvReturnApproval.Columns[0].Width = 12;
                this.dgvReturnApproval.Columns[1].Width = 100;
                this.dgvReturnApproval.Columns[2].Width = 15;
                this.dgvReturnApproval.Columns[4].Width = 15;
                this.dgvReturnApproval.Columns[5].Width = 20;
                this.dgvReturnApproval.Columns[6].Width = 100;

            }
            else
            {
                this.dgvReturnApproval.Columns[0].Width = 12;
                this.dgvReturnApproval.Columns[1].Width = 100;
                this.dgvReturnApproval.Columns[2].Width = 15;
                this.dgvReturnApproval.Columns[4].Width = 15;
                this.dgvReturnApproval.Columns[5].Width = 15;
                this.dgvReturnApproval.Columns[6].Width = 100;

            }


            dgvReturnApproval.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvReturnApproval.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }


        }





        #region Search
        private void pbxCollapse_Click(object sender, EventArgs e)
        {
            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;
                vLabel4.Enabled = true;
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
                //this.dgvSearch.Columns[1].Visible = false;
                //this.dgvSearch.Columns[2].Visible = false;

            }
        }

        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("Customer");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderNo.DataSource = bs.DataSource;
            cbxSearchOrderNo.SelectedIndex = 0;
            cmbstatus1.Visible = false;

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

            //    pictureBox1.Image = Image.FromStream(ms);
            //    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            //    pictureBox3.Image = Image.FromStream(ms);
            //    pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;

            //    pictureBox4.Image = Image.FromStream(ms);
            //    pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;

            //    pictureBox5.Image = Image.FromStream(ms);
            //    pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;

            //    pictureBox6.Image = Image.FromStream(ms);
            //    pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
            //}
        }



        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("Customer");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderDate.DataSource = bs.DataSource;
            cbxSearchOrderDate.SelectedIndex = 1;
            cmbstatus2.Visible = false;
        }

        private void SearchCreteria3()
        {
            List<string> search = new List<string>();
            search.Add("Customer");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxVendor.DataSource = bs.DataSource;
            cbxVendor.SelectedIndex = 2;
            txtsearch3.Visible = false;
        }

        private void cbxSearchOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchOrderNo.SelectedIndex == 2)
            {
                cmbstatus1.Visible = true;
                txtsearch1.Visible = false;
                ListSearchDate1.Visible = false;

            }
            else if (cbxSearchOrderNo.SelectedIndex == 1)
            {
                cmbstatus1.Visible = false;
                txtsearch1.Visible = true;
                txtsearch1.Text = "Today";
                ListSearchDate1.Visible = true;
            }
            else
            {
                cmbstatus1.Visible = false;
                txtsearch1.Visible = true;
                txtsearch1.Text = string.Empty;
                ListSearchDate1.Visible = false;
            }
            pnlCalender.Visible = false;
        }
        private void cbxSearchOrderDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchOrderDate.SelectedIndex == 2)
            {
                cmbstatus2.Visible = true;
                txtsearch2.Visible = false;
                ListSearchDate2.Visible = false;

            }
            else if (cbxSearchOrderDate.SelectedIndex == 1)
            {
                cmbstatus2.Visible = false;
                txtsearch2.Visible = true;
                txtsearch2.Text = "Today";
                ListSearchDate2.Visible = true;
            }
            else
            {
                cmbstatus2.Visible = false;
                txtsearch2.Visible = true;
                txtsearch2.Text = string.Empty;
                ListSearchDate2.Visible = false;
            }

            pnlCalender.Visible = false;
        }
        private void cbxVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxVendor.SelectedIndex == 2)
            {
                cmbstatus3.Visible = true;
                txtsearch3.Visible = false;
                ListSearchDate3.Visible = false;
            }
            else if (cbxVendor.SelectedIndex == 1)
            {
                cmbstatus3.Visible = false;
                txtsearch3.Visible = true;
                txtsearch3.Text = "Today";
                ListSearchDate3.Visible = true;
            }
            else
            {
                cmbstatus3.Visible = false;
                txtsearch3.Visible = true;
                txtsearch3.Text = string.Empty;
                ListSearchDate3.Visible = false;
            }

            pnlCalender.Visible = false;
        }
        private void txtsearch1_Click(object sender, EventArgs e)
        {
            string selecteditem = cbxSearchOrderNo.Text.ToString();
            if (selecteditem == "Date")
            {
                clickstatus = "search1";
                pnlCalender.BringToFront();
                pnlCalender.Visible = true;
                pnlCalender.Location = new Point(133, 54);
            }
            else
            {
                pnlCalender.Visible = false;
            }
        }
        private void txtsearch2_Click(object sender, EventArgs e)
        {
            if (cbxSearchOrderDate.Text.ToString() == "Date")
            {
                clickstatus = "search2";
                pnlCalender.BringToFront();
                pnlCalender.Visible = true;
                pnlCalender.Location = new Point(133, 79);
            }
            else
            {
                pnlCalender.Visible = false;
            }
        }

        private void txtsearch3_Click(object sender, EventArgs e)
        {
            if (cbxVendor.Text.ToString() == "Date")
            {
                clickstatus = "search3";
                pnlCalender.BringToFront();
                pnlCalender.Visible = true;
                pnlCalender.Location = new Point(133, 103);
            }
            else
            {
                pnlCalender.Visible = false;

            }
        }
        //private void lblAll_Click(object sender, EventArgs e)
        //{
        //    if (clickstatus == "search1")
        //    {
        //        txtsearch1.Text = lblAll.Text.Trim();
        //    }
        //    else if (clickstatus == "search2")
        //    {
        //        txtsearch2.Text = lblAll.Text.Trim();
        //    }
        //    else if (clickstatus == "search3")
        //    {
        //        txtsearch3.Text = lblAll.Text.Trim();
        //    }

        //    pnlCalender.Visible = false;
        //}
        private void lblToday_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblToday.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblToday.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblToday.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblThisWeek_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblThisWeek.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblThisWeek.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblThisWeek.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblThisMonth_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblThisMonth.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblThisMonth.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblThisMonth.Text.Trim();
            }
            pnlCalender.Visible = false;
        }

        private void lblThisYear_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblThisYear.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblThisYear.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblThisYear.Text.Trim();
            }
            pnlCalender.Visible = false;
        }
        private void lblYesterday_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblYesterday.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblYesterday.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblYesterday.Text.Trim();
            }

            pnlCalender.Visible = false;
        }

        private void lblLastWeek_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblLastWeek.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblLastWeek.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblLastWeek.Text.Trim();
            }

            pnlCalender.Visible = false;
        }

        private void lblLastMonth_Click(object sender, EventArgs e)
        {

            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblLastMonth.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblLastMonth.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblLastMonth.Text.Trim();
            }
            pnlCalender.Visible = false;
        }

        private void lblLastYear_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblLastYear.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblLastYear.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblLastYear.Text.Trim();
            }
            pnlCalender.Visible = false;
        }

        private void SearchFrmDate_ValueChanged(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = SearchFrmDate.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = SearchFrmDate.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = SearchFrmDate.Text.Trim();
            }
            pnlCalender.Visible = false;
        }

        private void ListSearchDate1_Click(object sender, EventArgs e)
        {
            clickstatus = "search1";
            Calender();
            pnlCalender.Location = new Point(133, 54);
        }

        private void ListSearchDate2_Click(object sender, EventArgs e)
        {
            clickstatus = "search2";
            Calender();
            pnlCalender.Location = new Point(133, 79);
        }

        private void ListSearchDate3_Click(object sender, EventArgs e)
        {
            clickstatus = "search3";
            Calender();
            pnlCalender.Location = new Point(133, 103);
        }
        private void Calender()
        {
            if (pnlCalender.Visible)
            {
                pnlCalender.Visible = false;
            }
            else
            {
                pnlCalender.BringToFront();
                pnlCalender.Visible = true;
            }
        }

        #endregion

        #region SalesBillTabPage
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

            // this.dgvNew.Columns[5].ReadOnly = true;

            //dgvNew.Columns[4].DefaultCellStyle.Format = "N2";
            //dgvNew.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvNew.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvNew.Columns[6].ReadOnly = true;


            this.dgvNew.Columns["S.NO"].ReadOnly = true;
            this.dgvNew.Columns["Items"].ReadOnly = true;
            this.dgvNew.Columns["UOM"].ReadOnly = true;
            this.dgvNew.Columns["Rate"].ReadOnly = true;
            this.dgvNew.Columns["Amount"].ReadOnly = true;

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvNew.Columns[0].Width = 12;
                this.dgvNew.Columns[1].Width = 100;
                this.dgvNew.Columns[2].Width = 15;
         
                this.dgvNew.Columns[4].Width = 20;
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

            foreach (DataGridViewColumn c in dgvNew.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvNew.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvNew.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvNew.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }
        private void LoadPortsCreditApproval()
        {

            dgvCreditApproval.Rows.Clear();
            dgvCreditApproval.ColumnCount = 7;
            //dgvOrder.RowCount = 16;

            dgvCreditApproval.Columns[0].Name = "S.NO";
            dgvCreditApproval.Columns[1].Name = "Items";
            dgvCreditApproval.Columns[2].Name = "UOM";
            dgvCreditApproval.Columns[5].Name = "Quantity";
            dgvCreditApproval.Columns[3].Name = "productid";
            dgvCreditApproval.Columns[4].Name = "Rate";
            dgvCreditApproval.Columns[6].Name = "Amount";

            dgvCreditApproval.Columns[3].Visible = false;

            this.dgvCreditApproval.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvCreditApproval.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCreditApproval.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCreditApproval.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCreditApproval.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCreditApproval.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCreditApproval.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvCreditApproval.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvCreditApproval.Columns[0].ReadOnly = true;
            this.dgvCreditApproval.Columns[1].ReadOnly = true;
            this.dgvCreditApproval.Columns[2].ReadOnly = true;
            this.dgvCreditApproval.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvCreditApproval.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvCreditApproval.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            this.dgvCreditApproval.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvCreditApproval.Columns[4].ReadOnly = true;

            this.dgvCreditApproval.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            dgvCreditApproval.Columns[4].DefaultCellStyle.Format = "N2";
            dgvCreditApproval.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvCreditApproval.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvCreditApproval.Columns[6].ReadOnly = true;

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvCreditApproval.Columns[0].Width = 12;
                this.dgvCreditApproval.Columns[1].Width = 100;
                this.dgvCreditApproval.Columns[2].Width = 15;
                this.dgvCreditApproval.Columns[4].Width = 15;
                this.dgvCreditApproval.Columns[5].Width = 20;
                this.dgvCreditApproval.Columns[6].Width = 100;

            }
            else
            {
                this.dgvCreditApproval.Columns[0].Width = 12;
                this.dgvCreditApproval.Columns[1].Width = 100;
                this.dgvCreditApproval.Columns[2].Width = 15;
                this.dgvCreditApproval.Columns[4].Width = 15;
                this.dgvCreditApproval.Columns[5].Width = 15;
                this.dgvCreditApproval.Columns[6].Width = 100;

            }


            dgvCreditApproval.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvCreditApproval.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }


        }
        private void LoadPortsPayment()
        {
            dgvPayment.Rows.Clear();
            dgvPayment.ColumnCount = 7;
            //dgvOrder.RowCount = 16;

            dgvPayment.Columns[0].Name = "S.NO";
            dgvPayment.Columns[1].Name = "Items";
            dgvPayment.Columns[2].Name = "UOM";
            dgvPayment.Columns[5].Name = "Quantity";
            dgvPayment.Columns[3].Name = "productid";
            dgvPayment.Columns[4].Name = "Rate";
            dgvPayment.Columns[6].Name = "Amount";

            dgvPayment.Columns[3].Visible = false;

            this.dgvPayment.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvPayment.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvPayment.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvPayment.Columns[0].ReadOnly = true;
            this.dgvPayment.Columns[1].ReadOnly = true;
            this.dgvPayment.Columns[2].ReadOnly = true;
            this.dgvPayment.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayment.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayment.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            this.dgvPayment.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvPayment.Columns[4].ReadOnly = true;
            this.dgvPayment.Columns[5].ReadOnly = true;

            this.dgvPayment.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            dgvPayment.Columns[4].DefaultCellStyle.Format = "N2";
            dgvPayment.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvPayment.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvPayment.Columns[6].ReadOnly = true;


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvPayment.Columns[0].Width = 15;
                this.dgvPayment.Columns[1].Width = 100;
                this.dgvPayment.Columns[2].Width = 15;
                this.dgvPayment.Columns[4].Width = 15;
                this.dgvPayment.Columns[5].Width = 25;
                this.dgvPayment.Columns[6].Width = 100;
            }
            else
            {
                this.dgvPayment.Columns[0].Width = 15;
                this.dgvPayment.Columns[1].Width = 100;
                this.dgvPayment.Columns[2].Width = 15;
                this.dgvPayment.Columns[4].Width = 15;
                this.dgvPayment.Columns[5].Width = 12;
                this.dgvPayment.Columns[6].Width = 100;

            }

            dgvPayment.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvPayment.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }


        }

        public void paymentdenobind()
        {
            cashddl.Rows.Clear();
            cashddl.ColumnCount = 5;
            cashddl.Columns[0].Name = "Denomination";
            cashddl.Columns[1].Name = "Count";
            cashddl.Columns[2].Name = "Amount";
            cashddl.Columns[3].Name = "DenominationID";
            cashddl.Columns[4].Name = "unique";


            this.cashddl.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.cashddl.Columns[0].Width = 150;
            this.cashddl.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.cashddl.Columns[0].ReadOnly = true;
            this.cashddl.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.cashddl.Columns[1].Width = 100;
            this.cashddl.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.cashddl.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.cashddl.Columns[2].Width = 100;
            this.cashddl.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.cashddl.Columns[2].ReadOnly = true;
            this.cashddl.Columns[3].Visible = false;
            this.cashddl.Columns[4].Visible = false;

            DataTable payment = new DataTable();
            payment = paymentDeno();
            int row = payment.Rows.Count;
            cashddl.Rows.Add(row);
            if (row > 0)
            {

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < payment.Columns.Count; j++)
                    {
                        cashddl.Rows[i].Cells[j].Value = payment.Rows[i][j];
                    }

                }



            }



            cashddl.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
            cashddl.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
            cashddl.DefaultCellStyle.BackColor = Color.Gainsboro;
            cashddl.DefaultCellStyle.ForeColor = Color.Black;
            cashddl.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            foreach (DataGridViewColumn column in cashddl.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }


        }
        public DataTable paymentDeno()
        {
            DataTable paymentdo = new DataTable();
            paymentdo.Columns.Add("Denomination", typeof(string));
            paymentdo.Columns.Add("Count", typeof(int));
            paymentdo.Columns.Add("Amount", typeof(decimal));

            //paymentdo.Columns.Add("DenominationID", typeof(int));
            //paymentdo.Columns.Add("unique", typeof(int));
            paymentdo.Rows.Add("1000", 0, 0.00);
            paymentdo.Rows.Add("500", 0, 0.00);
            paymentdo.Rows.Add("100", 0, 0.00);
            paymentdo.Rows.Add("50", 0, 0.00);
            paymentdo.Rows.Add("20", 0, 0.00);
            paymentdo.Rows.Add("10", 0, 0.00);
            paymentdo.Rows.Add("5", 0, 0.00);
            paymentdo.Rows.Add("2", 0, 0.00);
            paymentdo.Rows.Add("1", 0, 0.00);


            //List<SalesBal> paymodedeno = new List<SalesBal>();
            //paymodedeno = salesbal.Getdenamination().ToList();

            //SqlDataAdapter da = new SqlDataAdapter("SELECT  Id,PaymentMethod FROM MASTER_PaymentMethod", ObjConn);
            //DataTable dtp = new DataTable();
            //da.Fill(dtp);

            //for (int i = 0; i < paymodedeno.Count(); i++)
            //{

            //    string paydeno = paymodedeno.ToList()[i].denoType;
            //    int paydenoid = paymodedeno.ToList()[i].denoId;
            //    paymentdo.Rows.Add(paydeno, 0, 0.00, paydenoid, 0);
            //}
            return paymentdo;

        }

        public void paymentDenotoCustomerbind()
        {
            dgvCustomerpaid.Rows.Clear();
            dgvCustomerpaid.ColumnCount = 5;
            dgvCustomerpaid.Columns[0].Name = "Denomination";
            dgvCustomerpaid.Columns[1].Name = "Count";
            dgvCustomerpaid.Columns[2].Name = "Amount";
            dgvCustomerpaid.Columns[3].Name = "DenominationID";
            dgvCustomerpaid.Columns[4].Name = "unique";


            this.dgvCustomerpaid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvCustomerpaid.Columns[0].Width = 150;
            this.dgvCustomerpaid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dgvCustomerpaid.Columns[0].ReadOnly = true;
            this.dgvCustomerpaid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvCustomerpaid.Columns[1].Width = 100;
            this.dgvCustomerpaid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvCustomerpaid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvCustomerpaid.Columns[2].Width = 100;
            this.dgvCustomerpaid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvCustomerpaid.Columns[2].ReadOnly = true;
            this.dgvCustomerpaid.Columns[3].Visible = false;
            this.dgvCustomerpaid.Columns[4].Visible = false;

            DataTable payment = new DataTable();
            payment = paymentDenotoCustomer();//dt
            int row = payment.Rows.Count;
            dgvCustomerpaid.Rows.Add(row);
            if (row > 0)
            {

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < payment.Columns.Count; j++)
                    {
                        dgvCustomerpaid.Rows[i].Cells[j].Value = payment.Rows[i][j];
                    }

                }

                int sum = 0;
                for (int k = 0; k < dgvCustomerpaid.Rows.Count; k++)
                {
                    sum = sum + Convert.ToInt32(dgvCustomerpaid.Rows[k].Cells[2].Value);
                }
                lblpaidbalance.Text = Convert.ToString(sum);


                dgvCustomerpaid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
                dgvCustomerpaid.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
                dgvCustomerpaid.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvCustomerpaid.DefaultCellStyle.ForeColor = Color.Black;
                dgvCustomerpaid.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


                foreach (DataGridViewColumn column in dgvCustomerpaid.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                //this.dgvCustomerpaid.Columns[1].ValueType = typeof(System.Double);
                //this.dgvCustomerpaid.Columns[2].ValueType = typeof(System.Double);


            }
        }
        public DataTable paymentDenotoCustomer()
        {
            DataTable paymentdo = new DataTable();
            paymentdo.Columns.Add("Denomination", typeof(string));
            paymentdo.Columns.Add("Count", typeof(int));
            paymentdo.Columns.Add("Amount", typeof(decimal));

            //paymentdo.Columns.Add("DenominationID", typeof(int));
            //paymentdo.Columns.Add("unique", typeof(int));
            paymentdo.Rows.Add("1000", 0, 0.00);
            paymentdo.Rows.Add("500", 0, 0.00);
            paymentdo.Rows.Add("100", 0, 0.00);
            paymentdo.Rows.Add("50", 0, 0.00);
            paymentdo.Rows.Add("20", 0, 0.00);
            paymentdo.Rows.Add("10", 0, 0.00);
            paymentdo.Rows.Add("5", 0, 0.00);
            paymentdo.Rows.Add("2", 0, 0.00);
            paymentdo.Rows.Add("1", 0, 0.00);

            //List<SalesBal> paymodedeno = new List<SalesBal>();
            //paymodedeno = salesbal.Getdenamination().ToList();

            //SqlDataAdapter da = new SqlDataAdapter("SELECT  Id,PaymentMethod FROM MASTER_PaymentMethod", ObjConn);
            //DataTable dtp = new DataTable();
            //da.Fill(dtp);

            //for (int i = 0; i < paymodedeno.Count(); i++)
            //{

            //    string paydeno = paymodedeno.ToList()[i].denoType;
            //    int paydenoid = paymodedeno.ToList()[i].denoId;
            //    paymentdo.Rows.Add(paydeno, 0, 0.00, paydenoid, 0);
            //}
            return paymentdo;

        }


        private void LoadPortsFloorCheckOut()
        {
            dgvFloorCheckOut.Rows.Clear();
            dgvFloorCheckOut.ColumnCount = 5;


            dgvFloorCheckOut.Columns[0].Name = "S.NO";
            dgvFloorCheckOut.Columns[1].Name = "Items";
            dgvFloorCheckOut.Columns[2].Name = "Quantity";
            dgvFloorCheckOut.Columns[4].Name = "Productid";
            //dgvFloorCheckOut.Columns[6].Name = "Productid";
            dgvFloorCheckOut.Columns[3].Name = "Rack";


            this.dgvFloorCheckOut.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvFloorCheckOut.Columns[4].Visible = false;

            this.dgvFloorCheckOut.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvFloorCheckOut.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvFloorCheckOut.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            this.dgvFloorCheckOut.Columns[0].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[1].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[2].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[3].ReadOnly = true;



            this.dgvFloorCheckOut.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            DataGridViewComboBoxColumn name = new DataGridViewComboBoxColumn();
            name.HeaderText = "Location";
            name.DataPropertyName = "Location";
            name.FlatStyle = FlatStyle.Popup;
            dgvFloorCheckOut.Columns.Insert(3, name);
            //this.dgvFloorCheckOut.Columns[3].Width = 150;


            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "ChkFloorCheckOut";
            dgvCmb.HeaderText = "IsCheckOut";
            dgvCmb.FlatStyle = FlatStyle.Popup;

            dgvFloorCheckOut.Columns.Insert(6, dgvCmb);


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvFloorCheckOut.Columns[0].Width = 45;
                this.dgvFloorCheckOut.Columns[1].Width = 300;
                this.dgvFloorCheckOut.Columns[2].Width = 100;
                this.dgvFloorCheckOut.Columns[3].Width = 100;
                this.dgvFloorCheckOut.Columns["Rack"].Width = 250;
                this.dgvFloorCheckOut.Columns[6].Width = 80;

            }
            else
            {
                this.dgvFloorCheckOut.Columns[0].Width = 45;
                this.dgvFloorCheckOut.Columns[1].Width = 300;
                this.dgvFloorCheckOut.Columns[2].Width = 110;
                this.dgvFloorCheckOut.Columns[3].Width = 110;

                this.dgvFloorCheckOut.Columns[6].Width = 80;

            }
            dgvFloorCheckOut.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvFloorCheckOut.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }
        private void LoadPortsChecking()
        {
            dgvChecking.Rows.Clear();
            dgvChecking.ColumnCount = 5;


            dgvChecking.Columns[0].Name = "S.NO";
            dgvChecking.Columns[1].Name = "Items";

            dgvChecking.Columns[2].Name = "originalQuantity";
            dgvChecking.Columns[3].Name = "Quantity";
            dgvChecking.Columns[4].Name = "Productid";

            this.dgvChecking.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            this.dgvChecking.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvChecking.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvChecking.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvChecking.Columns[0].ReadOnly = true;
            this.dgvChecking.Columns[1].ReadOnly = true;


            this.dgvChecking.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvChecking.Columns[2].Visible = false;
            this.dgvChecking.Columns[4].Visible = false;

            this.dgvChecking.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            //DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            //dgvCmb.ValueType = typeof(bool);
            //dgvCmb.Name = "ChkIsCheckOut";
            //dgvCmb.HeaderText = "Is CheckOut";
            //dgvChecking.Columns.Insert(4, dgvCmb);
            //this.dgvChecking.Columns[4].Width = 110;

            //DataGridViewCheckBoxColumn dgvCm = new DataGridViewCheckBoxColumn();
            //dgvCm.ValueType = typeof(bool);
            //dgvCm.Name = "ChkIsChecking";
            //dgvCm.HeaderText = "Checked";
            //dgvChecking.Columns.Insert(5, dgvCm);
            //this.dgvChecking.Columns[5].Width = 110;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvChecking.Columns[0].Width = 10;
                this.dgvChecking.Columns[1].Width = 120;
                this.dgvChecking.Columns[3].Width = 80;
                //this.dgvChecking.Columns[4].Width = 80;
                //this.dgvChecking.Columns[5].Width = 80;
            }
            else
            {
                this.dgvChecking.Columns[0].Width = 10;
                this.dgvChecking.Columns[1].Width = 120;
                this.dgvChecking.Columns[3].Width = 80;
                //this.dgvChecking.Columns[4].Width = 80;
                //this.dgvChecking.Columns[5].Width = 80;

            }
            dgvChecking.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvChecking.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }
        private void LoadPortsDelivery()
        {
            dgvDelivery.Rows.Clear();
            dgvDelivery.ColumnCount = 6;


            dgvDelivery.Columns[0].Name = "S.NO";
            dgvDelivery.Columns[1].Name = "Items";
            dgvDelivery.Columns[2].Name = "Quantity";

            dgvDelivery.Columns[3].Name = "Productid";
            dgvDelivery.Columns[4].Name = "Delivered Qty";
            dgvDelivery.Columns[5].Name = "Balance Qty";


            this.dgvDelivery.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            dgvDelivery.Columns[3].Visible = false;
            this.dgvDelivery.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvDelivery.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvDelivery.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvDelivery.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvDelivery.Columns[0].ReadOnly = true;
            this.dgvDelivery.Columns[1].ReadOnly = true;
            this.dgvDelivery.Columns[2].ReadOnly = true;
            this.dgvDelivery.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvDelivery.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvDelivery.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvDelivery.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;




            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "ChkDelivery";
            dgvCmb.HeaderText = "Delivered";
            dgvDelivery.Columns.Insert(6, dgvCmb);
            this.dgvDelivery.Columns[6].Width = 60;
            this.dgvDelivery.Columns[5].ReadOnly = true;
            this.dgvDelivery.Columns[6].ReadOnly = true;

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvDelivery.Columns[0].Width = 40;
                this.dgvDelivery.Columns[1].Width = 250;
                this.dgvDelivery.Columns[2].Width = 70;
                this.dgvDelivery.Columns[3].Width = 120;
                this.dgvDelivery.Columns[4].Width = 120;
                this.dgvDelivery.Columns[5].Width = 120;
            }
            else
            {
                this.dgvDelivery.Columns[0].Width = 30;
                this.dgvDelivery.Columns[1].Width = 200;
                this.dgvDelivery.Columns[2].Width = 80;
                this.dgvDelivery.Columns[3].Width = 110;
                this.dgvDelivery.Columns[4].Width = 110;
                this.dgvDelivery.Columns[5].Width = 110;

            }
            dgvDelivery.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvDelivery.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }
        private void MainTabSalesBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedtab = MainTabSalesBill.SelectedTab.Name;
            if (selectedtab == "TabNew")
            {
                //search("Quotationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                btnPrint.Enabled = true;
                btnNew.Enabled = true;
                btnsave.Enabled = true;
                btnSavePending.Enabled = true;
                btnClear.Enabled = true;

            }


            else if (selectedtab == "TabFloorCheckOut")
            {
                searchcheckout("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                btnsave.Enabled = true;
                btnClear.Enabled = true;
            }


            else if (selectedtab == "TabCreditApproval")
            {

                searchcreditapproved("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                btnPrint.Enabled = false;
                btnNew.Enabled = false;
                btnsave.Enabled = false;
                btnSavePending.Enabled = false;
                btnClear.Enabled = true;
            }
            else if (selectedtab == "TapReturnApproval")
            {

                searchReturnapproved("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                btnPrint.Enabled = false;
                btnNew.Enabled = false;
                btnsave.Enabled = false;
                btnSavePending.Enabled = false;
                btnClear.Enabled = true;
            }

            else if (selectedtab == "TabPayment")
            {

                searchpay("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                btnPrint.Enabled = false;
                btnNew.Enabled = false;
                btnsave.Enabled = false;
                btnSavePending.Enabled = false;
                btnClear.Enabled = true;
            }

            else if (selectedtab == "TabChecking")
            {

                searchcheck("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                btnPrint.Enabled = false;
                btnNew.Enabled = false;
                btnsave.Enabled = true;
                btnSavePending.Enabled = false;
                btnClear.Enabled = true;
            }
            else if (selectedtab == "TabDelivery")
            {

                searchdelivery("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                btnPrint.Enabled = false;
                btnNew.Enabled = false;
                btnsave.Enabled = true;
                btnSavePending.Enabled = false;
                btnClear.Enabled = true;
            }


            else if (selectedtab == "TapReturnCheckin")
            {
                searchreturncheck("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                btnPrint.Enabled = false;
                btnNew.Enabled = false;
                btnsave.Enabled = true;
                btnSavePending.Enabled = false;
                btnClear.Enabled = true;
            }

        }
        #endregion



        private void LoadPortsreturnChecking()
        {
            DgvCkin.Rows.Clear();
            DgvCkin.ColumnCount = 5;


            DgvCkin.Columns[0].Name = "S.NO";
            DgvCkin.Columns[1].Name = "Items";
            DgvCkin.Columns[2].Name = "Quantity";
            DgvCkin.Columns[4].Name = "Productid";
            //dgvFloorCheckOut.Columns[6].Name = "Productid";
            DgvCkin.Columns[3].Name = "Rack";


            this.DgvCkin.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.DgvCkin.Columns[4].Visible = false;

            this.DgvCkin.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.DgvCkin.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.DgvCkin.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.DgvCkin.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.DgvCkin.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.DgvCkin.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            this.DgvCkin.Columns[0].ReadOnly = true;
            this.DgvCkin.Columns[1].ReadOnly = true;
            this.DgvCkin.Columns[2].ReadOnly = true;
            this.DgvCkin.Columns[3].ReadOnly = true;



            this.DgvCkin.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            DataGridViewComboBoxColumn name = new DataGridViewComboBoxColumn();
            name.HeaderText = "Location";
            name.DataPropertyName = "Location";
            name.FlatStyle = FlatStyle.Popup;
            DgvCkin.Columns.Insert(3, name);
            //this.dgvFloorCheckOut.Columns[3].Width = 150;


            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "ChkFloorCheckOut";
            dgvCmb.HeaderText = "IsCheckIn";
            dgvCmb.FlatStyle = FlatStyle.Popup;

            DgvCkin.Columns.Insert(6, dgvCmb);


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.DgvCkin.Columns[0].Width = 45;
                this.DgvCkin.Columns[1].Width = 300;
                this.DgvCkin.Columns[2].Width = 100;
                this.DgvCkin.Columns[3].Width = 100;
                this.DgvCkin.Columns["Rack"].Width = 250;
                this.DgvCkin.Columns[6].Width = 80;

            }
            else
            {
                this.DgvCkin.Columns[0].Width = 45;
                this.DgvCkin.Columns[1].Width = 300;
                this.DgvCkin.Columns[2].Width = 110;
                this.DgvCkin.Columns[3].Width = 110;

                this.DgvCkin.Columns[6].Width = 80;

            }
            DgvCkin.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in DgvCkin.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }


        public void AutoCompleteLoad(string s, int t)
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.itemauto(s, t);

            if (st.Rows.Count > 0)
            {
                DgvAutoRefNo.Visible = true;
                DgvAutoRefNo.DataSource = st;
                res = false;
                cas = Convert.ToString(DgvAutoRefNo.Rows[0].Cells[0].Value);

                DgvAutoRefNo.Focus();
                DgvAutoRefNo.CurrentCell = DgvAutoRefNo[0, 0];
                string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                getitems(sa);
            }
            else
            {
                DgvAutoRefNo.Visible = false;
                DgvAutoRefNo.Visible = false;
                lblproductid.Text = string.Empty;
                //Txtitem.Text = string.Empty;
                lblitemcode.Text = "0";
                lblrack.Text = "0";
                lbldisplay.Text = "0";
                lbldemo.Text = "0";
                lblservice.Text = "0";
                lbldamage.Text = "0";
                lblprice.Text = "0";
            }


            //string[] arr = new string[st.Rows.Count];
            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    arr[i] = st.Rows[i]["DisplayName"].ToString();
            //}
            //for (int i = 0; i < arr.Length; i++)
            //{
            //    //var combined = string.Join(", ", arr);
            //    var combined = arr[i];
            //    str.Add(combined);
            //}

            //Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //Txtitem.AutoCompleteCustomSource = str;
            //Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;

            //for (int i = 0; i < arr.Length; i++)
            //{
            //  var combined = string.Join(", ", arr);
            //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //str.Add(combined);
            //}

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}


        }

        public AutoCompleteStringCollection Autocusomer()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.Getcustomer();
            string[] arr = new string[st.Rows.Count];
            for (int i = 0; i < st.Rows.Count; i++)
            {
                arr[i] = st.Rows[i]["Name"].ToString();
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

        private void TabPaymentReceived_Click(object sender, EventArgs e)
        {

        }

        private void comboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlpaymode.SelectedIndex == 2)
            {
                lblpaymentamount.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpatroundoff.Text));
                cashpl.Visible = true;
                panelTransaction.Visible = false;
                this.ActiveControl = cashddl;
                Application.Idle += new EventHandler(Application_Idle);
            }
            else if (ddlpaymode.SelectedIndex == 1)
            {
                panelTransaction.Visible = true;
                lblChequeNo.Visible = true;
                txttransactionid.Visible = true;
                lblcardammount.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpatroundoff.Text));
                label105.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpatroundoff.Text));
                cmbbank.Focus();
            }
            else
            {
                panelCustomerpaid.Visible = false;
                panelTransaction.Visible = false;
                cashpl.Visible = false;
            }

        }

        private void btnDenomination_Click(object sender, EventArgs e)
        {
            cashpl.Visible = true;

        }

        private void cashclose_Click(object sender, EventArgs e)
        {
            cashpl.Visible = false;
        }

        private void cashdetailsclose_Click(object sender, EventArgs e)
        {
            cashpl.Visible = false;
            cashddl.Columns.Clear();
            RbPaymentCard.Checked = false;
            rbpaymentCash.Checked = false;
            Rdcashcard.Checked = false;
            Rdcashcard.Checked = false;

            paymentdenobind();
            lblpaymenttotal.Text = "0.00";
            lblpaymentamount.Text = "0.00";
            lblpaymentbalance.Text = "0.00";
        }

        private void transactionclose_Click(object sender, EventArgs e)
        {
            panelTransaction.Visible = false;
            RbPaymentCard.Checked = false;
            rbpaymentCash.Checked = false;
            Rdcashcard.Checked = false;
            cmbbank.SelectedIndex = 0;
            txtcardno.Text = string.Empty;
            txttransactionid.Text = string.Empty;
            Rdcashcard.Checked = false;

        }

        private void MainTabSalesBill_Click(object sender, EventArgs e)
        {

        }

        //private void SearchPurchaseOrder()
        //{

        //    dgvSearch.Rows.Clear();
        //    dgvSearch.Columns.Clear();
        //    dgvSearch.ColumnCount = 2;


        //    dgvSearch.Columns[0].Name = "Order No";
        //    dgvSearch.Columns[1].Name = "Quotation No";
        //    //dgvSearch.Columns[3].Name = "Customer Name";
        //    //dgvSearch.Columns[4].Name = "Date";




        //    this.dgvSearch.Columns[0].Width = 60;

        //    this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        //  //  this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


        //    this.dgvSearch.Columns[1].Width = 60;




        //   // this.dgvSearch.Columns[2].Width = 60;


        //    dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //    dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
        //    dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;




        //    dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    foreach (DataGridViewColumn c in dgvSearch.Columns)
        //    {
        //        c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
        //    }

        //    //this.dgvSearch.Columns[3].Visible = false;
        //    //this.dgvSearch.Columns[4].Visible = false;

        //}

        private void btnGenerateBill_Click(object sender, EventArgs e)
        {
            if (ddlpaymenttype.SelectedIndex == 0)
            {
                MessageBox.Show("Select payment type");
                this.ActiveControl = ddlpaymenttype;
                return;
            }


        }


        #region Validation
        public bool validationNEW()
        {
            bool Status = true;
            string msg = "";
            int i = 0;
            if (ddlpaymenttype.SelectedIndex == 0)
            {
                i++;
                msg = msg + "*Select Payment type" + "\n";
                if (i == 1)
                    this.ActiveControl = ddlpaymenttype;

            }
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + msg);
                Status = false;

            }
            return Status;


        }

        public bool validationPaymentCheque()
        {
            bool Status = true;
            string msg = "";
            int i = 0;
            if (cmbbank.SelectedIndex == 0)
            {
                i++;
                msg = msg + "*Select Your Account" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbbank;

            }

            if (string.IsNullOrEmpty(txtcardno.Text))
            {
                i++;
                msg = msg + "*Enter Card No" + "\n";
                if (i == 1)
                    this.ActiveControl = txttransactionid;

            }
            
            if (string.IsNullOrEmpty(lblcardammount.Text))
            {
                i++;
                msg = msg + "*Enter Your Amount" + "\n";
                if (i == 1)
                    this.ActiveControl = lblcardammount;

            }
            if (lblcardammount.Text==".")
            {
                i++;
                msg = msg + "*Enter Correct Amount" + "\n";
                if (i == 1)
                    this.ActiveControl = lblcardammount;

            }

           
            if (lblcardammount.Text!= ".")
            {
                if (Rdcashcard.Checked)
                {
                    double sas = Convert.ToDouble(label105.Text);
                    if (Convert.ToDouble(lblcardammount.Text) > sas)
                    {
                        i++;
                        msg = msg + "*Amount Should Not Greater Than Total Amount " + "\n";
                        if (i == 1)
                            this.ActiveControl = lblcardammount;
                    }

                }

                else
                {
                    if (Convert.ToDouble(lblcardammount.Text) > Convert.ToDouble(lblpatroundoff.Text))
                    {
                        i++;
                        msg = msg + "*Amount Should Not Greater Than Total Amount " + "\n";
                        if (i == 1)
                            this.ActiveControl = lblcardammount;

                    }
                }
            }

            if (lblpaymentmode.Text != "Partial Credit Bill")
            {
                if (lblcardammount.Text != ".")
                {
                    if (Rdcashcard.Checked)
                    {
                        if (Convert.ToDouble(lblcardammount.Text) != Convert.ToDouble(label105.Text))
                        {
                            i++;
                            msg = msg + "*Please Pay Full Amount" + "\n";
                            if (i == 1)
                                this.ActiveControl = lblcardammount;
                        }
                    }
                    else
                    {
                        if (Convert.ToDouble(lblcardammount.Text) != Convert.ToDouble(lblpatroundoff.Text))
                        {
                            i++;
                            msg = msg + "*Please Pay Full Amount" + "\n";
                            if (i == 1)
                                this.ActiveControl = lblcardammount;
                        }
                    }
                }
                else
                {
                    i++;
                    msg = msg + "*Please Enter Correct Amount" + "\n";
                    if (i == 1)
                        this.ActiveControl = lblcardammount;
                }
            }

            if (string.IsNullOrEmpty(txttransactionid.Text))
            {
                i++;
                msg = msg + "*Enter Transction  No" + "\n";
                if (i == 1)
                    this.ActiveControl = txttransactionid;

            }
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + msg);
                Status = false;

            }
            return Status;


        }
        #endregion

        private void btntransactionpay_Click(object sender, EventArgs e)
        {
            if (validationPaymentCheque())
            {
                if (Rdcashcard.Checked)
                {
                    savepayment();
                    if (Convert.ToDouble(lblpaidbalance.Text) > 0)
                    {
                        SavepaymentDenomination();
                    }

                }
                savecard();
                ddlpaymode.Enabled = false;
                rbpaymentCash.Enabled = false;
                RbPaymentCard.Enabled = false;
                Rdcashcard.Enabled = false;
                cashpl.Visible = false;



                searchpay("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);

            }
        }



        private void SalesBillNew_Load(object sender, EventArgs e)
        {
            this.ActiveControl = Txtcustomername;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {

                if (MainTabSalesBill.SelectedTab.Name == "TabNew")
                {
                    if (keyData == (Keys.Alt | Keys.Insert))
                    {
                        if (dgvNew.Rows.Count <= 0)
                        {
                            dgvNew.Rows.Add();
                        }
                        else
                        {
                            int rowindex = dgvNew.CurrentRow.Index;
                            int colindex = dgvNew.CurrentCell.ColumnIndex;
                            //dgvOrder.Rows.Insert(rowindex, dgvOrder.Rows.Add(1));
                            dgvNew.Rows.Insert(rowindex, 1);
                            return true;
                        }

                        getsino();

                    }

                    if (keyData == (Keys.Alt | Keys.Delete))
                    {
                        DialogResult result = MessageBox.Show("Do you want to Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            if (dgvNew.Rows.Count > 0)
                            {
                                try
                                {
                                    int rowindex = dgvNew.CurrentRow.Index;
                                    int colindex = dgvNew.CurrentCell.ColumnIndex;
                                    dgvNew.Rows.RemoveAt(rowindex);
                                }
                                catch
                                {

                                    if (dgvNew.Rows.Count - 1 == dgvNew.CurrentCell.RowIndex)
                                    {
                                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[0].Value = "";
                                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[1].Value = "";
                                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[2].Value = "";
                                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[3].Value = "";
                                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value = "";
                                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value = "";
                                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = "";

                                    }
                                }
                                getsino();
                                pnsearch.Visible = false;
                                return true;
                            }

                            if (dgvNew.Rows.Count == 0)
                            {
                                dgvNew.Rows.Add();
                            }

                        }

                    }
                }
                if (keyData == (Keys.Alt | Keys.S))
                {
                    rdbStartsWith.Checked = true;
                    return true;
                }
                if (keyData == (Keys.Alt | Keys.C))
                {
                    rdbContains.Checked = true;
                    return true;
                }

                if (groupBox5.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {

                        rbCash.Focus();
                        return true;
                    }
                }

                if (rbCash.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {

                        rbcredit.Focus();
                        return true;
                    }
                }

                if (rbcredit.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {

                        rbpartial.Focus();
                        return true;
                        
                    }
                }


                if (rbpartial.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {

                        txtRemarks.Focus();
                        return true;
                    }
                }

                if (txtRemarks.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {

                        btnsave.Focus();
                        return true;
                    }
                }

                //if (txtless.Focused)
                //{
                //    if (keyData == (Keys.Tab))
                //    {
                //        btnsave.Focus();


                //        return true;
                //    }
                //}

                //if (keyData == Keys.Escape)
                //{
                //    pnsearch.Visible = false;
                //    selectedtab = MainTabSalesBill.SelectedTab.Name;

                //    if (selectedtab == "TabPayment")
                //    {
                //        cashdetailsclose.PerformClick();
                //    }

                //    panel11.Focus();
                //    dgvNew.Focus();
                //    dgvNew.CurrentCell = dgvNew[2, dgvNew.CurrentCell.RowIndex];


                //    return true;
                //}




                if (keyData == Keys.Escape)
                {

                    selectedtab = MainTabSalesBill.SelectedTab.Name;

                    if (selectedtab == "TabPayment")
                    {
                        if (cashdetailsclose.Visible)
                        {
                            cashdetailsclose.PerformClick();
                        }
                    }

                    if (pnsearch.Visible)
                    {
                        pnsearch.Visible = false;
                        panel11.Focus();
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[2, dgvNew.CurrentCell.RowIndex];
                    }
                    else
                    {
                        if (dgvNew.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[0].Cells[1].Value)))
                        {
                            DialogResult result = MessageBox.Show("Do you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                this.Close();
                            }
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                    return true;
                }



                if (date1.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        dgvNew.Focus();
                        if (dgvNew.Rows.Count == 0)
                        {
                            dgvNew.Rows.Add();
                        }
                        dgvNew.CurrentCell = dgvNew[1, 0];
                        return true;
                    }

                }

                if (txtcheckquotationid.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = dgvFloorCheckOut;
                        return true;
                    }
                }
                if (btnreceiveBalance.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = btncashpay;
                        return true;
                    }

                }
                if (cmbloaction.Focused)
                {
                    try
                    {
                        if (keyData == (Keys.Tab))
                        {
                            this.ActiveControl = Txtitem;
                            return true;
                        }
                    }
                    catch
                    {

                    }

                }

                if (txtRemarks.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = rbCash;
                        return true;
                    }

                }
                if (rbCash.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = txtless;
                        return true;
                    }

                }
                if (cmbbank.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = txtcardno;
                        return true;
                    }

                }

                if (cmbloaction.Focused)
                {
                    try
                    {
                        if (keyData == (Keys.Tab))
                        {
                            this.ActiveControl = Txtitem;
                            return true;
                        }
                    }
                    catch
                    {

                    }

                }
                if (txtcardno.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = txttransactionid;
                        return true;
                    }

                }

                if (txttransactionid.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = btntransactionpay;
                        return true;
                    }

                }
                try
                {
                    if (keyData == Keys.Tab)
                    {
                        if (dgvNew.CurrentCell.ColumnIndex == 5)
                        {
                            dgvNew.Focus();
                            //edit = true;
                            dgvNew.CurrentCell = dgvNew[1, dgvNew.CurrentCell.RowIndex + 1];
                        }
                    }
                }
                catch
                {

                }

                if (dgvFloorCheckOut.CurrentCell.RowIndex == dgvFloorCheckOut.Rows.Count - 1 && dgvFloorCheckOut.CurrentCell.ColumnIndex == 6)
                {
                    try
                    {
                        if (keyData == Keys.Tab)
                        {
                            btnsave.Focus();
                            return true;
                        }
                    }
                    catch
                    {

                    }
                }

            }
            catch
            {

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        public void itemdetails(string s)
        {

            try
            {
                dtitems = new DataTable();
                string s1 = s.Trim();
                string s2 = Convert.ToString("");
                string name = s1.Replace("'", "''");


                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                dtitems = objQuotationbal.itemdetails(name, s2);
                Program.dtitems = dtitems;


            }
            catch (Exception e)
            {

            }

        }


        public void GetReport(string QuotationId)
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
                            cmd.Parameters.AddWithValue("@companyname", Program.Company);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "GetQuotationEstimationreport_print";
                            cmd.Connection = con;
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(ds);

                            EstimationReportDAL Obj = new EstimationReportDAL();
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

        private void btncreditsave_Click(object sender, EventArgs e)
        {
            if (selectedtab == "TabNew")
            {
                bool vali = Validation1();
                bool retuenSt = true;
                if (vali)
                {
                    for (int i = 0; i < dgvNew.Rows.Count; i++)
                    {
                        if (Convert.ToDecimal(dgvNew.Rows[i].Cells[5].Value) < 0)
                        {
                            retuenSt = false;
                        }
                    }
                    if (retuenSt == false)
                    {
                        save(3);
                    }
                    else
                    {
                        save(2);
                    }

                    search("Quotationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                    //Pnloading6.Visible = false;
                   // clear();
                }
            }
            else if (selectedtab == "TabFloorCheckOut")
            {
                bool valcheck = Validationcheckout();
                if (valcheck)
                {

                    DataTable dt = DataGridView2DataTable(dgvFloorCheckOut);
                    RemoveNullColumnFromDataTable(dt);
                    dt.Columns.RemoveAt(0);
                    dt.Columns.RemoveAt(0);
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(3);
                    dt.Columns["Column1"].ColumnName = "Rack";
                    dt.AcceptChanges();
                    savecheck(dt, Program.userid);
                    searchcheckout("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                    itemdetailsval("");
                }
                else
                {
                    MessageBox.Show("Please Check All Items");
                }

            }


            if (selectedtab == "TabChecking")
            {
                if (valcke())
                {
                    bool vali = Validationchecking();
                    if (vali)
                    {
                        DataTable dt = DataGridView2DataTable(dgvChecking);
                        RemoveNullColumnFromDataTable(dt);
                        dt.Columns.RemoveAt(0);
                        dt.Columns.RemoveAt(0);
                        dt.Columns["Quantity"].ColumnName = "Rack";
                        dt.AcceptChanges();
                        dt.Columns["originalQuantity"].ColumnName = "Quantity";

                        dt.AcceptChanges();

                        savechecking(dt);
                        searchcheck("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);

                    }
                    else
                    {
                        MessageBox.Show("Quantity Should Not Be Empty");
                    }
                }

            }

            if (selectedtab == "TabDelivery")
            {
                bool vas = Validationdelivery1();
                bool vali = Validationdelivery();
                if (vas == true)
                {
                    if (vali)
                    {
                        DataTable dt = DataGridView2DataTable(dgvDelivery);
                        RemoveNullColumnFromDataTable(dt);
                        dt.Columns.RemoveAt(0);
                        dt.Columns.RemoveAt(0);
                        dt.Columns.RemoveAt(2);
                        dt.Columns.RemoveAt(2);
                        dt.Columns.RemoveAt(2);
                        dt.Columns.Add("Rack").SetOrdinal(1);

                        deletedelivered();
                        savedelivered("", 2, dt, "", "", 0);
                        //string s = getcheckedvalue();
                        searchdelivery("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                        //Pnloading6.Visible = false;
                        clear();
                    }
                    else
                    {
                        //DialogResult result = MessageBox.Show("Do you want to Partial Deliverd?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        //if (result == DialogResult.Yes)
                        //{
                        deletedelivered();
                        string s = getcheckedvalue();
                        searchdelivery("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                        // Pnloading6.Visible = false;
                        clear();

                        //}
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Txtdeliveryorderno.Text))
                    {
                        MessageBox.Show("Please select Estimation");
                    }
                    else
                    {
                        MessageBox.Show("Please Enter Correct Quantity");
                    }
                }

            }

            if (selectedtab == "TapReturnCheckin")
            {
                bool valcheck = Validationreturncheck();

                if (valcheck)
                {

                    DataTable dt = DataGridView2DataTable(DgvCkin);
                    RemoveNullColumnFromDataTable(dt);

                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["ChkFloorCheckOut"])))
                        {
                            dt.Rows.RemoveAt(i);
                            dt.AcceptChanges();
                        }
                    }

                    dt.Columns.RemoveAt(0);
                    dt.Columns.RemoveAt(0);
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(3);
                    dt.Columns["Column1"].ColumnName = "Rack";
                    dt.AcceptChanges();
                    savereturncheck(dt);
                    searchreturncheck("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                }
                else
                {
                    MessageBox.Show("Please Check All Return Items");
                }
            }


        }

        public void savereturncheck(DataTable dt)
        {


            string output = objQuotationbal.savereturncheck(txtckinestno.Text, dt, txtckinRemarks.Text,Program.userid);
            if (output == "1")
            {
                //Pnloading1.Visible = false;
                clear();

            }


        }



        private void btnSavePending_Click(object sender, EventArgs e)
        {
            if (selectedtab == "TabNew")
            {
                bool vali = Validation1();
                if (vali)
                {
                    save(1);
                }
            }
            else if (selectedtab == "TabFloorCheckOut")
            {
                bool valcheck = Validationcheckout();

                if (valcheck)
                {
                    searchcheckout("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                }
                else
                {
                    MessageBox.Show("Please Check All Items");
                }

            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }


        private bool Validationchecking()
        {
            bool status = true;
            string message = "";
            for (int i = 0; i < dgvChecking.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dgvChecking.Rows[i].Cells[3].Value)) && (Convert.ToDecimal(dgvChecking.Rows[i].Cells[2].Value) >= 0))
                {
                    status = false;
                }

            }

            return status;
        }

        public bool valcke()
        {
            bool status = true;
            try
            {

                for (int i = 0; i < dgvChecking.Rows.Count; i++)
                {
                    if (Convert.ToDouble(dgvChecking.Rows[i].Cells[2].Value) > 0)
                    {
                        if (Convert.ToDouble(dgvChecking.Rows[i].Cells[2].Value) != Convert.ToDouble(dgvChecking.Rows[i].Cells[3].Value))
                        {

                            MessageBox.Show("Please Enter Correct Quantity");

                            dgvChecking.Focus();
                            edit = true;
                            dgvChecking.CurrentCell = dgvChecking[3, i];
                            dgvChecking.Rows[i].Cells[3].Value = "";
                            status = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {

                MessageBox.Show("Please Enter Correct Quantity");
                status = false;


            }
            return status;
        }


        private bool Validation1()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (Txtcustomername.Text == "--Select--")
            {
                i++;
                message = message + "* Please Enter Customer Name" + "\n";
                if (i == 1)
                    this.ActiveControl = Txtcustomername;
            }



            if (Convert.ToDouble(lblTotal.Text)>500)
            {
                i++;
                message = message + "* Estimation Should Be Equal And Less Than 500" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvNew;
            }

            //if (string.IsNullOrEmpty(cmdcity.Text))
            //{
            //    i++;
            //    message = message + "* Please Select city" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmdcity;
            //}

            if (cmbassistby.SelectedIndex == 0)
            {
                i++;
                message = message + "* Please Select Assist By " + "\n";
                if (i == 1)
                    this.ActiveControl = cmbassistby;
            }


            //if ((rbcredit.Checked || rbpartial.Checked) && (string.IsNullOrEmpty(Convert.ToString(Txtcustomername.SelectedValue)) || Convert.ToString(Txtcustomername.SelectedValue) == "0"))
            if ((rbcredit.Checked || rbpartial.Checked) )
            {
                int s = objQuotationbal.getcust(Convert.ToString(Txtcustomername.SelectedValue), Txtcustomername.Text.Trim());
                if (s != 1)
                {
                    if (string.IsNullOrEmpty(TxtMobilenumber.Text))
                    {
                        i++;
                        message = message + "* Please Enter Mobile Number" + "\n";
                        if (string.IsNullOrEmpty(objQuotationbal.Customerid))
                        {
                            panel11.Visible = true;
                            this.ActiveControl = TxtMobilenumber;
                        }
                        if (i == 1)
                            this.ActiveControl = TxtMobilenumber;
                    }
                }
            }


            //if (cmbreference.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please select Reference" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmbreference;
            //}


            if (val() == false)
            {
                i++;
                message = message + "* Please select PaymentMode" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbpaymode;
            }
            if (Txtothers.Text == ".")
            {
                i++;
                message = message + "* Please Enter Correct Others" + "\n";
                if (i == 1)
                    this.ActiveControl = Txtothers;
            }
            if (txtless.Text == ".")
            {
                i++;
                message = message + "* Please Enter Correct Less Amount" + "\n";
                if (i == 1)
                    this.ActiveControl = txtless;
            }

            if (txtless.Text != ".")
            {
                if (!string.IsNullOrEmpty(txtless.Text))
                {
                    if (Convert.ToDouble(lbltotalamount.Text) > 0)
                    {
                        if (Convert.ToDouble(lbltotalamount.Text) <= Convert.ToDouble(txtless.Text))
                        {
                            i++;
                            message = message + "* Less Amount Should Not Be Greater Than TotalAmount" + "\n";
                            if (i == 1)
                                this.ActiveControl = txtless;
                        }
                    }
                }
            }




            if (dgvNew.Rows.Count > 0)
            {
                i++;
                string Items = Convert.ToString(dgvNew.Rows[0].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvNew.Rows[0].Cells["Items"].Value);
                if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                {
                    message = message + "* Please Select Product" + "\n";
                }

                if (i == 1)
                    this.ActiveControl = dgvNew;
            }
            else if (dgvNew.Rows.Count == 0)
            {
                i++;
                message = message + "* Please Select Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvNew;
            }

            bool sas = false;

            for (int k = 0; k < dgvNew.RowCount; k++)
            {
                string Items = Convert.ToString(dgvNew.Rows[k].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvNew.Rows[k].Cells["Items"].Value);

                if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))) || Items == "." || Items == "-")
                {
                    sas = true;
                    break;
                }
            }
            if (sas == true)
            {
                i++;
                message = message + "* Product or quantity should not be empty." + "\n";
                if (i == 1)
                    this.ActiveControl = dgvNew;
            }




            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

        public bool val()
        {
            bool status = true;

            if (rbCash.Checked || rbcredit.Checked || rbpartial.Checked)
            {
                status = true;
            }
            else
            {
                status = false;
            }
            return status;
        }

        private bool Validationcheckout()
        {
            bool status = true;
            string message = "";
            int i = 0;

            foreach (DataGridViewRow row in dgvFloorCheckOut.Rows)
            {
                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];

                if (Convert.ToBoolean(CbxCell.Value) == false && Convert.ToDecimal(row.Cells[2].Value) >= 0)
                {
                    status = false;
                    break;
                }

            }
            return status;
        }



        private bool Validationcreditapproval()
        {
            bool status = true;
            string message = "";


            if (string.IsNullOrEmpty(txtcreditorderno.Text))
            {
                status = false;
            }
            return status;
        }

        private bool ValidationReturnApproval()
        {
            bool status = true;
            string message = "";


            if (string.IsNullOrEmpty(txtReturnapprovalEstNo.Text))
            {
                status = false;
            }
            return status;
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

        private void clear()
        {

            if (selectedtab == "TabNew")
            {
                pnsearch.Visible = false;
                Txtcustomername.Text = "--Select--";
                cmdcity.Text = string.Empty;
                cmbassistby.SelectedIndex = 0;
                cmbreference.SelectedIndex = 0;
                button6.Enabled = false;
                dgvNew.Rows.Clear();
                lblperare.Text = Program.UserName;
                lbltotalamount.Text = "0.00";
                txtless.Text = "0.00";
                lblTotal.Text = "0.00";
                Txtothers.Text = "0.00";
                tatQuotationno.Text = string.Empty;
                Txtcustomername.Focus();
                txtOrderNo.Text = string.Empty;

                txtRemarks.Text = string.Empty;
                cmbloaction.SelectedIndex = 0;
                cmbpaymode.SelectedIndex = 0;
                //rbCash.Checked = true;
                lblless.Visible = false;
                txtless.Visible = false;
                Cmbless.Visible = false;
                groupBox5.Enabled = true;
                rbCash.Checked = false;
                rbcredit.Checked = false;
                rbpartial.Checked = false;
                button6.Enabled = false;
                btnLess.Enabled = false;
                TxtMobilenumber.Text = string.Empty;
                panel11.Visible = false;
                Txtothers.Text = "0";
                 cusbool = 0;

                 this.dgvNew.Columns["Rate"].ReadOnly = true;
                 this.dgvNew.Columns[0].ReadOnly = true;
                 this.dgvNew.Columns["Items"].ReadOnly = true;
                 this.dgvNew.Columns["UOM"].ReadOnly = true;
                 this.dgvNew.Columns["Amount"].ReadOnly = true;
            }
            else if (selectedtab == "TabFloorCheckOut")
            {
                txtcheckcustomername.Text = string.Empty;
                Txtcheckcity.Text = string.Empty;
                txtcheckreference.Text = string.Empty;
                txtcheckprepareby.Text = string.Empty;
                txtcheckassistedby.Text = string.Empty;

                dgvFloorCheckOut.Rows.Clear();

                txtcheckremarks.Text = string.Empty;
                txtcheckorderno.Text = string.Empty;
                txtcheckquotationid.Text = string.Empty;
            }

            else if (selectedtab == "TapReturnApproval")
            {
                txtReturnapprovalCustomerName.Text = string.Empty;
                txtReturnapprovalCity.Text = string.Empty;
                txtReturnapprovalEstNo.Text = string.Empty;
                txtReturnapprovalPreparedBy.Text = string.Empty;
                txtReturnapprovalReference.Text = string.Empty;
                Txtothers.Text = "0";

                dgvReturnApproval.Rows.Clear();

                txtReturnapprovalQuotationNo.Text = string.Empty;
                txtReturnapprovalRemarks.Text = string.Empty;
                txtReturnapprovalAssistBy.Text = string.Empty;

                txtReturnapprovalNet.Text = "0.00";
                txtReturnapprovalLess.Text = "0.00";
                txtReturnapprovalTotal.Text = "0.00";
            }

            else if (selectedtab == "TabCreditApproval")
            {
                creditcustomername.Text = string.Empty;
                txtcreditcity.Text = string.Empty;
                txtrcrediteference.Text = string.Empty;
                txtpreperedby.Text = string.Empty;
                Txtassistby.Text = string.Empty;

                dgvCreditApproval.Rows.Clear();

                txtcreditorderno.Text = string.Empty;
                txtcreditremarks.Text = string.Empty;
                Txtcreditquotationno.Text = string.Empty;

                lblcredittotal.Text = "0.00";
                txtcreditless.Text = "0.00";
                lblcreditgranttotal.Text = "0.00";
                Txtothers.Text = "0";



            }
            else if (selectedtab == "TabPayment")
            {
                txtpauycustomername.Text = string.Empty;
                txtpaycity.Text = string.Empty;
                txtpayref.Text = string.Empty;
                Txtprepareby.Text = string.Empty;
                txtpayassist.Text = string.Empty;

                dgvPayment.Rows.Clear();

                txtpayorderno.Text = string.Empty;
                txtpayremarks.Text = string.Empty;
                Txtpayquotationid.Text = string.Empty;

                lblpaytotal.Text = "0.00";
                txtpayless.Text = "0.00";
                lblpaynet.Text = "0.00";
                lblpatroundoff.Text = "0";
                label105.Text = "0";

                ddlpaymode.SelectedIndex = 0;
                RbPaymentCard.Checked = false;
                cmbbank.SelectedIndex = 0;
                txtcardno.Text = string.Empty;
                txttransactionid.Text = string.Empty;

                lblpaymentmode.Text = string.Empty;

                cashddl.Rows[0].Cells[1].Value = 0;
                cashddl.Rows[1].Cells[1].Value = 0;
                cashddl.Rows[2].Cells[1].Value = 0;
                cashddl.Rows[3].Cells[1].Value = 0;
                cashddl.Rows[4].Cells[1].Value = 0;
                cashddl.Rows[5].Cells[1].Value = 0;
                cashddl.Rows[6].Cells[1].Value = 0;
                cashddl.Rows[7].Cells[1].Value = 0;
                cashddl.Rows[8].Cells[1].Value = 0;

                cashddl.Rows[0].Cells[2].Value = 0;
                cashddl.Rows[1].Cells[2].Value = 0;
                cashddl.Rows[2].Cells[2].Value = 0;
                cashddl.Rows[3].Cells[2].Value = 0;
                cashddl.Rows[4].Cells[2].Value = 0;
                cashddl.Rows[5].Cells[2].Value = 0;
                cashddl.Rows[6].Cells[2].Value = 0;
                cashddl.Rows[7].Cells[2].Value = 0;
                cashddl.Rows[8].Cells[2].Value = 0;

                lblpaymenttotal.Text = "0.00";
                lblpaymentamount.Text = "0.00";
                lblpaymentbalance.Text = "0.00";


                dgvCustomerpaid.Rows[0].Cells[1].Value = 0;
                dgvCustomerpaid.Rows[1].Cells[1].Value = 0;
                dgvCustomerpaid.Rows[2].Cells[1].Value = 0;
                dgvCustomerpaid.Rows[3].Cells[1].Value = 0;
                dgvCustomerpaid.Rows[4].Cells[1].Value = 0;
                dgvCustomerpaid.Rows[5].Cells[1].Value = 0;
                dgvCustomerpaid.Rows[6].Cells[1].Value = 0;
                dgvCustomerpaid.Rows[7].Cells[1].Value = 0;
                dgvCustomerpaid.Rows[8].Cells[1].Value = 0;

                dgvCustomerpaid.Rows[0].Cells[2].Value = 0;
                dgvCustomerpaid.Rows[1].Cells[2].Value = 0;
                dgvCustomerpaid.Rows[2].Cells[2].Value = 0;
                dgvCustomerpaid.Rows[3].Cells[2].Value = 0;
                dgvCustomerpaid.Rows[4].Cells[2].Value = 0;
                dgvCustomerpaid.Rows[5].Cells[2].Value = 0;
                dgvCustomerpaid.Rows[6].Cells[2].Value = 0;
                dgvCustomerpaid.Rows[7].Cells[2].Value = 0;
                dgvCustomerpaid.Rows[8].Cells[2].Value = 0;

                lblpaidbalance.Text = "0.00";
                btncashpay.Enabled = true;
                cashpl.Visible = false;
                panelCustomerpaid.Visible = false;
                rbpaymentCash.Checked = false;
                Rdcashcard.Checked = false;
                groupBox3.Enabled = false;
                Txtothers.Text = "0.00";
                lblpaymentothers.Text = "0.00";
            }
            else if (selectedtab == "TabChecking")
            {
                Txtcheckcustmername.Text = string.Empty;
                txtcheckingcity.Text = string.Empty;
                txtcheckingreference.Text = string.Empty;
                Txtcheckingprepareby.Text = string.Empty;
                txtcheckassistby.Text = string.Empty;

                dgvChecking.Rows.Clear();

                txtcheckingorderno.Text = string.Empty;
                txtcheckingquotationid.Text = string.Empty;

            }
            else if (selectedtab == "TabDelivery")
            {
                txtdeliverycustomername.Text = string.Empty;
                Txtdeviverycity.Text = string.Empty;
                Txtdeliveryreference.Text = string.Empty;
                Txtdeliveryprepareby.Text = string.Empty;
                Txtdeviveryassist.Text = string.Empty;

                dgvDelivery.Rows.Clear();

                Txtdeliveryorderno.Text = string.Empty;
                Txtdeliveryquotationno.Text = string.Empty;

            }


            else if (selectedtab == "TapReturnCheckin")
            {
                txtCkinCusname.Text = string.Empty;
                txtckincity.Text = string.Empty;
                txtchkinref.Text = string.Empty;
                txtckinpreparedby.Text = string.Empty;
                txtckinassistby.Text = string.Empty;

                DgvCkin.Rows.Clear();

                txtckinestno.Text = string.Empty;
                txtckinRemarks.Text = string.Empty;
                txtckInQuotNo.Text = string.Empty;


            }


        }


        private bool Validationreturncheck()
        {
            bool status = true;
            string message = "";
            int i = 0;

            foreach (DataGridViewRow row in dgvFloorCheckOut.Rows)
            {
                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];

                if (Convert.ToBoolean(CbxCell.Value) == false && Convert.ToDecimal(row.Cells[2].Value) < 0)
                {
                    status = false;
                    break;
                }

            }
            return status;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToDouble(lblpaymenttotal.Text) == 0.00)
            {
                MessageBox.Show("Please Enter Amount ");
            }
            else if (Convert.ToDouble(lblpaymentbalance.Text) <= 0.00)
            {
                if (Rdcashcard.Checked)
                {
                    btncashpay.Enabled = false;
                    panelCustomerpaid.Visible = true;
                    this.ActiveControl = dgvCustomerpaid;
                    dgvCustomerpaid.CurrentCell = dgvCustomerpaid[1, dgvCustomerpaid.CurrentCell.RowIndex];
                }
            }

            else
            {
                btncashpay.Enabled = false;
                panelCustomerpaid.Visible = true;
                this.ActiveControl = dgvCustomerpaid;
                dgvCustomerpaid.CurrentCell = dgvCustomerpaid[1, dgvCustomerpaid.CurrentCell.RowIndex];
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panelCustomerpaid.Visible = false;
            dgvCustomerpaid.Columns.Clear();
            paymentDenotoCustomerbind();
            lblpaidbalance.Text = "0.00";
            btncashpay.Enabled = true;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            preview();
        }

        public void preview()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtOrderNo.Text))
                {
                    DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Quotationreport rpt = new Quotationreport(txtOrderNo.Text);
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

        public void bindcustomer()
        {
            Txtcustomername.DataSource = objQuotationbal.Getcustomer();
            Txtcustomername.DisplayMember = "Name";
            Txtcustomername.ValueMember = "CustomerID";
        }


        public DataTable bindEstimation()
        {
            DataTable dt = new DataTable();
            dt = objQuotationbal.itemestimationorderno();
            DataRow dr = dt.NewRow();
            dr["Quotationid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);

            return dt;
        }

        public DataTable bindcheckout()
        {
            DataTable dt = new DataTable();
            dt = objQuotationbal.itemcheckoutorderno();
            DataRow dr = dt.NewRow();
            dr["Estimationid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);
            return dt;
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

        private void dgvNew_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5)
                {
                    dgvNew.Focus();
                    //edit = true;
                    dgvNew.CurrentCell = dgvNew[1, e.RowIndex + 1];
                }

                if (e.ColumnIndex == 4)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value)))
                    {
                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value = 0;
                    }
                }
            }
            catch
            {

            }
        }

        private void dgvNew_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            total();
            if (e.ColumnIndex == 1)
            {
                rdbStartsWith.Checked = true;
                if (dgvNew.ReadOnly == false)
                {
                    pnsearch.Visible = true;
                }
                if (!string.IsNullOrEmpty(lblhiddenproduct.Text))
                {
                    Txtitem.Text = lblhiddenproduct.Text;
                    AutoCompleteLoad(Txtitem.Text, 1);
                    if (DgvAutoRefNo.Rows.Count > 0)
                    {
                        DgvAutoRefNo.Rows[0].Cells[0].Selected = false;
                        DefaultFloor.Text = "0";
                        Display.Text = "0";
                        Damage.Text = "0";
                        Checking.Text = "0";
                        Delivery.Text = "0";
                        lblprice.Text = "0";
                    }


                }
                Txtitem.SelectionStart = 0;
                Txtitem.SelectionLength = Txtitem.Text.Length;

                this.ActiveControl = Txtitem;
                lblrowindex.Text = e.RowIndex.ToString();
                lblpageno.Text = Convert.ToString(Convert.ToInt32(lblpageno.Text) + 1);
            }
            //else
            //{
            //    pnsearch.Visible = false; ;
            //}
        }

        private void dgvNew_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (savevads == false)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value)))
                    {
                        decimal rate = Convert.ToDecimal(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value);
                        //decimal amt = rate * Convert.ToDecimal(tb.Text);

                        decimal amt = rate * Convert.ToDecimal(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value);

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
                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = amt;
                    }


                }
                catch (Exception sa)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[1].Value)))
                    {
                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = 0.00;
                    }
                }
                total();

            }
            else
            {
                savevads = false;
            }
        }

        private void dgvNew_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvNew.CurrentCell.ColumnIndex;
            string headerText = dgvNew.Columns[column].HeaderText;

            if (headerText.Equals("Quantity"))
            {
                tb = e.Control as TextBox;


                if (tb != null)
                {
                    ///tb.TextChanged += new EventHandler(textbox_Change);
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.MaxLength = 10;
                }
            }
            else if (headerText.Equals("Rate"))
            {
                tbrate = e.Control as TextBox;


                if (tbrate != null)
                {
                    tbrate.TextChanged += new EventHandler(textbox_Change);
                    tbrate.KeyPress += new KeyPressEventHandler(txtch);
                }


            }
        }

        private void textbox_Change(object sender, EventArgs e)
        {

            int column = dgvNew.CurrentCell.ColumnIndex;
            string headerText = dgvNew.Columns[column].HeaderText;
            if (headerText.Equals("Rate"))
            {
                if (!string.IsNullOrEmpty(tbrate.Text))
                {
                    if (tbrate.Text.Contains("-"))
                    {
                        tbrate.Text = tbrate.Text.Replace("-", "");
                    }
                }
            }
        }


        public void txtch(object sender, KeyPressEventArgs e)
        {
            //if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            //    e.Handled = true;
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


        }

        Regex reg = new Regex(@"^-?\d+[.]?\d*$");
        Regex reg1 = new Regex(@"^-?[.]?\d*$");
        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
            ////if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            ////    e.Handled = true;
            //if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar))
            //                    && (e.KeyChar != '.') && (e.KeyChar != '-'))
            //    e.Handled = true;


            ////if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            ////{
            ////    e.Handled = true;
            ////}

            //// only allow one decimal point
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = true;
            //}

            //// only allow minus sign at the beginning
            //if (e.KeyChar == '-' && (sender as TextBox).Text.Length > 0)
            //{
            //    e.Handled = true;
            //}
            try
            {
                if (char.IsControl(e.KeyChar)) return;
                if ((reg.IsMatch(tb.Text.Insert(tb.SelectionStart, e.KeyChar.ToString()) + "1")) || reg1.IsMatch(tb.Text.Insert(tb.SelectionStart, e.KeyChar.ToString()) + "1"))
                {

                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {

            }
        }


        private void dgvNew_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                try
                {
                    e.SuppressKeyPress = true;
                    if (dgvNew.CurrentCell.ColumnIndex == 0)
                    {
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[1, dgvNew.CurrentCell.RowIndex];

                    }
                    else if (dgvNew.CurrentCell.ColumnIndex == 1)
                    {
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[2, dgvNew.CurrentCell.RowIndex];

                    }
                    else if (dgvNew.CurrentCell.ColumnIndex == 2)
                    {
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[4, dgvNew.CurrentCell.RowIndex];

                    }
                    else if (dgvNew.CurrentCell.ColumnIndex == 4)
                    {
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[5, dgvNew.CurrentCell.RowIndex];

                    }


                    else if (dgvNew.CurrentCell.ColumnIndex == 6)
                    {
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[1, dgvNew.CurrentCell.RowIndex + 1];

                    }
                }
                catch
                {

                }

            }
        }

        private void dgvNew_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (edit == true)
                {
                    if (dgvNew.CurrentCell.RowIndex >= 1)
                    {
                        dgvNew.CurrentCell = dgvNew[dgvNew.CurrentCell.ColumnIndex, dgvNew.CurrentCell.RowIndex - 1];
                        edit = false;
                    }
                    else if (dgvNew.CurrentCell.RowIndex == 0)
                    {
                        dgvNew.CurrentCell = dgvNew[dgvNew.CurrentCell.ColumnIndex, dgvNew.CurrentCell.RowIndex - 1];
                    }

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

                    totalamount = totalamount + value;
                    //totalquantity = totalquantity + Convert.ToInt32(dgvNew.Rows[i].Cells[5].Value);
                }

                //  totalquantity = Math.Round(totalquantity);

                //totalamount = Math.Round(totalamount);

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


                //lbltotalquantity.Text = Convert.ToString(totalquantity);
                lbltotalamount.Text = String.Format("{0:00.00}", totalamount);
                if (string.IsNullOrEmpty(Txtothers.Text))
                {
                    Txtothers.Text = "0";
                }
                if (string.IsNullOrEmpty(txtless.Text))
                {
                    txtless.Text = "0";
                }
                if (Convert.ToDouble(lbltotalamount.Text) >= 0)
                {
                    totalamount = totalamount + Convert.ToDouble(Txtothers.Text) - Convert.ToDouble(txtless.Text);
                }
                else
                {
                    totalamount = totalamount + Convert.ToDouble(Txtothers.Text) + Convert.ToDouble(txtless.Text);
                }


                lblTotal.Text = String.Format("{0:00.00}", totalamount);
            }
            catch
            {

            }
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

        public void RemoveNullColumnFromDataTable1(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {

                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][1])) || string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][3])))
                    dt.Rows[i].Delete();

            }
            dt.AcceptChanges();
        }

        public void RemoveNullColumnFromDataTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (MainTabSalesBill.SelectedIndex == 0 || MainTabSalesBill.SelectedIndex == 2 || MainTabSalesBill.SelectedIndex == 3 || MainTabSalesBill.SelectedIndex == 6)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][1])) || string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][3])))
                        dt.Rows[i].Delete();
                }
                else if (MainTabSalesBill.SelectedIndex == 1 || MainTabSalesBill.SelectedIndex == 4 || MainTabSalesBill.SelectedIndex == 5)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][1])) || string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][3])) || (Convert.ToDecimal(dt.Rows[i][2]) < 0))
                        dt.Rows[i].Delete();
                }
            }
            dt.AcceptChanges();
        }
        public void save(int v)
        {

         
            // Pnloading.Visible = true;


            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(txtOrderNo.Text))
            {
                objQuotationbal.isnew = 0;
            }
            else
            {
                objQuotationbal.isnew = 1;
            }
            objQuotationbal.Quotationid = tatQuotationno.Text;

            if (Convert.ToString(Txtcustomername.SelectedValue) == "0")
            {
                objQuotationbal.Customerid = null;
            }
            else
            {
                objQuotationbal.Customerid = Convert.ToString(Txtcustomername.SelectedValue);
            }


            if (getreturns() == false)
            {
                objQuotationbal.returns = "1";
            }
            else
            {
                objQuotationbal.returns = "";
            }
            objQuotationbal.date = date1.Value;
            objQuotationbal.Referenceid = Convert.ToString(cmbreference.SelectedValue);
            objQuotationbal.Assist = Convert.ToString(cmbassistby.SelectedValue);
            objQuotationbal.Customername = Convert.ToString(Txtcustomername.Text.Trim());
            objQuotationbal.City = Convert.ToString(cmdcity.Text);
            objQuotationbal.Estinationid = txtOrderNo.Text;
            objQuotationbal.Remarks = txtRemarks.Text;
            objQuotationbal.Total = lbltotalamount.Text;
            objQuotationbal.others = Txtothers.Text;
            if (rbCash.Checked)
            {
                objQuotationbal.Paymentmode = "Cash Bill";
                objQuotationbal.phoneno = TxtMobilenumber.Text;
            }
            else if (rbcredit.Checked)
            {
                objQuotationbal.Paymentmode = "Credit Bill";
                objQuotationbal.phoneno = TxtMobilenumber.Text;

            }
            else if (rbpartial.Checked)
            {
                objQuotationbal.Paymentmode = "Partial Credit Bill";
                objQuotationbal.phoneno = TxtMobilenumber.Text;
            }
            if (!string.IsNullOrEmpty(txtless.Text))
            {
                objQuotationbal.Lessamount = txtless.Text;
            }
            else
            {
                objQuotationbal.Lessamount = "0";
            }

           // objQuotationbal.Grandtotal = Convert.ToString(Convert.ToDouble(lbltotalamount.Text) + Convert.ToDouble(Txtothers.Text) - Convert.ToDouble(objQuotationbal.Lessamount));

            objQuotationbal.Grandtotal = lblTotal.Text;

            if (v == 1)
            {
                objQuotationbal.status = "Open";
            }
            else if (v == 2)
            {
                objQuotationbal.status = "Estimation Completed";
            }
            else if (v == 3)
            {
                objQuotationbal.status = "Return Approval";
            }

            objQuotationbal.cusbool = cusbool;
            objQuotationbal.Updatedby = Program.userid;
            dt = DataGridView2DataTable(dgvNew);
            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            RemoveNullColumnFromDataTable(dt);

              bool dtval = RemoveDuplicateRows(dt, "ProductId");


              if (dtval)
              {


                  string[] output = objQuotationbal.SaveEstimation(objQuotationbal, dt);


                  if (output[1] == "3")
                  {

                      MessageBox.Show("Phone No Already Exist");
                      cusbool = 0;
                      panel11.Visible = true;
                      TxtMobilenumber.Focus();

                  }
                  else
                  {

                      if (!string.IsNullOrEmpty(output[0]) && string.IsNullOrEmpty(txtOrderNo.Text))
                      {

                          txtOrderNo.Text = output[0];
                          clear();
                          if (v == 2)
                          {

                          }
                      }
                      else if (!string.IsNullOrEmpty(output[0]) && !string.IsNullOrEmpty(txtOrderNo.Text))
                      {

                          clear();

                          txtOrderNo.Text = output[0];
                          if (v == 2)
                          {

                          }
                      }
                  }
              }

              else
              {
                  MessageBox.Show("Please Remove Duplication Product");
                  panel1.Enabled = true;
                  btnSavePending.Enabled = true;
                  btnsave.Enabled = true;
                  btnPrint.Enabled = true;

                  cmbcustomername.Enabled = true;
                  cmdcity.Enabled = true;
                  cmbreference.Enabled = true;
                  cmbassistby.Enabled = true;
                  dgvNew.ReadOnly = false;
                  this.dgvNew.Columns["Rate"].ReadOnly = true;
                  this.dgvNew.Columns[0].ReadOnly = true;
                  this.dgvNew.Columns["Items"].ReadOnly = true;
                  this.dgvNew.Columns["UOM"].ReadOnly = true;
                  this.dgvNew.Columns["Amount"].ReadOnly = true;
              }
          
        }


        public bool RemoveDuplicateRows(DataTable dTable, string colName)
        {
            bool sa = true;
            int index = 0;
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
            {
                index = dTable.Rows.IndexOf(dRow);
                dgvNew.Rows[index].DefaultCellStyle.ForeColor = Color.Red;
                sa = false;
            }
            //dTable.Rows.Remove(dRow);


            //Datatable which contains unique records will be return as output.
            return sa;
        }
        private void txtless_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //if (!string.IsNullOrEmpty(txtless.Text))
                //{
                if (Cmbless.SelectedIndex == 0)
                {


                    double total = Convert.ToDouble(lbltotalamount.Text);
                    if (string.IsNullOrEmpty(Convert.ToString(txtless.Text)))
                    {
                        txtless.Text = "0";

                        txtless.SelectionStart = 0;
                        txtless.SelectionLength = txtless.Text.Length;
                    }
                    if (string.IsNullOrEmpty(Convert.ToString(Txtothers.Text)))
                    {
                        Txtothers.Text = "0";
                        Txtothers.SelectionStart = 0;
                        Txtothers.SelectionLength = Txtothers.Text.Length;
                    }
                    double less = Convert.ToDouble(txtless.Text);
                    double others = Convert.ToDouble(Txtothers.Text);
                    double grandtotal = total + others - less;
                    lblTotal.Text = String.Format("{0:00.00}", grandtotal);
                }
                else
                {
                    double total = Convert.ToDouble(lbltotalamount.Text);
                    if (string.IsNullOrEmpty(Convert.ToString(txtless.Text)))
                    {
                        txtless.Text = "0";

                        txtless.SelectionStart = 0;
                        txtless.SelectionLength = txtless.Text.Length;
                    }
                    if (string.IsNullOrEmpty(Convert.ToString(Txtothers.Text)))
                    {
                        Txtothers.Text = "0";
                        Txtothers.SelectionStart = 0;
                        Txtothers.SelectionLength = Txtothers.Text.Length;
                    }
                    double less = Convert.ToDouble(txtless.Text);
                    double others = Convert.ToDouble(Txtothers.Text);
                    double grandtotal = total + others + less;
                    lblTotal.Text = String.Format("{0:00.00}", grandtotal);
                }



                //}
                //else
                //{
                //    lblTotal.Text = String.Format("{0:00.00}", lbltotalamount.Text);
                //}
            }
            catch
            {

            }

        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            if ((cbxSearchOrderNo.SelectedIndex == cbxSearchOrderDate.SelectedIndex) || cbxSearchOrderNo.SelectedIndex == cbxVendor.SelectedIndex || cbxSearchOrderDate.SelectedIndex == cbxVendor.SelectedIndex)
            {
                MessageBox.Show("* Search a item Should Not Be Same");
            }
            else
            {
                string firstname = string.Empty, firstvalue = string.Empty, secondname = string.Empty, secondvalue = string.Empty, thirdname = string.Empty, thirdvalue = string.Empty;
                string firstname1 = string.Empty, firstvalue1 = string.Empty, secondname1 = string.Empty, secondvalue1 = string.Empty, thirdname1 = string.Empty, thirdvalue1 = string.Empty;

                firstname = cbxSearchOrderNo.Text.Trim();
                if (firstname == "Order Number")
                {
                    firstname = "Order Number";
                    //firstvalue = Convert.ToString(cmbstatus1.SelectedValue);
                    if (cmbstatus1.SelectedIndex > 0)
                    {
                        firstvalue = cmbstatus1.Text;
                    }
                    else
                    {
                        firstvalue = "";
                    }
                }
                else
                {
                    //firstvalue = txtsearch1.Text.Trim();
                    string part1 = string.Empty;
                    //secondvalue = txtsearch2.Text.Trim();
                    if (txtsearch1.Text.Trim() != "ALL")
                    {
                        part1 = txtsearch1.Text.Trim();
                    }
                    else
                    {
                        part1 = string.Empty;
                        firstvalue = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(part1))
                    {
                        string part = part1.Substring(0, 2);
                        if (Char.IsDigit(part, 0))
                        {
                            string date = txtsearch1.Text;
                            DateTime dt = Convert.ToDateTime(date);
                            string formatted = dt.ToString("dd-MM-yyyy");
                            string[] rr = formatted.Split('-');
                            //  string[] rr = txtsearch1.Text.Split('-');
                            firstvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                        }
                        else
                        {
                            firstvalue = txtsearch1.Text.Trim();
                        }
                    }
                }

                secondname = cbxSearchOrderDate.Text.Trim();
                if (secondname == "Order Number")
                {
                    secondname = "Order Number";
                    //secondvalue = Convert.ToString(cmbstatus2.SelectedValue);
                    if (cmbstatus2.SelectedIndex > 0)
                    {
                        secondvalue = cmbstatus1.Text;
                    }
                    else
                    {
                        secondvalue = "";
                    }



                }
                else
                {
                    string part1 = string.Empty;
                    //secondvalue = txtsearch2.Text.Trim();
                    if (txtsearch2.Text.Trim() != "ALL")
                    {
                        part1 = txtsearch2.Text.Trim();
                    }
                    else
                    {
                        part1 = string.Empty;
                        secondvalue = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(part1))
                    {
                        string part = part1.Substring(0, 2);
                        if (Char.IsDigit(part, 0))
                        {
                            string date = txtsearch2.Text;
                            DateTime dt = Convert.ToDateTime(date);
                            string formatted = dt.ToString("dd-MM-yyyy");
                            string[] rr = formatted.Split('-');
                            // string[] rr = txtsearch2.Text.Split('-');
                            secondvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                        }
                        else
                        {
                            secondvalue = txtsearch2.Text.Trim();
                        }
                    }
                }


                thirdname = cbxVendor.Text.Trim();
                if (thirdname == "Order Number")
                {
                    thirdname = "Order Number";

                    if (cmbstatus3.SelectedIndex > 0)
                    {
                        thirdvalue = cmbstatus3.Text;
                    }
                    else
                    {
                        thirdvalue = "";
                    }


                }
                else
                {
                    //thirdvalue = txtsearch3.Text.Trim();
                    string part1 = string.Empty;
                    //secondvalue = txtsearch2.Text.Trim();
                    if (txtsearch3.Text.Trim() != "ALL")
                    {
                        part1 = txtsearch3.Text.Trim();
                    }
                    else
                    {
                        part1 = string.Empty;
                        thirdvalue = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(part1))
                    {
                        string part = part1.Substring(0, 2);
                        if (Char.IsDigit(part, 0))
                        {
                            string date = txtsearch3.Text;
                            DateTime dt = Convert.ToDateTime(date);
                            string formatted = dt.ToString("dd-MM-yyyy");
                            string[] rr = formatted.Split('-');

                            // string[] rr = txtsearch3.Text.Split('-');
                            thirdvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                        }
                        else
                        {
                            thirdvalue = txtsearch3.Text.Trim();
                        }
                    }
                }

                if (firstname == "Customer")
                {
                    firstname1 = "customername";
                    firstvalue1 = firstvalue;
                }
                else if (firstname == "Order Date")
                {
                    secondname1 = "date";
                    secondvalue1 = firstvalue;
                }
                else if (firstname == "Order Number")
                {
                    if (selectedtab == "TabNew")

                        thirdname1 = "Quotationid";

                    else if (selectedtab == "TabFloorCheckOut")

                        thirdname1 = "Estimationid";
                    thirdvalue1 = firstvalue;
                }

                if (secondname == "Customer")
                {
                    firstname1 = "customername";
                    firstvalue1 = secondvalue;
                }
                else if (secondname == "Order Date")
                {
                    secondname1 = "date";
                    secondvalue1 = secondvalue;
                }
                else if (secondname == "Order Number")
                {
                    if (selectedtab == "TabNew")

                        thirdname1 = "Quotationid";

                    else if (selectedtab == "TabFloorCheckOut")

                        thirdname1 = "Estimationid";

                    thirdvalue1 = secondvalue;
                }

                if (thirdname == "Customer")
                {
                    firstname1 = "customername";
                    firstvalue1 = thirdvalue;
                }
                else if (thirdname == "Order Date")
                {
                    secondname1 = "date";
                    secondvalue1 = thirdvalue;
                }
                else if (thirdname == "Order Number")
                {
                    if (selectedtab == "TabNew")

                        thirdname1 = "Quotationid";

                    else
                        thirdname1 = "Estimationid";


                    thirdvalue1 = thirdvalue;
                }


                //if (selectedtab == "TabNew")
                //{
                //    search(firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                //}
                else if (selectedtab == "TabFloorCheckOut")
                {
                    searchcheckout(firstname1, firstvalue1, "Updatedon", secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TabPaymentReceived")
                {
                    searchPaymentmode(firstname1, firstvalue1, "Updatedon", secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TabCreditApproval")
                {
                    searchcreditapproved(firstname1, firstvalue1, "Updatedon", secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TabPayment")
                {
                    searchpay(firstname1, firstvalue1, "Updatedon", secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TabChecking")
                {
                    searchcheck(firstname1, firstvalue1, "Updatedon", secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TabDelivery")
                {
                    searchdelivery(firstname1, firstvalue1, "Updatedon", secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TapReturnApproval")
                {
                    searchReturnapproved(firstname1, firstvalue1, "Updatedon", secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TapReturnCheckin")
                {
                    searchreturncheck(firstname1, firstvalue1, "Updatedon", secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }
            }
        }

        //public void getReturnApproval(string s)
        // {
        //     DataSet ds = objQuotationbal.Bindbillpay(s);
        //     if (ds.Tables[0].Rows.Count > 0)
        //     {


        //         txtReturnapprovalCustomerName.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
        //         txtReturnapprovalCity.Text = Convert.ToString(ds.Tables[0].Rows[0]["City"]);
        //         txtReturnapprovalReference.Text = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);

        //         txtReturnapprovalEstNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
        //         txtReturnapprovalPreparedBy.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);
        //         txtReturnapprovalAssistBy.Text = Convert.ToString(ds.Tables[0].Rows[0]["UserFullName"]);

        //         txtReturnapprovalDate.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);
        //         txtReturnapprovalQuotationNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);

        //         txtReturnapprovalRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
        //         txtReturnapprovalTotal.Text = Convert.ToString(ds.Tables[0].Rows[0]["Total"]);
        //         txtReturnapprovalLess.Text = Convert.ToString(ds.Tables[0].Rows[0]["LessAmount"]);
        //         txtReturnapprovalNet.Text = Convert.ToString(ds.Tables[0].Rows[0]["GrnandTotal"]);



        //         //  panel2.Enabled = false;
        //     }
        //     else
        //     {
        //         //panel2.Enabled = true;
        //         clear();
        //     }
        //     if (ds.Tables[1].Rows.Count > 0)
        //     {




        //         dgvReturnApproval.Rows.Clear();
        //         dgvReturnApproval.Enabled = false;
        //         for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
        //         {
        //             dgvReturnApproval.Rows.Add();
        //             dgvReturnApproval.Rows[i].Cells[0].Value = i + 1;
        //             dgvReturnApproval.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
        //             dgvReturnApproval.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
        //             dgvReturnApproval.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

        //             double qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
        //             dgvReturnApproval.Rows[i].Cells[4].Value = qty;
        //             dgvReturnApproval.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);

        //             double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
        //             dgvReturnApproval.Rows[i].Cells[6].Value = amt;


        //             //dgvPaymentReceived.Rows.Add();
        //             //dgvPaymentReceived.Rows[i].Cells[0].Value = i + 1;
        //             //dgvPaymentReceived.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["ItemName"]);
        //             //dgvPaymentReceived.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
        //             //dgvPaymentReceived.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
        //             //dgvPaymentReceived.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
        //         }
        //         // panel2.Enabled = false;
        //     }
        //     else
        //     {
        //         dgvReturnApproval.Rows.Clear();
        //         //panel2.Enabled = true;
        //     }
        // }
        public void searchReturnapproved(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchReturnapproved(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;


            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }
        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchestimation(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i][2]);
            //    dgvSearch.Rows[i].Cells[4].Value = Convert.ToString(dt.Rows[i][2]);

            //    //dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i][2]);
            //}
            //dgvSearch.Columns[1].Visible = false;

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void cbxSearchOrderNo_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void ListSearchDate1_Click_1(object sender, EventArgs e)
        {

        }

        private void pbxRightCollapse_Click(object sender, EventArgs e)
        {
            if (pnlCollapse2.Visible == true)
            {
                pnlOrder.Visible = true;
                vLabel5.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;
                //this.dgvSearch.Columns[1].Visible = true;
                //this.dgvSearch.Columns[2].Visible = true;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
            lblproductid.Text = string.Empty;
            // Txtitem.Text = string.Empty;
            lblitemcode.Text = "0";
            lblprice.Text = "0";
            lbldisplay.Text = "0";
            lbldemo.Text = "0";
            lblservice.Text = "0";
            lbldamage.Text = "0";
            lblrack.Text = "0";
        }

        public void getEstimation(string s)
        {
            DataSet ds = objQuotationbal.getEstimation(s);
            if (ds.Tables[0].Rows.Count > 0)
            {

                this.dgvNew.Columns["Amount"].ReadOnly = true;
                this.dgvNew.Columns[0].ReadOnly = true;
                this.dgvNew.Columns["Items"].ReadOnly = true;
                this.dgvNew.Columns["UOM"].ReadOnly = true;
                this.dgvNew.Columns["productid"].ReadOnly = true;
                this.dgvNew.Columns["Rate"].ReadOnly = true;

                panel9.Enabled = true;
                txtOrderNo.Enabled = false;
                Txtcustomername.Enabled = false;
                cmdcity.Enabled = false;
                cmbreference.Enabled = false;
                cmbassistby.Enabled = false;
                tatQuotationno.Enabled = false;
                date1.Enabled = false;

                // txtOrderNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
                Txtcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                cmdcity.Text = Convert.ToString(ds.Tables[0].Rows[0]["city"]);
                cmbreference.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Referenceid"]);
                cmbassistby.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Assist"]);
                lblperare.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);
                tatQuotationno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);
                date1.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);

                txtless.Text = Convert.ToString(ds.Tables[0].Rows[0]["LessAmount"]);
                lblTotal.Text = Convert.ToString(ds.Tables[0].Rows[0]["GrnandTotal"]);

                txtRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                panel2.Enabled = false;
               groupBox5.Enabled = true;
            }
            else
            {
                panel2.Enabled = true;
                // dgvNew.Enabled = false;
               // groupBox5.Enabled = false;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                // dgvNew.Enabled = false;
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
                }
                panel2.Enabled = false;
            }
            else
            {
                // dgvNew.Enabled = false;
                dgvNew.Rows.Clear();
                panel2.Enabled = true;
                // dgvNew.Enabled = false;
            }

        }

        public void getCheckout(string s)
        {
            DataSet ds = objQuotationbal.bindcheckoutBylocation(s, Program.Floor);
            if (ds.Tables[0].Rows.Count > 0)
            {
                txtcheckorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
                txtcheckcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                Txtcheckcity.Text = Convert.ToString(ds.Tables[0].Rows[0]["City"]);
                txtcheckreference.Text = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);

                txtcheckprepareby.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);

                txtcheckassistedby.Text = Convert.ToString(ds.Tables[0].Rows[0]["UserFullName"]);

                checkdate.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);

                txtcheckquotationid.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);

                txtcheckremarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);





                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                dgvFloorCheckOut.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {



                    dgvFloorCheckOut.Rows.Add();
                    dgvFloorCheckOut.Rows[i].Cells[0].Value = i + 1;
                    dgvFloorCheckOut.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvFloorCheckOut.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    //dgvFloorCheckOut.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
                    dgvFloorCheckOut.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    //dgvFloorCheckOut.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["LocationID"]);
                    if (Convert.ToDecimal(dgvFloorCheckOut.Rows[i].Cells[2].Value) < 0)
                    {
                        dgvFloorCheckOut.Rows[i].ReadOnly = true;
                        dgvFloorCheckOut.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    }


                    string vals = Convert.ToString(ds.Tables[1].Rows[i]["Location"]);
                    DataTable dt = getdatatable(vals);

                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).DataSource = dt;
                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).ValueMember = "id";
                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).DisplayMember = "location";
                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).Value = dt.Rows[0][0];
                    string val = Convert.ToString(dt.Rows[0][0]);
                    string val1 = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    string v = getrack(val, val1);
                    dgvFloorCheckOut.Rows[i].Cells["Rack"].Value = v;
                }
                panel2.Enabled = false;
            }
            else
            {
                dgvFloorCheckOut.Rows.Clear();
                panel2.Enabled = true;
            }

        }



        public void getcreditapproval(string s)
        {
            DataSet ds = objQuotationbal.Bindbillpay(s);
            if (ds.Tables[0].Rows.Count > 0)
            {

                txtcreditorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
                creditcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                txtcreditcity.Text = Convert.ToString(ds.Tables[0].Rows[0]["City"]);
                txtrcrediteference.Text = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);

                txtpreperedby.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);

                Txtassistby.Text = Convert.ToString(ds.Tables[0].Rows[0]["UserFullName"]);

                creditdate.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);

                Txtcreditquotationno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);

                txtcreditremarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                lblcredittotal.Text = Convert.ToString(ds.Tables[0].Rows[0]["Total"]);
                txtcreditless.Text = Convert.ToString(ds.Tables[0].Rows[0]["LessAmount"]);
                lblcreditgranttotal.Text = Convert.ToString(ds.Tables[0].Rows[0]["GrnandTotal"]);

                lblcreditothers.Text = Convert.ToString(ds.Tables[0].Rows[0]["others"]);

                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {




                dgvCreditApproval.Rows.Clear();
                dgvCreditApproval.Enabled = false;
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvCreditApproval.Rows.Add();
                    dgvCreditApproval.Rows[i].Cells[0].Value = i + 1;
                    dgvCreditApproval.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvCreditApproval.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvCreditApproval.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

                    double qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    dgvCreditApproval.Rows[i].Cells[4].Value = qty;
                    dgvCreditApproval.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);


                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvCreditApproval.Rows[i].Cells[6].Value = amt;


                    //dgvPaymentReceived.Rows.Add();
                    //dgvPaymentReceived.Rows[i].Cells[0].Value = i + 1;
                    //dgvPaymentReceived.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["ItemName"]);
                    //dgvPaymentReceived.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    //dgvPaymentReceived.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
                    //dgvPaymentReceived.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                }
                panel2.Enabled = false;
            }
            else
            {
                dgvCreditApproval.Rows.Clear();
                panel2.Enabled = true;
            }

        }

        public void getReturnApproval(string s)
        {
            DataSet ds = objQuotationbal.Bindbillpay(s);
            if (ds.Tables[0].Rows.Count > 0)
            {


                txtReturnapprovalCustomerName.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                txtReturnapprovalCity.Text = Convert.ToString(ds.Tables[0].Rows[0]["City"]);
                txtReturnapprovalReference.Text = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);

                txtReturnapprovalEstNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
                txtReturnapprovalPreparedBy.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);
                txtReturnapprovalAssistBy.Text = Convert.ToString(ds.Tables[0].Rows[0]["UserFullName"]);

                txtReturnapprovalDate.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);
                txtReturnapprovalQuotationNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);

                txtReturnapprovalRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                txtReturnapprovalTotal.Text = Convert.ToString(ds.Tables[0].Rows[0]["Total"]);
                txtReturnapprovalLess.Text = Convert.ToString(ds.Tables[0].Rows[0]["LessAmount"]);
                txtReturnapprovalNet.Text = Convert.ToString(ds.Tables[0].Rows[0]["GrnandTotal"]);
                lblretuenothers.Text = Convert.ToString(ds.Tables[0].Rows[0]["others"]);



                //  panel2.Enabled = false;
            }
            else
            {
                //panel2.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {




                dgvReturnApproval.Rows.Clear();
                dgvReturnApproval.Enabled = false;
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvReturnApproval.Rows.Add();
                    dgvReturnApproval.Rows[i].Cells[0].Value = i + 1;
                    dgvReturnApproval.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvReturnApproval.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvReturnApproval.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

                    double qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    dgvReturnApproval.Rows[i].Cells[4].Value = qty;
                    dgvReturnApproval.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);

                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvReturnApproval.Rows[i].Cells[6].Value = amt;


                    //dgvPaymentReceived.Rows.Add();
                    //dgvPaymentReceived.Rows[i].Cells[0].Value = i + 1;
                    //dgvPaymentReceived.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["ItemName"]);
                    //dgvPaymentReceived.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    //dgvPaymentReceived.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
                    //dgvPaymentReceived.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                }
                // panel2.Enabled = false;
            }
            else
            {
                dgvReturnApproval.Rows.Clear();
                //panel2.Enabled = true;
            }
        }


        public void getpay(string s)
        {
            DataSet ds = objQuotationbal.Bindbillpay(s);

            if (ds.Tables[0].Rows.Count > 0)
            {
                ddlpaymode.Enabled = true;
                rbpaymentCash.Enabled = true;
                RbPaymentCard.Enabled = true;
                Rdcashcard.Enabled = true;
                txtpayorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
                txtpauycustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                txtpaycity.Text = Convert.ToString(ds.Tables[0].Rows[0]["City"]);
                txtpayref.Text = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);

                Txtprepareby.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);

                txtpayassist.Text = Convert.ToString(ds.Tables[0].Rows[0]["UserFullName"]);

                paydate.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);

                lblpaymentmode.Text = Convert.ToString(ds.Tables[0].Rows[0]["Paymentmode"]);
                Txtpayquotationid.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);

                txtpayremarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                lblpaytotal.Text = Convert.ToString(ds.Tables[0].Rows[0]["Total"]);
                txtpayless.Text = Convert.ToString(ds.Tables[0].Rows[0]["LessAmount"]);
                lblpaymentothers.Text = Convert.ToString(ds.Tables[0].Rows[0]["others"]);
                lblpaynet.Text = Convert.ToString(Convert.ToDouble(lblpaytotal.Text) + Convert.ToDouble(lblpaymentothers.Text) - Convert.ToDouble(txtpayless.Text));

                double value = Math.Round(Convert.ToDouble(lblpaynet.Text));
                lblpatroundoff.Text = Convert.ToString(value);
                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                ddlpaymode.Enabled = true;
                rbpaymentCash.Enabled = true;
                RbPaymentCard.Enabled = true;
                Rdcashcard.Enabled = true;
                dgvPayment.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvPayment.Rows.Add();
                    dgvPayment.Rows[i].Cells[0].Value = i + 1;
                    dgvPayment.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvPayment.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvPayment.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

                    double qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    dgvPayment.Rows[i].Cells[4].Value = qty;
                    dgvPayment.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);

                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvPayment.Rows[i].Cells[6].Value = amt;

                    groupBox3.Enabled = true;
                    //dgvPaymentReceived.Rows.Add();
                    //dgvPaymentReceived.Rows[i].Cells[0].Value = i + 1;
                    //dgvPaymentReceived.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["ItemName"]);
                    //dgvPaymentReceived.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    //dgvPaymentReceived.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
                    //dgvPaymentReceived.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                }
                panel2.Enabled = false;
            }
            else
            {
                dgvPayment.Rows.Clear();
                panel2.Enabled = true;
            }

        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (selectedtab == "TabNew")
                {
                    if (e.RowIndex >= 0)
                    {
                        string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                        if (!string.IsNullOrEmpty(s))
                        {
                            lblless.Visible = false;
                            txtless.Visible = false;
                            Cmbless.Visible = false;
                            rbCash.Checked = false;
                            rbcredit.Checked = false;
                            rbpartial.Checked = false;
                            button6.Enabled = false;
                            //pnlnet.Visible = false;
                            //lblTotal.Visible = false;

                            getEstimation(s);
                            total();
                            groupBox5.Focus();

                        }
                        else
                        {
                            clear();
                        }

                    }
                }
                else if (selectedtab == "TabFloorCheckOut")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getCheckout(s);
                        dgvFloorCheckOut.Focus();
                        if (dgvFloorCheckOut.Rows.Count > 0)
                        {
                            dgvFloorCheckOut.CurrentCell = dgvFloorCheckOut[6, 0];
                        }
                    }
                    else
                    {
                        clear();
                    }


                }
                else if (selectedtab == "TapReturnApproval")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        // getcreditapproval(s);
                        getReturnApproval(s);
                        this.ActiveControl = btnApproved;
                    }
                    else
                    {
                        clear();
                    }


                }

                else if (selectedtab == "TabCreditApproval")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getcreditapproval(s);
                        this.ActiveControl = btnApproved;
                    }
                    else
                    {
                        clear();
                    }


                }
                else if (selectedtab == "TabPayment")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getpay(s);
                        //RbPaymentCard.Focus();
                        // ddlpaymode.Focus();
                    }
                    else
                    {
                        clear();
                    }


                }
                else if (selectedtab == "TabChecking")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getcheck(s);
                        dgvChecking.Focus();
                        dgvChecking.CurrentCell = dgvChecking[3, 0];
                    }
                    else
                    {
                        clear();
                    }


                }

                else if (selectedtab == "TabDelivery")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getDelivery(s);
                        dgvDelivery.Focus();
                        dgvDelivery.CurrentCell = dgvDelivery[4, 0];
                    }
                    else
                    {
                        clear();
                    }


                }

                else if (selectedtab == "TabReturnApproval")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getDelivery(s);
                        dgvReturnApproval.Focus();
                        dgvReturnApproval.CurrentCell = dgvReturnApproval[4, 0];
                    }
                    else
                    {
                        clear();
                    }


                }


                else if (selectedtab == "TapReturnCheckin")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getreturncheck(s);
                        //dgvChecking.Focus();
                        //dgvChecking.CurrentCell = dgvChecking[3, 0];
                    }
                    else
                    {
                        clear();
                    }


                }

            }
        }


        public void getreturncheck(string s)
        {
            DataSet ds = objQuotationbal.bindreturncheckin(s,Program.Floor);
            if (ds.Tables[0].Rows.Count > 0)
            {
                txtckinestno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
                txtCkinCusname.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                txtckincity.Text = Convert.ToString(ds.Tables[0].Rows[0]["City"]);
                txtchkinref.Text = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);

                txtckinpreparedby.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);

                txtckinassistby.Text = Convert.ToString(ds.Tables[0].Rows[0]["UserFullName"]);

                dtcheckin.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);

                txtckInQuotNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);

                txtckinRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);



                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                DgvCkin.Rows.Clear();
                //if (srole == "Admin")
                //{
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        DgvCkin.Rows.Add();
                        DgvCkin.Rows[i].Cells[0].Value = i + 1;
                        DgvCkin.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                        DgvCkin.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                        //dgvFloorCheckOut.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
                        DgvCkin.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                        //dgvFloorCheckOut.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["LocationID"]);
                        if (Convert.ToDecimal(DgvCkin.Rows[i].Cells[2].Value) > 0)
                        {
                            DgvCkin.Rows[i].ReadOnly = true;
                            DgvCkin.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                        }


                        string vals = Convert.ToString(ds.Tables[1].Rows[i]["Location"]);
                        DataTable dt = getdatatable(vals);

                        (DgvCkin.Rows[i].Cells[3] as DataGridViewComboBoxCell).DataSource = dt;
                        (DgvCkin.Rows[i].Cells[3] as DataGridViewComboBoxCell).ValueMember = "id";
                        (DgvCkin.Rows[i].Cells[3] as DataGridViewComboBoxCell).DisplayMember = "location";
                        (DgvCkin.Rows[i].Cells[3] as DataGridViewComboBoxCell).Value = dt.Rows[0][0];
                        string val = Convert.ToString(dt.Rows[0][0]);
                        string val1 = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                        string v = getrack(val, val1);
                        DgvCkin.Rows[i].Cells["Rack"].Value = v;
                    }
                    panel2.Enabled = false;
                //}

                //else
                //{
                //    if (!string.IsNullOrEmpty(Program.Floor))
                //    {
                //        int flr = Convert.ToInt32(Program.Floor);
                //        int k = 0;
                //        DataTable dtfloor = ObjPurchaseReceiptBAL.GetProductByFloor(flr, s);

                //        for (int i = 0; i < dtfloor.Rows.Count; i++)
                //        {
                //            string prodid = Convert.ToString(dtfloor.Rows[i]["Productid"]);
                //            int Location = Convert.ToInt32(dtfloor.Rows[i]["Location"]);
                //            for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                //            {
                //                string prodid1 = Convert.ToString(ds.Tables[1].Rows[j]["Productid"]);

                //                if (prodid == prodid1)
                //                {
                //                    if (Location == 0)
                //                    {
                //                        string vals = Convert.ToString(ds.Tables[1].Rows[i]["Location"]);
                //                        DataTable dt = getdatatable(vals);

                //                        (DgvCkin.Rows[i].Cells[3] as DataGridViewComboBoxCell).DataSource = dt;
                //                        (DgvCkin.Rows[i].Cells[3] as DataGridViewComboBoxCell).ValueMember = "id";
                //                        (DgvCkin.Rows[i].Cells[3] as DataGridViewComboBoxCell).DisplayMember = "location";
                //                        (DgvCkin.Rows[i].Cells[3] as DataGridViewComboBoxCell).Value = dt.Rows[0][0];
                //                        string val = Convert.ToString(dt.Rows[0][0]);
                //                        string val1 = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                //                        string v = getrack(val, val1);
                //                        DgvCkin.Rows[i].Cells["Rack"].Value = v;
                //                        k++;
                //                    }
                //                    else
                //                    {
                //                        txtckinestno.Text = string.Empty;
                //                        txtCkinCusname.Text = string.Empty;
                //                        txtckincity.Text = string.Empty;
                //                        txtchkinref.Text = string.Empty;

                //                        txtckinpreparedby.Text = string.Empty;
                //                        txtckinassistby.Text = string.Empty;
                //                        dtcheckin.Value = DateTime.Now.Date;
                //                        txtckInQuotNo.Text = string.Empty;
                //                        txtckinRemarks.Text = string.Empty;



                //                        //MessageBox.Show("FloorCheckIn Completed");
                //                        break;
                //                    }
                //                }
                //            }
                //        }

                //        if (dtfloor.Rows.Count == 0)
                //        {
                //            txtckinestno.Text = string.Empty;
                //            txtCkinCusname.Text = string.Empty;
                //            txtckincity.Text = string.Empty;
                //            txtchkinref.Text = string.Empty;

                //            txtckinpreparedby.Text = string.Empty;
                //            txtckinassistby.Text = string.Empty;
                //            dtcheckin.Value = DateTime.Now.Date;
                //            txtckInQuotNo.Text = string.Empty;
                //            txtckinRemarks.Text = string.Empty;
                //        }
                //    }
                //}

            }
            else
            {
                DgvCkin.Rows.Clear();
                panel2.Enabled = true;
            }

        }

        private void Btnsubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Txtitem.Text))
            {
                int rowindex = Convert.ToInt32(lblrowindex.Text);

                if (selectedtab == "TabNew")
                {
                    dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
                    dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    dgvNew.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                    dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                    dgvNew.Rows[rowindex].Cells[4].Value = lblprice.Text;
                    dgvNew.Rows[rowindex].Cells[0].Value = rowindex + 1;
                    pnsearch.Visible = false;
                    DgvAutoRefNo.Visible = false;

                    lblproductid.Text = string.Empty;
                    // Txtitem.Text = string.Empty;
                    lblitemcode.Text = "0";
                    lblprice.Text = "0";
                    lbldisplay.Text = "0";
                    lbldemo.Text = "0";
                    lblservice.Text = "0";
                    lbldamage.Text = "0";
                    lblrack.Text = "0";
                    dgvNew.Focus();
                    dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
                }
            }
            else
            {
                MessageBox.Show("Please Enter Product Name");
                Txtitem.Focus();
            }

        }

        public void bindLocation()
        {
            cmbloaction.DataSource = objQuotationbal.getLocation();
            cmbloaction.DisplayMember = "LocationName";
            cmbloaction.ValueMember = "LocationID";
        }

        public void itemdetailsval(string s)
        {

            try
            {
                dtitems = new DataTable();
                string s1 = s.Trim();
                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s1.Replace("'", "''");


                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                dtitems = objQuotationbal.itemdetails(name, s2);
                Program.dtitems = dtitems;


            }
            catch (Exception e)
            {

            }

        }


        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            ProdSelRowvalue = 0;
            //itemdetails();
        }

        public void searchcheckout(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchCheckout(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //    dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i][3]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }


        public void searchPaymentmode(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchPaymentmode(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        public void savecheck(DataTable dt,string userid)
        {


            string output = objQuotationbal.SaveCheckout(txtcheckorderno.Text, dt, txtcheckremarks.Text, userid);
            if (output == "1")
            {
                //Pnloading1.Visible = false;
                clear();

            }


        }

        public void savechecking(DataTable dt)
        {

            // Pnloading5.Visible = true;
            string output = objQuotationbal.savechecking(txtcheckingorderno.Text, Program.userid, dt);
            if (output == "1")
            {

                GetReport(txtcheckingorderno.Text);
                //if (!string.IsNullOrEmpty(txtcheckingorderno.Text))
                //{
                //    DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //    if (result == DialogResult.Yes)
                //    {
                //        QuotationEstimationreport rpt = new QuotationEstimationreport(txtcheckingorderno.Text);
                //        rpt.ShowDialog();
                //    }
                //}
                //else
                //{
                //    MessageBox.Show("Please Select Estimation ");
                //}

                // Pnloading5.Visible = false;
                clear();

            }


        }

        private void Txtitem_KeyDown_1(object sender, KeyEventArgs e)
        {


            //if (!string.IsNullOrEmpty(Txtitem.Text))
            //{
            //    if (Convert.ToInt32(lblproductid.Text) != 0)
            //    {
            //        int rowindex = Convert.ToInt32(lblrowindex.Text);

            //        if (selectedtab == "TabNew")
            //        {
            //            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
            //            dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
            //            dgvNew.Rows[rowindex].Cells[1].Value = cas.ToUpper();
            //            dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;

            //            dgvNew.Rows[rowindex].Cells[5].ReadOnly = false;

            //            double val = Convert.ToDouble(lblprice.Text);

            //            dgvNew.Rows[rowindex].Cells[4].Value = val;
            //            dgvNew.Rows[rowindex].Cells[0].Value = rowindex + 1;
            //            pnsearch.Visible = false;
            //            DgvAutoRefNo.Visible = false;

            //            lblproductid.Text = string.Empty;
            //            Txtitem.Text = string.Empty;
            //            lblitemcode.Text = "0";
            //            lblprice.Text = "0";
            //            lbldisplay.Text = "0";
            //            lbldemo.Text = "0";
            //            lblservice.Text = "0";
            //            lbldamage.Text = "0";
            //            lblrack.Text = "0";
            //            dgvNew.Focus();
            //            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please Enter Correct Product Name");
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please Enter Product Name");
            //    Txtitem.Focus();
            //}
        }

        public void searchcreditapproved(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchcreditapproved(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;

            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        public void savecreditapproval(string s)
        {

            // Pnloading3.Visible = true;
            string output = objQuotationbal.savecreditapproval(s, txtcreditorderno.Text);
            if (output == "1")
            {
                // Pnloading3.Visible = false;
                clear();

            }


        }

        private void btnApproved_Click(object sender, EventArgs e)
        {
            bool vali = Validationcreditapproval();
            if (vali)
            {
                savecreditapproval("Approved");
                searchcreditapproved("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
            }
            else
            {
                MessageBox.Show("Please Select Item");
            }



        }

        private void btnRejected_Click(object sender, EventArgs e)
        {
            bool vali = Validationcreditapproval();
            if (vali)
            {
                savecreditapproval("Rejected");
                searchcreditapproved("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
            }
            else
            {
                MessageBox.Show("Please Select Item");
            }

        }

        public void searchpay(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchpay(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        public void Application_Idle(object sender, EventArgs e)
        {
            try
            {
                if (cashddl.Focused)
                {
                    cashddl.CurrentCell = cashddl[1, cashddl.CurrentCell.RowIndex];
                }
                else if (dgvCustomerpaid.Focused)
                {
                    dgvCustomerpaid.CurrentCell = dgvCustomerpaid[1, dgvCustomerpaid.CurrentCell.RowIndex];
                }
            }
            catch { }
        }

        private void cashddl_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = cashddl.CurrentCell.ColumnIndex;
            string headerText = cashddl.Columns[column].HeaderText;

            if (headerText.Equals("Count"))
            {
                tbamount = e.Control as TextBox;


                if (tbamount != null)
                {

                    tbamount.KeyPress += new KeyPressEventHandler(textbox1_keypress);
                }
            }
        }

        private void textbox1_keypress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void dgvCustomerpaid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvCustomerpaid.CurrentCell.ColumnIndex;
            string headerText = dgvCustomerpaid.Columns[column].HeaderText;

            if (headerText.Equals("Count"))
            {
                tbbaalanceanount = e.Control as TextBox;


                if (tbbaalanceanount != null)
                {

                    tbbaalanceanount.KeyPress += new KeyPressEventHandler(textbox2_keypress);
                }
            }
        }


        private void textbox2_keypress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void cashddl_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    double s = Convert.ToDouble(cashddl.Rows[e.RowIndex].Cells[0].Value);
                    double s1 = Convert.ToDouble(tbamount.Text);
                    double s2 = s * s1;
                    cashddl.Columns[2].DefaultCellStyle.Format = "N2";
                    cashddl.Rows[e.RowIndex].Cells[2].Value = s2;
                    totalval();
                }
                if (e.RowIndex == 8)
                {
                    cashddl.Rows[8].Cells[1].Selected = false;
                    lblcutomerpay.Focus();
                }
            }
            catch
            {
                if (!string.IsNullOrEmpty(tbamount.Text))
                {
                    double s1 = Convert.ToDouble(tbamount.Text);
                    cashddl.Columns[2].DefaultCellStyle.Format = "N2";
                    cashddl.Rows[e.RowIndex].Cells[2].Value = s1;
                    totalval();
                }
                else
                {
                    cashddl.Rows[e.RowIndex].Cells[1].Value = 0;
                    cashddl.Rows[e.RowIndex].Cells[2].Value = 0;
                }
            }
        }


        public void totalval()
        {
            double totalamount = 0;
            for (int i = 0; i < cashddl.Rows.Count; i++)
            {
                totalamount = totalamount + Convert.ToDouble(cashddl.Rows[i].Cells[2].Value);
            }

            lblpaymenttotal.Text = String.Format("{0:00.00}", totalamount);

            double val = Convert.ToDouble(lblpaymenttotal.Text) - Convert.ToDouble(lblpaymentamount.Text);
            lblpaymentbalance.Text = String.Format("{0:00.00}", val);
        }

        public void totalvalbalance()
        {
            double totalamount = 0;
            for (int i = 0; i < dgvCustomerpaid.Rows.Count; i++)
            {
                totalamount = totalamount + Convert.ToDouble(dgvCustomerpaid.Rows[i].Cells[2].Value);
            }

            lblpaidbalance.Text = String.Format("{0:00.00}", totalamount);


        }

        private void dgvCustomerpaid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    double s = Convert.ToDouble(dgvCustomerpaid.Rows[e.RowIndex].Cells[0].Value);
                    double s1 = Convert.ToDouble(tbbaalanceanount.Text);
                    double s2 = s * s1;
                    dgvCustomerpaid.Columns[2].DefaultCellStyle.Format = "N2";
                    dgvCustomerpaid.Rows[e.RowIndex].Cells[2].Value = s2;
                    totalvalbalance();
                }
                if (e.RowIndex == 8)
                {
                    dgvCustomerpaid.Rows[8].Cells[1].Selected = false;
                    lblcutomerpay.Focus();
                }
            }
            catch
            {
                if (!string.IsNullOrEmpty(tbbaalanceanount.Text))
                {
                    double s1 = Convert.ToDouble(tbbaalanceanount.Text);
                    dgvCustomerpaid.Columns[2].DefaultCellStyle.Format = "N2";
                    dgvCustomerpaid.Rows[e.RowIndex].Cells[2].Value = s1;
                    totalvalbalance();
                }
                else
                {
                    dgvCustomerpaid.Rows[e.RowIndex].Cells[1].Value = 0;
                    dgvCustomerpaid.Rows[e.RowIndex].Cells[2].Value = 0;
                }
            }
        }

        private void lblcutomerpay_Click(object sender, EventArgs e)
        {

            if (Convert.ToDouble(lblpaymentbalance.Text) != Convert.ToDouble(lblpaidbalance.Text))
            {
                if (Rdcashcard.Checked)
                {
                    double ds = Convert.ToDouble(lblpaymenttotal.Text) - Convert.ToDouble(lblpaidbalance.Text);
                    double val = Convert.ToDouble(lblpaymentamount.Text) - ds;
                    panelTransaction.Visible = true;
                    lnkbackcard.Visible = true;
                    lblcardammount.Text = Convert.ToString(val);
                    label105.Text = Convert.ToString(val);
                    cashpl.Visible = false;
                    lblChequeNo.Visible = true;
                    txttransactionid.Visible = true;
                    cmbbank.Focus();
                    panelCustomerpaid.Visible = false;
                }
                else
                {
                    MessageBox.Show("Plese Enter Correct Balance");
                    dgvCustomerpaid.Focus();
                    dgvCustomerpaid.CurrentCell = dgvCustomerpaid[1, 0];
                }
            }
            else
            {
                btncashpay.Enabled = true;
                savepayment();
                SavepaymentDenomination();
                clear();
                RbPaymentCard.Enabled = false;
                rbpaymentCash.Enabled = false;
                Rdcashcard.Enabled = true;
                cashpl.Visible = false;
                panelCustomerpaid.Visible = false;
                //ddlpaymode.Enabled = false;
                searchpay("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
            }
        }

        private void cashddl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (cashddl.CurrentCell.RowIndex == 8)
                {
                    cashddl.Rows[8].Cells[1].Selected = false;
                    btnreceiveBalance.Focus();
                }
            }
        }

        private void dgvCustomerpaid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (dgvCustomerpaid.CurrentCell.RowIndex == 8)
                {
                    dgvCustomerpaid.Rows[8].Cells[1].Selected = false;
                    lblcutomerpay.Focus();
                }
            }
        }

        private void btncashpay_Click(object sender, EventArgs e)
        {
            //if (Convert.ToDouble(lblpaymentbalance.Text) > Convert.ToDouble(lblpaidbalance.Text))
            //{
            //    MessageBox.Show("Please Enter Paid Balance");
            //    btnreceiveBalance.Focus();

            //}
            //else if (Convert.ToDouble(lblpaymenttotal.Text) == 0 || Convert.ToDouble(lblpaymenttotal.Text)> 0)
            //{
            //    MessageBox.Show("Please Enter Amount ");
            //}
            //else if (Rdcashcard.Checked)
            //{
            //    double val = (Convert.ToDouble(lblpaymentamount.Text) - Convert.ToDouble(lblpaymenttotal.Text));
            //    if (val == 0.0)
            //    {
            //        MessageBox.Show("Please Enter Partial Amount ");
            //    }
            //    else
            //    {
            //        panelTransaction.Visible = true;
            //        lnkbackcard.Visible = true;
            //        lblcardammount.Text = Convert.ToString(val);
            //        cashpl.Visible = false;
            //        lblChequeNo.Visible = true;
            //        txttransactionid.Visible = true;
            //        cmbbank.Focus();
            //    }
            //}

            //else
            //{
            //    if (Convert.ToDouble(lblpaymentamount.Text) > Convert.ToDouble(lblpaymenttotal.Text))
            //    {
            //        if (lblpaymentmode.Text == "Partial Credit Bill")
            //        {
            //            //DialogResult result = MessageBox.Show("Do you want to  Parital Payment?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //            //if (result == DialogResult.Yes)
            //            //{

            //            double total = Convert.ToDouble(lblpaymenttotal.Text);
            //            // Pnloading4.Visible = true;
            //            objQuotationbal.transid = txtpayorderno.Text;
            //            objQuotationbal.paidthousand = Convert.ToString(cashddl.Rows[0].Cells[1].Value);
            //            objQuotationbal.paidfivehundred = Convert.ToString(cashddl.Rows[1].Cells[1].Value);
            //            objQuotationbal.paidhundred = Convert.ToString(cashddl.Rows[2].Cells[1].Value);
            //            objQuotationbal.paidfifty = Convert.ToString(cashddl.Rows[3].Cells[1].Value);
            //            objQuotationbal.paidtwenty = Convert.ToString(cashddl.Rows[4].Cells[1].Value);
            //            objQuotationbal.paidten = Convert.ToString(cashddl.Rows[5].Cells[1].Value);
            //            objQuotationbal.paidfive = Convert.ToString(cashddl.Rows[6].Cells[1].Value);
            //            objQuotationbal.paidcoin = Convert.ToString(cashddl.Rows[7].Cells[1].Value);
            //            objQuotationbal.OAmount = Convert.ToString(total);
            //            string a = objQuotationbal.Savepayment(objQuotationbal, lblpaymenttotal.Text);
            //            if (!string.IsNullOrEmpty(a))
            //            {
            //                //  Pnloading4.Visible = false;
            //                clear();
            //                rbpaymentCash.Enabled = false;
            //                RbPaymentCard.Enabled = false;
            //                Rdcashcard.Enabled = false;
            //                //   ddlpaymode.Enabled = false;
            //                searchpay("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
            //            }
            //        }
            //        //}
            //        else
            //        {
            //            MessageBox.Show("Please Pay Full Amount ");
            //        }



            //    }
            //    else
            //    {
            //        savepayment();
            //        clear();

            //        rbpaymentCash.Enabled = false;
            //        RbPaymentCard.Enabled = false;
            //        Rdcashcard.Enabled = false;
            //        ddlpaymode.Enabled = false;
            //        searchpay("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
            //        //  Pnloading4.Visible = false;
            //    }
            //}


            if (Convert.ToDouble(lblpaymentbalance.Text) > Convert.ToDouble(lblpaidbalance.Text))
            {
                MessageBox.Show("Please Enter Paid Balance");
                btnreceiveBalance.Focus();

            }

            else if (Rdcashcard.Checked)
            {
                double val = (Convert.ToDouble(lblpaymentamount.Text) - Convert.ToDouble(lblpaymenttotal.Text));
                if (val == 0.0)
                {
                    MessageBox.Show("Please Enter Partial Amount ");
                }
                else if (Convert.ToDouble(lblpaymenttotal.Text) == 0 || Convert.ToDouble(lblpaymentbalance.Text) > 0)
                {
                    MessageBox.Show("Please Enter Amount ");
                }
                else
                {
                    panelTransaction.Visible = true;
                    lnkbackcard.Visible = true;
                    lblcardammount.Text = Convert.ToString(val);
                    label105.Text = Convert.ToString(val);
                    cashpl.Visible = false;
                    lblChequeNo.Visible = true;
                    txttransactionid.Visible = true;
                    cmbbank.Focus();
                }
            }
            else if (Convert.ToDouble(lblpaymenttotal.Text) == 0)
            {
                MessageBox.Show("Please Enter Amount ");
            }

            else
            {
                if (Convert.ToDouble(lblpaymentamount.Text) > Convert.ToDouble(lblpaymenttotal.Text))
                {
                    if (lblpaymentmode.Text == "Partial Credit Bill")
                    {
                        //DialogResult result = MessageBox.Show("Do you want to  Parital Payment?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        //if (result == DialogResult.Yes)
                        //{

                        double total = Convert.ToDouble(lblpaymenttotal.Text);
                        // Pnloading4.Visible = true;
                        objQuotationbal.transid = txtpayorderno.Text;
                        ReceiptId = txtpayorderno.Text;
                        objQuotationbal.paidthousand = Convert.ToString(cashddl.Rows[0].Cells[1].Value);
                        objQuotationbal.paidfivehundred = Convert.ToString(cashddl.Rows[1].Cells[1].Value);
                        objQuotationbal.paidhundred = Convert.ToString(cashddl.Rows[2].Cells[1].Value);
                        objQuotationbal.paidfifty = Convert.ToString(cashddl.Rows[3].Cells[1].Value);
                        objQuotationbal.paidtwenty = Convert.ToString(cashddl.Rows[4].Cells[1].Value);
                        objQuotationbal.paidten = Convert.ToString(cashddl.Rows[5].Cells[1].Value);
                        objQuotationbal.paidfive = Convert.ToString(cashddl.Rows[6].Cells[1].Value);
                        objQuotationbal.paidcoin = Convert.ToString(cashddl.Rows[7].Cells[1].Value);
                        objQuotationbal.OAmount = Convert.ToString(total);
                        string a = objQuotationbal.Savepayment(objQuotationbal, lblpaymenttotal.Text);
                        if (!string.IsNullOrEmpty(a))
                        {
                            //  Pnloading4.Visible = false;
                            clear();
                            rbpaymentCash.Enabled = false;
                            RbPaymentCard.Enabled = false;
                            Rdcashcard.Enabled = false;
                            //   ddlpaymode.Enabled = false;
                            searchpay("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                        }
                    }
                    //}
                    else
                    {
                        MessageBox.Show("Please Pay Full Amount ");
                    }



                }
                else
                {
                    savepayment();
                    clear();

                    rbpaymentCash.Enabled = false;
                    RbPaymentCard.Enabled = false;
                    Rdcashcard.Enabled = false;
                    ddlpaymode.Enabled = false;
                    searchpay("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                    //  Pnloading4.Visible = false;
                }
            }
        }

        public void savepayment()
        {
            double total = 0.0;
            if (Rdcashcard.Checked)
            {
                total = Convert.ToDouble(lblpaymenttotal.Text) - Convert.ToDouble(lblpaidbalance.Text);
            }
            else
            {
                total = Convert.ToDouble(lblpaymenttotal.Text) - Convert.ToDouble(lblpaymentbalance.Text);
            }
            //  Pnloading4.Visible = true;
            objQuotationbal.transid = txtpayorderno.Text;
            ReceiptId = txtpayorderno.Text;
            objQuotationbal.paidthousand = Convert.ToString(cashddl.Rows[0].Cells[1].Value);
            objQuotationbal.paidfivehundred = Convert.ToString(cashddl.Rows[1].Cells[1].Value);
            objQuotationbal.paidhundred = Convert.ToString(cashddl.Rows[2].Cells[1].Value);
            objQuotationbal.paidfifty = Convert.ToString(cashddl.Rows[3].Cells[1].Value);
            objQuotationbal.paidtwenty = Convert.ToString(cashddl.Rows[4].Cells[1].Value);
            objQuotationbal.paidten = Convert.ToString(cashddl.Rows[5].Cells[1].Value);
            objQuotationbal.paidfive = Convert.ToString(cashddl.Rows[6].Cells[1].Value);
            objQuotationbal.paidcoin = Convert.ToString(cashddl.Rows[7].Cells[1].Value);
            objQuotationbal.PaidOne = Convert.ToString(cashddl.Rows[8].Cells[1].Value);
            objQuotationbal.OAmount = Convert.ToString(total);
            lblid.Text = objQuotationbal.Savepayment(objQuotationbal, lblpaymenttotal.Text);



        }

        public void SavepaymentDenomination()
        {

            // Pnloading4.Visible = true;
            objQuotationbal.recivetransid = lblid.Text;
            //ReceiptId = lblid.Text;
            if (Convert.ToString(dgvCustomerpaid.Rows[0].Cells[1].Value) != "0")
            {
                objQuotationbal.recivethousand = "-" + Convert.ToString(dgvCustomerpaid.Rows[0].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivethousand = Convert.ToString(dgvCustomerpaid.Rows[0].Cells[1].Value);
            }
            if (Convert.ToString(dgvCustomerpaid.Rows[1].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefivehundred = "-" + Convert.ToString(dgvCustomerpaid.Rows[1].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefivehundred = Convert.ToString(dgvCustomerpaid.Rows[1].Cells[1].Value);
            }



            if (Convert.ToString(dgvCustomerpaid.Rows[2].Cells[1].Value) != "0")
            {
                objQuotationbal.recivehundred = "-" + Convert.ToString(dgvCustomerpaid.Rows[2].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivehundred = Convert.ToString(dgvCustomerpaid.Rows[2].Cells[1].Value);
            }


            if (Convert.ToString(dgvCustomerpaid.Rows[3].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefifty = "-" + Convert.ToString(dgvCustomerpaid.Rows[3].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefifty = Convert.ToString(dgvCustomerpaid.Rows[3].Cells[1].Value);
            }


            if (Convert.ToString(dgvCustomerpaid.Rows[4].Cells[1].Value) != "0")
            {
                objQuotationbal.recivetwenty = "-" + Convert.ToString(dgvCustomerpaid.Rows[4].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivetwenty = Convert.ToString(dgvCustomerpaid.Rows[4].Cells[1].Value);
            }

            if (Convert.ToString(dgvCustomerpaid.Rows[5].Cells[1].Value) != "0")
            {
                objQuotationbal.reciveten = "-" + Convert.ToString(dgvCustomerpaid.Rows[5].Cells[1].Value);

            }
            else
            {
                objQuotationbal.reciveten = Convert.ToString(dgvCustomerpaid.Rows[5].Cells[1].Value);
            }


            if (Convert.ToString(dgvCustomerpaid.Rows[6].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefive = "-" + Convert.ToString(dgvCustomerpaid.Rows[6].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefive = Convert.ToString(dgvCustomerpaid.Rows[6].Cells[1].Value);
            }

            if (Convert.ToString(dgvCustomerpaid.Rows[7].Cells[1].Value) != "0")
            {
                objQuotationbal.recivecoin = "-" + Convert.ToString(dgvCustomerpaid.Rows[7].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivecoin = Convert.ToString(dgvCustomerpaid.Rows[7].Cells[1].Value);
            }

            if (Convert.ToString(dgvCustomerpaid.Rows[8].Cells[1].Value) != "0")
            {
                objQuotationbal.ReceiveOne = "-" + Convert.ToString(dgvCustomerpaid.Rows[8].Cells[1].Value);

            }
            else
            {
                objQuotationbal.ReceiveOne = Convert.ToString(dgvCustomerpaid.Rows[8].Cells[1].Value);
            }

            objQuotationbal.OAmount = "-" + lblpaidbalance.Text;

            string output = objQuotationbal.SavepaymentDenomination(objQuotationbal);

            if (output == "1")
            {
                //Pnloading4.Visible = false;

            }

        }

        public void savecard()
        {

            // Pnloading.Visible = true;
            objQuotationbal.Quotationid = txtpayorderno.Text;
            ReceiptId = txtpayorderno.Text;
            objQuotationbal.Bank = cmbbank.Text;
            objQuotationbal.Cardnumber = txtcardno.Text;
            objQuotationbal.transid = txttransactionid.Text;
            if (Rdcashcard.Checked)
            {
                objQuotationbal.OAmount = Convert.ToString(lblcardammount.Text);
                objQuotationbal.type = "Cash And Card";
            }
            else
            {
                objQuotationbal.OAmount = Convert.ToString(lblcardammount.Text);
                objQuotationbal.type = "Card";
            }
            string s = objQuotationbal.savecardtransaction(objQuotationbal);

            if (s == "1")
            {
                savesales();
                //Pnloading.Visible = false;
                //if (Rdcashcard.Checked==false)
                //{
                clear();
                // }
            }

        }

        public void savesales()
        {
            panel1.Enabled = false;
            panel2.Enabled = false;

            // Pnloading.Visible = true;


            DataTable dt = new DataTable();

            objQuotationbal.isnew = 0;


            //objQuotationbal.Customerid = Convert.ToString(txtpauycustomername.Text);
            objQuotationbal.Customername = Convert.ToString(txtpauycustomername.Text);
            objQuotationbal.City = Convert.ToString(txtpaycity.Text);

            //objQuotationbal.Referenceid = Convert.ToString();
            //objQuotationbal.Assist = Convert.ToString(cmbassistby.SelectedValue);
            objQuotationbal.Paymentmode = "Card";
            objQuotationbal.Updatedby = Program.userid;
            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            objQuotationbal.date = date;
            objQuotationbal.Total = Convert.ToString(lblcardammount.Text);
            objQuotationbal.Lessamount = txtpayless.Text; ;
            objQuotationbal.others = lblpaymentothers.Text; ;
            objQuotationbal.Grandtotal = Convert.ToString(lblpatroundoff.Text);
            dt = DataGridView2DataTable(dgvPayment);
            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            //dt.Columns.RemoveAt(4);
            RemoveNullColumnFromDataTable1(dt);

            string output = objQuotationbal.SaveQuotationsales(objQuotationbal, dt);
            if (!string.IsNullOrEmpty(output))
            {
                panel1.Enabled = true;
                panel2.Enabled = true;
                //Pnloading.Visible = false;
                //getsaleamount();
                //MessageBox.Show("save successfully");
                //    txtorder.Text = output;
                //if (RbPaymentCard.Checked == false && rbpaymentCash.Checked==true)
                //{
                DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Salesbillreport rpt = new Salesbillreport(output);
                    rpt.ShowDialog();
                }
                //}




                //else if (!string.IsNullOrEmpty(output) && !string.IsNullOrEmpty(txtorder.Text))
                //{
                //    //MessageBox.Show("Update successfully");
                //    panel1.Enabled = true;
                //    panel2.Enabled = true;
                //    Pnloading.Visible = false;

                //    txtorder.Text = output;
                //    if (v == 2)
                //    {
                //        DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                //        if (result == DialogResult.Yes)
                //        {
                //            Quotationreport rpt = new Quotationreport(output);
                //            rpt.ShowDialog();
                //        }
                //    }
            }


        }


        public void bindAccountno()
        {
            cmbbank.DataSource = objQuotationbal.getaccount();
            cmbbank.DisplayMember = "accountno";
            cmbbank.ValueMember = "accountno";
        }
        public void searchcheck(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchcheck(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        public void getcheck(string s)
        {
            DataSet ds = objQuotationbal.Bindbillpay(s);

            if (ds.Tables[0].Rows.Count > 0)
            {
                ddlpaymode.Enabled = true;
                RbPaymentCard.Enabled = true;
                rbpaymentCash.Enabled = true;
                Rdcashcard.Enabled = true;
                txtcheckingorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
                Txtcheckcustmername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                txtcheckingcity.Text = Convert.ToString(ds.Tables[0].Rows[0]["City"]);
                txtcheckingreference.Text = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);

                Txtcheckingprepareby.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);

                txtcheckassistby.Text = Convert.ToString(ds.Tables[0].Rows[0]["UserFullName"]);

                datechecking.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);

                txtcheckingquotationid.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);

                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {

                dgvChecking.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvChecking.Rows.Add();
                    dgvChecking.Rows[i].Cells[0].Value = i + 1;
                    dgvChecking.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvChecking.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    dgvChecking.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

                    if (Convert.ToDecimal(dgvChecking.Rows[i].Cells[2].Value) < 0)
                    {
                        dgvChecking.Rows[i].ReadOnly = true;
                        dgvChecking.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    }
                    //double qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    //dgvChecking.Rows[i].Cells[4].Value = qty;
                    //dgvChecking.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);

                    //double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    //dgvChecking.Rows[i].Cells[6].Value = amt;


                    //dgvPaymentReceived.Rows.Add();
                    //dgvPaymentReceived.Rows[i].Cells[0].Value = i + 1;
                    //dgvPaymentReceived.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["ItemName"]);
                    //dgvPaymentReceived.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    //dgvPaymentReceived.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
                    //dgvPaymentReceived.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                }
                panel2.Enabled = false;
            }
            else
            {
                dgvChecking.Rows.Clear();
                panel2.Enabled = true;
            }

        }

        public void getDelivery(string s)
        {
            DataSet ds = objQuotationbal.binddelivered(s);

            if (ds.Tables[0].Rows.Count > 0)
            {
                dgvDelivery.Enabled = true;
                Txtdeliveryorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
                txtdeliverycustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                Txtdeviverycity.Text = Convert.ToString(ds.Tables[0].Rows[0]["City"]);
                Txtdeliveryreference.Text = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);

                Txtdeliveryprepareby.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);

                Txtdeviveryassist.Text = Convert.ToString(ds.Tables[0].Rows[0]["UserFullName"]);

                datedelivery.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);

                Txtdeliveryquotationno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);

                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {

                dgvDelivery.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvDelivery.Rows.Add();
                    dgvDelivery.Rows[i].Cells[0].Value = i + 1;
                    dgvDelivery.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvDelivery.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    dgvDelivery.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    //dgvDelivery.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    if (Convert.ToDecimal(dgvDelivery.Rows[i].Cells[2].Value) < 0)
                    {
                        dgvDelivery.Rows[i].ReadOnly = true;
                        dgvDelivery.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    }
                    if (Convert.ToBoolean(ds.Tables[1].Rows[i]["isdelevered"]) == true)
                    {
                        dgvDelivery.Rows[i].Cells[6].Value = Convert.ToBoolean(ds.Tables[1].Rows[i]["isdelevered"]);
                        dgvDelivery.Rows[i].ReadOnly = true;
                    }
                    else
                    {
                        dgvDelivery.Rows[i].Cells[6].Value = Convert.ToBoolean(ds.Tables[1].Rows[i]["isdelevered"]);
                    }


                }
                panel2.Enabled = false;
            }
            else
            {
                dgvChecking.Rows.Clear();
                panel2.Enabled = true;
            }

        }

        private void dgvChecking_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                try
                {
                    if (Convert.ToDouble(dgvChecking.Rows[e.RowIndex].Cells[2].Value) != Convert.ToDouble(tborderquantoty.Text))
                    {
                        MessageBox.Show("Please Enter Correct Quantity");
                        dgvChecking.Focus();
                        edit = true;
                        dgvChecking.CurrentCell = dgvChecking[3, e.RowIndex];
                        dgvChecking.Rows[e.RowIndex].Cells[3].Value = "";
                    }
                }
                catch
                {

                }
            }
        }

        private void dgvChecking_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvChecking.CurrentCell.ColumnIndex;
            string headerText = dgvChecking.Columns[column].HeaderText;

            if (headerText.Equals("Quantity"))
            {
                tborderquantoty = e.Control as TextBox;
                tborderquantoty.KeyPress += new KeyPressEventHandler(txtless_KeyPress);
                tborderquantoty.MaxLength = 6;
            }

        }



        private void dgvChecking_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (edit == true)
                {
                    if (dgvChecking.CurrentCell.RowIndex >= 1)
                    {
                        dgvChecking.CurrentCell = dgvChecking[dgvChecking.CurrentCell.ColumnIndex, dgvChecking.CurrentCell.RowIndex - 1];
                        edit = false;
                    }
                    else if (dgvChecking.CurrentCell.RowIndex == 0)
                    {
                        dgvChecking.CurrentCell = dgvChecking[dgvChecking.CurrentCell.ColumnIndex, dgvChecking.CurrentCell.RowIndex - 1];
                    }

                }
            }
            catch
            {

            }
        }

        private void label40_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = label40.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = label40.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = label40.Text.Trim();
            }
            pnlCalender.Visible = false;
        }

        public void searchdelivery(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchdelivery(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }


        private bool Validationdelivery()
        {
            bool status = true;
            string message = "";
            int i = 0;

            foreach (DataGridViewRow row in dgvDelivery.Rows)
            {
                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];
                if (Convert.ToBoolean(CbxCell.Value) == false && Convert.ToDecimal(row.Cells[2].Value) >= 0)
                {
                    status = false;
                    break;
                }

            }
            return status;
        }

        private bool Validationdelivery1()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (dgvDelivery.Rows.Count > 0)
            {

                foreach (DataGridViewRow row in dgvDelivery.Rows)
                {
                    if (Convert.ToDouble(row.Cells["quantity"].Value) > 0)
                    {
                        if (Convert.ToString(row.Cells["Delivered Qty"].Value) != "." && !string.IsNullOrEmpty(Convert.ToString(row.Cells["Delivered Qty"].Value)) && row.DefaultCellStyle.BackColor != Color.LightSteelBlue)
                        {
                            if ((string.IsNullOrEmpty(Convert.ToString(row.Cells[5].Value)) && (Convert.ToDecimal(row.Cells["Delivered Qty"].Value) > 0)) || (Convert.ToDecimal(row.Cells["Quantity"].Value) < Convert.ToDecimal(row.Cells["Delivered Qty"].Value)))
                            {
                                status = false;
                            }

                        }
                        else
                        {
                            status = false;
                        }
                    }
                }
            }
            else
            {
                status = false;
            }

            return status;
        }

        public string getcheckedvalue()
        {
            string val = string.Empty;
            DataTable dt = DataGridView2DataTable(dgvDelivery);
            dt.Columns.RemoveAt(0);
            dt.Columns.RemoveAt(0);
            dt.Columns.RemoveAt(2);
            dt.Columns.RemoveAt(2);
            dt.Columns.RemoveAt(2);
            dt.Columns.Add("Rack").SetOrdinal(1);
            foreach (DataGridViewRow row in dgvDelivery.Rows)
            {
                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];
                if (Convert.ToBoolean(CbxCell.Value) == true)
                {

                    string s = Convert.ToString(row.Cells[2].Value);
                    string delivered = Convert.ToString(row.Cells[2].Value);
                    savedelivered(Convert.ToString(row.Cells[3].Value), 1, dt, s, delivered, 1);
                }
                else
                {
                    string s = Convert.ToString(row.Cells[2].Value);
                    string delivered = Convert.ToString(row.Cells[4].Value);
                    savedelivered(Convert.ToString(row.Cells[3].Value), 1, dt, s, delivered, 0);
                }

                //if(string.IsNullOrEmpty(val))
                //{
                //    val ="'"+Convert.ToString(row.Cells[3].Value)+"'";
                //}
                //else
                //{
                //    val =val+ ",'" + Convert.ToString(row.Cells[3].Value) + "'";
                //}

            }
            //string[] vasa = val.Split(',');
            //if (vasa.Length==1)
            //{
            //    val = val.Replace("'", "");
            //}

            return val;
        }

        public void savedelivered(string pid, int status, DataTable dt, string Quanity, string deliveredQty, int val)
        {
            //  Pnloading6.Visible = true;
            //string s = objQuotationbal.savedelivered(Txtdeliveryorderno.Text, pid, status, dt, Quanity, deliveredQty, val,Program.userid);

            //if (s == "1")
            //{
                //if (status == 2)
                //{
                //Pnloading6.Visible = false;
                //clear();
                //}
            //}

        }

        public void deletedelivered()
        {

            objQuotationbal.deletedelivered(Txtdeliveryorderno.Text);


        }

        private void dgvChecking_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvChecking.Rows.Count > 0)
            {
                if (dgvChecking.CurrentCell.RowIndex == dgvChecking.Rows.Count - 1)
                {
                    if (e.KeyData == Keys.Enter)
                    {
                        btnsave.Focus();
                    }
                }
            }
        }

        private void dgvFloorCheckOut_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dgvFloorCheckOut.CurrentCell.RowIndex == dgvFloorCheckOut.Rows.Count - 1)
                {
                    if (e.KeyData == Keys.Enter)
                    {
                        btnsave.Focus();
                    }
                }
            }
            catch
            {

            }

        }


        private void dgvDelivery_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvDelivery.Rows.Count > 0)
            {
                if (dgvDelivery.CurrentCell.RowIndex == dgvDelivery.Rows.Count - 1)
                {
                    if (e.KeyData == Keys.Enter)
                    {
                        btnsave.Focus();
                    }
                }
            }
        }

        public DataTable getdatatable(string itemsToAdd)
        {
            DataTable table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("location", typeof(string));


            string[] item = itemsToAdd.Split(';');

            for (int i = 0; i < item.Length; i++)
            {
                string[] val = item[i].Split(',');
                table.Rows.Add(val[0], val[1]);
            }
            return table;
        }

        private void dgvFloorCheckOut_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvFloorCheckOut.CurrentCell.ColumnIndex;
            string headerText = dgvFloorCheckOut.Columns[column].HeaderText;

            if (headerText.Equals("Location"))
            {
                cmbloaction = e.Control as ComboBox;


                if (cmbloaction != null)
                {
                    cmbloaction.SelectedIndexChanged += new EventHandler(cmbloaction_SelectedIndexChanged);

                }
            }
        }

        private void cmbloaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox cmb = (ComboBox)sender;
                if (cmb.SelectedIndex >= 0)
                {
                    string val = Convert.ToString(cmb.SelectedValue);
                    string val1 = Convert.ToString(dgvFloorCheckOut.Rows[dgvFloorCheckOut.CurrentCell.RowIndex].Cells["Productid"].Value);
                    string v = getrack(val, val1);
                    dgvFloorCheckOut.Rows[dgvFloorCheckOut.CurrentCell.RowIndex].Cells["Rack"].Value = v;
                }
                else
                {
                    dgvFloorCheckOut.Rows[dgvFloorCheckOut.CurrentCell.RowIndex].Cells["Rack"].Value = "";
                }
            }
            catch
            {

            }


        }


        public string getrack(string s, string s1)
        {
            string v = objQuotationbal.getrack(s, s1);
            return v;
        }

        private void dgvFloorCheckOut_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvFloorCheckOut_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex != 6)
                {
                    if (dgvFloorCheckOut[e.ColumnIndex, e.RowIndex].EditType.ToString() == "System.Windows.Forms.DataGridViewComboBoxEditingControl")
                    {
                        DataGridViewColumn column = dgvFloorCheckOut.Columns[e.ColumnIndex];
                        if (column is DataGridViewComboBoxColumn)
                        {
                            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dgvFloorCheckOut[e.ColumnIndex, e.RowIndex];
                            dgvFloorCheckOut.CurrentCell = cell;
                            dgvFloorCheckOut.BeginEdit(true);
                            DataGridViewComboBoxEditingControl editingControl = (DataGridViewComboBoxEditingControl)dgvFloorCheckOut.EditingControl;
                            editingControl.DroppedDown = true;
                        }
                    }
                }
            }
            catch
            {

            }
        }


        private void vLabel4_Click(object sender, EventArgs e)
        {

            if (pnlLabelSearch.Visible == true)
            {
                pnlLabelSearch.Visible = false;
                vLabel1.Visible = false;
                pnlSearch.Visible = true;
                splitContainer1.Panel1Collapsed = false;
            }
        }

        private void Txtcustomername_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Txtcustomername.SelectedIndex > 0)
            {
                cmdcity.Text = objQuotationbal.bindcity(Convert.ToString(Txtcustomername.SelectedValue));
            }

        }

        private void SalesBillNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int j = 0; j < MainTabSalesBill.TabPages.Count; j++)
                {

                    for (int i = 0; i < MainTabSalesBill.TabPages[j].Controls.Count; i++)
                    {
                        MainTabSalesBill.TabPages[j].Controls[i].Dispose();
                    }
                }

                MainTabSalesBill.Dispose();

                this.Dispose();
            }
            catch
            {

            }
        }

        private void txtcardno_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void btnLess_Click(object sender, EventArgs e)
        {
            pnlless.Visible = true;
            txtlesspwd.Text = string.Empty;
            this.ActiveControl = txtlesspwd;

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtlesspwd.Text))
            {
                MessageBox.Show("Password should not be empty");
                this.ActiveControl = txtlesspwd;
                return;
            }
            DataTable dt = LoginBAL.GetLesserDetials(txtlesspwd.Text, "LESS");
            if (dt.Rows.Count > 0)
            {
                lblless.Visible = true;
                txtless.Visible = true;
                Cmbless.Visible = true;
                if (Convert.ToDouble(lbltotalamount.Text) > 0)
                {
                    Cmbless.Enabled = false;
                }
                else
                {
                    Cmbless.Enabled = true;
                    Cmbless.SelectedIndex = 1;
                }
                pnlless.Visible = false;
                //pnlnet.Visible = true;
                //lblTotal.Visible = true;
                this.ActiveControl = txtless;
            }
            else
            {
                MessageBox.Show("Authentication Failed");
                lblless.Visible = false;
                txtless.Visible = false;
                Cmbless.Visible = false;
                //pnlnet.Visible = false;
                //lblTotal.Visible = false;

            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            pnlless.Visible = false;
            txtlesspwd.Text = string.Empty;
        }

        private void RbPaymentCard_CheckedChanged(object sender, EventArgs e)
        {
            lnkbackcard.Visible = false;

            if (rbpaymentCash.Checked)
            {
                lblpaymentamount.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpatroundoff.Text));
                cashpl.Visible = true;
                panelTransaction.Visible = false;

                this.ActiveControl = cashddl;
                Application.Idle += new EventHandler(Application_Idle);
            }
            else if (RbPaymentCard.Checked)
            {
                panelTransaction.Visible = true;
                lblChequeNo.Visible = true;
                txttransactionid.Visible = true;
                cashpl.Visible = false;
                lblcardammount.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpatroundoff.Text));
                label105.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpatroundoff.Text));
                cmbbank.Focus();

            }
            else
            {
                panelCustomerpaid.Visible = false;
                panelTransaction.Visible = false;
                cashpl.Visible = false;
            }
        }

        private void rbpaymentCash_CheckedChanged(object sender, EventArgs e)
        {
            btnreceiveBalance.Enabled = true;
            if (rbpaymentCash.Checked)
            {
                Rectangle resolution = Screen.PrimaryScreen.Bounds;
                int w = resolution.Width;
                int h = resolution.Height;

                if (w == 1024 && h == 768)
                {
                    //button3.Location = new Point(350, 2);
                    //button1.BringToFront();
                    panelCustomerpaid.Width = 350;
                    dgvCustomerpaid.Width = 330;
                    cashpl.Width = 360;
                    cashddl.Width = 340;
                    cashpl.Location = new Point(358, 32);
                    btnreceiveBalance.Location = new Point(180, 352);
                    btncashpay.Location = new Point(280, 351);
                    cashdetailsclose.Location = new Point(300, 1);
                    panelCustomerpaid.Location = new Point(0, 34);
                    cashddl.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11F, FontStyle.Bold);
                    cashddl.DefaultCellStyle.Font = new Font("Tahoma", 13F, GraphicsUnit.Pixel);
                    cashddl.DefaultCellStyle.BackColor = Color.Gainsboro;
                    cashddl.DefaultCellStyle.ForeColor = Color.Black;
                    cashddl.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                    lblcutomerpay.Location = new Point(270, 298);

                    dgvCustomerpaid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11F, FontStyle.Bold);
                    dgvCustomerpaid.DefaultCellStyle.Font = new Font("Tahoma", 13F, GraphicsUnit.Pixel);
                    dgvCustomerpaid.DefaultCellStyle.BackColor = Color.Gainsboro;
                    dgvCustomerpaid.DefaultCellStyle.ForeColor = Color.Black;
                    dgvCustomerpaid.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                }
                else
                {
                    //panelCustomerpaid.Location = new Point(18, 32);
                    //cashpl.Location = new Point(474, 36);

                    //panelCustomerpaid.Width = 439;
                    //dgvCustomerpaid.Width = 421;

                }

                lblpaymentamount.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpatroundoff.Text));
                cashpl.Visible = true;
                panelTransaction.Visible = false;
                this.ActiveControl = cashddl;
                Application.Idle += new EventHandler(Application_Idle);
            }
            else if (RbPaymentCard.Checked)
            {
                panelTransaction.Visible = true;
                lblChequeNo.Visible = true;
                txttransactionid.Visible = true;
                lblcardammount.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpatroundoff.Text));
                label105.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpatroundoff.Text));
                cmbbank.Focus();
            }
            else
            {
                panelCustomerpaid.Visible = false;
                panelTransaction.Visible = false;
                cashpl.Visible = false;
            }
        }

        private void dgvDelivery_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvDelivery.CurrentCell.ColumnIndex;
            string headerText = dgvDelivery.Columns[column].HeaderText;

            if (headerText.Equals("Delivered Qty"))
            {
                tbdeliveredqty = e.Control as TextBox;
                tbdeliveredqty.KeyPress += new KeyPressEventHandler(txtless_KeyPress);
                tbdeliveredqty.MaxLength = 6;

            }
        }

        private void dgvDelivery_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    if (e.ColumnIndex == 4)
                    {
                        double sas = Convert.ToDouble(dgvDelivery.Rows[e.RowIndex].Cells[2].Value);
                        double s = sas - Convert.ToDouble(tbdeliveredqty.Text);
                        dgvDelivery.Rows[e.RowIndex].Cells[5].Value = s;
                        if (s >= 0)
                        {
                            dgvDelivery.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                            dgvDelivery.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;

                            if (s == 0)
                            {
                                DataGridViewRow row = dgvDelivery.Rows[e.RowIndex];
                                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];
                                CbxCell.Value = 1;

                            }
                            else
                            {
                                DataGridViewRow row = dgvDelivery.Rows[e.RowIndex];
                                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];
                                CbxCell.Value = 0;
                            }
                        }
                        else
                        {
                            dgvDelivery.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                            dgvDelivery.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                        }
                    }
                }
                catch
                {

                }


            }
        }

        private void rbCash_CheckedChanged(object sender, EventArgs e)
        {
            btnLess.Enabled = true;
        }

        private void rbcredit_CheckedChanged(object sender, EventArgs e)
        {
            btnLess.Enabled = true;
            lblless.Visible = false;
            txtless.Visible = false;
            Cmbless.Visible = false;
        }

        private void rbpartial_CheckedChanged(object sender, EventArgs e)
        {
            btnLess.Enabled = true;
            lblless.Visible = false;
            txtless.Visible = false;
            Cmbless.Visible = false;
        }

        private void Txtitem_KeyUp(object sender, KeyEventArgs e)
        {
            string word;
            int typr = 0;
            try
            {

                if (e.KeyData != Keys.Enter && e.KeyData != Keys.Tab && e.KeyData != Keys.Down && e.KeyData != Keys.Up && e.KeyData != Keys.Left && e.KeyData != Keys.Right && e.KeyData != Keys.Escape && e.KeyData != Keys.F2 && e.KeyData != (Keys.S | Keys.Alt) && e.KeyData != (Keys.C | Keys.Alt) && e.KeyData != (Keys.N | Keys.Alt) && e.KeyData != (Keys.V | Keys.Alt) && e.KeyData != (Keys.M | Keys.Alt) && e.KeyData != (Keys.D | Keys.Alt) && e.KeyData != (Keys.X | Keys.Alt))
                {
                    word = Txtitem.Text;
                    if (rdbStartsWith.Checked)
                    {
                        typr = 1;
                    }
                    else if (rdbContains.Checked)
                    {
                        typr = 2;
                    }
                    AutoCompleteLoad(word, typr);

                }
                if (e.KeyData == Keys.Up)
                {
                    try
                    {
                        //if (DgvAutoRefNo.Rows.Count - 1 != ProdSelRowvalue)
                        //{
                        //DataGridViewRow theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue - 1];
                        //if (theRow3.Index != DgvAutoRefNo.RowCount)
                        //{

                        //    theRow3.DefaultCellStyle.BackColor = Color.LightGray;

                        //    theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue];
                        //    theRow3.DefaultCellStyle.BackColor = Color.White;

                        //    ProdSelRowvalue--;
                        //    cas = Convert.ToString(DgvAutoRefNo[0, ProdSelRowvalue].Value);
                        //    itemdetails(cas);
                        //    RefScrollGrid();
                        //}
                        //}
                    }
                    catch
                    {
                        //ProdSelRowvalue = DgvAutoRefNo.CurrentCell.RowIndex;
                    }

                }
                if (e.KeyData == Keys.Down)
                {
                    if (DgvAutoRefNo.Rows.Count > 0)
                    {
                        DgvAutoRefNo.Focus();
                        DgvAutoRefNo.CurrentCell = DgvAutoRefNo[0, 0];
                        DgvAutoRefNo.Rows[0].Cells[0].Selected = true;
                        string sa = Convert.ToString(DgvAutoRefNo.Rows[0].Cells[0].Value);
                        getitems(sa);
                    }
                    //try
                    //{
                    //    if (DgvAutoRefNo.Rows.Count - 1 != ProdSelRowvalue)
                    //    {
                    //        DataGridViewRow theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue + 1];
                    //        if (theRow3.Index != DgvAutoRefNo.RowCount)
                    //        {

                    //            theRow3.DefaultCellStyle.BackColor = Color.LightGray;

                    //            theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue];
                    //            theRow3.DefaultCellStyle.BackColor = Color.White;

                    //            ProdSelRowvalue++;
                    //            cas = Convert.ToString(DgvAutoRefNo[0, ProdSelRowvalue].Value);
                    //            itemdetails(cas);
                    //            RefScrollGrid();
                    //        }
                    //    }
                    //}
                    //catch
                    //{
                    //    //ProdSelRowvalue = DgvAutoRefNo.CurrentCell.RowIndex;
                    //}

                }

                if (e.KeyData == Keys.Enter)
                {
                    //if (!string.IsNullOrEmpty(Txtitem.Text))
                    //{
                    //    if (res == false)
                    //    {
                    //        if (DgvAutoRefNo.Visible == false)
                    //        {
                    //            DgvAutoRefNo.Visible = false;

                    //        }
                    //        else
                    //        {
                    //            Txtitem.Text = Convert.ToString(DgvAutoRefNo[0, DgvAutoRefNo.CurrentCell.RowIndex].Value);
                    //            DgvAutoRefNo.Visible = false;
                    //            DgvAutoRefNo.Rows[0].Selected = false;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    //if (status6 == false && v == false)
                    //    //{
                    //    //    MessageBox.Show("No records found");
                    //    //    txtRegNo.Focus();
                    //    //    status6 = true;
                    //    //}
                    //    //else
                    //    //{
                    //    //    status6 = false;
                    //    //    v = false;


                    //    //}
                    //}

                }
            }
            catch (Exception efd)
            {

            }
        }

        private void RefScrollGrid()
        {
            if (DgvAutoRefNo.Rows.Count - 1 >= ProdSelRowvalue)
            {
                DgvAutoRefNo.FirstDisplayedScrollingRowIndex = ProdSelRowvalue;
            }
        }

        private void DgvAutoRefNo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (Convert.ToInt32(lblproductid.Text) != 0)
                {
                    int rowindex = Convert.ToInt32(lblrowindex.Text);

                    if (selectedtab == "TabNew")
                    {
                        dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];

                        string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                        getitems(sa);

                        dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        dgvNew.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();

                             button6.Enabled = true;
                        //dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        //dgvNew.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                        dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;

                        dgvNew.Rows[rowindex].Cells[5].ReadOnly = false;

                        double val = Convert.ToDouble(lblprice.Text);

                        dgvNew.Rows[rowindex].Cells[4].Value = val;
                        dgvNew.Rows[rowindex].Cells[0].Value = rowindex + 1;
                        pnsearch.Visible = false;
                        DgvAutoRefNo.Visible = false;

                        lblproductid.Text = string.Empty;
                        //  Txtitem.Text = string.Empty;
                        lblitemcode.Text = "0";
                        lblprice.Text = "0";
                        lbldisplay.Text = "0";
                        lbldemo.Text = "0";
                        lblservice.Text = "0";
                        lbldamage.Text = "0";
                        lblrack.Text = "0";
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
                    }
                }
            }
        }

        private void dgvNew_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {


                if (!string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value)))
                {
                    decimal rate = Convert.ToDecimal(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value);
                    //decimal amt = rate * Convert.ToDecimal(tb.Text);

                    decimal amt = rate * Convert.ToDecimal(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value);

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
                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = amt;
                }


            }
            catch
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[1].Value)))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value)))
                    {
                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = 0.00;
                    }

                }

            }
            total();
        }

        public void getsino()
        {
            for (int i = 0; i < dgvNew.Rows.Count; i++)
            {
                dgvNew.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            //cashpl.Visible = true;
            //cashddl.Focus();
            //cashddl.CurrentCell = cashddl[1, 0];
            // btnreceiveBalance.Enabled = false;
            lblpaymentamount.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpatroundoff.Text));
            cashpl.Visible = true;
            panelTransaction.Visible = false;
            this.ActiveControl = cashddl;
            Application.Idle += new EventHandler(Application_Idle);

            if (Rdcashcard.Checked)
            {
                Rectangle resolution = Screen.PrimaryScreen.Bounds;
                int w = resolution.Width;
                int h = resolution.Height;

                if (w == 1024 && h == 768)
                {
                    //button3.Location = new Point(350, 2);
                    //button1.BringToFront();
                    panelCustomerpaid.Width = 350;
                    dgvCustomerpaid.Width = 330;
                    cashpl.Width = 360;
                    cashddl.Width = 340;
                    cashpl.Location = new Point(358, 32);
                    btnreceiveBalance.Location = new Point(180, 352);
                    btncashpay.Location = new Point(280, 351);
                    cashdetailsclose.Location = new Point(300, 1);
                    panelCustomerpaid.Location = new Point(0, 34);
                    cashddl.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11F, FontStyle.Bold);
                    cashddl.DefaultCellStyle.Font = new Font("Tahoma", 13F, GraphicsUnit.Pixel);
                    cashddl.DefaultCellStyle.BackColor = Color.Gainsboro;
                    cashddl.DefaultCellStyle.ForeColor = Color.Black;
                    cashddl.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                    lblcutomerpay.Location = new Point(270, 298);

                    dgvCustomerpaid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11F, FontStyle.Bold);
                    dgvCustomerpaid.DefaultCellStyle.Font = new Font("Tahoma", 13F, GraphicsUnit.Pixel);
                    dgvCustomerpaid.DefaultCellStyle.BackColor = Color.Gainsboro;
                    dgvCustomerpaid.DefaultCellStyle.ForeColor = Color.Black;
                    dgvCustomerpaid.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void lnkbackcard_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panelTransaction.Visible = false;
            cashpl.Visible = true;
            cashddl.Focus();
            cashddl.CurrentCell = cashddl[1, 0];

        }

        private void Btnmobilenumber_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtMobilenumber.Text))
            {
                MessageBox.Show("Please Enter Mobile Number");
                TxtMobilenumber.Focus();
            }
            else
            {
                panel11.Visible = false;
                btnsave.Focus();
            }
        }

        private void DgvAutoRefNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(Txtitem.Text))
                {

                    if (Convert.ToInt32(lblproductid.Text) != 0)
                    {
                        dgvNew.Columns[4].DefaultCellStyle.Format = "N2";
                        int rowindex = Convert.ToInt32(lblrowindex.Text);
                        dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
                        dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        dgvNew.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                        dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                        double val = Convert.ToDouble(lblprice.Text);
                        dgvNew.Rows[rowindex].Cells[4].Value = val;
                        dgvNew.Rows[rowindex].Cells[0].Value = rowindex + 1;
                        DgvAutoRefNo.Visible = false;

                        button6.Enabled = true;
                        pnsearch.Visible = false;
                        lblproductid.Text = string.Empty;
                        // Txtitem.Text = string.Empty;
                        lblitemcode.Text = "0";
                        lblrack.Text = "0";
                        lbldisplay.Text = "0";
                        lbldemo.Text = "0";
                        lblservice.Text = "0";
                        lbldamage.Text = "0";
                        lblprice.Text = "0";
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
                    }
                    else
                    {
                        MessageBox.Show("Please Enter Correct Product Name");
                    }
                }
                else
                {
                    //this.ActiveControl = btnSave;
                    pnsearch.Visible = false;
                    //MessageBox.Show("Please Enter Product Name");
                    //Txtitem.Focus();
                }
            }







            else if (e.KeyData == Keys.Up)
            {
                if (DgvAutoRefNo.CurrentCell.RowIndex != 0)
                {
                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex - 1].Cells[0].Value);
                    getitems(sa);
                }



            }
            else if (e.KeyData == Keys.Down)
            {
                if (DgvAutoRefNo.CurrentCell.RowIndex + 1 != DgvAutoRefNo.Rows.Count)
                {
                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex + 1].Cells[0].Value);
                    getitems(sa);
                }
            }



        }



        public void getitems(string sa)
        {
            dtitems = Program.dtitems;
            var rows = from row in dtitems.AsEnumerable()
                       where row.Field<string>("ProductName").Trim() == sa.Trim()
                       select row;
            DataTable st = rows.CopyToDataTable();

            if (st.Rows.Count > 0)
            {
                lblitem.Text = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);

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

                lblprice.Text = Convert.ToString(st.Rows[0]["Price"]);
                if (lblprice.Text == "")
                {
                    lblprice.Text = "0";
                }

                DefaultFloor.Text = Convert.ToString(st.Rows[0]["DefaultFloor"]);
                if (DefaultFloor.Text == "")
                {
                    DefaultFloor.Text = "0";
                }

                Checking.Text = Convert.ToString(st.Rows[0]["Checking"]);
                if (Checking.Text == "")
                {
                    Checking.Text = "0";
                }


                Display.Text = Convert.ToString(st.Rows[0]["Display"]);

                if (Display.Text == "")
                {
                    Display.Text = "0";
                }


                Damage.Text = Convert.ToString(st.Rows[0]["Damage"]);
                if (Damage.Text == "")
                {
                    Damage.Text = "0";
                }

                Delivery.Text = Convert.ToString(st.Rows[0]["Delivery"]);
                if (Delivery.Text == "")
                {
                    Delivery.Text = "0";
                }





                //pictureBox1.ImageLocation = Path.GetFullPath("131353W24J150-43D6.jpg");
                //pictureBox1.ImageLocation = itemdetails.ToList()[0].imagepath;




            }
            else
            {

                lblitemcode.Text = "0";
                lblproductid.Text = "0";
                lblprice.Text = "0";

                lblrack.Text = "0";
                lbldisplay.Text = "0";


            }
        }

        private void DgvAutoRefNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Up) || e.KeyChar != Convert.ToChar(Keys.Down))
            {
                Txtitem.Focus();
                SendKeys.Send(e.KeyChar.ToString());
                lblhiddenproduct.Text = Txtitem.Text;
                //if (DgvAutoRefNo.Rows.Count > 0)
                //{
                //    DgvAutoRefNo.Rows[0].Cells[0].Selected = false;
                //}

            }
        }

        private void TxtMobilenumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TxtMobilenumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                Btnmobilenumber.PerformClick();
            }
        }

        private void txtlesspwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void dgvFloorCheckOut_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }
        public void saveReturnapproval(string s,DataTable dt)
        {

            // Pnloading3.Visible = true;
            string output = objQuotationbal.savereturnapproval(s, txtReturnapprovalEstNo.Text,dt);

            if (output == "1")
            {
                // Pnloading3.Visible = false;
                clear();

            }


        }
        private void btnReturnApproved_Click(object sender, EventArgs e)
        {
            bool vali = ValidationReturnApproval();
            if (vali)
            {
                DataTable dt = DataGridView2DataTable(dgvReturnApproval);
                for (int i = 0; i < 3; i++)
                {
                    dt.Columns.RemoveAt(0);
                }
                RemoveNullColumnFromDataTable(dt);

                saveReturnapproval("Approved",dt);
                searchReturnapproved("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
            }
            else
            {
                MessageBox.Show("Please Select Item");
            }
        }

        private void btnReturnRejected_Click(object sender, EventArgs e)
        {
            bool vali = ValidationReturnApproval();
            if (vali)
            {
                DataTable dt = DataGridView2DataTable(dgvReturnApproval);
                for (int i = 0; i < 3; i++)
                {
                    dt.Columns.RemoveAt(0);
                }
                RemoveNullColumnFromDataTable(dt);
                saveReturnapproval("Rejected", dt);
                searchReturnapproved("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
            }
            else
            {
                MessageBox.Show("Please Select Item");
            }
        }

        private void txtless_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back) || (e.KeyChar == '.')))
                e.Handled = true;
            if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar))
                                && (e.KeyChar != '.'))
                e.Handled = true;


            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            // only allow minus sign at the beginning
            //if (e.KeyChar == '-' && (sender as TextBox).Text.Length > 0)
            //{
            //    e.Handled = true;
            //}
        }

        public void GetReport1(string QuotationId)
        {
            //try
            //{
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


                        SqlCommand cmd = new SqlCommand("proc_GetReceiptReport", con);
                        cmd.Parameters.AddWithValue("@ReceiptId", QuotationId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);





                        ReceiptDal Obj = new ReceiptDal();
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


            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message.ToString());
            //}
        }
       

        private void btnPrintReceipt_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ReceiptId))
                {
                    GetReport1(ReceiptId);
                    //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //if (result == DialogResult.Yes)
                    //{
                    //    ReceiptReportView objReceiptReportView = new ReceiptReportView(ReceiptId, "");
                    //    objReceiptReportView.Show();
                    //}
                }
                else
                {
                    MessageBox.Show("Receipt not available");
                }
            }
            catch
            {

            }
        }

        private void Cmbless_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Cmbless.SelectedIndex == 0)
                {

                    if (!string.IsNullOrEmpty(txtless.Text))
                    {

                        double total = Convert.ToDouble(lbltotalamount.Text);
                        double less = Convert.ToDouble(txtless.Text);
                        double others = Convert.ToDouble(Txtothers.Text);
                        double grandtotal = total + others - less;
                        lblTotal.Text = String.Format("{0:00.00}", grandtotal);



                    }
                    else
                    {
                        lblTotal.Text = String.Format("{0:00.00}", lbltotalamount.Text);
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(txtless.Text))
                    {

                        double total = Convert.ToDouble(lbltotalamount.Text);
                        double less = Convert.ToDouble(txtless.Text);
                        double others = Convert.ToDouble(Txtothers.Text);
                        double grandtotal = total + others + less;
                        lblTotal.Text = String.Format("{0:00.00}", grandtotal);



                    }
                    else
                    {
                        lblTotal.Text = String.Format("{0:00.00}", lbltotalamount.Text);
                    }
                }
            }
            catch
            {

            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {


                //if (!string.IsNullOrEmpty(Txtothers.Text))
                //{
                if (Cmbless.SelectedIndex == 0)
                {


                    double total = Convert.ToDouble(lbltotalamount.Text);
                    if (string.IsNullOrEmpty(Convert.ToString(txtless.Text)))
                    {
                        txtless.Text = "0";

                        txtless.SelectionStart = 0;
                        txtless.SelectionLength = txtless.Text.Length;
                    }
                    if (string.IsNullOrEmpty(Convert.ToString(Txtothers.Text)))
                    {
                        Txtothers.Text = "0";
                        Txtothers.SelectionStart = 0;
                        Txtothers.SelectionLength = Txtothers.Text.Length;
                    }
                    double less = Convert.ToDouble(txtless.Text);
                    double others = Convert.ToDouble(Txtothers.Text);
                    double grandtotal = total + others - less;
                    lblTotal.Text = String.Format("{0:00.00}", grandtotal);
                }
                else
                {
                    double total = Convert.ToDouble(lbltotalamount.Text);
                    if (string.IsNullOrEmpty(Convert.ToString(txtless.Text)))
                    {
                        txtless.Text = "0";

                        txtless.SelectionStart = 0;
                        txtless.SelectionLength = txtless.Text.Length;
                    }
                    if (string.IsNullOrEmpty(Convert.ToString(Txtothers.Text)))
                    {
                        Txtothers.Text = "0";
                        Txtothers.SelectionStart = 0;
                        Txtothers.SelectionLength = Txtothers.Text.Length;
                    }
                    double less = Convert.ToDouble(txtless.Text);
                    double others = Convert.ToDouble(Txtothers.Text);
                    double grandtotal = total + others + less;
                    lblTotal.Text = String.Format("{0:00.00}", grandtotal);
                }



                //}
                //else
                //{
                //    lblTotal.Text = String.Format("{0:00.00}", lbltotalamount.Text);
                //}
            }
            catch
            {

            }
        }

        private void DgvCkin_KeyDown(object sender, KeyEventArgs e)
        {
            if (DgvCkin.Rows.Count > 0)
            {
                if (DgvCkin.CurrentCell.RowIndex == DgvCkin.Rows.Count - 1)
                {
                    if (e.KeyData == Keys.Enter)
                    {
                        btnsave.Focus();
                    }
                }
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {

        }

        private void lblcardammount_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            //    e.Handled = true;
            if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar)) )
                e.Handled = true;


            //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            //{
            //    e.Handled = true;
            //}

            // only allow one decimal point
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = true;
            //}
        }

        private void lblcardammount_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Convert.ToString(lblcardammount.Text)))
            {
                lblcardammount.Text = "0";

                lblcardammount.SelectionStart = 0;
                lblcardammount.SelectionLength = lblcardammount.Text.Length;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel22.Visible = true;
            textBox3.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Password should not be empty");
                this.ActiveControl = textBox3;
                return;
            }
            DataTable dt = LoginBAL.GetLesserDetials(textBox3.Text, "PRICE");
            if (dt.Rows.Count > 0)
            {
                textBox3.Text = string.Empty;
                panel22.Visible = false;
                this.dgvNew.Columns["Rate"].ReadOnly = false;
                dgvNew.Focus();
                dgvNew.CurrentCell = dgvNew[4, 0];
                dgvNew.BeginEdit(true);
            }
            else
            {
                MessageBox.Show("Authentication Failed");
                this.dgvNew.Columns["Rate"].ReadOnly = true;
                textBox3.Focus();

            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData==Keys.Enter)
            {
                button4.PerformClick();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel22.Visible = false;
            textBox3.Text = string.Empty;
        }

     

    }
}

