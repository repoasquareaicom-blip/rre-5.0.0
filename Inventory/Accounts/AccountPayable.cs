using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace Inventory.Accounts
{
    public partial class AccountPayable : Form
    {
        QuotationBal objQuotationbal = new QuotationBal();
         string[] transdate = new string[50];
        public AccountPayable()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            //dgvPayableBalance.Focus();
            GetAccountPayableBalance();
            lblTotalBillSelected.Visible = false;
            label17.Visible = false;
        }


        public DataTable paymentDeno()
        {
            DataTable paymentdo = new DataTable();
            paymentdo.Columns.Add("Denomination", typeof(string));
            paymentdo.Columns.Add("Count", typeof(int));
            paymentdo.Columns.Add("Amount", typeof(decimal));
            paymentdo.Rows.Add("2000", 0, 0.0);
            paymentdo.Rows.Add("1000", 0, 0.0);
            paymentdo.Rows.Add("500", 0, 0.0);
            paymentdo.Rows.Add("100", 0, 0.0);
            paymentdo.Rows.Add("50", 0, 0.0);
            paymentdo.Rows.Add("20", 0, 0.0);
            paymentdo.Rows.Add("10", 0, 0.0);
            paymentdo.Rows.Add("5", 0, 0.0);
            paymentdo.Rows.Add("2", 0, 0.0);
            paymentdo.Rows.Add("1", 0, 0.0);

            return paymentdo;
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
            this.cashddl.Columns[0].Width = 100;
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

            cashddl.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
            cashddl.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
            cashddl.DefaultCellStyle.BackColor = Color.Gainsboro;
            cashddl.DefaultCellStyle.ForeColor = Color.Black;
            cashddl.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

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


            foreach (DataGridViewColumn column in cashddl.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }


        public void BindBankAccount()
        {
            DataTable dt = AccountReceivableBAL.GetBankAccount();
            ddlBankAccount.DataSource = dt;
            ddlBankAccount.ValueMember = "accountno";
            ddlBankAccount.DisplayMember = "accountno";
        }

        public void BindBankAccountTo()
        {
            DataTable dt = AccountReceivableBAL.GetBankAccount();
            ddlBankAccountTo.DataSource = dt;
            ddlBankAccountTo.ValueMember = "accountno";
            ddlBankAccountTo.DisplayMember = "accountno";
        }
        public void GetAccountPayableBalanceSearch()
        {

            DataTable dt = AccountPayableBAL.GetAccountPayableBalanceSearch(txtprodsearch.Text);
            //dgvReceivableBalance.DataSource = dt;
            dgvPayableBalance.Columns.Clear();
            dgvPayableBalance.DataSource = dt;
            //dgvPayableBalance.Columns["VendorId"].Visible = false;
            //dgvPayableBalance.Columns["OrderNumber"].Visible = false;
            //dgvPayableBalance.Columns["InvoiceDate"].HeaderText = "Invoice Date";
            //dgvPayableBalance.Columns["InvoiceNo"].HeaderText = "Invoice No";
            //dgvPayableBalance.Columns["PurchaseId"].HeaderText = "Purchase Id";
            dgvPayableBalance.Columns["NetAmount"].HeaderText = "Total_Bill_Amount";
            dgvPayableBalance.Columns["VendorId"].HeaderText = "Vendor Id";
            dgvPayableBalance.Columns["VendorName"].HeaderText = "Vendor Name";
            dgvPayableBalance.Columns["Total"].HeaderText = "NO_Of_Bills";
            //dgvPayableBalance.Columns["InvoiceDate"].Width = 120;
            //dgvPayableBalance.Columns["InvoiceNo"].Width = 100;
            //dgvPayableBalance.Columns["PurchaseId"].Width = 120;

            //dgvPayableBalance.Columns["VendorId"].Width = 130;
            //dgvPayableBalance.Columns["VendorName"].Width = 150;
            //dgvPayableBalance.Columns["NetAmount"].Width = 180;            
            //dgvPayableBalance.Columns["Balance"].Width = 200;
            //dgvPayableBalance.Columns["Total"].Width = 200;
            //dgvPayableBalance.Columns["Paid"].Width = 200;


            //dgvPayableBalance.Columns["ReorderPoint"].Width = 150;

            //dgvPayableBalance.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvPayableBalance.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvPayableBalance.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvPayableBalance.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10.1F, FontStyle.Bold);
            dgvPayableBalance.DefaultCellStyle.Font = new Font("Tahoma", 12.1F, GraphicsUnit.Pixel);
            dgvPayableBalance.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvPayableBalance.DefaultCellStyle.ForeColor = Color.Black;
            dgvPayableBalance.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            DataGridViewImageColumn img1 = new DataGridViewImageColumn();

            img1.Image = Inventory.Properties.Resources.user_edit;

            dgvPayableBalance.Columns.Insert(6, img1);
            img1.HeaderText = "Pay Now"; //
            img1.Name = "PayBalance";
            dgvPayableBalance.Columns["PayBalance"].Width = 120;
            this.dgvPayableBalance.Columns["Paid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayableBalance.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayableBalance.Columns["NetAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayableBalance.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {

                dgvPayableBalance.Columns["VendorId"].Width = 100;
                dgvPayableBalance.Columns["VendorName"].Width = 250;
                dgvPayableBalance.Columns["NetAmount"].Width = 110;
                dgvPayableBalance.Columns["Balance"].Width = 80;
                dgvPayableBalance.Columns["Total"].Width = 80;
                dgvPayableBalance.Columns["Paid"].Width = 80;
                dgvPayableBalance.Columns["PayBalance"].Width = 100;
            }
            else
            {
                dgvPayableBalance.Columns["VendorId"].Width = 100;
                dgvPayableBalance.Columns["VendorName"].Width = 300;
                dgvPayableBalance.Columns["NetAmount"].Width = 155;
                dgvPayableBalance.Columns["Balance"].Width = 155;
                dgvPayableBalance.Columns["Total"].Width = 100;
                dgvPayableBalance.Columns["Paid"].Width = 100;
                dgvPayableBalance.Columns["PayBalance"].Width = 92;
            }


            //dgvPayableBalance.CurrentCell = dgvPayableBalance[9,0];
            //dgvPayableBalance.Focus();
            //this.ActiveControl = dgvPayableBalance;
        }
        public void GetAccountPayableBalance()
        {
            DataTable dt = AccountPayableBAL.GetAccountPayableBalance();
            //dgvReceivableBalance.DataSource = dt;
            dgvPayableBalance.Columns.Clear();
            dgvPayableBalance.DataSource = dt;
            //dgvPayableBalance.Columns["VendorId"].Visible = false;
           //dgvPayableBalance.Columns["OrderNumber"].Visible = false;
            //dgvPayableBalance.Columns["InvoiceDate"].HeaderText = "Invoice Date";
            //dgvPayableBalance.Columns["InvoiceNo"].HeaderText = "Invoice No";
            //dgvPayableBalance.Columns["PurchaseId"].HeaderText = "Purchase Id";
            dgvPayableBalance.Columns["NetAmount"].HeaderText = "Total_Bill_Amount";
            dgvPayableBalance.Columns["VendorId"].HeaderText = "Vendor Id";
            dgvPayableBalance.Columns["VendorName"].HeaderText = "Vendor Name";
            dgvPayableBalance.Columns["Total"].HeaderText = "NO_Of_Bills";
            //dgvPayableBalance.Columns["InvoiceDate"].Width = 120;
            //dgvPayableBalance.Columns["InvoiceNo"].Width = 100;
            //dgvPayableBalance.Columns["PurchaseId"].Width = 120;

            //dgvPayableBalance.Columns["VendorId"].Width = 130;
            //dgvPayableBalance.Columns["VendorName"].Width = 150;
            //dgvPayableBalance.Columns["NetAmount"].Width = 180;            
            //dgvPayableBalance.Columns["Balance"].Width = 200;
            //dgvPayableBalance.Columns["Total"].Width = 200;
            //dgvPayableBalance.Columns["Paid"].Width = 200;

           
            //dgvPayableBalance.Columns["ReorderPoint"].Width = 150;

            //dgvPayableBalance.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvPayableBalance.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvPayableBalance.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvPayableBalance.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10.1F, FontStyle.Bold);
            dgvPayableBalance.DefaultCellStyle.Font = new Font("Tahoma", 12.1F, GraphicsUnit.Pixel);
            dgvPayableBalance.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvPayableBalance.DefaultCellStyle.ForeColor = Color.Black;
            dgvPayableBalance.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            DataGridViewImageColumn img1 = new DataGridViewImageColumn();

            img1.Image = Inventory.Properties.Resources.user_edit;

            dgvPayableBalance.Columns.Insert(6, img1);
            img1.HeaderText = "Pay Now"; //
            img1.Name = "PayBalance";
            dgvPayableBalance.Columns["PayBalance"].Width = 120;
            this.dgvPayableBalance.Columns["Paid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayableBalance.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayableBalance.Columns["NetAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayableBalance.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {

                dgvPayableBalance.Columns["VendorId"].Width = 100;
                dgvPayableBalance.Columns["VendorName"].Width = 250;
                dgvPayableBalance.Columns["NetAmount"].Width = 110;
                dgvPayableBalance.Columns["Balance"].Width = 80;
                dgvPayableBalance.Columns["Total"].Width = 80;
                dgvPayableBalance.Columns["Paid"].Width = 80;
                dgvPayableBalance.Columns["PayBalance"].Width = 100;
            }
            else
            {
                dgvPayableBalance.Columns["VendorId"].Width = 100;
                dgvPayableBalance.Columns["VendorName"].Width = 300;
                dgvPayableBalance.Columns["NetAmount"].Width = 155;
                dgvPayableBalance.Columns["Balance"].Width = 155;
                dgvPayableBalance.Columns["Total"].Width = 100;
                dgvPayableBalance.Columns["Paid"].Width = 100;
                dgvPayableBalance.Columns["PayBalance"].Width = 92;
            }

           
            //dgvPayableBalance.CurrentCell = dgvPayableBalance[9,0];
            //dgvPayableBalance.Focus();
            //this.ActiveControl = dgvPayableBalance;
        }

        private void dgvPayableBalance_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           // dgvPayableBill
           
            if (e.RowIndex >= 0)
            {
                if (dgvPayableBalance.Columns[e.ColumnIndex].HeaderText == "Pay Now")
                {
                    panelPayable.Visible = true;
                    lblBalancePaidAmount.Text = "0.00";
                    cashpl.Visible = false;
                    
                   
                    for (int i = 0; i < dgvPayableBalance.Rows.Count; i++)
                    {
                        string Id = Convert.ToString(dgvPayableBalance.Rows[e.RowIndex].Cells["VendorId"].Value);
                        GetAccountPayableBills(Id);
                        lblPaidAmount.Text = "0.00";
                        rbtnCash.Checked = false;
                        rbtnCard.Checked = false;
                        rbtnCheque.Checked = false;
                    }
                    lblPaidAmount.Enabled = true;
                }


            }
        }

        public void GetAccountPayableBills(string VendorId)
        {
            DataTable dt = AccountPayableBAL.GetAccountPayableBills(VendorId);

            dgvPayableBill.Columns.Clear();
            dgvPayableBill.DataSource = dt;

            dgvPayableBill.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvPayableBill.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvPayableBill.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
           
            //DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            //dgvReceivableBalance.Columns.Insert(0,chk);
            //chk.HeaderText = "Check Bill";
            //chk.Name = "CheckBill";
            //dgvEstimationBill.Columns["CheckBill"].Width = 100;
            ////dgvEstimationBill.Columns["CheckBill"].DisplayIndex = 0;


            DataGridViewCheckBoxColumn dgvChb = new DataGridViewCheckBoxColumn();
            dgvChb.HeaderText = "Bill Check";
            dgvChb.Name = "chbPass";
            dgvChb.FlatStyle = FlatStyle.Standard;

            //dgvChb.TrueValue = false;
            //dgvChb.ThreeState = true;
            //dgvChb.CellTemplate.Style.BackColor = System.Drawing.Color.LightBlue;

            dgvPayableBill.Columns.Add(dgvChb);
            dgvPayableBill.Columns["VendorId"].Visible = false;

            //dgvPayableBill.Columns["TotalQuantity"].Visible = false;
            //dgvPayableBill.Columns["Customerid"].Visible = false;
            //dgvPayableBill.Columns["LessAmount"].Visible = false;

            dgvPayableBill.Columns["chbPass"].DisplayIndex = 0;

           
            dgvPayableBill.Columns["TotalAmount"].HeaderText = "Bill Amount";
            dgvPayableBill.Columns["OrderNumber"].HeaderText = "Bill Number";

            this.dgvPayableBill.Columns["Paid"].DefaultCellStyle
    .Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayableBill.Columns["Balance"].DefaultCellStyle
   .Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayableBill.Columns["TotalAmount"].DefaultCellStyle
   .Alignment = DataGridViewContentAlignment.MiddleRight;
            if (dgvPayableBill.Rows.Count > 0)
            {
                dgvPayableBill.Focus();
                dgvPayableBill.CurrentCell = dgvPayableBill[7, 0];
            }

            foreach (DataGridViewColumn column in dgvPayableBill.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }

        private void dgvPayableBill_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int count = 0;
            if (e.RowIndex >= 0)
            {

                    if (dgvPayableBill.Columns[e.ColumnIndex].HeaderText == "Bill Check")
                    {

                        foreach (DataGridViewRow row in dgvPayableBill.Rows)
                        {
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells["chbPass"];

                            chk.Value = chk.FalseValue;
                        }
                        DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)dgvPayableBill.Rows[e.RowIndex].Cells["chbPass"];
                        CbxCell.Value = true;

                    }
                decimal TotalBalance = 0.00M;
                foreach (DataGridViewRow row in dgvPayableBill.Rows)
                {
                    DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells["chbPass"];
                    if (Convert.ToBoolean(CbxCell.Value) == true)
                    {
                        //CbxCell.Value = false;
                        count += 1;
                        TotalBalance += Convert.ToDecimal(row.Cells["Balance"].Value);
                    }
                }
                lblTotalBillSelected.Text = Convert.ToString(count);
                lblBalancePaidAmount.Text = Convert.ToString(TotalBalance);
            }
        }

        private void btnPayablePanelClose_Click(object sender, EventArgs e)
        {
            panelPayable.Visible = false;
            lblBalancePaidAmount.Text = "0.00";
            lblPaidAmount.Text = "0.00";
            rbtnCash.Checked = false;
            rbtnCard.Checked = false;
            rbtnCheque.Checked = false;
        }

        private void lblTotalBillSelected_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblTotalBillSelected.Text) > 0)
            {
                rbtnCard.Enabled = true;
                rbtnCash.Enabled = true;
                rbtnCheque.Enabled = true;
            }
            else
            {
                lblPaidAmount.Text = "0.00";

                rbtnCard.Enabled = false;
                rbtnCash.Enabled = false;
                rbtnCheque.Enabled = false;


                rbtnCard.Checked = false;
                rbtnCash.Checked = false;
                rbtnCheque.Checked = false;
            }


            
        }

        private void btnAccountPayableClear_Click(object sender, EventArgs e)
        {
            clear();
            GetAccountPayableBalance();
            dgvPayableBill.DataSource = null;
           
               
        }

        private void rbtnCash_CheckedChanged(object sender, EventArgs e)
        {
            clearpanel();
            paymentdenobind();
            txtTotal.Text = "0.00";
            txtReturnBalAmount.Text = "0.00";
            lblPaidAmount.Enabled = false;
            btnAccountPayableSave.Visible = false;
            if (rbtnCash.Checked == true)
            {
                //lblBalancePaidAmount,lblPaidAmount

                if (!string.IsNullOrEmpty(Convert.ToString(lblPaidAmount.Text)))
                {
                if (Convert.ToString(lblPaidAmount.Text) != "0.00" )
                {

                    if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) == Convert.ToDecimal(lblBalancePaidAmount.Text)))
                    {
                        lblcashamountpay.Text = lblPaidAmount.Text;
                        panelCard.Visible = false;
                        cashpl.Visible = true;
                        cashddl.Focus();
                        cashddl.CurrentCell = cashddl[1, 0];
                        //txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(/.Text) - Convert.ToDecimal(lblBalancePaidAmount.Text));
                    }
                    else if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) >Convert.ToDecimal(lblBalancePaidAmount.Text)))
                    {
                        MessageBox.Show("Amount pay should be equal to  Estimation Amount ");
                        panelCard.Visible = false;
                        cashpl.Visible = false;
                        
                        rbtnCard.Checked = false;
                        rbtnCheque.Checked = false;
                        rbtnCash.Checked = false;
                        lblPaidAmount.Enabled = true;
                    }
                    else
                    {
                        lblcashamountpay.Text = lblPaidAmount.Text;
                        panelCard.Visible = false;
                        cashpl.Visible = true;
                        cashddl.Focus();
                        cashddl.CurrentCell = cashddl[1, 0];
                    }

                }
                else
                {
                    MessageBox.Show("Enter Amount pay.");
                    rbtnCard.Checked = false;
                    rbtnCheque.Checked = false;
                    rbtnCash.Checked = false;
                    lblPaidAmount.Enabled = true;
                    lblPaidAmount.Focus();
                }
                }
                else
                {
                    MessageBox.Show("Enter Amount pay.");
                    rbtnCard.Checked = false;
                    rbtnCheque.Checked = false;
                    rbtnCash.Checked = false;
                    lblPaidAmount.Enabled = true;
                    lblPaidAmount.Focus();
                }


            }
        }

        private void rbtnCard_CheckedChanged(object sender, EventArgs e)
        {
            clearpanel();
            paymentdenobind();
            txtTotal.Text = "0.00";
            lblPaidAmount.Enabled = false;
            btnAccountPayableSave.Visible = false;
            lblcheque.Text = "Transfer";
            lblcheque.Visible = true;
            lblInstrumentDisplay.Visible = false;
            txtCardorChequeNo.Visible = false;
            lblTransferhead.Text = "Transaction Details";

            lblCardorChequeAmt.Text = lblPaidAmount.Text;
            cashpl.Visible = false;
            if (rbtnCard.Checked == true)
            {

                if (!string.IsNullOrEmpty(Convert.ToString(lblPaidAmount.Text)))
                {
                    if (Convert.ToString(lblPaidAmount.Text) != "0.00")
                    {

                        if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) == Convert.ToDecimal(lblBalancePaidAmount.Text)))
                        {
                            lblcashamountpay.Text = lblPaidAmount.Text;
                            //rbtnVisa.Visible = true;
                            //rbtnMastro.Visible = true;


                            panelCard.Visible = true;
                            ddlBankAccount.Focus();


                            //lblInstrumentDisplay.Visible = true;
                            //lblInstrumentSourceDisplay.Visible = true;

                            //txtInstrumentNo.Visible = true;
                            //txtInstrumentSource.Visible = true;
                            //txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblBalancePaidAmount.Text));
                        }
                        else if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) > Convert.ToDecimal(lblBalancePaidAmount.Text)))
                        {
                            MessageBox.Show("Amount pay should be equal to Estimation Amount");
                            lblPaidAmount.Enabled = true;
                            panelCard.Visible = false;
                            cashpl.Visible = false;

                            rbtnCard.Checked = false;
                            rbtnCheque.Checked = false;
                            rbtnCash.Checked = false;

                            //lblInstrumentDisplay.Visible = false;
                            //lblInstrumentSourceDisplay.Visible = false;

                            //txtInstrumentNo.Visible = false;
                            //txtInstrumentSource.Visible = false;
                        }
                        else
                        {
                            lblcashamountpay.Text = lblPaidAmount.Text;
                            //rbtnVisa.Visible = true;
                            //rbtnMastro.Visible = true;


                            panelCard.Visible = true;
                            ddlBankAccount.Focus();
                        }

                    }
                    else
                    {
                        MessageBox.Show("Enter Amount pay.");
                        
                        rbtnCard.Checked = false;
                        rbtnCheque.Checked = false;
                        rbtnCash.Checked = false;
                        lblPaidAmount.Enabled = true;
                        lblPaidAmount.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Enter Amount pay.");
                  
                    rbtnCard.Checked = false;
                    rbtnCheque.Checked = false;
                    rbtnCash.Checked = false;
                    lblPaidAmount.Enabled = true;
                    lblPaidAmount.Focus();
                }

            }
                
        }

        private void rbtnCheque_CheckedChanged(object sender, EventArgs e)
        {
            clearpanel();
            paymentdenobind();
            btnAccountPayableSave.Visible = false;
            txtTotal.Text = "0.00";
            lblPaidAmount.Enabled = false;
            lblcheque.Text = "Cheque";
            lblInstrumentDisplay.Visible=true;
            txtCardorChequeNo.Visible=true;
            lblTransferhead.Text = "Cheque Details";

            lblCardorChequeAmt.Text = lblPaidAmount.Text;
            if (rbtnCheque.Checked == true)
            {

                if (!string.IsNullOrEmpty(Convert.ToString(lblPaidAmount.Text)))
                {
                    if (Convert.ToString(lblPaidAmount.Text) != "0.00")
                    {

                        if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) == Convert.ToDecimal(lblBalancePaidAmount.Text)))
                        {

                            //lblInstrumentDisplay.Visible = false;
                            //lblInstrumentSourceDisplay.Visible = false;
                            //txtInstrumentNo.Visible = false;
                            //txtInstrumentSource.Visible = false;

                            //rbtnVisa.Visible = false;
                            //rbtnMastro.Visible = false;
                            lblcheque.Visible = true;

                            cashpl.Visible = false;
                            panelCard.Visible = true;
                            //txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblBalancePaidAmount.Text));
                        }
                        else if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) > Convert.ToDecimal(lblBalancePaidAmount.Text)))
                        {
                            MessageBox.Show("Amount pay should be equal to Estimation Amount ");
                            lblPaidAmount.Enabled = true;
                            panelCard.Visible = false;
                            cashpl.Visible = false;

                            rbtnCard.Checked = false;
                            rbtnCheque.Checked = false;
                            rbtnCash.Checked = false;
                        }
                        else
                        {
                            lblcheque.Visible = true;

                            cashpl.Visible = false;
                            panelCard.Visible = true;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Enter Amount pay.");
                        
                        rbtnCard.Checked = false;
                        rbtnCheque.Checked = false;
                        rbtnCash.Checked = false;
                        lblPaidAmount.Enabled = true;
                        lblPaidAmount.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Enter Amount pay.");
                 
                    rbtnCard.Checked = false;
                    rbtnCheque.Checked = false;
                    rbtnCash.Checked = false;
                    lblPaidAmount.Enabled = true;
                    lblPaidAmount.Focus();
                }

            }
        }

        private void btnCloseDenominationPanel_Click(object sender, EventArgs e)
        {
            cashpl.Visible = false;
            rbtnCash.Checked = false;
            rbtnCard.Checked = false;
            rbtnCheque.Checked = false;
            lblPaidAmount.Enabled = true;
        }

        private void btnCardPanelClose_Click(object sender, EventArgs e)
        {
            panelCard.Visible = false;
            rbtnCash.Checked = false;
            rbtnCard.Checked = false;
            rbtnCheque.Checked = false;
            lblPaidAmount.ReadOnly = false;
            lblPaidAmount.Enabled = true;
        }

        private void cashddl_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (cashddl.CurrentCell.ColumnIndex == 1)
            {
                if (e.Control is TextBox)
                {
                    TextBox Count = e.Control as TextBox;
                    Count.MaxLength = 8;
                    Count.KeyPress += new KeyPressEventHandler(Count_KeyPress);
                }
            }
        }

        void Count_KeyPress(object sender, KeyPressEventArgs e)
        {
            string headerText = cashddl.Columns[cashddl.CurrentCell.ColumnIndex].HeaderText;
            if (headerText == "Count")
            {
                if (!(char.IsDigit(e.KeyChar)))
                {
                    if (e.KeyChar != '\b')
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void cashddl_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            decimal sum = 0.00M;
            for (int i = 0; i < cashddl.Rows.Count; i++)
            {
                cashddl.Rows[i].Cells[2].Value = Convert.ToDecimal(cashddl.Rows[i].Cells[0].Value) * Convert.ToDecimal(cashddl.Rows[i].Cells[1].Value);
                sum = sum + Convert.ToDecimal(cashddl.Rows[i].Cells[2].Value);
                txtTotal.Text = Convert.ToString(sum);
            }

            if (Convert.ToDecimal(txtTotal.Text) > 0)
            {
                txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblcashamountpay.Text));
            }
            else
            {
                txtReturnBalAmount.Text = "0.00";
            }
        }

        private void btncashpay_Click(object sender, EventArgs e)
        {
            //decimal sum = 0;
            //for (int i = 0; i < cashddl.Rows.Count; i++)
            //{
            //    sum += Convert.ToDecimal(cashddl.Rows[i].Cells[2].Value);
            //}
            //lblPaidAmount.Text = Convert.ToString(sum) + ".00";

            if (Convert.ToString(txtTotal.Text) != "0.00")
            {

                if (Convert.ToDecimal(txtReturnBalAmount.Text)>0)
                {
                    MessageBox.Show("Please Enter Return Denomination");
                }
                else if (Convert.ToDouble(lblcashamountpay.Text) > Convert.ToDouble(txtTotal.Text))
                {
                    MessageBox.Show("Please Enter Correct Denomination");
                }
                else
                {
                    cashpl.Visible = false;
                    panelPayable.Visible = false;
                    lblPaidAmount.Enabled = false;
                    panelPayable.Visible = false;
                    btnAccountPayableSave.Visible = true;
                    btnAccountPayableSave.Focus();
                  
                }
            }
            else
            {
                MessageBox.Show("Enter paid amount in Denomination");
            }
           
        }
        public bool Validation()
        {
            bool Status = true;
            string msg = "";
            int i = 0;

            //1.Please check receive Balance

            if (dgvPayableBill.Rows.Count == 0)
            {
                i++;
                msg = msg + "*Please check Pay Now" + "\n";
                if(i==1)
                {
                    dgvPayableBill.Focus();
                }

            }

            //2.Please check Bill
            if (dgvPayableBill.Rows.Count > 0)
            {
                int temp = 0;
                for (int j = 0; j < dgvPayableBill.Rows.Count; j++)
                {

                    int col = dgvPayableBill.CurrentCell.ColumnIndex;
                    int row = dgvPayableBill.CurrentCell.RowIndex;

                    DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)dgvPayableBill.Rows[j].Cells["chbPass"];
                    if (Convert.ToBoolean(CbxCell.Value) == true)
                    {
                        temp++;
                    }
                }

                if (temp == 0)
                {
                    i++;
                    msg = msg + "*Please check bill" + "\n";
                    if (i == 1)
                    {
                        dgvPayableBill.Focus();
                        dgvPayableBill.CurrentCell = dgvPayableBill[7, 0];
                    }
                }
            }

            //3.please check Payment mode

            if (!string.IsNullOrEmpty(lblPaidAmount.Text) && Convert.ToString(lblPaidAmount.Text) != "0.00")
            {
                if (Convert.ToDecimal(lblPaidAmount.Text) != Convert.ToDecimal(lblBalancePaidAmount.Text))
                {
                    //i++;
                    //msg = msg + "*Paid amount should be equal to estimation amount" + "\n";
                    //if (i == 1)
                    //{
                    //    lblPaidAmount.Focus();
                    //}
                }
            }
            else
            {
                i++;
                msg = msg + "*Please enter paid amount" + "\n";
                if (i == 1)
                {
                    lblPaidAmount.Enabled = true;
                    lblPaidAmount.Focus();
                }
            }



            //4.please check Payment mode

            if (rbtnCard.Checked == false && rbtnCash.Checked == false && rbtnCheque.Checked == false)
            {
                i++;
                msg = msg + "*Please choose pay mode among Cash,Card or Cheque" + "\n";

            }
            else
            {
                if (rbtnCard.Checked == true || rbtnCash.Checked == true || rbtnCheque.Checked == true)
                {

                    if (!string.IsNullOrEmpty(lblPaidAmount.Text) || Convert.ToString(lblPaidAmount.Text) != "0.00")
                    {
                        //if (Convert.ToDecimal(lblPaidAmount.Text) != Convert.ToDecimal(lblBalancePaidAmount.Text))
                        //{
                        //    i++;
                        //    msg = msg + "*Paid amount should equal to estimation amount" + "\n";
                        //    if (i == 1)
                        //    {
                        //        lblPaidAmount.Focus();
                        //    }
                        //}
                    }
                    else
                    {
                        i++;
                        msg = msg + "*Please enter paid amount" + "\n";
                        if (i == 1)
                        {
                            lblPaidAmount.Enabled = true;
                            lblPaidAmount.Focus();
                        }
                    }
                }
               
            }

            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + msg);
                Status = false;

            }
            return Status;


        }
        private void btnAccountPayableSave_Click(object sender, EventArgs e)
        {
            string[] status = new string[3];
            string remainingamt = Convert.ToString(lblPaidAmount.Text);
            AccountPayableBAL ObjAccountPayableBAL = new AccountPayableBAL();

            if (Validation())
            {
                if (dgvPayableBill.Rows.Count > 0)
                {
                    for (int i = 0; i < dgvPayableBill.Rows.Count; i++)
                    {
                        int col = dgvPayableBill.CurrentCell.ColumnIndex;
                        int row = dgvPayableBill.CurrentCell.RowIndex;
                        //if (dgvEstimationBill.Columns["Bill Check"].HeaderText == "Bill Check")
                        // {
                        DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)dgvPayableBill.Rows[i].Cells["chbPass"];
                        if (Convert.ToBoolean(CbxCell.Value) == true)
                        {
                            if (rbtnCard.Checked == true || rbtnCash.Checked == true || rbtnCheque.Checked == true)
                            {

                                if (rbtnCash.Checked == true)
                                {
                                    //if (txtTotal.Text != "0.00")
                                    //{ 
                                    int coin = 0;
                                    for (int j = 0; j < cashddl.Rows.Count; j++)
                                    {
                                        if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 2000)
                                        {
                                            ObjAccountPayableBAL.TwoThousand = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                        }
                                        if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 1000)
                                        {
                                            ObjAccountPayableBAL.Thousand = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                        }
                                        if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 500)
                                        {
                                            ObjAccountPayableBAL.FiveHundred = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                        }
                                        if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 100)
                                        {
                                            ObjAccountPayableBAL.Hundred = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                        }
                                        if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 50)
                                        {
                                            ObjAccountPayableBAL.Fifty = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                        }
                                        if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 20)
                                        {
                                            ObjAccountPayableBAL.Twenty = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                        }
                                        if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 10)
                                        {
                                            ObjAccountPayableBAL.Ten = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                        }
                                        if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 5)
                                        {
                                            ObjAccountPayableBAL.Five = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                        }
                                        if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 2)
                                        {
                                            ObjAccountPayableBAL.Two = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                        }
                                        if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 1)
                                        {
                                            ObjAccountPayableBAL.One = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                        }
                                        
                                    }



                                    //ObjAccountPayableBAL.ReturnAmount = txtReturnAmount.Text;
                                    ObjAccountPayableBAL.Flag = "Cash";
                                    ObjAccountPayableBAL.returndenam = txtTotal.Text;
                                    DateTime dateCash = new DateTime(TransactionDate.Value.Year, TransactionDate.Value.Month, TransactionDate.Value.Day);
                                    ObjAccountPayableBAL.InstrumentDate = dateCash;
                                    //}
                                    //else
                                    //{
                                    //    MessageBox.Show("Enter amount in Denomination");
                                    //}
                                }
                                if (rbtnCard.Checked == true)
                                {
                                    DateTime dateCard = new DateTime(TransactionDate.Value.Year, TransactionDate.Value.Month, TransactionDate.Value.Day);

                                    ObjAccountPayableBAL.Type = "Transfer";
                                    ObjAccountPayableBAL.Reference = txtReference.Text;
                                    ObjAccountPayableBAL.Flag = "Card";

                                    ObjAccountPayableBAL.InstrumentBank = ddlBankAccount.Text;
                                    ObjAccountPayableBAL.InstrumentSource = ddlBankAccountTo.Text;
                                    ObjAccountPayableBAL.InstrumentNo = null;
                                   
                                    //transdate = TransactionDate.Text.Split('-');
                                    //ObjAccountPayableBAL.InstrumentDate = transdate[2] + "-" + transdate[1] + "-" + transdate[0];

                                    ObjAccountPayableBAL.InstrumentDate = dateCard;
                                }
                                if (rbtnCheque.Checked == true)
                                {
                                    DateTime dateCheque = new DateTime(TransactionDate.Value.Year, TransactionDate.Value.Month, TransactionDate.Value.Day);
                                    ObjAccountPayableBAL.Type = "Cheque";
                                    ObjAccountPayableBAL.InstrumentBank = ddlBankAccount.Text;
                                    ObjAccountPayableBAL.InstrumentSource = ddlBankAccountTo.Text;
                                    ObjAccountPayableBAL.InstrumentNo = txtCardorChequeNo.Text;
                                    //transdate = TransactionDate.Text.Split('-');
                                    //ObjAccountPayableBAL.InstrumentDate = transdate[2] + "-" + transdate[1] + "-" + transdate[0];
                                    ObjAccountPayableBAL.InstrumentDate = dateCheque;
                                    ObjAccountPayableBAL.Reference = txtReference.Text;
                                    ObjAccountPayableBAL.Flag = "Cheque";
                                }

                                ObjAccountPayableBAL.BillId = Convert.ToString(dgvPayableBill.Rows[i].Cells["OrderNumber"].Value);
                                ObjAccountPayableBAL.CustomerId = Convert.ToString(dgvPayableBill.Rows[i].Cells["VendorId"].Value);


                                if (Convert.ToInt32(lblTotalBillSelected.Text) > 1)
                                {
                                    string symbol = remainingamt.Substring(0, 1);
                                    if (symbol == "-")
                                    {
                                        string[] s = remainingamt.Split('-');
                                        ObjAccountPayableBAL.PaidAmount = s[1];
                                    }
                                    else
                                    {
                                        ObjAccountPayableBAL.PaidAmount = remainingamt;
                                    }

                                    ObjAccountPayableBAL.IsRecord = true; // 1
                                }
                                else
                                {
                                    ObjAccountPayableBAL.IsRecord = false;  // 0
                                    ObjAccountPayableBAL.PaidAmount = Convert.ToString(lblPaidAmount.Text);
                                }


                                if (rbtnCard.Checked == true || rbtnCash.Checked == true || rbtnCheque.Checked == true)
                                {

                                    if (rbtnCash.Checked == true)
                                    {
                                        if (txtTotal.Text != "0.00" )
                                        {

                                            status = AccountPayableBAL.SaveAccountPayableBalance(ObjAccountPayableBAL);
                                        }
                                        else
                                        {
                                            MessageBox.Show("Enter amount in denomination");
                                        }
                                    }

                                    if (rbtnCard.Checked == true)
                                    {
                                        

                                            status = AccountPayableBAL.SaveAccountPayableBalance(ObjAccountPayableBAL);
                                       
                                    }

                                    if (rbtnCheque.Checked == true)
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(txtCardorChequeNo.Text)) && (lblcheque.Text == "Cheque"))
                                        {

                                            status = AccountPayableBAL.SaveAccountPayableBalance(ObjAccountPayableBAL);
                                        }
                                        else
                                        {
                                            MessageBox.Show("Enter Cheque Number");
                                        }
                                    }


                                }

                                if (Convert.ToInt32(status[0]) == 1)
                                {
                                    remainingamt = Convert.ToString(status[1]);
                                    lblTotalBillSelected.Text = Convert.ToString(Convert.ToInt32(lblTotalBillSelected.Text) - 1);
                                }


                                //GetQuotationEstimationBills();
                            }


                        }

                    }
                }

                if (Convert.ToInt32(status[0]) == 1)
                {

                    if (Convert.ToDouble(txtReturnAmount.Text)>0)
                    {
                        SavepaymentDenomination(status[2]);
                    }
                    GetAccountPayableBalance();
                    dgvPayableBill.DataSource = null;
                    MessageBox.Show("Paid Successfully");
                    clear();
                }
            }
        }



        public void SavepaymentDenomination(string id)
        {

            // Pnloading4.Visible = true;
            objQuotationbal.recivetransid = id;
            //ReceiptId = lblid.Text;
            if (Convert.ToString(cashddlReturn.Rows[0].Cells[1].Value) != "0")
            {
                objQuotationbal.recivetwothousand = "-" + Convert.ToString(cashddlReturn.Rows[0].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivetwothousand = Convert.ToString(cashddlReturn.Rows[0].Cells[1].Value);
            }
            if (Convert.ToString(cashddlReturn.Rows[1].Cells[1].Value) != "0")
            {
                objQuotationbal.recivethousand = "-" + Convert.ToString(cashddlReturn.Rows[0].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivethousand = Convert.ToString(cashddlReturn.Rows[0].Cells[1].Value);
            }
            if (Convert.ToString(cashddlReturn.Rows[2].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefivehundred = "-" + Convert.ToString(cashddlReturn.Rows[1].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefivehundred = Convert.ToString(cashddlReturn.Rows[1].Cells[1].Value);
            }



            if (Convert.ToString(cashddlReturn.Rows[3].Cells[1].Value) != "0")
            {
                objQuotationbal.recivehundred = "-" + Convert.ToString(cashddlReturn.Rows[2].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivehundred = Convert.ToString(cashddlReturn.Rows[2].Cells[1].Value);
            }


            if (Convert.ToString(cashddlReturn.Rows[4].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefifty = "-" + Convert.ToString(cashddlReturn.Rows[3].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefifty = Convert.ToString(cashddlReturn.Rows[3].Cells[1].Value);
            }


            if (Convert.ToString(cashddlReturn.Rows[5].Cells[1].Value) != "0")
            {
                objQuotationbal.recivetwenty = "-" + Convert.ToString(cashddlReturn.Rows[4].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivetwenty = Convert.ToString(cashddlReturn.Rows[4].Cells[1].Value);
            }

            if (Convert.ToString(cashddlReturn.Rows[6].Cells[1].Value) != "0")
            {
                objQuotationbal.reciveten = "-" + Convert.ToString(cashddlReturn.Rows[5].Cells[1].Value);

            }
            else
            {
                objQuotationbal.reciveten = Convert.ToString(cashddlReturn.Rows[5].Cells[1].Value);
            }


            if (Convert.ToString(cashddlReturn.Rows[7].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefive = "-" + Convert.ToString(cashddlReturn.Rows[6].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefive = Convert.ToString(cashddlReturn.Rows[6].Cells[1].Value);
            }

            if (Convert.ToString(cashddlReturn.Rows[8].Cells[1].Value) != "0")
            {
                objQuotationbal.recivecoin = "-" + Convert.ToString(cashddlReturn.Rows[7].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivecoin = Convert.ToString(cashddlReturn.Rows[7].Cells[1].Value);
            }

            if (Convert.ToString(cashddlReturn.Rows[9].Cells[1].Value) != "0")
            {
                objQuotationbal.ReceiveOne = "-" + Convert.ToString(cashddlReturn.Rows[8].Cells[1].Value);

            }
            else
            {
                objQuotationbal.ReceiveOne = Convert.ToString(cashddlReturn.Rows[8].Cells[1].Value);
            }

            objQuotationbal.OAmount = "-" + txtReturnAmount.Text;

            string output = objQuotationbal.SavepaymentDenomination(objQuotationbal);

            if (output == "1")
            {
                //Pnloading4.Visible = false;

            }

        }

        private void btnCardPanelOK_Click(object sender, EventArgs e)
        {
            if (validationPaymentCheque())
            {
                panelCard.Visible = false;
                btnAccountPayableSave.Visible = true;
            }
            
        }
        public bool validationPaymentCheque()
        {
            bool Status = true;
            string msg = "";
            int i = 0;
           
            if (Convert.ToString(ddlBankAccount.SelectedIndex )== "-1")
            {
                i++;
                msg = msg + "*Select from account" + "\n";
                if (i == 1)
                    this.ActiveControl = ddlBankAccount;

            }
            if (Convert.ToString(ddlBankAccountTo.SelectedIndex) == "-1")
            {
                i++;
                msg = msg + "*Select to account" + "\n";
                if (i == 1)
                    this.ActiveControl = ddlBankAccountTo;

            }
            if (rbtnCheque.Checked==true)
            {
                if (string.IsNullOrEmpty(txtCardorChequeNo.Text))
                {
                    i++;
                    msg = msg + "*Enter cheque no" + "\n";
                    if (i == 1)
                        this.ActiveControl = txtCardorChequeNo;

                }
            }
            
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + msg);
                Status = false;
            }
            return Status;


        }
        public void clear()
        {
            
            lblTotalBillSelected.Text = "0";
            lblPaidAmount.Text = "0.00";
            lblBalancePaidAmount.Text = "0.00";
            panelPayable.Visible = false;
            pnlprodsearch.Visible = false;
            paymentdenobind();
            txtTotal.Text = "0.00";
            lblPaidAmount.Enabled = true;
            btnAccountPayableSave.Visible = false;
            GetAccountPayableBalance();
          
        }

        public void clearpanel()
        {
            txtTotal.Text = "0.00";
            paymentdenobind();
            txtCardorChequeNo.Text = string.Empty;
            txtReference.Text = string.Empty;
            BindBankAccount();
            BindBankAccountTo();
            TransactionDate.Text = Convert.ToString(DateTime.Now.Date);
            //rbtnVisa.Checked = false;
            //rbtnMastro.Checked = false;
        }

        private void txtCardorChequeNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar)))
                {
                    if (e.KeyChar != '\b')
                    {
                        e.Handled = true;
                    }
                }        
        }

        private void txtReference_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetterOrDigit(e.KeyChar)))
            {
                if (e.KeyChar != '\b')
                {
                    e.Handled = true;
                }
            }    
        }

        private void lblPaidAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar)))
            {
                if (e.KeyChar != '\b')
                {
                    e.Handled = true;
                }
            }
        }

        private void lblTotalBillSelected_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F3)
            {
                pnlprodsearch.Visible = true;
                txtprodsearch.SelectionStart = 0;
                txtprodsearch.SelectionLength = txtprodsearch.Text.Length;
                txtprodsearch.Focus();
                return true;
            }

            if (keyData == Keys.Escape)
            {
                if (pnlprodsearch.Visible)
                {
                    GetAccountPayableBalance();
                    pnlprodsearch.Visible = false;
                    return true;
                }
                else
                {
                    DialogResult result = MessageBox.Show("Do you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        this.Close();
                        return true;
                    }
                }
            }

            if (txtprodsearch.Focused)
            {
                if (keyData == (Keys.Enter))
                {
                    if (!string.IsNullOrEmpty(txtprodsearch.Text))
                    {
                        GetAccountPayableBalanceSearch();
                    }
                }
            }
           

            if (dgvPayableBalance.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    //this.ActiveControl = txtRemarks;
                    dgvPayableBalance.Focus();
                    dgvPayableBalance.CurrentCell = dgvPayableBalance[6, dgvPayableBalance.CurrentCell.RowIndex];
                    this.ActiveControl = dgvPayableBalance;
                    //dgvOrder.BeginEdit(true);
                    dgvPayableBill.Focus();
                    return true;
                }
            }


            if (lblPaidAmount.Focused)
            {
                if(keyData==Keys.Tab)
                {
                    rbtnCash.Focus();
                    return true;
                }

            }
            //if (cbVendor.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        //this.ActiveControl = txtRemarks;
            //        dgvOrder.Focus();
            //        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex];
            //        this.ActiveControl = dgvOrder;
            //        //dgvOrder.BeginEdit(true);
            //        //pnsearch.Visible = false;
            //        return true;
            //    }
            //}

            if (keyData == Keys.Escape)
            {
                DialogResult result = MessageBox.Show("Do you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    this.Close();
                }
                return true;
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void dgvPayableBill_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData==Keys.Enter)
            {
                if (dgvPayableBill.CurrentCell.RowIndex + 1 == dgvPayableBill.Rows.Count)
                {
                    lblPaidAmount.Focus();
                }
            }
        }

        private void AccountPayable_Load(object sender, EventArgs e)
        {
            paymentdenoReturnbind();
        }

        private void cashddl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (cashddl.CurrentCell.RowIndex == 8)
                {
                    cashddl.Rows[8].Cells[1].Selected = false;
                    if (btnPayReturnBalance.Enabled)
                    {
                        btnPayReturnBalance.Focus();
                    }
                    else
                    {
                        btncashpay.Focus();
                    }
                    
                }
            }
        }

        private void btnPayReturnBalance_Click(object sender, EventArgs e)
        {
            cashplReturn.Visible = true;
            lblReturnbalance.Text = txtReturnBalAmount.Text;
            cashddlReturn.Columns.Clear();
            paymentdenoReturnbind();
            cashddlReturn.Focus();
            txtReturnAmount.Text = "0.00";
            cashddlReturn.CurrentCell = cashddlReturn[1, 0];
        }
        public void paymentdenoReturnbind()
        {
            cashddlReturn.Rows.Clear();
            cashddlReturn.ColumnCount = 5;
            cashddlReturn.Columns[0].Name = "Denomination";
            cashddlReturn.Columns[1].Name = "Count";
            cashddlReturn.Columns[2].Name = "Amount";
            cashddlReturn.Columns[3].Name = "DenominationID";
            cashddlReturn.Columns[4].Name = "unique";


            this.cashddlReturn.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.cashddlReturn.Columns[0].Width = 150;
            this.cashddlReturn.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.cashddlReturn.Columns[0].ReadOnly = true;
            this.cashddlReturn.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.cashddlReturn.Columns[1].Width = 100;
            this.cashddlReturn.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.cashddlReturn.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.cashddlReturn.Columns[2].Width = 100;
            this.cashddlReturn.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.cashddlReturn.Columns[2].ReadOnly = true;
            this.cashddlReturn.Columns[3].Visible = false;
            this.cashddlReturn.Columns[4].Visible = false;

            DataTable payment = new DataTable();
            payment = paymentDeno();
            int row = payment.Rows.Count;
            cashddlReturn.Rows.Add(row);
            if (row > 0)
            {
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < payment.Columns.Count; j++)
                    {
                        cashddlReturn.Rows[i].Cells[j].Value = payment.Rows[i][j];
                    }
                }
            }
            cashddlReturn.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
            cashddlReturn.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
            //cashddlReturn.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            cashddlReturn.DefaultCellStyle.BackColor = Color.Gainsboro;
            cashddlReturn.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            foreach (DataGridViewColumn column in cashddlReturn.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void txtTotal_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToString(txtTotal.Text) != "")
            {

                if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) == Convert.ToDecimal(lblBalancePaidAmount.Text)))
                {
                    if (Convert.ToBoolean(Convert.ToDecimal(txtTotal.Text) == Convert.ToDecimal(lblcashamountpay.Text)))
                    {

                        txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblPaidAmount.Text));

                    }
                    if (Convert.ToBoolean(Convert.ToDecimal(txtTotal.Text) < Convert.ToDecimal(lblcashamountpay.Text)))
                    {

                        txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblPaidAmount.Text));

                    }
                    if (Convert.ToBoolean(Convert.ToDecimal(txtTotal.Text) > Convert.ToDecimal(lblcashamountpay.Text)))
                    {

                        txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblPaidAmount.Text));

                    }
                }
                if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) > Convert.ToDecimal(lblBalancePaidAmount.Text)))
                {
                    if (Convert.ToBoolean(Convert.ToDecimal(txtTotal.Text) == Convert.ToDecimal(lblPaidAmount.Text)))
                    {
                        txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblBalancePaidAmount.Text));
                    }

                    if (Convert.ToBoolean(Convert.ToDecimal(txtTotal.Text) > Convert.ToDecimal(lblPaidAmount.Text)))
                    {
                        txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblBalancePaidAmount.Text));
                    }

                    if (Convert.ToBoolean(Convert.ToDecimal(txtTotal.Text) < Convert.ToDecimal(lblPaidAmount.Text)))
                    {
                        txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblBalancePaidAmount.Text));
                    }

                }

                if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) < Convert.ToDecimal(lblBalancePaidAmount.Text)))
                {
                    if (Convert.ToBoolean(Convert.ToDecimal(txtTotal.Text) == Convert.ToDecimal(lblPaidAmount.Text)))
                    {
                        //txtReturnBalAmount.Text = "0.00";
                        txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblPaidAmount.Text));
                    }

                    if (Convert.ToBoolean(Convert.ToDecimal(txtTotal.Text) < Convert.ToDecimal(lblcashamountpay.Text)))
                    {

                        //txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblPaidAmount.Text));
                        // txtReturnBalAmount.Text="0.00";
                        // MessageBox.Show("Denomination total amount should not be less than paid amount");

                    }
                    if (Convert.ToBoolean(Convert.ToDecimal(txtTotal.Text) > Convert.ToDecimal(lblcashamountpay.Text)))
                    {

                        txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblPaidAmount.Text));

                    }
                }


            }
        }

        private void txtReturnBalAmount_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToDouble(txtReturnBalAmount.Text) > 0)
            {
                btnPayReturnBalance.Enabled = true;
            }
            else
            {
                btnPayReturnBalance.Enabled = false;
            }
        }

        private void btnReturnOK_Click(object sender, EventArgs e)
        {
            ReturnDenomination();
         
            if (Convert.ToDecimal(lblReturnbalance.Text) == Convert.ToDecimal(txtReturnAmount.Text))
            {
                cashplReturn.Visible = false;
                panelPayable.Visible = false;
                cashpl.Visible = false;
                lblPaidAmount.Enabled = false;
                panelPayable.Visible = false;
                btnAccountPayableSave.Visible = true;
                btnAccountPayableSave.Focus();
                lblPaidAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(txtReturnBalAmount.Text));
            }
           
        }
        public void ReturnDenomination()
        {
            if (Convert.ToString(txtReturnAmount.Text) != "0.00")
            {
                if (Convert.ToDecimal(lblReturnbalance.Text) != Convert.ToDecimal(txtReturnAmount.Text))
                {
                    MessageBox.Show("Total should be equal to return pay amount.");
                   // txtReturnAmount.Text = "0.00";
                }
                else
                {
                    cashplReturn.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("Enter return amount.");
                cashplReturn.Visible = true;
            }
        }

        private void btnCloseReturnDeno_Click(object sender, EventArgs e)
        {
            paymentdenoReturnbind();
            cashplReturn.Visible = false;
            rbtnCash.Checked = false;
            rbtnCard.Checked = false;
            rbtnCheque.Checked = false;
            txtReturnBalAmount.Text = "0.00";
            btnPayReturnBalance.Enabled = false;
        }

        private void cashddlReturn_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                decimal sum = 0.00M;
                for (int i = 0; i < cashddlReturn.Rows.Count; i++)
                {
                    cashddlReturn.Rows[i].Cells[2].Value = Convert.ToDecimal(cashddlReturn.Rows[i].Cells[0].Value) * Convert.ToDecimal(cashddlReturn.Rows[i].Cells[1].Value);
                    sum = sum + Convert.ToDecimal(cashddlReturn.Rows[i].Cells[2].Value);
                    txtReturnAmount.Text = Convert.ToString(sum);
                }
            }
        }

        private void cashddlReturn_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (cashddl.CurrentCell.ColumnIndex == 1)
            {
                if (e.Control is TextBox)
                {
                    TextBox CountReturn = e.Control as TextBox;
                    CountReturn.MaxLength = 8;
                    CountReturn.KeyPress += new KeyPressEventHandler(CountReturn_KeyPress);
                }
            }
        }
        void CountReturn_KeyPress(object sender, KeyPressEventArgs e)
        {
            string headerText = cashddl.Columns[cashddl.CurrentCell.ColumnIndex].HeaderText;
            if (headerText == "Count")
            {
                if (!(char.IsDigit(e.KeyChar)))
                {
                    if (e.KeyChar != '\b')
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void cashddlReturn_KeyDown(object sender, KeyEventArgs e)
        {
            if (cashddlReturn.CurrentCell.RowIndex == cashddlReturn.Rows.Count - 1)
            {
                cashddlReturn.Rows[cashddlReturn.CurrentCell.RowIndex].Cells[1].Selected = false;
                if (e.KeyData == Keys.Enter)
                {
                    btnReturnOK.Focus();
                }
            }
        }

        private void cashplReturn_Paint(object sender, PaintEventArgs e)
        {

        }

        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            
        }

        private void productsearchbttn_Click_1(object sender, EventArgs e)
        {
            GetAccountPayableBalance(); 
            pnlprodsearch.Visible = false;

        }
    }
}
