namespace Inventory.Movement
{
    partial class issuedreceivedrpt
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
            this.IssuedReceivedreportviewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // IssuedReceivedreportviewer
            // 
            this.IssuedReceivedreportviewer.ActiveViewIndex = -1;
            this.IssuedReceivedreportviewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IssuedReceivedreportviewer.Cursor = System.Windows.Forms.Cursors.Default;
            this.IssuedReceivedreportviewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IssuedReceivedreportviewer.Location = new System.Drawing.Point(0, 0);
            this.IssuedReceivedreportviewer.Name = "IssuedReceivedreportviewer";
            this.IssuedReceivedreportviewer.Size = new System.Drawing.Size(881, 620);
            this.IssuedReceivedreportviewer.TabIndex = 20;
            this.IssuedReceivedreportviewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // issuedreceivedrpt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 620);
            this.Controls.Add(this.IssuedReceivedreportviewer);
            this.Name = "issuedreceivedrpt";
            this.Text = "issuedreceivedrpt";
            this.Load += new System.EventHandler(this.issuedreceivedrpt_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer IssuedReceivedreportviewer;
    }
}