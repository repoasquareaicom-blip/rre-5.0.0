namespace Inventory.Report
{
    partial class RackWiseProductStockReport
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TextBox productSearch;
        private RackMultiSelect rackSearch;
        private System.Windows.Forms.CheckBox showZero;
        private System.Windows.Forms.FlowLayoutPanel filters;
        private System.Windows.Forms.FlowLayoutPanel toolbar;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Button excelButton;
        private System.Windows.Forms.Button pdfButton;
        private System.Windows.Forms.Button printButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();
            Font = new System.Drawing.Font("Segoe UI", 9F);
            Text = "Rack Wise Product Stock Report";
            ClientSize = new System.Drawing.Size(1120, 620);
            MinimumSize = new System.Drawing.Size(640, 420);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            var layout = new System.Windows.Forms.TableLayoutPanel();
            layout.Dock = System.Windows.Forms.DockStyle.Fill;
            layout.Padding = new System.Windows.Forms.Padding(8);
            layout.ColumnCount = 1;
            layout.RowCount = 5;
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            for (int i = 0; i < 3; i++) layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48));
            var title = new System.Windows.Forms.Label();
            title.Text = Text;
            title.AutoSize = true;
            title.Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold);
            title.Padding = new System.Windows.Forms.Padding(4, 4, 4, 10);
            layout.Controls.Add(title, 0, 0);
            filters = new System.Windows.Forms.FlowLayoutPanel();
            filters.Dock = System.Windows.Forms.DockStyle.Fill;
            filters.AutoSize = true;
            filters.WrapContents = true;
            var productLabel = new System.Windows.Forms.Label();
            productLabel.Text = "Product Search";
            productLabel.AutoSize = true;
            productLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            productSearch = new System.Windows.Forms.TextBox();
            productSearch.Width = 260;
            productSearch.MaxLength = 200;
            var tip = new System.Windows.Forms.ToolTip(components);
            tip.SetToolTip(productSearch, "Partial Product ID, Item Code, Display Name or Product Name");
            var rackLabel = new System.Windows.Forms.Label();
            rackLabel.Text = "Rack Search";
            rackLabel.AutoSize = true;
            rackLabel.Margin = productLabel.Margin;
            rackSearch = new RackMultiSelect();
            rackSearch.Width = 230;
            showZero = new System.Windows.Forms.CheckBox();
            showZero.Text = "Show Zero Stock";
            showZero.AutoSize = true;
            searchButton = MakeButton("Search", Search_Click);
            resetButton = MakeButton("Reset", Reset_Click);
            var productFilter = new System.Windows.Forms.FlowLayoutPanel();
            productFilter.AutoSize = true;
            productFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            productFilter.WrapContents = false;
            productFilter.Margin = new System.Windows.Forms.Padding(0);
            productFilter.Controls.AddRange(new System.Windows.Forms.Control[] { productLabel, productSearch });
            var rackFilter = new System.Windows.Forms.FlowLayoutPanel();
            rackFilter.AutoSize = true;
            rackFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            rackFilter.WrapContents = false;
            rackFilter.Margin = new System.Windows.Forms.Padding(0);
            rackFilter.Controls.AddRange(new System.Windows.Forms.Control[] { rackLabel, rackSearch });
            filters.Controls.AddRange(new System.Windows.Forms.Control[] { productFilter, rackFilter, showZero, searchButton, resetButton });
            layout.Controls.Add(filters, 0, 1);
            toolbar = new System.Windows.Forms.FlowLayoutPanel();
            toolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            toolbar.AutoSize = true;
            excelButton = MakeButton("Export Excel", Excel_Click);
            pdfButton = MakeButton("Export PDF", Pdf_Click);
            printButton = MakeButton("Print", Print_Click);
            toolbar.Controls.AddRange(new System.Windows.Forms.Control[] { excelButton, pdfButton, printButton });
            layout.Controls.Add(toolbar, 0, 2);
            grid = new System.Windows.Forms.DataGridView();
            grid.Dock = System.Windows.Forms.DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.RowHeadersVisible = false;
            grid.AutoGenerateColumns = false;
            grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            grid.BackgroundColor = System.Drawing.Color.White;
            grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            grid.Paint += delegate(object sender, System.Windows.Forms.PaintEventArgs e)
            {
                System.Windows.Forms.ControlPaint.DrawBorder(e.Graphics, grid.ClientRectangle, System.Drawing.Color.Black, System.Windows.Forms.ButtonBorderStyle.Solid);
            };
            grid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Single;
            grid.GridColor = System.Drawing.Color.Black;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(235, 238, 242);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Padding = new System.Windows.Forms.Padding(4);
            grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 32;
            grid.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            grid.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            grid.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grid.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grid.RowTemplate.Height = 28;
            grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            grid.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            string[] names = { "Location", "Rack", "ProductId", "ProductName", "UOM", "Quantity" };
            string[] captions = { "Location", "Rack", "Product ID", "Product Name / Summary", "UOM", "Stock Qty" };
            int[] widths = { 150, 120, 90, 480, 80, 120 };
            for (int i = 0; i < names.Length; i++)
            {
                var column = new System.Windows.Forms.DataGridViewTextBoxColumn();
                column.Name = names[i]; column.DataPropertyName = names[i]; column.HeaderText = captions[i]; column.Width = widths[i];
                column.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
                if (names[i] == "ProductName") { column.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill; column.MinimumWidth = 180; }
                if (names[i] == "Quantity") { column.DefaultCellStyle.Format = "0.###"; column.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight; }
                grid.Columns.Add(column);
            }
            grid.CellFormatting += Grid_CellFormatting;
            layout.Controls.Add(grid, 0, 3);
            status = new System.Windows.Forms.Label();
            status.Dock = System.Windows.Forms.DockStyle.Fill;
            status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            layout.Controls.Add(status, 0, 4);
            Controls.Add(layout);
            AcceptButton = searchButton;
            Shown += Report_Shown;
            ResumeLayout(true);
        }

        private System.Windows.Forms.Button MakeButton(string text, System.EventHandler handler)
        {
            var button = new System.Windows.Forms.Button();
            button.Text = text; button.AutoSize = true; button.Height = 30;
            button.Click += handler;
            return button;
        }
    }
}
