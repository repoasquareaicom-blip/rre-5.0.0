
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


namespace Inventory.Sales
{
    public partial class SalesReturn : Form
    {
        bool load = false;
        string role1 = string.Empty;
        string srole = string.Empty;
        QuotationBal objQuotationbal = new QuotationBal();
        DataTable dtreceivedbalance, dtpaidbalance;
        ComboBox cmblocation;
        DataTable dtitems;
        TextBox tb, tbamount, tbbaalanceanount, tborderquantoty;
        public bool edit = false;
        string clickstatus = string.Empty;
        string selectedtab = string.Empty;
        string cas = string.Empty;
        int ProdSelRowvalue = 0;
        bool res = false;
        bool ProdNotFoundMSg = false;
        public SalesReturn()
        {
            InitializeComponent();

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

            Getapproval("");

            LoadPortsChecking();

            bindLocation();

            cmbloaction.SelectedIndex = 0;
            Globeimage();
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();

            ddlpaymenttype.SelectedIndex = 0;

            DataTable dtitems = Program.dtitems;
            if (dtitems == null)
            {
                itemdetails("");
            }






            SearchPurchaseOrder();




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

            //    MainTabSalesBill.TabPages.Remove(TabPayment);


            //    MainTabSalesBill.TabPages.Remove(TabChecking);

            //    btnPrint.Enabled = true;
            //    btnNew.Enabled = true;
            //    btnsave.Enabled = true;
            //    btnSavePending.Enabled = true;
            //    btnClear.Enabled = true;

            //}
            //else if (Program.Userrole == "Checkout")
            //{

            //    MainTabSalesBill.TabPages.Remove(TabPayment);

            //    MainTabSalesBill.TabPages.Remove(TabNew);
            //    MainTabSalesBill.TabPages.Remove(TabChecking);

            //    btnsave.Enabled = true;
            //    btnClear.Enabled = true;
            //}

            //else if (Program.Userrole == "credit")
            //{

            //    MainTabSalesBill.TabPages.Remove(TabNew);
            //    MainTabSalesBill.TabPages.Remove(TabChecking);


            //    MainTabSalesBill.TabPages.Remove(TabPayment);
            //    btnPrint.Enabled = false;
            //    btnNew.Enabled = false;
            //    btnsave.Enabled = false;
            //    btnSavePending.Enabled = false;
            //    btnClear.Enabled = true;
            //}
            //else if (Program.Userrole == "Payment")
            //{

            //    MainTabSalesBill.TabPages.Remove(TabNew);
            //    MainTabSalesBill.TabPages.Remove(TabChecking);


            //    btnPrint.Enabled = false;
            //    btnNew.Enabled = false;
            //    btnsave.Enabled = false;
            //    btnSavePending.Enabled = false;
            //    btnClear.Enabled = true;
            //}
            //else if (Program.Userrole == "Check")
            //{

            //    MainTabSalesBill.TabPages.Remove(TabNew);
            //    MainTabSalesBill.TabPages.Remove(TabPayment);


            //    btnPrint.Enabled = false;
            //    btnNew.Enabled = false;
            //    btnsave.Enabled = true;
            //    btnSavePending.Enabled = false;
            //    btnClear.Enabled = true;
            //}
            //else if (Program.Userrole == "Delivered")
            //{

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
                search("Returnid", "", "sr.Updatedon", "Today", "CustomerName", "", role1, Program.userid);
                DataTable dt = bindEstimation();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Returnid";
                cmbstatus1.ValueMember = "Returnid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Returnid";
                cmbstatus2.ValueMember = "Returnid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Returnid";
                cmbstatus3.ValueMember = "Returnid";
                btnsave.Enabled = true;
                btnSavePending.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = true;
            }



            else if (selectedtab == "TabChecking")
            {

                searchcheck("Returnid", "", "sr.Updatedon", "Today", "CustomerName", "", role1, Program.userid);

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

                btnsave.Enabled = true;
                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
            }


            DataTable dt1 = Program.Dtmenu;
            bool contains = dt1.AsEnumerable()
                .Any(row => "SalesReturnApproval" == row.Field<String>("Data"));
            if (contains == false)
            {
                MainTabSalesBill.TabPages.Remove(TabPayment);
            }



            bool contains1 = dt1.AsEnumerable()
                .Any(row => "SalesReturnCheckin" == row.Field<String>("Data"));
            if (contains1 == false)
            {
                MainTabSalesBill.TabPages.Remove(TabChecking);
            }

        }

        public void Globeimage()
        {
            ////string pathname = Path.Combine(Environment.CurrentDirectory);
            //string pathname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
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



            //    pictureBox4.Image = Image.FromStream(ms);
            //    pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;

            //    pictureBox5.Image = Image.FromStream(ms);
            //    pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;


            //}
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
                this.dgvSearch.Columns[1].Visible = false;
                this.dgvSearch.Columns[2].Visible = false;

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
            this.dgvNew.Columns[0].Width = 15;
            this.dgvNew.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvNew.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvNew.Columns[1].Width = 100;
            this.dgvNew.Columns[2].Width = 15;
            this.dgvNew.Columns[0].ReadOnly = true;
            this.dgvNew.Columns[1].ReadOnly = true;
            this.dgvNew.Columns[2].ReadOnly = true;
            this.dgvNew.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvNew.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvNew.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            this.dgvNew.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvNew.Columns[4].Width = 15;
            this.dgvNew.Columns[4].ReadOnly = true;

            this.dgvNew.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvNew.Columns[5].Width = 20;

            dgvNew.Columns[4].DefaultCellStyle.Format = "N2";
            dgvNew.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvNew.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvNew.Columns[6].Width = 100;

            this.dgvNew.Columns[6].ReadOnly = true;


            dgvNew.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvNew.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvNew.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvNew.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
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
            paymentdo.Rows.Add("Coins", 0, 0.00);


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





        private void LoadPortsChecking()
        {
            dgvChecking.Rows.Clear();
            dgvChecking.ColumnCount = 6;


            dgvChecking.Columns[0].Name = "S.NO";
            dgvChecking.Columns[1].Name = "Items";

            dgvChecking.Columns[2].Name = "originalQuantity";
            dgvChecking.Columns[5].Name = "Quantity";
            dgvChecking.Columns[4].Name = "Productid";

            dgvChecking.Columns[3].Name = "Rack";

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





            DataGridViewComboBoxColumn name = new DataGridViewComboBoxColumn();
            name.HeaderText = "Location";
            name.DataPropertyName = "Location";
            name.FlatStyle = FlatStyle.Popup;
            dgvChecking.Columns.Insert(3, name);
            //this.dgvChecking.Columns[3].Width = 150;


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {


                this.dgvChecking.Columns[0].Width = 45;
                this.dgvChecking.Columns[1].Width = 300;
                this.dgvChecking.Columns[3].Width = 100;
                this.dgvChecking.Columns[5].Width = 250;


            }
            else
            {


                dgvChecking.Columns[0].Width = 50;
                dgvChecking.Columns[1].Width = 400;
                dgvChecking.Columns[3].Width = 150;
                dgvChecking.Columns[4].Width = 70;
            }

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

            dgvChecking.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvChecking.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }

        private void MainTabSalesBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedtab = MainTabSalesBill.SelectedTab.Name;

            selectedtab = MainTabSalesBill.SelectedTab.Name;

            if (selectedtab == "TabNew")
            {
                search("Returnid", "", "sr.Updatedon", "Today", "CustomerName", "", role1, Program.userid);
                DataTable dt = bindEstimation();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Returnid";
                cmbstatus1.ValueMember = "Returnid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Returnid";
                cmbstatus2.ValueMember = "Returnid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Returnid";
                cmbstatus3.ValueMember = "Returnid";
                btnsave.Enabled = true;
                btnSavePending.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = true;
            }


            else if (selectedtab == "TabPayment")
            {
                search("Returnid", "", "sr.Updatedon", "Today", "CustomerName", "", role1, Program.userid);
                DataTable dt = bindEstimation();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Returnid";
                cmbstatus1.ValueMember = "Returnid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Returnid";
                cmbstatus2.ValueMember = "Returnid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Returnid";
                cmbstatus3.ValueMember = "Returnid";

                btnSavePending.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = true;
                Getapproval("");
            }



            else if (selectedtab == "TabChecking")
            {

                searchcheck("Returnid", "", "sr.Updatedon", "Today", "CustomerName", "", role1, Program.userid);

                DataTable dt = bindcheckout();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Returnid";
                cmbstatus1.ValueMember = "Returnid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Returnid";
                cmbstatus2.ValueMember = "Returnid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Returnid";
                cmbstatus3.ValueMember = "Returnid";

                btnsave.Enabled = true;
                btnSavePending.Enabled = true;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
                panel9.Enabled = true;
                btnsave.Enabled = true;
            }



            //if (selectedtab == "TabNew")
            //{
            //    LoadPortsNew();
            //}
            //if (selectedtab == "TabCreditApproval")
            //{
            //    LoadPortsCreditApproval();
            //}
            //if (selectedtab == "TabPayment")
            //{
            //    LoadPortsPayment();
            //}
            ////if (MainTabSalesBill.SelectedIndex == 3)
            ////{
            ////    LoadPortsPaymentReceived();
            ////}
            //if (selectedtab == "TabFloorCheckOut")
            //{
            //    LoadPortsFloorCheckOut();
            //}
            //if (selectedtab == "TabChecking")
            //{
            //    LoadPortsChecking();
            //}
            //if (selectedtab == "TabDelivery")
            //{
            //    LoadPortsDelivery();
            //}
        }
        #endregion


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
                lblproductid.Text = string.Empty;
                //Txtitem.Text = string.Empty;
                lblitemcode.Text = "0";
                // lblrack.Text = "0";
                lbldisplay.Text = "0";
                lbldemo.Text = "0";
                lblservice.Text = "0";
                lbldamage.Text = "0";
                lblprice.Text = "0";
            }

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
            // var combined = string.Join(", ", arr);
            //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            // str.Add(combined);
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



        private void SearchPurchaseOrder()
        {

            dgvSearch.Rows.Clear();
            dgvSearch.Columns.Clear();
            dgvSearch.ColumnCount = 3;


            dgvSearch.Columns[0].Name = "Order No";
            dgvSearch.Columns[1].Name = "Customer Name";
            dgvSearch.Columns[2].Name = "Date";




            this.dgvSearch.Columns[0].Width = 60;

            this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvSearch.Columns[1].Width = 60;




            this.dgvSearch.Columns[2].Width = 60;






            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn c in dgvSearch.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            this.dgvSearch.Columns[1].Visible = false;
            this.dgvSearch.Columns[2].Visible = false;


            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        }

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


            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + msg);
                Status = false;

            }
            return Status;


        }
        #endregion





        private void SalesBillNew_Load(object sender, EventArgs e)
        {

            this.ActiveControl = Txtcustomername;

            Txtcustomername.Text = string.Empty;
            load = true;
            //if(string.IsNullOrEmpty(Txtcustomername.Text))
            //{
            //    load = false;
            //}

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
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

                            }
                            pnsearch.Visible = false;
                            return true;
                        }

                        if (dgvNew.Rows.Count == 0)
                        {
                            dgvNew.Rows.Add();
                        }

                    }

                }


                if (Txtcustomername.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        //this.ActiveControl = txtRemarks;
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[1, dgvNew.CurrentCell.RowIndex];
                        this.ActiveControl = dgvNew;
                        //dgvOrder.BeginEdit(true);
                        //pnsearch.Visible = false;
                        return true;
                    }
                }




                if (button2.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        //this.ActiveControl = txtRemarks;
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[dgvNew.CurrentCell.ColumnIndex + 1, dgvNew.CurrentCell.RowIndex];
                        //this.ActiveControl = dgvNew;
                        this.ActiveControl = txtRemarks;
                        //dgvOrder.BeginEdit(true);
                        pnsearch.Visible = false;
                        return true;
                    }
                }


                if (txtRemarks.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = btnsave;
                        btnsave.Focus();
                        return true;
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


                if (keyData == Keys.Escape)
                {
                    if (pnsearch.Visible)
                    {
                        pnsearch.Visible = false;
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
                    }
                    else
                    {
                        if (dgvOrder.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[0].Cells[1].Value)))
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


                if (txtless.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        btnsave.Focus();


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

                if (keyData == Keys.Escape)
                {
                    pnsearch.Visible = false;
                    dgvNew.Focus();
                    dgvNew.CurrentCell = dgvNew[2, dgvNew.CurrentCell.RowIndex];
                    return true;
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



            }
            catch
            {

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void btncreditsave_Click(object sender, EventArgs e)
        {
            if (selectedtab == "TabNew")
            {
                bool vali = Validation1();
                if (vali)
                {
                    save(2);
                    search("Returnid", "", "sr.Updatedon", "Today", "CustomerName", "", role1, Program.userid);
                }
            }


            if (selectedtab == "TabChecking")
            {
                bool vali = Validationchecking();
                if (vali)
                {
                    savechecking();
                    searchcheck("Returnid", "", "sr.Updatedon", "Today", "CustomerName", "", role1, Program.userid);

                }
                else
                {
                    MessageBox.Show("Quantity Should Not Be Empty");
                }

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
                if (string.IsNullOrEmpty(Convert.ToString(dgvChecking.Rows[i].Cells[6].Value)))
                {
                    status = false;
                }

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




            //if (string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[0].Cells[1].Value)) && string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[0].Cells["Quantity"].Value)))
            //{
            //    i++;
            //    message = message + "* Please Enter Product" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = dgvNew;
            //}




            if (dgvNew.Rows.Count > 0)
            {
                i++;
                string Items = Convert.ToString(dgvNew.Rows[0].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvNew.Rows[0].Cells["Items"].Value);
                if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                {
                    message = message + "* Please Enter Product" + "\n";
                }

                if (i == 1)
                    this.ActiveControl = dgvNew;
            }
            else if (dgvNew.Rows.Count == 0)
            {
                i++;
                message = message + "* Please Enter Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvNew;
            }

            bool sas = false;

            for (int k = 0; k < dgvNew.RowCount; k++)
            {
                string Items = Convert.ToString(dgvNew.Rows[k].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvNew.Rows[k].Cells["Items"].Value);

                if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))) || Items == ".")
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
                Txtcustomername.SelectedIndex = 0;
                dgvNew.Rows.Clear();
                lbltotalamount.Text = "0.00";
                txtless.Text = "0.00";
                lblTotal.Text = "0.00";
                lblpaid.Text = "0.00";
                lblbalance.Text = "0.00";
                Txtcustomername.Focus();
                txtRemarks.Text = string.Empty;
                cmbloaction.SelectedIndex = 0;
                panel9.Enabled = true;
                btnsave.Enabled = true;
                Txtcustomername.Text = string.Empty;
            }


            else if (selectedtab == "TabChecking")
            {
                Txtcheckcustmername.Text = string.Empty;
                txtcheckingorderno.Text = string.Empty;

                dgvChecking.Rows.Clear();



            }

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            clear();
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
            dt = objQuotationbal.itemSalesreturnorderno();
            DataRow dr = dt.NewRow();
            dr["Returnid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);

            return dt;
        }

        public DataTable bindcheckout()
        {
            DataTable dt = new DataTable();
            dt = objQuotationbal.getSalesreturncheckorderno();
            DataRow dr = dt.NewRow();
            dr["Returnid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);
            return dt;
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
                pnsearch.Visible = true;
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

            try
            {
                double rate = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value);
                if (!String.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value)))
                {
                    double amt = 0;
                    if (!string.IsNullOrEmpty(Convert.ToString(tb.Text)))
                    {
                        amt = rate * Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value);
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


            //if (e.ColumnIndex == 6)
            //{
            //try
            //{
            //    int rate = Convert.ToInt32(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value);
            //    int amt = rate * Convert.ToInt32(tb.Text);
            //    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = amt;

            //}
            //catch
            //{
            //    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = 0;
            //}

            total();
            //}
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
                    tb.TextChanged += new EventHandler(textbox_Change);
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.MaxLength = 10;
                }
            }
        }

        private void textbox_Change(object sender, EventArgs e)
        {

        }

        private void dgvNew_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (dgvNew.Rows.Count > 0)
                {
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
            double totalamount = 0, totalquantity = 0;
            for (int i = 0; i < dgvNew.Rows.Count; i++)
            {
                totalamount = totalamount + Convert.ToDouble(dgvNew.Rows[i].Cells[6].Value);
                //totalquantity = totalquantity + Convert.ToInt32(dgvNew.Rows[i].Cells[5].Value);
            }

            //lbltotalquantity.Text = Convert.ToString(totalquantity);

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

            lbltotalamount.Text = String.Format("{0:00.00}", totalamount);

            double total = Convert.ToDouble(lbltotalamount.Text);
            double less = Convert.ToDouble(txtless.Text);
            double grandtotal = less - total;



            lblTotal.Text = String.Format("{0:00.00}", grandtotal);



        }

        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
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

        public void RemoveNullColumnFromDataTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][3])))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }
        public void save(int v)
        {

            //Pnloading.Visible = true;
            DataTable dt = new DataTable();

            objQuotationbal.Customerid = Txtcustomername.Text;
            objQuotationbal.Paid = lblpaid.Text;
            objQuotationbal.Balance = lblbalance.Text;
            objQuotationbal.Remarks = txtRemarks.Text;
            objQuotationbal.Totalreturn = lbltotalamount.Text;

            objQuotationbal.status = "Salesreturn Completed";

            objQuotationbal.Updatedby = Program.userid;
            dt = DataGridView2DataTable(dgvNew);

            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            RemoveNullColumnFromDataTable(dt);

            string output = objQuotationbal.SaveSalesreturn(objQuotationbal, dt);
            if (output == "1")
            {

                //Pnloading.Visible = false;
                //MessageBox.Show("save successfully");
                clear();
            }
            //bindpending();
            //search("Quotationid", "", "customername", "", "r.Name", "", userid);
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

                            // string[] rr = txtsearch1.Text.Split('-');
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

                            //string[] rr = txtsearch2.Text.Split('-');
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
                            //string[] rr = txtsearch3.Text.Split('-');
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
                    firstname1 = "CustomerName";
                    firstvalue1 = firstvalue;
                }
                else if (firstname == "Order Date")
                {
                    secondname1 = "sr.Updatedon";
                    secondvalue1 = firstvalue;
                }
                else if (firstname == "Order Number")
                {
                    if (selectedtab == "TabNew")

                        thirdname1 = "Returnid";

                    else if (selectedtab == "TabFloorCheckOut")

                        thirdname1 = "Returnid";
                    thirdvalue1 = firstvalue;
                }

                if (secondname == "Customer")
                {
                    firstname1 = "CustomerName";
                    firstvalue1 = secondvalue;
                }
                else if (secondname == "Order Date")
                {
                    secondname1 = "sr.Updatedon";
                    secondvalue1 = secondvalue;
                }
                else if (secondname == "Order Number")
                {
                    if (selectedtab == "TabNew")

                        thirdname1 = "Returnid";

                    else if (selectedtab == "TabFloorCheckOut")

                        thirdname1 = "Returnid";

                    thirdvalue1 = secondvalue;
                }

                if (thirdname == "Customer")
                {
                    firstname1 = "CustomerName";
                    firstvalue1 = thirdvalue;
                }
                else if (thirdname == "Order Date")
                {
                    secondname1 = "sr.Updatedon";
                    secondvalue1 = thirdvalue;
                }
                else if (thirdname == "Order Number")
                {
                    thirdname1 = "Returnid";
                    thirdvalue1 = thirdvalue;
                }


                if (selectedtab == "TabNew" || selectedtab == "TabPayment")
                {
                    search(firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }


                else if (selectedtab == "TabChecking")
                {
                    searchcheck(firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }


            }
        }

        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchsalesreturn(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
                this.dgvSearch.Columns[1].Visible = true;
                this.dgvSearch.Columns[2].Visible = true;

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

            txtRemarks.Focus();
        }

        public void getEstimation(string s)
        {
            DataSet ds = objQuotationbal.getSalesreturn(s);
            if (ds.Tables[0].Rows.Count > 0)
            {

                Txtcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["Customerid"]);
                lblpaid.Text = Convert.ToString(ds.Tables[0].Rows[0]["Paid"]);
                lblbalance.Text = Convert.ToString(ds.Tables[0].Rows[0]["Balance"]);
                txtless.Text = lblbalance.Text;
                txtRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
                clear();
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

                    double qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    dgvNew.Rows[i].Cells[4].Value = qty;
                    dgvNew.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);

                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvNew.Rows[i].Cells[6].Value = amt;
                }
                panel9.Enabled = false;
                btnsave.Enabled = false;
            }
            else
            {
                dgvNew.Rows.Clear();
                panel9.Enabled = true;
                btnsave.Enabled = true;
            }

        }

        public void Getapproval(string s)
        {
            DataTable ds = objQuotationbal.Getapproval(s);
            dgvPayment.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);
            dgvPayment.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn c in dgvPayment.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }


            dgvPayment.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvPayment.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvPayment.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


            if (ds.Rows.Count > 0)
            {
                dgvPayment.DataSource = ds;
                //panel9.Enabled = false;
                btnsave.Enabled = false;


                DataGridViewButtonColumn bcol = new DataGridViewButtonColumn();
                bcol.HeaderText = "Approval";
                bcol.Text = "Approved";
                bcol.Name = "Approval";
                bcol.FlatStyle = FlatStyle.Popup;

                bcol.UseColumnTextForButtonValue = true;

                DataGridViewButtonColumn bcol1 = new DataGridViewButtonColumn();
                bcol1.HeaderText = "Reject";
                bcol1.Text = "Reject";
                bcol1.Name = "Reject";
                bcol1.FlatStyle = FlatStyle.Popup;

                bcol1.UseColumnTextForButtonValue = true;

                if (dgvPayment.Columns["Reject"] == null)
                {
                    dgvPayment.Columns.Insert(7, bcol1);
                }

                if (dgvPayment.Columns["Approval"] == null)
                {
                    dgvPayment.Columns.Insert(7, bcol);
                }

            }
            else
            {

                dgvPayment.DataSource = null;
                //panel9.Enabled = false;
                btnsave.Enabled = false;
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
                            getEstimation(s);
                            total();
                            Txtcustomername.Focus();
                        }
                        else
                        {
                            clear();
                        }

                    }
                }





                else if (selectedtab == "TabPayment")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        Getapproval(s);

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

                    }
                    else
                    {
                        clear();
                    }


                }

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
        public void itemdetails(string s)
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
        }

        public void searchcheckout(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchCheckout(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }


        public void searchPaymentmode(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchPaymentmode(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }


        public void savechecking()
        {

            // Pnloading5.Visible = true;
            DataTable st = getpay(dgvChecking);
            string output = objQuotationbal.savereturnchecking(txtcheckingorderno.Text, Program.userid, st);
            if (output == "1")
            {
                // Pnloading5.Visible = false;
                clear();

            }


        }

        private void Txtitem_KeyDown_1(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.Enter)
            //{
            //    if (!string.IsNullOrEmpty(Txtitem.Text))
            //    {
            //        if (ProdNotFoundMSg)
            //        {
            //            //LblProdNotFoundMSg.Visible = true;
            //        }
            //        else
            //        {
            //            int rowindex = Convert.ToInt32(lblrowindex.Text);
            //            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
            //            dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
            //            dgvNew.Rows[rowindex].Cells[1].Value = cas.ToUpper();
            //            dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
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

            //            dgvNew.Focus();
            //            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
            //            dgvNew.BeginEdit(true);
            //            // LblProdNotFoundMSg.Visible = false;
            //        }
            //    }
            //    else
            //    {
            //        this.ActiveControl = btnsave;
            //        pnsearch.Visible = false;
            //        //MessageBox.Show("Please Enter Product Name");
            //        //Txtitem.Focus();
            //    }

            //}
        }

        public void searchcreditapproved(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchcreditapproved(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }


            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }





        public void searchpay(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchwindowpay(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }






        public void searchcheck(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchsalesreturncheck(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);


            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        public void getcheck(string s)
        {
            DataSet ds = objQuotationbal.bindreturncheck(s);

            if (ds.Tables[0].Rows.Count > 0)
            {

                txtcheckingorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Returnid"]);
                Txtcheckcustmername.Text = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);


                //Pnloading5.Enabled = false;
            }
            else
            {
                // Pnloading5.Enabled = true;
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
                    dgvChecking.Rows[i].Cells["Productid"].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);


                    string vals = Convert.ToString(ds.Tables[1].Rows[i]["Location"]);
                    DataTable dt = getdatatable(vals);

                    (dgvChecking.Rows[i].Cells[3] as DataGridViewComboBoxCell).DataSource = dt;
                    (dgvChecking.Rows[i].Cells[3] as DataGridViewComboBoxCell).ValueMember = "id";
                    (dgvChecking.Rows[i].Cells[3] as DataGridViewComboBoxCell).DisplayMember = "location";
                    (dgvChecking.Rows[i].Cells[3] as DataGridViewComboBoxCell).Value = dt.Rows[0][0];
                    string val = Convert.ToString(dt.Rows[0][0]);
                    string val1 = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    string v = getrack(val, val1);
                    dgvChecking.Rows[i].Cells["Rack"].Value = v;




                    dgvChecking.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvChecking.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgvChecking.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                    dgvChecking.DefaultCellStyle.BackColor = Color.Gainsboro;
                    dgvChecking.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

                    Rectangle resolution = Screen.PrimaryScreen.Bounds;
                    int w = resolution.Width;
                    int h = resolution.Height;

                    if (w == 1024 && h == 768)
                    {


                        this.dgvChecking.Columns[0].Width = 45;
                        this.dgvChecking.Columns[1].Width = 300;
                        this.dgvChecking.Columns[3].Width = 100;
                        this.dgvChecking.Columns[5].Width = 250;


                    }
                    else
                    {


                        dgvChecking.Columns[0].Width = 50;
                        dgvChecking.Columns[1].Width = 600;
                        dgvChecking.Columns[3].Width = 150;
                        dgvChecking.Columns[4].Width = 70;
                    }

                }
                // Pnloading5.Enabled = false;
            }
            else
            {
                dgvChecking.Rows.Clear();
                // Pnloading5.Enabled = true;
            }

        }



        private void dgvChecking_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 6)
                {
                    if (Convert.ToString(dgvChecking.Rows[e.RowIndex].Cells[2].Value) != tborderquantoty.Text)
                    {
                        MessageBox.Show("Please Enter Correct Quantity");
                        dgvChecking.Focus();
                        edit = true;
                        dgvChecking.CurrentCell = dgvChecking[6, e.RowIndex];
                        dgvChecking.Rows[e.RowIndex].Cells[6].Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void dgvChecking_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvChecking.CurrentCell.ColumnIndex;
            string headerText = dgvChecking.Columns[column].HeaderText;

            if (headerText.Equals("Quantity"))
            {
                tborderquantoty = e.Control as TextBox;
                tborderquantoty.KeyPress += new KeyPressEventHandler(textbox_keypress);
                tborderquantoty.MaxLength = 6;
            }
            else if (headerText.Equals("Location"))
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
                    string val1 = Convert.ToString(dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].Cells["Productid"].Value);
                    string v = getrack(val, val1);
                    dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].Cells["Rack"].Value = v;
                    //dgvChecking.CurrentCell = dgvChecking[6, dgvChecking.CurrentCell.RowIndex];
                }
                //else
                //{
                //    dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].Cells["Rack"].Value = "";
                //}
            }
            catch
            {

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
            DataTable dt = objQuotationbal.searchWindowdelivery(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void dgvChecking_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dgvChecking.CurrentCell.ColumnIndex == 6)
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
            catch
            {

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


        public string getrack(string s, string s1)
        {
            string v = objQuotationbal.getrack(s, s1);
            return v;
        }

        private void dgvChecking_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
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

        private void SalesBillNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int j = 0; j < MainTabSalesBill.TabPages.Count; j++)
                {

                    for (int i = 0; i < MainTabSalesBill.TabPages[j].Controls.Count; )
                    {
                        MainTabSalesBill.TabPages[j].Controls[i].Dispose();
                    }
                    MainTabSalesBill.TabPages[j].Dispose();
                }

                MainTabSalesBill.Dispose();

                this.Dispose();
            }
            catch
            {

            }
        }

        public DataTable getpay(DataGridView dgv)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Quantity", typeof(string)),
                            new DataColumn("Rack", typeof(string)),
                            new DataColumn("Productid",typeof(string)) });

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                dt.Rows.Add(Convert.ToString(dgvChecking.Rows[i].Cells["Quantity"].Value), Convert.ToString((dgvChecking.Rows[i].Cells[3] as DataGridViewComboBoxCell).Value), Convert.ToString(dgvChecking.Rows[i].Cells["Productid"].Value));
            }

            return dt;
        }

        private void Txtcustomername_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (load)
            {
                Txtcustomername.Text = string.Empty;
                load = false;
            }
            else
            {
                load = false;
            }
            string s = "";
            if (Txtcustomername.SelectedIndex > 0)
            {
                s = Convert.ToString(Txtcustomername.SelectedValue);
            }

            DataTable dt = objQuotationbal.Getpaidamunt(s);

            lblpaid.Text = Convert.ToString(dt.Rows[0]["Paid"]);
            lblbalance.Text = Convert.ToString(dt.Rows[0]["Balance"]);
            txtless.Text = Convert.ToString(dt.Rows[0]["Balance"]);
            if (string.IsNullOrEmpty(lblpaid.Text))
            {
                lblpaid.Text = "0.00";
            }
            if (string.IsNullOrEmpty(lblbalance.Text))
            {
                lblbalance.Text = "0.00";
                txtless.Text = "0.00";
            }

        }

        private void dgvPayment_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string va = Convert.ToString(dgvPayment.Rows[e.RowIndex].Cells["Returnid"].Value);
                if (Convert.ToString(dgvPayment.Columns[e.ColumnIndex].HeaderText) == "Approval")
                {
                    int v = objQuotationbal.salesreturnapproval("SalesApproved", va);
                }
                else if (Convert.ToString(dgvPayment.Columns[e.ColumnIndex].HeaderText) == "Reject")
                {
                    int v = objQuotationbal.salesreturnapproval("Rejected", va);
                }
                Getapproval("");
            }
        }

        private void dgvChecking_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex != 6)
                {
                    if (dgvChecking[e.ColumnIndex, e.RowIndex].EditType.ToString() == "System.Windows.Forms.DataGridViewComboBoxEditingControl")
                    {
                        DataGridViewColumn column = dgvChecking.Columns[e.ColumnIndex];
                        if (column is DataGridViewComboBoxColumn)
                        {
                            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dgvChecking[e.ColumnIndex, e.RowIndex];
                            dgvChecking.CurrentCell = cell;
                            dgvChecking.BeginEdit(true);
                            DataGridViewComboBoxEditingControl editingControl = (DataGridViewComboBoxEditingControl)dgvChecking.EditingControl;
                            editingControl.DroppedDown = true;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void dgvChecking_DataError_1(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }


        private void RefScrollGrid()
        {
            if (DgvAutoRefNo.Rows.Count - 1 >= ProdSelRowvalue)
            {
                DgvAutoRefNo.FirstDisplayedScrollingRowIndex = ProdSelRowvalue;
            }
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

                    //try
                    //{
                    //    DataGridViewRow theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue - 1];
                    //    if (theRow3.Index != DgvAutoRefNo.RowCount)
                    //    {

                    //        theRow3.DefaultCellStyle.BackColor = Color.LightGray;

                    //        theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue];
                    //        theRow3.DefaultCellStyle.BackColor = Color.White;

                    //        ProdSelRowvalue--;
                    //        cas = Convert.ToString(DgvAutoRefNo[0, ProdSelRowvalue].Value);
                    //        itemdetails(cas);
                    //        RefScrollGrid();
                    //    }
                    //}
                    //catch
                    //{
                    //    //ProdSelRowvalue = 0;
                    //}

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
                    //    //ProdSelRowvalue = 0;
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
                    //            Txtitem.Text = Convert.ToString(DgvAutoRefNo[0, ProdSelRowvalue].Value);
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

                        //itemdetails(Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value));

                        dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        dgvNew.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();


                        //dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        //dgvNew.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                        dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                        double val = Convert.ToDouble(lblprice.Text);

                        dgvNew.Rows[rowindex].Cells[4].Value = val;
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

                double rate = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value);
                if (!String.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value)))
                {
                    double amt = 0;
                    if (!string.IsNullOrEmpty(Convert.ToString(tb.Text)))
                    {
                        amt = rate * Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value);
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


                //int rate = Convert.ToInt32(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value);
                //int amt = rate * Convert.ToInt32(tb.Text);
                //dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = amt;

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


                //dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = 0;
            }

            total();
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

                        pnsearch.Visible = false;
                        lblproductid.Text = string.Empty;
                        //Txtitem.Text = string.Empty;
                        lblitemcode.Text = "0";
                        //lblrack.Text = "0";
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
                //lblitem.Text = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);

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

                //  lblrack.Text = "0";
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
                if (DgvAutoRefNo.Rows.Count > 0)
                {
                    DgvAutoRefNo.Rows[0].Cells[0].Selected = false;
                }

            }
        }

        private void Txtcustomername_TextChanged(object sender, EventArgs e)
        {
            lblpaid.Text = "0.00";
            lblbalance.Text = "0.00";
        }

    }
}

