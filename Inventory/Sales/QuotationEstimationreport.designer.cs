namespace Inventory.Sales
{
    partial class QuotationEstimationreport
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
            this.Quotationreportviewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // Quotationreportviewer
            // 
            this.Quotationreportviewer.ActiveViewIndex = -1;
            this.Quotationreportviewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Quotationreportviewer.Cursor = System.Windows.Forms.Cursors.Default;
            this.Quotationreportviewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Quotationreportviewer.Location = new System.Drawing.Point(0, 0);
            this.Quotationreportviewer.Name = "Quotationreportviewer";
            this.Quotationreportviewer.Size = new System.Drawing.Size(881, 620);
            this.Quotationreportviewer.TabIndex = 20;
            this.Quotationreportviewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // QuotationEstimationreport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 620);
            this.Controls.Add(this.Quotationreportviewer);
            this.Name = "QuotationEstimationreport";
            this.Text = "QuotationEstimationreport";
            this.Load += new System.EventHandler(this.QuotationEstimationreport_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer Quotationreportviewer;
    }
}