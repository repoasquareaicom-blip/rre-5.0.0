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
    public partial class Requestlist : Form
    {
        QuotationBal objQuotationbal = new QuotationBal();
        public Requestlist()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            GetSearchSalesOrder();
        }

        private void GetSearchSalesOrder()
        {

            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);

            //dgvSearch.Rows.Clear();
            //dgvSearch.Columns.Clear();

            DataTable dt = objQuotationbal.searchcashrequestlist(FromDate, ToDate);


            if(dt.Rows.Count>0)
            {


                dgvSearch.Rows.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dgvSearch.Rows.Add();
                    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["RequestId"]);
                    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Requested"]);
                    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Date"]);
                    dgvSearch.Rows[i].Cells[3].Value = Convert.ToDecimal(dt.Rows[i]["Amount"]);
                    dgvSearch.Rows[i].Cells[4].Value = Convert.ToString(dt.Rows[i]["Reason"]);
                }
          //  dgvSearch.DataSource = dt;
            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvSearch.Columns["RequestedPerson"].Visible = false;
           dgvSearch.Columns["Requestid"].Width = 120;
           dgvSearch.Columns["RequestedPerson"].Width = 180;
           dgvSearch.Columns["Date"].Width = 100;
           dgvSearch.Columns["Amount"].Width = 100;
           dgvSearch.Columns["Reason"].Width = 400;

           dgvSearch.Columns["Print"].Width = 50;
          // dgvSearch.Columns["Delete"].Width = 50;

            //DataGridViewButtonColumn Print = new DataGridViewButtonColumn();
         
            //Print.HeaderText = "Print";
            //Print.Text = "Print";
            //Print.Name = "Print";
            //Print.FlatStyle = FlatStyle.Popup;
            //Print.UseColumnTextForButtonValue = true;
            //dgvSearch.Columns.Add(Print);

          
            }
            else
            {
                dgvSearch.Rows.Clear();
            }

        }

        private void btnsech_Click(object sender, EventArgs e)
        {
            GetSearchSalesOrder();
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
                            //frmPrintPreview objfrmpreview = new frmPrintPreview();
                            //objfrmpreview.fileName = Obj.fileName;
                            //objfrmpreview.Show();

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

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex>=0)
            {
                if (dgvSearch.Columns[e.ColumnIndex].HeaderText=="Print")
                {
                    //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    string HdnCashRequestId=Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["RequestId"].Value);

                    GetReport(HdnCashRequestId);
                    //if (result == DialogResult.Yes)
                    //{
                    //    TransactionCashReport tcr = new TransactionCashReport(HdnCashRequestId);
                    //    tcr.ShowDialog();
                    //}




                }
           
            }
        }

      
    }
}
