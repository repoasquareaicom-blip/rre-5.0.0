using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Sales
{
    public partial class PdiRackAllocationDialog : Form
    {
        private readonly DataTable racks;
        private readonly decimal requestedQty;
        private readonly string productName;

        public Dictionary<int, decimal> Allocations;
        public decimal TotalSelected;

        public PdiRackAllocationDialog(string productName, decimal requestedQty, DataTable racks, Dictionary<int, decimal> existingAllocations)
        {
            InitializeComponent();
            this.productName = productName;
            this.requestedQty = requestedQty;
            this.racks = racks;
            Allocations = new Dictionary<int, decimal>();
            if (existingAllocations != null)
            {
                foreach (KeyValuePair<int, decimal> item in existingAllocations)
                {
                    Allocations[item.Key] = item.Value;
                }
            }
        }

        private void PdiRackAllocationDialog_Load(object sender, EventArgs e)
        {
            Text = "Rack Allocation - " + productName;
            lblProduct.Text = productName;
            lblRequested.Text = "Requested Qty: " + requestedQty.ToString("0.###");
            LoadRackGrid();
            RecalculateTotal();
        }

        private void PdiRackAllocationDialog_Shown(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(FocusFirstPickCell));
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
                row.Cells["AvailableColumn"].Value = SafeDecimal(rackRow["AvailableQty"]).ToString("0.###");
                row.Cells["RackIdColumn"].Value = rackId;

                decimal quantity;
                if (Allocations.TryGetValue(rackId, out quantity) && quantity > 0)
                {
                    row.Cells["PickQtyColumn"].Value = quantity.ToString("0.###");
                }
                else
                {
                    row.Cells["PickQtyColumn"].Value = "";
                }
            }
        }

        private void FocusFirstPickCell()
        {
            if (dgvRacks.Rows.Count == 0)
            {
                return;
            }

            dgvRacks.Focus();
            dgvRacks.CurrentCell = dgvRacks.Rows[0].Cells["PickQtyColumn"];
            dgvRacks.BeginEdit(true);
        }

        private void dgvRacks_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvRacks.Columns[e.ColumnIndex].Name != "PickQtyColumn")
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
                MessageBox.Show("Enter a valid non-negative pick quantity.", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal available = SafeDecimal(dgvRacks.Rows[e.RowIndex].Cells["AvailableColumn"].Value);
            if (quantity > available)
            {
                e.Cancel = true;
                MessageBox.Show("Pick Qty cannot be greater than rack available quantity.", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                if (dgvRacks.CurrentCell != null && dgvRacks.Columns[dgvRacks.CurrentCell.ColumnIndex].Name == "PickQtyColumn")
                {
                    textBox.KeyPress += QuantityTextBox_KeyPress;
                }
            }
        }

        private void dgvRacks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                dgvRacks.EndEdit();
                RecalculateTotal();
                MoveToNextPickOrOk();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == Keys.Enter || keyData == Keys.Tab) && dgvRacks.ContainsFocus && dgvRacks.CurrentCell != null)
            {
                dgvRacks.EndEdit();
                RecalculateTotal();
                MoveToNextPickOrOk();
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

        private void MoveToNextPickOrOk()
        {
            if (dgvRacks.CurrentCell == null)
            {
                btnOK.Focus();
                return;
            }

            int nextRow = dgvRacks.CurrentCell.RowIndex + 1;
            if (nextRow < dgvRacks.Rows.Count)
            {
                dgvRacks.CurrentCell = dgvRacks.Rows[nextRow].Cells["PickQtyColumn"];
                dgvRacks.BeginEdit(true);
            }
            else
            {
                btnOK.Focus();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            dgvRacks.EndEdit();
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

                decimal quantity = SafeDecimal(row.Cells["PickQtyColumn"].Value);
                decimal available = SafeDecimal(row.Cells["AvailableColumn"].Value);
                if (quantity < 0)
                {
                    MessageBox.Show("Enter a valid non-negative pick quantity.", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (quantity > available)
                {
                    MessageBox.Show("Pick Qty cannot be greater than rack available quantity.", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                int rackId = SafeInt(row.Cells["RackIdColumn"].Value);
                newAllocations[rackId] = quantity;
                total += quantity;
            }

            if (total > requestedQty)
            {
                MessageBox.Show("Selected quantity cannot exceed requested quantity of " + requestedQty.ToString("0.###") + ".", "Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FocusFirstPickCell();
                return false;
            }

            Allocations = newAllocations;
            TotalSelected = total;
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
                    total += SafeDecimal(row.Cells["PickQtyColumn"].Value);
                }
            }

            TotalSelected = total;
            SetTotals(total);
        }

        private void SetTotals(decimal total)
        {
            decimal balance = requestedQty - total;
            lblSelected.Text = "Selected: " + total.ToString("0.###");
            lblBalance.Text = "Balance: " + balance.ToString("0.###");
            lblBalance.ForeColor = balance == 0 ? Color.DarkGreen : Color.Firebrick;
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
