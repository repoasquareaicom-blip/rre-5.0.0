using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Purchase
{
    public partial class PurchaseReceiptRackAllocation : Form
    {
        private readonly DataTable racks;
        private readonly decimal orderedQty;
        private readonly string productName;

        public Dictionary<int, decimal> Allocations;
        public decimal TotalReceived;

        public PurchaseReceiptRackAllocation(int productId, string productName, decimal orderedQty, DataTable racks, Dictionary<int, decimal> existingAllocations)
        {
            InitializeComponent();
            this.productName = productName;
            this.orderedQty = orderedQty;
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

        private void PurchaseReceiptRackAllocation_Load(object sender, EventArgs e)
        {
            lblProduct.Text = "Product: " + productName;
            lblOrderedQty.Text = "Ordered Qty: " + orderedQty.ToString("0.###");
            LoadRackGrid();
            RecalculateTotal();
        }

        private void PurchaseReceiptRackAllocation_Shown(object sender, EventArgs e)
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
                row.Cells["Location"].Value = Convert.ToString(rackRow["LocationName"]);
                row.Cells["Rack"].Value = Convert.ToString(rackRow["RackCaption"]);
                row.Cells["RackId"].Value = rackId;

                decimal quantity;
                if (Allocations.TryGetValue(rackId, out quantity) && quantity > 0)
                {
                    row.Cells["Quantity"].Value = quantity.ToString("0.###");
                }
                else
                {
                    row.Cells["Quantity"].Value = "";
                }
            }
        }

        private void FocusFirstQuantityCell()
        {
            if (dgvRacks.Rows.Count == 0)
            {
                return;
            }

            dgvRacks.Focus();
            dgvRacks.CurrentCell = dgvRacks.Rows[0].Cells["Quantity"];
            dgvRacks.BeginEdit(true);
        }

        private void dgvRacks_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvRacks.Columns[e.ColumnIndex].Name != "Quantity")
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
                MessageBox.Show("Enter a valid non-negative rack quantity.", "Rack-wise Received Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                if (dgvRacks.CurrentCell != null && dgvRacks.Columns[dgvRacks.CurrentCell.ColumnIndex].Name == "Quantity")
                {
                    textBox.KeyPress += QuantityTextBox_KeyPress;
                }
            }
        }

        private void dgvRacks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                dgvRacks.EndEdit();
                RecalculateTotal();
                MoveToNextQuantityOrOk();
            }
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
            if (nextRow < dgvRacks.Rows.Count)
            {
                dgvRacks.CurrentCell = dgvRacks.Rows[nextRow].Cells["Quantity"];
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

                decimal quantity = SafeDecimal(row.Cells["Quantity"].Value);
                if (quantity < 0)
                {
                    MessageBox.Show("Enter a valid non-negative rack quantity.", "Rack-wise Received Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                int rackId = SafeInt(row.Cells["RackId"].Value);
                newAllocations[rackId] = quantity;
                total += quantity;
            }

            if (total > orderedQty)
            {
                MessageBox.Show("Received quantity cannot exceed the remaining quantity of " + orderedQty.ToString("0.###") + " for " + productName + ".", "Rack-wise Received Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FocusFirstQuantityCell();
                return false;
            }

            Allocations = newAllocations;
            TotalReceived = total;
            lblTotal.Text = "Total Received: " + total.ToString("0.###");
            return true;
        }

        private void RecalculateTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvRacks.Rows)
            {
                if (!row.IsNewRow)
                {
                    total += SafeDecimal(row.Cells["Quantity"].Value);
                }
            }

            TotalReceived = total;
            lblTotal.Text = "Total Received: " + total.ToString("0.###");
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
