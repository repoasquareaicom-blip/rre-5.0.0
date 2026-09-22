using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Masters
{
    public partial class ProductLocationRackAssignmentControl : UserControl
    {
        private readonly List<RackSelectorItem> racks = new List<RackSelectorItem>();
        private readonly Dictionary<int, RackSelectorItem> selectedRacks = new Dictionary<int, RackSelectorItem>();
        private bool loadingSelector;
        private bool adjustingLayout;
        private int lastLayoutWidth;
        private static readonly Color AssignedRackBackColor = Color.FromArgb(226, 246, 226);

        public ProductLocationRackAssignmentControl()
        {
            InitializeComponent();
            cmbRackSelector.DrawMode = DrawMode.OwnerDrawFixed;
            cmbRackSelector.DrawItem += new DrawItemEventHandler(cmbRackSelector_DrawItem);
            pnlRackSelector.Visible = false;
            btnShowAddRacks.Visible = true;
            UpdateHeight();
        }

        public int LocationId { get; private set; }

        public string LocationName
        {
            get { return lblLocationName.Text; }
        }

        public List<int> SelectedRackIds
        {
            get
            {
                return new List<int>(selectedRacks.Keys);
            }
        }

        public List<string> SelectedRackCaptions
        {
            get
            {
                List<string> captions = new List<string>();
                for (int i = 0; i < racks.Count; i++)
                {
                    if (selectedRacks.ContainsKey(racks[i].RackId))
                    {
                        captions.Add(racks[i].RackCaption);
                    }
                }

                return captions;
            }
        }

        public void LoadLocation(int locationId, string locationName, DataTable rackTable)
        {
            LocationId = locationId;
            lblLocationName.Text = locationName;
            racks.Clear();
            selectedRacks.Clear();
            pnlRackSelector.Visible = false;
            btnShowAddRacks.Visible = true;

            if (rackTable != null)
            {
                for (int i = 0; i < rackTable.Rows.Count; i++)
                {
                    int rackId;
                    if (!int.TryParse(Convert.ToString(rackTable.Rows[i]["RackId"]), out rackId))
                    {
                        continue;
                    }

                    racks.Add(new RackSelectorItem(
                        rackId,
                        Convert.ToString(rackTable.Rows[i]["RackCaption"]),
                        GetBooleanColumnValue(rackTable.Rows[i], "IsAssigned")));
                }
            }

            ArrangeCompactLayout();
            RefreshSelector();
            RenderSelectedRacks();
        }

        public void AddSelectedRack(int rackId, string rackCaption)
        {
            if (rackId <= 0 || selectedRacks.ContainsKey(rackId))
            {
                return;
            }

            RackSelectorItem item = FindRack(rackId);
            if (item == null)
            {
                item = new RackSelectorItem(rackId, rackCaption, false);
            }

            selectedRacks.Add(item.RackId, item);
            RefreshSelector();
            RenderSelectedRacks();
        }

        public void ClearSelections()
        {
            selectedRacks.Clear();
            RefreshSelector();
            RenderSelectedRacks();
        }

        private void cmbRackSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (loadingSelector)
            {
                return;
            }
        }

        private void cmbRackSelector_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            RackSelectorItem item = cmbRackSelector.Items[e.Index] as RackSelectorItem;
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backColor = selected ? SystemColors.Highlight : (item != null && item.IsAssigned ? AssignedRackBackColor : e.BackColor);
            Color foreColor = selected ? SystemColors.HighlightText : e.ForeColor;

            using (SolidBrush backBrush = new SolidBrush(backColor))
            using (SolidBrush foreBrush = new SolidBrush(foreColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
                string text = item == null ? string.Empty : item.RackCaption;
                e.Graphics.DrawString(text, e.Font, foreBrush, e.Bounds.Left + 2, e.Bounds.Top + 2);
            }

            e.DrawFocusRectangle();
        }

        private void cmbRackSelector_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddRackFromSelector();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void AddRackFromSelector()
        {
            Debug.WriteLine("AddRack start");
            if (loadingSelector)
            {
                Debug.WriteLine("AddRack skipped: selector loading");
                return;
            }

            RackSelectorItem item = cmbRackSelector.SelectedItem as RackSelectorItem;
            if (item == null)
            {
                item = FindRackByCaption(cmbRackSelector.Text);
            }

            if (item == null || selectedRacks.ContainsKey(item.RackId))
            {
                Debug.WriteLine("AddRack skipped: no rack or duplicate");
                cmbRackSelector.Text = string.Empty;
                return;
            }

            Debug.WriteLine("RackId selected: " + item.RackId);
            selectedRacks.Add(item.RackId, item);
            Debug.WriteLine("Selected collection updated");
            RefreshSelector();
            RenderSelectedRacks();
            Debug.WriteLine("AddRack end");
        }

        private void btnShowAddRacks_Click(object sender, EventArgs e)
        {
            pnlRackSelector.Visible = true;
            btnShowAddRacks.Visible = false;
            cmbRackSelector.Text = string.Empty;
            cmbRackSelector.SelectedIndex = -1;
            UpdateHeight();
            cmbRackSelector.Focus();
        }

        private void btnAddRack_Click(object sender, EventArgs e)
        {
            AddRackFromSelector();
        }

        private void btnCancelAddRack_Click(object sender, EventArgs e)
        {
            cmbRackSelector.Text = string.Empty;
            cmbRackSelector.SelectedIndex = -1;
            pnlRackSelector.Visible = false;
            btnShowAddRacks.Visible = true;
            UpdateHeight();
        }

        private void RefreshSelector()
        {
            Debug.WriteLine("RefreshAvailableRacks start");
            loadingSelector = true;
            AutoCompleteMode previousAutoCompleteMode = cmbRackSelector.AutoCompleteMode;
            try
            {
                cmbRackSelector.BeginUpdate();
                cmbRackSelector.AutoCompleteMode = AutoCompleteMode.None;
                cmbRackSelector.SelectedIndex = -1;
                cmbRackSelector.Text = string.Empty;
                cmbRackSelector.Items.Clear();
                cmbRackSelector.AutoCompleteCustomSource.Clear();

                for (int i = 0; i < racks.Count; i++)
                {
                    RackSelectorItem item = racks[i];
                    if (selectedRacks.ContainsKey(item.RackId))
                    {
                        continue;
                    }

                    cmbRackSelector.Items.Add(item);
                    cmbRackSelector.AutoCompleteCustomSource.Add(item.RackCaption);
                }

                cmbRackSelector.SelectedIndex = -1;
                cmbRackSelector.Text = string.Empty;
            }
            finally
            {
                cmbRackSelector.AutoCompleteMode = previousAutoCompleteMode;
                cmbRackSelector.EndUpdate();
                loadingSelector = false;
                Debug.WriteLine("RefreshAvailableRacks end");
            }
        }

        private void RenderSelectedRacks()
        {
            Debug.WriteLine("RenderSelectedRacks start");
            flpSelectedRacks.SuspendLayout();
            try
            {
                flpSelectedRacks.Controls.Clear();

                List<RackSelectorItem> selectedRackSnapshot = new List<RackSelectorItem>(selectedRacks.Values);
                for (int i = 0; i < selectedRackSnapshot.Count; i++)
                {
                    flpSelectedRacks.Controls.Add(CreateRackChip(selectedRackSnapshot[i]));
                }
            }
            finally
            {
                flpSelectedRacks.ResumeLayout();
            }

            UpdateHeight();
            Debug.WriteLine("RenderSelectedRacks end");
        }

        private Control CreateRackChip(RackSelectorItem item)
        {
            Panel chip = new Panel();
            chip.AutoSize = true;
            chip.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            chip.BackColor = Color.White;
            chip.BorderStyle = BorderStyle.FixedSingle;
            chip.Margin = new Padding(0, 2, 6, 4);
            chip.Padding = new Padding(8, 3, 2, 3);

            Label caption = new Label();
            caption.AutoSize = true;
            caption.Font = new Font("Calibri", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            caption.ForeColor = Color.FromArgb(64, 64, 64);
            caption.Location = new Point(8, 5);
            caption.Text = item.RackCaption;

            Button remove = new Button();
            remove.Cursor = Cursors.Hand;
            remove.FlatAppearance.BorderSize = 0;
            remove.FlatStyle = FlatStyle.Popup;
            remove.Font = new Font("Calibri", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            remove.ForeColor = Color.Firebrick;
            remove.Location = new Point(caption.Right + 6, 2);
            remove.Margin = new Padding(0);
            remove.Size = new Size(22, 22);
            remove.TabStop = false;
            remove.Tag = item.RackId;
            remove.Text = "X";
            remove.UseVisualStyleBackColor = true;
            remove.Click += btnRemoveRack_Click;

            chip.Controls.Add(caption);
            chip.Controls.Add(remove);
            chip.Size = new Size(caption.Width + remove.Width + 18, 28);
            remove.Location = new Point(caption.Right + 6, 2);

            return chip;
        }

        private void btnRemoveRack_Click(object sender, EventArgs e)
        {
            Button remove = sender as Button;
            if (remove == null || remove.Tag == null)
            {
                return;
            }

            int rackId;
            if (!int.TryParse(Convert.ToString(remove.Tag), out rackId))
            {
                return;
            }

            if (selectedRacks.ContainsKey(rackId))
            {
                selectedRacks.Remove(rackId);
                RefreshSelector();
                RenderSelectedRacks();
            }
        }

        private RackSelectorItem FindRack(int rackId)
        {
            for (int i = 0; i < racks.Count; i++)
            {
                if (racks[i].RackId == rackId)
                {
                    return racks[i];
                }
            }

            return null;
        }

        private RackSelectorItem FindRackByCaption(string rackCaption)
        {
            if (string.IsNullOrEmpty(rackCaption))
            {
                return null;
            }

            for (int i = 0; i < racks.Count; i++)
            {
                if (selectedRacks.ContainsKey(racks[i].RackId))
                {
                    continue;
                }

                if (string.Equals(racks[i].RackCaption, rackCaption.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return racks[i];
                }
            }

            return null;
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

        private void ProductLocationRackAssignmentControl_SizeChanged(object sender, EventArgs e)
        {
            if (lastLayoutWidth == ClientSize.Width)
            {
                return;
            }

            ArrangeCompactLayout();
        }

        private void UpdateHeight()
        {
            Debug.WriteLine("RecalculateLayout start");
            if (adjustingLayout)
            {
                Debug.WriteLine("RecalculateLayout skipped: already adjusting");
                return;
            }

            adjustingLayout = true;
            try
            {
                int selectedHeight = Math.Max(32, flpSelectedRacks.PreferredSize.Height + 4);
                flpSelectedRacks.Top = pnlRackSelector.Visible ? pnlRackSelector.Bottom + 4 : 30;
                flpSelectedRacks.Height = selectedHeight;
                int newHeight = flpSelectedRacks.Bottom + 8;
                if (Height != newHeight)
                {
                    Height = newHeight;
                }
            }
            finally
            {
                adjustingLayout = false;
                Debug.WriteLine("RecalculateLayout end");
            }
        }

        private void ArrangeCompactLayout()
        {
            if (adjustingLayout)
            {
                return;
            }

            adjustingLayout = true;
            try
            {
            int addButtonWidth = 92;
            btnShowAddRacks.Width = addButtonWidth;
            btnShowAddRacks.Left = Math.Max(0, ClientSize.Width - addButtonWidth);
            lblLocationName.Width = Math.Max(80, btnShowAddRacks.Left - 8);

            int selectorWidth = Math.Min(250, Math.Max(120, ClientSize.Width - btnAddRack.Width - btnCancelAddRack.Width - 18));
            cmbRackSelector.Width = selectorWidth;
            btnAddRack.Left = cmbRackSelector.Right + 6;
            btnCancelAddRack.Left = btnAddRack.Right + 5;
            pnlRackSelector.Width = btnCancelAddRack.Right;

            flpSelectedRacks.Width = ClientSize.Width;
            flpSelectedRacks.MinimumSize = new Size(ClientSize.Width, 1);
            flpSelectedRacks.MaximumSize = new Size(ClientSize.Width, 0);
                lastLayoutWidth = ClientSize.Width;
            }
            finally
            {
                adjustingLayout = false;
            }

            UpdateHeight();
        }

        private class RackSelectorItem
        {
            public RackSelectorItem(int rackId, string rackCaption, bool isAssigned)
            {
                RackId = rackId;
                RackCaption = rackCaption;
                IsAssigned = isAssigned;
            }

            public int RackId { get; private set; }
            public string RackCaption { get; private set; }
            public bool IsAssigned { get; private set; }

            public override string ToString()
            {
                return RackCaption;
            }
        }
    }
}
