namespace Inventory
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.mainPanel = new System.Windows.Forms.Panel();
            this.brandPanel = new System.Windows.Forms.Panel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblBrandSubTitle = new System.Windows.Forms.Label();
            this.lblBrandTitle = new System.Windows.Forms.Label();
            this.loginCard = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtusername = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtpassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbcompany = new System.Windows.Forms.ComboBox();
            this.ChkFloorChanger = new System.Windows.Forms.CheckBox();
            this.lblFloors = new System.Windows.Forms.Label();
            this.CmbFloorInchanger = new System.Windows.Forms.ComboBox();
            this.ButLogin = new System.Windows.Forms.Button();
            this.lblFooter = new System.Windows.Forms.Label();
            this.lblBranchInfo = new System.Windows.Forms.Label();
            this.lblCloudIcon = new System.Windows.Forms.Label();
            this.lblVersionNumber = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.brandPanel.SuspendLayout();
            this.loginCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this.mainPanel.Controls.Add(this.brandPanel);
            this.mainPanel.Controls.Add(this.loginCard);
            this.mainPanel.Controls.Add(this.lblCloudIcon);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(860, 540);
            this.mainPanel.TabIndex = 0;
            // 
            // brandPanel
            // 
            this.brandPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(44)))), ((int)(((byte)(130)))));
            this.brandPanel.Controls.Add(this.lblVersionNumber);
            this.brandPanel.Controls.Add(this.lblVersion);
            this.brandPanel.Controls.Add(this.lblBrandSubTitle);
            this.brandPanel.Controls.Add(this.lblBrandTitle);
            this.brandPanel.Location = new System.Drawing.Point(0, 0);
            this.brandPanel.Name = "brandPanel";
            this.brandPanel.Size = new System.Drawing.Size(340, 540);
            this.brandPanel.TabIndex = 0;
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(170)))));
            this.lblVersion.Location = new System.Drawing.Point(35, 472);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(270, 24);
            this.lblVersion.TabIndex = 3;
            this.lblVersion.Text = "CloudEye Delight  •  Sync Enabled";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBrandSubTitle
            // 
            this.lblBrandSubTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblBrandSubTitle.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblBrandSubTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(238)))), ((int)(((byte)(255)))));
            this.lblBrandSubTitle.Location = new System.Drawing.Point(41, 260);
            this.lblBrandSubTitle.Name = "lblBrandSubTitle";
            this.lblBrandSubTitle.Size = new System.Drawing.Size(258, 62);
            this.lblBrandSubTitle.TabIndex = 2;
            this.lblBrandSubTitle.Text = "Smart inventory access panel with secure branch login";
            this.lblBrandSubTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBrandTitle
            // 
            this.lblBrandTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblBrandTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 25F, System.Drawing.FontStyle.Bold);
            this.lblBrandTitle.ForeColor = System.Drawing.Color.White;
            this.lblBrandTitle.Location = new System.Drawing.Point(29, 128);
            this.lblBrandTitle.Name = "lblBrandTitle";
            this.lblBrandTitle.Size = new System.Drawing.Size(282, 128);
            this.lblBrandTitle.TabIndex = 1;
            this.lblBrandTitle.Text = "RR\r\nElectricals";
            this.lblBrandTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // loginCard
            // 
            this.loginCard.BackColor = System.Drawing.Color.White;
            this.loginCard.Controls.Add(this.lblWelcome);
            this.loginCard.Controls.Add(this.label1);
            this.loginCard.Controls.Add(this.label2);
            this.loginCard.Controls.Add(this.txtusername);
            this.loginCard.Controls.Add(this.label3);
            this.loginCard.Controls.Add(this.txtpassword);
            this.loginCard.Controls.Add(this.label4);
            this.loginCard.Controls.Add(this.cmbcompany);
            this.loginCard.Controls.Add(this.ChkFloorChanger);
            this.loginCard.Controls.Add(this.lblFloors);
            this.loginCard.Controls.Add(this.CmbFloorInchanger);
            this.loginCard.Controls.Add(this.ButLogin);
            this.loginCard.Controls.Add(this.lblFooter);
            this.loginCard.Controls.Add(this.lblBranchInfo);
            this.loginCard.Location = new System.Drawing.Point(398, 35);
            this.loginCard.Name = "loginCard";
            this.loginCard.Size = new System.Drawing.Size(410, 470);
            this.loginCard.TabIndex = 1;
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI Semibold", 21F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(52)))), ((int)(((byte)(84)))));
            this.lblWelcome.Location = new System.Drawing.Point(34, 28);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(180, 38);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Secure Login";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(112)))), ((int)(((byte)(130)))));
            this.label1.Location = new System.Drawing.Point(38, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "RR Electricals inventory system";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(80)))), ((int)(((byte)(96)))));
            this.label2.Location = new System.Drawing.Point(38, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "User Name";
            // 
            // txtusername
            // 
            this.txtusername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.txtusername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtusername.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtusername.Location = new System.Drawing.Point(41, 158);
            this.txtusername.Name = "txtusername";
            this.txtusername.Size = new System.Drawing.Size(325, 27);
            this.txtusername.TabIndex = 0;
            this.txtusername.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtusername_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(80)))), ((int)(((byte)(96)))));
            this.label3.Location = new System.Drawing.Point(38, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Password";
            // 
            // txtpassword
            // 
            this.txtpassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.txtpassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtpassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtpassword.Location = new System.Drawing.Point(41, 214);
            this.txtpassword.Name = "txtpassword";
            this.txtpassword.Size = new System.Drawing.Size(325, 27);
            this.txtpassword.TabIndex = 1;
            this.txtpassword.UseSystemPasswordChar = true;
            this.txtpassword.WordWrap = false;
            this.txtpassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtpassword_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(80)))), ((int)(((byte)(96)))));
            this.label4.Location = new System.Drawing.Point(38, 249);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Company";
            // 
            // cmbcompany
            // 
            this.cmbcompany.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbcompany.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbcompany.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.cmbcompany.DisplayMember = "1";
            this.cmbcompany.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbcompany.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbcompany.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbcompany.FormattingEnabled = true;
            this.cmbcompany.Location = new System.Drawing.Point(41, 270);
            this.cmbcompany.Name = "cmbcompany";
            this.cmbcompany.Size = new System.Drawing.Size(325, 25);
            this.cmbcompany.TabIndex = 2;
            this.cmbcompany.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtpassword_KeyDown);
            // 
            // ChkFloorChanger
            // 
            this.ChkFloorChanger.AutoSize = true;
            this.ChkFloorChanger.BackColor = System.Drawing.Color.Transparent;
            this.ChkFloorChanger.Font = new System.Drawing.Font("Segoe UI", 9.2F);
            this.ChkFloorChanger.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(88)))), ((int)(((byte)(108)))));
            this.ChkFloorChanger.Location = new System.Drawing.Point(41, 306);
            this.ChkFloorChanger.Name = "ChkFloorChanger";
            this.ChkFloorChanger.Size = new System.Drawing.Size(111, 21);
            this.ChkFloorChanger.TabIndex = 3;
            this.ChkFloorChanger.Text = "Floor Incharge";
            this.ChkFloorChanger.UseVisualStyleBackColor = false;
            this.ChkFloorChanger.Visible = false;
            this.ChkFloorChanger.CheckedChanged += new System.EventHandler(this.ChkFloorChanger_CheckedChanged);
            // 
            // lblFloors
            // 
            this.lblFloors.AutoSize = true;
            this.lblFloors.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblFloors.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(80)))), ((int)(((byte)(96)))));
            this.lblFloors.Location = new System.Drawing.Point(38, 329);
            this.lblFloors.Name = "lblFloors";
            this.lblFloors.Size = new System.Drawing.Size(45, 17);
            this.lblFloors.TabIndex = 9;
            this.lblFloors.Text = "Floors";
            this.lblFloors.Visible = false;
            // 
            // CmbFloorInchanger
            // 
            this.CmbFloorInchanger.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CmbFloorInchanger.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CmbFloorInchanger.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.CmbFloorInchanger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbFloorInchanger.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CmbFloorInchanger.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.CmbFloorInchanger.FormattingEnabled = true;
            this.CmbFloorInchanger.Location = new System.Drawing.Point(41, 350);
            this.CmbFloorInchanger.Name = "CmbFloorInchanger";
            this.CmbFloorInchanger.Size = new System.Drawing.Size(325, 25);
            this.CmbFloorInchanger.TabIndex = 4;
            this.CmbFloorInchanger.Visible = false;
            this.CmbFloorInchanger.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CmbFloorInchanger_KeyDown);
            // 
            // ButLogin
            // 
            this.ButLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(112)))), ((int)(((byte)(67)))));
            this.ButLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ButLogin.FlatAppearance.BorderSize = 0;
            this.ButLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButLogin.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.ButLogin.ForeColor = System.Drawing.Color.White;
            this.ButLogin.Location = new System.Drawing.Point(41, 382);
            this.ButLogin.Name = "ButLogin";
            this.ButLogin.Size = new System.Drawing.Size(325, 42);
            this.ButLogin.TabIndex = 5;
            this.ButLogin.Text = "LOGIN TO SYSTEM";
            this.ButLogin.UseVisualStyleBackColor = false;
            this.ButLogin.Click += new System.EventHandler(this.ButLogin_Click);
            // 
            // lblFooter
            // 
            this.lblFooter.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblFooter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(154)))), ((int)(((byte)(170)))));
            this.lblFooter.Location = new System.Drawing.Point(38, 434);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(330, 20);
            this.lblFooter.TabIndex = 12;
            this.lblFooter.Text = "Powered by CloudEye Delight";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBranchInfo
            // 
            this.lblBranchInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(247)))), ((int)(((byte)(237)))));
            this.lblBranchInfo.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblBranchInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(65)))), ((int)(((byte)(12)))));
            this.lblBranchInfo.Location = new System.Drawing.Point(41, 93);
            this.lblBranchInfo.Name = "lblBranchInfo";
            this.lblBranchInfo.Size = new System.Drawing.Size(325, 30);
            this.lblBranchInfo.TabIndex = 13;
            this.lblBranchInfo.Text = "Branch :";
            this.lblBranchInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCloudIcon
            // 
            this.lblCloudIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.lblCloudIcon.Font = new System.Drawing.Font("Segoe UI Semibold", 32F, System.Drawing.FontStyle.Bold);
            this.lblCloudIcon.ForeColor = System.Drawing.Color.White;
            this.lblCloudIcon.Location = new System.Drawing.Point(293, 350);
            this.lblCloudIcon.Name = "lblCloudIcon";
            this.lblCloudIcon.Size = new System.Drawing.Size(124, 92);
            this.lblCloudIcon.TabIndex = 0;
            this.lblCloudIcon.Text = "RR";
            this.lblCloudIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCloudIcon.Visible = false;
            // 
            // lblVersionNumber
            // 
            this.lblVersionNumber.BackColor = System.Drawing.Color.Transparent;
            this.lblVersionNumber.Font = new System.Drawing.Font("Segoe UI Semibold", 25F, System.Drawing.FontStyle.Bold);
            this.lblVersionNumber.ForeColor = System.Drawing.Color.White;
            this.lblVersionNumber.Location = new System.Drawing.Point(23, 324);
            this.lblVersionNumber.Name = "lblVersionNumber";
            this.lblVersionNumber.Size = new System.Drawing.Size(282, 128);
            this.lblVersionNumber.TabIndex = 4;
            this.lblVersionNumber.Text = "Version";
            this.lblVersionNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(860, 540);
            this.Controls.Add(this.mainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RR Electricals - Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Login_FormClosing);
            this.mainPanel.ResumeLayout(false);
            this.brandPanel.ResumeLayout(false);
            this.loginCard.ResumeLayout(false);
            this.loginCard.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel brandPanel;
        private System.Windows.Forms.Panel loginCard;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblBrandSubTitle;
        private System.Windows.Forms.Label lblBrandTitle;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtusername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtpassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbcompany;
        private System.Windows.Forms.CheckBox ChkFloorChanger;
        private System.Windows.Forms.Label lblFloors;
        private System.Windows.Forms.ComboBox CmbFloorInchanger;
        private System.Windows.Forms.Button ButLogin;
        private System.Windows.Forms.Label lblFooter;
        private System.Windows.Forms.Label lblBranchInfo;
        private System.Windows.Forms.Label lblCloudIcon;
        private System.Windows.Forms.Label lblVersionNumber;
    }
}
