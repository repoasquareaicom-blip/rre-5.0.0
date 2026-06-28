using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace Inventory.Transactions
{
    public partial class CashRequestApproval : Form
    {
        TransactionBAL ObjTransactionBAL = new TransactionBAL();
        AutoCompleteStringCollection namesCollection = new AutoCompleteStringCollection();
        string clickstatus = string.Empty;
        string role = "Emp";
        string UserId = "1";
        public CashRequestApproval()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            BindUsers();
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            LoadCra();
                   
        }

        /* Left Side Content */
        public void BindUsers()
        {
            DataTable dReader = ObjTransactionBAL.AutoCompleteBindUsers();
            if (dReader.Rows.Count > 0)
            {
                for (int i = 0; i < dReader.Rows.Count; i++)
                {
                    namesCollection.Add(dReader.Rows[i]["Name"].ToString());
                }
            }            
            txtsearch1.AutoCompleteCustomSource = namesCollection;
            txtsearch2.AutoCompleteCustomSource = namesCollection;
            txtsearch3.AutoCompleteCustomSource = namesCollection;
        }

        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("RequestedBy");
            search.Add("Date");
            search.Add("Status");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderNo.DataSource = bs.DataSource;
            cbxSearchOrderNo.SelectedIndex = 0;
            cmbstatus1.Visible = false;
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

        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("RequestedBy");
            search.Add("Date");
            search.Add("Status");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderDate.DataSource = bs.DataSource;
            cbxSearchOrderDate.SelectedIndex = 1;
            cmbstatus2.Visible = false;
        }

        private void SearchCreteria3()
        {
            List<string> search = new List<string>();
            search.Add("RequestedBy");
            search.Add("Date");
            search.Add("Status");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxVendor.DataSource = bs.DataSource;
            cbxVendor.SelectedIndex = 2;
            txtsearch3.Visible = false;
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
                pnlPurchaseReceipt.Visible = true;
                vLabel2.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;
                this.dvgsearchcashapproval.Columns["RequestId"].Visible = true;
                this.dvgsearchcashapproval.Columns["Amount"].Visible = true;
                this.dvgsearchcashapproval.Columns["Status"].Visible = true;
            }
        }

        private void vLabel2_Click(object sender, EventArgs e)
        {
            if (pnlPurchaseReceipt.Visible == true)
            {
                pnlPurchaseReceipt.Visible = false;
                vLabel2.Visible = false;
                pnlCollapse2.Visible = true;
                splitContainer1.Panel2Collapsed = false;
                pbxCollapse.Visible = true;
                pbxRightCollapse.Visible = true;
                this.dvgsearchcashapproval.Columns["RequestId"].Visible = true;
                this.dvgsearchcashapproval.Columns["Amount"].Visible = true;
                this.dvgsearchcashapproval.Columns["Status"].Visible = true;

            }
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
      
        /* Left Side Content */
        /* Right Side Content */
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
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblAll.Text.Trim();
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

        private void ListSearchDate1_Click_1(object sender, EventArgs e)
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
        /* Right Side Content */

        private void LoadCra()
        {

            DataTable dt = ObjTransactionBAL.BindTransactionCashRequest();
            if (dgvCRA.Columns.Count == 0 && dgvCRA.Rows.Count == 0)
            {
                dgvCRA.DataSource = dt;
                DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
                cmb.HeaderText = "Status";
                cmb.Name = "cmb";
                cmb.MaxDropDownItems = 4;
                cmb.Items.Add("-Select-");
                cmb.Items.Add("Approve");
                cmb.Items.Add("Reject");
                cmb.FlatStyle = FlatStyle.Standard;
                dgvCRA.Columns.Add(cmb);
                cmb.DefaultCellStyle.NullValue = "-Select-";


                DataGridViewButtonColumn btn = new DataGridViewButtonColumn();

                btn.Text = "Save";
                btn.HeaderText = "Action";
                btn.Name = "send";
                btn.UseColumnTextForButtonValue = true;
                btn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                btn.FlatStyle = FlatStyle.Popup;
                btn.CellTemplate.Style.BackColor = Color.Honeydew;
                dgvCRA.Columns.Add(btn);

                dgvCRA.Columns[0].Visible = false;
                //dgvCRA.Columns["Amount"].Visible = false;


                this.dgvCRA.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                //this.dgvCRA.Columns[1].Width = 150;
                this.dgvCRA.Columns[1].ReadOnly = true;

                this.dgvCRA.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
                //this.dgvCRA.Columns[2].Width = 180;
                this.dgvCRA.Columns[2].ReadOnly = true;

                this.dgvCRA.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                //this.dgvCRA.Columns[3].Width = 400;
                this.dgvCRA.Columns[3].ReadOnly = true;

                this.dgvCRA.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
                //this.dgvCRA.Columns[4].Width = 100;
                this.dgvCRA.Columns[4].ReadOnly = true;
                this.dgvCRA.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                this.dgvCRA.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
                //this.dgvCRA.Columns[5].Width = 90;
                //dgvCRA.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dgvCRA_EditingControlShowing);

                dgvCRA.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

                search("RequestedBy", "", "Date", "", "Status", "",role,UserId);

                foreach (DataGridViewColumn c in dgvCRA.Columns)
                {
                    c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
                }
            }
            else
            {
                dgvCRA.DataSource = dt;
            }
            dgvCRA.Columns["RequestedBy"].Visible = false;
        }

        //private void dgvCRA_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        //{
        //    ComboBox combo = e.Control as ComboBox;
        //    if (combo != null)
        //    {
        //        combo.SelectedIndexChanged -= new EventHandler(ComboBox_SelectedIndexChanged);
        //        combo.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
        //    }
        //}

        //private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ComboBox cb = (ComboBox)sender;
        //    string item = cb.Text;
        //    if (item != null)
        //    {
        //        if (item != "Select")
        //        {
        //            DataGridViewTextBoxCell thisCboCell = (DataGridViewTextBoxCell)dgvCRA.CurrentRow.Cells["CashRequestId"];
        //            if (thisCboCell.Value != null)
        //            {
        //                string ModifiedBy = "1";
        //                int Id = Convert.ToInt32(thisCboCell.Value.ToString());
        //                bool status = ObjTransactionBAL.SaveTransactionCashRequestApproval(Id, item, ModifiedBy);
        //                LoadCra();
        //            }
        //        }
        //    }
        //}

        private void dgvCRA_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvCRA.Columns[e.ColumnIndex].Name == "send")
                {
                    DataGridViewComboBoxCell thisCbCell = (DataGridViewComboBoxCell)dgvCRA.CurrentRow.Cells["cmb"];
                    if (thisCbCell.Value != null)
                    {
                        string item = (string)thisCbCell.Value;
                        DataGridViewTextBoxCell thisCboCell = (DataGridViewTextBoxCell)dgvCRA.CurrentRow.Cells["CashRequestId"];
                        if (thisCboCell.Value != null)
                        {
                            string Id = Convert.ToString(thisCboCell.Value.ToString());
                            string ModifiedBy = Program.userid;
                            bool status = ObjTransactionBAL.SaveTransactionCashRequestApproval(Id, item, ModifiedBy);
                            LoadCra();
                        }
                    }
                }
            }
        }
      
      /*Search Button Action*/
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
                 
                 firstname = cbxSearchOrderNo.Text.Trim();
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


                 thirdname = cbxVendor.Text.Trim();
                 if (thirdname == "Status")
                 {
                     thirdname = "Status";
                     if (!string.IsNullOrEmpty(cmbstatus3.Text))
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

                 if (firstname == "RequestedBy")
                 {
                     firstname1 = firstname;
                     firstvalue1 = firstvalue;
                 }
                 else if (firstname == "Date")
                 {
                     secondname1 = firstname;
                     secondvalue1 = firstvalue;
                 }
                 else if (firstname == "Status")
                 {
                     thirdname1 = firstname;
                     thirdvalue1 = firstvalue;
                 }

                 if (secondname == "RequestedBy")
                 {
                     firstname1 = secondname;
                     firstvalue1 = secondvalue;
                 }
                 else if (secondname == "Date")
                 {
                     secondname1 = secondname;
                     secondvalue1 = secondvalue;
                 }
                 else if (secondname == "Status")
                 {
                     thirdname1 = secondname;
                     thirdvalue1 = secondvalue;
                 }

                 if (thirdname == "RequestedBy")
                 {
                     firstname1 = thirdname;
                     firstvalue1 = thirdvalue;
                 }
                 else if (thirdname == "Date")
                 {
                     secondname1 = thirdname;
                     secondvalue1 = thirdvalue;
                 }
                 else if (thirdname == "Status")
                 {
                     thirdname1 = thirdname;
                     thirdvalue1 = thirdvalue;
                 }

                 search(firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1, role, UserId);
             }
        }
        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            TransactionBAL obj = new TransactionBAL();
            DataTable dt = obj.searchcashApproval(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role, UserId);
            dvgsearchcashapproval.Columns.Clear();
            dvgsearchcashapproval.DataSource = dt;

            dvgsearchcashapproval.Columns["Reason"].Visible = false;
            dvgsearchcashapproval.Columns["RequestId"].Visible = false;
            dvgsearchcashapproval.Columns["Amount"].Visible = false;
            //dvgsearchcashapproval.Columns["RequestedBy"].Width = 175;
           // dvgsearchcashapproval.Columns["Date"].Width = 175;

            dvgsearchcashapproval.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dvgsearchcashapproval.DefaultCellStyle.BackColor = Color.Gainsboro;
            dvgsearchcashapproval.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dvgsearchcashapproval.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void dvgcashapproval_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                return;
            }
            var dataGridView = (sender as DataGridView);
            //Check the condition as per the requirement casting the cell value to the appropriate type

            dataGridView.Cursor = Cursors.Hand;
        }

        private void dgvCRA_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
                if (e.RowIndex >= 0)
                {
                    try
                    {
                        if (dgvCRA[e.ColumnIndex, e.RowIndex].EditType.ToString() == "System.Windows.Forms.DataGridViewComboBoxEditingControl")
                        {
                            DataGridViewColumn column = dgvCRA.Columns[e.ColumnIndex];
                            if (column is DataGridViewComboBoxColumn)
                            {
                                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dgvCRA[e.ColumnIndex, e.RowIndex];
                                dgvCRA.CurrentCell = cell;
                                dgvCRA.BeginEdit(true);
                                DataGridViewComboBoxEditingControl editingControl = (DataGridViewComboBoxEditingControl)dgvCRA.EditingControl;
                                editingControl.DroppedDown = true;
                            }
                        }
                    }
                    catch
                    {
                        //MessageBox.Show("Please Select Status");
                    }
                }
          
        }

        private void btnsave_Click(object sender, EventArgs e)
        {

        }
        /*Search Button Action*/
    }
}
