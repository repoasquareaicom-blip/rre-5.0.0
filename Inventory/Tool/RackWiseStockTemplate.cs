using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Tool
{
    public class RackWiseStockTemplate : Form
    {
        private const string AssignmentSql =
            "SELECT DISTINCT p.id AS ProductId, ISNULL(p.ItemCode, '') AS ItemCode, ISNULL(p.ItemName, '') AS ProductName, "
            + "ISNULL(u.UOM, ISNULL(p.UOM, '')) AS UOM, rm.RackId, rm.RackCaption, "
            + "ISNULL(lm.DisplayOrder, 2147483647) AS LocationOrder, ISNULL(rm.DisplayOrder, 2147483647) AS RackOrder "
            + "FROM dbo.ProductRackMapping prm "
            + "INNER JOIN dbo.RackMaster rm ON rm.RackId = prm.RackId AND rm.IsActive = 1 "
            + "INNER JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId AND lm.IsActive = 1 "
            + "INNER JOIN dbo.ProductMaster p ON p.id = prm.ProductId "
            + "LEFT JOIN dbo.UOM u ON CONVERT(varchar(50), u.Uomid) = LTRIM(RTRIM(p.UOM))";

        private TextBox txtSearch;
        private DataGridView dgvAvailable;
        private DataGridView dgvSelected;
        private Button btnAdd;
        private Button btnAddAll;
        private Button btnRemove;
        private Button btnRemoveAll;
        private Button btnDownload;
        private Label lblAvailable;
        private Label lblSelectedProducts;
        private Label lblTemplateRows;

        private Dictionary<int, ProductInfo> products = new Dictionary<int, ProductInfo>();
        private readonly List<int> selectedIds = new List<int>();

        public RackWiseStockTemplate()
        {
            Text = "Rack Wise Stock Upload - Download Template";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1180, 640);
            MinimumSize = new Size(900, 480);
            Font = new Font("Segoe UI", 9F);
            BuildLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                products = ToProducts(LoadAssignments(null));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load product rack assignments.\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            RefreshGrids();
        }

        private void BuildLayout()
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 3;
            layout.RowCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layout.Padding = new Padding(8);
            Controls.Add(layout);

            Panel left = new Panel();
            left.Dock = DockStyle.Fill;
            dgvAvailable = MakeGrid();
            left.Controls.Add(dgvAvailable);
            Panel leftTop = new Panel();
            leftTop.Dock = DockStyle.Top;
            leftTop.Height = 58;
            Label leftCaption = new Label();
            leftCaption.Text = "Available Products";
            leftCaption.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            leftCaption.Location = new Point(0, 2);
            leftCaption.AutoSize = true;
            leftTop.Controls.Add(leftCaption);
            Label searchCaption = new Label();
            searchCaption.Text = "Search (Product ID / Item Code / Name) :";
            searchCaption.Location = new Point(0, 32);
            searchCaption.AutoSize = true;
            leftTop.Controls.Add(searchCaption);
            txtSearch = new TextBox();
            txtSearch.Location = new Point(240, 29);
            txtSearch.Width = 260;
            txtSearch.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            txtSearch.TextChanged += new EventHandler(txtSearch_TextChanged);
            leftTop.Controls.Add(txtSearch);
            left.Controls.Add(leftTop);
            layout.Controls.Add(left, 0, 0);

            FlowLayoutPanel center = new FlowLayoutPanel();
            center.Dock = DockStyle.Fill;
            center.FlowDirection = FlowDirection.TopDown;
            center.Padding = new Padding(8, 150, 0, 0);
            btnAdd = CenterButton(center, ">", "Add selected product(s)");
            btnAdd.Click += new EventHandler(btnAdd_Click);
            btnAddAll = CenterButton(center, ">>", "Add all currently filtered products");
            btnAddAll.Click += new EventHandler(btnAddAll_Click);
            btnRemove = CenterButton(center, "<", "Remove selected product(s) with all their racks");
            btnRemove.Click += new EventHandler(btnRemove_Click);
            btnRemoveAll = CenterButton(center, "<<", "Remove all");
            btnRemoveAll.Click += new EventHandler(btnRemoveAll_Click);
            layout.Controls.Add(center, 1, 0);

            Panel right = new Panel();
            right.Dock = DockStyle.Fill;
            dgvSelected = MakeGrid();
            right.Controls.Add(dgvSelected);
            Panel rightTop = new Panel();
            rightTop.Dock = DockStyle.Top;
            rightTop.Height = 58;
            Label rightCaption = new Label();
            rightCaption.Text = "Selected Products / Racks";
            rightCaption.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            rightCaption.Location = new Point(0, 2);
            rightCaption.AutoSize = true;
            rightTop.Controls.Add(rightCaption);
            Label rightNote = new Label();
            rightNote.Text = "All active assigned racks of a selected product are included automatically.";
            rightNote.ForeColor = Color.DimGray;
            rightNote.Location = new Point(0, 32);
            rightNote.AutoSize = true;
            rightTop.Controls.Add(rightNote);
            right.Controls.Add(rightTop);
            layout.Controls.Add(right, 2, 0);

            Panel bottom = new Panel();
            bottom.Dock = DockStyle.Fill;
            lblAvailable = BottomLabel(bottom, 0);
            lblSelectedProducts = BottomLabel(bottom, 220);
            lblTemplateRows = BottomLabel(bottom, 440);
            btnDownload = new Button();
            btnDownload.Text = "Download Template";
            btnDownload.Size = new Size(150, 30);
            btnDownload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDownload.Click += new EventHandler(btnDownload_Click);
            bottom.Controls.Add(btnDownload);
            Button btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Size = new Size(90, 30);
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.Cancel;
            bottom.Controls.Add(btnClose);
            bottom.Resize += delegate
            {
                btnClose.Location = new Point(bottom.Width - btnClose.Width, 10);
                btnDownload.Location = new Point(btnClose.Left - btnDownload.Width - 8, 10);
            };
            CancelButton = btnClose;
            layout.Controls.Add(bottom, 0, 1);
            layout.SetColumnSpan(bottom, 3);
        }

        private static DataGridView MakeGrid()
        {
            DataGridView grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.RowHeadersVisible = false;
            grid.MultiSelect = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.BackgroundColor = Color.White;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.DataBindingComplete += delegate { grid.ClearSelection(); };
            return grid;
        }

        private Button CenterButton(Control parent, string text, string tip)
        {
            Button button = new Button();
            button.Text = text;
            button.Size = new Size(50, 32);
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button.Margin = new Padding(0, 0, 0, 10);
            new ToolTip().SetToolTip(button, tip);
            parent.Controls.Add(button);
            return button;
        }

        private static Label BottomLabel(Control parent, int x)
        {
            Label label = new Label();
            label.Location = new Point(x, 16);
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            parent.Controls.Add(label);
            return label;
        }

        private static DataTable LoadAssignments(List<int> productIds)
        {
            string sql = AssignmentSql;
            if (productIds != null)
            {
                if (productIds.Count == 0)
                    return new DataTable();
                StringBuilder ids = new StringBuilder();
                foreach (int id in productIds)
                {
                    if (ids.Length > 0)
                        ids.Append(',');
                    ids.Append(id.ToString());
                }
                sql += " WHERE prm.ProductId IN (" + ids + ")";
            }
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, con))
            {
                adapter.SelectCommand.CommandTimeout = 300;
                adapter.Fill(table);
            }
            return table;
        }

        private static Dictionary<int, ProductInfo> ToProducts(DataTable table)
        {
            Dictionary<int, ProductInfo> result = new Dictionary<int, ProductInfo>();
            foreach (DataRow dr in table.Rows)
            {
                int productId = Convert.ToInt32(dr["ProductId"]);
                ProductInfo product;
                if (!result.TryGetValue(productId, out product))
                {
                    product = new ProductInfo();
                    product.ProductId = productId;
                    product.ItemCode = Convert.ToString(dr["ItemCode"]).Trim();
                    product.ProductName = Convert.ToString(dr["ProductName"]).Trim();
                    product.Uom = Convert.ToString(dr["UOM"]).Trim();
                    result.Add(productId, product);
                }
                int rackId = Convert.ToInt32(dr["RackId"]);
                bool exists = false;
                foreach (RackInfo existing in product.Racks)
                {
                    if (existing.RackId == rackId)
                        exists = true;
                }
                if (exists)
                    continue;
                RackInfo rack = new RackInfo();
                rack.RackId = rackId;
                rack.RackCaption = Convert.ToString(dr["RackCaption"]).Trim();
                rack.LocationOrder = Convert.ToInt32(dr["LocationOrder"]);
                rack.RackOrder = Convert.ToInt32(dr["RackOrder"]);
                product.Racks.Add(rack);
            }
            foreach (ProductInfo product in result.Values)
                product.Racks.Sort(CompareRacks);
            return result;
        }

        private static int CompareRacks(RackInfo a, RackInfo b)
        {
            int c = a.LocationOrder.CompareTo(b.LocationOrder);
            if (c == 0)
                c = a.RackOrder.CompareTo(b.RackOrder);
            if (c == 0)
                c = string.Compare(a.RackCaption, b.RackCaption, StringComparison.OrdinalIgnoreCase);
            return c;
        }

        private static int CompareProducts(ProductInfo a, ProductInfo b)
        {
            int c = string.Compare(a.ProductName, b.ProductName, StringComparison.OrdinalIgnoreCase);
            return c != 0 ? c : a.ProductId.CompareTo(b.ProductId);
        }

        private List<ProductInfo> FilteredAvailable()
        {
            string filter = txtSearch.Text.Trim();
            List<ProductInfo> result = new List<ProductInfo>();
            foreach (ProductInfo product in products.Values)
            {
                if (selectedIds.Contains(product.ProductId))
                    continue;
                if (filter.Length > 0
                    && product.ProductId.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0
                    && product.ItemCode.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0
                    && product.ProductName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                result.Add(product);
            }
            result.Sort(CompareProducts);
            return result;
        }

        private List<ProductInfo> SelectedProducts()
        {
            List<ProductInfo> result = new List<ProductInfo>();
            foreach (int id in selectedIds)
            {
                ProductInfo product;
                if (products.TryGetValue(id, out product))
                    result.Add(product);
            }
            result.Sort(CompareProducts);
            return result;
        }

        private void RefreshGrids()
        {
            DataTable available = new DataTable();
            available.Columns.Add("Product ID", typeof(int));
            available.Columns.Add("Product Name", typeof(string));
            available.Columns.Add("UOM", typeof(string));
            available.Columns.Add("Assigned Racks", typeof(string));
            foreach (ProductInfo product in FilteredAvailable())
            {
                List<string> racks = new List<string>();
                foreach (RackInfo rack in product.Racks)
                    racks.Add(rack.RackCaption);
                available.Rows.Add(product.ProductId, product.ProductName, product.Uom, string.Join(", ", racks.ToArray()));
            }
            dgvAvailable.DataSource = available;
            SetWeights(dgvAvailable, new float[] { 60, 200, 45, 120 });
            dgvAvailable.ClearSelection();

            DataTable selected = new DataTable();
            selected.Columns.Add("Product ID", typeof(int));
            selected.Columns.Add("Product Name", typeof(string));
            selected.Columns.Add("UOM", typeof(string));
            selected.Columns.Add("Rack Name", typeof(string));
            int templateRows = 0;
            foreach (ProductInfo product in SelectedProducts())
            {
                foreach (RackInfo rack in product.Racks)
                {
                    selected.Rows.Add(product.ProductId, product.ProductName, product.Uom, rack.RackCaption);
                    templateRows++;
                }
            }
            dgvSelected.DataSource = selected;
            SetWeights(dgvSelected, new float[] { 60, 200, 45, 80 });
            dgvSelected.ClearSelection();

            lblAvailable.Text = "Available Products: " + available.Rows.Count;
            lblSelectedProducts.Text = "Selected Products: " + selectedIds.Count;
            lblTemplateRows.Text = "Template Rows: " + templateRows;
            btnDownload.Enabled = selectedIds.Count > 0;
            btnAdd.Enabled = available.Rows.Count > 0;
            btnAddAll.Enabled = available.Rows.Count > 0;
            btnRemove.Enabled = selectedIds.Count > 0;
            btnRemoveAll.Enabled = selectedIds.Count > 0;
        }

        private static void SetWeights(DataGridView grid, float[] weights)
        {
            for (int i = 0; i < grid.Columns.Count && i < weights.Length; i++)
            {
                grid.Columns[i].FillWeight = weights[i];
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            if (grid.Columns.Count > 0)
                grid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private static List<int> SelectedProductIds(DataGridView grid)
        {
            List<int> ids = new List<int>();
            foreach (DataGridViewRow row in grid.SelectedRows)
            {
                int id = Convert.ToInt32(row.Cells["Product ID"].Value);
                if (!ids.Contains(id))
                    ids.Add(id);
            }
            return ids;
        }

        private void AddProducts(List<int> ids)
        {
            foreach (int id in ids)
            {
                if (products.ContainsKey(id) && !selectedIds.Contains(id))
                    selectedIds.Add(id);
            }
            RefreshGrids();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            RefreshGrids();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddProducts(SelectedProductIds(dgvAvailable));
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            List<int> ids = new List<int>();
            foreach (ProductInfo product in FilteredAvailable())
                ids.Add(product.ProductId);
            AddProducts(ids);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (int id in SelectedProductIds(dgvSelected))
                selectedIds.Remove(id);
            RefreshGrids();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            selectedIds.Clear();
            RefreshGrids();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (selectedIds.Count == 0)
                return;

            Dictionary<int, ProductInfo> latest;
            try
            {
                latest = ToProducts(LoadAssignments(new List<int>(selectedIds)));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not refresh product rack assignments.\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string before = SelectionSignature(products);
            List<int> dropped = new List<int>();
            foreach (int id in selectedIds)
            {
                ProductInfo product;
                if (latest.TryGetValue(id, out product))
                    products[id] = product;
                else
                {
                    dropped.Add(id);
                    products.Remove(id);
                }
            }
            foreach (int id in dropped)
                selectedIds.Remove(id);
            bool changed = before != SelectionSignature(products);
            RefreshGrids();

            if (changed)
            {
                string message = "Rack assignments changed after the products were selected. The selected list has been refreshed with the latest active rack assignments.";
                if (dropped.Count > 0)
                    message += "\n\nRemoved (no active rack assigned any more): Product ID " + JoinIds(dropped) + ".";
                MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (selectedIds.Count == 0)
                return;

            string path;
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx|Excel 97-2003 Workbook (*.xls)|*.xls";
                dialog.FileName = "RackWiseStockTemplate_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";
                dialog.Title = "Save Rack Wise Stock Template";
                dialog.OverwritePrompt = true;
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                path = dialog.FileName;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                int rows = WriteTemplate(path, SelectedProducts());
                MessageBox.Show("Template saved.\n\n" + path + "\n\nProducts: " + selectedIds.Count + "\nTemplate Rows: " + rows
                    + "\n\nEnter the physical stock in the Stock column. Blank Stock is not accepted; enter 0 for an empty rack.",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save the template. Close the file if it is open in Excel and try again.\n\n" + ex.Message,
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private string SelectionSignature(Dictionary<int, ProductInfo> source)
        {
            StringBuilder text = new StringBuilder();
            List<int> ids = new List<int>(selectedIds);
            ids.Sort();
            foreach (int id in ids)
            {
                ProductInfo product;
                text.Append(id).Append(':');
                if (source.TryGetValue(id, out product))
                {
                    text.Append(product.ProductName).Append('|').Append(product.Uom).Append('|');
                    foreach (RackInfo rack in product.Racks)
                        text.Append(rack.RackId).Append('=').Append(rack.RackCaption).Append(',');
                }
                text.Append(';');
            }
            return text.ToString();
        }

        private static string JoinIds(List<int> ids)
        {
            List<string> text = new List<string>();
            foreach (int id in ids)
                text.Add(id.ToString());
            return string.Join(", ", text.ToArray());
        }

        public static int WriteTemplate(string path, List<ProductInfo> selected)
        {
            bool xls = Path.GetExtension(path).ToLowerInvariant() == ".xls";
            string connection = xls
                ? "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=Yes'"
                : "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0 Xml;HDR=Yes'";
            if (File.Exists(path))
                File.Delete(path);

            int rows = 0;
            using (OleDbConnection con = new OleDbConnection(connection))
            {
                con.Open();
                using (OleDbCommand create = new OleDbCommand(
                    "CREATE TABLE [RackStock] ([Product ID] double, [Product Name] varchar(255), [UOM] varchar(100), [Rack Name] varchar(100), [Stock] double)", con))
                    create.ExecuteNonQuery();

                using (OleDbCommand insert = new OleDbCommand(
                    "INSERT INTO [RackStock] ([Product ID], [Product Name], [UOM], [Rack Name], [Stock]) VALUES (?, ?, ?, ?, ?)", con))
                {
                    insert.Parameters.Add("@ProductId", OleDbType.Double);
                    insert.Parameters.Add("@ProductName", OleDbType.VarWChar, 255);
                    insert.Parameters.Add("@UOM", OleDbType.VarWChar, 100);
                    insert.Parameters.Add("@RackName", OleDbType.VarWChar, 100);
                    insert.Parameters.Add("@Stock", OleDbType.Double);
                    foreach (ProductInfo product in selected)
                    {
                        foreach (RackInfo rack in product.Racks)
                        {
                            insert.Parameters[0].Value = (double)product.ProductId;
                            insert.Parameters[1].Value = product.ProductName;
                            insert.Parameters[2].Value = product.Uom;
                            insert.Parameters[3].Value = rack.RackCaption;
                            insert.Parameters[4].Value = DBNull.Value;
                            insert.ExecuteNonQuery();
                            rows++;
                        }
                    }
                }
            }
            return rows;
        }

        public class ProductInfo
        {
            public int ProductId;
            public string ItemCode;
            public string ProductName;
            public string Uom;
            public readonly List<RackInfo> Racks = new List<RackInfo>();
        }

        public class RackInfo
        {
            public int RackId;
            public string RackCaption;
            public int LocationOrder;
            public int RackOrder;
        }
    }
}
