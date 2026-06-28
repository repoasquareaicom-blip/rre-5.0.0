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
    public partial class TransactionRequest : Form
    {
        TransactionBAL ObjTransactionBAL = new TransactionBAL();
        
        string HdnCashRequestId = string.Empty;
        public TransactionRequest()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            //LoadCRA();
            //LoadCRP();
            LoadDenomination();
        }

        private void LoadCRA()
        {
            dgvCRA.Rows.Clear();
            dgvCRA.Columns.Clear();
            dgvCRA.RowCount = 4;
            dgvCRA.ColumnCount = 5;

            dgvCRA.Columns[0].Name = "Id";
            dgvCRA.Columns[1].Name = "Date";
            dgvCRA.Columns[2].Name = "Requested By";
            dgvCRA.Columns[3].Name = "Reason";
            dgvCRA.Columns[4].Name = "Amount";

            this.dgvCRA.Columns[0].Visible = false;
            this.dgvCRA.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRA.Columns[1].Width = 150;
            this.dgvCRA.Columns[1].ReadOnly = true;

            this.dgvCRA.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRA.Columns[2].Width = 180;
            this.dgvCRA.Columns[2].ReadOnly = true;

            this.dgvCRA.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRA.Columns[3].Width = 450;
            this.dgvCRA.Columns[3].ReadOnly = true;

            this.dgvCRA.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRA.Columns[4].Width = 130;
            this.dgvCRA.Columns[4].ReadOnly = true;
            this.dgvCRA.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
            cmb.HeaderText = "";
            cmb.Name = "cmb";
            cmb.MaxDropDownItems = 4;
            cmb.Items.Add("Approve");
            cmb.Items.Add("Reject");
            dgvCRA.Columns.Add(cmb);

            this.dgvCRA.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRA.Columns[4].Width = 110;

            //this.dgvCRA.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvCRA.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvCRA.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvCRA.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvCRA.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            dgvCRA.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvCRA.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }

        private void LoadCRP()
        {
            dgvCRP.Rows.Clear();
            dgvCRP.Columns.Clear();
            dgvCRP.RowCount = 6;
            dgvCRP.ColumnCount = 6;

            dgvCRP.Columns[0].Name = "Date";
            dgvCRP.Columns[1].Name = "Requested By";
            dgvCRP.Columns[2].Name = "Reason";
            dgvCRP.Columns[3].Name = "Amount";
            dgvCRP.Columns[4].Name = "Approved By";
            dgvCRP.Columns[5].Name = "Approved On";

            this.dgvCRP.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[0].Width = 150;
            this.dgvCRP.Columns[0].ReadOnly = true;

            this.dgvCRP.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[1].Width = 150;
            this.dgvCRP.Columns[1].ReadOnly = true;

            this.dgvCRP.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[2].Width = 350;
            this.dgvCRP.Columns[2].ReadOnly = true;

            this.dgvCRP.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[3].Width = 100;
            this.dgvCRP.Columns[3].ReadOnly = true;
            this.dgvCRP.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvCRP.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[4].Width = 100;
            this.dgvCRP.Columns[4].ReadOnly = true;

            this.dgvCRP.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[5].Width = 130;
            this.dgvCRP.Columns[5].ReadOnly = true;

            DataGridViewButtonColumn btn = new DataGridViewButtonColumn();

            btn.Text = "Pay Cash";
            btn.HeaderText = "Pay Cash";
            btn.Name = "PayCash";
            btn.UseColumnTextForButtonValue = true;
            btn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            btn.FlatStyle = FlatStyle.Standard;
            btn.CellTemplate.Style.BackColor = Color.Honeydew;
            dgvCRP.Columns.Add(btn);

            this.dgvCRP.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[4].Width = 110;

            dgvCRP.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvCRP.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }

        private void LoadDenomination()
        {
            DataTable dtDenom = new DataTable();
            dtDenom.Columns.Add("Denomination", typeof(string));
            dtDenom.Columns.Add("NoOfCurrency", typeof(string));
            dtDenom.Rows.Add("1000", "");
            dtDenom.Rows.Add("500", "");
            dtDenom.Rows.Add("100", "");
            dtDenom.Rows.Add("50", "");
            dtDenom.Rows.Add("20", "");
            dtDenom.Rows.Add("10", "");
            dtDenom.Rows.Add("5", "");
            dtDenom.Rows.Add("Coins", "");
            dgvDenomination.DataSource = dtDenom;
            this.dgvDenomination.Columns[0].ReadOnly = true;

            //this.dgvCRP.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvCRP.Columns[0].Width = 150;
            //

            //this.dgvCRP.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvCRP.Columns[1].Width = 150;

            //dgvCRP.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            //foreach (DataGridViewColumn c in dgvCRP.Columns)
            //{
            //    c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            //}
        }

        private void dgvCRP_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvCRP.Columns[e.ColumnIndex].HeaderText == "Pay Cash")
                {
                    HdnCashRequestId = Convert.ToString(dgvCRP.Rows[e.RowIndex].Cells["CashRequestId"].Value);
                    a1Panel1.Visible = true;
                }
            }            
        }

        private void a1Panel1_Click(object sender, EventArgs e)
        {
            a1Panel1.Visible = false;
            a1Panel2.Visible = false;
        }

        private void rbCash_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCash.Checked)
            {
                a1Panel2.Visible = true;
            }
            else
            {
                a1Panel2.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Validation())
            {
                TransactionBAL ObjTranBAL = new TransactionBAL();
                ObjTranBAL.RequestedBy = txtrequest.Text;
                DateTime date = new DateTime(txtdate.Value.Year, txtdate.Value.Month, txtdate.Value.Day);
                ObjTranBAL.CashRequestedDate = date;
                ObjTranBAL.Amount = txtamount.Text;
                ObjTranBAL.Reason = txtreson.Text;
                ObjTranBAL.Updatedby = "1";
                int st = ObjTransactionBAL.SaveTransactionCashRequest(ObjTranBAL);
                if (st == 1)
                {
                    MessageBox.Show("Request Send Successfully");
                }
                else if (st == 0)
                {
                    MessageBox.Show("Request Already Successfully");
                }
            }
        }

        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(txtrequest.Text))
            {
                i++;
                message = message + "*Enter Request" + "\n";
                if (i == 1)
                    this.ActiveControl = txtrequest;
            }
            if (string.IsNullOrEmpty(txtamount.Text))
            {
                i++;
                message = message + "*Enter Amount" + "\n";
                if (i == 1)
                    this.ActiveControl = txtamount;
            }
            if (string.IsNullOrEmpty(txtreson.Text))
            {
                i++;
                message = message + "*Enter Reason" + "\n";
                if (i == 1)
                    this.ActiveControl = txtreson;
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message);
                status = false;
            }
            return status;
        }

        private void txtamount_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtamount1_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtamountrupe_KeyPress(object sender, KeyPressEventArgs e)
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

        private void button5_Click(object sender, EventArgs e)// save au bending
        {
            if (Validation())
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtamount.Clear();
            txtrequest.Clear();
            txtreson.Clear();
        }

        private void Transaction_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtrequest;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (btnprint.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = txtrequest;
                    return true;
                }

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                LoadCra();
            }
            else if(tabControl1.SelectedIndex == 2)
            {
                LoadCrp();
            }
        }

        private void LoadCra()
        {

            DataTable dt = ObjTransactionBAL.BindTransactionCashRequest();
            if (dgvCRA.Columns.Count == 0 && dgvCRA.Rows.Count == 0)
            {
                dgvCRA.DataSource = dt;
                DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
                cmb.HeaderText = "";
                cmb.Name = "cmb";
                cmb.MaxDropDownItems = 4;
                cmb.Items.Add("Select");
                cmb.Items.Add("Approve");
                cmb.Items.Add("Reject");
                dgvCRA.Columns.Add(cmb);
                cmb.DefaultCellStyle.NullValue = "Select";


                DataGridViewButtonColumn btn = new DataGridViewButtonColumn();

                btn.Text = "Send";
                btn.HeaderText = "";
                btn.Name = "send";
                btn.UseColumnTextForButtonValue = true;
                btn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                btn.FlatStyle = FlatStyle.Standard;
                btn.CellTemplate.Style.BackColor = Color.Honeydew;
                dgvCRA.Columns.Add(btn);

                dgvCRA.Columns[0].Visible = false;

                this.dgvCRA.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRA.Columns[1].Width = 150;
                this.dgvCRA.Columns[1].ReadOnly = true;

                this.dgvCRA.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRA.Columns[2].Width = 180;
                this.dgvCRA.Columns[2].ReadOnly = true;

                this.dgvCRA.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRA.Columns[3].Width = 400;
                this.dgvCRA.Columns[3].ReadOnly = true;

                this.dgvCRA.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRA.Columns[4].Width = 100;
                this.dgvCRA.Columns[4].ReadOnly = true;
                this.dgvCRA.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                this.dgvCRA.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRA.Columns[5].Width = 90;
                //dgvCRA.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dgvCRA_EditingControlShowing);
            }
            else
            {
                dgvCRA.DataSource = dt;
            }
        }

        private void LoadCrp()
        {
            DataTable dt = ObjTransactionBAL.BindTransactionCashApproval();
            if (dgvCRP.Columns.Count == 0 && dgvCRP.Rows.Count == 0)
            {

                dgvCRP.DataSource = dt;

                this.dgvCRP.Columns[0].Visible = false;

                this.dgvCRP.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRP.Columns[1].Width = 150;
                this.dgvCRP.Columns[1].ReadOnly = true;

                this.dgvCRP.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRP.Columns[2].Width = 150;
                this.dgvCRP.Columns[2].ReadOnly = true;

                this.dgvCRP.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRP.Columns[3].Width = 350;
                this.dgvCRP.Columns[3].ReadOnly = true;

                this.dgvCRP.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRP.Columns[4].Width = 100;
                this.dgvCRP.Columns[4].ReadOnly = true;
                this.dgvCRP.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                this.dgvCRP.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRP.Columns[5].Width = 100;
                this.dgvCRP.Columns[5].ReadOnly = true;

                this.dgvCRP.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRP.Columns[6].Width = 130;
                this.dgvCRP.Columns[6].ReadOnly = true;

                DataGridViewButtonColumn btn = new DataGridViewButtonColumn();

                btn.Text = "Pay Cash";
                btn.HeaderText = "Pay Cash";
                btn.Name = "PayCash";
                btn.UseColumnTextForButtonValue = true;
                btn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                btn.FlatStyle = FlatStyle.Standard;
                btn.CellTemplate.Style.BackColor = Color.Honeydew;
                dgvCRP.Columns.Add(btn);

                this.dgvCRP.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dgvCRP.Columns[5].Width = 110;

                dgvCRP.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

                foreach (DataGridViewColumn c in dgvCRP.Columns)
                {
                    c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
                }
            }
            else
            {
                dgvCRP.DataSource = dt;
            }
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
                            int Id = Convert.ToInt32(thisCboCell.Value.ToString());
                            string ModifiedBy = "1";
                            bool status = ObjTransactionBAL.SaveTransactionCashRequestApproval(Id, item, ModifiedBy);
                            LoadCra();
                        }
                    }
                }
            }
        }

        private void btnpaid_Click(object sender, EventArgs e)
        {
            SaveCashPay();
        }

        public void SaveCashPay()
        {
            string mode = string.Empty;
            DataTable dtd = null;
            if (rbCash.Checked)
            {
                mode = "Cash";
               if(!string.IsNullOrEmpty(txtamountrupe.Text))
               {
                   dtd = DataGridView2DataTable(dgvDenomination, "Denomination", 0);
               }
            }
            else if(rdcheque.Checked)
            {
                mode = "Cheque";
            }
            else if (rdtransfer.Checked)
            {
                mode = "Transfer";
            }
            
            if (!string.IsNullOrEmpty(HdnCashRequestId))
            {
                TransactionBAL ObjTranBAL = new TransactionBAL();
                ObjTranBAL.CashRequestId = HdnCashRequestId;
                ObjTranBAL.Mode = mode;
                ObjTranBAL.ChequeNo = txtamount1.Text;
                ObjTranBAL.BankName = txtbankname.Text;
                ObjTranBAL.FromAccount = txtfromaccount.Text;
                ObjTranBAL.ToAccount = txttoaccount.Text;   
                ObjTranBAL.Updatedby = "1";

                int st = ObjTransactionBAL.SaveTransactionCashPayment(ObjTranBAL);
                if (st == 1)
                {
                    MessageBox.Show("Request Send Successfully");
                }
                else if (st == 0)
                {
                    MessageBox.Show("Request Already Successfully");
                }
            }
        }

        private void dgvDenomination_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column2_KeyPress);
            if (dgvDenomination.CurrentCell.ColumnIndex == 1) //Desired Column
            {
                e.Control.KeyPress += new KeyPressEventHandler(Column2_KeyPress);
            }
        }

        private void Column2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        //Converts the DataGridView to DataTable
        public DataTable DataGridView2DataTable(DataGridView dgv, String tblName, int minRow = 0)
        {

            DataTable dt = new DataTable(tblName);

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
    }
}
