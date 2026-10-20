// Standalone regression harness; not compiled into Inventory.exe.
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Inventory.Report;

namespace Inventory { internal static class Program { public static string connection = ""; } }
internal static class RackStockHarness
{
    private static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static DataTable Source()
    {
        var table = new DataTable();
        foreach (string name in new string[] { "RackId", "RackName", "LocationName", "ProductId", "ProductName", "ItemCode", "UOM", "CurrentStockQuantity" })
            table.Columns.Add(name, name == "RackId" || name == "ProductId" ? typeof(int) : name == "CurrentStockQuantity" ? typeof(decimal) : typeof(string));
        return table;
    }
    private static object Field(object instance, string name) { return instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance); }
    private static void TestRackSelection(string directory)
    {
        var racks = new DataTable(); racks.Columns.Add("RackId", typeof(int)); racks.Columns.Add("Caption", typeof(string));
        racks.Rows.Add(1, "Shop / RS-1"); racks.Rows.Add(2, "Shop / RS-5"); racks.Rows.Add(3, "Store / RG-14"); racks.Rows.Add(4, "Shop / RS-10");
        racks.Rows.Add(1, "Shop / RS-1");
        using (var form = new Form())
        using (var selector = new RackMultiSelect())
        {
            form.ClientSize = new Size(700, 400); form.ShowInTaskbar = false; form.Opacity = 0;
            selector.Location = new Point(420, 350); selector.Width = 230; form.Controls.Add(selector); form.Show();
            selector.SetRacks(racks);
            Check(selector.SelectedIds().Rows.Count == 0 && selector.FilterCaption == "All Racks", "Default all racks");
            var search = (TextBox)Field(selector, "search"); var list = (CheckedListBox)Field(selector, "list");
            search.Text = "rS"; Check(list.Items.Count == 3, "Case-insensitive partial rack search");
            list.SetItemChecked(0, true); list.SetItemChecked(1, true);
            search.Text = "g-1"; Check(list.Items.Count == 1 && !list.GetItemChecked(0), "Match anywhere in rack caption");
            Check(selector.SelectedIds().Rows.Count == 2, "Search must preserve hidden checks");
            list.SetItemChecked(0, true);
            Check(selector.SelectedIds().Rows.Count == 3 && selector.FilterCaption.IndexOf("RS-1") >= 0 && selector.FilterCaption.IndexOf("RG-14") >= 0, "Selected captions / IDs");
            search.Text = "RS-1"; Check(list.Items.Count == 2 && list.GetItemChecked(0), "Restore selected checks");
            list.SetItemChecked(0, false); Check(selector.SelectedIds().Rows.Count == 2, "Remove individual selection");
            var popup = (ToolStripDropDown)Field(selector, "popup");
            var layout = (TableLayoutPanel)((ToolStripControlHost)popup.Items[0]).Control;
            var actions = (FlowLayoutPanel)layout.GetControlFromPosition(0, 1);
            MethodInfo click = typeof(Button).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
            click.Invoke(actions.Controls[0], new object[] { EventArgs.Empty });
            Check(selector.SelectedIds().Rows.Count == 4, "Select All includes hidden racks and deduplicates IDs");
            click.Invoke(actions.Controls[1], new object[] { EventArgs.Empty });
            Check(selector.SelectedIds().Rows.Count == 0 && selector.FilterCaption == "All Racks", "Clear All includes hidden racks");
            search.Text = "no-such-rack"; Check(list.Items.Count == 0, "Empty rack search");
            search.Clear(); list.SetItemChecked(0, true); selector.ClearSelection();
            Check(search.Text == "" && list.CheckedItems.Count == 0, "Reset selection and popup search");
            // Check popup positioning near a screen edge without showing a test window.
            popup.Opacity = 0;
            selector.GetType().GetMethod("OpenPopup", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(selector, null);
            Check(Screen.FromControl(selector).WorkingArea.Contains(popup.Bounds), "Popup clipped by screen edge");
            Check(layout.Height > 200 && list.Height > 100, "Popup list height");
            using (var bitmap = new Bitmap(layout.Width, layout.Height))
            { layout.DrawToBitmap(bitmap, new Rectangle(Point.Empty, layout.Size)); bitmap.Save(Path.Combine(directory, "rack-selector.png")); }
            popup.Close();
        }
        Console.WriteLine("PASS: partial/case-insensitive rack search, retained checks, individual removal, global Select/Clear All, Reset, unique IDs and screen-safe popup.");
    }
    [STAThread]
    public static int Main(string[] args)
    {
        try
        {
            string directory = args[0]; Directory.CreateDirectory(directory);
            TestRackSelection(directory);
            var source = Source();
            source.Rows.Add(1, "RS-1", "Shop", 1001, "LED Bulb 9W", "LED-9", "NOS", 25.125m);
            source.Rows.Add(1, "RS-1", "Shop", 1002, "PVC Pipe 20MM", "PVC-20", "MTR", 120.250m);
            source.Rows.Add(1, "RS-1", "Shop", 1003, "Zero stock switch", "SW-6", "NOS", 0m);
            source.Rows.Add(2, "RG-14", "Store", 1001, "LED Bulb 9W", "LED-9", "NOS", 40.375m);
            source.Rows.Add(2, "RG-14", "Store", 1010, "Copper Wire " + new string('W', 210), "WIRE", "MTR", 250.125m);
            var report = RackStockSnapshot.Create(source, "Product: All; Rack: All Racks; Show Zero Stock: Yes");
            Check(report.RecordCount == 5 && report.RackCount == 2 && report.ProductCount == 4, "Counts / product in multiple racks");
            var totals = new Dictionary<string, decimal>();
            int details = 0;
            foreach (DataRow row in report.Rows.Rows)
            {
                if (Convert.ToString(row["Kind"]) == "Detail") details++;
                if (Convert.ToString(row["ProductName"]) == "Grand Total") totals.Add(Convert.ToString(row["UOM"]), (decimal)row["Quantity"]);
            }
            Check(details == 5 && totals["NOS"] == 65.500m && totals["MTR"] == 370.375m, "Fractional totals must be separate by UOM");
            RackStockOutput.SaveExcel(report, Path.Combine(directory, "sample.xlsx"));
            RackStockOutput.SavePdf(report, Path.Combine(directory, "sample.pdf"));
            var captions = new System.Text.StringBuilder("Product: All; Selected racks: ");
            for (int i = 0; i < 1500; i++) captions.Append("Location / RS-").Append(i).Append(", ");
            captions.Append("; Show Zero Stock: Yes");
            var longFilters = RackStockSnapshot.Create(source, captions.ToString());
            RackStockOutput.SaveExcel(longFilters, Path.Combine(directory, "many-racks.xlsx"));
            RackStockOutput.SavePdf(longFilters, Path.Combine(directory, "many-racks.pdf"));
            var longPages = RackStockOutput.Paginate(longFilters);
            Check(longPages.Count > 2 && longPages[0].FiltersOnly, "Long selected-rack header pagination");
            var filterText = new System.Text.StringBuilder();
            foreach (var page in longPages) if (page.FiltersOnly) foreach (string line in page.FilterLines) filterText.Append(line);
            Check(filterText.ToString().IndexOf("RS-1499") >= 0, "Last selected rack caption must be retained");
            var portrait = new Inventory.Tool.SimplePdf();
            portrait.NewPage(); portrait.Text(30, 30, "Existing portrait report compatibility", 12, false, Color.Black);
            portrait.Save(Path.Combine(directory, "portrait-default.pdf"));
            var missingUom = Source();
            missingUom.Rows.Add(1, "Rack", "Shop", 1, "Unknown unit A", "", "", 2m);
            missingUom.Rows.Add(1, "Rack", "Shop", 2, "Unknown unit B", "", "", 3m);
            var unknown = RackStockSnapshot.Create(missingUom, "All");
            int unknownGrandTotals = 0;
            foreach (DataRow row in unknown.Rows.Rows) if (Convert.ToString(row["ProductName"]) == "Grand Total") unknownGrandTotals++;
            Check(unknownGrandTotals == 2, "Unknown UOMs must not combine unrelated products");
            var empty = RackStockSnapshot.Create(Source(), "No results");
            Check(empty.RecordCount == 0 && empty.Rows.Rows.Count == 1, "Empty report");
            Check(RackStockOutput.Paginate(empty).Count == 1, "Empty pagination");
            var large = Source();
            for (int i = 0; i < 30005; i++) large.Rows.Add(i / 1000 + 1, "Rack " + (i / 1000 + 1), "Shop", i, "Long product " + i + " " + new string('X', i % 3 == 0 ? 170 : 10), "CODE" + i, i % 2 == 0 ? "NOS" : "MTR", .125m);
            DateTime start = DateTime.Now;
            var many = RackStockSnapshot.Create(large, "Product: All; Rack: All Racks; Show Zero Stock: No");
            Check(many.RecordCount == 30005 && many.RackCount == 31, "Large report counts");
            Console.WriteLine("30,005-record snapshot: " + (DateTime.Now - start).TotalMilliseconds + " ms");
            var pages = RackStockOutput.Paginate(many);
            Check(pages.Count > 100, "Multiple pages");
            int exportedDetails = 0;
            foreach (var page in pages)
            {
                Check(page.Rows.Count > 0 && page.Rows[page.Rows.Count - 1].Cells[1][0] != "", "Page must not end in a rack heading");
                float height = 94;
                foreach (var row in page.Rows)
                {
                    Check(row.Cells.Length == 4, "PDF / print must omit Item Code and retain the other columns");
                    height += row.Height;
                    if (!row.Bold) exportedDetails++;
                }
                Check(height <= 550, "Page overflow");
            }
            Check(exportedDetails == 30005, "Exports must contain every detail");
            RackStockOutput.SaveExcel(many, Path.Combine(directory, "large.xlsx"));
            // A shorter multi-page PDF for visual QA and page extraction checks.
            while (large.Rows.Count > 120) large.Rows.RemoveAt(large.Rows.Count - 1);
            var multi = RackStockSnapshot.Create(large, report.Filters);
            RackStockOutput.SavePdf(multi, Path.Combine(directory, "multipage.pdf"));
            var paginated = RackStockOutput.Paginate(multi);
            using (var bitmap = new Bitmap(1684, 1190))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                RackStockOutput.DrawPrintPage(multi, paginated[0], graphics, new Rectangle(0, 0, 1169, 827), 1, paginated.Count);
                bitmap.Save(Path.Combine(directory, "print.png"));
            }
            using (var form = new RackWiseProductStockReport())
            {
                // Exercise layout with real control handles without contacting a
                // database or displaying an interactive test window.
                form.Shown -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), form, form.GetType().GetMethod("Report_Shown", BindingFlags.Instance | BindingFlags.NonPublic));
                form.ShowInTaskbar = false; form.Opacity = 0; form.Show();
                DateTime bindingStart = DateTime.Now;
                ((DataGridView)Field(form, "grid")).DataSource = many.Rows;
                Check(((DataGridView)Field(form, "grid")).Rows.Count == many.Rows.Rows.Count, "Full DataTable binding");
                Console.WriteLine("30,005-record grid binding: " + (DateTime.Now - bindingStart).TotalMilliseconds + " ms");
                form.GetType().GetField("snapshot", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(form, report);
                ((DataGridView)Field(form, "grid")).DataSource = report.Rows;
                foreach (Size size in new Size[] { new Size(1366, 680), new Size(800, 500), new Size(640, 420) })
                {
                    form.ClientSize = size; form.CreateControl(); form.PerformLayout();
                    var filters = (FlowLayoutPanel)Field(form, "filters");
                    foreach (Control control in filters.Controls)
                        Check(control.Bottom <= filters.ClientSize.Height && control.Right <= filters.ClientSize.Width, "Filter clipped at " + size + ": " + control.Text);
                    var grid = (DataGridView)Field(form, "grid");
                    Check(grid.Columns.Count == 6 && !grid.Columns.Contains("ItemCode"), "Grid must omit Item Code");
                    Check(grid.Columns.Contains("ProductId") && grid.Columns.Contains("ProductName") && grid.Columns.Contains("Location") && grid.Columns.Contains("Rack") && grid.Columns.Contains("UOM") && grid.Columns.Contains("Quantity"), "Grid must retain every other column");
                    Check(grid.Columns["Quantity"].DefaultCellStyle.Format == "0.###", "Stock quantity formatting");
                    Check(grid.Height > 100, "Grid too short at " + size);
                    using (var bitmap = new Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
                        bitmap.Save(Path.Combine(directory, "layout-" + size.Width + ".png"));
                    }
                }
            }
            Console.WriteLine("PASS: UOM totals, fractional quantities, multiple racks, empty results, 30,005 rows, complete pagination, XLSX/PDF/print generation and responsive layouts.");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
