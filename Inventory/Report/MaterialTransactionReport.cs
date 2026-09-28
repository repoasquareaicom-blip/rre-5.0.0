using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Report
{
    public class MaterialTransactionReport : Form
    {
        private static readonly string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private CheckBox chkTillDate;
        private TextBox txtProduct;
        private Button btnFindProduct;
        private Button btnAllProducts;
        private ComboBox cmbLocation;
        private ComboBox cmbRack;
        private ComboBox cmbMovement;
        private ComboBox cmbEntity;
        private Button btnView;
        private Button btnReset;
        private Button btnExport;
        private Label lblOpening;
        private Label lblIn;
        private Label lblOut;
        private Label lblClosing;
        private Label lblRackNotice;
        private TabControl tabs;
        private DataGridView dgvTransactions;
        private DataGridView dgvRacks;
        private DataTable movements = new DataTable();
        private DataTable openingRacks = new DataTable();
        private DataTable periodRacks = new DataTable();
        private DataTable rackSummary = new DataTable();
        private DataTable rackMaster = new DataTable();
        private int selectedProductId;
        private bool ledgerMode;

        public MaterialTransactionReport()
        {
            BuildUi();
            Load += MaterialTransactionReport_Load;
        }

        private void MaterialTransactionReport_Load(object sender, EventArgs e)
        {
            BindLocations();
            BindRacks();
            BindEntities();
            ResetFilters(true);
        }

        private void BuildUi()
        {
            Text = "Material Transaction Report";
            Name = "MaterialTransactionReport";
            WindowState = FormWindowState.Maximized;
            Font = new Font("Microsoft Sans Serif", 9F);
            StartPosition = FormStartPosition.CenterScreen;

            GroupBox filters = new GroupBox();
            filters.Text = "Filters";
            filters.Dock = DockStyle.Top;
            filters.Height = 132;

            Label lblFrom = MakeLabel("From Date", 12, 28);
            dtpFrom = new DateTimePicker();
            dtpFrom.Format = DateTimePickerFormat.Short;
            dtpFrom.Location = new Point(84, 24);
            dtpFrom.Width = 110;

            Label lblTo = MakeLabel("To Date", 206, 28);
            dtpTo = new DateTimePicker();
            dtpTo.Format = DateTimePickerFormat.Short;
            dtpTo.Location = new Point(264, 24);
            dtpTo.Width = 110;

            chkTillDate = new CheckBox();
            chkTillDate.Text = "Till Date";
            chkTillDate.Location = new Point(386, 26);
            chkTillDate.AutoSize = true;
            chkTillDate.CheckedChanged += delegate { dtpTo.Enabled = !chkTillDate.Checked; };

            Label lblLocation = MakeLabel("Location", 478, 28);
            cmbLocation = new ComboBox();
            cmbLocation.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLocation.Location = new Point(540, 24);
            cmbLocation.Width = 160;
            cmbLocation.SelectedIndexChanged += cmbLocation_SelectedIndexChanged;

            Label lblRack = MakeLabel("Rack", 712, 28);
            cmbRack = new ComboBox();
            cmbRack.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRack.Location = new Point(752, 24);
            cmbRack.Width = 160;

            Label lblProduct = MakeLabel("Product", 12, 62);
            txtProduct = new TextBox();
            txtProduct.Location = new Point(84, 58);
            txtProduct.Width = 292;
            txtProduct.ReadOnly = true;
            txtProduct.Text = "All Products";

            btnFindProduct = MakeButton("Find", 384, 56, 64);
            btnFindProduct.Click += btnFindProduct_Click;
            btnAllProducts = MakeButton("All", 454, 56, 52);
            btnAllProducts.Click += delegate { ClearProduct(); };

            Label lblMove = MakeLabel("Movement", 522, 62);
            cmbMovement = new ComboBox();
            cmbMovement.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMovement.Location = new Point(592, 58);
            cmbMovement.Width = 90;
            cmbMovement.Items.Add("All");
            cmbMovement.Items.Add("IN");
            cmbMovement.Items.Add("OUT");

            Label lblEntity = MakeLabel("Entity", 696, 62);
            cmbEntity = new ComboBox();
            cmbEntity.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEntity.Location = new Point(752, 58);
            cmbEntity.Width = 160;

            btnView = MakeButton("View Report", 12, 94, 116);
            btnView.Click += delegate { LoadReport(); };
            btnReset = MakeButton("Reset", 136, 94, 84);
            btnReset.Click += delegate { ResetFilters(true); };
            btnExport = MakeButton("Export to Excel", 228, 94, 132);
            btnExport.Click += btnExport_Click;

            filters.Controls.AddRange(new Control[] {
                lblFrom, dtpFrom, lblTo, dtpTo, chkTillDate, lblLocation, cmbLocation, lblRack, cmbRack,
                lblProduct, txtProduct, btnFindProduct, btnAllProducts, lblMove, cmbMovement, lblEntity, cmbEntity,
                btnView, btnReset, btnExport
            });

            TableLayoutPanel summary = new TableLayoutPanel();
            summary.Dock = DockStyle.Top;
            summary.Height = 78;
            summary.ColumnCount = 4;
            summary.RowCount = 1;
            summary.Padding = new Padding(8, 6, 8, 6);
            for (int i = 0; i < 4; i++)
                summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            lblOpening = AddSummaryBox(summary, 0, "Opening Stock");
            lblIn = AddSummaryBox(summary, 1, "Total IN");
            lblOut = AddSummaryBox(summary, 2, "Total OUT");
            lblClosing = AddSummaryBox(summary, 3, "Closing Stock");

            dgvTransactions = MakeGrid();
            dgvTransactions.CellDoubleClick += dgvTransactions_CellDoubleClick;
            dgvRacks = MakeGrid();
            lblRackNotice = new Label();
            lblRackNotice.Dock = DockStyle.Fill;
            lblRackNotice.TextAlign = ContentAlignment.MiddleCenter;
            lblRackNotice.Text = "Select one product to see location and rack stock.";
            lblRackNotice.Visible = false;

            tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            TabPage tabTx = new TabPage("Transactions");
            TabPage tabRack = new TabPage("Location / Rack Summary");
            tabTx.Controls.Add(dgvTransactions);
            tabRack.Controls.Add(dgvRacks);
            tabRack.Controls.Add(lblRackNotice);
            tabs.TabPages.Add(tabTx);
            tabs.TabPages.Add(tabRack);

            Controls.Add(tabs);
            Controls.Add(summary);
            Controls.Add(filters);
        }

        private static Label MakeLabel(string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.AutoSize = true;
            return label;
        }

        private static Button MakeButton(string text, int x, int y, int width)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(width, 26);
            button.FlatStyle = FlatStyle.Popup;
            return button;
        }

        private static Label AddSummaryBox(TableLayoutPanel summary, int column, string caption)
        {
            Panel box = new Panel();
            box.BorderStyle = BorderStyle.FixedSingle;
            box.Dock = DockStyle.Fill;
            box.Margin = new Padding(6, 2, 6, 2);
            box.BackColor = SystemColors.Window;
            Label title = new Label();
            title.Text = caption;
            title.Dock = DockStyle.Top;
            title.Height = 18;
            title.Padding = new Padding(8, 2, 0, 0);
            title.ForeColor = SystemColors.ControlText;
            Label value = new Label();
            value.Dock = DockStyle.Fill;
            value.TextAlign = ContentAlignment.MiddleLeft;
            value.Padding = new Padding(8, 0, 4, 0);
            value.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            value.Text = "Not available";
            box.Controls.Add(value);
            box.Controls.Add(title);
            summary.Controls.Add(box, column, 0);
            return value;
        }

        private static DataGridView MakeGrid()
        {
            DataGridView grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            grid.BackgroundColor = Color.White;
            grid.DefaultCellStyle.BackColor = Color.Gainsboro;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            return grid;
        }

        private void BindLocations()
        {
            DataTable table = Query(@"SELECT CONVERT(VARCHAR(100), LocationId) AS LocationId, LocationName
FROM dbo.LocationMaster
WHERE ISNULL(IsActive, 0) = 1
ORDER BY DisplayOrder, LocationName");
            cmbLocation.Items.Clear();
            cmbLocation.Items.Add(new LookupItem("", "All Locations"));
            foreach (DataRow row in table.Rows)
                cmbLocation.Items.Add(new LookupItem(Convert.ToString(row["LocationId"]), Convert.ToString(row["LocationName"])));
            cmbLocation.SelectedIndex = 0;
        }

        private void BindRacks()
        {
            rackMaster = Query(@"SELECT RackId, CONVERT(VARCHAR(100), LocationId) AS LocationId, RackCaption
FROM dbo.RackMaster
WHERE ISNULL(IsActive, 0) = 1
ORDER BY DisplayOrder, RackCaption");
            FillRackCombo();
        }

        private void FillRackCombo()
        {
            string locationId = SelectedLocationId();
            cmbRack.Items.Clear();
            cmbRack.Items.Add(new LookupItem("", "All Racks"));
            foreach (DataRow row in rackMaster.Rows)
            {
                if (locationId.Length > 0 && Convert.ToString(row["LocationId"]) != locationId)
                    continue;
                cmbRack.Items.Add(new LookupItem(Convert.ToString(row["RackId"]), Convert.ToString(row["RackCaption"])));
            }
            cmbRack.SelectedIndex = 0;
        }

        private void BindEntities()
        {
            List<string> values = new List<string>();

            DataTable found = Query("SELECT DISTINCT TranscationType FROM dbo.MaterialTranscation WHERE TranscationType IS NOT NULL AND LTRIM(RTRIM(TranscationType)) <> ''");
            foreach (DataRow row in found.Rows)
            {
                string value = Convert.ToString(row["TranscationType"]).Trim();
                if (value.Length > 0 && !values.Contains(value))
                    values.Add(value);
            }
            values.Sort(StringComparer.OrdinalIgnoreCase);

            cmbEntity.Items.Clear();
            cmbEntity.Items.Add(new LookupItem("", "All"));
            foreach (string value in values)
                cmbEntity.Items.Add(new LookupItem(value, EntityLabel(value)));
            cmbEntity.SelectedIndex = 0;
        }

        private void ResetFilters(bool reload)
        {
            dtpFrom.Value = DateTime.Today;
            dtpTo.Value = DateTime.Today;
            chkTillDate.Checked = false;
            dtpTo.Enabled = true;
            ClearProduct();
            if (cmbLocation.Items.Count > 0)
                cmbLocation.SelectedIndex = 0;
            if (cmbMovement.Items.Count > 0)
                cmbMovement.SelectedIndex = 0;
            if (cmbEntity.Items.Count > 0)
                cmbEntity.SelectedIndex = 0;
            FillRackCombo();
            if (reload)
                LoadReport();
        }

        private void ClearProduct()
        {
            selectedProductId = 0;
            txtProduct.Text = "All Products";
            txtProduct.BackColor = SystemColors.Window;
        }

        private void cmbLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rackMaster == null || cmbRack == null)
                return;
            FillRackCombo();
        }

        private void btnFindProduct_Click(object sender, EventArgs e)
        {
            using (MaterialProductPick pick = new MaterialProductPick())
            {
                if (pick.ShowDialog(this) == DialogResult.OK && pick.ProductId > 0)
                {
                    selectedProductId = pick.ProductId;
                    txtProduct.Text = pick.ProductName;
                    txtProduct.BackColor = SystemColors.Info;
                }
            }
        }

        private void LoadReport()
        {
            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toExclusive = chkTillDate.Checked ? DateTime.Now : dtpTo.Value.Date.AddDays(1);
            if (!chkTillDate.Checked && dtpTo.Value.Date < fromDate)
            {
                MessageBox.Show("To Date cannot be before From Date.");
                return;
            }

            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand("dbo.GetMaterialTransactionReport", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate;
                cmd.Parameters.Add("@ToDateExclusive", SqlDbType.DateTime).Value = toExclusive;
                cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = selectedProductId > 0 ? (object)selectedProductId : DBNull.Value;
                string locationId = SelectedLocationId();
                cmd.Parameters.Add("@LocationId", SqlDbType.VarChar, 100).Value = locationId.Length == 0 ? (object)DBNull.Value : locationId;
                int rackId = SelectedRackId();
                cmd.Parameters.Add("@RackId", SqlDbType.Int).Value = rackId > 0 ? (object)rackId : DBNull.Value;

                DataSet data = new DataSet();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    adapter.Fill(data);

                movements = data.Tables.Count > 0 ? data.Tables[0] : new DataTable();
                openingRacks = data.Tables.Count > 1 ? data.Tables[1] : new DataTable();
                periodRacks = data.Tables.Count > 2 ? data.Tables[2] : new DataTable();
            }

            ledgerMode = selectedProductId > 0;
            BindTransactionGrid();
            BindRackSummary();
        }

        private DataTable FilteredMovements()
        {
            string movement = cmbMovement.SelectedItem == null ? "All" : cmbMovement.SelectedItem.ToString();
            string entity = SelectedEntity();
            DataTable filtered = movements.Clone();
            foreach (DataRow row in movements.Rows)
            {
                if (movement != "All" && !string.Equals(Convert.ToString(row["MovementType"]), movement, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (entity.Length > 0 && !string.Equals(Convert.ToString(row["TranscationType"]), entity, StringComparison.OrdinalIgnoreCase))
                    continue;
                filtered.ImportRow(row);
            }
            return filtered;
        }

        private void BindTransactionGrid()
        {
            DataTable view = new DataTable();
            DataTable source = FilteredMovements();

            if (!ledgerMode)
            {
                view.Columns.Add("Date");
                view.Columns.Add("Time");
                view.Columns.Add("Product Code");
                view.Columns.Add("Product Name");
                view.Columns.Add("Location");
                view.Columns.Add("Rack");
                view.Columns.Add("IN / OUT");
                view.Columns.Add("Quantity");
                view.Columns.Add("Entity");
                view.Columns.Add("Reference");
                view.Columns.Add("User");
                view.Columns.Add("RowKey");
            }
            else
            {
                view.Columns.Add("Date/Time");
                view.Columns.Add("Location");
                view.Columns.Add("Rack");
                view.Columns.Add("Transaction Type");
                view.Columns.Add("Reference");
                view.Columns.Add("IN");
                view.Columns.Add("OUT");
                view.Columns.Add("Balance");
                view.Columns.Add("RowKey");
            }

            decimal opening = OpeningTotal();
            decimal periodIn = 0;
            decimal periodOut = 0;
            foreach (DataRow row in periodRacks.Rows)
            {
                periodIn += ToDecimal(row["InQty"]);
                periodOut += ToDecimal(row["OutQty"]);
            }
            Dictionary<string, decimal> runningBalance = new Dictionary<string, decimal>();
            if (ledgerMode)
            {
                decimal running = opening;
                foreach (DataRow row in movements.Rows)
                {
                    decimal qty = ToDecimal(row["Quantity"]);
                    running += IsIn(row) ? qty : -qty;
                    runningBalance[Convert.ToString(row["ID"])] = running;
                }

                DataRow openRow = view.NewRow();
                openRow["Date/Time"] = "Opening";
                openRow["Transaction Type"] = "Opening Balance";
                openRow["Balance"] = FormatQty(opening);
                openRow["RowKey"] = "";
                view.Rows.Add(openRow);
            }

            foreach (DataRow row in source.Rows)
            {
                bool inbound = IsIn(row);
                decimal qty = ToDecimal(row["Quantity"]);

                DataRow line = view.NewRow();
                if (!ledgerMode)
                {
                    DateTime when = Convert.ToDateTime(row["TranscationDate"]);
                    line["Date"] = when.ToString("dd-MM-yyyy");
                    line["Time"] = when.ToString("HH:mm");
                    line["Product Code"] = row["ItemCode"];
                    line["Product Name"] = row["ProductName"];
                    line["Location"] = row["LocationName"];
                    line["Rack"] = row["RackCaption"];
                    line["IN / OUT"] = Convert.ToString(row["MovementType"]).Trim().ToUpperInvariant();
                    line["Quantity"] = FormatQty(qty);
                    line["Entity"] = EntityLabel(Convert.ToString(row["TranscationType"]));
                    line["Reference"] = row["TransId"];
                    line["User"] = row["UserName"];
                    line["RowKey"] = row["ID"];
                }
                else
                {
                    DateTime when = Convert.ToDateTime(row["TranscationDate"]);
                    line["Date/Time"] = when.ToString("dd-MM-yyyy HH:mm");
                    line["Location"] = row["LocationName"];
                    line["Rack"] = row["RackCaption"];
                    line["Transaction Type"] = EntityLabel(Convert.ToString(row["TranscationType"]));
                    line["Reference"] = row["TransId"];
                    line["IN"] = inbound ? FormatQty(qty) : "";
                    line["OUT"] = inbound ? "" : FormatQty(qty);
                    string rowId = Convert.ToString(row["ID"]);
                    line["Balance"] = runningBalance.ContainsKey(rowId) ? FormatQty(runningBalance[rowId]) : "";
                    line["RowKey"] = row["ID"];
                }
                view.Rows.Add(line);
            }

            dgvTransactions.DataSource = view;
            if (dgvTransactions.Columns.Contains("RowKey"))
                dgvTransactions.Columns["RowKey"].Visible = false;
            AlignQtyColumns(dgvTransactions);
            SizeTransactionColumns();
            if (ledgerMode && dgvTransactions.Rows.Count > 0)
            {
                dgvTransactions.Rows[0].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
                dgvTransactions.Rows[0].DefaultCellStyle.BackColor = SystemColors.Info;
                dgvTransactions.Rows[0].DefaultCellStyle.SelectionBackColor = SystemColors.Info;
                dgvTransactions.Rows[0].DefaultCellStyle.SelectionForeColor = SystemColors.ControlText;
            }
            dgvTransactions.ClearSelection();

            decimal closing = opening + periodIn - periodOut;
            ShowSummary(lblOpening, FormatQty(opening), true);
            ShowSummary(lblIn, FormatQty(periodIn), true);
            ShowSummary(lblOut, FormatQty(periodOut), true);
            ShowSummary(lblClosing, FormatQty(closing), true);
        }

        private void BindRackSummary()
        {
            rackSummary = new DataTable();
            rackSummary.Columns.Add("Location");
            rackSummary.Columns.Add("Rack");
            rackSummary.Columns.Add("Opening");
            rackSummary.Columns.Add("IN");
            rackSummary.Columns.Add("OUT");
            rackSummary.Columns.Add("Closing");

            if (!ledgerMode)
            {
                dgvRacks.Visible = false;
                lblRackNotice.Visible = true;
                dgvRacks.DataSource = rackSummary;
                return;
            }

            lblRackNotice.Visible = false;
            dgvRacks.Visible = true;

            Dictionary<string, RackLine> lines = new Dictionary<string, RackLine>();
            foreach (DataRow row in openingRacks.Rows)
                TouchRack(lines, row["LocationName"], row["RackCaption"]).Opening += ToDecimal(row["OpeningQty"]);
            foreach (DataRow row in periodRacks.Rows)
            {
                RackLine line = TouchRack(lines, row["LocationName"], row["RackCaption"]);
                line.InQty += ToDecimal(row["InQty"]);
                line.OutQty += ToDecimal(row["OutQty"]);
            }

            List<string> keys = new List<string>(lines.Keys);
            keys.Sort(StringComparer.OrdinalIgnoreCase);
            foreach (string key in keys)
            {
                RackLine line = lines[key];
                DataRow row = rackSummary.NewRow();
                row["Location"] = line.Location;
                row["Rack"] = line.Rack;
                row["Opening"] = FormatQty(line.Opening);
                row["IN"] = FormatQty(line.InQty);
                row["OUT"] = FormatQty(line.OutQty);
                row["Closing"] = FormatQty(line.Opening + line.InQty - line.OutQty);
                rackSummary.Rows.Add(row);
            }
            dgvRacks.DataSource = rackSummary;
            AlignQtyColumns(dgvRacks);
            SizeRackColumns();
        }

        private static void ShowSummary(Label label, string text, bool available)
        {
            label.Text = text;
            label.ForeColor = available ? SystemColors.ControlText : SystemColors.GrayText;
        }

        private void SizeTransactionColumns()
        {
            if (!ledgerMode)
            {
                SetWidth(dgvTransactions, "Date", 90);
                SetWidth(dgvTransactions, "Time", 55);
                SetWidth(dgvTransactions, "Product Code", 110);
                SetWidth(dgvTransactions, "Product Name", 220);
                SetWidth(dgvTransactions, "Location", 100);
                SetWidth(dgvTransactions, "Rack", 70);
                SetWidth(dgvTransactions, "IN / OUT", 70);
                SetWidth(dgvTransactions, "Quantity", 80);
                SetWidth(dgvTransactions, "Entity", 120);
                SetWidth(dgvTransactions, "Reference", 150);
                SetWidth(dgvTransactions, "User", 80);
            }
            else
            {
                SetWidth(dgvTransactions, "Date/Time", 130);
                SetWidth(dgvTransactions, "Location", 110);
                SetWidth(dgvTransactions, "Rack", 70);
                SetWidth(dgvTransactions, "Transaction Type", 140);
                SetWidth(dgvTransactions, "Reference", 150);
                SetWidth(dgvTransactions, "IN", 70);
                SetWidth(dgvTransactions, "OUT", 70);
                SetWidth(dgvTransactions, "Balance", 90);
            }
        }

        private void SizeRackColumns()
        {
            SetWidth(dgvRacks, "Location", 140);
            SetWidth(dgvRacks, "Rack", 80);
            SetWidth(dgvRacks, "Opening", 90);
            SetWidth(dgvRacks, "IN", 70);
            SetWidth(dgvRacks, "OUT", 70);
            SetWidth(dgvRacks, "Closing", 90);
        }

        private static void SetWidth(DataGridView grid, string columnName, int width)
        {
            if (!grid.Columns.Contains(columnName))
                return;
            DataGridViewColumn column = grid.Columns[columnName];
            if (column == null)
                return;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Width = width;
        }

        private static RackLine TouchRack(Dictionary<string, RackLine> lines, object location, object rack)
        {
            string locationName = Convert.ToString(location);
            string rackName = Convert.ToString(rack);
            string key = locationName + "|" + rackName;
            RackLine line;
            if (!lines.TryGetValue(key, out line))
            {
                line = new RackLine();
                line.Location = locationName;
                line.Rack = rackName;
                lines[key] = line;
            }
            return line;
        }

        private void dgvTransactions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            DataGridViewRow gridRow = dgvTransactions.Rows[e.RowIndex];
            string key = Convert.ToString(gridRow.Cells["RowKey"].Value);
            if (key.Length == 0)
                return;
            DataRow source = FindMovement(key);
            if (source == null)
                return;
            using (MaterialTransactionDetail detail = new MaterialTransactionDetail(source))
                detail.ShowDialog(this);
        }

        private DataRow FindMovement(string id)
        {
            foreach (DataRow row in movements.Rows)
            {
                if (Convert.ToString(row["ID"]) == id)
                    return row;
            }
            return null;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DataTable grid = dgvTransactions.DataSource as DataTable;
            if (grid == null || grid.Rows.Count == 0)
            {
                MessageBox.Show("No data available to export.");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Excel Files (*.xls)|*.xls";
            dialog.FileName = "Material_Transaction_Report.xls";
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            StringBuilder export = new StringBuilder();
            export.Append(lblOpening.Text).Append("\t").Append(lblIn.Text).Append("\t").Append(lblOut.Text).Append("\t").Append(lblClosing.Text).Append("\r\n\r\n");
            AppendTable(export, grid);
            if (ledgerMode && rackSummary.Rows.Count > 0)
            {
                export.Append("\r\nLocation / Rack Summary\r\n");
                AppendTable(export, rackSummary);
            }
            System.IO.File.WriteAllText(dialog.FileName, export.ToString(), Encoding.Unicode);
        }

        private static void AppendTable(StringBuilder export, DataTable table)
        {
            bool first = true;
            foreach (DataColumn column in table.Columns)
            {
                if (column.ColumnName == "RowKey")
                    continue;
                if (!first)
                    export.Append("\t");
                export.Append(column.ColumnName);
                first = false;
            }
            export.Append("\r\n");
            foreach (DataRow row in table.Rows)
            {
                first = true;
                foreach (DataColumn column in table.Columns)
                {
                    if (column.ColumnName == "RowKey")
                        continue;
                    if (!first)
                        export.Append("\t");
                    export.Append(Convert.ToString(row[column]).Replace("\r", " ").Replace("\n", " ").Replace("\t", " "));
                    first = false;
                }
                export.Append("\r\n");
            }
        }

        private decimal OpeningTotal()
        {
            decimal total = 0;
            foreach (DataRow row in openingRacks.Rows)
                total += ToDecimal(row["OpeningQty"]);
            return total;
        }

        private string SelectedLocationId()
        {
            LookupItem item = cmbLocation.SelectedItem as LookupItem;
            return item == null ? "" : item.Value;
        }

        private int SelectedRackId()
        {
            LookupItem item = cmbRack.SelectedItem as LookupItem;
            int rackId;
            if (item == null || !int.TryParse(item.Value, out rackId))
                return 0;
            return rackId;
        }

        private string SelectedEntity()
        {
            LookupItem item = cmbEntity.SelectedItem as LookupItem;
            return item == null ? "" : item.Value;
        }

        private static bool IsIn(DataRow row)
        {
            return string.Equals(Convert.ToString(row["MovementType"]), "IN", StringComparison.OrdinalIgnoreCase);
        }

        private static decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            return Convert.ToDecimal(value);
        }

        private static string FormatQty(decimal value)
        {
            return value.ToString("0.000");
        }

        private static void AlignQtyColumns(DataGridView grid)
        {
            string[] names = new string[] { "Quantity", "IN", "OUT", "Balance", "Opening", "Closing" };
            foreach (string name in names)
            {
                if (grid.Columns.Contains(name))
                    grid.Columns[name].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        public static string EntityLabel(string value)
        {
            if (string.Equals(value, "Purchase", StringComparison.OrdinalIgnoreCase))
                return "Purchase Receipt";
            if (string.Equals(value, "PDI-REVERSE", StringComparison.OrdinalIgnoreCase))
                return "PDI Reverse";
            if (string.Equals(value, "Product Movement", StringComparison.OrdinalIgnoreCase))
                return "Stock Movement";
            if (string.Equals(value, "IssuedReceived", StringComparison.OrdinalIgnoreCase))
                return "Issued/Received";
            if (string.Equals(value, "RECEIPT NOTE", StringComparison.OrdinalIgnoreCase))
                return "Receipt Note";
            if (string.Equals(value, "DELIVERY NOTE", StringComparison.OrdinalIgnoreCase))
                return "Delivery Note";
            return value;
        }

        private static DataTable Query(string sql)
        {
            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        private class LookupItem
        {
            public string Value;
            public string Text;
            public LookupItem(string value, string text)
            {
                Value = value;
                Text = text;
            }
            public override string ToString()
            {
                return Text;
            }
        }

        private class RackLine
        {
            public string Location;
            public string Rack;
            public decimal Opening;
            public decimal InQty;
            public decimal OutQty;
        }
    }

    internal class MaterialProductPick : Form
    {
        private static readonly string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        private TextBox txtSearch;
        private ListBox list;
        public int ProductId;
        public string ProductName = "";

        public MaterialProductPick()
        {
            Text = "Find Product";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(520, 360);
            Font = new Font("Microsoft Sans Serif", 9F);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(12, 12);
            txtSearch.Width = 390;
            Button find = new Button();
            find.Text = "Search";
            find.Location = new Point(410, 10);
            find.Size = new Size(90, 26);
            find.FlatStyle = FlatStyle.Popup;
            find.Click += delegate { Search(); };
            txtSearch.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Search();
                    e.SuppressKeyPress = true;
                }
            };

            list = new ListBox();
            list.Location = new Point(12, 46);
            list.Size = new Size(488, 260);
            list.DoubleClick += delegate { Accept(); };

            Button ok = new Button();
            ok.Text = "Select";
            ok.Location = new Point(320, 318);
            ok.Size = new Size(80, 28);
            ok.Click += delegate { Accept(); };
            Button cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Location = new Point(408, 318);
            cancel.Size = new Size(80, 28);
            cancel.DialogResult = DialogResult.Cancel;
            CancelButton = cancel;

            Controls.Add(txtSearch);
            Controls.Add(find);
            Controls.Add(list);
            Controls.Add(ok);
            Controls.Add(cancel);
        }

        private void Search()
        {
            string text = txtSearch.Text.Trim();
            list.Items.Clear();
            if (text.Length == 0)
                return;
            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 40 id, ISNULL(ItemCode,'') AS ItemCode, ISNULL(DisplayName,'') AS DisplayName
FROM dbo.ProductMaster
WHERE DisplayName LIKE @Text OR ItemCode LIKE @Text OR CONVERT(VARCHAR(20), id) = @Exact
ORDER BY DisplayName", con))
            {
                cmd.Parameters.Add("@Text", SqlDbType.VarChar, 200).Value = "%" + text + "%";
                cmd.Parameters.Add("@Exact", SqlDbType.VarChar, 20).Value = text;
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Items.Add(new ProductHit(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                    }
                }
            }
        }

        private void Accept()
        {
            ProductHit hit = list.SelectedItem as ProductHit;
            if (hit == null)
                return;
            ProductId = hit.Id;
            ProductName = hit.ToString();
            DialogResult = DialogResult.OK;
            Close();
        }

        private class ProductHit
        {
            public int Id;
            public string Code;
            public string Name;
            public ProductHit(int id, string code, string name)
            {
                Id = id;
                Code = code;
                Name = name;
            }
            public override string ToString()
            {
                if (Code.Length == 0)
                    return Name;
                return Code + " - " + Name;
            }
        }
    }

    internal class MaterialTransactionDetail : Form
    {
        private static readonly string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

        public MaterialTransactionDetail(DataRow row)
        {
            Text = "Transaction Detail";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(640, 460);
            Font = new Font("Microsoft Sans Serif", 9F);

            TextBox body = new TextBox();
            body.Multiline = true;
            body.ReadOnly = true;
            body.ScrollBars = ScrollBars.Vertical;
            body.Location = new Point(12, 12);
            body.Size = new Size(616, 400);
            body.Text = BuildText(row);

            Button close = new Button();
            close.Text = "Close";
            close.Location = new Point(548, 422);
            close.Size = new Size(80, 28);
            close.DialogResult = DialogResult.OK;
            AcceptButton = close;

            Controls.Add(body);
            Controls.Add(close);
        }

        private static string BuildText(DataRow row)
        {
            StringBuilder text = new StringBuilder();
            DateTime when = Convert.ToDateTime(row["TranscationDate"]);
            text.AppendLine("Product: " + row["ItemCode"] + "  " + row["ProductName"]);
            text.AppendLine("Date/Time: " + when.ToString("dd-MM-yyyy HH:mm:ss"));
            text.AppendLine("Movement: " + row["MovementType"]);
            text.AppendLine("Quantity: " + Convert.ToDecimal(row["Quantity"]).ToString("0.000"));
            text.AppendLine("Location: " + row["LocationName"]);
            text.AppendLine("Rack: " + row["RackCaption"]);
            text.AppendLine("Entity: " + MaterialTransactionReport.EntityLabel(Convert.ToString(row["TranscationType"])));
            text.AppendLine("Reference: " + row["TransId"]);
            text.AppendLine("Created By: " + row["UserName"]);
            text.AppendLine();
            text.Append(LoadSource(Convert.ToString(row["TranscationType"]), Convert.ToString(row["TransId"])));
            return text.ToString();
        }

        private static string LoadSource(string entity, string transId)
        {
            if (transId.Trim().Length == 0)
                return "No source reference is stored on this row.";

            try
            {
                if (string.Equals(entity, "Purchase", StringComparison.OrdinalIgnoreCase))
                    return FirstBlock(@"SELECT 'Receipt: ' + CONVERT(VARCHAR(50), h.PurchaseId) + CHAR(13) + CHAR(10)
+ 'Supplier: ' + ISNULL(s.Name, '') + CHAR(13) + CHAR(10)
+ 'Invoice: ' + ISNULL(h.InvoiceNo, '') + CHAR(13) + CHAR(10)
+ 'Order Date: ' + CONVERT(VARCHAR(20), h.OrderDate, 105)
FROM dbo.PurchaseReceiptHeader h
LEFT JOIN dbo.Suppliers s ON s.SuppliersID = h.VendorId
WHERE h.OrderNumber = @Id", transId);

                if (string.Equals(entity, "PDI", StringComparison.OrdinalIgnoreCase))
                    return FirstBlock(@"SELECT 'Estimation: ' + ISNULL(Estimationid, '') + CHAR(13) + CHAR(10)
+ 'Quotation: ' + ISNULL(CONVERT(VARCHAR(50), Quotationid), '') + CHAR(13) + CHAR(10)
+ 'Customer: ' + ISNULL(customername, '')
FROM dbo.QuotationEstimation
WHERE Estimationid = @Id OR CONVERT(VARCHAR(50), Quotationid) = @Id", transId);

                if (string.Equals(entity, "PDI-REVERSE", StringComparison.OrdinalIgnoreCase))
                {
                    string current = FirstBlock(@"SELECT 'Estimation: ' + ISNULL(Estimationid, '') + CHAR(13) + CHAR(10)
+ 'Quotation: ' + ISNULL(CONVERT(VARCHAR(50), Quotationid), '') + CHAR(13) + CHAR(10)
+ 'Customer: ' + ISNULL(customername, '')
FROM dbo.QuotationEstimation
WHERE Estimationid = @Id OR CONVERT(VARCHAR(50), Quotationid) = @Id", transId);
                    string original = FirstBlock(@"SELECT 'Original PDI reference: ' + TransId + CHAR(13) + CHAR(10)
+ 'Original PDI date: ' + CONVERT(VARCHAR(20), MIN(TranscationDate), 105)
FROM dbo.MaterialTranscation
WHERE TranscationType = 'PDI' AND TransId = @Id
GROUP BY TransId", transId);
                    return current + Environment.NewLine + original;
                }

                if (string.Equals(entity, "Product Movement", StringComparison.OrdinalIgnoreCase))
                    return FirstBlock(@"SELECT 'From location/rack: ' + ISNULL(fl.LocationName, CONVERT(VARCHAR(50), d.FromLocation))
+ '  ' + ISNULL(fr.RackCaption, '') + CHAR(13) + CHAR(10)
+ 'To location/rack: ' + ISNULL(tl.LocationName, CONVERT(VARCHAR(50), d.ToLocation))
+ '  ' + ISNULL(tr.RackCaption, '') + CHAR(13) + CHAR(10)
+ 'Moved on: ' + CONVERT(VARCHAR(20), d.MovedOn, 105)
FROM dbo.MaterialTranscationDetails d
LEFT JOIN dbo.LocationMaster fl ON CONVERT(VARCHAR(100), fl.LocationId) = CONVERT(VARCHAR(100), d.FromLocation)
LEFT JOIN dbo.LocationMaster tl ON CONVERT(VARCHAR(100), tl.LocationId) = CONVERT(VARCHAR(100), d.ToLocation)
LEFT JOIN dbo.MaterialTranscation mout ON mout.TransId = CONVERT(VARCHAR(25), d.TransID) AND mout.TranscationType = 'Product Movement' AND mout.Type = 'OUT'
LEFT JOIN dbo.RackMaster fr ON fr.RackId = mout.RackId
LEFT JOIN dbo.MaterialTranscation minn ON minn.TransId = CONVERT(VARCHAR(25), d.TransID) AND minn.TranscationType = 'Product Movement' AND minn.Type = 'IN'
LEFT JOIN dbo.RackMaster tr ON tr.RackId = minn.RackId
WHERE CONVERT(VARCHAR(25), d.TransID) = @Id", transId);

                if (string.Equals(entity, "IssuedReceived", StringComparison.OrdinalIgnoreCase))
                    return FirstBlock(@"SELECT 'Received Id: ' + CONVERT(VARCHAR(20), ReceivedID) + CHAR(13) + CHAR(10)
+ 'Customer: ' + ISNULL(CustomerName, '') + CHAR(13) + CHAR(10)
+ 'Reference: ' + ISNULL(RefNo, '') + CHAR(13) + CHAR(10)
+ 'Status: ' + ISNULL(Status, '')
FROM dbo.Issuedreceived
WHERE CONVERT(VARCHAR(25), ReceivedID) = @Id", transId);

                if (string.Equals(entity, "RECEIPT NOTE", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(entity, "DELIVERY NOTE", StringComparison.OrdinalIgnoreCase))
                    return FirstBlock(@"SELECT 'Note: ' + DeliveryNoteNo + CHAR(13) + CHAR(10)
+ 'Date: ' + CONVERT(VARCHAR(20), DeliveryNoteDate, 105) + CHAR(13) + CHAR(10)
+ 'Reference: ' + ISNULL(ReferenceNo, '') + CHAR(13) + CHAR(10)
+ 'Status: ' + ISNULL(Status, '')
FROM dbo.DeliveryNote
WHERE DeliveryNoteNo = @Id", transId);

                if (string.Equals(entity, "Warranty Replacement", StringComparison.OrdinalIgnoreCase))
                    return "Warranty replacement stock reference: " + transId;
            }
            catch (Exception ex)
            {
                return "Source lookup failed: " + ex.Message;
            }

            return "No extra source record was found for this reference.";
        }

        private static string FirstBlock(string sql, string id)
        {
            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.Add("@Id", SqlDbType.VarChar, 50).Value = id;
                con.Open();
                object value = cmd.ExecuteScalar();
                if (value == null || value == DBNull.Value)
                    return "Source record was not found for reference " + id + ".";
                return Convert.ToString(value);
            }
        }
    }
}
