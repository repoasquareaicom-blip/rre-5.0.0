using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Inventory.Report
{
    public class HsnCorrectionDialog : Form
    {
        private readonly HsnSuggestionService suggestionService;
        private readonly HsnLookupRequest lookupRequest;
        private readonly HsnSuggestion initialSuggestion;
        private readonly Label lblProduct = new Label();
        private readonly Label lblCurrentHsn = new Label();
        private readonly TextBox txtSuggestedHsn = new TextBox();
        private readonly Label lblSimpleStatus = new Label();
        private readonly ToolTip toolTip = new ToolTip();
        private readonly Button btnFindOnline = new Button();
        private readonly Button btnUseCopied = new Button();
        private readonly Button btnUpdate = new Button();
        private readonly Button btnCancel = new Button();
        private readonly Label lblLookupStatus = new Label();
        private readonly Button btnOpenBrowser = new Button();
        private string currentSearchUrl = string.Empty;
        private string currentSource = "Manual";
        private HsnSuggestion cachedOnlineSuggestion;
        private bool lookupInProgress;
        private bool populatingSuggestion;
        private bool chromeOpenedForDialog;

        public HsnCorrectionDialog(HsnLookupRequest request, HsnSuggestion suggestion, HsnSuggestionService service)
        {
            lookupRequest = request ?? new HsnLookupRequest();
            initialSuggestion = suggestion ?? new HsnSuggestion();
            suggestionService = service ?? new HsnSuggestionService();
            ProductDisplayName = lookupRequest.ProductName ?? string.Empty;
            CurrentHsn = lookupRequest.CurrentHsn ?? string.Empty;
            Suggestion = initialSuggestion;

            InitializeComponent();
            BindValues();
        }

        public string ProductDisplayName { get; private set; }
        public string CurrentHsn { get; private set; }
        public HsnSuggestion Suggestion { get; private set; }
        public string SelectedHsn { get; private set; }

        private void InitializeComponent()
        {
            Text = "HSN Correction";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(1000, 150);
            Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            BackColor = Color.White;

            BuildTopPanel(this);

            AcceptButton = btnUpdate;
            CancelButton = btnCancel;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            OpenSearchInChrome();
        }

        private void BuildTopPanel(Control top)
        {
            top.BackColor = Color.White;

            Label productCaption = CreateCaption("Product:");
            Label currentCaption = CreateCaption("Current HSN:");
            Label suggestedCaption = CreateCaption("Suggested HSN:");
            Label statusCaption = CreateCaption("Status:");

            lblProduct.AutoEllipsis = true;
            lblProduct.Font = new Font(Font, FontStyle.Bold);
            txtSuggestedHsn.MaxLength = 8;
            txtSuggestedHsn.TextChanged += new EventHandler(txtSuggestedHsn_TextChanged);
            lblSimpleStatus.AutoEllipsis = true;
            lblLookupStatus.ForeColor = Color.SteelBlue;

            ConfigureButton(btnFindOnline, "Find HSN with OpenAI", 154);
            btnFindOnline.Click += new EventHandler(btnFindOnline_Click);
            ConfigureButton(btnUseCopied, "Use Copied HSN", 122);
            btnUseCopied.Click += new EventHandler(btnUseCopied_Click);
            ConfigureButton(btnUpdate, "Update HSN", 100);
            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            ConfigureButton(btnOpenBrowser, "Open in Chrome", 118);
            btnOpenBrowser.Click += new EventHandler(btnOpenBrowser_Click);
            btnCancel.Text = "Cancel";
            btnCancel.Width = 82;
            btnCancel.Height = 28;
            btnCancel.DialogResult = DialogResult.Cancel;

            top.Controls.Add(productCaption);
            top.Controls.Add(lblProduct);
            top.Controls.Add(currentCaption);
            top.Controls.Add(lblCurrentHsn);
            top.Controls.Add(suggestedCaption);
            top.Controls.Add(txtSuggestedHsn);
            top.Controls.Add(statusCaption);
            top.Controls.Add(lblSimpleStatus);
            top.Controls.Add(btnFindOnline);
            top.Controls.Add(btnUseCopied);
            top.Controls.Add(btnUpdate);
            top.Controls.Add(btnOpenBrowser);
            top.Controls.Add(btnCancel);
            top.Controls.Add(lblLookupStatus);

            top.Resize += delegate { LayoutTopControls(top, productCaption, currentCaption, suggestedCaption, statusCaption); };
            LayoutTopControls(top, productCaption, currentCaption, suggestedCaption, statusCaption);
        }

        private void LayoutTopControls(Control top, Label productCaption, Label currentCaption, Label suggestedCaption, Label statusCaption)
        {
            int left = 18;
            int right = Math.Max(left + 900, top.ClientSize.Width - 18);
            int row1 = 16;
            int row2 = 50;
            int row3 = 84;
            int h = 24;

            productCaption.SetBounds(left, row1, 62, h);
            lblProduct.SetBounds(left + 66, row1, right - left - 66, h);

            currentCaption.SetBounds(left, row2, 88, h);
            lblCurrentHsn.SetBounds(left + 92, row2, 125, h);

            suggestedCaption.SetBounds(left + 230, row2, 105, h);
            txtSuggestedHsn.SetBounds(left + 338, row2 - 1, 112, h);

            statusCaption.SetBounds(left + 475, row2, 54, h);
            lblSimpleStatus.SetBounds(left + 532, row2, Math.Max(280, right - left - 532), h);

            btnFindOnline.SetBounds(left, row3 - 1, 154, 28);
            btnUseCopied.SetBounds(left + 164, row3 - 1, 122, 28);
            btnUpdate.SetBounds(left + 296, row3 - 1, 100, 28);
            btnOpenBrowser.SetBounds(left + 406, row3 - 1, 118, 28);
            btnCancel.SetBounds(left + 534, row3 - 1, 82, 28);
            lblLookupStatus.SetBounds(left + 628, row3, 180, h);
        }

        private void BindValues()
        {
            lblProduct.Text = ProductDisplayName;
            lblCurrentHsn.Text = string.IsNullOrEmpty(CurrentHsn.Trim()) ? "[ blank ]" : CurrentHsn.Trim();
            ApplySuggestion(initialSuggestion, true);
        }

        private string BuildDefaultWebQuery()
        {
            string productName = (ProductDisplayName ?? string.Empty).Trim();
            if (productName.Length == 0) productName = "product";
            return productName + " HSN GST India";
        }

        private void ApplySuggestion(HsnSuggestion suggestion, bool overwriteText)
        {
            Suggestion = suggestion ?? new HsnSuggestion();
            populatingSuggestion = true;
            try
            {
                if (overwriteText)
                {
                    txtSuggestedHsn.Text = Suggestion.HsnCode ?? string.Empty;
                }
            }
            finally
            {
                populatingSuggestion = false;
            }

            currentSource = string.IsNullOrEmpty(Suggestion.Source) ? "Manual" : Suggestion.Source;
            SetSimpleStatus(Suggestion);
        }

        private void SetSimpleStatus(HsnSuggestion suggestion)
        {
            string hsn = txtSuggestedHsn.Text.Trim();
            string confidence = suggestion == null ? string.Empty : suggestion.Confidence ?? string.Empty;
            bool found = HsnSuggestionService.IsValidHsn(hsn) && !string.Equals(confidence, "No Match", StringComparison.OrdinalIgnoreCase);
            lblSimpleStatus.Text = found ? "Found" : "Not Found";
        }

        private void btnFindOnline_Click(object sender, EventArgs e)
        {
            StartOnlineLookup();
        }

        private void StartOnlineLookup()
        {
            if (lookupInProgress)
            {
                return;
            }

            if (cachedOnlineSuggestion != null)
            {
                DialogResult confirm = MessageBox.Show(
                    "An OpenAI suggestion has already been retrieved for this product.\n\nRunning the lookup again will make another API request.\n\nContinue?",
                    "OpenAI HSN Lookup",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }
            }

            SetLookupBusy(true);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync(lookupRequest);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = suggestionService.GetOnlineSuggestion(e.Argument as HsnLookupRequest);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetLookupBusy(false);

            HsnSuggestion suggestion = e.Error == null ? e.Result as HsnSuggestion : null;
            if (suggestion == null)
            {
                suggestion = new HsnSuggestion
                {
                    HsnCode = string.Empty,
                    Category = string.Empty,
                    Confidence = "No Match",
                    Source = "OpenAI",
                    Reason = "Unable to get an OpenAI HSN suggestion. You can try again or enter/search the HSN manually.",
                    NeedsReview = true
                };
            }

            cachedOnlineSuggestion = suggestion;
            ApplySuggestion(suggestion, true);
        }

        private void SetLookupBusy(bool busy)
        {
            lookupInProgress = busy;
            btnFindOnline.Enabled = !busy;
            btnUpdate.Enabled = !busy;
            lblLookupStatus.Text = busy ? "Finding HSN..." : string.Empty;
        }

        private void btnOpenBrowser_Click(object sender, EventArgs e)
        {
            try
            {
                OpenSearchInChrome();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open Chrome. " + ex.Message);
            }
        }

        private void btnUseCopied_Click(object sender, EventArgs e)
        {
            string clipboardText;
            try
            {
                if (!Clipboard.ContainsText())
                {
                    MessageBox.Show("Clipboard does not contain text.");
                    return;
                }

                clipboardText = Clipboard.GetText();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to read clipboard. " + ex.Message);
                return;
            }

            List<string> candidates = ExtractHsnCandidates(clipboardText);
            if (candidates.Count == 0)
            {
                MessageBox.Show("No valid 4, 6 or 8 digit HSN code was found in the clipboard.");
                return;
            }

            string selected = candidates.Count == 1 ? candidates[0] : SelectHsnCandidate(candidates);
            if (string.IsNullOrEmpty(selected))
            {
                return;
            }

            SetSuggestedHsnFromExternal(selected, "Google / Web Search", "Manual Verification", "Copied HSN selected after web search review.");
        }

        private static List<string> ExtractHsnCandidates(string text)
        {
            Dictionary<string, string> unique = new Dictionary<string, string>();
            MatchCollection matches = Regex.Matches(text ?? string.Empty, @"(?<!\d)\d{4}(?:\d{2})?(?:\d{2})?(?!\d)");
            foreach (Match match in matches)
            {
                string value = match.Value;
                if (HsnSuggestionService.IsValidHsn(value) && !unique.ContainsKey(value))
                {
                    unique.Add(value, value);
                }
            }

            List<string> values = new List<string>(unique.Values);
            values.Sort(delegate(string left, string right)
            {
                int lengthCompare = right.Length.CompareTo(left.Length);
                return lengthCompare != 0 ? lengthCompare : string.Compare(left, right, StringComparison.Ordinal);
            });

            if (values.Count > 1 && values[0].Length > values[1].Length)
            {
                return new List<string> { values[0] };
            }

            return values;
        }

        private string SelectHsnCandidate(List<string> candidates)
        {
            using (Form dialog = new Form())
            using (ListBox list = new ListBox())
            using (Button ok = new Button())
            using (Button cancel = new Button())
            {
                dialog.Text = "Select HSN";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ClientSize = new Size(260, 220);

                list.SetBounds(12, 12, 236, 150);
                list.Items.AddRange(candidates.ToArray());
                if (list.Items.Count > 0) list.SelectedIndex = 0;

                ok.Text = "Use HSN";
                ok.SetBounds(58, 176, 88, 28);
                ok.DialogResult = DialogResult.OK;
                cancel.Text = "Cancel";
                cancel.SetBounds(154, 176, 80, 28);
                cancel.DialogResult = DialogResult.Cancel;

                dialog.Controls.Add(list);
                dialog.Controls.Add(ok);
                dialog.Controls.Add(cancel);
                dialog.AcceptButton = ok;
                dialog.CancelButton = cancel;

                return dialog.ShowDialog(this) == DialogResult.OK && list.SelectedItem != null
                    ? Convert.ToString(list.SelectedItem)
                    : string.Empty;
            }
        }

        private void SetSuggestedHsnFromExternal(string hsn, string source, string confidence, string reason)
        {
            string existing = txtSuggestedHsn.Text.Trim();
            if (existing.Length > 0 && !string.Equals(existing, hsn, StringComparison.Ordinal))
            {
                DialogResult replace = MessageBox.Show("Suggested HSN already contains another value. Replace it?", "Use Copied HSN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (replace != DialogResult.Yes)
                {
                    return;
                }
            }

            ApplySuggestion(new HsnSuggestion
            {
                HsnCode = hsn,
                Source = source,
                Confidence = confidence,
                Reason = reason,
                NeedsReview = true
            }, true);
        }

        private void txtSuggestedHsn_TextChanged(object sender, EventArgs e)
        {
            if (populatingSuggestion)
            {
                return;
            }

            string source = string.IsNullOrEmpty(currentSource) ? "Manual" : currentSource;
            if (source == "OpenAI")
            {
                source = "OpenAI + Manual Edit";
            }
            else if (source == "Google / Web Search")
            {
                source = "Web Search + Manual Edit";
            }
            else if (source != "OpenAI + Manual Edit" && source != "Web Search + Manual Edit")
            {
                source = "Manual";
            }

            currentSource = source;
            lblSimpleStatus.Text = HsnSuggestionService.IsValidHsn(txtSuggestedHsn.Text.Trim()) ? "Found" : "Not Found";
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string hsn = txtSuggestedHsn.Text.Trim();
            if (!HsnSuggestionService.IsValidHsn(hsn))
            {
                MessageBox.Show("Please enter a valid 4, 6 or 8 digit numeric HSN code.");
                txtSuggestedHsn.Focus();
                return;
            }

            SelectedHsn = hsn;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ConfigureButton(Button button, string text, int width)
        {
            button.Text = text;
            button.Width = width;
            button.Height = 28;
            button.Margin = new Padding(0, 2, 7, 2);
            button.BackColor = Color.SteelBlue;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
        }

        private void AddCaption(TableLayoutPanel table, string text, int column, int row)
        {
            Label label = new Label();
            label.Text = text;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            table.Controls.Add(label, column, row);
        }

        private Label CreateCaption(string text)
        {
            Label label = new Label();
            label.Text = text;
            label.TextAlign = ContentAlignment.MiddleLeft;
            return label;
        }

        private void OpenSearchInChrome()
        {
            if (chromeOpenedForDialog)
            {
                return;
            }

            chromeOpenedForDialog = true;
            string url = "https://www.google.com/search?udm=50&q=" + HttpUtility.UrlEncode(BuildDefaultWebQuery());
            currentSearchUrl = url;
            string chromePath = FindChromePath();
            if (chromePath.Length > 0)
            {
                Process.Start(chromePath, url);
                return;
            }

            try
            {
                Process.Start("chrome.exe", url);
                return;
            }
            catch
            {
                // Fall through to the default browser only if Chrome is not registered or on PATH.
            }

            Process.Start(url);
        }

        private static string FindChromePath()
        {
            string registryPath = FindChromePathFromRegistry();
            if (registryPath.Length > 0)
            {
                return registryPath;
            }

            string[] paths = new string[]
            {
                CombinePath(Environment.GetEnvironmentVariable("ProgramW6432"), @"Google\Chrome\Application\chrome.exe"),
                CombinePath(Environment.GetEnvironmentVariable("ProgramFiles"), @"Google\Chrome\Application\chrome.exe"),
                CombinePath(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), @"Google\Chrome\Application\chrome.exe"),
                CombinePath(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\Application\chrome.exe")
            };

            for (int i = 0; i < paths.Length; i++)
            {
                if (File.Exists(paths[i]))
                {
                    return paths[i];
                }
            }

            return string.Empty;
        }

        private static string FindChromePathFromRegistry()
        {
            string[] keys = new string[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe"
            };

            for (int i = 0; i < keys.Length; i++)
            {
                string path = ReadChromeRegistryValue(Registry.CurrentUser, keys[i]);
                if (path.Length > 0) return path;

                path = ReadChromeRegistryValue(Registry.LocalMachine, keys[i]);
                if (path.Length > 0) return path;
            }

            return string.Empty;
        }

        private static string ReadChromeRegistryValue(RegistryKey root, string keyName)
        {
            try
            {
                using (RegistryKey key = root.OpenSubKey(keyName))
                {
                    if (key == null) return string.Empty;
                    string path = Convert.ToString(key.GetValue(string.Empty));
                    return !string.IsNullOrEmpty(path) && File.Exists(path) ? path : string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string CombinePath(string root, string relativePath)
        {
            return string.IsNullOrEmpty(root) ? string.Empty : Path.Combine(root, relativePath);
        }
    }
}
