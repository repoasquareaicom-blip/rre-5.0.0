using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory.Masters
{
    public partial class RackProductList : Form
    {
        private readonly LocationRackRepository repository;
        private readonly ProductRackMappingRepository mappingRepository = new ProductRackMappingRepository();
        private readonly int rackId;
        private readonly string rackCaption;

        public event EventHandler RackMappingsChanged;

        public RackProductList(LocationRackRepository repository, int rackId, string rackCaption)
        {
            InitializeComponent();
            this.repository = repository;
            this.rackId = rackId;
            this.rackCaption = rackCaption;
        }

        private void RackProductList_Load(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            lblRackCaption.Text = "Products in Rack - " + rackCaption;
            ConfigureGrid();
            LoadProducts();
        }

        private void ConfigureGrid()
        {
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.AllowUserToDeleteRows = false;
            dgvProducts.AllowUserToResizeRows = false;
            dgvProducts.MultiSelect = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.ReadOnly = true;
            dgvProducts.RowTemplate.Height = 24;
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.BorderStyle = BorderStyle.FixedSingle;
            dgvProducts.GridColor = Color.Gainsboro;
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 245, 252);
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 9.75F, FontStyle.Bold);
            dgvProducts.DefaultCellStyle.Font = new Font("Calibri", 9.75F);
            dgvProducts.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            dgvProducts.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
        }

        private void LoadProducts()
        {
            try
            {
                DataTable products = repository.GetProductsByRack(rackId);
                dgvProducts.DataSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load rack products. " + ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvProducts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            string columnName = dgvProducts.Columns[e.ColumnIndex].Name;
            if (columnName == "colChangeRack")
            {
                ChangeRack(dgvProducts.Rows[e.RowIndex]);
            }
            else if (columnName == "colRemove")
            {
                RemoveProductFromRack(dgvProducts.Rows[e.RowIndex]);
            }
        }

        private void RemoveProductFromRack(DataGridViewRow row)
        {
            DataRowView rowView = row.DataBoundItem as DataRowView;
            if (rowView == null)
            {
                return;
            }

            int productId = Convert.ToInt32(rowView["ProductId"]);
            string productName = Convert.ToString(rowView["ProductName"]);
            DialogResult result = MessageBox.Show(
                "Remove " + productName + " from rack " + rackCaption + "?",
                "Remove Product From Rack",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                mappingRepository.DeleteMapping(productId, rackId);
                RefreshAfterMappingChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Remove product from rack failed. " + ex.Message);
            }
        }

        private void ChangeRack(DataGridViewRow row)
        {
            DataRowView rowView = row.DataBoundItem as DataRowView;
            if (rowView == null)
            {
                return;
            }

            int productId = Convert.ToInt32(rowView["ProductId"]);
            using (RackChangeDialog dialog = new RackChangeDialog(repository, rackId, rackCaption))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                int newRackId = dialog.SelectedRackId;
                if (MappingExists(productId, newRackId))
                {
                    MessageBox.Show("This product is already assigned to the selected rack.");
                    return;
                }

                try
                {
                    mappingRepository.AddMapping(productId, newRackId, repository.GetCurrentUserId());
                    mappingRepository.DeleteMapping(productId, rackId);
                    RefreshAfterMappingChange();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Change rack failed. " + ex.Message);
                }
            }
        }

        private bool MappingExists(int productId, int targetRackId)
        {
            DataTable mappings = mappingRepository.GetMappings(productId);
            foreach (DataRow row in mappings.Rows)
            {
                if (row.Table.Columns.Contains("RackId") && row["RackId"] != DBNull.Value && Convert.ToInt32(row["RackId"]) == targetRackId)
                {
                    return true;
                }
            }

            return false;
        }

        private void RefreshAfterMappingChange()
        {
            LoadProducts();
            if (RackMappingsChanged != null)
            {
                RackMappingsChanged(this, EventArgs.Empty);
            }
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
