using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventory
{
    public partial class frmInvoice : Form
    {


        string Val = "";
        public static string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        public frmInvoice( )
        {
           
            InitializeComponent();
        }

        private void frmInvoice_Load(object sender, EventArgs e)
        {
            lblCompany.Text = Program.Company;
            lblGST.Text = "Print " + Program.PrintInvoiceNumber;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.P))
            {
                GetdataPrint();

                return true;
            }
            if (keyData == (Keys.V))
            {
                GetdataPreview();

                return true;
            }
            if (radioButton1.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = radioButton2;
                    return true;
                }

            }
            if (radioButton2.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = radioButton3;
                    return true;
                }

            } 
            if (radioButton3.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = button1;
                    return true;
                }

            }
            if (button1.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = button2;
                    return true;
                }

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            GetdataPrint();
        }

        public string GetdataPrint()
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();


                // SqlCommand cmd = new SqlCommand("SalesBillPrint", con);
                SqlCommand cmd = new SqlCommand();
                //cmd.CommandType = CommandType.StoredProcedure;
                //SqlDataAdapter ad = new SqlDataAdapter(cmd);
                cmd.Parameters.AddWithValue("@id", Program.PrintInvoiceNumber);
                cmd.Parameters.AddWithValue("@company", Program.Company);
                //cmd.Parameters.AddWithValue("@CopyName", Program.CopyText);
                cmd.CommandType = CommandType.StoredProcedure;

                if (Program.Company == "R.R. ELECTRICAL AGENCIES")
                {
                    cmd.CommandText = "SalesBillPrint_1";
                }
                if (Program.Company == "R.R. PIPES")
                {
                    cmd.CommandText = "SalesPipesBillPrint_1";
                }
                cmd.Connection = con;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(ds);
                con.Close();
            }

            string path = "GSTInvoice.rpt";

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

            ds.Tables[1].TableName = "BillDetails";
            ds.Tables[0].TableName = "BillHeader";

            ReportDocument rd = new ReportDocument();
            rd.Load(path);

            if (radioButton1.Checked)
            {
                Val = radioButton1.Text;
                //Val = "DUPLICATE COPY";
            }
            else if (radioButton3.Checked)
            {
                Val = radioButton3.Text;

                // Val = "TRANSPORT COPY";
            }
            else if (radioButton2.Checked)
            {
                Val = radioButton2.Text;

                //Val = "ORGINAL COPY";
            }

            //ParameterFields paramFields = new ParameterFields();
            //ParameterField pfItemYr = new ParameterField();
            //pfItemYr.ParameterFieldName = "BillType"; //year is Crystal Report Parameter name.
            //ParameterDiscreteValue dcItemYr = new ParameterDiscreteValue();

            //pfItemYr.CurrentValues.Add(dcItemYr);
            //paramFields.Add(pfItemYr);
            //crystalReportViewer1.ParameterFieldInfo = paramFields;


            ParameterFields paramFields = new ParameterFields();
            ParameterField pfItemYr = new ParameterField();
            pfItemYr.ParameterFieldName = "BillType"; //year is Crystal Report Parameter name.
            ParameterDiscreteValue dcItemYr = new ParameterDiscreteValue();
            dcItemYr.Value = Val;
            pfItemYr.CurrentValues.Add(dcItemYr);
            paramFields.Add(pfItemYr);
            crystalReportViewer1.ParameterFieldInfo = paramFields;


            rd.SetDataSource(ds);
            crystalReportViewer1.ReportSource = rd;
        
            panel1.Visible = false;

            crystalReportViewer1.PrintReport();
            return "";
        }
        string StateVal = string.Empty;
        public string GetdataPreview()
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();

                StateVal = Program.State;
                // SqlCommand cmd = new SqlCommand("SalesBillPrint", con);
                SqlCommand cmd = new SqlCommand();
                //cmd.CommandType = CommandType.StoredProcedure;
                //SqlDataAdapter ad = new SqlDataAdapter(cmd);
                cmd.Parameters.AddWithValue("@id", Program.PrintInvoiceNumber);
                cmd.Parameters.AddWithValue("@company", Program.Company);
                //cmd.Parameters.AddWithValue("@CopyName", Program.CopyText);
                cmd.CommandType = CommandType.StoredProcedure;

                if (Program.Company == "R.R. ELECTRICAL AGENCIES")
                {
                    cmd.CommandText = "SalesBillPrint_1";
                }
                if (Program.Company == "R.R. PIPES")
                {
                    cmd.CommandText = "SalesPipesBillPrint_1";
                }
                cmd.Connection = con;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(ds);
                con.Close();
            }

            string path = "GSTInvoice.rpt";

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

            ds.Tables[1].TableName = "BillDetails";
            ds.Tables[0].TableName = "BillHeader";

            ReportDocument rd = new ReportDocument();
            rd.Load(path);

            if (radioButton1.Checked)
            {
                Val = radioButton1.Text;
                //Val = "DUPLICATE COPY";
            }
            else if (radioButton3.Checked)
            {
                Val = radioButton3.Text;

                // Val = "TRANSPORT COPY";
            }
            else if (radioButton2.Checked)
            {
                Val = radioButton2.Text;

                //Val = "ORGINAL COPY";
            }

            //ParameterFields paramFields = new ParameterFields();
            //ParameterField pfItemYr = new ParameterField();
            //pfItemYr.ParameterFieldName = "BillType"; //year is Crystal Report Parameter name.
            //ParameterDiscreteValue dcItemYr = new ParameterDiscreteValue();

            //pfItemYr.CurrentValues.Add(dcItemYr);
            //paramFields.Add(pfItemYr);
            //crystalReportViewer1.ParameterFieldInfo = paramFields;


            ParameterFields paramFields = new ParameterFields();
            ParameterField pfItemYr = new ParameterField();
            pfItemYr.ParameterFieldName = "BillType"; //year is Crystal Report Parameter name.
            ParameterDiscreteValue dcItemYr = new ParameterDiscreteValue();
            dcItemYr.Value = Val;
            pfItemYr.CurrentValues.Add(dcItemYr);
            paramFields.Add(pfItemYr);
            crystalReportViewer1.ParameterFieldInfo = paramFields;





         

           

            rd.SetDataSource(ds);
            crystalReportViewer1.ReportSource = rd;
            panel1.Visible = false;

            //crystalReportViewer1.PrintReport();
            return "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetdataPreview();
        }
    }
}
