using InvBal;
using Inventory.Properties;
using Inventory.Report;
using Inventory.Sales;
using Receipt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Transactions
{
    public partial class CashReceipt : Form
    {
        TransactionBAL ObjTransactionBAL = new TransactionBAL();
        private RadioButton rbUpi;
        private RadioButton rbCash;
        private TextBox txtUpiReference;
        private TextBox txtUpiAmount;
        private ComboBox ddlMainHead;
        private Label lblMainHead;
        private Label lblReason;
        private TextBox txtReason;
        private Panel paymentModeHost;
        private Label lblPaymentMode;
        private Panel upiPanel;
        private Label lblUpiAmountValue;
        private Label lblUpiDateValue;
        private Label lblCashReceivedTitle;
        private Label lblBalanceGivenTitle;
        private Label lblAmountToReceiveValue;
        private Label lblCashReceivedValue;
        private Label lblBalanceGivenValue;
        private Label lblNetCashReceivedValue;
        private Label lblReceiptDifferenceValue;
        private bool arrangingLayout;
        string clickstatus = string.Empty;
        string AccountHeadID = string.Empty;
        string role = Program.Userrole;
        string UserId = Program.userid;
        decimal sum = 0.00M;
        string BillId = string.Empty;
        string ReceiptId = string.Empty;

        QuotationBal objQuotationbal = new QuotationBal();
        public CashReceipt()
        {
            InitializeComponent();
            //if (role == "Admin")
            //{
            //    role = "Admin";
            //}
            //else
            //{
            //    role = "Emp";
            //}
            this.WindowState = FormWindowState.Maximized;
         
            LoadDenomination();
            LoadreturnDenomination();
            bindaccounthead();
            bindcashreceipt();
            GetSearch();
            this.ActiveControl = txtrptFrom;


        }
        public void bindaccounthead()
        {
            TransactionBAL ObjTransactionBAL = new TransactionBAL();
            DataTable dt = ObjTransactionBAL.GetAccountHead();
            DataRow row = dt.NewRow();
            row["AccountHead"] = "-select-";
            dt.Rows.InsertAt(row, 0);
            ddlAccountHead.DataSource = dt;
            ddlAccountHead.DisplayMember = "AccountHead";
            ddlAccountHead.ValueMember = "Id";
        }
        private void LoadDenomination()
        {
            try
            {
                DataTable dtDenom = new DataTable();
                DataTable paymentdo = new DataTable();
                paymentdo.Columns.Add("Denomination", typeof(string));
                paymentdo.Columns.Add("Count", typeof(int));
                paymentdo.Columns.Add("Amount", typeof(decimal));
                paymentdo.Rows.Add("2000", 0, 0.00);
                paymentdo.Rows.Add("1000", 0, 0.00);
                paymentdo.Rows.Add("500", 0, 0.00);
                paymentdo.Rows.Add("200", 0, 0.00);
                paymentdo.Rows.Add("100", 0, 0.00);
                paymentdo.Rows.Add("50", 0, 0.00);
                paymentdo.Rows.Add("20", 0, 0.00);
                paymentdo.Rows.Add("10", 0, 0.00);
                paymentdo.Rows.Add("5", 0, 0.00);
                paymentdo.Rows.Add("2", 0, 0.00);
                paymentdo.Rows.Add("1", 0, 0.00);

                dgvDenomination.DataSource = paymentdo;
                this.dgvDenomination.Columns[0].ReadOnly = true;


                foreach (DataGridViewColumn column in dgvDenomination.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                dgvDenomination.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
                dgvDenomination.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
                dgvDenomination.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvDenomination.DefaultCellStyle.ForeColor = Color.Black;
                dgvDenomination.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


                this.dgvDenomination.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dgvDenomination.Columns[0].Width = 150;
                this.dgvDenomination.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                this.dgvDenomination.Columns[0].ReadOnly = true;
                this.dgvDenomination.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dgvDenomination.Columns[1].Width = 100;
                this.dgvDenomination.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dgvDenomination.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dgvDenomination.Columns[2].Width = 100;
                this.dgvDenomination.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dgvDenomination.Columns[2].ReadOnly = true;
                this.dgvDenomination.Columns[3].Visible = false;
                this.dgvDenomination.Columns[4].Visible = false;
            }
            catch
            {

            }
        }



        private void LoadreturnDenomination()
        {
            try
            {
                DataTable returnpaymentdo = new DataTable();
                returnpaymentdo.Columns.Add("Denomination", typeof(string));
                returnpaymentdo.Columns.Add("Count", typeof(int));
                returnpaymentdo.Columns.Add("Amount", typeof(decimal));
                returnpaymentdo.Rows.Add("2000", 0, 0.00);
                returnpaymentdo.Rows.Add("1000", 0, 0.00);
                returnpaymentdo.Rows.Add("500", 0, 0.00);
                returnpaymentdo.Rows.Add("200", 0, 0.00);
                returnpaymentdo.Rows.Add("100", 0, 0.00);
                returnpaymentdo.Rows.Add("50", 0, 0.00);
                returnpaymentdo.Rows.Add("20", 0, 0.00);
                returnpaymentdo.Rows.Add("10", 0, 0.00);
                returnpaymentdo.Rows.Add("5", 0, 0.00);
                returnpaymentdo.Rows.Add("2", 0, 0.00);
                returnpaymentdo.Rows.Add("1", 0, 0.00);

                dgvreturn.DataSource = returnpaymentdo;
                this.dgvreturn.Columns[0].ReadOnly = true;


                foreach (DataGridViewColumn column in dgvreturn.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }




                dgvreturn.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
                dgvreturn.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
                dgvreturn.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvreturn.DefaultCellStyle.ForeColor = Color.Black;
                dgvreturn.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


                this.dgvreturn.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dgvreturn.Columns[0].Width = 150;
                this.dgvreturn.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                this.dgvreturn.Columns[0].ReadOnly = true;
                this.dgvreturn.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dgvreturn.Columns[1].Width = 100;
                this.dgvreturn.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dgvreturn.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dgvreturn.Columns[2].Width = 100;
                this.dgvreturn.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dgvreturn.Columns[2].ReadOnly = true;
                this.dgvreturn.Columns[3].Visible = false;
                this.dgvreturn.Columns[4].Visible = false;


            }
            catch
            {

            }
        }

        private bool Validation()
        {
            return ValidateReceipt();
        }

        private bool ValidateReceipt()
        {
            if (string.IsNullOrEmpty(txtrptFrom.Text.Trim()))
            {
                MessageBox.Show("Enter Received From");
                txtrptFrom.Focus();
                return false;
            }
            if (ddlMainHead == null || ddlMainHead.SelectedIndex <= 0)
            {
                MessageBox.Show("Select Account Main Header");
                if (ddlMainHead != null)
                {
                    ddlMainHead.Focus();
                }
                return false;
            }
            if (ddlAccountHead.SelectedIndex <= 0)
            {
                MessageBox.Show("Select Account Head");
                ddlAccountHead.Focus();
                return false;
            }
            if (txtdate.Value.Year < 2000)
            {
                MessageBox.Show("Enter Date");
                txtdate.Focus();
                return false;
            }
            decimal receiptAmount;
            if (decimal.TryParse(txtamount.Text.Trim(), out receiptAmount) == false || receiptAmount <= 0m)
            {
                MessageBox.Show("Enter Amount");
                txtamount.Focus();
                return false;
            }
            if (txtReason == null || string.IsNullOrEmpty(txtReason.Text.Trim()))
            {
                MessageBox.Show("Enter Reason");
                if (txtReason != null)
                {
                    txtReason.Focus();
                }
                return false;
            }
            if (txtReason.Text.Trim().Length > NarrationMaxLength)
            {
                MessageBox.Show("Reason cannot exceed " + NarrationMaxLength.ToString() + " characters.");
                txtReason.Focus();
                return false;
            }
            if (rbCash == null || rbUpi == null || (rbCash.Checked == false && rbUpi.Checked == false))
            {
                MessageBox.Show("Select Payment Mode");
                return false;
            }
            return true;
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift)) && a1Panel2.Visible && a1Panel2.ContainsFocus)
            {
                MoveFocusWithin(a1Panel2, keyData == Keys.Tab);
                return true;
            }
            if (keyData == Keys.Escape)
            {
                if (a1Panel2.Visible || pnreturn.Visible)
                {
                    CloseReceiptCashPopup();
                    return true;
                }
                else
                {
                    this.Close();
                }


            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void btnsave_Click(object sender, EventArgs e)
        {

        }
        public void bindcashreceipt()
        {
            TransactionBAL ObjTransactionBAL = new TransactionBAL();
            DataTable dtCashRequest = ObjTransactionBAL.BindCashReceipt();
            dgvCashRequest.DataSource = null;
            dgvCashRequest.Columns.Clear();
            dgvCashRequest.DataSource = dtCashRequest;
            // dgvCashRequest.Columns[0].Visible = false;

            dgvCashRequest.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10.1F, FontStyle.Bold);
            dgvCashRequest.DefaultCellStyle.Font = new Font("Tahoma", 12.1F, GraphicsUnit.Pixel);
            dgvCashRequest.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvCashRequest.DefaultCellStyle.ForeColor = Color.Black;
            dgvCashRequest.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            if (dgvCashRequest.Columns.Contains("Delete") == true)
            {

            }
            else
            {
                DataGridViewImageColumn img = new DataGridViewImageColumn();
                img.ImageLayout = DataGridViewImageCellLayout.Normal;
                object O = Resources.ResourceManager.GetObject("trash");
                Image image = (Image)O;
                img.Image = image;
                dgvCashRequest.Columns.Add(img);
                img.HeaderText = "Delete";
                img.Name = "Delete";
                img.DisplayIndex = 0;
            }
            dgvCashRequest.Columns["Delete"].Width = 50;

            dgvCashRequest.Columns["Delete"].Visible = false;

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
            if (pnlCollapse2.Visible == true)
            {
                pnlCashRequest.Visible = true;
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
            if (pnlCashRequest.Visible == true)
            {
                pnlCashRequest.Visible = false;
                vLabel2.Visible = false;
                pnlCollapse2.Visible = true;
                splitContainer1.Panel2Collapsed = false;
                pbxCollapse.Visible = true;
                pbxRightCollapse.Visible = true;
                //this.dgvSearch.Columns["CustomerID"].Visible = false;
                //this.dgvSearch.Columns["Email"].Visible = false;
                //this.dgvSearch.Columns["Fax"].Visible = false;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            GetSearch();
        }


        public void GetSearch()
        {
            try
            {
                DateTime FromDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
                DateTime ToDate = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);

                string Product = textBox1.Text.Trim();
                search(FromDate, ToDate, Product);
            }




            catch
            {
                dvgsearchcashpayment.DataSource = null;
            }
        }
        public void search(DateTime Fromdate, DateTime Todate, string Productname)
        {
            TransactionBAL obj = new TransactionBAL();
            DataTable dt = obj.searchcashReceipt(Fromdate, Todate, Productname);
            dvgsearchcashpayment.Columns.Clear();
            dvgsearchcashpayment.DataSource = dt;


            dvgsearchcashpayment.Columns["ReceiptId"].Visible = false;

            dvgsearchcashpayment.Columns["ReceiptFrom"].Width = 175;
            dvgsearchcashpayment.Columns["Date"].Width = 155;

            dvgsearchcashpayment.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dvgsearchcashpayment.DefaultCellStyle.BackColor = Color.Gainsboro;
            dvgsearchcashpayment.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dvgsearchcashpayment.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        private void dgvDenomination_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateGridAmounts(dgvDenomination);
            RefreshReceiptSummary();
        }

        private void dgvDenomination_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Tot = 0;
                Double FinVal = CalcualteTotal();
                lblTotalCash.Text = FinVal.ToString() + ".00";

            }
        }
        Double Tot = 0;
        private Double CalcualteTotal()
        {
            try
            {
                for (int i = 0; i < dgvDenomination.Rows.Count; i++)
                {
                    if (dgvDenomination.Rows[i].Cells["Denomination"].Value.ToString() != "Coins")
                    {
                        double V = Convert.ToDouble(dgvDenomination.Rows[i].Cells["Denomination"].Value.ToString()) * (string.IsNullOrEmpty(dgvDenomination.Rows[i].Cells["Count"].Value.ToString()) ? 0 : Convert.ToDouble(dgvDenomination.Rows[i].Cells["Count"].Value.ToString().Replace('$', ' ').Trim()));
                        Tot += V;
                    }
                    else
                    {
                        double V = string.IsNullOrEmpty(dgvDenomination.Rows[i].Cells["NoOfCurrency"].Value.ToString()) ? 0 : Convert.ToDouble(dgvDenomination.Rows[i].Cells["Count"].Value.ToString().Replace('$', ' ').Trim());
                        Tot += V;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return Tot;
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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dgvDenomination_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (dgvDenomination.CurrentCell.RowIndex == dgvDenomination.Rows.Count - 1)
                {
                    dgvDenomination.Rows[9].Cells[1].Selected = false;
                    lblcutomerpay.Focus();
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void rdtransfer_CheckedChanged(object sender, EventArgs e)
        {
            LoadDenomination();
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;
            if (rdtransfer.Checked)
            {
                if (w == 1024 && h == 768)
                {

                    dgvDenomination.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11F, FontStyle.Bold);
                    dgvDenomination.DefaultCellStyle.Font = new Font("Tahoma", 13F, GraphicsUnit.Pixel);
                    dgvDenomination.DefaultCellStyle.BackColor = Color.Gainsboro;
                    dgvDenomination.DefaultCellStyle.ForeColor = Color.Black;
                    dgvDenomination.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                }

                lblTotalCash.Text = "0";
                txtamount.Text = "0";
                a1Panel2.Visible = true;
                dgvDenomination.Focus();
                dgvDenomination.CurrentCell = dgvDenomination[1, 0];
            }
            else
            {
                a1Panel2.Visible = false;
                LoadDenomination();
            }
        }

    

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtamount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
                return;
            }
            if (e.KeyChar == '.' && txtamount.Text.IndexOf('.') < 0)
            {
                e.Handled = false;
                return;
            }
            e.Handled = true;
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            clear();
        }
        public void clear()
        {
            txtamount.Text = "";
            txtrptFrom.Text = "";
            txtdate.Value = DateTime.Today;
            if (txtReason != null)
            {
                txtReason.Text = "";
            }
            if (txtUpiReference != null)
            {
                txtUpiReference.Text = "";
            }
            if (rbCash != null)
            {
                rbCash.Checked = false;
            }
            if (rbUpi != null)
            {
                rbUpi.Checked = false;
            }
            if (upiPanel != null)
            {
                upiPanel.Visible = false;
            }
            rdtransfer.Checked = false;
            CloseReceiptCashPopup();
            if (ddlMainHead != null && ddlMainHead.Items.Count > 0)
            {
                ddlMainHead.SelectedIndex = 0;
            }
            BindAccountHeadPlaceholder();
            Btnpay.Enabled = true;
            lblreturn.Text = "0";
            lblTotalCash.Text = "0";
            pnreturn.Visible = false;
            LayoutReceiptForm();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            CloseReceiptCashPopup();
        }

        private void txtamount_Click(object sender, EventArgs e)
        {
        }

        private void txtamount_Enter(object sender, EventArgs e)
        {
        }

        private void btnadd_Click(object sender, EventArgs e)
        {
            txtacchead.Text = "";
            ddlAccountHead.SelectedIndex = 0;
            a1Panel1.Visible = true;
            ddlAccountHead.Enabled = false;
            this.ActiveControl = txtacchead;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            a1Panel1.Visible = false;
            ddlAccountHead.Enabled = true;
        }
        private bool ValidationAccHead()
        {
            bool status = true;
            string message = "";
            int i = 0;


            if (string.IsNullOrEmpty(txtacchead.Text.Trim()))
            {

                i++;
                message = message + "* Please Enter Account Head Name" + "\n";
                if (i == 1)
                    this.ActiveControl = txtacchead;
            }


            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message);
                status = false;
            }
            return status;
        }
        private void btnAccheadSave_Click(object sender, EventArgs e)
        {
            if (ValidationAccHead())
            {
                TransactionBAL ObjTransactionBAL = new TransactionBAL();
                int res = ObjTransactionBAL.SaveAccountHead(txtacchead.Text);
                if (res == 1)
                {
                    MessageBox.Show("Inserted Successfully");
                    bindaccounthead();
                    ddlAccountHead.Text = txtacchead.Text;
                    txtacchead.Text = "";
                    a1Panel1.Visible = false;
                    ddlAccountHead.Enabled = true;
                    this.ActiveControl = ddlAccountHead;

                }
                else
                {
                    MessageBox.Show("Already Exist");
                    txtacchead.Text = "";
                    this.ActiveControl = txtacchead;
                }
            }
        }

        private void lblcutomerpay_Click(object sender, EventArgs e)
        {
            pnreturn.Visible = true;
            Btnpay.Enabled = false;
            dgvreturn.Focus();
            dgvreturn.CurrentCell = dgvreturn[1, 0];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pnreturn.Visible = false;
            LoadreturnDenomination();
            lblreturn.Text = "0";
            Btnpay.Enabled = true;
            txtamount.Text = Convert.ToString(Convert.ToDouble(lblTotalCash.Text) - Convert.ToDouble(lblreturn.Text));
        }

        private void Btnpay_Click(object sender, EventArgs e)
        {
            if (ValidateReceipt() == false)
            {
                return;
            }
            if (rbCash.Checked)
            {
                if (a1Panel2.Visible == false)
                {
                    OpenReceiptCashPopup();
                    return;
                }
                RefreshReceiptSummary();
                if (IsReceiptReconciled() == false)
                {
                    MessageBox.Show("Cash Received minus Balance Given must equal the Amount to Receive");
                    return;
                }
            }
            else
            {
                string reference;
                if (AccountsChannel.TryUpiReference(txtUpiReference.Text, out reference) == false)
                {
                    txtUpiReference.Focus();
                    return;
                }
                txtUpiReference.Text = reference;
                CloseReceiptCashPopup();
            }

            string res = savepayment();
            if (!string.IsNullOrEmpty(res))
            {
                MessageBox.Show("Inserted Successfully");
                clear();
                bindcashreceipt();
            }
            else
            {
                MessageBox.Show("Insertion Failed");
            }
        }

        public string savepayment()
        {
            string res = string.Empty;
            decimal receiptAmount;
            if (decimal.TryParse(txtamount.Text.Trim(), out receiptAmount) == false || receiptAmount <= 0m)
            {
                return res;
            }
            string net = receiptAmount.ToString("0.00");
            TransactionBAL objTransactionBAL = new TransactionBAL();
            objTransactionBAL.RequestedBy = txtrptFrom.Text.Trim();
            objTransactionBAL.CashRequestedDate = txtdate.Value.Date;
            objTransactionBAL.Amount = net;
            objTransactionBAL.Updatedby = Program.userid;
            objTransactionBAL.AccountHead = Convert.ToString(ddlAccountHead.Text);
            string CashRptId = objTransactionBAL.SaveTransactionCashReceipt(objTransactionBAL);
            if (string.IsNullOrEmpty(CashRptId))
            {
                return res;
            }
            AccountsChannel.SaveCashReceiptReason(CashRptId, txtReason.Text.Trim());
            if (rbUpi != null && rbUpi.Checked)
            {
                string reference;
                if (AccountsChannel.TryUpiReference(txtUpiReference.Text, out reference) == false)
                {
                    return res;
                }
                res = AccountsChannel.SaveCashReceiptUpi(CashRptId, receiptAmount, reference);
            }
            else
            {
                objTransactionBAL.CashRequestId = CashRptId;
                objTransactionBAL.Mode = "Cash";
                objTransactionBAL.Amount = net;
                objTransactionBAL.oamount = net;
                objTransactionBAL.TwoThousand = SignedReceiptCount(2000);
                objTransactionBAL.Thousand = SignedReceiptCount(1000);
                objTransactionBAL.FiveHundred = SignedReceiptCount(500);
                objTransactionBAL.Hundred = SignedReceiptCount(100);
                objTransactionBAL.Fifty = SignedReceiptCount(50);
                objTransactionBAL.Twenty = SignedReceiptCount(20);
                objTransactionBAL.Ten = SignedReceiptCount(10);
                objTransactionBAL.Five = SignedReceiptCount(5);
                objTransactionBAL.Coin = SignedReceiptCount(2);
                objTransactionBAL.One = SignedReceiptCount(1);
                res = objTransactionBAL.SaveTransactionCashReceiptPayment(objTransactionBAL);
                if (string.IsNullOrEmpty(res) == false)
                {
                    AccountsChannel.SaveTwoHundred(res, SignedReceiptCount(200), null);
                }
            }
            if (string.IsNullOrEmpty(res) == false)
            {
                GetReport(CashRptId);
            }
            return res;
        }

        private void bnpaybalance_Click(object sender, EventArgs e)
        {
            return;
            if (Convert.ToDouble(lblTotalCash.Text) > Convert.ToDouble(lblreturn.Text))
            {
                if (Validation())
                {
                    string res = savepayment();
                    if (!string.IsNullOrEmpty(res) && (rbUpi == null || !rbUpi.Checked))
                    {
                        SavepaymentDenomination(res);
                    }
                    //bnpaybalance.Visible = false;
                    dgvreturn.DataSource = null;
                    LoadreturnDenomination();
                    lblreturn.Text = "0";
                    lblTotalCash.Text = "0";
                    pnreturn.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("Please Enter Correct Denomination");
            }



        }


        public void SavepaymentDenomination(string s)
        {

            // Pnloading4.Visible = true;
            objQuotationbal.recivetransid = s;
            if (AccountsChannel.NoteCount(dgvreturn, 2000) != "0")
            {
                objQuotationbal.recivetwothousand = "-" + AccountsChannel.NoteCount(dgvreturn, 2000);

            }
            else
            {
                objQuotationbal.recivetwothousand = AccountsChannel.NoteCount(dgvreturn, 2000);
            }
            if (AccountsChannel.NoteCount(dgvreturn, 1000) != "0")
            {
                objQuotationbal.recivethousand = "-" + AccountsChannel.NoteCount(dgvreturn, 1000);

            }
            else
            {
                objQuotationbal.recivethousand = AccountsChannel.NoteCount(dgvreturn, 1000);
            }
            if (AccountsChannel.NoteCount(dgvreturn, 500) != "0")
            {
                objQuotationbal.recivefivehundred = "-" + AccountsChannel.NoteCount(dgvreturn, 500);

            }
            else
            {
                objQuotationbal.recivefivehundred = AccountsChannel.NoteCount(dgvreturn, 500);
            }



            if (AccountsChannel.NoteCount(dgvreturn, 100) != "0")
            {
                objQuotationbal.recivehundred = "-" + AccountsChannel.NoteCount(dgvreturn, 100);

            }
            else
            {
                objQuotationbal.recivehundred = AccountsChannel.NoteCount(dgvreturn, 100);
            }


            if (AccountsChannel.NoteCount(dgvreturn, 50) != "0")
            {
                objQuotationbal.recivefifty = "-" + AccountsChannel.NoteCount(dgvreturn, 50);

            }
            else
            {
                objQuotationbal.recivefifty = AccountsChannel.NoteCount(dgvreturn, 50);
            }


            if (AccountsChannel.NoteCount(dgvreturn, 20) != "0")
            {
                objQuotationbal.recivetwenty = "-" + AccountsChannel.NoteCount(dgvreturn, 20);

            }
            else
            {
                objQuotationbal.recivetwenty = AccountsChannel.NoteCount(dgvreturn, 20);
            }

            if (AccountsChannel.NoteCount(dgvreturn, 10) != "0")
            {
                objQuotationbal.reciveten = "-" + AccountsChannel.NoteCount(dgvreturn, 10);

            }
            else
            {
                objQuotationbal.reciveten = AccountsChannel.NoteCount(dgvreturn, 10);
            }


            if (AccountsChannel.NoteCount(dgvreturn, 5) != "0")
            {
                objQuotationbal.recivefive = "-" + AccountsChannel.NoteCount(dgvreturn, 5);

            }
            else
            {
                objQuotationbal.recivefive = AccountsChannel.NoteCount(dgvreturn, 5);
            }

            if (AccountsChannel.NoteCount(dgvreturn, 2) != "0")
            {
                objQuotationbal.recivecoin = "-" + AccountsChannel.NoteCount(dgvreturn, 2);

            }
            else
            {
                objQuotationbal.recivecoin = AccountsChannel.NoteCount(dgvreturn, 2);
            }

            if (AccountsChannel.NoteCount(dgvreturn, 1) != "0")
            {
                objQuotationbal.ReceiveOne = "-" + AccountsChannel.NoteCount(dgvreturn, 1);

            }
            else
            {
                objQuotationbal.ReceiveOne = AccountsChannel.NoteCount(dgvreturn, 1);
            }

            objQuotationbal.OAmount = "-" + lblreturn.Text;

            string output = objQuotationbal.SavepaymentDenomination(objQuotationbal);
            if (output == "1")
            {
                AccountsChannel.SaveTwoHundred(s, null, AccountsChannel.SignedChangeCount(dgvreturn, 200));
            }

            if (output == "1")
            {
                //Pnloading4.Visible = false;

                MessageBox.Show("Inserted Successfully");
                bindcashreceipt();
                bindaccounthead();
                clear();
            }
            else
            {
                MessageBox.Show("Insertion Failed");
                clear();
            }


        }

        private void dgvreturn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (dgvreturn.CurrentCell.RowIndex == dgvreturn.Rows.Count - 1)
                {
                    dgvreturn.Rows[9].Cells[1].Selected = false;
                    bnpaybalance.Focus();
                }
            }
        }

        private void dgvreturn_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateGridAmounts(dgvreturn);
            RefreshReceiptSummary();
        }


        public void GetReport(string QuotationId)
        {
            //try
            //{
            if (!string.IsNullOrEmpty(QuotationId))
            {
                DialogResult result = MessageBox.Show("Do you want to Print Receipt " + QuotationId + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Quotationreport rpt = new Quotationreport(txtorder.Text);
                    //rpt.ShowDialog();

                    using (SqlConnection con = new SqlConnection(Program.connection))
                    {
                        DataSet ds = new DataSet();
                        con.Open();


                        SqlCommand cmd = new SqlCommand("proc_GetCashReceivedReceiptReport_Direct", con);
                        cmd.Parameters.AddWithValue("@ReceiptId", QuotationId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);





                        ReciptDal Obj = new ReciptDal();
                        Obj.dsMain = ds;
                        if (Obj.GenerateQuoation())
                        {
                        }



                    }
                }
            }


            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message.ToString());
            //}
        }
        public void GetReports(string QuotationId)
        {
            //try
            //{
            if (!string.IsNullOrEmpty(QuotationId))
            {
                DialogResult result = MessageBox.Show("Do you want to Print " + QuotationId + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Quotationreport rpt = new Quotationreport(txtorder.Text);
                    //rpt.ShowDialog();

                    using (SqlConnection con = new SqlConnection(Program.connection))
                    {
                        DataSet ds = new DataSet();
                        con.Open();


                        SqlCommand cmd = new SqlCommand("proc_GetCashReceivedReceiptReport_Direct", con);
                        cmd.Parameters.AddWithValue("@ReceiptId", QuotationId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);
                        ReciptDal Obj = new ReciptDal();
                        Obj.dsMain = ds;
                        if (Obj.GenerateQuoation())
                        {
                        }



                    }
                }
            }


            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message.ToString());
            //}
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


        private void btnPrintReceipt_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ReceiptId))
            {
                GetReports(ReceiptId);

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

        private void dvgsearchcashpayment_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                ReceiptId = Convert.ToString(dvgsearchcashpayment.Rows[e.RowIndex].Cells["ReceiptId"].Value);
            }
        }

        private void dgvCashRequest_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    pnlless.Visible = false;
                    txtlesspwd.Text = string.Empty;

                    if (dgvCashRequest.Columns[e.ColumnIndex].HeaderText == "Delete")
                    {
                        DialogResult result = MessageBox.Show("Do you want to Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            txtlesspwd.Clear();
                            pnlless.Visible = true;
                            txtlesspwd.Focus();
                        }
                    }
                }
                BillId = dgvCashRequest.Rows[e.RowIndex].Cells["Receiptid"].Value.ToString();
                if (dgvCashRequest.Columns[e.ColumnIndex].HeaderText == "Receiptid")
                {
                    if (!string.IsNullOrEmpty(BillId))
                    {
                        GetReport(BillId);


                    }
                    else
                    {
                        MessageBox.Show("Receipt not available");
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            pnlless.Visible = false;
            txtlesspwd.Clear();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtlesspwd.Text))
            {
                MessageBox.Show("Password should not be empty");
                this.ActiveControl = txtlesspwd;
                return;
            }
            DataTable dt = LoginBAL.GetLesserDetials(txtlesspwd.Text, "CASHRECEIPT");
            if (dt.Rows.Count > 0)
            {
                txtlesspwd.Text = string.Empty;
                pnlless.Visible = false;
                string sa = Convert.ToString(dgvCashRequest.Rows[dgvCashRequest.CurrentCell.RowIndex].Cells["Receiptid"].Value);
                int v = ObjTransactionBAL.deletecashreceipt(sa);
                if (v == 1)
                {
                    MessageBox.Show("Deleted Successfully");
                    bindcashreceipt();
                 
                }
                else
                {
                    MessageBox.Show("Can't Delete");
                }



            }
            else
            {
                MessageBox.Show("Authentication Failed");
                txtlesspwd.Focus();

            }
        }

        private void txtlesspwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void panel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CashReceipt_Load(object sender, EventArgs e)
        {
            ArrangeCashReceiptScreen();
        }

        private void rbUpi_CheckedChanged(object sender, EventArgs e)
        {
            if (rbUpi.Checked == false)
            {
                return;
            }
            CloseReceiptCashPopup();
            if (upiPanel != null)
            {
                upiPanel.Visible = true;
            }
            ShowUpiDetails();
            LayoutReceiptForm();
        }

        private void ArrangeCashReceiptScreen()
        {
            Font labelFont = new Font("Calibri", 11F, FontStyle.Bold);
            Font inputFont = new Font("Calibri", 11F);
            label2.Text = "CASH RECEIPT";
            label2.RightToLeft = RightToLeft.No;
            label2.TextAlign = ContentAlignment.MiddleLeft;
            label2.Font = new Font("Calibri", 16F, FontStyle.Bold);
            label2.ForeColor = Color.SteelBlue;
            label6.Text = "Received From";
            label12.Text = "Account Head";
            label7.Text = "Date";
            label9.Text = "Amount";
            label6.Font = labelFont;
            label12.Font = labelFont;
            label7.Font = labelFont;
            label9.Font = labelFont;
            label11.Visible = false;
            label4.Visible = false;
            label8.Visible = false;
            txtamount.ReadOnly = false;
            txtamount.Text = "";
            txtamount.Font = inputFont;
            txtrptFrom.Font = inputFont;
            txtdate.Font = inputFont;
            ddlAccountHead.Font = inputFont;
            rdtransfer.Visible = false;
            lblcutomerpay.Visible = false;
            bnpaybalance.Visible = false;
            pnreturn.Visible = false;
            if (txtUpiAmount != null)
            {
                txtUpiAmount.Visible = false;
            }

            lblMainHead = new Label();
            lblMainHead.Text = "Account Main Header";
            lblMainHead.Font = labelFont;
            lblMainHead.AutoSize = false;
            panel2.Controls.Add(lblMainHead);

            ddlMainHead = new ComboBox();
            ddlMainHead.Name = "ddlMainHead";
            ddlMainHead.DropDownStyle = ComboBoxStyle.DropDownList;
            ddlMainHead.Font = inputFont;
            ddlMainHead.FlatStyle = FlatStyle.Popup;
            ddlMainHead.SelectedIndexChanged += new EventHandler(ddlMainHead_SelectedIndexChanged);
            panel2.Controls.Add(ddlMainHead);

            lblReason = new Label();
            lblReason.Text = "Reason / Narration";
            lblReason.Font = labelFont;
            lblReason.AutoSize = false;
            panel2.Controls.Add(lblReason);

            txtReason = new TextBox();
            txtReason.Name = "txtReason";
            txtReason.Font = inputFont;
            txtReason.Multiline = true;
            txtReason.MaxLength = NarrationMaxLength;
            txtReason.AcceptsTab = false;
            txtReason.ScrollBars = ScrollBars.Vertical;
            panel2.Controls.Add(txtReason);

            lblPaymentMode = new Label();
            lblPaymentMode.Text = "Payment Mode";
            lblPaymentMode.Font = labelFont;
            lblPaymentMode.AutoSize = false;
            panel2.Controls.Add(lblPaymentMode);

            paymentModeHost = new Panel();
            paymentModeHost.Name = "paymentModeHost";
            panel2.Controls.Add(paymentModeHost);

            rbCash = new RadioButton();
            rbCash.Name = "rbCash";
            rbCash.Text = "Cash";
            rbCash.Font = labelFont;
            rbCash.AutoSize = true;
            rbCash.Location = new Point(8, 4);
            rbCash.CheckedChanged += new EventHandler(rbCash_CheckedChanged);
            paymentModeHost.Controls.Add(rbCash);

            rbUpi = new RadioButton();
            rbUpi.Name = "rbUpi";
            rbUpi.Text = "UPI";
            rbUpi.Font = labelFont;
            rbUpi.AutoSize = true;
            rbUpi.Location = new Point(110, 4);
            rbUpi.CheckedChanged += new EventHandler(rbUpi_CheckedChanged);
            paymentModeHost.Controls.Add(rbUpi);

            upiPanel = new Panel();
            upiPanel.Name = "upiPanel";
            upiPanel.Visible = false;
            panel2.Controls.Add(upiPanel);

            Label amountCaption = new Label();
            amountCaption.Text = "Amount to Receive";
            amountCaption.Font = labelFont;
            amountCaption.AutoSize = true;
            amountCaption.Location = new Point(4, 8);
            upiPanel.Controls.Add(amountCaption);

            lblUpiAmountValue = new Label();
            lblUpiAmountValue.Font = labelFont;
            lblUpiAmountValue.AutoSize = true;
            lblUpiAmountValue.Location = new Point(180, 8);
            upiPanel.Controls.Add(lblUpiAmountValue);

            Label referenceCaption = new Label();
            referenceCaption.Text = "UPI Reference";
            referenceCaption.Font = labelFont;
            referenceCaption.AutoSize = true;
            referenceCaption.Location = new Point(4, 40);
            upiPanel.Controls.Add(referenceCaption);

            txtUpiReference = new TextBox();
            txtUpiReference.Font = inputFont;
            txtUpiReference.MaxLength = 30;
            txtUpiReference.Location = new Point(180, 36);
            txtUpiReference.Width = 220;
            upiPanel.Controls.Add(txtUpiReference);

            Label dateCaption = new Label();
            dateCaption.Text = "Transaction Date";
            dateCaption.Font = labelFont;
            dateCaption.AutoSize = true;
            dateCaption.Location = new Point(4, 74);
            upiPanel.Controls.Add(dateCaption);

            lblUpiDateValue = new Label();
            lblUpiDateValue.Font = labelFont;
            lblUpiDateValue.AutoSize = true;
            lblUpiDateValue.Location = new Point(180, 74);
            upiPanel.Controls.Add(lblUpiDateValue);

            Btnpay.Text = "Save Receipt";
            Btnpay.Font = new Font("Calibri", 12F, FontStyle.Bold);
            Btnpay.Size = new Size(160, 34);
            Btnpay.Parent = panel2;
            btnclear.Text = "Clear";
            btnclear.Font = new Font("Calibri", 12F, FontStyle.Bold);
            btnclear.Size = new Size(110, 34);
            btnclear.Parent = panel2;
            btnclear.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            BindMainHead();
            BindAccountHeadPlaceholder();
            txtdate.ValueChanged += new EventHandler(txtdate_ValueChanged);
            panel2.Resize += new EventHandler(panel2_Resize);
            ApplyReceiptTabOrder();
            LayoutReceiptForm();
        }

        private const int NarrationMaxLength = 250;

        private void ApplyReceiptTabOrder()
        {
            txtrptFrom.TabIndex = 0;
            ddlMainHead.TabIndex = 1;
            ddlAccountHead.TabIndex = 2;
            txtdate.TabIndex = 3;
            txtamount.TabIndex = 4;
            txtReason.TabIndex = 5;
            paymentModeHost.TabIndex = 6;
            rbCash.TabIndex = 0;
            rbUpi.TabIndex = 1;
            upiPanel.TabIndex = 7;
            txtUpiReference.TabIndex = 0;
            Btnpay.TabIndex = 8;
            btnclear.TabIndex = 9;

            btnadd.TabStop = false;
            rdtransfer.TabStop = false;
            dgvCashRequest.TabStop = false;

            panel15.TabIndex = 0;
            dgvDenomination.TabIndex = 0;
            dgvDenomination.StandardTab = true;
            dgvreturn.TabIndex = 1;
            dgvreturn.StandardTab = true;
        }

        private void EnsurePaymentModeTabStops()
        {
            if (rbCash == null || rbUpi == null || rbCash.Checked || rbUpi.Checked)
            {
                return;
            }
            rbCash.TabStop = true;
            rbUpi.TabStop = true;
        }

        private void MoveFocusWithin(Control container, bool forward)
        {
            Control current = ActiveControl;
            while (current is ContainerControl && ((ContainerControl)current).ActiveControl != null)
            {
                current = ((ContainerControl)current).ActiveControl;
            }
            if (current != null && current.Parent is DataGridView)
            {
                current = current.Parent;
            }
            container.SelectNextControl(current, forward, true, true, true);
        }

        private void rbCash_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCash.Checked == false)
            {
                return;
            }
            if (txtUpiReference != null)
            {
                txtUpiReference.Text = "";
            }
            if (upiPanel != null)
            {
                upiPanel.Visible = false;
            }
            LayoutReceiptForm();
        }

        private void txtdate_ValueChanged(object sender, EventArgs e)
        {
            ShowUpiDetails();
        }

        private void ShowUpiDetails()
        {
            decimal receiptAmount;
            decimal.TryParse(txtamount.Text.Trim(), out receiptAmount);
            if (lblUpiAmountValue != null)
            {
                lblUpiAmountValue.Text = receiptAmount.ToString("0.00");
            }
            if (lblUpiDateValue != null)
            {
                lblUpiDateValue.Text = txtdate.Value.ToString("dd-MM-yyyy");
            }
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
            if (arrangingLayout)
            {
                return;
            }
            LayoutReceiptForm();
            if (a1Panel2.Visible)
            {
                LayoutReceiptCashPopup();
            }
        }

        private void LayoutReceiptForm()
        {
            if (lblMainHead == null)
            {
                return;
            }
            arrangingLayout = true;
            int labelX = 16;
            int labelW = 180;
            int boxX = 206;
            int boxW = 360;
            if (panel2.ClientSize.Width - boxX - 48 < boxW)
            {
                boxW = panel2.ClientSize.Width - boxX - 48;
            }
            if (boxW < 220)
            {
                boxW = 220;
            }
            int y = 8;
            label2.SetBounds(labelX, y, 280, 30);
            y = 46;
            PlaceField(label6, txtrptFrom, labelX, labelW, boxX, boxW, y, 28);
            y = y + 40;
            PlaceField(lblMainHead, ddlMainHead, labelX, labelW, boxX, boxW, y, 28);
            y = y + 40;
            PlaceField(label12, ddlAccountHead, labelX, labelW, boxX, boxW - 36, y, 28);
            btnadd.SetBounds(boxX + boxW - 28, y, 28, 28);
            btnadd.BringToFront();
            y = y + 40;
            PlaceField(label7, txtdate, labelX, labelW, boxX, 180, y, 28);
            y = y + 40;
            PlaceField(label9, txtamount, labelX, labelW, boxX, 180, y, 28);
            y = y + 40;
            lblReason.SetBounds(labelX, y, labelW, 24);
            txtReason.SetBounds(boxX, y, boxW, 58);
            y = y + 70;
            lblPaymentMode.SetBounds(labelX, y, labelW, 28);
            paymentModeHost.SetBounds(boxX, y, 260, 32);
            y = y + 40;
            bool showUpi = rbUpi != null && rbUpi.Checked;
            upiPanel.Visible = showUpi;
            if (showUpi)
            {
                upiPanel.SetBounds(boxX, y, boxW, 108);
                ShowUpiDetails();
                y = y + 116;
            }
            if (Btnpay.Parent == panel2)
            {
                Btnpay.SetBounds(boxX, y, 160, 34);
                btnclear.SetBounds(boxX + 172, y, 110, 34);
                Btnpay.BringToFront();
                btnclear.BringToFront();
            }
            else
            {
                btnclear.SetBounds(boxX, y, 110, 34);
            }
            y = y + 46;
            dgvCashRequest.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            int gridHeight = panel2.ClientSize.Height - y - 8;
            if (gridHeight < 120)
            {
                gridHeight = 120;
            }
            dgvCashRequest.SetBounds(12, y, panel2.ClientSize.Width - 24, gridHeight);
            EnsurePaymentModeTabStops();
            arrangingLayout = false;
        }

        private void PlaceField(Label label, Control box, int labelX, int labelW, int boxX, int boxW, int y, int height)
        {
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.SetBounds(labelX, y, labelW, height);
            box.SetBounds(boxX, y, boxW, height);
        }

        private void BindMainHead()
        {
            TransactionBAL headBal = new TransactionBAL();
            DataTable dh = headBal.GetAccountMainHead();
            DataRow row = dh.NewRow();
            row["MeanHeadName"] = "-select-";
            dh.Rows.InsertAt(row, 0);
            ddlMainHead.DataSource = dh;
            ddlMainHead.DisplayMember = "MeanHeadName";
            ddlMainHead.ValueMember = "MainHeadId";
        }

        private void BindAccountHeadPlaceholder()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("AccountHead", typeof(string));
            DataRow row = dt.NewRow();
            row["Id"] = 0;
            row["AccountHead"] = "-select-";
            dt.Rows.Add(row);
            ddlAccountHead.DataSource = dt;
            ddlAccountHead.DisplayMember = "AccountHead";
            ddlAccountHead.ValueMember = "Id";
        }

        private void ddlMainHead_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (arrangingLayout)
            {
                return;
            }
            if (ddlMainHead.SelectedIndex > 0)
            {
                DataTable dt = PurchaseReturnBAL.GetSuppliersDetails(Convert.ToInt32(ddlMainHead.SelectedValue));
                DataRow row = dt.NewRow();
                row["AccountHead"] = "-select-";
                dt.Rows.InsertAt(row, 0);
                ddlAccountHead.DataSource = dt;
                ddlAccountHead.DisplayMember = "AccountHead";
                ddlAccountHead.ValueMember = "Id";
            }
            else
            {
                BindAccountHeadPlaceholder();
            }
        }

        private void OpenReceiptCashPopup()
        {
            EnsureReceiptCashPopup();
            LoadDenomination();
            LoadreturnDenomination();
            if (dgvreturn.Parent != panel15)
            {
                panel15.Controls.Add(dgvreturn);
            }
            pnreturn.Visible = false;
            a1Panel2.Visible = true;
            a1Panel2.BringToFront();
            LayoutReceiptCashPopup();
            RefreshReceiptSummary();
            dgvDenomination.Focus();
            if (dgvDenomination.Rows.Count > 0)
            {
                dgvDenomination.CurrentCell = dgvDenomination[1, 0];
            }
        }

        private void CloseReceiptCashPopup()
        {
            a1Panel2.Visible = false;
            pnreturn.Visible = false;
            if (Btnpay.Parent != panel2)
            {
                Btnpay.Parent = panel2;
            }
            LayoutReceiptForm();
        }

        private void EnsureReceiptCashPopup()
        {
            if (lblAmountToReceiveValue != null)
            {
                return;
            }
            label143.Text = "Cash Denomination";
            lblCashReceivedTitle = new Label();
            lblCashReceivedTitle.Text = "CASH RECEIVED";
            lblCashReceivedTitle.Font = new Font("Calibri", 12F, FontStyle.Bold);
            lblCashReceivedTitle.AutoSize = true;
            panel15.Controls.Add(lblCashReceivedTitle);

            lblBalanceGivenTitle = new Label();
            lblBalanceGivenTitle.Text = "BALANCE GIVEN";
            lblBalanceGivenTitle.Font = new Font("Calibri", 12F, FontStyle.Bold);
            lblBalanceGivenTitle.AutoSize = true;
            panel15.Controls.Add(lblBalanceGivenTitle);

            lblAmountToReceiveValue = AddReceiptSummary("Amount to Receive : 0.00");
            lblCashReceivedValue = AddReceiptSummary("Cash Received : 0.00");
            lblBalanceGivenValue = AddReceiptSummary("Balance Given : 0.00");
            lblNetCashReceivedValue = AddReceiptSummary("Net Cash Received : 0.00");
            lblReceiptDifferenceValue = AddReceiptSummary("Difference : 0.00");
            lblTotalCash.Visible = false;
            label3.Visible = false;
            lblcutomerpay.Visible = false;
        }

        private Label AddReceiptSummary(string text)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Font = new Font("Calibri", 13F, FontStyle.Bold);
            label.Text = text;
            panel15.Controls.Add(label);
            return label;
        }

        private void LayoutReceiptCashPopup()
        {
            if (lblAmountToReceiveValue == null)
            {
                return;
            }
            arrangingLayout = true;
            int width = panel2.ClientSize.Width - 16;
            int height = panel2.ClientSize.Height - 16;
            if (width < 760)
            {
                width = 760;
            }
            if (height < 480)
            {
                height = 480;
            }
            a1Panel2.SetBounds(8, 8, width, height);
            a1Panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel15.SetBounds(4, 32, a1Panel2.Width - 12, a1Panel2.Height - 84);
            panel15.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            int half = (panel15.ClientSize.Width - 24) / 2;
            if (half < 240)
            {
                half = 240;
            }
            int gridHeight = panel15.ClientSize.Height - 150;
            if (gridHeight < 220)
            {
                gridHeight = 220;
            }
            lblCashReceivedTitle.Location = new Point(8, 4);
            lblBalanceGivenTitle.Location = new Point(half + 16, 4);
            dgvDenomination.SetBounds(8, 28, half, gridHeight);
            dgvreturn.SetBounds(half + 16, 28, half, gridHeight);
            dgvDenomination.ScrollBars = ScrollBars.Both;
            dgvreturn.ScrollBars = ScrollBars.Both;
            int summaryTop = dgvDenomination.Bottom + 8;
            lblAmountToReceiveValue.Location = new Point(8, summaryTop);
            lblCashReceivedValue.Location = new Point(8, summaryTop + 26);
            lblBalanceGivenValue.Location = new Point(half + 16, summaryTop);
            lblNetCashReceivedValue.Location = new Point(half + 16, summaryTop + 26);
            lblReceiptDifferenceValue.Location = new Point(8, summaryTop + 52);
            picClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            picClose.Location = new Point(a1Panel2.Width - 36, 2);
            Btnpay.Parent = a1Panel2;
            Btnpay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Btnpay.Size = new Size(160, 34);
            Btnpay.Location = new Point(a1Panel2.Width - 180, a1Panel2.Height - 46);
            Btnpay.Visible = true;
            Btnpay.BringToFront();
            arrangingLayout = false;
        }

        private void UpdateGridAmounts(DataGridView grid)
        {
            if (grid == null)
            {
                return;
            }
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(Convert.ToString(grid.Rows[i].Cells[1].Value)))
                {
                    grid.Rows[i].Cells[1].Value = 0;
                }
                decimal face;
                decimal count;
                decimal.TryParse(Convert.ToString(grid.Rows[i].Cells[0].Value), out face);
                decimal.TryParse(Convert.ToString(grid.Rows[i].Cells[1].Value), out count);
                if (count < 0m)
                {
                    count = 0m;
                    grid.Rows[i].Cells[1].Value = 0;
                }
                grid.Rows[i].Cells[2].Value = face * count;
            }
        }

        private decimal SumReceiptNotes(DataGridView grid)
        {
            decimal sum = 0m;
            if (grid == null)
            {
                return sum;
            }
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                decimal face;
                decimal count;
                decimal.TryParse(Convert.ToString(grid.Rows[i].Cells[0].Value), out face);
                decimal.TryParse(Convert.ToString(grid.Rows[i].Cells[1].Value), out count);
                if (count < 0m)
                {
                    count = 0m;
                }
                sum = sum + (face * count);
            }
            return sum;
        }

        private decimal ReceiptAmount()
        {
            decimal receiptAmount;
            decimal.TryParse(txtamount.Text.Trim(), out receiptAmount);
            return receiptAmount;
        }

        private void RefreshReceiptSummary()
        {
            if (lblAmountToReceiveValue == null)
            {
                return;
            }
            decimal receiptAmount = ReceiptAmount();
            decimal received = SumReceiptNotes(dgvDenomination);
            decimal given = SumReceiptNotes(dgvreturn);
            decimal net = received - given;
            decimal difference = net - receiptAmount;
            lblAmountToReceiveValue.Text = "Amount to Receive : " + receiptAmount.ToString("0.00");
            lblCashReceivedValue.Text = "Cash Received : " + received.ToString("0.00");
            lblBalanceGivenValue.Text = "Balance Given : " + given.ToString("0.00");
            lblNetCashReceivedValue.Text = "Net Cash Received : " + net.ToString("0.00");
            lblReceiptDifferenceValue.Text = "Difference : " + difference.ToString("0.00");
            lblTotalCash.Text = received.ToString("0.00");
            lblreturn.Text = given.ToString("0.00");
        }

        private bool IsReceiptReconciled()
        {
            decimal receiptAmount = ReceiptAmount();
            if (receiptAmount <= 0m)
            {
                return false;
            }
            decimal net = SumReceiptNotes(dgvDenomination) - SumReceiptNotes(dgvreturn);
            return net == receiptAmount;
        }

        private int ReceiptNoteQuantity(DataGridView grid, int face)
        {
            int count;
            int.TryParse(AccountsChannel.NoteCount(grid, face), out count);
            if (count < 0)
            {
                count = 0;
            }
            return count;
        }

        private string SignedReceiptCount(int face)
        {
            int received = ReceiptNoteQuantity(dgvDenomination, face);
            int given = ReceiptNoteQuantity(dgvreturn, face);
            return (received - given).ToString();
        }




    }
}
