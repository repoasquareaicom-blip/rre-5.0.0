using InvBal;
using Inventory.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Inventory.Sales;
using Voucher;
using Inventory.Commission;

namespace Inventory.Transactions
{
    public partial class CashRequest : Form
    {
        TransactionBAL ObjTransactionBAL = new TransactionBAL();
        QuotationBal objQuotationbal = new QuotationBal();
        AutoCompleteStringCollection namesCollection = new AutoCompleteStringCollection();
        TextBox tb, tbamount, tbbaalanceanount, tborderquantoty;
        string clickstatus = string.Empty;
        string role1 = string.Empty;
        string transanctionid = string.Empty;
        string commisionId = string.Empty;
        DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
        string amountmode = string.Empty;
        string ReqId = "";
        string NewCashRequestId;
        string AccountHeadID = string.Empty;
        string role = Program.Userrole;
        string UserId = Program.userid;
        string HdnAmount = string.Empty;
        string HdnCashRequestId = string.Empty;
        string AccountHead = string.Empty;
        private RadioButton rbUpi;
        private TextBox txtUpiReference;
        private Panel paymentModeHost;
        private DataGridView dgvBalanceReceived;
        private Label lblCashGivenTitle;
        private Label lblBalanceReceivedTitle;
        private Label lblAmountToPayValue;
        private Label lblCashGivenValue;
        private Label lblBalanceReceivedValue;
        private Label lblNetCashPaidValue;
        private Label lblCashDifferenceValue;
        private Label lblUpiDateValue;
        private bool roseCashEntry;
        private Panel roseUpiPanel;
        private TextBox txtRoseUpiReference;
        private Label lblRoseUpiAmount;
        private Button btnRoseSave;
        string srole = string.Empty;
        string mode = string.Empty;
        public CashRequest()
        {
            try
            {
                InitializeComponent();
                InitializeCompactCashRequestLayout();
                this.WindowState = FormWindowState.Maximized;
                /* Left Side Content */
               

                /* Right Side Content */
                txtLoginBy.Text = Program.UserName;
                BindUsers();
                BindCashRequest();
                txtrequest.Visible = false;
                this.ActiveControl = CmbEmp;
               
                bindacchead();
                bindaccountmainhead();
                this.txtdate.Enabled = false;
               RdEmployee.Checked = false;
                RdOthers.Checked = true;
                a1Panel3.Visible = true;
                rdtransfer.Visible = false;
                //search("ReceiptFrom", "", "EnteredOn", "Today", role, UserId);
             
                bindregferncess();
                comreferenceperson.SelectedIndex = 0;
                //a1Panel5.Visible = false;
                LoadPortsChecking();
                amountmode = "Percentage";
                bindregfernce();
                referenceperson.SelectedIndex = 0;
                bindgrid();
                SearchCreterias1();
                SearchCreterias2();
                SearchCreterias3();
               
                pnlLabelSearch.Visible = true;
                panel18.Visible = true;
                vLabel1.Visible = true;
                vLabel4.Visible = true;
                //panel24.Visible = false;

                splitContainer1.Panel1Collapsed = true;
                splitContainer2.Panel1Collapsed = true;

                //payment Rose
                srole = Program.userid;
                if (srole == "1")
                {
                    role1 = "Admin";
                }
                else
                {
                    role1 = "Emp";
                }
                SearchPurchaseOrder();
                paymentdenobind();
                paymentDenotoCustomerbind();
                bindAccountno();
                paymentDenotoCustomerbindRosePayment();
                ddlpaymode.Enabled = true;
                bindgridss();
                Searchbtn();
            }
            catch (Exception ex)
            {

            }
        }

        /* Left Side Content */

     
        public DataTable bindcheckout()
        {
            DataTable dt = new DataTable();
            dt = objQuotationbal.itemcommisionpayment();
            DataRow dr = dt.NewRow();
            dr["Commissionid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);
            return dt;
        }

      








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
            //txtrequest.AutoCompleteCustomSource = namesCollection;
            //textBox4.AutoCompleteCustomSource = namesCollection;
            //txtsearch2.AutoCompleteCustomSource = namesCollection;
            //textBox4.AutoCompleteCustomSource = namesCollection;

            DataRow dr = dReader.NewRow();
            dr["Name"] = "-select-";
            dReader.Rows.InsertAt(dr, 0);
            CmbEmp.DataSource = dReader;
            CmbEmp.DisplayMember = "Name";
            CmbEmp.ValueMember = "Name";



        }
        public void paymentDenotoCustomerbind()
        {
            dataGridView2.Rows.Clear();
            dataGridView2.ColumnCount = 5;
            dataGridView2.Columns[0].Name = "Denomination";
            dataGridView2.Columns[1].Name = "Count";
            dataGridView2.Columns[2].Name = "Amount";
            dataGridView2.Columns[3].Name = "DenominationID";
            dataGridView2.Columns[4].Name = "unique";


            this.dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView2.Columns[0].Width = 100;
            this.dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridView2.Columns[0].ReadOnly = true;
            this.dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView2.Columns[1].Width = 100;
            this.dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView2.Columns[2].Width = 100;
            this.dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridView2.Columns[2].ReadOnly = true;
            this.dataGridView2.Columns[3].Visible = false;
            this.dataGridView2.Columns[4].Visible = false;

            DataTable payment = new DataTable();
            payment = paymentDenotoCustomer();//dt
            int row = payment.Rows.Count;
            dataGridView2.Rows.Add(row);
            if (row > 0)
            {

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < payment.Columns.Count; j++)
                    {
                        dataGridView2.Rows[i].Cells[j].Value = payment.Rows[i][j];
                    }

                }

                int sum = 0;
                for (int k = 0; k < dataGridView2.Rows.Count; k++)
                {
                    sum = sum + Convert.ToInt32(dataGridView2.Rows[k].Cells[2].Value);
                }
                label66.Text = Convert.ToString(sum);


                dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
                dataGridView2.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
                dataGridView2.DefaultCellStyle.BackColor = Color.Gainsboro;
                dataGridView2.DefaultCellStyle.ForeColor = Color.Black;
                dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


                foreach (DataGridViewColumn column in dataGridView2.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
        }
        public void paymentDenotoCustomerbindRosePayment()
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
        public void bindacchead()
        {
            DataTable dt = ObjTransactionBAL.GetAccountHead();
            DataRow row = dt.NewRow();
            row["AccountHead"] = "-select-";
            dt.Rows.InsertAt(row, 0);
            ddlAccountHead.DataSource = dt;
            ddlAccountHead.DisplayMember = "AccountHead";
            ddlAccountHead.ValueMember = "Id";
        }
        public void bindaccMainhead()
        {
            DataTable dHead = ObjTransactionBAL.GetAccountMainHead();
            DataRow row = dHead.NewRow();
            row["MeanHeadName"] = "-select-";
            dHead.Rows.InsertAt(row, 0);
            comboBox1.DataSource = dHead;
            comboBox1.DisplayMember = "MeanHeadName ";
            comboBox1.ValueMember = "MainHeadId";
        }
        public void BindCashRequest()
        {
            dgvCashRequest.DataSource = null;
            DataTable dtCashRequest = ObjTransactionBAL.BindCashRequest();
            dgvCashRequest.DataSource = dtCashRequest;
            //dgvCashRequest.Columns[1].Visible = false;
            dgvCashRequest.Columns["RequestedBy"].Visible = false;
            dgvCashRequest.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10.1F, FontStyle.Bold);
            dgvCashRequest.DefaultCellStyle.Font = new Font("Tahoma", 12.1F, GraphicsUnit.Pixel);
            dgvCashRequest.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvCashRequest.DefaultCellStyle.ForeColor = Color.Black;
            dgvCashRequest.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


            if (dgvCashRequest.Columns.Contains("Delete") == false)
            {
                DataGridViewImageColumn img = new DataGridViewImageColumn();
                img.ImageLayout = DataGridViewImageCellLayout.Normal;
                object O = Resources.ResourceManager.GetObject("trash");
                Image image = (Image)O;
                img.Image = image;
                img.HeaderText = "Delete";
                img.Name = "Delete";
                dgvCashRequest.Columns.Add(img);
            }
            dgvCashRequest.Columns["Delete"].Visible = false;
            //dgvCashRequest.Columns["Reason"].Width = 200;

        }

        private bool Validate()
        {
            bool status = true;
            string message = "";
            int i = 0;
            if (comboBox1.SelectedIndex == 0)
            {
                i++;
                message = message + "*Please Select AccountMainHeader" + "\n";
                if (i == 1)
                    this.ActiveControl = comboBox1;
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message);
                status = false;
            }
            //    {
            //        i++;
            //        message = message + "*Please Select Request" + "\n";
            //        if (i == 1)
            //            this.ActiveControl = CmbEmp;
            //    }
            return status;
        }
        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;
            //if (RdEmployee.Checked)
            //{
            //    if (CmbEmp.SelectedIndex == 0)
            //    {
            //        i++;
            //        message = message + "*Please Select Request" + "\n";
            //        if (i == 1)
            //            this.ActiveControl = CmbEmp;
            //    }
            //}
            //else
            //{
            //    if (string.IsNullOrEmpty(txtrequest.Text))
            //    {
            //        i++;
            //        message = message + "* Please Enter Request" + "\n";
            //        if (i == 1)
            //            this.ActiveControl = txtrequest;
            //    }
            //}
            if (comboBox1.SelectedIndex == 0)
            {
                i++;
                message = message + "* Please Enter Account Main Head" + "\n";
                if (i == 1)
                    this.ActiveControl = comboBox1;
            }
            if (ddlAccountHead.SelectedIndex == 0)
            {

                i++;
                message = message + "* Please select Account Head" + "\n";
                if (i == 1)
                    this.ActiveControl = ddlAccountHead;
            }
            if (string.IsNullOrEmpty(txtamount.Text))
            {
                i++;
                message = message + "* Please Enter Amount" + "\n";
                if (i == 1)
                    this.ActiveControl = txtamount;
            }
            if (string.IsNullOrEmpty(txtreson.Text))
            {
                i++;
                message = message + "*Please Enter Reason" + "\n";
                if (i == 1)
                    this.ActiveControl = txtreson;
            }
            //if (rdtransfer.Checked == false)
            //{
            //    i++;
            //    message = message + "*Please Check the Denomination" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = rdtransfer;
            //}


            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message);
                status = false;
            }
            return status;
        }
        //private bool ValidationAccHead()
        //{
        //    bool status = true;
        //    string message = "";
        //    int i = 0;


        //    if (string.IsNullOrEmpty(txtacchead.Text.Trim()))
        //    {

        //        i++;
        //        message = message + "* Please Enter Account Head Name" + "\n";
        //        if (i == 1)
        //            this.ActiveControl = txtacchead;
        //    }


        //    if (!string.IsNullOrEmpty(message))
        //    {
        //        MessageBox.Show(message);
        //        status = false;
        //    }
        //    return status;
        //}



        private void btnsave_Click_1(object sender, EventArgs e)
        {
            if (roseCashEntry)
            {
                CompleteRoseCash();
                return;
            }
            if (ValidatePaymentRequest() == false)
            {
                return;
            }
            if (rbCash.Checked)
            {
                if (a1Panel2.Visible == false)
                {
                    ShowCashDenominationPopup();
                    return;
                }
                RefreshCashSummary();
                if (IsCashReconciled() == false)
                {
                    MessageBox.Show("Cash Given minus Balance Received must equal the Amount to Pay");
                    return;
                }
                savetransaction();
                return;
            }
            SaveCashPay();
        }

        private bool ValidatePaymentRequest()
        {
            if (comboBox1.SelectedIndex <= 0)
            {
                MessageBox.Show("Select Account Main Header");
                comboBox1.Focus();
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
                return false;
            }
            decimal amount;
            string amountText = txtamount.Text == null ? "" : txtamount.Text.Trim();
            if (decimal.TryParse(amountText, out amount) == false || amount <= 0m)
            {
                MessageBox.Show("Enter Amount");
                txtamount.Focus();
                return false;
            }
            if (txtreson.Text == null || txtreson.Text.Trim().Length == 0)
            {
                MessageBox.Show("Enter Reason");
                txtreson.Focus();
                return false;
            }
            if (txtreson.Text.Trim().Length > NarrationMaxLength)
            {
                MessageBox.Show("Reason cannot exceed " + NarrationMaxLength.ToString() + " characters.");
                txtreson.Focus();
                return false;
            }
            bool modeSelected = rbCash.Checked || rdcheque.Checked || radioButton1.Checked || (rbUpi != null && rbUpi.Checked);
            if (modeSelected == false)
            {
                MessageBox.Show("Select Payment Mode");
                return false;
            }
            return true;
        }



        private void Clear()
        {
            txtamount.Clear();
            txtrequest.Clear();
            txtreson.Clear();
            var today = DateTime.Today;
            txtdate.Value = today;
            RdEmployee.Checked = false;
            RdOthers.Checked = true;
            CmbEmp.SelectedIndex = 0;
            BindUsers();
            //ddlAccountHead.SelectedIndex = 0;
            dgvCashRequest.Columns["Delete"].Visible = false;
            dataGridView2.Rows[0].Cells[1].Value = 0;
            dataGridView2.Rows[1].Cells[1].Value = 0;
            dataGridView2.Rows[2].Cells[1].Value = 0;
            dataGridView2.Rows[3].Cells[1].Value = 0;
            dataGridView2.Rows[4].Cells[1].Value = 0;
            dataGridView2.Rows[5].Cells[1].Value = 0;
            dataGridView2.Rows[6].Cells[1].Value = 0;
            dataGridView2.Rows[7].Cells[1].Value = 0;
            dataGridView2.Rows[8].Cells[1].Value = 0;
            dataGridView2.Rows[9].Cells[1].Value = 0;
            if (dataGridView2.Rows.Count > 10)
            {
                dataGridView2.Rows[10].Cells[1].Value = 0;
            }
            dataGridView2.Rows[0].Cells[2].Value = 0;
            dataGridView2.Rows[1].Cells[2].Value = 0;
            dataGridView2.Rows[2].Cells[2].Value = 0;
            dataGridView2.Rows[3].Cells[2].Value = 0;
            dataGridView2.Rows[4].Cells[2].Value = 0;
            dataGridView2.Rows[5].Cells[2].Value = 0;
            dataGridView2.Rows[6].Cells[2].Value = 0;
            dataGridView2.Rows[7].Cells[2].Value = 0;
            dataGridView2.Rows[8].Cells[2].Value = 0;
            dataGridView2.Rows[9].Cells[2].Value = 0;
            if (dataGridView2.Rows.Count > 10)
            {
                dataGridView2.Rows[10].Cells[2].Value = 0;
            }

        }

        private void CashRequest_Load(object sender, EventArgs e)
        {
            rbUpi = new RadioButton();
            rbUpi.Name = "rbUpi";
            rbUpi.Text = "UPI";
            rbUpi.AutoSize = true;
            rbUpi.Font = rbCash.Font;
            rbUpi.BackColor = rbCash.BackColor;
            rbUpi.Location = new System.Drawing.Point(100, 244);
            rbUpi.CheckedChanged += new EventHandler(radiobutton_CheckedChanged);
            a1Panel3.Controls.Add(rbUpi);

            txtUpiReference = new TextBox();
            txtUpiReference.Name = "txtUpiReference";
            txtUpiReference.Location = new System.Drawing.Point(160, 242);
            txtUpiReference.Width = 130;
            txtUpiReference.MaxLength = 30;
            txtUpiReference.Visible = false;
            a1Panel3.Controls.Add(txtUpiReference);
            txtUpiReference.BringToFront();
            ArrangePaymentRequestLayout();
            ArrangeRosePayment();
        }

        private void ArrangeRosePayment()
        {
            if (ddlpaymode.SelectedIndex < 0)
            {
                ddlpaymode.SelectedIndex = 0;
            }
            groupBox3.Width = 230;
            ddlpaymode.Width = 150;

            btnRoseSave = new Button();
            btnRoseSave.Name = "btnRoseSave";
            btnRoseSave.Text = "Save Rose Payment";
            btnRoseSave.Font = new Font("Calibri", 11F, FontStyle.Bold);
            btnRoseSave.Size = new Size(180, 32);
            btnRoseSave.Location = new Point(8, 455);
            btnRoseSave.Click += new EventHandler(btnRoseSave_Click);
            panelright.Controls.Add(btnRoseSave);
            btnRoseSave.BringToFront();

            roseUpiPanel = new Panel();
            roseUpiPanel.Name = "roseUpiPanel";
            roseUpiPanel.Location = new Point(8, 495);
            roseUpiPanel.Size = new Size(230, 150);
            roseUpiPanel.Visible = false;
            panelright.Controls.Add(roseUpiPanel);

            Label amountCaption = new Label();
            amountCaption.Text = "Amount to Pay";
            amountCaption.Location = new Point(4, 8);
            amountCaption.AutoSize = true;
            roseUpiPanel.Controls.Add(amountCaption);

            lblRoseUpiAmount = new Label();
            lblRoseUpiAmount.Location = new Point(110, 8);
            lblRoseUpiAmount.AutoSize = true;
            lblRoseUpiAmount.Font = new Font("Calibri", 11F, FontStyle.Bold);
            roseUpiPanel.Controls.Add(lblRoseUpiAmount);

            Label referenceCaption = new Label();
            referenceCaption.Text = "UPI Reference";
            referenceCaption.Location = new Point(4, 36);
            referenceCaption.AutoSize = true;
            roseUpiPanel.Controls.Add(referenceCaption);

            txtRoseUpiReference = new TextBox();
            txtRoseUpiReference.Location = new Point(4, 56);
            txtRoseUpiReference.Width = 210;
            txtRoseUpiReference.MaxLength = 30;
            roseUpiPanel.Controls.Add(txtRoseUpiReference);

            Label dateCaption = new Label();
            dateCaption.Text = "Transaction Date";
            dateCaption.Location = new Point(4, 88);
            dateCaption.AutoSize = true;
            roseUpiPanel.Controls.Add(dateCaption);

            Label dateValue = new Label();
            dateValue.Name = "lblRoseUpiDate";
            dateValue.Location = new Point(120, 88);
            dateValue.AutoSize = true;
            dateValue.Text = DateTime.Today.ToString("dd-MM-yyyy");
            roseUpiPanel.Controls.Add(dateValue);
        }

        private void btnRoseSave_Click(object sender, EventArgs e)
        {
            if (ValidateRosePayment() == false)
            {
                return;
            }
            if (ddlpaymode.SelectedIndex == 1)
            {
                if (roseCashEntry == false || a1Panel2.Visible == false)
                {
                    OpenRoseCashDenomination();
                    return;
                }
                CompleteRoseCash();
                return;
            }
            SaveRoseUpi();
        }

        private bool ValidateRosePayment()
        {
            if (valgrid() == false)
            {
                MessageBox.Show("Select at least one bill");
                return false;
            }
            decimal roseAmount;
            if (decimal.TryParse(lblAmt.Text, out roseAmount) == false || roseAmount <= 0m)
            {
                MessageBox.Show("Enter Rose Amount");
                return false;
            }
            if (ddlpaymode.SelectedIndex <= 0)
            {
                MessageBox.Show("Select Payment Mode");
                return false;
            }
            return true;
        }

        private void OpenRoseCashDenomination()
        {
            roseCashEntry = true;
            cashpl.Visible = false;
            if (roseUpiPanel != null)
            {
                roseUpiPanel.Visible = false;
            }
            a1Panel2.Parent = tabPage2;
            EnsureCashDenominationPopup();
            ResetCashDenomination();
            ShowCashDenominationPopup();
            btnsave.Text = "Save Rose Payment";
        }

        private void CloseRoseCashDenomination()
        {
            roseCashEntry = false;
            ResetCashDenomination();
            a1Panel2.Visible = false;
            if (a1Panel2.Parent != panel2)
            {
                a1Panel2.Parent = panel2;
            }
            PlaceSaveOnForm();
        }

        private void CompleteRoseCash()
        {
            RefreshCashSummary();
            if (IsCashReconciled() == false)
            {
                MessageBox.Show("Cash Given minus Balance Received must equal the Amount to Pay");
                return;
            }
            save();
            if (string.IsNullOrEmpty(lblid.Text) == false)
            {
                MessageBox.Show("Saved Successfully");
            }
            else
            {
                MessageBox.Show("Insertion Failed");
            }
            ResetCashDenomination();
            CloseRoseCashDenomination();
            ddlpaymode.SelectedIndex = 0;
        }

        private void SaveRoseUpi()
        {
            decimal roseAmount;
            decimal.TryParse(lblAmt.Text, out roseAmount);
            if (roseUpiPanel != null)
            {
                roseUpiPanel.Visible = true;
                lblRoseUpiAmount.Text = roseAmount.ToString("0.00");
            }
            string reference;
            if (AccountsChannel.TryUpiReference(txtRoseUpiReference.Text, out reference) == false)
            {
                txtRoseUpiReference.Focus();
                return;
            }
            txtRoseUpiReference.Text = reference;
            saveRoseCommission();
            if (string.IsNullOrEmpty(commisionValue.Text))
            {
                MessageBox.Show("Insertion Failed");
                return;
            }
            string result = AccountsChannel.SaveRoseRequestUpi(commisionValue.Text, roseAmount, reference);
            if (string.IsNullOrEmpty(result) == false)
            {
                MessageBox.Show("Saved Successfully");
                txtRoseUpiReference.Text = "";
                roseUpiPanel.Visible = false;
                ddlpaymode.SelectedIndex = 0;
                ClearRoseEntry();
            }
        }

        private void saveRoseCommission()
        {
            DataTable dt = DataGridView2DataTable();
            objQuotationbal.totalammount = lblBillAmt.Text;
            objQuotationbal.Customerid = label46.Text;
            objQuotationbal.commisionamount = lblAmt.Text;
            objQuotationbal.commisionpercentage = txtPercentage.Text;
            if (radioButton1.Checked)
            {
                objQuotationbal.commisionMode = "Percentage";
            }
            else if (radioButton2.Checked)
            {
                objQuotationbal.commisionMode = "Amount";
            }
            objQuotationbal.Updatedby = Program.userid;
            commisionValue.Text = objQuotationbal.SaveCommision(objQuotationbal, dt);
        }

        private void ClearRoseEntry()
        {
            referenceperson.SelectedIndex = 0;
            txtListDate.Text = string.Empty;
            checkBox1.Checked = false;
            dgvCommissionBill.Rows.Clear();
            label45.Text = string.Empty;
            label46.Text = string.Empty;
            lblBillAmt.Text = string.Empty;
            txtPercentage.Text = string.Empty;
            txtPercentage.ReadOnly = true;
            label44.Text = string.Empty;
            lblAmt.Text = string.Empty;
        }

        private void ArrangePaymentRequestLayout()
        {
            label10.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txtreson.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtreson.Location = new Point(120, 176);
            txtreson.Height = 46;
            txtreson.MaxLength = NarrationMaxLength;
            txtreson.AcceptsTab = false;

            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtLoginBy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnUpload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Text = "D";

            label26.Parent = panel2;
            label26.Text = "Payment Mode";
            label26.AutoSize = true;
            label26.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            paymentModeHost = new Panel();
            paymentModeHost.Name = "paymentModeHost";
            paymentModeHost.Size = new Size(520, 32);
            paymentModeHost.BackColor = Color.WhiteSmoke;
            paymentModeHost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel2.Controls.Add(paymentModeHost);

            rbCash.Parent = paymentModeHost;
            rbCash.Location = new Point(4, 4);
            rbUpi.Parent = paymentModeHost;
            rbUpi.Location = new Point(100, 4);
            radioButton1.Parent = paymentModeHost;
            radioButton1.Location = new Point(190, 4);
            rdcheque.Parent = paymentModeHost;
            rdcheque.Location = new Point(320, 4);

            panel4.Visible = false;
            panel4.Dock = DockStyle.None;
            a1Panel3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            a1Panel3.Visible = false;

            btnsave.Parent = panel2;
            btnsave.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnsave.Font = new Font("Calibri", 12F, FontStyle.Bold);
            btnsave.Size = new Size(220, 34);
            btnsave.Text = "Save Payment Request";
            btnsave.Visible = true;

            dgvCashRequest.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Btnpay.Visible = false;
            Btnpaybalance.Visible = false;
            txtamountrupe.ReadOnly = true;
            panel2.Resize += new EventHandler(panel2_Resize);
            ApplyPaymentRequestTabOrder();
            ApplyPaymentModeLayout();
        }

        private const int NarrationMaxLength = 250;

        private void ApplyPaymentRequestTabOrder()
        {
            comboBox1.TabIndex = 0;
            ddlAccountHead.TabIndex = 1;
            txtdate.TabIndex = 2;
            txtamount.TabIndex = 3;
            txtreson.TabIndex = 4;
            paymentModeHost.TabIndex = 5;
            rbCash.TabIndex = 0;
            rbUpi.TabIndex = 1;
            radioButton1.TabIndex = 2;
            rdcheque.TabIndex = 3;
            a1Panel3.TabIndex = 6;
            txtUpiReference.TabIndex = 0;
            txtamount1.TabIndex = 1;
            txtChqDate.TabIndex = 2;
            txtbankname.TabIndex = 3;
            txtfromaccount.TabIndex = 4;
            txttoaccount.TabIndex = 5;
            btnsave.TabIndex = 7;
            BtnUpload.TabIndex = 8;

            AddMainHeader.TabStop = false;
            btnadd.TabStop = false;
            button2.TabStop = false;
            rdtransfer.TabStop = false;
            txtLoginBy.TabStop = false;
            txtamountrupe.TabStop = false;
            dgvCashRequest.TabStop = false;

            panel15.TabIndex = 0;
            dgvDenomination.TabIndex = 0;
            dgvDenomination.StandardTab = true;
            button3.TabIndex = 20;
        }

        private void EnsurePaymentModeTabStops()
        {
            bool anyChecked = rbCash.Checked || rbUpi.Checked || radioButton1.Checked || rdcheque.Checked;
            if (anyChecked)
            {
                return;
            }
            rbCash.TabStop = true;
            rbUpi.TabStop = true;
            radioButton1.TabStop = true;
            rdcheque.TabStop = true;
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

        private void panel2_Resize(object sender, EventArgs e)
        {
            if (paymentModeHost == null || arrangingLayout)
            {
                return;
            }
            if (a1Panel2.Visible)
            {
                LayoutCashPopup();
            }
            else
            {
                LayoutPaymentSection();
            }
        }

        private bool arrangingLayout;

        private void PlaceSaveOnForm()
        {
            if (btnsave.Parent != panel2)
            {
                btnsave.Parent = panel2;
            }
            btnsave.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnsave.Size = new Size(220, 34);
            btnsave.Text = "Save Payment Request";
            btnsave.Visible = true;
            LayoutPaymentSection();
        }

        private void LayoutPaymentSection()
        {
            CompactPaymentSection();
        }

        private void HideModeFields()
        {
            txtamountrupe.Visible = false;
            txtamount1.Visible = false;
            txtbankname.Visible = false;
            txtChqDate.Visible = false;
            txtfromaccount.Visible = false;
            txttoaccount.Visible = false;
            label25.Visible = false;
            label24.Visible = false;
            label23.Visible = false;
            label19.Visible = false;
            label18.Visible = false;
            if (txtUpiReference != null)
            {
                txtUpiReference.Visible = false;
            }
            if (lblUpiDateValue != null)
            {
                lblUpiDateValue.Visible = false;
            }
            Control chequeTo = a1Panel3.Controls["lblChequeToAccount"];
            if (chequeTo != null)
            {
                chequeTo.Visible = false;
            }
        }

        private void ShowField(Label caption, string text, int y, Control editor)
        {
            caption.Visible = true;
            caption.Text = text;
            caption.Location = new Point(8, y + 4);
            editor.Visible = true;
            editor.Location = new Point(150, y);
            editor.Width = 220;
        }

        private void ApplyPaymentModeLayout()
        {
            HideModeFields();
            if (rbCash.Checked == false)
            {
                a1Panel2.Visible = false;
            }

            if (rbCash.Checked == false && rdcheque.Checked == false && radioButton1.Checked == false && (rbUpi == null || rbUpi.Checked == false))
            {
                a1Panel3.Visible = false;
                PlaceSaveOnForm();
                return;
            }

            if (rbCash.Checked)
            {
                a1Panel3.Visible = false;
                PlaceSaveOnForm();
                return;
            }

            a1Panel3.Visible = true;
            txtamountrupe.Text = txtamount.Text;
            int y = 8;

            if (rbUpi != null && rbUpi.Checked)
            {
                ShowField(label25, "Amount", y, txtamountrupe);
                txtamountrupe.Enabled = true;
                txtamountrupe.ReadOnly = true;
                y = y + 36;
                ShowField(label24, "UPI Reference", y, txtUpiReference);
                txtUpiReference.Enabled = true;
                txtUpiReference.ReadOnly = false;
                txtUpiReference.MaxLength = 30;
                y = y + 36;
                if (lblUpiDateValue == null)
                {
                    lblUpiDateValue = new Label();
                    lblUpiDateValue.Name = "lblUpiDateValue";
                    lblUpiDateValue.AutoSize = false;
                    lblUpiDateValue.Font = txtamountrupe.Font;
                    lblUpiDateValue.Size = new Size(220, 23);
                    a1Panel3.Controls.Add(lblUpiDateValue);
                }
                lblUpiDateValue.Text = txtdate.Value.ToString("dd-MM-yyyy");
                ShowField(label23, "Transaction Date", y, lblUpiDateValue);
                y = y + 36;
            }
            else if (radioButton1.Checked)
            {
                ShowField(label25, "Amount", y, txtamountrupe);
                txtamountrupe.Enabled = true;
                txtamountrupe.ReadOnly = true;
                y = y + 36;
                ShowField(label19, "From Account", y, txtfromaccount);
                txtfromaccount.Enabled = true;
                y = y + 36;
                ShowField(label18, "To Account", y, txttoaccount);
                txttoaccount.Enabled = true;
                y = y + 36;
            }
            else if (rdcheque.Checked)
            {
                ShowField(label25, "Amount", y, txtamountrupe);
                txtamountrupe.Enabled = true;
                txtamountrupe.ReadOnly = true;
                y = y + 36;
                ShowField(label24, "Cheque No", y, txtamount1);
                txtamount1.Enabled = true;
                y = y + 36;
                ShowField(label23, "Cheque Date", y, txtChqDate);
                txtChqDate.Enabled = true;
                y = y + 36;
                label19.Text = "Bank Name";
                ShowField(label19, "Bank Name", y, txtbankname);
                txtbankname.Enabled = true;
                y = y + 36;
                ShowField(label18, "From Account", y, txtfromaccount);
                txtfromaccount.Enabled = true;
                y = y + 36;
                if (lblUpiDateValue == null)
                {
                    lblUpiDateValue = new Label();
                    lblUpiDateValue.AutoSize = true;
                    a1Panel3.Controls.Add(lblUpiDateValue);
                }
                Label toCaption = new Label();
                toCaption.Name = "lblChequeToAccount";
                toCaption.AutoSize = true;
                toCaption.Font = label18.Font;
                Control existing = a1Panel3.Controls["lblChequeToAccount"];
                if (existing == null)
                {
                    a1Panel3.Controls.Add(toCaption);
                }
                else
                {
                    toCaption = (Label)existing;
                }
                ShowField(toCaption, "To Account", y, txttoaccount);
                txttoaccount.Enabled = true;
                y = y + 36;
            }

            a1Panel3.Size = new Size(420, y + 8);
            a1Panel3.BringToFront();
            PlaceSaveOnForm();
        }

        private void ShowCashDenominationPopup()
        {
            EnsureCashDenominationPopup();
            if (dgvDenomination.Rows.Count == 0)
            {
                LoadDenomination();
            }
            if (dgvBalanceReceived.Rows.Count == 0)
            {
                BindBalanceReceived();
            }
            txtamountrupe.Text = txtamount.Text;
            if (!roseCashEntry) a1Panel2.Parent = tabPage1;
            a1Panel2.Visible = true;
            LayoutCashPopup();
            a1Panel2.BringToFront();
            RefreshCashSummary();
            dgvDenomination.Focus();
            if (dgvDenomination.Rows.Count > 0)
            {
                dgvDenomination.CurrentCell = dgvDenomination[1, 0];
            }
        }

        private void LayoutCashPopup()
        {
            CompactCashPopup();
        }

        private void EnsureCashDenominationPopup()
        {
            if (dgvBalanceReceived != null)
            {
                return;
            }
            label143.Text = "Cash Denomination";
            label143.ForeColor = Color.Black;

            lblCashGivenTitle = new Label();
            lblCashGivenTitle.Text = "CASH GIVEN";
            lblCashGivenTitle.Font = new Font("Calibri", 12F, FontStyle.Bold);
            lblCashGivenTitle.Location = new Point(8, 4);
            lblCashGivenTitle.AutoSize = true;
            panel15.Controls.Add(lblCashGivenTitle);

            lblBalanceReceivedTitle = new Label();
            lblBalanceReceivedTitle.Text = "BALANCE RECEIVED";
            lblBalanceReceivedTitle.Font = new Font("Calibri", 12F, FontStyle.Bold);
            lblBalanceReceivedTitle.Location = new Point(430, 4);
            lblBalanceReceivedTitle.AutoSize = true;
            panel15.Controls.Add(lblBalanceReceivedTitle);

            dgvDenomination.Location = new Point(8, 28);
            dgvDenomination.Size = new Size(400, 250);

            dgvBalanceReceived = new DataGridView();
            dgvBalanceReceived.Name = "dgvBalanceReceived";
            dgvBalanceReceived.AllowUserToAddRows = false;
            dgvBalanceReceived.AllowUserToResizeColumns = false;
            dgvBalanceReceived.AllowUserToResizeRows = false;
            dgvBalanceReceived.RowHeadersVisible = false;
            dgvBalanceReceived.Location = new Point(430, 28);
            dgvBalanceReceived.Size = new Size(400, 250);
            dgvBalanceReceived.BackgroundColor = dgvDenomination.BackgroundColor;
            dgvBalanceReceived.CellEndEdit += new DataGridViewCellEventHandler(dgvBalanceReceived_CellEndEdit);
            dgvBalanceReceived.TabIndex = 1;
            dgvBalanceReceived.StandardTab = true;
            panel15.Controls.Add(dgvBalanceReceived);
            BindBalanceReceived();

            lblAmountToPayValue = AddSummaryLabel(panel15, "Amount to Pay : 0.00", 8, 290);
            lblCashGivenValue = AddSummaryLabel(panel15, "Cash Given : 0.00", 8, 318);
            lblBalanceReceivedValue = AddSummaryLabel(panel15, "Balance Received : 0.00", 8, 346);
            lblNetCashPaidValue = AddSummaryLabel(panel15, "Net Cash Paid : 0.00", 430, 290);
            lblCashDifferenceValue = AddSummaryLabel(panel15, "Difference : 0.00", 430, 318);
            label14.Visible = false;
            lblTotalCash.Visible = false;
        }

        private Label AddSummaryLabel(Control parent, string text, int x, int y)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Font = new Font("Calibri", 14F, FontStyle.Bold);
            label.Location = new Point(x, y);
            label.Text = text;
            parent.Controls.Add(label);
            return label;
        }

        private void BindBalanceReceived()
        {
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
            dgvBalanceReceived.DataSource = paymentdo;
            dgvBalanceReceived.Columns[0].ReadOnly = true;
            dgvBalanceReceived.Columns[2].ReadOnly = true;
        }

        private void dgvBalanceReceived_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateDenominationAmounts(dgvBalanceReceived);
            RefreshCashSummary();
        }

        private void UpdateDenominationAmounts(DataGridView grid)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(Convert.ToString(grid.Rows[i].Cells[1].Value)))
                {
                    grid.Rows[i].Cells[1].Value = 0;
                }
                try
                {
                    grid.Rows[i].Cells[2].Value = Convert.ToDecimal(grid.Rows[i].Cells[0].Value) * Convert.ToDecimal(grid.Rows[i].Cells[1].Value);
                }
                catch
                {
                }
            }
        }

        private decimal SumNotes(DataGridView grid)
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

        private void RefreshCashSummary()
        {
            if (lblAmountToPayValue == null)
            {
                return;
            }
            decimal payAmount = DenominationPayAmount();
            decimal given = SumNotes(dgvDenomination);
            decimal received = SumNotes(dgvBalanceReceived);
            decimal net = given - received;
            decimal difference = net - payAmount;
            lblAmountToPayValue.Text = "Amount to Pay : " + payAmount.ToString("0.00");
            lblCashGivenValue.Text = "Cash Given : " + given.ToString("0.00");
            lblBalanceReceivedValue.Text = "Balance Received : " + received.ToString("0.00");
            lblNetCashPaidValue.Text = "Net Cash Paid : " + net.ToString("0.00");
            lblCashDifferenceValue.Text = "Difference : " + difference.ToString("0.00");
            lblCashDifferenceValue.ForeColor = difference == 0m ? Color.Green : Color.Red;
            lblTotalCash.Text = net.ToString("0.00");
        }

        private decimal DenominationPayAmount()
        {
            decimal payAmount;
            string text = roseCashEntry ? lblAmt.Text : txtamount.Text;
            decimal.TryParse(text, out payAmount);
            return payAmount;
        }

        private bool IsCashReconciled()
        {
            decimal payAmount = DenominationPayAmount();
            if (payAmount <= 0m)
            {
                return false;
            }
            decimal net = SumNotes(dgvDenomination) - SumNotes(dgvBalanceReceived);
            return net == payAmount;
        }

        private int NoteQuantity(DataGridView grid, int face)
        {
            int count;
            int.TryParse(AccountsChannel.NoteCount(grid, face), out count);
            if (count < 0)
            {
                count = 0;
            }
            return count;
        }

        private string SignedPaymentCount(int face)
        {
            int given = NoteQuantity(dgvDenomination, face);
            int received = 0;
            if (dgvBalanceReceived != null)
            {
                received = NoteQuantity(dgvBalanceReceived, face);
            }
            return (given - received).ToString();
        }

        private void ResetCashDenomination()
        {
            LoadDenomination();
            if (dgvBalanceReceived != null)
            {
                BindBalanceReceived();
            }
            RefreshCashSummary();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool tabKey = keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift);
            if (tabKey && roseCashEntry == false && a1Panel2.Visible && a1Panel2.ContainsFocus)
            {
                MoveFocusWithin(a1Panel2, keyData == Keys.Tab);
                return true;
            }
            if (tabKey && panel2.ContainsFocus)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            //Cash Request

            if (comboBox1.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = AddMainHeader;
                    return true;
                }

            }

            if (AddMainHeader.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = ddlAccountHead;
                    return true;
                }

            }
            if (ddlAccountHead.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnadd;
                    return true;
                }

            }
            if (btnadd.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = txtamount;
                    return true;
                }

            }


            //if (txtdate.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtamount;
            //        return true;
            //    }

            //}
            if (txtamount.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = txtreson;
                    return true;
                }

            }

            if (txtreson.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = button2;
                    return true;
                }

            }

            if (button2.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = rbCash;
                    return true;
                }

            }

            if (rbCash.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = rdcheque;
                    return true;
                }

            }

            if (rdcheque.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = radioButton1;
                    return true;
                }

            }
            if (radioButton1.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnsave;
                    return true;
                }

            }
            if (txtbankname.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnsave;
                    return true;
                }

            }

            if (txttoaccount.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnsave;
                    return true;
                }

            }

               if (btnsave.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnclear;
                    return true;
                }

            }

            if (btnprint.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = txtrequest;
                    return true;
                }

            }

            

            //comboBox1

            if (txtrequest.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = ddlAccountHead;
                    return true;
                }
            }
            if (CmbEmp.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = ddlAccountHead;
                    return true;
                }

            }
            if (ddlAccountHead.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = txtdate;
                    return true;
                }

            }
            if (BtnUpload.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnsave;
                    return true;
                }

            }
            if (txtListDate.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = ListSearchDate;

                    return true;
                }

            }

            if (ListSearchDate.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnsech;

                    return true;
                }

            }
            if (btnsech.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = dgvCommissionBill;

                    return true;
                }

            }

            if (dgvCommissionBill.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = txtPercentage;

                    return true;
                }

            }

            if (btnupdatecommissionaccountpayable.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnsave;

                    return true;
                }

            }
            if (btnprint.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = referenceperson;

                    return true;
                }

            }

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

                if (dgvCommissionBill.Rows.Count > 0)
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

        /* Left Side Content */



        private void Calender()
        {
            //if (panel25.Visible)
            //{
            //    panel25.Visible = false;
            //}
            //else
            //{
            //    panel25.BringToFront();
            //    panel25.Visible = true;
            //}
        }







   



        public void UploadFile()
        {
            OpenFileDialog opFile = new OpenFileDialog();
            DialogResult result = opFile.ShowDialog();
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            string parent = System.IO.Directory.GetParent(appPath).FullName;
            string parent1 = System.IO.Directory.GetParent(parent).FullName + @"\TransactionImages\";
            if (Directory.Exists(appPath) == false)
            {
                Directory.CreateDirectory(appPath);
            }
            if (result == DialogResult.OK) // Test result.
            {
                try
                {
                    if (!string.IsNullOrEmpty(ReqId))
                    {
                        string iName = opFile.SafeFileName;
                        string filepath = opFile.FileName;
                        string[] dp = iName.Split('.');
                        string path = parent1 + ReqId + "." + dp[1];

                        File.Copy(filepath, path);
                        ReqId = "";
                    }
                    else
                    {
                        MessageBox.Show("Please send request and Upload an image");
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Unable to open file " + exp.Message);
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnprint_Click(object sender, EventArgs e)
        {

        }

        private void RdEmployee_CheckedChanged(object sender, EventArgs e)
        {
            CmbEmp.Visible = true;
            txtrequest.Visible = false;
            this.ActiveControl = CmbEmp;
        }

        private void RdOthers_CheckedChanged(object sender, EventArgs e)
        {

            CmbEmp.Visible = false;
            //  txtrequest.Visible = true;
            txtrequest.Visible = false;
            this.ActiveControl = txtrequest;
        }

        private void ddlAccountHead_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnadd_Click(object sender, EventArgs e)
        {


        }

        private void btnAccheadSave_Click(object sender, EventArgs e)
        {




        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

        }

        private void dgvCashRequest_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtlesspwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void pnlless_Paint(object sender, PaintEventArgs e)
        {

        }

        //private void button2_Click(object sender, EventArgs e)
        //{

        //}

        private void rdtransfer_CheckedChanged(object sender, EventArgs e)
        {
            a1Panel3.Visible = true;

        }
        private void LoadDenomination()
        {
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

            dgvDenomination.AutoGenerateColumns = true;
            dgvDenomination.DataSource = paymentdo;
            dgvDenomination.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
            dgvDenomination.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
            dgvDenomination.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvDenomination.DefaultCellStyle.ForeColor = Color.Black;
            dgvDenomination.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            ConfigureDenominationColumn("Denomination", true, DataGridViewContentAlignment.MiddleLeft);
            ConfigureDenominationColumn("Count", false, DataGridViewContentAlignment.MiddleRight);
            ConfigureDenominationColumn("Amount", true, DataGridViewContentAlignment.MiddleRight);
        }

        private void ConfigureDenominationColumn(string columnName, bool readOnly, DataGridViewContentAlignment alignment)
        {
            if (dgvDenomination.Columns.Contains(columnName) == false)
            {
                return;
            }
            DataGridViewColumn column = dgvDenomination.Columns[columnName];
            if (column == null)
            {
                return;
            }
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            column.ReadOnly = readOnly;
            column.DefaultCellStyle.Alignment = alignment;
        }




        //public void bindcashreceipt()
        //{
        //    TransactionBAL ObjTransactionBAL = new TransactionBAL();
        //    DataTable dtCashRequest = ObjTransactionBAL.BindCashReceipt();
        //    dgvCashRequest.DataSource = null;
        //    dgvCashRequest.Columns.Clear();
        //    dgvCashRequest.DataSource = dtCashRequest;
        //    // dgvCashRequest.Columns[0].Visible = false;

        //    dgvCashRequest.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10.1F, FontStyle.Bold);
        //    dgvCashRequest.DefaultCellStyle.Font = new Font("Tahoma", 12.1F, GraphicsUnit.Pixel);
        //    dgvCashRequest.DefaultCellStyle.BackColor = Color.Gainsboro;
        //    dgvCashRequest.DefaultCellStyle.ForeColor = Color.Black;
        //    dgvCashRequest.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        //    if (dgvCashRequest.Columns.Contains("Delete") == true)
        //    {

        //    }
        //    else
        //    {
        //        DataGridViewImageColumn img = new DataGridViewImageColumn();
        //        img.ImageLayout = DataGridViewImageCellLayout.Normal;
        //        object O = Resources.ResourceManager.GetObject("trash");
        //        Image image = (Image)O;
        //        img.Image = image;
        //        dgvCashRequest.Columns.Add(img);
        //        img.HeaderText = "Delete";
        //        img.Name = "Delete";
        //        img.DisplayIndex = 0;
        //    }
        //    dgvCashRequest.Columns["Delete"].Width = 50;

        //    dgvCashRequest.Columns["Delete"].Visible = false;

        //}
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
        public void bindaccountmainhead()
        {
            TransactionBAL ObjTransactionBAL = new TransactionBAL();
            DataTable dh = ObjTransactionBAL.GetAccountMainHead();
            DataRow row = dh.NewRow();
            row["MeanHeadName"] = "-select-";
            dh.Rows.InsertAt(row, 0);
            comboBox1.DataSource = dh;
            comboBox1.DisplayMember = "MeanHeadName";
            comboBox1.ValueMember = "MainHeadId";
        }
        public void savepayment()
        {

            DataTable dt = new DataTable();
            objQuotationbal.totalammount = lblBillAmt.Text;
            objQuotationbal.Customerid = label46.Text;
            objQuotationbal.commisionamount = lblAmt.Text;
            objQuotationbal.commisionpercentage = txtPercentage.Text;
            if (radioButton1.Checked)
            {
                objQuotationbal.commisionMode = "Percentage";
            }
            else if (radioButton2.Checked)
            {
                objQuotationbal.commisionMode = "Amount";
            }

            objQuotationbal.Updatedby = Program.userid;


            dt = DataGridView2DataTable();


            string output = objQuotationbal.SaveCommision(objQuotationbal, dt);
            commisionValue.Text = output;

            double d = 0.00, balance = 0.00;
            // objQuotationbal.Text = lblhidden.Text;
            objQuotationbal.CommissionRefId = commisionValue.Text;
            objQuotationbal.transid = commisionValue.Text;
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
            //lblpaymentamount.Text = String.Format("{0:00.00}", Convert.ToDouble(lblpaymentbalance.Text));
            objQuotationbal.OAmount = lblpaymentbalance.Text;


            Txtpaid.Text = txtPercentage.Text;
            txtpayamount.Text = lblAmt.Text;
            if (Convert.ToDouble(lblpaymentbalance.Text) > 0.00)
            {
                d = Convert.ToDouble(lblAmt.Text);
                balance = Convert.ToDouble(Txtbalance.Text) - d;
                objQuotationbal.denam = 2;
                objQuotationbal.paid = Convert.ToString(d);
                objQuotationbal.balance = Convert.ToString(balance);
            }
            else
            {

                if (Txtpaid.Text == "")
                {
                    Txtpaid.Text = "0";
                }
                if (txtpayamount.Text == "")
                {
                    txtpayamount.Text = "0";
                }

                d = Convert.ToDouble(lblAmt.Text);
                balance = Convert.ToDouble(lblBillAmt.Text) - d;
                objQuotationbal.paid = Convert.ToString(d);
                objQuotationbal.balance = Convert.ToString(balance);
                objQuotationbal.denam = 1;
            }



            lblid.Text = objQuotationbal.SavecommisionPayment(objQuotationbal, lblpaymenttotal.Text);
        }
        //public void search(string firstname, string firstvalue, string secondname, string secondvalue, string role, string UserId)
        //{
        //    TransactionBAL obj = new TransactionBAL();
        //    DataTable dt = obj.searchcashReceipt(firstname, firstvalue, secondname, secondvalue, role, UserId);
        //    dgvCashRequest.Columns.Clear();
        //    dgvCashRequest.DataSource = dt;

        //    //dvgsearchcashpayment.Columns["Reason"].Visible = false;
        //    //dvgsearchcashpayment.Columns["RequestId"].Visible = false;
        //    //dgvCashRequest.Columns["ReceiptId"].Visible = false;

        //    //dgvCashRequest.Columns["ReceiptFrom"].Width = 175;
        //    //dgvCashRequest.Columns["Date"].Width = 155;

        //    //dgvCashRequest.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //    //dgvCashRequest.DefaultCellStyle.BackColor = Color.Gainsboro;
        //    //dgvCashRequest.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        //    //dgvCashRequest.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //}

        //private void bnpaybalance_Click(object sender, EventArgs e)
        //{
        //    if (Convert.ToDouble(lblTotalCash.Text) > Convert.ToDouble(lblreturn.Text))
        //    {
        //        if (Validation())
        //        {
        //           savepayment();

        //           SavepaymentDenomination();

        //            //bnpaybalance.Visible = false;
        //            dgvreturn.DataSource = null;
        //            LoadreturnDenomination();
        //            lblreturn.Text = "0";
        //            lblTotalCash.Text = "0";
        //            pnreturn.Visible = false;
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please Enter Correct Denomination");
        //    }

        //}
        public void savereturn()
        {
            TransactionBAL objTransactionBAL = new TransactionBAL();
            objTransactionBAL.RequestedBy = txtdate.Text;
            DateTime date = new DateTime(txtdate.Value.Year, txtdate.Value.Month, txtdate.Value.Day);
            if (RdEmployee.Checked)
            {
                objTransactionBAL.Requestedtype = "Employee";
                objTransactionBAL.RequestedBy = CmbEmp.Text;
            }
            else if (RdOthers.Checked)
            {
                objTransactionBAL.Requestedtype = "Others";
                objTransactionBAL.RequestedBy = txtrequest.Text;
            }
            objTransactionBAL.CashRequestedDate = date;
            objTransactionBAL.Amount = txtamount.Text;
            objTransactionBAL.Updatedby = Program.userid;
            objTransactionBAL.Mode = mode;
            objTransactionBAL.Reason = txtreson.Text;
            HdnAmount = txtamount.Text;
            objTransactionBAL.AccountHead = Convert.ToString(ddlAccountHead.Text);
            //AccountHeadID = Convert.ToString(ddlAccountHead.SelectedValue);
            //objTransactionBAL.SaveAccountHead(AccountHeadID, Convert.ToString(ddlAccountHead.Text));
            string CashRptId = ObjTransactionBAL.SaveTransactionCashRequest(objTransactionBAL);
            NewCashRequestId = CashRptId;

            if (!string.IsNullOrEmpty(CashRptId))
            {

                objTransactionBAL.CashRequestId = CashRptId;
                objTransactionBAL.Amount = HdnAmount;
                objTransactionBAL.oamount = lblTotalCash.Text;
                if (rbCash.Checked)
                {
                    mode = "Cash";
                    objTransactionBAL.TwoThousand = AccountsChannel.NoteCount(dgvDenomination, 2000);
                    objTransactionBAL.Thousand = AccountsChannel.NoteCount(dgvDenomination, 1000);
                    objTransactionBAL.FiveHundred = AccountsChannel.NoteCount(dgvDenomination, 500);
                    objTransactionBAL.Hundred = AccountsChannel.NoteCount(dgvDenomination, 100);
                    objTransactionBAL.Fifty = AccountsChannel.NoteCount(dgvDenomination, 50);
                    objTransactionBAL.Twenty = AccountsChannel.NoteCount(dgvDenomination, 20);
                    objTransactionBAL.Ten = AccountsChannel.NoteCount(dgvDenomination, 10);
                    objTransactionBAL.Five = AccountsChannel.NoteCount(dgvDenomination, 5);
                    objTransactionBAL.Coin = AccountsChannel.NoteCount(dgvDenomination, 2);
                    objTransactionBAL.One = AccountsChannel.NoteCount(dgvDenomination, 1);
                    objTransactionBAL.AccountHead = ddlAccountHead.Text;

                    //ObjTranBAL.AccountHead=
                    //ObjTranBAL.ChequeDate = null;
                }
                else if (rdcheque.Checked)
                {
                    mode = "Cheque";
                    objTransactionBAL.ChequeNo = txtamount1.Text;
                    DateTime dates = new DateTime(txtChqDate.Value.Year, txtChqDate.Value.Month, txtChqDate.Value.Day);
                    objTransactionBAL.ChequeDate = dates.ToString("yyyy/MM/dd");
                    objTransactionBAL.BankName = txtbankname.Text;
                    objTransactionBAL.AccountHead = Convert.ToString(ddlAccountHead.Text);
                }
                else if (rdtransfer.Checked)
                {
                    mode = "Transfer";
                    objTransactionBAL.FromAccount = txtfromaccount.Text;
                    objTransactionBAL.ToAccount = txttoaccount.Text;
                    //ObjTranBAL.ChequeDate = null;
                    objTransactionBAL.AccountHead = Convert.ToString(ddlAccountHead.Text);
                }
                objTransactionBAL.Mode = mode;

                objTransactionBAL.Updatedby = Program.userid;

                lbltransid.Text = ObjTransactionBAL.SaveTransactionCashPayment(objTransactionBAL);
                if (mode == "Cash")
                {
                    AccountsChannel.SaveTwoHundred(lbltransid.Text, AccountsChannel.NoteCount(dgvDenomination, 200), null);
                }
                if (!string.IsNullOrEmpty(lbltransid.Text))
                {
                    MessageBox.Show("Save Successfully");


                    a1Panel1.Visible = false;
                    a1Panel2.Visible = false;
                    rbCash.Checked = false;
                    rdcheque.Checked = false;
                    rdtransfer.Checked = false;
                    comboBox1.SelectedIndex = 0;
                    ddlAccountHead.SelectedIndex = 0;
                    txtdate.Text = "";
                    txtamount.Text = "";
                    txtreson.Text = "";
                    txtamountrupe.Text = "";
                    BindCashRequest();
                }


            }
        }
        public void SavepaymentDenominations()
        {

            // Pnloading4.Visible = true;
            objQuotationbal.recivetransid = lbltransid.Text;
            if (AccountsChannel.NoteCount(dataGridView2, 2000) != "0")
            {
                objQuotationbal.recivetwothousand = "-" + AccountsChannel.NoteCount(dataGridView2, 2000);

            }
            else
            {
                objQuotationbal.recivetwothousand = AccountsChannel.NoteCount(dataGridView2, 2000);
            }
            if (AccountsChannel.NoteCount(dataGridView2, 1000) != "0")
            {
                objQuotationbal.recivethousand = "-" + AccountsChannel.NoteCount(dataGridView2, 1000);

            }
            else
            {
                objQuotationbal.recivethousand = AccountsChannel.NoteCount(dataGridView2, 1000);
            }
            if (AccountsChannel.NoteCount(dataGridView2, 500) != "0")
            {
                objQuotationbal.recivefivehundred = "-" + AccountsChannel.NoteCount(dataGridView2, 500);

            }
            else
            {
                objQuotationbal.recivefivehundred = AccountsChannel.NoteCount(dataGridView2, 500);
            }



            if (AccountsChannel.NoteCount(dataGridView2, 100) != "0")
            {
                objQuotationbal.recivehundred = "-" + AccountsChannel.NoteCount(dataGridView2, 100);

            }
            else
            {
                objQuotationbal.recivehundred = AccountsChannel.NoteCount(dataGridView2, 100);
            }


            if (AccountsChannel.NoteCount(dataGridView2, 50) != "0")
            {
                objQuotationbal.recivefifty = "-" + AccountsChannel.NoteCount(dataGridView2, 50);

            }
            else
            {
                objQuotationbal.recivefifty = AccountsChannel.NoteCount(dataGridView2, 50);
            }


            if (AccountsChannel.NoteCount(dataGridView2, 20) != "0")
            {
                objQuotationbal.recivetwenty = "-" + AccountsChannel.NoteCount(dataGridView2, 20);

            }
            else
            {
                objQuotationbal.recivetwenty = AccountsChannel.NoteCount(dataGridView2, 20);
            }

            if (AccountsChannel.NoteCount(dataGridView2, 10) != "0")
            {
                objQuotationbal.reciveten = "-" + AccountsChannel.NoteCount(dataGridView2, 10);

            }
            else
            {
                objQuotationbal.reciveten = AccountsChannel.NoteCount(dataGridView2, 10);
            }


            if (AccountsChannel.NoteCount(dataGridView2, 5) != "0")
            {
                objQuotationbal.recivefive = "-" + AccountsChannel.NoteCount(dataGridView2, 5);

            }
            else
            {
                objQuotationbal.recivefive = AccountsChannel.NoteCount(dataGridView2, 5);
            }

            if (AccountsChannel.NoteCount(dataGridView2, 2) != "0")
            {
                objQuotationbal.recivecoin = "-" + AccountsChannel.NoteCount(dataGridView2, 2);

            }
            else
            {
                objQuotationbal.recivecoin = AccountsChannel.NoteCount(dataGridView2, 2);
            }

            if (AccountsChannel.NoteCount(dataGridView2, 1) != "0")
            {
                objQuotationbal.ReceiveOne = "-" + AccountsChannel.NoteCount(dataGridView2, 1);

            }
            else
            {
                objQuotationbal.ReceiveOne = AccountsChannel.NoteCount(dataGridView2, 1);
            }

            objQuotationbal.OAmount = "-" + label66.Text;

            string output = objQuotationbal.SavepaymentDenomination(objQuotationbal);
            if (output == "1")
            {
                AccountsChannel.SaveTwoHundred(lbltransid.Text, null, AccountsChannel.SignedChangeCount(dataGridView2, 200));
            }

            if (output == "1")
            {
                //Pnloading4.Visible = false;

                // MessageBox.Show("Inserted Successfully");
                BindCashRequest();
                bindaccounthead();
                bindaccountmainhead();
                //clear();
            }
            else
            {
                MessageBox.Show("Insertion Failed");
                //clear();
            }


        }
        private void LoadreturnDenomination()
        {
            try
            {


                // DataTable dtDenom = new DataTable();


                DataTable returnpaymentdo = new DataTable();
                returnpaymentdo.Columns.Add("Denomination", typeof(string));
                returnpaymentdo.Columns.Add("Count", typeof(int));
                returnpaymentdo.Columns.Add("Amount", typeof(decimal));

                //paymentdo.Columns.Add("DenominationID", typeof(int));
                //paymentdo.Columns.Add("unique", typeof(int));
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

                dataGridView2.DataSource = returnpaymentdo;
                this.dataGridView2.Columns[0].ReadOnly = true;


                foreach (DataGridViewColumn column in dataGridView2.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }




                dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
                dataGridView2.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
                dataGridView2.DefaultCellStyle.BackColor = Color.Gainsboro;
                dataGridView2.DefaultCellStyle.ForeColor = Color.Black;
                dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


                this.dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dataGridView2.Columns[0].Width = 150;
                this.dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                this.dataGridView2.Columns[0].ReadOnly = true;
                this.dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dataGridView2.Columns[1].Width = 100;
                this.dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dataGridView2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dataGridView2.Columns[2].Width = 100;
                this.dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dataGridView2.Columns[2].ReadOnly = true;
                this.dataGridView2.Columns[3].Visible = false;
                this.dataGridView2.Columns[4].Visible = false;


                DataTable payment = new DataTable();
                payment = paymentDenotoCustomer();//dt
                int row = payment.Rows.Count;
                dataGridView2.Rows.Add(row);
                if (row > 0)
                {

                    for (int i = 0; i < row; i++)
                    {
                        for (int j = 0; j < payment.Columns.Count; j++)
                        {
                            dataGridView2.Rows[i].Cells[j].Value = payment.Rows[i][j];
                        }

                    }

                    int sum = 0;
                    for (int k = 0; k < dataGridView2.Rows.Count; k++)
                    {
                        sum = sum + Convert.ToInt32(dataGridView2.Rows[k].Cells[2].Value);
                    }
                    lblpaidbalance.Text = Convert.ToString(sum);

                }

            }
            catch
            {

            }
        }

        private void radiobutton_CheckedChanged(object sender, EventArgs e)
        {
            ApplyPaymentModeLayout();
            return;
            btnsave.Visible = true;
            txtamount1.Text = string.Empty;
            txtbankname.Text = string.Empty;
            txtfromaccount.Text = string.Empty;
            txttoaccount.Text = string.Empty;
            var today = DateTime.Today;
            txtChqDate.Value = today;
            lblTotalCash.ForeColor = Color.Black;
            LoadDenomination();


            if (rbCash.Checked)
            {
                btnsave.Visible = false;

                Rectangle resolution = Screen.PrimaryScreen.Bounds;
                int w = resolution.Width;
                int h = resolution.Height;

                if (w == 1024 && h == 768)
                {
                    //button3.Location = new Point(350, 2);
                    //button1.BringToFront();
                    a1Panel2.Width = 350;
                    dgvDenomination.Width = 330;
                    dgvDenomination.Location = new Point(5, 20);
                    a1Panel2.Location = new Point(332, 71);
                    label9.Location = new Point(100, 260);
                    lblTotalCash.Location = new Point(170, 260);
                    //cashpl.Width = 360;
                    //cashddl.Width = 340;
                    //cashpl.Location = new Point(358, 32);
                    //btnreceiveBalance.Location = new Point(180, 352);
                    //btncashpay.Location = new Point(280, 351);
                    //cashdetailsclose.Location = new Point(300, 1);
                    //panelCustomerpaid.Location = new Point(0, 34);
                    //cashddl.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11F, FontStyle.Bold);
                    //cashddl.DefaultCellStyle.Font = new Font("Tahoma", 13F, GraphicsUnit.Pixel);
                    //cashddl.DefaultCellStyle.BackColor = Color.Gainsboro;
                    //cashddl.DefaultCellStyle.ForeColor = Color.Black;
                    //cashddl.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                    //lblcutomerpay.Location = new Point(270, 298);

                    dgvDenomination.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11F, FontStyle.Bold);
                    dgvDenomination.DefaultCellStyle.Font = new Font("Tahoma", 13F, GraphicsUnit.Pixel);
                    dgvDenomination.DefaultCellStyle.BackColor = Color.Gainsboro;
                    dgvDenomination.DefaultCellStyle.ForeColor = Color.Black;
                    dgvDenomination.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                }

                lblTotalCash.Text = "0";
                txtamountrupe.Text = txtamount.Text;
                a1Panel2.Visible = true;
                if (txtUpiReference != null)
                {
                    txtUpiReference.Visible = false;
                }
                txtamount1.Enabled = false;
                txtbankname.Enabled = false;
                txtChqDate.Enabled = false;
                txtfromaccount.Enabled = false;
                txttoaccount.Enabled = false;
                dgvDenomination.Focus();
                dgvDenomination.CurrentCell = dgvDenomination[1, 0];

            }
            else if (rdcheque.Checked)
            {
                txtamountrupe.Text = string.Empty;
                a1Panel2.Visible = false;
                if (txtUpiReference != null)
                {
                    txtUpiReference.Visible = false;
                }
                
                txtamount1.Enabled = true;
                txtbankname.Enabled = true;
                txtChqDate.Enabled = true;
                txtfromaccount.Enabled = false;
                txttoaccount.Enabled = false;
                this.ActiveControl = txtamount1;
                btnsave.Visible = true;
            }
            else if (radioButton1.Checked)
            {
                txtamountrupe.Text = string.Empty;
                a1Panel2.Visible = false;
                txtamount1.Enabled = false;
                txtbankname.Enabled = false;
                txtChqDate.Enabled = false;
                txtfromaccount.Enabled = true;
                txttoaccount.Enabled = true;
                btnsave.Visible = true;
                if (txtUpiReference != null)
                {
                    txtUpiReference.Visible = false;
                }
                this.ActiveControl = txtfromaccount;
            }
            else if (rbUpi != null && rbUpi.Checked)
            {
                txtamountrupe.Text = string.Empty;
                a1Panel2.Visible = false;
                txtamount1.Enabled = false;
                txtbankname.Enabled = false;
                txtChqDate.Enabled = false;
                txtfromaccount.Enabled = false;
                txttoaccount.Enabled = false;
                txtUpiReference.Visible = true;
                btnsave.Visible = true;
                txtUpiReference.Focus();
            }
        }

        private void AddMainHeader_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            comboBox1.SelectedIndex = 0;
            a1Panel4.Visible = true;
            comboBox1.Enabled = false;
            this.ActiveControl = textBox1;
        }

        private void button7_Click(object sender, EventArgs e)
        {




        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void GetSuppliersDetail(int suplierid)
        {
            DataTable dt = PurchaseReturnBAL.GetSuppliersDetails(suplierid);
            DataRow row = dt.NewRow();
            row["AccountHead"] = "-select-";
            dt.Rows.InsertAt(row, 0);

            ddlAccountHead.DataSource = dt;
            ddlAccountHead.ValueMember = "Id";
            ddlAccountHead.DisplayMember = "AccountHead";

            //try
            //{
            //    if (dt.Rows.Count > 0)
            //    {

            //        ddlAccountHead.Text = Convert.ToString(dt.Rows[0]["AccountHead"]);
            //    }
            //    else
            //    {
            //        ddlAccountHead.SelectedItem = null;
            //    }
            //}


            //catch(Exception ex)
            // {

            // }


        }



        private void AddMainHeader_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = "";
            comboBox1.SelectedIndex = 0;
            a1Panel4.Visible = true;
            comboBox1.Enabled = false;
            this.ActiveControl = textBox1;
        }

        private void btnadd_Click_1(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
            {
                txtacchead.Text = "";
                ddlAccountHead.SelectedIndex = 0;
                a1Panel1.Visible = true;
                ddlAccountHead.Enabled = false;
                this.ActiveControl = txtacchead;
            }
            else
            {
                Validate();
            }
        }

        private void Btnpay_Click_1(object sender, EventArgs e)
        {
            btnsave_Click_1(sender, e);
            return;
            if (Validation())
            {
                string res = string.Empty;
                if (txtamount.Text == lblTotalCash.Text)
                {
                    TransactionBAL objTransactionBAL = new TransactionBAL();
                   // objTransactionBAL.RequestedBy = txtdate.Text;
                    DateTime date = new DateTime(txtdate.Value.Year, txtdate.Value.Month, txtdate.Value.Day);
                    if (RdEmployee.Checked)
                    {
                        objTransactionBAL.Requestedtype = "Employee";
                        objTransactionBAL.RequestedBy = CmbEmp.Text;
                    }
                    else if (RdOthers.Checked)
                    {
                        objTransactionBAL.Requestedtype = "Others";
                        objTransactionBAL.RequestedBy = txtrequest.Text;
                    }
                    objTransactionBAL.CashRequestedDate = date;
                    objTransactionBAL.Amount = txtamount.Text;
                    objTransactionBAL.Updatedby = Program.userid;
                    objTransactionBAL.Mode = mode;
                    objTransactionBAL.Reason = txtreson.Text;

                    objTransactionBAL.AccountHead = Convert.ToString(ddlAccountHead.Text);
                    //AccountHeadID = Convert.ToString(ddlAccountHead.SelectedValue);
                    //objTransactionBAL.SaveAccountHead(AccountHeadID, Convert.ToString(ddlAccountHead.Text));
                    string CashRptId = ObjTransactionBAL.SaveTransactionCashRequest(objTransactionBAL);
                    HdnCashRequestId = CashRptId;

                    if (!string.IsNullOrEmpty(CashRptId))
                    {

                        objTransactionBAL.CashRequestId = CashRptId;
                        objTransactionBAL.Amount = txtamount.Text;
                        objTransactionBAL.oamount = lblTotalCash.Text;
                        if (rbCash.Checked)
                        {
                            mode = "Cash";
                            objTransactionBAL.TwoThousand = AccountsChannel.NoteCount(dgvDenomination, 2000);
                            objTransactionBAL.Thousand = AccountsChannel.NoteCount(dgvDenomination, 1000);
                            objTransactionBAL.FiveHundred = AccountsChannel.NoteCount(dgvDenomination, 500);
                            objTransactionBAL.Hundred = AccountsChannel.NoteCount(dgvDenomination, 100);
                            objTransactionBAL.Fifty = AccountsChannel.NoteCount(dgvDenomination, 50);
                            objTransactionBAL.Twenty = AccountsChannel.NoteCount(dgvDenomination, 20);
                            objTransactionBAL.Ten = AccountsChannel.NoteCount(dgvDenomination, 10);
                            objTransactionBAL.Five = AccountsChannel.NoteCount(dgvDenomination, 5);
                            objTransactionBAL.Coin = AccountsChannel.NoteCount(dgvDenomination, 2);
                            objTransactionBAL.One = AccountsChannel.NoteCount(dgvDenomination, 1);
                            objTransactionBAL.AccountHead = ddlAccountHead.Text;

                            //ObjTranBAL.AccountHead=
                            //ObjTranBAL.ChequeDate = null;
                        }
                        else if (rdcheque.Checked)
                        {
                            mode = "Cheque";
                            objTransactionBAL.ChequeNo = txtamount1.Text;
                            DateTime dates = new DateTime(txtChqDate.Value.Year, txtChqDate.Value.Month, txtChqDate.Value.Day);
                            objTransactionBAL.ChequeDate = dates.ToString("yyyy/MM/dd");
                            objTransactionBAL.BankName = txtbankname.Text;
                            objTransactionBAL.AccountHead = null;
                        }
                        else if (rdtransfer.Checked)
                        {
                            mode = "Transfer";
                            objTransactionBAL.FromAccount = txtfromaccount.Text;
                            objTransactionBAL.ToAccount = txttoaccount.Text;
                            //ObjTranBAL.ChequeDate = null;
                            objTransactionBAL.AccountHead = null;
                        }
                        objTransactionBAL.Mode = mode;

                        objTransactionBAL.Updatedby = Program.userid;

                        lbltransid.Text = ObjTransactionBAL.SaveTransactionCashPayment(objTransactionBAL);
                        if (mode == "Cash")
                        {
                            AccountsChannel.SaveTwoHundred(lbltransid.Text, AccountsChannel.NoteCount(dgvDenomination, 200), null);
                        }
                        if (!string.IsNullOrEmpty(lbltransid.Text))
                        {
                            MessageBox.Show("Save Successfully");

                            //HdnCashRequestId = lbltransid.Text;
                            GetReport(CashRptId);
                            paymentDenotoCustomerbind();
                            a1Panel1.Visible = false;
                            a1Panel2.Visible = false;
                            rbCash.Checked = false;
                            rdcheque.Checked = false;
                            rdtransfer.Checked = false;
                            comboBox1.SelectedIndex = 0;
                            ddlAccountHead.SelectedIndex = 0;
                            txtdate.Text = "";
                            txtamount.Text = "";
                            txtreson.Text = "";
                            txtamountrupe.Text = "";
                           
                            BindCashRequest();
                            Clear();
                        }


                    }

                    //search("ReceiptFrom", "", "EnteredOn", "Today", role, UserId);

                }
                else
                {
                    MessageBox.Show("Cash and Denomination is not matched");
                }
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                int res = ObjTransactionBAL.SaveAccountMainHead(textBox1.Text);
                if (res == 1)
                {
                    MessageBox.Show("Inserted Successfully");
                    bindaccMainhead();
                    comboBox1.Text = textBox1.Text;
                    textBox1.Text = "";
                    a1Panel4.Visible = false;
                    comboBox1.Enabled = true;
                    this.ActiveControl = comboBox1;

                }
                else
                {
                    MessageBox.Show("Already Exist");
                    textBox1.Text = "";
                    this.ActiveControl = textBox1;
                }
            }
            else
            {
                MessageBox.Show("Please Enter Account Main Header");
            }

        }

        private void btnAccheadSave_Click_1(object sender, EventArgs e)
        {
            if (txtacchead.Text != "")
            {
                int res = ObjTransactionBAL.SaveAccountHead(txtacchead.Text);
                if (res == 1)
                {
                    MessageBox.Show("Inserted Successfully");
                    bindacchead();
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
            else
            {
                MessageBox.Show("Please Enter Account Head");
            }

        }

        private void picClose_Click_1(object sender, EventArgs e)
        {
            a1Panel1.Visible = false;
            ddlAccountHead.Enabled = true;
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            a1Panel4.Visible = false;
            comboBox1.Enabled = true;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            a1Panel2.Visible = false;
            Btnpay.Enabled = true;
            if (roseCashEntry)
            {
                CloseRoseCashDenomination();
            }
            else
            {
                PlaceSaveOnForm();
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
            {
                GetSuppliersDetail(Convert.ToInt32(comboBox1.SelectedValue));
            }
        }

        private void btnclear_Click_1(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            pnlless.Visible = false;
            txtlesspwd.Clear();
        }

        private void dgvDenomination_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvDenomination_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Tot = 0;
                Double FinVal = CalcualteTotal();
                lblTotalCash.Text = FinVal.ToString() + ".00";
                //if (txtamount.Text != lblTotalCash.Text)
                //{
                //    lblTotalCash.ForeColor = Color.Red;
                //}
                //else
                //{
                //    lblTotalCash.ForeColor = Color.Black;
                //}
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

        private void dgvDenomination_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {
            decimal sum = 0;
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
                    //txtamount.Text = Convert.ToString(sum);
                }
                catch
                {

                }
            }
            RefreshCashSummary();
        }

        private void dgvDenomination_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (dgvDenomination.CurrentCell.RowIndex == dgvDenomination.Rows.Count - 1)
                {
                    dgvDenomination.Rows[9].Cells[1].Selected = false;
                    //lblcutomerpay.Focus();
                }
            }
        }

        private void btnprint_Click_1(object sender, EventArgs e)
        {

        }

        private void txtamount_KeyPress_1(object sender, KeyPressEventArgs e)
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

    

       

     

        private void pbxCollapse_Click_1(object sender, EventArgs e)
        {
            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;
            }
        }

        private void vLabel1_Click_1(object sender, EventArgs e)
        {
            if (pnlLabelSearch.Visible == true)
            {
                pnlLabelSearch.Visible = false;
                vLabel1.Visible = false;
                pnlSearch.Visible = true;
                splitContainer1.Panel1Collapsed = false;
            }
        }

        private void pbxRightCollapse_Click_1(object sender, EventArgs e)
        {
            if (pnlCollapse2.Visible == true)
            {
                pnlCashRequest.Visible = true;
                vLabel2.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;
                //this.dgvSearch.Columns["CustomerID"].Visible = true;
                //this.dgvSearch.Columns["Email"].Visible = true;
                //this.dgvSearch.Columns["Fax"].Visible = true;
            }
        }

        private void vLabel2_Click_1(object sender, EventArgs e)
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

     

        private void textBox4_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox3_Click_1(object sender, EventArgs e)
        {

        }

       

        private void btnSearch_Click_1(object sender, EventArgs e)
        {


            Searchbtn();  
        }


        public void Searchbtn()
        {
            try
            {
                DateTime FromDate = new DateTime(dateTimePicker4.Value.Year, dateTimePicker4.Value.Month, dateTimePicker4.Value.Day);
                DateTime ToDate = new DateTime(dateTimePicker3.Value.Year, dateTimePicker3.Value.Month, dateTimePicker3.Value.Day);

                string RequestedBy = textBox2.Text.Trim();
                string StatusVal = textBox3.Text;
                search(RequestedBy, FromDate, ToDate, StatusVal);

            }
            catch (Exception ex)
            {

            }
            
        }

        public void search(string RequestedBy, DateTime FromDate, DateTime ToDate, string StatusVal)
        {
            TransactionBAL obj = new TransactionBAL();
            DataTable dt = obj.searchcashrequest(RequestedBy, FromDate, ToDate, StatusVal);
            dgvSearch.Columns.Clear();
            dgvSearch.DataSource = dt;

            dgvSearch.Columns["Reason"].Visible = false;
            dgvSearch.Columns["RequestId"].Visible = false;
            dgvSearch.Columns["RequestedBy"].Visible = false;
            dgvSearch.Columns["Date"].Width = 200;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        private void BtnUpload_Click_1(object sender, EventArgs e)
        {
            UploadFile();
        }

        //rese bill
        public Button btn;


        private void SearchCreterias1()
        {
            List<string> search = new List<string>();
            search.Add("References");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            //comboBox7.DataSource = bs.DataSource;
            //comboBox7.SelectedIndex = 0;

        }

        private void SearchCreterias2()
        {
            List<string> search = new List<string>();
            search.Add("References");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            //comboBox6.DataSource = bs.DataSource;
            //comboBox6.SelectedIndex = 1;
        }

        private void SearchCreterias3()
        {
            List<string> search = new List<string>();
            search.Add("References");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            //comboBox5.DataSource = bs.DataSource;
            //comboBox5.SelectedIndex = 2;
        }

        private void LoadPortsChecking()
        {
            dgvCommissionBill.Rows.Clear();
            dgvCommissionBill.ColumnCount = 6;


            dgvCommissionBill.Columns[0].Name = "S.NO";
            dgvCommissionBill.Columns[1].Name = "Estimationid";

            dgvCommissionBill.Columns[2].Name = "Date";
            dgvCommissionBill.Columns[5].Name = "Reference Name";

            dgvCommissionBill.Columns[3].Name = "Amount";
            dgvCommissionBill.Columns[4].Name = "Balance";

            this.dgvCommissionBill.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            this.dgvCommissionBill.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCommissionBill.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCommissionBill.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCommissionBill.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvCommissionBill.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvCommissionBill.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvCommissionBill.Columns[0].ReadOnly = true;
            this.dgvCommissionBill.Columns[1].ReadOnly = true;
            this.dgvCommissionBill.Columns[3].ReadOnly = true;
            this.dgvCommissionBill.Columns[4].ReadOnly = true;
            this.dgvCommissionBill.Columns[5].ReadOnly = true;
            this.dgvCommissionBill.Columns[2].ReadOnly = true;


            this.dgvCommissionBill.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            this.dgvCommissionBill.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "ChkIsCheckOut";
            dgvCmb.HeaderText = "Action";
            dgvCmb.FlatStyle = FlatStyle.Popup;
            dgvCommissionBill.Columns.Insert(6, dgvCmb);



            foreach (DataGridViewColumn c in dgvCommissionBill.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvCommissionBill.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvCommissionBill.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvCommissionBill.AlternatingRowsDefaultCellStyle.BackColor = Color.White;



            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {

                this.dgvCommissionBill.Columns[0].Width = 20;
                this.dgvCommissionBill.Columns[1].Width = 30;
                this.dgvCommissionBill.Columns[2].Width = 30;
                this.dgvCommissionBill.Columns[3].Width = 50;
                this.dgvCommissionBill.Columns[4].Width = 60;
                this.dgvCommissionBill.Columns[5].Width = 60;
                this.dgvCommissionBill.Columns[6].Width = 100;
            }
            else
            {
                this.dgvCommissionBill.Columns[0].Width = 30;
                this.dgvCommissionBill.Columns[1].Width = 80;
                this.dgvCommissionBill.Columns[2].Width = 70;
                this.dgvCommissionBill.Columns[3].Width = 60;
                this.dgvCommissionBill.Columns[4].Width = 90;
                this.dgvCommissionBill.Columns[5].Width = 100;
                this.dgvCommissionBill.Columns[6].Width = 60;
            }
        }
        private void btnupdatecommissionaccountpayable_Click(object sender, EventArgs e)
        {

        }
        public void save()
        {
            DataTable dt = new DataTable();
            objQuotationbal.totalammount = lblBillAmt.Text;
            objQuotationbal.Customerid = label46.Text;
            objQuotationbal.commisionamount = lblAmt.Text;
            objQuotationbal.commisionpercentage = txtPercentage.Text;
            if (radioButton1.Checked)
            {
                objQuotationbal.commisionMode = "Percentage";
            }
            else if (radioButton2.Checked)
            {
                objQuotationbal.commisionMode = "Amount";
            }

            objQuotationbal.Updatedby = Program.userid;


            dt = DataGridView2DataTable();


            string output = objQuotationbal.SaveCommision(objQuotationbal, dt);
            commisionValue.Text = output;

            objQuotationbal.CommissionRefId = commisionValue.Text;
            objQuotationbal.transid = commisionValue.Text;
            objQuotationbal.paidtwothousand = SignedPaymentCount(2000);
            objQuotationbal.paidthousand = SignedPaymentCount(1000);
            objQuotationbal.paidfivehundred = SignedPaymentCount(500);
            objQuotationbal.paidhundred = SignedPaymentCount(100);
            objQuotationbal.paidfifty = SignedPaymentCount(50);
            objQuotationbal.paidtwenty = SignedPaymentCount(20);
            objQuotationbal.paidten = SignedPaymentCount(10);
            objQuotationbal.paidfive = SignedPaymentCount(5);
            objQuotationbal.paidcoin = SignedPaymentCount(2);
            objQuotationbal.PaidOne = SignedPaymentCount(1);
            objQuotationbal.OAmount = lblAmt.Text;
            objQuotationbal.paid = lblAmt.Text;
            objQuotationbal.balance = "0";
            objQuotationbal.denam = 1;

            lblid.Text = objQuotationbal.SavecommisionPayment(objQuotationbal, lblAmt.Text);
            AccountsChannel.SaveTwoHundred(lblid.Text, SignedPaymentCount(200), null);
            GetReportRosePrint(commisionValue.Text);
            referenceperson.SelectedIndex = 0;
            txtListDate.Text = string.Empty;
            checkBox1.Checked = false;
            dgvCommissionBill.Rows.Clear();

            label45.Text = string.Empty;
            label46.Text = string.Empty;
            lblBillAmt.Text = string.Empty;
            //symbol.Text = string.Empty;
            txtPercentage.Text = string.Empty;
            txtPercentage.ReadOnly = true;
            label44.Text = string.Empty;
            lblAmt.Text = string.Empty;
            pictureBox1.Image = Inventory.Properties.Resources.images1;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

        }

        public bool valgrid()
        {
            bool vas = false;
            foreach (DataGridViewRow dataGridRow in dgvCommissionBill.Rows)
            {
                if (dataGridRow.Cells["ChkIsCheckOut"].Value != null && (bool)dataGridRow.Cells["ChkIsCheckOut"].Value)
                {
                    vas = true;
                }

            }
            return vas;
        }

        public DataTable DataGridView2DataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[2] { new DataColumn("Estimationid", typeof(string)),
                            new DataColumn("Amount", typeof(string))
                            });
            foreach (DataGridViewRow dataGridRow in dgvCommissionBill.Rows)
            {
                if (dataGridRow.Cells["ChkIsCheckOut"].Value != null && (bool)dataGridRow.Cells["ChkIsCheckOut"].Value)
                {
                    dt.Rows.Add(Convert.ToString(dataGridRow.Cells["Estimationid"].Value), Convert.ToString(dataGridRow.Cells["Amount"].Value));
                }

            }
            return dt;

        }
        public void clears()
        {
            referenceperson.SelectedIndex = 0;
            txtListDate.Text = string.Empty;
            checkBox1.Checked = false;
            dgvCommissionBill.Rows.Clear();

            label45.Text = string.Empty;
            label46.Text = string.Empty;
            lblBillAmt.Text = string.Empty;
            //symbol.Text = string.Empty;
            txtPercentage.Text = string.Empty;
            txtPercentage.ReadOnly = true;
            cashpl.Visible = false;
            lblpaymenttotal.Text = "0";
            lblpaymentamount.Text = "0";
            lblpaymentbalance.Text = "0";
            cashddl.Rows[0].Cells[1].Value = 0;
            cashddl.Rows[1].Cells[1].Value = 0;
            cashddl.Rows[2].Cells[1].Value = 0;
            cashddl.Rows[3].Cells[1].Value = 0;
            cashddl.Rows[4].Cells[1].Value = 0;
            cashddl.Rows[5].Cells[1].Value = 0;
            cashddl.Rows[6].Cells[1].Value = 0;
            cashddl.Rows[7].Cells[1].Value = 0;
            cashddl.Rows[8].Cells[1].Value = 0;
            cashddl.Rows[9].Cells[1].Value = 0;

            cashddl.Rows[0].Cells[2].Value = 0;
            cashddl.Rows[1].Cells[2].Value = 0;
            cashddl.Rows[2].Cells[2].Value = 0;
            cashddl.Rows[3].Cells[2].Value = 0;
            cashddl.Rows[4].Cells[2].Value = 0;
            cashddl.Rows[5].Cells[2].Value = 0;
            cashddl.Rows[6].Cells[2].Value = 0;
            cashddl.Rows[7].Cells[2].Value = 0;
            cashddl.Rows[8].Cells[2].Value = 0;
            cashddl.Rows[9].Cells[2].Value = 0;


            dgvCustomerpaid.Rows[0].Cells[1].Value = 0;
            dgvCustomerpaid.Rows[1].Cells[1].Value = 0;
            dgvCustomerpaid.Rows[2].Cells[1].Value = 0;
            dgvCustomerpaid.Rows[3].Cells[1].Value = 0;
            dgvCustomerpaid.Rows[4].Cells[1].Value = 0;
            dgvCustomerpaid.Rows[5].Cells[1].Value = 0;
            dgvCustomerpaid.Rows[6].Cells[1].Value = 0;
            dgvCustomerpaid.Rows[7].Cells[1].Value = 0;
            dgvCustomerpaid.Rows[8].Cells[1].Value = 0;
            dgvCustomerpaid.Rows[9].Cells[1].Value = 0;

            dgvCustomerpaid.Rows[0].Cells[2].Value = 0;
            dgvCustomerpaid.Rows[1].Cells[2].Value = 0;
            dgvCustomerpaid.Rows[2].Cells[2].Value = 0;
            dgvCustomerpaid.Rows[3].Cells[2].Value = 0;
            dgvCustomerpaid.Rows[4].Cells[2].Value = 0;
            dgvCustomerpaid.Rows[5].Cells[2].Value = 0;
            dgvCustomerpaid.Rows[6].Cells[2].Value = 0;
            dgvCustomerpaid.Rows[7].Cells[2].Value = 0;
            dgvCustomerpaid.Rows[8].Cells[2].Value = 0;
            dgvCustomerpaid.Rows[9].Cells[2].Value = 0;
            panelCustomerpaid.Visible = false;
            lblpaidbalance.Text = "0.00";
            pictureBox1.Image = Inventory.Properties.Resources.images1;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        public void bindregfernce()
        {
            DataTable dt = ReferenceBAL.GetReference();
            referenceperson.DataSource = dt;
            referenceperson.DisplayMember = "Name";
            referenceperson.ValueMember = "ReferencesID";
            DataRow dr = dt.NewRow();
            dr["Name"] = "-Select-";
            dr["ReferencesID"] = "0";
            dt.Rows.InsertAt(dr, 0);
        }
        public void bindgrid()
        {
            string s = string.Empty;
            if (referenceperson.SelectedIndex > 0)
            {
                s = Convert.ToString(referenceperson.SelectedValue);
            }
            else
            {
                s = "";
            }
            DataSet ds = objQuotationbal.GetTable(txtListDate.Text, s);


            if (dgvCommissionBill.Rows.Count > 0)
            {
                dgvCommissionBill.Rows.Clear();
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                dgvCommissionBill.Rows.Add();
                dgvCommissionBill.Rows[i].Cells[0].Value = i + 1;
                dgvCommissionBill.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[0].Rows[i]["Estimationid"]);
                dgvCommissionBill.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[0].Rows[i]["Date"]);
                dgvCommissionBill.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[0].Rows[i]["Amount"]);
                dgvCommissionBill.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[0].Rows[i]["Balance"]);
                dgvCommissionBill.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[0].Rows[i]["Reference Name"]);


                //if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[i]["Balance"])))
                //{
                //    Txtbalance.Text = Convert.ToString(ds.Tables[0].Rows[i]["Balance"]);
                //}
                //else
                //{
                //    Txtbalance.Text = string.Empty;
                //}
                try
                {
                    if (Convert.ToDouble(dgvCommissionBill.Rows[i].Cells[4].Value) > 0)
                    {
                        DataGridViewRow cRow = dgvCommissionBill.Rows[i];
                        cRow.DefaultCellStyle.BackColor = Color.Orange;
                    }
                }
                catch (Exception exp)
                {

                }


            }



            if (ds.Tables[1].Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[0]["Name"])))
                {
                    label46.Text = Convert.ToString(ds.Tables[1].Rows[0]["Name"]);
                }
                else
                {
                    label46.Text = string.Empty;
                }
                //if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[0]["Balance"])))
                //{
                //    Txtbalance.Text = Convert.ToString(ds.Tables[1].Rows[0]["Balance"]);
                //}
                //else
                //{
                //    Txtbalance.Text = string.Empty;
                //}
                //if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[0]["Amount"])))
                //{
                //    Txtpaid.Text = Convert.ToString(ds.Tables[1].Rows[0]["Amount"]);
                //}
                //else
                //{
                //    Txtpaid.Text = string.Empty;
                //}
                if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[0]["Phone"])))
                {
                    label45.Text = Convert.ToString(ds.Tables[1].Rows[0]["Phone"]);
                }
                else
                {
                    label45.Text = string.Empty;
                }

                if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[0]["path"])))
                {
                    pictureBox1.ImageLocation = Convert.ToString(ds.Tables[1].Rows[0]["path"]);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else
                {
                    pictureBox1.Image = Inventory.Properties.Resources.images1;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            else
            {
                label46.Text = string.Empty;
                label45.Text = string.Empty;
                pictureBox1.Image = Inventory.Properties.Resources.images1;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }


        }

        private void btnproceed_Click(object sender, EventArgs e)
        {
            GetData();
        }


       public void GetData()
        {
            double val = 0.0;
            int vas = 0;
            double value = 0.0;
            int valBalance = 0;





            foreach (DataGridViewRow dataGridRow in dgvCommissionBill.Rows)
            {
                if (dataGridRow.Cells["ChkIsCheckOut"].Value != null && (bool)dataGridRow.Cells["ChkIsCheckOut"].Value)
                {
                    val += Convert.ToDouble(dataGridRow.Cells["Amount"].Value);
                    vas = vas + 1;

                    value += Convert.ToDouble(dataGridRow.Cells["Balance"].Value);
                    valBalance = valBalance + 1;




                }


            }
            if (vas == 0)
            {
                MessageBox.Show("Please Check Item");
                txtPercentage.ReadOnly = true;
                lblBillAmt.Text = Convert.ToString(0.00);
                Txtbalance.Text = Convert.ToString(0.00);
                label44.Text = Convert.ToString(0.00);

                //symbol.Text = Convert.ToString(0.00);
            }
            else
            {

                txtPercentage.ReadOnly = false;
                lblBillAmt.Text = String.Format("{0:00.00}", Convert.ToString(val));
                Txtbalance.Text = String.Format("{0:00.00}", Convert.ToString(value));
                label44.Text = String.Format("{0:00.00}", Convert.ToString(vas));



                //symbol.Text = Convert.ToString(vas);
            }
            ddlpaymode.Enabled = true;
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            amountmode = "Percentage";
            geamount();
        }
        public void geamount()
        {
            if (!string.IsNullOrEmpty(txtPercentage.Text.Trim()))
            {
                double P = Convert.ToDouble(txtPercentage.Text.Trim());


                if (amountmode == "Percentage")
                {
                    if (P > 100)
                    {
                        MessageBox.Show("Percentage should not be greater than 100");
                        txtPercentage.Text = "";
                        lblAmt.Text = "0";
                        txtPercentage.Focus();
                        return;
                    }
                    double BillAmt = Convert.ToDouble(lblBillAmt.Text.Trim());
                    double Amt = Convert.ToDouble((P / 100) * BillAmt);
                   double Amount= Math.Round(Amt);
                   lblAmt.Text = Convert.ToString(Amount);
                }
                else if (amountmode == "Net")
                {
                    if (P > Convert.ToDouble(lblBillAmt.Text.Trim()))
                    {
                        MessageBox.Show("Amount should not be greater than Bill Amount");
                        txtPercentage.Text = "";
                        lblAmt.Text = "0";
                        txtPercentage.Focus();
                        return;
                    }
                    int BillAmt = Convert.ToInt32(lblBillAmt.Text.Trim());
                    //decimal Amt = BillAmt - Convert.ToDecimal((P));
                    //lblAmt.Text = Convert.ToString(Amt);
                    lblAmt.Text = Convert.ToString((P));
                }
            }
            else
            {
                lblAmt.Text = string.Empty;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            amountmode = "Net";
            geamount();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (dgvCommissionBill.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvCommissionBill.Rows)
                {
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[6];
                    chk.Value = !(chk.Value == null ? false : (bool)chk.Value); //because chk.Value is initialy null

                }
            }
            else
            {

                checkBox1.Checked = false;

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string message = "Please Select Atleast One Person";
            int gridval = valid();
            if (gridval <= 0)
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);

            }
        }
        public int valid()
        {
            int val = 0;

            foreach (DataGridViewRow row in dgvCommissionBill.Rows)
            {
                if (Convert.ToBoolean(row.Cells[0].Value))
                {
                    val = val + 1;
                }

            }
            return val;
        }

        private void btnsech_Click(object sender, EventArgs e)
        {
            bool Val = Validation_Rose();
            if (Val == true)
            {
                bindgrid();
            }
        }
        public bool Validation_Rose()
        {
            bool status = true;
            string message = "";
            int i = 0;
            if (!string.IsNullOrEmpty(txtListDate.Text))
            {
                if (referenceperson.SelectedIndex == 0)
                {
                    message += "* Please Select Reference Person" + "\n";
                    i++;
                    //if (i == 1)
                    //{
                    //this.ActiveControl = txtListDate;

                    // }
                }
            }
            if (string.IsNullOrEmpty(message))
            {
                status = true;
            }
            else
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

        private void radioButton3_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtPercentage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
    && !char.IsDigit(e.KeyChar)
    && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            // only allow one decimal point
            if (e.KeyChar == '.'
                && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtPercentage_TextChanged(object sender, EventArgs e)
        {
            try
            {
                geamount();
            }
            catch
            {

            }
        }

        private void revious_click(object sender, EventArgs e)
        {
            if (panel24.Visible == true)
            {
                panel18.Visible = true;
                vLabel4.Visible = true;
                panel24.Visible = false;
                splitContainer2.Panel1Collapsed = true;
            }
        }

        private void ListSearchDate_Click(object sender, EventArgs e)
        {
            if (lblcommPanel.Visible == true)
            {
                lblcommPanel.Visible = false;
            }
            else
            {
                lblcommPanel.Visible = true;
                lblcommPanel.BringToFront();
            }
        }

        private void lblcommall_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommall.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommtoday_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommtoday.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommthisweek_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommthisweek.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommthismonth_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommthismonth.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommthisyear_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommthisyear.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommyest_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommyest.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommlastweek_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommlastweek.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommlastmonth_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommlastmonth.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommlastyear_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommlastyear.Text.Trim();
            lblcommPanel.Visible = false;
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

        public void totalvalbalancerose()
        {
            double totalamount = 0;
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                totalamount = totalamount + Convert.ToDouble(dataGridView2.Rows[i].Cells[2].Value);
            }

            label66.Text = String.Format("{0:00.00}", totalamount);


        }
        private void cashddl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (cashddl.CurrentCell.RowIndex == 9)
                {
                    cashddl.Rows[9].Cells[1].Selected = false;
                    btnreceiveBalance.Focus();
                }
            }
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
                string message = string.Empty;

                bool das = valgrid();


                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                }
                else
                {
                    //a1Panel5.Visible = true;
                    save();

                    // savepayments();



                    ddlpaymode.Enabled = false;
                    MessageBox.Show("Saved Successfully");
                    cashpl.Visible = false;
                    paymentdenobind();
                    bindgridss();

                    //    }

                }
            }
        }
        public void savepayments()
        {


        }

        public void SavepaymentDenominationRose()
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

                //clears();
            }

        }

        public void savecard()
        {

            //Pnloading4.Visible = true;
            //objQuotationbal.Quotationid = lblhidden.Text;
            objQuotationbal.Bank = cmbbank.Text;
            objQuotationbal.Cardnumber = txtcardno.Text;
            objQuotationbal.transid = txttransactionid.Text;

            objQuotationbal.OAmount = Convert.ToString(lblcardammount.Text);
            string s = objQuotationbal.savecardCommission(objQuotationbal);

            if (s == "1")
            {
                //Pnloading4.Visible = false;

            }

        }

        public void bindAccountno()
        {
            cmbbank.DataSource = objQuotationbal.getaccount();
            cmbbank.DisplayMember = "accountno";
            cmbbank.ValueMember = "accountno";
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
        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void dgvCustomerpaid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (dgvCustomerpaid.CurrentCell.RowIndex == 9)
                {
                    dgvCustomerpaid.Rows[9].Cells[1].Selected = false;
                    lblcutomerpay.Focus();
                }
            }
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

        private void ddlpaymode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlpaymode.SelectedIndex == 1)
            {
                cashpl.Visible = false;
                panelTransaction.Visible = false;
                if (roseUpiPanel != null)
                {
                    roseUpiPanel.Visible = false;
                }
                return;
            }
            if (ddlpaymode.SelectedIndex == 1)
            {
                GetData();

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
                if (txtPercentage.Text != "")
                {
                    if (lblAmt.Text == "")
                    {
                        lblAmt.Text = "0";
                    }
                    lblpaymentamount.Text = String.Format("{0:00.00}", Convert.ToDouble(lblAmt.Text));
                    cashpl.Visible = true;
                    panelTransaction.Visible = false;
                    this.ActiveControl = cashddl;
                    Application.Idle += new EventHandler(Application_Idle);
                }
                else
                {
                    //MessageBox.Show("Please Select the Reference Person");
                    ddlpaymode.SelectedIndex = 0;
                }
            }
            else if (ddlpaymode.SelectedIndex == 2)
            {
                cashpl.Visible = false;
                panelTransaction.Visible = false;
                decimal roseAmount;
                decimal.TryParse(lblAmt.Text, out roseAmount);
                if (roseAmount <= 0m)
                {
                    MessageBox.Show("Enter Rose Amount");
                    ddlpaymode.SelectedIndex = 0;
                    return;
                }
                roseUpiPanel.Visible = true;
                lblRoseUpiAmount.Text = roseAmount.ToString("0.00");
                txtRoseUpiReference.Focus();
            }
            else
            {
                panelCustomerpaid.Visible = false;
                panelTransaction.Visible = false;
                cashpl.Visible = false;
                if (roseUpiPanel != null)
                {
                    roseUpiPanel.Visible = false;
                }
            }
        }

        private void cashdetailsclose_Click(object sender, EventArgs e)
        {
            cashpl.Visible = false;
            paymentdenobind();
            lblpaymenttotal.Text="0";
            lblpaymentamount.Text="0";
            lblpaymentbalance.Text = "0";
           
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (comboBox7.SelectedIndex == 2)
            //{
            //    comboBox4.Visible = true;
            //    textBox4.Visible = false;
            //    button12.Visible = false;

            //}
            //else if (comboBox7.SelectedIndex == 1)
            //{
            //    comboBox4.Visible = false;
            //    textBox4.Visible = true;
            //    textBox4.Text = "Today";
            //    button12.Visible = true;
            //}
            //else
            //{
            //    comboBox4.Visible = false;
            //    textBox4.Visible = true;
            //    textBox4.Text = string.Empty;
            //    button12.Visible = false;
            //}
            //panel25.Visible = false;
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (comboBox6.SelectedIndex == 2)
            //{
            //    comboBox3.Visible = true;
            //    textBox3.Visible = false;
            //    button13.Visible = false;

            //}
            //else if (comboBox6.SelectedIndex == 1)
            //{
            //    comboBox3.Visible = false;
            //    textBox3.Visible = true;
            //    textBox3.Text = "Today";
            //    button13.Visible = true;
            //}
            //else
            //{
            //    comboBox3.Visible = false;
            //    textBox3.Visible = true;
            //    textBox3.Text = string.Empty;
            //    button13.Visible = false;
            //}

            //panel25.Visible = false;
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (comboBox5.SelectedIndex == 2)
            //{
            //    comboBox2.Visible = true;
            //    textBox2.Visible = false;
            //    button14.Visible = false;
            //}
            //else if (comboBox5.SelectedIndex == 1)
            //{
            //    comboBox2.Visible = false;
            //    textBox2.Visible = true;
            //    textBox2.Text = "Today";
            //    button14.Visible = true;
            //}
            //else
            //{
            //    comboBox2.Visible = false;
            //    textBox2.Visible = true;
            //    textBox2.Text = string.Empty;
            //    button14.Visible = false;
            //}

            //panel25.Visible = false;
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            //string selecteditem = comboBox7.Text.ToString();
            //if (selecteditem == "Date")
            //{
            //    clickstatus = "search1";
            //    panel25.BringToFront();
            //    panel25.Visible = true;
            //    panel25.Location = new Point(133, 54);
            //}
            //else
            //{
            //    panel25.Visible = false;
            //}
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            //if (comboBox6.Text.ToString() == "Date")
            //{
            //    clickstatus = "search2";
            //    panel25.BringToFront();
            //    panel25.Visible = true;
            //    panel25.Location = new Point(133, 79);
            //}
            //else
            //{
            //    panel25.Visible = false;
            //}
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            //if (comboBox5.Text.ToString() == "Date")
            //{
            //    clickstatus = "search3";
            //    panel25.BringToFront();
            //    panel25.Visible = true;
            //    panel25.Location = new Point(133, 103);
            //}
            //else
            //{
            //    panel25.Visible = false;

            //}
        }

        private void button15_Click(object sender, EventArgs e)
        {
           // panel34.Visible=false;
            panelCustomerpaid.Visible = false;
            //dgvCustomerpaid.Columns.Clear();
            paymentDenotoCustomerbindRosePayment();
            lblpaidbalance.Text = "0.00";
            btncashpay.Enabled = true;
        }

        private void btnreceiveBalance_Click(object sender, EventArgs e)
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

               
                //dgvCustomerpaid.CurrentCell = dgvCustomerpaid[1, 0];
            }
            //cashpl.Visible = false;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            clickstatus = "search2";
            Calender();
           // panel25.Location = new Point(133, 79);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            clickstatus = "search1";
            Calender();
            //panel25.Location = new Point(133, 54);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            clickstatus = "search3";
            Calender();
           // panel25.Location = new Point(133, 103);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            txtlesspwd.Clear();
            pnlless.Visible = true;
            txtlesspwd.Focus();
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtlesspwd.Clear();
            pnlless.Visible = true;
            txtlesspwd.Focus();
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            referenceperson.SelectedIndex = 0;
            txtListDate.Text = string.Empty;
            checkBox1.Checked = false;
            dgvCommissionBill.Rows.Clear();
            label44.Text = string.Empty;
            label45.Text = string.Empty;
            label46.Text = string.Empty;
            lblBillAmt.Text = string.Empty;
            //symbol.Text = string.Empty;
            txtPercentage.Text = string.Empty;
            txtPercentage.ReadOnly = true;

            pictureBox1.Image = Inventory.Properties.Resources.images1;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void Btnpaybalance_Click(object sender, EventArgs e)
        {
             
            try
            {
                if (Validation())
                {
                    if (Convert.ToDouble(lblTotalCash.Text) == 0.00)
                    {
                        MessageBox.Show("Please Enter Amount ");
                    }
                    else if (Convert.ToDouble(lblTotalCash.Text) <= 0.00)
                    {
                        //MessageBox.Show("Please Enter Amount ");
                    }
                    else if (Convert.ToDouble(lblTotalCash.Text) <= Convert.ToDouble(txtamountrupe.Text))
                    {
                        //MessageBox.Show("Please Enter Amount ");
                    }

                    else
                    {

                        panel32.Visible = true;
                        panel33.Visible = true;
                        Btnpay.Enabled = false;
                        dataGridView2.Focus();
                        //a1Panel1.SendToBack();
                        panelCustomerpaid.BringToFront();
                        dataGridView2.CurrentCell = dataGridView2[1, 0];
                    }
                }

            }
            catch(Exception ex)
            {

            }
           
        }

        private void button18_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dataGridView2.CurrentCell.ColumnIndex;
            string headerText = dataGridView2.Columns[column].HeaderText;

            if (headerText.Equals("Count"))
            {
                tbbaalanceanount = e.Control as TextBox;


                if (tbbaalanceanount != null)
                {

                    tbbaalanceanount.KeyPress += new KeyPressEventHandler(textbox2_keypress);
                }
            }
        }
        private void textbox2_keypress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (dataGridView2.CurrentCell.RowIndex == dataGridView2.Rows.Count - 1)
                {
                    dataGridView2.Rows[9].Cells[1].Selected = false;
                    lblcutomerpay.Focus();
                }
            }
        }
        //public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        //{
        //    TransactionBAL obj = new TransactionBAL();
        //    DataTable dt = obj.searchcashrequest(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role, UserId);
        //    dgvSearch.Columns.Clear();
        //    dgvSearch.DataSource = dt;

        //    dgvSearch.Columns["Reason"].Visible = false;
        //    dgvSearch.Columns["RequestId"].Visible = false;
        //    dgvSearch.Columns["RequestedBy"].Width = 175;
        //    dgvSearch.Columns["Date"].Width = 175;

        //    dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //    dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
        //    dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        //    dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //}
        private void button19_Click(object sender, EventArgs e)
        {
            panel32.Visible = false;
        }

        private void button11_Click(object sender, EventArgs e)
        {


            bindgridss();
               
        }

        public void bindregferncess()
        {
            DataTable dt = ReferenceBAL.GetReference();
            comreferenceperson.DataSource = dt;
            comreferenceperson.DisplayMember = "Name";
            comreferenceperson.ValueMember = "ReferencesID";
            DataRow dr = dt.NewRow();
            dr["Name"] = "-Select-";
            dr["ReferencesID"] = "0";
            dt.Rows.InsertAt(dr, 0);
        }

        private void SearchPurchaseOrder()
        {

            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.ColumnCount = 6;


            dataGridView1.Columns[0].Name = "Order No";
            dataGridView1.Columns[1].Name = "referenceid";
            dataGridView1.Columns[2].Name = "Phone";
            dataGridView1.Columns[3].Name = "CommissionAmount";
            dataGridView1.Columns[4].Name = "Totalbalance";
            dataGridView1.Columns[5].Name = "Updatedon";




            this.dataGridView1.Columns[0].Width = 100;

            this.dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dataGridView1.Columns[1].Width = 60;




            this.dataGridView1.Columns[2].Width = 60;






            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            this.dataGridView1.Columns[1].Visible = false;
            this.dataGridView1.Columns[2].Visible = false;
            this.dataGridView1.Columns[3].Visible = false;
            this.dataGridView1.Columns[4].Visible = false;
            this.dataGridView1.Columns[5].Visible = false;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.BackColor = Color.Gainsboro;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        }
        private void bindgridss()
        {
            string s = string.Empty;
            if (comreferenceperson.SelectedIndex > 0)
            {
                s = Convert.ToString(comreferenceperson.Text);
            }
            else
            {
                s = "";
            }
            DateTime Fromdates = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
            DateTime Todates = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);
            DataSet ds = objQuotationbal.GetTables(Fromdates, Todates, s);

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows.Clear();
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {


                dataGridView1.Rows.Add();
               
                dataGridView1.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[0].Rows[i]["referenceid"]);
                dataGridView1.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[0].Rows[i]["Phone"]);
                dataGridView1.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[0].Rows[i]["CommissionAmount"]);
                dataGridView1.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[0].Rows[i]["Totalbalance"]);

                dataGridView1.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[0].Rows[i]["Updatedon"]);
                dataGridView1.Rows[i].Cells[0].Value = Convert.ToString(ds.Tables[0].Rows[i]["Commissionid"]);

                //dgvCommissionBill.Rows[i].Cells[8].Value = dgvCommissionBill.Columns.Add(btn);
                //btn.Text = "Delete";
            }
            ////DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
            //dgvCommissionBill.Columns.Add(btn);
            //btn.HeaderText = "";
            //btn.Text = "Delete";
            //btn.Name = "btn";
            //btn.UseColumnTextForButtonValue = true;
        }

        //public void searchpay(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        //{
        //    DataTable dt = objQuotationbal.searchCommissionpatyment(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
        //    dataGridView1.Rows.Clear();
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        dataGridView1.Rows.Add();
        //        dataGridView1.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
        //        dataGridView1.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
        //        dataGridView1.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
        //    }


        //    //dgvSearch.Columns["Assist"].Visible = false;
        //    //dgvSearch.Columns["Referenceid"].Visible = false;
        //    //dgvSearch.Columns["customername"].Width = 175;
        //    //dgvSearch.Columns["date"].Width = 175;

        //    //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //    //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
        //    //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        //    //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        //}

    

      
        private void label63_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    textBox4.Text = label63.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    textBox3.Text = label63.Text.Trim();
            //}
            //else if (clickstatus == "search3")
            //{
            //    textBox2.Text = label63.Text.Trim();
            //}

            //panel25.Visible = false;
        }

        private void label64_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    textBox4.Text = label64.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    textBox3.Text = label64.Text.Trim();
            //}
            //else if (clickstatus == "search3")
            //{
            //    textBox2.Text = label64.Text.Trim();
            //}
            //panel25.Visible = false;
        }

        private void label62_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    textBox4.Text = label62.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    textBox3.Text = label62.Text.Trim();
            //}
            //else if (clickstatus == "search3")
            //{
            //    textBox2.Text = label62.Text.Trim();
            //}

            ////txtListDate.Text = lblThisWeek.Text.Trim();
            //panel25.Visible = false;
        }

        private void label58_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    textBox4.Text = label58.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    textBox3.Text = label58.Text.Trim();
            //}
            //else if (clickstatus == "search3")
            //{
            //    textBox2.Text = label58.Text.Trim();
            //}

            ////txtListDate.Text = lblThisMonth.Text.Trim();
            //panel25.Visible = false;
        }

        private void label57_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    textBox4.Text = label57.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    textBox3.Text = label57.Text.Trim();
            //}
            //else if (clickstatus == "search3")
            //{
            //    textBox2.Text = label57.Text.Trim();
            //}

            ////txtListDate.Text = lblThisYear.Text.Trim();
            //panel25.Visible = false;
        }

        private void label56_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    textBox4.Text = label56.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    textBox3.Text = label56.Text.Trim();
            //}
            //else if (clickstatus == "search3")
            //{
            //    textBox2.Text = label56.Text.Trim();
            //}

            ////txtListDate.Text = lblYesterday.Text.Trim();
            //panel25.Visible = false;
        }

        private void label55_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    textBox4.Text = label55.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    textBox3.Text = label55.Text.Trim();
            //}
            //else if (clickstatus == "search3")
            //{
            //    textBox2.Text = label55.Text.Trim();
            //}

            ////txtListDate.Text = lblLastWeek.Text.Trim();
            //panel25.Visible = false;
        }

        private void label54_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    textBox4.Text = label54.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    textBox3.Text = label54.Text.Trim();
            //}
            //else if (clickstatus == "search3")
            //{
            //    textBox2.Text = label54.Text.Trim();
            //}

            ////txtListDate.Text = lblLastMonth.Text.Trim();
            //panel25.Visible = false;
        }

        private void label53_Click(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    textBox4.Text = label53.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    textBox3.Text = label53.Text.Trim();
            //}
            //else if (clickstatus == "search3")
            //{
            //    textBox2.Text = label53.Text.Trim();
            //}

            ////txtListDate.Text = lblLastYear.Text.Trim();
            //panel25.Visible = false;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            //if (clickstatus == "search1")
            //{
            //    textBox4.Text = dateTimePicker1.Text.Trim();
            //}
            //else if (clickstatus == "search2")
            //{
            //    textBox3.Text = dateTimePicker1.Text.Trim();
            //}
            //else if (clickstatus == "search3")
            //{
            //    textBox2.Text = dateTimePicker1.Text.Trim();
            //}
            ////txtListDate.Text = lblLastMonth.Text.Trim();
            //panel25.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pnlless.Visible = false;
        }

        private void SavePaymentRequestUpi(string upiReference)
        {
            TransactionBAL objTransactionBAL = new TransactionBAL();
            DateTime date = new DateTime(txtdate.Value.Year, txtdate.Value.Month, txtdate.Value.Day);
            if (RdEmployee.Checked)
            {
                objTransactionBAL.Requestedtype = "Employee";
                objTransactionBAL.RequestedBy = CmbEmp.Text;
            }
            else if (RdOthers.Checked)
            {
                objTransactionBAL.Requestedtype = "Others";
                objTransactionBAL.RequestedBy = txtrequest.Text;
            }
            objTransactionBAL.CashRequestedDate = date;
            objTransactionBAL.Amount = txtamount.Text;
            objTransactionBAL.Updatedby = Program.userid;
            objTransactionBAL.Mode = "UPI";
            objTransactionBAL.Reason = txtreson.Text;
            objTransactionBAL.AccountHead = Convert.ToString(ddlAccountHead.Text);
            string cashRequestId = ObjTransactionBAL.SaveTransactionCashRequest(objTransactionBAL);
            if (string.IsNullOrEmpty(cashRequestId))
            {
                MessageBox.Show("Insertion Failed");
                return;
            }
            int paidBy = 0;
            int.TryParse(Program.userid, out paidBy);
            string result = AccountsChannel.SavePaymentRequestUpi(cashRequestId, Convert.ToDecimal(txtamount.Text), upiReference, paidBy, Convert.ToString(ddlAccountHead.Text));
            if (!string.IsNullOrEmpty(result))
            {
                MessageBox.Show("Save Successfully");
                GetReport(cashRequestId);
                a1Panel1.Visible = false;
                a1Panel2.Visible = false;
                rbCash.Checked = false;
                rdcheque.Checked = false;
                rdtransfer.Checked = false;
                radioButton1.Checked = false;
                rbUpi.Checked = false;
                txtUpiReference.Text = "";
                txtUpiReference.Visible = false;
                PlaceSaveOnForm();
                BindCashRequest();
            }
        }

        public void SaveCashPay()
        {
            if (rdcheque.Checked)
            {
                if (txtamount1.Text != "" && txtbankname.Text != "")
                {
                    savetransaction();
                }
                else
                {
                    MessageBox.Show("Please Fill ChequeNo & BankName");
                }

            }
            else if (radioButton1.Checked)
            {
                if (txtfromaccount.Text != "" && txttoaccount.Text != "")
                {
                    savetransaction();
                }

                else
                {
                    MessageBox.Show("Please fill FromAccount & ToAccount");
                }
            }
            else if (rbUpi != null && rbUpi.Checked)
            {
                string upiReference;
                if (AccountsChannel.TryUpiReference(txtUpiReference.Text, out upiReference))
                {
                    txtUpiReference.Text = upiReference;
                    SavePaymentRequestUpi(upiReference);
                }
            }
        }

        public void savetransaction()
        {
            TransactionBAL objTransactionBAL = new TransactionBAL();
           // objTransactionBAL.RequestedBy = txtdate.Text;
            DateTime date = new DateTime(txtdate.Value.Year, txtdate.Value.Month, txtdate.Value.Day);
            if (RdEmployee.Checked)
            {
                objTransactionBAL.Requestedtype = "Employee";
                objTransactionBAL.RequestedBy = CmbEmp.Text;
            }
            else if (RdOthers.Checked)
            {
                objTransactionBAL.Requestedtype = "Others";
                objTransactionBAL.RequestedBy = txtrequest.Text;
            }
            objTransactionBAL.CashRequestedDate = date;
            objTransactionBAL.Amount = txtamount.Text;
            objTransactionBAL.Updatedby = Program.userid;
            if (rbCash.Checked)
            {
                mode = "Cash";
            }
            else if (rdcheque.Checked)
            {
                mode = "Cheque";
            }
            else if (radioButton1.Checked)
            {
                mode = "Transfer";
            }
            objTransactionBAL.Mode = mode;
            objTransactionBAL.Reason = txtreson.Text;

            objTransactionBAL.AccountHead = Convert.ToString(ddlAccountHead.Text);
            //AccountHeadID = Convert.ToString(ddlAccountHead.SelectedValue);
            //objTransactionBAL.SaveAccountHead(AccountHeadID, Convert.ToString(ddlAccountHead.Text));
            string CashRptId = ObjTransactionBAL.SaveTransactionCashRequest(objTransactionBAL);

            HdnCashRequestId = CashRptId;
            if (!string.IsNullOrEmpty(CashRptId))
            {

                objTransactionBAL.CashRequestId = CashRptId;
                objTransactionBAL.Amount = txtamount.Text;
                objTransactionBAL.oamount = lblTotalCash.Text;
                if (rbCash.Checked)
                {
                    mode = "Cash";
                    decimal netPaid = SumNotes(dgvDenomination) - SumNotes(dgvBalanceReceived);
                    objTransactionBAL.Amount = txtamount.Text;
                    objTransactionBAL.oamount = netPaid.ToString("0.00");
                    objTransactionBAL.TwoThousand = SignedPaymentCount(2000);
                    objTransactionBAL.Thousand = SignedPaymentCount(1000);
                    objTransactionBAL.FiveHundred = SignedPaymentCount(500);
                    objTransactionBAL.Hundred = SignedPaymentCount(100);
                    objTransactionBAL.Fifty = SignedPaymentCount(50);
                    objTransactionBAL.Twenty = SignedPaymentCount(20);
                    objTransactionBAL.Ten = SignedPaymentCount(10);
                    objTransactionBAL.Five = SignedPaymentCount(5);
                    objTransactionBAL.Coin = SignedPaymentCount(2);
                    objTransactionBAL.One = SignedPaymentCount(1);
                    objTransactionBAL.AccountHead = Convert.ToString(ddlAccountHead.Text);

                    //ObjTranBAL.AccountHead=
                    //ObjTranBAL.ChequeDate = null;
                }
                else if (rdcheque.Checked)
                {
                    mode = "Cheque";
                    objTransactionBAL.ChequeNo = txtamount1.Text;
                    DateTime dates = new DateTime(txtChqDate.Value.Year, txtChqDate.Value.Month, txtChqDate.Value.Day);
                    objTransactionBAL.ChequeDate = dates.ToString("yyyy/MM/dd");
                    objTransactionBAL.BankName = txtbankname.Text;
                    if (txtfromaccount.Text != null && txtfromaccount.Text.Trim().Length > 0)
                    {
                        objTransactionBAL.FromAccount = txtfromaccount.Text.Trim();
                    }
                    if (txttoaccount.Text != null && txttoaccount.Text.Trim().Length > 0)
                    {
                        objTransactionBAL.ToAccount = txttoaccount.Text.Trim();
                    }
                    objTransactionBAL.AccountHead = Convert.ToString(ddlAccountHead.Text);
                }
                else if (radioButton1.Checked)
                {
                    mode = "Transfer";
                    objTransactionBAL.FromAccount = txtfromaccount.Text;
                    objTransactionBAL.ToAccount = txttoaccount.Text;
                    //ObjTranBAL.ChequeDate = null;
                    objTransactionBAL.AccountHead = Convert.ToString(ddlAccountHead.Text);
                }
                objTransactionBAL.Mode = mode;

                objTransactionBAL.Updatedby = Program.userid;

                lbltransid.Text = ObjTransactionBAL.SaveTransactionCashPayment(objTransactionBAL);
                if (mode == "Cash")
                {
                    AccountsChannel.SaveTwoHundred(lbltransid.Text, SignedPaymentCount(200), null);
                }
                if (!string.IsNullOrEmpty(lbltransid.Text))
                {
                    MessageBox.Show("Save Successfully");
                    //HdnCashRequestId = lbltransid.Text;
                    GetReport(HdnCashRequestId);
                    a1Panel1.Visible = false;
                    a1Panel2.Visible = false;
                    ResetCashDenomination();
                    PlaceSaveOnForm();
                    rbCash.Checked = false;
                    rdcheque.Checked = false;
                    rdtransfer.Checked = false;
                    radioButton1.Checked = false;
                    if (rbUpi != null)
                    {
                        rbUpi.Checked = false;
                    }
                    txtfromaccount.Text = "";
                    txttoaccount.Text = "";
                    comboBox1.SelectedIndex = 0;
                    ddlAccountHead.SelectedIndex = 0;
                    txtdate.Text = "";
                    txtamount.Text = "";
                    txtreson.Text = "";
                    txtamountrupe.Text = "";
                    BindCashRequest();
                }


            }



        }

        private void button18_Click_1(object sender, EventArgs e)
        {

            if (Validation())
            {
                double vals = Convert.ToDouble(lblTotalCash.Text) - Convert.ToDouble(txtamountrupe.Text);
                if (vals != Convert.ToDouble(label66.Text))
                {
                    MessageBox.Show("Please Enter Correct Balance");
                    dataGridView2.Focus();
                    dataGridView2.CurrentCell = dataGridView2[1, dataGridView2.CurrentCell.RowIndex];
                }
                else
                {
                    Btnpay.Enabled = true;

                    savereturn();
                    SavepaymentDenominations();
                    GetReport(NewCashRequestId);
                    paymentDenotoCustomerbind();
                    a1Panel1.Visible = false;
                    panel32.Visible = false;
                    panelCustomerpaid.Visible = false;
                    Clear();


                }
            }
        }

        private void dataGridView2_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {
            decimal sum = 0;
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dataGridView2.Rows[i].Cells[1].Value)))
                {
                    dataGridView2.Rows[i].Cells[1].Value = 0;
                }
                try
                {
                    dataGridView2.Rows[i].Cells[2].Value = Convert.ToDecimal(dataGridView2.Rows[i].Cells[0].Value) * Convert.ToDecimal(dataGridView2.Rows[i].Cells[1].Value);
                    sum = sum + Convert.ToDecimal(dataGridView2.Rows[i].Cells[2].Value);
                    label66.Text = Convert.ToString(sum);
                    //txtamount.Text = Convert.ToString(sum);
                }
                catch
                {

                }
            }
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        public void GetReport(string QuotationId)
        {
            //try
            //{
            if (!string.IsNullOrEmpty(QuotationId))
            {
                DialogResult result = MessageBox.Show("Do you want to Print Voucher " + QuotationId + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Quotationreport rpt = new Quotationreport(txtorder.Text);
                    //rpt.ShowDialog();

                    using (SqlConnection con = new SqlConnection(Program.connection))
                    {
                        DataSet ds = new DataSet();
                        con.Open();


                        SqlCommand cmd = new SqlCommand("GetCashRequestById", con);
                        cmd.Parameters.AddWithValue("@RequestId", QuotationId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);



                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            try
                            {

                                VoucherReport objRREPrint = new VoucherReport();
                                objRREPrint.dsMain = ds;
                                objRREPrint.pagenumber = 1;
                                objRREPrint.status = true;
                                objRREPrint._strRefText = "Voch:";
                                objRREPrint._strRef = QuotationId;

                                objRREPrint.RREPrintReuest();
                            }

                            catch (Exception ex)
                            {

                            }
                        }


                        //System.Diagnostics.Process myProc = new System.Diagnostics.Process();
                        //myProc.StartInfo.FileName = "type " + Obj.fileName + " >prn";  //Attempting to start a non-existing executable
                        //myProc.Start();    //Start the application and assign it to the process component.    
                        //ExecuteCommandSync("type " + Obj.fileName + " >prn");



                    }
                }
            }


            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message.ToString());
            //}
        }
        public void GetReportRosePrint(string QuotationId)
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
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            try
                            {

                                RoseBillReport objRREPrint = new RoseBillReport();
                                objRREPrint.dsMain = ds;
                                objRREPrint.pagenumber = 1;
                                objRREPrint.status = true;
                                objRREPrint._strRefText = "Ro:";
                                objRREPrint._strRef = QuotationId;

                                objRREPrint.RREPrintRoseBill();
                            }

                            catch (Exception ex)
                            {

                            }
                        }

                      



                    }
                }
            }


        }

        private void dgvCashRequest_CellClick_1(object sender, DataGridViewCellEventArgs e)
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

                            txtlesspwd.Text = string.Empty;
                            pnlless.Visible = false;
                            string sa = Convert.ToString(dgvCashRequest.Rows[dgvCashRequest.CurrentCell.RowIndex].Cells["RequestId"].Value);
                            int v = ObjTransactionBAL.deletecashrequest(sa);
                            if (v == 1)
                            {
                                MessageBox.Show("Deleted Successfully");
                                BindCashRequest();
                            }
                            else if (v == 2)
                            {
                                MessageBox.Show("Can't Delete");
                                Clear();
                            }


                        }
                    }
                }
                if (dgvCashRequest.Columns[e.ColumnIndex].HeaderText == "RequestId")
                {
                    string RequestId = dgvCashRequest.Rows[e.RowIndex].Cells["RequestId"].Value.ToString();
                    GetReport(RequestId);
                }
            }
           catch(Exception ex)
            {

            }
        }

        private void btnLogin_Click_2(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtlesspwd.Text))
            {
                MessageBox.Show("Password should not be empty");
                this.ActiveControl = txtlesspwd;
                return;
            }
            DataTable dt = LoginBAL.GetLesserDetials(txtlesspwd.Text, "CASHREQUEST");
            if (dt.Rows.Count > 0)
            {


                dgvCashRequest.Columns["Delete"].Visible = true;
                pnlless.Visible = false;
                txtlesspwd.Text = string.Empty;

            }
            else
            {
                MessageBox.Show("Authentication Failed");
                txtlesspwd.Focus();

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
                SavepaymentDenominationRose();
                string hdnCommission = lblhidden.Text;
                GetReport(hdnCommission);
                clears();
                ddlpaymode.Enabled = false;
                bindgridss();
                //searchpay("Commissionid", "", "Updatedon", "Today", "Referenceid", "", role1, Program.userid);
            }
        }

        private void button19_Click_1(object sender, EventArgs e)
        {
            panel32.Visible = false;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            panel32.Visible = false;
              panel33.Visible = true;
            label66.Text = "0.00";
            Btnpay.Enabled = true;
            dataGridView2.Columns.Clear();
            paymentDenotoCustomerbind();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
         
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void vLabel4_Click(object sender, EventArgs e)
        {
            if (panel18.Visible == true)
            {
                panel18.Visible = false;
                vLabel4.Visible = false;
                panel24.Visible = true;
                splitContainer2.Panel1Collapsed = false;
            }
        }

        //public void paymentdenobinds()
        //{
        //    dataGridView2.Rows.Clear();
        //    dataGridView2.ColumnCount = 5;
        //    dataGridView2.Columns[0].Name = "Denomination";
        //    dataGridView2.Columns[1].Name = "Count";
        //    dataGridView2.Columns[2].Name = "Amount";
        //    dataGridView2.Columns[3].Name = "DenominationID";
        //    dataGridView2.Columns[4].Name = "unique";


        //    this.dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        //    this.dataGridView2.Columns[0].Width = 150;
        //    this.dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        //    this.dataGridView2.Columns[0].ReadOnly = true;
        //    this.dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        //    this.dataGridView2.Columns[1].Width = 100;
        //    this.dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //    this.dataGridView2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        //    this.dataGridView2.Columns[2].Width = 100;
        //    this.dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //    this.dataGridView2.Columns[2].ReadOnly = true;
        //    this.dataGridView2.Columns[3].Visible = false;
        //    this.dataGridView2.Columns[4].Visible = false;

        //    DataTable payment = new DataTable();
        //    payment = paymentDeno();
        //    int row = payment.Rows.Count;
        //    dataGridView2.Rows.Add(row);
        //    if (row > 0)
        //    {

        //        for (int i = 0; i < row; i++)
        //        {
        //            for (int j = 0; j < payment.Columns.Count; j++)
        //            {
        //                dataGridView2.Rows[i].Cells[j].Value = payment.Rows[i][j];
        //            }

        //        }



        //    }



        //    cashddl.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
        //    cashddl.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
        //    cashddl.DefaultCellStyle.BackColor = Color.Gainsboro;
        //    cashddl.DefaultCellStyle.ForeColor = Color.Black;
        //    cashddl.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


        //}

    }
}
