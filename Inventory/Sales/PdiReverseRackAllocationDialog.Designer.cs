namespace Inventory.Sales
{
    partial class PdiReverseRackAllocationDialog
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
            this.lblReturnQty = new System.Windows.Forms.Label();
            this.dgvRacks = new System.Windows.Forms.DataGridView();
            this.LocationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RackColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReturnQtyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RackIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblAllocated = new System.Windows.Forms.Label();
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
            this.lblProduct.Size = new System.Drawing.Size(520, 20);
            this.lblProduct.TabIndex = 0;
            this.lblProduct.Text = "Product:";
            // 
            // lblReturnQty
            // 
            this.lblReturnQty.AutoSize = true;
            this.lblReturnQty.Location = new System.Drawing.Point(12, 35);
            this.lblReturnQty.Name = "lblReturnQty";
            this.lblReturnQty.Size = new System.Drawing.Size(110, 14);
            this.lblReturnQty.TabIndex = 1;
            this.lblReturnQty.Text = "Quantity to Return:";
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
            this.ReturnQtyColumn,
            this.RackIdColumn});
            this.dgvRacks.Location = new System.Drawing.Point(12, 58);
            this.dgvRacks.MultiSelect = false;
            this.dgvRacks.Name = "dgvRacks";
            this.dgvRacks.RowHeadersVisible = false;
            this.dgvRacks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvRacks.Size = new System.Drawing.Size(520, 246);
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
            this.LocationColumn.Width = 170;
            // 
            // RackColumn
            // 
            this.RackColumn.HeaderText = "Rack";
            this.RackColumn.Name = "RackColumn";
            this.RackColumn.ReadOnly = true;
            this.RackColumn.Width = 190;
            // 
            // ReturnQtyColumn
            // 
            this.ReturnQtyColumn.HeaderText = "Return Qty";
            this.ReturnQtyColumn.Name = "ReturnQtyColumn";
            this.ReturnQtyColumn.Width = 130;
            // 
            // RackIdColumn
            // 
            this.RackIdColumn.HeaderText = "RackId";
            this.RackIdColumn.Name = "RackIdColumn";
            this.RackIdColumn.Visible = false;
            // 
            // lblAllocated
            // 
            this.lblAllocated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAllocated.AutoSize = true;
            this.lblAllocated.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblAllocated.Location = new System.Drawing.Point(12, 318);
            this.lblAllocated.Name = "lblAllocated";
            this.lblAllocated.Size = new System.Drawing.Size(79, 14);
            this.lblAllocated.TabIndex = 3;
            this.lblAllocated.Text = "Allocated: 0";
            // 
            // lblBalance
            // 
            this.lblBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBalance.AutoSize = true;
            this.lblBalance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblBalance.Location = new System.Drawing.Point(170, 318);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(69, 14);
            this.lblBalance.TabIndex = 4;
            this.lblBalance.Text = "Balance: 0";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(376, 311);
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
            this.btnCancel.Location = new System.Drawing.Point(457, 311);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PdiReverseRackAllocationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(544, 351);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblBalance);
            this.Controls.Add(this.lblAllocated);
            this.Controls.Add(this.dgvRacks);
            this.Controls.Add(this.lblReturnQty);
            this.Controls.Add(this.lblProduct);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PdiReverseRackAllocationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Reverse PDI Rack Allocation";
            this.Load += new System.EventHandler(this.PdiReverseRackAllocationDialog_Load);
            this.Shown += new System.EventHandler(this.PdiReverseRackAllocationDialog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRacks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblReturnQty;
        private System.Windows.Forms.DataGridView dgvRacks;
        private System.Windows.Forms.DataGridViewTextBoxColumn LocationColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn RackColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReturnQtyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn RackIdColumn;
        private System.Windows.Forms.Label lblAllocated;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
