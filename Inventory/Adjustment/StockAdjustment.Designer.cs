namespace Inventory.Adjustment
{
    partial class StockAdjustment
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StockAdjustment));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label20 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbloaction = new System.Windows.Forms.ComboBox();
            this.dgvinvoice = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnReceive = new System.Windows.Forms.Button();
            this.Btnclear = new System.Windows.Forms.Button();
            this.Pnloading = new System.Windows.Forms.Panel();
            this.label18 = new System.Windows.Forms.Label();
            this.pnlprodsearch = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.productsearchbttn = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.txtprodsearch = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.lblout = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvinvoice)).BeginInit();
            this.Pnloading.SuspendLayout();
            this.pnlprodsearch.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label20);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(977, 32);
            this.panel1.TabIndex = 1;
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.SteelBlue;
            this.label20.Location = new System.Drawing.Point(3, 3);
            this.label20.Name = "label20";
            this.label20.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label20.Size = new System.Drawing.Size(166, 28);
            this.label20.TabIndex = 0;
            this.label20.Text = "Stock Adjustment";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Floor";
            // 
            // cmbloaction
            // 
            this.cmbloaction.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbloaction.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbloaction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbloaction.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbloaction.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.cmbloaction.FormattingEnabled = true;
            this.cmbloaction.Location = new System.Drawing.Point(68, 47);
            this.cmbloaction.Name = "cmbloaction";
            this.cmbloaction.Size = new System.Drawing.Size(233, 23);
            this.cmbloaction.TabIndex = 0;
            // 
            // dgvinvoice
            // 
            this.dgvinvoice.AllowUserToOrderColumns = true;
            this.dgvinvoice.AllowUserToResizeRows = false;
            this.dgvinvoice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvinvoice.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvinvoice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvinvoice.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvinvoice.Location = new System.Drawing.Point(12, 92);
            this.dgvinvoice.Name = "dgvinvoice";
            this.dgvinvoice.ReadOnly = true;
            this.dgvinvoice.RowHeadersVisible = false;
            this.dgvinvoice.Size = new System.Drawing.Size(944, 414);
            this.dgvinvoice.TabIndex = 2;
            this.dgvinvoice.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvinvoice_CellValueChanged);
            this.dgvinvoice.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvinvoice_EditingControlShowing);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.btnSave.Location = new System.Drawing.Point(854, 525);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 25);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Update";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnReceive
            // 
            this.btnReceive.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReceive.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnReceive.Location = new System.Drawing.Point(335, 46);
            this.btnReceive.Name = "btnReceive";
            this.btnReceive.Size = new System.Drawing.Size(126, 23);
            this.btnReceive.TabIndex = 1;
            this.btnReceive.Text = "Proceed";
            this.btnReceive.UseVisualStyleBackColor = true;
            this.btnReceive.Click += new System.EventHandler(this.btnReceive_Click);
            // 
            // Btnclear
            // 
            this.Btnclear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btnclear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btnclear.Location = new System.Drawing.Point(467, 46);
            this.Btnclear.Name = "Btnclear";
            this.Btnclear.Size = new System.Drawing.Size(90, 23);
            this.Btnclear.TabIndex = 8;
            this.Btnclear.Text = "Clear";
            this.Btnclear.UseVisualStyleBackColor = true;
            this.Btnclear.Click += new System.EventHandler(this.Btnclear_Click);
            // 
            // Pnloading
            // 
            this.Pnloading.BackColor = System.Drawing.Color.Gainsboro;
            this.Pnloading.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Pnloading.Controls.Add(this.label18);
            this.Pnloading.Location = new System.Drawing.Point(348, 250);
            this.Pnloading.Name = "Pnloading";
            this.Pnloading.Size = new System.Drawing.Size(281, 62);
            this.Pnloading.TabIndex = 394;
            this.Pnloading.Visible = false;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(45, 22);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(160, 16);
            this.label18.TabIndex = 51;
            this.label18.Text = "Saving   Please Wait ....";
            // 
            // pnlprodsearch
            // 
            this.pnlprodsearch.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlprodsearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlprodsearch.Controls.Add(this.panel5);
            this.pnlprodsearch.Controls.Add(this.txtprodsearch);
            this.pnlprodsearch.Location = new System.Drawing.Point(242, 129);
            this.pnlprodsearch.Name = "pnlprodsearch";
            this.pnlprodsearch.Size = new System.Drawing.Size(277, 74);
            this.pnlprodsearch.TabIndex = 395;
            this.pnlprodsearch.Visible = false;
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::Inventory.Properties.Resources.labelBack;
            this.panel5.Controls.Add(this.productsearchbttn);
            this.panel5.Controls.Add(this.label24);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(275, 22);
            this.panel5.TabIndex = 0;
            // 
            // productsearchbttn
            // 
            this.productsearchbttn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.productsearchbttn.BackColor = System.Drawing.Color.Transparent;
            this.productsearchbttn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("productsearchbttn.BackgroundImage")));
            this.productsearchbttn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.productsearchbttn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.productsearchbttn.FlatAppearance.BorderSize = 0;
            this.productsearchbttn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.productsearchbttn.Location = new System.Drawing.Point(249, -1);
            this.productsearchbttn.Name = "productsearchbttn";
            this.productsearchbttn.Size = new System.Drawing.Size(26, 24);
            this.productsearchbttn.TabIndex = 94;
            this.productsearchbttn.UseVisualStyleBackColor = false;
            this.productsearchbttn.Click += new System.EventHandler(this.productsearchbttn_Click);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.BackColor = System.Drawing.Color.Transparent;
            this.label24.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold);
            this.label24.ForeColor = System.Drawing.Color.White;
            this.label24.Location = new System.Drawing.Point(8, 3);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(89, 15);
            this.label24.TabIndex = 93;
            this.label24.Text = "Search Product";
            // 
            // txtprodsearch
            // 
            this.txtprodsearch.Location = new System.Drawing.Point(11, 34);
            this.txtprodsearch.MaxLength = 50;
            this.txtprodsearch.Name = "txtprodsearch";
            this.txtprodsearch.Size = new System.Drawing.Size(257, 20);
            this.txtprodsearch.TabIndex = 4;
            // 
            // label25
            // 
            this.label25.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.Red;
            this.label25.Location = new System.Drawing.Point(20, 525);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(133, 25);
            this.label25.TabIndex = 396;
            this.label25.Text = "F3-> Search";
            // 
            // lblout
            // 
            this.lblout.AutoSize = true;
            this.lblout.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblout.Location = new System.Drawing.Point(742, 42);
            this.lblout.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblout.Name = "lblout";
            this.lblout.Size = new System.Drawing.Size(28, 31);
            this.lblout.TabIndex = 400;
            this.lblout.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label5.Location = new System.Drawing.Point(592, 42);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(152, 31);
            this.label5.TabIndex = 399;
            this.label5.Text = "Total  Stock:";
            // 
            // StockAdjustment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 562);
            this.Controls.Add(this.lblout);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.pnlprodsearch);
            this.Controls.Add(this.Pnloading);
            this.Controls.Add(this.Btnclear);
            this.Controls.Add(this.btnReceive);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvinvoice);
            this.Controls.Add(this.cmbloaction);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "StockAdjustment";
            this.Text = "StockAdjustment";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvinvoice)).EndInit();
            this.Pnloading.ResumeLayout(false);
            this.Pnloading.PerformLayout();
            this.pnlprodsearch.ResumeLayout(false);
            this.pnlprodsearch.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbloaction;
        private System.Windows.Forms.DataGridView dgvinvoice;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnReceive;
        private System.Windows.Forms.Button Btnclear;
        private System.Windows.Forms.Panel Pnloading;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Panel pnlprodsearch;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button productsearchbttn;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtprodsearch;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label lblout;
        private System.Windows.Forms.Label label5;
    }
}