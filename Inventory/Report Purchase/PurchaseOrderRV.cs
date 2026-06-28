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

namespace Inventory.Report_Purchase
{
    public partial class PurchaseOrderRV : Form
    {
        public PurchaseOrderRV(string ordernumber)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            ReportPurchaseOrder(ordernumber);
        }

        public void ReportPurchaseOrder(string OrderNumber)
        {
            using (SqlConnection ObjConn3 = new SqlConnection(Program.connection))
            {
                SqlCommand sqlquery = new SqlCommand();
                sqlquery.Connection = ObjConn3;
                ObjConn3.Open();
                sqlquery.CommandType = CommandType.StoredProcedure;
                sqlquery.CommandText = "[Report_PurchaseOrder]";
                SqlParameter startdate;
                startdate = new SqlParameter("@OrderNumber", OrderNumber);
                sqlquery.Parameters.Add(startdate);
                SqlDataAdapter adp = new SqlDataAdapter(sqlquery);

                PurchaseOrderDDSS dd = new PurchaseOrderDDSS();
                adp.Fill(dd, "PurchaseOrder");

                ReportDocument rd = new ReportDocument();
                //rd.Load(@"C:\\Users\\swathika\\Desktop\\Latest 14 March\\GP1\DestinationStations\\Report\\Transport Report\\MoffesialCrystalReport.rpt");
                string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                System.IO.DirectoryInfo directoryInfo = System.IO.Directory.GetParent(appPath);
                System.IO.DirectoryInfo directoryInfo2 = System.IO.Directory.GetParent(directoryInfo.FullName);

                //string path = directoryInfo2 + @"\Report Transaction\PaymentCrystalReport.rpt";
                string path = directoryInfo2 + @"\Report Purchase\PurchaseOrderCR.rpt";
                rd.Load(path);
                rd.SetDataSource(dd.Tables["PurchaseOrder"]);
                //TCReportViewer.SelectionFormula = "{PaymentVoucher.Amount} = '10000'";
                PurchaseOrderCRV.ReportSource = rd; 
            }
        }

        private void PurchaseOrderRV_Load(object sender, EventArgs e)
        {

        }
    }
}
