using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Inventory
{
    public class DotMatrixPrintPreviewForm : Form
    {
        private const int GutterWidth = 5;
        private const int RulerLines = 2;
        private const int EM_GETSCROLLPOS = 0x04DD;
        private const int EM_SETSCROLLPOS = 0x04DE;

        private readonly string rawOutput;
        private readonly List<string> contentLines;
        private readonly int contentLineCount;
        private Font previewFont;
        private PreviewBox txtRuler;
        private PreviewBox txtPreview;
        private Label lblInfo;
        private bool syncingScroll;

        public static void ShowPreview(string rawPrintOutput)
        {
            using (DotMatrixPrintPreviewForm form = new DotMatrixPrintPreviewForm(rawPrintOutput))
            {
                Form owner = Form.ActiveForm;
                if (owner != null && owner.Visible && !owner.IsDisposed)
                {
                    form.ShowDialog(owner);
                }
                else
                {
                    form.ShowDialog();
                }
            }
        }

        public DotMatrixPrintPreviewForm(string rawPrintOutput)
        {
            rawOutput = rawPrintOutput ?? "";
            contentLines = SplitLines(HideControlCodes(rawOutput));
            contentLineCount = contentLines.Count;
            InitializeComponent();
            txtRuler.Text = BuildRulerText();
            txtPreview.Text = BuildBodyText(contentLines);
            UpdatePosition();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (txtPreview.Text.Length > 0)
            {
                txtPreview.SelectionStart = GutterWidth;
                txtPreview.SelectionLength = 0;
            }

            txtPreview.Focus();
            UpdatePosition();
            SyncRulerScroll();
        }

        private void InitializeComponent()
        {
            this.Text = "Dot Matrix Print Preview";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(980, 700);
            this.MinimumSize = new Size(720, 420);
            this.Font = new Font("Microsoft Sans Serif", 9F);
            this.KeyPreview = true;

            previewFont = CreatePreviewFont();

            Label lblNote = new Label();
            lblNote.Name = "lblNote";
            lblNote.Text = "Control codes are hidden here and stay in the text that Print sends. Paper output can differ because the printer interprets ESC/P commands such as condensed print and reverse feed.";
            lblNote.AutoSize = false;

            txtRuler = new PreviewBox();
            txtRuler.Font = previewFont;
            txtRuler.ReadOnly = true;
            txtRuler.WordWrap = false;
            txtRuler.BorderStyle = BorderStyle.Fixed3D;
            txtRuler.ScrollBars = RichTextBoxScrollBars.None;
            txtRuler.DetectUrls = false;
            txtRuler.TabStop = false;
            txtRuler.BackColor = Color.FromArgb(245, 245, 245);
            txtRuler.ShortcutsEnabled = false;

            txtPreview = new PreviewBox();
            txtPreview.Font = previewFont;
            txtPreview.ReadOnly = true;
            txtPreview.WordWrap = false;
            txtPreview.BorderStyle = BorderStyle.Fixed3D;
            txtPreview.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
            txtPreview.DetectUrls = false;
            txtPreview.HideSelection = false;
            txtPreview.BackColor = Color.White;
            txtPreview.AcceptsTab = false;
            txtPreview.HorizontalScrolled += new EventHandler(PreviewScrolled);
            txtPreview.VScroll += new EventHandler(PreviewScrolled);
            txtPreview.SelectionChanged += new EventHandler(PreviewSelectionChanged);

            lblInfo = new Label();
            lblInfo.AutoSize = false;
            lblInfo.TextAlign = ContentAlignment.MiddleLeft;

            Button btnPrint = new Button();
            btnPrint.Text = "Print";
            btnPrint.Size = new Size(84, 28);
            btnPrint.Click += new EventHandler(PrintClicked);

            Button btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Size = new Size(84, 28);
            btnClose.DialogResult = DialogResult.Cancel;

            this.CancelButton = btnClose;
            this.Controls.Add(lblNote);
            this.Controls.Add(txtRuler);
            this.Controls.Add(txtPreview);
            this.Controls.Add(lblInfo);
            this.Controls.Add(btnPrint);
            this.Controls.Add(btnClose);

            this.Resize += new EventHandler(FormResize);
            LayoutControls();
        }

        private void FormResize(object sender, EventArgs e)
        {
            LayoutControls();
            SyncRulerScroll();
        }

        private void LayoutControls()
        {
            int margin = 12;
            int noteHeight = 36;
            int rulerHeight = previewFont.Height * RulerLines + 12;
            int buttonTop = this.ClientSize.Height - margin - 28;
            int infoWidth = Math.Max(200, this.ClientSize.Width - margin * 2 - 180);

            Label lblNote = (Label)this.Controls["lblNote"];
            lblNote.SetBounds(margin, 8, this.ClientSize.Width - margin * 2, noteHeight);

            int top = 8 + noteHeight + 6;
            int scrollWidth = SystemInformation.VerticalScrollBarWidth;
            txtRuler.SetBounds(margin, top, Math.Max(80, this.ClientSize.Width - margin * 2 - scrollWidth), rulerHeight);
            int bodyTop = top + rulerHeight;
            int bodyHeight = buttonTop - 8 - bodyTop;
            if (bodyHeight < 80)
            {
                bodyHeight = 80;
            }

            txtPreview.SetBounds(margin, bodyTop, this.ClientSize.Width - margin * 2, bodyHeight);
            lblInfo.SetBounds(margin, buttonTop, infoWidth, 28);

            Button btnClose = (Button)this.CancelButton;
            btnClose.Location = new Point(this.ClientSize.Width - margin - btnClose.Width, buttonTop);
            foreach (Control control in this.Controls)
            {
                Button printButton = control as Button;
                if (printButton != null && printButton.Text == "Print")
                {
                    printButton.Location = new Point(btnClose.Left - 8 - printButton.Width, buttonTop);
                }
            }
        }

        private void PrintClicked(object sender, EventArgs e)
        {
            DotMatrixPrinter.Print(rawOutput);
        }

        private void PreviewSelectionChanged(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        private void PreviewScrolled(object sender, EventArgs e)
        {
            SyncRulerScroll();
        }

        private void UpdatePosition()
        {
            int line = txtPreview.GetLineFromCharIndex(txtPreview.SelectionStart);
            int lineStart = txtPreview.GetFirstCharIndexFromLine(line);
            int displayColumn = txtPreview.SelectionStart - lineStart;
            int column = displayColumn - GutterWidth + 1;
            if (column < 1)
            {
                column = 1;
            }

            int length = 0;
            if (line >= 0 && line < contentLines.Count)
            {
                length = contentLines[line].Length;
            }

            lblInfo.Text = "Line " + (line + 1).ToString()
                + " of " + contentLineCount.ToString()
                + "    Column " + column.ToString()
                + "    Line length " + length.ToString();
        }

        private void SyncRulerScroll()
        {
            if (syncingScroll || txtPreview == null || txtRuler == null)
            {
                return;
            }

            if (!txtPreview.IsHandleCreated || !txtRuler.IsHandleCreated)
            {
                return;
            }

            syncingScroll = true;
            try
            {
                POINT pos = new POINT();
                SendMessage(txtPreview.Handle, EM_GETSCROLLPOS, IntPtr.Zero, ref pos);
                pos.y = 0;
                SendMessage(txtRuler.Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref pos);
            }
            finally
            {
                syncingScroll = false;
            }
        }

        private static string BuildRulerText()
        {
            return new string(' ', 4) + "|" + TensRuler() + "\r\n"
                + new string(' ', 4) + "|" + UnitsRuler();
        }

        private static string BuildBodyText(List<string> lines)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lines.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append("\r\n");
                }

                sb.Append((i + 1).ToString().PadLeft(4));
                sb.Append('|');
                sb.Append(lines[i]);
            }

            return sb.ToString();
        }

        internal static string HideControlCodes(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return "";
            }

            StringBuilder sb = new StringBuilder(raw.Length);
            for (int i = 0; i < raw.Length; i++)
            {
                char c = raw[i];
                if (c == '\r' || c == '\n' || c == '\t' || c == ' ')
                {
                    sb.Append(c);
                    continue;
                }

                if (c == (char)0x0F || c == (char)0x12)
                {
                    continue;
                }

                if (c == (char)0x1B)
                {
                    i = SkipEscCommand(raw, i);
                    continue;
                }

                if (char.IsControl(c))
                {
                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        private static int SkipEscCommand(string raw, int escIndex)
        {
            int commandIndex = escIndex + 1;
            if (commandIndex >= raw.Length)
            {
                return escIndex;
            }

            char command = raw[commandIndex];
            if (command == '\r' || command == '\n')
            {
                return escIndex;
            }

            if (TakesOneParameter(command))
            {
                int paramIndex = commandIndex + 1;
                if (paramIndex < raw.Length && raw[paramIndex] != '\r' && raw[paramIndex] != '\n')
                {
                    return paramIndex;
                }
            }

            return commandIndex;
        }

        private static bool TakesOneParameter(char command)
        {
            switch (command)
            {
                case '!':
                case '-':
                case '3':
                case 'A':
                case 'C':
                case 'J':
                case 'Q':
                case 'S':
                case 'U':
                case 'W':
                case 'a':
                case 'j':
                case 'l':
                case 'p':
                case 'x':
                    return true;
                default:
                    return false;
            }
        }

        private static List<string> SplitLines(string text)
        {
            List<string> lines = new List<string>();
            if (text == null)
            {
                text = "";
            }

            int start = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != '\r' && text[i] != '\n')
                {
                    continue;
                }

                lines.Add(text.Substring(start, i - start));
                if (text[i] == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
                {
                    i++;
                }

                start = i + 1;
            }

            if (start < text.Length || text.Length == 0)
            {
                lines.Add(text.Substring(start));
            }

            return lines;
        }

        private static string TensRuler()
        {
            StringBuilder sb = new StringBuilder(80);
            for (int n = 1; n <= 8; n++)
            {
                sb.Append(' ', 9);
                sb.Append((char)('0' + n));
            }

            return sb.ToString();
        }

        private static string UnitsRuler()
        {
            StringBuilder sb = new StringBuilder(80);
            for (int n = 0; n < 8; n++)
            {
                sb.Append("1234567890");
            }

            return sb.ToString();
        }

        private static Font CreatePreviewFont()
        {
            Font font = new Font("Consolas", 11F, FontStyle.Regular, GraphicsUnit.Point);
            if (string.Compare(font.Name, "Consolas", true) != 0)
            {
                font.Dispose();
                font = new Font("Courier New", 11F, FontStyle.Regular, GraphicsUnit.Point);
            }

            return font;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && previewFont != null)
            {
                previewFont.Dispose();
                previewFont = null;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref POINT lParam);

        private class PreviewBox : RichTextBox
        {
            public event EventHandler HorizontalScrolled;

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (m.Msg == 0x0114 && HorizontalScrolled != null)
                {
                    HorizontalScrolled(this, EventArgs.Empty);
                }
            }
        }
    }
}
