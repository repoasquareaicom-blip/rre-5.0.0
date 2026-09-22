namespace Inventory.Masters
{
    partial class ProductLocationRackAssignmentControl
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.lblLocationName = new System.Windows.Forms.Label();
            this.btnShowAddRacks = new System.Windows.Forms.Button();
            this.pnlRackSelector = new System.Windows.Forms.Panel();
            this.cmbRackSelector = new System.Windows.Forms.ComboBox();
            this.btnAddRack = new System.Windows.Forms.Button();
            this.btnCancelAddRack = new System.Windows.Forms.Button();
            this.flpSelectedRacks = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlRackSelector.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLocationName
            // 
            this.lblLocationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLocationName.AutoSize = false;
            this.lblLocationName.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocationName.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblLocationName.Location = new System.Drawing.Point(0, 0);
            this.lblLocationName.Name = "lblLocationName";
            this.lblLocationName.Size = new System.Drawing.Size(758, 22);
            this.lblLocationName.TabIndex = 0;
            this.lblLocationName.Text = "Location Name";
            this.lblLocationName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnShowAddRacks
            // 
            this.btnShowAddRacks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowAddRacks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShowAddRacks.FlatAppearance.BorderSize = 0;
            this.btnShowAddRacks.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnShowAddRacks.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowAddRacks.Location = new System.Drawing.Point(764, 0);
            this.btnShowAddRacks.Name = "btnShowAddRacks";
            this.btnShowAddRacks.Size = new System.Drawing.Size(126, 24);
            this.btnShowAddRacks.TabIndex = 1;
            this.btnShowAddRacks.Text = "+ Add Racks";
            this.btnShowAddRacks.UseVisualStyleBackColor = true;
            this.btnShowAddRacks.Click += new System.EventHandler(this.btnShowAddRacks_Click);
            // 
            // pnlRackSelector
            // 
            this.pnlRackSelector.Controls.Add(this.cmbRackSelector);
            this.pnlRackSelector.Controls.Add(this.btnAddRack);
            this.pnlRackSelector.Controls.Add(this.btnCancelAddRack);
            this.pnlRackSelector.Location = new System.Drawing.Point(0, 28);
            this.pnlRackSelector.Name = "pnlRackSelector";
            this.pnlRackSelector.Size = new System.Drawing.Size(315, 28);
            this.pnlRackSelector.TabIndex = 2;
            this.pnlRackSelector.Visible = false;
            // 
            // cmbRackSelector
            // 
            this.cmbRackSelector.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbRackSelector.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cmbRackSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbRackSelector.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbRackSelector.FormattingEnabled = true;
            this.cmbRackSelector.Location = new System.Drawing.Point(0, 3);
            this.cmbRackSelector.Name = "cmbRackSelector";
            this.cmbRackSelector.Size = new System.Drawing.Size(205, 22);
            this.cmbRackSelector.TabIndex = 0;
            this.cmbRackSelector.SelectionChangeCommitted += new System.EventHandler(this.cmbRackSelector_SelectionChangeCommitted);
            this.cmbRackSelector.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbRackSelector_KeyDown);
            // 
            // btnAddRack
            // 
            this.btnAddRack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddRack.FlatAppearance.BorderSize = 0;
            this.btnAddRack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddRack.Font = new System.Drawing.Font("Calibri", 8.25F);
            this.btnAddRack.Location = new System.Drawing.Point(211, 2);
            this.btnAddRack.Name = "btnAddRack";
            this.btnAddRack.Size = new System.Drawing.Size(44, 24);
            this.btnAddRack.TabIndex = 1;
            this.btnAddRack.Text = "Add";
            this.btnAddRack.UseVisualStyleBackColor = true;
            this.btnAddRack.Click += new System.EventHandler(this.btnAddRack_Click);
            // 
            // btnCancelAddRack
            // 
            this.btnCancelAddRack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelAddRack.FlatAppearance.BorderSize = 0;
            this.btnCancelAddRack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelAddRack.Font = new System.Drawing.Font("Calibri", 8.25F);
            this.btnCancelAddRack.Location = new System.Drawing.Point(260, 2);
            this.btnCancelAddRack.Name = "btnCancelAddRack";
            this.btnCancelAddRack.Size = new System.Drawing.Size(52, 24);
            this.btnCancelAddRack.TabIndex = 2;
            this.btnCancelAddRack.Text = "Cancel";
            this.btnCancelAddRack.UseVisualStyleBackColor = true;
            this.btnCancelAddRack.Click += new System.EventHandler(this.btnCancelAddRack_Click);
            // 
            // flpSelectedRacks
            // 
            this.flpSelectedRacks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpSelectedRacks.AutoScroll = false;
            this.flpSelectedRacks.Location = new System.Drawing.Point(0, 30);
            this.flpSelectedRacks.Name = "flpSelectedRacks";
            this.flpSelectedRacks.Size = new System.Drawing.Size(890, 32);
            this.flpSelectedRacks.TabIndex = 3;
            this.flpSelectedRacks.WrapContents = true;
            // 
            // ProductLocationRackAssignmentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.flpSelectedRacks);
            this.Controls.Add(this.pnlRackSelector);
            this.Controls.Add(this.btnShowAddRacks);
            this.Controls.Add(this.lblLocationName);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ProductLocationRackAssignmentControl";
            this.Size = new System.Drawing.Size(890, 70);
            this.SizeChanged += new System.EventHandler(this.ProductLocationRackAssignmentControl_SizeChanged);
            this.pnlRackSelector.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblLocationName;
        private System.Windows.Forms.Button btnShowAddRacks;
        private System.Windows.Forms.Panel pnlRackSelector;
        private System.Windows.Forms.ComboBox cmbRackSelector;
        private System.Windows.Forms.Button btnAddRack;
        private System.Windows.Forms.Button btnCancelAddRack;
        private System.Windows.Forms.FlowLayoutPanel flpSelectedRacks;
    }
}
