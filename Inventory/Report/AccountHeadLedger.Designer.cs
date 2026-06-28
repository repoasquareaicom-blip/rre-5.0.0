namespace Inventory.Report
{
    partial class AccountHeadLedger
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnsearch = new System.Windows.Forms.Button();
            this.comboAccountheadValue = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radioRose = new System.Windows.Forms.RadioButton();
            this.radioAcHead = new System.Windows.Forms.RadioButton();
            this.radioCustomers = new System.Windows.Forms.RadioButton();
            this.AccountCRV = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.dgvStockrpt = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockrpt)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.dtpToDate);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dtpFromDate);
            this.panel1.Controls.Add(this.btnsearch);
            this.panel1.Controls.Add(this.comboAccountheadValue);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1020, 60);
            this.panel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(497, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 129;
            this.label3.Text = "To Date";
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "dd-MM-yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(560, 22);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(120, 20);
            this.dtpToDate.TabIndex = 128;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(267, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 127;
            this.label2.Text = "From Date";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd-MM-yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(346, 23);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(120, 20);
            this.dtpFromDate.TabIndex = 126;
            // 
            // btnsearch
            // 
            this.btnsearch.Location = new System.Drawing.Point(710, 21);
            this.btnsearch.Name = "btnsearch";
            this.btnsearch.Size = new System.Drawing.Size(69, 23);
            this.btnsearch.TabIndex = 5;
            this.btnsearch.Text = "Search";
            this.btnsearch.UseVisualStyleBackColor = true;
            this.btnsearch.Click += new System.EventHandler(this.btnsearch_Click);
            // 
            // comboAccountheadValue
            // 
            this.comboAccountheadValue.FormattingEnabled = true;
            this.comboAccountheadValue.Location = new System.Drawing.Point(95, 23);
            this.comboAccountheadValue.Name = "comboAccountheadValue";
            this.comboAccountheadValue.Size = new System.Drawing.Size(144, 21);
            this.comboAccountheadValue.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radioRose);
            this.panel2.Controls.Add(this.radioAcHead);
            this.panel2.Controls.Add(this.radioCustomers);
            this.panel2.Location = new System.Drawing.Point(60, 104);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(285, 35);
            this.panel2.TabIndex = 1;
            this.panel2.Visible = false;
            // 
            // radioRose
            // 
            this.radioRose.AutoSize = true;
            this.radioRose.Location = new System.Drawing.Point(211, 12);
            this.radioRose.Name = "radioRose";
            this.radioRose.Size = new System.Drawing.Size(50, 17);
            this.radioRose.TabIndex = 3;
            this.radioRose.TabStop = true;
            this.radioRose.Text = "Rose";
            this.radioRose.UseVisualStyleBackColor = true;
            this.radioRose.CheckedChanged += new System.EventHandler(this.radioRose_CheckedChanged);
            // 
            // radioAcHead
            // 
            this.radioAcHead.AutoSize = true;
            this.radioAcHead.Location = new System.Drawing.Point(97, 12);
            this.radioAcHead.Name = "radioAcHead";
            this.radioAcHead.Size = new System.Drawing.Size(94, 17);
            this.radioAcHead.TabIndex = 2;
            this.radioAcHead.TabStop = true;
            this.radioAcHead.Text = "Account Head";
            this.radioAcHead.UseVisualStyleBackColor = true;
            this.radioAcHead.CheckedChanged += new System.EventHandler(this.radioAcHead_CheckedChanged);
            // 
            // radioCustomers
            // 
            this.radioCustomers.AutoSize = true;
            this.radioCustomers.Location = new System.Drawing.Point(8, 12);
            this.radioCustomers.Name = "radioCustomers";
            this.radioCustomers.Size = new System.Drawing.Size(69, 17);
            this.radioCustomers.TabIndex = 0;
            this.radioCustomers.TabStop = true;
            this.radioCustomers.Text = "Customer";
            this.radioCustomers.UseVisualStyleBackColor = true;
            this.radioCustomers.CheckedChanged += new System.EventHandler(this.radioCustomers_CheckedChanged);
            // 
            // AccountCRV
            // 
            this.AccountCRV.ActiveViewIndex = -1;
            this.AccountCRV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AccountCRV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //this.AccountCRV.CachedPageNumberPerDoc = 10;
            this.AccountCRV.Cursor = System.Windows.Forms.Cursors.Default;
            this.AccountCRV.Location = new System.Drawing.Point(0, 51);
            this.AccountCRV.Name = "AccountCRV";
            this.AccountCRV.Size = new System.Drawing.Size(877, 481);
            this.AccountCRV.TabIndex = 1;
            this.AccountCRV.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // dgvStockrpt
            // 
            this.dgvStockrpt.AllowUserToAddRows = false;
            this.dgvStockrpt.AllowUserToResizeColumns = false;
            this.dgvStockrpt.AllowUserToResizeRows = false;
            this.dgvStockrpt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStockrpt.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStockrpt.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStockrpt.Location = new System.Drawing.Point(14, 66);
            this.dgvStockrpt.Name = "dgvStockrpt";
            this.dgvStockrpt.ReadOnly = true;
            this.dgvStockrpt.RowHeadersVisible = false;
            this.dgvStockrpt.Size = new System.Drawing.Size(984, 586);
            this.dgvStockrpt.TabIndex = 130;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 130;
            this.label1.Text = "Main Head:";
            // 
            // AccountHeadLedger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 664);
            this.Controls.Add(this.dgvStockrpt);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.MinimizeBox = false;
            this.Name = "AccountHeadLedger";
            this.Text = "AccountHeadLedger";
            this.Load += new System.EventHandler(this.AccountHeadLedger_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockrpt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboAccountheadValue;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radioRose;
        private System.Windows.Forms.RadioButton radioAcHead;
        private System.Windows.Forms.RadioButton radioCustomers;
        private System.Windows.Forms.Button btnsearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer AccountCRV;
        private System.Windows.Forms.DataGridView dgvStockrpt;
        private System.Windows.Forms.Label label1;
    }
}