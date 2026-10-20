using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Inventory.Report
{
    public partial class RackWiseProductStockReport : Form
    {
        private RackStockSnapshot snapshot;
        private bool busy;
        private Font summaryFont;

        public RackWiseProductStockReport()
        {
            InitializeComponent();
            summaryFont = new Font(Font, FontStyle.Bold);
            components.Add(new FontResource(summaryFont));
            grid.ColumnHeadersDefaultCellStyle.Font = summaryFont;
            SetBusy(false, "Ready");
        }

        private sealed class FontResource : Component
        {
            private readonly Font font;
            public FontResource(Font value) { font = value; }
            protected override void Dispose(bool disposing) { if (disposing) font.Dispose(); base.Dispose(disposing); }
        }

        private void Report_Shown(object sender, EventArgs e) { LoadReport(true); }
        private void Search_Click(object sender, EventArgs e) { LoadReport(false); }
        private void Reset_Click(object sender, EventArgs e)
        {
            productSearch.Clear();
            rackSearch.ClearSelection();
            showZero.Checked = false;
            LoadReport(false);
        }

        private void SetBusy(bool value, string message)
        {
            busy = value;
            filters.Enabled = !value;
            toolbar.Enabled = !value && snapshot != null && snapshot.RecordCount > 0;
            UseWaitCursor = value;
            status.Text = message;
        }

        private void Work(string message, Func<object> action, Action<object> complete)
        {
            if (busy) return;
            SetBusy(true, message);
            var worker = new BackgroundWorker();
            worker.DoWork += delegate(object s, DoWorkEventArgs args) { args.Result = action(); };
            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                worker.Dispose();
                if (IsDisposed || Disposing) return;
                SetBusy(false, snapshot == null ? "No report loaded" : snapshot.CountSummary);
                if (args.Error != null)
                {
                    MessageBox.Show(this, args.Error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                complete(args.Result);
            };
            worker.RunWorkerAsync();
        }

        private void LoadReport(bool initialize)
        {
            if (busy) return;
            initialize = initialize || !rackSearch.Loaded;
            DataTable rackIds = rackSearch.SelectedIds();
            string search = productSearch.Text.Trim();
            bool zero = showZero.Checked;
            string rackCaption = rackSearch.FilterCaption;
            Work("Loading rack stock...", delegate
            {
                DataTable racks = null;
                DataTable data = new DataTable();
                using (var connection = new SqlConnection(Program.connection))
                {
                    connection.Open();
                    if (initialize)
                    {
                        racks = new DataTable();
                        using (var command = new SqlCommand("SELECT rm.RackId, lm.LocationName + N' / ' + rm.RackCaption AS Caption FROM dbo.RackMaster rm INNER JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId ORDER BY lm.LocationName, rm.RackCaption, rm.RackId", connection))
                        using (var adapter = new SqlDataAdapter(command)) adapter.Fill(racks);
                    }
                    using (var command = new SqlCommand("dbo.sp_RackWiseProductStockReport", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 180;
                        command.Parameters.Add("@ProductSearch", SqlDbType.NVarChar, 200).Value = search.Length == 0 ? (object)DBNull.Value : search;
                        SqlParameter racksParameter = command.Parameters.Add("@RackIds", SqlDbType.Structured);
                        racksParameter.TypeName = "dbo.RackStockReportRackIds";
                        racksParameter.Value = rackIds;
                        command.Parameters.Add("@ShowZeroStock", SqlDbType.Bit).Value = zero;
                        using (var adapter = new SqlDataAdapter(command)) adapter.Fill(data);
                    }
                }
                return new object[] { racks, RackStockSnapshot.Create(data, "Product: " + (search.Length == 0 ? "All" : search) + "; Rack: " + rackCaption + "; Show Zero Stock: " + (zero ? "Yes" : "No")) };
            }, delegate(object result)
            {
                var values = (object[])result;
                if (initialize)
                {
                    rackSearch.SetRacks((DataTable)values[0]);
                }
                snapshot = (RackStockSnapshot)values[1];
                grid.SuspendLayout();
                try { grid.DataSource = snapshot.Rows; grid.ClearSelection(); }
                finally { grid.ResumeLayout(); }
                SetBusy(false, snapshot.CountSummary);
            });
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (snapshot == null || e.RowIndex < 0 || e.RowIndex >= snapshot.Rows.Rows.Count) return;
            string kind = Convert.ToString(snapshot.Rows.Rows[e.RowIndex]["Kind"]);
            if (kind == "Detail") return;
            e.CellStyle.Font = summaryFont;
            e.CellStyle.BackColor = kind == "Rack" ? Color.FromArgb(235, 238, 242) : Color.FromArgb(245, 247, 249);
            e.CellStyle.ForeColor = Color.Black;
        }

        private void Excel_Click(object sender, EventArgs e) { Export(false); }
        private void Pdf_Click(object sender, EventArgs e) { Export(true); }
        private void Export(bool pdf)
        {
            if (snapshot == null || busy) return;
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = pdf ? "PDF (*.pdf)|*.pdf" : "Excel Workbook (*.xlsx)|*.xlsx";
                dialog.FileName = "RackWiseProductStock_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + (pdf ? ".pdf" : ".xlsx");
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                string path = dialog.FileName;
                RackStockSnapshot report = snapshot;
                Work("Exporting complete filtered report...", delegate
                {
                    if (pdf) RackStockOutput.SavePdf(report, path);
                    else RackStockOutput.SaveExcel(report, path);
                    return path;
                }, delegate(object result) { MessageBox.Show(this, "Report saved to:\n" + result, Text, MessageBoxButtons.OK, MessageBoxIcon.Information); });
            }
        }

        private void Print_Click(object sender, EventArgs e)
        {
            if (snapshot == null || busy) return;
            try
            {
                using (var document = new PrintDocument())
                using (var selection = new PrintDialog())
                using (var preview = new PrintPreviewDialog())
                {
                    document.DocumentName = Text;
                    document.DefaultPageSettings.Landscape = true;
                    foreach (PaperSize paper in document.PrinterSettings.PaperSizes)
                        if (paper.Kind == PaperKind.A4) { document.DefaultPageSettings.PaperSize = paper; break; }
                    document.DefaultPageSettings.Margins = new Margins(35, 35, 35, 35);
                    selection.Document = document;
                    selection.UseEXDialog = true;
                    if (selection.ShowDialog(this) != DialogResult.OK) return;
                    document.DefaultPageSettings.Landscape = true;
                    foreach (PaperSize paper in document.PrinterSettings.PaperSizes)
                        if (paper.Kind == PaperKind.A4) { document.DefaultPageSettings.PaperSize = paper; break; }
                    List<RackStockOutput.Page> pages = RackStockOutput.Paginate(snapshot);
                    int page = 0;
                    document.BeginPrint += delegate { page = 0; };
                    document.PrintPage += delegate(object s, PrintPageEventArgs args)
                    {
                        RackStockOutput.DrawPrintPage(snapshot, pages[page], args.Graphics, args.MarginBounds, page + 1, pages.Count);
                        page++;
                        args.HasMorePages = page < pages.Count;
                    };
                    preview.Document = document;
                    preview.Width = 1100; preview.Height = 720;
                    preview.ShowDialog(this);
                }
            }
            catch (Exception ex) { MessageBox.Show(this, "Could not print report.\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }

    internal sealed class RackStockSnapshot
    {
        public const string Title = "RRE Electricals - Rack Wise Product Stock Report";
        public DataTable Rows;
        public string Filters;
        public DateTime Generated;
        public int RecordCount;
        public int RackCount;
        public int ProductCount;
        public string CountSummary { get { return RecordCount + " records | " + RackCount + " racks | " + ProductCount + " distinct products | Totals by UOM below"; } }

        private static void AddTotals(DataTable rows, SortedDictionary<string, decimal> totals, string caption)
        {
            foreach (var total in totals) rows.Rows.Add("", "", DBNull.Value, "", caption, total.Key, total.Value, "Total");
        }
        private static void Sum(SortedDictionary<string, decimal> totals, string uom, decimal value)
        {
            if (!totals.ContainsKey(uom)) totals.Add(uom, 0);
            totals[uom] += value;
        }
        public static RackStockSnapshot Create(DataTable source, string filters)
        {
            var report = new RackStockSnapshot();
            report.Filters = filters; report.Generated = DateTime.Now; report.RecordCount = source.Rows.Count;
            var table = new DataTable();
            foreach (string name in new string[] { "Location", "Rack", "ProductId", "ItemCode", "ProductName", "UOM", "Quantity", "Kind" })
                table.Columns.Add(name, name == "Quantity" ? typeof(decimal) : name == "ProductId" ? typeof(int) : typeof(string));
            var grand = new SortedDictionary<string, decimal>();
            var rack = new SortedDictionary<string, decimal>();
            var products = new HashSet<int>();
            var racks = new HashSet<int>();
            int? previous = null;
            table.BeginLoadData();
            foreach (DataRow row in source.Rows)
            {
                int rackId = Convert.ToInt32(row["RackId"]);
                int productId = Convert.ToInt32(row["ProductId"]);
                string location = Convert.ToString(row["LocationName"]);
                string rackName = Convert.ToString(row["RackName"]);
                if (previous != rackId)
                {
                    if (previous.HasValue) AddTotals(table, rack, "Rack Total");
                    rack.Clear();
                    table.Rows.Add(location, rackName, DBNull.Value, "", "Rack: " + rackName, "", DBNull.Value, "Rack");
                    previous = rackId;
                }
                string uom = Convert.ToString(row["UOM"]).Trim().ToUpperInvariant();
                if (uom.Length == 0) uom = "Unknown";
                decimal quantity = Convert.ToDecimal(row["CurrentStockQuantity"]);
                table.Rows.Add(location, rackName, productId, row["ItemCode"], row["ProductName"], uom, quantity, "Detail");
                string totalUom = uom == "Unknown" ? "Unknown (Product " + productId + ")" : uom;
                Sum(rack, totalUom, quantity); Sum(grand, totalUom, quantity);
                products.Add(productId); racks.Add(rackId);
            }
            if (previous.HasValue) AddTotals(table, rack, "Rack Total");
            AddTotals(table, grand, "Grand Total");
            if (source.Rows.Count == 0) table.Rows.Add("", "", DBNull.Value, "", "No matching stock records", "", DBNull.Value, "Total");
            table.EndLoadData();
            report.Rows = table; report.RackCount = racks.Count; report.ProductCount = products.Count;
            return report;
        }
    }

    // Selection is keyed by RackId, independent of the currently visible search results.
    internal sealed class RackMultiSelect : UserControl
    {
        private sealed class Rack
        {
            public int Id;
            public string Caption;
            public override string ToString() { return Caption; }
        }
        private readonly List<Rack> racks = new List<Rack>();
        private readonly HashSet<int> selected = new HashSet<int>();
        private readonly Button display = new Button();
        private readonly TextBox search = new TextBox();
        private readonly CheckedListBox list = new CheckedListBox();
        private readonly ToolStripDropDown popup = new ToolStripDropDown();
        private readonly ToolTip tip = new ToolTip();
        private readonly Label count = new Label();
        private bool rebuilding;
        public bool Loaded { get; private set; }

        public RackMultiSelect()
        {
            Height = 25;
            display.Dock = DockStyle.Fill;
            display.TextAlign = ContentAlignment.MiddleLeft;
            display.AutoEllipsis = true;
            display.Click += delegate { if (popup.Visible) popup.Close(); else OpenPopup(); };
            Controls.Add(display);
            var layout = new TableLayoutPanel();
            layout.ColumnCount = 1; layout.RowCount = 5;
            layout.Padding = new Padding(6); layout.BackColor = SystemColors.Window;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            search.Dock = DockStyle.Fill;
            search.TextChanged += delegate { Rebuild(); };
            tip.SetToolTip(search, "Type any part of the rack name/code or location (case-insensitive).");
            layout.Controls.Add(search, 0, 0);
            var actions = new FlowLayoutPanel(); actions.Dock = DockStyle.Fill;
            var all = new Button(); all.Text = "Select All"; all.AutoSize = true;
            all.Click += delegate { foreach (Rack rack in racks) selected.Add(rack.Id); Rebuild(); };
            var clear = new Button(); clear.Text = "Clear All"; clear.AutoSize = true;
            clear.Click += delegate { selected.Clear(); Rebuild(); };
            tip.SetToolTip(all, "Select every rack, including racks hidden by the search.");
            tip.SetToolTip(clear, "Remove every selection, including hidden racks. No selection means All Racks.");
            actions.Controls.Add(all); actions.Controls.Add(clear); layout.Controls.Add(actions, 0, 1);
            list.Dock = DockStyle.Fill; list.CheckOnClick = true;
            list.IntegralHeight = false; list.HorizontalScrollbar = true;
            list.ItemCheck += delegate(object sender, ItemCheckEventArgs args)
            {
                if (rebuilding) return;
                Rack rack = (Rack)list.Items[args.Index];
                if (args.NewValue == CheckState.Checked) selected.Add(rack.Id); else selected.Remove(rack.Id);
                UpdateSummary();
            };
            layout.Controls.Add(list, 0, 2);
            count.Dock = DockStyle.Fill;
            layout.Controls.Add(count, 0, 3);
            var done = new Button(); done.Text = "Done"; done.Dock = DockStyle.Right;
            done.Click += delegate { popup.Close(); display.Focus(); };
            layout.Controls.Add(done, 0, 4);
            var host = new ToolStripControlHost(layout);
            host.AutoSize = false; host.Margin = Padding.Empty; host.Padding = Padding.Empty;
            popup.AutoSize = false; popup.Padding = new Padding(1); popup.Items.Add(host);
            popup.Opened += delegate { search.Focus(); };
            popup.Closed += delegate(object sender, ToolStripDropDownClosedEventArgs args)
            { if (args.CloseReason == ToolStripDropDownCloseReason.Keyboard) display.Focus(); };
            search.KeyDown += delegate(object sender, KeyEventArgs args)
            {
                if (args.KeyCode == Keys.Down && list.Items.Count > 0) { list.Focus(); list.SelectedIndex = 0; args.Handled = true; }
                if (args.KeyCode == Keys.Enter) { popup.Close(); args.SuppressKeyPress = true; }
            };
            EnabledChanged += delegate { if (!Enabled) popup.Close(); };
            UpdateSummary();
        }

        public void SetRacks(DataTable table)
        {
            racks.Clear();
            var ids = new HashSet<int>();
            foreach (DataRow row in table.Rows)
            {
                int id = Convert.ToInt32(row["RackId"]);
                if (ids.Add(id)) racks.Add(new Rack { Id = id, Caption = Convert.ToString(row["Caption"]) });
            }
            selected.IntersectWith(ids);
            Loaded = true;
            Rebuild();
        }
        public DataTable SelectedIds()
        {
            var table = new DataTable(); table.Columns.Add("RackId", typeof(int));
            foreach (Rack rack in racks) if (selected.Contains(rack.Id)) table.Rows.Add(rack.Id);
            return table;
        }
        public string FilterCaption
        {
            get
            {
                var captions = new List<string>();
                foreach (Rack rack in racks) if (selected.Contains(rack.Id)) captions.Add(rack.Caption);
                return captions.Count == 0 ? "All Racks" : string.Join(", ", captions.ToArray());
            }
        }
        public void ClearSelection()
        {
            selected.Clear(); search.Clear(); Rebuild(); popup.Close();
        }
        private void Rebuild()
        {
            rebuilding = true; list.BeginUpdate();
            try
            {
                list.Items.Clear();
                string query = search.Text.Trim();
                foreach (Rack rack in racks)
                    if (query.Length == 0 || rack.Caption.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                        list.Items.Add(rack, selected.Contains(rack.Id));
            }
            finally { list.EndUpdate(); rebuilding = false; }
            UpdateSummary();
        }
        private void UpdateSummary()
        {
            display.Text = (selected.Count == 0 ? "All Racks" : selected.Count == 1 ? FilterCaption : selected.Count + " Racks Selected") + "  \u25BE";
            tip.SetToolTip(display, FilterCaption);
            count.Text = list.Items.Count + " matching | " + (selected.Count == 0 ? "All Racks (no restriction)" : selected.Count + " selected") + "\nSelect All / Clear All affect every rack, including hidden racks.";
        }
        private void OpenPopup()
        {
            Rectangle area = Screen.FromControl(this).WorkingArea;
            Point below = PointToScreen(new Point(0, Height));
            Point above = PointToScreen(Point.Empty);
            int width = Math.Min(Math.Max(Width, 420), area.Width - 8);
            int available = Math.Max(area.Bottom - below.Y, above.Y - area.Top) - 4;
            int height = Math.Min(380, Math.Max(180, available));
            height = Math.Min(height, area.Height - 8);
            int y = area.Bottom - below.Y >= height ? below.Y : above.Y - height;
            int x = Math.Max(area.Left + 4, Math.Min(below.X, area.Right - width - 4));
            y = Math.Max(area.Top + 4, Math.Min(y, area.Bottom - height - 4));
            popup.Size = new Size(width, height);
            popup.Items[0].Size = new Size(width - 2, height - 2);
            popup.Show(new Point(x, y));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) { popup.Dispose(); tip.Dispose(); }
            base.Dispose(disposing);
        }
    }
}
