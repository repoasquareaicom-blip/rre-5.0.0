namespace Inventory.Purchase
{
    partial class Purchase_Ageing
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSearch = new System.Windows.Forms.Button();
            this.DTPTodate = new System.Windows.Forms.DateTimePicker();
            this.dtfromdate = new System.Windows.Forms.DateTimePicker();
            this.lblOrderDateTo = new System.Windows.Forms.Label();
            this.lblOrderDateFrom = new System.Windows.Forms.Label();
            this.dgvSearch = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtvendor = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearch.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Location = new System.Drawing.Point(755, 45);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 25);
            this.btnSearch.TabIndex = 131;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // DTPTodate
            // 
            this.DTPTodate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DTPTodate.Location = new System.Drawing.Point(591, 47);
            this.DTPTodate.Name = "DTPTodate";
            this.DTPTodate.Size = new System.Drawing.Size(123, 20);
            this.DTPTodate.TabIndex = 128;
            // 
            // dtfromdate
            // 
            this.dtfromdate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtfromdate.Location = new System.Drawing.Point(384, 47);
            this.dtfromdate.Name = "dtfromdate";
            this.dtfromdate.Size = new System.Drawing.Size(123, 20);
            this.dtfromdate.TabIndex = 127;
            // 
            // lblOrderDateTo
            // 
            this.lblOrderDateTo.AutoSize = true;
            this.lblOrderDateTo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrderDateTo.Location = new System.Drawing.Point(532, 49);
            this.lblOrderDateTo.Name = "lblOrderDateTo";
            this.lblOrderDateTo.Size = new System.Drawing.Size(57, 16);
            this.lblOrderDateTo.TabIndex = 130;
            this.lblOrderDateTo.Text = "To Date";
            // 
            // lblOrderDateFrom
            // 
            this.lblOrderDateFrom.AutoSize = true;
            this.lblOrderDateFrom.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrderDateFrom.Location = new System.Drawing.Point(300, 48);
            this.lblOrderDateFrom.Name = "lblOrderDateFrom";
            this.lblOrderDateFrom.Size = new System.Drawing.Size(79, 16);
            this.lblOrderDateFrom.TabIndex = 129;
            this.lblOrderDateFrom.Text = " From Date";
            // 
            // dgvSearch
            // 
            this.dgvSearch.AllowUserToAddRows = false;
            this.dgvSearch.AllowUserToOrderColumns = true;
            this.dgvSearch.AllowUserToResizeRows = false;
            this.dgvSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSearch.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSearch.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSearch.Location = new System.Drawing.Point(12, 88);
            this.dgvSearch.Name = "dgvSearch";
            this.dgvSearch.ReadOnly = true;
            this.dgvSearch.RowHeadersVisible = false;
            this.dgvSearch.Size = new System.Drawing.Size(998, 570);
            this.dgvSearch.TabIndex = 126;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label17);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1020, 32);
            this.panel1.TabIndex = 125;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold);
            this.label17.ForeColor = System.Drawing.Color.SteelBlue;
            this.label17.Location = new System.Drawing.Point(1, 2);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(219, 26);
            this.label17.TabIndex = 5;
            this.label17.Text = "Purchase Ageing Report";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 16);
            this.label1.TabIndex = 132;
            this.label1.Text = "Vendor";
            // 
            // txtvendor
            // 
            this.txtvendor.Location = new System.Drawing.Point(69, 46);
            this.txtvendor.Name = "txtvendor";
            this.txtvendor.Size = new System.Drawing.Size(225, 20);
            this.txtvendor.TabIndex = 133;
            // 
            // Purchase_Ageing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 664);
            this.Controls.Add(this.txtvendor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.DTPTodate);
            this.Controls.Add(this.dtfromdate);
            this.Controls.Add(this.lblOrderDateTo);
            this.Controls.Add(this.lblOrderDateFrom);
            this.Controls.Add(this.dgvSearch);
            this.Controls.Add(this.panel1);
            this.Name = "Purchase_Ageing";
            this.Text = "Purchase_Ageing";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DateTimePicker DTPTodate;
        private System.Windows.Forms.DateTimePicker dtfromdate;
        private System.Windows.Forms.Label lblOrderDateTo;
        private System.Windows.Forms.Label lblOrderDateFrom;
        private System.Windows.Forms.DataGridView dgvSearch;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtvendor;
    }
}