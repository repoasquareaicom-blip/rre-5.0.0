namespace Inventory.Movement
{
    partial class FrmIssuedReceivedReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel15 = new System.Windows.Forms.Panel();
            this.dgvIssuedReceivedReport = new System.Windows.Forms.DataGridView();
            this.SNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Issued = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Received = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Balance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ddlcustomer = new System.Windows.Forms.ComboBox();
            this.btnsech = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.pnlLabelSearch = new System.Windows.Forms.Panel();
            this.vLabel1 = new VSM.Q_and_A.VLabel();
            this.label20 = new System.Windows.Forms.Label();
            this.btnclear = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblhidden = new System.Windows.Forms.Label();
            this.prntbttn = new System.Windows.Forms.Button();
            this.pnlOrder = new System.Windows.Forms.Panel();
            this.vLabel2 = new VSM.Q_and_A.VLabel();
            this.pbxRightCollapse = new System.Windows.Forms.PictureBox();
            this.pbxCollapse = new System.Windows.Forms.PictureBox();
            this.txtsearch1 = new System.Windows.Forms.TextBox();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.dgvissuedreceivedSearch = new System.Windows.Forms.DataGridView();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cbxSearchOrderNo = new System.Windows.Forms.ComboBox();
            this.pnlSearchHeader = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlCollapse2 = new System.Windows.Forms.Panel();
            this.PanelMain = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIssuedReceivedReport)).BeginInit();
            this.pnlLabelSearch.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlOrder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxRightCollapse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxCollapse)).BeginInit();
            this.pnlSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvissuedreceivedSearch)).BeginInit();
            this.pnlSearchHeader.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlCollapse2.SuspendLayout();
            this.PanelMain.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel15
            // 
            this.panel15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel15.Controls.Add(this.dgvIssuedReceivedReport);
            this.panel15.Controls.Add(this.ddlcustomer);
            this.panel15.Controls.Add(this.btnsech);
            this.panel15.Controls.Add(this.label7);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel15.Location = new System.Drawing.Point(0, 0);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(972, 606);
            this.panel15.TabIndex = 113;
            // 
            // dgvIssuedReceivedReport
            // 
            this.dgvIssuedReceivedReport.AllowUserToAddRows = false;
            this.dgvIssuedReceivedReport.AllowUserToDeleteRows = false;
            this.dgvIssuedReceivedReport.AllowUserToOrderColumns = true;
            this.dgvIssuedReceivedReport.AllowUserToResizeColumns = false;
            this.dgvIssuedReceivedReport.AllowUserToResizeRows = false;
            this.dgvIssuedReceivedReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvIssuedReceivedReport.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvIssuedReceivedReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIssuedReceivedReport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SNo,
            this.CustomerName,
            this.ProductName,
            this.Issued,
            this.Received,
            this.Balance});
            this.dgvIssuedReceivedReport.GridColor = System.Drawing.SystemColors.ActiveBorder;
            this.dgvIssuedReceivedReport.Location = new System.Drawing.Point(10, 13);
            this.dgvIssuedReceivedReport.Name = "dgvIssuedReceivedReport";
            this.dgvIssuedReceivedReport.RowHeadersVisible = false;
            this.dgvIssuedReceivedReport.Size = new System.Drawing.Size(948, 575);
            this.dgvIssuedReceivedReport.TabIndex = 1;
            // 
            // SNo
            // 
            this.SNo.HeaderText = "S.No";
            this.SNo.Name = "SNo";
            // 
            // CustomerName
            // 
            this.CustomerName.HeaderText = "CustomerName";
            this.CustomerName.Name = "CustomerName";
            // 
            // ProductName
            // 
            this.ProductName.HeaderText = "Product Name";
            this.ProductName.Name = "ProductName";
            // 
            // Issued
            // 
            this.Issued.HeaderText = "Issued";
            this.Issued.Name = "Issued";
            // 
            // Received
            // 
            this.Received.HeaderText = "Received";
            this.Received.Name = "Received";
            // 
            // Balance
            // 
            this.Balance.HeaderText = "Balance";
            this.Balance.Name = "Balance";
            // 
            // ddlcustomer
            // 
            this.ddlcustomer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ddlcustomer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ddlcustomer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ddlcustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlcustomer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ddlcustomer.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.ddlcustomer.FormattingEnabled = true;
            this.ddlcustomer.Location = new System.Drawing.Point(223, 93);
            this.ddlcustomer.Name = "ddlcustomer";
            this.ddlcustomer.Size = new System.Drawing.Size(209, 23);
            this.ddlcustomer.TabIndex = 0;
            this.ddlcustomer.Visible = false;
            // 
            // btnsech
            // 
            this.btnsech.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnsech.FlatAppearance.BorderSize = 0;
            this.btnsech.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnsech.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnsech.Location = new System.Drawing.Point(438, 94);
            this.btnsech.Name = "btnsech";
            this.btnsech.Size = new System.Drawing.Size(75, 25);
            this.btnsech.TabIndex = 0;
            this.btnsech.Text = "Search";
            this.btnsech.UseVisualStyleBackColor = true;
            this.btnsech.Visible = false;
            this.btnsech.Click += new System.EventHandler(this.btnsech_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.label7.Location = new System.Drawing.Point(159, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 15);
            this.label7.TabIndex = 22;
            this.label7.Text = "Customer";
            this.label7.Visible = false;
            // 
            // pnlLabelSearch
            // 
            this.pnlLabelSearch.BackColor = System.Drawing.Color.Transparent;
            this.pnlLabelSearch.BackgroundImage = global::Inventory.Properties.Resources._5;
            this.pnlLabelSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlLabelSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLabelSearch.Controls.Add(this.vLabel1);
            this.pnlLabelSearch.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLabelSearch.Location = new System.Drawing.Point(0, 0);
            this.pnlLabelSearch.Name = "pnlLabelSearch";
            this.pnlLabelSearch.Size = new System.Drawing.Size(25, 644);
            this.pnlLabelSearch.TabIndex = 114;
            this.pnlLabelSearch.Visible = false;
            // 
            // vLabel1
            // 
            this.vLabel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.vLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vLabel1.Flip180 = true;
            this.vLabel1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vLabel1.ForeColor = System.Drawing.Color.White;
            this.vLabel1.Location = new System.Drawing.Point(0, 0);
            this.vLabel1.Name = "vLabel1";
            this.vLabel1.Size = new System.Drawing.Size(23, 642);
            this.vLabel1.TabIndex = 0;
            this.vLabel1.Text = "Search";
            this.vLabel1.Click += new System.EventHandler(this.vLabel1_Click);
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.SteelBlue;
            this.label20.Location = new System.Drawing.Point(1, 2);
            this.label20.Name = "label20";
            this.label20.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label20.Size = new System.Drawing.Size(237, 28);
            this.label20.TabIndex = 61;
            this.label20.Text = "Issued/Received Report";
            // 
            // btnclear
            // 
            this.btnclear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnclear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnclear.FlatAppearance.BorderSize = 0;
            this.btnclear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnclear.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnclear.Location = new System.Drawing.Point(894, 2);
            this.btnclear.Name = "btnclear";
            this.btnclear.Size = new System.Drawing.Size(75, 25);
            this.btnclear.TabIndex = 0;
            this.btnclear.Text = "Clear";
            this.btnclear.UseVisualStyleBackColor = true;
            this.btnclear.Click += new System.EventHandler(this.btnclear_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblhidden);
            this.panel1.Controls.Add(this.prntbttn);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.btnclear);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(974, 32);
            this.panel1.TabIndex = 3;
            // 
            // lblhidden
            // 
            this.lblhidden.AutoSize = true;
            this.lblhidden.Location = new System.Drawing.Point(343, 13);
            this.lblhidden.Name = "lblhidden";
            this.lblhidden.Size = new System.Drawing.Size(0, 13);
            this.lblhidden.TabIndex = 62;
            this.lblhidden.Visible = false;
            // 
            // prntbttn
            // 
            this.prntbttn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.prntbttn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.prntbttn.FlatAppearance.BorderSize = 0;
            this.prntbttn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.prntbttn.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prntbttn.Location = new System.Drawing.Point(812, 2);
            this.prntbttn.Name = "prntbttn";
            this.prntbttn.Size = new System.Drawing.Size(75, 25);
            this.prntbttn.TabIndex = 23;
            this.prntbttn.Text = "Print";
            this.prntbttn.UseVisualStyleBackColor = true;
            this.prntbttn.Click += new System.EventHandler(this.prntbttn_Click);
            // 
            // pnlOrder
            // 
            this.pnlOrder.BackgroundImage = global::Inventory.Properties.Resources._5;
            this.pnlOrder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlOrder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlOrder.Controls.Add(this.vLabel2);
            this.pnlOrder.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlOrder.Location = new System.Drawing.Point(1307, 0);
            this.pnlOrder.Name = "pnlOrder";
            this.pnlOrder.Size = new System.Drawing.Size(25, 644);
            this.pnlOrder.TabIndex = 113;
            this.pnlOrder.Visible = false;
            // 
            // vLabel2
            // 
            this.vLabel2.BackColor = System.Drawing.Color.Transparent;
            this.vLabel2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.vLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vLabel2.Flip180 = true;
            this.vLabel2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vLabel2.ForeColor = System.Drawing.Color.White;
            this.vLabel2.Location = new System.Drawing.Point(0, 0);
            this.vLabel2.Name = "vLabel2";
            this.vLabel2.Size = new System.Drawing.Size(23, 642);
            this.vLabel2.TabIndex = 1;
            this.vLabel2.Text = "View Issued/ReceivedReport";
            this.vLabel2.Click += new System.EventHandler(this.vLabel2_Click);
            // 
            // pbxRightCollapse
            // 
            this.pbxRightCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxRightCollapse.BackColor = System.Drawing.Color.Transparent;
            this.pbxRightCollapse.BackgroundImage = global::Inventory.Properties.Resources.right_collapse;
            this.pbxRightCollapse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbxRightCollapse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbxRightCollapse.Location = new System.Drawing.Point(272, 0);
            this.pbxRightCollapse.Name = "pbxRightCollapse";
            this.pbxRightCollapse.Size = new System.Drawing.Size(25, 26);
            this.pbxRightCollapse.TabIndex = 1;
            this.pbxRightCollapse.TabStop = false;
            this.pbxRightCollapse.Click += new System.EventHandler(this.pbxRightCollapse_Click);
            // 
            // pbxCollapse
            // 
            this.pbxCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxCollapse.BackColor = System.Drawing.Color.Transparent;
            this.pbxCollapse.BackgroundImage = global::Inventory.Properties.Resources.left_expand;
            this.pbxCollapse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbxCollapse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbxCollapse.Location = new System.Drawing.Point(247, -1);
            this.pbxCollapse.Name = "pbxCollapse";
            this.pbxCollapse.Size = new System.Drawing.Size(25, 26);
            this.pbxCollapse.TabIndex = 0;
            this.pbxCollapse.TabStop = false;
            this.pbxCollapse.Click += new System.EventHandler(this.pbxCollapse_Click);
            // 
            // txtsearch1
            // 
            this.txtsearch1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtsearch1.Location = new System.Drawing.Point(133, 34);
            this.txtsearch1.Name = "txtsearch1";
            this.txtsearch1.Size = new System.Drawing.Size(162, 20);
            this.txtsearch1.TabIndex = 106;
            this.txtsearch1.Click += new System.EventHandler(this.txtsearch1_Click);
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.pnlSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSearch.Controls.Add(this.txtsearch1);
            this.pnlSearch.Controls.Add(this.dgvissuedreceivedSearch);
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.cbxSearchOrderNo);
            this.pnlSearch.Controls.Add(this.pnlSearchHeader);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(0, 0);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(300, 644);
            this.pnlSearch.TabIndex = 0;
            // 
            // dgvissuedreceivedSearch
            // 
            this.dgvissuedreceivedSearch.AllowUserToAddRows = false;
            this.dgvissuedreceivedSearch.AllowUserToDeleteRows = false;
            this.dgvissuedreceivedSearch.AllowUserToOrderColumns = true;
            this.dgvissuedreceivedSearch.AllowUserToResizeRows = false;
            this.dgvissuedreceivedSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvissuedreceivedSearch.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvissuedreceivedSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvissuedreceivedSearch.Location = new System.Drawing.Point(2, 91);
            this.dgvissuedreceivedSearch.Name = "dgvissuedreceivedSearch";
            this.dgvissuedreceivedSearch.ReadOnly = true;
            this.dgvissuedreceivedSearch.RowHeadersVisible = false;
            this.dgvissuedreceivedSearch.Size = new System.Drawing.Size(294, 549);
            this.dgvissuedreceivedSearch.TabIndex = 104;
            this.dgvissuedreceivedSearch.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataSearch_CellClick);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearch.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Location = new System.Drawing.Point(221, 60);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 25);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cbxSearchOrderNo
            // 
            this.cbxSearchOrderNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.cbxSearchOrderNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSearchOrderNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxSearchOrderNo.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxSearchOrderNo.FormattingEnabled = true;
            this.cbxSearchOrderNo.Location = new System.Drawing.Point(5, 33);
            this.cbxSearchOrderNo.Name = "cbxSearchOrderNo";
            this.cbxSearchOrderNo.Size = new System.Drawing.Size(121, 22);
            this.cbxSearchOrderNo.TabIndex = 1;
            this.cbxSearchOrderNo.SelectedIndexChanged += new System.EventHandler(this.cbxSearchOrderNo_SelectedIndexChanged);
            // 
            // pnlSearchHeader
            // 
            this.pnlSearchHeader.BackgroundImage = global::Inventory.Properties.Resources._5;
            this.pnlSearchHeader.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlSearchHeader.Controls.Add(this.pbxRightCollapse);
            this.pnlSearchHeader.Controls.Add(this.pbxCollapse);
            this.pnlSearchHeader.Controls.Add(this.label1);
            this.pnlSearchHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearchHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlSearchHeader.Name = "pnlSearchHeader";
            this.pnlSearchHeader.Size = new System.Drawing.Size(298, 25);
            this.pnlSearchHeader.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(4, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(25, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnlSearch);
            this.splitContainer1.Panel1MinSize = 300;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.AutoScrollMinSize = new System.Drawing.Size(700, 535);
            this.splitContainer1.Panel2.Controls.Add(this.pnlCollapse2);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Size = new System.Drawing.Size(1282, 644);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 112;
            // 
            // pnlCollapse2
            // 
            this.pnlCollapse2.Controls.Add(this.PanelMain);
            this.pnlCollapse2.Controls.Add(this.panel1);
            this.pnlCollapse2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCollapse2.Location = new System.Drawing.Point(2, 2);
            this.pnlCollapse2.Name = "pnlCollapse2";
            this.pnlCollapse2.Size = new System.Drawing.Size(974, 640);
            this.pnlCollapse2.TabIndex = 0;
            // 
            // PanelMain
            // 
            this.PanelMain.Controls.Add(this.panel2);
            this.PanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelMain.Location = new System.Drawing.Point(0, 32);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(974, 608);
            this.PanelMain.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.panel15);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(974, 608);
            this.panel2.TabIndex = 2;
            // 
            // FrmIssuedReceivedReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1332, 644);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlLabelSearch);
            this.Controls.Add(this.pnlOrder);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1030, 660);
            this.Name = "FrmIssuedReceivedReport";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "FrmIssuedReceivedReport";
            this.Load += new System.EventHandler(this.FrmIssuedReceivedReport_Load);
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIssuedReceivedReport)).EndInit();
            this.pnlLabelSearch.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlOrder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxRightCollapse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxCollapse)).EndInit();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvissuedreceivedSearch)).EndInit();
            this.pnlSearchHeader.ResumeLayout(false);
            this.pnlSearchHeader.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.pnlCollapse2.ResumeLayout(false);
            this.PanelMain.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.ComboBox ddlcustomer;
        private System.Windows.Forms.Label label7;
        private VSM.Q_and_A.VLabel vLabel2;
        private VSM.Q_and_A.VLabel vLabel1;
        private System.Windows.Forms.Panel pnlLabelSearch;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnclear;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlOrder;
        private System.Windows.Forms.PictureBox pbxRightCollapse;
        private System.Windows.Forms.PictureBox pbxCollapse;
        private System.Windows.Forms.TextBox txtsearch1;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.DataGridView dgvissuedreceivedSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cbxSearchOrderNo;
        private System.Windows.Forms.Panel pnlSearchHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlCollapse2;
        private System.Windows.Forms.Panel PanelMain;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnsech;
        private System.Windows.Forms.DataGridView dgvIssuedReceivedReport;
        private System.Windows.Forms.DataGridViewTextBoxColumn SNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Issued;
        private System.Windows.Forms.DataGridViewTextBoxColumn Received;
        private System.Windows.Forms.DataGridViewTextBoxColumn Balance;
        private System.Windows.Forms.Button prntbttn;
        private System.Windows.Forms.Label lblhidden;
    }
}