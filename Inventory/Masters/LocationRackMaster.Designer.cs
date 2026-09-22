namespace Inventory.Masters
{
    partial class LocationRackMaster
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.pnlLeftBody = new System.Windows.Forms.Panel();
            this.dgvLocations = new System.Windows.Forms.DataGridView();
            this.colLocationId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocationName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocationDisplayOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocationAllowForSales = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlLocationEditor = new System.Windows.Forms.Panel();
            this.btnCancelLocation = new System.Windows.Forms.Button();
            this.btnSaveLocation = new System.Windows.Forms.Button();
            this.nudDisplayOrder = new System.Windows.Forms.NumericUpDown();
            this.lblDisplayOrder = new System.Windows.Forms.Label();
            this.chkAllowForSales = new System.Windows.Forms.CheckBox();
            this.txtLocationName = new System.Windows.Forms.TextBox();
            this.lblLocationName = new System.Windows.Forms.Label();
            this.pnlLeftButtons = new System.Windows.Forms.Panel();
            this.btnDeleteLocation = new System.Windows.Forms.Button();
            this.btnEditLocation = new System.Windows.Forms.Button();
            this.btnAddLocation = new System.Windows.Forms.Button();
            this.pnlLeftTitle = new System.Windows.Forms.Panel();
            this.lblLocationsTitle = new System.Windows.Forms.Label();
            this.pnlRightBody = new System.Windows.Forms.Panel();
            this.dgvRacks = new System.Windows.Forms.DataGridView();
            this.colRackId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRackCaption = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRackProducts = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRackEdit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.pnlRackEditor = new System.Windows.Forms.Panel();
            this.btnCancelRack = new System.Windows.Forms.Button();
            this.btnSaveRack = new System.Windows.Forms.Button();
            this.txtRackCaption = new System.Windows.Forms.TextBox();
            this.lblRackCaption = new System.Windows.Forms.Label();
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
            this.pnlRackButtons = new System.Windows.Forms.Panel();
            this.btnDeleteRack = new System.Windows.Forms.Button();
            this.btnEditRack = new System.Windows.Forms.Button();
            this.btnAddRacks = new System.Windows.Forms.Button();
            this.pnlRightTitle = new System.Windows.Forms.Panel();
            this.lblLegendText = new System.Windows.Forms.Label();
            this.pnlAssignedLegend = new System.Windows.Forms.Panel();
            this.lblRackTitle = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.pnlLeftBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLocations)).BeginInit();
            this.pnlLocationEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDisplayOrder)).BeginInit();
            this.pnlLeftButtons.SuspendLayout();
            this.pnlLeftTitle.SuspendLayout();
            this.pnlRightBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRacks)).BeginInit();
            this.pnlRackEditor.SuspendLayout();
            this.pnlRackCreate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRackCount)).BeginInit();
            this.pnlRackButtons.SuspendLayout();
            this.pnlRightTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(245)))), ((int)(((byte)(252)))));
            this.pnlHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(984, 48);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClose.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnClose.Location = new System.Drawing.Point(902, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(66, 27);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblTitle.Location = new System.Drawing.Point(16, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(203, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Location && Rack Master";
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 48);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.Controls.Add(this.pnlLeftBody);
            this.splitMain.Panel1.Controls.Add(this.pnlLocationEditor);
            this.splitMain.Panel1.Controls.Add(this.pnlLeftButtons);
            this.splitMain.Panel1.Controls.Add(this.pnlLeftTitle);
            this.splitMain.Panel2.Controls.Add(this.pnlRightBody);
            this.splitMain.Panel2.Controls.Add(this.pnlRackEditor);
            this.splitMain.Panel2.Controls.Add(this.pnlRackCreate);
            this.splitMain.Panel2.Controls.Add(this.pnlRackButtons);
            this.splitMain.Panel2.Controls.Add(this.pnlRightTitle);
            this.splitMain.Size = new System.Drawing.Size(984, 613);
            this.splitMain.SplitterDistance = 350;
            this.splitMain.TabIndex = 1;
            // 
            // pnlLeftBody
            // 
            this.pnlLeftBody.Controls.Add(this.dgvLocations);
            this.pnlLeftBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeftBody.Location = new System.Drawing.Point(0, 42);
            this.pnlLeftBody.Name = "pnlLeftBody";
            this.pnlLeftBody.Padding = new System.Windows.Forms.Padding(10, 8, 8, 8);
            this.pnlLeftBody.Size = new System.Drawing.Size(350, 443);
            this.pnlLeftBody.TabIndex = 1;
            // 
            // dgvLocations
            // 
            this.dgvLocations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLocations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLocationId,
            this.colLocationName,
            this.colLocationDisplayOrder,
            this.colLocationAllowForSales});
            this.dgvLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLocations.Location = new System.Drawing.Point(10, 8);
            this.dgvLocations.Name = "dgvLocations";
            this.dgvLocations.Size = new System.Drawing.Size(332, 427);
            this.dgvLocations.TabIndex = 0;
            this.dgvLocations.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvLocations_CellFormatting);
            this.dgvLocations.SelectionChanged += new System.EventHandler(this.dgvLocations_SelectionChanged);
            // 
            // colLocationId
            // 
            this.colLocationId.DataPropertyName = "LocationId";
            this.colLocationId.HeaderText = "LocationId";
            this.colLocationId.Name = "colLocationId";
            this.colLocationId.Visible = false;
            // 
            // colLocationName
            // 
            this.colLocationName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colLocationName.DataPropertyName = "LocationName";
            this.colLocationName.HeaderText = "Location Name";
            this.colLocationName.Name = "colLocationName";
            // 
            // colLocationDisplayOrder
            // 
            this.colLocationDisplayOrder.DataPropertyName = "DisplayOrder";
            this.colLocationDisplayOrder.HeaderText = "Priority / Display Order";
            this.colLocationDisplayOrder.Name = "colLocationDisplayOrder";
            this.colLocationDisplayOrder.Width = 65;
            // 
            // colLocationAllowForSales
            // 
            this.colLocationAllowForSales.DataPropertyName = "AllowForSales";
            this.colLocationAllowForSales.HeaderText = "Allow For Sales";
            this.colLocationAllowForSales.Name = "colLocationAllowForSales";
            this.colLocationAllowForSales.Width = 75;
            // 
            // pnlLocationEditor
            // 
            this.pnlLocationEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(248)))), ((int)(((byte)(250)))));
            this.pnlLocationEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLocationEditor.Controls.Add(this.btnCancelLocation);
            this.pnlLocationEditor.Controls.Add(this.btnSaveLocation);
            this.pnlLocationEditor.Controls.Add(this.nudDisplayOrder);
            this.pnlLocationEditor.Controls.Add(this.lblDisplayOrder);
            this.pnlLocationEditor.Controls.Add(this.chkAllowForSales);
            this.pnlLocationEditor.Controls.Add(this.txtLocationName);
            this.pnlLocationEditor.Controls.Add(this.lblLocationName);
            this.pnlLocationEditor.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLocationEditor.Location = new System.Drawing.Point(0, 457);
            this.pnlLocationEditor.Name = "pnlLocationEditor";
            this.pnlLocationEditor.Size = new System.Drawing.Size(350, 102);
            this.pnlLocationEditor.TabIndex = 2;
            // 
            // btnCancelLocation
            // 
            this.btnCancelLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelLocation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelLocation.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnCancelLocation.Location = new System.Drawing.Point(272, 69);
            this.btnCancelLocation.Name = "btnCancelLocation";
            this.btnCancelLocation.Size = new System.Drawing.Size(66, 24);
            this.btnCancelLocation.TabIndex = 5;
            this.btnCancelLocation.Text = "Cancel";
            this.btnCancelLocation.UseVisualStyleBackColor = true;
            this.btnCancelLocation.Click += new System.EventHandler(this.btnCancelLocation_Click);
            // 
            // btnSaveLocation
            // 
            this.btnSaveLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveLocation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSaveLocation.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnSaveLocation.Location = new System.Drawing.Point(200, 69);
            this.btnSaveLocation.Name = "btnSaveLocation";
            this.btnSaveLocation.Size = new System.Drawing.Size(66, 24);
            this.btnSaveLocation.TabIndex = 4;
            this.btnSaveLocation.Text = "Save";
            this.btnSaveLocation.UseVisualStyleBackColor = true;
            this.btnSaveLocation.Click += new System.EventHandler(this.btnSaveLocation_Click);
            // 
            // nudDisplayOrder
            // 
            this.nudDisplayOrder.Location = new System.Drawing.Point(95, 71);
            this.nudDisplayOrder.Maximum = new decimal(new int[] {10000, 0, 0, 0});
            this.nudDisplayOrder.Minimum = new decimal(new int[] {1, 0, 0, 0});
            this.nudDisplayOrder.Name = "nudDisplayOrder";
            this.nudDisplayOrder.Size = new System.Drawing.Size(72, 20);
            this.nudDisplayOrder.TabIndex = 3;
            this.nudDisplayOrder.Value = new decimal(new int[] {1, 0, 0, 0});
            // 
            // lblDisplayOrder
            // 
            this.lblDisplayOrder.AutoSize = true;
            this.lblDisplayOrder.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblDisplayOrder.Location = new System.Drawing.Point(10, 73);
            this.lblDisplayOrder.Name = "lblDisplayOrder";
            this.lblDisplayOrder.Size = new System.Drawing.Size(42, 15);
            this.lblDisplayOrder.TabIndex = 2;
            this.lblDisplayOrder.Text = "Priority";
            // 
            // chkAllowForSales
            // 
            this.chkAllowForSales.AutoSize = true;
            this.chkAllowForSales.Checked = true;
            this.chkAllowForSales.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAllowForSales.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.chkAllowForSales.Location = new System.Drawing.Point(95, 43);
            this.chkAllowForSales.Name = "chkAllowForSales";
            this.chkAllowForSales.Size = new System.Drawing.Size(109, 19);
            this.chkAllowForSales.TabIndex = 2;
            this.chkAllowForSales.Text = "Allow For Sales";
            this.chkAllowForSales.UseVisualStyleBackColor = true;
            // 
            // txtLocationName
            // 
            this.txtLocationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocationName.Font = new System.Drawing.Font("Arial", 8.25F);
            this.txtLocationName.Location = new System.Drawing.Point(95, 13);
            this.txtLocationName.MaxLength = 100;
            this.txtLocationName.Name = "txtLocationName";
            this.txtLocationName.Size = new System.Drawing.Size(243, 20);
            this.txtLocationName.TabIndex = 1;
            // 
            // lblLocationName
            // 
            this.lblLocationName.AutoSize = true;
            this.lblLocationName.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblLocationName.Location = new System.Drawing.Point(10, 15);
            this.lblLocationName.Name = "lblLocationName";
            this.lblLocationName.Size = new System.Drawing.Size(84, 15);
            this.lblLocationName.TabIndex = 0;
            this.lblLocationName.Text = "Location Name";
            // 
            // pnlLeftButtons
            // 
            this.pnlLeftButtons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLeftButtons.Controls.Add(this.btnDeleteLocation);
            this.pnlLeftButtons.Controls.Add(this.btnEditLocation);
            this.pnlLeftButtons.Controls.Add(this.btnAddLocation);
            this.pnlLeftButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLeftButtons.Location = new System.Drawing.Point(0, 559);
            this.pnlLeftButtons.Name = "pnlLeftButtons";
            this.pnlLeftButtons.Size = new System.Drawing.Size(350, 54);
            this.pnlLeftButtons.TabIndex = 3;
            // 
            // btnDeleteLocation
            // 
            this.btnDeleteLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteLocation.Enabled = false;
            this.btnDeleteLocation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeleteLocation.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnDeleteLocation.Location = new System.Drawing.Point(226, 13);
            this.btnDeleteLocation.Name = "btnDeleteLocation";
            this.btnDeleteLocation.Size = new System.Drawing.Size(104, 28);
            this.btnDeleteLocation.TabIndex = 2;
            this.btnDeleteLocation.Text = "Delete Location";
            this.btnDeleteLocation.UseVisualStyleBackColor = true;
            this.btnDeleteLocation.Click += new System.EventHandler(this.btnDeleteLocation_Click);
            // 
            // btnEditLocation
            // 
            this.btnEditLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditLocation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEditLocation.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnEditLocation.Location = new System.Drawing.Point(116, 13);
            this.btnEditLocation.Name = "btnEditLocation";
            this.btnEditLocation.Size = new System.Drawing.Size(104, 28);
            this.btnEditLocation.TabIndex = 1;
            this.btnEditLocation.Text = "Edit Location";
            this.btnEditLocation.UseVisualStyleBackColor = true;
            this.btnEditLocation.Click += new System.EventHandler(this.btnEditLocation_Click);
            // 
            // btnAddLocation
            // 
            this.btnAddLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddLocation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddLocation.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnAddLocation.Location = new System.Drawing.Point(10, 13);
            this.btnAddLocation.Name = "btnAddLocation";
            this.btnAddLocation.Size = new System.Drawing.Size(100, 28);
            this.btnAddLocation.TabIndex = 0;
            this.btnAddLocation.Text = "Add Location";
            this.btnAddLocation.UseVisualStyleBackColor = true;
            this.btnAddLocation.Click += new System.EventHandler(this.btnAddLocation_Click);
            // 
            // pnlLeftTitle
            // 
            this.pnlLeftTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(245)))), ((int)(((byte)(252)))));
            this.pnlLeftTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLeftTitle.Controls.Add(this.lblLocationsTitle);
            this.pnlLeftTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLeftTitle.Location = new System.Drawing.Point(0, 0);
            this.pnlLeftTitle.Name = "pnlLeftTitle";
            this.pnlLeftTitle.Size = new System.Drawing.Size(350, 42);
            this.pnlLeftTitle.TabIndex = 0;
            // 
            // lblLocationsTitle
            // 
            this.lblLocationsTitle.AutoSize = true;
            this.lblLocationsTitle.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.lblLocationsTitle.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblLocationsTitle.Location = new System.Drawing.Point(10, 10);
            this.lblLocationsTitle.Name = "lblLocationsTitle";
            this.lblLocationsTitle.Size = new System.Drawing.Size(72, 19);
            this.lblLocationsTitle.TabIndex = 0;
            this.lblLocationsTitle.Text = "Locations";
            // 
            // pnlRightBody
            // 
            this.pnlRightBody.Controls.Add(this.dgvRacks);
            this.pnlRightBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRightBody.Location = new System.Drawing.Point(0, 42);
            this.pnlRightBody.Name = "pnlRightBody";
            this.pnlRightBody.Padding = new System.Windows.Forms.Padding(8, 8, 10, 8);
            this.pnlRightBody.Size = new System.Drawing.Size(630, 367);
            this.pnlRightBody.TabIndex = 1;
            // 
            // dgvRacks
            // 
            this.dgvRacks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRacks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRackId,
            this.colRackCaption,
            this.colRackProducts,
            this.colRackEdit});
            this.dgvRacks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRacks.Location = new System.Drawing.Point(8, 8);
            this.dgvRacks.Name = "dgvRacks";
            this.dgvRacks.Size = new System.Drawing.Size(612, 351);
            this.dgvRacks.TabIndex = 0;
            this.dgvRacks.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRacks_CellClick);
            this.dgvRacks.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRacks_CellContentClick);
            this.dgvRacks.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRacks_CellMouseLeave);
            this.dgvRacks.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRacks_CellMouseMove);
            this.dgvRacks.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvRacks_DataBindingComplete);
            // 
            // colRackId
            // 
            this.colRackId.DataPropertyName = "RackId";
            this.colRackId.HeaderText = "RackId";
            this.colRackId.Name = "colRackId";
            this.colRackId.Visible = false;
            // 
            // colRackCaption
            // 
            this.colRackCaption.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRackCaption.DataPropertyName = "RackCaption";
            this.colRackCaption.HeaderText = "Rack Caption";
            this.colRackCaption.Name = "colRackCaption";
            // 
            // colRackProducts
            // 
            this.colRackProducts.DataPropertyName = "ProductCount";
            this.colRackProducts.HeaderText = "Products";
            this.colRackProducts.Name = "colRackProducts";
            this.colRackProducts.Width = 80;
            // 
            // colRackEdit
            // 
            this.colRackEdit.HeaderText = "Edit";
            this.colRackEdit.Name = "colRackEdit";
            this.colRackEdit.Text = "Edit";
            this.colRackEdit.UseColumnTextForButtonValue = true;
            this.colRackEdit.Width = 60;
            // 
            // pnlRackEditor
            // 
            this.pnlRackEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(248)))), ((int)(((byte)(250)))));
            this.pnlRackEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRackEditor.Controls.Add(this.btnCancelRack);
            this.pnlRackEditor.Controls.Add(this.btnSaveRack);
            this.pnlRackEditor.Controls.Add(this.txtRackCaption);
            this.pnlRackEditor.Controls.Add(this.lblRackCaption);
            this.pnlRackEditor.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRackEditor.Location = new System.Drawing.Point(0, 409);
            this.pnlRackEditor.Name = "pnlRackEditor";
            this.pnlRackEditor.Size = new System.Drawing.Size(630, 56);
            this.pnlRackEditor.TabIndex = 2;
            // 
            // btnCancelRack
            // 
            this.btnCancelRack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelRack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelRack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelRack.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnCancelRack.Location = new System.Drawing.Point(551, 14);
            this.btnCancelRack.Name = "btnCancelRack";
            this.btnCancelRack.Size = new System.Drawing.Size(66, 26);
            this.btnCancelRack.TabIndex = 3;
            this.btnCancelRack.Text = "Cancel";
            this.btnCancelRack.UseVisualStyleBackColor = true;
            this.btnCancelRack.Click += new System.EventHandler(this.btnCancelRack_Click);
            // 
            // btnSaveRack
            // 
            this.btnSaveRack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveRack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveRack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSaveRack.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnSaveRack.Location = new System.Drawing.Point(479, 14);
            this.btnSaveRack.Name = "btnSaveRack";
            this.btnSaveRack.Size = new System.Drawing.Size(66, 26);
            this.btnSaveRack.TabIndex = 2;
            this.btnSaveRack.Text = "Save";
            this.btnSaveRack.UseVisualStyleBackColor = true;
            this.btnSaveRack.Click += new System.EventHandler(this.btnSaveRack_Click);
            // 
            // txtRackCaption
            // 
            this.txtRackCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRackCaption.Font = new System.Drawing.Font("Arial", 8.25F);
            this.txtRackCaption.Location = new System.Drawing.Point(99, 16);
            this.txtRackCaption.MaxLength = 100;
            this.txtRackCaption.Name = "txtRackCaption";
            this.txtRackCaption.Size = new System.Drawing.Size(366, 20);
            this.txtRackCaption.TabIndex = 1;
            // 
            // lblRackCaption
            // 
            this.lblRackCaption.AutoSize = true;
            this.lblRackCaption.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblRackCaption.Location = new System.Drawing.Point(10, 18);
            this.lblRackCaption.Name = "lblRackCaption";
            this.lblRackCaption.Size = new System.Drawing.Size(77, 15);
            this.lblRackCaption.TabIndex = 0;
            this.lblRackCaption.Text = "Rack Caption";
            // 
            // pnlRackCreate
            // 
            this.pnlRackCreate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(248)))), ((int)(((byte)(250)))));
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
            this.pnlRackCreate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRackCreate.Location = new System.Drawing.Point(0, 465);
            this.pnlRackCreate.Name = "pnlRackCreate";
            this.pnlRackCreate.Size = new System.Drawing.Size(630, 94);
            this.pnlRackCreate.TabIndex = 3;
            // 
            // flpManualRacks
            // 
            this.flpManualRacks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flpManualRacks.AutoScroll = true;
            this.flpManualRacks.Location = new System.Drawing.Point(13, 58);
            this.flpManualRacks.Name = "flpManualRacks";
            this.flpManualRacks.Size = new System.Drawing.Size(444, 28);
            this.flpManualRacks.TabIndex = 8;
            // 
            // btnCancelRackCreate
            // 
            this.btnCancelRackCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelRackCreate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelRackCreate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelRackCreate.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnCancelRackCreate.Location = new System.Drawing.Point(551, 57);
            this.btnCancelRackCreate.Name = "btnCancelRackCreate";
            this.btnCancelRackCreate.Size = new System.Drawing.Size(66, 26);
            this.btnCancelRackCreate.TabIndex = 9;
            this.btnCancelRackCreate.Text = "Cancel";
            this.btnCancelRackCreate.UseVisualStyleBackColor = true;
            this.btnCancelRackCreate.Click += new System.EventHandler(this.btnCancelRackCreate_Click);
            // 
            // btnCreateRacks
            // 
            this.btnCreateRacks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateRacks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreateRacks.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCreateRacks.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnCreateRacks.Location = new System.Drawing.Point(479, 57);
            this.btnCreateRacks.Name = "btnCreateRacks";
            this.btnCreateRacks.Size = new System.Drawing.Size(66, 26);
            this.btnCreateRacks.TabIndex = 7;
            this.btnCreateRacks.Text = "Create";
            this.btnCreateRacks.UseVisualStyleBackColor = true;
            this.btnCreateRacks.Click += new System.EventHandler(this.btnCreateRacks_Click);
            // 
            // txtPrefix
            // 
            this.txtPrefix.Font = new System.Drawing.Font("Arial", 8.25F);
            this.txtPrefix.Location = new System.Drawing.Point(459, 28);
            this.txtPrefix.MaxLength = 20;
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(112, 20);
            this.txtPrefix.TabIndex = 6;
            // 
            // lblPrefix
            // 
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblPrefix.Location = new System.Drawing.Point(413, 31);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(38, 15);
            this.lblPrefix.TabIndex = 5;
            this.lblPrefix.Text = "Prefix";
            // 
            // rdoManual
            // 
            this.rdoManual.AutoSize = true;
            this.rdoManual.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.rdoManual.Location = new System.Drawing.Point(334, 29);
            this.rdoManual.Name = "rdoManual";
            this.rdoManual.Size = new System.Drawing.Size(65, 19);
            this.rdoManual.TabIndex = 4;
            this.rdoManual.Text = "Manual";
            this.rdoManual.UseVisualStyleBackColor = true;
            this.rdoManual.CheckedChanged += new System.EventHandler(this.rdoManual_CheckedChanged);
            // 
            // rdoAuto
            // 
            this.rdoAuto.AutoSize = true;
            this.rdoAuto.Checked = true;
            this.rdoAuto.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.rdoAuto.Location = new System.Drawing.Point(276, 29);
            this.rdoAuto.Name = "rdoAuto";
            this.rdoAuto.Size = new System.Drawing.Size(51, 19);
            this.rdoAuto.TabIndex = 3;
            this.rdoAuto.TabStop = true;
            this.rdoAuto.Text = "Auto";
            this.rdoAuto.UseVisualStyleBackColor = true;
            this.rdoAuto.CheckedChanged += new System.EventHandler(this.rdoAuto_CheckedChanged);
            // 
            // nudRackCount
            // 
            this.nudRackCount.Location = new System.Drawing.Point(128, 29);
            this.nudRackCount.Maximum = new decimal(new int[] {1000, 0, 0, 0});
            this.nudRackCount.Minimum = new decimal(new int[] {1, 0, 0, 0});
            this.nudRackCount.Name = "nudRackCount";
            this.nudRackCount.Size = new System.Drawing.Size(70, 20);
            this.nudRackCount.TabIndex = 2;
            this.nudRackCount.Value = new decimal(new int[] {1, 0, 0, 0});
            this.nudRackCount.ValueChanged += new System.EventHandler(this.nudRackCount_ValueChanged);
            // 
            // lblNumber
            // 
            this.lblNumber.AutoSize = true;
            this.lblNumber.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblNumber.Location = new System.Drawing.Point(10, 31);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size(100, 15);
            this.lblNumber.TabIndex = 1;
            this.lblNumber.Text = "Number of Racks";
            // 
            // lblRackCreateTitle
            // 
            this.lblRackCreateTitle.AutoSize = true;
            this.lblRackCreateTitle.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblRackCreateTitle.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblRackCreateTitle.Location = new System.Drawing.Point(10, 8);
            this.lblRackCreateTitle.Name = "lblRackCreateTitle";
            this.lblRackCreateTitle.Size = new System.Drawing.Size(68, 15);
            this.lblRackCreateTitle.TabIndex = 0;
            this.lblRackCreateTitle.Text = "Rack Setup";
            // 
            // pnlRackButtons
            // 
            this.pnlRackButtons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRackButtons.Controls.Add(this.btnDeleteRack);
            this.pnlRackButtons.Controls.Add(this.btnEditRack);
            this.pnlRackButtons.Controls.Add(this.btnAddRacks);
            this.pnlRackButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRackButtons.Location = new System.Drawing.Point(0, 559);
            this.pnlRackButtons.Name = "pnlRackButtons";
            this.pnlRackButtons.Size = new System.Drawing.Size(630, 54);
            this.pnlRackButtons.TabIndex = 4;
            // 
            // btnDeleteRack
            // 
            this.btnDeleteRack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteRack.Enabled = false;
            this.btnDeleteRack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeleteRack.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnDeleteRack.Location = new System.Drawing.Point(222, 13);
            this.btnDeleteRack.Name = "btnDeleteRack";
            this.btnDeleteRack.Size = new System.Drawing.Size(90, 28);
            this.btnDeleteRack.TabIndex = 2;
            this.btnDeleteRack.Text = "Delete Rack";
            this.btnDeleteRack.UseVisualStyleBackColor = true;
            this.btnDeleteRack.Click += new System.EventHandler(this.btnDeleteRack_Click);
            // 
            // btnEditRack
            // 
            this.btnEditRack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditRack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEditRack.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnEditRack.Location = new System.Drawing.Point(116, 13);
            this.btnEditRack.Name = "btnEditRack";
            this.btnEditRack.Size = new System.Drawing.Size(90, 28);
            this.btnEditRack.TabIndex = 1;
            this.btnEditRack.Text = "Edit Rack";
            this.btnEditRack.UseVisualStyleBackColor = true;
            this.btnEditRack.Click += new System.EventHandler(this.btnEditRack_Click);
            // 
            // btnAddRacks
            // 
            this.btnAddRacks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddRacks.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddRacks.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnAddRacks.Location = new System.Drawing.Point(10, 13);
            this.btnAddRacks.Name = "btnAddRacks";
            this.btnAddRacks.Size = new System.Drawing.Size(90, 28);
            this.btnAddRacks.TabIndex = 0;
            this.btnAddRacks.Text = "Add Rack(s)";
            this.btnAddRacks.UseVisualStyleBackColor = true;
            this.btnAddRacks.Click += new System.EventHandler(this.btnAddRacks_Click);
            // 
            // pnlRightTitle
            // 
            this.pnlRightTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(245)))), ((int)(((byte)(252)))));
            this.pnlRightTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRightTitle.Controls.Add(this.lblLegendText);
            this.pnlRightTitle.Controls.Add(this.pnlAssignedLegend);
            this.pnlRightTitle.Controls.Add(this.lblRackTitle);
            this.pnlRightTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRightTitle.Location = new System.Drawing.Point(0, 0);
            this.pnlRightTitle.Name = "pnlRightTitle";
            this.pnlRightTitle.Size = new System.Drawing.Size(630, 42);
            this.pnlRightTitle.TabIndex = 0;
            // 
            // lblLegendText
            // 
            this.lblLegendText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLegendText.AutoSize = true;
            this.lblLegendText.Font = new System.Drawing.Font("Calibri", 9F);
            this.lblLegendText.Location = new System.Drawing.Point(468, 13);
            this.lblLegendText.Name = "lblLegendText";
            this.lblLegendText.Size = new System.Drawing.Size(149, 14);
            this.lblLegendText.TabIndex = 2;
            this.lblLegendText.Text = "= Rack assigned to product(s)";
            // 
            // pnlAssignedLegend
            // 
            this.pnlAssignedLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlAssignedLegend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(246)))), ((int)(((byte)(226)))));
            this.pnlAssignedLegend.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAssignedLegend.Location = new System.Drawing.Point(440, 12);
            this.pnlAssignedLegend.Name = "pnlAssignedLegend";
            this.pnlAssignedLegend.Size = new System.Drawing.Size(22, 16);
            this.pnlAssignedLegend.TabIndex = 1;
            // 
            // lblRackTitle
            // 
            this.lblRackTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRackTitle.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.lblRackTitle.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblRackTitle.Location = new System.Drawing.Point(10, 10);
            this.lblRackTitle.Name = "lblRackTitle";
            this.lblRackTitle.Size = new System.Drawing.Size(420, 20);
            this.lblRackTitle.TabIndex = 0;
            this.lblRackTitle.Text = "Racks";
            // 
            // LocationRackMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlHeader);
            this.Name = "LocationRackMaster";
            this.Text = "Location & Rack Master";
            this.Load += new System.EventHandler(this.LocationRackMaster_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.pnlLeftBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLocations)).EndInit();
            this.pnlLocationEditor.ResumeLayout(false);
            this.pnlLocationEditor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDisplayOrder)).EndInit();
            this.pnlLeftButtons.ResumeLayout(false);
            this.pnlLeftTitle.ResumeLayout(false);
            this.pnlLeftTitle.PerformLayout();
            this.pnlRightBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRacks)).EndInit();
            this.pnlRackEditor.ResumeLayout(false);
            this.pnlRackEditor.PerformLayout();
            this.pnlRackCreate.ResumeLayout(false);
            this.pnlRackCreate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRackCount)).EndInit();
            this.pnlRackButtons.ResumeLayout(false);
            this.pnlRightTitle.ResumeLayout(false);
            this.pnlRightTitle.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Panel pnlLeftTitle;
        private System.Windows.Forms.Label lblLocationsTitle;
        private System.Windows.Forms.Panel pnlLeftBody;
        private System.Windows.Forms.DataGridView dgvLocations;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocationId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocationName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocationDisplayOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocationAllowForSales;
        private System.Windows.Forms.Panel pnlLocationEditor;
        private System.Windows.Forms.Button btnCancelLocation;
        private System.Windows.Forms.Button btnSaveLocation;
        private System.Windows.Forms.NumericUpDown nudDisplayOrder;
        private System.Windows.Forms.Label lblDisplayOrder;
        private System.Windows.Forms.CheckBox chkAllowForSales;
        private System.Windows.Forms.TextBox txtLocationName;
        private System.Windows.Forms.Label lblLocationName;
        private System.Windows.Forms.Panel pnlLeftButtons;
        private System.Windows.Forms.Button btnDeleteLocation;
        private System.Windows.Forms.Button btnEditLocation;
        private System.Windows.Forms.Button btnAddLocation;
        private System.Windows.Forms.Panel pnlRightTitle;
        private System.Windows.Forms.Label lblLegendText;
        private System.Windows.Forms.Panel pnlAssignedLegend;
        private System.Windows.Forms.Label lblRackTitle;
        private System.Windows.Forms.Panel pnlRightBody;
        private System.Windows.Forms.DataGridView dgvRacks;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRackId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRackCaption;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRackProducts;
        private System.Windows.Forms.DataGridViewButtonColumn colRackEdit;
        private System.Windows.Forms.Panel pnlRackEditor;
        private System.Windows.Forms.Button btnCancelRack;
        private System.Windows.Forms.Button btnSaveRack;
        private System.Windows.Forms.TextBox txtRackCaption;
        private System.Windows.Forms.Label lblRackCaption;
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
        private System.Windows.Forms.Panel pnlRackButtons;
        private System.Windows.Forms.Button btnDeleteRack;
        private System.Windows.Forms.Button btnEditRack;
        private System.Windows.Forms.Button btnAddRacks;
    }
}
