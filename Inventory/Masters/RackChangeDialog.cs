using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Masters
{
    public partial class RackChangeDialog : Form
    {
        private readonly LocationRackRepository repository;
        private readonly int currentRackId;
        private readonly string currentRackCaption;

        public int SelectedRackId { get; private set; }

        public RackChangeDialog(LocationRackRepository repository, int currentRackId, string currentRackCaption)
        {
            InitializeComponent();
            this.repository = repository;
            this.currentRackId = currentRackId;
            this.currentRackCaption = currentRackCaption;
        }

        private void RackChangeDialog_Load(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            lblCurrentRackValue.Text = currentRackCaption;
            LoadLocations();
        }

        private void LoadLocations()
        {
            try
            {
                DataTable locations = repository.GetLocations();
                cboLocations.DisplayMember = "LocationName";
                cboLocations.ValueMember = "LocationId";
                cboLocations.DataSource = locations;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load locations. " + ex.Message);
            }
        }

        private void cboLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLocations.SelectedValue == null || cboLocations.SelectedValue == DBNull.Value)
            {
                return;
            }

            int locationId;
            if (!int.TryParse(Convert.ToString(cboLocations.SelectedValue), out locationId))
            {
                return;
            }

            LoadRacks(locationId);
        }

        private void LoadRacks(int locationId)
        {
            try
            {
                DataTable racks = repository.GetRacks(locationId);
                cboRacks.DisplayMember = "RackCaption";
                cboRacks.ValueMember = "RackId";
                cboRacks.DataSource = racks;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load racks. " + ex.Message);
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (cboRacks.SelectedValue == null || cboRacks.SelectedValue == DBNull.Value)
            {
                MessageBox.Show("Select valid rack");
                return;
            }

            int rackId;
            if (!int.TryParse(Convert.ToString(cboRacks.SelectedValue), out rackId))
            {
                MessageBox.Show("Select valid rack");
                return;
            }

            if (rackId == currentRackId)
            {
                MessageBox.Show("Select a different rack.");
                return;
            }

            SelectedRackId = rackId;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
