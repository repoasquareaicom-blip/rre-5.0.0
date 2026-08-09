using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.InventoryNotes
{
    public class InventoryNoteApprovalForm : Form
    {
        private readonly string noteKind;
        private readonly InventoryNoteService service = new InventoryNoteService();
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private TextBox txtNoteNo;
        private ComboBox cmbFromLocation;
        private ComboBox cmbToLocation;
        private ComboBox cmbStatus;
        private DataGridView dgvNotes;
        private DataGridView dgvDetails;
        private TextBox txtRejectionRemarks;
        private int selectedNoteId;

        public InventoryNoteApprovalForm(string noteKind)
        {
            this.noteKind = noteKind;
            Initialize();
            LoadLocations();
            LoadNoteGrid();
            LoadDetailGrid();
            Search();
        }

        private string NoteTitle
        {
            get { return noteKind == "DN" ? "Delivery Note Approval" : "Receipt Note Approval"; }
        }

        private void Initialize()
        {
            Text = NoteTitle;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.WhiteSmoke;
            Font = new Font("Arial", 9F);

            Controls.Add(CreateHeaderPanel(NoteTitle));

            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 105;
            searchPanel.BorderStyle = BorderStyle.Fixed3D;
            searchPanel.BackColor = Color.WhiteSmoke;
            Controls.Add(searchPanel);

            dtpFrom = AddDate(searchPanel, "From Date:", 18, 18);
            dtpTo = AddDate(searchPanel, "To Date:", 250, 18);
            txtNoteNo = AddText(searchPanel, "Note No:", 482, 18, 550);
            cmbStatus = new ComboBox();
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Font = new Font("Arial", 8.25F);
            cmbStatus.Items.AddRange(new object[] { "PENDING", "APPROVED", "REJECTED", "ALL" });
            cmbStatus.SelectedIndex = 0;
            AddLabel(searchPanel, "Status:", 720, 18, 60);
            cmbStatus.Location = new Point(785, 18);
            cmbStatus.Size = new Size(120, 22);
            searchPanel.Controls.Add(cmbStatus);
            cmbFromLocation = AddCombo(searchPanel, "From Location:", 18, 55, 120);
            cmbToLocation = AddCombo(searchPanel, "To Location:", 330, 55, 425);
            Button btnSearch = AddButton(searchPanel, "Search", 650, 52);
            btnSearch.Click += delegate { Search(); };
            Button btnClose = AddButton(searchPanel, "Close", 735, 52);
            btnClose.Click += delegate { Close(); };

            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Horizontal;
            split.SplitterDistance = 260;
            Controls.Add(split);

            dgvNotes = new DataGridView();
            dgvNotes.Dock = DockStyle.Fill;
            dgvNotes.ReadOnly = true;
            dgvNotes.AllowUserToAddRows = false;
            dgvNotes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNotes.MultiSelect = false;
            dgvNotes.CellClick += DgvNotesCellClick;
            ApplyGridStyle(dgvNotes);
            split.Panel1.Controls.Add(dgvNotes);

            Panel actionPanel = new Panel();
            actionPanel.Dock = DockStyle.Top;
            actionPanel.Height = 55;
            actionPanel.BackColor = Color.WhiteSmoke;
            split.Panel2.Controls.Add(actionPanel);
            AddLabel(actionPanel, "Rejection Remarks:", 18, 17, 130);
            txtRejectionRemarks = new TextBox();
            txtRejectionRemarks.Font = new Font("Arial", 8.25F);
            txtRejectionRemarks.Location = new Point(150, 15);
            txtRejectionRemarks.Size = new Size(430, 20);
            actionPanel.Controls.Add(txtRejectionRemarks);
            Button btnApprove = AddButton(actionPanel, "Approve", 600, 12);
            btnApprove.Click += delegate { Approve(); };
            Button btnReject = AddButton(actionPanel, "Reject", 685, 12);
            btnReject.Click += delegate { Reject(); };
            if (noteKind == "DN")
            {
                Button btnPrint = AddButton(actionPanel, "Print", 770, 12);
                btnPrint.Click += delegate { PrintDeliveryNote(); };
            }

            dgvDetails = new DataGridView();
            dgvDetails.Dock = DockStyle.Fill;
            dgvDetails.ReadOnly = true;
            dgvDetails.AllowUserToAddRows = false;
            dgvDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ApplyGridStyle(dgvDetails);
            split.Panel2.Controls.Add(dgvDetails);
            dgvDetails.BringToFront();
        }

        private void LoadLocations()
        {
            try
            {
                DataTable locations = service.GetLocations();
                DataRow row = locations.NewRow();
                row["LocationId"] = 0;
                row["DisplayName"] = "All";
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

        private void LoadNoteGrid()
        {
            dgvNotes.AutoGenerateColumns = true;
        }

        private void LoadDetailGrid()
        {
            dgvDetails.AutoGenerateColumns = true;
        }

        private void Search()
        {
            try
            {
                dgvNotes.DataSource = service.SearchNotes(noteKind, dtpFrom.Value.Date, dtpTo.Value.Date, txtNoteNo.Text, GetComboValue(cmbFromLocation), GetComboValue(cmbToLocation), Convert.ToString(cmbStatus.SelectedItem));
                if (dgvNotes.Columns.Contains("NoteId"))
                    dgvNotes.Columns["NoteId"].Visible = false;
                selectedNoteId = 0;
                dgvDetails.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search Failed" + Environment.NewLine + ex.Message);
            }
        }

        private void DgvNotesCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            selectedNoteId = Convert.ToInt32(dgvNotes.Rows[e.RowIndex].Cells["NoteId"].Value);
            LoadSelectedDetails();
        }

        private void LoadSelectedDetails()
        {
            if (selectedNoteId == 0)
                return;
            dgvDetails.DataSource = service.GetNoteDetails(noteKind, selectedNoteId);
            if (dgvDetails.Columns.Contains("MaterialId"))
                dgvDetails.Columns["MaterialId"].Visible = false;
        }

        private void Approve()
        {
            if (selectedNoteId == 0)
            {
                MessageBox.Show("Please select a document");
                return;
            }
            DialogResult result = MessageBox.Show("Do you want to Approve?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;
            try
            {
                service.ApproveNote(noteKind, selectedNoteId);
                MessageBox.Show("Approved Succesfully");
                Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Approval Failed" + Environment.NewLine + ex.Message);
            }
        }

        private void Reject()
        {
            if (selectedNoteId == 0)
            {
                MessageBox.Show("Please select a document");
                return;
            }
            if (txtRejectionRemarks.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Rejection Remarks");
                txtRejectionRemarks.Focus();
                return;
            }
            DialogResult result = MessageBox.Show("Do you want to Reject?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;
            try
            {
                service.RejectNote(noteKind, selectedNoteId, txtRejectionRemarks.Text.Trim());
                MessageBox.Show("Rejected Succesfully");
                txtRejectionRemarks.Text = "";
                Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reject Failed" + Environment.NewLine + ex.Message);
            }
        }

        private void PrintDeliveryNote()
        {
            if (selectedNoteId == 0)
            {
                MessageBox.Show("Please select a Delivery Note");
                return;
            }
            try
            {
                DataRow header = service.GetDeliveryNotePrintHeader(selectedNoteId);
                if (header == null)
                {
                    MessageBox.Show("Selected Delivery Note was not found");
                    return;
                }
                DataTable details = service.GetNoteDetails("DN", selectedNoteId);
                DeliveryNotePrinter printer = new DeliveryNotePrinter(header, details);
                printer.ShowPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print Failed" + Environment.NewLine + ex.Message);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
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

        private static TextBox AddText(Control parent, string label, int x, int y, int textX)
        {
            AddLabel(parent, label, x, y, textX - x - 4);
            TextBox text = new TextBox();
            text.Font = new Font("Arial", 8.25F);
            text.Location = new Point(textX, y);
            text.Size = new Size(150, 20);
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

        private static ComboBox AddCombo(Control parent, string label, int x, int y, int comboX)
        {
            AddLabel(parent, label, x, y, comboX - x - 4);
            ComboBox combo = new ComboBox();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Font = new Font("Arial", 8.25F);
            combo.Location = new Point(comboX, y);
            combo.Size = new Size(185, 22);
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
    }
}
