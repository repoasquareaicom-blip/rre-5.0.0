using InvBal;
using Inventory.Report_Transaction;
using Inventory.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Voucher;

namespace Inventory.Transactions
{
    public partial class CashPayment : Form
    {
        QuotationBal objQuotationbal = new QuotationBal();
        TransactionBAL ObjTransactionBAL = new TransactionBAL();
        string HdnCashRequestId = string.Empty;
        string HdnAmount = string.Empty;
        string clickstatus = string.Empty;
        string role = "Emp";
        string UserId = "1";
        string AccountHead = string.Empty;
        TextBox tbbaalanceanount;
        public CashPayment()
        {            
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            SearchCreteria1();
            SearchCreteria2();            
            LoadCrp();
            LoadDenomination();
            paymentDenotoCustomerbind();
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

        /* Left Side Content */

        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("RequestedBy");
            search.Add("Date");            
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderNo.DataSource = bs.DataSource;
            cbxSearchOrderNo.SelectedIndex = 0;
            cmbstatus1.Visible = false;
        }

        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("RequestedBy");
            search.Add("Date");            
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderDate.DataSource = bs.DataSource;
            cbxSearchOrderDate.SelectedIndex = 1;
            cmbstatus2.Visible = false;
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
                //this.dvgsearchcashpayment.Columns["RequestId"].Visible = true;
                //this.dvgsearchcashpayment.Columns["Amount"].Visible = true;
                //this.dvgsearchcashpayment.Columns["Status"].Visible = true;
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
                //this.dvgsearchcashpayment.Columns["RequestId"].Visible = true;
                //this.dvgsearchcashpayment.Columns["Amount"].Visible = true;
                //this.dvgsearchcashpayment.Columns["Status"].Visible = true;

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
        /* Right Side Content */
        private void LoadCrp()
        {
            DataTable dt = ObjTransactionBAL.BindTransactionCashApproval();
            if (dgvCRP.Columns.Count == 0 && dgvCRP.Rows.Count == 0)
            {

                dgvCRP.DataSource = dt;

                this.dgvCRP.Columns[0].Visible = false;

                this.dgvCRP.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                //this.dgvCRP.Columns[1].Width = 150;
                this.dgvCRP.Columns[1].ReadOnly = true;

                this.dgvCRP.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
                //this.dgvCRP.Columns[2].Width = 150;
                this.dgvCRP.Columns[2].ReadOnly = true;

                this.dgvCRP.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                //this.dgvCRP.Columns[3].Width = 350;
                this.dgvCRP.Columns[3].ReadOnly = true;

                this.dgvCRP.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
                //this.dgvCRP.Columns[4].Width = 100;
                this.dgvCRP.Columns[4].ReadOnly = true;
                this.dgvCRP.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                this.dgvCRP.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
                //this.dgvCRP.Columns[5].Width = 100;
                this.dgvCRP.Columns[5].ReadOnly = true;

                this.dgvCRP.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
                //this.dgvCRP.Columns[6].Width = 130;
                this.dgvCRP.Columns[6].ReadOnly = true;

                DataGridViewButtonColumn btn = new DataGridViewButtonColumn();

                btn.Text = "Pay Cash";
                btn.HeaderText = "Pay Cash";
                btn.Name = "PayCash";
                btn.UseColumnTextForButtonValue = true;
                btn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                btn.FlatStyle = FlatStyle.Popup;
                btn.CellTemplate.Style.BackColor = Color.Honeydew;
                dgvCRP.Columns.Add(btn);

                this.dgvCRP.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
               //this.dgvCRP.Columns[5].Width = 110;

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
            dgvCRP.Columns["RequestedBy"].Visible = false;

        }

        private void LoadDenomination()
        {
            try
            {

                //dgvDenomination.Rows.Clear();
                //dgvDenomination.ColumnCount = 5;
                //dgvDenomination.Columns[0].Name = "Denomination";
                //dgvDenomination.Columns[1].Name = "Count";
                //dgvDenomination.Columns[2].Name = "Amount";
                //dgvDenomination.Columns[3].Name = "DenominationID";
                //dgvDenomination.Columns[4].Name = "unique";





                DataTable dtDenom = new DataTable();
                //dtDenom.Columns.Add("Denomination", typeof(string));
                //dtDenom.Columns.Add("NoOfCurrency", typeof(string));
                //dtDenom.Rows.Add("1000", "");
                //dtDenom.Rows.Add("500", "");
                //dtDenom.Rows.Add("100", "");
                //dtDenom.Rows.Add("50", "");
                //dtDenom.Rows.Add("20", "");
                //dtDenom.Rows.Add("10", "");
                //dtDenom.Rows.Add("5", "");
                //dtDenom.Rows.Add("Coins", "");

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

                dgvDenomination.DataSource = paymentdo;
                this.dgvDenomination.Columns[0].ReadOnly = true;


                foreach (DataGridViewColumn column in dgvDenomination.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }


                //DataTable payment = new DataTable();

                //int row = paymentdo.Rows.Count;
                //dgvDenomination.Rows.Add(row);
                //if (row > 0)
                //{

                //    for (int i = 0; i < row; i++)
                //    {
                //        for (int j = 0; j < paymentdo.Columns.Count; j++)
                //        {
                //            dgvDenomination.Rows[i].Cells[j].Value = paymentdo.Rows[i][j];
                //        }

                //    }



                //}

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
            catch
            {

            }
        }

       

        private void a1Panel1_Click(object sender, EventArgs e)
        {
            //a1Panel1.Visible = false;
            //a1Panel2.Visible = false;
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
                txtamountrupe.Text = HdnAmount;
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
            }
            else if (rdtransfer.Checked)
            {
                txtamountrupe.Text = string.Empty;
                a1Panel2.Visible = false;                
                txtamount1.Enabled = false;
                txtbankname.Enabled = false;
                txtChqDate.Enabled = false;
                txtfromaccount.Enabled = true;
                txttoaccount.Enabled = true;
                this.ActiveControl = txtfromaccount;
            }

           
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData==Keys.Escape)
            {
                if (picClose.Visible)
                {
                picClose.PerformClick();
                return true;
                }
                else
                {
                    this.Close();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnpaid_Click(object sender, EventArgs e)
        {
            if (validation() == true)
            {
                SaveCashPay();
               
            }
        }



        public void GetReport(string QuotationId)
        {
            //try
            //{
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


                        SqlCommand cmd = new SqlCommand("GetCashRequestById", con);
                        cmd.Parameters.AddWithValue("@RequestId", QuotationId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);





                        VoucherDal Obj = new VoucherDal();
                        Obj.dsMain = ds;
                        if (Obj.GenerateQuoation())
                        {
                            frmPrintPreview objfrmpreview = new frmPrintPreview();
                            objfrmpreview.fileName = Obj.fileName;
                            objfrmpreview.Show();

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


        public void SaveCashPay()
        {

            if (rbCash.Checked)
            {


                if (txtamountrupe.Text == lblTotalCash.Text)
                {
                    save();
                    GetReport(HdnCashRequestId);
                    //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    //if (result == DialogResult.Yes)
                    //{
                    //    TransactionCashReport tcr = new TransactionCashReport(HdnCashRequestId);
                    //    tcr.ShowDialog();
                    //}
                    clear();
                }
                else
                {
                    MessageBox.Show("Cash and Denomination is not matched");
                    //lblTotalCash.ForeColor = Color.Red;
                }
            }
            else if (rdcheque.Checked)
            {
                save();
                GetReport(HdnCashRequestId);
                //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //if (result == DialogResult.Yes)
                //{
                //    TransactionCashReport tcr = new TransactionCashReport(HdnCashRequestId);
                //    tcr.ShowDialog();
                //}
                clear();
            }
            else if (rdtransfer.Checked)
            {
                save();
                GetReport(HdnCashRequestId);
                //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //if (result == DialogResult.Yes)
                //{
                //    TransactionCashReport tcr = new TransactionCashReport(HdnCashRequestId);
                //    tcr.ShowDialog();
                //}
                clear();
            }
               
            
        }

        public void save()
        {
            string mode = string.Empty;
            TransactionBAL ObjTranBAL = new TransactionBAL();
            if (!string.IsNullOrEmpty(HdnCashRequestId))
            {
                ObjTranBAL.CashRequestId = HdnCashRequestId;
                ObjTranBAL.Amount = HdnAmount;
                ObjTranBAL.oamount = lblTotalCash.Text;
                if (rbCash.Checked)
                {
                    mode = "Cash";
                    ObjTranBAL.TwoThousand = Convert.ToString(dgvDenomination.Rows[0].Cells[1].Value.ToString());
                    ObjTranBAL.Thousand = Convert.ToString(dgvDenomination.Rows[1].Cells[1].Value.ToString());
                    ObjTranBAL.FiveHundred = Convert.ToString(dgvDenomination.Rows[2].Cells[1].Value.ToString());
                    ObjTranBAL.Hundred = Convert.ToString(dgvDenomination.Rows[3].Cells[1].Value.ToString());
                    ObjTranBAL.Fifty = Convert.ToString(dgvDenomination.Rows[4].Cells[1].Value.ToString());
                    ObjTranBAL.Twenty = Convert.ToString(dgvDenomination.Rows[5].Cells[1].Value.ToString());
                    ObjTranBAL.Ten = Convert.ToString(dgvDenomination.Rows[6].Cells[1].Value.ToString());
                    ObjTranBAL.Five = Convert.ToString(dgvDenomination.Rows[7].Cells[1].Value.ToString());
                    ObjTranBAL.Coin = Convert.ToString(dgvDenomination.Rows[8].Cells[1].Value.ToString());
                    ObjTranBAL.One = Convert.ToString(dgvDenomination.Rows[9].Cells[1].Value.ToString());
                    ObjTranBAL.AccountHead = AccountHead;
                  
                    //ObjTranBAL.AccountHead=
                    //ObjTranBAL.ChequeDate = null;
                }
                else if (rdcheque.Checked)
                {
                    mode = "Cheque";
                    ObjTranBAL.ChequeNo = txtamount1.Text;
                    DateTime date = new DateTime(txtChqDate.Value.Year, txtChqDate.Value.Month, txtChqDate.Value.Day);
                    ObjTranBAL.ChequeDate = date.ToString("yyyy/MM/dd");
                    ObjTranBAL.BankName = txtbankname.Text;
                    ObjTranBAL.AccountHead = null;
                }
                else if (rdtransfer.Checked)
                {
                    mode = "Transfer";
                    ObjTranBAL.FromAccount = txtfromaccount.Text;
                    ObjTranBAL.ToAccount = txttoaccount.Text;
                    //ObjTranBAL.ChequeDate = null;
                    ObjTranBAL.AccountHead = null;
                }
                ObjTranBAL.Mode = mode;

                ObjTranBAL.Updatedby = Program.userid;

                 lbltransid.Text  = ObjTransactionBAL.SaveTransactionCashPayment(ObjTranBAL);
                 if (!string.IsNullOrEmpty(lbltransid.Text))
                {
                    //MessageBox.Show("Save Successfully");
                    LoadCrp();
                  
                    a1Panel1.Visible = false;
                    a1Panel2.Visible = false;
                    rbCash.Checked = false;
                    rdcheque.Checked = false;
                    rdtransfer.Checked = false;
                }
                

            }
        }

        public void clear()
        {
            txtamountrupe.Text = string.Empty;
            txtamount1.Text = string.Empty;
            txtbankname.Text = string.Empty;
            txtfromaccount.Text = string.Empty;
            txttoaccount.Text = string.Empty;
            lblTotalCash.Text = string.Empty;
            lblTotalCash.ForeColor = Color.Black;
            var today = DateTime.Today;
            txtChqDate.Value = today;
            lbltransid.Text = string.Empty;
            
            LoadDenomination();
            
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

        private void txtfromaccount_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txttoaccount_KeyPress(object sender, KeyPressEventArgs e)
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

        private void dgvDenomination_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Tot = 0;
                Double FinVal = CalcualteTotal();
                lblTotalCash.Text = FinVal.ToString() + ".00";
                if (txtamountrupe.Text != lblTotalCash.Text)
                {
                    lblTotalCash.ForeColor = Color.Red;
                }
                else
                {
                    lblTotalCash.ForeColor = Color.Black;
                }
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
            catch(Exception e)
            {

            }
            return Tot;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if ((cbxSearchOrderNo.SelectedIndex == cbxSearchOrderDate.SelectedIndex))
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
                    else if (firstname == "ApprovedBy")
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
                    else if (secondname == "ApprovedBy")
                    {
                        thirdname1 = secondname;
                        thirdvalue1 = secondvalue;
                    }



                    search(firstname1, firstvalue1, secondname1, secondvalue1, role, UserId);
                }
            }
            catch
            {
                dvgsearchcashpayment.DataSource = null;
            }
        }

        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string role, string UserId)
        {
            TransactionBAL obj = new TransactionBAL();
            DataTable dt = obj.searchcashPayment(firstname, firstvalue, secondname, secondvalue, role, UserId);
            dvgsearchcashpayment.Columns.Clear();
            dvgsearchcashpayment.DataSource = dt;

            dvgsearchcashpayment.Columns["Reason"].Visible = false;
            dvgsearchcashpayment.Columns["RequestId"].Visible = false;
            dvgsearchcashpayment.Columns["RequestedBy"].Width = 175;
            dvgsearchcashpayment.Columns["Date"].Width = 175;

            dvgsearchcashpayment.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dvgsearchcashpayment.DefaultCellStyle.BackColor = Color.Gainsboro;
            dvgsearchcashpayment.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dvgsearchcashpayment.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void btnuploadvoucher_Click(object sender, EventArgs e)
        {
            
            
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            picClose.Visible = false;
            a1Panel1.Visible = false;
            a1Panel2.Visible = false;
            panelCustomerpaid.Visible = false;
            rbCash.Checked = false;
            rdcheque.Checked = false;
            rdtransfer.Checked = false;
            txtamountrupe.Text = string.Empty;

            txtamount1.Text = string.Empty;
            txtbankname.Text = string.Empty;
            txtfromaccount.Text = string.Empty;
            txttoaccount.Text = string.Empty;
            dgvCustomerpaid.Rows.Clear();
            lblpaidbalance.Text = "0.00";
            paymentDenotoCustomerbind();


        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            if (validation() == true)
            {
                SaveCashPay();

            }
        }

        public bool validation()
        {
            int i = 0;
            bool status = true;
            string message = string.Empty;
            if (rbCash.Checked || rdcheque.Checked || rdtransfer.Checked)
            {
                if (rbCash.Checked)
                {
                    if (string.IsNullOrEmpty(txtamountrupe.Text))
                    {
                        i++;
                        message = message + "* Please Enter Amount" + "\n";
                        if (i == 1)
                            this.ActiveControl = txtamountrupe;
                    }
                }
                else if (rdcheque.Checked)
                {
                    if (string.IsNullOrEmpty(txtamount1.Text))
                    {
                        message = message + "* Please Enter Cheque No" + "\n";
                        if (i == 1)
                            this.ActiveControl = txtamount1;
                    }
                    if (string.IsNullOrEmpty(txtbankname.Text))
                    {
                        message = message + "* Please Enter BankName " + "\n";
                        if (i == 1)
                            this.ActiveControl = txtbankname;
                    }
                }
                else if (rdtransfer.Checked)
                {
                    if (string.IsNullOrEmpty(txtfromaccount.Text))
                    {
                        message = message + "* Please Enter From Account" + "\n";
                        if (i == 1)
                            this.ActiveControl = txtfromaccount;
                    }
                    if (string.IsNullOrEmpty(txttoaccount.Text))
                    {
                        message = message + "* Please Enter To Account " + "\n";
                        if (i == 1)
                            this.ActiveControl = txttoaccount;
                    }
                }

            }
            else
            {
                message = message + "* Please Select Payment Mode" + "\n";
                if (i == 1)
                    this.ActiveControl = a1Panel1;
            }

            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }

            return status;

        }

        private void btnprintvoucher_Click(object sender, EventArgs e)
        {

        }
        public void paymentDenotoCustomerbind()
        {
            dgvCustomerpaid.Rows.Clear();
            dgvCustomerpaid.ColumnCount = 5;
            dgvCustomerpaid.Columns[0].Name = "Denomination";
            dgvCustomerpaid.Columns[1].Name = "Count";
            dgvCustomerpaid.Columns[2].Name = "Amount";
            dgvCustomerpaid.Columns[3].Name = "DenominationID";
            dgvCustomerpaid.Columns[4].Name = "unique";


            this.dgvCustomerpaid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvCustomerpaid.Columns[0].Width = 150;
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
                //this.dgvCustomerpaid.Columns[1].ValueType = typeof(System.Double);
                //this.dgvCustomerpaid.Columns[2].ValueType = typeof(System.Double);


            }
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

        private void dgvDenomination_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (dgvDenomination.CurrentCell.RowIndex == 8)
                {
                    dgvDenomination.Rows[8].Cells[1].Selected = false;
                    Btnpaybalance.Focus();
                }
            }
        }

        private void dgvCRP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvCRP.Columns[e.ColumnIndex].HeaderText == "Pay Cash")
                {
                    HdnCashRequestId = Convert.ToString(dgvCRP.Rows[e.RowIndex].Cells["CashRequestId"].Value);
                    HdnAmount = Convert.ToString(dgvCRP.Rows[e.RowIndex].Cells["Amount"].Value);
                    AccountHead = Convert.ToString(dgvCRP.Rows[e.RowIndex].Cells["AccountHead"].Value);
                    a1Panel1.Visible = true;
                    picClose.Visible = true;
                }
            }
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
                dgvDenomination.Rows[i].Cells[2].Value = Convert.ToDecimal(dgvDenomination.Rows[i].Cells[0].Value) * Convert.ToDecimal(dgvDenomination.Rows[i].Cells[1].Value);
                sum = sum + Convert.ToDecimal(dgvDenomination.Rows[i].Cells[2].Value);
                lblTotalCash.Text = Convert.ToString(sum);
            }
        }

        private void CashPayment_Load(object sender, EventArgs e)
        {

        }

        private void Btnpaybalance_Click(object sender, EventArgs e)
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
                panelCustomerpaid.Visible = true;
                btnpaid.Enabled = false;
                dgvCustomerpaid.Focus();
               //a1Panel1.SendToBack();
               panelCustomerpaid.BringToFront();
                dgvCustomerpaid.CurrentCell = dgvCustomerpaid[1, 0];
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
                    //dgvCustomerpaid.Columns[2].DefaultCellStyle.Format = "N2";
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

        public void totalvalbalance()
        {
            double totalamount = 0;
            for (int i = 0; i < dgvCustomerpaid.Rows.Count; i++)
            {
                totalamount = totalamount + Convert.ToDouble(dgvCustomerpaid.Rows[i].Cells[2].Value);
            }

            lblpaidbalance.Text = String.Format("{0:00.00}", totalamount);


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

                    tbbaalanceanount.KeyPress += new KeyPressEventHandler(textbox2_keypress);
                }
            }
        }

        private void textbox2_keypress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }
        private void dgvCustomerpaid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (dgvCustomerpaid.CurrentCell.RowIndex == 8)
                {
                    dgvCustomerpaid.Rows[8].Cells[1].Selected = false;
                    lblcutomerpay.Focus();
                }
            }
        }

        private void lblcutomerpay_Click(object sender, EventArgs e)
        {
            double vals = Convert.ToDouble(lblTotalCash.Text) - Convert.ToDouble(txtamountrupe.Text);
            if (vals != Convert.ToDouble(lblpaidbalance.Text))
            {
                MessageBox.Show("Plese Enter Correct Balance");
                dgvCustomerpaid.Focus();
                dgvCustomerpaid.CurrentCell = dgvCustomerpaid[1, 0];
            }
            else
            {
                btnpaid.Enabled = true;

                save();
                SavepaymentDenomination();
                a1Panel1.Visible = false;
                panelCustomerpaid.Visible = false;
                GetReport(HdnCashRequestId);
                //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //if (result == DialogResult.Yes)
                //{
                //    TransactionCashReport tcr = new TransactionCashReport(HdnCashRequestId);
                //    tcr.ShowDialog();
                //}
            }
        }


        public void SavepaymentDenomination()
        {

            // Pnloading4.Visible = true;
            objQuotationbal.recivetransid = lbltransid.Text;
            //ReceiptId = lblid.Text;

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

            objQuotationbal.OAmount = "-" + lblpaidbalance.Text;

            string output = objQuotationbal.SavepaymentDenomination(objQuotationbal);

            if (output == "1")
            {
                //Pnloading4.Visible = false;
                clear();
            }

        }

        private void btnclear_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel15_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
