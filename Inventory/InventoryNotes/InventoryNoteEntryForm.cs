using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Inventory.InventoryNotes
{
    public class InventoryNoteEntryForm : Form
    {
        private readonly string noteKind;
        private readonly InventoryNoteService service = new InventoryNoteService();
        private ComboBox cmbFromLocation;
        private ComboBox cmbToLocation;
        private DateTimePicker dtpNoteDate;
        private DateTimePicker dtpReferenceDate;
        private CheckBox chkReferenceDate;
        private TextBox txtNoteNo;
        private TextBox txtReferenceNo;
        private TextBox txtRemarks;
        private TextBox txtProductSearch;
        private TextBox txtQuantity;
        private Label lblSelectedProduct;
        private Label lblEntryStatus;
        private DataGridView dgvExistingNotes;
        private DataGridView dgvProductSearch;
        private DataGridView dgvDetails;
        private TextBox editingTextBox;
        private DataRow selectedProduct;
        private int viewingNoteId;

        public InventoryNoteEntryForm(string noteKind)
        {
            this.noteKind = noteKind;
            Initialize();
            LoadLocations();
            LoadDetailGrid();
            LoadExistingNotes();
        }

        private string NoteTitle
        {
            get { return noteKind == "DN" ? "Delivery Note Entry" : "Receipt Note Entry"; }
        }

        private void Initialize()
        {
            Text = NoteTitle;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.WhiteSmoke;
            Font = new Font("Arial", 9F, FontStyle.Regular);

            Controls.Add(CreateHeaderPanel(NoteTitle));

            Panel formPanel = new Panel();
            formPanel.Dock = DockStyle.Top;
            formPanel.Height = 122;
            formPanel.BackColor = Color.WhiteSmoke;
            formPanel.BorderStyle = BorderStyle.Fixed3D;
            Controls.Add(formPanel);

            txtNoteNo = AddText(formPanel, "No:", 18, 18, 145, true);
            dtpNoteDate = AddDate(formPanel, "Date:", 330, 18);
            cmbFromLocation = AddCombo(formPanel, "From Location:", 18, 50, 145, 175);
            cmbFromLocation.SelectedIndexChanged += delegate { RefreshProductSearch(); };
            cmbToLocation = AddCombo(formPanel, "To Location:", 330, 50, 430, 175);
            cmbToLocation.SelectedIndexChanged += delegate { RefreshProductSearch(); };
            txtReferenceNo = AddText(formPanel, "Reference No:", 18, 82, 145, false);

            chkReferenceDate = new CheckBox();
            chkReferenceDate.Text = "Reference Date";
            chkReferenceDate.Font = new Font("Arial", 9F, FontStyle.Bold);
            chkReferenceDate.Location = new Point(330, 84);
            chkReferenceDate.Size = new Size(112, 18);
            formPanel.Controls.Add(chkReferenceDate);

            dtpReferenceDate = new DateTimePicker();
            dtpReferenceDate.CustomFormat = "dd-MM-yyyy";
            dtpReferenceDate.Format = DateTimePickerFormat.Custom;
            dtpReferenceDate.Font = new Font("Calibri", 8F);
            dtpReferenceDate.Location = new Point(445, 82);
            dtpReferenceDate.Size = new Size(110, 21);
            formPanel.Controls.Add(dtpReferenceDate);

            txtRemarks = AddText(formPanel, "Remarks:", 586, 82, 655, false);
            txtRemarks.Width = 360;

            Button btnNew = AddButton(formPanel, "New", 980, 16);
            btnNew.Click += delegate { ClearForm(); };
            Button btnClose = AddButton(formPanel, "Close", 1062, 16);
            btnClose.Click += delegate { Close(); };
            Button btnSave = AddButton(formPanel, "Save", 980, 48);
            btnSave.Click += delegate { Save(); };
            Button btnClear = AddButton(formPanel, "Clear", 1062, 48);
            btnClear.Click += delegate { ClearForm(); };

            lblEntryStatus = new Label();
            lblEntryStatus.Font = new Font("Arial", 9F, FontStyle.Bold);
            lblEntryStatus.ForeColor = Color.DarkGreen;
            lblEntryStatus.BackColor = Color.Transparent;
            lblEntryStatus.Location = new Point(586, 52);
            lblEntryStatus.Size = new Size(360, 18);
            lblEntryStatus.Text = "New Entry";
            formPanel.Controls.Add(lblEntryStatus);

            Panel addPanel = new Panel();
            addPanel.Dock = DockStyle.Top;
            addPanel.Height = 162;
            addPanel.BackColor = Color.WhiteSmoke;
            addPanel.BorderStyle = BorderStyle.Fixed3D;
            Controls.Add(addPanel);

            Panel addHeader = CreateHeaderPanel("Product Search");
            addHeader.Dock = DockStyle.Top;
            addPanel.Controls.Add(addHeader);

            AddLabel(addPanel, "Product:", 18, 42, 65);
            txtProductSearch = new TextBox();
            txtProductSearch.Font = new Font("Arial", 11F, FontStyle.Regular);
            txtProductSearch.Location = new Point(88, 40);
            txtProductSearch.Size = new Size(545, 24);
            txtProductSearch.TextChanged += delegate { RefreshProductSearch(); };
            txtProductSearch.KeyDown += ProductSearchKeyDown;
            addPanel.Controls.Add(txtProductSearch);

            AddLabel(addPanel, "Qty:", 650, 42, 35);
            txtQuantity = new TextBox();
            txtQuantity.Font = new Font("Arial", 11F, FontStyle.Regular);
            txtQuantity.Location = new Point(690, 40);
            txtQuantity.Size = new Size(95, 24);
            txtQuantity.Text = "1";
            txtQuantity.KeyPress += NumericKeyPress;
            txtQuantity.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    AddSelectedProduct();
                    e.SuppressKeyPress = true;
                }
            };
            addPanel.Controls.Add(txtQuantity);

            Button btnAdd = AddButton(addPanel, "Add", 804, 39);
            btnAdd.Click += delegate { AddSelectedProduct(); };

            lblSelectedProduct = new Label();
            lblSelectedProduct.Font = new Font("Arial", 9F, FontStyle.Bold);
            lblSelectedProduct.ForeColor = Color.DarkGreen;
            lblSelectedProduct.BackColor = Color.Transparent;
            lblSelectedProduct.Location = new Point(18, 68);
            lblSelectedProduct.Size = new Size(1038, 18);
            addPanel.Controls.Add(lblSelectedProduct);

            dgvProductSearch = new DataGridView();
            dgvProductSearch.Location = new Point(18, 90);
            dgvProductSearch.Size = new Size(870, 62);
            dgvProductSearch.ReadOnly = true;
            dgvProductSearch.AllowUserToAddRows = false;
            dgvProductSearch.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductSearch.MultiSelect = false;
            dgvProductSearch.Visible = false;
            dgvProductSearch.CellClick += delegate { PickHighlightedProduct(); };
            dgvProductSearch.CellDoubleClick += delegate { PickHighlightedProduct(); txtQuantity.Focus(); txtQuantity.SelectAll(); };
            dgvProductSearch.KeyDown += ProductGridKeyDown;
            ApplyAutoCompleteGridStyle(dgvProductSearch);
            addPanel.Controls.Add(dgvProductSearch);

            Panel leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = noteKind == "DN" ? 250 : 0;
            leftPanel.BackColor = Color.WhiteSmoke;
            leftPanel.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(leftPanel);

            if (noteKind == "DN")
            {
                Panel leftHeader = CreateHeaderPanel("Entered Delivery Notes");
                leftPanel.Controls.Add(leftHeader);
                dgvExistingNotes = new DataGridView();
                dgvExistingNotes.Dock = DockStyle.Fill;
                dgvExistingNotes.ReadOnly = true;
                dgvExistingNotes.AllowUserToAddRows = false;
                dgvExistingNotes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvExistingNotes.MultiSelect = false;
                dgvExistingNotes.CellClick += ExistingNotesCellClick;
                ApplyGridStyle(dgvExistingNotes);
                leftPanel.Controls.Add(dgvExistingNotes);
                dgvExistingNotes.BringToFront();
            }

            Panel detailPanel = new Panel();
            detailPanel.Dock = DockStyle.Fill;
            detailPanel.BackColor = Color.WhiteSmoke;
            detailPanel.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(detailPanel);
            detailPanel.BringToFront();

            Panel detailHeader = CreateHeaderPanel("Product Details");
            detailHeader.Dock = DockStyle.Top;
            detailPanel.Controls.Add(detailHeader);

            dgvDetails = new DataGridView();
            dgvDetails.Dock = DockStyle.Fill;
            dgvDetails.AllowUserToAddRows = false;
            dgvDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDetails.CellClick += DgvDetailsCellClick;
            dgvDetails.EditingControlShowing += DgvDetailsEditingControlShowing;
            ApplyGridStyle(dgvDetails);
            detailPanel.Controls.Add(dgvDetails);
            detailHeader.BringToFront();
        }

        private void LoadLocations()
        {
            try
            {
                DataTable locations = service.GetLocations();
                DataRow row = locations.NewRow();
                row["LocationId"] = 0;
                row["DisplayName"] = "-- Select --";
                locations.Rows.InsertAt(row, 0);
                cmbFromLocation.DataSource = locations.Copy();
                cmbFromLocation.DisplayMember = "DisplayName";
                cmbFromLocation.ValueMember = "LocationId";
                cmbToLocation.DataSource = locations.Copy();
                cmbToLocation.DisplayMember = "DisplayName";
                cmbToLocation.ValueMember = "LocationId";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load Inventory Location Master." + Environment.NewLine + ex.Message);
            }
        }

        private void RefreshProductSearch()
        {
            if (txtProductSearch == null || dgvProductSearch == null)
                return;
            if (txtProductSearch.Text.Trim().Length == 0)
            {
                dgvProductSearch.Visible = false;
                selectedProduct = null;
                lblSelectedProduct.Text = "";
                return;
            }
            try
            {
                int locationId = noteKind == "DN" ? GetComboValue(cmbFromLocation) : GetComboValue(cmbToLocation);
                DataTable products = service.SearchProducts(txtProductSearch.Text, locationId);
                dgvProductSearch.DataSource = products;
                FormatProductSearchColumns();
                dgvProductSearch.Visible = products.Rows.Count > 0;
                if (products.Rows.Count > 0)
                {
                    dgvProductSearch.CurrentCell = dgvProductSearch.Rows[0].Cells["ItemCode"];
                    PickHighlightedProduct();
                }
                else
                {
                    selectedProduct = null;
                    lblSelectedProduct.Text = "No product found";
                }
            }
            catch
            {
                dgvProductSearch.DataSource = null;
                dgvProductSearch.Visible = false;
            }
        }

        private void ProductSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && dgvProductSearch.Visible && dgvProductSearch.Rows.Count > 0)
            {
                dgvProductSearch.Focus();
                dgvProductSearch.CurrentCell = dgvProductSearch.Rows[0].Cells["ItemCode"];
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                PickHighlightedProduct();
                txtQuantity.Focus();
                txtQuantity.SelectAll();
                e.SuppressKeyPress = true;
            }
        }

        private void ProductGridKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                PickHighlightedProduct();
                txtQuantity.Focus();
                txtQuantity.SelectAll();
                e.SuppressKeyPress = true;
            }
        }

        private void PickHighlightedProduct()
        {
            if (dgvProductSearch.CurrentRow == null || dgvProductSearch.CurrentRow.DataBoundItem == null)
                return;
            DataRowView view = dgvProductSearch.CurrentRow.DataBoundItem as DataRowView;
            if (view == null)
                return;
            selectedProduct = view.Row;
            lblSelectedProduct.Text = "Selected: " + Convert.ToString(selectedProduct["ItemCode"]) + " - " +
                Convert.ToString(selectedProduct["DisplayName"]) + "   Stock: " +
                Convert.ToDecimal(selectedProduct["AvailableStock"]).ToString("0.###");
        }

        private void LoadDetailGrid()
        {
            dgvDetails.Columns.Clear();
            dgvDetails.ColumnCount = 7;
            dgvDetails.Columns[0].Name = "S.NO";
            dgvDetails.Columns[1].Name = "Items";
            dgvDetails.Columns[2].Name = "UOM";
            dgvDetails.Columns[3].Name = "Quantity";
            dgvDetails.Columns[4].Name = "productid";
            dgvDetails.Columns[5].Name = "ItemCode";
            dgvDetails.Columns[6].Name = "Available Stock";
            dgvDetails.Columns["productid"].Visible = false;
            dgvDetails.Columns["ItemCode"].Visible = false;
            dgvDetails.Columns[0].Width = 55;
            dgvDetails.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDetails.Columns[2].Width = 80;
            dgvDetails.Columns[3].Width = 95;
            dgvDetails.Columns[6].Width = 110;
            for (int i = 0; i <= 2; i++)
                dgvDetails.Columns[i].ReadOnly = true;
            dgvDetails.Columns["Available Stock"].ReadOnly = true;
            dgvDetails.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetails.Columns["Available Stock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn();
            deleteColumn.Name = "Remove";
            deleteColumn.Text = "Remove";
            deleteColumn.UseColumnTextForButtonValue = true;
            deleteColumn.Width = 75;
            dgvDetails.Columns.Add(deleteColumn);
        }

        private void AddSelectedProduct()
        {
            if (viewingNoteId != 0)
                ClearCurrentDetails();
            PickHighlightedProduct();
            if (selectedProduct == null)
            {
                MessageBox.Show("Please select product");
                txtProductSearch.Focus();
                return;
            }

            decimal quantity;
            if (!decimal.TryParse(txtQuantity.Text, out quantity) || quantity <= 0)
            {
                MessageBox.Show("Please Enter Valid Quantity");
                txtQuantity.Focus();
                txtQuantity.SelectAll();
                return;
            }

            int materialId = Convert.ToInt32(selectedProduct["id"]);
            for (int i = 0; i < dgvDetails.Rows.Count; i++)
            {
                if (Convert.ToInt32(dgvDetails.Rows[i].Cells["productid"].Value) == materialId)
                {
                    decimal oldQty = Convert.ToDecimal(dgvDetails.Rows[i].Cells["Quantity"].Value);
                    dgvDetails.Rows[i].Cells["Quantity"].Value = (oldQty + quantity).ToString("0.###");
                    ClearProductEntry();
                    return;
                }
            }

            int row = dgvDetails.Rows.Add();
            dgvDetails.Rows[row].Cells["S.NO"].Value = row + 1;
            dgvDetails.Rows[row].Cells["Items"].Value = Convert.ToString(selectedProduct["DisplayName"]);
            dgvDetails.Rows[row].Cells["UOM"].Value = Convert.ToString(selectedProduct["UOM"]);
            dgvDetails.Rows[row].Cells["Quantity"].Value = quantity.ToString("0.###");
            dgvDetails.Rows[row].Cells["productid"].Value = materialId;
            dgvDetails.Rows[row].Cells["ItemCode"].Value = Convert.ToString(selectedProduct["ItemCode"]);
            dgvDetails.Rows[row].Cells["Available Stock"].Value = Convert.ToDecimal(selectedProduct["AvailableStock"]).ToString("0.###");
            ClearProductEntry();
        }

        private void ClearProductEntry()
        {
            txtProductSearch.Text = "";
            txtQuantity.Text = "1";
            selectedProduct = null;
            lblSelectedProduct.Text = "";
            dgvProductSearch.Visible = false;
            txtProductSearch.Focus();
        }

        private void Save()
        {
            if (viewingNoteId != 0)
            {
                MessageBox.Show("Selected Delivery Note is opened for view. Click New to create a new entry.");
                return;
            }
            if (!ValidateForm())
                return;
            try
            {
                List<InventoryNoteLine> lines = BuildLines();
                object referenceDate = chkReferenceDate.Checked ? (object)dtpReferenceDate.Value.Date : null;
                string noteNo = service.SaveNote(noteKind, dtpNoteDate.Value.Date, GetComboValue(cmbFromLocation), GetComboValue(cmbToLocation), txtReferenceNo.Text, referenceDate, txtRemarks.Text, lines);
                MessageBox.Show(NoteTitle.Replace(" Entry", "") + " saved as Pending." + Environment.NewLine + "No: " + noteNo);
                ClearForm();
                LoadExistingNotes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save Failed" + Environment.NewLine + ex.Message);
            }
        }

        private bool ValidateForm()
        {
            string message = "";
            if (GetComboValue(cmbFromLocation) == 0)
                message += "* Select From Location" + Environment.NewLine;
            if (GetComboValue(cmbToLocation) == 0)
                message += "* Select To Location" + Environment.NewLine;
            if (GetComboValue(cmbFromLocation) != 0 && GetComboValue(cmbFromLocation) == GetComboValue(cmbToLocation))
                message += "* From and To Location should be different" + Environment.NewLine;
            if (dgvDetails.Rows.Count == 0)
                message += "* Add product details" + Environment.NewLine;
            for (int i = 0; i < dgvDetails.Rows.Count; i++)
            {
                decimal quantity;
                if (!decimal.TryParse(Convert.ToString(dgvDetails.Rows[i].Cells["Quantity"].Value), out quantity) || quantity <= 0)
                    message += "* Enter valid quantity for " + Convert.ToString(dgvDetails.Rows[i].Cells["Items"].Value) + Environment.NewLine;
            }
            if (message != "")
            {
                MessageBox.Show("* Mantatory Fields" + Environment.NewLine + "----------------------------------------" + Environment.NewLine + message);
                return false;
            }
            return true;
        }

        private List<InventoryNoteLine> BuildLines()
        {
            List<InventoryNoteLine> lines = new List<InventoryNoteLine>();
            for (int i = 0; i < dgvDetails.Rows.Count; i++)
            {
                InventoryNoteLine line = new InventoryNoteLine();
                line.MaterialId = Convert.ToInt32(dgvDetails.Rows[i].Cells["productid"].Value);
                line.ItemCode = Convert.ToString(dgvDetails.Rows[i].Cells["ItemCode"].Value);
                line.ProductName = Convert.ToString(dgvDetails.Rows[i].Cells["Items"].Value);
                line.Brand = "";
                line.Size = "";
                line.UOM = Convert.ToString(dgvDetails.Rows[i].Cells["UOM"].Value);
                line.AvailableStock = Convert.ToDecimal(dgvDetails.Rows[i].Cells["Available Stock"].Value);
                line.Quantity = Convert.ToDecimal(dgvDetails.Rows[i].Cells["Quantity"].Value);
                line.Remarks = "";
                lines.Add(line);
            }
            return lines;
        }

        private void ClearForm()
        {
            txtNoteNo.Text = "";
            txtReferenceNo.Text = "";
            txtRemarks.Text = "";
            dtpNoteDate.Value = DateTime.Today;
            dtpReferenceDate.Value = DateTime.Today;
            chkReferenceDate.Checked = false;
            viewingNoteId = 0;
            lblEntryStatus.Text = "New Entry";
            if (cmbFromLocation.Items.Count > 0)
                cmbFromLocation.SelectedIndex = 0;
            if (cmbToLocation.Items.Count > 0)
                cmbToLocation.SelectedIndex = 0;
            dgvDetails.Rows.Clear();
            ClearProductEntry();
        }

        private void DgvDetailsCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvDetails.Columns[e.ColumnIndex].Name == "Remove")
            {
                dgvDetails.Rows.RemoveAt(e.RowIndex);
                ResetSerialNumbers();
            }
        }

        private void DgvDetailsEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (editingTextBox != null)
                editingTextBox.KeyPress -= NumericKeyPress;
            editingTextBox = e.Control as TextBox;
            if (editingTextBox != null && dgvDetails.CurrentCell.OwningColumn.Name == "Quantity")
                editingTextBox.KeyPress += NumericKeyPress;
        }

        private void FormatProductSearchColumns()
        {
            foreach (DataGridViewColumn column in dgvProductSearch.Columns)
                column.Visible = false;
            if (dgvProductSearch.Columns.Contains("ItemCode"))
            {
                dgvProductSearch.Columns["ItemCode"].Visible = true;
                dgvProductSearch.Columns["ItemCode"].HeaderText = "Item Code";
                dgvProductSearch.Columns["ItemCode"].Width = 150;
            }
            if (dgvProductSearch.Columns.Contains("DisplayName"))
            {
                dgvProductSearch.Columns["DisplayName"].Visible = true;
                dgvProductSearch.Columns["DisplayName"].HeaderText = "Product Name";
                dgvProductSearch.Columns["DisplayName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void LoadExistingNotes()
        {
            if (noteKind != "DN" || dgvExistingNotes == null)
                return;
            try
            {
                DataTable notes = service.SearchNotes("DN", DateTime.Today.AddDays(-60), DateTime.Today, "", 0, 0, "ALL");
                dgvExistingNotes.DataSource = notes;
                foreach (DataGridViewColumn column in dgvExistingNotes.Columns)
                    column.Visible = false;
                if (dgvExistingNotes.Columns.Contains("NoteNo"))
                {
                    dgvExistingNotes.Columns["NoteNo"].Visible = true;
                    dgvExistingNotes.Columns["NoteNo"].HeaderText = "Delivery Note";
                    dgvExistingNotes.Columns["NoteNo"].Width = 125;
                }
                if (dgvExistingNotes.Columns.Contains("Status"))
                {
                    dgvExistingNotes.Columns["Status"].Visible = true;
                    dgvExistingNotes.Columns["Status"].Width = 80;
                }
            }
            catch
            {
                dgvExistingNotes.DataSource = null;
            }
        }

        private void ExistingNotesCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvExistingNotes.Rows[e.RowIndex].Cells["NoteId"].Value == null)
                return;
            viewingNoteId = Convert.ToInt32(dgvExistingNotes.Rows[e.RowIndex].Cells["NoteId"].Value);
            lblEntryStatus.Text = "Viewing: " + Convert.ToString(dgvExistingNotes.Rows[e.RowIndex].Cells["NoteNo"].Value);
            LoadViewedNoteDetails(viewingNoteId);
        }

        private void LoadViewedNoteDetails(int noteId)
        {
            dgvDetails.Rows.Clear();
            DataTable details = service.GetNoteDetails("DN", noteId);
            for (int i = 0; i < details.Rows.Count; i++)
            {
                int row = dgvDetails.Rows.Add();
                dgvDetails.Rows[row].Cells["S.NO"].Value = row + 1;
                dgvDetails.Rows[row].Cells["Items"].Value = Convert.ToString(details.Rows[i]["ProductName"]);
                dgvDetails.Rows[row].Cells["UOM"].Value = Convert.ToString(details.Rows[i]["UOM"]);
                dgvDetails.Rows[row].Cells["Quantity"].Value = Convert.ToDecimal(details.Rows[i]["Quantity"]).ToString("0.###");
                dgvDetails.Rows[row].Cells["productid"].Value = Convert.ToString(details.Rows[i]["MaterialId"]);
                dgvDetails.Rows[row].Cells["ItemCode"].Value = Convert.ToString(details.Rows[i]["ItemCode"]);
                dgvDetails.Rows[row].Cells["Available Stock"].Value = "";
            }
        }

        private void ClearCurrentDetails()
        {
            viewingNoteId = 0;
            lblEntryStatus.Text = "New Entry";
            dgvDetails.Rows.Clear();
        }

        private void ResetSerialNumbers()
        {
            for (int i = 0; i < dgvDetails.Rows.Count; i++)
                dgvDetails.Rows[i].Cells["S.NO"].Value = i + 1;
        }

        private void NumericKeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;
            TextBox textBox = sender as TextBox;
            string next = textBox.Text.Insert(textBox.SelectionStart, e.KeyChar.ToString());
            if (!Regex.IsMatch(next, @"^\d*\.?\d{0,3}$"))
                e.Handled = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            if (keyData == (Keys.S | Keys.Control))
            {
                Save();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static int GetComboValue(ComboBox combo)
        {
            if (combo.SelectedValue == null || combo.SelectedValue == DBNull.Value)
                return 0;
            int value;
            return int.TryParse(Convert.ToString(combo.SelectedValue), out value) ? value : 0;
        }

        private static Panel CreateHeaderPanel(string title)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Top;
            panel.Height = 28;
            panel.BackgroundImage = Inventory.Properties.Resources.labelBack1;
            Label label = new Label();
            label.Text = title;
            label.ForeColor = Color.White;
            label.BackColor = Color.Transparent;
            label.Font = new Font("Calibri", 10F, FontStyle.Bold);
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Padding = new Padding(12, 0, 0, 0);
            panel.Controls.Add(label);
            return panel;
        }

        private static Label AddLabel(Control parent, string text, int x, int y, int width)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Arial", 9F, FontStyle.Bold);
            label.BackColor = Color.Transparent;
            label.Location = new Point(x, y + 3);
            label.Size = new Size(width, 18);
            parent.Controls.Add(label);
            return label;
        }

        private static TextBox AddText(Control parent, string label, int x, int y, int textX, bool readOnly)
        {
            AddLabel(parent, label, x, y, textX - x - 4);
            TextBox text = new TextBox();
            text.Font = new Font("Arial", 8.25F);
            text.Location = new Point(textX, y);
            text.Size = new Size(150, 20);
            text.ReadOnly = readOnly;
            parent.Controls.Add(text);
            return text;
        }

        private static DateTimePicker AddDate(Control parent, string label, int x, int y)
        {
            AddLabel(parent, label, x, y, 95);
            DateTimePicker date = new DateTimePicker();
            date.CustomFormat = "dd-MM-yyyy";
            date.Format = DateTimePickerFormat.Custom;
            date.Font = new Font("Calibri", 8F);
            date.Location = new Point(x + 100, y);
            date.Size = new Size(110, 21);
            parent.Controls.Add(date);
            return date;
        }

        private static ComboBox AddCombo(Control parent, string label, int x, int y, int comboX, int width)
        {
            AddLabel(parent, label, x, y, comboX - x - 4);
            ComboBox combo = new ComboBox();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Font = new Font("Arial", 8.25F);
            combo.Location = new Point(comboX, y);
            combo.Size = new Size(width, 22);
            parent.Controls.Add(combo);
            return combo;
        }

        private static Button AddButton(Control parent, string text, int x, int y)
        {
            Button button = new Button();
            button.Text = text;
            button.Cursor = Cursors.Hand;
            button.FlatStyle = FlatStyle.Popup;
            button.Font = new Font("Calibri", 9F);
            button.Location = new Point(x, y);
            button.Size = new Size(75, 25);
            button.UseVisualStyleBackColor = true;
            parent.Controls.Add(button);
            return button;
        }

        private static void ApplyGridStyle(DataGridView grid)
        {
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            grid.DefaultCellStyle.BackColor = Color.Gainsboro;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            grid.BackgroundColor = Color.WhiteSmoke;
            grid.BorderStyle = BorderStyle.Fixed3D;
        }

        private static void ApplyAutoCompleteGridStyle(DataGridView grid)
        {
            ApplyGridStyle(grid);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("Arial", 13F, GraphicsUnit.Pixel);
            grid.RowTemplate.Height = 26;
        }
    }
}
