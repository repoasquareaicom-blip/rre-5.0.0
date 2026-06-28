namespace Inventory.Sales
{
    partial class Salesbillreport
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
            this.salesreportviewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // salesreportviewer
            // 
            this.salesreportviewer.ActiveViewIndex = -1;
            this.salesreportviewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //this.salesreportviewer.CachedPageNumberPerDoc = 10;
            this.salesreportviewer.Cursor = System.Windows.Forms.Cursors.Default;
            this.salesreportviewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.salesreportviewer.Location = new System.Drawing.Point(0, 0);
            this.salesreportviewer.Name = "salesreportviewer";
            this.salesreportviewer.Size = new System.Drawing.Size(881, 620);
            this.salesreportviewer.TabIndex = 21;
            this.salesreportviewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            this.salesreportviewer.Load += new System.EventHandler(this.salesreportviewer_Load);
            // 
            // Salesbillreport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 620);
            this.Controls.Add(this.salesreportviewer);
            this.Name = "Salesbillreport";
            this.Text = "Salesbillreport";
            this.Load += new System.EventHandler(this.Salesbillreport_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer salesreportviewer;

    }
}