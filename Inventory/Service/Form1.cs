using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Service
{
    public partial class Form1 : Form
    {

        DataTable dtAdd = new DataTable();
        bool first = true;
        ServiceRequestBAL objServiceRequestBAL = new ServiceRequestBAL();
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            pnlLabelSearch.Visible = true;
            // vLabel1.Visible = true;
            pnlSearch.Visible = false;
            splitContainer1.Panel1Collapsed = true;
            this.ActiveControl = TxtInvoiceOrdNo;
            BindServiceRequest();
        }
        private void ServiceRequest_Load(object sender, EventArgs e)
        {

        }

        private void vLabel3_Click(object sender, EventArgs e)
        {
            if (pnlLabelSearch.Visible == true)
            {
                pnlLabelSearch.Visible = false;
                //vLabel1.Visible = false;
                pnlSearch.Visible = true;
                splitContainer1.Panel1Collapsed = false;
                //cbxSearchOrderNo.Focus();
            }
        }

        private void vLabel2_Click(object sender, EventArgs e)
        {
            if (pnlOrder.Visible == true)
            {
                pnlOrder.Visible = false;
                vLabel2.Visible = false;
                pnlCollapse2.Visible = true;
                splitContainer1.Panel2Collapsed = false;
                pbxCollapse.Visible = true;
                pbxRightCollapse.Visible = true;

                pnlLabelSearch.Visible = true;
                // vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;

                //this.dgvSearch.Columns["ItemCode"].Visible = false;
                //this.dgvSearch.Columns["BarCode"].Visible = false;
                //this.dgvSearch.Columns["Brandname"].Visible = false;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtsearch1.Text) || !string.IsNullOrEmpty(txtsearch2.Text))
            {
                Searchresult();
                txtsearch1.Text = string.Empty;
                txtsearch2.Text = string.Empty;
            }
        }
        public void Searchresult()
        {
            dgvSearch.DataSource = null;
            DataTable dt = objServiceRequestBAL.SearchData(txtsearch1.Text, txtsearch2.Text,"");
            dgvSearch.DataSource = dt;
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            dgvSearch.Columns["ServiceReqId"].Visible = false;
            dgvSearch.Columns["SalesOrderno"].HeaderText = "Sales Order No";
        }

        private void cbxSearchOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cbxSearchOrderDate_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cbxVendor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pbxCollapse_Click(object sender, EventArgs e)
        {
            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                //vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;
            }
        }

        private void pbxRightCollapse_Click(object sender, EventArgs e)
        {
            if (pnlCollapse2.Visible == true)
            {
                pnlOrder.Visible = true;
                vLabel2.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;

                //this.dgvSearch.Columns["ItemCode"].Visible = true;
                //this.dgvSearch.Columns["BarCode"].Visible = true;
                //this.dgvSearch.Columns["Brandname"].Visible = true;
                //dgvSearch.Columns["BarCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void Btnprint_Click(object sender, EventArgs e)
        {
            preview();
        }

        public void preview()
        {
            try
            {
                //int c = Convert.ToInt32(1);
                //Purchasereport pos = new Purchasereport(c);
                //pos.Show();
            }
            catch
            {
                MessageBox.Show("Please Select The Item");
            }
        }

        private void btnnew_Click(object sender, EventArgs e)
        {
            clear();
        }
        public bool SaveValidation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(TxtInvoiceOrdNo.Text))
            {
                i++;
                message = message + "* Sales Order Number Should Not Be Empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtqty;
            }
            DataTable dt = new DataTable();
            dt = (DataTable)dgvProductqty.DataSource;
            if (dt != null)
            {
                if (dt.Rows.Count <= 0)
                {
                    i++;
                    message = message + "* Please add Product" + "\n";
                    if (i == 1)
                        this.ActiveControl = cbxProduct;
                }
            }
            else
            {
                i++;
                message = message + "* Please add Product" + "\n";
                if (i == 1)
                    this.ActiveControl = cbxProduct;
            }
            if (string.IsNullOrEmpty(message))
            {
                status = true;
            }
            else
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

        private void btnProductSave_Click(object sender, EventArgs e)
        {
            bool Val = SaveValidation();
            if (Val == true)
            {
                SaveServicerequest();
                clear();
                BindServiceRequest();
            }
        }
        public void BindServiceRequest()
        {
            DataTable dt = objServiceRequestBAL.GetServiceRequest();
            dgvserviceProductlist.Columns.Clear();
            dgvserviceProductlist.DataSource = dt;
            dgvserviceProductlist.Columns["ServiceReqId"].Visible = false;
            dgvserviceProductlist.Columns["IsDeleted"].Visible = false;
            DataGridViewImageColumn img1 = new DataGridViewImageColumn();
            img1.Image = Inventory.Properties.Resources.user_edit;
            dgvserviceProductlist.Columns.Insert(9, img1);
            img1.HeaderText = "Edit";
            img1.Name = "Edit";

            DataGridViewImageColumn img2 = new DataGridViewImageColumn();
            img2.Image = Inventory.Properties.Resources.trash;
            dgvserviceProductlist.Columns.Insert(10, img2);
            img2.HeaderText = "Delete";
            img2.Name = "Delete";
            dgvserviceProductlist.Columns["Edit"].Width = 40;
            dgvserviceProductlist.Columns["Delete"].Width = 50;
            dgvserviceProductlist.Columns["SalesOrderno"].HeaderText = "Sales Order No";
            dgvserviceProductlist.Columns["Noofproducts"].HeaderText = "No of products";
            dgvserviceProductlist.Columns["Updatestatus"].HeaderText = "Status";
        }
        public void SaveServicerequest()
        {
            DataTable dt = new DataTable();
            dt = (DataTable)dgvProductqty.DataSource;
            objServiceRequestBAL.SaveServceRequest(lblservicerequestid.Text.Trim(), TxtInvoiceOrdNo.Text.Trim(), txtcustname.Text.Trim(), dt);
        }

        private void btnsaveaspending_Click(object sender, EventArgs e)
        {
            //bool Val = Validationpending();
            //if (Val == true)
            //{
            //    InsertUpdatepending();
            //}
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            clear();
        }
        public void clear()
        {
            lblservicerequestid.Text = string.Empty;
            TxtInvoiceOrdNo.Text = string.Empty;
            txtcustname.Text = string.Empty;

            txtcomplients.Enabled = false;
            txtqty.Enabled = false;
            cbxProduct.Enabled = false;
            if (btnAdd.Enabled)
            {
                try
                {
                    txtqty.Text = string.Empty;
                    txtcomplients.Text = string.Empty;
                    cbxProduct.SelectedIndex = 0;
                    cbxProduct.DataSource = null;
                    if (dtAdd.Rows.Count > 0)
                    {
                        dtAdd.Rows.Clear();
                    }
                    dgvProductqty.DataSource = dtAdd;
                }
                catch
                {

                }
            }
            btnAdd.Enabled = false;

        }
        public bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;
            if (cbxProduct.SelectedIndex == 0)
            {
                i++;
                message = message + "* Please select the Product" + "\n";
                if (i == 1)
                    this.ActiveControl = cbxProduct;
            }
            if (string.IsNullOrEmpty(Convert.ToString(txtqty.Text)))
            {
                i++;
                message = message + "* QTY Should Not Be Empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtqty;
            }
            if (string.IsNullOrEmpty(message))
            {
                status = true;
            }
            else
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool Val = Validation();
            if (Val == true)
            {
                if (dtAdd.Rows.Count == 0)
                {
                    dtAdd.Columns.Add("ProductId", typeof(string));
                    dtAdd.Columns.Add("Product", typeof(string));
                    dtAdd.Columns.Add("QTY", typeof(string));
                    dtAdd.Columns.Add("Complients", typeof(string));
                    DataRow dr = dtAdd.NewRow();
                    dr["ProductId"] = cbxProduct.SelectedValue;
                    dr["Product"] = cbxProduct.Text;
                    dr["QTY"] = txtqty.Text;
                    dr["Complients"] = txtcomplients.Text;
                    dtAdd.Rows.Add(dr);

                }
                else
                {
                    DataRow dr = dtAdd.NewRow();
                    dr["ProductId"] = cbxProduct.SelectedValue;
                    dr["Product"] = cbxProduct.Text;
                    dr["QTY"] = txtqty.Text;
                    dr["Complients"] = txtcomplients.Text;
                    dtAdd.Rows.Add(dr);
                }
                gridaddbind();
            }
        }
        public void gridaddbind()
        {
            dgvProductqty.DataSource = dtAdd;
            if (first)
            {
                DataGridViewImageColumn img2 = new DataGridViewImageColumn();
                img2.Image = Inventory.Properties.Resources.trash;
                dgvProductqty.Columns.Insert(4, img2);
                img2.HeaderText = "Delete";
                img2.Name = "Delete";
                dgvProductqty.Columns["Delete"].Width = 50;
                dgvProductqty.Columns["ProductId"].Visible = false;
                first = false;
            }
            txtqty.Text = string.Empty;
            txtcomplients.Text = string.Empty;
            cbxProduct.SelectedIndex = 0;
        }
        private void TxtInvoiceOrdNo_Leave(object sender, EventArgs e)
        {
            serviceordernumberchange();
        }
        public void serviceordernumberchange()
        {
            if (objServiceRequestBAL.checksalesbillnumber(TxtInvoiceOrdNo.Text.Trim()))
            {
                cbxProduct.Enabled = true;
                txtqty.Enabled = true;
                txtcomplients.Enabled = true;
                btnAdd.Enabled = true;
                DataTable dt = objServiceRequestBAL.GetProduct(TxtInvoiceOrdNo.Text.Trim());

                DataRow row = dt.NewRow();
                row["Productid"] = "0";
                row["DisplayName"] = "-Select-";
                dt.Rows.InsertAt(row, 0);
                cbxProduct.DataSource = dt;
                cbxProduct.ValueMember = "Productid";
                cbxProduct.DisplayMember = "DisplayName";
                cbxProduct.SelectedIndex = 0;
            }
            else
            {
                cbxProduct.Enabled = false;
                txtqty.Enabled = false;
                txtcomplients.Enabled = false;
                btnAdd.Enabled = false;

                // MessageBox.Show("* Error" + "\n" + "----------------------------------------" + "\n" + "Invalid Sales Order Number");               
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (txtcustname.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    if (cbxProduct.Enabled)
                    {
                        this.ActiveControl = cbxProduct;
                        return true;
                    }
                    else
                    {
                        this.ActiveControl = TxtInvoiceOrdNo;
                        return true;
                    }

                }

            }
            if (btnAdd.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnProductSave;
                    return true;
                }
            }



            if (keyData == Keys.Escape)
            {

                if (dgvProductqty.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dgvProductqty.Rows[0].Cells[1].Value)))
                    {
                        DialogResult result = MessageBox.Show("Do you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            this.Close();
                        }
                    }
                    else
                    {
                        this.Close();
                    }
                
                return true;
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void dgvserviceProductlist_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvserviceProductlist.Columns[e.ColumnIndex].HeaderText == "Edit")
                {
                    lblservicerequestid.Text = Convert.ToString(dgvserviceProductlist.Rows[e.RowIndex].Cells["ServiceReqId"].Value);
                    TxtInvoiceOrdNo.Text = Convert.ToString(dgvserviceProductlist.Rows[e.RowIndex].Cells["SalesOrderno"].Value);
                    txtcustname.Text = Convert.ToString(dgvserviceProductlist.Rows[e.RowIndex].Cells["Customer"].Value);
                    dtAdd = objServiceRequestBAL.GetserviceProduct(lblservicerequestid.Text.Trim(), Program.Service);
                    dgvProductqty.DataSource = dtAdd;
                    if (first)
                    {
                        DataGridViewImageColumn img2 = new DataGridViewImageColumn();
                        img2.Image = Inventory.Properties.Resources.trash;
                        dgvProductqty.Columns.Insert(4, img2);
                        img2.HeaderText = "Delete";
                        img2.Name = "Delete";
                        dgvProductqty.Columns["Delete"].Width = 50;
                        dgvProductqty.Columns["ProductId"].Visible = false;
                        first = false;
                    }
                    serviceordernumberchange();
                }
                else if (dgvserviceProductlist.Columns[e.ColumnIndex].HeaderText == "Delete")
                {
                    string namestatus = Convert.ToString(dgvserviceProductlist.Rows[e.RowIndex].Cells["ItemName"].Value);
                    DialogResult result = MessageBox.Show("Are you sure delete Product " + namestatus + "?", "Delete Product?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.Yes))
                    {
                        lblservicerequestid.Text = Convert.ToString(dgvserviceProductlist.Rows[e.RowIndex].Cells["id"].Value);
                        int i = objServiceRequestBAL.DeleteServiceRequest(lblservicerequestid.Text.Trim());
                        if (i == 1)
                        {
                            MessageBox.Show("Delete Successfully");
                            clear();
                            BindServiceRequest();
                        }
                    }
                }
            }
        }

        private void dgvProductqty_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dtAdd.Rows.Remove(dtAdd.Rows[e.RowIndex]);
            gridaddbind();
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lblservicerequestid.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ServiceReqId"].Value);
                TxtInvoiceOrdNo.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["SalesOrderno"].Value);
                txtcustname.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Customer"].Value);
                dtAdd = objServiceRequestBAL.GetserviceProduct(lblservicerequestid.Text.Trim(), Program.Service);
                dgvProductqty.DataSource = dtAdd;
                if (first)
                {
                    DataGridViewImageColumn img2 = new DataGridViewImageColumn();
                    img2.Image = Inventory.Properties.Resources.trash;
                    dgvProductqty.Columns.Insert(4, img2);
                    img2.HeaderText = "Delete";
                    img2.Name = "Delete";
                    dgvProductqty.Columns["Delete"].Width = 50;
                    dgvProductqty.Columns["ProductId"].Visible = false;
                    first = false;
                }
                serviceordernumberchange();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
