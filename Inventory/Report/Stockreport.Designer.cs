namespace Inventory.Report
{
    partial class Stockreport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Stockreport));
            this.dgvStockrpt = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.electiclas = new System.Windows.Forms.CheckBox();
            this.lights = new System.Windows.Forms.CheckBox();
            this.label25 = new System.Windows.Forms.Label();
            this.btnsearch = new System.Windows.Forms.Button();
            this.txtproductname = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelIn = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.InClose = new System.Windows.Forms.Button();
            this.dgvInBills = new System.Windows.Forms.DataGridView();
            this.pnlprodsearch = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.productsearchbttn = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.txtprodsearch = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockrpt)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panelIn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInBills)).BeginInit();
            this.pnlprodsearch.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvStockrpt
            // 
            this.dgvStockrpt.AllowUserToAddRows = false;
            this.dgvStockrpt.AllowUserToResizeColumns = false;
            this.dgvStockrpt.AllowUserToResizeRows = false;
            this.dgvStockrpt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStockrpt.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStockrpt.Location = new System.Drawing.Point(12, 65);
            this.dgvStockrpt.Name = "dgvStockrpt";
            this.dgvStockrpt.ReadOnly = true;
            this.dgvStockrpt.RowHeadersVisible = false;
            this.dgvStockrpt.Size = new System.Drawing.Size(877, 568);
            this.dgvStockrpt.TabIndex = 8;
            this.dgvStockrpt.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStockrpt_CellClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.electiclas);
            this.groupBox1.Controls.Add(this.lights);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.btnsearch);
            this.groupBox1.Controls.Add(this.txtproductname);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(877, 48);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stock Report";
            // 
            // electiclas
            // 
            this.electiclas.AutoSize = true;
            this.electiclas.Location = new System.Drawing.Point(658, 20);
            this.electiclas.Name = "electiclas";
            this.electiclas.Size = new System.Drawing.Size(84, 20);
            this.electiclas.TabIndex = 492;
            this.electiclas.Text = "Electricals";
            this.electiclas.UseVisualStyleBackColor = true;
            // 
            // lights
            // 
            this.lights.AutoSize = true;
            this.lights.Location = new System.Drawing.Point(586, 20);
            this.lights.Name = "lights";
            this.lights.Size = new System.Drawing.Size(61, 20);
            this.lights.TabIndex = 491;
            this.lights.Text = "Lights";
            this.lights.UseVisualStyleBackColor = true;
            // 
            // label25
            // 
            this.label25.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label25.Location = new System.Drawing.Point(752, 18);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(108, 20);
            this.label25.TabIndex = 490;
            this.label25.Text = "F3-> Search";
            this.label25.Visible = false;
            // 
            // btnsearch
            // 
            this.btnsearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnsearch.Location = new System.Drawing.Point(508, 18);
            this.btnsearch.Name = "btnsearch";
            this.btnsearch.Size = new System.Drawing.Size(69, 23);
            this.btnsearch.TabIndex = 4;
            this.btnsearch.Text = "Search";
            this.btnsearch.UseVisualStyleBackColor = true;
            this.btnsearch.Click += new System.EventHandler(this.btnsearch_Click);
            // 
            // txtproductname
            // 
            this.txtproductname.Location = new System.Drawing.Point(99, 19);
            this.txtproductname.Name = "txtproductname";
            this.txtproductname.Size = new System.Drawing.Size(394, 23);
            this.txtproductname.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Product Name";
            // 
            // panelIn
            // 
            this.panelIn.BackColor = System.Drawing.Color.Transparent;
            this.panelIn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelIn.BackgroundImage")));
            this.panelIn.Controls.Add(this.label6);
            this.panelIn.Controls.Add(this.InClose);
            this.panelIn.Controls.Add(this.dgvInBills);
            this.panelIn.Location = new System.Drawing.Point(280, 241);
            this.panelIn.Name = "panelIn";
            this.panelIn.Size = new System.Drawing.Size(341, 163);
            this.panelIn.TabIndex = 440;
            this.panelIn.Visible = false;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(3, 1);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label6.Size = new System.Drawing.Size(122, 23);
            this.label6.TabIndex = 439;
            this.label6.Text = "Floor Details";
            this.label6.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // InClose
            // 
            this.InClose.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InClose.BackColor = System.Drawing.Color.Transparent;
            this.InClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("InClose.BackgroundImage")));
            this.InClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.InClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.InClose.FlatAppearance.BorderSize = 0;
            this.InClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.InClose.Location = new System.Drawing.Point(311, 2);
            this.InClose.Name = "InClose";
            this.InClose.Size = new System.Drawing.Size(27, 25);
            this.InClose.TabIndex = 366;
            this.InClose.UseVisualStyleBackColor = false;
            this.InClose.Click += new System.EventHandler(this.InClose_Click);
            // 
            // dgvInBills
            // 
            this.dgvInBills.AllowUserToAddRows = false;
            this.dgvInBills.AllowUserToDeleteRows = false;
            this.dgvInBills.AllowUserToResizeColumns = false;
            this.dgvInBills.AllowUserToResizeRows = false;
            this.dgvInBills.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvInBills.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInBills.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInBills.Location = new System.Drawing.Point(0, 27);
            this.dgvInBills.Name = "dgvInBills";
            this.dgvInBills.ReadOnly = true;
            this.dgvInBills.RowHeadersVisible = false;
            this.dgvInBills.Size = new System.Drawing.Size(341, 136);
            this.dgvInBills.TabIndex = 438;
            // 
            // pnlprodsearch
            // 
            this.pnlprodsearch.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlprodsearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlprodsearch.Controls.Add(this.panel3);
            this.pnlprodsearch.Controls.Add(this.txtprodsearch);
            this.pnlprodsearch.Location = new System.Drawing.Point(369, 93);
            this.pnlprodsearch.Name = "pnlprodsearch";
            this.pnlprodsearch.Size = new System.Drawing.Size(277, 74);
            this.pnlprodsearch.TabIndex = 491;
            this.pnlprodsearch.Visible = false;
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = global::Inventory.Properties.Resources.labelBack;
            this.panel3.Controls.Add(this.productsearchbttn);
            this.panel3.Controls.Add(this.label24);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(275, 22);
            this.panel3.TabIndex = 0;
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
            this.label24.Size = new System.Drawing.Size(79, 15);
            this.label24.TabIndex = 93;
            this.label24.Text = "Search Name";
            // 
            // txtprodsearch
            // 
            this.txtprodsearch.Location = new System.Drawing.Point(11, 34);
            this.txtprodsearch.MaxLength = 50;
            this.txtprodsearch.Name = "txtprodsearch";
            this.txtprodsearch.Size = new System.Drawing.Size(257, 20);
            this.txtprodsearch.TabIndex = 4;
            // 
            // Stockreport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 644);
            this.Controls.Add(this.pnlprodsearch);
            this.Controls.Add(this.panelIn);
            this.Controls.Add(this.dgvStockrpt);
            this.Controls.Add(this.groupBox1);
            this.Name = "Stockreport";
            this.Text = "Stockreport";
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockrpt)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelIn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInBills)).EndInit();
            this.pnlprodsearch.ResumeLayout(false);
            this.pnlprodsearch.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvStockrpt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnsearch;
        private System.Windows.Forms.TextBox txtproductname;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelIn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button InClose;
        private System.Windows.Forms.DataGridView dgvInBills;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Panel pnlprodsearch;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button productsearchbttn;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtprodsearch;
        private System.Windows.Forms.CheckBox electiclas;
        private System.Windows.Forms.CheckBox lights;
    }
}