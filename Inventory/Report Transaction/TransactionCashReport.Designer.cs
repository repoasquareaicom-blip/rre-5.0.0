namespace Inventory.Report_Transaction
{
    partial class TransactionCashReport
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
            this.TCReportViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // TCReportViewer
            // 
            this.TCReportViewer.ActiveViewIndex = -1;
            this.TCReportViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TCReportViewer.Cursor = System.Windows.Forms.Cursors.Default;
            this.TCReportViewer.DisplayStatusBar = false;
            this.TCReportViewer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.TCReportViewer.Location = new System.Drawing.Point(0, 1);
            this.TCReportViewer.Name = "TCReportViewer";
            this.TCReportViewer.Size = new System.Drawing.Size(888, 521);
            this.TCReportViewer.TabIndex = 0;
            this.TCReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // TransactionCashReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 522);
            this.Controls.Add(this.TCReportViewer);
            this.Name = "TransactionCashReport";
            this.Text = "TransactionCashReport";
            this.Load += new System.EventHandler(this.TransactionCashReport_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer TCReportViewer;
    }
}