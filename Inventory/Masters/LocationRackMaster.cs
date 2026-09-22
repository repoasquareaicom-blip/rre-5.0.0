using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Masters
{
    public partial class LocationRackMaster : Form
    {
        private readonly LocationRackRepository repository = new LocationRackRepository();
        private DataTable locationsTable;
        private int selectedLocationId;
        private string selectedLocationName = "";
        private int editingLocationId;
        private int editingRackId;
        private static readonly Color AssignedRackBackColor = Color.FromArgb(226, 246, 226);

        public LocationRackMaster()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void LocationRackMaster_Load(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            ConfigureGrids();
            ResetLocationEditor();
            ResetRackEditor();
            pnlRackCreate.Visible = false;
            LoadLocations();
        }

        private void ConfigureGrids()
        {
            ConfigureGrid(dgvLocations);
            ConfigureGrid(dgvRacks);
        }

        private void ConfigureGrid(DataGridView grid)
        {
            grid.AutoGenerateColumns = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            grid.ReadOnly = true;
            grid.RowTemplate.Height = 24;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.GridColor = Color.Gainsboro;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 245, 252);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 9.75F, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("Calibri", 9.75F);
            grid.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            grid.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
        }

        private void dgvLocations_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvLocations.Columns[e.ColumnIndex].Name != "colLocationAllowForSales")
            {
                return;
            }

            bool allowForSales = false;
            if (e.Value != null && e.Value != DBNull.Value)
            {
                bool.TryParse(Convert.ToString(e.Value), out allowForSales);

                int intValue;
                if (!allowForSales && int.TryParse(Convert.ToString(e.Value), out intValue))
                {
                    allowForSales = intValue != 0;
                }
            }

            e.Value = allowForSales ? "Yes" : "No";
            e.FormattingApplied = true;
        }

        private void LoadLocations()
        {
            try
            {
                int previousLocationId = selectedLocationId;
                locationsTable = repository.GetLocations();
                dgvLocations.DataSource = locationsTable;

                if (locationsTable.Rows.Count == 0)
                {
                    selectedLocationId = 0;
                    selectedLocationName = "";
                    lblRackTitle.Text = "Racks";
                    dgvRacks.DataSource = null;
                    btnEditLocation.Enabled = false;
                    btnDeleteLocation.Enabled = false;
                    btnAddRacks.Enabled = false;
                    btnEditRack.Enabled = false;
                    btnDeleteRack.Enabled = false;
                    return;
                }

                SelectLocation(previousLocationId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load locations. " + ex.Message);
            }
        }

        private void SelectLocation(int locationId)
        {
            int rowIndex = 0;
            if (locationId > 0)
            {
                for (int i = 0; i < dgvLocations.Rows.Count; i++)
                {
                    object value = dgvLocations.Rows[i].Cells["colLocationId"].Value;
                    if (value != null && value != DBNull.Value && Convert.ToInt32(value) == locationId)
                    {
                        rowIndex = i;
                        break;
                    }
                }
            }

            if (dgvLocations.Rows.Count > 0)
            {
                dgvLocations.ClearSelection();
                dgvLocations.Rows[rowIndex].Selected = true;
                dgvLocations.CurrentCell = dgvLocations.Rows[rowIndex].Cells["colLocationName"];
                ApplySelectedLocationFromGrid(true);
            }
        }

        private void dgvLocations_SelectionChanged(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || dgvLocations.CurrentRow == null)
            {
                return;
            }

            ApplySelectedLocationFromGrid(false);
        }

        private void ApplySelectedLocationFromGrid(bool forceLoad)
        {
            if (dgvLocations.CurrentRow == null || dgvLocations.CurrentRow.DataBoundItem == null)
            {
                return;
            }

            DataRowView rowView = dgvLocations.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null)
            {
                return;
            }

            int locationId = Convert.ToInt32(rowView["LocationId"]);
            if (!forceLoad && selectedLocationId == locationId && dgvRacks.DataSource != null)
            {
                return;
            }

            selectedLocationId = locationId;
            selectedLocationName = Convert.ToString(rowView["LocationName"]);
            lblRackTitle.Text = "Racks - " + selectedLocationName;
            btnEditLocation.Enabled = true;
            btnDeleteLocation.Enabled = false;
            btnAddRacks.Enabled = true;
            ResetRackEditor();
            pnlRackCreate.Visible = false;
            LoadRacks();
        }

        private void LoadRacks()
        {
            if (selectedLocationId <= 0)
            {
                dgvRacks.DataSource = null;
                btnEditRack.Enabled = false;
                btnDeleteRack.Enabled = false;
                return;
            }

            try
            {
                dgvRacks.DataSource = repository.GetRacks(selectedLocationId);
                ApplyRackRowColors();
                btnEditRack.Enabled = dgvRacks.Rows.Count > 0;
                btnDeleteRack.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load racks. " + ex.Message);
            }
        }

        private void dgvRacks_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ApplyRackRowColors();
        }

        private void ApplyRackRowColors()
        {
            foreach (DataGridViewRow row in dgvRacks.Rows)
            {
                DataRowView rowView = row.DataBoundItem as DataRowView;
                bool isAssigned = rowView != null && GetBooleanColumnValue(rowView.Row, "IsAssigned");
                row.DefaultCellStyle.BackColor = isAssigned ? AssignedRackBackColor : Color.White;

                int productCount = rowView != null ? GetIntegerColumnValue(rowView.Row, "ProductCount") : 0;
                DataGridViewCell productCell = row.Cells["colRackProducts"];
                productCell.Style.ForeColor = productCount > 0 ? Color.Blue : Color.Black;
                productCell.Style.SelectionForeColor = productCount > 0 ? Color.White : SystemColors.HighlightText;
                productCell.Style.Font = productCount > 0
                    ? new Font(dgvRacks.DefaultCellStyle.Font, FontStyle.Underline)
                    : dgvRacks.DefaultCellStyle.Font;
            }
        }

        private void btnAddLocation_Click(object sender, EventArgs e)
        {
            editingLocationId = 0;
            txtLocationName.Text = "";
            nudDisplayOrder.Value = GetNextDisplayOrder();
            chkAllowForSales.Checked = true;
            pnlLocationEditor.Visible = true;
            txtLocationName.Focus();
        }

        private void btnEditLocation_Click(object sender, EventArgs e)
        {
            DataRowView rowView = GetCurrentLocationRow();
            if (rowView == null)
            {
                return;
            }

            editingLocationId = Convert.ToInt32(rowView["LocationId"]);
            txtLocationName.Text = Convert.ToString(rowView["LocationName"]);
            nudDisplayOrder.Value = GetDisplayOrder(rowView.Row);
            chkAllowForSales.Checked = GetBooleanColumnValue(rowView.Row, "AllowForSales");
            pnlLocationEditor.Visible = true;
            txtLocationName.Focus();
            txtLocationName.SelectAll();
        }

        private void btnSaveLocation_Click(object sender, EventArgs e)
        {
            string name = txtLocationName.Text.Trim();
            if (name.Length == 0)
            {
                MessageBox.Show("* Mantatory Fields\n----------------------------------------\n* Location Name Should Not Be Empty");
                txtLocationName.Focus();
                return;
            }

            int displayOrder = Convert.ToInt32(nudDisplayOrder.Value);
            if (displayOrder <= 0)
            {
                MessageBox.Show("* Mantatory Fields\n----------------------------------------\n* Sales Priority Should Be Positive Number");
                nudDisplayOrder.Focus();
                return;
            }

            try
            {
                int updatedBy = repository.GetCurrentUserId();
                bool allowForSales = chkAllowForSales.Checked;
                if (editingLocationId == 0)
                {
                    selectedLocationId = repository.InsertLocation(name, displayOrder, allowForSales, updatedBy);
                }
                else
                {
                    repository.UpdateLocation(editingLocationId, name, displayOrder, allowForSales, updatedBy);
                    selectedLocationId = editingLocationId;
                }

                ResetLocationEditor();
                LoadLocations();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Location save failed. " + ex.Message);
                txtLocationName.Focus();
            }
        }

        private void btnCancelLocation_Click(object sender, EventArgs e)
        {
            ResetLocationEditor();
        }

        private void btnDeleteLocation_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Delete Location is not available in the existing Location & Rack Master behavior.");
        }

        private void ResetLocationEditor()
        {
            editingLocationId = 0;
            txtLocationName.Text = "";
            nudDisplayOrder.Value = 1;
            chkAllowForSales.Checked = true;
            pnlLocationEditor.Visible = false;
        }

        private void btnAddRacks_Click(object sender, EventArgs e)
        {
            ShowRackCreatePanel();
        }

        private void ShowRackCreatePanel()
        {
            if (selectedLocationId <= 0)
            {
                MessageBox.Show("Select valid location");
                return;
            }

            pnlRackCreate.Visible = true;
            nudRackCount.Value = 1;
            txtPrefix.Text = "";
            rdoAuto.Checked = true;
            UpdateRackCreateMode();
            txtPrefix.Focus();
        }

        private void btnCancelRackCreate_Click(object sender, EventArgs e)
        {
            pnlRackCreate.Visible = false;
            flpManualRacks.Controls.Clear();
        }

        private void rdoAuto_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRackCreateMode();
        }

        private void rdoManual_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRackCreateMode();
        }

        private void nudRackCount_ValueChanged(object sender, EventArgs e)
        {
            if (rdoManual.Checked)
            {
                BuildManualRackInputs();
            }
        }

        private void UpdateRackCreateMode()
        {
            bool autoMode = rdoAuto.Checked;
            lblPrefix.Visible = autoMode;
            txtPrefix.Visible = autoMode;
            flpManualRacks.Visible = !autoMode;

            if (autoMode)
            {
                flpManualRacks.Controls.Clear();
            }
            else
            {
                BuildManualRackInputs();
            }
        }

        private void BuildManualRackInputs()
        {
            flpManualRacks.SuspendLayout();
            flpManualRacks.Controls.Clear();

            for (int i = 1; i <= Convert.ToInt32(nudRackCount.Value); i++)
            {
                Panel row = new Panel();
                row.Width = 260;
                row.Height = 28;
                row.Margin = new Padding(0, 0, 8, 4);

                Label label = new Label();
                label.AutoSize = false;
                label.Text = "Rack " + i.ToString();
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Font = new Font("Calibri", 9.75F);
                label.Location = new Point(0, 4);
                label.Size = new Size(55, 20);

                TextBox textBox = new TextBox();
                textBox.Name = "txtManualRack" + i.ToString();
                textBox.Font = new Font("Arial", 8.25F);
                textBox.Location = new Point(60, 3);
                textBox.Size = new Size(190, 20);
                textBox.MaxLength = 100;

                row.Controls.Add(label);
                row.Controls.Add(textBox);
                flpManualRacks.Controls.Add(row);
            }

            flpManualRacks.ResumeLayout();
        }

        private void btnCreateRacks_Click(object sender, EventArgs e)
        {
            string[] rackNames = BuildRackNames();
            if (rackNames.Length == 0)
            {
                return;
            }

            int successCount = 0;
            try
            {
                int updatedBy = repository.GetCurrentUserId();
                foreach (string rackName in rackNames)
                {
                    repository.InsertRack(selectedLocationId, rackName, updatedBy);
                    successCount++;
                }

                MessageBox.Show("Rack created successfully");
                pnlRackCreate.Visible = false;
                flpManualRacks.Controls.Clear();
                LoadRacks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Rack creation failed after " + successCount.ToString() + " rack(s). " + ex.Message);
                LoadRacks();
            }
        }

        private string[] BuildRackNames()
        {
            System.Collections.Generic.List<string> rackNames = new System.Collections.Generic.List<string>();
            int count = Convert.ToInt32(nudRackCount.Value);

            if (rdoAuto.Checked)
            {
                string prefix = txtPrefix.Text.Trim();
                if (prefix.Length == 0)
                {
                    MessageBox.Show("Prefix Should Not Be Empty");
                    txtPrefix.Focus();
                    return new string[0];
                }

                for (int i = 1; i <= count; i++)
                {
                    rackNames.Add(prefix + i.ToString());
                }
            }
            else
            {
                foreach (Control row in flpManualRacks.Controls)
                {
                    foreach (Control child in row.Controls)
                    {
                        TextBox textBox = child as TextBox;
                        if (textBox != null)
                        {
                            string caption = textBox.Text.Trim();
                            if (caption.Length == 0)
                            {
                                MessageBox.Show("Rack caption should not be empty");
                                textBox.Focus();
                                return new string[0];
                            }
                            rackNames.Add(caption);
                        }
                    }
                }
            }

            return rackNames.ToArray();
        }

        private void dgvRacks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            string columnName = dgvRacks.Columns[e.ColumnIndex].Name;
            if (columnName == "colRackEdit")
            {
                BeginRackEditFromRow(dgvRacks.Rows[e.RowIndex]);
            }
        }

        private void dgvRacks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgvRacks.Columns[e.ColumnIndex].Name != "colRackProducts")
            {
                return;
            }

            OpenRackProductList(dgvRacks.Rows[e.RowIndex]);
        }

        private void dgvRacks_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgvRacks.Columns[e.ColumnIndex].Name != "colRackProducts")
            {
                dgvRacks.Cursor = Cursors.Default;
                return;
            }

            DataRowView rowView = dgvRacks.Rows[e.RowIndex].DataBoundItem as DataRowView;
            dgvRacks.Cursor = rowView != null && GetIntegerColumnValue(rowView.Row, "ProductCount") > 0
                ? Cursors.Hand
                : Cursors.Default;
        }

        private void dgvRacks_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvRacks.Cursor = Cursors.Default;
        }

        private void OpenRackProductList(DataGridViewRow row)
        {
            DataRowView rowView = row.DataBoundItem as DataRowView;
            if (rowView == null || GetIntegerColumnValue(rowView.Row, "ProductCount") <= 0)
            {
                return;
            }

            int rackId = Convert.ToInt32(rowView["RackId"]);
            string rackCaption = Convert.ToString(rowView["RackCaption"]);
            using (RackProductList form = new RackProductList(repository, rackId, rackCaption))
            {
                form.RackMappingsChanged += delegate
                {
                    LoadRacks();
                };
                form.ShowDialog(this);
            }
        }

        private void btnEditRack_Click(object sender, EventArgs e)
        {
            if (dgvRacks.CurrentRow != null)
            {
                BeginRackEditFromRow(dgvRacks.CurrentRow);
            }
        }

        private void BeginRackEditFromRow(DataGridViewRow row)
        {
            DataRowView rowView = row.DataBoundItem as DataRowView;
            if (rowView == null)
            {
                return;
            }

            editingRackId = Convert.ToInt32(rowView["RackId"]);
            txtRackCaption.Text = Convert.ToString(rowView["RackCaption"]);
            pnlRackEditor.Visible = true;
            txtRackCaption.Focus();
            txtRackCaption.SelectAll();
        }

        private void btnSaveRack_Click(object sender, EventArgs e)
        {
            string caption = txtRackCaption.Text.Trim();
            if (caption.Length == 0)
            {
                MessageBox.Show("Rack caption should not be empty");
                txtRackCaption.Focus();
                return;
            }

            try
            {
                repository.UpdateRack(editingRackId, caption, repository.GetCurrentUserId());
                ResetRackEditor();
                LoadRacks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Rack update failed. " + ex.Message);
                txtRackCaption.Focus();
            }
        }

        private void btnCancelRack_Click(object sender, EventArgs e)
        {
            ResetRackEditor();
        }

        private void btnDeleteRack_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Delete Rack is not available in the existing Location & Rack Master behavior.");
        }

        private void ResetRackEditor()
        {
            editingRackId = 0;
            txtRackCaption.Text = "";
            pnlRackEditor.Visible = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private DataRowView GetCurrentLocationRow()
        {
            if (dgvLocations.CurrentRow == null)
            {
                return null;
            }

            return dgvLocations.CurrentRow.DataBoundItem as DataRowView;
        }

        private int GetDisplayOrder(DataRow row)
        {
            if (row.Table.Columns.Contains("DisplayOrder") && row["DisplayOrder"] != DBNull.Value)
            {
                return Convert.ToInt32(row["DisplayOrder"]);
            }

            return 1;
        }

        private int GetNextDisplayOrder()
        {
            int nextDisplayOrder = 1;
            if (locationsTable == null)
            {
                return nextDisplayOrder;
            }

            foreach (DataRow row in locationsTable.Rows)
            {
                int displayOrder = GetDisplayOrder(row);
                if (displayOrder >= nextDisplayOrder)
                {
                    nextDisplayOrder = displayOrder + 1;
                }
            }

            return nextDisplayOrder;
        }

        private bool GetBooleanColumnValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return false;
            }

            bool boolValue;
            if (bool.TryParse(Convert.ToString(row[columnName]), out boolValue))
            {
                return boolValue;
            }

            int intValue;
            return int.TryParse(Convert.ToString(row[columnName]), out intValue) && intValue != 0;
        }

        private int GetIntegerColumnValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return 0;
            }

            int value;
            return int.TryParse(Convert.ToString(row[columnName]), out value) ? value : 0;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
