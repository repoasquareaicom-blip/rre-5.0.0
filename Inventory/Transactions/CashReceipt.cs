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
            bool status = true;
            string message = "";
            int i = 0;
            if (string.IsNullOrEmpty(txtrptFrom.Text))
            {
                i++;
                message = message + "* Please Enter Receipt From" + "\n";
                if (i == 1)
                    //a1Panel2.Visible = false;


                    this.ActiveControl = txtrptFrom;

            }

            if (ddlAccountHead.SelectedIndex == 0)
            {
                i++;
                message = message + "* Please Select Account Head" + "\n";
                if (i == 1)
                    this.ActiveControl = ddlAccountHead;
            }

            if (Convert.ToDecimal(txtamount.Text) <= 0)
            {
                i++;
                message = message + "*Denomination Should Not Be Zero" + "\n";
                if (i == 1)
                    this.ActiveControl = txtamount;
                txtamount.Text = "0.00";
            }

            if (pnreturn.Visible)
            {
                if (Convert.ToDecimal(lblreturn.Text) <= 0)
                {
                    i++;
                    message = message + "*Return Denomination Should Not Be Zero" + "\n";
                    if (i == 1)
                        this.ActiveControl = lblreturn;
                    lblreturn.Text = "0.00";
                }
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message);
                status = false;
            }
            return status;
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                if (a1Panel2.Visible || pnreturn.Visible)
                {
                    a1Panel2.Visible = false;
                    pnreturn.Visible = false;
                    txtamount.Text = "0.00";
                    Btnpay.Enabled = true;
                    dgvreturn.DataSource = null;
                    LoadreturnDenomination();
                    lblreturn.Text = "0";
                    lblTotalCash.Text = "0";
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

            decimal sum = 0.00M;
            for (int i = 0; i < dgvDenomination.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dgvDenomination.Rows[i].Cells[1].Value)))
                {
                    dgvDenomination.Rows[i].Cells[1].Value = 0;
                }
                try
                {
                    dgvDenomination.Rows[i].Cells[2].Value = Convert.ToDecimal(dgvDenomination.Rows[i].Cells[0].Value) * Convert.ToDecimal(dgvDenomination.Rows[i].Cells[1].Value);
                    sum = sum + Convert.ToDecimal(dgvDenomination.Rows[i].Cells[2].Value);
                    lblTotalCash.Text = Convert.ToString(sum);
                    txtamount.Text = Convert.ToString(sum);
                }
                catch
                {

                }
            }
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
                if (dgvDenomination.CurrentCell.RowIndex == 9)
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
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            clear();
        }
        public void clear()
        {
            txtamount.Text = "";
            txtrptFrom.Text = "";
            txtdate.Text = "";
            LoadDenomination();
            rdtransfer.Checked = false;
            a1Panel2.Visible = false;
            ddlAccountHead.SelectedIndex = 0;
            Btnpay.Enabled = true;
            dgvreturn.DataSource = null;
            LoadreturnDenomination();
            lblreturn.Text = "0";
            lblTotalCash.Text = "0";
            pnreturn.Visible = false;
            LoadreturnDenomination();

        }

        private void picClose_Click(object sender, EventArgs e)
        {
            a1Panel2.Visible = false;
            LoadDenomination();
            lblTotalCash.Text = "0";
            txtamount.Text = Convert.ToString(Convert.ToDouble(lblTotalCash.Text) - Convert.ToDouble(lblreturn.Text));
            button2.PerformClick();
        }

        private void txtamount_Click(object sender, EventArgs e)
        {


            LoadDenomination();
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

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

        private void txtamount_Enter(object sender, EventArgs e)
        {
            LoadDenomination();
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

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
            if (Validation())
            {
                string res = savepayment();
                lblreturn.Text = "0";
                lblTotalCash.Text = "0";
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show("Inserted Successfully");
                    bindaccounthead();
                    clear();
                    bindcashreceipt();

                }
                else
                {
                    MessageBox.Show("Insertion Failed");
                    //clear();
                }
            }

        }

        public string savepayment()
        {
            string res = string.Empty;
            if (Validation())
            {
                TransactionBAL objTransactionBAL = new TransactionBAL();
                objTransactionBAL.RequestedBy = txtrptFrom.Text;
                DateTime date = new DateTime(txtdate.Value.Year, txtdate.Value.Month, txtdate.Value.Day);
                objTransactionBAL.CashRequestedDate = date;
                objTransactionBAL.Amount = txtamount.Text;
                objTransactionBAL.Updatedby = Program.userid;
                objTransactionBAL.AccountHead = Convert.ToString(ddlAccountHead.Text);
                //AccountHeadID = Convert.ToString(ddlAccountHead.SelectedValue);
                //objTransactionBAL.SaveAccountHead(AccountHeadID, Convert.ToString(ddlAccountHead.Text));
                string CashRptId = objTransactionBAL.SaveTransactionCashReceipt(objTransactionBAL);


                if (!string.IsNullOrEmpty(CashRptId))
                {

                    objTransactionBAL.CashRequestId = CashRptId;
                    objTransactionBAL.Mode = "Cash";
                    objTransactionBAL.Amount = lblTotalCash.Text;
                    objTransactionBAL.oamount = txtamount.Text;
                    objTransactionBAL.TwoThousand = Convert.ToString(dgvDenomination.Rows[0].Cells[1].Value.ToString());
                    objTransactionBAL.Thousand = Convert.ToString(dgvDenomination.Rows[1].Cells[1].Value.ToString());
                    objTransactionBAL.FiveHundred = Convert.ToString(dgvDenomination.Rows[2].Cells[1].Value.ToString());
                    objTransactionBAL.Hundred = Convert.ToString(dgvDenomination.Rows[3].Cells[1].Value.ToString());
                    objTransactionBAL.Fifty = Convert.ToString(dgvDenomination.Rows[4].Cells[1].Value.ToString());
                    objTransactionBAL.Twenty = Convert.ToString(dgvDenomination.Rows[5].Cells[1].Value.ToString());
                    objTransactionBAL.Ten = Convert.ToString(dgvDenomination.Rows[6].Cells[1].Value.ToString());
                    objTransactionBAL.Five = Convert.ToString(dgvDenomination.Rows[7].Cells[1].Value.ToString());
                    objTransactionBAL.Coin = Convert.ToString(dgvDenomination.Rows[8].Cells[1].Value.ToString());
                    objTransactionBAL.One = Convert.ToString(dgvDenomination.Rows[9].Cells[1].Value.ToString());
                    res = objTransactionBAL.SaveTransactionCashReceiptPayment(objTransactionBAL);


                }
                GetReport(CashRptId);
               // search("ReceiptFrom", "", "EnteredOn", "Today", role, UserId);

            }
            return res;
        }

        private void bnpaybalance_Click(object sender, EventArgs e)
        {
            if (Convert.ToDouble(lblTotalCash.Text) > Convert.ToDouble(lblreturn.Text))
            {
                if (Validation())
                {
                    string res = savepayment();
                    if (!string.IsNullOrEmpty(res))
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
            if (Convert.ToString(dgvreturn.Rows[0].Cells[1].Value) != "0")
            {
                objQuotationbal.recivetwothousand = "-" + Convert.ToString(dgvreturn.Rows[0].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivetwothousand = Convert.ToString(dgvreturn.Rows[0].Cells[1].Value);
            }
            if (Convert.ToString(dgvreturn.Rows[1].Cells[1].Value) != "0")
            {
                objQuotationbal.recivethousand = "-" + Convert.ToString(dgvreturn.Rows[1].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivethousand = Convert.ToString(dgvreturn.Rows[1].Cells[1].Value);
            }
            if (Convert.ToString(dgvreturn.Rows[2].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefivehundred = "-" + Convert.ToString(dgvreturn.Rows[2].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefivehundred = Convert.ToString(dgvreturn.Rows[2].Cells[1].Value);
            }



            if (Convert.ToString(dgvreturn.Rows[3].Cells[1].Value) != "0")
            {
                objQuotationbal.recivehundred = "-" + Convert.ToString(dgvreturn.Rows[3].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivehundred = Convert.ToString(dgvreturn.Rows[3].Cells[1].Value);
            }


            if (Convert.ToString(dgvreturn.Rows[4].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefifty = "-" + Convert.ToString(dgvreturn.Rows[4].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefifty = Convert.ToString(dgvreturn.Rows[4].Cells[1].Value);
            }


            if (Convert.ToString(dgvreturn.Rows[5].Cells[1].Value) != "0")
            {
                objQuotationbal.recivetwenty = "-" + Convert.ToString(dgvreturn.Rows[5].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivetwenty = Convert.ToString(dgvreturn.Rows[5].Cells[1].Value);
            }

            if (Convert.ToString(dgvreturn.Rows[6].Cells[1].Value) != "0")
            {
                objQuotationbal.reciveten = "-" + Convert.ToString(dgvreturn.Rows[6].Cells[1].Value);

            }
            else
            {
                objQuotationbal.reciveten = Convert.ToString(dgvreturn.Rows[6].Cells[1].Value);
            }


            if (Convert.ToString(dgvreturn.Rows[7].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefive = "-" + Convert.ToString(dgvreturn.Rows[7].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefive = Convert.ToString(dgvreturn.Rows[7].Cells[1].Value);
            }

            if (Convert.ToString(dgvreturn.Rows[8].Cells[1].Value) != "0")
            {
                objQuotationbal.recivecoin = "-" + Convert.ToString(dgvreturn.Rows[8].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivecoin = Convert.ToString(dgvreturn.Rows[8].Cells[1].Value);
            }

            if (Convert.ToString(dgvreturn.Rows[9].Cells[1].Value) != "0")
            {
                objQuotationbal.ReceiveOne = "-" + Convert.ToString(dgvreturn.Rows[9].Cells[1].Value);

            }
            else
            {
                objQuotationbal.ReceiveOne = Convert.ToString(dgvreturn.Rows[9].Cells[1].Value);
            }

            objQuotationbal.OAmount = "-" + lblreturn.Text;

            string output = objQuotationbal.SavepaymentDenomination(objQuotationbal);

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
                if (dgvreturn.CurrentCell.RowIndex == 9)
                {
                    dgvreturn.Rows[9].Cells[1].Selected = false;
                    bnpaybalance.Focus();
                }
            }
        }

        private void dgvreturn_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            decimal sum = 0.00M;
            for (int i = 0; i < dgvreturn.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dgvreturn.Rows[i].Cells[1].Value)))
                {
                    dgvreturn.Rows[i].Cells[1].Value = 0;
                }

                try
                {
                    dgvreturn.Rows[i].Cells[2].Value = Convert.ToDecimal(dgvreturn.Rows[i].Cells[0].Value) * Convert.ToDecimal(dgvreturn.Rows[i].Cells[1].Value);
                    sum = sum + Convert.ToDecimal(dgvreturn.Rows[i].Cells[2].Value);
                    //lblTotalCash.Text = Convert.ToString(sum);
                    lblreturn.Text = Convert.ToString(sum);
                }
                catch
                {

                }

                txtamount.Text = Convert.ToString(Convert.ToDouble(lblTotalCash.Text) - Convert.ToDouble(lblreturn.Text));
            }



            //decimal retunval = Convert.ToDecimal(txtamount.Text);
            ////for (int i = 0; i < dgvreturn.Rows.Count; i++)
            ////{
            ////    try
            ////    {
            //dgvreturn.Rows[e.RowIndex].Cells[2].Value = Convert.ToDecimal(dgvreturn.Rows[e.RowIndex].Cells[0].Value) * Convert.ToDecimal(dgvreturn.Rows[e.RowIndex].Cells[1].Value);
            //sum = sum + Convert.ToDecimal(dgvreturn.Rows[e.RowIndex].Cells[2].Value);

            //retunval = Convert.ToDecimal(lblTotalCash.Text) - sum;
            ////    }
            ////    catch
            ////    {

            ////    }
            ////}
            //lblreturn.Text = Convert.ToString(sum);

            ////lblTotalCash.Text = Convert.ToString(retunval);
            //txtamount.Text = Convert.ToString(retunval);
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

        }




    }
}
