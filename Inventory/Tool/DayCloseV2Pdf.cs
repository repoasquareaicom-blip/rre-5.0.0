using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace Inventory.Tool
{
    public class DayCloseV2Pdf
    {
        private const float Left = 36f;
        private const float Right = 559.28f;
        private const float ContentWidth = Right - Left;
        private const float Bottom = 790f;
        private const float RowHeight = 14f;
        private const float BodySize = 8f;
        private const float WrapLineHeight = 9.6f;
        private const string CompanyName = "R R Electrical Agencies";

        private static readonly Color Ink = Color.FromArgb(33, 37, 41);
        private static readonly Color Muted = Color.FromArgb(108, 117, 125);
        private static readonly Color TitleRed = Color.FromArgb(128, 20, 24);
        private static readonly Color Brand = Color.FromArgb(22, 58, 107);
        private static readonly Color HeaderFill = Color.FromArgb(226, 232, 240);
        private static readonly Color SectionFill = Color.FromArgb(241, 245, 249);
        private static readonly Color TotalFill = Color.FromArgb(236, 240, 245);
        private static readonly Color StripeFill = Color.FromArgb(249, 250, 251);
        private static readonly Color GridLine = Color.FromArgb(203, 213, 225);
        private static readonly Color Alert = Color.FromArgb(200, 30, 30);
        private static readonly Color Good = Color.FromArgb(22, 128, 61);
        private static readonly CultureInfo Money = CultureInfo.GetCultureInfo("en-IN");

        private SimplePdf pdf;
        private float y;
        private DateTime businessDate;
        private DateTime generatedOn;
        private bool closed;
        private bool draft;
        private string draftNote;
        private string[] currentHeader;
        private Column[] currentColumns;

        private class Column
        {
            public string Title;
            public float Width;
            public int Align;
            public bool Wrap;

            public Column(string title, float width, int align)
            {
                Title = title;
                Width = width;
                Align = align;
            }
        }

        private delegate Color CellColor(int row, int column);

        public static string ReportFolder(DateTime date)
        {
            string root = ConfigurationManager.AppSettings["DayCloseReportPath"];
            if (root == null || root.Trim().Length == 0)
            {
                root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"RRE Electricals\Reports\DayClose");
            }
            else
            {
                root = Environment.ExpandEnvironmentVariables(root.Trim());
            }
            return Path.Combine(Path.Combine(root, date.ToString("yyyy", CultureInfo.InvariantCulture)), date.ToString("MM", CultureInfo.InvariantCulture));
        }

        public static string ReportFileName(DateTime date, bool closed)
        {
            string branch = BranchCode();
            string status = closed ? "_CLOSED" : "_NOT-CLOSED";
            string name = branch.Length > 0
                ? "RRE_" + branch + "_DayClose_" + date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + status + ".pdf"
                : "RRE_DayClose_" + date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + status + ".pdf";
            return Sanitize(name);
        }

        public static string Generate(DateTime date)
        {
            DayCloseV2Pdf report = new DayCloseV2Pdf();
            return report.Build(date.Date);
        }

        public static void LogError(DateTime date, Exception ex)
        {
            string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                + " | Day Close " + date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                + " | User " + Convert.ToString(Program.userid)
                + " | " + ex.ToString() + Environment.NewLine;
            string[] folders = new string[] { SafeLogFolder(), Path.GetTempPath() };
            foreach (string folder in folders)
            {
                if (folder == null)
                {
                    continue;
                }
                try
                {
                    Directory.CreateDirectory(folder);
                    File.AppendAllText(Path.Combine(folder, "DayClosePdfErrors.log"), line);
                    return;
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        private static string SafeLogFolder()
        {
            try
            {
                return Path.GetDirectoryName(Path.GetDirectoryName(ReportFolder(DateTime.Today)));
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static string BranchCode()
        {
            string branch = ConfigurationManager.AppSettings["BranchCode"];
            return branch == null ? "" : branch.Trim();
        }

        private static string Sanitize(string name)
        {
            StringBuilder builder = new StringBuilder(name.Length);
            char[] invalid = Path.GetInvalidFileNameChars();
            foreach (char c in name)
            {
                builder.Append(Array.IndexOf(invalid, c) >= 0 || c == ' ' ? '_' : c);
            }
            return builder.ToString();
        }

        private string Build(DateTime date)
        {
            businessDate = date;
            generatedOn = DateTime.Now;

            DataSet saved = Load("dbo.Proc_DayCloseV2_GetReport", date);
            closed = saved.Tables.Count >= 3 && saved.Tables[0].Rows.Count > 0;
            DataRow head;
            DataTable summary;
            DataTable denomination;
            if (closed)
            {
                head = saved.Tables[0].Rows[0];
                summary = saved.Tables[1];
                denomination = saved.Tables[2];
            }
            else
            {
                LoadNotClosed(date, out head, out summary, out denomination, out draftNote);
                draft = draftNote != null;
            }
            decimal opening = Convert.ToDecimal(head["OpeningCash"]);
            DataTable ledger = LoadLedger(date, opening);
            DataSet postClose = Load("dbo.Proc_DayCloseV2_GetPostCloseTransactions", date);
            DataSet goods = Load("dbo.Proc_DayCloseV2_GetGoodsSummary", date);
            DataRow branch = LoadBranch();

            pdf = new SimplePdf();
            pdf.Title = CompanyName + " Daily Closing Report " + date.ToString("dd-MM-yyyy");
            pdf.NewPage();
            DrawHeader(head, branch);
            DrawPaymentSummary(summary);
            DrawCashBridge(head, summary);
            DrawDenomination(denomination);
            DrawLedger(ledger);
            DrawPostClose(postClose.Tables.Count > 0 ? postClose.Tables[0] : new DataTable());
            DrawGoods(goods);
            DrawVariance(head);
            DrawSignatures(head);
            DrawFooters();

            string folder = ReportFolder(date);
            Directory.CreateDirectory(folder);
            string target = Path.Combine(folder, ReportFileName(date, closed));
            string temp = target + ".tmp";
            pdf.Save(temp);
            try
            {
                if (File.Exists(target))
                {
                    File.Delete(target);
                }
                File.Move(temp, target);
            }
            catch (IOException)
            {
                target = AlternatePath(target);
                File.Move(temp, target);
            }
            catch (UnauthorizedAccessException)
            {
                target = AlternatePath(target);
                File.Move(temp, target);
            }
            return target;
        }

        private static string AlternatePath(string target)
        {
            return Path.Combine(Path.GetDirectoryName(target),
                Path.GetFileNameWithoutExtension(target) + "_" + DateTime.Now.ToString("HHmmss", CultureInfo.InvariantCulture) + ".pdf");
        }

        private static DataSet Load(string procedure, DateTime date)
        {
            DataSet set = new DataSet();
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand(procedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BusinessDate", date);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(set);
                }
            }
            return set;
        }

        // Current Day Close V2 calculation for a date without a completed close; the same procedures the screen uses.
        // A saved draft supplies only the user-entered values (opening, counts, actual UPI/card, remarks); expected figures stay live.
        private static void LoadNotClosed(DateTime date, out DataRow head, out DataTable summary, out DataTable denomination, out string draftNote)
        {
            DataSet expectedSet = Load("dbo.Proc_DayCloseV2_GetExpectedSummary", date);
            if (expectedSet.Tables.Count == 0 || expectedSet.Tables[0].Rows.Count == 0)
            {
                throw new InvalidOperationException("Day Close figures could not be calculated for " + date.ToString("dd-MM-yyyy") + ".");
            }
            DataRow e = expectedSet.Tables[0].Rows[0];
            decimal defaultOpening = Convert.ToDecimal(e["OpeningCash"]);
            decimal opening = defaultOpening;
            decimal expectedCash = Convert.ToDecimal(e["ExpectedCash"]);
            decimal expectedUpi = Convert.ToDecimal(e["ExpectedUpi"]);
            decimal expectedCard = Convert.ToDecimal(e["ExpectedCard"]);

            DataSet draftSet = Load("dbo.Proc_DayCloseV2_GetDraft", date);
            DataRow draftRow = draftSet.Tables.Count > 0 && draftSet.Tables[0].Rows.Count > 0 ? draftSet.Tables[0].Rows[0] : null;
            draftNote = null;
            if (draftRow != null)
            {
                if (draftRow["OpeningCash"] != DBNull.Value)
                {
                    opening = Convert.ToDecimal(draftRow["OpeningCash"]);
                    expectedCash = opening + Convert.ToDecimal(e["CashIn"]) - Convert.ToDecimal(e["CashOut"]);
                }
                draftNote = "last saved " + Convert.ToDateTime(draftRow["UpdatedOn"]).ToString("dd-MM-yyyy hh:mm tt", CultureInfo.InvariantCulture)
                    + " by " + Text(draftRow["UpdatedByName"]);
            }

            DataTable headTable = new DataTable();
            string[] headColumns = new string[]
            {
                "Id", "OpeningCash", "OpeningDefaultCash", "ExpectedCashClosing", "ActualCashClosing", "CashDifference",
                "UpiDifference", "CardDifference", "ExpectedTotal", "ActualTotal", "TotalDifference", "Status", "Remarks",
                "ClosedBy", "ClosedOn", "PreparedBy", "ReviewedBy", "ReviewedOn"
            };
            foreach (string name in headColumns)
            {
                headTable.Columns.Add(name, typeof(object));
            }
            head = headTable.NewRow();
            foreach (string name in headColumns)
            {
                head[name] = DBNull.Value;
            }
            head["OpeningCash"] = opening;
            if (Convert.ToInt32(e["HasPreviousClose"]) == 1)
            {
                head["OpeningDefaultCash"] = defaultOpening;
            }
            head["ExpectedCashClosing"] = expectedCash;
            head["ExpectedTotal"] = expectedCash + expectedUpi + expectedCard;
            head["Status"] = "NOT CLOSED";
            headTable.Rows.Add(head);

            summary = new DataTable();
            foreach (string name in new string[] { "PaymentMode", "OpeningAmount", "TotalIn", "TotalOut", "ExpectedNet", "ActualAmount", "Difference", "CarriedIn", "CarriedOut" })
            {
                summary.Columns.Add(name, name == "PaymentMode" ? typeof(string) : typeof(decimal));
            }
            summary.Rows.Add("CASH", opening, Convert.ToDecimal(e["CashIn"]), Convert.ToDecimal(e["CashOut"]), expectedCash, 0m, 0m,
                Optional(e, "CashCarriedIn"), Optional(e, "CashCarriedOut"));
            summary.Rows.Add("UPI", 0m, Convert.ToDecimal(e["UpiIn"]), Convert.ToDecimal(e["UpiOut"]), expectedUpi, 0m, 0m,
                Optional(e, "UpiCarriedIn"), Optional(e, "UpiCarriedOut"));
            summary.Rows.Add("CARD", 0m, Convert.ToDecimal(e["CardIn"]), Convert.ToDecimal(e["CardOut"]), expectedCard, 0m, 0m,
                Optional(e, "CardCarriedIn"), Optional(e, "CardCarriedOut"));

            DataSet denominationSet = Load("dbo.Proc_DayCloseV2_GetDenominationRecon", date);
            denomination = denominationSet.Tables.Count > 0 ? denominationSet.Tables[0] : new DataTable();
            if (draftRow == null)
            {
                return;
            }

            Dictionary<int, int[]> counts = new Dictionary<int, int[]>();
            if (draftSet.Tables.Count > 1)
            {
                foreach (DataRow line in draftSet.Tables[1].Rows)
                {
                    counts[Convert.ToInt32(line["FaceValue"])] = new int[] { Convert.ToInt32(line["OpeningQty"]), Convert.ToInt32(line["PhysicalQty"]) };
                }
            }
            decimal actualCash = 0m;
            foreach (DataColumn column in denomination.Columns)
            {
                column.ReadOnly = false;
            }
            foreach (DataRow row in denomination.Rows)
            {
                int face = Convert.ToInt32(row["FaceValue"]);
                int[] count;
                if (counts.TryGetValue(face, out count) == false)
                {
                    count = new int[] { IntOf(row["OpeningQty"]), 0 };
                }
                row["OpeningQty"] = count[0];
                row["ExpectedQty"] = count[0] + IntOf(row["InQty"]) - IntOf(row["OutQty"]);
                row["ActualQty"] = count[1];
                actualCash += face * count[1];
            }
            decimal actualUpi = Convert.ToDecimal(draftRow["ActualUpi"]);
            decimal actualCard = Convert.ToDecimal(draftRow["ActualCard"]);
            decimal[] actuals = new decimal[] { actualCash, actualUpi, actualCard };
            for (int i = 0; i < summary.Rows.Count && i < 3; i++)
            {
                summary.Rows[i]["ActualAmount"] = actuals[i];
                summary.Rows[i]["Difference"] = actuals[i] - Convert.ToDecimal(summary.Rows[i]["ExpectedNet"]);
            }
            head["ActualCashClosing"] = actualCash;
            head["CashDifference"] = actualCash - expectedCash;
            head["UpiDifference"] = actualUpi - expectedUpi;
            head["CardDifference"] = actualCard - expectedCard;
            head["ActualTotal"] = actualCash + actualUpi + actualCard;
            head["TotalDifference"] = (actualCash + actualUpi + actualCard) - (expectedCash + expectedUpi + expectedCard);
            head["Remarks"] = Text(draftRow["Remarks"]);
        }

        private static DataTable LoadLedger(DateTime date, decimal opening)
        {
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.Proc_DayCloseV2_GetPaymentLedger", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BusinessDate", date);
                cmd.Parameters.AddWithValue("@Filter", "All");
                cmd.Parameters.AddWithValue("@OpeningCash", opening);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }

        private static DataRow LoadBranch()
        {
            string code = BranchCode();
            if (code.Length == 0)
            {
                return null;
            }
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand(
                "IF OBJECT_ID('dbo.Branches', 'U') IS NOT NULL SELECT TOP 1 location_name, AddressLine1, AddressLine2, AddressLine3, City, Pincode, GST FROM dbo.Branches WHERE branch_code = @code", con))
            {
                cmd.Parameters.AddWithValue("@code", code);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(table);
                }
            }
            return table.Rows.Count == 0 ? null : table.Rows[0];
        }

        private void DrawHeader(DataRow head, DataRow branch)
        {
            pdf.FillRect(Left, 30f, ContentWidth, 48f, TitleRed);
            pdf.Text(Left + 12f, 52f, CompanyName, 17f, true, Color.White);
            pdf.Text(Left + 12f, 69f, "DAILY CLOSING REPORT", 10f, false, Color.White);
            string code = BranchCode();
            if (code.Length > 0)
            {
                pdf.TextRight(Right - 12f, 52f, "Branch: " + code, 10f, true, Color.White);
            }
            if (branch != null)
            {
                string address = JoinParts(new string[]
                {
                    Text(branch["AddressLine1"]), Text(branch["AddressLine2"]), Text(branch["AddressLine3"]),
                    Text(branch["City"]), Text(branch["Pincode"])
                });
                pdf.TextRight(Right - 12f, 69f, pdf.Fit(address, 300f, 7.5f, false), 7.5f, false, Color.White);
            }

            y = 96f;
            string preparedBy = closed ? PreparedBy(head) : "-";
            string[,] info = new string[,]
            {
                { "Business Date", businessDate.ToString("dd-MM-yyyy") },
                { "Day Close No", closed ? "DC-" + Convert.ToInt32(head["Id"]).ToString("00000") : "-" },
                { "Day Close Status", closed ? "CLOSED" : "NOT CLOSED" },
                { "Prepared By", preparedBy.Length == 0 ? "-" : preparedBy },
                { "Closed On", closed ? DateText(head["ClosedOn"]) : "-" },
                { "Reviewed By", head["ReviewedBy"] == DBNull.Value ? "-" : Text(head["ReviewedBy"]) + " (" + DateText(head["ReviewedOn"]) + ")" },
                { "Generated On", generatedOn.ToString("dd-MM-yyyy hh:mm tt", CultureInfo.InvariantCulture) },
                { "PDF Generated By", CurrentUser() }
            };
            float half = ContentWidth / 2f;
            for (int i = 0; i < info.GetLength(0); i++)
            {
                float x = Left + ((i % 2) * half);
                if (i % 2 == 0 && i > 0)
                {
                    y += 15f;
                }
                bool statusCell = i == 2;
                pdf.Text(x, y, info[i, 0], BodySize, false, Muted);
                pdf.Text(x + 88f, y, info[i, 1], statusCell ? 10f : 9f, true, statusCell && closed == false ? Alert : Ink);
            }
            y += 10f;
            pdf.Line(Left, y, Right, y, 0.8f, Brand);
            y += 10f;
            if (closed == false)
            {
                pdf.StrokeRect(Left, y, ContentWidth, 30f, 1f, Alert);
                pdf.Text(Left + 8f, y + 12f, "DAY CLOSE STATUS: NOT CLOSED", 10f, true, Alert);
                pdf.Text(Left + 8f, y + 24f, draft
                    ? "NOT a final report. Actual values are a saved DRAFT (" + draftNote + ")."
                    : "Day Close has not been completed for this date. NOT a final report - figures are the current calculation.",
                    7.5f, true, Alert);
                y += 40f;
            }
        }

        private static string CurrentUser()
        {
            string fullName = Program.Userfullname == null ? "" : Program.Userfullname.Trim();
            if (fullName.Length > 0)
            {
                return fullName;
            }
            string user = Program.UserName == null ? "" : Program.UserName.Trim();
            return user.Length == 0 ? "-" : user;
        }

        private void DrawPaymentSummary(DataTable summary)
        {
            Section("1. PAYMENT RECONCILIATION");
            Column[] columns = new Column[]
            {
                new Column("Mode", 73f, 0), new Column("Opening", 75f, 1), new Column("IN", 75f, 1), new Column("OUT", 75f, 1),
                new Column("Expected", 75f, 1), new Column(draft ? "Actual (Draft)" : "Actual", 75f, 1), new Column("Variance", 75.28f, 1)
            };
            bool actuals = HasActuals();
            List<string[]> rows = new List<string[]>();
            List<decimal> variances = new List<decimal>();
            decimal[] totals = new decimal[6];
            foreach (DataRow row in summary.Rows)
            {
                decimal[] values = new decimal[]
                {
                    Convert.ToDecimal(row["OpeningAmount"]), Convert.ToDecimal(row["TotalIn"]), Convert.ToDecimal(row["TotalOut"]),
                    Convert.ToDecimal(row["ExpectedNet"]), Convert.ToDecimal(row["ActualAmount"]), Convert.ToDecimal(row["Difference"])
                };
                for (int i = 0; i < 6; i++)
                {
                    totals[i] += values[i];
                }
                string mode = Text(row["PaymentMode"]);
                rows.Add(new string[]
                {
                    mode, mode == "CASH" ? Amount(values[0]) : "-", Amount(values[1]), Amount(values[2]),
                    Amount(values[3]), actuals ? Amount(values[4]) : "-", actuals ? Signed(values[5]) : "-"
                });
                variances.Add(values[5]);
            }
            variances.Add(totals[5]);
            rows.Add(new string[] { "TOTAL", Amount(totals[0]), Amount(totals[1]), Amount(totals[2]), Amount(totals[3]),
                actuals ? Amount(totals[4]) : "-", actuals ? Signed(totals[5]) : "-" });
            Table(columns, rows, delegate(int r, int c) { return c == 6 && actuals ? VarianceColor(variances[r]) : Ink; }, true);
            if (draft)
            {
                Note("Actual amounts are DRAFT values entered on the Day Close screen (" + draftNote + "). Day Close is NOT CLOSED - not final.");
            }
            else if (closed == false)
            {
                Note("Actual amounts and variances are recorded only when the Day Close is completed.");
            }

            List<string> carried = new List<string>();
            foreach (DataRow row in summary.Rows)
            {
                decimal carriedIn = Optional(row, "CarriedIn");
                decimal carriedOut = Optional(row, "CarriedOut");
                if (carriedIn != 0m || carriedOut != 0m)
                {
                    carried.Add(Text(row["PaymentMode"]) + " IN " + Amount(carriedIn) + " / OUT " + Amount(carriedOut));
                }
            }
            if (carried.Count > 0)
            {
                Note("IN / OUT include late postings carried forward from earlier closed dates:  " + string.Join(";  ", carried.ToArray()));
            }
        }

        private bool HasActuals()
        {
            return closed || draft;
        }

        private static decimal Optional(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) && row[column] != DBNull.Value ? Convert.ToDecimal(row[column]) : 0m;
        }

        private void DrawCashBridge(DataRow head, DataTable summary)
        {
            decimal opening = Convert.ToDecimal(head["OpeningCash"]);
            decimal cashIn = 0m;
            decimal cashOut = 0m;
            decimal carriedIn = 0m;
            decimal carriedOut = 0m;
            foreach (DataRow row in summary.Rows)
            {
                if (Text(row["PaymentMode"]) == "CASH")
                {
                    cashIn = Convert.ToDecimal(row["TotalIn"]);
                    cashOut = Convert.ToDecimal(row["TotalOut"]);
                    carriedIn = Optional(row, "CarriedIn");
                    carriedOut = Optional(row, "CarriedOut");
                }
            }
            decimal expected = Convert.ToDecimal(head["ExpectedCashClosing"]);
            bool actuals = HasActuals();
            decimal actual = actuals ? Convert.ToDecimal(head["ActualCashClosing"]) : 0m;
            decimal variance = actuals ? Convert.ToDecimal(head["CashDifference"]) : 0m;
            Section("2. CASH BRIDGE");
            Column[] columns = new Column[] { new Column("Particulars", 200f, 0), new Column("Amount (Rs.)", 120f, 1), new Column("Note", 203.28f, 0) };
            string openingNote = "";
            if (head["OpeningDefaultCash"] != DBNull.Value)
            {
                decimal suggested = Convert.ToDecimal(head["OpeningDefaultCash"]);
                openingNote = suggested == opening ? "Previous actual closing" : "Edited from previous actual " + Amount(suggested);
            }
            List<string[]> rows = new List<string[]>();
            rows.Add(new string[] { "Opening Cash", Amount(opening), openingNote });
            rows.Add(new string[] { "Add: Cash IN", Amount(cashIn), carriedIn == 0m ? "" : "Includes carried forward " + Amount(carriedIn) });
            rows.Add(new string[] { "Less: Cash OUT", Amount(cashOut), carriedOut == 0m ? "" : "Includes carried forward " + Amount(carriedOut) });
            rows.Add(new string[] { "Expected Cash Closing", Amount(expected), "Opening + IN - OUT" });
            if (actuals)
            {
                string varianceNote = variance == 0m ? "MATCH - Balanced" : (variance > 0m ? "MISMATCH - Excess" : "MISMATCH - Short");
                rows.Add(new string[] { "Actual Physical Cash", Amount(actual), closed ? "Counted denominations" : "Draft count - not final" });
                rows.Add(new string[] { "Cash Variance", Signed(variance), closed ? varianceNote : varianceNote + " (draft)" });
            }
            else
            {
                rows.Add(new string[] { "Actual Physical Cash", "-", "Not counted - Day Close not completed" });
                rows.Add(new string[] { "Cash Variance", "-", "Not reconciled" });
            }
            Table(columns, rows, delegate(int r, int c) { return r == 5 && c > 0 && actuals ? VarianceColor(variance) : Ink; }, true);
        }

        private void DrawDenomination(DataTable denomination)
        {
            Section("3. DENOMINATION RECONCILIATION");
            Column[] columns = new Column[]
            {
                new Column("Denomination", 88f, 0), new Column("Opening", 72f, 1), new Column("IN", 72f, 1), new Column("OUT", 72f, 1),
                new Column("Expected", 72f, 1), new Column("Physical", 72f, 1), new Column("Difference", 75.28f, 1)
            };
            List<string[]> rows = new List<string[]>();
            List<decimal?> differences = new List<decimal?>();
            decimal[] values = new decimal[6];
            bool legacy = false;
            foreach (DataRow row in denomination.Rows)
            {
                int face = Convert.ToInt32(row["FaceValue"]);
                if (row["OpeningQty"] == DBNull.Value)
                {
                    legacy = true;
                }
                int opening = IntOf(row["OpeningQty"]);
                int inQty = IntOf(row["InQty"]);
                int outQty = IntOf(row["OutQty"]);
                int expected = IntOf(row["ExpectedQty"]);
                int actual = IntOf(row["ActualQty"]);
                int difference = actual - expected;
                values[0] += face * opening;
                values[1] += face * inQty;
                values[2] += face * outQty;
                values[3] += face * expected;
                values[4] += face * actual;
                values[5] += face * difference;
                if (legacy)
                {
                    rows.Add(new string[] { "Rs. " + face.ToString(), "-", "-", "-", "-", actual.ToString(), "-" });
                    differences.Add(null);
                }
                else if (HasActuals() == false)
                {
                    rows.Add(new string[] { "Rs. " + face.ToString(), opening.ToString(), inQty.ToString(), outQty.ToString(), expected.ToString(), "-", "-" });
                    differences.Add(null);
                }
                else
                {
                    rows.Add(new string[] { "Rs. " + face.ToString(), opening.ToString(), inQty.ToString(), outQty.ToString(), expected.ToString(), actual.ToString(), SignedInt(difference) });
                    differences.Add(difference);
                }
            }
            differences.Add(legacy || HasActuals() == false ? (decimal?)null : values[5]);
            if (legacy)
            {
                rows.Add(new string[] { "VALUE (Rs.)", "-", "-", "-", "-", Amount(values[4]), "-" });
            }
            else if (HasActuals() == false)
            {
                rows.Add(new string[] { "VALUE (Rs.)", Amount(values[0]), Amount(values[1]), Amount(values[2]), Amount(values[3]), "-", "-" });
            }
            else
            {
                rows.Add(new string[] { "VALUE (Rs.)", Amount(values[0]), Amount(values[1]), Amount(values[2]), Amount(values[3]), Amount(values[4]), Signed(values[5]) });
            }
            Table(columns, rows, delegate(int r, int c) { return c == 6 && differences[r].HasValue ? VarianceColor(differences[r].Value) : Ink; }, true);
            if (legacy)
            {
                Note("Opening and movement counts were not stored for this older close; only physical counts are shown.");
            }
            if (draft)
            {
                Note("Physical counts are DRAFT counts - not final.");
            }
        }

        private void DrawLedger(DataTable ledger)
        {
            Section("4. PAYMENT LEDGER");
            Column[] columns = new Column[]
            {
                new Column("Time", 44f, 0), new Column("Channel", 42f, 0), new Column("Dir", 28f, 0), new Column("Reference", 92f, 0),
                new Column("Type", 92f, 0), new Column("Description", 105.28f, 0), new Column("IN", 60f, 1), new Column("OUT", 60f, 1)
            };
            columns[5].Wrap = true;
            List<string[]> rows = new List<string[]>();
            Dictionary<string, decimal[]> channels = new Dictionary<string, decimal[]>();
            channels.Add("CASH", new decimal[2]);
            channels.Add("UPI", new decimal[2]);
            channels.Add("CARD", new decimal[2]);
            decimal totalIn = 0m;
            decimal totalOut = 0m;
            foreach (DataRow row in ledger.Rows)
            {
                decimal amountIn = Convert.ToDecimal(row["AmountIn"]);
                decimal amountOut = Convert.ToDecimal(row["AmountOut"]);
                string channel = Text(row["Channel"]);
                totalIn += amountIn;
                totalOut += amountOut;
                if (channels.ContainsKey(channel))
                {
                    channels[channel][0] += amountIn;
                    channels[channel][1] += amountOut;
                }
                string posting = ledger.Columns.Contains("Posting") ? Text(row["Posting"]) : "";
                string description = Text(row["Description"]);
                rows.Add(new string[]
                {
                    row["TransTime"] == DBNull.Value ? "" : Convert.ToDateTime(row["TransTime"]).ToString("HH:mm"),
                    channel, Text(row["Direction"]), Text(row["EntityReference"]), Text(row["EntityType"]),
                    posting.Length == 0 ? description : "[" + posting + "] " + description,
                    amountIn == 0m ? "" : Amount(amountIn), amountOut == 0m ? "" : Amount(amountOut)
                });
            }
            if (rows.Count == 0)
            {
                rows.Add(new string[] { "", "", "", "No payment transactions", "", "", "", "" });
            }
            rows.Add(new string[] { "TOTAL", "", "", ledger.Rows.Count.ToString() + " entries", "", "", Amount(totalIn), Amount(totalOut) });
            Table(columns, rows, null, true);

            Column[] totalsColumns = new Column[] { new Column("Channel", 120f, 0), new Column("Total IN", 120f, 1), new Column("Total OUT", 120f, 1), new Column("Net", 123.28f, 1) };
            List<string[]> totals = new List<string[]>();
            foreach (KeyValuePair<string, decimal[]> pair in channels)
            {
                totals.Add(new string[] { pair.Key, Amount(pair.Value[0]), Amount(pair.Value[1]), Signed(pair.Value[0] - pair.Value[1]) });
            }
            Table(totalsColumns, totals, null, false);
        }

        private void DrawPostClose(DataTable postClose)
        {
            if (postClose.Rows.Count == 0)
            {
                return;
            }
            Section("4A. POST-CLOSE TRANSACTIONS  (NOT INCLUDED IN SUBMITTED DAY CLOSE TOTALS)");
            Note("Posted to this business date after the Day Close was submitted. Reconciled in the next Day Close; the figures above are unchanged.");
            Column[] columns = new Column[]
            {
                new Column("Time", 36f, 0), new Column("Channel", 40f, 0), new Column("Dir", 26f, 0), new Column("Reference", 82f, 0),
                new Column("Type", 80f, 0), new Column("Description", 95.28f, 0), new Column("IN", 52f, 1), new Column("OUT", 52f, 1),
                new Column("Status", 60f, 0)
            };
            columns[5].Wrap = true;
            columns[8].Wrap = true;
            List<string[]> rows = new List<string[]>();
            string[] channels = new string[] { "CASH", "UPI", "CARD" };
            decimal[,] totals = new decimal[3, 2];
            bool[] present = new bool[3];
            foreach (DataRow row in postClose.Rows)
            {
                decimal amountIn = Convert.ToDecimal(row["AmountIn"]);
                decimal amountOut = Convert.ToDecimal(row["AmountOut"]);
                string channel = Text(row["Channel"]);
                int index = Array.IndexOf(channels, channel);
                if (index >= 0)
                {
                    present[index] = true;
                    totals[index, 0] += amountIn;
                    totals[index, 1] += amountOut;
                }
                rows.Add(new string[]
                {
                    row["TransTime"] == DBNull.Value ? "" : Convert.ToDateTime(row["TransTime"]).ToString("HH:mm"),
                    channel, Text(row["Direction"]), Text(row["EntityReference"]), Text(row["EntityType"]), Text(row["Description"]),
                    amountIn == 0m ? "" : Amount(amountIn), amountOut == 0m ? "" : Amount(amountOut), Text(row["Status"])
                });
            }
            Table(columns, rows, null, false);

            Column[] totalsColumns = new Column[] { new Column("Post-close channel", 120f, 0), new Column("IN", 120f, 1), new Column("OUT", 120f, 1), new Column("Net", 123.28f, 1) };
            List<string[]> totalRows = new List<string[]>();
            for (int i = 0; i < channels.Length; i++)
            {
                if (present[i])
                {
                    totalRows.Add(new string[] { channels[i], Amount(totals[i, 0]), Amount(totals[i, 1]), Signed(totals[i, 0] - totals[i, 1]) });
                }
            }
            Table(totalsColumns, totalRows, null, false);
        }

        private void DrawGoods(DataSet goods)
        {
            DataTable products = goods.Tables.Count > 0 ? goods.Tables[0] : new DataTable();
            DataTable categories = goods.Tables.Count > 1 ? goods.Tables[1] : new DataTable();

            Section("5. GOODS MOVEMENT - BY CATEGORY");
            Column[] categoryColumns = new Column[]
            {
                new Column("Category", 183.28f, 0), new Column("Goods IN", 85f, 1), new Column("Goods OUT", 85f, 1),
                new Column("Net", 85f, 1), new Column("Products", 85f, 1)
            };
            List<string[]> categoryRows = new List<string[]>();
            decimal goodsIn = 0m;
            decimal goodsOut = 0m;
            foreach (DataRow row in categories.Rows)
            {
                decimal inQty = Convert.ToDecimal(row["GoodsIn"]);
                decimal outQty = Convert.ToDecimal(row["GoodsOut"]);
                goodsIn += inQty;
                goodsOut += outQty;
                categoryRows.Add(new string[] { Text(row["Category"]), Qty(inQty), Qty(outQty), QtySigned(inQty - outQty), Text(row["Products"]) });
            }
            if (categoryRows.Count == 0)
            {
                categoryRows.Add(new string[] { "No goods movement", "", "", "", "" });
            }
            categoryRows.Add(new string[] { "TOTAL", Qty(goodsIn), Qty(goodsOut), QtySigned(goodsIn - goodsOut), "" });
            Table(categoryColumns, categoryRows, null, true);

            Section("6. GOODS MOVEMENT - PRODUCT-WISE");
            Column[] productColumns = new Column[]
            {
                new Column("Code", 60f, 0), new Column("Product", 183.28f, 0), new Column("Category", 100f, 0),
                new Column("Goods IN", 60f, 1), new Column("Goods OUT", 60f, 1), new Column("Net", 60f, 1)
            };
            List<string[]> productRows = new List<string[]>();
            foreach (DataRow row in products.Rows)
            {
                decimal inQty = Convert.ToDecimal(row["GoodsIn"]);
                decimal outQty = Convert.ToDecimal(row["GoodsOut"]);
                productRows.Add(new string[] { Text(row["ProductCode"]), Text(row["ProductName"]), Text(row["Category"]), Qty(inQty), Qty(outQty), QtySigned(inQty - outQty) });
            }
            if (productRows.Count == 0)
            {
                productRows.Add(new string[] { "", "No goods movement", "", "", "", "" });
            }
            Table(productColumns, productRows, null, false);
        }

        private void DrawVariance(DataRow head)
        {
            Section("7. VARIANCE AND REMARKS");
            if (closed == false && draft == false)
            {
                Ensure(RowHeight * 3);
                pdf.Text(Left, y + 10f, "Day Close Status:", 9f, false, Muted);
                pdf.Text(Left + 88f, y + 10f, "NOT CLOSED", 11f, true, Alert);
                pdf.Text(Left + 200f, y + 10f, "Total Expected " + Amount(Convert.ToDecimal(head["ExpectedTotal"])) + "   Actual -", 8.5f, false, Ink);
                y += 22f;
                Note("Variance is not available: actual cash, UPI and card are counted when the Day Close is completed.");
                y += 6f;
                return;
            }
            decimal cash = Convert.ToDecimal(head["CashDifference"]);
            decimal upi = Convert.ToDecimal(head["UpiDifference"]);
            decimal card = Convert.ToDecimal(head["CardDifference"]);
            decimal total = Convert.ToDecimal(head["TotalDifference"]);
            bool balanced = cash == 0m && upi == 0m && card == 0m;
            Ensure(RowHeight * 3);
            if (closed)
            {
                pdf.Text(Left, y + 10f, "Overall Status:", 9f, false, Muted);
                pdf.Text(Left + 80f, y + 10f, balanced ? "BALANCED" : "VARIANCE", 11f, true, balanced ? Good : Alert);
            }
            else
            {
                pdf.Text(Left, y + 10f, "Day Close Status:", 9f, false, Muted);
                pdf.Text(Left + 88f, y + 10f, "NOT CLOSED", 11f, true, Alert);
            }
            pdf.Text(Left + 200f, y + 10f, "Total Expected " + Amount(Convert.ToDecimal(head["ExpectedTotal"]))
                + "   Actual " + Amount(Convert.ToDecimal(head["ActualTotal"])) + (closed ? "" : " (draft)"), 8.5f, false, Ink);
            pdf.TextRight(Right, y + 10f, "Net " + Signed(total), 8.5f, true, VarianceColor(total));
            y += 22f;
            float x = Left;
            x = DrawVarianceItem(x, "Cash", cash);
            x = DrawVarianceItem(x, "UPI", upi);
            DrawVarianceItem(x, "Card", card);
            y += 14f;
            string remarks = Text(head["Remarks"]);
            List<string> lines = pdf.Wrap(remarks.Length == 0 ? "No remarks entered." : remarks, ContentWidth - 16f, 8.5f, false);
            float boxHeight = 18f + (lines.Count * 12f);
            Ensure(boxHeight + 4f);
            pdf.StrokeRect(Left, y, ContentWidth, boxHeight, 0.6f, GridLine);
            pdf.Text(Left + 8f, y + 12f, "Remarks / Variance Reason", 7.5f, true, Muted);
            float lineY = y + 24f;
            foreach (string text in lines)
            {
                pdf.Text(Left + 8f, lineY, text, 8.5f, false, Ink);
                lineY += 12f;
            }
            y += boxHeight + 12f;
        }

        private float DrawVarianceItem(float x, string label, decimal value)
        {
            string text = label + " " + Signed(value) + (value == 0m ? " (MATCH)" : " (MISMATCH)");
            pdf.Text(x, y, text, 8.5f, false, VarianceColor(value));
            return x + pdf.Width(text, 8.5f, false) + 18f;
        }

        private static Color VarianceColor(decimal value)
        {
            return value == 0m ? Good : Alert;
        }

        private void DrawSignatures(DataRow head)
        {
            Ensure(80f);
            y += 30f;
            float boxWidth = (ContentWidth - 40f) / 3f;
            string[] labels = new string[] { "Prepared By", "Verified By", "Authorized By" };
            string[] names = closed
                ? new string[] { PreparedBy(head), head["ReviewedBy"] == DBNull.Value ? "" : Text(head["ReviewedBy"]), "" }
                : new string[] { "", "", "" };
            for (int i = 0; i < 3; i++)
            {
                float x = Left + (i * (boxWidth + 20f));
                pdf.Line(x, y, x + boxWidth, y, 0.6f, Ink);
                pdf.TextCenter(x + (boxWidth / 2f), y + 12f, labels[i], 8.5f, true, Ink);
                if (names[i].Length > 0)
                {
                    pdf.TextCenter(x + (boxWidth / 2f), y + 24f, names[i], 8f, false, Muted);
                }
            }
            y += 30f;
        }

        private static string PreparedBy(DataRow head)
        {
            string saved = head.Table.Columns.Contains("PreparedBy") ? Text(head["PreparedBy"]) : "";
            if (saved.Length > 0)
            {
                return saved;
            }
            return head.Table.Columns.Contains("ClosedBy") ? Text(head["ClosedBy"]) : "";
        }

        private void DrawFooters()
        {
            int total = pdf.PageCount;
            for (int i = 0; i < total; i++)
            {
                pdf.SelectPage(i);
                pdf.Line(Left, 806f, Right, 806f, 0.5f, GridLine);
                pdf.Text(Left, 818f, CompanyName + " - Daily Closing Report  |  Business Date " + businessDate.ToString("dd-MM-yyyy"), 7f, false, Muted);
                pdf.TextRight(Right, 818f, "Page " + (i + 1).ToString() + " of " + total.ToString(), 7f, true, Muted);
                pdf.Text(Left, 828f, "Generated On " + generatedOn.ToString("dd-MM-yyyy HH:mm"), 7f, false, Muted);
                pdf.TextRight(Right, 828f, "Day Close Status: " + (closed ? "CLOSED" : "NOT CLOSED"), 7f, true, closed ? Muted : Alert);
            }
        }

        private void Section(string title)
        {
            Ensure(22f + (RowHeight * 3));
            y += 6f;
            pdf.FillRect(Left, y, ContentWidth, 16f, SectionFill);
            pdf.FillRect(Left, y, 3f, 16f, Brand);
            pdf.Text(Left + 8f, y + 11.5f, title, 9.5f, true, Brand);
            y += 20f;
        }

        private void Note(string text)
        {
            foreach (string line in pdf.Wrap(text, ContentWidth, 7f, false))
            {
                Ensure(12f);
                pdf.Text(Left, y + 8f, line, 7f, false, Muted);
                y += 12f;
            }
        }

        private void Ensure(float height)
        {
            if (y + height <= Bottom)
            {
                return;
            }
            pdf.NewPage();
            pdf.Text(Left, 30f, CompanyName + " - DAILY CLOSING REPORT", 9f, true, TitleRed);
            pdf.TextRight(Right, 30f, "Business Date " + businessDate.ToString("dd-MM-yyyy"), 8f, false, Muted);
            pdf.Line(Left, 36f, Right, 36f, 0.6f, Brand);
            y = 46f;
            if (currentColumns != null)
            {
                DrawTableHeader(currentColumns);
            }
        }

        private void Table(Column[] columns, List<string[]> rows, CellColor color, bool lastIsTotal)
        {
            currentColumns = null;
            Ensure(RowHeight * 2);
            DrawTableHeader(columns);
            currentColumns = columns;
            for (int r = 0; r < rows.Count; r++)
            {
                bool total = lastIsTotal && r == rows.Count - 1;
                List<string>[] wrapped = new List<string>[columns.Length];
                float height = RowHeight;
                for (int c = 0; c < columns.Length; c++)
                {
                    string value = c < rows[r].Length ? rows[r][c] : "";
                    if (columns[c].Wrap && value.Length > 0)
                    {
                        wrapped[c] = pdf.Wrap(value, columns[c].Width - 8f, BodySize, total);
                        height = Math.Max(height, 4f + (wrapped[c].Count * WrapLineHeight));
                    }
                }
                Ensure(height);
                if (total)
                {
                    pdf.FillRect(Left, y, ContentWidth, height, TotalFill);
                }
                else if (r % 2 == 1)
                {
                    pdf.FillRect(Left, y, ContentWidth, height, StripeFill);
                }
                float x = Left;
                for (int c = 0; c < columns.Length; c++)
                {
                    string value = c < rows[r].Length ? rows[r][c] : "";
                    Color ink = color == null ? Ink : color(r, c);
                    if (wrapped[c] != null)
                    {
                        for (int i = 0; i < wrapped[c].Count; i++)
                        {
                            string line = pdf.Fit(wrapped[c][i], columns[c].Width - 8f, BodySize, total);
                            pdf.Text(x + 4f, y + 10f + (i * WrapLineHeight), line, BodySize, total, ink);
                        }
                    }
                    else
                    {
                        DrawCell(x, columns[c], value, total, ink);
                    }
                    x += columns[c].Width;
                }
                y += height;
                pdf.Line(Left, y, Right, y, total ? 0.8f : 0.3f, total ? Ink : GridLine);
            }
            currentColumns = null;
            y += 8f;
        }

        private void DrawTableHeader(Column[] columns)
        {
            pdf.FillRect(Left, y, ContentWidth, RowHeight + 2f, HeaderFill);
            float x = Left;
            foreach (Column column in columns)
            {
                DrawCell(x, column, column.Title, true, Brand);
                x += column.Width;
            }
            y += RowHeight + 2f;
        }

        private void DrawCell(float x, Column column, string value, bool bold, Color ink)
        {
            float padding = 4f;
            string text = pdf.Fit(value, column.Width - (padding * 2f), BodySize, bold);
            float baseline = y + 10f;
            if (column.Align == 1)
            {
                pdf.TextRight(x + column.Width - padding, baseline, text, BodySize, bold, ink);
            }
            else if (column.Align == 2)
            {
                pdf.TextCenter(x + (column.Width / 2f), baseline, text, BodySize, bold, ink);
            }
            else
            {
                pdf.Text(x + padding, baseline, text, BodySize, bold, ink);
            }
        }

        private static string JoinParts(string[] parts)
        {
            List<string> kept = new List<string>();
            foreach (string part in parts)
            {
                string trimmed = part.Trim().TrimEnd(',').Trim();
                if (trimmed.Length > 0)
                {
                    kept.Add(trimmed);
                }
            }
            return string.Join(", ", kept.ToArray());
        }

        private static string Text(object value)
        {
            return value == null || value == DBNull.Value ? "" : Convert.ToString(value).Trim();
        }

        private static string DateText(object value)
        {
            return value == null || value == DBNull.Value ? "-" : Convert.ToDateTime(value).ToString("dd-MM-yyyy HH:mm");
        }

        private static int IntOf(object value)
        {
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        private static string Amount(decimal value)
        {
            return value.ToString("N2", Money);
        }

        private static string Signed(decimal value)
        {
            return value > 0m ? "+" + Amount(value) : Amount(value);
        }

        private static string SignedInt(int value)
        {
            return value > 0 ? "+" + value.ToString() : value.ToString();
        }

        private static string Qty(decimal value)
        {
            return value.ToString("#,##0.###", Money);
        }

        private static string QtySigned(decimal value)
        {
            return value > 0m ? "+" + Qty(value) : Qty(value);
        }
    }
}
