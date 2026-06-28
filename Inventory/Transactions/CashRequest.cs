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
        string srole = string.Empty;
        string mode = string.Empty;
        public CashRequest()
        {
            try
            {
                InitializeComponent();
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
            if (Validation())
            {
                SaveCashPay();
            }

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

        }

        private void CashRequest_Load(object sender, EventArgs e)
        {
            //this.ActiveControl = txtrequest;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
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
            catch (Exception ex)
            {

            }
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
            if (Convert.ToString(dataGridView2.Rows[0].Cells[1].Value) != "0")
            {
                objQuotationbal.recivetwothousand = "-" + Convert.ToString(dataGridView2.Rows[0].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivetwothousand = Convert.ToString(dataGridView2.Rows[0].Cells[1].Value);
            }
            if (Convert.ToString(dataGridView2.Rows[1].Cells[1].Value) != "0")
            {
                objQuotationbal.recivethousand = "-" + Convert.ToString(dataGridView2.Rows[1].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivethousand = Convert.ToString(dataGridView2.Rows[1].Cells[1].Value);
            }
            if (Convert.ToString(dataGridView2.Rows[2].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefivehundred = "-" + Convert.ToString(dataGridView2.Rows[2].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefivehundred = Convert.ToString(dataGridView2.Rows[2].Cells[1].Value);
            }



            if (Convert.ToString(dataGridView2.Rows[3].Cells[1].Value) != "0")
            {
                objQuotationbal.recivehundred = "-" + Convert.ToString(dataGridView2.Rows[3].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivehundred = Convert.ToString(dataGridView2.Rows[3].Cells[1].Value);
            }


            if (Convert.ToString(dataGridView2.Rows[4].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefifty = "-" + Convert.ToString(dataGridView2.Rows[4].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefifty = Convert.ToString(dataGridView2.Rows[4].Cells[1].Value);
            }


            if (Convert.ToString(dataGridView2.Rows[5].Cells[1].Value) != "0")
            {
                objQuotationbal.recivetwenty = "-" + Convert.ToString(dataGridView2.Rows[5].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivetwenty = Convert.ToString(dataGridView2.Rows[5].Cells[1].Value);
            }

            if (Convert.ToString(dataGridView2.Rows[6].Cells[1].Value) != "0")
            {
                objQuotationbal.reciveten = "-" + Convert.ToString(dataGridView2.Rows[6].Cells[1].Value);

            }
            else
            {
                objQuotationbal.reciveten = Convert.ToString(dataGridView2.Rows[6].Cells[1].Value);
            }


            if (Convert.ToString(dataGridView2.Rows[7].Cells[1].Value) != "0")
            {
                objQuotationbal.recivefive = "-" + Convert.ToString(dataGridView2.Rows[7].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivefive = Convert.ToString(dataGridView2.Rows[7].Cells[1].Value);
            }

            if (Convert.ToString(dataGridView2.Rows[8].Cells[1].Value) != "0")
            {
                objQuotationbal.recivecoin = "-" + Convert.ToString(dataGridView2.Rows[8].Cells[1].Value);

            }
            else
            {
                objQuotationbal.recivecoin = Convert.ToString(dataGridView2.Rows[8].Cells[1].Value);
            }

            if (Convert.ToString(dataGridView2.Rows[9].Cells[1].Value) != "0")
            {
                objQuotationbal.ReceiveOne = "-" + Convert.ToString(dataGridView2.Rows[9].Cells[1].Value);

            }
            else
            {
                objQuotationbal.ReceiveOne = Convert.ToString(dataGridView2.Rows[9].Cells[1].Value);
            }

            objQuotationbal.OAmount = "-" + label66.Text;

            string output = objQuotationbal.SavepaymentDenomination(objQuotationbal);

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
                this.ActiveControl = txtfromaccount;
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
        }

        private void dgvDenomination_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (dgvDenomination.CurrentCell.RowIndex == 9)
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
                GetData();

                cashpl.Visible = false;
                panelTransaction.Visible = true;
                lblChequeNo.Visible = true;
                txttransactionid.Visible = true;
                lblcardammount.Text = String.Format("{0:00.00}", Convert.ToDouble(lblAmt.Text));
                cmbbank.Focus();
            }
            else
            {
                //GetData();
                panelCustomerpaid.Visible = false;
                panelTransaction.Visible = false;
                cashpl.Visible = false;
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
                if (dataGridView2.CurrentCell.RowIndex == 9)
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
                if (!string.IsNullOrEmpty(lbltransid.Text))
                {
                    MessageBox.Show("Save Successfully");
                    //HdnCashRequestId = lbltransid.Text;
                    GetReport(HdnCashRequestId);
                    a1Panel1.Visible = false;
                    a1Panel2.Visible = false;
                    rbCash.Checked = false;
                    rdcheque.Checked = false;
                    rdtransfer.Checked = false;
                    radioButton1.Checked = false;
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
