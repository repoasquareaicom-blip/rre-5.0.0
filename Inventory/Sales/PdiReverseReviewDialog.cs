using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Inventory.Masters;

namespace Inventory.Sales
{
    public class PdiReverseReviewDialog : Form
    {
        private readonly string quotationId;
        private readonly DataTable lines;
        private readonly PdiReverseRepository repository;
        private readonly ProductRackMappingRepository rackRepository;
        private readonly DataGridView grid;
        private readonly List<ReverseLine> rowLines;

        public DataTable SaleRackDetails;
        public DataTable ReturnRackDetails;

        public PdiReverseReviewDialog(string quotationId, DataTable lines, PdiReverseRepository repository)
        {
            this.quotationId = quotationId;
            this.lines = lines;
            this.repository = repository;
            rackRepository = new ProductRackMappingRepository();
            rowLines = new List<ReverseLine>();

            Text = "Reverse PDI - " + quotationId;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(980, 460);
            Font = new Font("Tahoma", 9F);

            Label heading = new Label();
            heading.AutoSize = false;
            heading.Location = new Point(12, 10);
            heading.Size = new Size(956, 36);
            heading.Text = "Every product movement will be reversed in full. Status is Verified. Select a rack for each product.";

            grid = new DataGridView();
            grid.Location = new Point(12, 50);
            grid.Size = new Size(956, 340);
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.DataError += new DataGridViewDataErrorEventHandler(grid_DataError);

            DataGridViewTextBoxColumn productColumn = new DataGridViewTextBoxColumn();
            productColumn.Name = "Product";
            productColumn.HeaderText = "Product";
            productColumn.ReadOnly = true;
            productColumn.FillWeight = 180;

            DataGridViewTextBoxColumn qtyColumn = new DataGridViewTextBoxColumn();
            qtyColumn.Name = "Qty";
            qtyColumn.HeaderText = "Qty";
            qtyColumn.ReadOnly = true;
            qtyColumn.FillWeight = 50;
            qtyColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            DataGridViewTextBoxColumn movementColumn = new DataGridViewTextBoxColumn();
            movementColumn.Name = "Movement";
            movementColumn.HeaderText = "Movement";
            movementColumn.ReadOnly = true;
            movementColumn.FillWeight = 110;

            DataGridViewComboBoxColumn rackColumn = new DataGridViewComboBoxColumn();
            rackColumn.Name = "Rack";
            rackColumn.HeaderText = "Rack";
            rackColumn.FlatStyle = FlatStyle.Popup;
            rackColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            rackColumn.FillWeight = 90;

            DataGridViewTextBoxColumn statusColumn = new DataGridViewTextBoxColumn();
            statusColumn.Name = "Status";
            statusColumn.HeaderText = "Status";
            statusColumn.ReadOnly = true;
            statusColumn.FillWeight = 60;

            grid.Columns.Add(productColumn);
            grid.Columns.Add(qtyColumn);
            grid.Columns.Add(movementColumn);
            grid.Columns.Add(rackColumn);
            grid.Columns.Add(statusColumn);

            Button okButton = new Button();
            okButton.Text = "Reverse PDI";
            okButton.Location = new Point(760, 408);
            okButton.Size = new Size(110, 32);
            okButton.Click += new EventHandler(okButton_Click);

            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(878, 408);
            cancelButton.Size = new Size(90, 32);

            AcceptButton = okButton;
            CancelButton = cancelButton;
            Controls.Add(heading);
            Controls.Add(grid);
            Controls.Add(okButton);
            Controls.Add(cancelButton);

            LoadLines();
        }

        private void LoadLines()
        {
            foreach (DataRow row in lines.Rows)
            {
                ReverseLine line = new ReverseLine();
                line.ProductId = ToInt(row["ProductId"]);
                line.ProductName = Convert.ToString(row["ProductName"]);
                line.SignedQuantity = ToDecimal(row["SignedQuantity"]);
                line.ReverseQuantity = ToDecimal(row["ReverseQuantity"]);
                line.IsReturn = ToBool(row["IsReturn"]);
                line.RackWise = ToBool(row["RackWise"]);
                line.RackId = ToInt(row["RackId"]);
                line.RackCaption = Convert.ToString(row["RackCaption"]);
                line.RackChoices = new Dictionary<string, int>();

                int index = grid.Rows.Add();
                DataGridViewRow gridRow = grid.Rows[index];
                gridRow.Cells["Product"].Value = line.ProductName;
                gridRow.Cells["Qty"].Value = line.SignedQuantity.ToString("0.###");
                gridRow.Cells["Movement"].Value = line.IsReturn ? "Return Reverse" : "Sale Return to Stock";
                gridRow.Cells["Status"].Value = "Verified";
                gridRow.Tag = line;
                rowLines.Add(line);

                DataGridViewComboBoxCell rackCell = (DataGridViewComboBoxCell)gridRow.Cells["Rack"];
                DataTable racks = repository.GetReturnRacksForProduct(line.ProductId);
                foreach (DataRow rackRow in racks.Rows)
                {
                    string caption = Convert.ToString(rackRow["RackCaption"]);
                    if (line.RackChoices.ContainsKey(caption))
                    {
                        caption = Convert.ToString(rackRow["LocationName"]) + " / " + caption;
                    }

                    if (!line.RackChoices.ContainsKey(caption))
                    {
                        line.RackChoices.Add(caption, ToInt(rackRow["RackId"]));
                        rackCell.Items.Add(caption);
                    }
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            grid.EndEdit();
            StringBuilder errors = new StringBuilder();
            DataTable saleDetails = repository.CreateRackDetailsTable();
            DataTable returnDetails = repository.CreateRackDetailsTable();

            foreach (DataGridViewRow gridRow in grid.Rows)
            {
                ReverseLine line = gridRow.Tag as ReverseLine;
                if (line == null)
                {
                    continue;
                }

                string selectedCaption = Convert.ToString(gridRow.Cells["Rack"].Value);
                int rackId = 0;
                if (selectedCaption.Length > 0 && line.RackChoices.ContainsKey(selectedCaption))
                {
                    rackId = line.RackChoices[selectedCaption];
                }

                if (line.RackChoices.Count == 0)
                {
                    errors.AppendLine("No active rack is assigned to " + line.ProductName + ".");
                    continue;
                }

                if (rackId <= 0)
                {
                    errors.AppendLine(line.ProductName + ": select a rack.");
                    continue;
                }

                if (line.IsReturn)
                {
                    decimal available = rackRepository.GetRackBalance(line.ProductId, rackId);
                    if (available < line.ReverseQuantity)
                    {
                        errors.AppendLine(
                            "Product: " + line.ProductName
                            + ", Rack: " + selectedCaption
                            + ", Required Qty: " + line.ReverseQuantity.ToString("0.###")
                            + ", Available Qty: " + available.ToString("0.###"));
                        continue;
                    }
                }

                DataRow detail = (line.IsReturn ? returnDetails : saleDetails).NewRow();
                detail["ProductId"] = line.ProductId;
                detail["RackId"] = rackId;
                detail["Quantity"] = line.ReverseQuantity;
                if (line.IsReturn)
                {
                    returnDetails.Rows.Add(detail);
                }
                else
                {
                    saleDetails.Rows.Add(detail);
                }
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(
                    "This PDI was not reversed." + Environment.NewLine + Environment.NewLine + errors.ToString(),
                    "Reverse PDI",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            SaleRackDetails = saleDetails;
            ReturnRackDetails = returnDetails;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private int ToInt(object value)
        {
            if (value == null || value == DBNull.Value || Convert.ToString(value).Trim().Length == 0)
            {
                return 0;
            }

            int result;
            return int.TryParse(Convert.ToString(value), out result) ? result : 0;
        }

        private decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value || Convert.ToString(value).Trim().Length == 0)
            {
                return 0;
            }

            decimal result;
            return decimal.TryParse(Convert.ToString(value), out result) ? result : 0;
        }

        private bool ToBool(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            if (value is bool)
            {
                return (bool)value;
            }

            return Convert.ToString(value) == "1" || Convert.ToString(value).ToLower() == "true";
        }

        private class ReverseLine
        {
            public int ProductId;
            public string ProductName;
            public decimal SignedQuantity;
            public decimal ReverseQuantity;
            public bool IsReturn;
            public bool RackWise;
            public int RackId;
            public string RackCaption;
            public Dictionary<string, int> RackChoices;
        }
    }
}
