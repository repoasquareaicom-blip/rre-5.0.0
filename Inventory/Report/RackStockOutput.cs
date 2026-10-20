using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Xml;
using Inventory.Tool;

namespace Inventory.Report
{
    // Office-free Open XML output using the framework Packaging API. The existing
    // template exporter uses ACE/Jet; those providers are not required here.
    internal static class RackStockOutput
    {
        private const string Spreadsheet = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        private static readonly string[] Fields = { "Location", "Rack", "ProductId", "ProductName", "UOM", "Quantity" };
        private static readonly string[] Headings = { "Location", "Rack", "Product ID", "Product Name / Summary", "UOM", "Stock Qty" };
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        private static XmlWriter Part(Package package, string name, string contentType)
        {
            var part = package.CreatePart(new Uri(name, UriKind.Relative), contentType, CompressionOption.Normal);
            return XmlWriter.Create(part.GetStream(), new XmlWriterSettings { Encoding = new UTF8Encoding(false), CloseOutput = true });
        }
        private static void Cell(XmlWriter xml, int column, int row, object value, int style, bool numeric)
        {
            xml.WriteStartElement("c", Spreadsheet);
            xml.WriteAttributeString("r", ((char)('A' + column)).ToString() + row.ToString(Invariant));
            xml.WriteAttributeString("s", style.ToString(Invariant));
            if (numeric && value != DBNull.Value)
                xml.WriteElementString("v", Spreadsheet, Convert.ToString(value, Invariant));
            else
            {
                xml.WriteAttributeString("t", "inlineStr");
                xml.WriteStartElement("is", Spreadsheet);
                xml.WriteStartElement("t", Spreadsheet);
                xml.WriteAttributeString("xml", "space", null, "preserve");
                xml.WriteString(Convert.ToString(value));
                xml.WriteEndElement(); xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }
        private static void TextRow(XmlWriter xml, int row, string text, int style)
        {
            xml.WriteStartElement("row", Spreadsheet); xml.WriteAttributeString("r", row.ToString(Invariant));
            xml.WriteAttributeString("ht", row == 3 ? "42" : "24"); xml.WriteAttributeString("customHeight", "1");
            Cell(xml, 0, row, text, style, false); xml.WriteEndElement();
        }

        public static void SaveExcel(RackStockSnapshot report, string path)
        {
            string[] filterLines = Wrap(report.Filters, 782, false);
            int summaryRow = filterLines.Length + 3;
            int headingRow = summaryRow + 2;
            if (report.Rows.Rows.Count + headingRow > 1048576) throw new InvalidOperationException("This report exceeds the Excel worksheet row limit. Select a rack or product filter.");
            using (Package package = Package.Open(path, FileMode.Create, FileAccess.ReadWrite))
            {
                package.CreateRelationship(new Uri("/xl/workbook.xml", UriKind.Relative), TargetMode.Internal,
                    "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "rId1");
                using (XmlWriter xml = Part(package, "/xl/workbook.xml", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"))
                {
                    xml.WriteStartElement("workbook", Spreadsheet);
                    xml.WriteStartElement("sheets", Spreadsheet); xml.WriteStartElement("sheet", Spreadsheet);
                    xml.WriteAttributeString("name", "Rack Stock"); xml.WriteAttributeString("sheetId", "1");
                    xml.WriteAttributeString("r", "id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", "rId1");
                    xml.WriteEndElement(); xml.WriteEndElement(); xml.WriteEndElement();
                }
                PackagePart workbook = package.GetPart(new Uri("/xl/workbook.xml", UriKind.Relative));
                workbook.CreateRelationship(new Uri("worksheets/sheet1.xml", UriKind.Relative), TargetMode.Internal,
                    "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet", "rId1");
                workbook.CreateRelationship(new Uri("styles.xml", UriKind.Relative), TargetMode.Internal,
                    "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles", "rId2");
                using (XmlWriter xml = Part(package, "/xl/styles.xml", "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"))
                {
                    xml.WriteRaw("<styleSheet xmlns=\"" + Spreadsheet + "\"><numFmts count=\"1\"><numFmt numFmtId=\"164\" formatCode=\"0.###\"/></numFmts>"
                        + "<fonts count=\"2\"><font><sz val=\"11\"/><name val=\"Calibri\"/></font><font><b/><sz val=\"11\"/><name val=\"Calibri\"/></font></fonts>"
                        + "<fills count=\"3\"><fill><patternFill patternType=\"none\"/></fill><fill><patternFill patternType=\"gray125\"/></fill><fill><patternFill patternType=\"solid\"><fgColor rgb=\"FFDCE6F1\"/><bgColor indexed=\"64\"/></patternFill></fill></fills>"
                        + "<borders count=\"1\"><border><left/><right/><top/><bottom/><diagonal/></border></borders>"
                        + "<cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs>"
                        + "<cellXfs count=\"4\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"><alignment vertical=\"top\" wrapText=\"1\"/></xf>"
                        + "<xf numFmtId=\"0\" fontId=\"1\" fillId=\"2\" borderId=\"0\" xfId=\"0\" applyFill=\"1\" applyFont=\"1\"><alignment wrapText=\"1\"/></xf>"
                        + "<xf numFmtId=\"164\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyNumberFormat=\"1\"><alignment horizontal=\"right\"/></xf>"
                        + "<xf numFmtId=\"164\" fontId=\"1\" fillId=\"2\" borderId=\"0\" xfId=\"0\" applyNumberFormat=\"1\" applyFont=\"1\" applyFill=\"1\"><alignment horizontal=\"right\"/></xf></cellXfs>"
                        + "<cellStyles count=\"1\"><cellStyle name=\"Normal\" xfId=\"0\" builtinId=\"0\"/></cellStyles></styleSheet>");
                }
                using (XmlWriter xml = Part(package, "/xl/worksheets/sheet1.xml", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"))
                {
                    xml.WriteStartElement("worksheet", Spreadsheet);
                    xml.WriteStartElement("sheetViews", Spreadsheet); xml.WriteStartElement("sheetView", Spreadsheet); xml.WriteAttributeString("workbookViewId", "0");
                    xml.WriteStartElement("pane", Spreadsheet); xml.WriteAttributeString("ySplit", headingRow.ToString(Invariant)); xml.WriteAttributeString("topLeftCell", "A" + (headingRow + 1).ToString(Invariant)); xml.WriteAttributeString("state", "frozen"); xml.WriteEndElement();
                    xml.WriteEndElement(); xml.WriteEndElement();
                    xml.WriteStartElement("cols", Spreadsheet);
                    for (int c = 0; c < Fields.Length; c++)
                    {
                        int width = Headings[c].Length + 2;
                        foreach (DataRow row in report.Rows.Rows) width = Math.Max(width, Math.Min(65, Convert.ToString(row[Fields[c]]).Length + 2));
                        xml.WriteStartElement("col", Spreadsheet);
                        xml.WriteAttributeString("min", (c + 1).ToString(Invariant)); xml.WriteAttributeString("max", (c + 1).ToString(Invariant));
                        xml.WriteAttributeString("width", width.ToString(Invariant)); xml.WriteAttributeString("customWidth", "1"); xml.WriteAttributeString("bestFit", "1"); xml.WriteEndElement();
                    }
                    xml.WriteEndElement();
                    xml.WriteStartElement("sheetData", Spreadsheet);
                    TextRow(xml, 1, RackStockSnapshot.Title, 1);
                    TextRow(xml, 2, "Generated: " + report.Generated.ToString("dd-MMM-yyyy HH:mm:ss"), 0);
                    for (int f = 0; f < filterLines.Length; f++) TextRow(xml, f + 3, filterLines[f], 0);
                    TextRow(xml, summaryRow, report.CountSummary, 0);
                    xml.WriteStartElement("row", Spreadsheet); xml.WriteAttributeString("r", headingRow.ToString(Invariant));
                    for (int c = 0; c < Headings.Length; c++) Cell(xml, c, headingRow, Headings[c], 1, false);
                    xml.WriteEndElement();
                    int index = headingRow + 1;
                    foreach (DataRow row in report.Rows.Rows)
                    {
                        bool bold = Convert.ToString(row["Kind"]) != "Detail";
                        xml.WriteStartElement("row", Spreadsheet); xml.WriteAttributeString("r", index.ToString(Invariant));
                        for (int c = 0; c < Fields.Length; c++) Cell(xml, c, index, row[Fields[c]], Fields[c] == "Quantity" ? (bold ? 3 : 2) : (bold ? 1 : 0), Fields[c] == "ProductId" || Fields[c] == "Quantity");
                        xml.WriteEndElement(); index++;
                    }
                    xml.WriteEndElement();
                    xml.WriteStartElement("mergeCells", Spreadsheet); xml.WriteAttributeString("count", summaryRow.ToString(Invariant));
                    for (int i = 1; i <= summaryRow; i++) { xml.WriteStartElement("mergeCell", Spreadsheet); xml.WriteAttributeString("ref", "A" + i + ":F" + i); xml.WriteEndElement(); }
                    xml.WriteEndElement();
                    xml.WriteStartElement("pageSetup", Spreadsheet); xml.WriteAttributeString("paperSize", "9"); xml.WriteAttributeString("orientation", "landscape"); xml.WriteEndElement();
                    xml.WriteEndElement();
                }
            }
        }

        internal sealed class LineRow
        {
            public string[][] Cells;
            public bool Bold;
            public bool RackHeading;
            public float Height;
        }
        internal sealed class Page
        {
            public readonly List<LineRow> Rows = new List<LineRow>();
            public string[] FilterLines;
            public bool FiltersOnly;
        }
        private static readonly float[] Lefts = { 30, 100, 635, 705 };
        private static readonly float[] Widths = { 70, 535, 70, 107 };
        private static readonly string[] PrintFields = { "ProductId", "ProductName", "UOM", "Quantity" };
        private static readonly string[] PrintHeaders = { "Product ID", "Product Name / Summary", "UOM", "Stock Qty" };

        // Character wrapping also handles long product details without spaces.
        private static string[] Wrap(string text, float width, bool bold)
        {
            var metrics = new SimplePdf();
            var lines = new List<string>();
            string remaining = (text ?? "").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            while (remaining.Length > 0)
            {
                int count = 0;
                float used = 0;
                while (count < remaining.Length)
                {
                    float next = metrics.Width(remaining[count].ToString(), 9, bold);
                    if (count > 0 && used + next > width) break;
                    used += next; count++;
                }
                if (count < remaining.Length)
                {
                    int space = remaining.LastIndexOf(' ', Math.Max(0, count - 1), count);
                    if (space > count / 2) count = space;
                }
                lines.Add(remaining.Substring(0, count).TrimEnd());
                remaining = remaining.Substring(count).TrimStart();
            }
            if (lines.Count == 0) lines.Add("");
            return lines.ToArray();
        }
        private static float BodyTop(Page page) { return 80 + page.FilterLines.Length * 12; }
        private static LineRow Heading(string caption)
        {
            var row = new LineRow(); row.Bold = true; row.RackHeading = true; row.Cells = new string[PrintFields.Length][];
            row.Cells[0] = Wrap(caption, 772, true);
            for (int i = 1; i < PrintFields.Length; i++) row.Cells[i] = new string[] { "" };
            row.Height = 8 + row.Cells[0].Length * 12;
            return row;
        }
        public static List<Page> Paginate(RackStockSnapshot report)
        {
            var pages = new List<Page>();
            string[] filterLines = Wrap(report.Filters, 782, false);
            if (filterLines.Length > 12)
            {
                // Large multi-rack selections must retain every selected caption
                // without consuming the data-page header or clipping the totals.
                for (int start = 0; start < filterLines.Length; start += 35)
                {
                    var details = new Page(); details.FiltersOnly = true;
                    details.FilterLines = new string[Math.Min(35, filterLines.Length - start)];
                    Array.Copy(filterLines, start, details.FilterLines, 0, details.FilterLines.Length);
                    pages.Add(details);
                }
                filterLines = new string[] { "Applied filters: see filter details on preceding pages." };
            }
            var page = new Page(); page.FilterLines = filterLines; pages.Add(page);
            float top = BodyTop(page), y = top;
            string activeRack = "";
            LineRow pendingHeading = null;
            foreach (DataRow data in report.Rows.Rows)
            {
                string kind = Convert.ToString(data["Kind"]);
                LineRow row;
                if (kind == "Rack")
                {
                    activeRack = Convert.ToString(data["Location"]) + " / Rack: " + data["Rack"];
                    pendingHeading = Heading(activeRack);
                    continue;
                }
                else
                {
                    row = new LineRow(); row.Bold = kind != "Detail"; row.Cells = new string[PrintFields.Length][];
                    int lines = 1;
                    for (int c = 0; c < PrintFields.Length; c++)
                    {
                        string value = PrintFields[c] == "Quantity" && data[PrintFields[c]] != DBNull.Value ? Convert.ToDecimal(data[PrintFields[c]]).ToString("0.###", Invariant) : Convert.ToString(data[PrintFields[c]]);
                        row.Cells[c] = Wrap(value, Widths[c] - 10, row.Bold);
                        lines = Math.Max(lines, row.Cells[c].Length);
                    }
                    row.Height = 8 + lines * 12;
                }
                // Keep a rack heading with the first row, and repeat its context
                // when a rack continues on the next page. Never split a row.
                float headingHeight = pendingHeading == null ? 0 : pendingHeading.Height;
                if (y + row.Height + headingHeight > 550 && page.Rows.Count > 0)
                {
                    page = new Page(); page.FilterLines = filterLines; pages.Add(page); y = top;
                    if (pendingHeading == null && activeRack.Length > 0 && Convert.ToString(data["ProductName"]) != "Grand Total")
                    {
                        LineRow continued = Heading(activeRack + " (continued)");
                        page.Rows.Add(continued); y += continued.Height;
                    }
                }
                if (pendingHeading != null) { page.Rows.Add(pendingHeading); y += pendingHeading.Height; pendingHeading = null; }
                if (y + row.Height > 550) throw new InvalidOperationException("A report row is too tall for A4. Shorten the product details or filters before printing.");
                page.Rows.Add(row); y += row.Height;
            }
            return pages;
        }

        private delegate void DrawText(float x, float baseline, string value, float size, bool bold, bool right);
        private delegate void DrawFill(float x, float y, float width, float height, Color color);
        private delegate void DrawLine(float x1, float y1, float x2, float y2);
        private const float BorderWidth = .75f;
        private static void DrawRowBorders(float y, float height, bool merged, DrawLine line)
        {
            line(30, y + height, 812, y + height);
            if (!merged)
                for (int c = 1; c < Lefts.Length; c++) line(Lefts[c], y, Lefts[c], y + height);
        }
        private static void DrawPage(RackStockSnapshot report, Page page, int number, int count, DrawText text, DrawFill fill, DrawLine line)
        {
            text(30, 29, RackStockSnapshot.Title, 13, true, false);
            text(30, 45, "Generated: " + report.Generated.ToString("dd-MMM-yyyy HH:mm:ss"), 9, false, false);
            float y = 60;
            foreach (string filterLine in page.FilterLines) { text(30, y, filterLine, 9, false, false); y += 12; }
            if (!page.FiltersOnly)
            {
            float tableTop = y;
            fill(30, y, 782, 20, Color.FromArgb(235, 238, 242));
            for (int c = 0; c < PrintFields.Length; c++) text(PrintFields[c] == "Quantity" ? 807 : Lefts[c] + 4, y + 14, PrintHeaders[c], 9, true, PrintFields[c] == "Quantity");
            y = BodyTop(page);
            foreach (LineRow row in page.Rows)
            {
                fill(30, y, 782, row.Height, row.RackHeading ? Color.FromArgb(235, 238, 242) : row.Bold ? Color.FromArgb(245, 247, 249) : Color.White);
                for (int c = 0; c < PrintFields.Length; c++)
                    for (int i = 0; i < row.Cells[c].Length; i++)
                        text(PrintFields[c] == "Quantity" ? 807 : Lefts[c] + 4, y + 13 + i * 12, row.Cells[c][i], 9, row.Bold, PrintFields[c] == "Quantity");
                y += row.Height;
            }
            // Draw the complete grid last so row fills cannot cover any part
            // of a stroke. Use continuous black lines for the outside edges.
            float tableBottom = y;
            DrawRowBorders(tableTop, 20, false, line);
            y = BodyTop(page);
            foreach (LineRow row in page.Rows)
            {
                DrawRowBorders(y, row.Height, row.RackHeading, line);
                y += row.Height;
            }
            line(30, tableTop, 812, tableTop);
            line(30, tableTop, 30, tableBottom);
            line(812, tableTop, 812, tableBottom);
            }
            text(30, 571, report.CountSummary, 9, false, false);
            text(812, 585, "Page " + number + " of " + count, 9, false, true);
        }
        public static void SavePdf(RackStockSnapshot report, string path)
        {
            var pages = Paginate(report);
            var pdf = new SimplePdf(SimplePdf.PageHeight, SimplePdf.PageWidth);
            pdf.Title = RackStockSnapshot.Title;
            for (int p = 0; p < pages.Count; p++)
            {
                pdf.NewPage();
                DrawPage(report, pages[p], p + 1, pages.Count,
                    delegate(float x, float y, string value, float size, bool bold, bool right)
                    { if (right) pdf.TextRight(x, y, value, size, bold, Color.Black); else pdf.Text(x, y, value, size, bold, Color.Black); },
                    delegate(float x, float y, float w, float h, Color color) { pdf.FillRect(x, y, w, h, color); },
                    delegate(float x1, float y1, float x2, float y2) { pdf.Line(x1, y1, x2, y2, BorderWidth, Color.Black); });
            }
            pdf.Save(path);
        }
        public static void DrawPrintPage(RackStockSnapshot report, Page page, Graphics graphics, Rectangle bounds, int number, int count)
        {
            GraphicsState saved = graphics.Save();
            try
            {
                graphics.PageUnit = GraphicsUnit.Point;
                graphics.TranslateTransform(bounds.Left * .72f, bounds.Top * .72f);
                float scale = Math.Min(bounds.Width * .72f / SimplePdf.PageHeight, bounds.Height * .72f / SimplePdf.PageWidth);
                graphics.ScaleTransform(scale, scale);
                using (var normal = new Font("Arial", 9))
                using (var boldFont = new Font("Arial", 9, FontStyle.Bold))
                using (var title = new Font("Arial", 13, FontStyle.Bold))
                using (var gridPen = new Pen(Color.Black, BorderWidth))
                using (var format = new StringFormat(StringFormat.GenericTypographic))
                {
                    DrawPage(report, page, number, count,
                        delegate(float x, float y, string value, float size, bool bold, bool right)
                        {
                            Font font = size == 13 ? title : bold ? boldFont : normal;
                            float width = graphics.MeasureString(value, font, int.MaxValue, format).Width;
                            graphics.DrawString(value, font, Brushes.Black, right ? x - width : x, y - size, format);
                        },
                        delegate(float x, float y, float w, float h, Color color)
                        { using (var brush = new SolidBrush(color)) graphics.FillRectangle(brush, x, y, w, h); },
                        delegate(float x1, float y1, float x2, float y2) { graphics.DrawLine(gridPen, x1, y1, x2, y2); });
                }
            }
            finally { graphics.Restore(saved); }
        }
    }
}
