using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using InvBal;
using InvDal;

namespace Inventory.Purchase_Return
{
    public partial class FrmPurchaseReturn : Form
    {
        public TextBox tb;
        public bool edit = false;
        //string role1 = string.Empty;
        string srole = string.Empty;
        string UserId = "";
        string role1 = "";
        string cas = string.Empty;
        int ProdSelRowvalue = 0;
        bool res = false;
        DataTable dtitems;
        string test;
        bool ProdNotFoundMSg = false;
        string selectedtab = string.Empty;
        TextBox tbamount, tbbaalanceanount, tborderquantoty;
        QuotationBal objQuotationbal = new QuotationBal();
        PurchaseReturnDAL objPurchaseReturnDAL = new PurchaseReturnDAL();

        string clickstatus = string.Empty;
        public FrmPurchaseReturn()
        {
            InitializeComponent();
            LoadPortsChecking();
            srole = Program.userid;
            if (srole == "1")
            {
                role1 = "Admin";
            }
            else
            {
                role1 = "Emp";
            }
            this.WindowState = FormWindowState.Maximized;
            LoadPorts();
            GetSuppliers();
            bindLocation();

            this.ActiveControl = cbVendor;
            SearchPurchaseOrder();
         
            DataTable dtitems = Program.dtitems;
            if (dtitems == null)
            {
                itemdetails("");
            }

            cmbloaction.SelectedIndex = 0;
            //Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //Txtitem.AutoCompleteCustomSource = AutoCompleteLoad();
            //Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;

            string tabname = tabControl1.SelectedTab.Name;

            if (tabname == "TabPurchaseReturn")
            {
               // search("PurchaseRtnID", "", "sr.Updatedon", "Today", "Name", "", role1, Program.userid);
                Btnsearch();

                butsave.Enabled = true;

                btnNew.Enabled = true;
                butprint.Enabled = true;
                butprint.Enabled = true;
            }
            else if (tabname == "TabApproval")
            {
                //search("PurchaseRtnID", "", "sr.Updatedon", "Today", "Name", "", role1, Program.userid);
                Btnsearch();

                butsave.Enabled = false;

                btnNew.Enabled = false;
                butprint.Enabled = false;
                butclear.Enabled = true;
            }


            else if (tabname == "TabCheckOut")
            {
                BtnsearchCheckOut();
              


                butprint.Enabled = true;

            }

            DataTable dt1 = Program.Dtmenu;
            bool contains = dt1.AsEnumerable()
                .Any(row => "PurchaseReturnApproval" == row.Field<String>("Data"));
            if (contains == false)
            {
                tabControl1.TabPages.Remove(TabApproval);
            }

            bool contains1 = dt1.AsEnumerable()
            .Any(row => "PurchaseReturnCheckout" == row.Field<String>("Data"));
            if (contains1 == false)
            {
                tabControl1.TabPages.Remove(TabCheckOut);
            }

        }
        public void bindLocation()
        {
            cmbloaction.DataSource = PurchaseReturnBAL.getLocation();
            cmbloaction.DisplayMember = "LocationName";
            cmbloaction.ValueMember = "LocationID";
        }

     
        public void GetSuppliers()
        {
            DataTable dtsup = PurchaseReturnBAL.GetSuppliers(null);
            cbVendor.DataSource = dtsup;
            cbVendor.ValueMember = "SuppliersID";
            cbVendor.DisplayMember = "Name";
            cbVendor.SelectedIndex = 0;

        }
        private void LoadPortsChecking()
        {
            dgvChecking.Rows.Clear();
            dgvChecking.ColumnCount = 6;


            dgvChecking.Columns[0].Name = "S.NO";
            dgvChecking.Columns[1].Name = "Items";
            dgvChecking.Columns[2].Name = "originalQuantity";
            dgvChecking.Columns[3].Name = "Rack";

            dgvChecking.Columns[4].Name = "Productid";
            dgvChecking.Columns[5].Name = "Quantity";




            this.dgvChecking.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvChecking.Columns[0].Width = 70;


            this.dgvChecking.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvChecking.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvChecking.Columns[1].Width = 120;
            this.dgvChecking.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvChecking.Columns[0].ReadOnly = true;
            this.dgvChecking.Columns[1].ReadOnly = true;


            this.dgvChecking.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvChecking.Columns[2].Visible = false;
            this.dgvChecking.Columns[4].Visible = false;
            //this.dgvChecking.Columns[3].Visible = false;
            DataGridViewComboBoxColumn name = new DataGridViewComboBoxColumn();
            name.HeaderText = "Location";
            name.DataPropertyName = "Location";
            name.FlatStyle = FlatStyle.Popup;
            dgvChecking.Columns.Insert(3, name);








            dgvChecking.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvChecking.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }
        private void LoadPorts()
        {
            dgvOrder.Rows.Clear();
            dgvOrder.Columns.Clear();
            dgvOrder.ColumnCount = 7;
            dgvOrder.RowCount = 1;

            dgvOrder.Columns[0].Name = "S.No";
            dgvOrder.Columns[1].Name = "Items";
            dgvOrder.Columns[2].Name = "Rate";
            dgvOrder.Columns[3].Name = "Quantity";
            dgvOrder.Columns[4].Name = "Amount";

            dgvOrder.Columns[5].Name = "ProductId";
            dgvOrder.Columns[6].Name = "LocationId";




            //this.dgvOrder.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[0].Width = 60;

            //this.dgvOrder.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;            
            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[1].Width = 300;
            this.dgvOrder.Columns[1].ReadOnly = true;

            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[2].Width = 80;
            this.dgvOrder.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvOrder.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[3].Width = 90;
            this.dgvOrder.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvOrder.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[4].Width = 100;
            this.dgvOrder.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvOrder.Columns[5].Visible = false;
            dgvOrder.Columns[6].Visible = false;
            this.dgvOrder.Columns[2].ReadOnly = true;
            this.dgvOrder.Columns[4].ReadOnly = true;



            //this.dgvOrder.Columns[4].ReadOnly = true;
            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }

        }



        public void AutoCompleteLoad(string s, int t)
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.itemauto(s, t);

            if (st.Rows.Count > 0)
            {
                DgvAutoRefNo.Visible = true;
                DgvAutoRefNo.DataSource = st;
                res = false;
                cas = Convert.ToString(DgvAutoRefNo.Rows[0].Cells[0].Value);

                DgvAutoRefNo.Focus();
                DgvAutoRefNo.CurrentCell = DgvAutoRefNo[0, 0];
                string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                getitems(sa);
            }
            else
            {
                DgvAutoRefNo.Visible = false;
                lblproductid.Text = string.Empty;
                //Txtitem.Text = string.Empty;
                lblitemcode.Text = "0";
                lblrack.Text = "0";
                lbldisplay.Text = "0";
                DefaultFloor.Text = "0";
                Display.Text = "0";
                Damage.Text = "0";
                lblprice.Text = "0";
            }
        }


        public void GetSuppliersDetails(int suplierid)
        {
            DataTable dt = PurchaseReturnBAL.GetPurcahseReturnDetails(suplierid);
            txtCity.Text = Convert.ToString(dt.Rows[0]["City"]);
            txtbal.Text = Convert.ToString(dt.Rows[0]["Balance"]);
            txtPaid.Text = Convert.ToString(dt.Rows[0]["Paid"]);


        }

        private void cbVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVendor.SelectedIndex > 0)
            {
                GetSuppliersDetails(Convert.ToInt32(cbVendor.SelectedValue));
            }
        }

        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            ProdSelRowvalue = 0;
        }
        private void RefScrollGrid()
        {
            if (DgvAutoRefNo.Rows.Count - 1 >= ProdSelRowvalue)
            {
                DgvAutoRefNo.FirstDisplayedScrollingRowIndex = ProdSelRowvalue;
            }
        }

        public void itemdetails()
        {

            try
            {

                string s1 = Txtitem.Text.Trim();
                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s1.Replace("'", "''");


                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                DataTable st = objQuotationbal.itemdetails(name, s2);


                if (st.Rows.Count > 0)
                {
                    lblitem.Text = name;

                    lblitemcode.Text = Convert.ToString(st.Rows[0]["UOM"]);
                    if (lblitemcode.Text == "")
                    {
                        lblitemcode.Text = "0";
                    }


                    lblproductid.Text = Convert.ToString(st.Rows[0]["ProductId"]);
                    if (lblproductid.Text == "")
                    {
                        lblproductid.Text = "0";
                    }

                    lblprice.Text = Convert.ToString(st.Rows[0]["Price"]);
                    if (lblprice.Text == "")
                    {
                        lblprice.Text = "0";
                    }

                    DefaultFloor.Text = Convert.ToString(st.Rows[0]["DefaultFloor"]);
                    if (DefaultFloor.Text == "")
                    {
                        DefaultFloor.Text = "0";
                    }

                    Checking.Text = Convert.ToString(st.Rows[0]["Checking"]);
                    if (Checking.Text == "")
                    {
                        Checking.Text = "0";
                    }


                    Display.Text = Convert.ToString(st.Rows[0]["Display"]);

                    if (Display.Text == "")
                    {
                        Display.Text = "0";
                    }


                    Damage.Text = Convert.ToString(st.Rows[0]["Damage"]);
                    if (Damage.Text == "")
                    {
                        Damage.Text = "0";
                    }

                    Delivery.Text = Convert.ToString(st.Rows[0]["Delivery"]);
                    if (Delivery.Text == "")
                    {
                        Delivery.Text = "0";
                    }





                    //pictureBox1.ImageLocation = Path.GetFullPath("131353W24J150-43D6.jpg");
                    //pictureBox1.ImageLocation = itemdetails.ToList()[0].imagepath;




                }
                else
                {

                    lblitemcode.Text = "0";
                    lblproductid.Text = "0";
                    lblprice.Text = "0";

                    lblrack.Text = "0";
                    lbldisplay.Text = "0";


                }

            }
            catch (Exception e)
            {

            }

        }

        private void Txtitem_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void dgvOrder_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                return;
            }
            var dataGridView = (sender as DataGridView);

            if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
                dataGridView.Cursor = Cursors.Hand;
            else
                dataGridView.Cursor = Cursors.Default;
        }

        private void dgvOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvOrder.CurrentCell.ColumnIndex;
            string headerText = dgvOrder.Columns[column].HeaderText;
            if (headerText.Equals("Quantity"))
            {
                tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.TextChanged += new EventHandler(textbox_Change);
                    tb.MaxLength = 10;
                }
            }
        }


        private void textbox_Change(object sender, EventArgs e)
        {


        }

        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
            //if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            //    e.Handled = true;


            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
      (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void dgvOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // MessageBox.Show(e.ColumnIndex.ToString());
        }

        public void total()
        {
            double totalamount = 0, totalquantity = 0;
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {

                totalquantity = totalquantity + Convert.ToDouble(dgvOrder.Rows[i].Cells[4].Value);
            }

            string[] str = totalquantity.ToString().Split('.');
            if (str.Length > 1)
            {
                double num1 = Convert.ToDouble("0." + str[1]);

                if (num1 >= 0.5)
                {
                    totalquantity = Math.Ceiling(totalquantity);
                }
                else
                {
                    totalquantity = Math.Floor(totalquantity);
                }

            }

            //totalquantity = Math.Round(totalquantity);

            lbltotalamount.Text = Convert.ToString(totalquantity);


        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            if (keyData == (Keys.Alt | Keys.Insert))
            {
                if (dgvOrder.Rows.Count <= 0)
                {
                    dgvOrder.Rows.Add();
                }
                else
                {
                    int rowindex = dgvOrder.CurrentRow.Index;
                    int colindex = dgvOrder.CurrentCell.ColumnIndex;
                    //dgvOrder.Rows.Insert(rowindex, dgvOrder.Rows.Add(1));
                    dgvOrder.Rows.Insert(rowindex, 1);
                    getsino();
                    return true;
                }



            }

            if (keyData == (Keys.Alt | Keys.Delete))
            {
                if (dgvOrder.Rows.Count == 1)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[0].Cells[1].Value)))
                    {
                        DialogResult result = MessageBox.Show("Do You Want To Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            if (dgvOrder.Rows.Count > 0)
                            {
                                int rowindex = dgvOrder.CurrentRow.Index;
                                int colindex = dgvOrder.CurrentCell.ColumnIndex;
                                dgvOrder.Rows.RemoveAt(rowindex);
                                dgvOrder.Rows.Add(1);
                                dgvOrder.CurrentCell = dgvOrder[1, 0];
                                pnsearch.Visible = false;
                                getsino();
                                return true;
                            }

                        }
                    }
                    //else
                    //{
                    //    MessageBox.Show("Cant Be Deleted");
                    //}
                }
                else
                {
                    DialogResult result = MessageBox.Show("Do You Want To Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (dgvOrder.Rows.Count > 0)
                        {
                            int rowindex = dgvOrder.CurrentRow.Index;
                            int colindex = dgvOrder.CurrentCell.ColumnIndex;

                            try
                            {
                                dgvOrder.Rows.RemoveAt(rowindex);
                                dgvOrder.CurrentCell = dgvOrder[1, rowindex - 1];
                            }
                            catch
                            { }
                            pnsearch.Visible = false;
                            getsino();
                            return true;
                        }

                    }

                }

                //getsino();

            }



            if (keyData == (Keys.Alt | Keys.S))
            {
                rdbStartsWith.Checked = true;
                return true;
            }
            if (keyData == (Keys.Alt | Keys.C))
            {
                rdbContains.Checked = true;
                return true;
            }

            if (transactionclose.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    //this.ActiveControl = txtRemarks;
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex + 1, dgvOrder.CurrentCell.RowIndex];
                    this.ActiveControl = dgvOrder;
                    //dgvOrder.BeginEdit(true);
                    pnsearch.Visible = false;
                    return true;
                }
            }

            if (cbVendor.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    //this.ActiveControl = txtRemarks;
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex];
                    this.ActiveControl = dgvOrder;
                    //dgvOrder.BeginEdit(true);
                    //pnsearch.Visible = false;
                    return true;
                }
            }
          
            if (btnSearch.Focused)
            {
                if (keyData == (Keys.Tab))
                {

                    cbVendor.Focus();

                    this.ActiveControl = cbVendor;

                    return true;
                }
            }

            //if (keyData == Keys.Escape)
            //{
            //    pnsearch.Visible = false;
            //    dgvOrder.Focus();
            //    dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
            //    dgvOrder.BeginEdit(true);
            //    return true;
            //}

            if (keyData == Keys.Escape)
            {
                if (pnsearch.Visible)
                {
                    pnsearch.Visible = false;
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
                    dgvOrder.BeginEdit(true);
                    return true;
                }
                else
                {
                    if (dgvOrder.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[0].Cells[1].Value)))
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
                }
                return true;
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void dgvOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dgvOrder.CurrentCell.ColumnIndex == 0)
                {
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex];
                    dgvOrder.BeginEdit(true);

                }
                else if (dgvOrder.CurrentCell.ColumnIndex == 1)
                {
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];

                }
                else if (dgvOrder.CurrentCell.ColumnIndex == 2)
                {
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[3, dgvOrder.CurrentCell.RowIndex];
                    dgvOrder.BeginEdit(true);
                }


            }
        }

        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 3)
                {
                    dgvOrder.Focus();
                    edit = true;
                    string val = Convert.ToString(dgvOrder.Rows[e.RowIndex].Cells[1].Value);
                    string qtyval = Convert.ToString(dgvOrder.Rows[e.RowIndex].Cells[3].Value);



                    if (!string.IsNullOrEmpty(val) && !string.IsNullOrEmpty(qtyval))
                    {
                        if (Convert.ToString(qtyval) == "" || Convert.ToString(qtyval) == "0")
                        {
                            dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex].Value = 1;
                            tb.Text = "1";
                        }

                        dgvOrder.Rows.Add(1);
                        dgvOrder.CurrentCell = dgvOrder[1, e.RowIndex + 1];
                        dgvOrder.BeginEdit(true);
                    }

                }

            }
            catch
            {

            }

        }

        private void dgvOrder_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void dgvOrder_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                string itemval = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value);
                if (!string.IsNullOrEmpty(itemval))
                {
                    try
                    {
                        double rate = Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value);
                        if (!String.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value)))
                        {
                            double amt = 0;
                            //amt = rate * Convert.ToDouble(tb.Text);
                            amt = rate * Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value);
                            string[] str = amt.ToString().Split('.');
                            if (str.Length > 1)
                            {
                                double num1 = Convert.ToDouble("0." + str[1]);

                                if (num1 >= 0.5)
                                {
                                    amt = Math.Ceiling(amt);
                                }
                                else
                                {
                                    amt = Math.Floor(amt);
                                }

                            }
                            dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = amt;
                        }


                    }
                    catch
                    {

                        if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                        {
                            if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value)))
                            {
                                dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = 0.00;
                            }
                        }
                        //dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = 0;
                    }

                    total();
                }
            }
        }

        private void dgvOrder_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                pnsearch.Visible = true;
                if (!string.IsNullOrEmpty(lblhiddenproduct.Text))
                {
                    Txtitem.Text = lblhiddenproduct.Text;
                    AutoCompleteLoad(Txtitem.Text, 1);
                    if (DgvAutoRefNo.Rows.Count > 0)
                    {
                        DgvAutoRefNo.Rows[0].Cells[0].Selected = false;
                        DefaultFloor.Text = "0";
                        Display.Text = "0";
                        Damage.Text = "0";
                        Checking.Text = "0";
                        Delivery.Text = "0";
                        lblprice.Text = "0";
                    }


                }
                Txtitem.SelectionStart = 0;
                Txtitem.SelectionLength = Txtitem.Text.Length;
                this.ActiveControl = Txtitem;
                lblrowindex.Text = e.RowIndex.ToString();
                lblpageno.Text = Convert.ToString(Convert.ToInt32(lblpageno.Text) + 1);
            }

            //else if (e.ColumnIndex == 4)
            //{
            //    dgvOrder.CancelEdit();
            //    dgvOrder.CurrentCell = dgvOrder[1, e.RowIndex + 1];
            //}
            else
            {
                pnsearch.Visible = false; ;
            }
        }


        private void butsave_Click(object sender, EventArgs e)
        {
            string tabname = tabControl1.SelectedTab.Name;

            if (tabname == "TabPurchaseReturn")
            {
                if (ValidationNew())
                {
                    SaveNew();
                    clear();
                }
            }
            else if (tabname == "TabApproval")
            {

            }
            else if (tabname == "TabCheckOut")
            {
                if (Validationchecking())
                {
                    string st = string.Empty;
                    PurchaseReturnBAL objPurchaseReturnBAL = new PurchaseReturnBAL();
                    for (int i = 0; i < dgvChecking.Rows.Count; i++)
                    {
                        objPurchaseReturnBAL.PHeadID = txorderno.Text;
                        objPurchaseReturnBAL.ProductID = Convert.ToString(dgvChecking.Rows[i].Cells["Productid"].Value);
                        objPurchaseReturnBAL.Quantity = Convert.ToString(dgvChecking.Rows[i].Cells[2].Value);
                        objPurchaseReturnBAL.LocationID = Convert.ToString(dgvChecking.Rows[i].Cells[3].Value);
                        st = PurchaseReturnBAL.SavePurchaseRtnMatTrans(objPurchaseReturnBAL);

                    }
                    if (!string.IsNullOrEmpty(st))
                    {
                        MessageBox.Show("Purchase Return successfully checkedout");
                        BtnsearchCheckOut();
                        itemdetailsval("");
                    }
                    else
                    {
                        MessageBox.Show("Purchase Return failed");
                    }

                    clearchecked();
                }

                else
                {
                    MessageBox.Show("Quantity should not be empty");
                }
            }
        }

        public void itemdetailsval(string s)
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

        private bool Validationchecking()
        {
            bool status = true;
            string message = "";
            for (int i = 0; i < dgvChecking.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dgvChecking.Rows[i].Cells[6].Value)))
                {
                    status = false;
                }

            }

            return status;
        }
        public void clearchecked()
        {
            txorderno.Text = string.Empty;
            txtvendorname.Text = string.Empty;
            dgvChecking.Rows.Clear();
            butsave.Enabled = false;
        }
        public bool ValidationNew()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (cbVendor.Text == "--Select--")
            {
                i++;
                message = message + "* Please select from vendor" + "\n";
                if (i == 1)
                    this.ActiveControl = cbVendor;
            }
            string val = Convert.ToString(dgvOrder[1, 0].Value);
            if (string.IsNullOrEmpty(val))
            {
                i++;
                message = message + "* Please move atleast one product" + "\n";
                if (i == 1)
                    dgvOrder.CurrentCell = dgvOrder.Rows[0].Cells[1];
                pnsearch.Visible = true;
                this.ActiveControl = Txtitem;
            }
            bool sas = false;

            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                string Items = Convert.ToString(dgvOrder.Rows[k].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvOrder.Rows[k].Cells["Items"].Value);

                if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))) || Items == ".")
                {
                    sas = true;
                    break;
                }
            }
            if (sas == true)
            {
                i++;
                message = message + "*Product or quantity should not be empty." + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }



            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

         public DataTable DataGridView2DataTable(DataGridView dgv, int minRow = 0)
        {

            DataTable dt = new DataTable();

            // Header columns
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                DataColumn dc = new DataColumn(column.Name.ToString());
                dt.Columns.Add(dc);
            }

            // Data cells
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                DataGridViewRow row = dgv.Rows[i];
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    dr[j] = (row.Cells[j].Value == null) ? "" : row.Cells[j].Value.ToString();
                }
                dt.Rows.Add(dr);
            }

            // Related to the bug arround min size when using ExcelLibrary for export
            for (int i = dgv.Rows.Count; i < minRow; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dr[j] = "  ";
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public void SaveNew()
        {
            DataTable dt=DataGridView2DataTable(dgvOrder);
              foreach (DataRow row in dt.Rows)
            {
                if (row["Quantity"].ToString() == "0")
                {
                    //MessageBox.Show("Quanityy");

                   test = "1";
                }
                else
                {
                    test = "0";
                }
            }

              if (test == "1")
              {
                  MessageBox.Show("Quantity Should not be Zero");
              }
              else
              {

                  PurchaseReturnBAL objPurchaseReturnBAL = new PurchaseReturnBAL();
                  objPurchaseReturnBAL.VendorID = Convert.ToString(cbVendor.SelectedValue);
                  objPurchaseReturnBAL.Paid = txtPaid.Text;
                  objPurchaseReturnBAL.PBalance = txtbal.Text;
                  objPurchaseReturnBAL.Remarks = txtRemarks.Text;
                  objPurchaseReturnBAL.TotalReturn = lbltotalamount.Text;
                  objPurchaseReturnBAL.UpdatedBy = Program.userid;
                  string HeadId = PurchaseReturnBAL.SavePurchaseHeader(objPurchaseReturnBAL);
                  for (int i = 0; i < dgvOrder.Rows.Count; i++)
                  {
                      if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[1].Value)))
                      {
                          objPurchaseReturnBAL.PHeadID = HeadId;
                          objPurchaseReturnBAL.ProductName = Convert.ToString(dgvOrder.Rows[i].Cells[1].Value);
                          objPurchaseReturnBAL.Quantity = Convert.ToString(dgvOrder.Rows[i].Cells[3].Value);
                          objPurchaseReturnBAL.ProductID = Convert.ToString(dgvOrder.Rows[i].Cells[5].Value);
                          objPurchaseReturnBAL.Rate = Convert.ToString(dgvOrder.Rows[i].Cells[2].Value);
                          objPurchaseReturnBAL.Amount = Convert.ToString(dgvOrder.Rows[i].Cells[4].Value);
                          objPurchaseReturnBAL.LocationID = Convert.ToString(dgvOrder.Rows[i].Cells[6].Value);

                          string DetailID = PurchaseReturnBAL.SavePurchaseDetails(objPurchaseReturnBAL);
                      }

                  }

                  Btnsearch();
              }
        }
        public void Getapproval(string s)
        {
            DataTable ds = PurchaseReturnBAL.GetPurcahseApproval(s);
            dgvApproval.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);
            dgvApproval.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn c in dgvApproval.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvApproval.DataSource = ds;
            if (ds.Rows.Count > 0)
            {
                DataGridViewButtonColumn bcol = new DataGridViewButtonColumn();
                bcol.HeaderText = "Approval";
                bcol.Text = "Approved";
                bcol.Name = "Approval";
                bcol.FlatStyle = FlatStyle.Popup;

                bcol.UseColumnTextForButtonValue = true;
                if (dgvApproval.Columns["Approval"] == null)
                {
                    dgvApproval.Columns.Insert(7, bcol);
                }

            }
            else
            {


            }

        }
        private void SearchPurchaseOrder()
        {
            dgvSearch.Rows.Clear();
            dgvSearch.Columns.Clear();
            dgvSearch.ColumnCount = 3;


            dgvSearch.Columns[0].Name = "Order No";
            dgvSearch.Columns[1].Name = "Vendor";
            dgvSearch.Columns[2].Name = "Order Date";


            this.dgvSearch.Columns[0].Width = 60;
            this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvSearch.Columns[1].Width = 120;
            //this.dgvSearch.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvSearch.Columns[2].Width = 30;


            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn c in dgvSearch.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
            this.dgvSearch.Columns[1].Visible = false;
            this.dgvSearch.Columns[2].Visible = false;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        }
        private void clear()
        {
            cbVendor.SelectedIndex = 0;
            txtbal.Clear();
            txtPaid.Clear();
            dgvOrder.Rows.Clear();

            txtCity.Clear();
            txtRemarks.Clear();

            LoadPorts();
            cbVendor.Focus();
        }
        private void dgvApproval_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvApproval.Columns[e.ColumnIndex].HeaderText == "Approval")
                {
                    PurchaseReturnBAL objPurchaseReturnBAL = new PurchaseReturnBAL();
                    objPurchaseReturnBAL.PHeadID = Convert.ToString(dgvApproval.Rows[e.RowIndex].Cells["PurchaseRtnID"].Value);

                    string PurchaseReturnId = Convert.ToString(dgvApproval.Rows[e.RowIndex].Cells["PurchaseRtnID"].Value);
                    string res = PurchaseReturnBAL.SavePurchaseApproval(objPurchaseReturnBAL);
                    DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        PurchaseReturnReport rpt = new PurchaseReturnReport(PurchaseReturnId);
                        rpt.ShowDialog();
                    }
                    Getapproval("");
                  //  search("PurchaseRtnID", "", "sr.Updatedon", "Today", "Name", "", role1, Program.userid);
                    Btnsearch();
                }

            }
        }

        #region search
        public DataTable bindEstimation()
        {
            DataTable dt = new DataTable();
            dt = objPurchaseReturnDAL.itemSalesreturnorderno();
            DataRow dr = dt.NewRow();
            dr["PurchaseRtnID"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);

            return dt;
        }
        public DataTable bindcheckout()
        {
            DataTable dt = new DataTable();
            dt = objPurchaseReturnDAL.getpurchaesreturncheckorderno();
            DataRow dr = dt.NewRow();
            dr["PurchaseRtnID"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);
            return dt;
        }
     
     
        public void Btnsearch()
        {
            DateTime FromDate = new DateTime(dateTimePicker8.Value.Year, dateTimePicker8.Value.Month, dateTimePicker8.Value.Day);
            DateTime ToDate = new DateTime(dateTimePicker9.Value.Year, dateTimePicker9.Value.Month, dateTimePicker9.Value.Day);
            DataTable dt;
            string Ordernumber = textBox2.Text.Trim().ToString();
            string ProductName = textBox3.Text.Trim().ToString();
            string VenderId = textBox1.Text.Trim().ToString();
            search(FromDate, ToDate, Ordernumber, ProductName, VenderId);
        }

        public void BtnsearchCheckOut()
        {
            DateTime FromDate = new DateTime(dateTimePicker8.Value.Year, dateTimePicker8.Value.Month, dateTimePicker8.Value.Day);
            DateTime ToDate = new DateTime(dateTimePicker9.Value.Year, dateTimePicker9.Value.Month, dateTimePicker9.Value.Day);
            DataTable dt;
            string Ordernumber = textBox2.Text.Trim().ToString();
            string ProductName = textBox3.Text.Trim().ToString();
            string VenderId = textBox1.Text.Trim().ToString();
            searchcheck(FromDate, ToDate, Ordernumber, ProductName, VenderId);
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
           
                string tabname = tabControl1.SelectedTab.Name;


                if (tabname == "TabPurchaseReturn" || tabname == "TabApproval")
                {
                    Btnsearch();
                }


                else if (tabname == "TabCheckOut")
                {
                    BtnsearchCheckOut();
                   // searchcheck(firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
                }


            
        }


        public void search(DateTime FromDate, DateTime ToDate, string Ordernumber, string ProductName, string VenderId)
        {
            DataTable dt = objPurchaseReturnDAL.searchPurchaseReturn(FromDate, ToDate, Ordernumber, ProductName, VenderId);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);


        }
        public void searchcheck(DateTime FromDate, DateTime ToDate, string Ordernumber, string ProductName, string VenderId)
        {
            DataTable dt = objPurchaseReturnDAL.searchsalesreturncheck(FromDate, ToDate, Ordernumber, ProductName, VenderId);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

        }

      

        #endregion

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {


            selectedtab = tabControl1.SelectedTab.Name;

            if (selectedtab == "TabPurchaseReturn")
            {
               // search("PurchaseRtnID", "", "sr.Updatedon", "Today", "Name", "", role1, Program.userid);
                Btnsearch();

                butclear.Enabled = true;
                butsave.Enabled = true;

                butprint.Enabled = true;
            }


            else if (selectedtab == "TabApproval")
            {
               //search("PurchaseRtnID", "", "sr.Updatedon", "Today", "Name", "", role1, Program.userid);

                Btnsearch();
                Getapproval("");
                butclear.Enabled = true;
                butsave.Enabled = false;


                butprint.Enabled = true;

            }



            else if (selectedtab == "TabCheckOut")
            {

                BtnsearchCheckOut();

               // Btnsearch();

                btnNew.Enabled = false;
                butclear.Enabled = true;
                butsave.Enabled = false;
                butprint.Enabled = false;
            }

        }



        private void transactionclose_Click(object sender, EventArgs e)
        {

            pnsearch.Visible = false;

        }

        private void butclear_Click(object sender, EventArgs e)
        {
            selectedtab = tabControl1.SelectedTab.Name;
            if (selectedtab == "TabPurchaseReturn")
            {
                clear();
            }
            else if (selectedtab == "TabApproval")
            {
                Getapproval("");
            }
            else if (selectedtab == "TabCheckOut")
            {
                clearchecked();
            }
        }

        private void butprint_Click(object sender, EventArgs e)
        {
            clear();
        }

        public void getEstimation(string s)
        {

            DataSet ds = PurchaseReturnBAL.getPurcahseReturn(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                cbVendor.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0]["VendorId"]);
                txtbal.Text = Convert.ToString(ds.Tables[0].Rows[0]["Balance"]);
                txtPaid.Text = Convert.ToString(ds.Tables[0].Rows[0]["Paid"]);
                txtCity.Text = Convert.ToString(ds.Tables[0].Rows[0]["CITY"]);
                lbltotalamount.Text = Convert.ToString(ds.Tables[0].Rows[0]["TotalReturn"]);
                txtRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);

                pnldet.Enabled = false;
            }
            else
            {
                pnldet.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                dgvOrder.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvOrder.Rows.Add();
                    dgvOrder.Rows[i].Cells[0].Value = i + 1;
                    dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    //dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvOrder.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

                    double qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    dgvOrder.Rows[i].Cells[2].Value = qty;
                    dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);

                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvOrder.Rows[i].Cells[4].Value = amt;
                }
                // panel9.Enabled = false;

                butsave.Enabled = false;
            }
            else
            {
                dgvOrder.Rows.Clear();

                butsave.Enabled = true;
            }
            pnltab1.Enabled = false;


        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedtab = tabControl1.SelectedTab.Name;
            if (e.RowIndex >= 0)
            {
                if (selectedtab == "TabPurchaseReturn")
                {
                    if (e.RowIndex >= 0)
                    {
                        string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                        if (!string.IsNullOrEmpty(s))
                        {
                            getEstimation(s);
                            total();
                            pnltab1.Enabled = false;
                            // Txtcustomername.Focus();
                        }
                        else
                        {
                            clear();
                        }

                    }
                }





                else if (selectedtab == "TabApproval")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        Getapproval(s);

                    }
                    else
                    {
                        clear();
                    }


                }

                else if (selectedtab == "TabCheckOut")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getcheck(s);

                    }
                    else
                    {
                        clear();
                    }


                }

            }
        }
        public void getcheck(string s)
        {
            DataSet ds = PurchaseReturnBAL.GetPurchaseReturnCheck(s);

            if (ds.Tables[0].Rows.Count > 0)
            {
                txorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["PurchaseRtnID"]);
                txtvendorname.Text = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);



                //  Pnloading5.Enabled = false;
            }
            else
            {
                //    Pnloading5.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {

                dgvChecking.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvChecking.Rows.Add();
                    dgvChecking.Rows[i].Cells[0].Value = i + 1;
                    dgvChecking.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvChecking.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    int loc = Convert.ToInt32(ds.Tables[1].Rows[i]["location"]);
                    dgvChecking.Rows[i].Cells["Productid"].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

                    string vals = Convert.ToString(ds.Tables[1].Rows[i]["LocationID"]);
                    DataTable dt = getdatatable(vals);

                    (dgvChecking.Rows[i].Cells[3] as DataGridViewComboBoxCell).DataSource = dt;
                    (dgvChecking.Rows[i].Cells[3] as DataGridViewComboBoxCell).ValueMember = "id";
                    (dgvChecking.Rows[i].Cells[3] as DataGridViewComboBoxCell).DisplayMember = "location";
                    (dgvChecking.Rows[i].Cells[3] as DataGridViewComboBoxCell).Value = dt.Rows[0][0];
                    string val = Convert.ToString(dt.Rows[0][0]);
                    string val1 = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    string v = getrack(val, val1);
                    dgvChecking.Rows[i].Cells["Rack"].Value = v;


                    //dgvChecking.Columns[0].Width = 50;
                    //dgvChecking.Columns[1].Width = 600;
                    //dgvChecking.Columns[3].Width = 150;
                    //dgvChecking.Columns[4].Width = 70;

                    dgvChecking.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvChecking.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgvChecking.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                    dgvChecking.DefaultCellStyle.BackColor = Color.Gainsboro;
                    dgvChecking.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

                    Rectangle resolution = Screen.PrimaryScreen.Bounds;
                    int w = resolution.Width;
                    int h = resolution.Height;

                    if (w == 1024 && h == 768)
                    {
                        dgvChecking.Columns[0].Width = 50;
                        dgvChecking.Columns[1].Width = 300;
                        dgvChecking.Columns[3].Width = 150;
                        dgvChecking.Columns[4].Width = 70;
                    }
                    else
                    {
                        dgvChecking.Columns[0].Width = 50;
                        dgvChecking.Columns[1].Width = 500;
                        dgvChecking.Columns[3].Width = 150;
                        dgvChecking.Columns[4].Width = 70;
                    }


                    dgvChecking.Columns[0].ReadOnly = true;
                    dgvChecking.Columns[1].ReadOnly = true;
                    dgvChecking.Columns[3].ReadOnly = true;
                    dgvChecking.Columns[4].ReadOnly = true;

                    dgvChecking.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    // dgvChecking.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgvChecking.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                    dgvChecking.DefaultCellStyle.BackColor = Color.Gainsboro;
                    dgvChecking.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                    butsave.Enabled = true;
                }
                //  Pnloading5.Enabled = false;
            }
            else
            {
                dgvChecking.Rows.Clear();
                //    Pnloading5.Enabled = true;
            }

        }
        public DataTable getdatatable(string itemsToAdd)
        {
            DataTable table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("location", typeof(string));


            string[] item = itemsToAdd.Split(';');

            for (int i = 0; i < item.Length; i++)
            {
                string[] val = item[i].Split(',');
                table.Rows.Add(val[0], val[1]);
            }
            return table;
        }

        public string getrack(string s, string s1)
        {

            string v = PurchaseReturnBAL.getrack(s, s1);
            return v;
        }

        private void dgvChecking_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex != 6)
                {
                    if (dgvChecking[e.ColumnIndex, e.RowIndex].EditType.ToString() == "System.Windows.Forms.DataGridViewComboBoxEditingControl")
                    {
                        DataGridViewColumn column = dgvChecking.Columns[e.ColumnIndex];
                        if (column is DataGridViewComboBoxColumn)
                        {
                            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dgvChecking[e.ColumnIndex, e.RowIndex];
                            dgvChecking.CurrentCell = cell;
                            dgvChecking.BeginEdit(true);
                            DataGridViewComboBoxEditingControl editingControl = (DataGridViewComboBoxEditingControl)dgvChecking.EditingControl;
                            editingControl.DroppedDown = true;
                        }
                    }
                }
            }
            catch
            {

            }
        }
        private void dgvChecking_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (edit == true)
                {
                    if (dgvChecking.CurrentCell.RowIndex >= 1)
                    {
                        dgvChecking.CurrentCell = dgvChecking[dgvChecking.CurrentCell.ColumnIndex, dgvChecking.CurrentCell.RowIndex - 1];
                        edit = false;
                    }
                    else if (dgvChecking.CurrentCell.RowIndex == 0)
                    {
                        dgvChecking.CurrentCell = dgvChecking[dgvChecking.CurrentCell.ColumnIndex, dgvChecking.CurrentCell.RowIndex - 1];
                    }

                }
            }
            catch
            {

            }
        }
        private void dgvChecking_DataError_1(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
        private void dgvChecking_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 6)
                {
                    if (Convert.ToString(dgvChecking.Rows[e.RowIndex].Cells[2].Value) != tborderquantoty.Text)
                    {
                        MessageBox.Show("Please Enter Correct Quantity");
                        dgvChecking.Focus();
                        edit = true;
                        dgvChecking.CurrentCell = dgvChecking[6, e.RowIndex];
                        dgvChecking.Rows[e.RowIndex].Cells[6].Value = "";
                        dgvChecking.Rows[e.RowIndex].Cells[6].Selected = true;
                        dgvOrder.BeginEdit(true);

                    }
                }
            }
            catch
            {

            }
        }

        private void dgvChecking_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvChecking.CurrentCell.ColumnIndex;
            string headerText = dgvChecking.Columns[column].HeaderText;

            if (headerText.Equals("Quantity"))
            {
                tborderquantoty = e.Control as TextBox;
                tborderquantoty.KeyPress += new KeyPressEventHandler(textbox_keypress);
                tborderquantoty.MaxLength = 6;
            }

            else if (headerText.Equals("Location"))
            {
                cmbloaction = e.Control as ComboBox;


                if (cmbloaction != null)
                {
                    cmbloaction.SelectedIndexChanged += new EventHandler(cmbloaction_SelectedIndexChanged);

                }
            }

        }


        private void cmbloaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox cmb = (ComboBox)sender;
                if (cmb.SelectedIndex >= 0)
                {
                    string val = Convert.ToString(cmb.SelectedValue);
                    string val1 = Convert.ToString(dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].Cells["Productid"].Value);
                    string v = getrack(val, val1);
                    dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].Cells["Rack"].Value = v;
                    //dgvChecking.CurrentCell = dgvChecking[6, dgvChecking.CurrentCell.RowIndex];
                }
                //else
                //{
                //    dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].Cells["Rack"].Value = "";
                //}
            }
            catch
            {

            }


        }

        private void dgvChecking_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvChecking_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dgvChecking.CurrentCell.ColumnIndex == 6)
                {
                    if (dgvChecking.CurrentCell.RowIndex == dgvChecking.Rows.Count - 1)
                    {
                        if (e.KeyData == Keys.Enter)
                        {
                            butsave.Enabled = true;
                            butsave.Focus();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            selectedtab = tabControl1.SelectedTab.Name;
            if (selectedtab == "TabPurchaseReturn")
            {

                clear();
                pnltab1.Enabled = true;
            }
            else if (selectedtab == "TabApproval")
            {
                Getapproval("");
            }
            else if (selectedtab == "TabCheckOut")
            {

            }
        }

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

        private void pbxRightCollapse_Click(object sender, EventArgs e)
        {
            if (pnlCollapse2.Visible == true)
            {
                pnlPurchaseReceipt.Visible = true;
                vLabel2.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;
                this.dgvSearch.Columns[1].Visible = true;
                this.dgvSearch.Columns[2].Visible = true;

            }
        }

        private void vLabel2_Click(object sender, EventArgs e)
        {
            if (pnlPurchaseReceipt.Visible == true)
            {
                pnlPurchaseReceipt.Visible = false;
                vLabel2.Visible = false;
                pnlCollapse2.Visible = true;
                splitContainer1.Panel2Collapsed = false;
                pbxCollapse.Visible = true;
                pbxRightCollapse.Visible = true;
                this.dgvSearch.Columns[1].Visible = false;
                this.dgvSearch.Columns[2].Visible = false;

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
            }
        }

        private void FrmPurchaseReturn_Load(object sender, EventArgs e)
        {

        }

        public void itemdetails(string s)
        {

            try
            {
                dtitems = new DataTable();
                string s1 = s.Trim();
                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s1.Replace("'", "''");


                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                dtitems = objQuotationbal.itemdetails(name, s2);
                Program.dtitems = dtitems;


            }
            catch (Exception e)
            {

            }

        }

        private void FrmPurchaseReturn_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int j = 0; j < tabControl1.TabPages.Count; j++)
                {

                    for (int i = 0; i < tabControl1.TabPages[j].Controls.Count; )
                    {
                        tabControl1.TabPages[j].Controls[i].Dispose();
                    }
                    tabControl1.TabPages[j].Dispose();
                }

                tabControl1.Dispose();
            }
            catch
            {

            }
        }

        private void Txtitem_KeyUp(object sender, KeyEventArgs e)
        {
            int typr = 0;
            string word;
            try
            {

                if (e.KeyData != Keys.Enter && e.KeyData != Keys.Tab && e.KeyData != Keys.Down && e.KeyData != Keys.Up && e.KeyData != Keys.Left && e.KeyData != Keys.Right && e.KeyData != Keys.Escape && e.KeyData != Keys.F2 && e.KeyData != (Keys.S | Keys.Alt) && e.KeyData != (Keys.C | Keys.Alt) && e.KeyData != (Keys.N | Keys.Alt) && e.KeyData != (Keys.V | Keys.Alt) && e.KeyData != (Keys.M | Keys.Alt) && e.KeyData != (Keys.D | Keys.Alt) && e.KeyData != (Keys.X | Keys.Alt))
                {
                    word = Txtitem.Text;
                    if (rdbStartsWith.Checked)
                    {
                        typr = 1;
                    }
                    else if (rdbContains.Checked)
                    {
                        typr = 2;
                    }
                    AutoCompleteLoad(word, typr);


                }
                if (e.KeyData == Keys.Up)
                {

                    //try
                    //{
                    //    DataGridViewRow theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue - 1];
                    //    if (theRow3.Index != DgvAutoRefNo.RowCount)
                    //    {

                    //        theRow3.DefaultCellStyle.BackColor = Color.LightGray;

                    //        theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue];
                    //        theRow3.DefaultCellStyle.BackColor = Color.White;

                    //        ProdSelRowvalue--;
                    //        cas = Convert.ToString(DgvAutoRefNo[0, ProdSelRowvalue].Value);
                    //        itemdetails(cas);
                    //        RefScrollGrid();
                    //    }
                    //}
                    //catch
                    //{
                    //    //ProdSelRowvalue = 0;
                    //}

                }
                if (e.KeyData == Keys.Down)
                {

                    if (DgvAutoRefNo.Rows.Count > 0)
                    {
                        DgvAutoRefNo.Focus();
                        DgvAutoRefNo.CurrentCell = DgvAutoRefNo[0, 0];
                        DgvAutoRefNo.Rows[0].Cells[0].Selected = true;
                        string sa = Convert.ToString(DgvAutoRefNo.Rows[0].Cells[0].Value);
                        getitems(sa);
                    }
                    //try
                    //{
                    //    if (DgvAutoRefNo.Rows.Count - 1 != ProdSelRowvalue)
                    //    {
                    //        DataGridViewRow theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue + 1];
                    //        if (theRow3.Index != DgvAutoRefNo.RowCount)
                    //        {

                    //            theRow3.DefaultCellStyle.BackColor = Color.LightGray;

                    //            theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue];
                    //            theRow3.DefaultCellStyle.BackColor = Color.White;

                    //            ProdSelRowvalue++;
                    //            cas = Convert.ToString(DgvAutoRefNo[0, ProdSelRowvalue].Value);
                    //            itemdetails(cas);
                    //            RefScrollGrid();
                    //        }
                    //    }
                    //}
                    //catch
                    //{
                    //    //ProdSelRowvalue = 0;
                    //}

                }

                if (e.KeyData == Keys.Enter)
                {
                    //if (!string.IsNullOrEmpty(Txtitem.Text))
                    //{
                    //    if (res == false)
                    //    {
                    //        if (DgvAutoRefNo.Visible == false)
                    //        {
                    //            DgvAutoRefNo.Visible = false;

                    //        }
                    //        else
                    //        {
                    //            Txtitem.Text = Convert.ToString(DgvAutoRefNo[0, DgvAutoRefNo.CurrentCell.RowIndex].Value);
                    //            DgvAutoRefNo.Visible = false;
                    //            DgvAutoRefNo.Rows[0].Selected = false;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    //if (status6 == false && v == false)
                    //    //{
                    //    //    MessageBox.Show("No records found");
                    //    //    txtRegNo.Focus();
                    //    //    status6 = true;
                    //    //}
                    //    //else
                    //    //{
                    //    //    status6 = false;
                    //    //    v = false;


                    //    //}
                    //}

                }
            }
            catch (Exception efd)
            {

            }
        }

        private void DgvAutoRefNo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (!string.IsNullOrEmpty(Txtitem.Text))
            //{
            if (e.RowIndex >= 0)
            {
                if (ProdNotFoundMSg)
                {
                    LblProdNotFoundMSg.Visible = true;
                }
                else
                {
                    int rowindex = Convert.ToInt32(lblrowindex.Text);
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];

                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                    getitems(sa);

                    //dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    //dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();


                    dgvOrder.Rows[rowindex].Cells[5].Value = lblproductid.Text;
                    dgvOrder.Rows[rowindex].Cells[6].Value = lbllocationid.Text;

                    dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();
                    dgvOrder.Rows[rowindex].Cells[2].Value = lblprice.Text;
                    dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;

                    pnsearch.Visible = false;
                    DgvAutoRefNo.Visible = false;

                    lblproductid.Text = string.Empty;
                    //Txtitem.Text = string.Empty;
                    lblitemcode.Text = "0";
                    lblrack.Text = "0";
                    lbldisplay.Text = "0";
                    lblprice.Text = "0";
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3];
                    dgvOrder.BeginEdit(true);
                    LblProdNotFoundMSg.Visible = false;
                }
            }
            //}
            //else
            //{
            //    MessageBox.Show("Please Enter Product Name");
            //    Txtitem.Focus();
            //}

        }

        public void getsino()
        {
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                dgvOrder.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void DgvAutoRefNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Up) || e.KeyChar != Convert.ToChar(Keys.Down))
            {
                Txtitem.Focus();
                SendKeys.Send(e.KeyChar.ToString());
                lblhiddenproduct.Text = Txtitem.Text;

            }
        }

        private void DgvAutoRefNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(Txtitem.Text))
                {
                    if (ProdNotFoundMSg)
                    {
                        LblProdNotFoundMSg.Visible = true;
                    }
                    else
                    {
                        int rowindex = Convert.ToInt32(lblrowindex.Text);
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];
                        dgvOrder.Rows[rowindex].Cells[5].Value = lblproductid.Text;
                        dgvOrder.Rows[rowindex].Cells[6].Value = lbllocationid.Text;

                        dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                        dgvOrder.Rows[rowindex].Cells[2].Value = lblprice.Text;
                        dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;

                        pnsearch.Visible = false;
                        DgvAutoRefNo.Visible = false;

                        lblproductid.Text = string.Empty;
                        //Txtitem.Text = string.Empty;
                        lblitemcode.Text = "0";
                        lblrack.Text = "0";
                        lbldisplay.Text = "0";
                        lblprice.Text = "0";
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3];
                        dgvOrder.BeginEdit(true);
                        LblProdNotFoundMSg.Visible = false;
                    }
                }
                else
                {
                    this.ActiveControl = butsave;
                    pnsearch.Visible = false;
                    //MessageBox.Show("Please Enter Product Name");
                    //Txtitem.Focus();
                }

            }
            else if (e.KeyData == Keys.Up)
            {
                if (DgvAutoRefNo.CurrentCell.RowIndex != 0)
                {
                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex - 1].Cells[0].Value);
                    getitems(sa);
                }



            }
            else if (e.KeyData == Keys.Down)
            {
                if (DgvAutoRefNo.CurrentCell.RowIndex + 1 != DgvAutoRefNo.Rows.Count)
                {
                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex + 1].Cells[0].Value);
                    getitems(sa);
                }
            }
        }

        public void getitems(string sa)
        {
            dtitems = Program.dtitems;
            var rows = from row in dtitems.AsEnumerable()
                       where row.Field<string>("ProductName").Trim() == sa.Trim()
                       select row;
            DataTable st = rows.CopyToDataTable();

            if (st.Rows.Count > 0)
            {
                lblitem.Text = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);

                lblitemcode.Text = Convert.ToString(st.Rows[0]["UOM"]);
                if (lblitemcode.Text == "")
                {
                    lblitemcode.Text = "0";
                }


                lblproductid.Text = Convert.ToString(st.Rows[0]["ProductId"]);
                if (lblproductid.Text == "")
                {
                    lblproductid.Text = "0";
                }

                lblprice.Text = Convert.ToString(st.Rows[0]["Price"]);
                if (lblprice.Text == "")
                {
                    lblprice.Text = "0";
                }

                DefaultFloor.Text = Convert.ToString(st.Rows[0]["DefaultFloor"]);
                if (DefaultFloor.Text == "")
                {
                    DefaultFloor.Text = "0";
                }

                Checking.Text = Convert.ToString(st.Rows[0]["Checking"]);
                if (Checking.Text == "")
                {
                    Checking.Text = "0";
                }


                Display.Text = Convert.ToString(st.Rows[0]["Display"]);

                if (Display.Text == "")
                {
                    Display.Text = "0";
                }


                Damage.Text = Convert.ToString(st.Rows[0]["Damage"]);
                if (Damage.Text == "")
                {
                    Damage.Text = "0";
                }

                Delivery.Text = Convert.ToString(st.Rows[0]["Delivery"]);
                if (Delivery.Text == "")
                {
                    Delivery.Text = "0";
                }





                //pictureBox1.ImageLocation = Path.GetFullPath("131353W24J150-43D6.jpg");
                //pictureBox1.ImageLocation = itemdetails.ToList()[0].imagepath;




            }
            else
            {

                lblitemcode.Text = "0";
                lblproductid.Text = "0";
                lblprice.Text = "0";

                lblrack.Text = "0";
                lbldisplay.Text = "0";


            }
        }

        private void dgvOrder_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            string itemval = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value);
            if (!string.IsNullOrEmpty(itemval))
            {
                try
                {
                    double rate = Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value);
                    if (!String.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value)))
                    {
                        double amt = 0;
                        //amt = rate * Convert.ToDouble(tb.Text);
                        amt = rate * Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value);
                        string[] str = amt.ToString().Split('.');
                        if (str.Length > 1)
                        {
                            double num1 = Convert.ToDouble("0." + str[1]);

                            if (num1 >= 0.5)
                            {
                                amt = Math.Ceiling(amt);
                            }
                            else
                            {
                                amt = Math.Floor(amt);
                            }

                        }
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = amt;
                    }


                }
                catch
                {

                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value)))
                        {
                            dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = 0.00;
                        }
                    }
                    //dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = 0;
                }





                //try
                //{
                //    double rate = Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value);
                //    double amt = rate * Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value);
                //    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = amt;
                //    //total();


                //}
                //catch
                //{
                //    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = 0;
                //}
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
