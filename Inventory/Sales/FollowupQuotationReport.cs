using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory
{
    public class FollowupQuotationReport : Form
    {
        private static readonly string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

        private readonly CheckBox chkDateRange = new CheckBox();
        private readonly CheckBox chkBlankFollowups = new CheckBox();
        private readonly DateTimePicker fromDate = new DateTimePicker();
        private readonly DateTimePicker toDate = new DateTimePicker();
        private readonly DateTimePicker rescheduleDate = new DateTimePicker();
        private readonly Button btnSearch = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnReschedule = new Button();
        private readonly Button btnClearFollowup = new Button();
        private readonly Button btnClose = new Button();
        private readonly ComboBox cmbRows = new ComboBox();
        private readonly DataGridView dgvFollowups = new DataGridView();
        private readonly Label lblCount = new Label();

        public FollowupQuotationReport()
        {
            InitializeComponent();
            EnsureFollowupReportStorage();
        }

        private void InitializeComponent()
        {
            this.Text = "Follow Up Quotations";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(980, 620);
            this.Font = new Font("Calibri", 9.75F);

            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 78;
            topPanel.Padding = new Padding(8);
            topPanel.BackColor = Color.Gainsboro;

            chkDateRange.Text = "Date Range";
            chkDateRange.Location = new Point(10, 15);
            chkDateRange.AutoSize = true;
            chkDateRange.CheckedChanged += chkDateRange_CheckedChanged;

            chkBlankFollowups.Text = "Show Blank Follow Up";
            chkBlankFollowups.Location = new Point(590, 15);
            chkBlankFollowups.AutoSize = true;
            chkBlankFollowups.CheckedChanged += chkBlankFollowups_CheckedChanged;

            Label lblRows = new Label();
            lblRows.Text = "Rows";
            lblRows.Location = new Point(590, 49);
            lblRows.AutoSize = true;

            cmbRows.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRows.Items.AddRange(new object[] { "10", "50", "100" });
            cmbRows.Location = new Point(632, 45);
            cmbRows.Width = 70;
            cmbRows.SelectedIndex = 0;
            cmbRows.SelectedIndexChanged += cmbRows_SelectedIndexChanged;

            Label lblFrom = new Label();
            lblFrom.Text = "From";
            lblFrom.Location = new Point(105, 17);
            lblFrom.AutoSize = true;

            fromDate.CustomFormat = "dd-MM-yyyy";
            fromDate.Format = DateTimePickerFormat.Custom;
            fromDate.Location = new Point(145, 13);
            fromDate.Width = 110;
            fromDate.Value = DateTime.Today;
            fromDate.Enabled = false;

            Label lblTo = new Label();
            lblTo.Text = "To";
            lblTo.Location = new Point(267, 17);
            lblTo.AutoSize = true;

            toDate.CustomFormat = "dd-MM-yyyy";
            toDate.Format = DateTimePickerFormat.Custom;
            toDate.Location = new Point(295, 13);
            toDate.Width = 110;
            toDate.Value = DateTime.Today.AddDays(30);
            toDate.Enabled = false;

            Label lblReschedule = new Label();
            lblReschedule.Text = "Set Follow Date";
            lblReschedule.Location = new Point(10, 49);
            lblReschedule.AutoSize = true;

            rescheduleDate.CustomFormat = "dd-MM-yyyy";
            rescheduleDate.Format = DateTimePickerFormat.Custom;
            rescheduleDate.Location = new Point(108, 45);
            rescheduleDate.Width = 125;

            btnSearch.Text = "Search";
            btnSearch.Location = new Point(418, 11);
            btnSearch.Size = new Size(78, 26);
            btnSearch.Click += btnSearch_Click;

            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(502, 11);
            btnRefresh.Size = new Size(78, 26);
            btnRefresh.Click += btnRefresh_Click;

            btnReschedule.Text = "Reschedule";
            btnReschedule.Location = new Point(245, 43);
            btnReschedule.Size = new Size(100, 26);
            btnReschedule.Click += btnReschedule_Click;

            btnClearFollowup.Text = "Clear Follow";
            btnClearFollowup.Location = new Point(351, 43);
            btnClearFollowup.Size = new Size(96, 26);
            btnClearFollowup.Click += btnClearFollowup_Click;

            btnClose.Text = "Mark Closed";
            btnClose.Location = new Point(453, 43);
            btnClose.Size = new Size(100, 26);
            btnClose.Click += btnClose_Click;

            topPanel.Controls.Add(chkDateRange);
            topPanel.Controls.Add(chkBlankFollowups);
            topPanel.Controls.Add(lblFrom);
            topPanel.Controls.Add(fromDate);
            topPanel.Controls.Add(lblTo);
            topPanel.Controls.Add(toDate);
            topPanel.Controls.Add(lblReschedule);
            topPanel.Controls.Add(rescheduleDate);
            topPanel.Controls.Add(btnSearch);
            topPanel.Controls.Add(btnRefresh);
            topPanel.Controls.Add(btnReschedule);
            topPanel.Controls.Add(btnClearFollowup);
            topPanel.Controls.Add(btnClose);
            topPanel.Controls.Add(lblRows);
            topPanel.Controls.Add(cmbRows);

            dgvFollowups.AllowUserToAddRows = false;
            dgvFollowups.AllowUserToDeleteRows = false;
            dgvFollowups.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFollowups.BackgroundColor = SystemColors.Window;
            dgvFollowups.Dock = DockStyle.Fill;
            dgvFollowups.MultiSelect = false;
            dgvFollowups.ReadOnly = true;
            dgvFollowups.RowHeadersVisible = false;
            dgvFollowups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFollowups.DataBindingComplete += dgvFollowups_DataBindingComplete;
            dgvFollowups.SelectionChanged += dgvFollowups_SelectionChanged;

            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 28;
            bottomPanel.Padding = new Padding(8, 5, 8, 5);

            lblCount.Dock = DockStyle.Fill;
            lblCount.TextAlign = ContentAlignment.MiddleLeft;
            bottomPanel.Controls.Add(lblCount);

            this.Controls.Add(dgvFollowups);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(topPanel);
            this.Load += FollowupQuotationReport_Load;
        }

        private void FollowupQuotationReport_Load(object sender, EventArgs e)
        {
            LoadFollowups();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadFollowups();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadFollowups();
        }

        private void chkDateRange_CheckedChanged(object sender, EventArgs e)
        {
            fromDate.Enabled = chkDateRange.Checked;
            toDate.Enabled = chkDateRange.Checked;
            if (chkDateRange.Checked)
            {
                chkBlankFollowups.Checked = false;
            }
        }

        private void chkBlankFollowups_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBlankFollowups.Checked)
            {
                chkDateRange.Checked = false;
            }
        }

        private void cmbRows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.IsHandleCreated)
            {
                LoadFollowups();
            }
        }

        private void btnReschedule_Click(object sender, EventArgs e)
        {
            dgvFollowups.EndEdit();
            string quotationId = GetSelectedQuotationId();
            if (string.IsNullOrEmpty(quotationId))
            {
                MessageBox.Show("Please select a quotation.");
                return;
            }

            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand("UPDATE QuotationHeader SET FollowupDate = @FollowupDate WHERE Quotationid = @Quotationid", con))
            {
                cmd.Parameters.Add("@FollowupDate", SqlDbType.DateTime).Value = rescheduleDate.Value.Date;
                cmd.Parameters.Add("@Quotationid", SqlDbType.VarChar).Value = quotationId;
                con.Open();
                cmd.ExecuteNonQuery();
            }

            LoadFollowups();
        }

        private void btnClearFollowup_Click(object sender, EventArgs e)
        {
            string quotationId = GetSelectedQuotationId();
            if (string.IsNullOrEmpty(quotationId))
            {
                MessageBox.Show("Please select a quotation.");
                return;
            }

            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand("UPDATE QuotationHeader SET FollowupDate = NULL WHERE Quotationid = @Quotationid", con))
            {
                cmd.Parameters.Add("@Quotationid", SqlDbType.VarChar).Value = quotationId;
                con.Open();
                cmd.ExecuteNonQuery();
            }

            LoadFollowups();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            string quotationId = GetSelectedQuotationId();
            if (string.IsNullOrEmpty(quotationId))
            {
                MessageBox.Show("Please select a quotation.");
                return;
            }

            DialogResult result = MessageBox.Show("Mark follow up closed for " + quotationId + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand("UPDATE QuotationHeader SET FollowupClosed = 1 WHERE Quotationid = @Quotationid", con))
            {
                cmd.Parameters.Add("@Quotationid", SqlDbType.VarChar).Value = quotationId;
                con.Open();
                cmd.ExecuteNonQuery();
            }

            LoadFollowups();
        }

        private void LoadFollowups()
        {
            DateTime from = fromDate.Value.Date;
            DateTime to = toDate.Value.Date;
            if (chkDateRange.Checked && from > to)
            {
                MessageBox.Show("From date should be before To date.");
                return;
            }

            string followupFilter;
            if (chkBlankFollowups.Checked)
            {
                followupFilter = "  AND q.FollowupDate IS NULL";
            }
            else if (chkDateRange.Checked)
            {
                followupFilter = @"  AND q.FollowupDate IS NOT NULL
  AND q.FollowupDate >= @FromDate
  AND q.FollowupDate < DATEADD(day, 1, @ToDate)";
            }
            else
            {
                followupFilter = "  AND q.FollowupDate IS NOT NULL";
            }

            int rowCount = GetSelectedRowCount();
            string sql = @"
SELECT TOP " + rowCount.ToString() + @"
    q.Quotationid AS [Quotation No],
    q.customername AS [Customer],
    q.City,
    q.date AS [Quotation Date],
    q.FollowupDate AS [Follow Up Date],
    q.FollowupPhone AS [Follow Up Phone],
    q.Status
FROM QuotationHeader q
LEFT JOIN QuotationEstimation qe ON qe.Quotationid = q.Quotationid
WHERE ISNULL(q.FollowupClosed, 0) = 0
  AND q.Status = 'Open'
  AND qe.Quotationid IS NULL
" + followupFilter + @"
ORDER BY q.date DESC, q.Quotationid DESC";

            Cursor previousCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            btnSearch.Enabled = false;
            btnRefresh.Enabled = false;

            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.CommandTimeout = 20;
                if (chkDateRange.Checked && !chkBlankFollowups.Checked)
                {
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = from;
                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = to;
                }

                try
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    dgvFollowups.DataSource = dt;
                    lblCount.Text = chkBlankFollowups.Checked ? "Blank follow up quotations: " + dt.Rows.Count : "Follow up quotations: " + dt.Rows.Count;
                }
                finally
                {
                    btnSearch.Enabled = true;
                    btnRefresh.Enabled = true;
                    this.Cursor = previousCursor;
                }
            }
        }

        private int GetSelectedRowCount()
        {
            int rowCount;
            if (cmbRows.SelectedItem == null || !int.TryParse(cmbRows.SelectedItem.ToString(), out rowCount))
            {
                return 10;
            }

            return rowCount;
        }

        private void dgvFollowups_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvFollowups.Columns["Quotation No"] != null)
            {
                dgvFollowups.Columns["Quotation No"].ReadOnly = true;
                dgvFollowups.Columns["Quotation No"].FillWeight = 85;
            }
            if (dgvFollowups.Columns["Customer"] != null)
            {
                dgvFollowups.Columns["Customer"].ReadOnly = true;
                dgvFollowups.Columns["Customer"].FillWeight = 150;
            }
            if (dgvFollowups.Columns["City"] != null)
            {
                dgvFollowups.Columns["City"].ReadOnly = true;
                dgvFollowups.Columns["City"].FillWeight = 80;
            }
            if (dgvFollowups.Columns["Quotation Date"] != null)
            {
                dgvFollowups.Columns["Quotation Date"].ReadOnly = true;
                dgvFollowups.Columns["Quotation Date"].DefaultCellStyle.Format = "dd-MM-yyyy";
                dgvFollowups.Columns["Quotation Date"].FillWeight = 90;
            }
            if (dgvFollowups.Columns["Follow Up Date"] != null)
            {
                dgvFollowups.Columns["Follow Up Date"].DefaultCellStyle.Format = "dd-MM-yyyy";
                dgvFollowups.Columns["Follow Up Date"].FillWeight = 95;
            }
            if (dgvFollowups.Columns["Follow Up Phone"] != null)
            {
                dgvFollowups.Columns["Follow Up Phone"].ReadOnly = true;
                dgvFollowups.Columns["Follow Up Phone"].FillWeight = 95;
            }
            if (dgvFollowups.Columns["Status"] != null)
            {
                dgvFollowups.Columns["Status"].ReadOnly = true;
                dgvFollowups.Columns["Status"].FillWeight = 90;
            }
            SetRescheduleDateFromSelectedRow();
        }

        private void dgvFollowups_SelectionChanged(object sender, EventArgs e)
        {
            SetRescheduleDateFromSelectedRow();
        }

        private string GetSelectedQuotationId()
        {
            if (dgvFollowups.CurrentRow == null || dgvFollowups.CurrentRow.Cells["Quotation No"].Value == null)
            {
                return string.Empty;
            }

            return Convert.ToString(dgvFollowups.CurrentRow.Cells["Quotation No"].Value);
        }

        private void SetRescheduleDateFromSelectedRow()
        {
            if (dgvFollowups.CurrentRow == null || dgvFollowups.CurrentRow.Cells["Follow Up Date"].Value == null)
            {
                rescheduleDate.Value = DateTime.Today;
                return;
            }

            object value = dgvFollowups.CurrentRow.Cells["Follow Up Date"].Value;
            if (value == DBNull.Value)
            {
                rescheduleDate.Value = DateTime.Today;
                return;
            }

            rescheduleDate.Value = Convert.ToDateTime(value);
        }

        private void EnsureFollowupReportStorage()
        {
            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(@"
DECLARE @schema sysname;
DECLARE @sql nvarchar(max);

SELECT TOP 1 @schema = SCHEMA_NAME(schema_id)
FROM sys.tables
WHERE name = 'QuotationHeader';

IF @schema IS NOT NULL
   AND COL_LENGTH(QUOTENAME(@schema) + '.QuotationHeader', 'FollowupDate') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + QUOTENAME(@schema) + N'.QuotationHeader ADD FollowupDate datetime NULL';
    EXEC sp_executesql @sql;
END

IF @schema IS NOT NULL
   AND COL_LENGTH(QUOTENAME(@schema) + '.QuotationHeader', 'FollowupClosed') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + QUOTENAME(@schema) + N'.QuotationHeader ADD FollowupClosed bit NULL';
    EXEC sp_executesql @sql;
END

IF @schema IS NOT NULL
   AND COL_LENGTH(QUOTENAME(@schema) + '.QuotationHeader', 'FollowupPhone') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + QUOTENAME(@schema) + N'.QuotationHeader ADD FollowupPhone varchar(50) NULL';
    EXEC sp_executesql @sql;
END", con))
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
