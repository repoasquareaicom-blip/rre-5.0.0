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

namespace Inventory.Report
{
    public partial class ReceiptReportView : Form
    {
        string quno = string.Empty;
        string quno1 = string.Empty;
        public ReceiptReportView(string s, string ReceiptId1)
        {
            InitializeComponent();
            quno = s;
            quno1 = ReceiptId1;
            Loadreport(quno);
        }

        private void ReceiptReportView_Load(object sender, EventArgs e)
        {

        }
        public void Loadreport(string quno)
        {
            ReceiptDS tcd = new ReceiptDS();
            using (SqlConnection con = new SqlConnection(Program.connection))
            {

                con.Open();
                if (string.IsNullOrEmpty(quno1))
                {
                SqlCommand cmd = new SqlCommand("proc_GetReceiptReport", con);
                cmd.Parameters.AddWithValue("@ReceiptId", quno);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.SelectCommand = cmd;
                ad.Fill(tcd.Tables[0]);
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("proc_GetReceiptReport1", con);
                    cmd.Parameters.AddWithValue("@ReceiptId", quno1);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.SelectCommand = cmd;
                    ad.Fill(tcd.Tables[0]);
                }

                var path = @"Report\Receipt.rpt";
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
                Receipteportviewer.ReportSource = rd;
                


            }



        }
    }
}
