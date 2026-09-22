namespace Inventory.Purchase
{
    partial class PurchaseReceiptRackAllocation
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
            this.lblOrderedQty = new System.Windows.Forms.Label();
            this.dgvRacks = new System.Windows.Forms.DataGridView();
            this.Location = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rack = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RackId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTotal = new System.Windows.Forms.Label();
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
            this.lblProduct.Size = new System.Drawing.Size(460, 20);
            this.lblProduct.TabIndex = 0;
            this.lblProduct.Text = "Product:";
            // 
            // lblOrderedQty
            // 
            this.lblOrderedQty.AutoSize = true;
            this.lblOrderedQty.Location = new System.Drawing.Point(12, 33);
            this.lblOrderedQty.Name = "lblOrderedQty";
            this.lblOrderedQty.Size = new System.Drawing.Size(76, 14);
            this.lblOrderedQty.TabIndex = 1;
            this.lblOrderedQty.Text = "Ordered Qty:";
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
            this.Location,
            this.Rack,
            this.Quantity,
            this.RackId});
            this.dgvRacks.Location = new System.Drawing.Point(12, 55);
            this.dgvRacks.MultiSelect = false;
            this.dgvRacks.Name = "dgvRacks";
            this.dgvRacks.RowHeadersVisible = false;
            this.dgvRacks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvRacks.Size = new System.Drawing.Size(460, 224);
            this.dgvRacks.TabIndex = 2;
            this.dgvRacks.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRacks_CellEndEdit);
            this.dgvRacks.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvRacks_CellValidating);
            this.dgvRacks.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRacks_CellValueChanged);
            this.dgvRacks.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvRacks_EditingControlShowing);
            this.dgvRacks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvRacks_KeyDown);
            // 
            // Location
            // 
            this.Location.HeaderText = "Location";
            this.Location.Name = "Location";
            this.Location.ReadOnly = true;
            this.Location.Width = 150;
            // 
            // Rack
            // 
            this.Rack.HeaderText = "Rack";
            this.Rack.Name = "Rack";
            this.Rack.ReadOnly = true;
            this.Rack.Width = 170;
            // 
            // Quantity
            // 
            this.Quantity.HeaderText = "Quantity";
            this.Quantity.Name = "Quantity";
            this.Quantity.Width = 110;
            // 
            // RackId
            // 
            this.RackId.HeaderText = "RackId";
            this.RackId.Name = "RackId";
            this.RackId.Visible = false;
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(12, 293);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(112, 14);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.Text = "Total Received: 0";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(316, 286);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 28);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(397, 286);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PurchaseReceiptRackAllocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(484, 326);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.dgvRacks);
            this.Controls.Add(this.lblOrderedQty);
            this.Controls.Add(this.lblProduct);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PurchaseReceiptRackAllocation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rack-wise Received Quantity";
            this.Load += new System.EventHandler(this.PurchaseReceiptRackAllocation_Load);
            this.Shown += new System.EventHandler(this.PurchaseReceiptRackAllocation_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRacks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblOrderedQty;
        private System.Windows.Forms.DataGridView dgvRacks;
        private System.Windows.Forms.DataGridViewTextBoxColumn Location;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rack;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn RackId;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
