using System;
using System.Collections.Generic;
using Inventory.Report_Transaction;
using Inventory.Sales;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

using InvBal;
using Inventory.Commission;




namespace Inventory.Sales
{
    public partial class CommissionPayment : Form
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
        public CommissionPayment()
        {
            InitializeComponent();

            srole = Program.userid;
            if (srole == "1")
            {
                role1 = "Admin";
            }
            else
            {
                role1 = "Emp";
            }

            this.WindowState = FormWindowState.Maximized;
            bindAccountno();

            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();

            ddlpaymode.SelectedIndex = 0;
            cmbbank.SelectedIndex = 0;


            paymentdenobind();
            paymentDenotoCustomerbind();

            ddlpaymode.SelectedIndex = 0;


            SearchPurchaseOrder();




            txtsearch1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtsearch1.AutoCompleteCustomSource = Autocusomer();
            txtsearch1.AutoCompleteSource = AutoCompleteSource.CustomSource;


            txtsearch2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtsearch2.AutoCompleteCustomSource = Autocusomer();
            txtsearch2.AutoCompleteSource = AutoCompleteSource.CustomSource;


            txtsearch3.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtsearch3.AutoCompleteCustomSource = Autocusomer();
            txtsearch3.AutoCompleteSource = AutoCompleteSource.CustomSource;




            selectedtab = MainTabSalesBill.SelectedTab.Name;


            searchpay("Commissionid", "", "Updatedon", "Today", "Referenceid", "", role1, Program.userid);

            DataTable dt = bindcheckout();
            cmbstatus1.DataSource = dt;
            cmbstatus1.DisplayMember = "Commissionid";
            cmbstatus1.ValueMember = "Commissionid";

            cmbstatus2.DataSource = dt;
            cmbstatus2.DisplayMember = "Commissionid";
            cmbstatus2.ValueMember = "Commissionid";


            cmbstatus3.DataSource = dt;
            cmbstatus3.DisplayMember = "Commissionid";
            cmbstatus3.ValueMember = "Commissionid";


            btnSavePending.Enabled = false;
            btnNew.Enabled = false;
            btnPrint.Enabled = false;

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
            search.Add("Reference");
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
            search.Add("Reference");
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
            search.Add("Reference");
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
           
        }

        private void ListSearchDate2_Click(object sender, EventArgs e)
        {
           
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
            paymentdo.Rows.Add("2000", 0, 0.00);
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
            this.dgvCustomerpaid.Columns[0].Width = 100;
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
            paymentdo.Rows.Add("2000", 0, 0.00);
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



        private void MainTabSalesBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedtab = MainTabSalesBill.SelectedTab.Name;

            if (selectedtab == "TabPayment")
            {
                searchpay("Estimationid", "", "Updatedon", "Today", "customername", "", role1, Program.userid);
            }

        }
        #endregion


        //public AutoCompleteStringCollection AutoCompleteLoad()
        //{
        //    AutoCompleteStringCollection str = new AutoCompleteStringCollection();
        //    DataTable st = objQuotationbal.itemauto();
        //    string[] arr = new string[st.Rows.Count];
        //    for (int i = 0; i < st.Rows.Count; i++)
        //    {
        //        arr[i] = st.Rows[i]["DisplayName"].ToString();
        //    }

        //    //for (int i = 0; i < arr.Length; i++)
        //    //{
        //    var combined = string.Join(", ", arr);
        //    //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
        //    str.Add(combined);
        //    //}

        //    //for (int i = 0; i < st.Rows.Count; i++)
        //    //{
        //    //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
        //    //    str.Add(combined);
        //    //}

        //    return str;
        //}

        public AutoCompleteStringCollection Autocusomer()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.Getreference();
            string[] arr = new string[st.Rows.Count];
            for (int i = 0; i < st.Rows.Count; i++)
            {
                arr[i] = st.Rows[i]["Name"].ToString();
            }

            //for (int i = 0; i < arr.Length; i++)
            //{
            var combined = string.Join(", ", arr);
            //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);

            str.Add(combined);
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
            if (ddlpaymode.SelectedIndex == 1)
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

                lblpaymentamount.Text = String.Format("{0:00.00}", Convert.ToDouble(Txtbalance.Text));
                cashpl.Visible = true;
                panelTransaction.Visible = false;
                this.ActiveControl = cashddl;
                Application.Idle += new EventHandler(Application_Idle);
            }
            else if (ddlpaymode.SelectedIndex == 2)
            {
                cashpl.Visible = false;
                panelTransaction.Visible = true;
                lblChequeNo.Visible = true;
                txttransactionid.Visible = true;
                lblcardammount.Text = String.Format("{0:00.00}", Convert.ToDouble(Txtbalance.Text));
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




        #region Validation


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

                string hdnCommission = lblhidden.Text;
                GetReport(hdnCommission);
                ddlpaymode.Enabled = false;
                searchpay("Commissionid", "", "Updatedon", "Today", "Referenceid", "", role1, Program.userid);
            }
        }



        private void SalesBillNew_Load(object sender, EventArgs e)
        {

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {




                if (btnreceiveBalance.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = btncashpay;
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
                if (keyData == Keys.Escape)
                {
                    if (cashdetailsclose.Focused)
                    {
                    cashdetailsclose.PerformClick();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(txtpauycustomername.Text))
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
                }


            }
            catch
            {

            }

            return base.ProcessCmdKey(ref msg, keyData);
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

        private void clear()
        {


            if (selectedtab == "TabPayment")
            {
                txtpauycustomername.Text = string.Empty;
                txtpayamount.Text = string.Empty;
                txtpaypercentage.Text = string.Empty;
                Txtpaid.Text = string.Empty;
                Txtbalance.Text = string.Empty;


                ddlpaymode.Enabled = false;
                ddlpaymode.SelectedIndex = 0;
                cmbbank.SelectedIndex = 0;
                txtcardno.Text = string.Empty;
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
                Txtmode.Text = string.Empty;


            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToDouble(lblpaymenttotal.Text) == 0.00)
            {
                MessageBox.Show("Please Enter Amount ");
            }
            else if (Convert.ToDouble(lblpaymentbalance.Text) > 0)
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

        }








        public DataTable bindcheckout()
        {
            DataTable dt = new DataTable();
            dt = objQuotationbal.itemcommisionpayment();
            DataRow dr = dt.NewRow();
            dr["Commissionid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);
            return dt;
        }




        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
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
                            string[] rr = txtsearch1.Text.Split('-');
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
                            string[] rr = txtsearch2.Text.Split('-');
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
                            string[] rr = txtsearch3.Text.Split('-');
                            thirdvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                        }
                        else
                        {
                            thirdvalue = txtsearch3.Text.Trim();
                        }
                    }
                }

                if (firstname == "Reference")
                {
                    firstname1 = "Referenceid";
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

                if (secondname == "Reference")
                {
                    firstname1 = "Referenceid";
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

                if (thirdname == "Reference")
                {
                    firstname1 = "Referenceid";
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



                searchpay("Referenceid", firstvalue1, "Updatedon", secondvalue1, "Commissionid", thirdvalue1, role1, Program.userid);

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



        public void getpay(string s)
        {
            DataTable ds = objQuotationbal.Getpaycommision(s);

            if (ds.Rows.Count > 0)
            {
                panelCustomerpaid.Visible = false;
                cashpl.Visible = false;
                panelTransaction.Visible = false;
                ddlpaymode.Enabled = true;
                lblhidden.Text = Convert.ToString(ds.Rows[0]["Commissionid"]);
                txtpauycustomername.Text = Convert.ToString(ds.Rows[0]["Referenceid"]);
                txtpayamount.Text = Convert.ToString(ds.Rows[0]["CommissionAmount"]);
                txtpaypercentage.Text = Convert.ToString(ds.Rows[0]["CommissionPercentage"]);
                Txtpaid.Text = Convert.ToString(ds.Rows[0]["CommissionPaid"]);
                Txtbalance.Text = Convert.ToString(ds.Rows[0]["CommissionBalance"]);
                Txtmode.Text = Convert.ToString(ds.Rows[0]["CommisionMode"]);
                if (Txtmode.Text == "Percentage")
                {
                    label61.Visible = true;
                    txtpaypercentage.Visible = true;
                    label131.Location = new Point(65, 55);
                    txtpauycustomername.Location = new Point(239, 52);
                    label62.Location = new Point(65, 87);
                    txtpayamount.Location = new Point(239, 87);
                    //label131.Location = new Point(65, 117);
                    //txtpauycustomername.Location = new Point(239, 116);


                    label131.Location = new Point(65, 55);
                    txtpauycustomername.Location = new Point(239, 52);

                }
                else
                {
                    label61.Visible = false;
                    txtpaypercentage.Visible = false;

                    label62.Location = new Point(65, 117);
                    txtpayamount.Location = new Point(239, 116);
                    label131.Location = new Point(65, 87);
                    txtpauycustomername.Location = new Point(239, 87);
                    //label131.Location = new Point(65, 117);
                    //txtpauycustomername.Location = new Point(239, 116);

                }


                ddlpaymode.Focus();

            }
            else
            {
                panel2.Enabled = true;
                clear();
            }


        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {


                string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                if (!string.IsNullOrEmpty(s))
                {
                    getpay(s);


                }
                else
                {
                    clear();
                }



            }
        }

        private void Btnsubmit_Click(object sender, EventArgs e)
        {


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
            DataTable dt = objQuotationbal.searchCommissionpatyment(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
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

            if (Math.Round(Convert.ToDouble(lblpaymentbalance.Text)) != Math.Round(Convert.ToDouble(lblpaidbalance.Text)))
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
                string hdnCommission = lblhidden.Text;
                GetReport(hdnCommission);
                ddlpaymode.Enabled = false;
                searchpay("Commissionid", "", "Updatedon", "Today", "Referenceid", "", role1, Program.userid);
            }
        }
        public void GetReport(string QuotationId)
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
                        SqlCommand cmd = new SqlCommand("GetComissionPaymentId", con);
                        cmd.Parameters.AddWithValue("@RequestId", QuotationId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);
                        VocharCommissionBillDal Obj1 = new VocharCommissionBillDal();
                        Obj1.dsMain = ds;
                        if (Obj1.GenerateQuoation())
                        {
                            frmPrintPreview objfrmpreview = new frmPrintPreview();
                            objfrmpreview.fileName = Obj1.fileName;
                            objfrmpreview.Show();

                        }



                    }
                }
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
                //if (Convert.ToDouble(lblpaymentamount.Text) > Convert.ToDouble(lblpaymenttotal.Text))
                //{
                //    MessageBox.Show("Please Pay Full Amount");
                //}
                //else
                //{
                savepayment();
                string hdnCommission = lblhidden.Text;
                GetReport(hdnCommission);
                clear();
                ddlpaymode.Enabled = false;
                searchpay("Commissionid", "", "Updatedon", "Today", "Referenceid", "", role1, Program.userid);

                //    }
            }
        }

        public void savepayment()
        {
            double d = 0.00, balance = 0.00;

            objQuotationbal.transid = lblhidden.Text;
            objQuotationbal.paidtwothousand = Convert.ToString(cashddl.Rows[0].Cells[1].Value);
            objQuotationbal.paidthousand = Convert.ToString(cashddl.Rows[1].Cells[1].Value);
            objQuotationbal.paidfivehundred = Convert.ToString(cashddl.Rows[2].Cells[1].Value);
            objQuotationbal.paidhundred = Convert.ToString(cashddl.Rows[3].Cells[1].Value);
            objQuotationbal.paidfifty = Convert.ToString(cashddl.Rows[4].Cells[1].Value);
            objQuotationbal.paidtwenty = Convert.ToString(cashddl.Rows[5].Cells[1].Value);
            objQuotationbal.paidten = Convert.ToString(cashddl.Rows[6].Cells[1].Value);
            objQuotationbal.paidfive = Convert.ToString(cashddl.Rows[7].Cells[1].Value);
            objQuotationbal.paidcoin = Convert.ToString(cashddl.Rows[8].Cells[1].Value);
            objQuotationbal.PaidOne = Convert.ToString(cashddl.Rows[9].Cells[1].Value);
            objQuotationbal.OAmount = lblpaymentamount.Text;




            if (Convert.ToDouble(lblpaymentbalance.Text) > 0.00)
            {
                d = Convert.ToDouble(Txtpaid.Text) + Convert.ToDouble(lblpaymentamount.Text);
                balance = Convert.ToDouble(Txtbalance.Text) - d;
                objQuotationbal.denam = 2;
                objQuotationbal.paid = Convert.ToString(d);
                objQuotationbal.balance = Convert.ToString(balance);
            }
            else
            {
                d = Convert.ToDouble(Txtpaid.Text) + Convert.ToDouble(lblpaymenttotal.Text);
                balance = Convert.ToDouble(txtpayamount.Text) - d;
                objQuotationbal.paid = Convert.ToString(d);
                objQuotationbal.balance = Convert.ToString(balance);
                objQuotationbal.denam = 1;
            }



            lblid.Text = objQuotationbal.SavecommisionPayment(objQuotationbal, lblpaymenttotal.Text);



        }

        public void SavepaymentDenomination()
        {


            objQuotationbal.recivetransid = lblid.Text;
            if (Convert.ToString(dgvCustomerpaid.Rows[0].Cells[1].Value) != "0")
            {
                objQuotationbal.recivetwothousand = "-" + Convert.ToString(dgvCustomerpaid.Rows[0].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivetwothousand = Convert.ToString(dgvCustomerpaid.Rows[0].Cells[1].Value);
            }
            if (Convert.ToString(dgvCustomerpaid.Rows[1].Cells[1].Value) != "0")
            {
                objQuotationbal.recivethousand = "-" + Convert.ToString(dgvCustomerpaid.Rows[1].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivethousand = Convert.ToString(dgvCustomerpaid.Rows[1].Cells[1].Value);
            }
            if (Convert.ToString(dgvCustomerpaid.Rows[2].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefivehundred = "-" + Convert.ToString(dgvCustomerpaid.Rows[2].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefivehundred = Convert.ToString(dgvCustomerpaid.Rows[2].Cells[1].Value);
            }



            if (Convert.ToString(dgvCustomerpaid.Rows[3].Cells[1].Value) != "0")
            {
                objQuotationbal.recivehundred = "-" + Convert.ToString(dgvCustomerpaid.Rows[3].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivehundred = Convert.ToString(dgvCustomerpaid.Rows[3].Cells[1].Value);
            }


            if (Convert.ToString(dgvCustomerpaid.Rows[4].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefifty = "-" + Convert.ToString(dgvCustomerpaid.Rows[4].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefifty = Convert.ToString(dgvCustomerpaid.Rows[4].Cells[1].Value);
            }


            if (Convert.ToString(dgvCustomerpaid.Rows[5].Cells[1].Value) != "0")
            {
                objQuotationbal.recivetwenty = "-" + Convert.ToString(dgvCustomerpaid.Rows[5].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivetwenty = Convert.ToString(dgvCustomerpaid.Rows[5].Cells[1].Value);
            }

            if (Convert.ToString(dgvCustomerpaid.Rows[6].Cells[1].Value) != "0")
            {
                objQuotationbal.reciveten = "-" + Convert.ToString(dgvCustomerpaid.Rows[6].Cells[1].Value);

            }
            else
            {
                objQuotationbal.reciveten = Convert.ToString(dgvCustomerpaid.Rows[6].Cells[1].Value);
            }


            if (Convert.ToString(dgvCustomerpaid.Rows[7].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefive = "-" + Convert.ToString(dgvCustomerpaid.Rows[7].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefive = Convert.ToString(dgvCustomerpaid.Rows[7].Cells[1].Value);
            }

            if (Convert.ToString(dgvCustomerpaid.Rows[8].Cells[1].Value) != "0")
            {
                objQuotationbal.recivecoin = "-" + Convert.ToString(dgvCustomerpaid.Rows[8].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivecoin = Convert.ToString(dgvCustomerpaid.Rows[8].Cells[1].Value);
            }

            if (Convert.ToString(dgvCustomerpaid.Rows[9].Cells[1].Value) != "0")
            {
                objQuotationbal.ReceiveOne = "-" + Convert.ToString(dgvCustomerpaid.Rows[9].Cells[1].Value);

            }
            else
            {
                objQuotationbal.ReceiveOne = Convert.ToString(dgvCustomerpaid.Rows[9].Cells[1].Value);
            }

            objQuotationbal.OAmount = lblpaidbalance.Text;
            string output = objQuotationbal.SavepaymentDenomination(objQuotationbal);

            if (output == "1")
            {

                clear();
            }

        }

        public void savecard()
        {

            //Pnloading4.Visible = true;
            objQuotationbal.Quotationid = lblhidden.Text;
            objQuotationbal.Bank = cmbbank.Text;
            objQuotationbal.Cardnumber = txtcardno.Text;
            objQuotationbal.transid = txttransactionid.Text;

            objQuotationbal.OAmount = Convert.ToString(lblcardammount.Text);
            string s = objQuotationbal.savecardCommission(objQuotationbal);

            if (s == "1")
            {
                //Pnloading4.Visible = false;
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



        private void TabPayment_Click(object sender, EventArgs e)
        {

        }

        private void txtcardno_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void btnsave_Click(object sender, EventArgs e)
        {

        }







    }
}

