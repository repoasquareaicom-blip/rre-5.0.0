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
    public partial class AdjustmentforSalesBill : Form
    {
        string role1 = string.Empty;
        string srole = string.Empty;
        string Estimationid, Customerid, Quotationid;
        QuotationBal objQuotationbal = new QuotationBal();
        int userid = 0;
        string clickstatus = string.Empty;
        string firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue;

        public AdjustmentforSalesBill()
        {
            InitializeComponent();
            LoadPortsNew();
            LoadBill();
            userid = Convert.ToInt32(Program.userid);
            this.WindowState = FormWindowState.Maximized;
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            srole = Program.Userrole;
            if (srole != "Admin")
            {
                role1 = "Emp";
            }
            else
            {
                // role1 = "Admin";
                role1 = "Emp";
            }
        }

        public DataTable bindEstimation()
        {
            DataTable dt = new DataTable();
            dt = BillAdjustmentsBAL.GetOrderNumberForSales();
            DataRow dr = dt.NewRow();
            dr["Quotationid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);

            return dt;
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

            dgvNew.Columns[5].HeaderText = "Qty";

            //dgvNew.Columns[4].DefaultCellStyle.Format = "N2";
            //dgvNew.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvNew.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvNew.Columns[6].ReadOnly = true;


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvNew.Columns[0].Width = 10;
                this.dgvNew.Columns[1].Width = 100;
                this.dgvNew.Columns[2].Width = 15;
                this.dgvNew.Columns[4].Width = 15;
                this.dgvNew.Columns[5].Width = 12;
                this.dgvNew.Columns[6].Width = 100;

            }
            else
            {
                this.dgvNew.Columns[0].Width = 10;
                this.dgvNew.Columns[1].Width = 70;
                this.dgvNew.Columns[2].Width = 30;
                this.dgvNew.Columns[4].Width = 30;
                this.dgvNew.Columns[5].Width = 20;
                this.dgvNew.Columns[6].Width = 70;
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

        private void LoadBill()
        {

            // dgvEstimationBill.Rows.Clear();
            // dgvEstimationBill.Columns.Clear();
            // dgvEstimationBill.RowCount = 1;
            if (dgvEstimationBill.Rows.Count > 0)
            {
                dgvEstimationBill.Columns.Clear();
                dgvEstimationBill.DataSource = null;

            }

            dgvEstimationBill.ColumnCount = 6;

            dgvEstimationBill.Columns[0].Name = "Bill Number";
            dgvEstimationBill.Columns[1].Name = "Bill Date";
            dgvEstimationBill.Columns[2].Name = "Bill Amount";
            dgvEstimationBill.Columns[3].Name = "Paid";
            dgvEstimationBill.Columns[4].Name = "Balance";
            dgvEstimationBill.Columns[5].Name = "Action";

            this.dgvEstimationBill.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvEstimationBill.Columns[0].Width = 60;
            this.dgvEstimationBill.Columns[0].ReadOnly = true;

            this.dgvEstimationBill.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            // this.dgvEstimationBill.Columns[1].Width = 60;
            this.dgvEstimationBill.Columns[1].ReadOnly = true;

            this.dgvEstimationBill.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            // this.dgvEstimationBill.Columns[2].Width = 370;
            this.dgvEstimationBill.Columns[2].ReadOnly = true;
            this.dgvEstimationBill.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvEstimationBill.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            // this.dgvEstimationBill.Columns[3].Width = 120;
            this.dgvEstimationBill.Columns[3].ReadOnly = true;
            this.dgvEstimationBill.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvEstimationBill.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            //  this.dgvEstimationBill.Columns[4].Width = 90;
            this.dgvEstimationBill.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvEstimationBill.Columns[4].ReadOnly = true;


            this.dgvEstimationBill.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            //  this.dgvEstimationBill.Columns[4].Width = 80;
            this.dgvEstimationBill.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

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
                dgvEstimationBill.Columns[4].Width = 200;
                dgvEstimationBill.Columns[5].Width = 300;

            }
            else
            {
                dgvEstimationBill.Columns[0].Width = 100;
                dgvEstimationBill.Columns[1].Width = 100;
                dgvEstimationBill.Columns[2].Width = 150;
                dgvEstimationBill.Columns[3].Width = 150;
                dgvEstimationBill.Columns[4].Width = 150;
                dgvEstimationBill.Columns[5].Width = 250;
            }

            dgvEstimationBill.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvEstimationBill.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            if (keyData == Keys.Escape)
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





                return true;

            }

            return base.ProcessCmdKey(ref msg, keyData);
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

                if (dgvSearch.Rows.Count > 0)
                {
                    this.dgvSearch.Columns[1].Visible = false;
                    this.dgvSearch.Columns[2].Visible = false;
                }

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

        private void btnSearch_Click(object sender, EventArgs e)
        {


            if ((cbxSearchOrderNo.SelectedIndex == cbxSearchOrderDate.SelectedIndex) || cbxSearchOrderNo.SelectedIndex == cbxVendor.SelectedIndex || cbxSearchOrderDate.SelectedIndex == cbxVendor.SelectedIndex)
            {
                MessageBox.Show("Search a item Should Not Be Same");
            }
            else
            {


                firstname = cbxSearchOrderNo.Text.Trim();
                if (firstname == "Order Number")
                {
                    firstname = "Quotationid";
                    if (cmbstatus1.SelectedIndex != 0)
                    {
                        firstvalue = cmbstatus1.Text.Trim();
                    }
                    else
                    {
                        firstvalue = "";
                    }
                }
                else if (firstname == "Customer")
                {
                    firstname = "customername";
                    if (cmbstatus1.SelectedIndex != 0)
                    {
                        firstvalue = cmbstatus1.Text.Trim();
                    }
                    else
                    {
                        firstvalue = "";
                    }
                }
                else if (firstname == "Order Date")
                {
                    firstname = "Updatedon";
                    if (cmbstatus1.SelectedIndex != 0)
                    {
                        firstvalue = cmbstatus1.Text.Trim();
                    }
                    else
                    {
                        firstvalue = "";
                    }
                }


                secondname = cbxSearchOrderDate.Text.Trim();
                if (secondname == "Order Number")
                {
                    secondname = "Quotationid";
                    if (cmbstatus2.SelectedIndex != 0)
                    {
                        secondvalue = cmbstatus2.Text.Trim();
                    }
                    else
                    {
                        secondvalue = "";
                    }

                }
                else if (secondname == "Customer")
                {
                    secondname = "customername";
                    if (cmbstatus2.SelectedIndex != 0)
                    {
                        secondvalue = cmbstatus2.Text.Trim();
                    }
                    else
                    {
                        secondvalue = "";
                    }
                }
                else if (secondname == "Order Date")
                {
                    secondname = "Updatedon";
                    if (cmbstatus2.SelectedIndex != 0)
                    {
                        secondvalue = cmbstatus2.Text.Trim();
                    }
                    else
                    {
                        secondvalue = "";
                    }
                }


                thirdname = cbxVendor.Text.Trim();
                if (thirdname == "Order Number")
                {
                    thirdname = "Quotationid";
                    if (cmbstatus3.SelectedIndex != 0)
                    {
                        thirdvalue = cmbstatus3.Text.Trim();
                    }
                    else
                    {
                        thirdvalue = "";
                    }


                }
                else if (thirdname == "Customer")
                {
                    thirdname = "customername";
                    if (cmbstatus3.SelectedIndex != 0)
                    {
                        thirdvalue = cmbstatus3.Text.Trim();
                    }
                    else
                    {
                        thirdvalue = "";
                    }
                }
                else if (thirdname == "Order Date")
                {
                    thirdname = "Updatedon";
                    if (cmbstatus3.SelectedIndex != 0)
                    {
                        thirdvalue = cmbstatus3.Text.Trim();
                    }
                    else
                    {
                        thirdvalue = "";
                    }
                }

                search(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Convert.ToString(userid));
            }
        }

        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string userid)
        {

            DataTable dt = BillAdjustmentsBAL.SearchAdjustmentforSales(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, userid);

            if (dgvSearch.Rows.Count > 0)
            {
                dgvSearch.DataSource = null;
            }

            dgvSearch.DataSource = dt;
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            dgvSearch.Columns[0].Visible = false;
            dgvSearch.Columns[1].Visible = true;
            dgvSearch.Columns[2].Visible = false;
            dgvSearch.Columns[1].HeaderText = "Customer Name";
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add(dt.Rows.Count);
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Estimationid"]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Quotationid"]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Customerid"]);
            //}

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.Font = new Font("Tahoma", 12.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.DefaultCellStyle.ForeColor = Color.Tomato;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
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
                if (dgvSearch.Rows.Count > 0)
                {
                    this.dgvSearch.Columns[1].Visible = false;
                    this.dgvSearch.Columns[2].Visible = false;
                }


            }
        }

        private void AdjustmentforSalesBill_Load(object sender, EventArgs e)
        {

        }

        private void cbxSearchOrderNo_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            DataTable dt = bindEstimation();


            if (cbxSearchOrderNo.SelectedIndex == 2)
            {
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Quotationid";
                cmbstatus1.ValueMember = "Quotationid";


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
            //DataTable dt = bindEstimation();
            //cmbstatus1.DataSource = dt;
            //cmbstatus1.DisplayMember = "Quotationid";
            //cmbstatus1.ValueMember = "Quotationid";
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
            DataTable dt = bindEstimation();

            if (cbxVendor.SelectedIndex == 2)
            {
                cmbstatus3.Visible = true;
                txtsearch3.Visible = false;
                ListSearchDate3.Visible = false;

                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Quotationid";
                cmbstatus3.ValueMember = "Quotationid";
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

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                for (int i = 0; i < dgvSearch.Rows.Count; i++)
                {
                    string Id = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    GetQuotationEstimationBills(Id);
                }
            }
        }

        public void GetQuotationEstimationBills(string CustomerId)
        {
            DataTable dt = AccountReceivableBAL.GetQuotationEstimationBills(CustomerId);
            dgvEstimationBill.DataSource = null;
            dgvEstimationBill.Columns.Clear();
            dgvEstimationBill.DataSource = dt;

            DataGridViewImageColumn img1 = new DataGridViewImageColumn();

            img1.Image = Inventory.Properties.Resources.user_edit;
            // dgvEstimationBill.Columns.Insert(7, img1);
            img1.HeaderText = "Action";
            img1.Name = "Action";
            dgvEstimationBill.Columns.Add(img1);

            dgvEstimationBill.Columns["TotalQuantity"].Visible = false;
            dgvEstimationBill.Columns["Customerid"].Visible = false;
            dgvEstimationBill.Columns["LessAmount"].Visible = false;
            dgvEstimationBill.Columns["Quotationid"].Visible = false;
            dgvEstimationBill.Columns["TotalAmount"].Visible = false;


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                dgvEstimationBill.Columns["Estimationid"].Width = 170;
                dgvEstimationBill.Columns["Date"].Width = 170;
                dgvEstimationBill.Columns["GrandTotal"].Width = 150;
                dgvEstimationBill.Columns["Paid"].Width = 150;
                dgvEstimationBill.Columns["Balance"].Width = 150;
                dgvEstimationBill.Columns["Action"].Width = 300;

            }
            else
            {
                dgvEstimationBill.Columns[0].Width = 100;
                dgvEstimationBill.Columns[1].Width = 100;
                dgvEstimationBill.Columns[2].Width = 150;
                dgvEstimationBill.Columns[3].Width = 150;
                dgvEstimationBill.Columns[4].Width = 150;
                dgvEstimationBill.Columns[5].Width = 150;
                this.dgvEstimationBill.Columns["Action"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            dgvEstimationBill.Columns["Estimationid"].HeaderText = "Bill No";
            dgvEstimationBill.Columns["Date"].HeaderText = "Bill Date";
            dgvEstimationBill.Columns["TotalAmount"].HeaderText = "Total";
            dgvEstimationBill.Columns["GrandTotal"].HeaderText = "Bill Amount";
            dgvEstimationBill.Columns["Paid"].HeaderText = "Paid";
            dgvEstimationBill.Columns["Balance"].HeaderText = "Balance";

            //dgvEstimationBill.Columns[0].Width = 160;
            //dgvEstimationBill.Columns[1].Width = 160;
            //dgvEstimationBill.Columns[2].Width = 100;
            //dgvEstimationBill.Columns[3].Width = 80;
            //dgvEstimationBill.Columns[4].Width = 80;
            //dgvEstimationBill.Columns[5].Width = 80;
            //dgvEstimationBill.Columns[6].Width = 80;
            //dgvEstimationBill.Columns[7].Width = 300;



            this.dgvEstimationBill.Columns["TotalQuantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvEstimationBill.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvEstimationBill.Columns["LessAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvEstimationBill.Columns["GrandTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvEstimationBill.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvEstimationBill.Columns["Paid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvEstimationBill.Columns["Action"].DisplayIndex = 10;

            dgvEstimationBill.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvEstimationBill.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvEstimationBill.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        }

        private void dgvEstimationBill_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvEstimationBill.Columns[e.ColumnIndex].HeaderText == "Action")
                {

                    for (int i = 0; i < dgvEstimationBill.Rows.Count; i++)
                    {
                        Estimationid = Convert.ToString(dgvEstimationBill.Rows[e.RowIndex].Cells["Estimationid"].Value);
                        Customerid = Convert.ToString(dgvEstimationBill.Rows[e.RowIndex].Cells["Customerid"].Value);
                        Quotationid = Convert.ToString(dgvEstimationBill.Rows[e.RowIndex].Cells["Quotationid"].Value);
                        GetCustomerOrderedDetails(Estimationid, Customerid);

                        lblCurrentBal.Text = Convert.ToString(Convert.ToDecimal(lbltotalamount.Text) - Convert.ToDecimal(txtPreviouseLess.Text));
                        GetNetAmount();
                    }
                }


            }
            //DataSet GetCustomerOrderedDetails(string Estimationid, string Customerid)
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

        public void GetCustomerOrderedDetails(string Estimationid, string Customerid)
        {
            bindreference();
            bindAssist();
            DataSet ds = BillAdjustmentsBAL.GetCustomerOrderedDetails(Estimationid, Customerid);
            if (ds.Tables[0].Rows.Count > 0)
            {
                txtOrderNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["Estimationid"]);
                Txtcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                cmdcity.Text = Convert.ToString(ds.Tables[0].Rows[0]["city"]);
                cmbreference.Text = Convert.ToString(ds.Tables[0].Rows[0]["Referenceid"]);
                cmbassistby.Text = Convert.ToString(ds.Tables[0].Rows[0]["Assist"]);
                lblperare.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);
                tatQuotationno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);
                date1.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);
                txtPreviouseLess.Text = Convert.ToString(ds.Tables[0].Rows[0]["LessAmount"]);
                lblTotal.Text = Convert.ToString(ds.Tables[0].Rows[0]["GrnandTotal"]);
                txtRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);

            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                decimal Total = 0.00M;
                dgvNew.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvNew.Rows.Add();
                    dgvNew.Rows[i].Cells[0].Value = i + 1;
                    dgvNew.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvNew.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvNew.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

                    double qty;
                    decimal amt = 0.00M;
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


                    amt = Convert.ToDecimal(ds.Tables[1].Rows[i]["Amount"]);
                    Total = Total + Convert.ToDecimal(amt);
                    dgvNew.Rows[i].Cells[6].Value = amt;

                }
                lbltotalamount.Text = Convert.ToString(Total);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validation())
            {
                if (string.IsNullOrEmpty(txtless.Text))
                {
                    txtless.Text = "0";
                }
                string AdjustmentAmount = txtless.Text;
                int result = BillAdjustmentsBAL.UpdateLessAmountforSales(Quotationid, Customerid, AdjustmentAmount);
                if (result > 0)
                {
                    MessageBox.Show("Saved Successfully.");
                    Clear();
                    LoadPortsNew();
                    LoadBill();
                }
            }

        }

        public void Clear()
        {
            bindreference();
            bindAssist();
            Txtcustomername.Text = "--Select--";
            cmdcity.Text = string.Empty;
            cmbassistby.SelectedIndex = 0;
            cmbreference.SelectedIndex = 0;

            dgvNew.Rows.Clear();
           // lblperare.Text = Program.userid;
            lbltotalamount.Text = "0.00";
            txtless.Text = "0.00";
            lblTotal.Text = "0.00";
            tatQuotationno.Text = string.Empty;
            Txtcustomername.Focus();
            txtOrderNo.Text = string.Empty;

            txtRemarks.Text = string.Empty;
            lblCurrentBal.Text = "0.00";
            txtless.Text = "0.00";
            txtPreviouseLess.Text = "0.00";
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

            if (string.IsNullOrEmpty(txtOrderNo.Text))
            {
                msg += "Please Select Bill." + "\n";
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

        private void txtless_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
            if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar)))
                e.Handled = true;


            //if (!(char.IsDigit(e.KeyChar)))
            //{
            //    //if (e.KeyChar != '\b')
            //    //{
            //    //    e.Handled = true;
            //    //}


            //    if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar))
            //                   && (e.KeyChar != '.') )
            //        e.Handled = true;


            //    //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            //    //{
            //    //    e.Handled = true;
            //    //}

            //    // only allow one decimal point
            //    if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //    {
            //        e.Handled = true;
            //    }

            //    // only allow minus sign at the beginning
            //    if (e.KeyChar == '-' && (sender as TextBox).Text.Length > 0)
            //    {
            //        e.Handled = true;
            //    }
            //}
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
            Clear();
            LoadPortsNew();
            LoadBill();
        }

        public void GetNetAmount()
        {
            if (string.IsNullOrEmpty(txtless.Text))
            {
                txtless.Text = "0";
            }
            if (Convert.ToString(lblCurrentBal.Text) != "0.00" && !string.IsNullOrEmpty(Convert.ToString(lblCurrentBal.Text)))
            {
                lblTotal.Text = Convert.ToString(Convert.ToDecimal(lblCurrentBal.Text) - Convert.ToDecimal(txtless.Text));
            }
        }
        private void txtless_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                GetNetAmount();
                btnSave.Focus();
            }
        }

        private void txtless_Leave(object sender, EventArgs e)
        {
            GetNetAmount();
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
                    lblTotal.Text = String.Format("{0:00.00}", grandtotal);



                }
                else
                {
                    lblTotal.Text = String.Format("{0:00.00}", lblCurrentBal.Text);
                }
            }
            catch
            {

            }
        }


    }
}
