namespace Inventory
{
    partial class Updated
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Updated));
            this.dgvOrder = new System.Windows.Forms.DataGridView();
            this.pnlprodsearch = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.productsearchbttn = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.txtprodsearch = new System.Windows.Forms.TextBox();
            this.Sino = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label25 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrder)).BeginInit();
            this.pnlprodsearch.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvOrder
            // 
            this.dgvOrder.AllowUserToAddRows = false;
            this.dgvOrder.AllowUserToDeleteRows = false;
            this.dgvOrder.AllowUserToOrderColumns = true;
            this.dgvOrder.AllowUserToResizeRows = false;
            this.dgvOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOrder.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Sino,
            this.ProductName,
            this.Price,
            this.Date});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvOrder.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvOrder.Location = new System.Drawing.Point(23, 28);
            this.dgvOrder.Name = "dgvOrder";
            this.dgvOrder.ReadOnly = true;
            this.dgvOrder.RowHeadersVisible = false;
            this.dgvOrder.Size = new System.Drawing.Size(659, 401);
            this.dgvOrder.TabIndex = 6;
            // 
            // pnlprodsearch
            // 
            this.pnlprodsearch.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlprodsearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlprodsearch.Controls.Add(this.panel5);
            this.pnlprodsearch.Controls.Add(this.txtprodsearch);
            this.pnlprodsearch.Location = new System.Drawing.Point(171, 166);
            this.pnlprodsearch.Name = "pnlprodsearch";
            this.pnlprodsearch.Size = new System.Drawing.Size(277, 74);
            this.pnlprodsearch.TabIndex = 7;
            this.pnlprodsearch.Visible = false;
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::Inventory.Properties.Resources.labelBack;
            this.panel5.Controls.Add(this.productsearchbttn);
            this.panel5.Controls.Add(this.label24);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(275, 22);
            this.panel5.TabIndex = 0;
            // 
            // productsearchbttn
            // 
            this.productsearchbttn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.productsearchbttn.BackColor = System.Drawing.Color.Transparent;
            this.productsearchbttn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("productsearchbttn.BackgroundImage")));
            this.productsearchbttn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.productsearchbttn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.productsearchbttn.FlatAppearance.BorderSize = 0;
            this.productsearchbttn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.productsearchbttn.Location = new System.Drawing.Point(249, -1);
            this.productsearchbttn.Name = "productsearchbttn";
            this.productsearchbttn.Size = new System.Drawing.Size(26, 24);
            this.productsearchbttn.TabIndex = 94;
            this.productsearchbttn.UseVisualStyleBackColor = false;
            this.productsearchbttn.Click += new System.EventHandler(this.productsearchbttn_Click);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.BackColor = System.Drawing.Color.Transparent;
            this.label24.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold);
            this.label24.ForeColor = System.Drawing.Color.White;
            this.label24.Location = new System.Drawing.Point(8, 3);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(89, 15);
            this.label24.TabIndex = 93;
            this.label24.Text = "Search Product";
            // 
            // txtprodsearch
            // 
            this.txtprodsearch.Location = new System.Drawing.Point(11, 34);
            this.txtprodsearch.MaxLength = 50;
            this.txtprodsearch.Name = "txtprodsearch";
            this.txtprodsearch.Size = new System.Drawing.Size(257, 20);
            this.txtprodsearch.TabIndex = 4;
            // 
            // Sino
            // 
            this.Sino.HeaderText = "Sino";
            this.Sino.Name = "Sino";
            this.Sino.ReadOnly = true;
            // 
            // ProductName
            // 
            this.ProductName.HeaderText = "ProductName";
            this.ProductName.Name = "ProductName";
            this.ProductName.ReadOnly = true;
            // 
            // Price
            // 
            this.Price.HeaderText = "Price";
            this.Price.Name = "Price";
            this.Price.ReadOnly = true;
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label25.Location = new System.Drawing.Point(596, 441);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(76, 13);
            this.label25.TabIndex = 101;
            this.label25.Text = "F3-> Search";
            // 
            // Updated
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 463);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.pnlprodsearch);
            this.Controls.Add(this.dgvOrder);
            this.Name = "Updated";
            this.Text = "Updated";
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrder)).EndInit();
            this.pnlprodsearch.ResumeLayout(false);
            this.pnlprodsearch.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sino;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.Panel pnlprodsearch;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button productsearchbttn;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtprodsearch;
        private System.Windows.Forms.Label label25;

    }
}