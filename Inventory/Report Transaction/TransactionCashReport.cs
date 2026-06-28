using CrystalDecisions.CrystalReports.Engine;
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

namespace Inventory.Report_Transaction
{
    public partial class TransactionCashReport : Form
    {
        string flag = "1";
        public TransactionCashReport(string ReqId)
        {
            InitializeComponent();
            BindData(ReqId);
        }

        public void BindData(string ReqId)
        {
            using (SqlConnection ObjConn3 = new SqlConnection(Program.connection))
            {
                SqlCommand sqlquery = new SqlCommand();
                sqlquery.Connection = ObjConn3;
                ObjConn3.Open();
                sqlquery.CommandType = CommandType.StoredProcedure;
                sqlquery.CommandText = "[GetCashRequestById]";
                SqlParameter startdate;
                startdate = new SqlParameter("@RequestId", ReqId);                
                sqlquery.Parameters.Add(startdate);                
                SqlDataAdapter adp = new SqlDataAdapter(sqlquery);

                Payment dd = new Payment();
                adp.Fill(dd.Tables[0]);
                

                ReportDocument rd = new ReportDocument();
                //rd.Load(@"C:\\Users\\swathika\\Desktop\\Latest 14 March\\GP1\DestinationStations\\Report\\Transport Report\\MoffesialCrystalReport.rpt");
                string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                System.IO.DirectoryInfo directoryInfo = System.IO.Directory.GetParent(appPath);
                System.IO.DirectoryInfo directoryInfo2 = System.IO.Directory.GetParent(directoryInfo.FullName);

                string path = directoryInfo2 + @"\Report Transaction\PaymentCrystalReport.rpt";
                rd.Load(path);
                rd.SetDataSource(dd.Tables["PaymentVoucher"]);
                TCReportViewer.ReportSource = rd;


            }
        }

        

        private void TransactionCashReport_Load(object sender, EventArgs e)
        {

        }


    }
}
