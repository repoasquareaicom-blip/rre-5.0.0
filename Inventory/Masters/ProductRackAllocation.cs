using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Inventory.Masters
{
    public class ProductRackAllocation : Form
    {
        private readonly TextBox txtSearch = new TextBox();

        private readonly Button btnSearch = new Button();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnBulkUpdateRacks = new Button();

        private readonly Button btnPrevious = new Button();
        private readonly Button btnNext = new Button();

        private readonly Label lblSearch = new Label();
        private readonly Label lblStatus = new Label();
        private readonly Label lblPage = new Label();

        private readonly CheckBox chkRackAllocated = new CheckBox();
        private readonly CheckBox chkIncludeZeroStock = new CheckBox();

        private readonly DataGridView dgvProducts = new DataGridView();

        private readonly Panel pnlTop = new Panel();
        private readonly Panel pnlBottom = new Panel();

        // ================================================================
        // PAGINATION
        // ================================================================

        private int currentPage = 1;

        // Recommended for this screen.
        private const int PageSize = 100;

        private int totalRecords = 0;
        private int totalPages = 1;

        public ProductRackAllocation()
        {
            InitializeComponent();
        }

        // ================================================================
        // INITIALIZE FORM
        // ================================================================

        private void InitializeComponent()
        {
            Text = "Product Rack Allocation";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;

            Font = new Font(
                "Calibri",
                9.75F,
                FontStyle.Regular,
                GraphicsUnit.Point,
                ((byte)(0)));

            // ============================================================
            // TOP PANEL
            // ============================================================

            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 56;
            pnlTop.Padding = new Padding(8);
            pnlTop.BackColor = Color.WhiteSmoke;

            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(10, 15);
            lblSearch.Text = "Product Name";

            txtSearch.Location = new Point(105, 11);
            txtSearch.Width = 300;
            txtSearch.Anchor =
                AnchorStyles.Left |
                AnchorStyles.Top;

            txtSearch.KeyDown += txtSearch_KeyDown;

            // ============================================================
            // FILTER - RACK ALLOCATED
            // ============================================================

            chkRackAllocated.AutoSize = true;
            chkRackAllocated.Location = new Point(415, 14);
            chkRackAllocated.Text = "Rack Allocated Products";

            // Filter changes do NOT hit database immediately.
            chkRackAllocated.CheckedChanged += filter_Changed;

            // ============================================================
            // FILTER - ZERO STOCK
            // ============================================================

            chkIncludeZeroStock.AutoSize = true;
            chkIncludeZeroStock.Location = new Point(590, 14);
            chkIncludeZeroStock.Text = "Zero Stock Products";

            // Filter changes do NOT hit database immediately.
            chkIncludeZeroStock.CheckedChanged += filter_Changed;

            // ============================================================
            // SEARCH BUTTON
            // ============================================================

            btnSearch.Location = new Point(735, 10);
            btnSearch.Size = new Size(80, 27);
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;

            // ============================================================
            // REFRESH BUTTON
            // ============================================================

            btnRefresh.Location = new Point(821, 10);
            btnRefresh.Size = new Size(80, 27);
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;

            // ============================================================
            // BULK UPDATE RACKS BUTTON
            // ============================================================

            btnBulkUpdateRacks.Location = new Point(907, 10);
            btnBulkUpdateRacks.Size = new Size(150, 27);
            btnBulkUpdateRacks.Text = "Bulk Update Racks";
            btnBulkUpdateRacks.UseVisualStyleBackColor = true;
            btnBulkUpdateRacks.Enabled = false;
            btnBulkUpdateRacks.Click += btnBulkUpdateRacks_Click;

            // ============================================================
            // STATUS
            // ============================================================

            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(1068, 15);
            lblStatus.ForeColor = Color.DimGray;

            pnlTop.Controls.Add(lblSearch);
            pnlTop.Controls.Add(txtSearch);
            pnlTop.Controls.Add(chkRackAllocated);
            pnlTop.Controls.Add(chkIncludeZeroStock);
            pnlTop.Controls.Add(btnSearch);
            pnlTop.Controls.Add(btnRefresh);
            pnlTop.Controls.Add(btnBulkUpdateRacks);
            pnlTop.Controls.Add(lblStatus);

            // ============================================================
            // BOTTOM PAGINATION PANEL
            // ============================================================

            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 48;
            pnlBottom.Padding = new Padding(8);
            pnlBottom.BackColor = Color.WhiteSmoke;

            btnPrevious.Text = "< Previous";
            btnPrevious.Size = new Size(100, 28);
            btnPrevious.Location = new Point(10, 9);
            btnPrevious.UseVisualStyleBackColor = true;
            btnPrevious.Click += btnPrevious_Click;

            lblPage.AutoSize = true;
            lblPage.Location = new Point(125, 15);
            lblPage.Font = new Font(
                Font,
                FontStyle.Bold);

            lblPage.Text = "Page 1 of 1";

            btnNext.Text = "Next >";
            btnNext.Size = new Size(100, 28);
            btnNext.Location = new Point(250, 9);
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += btnNext_Click;

            pnlBottom.Controls.Add(btnPrevious);
            pnlBottom.Controls.Add(lblPage);
            pnlBottom.Controls.Add(btnNext);

            // ============================================================
            // PRODUCT GRID
            // ============================================================

            dgvProducts.Dock = DockStyle.Fill;

            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.AllowUserToDeleteRows = false;

            dgvProducts.AutoGenerateColumns = false;

            dgvProducts.BackgroundColor = Color.White;

            dgvProducts.RowHeadersVisible = false;
            dgvProducts.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(51, 122, 183);
            dgvProducts.DefaultCellStyle.SelectionForeColor =
                Color.White;

            dgvProducts.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvProducts.MultiSelect = true;

            // ============================================================
            // IMPORTANT PERFORMANCE CHANGE
            //
            // DO NOT USE:
            //
            // AutoSizeRowsMode.AllCells
            //
            // It becomes expensive with large datasets and wrapped text.
            // ============================================================

            dgvProducts.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.None;

            dgvProducts.RowTemplate.Height = 28;

            dgvProducts.CellBeginEdit +=
                dgvProducts_CellBeginEdit;

            dgvProducts.CellClick +=
                dgvProducts_CellClick;

            dgvProducts.CellEndEdit +=
                dgvProducts_CellEndEdit;

            dgvProducts.SelectionChanged +=
                dgvProducts_SelectionChanged;

            dgvProducts.DataError +=
                dgvProducts_DataError;

            // ============================================================
            // GRID COLUMNS
            // ============================================================

            AddTextColumn(
                "SNo",
                "SNo",
                55,
                true);

            AddTextColumn(
                "ProductName",
                "Product Name",
                310,
                true);

            AddTextColumn(
                "Category",
                "Category",
                130,
                true);

            AddTextColumn(
                "BrandPrice",
                "Brand Price",
                90,
                true);

            AddTextColumn(
                "UOM",
                "UOM",
                80,
                true);

            AddTextColumn(
                "InRacks",
                "In Racks",
                360,
                true);

            AddTextColumn(
                "AddRack",
                "Add Rack",
                260,
                false);

            // Internal hidden columns

            AddTextColumn(
                "ProductId",
                "Product ID",
                90,
                true);

            AddTextColumn(
                "CurrentStock",
                "Current Stock",
                110,
                true);

            dgvProducts.Columns["ProductId"].Visible = false;
            dgvProducts.Columns["CurrentStock"].Visible = false;

            // Improve scrolling/redraw.
            EnableDoubleBuffering(dgvProducts);

            // ============================================================
            // ADD CONTROLS
            // ============================================================

            Controls.Add(dgvProducts);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);

            Load += ProductRackAllocation_Load;
        }

        // ================================================================
        // ADD GRID COLUMN
        // ================================================================

        private void AddTextColumn(
            string dataPropertyName,
            string headerText,
            int width,
            bool readOnly)
        {
            DataGridViewTextBoxColumn column =
                new DataGridViewTextBoxColumn();

            column.DataPropertyName = dataPropertyName;
            column.Name = dataPropertyName;
            column.HeaderText = headerText;
            column.Width = width;
            column.ReadOnly = readOnly;

            column.SortMode =
                DataGridViewColumnSortMode.Automatic;

            // IMPORTANT:
            // No wrapping. This prevents expensive row-height calculation.
            column.DefaultCellStyle.WrapMode =
                DataGridViewTriState.False;

            if (dataPropertyName == "BrandPrice" ||
                dataPropertyName == "CurrentStock")
            {
                column.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }

            dgvProducts.Columns.Add(column);
        }

        // ================================================================
        // DOUBLE BUFFERING
        // ================================================================

        private static void EnableDoubleBuffering(
            DataGridView grid)
        {
            try
            {
                PropertyInfo property =
                    typeof(DataGridView).GetProperty(
                        "DoubleBuffered",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

                if (property != null)
                {
                    property.SetValue(
                        grid,
                        true,
                        null);
                }
            }
            catch
            {
                // Not critical.
                // Grid will still work without double buffering.
            }
        }

        // ================================================================
        // FORM LOAD
        // ================================================================

        private void ProductRackAllocation_Load(
            object sender,
            EventArgs e)
        {
            currentPage = 1;

            LoadProducts();
        }

        // ================================================================
        // SEARCH
        // ================================================================

        private void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            // Any new search starts from page 1.
            currentPage = 1;

            LoadProducts();
        }

        // ================================================================
        // REFRESH
        // ================================================================

        private void btnRefresh_Click(
            object sender,
            EventArgs e)
        {
            // Keep current page during Refresh.
            LoadProducts();
        }

        // ================================================================
        // FILTER CHANGED
        // ================================================================

        private void filter_Changed(
            object sender,
            EventArgs e)
        {
            // IMPORTANT:
            //
            // Do not automatically hit SQL every time a checkbox changes.
            //
            // User selects the required filters and clicks Search.

            currentPage = 1;

            lblStatus.Text =
                "Filter changed - click Search to apply.";
        }

        // ================================================================
        // SEARCH ON ENTER
        // ================================================================

        private void txtSearch_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                currentPage = 1;

                LoadProducts();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // ================================================================
        // PREVIOUS PAGE
        // ================================================================

        private void btnPrevious_Click(
            object sender,
            EventArgs e)
        {
            if (currentPage <= 1)
            {
                return;
            }

            currentPage--;

            LoadProducts();
        }

        // ================================================================
        // NEXT PAGE
        // ================================================================

        private void btnNext_Click(
            object sender,
            EventArgs e)
        {
            if (currentPage >= totalPages)
            {
                return;
            }

            currentPage++;

            LoadProducts();
        }

        // ================================================================
        // GRID EDITING
        // ================================================================

        private void dgvProducts_CellBeginEdit(
            object sender,
            DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            e.Cancel =
                dgvProducts.Columns[e.ColumnIndex].Name
                != "AddRack";
        }

        private void dgvProducts_DataError(
            object sender,
            DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvProducts_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.ColumnIndex < 0)
            {
                return;
            }

            if (dgvProducts.Columns[e.ColumnIndex].Name == "AddRack")
            {
                dgvProducts.CurrentCell =
                    dgvProducts.Rows[e.RowIndex].Cells[e.ColumnIndex];

                dgvProducts.BeginEdit(true);
            }
        }

        private void dgvProducts_SelectionChanged(
            object sender,
            EventArgs e)
        {
            UpdateBulkUpdateRacksButton();
        }

        // ================================================================
        // LOAD PRODUCTS - SERVER SIDE PAGINATION
        // ================================================================

        private void LoadProducts()
        {
            Cursor previousCursor =
                Cursor.Current;

            try
            {
                Cursor.Current =
                    Cursors.WaitCursor;

                btnSearch.Enabled = false;
                btnRefresh.Enabled = false;
                btnBulkUpdateRacks.Enabled = false;
                btnPrevious.Enabled = false;
                btnNext.Enabled = false;

                lblStatus.Text =
                    "Loading products...";

                lblStatus.Refresh();

                using (SqlConnection con =
                    new SqlConnection(
                        Program.connection))
                using (SqlCommand cmd =
                    new SqlCommand(
                        "sp_ProductRackAllocation_Search",
                        con))
                using (SqlDataAdapter ad =
                    new SqlDataAdapter(cmd))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.CommandTimeout = 60;

                    // Search text
                    cmd.Parameters.Add(
                        "@SearchText",
                        SqlDbType.NVarChar,
                        200).Value =
                        txtSearch.Text.Trim();

                    // Rack allocated filter
                    cmd.Parameters.Add(
                        "@RackAllocatedOnly",
                        SqlDbType.Bit).Value =
                        chkRackAllocated.Checked;

                    // Zero stock filter
                    cmd.Parameters.Add(
                        "@IncludeZeroStock",
                        SqlDbType.Bit).Value =
                        chkIncludeZeroStock.Checked;

                    // Pagination
                    cmd.Parameters.Add(
                        "@PageNumber",
                        SqlDbType.Int).Value =
                        currentPage;

                    cmd.Parameters.Add(
                        "@PageSize",
                        SqlDbType.Int).Value =
                        PageSize;

                    DataSet ds =
                        new DataSet();

                    ad.Fill(ds);

                    // ====================================================
                    // RESULT SET 1
                    //
                    // TotalRecords
                    // ====================================================

                    totalRecords = 0;

                    if (ds.Tables.Count > 0 &&
                        ds.Tables[0].Rows.Count > 0)
                    {
                        object value =
                            ds.Tables[0].Rows[0]["TotalRecords"];

                        if (value != DBNull.Value)
                        {
                            totalRecords =
                                Convert.ToInt32(value);
                        }
                    }

                    // ====================================================
                    // CALCULATE TOTAL PAGES
                    // ====================================================

                    if (totalRecords <= 0)
                    {
                        totalPages = 1;
                    }
                    else
                    {
                        totalPages =
                            (int)Math.Ceiling(
                                totalRecords /
                                (double)PageSize);
                    }

                    // Safety
                    if (currentPage > totalPages)
                    {
                        currentPage = totalPages;
                    }

                    // ====================================================
                    // RESULT SET 2
                    //
                    // Actual page rows
                    // ====================================================

                    DataTable dt;

                    if (ds.Tables.Count > 1)
                    {
                        dt = ds.Tables[1];
                    }
                    else
                    {
                        dt = new DataTable();
                    }

                    if (!dt.Columns.Contains("AddRack"))
                    {
                        dt.Columns.Add(
                            "AddRack",
                            typeof(string));
                    }

                    // ====================================================
                    // BIND ONLY 100 ROWS
                    // ====================================================

                    dgvProducts.SuspendLayout();

                    try
                    {
                        dgvProducts.DataSource = null;
                        dgvProducts.DataSource = dt;
                        dgvProducts.ClearSelection();
                    }
                    finally
                    {
                        dgvProducts.ResumeLayout();
                    }

                    UpdatePaginationStatus(
                        dt.Rows.Count);

                    UpdateBulkUpdateRacksButton();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Product rack allocation list could not be loaded."
                    + Environment.NewLine
                    + ex.Message,
                    "Product Rack Allocation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnSearch.Enabled = true;
                btnRefresh.Enabled = true;

                UpdatePaginationButtons();
                UpdateBulkUpdateRacksButton();

                Cursor.Current =
                    previousCursor;
            }
        }

        // ================================================================
        // UPDATE PAGINATION STATUS
        // ================================================================

        private void UpdatePaginationStatus(
            int rowsOnCurrentPage)
        {
            lblPage.Text =
                "Page "
                + currentPage
                + " of "
                + totalPages;

            if (totalRecords == 0)
            {
                lblStatus.Text =
                    "No products found.";

                return;
            }

            int startRecord =
                ((currentPage - 1) * PageSize) + 1;

            int endRecord =
                startRecord
                + rowsOnCurrentPage
                - 1;

            if (endRecord > totalRecords)
            {
                endRecord =
                    totalRecords;
            }

            lblStatus.Text =
                "Showing "
                + startRecord
                + "-"
                + endRecord
                + " of "
                + totalRecords
                + " product(s)";
        }

        // ================================================================
        // UPDATE PREVIOUS / NEXT
        // ================================================================

        private void UpdatePaginationButtons()
        {
            btnPrevious.Enabled =
                currentPage > 1;

            btnNext.Enabled =
                currentPage < totalPages &&
                totalRecords > 0;
        }

        // ================================================================
        // ADD RACK CELL END EDIT
        // ================================================================

        private void dgvProducts_CellEndEdit(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.ColumnIndex < 0)
            {
                return;
            }

            if (dgvProducts
                    .Columns[e.ColumnIndex]
                    .Name != "AddRack")
            {
                return;
            }

            SaveEditedRackCell(
                dgvProducts.Rows[e.RowIndex]);
        }

        // ================================================================
        // SAVE EDITED RACK CELL
        // ================================================================

        private void SaveEditedRackCell(
            DataGridViewRow row)
        {
            if (row == null ||
                row.IsNewRow)
            {
                return;
            }

            string addRack =
                Convert.ToString(
                    row.Cells["AddRack"].Value)
                    .Trim();

            if (string.IsNullOrEmpty(addRack))
            {
                return;
            }

            int productId;

            if (!int.TryParse(
                    Convert.ToString(
                        row.Cells["ProductId"].Value),
                    out productId)
                || productId <= 0)
            {
                MessageBox.Show(
                    "Invalid product row.",
                    "Product Rack Allocation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            Cursor previousCursor =
                Cursor.Current;

            try
            {
                Cursor.Current =
                    Cursors.WaitCursor;

                int insertedCount =
                    SaveProductRacks(
                        productId,
                        addRack);

                // ========================================================
                // IMPORTANT:
                //
                // Do NOT call LoadProducts() here.
                //
                // We only refresh rack names for this one product.
                // ========================================================

                string currentRacks =
                    GetProductRackCaptions(
                        productId);

                row.Cells["InRacks"].Value =
                     string.IsNullOrEmpty(currentRacks == null ? null : currentRacks.Trim())
                         ? "Not Allocated"
                        : currentRacks;

                row.Cells["AddRack"].Value =
                    "";

                if (insertedCount == 0)
                {
                    row.DefaultCellStyle.BackColor =
                        Color.FromArgb(
                            245,
                            245,
                            245);

                    lblStatus.Text =
                        "Rack already allocated.";
                }
                else
                {
                    row.DefaultCellStyle.BackColor =
                        Color.FromArgb(
                            226,
                            246,
                            226);

                    lblStatus.Text =
                        insertedCount.ToString()
                        + " rack(s) allocated.";
                }
            }
            catch (Exception ex)
            {
                row.DefaultCellStyle.BackColor =
                    Color.FromArgb(
                        255,
                        235,
                        235);

                // Keep AddRack text so user can correct invalid rack.
                MessageBox.Show(
                    GetFullErrorMessage(ex),
                    "Product Rack Allocation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor.Current =
                    previousCursor;
            }
        }

        // ================================================================
        // BULK UPDATE RACKS
        // ================================================================

        private void btnBulkUpdateRacks_Click(
            object sender,
            EventArgs e)
        {
            List<int> productIds =
                GetSelectedProductIds();

            if (productIds.Count == 0)
            {
                UpdateBulkUpdateRacksButton();
                return;
            }

            int updatedBy = 0;

            int.TryParse(
                Program.userid,
                out updatedBy);

            using (BulkRackAllocationDialog dialog =
                new BulkRackAllocationDialog(productIds, updatedBy))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                ProductRackBulkAllocationResult result =
                    dialog.AllocationResult;

                MessageBox.Show(
                    "Bulk rack allocation completed."
                    + Environment.NewLine
                    + "Selected products: "
                    + result.SelectedProductCount
                    + Environment.NewLine
                    + "New mappings created: "
                    + result.InsertedCount
                    + Environment.NewLine
                    + "Existing mappings skipped: "
                    + result.SkippedCount,
                    "Product Rack Allocation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadProducts();

                dgvProducts.ClearSelection();
                UpdateBulkUpdateRacksButton();
            }
        }

        private List<int> GetSelectedProductIds()
        {
            List<int> productIds =
                new List<int>();

            foreach (DataGridViewRow row in dgvProducts.SelectedRows)
            {
                if (row == null ||
                    row.IsNewRow)
                {
                    continue;
                }

                int productId;

                if (!int.TryParse(
                        Convert.ToString(
                            row.Cells["ProductId"].Value),
                        out productId)
                    || productId <= 0)
                {
                    continue;
                }

                if (!productIds.Contains(productId))
                {
                    productIds.Add(productId);
                }
            }

            return productIds;
        }

        private void UpdateBulkUpdateRacksButton()
        {
            int selectedCount =
                GetSelectedProductIds().Count;

            btnBulkUpdateRacks.Text =
                selectedCount > 0
                    ? "Bulk Update Racks (" + selectedCount + ")"
                    : "Bulk Update Racks";

            btnBulkUpdateRacks.Enabled =
                selectedCount > 0 &&
                btnSearch.Enabled &&
                btnRefresh.Enabled;
        }

        // ================================================================
        // SAVE RACKS
        // ================================================================

        private int SaveProductRacks(
            int productId,
            string rackCaptions)
        {
            int updatedBy = 0;

            int.TryParse(
                Program.userid,
                out updatedBy);

            using (SqlConnection con =
                new SqlConnection(
                    Program.connection))
            using (SqlCommand cmd =
                new SqlCommand(
                    "sp_ProductRackAllocation_AddRacks",
                    con))
            {
                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.CommandTimeout = 60;

                cmd.Parameters.Add(
                    "@ProductId",
                    SqlDbType.Int).Value =
                    productId;

                cmd.Parameters.Add(
                    "@RackCaptions",
                    SqlDbType.NVarChar,
                    -1).Value =
                    rackCaptions;

                cmd.Parameters.Add(
                    "@UpdatedBy",
                    SqlDbType.Int).Value =
                    updatedBy;

                SqlParameter inserted =
                    cmd.Parameters.Add(
                        "@InsertedCount",
                        SqlDbType.Int);

                inserted.Direction =
                    ParameterDirection.Output;

                con.Open();

                cmd.ExecuteNonQuery();

                if (inserted.Value == null ||
                    inserted.Value == DBNull.Value)
                {
                    return 0;
                }

                return Convert.ToInt32(
                    inserted.Value);
            }
        }

        // ================================================================
        // GET RACKS FOR ONE PRODUCT
        // ================================================================

        private string GetProductRackCaptions(
            int productId)
        {
            using (SqlConnection con =
                new SqlConnection(
                    Program.connection))
            using (SqlCommand cmd =
                new SqlCommand(
                    @"
                    SELECT
                        STUFF
                        (
                            (
                                SELECT
                                    ', ' + rm.RackCaption

                                FROM dbo.ProductRackMapping prm

                                INNER JOIN dbo.RackMaster rm
                                    ON rm.RackId = prm.RackId

                                LEFT JOIN dbo.LocationMaster lm
                                    ON lm.LocationId = rm.LocationId

                                WHERE
                                    prm.ProductId = @ProductId

                                ORDER BY
                                    ISNULL(
                                        lm.DisplayOrder,
                                        2147483647
                                    ),
                                    ISNULL(
                                        rm.DisplayOrder,
                                        2147483647
                                    ),
                                    rm.RackId

                                FOR XML PATH(''), TYPE

                            ).value(
                                '.',
                                'NVARCHAR(MAX)'
                            ),

                            1,
                            2,
                            ''
                        ) AS InRacks;",
                    con))
            {
                cmd.CommandType =
                    CommandType.Text;

                cmd.CommandTimeout = 30;

                cmd.Parameters.Add(
                    "@ProductId",
                    SqlDbType.Int).Value =
                    productId;

                con.Open();

                object result =
                    cmd.ExecuteScalar();

                if (result == null ||
                    result == DBNull.Value)
                {
                    return "";
                }

                return Convert.ToString(
                    result);
            }
        }

        // ================================================================
        // ERROR MESSAGE
        // ================================================================

        private static string GetFullErrorMessage(
            Exception ex)
        {
            string message = "";

            while (ex != null)
            {
                if (message.Length > 0)
                {
                    message +=
                        Environment.NewLine;
                }

                message +=
                    ex.Message;

                ex =
                    ex.InnerException;
            }

            return message;
        }
    }

    internal class BulkRackAllocationDialog : Form
    {
        private readonly List<int> productIds;
        private readonly int updatedBy;

        private readonly Label lblProductCount = new Label();
        private readonly Label lblRackNames = new Label();
        private readonly TextBox txtRackCaptions = new TextBox();
        private readonly Label lblError = new Label();
        private readonly Button btnAllocate = new Button();
        private readonly Button btnCancel = new Button();

        public BulkRackAllocationDialog(IList<int> selectedProductIds, int updatedBy)
        {
            if (selectedProductIds == null)
            {
                selectedProductIds =
                    new List<int>();
            }

            productIds =
                new List<int>(selectedProductIds);

            this.updatedBy =
                updatedBy;

            InitializeComponent(productIds.Count);
        }

        public ProductRackBulkAllocationResult AllocationResult { get; private set; }

        private void InitializeComponent(int selectedProductCount)
        {
            Text = "Bulk Update Racks";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(440, 178);
            Font = new Font("Calibri", 9.75F, FontStyle.Regular);

            lblProductCount.AutoSize = true;
            lblProductCount.Location = new Point(14, 14);
            lblProductCount.Text =
                "Selected products: " + selectedProductCount.ToString();

            lblRackNames.AutoSize = true;
            lblRackNames.Location = new Point(14, 48);
            lblRackNames.Text = "Rack names";

            txtRackCaptions.Location = new Point(105, 44);
            txtRackCaptions.Size = new Size(315, 23);
            txtRackCaptions.Anchor =
                AnchorStyles.Left |
                AnchorStyles.Top |
                AnchorStyles.Right;

            lblError.AutoSize = false;
            lblError.Location = new Point(105, 72);
            lblError.Size = new Size(315, 38);
            lblError.ForeColor = Color.Firebrick;
            lblError.Text = "";

            btnAllocate.Location = new Point(248, 132);
            btnAllocate.Size = new Size(82, 28);
            btnAllocate.Text = "Allocate";
            btnAllocate.UseVisualStyleBackColor = true;
            btnAllocate.Click += btnAllocate_Click;

            btnCancel.Location = new Point(338, 132);
            btnCancel.Size = new Size(82, 28);
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.DialogResult = DialogResult.Cancel;

            AcceptButton = btnAllocate;
            CancelButton = btnCancel;

            Controls.Add(lblProductCount);
            Controls.Add(lblRackNames);
            Controls.Add(txtRackCaptions);
            Controls.Add(lblError);
            Controls.Add(btnAllocate);
            Controls.Add(btnCancel);
        }

        private void btnAllocate_Click(
            object sender,
            EventArgs e)
        {
            lblError.Text = "";

            string rackCaptions =
                NormalizeRackCaptions(txtRackCaptions.Text);

            if (string.IsNullOrEmpty(rackCaptions))
            {
                ShowValidationError(
                    "Please enter at least one rack name.");

                return;
            }

            txtRackCaptions.Text =
                rackCaptions;

            Cursor previousCursor =
                Cursor.Current;

            try
            {
                Cursor.Current =
                    Cursors.WaitCursor;

                btnAllocate.Enabled = false;

                ProductRackMappingRepository repository =
                    new ProductRackMappingRepository();

                AllocationResult =
                    repository.BulkAddMappings(
                        productIds,
                        rackCaptions,
                        updatedBy);

                DialogResult =
                    DialogResult.OK;

                Close();
            }
            catch (Exception ex)
            {
                ShowValidationError(
                    GetFriendlyErrorMessage(ex));
            }
            finally
            {
                btnAllocate.Enabled = true;

                Cursor.Current =
                    previousCursor;
            }
        }

        private void ShowValidationError(string message)
        {
            lblError.Text =
                message;

            txtRackCaptions.Focus();
        }

        private static string NormalizeRackCaptions(string rackCaptions)
        {
            string[] parts =
                Convert.ToString(rackCaptions)
                    .Replace("+", ",")
                    .Split(',');

            List<string> uniqueRacks =
                new List<string>();

            foreach (string part in parts)
            {
                string rackCaption =
                    part.Trim();

                if (rackCaption.Length == 0)
                {
                    continue;
                }

                bool exists = false;

                foreach (string existingRack in uniqueRacks)
                {
                    if (string.Equals(
                            existingRack,
                            rackCaption,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    uniqueRacks.Add(rackCaption);
                }
            }

            return string.Join(
                ", ",
                uniqueRacks.ToArray());
        }

        private static string GetFriendlyErrorMessage(Exception ex)
        {
            string message =
                GetFullErrorMessage(ex);

            const string invalidPrefix =
                "Invalid Rack(s): ";

            int invalidIndex =
                message.IndexOf(
                    invalidPrefix,
                    StringComparison.OrdinalIgnoreCase);

            if (invalidIndex >= 0)
            {
                string invalidRacks =
                    message.Substring(invalidIndex + invalidPrefix.Length).Trim();

                if (invalidRacks.IndexOf(',') >= 0)
                {
                    return "Invalid rack names: "
                        + invalidRacks
                        + ". Please enter valid racks.";
                }

                return "Invalid rack name: "
                    + invalidRacks
                    + ". Please enter a valid rack.";
            }

            return message;
        }

        private static string GetFullErrorMessage(Exception ex)
        {
            string message = "";

            while (ex != null)
            {
                if (message.Length > 0)
                {
                    message +=
                        Environment.NewLine;
                }

                message +=
                    ex.Message;

                ex =
                    ex.InnerException;
            }

            return message;
        }
    }
}
