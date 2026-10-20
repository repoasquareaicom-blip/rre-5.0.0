using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Inventory.Tool
{
    public class DayCloseV2 : Form
    {
        private DateTimePicker dtpBusinessDate;
        private Button btnLoad;
        private Button btnPdf;
        private Label lblStatus;
        private TextBox txtOpening;
        private Button btnUseDefault;
        private Label lblOpeningDefault;
        private DataGridView dgvSummary;
        private ComboBox cmbLedger;
        private DataGridView dgvLedger;
        private Label lblLedgerTotals;
        private Label lblCashBridge;
        private DataGridView dgvCash;
        private Label lblDenomTotals;
        private Label lblOpeningCheck;
        private TextBox txtUpiActual;
        private TextBox txtCardActual;
        private Label lblUpiCard;
        private DataGridView dgvGoodsCategory;
        private ComboBox cmbGoodsCategory;
        private ComboBox cmbGoodsMove;
        private TextBox txtGoodsSearch;
        private DataGridView dgvGoods;
        private Label lblGoodsTotals;
        private Label lblFinal;
        private TextBox txtRemarks;
        private Button btnSubmit;
        private DataTable goodsProducts;
        private DataTable goodsCategories;
        private readonly Dictionary<int, int> defaultOpeningCounts = new Dictionary<int, int>();
        private bool hasPreviousClose;
        private bool isClosed;
        private bool filling;
        private DateTime loadedDate;
        private decimal defaultOpeningCash;
        private decimal openingCash;
        private decimal cashIn;
        private decimal cashOut;
        private decimal expectedCash;
        private decimal upiIn;
        private decimal upiOut;
        private decimal expectedUpi;
        private decimal cardIn;
        private decimal cardOut;
        private decimal expectedCard;
        private decimal cashCarriedIn;
        private decimal cashCarriedOut;
        private decimal upiCarriedIn;
        private decimal upiCarriedOut;
        private decimal cardCarriedIn;
        private decimal cardCarriedOut;
        private Panel page;
        private Label lblPostCloseCaption;
        private DataGridView dgvPostClose;
        private Label lblPostCloseTotals;
        private bool postCloseShown;
        private const int PostCloseTop = 668;
        private const int PostCloseHeight = 250;
        private readonly bool historyMode;
        private Button btnHistory;
        private Label lblHistoryBanner;
        private Label lblDraftSaved;
        private Timer draftTimer;
        private bool hasDraft;
        private string draftInfo = "";
        private string savedDraftSignature;
        private bool draftFailureShown;
        private const int DraftDelayMs = 3000;
        private bool ledgerStale;

        public DayCloseV2()
            : this(DateTime.Today, false)
        {
        }

        public DayCloseV2(DateTime businessDate, bool viewOnly)
        {
            historyMode = viewOnly;
            Text = historyMode ? "Day Close V2 - History View (Read Only) - " + businessDate.ToString("dd-MM-yyyy") : "Day Close V2";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            Font = new Font("Segoe UI", 9F);
            BuildLayout();
            dtpBusinessDate.Value = businessDate.Date;
            loadedDate = DateTime.MinValue;
            LoadExpected();
        }

        private void BuildLayout()
        {
            Panel scroller = new Panel();
            scroller.Dock = DockStyle.Fill;
            scroller.AutoScroll = true;
            Controls.Add(scroller);

            page = new Panel();
            page.Location = new Point(0, 0);
            page.Size = new Size(980, 2110);
            scroller.Controls.Add(page);

            int y = 12;
            Caption(page, "DAILY PAYMENT RECONCILIATION", 16, y, 16, true);
            y = 48;
            Caption(page, "Business Date", 16, y, 10, false);
            dtpBusinessDate = new DateTimePicker();
            dtpBusinessDate.Format = DateTimePickerFormat.Custom;
            dtpBusinessDate.CustomFormat = "dd-MM-yyyy";
            dtpBusinessDate.Location = new Point(130, y - 4);
            dtpBusinessDate.Width = 120;
            page.Controls.Add(dtpBusinessDate);
            btnLoad = new Button();
            btnLoad.Text = "Load";
            btnLoad.Location = new Point(260, y - 6);
            btnLoad.Size = new Size(80, 28);
            btnLoad.Click += new EventHandler(btnLoad_Click);
            page.Controls.Add(btnLoad);
            lblStatus = Caption(page, "Status : OPEN", 360, y, 12, true);
            btnPdf = new Button();
            btnPdf.Text = "Generate PDF";
            btnPdf.Location = new Point(560, y - 6);
            btnPdf.Size = new Size(180, 28);
            btnPdf.Click += new EventHandler(btnPdf_Click);
            page.Controls.Add(btnPdf);
            btnHistory = new Button();
            btnHistory.Text = "Day Close History";
            btnHistory.Location = new Point(750, y - 6);
            btnHistory.Size = new Size(150, 28);
            btnHistory.Click += new EventHandler(btnHistory_Click);
            page.Controls.Add(btnHistory);
            lblHistoryBanner = Caption(page, "HISTORY VIEW - READ ONLY", 560, 12, 12, true);
            lblHistoryBanner.ForeColor = Color.DarkBlue;
            lblHistoryBanner.Visible = false;
            lblDraftSaved = Caption(page, "", 560, 16, 9, false);
            lblDraftSaved.ForeColor = Color.DimGray;
            draftTimer = new Timer();
            draftTimer.Interval = DraftDelayMs;
            draftTimer.Tick += new EventHandler(draftTimer_Tick);

            y = 88;
            Caption(page, "Opening Cash", 16, y, 10, false);
            txtOpening = new TextBox();
            txtOpening.Location = new Point(130, y - 4);
            txtOpening.Width = 120;
            txtOpening.TextChanged += new EventHandler(txtOpening_TextChanged);
            txtOpening.TextChanged += new EventHandler(draftField_TextChanged);
            txtOpening.Leave += new EventHandler(draftField_Leave);
            txtOpening.Leave += new EventHandler(txtOpening_Leave);
            page.Controls.Add(txtOpening);
            btnUseDefault = new Button();
            btnUseDefault.Text = "Use Previous Close";
            btnUseDefault.Location = new Point(260, y - 6);
            btnUseDefault.Size = new Size(150, 28);
            btnUseDefault.Click += new EventHandler(btnUseDefault_Click);
            page.Controls.Add(btnUseDefault);
            lblOpeningDefault = Caption(page, "", 420, y, 9, false);

            y = 128;
            Caption(page, "PAYMENT SUMMARY", 16, y, 12, true);
            y = 156;
            dgvSummary = Grid(page, 16, y, 940, 130);
            dgvSummary.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvSummary_CellFormatting);

            y = 300;
            Caption(page, "CASH / PAYMENT TRANSACTION LEDGER", 16, y, 12, true);
            y = 328;
            cmbLedger = new ComboBox();
            cmbLedger.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLedger.Location = new Point(16, y);
            cmbLedger.Width = 180;
            cmbLedger.Items.AddRange(new object[] { "All", "CASH IN", "CASH OUT", "UPI IN", "UPI OUT", "CARD IN", "CARD OUT" });
            cmbLedger.SelectedIndex = 0;
            cmbLedger.SelectedIndexChanged += new EventHandler(cmbLedger_SelectedIndexChanged);
            page.Controls.Add(cmbLedger);
            y = 360;
            dgvLedger = Grid(page, 16, y, 940, 220);
            dgvLedger.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvLedger.ShowCellToolTips = true;
            dgvLedger.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvLedger_CellFormatting);
            y = 590;
            lblLedgerTotals = Caption(page, "", 16, y, 10, false);
            lblLedgerTotals.Size = new Size(940, 72);
            y = 668;
            lblCashBridge = Caption(page, "", 16, y, 10, true);
            lblCashBridge.Size = new Size(940, 88);

            y = 768;
            Caption(page, "PHYSICAL CASH RECONCILIATION", 16, y, 12, true);
            y = 796;
            dgvCash = Grid(page, 16, y, 940, 280);
            dgvCash.ReadOnly = false;
            dgvCash.CellEndEdit += new DataGridViewCellEventHandler(dgvCash_CellEndEdit);
            BuildCashRows();
            y = 1084;
            lblDenomTotals = Caption(page, "", 16, y, 11, true);
            lblDenomTotals.AutoSize = false;
            lblDenomTotals.Size = new Size(940, 24);
            y = 1110;
            lblOpeningCheck = Caption(page, "", 16, y, 10, true);
            lblOpeningCheck.AutoSize = false;
            lblOpeningCheck.Size = new Size(940, 24);

            y = 1140;
            Caption(page, "UPI / CARD RECONCILIATION", 16, y, 12, true);
            y = 1168;
            Caption(page, "Actual UPI", 16, y, 10, false);
            txtUpiActual = new TextBox();
            txtUpiActual.Location = new Point(110, y - 4);
            txtUpiActual.Width = 120;
            txtUpiActual.Text = "0.00";
            txtUpiActual.TextChanged += new EventHandler(txtActual_TextChanged);
            txtUpiActual.TextChanged += new EventHandler(draftField_TextChanged);
            txtUpiActual.Leave += new EventHandler(draftField_Leave);
            page.Controls.Add(txtUpiActual);
            Caption(page, "Actual Card", 260, y, 10, false);
            txtCardActual = new TextBox();
            txtCardActual.Location = new Point(360, y - 4);
            txtCardActual.Width = 120;
            txtCardActual.Text = "0.00";
            txtCardActual.TextChanged += new EventHandler(txtActual_TextChanged);
            txtCardActual.TextChanged += new EventHandler(draftField_TextChanged);
            txtCardActual.Leave += new EventHandler(draftField_Leave);
            page.Controls.Add(txtCardActual);
            y = 1204;
            lblUpiCard = Caption(page, "", 16, y, 10, false);
            lblUpiCard.Size = new Size(940, 48);

            y = 1264;
            Caption(page, "GOODS MOVEMENT SUMMARY", 16, y, 12, true);
            y = 1292;
            Caption(page, "By category (click a row to filter the product list)", 16, y, 9, false);
            dgvGoodsCategory = Grid(page, 16, y + 20, 940, 130);
            dgvGoodsCategory.CellClick += new DataGridViewCellEventHandler(dgvGoodsCategory_CellClick);
            y = 1454;
            Caption(page, "Category", 16, y, 9, false);
            cmbGoodsCategory = new ComboBox();
            cmbGoodsCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGoodsCategory.Location = new Point(80, y - 4);
            cmbGoodsCategory.Width = 220;
            cmbGoodsCategory.Items.Add("All");
            cmbGoodsCategory.SelectedIndex = 0;
            cmbGoodsCategory.SelectedIndexChanged += new EventHandler(goodsFilter_Changed);
            page.Controls.Add(cmbGoodsCategory);
            Caption(page, "Movement", 320, y, 9, false);
            cmbGoodsMove = new ComboBox();
            cmbGoodsMove.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGoodsMove.Location = new Point(390, y - 4);
            cmbGoodsMove.Width = 100;
            cmbGoodsMove.Items.AddRange(new object[] { "All", "IN", "OUT" });
            cmbGoodsMove.SelectedIndex = 0;
            cmbGoodsMove.SelectedIndexChanged += new EventHandler(goodsFilter_Changed);
            page.Controls.Add(cmbGoodsMove);
            Caption(page, "Product", 510, y, 9, false);
            txtGoodsSearch = new TextBox();
            txtGoodsSearch.Location = new Point(570, y - 4);
            txtGoodsSearch.Width = 220;
            txtGoodsSearch.TextChanged += new EventHandler(goodsFilter_Changed);
            page.Controls.Add(txtGoodsSearch);
            Caption(page, "Double-click a product to see its transactions", 800, y, 8, false);
            y = 1482;
            dgvGoods = Grid(page, 16, y, 940, 240);
            dgvGoods.CellDoubleClick += new DataGridViewCellEventHandler(dgvGoods_CellDoubleClick);
            y = 1730;
            lblGoodsTotals = Caption(page, "", 16, y, 10, false);
            lblGoodsTotals.Size = new Size(940, 40);

            y = 1780;
            Caption(page, "FINAL RECONCILIATION", 16, y, 12, true);
            y = 1808;
            lblFinal = Caption(page, "", 16, y, 11, true);
            lblFinal.Size = new Size(940, 120);
            y = 1938;
            Caption(page, "Variance Reason", 16, y, 10, false);
            txtRemarks = new TextBox();
            txtRemarks.Location = new Point(16, y + 24);
            txtRemarks.Size = new Size(640, 72);
            txtRemarks.Multiline = true;
            txtRemarks.MaxLength = 500;
            txtRemarks.TextChanged += new EventHandler(draftField_TextChanged);
            txtRemarks.Leave += new EventHandler(draftField_Leave);
            page.Controls.Add(txtRemarks);
            btnSubmit = new Button();
            btnSubmit.Text = "Submit Day Close";
            btnSubmit.Location = new Point(16, y + 108);
            btnSubmit.Size = new Size(180, 34);
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
            page.Controls.Add(btnSubmit);

            lblPostCloseCaption = Caption(page, "POST-CLOSE TRANSACTIONS  (Not included in submitted Day Close totals)", 16, PostCloseTop, 12, true);
            lblPostCloseCaption.ForeColor = Color.DarkRed;
            dgvPostClose = Grid(page, 16, PostCloseTop + 28, 940, 150);
            dgvPostClose.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvPostClose.ShowCellToolTips = true;
            lblPostCloseTotals = Caption(page, "", 16, PostCloseTop + 184, 10, false);
            lblPostCloseTotals.AutoSize = false;
            lblPostCloseTotals.Size = new Size(940, 56);
            lblPostCloseCaption.Visible = false;
            dgvPostClose.Visible = false;
            lblPostCloseTotals.Visible = false;
        }

        private void ShowPostCloseSection(bool show)
        {
            if (show == postCloseShown)
            {
                return;
            }
            int delta = show ? PostCloseHeight : -PostCloseHeight;
            foreach (Control control in page.Controls)
            {
                if (control != lblPostCloseCaption && control != dgvPostClose && control != lblPostCloseTotals && control.Top >= PostCloseTop)
                {
                    control.Top += delta;
                }
            }
            page.Height += delta;
            lblPostCloseCaption.Visible = show;
            dgvPostClose.Visible = show;
            lblPostCloseTotals.Visible = show;
            postCloseShown = show;
        }

        private void LoadPostClose()
        {
            if (isClosed == false)
            {
                dgvPostClose.DataSource = null;
                lblPostCloseTotals.Text = "";
                ShowPostCloseSection(false);
                return;
            }
            DataTable table = Query("dbo.Proc_DayCloseV2_GetPostCloseTransactions");
            dgvPostClose.DataSource = table;
            FormatLedgerColumns(dgvPostClose);
            decimal[] totals = new decimal[6];
            string[] channels = new string[] { "CASH", "UPI", "CARD" };
            bool[] present = new bool[3];
            foreach (DataRow row in table.Rows)
            {
                int index = Array.IndexOf(channels, Convert.ToString(row["Channel"]));
                if (index < 0)
                {
                    continue;
                }
                present[index] = true;
                totals[index * 2] += Convert.ToDecimal(row["AmountIn"]);
                totals[(index * 2) + 1] += Convert.ToDecimal(row["AmountOut"]);
            }
            List<string> lines = new List<string>();
            string[] names = new string[] { "Cash", "UPI", "Card" };
            for (int i = 0; i < 3; i++)
            {
                if (present[i])
                {
                    decimal totalIn = totals[i * 2];
                    decimal totalOut = totals[(i * 2) + 1];
                    lines.Add("Post-close " + names[i] + "   IN  " + totalIn.ToString("N2") + "      OUT  " + totalOut.ToString("N2")
                        + "      NET  " + Signed(totalIn - totalOut));
                }
            }
            lblPostCloseTotals.Text = lines.Count == 0
                ? "No transactions were posted to this business date after the Day Close was submitted."
                : string.Join(Environment.NewLine, lines.ToArray());
            ShowPostCloseSection(true);
        }

        private Label Caption(Control parent, string text, int x, int y, float size, bool bold)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular);
            label.Location = new Point(x, y);
            label.AutoSize = true;
            parent.Controls.Add(label);
            return label;
        }

        private DataGridView Grid(Control parent, int x, int y, int width, int height)
        {
            DataGridView grid = new DataGridView();
            grid.Location = new Point(x, y);
            grid.Size = new Size(width, height);
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            parent.Controls.Add(grid);
            return grid;
        }

        private void BuildCashRows()
        {
            dgvCash.Columns.Clear();
            dgvCash.Columns.Add("Denomination", "Denomination");
            dgvCash.Columns.Add("Opening", "Opening");
            dgvCash.Columns.Add("IN", "IN");
            dgvCash.Columns.Add("OUT", "OUT");
            dgvCash.Columns.Add("Expected", "Expected");
            dgvCash.Columns.Add("Physical", "Physical");
            dgvCash.Columns.Add("Difference", "Difference");
            dgvCash.Columns[0].ReadOnly = true;
            dgvCash.Columns[2].ReadOnly = true;
            dgvCash.Columns[3].ReadOnly = true;
            dgvCash.Columns[4].ReadOnly = true;
            dgvCash.Columns[6].ReadOnly = true;
            int[] faces = new int[] { 2000, 1000, 500, 200, 100, 50, 20, 10, 5, 2, 1 };
            for (int i = 0; i < faces.Length; i++)
            {
                dgvCash.Rows.Add(faces[i].ToString(), "0", "0", "0", "0", "0", "0");
            }
            dgvCash.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvCash_CellFormatting);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (SaveDraft() == false && ConfirmDiscardDraft() == false)
            {
                return;
            }
            LoadExpected();
        }

        private void LoadExpected()
        {
            bool dateChanged = loadedDate != dtpBusinessDate.Value.Date;
            DataTable table = Query("dbo.Proc_DayCloseV2_GetExpectedSummary");
            if (table.Rows.Count == 0)
            {
                return;
            }
            DataRow row = table.Rows[0];
            filling = true;
            hasPreviousClose = Convert.ToInt32(row["HasPreviousClose"]) == 1;
            isClosed = Convert.ToInt32(row["IsClosed"]) == 1;
            openingCash = Convert.ToDecimal(row["OpeningCash"]);
            cashIn = Convert.ToDecimal(row["CashIn"]);
            cashOut = Convert.ToDecimal(row["CashOut"]);
            upiIn = Convert.ToDecimal(row["UpiIn"]);
            upiOut = Convert.ToDecimal(row["UpiOut"]);
            expectedUpi = Convert.ToDecimal(row["ExpectedUpi"]);
            cardIn = Convert.ToDecimal(row["CardIn"]);
            cardOut = Convert.ToDecimal(row["CardOut"]);
            expectedCard = Convert.ToDecimal(row["ExpectedCard"]);
            expectedCash = Convert.ToDecimal(row["ExpectedCash"]);
            cashCarriedIn = OptionalAmount(row, "CashCarriedIn");
            cashCarriedOut = OptionalAmount(row, "CashCarriedOut");
            upiCarriedIn = OptionalAmount(row, "UpiCarriedIn");
            upiCarriedOut = OptionalAmount(row, "UpiCarriedOut");
            cardCarriedIn = OptionalAmount(row, "CardCarriedIn");
            cardCarriedOut = OptionalAmount(row, "CardCarriedOut");
            defaultOpeningCash = hasPreviousClose && isClosed == false ? openingCash : 0m;
            if (isClosed)
            {
                txtOpening.Text = openingCash.ToString("0.00");
                txtUpiActual.Text = Convert.ToDecimal(row["ActualUpiNet"]).ToString("0.00");
                txtCardActual.Text = Convert.ToDecimal(row["ActualCardNet"]).ToString("0.00");
                txtRemarks.Text = Convert.ToString(row["Remarks"]);
                lblOpeningDefault.Text = "Saved opening for this close";
            }
            else
            {
                if (dateChanged)
                {
                    txtOpening.Text = hasPreviousClose ? defaultOpeningCash.ToString("0.00") : "";
                    txtUpiActual.Text = "0.00";
                    txtCardActual.Text = "0.00";
                    txtRemarks.Text = "";
                    ClearPhysical();
                }
                ApplyTypedOpening();
                lblOpeningDefault.Text = hasPreviousClose
                    ? "Default " + defaultOpeningCash.ToString("N2") + " = previous actual closing (editable)"
                    : "First Day Close: enter opening cash and opening counts";
            }
            txtOpening.ReadOnly = isClosed;
            btnUseDefault.Visible = isClosed == false && hasPreviousClose;
            txtUpiActual.ReadOnly = isClosed;
            txtCardActual.ReadOnly = isClosed;
            txtRemarks.ReadOnly = isClosed;
            btnSubmit.Enabled = isClosed == false;
            dgvCash.Columns[1].ReadOnly = isClosed;
            dgvCash.Columns[5].ReadOnly = isClosed;
            filling = false;
            loadedDate = dtpBusinessDate.Value.Date;
            LoadDenomination(dateChanged || isClosed);
            if (isClosed)
            {
                draftTimer.Stop();
                hasDraft = false;
                draftInfo = "";
                lblDraftSaved.Text = "";
            }
            else if (dateChanged)
            {
                LoadDraft();
                savedDraftSignature = DraftSignature();
            }
            UpdateStatusLabel();
            BindSummary();
            ledgerStale = false;
            LoadLedger();
            LoadPostClose();
            LoadGoods();
            RefreshFigures();
            if (historyMode)
            {
                ApplyHistoryMode();
            }
        }

        private void ApplyHistoryMode()
        {
            dtpBusinessDate.Enabled = false;
            btnLoad.Visible = false;
            btnHistory.Visible = false;
            lblHistoryBanner.Visible = true;
            lblDraftSaved.Visible = false;
            lblStatus.Text = isClosed ? "Status : CLOSED" : "Status : NOT CLOSED";
            lblStatus.ForeColor = isClosed ? Color.FromArgb(33, 37, 41) : Color.Red;
            btnPdf.Text = "Generate PDF";
            btnPdf.Visible = true;
            txtOpening.ReadOnly = true;
            btnUseDefault.Visible = false;
            txtUpiActual.ReadOnly = true;
            txtCardActual.ReadOnly = true;
            txtRemarks.ReadOnly = true;
            dgvCash.ReadOnly = true;
            btnSubmit.Enabled = false;
            btnSubmit.Visible = false;
            if (isClosed == false)
            {
                lblOpeningDefault.Text = hasDraft
                    ? "NOT CLOSED - view only. Showing saved draft values (" + draftInfo + ")."
                    : "NOT CLOSED - view only. Day Close has not been completed for this date.";
                lblOpeningDefault.ForeColor = Color.Red;
            }
        }

        private void UpdateStatusLabel()
        {
            if (isClosed)
            {
                lblStatus.Text = "Status : CLOSED";
                lblStatus.ForeColor = Color.DarkGreen;
            }
            else if (hasDraft)
            {
                lblStatus.Text = "Status : DRAFT";
                lblStatus.ForeColor = Color.DarkOrange;
            }
            else
            {
                lblStatus.Text = "Status : OPEN";
                lblStatus.ForeColor = Color.DarkBlue;
            }
        }

        private void LoadDraft()
        {
            DataSet set = new DataSet();
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.Proc_DayCloseV2_GetDraft", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BusinessDate", loadedDate);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(set);
                }
            }
            hasDraft = set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0;
            draftInfo = "";
            lblDraftSaved.Text = "";
            lblDraftSaved.ForeColor = Color.DimGray;
            draftFailureShown = false;
            if (hasDraft == false)
            {
                return;
            }
            DataRow draft = set.Tables[0].Rows[0];
            filling = true;
            if (draft["OpeningCash"] != DBNull.Value)
            {
                txtOpening.Text = Convert.ToDecimal(draft["OpeningCash"]).ToString("0.00");
            }
            txtUpiActual.Text = Convert.ToDecimal(draft["ActualUpi"]).ToString("0.00");
            txtCardActual.Text = Convert.ToDecimal(draft["ActualCard"]).ToString("0.00");
            txtRemarks.Text = Convert.ToString(draft["Remarks"]);
            if (set.Tables.Count > 1)
            {
                foreach (DataRow line in set.Tables[1].Rows)
                {
                    int rowIndex = FindFace(Convert.ToInt32(line["FaceValue"]));
                    if (rowIndex < 0)
                    {
                        continue;
                    }
                    dgvCash.Rows[rowIndex].Cells[1].Value = Convert.ToInt32(line["OpeningQty"]).ToString();
                    dgvCash.Rows[rowIndex].Cells[5].Value = Convert.ToInt32(line["PhysicalQty"]).ToString();
                    RefreshCashRow(rowIndex);
                }
            }
            filling = false;
            ApplyTypedOpening();
            draftInfo = "last saved " + Convert.ToDateTime(draft["UpdatedOn"]).ToString("dd-MM-yyyy hh:mm tt", CultureInfo.InvariantCulture)
                + " by " + Convert.ToString(draft["UpdatedByName"]);
            lblDraftSaved.Text = "Draft " + draftInfo;
        }

        private bool CanSaveDraft()
        {
            return historyMode == false && isClosed == false && filling == false && loadedDate != DateTime.MinValue;
        }

        private string DraftSignature()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            decimal opening;
            builder.Append(decimal.TryParse(txtOpening.Text, out opening) ? opening.ToString("0.00", CultureInfo.InvariantCulture) : "-");
            builder.Append('|').Append(TypedAmount(txtUpiActual).ToString("0.00", CultureInfo.InvariantCulture));
            builder.Append('|').Append(TypedAmount(txtCardActual).ToString("0.00", CultureInfo.InvariantCulture));
            builder.Append('|').Append(txtRemarks.Text.Trim());
            for (int i = 0; i < dgvCash.Rows.Count; i++)
            {
                builder.Append('|').Append(CellInt(dgvCash.Rows[i].Cells[1].Value)).Append(':').Append(CellInt(dgvCash.Rows[i].Cells[5].Value));
            }
            return builder.ToString();
        }

        private void draftField_TextChanged(object sender, EventArgs e)
        {
            if (CanSaveDraft() == false)
            {
                return;
            }
            draftTimer.Stop();
            draftTimer.Start();
        }

        private void draftField_Leave(object sender, EventArgs e)
        {
            SaveDraft();
        }

        private void draftTimer_Tick(object sender, EventArgs e)
        {
            draftTimer.Stop();
            SaveDraft();
        }

        // Saves only the user-entered values; returns false only when a save was needed and failed.
        private bool SaveDraft()
        {
            draftTimer.Stop();
            if (CanSaveDraft() == false)
            {
                return true;
            }
            string signature = DraftSignature();
            if (signature == savedDraftSignature)
            {
                return true;
            }
            DataTable counts = new DataTable();
            counts.Columns.Add("FaceValue", typeof(int));
            counts.Columns.Add("OpeningQty", typeof(int));
            counts.Columns.Add("PhysicalQty", typeof(int));
            for (int i = 0; i < dgvCash.Rows.Count; i++)
            {
                counts.Rows.Add(CellInt(dgvCash.Rows[i].Cells[0].Value), Math.Max(0, CellInt(dgvCash.Rows[i].Cells[1].Value)), Math.Max(0, CellInt(dgvCash.Rows[i].Cells[5].Value)));
            }
            int result = 0;
            string message = "";
            DateTime savedOn = DateTime.Now;
            try
            {
                using (SqlConnection con = new SqlConnection(Program.connection))
                using (SqlCommand cmd = new SqlCommand("dbo.Proc_DayCloseV2_SaveDraft", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BusinessDate", loadedDate);
                    decimal opening;
                    cmd.Parameters.AddWithValue("@OpeningCash", decimal.TryParse(txtOpening.Text, out opening) ? (object)opening : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ActualUpi", TypedAmount(txtUpiActual));
                    cmd.Parameters.AddWithValue("@ActualCard", TypedAmount(txtCardActual));
                    cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());
                    int updatedBy;
                    cmd.Parameters.AddWithValue("@UpdatedBy", int.TryParse(Program.userid, out updatedBy) ? (object)updatedBy : DBNull.Value);
                    SqlParameter countsParameter = cmd.Parameters.Add("@Denominations", SqlDbType.Structured);
                    countsParameter.TypeName = "dbo.DayCloseV2DraftDenominationRow";
                    countsParameter.Value = counts;
                    SqlParameter resultParameter = cmd.Parameters.Add("@Result", SqlDbType.Int);
                    resultParameter.Direction = ParameterDirection.Output;
                    SqlParameter messageParameter = cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 400);
                    messageParameter.Direction = ParameterDirection.Output;
                    SqlParameter savedOnParameter = cmd.Parameters.Add("@SavedOn", SqlDbType.DateTime);
                    savedOnParameter.Direction = ParameterDirection.Output;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    result = Convert.ToInt32(resultParameter.Value);
                    message = Convert.ToString(messageParameter.Value);
                    if (savedOnParameter.Value != DBNull.Value)
                    {
                        savedOn = Convert.ToDateTime(savedOnParameter.Value);
                    }
                }
            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            if (result != 1)
            {
                lblDraftSaved.Text = "Draft NOT saved: " + message;
                lblDraftSaved.ForeColor = Color.Red;
                if (draftFailureShown == false)
                {
                    draftFailureShown = true;
                    MessageBox.Show("The Day Close draft could not be saved." + Environment.NewLine + Environment.NewLine + message,
                        "Day Close", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return false;
            }
            savedDraftSignature = signature;
            draftFailureShown = false;
            hasDraft = true;
            string user = Program.Userfullname == null || Program.Userfullname.Trim().Length == 0 ? Convert.ToString(Program.UserName) : Program.Userfullname.Trim();
            draftInfo = "last saved " + savedOn.ToString("dd-MM-yyyy hh:mm tt", CultureInfo.InvariantCulture) + " by " + user;
            lblDraftSaved.Text = "Draft saved at " + savedOn.ToString("hh:mm tt", CultureInfo.InvariantCulture);
            lblDraftSaved.ForeColor = Color.DimGray;
            UpdateStatusLabel();
            return true;
        }

        private bool ConfirmDiscardDraft()
        {
            return MessageBox.Show("The Day Close draft could not be saved. Unsaved changes will be lost. Continue anyway?",
                "Day Close", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (SaveDraft() == false && ConfirmDiscardDraft() == false)
            {
                e.Cancel = true;
                return;
            }
            draftTimer.Stop();
            base.OnFormClosing(e);
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            using (DayCloseV2History history = new DayCloseV2History())
            {
                history.ShowDialog(this);
            }
        }

        private static decimal OptionalAmount(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) && row[column] != DBNull.Value ? Convert.ToDecimal(row[column]) : 0m;
        }

        private void ApplyTypedOpening()
        {
            decimal typed;
            if (decimal.TryParse(txtOpening.Text, out typed) == false)
            {
                typed = 0m;
            }
            openingCash = typed;
            expectedCash = openingCash + cashIn - cashOut;
        }

        private void txtOpening_TextChanged(object sender, EventArgs e)
        {
            if (filling || isClosed)
            {
                return;
            }
            ApplyTypedOpening();
            BindSummary();
            RefreshFigures();
            ledgerStale = true;
        }

        private void txtOpening_Leave(object sender, EventArgs e)
        {
            ReloadLedgerIfStale();
        }

        private void ReloadLedgerIfStale()
        {
            if (ledgerStale == false)
            {
                return;
            }
            ledgerStale = false;
            LoadLedger();
        }

        private void btnUseDefault_Click(object sender, EventArgs e)
        {
            if (historyMode || isClosed || hasPreviousClose == false)
            {
                return;
            }
            for (int i = 0; i < dgvCash.Rows.Count; i++)
            {
                int face = CellInt(dgvCash.Rows[i].Cells[0].Value);
                int count;
                defaultOpeningCounts.TryGetValue(face, out count);
                dgvCash.Rows[i].Cells[1].Value = count.ToString();
                RefreshCashRow(i);
            }
            txtOpening.Text = defaultOpeningCash.ToString("0.00");
            ReloadLedgerIfStale();
            SaveDraft();
        }

        private void txtActual_TextChanged(object sender, EventArgs e)
        {
            if (filling)
            {
                return;
            }
            BindSummary();
            RefreshFigures();
        }

        private void BindSummary()
        {
            decimal actualCash = ActualCash();
            decimal actualUpi = TypedAmount(txtUpiActual);
            decimal actualCard = TypedAmount(txtCardActual);
            DataTable summary = new DataTable();
            summary.Columns.Add("Mode");
            summary.Columns.Add("Opening");
            summary.Columns.Add("IN");
            summary.Columns.Add("OUT");
            summary.Columns.Add("Carried IN");
            summary.Columns.Add("Carried OUT");
            summary.Columns.Add("Expected");
            summary.Columns.Add("Actual");
            summary.Columns.Add("Difference");
            summary.Rows.Add("Cash", openingCash.ToString("N2"), cashIn.ToString("N2"), cashOut.ToString("N2"), cashCarriedIn.ToString("N2"), cashCarriedOut.ToString("N2"), expectedCash.ToString("N2"), actualCash.ToString("N2"), Signed(actualCash - expectedCash));
            summary.Rows.Add("UPI", "", upiIn.ToString("N2"), upiOut.ToString("N2"), upiCarriedIn.ToString("N2"), upiCarriedOut.ToString("N2"), expectedUpi.ToString("N2"), actualUpi.ToString("N2"), Signed(actualUpi - expectedUpi));
            summary.Rows.Add("Card", "", cardIn.ToString("N2"), cardOut.ToString("N2"), cardCarriedIn.ToString("N2"), cardCarriedOut.ToString("N2"), expectedCard.ToString("N2"), actualCard.ToString("N2"), Signed(actualCard - expectedCard));
            dgvSummary.DataSource = summary;
        }

        private void dgvSummary_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgvSummary.Columns.Count <= e.ColumnIndex)
            {
                return;
            }
            if (dgvSummary.Columns[e.ColumnIndex].Name != "Difference")
            {
                return;
            }
            e.CellStyle.ForeColor = AmountOf(e.Value) == 0m ? Color.Black : Color.Red;
            e.CellStyle.Font = new Font(dgvSummary.Font, FontStyle.Bold);
        }

        private void cmbLedger_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (filling)
            {
                return;
            }
            LoadLedger();
        }

        private void LoadLedger()
        {
            string filter = cmbLedger.SelectedItem == null ? "All" : Convert.ToString(cmbLedger.SelectedItem);
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.Proc_DayCloseV2_GetPaymentLedger", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BusinessDate", dtpBusinessDate.Value.Date);
                cmd.Parameters.AddWithValue("@Filter", filter);
                cmd.Parameters.AddWithValue("@OpeningCash", openingCash);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(table);
                }
            }
            dgvLedger.DataSource = table;
            FormatLedgerColumns(dgvLedger);
            if (dgvLedger.Columns.Contains("Posting"))
            {
                bool carried = false;
                foreach (DataRow row in table.Rows)
                {
                    carried = carried || Convert.ToString(row["Posting"]).Length > 0;
                }
                dgvLedger.Columns["Posting"].Visible = carried;
            }
        }

        private void dgvLedger_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvLedger.Columns.Contains("Posting") == false)
            {
                return;
            }
            if (Convert.ToString(dgvLedger.Rows[e.RowIndex].Cells["Posting"].Value).Length > 0)
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 243, 205);
            }
        }

        private static string CarriedNote(decimal carried)
        {
            return carried == 0m ? "" : "  (carried fwd " + carried.ToString("N2") + ")";
        }

        // Runs before the handle exists (constructor load); setting Width/MinimumWidth on a Fill column then throws NullReferenceException,
        // so widths are only assigned to columns switched to AutoSizeMode.None first.
        private static void FormatLedgerColumns(DataGridView grid)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            SetLedgerHeader(grid, "TransTime", "Time");
            SetLedgerHeader(grid, "Direction", "Dir");
            SetLedgerHeader(grid, "EntityReference", "Reference");
            SetLedgerHeader(grid, "EntityType", "Type");
            SetLedgerHeader(grid, "AmountIn", "IN");
            SetLedgerHeader(grid, "AmountOut", "OUT");
            SetLedgerHeader(grid, "RunningBalance", "Balance");
            if (grid.Columns.Contains("TransTime"))
            {
                grid.Columns["TransTime"].DefaultCellStyle.Format = "HH:mm";
            }
            SetLedgerFixedWidth(grid, "EntityReference", 130);
            SetLedgerFixedWidth(grid, "EntityType", 150);
            if (grid.Columns.Contains("Description"))
            {
                DataGridViewColumn description = grid.Columns["Description"];
                description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                description.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
        }

        private static void SetLedgerHeader(DataGridView grid, string name, string header)
        {
            if (grid.Columns.Contains(name))
            {
                grid.Columns[name].HeaderText = header;
            }
        }

        private static void SetLedgerFixedWidth(DataGridView grid, string name, int width)
        {
            if (grid.Columns.Contains(name) == false)
            {
                return;
            }
            DataGridViewColumn column = grid.Columns[name];
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Width = width;
            column.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        }

        private void LoadDenomination(bool resetOpening)
        {
            DataTable table = Query("dbo.Proc_DayCloseV2_GetDenominationRecon");
            defaultOpeningCounts.Clear();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int face = Convert.ToInt32(table.Rows[i]["FaceValue"]);
                int rowIndex = FindFace(face);
                if (rowIndex < 0)
                {
                    continue;
                }
                int openingQty = Convert.ToInt32(table.Rows[i]["OpeningQty"]);
                int inQty = Convert.ToInt32(table.Rows[i]["InQty"]);
                int outQty = Convert.ToInt32(table.Rows[i]["OutQty"]);
                if (isClosed == false && hasPreviousClose)
                {
                    defaultOpeningCounts[face] = openingQty;
                }
                if (resetOpening == false)
                {
                    openingQty = CellInt(dgvCash.Rows[rowIndex].Cells[1].Value);
                }
                dgvCash.Rows[rowIndex].Cells[1].Value = openingQty.ToString();
                dgvCash.Rows[rowIndex].Cells[2].Value = inQty.ToString();
                dgvCash.Rows[rowIndex].Cells[3].Value = outQty.ToString();
                if (table.Rows[i]["ActualQty"] != DBNull.Value)
                {
                    dgvCash.Rows[rowIndex].Cells[5].Value = Convert.ToInt32(table.Rows[i]["ActualQty"]).ToString();
                }
                RefreshCashRow(rowIndex);
            }
        }

        private void dgvCash_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            if (historyMode || (e.ColumnIndex != 1 && e.ColumnIndex != 5))
            {
                return;
            }
            int count = CellInt(dgvCash.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            if (count < 0)
            {
                count = 0;
            }
            dgvCash.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = count.ToString();
            RefreshCashRow(e.RowIndex);
            BindSummary();
            RefreshFigures();
            SaveDraft();
        }

        private void RefreshCashRow(int rowIndex)
        {
            int openingQty = CellInt(dgvCash.Rows[rowIndex].Cells[1].Value);
            int inQty = CellInt(dgvCash.Rows[rowIndex].Cells[2].Value);
            int outQty = CellInt(dgvCash.Rows[rowIndex].Cells[3].Value);
            int expectedQty = openingQty + inQty - outQty;
            int physical = CellInt(dgvCash.Rows[rowIndex].Cells[5].Value);
            dgvCash.Rows[rowIndex].Cells[4].Value = expectedQty.ToString();
            dgvCash.Rows[rowIndex].Cells[6].Value = (physical - expectedQty).ToString();
        }

        private void dgvCash_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 6)
            {
                return;
            }
            int difference = CellInt(e.Value);
            e.CellStyle.ForeColor = difference == 0 ? Color.Black : Color.Red;
            e.CellStyle.Font = new Font(dgvCash.Font, FontStyle.Bold);
        }

        private void LoadGoods()
        {
            DataSet set = new DataSet();
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.Proc_DayCloseV2_GetGoodsSummary", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BusinessDate", dtpBusinessDate.Value.Date);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(set);
                }
            }
            goodsProducts = set.Tables.Count > 0 ? set.Tables[0] : new DataTable();
            goodsCategories = set.Tables.Count > 1 ? set.Tables[1] : new DataTable();
            dgvGoodsCategory.DataSource = goodsCategories;
            FormatGoodsGrid(dgvGoodsCategory);

            string selected = cmbGoodsCategory.SelectedItem == null ? "All" : Convert.ToString(cmbGoodsCategory.SelectedItem);
            filling = true;
            cmbGoodsCategory.Items.Clear();
            cmbGoodsCategory.Items.Add("All");
            for (int i = 0; i < goodsCategories.Rows.Count; i++)
            {
                cmbGoodsCategory.Items.Add(Convert.ToString(goodsCategories.Rows[i]["Category"]));
            }
            cmbGoodsCategory.SelectedItem = cmbGoodsCategory.Items.Contains(selected) ? selected : "All";
            filling = false;
            BindGoods();
        }

        private void FormatGoodsGrid(DataGridView grid)
        {
            SetHeader(grid, "ProductId", "Product ID");
            SetHeader(grid, "ProductCode", null);
            if (grid.Columns.Contains("ProductId"))
            {
                grid.Columns["ProductId"].Visible = true;
                grid.Columns["ProductId"].DisplayIndex = 0;
            }
            SetHeader(grid, "ProductName", "Product Name");
            SetHeader(grid, "GoodsIn", "Goods IN");
            SetHeader(grid, "GoodsOut", "Goods OUT");
            string[] numeric = new string[] { "GoodsIn", "GoodsOut", "Net", "Products" };
            foreach (string name in numeric)
            {
                if (grid.Columns.Contains(name))
                {
                    grid.Columns[name].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    grid.Columns[name].DefaultCellStyle.Format = name == "Products" ? "0" : "#,##0.###";
                }
            }
        }

        private void SetHeader(DataGridView grid, string name, string header)
        {
            if (grid.Columns.Contains(name) == false)
            {
                return;
            }
            if (header == null)
            {
                grid.Columns[name].Visible = false;
            }
            else
            {
                grid.Columns[name].HeaderText = header;
            }
        }

        private void goodsFilter_Changed(object sender, EventArgs e)
        {
            if (filling)
            {
                return;
            }
            BindGoods();
        }

        private void dgvGoodsCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvGoodsCategory.Columns.Contains("Category") == false)
            {
                return;
            }
            string category = Convert.ToString(dgvGoodsCategory.Rows[e.RowIndex].Cells["Category"].Value);
            if (cmbGoodsCategory.Items.Contains(category))
            {
                cmbGoodsCategory.SelectedItem = category;
            }
        }

        private void BindGoods()
        {
            if (goodsProducts == null)
            {
                return;
            }
            List<string> filters = new List<string>();
            string category = cmbGoodsCategory.SelectedItem == null ? "All" : Convert.ToString(cmbGoodsCategory.SelectedItem);
            string movement = cmbGoodsMove.SelectedItem == null ? "All" : Convert.ToString(cmbGoodsMove.SelectedItem);
            string search = txtGoodsSearch.Text.Trim();
            if (category != "All")
            {
                filters.Add("Category = '" + category.Replace("'", "''") + "'");
            }
            if (movement == "IN")
            {
                filters.Add("GoodsIn > 0");
            }
            else if (movement == "OUT")
            {
                filters.Add("GoodsOut > 0");
            }
            if (search.Length > 0)
            {
                string like = LikeValue(search);
                filters.Add("(ProductName LIKE '%" + like + "%' OR CONVERT(ProductId, 'System.String') LIKE '%" + like + "%')");
            }
            DataView view = new DataView(goodsProducts);
            view.RowFilter = string.Join(" AND ", filters.ToArray());
            dgvGoods.DataSource = view;
            FormatGoodsGrid(dgvGoods);

            decimal shownIn = 0m;
            decimal shownOut = 0m;
            foreach (DataRowView row in view)
            {
                shownIn += Convert.ToDecimal(row["GoodsIn"]);
                shownOut += Convert.ToDecimal(row["GoodsOut"]);
            }
            decimal dayIn = 0m;
            decimal dayOut = 0m;
            foreach (DataRow row in goodsCategories.Rows)
            {
                dayIn += Convert.ToDecimal(row["GoodsIn"]);
                dayOut += Convert.ToDecimal(row["GoodsOut"]);
            }
            lblGoodsTotals.Text = "Shown  :  Goods IN  " + shownIn.ToString("#,##0.###")
                + "      Goods OUT  " + shownOut.ToString("#,##0.###")
                + "      Net  " + (shownIn - shownOut).ToString("#,##0.###")
                + "      Rows  " + view.Count.ToString()
                + Environment.NewLine
                + "Day total  :  Goods IN  " + dayIn.ToString("#,##0.###")
                + "      Goods OUT  " + dayOut.ToString("#,##0.###")
                + "      Net  " + (dayIn - dayOut).ToString("#,##0.###");
        }

        private static string LikeValue(string text)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            foreach (char c in text)
            {
                if (c == '*' || c == '%' || c == '[' || c == ']')
                {
                    builder.Append('[').Append(c).Append(']');
                }
                else if (c == '\'')
                {
                    builder.Append("''");
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

        private void dgvGoods_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvGoods.Columns.Contains("ProductId") == false)
            {
                return;
            }
            DataGridViewRow row = dgvGoods.Rows[e.RowIndex];
            object productId = row.Cells["ProductId"].Value;
            string category = Convert.ToString(row.Cells["Category"].Value);
            string product = Convert.ToString(row.Cells["ProductName"].Value);
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.Proc_DayCloseV2_GetGoodsDetail", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BusinessDate", dtpBusinessDate.Value.Date);
                cmd.Parameters.AddWithValue("@ProductId", productId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Category", category);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(table);
                }
            }
            Form detail = new Form();
            detail.Text = "Goods transactions - " + product + " - " + category + " - " + dtpBusinessDate.Value.ToString("dd-MM-yyyy");
            detail.StartPosition = FormStartPosition.CenterParent;
            detail.Size = new Size(900, 400);
            detail.Font = Font;
            DataGridView grid = Grid(detail, 0, 0, 900, 400);
            grid.Dock = DockStyle.Fill;
            grid.DataSource = table;
            detail.ShowDialog(this);
        }

        private void RefreshFigures()
        {
            decimal actualCash = ActualCash();
            decimal actualUpi = TypedAmount(txtUpiActual);
            decimal actualCard = TypedAmount(txtCardActual);
            decimal cashDiff = actualCash - expectedCash;
            decimal upiDiff = actualUpi - expectedUpi;
            decimal cardDiff = actualCard - expectedCard;
            lblLedgerTotals.Text = "Cash IN Total  " + cashIn.ToString("N2") + CarriedNote(cashCarriedIn)
                + "      Cash OUT Total  " + cashOut.ToString("N2") + CarriedNote(cashCarriedOut)
                + Environment.NewLine
                + "UPI IN Total  " + upiIn.ToString("N2") + CarriedNote(upiCarriedIn)
                + "      UPI OUT Total  " + upiOut.ToString("N2") + CarriedNote(upiCarriedOut)
                + Environment.NewLine
                + "Card IN Total  " + cardIn.ToString("N2") + CarriedNote(cardCarriedIn)
                + "      Card OUT Total  " + cardOut.ToString("N2") + CarriedNote(cardCarriedOut);
            lblCashBridge.Text = "Opening Cash          " + openingCash.ToString("N2")
                + Environment.NewLine
                + "+ Cash IN                " + cashIn.ToString("N2") + CarriedNote(cashCarriedIn)
                + Environment.NewLine
                + "- Cash OUT               " + cashOut.ToString("N2") + CarriedNote(cashCarriedOut)
                + Environment.NewLine
                + "Expected Cash Closing    " + expectedCash.ToString("N2");
            lblDenomTotals.Text = "Expected Cash Total  " + expectedCash.ToString("N2")
                + "      Physical Cash Total  " + actualCash.ToString("N2")
                + "      Cash Difference  " + Signed(cashDiff);
            lblDenomTotals.ForeColor = cashDiff == 0m ? Color.Black : Color.Red;
            if (isClosed)
            {
                lblOpeningCheck.Text = "Opening snapshot saved with this close.";
                lblOpeningCheck.ForeColor = Color.DimGray;
            }
            else
            {
                decimal openingTotal = OpeningDenominationTotal();
                decimal openingDiff = openingTotal - openingCash;
                lblOpeningCheck.Text = "Opening Denomination Total  " + openingTotal.ToString("N2")
                    + "      Opening Cash  " + openingCash.ToString("N2")
                    + "      Difference  " + Signed(openingDiff)
                    + (openingDiff == 0m ? "      MATCHED" : "      DOES NOT MATCH");
                lblOpeningCheck.ForeColor = openingDiff == 0m ? Color.DarkGreen : Color.Red;
            }
            lblUpiCard.Text = "UPI Expected  " + expectedUpi.ToString("N2")
                + "    Actual  " + actualUpi.ToString("N2")
                + "    Difference  " + Signed(upiDiff)
                + Environment.NewLine
                + "Card Expected  " + expectedCard.ToString("N2")
                + "    Actual  " + actualCard.ToString("N2")
                + "    Difference  " + Signed(cardDiff);
            lblUpiCard.ForeColor = (upiDiff == 0m && cardDiff == 0m) ? Color.Black : Color.Red;
            bool balanced = cashDiff == 0m && upiDiff == 0m && cardDiff == 0m;
            lblFinal.Text = "CASH    Expected  " + expectedCash.ToString("N2") + "    Actual  " + actualCash.ToString("N2") + "    Variance  " + Signed(cashDiff)
                + Environment.NewLine
                + "UPI     Expected  " + expectedUpi.ToString("N2") + "    Actual  " + actualUpi.ToString("N2") + "    Variance  " + Signed(upiDiff)
                + Environment.NewLine
                + "CARD    Expected  " + expectedCard.ToString("N2") + "    Actual  " + actualCard.ToString("N2") + "    Variance  " + Signed(cardDiff)
                + Environment.NewLine
                + Environment.NewLine
                + (balanced ? "Overall Status : BALANCED" : "Overall Status : VARIANCE");
            lblFinal.ForeColor = balanced ? Color.DarkGreen : Color.Red;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (historyMode)
            {
                return;
            }
            if (isClosed)
            {
                MessageBox.Show("This business date is already closed.");
                return;
            }
            draftTimer.Stop();
            decimal actualUpi;
            decimal actualCard;
            decimal opening;
            if (decimal.TryParse(txtUpiActual.Text, out actualUpi) == false || decimal.TryParse(txtCardActual.Text, out actualCard) == false || decimal.TryParse(txtOpening.Text, out opening) == false)
            {
                MessageBox.Show("Enter numeric actual UPI, Card, and opening cash amounts.");
                return;
            }
            if (opening < 0m)
            {
                MessageBox.Show("Opening cash cannot be negative.");
                txtOpening.Focus();
                return;
            }
            decimal openingTotal = OpeningDenominationTotal();
            if (Math.Abs(openingTotal - opening) > 0.009m)
            {
                MessageBox.Show("Opening denomination total does not match Opening Cash."
                    + Environment.NewLine + Environment.NewLine
                    + "Opening Cash: " + opening.ToString("N2") + Environment.NewLine
                    + "Opening Denomination Total: " + openingTotal.ToString("N2") + Environment.NewLine
                    + "Difference: " + Signed(openingTotal - opening));
                return;
            }
            decimal actualCash = ActualCash();
            decimal cashDiff = actualCash - expectedCash;
            decimal upiDiff = actualUpi - expectedUpi;
            decimal cardDiff = actualCard - expectedCard;
            if ((cashDiff != 0m || upiDiff != 0m || cardDiff != 0m) && txtRemarks.Text.Trim().Length == 0)
            {
                MessageBox.Show("Enter a variance reason.");
                txtRemarks.Focus();
                return;
            }
            DateTime closeDate = dtpBusinessDate.Value.Date;
            string confirm = "Business Date: " + closeDate.ToString("dd-MM-yyyy")
                + Environment.NewLine + Environment.NewLine
                + "Cash    Expected " + expectedCash.ToString("N2") + "    Actual " + actualCash.ToString("N2") + "    Difference " + Signed(cashDiff)
                + Environment.NewLine
                + "UPI     Expected " + expectedUpi.ToString("N2") + "    Actual " + actualUpi.ToString("N2") + "    Difference " + Signed(upiDiff)
                + Environment.NewLine
                + "Card    Expected " + expectedCard.ToString("N2") + "    Actual " + actualCard.ToString("N2") + "    Difference " + Signed(cardDiff)
                + Environment.NewLine + Environment.NewLine
                + "Closing Physical Cash: " + actualCash.ToString("N2")
                + Environment.NewLine + Environment.NewLine
                + "This Day Close will be finalized. Continue?";
            if (MessageBox.Show(confirm, "Confirm Day Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            btnSubmit.Enabled = false;
            int result = 0;
            string message = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Program.connection))
                using (SqlCommand cmd = new SqlCommand("dbo.Proc_DayCloseV2_Submit", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BusinessDate", closeDate);
                    cmd.Parameters.AddWithValue("@OpeningCash", opening);
                    cmd.Parameters.AddWithValue("@ActualUpi", actualUpi);
                    cmd.Parameters.AddWithValue("@ActualCard", actualCard);
                    cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());
                    int enteredBy = 0;
                    int.TryParse(Program.userid, out enteredBy);
                    cmd.Parameters.AddWithValue("@EnteredBy", enteredBy);
                    cmd.Parameters.AddWithValue("@TwoThousand", NoteCount(2000));
                    cmd.Parameters.AddWithValue("@Thousand", NoteCount(1000));
                    cmd.Parameters.AddWithValue("@FiveHundred", NoteCount(500));
                    cmd.Parameters.AddWithValue("@TwoHundred", NoteCount(200));
                    cmd.Parameters.AddWithValue("@Hundred", NoteCount(100));
                    cmd.Parameters.AddWithValue("@Fifty", NoteCount(50));
                    cmd.Parameters.AddWithValue("@Twenty", NoteCount(20));
                    cmd.Parameters.AddWithValue("@Ten", NoteCount(10));
                    cmd.Parameters.AddWithValue("@Five", NoteCount(5));
                    cmd.Parameters.AddWithValue("@Two", NoteCount(2));
                    cmd.Parameters.AddWithValue("@One", NoteCount(1));
                    cmd.Parameters.AddWithValue("@OpenTwoThousand", OpeningCount(2000));
                    cmd.Parameters.AddWithValue("@OpenThousand", OpeningCount(1000));
                    cmd.Parameters.AddWithValue("@OpenFiveHundred", OpeningCount(500));
                    cmd.Parameters.AddWithValue("@OpenTwoHundred", OpeningCount(200));
                    cmd.Parameters.AddWithValue("@OpenHundred", OpeningCount(100));
                    cmd.Parameters.AddWithValue("@OpenFifty", OpeningCount(50));
                    cmd.Parameters.AddWithValue("@OpenTwenty", OpeningCount(20));
                    cmd.Parameters.AddWithValue("@OpenTen", OpeningCount(10));
                    cmd.Parameters.AddWithValue("@OpenFive", OpeningCount(5));
                    cmd.Parameters.AddWithValue("@OpenTwo", OpeningCount(2));
                    cmd.Parameters.AddWithValue("@OpenOne", OpeningCount(1));
                    SqlParameter resultParameter = cmd.Parameters.Add("@Result", SqlDbType.Int);
                    resultParameter.Direction = ParameterDirection.Output;
                    SqlParameter messageParameter = cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 400);
                    messageParameter.Direction = ParameterDirection.Output;
                    SqlParameter idParameter = cmd.Parameters.Add("@DayCloseId", SqlDbType.Int);
                    idParameter.Direction = ParameterDirection.Output;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    result = Convert.ToInt32(resultParameter.Value);
                    message = Convert.ToString(messageParameter.Value);
                }
            }
            catch (SqlException ex)
            {
                btnSubmit.Enabled = true;
                MessageBox.Show("Day Close was not saved." + Environment.NewLine + Environment.NewLine + ex.Message);
                return;
            }
            if (result != 1)
            {
                btnSubmit.Enabled = true;
                MessageBox.Show(message);
                return;
            }

            isClosed = true;
            try
            {
                loadedDate = DateTime.MinValue;
                LoadExpected();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Day Close saved, but the screen could not be refreshed. Click Load to view it." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
            GeneratePdfAfterSave(closeDate);
        }

        private void GeneratePdfAfterSave(DateTime closeDate)
        {
            string path;
            try
            {
                path = DayCloseV2Pdf.Generate(closeDate);
            }
            catch (Exception ex)
            {
                DayCloseV2Pdf.LogError(closeDate, ex);
                MessageBox.Show("Day Close completed successfully, but PDF generation failed."
                    + Environment.NewLine
                    + "You can regenerate the report from the completed Day Close (View / Regenerate PDF)."
                    + Environment.NewLine + Environment.NewLine
                    + "Error: " + ex.Message, "Day Close", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Day Close completed successfully." + Environment.NewLine + Environment.NewLine
                + "PDF saved to:" + Environment.NewLine + path + Environment.NewLine + Environment.NewLine
                + "Open the PDF now?", "Day Close", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                OpenPdf(path);
            }
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            if (loadedDate == DateTime.MinValue)
            {
                MessageBox.Show("Load a business date before generating the PDF.", "Day Close", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DateTime closeDate = loadedDate;
            string path;
            try
            {
                Cursor = Cursors.WaitCursor;
                path = DayCloseV2Pdf.Generate(closeDate);
            }
            catch (Exception ex)
            {
                DayCloseV2Pdf.LogError(closeDate, ex);
                MessageBox.Show("The PDF could not be generated." + Environment.NewLine + Environment.NewLine + "Error: " + ex.Message,
                    "Day Close", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
            OpenPdf(path);
        }

        private void OpenPdf(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch (Win32Exception)
            {
                MessageBox.Show("PDF saved to:" + Environment.NewLine + path + Environment.NewLine + Environment.NewLine + "No PDF viewer is available to open it.");
            }
        }

        private DataTable Query(string procedureName)
        {
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand(procedureName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BusinessDate", dtpBusinessDate.Value.Date);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }

        private decimal ActualCash()
        {
            decimal total = 0m;
            for (int i = 0; i < dgvCash.Rows.Count; i++)
            {
                int face = CellInt(dgvCash.Rows[i].Cells[0].Value);
                int count = CellInt(dgvCash.Rows[i].Cells[5].Value);
                total += face * count;
            }
            return total;
        }

        private decimal OpeningDenominationTotal()
        {
            decimal total = 0m;
            for (int i = 0; i < dgvCash.Rows.Count; i++)
            {
                total += CellInt(dgvCash.Rows[i].Cells[0].Value) * CellInt(dgvCash.Rows[i].Cells[1].Value);
            }
            return total;
        }

        private int NoteCount(int face)
        {
            int rowIndex = FindFace(face);
            if (rowIndex < 0)
            {
                return 0;
            }
            return CellInt(dgvCash.Rows[rowIndex].Cells[5].Value);
        }

        private int OpeningCount(int face)
        {
            int rowIndex = FindFace(face);
            if (rowIndex < 0)
            {
                return 0;
            }
            return CellInt(dgvCash.Rows[rowIndex].Cells[1].Value);
        }

        private int FindFace(int face)
        {
            for (int i = 0; i < dgvCash.Rows.Count; i++)
            {
                if (CellInt(dgvCash.Rows[i].Cells[0].Value) == face)
                {
                    return i;
                }
            }
            return -1;
        }

        private void ClearPhysical()
        {
            for (int i = 0; i < dgvCash.Rows.Count; i++)
            {
                dgvCash.Rows[i].Cells[5].Value = "0";
                RefreshCashRow(i);
            }
        }

        private int CellInt(object value)
        {
            int count;
            int.TryParse(Convert.ToString(value), out count);
            return count;
        }

        private decimal TypedAmount(TextBox box)
        {
            decimal amount;
            decimal.TryParse(box.Text, out amount);
            return amount;
        }

        private decimal AmountOf(object value)
        {
            decimal amount;
            decimal.TryParse(Convert.ToString(value), NumberStyles.Number, CultureInfo.CurrentCulture, out amount);
            return amount;
        }

        private string Signed(decimal value)
        {
            if (value > 0m)
            {
                return "+" + value.ToString("N2");
            }
            return value.ToString("N2");
        }
    }
}
