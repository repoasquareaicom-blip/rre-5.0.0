namespace Inventory.Transactions
{
    partial class Cashreceiptlist
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
            this.pnlCollapse2 = new System.Windows.Forms.Panel();
            this.PanelMain = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgvSearch = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.panel15 = new System.Windows.Forms.Panel();
            this.btnsech = new System.Windows.Forms.Button();
            this.DTPTodate = new System.Windows.Forms.DateTimePicker();
            this.dtfromdate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label20 = new System.Windows.Forms.Label();
            this.Transid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Requestid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RequestedPerson = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Print = new System.Windows.Forms.DataGridViewButtonColumn();
            this.pnlCollapse2.SuspendLayout();
            this.PanelMain.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).BeginInit();
            this.panel15.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCollapse2
            // 
            this.pnlCollapse2.Controls.Add(this.PanelMain);
            this.pnlCollapse2.Controls.Add(this.panel1);
            this.pnlCollapse2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCollapse2.Location = new System.Drawing.Point(0, 0);
            this.pnlCollapse2.Name = "pnlCollapse2";
            this.pnlCollapse2.Size = new System.Drawing.Size(1030, 605);
            this.pnlCollapse2.TabIndex = 2;
            // 
            // PanelMain
            // 
            this.PanelMain.Controls.Add(this.panel2);
            this.PanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelMain.Location = new System.Drawing.Point(0, 32);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(1030, 573);
            this.PanelMain.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.dgvSearch);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.panel15);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1030, 573);
            this.panel2.TabIndex = 2;
            // 
            // dgvSearch
            // 
            this.dgvSearch.AllowUserToAddRows = false;
            this.dgvSearch.AllowUserToDeleteRows = false;
            this.dgvSearch.AllowUserToResizeColumns = false;
            this.dgvSearch.AllowUserToResizeRows = false;
            this.dgvSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSearch.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearch.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Transid,
            this.Requestid,
            this.RequestedPerson,
            this.Date,
            this.Amount,
            this.Reason,
            this.Print});
            this.dgvSearch.GridColor = System.Drawing.SystemColors.ActiveBorder;
            this.dgvSearch.Location = new System.Drawing.Point(3, 39);
            this.dgvSearch.Name = "dgvSearch";
            this.dgvSearch.ReadOnly = true;
            this.dgvSearch.RowHeadersVisible = false;
            this.dgvSearch.Size = new System.Drawing.Size(1020, 522);
            this.dgvSearch.TabIndex = 30;
            this.dgvSearch.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearch_CellClick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.label7.Location = new System.Drawing.Point(8, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 15);
            this.label7.TabIndex = 22;
            this.label7.Text = "From Date";
            // 
            // panel15
            // 
            this.panel15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel15.Controls.Add(this.btnsech);
            this.panel15.Controls.Add(this.DTPTodate);
            this.panel15.Controls.Add(this.dtfromdate);
            this.panel15.Controls.Add(this.label2);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel15.Location = new System.Drawing.Point(0, 0);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(1028, 33);
            this.panel15.TabIndex = 113;
            // 
            // btnsech
            // 
            this.btnsech.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnsech.FlatAppearance.BorderSize = 0;
            this.btnsech.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnsech.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnsech.Location = new System.Drawing.Point(401, 2);
            this.btnsech.Name = "btnsech";
            this.btnsech.Size = new System.Drawing.Size(75, 25);
            this.btnsech.TabIndex = 3;
            this.btnsech.Text = "Search";
            this.btnsech.UseVisualStyleBackColor = true;
            this.btnsech.Click += new System.EventHandler(this.btnsech_Click);
            // 
            // DTPTodate
            // 
            this.DTPTodate.CustomFormat = "dd-MM-yyyy";
            this.DTPTodate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTPTodate.Location = new System.Drawing.Point(266, 5);
            this.DTPTodate.Name = "DTPTodate";
            this.DTPTodate.Size = new System.Drawing.Size(123, 20);
            this.DTPTodate.TabIndex = 124;
            // 
            // dtfromdate
            // 
            this.dtfromdate.CustomFormat = "dd-MM-yyyy";
            this.dtfromdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtfromdate.Location = new System.Drawing.Point(71, 4);
            this.dtfromdate.Name = "dtfromdate";
            this.dtfromdate.Size = new System.Drawing.Size(123, 20);
            this.dtfromdate.TabIndex = 123;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.label2.Location = new System.Drawing.Point(213, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 15);
            this.label2.TabIndex = 20;
            this.label2.Text = "To Date";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label20);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1030, 32);
            this.panel1.TabIndex = 3;
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.SteelBlue;
            this.label20.Location = new System.Drawing.Point(1, 2);
            this.label20.Name = "label20";
            this.label20.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label20.Size = new System.Drawing.Size(165, 28);
            this.label20.TabIndex = 61;
            this.label20.Text = "Cash Receipt List";
            // 
            // Transid
            // 
            this.Transid.HeaderText = "Transid";
            this.Transid.Name = "Transid";
            this.Transid.ReadOnly = true;
            this.Transid.Visible = false;
            // 
            // Requestid
            // 
            this.Requestid.FillWeight = 13.25724F;
            this.Requestid.HeaderText = "Receiptid";
            this.Requestid.Name = "Requestid";
            this.Requestid.ReadOnly = true;
            // 
            // RequestedPerson
            // 
            this.RequestedPerson.FillWeight = 13.25724F;
            this.RequestedPerson.HeaderText = "CustomerName";
            this.RequestedPerson.Name = "RequestedPerson";
            this.RequestedPerson.ReadOnly = true;
            // 
            // Date
            // 
            this.Date.FillWeight = 13.25724F;
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // Amount
            // 
            this.Amount.FillWeight = 13.25724F;
            this.Amount.HeaderText = "Amount";
            this.Amount.Name = "Amount";
            this.Amount.ReadOnly = true;
            // 
            // Reason
            // 
            this.Reason.FillWeight = 456.0488F;
            this.Reason.HeaderText = "Mode";
            this.Reason.Name = "Reason";
            this.Reason.ReadOnly = true;
            // 
            // Print
            // 
            this.Print.FillWeight = 13.25724F;
            this.Print.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Print.HeaderText = "Print";
            this.Print.Name = "Print";
            this.Print.ReadOnly = true;
            this.Print.Text = "Print";
            this.Print.ToolTipText = "Print";
            this.Print.UseColumnTextForButtonValue = true;
            // 
            // Cashreceiptlist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1030, 605);
            this.Controls.Add(this.pnlCollapse2);
            this.Name = "Cashreceiptlist";
            this.Text = "Cashreceiptlist";
            this.pnlCollapse2.ResumeLayout(false);
            this.PanelMain.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).EndInit();
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCollapse2;
        private System.Windows.Forms.Panel PanelMain;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgvSearch;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.Button btnsech;
        private System.Windows.Forms.DateTimePicker DTPTodate;
        private System.Windows.Forms.DateTimePicker dtfromdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.DataGridViewTextBoxColumn Transid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Requestid;
        private System.Windows.Forms.DataGridViewTextBoxColumn RequestedPerson;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reason;
        private System.Windows.Forms.DataGridViewButtonColumn Print;
    }
}