using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Tool
{
    public class ProductSyncQueue : Form
    {
        private readonly DataGridView grid;
        private readonly ComboBox statusFilter;
        private readonly Button refreshButton;
        private readonly Button pushSelectedButton;
        private readonly Button pushPendingButton;

        public ProductSyncQueue()
        {
            Text = "Product Sync Queue";
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(236, 240, 245);

            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 48;
            topPanel.Padding = new Padding(10);
            topPanel.BackColor = Color.White;

            Label statusLabel = new Label();
            statusLabel.Text = "Status";
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(10, 16);

            statusFilter = new ComboBox();
            statusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            statusFilter.Items.AddRange(new object[] { "Pending", "Failed", "Synced", "All" });
            statusFilter.SelectedIndex = 3;
            statusFilter.Location = new Point(60, 12);
            statusFilter.Width = 120;
            statusFilter.SelectedIndexChanged += delegate { LoadQueue(); };

            refreshButton = new Button();
            refreshButton.Text = "Refresh";
            refreshButton.Location = new Point(195, 10);
            refreshButton.Width = 90;
            refreshButton.Click += delegate { LoadQueue(); };

            pushSelectedButton = new Button();
            pushSelectedButton.Text = "Push Selected";
            pushSelectedButton.Location = new Point(300, 10);
            pushSelectedButton.Width = 110;
            pushSelectedButton.Click += delegate { PushSelected(); };

            pushPendingButton = new Button();
            pushPendingButton.Text = "Push All Pending";
            pushPendingButton.Location = new Point(425, 10);
            pushPendingButton.Width = 125;
            pushPendingButton.Click += delegate { PushPending(); };

            topPanel.Controls.Add(statusLabel);
            topPanel.Controls.Add(statusFilter);
            topPanel.Controls.Add(refreshButton);
            topPanel.Controls.Add(pushSelectedButton);
            topPanel.Controls.Add(pushPendingButton);

            grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            grid.BackgroundColor = Color.White;

            Controls.Add(grid);
            Controls.Add(topPanel);

            Load += delegate { LoadQueue(); };
        }

        private void LoadQueue()
        {
            try
            {
                grid.DataSource = ProductMasterCloudQueue.GetQueue(Convert.ToString(statusFilter.SelectedItem));
                if (grid.Columns.Contains("QueueId"))
                {
                    grid.Columns["QueueId"].Visible = false;
                }
                if (grid.Columns.Contains("LastError"))
                {
                    grid.Columns["LastError"].Width = 360;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load product sync queue." + Environment.NewLine + ex.Message);
            }
        }

        private void PushSelected()
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select one or more queue rows.");
                return;
            }

            int success = 0;
            int failed = 0;

            foreach (DataGridViewRow row in grid.SelectedRows)
            {
                if (row.Cells["QueueId"].Value == null)
                {
                    continue;
                }

                ProductCloudSyncResult result = ProductMasterCloudQueue.PushQueueId(Convert.ToInt32(row.Cells["QueueId"].Value));
                if (result.Success)
                {
                    success++;
                }
                else
                {
                    failed++;
                }
            }

            LoadQueue();
            MessageBox.Show("Push completed. Success: " + success + ", Failed: " + failed);
        }

        private void PushPending()
        {
            try
            {
                int success = ProductMasterCloudQueue.PushPending();
                LoadQueue();
                MessageBox.Show("Pending/failed push completed. Success: " + success + ". Failed items remain in queue.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to push pending products." + Environment.NewLine + ex.Message);
            }
        }
    }
}
