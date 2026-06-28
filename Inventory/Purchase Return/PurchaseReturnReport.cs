using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Purchase_Return
{
    public partial class PurchaseReturnReport : Form
    {
        string ReturnId = string.Empty;
        public PurchaseReturnReport(string PurchaseReturnId)
        {
            InitializeComponent();
            ReturnId = PurchaseReturnId;
            Loadreport(ReturnId);

        }
        public void Loadreport(string ReturnId)
        {
            ReportPurchaseReturnDS tcd = new ReportPurchaseReturnDS();

            using (SqlConnection con = new SqlConnection(Program.connection))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Proc_GetPurchaseReturnHeaderReport", con);
                cmd.Parameters.AddWithValue("@PurchaseRtnID", ReturnId);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.SelectCommand = cmd;
                ad.Fill(tcd.Tables[0]);

                ad.SelectCommand.CommandText = "Proc_GetPurchaseReturndetailsReport";
                cmd.CommandType = CommandType.StoredProcedure;
                ad.Fill(tcd.Tables[1]);

                var path = @"Purchase Return\PurchaseReturnReport.rpt";
                var currentDir = System.IO.Directory.GetCurrentDirectory();
                if (currentDir.ToLower().EndsWith(@"\bin\debug") ||
                    currentDir.ToLower().EndsWith(@"\bin\release"))
                {

                    path = System.IO.Path.GetFullPath(@"..\..\" + path);
                }
                else
                {
                    path = System.IO.Path.GetFullPath(path);
                }

                ReportDocument rd = new ReportDocument();
               
                rd.Load(path);
                rd.SetDataSource(tcd);
                RptViewerPurchaseReturn.ReportSource = rd;


            }



        }
        private void PurchaseReturnReport_Load(object sender, EventArgs e)
        {

        }
    }
}
