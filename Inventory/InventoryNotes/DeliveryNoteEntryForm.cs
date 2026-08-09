using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.InventoryNotes
{
    public class DeliveryNoteEntryForm : Form
    {
        private readonly string conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        private SplitContainer splitContainer1;
        private Panel pnlSearch;
        private Panel pnlSearchHeader;
        private Panel pnlTop;
        private Panel pnlHeader;
        private Panel pnlProduct;
        private Panel pnlBottom;
        private ComboBox cmbLocation;
        private DateTimePicker date;
        private TextBox txtorder;
        private TextBox txtReference;
        private TextBox txtRemarks;
        private ComboBox cmbstatus;
        private TextBox txtProductSearch;
        private TextBox txtQuantity;
        private DataGridView DgvAutoRefNo;
        private DataGridView dgvOrder;
        private DataGridView dgvSearch;
        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;
        private TextBox txtSearchLocation;
        private TextBox txtOrderNo;
        private TextBox txtSearchProduct;
        private TextBox textSearchQty;
        private Label lbltotalquantity;
        private Label lblTotalItems;
        private Label lblProductId;
        private Label lblProductName;
        private Label lblItemCode;
        private Label lblUom;
        private Label lblAvailableStock;
        private Button btnSave;
        private int currentDeliveryNoteId;
        private bool loading;
        private DataTable locations;

        public DeliveryNoteEntryForm()
        {
            InitializeComponent();
            LoadLocations();
            LoadPorts();
            ClearForm();
            SearchDeliveryNotes();
        }

        private void InitializeComponent()
        {
            Text = "Delivery Note";
            WindowState = FormWindowState.Maximized;
            BackColor = Color.WhiteSmoke;
            Font = new Font("Arial", 9F, FontStyle.Regular);
            KeyPreview = true;

            splitContainer1 = new SplitContainer();
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.SplitterDistance = 250;
            splitContainer1.Panel1MinSize = 200;
            splitContainer1.Panel2.AutoScroll = true;
            splitContainer1.Panel2.AutoScrollMinSize = new Size(700, 535);
            Controls.Add(splitContainer1);

            BuildSearchPanel();
            BuildEntryPanel();
        }

        private void BuildSearchPanel()
        {
            pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Fill;
            pnlSearch.BackColor = Color.FromArgb(255, 192, 192);
            splitContainer1.Panel1.Controls.Add(pnlSearch);

            pnlSearchHeader = new Panel();
            pnlSearchHeader.Dock = DockStyle.Top;
            pnlSearchHeader.Height = 29;
            pnlSearchHeader.BackColor = Color.DarkCyan;
            pnlSearch.Controls.Add(pnlSearchHeader);

            Label title = new Label();
            title.Text = "Search";
            title.Font = new Font("Arial", 10F, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.Location = new Point(7, 6);
            title.Size = new Size(100, 18);
            pnlSearchHeader.Controls.Add(title);

            AddSearchLabel("Location", 42);
            txtSearchLocation = AddSearchText(132, 39);
            AddSearchLabel("From Date", 68);
            dateTimePicker1 = AddSearchDate(132, 65);
            AddSearchLabel("To Date", 94);
            dateTimePicker2 = AddSearchDate(132, 91);
            AddSearchLabel("Delivery Note No", 120);
            txtOrderNo = AddSearchText(132, 117);
            AddSearchLabel("Product Name", 146);
            txtSearchProduct = AddSearchText(132, 143);
            AddSearchLabel("Qty", 172);
            textSearchQty = AddSearchText(132, 169);

            Button btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Location = new Point(412, 199);
            btnSearch.Size = new Size(74, 26);
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Click += delegate { SearchDeliveryNotes(); };
            pnlSearch.Controls.Add(btnSearch);

            dgvSearch = new DataGridView();
            dgvSearch.Location = new Point(5, 229);
            dgvSearch.Size = new Size(484, 490);
            dgvSearch.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvSearch.ReadOnly = true;
            dgvSearch.AllowUserToAddRows = false;
            dgvSearch.AllowUserToDeleteRows = false;
            dgvSearch.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSearch.MultiSelect = false;
            dgvSearch.RowHeadersVisible = false;
            dgvSearch.CellClick += dgvSearch_CellClick;
            dgvSearch.KeyDown += dgvSearch_KeyDown;
            ApplyGridStyle(dgvSearch);
            pnlSearch.Controls.Add(dgvSearch);
        }

        private void BuildEntryPanel()
        {
            pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 36;
            pnlTop.BackColor = Color.WhiteSmoke;
            pnlTop.BorderStyle = BorderStyle.FixedSingle;
            splitContainer1.Panel2.Controls.Add(pnlTop);

            Label title = new Label();
            title.Text = "Delivery Note";
            title.ForeColor = Color.SteelBlue;
            title.Font = new Font("Arial", 12F, FontStyle.Bold);
            title.Location = new Point(5, 7);
            title.Size = new Size(180, 20);
            pnlTop.Controls.Add(title);

            Button btnNew = AddTopButton("New", 910);
            btnNew.Click += delegate { ClearForm(); };
            btnSave = AddTopButton("Save", 990);
            btnSave.Click += btnSave_Click;
            Button btnClear = AddTopButton("Clear", 1070);
            btnClear.Click += delegate { ClearForm(); };

            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 154;
            pnlHeader.BackColor = Color.WhiteSmoke;
            splitContainer1.Panel2.Controls.Add(pnlHeader);
            pnlHeader.BringToFront();

            AddEntryLabel("* Location", 12, 82);
            cmbLocation = new ComboBox();
            cmbLocation.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbLocation.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbLocation.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLocation.FlatStyle = FlatStyle.Flat;
            cmbLocation.Font = new Font("Calibri", 9.75F);
            cmbLocation.Location = new Point(110, 78);
            cmbLocation.Size = new Size(260, 23);
            pnlHeader.Controls.Add(cmbLocation);

            AddEntryLabel("Reference", 12, 114);
            txtReference = AddEntryText(110, 110, 260, false);

            Panel right = new Panel();
            right.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            right.BorderStyle = BorderStyle.FixedSingle;
            right.Location = new Point(860, 66);
            right.Size = new Size(230, 86);
            pnlHeader.Controls.Add(right);

            AddRightLabel(right, "Delivery Note No", 10, 12);
            txtorder = new TextBox();
            txtorder.Enabled = false;
            txtorder.Location = new Point(98, 10);
            txtorder.Size = new Size(118, 21);
            right.Controls.Add(txtorder);

            AddRightLabel(right, "Date", 10, 38);
            date = new DateTimePicker();
            date.CustomFormat = "dd-MM-yyyy";
            date.Format = DateTimePickerFormat.Custom;
            date.Font = new Font("Calibri", 8F);
            date.Location = new Point(98, 35);
            date.Size = new Size(118, 20);
            right.Controls.Add(date);

            AddRightLabel(right, "Status", 10, 64);
            cmbstatus = new ComboBox();
            cmbstatus.Enabled = false;
            cmbstatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbstatus.Items.Add("PENDING");
            cmbstatus.Items.Add("APPROVED");
            cmbstatus.Items.Add("REJECTED");
            cmbstatus.Location = new Point(98, 60);
            cmbstatus.Size = new Size(118, 21);
            right.Controls.Add(cmbstatus);

            pnlProduct = new Panel();
            pnlProduct.Dock = DockStyle.Top;
            pnlProduct.Height = 82;
            pnlProduct.BackColor = Color.WhiteSmoke;
            splitContainer1.Panel2.Controls.Add(pnlProduct);
            pnlProduct.BringToFront();

            AddProductLabel("Product", 12, 10);
            txtProductSearch = new TextBox();
            txtProductSearch.Font = new Font("Arial", 11F);
            txtProductSearch.Location = new Point(110, 6);
            txtProductSearch.Size = new Size(520, 24);
            txtProductSearch.KeyUp += txtProductSearch_KeyUp;
            txtProductSearch.KeyDown += txtProductSearch_KeyDown;
            pnlProduct.Controls.Add(txtProductSearch);

            AddProductLabel("Qty", 646, 10);
            txtQuantity = new TextBox();
            txtQuantity.Font = new Font("Arial", 11F);
            txtQuantity.Location = new Point(682, 6);
            txtQuantity.Size = new Size(90, 24);
            txtQuantity.Text = "1";
            txtQuantity.KeyPress += NumericKeyPress;
            txtQuantity.KeyDown += txtQuantity_KeyDown;
            pnlProduct.Controls.Add(txtQuantity);

            Button btnAdd = new Button();
            btnAdd.Text = "Add";
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Location = new Point(786, 5);
            btnAdd.Size = new Size(68, 26);
            btnAdd.Click += delegate { AddSelectedProduct(); };
            pnlProduct.Controls.Add(btnAdd);

            txtRemarks = new TextBox();
            txtRemarks.Visible = false;
            pnlProduct.Controls.Add(txtRemarks);

            lblProductId = HiddenLabel();
            lblProductName = HiddenLabel();
            lblItemCode = HiddenLabel();
            lblUom = HiddenLabel();
            lblAvailableStock = HiddenLabel();
            pnlProduct.Controls.Add(lblProductId);
            pnlProduct.Controls.Add(lblProductName);
            pnlProduct.Controls.Add(lblItemCode);
            pnlProduct.Controls.Add(lblUom);
            pnlProduct.Controls.Add(lblAvailableStock);

            DgvAutoRefNo = new DataGridView();
            DgvAutoRefNo.Location = new Point(110, 31);
            DgvAutoRefNo.Size = new Size(520, 48);
            DgvAutoRefNo.ReadOnly = true;
            DgvAutoRefNo.AllowUserToAddRows = false;
            DgvAutoRefNo.AllowUserToDeleteRows = false;
            DgvAutoRefNo.RowHeadersVisible = false;
            DgvAutoRefNo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DgvAutoRefNo.MultiSelect = false;
            DgvAutoRefNo.Visible = false;
            DgvAutoRefNo.CellClick += DgvAutoRefNo_CellClick;
            DgvAutoRefNo.KeyDown += DgvAutoRefNo_KeyDown;
            ApplyAutoGridStyle(DgvAutoRefNo);
            pnlProduct.Controls.Add(DgvAutoRefNo);

            dgvOrder = new DataGridView();
            dgvOrder.Dock = DockStyle.Fill;
            dgvOrder.AllowUserToAddRows = false;
            dgvOrder.RowHeadersVisible = false;
            dgvOrder.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrder.MultiSelect = false;
            dgvOrder.CellEndEdit += dgvOrder_CellEndEdit;
            dgvOrder.EditingControlShowing += dgvOrder_EditingControlShowing;
            splitContainer1.Panel2.Controls.Add(dgvOrder);
            dgvOrder.BringToFront();

            pnlBottom = new Panel();
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 40;
            pnlBottom.BackColor = Color.WhiteSmoke;
            splitContainer1.Panel2.Controls.Add(pnlBottom);
            pnlBottom.BringToFront();

            Label help = new Label();
            help.Text = "Alt + Insert  --> Insert Rows        Alt + Delete  -->Delete  Rows";
            help.Font = new Font("Arial", 9F, FontStyle.Bold);
            help.ForeColor = Color.Red;
            help.Location = new Point(10, 13);
            help.Size = new Size(460, 18);
            pnlBottom.Controls.Add(help);

            lblTotalItems = new Label();
            lblTotalItems.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            lblTotalItems.Font = new Font("Arial", 12F, FontStyle.Bold);
            lblTotalItems.Location = new Point(745, 8);
            lblTotalItems.Size = new Size(150, 22);
            lblTotalItems.TextAlign = ContentAlignment.MiddleRight;
            pnlBottom.Controls.Add(lblTotalItems);

            lbltotalquantity = new Label();
            lbltotalquantity.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            lbltotalquantity.BorderStyle = BorderStyle.FixedSingle;
            lbltotalquantity.Font = new Font("Arial", 12F, FontStyle.Bold);
            lbltotalquantity.Location = new Point(900, 8);
            lbltotalquantity.Size = new Size(160, 24);
            lbltotalquantity.TextAlign = ContentAlignment.MiddleRight;
            pnlBottom.Controls.Add(lbltotalquantity);
        }

        private void LoadLocations()
        {
            locations = ExecuteTable(@"
SELECT LocationId, LocationCode, LocationName, LocationType,
       LocationName + CASE WHEN ISNULL(LocationCode,'') = '' THEN '' ELSE ' (' + LocationCode + ')' END AS DisplayName
FROM InventoryLocationMaster
WHERE ISNULL(IsActive, 1) = 1
ORDER BY LocationName", null);

            DataRow row = locations.NewRow();
            row["LocationId"] = 0;
            row["LocationCode"] = "";
            row["LocationName"] = "-Select-";
            row["LocationType"] = "";
            row["DisplayName"] = "-Select-";
            locations.Rows.InsertAt(row, 0);

            cmbLocation.DataSource = locations.Copy();
            cmbLocation.DisplayMember = "DisplayName";
            cmbLocation.ValueMember = "LocationId";
        }

        private void LoadPorts()
        {
            dgvOrder.Rows.Clear();
            dgvOrder.ColumnCount = 6;
            dgvOrder.Columns[0].Name = "S.NO";
            dgvOrder.Columns[1].Name = "Items";
            dgvOrder.Columns[2].Name = "UOM";
            dgvOrder.Columns[3].Name = "Available Stock";
            dgvOrder.Columns[4].Name = "Quantity";
            dgvOrder.Columns[5].Name = "productid";
            dgvOrder.Columns[5].Visible = false;

            dgvOrder.Columns["S.NO"].ReadOnly = true;
            dgvOrder.Columns["Items"].ReadOnly = true;
            dgvOrder.Columns["UOM"].ReadOnly = true;
            dgvOrder.Columns["Available Stock"].ReadOnly = true;
            dgvOrder.Columns["Available Stock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvOrder.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvOrder.Columns[0].Width = 90;
            dgvOrder.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvOrder.Columns[2].Width = 115;
            dgvOrder.Columns[3].Width = 125;
            dgvOrder.Columns[4].Width = 115;

            ApplyGridStyle(dgvOrder);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.Insert))
            {
                int rowindex = dgvOrder.CurrentRow == null ? dgvOrder.Rows.Count : dgvOrder.CurrentRow.Index;
                dgvOrder.Rows.Insert(rowindex, 1);
                getsino();
                return true;
            }

            if (keyData == (Keys.Alt | Keys.Delete))
            {
                DeleteCurrentRow();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ClearForm()
        {
            loading = true;
            currentDeliveryNoteId = 0;
            txtorder.Text = GenerateDeliveryNoteNo(DateTime.Today);
            date.Value = DateTime.Today;
            cmbstatus.SelectedIndex = 0;
            txtReference.Clear();
            txtRemarks.Clear();
            txtProductSearch.Clear();
            txtQuantity.Text = "1";
            ClearSelectedProduct();
            dgvOrder.Rows.Clear();
            btnSave.Enabled = true;
            dgvOrder.ReadOnly = false;
            loading = false;
            txtProductSearch.Focus();
            UpdateTotals();
        }

        private void txtProductSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Up || e.KeyData == Keys.Down || e.KeyData == Keys.Enter || e.KeyData == Keys.Escape)
                return;

            if (txtProductSearch.Text.Trim() == "")
            {
                DgvAutoRefNo.Visible = false;
                ClearSelectedProduct();
                return;
            }

            AutoCompleteLoad(txtProductSearch.Text.Trim());
        }

        private void txtProductSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && DgvAutoRefNo.Visible && DgvAutoRefNo.Rows.Count > 0)
            {
                DgvAutoRefNo.Focus();
                DgvAutoRefNo.CurrentCell = DgvAutoRefNo[0, 0];
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (DgvAutoRefNo.Visible && DgvAutoRefNo.Rows.Count > 0)
                    SelectProductFromAutoGrid(DgvAutoRefNo.CurrentRow == null ? 0 : DgvAutoRefNo.CurrentRow.Index);
                txtQuantity.Focus();
                txtQuantity.SelectAll();
                e.SuppressKeyPress = true;
            }
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddSelectedProduct();
                e.SuppressKeyPress = true;
            }
        }

        private void AutoCompleteLoad(string text)
        {
            DataTable st = SearchProducts(text, GetSourceLocationId());
            if (st.Rows.Count > 0)
            {
                DgvAutoRefNo.DataSource = st;
                DgvAutoRefNo.Columns["Productid"].Visible = false;
                DgvAutoRefNo.Columns["UOM"].Visible = false;
                DgvAutoRefNo.Columns["AvailableStock"].Visible = false;
                DgvAutoRefNo.Columns["ItemCode"].HeaderText = "Code";
                DgvAutoRefNo.Columns["DisplayName"].HeaderText = "Product Name";
                DgvAutoRefNo.Visible = true;
                SelectProductFromAutoGrid(0);
            }
            else
            {
                DgvAutoRefNo.Visible = false;
                ClearSelectedProduct();
            }
        }

        private void DgvAutoRefNo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectProductFromAutoGrid(e.RowIndex);
                txtQuantity.Focus();
                txtQuantity.SelectAll();
            }
        }

        private void DgvAutoRefNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && DgvAutoRefNo.CurrentRow != null)
            {
                SelectProductFromAutoGrid(DgvAutoRefNo.CurrentRow.Index);
                txtQuantity.Focus();
                txtQuantity.SelectAll();
                e.SuppressKeyPress = true;
            }
        }

        private void SelectProductFromAutoGrid(int rowIndex)
        {
            if (DgvAutoRefNo.Rows.Count == 0 || rowIndex < 0)
                return;

            DataGridViewRow row = DgvAutoRefNo.Rows[rowIndex];
            lblProductId.Text = Convert.ToString(row.Cells["Productid"].Value);
            lblItemCode.Text = Convert.ToString(row.Cells["ItemCode"].Value);
            lblProductName.Text = Convert.ToString(row.Cells["DisplayName"].Value);
            lblUom.Text = Convert.ToString(row.Cells["UOM"].Value);
            lblAvailableStock.Text = Convert.ToDecimal(row.Cells["AvailableStock"].Value).ToString("0.###");
        }

        private void AddSelectedProduct()
        {
            int productId;
            decimal qty;
            decimal available;

            if (!int.TryParse(lblProductId.Text, out productId) || productId <= 0)
            {
                MessageBox.Show("Please Enter Correct Product Name");
                txtProductSearch.Focus();
                return;
            }

            if (!decimal.TryParse(txtQuantity.Text, out qty) || qty <= 0)
            {
                MessageBox.Show("Please Enter Quantity");
                txtQuantity.Focus();
                return;
            }

            decimal.TryParse(lblAvailableStock.Text, out available);
            if (qty > available)
            {
                MessageBox.Show("Insufficient stock.\nAvailable quantity: " + available.ToString("0.###"));
                txtQuantity.Focus();
                txtQuantity.SelectAll();
                return;
            }

            int rowIndex = FindProductRow(productId);
            if (rowIndex < 0)
            {
                rowIndex = dgvOrder.Rows.Add();
                dgvOrder.Rows[rowIndex].Cells["S.NO"].Value = rowIndex + 1;
                dgvOrder.Rows[rowIndex].Cells["Items"].Value = lblProductName.Text.ToUpper();
                dgvOrder.Rows[rowIndex].Cells["UOM"].Value = lblUom.Text;
                dgvOrder.Rows[rowIndex].Cells["Available Stock"].Value = available.ToString("0.###");
                dgvOrder.Rows[rowIndex].Cells["productid"].Value = productId;
            }

            dgvOrder.Rows[rowIndex].Cells["Quantity"].Value = qty.ToString("0.###");
            getsino();
            UpdateTotals();
            DgvAutoRefNo.Visible = false;
            txtProductSearch.Clear();
            txtQuantity.Text = "1";
            ClearSelectedProduct();
            txtProductSearch.Focus();
        }

        private int FindProductRow(int productId)
        {
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                int id;
                if (int.TryParse(Convert.ToString(dgvOrder.Rows[i].Cells["productid"].Value), out id) && id == productId)
                    return i;
            }
            return -1;
        }

        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvOrder.Columns[e.ColumnIndex].Name != "Quantity")
                return;

            decimal qty;
            decimal available;
            decimal.TryParse(Convert.ToString(dgvOrder.Rows[e.RowIndex].Cells["Available Stock"].Value), out available);
            if (!decimal.TryParse(Convert.ToString(dgvOrder.Rows[e.RowIndex].Cells["Quantity"].Value), out qty) || qty <= 0)
            {
                MessageBox.Show("Please Enter Quantity");
                dgvOrder.Rows[e.RowIndex].Cells["Quantity"].Value = "";
                return;
            }

            if (qty > available)
            {
                MessageBox.Show("Insufficient stock.\nAvailable quantity: " + available.ToString("0.###"));
                dgvOrder.Rows[e.RowIndex].Cells["Quantity"].Value = "";
            }

            UpdateTotals();
        }

        private void dgvOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = e.Control as TextBox;
            if (tb != null)
            {
                tb.KeyPress -= NumericKeyPress;
                if (dgvOrder.CurrentCell != null && dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].Name == "Quantity")
                    tb.KeyPress += NumericKeyPress;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveDeliveryNote();
        }

        private void SaveDeliveryNote()
        {
            if (!ValidateBeforeSave())
                return;

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    bool hasSingleLocation = ColumnExists(con, tran, "DeliveryNote", "LocationId");
                    int deliveryNoteId = currentDeliveryNoteId;
                    string deliveryNo = txtorder.Text.Trim();

                    if (deliveryNoteId == 0)
                    {
                        deliveryNo = GenerateDeliveryNoteNo(con, tran, date.Value.Date);
                        txtorder.Text = deliveryNo;
                        deliveryNoteId = InsertHeader(con, tran, hasSingleLocation, deliveryNo);
                        currentDeliveryNoteId = deliveryNoteId;
                    }
                    else
                    {
                        EnsurePending(con, tran, deliveryNoteId);
                        UpdateHeader(con, tran, hasSingleLocation, deliveryNoteId);
                        ExecuteNonQuery(con, tran, "DELETE FROM DeliveryNoteDetail WHERE DeliveryNoteId = @DeliveryNoteId",
                            new SqlParameter("@DeliveryNoteId", deliveryNoteId));
                    }

                    for (int i = 0; i < dgvOrder.Rows.Count; i++)
                    {
                        if (IsEmptyGridRow(i))
                            continue;
                        ExecuteNonQuery(con, tran,
                            "INSERT INTO DeliveryNoteDetail (DeliveryNoteId, MaterialId, Quantity, Remarks) VALUES (@DeliveryNoteId, @MaterialId, @Quantity, NULL)",
                            new SqlParameter("@DeliveryNoteId", deliveryNoteId),
                            new SqlParameter("@MaterialId", Convert.ToInt32(dgvOrder.Rows[i].Cells["productid"].Value)),
                            new SqlParameter("@Quantity", Convert.ToDecimal(dgvOrder.Rows[i].Cells["Quantity"].Value)));
                    }

                    tran.Commit();
                    MessageBox.Show("Delivery Note saved successfully.");
                    SearchDeliveryNotes();
                    LoadDeliveryNote(currentDeliveryNoteId);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Unable to save Delivery Note: " + ex.Message);
                }
            }
        }

        private int InsertHeader(SqlConnection con, SqlTransaction tran, bool hasSingleLocation, string deliveryNo)
        {
            if (hasSingleLocation)
            {
                object id = ExecuteScalar(con, tran, @"
INSERT INTO DeliveryNote (DeliveryNoteNo, DeliveryNoteDate, LocationId, ReferenceNo, Remarks, Status, EnteredBy, EnteredOn, IsDeleted)
VALUES (@DeliveryNoteNo, @DeliveryNoteDate, @LocationId, @ReferenceNo, @Remarks, 'PENDING', @EnteredBy, GETDATE(), 0);
SELECT SCOPE_IDENTITY();",
                    new SqlParameter("@DeliveryNoteNo", deliveryNo),
                    new SqlParameter("@DeliveryNoteDate", date.Value.Date),
                    new SqlParameter("@LocationId", Convert.ToInt32(cmbLocation.SelectedValue)),
                    new SqlParameter("@ReferenceNo", EmptyToDbNull(txtReference.Text)),
                    new SqlParameter("@Remarks", EmptyToDbNull(txtRemarks.Text)),
                    new SqlParameter("@EnteredBy", Program.userid));
                return Convert.ToInt32(id);
            }

            object oldId = ExecuteScalar(con, tran, @"
INSERT INTO DeliveryNote (DeliveryNoteNo, DeliveryNoteDate, FromLocationId, ToLocationId, ReferenceNo, ReferenceDate, Remarks, Status, EnteredBy, EnteredOn, IsDeleted)
VALUES (@DeliveryNoteNo, @DeliveryNoteDate, @FromLocationId, @ToLocationId, @ReferenceNo, NULL, @Remarks, 'PENDING', @EnteredBy, GETDATE(), 0);
SELECT SCOPE_IDENTITY();",
                new SqlParameter("@DeliveryNoteNo", deliveryNo),
                new SqlParameter("@DeliveryNoteDate", date.Value.Date),
                new SqlParameter("@FromLocationId", GetSourceLocationId()),
                new SqlParameter("@ToLocationId", Convert.ToInt32(cmbLocation.SelectedValue)),
                new SqlParameter("@ReferenceNo", EmptyToDbNull(txtReference.Text)),
                new SqlParameter("@Remarks", EmptyToDbNull(txtRemarks.Text)),
                new SqlParameter("@EnteredBy", Program.userid));
            return Convert.ToInt32(oldId);
        }

        private void UpdateHeader(SqlConnection con, SqlTransaction tran, bool hasSingleLocation, int deliveryNoteId)
        {
            if (hasSingleLocation)
            {
                ExecuteNonQuery(con, tran, @"
UPDATE DeliveryNote
SET DeliveryNoteDate = @DeliveryNoteDate,
    LocationId = @LocationId,
    ReferenceNo = @ReferenceNo,
    Remarks = @Remarks,
    UpdatedBy = @UpdatedBy,
    UpdatedOn = GETDATE()
WHERE DeliveryNoteId = @DeliveryNoteId",
                    new SqlParameter("@DeliveryNoteDate", date.Value.Date),
                    new SqlParameter("@LocationId", Convert.ToInt32(cmbLocation.SelectedValue)),
                    new SqlParameter("@ReferenceNo", EmptyToDbNull(txtReference.Text)),
                    new SqlParameter("@Remarks", EmptyToDbNull(txtRemarks.Text)),
                    new SqlParameter("@UpdatedBy", Program.userid),
                    new SqlParameter("@DeliveryNoteId", deliveryNoteId));
                return;
            }

            ExecuteNonQuery(con, tran, @"
UPDATE DeliveryNote
SET DeliveryNoteDate = @DeliveryNoteDate,
    FromLocationId = @FromLocationId,
    ToLocationId = @ToLocationId,
    ReferenceNo = @ReferenceNo,
    Remarks = @Remarks
WHERE DeliveryNoteId = @DeliveryNoteId",
                new SqlParameter("@DeliveryNoteDate", date.Value.Date),
                new SqlParameter("@FromLocationId", GetSourceLocationId()),
                new SqlParameter("@ToLocationId", Convert.ToInt32(cmbLocation.SelectedValue)),
                new SqlParameter("@ReferenceNo", EmptyToDbNull(txtReference.Text)),
                new SqlParameter("@Remarks", EmptyToDbNull(txtRemarks.Text)),
                new SqlParameter("@DeliveryNoteId", deliveryNoteId));
        }

        private bool ValidateBeforeSave()
        {
            if (cmbLocation.SelectedValue == null || Convert.ToInt32(cmbLocation.SelectedValue) <= 0)
            {
                MessageBox.Show("Please Select Location");
                cmbLocation.Focus();
                return false;
            }

            if (!ColumnExists("DeliveryNote", "LocationId") && Convert.ToInt32(cmbLocation.SelectedValue) == GetSourceLocationId())
            {
                MessageBox.Show("Please select a destination Location different from current stock Location.");
                cmbLocation.Focus();
                return false;
            }

            int validRows = 0;
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                if (IsEmptyGridRow(i))
                    continue;

                int productId;
                decimal qty;
                decimal available = GetAvailableStock(Convert.ToInt32(dgvOrder.Rows[i].Cells["productid"].Value), GetSourceLocationId());
                dgvOrder.Rows[i].Cells["Available Stock"].Value = available.ToString("0.###");

                if (!int.TryParse(Convert.ToString(dgvOrder.Rows[i].Cells["productid"].Value), out productId) || productId <= 0)
                {
                    MessageBox.Show("Please Enter Correct Product Name");
                    return false;
                }

                if (!decimal.TryParse(Convert.ToString(dgvOrder.Rows[i].Cells["Quantity"].Value), out qty) || qty <= 0)
                {
                    MessageBox.Show("Please Enter Quantity");
                    return false;
                }

                if (qty > available)
                {
                    MessageBox.Show("Insufficient stock.\nAvailable quantity: " + available.ToString("0.###"));
                    return false;
                }

                validRows++;
            }

            if (validRows == 0)
            {
                MessageBox.Show("Please Enter Product Details");
                txtProductSearch.Focus();
                return false;
            }

            return true;
        }

        private void SearchDeliveryNotes()
        {
            List<SqlParameter> p = new List<SqlParameter>();
            p.Add(new SqlParameter("@FromDate", dateTimePicker1.Value.Date));
            p.Add(new SqlParameter("@ToDate", dateTimePicker2.Value.Date.AddDays(1)));
            p.Add(new SqlParameter("@NoteNo", "%" + txtOrderNo.Text.Trim() + "%"));
            p.Add(new SqlParameter("@Location", "%" + txtSearchLocation.Text.Trim() + "%"));
            p.Add(new SqlParameter("@Product", "%" + txtSearchProduct.Text.Trim() + "%"));
            p.Add(new SqlParameter("@Qty", textSearchQty.Text.Trim()));

            string locationJoin = ColumnExists("DeliveryNote", "LocationId")
                ? "INNER JOIN InventoryLocationMaster l ON l.LocationId = h.LocationId"
                : "INNER JOIN InventoryLocationMaster l ON l.LocationId = h.ToLocationId";

            string sql = @"
SELECT h.DeliveryNoteId, h.DeliveryNoteNo AS [Delivery Note No], CONVERT(varchar(10), h.DeliveryNoteDate, 105) AS [Date],
       l.LocationName AS [Location], h.Status
FROM DeliveryNote h
" + locationJoin + @"
WHERE ISNULL(h.IsDeleted, 0) = 0
  AND h.DeliveryNoteDate >= @FromDate
  AND h.DeliveryNoteDate < @ToDate
  AND h.DeliveryNoteNo LIKE @NoteNo
  AND l.LocationName LIKE @Location
  AND (@Product = '%%' OR EXISTS
      (SELECT 1 FROM DeliveryNoteDetail d INNER JOIN ProductMaster p ON p.id = d.MaterialId
       WHERE d.DeliveryNoteId = h.DeliveryNoteId
         AND ISNULL(NULLIF(p.DisplayName, ''), p.ItemName) LIKE @Product))
  AND (@Qty = '' OR EXISTS
      (SELECT 1 FROM DeliveryNoteDetail d WHERE d.DeliveryNoteId = h.DeliveryNoteId AND CONVERT(varchar(50), d.Quantity) = @Qty))
ORDER BY h.DeliveryNoteDate DESC, h.DeliveryNoteId DESC";

            DataTable dt = ExecuteTable(sql, p);
            dgvSearch.DataSource = dt;
            if (dgvSearch.Columns.Contains("DeliveryNoteId"))
                dgvSearch.Columns["DeliveryNoteId"].Visible = false;
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvSearch.Rows[e.RowIndex].Cells["DeliveryNoteId"].Value != null)
                LoadDeliveryNote(Convert.ToInt32(dgvSearch.Rows[e.RowIndex].Cells["DeliveryNoteId"].Value));
        }

        private void dgvSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dgvSearch.CurrentRow != null)
            {
                LoadDeliveryNote(Convert.ToInt32(dgvSearch.CurrentRow.Cells["DeliveryNoteId"].Value));
                e.SuppressKeyPress = true;
            }
        }

        private void LoadDeliveryNote(int deliveryNoteId)
        {
            bool hasSingleLocation = ColumnExists("DeliveryNote", "LocationId");
            string locCol = hasSingleLocation ? "h.LocationId" : "h.ToLocationId";
            DataTable header = ExecuteTable(@"
SELECT h.DeliveryNoteId, h.DeliveryNoteNo, h.DeliveryNoteDate, " + locCol + @" AS LocationId,
       h.ReferenceNo, h.Remarks, h.Status
FROM DeliveryNote h
WHERE h.DeliveryNoteId = @DeliveryNoteId",
                new SqlParameter("@DeliveryNoteId", deliveryNoteId));
            if (header.Rows.Count == 0)
                return;

            loading = true;
            currentDeliveryNoteId = deliveryNoteId;
            txtorder.Text = Convert.ToString(header.Rows[0]["DeliveryNoteNo"]);
            date.Value = Convert.ToDateTime(header.Rows[0]["DeliveryNoteDate"]);
            cmbLocation.SelectedValue = Convert.ToInt32(header.Rows[0]["LocationId"]);
            txtReference.Text = Convert.ToString(header.Rows[0]["ReferenceNo"]);
            txtRemarks.Text = Convert.ToString(header.Rows[0]["Remarks"]);
            cmbstatus.Text = Convert.ToString(header.Rows[0]["Status"]);
            dgvOrder.Rows.Clear();

            DataTable detail = ExecuteTable(@"
SELECT d.MaterialId AS Productid,
       ISNULL(NULLIF(p.DisplayName, ''), p.ItemName) AS DisplayName,
       ISNULL(p.UOM, '') AS UOM,
       d.Quantity
FROM DeliveryNoteDetail d
INNER JOIN ProductMaster p ON p.id = d.MaterialId
WHERE d.DeliveryNoteId = @DeliveryNoteId
ORDER BY d.DeliveryNoteDetailId",
                new SqlParameter("@DeliveryNoteId", deliveryNoteId));

            for (int i = 0; i < detail.Rows.Count; i++)
            {
                int row = dgvOrder.Rows.Add();
                int materialId = Convert.ToInt32(detail.Rows[i]["Productid"]);
                dgvOrder.Rows[row].Cells["S.NO"].Value = row + 1;
                dgvOrder.Rows[row].Cells["Items"].Value = Convert.ToString(detail.Rows[i]["DisplayName"]).ToUpper();
                dgvOrder.Rows[row].Cells["UOM"].Value = Convert.ToString(detail.Rows[i]["UOM"]);
                dgvOrder.Rows[row].Cells["Available Stock"].Value = GetAvailableStock(materialId, GetSourceLocationId()).ToString("0.###");
                dgvOrder.Rows[row].Cells["Quantity"].Value = Convert.ToDecimal(detail.Rows[i]["Quantity"]).ToString("0.###");
                dgvOrder.Rows[row].Cells["productid"].Value = materialId;
            }

            bool pending = cmbstatus.Text.ToUpper() == "PENDING";
            btnSave.Enabled = pending;
            dgvOrder.ReadOnly = !pending;
            if (pending)
                dgvOrder.Columns["Quantity"].ReadOnly = false;
            loading = false;
            UpdateTotals();
        }

        private DataTable SearchProducts(string text, int sourceLocationId)
        {
            List<SqlParameter> p = new List<SqlParameter>();
            p.Add(new SqlParameter("@Search", "%" + text + "%"));
            p.Add(new SqlParameter("@LocationId", sourceLocationId));

            string stockSql = TableExists("MaterialTransaction")
                ? @"CAST(ISNULL((SELECT SUM(CASE WHEN mt.Type = 'IN' THEN mt.Quantity WHEN mt.Type = 'OUT' THEN -mt.Quantity ELSE 0 END)
                     FROM MaterialTransaction mt WHERE mt.MaterialId = p.id AND mt.LocationId = @LocationId), 0) AS decimal(18,3))"
                : "CAST(0 AS decimal(18,3))";

            string sql = @"
SELECT TOP 75
       p.id AS Productid,
       ISNULL(p.ItemCode, '') AS ItemCode,
       ISNULL(NULLIF(p.DisplayName, ''), p.ItemName) AS DisplayName,
       ISNULL(p.UOM, '') AS UOM,
       " + stockSql + @" AS AvailableStock
FROM ProductMaster p
WHERE ISNULL(p.IsDeleted, 0) = 0
  AND (ISNULL(p.ItemCode, '') LIKE @Search
       OR ISNULL(p.DisplayName, '') LIKE @Search
       OR ISNULL(p.ItemName, '') LIKE @Search
       OR ISNULL(p.BarCode, '') LIKE @Search)
ORDER BY ISNULL(NULLIF(p.DisplayName, ''), p.ItemName)";
            return ExecuteTable(sql, p);
        }

        private decimal GetAvailableStock(int materialId, int locationId)
        {
            if (!TableExists("MaterialTransaction"))
                return 0;

            object value = ExecuteScalar(@"
SELECT CAST(ISNULL(SUM(CASE WHEN Type = 'IN' THEN Quantity WHEN Type = 'OUT' THEN -Quantity ELSE 0 END), 0) AS decimal(18,3))
FROM MaterialTransaction
WHERE MaterialId = @MaterialId AND LocationId = @LocationId",
                new SqlParameter("@MaterialId", materialId),
                new SqlParameter("@LocationId", locationId));
            return value == null || value == DBNull.Value ? 0 : Convert.ToDecimal(value);
        }

        private int GetSourceLocationId()
        {
            if (locations == null || locations.Rows.Count <= 1)
                return 0;

            string branchCode = ConfigurationManager.AppSettings["BranchCode"];
            if (!string.IsNullOrEmpty(branchCode))
            {
                for (int i = 0; i < locations.Rows.Count; i++)
                {
                    if (string.Equals(Convert.ToString(locations.Rows[i]["LocationCode"]), branchCode, StringComparison.OrdinalIgnoreCase))
                        return Convert.ToInt32(locations.Rows[i]["LocationId"]);
                }
            }

            int shop;
            if (int.TryParse(Program.ShopName, out shop) && shop > 0)
                return shop;

            return Convert.ToInt32(locations.Rows[1]["LocationId"]);
        }

        private string GenerateDeliveryNoteNo(DateTime noteDate)
        {
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                return GenerateDeliveryNoteNo(con, null, noteDate);
            }
        }

        private string GenerateDeliveryNoteNo(SqlConnection con, SqlTransaction tran, DateTime noteDate)
        {
            int startYear = noteDate.Month >= 4 ? noteDate.Year : noteDate.Year - 1;
            string fy = startYear.ToString() + "-" + (startYear + 1).ToString().Substring(2, 2);
            string prefix = "DN/" + fy + "/";
            object max = ExecuteScalar(con, tran,
                "SELECT ISNULL(MAX(CAST(RIGHT(DeliveryNoteNo, 6) AS int)), 0) FROM DeliveryNote WITH (UPDLOCK, HOLDLOCK) WHERE DeliveryNoteNo LIKE @Prefix",
                new SqlParameter("@Prefix", prefix + "%"));
            return prefix + (Convert.ToInt32(max) + 1).ToString("000000");
        }

        private void EnsurePending(SqlConnection con, SqlTransaction tran, int deliveryNoteId)
        {
            object status = ExecuteScalar(con, tran, "SELECT Status FROM DeliveryNote WHERE DeliveryNoteId = @DeliveryNoteId",
                new SqlParameter("@DeliveryNoteId", deliveryNoteId));
            if (status == null || Convert.ToString(status).ToUpper() != "PENDING")
                throw new ApplicationException("Approved Delivery Note cannot be edited.");
        }

        private bool IsEmptyGridRow(int rowIndex)
        {
            return string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[rowIndex].Cells["Items"].Value));
        }

        private void DeleteCurrentRow()
        {
            if (dgvOrder.CurrentRow == null)
                return;
            DialogResult result = MessageBox.Show("Do you want to Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                dgvOrder.Rows.RemoveAt(dgvOrder.CurrentRow.Index);
                getsino();
                UpdateTotals();
            }
        }

        private void getsino()
        {
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
                dgvOrder.Rows[i].Cells["S.NO"].Value = i + 1;
        }

        private void UpdateTotals()
        {
            decimal totalQty = 0;
            int items = 0;
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                if (IsEmptyGridRow(i))
                    continue;
                decimal qty;
                decimal.TryParse(Convert.ToString(dgvOrder.Rows[i].Cells["Quantity"].Value), out qty);
                totalQty += qty;
                items++;
            }
            lblTotalItems.Text = "Total Items : " + items.ToString();
            lbltotalquantity.Text = "Total Qty : " + totalQty.ToString("0.###");
        }

        private void ClearSelectedProduct()
        {
            lblProductId.Text = "";
            lblProductName.Text = "";
            lblItemCode.Text = "";
            lblUom.Text = "";
            lblAvailableStock.Text = "";
        }

        private static object EmptyToDbNull(string value)
        {
            return string.IsNullOrEmpty(value == null ? "" : value.Trim()) ? (object)DBNull.Value : value.Trim();
        }

        private bool TableExists(string tableName)
        {
            object value = ExecuteScalar("SELECT COUNT(1) FROM sys.tables WHERE name = @Name", new SqlParameter("@Name", tableName));
            return Convert.ToInt32(value) > 0;
        }

        private bool ColumnExists(string tableName, string columnName)
        {
            object value = ExecuteScalar("SELECT COUNT(1) FROM sys.columns WHERE object_id = OBJECT_ID(@TableName) AND name = @ColumnName",
                new SqlParameter("@TableName", "dbo." + tableName),
                new SqlParameter("@ColumnName", columnName));
            return Convert.ToInt32(value) > 0;
        }

        private bool ColumnExists(SqlConnection con, SqlTransaction tran, string tableName, string columnName)
        {
            object value = ExecuteScalar(con, tran, "SELECT COUNT(1) FROM sys.columns WHERE object_id = OBJECT_ID(@TableName) AND name = @ColumnName",
                new SqlParameter("@TableName", "dbo." + tableName),
                new SqlParameter("@ColumnName", columnName));
            return Convert.ToInt32(value) > 0;
        }

        private DataTable ExecuteTable(string sql, IEnumerable<SqlParameter> parameters)
        {
            using (SqlConnection con = new SqlConnection(conn))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                if (parameters != null)
                    foreach (SqlParameter parameter in parameters)
                        cmd.Parameters.Add(parameter);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        private DataTable ExecuteTable(string sql, params SqlParameter[] parameters)
        {
            return ExecuteTable(sql, (IEnumerable<SqlParameter>)parameters);
        }

        private object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(conn))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                con.Open();
                return cmd.ExecuteScalar();
            }
        }

        private object ExecuteScalar(SqlConnection con, SqlTransaction tran, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = new SqlCommand(sql, con, tran))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }

        private void ExecuteNonQuery(SqlConnection con, SqlTransaction tran, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = new SqlCommand(sql, con, tran))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
            }
        }

        private void NumericKeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if (textBox != null && e.KeyChar == '.' && textBox.Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void ApplyGridStyle(DataGridView grid)
        {
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            grid.DefaultCellStyle.BackColor = Color.Gainsboro;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            grid.BackgroundColor = Color.DarkGray;
            grid.GridColor = SystemColors.ActiveBorder;
            grid.RowTemplate.Height = 22;
            foreach (DataGridViewColumn c in grid.Columns)
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
        }

        private void ApplyAutoGridStyle(DataGridView grid)
        {
            ApplyGridStyle(grid);
            grid.DefaultCellStyle.Font = new Font("Arial", 12F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10F, FontStyle.Bold);
        }

        private Label HiddenLabel()
        {
            Label label = new Label();
            label.Visible = false;
            label.Text = "";
            return label;
        }

        private void AddSearchLabel(string text, int top)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(14, top);
            label.Size = new Size(112, 18);
            label.Font = new Font("Arial", 9F);
            pnlSearch.Controls.Add(label);
        }

        private TextBox AddSearchText(int left, int top)
        {
            TextBox text = new TextBox();
            text.Location = new Point(left, top);
            text.Size = new Size(356, 20);
            pnlSearch.Controls.Add(text);
            return text;
        }

        private DateTimePicker AddSearchDate(int left, int top)
        {
            DateTimePicker picker = new DateTimePicker();
            picker.CustomFormat = "dd/MM/yyyy";
            picker.Format = DateTimePickerFormat.Custom;
            picker.Location = new Point(left, top);
            picker.Size = new Size(356, 20);
            pnlSearch.Controls.Add(picker);
            return picker;
        }

        private void AddEntryLabel(string text, int left, int top)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Arial", 9F);
            label.Location = new Point(left, top);
            label.Size = new Size(95, 18);
            pnlHeader.Controls.Add(label);
        }

        private TextBox AddEntryText(int left, int top, int width, bool readOnly)
        {
            TextBox text = new TextBox();
            text.Location = new Point(left, top);
            text.Size = new Size(width, 21);
            text.ReadOnly = readOnly;
            pnlHeader.Controls.Add(text);
            return text;
        }

        private void AddRightLabel(Control parent, string text, int left, int top)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(left, top);
            label.Size = new Size(86, 18);
            parent.Controls.Add(label);
        }

        private void AddProductLabel(string text, int left, int top)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Arial", 9F);
            label.Location = new Point(left, top);
            label.Size = new Size(90, 18);
            pnlProduct.Controls.Add(label);
        }

        private Button AddTopButton(string text, int left)
        {
            Button button = new Button();
            button.Text = text;
            button.FlatStyle = FlatStyle.Flat;
            button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button.Location = new Point(left, 5);
            button.Size = new Size(75, 25);
            pnlTop.Controls.Add(button);
            return button;
        }
    }
}
