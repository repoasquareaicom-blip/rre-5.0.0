using InvBal;
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

namespace Inventory.Accounts
{
    public partial class AccountsReceivable : Form
    {
        QuotationBal objQuotationBal = new QuotationBal();
        string[] transdate = new string[50];
        string ReceiptId = string.Empty;
        string BillBillIdValueId;
        string valuedata = string.Empty;
        


        public AccountsReceivable()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            lblTotalBillSelected.Visible = false;
            label17.Visible = false;
            btnAccountReceivableSave.Visible = false;
            panel11.Visible = false;
            //double less = Convert.ToDouble(txtDiscount.Text);
            //double total = Convert.ToDouble(lblBalancePaidAmount.Text);
            //double grandtotal = total - less;
            //lblPaidAmount.Text = grandtotal.ToString();
            //lblPaidAmount.Enabled = false;
        }
        string[] arr;
        private void LoadPortsChecking()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = 6;


            //dataGridView1.Columns[0].Name = "S.NO";
            dataGridView1.Columns[0].Name = "TransId";

            dataGridView1.Columns[1].Name = "TransDate";
            dataGridView1.Columns[2].Name = "Recipt Id";
            dataGridView1.Columns[3].Name = "Customer Name";
            
            dataGridView1.Columns[4].Name = "Amount";
            dataGridView1.Columns[5].Name = "Mode";

           



            //this.dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dataGridView1.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter; 

            this.dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            //this.dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dataGridView1.Columns[0].ReadOnly = true;
            this.dataGridView1.Columns[1].ReadOnly = true;
            this.dataGridView1.Columns[3].ReadOnly = true;
            this.dataGridView1.Columns[2].ReadOnly = true;
            this.dataGridView1.Columns[4].ReadOnly = true;
           



            //this.dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            //this.dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


           
            dataGridView1.Columns[0].HeaderText = "referenceid";

            dataGridView1.Columns[1].HeaderText = "Date";
            dataGridView1.Columns[2].HeaderText = "Reciept Id";
            dataGridView1.Columns[3].HeaderText = "Customer Name";
           
            dataGridView1.Columns[4].HeaderText = "Amount";
            dataGridView1.Columns[5].HeaderText = "Mode";

           

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.BackColor = Color.Gainsboro;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //this.dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


           //this.dataGridView1.Columns[0].Width = 30;
           this.dataGridView1.Columns[1].Width = 100;
           this.dataGridView1.Columns[2].Width = 100;
           this.dataGridView1.Columns[3].Width = 270;
          
           this.dataGridView1.Columns[4].Width = 100;
           this.dataGridView1.Columns[5].Width = 100;
           //this.dataGridView1.Columns[6].Width = 100;
            dataGridView1.Columns[0].Visible = false;

            DataGridViewButtonColumn Print = new DataGridViewButtonColumn();

            Print.HeaderText = "Print";
            Print.Text = "Print";
            Print.Name = "Print";
            
            Print.FlatStyle = FlatStyle.Popup;
            Print.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(Print);
            dataGridView1.Columns["Print"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter; 
            dataGridView1.Columns["Print"].Width = 100;

            DataGridViewButtonColumn Prints = new DataGridViewButtonColumn();

            Prints.Text = "Preview";
            Prints.Name = "Preview";
            Prints.FlatStyle = FlatStyle.Popup;
            Prints.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(Prints);
            dataGridView1.Columns["Preview"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter; 
            dataGridView1.Columns["Preview"].Width = 100;

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
            cashddl.AlternatingRowsDefaultCellStyle.BackColor = Color.White;



            foreach (DataGridViewColumn column in cashddl.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

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


        private void bindgrid()
        {
            try
            {
                string Customer_Name = Cusname.Text.Trim();
                DateTime FromDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
                DateTime ToDate = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);
                DataSet serachdata = objQuotationBal.SearchAccountProduct(Customer_Name, FromDate, ToDate);
                DataTable ds = serachdata.Tables[0];
                dataGridView1.DataSource = null;


                dataGridView1.Rows.Clear();


                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].Cells[0].Value = Convert.ToString(ds.Rows[i]["TransId"]);
                    dataGridView1.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["TransDate"]);
                    dataGridView1.Rows[i].Cells[2].Value = Convert.ToString(ds.Rows[i]["ReceiptId"]);
                    dataGridView1.Rows[i].Cells[3].Value = Convert.ToString(ds.Rows[i]["CustomerName"]);
                    dataGridView1.Rows[i].Cells[4].Value = Convert.ToString(ds.Rows[i]["Amount"]);
                    dataGridView1.Rows[i].Cells[5].Value = Convert.ToString(ds.Rows[i]["Mode"]);
                }
                // dataGridView1.DataSource = ds;

                foreach (DataGridViewColumn c in dataGridView1.Columns)
                {
                    c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
                }

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                dataGridView1.DefaultCellStyle.BackColor = Color.Gainsboro;
                dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                this.dataGridView1.Columns[1].Width = 100;
                this.dataGridView1.Columns[2].Width = 100;
                this.dataGridView1.Columns[3].Width = 320;
                this.dataGridView1.Columns[4].Width = 100;
                this.dataGridView1.Columns[5].Width = 70;
                this.dataGridView1.Columns[6].Width = 100;
                dataGridView1.Columns[0].Visible = false;
                //DataGridViewButtonColumn Print = new DataGridViewButtonColumn();
                //Print.HeaderText = "Print";
                //Print.Text = "Print";
                //Print.Name = "Print";
                //Print.FlatStyle = FlatStyle.Popup;
                //Print.UseColumnTextForButtonValue = true;
                //dataGridView1.Columns.Add(Print);
                //dataGridView1.Columns["Print"].Width = 80;

            }

            catch (Exception ex)
            {

            }
            
         
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

        public DataTable paymentDenoReturn()
        {
            DataTable paymentReturn = new DataTable();
            paymentReturn.Columns.Add("Denomination", typeof(string));
            paymentReturn.Columns.Add("Count", typeof(int));
            paymentReturn.Columns.Add("Amount", typeof(decimal));
            paymentReturn.Rows.Add("2000", 0, 0.0);
            paymentReturn.Rows.Add("1000", 0, 0.0);
            paymentReturn.Rows.Add("500", 0, 0.0);
            paymentReturn.Rows.Add("100", 0, 0.0);
            paymentReturn.Rows.Add("50", 0, 0.0);
            paymentReturn.Rows.Add("20", 0, 0.0);
            paymentReturn.Rows.Add("10", 0, 0.0);
            paymentReturn.Rows.Add("5", 0, 0.0);
            paymentReturn.Rows.Add("2", 0, 0.0);
            paymentReturn.Rows.Add("1", 0, 0.0);

            return paymentReturn;
        }

        private void AccountsReceivable_Load(object sender, EventArgs e)
        {
            paymentdenobind();
            paymentdenoReturnbind();
            GetAccountReceivableBalance();
            BindBankAccount();
            BindBankAccountTo();
            panel9.Visible = false;
           
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F3)
            {
                pnlprodsearch.Visible = true;
                txtprodsearch.SelectionStart = 0;
                txtprodsearch.SelectionLength = txtprodsearch.Text.Length;
                txtprodsearch.Text="";
                txtprodsearch.Focus();
                return true;
            }

            if (keyData == Keys.Escape)
            {
                if (pnlprodsearch.Visible)
                {
                    GetAccountReceivableBalance();
                    pnlprodsearch.Visible = false;
                    return true;
                }
                else
                {

                    DialogResult result = MessageBox.Show("Do you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        this.Close();
                    }
                    return true;
                }
            }
            if (txtprodsearch.Focused)
            {
                if (keyData == (Keys.Enter))
                {
                    //if (!string.IsNullOrEmpty(txtprodsearch.Text))
                    //{
                        GetAccountReceivableBalancesearch();
                    //}
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


            return base.ProcessCmdKey(ref msg, keyData);

        }
        public void GetAccountReceivableBalance()
        {
            DataTable dt = AccountReceivableBAL.GetAccountReceiveBalance();
            //dgvReceivableBalance.DataSource = dt;
            dgvReceivableBalance.Columns.Clear();
            dgvReceivableBalance.DataSource = dt;
            //dgvReceivableBalance.Columns["Estimationid"].Visible = false;
            //dgvReceivableBalance.Columns["Quotationid"].Visible = false;
            DataGridViewImageColumn img1 = new DataGridViewImageColumn();

            img1.Image = Inventory.Properties.Resources.user_edit;

            dgvReceivableBalance.Columns.Insert(7, img1);
            img1.HeaderText = "Pay Now";
            img1.Name = "ReceiveBalance";
            //dgvReceivableBalance.Columns["ReceiveBalance"].Width = 100;

            this.dgvReceivableBalance.Columns["Paid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvReceivableBalance.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvReceivableBalance.Columns["EstimationAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvReceivableBalance.Columns["TotalEstimation"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dgvReceivableBalance.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvReceivableBalance.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvReceivableBalance.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvReceivableBalance.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10.1F, FontStyle.Bold);
            dgvReceivableBalance.DefaultCellStyle.Font = new Font("Tahoma", 12.1F, GraphicsUnit.Pixel);
            dgvReceivableBalance.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvReceivableBalance.DefaultCellStyle.ForeColor = Color.Black;
            dgvReceivableBalance.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvReceivableBalance.Columns[0].Width =20;
            //dgvReceivableBalance.Columns[1].Width =20;
            //dgvReceivableBalance.Columns[2].Width =20;
            //dgvReceivableBalance.Columns[3].Width = 25;
            //dgvReceivableBalance.Columns[4].Width = 25;
            //dgvReceivableBalance.Columns[5].Width = 20;
            //dgvReceivableBalance.Columns[6].Width = 20;
            //dgvReceivableBalance.Columns[7].Width = 120;





            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {

                dgvReceivableBalance.Columns[0].Width = 30;
                dgvReceivableBalance.Columns[1].Width = 50;
                dgvReceivableBalance.Columns[2].Width = 25;
                dgvReceivableBalance.Columns[3].Width = 25;
                dgvReceivableBalance.Columns[4].Width = 35;
                dgvReceivableBalance.Columns[5].Width = 15;
                dgvReceivableBalance.Columns[6].Width = 20;
                dgvReceivableBalance.Columns[7].Width = 120;
            }
            else
            {
                dgvReceivableBalance.Columns[0].Width = 25;
                dgvReceivableBalance.Columns[1].Width = 70;
                dgvReceivableBalance.Columns[2].Width = 25;
                dgvReceivableBalance.Columns[3].Width = 25;
                dgvReceivableBalance.Columns[4].Width = 30;
                dgvReceivableBalance.Columns[5].Width = 15;
                dgvReceivableBalance.Columns[6].Width = 15;
                dgvReceivableBalance.Columns[7].Width = 120;
            }
            dgvReceivableBalance.Columns[0].HeaderText = "Customer Id";
            dgvReceivableBalance.Columns[1].HeaderText = "Customer Name";
            dgvReceivableBalance.Columns[2].HeaderText = "Mobile No";
            dgvReceivableBalance.Columns[3].HeaderText = "NO_Of_Bills";
            dgvReceivableBalance.Columns[4].HeaderText = "Total_Bill_Amount";
            dgvReceivableBalance.Columns[5].HeaderText = "Paid";
            dgvReceivableBalance.Columns[6].HeaderText = "Balance";
            dgvReceivableBalance.Columns[7].HeaderText = "Pay Now";

        }
        public void GetAccountReceivableBalancesearch()
        {
            DataTable dt = AccountReceivableBAL.GetAccountReceivableBalancesearch(txtprodsearch.Text);
            //dgvReceivableBalance.DataSource = dt;
            dgvReceivableBalance.Columns.Clear();
            dgvReceivableBalance.DataSource = dt;
            //dgvReceivableBalance.Columns["Estimationid"].Visible = false;
            //dgvReceivableBalance.Columns["Quotationid"].Visible = false;
            DataGridViewImageColumn img1 = new DataGridViewImageColumn();

            img1.Image = Inventory.Properties.Resources.user_edit;

            dgvReceivableBalance.Columns.Insert(7, img1);
            img1.HeaderText = "Pay Now";
            img1.Name = "ReceiveBalance";
            //dgvReceivableBalance.Columns["ReceiveBalance"].Width = 100;

            this.dgvReceivableBalance.Columns["Paid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvReceivableBalance.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvReceivableBalance.Columns["EstimationAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvReceivableBalance.Columns["TotalEstimation"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dgvReceivableBalance.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvReceivableBalance.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvReceivableBalance.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvReceivableBalance.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10.1F, FontStyle.Bold);
            dgvReceivableBalance.DefaultCellStyle.Font = new Font("Tahoma", 12.1F, GraphicsUnit.Pixel);
            dgvReceivableBalance.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvReceivableBalance.DefaultCellStyle.ForeColor = Color.Black;
            dgvReceivableBalance.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvReceivableBalance.Columns[0].Width =20;
            //dgvReceivableBalance.Columns[1].Width =20;
            //dgvReceivableBalance.Columns[2].Width =20;
            //dgvReceivableBalance.Columns[3].Width = 25;
            //dgvReceivableBalance.Columns[4].Width = 25;
            //dgvReceivableBalance.Columns[5].Width = 20;
            //dgvReceivableBalance.Columns[6].Width = 20;
            //dgvReceivableBalance.Columns[7].Width = 120;





            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {

                dgvReceivableBalance.Columns[0].Width = 30;
                dgvReceivableBalance.Columns[1].Width = 50;
                dgvReceivableBalance.Columns[2].Width = 25;
                dgvReceivableBalance.Columns[3].Width = 25;
                dgvReceivableBalance.Columns[4].Width = 35;
                dgvReceivableBalance.Columns[5].Width = 15;
                dgvReceivableBalance.Columns[6].Width = 20;
                dgvReceivableBalance.Columns[7].Width = 120;
            }
            else
            {
                dgvReceivableBalance.Columns[0].Width = 25;
                dgvReceivableBalance.Columns[1].Width = 70;
                dgvReceivableBalance.Columns[2].Width = 25;
                dgvReceivableBalance.Columns[3].Width = 25;
                dgvReceivableBalance.Columns[4].Width = 30;
                dgvReceivableBalance.Columns[5].Width = 15;
                dgvReceivableBalance.Columns[6].Width = 15;
                dgvReceivableBalance.Columns[7].Width = 120;
            }
            dgvReceivableBalance.Columns[0].HeaderText = "Customer Id";
            dgvReceivableBalance.Columns[1].HeaderText = "Customer Name";
            dgvReceivableBalance.Columns[2].HeaderText = "Mobile No";
            dgvReceivableBalance.Columns[3].HeaderText = "NO_Of_Bills";
            dgvReceivableBalance.Columns[4].HeaderText = "Total_Bill_Amount";
            dgvReceivableBalance.Columns[5].HeaderText = "Paid";
            dgvReceivableBalance.Columns[6].HeaderText = "Balance";
            dgvReceivableBalance.Columns[7].HeaderText = "Pay Now";

        }

        private void cashddl_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                decimal sum = 0;
                for (int i = 0; i < cashddl.Rows.Count; i++)
                {
                    cashddl.Rows[i].Cells[2].Value = Convert.ToDecimal(cashddl.Rows[i].Cells[0].Value) * Convert.ToDecimal(cashddl.Rows[i].Cells[1].Value);
                    sum = sum + Convert.ToDecimal(cashddl.Rows[i].Cells[2].Value);
                    txtTotal.Text = Convert.ToString(sum) + ".00";
                }
                if(Convert.ToDecimal(txtTotal.Text)>0)
                {
                txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblcashamountpay.Text));
                }
                else
                {
                    txtReturnBalAmount.Text = "0.00";
                }
            }
        }

        private void btncashpay_Click(object sender, EventArgs e)
        {
            lblPaidAmount.Enabled = false;
            panelEstimation.Visible = false;
            if (CashDenomination())
            {
                DenominationCheck();
                //btnAccountReceivableSave.Visible = true;
                //btnAccountReceivableSave.Focus();

            }
        }


        public bool CashDenomination()
        {
            bool s = true;
            int i = 0;
            string m = "";
            if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) < Convert.ToDecimal(lblBalancePaidAmount.Text)))
            {
                if (Convert.ToBoolean(Convert.ToDecimal(txtTotal.Text) < Convert.ToDecimal(lblcashamountpay.Text)))
                {

                    //txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblPaidAmount.Text));
                    // txtReturnBalAmount.Text="0.00";
                    // MessageBox.Show("Denomination total amount should not be less than paid amount");

                    i++;
                    m = m + "Denomination total amount should not be less than paid amount" + "\n";
                }
            }


            else if (Convert.ToDouble(lblcashamountpay.Text) > Convert.ToDouble(txtTotal.Text))
            {
                MessageBox.Show("Please Enter Correct Denomination");
                lblPaidAmount.Enabled = true;
                panelEstimation.Visible = true;
                btnAccountReceivableSave.Visible = false;
            }

            if (Convert.ToDecimal(txtReturnBalAmount.Text) > 0)
            {
                MessageBox.Show("Please Enter Return Denomination");
                lblPaidAmount.Enabled = true;
                panelEstimation.Visible = true;
                btnAccountReceivableSave.Visible = false;
            }

            //if (txtReturnBalAmount.Text != "0.00")
            //{
            //    if (txtReturnAmount.Text == "0.00")
            //    {
            //        i++;
            //        m = m + "Enter return amount." + "\n";
            //        // MessageBox.Show("Enter return amount.");

            //    }

            //}


            if (!string.IsNullOrEmpty(m))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + m);
                s = false;

            }
            return s;
        }

        public void DenominationCheck()
        {
            decimal sum = 0.00M;
            for (int i = 0; i < cashddl.Rows.Count; i++)
            {
                sum += Convert.ToDecimal(cashddl.Rows[i].Cells[2].Value);
            }
            lblPaidAmount.Text = Convert.ToString(sum);




            //if (Convert.ToString(txtTotal.Text) != "0.00")
            //{


            //    if (Convert.ToBoolean(Convert.ToDecimal(txtTotal.Text) == Convert.ToDecimal(lblcashamountpay.Text)))
            //    {

            //        txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblcashamountpay.Text));

            //        cashpl.Visible = false;
            //        btnAccountReceivableSave.Focus();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Total should be equal to pay Amount ");
            //        lblPaidAmount.Text = lblcashamountpay.Text;
            //    }

            //}

            if (Convert.ToString(txtTotal.Text) != "0.00")
            {




                txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblcashamountpay.Text));

               
                if (Convert.ToDouble(txtReturnBalAmount.Text)==0)
                {
                    cashpl.Visible = false;
                    cashplReturn.Visible = false;
                    btnAccountReceivableSave.Visible = true;
                    btnAccountReceivableSave.Focus();
                }

                btnAccountReceivableSave.Focus();


                lblPaidAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(txtReturnBalAmount.Text));


            }

        }

        private void rbtnCash_CheckedChanged(object sender, EventArgs e)
        {
            txtReturnBalAmount.Text = "0.00";
            txtTotal.Text = "0.00";
            txtReturnAmount.Text = "0.00";
            paymentdenoReturnbind();
            paymentdenobind();
            btnPayReturnBalance.Enabled = false;
            lblPaidAmount.Enabled = false;
            //txtDiscount.Enabled = false;
            btnAccountReceivableSave.Visible = false;
            if (rbtnCash.Checked == true)
            {
                //lblBalancePaidAmount,lblPaidAmount
                if (!string.IsNullOrEmpty(Convert.ToString(lblPaidAmount.Text)))
                {
                if (Convert.ToDecimal(lblPaidAmount.Text) > Convert.ToDecimal(lblBalancePaidAmount.Text))
                {
                    MessageBox.Show("Amount pay should be equal to  Estimation Amount ");
                    panelCard.Visible = false;
                    cashpl.Visible = false;
                
                    rbtnCard.Checked = false;
                    rbtnCheque.Checked = false;
                    rbtnCash.Checked = false;
                    lblPaidAmount.Enabled = true;
                    lblPaidAmount.Focus();
                }
               else if (Convert.ToString(lblPaidAmount.Text) != "0.00" && !string.IsNullOrEmpty(Convert.ToString(lblPaidAmount.Text)))
                {

                    //if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) >= Convert.ToDecimal(lblBalancePaidAmount.Text)))
                    //{
                    //if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) <= Convert.ToDecimal(lblBalancePaidAmount.Text)))
                    //{
                    lblcashamountpay.Text = lblPaidAmount.Text;
                    panelCard.Visible = false;
                    cashpl.Visible = true;
                    cashddl.Focus();
                    cashddl.CurrentCell = cashddl[1, 0];
                    ////txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblBalancePaidAmount.Text));
                    //}
                    //else
                    //{
                    //   // MessageBox.Show("Amount pay should be equal to or greater than Estimation Amount ");
                    //    panelCard.Visible = false;
                    //    cashpl.Visible = false;

                    //    rbtnCard.Checked = false;
                    //    rbtnCheque.Checked = false;
                    //    rbtnCash.Checked = false;
                    //}

                }
                
                else
                {
                    MessageBox.Show("Enter Amount pay.");
                   
                    rbtnCard.Checked = false;
                    rbtnCheque.Checked = false;
                    rbtnCash.Checked = false;
                    cashpl.Visible = false;
                    cashpl.BringToFront();
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
                    cashpl.Visible = false;
                    cashpl.BringToFront();
                    lblPaidAmount.Enabled = true;
                    lblPaidAmount.Focus();
                }


            }
        }

        private void btnCloseDenominationPanel_Click(object sender, EventArgs e)
        {
            //DenominationCheck();
            cashpl.Visible = false;
            rbtnCash.Checked = false;
            rbtnCard.Checked = false;
            rbtnCheque.Checked = false;
            lblPaidAmount.Enabled = true;
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
        private void rbtnCard_CheckedChanged(object sender, EventArgs e)
        {
            clearpanel();
            btnAccountReceivableSave.Visible = false;
            lblPaidAmount.Enabled = false;
         //   txtDiscount.Enabled = false;
            txtReturnBalAmount.Text = "0.00";
            txtTotal.Text = "0.00";
            txtReturnAmount.Text = "0.00";
            paymentdenoReturnbind();
            paymentdenobind();
            btnPayReturnBalance.Enabled = false;

            lblInstrumentDisplay.Visible = false;
            txtCardorChequeNo.Visible = false;
            lblcheque.Text = "Transfer";
            lblcheque.Visible = true;
            lblTransaction.Text = "Transaction Details";
            lblCardorChequeAmt.Text = lblPaidAmount.Text;
            cashpl.Visible = false;
            if (rbtnCard.Checked == true)
            {

                if (Convert.ToString(lblPaidAmount.Text) != "0.00" && !string.IsNullOrEmpty(Convert.ToString(lblPaidAmount.Text)))
                {

                    //if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) == Convert.ToDecimal(lblBalancePaidAmount.Text)))
                    //{
                    if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) <= Convert.ToDecimal(lblBalancePaidAmount.Text)))
                    {
                        lblcashamountpay.Text = lblPaidAmount.Text;
                        //rbtnVisa.Visible = true;
                        //rbtnMastro.Visible = true;
                        panelCard.Visible = true;
                        // lblcheque.Visible = false;



                        //txtReturnBalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(lblBalancePaidAmount.Text));
                    }
                    else
                    {
                        MessageBox.Show("Amount pay should be less than or equal to Estimation Amount ");
                       
                        cashpl.Visible = false;

                        rbtnCard.Checked = false;
                        rbtnCheque.Checked = false;
                        rbtnCash.Checked = false;
                        lblPaidAmount.Enabled = true;
                        panelCard.Visible = false;

                    }

                }
                else
                {
                    MessageBox.Show("Enter Amount pay.");
                   // lblPaidAmount.Enabled = true;
                    

                    rbtnCard.Checked = false;
                    rbtnCheque.Checked = false;
                    rbtnCash.Checked = false;
                    lblPaidAmount.Enabled = true;
                    lblPaidAmount.Focus();
                }
            }



        }

        private void dgvReceivableBalance_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                if (dgvReceivableBalance.Columns[e.ColumnIndex].HeaderText == "Pay Now")
                {
                   
                    panelEstimation.Visible = true;
                    lblBalancePaidAmount.Text = "0.00";
                    cashpl.Visible = false;
                    cashplReturn.Visible = false;
                    for (int i = 0; i < dgvReceivableBalance.Rows.Count; i++)
                    {
                        string Id = Convert.ToString(dgvReceivableBalance.Rows[e.RowIndex].Cells["Customer Id"].Value);
                        GetQuotationEstimationBills(Id);
                        lblPaidAmount.Text = "0.00";
                        rbtnCash.Checked = false;
                        rbtnCard.Checked = false;
                        rbtnCheque.Checked = false;
                   }

                    if (dgvEstimationBill.Rows.Count > 0)
                    {
                        dgvEstimationBill.Focus();
                        dgvEstimationBill.CurrentCell = dgvEstimationBill[7, 0];
                    }

                    lblPaidAmount.Enabled = true;
                }


            }
        }

        public void GetQuotationEstimationBills(string CustomerId)
        {
            DataTable dt = AccountReceivableBAL.GetQuotationEstimationBills_fix(CustomerId);

            dgvEstimationBill.Columns.Clear();
            dgvEstimationBill.DataSource = dt;


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

            dgvEstimationBill.Columns.Add(dgvChb);

            //dgvEstimationBill.Columns["TotalQuantity"].Visible = false;
            //dgvEstimationBill.Columns["Customerid"].Visible = false;
            //dgvEstimationBill.Columns["LessAmount"].Visible = false;
            //dgvEstimationBill.Columns["Quotationid"].Visible = false;
            //dgvEstimationBill.Columns["TotalAmount"].Visible = false;

            dgvEstimationBill.Columns["Estimationid"].HeaderText = "Bill No";
            dgvEstimationBill.Columns["Date"].HeaderText = "Date";
            dgvEstimationBill.Columns["Customerid"].HeaderText = "Customer Id";
            // dgvEstimationBill.Columns["GrandTotal"].HeaderText = "Paid Amount";
            dgvEstimationBill.Columns["GrandTotal"].HeaderText = "Bill Amount";
            dgvEstimationBill.Columns["Paid"].HeaderText = "Paid";
            dgvEstimationBill.Columns["Balance"].HeaderText = "Balance";

           // this.dgvEstimationBill.Columns["TotalQuantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dgvEstimationBill.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
           // this.dgvEstimationBill.Columns["LessAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dgvEstimationBill.Columns["GrandTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvEstimationBill.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvEstimationBill.Columns["Paid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvEstimationBill.Columns["chbPass"].DisplayIndex = 0;

            dgvEstimationBill.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvEstimationBill.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvEstimationBill.AlternatingRowsDefaultCellStyle.BackColor = Color.White;




        }

        private void dgvEstimationBill_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int count = 0;

            if (e.RowIndex >= 0)
            {
                if (dgvEstimationBill.Columns[e.ColumnIndex].HeaderText == "Bill Check")
                {


                    if (valuedata == string.Empty)
                    {
                        valuedata = Convert.ToString(dgvEstimationBill.Rows[dgvEstimationBill.CurrentCell.RowIndex].Cells[0].Value);
                    }
                    else
                    {
                        valuedata = valuedata + ", " + Convert.ToString(dgvEstimationBill.Rows[dgvEstimationBill.CurrentCell.RowIndex].Cells[0].Value);
                    }
                        
                    
                        //string[] items = valuedata.Split(new char[] {',' }, StringSplitOptions.None);
                        
                   

                    DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)dgvEstimationBill.Rows[e.RowIndex].Cells["chbPass"];
                    //CbxCell.Value = true;
                    try
                    {
                        if (CbxCell.Value.ToString() == "True")
                        {
                            CbxCell.Value = false;
                        }
                        else
                        {
                            CbxCell.Value = true;
                        }
                    }
                    catch(Exception ex){
                        CbxCell.Value = true;
                    }


                }
               
                 if (dgvEstimationBill.Columns[e.ColumnIndex].HeaderText == "Bill No")
                {
                  BillBillIdValueId = Convert.ToString(dgvEstimationBill.Rows[dgvEstimationBill.CurrentCell.RowIndex].Cells[0].Value);
                  panel9.Visible = true;
                  Getestimationvalue(BillBillIdValueId);
                 }
                Decimal TotalBalance = 0.00M;
              
                foreach (DataGridViewRow row in dgvEstimationBill.Rows)
                {
                    DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells["chbPass"];
                    if (Convert.ToBoolean(CbxCell.Value) == true)
                    {
                        //CbxCell.Value = false;
                       // panel9.Visible = true ;
                        count += 1;
                        TotalBalance += Convert.ToDecimal(row.Cells["Balance"].Value);
                        //BillBillIdValueId = Convert.ToString(row.Cells["Estimationid"].Value);
                       
                    }
                }
               
               
                lblTotalBillSelected.Text = Convert.ToString(count);
                lblBalancePaidAmount.Text = Convert.ToString(TotalBalance);
                //double less = Convert.ToDouble(txtDiscount.Text);
                //double total = Convert.ToDouble(lblBalancePaidAmount.Text);
                //double grandtotal = total - less;
                //lblPaidAmount.Text = grandtotal.ToString();
                //lblPaidAmount.Enabled = false;
                //if (count==0)
                //{
                //    lblPaidAmount.Text = "0.00";
                //}
            }
        }

        public void Getestimationvalue(string value)
        {
            panel9.Visible = true;
            DataTable dt = AccountReceivableBAL.GetQuotationEstimationBills_Data(value);
            dvgestimationdata.Columns.Clear();
            dvgestimationdata.DataSource = dt;
            dvgestimationdata.Columns["DisplayName"].HeaderText = "Display Name";
            dvgestimationdata.Columns["Quantity"].HeaderText = "Quantity";
            dvgestimationdata.Columns["UOM"].HeaderText = "UOM";
            dvgestimationdata.Columns["Rate"].HeaderText = "Rate";
            dvgestimationdata.Columns["Amount"].HeaderText = "Amount";
            this.dvgestimationdata.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dvgestimationdata.Columns["DisplayName"].Width = 200;        
            dvgestimationdata.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dvgestimationdata.DefaultCellStyle.BackColor = Color.Gainsboro;
            dvgestimationdata.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
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

        private void btnAccountReceivableSave_Click(object sender, EventArgs e)
        {
            int[] status = new int[2];
            string remainingamt = Convert.ToString(lblPaidAmount.Text);
            AccountReceivableBAL ObjAccountReceivableBAL = new AccountReceivableBAL();

            if (Validation())
            {
                if (dgvEstimationBill.Rows.Count > 0)
                {
                    //for (int i = 0; i < dgvEstimationBill.Rows.Count; i++)
                    //{
                        int col = dgvEstimationBill.CurrentCell.ColumnIndex;
                        int row = dgvEstimationBill.CurrentCell.RowIndex;
                        //if (dgvEstimationBill.Columns["Bill Check"].HeaderText == "Bill Check")
                        // {
                       // DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)dgvEstimationBill.Rows[i].Cells["chbPass"];
                        //if (Convert.ToBoolean(CbxCell.Value) == true)
                        //{

                            if (rbtnCard.Checked == true || rbtnCash.Checked == true || rbtnCheque.Checked == true)
                            {
                                if (rbtnCash.Checked == true)
                                {
                                    if (txtTotal.Text != "0.00")
                                    {

                                        int coin = 0;
                                        for (int j = 0; j < cashddl.Rows.Count; j++)
                                        {
                                            if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 2000)
                                            {
                                                ObjAccountReceivableBAL.TwoThousand = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 1000)
                                            {
                                                ObjAccountReceivableBAL.Thousand = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 500)
                                            {
                                                ObjAccountReceivableBAL.FiveHundred = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 100)
                                            {
                                                ObjAccountReceivableBAL.Hundred = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 50)
                                            {
                                                ObjAccountReceivableBAL.Fifty = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 20)
                                            {
                                                ObjAccountReceivableBAL.Twenty = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 10)
                                            {
                                                ObjAccountReceivableBAL.Ten = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 5)
                                            {
                                                ObjAccountReceivableBAL.Five = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 2)
                                            {
                                                ObjAccountReceivableBAL.Two = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddl.Rows[j].Cells[0].Value) == 1)
                                            {
                                                ObjAccountReceivableBAL.One = Convert.ToString(cashddl.Rows[j].Cells[1].Value);
                                            }

                                        }


                                        int coinreturn = 0;
                                        for (int k = 0; k < cashddlReturn.Rows.Count; k++)
                                        {
                                            if (Convert.ToInt32(cashddlReturn.Rows[k].Cells[0].Value) == 2000)
                                            {
                                                ObjAccountReceivableBAL.TwoThousandReturn = "-" + Convert.ToString(cashddlReturn.Rows[k].Cells[1].Value);
                                            }

                                            if (Convert.ToInt32(cashddlReturn.Rows[k].Cells[0].Value) == 1000)
                                            {
                                                ObjAccountReceivableBAL.ThousandReturn = "-" + Convert.ToString(cashddlReturn.Rows[k].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddlReturn.Rows[k].Cells[0].Value) == 500)
                                            {
                                                ObjAccountReceivableBAL.FiveHundredReturn = "-" + Convert.ToString(cashddlReturn.Rows[k].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddlReturn.Rows[k].Cells[0].Value) == 100)
                                            {
                                                ObjAccountReceivableBAL.HundredReturn = "-" + Convert.ToString(cashddlReturn.Rows[k].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddlReturn.Rows[k].Cells[0].Value) == 50)
                                            {
                                                ObjAccountReceivableBAL.FiftyReturn = "-" + Convert.ToString(cashddlReturn.Rows[k].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddlReturn.Rows[k].Cells[0].Value) == 20)
                                            {
                                                ObjAccountReceivableBAL.TwentyReturn = "-" + Convert.ToString(cashddlReturn.Rows[k].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddlReturn.Rows[k].Cells[0].Value) == 10)
                                            {
                                                ObjAccountReceivableBAL.TenReturn = "-" + Convert.ToString(cashddlReturn.Rows[k].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddlReturn.Rows[k].Cells[0].Value) == 5)
                                            {
                                                ObjAccountReceivableBAL.FiveReturn = "-" + Convert.ToString(cashddlReturn.Rows[k].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddlReturn.Rows[k].Cells[0].Value) == 2)
                                            {
                                                ObjAccountReceivableBAL.TwoReturn = "-" + Convert.ToInt32(cashddlReturn.Rows[k].Cells[1].Value);
                                            }
                                            if (Convert.ToInt32(cashddlReturn.Rows[k].Cells[0].Value) == 1)
                                            {
                                                ObjAccountReceivableBAL.OneReturn = "-" + Convert.ToInt32(cashddlReturn.Rows[k].Cells[1].Value);
                                            }

                                        }

                                        if (Convert.ToDecimal(txtReturnBalAmount.Text) > 0)
                                        {
                                            ObjAccountReceivableBAL.IsReturnAmount = true;
                                            //if (Convert.ToString(txtReturnAmount.Text) == Convert.ToString(lblReturnbalance.Text))
                                            //{

                                            //}

                                        }
                                        else
                                        {
                                            ObjAccountReceivableBAL.IsReturnAmount = false;

                                        }

                                        ObjAccountReceivableBAL.ReturnAmount = txtReturnAmount.Text;
                                        ObjAccountReceivableBAL.Flag = "Cash";
                                        DateTime dateCash = new DateTime(TransactionDate.Value.Year, TransactionDate.Value.Month, TransactionDate.Value.Day);
                                        ObjAccountReceivableBAL.InstrumentDate = dateCash;
                                        //ObjAccountReceivableBAL.EnteredBy = Program.userid;
                                        ObjAccountReceivableBAL.Total = Convert.ToString(Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(txtReturnAmount.Text));
                                        ObjAccountReceivableBAL.DenominationTotal = Convert.ToString(Convert.ToDecimal(txtTotal.Text));
                                    }
                                    else
                                    {
                                        MessageBox.Show("Enter amount in Denomination");
                                    }

                                }

                                ///////////////////////////////////////////////////
                                if (rbtnCard.Checked == true)
                                {
                                    DateTime dateCard = new DateTime(TransactionDate.Value.Year, TransactionDate.Value.Month, TransactionDate.Value.Day);
                                  

                                    ObjAccountReceivableBAL.Type = "Transfer";
                                    ObjAccountReceivableBAL.Reference = txtReference.Text;
                                    ObjAccountReceivableBAL.Flag = "Card";

                                    ObjAccountReceivableBAL.InstrumentBank = ddlBankAccount.Text;
                                    ObjAccountReceivableBAL.InstrumentSource = ddlBankAccountTo.Text;
                                    ObjAccountReceivableBAL.InstrumentNo = null;

                                    //transdate = TransactionDate.Text.Split('-');
                                    //ObjAccountReceivableBAL.InstrumentDate = TransactionDate.Text;
                                    ObjAccountReceivableBAL.InstrumentDate = dateCard;
                                    ObjAccountReceivableBAL.Total = Convert.ToString(lblPaidAmount.Text);
                                    ObjAccountReceivableBAL.DenominationTotal = "0.00";

                                }
                                if (rbtnCheque.Checked == true)
                                {
                                    DateTime dateCheque = new DateTime(TransactionDate.Value.Year, TransactionDate.Value.Month, TransactionDate.Value.Day);
                                    ObjAccountReceivableBAL.Type = "Cheque";
                                    ObjAccountReceivableBAL.InstrumentBank = ddlBankAccount.Text;
                                    ObjAccountReceivableBAL.InstrumentSource = ddlBankAccountTo.Text;
                                    ObjAccountReceivableBAL.InstrumentNo = txtCardorChequeNo.Text;
                                    // ObjAccountReceivableBAL.InstrumentDate = TransactionDate.Text;
                                    ObjAccountReceivableBAL.InstrumentDate =dateCheque;
                                    ObjAccountReceivableBAL.Reference = txtReference.Text;
                                    ObjAccountReceivableBAL.Flag = "Cheque";
                                    ObjAccountReceivableBAL.Total = Convert.ToString(lblPaidAmount.Text);
                                    ObjAccountReceivableBAL.DenominationTotal = "0.00";
                                }

                                ObjAccountReceivableBAL.BillId = valuedata;
                                ObjAccountReceivableBAL.CustomerId = Convert.ToString(dgvEstimationBill.Rows[0].Cells["Customerid"].Value);
                                ObjAccountReceivableBAL.EnteredBy = Program.userid;

                                if (Convert.ToInt32(lblTotalBillSelected.Text) >= 1)
                                {
                                    string symbol = remainingamt.Substring(0, 1);
                                    if (symbol == "-")
                                    {
                                        string[] s = remainingamt.Split('-');
                                        ObjAccountReceivableBAL.PaidAmount = s[1];
                                    }
                                    else
                                    {
                                        ObjAccountReceivableBAL.PaidAmount = remainingamt;
                                    }

                                    ObjAccountReceivableBAL.IsRecord = true; // 1
                                    //ObjAccountReceivableBAL.PaidAmount = remainingamt;
                                }
                                else
                                {
                                    ObjAccountReceivableBAL.IsRecord = false;  // 0
                                    ObjAccountReceivableBAL.PaidAmount = Convert.ToString(lblPaidAmount.Text);

                                    // ObjAccountReceivableBAL.PaidAmount = remainingamt;
                                }

                                if (rbtnCard.Checked == true || rbtnCash.Checked == true || rbtnCheque.Checked == true)
                                {
                                    bool d = false, r = true;
                                    if (rbtnCash.Checked == true)
                                    {
                                        if (txtTotal.Text != "0.00")
                                        //  if (txtTotal.Text != "0.00" && Convert.ToDecimal(txtTotal.Text) == Convert.ToDecimal(lblcashamountpay.Text))
                                        {

                                            //  
                                            d = true;
                                        }
                                        else
                                        {
                                            MessageBox.Show("Enter amount in denomination");
                                            d = false;
                                        }

                                        if (ObjAccountReceivableBAL.IsReturnAmount == true)
                                        {
                                            if (Convert.ToString(txtReturnAmount.Text) == "0.00" || string.IsNullOrEmpty(txtReturnAmount.Text))
                                            {
                                                MessageBox.Show("Enter return amount in Denomination");
                                                r = false;
                                            }
                                            else
                                            {
                                                r = true;
                                            }
                                        }

                                        if (Convert.ToString(txtReturnAmount.Text) == Convert.ToString(txtReturnBalAmount.Text))
                                        {
                                            r = true;
                                        }
                                        else
                                        {
                                            if (cashplReturn.Visible)
                                            {
                                                MessageBox.Show("Enter valid return amount in Denomination");
                                                r = false;
                                            }
                                        }

                                        if (d == true && r == true)
                                        {
                                            status = AccountReceivableBAL.SaveAccountReceiveBalance(ObjAccountReceivableBAL);
                                        }

                                    }

                                    if (rbtnCard.Checked == true)
                                    {

                                        status = AccountReceivableBAL.SaveAccountReceiveBalance(ObjAccountReceivableBAL);


                                    }

                                    if (rbtnCheque.Checked == true)
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(txtCardorChequeNo.Text)) && (lblcheque.Text == "Cheque"))
                                        {

                                            status = AccountReceivableBAL.SaveAccountReceiveBalance(ObjAccountReceivableBAL);
                                        }
                                        else
                                        {
                                            MessageBox.Show("Enter Cheque Number");
                                        }
                                    }


                                }
                               
                                    MessageBox.Show("Paid Successfully");
                                    valuedata = string.Empty;
                                    //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    //if (result == DialogResult.Yes)
                                    //{
                                    //    ReceiptReportView objReceiptReportView = new ReceiptReportView(ObjAccountReceivableBAL.BillId);
                                    //    objReceiptReportView.Show();
                                    //}
                                    // GetAccountReceivableBalance();
                                    // dgvEstimationBill.DataSource = null;


                                    GetReceipt();


                              
                                //if (status[0] == 1)
                                //{
                                //    remainingamt = Convert.ToString(status[1]);
                                //    // lblPaidAmount.Text = Convert.ToString(status[1]);
                                //    lblTotalBillSelected.Text = Convert.ToString(Convert.ToInt32(lblTotalBillSelected.Text) - 1);

                                //    //arr = new string[1];
                                //    ////for (int a = 0; a < arr.Length; a++)
                                //    ////{
                                //    //    arr[0] =Convert.ToString(AccountReceivableBAL.GetTransId(ObjAccountReceivableBAL.BillId));
                                //    //    GetReceipt(arr[0]);
                                //    ////}
                                //}


                                //GetQuotationEstimationBills();
                            }


                        //}



                    //}
                }

                

            }

        }

        public void clear()
        {
            lblTotalBillSelected.Text = "0";
            lblPaidAmount.Text = "0.00";
            lblBalancePaidAmount.Text = "0.00";
          //  txtDiscount.Text = "0.00";
            panelEstimation.Visible = false;

            paymentdenobind();
            paymentdenoReturnbind();
            txtTotal.Text = "0.00";
            lblReturnbalance.Text = "0.00";
            txtReturnAmount.Text = "0.00";
            GetAccountReceivableBalance();
            cashpl.Visible = false;
            cashplReturn.Visible = false;
            lblPaidAmount.Enabled = true;
            btnAccountReceivableSave.Visible = false;
            GetAccountReceivableBalance();
            panel9.Visible = false;
            pnlprodsearch.Visible = false;
        }

        private void btnAccountReceivableClear_Click(object sender, EventArgs e)
        {
            clear();
            GetAccountReceivableBalance();
            dgvEstimationBill.DataSource = null;
        }

        private void btnEstimationPanelClose_Click(object sender, EventArgs e)
        {
            panelEstimation.Visible = false;
            lblBalancePaidAmount.Text = "0.00";
            lblPaidAmount.Text = "0.00";
          //  txtDiscount.Text = "0.00";
            rbtnCash.Checked = false;
            rbtnCard.Checked = false;
            rbtnCheque.Checked = false;
            lblTotalBillSelected.Text = "0";
        }

        private void cashdetailsclose_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (validationPaymentCheque())
            {
                panelCard.Visible = false;
                btnAccountReceivableSave.Visible = true;
            }

        }
        public bool validationPaymentCheque()
        {
            bool Status = true;
            string msg = "";
            int i = 0;


            if (Convert.ToString(ddlBankAccount.SelectedIndex) == "-1")
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
            if (rbtnCheque.Checked == true)
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
        private void rbtnCheque_CheckedChanged(object sender, EventArgs e)
        {
            btnAccountReceivableSave.Visible = false;
            lblPaidAmount.Enabled = false;
           // txtDiscount.Enabled = false;
            txtReturnBalAmount.Text = "0.00";
            txtTotal.Text = "0.00";
            txtReturnAmount.Text = "0.00";
            paymentdenoReturnbind();
            paymentdenobind();

            btnPayReturnBalance.Enabled = false;

            clearpanel();
            lblInstrumentDisplay.Visible = true;
            txtCardorChequeNo.Visible = true;
            lblcheque.Text = "Cheque";
            lblTransaction.Text = "Cheque Details";
            lblCardorChequeAmt.Text = lblPaidAmount.Text;
            if (rbtnCheque.Checked == true)
            {

                if (Convert.ToString(lblPaidAmount.Text) != "0.00" && !string.IsNullOrEmpty(Convert.ToString(lblPaidAmount.Text)))
                {
                    if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) <= Convert.ToDecimal(lblBalancePaidAmount.Text)))
                    {
                        //if (Convert.ToBoolean(Convert.ToDecimal(lblPaidAmount.Text) == Convert.ToDecimal(lblBalancePaidAmount.Text)))
                        //{

                        //rbtnVisa.Visible = false;
                        //rbtnMastro.Visible = false;
                        lblcheque.Visible = true;
                        cashpl.Visible = false;
                        panelCard.Visible = true;

                    }
                    else
                    {
                        MessageBox.Show("Amount pay should be less than or equal to Estimation Amount ");
                        
                        panelCard.Visible = false;
                        cashpl.Visible = false;

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
                    lblPaidAmount.Enabled = true;
                    lblPaidAmount.Focus();
                    rbtnCard.Checked = false;
                    rbtnCheque.Checked = false;
                    rbtnCash.Checked = false;
                    lblPaidAmount.Enabled = true;
                    lblPaidAmount.Focus();
                }


            }
        }


        private void btnCardPanelClose_Click(object sender, EventArgs e)
        {
            panelCard.Visible = false;
        }

        private void btnPayReturnBalance_Click(object sender, EventArgs e)
        {
            cashplReturn.Visible = true;
            lblReturnbalance.Text = txtReturnBalAmount.Text;
            cashddlReturn.Focus();
            cashddlReturn.CurrentCell = cashddlReturn[1, 0];
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

        private void btnCloseReturnDeno_Click(object sender, EventArgs e)
        {
            // ReturnDenomination();
            cashplReturn.Visible = false;
        }

        private void btnReturnOK_Click(object sender, EventArgs e)
        {
            ReturnDenomination();
            if (Convert.ToDecimal(lblReturnbalance.Text) == Convert.ToDecimal(txtReturnAmount.Text))
            {
                panelEstimation.Visible = false;
                lblPaidAmount.Enabled = false;
                cashplReturn.Visible = false;
                cashpl.Visible = false;
                btnAccountReceivableSave.Visible = true;
                btnAccountReceivableSave.Focus();
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

        private void txtReturnBalAmount_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToDecimal(txtReturnBalAmount.Text) > 0)
            {
                btnPayReturnBalance.Enabled = true;
            }
            else
            {
                btnPayReturnBalance.Enabled = false;
                cashplReturn.Visible = false;
            }
        }

        private void txtReturnAmount_TextChanged(object sender, EventArgs e)
        {
            //if (Convert.ToDecimal(lblReturnbalance.Text)<Convert.ToDecimal(txtReturnAmount.Text))
            //{
            //    MessageBox.Show("Total should be equal to return pay amount.");
            //    txtReturnAmount.Text = "0.00";
            //}
        }

        protected void Quantitytomove_press(object sender, KeyPressEventArgs e)
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
                rbtnCard.Enabled = false;
                rbtnCash.Enabled = false;
                rbtnCheque.Enabled = false;
                lblPaidAmount.Text = "0.00";

                rbtnCard.Checked = false;
                rbtnCash.Checked = false;
                rbtnCheque.Checked = false;

            }
        }

        private void btnCardClose_Click(object sender, EventArgs e)
        {
            //panelCard.Visible = false;
            panelCard.Visible = false;
            rbtnCash.Checked = false;
            rbtnCard.Checked = false;
            rbtnCheque.Checked = false;
            lblPaidAmount.ReadOnly = false;
            lblPaidAmount.Enabled = true;
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
            if (!(char.IsLetterOrDigit(e.KeyChar)) && !(char.IsWhiteSpace(e.KeyChar)))
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

        private void lblBalancePaidAmount_Click(object sender, EventArgs e)
        {

        }

        public bool Validation()
        {
            bool Status = true;
            string msg = "";
            int i = 0;

            //1.Please check Pay Now
            if (dgvEstimationBill.Rows.Count == 0)
            {
                i++;
                msg = msg + "*Please check Pay Now" + "\n";

            }

            //2.Please check Bill
            if (dgvEstimationBill.Rows.Count > 0)
            {
                int temp = 0;
                for (int j = 0; j < dgvEstimationBill.Rows.Count; j++)
                {

                    int col = dgvEstimationBill.CurrentCell.ColumnIndex;
                    int row = dgvEstimationBill.CurrentCell.RowIndex;

                    DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)dgvEstimationBill.Rows[j].Cells["chbPass"];
                    if (Convert.ToBoolean(CbxCell.Value) == true)
                    {
                        temp++;
                    }
                }

                if (temp == 0)
                {
                    i++;
                    msg = msg + "*Please check Bill" + "\n";
                }
            }

            //3.please check Payment mode

            if (!string.IsNullOrEmpty(lblPaidAmount.Text) && Convert.ToString(lblPaidAmount.Text) != "0.00")
            {
                //if (Convert.ToDecimal(lblPaidAmount.Text) < Convert.ToDecimal(txtTotal.Text))
                //{
                //    i++;
                //    msg = msg + "*Paid amount should be greater than or equal to estimation amount" + "\n";
                //}
                if (rbtnCard.Checked == true || rbtnCheque.Checked == true)
                {
                    if (Convert.ToDecimal(lblPaidAmount.Text) > Convert.ToDecimal(lblBalancePaidAmount.Text))
                    {
                        i++;
                        msg = msg + "*Paid amount should be less than or equal to estimation amount" + "\n";
                    }
                }
            }
            else
            {
                i++;
                msg = msg + "*Please enter paid amount" + "\n";
                lblPaidAmount.Enabled = true;
            }



            //4.please check Payment mode

            if (rbtnCard.Checked == false && rbtnCash.Checked == false && rbtnCheque.Checked == false)
            {
                i++;
                msg = msg + "*Please choose pay mode among Cash,Card or Cheque" + "\n";
            }
            else
            {
                //if (rbtnCash.Checked == true)
                //{

                //    if (!string.IsNullOrEmpty(lblPaidAmount.Text) || Convert.ToString(lblPaidAmount.Text) != "0.00")
                //    {
                //        if (Convert.ToDecimal(lblcashamountpay.Text) < Convert.ToDecimal(txtTotal.Text))
                //        {
                //            i++;
                //            //msg = msg + "*Paid amount should be greater than or equal to estimation amount" + "\n";
                //            msg = msg + "*Paid amount should be  equal to  total amount" + "\n";
                //        }
                //    }
                //    else
                //    {
                //        i++;
                //        msg = msg + "*Please enter paid amount" + "\n";
                //    }
                //}

                if (rbtnCash.Checked == true)
                {

                    if (string.IsNullOrEmpty(lblPaidAmount.Text) || Convert.ToString(lblPaidAmount.Text) == "0.00")
                    {

                        i++;
                        msg = msg + "*Please enter paid amount" + "\n";
                        lblPaidAmount.Enabled = true;
                    }
                }
                if (rbtnCard.Checked == true)
                {
                    if (!string.IsNullOrEmpty(lblPaidAmount.Text) || Convert.ToString(lblPaidAmount.Text) != "0.00")
                    {
                        //if (Convert.ToDecimal(lblPaidAmount.Text) < Convert.ToDecimal(lblBalancePaidAmount.Text))
                        //{
                        if (Convert.ToDecimal(lblcashamountpay.Text) > Convert.ToDecimal(lblBalancePaidAmount.Text))
                        {
                            i++;
                            //  msg = msg + "*In card payment, paid amount should be  equal to estimation amount" + "\n";
                            msg = msg + "*In card payment Paid amount should be less than or equal to  estimation amount" + "\n";
                        }
                    }
                    else
                    {
                        i++;
                        msg = msg + "*Please enter Paid amount" + "\n";
                        lblPaidAmount.Enabled = true;
                    }
                }
                if (rbtnCheque.Checked == true)
                {
                    if (!string.IsNullOrEmpty(lblPaidAmount.Text) || Convert.ToString(lblPaidAmount.Text) != "0.00")
                    {
                        //if (Convert.ToDecimal(lblPaidAmount.Text) < Convert.ToDecimal(lblBalancePaidAmount.Text))
                        //{

                        if (Convert.ToDecimal(lblcashamountpay.Text) > Convert.ToDecimal(lblBalancePaidAmount.Text))
                        {
                            i++;
                            //msg = msg + "*In cheque payment, paid amount should be  equal to estimation amount" + "\n";
                            msg = msg + "*In cheque payment Paid amount should be less than or equal to  estimation amount" + "\n";
                        }
                    }
                    else
                    {
                        i++;
                        msg = msg + "*Please enter paid amount" + "\n";
                        lblPaidAmount.Enabled = true;
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

        private void ddlBankAccount_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cashddl_KeyDown(object sender, KeyEventArgs e)
        {
            if (cashddl.CurrentCell.RowIndex == cashddl.Rows.Count - 1)
            {
                if (e.KeyData == Keys.Enter)
                {
                    cashddl.Rows[cashddl.CurrentCell.RowIndex].Cells[1].Selected = false;
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

        public void GetReport(string QuotationId)
        {
            //try
            //{
            if (!string.IsNullOrEmpty(QuotationId))
            {
               
                    using (SqlConnection con = new SqlConnection(Program.connection))
                    {
                        DataSet ds = new DataSet();
                        con.Open();


                        SqlCommand cmd = new SqlCommand("proc_GetReceiptReport_direct", con);
                        cmd.Parameters.AddWithValue("@ReceiptId", QuotationId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);


                     

                        Inventory.ReciptReport Obj = new ReciptReport();
                      
                        Obj.dsMain = ds;
                        if (Obj.GenerateQuoation())
                        {
                            
                        }


                    }
            }
            //}


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

        public void GetReceipt()
        {
            DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (dgvEstimationBill.RowCount > 0)
                {
                    for (int i = 0; i < dgvEstimationBill.RowCount; i++)
                    {
                        DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)dgvEstimationBill.Rows[i].Cells["chbPass"];
                        if (Convert.ToBoolean(CbxCell.Value) == true)
                        {
                            //int col = dgvEstimationBill.CurrentCell.ColumnIndex;
                            //int row = dgvEstimationBill.CurrentCell.RowIndex;

                            ReceiptId = Convert.ToString(dgvEstimationBill.Rows[i].Cells["Estimationid"].Value);
                            arr = new string[1];
                            arr[0] = Convert.ToString(AccountReceivableBAL.GetTransId(ReceiptId));
                            if (!string.IsNullOrEmpty(Convert.ToString(arr[0])))
                            {
                                GetReport(arr[0]);
                                break;

                                //ReceiptReportView objReceiptReportView = new ReceiptReportView("", arr[0]);
                                //objReceiptReportView.Show();


                            }

                        }

                    }

                }
                else
                {
                    MessageBox.Show("Receipt not available");
                }
            }
            clear();
            GetAccountReceivableBalance();
            dgvEstimationBill.DataSource = null;



        }

        private void lblPaidAmount_Enter(object sender, EventArgs e)
        {

        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (!string.IsNullOrEmpty(txtDiscount.Text))
            //    {

            //        double total = Convert.ToDouble(lblBalancePaidAmount.Text);
            //        if (string.IsNullOrEmpty(txtDiscount.Text))
            //        {
            //            txtDiscount.Text = "0";
            //        }

            //        double less = Convert.ToDouble(txtDiscount.Text);

            //        if (total < less)
            //        {
            //            MessageBox.Show("Less amount shold not be greater than actual amount");
            //            txtDiscount.Text = "0";
            //            txtDiscount.SelectionStart = 0;
            //            txtDiscount.SelectionLength = 1;
            //            txtDiscount.Focus();
                       

            //        }

            //        else
            //        {
                       
            //            double grandtotal = total  - less;
            //            lblPaidAmount.Text = grandtotal.ToString();
                      
            //        }
            //    }
               
            //}
            //catch
            //{

            //}

        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
        //    if (!(char.IsDigit(e.KeyChar)))
        //    {
        //        if (e.KeyChar != '\b')
        //        {
        //            e.Handled = true;
        //        }
        //    }
        }

       

        private void productsearchbttn_Click_1(object sender, EventArgs e)
        {
            GetAccountReceivableBalance();
            pnlprodsearch.Visible = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            panel9.Visible = false;
        }

        private void btnCardPanelOK_Click(object sender, EventArgs e)
        {
            if (validationPaymentCheque())
            {
                panelCard.Visible = false;
                btnAccountReceivableSave.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Cusname.Text = "";
            
            panel11.Visible = true;
            LoadPortsChecking();
            bindgrid();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dataGridView1.Columns[e.ColumnIndex].HeaderText == "Print")
                {
                   

                    string ReceiptId = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["TransId"].Value);

                    GetReport(ReceiptId);


                }
                if (dataGridView1.Columns[e.ColumnIndex].HeaderText == "Preview")
                {

                    using (SqlConnection con = new SqlConnection(Program.connection))
                    {
                        string ReceiptId = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["TransId"].Value);
                        DataSet ds = new DataSet();
                        con.Open();


                        SqlCommand cmd = new SqlCommand("proc_GetReceiptReport_direct", con);
                        cmd.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);




                        Inventory.ReciptReport Obj = new ReciptReport();

                        Obj.dsMain = ds;
                        if (Obj.GenerateQuoationReport())
                        {
                            frmPrintPreview objfrmpreview = new frmPrintPreview();
                            objfrmpreview.fileName = Obj.fileName;
                            
                            objfrmpreview.Show();
                        }


                    }


                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            panel11.Visible = false;
            
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {
                string Customer_Name = Cusname.Text.Trim();
                DateTime FromDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
                DateTime ToDate = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);
                DataSet serachdata = objQuotationBal.SearchAccountProduct(Customer_Name, FromDate, ToDate);             
                DataTable ds = serachdata.Tables[0];
                dataGridView1.DataSource = null;


                dataGridView1.Rows.Clear();
               

                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].Cells[0].Value = Convert.ToString(ds.Rows[i]["TransId"]);
                    dataGridView1.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["TransDate"]);
                    dataGridView1.Rows[i].Cells[2].Value = Convert.ToString(ds.Rows[i]["ReceiptId"]);
                    dataGridView1.Rows[i].Cells[3].Value = Convert.ToString(ds.Rows[i]["CustomerName"]);
                    dataGridView1.Rows[i].Cells[4].Value = Convert.ToString(ds.Rows[i]["Amount"]);
                    dataGridView1.Rows[i].Cells[5].Value = Convert.ToString(ds.Rows[i]["Mode"]);
                }
               // dataGridView1.DataSource = ds;
                
                foreach (DataGridViewColumn c in dataGridView1.Columns)
                {
                    c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
                }

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                dataGridView1.DefaultCellStyle.BackColor = Color.Gainsboro;
                dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                this.dataGridView1.Columns[1].Width = 100;
                this.dataGridView1.Columns[2].Width = 100;
                this.dataGridView1.Columns[3].Width = 320;
                this.dataGridView1.Columns[4].Width = 100;
                this.dataGridView1.Columns[5].Width = 70;
                this.dataGridView1.Columns[6].Width = 100;
                dataGridView1.Columns[0].Visible = false;
                //DataGridViewButtonColumn Print = new DataGridViewButtonColumn();
                //Print.HeaderText = "Print";
                //Print.Text = "Print";
                //Print.Name = "Print";
                //Print.FlatStyle = FlatStyle.Popup;
                //Print.UseColumnTextForButtonValue = true;
                //dataGridView1.Columns.Add(Print);
                //dataGridView1.Columns["Print"].Width = 80;
           
            }
            
            catch(Exception ex)
            {

            }
        }

        private void dataGridView1_CellBorderStyleChanged(object sender, EventArgs e)
        {

        }
       
    }
}
