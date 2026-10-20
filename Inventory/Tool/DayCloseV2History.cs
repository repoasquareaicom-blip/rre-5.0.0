using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Tool
{
    public class DayCloseV2History : Form
    {
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private ComboBox cmbStatus;
        private DataGridView dgvHistory;
        private Label lblCount;

        public DayCloseV2History()
        {
            Text = "Day Close History";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1080, 600);
            MinimumSize = new Size(900, 400);
            Font = new Font("Segoe UI", 9F);
            BuildLayout();
            dtpTo.Value = DateTime.Today;
            dtpFrom.Value = DateTime.Today.AddDays(-30);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadHistory();
        }

        private void BuildLayout()
        {
            Panel top = new Panel();
            top.Dock = DockStyle.Top;
            top.Height = 86;
            Controls.Add(top);

            Label title = new Label();
            title.Text = "DAY CLOSE HISTORY";
            title.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(128, 20, 24);
            title.Location = new Point(12, 8);
            title.AutoSize = true;
            top.Controls.Add(title);

            AddLabel(top, "From Date", 12, 52);
            dtpFrom = DatePicker(top, 80, 48);
            AddLabel(top, "To Date", 220, 52);
            dtpTo = DatePicker(top, 275, 48);
            AddLabel(top, "Status", 415, 52);
            cmbStatus = new ComboBox();
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new object[] { "All", "Closed", "Not Closed" });
            cmbStatus.SelectedIndex = 0;
            cmbStatus.Location = new Point(462, 48);
            cmbStatus.Width = 110;
            top.Controls.Add(cmbStatus);
            Button btnShow = new Button();
            btnShow.Text = "Show";
            btnShow.Location = new Point(586, 46);
            btnShow.Size = new Size(80, 28);
            btnShow.Click += delegate { LoadHistory(); };
            top.Controls.Add(btnShow);
            Label note = new Label();
            note.Text = "History is view only. Nothing can be edited or submitted from here.";
            note.ForeColor = Color.DimGray;
            note.Location = new Point(680, 52);
            note.AutoSize = true;
            top.Controls.Add(note);

            Panel bottom = new Panel();
            bottom.Dock = DockStyle.Bottom;
            bottom.Height = 40;
            Controls.Add(bottom);
            lblCount = new Label();
            lblCount.Location = new Point(12, 12);
            lblCount.AutoSize = true;
            lblCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            bottom.Controls.Add(lblCount);
            Button btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Size = new Size(90, 28);
            bottom.Padding = new Padding(0, 6, 12, 6);
            btnClose.Dock = DockStyle.Right;
            btnClose.DialogResult = DialogResult.Cancel;
            bottom.Controls.Add(btnClose);
            CancelButton = btnClose;

            dgvHistory = new DataGridView();
            dgvHistory.Dock = DockStyle.Fill;
            dgvHistory.ReadOnly = true;
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.AllowUserToDeleteRows = false;
            dgvHistory.AllowUserToResizeRows = false;
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.MultiSelect = false;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.BackgroundColor = Color.White;
            dgvHistory.AutoGenerateColumns = false;
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            AddColumn("BusinessDate", "Business Date", 90, "dd-MM-yyyy", false);
            AddColumn("DayCloseStatus", "Status", 90, null, false);
            AddColumn("ExpectedTotal", "Expected Total", 100, "N2", true);
            AddColumn("ActualTotal", "Actual Total", 100, "N2", true);
            AddColumn("TotalDifference", "Difference", 90, "N2", true);
            AddColumn("EnteredBy", "Entered By", 110, null, false);
            AddColumn("EnteredOn", "Entered On", 120, "dd-MM-yyyy hh:mm tt", false);
            AddButton("View", 60);
            AddButton("PDF", 60);
            dgvHistory.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvHistory_CellFormatting);
            dgvHistory.CellContentClick += new DataGridViewCellEventHandler(dgvHistory_CellContentClick);
            dgvHistory.CellDoubleClick += new DataGridViewCellEventHandler(dgvHistory_CellDoubleClick);
            Controls.Add(dgvHistory);
            dgvHistory.BringToFront();
        }

        private static void AddLabel(Control parent, string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.AutoSize = true;
            parent.Controls.Add(label);
        }

        private static DateTimePicker DatePicker(Control parent, int x, int y)
        {
            DateTimePicker picker = new DateTimePicker();
            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = "dd-MM-yyyy";
            picker.Location = new Point(x, y);
            picker.Width = 120;
            parent.Controls.Add(picker);
            return picker;
        }

        private void AddColumn(string property, string header, float weight, string format, bool right)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = property;
            column.DataPropertyName = property;
            column.HeaderText = header;
            column.FillWeight = weight;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            if (format != null)
            {
                column.DefaultCellStyle.Format = format;
            }
            if (right)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            dgvHistory.Columns.Add(column);
        }

        private void AddButton(string name, float weight)
        {
            DataGridViewButtonColumn column = new DataGridViewButtonColumn();
            column.Name = name;
            column.HeaderText = name;
            column.Text = name;
            column.UseColumnTextForButtonValue = true;
            column.FillWeight = weight;
            dgvHistory.Columns.Add(column);
        }

        private void LoadHistory()
        {
            if (dtpTo.Value.Date < dtpFrom.Value.Date)
            {
                MessageBox.Show("To Date cannot be before From Date.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string status = cmbStatus.SelectedIndex == 1 ? "CLOSED" : (cmbStatus.SelectedIndex == 2 ? "NOT CLOSED" : "ALL");
            DataTable table = new DataTable();
            try
            {
                Cursor = Cursors.WaitCursor;
                using (SqlConnection con = new SqlConnection(Program.connection))
                using (SqlCommand cmd = new SqlCommand("dbo.Proc_DayCloseV2_GetHistory", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@FromDate", dtpFrom.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", dtpTo.Value.Date);
                    cmd.Parameters.AddWithValue("@Status", status);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Day Close history could not be loaded.\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
            dgvHistory.DataSource = table;
            int closedCount = 0;
            foreach (DataRow row in table.Rows)
            {
                if (Convert.ToString(row["DayCloseStatus"]) == "CLOSED")
                {
                    closedCount++;
                }
            }
            lblCount.Text = "Dates: " + table.Rows.Count + "      Closed: " + closedCount + "      Not Closed: " + (table.Rows.Count - closedCount);
        }

        private void dgvHistory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }
            string column = dgvHistory.Columns[e.ColumnIndex].Name;
            if (column == "DayCloseStatus")
            {
                bool notClosed = Convert.ToString(e.Value) != "CLOSED";
                e.CellStyle.ForeColor = notClosed ? Color.Red : Color.FromArgb(33, 37, 41);
                e.CellStyle.Font = new Font(dgvHistory.Font, FontStyle.Bold);
            }
            else if (column == "TotalDifference" && e.Value != null && e.Value != DBNull.Value)
            {
                e.CellStyle.ForeColor = Convert.ToDecimal(e.Value) == 0m ? Color.DarkGreen : Color.Red;
            }
        }

        private DateTime? RowDate(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvHistory.Rows.Count)
            {
                return null;
            }
            object value = dgvHistory.Rows[rowIndex].Cells["BusinessDate"].Value;
            return value == null || value == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(value).Date;
        }

        private void dgvHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DateTime? date = RowDate(e.RowIndex);
            if (date.HasValue == false || e.ColumnIndex < 0)
            {
                return;
            }
            string column = dgvHistory.Columns[e.ColumnIndex].Name;
            if (column == "View")
            {
                ViewDate(date.Value);
            }
            else if (column == "PDF")
            {
                GeneratePdf(date.Value);
            }
        }

        private void dgvHistory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DateTime? date = RowDate(e.RowIndex);
            if (date.HasValue && e.ColumnIndex >= 0 && dgvHistory.Columns[e.ColumnIndex] is DataGridViewTextBoxColumn)
            {
                ViewDate(date.Value);
            }
        }

        private void ViewDate(DateTime date)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                using (DayCloseV2 view = new DayCloseV2(date, true))
                {
                    Cursor = Cursors.Default;
                    view.ShowDialog(this);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("The Day Close could not be loaded.\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void GeneratePdf(DateTime date)
        {
            string path;
            try
            {
                Cursor = Cursors.WaitCursor;
                path = DayCloseV2Pdf.Generate(date);
            }
            catch (Exception ex)
            {
                DayCloseV2Pdf.LogError(date, ex);
                MessageBox.Show("The PDF could not be generated.\n\nError: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
            try
            {
                Process.Start(path);
            }
            catch (Win32Exception)
            {
                MessageBox.Show("PDF saved to:\n" + path + "\n\nNo PDF viewer is available to open it.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
