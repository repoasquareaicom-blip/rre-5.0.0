using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Inventory.InventoryNotes
{
    internal sealed class DeliveryNotePrinter
    {
        private readonly DataRow header;
        private readonly DataTable details;

        public DeliveryNotePrinter(DataRow header, DataTable details)
        {
            this.header = header;
            this.details = details;
        }

        public void ShowPreview()
        {
            PrintDocument document = new PrintDocument();
            document.DocumentName = Convert.ToString(header["DeliveryNoteNo"]);
            document.PrintPage += PrintPage;

            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = document;
            preview.WindowState = FormWindowState.Maximized;
            preview.ShowDialog();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle margin = e.MarginBounds;
            int y = margin.Top;
            Font titleFont = new Font("Arial", 16F, FontStyle.Bold);
            Font headerFont = new Font("Arial", 9F, FontStyle.Bold);
            Font normalFont = new Font("Arial", 9F);
            Font smallFont = new Font("Arial", 8F);
            Pen pen = Pens.Black;

            string company = string.IsNullOrEmpty(Program.ShopName) ? "Delivery Note" : Program.ShopName;
            DrawCentered(g, company, titleFont, margin.Left, y, margin.Width);
            y += 28;
            DrawCentered(g, "DELIVERY NOTE", headerFont, margin.Left, y, margin.Width);
            y += 30;

            g.DrawRectangle(pen, margin.Left, y, margin.Width, 92);
            DrawPair(g, "Delivery Note No", Convert.ToString(header["DeliveryNoteNo"]), margin.Left + 10, y + 10, normalFont, headerFont);
            DrawPair(g, "Date", FormatDate(header["DeliveryNoteDate"]), margin.Left + 370, y + 10, normalFont, headerFont);
            DrawPair(g, "From", Convert.ToString(header["FromLocation"]), margin.Left + 10, y + 34, normalFont, headerFont);
            DrawPair(g, "To", Convert.ToString(header["ToLocation"]), margin.Left + 370, y + 34, normalFont, headerFont);
            DrawPair(g, "Reference No", Convert.ToString(header["ReferenceNo"]), margin.Left + 10, y + 58, normalFont, headerFont);
            DrawPair(g, "Reference Date", FormatDate(header["ReferenceDate"]), margin.Left + 370, y + 58, normalFont, headerFont);
            y += 110;

            int[] widths = new int[] { 40, 95, 260, 95, 70, 55, 75, 150 };
            string[] heads = new string[] { "S.No", "Item Code", "Product", "Brand", "Size", "UOM", "Qty", "Remarks" };
            int x = margin.Left;
            int rowHeight = 24;
            for (int i = 0; i < heads.Length; i++)
            {
                g.DrawRectangle(pen, x, y, widths[i], rowHeight);
                g.DrawString(heads[i], headerFont, Brushes.Black, new RectangleF(x + 3, y + 5, widths[i] - 6, rowHeight - 5));
                x += widths[i];
            }
            y += rowHeight;

            decimal totalQty = 0;
            for (int r = 0; r < details.Rows.Count; r++)
            {
                DataRow row = details.Rows[r];
                decimal qty = Convert.ToDecimal(row["Quantity"]);
                totalQty += qty;
                string[] vals = new string[]
                {
                    Convert.ToString(r + 1),
                    Convert.ToString(row["ItemCode"]),
                    Convert.ToString(row["ProductName"]),
                    Convert.ToString(row["Brand"]),
                    Convert.ToString(row["Size"]),
                    Convert.ToString(row["UOM"]),
                    qty.ToString("0.###"),
                    Convert.ToString(row["Remarks"])
                };
                x = margin.Left;
                for (int c = 0; c < vals.Length; c++)
                {
                    g.DrawRectangle(pen, x, y, widths[c], rowHeight);
                    g.DrawString(vals[c], smallFont, Brushes.Black, new RectangleF(x + 3, y + 5, widths[c] - 6, rowHeight - 5));
                    x += widths[c];
                }
                y += rowHeight;
            }

            x = margin.Left;
            int totalLabelWidth = widths[0] + widths[1] + widths[2] + widths[3] + widths[4] + widths[5];
            g.DrawRectangle(pen, x, y, totalLabelWidth, rowHeight);
            g.DrawString("Total", headerFont, Brushes.Black, new RectangleF(x + totalLabelWidth - 55, y + 5, 50, rowHeight - 5));
            x += totalLabelWidth;
            g.DrawRectangle(pen, x, y, widths[6], rowHeight);
            g.DrawString(totalQty.ToString("0.###"), headerFont, Brushes.Black, new RectangleF(x + 3, y + 5, widths[6] - 6, rowHeight - 5));
            x += widths[6];
            g.DrawRectangle(pen, x, y, widths[7], rowHeight);
            y += 42;

            if (Convert.ToString(header["Remarks"]) != "")
            {
                g.DrawString("Remarks: " + Convert.ToString(header["Remarks"]), normalFont, Brushes.Black, margin.Left, y);
                y += 38;
            }

            g.DrawString("Entered By: " + Convert.ToString(header["EnteredBy"]), normalFont, Brushes.Black, margin.Left, y);
            g.DrawString("Approved By: " + Convert.ToString(header["ApprovedBy"]), normalFont, Brushes.Black, margin.Left + 370, y);
            y += 70;
            g.DrawString("Prepared By", headerFont, Brushes.Black, margin.Left + 20, y);
            g.DrawString("Received By", headerFont, Brushes.Black, margin.Left + 310, y);
            g.DrawString("Authorized Signatory", headerFont, Brushes.Black, margin.Left + 570, y);
        }

        private static void DrawPair(Graphics g, string label, string value, int x, int y, Font normalFont, Font headerFont)
        {
            g.DrawString(label + ":", headerFont, Brushes.Black, x, y);
            g.DrawString(value, normalFont, Brushes.Black, x + 115, y);
        }

        private static void DrawCentered(Graphics g, string text, Font font, int x, int y, int width)
        {
            SizeF size = g.MeasureString(text, font);
            g.DrawString(text, font, Brushes.Black, x + ((width - size.Width) / 2), y);
        }

        private static string FormatDate(object value)
        {
            if (value == null || value == DBNull.Value)
                return "";
            return Convert.ToDateTime(value).ToString("dd-MM-yyyy");
        }
    }
}
