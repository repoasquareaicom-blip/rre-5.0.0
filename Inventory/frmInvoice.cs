using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventory
{
    public partial class frmInvoice : Form
    {


        string Val = "";
        private static string Conn
        {
            get { return ConfigurationManager.ConnectionStrings["con"].ConnectionString; }
        }
        public frmInvoice( )
        {
           
            InitializeComponent();
        }

        private void frmInvoice_Load(object sender, EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            lblCompany.Text = Program.Company;
            lblGST.Text = "Print " + Program.PrintInvoiceNumber;
            ShowPrintOptions();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ShowPrintOptions();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (!DesignMode && panel1 != null && panel1.Visible)
            {
                CenterPrintOptions();
            }
        }

        private void ShowPrintOptions()
        {
            if (panel1 == null)
            {
                return;
            }

            if (crystalReportViewer1 != null)
            {
                crystalReportViewer1.Visible = false;
            }

            panel1.Visible = true;
            CenterPrintOptions();
            panel1.BringToFront();
            ActiveControl = radioButton1;
        }

        private void CenterPrintOptions()
        {
            if (panel1 == null)
            {
                return;
            }

            panel1.Left = (ClientSize.Width - panel1.Width) / 2;
            panel1.Top = (ClientSize.Height - panel1.Height) / 2;
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
            try
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

                PrepareInvoiceDataSet(ds);

                ReportDocument rd = LoadInvoiceReport();

                SetBillTypeParameter();

                SetReportDataSource(rd, ds);
                SetViewerReportSource(rd);

                panel1.Visible = false;

                crystalReportViewer1.PrintReport();
            }
            catch (Exception ex)
            {
                ShowPrintOptions();
                MessageBox.Show(GetFullErrorMessage(ex), "Invoice report error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "";
        }

        private ReportDocument LoadInvoiceReport()
        {
            string errors = "";

            try
            {
                ReportDocument rd = new GSTInvoice();
                ForceReportLoaded(rd);
                return rd;
            }
            catch (Exception ex)
            {
                errors += "Embedded GSTInvoice: " + GetFullErrorMessage(ex) + "\r\n\r\n";
            }

            foreach (string reportPath in GetInvoiceReportCandidates())
            {
                try
                {
                    if (!File.Exists(reportPath))
                    {
                        continue;
                    }

                    string loadPath = CopyReportToTemp(reportPath);
                    ReportDocument rd = new ReportDocument();
                    rd.Load(loadPath);
                    ForceReportLoaded(rd);
                    return rd;
                }
                catch (Exception ex)
                {
                    errors += reportPath + ":\r\n" + GetFullErrorMessage(ex) + "\r\n\r\n";
                }
            }

            throw new ApplicationException("Unable to load GST invoice report. Tried embedded report and report files.\r\n\r\n" + errors);
        }

        private void ForceReportLoaded(ReportDocument rd)
        {
            int sectionCount = rd.ReportDefinition.Sections.Count;
        }

        private void SetReportDataSource(ReportDocument rd, DataSet ds)
        {
            try
            {
                rd.SetDataSource(ds);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to set invoice data source.\r\n\r\n" + GetFullErrorMessage(ex), ex);
            }
        }

        private void SetViewerReportSource(ReportDocument rd)
        {
            try
            {
                crystalReportViewer1.Visible = true;
                crystalReportViewer1.ReportSource = rd;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to show invoice report in viewer.\r\n\r\n" + GetFullErrorMessage(ex), ex);
            }
        }

        private string[] GetInvoiceReportCandidates()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\"));
            string currentDirectory = Directory.GetCurrentDirectory();

            return new string[]
            {
                Path.Combine(Application.StartupPath, "GSTInvoice.rpt"),
                Path.Combine(projectDirectory, "GSTInvoice.rpt"),
                Path.Combine(projectDirectory, "17-08-2017GSTInvoice.rpt"),
                Path.Combine(projectDirectory, "dfdfdfdGSTInvoice.rpt"),
                Path.Combine(currentDirectory, "GSTInvoice.rpt"),
                Path.Combine(Path.Combine(currentDirectory, "Inventory"), "GSTInvoice.rpt")
            };
        }

        private string CopyReportToTemp(string reportPath)
        {
            string reportDirectory = Path.Combine(Path.GetTempPath(), "RRInventoryReports");
            Directory.CreateDirectory(reportDirectory);

            string tempReportPath = Path.Combine(reportDirectory, Path.GetFileName(reportPath));
            File.Copy(reportPath, tempReportPath, true);
            return tempReportPath;
        }

        private string GetFullErrorMessage(Exception ex)
        {
            string message = "";
            while (ex != null)
            {
                if (message.Length > 0)
                {
                    message += "\r\n";
                }

                message += ex.GetType().Name + ": " + ex.Message;
                ex = ex.InnerException;
            }

            return message;
        }

        private void PrepareInvoiceDataSet(DataSet ds)
        {
            if (ds.Tables.Count < 2)
            {
                throw new ApplicationException("Sales bill print did not return the required invoice tables. Please check the invoice number and company.");
            }

            ds.Tables[1].TableName = "BillDetails";
            ds.Tables[0].TableName = "BillHeader";
        }

        private void SetBillTypeParameter()
        {
            Val = GetSelectedCopyText();

            ParameterFields paramFields = new ParameterFields();
            ParameterField pfItemYr = new ParameterField();
            pfItemYr.ParameterFieldName = "BillType"; // Crystal Report parameter name.
            ParameterDiscreteValue dcItemYr = new ParameterDiscreteValue();
            dcItemYr.Value = Val;
            pfItemYr.CurrentValues.Add(dcItemYr);
            paramFields.Add(pfItemYr);
            crystalReportViewer1.ParameterFieldInfo = paramFields;
        }

        private string GetSelectedCopyText()
        {
            if (radioButton1.Checked)
            {
                return radioButton1.Text;
            }
            if (radioButton3.Checked)
            {
                return radioButton3.Text;
            }

            return radioButton2.Text;
        }
        string StateVal = string.Empty;
        public string GetdataPreview()
        {
            try
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

                PrepareInvoiceDataSet(ds);

                ReportDocument rd = LoadInvoiceReport();

                SetBillTypeParameter();

                SetReportDataSource(rd, ds);
                SetViewerReportSource(rd);
                panel1.Visible = false;
            }
            catch (Exception ex)
            {
                ShowPrintOptions();
                MessageBox.Show(GetFullErrorMessage(ex), "Invoice report error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetdataPreview();
        }
    }
}
