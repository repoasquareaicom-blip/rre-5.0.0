using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Report
{
    public class RemembranceReport : Form
    {
        private readonly string conn = Program.connection;
        private DataGridView dgvCustomers;
        private DataGridView dgvReferences;
        private DataGridView dgvEmployees;
        private NumericUpDown nudDays;
        private Button btnRefresh;

        public RemembranceReport()
        {
            InitializeComponent();
            LoadReport();
        }

        private void InitializeComponent()
        {
            this.Text = "Remembrance Report";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 48;
            topPanel.BackColor = Color.FromArgb(236, 240, 245);

            Label lblTitle = new Label();
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Calibri", 14F, FontStyle.Bold);
            lblTitle.Location = new Point(12, 12);
            lblTitle.Text = "Upcoming Celebrations";

            Label lblDays = new Label();
            lblDays.AutoSize = true;
            lblDays.Font = new Font("Calibri", 10F);
            lblDays.Location = new Point(300, 16);
            lblDays.Text = "Next Days";

            nudDays = new NumericUpDown();
            nudDays.Location = new Point(370, 13);
            nudDays.Minimum = 1;
            nudDays.Maximum = 365;
            nudDays.Value = 30;
            nudDays.Width = 60;

            btnRefresh = new Button();
            btnRefresh.Location = new Point(445, 11);
            btnRefresh.Size = new Size(85, 26);
            btnRefresh.Text = "Refresh";
            btnRefresh.Click += new EventHandler(btnRefresh_Click);

            TableLayoutPanel content = new TableLayoutPanel();
            content.Dock = DockStyle.Fill;
            content.ColumnCount = 1;
            content.RowCount = 3;
            content.Padding = new Padding(10);
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 33.34F));

            dgvCustomers = CreateGrid();
            dgvReferences = CreateGrid();
            dgvEmployees = CreateGrid();

            content.Controls.Add(CreateSection("Customer Events", dgvCustomers), 0, 0);
            content.Controls.Add(CreateSection("Reference Events", dgvReferences), 0, 1);
            content.Controls.Add(CreateSection("Employee Events", dgvEmployees), 0, 2);

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblDays);
            topPanel.Controls.Add(nudDays);
            topPanel.Controls.Add(btnRefresh);
            this.Controls.Add(content);
            this.Controls.Add(topPanel);
        }

        private DataGridView CreateGrid()
        {
            DataGridView grid = new DataGridView();
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            return grid;
        }

        private Control CreateSection(string title, DataGridView grid)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(0, 24, 0, 8);

            Label label = new Label();
            label.AutoSize = true;
            label.Font = new Font("Calibri", 11F, FontStyle.Bold);
            label.Location = new Point(0, 2);
            label.Text = title;

            panel.Controls.Add(grid);
            panel.Controls.Add(label);
            return panel;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            DateTime today = DateTime.Today;
            DateTime until = today.AddDays(Convert.ToInt32(nudDays.Value));

            DataTable customers = CreateReportTable();
            DataTable references = CreateReportTable();
            DataTable employees = CreateReportTable();

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                AddTableEvents(con, customers, "Customers", "DOB", "Birthday", today, until);
                AddTableEvents(con, customers, "Customers", "AnniversaryDate", "Anniversary", today, until);

                AddTableEvents(con, references, "References", "DOB", "Birthday", today, until);
                AddTableEvents(con, references, "References", "AnniversaryDate", "Anniversary", today, until);

                AddTableEvents(con, employees, "Employee", "DOB", "Birthday", today, until);
                AddTableEvents(con, employees, "Employee", "AnniversaryDate", "Anniversary", today, until);
                AddTableEvents(con, employees, "Employee", "DateOfJoining", "Joining Day", today, until);
            }

            BindGrid(dgvCustomers, customers);
            BindGrid(dgvReferences, references);
            BindGrid(dgvEmployees, employees);
        }

        private DataTable CreateReportTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Event", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Phone", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("EventDate", typeof(DateTime));
            table.Columns.Add("UpcomingDate", typeof(DateTime));
            table.Columns.Add("DaysLeft", typeof(int));
            return table;
        }

        private void BindGrid(DataGridView grid, DataTable table)
        {
            table.DefaultView.Sort = "UpcomingDate ASC, Name ASC, Event ASC";
            grid.DataSource = table.DefaultView;

            if (grid.Columns.Contains("EventDate"))
            {
                grid.Columns["EventDate"].HeaderText = "Date";
                grid.Columns["EventDate"].DefaultCellStyle.Format = "dd-MMM-yyyy";
            }
            if (grid.Columns.Contains("UpcomingDate"))
            {
                grid.Columns["UpcomingDate"].HeaderText = "This Year";
                grid.Columns["UpcomingDate"].DefaultCellStyle.Format = "dd-MMM-yyyy";
            }
            if (grid.Columns.Contains("DaysLeft"))
            {
                grid.Columns["DaysLeft"].HeaderText = "Days Left";
                grid.Columns["DaysLeft"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void AddTableEvents(SqlConnection con, DataTable report, string tableName, string dateColumn, string eventName, DateTime today, DateTime until)
        {
            if (!ColumnExists(con, tableName, dateColumn))
                return;

            string sql = "SELECT Name, Phone, Email, " + dateColumn + " AS EventDate FROM dbo.[" + tableName + "] WHERE " + dateColumn + " IS NOT NULL";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    DateTime eventDate = Convert.ToDateTime(reader["EventDate"]);
                    DateTime upcoming = GetUpcomingDate(eventDate, today);
                    if (upcoming > until)
                        continue;

                    DataRow row = report.NewRow();
                    row["Event"] = eventName;
                    row["Name"] = Convert.ToString(reader["Name"]);
                    row["Phone"] = Convert.ToString(reader["Phone"]);
                    row["Email"] = Convert.ToString(reader["Email"]);
                    row["EventDate"] = eventDate;
                    row["UpcomingDate"] = upcoming;
                    row["DaysLeft"] = (upcoming - today).Days;
                    report.Rows.Add(row);
                }
            }
        }

        private DateTime GetUpcomingDate(DateTime eventDate, DateTime today)
        {
            int day = eventDate.Day;
            int month = eventDate.Month;
            int year = today.Year;
            int maxDay = DateTime.DaysInMonth(year, month);
            DateTime upcoming = new DateTime(year, month, Math.Min(day, maxDay));

            if (upcoming < today)
            {
                year++;
                maxDay = DateTime.DaysInMonth(year, month);
                upcoming = new DateTime(year, month, Math.Min(day, maxDay));
            }

            return upcoming;
        }

        private bool ColumnExists(SqlConnection con, string tableName, string columnName)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT COL_LENGTH('dbo." + tableName + "', '" + columnName + "')", con))
            {
                return cmd.ExecuteScalar() != DBNull.Value;
            }
        }
    }
}
