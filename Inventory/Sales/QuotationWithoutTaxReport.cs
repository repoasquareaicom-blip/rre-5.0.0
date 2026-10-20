using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Inventory.Sales
{
    public class QuotationWithoutTaxReport : Form
    {
        private readonly string quotationId;
        private readonly PrintDocument printDocument;
        private readonly PrintPreviewControl previewControl;
        private Quotationdataset reportData;
        private int printRowIndex;
        private int printPageNumber;
        private int totalPageCount;
        private int currentPreviewPage;
        private ToolStripButton previousPageButton;
        private ToolStripButton nextPageButton;
        private ToolStripLabel pageStatusLabel;

        public QuotationWithoutTaxReport(string quotationId)
        {
            this.quotationId = quotationId;
            Text = "Quotation Without Tax";
            WindowState = FormWindowState.Maximized;

            printDocument = new PrintDocument();
            printDocument.DocumentName = "Quotation Without Tax";
            printDocument.DefaultPageSettings.Margins = new Margins(45, 45, 45, 45);
            printDocument.BeginPrint += PrintDocument_BeginPrint;
            printDocument.PrintPage += PrintDocument_PrintPage;

            ToolStrip toolStrip = new ToolStrip();
            ToolStripButton printButton = new ToolStripButton("Print");
            ToolStripButton savePdfButton = new ToolStripButton("Save PDF");
            ToolStripButton pageSetupButton = new ToolStripButton("Page Setup");
            previousPageButton = new ToolStripButton("Previous");
            nextPageButton = new ToolStripButton("Next");
            pageStatusLabel = new ToolStripLabel("Page 1 of 1");
            ToolStripButton closeButton = new ToolStripButton("Close");

            printButton.Click += PrintButton_Click;
            savePdfButton.Click += SavePdfButton_Click;
            pageSetupButton.Click += PageSetupButton_Click;
            previousPageButton.Click += PreviousPageButton_Click;
            nextPageButton.Click += NextPageButton_Click;
            closeButton.Click += delegate { Close(); };

            toolStrip.Items.Add(printButton);
            toolStrip.Items.Add(savePdfButton);
            toolStrip.Items.Add(pageSetupButton);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(previousPageButton);
            toolStrip.Items.Add(nextPageButton);
            toolStrip.Items.Add(pageStatusLabel);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(closeButton);
            toolStrip.Dock = DockStyle.Top;

            previewControl = new PrintPreviewControl();
            previewControl.Document = printDocument;
            previewControl.Dock = DockStyle.Fill;
            previewControl.Zoom = 1.0;
            previewControl.BackColor = Color.FromArgb(150, 150, 150);

            Controls.Add(previewControl);
            Controls.Add(toolStrip);

            Load += QuotationWithoutTaxReport_Load;
        }

        private void QuotationWithoutTaxReport_Load(object sender, EventArgs e)
        {
            try
            {
                reportData = BuildDataSet();
                currentPreviewPage = 0;
                UpdatePreviewPageCount();
                previewControl.InvalidatePreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show(GetFullErrorMessage(ex), "Quotation Without Tax report error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            using (PrintDialog dialog = new PrintDialog())
            {
                dialog.Document = printDocument;
                dialog.UseEXDialog = true;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
        }

        private void PageSetupButton_Click(object sender, EventArgs e)
        {
            using (PageSetupDialog dialog = new PageSetupDialog())
            {
                dialog.Document = printDocument;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    currentPreviewPage = 0;
                    previewControl.StartPage = 0;
                    UpdatePreviewPageCount();
                    previewControl.InvalidatePreview();
                }
            }
        }

        private void PreviousPageButton_Click(object sender, EventArgs e)
        {
            if (currentPreviewPage <= 0)
            {
                return;
            }

            currentPreviewPage--;
            previewControl.StartPage = currentPreviewPage;
            UpdatePageStatus();
            previewControl.Invalidate();
        }

        private void NextPageButton_Click(object sender, EventArgs e)
        {
            if (currentPreviewPage >= totalPageCount - 1)
            {
                return;
            }

            currentPreviewPage++;
            previewControl.StartPage = currentPreviewPage;
            UpdatePageStatus();
            previewControl.Invalidate();
        }

        private void SavePdfButton_Click(object sender, EventArgs e)
        {
            string pdfPrinter = FindPdfPrinter();
            if (string.IsNullOrEmpty(pdfPrinter))
            {
                MessageBox.Show("Microsoft Print to PDF printer is not installed on this machine.", "Save PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Save quotation PDF";
                dialog.Filter = "PDF files (*.pdf)|*.pdf";
                dialog.FileName = "Quotation-" + quotationId + "-WithoutTax.pdf";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                PrinterSettings oldPrinterSettings = printDocument.PrinterSettings;
                PrintController oldController = printDocument.PrintController;

                try
                {
                    PrinterSettings settings = new PrinterSettings();
                    settings.PrinterName = pdfPrinter;
                    settings.PrintToFile = true;
                    settings.PrintFileName = dialog.FileName;

                    printDocument.PrinterSettings = settings;
                    printDocument.PrintController = new StandardPrintController();
                    printDocument.Print();

                    MessageBox.Show("PDF saved successfully.", "Save PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(GetFullErrorMessage(ex), "Save PDF error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    printDocument.PrinterSettings = oldPrinterSettings;
                    printDocument.PrintController = oldController;
                }
            }
        }

        private static string FindPdfPrinter()
        {
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                if (string.Equals(printer, "Microsoft Print to PDF", StringComparison.OrdinalIgnoreCase))
                {
                    return printer;
                }
            }

            return "";
        }

        private void UpdatePreviewPageCount()
        {
            if (reportData == null || reportData.Tables.Count < 2)
            {
                totalPageCount = 1;
            }
            else
            {
                totalPageCount = CalculateTotalPages(GetCurrentPageBounds(), reportData.Tables[1].Rows.Count);
            }

            UpdatePageStatus();
        }

        private void UpdatePageStatus()
        {
            if (pageStatusLabel != null)
            {
                pageStatusLabel.Text = "Page " + (currentPreviewPage + 1).ToString(CultureInfo.InvariantCulture) + " of " + totalPageCount.ToString(CultureInfo.InvariantCulture);
            }

            if (previousPageButton != null)
            {
                previousPageButton.Enabled = currentPreviewPage > 0;
            }

            if (nextPageButton != null)
            {
                nextPageButton.Enabled = currentPreviewPage < totalPageCount - 1;
            }
        }

        private RectangleF GetCurrentPageBounds()
        {
            PageSettings settings = printDocument.DefaultPageSettings;
            Rectangle bounds = settings.Bounds;
            Margins margins = settings.Margins;
            return new RectangleF(
                bounds.Left + margins.Left,
                bounds.Top + margins.Top,
                bounds.Width - margins.Left - margins.Right,
                bounds.Height - margins.Top - margins.Bottom);
        }

        private Quotationdataset BuildDataSet()
        {
            Quotationdataset dataSet = new Quotationdataset();

            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("GetQuotationreport", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", quotationId);
                cmd.Parameters.AddWithValue("@companyname", Program.Company);

                con.Open();
                ad.Fill(dataSet.Tables[0]);

                cmd.CommandText = "GetQuotationreportdetails";
                ad.Fill(dataSet.Tables[1]);
            }

            if (dataSet.Tables.Count < 2 || dataSet.Tables[0].Rows.Count == 0)
            {
                throw new ApplicationException("Quotation not found: " + quotationId);
            }

            if (dataSet.Tables[1].Rows.Count == 0)
            {
                throw new ApplicationException("Quotation details not found: " + quotationId);
            }

            ApplyWithoutTaxAmounts(dataSet);
            return dataSet;
        }

        private void ApplyWithoutTaxAmounts(Quotationdataset dataSet)
        {
            DataTable details = dataSet.Tables[1];
            decimal totalAmount = 0m;

            foreach (DataRow row in details.Rows)
            {
                decimal amount = GetLineAmount(row);
                totalAmount += amount;
                SetFirstExisting(row, FormatAmount(amount), "Amount", "TAXABLEVALUE", "NetAmount", "BasicAmount");
            }

            DataTable header = dataSet.Tables[0];
            if (header.Rows.Count > 0)
            {
                DataRow row = header.Rows[0];
                SetFirstExisting(row, FormatAmount(totalAmount), "TOTALAMOUNT", "TOTALTAXABLE", "NETAMOUNT", "GrandTotal", "TotalAmount", "NetAmount", "Amount");
                SetFirstExisting(row, "0.00", "LESSAMOUNT", "Less", "Discount", "RoundOff");
                PrefixPreparedBy(row);
            }
        }

        private static void PrefixPreparedBy(DataRow row)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains("Assist"))
            {
                return;
            }

            string preparedBy = Convert.ToString(row["Assist"]);
            preparedBy = preparedBy == null ? "" : preparedBy.Trim();
            if (preparedBy.StartsWith("Prepared By:", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            row["Assist"] = string.IsNullOrEmpty(preparedBy) ? "Prepared By:" : "Prepared By: " + preparedBy;
        }

        private decimal GetLineAmount(DataRow row)
        {
            decimal amount = FirstExistingDecimal(row, "TAXABLEVALUE", "BasicAmount", "NetAmount", "Amount");
            if (amount != 0m)
            {
                return amount;
            }

            decimal quantity = FirstExistingDecimal(row, "Quantity", "Qty");
            decimal rate = FirstExistingDecimal(row, "Rate", "TRate");
            decimal gross = quantity * rate;
            decimal discount = FirstExistingDecimal(row, "DiscountAmount", "DiscAmount", "Discount");
            if (discount == 0m)
            {
                decimal discountPercent = FirstExistingDecimal(row, "DiscountPer", "DiscPer");
                if (discountPercent != 0m)
                {
                    discount = decimal.Round(gross * discountPercent / 100m, 2);
                }
            }

            return gross - discount;
        }

        private void PrintDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            printRowIndex = 0;
            printPageNumber = 0;
            totalPageCount = 1;
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (reportData == null)
            {
                e.HasMorePages = false;
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            RectangleF page = e.MarginBounds;
            DataRow header = reportData.Tables[0].Rows[0];
            DataTable details = reportData.Tables[1];
            if (printPageNumber == 0)
            {
                totalPageCount = CalculateTotalPages(page, details.Rows.Count);
            }

            printPageNumber++;

            Color red = Color.FromArgb(170, 18, 28);
            Color darkRed = Color.FromArgb(130, 0, 12);
            Color lightRed = Color.FromArgb(255, 244, 245);
            Color border = Color.FromArgb(205, 44, 50);
            Color text = Color.FromArgb(24, 24, 24);

            using (Pen outerPen = new Pen(Color.Black, 1.2f))
            using (Pen redPen = new Pen(border, 1.0f))
            using (SolidBrush redBrush = new SolidBrush(red))
            using (SolidBrush darkRedBrush = new SolidBrush(darkRed))
            using (SolidBrush textBrush = new SolidBrush(text))
            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            using (SolidBrush lightRedBrush = new SolidBrush(lightRed))
            using (Font companyFont = new Font("Times New Roman", 22f, FontStyle.Bold))
            using (Font addressFont = new Font("Times New Roman", 10.5f, FontStyle.Regular))
            using (Font titleFont = new Font("Times New Roman", 16f, FontStyle.Bold))
            using (Font labelFont = new Font("Times New Roman", 11f, FontStyle.Bold))
            using (Font valueFont = new Font("Times New Roman", 11f, FontStyle.Regular))
            using (Font tableHeadFont = new Font("Times New Roman", 10.5f, FontStyle.Bold))
            using (Font rowFont = new Font("Times New Roman", 10.5f, FontStyle.Regular))
            using (Font totalFont = new Font("Times New Roman", 14f, FontStyle.Bold))
            using (Font footerFont = new Font("Times New Roman", 11f, FontStyle.Regular))
            {
                DrawPageBorder(e.Graphics, page, outerPen);

                float y = page.Top + 12;
                DrawCentered(e.Graphics, GetValue(header, "CompanyName"), companyFont, redBrush, page.Left, y, page.Width, 30);
                y += 31;
                DrawCentered(e.Graphics, GetValue(header, "DoorNo"), addressFont, textBrush, page.Left, y, page.Width, 16);
                y += 17;
                DrawCentered(e.Graphics, GetValue(header, "Address1"), addressFont, textBrush, page.Left, y, page.Width, 16);
                y += 17;
                DrawCentered(e.Graphics, GetPhoneLine(header), addressFont, textBrush, page.Left, y, page.Width, 16);

                y += 24;
                RectangleF titleBox = new RectangleF(page.Left + 18, y, page.Width - 36, 30);
                FillRoundRect(e.Graphics, lightRedBrush, titleBox, 12f);
                e.Graphics.DrawLine(new Pen(red, 1.4f), titleBox.Left + 18, titleBox.Top + titleBox.Height / 2, titleBox.Left + titleBox.Width * 0.38f, titleBox.Top + titleBox.Height / 2);
                e.Graphics.DrawLine(new Pen(red, 1.4f), titleBox.Left + titleBox.Width * 0.62f, titleBox.Top + titleBox.Height / 2, titleBox.Right - 18, titleBox.Top + titleBox.Height / 2);
                DrawCentered(e.Graphics, "QUOTATION", titleFont, redBrush, titleBox.Left, titleBox.Top + 3, titleBox.Width, 24);

                y += 46;
                float infoTop = y;
                float half = page.Width / 2f;
                DrawLabelValue(e.Graphics, "To", GetValue(header, "customername"), labelFont, valueFont, redBrush, textBrush, page.Left + 10, infoTop, half - 20, 22);
                DrawRightLabelValue(e.Graphics, "Quotation No", GetValue(header, "Quotationid"), labelFont, valueFont, redBrush, textBrush, page.Left + half, infoTop, half - 10, 22);
                DrawRightLabelValue(e.Graphics, "Date", GetValue(header, "Date"), labelFont, valueFont, redBrush, textBrush, page.Left + half, infoTop + 26, half - 10, 22);

                y += 58;
                float[] widths = GetTableWidths(page.Width - 20);
                float tableLeft = page.Left + 10;
                float rowHeight = 30;
                float headerHeight = 34;
                float tableBottomLimit = page.Bottom - 185;

                DrawTableHeader(e.Graphics, tableLeft, y, widths, headerHeight, tableHeadFont, whiteBrush, darkRedBrush, redPen);
                y += headerHeight;

                while (printRowIndex < details.Rows.Count && y + rowHeight <= tableBottomLimit)
                {
                    DrawDetailRow(e.Graphics, details.Rows[printRowIndex], tableLeft, y, widths, rowHeight, rowFont, textBrush, redPen);
                    y += rowHeight;
                    printRowIndex++;
                }

                if (printRowIndex < details.Rows.Count)
                {
                    DrawPageNumber(e.Graphics, page, footerFont, textBrush);
                    e.HasMorePages = true;
                    return;
                }

                y += 14;
                DrawTotal(e.Graphics, page, y, GetTotal(header, details), totalFont, whiteBrush, darkRedBrush, redPen);

                y += 58;
                DrawText(e.Graphics, GetValue(header, "Assist"), labelFont, textBrush, new RectangleF(page.Left + 10, y, page.Width * 0.6f, 24), StringAlignment.Near, StringAlignment.Center);

                y += 34;
                DrawAgencySignOff(e.Graphics, page, y, footerFont, labelFont, redBrush, textBrush);
                DrawPageNumber(e.Graphics, page, footerFont, textBrush);
            }

            e.HasMorePages = false;
        }

        private static int CalculateTotalPages(RectangleF page, int rowCount)
        {
            float tableStart = GetTableStartY(page);
            float tableBottomLimit = page.Bottom - 185;
            int rowsPerPage = Math.Max(1, (int)Math.Floor((tableBottomLimit - tableStart - 34) / 30));
            return Math.Max(1, (int)Math.Ceiling(rowCount / (double)rowsPerPage));
        }

        private static float GetTableStartY(RectangleF page)
        {
            return page.Top + 12 + 31 + 17 + 17 + 16 + 24 + 30 + 46 + 58;
        }

        private static void DrawPageBorder(Graphics g, RectangleF page, Pen pen)
        {
            RectangleF border = new RectangleF(page.Left - 20, page.Top - 20, page.Width + 40, page.Height + 40);
            g.DrawRectangle(pen, border.X, border.Y, border.Width, border.Height);
        }

        private static float[] GetTableWidths(float totalWidth)
        {
            return new float[]
            {
                totalWidth * 0.09f,
                totalWidth * 0.48f,
                totalWidth * 0.14f,
                totalWidth * 0.14f,
                totalWidth * 0.15f
            };
        }

        private static void DrawTableHeader(Graphics g, float x, float y, float[] widths, float height, Font font, Brush white, Brush fill, Pen pen)
        {
            string[] headers = new string[] { "S.No", "Product Name", "Qty", "Rate", "Amount" };
            float current = x;
            for (int i = 0; i < headers.Length; i++)
            {
                RectangleF rect = new RectangleF(current, y, widths[i], height);
                g.FillRectangle(fill, rect);
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                DrawText(g, headers[i], font, white, rect, StringAlignment.Center, StringAlignment.Center);
                current += widths[i];
            }
        }

        private static void DrawDetailRow(Graphics g, DataRow row, float x, float y, float[] widths, float height, Font font, Brush brush, Pen pen)
        {
            string[] values = new string[]
            {
                GetValue(row, "Sino"),
                GetValue(row, "ItemName"),
                GetValue(row, "Quantity"),
                FormatNumeric(GetValue(row, "Rate")),
                FormatNumeric(GetValue(row, "Amount"))
            };

            float current = x;
            for (int i = 0; i < values.Length; i++)
            {
                RectangleF rect = new RectangleF(current, y, widths[i], height);
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                RectangleF textRect = new RectangleF(rect.X + 5, rect.Y, rect.Width - 10, rect.Height);
                StringAlignment align = i == 1 ? StringAlignment.Near : (i == 0 || i == 2 ? StringAlignment.Center : StringAlignment.Far);
                DrawText(g, values[i], font, brush, textRect, align, StringAlignment.Center);
                current += widths[i];
            }
        }

        private static void DrawTotal(Graphics g, RectangleF page, float y, string total, Font font, Brush white, Brush fill, Pen pen)
        {
            float width = 270;
            float left = page.Right - width - 10;
            RectangleF label = new RectangleF(left, y, width * 0.58f, 38);
            RectangleF value = new RectangleF(label.Right, y, width * 0.42f, 38);

            g.FillRectangle(fill, label);
            g.FillRectangle(fill, value);
            g.DrawRectangle(pen, label.X, label.Y, label.Width, label.Height);
            g.DrawRectangle(pen, value.X, value.Y, value.Width, value.Height);
            DrawText(g, "TOTAL", font, white, label, StringAlignment.Center, StringAlignment.Center);
            DrawText(g, total, font, white, new RectangleF(value.X + 4, value.Y, value.Width - 10, value.Height), StringAlignment.Far, StringAlignment.Center);
        }

        private static void DrawAgencySignOff(Graphics g, RectangleF page, float y, Font footerFont, Font boldFont, Brush red, Brush text)
        {
            float sigY = Math.Min(y + 54, page.Bottom - 80);
            g.DrawLine(Pens.Black, page.Right - 310, sigY, page.Right - 45, sigY);

            DrawText(g, "For R.R. ELECTRICAL AGENCIES", boldFont, red, new RectangleF(page.Right - 360, sigY - 42, 320, 28), StringAlignment.Far, StringAlignment.Center);
            DrawText(g, "Authorised Signatory", footerFont, text, new RectangleF(page.Right - 310, sigY + 8, 265, 22), StringAlignment.Center, StringAlignment.Center);
        }

        private void DrawPageNumber(Graphics g, RectangleF page, Font font, Brush brush)
        {
            string pageText = "Page " + printPageNumber.ToString(CultureInfo.InvariantCulture) + " of " + totalPageCount.ToString(CultureInfo.InvariantCulture);
            DrawText(g, pageText, font, brush, new RectangleF(page.Left, page.Bottom - 26, page.Width, 20), StringAlignment.Far, StringAlignment.Center);
        }

        private static void DrawLabelValue(Graphics g, string label, string value, Font labelFont, Font valueFont, Brush labelBrush, Brush valueBrush, float x, float y, float width, float height)
        {
            DrawText(g, label + " :", labelFont, labelBrush, new RectangleF(x, y, 78, height), StringAlignment.Near, StringAlignment.Center);
            DrawText(g, value, valueFont, valueBrush, new RectangleF(x + 88, y, width - 88, height), StringAlignment.Near, StringAlignment.Center);
        }

        private static void DrawRightLabelValue(Graphics g, string label, string value, Font labelFont, Font valueFont, Brush labelBrush, Brush valueBrush, float x, float y, float width, float height)
        {
            DrawText(g, label + " :", labelFont, labelBrush, new RectangleF(x, y, width * 0.45f, height), StringAlignment.Far, StringAlignment.Center);
            DrawText(g, value, valueFont, valueBrush, new RectangleF(x + width * 0.46f, y, width * 0.54f, height), StringAlignment.Far, StringAlignment.Center);
        }

        private static void DrawCentered(Graphics g, string text, Font font, Brush brush, float x, float y, float width, float height)
        {
            DrawText(g, text, font, brush, new RectangleF(x, y, width, height), StringAlignment.Center, StringAlignment.Center);
        }

        private static void DrawText(Graphics g, string value, Font font, Brush brush, RectangleF rect, StringAlignment horizontal, StringAlignment vertical)
        {
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = horizontal;
                format.LineAlignment = vertical;
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.NoClip;
                g.DrawString(value ?? "", font, brush, rect, format);
            }
        }

        private static void FillRoundRect(Graphics g, Brush brush, RectangleF rect, float radius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                float diameter = radius * 2f;
                path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }

        private static string GetPhoneLine(DataRow row)
        {
            string phone1 = GetValue(row, "Phone1");
            string phone2 = GetValue(row, "Phone2");
            if (!string.IsNullOrEmpty(phone1) && !string.IsNullOrEmpty(phone2))
            {
                return "Phone: " + phone1 + ", " + phone2;
            }

            if (!string.IsNullOrEmpty(phone1))
            {
                return "Phone: " + phone1;
            }

            return phone2;
        }

        private static string GetTotal(DataRow header, DataTable details)
        {
            string total = FirstValue(header, "TOTALAMOUNT", "TOTALTAXABLE", "NETAMOUNT", "GrandTotal", "TotalAmount", "NetAmount", "Amount");
            if (!string.IsNullOrEmpty(total))
            {
                return FormatNumeric(total);
            }

            decimal sum = 0m;
            foreach (DataRow row in details.Rows)
            {
                sum += FirstExistingDecimal(row, "Amount", "TAXABLEVALUE", "NetAmount", "BasicAmount");
            }

            return FormatAmount(sum);
        }

        private static string FirstValue(DataRow row, params string[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                string value = GetValue(row, columns[i]);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            return "";
        }

        private static decimal FirstExistingDecimal(DataRow row, params string[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                string value = GetValue(row, columns[i]);
                decimal result;
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) && result != 0m)
                {
                    return result;
                }

                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result) && result != 0m)
                {
                    return result;
                }
            }

            return 0m;
        }

        private static string FormatNumeric(string value)
        {
            decimal number;
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out number)
                || decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
            {
                return FormatAmount(number);
            }

            return value ?? "";
        }

        private static void SetFirstExisting(DataRow row, string value, params string[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                if (row.Table.Columns.Contains(columns[i]))
                {
                    Set(row, columns[i], value);
                    return;
                }
            }
        }

        private static void Set(DataRow row, string column, string value)
        {
            if (!row.Table.Columns.Contains(column))
            {
                return;
            }

            Type columnType = row.Table.Columns[column].DataType;
            if (columnType == typeof(decimal) || columnType == typeof(double) || columnType == typeof(float))
            {
                decimal decimalValue;
                if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimalValue))
                {
                    decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out decimalValue);
                }

                row[column] = decimalValue;
                return;
            }

            row[column] = value ?? "";
        }

        private static string GetValue(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return "";
            }

            return Convert.ToString(row[column]);
        }

        private static string FormatAmount(decimal value)
        {
            return value.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private static string GetFullErrorMessage(Exception ex)
        {
            string message = "";
            while (ex != null)
            {
                if (message.Length > 0)
                {
                    message += "\r\n";
                }

                message += ex.GetType().Name + ": " + ex.Message;
                ex = ex.InnerException;
            }

            return message;
        }
    }
}
