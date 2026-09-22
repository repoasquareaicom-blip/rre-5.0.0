using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Sales
{
    public class RackStockAvailabilityForm : Form
    {
        private DataGridView grid;

        public static void ShowForProduct(IWin32Window owner, int productId, string productName)
        {
            DataTable details;
            try
            {
                details = LoadStockRackWiseDetail(productId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Rack stock could not be loaded. " + ex.Message,
                    "Rack Stock Availability",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            DataTable positive = SelectPositiveStock(details);
            if (positive.Rows.Count == 0)
            {
                MessageBox.Show(
                    "No rack-wise stock available for this product.",
                    "Rack Stock Availability",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (RackStockAvailabilityForm form = new RackStockAvailabilityForm(productName, positive))
            {
                if (owner != null)
                {
                    form.ShowDialog(owner);
                }
                else
                {
                    form.ShowDialog();
                }
            }
        }

        private RackStockAvailabilityForm(string productName, DataTable stock)
        {
            Text = "Rack Stock Availability";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            KeyPreview = true;
            ClientSize = new Size(560, 390);
            Font = new Font("Microsoft Sans Serif", 9F);

            Label title = new Label();
            title.Text = "Rack Stock Availability";
            title.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            title.Location = new Point(16, 12);
            title.AutoSize = true;

            Label product = new Label();
            if (productName == null)
            {
                productName = "";
            }
            product.Text = "Product: " + productName;
            product.Location = new Point(16, 40);
            product.Size = new Size(528, 36);

            grid = new DataGridView();
            grid.Location = new Point(16, 82);
            grid.Size = new Size(528, 230);
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoGenerateColumns = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = SystemColors.Window;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.StandardTab = true;
            grid.KeyDown += Grid_KeyDown;

            DataGridViewTextBoxColumn locationColumn = new DataGridViewTextBoxColumn();
            locationColumn.Name = "Location";
            locationColumn.HeaderText = "Location";
            locationColumn.DataPropertyName = "Location";
            locationColumn.ReadOnly = true;
            locationColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            locationColumn.FillWeight = 45;

            DataGridViewTextBoxColumn rackColumn = new DataGridViewTextBoxColumn();
            rackColumn.Name = "Rack";
            rackColumn.HeaderText = "Rack";
            rackColumn.DataPropertyName = "Rack";
            rackColumn.ReadOnly = true;
            rackColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            rackColumn.FillWeight = 30;

            DataGridViewTextBoxColumn stockColumn = new DataGridViewTextBoxColumn();
            stockColumn.Name = "Stock";
            stockColumn.HeaderText = "Stock";
            stockColumn.DataPropertyName = "Stock";
            stockColumn.ReadOnly = true;
            stockColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            stockColumn.FillWeight = 25;
            stockColumn.DefaultCellStyle.Format = "0.000";
            stockColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            stockColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

            grid.Columns.Add(locationColumn);
            grid.Columns.Add(rackColumn);
            grid.Columns.Add(stockColumn);
            grid.DataSource = stock;

            decimal total = 0;
            foreach (DataRow row in stock.Rows)
            {
                total += Convert.ToDecimal(row["Stock"]);
            }

            Label totalCaption = new Label();
            totalCaption.Text = "TOTAL";
            totalCaption.Font = new Font(Font, FontStyle.Bold);
            totalCaption.Location = new Point(16, 320);
            totalCaption.AutoSize = true;

            Label totalValue = new Label();
            totalValue.Text = total.ToString("0.000");
            totalValue.Font = new Font(Font, FontStyle.Bold);
            totalValue.TextAlign = ContentAlignment.MiddleRight;
            totalValue.Location = new Point(360, 318);
            totalValue.Size = new Size(184, 20);

            Button close = new Button();
            close.Text = "Close";
            close.DialogResult = DialogResult.Cancel;
            close.Location = new Point(452, 348);
            close.Size = new Size(92, 28);

            CancelButton = close;
            Controls.Add(title);
            Controls.Add(product);
            Controls.Add(grid);
            Controls.Add(totalCaption);
            Controls.Add(totalValue);
            Controls.Add(close);
            KeyDown += RackStockAvailabilityForm_KeyDown;
        }

        private static DataTable LoadStockRackWiseDetail(int productId)
        {
            DataTable details = new DataTable();
            using (SqlConnection con = new SqlConnection(Program.connection))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.GetStockRackWiseDetail", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(details);
                    }
                }
            }
            return details;
        }

        private static DataTable SelectPositiveStock(DataTable details)
        {
            DataTable result = new DataTable();
            result.Columns.Add("Location", typeof(string));
            result.Columns.Add("Rack", typeof(string));
            result.Columns.Add("Stock", typeof(decimal));

            if (details == null)
            {
                return result;
            }

            foreach (DataRow row in details.Rows)
            {
                decimal quantity = 0;
                if (row["Quantity"] != DBNull.Value)
                {
                    quantity = Convert.ToDecimal(row["Quantity"]);
                }

                if (quantity > 0)
                {
                    DataRow newRow = result.NewRow();
                    newRow["Location"] = Convert.ToString(row["LocationName"]);
                    newRow["Rack"] = Convert.ToString(row["RackCaption"]);
                    newRow["Stock"] = quantity;
                    result.Rows.Add(newRow);
                }
            }

            return result;
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void RackStockAvailabilityForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
    }
}
