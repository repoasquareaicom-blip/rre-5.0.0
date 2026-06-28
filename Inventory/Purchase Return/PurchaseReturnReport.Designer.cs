namespace Inventory.Purchase_Return
{
    partial class PurchaseReturnReport
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
            this.RptViewerPurchaseReturn = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // RptViewerPurchaseReturn
            // 
            this.RptViewerPurchaseReturn.ActiveViewIndex = -1;
            this.RptViewerPurchaseReturn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RptViewerPurchaseReturn.Cursor = System.Windows.Forms.Cursors.Default;
            this.RptViewerPurchaseReturn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RptViewerPurchaseReturn.Location = new System.Drawing.Point(0, 0);
            this.RptViewerPurchaseReturn.Name = "RptViewerPurchaseReturn";
            this.RptViewerPurchaseReturn.Size = new System.Drawing.Size(881, 620);
            this.RptViewerPurchaseReturn.TabIndex = 0;
            // 
            // PurchaseReturnReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 620);
            this.Controls.Add(this.RptViewerPurchaseReturn);
            this.Name = "PurchaseReturnReport";
            this.Text = "PurchaseReturnReport";
            this.Load += new System.EventHandler(this.PurchaseReturnReport_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer RptViewerPurchaseReturn;
    }
}