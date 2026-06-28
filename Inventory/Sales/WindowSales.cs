
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
    public partial class WindowSales : Form
    {
        string role1 = string.Empty;
        string srole = string.Empty;
        QuotationBal objQuotationbal = new QuotationBal();
        DataTable dtreceivedbalance, dtpaidbalance;
        ComboBox cmblocation;
        TextBox tb, tbamount, tbbaalanceanount, tborderquantoty;
        public bool edit = false;
        string clickstatus = string.Empty;
        string selectedtab = string.Empty;
        public WindowSales()
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

            LoadPortsPayment();
            bindAccountno();
            LoadPortsChecking();
            LoadPortsDelivery();
            bindLocation();
           
            cmbloaction.SelectedIndex = 0;
            Globeimage();
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();

            ddlpaymenttype.SelectedIndex = 0;
            ddlpaymode.SelectedIndex = 0;
            cmbbank.SelectedIndex = 0;


            paymentdenobind();
            paymentDenotoCustomerbind();

            ddlpaymode.SelectedIndex = 0;


            SearchPurchaseOrder();


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

            //    MainTabSalesBill.TabPages.Remove(TabPayment);


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

            //    MainTabSalesBill.TabPages.Remove(TabPayment);

            //    MainTabSalesBill.TabPages.Remove(TabNew);
            //    MainTabSalesBill.TabPages.Remove(TabChecking);
            //    MainTabSalesBill.TabPages.Remove(TabDelivery);
            //    btnsave.Enabled = true;
            //    btnClear.Enabled = true;
            //}

            //else if (Program.Userrole == "credit")
            //{

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

            //    MainTabSalesBill.TabPages.Remove(TabNew);
            //    MainTabSalesBill.TabPages.Remove(TabChecking);
            //    MainTabSalesBill.TabPages.Remove(TabDelivery);


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
            //    MainTabSalesBill.TabPages.Remove(TabDelivery);


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
                search("Quotationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
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


            DataTable dt1 = Program.Dtmenu;
            bool contains = dt1.AsEnumerable()
                .Any(row => "windowsalePayment" == row.Field<String>("Data"));
            if (contains == false)
            {
                MainTabSalesBill.TabPages.Remove(TabPayment);
            }

            bool contains1 = dt1.AsEnumerable()
               .Any(row => "windowsaleChecking" == row.Field<String>("Data"));
            if (contains1 == false)
            {
                MainTabSalesBill.TabPages.Remove(TabChecking);
            }

            bool contains2 = dt1.AsEnumerable()
               .Any(row => "windowsaleDelivery" == row.Field<String>("Data"));
            if (contains2 == false)
            {
                MainTabSalesBill.TabPages.Remove(TabDelivery);
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

            //    pictureBox6.Image = Image.FromStream(ms);
            //    pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
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
           // this.dgvNew.Columns[0].Width = 10;
            this.dgvNew.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvNew.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvNew.Columns[1].Width = 100;
           // this.dgvNew.Columns[2].Width = 15;
            this.dgvNew.Columns[0].ReadOnly = true;
            this.dgvNew.Columns[1].ReadOnly = true;
            this.dgvNew.Columns[2].ReadOnly = true;
            this.dgvNew.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvNew.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvNew.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            this.dgvNew.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
           // this.dgvNew.Columns[4].Width = 15;
            this.dgvNew.Columns[4].ReadOnly = true;

            this.dgvNew.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
           // this.dgvNew.Columns[5].Width = 12;

            dgvNew.Columns[4].DefaultCellStyle.Format = "N2";
            dgvNew.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvNew.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvNew.Columns[6].Width = 100;

           // this.dgvNew.Columns[6].ReadOnly = true;

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

            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvNew.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvNew.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvNew.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
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
            //this.dgvPayment.Columns[0].Width = 10;
            this.dgvPayment.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvPayment.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvPayment.Columns[1].Width = 100;
           // this.dgvPayment.Columns[2].Width = 15;
            this.dgvPayment.Columns[0].ReadOnly = true;
            this.dgvPayment.Columns[1].ReadOnly = true;
            this.dgvPayment.Columns[2].ReadOnly = true;
            this.dgvPayment.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayment.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayment.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            this.dgvPayment.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
           // this.dgvPayment.Columns[4].Width = 15;
            this.dgvPayment.Columns[4].ReadOnly = true;

            this.dgvPayment.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
           // this.dgvPayment.Columns[5].Width = 12;

            dgvPayment.Columns[4].DefaultCellStyle.Format = "N2";
            dgvPayment.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvPayment.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvPayment.Columns[6].Width = 100;

            this.dgvPayment.Columns[6].ReadOnly = true;

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvPayment.Columns[0].Width = 3;
                this.dgvPayment.Columns[1].Width = 10;
                this.dgvPayment.Columns[2].Width = 5;
                this.dgvPayment.Columns[4].Width =5;
                this.dgvPayment.Columns[5].Width =5;
                this.dgvPayment.Columns[6].Width = 70;
            }
            else
            {
                this.dgvPayment.Columns[0].Width = 15;
                this.dgvPayment.Columns[1].Width = 100;
                this.dgvPayment.Columns[2].Width = 15;
                this.dgvPayment.Columns[4].Width = 15;
                this.dgvPayment.Columns[5].Width = 15;
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
            this.dgvChecking.Columns[0].Width = 7;


            this.dgvChecking.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvChecking.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvChecking.Columns[1].Width = 120;
            this.dgvChecking.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvChecking.Columns[0].ReadOnly = true;
            this.dgvChecking.Columns[1].ReadOnly = true;


            this.dgvChecking.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvChecking.Columns[2].Visible = false;
            this.dgvChecking.Columns[4].Visible = false;

            this.dgvChecking.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvChecking.Columns[3].Width = 80;


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
            dgvDelivery.ColumnCount = 4;


            dgvDelivery.Columns[0].Name = "S.NO";
            dgvDelivery.Columns[1].Name = "Items";
            dgvDelivery.Columns[2].Name = "Quantity";

            dgvDelivery.Columns[3].Name = "Productid";


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
          




            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "ChkDelivery";
            dgvCmb.HeaderText = "Delivered";
            dgvDelivery.Columns.Insert(4, dgvCmb);
            this.dgvDelivery.Columns[4].Width = 60;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvDelivery.Columns[0].Width = 40;
                this.dgvDelivery.Columns[1].Width = 400;
                this.dgvDelivery.Columns[2].Width = 80;
            }
            else
            {
                this.dgvDelivery.Columns[0].Width = 40;
                this.dgvDelivery.Columns[1].Width = 400;
                this.dgvDelivery.Columns[2].Width = 80;

                //this.dgvDelivery.Columns[0].Width = 30;
                //this.dgvDelivery.Columns[1].Width = 400;
                //this.dgvDelivery.Columns[2].Width = 80;
                //this.dgvDelivery.Columns[3].Width = 100;
                //this.dgvDelivery.Columns[4].Width = 100;
                //this.dgvDelivery.Columns[5].Width = 100;

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
                search("Quotationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                btnPrint.Enabled = true;
                btnNew.Enabled = true;
                btnsave.Enabled = true;
                btnSavePending.Enabled = true;
                btnClear.Enabled = true;
            }
            else if (selectedtab == "TabPayment")
            {
                searchpay("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
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
        }
        #endregion


        public void AutoCompleteLoad(string s,int t)
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.itemauto(s,t);
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

            Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            Txtitem.AutoCompleteCustomSource = str;
            Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;

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
           // var combined = string.Join(", ", arr);
            //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
          //  str.Add(combined);
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
            else if (ddlpaymode.SelectedIndex == 1)
            {
                cashpl.Visible = false;
                panelCustomerpaid.Visible = false;
                panelTransaction.Visible = true;
                lblChequeNo.Visible = true;
                txttransactionid.Visible = true;
                lblcardammount.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpatroundoff.Text));
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
            ddlpaymode.SelectedIndex = 0;
            paymentdenobind();
            lblpaymenttotal.Text = "0.00";
            lblpaymentamount.Text = "0.00";
            lblpaymentbalance.Text = "0.00";


        }

        private void transactionclose_Click(object sender, EventArgs e)
        {
            panelTransaction.Visible = false;
            ddlpaymode.SelectedIndex = 0;
        }

        private void MainTabSalesBill_Click(object sender, EventArgs e)
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
                savecard();
                ddlpaymode.Enabled = false;
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

                if (cmbassistby.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        date1.Focus();


                        return true;
                    }
                }

                if (txtless.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        btnsave.Focus();


                        return true;
                    }
                }

                //if (keyData == Keys.Escape)
                //{
                //    pnsearch.Visible = false;
                //    cashdetailsclose.PerformClick();
                //    return true;
                //}


                if (keyData == Keys.Escape)
                {
                    if (pnsearch.Visible)
                    {
                        pnsearch.Visible = false;
                        cashdetailsclose.PerformClick();
                        return true;
                        //dgvOrder.Focus();
                        //dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
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


                if (date1.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[1, 0];
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
                        this.ActiveControl = txtless;
                        return true;
                    }

                }
                //if (cmbpaymode.Focused)
                //{
                //    if (keyData == (Keys.Tab))
                //    {
                //        this.ActiveControl = txtless;
                //        return true;
                //    }

                //}
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
                    search("Quotationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                }
            }


            if (selectedtab == "TabChecking")
            {
                bool vali = Validationchecking();
                if (vali)
                {
                    savechecking();
                    searchcheck("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);

                }
                else
                {
                    MessageBox.Show("Quantity Should Not Be Empty");
                }

            }

            if (selectedtab == "TabDelivery")
            {
                bool vali = Validationdelivery();
                if (vali)
                {
                    deletedelivered();
                    savedelivered("", 2);
                    searchdelivery("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                    //Pnloading6.Visible = false;
                    clear();
                }
                else
                {
                    if (dgvDelivery.Rows.Count == 1)
                    {
                        MessageBox.Show("Please Select Product");
                    }

                    else
                    {
                        DialogResult result = MessageBox.Show("Do you want to Partial Deliverd?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            deletedelivered();
                            string s = getcheckedvalue();
                            searchdelivery("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
                           // Pnloading6.Visible = false;
                            clear();

                        }
                    }
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
                if (string.IsNullOrEmpty(Convert.ToString(dgvChecking.Rows[i].Cells[3].Value)))
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



            if (string.IsNullOrEmpty(tatQuotationno.Text))
            {
                i++;
                message = message + "* Please Select Estimation" + "\n";
                if (i == 1)
                    this.ActiveControl = cmdcity;
            }

            if (string.IsNullOrEmpty(cmdcity.Text))
            {
                i++;
                message = message + "* Please Select city" + "\n";
                if (i == 1)
                    this.ActiveControl = cmdcity;
            }

            if (cmbassistby.SelectedIndex == 0)
            {
                i++;
                message = message + "* Please Select Assist " + "\n";
                if (i == 1)
                    this.ActiveControl = cmbassistby;
            }



            //if (cmbreference.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please select Reference" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmbreference;
            //}


           


            if (!string.IsNullOrEmpty(txtless.Text))
            {
                if (Convert.ToDouble(lbltotalamount.Text) <= Convert.ToDouble(txtless.Text))
                {
                    i++;
                    message = message + "* Less Amount Should Not Be Greater Than TotalAmount" + "\n";
                    if (i == 1)
                        this.ActiveControl = txtless;


                }
            }

            if (dgvNew.Rows.Count > 0)
            {
                i++;
                string Items = Convert.ToString(dgvNew.Rows[0].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvNew.Rows[0].Cells["Items"].Value);
                if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                {
                    message = message + "*.Please Select Product" + "\n";
                }

                if (i == 1)
                    this.ActiveControl = dgvNew;
            }
            else if (dgvNew.Rows.Count == 0)
            {
                i++;
                message = message + "*.Please Select Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvNew;
            }

            bool sas = false;

            for (int k = 0; k < dgvNew.RowCount; k++)
            {
                string Items = Convert.ToString(dgvNew.Rows[k].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvNew.Rows[k].Cells["Items"].Value);

                if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))))
                {
                    sas = true;
                    break;
                }
            }
            if (sas == true)
            {
                i++;
                message = message + "*Product or quantity should not be empty." + "\n";
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
                txtOrderNo.Text = string.Empty;

                txtRemarks.Text = string.Empty;
                cmbloaction.SelectedIndex = 0;
                
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

                ddlpaymode.SelectedIndex = 0;
                cmbbank.SelectedIndex = 0;
                txtcardno.Text = string.Empty;
                txttransactionid.Text = string.Empty;
          

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

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToDouble(lblpaymenttotal.Text) == 0.00)
            {
                MessageBox.Show("Please Enter Amount ");
            }
            else if (Convert.ToDouble(lblpaymentbalance.Text) <= 0.00)
            {
                //MessageBox.Show("Please Enter Amount ");
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
                    edit = true;
                    dgvNew.CurrentCell = dgvNew[6, e.RowIndex];
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
                //pnsearch.Visible = true;
                this.ActiveControl = cmbloaction;
                lblrowindex.Text = e.RowIndex.ToString();
                lblpageno.Text = Convert.ToString(Convert.ToInt32(lblpageno.Text) + 1);
            }
            else
            {
                pnsearch.Visible = false; ;
            }
        }

        private void dgvNew_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6)
            {
                total();
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
                    tb.TextChanged += new EventHandler(textbox_Change);
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.MaxLength = 10;
                }
            }
        }

        private void textbox_Change(object sender, EventArgs e)
        {
            try
            {
                int rate = Convert.ToInt32(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value);
                int amt = rate * Convert.ToInt32(tb.Text);
                dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = amt;

            }
            catch
            {
                dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = 0;
            }
        }

        private void dgvNew_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
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
            lbltotalamount.Text = String.Format("{0:00.00}", totalamount);
            lblTotal.Text = String.Format("{0:00.00}", totalamount);

            if (!string.IsNullOrEmpty(txtless.Text))
            {

                double total = Convert.ToDouble(lbltotalamount.Text);
                double less = Convert.ToDouble(txtless.Text);
                double grandtotal = total - less;
                lblTotal.Text = String.Format("{0:00.00}", grandtotal);



            }
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
            if (string.IsNullOrEmpty(txtOrderNo.Text))
            {
                objQuotationbal.isnew = 0;
            }
            else
            {
                objQuotationbal.isnew = 1;
            }
            objQuotationbal.Quotationid = tatQuotationno.Text;
            objQuotationbal.Customerid = Convert.ToString(Txtcustomername.SelectedValue);
            objQuotationbal.date = date1.Value;
            objQuotationbal.Referenceid = Convert.ToString(cmbreference.SelectedValue);
            objQuotationbal.Assist = Convert.ToString(cmbassistby.SelectedValue);
            objQuotationbal.Customername = Convert.ToString(Txtcustomername.Text);
            objQuotationbal.City = Convert.ToString(cmdcity.Text);
            objQuotationbal.Estinationid = txtOrderNo.Text;
            objQuotationbal.Remarks = txtRemarks.Text;
            objQuotationbal.Total = lbltotalamount.Text;
            objQuotationbal.Paymentmode = "Cash Bill";
            if (!string.IsNullOrEmpty(txtless.Text))
            {
                objQuotationbal.Lessamount = txtless.Text;
            }
            else
            {
                objQuotationbal.Lessamount = "0";
            }

            objQuotationbal.Grandtotal = Convert.ToString(Convert.ToDouble(lbltotalamount.Text) - Convert.ToDouble(objQuotationbal.Lessamount));

            //objQuotationbal.Grandtotal = lblTotal.Text;

            if (v == 1)
            {
                objQuotationbal.status = "Open";
            }
            else if (v == 2)
            {
                objQuotationbal.status = "WindowSale Completed";
            }
            objQuotationbal.Updatedby = Program.userid;
            dt = DataGridView2DataTable(dgvNew);
            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            RemoveNullColumnFromDataTable(dt);

            //string output = objQuotationbal.SaveEstimation(objQuotationbal, dt);
            //if (!string.IsNullOrEmpty(output) && string.IsNullOrEmpty(txtOrderNo.Text))
            //{

            //   // Pnloading.Visible = false;

            //    //MessageBox.Show("save successfully");
            //    txtOrderNo.Text = output;
            //    clear();
            //    if (v == 2)
            //    {
            //        DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //        if (result == DialogResult.Yes)
            //        {
            //            QuotationEstimationreport rpt = new QuotationEstimationreport(output);
            //            rpt.ShowDialog();
            //        }
            //    }
            //}
            //else if (!string.IsNullOrEmpty(output) && !string.IsNullOrEmpty(txtOrderNo.Text))
            //{
            //    //MessageBox.Show("Update successfully");

            //    clear();
            //    //Pnloading.Visible = false;

            //    txtOrderNo.Text = output;
            //    if (v == 2)
            //    {
            //        DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //        if (result == DialogResult.Yes)
            //        {
            //            QuotationEstimationreport rpt = new QuotationEstimationreport(output);
            //            rpt.ShowDialog();
            //        }
            //    }
            //}
            //bindpending();
            //search("Quotationid", "", "customername", "", "r.Name", "", userid);
        }

        private void txtless_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtless.Text))
            {

                double total = Convert.ToDouble(lbltotalamount.Text);
                double less = Convert.ToDouble(txtless.Text);
                double grandtotal = total - less;
                lblTotal.Text = String.Format("{0:00.00}", grandtotal);



            }
            else
            {
                lblTotal.Text = String.Format("{0:00.00}", lbltotalamount.Text);
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
                            //string[] rr = txtsearch1.Text.Split('-');
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

                    else if (selectedtab == "TabFloorCheckOut")
                        thirdname1 = "Estimationid";


                    thirdvalue1 = thirdvalue;
                }


                if (selectedtab == "TabNew")
                {
                    search(firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TabFloorCheckOut")
                {
                    searchcheckout(firstname1, firstvalue1, "Updatedon", secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TabPaymentReceived")
                {
                    searchPaymentmode("Estimationid", firstvalue1, "Updatedon", secondvalue1, "customername", thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TabCreditApproval")
                {
                    searchcreditapproved("Estimationid", firstvalue1, "Updatedon", secondvalue1, "customername", thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TabPayment")
                {
                    searchpay("Estimationid", firstvalue1, "Updatedon", secondvalue1, "customername", thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TabChecking")
                {
                    searchcheck("Estimationid", firstvalue1, "Updatedon", secondvalue1, "customername", thirdvalue1, role1, Program.userid);
                }
                else if (selectedtab == "TabDelivery")
                {
                    searchdelivery("Estimationid", firstvalue1, "Updatedon", secondvalue1, "customername", thirdvalue1, role1, Program.userid);
                }
            }
        }

        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchWindowEstimation(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

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
            Txtitem.Text = string.Empty;
            lblitemcode.Text = "0";
            lblprice.Text = "0";
            lbldisplay.Text = "0";
            lbldemo.Text = "0";
            lblservice.Text = "0";
            lbldamage.Text = "0";
            lblrack.Text = "0";
            txtRemarks.Focus();
        }

        public void getEstimation(string s)
        {
            DataSet ds = objQuotationbal.getEstimation(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                txtOrderNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
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
            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                panel9.Enabled = true;
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
                panel2.Enabled = false;
            }
            else
            {
                dgvNew.Rows.Clear();
                panel2.Enabled = true;
            }

        }


        public void getpay(string s)
        {
            DataSet ds = objQuotationbal.Bindbillpay(s);

            if (ds.Tables[0].Rows.Count > 0)
            {
                ddlpaymode.Enabled = true;
                txtpayorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
                txtpauycustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                txtpaycity.Text = Convert.ToString(ds.Tables[0].Rows[0]["City"]);
                txtpayref.Text = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);

                Txtprepareby.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);

                txtpayassist.Text = Convert.ToString(ds.Tables[0].Rows[0]["UserFullName"]);

                paydate.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);

                Txtpayquotationid.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);

                txtpayremarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                lblpaytotal.Text = Convert.ToString(ds.Tables[0].Rows[0]["Total"]);
                txtpayless.Text = Convert.ToString(ds.Tables[0].Rows[0]["LessAmount"]);
                lblpaynet.Text = Convert.ToString(ds.Tables[0].Rows[0]["GrnandTotal"]);

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
                        getpay(s);
                        ddlpaymode.Focus();
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
                    dgvNew.Rows[rowindex].Cells[1].Value = Txtitem.Text.ToUpper();
                    dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                    dgvNew.Rows[rowindex].Cells[4].Value = lblprice.Text;
                    dgvNew.Rows[rowindex].Cells[0].Value = rowindex + 1;
                    pnsearch.Visible = false;
                    lblproductid.Text = string.Empty;
                    Txtitem.Text = string.Empty;
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
        public void itemdetails()
        {

            try
            {

                string s1 = Txtitem.Text.Trim();
                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s1.Replace("'", "''");
                DataTable st = objQuotationbal.itemdetails(name, s2);


                if (st.Rows.Count > 0)
                {

                    lblitemcode.Text = Convert.ToString(st.Rows[0]["UOM"]);
                    if (lblitemcode.Text == "")
                    {
                        lblitemcode.Text = "0";
                    }


                    lblproductid.Text = Convert.ToString(st.Rows[0]["id"]);
                    if (lblproductid.Text == "")
                    {
                        lblproductid.Text = "0";
                    }

                    lblprice.Text = Convert.ToString(st.Rows[0]["price"]);
                    if (lblprice.Text == "")
                    {
                        lblprice.Text = "0";
                    }

                    lbldemo.Text = Convert.ToString(st.Rows[0]["Demo"]);
                    if (lbldemo.Text == "")
                    {
                        lbldemo.Text = "0";
                    }

                    lblrack.Text = Convert.ToString(st.Rows[0]["Rack"]);
                    if (lblrack.Text == "")
                    {
                        lblrack.Text = "0";
                    }


                    lbldisplay.Text = Convert.ToString(st.Rows[0]["Display"]);

                    if (lbldisplay.Text == "")
                    {
                        lbldisplay.Text = "0";
                    }


                    lbldamage.Text = Convert.ToString(st.Rows[0]["Demage"]);
                    if (lbldamage.Text == "")
                    {
                        lbldamage.Text = "0";
                    }

                    lblservice.Text = Convert.ToString(st.Rows[0]["service"]);
                    if (lblservice.Text == "")
                    {
                        lblservice.Text = "0";
                    }
                    //pictureBox1.ImageLocation = Path.GetFullPath("131353W24J150-43D6.jpg");
                    //pictureBox1.ImageLocation = itemdetails.ToList()[0].imagepath;




                }
                else
                {

                    lblitemcode.Text = "0";
                    lblproductid.Text = "0";
                    lblprice.Text = "0";
                    lbldemo.Text = "0";
                    lblrack.Text = "0";
                    lbldisplay.Text = "0";
                    lbldamage.Text = "0";
                    lblservice.Text = "0";

                }

            }
            catch (Exception)
            {

            }

        }

        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            itemdetails();
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
            string output = objQuotationbal.windowsalesavechecking(txtcheckingorderno.Text, Program.userid);
            if (output == "1")
            {
               // Pnloading5.Visible = false;
                clear();

            }


        }

        private void Txtitem_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(Txtitem.Text))
                {
                    if (Convert.ToInt32(lblproductid.Text) != 0)
                    {
                        int rowindex = Convert.ToInt32(lblrowindex.Text);

                        if (selectedtab == "TabNew")
                        {
                            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
                            dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                            dgvNew.Rows[rowindex].Cells[1].Value = Txtitem.Text.ToUpper();
                            dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                            double val = Convert.ToDouble(lblprice.Text);

                            dgvNew.Rows[rowindex].Cells[4].Value = val;
                            dgvNew.Rows[rowindex].Cells[0].Value = rowindex + 1;
                            pnsearch.Visible = false;
                            lblproductid.Text = string.Empty;
                            Txtitem.Text = string.Empty;
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
                        MessageBox.Show("Please Enter Correct Product Name");
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter Product Name");
                    Txtitem.Focus();
                }

            }
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

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

                    tbamount.KeyPress += new KeyPressEventHandler(textbox_keypress);
                }
            }
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

                    tbbaalanceanount.KeyPress += new KeyPressEventHandler(textbox_keypress);
                }
            }
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
                MessageBox.Show("Plese Enter Correct Balance");
                dgvCustomerpaid.Focus();
                dgvCustomerpaid.CurrentCell = dgvCustomerpaid[1, 0];
            }
            else
            {
                btncashpay.Enabled = true;
                savepayment();
                SavepaymentDenomination();
                ddlpaymode.Enabled = false;
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
            if (Convert.ToDouble(lblpaymentbalance.Text) > Convert.ToDouble(lblpaidbalance.Text))
            {
                MessageBox.Show("Please Enter Paid Balance");
                btnreceiveBalance.Focus();

            }
            else if (Convert.ToDouble(lblpaymenttotal.Text) == 0)
            {
                MessageBox.Show("Please Enter Amount ");
            }

            else
            {
                if (Convert.ToDouble(lblpaymentamount.Text) > Convert.ToDouble(lblpaymenttotal.Text))
                {
                    MessageBox.Show("Please Pay Full Amount");
                }
                else
                {
                    savepayment();
                    //Pnloading.Visible = false;
                    clear();
                    ddlpaymode.Enabled = false;
                    searchpay("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);

                }
            }
        }

        public void savepayment()
        {
            DataTable st = getpay(dgvPayment);
            double total = Convert.ToDouble(lblpaymenttotal.Text) - Convert.ToDouble(lblpaymentbalance.Text);
            //Pnloading4.Visible = true;
            objQuotationbal.transid = txtpayorderno.Text;
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
            lblid.Text = objQuotationbal.SaveWindowPayment(objQuotationbal, lblpaymenttotal.Text, st);



        }

        public void SavepaymentDenomination()
        {

           // Pnloading4.Visible = true;
            objQuotationbal.recivetransid = lblid.Text;
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

            objQuotationbal.OAmount = lblpaidbalance.Text;
            string output = objQuotationbal.SavepaymentDenomination(objQuotationbal);

            if (output == "1")
            {
                //Pnloading4.Visible = false;
                clear();
            }

        }

        public void savecard()
        {

           // Pnloading.Visible = true;
            objQuotationbal.Quotationid = txtpayorderno.Text;
            objQuotationbal.Bank = cmbbank.Text;
            objQuotationbal.Cardnumber = txtcardno.Text;
            objQuotationbal.transid = txttransactionid.Text;
            objQuotationbal.type = "Card";
            objQuotationbal.OAmount = Convert.ToString(lblpatroundoff.Text);
            string s = objQuotationbal.savecardtransaction(objQuotationbal);

            if (s == "1")
            {
                //Pnloading.Visible = false;
                clear();
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
            DataTable dt = objQuotationbal.searchWindowcheck(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

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
            DataSet ds = objQuotationbal.Bindbillpay(s);

            if (ds.Tables[0].Rows.Count > 0)
            {
                ddlpaymode.Enabled = true;
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
                    dgvDelivery.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    if (Convert.ToBoolean(ds.Tables[1].Rows[i]["isdelevered"]) == true)
                    {
                        dgvDelivery.Rows[i].Cells[4].Value = Convert.ToBoolean(ds.Tables[1].Rows[i]["isdelevered"]);
                        dgvDelivery.Rows[i].Cells[4].ReadOnly = true;
                    }
                    else
                    {
                        dgvDelivery.Rows[i].Cells[4].Value = Convert.ToBoolean(ds.Tables[1].Rows[i]["isdelevered"]);
                    }

                    //dgvDelivery.Rows[i].Cells[4].Value = Convert.ToBoolean(ds.Tables[1].Rows[i]["isdelevered"]);


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
                if (Convert.ToString(dgvChecking.Rows[e.RowIndex].Cells[2].Value) != tborderquantoty.Text)
                {
                    MessageBox.Show("Please Enter Correct Quantity");
                    dgvChecking.Focus();
                    edit = true;
                    dgvChecking.CurrentCell = dgvChecking[3, e.RowIndex];
                    dgvChecking.Rows[e.RowIndex].Cells[3].Value = "";
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
                tborderquantoty.KeyPress += new KeyPressEventHandler(textbox_keypress);
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
            DataTable dt = objQuotationbal.searchWindowdelivery(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }


        private bool Validationdelivery()
        {
            bool status = true;
            string message = "";
            int i = 0;

            foreach (DataGridViewRow row in dgvDelivery.Rows)
            {
                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[4];
                if (Convert.ToBoolean(CbxCell.Value) == false)
                {
                    status = false;
                    break;
                }
            }
            return status;
        }

        public string getcheckedvalue()
        {
            string val = string.Empty;
            foreach (DataGridViewRow row in dgvDelivery.Rows)
            {
                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[4];
                if (Convert.ToBoolean(CbxCell.Value) == true)
                {
                    savedelivered(Convert.ToString(row.Cells[3].Value), 1);

                    //if(string.IsNullOrEmpty(val))
                    //{
                    //    val ="'"+Convert.ToString(row.Cells[3].Value)+"'";
                    //}
                    //else
                    //{
                    //    val =val+ ",'" + Convert.ToString(row.Cells[3].Value) + "'";
                    //}
                }
            }
            //string[] vasa = val.Split(',');
            //if (vasa.Length==1)
            //{
            //    val = val.Replace("'", "");
            //}

            return val;
        }

        public void savedelivered(string pid, int status)
        {
           // Pnloading6.Visible = true;
            string s = objQuotationbal.Windowsalesavedelivered(Txtdeliveryorderno.Text, pid, status);

            if (s == "1")
            {
                //if (status == 2)
                //{
                    //Pnloading6.Visible = false;
                    //clear();
                //}
            }

        }

        public void deletedelivered()
        {

            objQuotationbal.deletedelivered(Txtdeliveryorderno.Text);


        }

        private void dgvChecking_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvChecking.CurrentCell.RowIndex == dgvChecking.Rows.Count - 1)
            {
                if (e.KeyData == Keys.Enter)
                {
                    btnsave.Focus();
                }
            }
        }




        private void dgvDelivery_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvDelivery.CurrentCell.RowIndex == dgvDelivery.Rows.Count - 1)
            {
                if (e.KeyData == Keys.Enter)
                {
                    btnsave.Focus();
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


        public string getrack(string s, string s1)
        {
            string v = objQuotationbal.getrack(s, s1);
            return v;
        }

        private void dgvFloorCheckOut_DataError(object sender, DataGridViewDataErrorEventArgs e)
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

        public DataTable getpay(DataGridView dgv)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Quantity", typeof(string)),
                            new DataColumn("Rack", typeof(string)),
                            new DataColumn("Productid",typeof(string)) });

            for (int i = 0; i < dgv.Rows.Count; i++)
            {

                dt.Rows.Add(Convert.ToString(dgv.Rows[i].Cells["Quantity"].Value), " ", Convert.ToString(dgv.Rows[i].Cells["Productid"].Value));
            }

            return dt;
        }

        private void Txtcustomername_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Txtcustomername.SelectedIndex > 0)
            {
                cmdcity.Text = objQuotationbal.bindcity(Convert.ToString(Txtcustomername.SelectedValue));
            }

        }

        private void txtcardno_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void btntransactionpay_Click_1(object sender, EventArgs e)
        {

        }

      




    }
}

