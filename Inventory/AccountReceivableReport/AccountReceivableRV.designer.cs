namespace Inventory.Accounts
{
    partial class CashTransactionReportRV
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
            this.AccountCRV = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.Btnprint = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.rbtnElectrical = new System.Windows.Forms.RadioButton();
            this.rbtnLights = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AccountCRV
            // 
            this.AccountCRV.ActiveViewIndex = -1;
            this.AccountCRV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AccountCRV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AccountCRV.Cursor = System.Windows.Forms.Cursors.Default;
            this.AccountCRV.Location = new System.Drawing.Point(0, 51);
            this.AccountCRV.Name = "AccountCRV";
            this.AccountCRV.Size = new System.Drawing.Size(877, 481);
            this.AccountCRV.TabIndex = 1;
            this.AccountCRV.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(360, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 125;
            this.label3.Text = "To Date";
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "dd-MM-yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(409, 12);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(120, 20);
            this.dtpToDate.TabIndex = 124;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(172, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 123;
            this.label2.Text = "From Date";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd-MM-yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(231, 13);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(120, 20);
            this.dtpFromDate.TabIndex = 122;
            // 
            // Btnprint
            // 
            this.Btnprint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btnprint.FlatAppearance.BorderSize = 0;
            this.Btnprint.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btnprint.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btnprint.Location = new System.Drawing.Point(538, 10);
            this.Btnprint.Name = "Btnprint";
            this.Btnprint.Size = new System.Drawing.Size(75, 25);
            this.Btnprint.TabIndex = 121;
            this.Btnprint.Text = "Print";
            this.Btnprint.UseVisualStyleBackColor = true;
            this.Btnprint.Click += new System.EventHandler(this.Btnprint_Click);
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.SteelBlue;
            this.label20.Location = new System.Drawing.Point(5, 9);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(155, 28);
            this.label20.TabIndex = 120;
            this.label20.Text = "Cash Transaction";
            // 
            // rbtnElectrical
            // 
            this.rbtnElectrical.AutoSize = true;
            this.rbtnElectrical.Checked = true;
            this.rbtnElectrical.Location = new System.Drawing.Point(787, 18);
            this.rbtnElectrical.Name = "rbtnElectrical";
            this.rbtnElectrical.Size = new System.Drawing.Size(90, 17);
            this.rbtnElectrical.TabIndex = 126;
            this.rbtnElectrical.TabStop = true;
            this.rbtnElectrical.Text = "R.R Electrical";
            this.rbtnElectrical.UseVisualStyleBackColor = true;
            this.rbtnElectrical.Visible = false;
            this.rbtnElectrical.CheckedChanged += new System.EventHandler(this.rbtnElectrical_CheckedChanged);
            // 
            // rbtnLights
            // 
            this.rbtnLights.AutoSize = true;
            this.rbtnLights.Location = new System.Drawing.Point(779, 12);
            this.rbtnLights.Name = "rbtnLights";
            this.rbtnLights.Size = new System.Drawing.Size(75, 17);
            this.rbtnLights.TabIndex = 127;
            this.rbtnLights.Text = "R.R Lights";
            this.rbtnLights.UseVisualStyleBackColor = true;
            this.rbtnLights.Visible = false;
            this.rbtnLights.CheckedChanged += new System.EventHandler(this.rbtnLights_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(619, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 25);
            this.button1.TabIndex = 128;
            this.button1.Text = "Print Page";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CashTransactionReportRV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 535);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.rbtnLights);
            this.Controls.Add(this.rbtnElectrical);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtpToDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtpFromDate);
            this.Controls.Add(this.Btnprint);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.AccountCRV);
            this.Name = "CashTransactionReportRV";
            this.Text = "Cash Transaction Report";
            this.Load += new System.EventHandler(this.CashTransactionReportRV_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer AccountCRV;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Button Btnprint;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.RadioButton rbtnElectrical;
        private System.Windows.Forms.RadioButton rbtnLights;
        private System.Windows.Forms.Button button1;


    }
}