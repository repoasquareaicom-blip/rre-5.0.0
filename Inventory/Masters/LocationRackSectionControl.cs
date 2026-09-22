using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Masters
{
    public partial class LocationRackSectionControl : UserControl
    {
        private readonly LocationRackRepository repository;
        private int locationId;
        private string locationName;
        private int displayOrder;
        private bool isNewLocation;
        private static readonly Color AssignedRackBackColor = Color.FromArgb(226, 246, 226);

        public event EventHandler LocationSaved;
        public event EventHandler UnsavedLocationCancelled;

        public int DisplayOrderValue
        {
            get { return displayOrder; }
        }

        public LocationRackSectionControl()
            : this(new LocationRackRepository())
        {
        }

        public LocationRackSectionControl(LocationRackRepository repository)
        {
            InitializeComponent();
            this.repository = repository;
            rdoAuto.Checked = true;
            pnlRackCreate.Visible = false;
            txtPrefix.Visible = true;
            lblPrefix.Visible = true;
            flpManualRacks.Visible = false;
            ArrangeLayout();
        }

        public void LoadNewLocation(int displayOrder)
        {
            locationId = 0;
            locationName = "";
            this.displayOrder = displayOrder > 0 ? displayOrder : 1;
            isNewLocation = true;
            lblLocationName.Text = "New Location";
            lblLocationPriority.Text = "Priority: " + this.displayOrder.ToString();
            txtLocationName.Text = "";
            nudDisplayOrder.Value = this.displayOrder;
            flpRacks.Controls.Clear();
            SetLocationEditMode(true);
            btnAddRacks.Enabled = false;
            btnAddMoreRacks.Enabled = false;
            ArrangeLayout();
        }

        public void LoadLocation(int locationId, string locationName, int displayOrder)
        {
            this.locationId = locationId;
            this.locationName = locationName;
            this.displayOrder = displayOrder > 0 ? displayOrder : 1;
            isNewLocation = false;
            lblLocationName.Text = locationName;
            lblLocationPriority.Text = "Priority: " + this.displayOrder.ToString();
            txtLocationName.Text = locationName;
            nudDisplayOrder.Value = this.displayOrder;
            SetLocationEditMode(false);
            btnAddRacks.Enabled = true;
            btnAddMoreRacks.Enabled = true;
            LoadRacks();
            ArrangeLayout();
        }

        public void EnsureUsableSize(int width)
        {
            if (width < 600)
            {
                width = 600;
            }

            this.Width = width;
            this.Visible = true;
            this.Enabled = true;
            ArrangeLayout();
        }

        public void FocusLocationName()
        {
            txtLocationName.Focus();
            txtLocationName.SelectAll();
        }

        private void SetLocationEditMode(bool editing)
        {
            lblLocationName.Visible = !editing;
            lblLocationPriority.Visible = !editing;
            lblLocationEditName.Visible = editing;
            txtLocationName.Visible = editing;
            lblPriority.Visible = editing;
            nudDisplayOrder.Visible = editing;
            btnEditLocation.Visible = !editing && !isNewLocation;
            btnSaveLocation.Visible = editing;
            btnCancelLocation.Visible = editing;
        }

        private void btnEditLocation_Click(object sender, EventArgs e)
        {
            txtLocationName.Text = locationName;
            nudDisplayOrder.Value = displayOrder > 0 ? displayOrder : 1;
            SetLocationEditMode(true);
            FocusLocationName();
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

            int priority = Convert.ToInt32(nudDisplayOrder.Value);
            if (priority <= 0)
            {
                MessageBox.Show("* Mantatory Fields\n----------------------------------------\n* Sales Priority Should Be Positive Number");
                nudDisplayOrder.Focus();
                return;
            }

            try
            {
                int updatedBy = repository.GetCurrentUserId();
                if (isNewLocation)
                {
                    locationId = repository.InsertLocation(name, priority, updatedBy);
                    isNewLocation = false;
                }
                else
                {
                    repository.UpdateLocation(locationId, name, priority, updatedBy);
                }

                locationName = name;
                displayOrder = priority;
                lblLocationName.Text = locationName;
                lblLocationPriority.Text = "Priority: " + displayOrder.ToString();
                SetLocationEditMode(false);
                btnAddRacks.Enabled = true;
                btnAddMoreRacks.Enabled = true;

                if (LocationSaved != null)
                {
                    LocationSaved(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Location save failed. " + ex.Message);
                txtLocationName.Focus();
            }
        }

        private void btnCancelLocation_Click(object sender, EventArgs e)
        {
            if (isNewLocation)
            {
                if (UnsavedLocationCancelled != null)
                {
                    UnsavedLocationCancelled(this, EventArgs.Empty);
                }
                return;
            }

            txtLocationName.Text = locationName;
            nudDisplayOrder.Value = displayOrder > 0 ? displayOrder : 1;
            SetLocationEditMode(false);
        }

        private void btnAddRacks_Click(object sender, EventArgs e)
        {
            ShowRackCreatePanel();
        }

        private void btnAddMoreRacks_Click(object sender, EventArgs e)
        {
            ShowRackCreatePanel();
        }

        private void ShowRackCreatePanel()
        {
            pnlRackCreate.Visible = true;
            nudRackCount.Value = 1;
            txtPrefix.Text = "";
            rdoAuto.Checked = true;
            UpdateRackCreateMode();
            ArrangeLayout();
            txtPrefix.Focus();
        }

        private void btnCancelRackCreate_Click(object sender, EventArgs e)
        {
            pnlRackCreate.Visible = false;
            flpManualRacks.Controls.Clear();
            ArrangeLayout();
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

            if (!autoMode)
            {
                BuildManualRackInputs();
            }
            else
            {
                flpManualRacks.Controls.Clear();
            }
        }

        private void BuildManualRackInputs()
        {
            flpManualRacks.SuspendLayout();
            flpManualRacks.Controls.Clear();

            for (int i = 1; i <= Convert.ToInt32(nudRackCount.Value); i++)
            {
                Panel row = new Panel();
                row.Width = 310;
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
                textBox.Size = new Size(235, 20);
                textBox.MaxLength = 100;

                row.Controls.Add(label);
                row.Controls.Add(textBox);
                flpManualRacks.Controls.Add(row);
            }

            flpManualRacks.ResumeLayout();
            ArrangeLayout();
        }

        private void btnCreateRacks_Click(object sender, EventArgs e)
        {
            List<string> rackNames = BuildRackNames();
            if (rackNames.Count == 0)
            {
                MessageBox.Show("Please enter rack name");
                return;
            }

            int successCount = 0;
            try
            {
                int updatedBy = repository.GetCurrentUserId();
                foreach (string rackName in rackNames)
                {
                    repository.InsertRack(locationId, rackName, updatedBy);
                    successCount++;
                }

                MessageBox.Show("Rack created successfully");
                pnlRackCreate.Visible = false;
                flpManualRacks.Controls.Clear();
                ArrangeLayout();
                LoadRacks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Rack creation failed after " + successCount.ToString() + " rack(s). " + ex.Message);
                LoadRacks();
            }
        }

        private List<string> BuildRackNames()
        {
            List<string> rackNames = new List<string>();
            int count = Convert.ToInt32(nudRackCount.Value);

            if (rdoAuto.Checked)
            {
                string prefix = txtPrefix.Text.Trim();
                if (prefix.Length == 0)
                {
                    MessageBox.Show("Prefix Should Not Be Empty");
                    txtPrefix.Focus();
                    return rackNames;
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
                                return new List<string>();
                            }
                            rackNames.Add(caption);
                        }
                    }
                }
            }

            return rackNames;
        }

        private void LoadRacks()
        {
            flpRacks.SuspendLayout();
            flpRacks.Controls.Clear();

            try
            {
                DataTable racks = repository.GetRacks(locationId);
                foreach (DataRow row in racks.Rows)
                {
                    AddRackItem(
                        Convert.ToInt32(row["RackId"]),
                        Convert.ToString(row["RackCaption"]),
                        GetBooleanColumnValue(row, "IsAssigned"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load racks. " + ex.Message);
            }
            finally
            {
                flpRacks.ResumeLayout();
                ArrangeLayout();
            }
        }

        private void ArrangeLayout()
        {
            int contentWidth = this.ClientSize.Width - 24;
            if (contentWidth < 560)
            {
                contentWidth = 560;
            }

            pnlLocationHeader.Width = this.ClientSize.Width;
            ArrangeLocationHeader(contentWidth);
            pnlRackCreate.Width = contentWidth;
            flpRacks.Width = contentWidth;
            flpRacks.MinimumSize = new Size(contentWidth, 1);
            flpRacks.MaximumSize = new Size(contentWidth, 0);
            flpRacks.AutoScroll = false;
            flpRacks.WrapContents = true;
            flpRacks.PerformLayout();
            flpRacks.Height = Math.Max(1, flpRacks.PreferredSize.Height);

            if (pnlRackCreate.Visible)
            {
                flpRacks.Top = pnlRackCreate.Bottom + 10;
            }
            else
            {
                flpRacks.Top = pnlLocationHeader.Bottom + 10;
            }

            btnAddMoreRacks.Top = flpRacks.Bottom + 10;
            this.Height = btnAddMoreRacks.Bottom + 10;
        }

        private void ArrangeLocationHeader(int contentWidth)
        {
            int right = this.ClientSize.Width - 15;
            if (right < 585)
            {
                right = 585;
            }

            btnAddRacks.Left = right - btnAddRacks.Width;
            btnCancelLocation.Left = btnAddRacks.Left - btnCancelLocation.Width - 6;
            btnSaveLocation.Left = btnCancelLocation.Left - btnSaveLocation.Width - 6;
            btnEditLocation.Left = btnAddRacks.Left - btnEditLocation.Width - 6;

            int actionLeft = btnSaveLocation.Visible ? btnSaveLocation.Left : btnEditLocation.Left;
            int fieldRight = actionLeft - 12;
            int priorityWidth = 170;
            int locationLabelWidth = 72;
            int locationTextLeft = 13 + locationLabelWidth + 8;
            int displayLocationWidth = fieldRight - 13 - priorityWidth - 12;
            int editLocationWidth = fieldRight - locationTextLeft - priorityWidth - 12;

            if (displayLocationWidth < 220)
            {
                displayLocationWidth = Math.Min(contentWidth - priorityWidth - 12, 320);
                if (displayLocationWidth < 180)
                {
                    displayLocationWidth = 180;
                }
            }

            if (editLocationWidth < 220)
            {
                editLocationWidth = Math.Min(contentWidth - locationLabelWidth - priorityWidth - 20, 320);
                if (editLocationWidth < 180)
                {
                    editLocationWidth = 180;
                }
            }

            lblLocationEditName.Left = 13;
            txtLocationName.Left = locationTextLeft;
            lblLocationName.Width = displayLocationWidth;
            txtLocationName.Width = editLocationWidth;

            lblLocationPriority.Left = lblLocationName.Right + 12;
            lblPriority.Left = txtLocationName.Right + 12;
            nudDisplayOrder.Left = lblPriority.Right + 8;
        }

        private void AddRackItem(int rackId, string rackCaption, bool isAssigned)
        {
            Panel panel = new Panel();
            panel.Tag = rackId;
            panel.Width = 175;
            panel.Height = 32;
            panel.Margin = new Padding(0, 0, 8, 8);
            panel.BackColor = isAssigned ? AssignedRackBackColor : Color.Gainsboro;
            panel.BorderStyle = BorderStyle.FixedSingle;

            Label label = new Label();
            label.Name = "lblRackCaption";
            label.Text = rackCaption;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Font = new Font("Calibri", 9.75F);
            label.ForeColor = Color.Black;
            label.Location = new Point(4, 5);
            label.Size = new Size(87, 20);

            TextBox textBox = new TextBox();
            textBox.Name = "txtRackCaption";
            textBox.Text = rackCaption;
            textBox.Font = new Font("Arial", 8.25F);
            textBox.Location = new Point(4, 5);
            textBox.Size = new Size(87, 20);
            textBox.MaxLength = 100;
            textBox.Visible = false;

            Button editButton = new Button();
            editButton.Name = "btnRackEdit";
            editButton.Text = "Edit";
            editButton.Location = new Point(96, 3);
            editButton.Size = new Size(35, 24);
            editButton.Tag = panel;
            ApplyQuotationButtonStyle(editButton);
            editButton.Click += new EventHandler(btnRackEdit_Click);

            Button saveButton = new Button();
            saveButton.Name = "btnRackSave";
            saveButton.Text = "Save";
            saveButton.Location = new Point(96, 3);
            saveButton.Size = new Size(39, 24);
            saveButton.Tag = panel;
            saveButton.Visible = false;
            ApplyQuotationButtonStyle(saveButton);
            saveButton.Click += new EventHandler(btnRackSave_Click);

            Button cancelButton = new Button();
            cancelButton.Name = "btnRackCancel";
            cancelButton.Text = "X";
            cancelButton.Location = new Point(137, 3);
            cancelButton.Size = new Size(30, 24);
            cancelButton.Tag = panel;
            cancelButton.Visible = false;
            ApplyQuotationButtonStyle(cancelButton);
            cancelButton.Click += new EventHandler(btnRackCancel_Click);

            panel.Controls.Add(label);
            panel.Controls.Add(textBox);
            panel.Controls.Add(editButton);
            panel.Controls.Add(saveButton);
            panel.Controls.Add(cancelButton);
            flpRacks.Controls.Add(panel);
            ArrangeLayout();
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

        private void ApplyQuotationButtonStyle(Button button)
        {
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.BorderSize = 0;
            button.FlatStyle = FlatStyle.Popup;
            button.Font = new Font("Calibri", 9.75F);
            button.UseVisualStyleBackColor = true;
        }

        private void btnRackEdit_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Panel panel = button.Tag as Panel;
            SetRackEditMode(panel, true);
        }

        private void btnRackSave_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Panel panel = button.Tag as Panel;
            TextBox textBox = panel.Controls["txtRackCaption"] as TextBox;
            Label label = panel.Controls["lblRackCaption"] as Label;
            string caption = textBox.Text.Trim();

            if (caption.Length == 0)
            {
                MessageBox.Show("Rack caption should not be empty");
                textBox.Focus();
                return;
            }

            try
            {
                repository.UpdateRack(Convert.ToInt32(panel.Tag), caption, repository.GetCurrentUserId());
                label.Text = caption;
                textBox.Text = caption;
                SetRackEditMode(panel, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Rack update failed. " + ex.Message);
                textBox.Focus();
            }
        }

        private void btnRackCancel_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            Panel panel = button.Tag as Panel;
            Label label = panel.Controls["lblRackCaption"] as Label;
            TextBox textBox = panel.Controls["txtRackCaption"] as TextBox;
            textBox.Text = label.Text;
            SetRackEditMode(panel, false);
        }

        private void SetRackEditMode(Panel panel, bool editing)
        {
            if (panel == null)
            {
                return;
            }

            panel.Controls["lblRackCaption"].Visible = !editing;
            panel.Controls["txtRackCaption"].Visible = editing;
            panel.Controls["btnRackEdit"].Visible = !editing;
            panel.Controls["btnRackSave"].Visible = editing;
            panel.Controls["btnRackCancel"].Visible = editing;

            if (editing)
            {
                TextBox textBox = panel.Controls["txtRackCaption"] as TextBox;
                textBox.Focus();
                textBox.SelectAll();
            }
        }
    }
}
