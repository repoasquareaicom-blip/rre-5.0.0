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

namespace Inventory.Movement
{
    public partial class FrmIssuedReceivedReport : Form
    {
          FrmIssuedReceivedReportBAL objFrmIssuedReceivedReportBAL = new FrmIssuedReceivedReportBAL();
          string clickstatus = string.Empty;
          string role1 = string.Empty;
          string srole = string.Empty;
        public FrmIssuedReceivedReport()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            srole = Program.userid;
            if (srole == "1")
            {
                role1 = "Admin";
            }
            else
            {
                role1 = "Emp";
            }
            GetCustomers();
            LoadPorts();
            GetFrmIssuedReceivedReport(null);
            SearchCreteria1();
            search("CustomerID", "", role1);
            //SearchCreteria2();
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
        #endregion
        #region Bind
        private void LoadPorts()
        {
            dgvIssuedReceivedReport.Rows.Clear();

            dgvIssuedReceivedReport.Rows.Add(1);


            this.dgvIssuedReceivedReport.Columns[0].ReadOnly = true;
            this.dgvIssuedReceivedReport.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvIssuedReceivedReport.Columns[0].Width = 50;
           

            this.dgvIssuedReceivedReport.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvIssuedReceivedReport.Columns[1].Width = 250;
            this.dgvIssuedReceivedReport.Columns[1].ReadOnly = true;

            this.dgvIssuedReceivedReport.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvIssuedReceivedReport.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvIssuedReceivedReport.Columns[2].Width = 400;
            this.dgvIssuedReceivedReport.Columns[2].ReadOnly = true;

            this.dgvIssuedReceivedReport.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvIssuedReceivedReport.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvIssuedReceivedReport.Columns[3].Width = 100;
            this.dgvIssuedReceivedReport.Columns[3].ReadOnly = true;

            this.dgvIssuedReceivedReport.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvIssuedReceivedReport.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvIssuedReceivedReport.Columns[4].Width = 100;
            this.dgvIssuedReceivedReport.Columns[4].ReadOnly = true;

            this.dgvIssuedReceivedReport.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvIssuedReceivedReport.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvIssuedReceivedReport.Columns[5].Width = 100;
            this.dgvIssuedReceivedReport.Columns[5].ReadOnly = true;

            dgvIssuedReceivedReport.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvIssuedReceivedReport.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvIssuedReceivedReport.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            this.dgvIssuedReceivedReport.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvIssuedReceivedReport.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvIssuedReceivedReport.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            

            dgvIssuedReceivedReport.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvIssuedReceivedReport.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }

        }
        public void GetCustomers()
        {
            DataTable dtlocation = FrmIssuedReceivedReportBAL.GetCustomers();
            DataRow row = dtlocation.NewRow();

            row["CustomerID"] = "0";
            row["Name"] = "--Select--";
            dtlocation.Rows.InsertAt(row, 0);
            ddlcustomer.DataSource = dtlocation;
            ddlcustomer.ValueMember = "CustomerID";
            ddlcustomer.DisplayMember = "Name";
            ddlcustomer.SelectedIndex = 0;

            this.dgvIssuedReceivedReport.Columns[0].Width = 50;
            this.dgvIssuedReceivedReport.Columns[1].Width = 200;
            this.dgvIssuedReceivedReport.Columns[2].Width = 400;
            this.dgvIssuedReceivedReport.Columns[3].Width = 100;
            this.dgvIssuedReceivedReport.Columns[4].Width = 100;
            this.dgvIssuedReceivedReport.Columns[5].Width = 100;

            dgvIssuedReceivedReport.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvIssuedReceivedReport.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }

        }
        private void btnsech_Click(object sender, EventArgs e)
        {
            {
                if (ddlcustomer.SelectedIndex != 0)
                {

                    GetFrmIssuedReceivedReport(Convert.ToString(ddlcustomer.SelectedValue));
                }
                else
                {
                    GetFrmIssuedReceivedReport(null);
                    clear();
                }

            }
        }
        public void GetFrmIssuedReceivedReport(string id)
        {

            DataTable ds = objFrmIssuedReceivedReportBAL.GetFrmIssuedReceivedReport(id);
            if (ds.Rows.Count > 0)
            {

                ddlcustomer.Text = Convert.ToString(ds.Rows[0]["CustomerName"]);
                dgvIssuedReceivedReport.Rows.Clear();

                dgvIssuedReceivedReport.Rows.Add(ds.Rows.Count);

                for (int i = 0; i < ds.Rows.Count; i++)
                {

                    dgvIssuedReceivedReport.Rows[i].Cells[0].Value = i + 1;
                    dgvIssuedReceivedReport.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["CustomerName"]);
                    dgvIssuedReceivedReport.Rows[i].Cells[2].Value = ds.Rows[i]["DisplayName"];
                    dgvIssuedReceivedReport.Rows[i].Cells[3].Value = ds.Rows[i]["Issued"];
                    dgvIssuedReceivedReport.Rows[i].Cells[4].Value = ds.Rows[i]["Received"];
                    dgvIssuedReceivedReport.Rows[i].Cells[5].Value = ds.Rows[i]["Balance"];

                  
                }
                this.ActiveControl = ddlcustomer;
            }
            else
            {
                dgvIssuedReceivedReport.Rows.Clear();
                panel2.Enabled = true;
                // pnsearch.Visible = true;
            }



        }
        #endregion
        #region Processcmd
        [DllImport("user32.dll")]
        internal static extern IntPtr GetFocus();
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.C | Keys.Alt))
            {

                clear();

                return true;
            }
           
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion 
        #region Button
        private void btnclear_Click(object sender, EventArgs e)
        {
            GetFrmIssuedReceivedReport(null);
        }
        public void clear()
        {
            ddlcustomer.SelectedIndex = 0;
            LoadPorts();
        }

        private void prntbttn_Click(object sender, EventArgs e)
        {
            preview();
        }
        public void preview()
        {
            try
            {
                issuedreceivedrpt pos = new issuedreceivedrpt(lblhidden.Text);
                pos.Show();
            }
            catch
            {
                MessageBox.Show("Please Select The Item");
            }
        }
        #endregion       
        #region Search
        private void dataSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex>=0 )
            {
            string cusid=Convert.ToString(dgvissuedreceivedSearch.Rows[e.RowIndex].Cells[0].Value);
            lblhidden.Text = cusid;
            GetFrmIssuedReceivedReport(cusid);
            }
        }
        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("CustomerName");
            search.Add("CustomerID");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderNo.DataSource = bs.DataSource;
            cbxSearchOrderNo.SelectedIndex = 0;
          
        }
       
        private void cbxSearchOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ////if (cbxSearchOrderNo.SelectedIndex == 2)
            ////{
            ////    cmbstatus1.Visible = true;
            ////    txtsearch1.Visible = false;
            ////    ListSearchDate1.Visible = false;

            ////}
            ////else if (cbxSearchOrderNo.SelectedIndex == 1)
            ////{
            ////    cmbstatus1.Visible = false;
            ////    txtsearch1.Visible = true;
            ////    txtsearch1.Text = "Today";
            ////    ListSearchDate1.Visible = true;
            ////}
            ////else
            ////{
            ////    cmbstatus1.Visible = false;
            ////    txtsearch1.Visible = true;
            ////    txtsearch1.Text = string.Empty;
            ////    ListSearchDate1.Visible = false;
            ////}
            ////pnlCalender.Visible = false;
        }
        private void cbxSearchOrderDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cbxSearchOrderDate.SelectedIndex == 2)
            //{
            //    cmbstatus2.Visible = true;
            //    txtsearch2.Visible = false;
            //    ListSearchDate2.Visible = false;

            //}
            //else if (cbxSearchOrderDate.SelectedIndex == 1)
            //{
            //    cmbstatus2.Visible = false;
            //    txtsearch2.Visible = true;
            //    txtsearch2.Text = "Today";
            //    ListSearchDate2.Visible = true;
            //}
            //else
            //{
            //    cmbstatus2.Visible = false;
            //    txtsearch2.Visible = true;
            //    txtsearch2.Text = string.Empty;
            //    ListSearchDate2.Visible = false;
            //}

            //pnlCalender.Visible = false;
        }
        private void ListSearchDate1_Click(object sender, EventArgs e)
        {

            clickstatus = "search1";
            Calender();
            //pnlCalender.Location = new Point(133, 54);
        }
        private void ListSearchDate2_Click(object sender, EventArgs e)
        {
            clickstatus = "search2";
            Calender();
          //  pnlCalender.Location = new Point(133, 79);
        }
        private void Calender()
        {
            //if (pnlCalender.Visible)
            //{
            //    pnlCalender.Visible = false;
            //}
            //else
            //{
            //    pnlCalender.BringToFront();
            //    pnlCalender.Visible = true;
            //}
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ////if ((cbxSearchOrderNo.SelectedIndex == cbxSearchOrderDate.SelectedIndex))
            ////{
            ////    MessageBox.Show("*Search a item Should Not Be Same");
            ////}
            //else
            //{
            string firstname = string.Empty, firstvalue = string.Empty, secondname = string.Empty, secondvalue = string.Empty;
            string firstname1 = string.Empty, firstvalue1 = string.Empty, secondname1 = string.Empty, secondvalue1 = string.Empty;
            //    firstname = cbxSearchOrderNo.Text.Trim();
            //    if (firstname == "head.EnteredBy")
            //    {
            //        firstname = "head.EnteredBy";c
            firstname = Convert.ToString(cbxSearchOrderNo.SelectedValue);
            //        if (!string.IsNullOrEmpty(cmbstatus1.Text))
            //        {
            //            firstvalue = cmbstatus1.Text;
            //        }
            //    }
            //    else
            //    {
            firstvalue = txtsearch1.Text.Trim();
            //        string part1 = txtsearch1.Text.Trim();
            //        if (txtsearch1.Text.Trim() != "All")
            //        {
            //            part1 = txtsearch1.Text.Trim();
            //        }
            //        else
            //        {
            //            part1 = string.Empty;
            //            firstvalue = string.Empty;
            //        }
            //        if (!string.IsNullOrEmpty(part1))
            //        {
            //            string part = part1.Substring(0, 1);
            //            if (Char.IsDigit(part, 0))
            //            {
            //                string[] rr = txtsearch1.Text.Split('-');
            //                firstvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
            //            }
            //            else
            //            {
            //                firstvalue = txtsearch1.Text.Trim();
            //            }
            //        }
            //  }
            //    secondname = cbxSearchOrderDate.Text.Trim();
            //    if (secondname == "head.EnteredBy")
            //    {
            //        secondname = "head.EnteredBy";
            //        //secondvalue = Convert.ToString(cmbstatus2.SelectedValue);
            //        if (!string.IsNullOrEmpty(cmbstatus2.Text))
            //        {
            //            secondvalue = cmbstatus2.Text;
            //        }
            //    }
            //    else
            //    {
            //        //secondvalue = txtsearch2.Text.Trim();
            //        string part1 = txtsearch2.Text.Trim();
            //        if (txtsearch2.Text.Trim() != "All")
            //        {
            //            part1 = txtsearch2.Text.Trim();
            //        }
            //        else
            //        {
            //            part1 = string.Empty;
            //            secondvalue = string.Empty;
            //        }
            //        if (!string.IsNullOrEmpty(part1))
            //        {
            //            string part = part1.Substring(0, 2);
            //            if (Char.IsDigit(part, 0))
            //            {
            //                string[] rr = txtsearch2.Text.Split('-');
            //                secondvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
            //            }
            //            else
            //            {
            //                secondvalue = txtsearch2.Text.Trim();
            //            }
            //        }
            //    }


            //    if (firstname == "EnteredBy")
            //    {
            //        firstname1 = "head.EnteredBy";
            //        firstvalue1 = firstvalue;
            //    }
            //    else if (firstname == "EnteredOn")
            //    {
            //        secondname1 = "head.EnteredOn";
            //        secondvalue1 = firstvalue;
            //    }


            //    if (secondname == "EnteredBy")
            //    {
            //        firstname1 = "head.EnteredBy";
            //        firstvalue1 = secondvalue;
            //    }
            //    else if (secondname == "EnteredOn")
            //    {
            //        secondname1 = "head.EnteredOn";
            //        secondvalue1 = secondvalue;
            //    }


            //if(firstname=="CustomerID")
            //{
            //    firstname="cus.CustomerID";
            //}
            //else
            //{
            firstname = "CustomerID";
       //     }


            search(firstname, firstvalue, role1);
            //}
        }
        public void search(string firstname, string firstvalue, string secondname)
        {
            FrmIssuedReceivedReportBAL obj = new FrmIssuedReceivedReportBAL();
            DataTable dt = FrmIssuedReceivedReportBAL.searchFrmIssuedReceivedReportFinal(firstname, firstvalue, secondname);
            dgvissuedreceivedSearch.Columns.Clear();
            dgvissuedreceivedSearch.DataSource = dt;
          //  this.dgvissuedreceivedSearch.Columns[0].Visible = false;
          // this.dgvissuedreceivedSearch.Columns[0].Width = 400;

         //   this.dgvissuedreceivedSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
          //  this.dgvissuedreceivedSearch.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvissuedreceivedSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvissuedreceivedSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvissuedreceivedSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvissuedreceivedSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        private void txtsearch1_Click(object sender, EventArgs e)
        {
            //string selecteditem = cbxSearchOrderNo.Text.ToString();
            //if (selecteditem == "Date")
            //{
            //    clickstatus = "search1";
            //    pnlCalender.BringToFront();
            //    pnlCalender.Visible = true;
            //    pnlCalender.Location = new Point(133, 54);
            //}
            //else
            //{
            //    pnlCalender.Visible = false;
            //}
        }

        private void txtsearch2_Click(object sender, EventArgs e)
        {
            //if (cbxSearchOrderDate.Text.ToString() == "Date")
            //{
            //    clickstatus = "search2";
            //    pnlCalender.BringToFront();
            //    pnlCalender.Visible = true;
            //    pnlCalender.Location = new Point(133, 79);
            //}
            //else
            //{
            //    pnlCalender.Visible = false;
            //}
        }

        private void panel3_Click(object sender, EventArgs e)
        {

            //if (clickstatus == "search1")
            //{
            //    txtsearch1.Text = lblAll.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    txtsearch2.Text = lblAll.Text.Trim();
            //}

            //pnlCalender.Visible = false;
        }
        private void lblAll_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    txtsearch1.Text = lblAll.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    txtsearch2.Text = lblAll.Text.Trim();
            //}

            //pnlCalender.Visible = false;
        }
        private void lblToday_Click(object sender, EventArgs e)
        {

            //if (clickstatus == "search1")
            //{
            //    txtsearch1.Text = lblToday.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    txtsearch2.Text = lblToday.Text.Trim();
            //}

            //pnlCalender.Visible = false;
        }
        private void lblThisWeek_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    txtsearch1.Text = lblThisWeek.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    txtsearch2.Text = lblThisWeek.Text.Trim();
            //}

            //pnlCalender.Visible = false;
        }

        private void lblThisMonth_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    txtsearch1.Text = lblThisMonth.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    txtsearch2.Text = lblThisMonth.Text.Trim();
            //}

            //pnlCalender.Visible = false;
        }

        private void lblThisYear_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    txtsearch1.Text = lblThisYear.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    txtsearch2.Text = lblThisYear.Text.Trim();
            //}

            //pnlCalender.Visible = false;
        }

        private void lblYesterday_Click(object sender, EventArgs e)
        {

            //if (clickstatus == "search1")
            //{
            //    txtsearch1.Text = lblYesterday.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    txtsearch2.Text = lblYesterday.Text.Trim();
            //}

            //pnlCalender.Visible = false;
        }

        private void lblLastWeek_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    txtsearch1.Text = lblLastWeek.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    txtsearch2.Text = lblLastWeek.Text.Trim();
            //}

            //pnlCalender.Visible = false;
        }

        private void lblLastMonth_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    txtsearch1.Text = lblLastMonth.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    txtsearch2.Text = lblLastMonth.Text.Trim();
            //}

            //pnlCalender.Visible = false;
        }

        private void lblLastYear_Click(object sender, EventArgs e)
        {
        //    if (clickstatus == "search1")
        //    {
        //        txtsearch1.Text = lblLastYear.Text.Trim();
        //    }
        //    else if (clickstatus == "search2")
        //    {
        //        txtsearch2.Text = lblLastYear.Text.Trim();
        //    }

        //    pnlCalender.Visible = false;
        }

        private void SearchFrmDate_ValueChanged(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    txtsearch1.Text = SearchFrmDate.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    txtsearch2.Text = SearchFrmDate.Text.Trim();
            //}

            //pnlCalender.Visible = false;
        }

        #endregion

        private void FrmIssuedReceivedReport_Load(object sender, EventArgs e)
        {

        }


    }
}
