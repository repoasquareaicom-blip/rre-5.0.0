using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Inventory.Tool
{
    public class RackWiseStockUpload : Form
    {
        public const string MenuKey = "RackWiseStockUpload";
        public const string SourceExcel = "EXCEL";
        public const string SourceResetToZero = "RESET TO ZERO";

        private static readonly string[] RequiredColumns = new string[] { "Product ID", "Product Name", "UOM", "Rack Name", "Stock" };

        private TextBox txtFile;
        private Button btnBrowse;
        private Button btnLoad;
        private Button btnValidate;
        private Button btnUpload;
        private Button btnClear;
        private Label lblTotal;
        private Label lblValid;
        private Label lblError;
        private Label lblZero;
        private Label lblAffected;
        private Label lblStatus;
        private DataGridView dgvRows;

        private readonly List<UploadRow> rows = new List<UploadRow>();
        private List<PlanRow> plan;
        private DataTable view;
        private string loadedFile;
        private bool readyToUpload;

        public RackWiseStockUpload()
        {
            Text = "Rack Wise Stock Upload";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            Font = new Font("Segoe UI", 9F);
            BuildLayout();
            ResetScreen();
        }

        public static bool HasPermission()
        {
            if (Program.Userrole == "Admin")
                return true;
            DataTable menus = Program.Dtmenu;
            if (menus == null || !menus.Columns.Contains("Data"))
                return false;
            foreach (DataRow row in menus.Rows)
            {
                if (Convert.ToString(row["Data"]).Trim() == MenuKey)
                    return true;
            }
            return false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!HasPermission())
            {
                MessageBox.Show("You do not have permission to use Rack Wise Stock Upload.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                BeginInvoke(new MethodInvoker(Close));
            }
        }

        private void BuildLayout()
        {
            Panel top = new Panel();
            top.Dock = DockStyle.Top;
            top.Height = 210;
            Controls.Add(top);

            Caption(top, "RACK WISE STOCK UPLOAD  (Physical Rack Stock Reset)", 16, 10, 16, true);

            Caption(top, "Excel File :", 16, 56, 10, false);
            txtFile = new TextBox();
            txtFile.Location = new Point(110, 53);
            txtFile.Width = 520;
            txtFile.ReadOnly = true;
            top.Controls.Add(txtFile);
            btnBrowse = MakeButton(top, "Browse", 640, 50, 90);
            btnBrowse.Click += new EventHandler(btnBrowse_Click);

            btnLoad = MakeButton(top, "Load Excel", 110, 88, 110);
            btnLoad.Click += new EventHandler(btnLoad_Click);
            btnValidate = MakeButton(top, "Validate", 228, 88, 100);
            btnValidate.Click += new EventHandler(btnValidate_Click);
            btnUpload = MakeButton(top, "Upload Stock", 336, 88, 120);
            btnUpload.Click += new EventHandler(btnUpload_Click);
            btnClear = MakeButton(top, "Clear", 464, 88, 90);
            btnClear.Click += new EventHandler(btnClear_Click);
            Button btnTemplate = MakeButton(top, "Download Template", 562, 88, 150);
            btnTemplate.Click += new EventHandler(btnTemplate_Click);

            Label note = Caption(top,
                "Excel Stock represents the final physical rack stock.\n"
                + "All allocated racks of products included in the Excel will be reset.\n"
                + "Allocated racks omitted from Excel will be reset to zero.",
                16, 124, 9.5F, true);
            note.ForeColor = Color.DarkRed;

            lblTotal = Caption(top, "", 760, 40, 10, true);
            lblValid = Caption(top, "", 760, 62, 10, true);
            lblError = Caption(top, "", 760, 84, 10, true);
            lblZero = Caption(top, "", 760, 106, 10, true);
            lblAffected = Caption(top, "", 760, 128, 10, true);
            lblValid.ForeColor = Color.DarkGreen;
            lblError.ForeColor = Color.DarkRed;
            lblZero.ForeColor = Color.DarkOrange;
            lblStatus = Caption(top, "", 16, 184, 10, true);

            dgvRows = new DataGridView();
            dgvRows.Dock = DockStyle.Fill;
            dgvRows.ReadOnly = true;
            dgvRows.AllowUserToAddRows = false;
            dgvRows.AllowUserToDeleteRows = false;
            dgvRows.RowHeadersVisible = false;
            dgvRows.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRows.BackgroundColor = Color.White;
            dgvRows.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRows.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvRows.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvRows_CellFormatting);
            Controls.Add(dgvRows);
            dgvRows.BringToFront();
        }

        private static Label Caption(Control parent, string text, int x, int y, float size, bool bold)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular);
            label.Location = new Point(x, y);
            label.AutoSize = true;
            parent.Controls.Add(label);
            return label;
        }

        private static Button MakeButton(Control parent, string text, int x, int y, int width)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(width, 28);
            parent.Controls.Add(button);
            return button;
        }

        private void ResetScreen()
        {
            rows.Clear();
            plan = null;
            loadedFile = null;
            readyToUpload = false;
            txtFile.Text = string.Empty;
            BindRows();
            ShowSummary();
            lblStatus.Text = "Select an Excel file and click Load Excel.";
            lblStatus.ForeColor = Color.Black;
        }

        private static DataTable NewViewTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Row No", typeof(string));
            table.Columns.Add("Product ID", typeof(string));
            table.Columns.Add("Product Name", typeof(string));
            table.Columns.Add("UOM", typeof(string));
            table.Columns.Add("Rack Name", typeof(string));
            table.Columns.Add("Current Stock", typeof(string));
            table.Columns.Add("Target Stock", typeof(string));
            table.Columns.Add("Adjustment", typeof(string));
            table.Columns.Add("Source", typeof(string));
            table.Columns.Add("Validation Result", typeof(string));
            table.Columns.Add("Remarks", typeof(string));
            table.Columns.Add("Resulting Stock", typeof(string));
            table.Columns.Add("Upload Result", typeof(string));
            return table;
        }

        private void FormatGrid()
        {
            SetFill("Row No", 40);
            SetFill("Product ID", 55);
            SetFill("Product Name", 170);
            SetFill("UOM", 40);
            SetFill("Rack Name", 65);
            SetFill("Current Stock", 65);
            SetFill("Target Stock", 65);
            SetFill("Adjustment", 65);
            SetFill("Source", 80);
            SetFill("Validation Result", 65);
            SetFill("Remarks", 200);
            SetFill("Resulting Stock", 65);
            SetFill("Upload Result", 60);
            foreach (string name in new string[] { "Row No", "Product ID", "Current Stock", "Target Stock", "Adjustment", "Resulting Stock" })
            {
                if (dgvRows.Columns.Contains(name))
                    dgvRows.Columns[name].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            foreach (DataGridViewColumn column in dgvRows.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void SetFill(string name, float weight)
        {
            if (dgvRows.Columns.Contains(name))
                dgvRows.Columns[name].FillWeight = weight;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx";
                dialog.Title = "Select Rack Wise Stock Excel";
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    txtFile.Text = dialog.FileName;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string path = txtFile.Text.Trim();
            if (path.Length == 0 || !File.Exists(path))
            {
                MessageBox.Show("Please select an Excel file.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                rows.Clear();
                plan = null;
                loadedFile = null;
                readyToUpload = false;
                string error;
                List<UploadRow> loaded = ReadExcel(path, out error);
                if (loaded == null)
                {
                    BindRows();
                    ShowSummary();
                    lblStatus.Text = "Excel was not loaded.";
                    lblStatus.ForeColor = Color.DarkRed;
                    MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                rows.AddRange(loaded);
                loadedFile = path;
                ValidateRows();
            }
            catch (Exception ex)
            {
                plan = null;
                readyToUpload = false;
                BindRows();
                ShowSummary();
                lblStatus.Text = "Excel was not loaded.";
                lblStatus.ForeColor = Color.DarkRed;
                MessageBox.Show("Could not read the Excel file.\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            if (rows.Count == 0)
            {
                MessageBox.Show("Please load an Excel file first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Cursor = Cursors.WaitCursor;
            try
            {
                ValidateRows();
            }
            catch (Exception ex)
            {
                plan = null;
                readyToUpload = false;
                ShowSummary();
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetScreen();
        }

        private void btnTemplate_Click(object sender, EventArgs e)
        {
            using (RackWiseStockTemplate template = new RackWiseStockTemplate())
                template.ShowDialog(this);
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (!readyToUpload || plan == null)
                return;

            Dictionary<int, bool> products = new Dictionary<int, bool>();
            foreach (UploadRow row in rows)
            {
                if (row.ProductId.HasValue)
                    products[row.ProductId.Value] = true;
            }
            int zeroRacks = ZeroRackCount();

            string message = "Products in Excel        : " + products.Count + "\n"
                + "Excel Rack Rows          : " + rows.Count + "\n"
                + "Additional Racks to Zero : " + zeroRacks + "\n"
                + "Total Affected Racks     : " + plan.Count + "\n\n"
                + "This operation will reset the FINAL rack stock for all racks allocated to the products included in this Excel file.\n\n"
                + "RACKS OMITTED FROM EXCEL FOR THESE PRODUCTS WILL BE RESET TO ZERO."
                + (zeroRacks > 0 ? "  (" + zeroRacks + " rack(s) - shown as RESET TO ZERO in the grid)" : string.Empty) + "\n\n"
                + "Existing transaction history will be preserved.\n\n"
                + "Continue?";
            if (MessageBox.Show(message, "Confirm Rack Wise Stock Reset", MessageBoxButtons.YesNo,
                    zeroRacks > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            Cursor = Cursors.WaitCursor;
            try
            {
                DataTable result;
                using (SqlConnection con = new SqlConnection(Program.connection))
                using (SqlCommand cmd = new SqlCommand("dbo.Proc_RackWiseStockUpload_Apply", con))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 600;
                    AddRowsParameter(cmd);
                    cmd.Parameters.AddWithValue("@FileName", Path.GetFileName(loadedFile));
                    cmd.Parameters.AddWithValue("@UploadedBy", Convert.ToString(Program.UserName));
                    cmd.Parameters.AddWithValue("@UserId", Convert.ToInt32(Program.userid));
                    result = new DataTable();
                    adapter.Fill(result);
                }

                List<PlanRow> posted = new List<PlanRow>();
                string uploadRef = string.Empty;
                bool recalculated = result.Rows.Count != plan.Count;
                foreach (DataRow dr in result.Rows)
                {
                    PlanRow row = new PlanRow();
                    row.RowNo = dr["RowNo"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["RowNo"]);
                    row.ProductId = Convert.ToInt32(dr["ProductId"]);
                    row.RackId = Convert.ToInt32(dr["RackId"]);
                    row.RackCaption = Convert.ToString(dr["RackCaption"]);
                    row.Source = Convert.ToString(dr["Source"]);
                    row.CurrentStock = Convert.ToDecimal(dr["PreviousStock"]);
                    row.TargetStock = Convert.ToDecimal(dr["TargetStock"]);
                    row.Adjustment = Convert.ToDecimal(dr["Adjustment"]);
                    row.ResultingStock = Convert.ToDecimal(dr["ResultingStock"]);
                    row.UploadResult = Convert.ToString(dr["Result"]);
                    PlanRow previewed = FindPlanRow(row.ProductId, row.RackId);
                    if (previewed == null || previewed.Adjustment != row.Adjustment)
                        recalculated = true;
                    if (previewed != null)
                    {
                        row.ProductName = previewed.ProductName;
                        row.Uom = previewed.Uom;
                    }
                    posted.Add(row);
                    uploadRef = Convert.ToString(dr["UploadRef"]);
                }
                posted.Sort(ComparePosted);
                plan = posted;
                readyToUpload = false;
                BindRows();
                ShowSummary();
                lblStatus.Text = "Rack Wise Stock uploaded successfully. Upload Ref: " + uploadRef
                    + (recalculated ? ". Stock changed after validation; adjustments were recalculated from the latest stock." : ".")
                    + " Load or validate again before another upload.";
                lblStatus.ForeColor = Color.DarkGreen;
                MessageBox.Show("Rack Wise Stock uploaded successfully.\n\nUpload Ref: " + uploadRef, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                readyToUpload = false;
                ShowSummary();
                lblStatus.Text = "Upload failed. No stock was changed. Validate again before uploading.";
                lblStatus.ForeColor = Color.DarkRed;
                MessageBox.Show("Upload failed. No stock was changed.\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private int ComparePosted(PlanRow a, PlanRow b)
        {
            int ia = PlanIndex(a);
            int ib = PlanIndex(b);
            return ia.CompareTo(ib);
        }

        private int PlanIndex(PlanRow row)
        {
            for (int i = 0; i < plan.Count; i++)
            {
                if (plan[i].ProductId == row.ProductId && plan[i].RackId == row.RackId)
                    return i;
            }
            return int.MaxValue;
        }

        private void ValidateRows()
        {
            readyToUpload = false;
            plan = null;
            foreach (UploadRow row in rows)
            {
                row.Result = null;
                row.Remarks = null;
                row.CurrentStock = null;
            }

            if (rows.Count > 0)
            {
                using (SqlConnection con = new SqlConnection(Program.connection))
                using (SqlCommand cmd = new SqlCommand("dbo.Proc_RackWiseStockUpload_Validate", con))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    AddRowsParameter(cmd);
                    DataTable result = new DataTable();
                    adapter.Fill(result);
                    foreach (DataRow dr in result.Rows)
                    {
                        UploadRow row = FindRow(Convert.ToInt32(dr["RowNo"]));
                        if (row == null)
                            continue;
                        row.Result = Convert.ToString(dr["Result"]);
                        row.Remarks = Convert.ToString(dr["Remarks"]);
                        if (dr["RackId"] != DBNull.Value)
                            row.CurrentStock = Convert.ToDecimal(dr["CurrentStock"]);
                    }
                }
            }

            int valid = 0;
            foreach (UploadRow row in rows)
            {
                if (row.Result == "VALID")
                    valid++;
            }
            if (rows.Count > 0 && valid == rows.Count)
            {
                plan = LoadPlan();
                readyToUpload = PlanCoversExcel();
            }

            BindRows();
            ShowSummary();
            if (rows.Count == 0)
            {
                lblStatus.Text = "The Excel file has no data rows.";
                lblStatus.ForeColor = Color.DarkRed;
            }
            else if (readyToUpload)
            {
                int zeroRacks = ZeroRackCount();
                lblStatus.Text = "All records passed validation. Stock upload is ready."
                    + (zeroRacks > 0 ? "  " + zeroRacks + " allocated rack(s) not in the Excel will be RESET TO ZERO." : string.Empty);
                lblStatus.ForeColor = Color.DarkGreen;
            }
            else if (valid == rows.Count)
            {
                lblStatus.Text = "Validation failed. Could not resolve the allocated racks. Validate again.";
                lblStatus.ForeColor = Color.DarkRed;
            }
            else
            {
                lblStatus.Text = "Validation failed. Correct all errors before uploading stock.";
                lblStatus.ForeColor = Color.DarkRed;
            }
        }

        private List<PlanRow> LoadPlan()
        {
            List<PlanRow> result = new List<PlanRow>();
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.Proc_RackWiseStockUpload_Preview", con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                AddRowsParameter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                foreach (DataRow dr in table.Rows)
                {
                    PlanRow row = new PlanRow();
                    row.RowNo = dr["RowNo"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["RowNo"]);
                    row.ProductId = Convert.ToInt32(dr["ProductId"]);
                    row.ProductName = Convert.ToString(dr["ProductName"]);
                    row.Uom = Convert.ToString(dr["UOM"]);
                    row.RackId = Convert.ToInt32(dr["RackId"]);
                    row.RackCaption = Convert.ToString(dr["RackCaption"]);
                    row.Source = Convert.ToString(dr["Source"]);
                    row.CurrentStock = Convert.ToDecimal(dr["CurrentStock"]);
                    row.TargetStock = Convert.ToDecimal(dr["TargetStock"]);
                    row.Adjustment = Convert.ToDecimal(dr["Adjustment"]);
                    result.Add(row);
                }
            }
            return result;
        }

        private bool PlanCoversExcel()
        {
            if (plan == null)
                return false;
            int excelRows = 0;
            foreach (PlanRow row in plan)
            {
                if (row.Source == SourceExcel)
                    excelRows++;
            }
            return excelRows == rows.Count;
        }

        private int ZeroRackCount()
        {
            int count = 0;
            if (plan != null)
            {
                foreach (PlanRow row in plan)
                {
                    if (row.Source == SourceResetToZero)
                        count++;
                }
            }
            return count;
        }

        private void AddRowsParameter(SqlCommand cmd)
        {
            DataTable table = new DataTable();
            table.Columns.Add("RowNo", typeof(int));
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("UOM", typeof(string));
            table.Columns.Add("RackName", typeof(string));
            table.Columns.Add("Stock", typeof(decimal));
            foreach (UploadRow row in rows)
            {
                table.Rows.Add(row.RowNo,
                    row.ProductId.HasValue ? (object)row.ProductId.Value : DBNull.Value,
                    row.ProductName,
                    Limit(row.Uom, 400),
                    Limit(row.RackName, 400),
                    row.Stock.HasValue ? (object)row.Stock.Value : DBNull.Value);
            }
            SqlParameter parameter = cmd.Parameters.Add("@Rows", SqlDbType.Structured);
            parameter.TypeName = "dbo.RackWiseStockUploadRow";
            parameter.Value = table;
        }

        private static string Limit(string text, int length)
        {
            return text.Length > length ? text.Substring(0, length) : text;
        }

        private UploadRow FindRow(int rowNo)
        {
            foreach (UploadRow row in rows)
            {
                if (row.RowNo == rowNo)
                    return row;
            }
            return null;
        }

        private PlanRow FindPlanRow(int productId, int rackId)
        {
            if (plan == null)
                return null;
            foreach (PlanRow row in plan)
            {
                if (row.ProductId == productId && row.RackId == rackId)
                    return row;
            }
            return null;
        }

        private static string Qty(decimal? value)
        {
            return value.HasValue ? value.Value.ToString("0.000") : string.Empty;
        }

        private static string SignedQty(decimal value)
        {
            return value > 0 ? "+" + value.ToString("0.000") : value.ToString("0.000");
        }

        private void BindRows()
        {
            view = NewViewTable();
            if (plan != null)
            {
                foreach (PlanRow row in plan)
                {
                    view.Rows.Add(row.RowNo.HasValue ? row.RowNo.Value.ToString() : string.Empty,
                        row.ProductId.ToString(), row.ProductName, row.Uom, row.RackCaption,
                        Qty(row.CurrentStock), Qty(row.TargetStock), SignedQty(row.Adjustment), row.Source,
                        "VALID",
                        row.Source == SourceResetToZero ? "Allocated rack not in Excel - will be reset to zero." : string.Empty,
                        Qty(row.ResultingStock), row.UploadResult);
                }
            }
            else
            {
                foreach (UploadRow row in rows)
                {
                    view.Rows.Add(row.RowNo.ToString(), row.ProductIdText, row.ProductName, row.Uom, row.RackName,
                        Qty(row.CurrentStock), row.StockText, string.Empty, SourceExcel,
                        row.Result, row.Remarks, string.Empty, string.Empty);
                }
            }
            dgvRows.DataSource = view;
            FormatGrid();
        }

        private void ShowSummary()
        {
            int valid = 0;
            int errors = 0;
            foreach (UploadRow row in rows)
            {
                if (row.Result == "VALID")
                    valid++;
                else if (row.Result == "ERROR")
                    errors++;
            }
            lblTotal.Text = "Total Rows     : " + rows.Count;
            lblValid.Text = "Valid Rows     : " + valid;
            lblError.Text = "Error Rows     : " + errors;
            lblZero.Text = plan == null ? string.Empty : "Additional Racks to Zero : " + ZeroRackCount();
            lblAffected.Text = plan == null ? string.Empty : "Total Affected Racks     : " + plan.Count;
            btnUpload.Enabled = readyToUpload && plan != null && rows.Count > 0 && valid == rows.Count && errors == 0;
            btnValidate.Enabled = rows.Count > 0;
        }

        private void dgvRows_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            string column = dgvRows.Columns[e.ColumnIndex].Name;
            string value = Convert.ToString(e.Value);
            if (column == "Source")
            {
                if (value == SourceResetToZero)
                {
                    e.CellStyle.ForeColor = Color.DarkRed;
                    e.CellStyle.BackColor = Color.LightYellow;
                    e.CellStyle.Font = new Font(dgvRows.Font, FontStyle.Bold);
                }
                return;
            }
            if (column == "Adjustment")
            {
                if (value.StartsWith("+"))
                    e.CellStyle.ForeColor = Color.DarkGreen;
                else if (value.StartsWith("-"))
                    e.CellStyle.ForeColor = Color.DarkRed;
                return;
            }
            if (column != "Validation Result" && column != "Upload Result")
                return;
            if (value == "VALID" || value == "SUCCESS")
            {
                e.CellStyle.ForeColor = Color.DarkGreen;
                e.CellStyle.BackColor = Color.Honeydew;
                e.CellStyle.Font = new Font(dgvRows.Font, FontStyle.Bold);
            }
            else if (value == "ERROR")
            {
                e.CellStyle.ForeColor = Color.DarkRed;
                e.CellStyle.BackColor = Color.MistyRose;
                e.CellStyle.Font = new Font(dgvRows.Font, FontStyle.Bold);
            }
        }

        public static List<UploadRow> ReadExcel(string path, out string error)
        {
            error = null;
            string extension = Path.GetExtension(path).ToLowerInvariant();
            string connection;
            if (extension == ".xls")
                connection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
            else if (extension == ".xlsx")
                connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0 Xml;HDR=Yes;IMEX=1'";
            else
            {
                error = "Please select an Excel file (.xls or .xlsx).";
                return null;
            }

            DataTable sheet = null;
            string firstMissing = null;
            int matchingSheets = 0;
            using (OleDbConnection con = new OleDbConnection(connection))
            {
                con.Open();
                DataTable sheets = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow sheetRow in sheets.Rows)
                {
                    string name = Convert.ToString(sheetRow["TABLE_NAME"]);
                    if (!name.EndsWith("$") && !name.EndsWith("$'"))
                        continue;
                    DataTable data = new DataTable();
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM [" + name + "]", con))
                        adapter.Fill(data);
                    string missing = MissingColumns(data);
                    if (missing == null)
                    {
                        matchingSheets++;
                        if (sheet == null)
                            sheet = data;
                    }
                    else if (firstMissing == null)
                    {
                        firstMissing = missing;
                    }
                }
            }

            if (matchingSheets > 1)
            {
                error = "More than one worksheet contains the Rack Wise Stock columns. Keep only one worksheet in the Excel file.";
                return null;
            }
            if (sheet == null)
            {
                error = firstMissing ?? "The Excel file does not contain a worksheet.";
                return null;
            }

            Dictionary<string, DataColumn> columns = new Dictionary<string, DataColumn>(StringComparer.OrdinalIgnoreCase);
            foreach (DataColumn column in sheet.Columns)
                columns[column.ColumnName.Trim()] = column;

            List<UploadRow> result = new List<UploadRow>();
            for (int i = 0; i < sheet.Rows.Count; i++)
            {
                DataRow dr = sheet.Rows[i];
                UploadRow row = new UploadRow();
                row.RowNo = i + 2;
                row.ProductIdText = CellText(dr[columns["Product ID"]]);
                row.ProductName = CellText(dr[columns["Product Name"]]);
                row.Uom = CellText(dr[columns["UOM"]]);
                row.RackName = CellText(dr[columns["Rack Name"]]);
                row.StockText = CellText(dr[columns["Stock"]]);
                if (row.ProductIdText.Length == 0 && row.ProductName.Length == 0 && row.Uom.Length == 0
                    && row.RackName.Length == 0 && row.StockText.Length == 0)
                    continue;

                int productId;
                if (int.TryParse(row.ProductIdText, NumberStyles.Integer, CultureInfo.InvariantCulture, out productId))
                    row.ProductId = productId;
                row.Stock = ParseStock(row.StockText);
                result.Add(row);
            }
            return result;
        }

        private static string MissingColumns(DataTable data)
        {
            List<string> names = new List<string>();
            foreach (DataColumn column in data.Columns)
                names.Add(column.ColumnName.Trim().ToUpperInvariant());
            List<string> messages = new List<string>();
            foreach (string required in RequiredColumns)
            {
                if (!names.Contains(required.ToUpperInvariant()))
                    messages.Add("Required Excel column '" + required + "' is missing.");
            }
            return messages.Count == 0 ? null : string.Join("\n", messages.ToArray());
        }

        private static string CellText(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            if (value is double)
                return ((double)value).ToString("0.############", CultureInfo.InvariantCulture);
            if (value is decimal)
                return ((decimal)value).ToString(CultureInfo.InvariantCulture);
            return Convert.ToString(value, CultureInfo.InvariantCulture).Trim();
        }

        public static decimal? ParseStock(string text)
        {
            decimal stock;
            if (text == null || text.Trim().Length == 0)
                return null;
            if (!decimal.TryParse(text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out stock))
                return null;
            if (decimal.Round(stock, 3) != stock || Math.Abs(stock) >= 1000000000000000m)
                return null;
            return stock;
        }

        public class UploadRow
        {
            public int RowNo;
            public string ProductIdText;
            public int? ProductId;
            public string ProductName;
            public string Uom;
            public string RackName;
            public string StockText;
            public decimal? Stock;
            public string Result;
            public string Remarks;
            public decimal? CurrentStock;
        }

        public class PlanRow
        {
            public int? RowNo;
            public int ProductId;
            public string ProductName;
            public string Uom;
            public int RackId;
            public string RackCaption;
            public string Source;
            public decimal CurrentStock;
            public decimal TargetStock;
            public decimal Adjustment;
            public decimal? ResultingStock;
            public string UploadResult;
        }
    }
}
