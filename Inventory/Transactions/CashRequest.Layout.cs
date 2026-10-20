using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Transactions
{
    public partial class CashRequest
    {
        private void InitializeCompactCashRequestLayout()
        {
            MinimumSize = new Size(640, 400);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Margin = Padding.Empty;
            splitContainer1.Panel2.AutoScrollMinSize = Size.Empty;
            pnlCollapse2.MinimumSize = Size.Empty;
            panel2.AutoScroll = true;
            panel1.Height = 28;
            tabPage1.Resize += CompactCashPopup_Resize;
            tabPage2.Resize += CompactCashPopup_Resize;
        }

        private void CompactCashPopup_Resize(object sender, EventArgs e)
        {
            if (a1Panel2.Visible && !arrangingLayout) LayoutCashPopup();
        }

        private void CompactPaymentSection()
        {
            if (paymentModeHost == null || arrangingLayout) return;
            arrangingLayout = true;
            panel2.SuspendLayout();
            try
            {
                int width = Math.Max(560, panel2.ClientSize.Width - (panel2.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0));
                bool wide = width >= 780;
                // Coordinates are in the scrollable content, not the viewport.
                Point scroll = panel2.AutoScrollPosition;
                panel2.AutoScrollPosition = Point.Empty;
                foreach (Control control in new Control[] { label2, label27, comboBox1, AddMainHeader, label12, ddlAccountHead, btnadd,
                    label7, txtdate, label9, txtamount, label10, txtreson, button2, label3, txtLoginBy, BtnUpload,
                    paymentModeHost, a1Panel3, btnsave, dgvCashRequest }) control.Anchor = AnchorStyles.Top | AnchorStyles.Left;

                label2.SetBounds(12, 2, 180, 26);
                label3.Location = new Point(width - 245, 8);
                txtLoginBy.SetBounds(width - 190, 4, 110, 22);
                BtnUpload.SetBounds(width - 74, 3, 68, 25);
                label27.Text = "Main Header";
                label27.Location = new Point(16, 38);
                comboBox1.SetBounds(120, 34, wide ? 220 : Math.Min(300, width - 170), 23);
                AddMainHeader.SetBounds(comboBox1.Right + 4, 34, 24, 24);
                int accountY = wide ? 34 : 63;
                int accountX = wide ? Math.Max(390, width / 2) : 16;
                label12.Location = new Point(accountX, accountY + 4);
                ddlAccountHead.SetBounds(accountX + 104, accountY, Math.Max(140, wide ? width - accountX - 144 : Math.Min(300, width - 170)), 23);
                btnadd.SetBounds(ddlAccountHead.Right + 4, accountY, 24, 24);
                int dateY = accountY + 29;
                label7.Location = new Point(16, dateY + 3);
                txtdate.SetBounds(120, dateY, 180, 23);
                label9.Location = new Point(330, dateY + 3);
                txtamount.SetBounds(395, dateY, Math.Min(180, width - 411), 23);
                int reasonY = dateY + 29;
                label10.Location = new Point(16, reasonY + 4);
                txtreson.SetBounds(120, reasonY, width - 166, 36);
                txtreson.ScrollBars = ScrollBars.Vertical;
                button2.SetBounds(width - 36, reasonY, 26, 26);
                // Required indicators follow their fields instead of old fixed rows.
                label5.Location = new Point(accountX - 12, accountY + 1);
                label13.Location = new Point(2, dateY);
                label51.Location = new Point(383, dateY);
                label15.Location = new Point(2, reasonY);
                foreach (Label required in new Label[] { label5, label13, label51, label15 })
                { required.AutoSize = false; required.Size = new Size(12, 23); }
                label8.Visible = false;
                label26.Location = new Point(16, txtreson.Bottom + 11);
                paymentModeHost.SetBounds(140, txtreson.Bottom + 5, width - 156, 28);
                RadioButton[] modes = { rbCash, rbUpi, radioButton1, rdcheque };
                int[] positions = { 4, 85, 155, 270 };
                for (int i = 0; i < modes.Length; i++) { modes[i].AutoSize = true; modes[i].Location = new Point(positions[i], 4); }

                a1Panel3.Location = new Point(12, paymentModeHost.Bottom + 3);
                a1Panel3.Width = width - 24;
                // Reflow visible mode fields when resizing across the column breakpoint.
                int index = 0;
                foreach (Control editor in new Control[] { txtamountrupe, rbUpi != null && rbUpi.Checked ? (Control)txtUpiReference : txtamount1,
                    rbUpi != null && rbUpi.Checked ? (Control)lblUpiDateValue : txtChqDate, txtbankname, txtfromaccount, txttoaccount })
                {
                    if (editor == null || !editor.Visible) continue;
                    Label caption = editor == txtamountrupe ? label25 : editor == txtUpiReference || editor == txtamount1 ? label24 : editor == lblUpiDateValue || editor == txtChqDate ? label23 : editor == txtbankname ? label19 : editor == txtfromaccount ? (radioButton1.Checked ? label19 : label18) : (Label)a1Panel3.Controls["lblChequeToAccount"];
                    if (radioButton1.Checked && editor == txttoaccount) caption = label18;
                    if (caption != null) CompactModeField(caption, caption.Text, index++, editor, width);
                }
                if (a1Panel3.Visible) a1Panel3.Height = 8 + ((index + (wide ? 1 : 0)) / (wide ? 2 : 1)) * 28;
                int saveTop = (a1Panel3.Visible ? a1Panel3.Bottom : paymentModeHost.Bottom) + 5;
                btnsave.SetBounds(120, saveTop, 210, 30);
                btnsave.BringToFront();
                int gridTop = btnsave.Bottom + 6;
                dgvCashRequest.SetBounds(12, gridTop, width - 24, Math.Max(100, panel2.ClientSize.Height - gridTop - 8));
                dgvCashRequest.ScrollBars = ScrollBars.Both;
                panel2.AutoScrollMinSize = new Size(560, gridTop + 108);
                paymentModeHost.BringToFront();
                EnsurePaymentModeTabStops();
                panel2.AutoScrollPosition = new Point(-scroll.X, -scroll.Y);
            }
            finally { panel2.ResumeLayout(); arrangingLayout = false; }
        }

        private void CompactModeField(Label caption, string text, int index, Control editor, int width)
        {
            int columns = width >= 780 ? 2 : 1;
            int cellWidth = (width - 32) / columns;
            int x = 8 + index % columns * cellWidth;
            int y = 4 + index / columns * 28;
            caption.Visible = true; caption.Text = text; caption.Location = new Point(x, y + 3);
            editor.Visible = true; editor.SetBounds(x + 112, y, Math.Max(120, cellWidth - 128), 23);
        }

        private void CompactCashPopup()
        {
            if (dgvBalanceReceived == null || arrangingLayout) return;
            arrangingLayout = true;
            try
            {
                Control host = a1Panel2.Parent ?? tabPage1;
                int width = Math.Max(300, host.ClientSize.Width - 12);
                int height = Math.Max(240, host.ClientSize.Height - 12);
                a1Panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                a1Panel2.SetBounds(6, 6, width, height);
                button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                button3.SetBounds(width - 30, 2, 24, 25);
                panel15.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                panel15.SetBounds(4, 30, width - 8, height - 74);
                int half = (panel15.Width - 24) / 2;
                int gridHeight = Math.Max(40, panel15.Height - 110);
                lblCashGivenTitle.Location = new Point(8, 2);
                lblBalanceReceivedTitle.Location = new Point(half + 16, 2);
                dgvDenomination.Anchor = dgvBalanceReceived.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                dgvDenomination.SetBounds(8, 24, half, gridHeight);
                dgvBalanceReceived.SetBounds(half + 16, 24, half, gridHeight);
                foreach (DataGridView grid in new DataGridView[] { dgvDenomination, dgvBalanceReceived })
                {
                    grid.ScrollBars = ScrollBars.Both;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                    grid.ColumnHeadersHeight = 26;
                    grid.ColumnHeadersDefaultCellStyle.Font = Font;
                    grid.DefaultCellStyle.Font = Font;
                    grid.RowTemplate.Height = 23;
                    foreach (DataGridViewRow row in grid.Rows) row.Height = 23;
                    foreach (DataGridViewColumn column in grid.Columns) { column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; column.MinimumWidth = 55; }
                }
                int summaryTop = dgvDenomination.Bottom + 4;
                Label[] summaries = { lblAmountToPayValue, lblCashGivenValue, lblBalanceReceivedValue, lblNetCashPaidValue, lblCashDifferenceValue };
                for (int i = 0; i < summaries.Length; i++)
                {
                    Label label = summaries[i];
                    label.AutoSize = false;
                    label.Font = lblCashGivenTitle.Font;
                    label.AutoEllipsis = true;
                    label.SetBounds(i == 2 || i == 3 ? half + 16 : 8, summaryTop + (i == 1 || i == 3 ? 24 : i == 4 ? 48 : 0), half, 23);
                }
                btnsave.Parent = a1Panel2;
                btnsave.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                btnsave.SetBounds(Math.Max(8, width - 224), height - 38, 210, 30);
                btnsave.Visible = true;
                btnsave.BringToFront();
            }
            finally { arrangingLayout = false; }
        }
    }
}
