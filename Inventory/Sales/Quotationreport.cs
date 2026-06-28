using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
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
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace Inventory.Sales
{
    public partial class Quotationreport : Form
    {
        string quno = string.Empty;
        QuotationBal ibjQuotationBal = new QuotationBal();
        public Quotationreport(string s)
        {
            InitializeComponent();
            quno = s;
            Loadreport(quno);
        }
        public void Loadreport(string quno)
        {
            Quotationdataset tcd = new Quotationdataset();
            using (SqlConnection con = new SqlConnection(Program.connection))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("GetQuotationreport", con);
                cmd.Parameters.AddWithValue("@id", quno);
                cmd.Parameters.AddWithValue("@companyname", Program.Company);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.SelectCommand = cmd;
                ad.Fill(tcd.Tables[0]);

                ad.SelectCommand.CommandText = "GetQuotationreportdetails";
                cmd.CommandType = CommandType.StoredProcedure;
                ad.Fill(tcd.Tables[1]);

                var path = @"Sales\Quotation.rpt";
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
                Quotationreportviewer.ReportSource = rd;

               // Quotationreportviewer.Height = 
               // Quotationreportviewer.Width = Unit.Pixel(300);
                //this.MaximumSize = new Size(100,200);
               // this.MinimumSize = new Size(100,200);
               // printReport.PrintOptions.PrinterDuplex = PrinterDuplex.Vertical;
              }
        }

        private void Quotationreport_Load(object sender, EventArgs e)
        {

        }
    }
}
