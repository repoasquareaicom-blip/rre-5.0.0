using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Sales
{
    public partial class PdiReverseRackAllocationDialog : Form
    {
        private readonly DataTable racks;
        private readonly decimal returnQty;
        private readonly string productName;

        public Dictionary<int, decimal> Allocations;
        public decimal TotalAllocated;

        public PdiReverseRackAllocationDialog(string productName, decimal returnQty, DataTable racks)
        {
            InitializeComponent();
            this.productName = productName;
            this.returnQty = returnQty;
            this.racks = racks;
            Allocations = new Dictionary<int, decimal>();
        }

        private void PdiReverseRackAllocationDialog_Load(object sender, EventArgs e)
        {
            Text = "Reverse PDI Rack Allocation - " + productName;
            lblProduct.Text = "Product: " + productName;
            lblReturnQty.Text = "Quantity to Return: " + returnQty.ToString("0.000");
            LoadRackGrid();
            RecalculateTotal();
        }

        private void PdiReverseRackAllocationDialog_Shown(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(FocusFirstQuantityCell));
        }

        private void LoadRackGrid()
        {
            dgvRacks.Rows.Clear();
            foreach (DataRow rackRow in racks.Rows)
            {
                int rowIndex = dgvRacks.Rows.Add();
                DataGridViewRow row = dgvRacks.Rows[rowIndex];
                row.Cells["LocationColumn"].Value = Convert.ToString(rackRow["LocationName"]);
                row.Cells["RackColumn"].Value = Convert.ToString(rackRow["RackCaption"]);
                row.Cells["RackIdColumn"].Value = SafeInt(rackRow["RackId"]);
                row.Cells["ReturnQtyColumn"].Value = "";
            }
        }

        private void FocusFirstQuantityCell()
        {
            if (dgvRacks.Rows.Count == 0)
            {
                return;
            }

            dgvRacks.Focus();
            dgvRacks.CurrentCell = dgvRacks.Rows[0].Cells["ReturnQtyColumn"];
            dgvRacks.BeginEdit(true);
        }

        private void dgvRacks_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvRacks.Columns[e.ColumnIndex].Name != "ReturnQtyColumn")
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
                MessageBox.Show("Enter a valid non-negative return quantity.", "Reverse PDI Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                if (dgvRacks.CurrentCell != null && dgvRacks.Columns[dgvRacks.CurrentCell.ColumnIndex].Name == "ReturnQtyColumn")
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
                MoveToNextQuantityOrOk();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == Keys.Enter || keyData == Keys.Tab) && dgvRacks.ContainsFocus && dgvRacks.CurrentCell != null)
            {
                dgvRacks.EndEdit();
                RecalculateTotal();
                MoveToNextQuantityOrOk();
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
            if (nextRow < dgvRacks.Rows.Count)
            {
                dgvRacks.CurrentCell = dgvRacks.Rows[nextRow].Cells["ReturnQtyColumn"];
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

                decimal quantity = SafeDecimal(row.Cells["ReturnQtyColumn"].Value);
                if (quantity < 0)
                {
                    MessageBox.Show("Enter a valid non-negative return quantity.", "Reverse PDI Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (quantity > 0)
                {
                    int rackId = SafeInt(row.Cells["RackIdColumn"].Value);
                    newAllocations[rackId] = quantity;
                    total += quantity;
                }
            }

            if (total != returnQty)
            {
                MessageBox.Show("Allocated return quantity must equal " + returnQty.ToString("0.000") + " for " + productName + ".", "Reverse PDI Rack Allocation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    total += SafeDecimal(row.Cells["ReturnQtyColumn"].Value);
                }
            }

            TotalAllocated = total;
            SetTotals(total);
        }

        private void SetTotals(decimal total)
        {
            decimal balance = returnQty - total;
            lblAllocated.Text = "Allocated: " + total.ToString("0.000");
            lblBalance.Text = "Balance: " + balance.ToString("0.000");
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
