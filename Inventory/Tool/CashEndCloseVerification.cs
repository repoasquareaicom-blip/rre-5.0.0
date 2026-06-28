using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using InvDal;
//using Inventory.AccountReceivableReport;
using Inventory.Accounts;

namespace Inventory.Tool
{
    public partial class CashEndCloseVerification : Form
    {
        private string clickstatus;
        string role = "Emp";
        string UserId = "1";
        public CashEndCloseVerification()
        {

            CashEndCloseBAL ObjCashEndCloseBAL = new CashEndCloseBAL();
            string HdnCashRequestId = string.Empty;
            string HdnAmount = string.Empty;
            string clickstatus = string.Empty;

            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            GetcashEndTable();
            GetcashEndTablesearch();
            SearchCreteria1();
             SearchCreteria2();

                 pnlLabelSearch.Visible = true;
                 vLabel1.Visible = true;
                 pnlSearch.Visible = false;
                 splitContainer1.Panel1Collapsed = true;
             
        }

        public void GetcashEndTable()
        {
           DataTable dt = CashEndCloseBAL.GetCashEndClose();
           dgvCashEndCloseVerification.Columns.Clear();
           dgvCashEndCloseVerification.DataSource = dt;
           dgvCashEndCloseVerification.Columns["Id"].Visible = false;
           DataGridViewImageColumn img1 = new DataGridViewImageColumn();
          
           img1.Image = Inventory.Properties.Resources.user_edit;
       
           dgvCashEndCloseVerification.Columns.Insert(0, img1);
           img1.HeaderText = "Verification";
           img1.Name = "Verification";
           
           dgvCashEndCloseVerification.Columns["AccountingDate"].HeaderText = "Accounting_Date";
           dgvCashEndCloseVerification.Columns["GrossBalance"].HeaderText = "Gross_Balance";
           dgvCashEndCloseVerification.Columns["ClosingBalance"].HeaderText = "Closing_Balance";

           dgvCashEndCloseVerification.Columns["Verification"].Width = 175;


           dgvCashEndCloseVerification.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10.1F, FontStyle.Bold);
           dgvCashEndCloseVerification.DefaultCellStyle.Font = new Font("Tahoma", 12.1F, GraphicsUnit.Pixel);
           dgvCashEndCloseVerification.DefaultCellStyle.BackColor = Color.Gainsboro;
           dgvCashEndCloseVerification.DefaultCellStyle.ForeColor = Color.Black;
           dgvCashEndCloseVerification.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            //dgvCashEndCloseVerification.Columns["Adjustment"].Width = 40;
            //dgvCashEndCloseVerification.Columns["ClosingBalance"].Width = 100;
            //dgvCashEndCloseVerification.Columns["Thousand"].Width = 40;
            //dgvCashEndCloseVerification.Columns["FiveHundred"].Width = 40;
            //dgvCashEndCloseVerification.Columns["Hundred"].Width = 100;
            //dgvCashEndCloseVerification.Columns["Fifty"].Width = 40;
            //dgvCashEndCloseVerification.Columns["Twenty"].Width = 40;
            //dgvCashEndCloseVerification.Columns["Ten"].Width = 100;
            //dgvCashEndCloseVerification.Columns["Five"].Width = 100;
            //dgvCashEndCloseVerification.Columns["Coin"].Width = 40;
            //dgvCashEndCloseVerification.Columns["Remarks"].Width = 40;
           
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        public void GetcashEndTablesearch()
        {
            DataTable dt = CashEndCloseBAL.GetCashEndClose();
            dgvCashSearch.Columns.Clear();
            dgvCashSearch.DataSource = dt;
            dgvCashSearch.Columns["Id"].Visible = false;
            dgvCashSearch.Columns["GrossBalance"].Visible = false;
            dgvCashSearch.Columns["Adjustment"].Visible = false;
            dgvCashSearch.Columns["ClosingBalance"].Visible = false;
            dgvCashSearch.Columns["TwoThousand"].Visible = false;
            dgvCashSearch.Columns["Thousand"].Visible = false;
            dgvCashSearch.Columns["FiveHundred"].Visible = false;
            dgvCashSearch.Columns["Hundred"].Visible = false;
            dgvCashSearch.Columns["Fifty"].Visible = false;
            dgvCashSearch.Columns["Twenty"].Visible = false;
            dgvCashSearch.Columns["Ten"].Visible = false;
            dgvCashSearch.Columns["Five"].Visible = false;
            dgvCashSearch.Columns["Two"].Visible = false;
            dgvCashSearch.Columns["One"].Visible = false;
            dgvCashSearch.Columns["Remarks"].Visible = false;
            dgvCashSearch.Columns["AccountingDate"].HeaderText = "Accounting Date";

            dgvCashSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvCashSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvCashSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }

        private void cbxSearchOrderDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchAccountingDate.SelectedIndex == 2)
            {
                cmbstatus2.Visible = true;
                txtsearch2.Visible = false;
                ListSearchDate2.Visible = false;

            }
            else if (cbxSearchAccountingDate.SelectedIndex == 1)
            {
                cmbstatus2.Visible = false;
                txtsearch2.Visible = true;
                txtsearch2.Text = SearchFrmDate.Value.ToString("dd/MM/yyyy");
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

        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("Type");
            search.Add("AccountingDate");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchType.DataSource = bs.DataSource;
            cbxSearchType.SelectedIndex = 0;
            cmbstatus1.Visible = false;
        }

        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("Type");
            search.Add("AccountingDate");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchAccountingDate.DataSource = bs.DataSource;
            cbxSearchAccountingDate.SelectedIndex = 1;
            cmbstatus2.Visible = false;
        }

        private void cbxSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchType.SelectedIndex == 2)
            {
                cmbstatus1.Visible = true;
                txtsearch1.Visible = false;
                ListSearchDate1.Visible = false;

            }
            else if (cbxSearchType.SelectedIndex == 1)
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

       

        private void cbxSearchAccountingDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchAccountingDate.SelectedIndex == 2)
            {
                cmbstatus2.Visible = true;
                txtsearch2.Visible = false;
                ListSearchDate2.Visible = false;

            }
            else if (cbxSearchAccountingDate.SelectedIndex == 1)
            {
                cmbstatus2.Visible = false;
                txtsearch2.Visible = true;
                txtsearch2.Text = SearchFrmDate.Value.ToString("dd/MM/yyyy");
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

        //private void txtsearch1_Click_1(object sender, EventArgs e)
        //{
        //    string selecteditem = cbxSearchType.Text.ToString();
        //    if (selecteditem == "Date")
        //    {
        //        clickstatus = "search1";
        //        pnlCalender.BringToFront();
        //        pnlCalender.Visible = true;
        //        pnlCalender.Location = new Point(133, 54);
        //    }
        //    else
        //    {
        //        pnlCalender.Visible = false;
        //    }

        //}
        
        ////private void txtsearch2_Click(object sender, EventArgs e)
        ////{

        ////    if (cbxSearchAccountingDate.Text.ToString() == "Date")
        ////    {
        ////        clickstatus = "search2";
        ////        pnlCalender.BringToFront();
        ////        pnlCalender.Visible = true;
        ////        pnlCalender.Location = new Point(133, 79);
        ////    }
        ////    else
        ////    {
        ////        pnlCalender.Visible = false;
        ////    }
        ////}

        private void txtsearch1_Click(object sender, EventArgs e)
        {
            string selecteditem = cbxSearchType.Text.ToString();
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
        private void lblAll_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblAll.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblAll.Text.Trim();
            }          

            pnlCalender.Visible = false;

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

            pnlCalender.Visible = false;
        }

        private void label28_Click(object sender, EventArgs e)
        

        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = SearchFrmDate.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = SearchFrmDate.Text.Trim();
            }

            pnlCalender.Visible = false;

        }

        private void txtsearch2_Click(object sender, EventArgs e)
        {
            if (cbxSearchAccountingDate.Text.ToString() == "Date")
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
                //pnlPurchaseReceipt.Visible = true;
                vLabel2.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;
                //this.dvgsearchcashpayment.Columns["RequestId"].Visible = true;
                //this.dvgsearchcashpayment.Columns["Amount"].Visible = true;
                //this.dvgsearchcashpayment.Columns["Status"].Visible = true;
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if ((cbxSearchType.SelectedIndex == cbxSearchAccountingDate.SelectedIndex))
            {
                MessageBox.Show("* Search a item Should Not Be Same");
            }
            else
            {
                string firstname = string.Empty, firstvalue = string.Empty, secondname = string.Empty, secondvalue = string.Empty, thirdname = string.Empty, thirdvalue = string.Empty;
                string firstname1 = string.Empty, firstvalue1 = string.Empty, secondname1 = string.Empty, secondvalue1 = string.Empty, thirdname1 = string.Empty, thirdvalue1 = string.Empty;

                firstname = cbxSearchType.Text.Trim();
                if (firstname == "Status")
                {
                    firstname = "Status";
                    //firstvalue = Convert.ToString(cmbstatus1.SelectedValue);
                    if (!string.IsNullOrEmpty(cmbstatus1.Text))
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
                        string part = part1.Substring(0,1);
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

                secondname = cbxSearchAccountingDate.Text.Trim();
                if (secondname == "Status")
                {
                    secondname = "Status";
                    //secondvalue = Convert.ToString(cmbstatus2.SelectedValue);
                    if (!string.IsNullOrEmpty(cmbstatus2.Text))
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




                search(firstname, firstvalue, secondname, secondvalue,role,UserId);
            }
        }

        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string role, string UserId)
        {
            CashEndCloseBAL obj = new CashEndCloseBAL();
            DataTable dt = obj.searchCashEndClose(firstname, firstvalue, secondname, secondvalue, role, UserId);
            dgvCashSearch.Columns.Clear();
            dgvCashSearch.DataSource = dt;

            dgvCashSearch.Columns["Id"].Visible = false;
            dgvCashSearch.Columns["GrossBalance"].Visible = false;
            dgvCashSearch.Columns["Adjustment"].Visible = false;
            dgvCashSearch.Columns["ClosingBalance"].Visible = false;
            dgvCashSearch.Columns["TwoThousand"].Visible = false;
            dgvCashSearch.Columns["Thousand"].Visible = false;
            dgvCashSearch.Columns["FiveHundred"].Visible = false;
            dgvCashSearch.Columns["Hundred"].Visible = false;
            dgvCashSearch.Columns["Fifty"].Visible = false;
            dgvCashSearch.Columns["Twenty"].Visible = false;
            dgvCashSearch.Columns["Ten"].Visible = false;
            dgvCashSearch.Columns["Five"].Visible = false;
            dgvCashSearch.Columns["Two"].Visible = false;
            dgvCashSearch.Columns["One"].Visible = false;
            dgvCashSearch.Columns["Remarks"].Visible = false;


            //dgvCashSearch.Columns["AccountingDate"].Width = 175;
            //dgvCashSearch.Columns["Type"].Width = 175;

            dgvCashSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvCashSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvCashSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvCashSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void CashEndCloseVerification_Load(object sender, EventArgs e)
        {

        }

        private void dgvCashEndCloseVerification_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int status = 0;
            CashEndCloseBAL ObjCashEndCloseBAL = new CashEndCloseBAL();
            if (e.RowIndex >= 0)
            {
                if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Verification")
                {

                    for (int i = 0; i < dgvCashEndCloseVerification.Rows.Count; i++)
                    {            
                        ObjCashEndCloseBAL.Id = Convert.ToString(dgvCashEndCloseVerification.Rows[e.RowIndex].Cells["Id"].Value);
                        ObjCashEndCloseBAL.Type = "C";
                        ObjCashEndCloseBAL.ClosingBalance = Convert.ToDecimal(dgvCashEndCloseVerification.Rows[e.RowIndex].Cells["ClosingBalance"].Value);
                        ObjCashEndCloseBAL.UserId = Program.userlevel;
                        status= CashEndCloseBAL.CashEndCloseVerification(ObjCashEndCloseBAL);
                    }

                    
                        MessageBox.Show("Verified Successfully");
                        GetcashEndTable(); 
                    
                }
            }
        }

        private void vLabel2_Click_1(object sender, EventArgs e)
        {

        }

        private void dgvCashEndCloseVerification_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "AccountingDate")
              {
                  dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 100;
              }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "GrossBalance")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 50;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Type")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 60;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Adjustment")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "ClosingBalance")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 150;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "TwoThousand")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Thousand")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "FiveHundred")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 70;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Hundred")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 90;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Fifty")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Twenty")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Ten")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 70;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Five")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 90;
            }
            //if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "CoinRemarks")
            //{
            //    dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 100;
            //}
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Two")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "One")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Remarks")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 100;
            }

            if (dgvCashEndCloseVerification.Columns[e.ColumnIndex].HeaderText == "Verification")
            {
                dgvCashEndCloseVerification.Columns[e.ColumnIndex].Width = 100;
            }
        }

        

        
        
    }
        
    }

