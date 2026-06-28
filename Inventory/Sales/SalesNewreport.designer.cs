namespace Inventory.Sales
{
    partial class SalesNewreport
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
            this.vLabel2 = new VSM.Q_and_A.VLabel();
            this.vLabel1 = new VSM.Q_and_A.VLabel();
            this.Tomtdate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.Frommtdate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.btnsearch = new System.Windows.Forms.Button();
            this.dgvStockrpt = new System.Windows.Forms.DataGridView();
            this.lbltotalamount = new System.Windows.Forms.Label();
            this.LbelPipesTotal = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockrpt)).BeginInit();
            this.SuspendLayout();
            // 
            // vLabel2
            // 
            this.vLabel2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.vLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vLabel2.Flip180 = true;
            this.vLabel2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vLabel2.ForeColor = System.Drawing.Color.White;
            this.vLabel2.Location = new System.Drawing.Point(0, 0);
            this.vLabel2.Name = "vLabel2";
            this.vLabel2.Size = new System.Drawing.Size(1020, 664);
            this.vLabel2.TabIndex = 2;
            this.vLabel2.Text = "Search";
            // 
            // vLabel1
            // 
            this.vLabel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.vLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vLabel1.Flip180 = true;
            this.vLabel1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vLabel1.ForeColor = System.Drawing.Color.White;
            this.vLabel1.Location = new System.Drawing.Point(0, 0);
            this.vLabel1.Name = "vLabel1";
            this.vLabel1.Size = new System.Drawing.Size(1020, 664);
            this.vLabel1.TabIndex = 1;
            this.vLabel1.Text = "Search";
            // 
            // Tomtdate
            // 
            this.Tomtdate.CustomFormat = "dd-MM-yyyy";
            this.Tomtdate.Font = new System.Drawing.Font("Calibri", 8F);
            this.Tomtdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Tomtdate.Location = new System.Drawing.Point(226, 14);
            this.Tomtdate.Name = "Tomtdate";
            this.Tomtdate.Size = new System.Drawing.Size(109, 21);
            this.Tomtdate.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(194, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "To";
            // 
            // Frommtdate
            // 
            this.Frommtdate.CustomFormat = "dd-MM-yyyy";
            this.Frommtdate.Font = new System.Drawing.Font("Calibri", 8F);
            this.Frommtdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Frommtdate.Location = new System.Drawing.Point(67, 16);
            this.Frommtdate.Name = "Frommtdate";
            this.Frommtdate.Size = new System.Drawing.Size(109, 21);
            this.Frommtdate.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(22, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "From";
            // 
            // btnsearch
            // 
            this.btnsearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnsearch.Location = new System.Drawing.Point(368, 12);
            this.btnsearch.Name = "btnsearch";
            this.btnsearch.Size = new System.Drawing.Size(81, 23);
            this.btnsearch.TabIndex = 13;
            this.btnsearch.Text = "Search";
            this.btnsearch.UseVisualStyleBackColor = true;
            this.btnsearch.Click += new System.EventHandler(this.btnsearch_Click);
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
            this.dgvStockrpt.Location = new System.Drawing.Point(12, 51);
            this.dgvStockrpt.Name = "dgvStockrpt";
            this.dgvStockrpt.ReadOnly = true;
            this.dgvStockrpt.RowHeadersVisible = false;
            this.dgvStockrpt.Size = new System.Drawing.Size(984, 578);
            this.dgvStockrpt.TabIndex = 14;
            // 
            // lbltotalamount
            // 
            this.lbltotalamount.AutoSize = true;
            this.lbltotalamount.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbltotalamount.ForeColor = System.Drawing.Color.Green;
            this.lbltotalamount.Location = new System.Drawing.Point(444, 634);
            this.lbltotalamount.Name = "lbltotalamount";
            this.lbltotalamount.Size = new System.Drawing.Size(66, 24);
            this.lbltotalamount.TabIndex = 15;
            this.lbltotalamount.Text = "label1";
            // 
            // LbelPipesTotal
            // 
            this.LbelPipesTotal.AutoSize = true;
            this.LbelPipesTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbelPipesTotal.ForeColor = System.Drawing.Color.Green;
            this.LbelPipesTotal.Location = new System.Drawing.Point(881, 636);
            this.LbelPipesTotal.Name = "LbelPipesTotal";
            this.LbelPipesTotal.Size = new System.Drawing.Size(66, 24);
            this.LbelPipesTotal.TabIndex = 16;
            this.LbelPipesTotal.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(277, 633);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 25);
            this.label1.TabIndex = 17;
            this.label1.Text = "Total Amount:";
            // 
            // SalesNewreport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 664);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LbelPipesTotal);
            this.Controls.Add(this.lbltotalamount);
            this.Controls.Add(this.dgvStockrpt);
            this.Controls.Add(this.btnsearch);
            this.Controls.Add(this.Tomtdate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Frommtdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.vLabel2);
            this.Controls.Add(this.vLabel1);
            this.Name = "SalesNewreport";
            this.Text = "SalesBillNew";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SalesBillNew_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockrpt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VSM.Q_and_A.VLabel vLabel1;
        private VSM.Q_and_A.VLabel vLabel2;
        private System.Windows.Forms.DateTimePicker Tomtdate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker Frommtdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnsearch;
        private System.Windows.Forms.DataGridView dgvStockrpt;
        private System.Windows.Forms.Label lbltotalamount;
        private System.Windows.Forms.Label LbelPipesTotal;
        private System.Windows.Forms.Label label1;
    }
}