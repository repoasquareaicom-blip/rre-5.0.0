namespace Inventory.Report
{
    partial class HsnSummaryReport
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label20 = new System.Windows.Forms.Label();
            this.Tomtdate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.Frommtdate = new System.Windows.Forms.DateTimePicker();
            this.btnsearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvStockrpt = new System.Windows.Forms.DataGridView();
            this.chkB2B = new System.Windows.Forms.CheckBox();
            this.chkB2C = new System.Windows.Forms.CheckBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label26 = new System.Windows.Forms.Label();
            this.cmbcompanychange = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockrpt)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label20);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1358, 34);
            this.panel1.TabIndex = 23;
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.SteelBlue;
            this.label20.Location = new System.Drawing.Point(5, 5);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(206, 28);
            this.label20.TabIndex = 21;
            this.label20.Text = "HSN Summary Report";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Tomtdate
            // 
            this.Tomtdate.CustomFormat = "dd-MM-yyyy";
            this.Tomtdate.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tomtdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Tomtdate.Location = new System.Drawing.Point(233, 41);
            this.Tomtdate.Name = "Tomtdate";
            this.Tomtdate.Size = new System.Drawing.Size(109, 23);
            this.Tomtdate.TabIndex = 468;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 20);
            this.label2.TabIndex = 465;
            this.label2.Text = "From";
            // 
            // Frommtdate
            // 
            this.Frommtdate.CustomFormat = "dd-MM-yyyy";
            this.Frommtdate.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frommtdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Frommtdate.Location = new System.Drawing.Point(57, 41);
            this.Frommtdate.Name = "Frommtdate";
            this.Frommtdate.Size = new System.Drawing.Size(122, 23);
            this.Frommtdate.TabIndex = 466;
            // 
            // btnsearch
            // 
            this.btnsearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnsearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnsearch.Location = new System.Drawing.Point(775, 40);
            this.btnsearch.Name = "btnsearch";
            this.btnsearch.Size = new System.Drawing.Size(97, 27);
            this.btnsearch.TabIndex = 469;
            this.btnsearch.Text = "Search";
            this.btnsearch.UseVisualStyleBackColor = true;
            this.btnsearch.Click += new System.EventHandler(this.btnsearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(200, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 20);
            this.label3.TabIndex = 467;
            this.label3.Text = "To";
            // 
            // dgvStockrpt
            // 
            this.dgvStockrpt.AllowUserToAddRows = false;
            this.dgvStockrpt.AllowUserToResizeRows = false;
            this.dgvStockrpt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStockrpt.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStockrpt.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStockrpt.Location = new System.Drawing.Point(8, 85);
            this.dgvStockrpt.Name = "dgvStockrpt";
            this.dgvStockrpt.ReadOnly = true;
            this.dgvStockrpt.RowHeadersVisible = false;
            this.dgvStockrpt.Size = new System.Drawing.Size(1342, 565);
            this.dgvStockrpt.TabIndex = 472;
            // 
            // chkB2B
            // 
            this.chkB2B.AutoSize = true;
            this.chkB2B.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkB2B.Location = new System.Drawing.Point(649, 42);
            this.chkB2B.Name = "chkB2B";
            this.chkB2B.Size = new System.Drawing.Size(54, 22);
            this.chkB2B.TabIndex = 473;
            this.chkB2B.Text = "B2B";
            this.chkB2B.UseVisualStyleBackColor = true;
            // 
            // chkB2C
            // 
            this.chkB2C.AutoSize = true;
            this.chkB2C.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkB2C.Location = new System.Drawing.Point(709, 42);
            this.chkB2C.Name = "chkB2C";
            this.chkB2C.Size = new System.Drawing.Size(55, 22);
            this.chkB2C.TabIndex = 474;
            this.chkB2C.Text = "B2C";
            this.chkB2C.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.Location = new System.Drawing.Point(887, 40);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(122, 27);
            this.btnExport.TabIndex = 475;
            this.btnExport.Text = "Export to Excel";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ForeColor = System.Drawing.Color.Teal;
            this.label26.Location = new System.Drawing.Point(360, 43);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(117, 19);
            this.label26.TabIndex = 476;
            this.label26.Text = "Company Name";
            // 
            // cmbcompanychange
            // 
            this.cmbcompanychange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbcompanychange.FormattingEnabled = true;
            this.cmbcompanychange.Location = new System.Drawing.Point(490, 43);
            this.cmbcompanychange.Name = "cmbcompanychange";
            this.cmbcompanychange.Size = new System.Drawing.Size(145, 21);
            this.cmbcompanychange.TabIndex = 477;
            // 
            // HsnSummaryReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 662);
            this.Controls.Add(this.cmbcompanychange);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.chkB2C);
            this.Controls.Add(this.chkB2B);
            this.Controls.Add(this.dgvStockrpt);
            this.Controls.Add(this.Tomtdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Frommtdate);
            this.Controls.Add(this.btnsearch);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.Name = "HsnSummaryReport";
            this.Text = "HSN Summary Report";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockrpt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.DateTimePicker Tomtdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker Frommtdate;
        private System.Windows.Forms.Button btnsearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvStockrpt;
        private System.Windows.Forms.CheckBox chkB2B;
        private System.Windows.Forms.CheckBox chkB2C;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox cmbcompanychange;
    }
}
