using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Inventory.Tool
{
    public class ProductUpdatedForm : Form
    {
        private readonly DataGridView grid;
        private readonly Button acceptAllButton;
        private readonly Button refreshButton;
        private readonly Button closeButton;
        private readonly Label statusLabel;
        private List<ProductUpdatedPendingItem> pendingItems;
        private bool refreshInProgress;
        private readonly bool isSalemMonitor;

        public event EventHandler PendingChanged;

        public ProductUpdatedForm()
        {
            Text = "Product Updated";
            isSalemMonitor = BranchAccess.IsMainOffice;
            Width = 920;
            Height = 520;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(236, 240, 245);

            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 48;
            topPanel.Padding = new Padding(10);
            topPanel.BackColor = Color.White;

            acceptAllButton = new Button();
            acceptAllButton.Text = "Accept All";
            acceptAllButton.Location = new Point(10, 10);
            acceptAllButton.Width = 95;
            acceptAllButton.Visible = !isSalemMonitor;
            acceptAllButton.Click += delegate { AcceptAll(); };

            refreshButton = new Button();
            refreshButton.Text = "Refresh";
            refreshButton.Location = isSalemMonitor ? new Point(10, 10) : new Point(115, 10);
            refreshButton.Width = 85;
            refreshButton.Click += delegate { LoadPendingAsync(); };

            closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.Location = isSalemMonitor ? new Point(105, 10) : new Point(210, 10);
            closeButton.Width = 75;
            closeButton.Click += delegate { Close(); };

            statusLabel = new Label();
            statusLabel.AutoSize = false;
            statusLabel.Location = isSalemMonitor ? new Point(200, 14) : new Point(305, 14);
            statusLabel.Size = isSalemMonitor ? new Size(665, 22) : new Size(560, 22);
            statusLabel.ForeColor = Color.FromArgb(70, 70, 70);

            topPanel.Controls.Add(acceptAllButton);
            topPanel.Controls.Add(refreshButton);
            topPanel.Controls.Add(closeButton);
            topPanel.Controls.Add(statusLabel);

            grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.DataBindingComplete += grid_DataBindingComplete;

            Controls.Add(grid);
            Controls.Add(topPanel);

            pendingItems = new List<ProductUpdatedPendingItem>();
            Load += delegate { LoadPendingAsync(); };
        }

        private void LoadPendingAsync()
        {
            if (refreshInProgress)
            {
                return;
            }

            refreshInProgress = true;
            SetBusy(true, "Loading pending product updates...");

            ThreadPool.QueueUserWorkItem(delegate
            {
                ProductUpdatedReadResult result = ProductUpdatedSyncClient.FetchPendingForCurrentBranch();
                BeginInvoke(new MethodInvoker(delegate
                {
                    refreshInProgress = false;
                    SetBusy(false, string.Empty);

                    if (!result.Success)
                    {
                        pendingItems = new List<ProductUpdatedPendingItem>();
                        BindGrid();
                        statusLabel.Text = "Salem unavailable: " + result.Message;
                        RaisePendingChanged();
                        return;
                    }

                    pendingItems = result.Items ?? new List<ProductUpdatedPendingItem>();
                    BindGrid();
                    statusLabel.Text = pendingItems.Count == 0
                        ? "No pending product updates."
                        : pendingItems.Count + " pending product update(s).";
                    RaisePendingChanged();
                }));
            });
        }

        private void BindGrid()
        {
            DataTable table = new DataTable();
            table.Columns.Add("QueueId", typeof(int));
            if (isSalemMonitor)
            {
                table.Columns.Add("Branch");
            }
            table.Columns.Add("ProductId");
            table.Columns.Add("DisplayName");
            table.Columns.Add("SalesPrice");
            table.Columns.Add("MRP");
            table.Columns.Add("GST");
            table.Columns.Add("Status");
            table.Columns.Add("AttemptCount", typeof(int));
            table.Columns.Add("LocalStatus");
            table.Columns.Add("LastError");

            foreach (ProductUpdatedPendingItem item in pendingItems)
            {
                DataRow row = table.NewRow();
                row["QueueId"] = item.QueueId;
                if (isSalemMonitor)
                {
                    row["Branch"] = GetFriendlyBranchName(item.TargetBranchCode);
                }
                row["ProductId"] = item.ProductId;
                row["DisplayName"] = item.DisplayName;
                row["SalesPrice"] = item.SalesPrice ?? DBNull.Value;
                row["MRP"] = item.MRP ?? DBNull.Value;
                row["GST"] = item.GST ?? DBNull.Value;
                row["Status"] = item.Status;
                row["AttemptCount"] = item.AttemptCount;
                row["LocalStatus"] = item.LocalStatus;
                row["LastError"] = item.LastError;
                table.Rows.Add(row);
            }

            grid.DataSource = table;
            if (grid.Columns.Contains("QueueId"))
            {
                grid.Columns["QueueId"].Visible = false;
            }
            if (grid.Columns.Contains("LastError"))
            {
                grid.Columns["LastError"].Visible = false;
            }
        }

        private void grid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (!grid.Columns.Contains("LastError"))
            {
                return;
            }

            foreach (DataGridViewRow row in grid.Rows)
            {
                string lastError = Convert.ToString(row.Cells["LastError"].Value);
                if (!string.IsNullOrEmpty(lastError))
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.ToolTipText = lastError;
                    }
                }
            }
        }

        private void AcceptAll()
        {
            if (isSalemMonitor)
            {
                return;
            }

            if (pendingItems.Count == 0)
            {
                MessageBox.Show("No pending product updates.");
                return;
            }

            SetBusy(true, "Accepting product updates...");
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += acceptWorker_DoWork;
            worker.ProgressChanged += acceptWorker_ProgressChanged;
            worker.RunWorkerCompleted += acceptWorker_RunWorkerCompleted;
            worker.RunWorkerAsync(new List<ProductUpdatedPendingItem>(pendingItems));
        }

        private void acceptWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<ProductUpdatedPendingItem> items = (List<ProductUpdatedPendingItem>)e.Argument;
            ProductUpdatedAcceptSummary summary = new ProductUpdatedAcceptSummary();
            BackgroundWorker worker = (BackgroundWorker)sender;

            foreach (ProductUpdatedPendingItem item in items)
            {
                ProductUpdatedApplyResult result = ProductUpdatedSyncClient.AcceptOne(item);
                if (result.LocalUpdated && result.Acknowledged)
                {
                    summary.Updated++;
                    worker.ReportProgress(0, new object[] { item.QueueId, "Synced" });
                }
                else if (result.LocalUpdated && !result.Acknowledged)
                {
                    summary.AcknowledgementPending++;
                    worker.ReportProgress(0, new object[] { item.QueueId, result.Message });
                }
                else
                {
                    summary.Failed++;
                    worker.ReportProgress(0, new object[] { item.QueueId, result.Message });
                }
            }

            e.Result = summary;
        }

        private void acceptWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object[] state = e.UserState as object[];
            if (state == null || state.Length < 2)
            {
                return;
            }

            int queueId = Convert.ToInt32(state[0]);
            string message = Convert.ToString(state[1]);
            foreach (ProductUpdatedPendingItem item in pendingItems)
            {
                if (item.QueueId == queueId)
                {
                    item.LocalStatus = message;
                    break;
                }
            }

            BindGrid();
        }

        private void acceptWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetBusy(false, string.Empty);

            ProductUpdatedAcceptSummary summary = e.Result as ProductUpdatedAcceptSummary;
            if (summary == null)
            {
                MessageBox.Show("Accept All completed, but the summary was not available.");
            }
            else
            {
                MessageBox.Show(
                    "Updated: " + summary.Updated +
                    Environment.NewLine + "Conflicts/Failed: " + summary.Failed +
                    Environment.NewLine + "Acknowledgement Pending: " + summary.AcknowledgementPending,
                    "Accept All");
            }

            LoadPendingAsync();
        }

        private void SetBusy(bool busy, string message)
        {
            acceptAllButton.Enabled = !busy && !isSalemMonitor;
            refreshButton.Enabled = !busy;
            statusLabel.Text = message;
        }

        private string GetFriendlyBranchName(string branchCode)
        {
            if (branchCode == "RR-NAMAKKAL")
            {
                return "Namakkal";
            }

            if (branchCode == "RR-KOLATHUR")
            {
                return "Kolathur";
            }

            return branchCode;
        }

        private void RaisePendingChanged()
        {
            if (PendingChanged != null)
            {
                PendingChanged(this, EventArgs.Empty);
            }
        }
    }
}
