namespace Inventory.Sales
{
    partial class PdiRackAllocationDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblProduct = new System.Windows.Forms.Label();
            this.lblRequested = new System.Windows.Forms.Label();
            this.dgvRacks = new System.Windows.Forms.DataGridView();
            this.LocationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RackColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvailableColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PickQtyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RackIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSelected = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRacks)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProduct
            // 
            this.lblProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProduct.AutoEllipsis = true;
            this.lblProduct.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblProduct.Location = new System.Drawing.Point(12, 9);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(560, 20);
            this.lblProduct.TabIndex = 0;
            this.lblProduct.Text = "Product";
            // 
            // lblRequested
            // 
            this.lblRequested.AutoSize = true;
            this.lblRequested.Location = new System.Drawing.Point(12, 35);
            this.lblRequested.Name = "lblRequested";
            this.lblRequested.Size = new System.Drawing.Size(92, 14);
            this.lblRequested.TabIndex = 1;
            this.lblRequested.Text = "Requested Qty:";
            // 
            // dgvRacks
            // 
            this.dgvRacks.AllowUserToAddRows = false;
            this.dgvRacks.AllowUserToDeleteRows = false;
            this.dgvRacks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRacks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRacks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LocationColumn,
            this.RackColumn,
            this.AvailableColumn,
            this.PickQtyColumn,
            this.RackIdColumn});
            this.dgvRacks.Location = new System.Drawing.Point(12, 58);
            this.dgvRacks.MultiSelect = false;
            this.dgvRacks.Name = "dgvRacks";
            this.dgvRacks.RowHeadersVisible = false;
            this.dgvRacks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvRacks.Size = new System.Drawing.Size(560, 250);
            this.dgvRacks.TabIndex = 2;
            this.dgvRacks.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRacks_CellEndEdit);
            this.dgvRacks.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvRacks_CellValidating);
            this.dgvRacks.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRacks_CellValueChanged);
            this.dgvRacks.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvRacks_EditingControlShowing);
            this.dgvRacks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvRacks_KeyDown);
            // 
            // LocationColumn
            // 
            this.LocationColumn.HeaderText = "Location";
            this.LocationColumn.Name = "LocationColumn";
            this.LocationColumn.ReadOnly = true;
            this.LocationColumn.Width = 150;
            // 
            // RackColumn
            // 
            this.RackColumn.HeaderText = "Rack";
            this.RackColumn.Name = "RackColumn";
            this.RackColumn.ReadOnly = true;
            this.RackColumn.Width = 150;
            // 
            // AvailableColumn
            // 
            this.AvailableColumn.HeaderText = "Available";
            this.AvailableColumn.Name = "AvailableColumn";
            this.AvailableColumn.ReadOnly = true;
            this.AvailableColumn.Width = 110;
            // 
            // PickQtyColumn
            // 
            this.PickQtyColumn.HeaderText = "Pick Qty";
            this.PickQtyColumn.Name = "PickQtyColumn";
            this.PickQtyColumn.Width = 120;
            // 
            // RackIdColumn
            // 
            this.RackIdColumn.HeaderText = "RackId";
            this.RackIdColumn.Name = "RackIdColumn";
            this.RackIdColumn.Visible = false;
            // 
            // lblSelected
            // 
            this.lblSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelected.AutoSize = true;
            this.lblSelected.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblSelected.Location = new System.Drawing.Point(12, 322);
            this.lblSelected.Name = "lblSelected";
            this.lblSelected.Size = new System.Drawing.Size(74, 14);
            this.lblSelected.TabIndex = 3;
            this.lblSelected.Text = "Selected: 0";
            // 
            // lblBalance
            // 
            this.lblBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBalance.AutoSize = true;
            this.lblBalance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblBalance.Location = new System.Drawing.Point(150, 322);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(69, 14);
            this.lblBalance.TabIndex = 4;
            this.lblBalance.Text = "Balance: 0";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(416, 315);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 28);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(497, 315);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PdiRackAllocationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(584, 355);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblBalance);
            this.Controls.Add(this.lblSelected);
            this.Controls.Add(this.dgvRacks);
            this.Controls.Add(this.lblRequested);
            this.Controls.Add(this.lblProduct);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PdiRackAllocationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rack Allocation";
            this.Load += new System.EventHandler(this.PdiRackAllocationDialog_Load);
            this.Shown += new System.EventHandler(this.PdiRackAllocationDialog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRacks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblRequested;
        private System.Windows.Forms.DataGridView dgvRacks;
        private System.Windows.Forms.DataGridViewTextBoxColumn LocationColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn RackColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvailableColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PickQtyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn RackIdColumn;
        private System.Windows.Forms.Label lblSelected;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
