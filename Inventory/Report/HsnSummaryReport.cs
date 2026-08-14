using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Report
{
    public partial class HsnSummaryReport : Form
    {
        private static readonly string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        private DataTable reportData = new DataTable();

        public HsnSummaryReport()
        {
            InitializeComponent();
            chkB2B.Checked = true;
            chkB2C.Checked = true;
            BindCompany();
        }

        private void btnsearch_Click(object sender, EventArgs e)
        {
            if (!ValidateFilters())
            {
                return;
            }

            LoadReport();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (reportData == null || reportData.Rows.Count == 0)
            {
                MessageBox.Show("No data available to export.");
                return;
            }

            saveFileDialog1.Filter = "Excel Files (*.xls)|*.xls";
            saveFileDialog1.FileName = "HSN_Summary_Report.xls";

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string fileName = saveFileDialog1.FileName;
            if (!fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".xls";
            }

            StringBuilder export = new StringBuilder();
            for (int i = 0; i < reportData.Columns.Count; i++)
            {
                if (i > 0)
                {
                    export.Append("\t");
                }
                export.Append(reportData.Columns[i].ColumnName);
            }
            export.Append("\r\n");

            foreach (DataRow row in reportData.Rows)
            {
                for (int i = 0; i < reportData.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        export.Append("\t");
                    }
                    export.Append(Convert.ToString(row[i]).Replace("\r", " ").Replace("\n", " "));
                }
                export.Append("\r\n");
            }

            byte[] output = Encoding.GetEncoding(1254).GetBytes(export.ToString());
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(output, 0, output.Length);
            }

            MessageBox.Show("Report saved in " + fileName);
        }

        private bool ValidateFilters()
        {
            if (Frommtdate.Value.Date > Tomtdate.Value.Date)
            {
                MessageBox.Show("From date should not be greater than To date.");
                Frommtdate.Focus();
                return false;
            }

            if (!chkB2B.Checked && !chkB2C.Checked)
            {
                MessageBox.Show("Please select B2B or B2C.");
                chkB2B.Focus();
                return false;
            }

            if (cmbcompanychange.SelectedIndex < 0 || cmbcompanychange.Text == "--Select--")
            {
                MessageBox.Show("Please select company.");
                cmbcompanychange.Focus();
                return false;
            }

            return true;
        }

        private void BindCompany()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conn))
                using (SqlCommand cmd = new SqlCommand("SELECT Id, CompanyName FROM ReportAddressDetails ORDER BY Id", con))
                using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    ad.Fill(dt);

                    DataRow dr = dt.NewRow();
                    dr["Id"] = 0;
                    dr["CompanyName"] = "--Select--";
                    dt.Rows.InsertAt(dr, 0);

                    cmbcompanychange.DataSource = dt;
                    cmbcompanychange.ValueMember = "Id";
                    cmbcompanychange.DisplayMember = "CompanyName";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadReport()
        {
            DateTime fromDate = Frommtdate.Value.Date;
            DateTime toDate = Tomtdate.Value.Date.AddDays(1);
            string salesTable;
            string detailsTable;

            GetSalesTables(cmbcompanychange.Text, out salesTable, out detailsTable);

            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(GetReportSql(salesTable, detailsTable), con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@fromdate", fromDate);
                cmd.Parameters.AddWithValue("@todate", toDate);
                cmd.Parameters.AddWithValue("@includeB2B", chkB2B.Checked ? 1 : 0);
                cmd.Parameters.AddWithValue("@includeB2C", chkB2C.Checked ? 1 : 0);

                reportData = new DataTable();
                ad.Fill(reportData);
            }

            dgvStockrpt.DataSource = reportData;
            ApplyGridStyle();
        }

        private void GetSalesTables(string companyName, out string salesTable, out string detailsTable)
        {
            if (companyName == "R.R. ELECTRICAL AGENCIES")
            {
                salesTable = "Sales";
                detailsTable = "SalesDetails";
                return;
            }

            if (companyName == "R.R. PIPES")
            {
                salesTable = "SalesPipes";
                detailsTable = "SalesPipesDetails";
                return;
            }

            if (companyName == "RR TRADERS")
            {
                salesTable = "SalesTraders";
                detailsTable = "SalesTradersDetails";
                return;
            }

            throw new InvalidOperationException("Invalid company selected.");
        }

        private string GetReportSql(string salesTable, string detailsTable)
        {
            return @"
SELECT
    ISNULL(NULLIF(LTRIM(RTRIM(pm.HSN)), ''), '') AS HSN,
    MAX(ISNULL(um.UOM, pm.UOM)) AS UOM,
    CAST(SUM(CASE WHEN ISNUMERIC(sd.Quantity) = 1 THEN CONVERT(decimal(18, 3), sd.Quantity) ELSE 0 END) AS decimal(18, 3)) AS [Total Quantity],
    CAST(SUM(line.AmountIncludingTax) AS decimal(18, 2)) AS TotalAmount,
    CAST(SUM(line.TaxableAmount) AS decimal(18, 2)) AS TaxableValue,
    CAST(SUM((line.AmountIncludingTax - line.TaxableAmount) / 2) AS decimal(18, 2)) AS CGST,
    CAST(SUM((line.AmountIncludingTax - line.TaxableAmount) / 2) AS decimal(18, 2)) AS SGST,
    CAST(MAX(ISNULL(sd.gst, 0)) AS decimal(18, 2)) AS [Tax %]
FROM " + salesTable + @" s
INNER JOIN " + detailsTable + @" sd ON sd.Salesid = s.Salesid
LEFT JOIN ProductMaster pm ON CONVERT(varchar(50), pm.id) = LTRIM(RTRIM(sd.Productid))
LEFT JOIN UOM um ON CONVERT(varchar(50), um.Uomid) = LTRIM(RTRIM(pm.UOM)) AND ISNULL(um.IsDeleted, 0) = 0
CROSS APPLY (
    SELECT
        CASE WHEN ISNUMERIC(sd.Amount) = 1 THEN CONVERT(decimal(18, 2), sd.Amount) ELSE 0 END AS AmountIncludingTax,
        CASE
            WHEN ISNULL(sd.gst, 0) > 0 AND ISNUMERIC(sd.Amount) = 1
                THEN CONVERT(decimal(18, 6), sd.Amount) * 100 / (100 + ISNULL(sd.gst, 0))
            WHEN ISNUMERIC(sd.Amount) = 1 THEN CONVERT(decimal(18, 6), sd.Amount)
            ELSE 0
        END AS TaxableAmount
) line
WHERE s.Updatedon >= @fromdate
  AND s.Updatedon < @todate
  AND (
        (@includeB2B = 1 AND NULLIF(LTRIM(RTRIM(ISNULL(s.Tin, ''))), '') IS NOT NULL)
        OR
        (@includeB2C = 1 AND NULLIF(LTRIM(RTRIM(ISNULL(s.Tin, ''))), '') IS NULL)
      )
GROUP BY
    ISNULL(NULLIF(LTRIM(RTRIM(pm.HSN)), ''), '')
ORDER BY
    ISNULL(NULLIF(LTRIM(RTRIM(pm.HSN)), ''), '')";
        }

        private void ApplyGridStyle()
        {
            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvStockrpt.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvStockrpt.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            foreach (DataGridViewColumn c in dgvStockrpt.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
                c.SortMode = DataGridViewColumnSortMode.NotSortable;

                if (c.ValueType == typeof(decimal) || c.Name == "Total Quantity" || c.Name == "TotalAmount" ||
                    c.Name == "TaxableValue" || c.Name == "CGST" || c.Name == "SGST" || c.Name == "Tax %")
                {
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
        }
    }
}
