using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Inventory.Sales
{
    public class QuotationGSTReport : Form
    {
        private readonly string quotationId;
        private readonly CrystalReportViewer reportViewer;

        private static string Conn
        {
            get { return ConfigurationManager.ConnectionStrings["con"].ConnectionString; }
        }

        public QuotationGSTReport(string quotationId)
        {
            this.quotationId = quotationId;
            Text = "Quotation GST Report";
            WindowState = FormWindowState.Maximized;

            reportViewer = new CrystalReportViewer();
            reportViewer.Dock = DockStyle.Fill;
            reportViewer.ToolPanelView = ToolPanelViewType.None;
            Controls.Add(reportViewer);

            Load += QuotationGSTReport_Load;
        }

        private void QuotationGSTReport_Load(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = BuildReportDataSet();
                ReportDocument rd = LoadInvoiceReport();
                SetBillTypeParameter();
                rd.SetDataSource(ds);
                reportViewer.ReportSource = rd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(GetFullErrorMessage(ex), "Quotation GST report error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private DataSet BuildReportDataSet()
        {
            DataTable header = GetHeader();
            if (header.Rows.Count == 0)
            {
                throw new ApplicationException("Quotation not found: " + quotationId);
            }

            DataTable customer = GetCustomer(GetValue(header.Rows[0], "Customerid"));
            DataTable details = GetDetails();
            if (details.Rows.Count == 0)
            {
                throw new ApplicationException("Quotation details not found: " + quotationId);
            }

            Inventory.Invoice ds = new Inventory.Invoice();
            DataRow headerRow = ds.BillHeader.NewRow();
            FillHeaderRow(headerRow, header.Rows[0], customer.Rows.Count > 0 ? customer.Rows[0] : null, details);
            ds.BillHeader.Rows.Add(headerRow);

            int sno = 1;
            foreach (DataRow detail in details.Rows)
            {
                DataRow detailRow = ds.BillDetails.NewRow();
                FillDetailRow(detailRow, detail, sno);
                ds.BillDetails.Rows.Add(detailRow);
                sno++;
            }

            return ds;
        }

        private DataTable GetHeader()
        {
            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM QuotationHeader WHERE Quotationid = @Quotationid", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Quotationid", quotationId);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        private DataTable GetCustomer(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                return new DataTable();
            }

            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM Customers WHERE CustomerID = @CustomerID", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        private DataTable GetDetails()
        {
            string sql = @"
SELECT qd.*,
       pm.DisplayName,
       pm.ItemName,
       pm.HSNCODE,
       pm.GST,
       pm.UOM
FROM QuotationDetails qd
LEFT JOIN ProductMaster pm ON CONVERT(varchar(50), pm.id) = qd.Productid
WHERE qd.Quotationid = @Quotationid
ORDER BY qd.sino";

            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Quotationid", quotationId);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        private void FillHeaderRow(DataRow row, DataRow quotation, DataRow customer, DataTable details)
        {
            decimal totalTaxable = 0;
            decimal totalGst = 0;
            decimal totalAmount = 0;
            decimal taxable5 = 0;
            decimal taxable12 = 0;
            decimal taxable18 = 0;
            decimal taxable28 = 0;
            decimal gst5 = 0;
            decimal gst12 = 0;
            decimal gst18 = 0;
            decimal gst28 = 0;

            foreach (DataRow detail in details.Rows)
            {
                decimal taxable = GetAmount(detail);
                decimal gstRate = ToDecimal(GetValue(detail, "GST"));
                decimal gstAmount = Decimal.Round(taxable * gstRate / 100, 2);

                totalTaxable += taxable;
                totalGst += gstAmount;
                totalAmount += taxable + gstAmount;

                if (gstRate == 5)
                {
                    taxable5 += taxable;
                    gst5 += gstAmount;
                }
                else if (gstRate == 12)
                {
                    taxable12 += taxable;
                    gst12 += gstAmount;
                }
                else if (gstRate == 18)
                {
                    taxable18 += taxable;
                    gst18 += gstAmount;
                }
                else if (gstRate == 28)
                {
                    taxable28 += taxable;
                    gst28 += gstAmount;
                }
            }

            Set(row, "CompanyName", Program.Company);
            Set(row, "COMPANYADDRESS", "");
            Set(row, "BILLTYPE", "QUOTATION");
            Set(row, "Cst", "");
            Set(row, "PHONE1", "");
            Set(row, "Billno", quotationId);
            Set(row, "salesdate", FormatDate(GetValue(quotation, "date")));
            Set(row, "CustomerName", FirstValue(quotation, customer, "customename", "customemame", "Name"));
            Set(row, "Customeraddress1", FirstValue(customer, null, "Address1", "Address"));
            Set(row, "City", FirstValue(quotation, customer, "City", "CITY"));
            Set(row, "Customerphone", FirstValue(customer, null, "Phone", "Mobile"));
            Set(row, "TOTAL12", FormatAmount(gst12));
            Set(row, "TOTAL18", FormatAmount(gst18));
            Set(row, "TOTAL28", FormatAmount(gst28));
            Set(row, "TOTAL12TAXABLE", FormatAmount(taxable12));
            Set(row, "TOTAL18TAXABLE", FormatAmount(taxable18));
            Set(row, "TOTAL28TAXABLE", FormatAmount(taxable28));
            Set(row, "TOTALTAXABLE", FormatAmount(totalTaxable));
            Set(row, "TOTALGST", FormatAmount(totalGst));
            Set(row, "CGST", FormatAmount(totalGst / 2));
            Set(row, "SGST", FormatAmount(totalGst / 2));
            Set(row, "TOTALAMOUNT", FormatAmount(totalAmount));
            Set(row, "LESSAMOUNT", "0.00");
            Set(row, "NETAMOUNT", FormatAmount(totalAmount));
            Set(row, "GST", "");
            Set(row, "NumberInwords", "");
            Set(row, "CustomerTin", FirstValue(customer, null, "Tin", "GSTNo", "GST"));
            Set(row, "Bank", "");
            Set(row, "ACNO", "");
            Set(row, "IFSC", "");
            Set(row, "BRANCH", "");
            Set(row, "PAN", "");
            Set(row, "Discliamer", "");
            Set(row, "TOTAL5", FormatAmount(gst5));
            Set(row, "TOTAL5TAXABLE", FormatAmount(taxable5));
            Set(row, "CGST5", FormatAmount(gst5 / 2));
            Set(row, "SGSTT5", FormatAmount(gst5 / 2));
            Set(row, "CGST12", FormatAmount(gst12 / 2));
            Set(row, "SGSTT12", FormatAmount(gst12 / 2));
            Set(row, "CGST18", FormatAmount(gst18 / 2));
            Set(row, "SGSTT18", FormatAmount(gst18 / 2));
            Set(row, "CGST28", FormatAmount(gst28 / 2));
            Set(row, "SGSTT28", FormatAmount(gst28 / 2));
            Set(row, "CustomerCity", FirstValue(quotation, customer, "City", "CITY"));
            Set(row, "IGST", "0.00");
            Set(row, "IGST5", "0.00");
            Set(row, "IGST12", "0.00");
            Set(row, "IGST18", "0.00");
            Set(row, "IGST28", "0.00");
            Set(row, "RoundOff", "0.00");
            Set(row, "DiscountPer", "0.00");
        }

        private void FillDetailRow(DataRow row, DataRow detail, int sno)
        {
            decimal taxable = GetAmount(detail);
            decimal gstRate = ToDecimal(GetValue(detail, "GST"));
            decimal gstAmount = Decimal.Round(taxable * gstRate / 100, 2);

            Set(row, "Sno", sno.ToString(CultureInfo.InvariantCulture));
            Set(row, "ItemName", FirstValue(detail, null, "DisplayName", "ItemName", "ProductName"));
            Set(row, "HSN", GetValue(detail, "HSNCODE"));
            Set(row, "Quantity", GetValue(detail, "Quantity"));
            Set(row, "TRate", FormatAmount(ToDecimal(GetValue(detail, "Rate"))));
            Set(row, "T12", gstRate == 12 ? FormatAmount(gstAmount) : "");
            Set(row, "T18", gstRate == 18 ? FormatAmount(gstAmount) : "");
            Set(row, "T28", gstRate == 28 ? FormatAmount(gstAmount) : "");
            Set(row, "TAXABLEVALUE", FormatAmount(taxable));
            Set(row, "TAX", FormatAmount(gstAmount));
            Set(row, "VAT", FormatAmount(gstRate));
            Set(row, "Test", "");
            Set(row, "Copy", "Original");
            Set(row, "T5", gstRate == 5 ? FormatAmount(gstAmount) : "");
        }

        private decimal GetAmount(DataRow row)
        {
            decimal amount = ToDecimal(GetValue(row, "Amount"));
            if (amount != 0)
            {
                return amount;
            }

            return ToDecimal(GetValue(row, "Quantity")) * ToDecimal(GetValue(row, "Rate"));
        }

        private ReportDocument LoadInvoiceReport()
        {
            try
            {
                ReportDocument rd = new GSTInvoice();
                int sectionCount = rd.ReportDefinition.Sections.Count;
                return rd;
            }
            catch
            {
                foreach (string reportPath in GetInvoiceReportCandidates())
                {
                    if (!File.Exists(reportPath))
                    {
                        continue;
                    }

                    ReportDocument rd = new ReportDocument();
                    rd.Load(reportPath);
                    int sectionCount = rd.ReportDefinition.Sections.Count;
                    return rd;
                }
            }

            throw new ApplicationException("Unable to load GST invoice report.");
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
                Path.Combine(currentDirectory, "GSTInvoice.rpt"),
                Path.Combine(Path.Combine(currentDirectory, "Inventory"), "GSTInvoice.rpt")
            };
        }

        private void SetBillTypeParameter()
        {
            ParameterFields paramFields = new ParameterFields();
            ParameterField pfItemYr = new ParameterField();
            pfItemYr.ParameterFieldName = "BillType";
            ParameterDiscreteValue dcItemYr = new ParameterDiscreteValue();
            dcItemYr.Value = "Original";
            pfItemYr.CurrentValues.Add(dcItemYr);
            paramFields.Add(pfItemYr);
            reportViewer.ParameterFieldInfo = paramFields;
        }

        private static string FirstValue(DataRow primary, DataRow secondary, params string[] columns)
        {
            foreach (string column in columns)
            {
                string value = GetValue(primary, column);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }

                value = GetValue(secondary, column);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            return "";
        }

        private static string GetValue(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return "";
            }

            return Convert.ToString(row[column]);
        }

        private static decimal ToDecimal(string value)
        {
            decimal result;
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }

            return 0;
        }

        private static string FormatAmount(decimal value)
        {
            return value.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private static string FormatDate(string value)
        {
            DateTime date;
            if (DateTime.TryParse(value, out date))
            {
                return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return value;
        }

        private static void Set(DataRow row, string column, string value)
        {
            if (row.Table.Columns.Contains(column))
            {
                row[column] = value ?? "";
            }
        }

        private static string GetFullErrorMessage(Exception ex)
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
    }
}
