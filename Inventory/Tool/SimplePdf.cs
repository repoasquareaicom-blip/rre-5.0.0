using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace Inventory.Tool
{
    public class SimplePdf
    {
        public const float PageWidth = 595.28f;
        public const float PageHeight = 841.89f;

        private static readonly int[] RegularWidths = new int[]
        {
            278, 278, 355, 556, 556, 889, 667, 191, 333, 333, 389, 584, 278, 333, 278, 278,
            556, 556, 556, 556, 556, 556, 556, 556, 556, 556,
            278, 278, 584, 584, 584, 556, 1015,
            667, 667, 722, 722, 667, 611, 778, 722, 278, 500, 667, 556, 833, 722, 778, 667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611,
            278, 278, 278, 469, 556, 333,
            556, 556, 500, 556, 556, 278, 556, 556, 222, 222, 500, 222, 833, 556, 556, 556, 556, 333, 500, 278, 556, 500, 722, 500, 500, 500,
            334, 260, 334, 584
        };

        private static readonly int[] BoldWidths = new int[]
        {
            278, 333, 474, 556, 556, 889, 722, 238, 333, 333, 389, 584, 278, 333, 278, 278,
            556, 556, 556, 556, 556, 556, 556, 556, 556, 556,
            333, 333, 584, 584, 584, 611, 975,
            722, 722, 722, 722, 667, 611, 778, 722, 278, 556, 722, 611, 833, 722, 778, 667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611,
            333, 278, 333, 584, 556, 333,
            556, 611, 556, 611, 556, 333, 611, 611, 278, 278, 556, 278, 889, 611, 611, 611, 611, 389, 556, 333, 611, 556, 778, 556, 556, 500,
            389, 280, 389, 584
        };

        private readonly float pageWidth;
        private readonly float pageHeight;
        public SimplePdf() : this(PageWidth, PageHeight) { }
        public SimplePdf(float width, float height) { pageWidth = width; pageHeight = height; }

        private readonly List<StringBuilder> pages = new List<StringBuilder>();
        private StringBuilder current;
        private string title = "";

        public string Title
        {
            get { return title; }
            set { title = value ?? ""; }
        }

        public int PageCount
        {
            get { return pages.Count; }
        }

        public void NewPage()
        {
            current = new StringBuilder();
            pages.Add(current);
        }

        public void SelectPage(int index)
        {
            current = pages[index];
        }

        public static string Clean(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            StringBuilder builder = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                if (c == '\u20B9')
                {
                    builder.Append("Rs.");
                }
                else if (c == '\r' || c == '\n' || c == '\t')
                {
                    builder.Append(' ');
                }
                else if (c < 32 || c > 126)
                {
                    builder.Append('?');
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

        public float Width(string text, float size, bool bold)
        {
            string clean = Clean(text);
            int[] widths = bold ? BoldWidths : RegularWidths;
            int total = 0;
            foreach (char c in clean)
            {
                total += widths[c - 32];
            }
            return total * size / 1000f;
        }

        public string Fit(string text, float maxWidth, float size, bool bold)
        {
            string clean = Clean(text);
            if (Width(clean, size, bold) <= maxWidth)
            {
                return clean;
            }
            while (clean.Length > 0 && Width(clean + "...", size, bold) > maxWidth)
            {
                clean = clean.Substring(0, clean.Length - 1);
            }
            return clean + "...";
        }

        public List<string> Wrap(string text, float maxWidth, float size, bool bold)
        {
            List<string> lines = new List<string>();
            string[] paragraphs = (text ?? "").Replace("\r\n", "\n").Split('\n');
            foreach (string paragraph in paragraphs)
            {
                string line = "";
                foreach (string word in Clean(paragraph).Split(' '))
                {
                    string candidate = line.Length == 0 ? word : line + " " + word;
                    if (Width(candidate, size, bold) <= maxWidth || line.Length == 0)
                    {
                        line = candidate;
                    }
                    else
                    {
                        lines.Add(line);
                        line = word;
                    }
                }
                lines.Add(line);
            }
            return lines;
        }

        public void Text(float x, float baseline, string text, float size, bool bold, Color color)
        {
            string clean = Clean(text);
            if (clean.Length == 0)
            {
                return;
            }
            current.Append("BT ").Append(bold ? "/F2 " : "/F1 ").Append(N(size)).Append(" Tf ")
                .Append(ColorOp(color, "rg")).Append(' ')
                .Append(N(x)).Append(' ').Append(N(pageHeight - baseline)).Append(" Td (")
                .Append(Escape(clean)).Append(") Tj ET\n");
        }

        public void TextRight(float right, float baseline, string text, float size, bool bold, Color color)
        {
            Text(right - Width(text, size, bold), baseline, text, size, bold, color);
        }

        public void TextCenter(float center, float baseline, string text, float size, bool bold, Color color)
        {
            Text(center - (Width(text, size, bold) / 2f), baseline, text, size, bold, color);
        }

        public void Line(float x1, float y1, float x2, float y2, float width, Color color)
        {
            current.Append(N(width)).Append(" w ").Append(ColorOp(color, "RG")).Append(' ')
                .Append(N(x1)).Append(' ').Append(N(pageHeight - y1)).Append(" m ")
                .Append(N(x2)).Append(' ').Append(N(pageHeight - y2)).Append(" l S\n");
        }

        public void FillRect(float x, float top, float width, float height, Color color)
        {
            current.Append(ColorOp(color, "rg")).Append(' ')
                .Append(N(x)).Append(' ').Append(N(pageHeight - top - height)).Append(' ')
                .Append(N(width)).Append(' ').Append(N(height)).Append(" re f\n");
        }

        public void StrokeRect(float x, float top, float width, float height, float lineWidth, Color color)
        {
            current.Append(N(lineWidth)).Append(" w ").Append(ColorOp(color, "RG")).Append(' ')
                .Append(N(x)).Append(' ').Append(N(pageHeight - top - height)).Append(' ')
                .Append(N(width)).Append(' ').Append(N(height)).Append(" re S\n");
        }

        public void Save(string path)
        {
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                byte[] bytes = Build();
                file.Write(bytes, 0, bytes.Length);
            }
        }

        private byte[] Build()
        {
            MemoryStream stream = new MemoryStream();
            List<long> offsets = new List<long>();
            int pageCount = pages.Count;
            int firstPageObject = 5;
            int infoObject = firstPageObject + (pageCount * 2);

            Write(stream, "%PDF-1.4\n%\xE2\xE3\xCF\xD3\n");

            offsets.Add(stream.Position);
            Write(stream, "1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");

            offsets.Add(stream.Position);
            StringBuilder kids = new StringBuilder();
            for (int i = 0; i < pageCount; i++)
            {
                kids.Append(firstPageObject + (i * 2)).Append(" 0 R ");
            }
            Write(stream, "2 0 obj\n<< /Type /Pages /Kids [" + kids.ToString() + "] /Count " + pageCount.ToString(CultureInfo.InvariantCulture) + " >>\nendobj\n");

            offsets.Add(stream.Position);
            Write(stream, "3 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>\nendobj\n");

            offsets.Add(stream.Position);
            Write(stream, "4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>\nendobj\n");

            for (int i = 0; i < pageCount; i++)
            {
                int pageObject = firstPageObject + (i * 2);
                int contentObject = pageObject + 1;
                byte[] content = Encoding.ASCII.GetBytes(pages[i].ToString());

                offsets.Add(stream.Position);
                Write(stream, pageObject.ToString(CultureInfo.InvariantCulture) + " 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 "
                    + N(pageWidth) + " " + N(pageHeight) + "] /Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents "
                    + contentObject.ToString(CultureInfo.InvariantCulture) + " 0 R >>\nendobj\n");

                offsets.Add(stream.Position);
                Write(stream, contentObject.ToString(CultureInfo.InvariantCulture) + " 0 obj\n<< /Length "
                    + content.Length.ToString(CultureInfo.InvariantCulture) + " >>\nstream\n");
                stream.Write(content, 0, content.Length);
                Write(stream, "\nendstream\nendobj\n");
            }

            offsets.Add(stream.Position);
            Write(stream, infoObject.ToString(CultureInfo.InvariantCulture) + " 0 obj\n<< /Title (" + Escape(Clean(title))
                + ") /Producer (RR Inventory) /CreationDate (D:" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + ") >>\nendobj\n");

            long xref = stream.Position;
            int size = offsets.Count + 1;
            StringBuilder table = new StringBuilder();
            table.Append("xref\n0 ").Append(size.ToString(CultureInfo.InvariantCulture)).Append("\n0000000000 65535 f \n");
            foreach (long offset in offsets)
            {
                table.Append(offset.ToString("0000000000", CultureInfo.InvariantCulture)).Append(" 00000 n \n");
            }
            table.Append("trailer\n<< /Size ").Append(size.ToString(CultureInfo.InvariantCulture))
                .Append(" /Root 1 0 R /Info ").Append(infoObject.ToString(CultureInfo.InvariantCulture))
                .Append(" 0 R >>\nstartxref\n").Append(xref.ToString(CultureInfo.InvariantCulture)).Append("\n%%EOF\n");
            Write(stream, table.ToString());
            return stream.ToArray();
        }

        private static void Write(Stream stream, string text)
        {
            byte[] bytes = Encoding.GetEncoding(28591).GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        private static string Escape(string text)
        {
            return text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }

        private static string ColorOp(Color color, string op)
        {
            return N(color.R / 255f) + " " + N(color.G / 255f) + " " + N(color.B / 255f) + " " + op;
        }

        private static string N(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }
    }
}
