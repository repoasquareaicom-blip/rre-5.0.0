using CashTransactionReport;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Inventory.AccountReceivableReport;
using Inventory.Sales;
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
//using CashTransactionReport;
namespace Inventory.Accounts
{
    public partial class CashTransactionReportRV : Form
    {
        string flag = "";
        public CashTransactionReportRV()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            flag = Program.Company;
        }
        public void GetReport(string FromDate, string ToDate,string Flag)
        {  
            using (SqlConnection ObjConn3 = new SqlConnection(Program.connection))
            {
                try
                {
                    ObjConn3.Open();
                    SqlCommand cmd = new SqlCommand("Proc_GetAccountReport", ObjConn3);
                    AccountReceivableDS dd = new AccountReceivableDS();
                    cmd.Parameters.AddWithValue("@FromDate", FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", ToDate);
                    cmd.Parameters.AddWithValue("@Flag", Flag);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.SelectCommand = cmd;
                    ad.Fill(dd.Tables[0]);

                    ad.SelectCommand.CommandText = "Proc_GetAccountReport1";
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.Fill(dd.Tables[1]);

                    ad.SelectCommand.CommandText = "Proc_GetAddress";
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.Fill(dd.Tables[2]);

                    ad.SelectCommand.CommandText = "Proc_GetAccountCardReport";
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.Fill(dd.Tables[3]);

                    ad.SelectCommand.CommandText = "Proc_GetAccountCardReport1";
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.Fill(dd.Tables[4]);

                    ad.SelectCommand.CommandText = "Proc_GetAccountEstimationReport";
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.Fill(dd.Tables[5]);

                    ad.SelectCommand.CommandText = "Proc_GetAccountEstimationReport1";
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.Fill(dd.Tables[7]);

                    ad.SelectCommand.CommandText = "Proc_GetAccountPurchaseReport";
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.Fill(dd.Tables[6]);

                    ad.SelectCommand.CommandText = "Proc_GetAccountPurchaseReport1";
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.Fill(dd.Tables[8]);


                    //ad.SelectCommand.CommandText = "Proc_GetOpenningBalance";
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //ad.Fill(dd.Tables[2]);

                    //string [] arr=new string[4];
                    //arr[0] = @"\AccountReceivableReport\AccountReport.rpt";
                    //arr[1] = @"\AccountReceivableReport\CardTransaction.rpt";
                    //arr[2] = @"\AccountReceivableReport\Purchase.rpt";
                    //arr[3] = @"\AccountReceivableReport\Estimation.rpt";

                    ReportDocument rd = new ReportDocument();

                    string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                    System.IO.DirectoryInfo directoryInfo = System.IO.Directory.GetParent(appPath);
                    System.IO.DirectoryInfo directoryInfo2 = System.IO.Directory.GetParent(directoryInfo.FullName);

                    //string[] StrPath = new string[4];
                    //string FileName;

                    //for (int i = 0; i < arr.Length; i++)
                    //{
                    //    FileName = "Report" + i;
                    //   // ReportDocument rd = new ReportDocument();
                    //    StrPath[i] = directoryInfo2 + arr[i];
                    //    rd.Load(StrPath[i]);
                    //    rd.SetDataSource(dd);
                    //    this.AccountCRV.RefreshReport();       
                    //    rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, @"d:\\" + FileName + ".pdf");


                    //}




                    string path = directoryInfo2 + @"\AccountReceivableReport\AccountReport.rpt";
                    rd.Load(path);
                    rd.SetDataSource(dd);
                    AccountCRV.ReportSource = rd;
                    this.AccountCRV.RefreshReport();
                    // rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, @"d:\\TransactionReport.pdf");

                }
               

                catch(Exception ex)
                {

                }

            }
        }

        

        private void Btnprint_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(dtpFromDate.Text) && string.IsNullOrEmpty(dtpToDate.Text))
            {
                MessageBox.Show("Choose Date");
            }
            else
            {  
                string[] dFrom = new string[10];
                string[] dTo = new string[10];
               
                dFrom = dtpFromDate.Text.Split('-');
                dTo = dtpToDate.Text.Split('-');
                string FDate, TDate;
                FDate = dFrom[2] + "-" + dFrom[1] + "-" + dFrom[0];
                TDate = dTo[2] + "-" + dTo[1] + "-" + dTo[0];

                if (rbtnElectrical.Checked == true)
                {
                    flag = "1";
                }
                if (rbtnLights.Checked == true)
                {
                    flag = "2";
                }

                if (Convert.ToDateTime(FDate) <= Convert.ToDateTime(TDate))
                {
                    GetReport(FDate, TDate, flag);
                   // GetCashTransactionReport(FDate, TDate, flag);
                }
                else
                {
                    MessageBox.Show("From Date Must be Less Than To Date");
                }
               
                //AccountReceivableRV obj = new AccountReceivableRV(FDate,TDate);
                //obj.ShowDialog();
            }
            
            
        }

        private void CashTransactionReportRV_Load(object sender, EventArgs e)
        {

        }

        private void rbtnLights_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbtnElectrical_CheckedChanged(object sender, EventArgs e)
        {

        }

        public void GetCashTransactionReport(string FromDate, string ToDate, string Flag)
        {
            using (SqlConnection ObjConn3 = new SqlConnection(Program.connection))
            {
            try
            {
                
                    ObjConn3.Open();
                    SqlCommand cmd = new SqlCommand("Proc_TransactionReport", ObjConn3);
                    DataSet dd = new DataSet();
                    cmd.Parameters.AddWithValue("@FromDate", FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", ToDate);
                    cmd.Parameters.AddWithValue("@Flag", Flag);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.SelectCommand = cmd;
                    ad.Fill(dd);



                    //GetDataSet(dd)

                    CashTransactionReportDAL ObjCashTransaction = new CashTransactionReportDAL();
                    ObjCashTransaction.dsMain = dd;
                    if (ObjCashTransaction.GenerateTransactionReport())
                    {
                        //frmPrintPreview objfrmpreview = new frmPrintPreview();
                        //objfrmpreview.fileName = ObjCashTransaction.fileName;
                        //objfrmpreview.Show();

                    }
            }
                catch(Exception ex)
            {

                }
           
               // ObjCashTransaction.GetDataSet(dd);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dtpFromDate.Text) && string.IsNullOrEmpty(dtpToDate.Text))
            {
                MessageBox.Show("Choose Date");
            }
            else
            {
                string[] dFrom = new string[10];
                string[] dTo = new string[10];

                dFrom = dtpFromDate.Text.Split('-');
                dTo = dtpToDate.Text.Split('-');
                string FDate, TDate;
                FDate = dFrom[2] + "-" + dFrom[1] + "-" + dFrom[0];
                TDate = dTo[2] + "-" + dTo[1] + "-" + dTo[0];

                flag = Program.Company;

                //if (rbtnElectrical.Checked==true)
                //{
                //    flag = "1";
                //}
                //if (rbtnLights.Checked==true)
                //{
                //    flag = "2";
                //}

                if (Convert.ToDateTime(FDate) <= Convert.ToDateTime(TDate))
                {
                    // GetReport(FDate, TDate, flag);
                    GetCashTransactionReport(FDate, TDate, flag);
                }
                else
                {
                    MessageBox.Show("From Date Must be Less Than To Date");
                }

                //AccountReceivableRV obj = new AccountReceivableRV(FDate,TDate);
                //obj.ShowDialog();
            }
        }
    }
}
