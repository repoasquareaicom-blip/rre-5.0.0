using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Adjustment
{
    public partial class AdjustmentforPurchaseBill : Form
    {
        int userid = 0;
        string role = "";
        string UserId = "";
        string clickstatus = string.Empty;
        string OrderNumber, VendorId;
        PurchaseReceiptBAL ObjPurchaseReceiptBAL = new PurchaseReceiptBAL();
       // int userid = 0;
       // string firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1;
        public AdjustmentforPurchaseBill()
        {
            InitializeComponent();
            LoadInvoice();
            LoadBill();
            this.WindowState = FormWindowState.Maximized;
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            role="Admin";
            UserId=Convert.ToString(Program.userid);
        }
        private void LoadInvoice()
        {

            dgvinvoice.Rows.Clear();
            dgvinvoice.Columns.Clear();
            dgvinvoice.RowCount = 1;
            dgvinvoice.ColumnCount = 9;

            dgvinvoice.Columns[0].Name = "Sno";
            dgvinvoice.Columns[1].Name = "Items";
            dgvinvoice.Columns[2].Name = "UOM";
            dgvinvoice.Columns[3].Name = "Quantity";
            dgvinvoice.Columns[4].Name = "Price";
            dgvinvoice.Columns[5].Name = "Tax";
            dgvinvoice.Columns[6].Name = "Amount";
            dgvinvoice.Columns[7].Name = "ProductId";
            dgvinvoice.Columns[8].Name = "Sum";
            //dgvinvoice.Columns[4].Name = "Discount";
            //dgvinvoice.Columns[5].Name = "Tax";
            //dgvinvoice.Columns[6].Name = "Sub Total";

            this.dgvinvoice.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
           
            // this.dgvOrder.Columns[0].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[0].ReadOnly = true;

            this.dgvinvoice.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
           
            // this.dgvOrder.Columns[0].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[1].ReadOnly = true;


            this.dgvinvoice.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
          
            //this.dgvOrder.Columns[1].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            this.dgvinvoice.Columns[2].ReadOnly = true;


            this.dgvinvoice.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
          
            //this.dgvOrder.Columns[2].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvinvoice.Columns[3].ReadOnly = true;
            dgvinvoice.Columns[3].HeaderText = "Qty";

            this.dgvinvoice.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
     
            //this.dgvOrder.Columns[3].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            this.dgvinvoice.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
       

            this.dgvinvoice.Columns[5].ReadOnly = true;
            //this.dgvOrder.Columns[4].DefaultCellStyle.BackColor = Color.Beige;
            dgvinvoice.Columns[6].Visible = true;
            dgvinvoice.Columns[7].Visible = false;
            dgvinvoice.Columns[8].Visible = false;

            //DataTable dt = new DataTable();
            //dt.Columns.Add("ID",typeof(string));
            //dt.Columns.Add("Name", typeof(string));
            //dt.Rows.Add("0","Select");
            //dt.Rows.Add("1", "14.5");
            //dt.Rows.Add("2", "5.0");
            //dt.Rows.Add("3", "No Tax");

            //DataGridViewComboBoxCell cmb = new DataGridViewComboBoxCell();
            //cmb.HeaderText = "Tax";
            //cmb.Name = "cmbtax";
            //cmb.DataSource = dt;
            //cmb.DisplayMember = "Name";
            //cmb.ValueMember = "ID";
            ////cmb.MaxDropDownItems = 4;
            ////cmb.Items.Add("14.5");
            ////cmb.Items.Add("5.0");
            ////cmb.Items.Add("No Tax");
            ////cmb.DefaultCellStyle.NullValue = "Select";
            //dgvinvoice.Columns.Insert(5, cmb);

         
            
            this.dgvinvoice.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            //this.dgvinvoice.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvinvoice.Columns[5].Width = 50;
            ////this.dgvOrder.Columns[5].DefaultCellStyle.BackColor = Color.Beige;


            //this.dgvinvoice.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvinvoice.Columns[6].Width = 100;
            // this.dgvOrder.Columns[6].DefaultCellStyle.BackColor = Color.Beige;

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {

                this.dgvinvoice.Columns[0].Width = 60;
                this.dgvinvoice.Columns[1].Width = 290;
                this.dgvinvoice.Columns[2].Width = 80;
                this.dgvinvoice.Columns[3].Width = 50;
                this.dgvinvoice.Columns[4].Width = 90;
                this.dgvinvoice.Columns[5].Width = 70;
                this.dgvinvoice.Columns[6].Width = 100;
            }
            else
            {              
                this.dgvinvoice.Columns[0].Width = 60;
                this.dgvinvoice.Columns[1].Width = 300;
                this.dgvinvoice.Columns[2].Width = 70;
                this.dgvinvoice.Columns[3].Width = 90;
                this.dgvinvoice.Columns[4].Width = 60;
                this.dgvinvoice.Columns[5].Width = 60;
                this.dgvinvoice.Columns[6].Width = 100;
            }
            dgvinvoice.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvinvoice.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }
        }



        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            if (keyData == Keys.Escape)
            {

                if (dgvinvoice.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dgvinvoice.Rows[0].Cells[1].Value)))
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
                




                return true;

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void LoadBill()
        {

           // dgvEstimationBill.Rows.Clear();
           // dgvEstimationBill.Columns.Clear();
           // dgvEstimationBill.RowCount = 1;
            if (dgvEstimationBill.Rows.Count>0)
            {
                dgvEstimationBill.Columns.Clear();
                dgvEstimationBill.DataSource = null;
                
            }
          
            dgvEstimationBill.ColumnCount =5;

            dgvEstimationBill.Columns[0].Name = "Bill Number";
            dgvEstimationBill.Columns[1].Name = "Bill Amount";
            dgvEstimationBill.Columns[2].Name = "Paid";
            dgvEstimationBill.Columns[3].Name = "Balance";
            dgvEstimationBill.Columns[4].Name = "Action";

            this.dgvEstimationBill.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvEstimationBill.Columns[0].Width = 60;           
            this.dgvEstimationBill.Columns[0].ReadOnly = true;

            this.dgvEstimationBill.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvEstimationBill.Columns[1].Width = 370;            
            this.dgvEstimationBill.Columns[1].ReadOnly = true;
            this.dgvEstimationBill.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvEstimationBill.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvEstimationBill.Columns[2].Width = 120;          
            this.dgvEstimationBill.Columns[2].ReadOnly = true;
            this.dgvEstimationBill.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvEstimationBill.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvEstimationBill.Columns[3].Width = 90;          
            this.dgvEstimationBill.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvEstimationBill.Columns[3].ReadOnly = true;


            this.dgvEstimationBill.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
          //  this.dgvEstimationBill.Columns[4].Width = 80;
            this.dgvEstimationBill.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //DataGridViewImageColumn img1 = new DataGridViewImageColumn();
            //img1.Image = Inventory.Properties.Resources.user_edit;
            //img1.HeaderText = "Action";
            //img1.Name = "Action";
            //dgvEstimationBill.Columns.Add(img1);


           


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                dgvEstimationBill.Columns[0].Width = 150;
                dgvEstimationBill.Columns[1].Width = 150;
                dgvEstimationBill.Columns[2].Width = 200;
                dgvEstimationBill.Columns[3].Width = 200;
                dgvEstimationBill.Columns[4].Width = 300;

            }
            else
            {
                dgvEstimationBill.Columns[0].Width = 100;
                dgvEstimationBill.Columns[1].Width = 100;
                dgvEstimationBill.Columns[2].Width = 150;
                dgvEstimationBill.Columns[3].Width = 150;
                dgvEstimationBill.Columns[4].Width = 250;
            }
            dgvEstimationBill.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvEstimationBill.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }
        }
        private void AdjustmentforPurchaseBill_Load(object sender, EventArgs e)
        {

        }

        //public DataTable bindEstimation()
        //{
        //    DataTable dt = new DataTable();
        //    dt = BillAdjustmentsBAL.GetOrderNumberForPurchase();
        //    DataRow dr = dt.NewRow();
        //    dr["OrderNumber"] = "-Select-";
        //    dt.Rows.InsertAt(dr, 0);

        //    return dt;
        //}

        public DataTable bindVendorname()
        {
            DataTable dt = new DataTable();
            dt = BillAdjustmentsBAL.GetVendorNameForPurchase();
            DataRow dr = dt.NewRow();
            dr["Name"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);

            return dt;
        }

        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("Order Number");
            search.Add("Order Date");
            search.Add("Vendor");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderNo.DataSource = bs.DataSource;
            cbxSearchOrderNo.SelectedIndex = 0;

        }

        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("Order Number");
            search.Add("Order Date");
            search.Add("Vendor");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderDate.DataSource = bs.DataSource;
            cbxSearchOrderDate.SelectedIndex = 1;
        }

        private void SearchCreteria3()
        {
            List<string> search = new List<string>();
            search.Add("Order Number");
            search.Add("Order Date");
            search.Add("Vendor");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxVendor.DataSource = bs.DataSource;
            cbxVendor.SelectedIndex = 2;
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
            //if (pnlOrder.Visible == true)
            //{
            //    pnlOrder.Visible = false;
            //    vLabel2.Visible = false;
            //    pnlCollapse2.Visible = true;
            //    splitContainer1.Panel2Collapsed = false;
            //    pbxCollapse.Visible = true;
            //    pbxRightCollapse.Visible = true;
            //    this.dgvSearch.Columns[1].Visible = false;
            //    this.dgvSearch.Columns[2].Visible = false;

            //}

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
                if (dgvSearch.Rows.Count>0)
                {
                    this.dgvSearch.Columns[1].Visible = false;
                    this.dgvSearch.Columns[2].Visible = false;
                }
               

            }
        }



        private void cbxSearchOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string ordno = Convert.ToString(cbxSearchOrderNo.SelectedValue);

            //if (ordno == "RequestedBy")
            DataTable dtvendor = bindVendorname();
            
           
            if (cbxSearchOrderNo.SelectedIndex == 2)
            {
                cmbstatus1.Visible = true;
                txtsearch1.Visible = false;
                ListSearchDate1.Visible = false;
                cmbstatus1.DataSource = dtvendor;
                cmbstatus1.DisplayMember = "Name";
                cmbstatus1.ValueMember = "Name";
            }
            else if (cbxSearchOrderNo.SelectedIndex == 1)
            {
                cmbstatus1.Visible = false;
                txtsearch1.Visible = true;
                txtsearch1.Text = SearchFrmDate.Value.ToString("dd/MM/yyyy");
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
            //string orddate = Convert.ToString(cbxSearchOrderDate.SelectedValue);
            //if (orddate == "RequestedBy")
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
                //txtsearch2.Text = SearchFrmDate.Value.ToString("dd/MM/yyyy");
                txtsearch2.Text = string.Empty;
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
            DataTable dtvendor = bindVendorname();
            //string ordvender = Convert.ToString(cbxVendor.SelectedValue);
            //if (ordvender == "RequestedBy")
            if (cbxVendor.SelectedIndex == 2)
            {
                cmbstatus3.DataSource = dtvendor;
                cmbstatus3.DisplayMember = "Name";
                cmbstatus3.ValueMember = "Name";

                cmbstatus3.Visible = true;
                txtsearch3.Visible = false;
                ListSearchDate3.Visible = false;
            }
            else if (cbxVendor.SelectedIndex == 1)
            {
                cmbstatus3.Visible = false;
                txtsearch3.Visible = true;
                txtsearch3.Text = SearchFrmDate.Value.ToString("dd/MM/yyyy");
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if ((cbxSearchOrderNo.SelectedIndex == cbxSearchOrderDate.SelectedIndex) || cbxSearchOrderNo.SelectedIndex == cbxVendor.SelectedIndex || cbxSearchOrderDate.SelectedIndex == cbxVendor.SelectedIndex)
            {
                MessageBox.Show("* Search a item Should Not Be Same");
            }
            else
            {
                string firstname = string.Empty, firstvalue = string.Empty, secondname = string.Empty, secondvalue = string.Empty, thirdname = string.Empty, thirdvalue = string.Empty;
                string firstname1 = string.Empty, firstvalue1 = string.Empty, secondname1 = string.Empty, secondvalue1 = string.Empty, thirdname1 = string.Empty, thirdvalue1 = string.Empty;
                string UserBy = string.Empty, IsCompleted = string.Empty, ChkComplete = string.Empty;


                firstname = cbxSearchOrderNo.Text.Trim();
                if (firstname == "Vendor")
                {
                    firstname = "Name";
                    //firstvalue = Convert.ToString(cmbstatus1.SelectedValue);
                    if (!string.IsNullOrEmpty(cmbstatus1.Text) && cmbstatus1.Text != "--Select--")
                    {
                        firstvalue = cmbstatus1.Text;
                    }
                }
                else
                {
                    //firstvalue = txtsearch1.Text.Trim();
                    string part1 = txtsearch1.Text.Trim();
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
                if (secondname == "Vendor")
                {
                    secondname = "Vendor";
                    //secondvalue = Convert.ToString(cmbstatus2.SelectedValue);
                    if (!string.IsNullOrEmpty(cmbstatus2.Text) && cmbstatus2.Text != "--Select--")
                    {
                        secondvalue = cmbstatus2.Text;
                    }
                }
                else
                {
                    //secondvalue = txtsearch2.Text.Trim();
                    string part1 = txtsearch2.Text.Trim();
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
                if (thirdname == "Vendor")
                {
                    thirdname = "Vendor";
                    if (!string.IsNullOrEmpty(cmbstatus3.Text) && cmbstatus3.Text != "-Select-")
                    {
                        thirdvalue = cmbstatus3.Text;
                    }
                }
                else
                {
                    //thirdvalue = txtsearch3.Text.Trim();
                    string part1 = txtsearch3.Text.Trim();
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

                if (firstname == "Order Number")
                {
                    firstname1 = firstname;
                    firstvalue1 = firstvalue;
                }
                else if (firstname == "Order Date")
                {
                    secondname1 = firstname;
                    secondvalue1 = firstvalue;
                }
                else if (firstname == "Vendor")
                {
                    thirdname1 = firstname;
                    thirdvalue1 = firstvalue;
                }


                if (secondname == "Order Number")
                {
                    firstname1 = secondname;
                    firstvalue1 = secondvalue;
                }
                else if (secondname == "Order Date")
                {
                    secondname1 = secondname;
                    secondvalue1 = secondvalue;
                }
                else if (secondname == "Vendor")
                {
                    thirdname1 = secondname;
                    thirdvalue1 = secondvalue;
                }


                if (thirdname == "Order Number")
                {
                    firstname1 = thirdname;
                    firstvalue1 = thirdvalue;
                }
                else if (thirdname == "Order Date")
                {
                    secondname1 = thirdname;
                    secondvalue1 = thirdvalue;
                }
                else if (thirdname == "Vendor")
                {
                    thirdname1 = thirdname;
                    thirdvalue1 = thirdvalue;
                }
                firstname1 = "OrderNumber";
                secondname1 = "InvoiceDate";
                thirdname1 = "Name";
                search(firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1, role, UserId);
                
            }
            
        }

        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
                    DataTable dt;
                    dt = BillAdjustmentsBAL.SearchAdjustmentforPurchase(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role, UserId);
           
                    dgvSearch.Columns.Clear();
                    dgvSearch.DataSource = dt;
                    lblItemCount.Text = Convert.ToString(dt.Rows.Count);

                    dgvSearch.Columns["VendorId"].Visible = false;
                    dgvSearch.Columns["Name"].Visible = true;
                    dgvSearch.Columns["NetAmount"].Visible = false;
                    dgvSearch.Columns["Name"].HeaderText = "Vendor Name";

                    dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10.1F, FontStyle.Bold);
                    dgvSearch.DefaultCellStyle.Font = new Font("Tahoma", 12.1F, FontStyle.Bold);
                    dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
                    dgvSearch.DefaultCellStyle.ForeColor = Color.Tomato;
                    dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                    dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
         }

        private void CalculateInvoiceNetAmount_Change(object sender, EventArgs e)
        {

        }

        private void IntOnly_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void CmbInvOCS_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CmbInvROS_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dgvinvoice_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void dgvinvoice_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

        }

        private void dgvinvoice_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                for (int i = 0; i < dgvSearch.Rows.Count; i++)
                {
                    string Id = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    GetAccountPayableBills(Id);

                    



                }
            }
        }
        public void GetAccountPayableBills(string VendorId)
        {
            DataTable dt = AccountPayableBAL.GetAccountPayableBills(VendorId);

            dgvEstimationBill.Columns.Clear();
            dgvEstimationBill.DataSource = dt;

            dgvEstimationBill.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvEstimationBill.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvEstimationBill.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            DataGridViewImageColumn img1 = new DataGridViewImageColumn();
            img1.Image = Inventory.Properties.Resources.user_edit;
            img1.HeaderText = "Action";
            img1.Name = "Action";
            dgvEstimationBill.Columns.Add(img1);
            this.dgvEstimationBill.Columns["Action"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
           
            dgvEstimationBill.Columns["VendorId"].Visible = false;
            dgvEstimationBill.Columns["InvoiceNo"].Visible = false;
            dgvEstimationBill.Columns["InvoiceDate"].Visible = false;

          

            dgvEstimationBill.Columns["Action"].DisplayIndex = 7;

            //dgvEstimationBill.Columns["OrderNumber"].Width = 150;
            //dgvEstimationBill.Columns["TotalAmount"].Width = 150;
            //dgvEstimationBill.Columns["Paid"].Width = 200;
            //dgvEstimationBill.Columns["Balance"].Width = 200;
            //dgvEstimationBill.Columns["Action"].Width = 350;


            

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                dgvEstimationBill.Columns["OrderNumber"].Width = 170;
                //dgvEstimationBill.Columns["Date"].Width = 170;
                dgvEstimationBill.Columns["TotalAmount"].Width = 150;
                dgvEstimationBill.Columns["Paid"].Width = 150;
                dgvEstimationBill.Columns["Balance"].Width = 150;
                dgvEstimationBill.Columns["Action"].Width = 500;
               // dgvEstimationBill.Columns["Action"].Width = 250;
              
            }
            else
            {
                dgvEstimationBill.Columns[0].Width = 150;
                dgvEstimationBill.Columns[1].Width = 100;
                dgvEstimationBill.Columns[2].Width = 150;
                dgvEstimationBill.Columns[3].Width = 150;
                dgvEstimationBill.Columns[4].Width = 180;
                this.dgvEstimationBill.Columns["Action"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }


            dgvEstimationBill.Columns["TotalAmount"].HeaderText = "Bill Amount";
            dgvEstimationBill.Columns["OrderNumber"].HeaderText = "Bill Number";

            this.dgvEstimationBill.Columns["Paid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvEstimationBill.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvEstimationBill.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void dgvEstimationBill_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvEstimationBill.Columns[e.ColumnIndex].HeaderText == "Action")
                {

                    for (int i = 0; i < dgvEstimationBill.Rows.Count; i++)
                    {
                        OrderNumber = Convert.ToString(dgvEstimationBill.Rows[e.RowIndex].Cells["OrderNumber"].Value);
                        VendorId = Convert.ToString(dgvEstimationBill.Rows[e.RowIndex].Cells["VendorId"].Value);

                        GetPurchaseReceiptInvoiceByOrderNo(OrderNumber);
                        //lblinvoiceTotals
                        //-
                        //txtPreviouseLess

                        lblCurrentBal.Text = Convert.ToString((Convert.ToDecimal(lblinvoiceTotals.Text)+Convert.ToDecimal(txttax1.Text)+Convert.ToDecimal(txttax2.Text))- Convert.ToDecimal(txtPreviouseLess.Text));
                        GetNetAmount();
                    }
                }
            }
                
        }

        public void GetPurchaseReceiptInvoiceByOrderNo(string s)
        {
            DataSet ds = BillAdjustmentsBAL.GetVendorOrderDetails(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                dateTimePicker6.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderDate"]);
                //dateTimePicker1.Text = Convert.ToString(ds.Tables[0].Rows[0]["ExpectedDeliveryDate"]);
                //cbVendor.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"]);
                TxtInvoiceOrdNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["POOrdno"]);
                txtInvoiceReceiptNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["PROrdno"]);
                cmbInvoiceStatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                txtInvoiceRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                
                TxtInInvoiceNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["InvoiceNo"]);
               // dtpInInvoicedate.Text = Convert.ToString(ds.Tables[0].Rows[0]["InvoiceDate"]);

                lblinvoiceTotals.Text = Convert.ToString(ds.Tables[0].Rows[0]["Total"]);
                txttax1.Text = Convert.ToString(ds.Tables[0].Rows[0]["Tax1"]);
                txttax2.Text = Convert.ToString(ds.Tables[0].Rows[0]["Tax2"]);
                txtInvoiceDis.Text = Convert.ToString(ds.Tables[0].Rows[0]["Discount"]);
                CmbInvROS.Text = Convert.ToString(ds.Tables[0].Rows[0]["RoundOffSimbol"]);
                txtinvoiceRoundOff.Text = Convert.ToString(ds.Tables[0].Rows[0]["RoundOff"]);
                CmbInvOCS.Text = Convert.ToString(ds.Tables[0].Rows[0]["OtherChargesSimbol"]);
                txtinvioceOtherCharges.Text = Convert.ToString(ds.Tables[0].Rows[0]["OtherCharges"]);
                lblinvoiceNet.Text = Convert.ToString(ds.Tables[0].Rows[0]["NetAmount"]);
                txtPreviouseLess.Text = Convert.ToString(ds.Tables[0].Rows[0]["LessAmount"]);
                //panel4.Enabled = false;

                lblCurrentBal.Text = Convert.ToString(Convert.ToDecimal(ds.Tables[0].Rows[0]["Total"]) + Convert.ToDecimal(ds.Tables[0].Rows[0]["Tax1"]) + Convert.ToDecimal(ds.Tables[0].Rows[0]["Tax2"]));
            }
            else
            {
               // panel4.Enabled = false;
                //clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
               // LoadInvoice();
                dgvinvoice.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvinvoice.Rows.Add();
                    dgvinvoice.Rows[i].Cells[0].Value = i + 1;
                    dgvinvoice.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);

                    dgvinvoice.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[i]["Price"]);
                    dgvinvoice.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Tax"]);
                    dgvinvoice.Rows[i].Cells[6].Value = Convert.ToString(ds.Tables[1].Rows[i]["Amount"]);

                    dgvinvoice.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["ReceivedQuantity"]);
                    dgvinvoice.Rows[i].Cells[7].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    string uom = ObjPurchaseReceiptBAL.GetUOMByProductId(Convert.ToInt32(ds.Tables[1].Rows[i]["Productid"]));
                    dgvinvoice.Rows[i].Cells[2].Value = uom;
                }
               // panel2.Enabled = false;
                dgvinvoice.CurrentCell = dgvinvoice[4, 0];
                dgvinvoice.BeginEdit(true);
            }
            else
            {
                dgvinvoice.Rows.Clear();
                //panel2.Enabled = true;
            }
        }

        private void txtless_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
            if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar)))
                e.Handled = true;


            //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            //{
            //    e.Handled = true;
            //}

            //// only allow one decimal point
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = true;
            //}
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validation())
            {
                if(string.IsNullOrEmpty(txtless.Text))
                {
                    txtless.Text = "0";
                }

                string AdjustmentAmount = txtless.Text;
                int result = BillAdjustmentsBAL.UpdateLessAmountforPurchase(OrderNumber, VendorId, AdjustmentAmount);
                if (result > 0)
                {
                    MessageBox.Show("Saved Successfully.");
                    LoadInvoice();
                    clear1();
                    LoadBill();
                }
            }
        }

        private void clear1()
        {
            TxtInvoiceOrdNo.Clear();
            txtInvoiceReceiptNo.Clear();
            cmbInvoiceStatus.SelectedIndex = 0;
            var today = DateTime.Today;
            dateTimePicker6.Value = today;

            TxtInInvoiceNo.Clear();
            dtpInInvoicedate.Value = today;
            txtInvoiceRemarks.Clear();

            lblinvoiceTotals.Text = "0.00";
            txttax1.Text = "0.00";
            txttax2.Text = "0.00";
            txtInvoiceDis.Text = "0.00";
            CmbInvROS.SelectedIndex = 0;
            txtinvoiceRoundOff.Text = "0.00";
            CmbInvOCS.SelectedIndex = 0;
            txtinvioceOtherCharges.Text = "0.00";
            lblinvoiceNet.Text = "0.00";

            dgvinvoice.Rows.Clear();
            //dgvinvoice.Rows.Add(1);
            txtPreviouseLess.Text = "0.00";
            txtless.Text = "0.00";
            lblCurrentBal.Text = "0.00";
            lblinvoiceNet.Text = "0.00";

        }
        public bool Validation()
        {
            string msg = "";
            bool status = true;
            if (string.IsNullOrEmpty(txtless.Text))
            {
                txtless.Text = "0.00";
            }
            if (!string.IsNullOrEmpty(txtless.Text) && Convert.ToDouble(txtless.Text) == 0.00)
            {
                msg += "Enter Less amount." + "\n";
            }
            if (Convert.ToString(lblCurrentBal.Text) != "0.00")
            {
               
                if (Convert.ToDecimal(lblCurrentBal.Text) <= Convert.ToDecimal(txtless.Text))
                {
                    msg += "Enter less amount should be less than to total amount." + "\n";
                }
            }
            else
            {
                status = false;
            }
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg);
                status = false;
            }

            return status;
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear1();
            LoadInvoice();
        }

        private void txtless_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtless.Text))
                {

                    double total = Convert.ToDouble(lblCurrentBal.Text);
                    double less = Convert.ToDouble(txtless.Text);
                    double grandtotal = total - less;
                    lblinvoiceNet.Text = String.Format("{0:00.00}", grandtotal);



                }
                else
                {
                    lblinvoiceNet.Text = String.Format("{0:00.00}", lblCurrentBal.Text);
                }
            }
            catch
            {

            }
            
           
        }

        public void GetNetAmount()
        {
            if (Convert.ToString(lblCurrentBal.Text) != "0.00" && !string.IsNullOrEmpty(Convert.ToString(lblCurrentBal.Text)))
            {
                lblinvoiceNet.Text = Convert.ToString(Convert.ToDecimal(lblCurrentBal.Text) - Convert.ToDecimal(txtless.Text));
            }
        }

        private void txtless_Leave(object sender, EventArgs e)
        {
            GetNetAmount();
        }

        private void txtless_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter || e.KeyCode==Keys.Tab)
            {
                GetNetAmount();
                btnSave.Focus();
            }
        }

        private void dgvinvoice_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = false;
        }

    }
}
