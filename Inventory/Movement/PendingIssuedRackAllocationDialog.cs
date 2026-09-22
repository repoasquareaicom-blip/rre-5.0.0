using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Sales
{
    public class PendingIssuedRackAllocationDialog : Form
    {
        private readonly DataTable racks;
        private readonly decimal remainingPending;
        private readonly string productName;
        private readonly Dictionary<int, decimal> initialAllocations;
        private Label lblProduct;
        private Label lblIssueQuantity;
        private Label lblAllocated;
        private Label lblBalance;
        private DataGridView dgvRacks;
        private Button btnOK;
        private Button btnCancel;
        private bool quantityValidationFailed;
        private bool quantityNavigationPending;
        private int quantityNavigationVersion;

        public Dictionary<int, decimal> Allocations;
        public decimal TotalAllocated;

        public PendingIssuedRackAllocationDialog(string productName, decimal remainingPending, DataTable racks)
            : this(productName, remainingPending, racks, null)
        {
        }

        public PendingIssuedRackAllocationDialog(string productName, decimal remainingPending, DataTable racks, Dictionary<int, decimal> initialAllocations)
        {
            this.productName = productName;
            this.remainingPending = remainingPending;
            this.racks = racks;
            this.initialAllocations = initialAllocations == null ? new Dictionary<int, decimal>() : new Dictionary<int, decimal>(initialAllocations);
            Allocations = new Dictionary<int, decimal>();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            lblProduct = new Label();
            lblIssueQuantity = new Label();
            lblAllocated = new Label();
            lblBalance = new Label();
            dgvRacks = new DataGridView();
            btnOK = new Button();
            btnCancel = new Button();

            SuspendLayout();

            lblProduct.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblProduct.AutoEllipsis = true;
            lblProduct.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblProduct.Location = new Point(12, 9);
            lblProduct.Size = new Size(610, 20);

            lblIssueQuantity.AutoSize = true;
            lblIssueQuantity.Location = new Point(12, 35);

            lblAllocated.AutoSize = true;
            lblAllocated.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblAllocated.Location = new Point(190, 35);

            lblBalance.AutoSize = true;
            lblBalance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblBalance.Location = new Point(340, 35);

            dgvRacks.AllowUserToAddRows = false;
            dgvRacks.AllowUserToDeleteRows = false;
            dgvRacks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvRacks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRacks.Location = new Point(12, 62);
            dgvRacks.MultiSelect = false;
            dgvRacks.RowHeadersVisible = false;
            dgvRacks.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvRacks.Size = new Size(610, 260);
            dgvRacks.TabIndex = 0;
            dgvRacks.CellValidating += dgvRacks_CellValidating;
            dgvRacks.CellEndEdit += dgvRacks_CellEndEdit;
            dgvRacks.CellValueChanged += dgvRacks_CellValueChanged;
            dgvRacks.EditingControlShowing += dgvRacks_EditingControlShowing;

            DataGridViewTextBoxColumn locationColumn = new DataGridViewTextBoxColumn();
            locationColumn.HeaderText = "Location";
            locationColumn.Name = "LocationColumn";
            locationColumn.ReadOnly = true;
            locationColumn.Width = 150;
            dgvRacks.Columns.Add(locationColumn);

            DataGridViewTextBoxColumn rackColumn = new DataGridViewTextBoxColumn();
            rackColumn.HeaderText = "Rack";
            rackColumn.Name = "RackColumn";
            rackColumn.ReadOnly = true;
            rackColumn.Width = 150;
            dgvRacks.Columns.Add(rackColumn);

            DataGridViewTextBoxColumn availableColumn = new DataGridViewTextBoxColumn();
            availableColumn.HeaderText = "Available";
            availableColumn.Name = "AvailableColumn";
            availableColumn.ReadOnly = true;
            availableColumn.Width = 120;
            dgvRacks.Columns.Add(availableColumn);

            DataGridViewTextBoxColumn quantityColumn = new DataGridViewTextBoxColumn();
            quantityColumn.HeaderText = "Issue Qty";
            quantityColumn.Name = "QuantityColumn";
            quantityColumn.Width = 120;
            dgvRacks.Columns.Add(quantityColumn);

            DataGridViewTextBoxColumn rackIdColumn = new DataGridViewTextBoxColumn();
            rackIdColumn.HeaderText = "RackId";
            rackIdColumn.Name = "RackIdColumn";
            rackIdColumn.Visible = false;
            dgvRacks.Columns.Add(rackIdColumn);

            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Location = new Point(466, 335);
            btnOK.Size = new Size(75, 28);
            btnOK.Text = "OK";
            btnOK.Click += btnOK_Click;

            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Location = new Point(547, 335);
            btnCancel.Size = new Size(75, 28);
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;

            AutoScaleDimensions = new SizeF(7F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(634, 375);
            Controls.Add(lblProduct);
            Controls.Add(lblIssueQuantity);
            Controls.Add(lblAllocated);
            Controls.Add(lblBalance);
            Controls.Add(dgvRacks);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Font = new Font("Tahoma", 9F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Rack Allocation";
            Load += PendingIssuedRackAllocationDialog_Load;
            Shown += PendingIssuedRackAllocationDialog_Shown;

            ResumeLayout(false);
            PerformLayout();
        }

        private void PendingIssuedRackAllocationDialog_Load(object sender, EventArgs e)
        {
            Text = "Rack Allocation - " + productName;
            lblProduct.Text = "Product: " + productName;
            lblIssueQuantity.Text = "Remaining Pending: " + remainingPending.ToString("0.000");
            LoadRackGrid();
            RecalculateTotal();
        }

        private void PendingIssuedRackAllocationDialog_Shown(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(FocusFirstQuantityCell));
        }

        private void LoadRackGrid()
        {
            dgvRacks.Rows.Clear();
            foreach (DataRow rackRow in racks.Rows)
            {
                int rackId = SafeInt(rackRow["RackId"]);
                int rowIndex = dgvRacks.Rows.Add();
                DataGridViewRow row = dgvRacks.Rows[rowIndex];
                row.Cells["LocationColumn"].Value = Convert.ToString(rackRow["LocationName"]);
                row.Cells["RackColumn"].Value = Convert.ToString(rackRow["RackCaption"]);
                row.Cells["AvailableColumn"].Value = SafeDecimal(rackRow["AvailableQty"]).ToString("0.000");
                decimal selectedQuantity;
                row.Cells["QuantityColumn"].Value = initialAllocations.TryGetValue(rackId, out selectedQuantity) && selectedQuantity > 0 ? selectedQuantity.ToString("0.000") : "";
                row.Cells["RackIdColumn"].Value = rackId;
            }
        }

        private void FocusFirstQuantityCell()
        {
            if (dgvRacks.Rows.Count == 0)
            {
                return;
            }

            dgvRacks.Focus();
            dgvRacks.CurrentCell = dgvRacks.Rows[0].Cells["QuantityColumn"];
            dgvRacks.BeginEdit(true);
        }

        private void dgvRacks_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvRacks.Columns[e.ColumnIndex].Name != "QuantityColumn")
            {
                return;
            }

            string text = Convert.ToString(e.FormattedValue).Trim();
            if (text.Length == 0)
            {
                return;
            }

            decimal quantity;
            if (!decimal.TryParse(text, out quantity) || quantity < 0)
            {
                e.Cancel = true;
                MarkQuantityValidationFailed();
                MessageBox.Show("Enter a valid non-negative issue quantity.", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SelectCurrentQuantityText();
                return;
            }

            decimal available = SafeDecimal(dgvRacks.Rows[e.RowIndex].Cells["AvailableColumn"].Value);
            if (quantity > available)
            {
                e.Cancel = true;
                MarkQuantityValidationFailed();
                MessageBox.Show("Quantity cannot be greater than rack available quantity.", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SelectCurrentQuantityText();
                return;
            }

            if (GetTotalWithCurrentValue(e.RowIndex, quantity) > remainingPending)
            {
                e.Cancel = true;
                MarkQuantityValidationFailed();
                MessageBox.Show("Allocated quantity cannot be greater than remaining pending quantity of " + remainingPending.ToString("0.000") + ".", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SelectCurrentQuantityText();
            }
        }

        private void dgvRacks_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            RecalculateTotal();
        }

        private void dgvRacks_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            RecalculateTotal();
        }

        private void dgvRacks_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox textBox = e.Control as TextBox;
            if (textBox != null)
            {
                textBox.KeyPress -= QuantityTextBox_KeyPress;
                if (dgvRacks.CurrentCell != null && dgvRacks.Columns[dgvRacks.CurrentCell.ColumnIndex].Name == "QuantityColumn")
                {
                    textBox.KeyPress += QuantityTextBox_KeyPress;
                }
            }
        }

        private void dgvRacks_KeyDown(object sender, KeyEventArgs e)
        {
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == Keys.Enter || keyData == Keys.Tab) && dgvRacks.ContainsFocus && dgvRacks.CurrentCell != null)
            {
                if (CommitCurrentQuantityEdit())
                {
                    RecalculateTotal();
                    MoveToNextQuantityOrOk();
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void QuantityTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || e.KeyChar == '.')
            {
                if (e.KeyChar == '.')
                {
                    TextBox textBox = sender as TextBox;
                    if (textBox != null && textBox.Text.IndexOf('.') >= 0)
                    {
                        e.Handled = true;
                    }
                }
                return;
            }

            e.Handled = true;
        }

        private void MoveToNextQuantityOrOk()
        {
            if (dgvRacks.CurrentCell == null)
            {
                btnOK.Focus();
                return;
            }

            int nextRow = dgvRacks.CurrentCell.RowIndex + 1;
            if (quantityNavigationPending)
            {
                return;
            }

            quantityNavigationPending = true;
            int navigationVersion = quantityNavigationVersion;
            BeginInvoke(new MethodInvoker(delegate
            {
                quantityNavigationPending = false;

                if (IsDisposed || dgvRacks.IsDisposed)
                {
                    return;
                }

                if (navigationVersion != quantityNavigationVersion || quantityValidationFailed || dgvRacks.IsCurrentCellInEditMode)
                {
                    return;
                }

                if (nextRow < dgvRacks.Rows.Count)
                {
                    dgvRacks.CurrentCell = dgvRacks.Rows[nextRow].Cells["QuantityColumn"];
                    dgvRacks.Focus();
                }
                else
                {
                    btnOK.Focus();
                }
            }));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CommitCurrentQuantityEdit())
            {
                return;
            }

            if (!ValidateAndStoreAllocations())
            {
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void MarkQuantityValidationFailed()
        {
            quantityValidationFailed = true;
            quantityNavigationPending = false;
            quantityNavigationVersion++;
        }

        private bool CommitCurrentQuantityEdit()
        {
            quantityValidationFailed = false;
            bool committed = dgvRacks.EndEdit();
            return committed && !quantityValidationFailed;
        }

        private decimal GetTotalWithCurrentValue(int currentRowIndex, decimal currentQuantity)
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvRacks.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                total += row.Index == currentRowIndex ? currentQuantity : SafeDecimal(row.Cells["QuantityColumn"].Value);
            }

            return total;
        }

        private void SelectCurrentQuantityText()
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                dgvRacks.Focus();
                TextBox textBox = dgvRacks.EditingControl as TextBox;
                if (textBox != null)
                {
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }));
        }

        private bool ValidateAndStoreAllocations()
        {
            Dictionary<int, decimal> newAllocations = new Dictionary<int, decimal>();
            decimal total = 0;

            foreach (DataGridViewRow row in dgvRacks.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                decimal quantity = SafeDecimal(row.Cells["QuantityColumn"].Value);
                decimal available = SafeDecimal(row.Cells["AvailableColumn"].Value);
                if (quantity < 0)
                {
                    MessageBox.Show("Enter a valid non-negative issue quantity.", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (quantity > available)
                {
                    MessageBox.Show("Quantity cannot be greater than rack available quantity.", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                int rackId = SafeInt(row.Cells["RackIdColumn"].Value);
                if (quantity > 0)
                {
                    newAllocations[rackId] = quantity;
                    total += quantity;
                }
            }

            if (total <= 0)
            {
                MessageBox.Show("Allocated quantity must be greater than zero.", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FocusFirstQuantityCell();
                return false;
            }

            if (total > remainingPending)
            {
                MessageBox.Show("Allocated quantity cannot be greater than remaining pending quantity of " + remainingPending.ToString("0.000") + ".", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FocusFirstQuantityCell();
                return false;
            }

            Allocations = newAllocations;
            TotalAllocated = total;
            SetTotals(total);
            return true;
        }

        private void RecalculateTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvRacks.Rows)
            {
                if (!row.IsNewRow)
                {
                    total += SafeDecimal(row.Cells["QuantityColumn"].Value);
                }
            }

            TotalAllocated = total;
            SetTotals(total);
        }

        private void SetTotals(decimal total)
        {
            decimal balance = remainingPending - total;
            lblAllocated.Text = "Allocated: " + total.ToString("0.000");
            lblBalance.Text = "Balance Pending: " + balance.ToString("0.000");
            lblBalance.ForeColor = balance >= 0 ? Color.DarkGreen : Color.Firebrick;
            btnOK.Enabled = total > 0 && balance >= 0;
        }

        private int SafeInt(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }

            int result;
            return int.TryParse(Convert.ToString(value), out result) ? result : 0;
        }

        private decimal SafeDecimal(object value)
        {
            if (value == null || value == DBNull.Value || Convert.ToString(value).Trim().Length == 0)
            {
                return 0;
            }

            decimal result;
            return decimal.TryParse(Convert.ToString(value), out result) ? result : 0;
        }
    }
}
