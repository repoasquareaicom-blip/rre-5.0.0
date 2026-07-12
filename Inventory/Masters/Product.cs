
using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using System.Windows.Forms;


namespace Inventory.Masters
{
    public partial class Product : Form
    {
        ProductBAL ObjProductBAL = new ProductBAL();
        QuotationBal objQuotationbal = new QuotationBal();
        string sourcepath;
        string filename = null;
        string newnameimage;
        int valindex = 0;
        string aaa;
        //static string imagepath;
        public TextBox tb;
        string filepath = string.Empty;
        string imgpath = string.Empty;
        string conn = Program.connection;
        DataTable dtitems;
        // ComboBox cmblocation;
        public Product()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            if (!BranchAccess.IsMainOffice)
            {
                btnProductSave.Enabled = false;
                btnnew.Enabled = false;
                MessageBox.Show(BranchAccess.MainOfficeOnlyMessage);
            }
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            bindBrand();
            search("itemcode", "", "itemname", "", "ispending", "");
            bindcatogory();
            bindUom();
            bindsizeUom();
            //BindRackName();
            bindproductcolor();

            ddlcategorys.SelectedIndex = 0;
            ddlcolors.SelectedIndex = 0;
            dropmeasure.SelectedIndex = 0;
            dropunitofmeasure.SelectedIndex = 0;
            bindproduct();
            cmbstatus1.SelectedIndex = 0;
            cmbstatus2.SelectedIndex = 0;
            cmbstatus3.SelectedIndex = 0;
            cmbTax.SelectedIndex = 0;
            //if (pnlOrder.Visible == true)
            //{
            //pnlOrder.Visible = false;
            //vLabel2.Visible = false;
            //pnlCollapse2.Visible = true;
            //splitContainer1.Panel2Collapsed = false;
            //pbxCollapse.Visible = true;
            //pbxRightCollapse.Visible = true;
            pnlLabelSearch.Visible = true;
            vLabel1.Visible = true;
            pnlSearch.Visible = false;
            splitContainer1.Panel1Collapsed = true;
            this.dgvSearch.Columns["ItemCode"].Visible = false;
            this.dgvSearch.Columns["BarCode"].Visible = false;
            this.dgvSearch.Columns["Brandname"].Visible = false;
            // }

            //pnlLabelSearch.Visible = true;
            //vLabel1.Visible = true;
            //pnlSearch.Visible = false;
            //splitContainer1.Panel1Collapsed = true;
            //vLabel1.Enabled = false;
            //pnlLabelSearch.Visible = false;
        }
      
        public Button btn;

        private void pbxCollapse_Click(object sender, EventArgs e)
        {
            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;
            }


        }

        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("ItemCode");
            search.Add("ItemName");
            search.Add("status");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderNo.DataSource = bs.DataSource;
            cbxSearchOrderNo.SelectedIndex = 0;

            cmbstatus1.Visible = false;

        }

        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("ItemCode");
            search.Add("ItemName");
            search.Add("status");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderDate.DataSource = bs.DataSource;
            cbxSearchOrderDate.SelectedIndex = 1;
            cmbstatus2.Visible = false;
        }

        private void SearchCreteria3()
        {
            List<string> search = new List<string>();
            search.Add("ItemCode");
            search.Add("ItemName");
            search.Add("status");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxVendor.DataSource = bs.DataSource;
            cbxVendor.SelectedIndex = 2;
            txtsearch3.Visible = false;
        }

        private void cbxSearchOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchOrderNo.SelectedIndex == 2)
            {
                cmbstatus1.Visible = true;
                txtsearch1.Visible = false;
            }
            else
            {
                cmbstatus1.Visible = false;
                txtsearch1.Visible = true;
            }


        }

        private void cbxSearchOrderDate_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbxSearchOrderDate.SelectedIndex == 2)
            {
                cmbstatus2.Visible = true;
                txtsearch2.Visible = false;
            }
            else
            {
                cmbstatus2.Visible = false;
                txtsearch2.Visible = true;
            }
        }

        private void cbxVendor_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbxVendor.SelectedIndex == 2)
            {
                cmbstatus3.Visible = true;
                txtsearch3.Visible = false;
            }
            else
            {
                cmbstatus3.Visible = false;
                txtsearch3.Visible = true;
            }
        }

        private void vLabel1_Click(object sender, EventArgs e)
        {
            if (pnlLabelSearch.Visible == true)
            {
                pnlLabelSearch.Visible = false;
                vLabel1.Visible = false;
                pnlSearch.Visible = true;
                splitContainer1.Panel1Collapsed = false;
                cbxSearchOrderNo.Focus();
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

                this.dgvSearch.Columns["ItemCode"].Visible = true;
                this.dgvSearch.Columns["BarCode"].Visible = true;
                this.dgvSearch.Columns["Brandname"].Visible = true;
                dgvSearch.Columns["BarCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
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
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;

                this.dgvSearch.Columns["ItemCode"].Visible = false;
                this.dgvSearch.Columns["BarCode"].Visible = false;
                this.dgvSearch.Columns["Brandname"].Visible = false;
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


        private void Product_Load(object sender, EventArgs e)
        {
           

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (txtitemnames.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtitemcodes;
            //        return true;
            //    }

            //}
            //if (txtitemcodes.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = chkBoxBarcode;
            //        return true;
            //    }

            //}

            //if (ddlcategorys.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtMinimunStock;
            //        return true;
            //    }

            //}

            //if (cbxSearchOrderNo.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        if (cmbstatus1.Visible == true)
            //        {
            //            this.ActiveControl = cmbstatus1;
            //            return true;
            //        }
            //        else
            //        {
            //            this.ActiveControl = txtsearch1;
            //            return true;
            //        }

            //    }

            //}
            //if (cmbstatus1.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = cbxSearchOrderDate;
            //        return true;
            //    }
            //}
            //if (txtsearch1.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = cbxSearchOrderDate;
            //        return true;
            //    }
            //}
            ////cmbstatus2
            //if (cbxSearchOrderDate.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        if (cmbstatus2.Visible == true)
            //        {
            //            this.ActiveControl = cmbstatus2;
            //            return true;
            //        }
            //        else
            //        {
            //            this.ActiveControl = txtsearch2;
            //            return true;
            //        }

            //    }

            //}
            //if (cmbstatus2.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = cbxVendor;
            //        return true;
            //    }
            //}
            //if (txtsearch2.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = cbxVendor;
            //        return true;
            //    }
            //}
            ////cmbstatus2
            //if (cbxVendor.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        if (cmbstatus3.Visible == true)
            //        {
            //            this.ActiveControl = cmbstatus3;
            //            return true;
            //        }
            //        else
            //        {
            //            this.ActiveControl = txtsearch3;
            //            return true;
            //        }

            //    }

            //}
            //if (cmbstatus3.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = btnSearch;
            //        return true;
            //    }
            //}
            //if (txtsearch3.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = btnSearch;
            //        return true;
            //    }
            //}

            //if (chkBoxBarcode.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        if (txtbarcodes.Visible == true)
            //        {
            //            this.ActiveControl = txtbarcodes;
            //            return true;
            //        }
            //        else
            //        {
            //            this.ActiveControl = txtsizes;
            //            return true;
            //        }
            //    }

            //}
            //if (txtbarcodes.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtsizes;
            //        return true;
            //    }

            //}
            //if (dropunitofmeasure.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = ddlcategorys;
            //        return true;
            //    }

            //}
            //if (txtreorderpt.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = locationinfo;
            //        return true;
            //    }
            //}

            //if (txtsizes.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = dropmeasure;
            //        return true;
            //    }

            //}
            //if (dropmeasure.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = ddlBrands;
            //        return true;
            //    }

            //}
            //if (ddlBrands.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = ddlcolors;
            //        return true;
            //    }

            //}
            //if (ddlcolors.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtRack;
            //        return true;
            //    }

            //}

            //if (txtreorderpt.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtRemarks;
            //        return true;
            //    }

            //}
            //if (txtRemarks.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = butbrowse;
            //        return true;
            //    }

            //}
            //if (butbrowse.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = butclear;
            //        return true;
            //    }

            //}
            if (txtitemnames.Focused)
            {
                if (keyData == (Keys.Tab))
                {

                    txtitemcodes.Focus();
                    return true;
                   
                }

            }

            if (txtitemcodes.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    ddlBrands.Focus();
                    return true;
                }

            }

            if (ddlBrands.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    textBox2.Focus();
                    return true;
                }

            }
            if (textBox2.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    ddlcolors.Focus();
                    return true;
                }

            }

            if (ddlcolors.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    chkBoxBarcode.Focus();
                    return true;
                }

            }

            if (chkBoxBarcode.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    dropunitofmeasure.Focus();
                    return true;
                }

            }

            if (dropunitofmeasure.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    ddlcategorys.Focus();
                    return true;
                }

            }

            if (ddlcategorys.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    txtsizes.Focus();
                    return true;
                }
            }
            if (txtsizes.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    dropmeasure.Focus();
                    return true;
                }
            }

            if (dropmeasure.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    lightValue.Focus();
                    return true;
                }
            }

            if (lightValue.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    chkIncentive.Focus();
                    return true;
                }
            }

            if (chkIncentive.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    txtRack.Focus();
                    return true;
                }
            }
            if (txtRack.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    txtMinimunStock.Focus();
                    return true;
                }
            }

            if (btnnew.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    txtitemnames.Focus();
                    return true;
                }
            }
            if (keyData == (Keys.F3))
            {
                pnlprodsearch.Visible = true;
                this.ActiveControl = txtprodsearch;
                txtprodsearch.SelectionStart = 0;
                txtprodsearch.SelectionLength = txtprodsearch.Text.Length;
                return true;
            }

            if (keyData == (Keys.F5))
            {
                bool Val = Validation();
                if (Val == true)
                {

                    InsertUpdateRightToInformation();


                }
            }


            if (keyData == (Keys.F1))
            {
                clear();
            }


            if (keyData == (Keys.Escape))
            {
                if (pnlprodsearch.Visible)
                {
                pnlprodsearch.Visible = false;
                this.ActiveControl = txtitemcode;
                return true;
                }
                else
                {
                     this.Close();
                        return true;
                   
         
                }
            }


            //if (btnsaveaspending.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = btnclear;
            //        return true;
            //    }

            //}
            //if (btnclear.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = btnnew;
            //        return true;
            //    }

            //}
            //if (btnnew.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtitemnames;
            //        return true;
            //    }

            //}
            //if (txtSalesPrice.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = cmbTax;
            //        return true;
            //    }

            //}
            //if (cmbTax.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtRemarks;
            //        return true;
            //    }

            //}
            //if (Btnprint.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtitemnames;
            //        return true;
            //    }

            //}
            //if(pnlprodsearch.Visible==true)
            //{
            if (keyData == (Keys.Control | Keys.F)) 
            {
                bindproduct();
                return true;
            }
            //}
            //if (keyData == (Keys.A | Keys.Control))
            //{
            //    search("itemcode", "", "itemname", "", "ispending", "");
            //    return true;
            //}

            if (txtprodsearch.Focused)
            {
                if (keyData == (Keys.Enter))
                {
                    
                        ProductSearchKey(txtprodsearch.Text);
                   
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void ProductSearchKey(string ptod)
        {
            dgvProduct.Columns.Clear();
            dgvProduct.DataSource = ProductBAL.GetProductKeysearch(ptod);
            dgvProduct.Columns["id"].Visible = true;
            dgvProduct.Columns["Imagepath"].Visible = false;
            dgvProduct.Columns["sizwuom"].Visible = false;
            dgvProduct.Columns["ItemName"].Visible = false;


            dgvProduct.Columns["Size"].Visible = false;
            dgvProduct.Columns["color"].Visible = false;
            dgvProduct.Columns["BarCode"].Visible = false;

            dgvProduct.Columns["VAT"].Visible = false;

            dgvProduct.Columns["ItemCode"].Visible = false;
            dgvProduct.Columns["ReorderPoint"].Visible = false;
            dgvProduct.Columns["Remarks"].Visible = false;
            dgvProduct.Columns["ISBarCodeable"].Visible = false;



            DataGridViewImageColumn img1 = new DataGridViewImageColumn();
            img1.Image = Inventory.Properties.Resources.user_edit;
            dgvProduct.Columns.Insert(0, img1);
            img1.HeaderText = "Edit";
            img1.Name = "Edit";

            DataGridViewImageColumn img2 = new DataGridViewImageColumn();
            img2.Image = Inventory.Properties.Resources.trash;
            dgvProduct.Columns.Insert(23, img2);
            img2.HeaderText = "Delete";
            img2.Name = "Delete";

            dgvProduct.Columns["categoryname"].Width = 90;
            dgvProduct.Columns["DisplayName"].Width = 410;
            dgvProduct.Columns["Brandname"].Width = 110;
            dgvProduct.Columns["SalesPrice"].Width = 60;
            if (GridHasColumn(dgvProduct, "MRP"))
            {
                dgvProduct.Columns["MRP"].Width = 60;
                dgvProduct.Columns["MRP"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProduct.Columns["MRP"].HeaderText = "MRP";
            }
            dgvProduct.Columns["ReorderPoint"].Width = 150;
            dgvProduct.Columns["Edit"].Width = 40;
            dgvProduct.Columns["Delete"].Width = 50;
            dgvProduct.Columns["HSNCODE"].Width = 100;
            dgvProduct.Columns["GST"].Width = 50;
            dgvProduct.Columns["ISBarCodeable"].Width = 100;
            dgvProduct.Columns["BarCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProduct.Columns["Size"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProduct.Columns["Rack"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProduct.Columns["MinStock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProduct.Columns["MaxStock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProduct.Columns["ReorderQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProduct.Columns["ReorderPoint"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProduct.Columns["ItemName"].HeaderText = "Name";
            dgvProduct.Columns["categoryname"].HeaderText = "Category";
            dgvProduct.Columns["ItemCode"].HeaderText = "Item Code";
            dgvProduct.Columns["Types"].HeaderText = "Light Category";

            dgvProduct.Columns["sizwuom"].HeaderText = "size Uom";
            dgvProduct.Columns["Brandname"].HeaderText = "Brand";
            dgvProduct.Columns["color"].HeaderText = "Color";

            //dgvProduct.Columns["Tax"].HeaderText = "CGST";





            //dgvProduct.Columns["ItemCode"].HeaderText = "Item Code";

            dgvProduct.Columns["HSNCODE"].HeaderText = "HSN Code";

            dgvProduct.Columns["DisplayName"].HeaderText = "Display Name";
            //dgvProduct.Columns["BarCode"].HeaderText = "Bar Code";
            //dgvProduct.Columns["categoryname"].HeaderText = "category Name";
            //  dgvProduct.Columns["Brandname"].HeaderText = "Brand Name";


            dgvProduct.Columns["ReorderPoint"].HeaderText = "Reorder Point";

            dgvProduct.Columns["Imagepath"].HeaderText = "Image path";


            dgvProduct.Columns["SalesPrice"].HeaderText = "Price";

            dgvProduct.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvProduct.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvProduct.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }

        private void butbrowse_Click(object sender, EventArgs e)
        {

            try
            {
                aaa = "";

                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    //Bitmap image = new Bitmap(248, 64);
                    Bitmap image1 = new Bitmap(open.FileName);

                    sourcepath = open.FileName;
                    filename = Path.GetFileName(sourcepath);

                    pcphoto.SizeMode = PictureBoxSizeMode.StretchImage;
                    pcphoto.Image = image1;
                    pcphoto.BackgroundImage = image1;
                    Guid guidimagename = Guid.NewGuid();
                    newnameimage = "\\\\192.168.1.250\\pit\\PICTURES\\" + guidimagename + filename;

                }
            }
            catch (Exception)
            {
                throw new ApplicationException("Image loading error....");
            }
        }

        private void butclear_Click(object sender, EventArgs e)
        {
            pcphoto.Image = null;
            pcphoto.BackgroundImage = null;
        }



        #region Product Validation

        private void txtMinimunStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;

            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtMaximunStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;

            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtreorderqty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtreorderpt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtbarcodes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtRack_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtsizes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtRemarks_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (char.IsLetter(e.KeyChar) || char.IsNumber(e.KeyChar)|| char.IsDigit(e.KeyChar)|| char.IsControl(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
            //{
            //    e.Handled = false;
            //}
            //else
            //{
            //    e.Handled = true;
            //}
        }

        private void txtitemnames_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (char.IsLetter(e.KeyChar) || char.IsControl(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
            //{
            //    e.Handled = false;
            //}
            //else
            //{
            //    e.Handled = true;
            //}
        }

        private void txtitemcodes_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (char.IsLetterOrDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            //{
            //    e.Handled = false;

            //}
            //else
            //{
            //    e.Handled = true;
            //}
        }

        public bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;


            //if (string.IsNullOrEmpty(Convert.ToString(txtitemcodes.Text)))
            //{
            //    i++;
            //    message = message + "* Itemcode should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtitemcodes;
            //}

            if (string.IsNullOrEmpty(Convert.ToString(txtitemnames.Text.Trim())))
            {
                i++;
                message = message + "* Item Name Should Not Be Empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtitemnames;

            }

            if (txtSalesPrice.Text==".")
            {
                i++;
                message = message + "* Please Enter Correct Sale Price" + "\n";
                if (i == 1)
                    this.ActiveControl = txtitemnames;

            }
            if (txtMRP.Text == ".")
            {
                i++;
                message = message + "* Please Enter Correct MRP" + "\n";
                if (i == 1)
                    this.ActiveControl = txtMRP;

            }

            //if (chkBoxBarcode.Checked == false)
            //{
            //    message += "* Select Barcode" + "\n";
            //    i++;
            //    if (i == 1)
            //        this.ActiveControl = chkBoxBarcode;

            //}

            //if (string.IsNullOrEmpty(Convert.ToString(txtbarcodes.Text)))
            //{
            //    i++;
            //    message = message + "* Barcode should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtbarcodes;
            //}

            //if (string.IsNullOrEmpty(Convert.ToString(txtsizes.Text)))
            //{
            //    i++;
            //    message = message + "* Size should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtsizes;
            //}

            //if (dropmeasure.SelectedIndex == 0)
            //{
            //    message += "* Choose Measurement" + "\n";
            //    i++;
            //    if (i == 1)
            //        this.ActiveControl = dropmeasure;

            //}

            if (cmbTax.SelectedIndex == 0)
            {
                message += "* Choose Tax" + "\n";
                i++;
                if (i == 1)
                    this.ActiveControl = cmbTax;

            }



            //if (textBox2.Text == "")
            //{
            //    message += "* Choose HSN CODE" + "\n";
            //    i++;
            //    if (i == 1)
            //        this.ActiveControl = textBox2;

            //}


            if (ddlcategorys.SelectedIndex == 0)
            {
                message += "* Choose Category" + "\n";
                i++;
                if (i == 1)
                    this.ActiveControl = ddlcategorys;

            }
            if (ddlBrands.SelectedIndex == 0)
            {
                message += "* Choose Brand" + "\n";
                i++;
                if (i == 1)
                    this.ActiveControl = ddlBrands;

            }
            //if (ddlcolors.SelectedIndex == 0)
            //{
            //    message += "* Choose color" + "\n";
            //    i++;
            //    if (i == 1)
            //        this.ActiveControl = ddlcolors;

            //}
            //for (int s = 0;s < locationinfo.Rows.Count;s++ )
            //{
            //    if (Convert.ToInt32(locationinfo.Rows[s].Cells[0].Value) == 0)
            //    {
            //        i++;
            //        message = message + "* Choose Location" + "\n";
            //        if (i == 1)
            //            this.ActiveControl = locationinfo;
            //        break;
            //    }
            //    if (string.IsNullOrEmpty(Convert.ToString(locationinfo.Rows[s].Cells[1].Value)))
            //    {
            //        i++;
            //        message = message + "* Enter Rack" + "\n";
            //        if (i == 1)
            //            this.ActiveControl = locationinfo;
            //        break;
            //    }
            //}


           

            //if (string.IsNullOrEmpty(Convert.ToString(txtRack.Text)))
            //{
            //    i++;
            //    message = message + "* Rack should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtRack;
            //}
            if (dropunitofmeasure.SelectedIndex == 0)
            {
                message += "* Choose UOM" + "\n";
                i++;
                if (i == 1)
                    this.ActiveControl = dropunitofmeasure;

            }
            if (string.IsNullOrEmpty(Convert.ToString(txtSalesPrice.Text)))
            {
                i++;
                message = message + "* Sales Price Should Not Be Empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtSalesPrice;
            }
            if (!string.IsNullOrEmpty(txtMRP.Text.Trim()) && IsMRPLessThanSalesPrice())
            {
                i++;
                message = message + "* MRP Should Not Be Less Than Sales Price" + "\n";
                if (i == 1)
                    this.ActiveControl = txtMRP;
            }

            //if (string.IsNullOrEmpty(Convert.ToString(txtMinimunStock.Text)))
            //{
            //    i++;
            //    message = message + "* Minimum stock should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtMinimunStock;
            //}

            //if (string.IsNullOrEmpty(Convert.ToString(txtMaximunStock.Text)))
            //{
            //    i++;
            //    message = message + "* Maximun stock  should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtMaximunStock;
            //}
            //if (string.IsNullOrEmpty(Convert.ToString(txtreorderqty.Text)))
            //{
            //    i++;
            //    message = message + "* Reorder quantity  should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtreorderqty;
            //}
            //if (string.IsNullOrEmpty(Convert.ToString(txtreorderpt.Text)))
            //{
            //    i++;
            //    message = message + "* Reorder point  should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtreorderpt;
            //}
            //if (string.IsNullOrEmpty(Convert.ToString(filename)))
            //{
            //    i++;
            //    message = message + "* Image should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = butbrowse;
            //}

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

        #endregion

        private void btnProductSave_Click(object sender, EventArgs e)
        {
            if (!BranchAccess.IsMainOffice)
            {
                MessageBox.Show(BranchAccess.MainOfficeOnlyMessage);
                return;
            }

            bool Val = Validation();
            if (Val == true)
            {

                InsertUpdateRightToInformation();
                

            }

        }

        private void clear()
        {
            //textBox3.Clear();
            //textBox2.Clear();
            txtitemcodes.Clear();
            txtitemnames.Clear();
            chkBoxBarcode.Checked = false;
            chkIncentive.Checked = false;
            txtbarcodes.Clear();
            txtSalesPrice.Clear();
            txtMRP.Clear();
            txtsizes.Clear();
            lightValue.Checked = false;
            txtMinimunStock.Clear();
            txtMaximunStock.Clear();
            txtreorderqty.Clear();
            txtreorderpt.Clear();
            txtRack.Clear();
            txtRemarks.Clear();
            cmbTax.SelectedIndex = 0;
            textBox2.Text = "";
            //textBox3.Text = "";
            dropmeasure.SelectedIndex = 0;
            ddlcategorys.SelectedIndex = 0;
            ddlcolors.SelectedIndex = 0;
            dropunitofmeasure.SelectedIndex = 0;
            ddlBrands.SelectedIndex = 0;
            pcphoto.Image = null;
            filename = null;
            lblhidden.Text = string.Empty;
          

        }

        private void btnclear_Click(object sender, EventArgs e) // clear
        {
            clear();
        }

        private void btnnew_Click(object sender, EventArgs e)// New
        {
            clear();
        }

        private void butbrowse_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog opFile = new OpenFileDialog();
            opFile.Title = "Select a Image";
            opFile.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";

            //string appPath = Path.GetPathRoot(@"/192.168.1.250./Hrms");
            string appPath = Program.path;
            //if (Directory.Exists(appPath) == false)
            //{
            //    Directory.CreateDirectory(appPath);
            //}

            if (opFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string iName = opFile.SafeFileName;
                    filepath = opFile.FileName;
                    imgpath = appPath + DateTime.Now.Ticks.ToString() + iName;
                    //File.Copy(filepath, imgpath);
                    filename = imgpath;
                    //filename = iName;
                    //lblimagepath.Text = imagePath;
                    pcphoto.Image = new Bitmap(opFile.OpenFile());
                    pcphoto.SizeMode = PictureBoxSizeMode.StretchImage;
                }

                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message, "Could not load image");
                }
            }
            else
            {
                opFile.Dispose();
            }


            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    Bitmap image1 = new Bitmap(openFileDialog.FileName);
            //    //sourcePath = openFileDialog.FileName;
            //    //fileName = Path.GetFileName(sourcePath);
            //    pcphoto.ImageLocation = openFileDialog.FileName;
            //    pcphoto.SizeMode = PictureBoxSizeMode.StretchImage;
            //    //imagePath = sourcePath;
            //}
        }

        private void butclear_Click_1(object sender, EventArgs e)
        {
            pcphoto.Image = null;
            filename = null;
        }

        public void bindcatogory()
        {
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[getcategry]", con);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                DataRow row = dt.NewRow();
                row["id"] = "0";
                row["categoryname"] = "--Select--";
                dt.Rows.InsertAt(row, 0);
                ddlcategorys.DataSource = dt;
                ddlcategorys.ValueMember = "id";
                ddlcategorys.DisplayMember = "categoryname";
                ddlcategorys.SelectedIndex = 0;

            }
        }
        public void bindBrand()
        {
            DataTable dt = ProductBAL.GetBrand();
            DataRow row = dt.NewRow();
            row["id"] = "0";
            row["Brandname"] = "--Select--";
            dt.Rows.InsertAt(row, 0);
            ddlBrands.DataSource = dt;
            ddlBrands.ValueMember = "id";
            ddlBrands.DisplayMember = "Brandname";
            ddlBrands.SelectedIndex = 0;


        }
        public void bindUom()
        {
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[getuom]", con);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                DataRow row = dt.NewRow();
                row["Uomid"] = "0";
                row["UOM"] = "--Select--";
                dt.Rows.InsertAt(row, 0);
                dropunitofmeasure.DataSource = dt;
                dropunitofmeasure.ValueMember = "Uomid";
                dropunitofmeasure.DisplayMember = "UOM";
                dropunitofmeasure.SelectedIndex = 0;

            }
        }


        public void bindsizeUom()
        {
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[getsizeuom]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                DataRow row = dt.NewRow();
                row["id"] = "0";
                row["sizwuom"] = "--Select--";
                dt.Rows.InsertAt(row, 0);
                dropmeasure.DataSource = dt;
                dropmeasure.ValueMember = "id";
                dropmeasure.DisplayMember = "sizwuom";
                dropmeasure.SelectedIndex = 0;

            }
        }
        //public void BindRackName()
        //{
        //    dataGridView1.DataSource = null;
        //    DataTable dtCashRequest = ObjProductBAL.BindRackName();
        //    dataGridView1.DataSource = dtCashRequest;
        //    //dgvCashRequest.Columns[1].Visible = false;

        //    dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10.1F, FontStyle.Bold);
        //    dataGridView1.DefaultCellStyle.Font = new Font("Tahoma", 12.1F, GraphicsUnit.Pixel);
        //    dataGridView1.DefaultCellStyle.BackColor = Color.Gainsboro;
        //    dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
        //    dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        //    this.dataGridView1.Columns[0].Width = 100;
        //    this.dataGridView1.Columns[1].Width = 200;
        //    this.dataGridView1.Columns[2].Width = 100;
        //}
        public void bindproductcolor()
        {
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[getproductcolor]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                DataRow row = dt.NewRow();
                row["id"] = "0";
                row["color"] = "--Select--";
                dt.Rows.InsertAt(row, 0);
                ddlcolors.DataSource = dt;
                ddlcolors.ValueMember = "id";
                ddlcolors.DisplayMember = "color";
                ddlcolors.SelectedIndex = 0;

            }
        }

        //public void bindbrand()
        //{
        //    int a = 0;
        //    if (ddlcategorys.SelectedIndex > 0)
        //    {
        //        a = Convert.ToInt32(ddlcategorys.SelectedValue);
        //    }
        //    using (SqlConnection con = new SqlConnection(conn))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("[getbrand]", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@categoryid", a);
        //        SqlDataAdapter ad = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        ad.Fill(dt);
        //        DataRow row = dt.NewRow();
        //        row["id"] = "0";
        //        row["Brandname"] = "--Select--";
        //        dt.Rows.InsertAt(row, 0);
        //        ddlBrands.DataSource = dt;
        //        ddlBrands.ValueMember = "id";
        //        ddlBrands.DisplayMember = "Brandname";
        //        ddlBrands.SelectedIndex = 0;

        //    }
        //}

        private void ddlcategorys_SelectedIndexChanged(object sender, EventArgs e)
        {
            // bindbrand();
        }

        private void InsertUpdateRightToInformation()
        {
            int Status = 0;
            if (string.IsNullOrEmpty(Convert.ToString(lblhidden.Text)))
            {
                ObjProductBAL.id = "";
                // File.Copy(filepath, imgpath);
            }
            else
            {
                ObjProductBAL.id = lblhidden.Text;
            }

            if (chkBoxBarcode.Checked == true)
            {
                ObjProductBAL.BarCode = txtbarcodes.Text;
                ObjProductBAL.IsBarcodeable = "1";
            }
            else
            {
                ObjProductBAL.IsBarcodeable = "0";
                ObjProductBAL.BarCode = null;
                //if (!string.IsNullOrEmpty(filepath))
                //{
                //    try
                //    {
                //        File.Copy(filepath, imgpath);
                //    }
                //    catch
                //    {

                //    }
                //}
            }
            if (chkIncentive.Checked == true)
            {

                ObjProductBAL.Incentive = "1";
            }
            else
            {
                ObjProductBAL.Incentive = "0";
              
            }

            if (lightValue.Checked == true)
            {

                ObjProductBAL.Type = "Yes";
            }
            else
            {
                ObjProductBAL.Type = "No";

            }

            ObjProductBAL.ItemCode = txtitemcodes.Text;
            ObjProductBAL.ItemName = txtitemnames.Text.Trim();

            ObjProductBAL.Size = txtsizes.Text;
            ObjProductBAL.SizeId = Convert.ToString(dropmeasure.SelectedValue);
            ObjProductBAL.Category = Convert.ToString(ddlcategorys.SelectedValue);
            ObjProductBAL.Brand = Convert.ToString(ddlBrands.SelectedValue);
            ObjProductBAL.ProductColor = Convert.ToString(ddlcolors.SelectedValue);
            ObjProductBAL.Rack = txtRack.Text;
            ObjProductBAL.UOM = Convert.ToString(dropunitofmeasure.SelectedValue);
            ObjProductBAL.MinStock = txtMinimunStock.Text;
            ObjProductBAL.MaxStock = txtMaximunStock.Text;
            ObjProductBAL.ReorderQty = txtreorderqty.Text;
            ObjProductBAL.ReorderPoint = txtreorderpt.Text;
            ObjProductBAL.Tax = cmbTax.Text;
            ObjProductBAL.SGST = textBox2.Text;
           
            ObjProductBAL.Remarks = txtRemarks.Text;
            ObjProductBAL.Imagepath = filename;
            ObjProductBAL.UserId = Convert.ToString(Program.userlevel);
          


            DataTable dtname = GetSizeName(Convert.ToInt32(dropmeasure.SelectedValue));

            ObjProductBAL.DisplayName = txtitemnames.Text.Trim().ToUpper();

            ObjProductBAL.SalesPrice = txtSalesPrice.Text;
            Status = ProductBAL.SaveProduct(ObjProductBAL);
            UpdateProductMRP(Status);

            DataTable dt = new DataTable();

            dt.Columns.Add("LocationID", typeof(int));
            dt.Columns.Add("Rack", typeof(string));

          
            int count = dt.Rows.Count;
            if (count>0)
            {
                int res = ProductBAL.deleteproductlocation(Convert.ToString(Status));
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //if(Convert.ToString(locationinfo.Rows[i].Cells[0].Value)!="0")
                //{
                //if (!string.IsNullOrEmpty(Convert.ToString(locationinfo.Rows[i].Cells[3].Value)))
                //{
                //    ObjProductBAL.RackID = Convert.ToString(locationinfo.Rows[i].Cells[3].Value);
                //}
                //else
                //{
                ObjProductBAL.RackID = "";
                //}


                ObjProductBAL.LocationId = Convert.ToString(dt.Rows[i][0]);
                if (Convert.ToString(dt.Rows[i][1]) == "--Select--")
                {
                    ObjProductBAL.Rack = "";
                }
                else
                {
                    ObjProductBAL.Rack = Convert.ToString(dt.Rows[i][1]);
                }
                ObjProductBAL.ItemCode = Convert.ToString(Status);
                int res = ProductBAL.SaveProductlocation(ObjProductBAL);
                
            }

            if (lblhidden.Text == string.Empty)
            {
                QueueProductMasterChange(Status);
                MessageBox.Show("Inserted successfully");

                itemdetails("");
                try
                {
                    File.Copy(filepath, imgpath);
                }
                catch
                {

                }
                bindproduct();
                search("itemcode", "", "itemname", "", "ispending", "");
                clear();

            }
            else if (lblhidden.Text != string.Empty)
            {
               
                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        con.Open();
                        string query = "UPDATE ProductMaster SET IsArchived = " + (chkArchive.Checked ? "1" : "0") + " WHERE id = " + lblhidden.Text;

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {

                            cmd.ExecuteNonQuery();
                        }
                    }


                QueueProductMasterChange(Status);
                clear();
                MessageBox.Show("Updated Succesfully");
                bindproduct();
                itemdetails("");
                if (!string.IsNullOrEmpty(filepath))
                {
                    try
                    {
                        File.Copy(filepath, imgpath);
                    }
                    catch
                    {

                    }
                }
               
                search("itemcode", "", "itemname", "", "ispending", "");
                clear();

            }
            else if (Status == 0)
            {
                MessageBox.Show("Projduct already exist ");
                txtitemcodes.Focus();
            }
        }



        public DataTable GetSizeName(int SizeID)
        {
            DataTable dtSizeName = ProductBAL.ObjProductDAL.GetsizeName(SizeID);

            return dtSizeName;
        }

        private void QueueProductMasterChange(int productId)
        {
            if (productId > 0)
            {
                ProductMasterCloudQueue.EnqueueAndTryPush(Convert.ToString(productId), "Product", true);
            }
        }

        private void UpdateProductMRP(int productId)
        {
            if (productId <= 0)
                return;

            decimal mrp;
            object mrpValue = DBNull.Value;
            if (!string.IsNullOrEmpty(txtMRP.Text.Trim()) && decimal.TryParse(txtMRP.Text.Trim(), out mrp))
            {
                mrpValue = mrp;
            }

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                EnsureProductMRPColumn(con);

                using (SqlCommand cmd = new SqlCommand("UPDATE ProductMaster SET MRP = @MRP WHERE id = @id", con))
                {
                    SqlParameter mrpParameter = cmd.Parameters.Add("@MRP", SqlDbType.Decimal);
                    mrpParameter.Precision = 18;
                    mrpParameter.Scale = 2;
                    mrpParameter.Value = mrpValue;
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void EnsureProductMRPColumn(SqlConnection con)
        {
            using (SqlCommand existsCmd = new SqlCommand("SELECT COL_LENGTH('dbo.ProductMaster', 'MRP')", con))
            {
                if (existsCmd.ExecuteScalar() != DBNull.Value)
                    return;
            }

            using (SqlCommand alterCmd = new SqlCommand("ALTER TABLE dbo.ProductMaster ADD MRP decimal(18,2) NULL", con))
            {
                alterCmd.ExecuteNonQuery();
            }
        }

        private bool IsMRPLessThanSalesPrice()
        {
            decimal mrp;
            decimal salesPrice;
            if (!decimal.TryParse(txtMRP.Text.Trim(), out mrp) || !decimal.TryParse(txtSalesPrice.Text.Trim(), out salesPrice))
                return false;

            return mrp < salesPrice;
        }

        private bool GridHasColumn(DataGridView grid, string columnName)
        {
            return grid.Columns.Contains(columnName);
        }

        private string GetGridCellValue(DataGridView grid, int rowIndex, string columnName)
        {
            if (!GridHasColumn(grid, columnName))
                return string.Empty;

            return Convert.ToString(grid.Rows[rowIndex].Cells[columnName].Value);
        }

        private string GetProductMRP(string productId, DataGridView grid, int rowIndex)
        {
            string gridValue = GetGridCellValue(grid, rowIndex, "MRP");
            if (!string.IsNullOrEmpty(gridValue))
                return gridValue;

            if (string.IsNullOrEmpty(productId))
                return string.Empty;

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                using (SqlCommand existsCmd = new SqlCommand("SELECT COL_LENGTH('dbo.ProductMaster', 'MRP')", con))
                {
                    if (existsCmd.ExecuteScalar() == DBNull.Value)
                        return string.Empty;
                }

                using (SqlCommand cmd = new SqlCommand("SELECT CONVERT(varchar(50), MRP) FROM ProductMaster WHERE id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", productId);
                    object mrp = cmd.ExecuteScalar();
                    return Convert.ToString(mrp);
                }
            }
        }


        private void InsertUpdatepending()
        {
            int Status = 0;
            if (string.IsNullOrEmpty(Convert.ToString(lblhidden.Text)))
            {
                ObjProductBAL.id = "";
                // File.Copy(filepath, imgpath);
            }
            else
            {
                ObjProductBAL.id = lblhidden.Text;
            }
            if (chkIncentive.Checked == true)
            {

                ObjProductBAL.Incentive = "1";
            }
            else
            {
                ObjProductBAL.Incentive = "0";

            }


            if (lightValue.Checked == true)
            {

                ObjProductBAL.Type ="Yes";
            }
            else
            {
                ObjProductBAL.Type = "No";

            }

            if (chkBoxBarcode.Checked == true)
            {
                ObjProductBAL.BarCode = txtbarcodes.Text;
                ObjProductBAL.IsBarcodeable = "1";
            }
            else
            {
                ObjProductBAL.IsBarcodeable = "0";
                ObjProductBAL.BarCode = null;
                //if (!string.IsNullOrEmpty(filepath))
                //{
                //    try
                //    {
                //        File.Copy(filepath, imgpath);
                //    }
                //    catch
                //    {

                //    }
                //}
            }

            ObjProductBAL.ItemCode = txtitemcodes.Text;
            ObjProductBAL.ItemName = txtitemnames.Text.Trim();

            ObjProductBAL.Size = txtsizes.Text;
            ObjProductBAL.SizeId = Convert.ToString(dropmeasure.SelectedValue);
            ObjProductBAL.Category = Convert.ToString(ddlcategorys.SelectedValue);
            ObjProductBAL.Brand = Convert.ToString(ddlBrands.SelectedValue);
            ObjProductBAL.ProductColor = Convert.ToString(ddlcolors.SelectedValue);
            ObjProductBAL.Rack = txtRack.Text;
            ObjProductBAL.UOM = Convert.ToString(dropunitofmeasure.SelectedValue);
            ObjProductBAL.MinStock = txtMinimunStock.Text;
            ObjProductBAL.MaxStock = txtMaximunStock.Text;
            ObjProductBAL.ReorderQty = txtreorderqty.Text;
            ObjProductBAL.ReorderPoint = txtreorderpt.Text;
            ObjProductBAL.Tax = cmbTax.Text;
            ObjProductBAL.Remarks = txtRemarks.Text;
            ObjProductBAL.Imagepath = filename;
            ObjProductBAL.UserId = Convert.ToString(Program.userlevel);
          
            DataTable dtname = GetSizeName(Convert.ToInt32(dropmeasure.SelectedValue));

            ObjProductBAL.DisplayName = txtitemnames.Text.Trim().ToUpper();

            ObjProductBAL.SalesPrice = txtSalesPrice.Text;
            Status = ProductBAL.SaveProductPending(ObjProductBAL);
            UpdateProductMRP(Status);

            DataTable dt = new DataTable();

            dt.Columns.Add("LocationID", typeof(int));
            dt.Columns.Add("Rack", typeof(string));

            
            int count = dt.Rows.Count;
            if (count > 0)
            {
                int res = ProductBAL.deleteproductlocation(Convert.ToString(Status));
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //if(Convert.ToString(locationinfo.Rows[i].Cells[0].Value)!="0")
                //{
                //if (!string.IsNullOrEmpty(Convert.ToString(locationinfo.Rows[i].Cells[3].Value)))
                //{
                //    ObjProductBAL.RackID = Convert.ToString(locationinfo.Rows[i].Cells[3].Value);
                //}
                //else
                //{
                ObjProductBAL.RackID = "";
                //}


                ObjProductBAL.LocationId = Convert.ToString(dt.Rows[i][0]);
                if (Convert.ToString(dt.Rows[i][1]) == "--Select--")
                {
                    ObjProductBAL.Rack = "";
                }
                else
                {
                    ObjProductBAL.Rack = Convert.ToString(dt.Rows[i][1]);
                }
                ObjProductBAL.ItemCode = Convert.ToString(Status);
                int res = ProductBAL.SaveProductlocation(ObjProductBAL);

            }

            if (lblhidden.Text == string.Empty)
            {
                QueueProductMasterChange(Status);
                MessageBox.Show("Inserted successfully");

                itemdetails("");
                try
                {
                    File.Copy(filepath, imgpath);
                }
                catch
                {

                }
                bindproduct();
                search("itemcode", "", "itemname", "", "ispending", "");
               clear();

            }
            else if (lblhidden.Text != string.Empty)
            {
                //clear();
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    string query = "UPDATE ProductMaster SET IsArchived = " + (chkArchive.Checked ? "1" : "0") + " WHERE id = " + lblhidden.Text;
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {

                        cmd.ExecuteNonQuery();
                    }
                }

                QueueProductMasterChange(Status);
                MessageBox.Show("Updated Succesfully");
                itemdetails("");
                if (!string.IsNullOrEmpty(filepath))
                {
                    try
                    {
                        File.Copy(filepath, imgpath);
                    }
                    catch
                    {

                    }
                }
                bindproduct();
                search("itemcode", "", "itemname", "", "ispending", "");
              clear();

            }
            else if (Status == 0)
            {
                MessageBox.Show("Projduct already exist ");
                txtitemcodes.Focus();
            }
        }

        public void itemdetails(string s)
        {

            try
            {
                dtitems = new DataTable();
                string s1 = s.Trim();
                string s2 = Convert.ToString("");
                string name = s1.Replace("'", "''");


                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                dtitems = objQuotationbal.itemdetails(name, s2);
                Program.dtitems = dtitems;


            }
            catch (Exception e)
            {

            }

        }


        public void bindproduct()
        {

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("getproduct_Direct_Product1", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);

                dgvProduct.Columns.Clear();
                dgvProduct.DataSource = dt;
                
                dgvProduct.Columns["id"].Visible = true;
                dgvProduct.Columns["Imagepath"].Visible = false;
                dgvProduct.Columns["sizwuom"].Visible = false;
                dgvProduct.Columns["ItemName"].Visible = false;



                dgvProduct.Columns["Size"].Visible = false;
                dgvProduct.Columns["color"].Visible = false;
                dgvProduct.Columns["BarCode"].Visible = false;

                dgvProduct.Columns["ItemCode"].Visible = false;
                dgvProduct.Columns["ReorderPoint"].Visible = false;
                dgvProduct.Columns["Remarks"].Visible = false;
                dgvProduct.Columns["ISBarCodeable"].Visible = false;

                dgvProduct.Columns["VAT"].Visible = false;


                DataGridViewImageColumn img1 = new DataGridViewImageColumn();
                img1.Image = Inventory.Properties.Resources.user_edit;
                dgvProduct.Columns.Insert(0, img1);
                img1.HeaderText = "Edit";
                img1.Name = "Edit";

                DataGridViewImageColumn img2 = new DataGridViewImageColumn();
                img2.Image = Inventory.Properties.Resources.trash;
                dgvProduct.Columns.Insert(23, img2);
                img2.HeaderText = "Delete";
                img2.Name = "Delete";

               // dgvProduct.Columns["ItemName"].Width = 150;
                dgvProduct.Columns["categoryname"].Width = 90;
                dgvProduct.Columns["DisplayName"].Width = 410;
                dgvProduct.Columns["Brandname"].Width = 100;
                dgvProduct.Columns["SalesPrice"].Width = 60;
                if (GridHasColumn(dgvProduct, "MRP"))
                {
                    dgvProduct.Columns["MRP"].Width = 60;
                    dgvProduct.Columns["MRP"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvProduct.Columns["MRP"].HeaderText = "MRP";
                }
                dgvProduct.Columns["ReorderPoint"].Width = 150;
                dgvProduct.Columns["Types"].Width = 100;
                dgvProduct.Columns["HSNCODE"].Width = 50;
                dgvProduct.Columns["GST"].Width = 45;
                dgvProduct.Columns["Edit"].Width = 40;
                dgvProduct.Columns["Delete"].Width = 60;
                dgvProduct.Columns["ISBarCodeable"].Width = 100;
                dgvProduct.Columns["BarCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProduct.Columns["Size"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProduct.Columns["Rack"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProduct.Columns["MinStock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProduct.Columns["MaxStock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProduct.Columns["ReorderQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProduct.Columns["ReorderPoint"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProduct.Columns["ItemName"].HeaderText = "Name";
                dgvProduct.Columns["categoryname"].HeaderText = "Category";
                dgvProduct.Columns["ItemCode"].HeaderText = "Item Code";
                dgvProduct.Columns["ItemName"].HeaderText = "Item Name";


                dgvProduct.Columns["Types"].HeaderText = "Light Category";

                dgvProduct.Columns["sizwuom"].HeaderText = "size Uom";
                dgvProduct.Columns["Brandname"].HeaderText = "Brand";
                dgvProduct.Columns["color"].HeaderText = "Color";

                dgvProduct.Columns["HSNCODE"].HeaderText = "HSN Code";






                //dgvProduct.Columns["ItemCode"].HeaderText = "Item Code";

                dgvProduct.Columns["DisplayName"].HeaderText = "Display Name";
                //dgvProduct.Columns["BarCode"].HeaderText = "Bar Code";
                //dgvProduct.Columns["categoryname"].HeaderText = "category Name";
                //  dgvProduct.Columns["Brandname"].HeaderText = "Brand Name";


                dgvProduct.Columns["ReorderPoint"].HeaderText = "Reorder Point";

                dgvProduct.Columns["Imagepath"].HeaderText = "Image path";


                dgvProduct.Columns["SalesPrice"].HeaderText = "Price";
                
                dgvProduct.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                dgvProduct.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvProduct.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            }
            dgvProduct.FirstDisplayedCell = dgvProduct.Rows[valindex].Cells[0];
            if (dgvProduct.Rows.Count == valindex + 1)
            {
                dgvProduct.Rows[valindex].Cells[0].Selected = true;
            }
            else
            {
                dgvProduct.Rows[valindex + 1].Cells[0].Selected = true;
            }
        }

        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                
                if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "Edit")
                {
                    valindex = e.RowIndex;
                    lblhidden.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["id"].Value);
                    txtitemcodes.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["ItemCode"].Value);
                    txtitemnames.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["ItemName"].Value);
                    //if (dgvProduct.Rows[e.RowIndex].Cells["IsArchived"].Value.ToString()=="True")
                    //{
                    //    chkArchive.Checked = true;
                    //}
                    //else
                    //{
                    //    chkArchive.Checked = false;
                    //}
                    if (string.IsNullOrEmpty(Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["Incentive"].Value)))
                    {
                        chkIncentive.Checked = false;
                    }
                    else
                    {
                        chkIncentive.Checked = Convert.ToBoolean(dgvProduct.Rows[e.RowIndex].Cells["Incentive"].Value);
                    }
                    if (string.IsNullOrEmpty(Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["ISBarCodeable"].Value)))
                    {
                        chkBoxBarcode.Checked = false;
                    }
                    else
                    {
                        chkBoxBarcode.Checked = Convert.ToBoolean(dgvProduct.Rows[e.RowIndex].Cells["ISBarCodeable"].Value);
                    }

                    if (string.IsNullOrEmpty(Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["GST"].Value)))
                    {

                        cmbTax.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbTax.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["GST"].Value);
                    }
                    //textBox1.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["GST"].Value);
                    textBox2.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["HSNCODE"].Value);
                   // textBox3.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["IGST"].Value);
                    //if (string.IsNullOrEmpty(Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["Tax"].Value)))
                    //{

                    //    textBox1.SelectedIndex = 0;
                    //}
                    //else
                    //{
                        
                    //    textBox1.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["Tax"].Value);
                    //}
                    lightValue.Checked = false;
                    string Types = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["Types"].Value);
                    if (Types=="No")
                    {
                        lightValue.Checked = false;
                    }
                    else if (Types == "Yes")
                    {
                        
                        lightValue.Checked = true;
                    }

                    //textBox1.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["Types"].Value);
                    txtbarcodes.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["BarCode"].Value);
                    txtsizes.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["Size"].Value);
                    dropmeasure.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["sizwuom"].Value);
                    ddlcategorys.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["categoryname"].Value);
                    ddlBrands.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["Brandname"].Value);
                    ddlcolors.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["color"].Value);
                    txtRack.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["Rack"].Value);
                    dropunitofmeasure.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["UOM"].Value);
                    txtMinimunStock.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["MinStock"].Value);
                    txtMaximunStock.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["MaxStock"].Value);
                    txtreorderqty.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["ReorderQty"].Value);
                    txtreorderpt.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["ReorderPoint"].Value);
                    filename = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["Imagepath"].Value);
                    txtRemarks.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["Remarks"].Value);
                    txtSalesPrice.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["SalesPrice"].Value);
                    txtMRP.Text = GetProductMRP(lblhidden.Text, dgvProduct, e.RowIndex);

                    if (!string.IsNullOrEmpty(filename))
                    {
                        pcphoto.ImageLocation = filename;
                        pcphoto.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    DataTable dt = ProductBAL.GetProductLocation(lblhidden.Text);

                    DataTable productlication = new DataTable();
                    productlication.Columns.Add("LocationID", typeof(int));
                    productlication.Columns.Add("Rack", typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        // dr["LocationID"] = dt.Rows[i][0];
                        string rackname = Convert.ToString(dt.Rows[i][1]);
                        string[] reck = rackname.Split(',');
                        for (int h = 0; h < reck.Count(); h++)
                        {
                            DataRow dr = productlication.NewRow();
                            dr["LocationID"] = dt.Rows[i][0];
                            dr["Rack"] = reck[h];
                            productlication.Rows.Add(dr);
                        }
                       // locationinfo.Rows[i].Cells["LocationRack"].Value = Convert.ToString(dt.Rows[i][1]);
                        // locationinfo.Rows[i].Cells[0].Value=dt.Rows[i][0];
                        // GetLocationRack(Convert.ToString(dt.Rows[i][0]), Convert.ToString(i), Convert.ToInt32(dt.Rows[i][1]));
                        //// locationinfo.Rows[i].Cells["LocationRack"].Value = dt.Rows[i][1];

                        // locationinfo.Rows[i].Cells[3].Value = dt.Rows[i][2];
                    }
                    int count = productlication.Rows.Count;
                   
                   


                }
                else if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "Delete")
                {
                    string namestatus = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["ItemName"].Value);
                    DialogResult result = MessageBox.Show("Are you sure delete Product " + namestatus + "?", "Delete Product?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.Yes))
                    {
                        lblhidden.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["id"].Value);
                        int i = Delete();
                        if (i == 1)
                        {
                            MessageBox.Show("Delete Successfully");
                            clear();
                            bindproduct();
                        }
                    }
                }
            }
        }

        public int Delete()
        {
            int result = 0;
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[deleteproduct]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", lblhidden.Text);

                SqlParameter outid = new SqlParameter("@out", DbType.Int32);
                outid.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outid);
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@out"].Value);
            }
            return result;
        }

        private void dgvProduct_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                return;
            }
            var dataGridView = (sender as DataGridView);
            //Check the condition as per the requirement casting the cell value to the appropriate type
            if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
                dataGridView.Cursor = Cursors.Hand;
            else
                dataGridView.Cursor = Cursors.Default;
        }

        private void btnsaveaspending_Click(object sender, EventArgs e)
        {
            bool Val = Validationpending();
            if (Val == true)
            {
                InsertUpdatepending();
            }
        }

        public bool Validationpending()
        {
            bool status = true;
            string message = "";
            int i = 0;


            //if (string.IsNullOrEmpty(Convert.ToString(txtitemcodes.Text)))
            //{
            //    message += "* Itemcode should not empty" + "\n";
            //    i++;
            //    if (i == 1)
            //    {
            //        this.ActiveControl = txtitemcodes;

            //    }
            //}
            //if (textBox2.Text == "")
            //{
            //    message += "* Choose HSN CODE" + "\n";
            //    i++;
            //    if (i == 1)
            //        this.ActiveControl = textBox2;

            //}

            if (string.IsNullOrEmpty(Convert.ToString(txtitemnames.Text)))
            {
                message += "* Itemname should not empty" + "\n";
                i++;
                if (i == 1)
                {
                    this.ActiveControl = txtitemnames;

                }
            }

            if (cmbTax.SelectedIndex == 0)
            {
                message += "* Choose Tax" + "\n";
                i++;
                if (i == 1)
                    this.ActiveControl = cmbTax;

            }
            //if (textBox1.Text =="")
            //{
            //    message += "* Choose Tax" + "\n";
            //    i++;
            //    if (i == 1)
            //        this.ActiveControl = textBox1;

            //}


            if (ddlcategorys.SelectedIndex == 0)
            {
                message += "* Choose Category" + "\n";
                i++;
                if (i == 1)
                    this.ActiveControl = ddlcategorys;

            }
            if (ddlBrands.SelectedIndex == 0)
            {
                message += "* Choose Brand" + "\n";
                i++;
                if (i == 1)
                    this.ActiveControl = ddlBrands;

            }
            //if (ddlcolors.SelectedIndex == 0)
            //{
            //    message += "* Choose color" + "\n";
            //    i++;
            //    if (i == 1)
            //        this.ActiveControl = ddlcolors;

            //}
            

            //if (string.IsNullOrEmpty(Convert.ToString(txtRack.Text)))
            //{
            //    i++;
            //    message = message + "* Rack should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtRack;
            //}
            if (dropunitofmeasure.SelectedIndex == 0)
            {
                message += "* Choose UOM" + "\n";
                i++;
                if (i == 1)
                    this.ActiveControl = dropunitofmeasure;

            }
            if (string.IsNullOrEmpty(Convert.ToString(txtSalesPrice.Text)))
            {
                i++;
                message = message + "* Sales Price Should Not Be Empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtSalesPrice;
            }
            if (txtMRP.Text == ".")
            {
                i++;
                message = message + "* Please Enter Correct MRP" + "\n";
                if (i == 1)
                    this.ActiveControl = txtMRP;
            }
            if (!string.IsNullOrEmpty(txtMRP.Text.Trim()) && IsMRPLessThanSalesPrice())
            {
                i++;
                message = message + "* MRP Should Not Be Less Than Sales Price" + "\n";
                if (i == 1)
                    this.ActiveControl = txtMRP;
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

        private void btnSearch_Click(object sender, EventArgs e)
        {

            if ((cbxSearchOrderNo.SelectedIndex == cbxSearchOrderDate.SelectedIndex) || cbxSearchOrderNo.SelectedIndex == cbxVendor.SelectedIndex || cbxSearchOrderDate.SelectedIndex == cbxVendor.SelectedIndex)
            {
                MessageBox.Show("Search a item Should Not Be Same");
            }
            else
            {
                string firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue;

                firstname = cbxSearchOrderNo.Text.Trim();
                if (firstname == "status")
                {
                    firstname = "ispending";
                    firstvalue = Convert.ToString(cmbstatus1.SelectedIndex);
                }
                else
                {
                    firstvalue = txtsearch1.Text.Trim();
                }

                secondname = cbxSearchOrderDate.Text.Trim();
                if (secondname == "status")
                {
                    secondname = "ispending";
                    secondvalue = Convert.ToString(cmbstatus2.SelectedIndex);
                }
                else
                {
                    secondvalue = txtsearch2.Text.Trim();
                }


                thirdname = cbxVendor.Text.Trim();
                if (thirdname == "status")
                {
                    thirdname = "ispending";
                    thirdvalue = Convert.ToString(cmbstatus3.SelectedIndex);
                }
                else
                {
                    thirdvalue = txtsearch3.Text.Trim();
                }

                search(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue);
            }
        }

        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue)
        {
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("searchprodut_Direct", con);
                cmd.Parameters.AddWithValue("@firstname", firstname);
                cmd.Parameters.AddWithValue("@firstvale", firstvalue);
                cmd.Parameters.AddWithValue("@secondname", secondname);
                cmd.Parameters.AddWithValue("@secondvalue", secondvalue);
                cmd.Parameters.AddWithValue("@thirdname", thirdname);
                cmd.Parameters.AddWithValue("@thirdvalue", thirdvalue);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);

                dgvSearch.Columns.Clear();
                dgvSearch.DataSource = dt;
                dgvSearch.Columns["Types"].Visible = false;
                dgvSearch.Columns["ItemCode"].Visible = false;
                dgvSearch.Columns["ISBarCodeable"].Visible = false;
                dgvSearch.Columns["BarCode"].Visible = false;
                dgvSearch.Columns["sizwuom"].Visible = false;
                dgvSearch.Columns["categoryname"].Visible = false;
                dgvSearch.Columns["Brandname"].Visible = false;
                dgvSearch.Columns["color"].Visible = false;
                dgvSearch.Columns["MinStock"].Visible = false;
                dgvSearch.Columns["MaxStock"].Visible = false;
                dgvSearch.Columns["ReorderQty"].Visible = false;
                dgvSearch.Columns["ReorderPoint"].Visible = false;
                dgvSearch.Columns["Imagepath"].Visible = false;
                dgvSearch.Columns["Remarks"].Visible = false;
                //dgvSearch.Columns["Remarks"].Visible = false;
                dgvSearch.Columns["id"].Visible = false;



                if (dt.Rows.Count > 0)
                {
                    // dgvSearch.Columns["ItemName"].Width = 100;
                    //dgvProduct.Columns["Brandname"].HeaderText = "Brand Name";
                    //dgvProduct.Columns["color"].HeaderText = "Color";
                }
                dgvSearch.Columns["ItemName"].HeaderText = "Item Name";
                dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

                dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgvSearch.Columns["Size"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvSearch.Columns["Rack"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void dgvSearch_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                return;
            }
            var dataGridView = (sender as DataGridView);
            //Check the condition as per the requirement casting the cell value to the appropriate type

            dataGridView.Cursor = Cursors.Hand;

        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lblhidden.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["id"].Value);
                txtitemcodes.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ItemCode"].Value);
                txtitemnames.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ItemName"].Value);
                //textBox1.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Types"].Value);
                txtSalesPrice.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["SalesPrice"].Value);
                txtMRP.Text = GetProductMRP(lblhidden.Text, dgvSearch, e.RowIndex);
                if (Convert.ToBoolean(dgvSearch.Rows[e.RowIndex].Cells["ISBarCodeable"].Value)==true)
                {
                    chkBoxBarcode.Checked = true;
                }
                else
                {
                    chkBoxBarcode.Checked = false;
                }

                if (Convert.ToBoolean(dgvSearch.Rows[e.RowIndex].Cells["Types"].Value) == true)
                {
                    lightValue.Checked = true;
                }
                else
                {
                    lightValue.Checked = false;
                }

                if (!string.IsNullOrEmpty(Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["GST"].Value)))
                {
                    cmbTax.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["GST"].Value);
                }
                else
                {
                    cmbTax.SelectedIndex = 0;
                }

                txtbarcodes.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["BarCode"].Value);
                txtsizes.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Size"].Value);
              //  textBox1.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["GST"].Value);
                textBox2.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["HSNCODE"].Value);
                //textBox3.Text = Convert.ToString(dgvProduct.Rows[e.RowIndex].Cells["IGST"].Value);
                //if (!string.IsNullOrEmpty(Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Tax"].Value)))
                //{
                //    cmbTax.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Tax"].Value);
                //}
                //else
                //{
                //    cmbTax.SelectedIndex = 0;
                //}


                dropmeasure.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["sizwuom"].Value);
                ddlcategorys.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["categoryname"].Value);
                ddlBrands.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Brandname"].Value);
                ddlcolors.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["color"].Value);
                txtRack.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Rack"].Value);
                dropunitofmeasure.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["UOM"].Value);
                txtMinimunStock.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["MinStock"].Value);
                txtMaximunStock.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["MaxStock"].Value);
                txtreorderqty.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ReorderQty"].Value);
                txtreorderpt.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ReorderPoint"].Value);
                filename = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Imagepath"].Value);
                txtRemarks.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Remarks"].Value);
                if (!string.IsNullOrEmpty(filename))
                {
                    //pcphoto.Image = new Bitmap(imgpath);
                    pcphoto.ImageLocation = filename;
                    pcphoto.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                DataTable dt = ProductBAL.GetProductLocation(lblhidden.Text);

                DataTable productlication = new DataTable();
                productlication.Columns.Add("LocationID", typeof(int));
                productlication.Columns.Add("Rack", typeof(string));
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    // dr["LocationID"] = dt.Rows[i][0];
                    string rackname = Convert.ToString(dt.Rows[i][1]);
                    string[] reck = rackname.Split(',');
                    for (int h = 0; h < reck.Count(); h++)
                    {
                        DataRow dr = productlication.NewRow();
                        dr["LocationID"] = dt.Rows[i][0];
                        dr["Rack"] = reck[h];
                        productlication.Rows.Add(dr);
                    }

                    // locationinfo.Rows[i].Cells[0].Value=dt.Rows[i][0];
                    // GetLocationRack(Convert.ToString(dt.Rows[i][0]), Convert.ToString(i), Convert.ToInt32(dt.Rows[i][1]));
                    //// locationinfo.Rows[i].Cells["LocationRack"].Value = dt.Rows[i][1];

                    // locationinfo.Rows[i].Cells[3].Value = dt.Rows[i][2];
                }
                int count = productlication.Rows.Count;
               

            }
        }

        private void chkBoxBarcode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBoxBarcode.Checked)
            {
                lblborcode.Visible = true;
                txtbarcodes.Visible = true;
            }

            else
            {
                lblborcode.Visible = false;
                txtbarcodes.Visible = false;
            }
        }

        private void dgvProduct_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "ItemCode")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "ItemName")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "ISBarCodeable")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 110;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "BarCode")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "Size")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 50;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "sizwuom")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 50;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "categoryname")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 150;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "Brandname")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "color")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 50;
            }
            //if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "Rack")
            //{
            //    dgvProduct.Columns[e.ColumnIndex].Width = 50;
            //}
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "UOM")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 50;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "MinStock")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 75;
            }

            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "Delete")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 75;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "MaxStock")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 75;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "ReorderQty")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 90;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "ReorderPoint")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 95;
            }
            if (dgvProduct.Columns[e.ColumnIndex].HeaderText == "Remarks")
            {
                dgvProduct.Columns[e.ColumnIndex].Width = 100;
            }
        }



        private void Locationrackbttn_Click(object sender, EventArgs e)
        {


            //for (int l = 0; l < locationinfo.Rows.Count; l++)
            //{
            //    locationinfo["Location", l].Value = 0;
            //}
        }

        private void locationinfo_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            

        }



        private void cmblocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (locationinfo.CurrentCell.ColumnIndex == 0)
            //{
            //    ComboBox cmb = (ComboBox)sender;
            //    string cmbval = Convert.ToString(Convert.ToString(cmb.SelectedValue));
            //    if (!string.IsNullOrEmpty(cmbval))
            //    {
            //        if (cmbval != "0" && cmbval != "System.Data.DataRowView")
            //        {
            //            GetLocationRack(Convert.ToString(cmb.SelectedValue), "", "0");
            //        }
            //    }
            //}
        }

        //public void GetLocationRack(string locID, string row1, string val)
        //{

        //    DataTable dtlocation = ProductBAL.GetLocationRack(locID);
        //    DataRow row = dtlocation.NewRow();
        //    row["RackId"] = "0";
        //    row["RackName"] = "--Select--";
        //    dtlocation.Rows.InsertAt(row, 0);
        //    //LocationRack
           
        //    else
        //    {
        //        DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
        //        cell.DataSource = dtlocation;
        //        cell.ValueMember = "RackId";
        //        cell.DisplayMember = "RackName";
        //        locationinfo.Rows[Convert.ToInt32(row1)].Cells["LocationRack"] = cell;
        //        //locationinfo.Rows[Convert.ToInt32(row1].Cells["LocationRack"]).FormattedValue.ToString()=val;

        //        locationinfo.Rows[Convert.ToInt32(row1)].Cells["LocationRack"].Value = val;

        //    }




        //}

        public void binddgvlocationrack(string row1)
        {

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[GetLocationForProd]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                DataRow row = dt.NewRow();
                row["LocationID"] = "0";
                row["LocationName"] = "--Select--";
                dt.Rows.InsertAt(row, 0);

                DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                cell.DataSource = dt;
                cell.ValueMember = "LocationID";
                cell.DisplayMember = "LocationName";
                cell.FlatStyle = FlatStyle.Popup;
                



            }
        }

        private void locationinfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void locationinfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = false;
        }



        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
        }

        private void txtSalesPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back) || (e.KeyChar == '.')))
                e.Handled = true;
            if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar))
                                && (e.KeyChar != '.'))
                e.Handled = true;


            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            // only allow minus sign at the beginning
            //if (e.KeyChar == '-' && (sender as TextBox).Text.Length > 0)
            //{
            //    e.Handled = true;
            //}
        }

        private void txtMinimunStock_TextChanged(object sender, EventArgs e)
        {

        }

        



    }
}
