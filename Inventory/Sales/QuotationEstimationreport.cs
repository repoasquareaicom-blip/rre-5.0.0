using CrystalDecisions.CrystalReports.Engine;
using InvBal;
//using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace Inventory.Sales
{
    public partial class QuotationEstimationreport : Form
    {
        string quno = string.Empty;
        QuotationBal ibjQuotationBal = new QuotationBal();
        public QuotationEstimationreport(string s)
        {
            InitializeComponent();
            quno = s;
            Loadreport(quno);
        }
        public void Loadreport(string quno)
        {
            EstimationDS tcd = new EstimationDS();
            using (SqlConnection con = new SqlConnection(Program.connection))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("GetQuotationEstimationreport", con);
                cmd.Parameters.AddWithValue("@id", quno);
                cmd.Parameters.AddWithValue("@companyname", Program.Company);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.SelectCommand = cmd;
                ad.Fill(tcd.Tables[1]);

                ad.SelectCommand.CommandText = "GetQuotationEstimationreportdetails";
                cmd.CommandType = CommandType.StoredProcedure;
                ad.Fill(tcd.Tables[0]);

                var path = @"Sales\QuotationEstimationrpt.rpt";
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
//                CrystalDecisions.CrystalReports.Engine.ReportDocument doc = rd;
//            doc.DataDefinition.FormulaFields["yourformulaname"].Text = "your value";
////or you can directly set the visibility of your section from code behind on the basis of your business logc as
// doc.ReportDefinition.Sections["sectionnameOrIndex"].SectionFormat.EnableSuppress = true;
                rd.Load(path);
                rd.SetDataSource(tcd);
                Quotationreportviewer.ReportSource = rd;
            }
        }

        private void QuotationEstimationreport_Load(object sender, EventArgs e)
        {

        }
    }
}
