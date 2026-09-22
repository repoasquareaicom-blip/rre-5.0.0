namespace Inventory.Masters
{
    partial class LocationRackSectionControl
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
            this.pnlLocationHeader = new System.Windows.Forms.Panel();
            this.btnAddRacks = new System.Windows.Forms.Button();
            this.btnCancelLocation = new System.Windows.Forms.Button();
            this.btnSaveLocation = new System.Windows.Forms.Button();
            this.btnEditLocation = new System.Windows.Forms.Button();
            this.nudDisplayOrder = new System.Windows.Forms.NumericUpDown();
            this.lblPriority = new System.Windows.Forms.Label();
            this.txtLocationName = new System.Windows.Forms.TextBox();
            this.lblLocationEditName = new System.Windows.Forms.Label();
            this.lblLocationPriority = new System.Windows.Forms.Label();
            this.lblLocationName = new System.Windows.Forms.Label();
            this.pnlRackCreate = new System.Windows.Forms.Panel();
            this.flpManualRacks = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancelRackCreate = new System.Windows.Forms.Button();
            this.btnCreateRacks = new System.Windows.Forms.Button();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.rdoManual = new System.Windows.Forms.RadioButton();
            this.rdoAuto = new System.Windows.Forms.RadioButton();
            this.nudRackCount = new System.Windows.Forms.NumericUpDown();
            this.lblNumber = new System.Windows.Forms.Label();
            this.lblRackCreateTitle = new System.Windows.Forms.Label();
            this.flpRacks = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddMoreRacks = new System.Windows.Forms.Button();
            this.pnlLocationHeader.SuspendLayout();
            this.pnlRackCreate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDisplayOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRackCount)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlLocationHeader
            // 
            this.pnlLocationHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlLocationHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLocationHeader.Controls.Add(this.btnAddRacks);
            this.pnlLocationHeader.Controls.Add(this.btnCancelLocation);
            this.pnlLocationHeader.Controls.Add(this.btnSaveLocation);
            this.pnlLocationHeader.Controls.Add(this.btnEditLocation);
            this.pnlLocationHeader.Controls.Add(this.nudDisplayOrder);
            this.pnlLocationHeader.Controls.Add(this.lblPriority);
            this.pnlLocationHeader.Controls.Add(this.txtLocationName);
            this.pnlLocationHeader.Controls.Add(this.lblLocationEditName);
            this.pnlLocationHeader.Controls.Add(this.lblLocationPriority);
            this.pnlLocationHeader.Controls.Add(this.lblLocationName);
            this.pnlLocationHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlLocationHeader.Name = "pnlLocationHeader";
            this.pnlLocationHeader.Size = new System.Drawing.Size(900, 42);
            this.pnlLocationHeader.TabIndex = 0;
            // 
            // btnAddRacks
            // 
            this.btnAddRacks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddRacks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddRacks.FlatAppearance.BorderSize = 0;
            this.btnAddRacks.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddRacks.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnAddRacks.Location = new System.Drawing.Point(782, 8);
            this.btnAddRacks.Name = "btnAddRacks";
            this.btnAddRacks.Size = new System.Drawing.Size(103, 26);
            this.btnAddRacks.TabIndex = 5;
            this.btnAddRacks.Text = "Add Rack(s)";
            this.btnAddRacks.UseVisualStyleBackColor = true;
            this.btnAddRacks.Click += new System.EventHandler(this.btnAddRacks_Click);
            // 
            // btnCancelLocation
            // 
            this.btnCancelLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelLocation.FlatAppearance.BorderSize = 0;
            this.btnCancelLocation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelLocation.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnCancelLocation.Location = new System.Drawing.Point(708, 8);
            this.btnCancelLocation.Name = "btnCancelLocation";
            this.btnCancelLocation.Size = new System.Drawing.Size(68, 26);
            this.btnCancelLocation.TabIndex = 4;
            this.btnCancelLocation.Text = "Cancel";
            this.btnCancelLocation.UseVisualStyleBackColor = true;
            this.btnCancelLocation.Click += new System.EventHandler(this.btnCancelLocation_Click);
            // 
            // btnSaveLocation
            // 
            this.btnSaveLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveLocation.FlatAppearance.BorderSize = 0;
            this.btnSaveLocation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSaveLocation.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnSaveLocation.Location = new System.Drawing.Point(642, 8);
            this.btnSaveLocation.Name = "btnSaveLocation";
            this.btnSaveLocation.Size = new System.Drawing.Size(60, 26);
            this.btnSaveLocation.TabIndex = 3;
            this.btnSaveLocation.Text = "Save";
            this.btnSaveLocation.UseVisualStyleBackColor = true;
            this.btnSaveLocation.Click += new System.EventHandler(this.btnSaveLocation_Click);
            // 
            // btnEditLocation
            // 
            this.btnEditLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditLocation.FlatAppearance.BorderSize = 0;
            this.btnEditLocation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEditLocation.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnEditLocation.Location = new System.Drawing.Point(716, 8);
            this.btnEditLocation.Name = "btnEditLocation";
            this.btnEditLocation.Size = new System.Drawing.Size(60, 26);
            this.btnEditLocation.TabIndex = 2;
            this.btnEditLocation.Text = "Edit";
            this.btnEditLocation.UseVisualStyleBackColor = true;
            this.btnEditLocation.Click += new System.EventHandler(this.btnEditLocation_Click);
            // 
            // nudDisplayOrder
            // 
            this.nudDisplayOrder.Location = new System.Drawing.Point(511, 11);
            this.nudDisplayOrder.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudDisplayOrder.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDisplayOrder.Name = "nudDisplayOrder";
            this.nudDisplayOrder.Size = new System.Drawing.Size(72, 20);
            this.nudDisplayOrder.TabIndex = 7;
            this.nudDisplayOrder.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblPriority
            // 
            this.lblPriority.AutoSize = true;
            this.lblPriority.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblPriority.Location = new System.Drawing.Point(423, 14);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(68, 13);
            this.lblPriority.TabIndex = 6;
            this.lblPriority.Text = "Sales Priority";
            // 
            // txtLocationName
            // 
            this.txtLocationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocationName.Font = new System.Drawing.Font("Arial", 8.25F);
            this.txtLocationName.Location = new System.Drawing.Point(13, 10);
            this.txtLocationName.MaxLength = 100;
            this.txtLocationName.Name = "txtLocationName";
            this.txtLocationName.Size = new System.Drawing.Size(610, 23);
            this.txtLocationName.TabIndex = 1;
            // 
            // lblLocationEditName
            // 
            this.lblLocationEditName.AutoSize = true;
            this.lblLocationEditName.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblLocationEditName.Location = new System.Drawing.Point(13, 14);
            this.lblLocationEditName.Name = "lblLocationEditName";
            this.lblLocationEditName.Size = new System.Drawing.Size(60, 13);
            this.lblLocationEditName.TabIndex = 8;
            this.lblLocationEditName.Text = "Location Name";
            // 
            // lblLocationPriority
            // 
            this.lblLocationPriority.AutoSize = true;
            this.lblLocationPriority.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblLocationPriority.Location = new System.Drawing.Point(423, 14);
            this.lblLocationPriority.Name = "lblLocationPriority";
            this.lblLocationPriority.Size = new System.Drawing.Size(49, 13);
            this.lblLocationPriority.TabIndex = 6;
            this.lblLocationPriority.Text = "Priority: 1";
            // 
            // lblLocationName
            // 
            this.lblLocationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLocationName.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.lblLocationName.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblLocationName.Location = new System.Drawing.Point(13, 9);
            this.lblLocationName.Name = "lblLocationName";
            this.lblLocationName.Size = new System.Drawing.Size(690, 24);
            this.lblLocationName.TabIndex = 0;
            this.lblLocationName.Text = "Location Name";
            this.lblLocationName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlRackCreate
            // 
            this.pnlRackCreate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRackCreate.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlRackCreate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRackCreate.Controls.Add(this.flpManualRacks);
            this.pnlRackCreate.Controls.Add(this.btnCancelRackCreate);
            this.pnlRackCreate.Controls.Add(this.btnCreateRacks);
            this.pnlRackCreate.Controls.Add(this.txtPrefix);
            this.pnlRackCreate.Controls.Add(this.lblPrefix);
            this.pnlRackCreate.Controls.Add(this.rdoManual);
            this.pnlRackCreate.Controls.Add(this.rdoAuto);
            this.pnlRackCreate.Controls.Add(this.nudRackCount);
            this.pnlRackCreate.Controls.Add(this.lblNumber);
            this.pnlRackCreate.Controls.Add(this.lblRackCreateTitle);
            this.pnlRackCreate.Location = new System.Drawing.Point(12, 52);
            this.pnlRackCreate.Name = "pnlRackCreate";
            this.pnlRackCreate.Size = new System.Drawing.Size(876, 150);
            this.pnlRackCreate.TabIndex = 1;
            // 
            // flpManualRacks
            // 
            this.flpManualRacks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flpManualRacks.AutoScroll = true;
            this.flpManualRacks.Location = new System.Drawing.Point(17, 72);
            this.flpManualRacks.Name = "flpManualRacks";
            this.flpManualRacks.Size = new System.Drawing.Size(728, 66);
            this.flpManualRacks.TabIndex = 9;
            // 
            // btnCancelRackCreate
            // 
            this.btnCancelRackCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelRackCreate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelRackCreate.FlatAppearance.BorderSize = 0;
            this.btnCancelRackCreate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelRackCreate.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnCancelRackCreate.Location = new System.Drawing.Point(788, 103);
            this.btnCancelRackCreate.Name = "btnCancelRackCreate";
            this.btnCancelRackCreate.Size = new System.Drawing.Size(74, 26);
            this.btnCancelRackCreate.TabIndex = 8;
            this.btnCancelRackCreate.Text = "Cancel";
            this.btnCancelRackCreate.UseVisualStyleBackColor = true;
            this.btnCancelRackCreate.Click += new System.EventHandler(this.btnCancelRackCreate_Click);
            // 
            // btnCreateRacks
            // 
            this.btnCreateRacks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateRacks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreateRacks.FlatAppearance.BorderSize = 0;
            this.btnCreateRacks.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCreateRacks.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnCreateRacks.Location = new System.Drawing.Point(788, 71);
            this.btnCreateRacks.Name = "btnCreateRacks";
            this.btnCreateRacks.Size = new System.Drawing.Size(74, 26);
            this.btnCreateRacks.TabIndex = 7;
            this.btnCreateRacks.Text = "Create";
            this.btnCreateRacks.UseVisualStyleBackColor = true;
            this.btnCreateRacks.Click += new System.EventHandler(this.btnCreateRacks_Click);
            // 
            // txtPrefix
            // 
            this.txtPrefix.Location = new System.Drawing.Point(554, 38);
            this.txtPrefix.MaxLength = 20;
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(132, 20);
            this.txtPrefix.Font = new System.Drawing.Font("Arial", 8.25F);
            this.txtPrefix.TabIndex = 6;
            // 
            // lblPrefix
            // 
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblPrefix.Location = new System.Drawing.Point(509, 41);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(33, 13);
            this.lblPrefix.TabIndex = 5;
            this.lblPrefix.Text = "Prefix";
            // 
            // rdoManual
            // 
            this.rdoManual.AutoSize = true;
            this.rdoManual.BackColor = System.Drawing.Color.Transparent;
            this.rdoManual.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.rdoManual.Location = new System.Drawing.Point(384, 39);
            this.rdoManual.Name = "rdoManual";
            this.rdoManual.Size = new System.Drawing.Size(60, 17);
            this.rdoManual.TabIndex = 4;
            this.rdoManual.Text = "Manual";
            this.rdoManual.UseVisualStyleBackColor = false;
            this.rdoManual.CheckedChanged += new System.EventHandler(this.rdoManual_CheckedChanged);
            // 
            // rdoAuto
            // 
            this.rdoAuto.AutoSize = true;
            this.rdoAuto.BackColor = System.Drawing.Color.Transparent;
            this.rdoAuto.Checked = true;
            this.rdoAuto.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.rdoAuto.Location = new System.Drawing.Point(326, 39);
            this.rdoAuto.Name = "rdoAuto";
            this.rdoAuto.Size = new System.Drawing.Size(47, 17);
            this.rdoAuto.TabIndex = 3;
            this.rdoAuto.TabStop = true;
            this.rdoAuto.Text = "Auto";
            this.rdoAuto.UseVisualStyleBackColor = false;
            this.rdoAuto.CheckedChanged += new System.EventHandler(this.rdoAuto_CheckedChanged);
            // 
            // nudRackCount
            // 
            this.nudRackCount.Location = new System.Drawing.Point(129, 38);
            this.nudRackCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudRackCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRackCount.Name = "nudRackCount";
            this.nudRackCount.Size = new System.Drawing.Size(75, 20);
            this.nudRackCount.TabIndex = 2;
            this.nudRackCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRackCount.ValueChanged += new System.EventHandler(this.nudRackCount_ValueChanged);
            // 
            // lblNumber
            // 
            this.lblNumber.AutoSize = true;
            this.lblNumber.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblNumber.Location = new System.Drawing.Point(14, 41);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size(89, 13);
            this.lblNumber.TabIndex = 1;
            this.lblNumber.Text = "Number of Racks";
            // 
            // lblRackCreateTitle
            // 
            this.lblRackCreateTitle.AutoSize = true;
            this.lblRackCreateTitle.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblRackCreateTitle.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblRackCreateTitle.Location = new System.Drawing.Point(14, 12);
            this.lblRackCreateTitle.Name = "lblRackCreateTitle";
            this.lblRackCreateTitle.Size = new System.Drawing.Size(119, 15);
            this.lblRackCreateTitle.TabIndex = 0;
            this.lblRackCreateTitle.Text = "Rack Setup";
            // 
            // flpRacks
            // 
            this.flpRacks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flpRacks.AutoSize = true;
            this.flpRacks.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpRacks.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flpRacks.Location = new System.Drawing.Point(12, 213);
            this.flpRacks.MinimumSize = new System.Drawing.Size(876, 1);
            this.flpRacks.Name = "flpRacks";
            this.flpRacks.Size = new System.Drawing.Size(876, 1);
            this.flpRacks.TabIndex = 2;
            this.flpRacks.WrapContents = true;
            // 
            // btnAddMoreRacks
            // 
            this.btnAddMoreRacks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddMoreRacks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddMoreRacks.FlatAppearance.BorderSize = 0;
            this.btnAddMoreRacks.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddMoreRacks.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnAddMoreRacks.Location = new System.Drawing.Point(12, 307);
            this.btnAddMoreRacks.Name = "btnAddMoreRacks";
            this.btnAddMoreRacks.Size = new System.Drawing.Size(122, 27);
            this.btnAddMoreRacks.TabIndex = 3;
            this.btnAddMoreRacks.Text = "Add More Racks";
            this.btnAddMoreRacks.UseVisualStyleBackColor = true;
            this.btnAddMoreRacks.Click += new System.EventHandler(this.btnAddMoreRacks_Click);
            // 
            // LocationRackSectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnAddMoreRacks);
            this.Controls.Add(this.flpRacks);
            this.Controls.Add(this.pnlRackCreate);
            this.Controls.Add(this.pnlLocationHeader);
            this.Name = "LocationRackSectionControl";
            this.Size = new System.Drawing.Size(900, 92);
            this.pnlLocationHeader.ResumeLayout(false);
            this.pnlLocationHeader.PerformLayout();
            this.pnlRackCreate.ResumeLayout(false);
            this.pnlRackCreate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDisplayOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRackCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlLocationHeader;
        private System.Windows.Forms.Button btnAddRacks;
        private System.Windows.Forms.Button btnCancelLocation;
        private System.Windows.Forms.Button btnSaveLocation;
        private System.Windows.Forms.Button btnEditLocation;
        private System.Windows.Forms.NumericUpDown nudDisplayOrder;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.TextBox txtLocationName;
        private System.Windows.Forms.Label lblLocationEditName;
        private System.Windows.Forms.Label lblLocationPriority;
        private System.Windows.Forms.Label lblLocationName;
        private System.Windows.Forms.Panel pnlRackCreate;
        private System.Windows.Forms.FlowLayoutPanel flpManualRacks;
        private System.Windows.Forms.Button btnCancelRackCreate;
        private System.Windows.Forms.Button btnCreateRacks;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.RadioButton rdoManual;
        private System.Windows.Forms.RadioButton rdoAuto;
        private System.Windows.Forms.NumericUpDown nudRackCount;
        private System.Windows.Forms.Label lblNumber;
        private System.Windows.Forms.Label lblRackCreateTitle;
        private System.Windows.Forms.FlowLayoutPanel flpRacks;
        private System.Windows.Forms.Button btnAddMoreRacks;
    }
}
