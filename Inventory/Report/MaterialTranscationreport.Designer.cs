namespace Inventory.Report
{
    partial class MaterialTranscationreport
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
            this.dgvStockrpt = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Tomtdate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.Frommtdate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.btnsearch = new System.Windows.Forms.Button();
            this.txtproductname = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblstock = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblout = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblin = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.electiclas = new System.Windows.Forms.CheckBox();
            this.lights = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockrpt)).BeginInit();
            this.groupBox1.SuspendLayout();
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
            this.dgvStockrpt.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStockrpt.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStockrpt.Location = new System.Drawing.Point(27, 116);
            this.dgvStockrpt.Name = "dgvStockrpt";
            this.dgvStockrpt.ReadOnly = true;
            this.dgvStockrpt.RowHeadersVisible = false;
            this.dgvStockrpt.Size = new System.Drawing.Size(984, 507);
            this.dgvStockrpt.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.electiclas);
            this.groupBox1.Controls.Add(this.lights);
            this.groupBox1.Controls.Add(this.Tomtdate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.Frommtdate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnsearch);
            this.groupBox1.Controls.Add(this.txtproductname);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(10, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1018, 48);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Material Transcation Report";
            // 
            // Tomtdate
            // 
            this.Tomtdate.CustomFormat = "dd-MM-yyyy";
            this.Tomtdate.Font = new System.Drawing.Font("Calibri", 8F);
            this.Tomtdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Tomtdate.Location = new System.Drawing.Point(665, 17);
            this.Tomtdate.Name = "Tomtdate";
            this.Tomtdate.Size = new System.Drawing.Size(109, 21);
            this.Tomtdate.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(633, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "To";
            // 
            // Frommtdate
            // 
            this.Frommtdate.CustomFormat = "dd-MM-yyyy";
            this.Frommtdate.Font = new System.Drawing.Font("Calibri", 8F);
            this.Frommtdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Frommtdate.Location = new System.Drawing.Point(506, 18);
            this.Frommtdate.Name = "Frommtdate";
            this.Frommtdate.Size = new System.Drawing.Size(109, 21);
            this.Frommtdate.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(461, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "From";
            // 
            // btnsearch
            // 
            this.btnsearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnsearch.Location = new System.Drawing.Point(942, 16);
            this.btnsearch.Name = "btnsearch";
            this.btnsearch.Size = new System.Drawing.Size(69, 23);
            this.btnsearch.TabIndex = 4;
            this.btnsearch.Text = "Search";
            this.btnsearch.UseVisualStyleBackColor = true;
            this.btnsearch.Click += new System.EventHandler(this.btnsearch_Click);
            // 
            // txtproductname
            // 
            this.txtproductname.Location = new System.Drawing.Point(103, 19);
            this.txtproductname.Name = "txtproductname";
            this.txtproductname.Size = new System.Drawing.Size(348, 23);
            this.txtproductname.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Key Word:";
            // 
            // lblstock
            // 
            this.lblstock.AutoSize = true;
            this.lblstock.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblstock.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblstock.Location = new System.Drawing.Point(950, 76);
            this.lblstock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblstock.Name = "lblstock";
            this.lblstock.Size = new System.Drawing.Size(28, 31);
            this.lblstock.TabIndex = 26;
            this.lblstock.Text = "0";
            this.lblstock.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.DarkBlue;
            this.label7.Location = new System.Drawing.Point(803, 75);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(152, 31);
            this.label7.TabIndex = 25;
            this.label7.Text = "Total Stock :";
            this.label7.Visible = false;
            // 
            // lblout
            // 
            this.lblout.AutoSize = true;
            this.lblout.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblout.ForeColor = System.Drawing.Color.Red;
            this.lblout.Location = new System.Drawing.Point(541, 76);
            this.lblout.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblout.Name = "lblout";
            this.lblout.Size = new System.Drawing.Size(28, 31);
            this.lblout.TabIndex = 24;
            this.lblout.Text = "0";
            this.lblout.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(415, 74);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 31);
            this.label5.TabIndex = 23;
            this.label5.Text = "Total Out :";
            this.label5.Visible = false;
            // 
            // lblin
            // 
            this.lblin.AutoSize = true;
            this.lblin.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblin.Location = new System.Drawing.Point(147, 74);
            this.lblin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblin.Name = "lblin";
            this.lblin.Size = new System.Drawing.Size(28, 31);
            this.lblin.TabIndex = 22;
            this.lblin.Text = "0";
            this.lblin.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label4.Location = new System.Drawing.Point(40, 73);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 31);
            this.label4.TabIndex = 21;
            this.label4.Text = "Total In :";
            this.label4.Visible = false;
            // 
            // electiclas
            // 
            this.electiclas.AutoSize = true;
            this.electiclas.Location = new System.Drawing.Point(852, 19);
            this.electiclas.Name = "electiclas";
            this.electiclas.Size = new System.Drawing.Size(85, 20);
            this.electiclas.TabIndex = 494;
            this.electiclas.Text = "Electricals";
            this.electiclas.UseVisualStyleBackColor = true;
            // 
            // lights
            // 
            this.lights.AutoSize = true;
            this.lights.Location = new System.Drawing.Point(780, 19);
            this.lights.Name = "lights";
            this.lights.Size = new System.Drawing.Size(62, 20);
            this.lights.TabIndex = 493;
            this.lights.Text = "Lights";
            this.lights.UseVisualStyleBackColor = true;
            // 
            // MaterialTranscationreport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1035, 644);
            this.Controls.Add(this.lblstock);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblout);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dgvStockrpt);
            this.Controls.Add(this.groupBox1);
            this.Name = "MaterialTranscationreport";
            this.Text = "MaterialTranscationreport";
            this.Load += new System.EventHandler(this.MaterialTranscationreport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockrpt)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvStockrpt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnsearch;
        private System.Windows.Forms.TextBox txtproductname;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker Tomtdate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker Frommtdate;
        private System.Windows.Forms.Label lblstock;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblout;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox electiclas;
        private System.Windows.Forms.CheckBox lights;
    }
}