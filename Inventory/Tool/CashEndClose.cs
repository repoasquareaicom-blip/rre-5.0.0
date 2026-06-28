using InvBal;
using Inventory.Report_Transaction;
using Inventory.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Tool
{
    public partial class CashEndClose : Form
    {
         CashEndCloseBAL ObjCashEndCloseBal = new CashEndCloseBAL();
        decimal TotalCashIn=0;
        public TextBox tb;
        public TextBox Quantitytomove;
        decimal TotalCashOut=0;
        DataTable dtCashIn;
        DataTable dtCashOut;
        DataSet dsin;
        DataTable dtCashInData;
        DataTable dtCashInData1;
        DataTable dtCashOutData;
        DataSet dsout;
        int[] Status = new int[2];
        int result = 0;
        Guid TransID;
        decimal BeginingCash, CashIn, CashOut, Addvalue, Minusvalue, OverShort;
        string EntitySource = string.Empty;
        string EntitySource1 = string.Empty;
        public CashEndClose()
        {
            InitializeComponent();
            panel2.Visible = false;
             this.WindowState = FormWindowState.Maximized;

             BindGrid();
             GetBalance();
           
             DateTime date1 =DateTime.Now.Date;
             dtpTodaydate.Text = Convert.ToString(date1.ToString("dd-MMM-yyyy"));
             txtCashinDrawer.Focus();
        }

        private void CashEndClose_Load(object sender, EventArgs e)
        {
            paymentdenobind();
            txtCashinDrawer.Focus(); 
        }

        public void GetBalance()
        {
            DataTable dt = CashEndCloseBAL.GetBeginingBalance();
            if ( dt.Rows.Count>0)
            {
                lblBeginingCash.Text = Convert.ToString(Convert.ToDecimal(dt.Rows[0]["Amount"]));
                DateTime date1 = Convert.ToDateTime(dt.Rows[0]["TransDate"]);
                dtpPreviouseClosure.Text = Convert.ToString(date1.ToString("dd-MMM-yyyy"));
                TransID = (Guid)dt.Rows[0]["TransId"];
            }
       
        }

        public void BindGrid()
        {
            dsin = CashEndCloseBAL.GetCashIn();
            dsout= CashEndCloseBAL.GetCashOut();
             dtCashIn = dsin.Tables[0];
             dtCashOut = dsout.Tables[0];
             dgvIn.DataSource = dsin.Tables[1];
             dgvOut.DataSource = dsout.Tables[1];
             dtCashInData = dsin.Tables[0];
             dtCashInData1 = dsin.Tables[0];
             dtCashOutData = dsout.Tables[0];
             if (dtCashOut.Rows.Count > 0)
             {
                 dgvCashOut.DataSource = dtCashOut;
                 dgvCashOut.Columns[2].Visible = false;
                 for (int i = 0; i < dgvCashOut.Rows.Count; i++)
                 {
                     TotalCashOut += Convert.ToDecimal(dgvCashOut.Rows[i].Cells["Amount"].Value);
                 }

                 txtCashOUT.Text = Convert.ToString(TotalCashOut);
                 lblTotalCashOut.Text = Convert.ToString(TotalCashOut);

                 this.dgvOut.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                 
             }

            if (dtCashIn.Rows.Count > 0)
            {
                dgvCashIn.DataSource = dtCashIn;
                dgvCashIn.Columns[2].Visible = false;
                for (int i = 0; i < dgvCashIn.Rows.Count; i++)
                {
                    TotalCashIn += Convert.ToDecimal(dgvCashIn.Rows[i].Cells["Amount"].Value);
                }
               
                lblTotalCashIn.Text = Convert.ToString(TotalCashIn);
                txtCashIN.Text = Convert.ToString(TotalCashIn);

                this.dgvIn.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                
            }
            DataTable dt = CashEndCloseBAL.GetBeginingBalance();
            if (dt.Rows.Count>0)
            {
                Addvalue = Convert.ToDecimal(dt.Rows[0]["Amount"]) + Convert.ToDecimal(lblTotalCashIn.Text);
                Minusvalue = Addvalue - Convert.ToDecimal(txtCashOUT.Text);
               
            }
            else
            {
                Addvalue = Convert.ToDecimal(lblTotalCashIn.Text);
                Minusvalue = Addvalue - Convert.ToDecimal(txtCashOUT.Text);
            }

            if (dtCashOut.Rows.Count > 0 && dgvCashIn.Rows.Count>0)
            {
                txtNetCASH.Text = Convert.ToString(Minusvalue);
            }
            else
            {
                txtNetCASH.Text = Convert.ToString(Minusvalue);
            }


            dgvIn.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvIn.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvIn.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvOut.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvOut.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvOut.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
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

        private void btncashpay_Click(object sender, EventArgs e)
        {
            decimal sum = 0;
            for (int i = 0; i < cashddl.Rows.Count; i++)
            {
                sum += Convert.ToDecimal(cashddl.Rows[i].Cells[2].Value);
            }
            txtCashinDrawer.Text = Convert.ToString(sum) + ".00";

            cashpl.Visible = false;

            if (Convert.ToDecimal(txtCashinDrawer.Text) >= Minusvalue)
            {
                OverShort = Convert.ToDecimal(txtCashinDrawer.Text) - Minusvalue;
            }
            else
            {
                OverShort = Minusvalue - Convert.ToDecimal(txtCashinDrawer.Text);
            }

            if (OverShort==0)
            {
                txtOverORShort.Text = "0.00";
                txtClosingBALANCE.Text = Convert.ToString(Convert.ToDecimal(txtOverORShort.Text) + Convert.ToDecimal(txtNetCASH.Text));
            }
            else
            {

            if (Convert.ToDecimal(txtCashinDrawer.Text) > Minusvalue)
            {
                txtOverORShort.Text = "+" + Convert.ToString(OverShort);
            }
            else
            {
                txtOverORShort.Text = "-" + Convert.ToString(OverShort);
            }

            }

            string s = txtOverORShort.Text;
            string[] SymbolPlusValue = s.Split('+');
            string[] SymbolMinusValue = s.Split('-');
            string SymbolPlus = s.Substring(0, 1);
            string SymbolMinus = s.Substring(0, 1);


            if (SymbolPlus == "+")
            {
                txtClosingBALANCE.Text = Convert.ToString(Convert.ToDecimal(SymbolPlusValue[1]) + Convert.ToDecimal(txtNetCASH.Text));
            }
            if (SymbolMinus == "-")
            {
                txtClosingBALANCE.Text = Convert.ToString(Convert.ToDecimal(txtNetCASH.Text) - Convert.ToDecimal(SymbolMinusValue[1]));
            }

            txtremark.Focus();
        }

        private void btnCloseDenominationPanel_Click(object sender, EventArgs e)
        {
            cashpl.Visible = false;
        }

        private void txtCashinDrawer_Enter(object sender, EventArgs e)
        {
            DataTable Getdata = CashEndCloseBAL.GetCashEndClose();
            if (Getdata.Rows.Count <= 0)
            {
            GetSystemDenomination();
            cashddl.Focus();
            cashddl.CurrentCell = cashddl[1,0];
            if (panel2.Visible == false)
            {
                panelAuthentication.Visible = true;
                txtCashClosePwd.Focus();
            }
                }
             

                else
                {
                    MessageBox.Show("Please Verify the CashEndCloseVerification");
                    
                }
          
        }

        public void GetSystemDenomination()
        {
            try
            {
                cashpl.Visible = true;
                DataTable paymentdo;
                DataTable dt = CashEndCloseBAL.GetSystemDenomination(TransID);



                if (dt.Rows.Count == 1)
                {
                    paymentdo = new DataTable();
                    paymentdo.Columns.Add("Denomination", typeof(string));
                    paymentdo.Columns.Add("Count", typeof(int));
                    paymentdo.Columns.Add("Amount", typeof(decimal));

                    paymentdo.Rows.Add("2000", Convert.ToInt64(dt.Rows[0]["TwoThousand"]), 2000 * Convert.ToDecimal(dt.Rows[0]["TwoThousand"]));
                    paymentdo.Rows.Add("1000", Convert.ToInt64(dt.Rows[0]["Thousand"]), 1000 * Convert.ToDecimal(dt.Rows[0]["Thousand"]));
                    paymentdo.Rows.Add("500", Convert.ToInt64(dt.Rows[0]["FiveHundred"]), 500 * Convert.ToDecimal(dt.Rows[0]["FiveHundred"]));
                    paymentdo.Rows.Add("100", Convert.ToInt64(dt.Rows[0]["Hundred"]), 100 * Convert.ToDecimal(dt.Rows[0]["Hundred"]));
                    paymentdo.Rows.Add("50", Convert.ToInt64(dt.Rows[0]["Fifty"]), 50 * Convert.ToDecimal(dt.Rows[0]["Fifty"]));
                    paymentdo.Rows.Add("20", Convert.ToInt64(dt.Rows[0]["Twenty"]), 20 * Convert.ToDecimal(dt.Rows[0]["Twenty"]));
                    paymentdo.Rows.Add("10", Convert.ToInt64(dt.Rows[0]["Ten"]), 10 * Convert.ToDecimal(dt.Rows[0]["Ten"]));
                    paymentdo.Rows.Add("5", Convert.ToInt64(dt.Rows[0]["Five"]), 5 * Convert.ToDecimal(dt.Rows[0]["Five"]));
                    paymentdo.Rows.Add("2", Convert.ToInt64(dt.Rows[0]["Two"]), 2 * Convert.ToDecimal(dt.Rows[0]["Two"]));
                    paymentdo.Rows.Add("1", Convert.ToInt64(dt.Rows[0]["One"]), 1 * Convert.ToDecimal(dt.Rows[0]["One"]));

                    dgvSystemdenomination.DataSource = paymentdo;

                    dgvSystemdenomination.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
                    dgvSystemdenomination.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
                    dgvSystemdenomination.DefaultCellStyle.BackColor = Color.Gainsboro;
                    dgvSystemdenomination.DefaultCellStyle.ForeColor = Color.Black;
                    dgvSystemdenomination.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                }

                else
                {
                    DataTable dtsys = paymentDeno();
                    dgvSystemdenomination.DataSource = dtsys;
                    dgvSystemdenomination.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 14.1F, FontStyle.Bold);
                    dgvSystemdenomination.DefaultCellStyle.Font = new Font("Tahoma", 16.1F, GraphicsUnit.Pixel);
                    dgvSystemdenomination.DefaultCellStyle.BackColor = Color.Gainsboro;
                    dgvSystemdenomination.DefaultCellStyle.ForeColor = Color.Black;
                    dgvSystemdenomination.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                }

                this.dgvSystemdenomination.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                this.dgvSystemdenomination.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dgvSystemdenomination.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                decimal sysSum = 0.00M;
                for (int i = 0; i < dgvSystemdenomination.Rows.Count; i++)
                {
                    sysSum += Convert.ToDecimal(dgvSystemdenomination.Rows[i].Cells[2].Value);
                }

                if (Convert.ToString(sysSum) == "0")
                {
                    txtsys.Text = "0.00";
                }
                else
                {
                    txtsys.Text = Convert.ToString(sysSum);
                }
            }
            catch(Exception ex)
            {

            }
            {

            }
        
        }

        private void cashddl_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void cashddl_CellClick(object sender, DataGridViewCellEventArgs e)
        {
          
        }

        private void cashddl_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            decimal sum = 0.00M;
            for (int i = 0; i < cashddl.Rows.Count; i++)
            {
                cashddl.Rows[i].Cells[2].Value = Convert.ToDecimal(cashddl.Rows[i].Cells[0].Value) * Convert.ToDecimal(cashddl.Rows[i].Cells[1].Value);
                 sum = sum+Convert.ToDecimal(cashddl.Rows[i].Cells[2].Value);
                 txtTotal.Text = Convert.ToString(sum);
            }

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                DialogResult result = MessageBox.Show("Do you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    this.Close();
                }
                return true;
            }

            if (txtremark.Focused)
            {
                if (keyData == Keys.Tab)
                {
                    btnCashEndCloseSave.Focus();
                    return true;
                }

            }


            return base.ProcessCmdKey(ref msg, keyData);

        }

        private void cashddl_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void cashddl_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = cashddl.CurrentCell.ColumnIndex;
            string headerText = cashddl.Columns[column].HeaderText;

            if (headerText.Equals("Count"))
            {
                
                tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    tb.TextChanged += new EventHandler(textbox_Change);
                    tb.KeyPress += new KeyPressEventHandler(Quantitytomove_press);
                    tb.MaxLength = 8;
                }
            }


           }
   
        

      private void textbox_Change(object sender, EventArgs e)
      {
          if (cashddl.CurrentCell.ColumnIndex == 0)
            {

            }
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

        private void btnCashEndCloseSave_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtCashinDrawer.Text) || Convert.ToString(txtCashinDrawer.Text) == "0.00")
            {
                MessageBox.Show("Enter Cash amount");
                txtCashinDrawer.Focus();
            }
            else
            {
               
                //if (dgvCashIn.Rows.Count > 0 && dgvCashOut.Rows.Count > 0)
                //{
                    DialogResult saveresult = MessageBox.Show("Do you want to save ?", "Save Closing Balance", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if(saveresult.Equals(DialogResult.Yes))
                    {

                        ObjCashEndCloseBal.Id = null;

                        string[] date = dtpTodaydate.Text.Split('-');
                        string IMonth = Convert.ToString(Convert.ToDateTime(dtpTodaydate.Text).ToString("MM"));

                        ObjCashEndCloseBal.AccountingDate = date[2] + "-" + IMonth + "-" + date[0];

                        ObjCashEndCloseBal.GrossBalance = Convert.ToDecimal(txtNetCASH.Text);
                        ObjCashEndCloseBal.Type = "C";
                        if (Convert.ToString(txtOverORShort.Text)=="0.00")
                        {
                            ObjCashEndCloseBal.Adjustment = "-0.00";
                        }
                        else
                        {
                            ObjCashEndCloseBal.Adjustment = Convert.ToString(txtOverORShort.Text);
                        }
                        

                        ObjCashEndCloseBal.ClosingBalance = Convert.ToDecimal(txtClosingBALANCE.Text);
                        ObjCashEndCloseBal.UserId = Program.userlevel;
                        ObjCashEndCloseBal.Remarks = txtremark.Text;
                        for(int i = 0; i < cashddl.Rows.Count; i++)
                        {
                            if (Convert.ToInt32(cashddl.Rows[i].Cells[0].Value) == 2000)
                            {
                                ObjCashEndCloseBal.TwoThousand = Convert.ToInt32(cashddl.Rows[i].Cells[2].Value);

                                ObjCashEndCloseBal.DTwoThousand = Convert.ToInt32(cashddl.Rows[i].Cells[1].Value);
                            }

                            if(Convert.ToInt32(cashddl.Rows[i].Cells[0].Value) == 1000)
                            {
                                ObjCashEndCloseBal.Thousand = Convert.ToInt32(cashddl.Rows[i].Cells[2].Value);

                                ObjCashEndCloseBal.DThousand = Convert.ToInt32(cashddl.Rows[i].Cells[1].Value);
                            }
                            if(Convert.ToInt32(cashddl.Rows[i].Cells[0].Value) == 500)
                            {
                                ObjCashEndCloseBal.FiveHundred = Convert.ToInt32(cashddl.Rows[i].Cells[2].Value);
                                ObjCashEndCloseBal.DFiveHundred = Convert.ToInt32(cashddl.Rows[i].Cells[1].Value);
                            }
                            if(Convert.ToInt32(cashddl.Rows[i].Cells[0].Value) == 100)
                            {
                                ObjCashEndCloseBal.Hundred = Convert.ToInt32(cashddl.Rows[i].Cells[2].Value);
                                ObjCashEndCloseBal.DHundred = Convert.ToInt32(cashddl.Rows[i].Cells[1].Value);
                            }
                            if(Convert.ToInt32(cashddl.Rows[i].Cells[0].Value) == 50)
                            {
                                ObjCashEndCloseBal.Fifty = Convert.ToInt32(cashddl.Rows[i].Cells[2].Value);
                                ObjCashEndCloseBal.DFifty = Convert.ToInt32(cashddl.Rows[i].Cells[1].Value);
                            }
                            if(Convert.ToInt32(cashddl.Rows[i].Cells[0].Value) == 20)
                            {
                                ObjCashEndCloseBal.Twenty = Convert.ToInt32(cashddl.Rows[i].Cells[2].Value);
                                ObjCashEndCloseBal.DTwenty = Convert.ToInt32(cashddl.Rows[i].Cells[1].Value);
                            }
                            if(Convert.ToInt32(cashddl.Rows[i].Cells[0].Value) == 10)
                            {
                                ObjCashEndCloseBal.Ten = Convert.ToInt32(cashddl.Rows[i].Cells[2].Value);
                                ObjCashEndCloseBal.DTen = Convert.ToInt32(cashddl.Rows[i].Cells[1].Value);
                            }
                            if(Convert.ToInt32(cashddl.Rows[i].Cells[0].Value) == 5)
                            {
                                ObjCashEndCloseBal.Five = Convert.ToInt32(cashddl.Rows[i].Cells[2].Value);
                                ObjCashEndCloseBal.DFive = Convert.ToInt32(cashddl.Rows[i].Cells[1].Value);
                            }
                            if(Convert.ToInt32(cashddl.Rows[i].Cells[0].Value) == 2)
                            {
                                ObjCashEndCloseBal.Two = Convert.ToInt32(cashddl.Rows[i].Cells[2].Value);
                                ObjCashEndCloseBal.DTwo = Convert.ToInt32(cashddl.Rows[i].Cells[1].Value);
                            }
                            if(Convert.ToInt32(cashddl.Rows[i].Cells[0].Value) == 1)
                            {
                                ObjCashEndCloseBal.One += Convert.ToInt32(cashddl.Rows[i].Cells[2].Value);
                                ObjCashEndCloseBal.DOne += Convert.ToInt32(cashddl.Rows[i].Cells[1].Value);
                            }
                        }
                       
                            Status = CashEndCloseBAL.SaveCashEndClose(ObjCashEndCloseBal);

                            for (int i = 0; i < dgvCashIn.Rows.Count; i++)
                            {
                                result = CashEndCloseBAL.UpdateDayEndCloseID(Status[1], "C", Convert.ToString(dgvCashIn.Rows[i].Cells[2].Value));
                            }
                            for (int i = 0; i < dgvCashOut.Rows.Count; i++)
                            {
                                result = CashEndCloseBAL.UpdateDayEndCloseID(Status[1], "D", Convert.ToString(dgvCashOut.Rows[i].Cells[2].Value));
                            }


                            if (Status[0] == 1)
                            {
                                MessageBox.Show("Saved Successfully");
                                Clear();
                                GetSystemDenomination();
                                BindGrid();
                                cashpl.Visible = false;
                                lblTotalCashIn.Text = "0.00";
                                lblTotalCashOut.Text = "0.00";
                                txtNetCASH.Text = "0.00";
                            }
                        }

                       

                   
                //}
                //else
                //{
                //    MessageBox.Show("No records in grid");
                //}

            }
        }
      

        private void btnCashEndCloseClaer_Click(object sender, EventArgs e)
        {
            Clear();  
        }

        public void Clear()
        {
            paymentdenobind();
            DataTable dt = paymentDeno();
            dgvSystemdenomination.DataSource = dt;
            dgvSystemdenomination.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSystemdenomination.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSystemdenomination.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            //GetSystemDenomination();
            //BindGrid();
            txtCashinDrawer.Text = string.Empty;
            txtremark.Text = string.Empty;
            txtClosingBALANCE.Text = "0.00";
            txtOverORShort.Text = "0.00";
            txtNetCASH.Text = "0.00";
            txtCashIN.Text="0.00";
            txtCashOUT.Text = "0.00";
            txtTotal.Text = "0.00";
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnPanelClose_Click(object sender, EventArgs e)
        {
            cashpl.Visible = false;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            Login ObjLogin = new Login();
            ObjLogin.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == false)
            {
                panelAuthentication.Visible = true;
                txtCashClosePwd.Focus();
            }
          
            this.ActiveControl = txtCashClosePwd;
            //panel2.Visible = true;
            //splitContainer1.Visible = true;
        }

        private void cashpl_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            CashcloseLogin();
        }

        public void CashcloseLogin()
        {
            if (string.IsNullOrEmpty(txtCashClosePwd.Text))
            {
                MessageBox.Show("Password should not be empty");
                this.ActiveControl = txtCashClosePwd;
                return;
            }
            DataTable dt = LoginBAL.GetCashClosedetails(txtCashClosePwd.Text);
            if (dt.Rows.Count > 0)
            {
                button2.Visible = true;
                button1.Visible = false;
                panel2.Visible = true;
                splitContainer1.Visible = true;
                panelAuthentication.Visible = false;
                txtCashClosePwd.Text = "";
                //lblless.Visible = true;
                //txtless.Visible = true;
                //pnlless.Visible = false;
                //pnlnet.Visible = true;
                //lblTotal.Visible = true;
                //this.ActiveControl = txtless;
            }
            else
            {
                MessageBox.Show("Authentication Failed");
                txtCashClosePwd.Text = "";
                this.ActiveControl = txtCashClosePwd;
                panel2.Visible = false;
                splitContainer1.Visible = false;
                if (panel2.Visible == false)
                {
                    panelAuthentication.Visible = true;
                    txtCashClosePwd.Focus();
                }
               
                //lblless.Visible = false;
                //txtless.Visible = false;
                //pnlnet.Visible = false;
                //lblTotal.Visible = false;

            }
        }
        private void btnCloseAuthentication_Click(object sender, EventArgs e)
        {
            panelAuthentication.Visible = false;
            txtCashClosePwd.Text = "";
        }

        private void txtCashClosePwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData==Keys.Enter)
            {
                CashcloseLogin();
            }
        }

        private void txtCashIN_Click(object sender, EventArgs e)
        {
            if (panel2.Visible==false)
            {
                panelAuthentication.Visible = true;
                txtCashClosePwd.Focus();
            }
                
          
        }

        private void txtCashOUT_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == false)
            {
                panelAuthentication.Visible = true;
                txtCashClosePwd.Focus();

            }
        }

        private void txtNetCASH_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == false)
            {
                panelAuthentication.Visible = true;
                txtCashClosePwd.Focus();
            }
        }

        private void txtOverORShort_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == false)
            {
                panelAuthentication.Visible = true;
                txtCashClosePwd.Focus();
            }
        }

        private void txtClosingBALANCE_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == false)
            {
                panelAuthentication.Visible = true;
                txtCashClosePwd.Focus();
            }
        }

        private void dgvIn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
            //    if (dgvIn.Columns[e.ColumnIndex].HeaderText == "Pay Now")
            //    {
            //       // panelPayable.Visible = true;
            
                panelIn.Visible = true;
                panelOut.Visible = false;
                    for (int i = 0; i < dgvIn.Rows.Count; i++)
                    {
                         EntitySource = Convert.ToString(dgvIn.Rows[e.RowIndex].Cells["Type"].Value);

                        GetBills(EntitySource, "C", "Proc_GetCashInBills");
                    }
                //}


            }
        }

        private void dgvOut_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                panelIn.Visible = false;
                panelOut.Visible = true;
                    for (int i = 0; i < dgvOut.Rows.Count; i++)
                    {
                         EntitySource1 = Convert.ToString(dgvOut.Rows[e.RowIndex].Cells["Type"].Value);
                        GetBills(EntitySource1, "D", "Proc_GetCashOutBills");
                    }
               


            }
        }



        public DataTable GetBillsData(string Entity, string Type, string ProcedureName)
        {
            DataTable dt = CashEndCloseBAL.GetBillDetails(Entity, Type, ProcedureName);
            return dt;
        }

        public void GetBills(string Entity,string Type,string ProcedureName)
        {
            DataTable dt = CashEndCloseBAL.GetBillDetails(Entity, Type, ProcedureName);
            if (Type=="C")
            {
                dgvInBills.DataSource=dt;
                dgvInBills.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                dgvInBills.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvInBills.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                this.dgvInBills.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //this.dgvInBills.Columns[0].Width = 1;
                //this.dgvInBills.Columns[1].Width = 2;
            }
            if (Type == "D")
            {
                dgvOutBills.DataSource = dt;
                dgvOutBills.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                dgvOutBills.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvOutBills.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                this.dgvOutBills.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
             
            }
             
        }

        private void InClose_Click(object sender, EventArgs e)
        {
            panelIn.Visible = false;
        }

        private void OutClose_Click(object sender, EventArgs e)
        {
            panelOut.Visible = false;
        }

        private void dgvInBills_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string ID = string.Empty;
            if (e.RowIndex >= 0)
            {

                for (int i = 0; i < dgvInBills.Rows.Count; i++)
                {
                     ID = Convert.ToString(dgvInBills.Rows[e.RowIndex].Cells["Bill Number"].Value);                    
                    
                }

                if (EntitySource.ToUpper() == "ESTIMATION".ToUpper())
                {
                    QuotationEstimationreport objEstimation = new QuotationEstimationreport(ID);
                    objEstimation.Show();
                }

              

            }
        }

        private void dgvOutBills_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string ID = string.Empty;
                if (e.RowIndex >= 0)
                {

                    for (int i = 0; i < dgvOutBills.Rows.Count; i++)
                    {
                        ID = Convert.ToString(dgvOutBills.Rows[e.RowIndex].Cells["Bill Number"].Value);

                    }

                    string Source = EntitySource1.Substring(0, 12);
                    if (Source.ToUpper() == "CASH REQUEST".ToUpper())
                    {
                        TransactionCashReport objCashRequestTransaction = new TransactionCashReport(ID);
                        objCashRequestTransaction.Show();
                    }

                }
            }
            catch
            {

            }
        }

        private void cashddl_KeyDown(object sender, KeyEventArgs e)
        {
            if (cashddl.CurrentCell.RowIndex == cashddl.Rows.Count - 1)
            {
                if (e.KeyData == Keys.Enter)
                {
                    cashddl.Rows[cashddl.CurrentCell.RowIndex].Cells[1].Selected = false;
                    btncashpay.Focus();
                  
                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {



            BindGridData();


            if (dtCashInData.Rows.Count > 0 || dtCashOutData.Rows.Count > 0)
            {
                try
                {

                    CashEndCloseReport objRREPrint = new CashEndCloseReport();
                    objRREPrint.dsMain = dtCashInData;
                    objRREPrint.dsMain2 = dtCashInData1;
                    objRREPrint.dsMain1 = dtCashOutData;
                    objRREPrint.strTypevalue = EntitySource;
                    objRREPrint.strTypevalue1 = EntitySource1;
                    objRREPrint.pagenumber = 1;
                    objRREPrint.status = true;
                    objRREPrint._strRefText = "Qtn:";
                   

                    objRREPrint.CashEndClosePrintStatemnet();
                }

                catch (Exception ex)
                {

                }
            }

        }

        public void BindGridData()
        {
            dsin = CashEndCloseBAL.GetCashIn();
            dsout = CashEndCloseBAL.GetCashOut();
            dtCashIn = dsin.Tables[0];
            dtCashInData = dsin.Tables[0];
            dtCashInData1 = dsin.Tables[0];
            dtCashOutData = dsout.Tables[0];
            dtCashOut = dsout.Tables[0];
        }

        public DataTable DataGridView2DataTable(DataGridView dgv, int minRow = 0)
        {

            DataTable dt = new DataTable();

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
