using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory
{
    public class MaterialMovementRackAllocationDialog : Form
    {
        private readonly bool manualSource;
        private readonly DataTable rackTable;
        private readonly DataGridView dgvFrom = new DataGridView();
        private readonly DataGridView dgvTo = new DataGridView();
        private readonly ComboBox ddlLocation = new ComboBox();
        private readonly ComboBox ddlRack = new ComboBox();
        private readonly TextBox txtQuantity = new TextBox();
        private readonly Label lblFromTotal = new Label();
        private readonly Label lblToTotal = new Label();
        private readonly Button btnOk = new Button();
        private readonly Button btnCancel = new Button();
        private bool loading;

        public MaterialMovementAllocation Allocation { get; private set; }

        public MaterialMovementRackAllocationDialog(ProductMovementInfo product, DataTable racks, MaterialMovementAllocation existing, bool manualSource)
        {
            this.manualSource = manualSource;
            rackTable = racks;
            Allocation = CloneAllocation(existing);
            if (Allocation == null)
            {
                Allocation = new MaterialMovementAllocation();
                Allocation.ProductId = product.ProductId;
                Allocation.ProductName = product.ProductName;
                Allocation.ManualSource = manualSource;
            }

            Initialize(product);
            LoadData(product);
        }

        private void Initialize(ProductMovementInfo product)
        {
            Text = manualSource ? "Rack Allocation" : "Move Stock";
            StartPosition = FormStartPosition.CenterParent;
            Size = manualSource ? new Size(900, 620) : new Size(520, 300);
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            Label header = new Label();
            header.Text = product.ProductName;
            header.Font = new Font("Calibri", 12F, FontStyle.Bold);
            header.Location = new Point(12, 12);
            header.Size = new Size(ClientSize.Width - 24, 24);
            Controls.Add(header);

            Label stock = new Label();
            stock.Text = "Available Stock: " + FormatQty(product.TotalStock);
            stock.Location = new Point(12, 40);
            stock.Size = new Size(300, 22);
            Controls.Add(stock);

            if (manualSource)
            {
                BuildManualUi();
            }
            else
            {
                BuildAutomaticUi();
            }
        }

        private void BuildManualUi()
        {
            Label fromLabel = new Label();
            fromLabel.Text = "FROM RACKS";
            fromLabel.Font = new Font("Calibri", 10F, FontStyle.Bold);
            fromLabel.Location = new Point(12, 72);
            fromLabel.Size = new Size(200, 20);
            Controls.Add(fromLabel);

            dgvFrom.Location = new Point(12, 95);
            dgvFrom.Size = new Size(860, 170);
            ConfigureGrid(dgvFrom);
            dgvFrom.Columns.Add(HiddenColumn("LocationId"));
            dgvFrom.Columns.Add(HiddenColumn("RackId"));
            dgvFrom.Columns.Add(TextColumn("LocationName", "Location", true));
            dgvFrom.Columns.Add(TextColumn("RackCaption", "Rack", true));
            dgvFrom.Columns.Add(TextColumn("AvailableQuantity", "Available", true));
            dgvFrom.Columns.Add(TextColumn("MoveQty", "Move Qty", false));
            dgvFrom.CellEndEdit += totals_Changed;
            Controls.Add(dgvFrom);

            Label toLabel = new Label();
            toLabel.Text = "TO RACKS";
            toLabel.Font = new Font("Calibri", 10F, FontStyle.Bold);
            toLabel.Location = new Point(12, 275);
            toLabel.Size = new Size(200, 20);
            Controls.Add(toLabel);

            dgvTo.Location = new Point(12, 298);
            dgvTo.Size = new Size(860, 170);
            ConfigureGrid(dgvTo);
            dgvTo.Columns.Add(HiddenColumn("LocationId"));
            dgvTo.Columns.Add(HiddenColumn("RackId"));
            dgvTo.Columns.Add(TextColumn("LocationName", "Location", true));
            dgvTo.Columns.Add(TextColumn("RackCaption", "Rack", true));
            dgvTo.Columns.Add(TextColumn("ReceiveQty", "Receive Qty", false));
            dgvTo.CellEndEdit += totals_Changed;
            Controls.Add(dgvTo);

            lblFromTotal.Location = new Point(12, 478);
            lblFromTotal.Size = new Size(250, 22);
            Controls.Add(lblFromTotal);

            lblToTotal.Location = new Point(280, 478);
            lblToTotal.Size = new Size(250, 22);
            Controls.Add(lblToTotal);

            AddButtons(697, 528);
        }

        private void BuildAutomaticUi()
        {
            Label qtyLabel = new Label();
            qtyLabel.Text = "Quantity to Move";
            qtyLabel.Location = new Point(12, 82);
            qtyLabel.Size = new Size(120, 22);
            Controls.Add(qtyLabel);

            txtQuantity.Location = new Point(145, 80);
            txtQuantity.Size = new Size(150, 22);
            Controls.Add(txtQuantity);

            Label locationLabel = new Label();
            locationLabel.Text = "To Location";
            locationLabel.Location = new Point(12, 120);
            locationLabel.Size = new Size(120, 22);
            Controls.Add(locationLabel);

            ddlLocation.DropDownStyle = ComboBoxStyle.DropDownList;
            ddlLocation.Location = new Point(145, 118);
            ddlLocation.Size = new Size(250, 22);
            ddlLocation.SelectedIndexChanged += ddlLocation_SelectedIndexChanged;
            Controls.Add(ddlLocation);

            Label rackLabel = new Label();
            rackLabel.Text = "To Rack";
            rackLabel.Location = new Point(12, 158);
            rackLabel.Size = new Size(120, 22);
            Controls.Add(rackLabel);

            ddlRack.DropDownStyle = ComboBoxStyle.DropDownList;
            ddlRack.Location = new Point(145, 156);
            ddlRack.Size = new Size(250, 22);
            Controls.Add(ddlRack);

            AddButtons(320, 220);
        }

        private void AddButtons(int x, int y)
        {
            btnOk.Text = "OK";
            btnOk.Location = new Point(x, y);
            btnOk.Size = new Size(80, 28);
            btnOk.Click += btnOk_Click;
            Controls.Add(btnOk);

            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(x + 90, y);
            btnCancel.Size = new Size(80, 28);
            btnCancel.DialogResult = DialogResult.Cancel;
            Controls.Add(btnCancel);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void ConfigureGrid(DataGridView grid)
        {
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.RowHeadersVisible = false;
            grid.AutoGenerateColumns = false;
            grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        private DataGridViewTextBoxColumn TextColumn(string name, string header, bool readOnly)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.DataPropertyName = name;
            column.HeaderText = header;
            column.ReadOnly = readOnly;
            column.Width = header == "Location" ? 220 : 140;
            return column;
        }

        private DataGridViewTextBoxColumn HiddenColumn(string name)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.DataPropertyName = name;
            column.Visible = false;
            return column;
        }

        private void LoadData(ProductMovementInfo product)
        {
            loading = true;
            if (manualSource)
            {
                DataTable from = BuildManualTable(true);
                DataTable to = BuildManualTable(false);
                dgvFrom.DataSource = from;
                dgvTo.DataSource = to;
                ApplyManualExisting(from, to);
                UpdateTotals();
            }
            else
            {
                txtQuantity.Text = Allocation.TotalQuantity > 0 ? FormatQty(Allocation.TotalQuantity) : string.Empty;
                BindLocations();
                ApplyAutomaticExisting();
            }
            loading = false;
        }

        private DataTable BuildManualTable(bool source)
        {
            DataTable table = new DataTable();
            table.Columns.Add("LocationId", typeof(int));
            table.Columns.Add("LocationName", typeof(string));
            table.Columns.Add("RackId", typeof(int));
            table.Columns.Add("RackCaption", typeof(string));
            table.Columns.Add("AvailableQuantity", typeof(decimal));
            table.Columns.Add(source ? "MoveQty" : "ReceiveQty", typeof(decimal));

            foreach (DataRow rack in rackTable.Rows)
            {
                decimal available = ToDecimal(rack["AvailableQuantity"]);
                if (source && available <= 0)
                {
                    continue;
                }

                DataRow row = table.NewRow();
                row["LocationId"] = ToInt(rack["LocationId"]);
                row["LocationName"] = Convert.ToString(rack["LocationName"]);
                row["RackId"] = ToInt(rack["RackId"]);
                row["RackCaption"] = Convert.ToString(rack["RackCaption"]);
                row["AvailableQuantity"] = available;
                row[source ? "MoveQty" : "ReceiveQty"] = 0m;
                table.Rows.Add(row);
            }

            return table;
        }

        private void ApplyManualExisting(DataTable from, DataTable to)
        {
            foreach (MaterialMovementRackLine line in Allocation.SourceLines)
            {
                ApplyQuantity(from, "MoveQty", line.RackId, line.Quantity);
            }

            foreach (MaterialMovementRackLine line in Allocation.DestinationLines)
            {
                ApplyQuantity(to, "ReceiveQty", line.RackId, line.Quantity);
            }
        }

        private void ApplyQuantity(DataTable table, string columnName, int rackId, decimal quantity)
        {
            foreach (DataRow row in table.Rows)
            {
                if (ToInt(row["RackId"]) == rackId)
                {
                    row[columnName] = quantity;
                    return;
                }
            }
        }

        private void BindLocations()
        {
            DataTable locations = rackTable.DefaultView.ToTable(true, new string[] { "LocationId", "LocationName" });
            ddlLocation.DisplayMember = "LocationName";
            ddlLocation.ValueMember = "LocationId";
            ddlLocation.DataSource = locations;
        }

        private void ApplyAutomaticExisting()
        {
            if (Allocation.DestinationLines.Count == 0)
            {
                if (ddlLocation.Items.Count > 0)
                {
                    ddlLocation.SelectedIndex = 0;
                }
                return;
            }

            int rackId = Allocation.DestinationLines[0].RackId;
            DataRow rack = FindRack(rackId);
            if (rack == null)
            {
                return;
            }

            ddlLocation.SelectedValue = ToInt(rack["LocationId"]);
            BindRacksForSelectedLocation();
            ddlRack.SelectedValue = rackId;
        }

        private void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                BindRacksForSelectedLocation();
            }
        }

        private void BindRacksForSelectedLocation()
        {
            if (ddlLocation.SelectedValue == null)
            {
                return;
            }

            int locationId = ToInt(ddlLocation.SelectedValue);
            DataView view = new DataView(rackTable);
            view.RowFilter = "LocationId = " + locationId.ToString();
            DataTable racks = view.ToTable();
            ddlRack.DisplayMember = "RackCaption";
            ddlRack.ValueMember = "RackId";
            ddlRack.DataSource = racks;
        }

        private void totals_Changed(object sender, EventArgs e)
        {
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            decimal fromTotal = GridTotal(dgvFrom, "MoveQty");
            decimal toTotal = GridTotal(dgvTo, "ReceiveQty");
            lblFromTotal.Text = "From Total: " + FormatQty(fromTotal);
            lblToTotal.Text = "To Total: " + FormatQty(toTotal);
        }

        private decimal GridTotal(DataGridView grid, string columnName)
        {
            decimal total = 0;
            foreach (DataGridViewRow row in grid.Rows)
            {
                total += ToDecimal(row.Cells[columnName].Value);
            }
            return total;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string message = manualSource ? AcceptManual() : AcceptAutomatic();
            if (message.Length > 0)
            {
                MessageBox.Show(message);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private string AcceptManual()
        {
            MaterialMovementAllocation next = new MaterialMovementAllocation();
            next.ProductId = Allocation.ProductId;
            next.ProductName = Allocation.ProductName;
            next.ManualSource = true;

            foreach (DataGridViewRow row in dgvFrom.Rows)
            {
                decimal qty = ToDecimal(row.Cells["MoveQty"].Value);
                if (qty <= 0)
                {
                    continue;
                }

                decimal available = ToDecimal(row.Cells["AvailableQuantity"].Value);
                if (qty > available)
                {
                    return "Source quantity cannot exceed rack stock.";
                }

                next.SourceLines.Add(BuildLine(row, qty));
            }

            foreach (DataGridViewRow row in dgvTo.Rows)
            {
                decimal qty = ToDecimal(row.Cells["ReceiveQty"].Value);
                if (qty <= 0)
                {
                    continue;
                }

                next.DestinationLines.Add(BuildLine(row, qty));
            }

            return ValidateAndSet(next);
        }

        private string AcceptAutomatic()
        {
            decimal qty = ToDecimal(txtQuantity.Text);
            if (qty <= 0)
            {
                return "Quantity must be greater than zero.";
            }

            if (ddlRack.SelectedValue == null)
            {
                return "Please select destination rack.";
            }

            int destinationRackId = ToInt(ddlRack.SelectedValue);
            MaterialMovementAllocation next = new MaterialMovementAllocation();
            next.ProductId = Allocation.ProductId;
            next.ProductName = Allocation.ProductName;
            next.ManualSource = false;

            decimal remaining = qty;
            foreach (DataRow rack in rackTable.Rows)
            {
                int rackId = ToInt(rack["RackId"]);
                if (rackId == destinationRackId)
                {
                    continue;
                }

                decimal available = ToDecimal(rack["AvailableQuantity"]);
                if (available <= 0)
                {
                    continue;
                }

                decimal take = available < remaining ? available : remaining;
                if (take > 0)
                {
                    next.SourceLines.Add(BuildLine(rack, take));
                    remaining -= take;
                }

                if (remaining <= 0)
                {
                    break;
                }
            }

            if (remaining > 0)
            {
                return "Insufficient source stock after excluding destination rack.";
            }

            DataRow destination = FindRack(destinationRackId);
            if (destination == null)
            {
                return "Please select destination rack.";
            }

            next.DestinationLines.Add(BuildLine(destination, qty));
            return ValidateAndSet(next);
        }

        private string ValidateAndSet(MaterialMovementAllocation next)
        {
            decimal sourceTotal = next.SourceTotal;
            decimal destinationTotal = next.DestinationTotal;
            if (sourceTotal <= 0 || destinationTotal <= 0)
            {
                return "Movement quantity must be greater than zero.";
            }

            if (sourceTotal != destinationTotal)
            {
                return "Source and destination totals must be equal.";
            }

            Dictionary<int, bool> sourceRacks = new Dictionary<int, bool>();
            foreach (MaterialMovementRackLine line in next.SourceLines)
            {
                sourceRacks[line.RackId] = true;
            }

            foreach (MaterialMovementRackLine line in next.DestinationLines)
            {
                if (sourceRacks.ContainsKey(line.RackId))
                {
                    return "Source and destination rack cannot be the same.";
                }
            }

            Allocation = next;
            return string.Empty;
        }

        private MaterialMovementRackLine BuildLine(DataGridViewRow row, decimal qty)
        {
            MaterialMovementRackLine line = new MaterialMovementRackLine();
            line.LocationId = ToInt(row.Cells["LocationId"].Value);
            line.LocationName = Convert.ToString(row.Cells["LocationName"].Value);
            line.RackId = ToInt(row.Cells["RackId"].Value);
            line.RackCaption = Convert.ToString(row.Cells["RackCaption"].Value);
            line.Quantity = qty;
            return line;
        }

        private MaterialMovementRackLine BuildLine(DataRow row, decimal qty)
        {
            MaterialMovementRackLine line = new MaterialMovementRackLine();
            line.LocationId = ToInt(row["LocationId"]);
            line.LocationName = Convert.ToString(row["LocationName"]);
            line.RackId = ToInt(row["RackId"]);
            line.RackCaption = Convert.ToString(row["RackCaption"]);
            line.Quantity = qty;
            return line;
        }

        private DataRow FindRack(int rackId)
        {
            foreach (DataRow row in rackTable.Rows)
            {
                if (ToInt(row["RackId"]) == rackId)
                {
                    return row;
                }
            }

            return null;
        }

        private MaterialMovementAllocation CloneAllocation(MaterialMovementAllocation source)
        {
            if (source == null)
            {
                return null;
            }

            MaterialMovementAllocation copy = new MaterialMovementAllocation();
            copy.ProductId = source.ProductId;
            copy.ProductName = source.ProductName;
            copy.ManualSource = source.ManualSource;
            foreach (MaterialMovementRackLine line in source.SourceLines)
            {
                copy.SourceLines.Add(line.Clone());
            }
            foreach (MaterialMovementRackLine line in source.DestinationLines)
            {
                copy.DestinationLines.Add(line.Clone());
            }
            return copy;
        }

        private int ToInt(object value)
        {
            if (value == null || value == DBNull.Value || Convert.ToString(value).Trim().Length == 0)
            {
                return 0;
            }

            return Convert.ToInt32(value);
        }

        private decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value || Convert.ToString(value).Trim().Length == 0)
            {
                return 0;
            }

            return Convert.ToDecimal(value);
        }

        private string FormatQty(decimal quantity)
        {
            return quantity.ToString("0.000");
        }
    }

    public class MaterialMovementAllocation
    {
        public int ProductId;
        public string ProductName;
        public bool ManualSource;
        public readonly List<MaterialMovementRackLine> SourceLines = new List<MaterialMovementRackLine>();
        public readonly List<MaterialMovementRackLine> DestinationLines = new List<MaterialMovementRackLine>();

        public decimal SourceTotal
        {
            get { return Total(SourceLines); }
        }

        public decimal DestinationTotal
        {
            get { return Total(DestinationLines); }
        }

        public decimal TotalQuantity
        {
            get { return SourceTotal; }
        }

        private decimal Total(List<MaterialMovementRackLine> lines)
        {
            decimal total = 0;
            foreach (MaterialMovementRackLine line in lines)
            {
                total += line.Quantity;
            }
            return total;
        }
    }

    public class MaterialMovementRackLine
    {
        public int LocationId;
        public string LocationName;
        public int RackId;
        public string RackCaption;
        public decimal Quantity;

        public MaterialMovementRackLine Clone()
        {
            MaterialMovementRackLine copy = new MaterialMovementRackLine();
            copy.LocationId = LocationId;
            copy.LocationName = LocationName;
            copy.RackId = RackId;
            copy.RackCaption = RackCaption;
            copy.Quantity = Quantity;
            return copy;
        }
    }
}
