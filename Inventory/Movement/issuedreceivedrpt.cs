using CrystalDecisions.CrystalReports.Engine;
using System;
using InvBal;
//using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Data.SqlClient;

namespace Inventory.Movement
{
    public partial class issuedreceivedrpt : Form
    {
        FrmIssuedReceivedReportBAL ibjFrmIssuedReceivedReportBAL = new FrmIssuedReceivedReportBAL();
        string quno = string.Empty;

        public issuedreceivedrpt(string s)
        {  
            InitializeComponent();
            quno = s;
            Loadreport(quno);
        }
        public void Loadreport(string quno)
        {
            issuedreceived1 tcd = new issuedreceived1();
            using (SqlConnection con = new SqlConnection(Program.connection))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("IssuedReceivedReport", con);
                cmd.Parameters.AddWithValue("@customerid", quno);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.SelectCommand = cmd;
                ad.Fill(tcd.Tables[0]);

               // ad.SelectCommand.CommandText = "GetQuotationreportdetails";
              

                var path = @"Movement\issuedreceivedreport.rpt";
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
                IssuedReceivedreportviewer.ReportSource = rd;


            }



        }

        private void issuedreceivedrpt_Load(object sender, EventArgs e)
        {

        }
    }
}
