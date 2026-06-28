namespace Inventory.Report
{
    partial class Userestimation
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblamount = new System.Windows.Forms.Label();
            this.lblbill = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.btnSearch = new System.Windows.Forms.Button();
            this.DTPTodate = new System.Windows.Forms.DateTimePicker();
            this.dtfromdate = new System.Windows.Forms.DateTimePicker();
            this.lblOrderDateTo = new System.Windows.Forms.Label();
            this.lblOrderDateFrom = new System.Windows.Forms.Label();
            this.ddlRole = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dgvSearch = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblamount);
            this.panel1.Controls.Add(this.lblbill);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1020, 32);
            this.panel1.TabIndex = 7;
            // 
            // lblamount
            // 
            this.lblamount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblamount.AutoSize = true;
            this.lblamount.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblamount.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblamount.Location = new System.Drawing.Point(715, 3);
            this.lblamount.Name = "lblamount";
            this.lblamount.Size = new System.Drawing.Size(64, 25);
            this.lblamount.TabIndex = 7;
            this.lblamount.Text = "label1";
            // 
            // lblbill
            // 
            this.lblbill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblbill.AutoSize = true;
            this.lblbill.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblbill.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblbill.Location = new System.Drawing.Point(534, 3);
            this.lblbill.Name = "lblbill";
            this.lblbill.Size = new System.Drawing.Size(64, 25);
            this.lblbill.TabIndex = 6;
            this.lblbill.Text = "label1";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold);
            this.label17.ForeColor = System.Drawing.Color.SteelBlue;
            this.label17.Location = new System.Drawing.Point(1, 2);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(148, 26);
            this.label17.TabIndex = 5;
            this.label17.Text = "User Bill Report";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(11, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(165, 32);
            this.groupBox1.TabIndex = 390;
            this.groupBox1.TabStop = false;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(87, 10);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(56, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "Helper";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(21, 10);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(51, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Sales";
            this.radioButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearch.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Location = new System.Drawing.Point(856, 36);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 25);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // DTPTodate
            // 
            this.DTPTodate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DTPTodate.Location = new System.Drawing.Point(714, 38);
            this.DTPTodate.Name = "DTPTodate";
            this.DTPTodate.Size = new System.Drawing.Size(123, 20);
            this.DTPTodate.TabIndex = 2;
            // 
            // dtfromdate
            // 
            this.dtfromdate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtfromdate.Location = new System.Drawing.Point(507, 38);
            this.dtfromdate.Name = "dtfromdate";
            this.dtfromdate.Size = new System.Drawing.Size(123, 20);
            this.dtfromdate.TabIndex = 1;
            // 
            // lblOrderDateTo
            // 
            this.lblOrderDateTo.AutoSize = true;
            this.lblOrderDateTo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrderDateTo.Location = new System.Drawing.Point(655, 40);
            this.lblOrderDateTo.Name = "lblOrderDateTo";
            this.lblOrderDateTo.Size = new System.Drawing.Size(57, 16);
            this.lblOrderDateTo.TabIndex = 128;
            this.lblOrderDateTo.Text = "To Date";
            // 
            // lblOrderDateFrom
            // 
            this.lblOrderDateFrom.AutoSize = true;
            this.lblOrderDateFrom.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrderDateFrom.Location = new System.Drawing.Point(423, 39);
            this.lblOrderDateFrom.Name = "lblOrderDateFrom";
            this.lblOrderDateFrom.Size = new System.Drawing.Size(79, 16);
            this.lblOrderDateFrom.TabIndex = 127;
            this.lblOrderDateFrom.Text = " From Date";
            // 
            // ddlRole
            // 
            this.ddlRole.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ddlRole.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ddlRole.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ddlRole.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlRole.FormattingEnabled = true;
            this.ddlRole.Location = new System.Drawing.Point(220, 38);
            this.ddlRole.Name = "ddlRole";
            this.ddlRole.Size = new System.Drawing.Size(197, 22);
            this.ddlRole.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 10F);
            this.label6.Location = new System.Drawing.Point(182, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 16);
            this.label6.TabIndex = 389;
            this.label6.Text = "User";
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
            this.dgvSearch.Location = new System.Drawing.Point(11, 79);
            this.dgvSearch.Name = "dgvSearch";
            this.dgvSearch.ReadOnly = true;
            this.dgvSearch.RowHeadersVisible = false;
            this.dgvSearch.Size = new System.Drawing.Size(998, 570);
            this.dgvSearch.TabIndex = 4;
            // 
            // Userestimation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 664);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgvSearch);
            this.Controls.Add(this.ddlRole);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.DTPTodate);
            this.Controls.Add(this.dtfromdate);
            this.Controls.Add(this.lblOrderDateTo);
            this.Controls.Add(this.lblOrderDateFrom);
            this.Controls.Add(this.panel1);
            this.Name = "Userestimation";
            this.Text = "Userestimation";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DateTimePicker DTPTodate;
        private System.Windows.Forms.DateTimePicker dtfromdate;
        private System.Windows.Forms.Label lblOrderDateTo;
        private System.Windows.Forms.Label lblOrderDateFrom;
        private System.Windows.Forms.ComboBox ddlRole;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView dgvSearch;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label lblamount;
        private System.Windows.Forms.Label lblbill;
    }
}