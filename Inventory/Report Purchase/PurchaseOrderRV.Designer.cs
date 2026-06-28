namespace Inventory.Report_Purchase
{
    partial class PurchaseOrderRV
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
            this.PurchaseOrderCRV = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // PurchaseOrderCRV
            // 
            this.PurchaseOrderCRV.ActiveViewIndex = -1;
            this.PurchaseOrderCRV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PurchaseOrderCRV.Cursor = System.Windows.Forms.Cursors.Default;
            this.PurchaseOrderCRV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PurchaseOrderCRV.Location = new System.Drawing.Point(0, 0);
            this.PurchaseOrderCRV.Name = "PurchaseOrderCRV";
            this.PurchaseOrderCRV.Size = new System.Drawing.Size(874, 455);
            this.PurchaseOrderCRV.TabIndex = 0;
            this.PurchaseOrderCRV.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // PurchaseOrderRV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 455);
            this.Controls.Add(this.PurchaseOrderCRV);
            this.Name = "PurchaseOrderRV";
            this.Text = "Purchase Order";
            this.Load += new System.EventHandler(this.PurchaseOrderRV_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer PurchaseOrderCRV;
    }
}