using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Report
{
    public class FrmHsnCorrection : Form
    {
        private static readonly string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        private readonly HsnSuggestionService hsnSuggestionService = new HsnSuggestionService();
        private readonly DataTable products = new DataTable();
        private readonly TextBox txtSearch = new TextBox();
        private readonly ComboBox cmbStatus = new ComboBox();
        private readonly Button btnSearch = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnCorrectHsn = new Button();
        private readonly DataGridView dgvProducts = new DataGridView();
        private readonly Label lblTotalProducts = new Label();
        private readonly Label lblValidHsn = new Label();
        private readonly Label lblEmptyHsn = new Label();
        private readonly Label lblInvalidHsn = new Label();
        private readonly Label lblPendingCorrection = new Label();

        public FrmHsnCorrection()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RefreshData();
        }

        private void InitializeComponent()
        {
            Text = "HSN Correction";
            BackColor = Color.White;
            Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            ClientSize = new Size(1120, 640);
            MinimumSize = new Size(920, 560);

            Label title = new Label();
            title.Text = "HSN Correction";
            title.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            title.ForeColor = Color.SteelBlue;
            title.SetBounds(14, 10, 250, 30);

            FlowLayoutPanel countPanel = new FlowLayoutPanel();
            countPanel.SetBounds(14, 45, 1085, 36);
            countPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            countPanel.BackColor = Color.Gainsboro;
            countPanel.Padding = new Padding(8, 6, 8, 4);
            countPanel.Controls.Add(lblTotalProducts);
            countPanel.Controls.Add(lblValidHsn);
            countPanel.Controls.Add(lblEmptyHsn);
            countPanel.Controls.Add(lblInvalidHsn);
            countPanel.Controls.Add(lblPendingCorrection);

            ConfigureCountLabel(lblTotalProducts);
            ConfigureCountLabel(lblValidHsn);
            ConfigureCountLabel(lblEmptyHsn);
            ConfigureCountLabel(lblInvalidHsn);
            ConfigureCountLabel(lblPendingCorrection);

            Label lblSearch = new Label();
            lblSearch.Text = "Search Product:";
            lblSearch.SetBounds(14, 95, 110, 24);

            txtSearch.SetBounds(125, 93, 250, 24);
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txtSearch.KeyDown += new KeyEventHandler(txtSearch_KeyDown);

            Label lblStatus = new Label();
            lblStatus.Text = "Status:";
            lblStatus.SetBounds(395, 95, 55, 24);

            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new object[] { "All", "Empty HSN", "Invalid HSN" });
            cmbStatus.SelectedIndex = 0;
            cmbStatus.SetBounds(455, 93, 135, 24);

            ConfigureButton(btnSearch, "Search", 605, 91, 80);
            btnSearch.Click += new EventHandler(btnSearch_Click);

            ConfigureButton(btnRefresh, "Refresh", 693, 91, 82);
            btnRefresh.Click += new EventHandler(btnRefresh_Click);

            ConfigureButton(btnCorrectHsn, "Correct / Update HSN", 785, 91, 165);
            btnCorrectHsn.Click += new EventHandler(btnCorrectHsn_Click);

            dgvProducts.SetBounds(14, 130, 1085, 492);
            dgvProducts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.AllowUserToDeleteRows = false;
            dgvProducts.ReadOnly = true;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.MultiSelect = false;
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.CellDoubleClick += new DataGridViewCellEventHandler(dgvProducts_CellDoubleClick);
            dgvProducts.CellContentClick += new DataGridViewCellEventHandler(dgvProducts_CellContentClick);

            AddGridColumns();

            Controls.Add(title);
            Controls.Add(countPanel);
            Controls.Add(lblSearch);
            Controls.Add(txtSearch);
            Controls.Add(lblStatus);
            Controls.Add(cmbStatus);
            Controls.Add(btnSearch);
            Controls.Add(btnRefresh);
            Controls.Add(btnCorrectHsn);
            Controls.Add(dgvProducts);
        }

        private void RefreshData()
        {
            LoadCounts();
            LoadProducts();
        }

        private void LoadCounts()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conn))
                using (SqlCommand cmd = new SqlCommand(GetCountsSql(), con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblTotalProducts.Text = "Total Products: " + Convert.ToString(reader["TotalProducts"]);
                            lblValidHsn.Text = "Valid HSN: " + Convert.ToString(reader["ValidHsn"]);
                            lblEmptyHsn.Text = "Empty HSN: " + Convert.ToString(reader["EmptyHsn"]);
                            lblInvalidHsn.Text = "Invalid HSN: " + Convert.ToString(reader["InvalidHsn"]);
                            lblPendingCorrection.Text = "Pending Correction: " + Convert.ToString(reader["PendingCorrection"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load HSN counts. " + ex.Message);
            }
        }

        private void LoadProducts()
        {
            try
            {
                products.Rows.Clear();
                using (SqlConnection con = new SqlConnection(Conn))
                using (SqlCommand cmd = new SqlCommand(GetProductsSql(), con))
                using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text.Trim() + "%");
                    cmd.Parameters.AddWithValue("@status", Convert.ToString(cmbStatus.SelectedItem));
                    ad.Fill(products);
                }

                EnsureSuggestionColumns(products);
                dgvProducts.DataSource = products;
                ApplyGridStyle();
            }
            catch (Exception ex)
            {
                dgvProducts.DataSource = null;
                MessageBox.Show("Unable to load products. " + ex.Message);
            }
        }

        private void CorrectSelectedProduct()
        {
            if (dgvProducts.CurrentRow == null || dgvProducts.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Please select a product.");
                return;
            }

            DataRowView rowView = dgvProducts.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null)
            {
                return;
            }

            DataRow row = rowView.Row;
            int productId = Convert.ToInt32(row["ProductId"]);
            HsnLookupRequest request = BuildLookupRequest(row);

            HsnSuggestion suggestion = hsnSuggestionService.GetLocalSuggestion(request);

            using (HsnCorrectionDialog dialog = new HsnCorrectionDialog(request, suggestion, hsnSuggestionService))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                if (UpdateHsn(productId, dialog.SelectedHsn))
                {
                    ProductMasterCloudQueue.EnqueueAndTryPush(Convert.ToString(productId), "HSN", true);
                    MessageBox.Show("HSN updated successfully. Branch sync queued.");
                    products.Rows.Remove(row);
                    LoadCounts();
                }
            }
        }

        private bool UpdateHsn(int productId, string hsn)
        {
            if (!HsnSuggestionService.IsValidHsn(hsn))
            {
                MessageBox.Show("Please enter a valid 4, 6 or 8 digit numeric HSN code.");
                return false;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(Conn))
                {
                    con.Open();
                    string updateSql = BuildUpdateSql(con);
                    using (SqlCommand cmd = new SqlCommand(updateSql, con))
                    {
                        cmd.Parameters.AddWithValue("@HSN", hsn.Trim());
                        cmd.Parameters.AddWithValue("@ProductId", productId);
                        cmd.Parameters.AddWithValue("@UserId", Convert.ToString(Program.userlevel));

                        int affected = cmd.ExecuteNonQuery();
                        if (affected == 0)
                        {
                            MessageBox.Show("Unable to update HSN. Product was not found.");
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to update HSN. " + ex.Message);
                return false;
            }
        }

        private string BuildUpdateSql(SqlConnection con)
        {
            string sql = "UPDATE ProductMaster SET HSN = @HSN";
            if (ColumnExists(con, "ProductMaster", "UpdatedOn"))
            {
                sql += ", UpdatedOn = GETDATE()";
            }
            else if (ColumnExists(con, "ProductMaster", "ModifiedOn"))
            {
                sql += ", ModifiedOn = GETDATE()";
            }

            if (ColumnExists(con, "ProductMaster", "UpdatedBy"))
            {
                sql += ", UpdatedBy = @UserId";
            }
            else if (ColumnExists(con, "ProductMaster", "ModifiedBy"))
            {
                sql += ", ModifiedBy = @UserId";
            }

            sql += " WHERE id = @ProductId";
            return sql;
        }

        private bool ColumnExists(SqlConnection con, string tableName, string columnName)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT COL_LENGTH(@tableName, @columnName)", con))
            {
                cmd.Parameters.AddWithValue("@tableName", "dbo." + tableName);
                cmd.Parameters.AddWithValue("@columnName", columnName);
                object value = cmd.ExecuteScalar();
                return value != null && value != DBNull.Value;
            }
        }

        private string GetProductsSql()
        {
            return @"
SELECT
    id AS ProductId,
    ISNULL(NULLIF(LTRIM(RTRIM(DisplayName)), ''), ItemName) AS ProductName,
    ISNULL(HSN, '') AS CurrentHsn,
    GST AS GstPercent,
    Brand AS Brand,
    Category AS Category,
    UOM AS Uom,
    CASE WHEN NULLIF(LTRIM(RTRIM(ISNULL(HSN, ''))), '') IS NULL THEN 'Empty HSN' ELSE 'Invalid HSN' END AS HsnStatus
FROM ProductMaster
WHERE (
        NULLIF(LTRIM(RTRIM(ISNULL(HSN, ''))), '') IS NULL
        OR LTRIM(RTRIM(ISNULL(HSN, ''))) LIKE '%[^0-9]%'
        OR LEN(LTRIM(RTRIM(ISNULL(HSN, '')))) NOT IN (4, 6, 8)
      )
  AND (@status = 'All'
       OR (@status = 'Empty HSN' AND NULLIF(LTRIM(RTRIM(ISNULL(HSN, ''))), '') IS NULL)
       OR (@status = 'Invalid HSN' AND NULLIF(LTRIM(RTRIM(ISNULL(HSN, ''))), '') IS NOT NULL
           AND (LTRIM(RTRIM(ISNULL(HSN, ''))) LIKE '%[^0-9]%'
                OR LEN(LTRIM(RTRIM(ISNULL(HSN, '')))) NOT IN (4, 6, 8))))
  AND (@search = '%%'
       OR ISNULL(ItemName, '') LIKE @search
       OR ISNULL(DisplayName, '') LIKE @search)
ORDER BY ISNULL(NULLIF(LTRIM(RTRIM(DisplayName)), ''), ItemName)";
        }

        private string GetCountsSql()
        {
            return @"
SELECT
    COUNT(1) AS TotalProducts,
    SUM(CASE WHEN NULLIF(LTRIM(RTRIM(ISNULL(HSN, ''))), '') IS NOT NULL
              AND LTRIM(RTRIM(ISNULL(HSN, ''))) NOT LIKE '%[^0-9]%'
              AND LEN(LTRIM(RTRIM(ISNULL(HSN, '')))) IN (4, 6, 8)
             THEN 1 ELSE 0 END) AS ValidHsn,
    SUM(CASE WHEN NULLIF(LTRIM(RTRIM(ISNULL(HSN, ''))), '') IS NULL THEN 1 ELSE 0 END) AS EmptyHsn,
    SUM(CASE WHEN NULLIF(LTRIM(RTRIM(ISNULL(HSN, ''))), '') IS NOT NULL
              AND (LTRIM(RTRIM(ISNULL(HSN, ''))) LIKE '%[^0-9]%'
                   OR LEN(LTRIM(RTRIM(ISNULL(HSN, '')))) NOT IN (4, 6, 8))
             THEN 1 ELSE 0 END) AS InvalidHsn,
    SUM(CASE WHEN NULLIF(LTRIM(RTRIM(ISNULL(HSN, ''))), '') IS NULL
              OR LTRIM(RTRIM(ISNULL(HSN, ''))) LIKE '%[^0-9]%'
              OR LEN(LTRIM(RTRIM(ISNULL(HSN, '')))) NOT IN (4, 6, 8)
             THEN 1 ELSE 0 END) AS PendingCorrection
FROM ProductMaster";
        }

        private void EnsureSuggestionColumns(DataTable table)
        {
            if (!table.Columns.Contains("SuggestedHsn"))
            {
                table.Columns.Add("SuggestedHsn", typeof(string));
            }
            if (!table.Columns.Contains("SuggestionStatus"))
            {
                table.Columns.Add("SuggestionStatus", typeof(string));
            }
            if (!table.Columns.Contains("SuggestionSource"))
            {
                table.Columns.Add("SuggestionSource", typeof(string));
            }
            if (!table.Columns.Contains("SuggestionCategory"))
            {
                table.Columns.Add("SuggestionCategory", typeof(string));
            }
            if (!table.Columns.Contains("SuggestionReason"))
            {
                table.Columns.Add("SuggestionReason", typeof(string));
            }
        }

        private void AddGridColumns()
        {
            dgvProducts.Columns.Add(CreateTextColumn("ProductId", "Product ID", "ProductId", 85));
            dgvProducts.Columns.Add(CreateTextColumn("ProductName", "Product Name", "ProductName", 360));
            dgvProducts.Columns.Add(CreateTextColumn("CurrentHsn", "Current HSN", "CurrentHsn", 100));
            dgvProducts.Columns.Add(CreateTextColumn("GstPercent", "GST %", "GstPercent", 70));
            dgvProducts.Columns.Add(CreateTextColumn("HsnStatus", "Status", "HsnStatus", 105));

            DataGridViewButtonColumn actionColumn = new DataGridViewButtonColumn();
            actionColumn.Name = "Action";
            actionColumn.HeaderText = "Action";
            actionColumn.Text = "Correct HSN";
            actionColumn.UseColumnTextForButtonValue = true;
            actionColumn.Width = 105;
            dgvProducts.Columns.Add(actionColumn);
        }

        private DataGridViewTextBoxColumn CreateTextColumn(string name, string headerText, string dataPropertyName, int width)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = headerText;
            column.DataPropertyName = dataPropertyName;
            column.Width = width;
            return column;
        }

        private HsnLookupRequest BuildLookupRequest(DataRow row)
        {
            HsnLookupRequest request = new HsnLookupRequest();
            request.ProductId = Convert.ToInt32(row["ProductId"]);
            request.ProductName = Convert.ToString(row["ProductName"]);
            request.CurrentHsn = Convert.ToString(row["CurrentHsn"]);
            request.GstPercent = Convert.ToString(row["GstPercent"]);
            request.Brand = Convert.ToString(row["Brand"]);
            request.Category = Convert.ToString(row["Category"]);
            request.Uom = Convert.ToString(row["Uom"]);
            return request;
        }

        private void ApplyGridStyle()
        {
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvProducts.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            foreach (DataGridViewColumn c in dgvProducts.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void ConfigureButton(Button button, string text, int x, int y, int width)
        {
            button.Text = text;
            button.SetBounds(x, y, width, 28);
            button.BackColor = Color.SteelBlue;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
        }

        private void ConfigureCountLabel(Label label)
        {
            label.AutoSize = true;
            label.Margin = new Padding(8, 3, 18, 3);
            label.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void btnCorrectHsn_Click(object sender, EventArgs e)
        {
            CorrectSelectedProduct();
        }

        private void dgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                CorrectSelectedProduct();
            }
        }

        private void dgvProducts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvProducts.Columns[e.ColumnIndex].Name == "Action")
            {
                CorrectSelectedProduct();
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadProducts();
                e.Handled = true;
            }
        }
    }
}
