using CrystalDecisions.CrystalReports.Engine;
using InvBal;
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
    public partial class Salesbillreport : Form
    {
        string quno = string.Empty;
        int shop = Convert.ToInt32(null);
        string company = string.Empty;
       
        public static string Flag = System.Configuration.ConfigurationManager.AppSettings["PrePrinted"];
        QuotationBal ibjQuotationBal = new QuotationBal();
        public Salesbillreport(string s)
        {
            
            InitializeComponent();
            quno = s;
            shop = Convert.ToInt32(Program.ShopName);
            Loadreport(quno, shop);
           
        }
        public void Loadreport(string quno,int shop)
        {
            var path = "";
            Saledataset tcd = new Saledataset();
            shop = Convert.ToInt32(Program.ShopName);
            using (SqlConnection con = new SqlConnection(Program.connection))
            {

                con.Open();
                SqlCommand cmd = null;

                if (shop == 1)
                {

                    cmd = new SqlCommand("GetQuotationsalesreport", con);
                }

                if (shop == 2)
                {
                    cmd = new SqlCommand("GetQuotationsalesreportpipes", con);

                }
               // SqlCommand cmd = new SqlCommand("GetQuotationsalesreport", con);
                //SqlCommand cmd = new SqlCommand("GetQuotationsalesreportpipes", con);
                cmd.Parameters.AddWithValue("@id", quno);
                if (shop == 1)
                {

                     company = "R.R. ELECTRICAL AGENCIES";
                }

                if (shop == 2)
                {
                     company = "R.R. PIPES";

                }
                cmd.Parameters.AddWithValue("@company",company);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.SelectCommand = cmd;
                ad.Fill(tcd.Tables[0]);

                //ad.SelectCommand.CommandText = "Getsalesreportdetails";
                ad.SelectCommand.CommandText = "Getsalesreportdetails";
                cmd.CommandType = CommandType.StoredProcedure;
                ad.Fill(tcd.Tables[1]);
              
                //if (Flag.ToString().ToUpper()=="YES")
                //{
                //    path = @"Sales\SalesBillreportPrePrint.rpt";
                //}
                //else
                //{
                    path = @"Sales\Salesbillreport.rpt";
                //}
               
                
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
                salesreportviewer.ReportSource = rd;


            }



        }

        private void Salesbillreport_Load(object sender, EventArgs e)
        {

        }

        private void salesreportviewer_Load(object sender, EventArgs e)
        {

        }
    }
}
