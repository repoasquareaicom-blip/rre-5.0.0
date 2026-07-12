using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using InvBal;
using Inventory.Sales;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using QuotationReport;
using System.Collections;
using System.Configuration;
namespace Inventory
{
    public partial class SalesNewQuotation : Form
    {
        public static string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        QuotationBal objQuotationbal = new QuotationBal();
        TextBox tb, tbrate;       
        public bool edit = false;
        int userid = 0;
        string cas = string.Empty;
        string role1 = string.Empty;
        string srole = string.Empty;
        bool res = false;
        int ProdSelRowvalue = 0;     
        DataTable dtitems;
        string clickstatus = string.Empty;       
        bool savevads = false;
        DataTable StockCheck;
        public SalesNewQuotation()
        {
            InitializeComponent();
            srole = Program.Userrole;
            if (srole != "Admin")
            {
                role1 = "Emp";
            }
            else
            {
                role1 = "Admin";
            }
            pnlprodsearchs.Visible = false;
            userid = Convert.ToInt32(Program.userid);
            this.WindowState = FormWindowState.Maximized;
            LoadPorts();
            SearchCreteria1s();
            SearchCreteria2s();
            SearchCreteria3s();
            bindLocations();
            cmbloaction.SelectedIndex = 0;
            //comboBox1.SelectedIndex = 0;
            cmbstatuss.SelectedIndex = 0;
            SearchPurchaseOrders();
            lblperare.Text = Program.Userfullname;
            bindAssists();
            bindAssistsNames();
            bindreferences();
            bindcustomers();
            DataTable dtitems = Program.dtitems;
            if (dtitems == null)
            {
                itemdetails("");
            }
            DataTable dt = bindEstimation();
            cmbstatus3.DataSource = dt;
            cmbstatus3.ValueMember = "Quotationid";
            cmbstatus3.DisplayMember = "Quotationid";
            cmbstatus3.SelectedItem = null;
           

            GetSearchOrder();
           
        }       
        private void btnGSTPrint_Click(object sender, EventArgs e)
        {
            string quotationId = txtorders.Text.Trim();
            if (string.IsNullOrEmpty(quotationId))
            {
                MessageBox.Show("Please save or select a quotation before GST print.", "GST Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            QuotationGSTReport report = new QuotationGSTReport(quotationId);
            report.ShowDialog();
        }

        public void bindcustomers()
        {
            cmbcustomernames.DataSource = objQuotationbal.Getcustomer();
            cmbcustomernames.DisplayMember = "Name";
            cmbcustomernames.ValueMember = "CustomerID";
        }
        public void bindLocations()
        {
            cmbloaction.DataSource = objQuotationbal.getLocation();
            cmbloaction.DisplayMember = "LocationName";
            cmbloaction.ValueMember = "LocationID";
        }
        public void bindreferences()
        {
            cmbreferences.DataSource = objQuotationbal.Getreference();
            cmbreferences.DisplayMember = "Name";
            cmbreferences.ValueMember = "ReferencesID";
        }
        public void bindAssists()
        {
            cmbassistbys.DataSource = objQuotationbal.GetProductsusername();
            cmbassistbys.DisplayMember = "Name";
            cmbassistbys.ValueMember = "employeeid";
        }

        public void bindAssistsNames()
        {
            comboBox1.DataSource = objQuotationbal.GetProductsusername();
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "employeeid";
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
        private void RefScrollGrid()
        {
            if (DgvAutoRefNo.Rows.Count - 1 >= ProdSelRowvalue)
            {
                DgvAutoRefNo.FirstDisplayedScrollingRowIndex = ProdSelRowvalue;
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
                this.dgvSearch.Columns[1].Visible = false;
                this.dgvSearch.Columns[2].Visible = false;

            }
        }
        private void LoadPorts()
        {
            dgvOrders.Rows.Clear();
            dgvOrders.ColumnCount =8;
            //dgvOrders.RowCount = 16;

            dgvOrders.Columns[0].Name = "S.NO";
            dgvOrders.Columns[1].Name = "Items";
            dgvOrders.Columns[2].Name = "UOM";
            dgvOrders.Columns[5].Name = "Quantity";
            dgvOrders.Columns[3].Name = "productid";
            dgvOrders.Columns[4].Name = "Rate";
            dgvOrders.Columns[6].Name = "Amount";
            dgvOrders.Columns[7].Name = "Types";
            dgvOrders.Columns[3].Visible = false;
            dgvOrders.Columns[7].Visible = false;
            this.dgvOrders.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrders.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrders.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrders.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrders.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrders.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrders.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvOrders.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrders.Columns["S.NO"].ReadOnly = true;
            this.dgvOrders.Columns["Items"].ReadOnly = true;
            this.dgvOrders.Columns["UOM"].ReadOnly = true;
            this.dgvOrders.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrders.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrders.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dgvOrders.Columns[4].DefaultCellStyle.Format = "N2";
            dgvOrders.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvOrders.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvOrders.Columns["Rate"].ReadOnly = true;

            this.dgvOrders.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;




            this.dgvOrders.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvOrders.Columns["Amount"].ReadOnly = true;




            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvOrders.Columns[0].Width = 12;
                this.dgvOrders.Columns[1].Width = 100;
                this.dgvOrders.Columns[2].Width = 15;
                this.dgvOrders.Columns[4].Width = 15;
                this.dgvOrders.Columns[5].Width = 20;
                this.dgvOrders.Columns[6].Width = 100;
              
               

            }
            else
            {
                this.dgvOrders.Columns[0].Width = 12;
                this.dgvOrders.Columns[1].Width = 100;
                this.dgvOrders.Columns[2].Width = 15;
                this.dgvOrders.Columns[4].Width = 15;
                this.dgvOrders.Columns[5].Width = 15;
                this.dgvOrders.Columns[6].Width = 100;
               

            }

            dgvOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);



            foreach (DataGridViewColumn c in dgvOrders.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvOrders.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvOrders.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }
        private void SearchCreteria1s()
        {
            List<string> search = new List<string>();
            search.Add("Customer");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
           

        }
        private void SearchCreteria2s()
        {
            List<string> search = new List<string>();
            search.Add("Customer");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
           
        }
        private void SearchCreteria3s()
        {
            List<string> search = new List<string>();
            search.Add("Customer");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            
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
                this.dgvSearch.Columns[1].Visible = true;
                this.dgvSearch.Columns[2].Visible = true;

            }
        }
        private void SalesQuotation1_Load(object sender, EventArgs e)
        {
            this.ActiveControl = cmbcustomernames;
            //clear();

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
          
            {
                try
                {
                    if (keyData == (Keys.Alt | Keys.Insert))
                    {

                        if (dgvOrders.Rows.Count <= 0)
                        {
                            dgvOrders.Rows.Add();
                        }
                        else
                        {
                            int rowindex = dgvOrders.CurrentRow.Index;
                            int colindex = dgvOrders.CurrentCell.ColumnIndex;
                           
                            dgvOrders.Rows.Insert(rowindex, 1);

                            return true;
                        }
                        getsino();

                    }

                    if (keyData == (Keys.Alt | Keys.Delete))
                    {
                        DialogResult result = MessageBox.Show("Do you want to Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            if (dgvOrders.Rows.Count > 0)
                            {
                                try
                                {
                                    int rowindex = dgvOrders.CurrentRow.Index;
                                    int colindex = dgvOrders.CurrentCell.ColumnIndex;
                                    dgvOrders.Rows.RemoveAt(rowindex);
                                }
                                catch
                                {
                                    if (dgvOrders.Rows.Count - 1 == dgvOrders.CurrentCell.RowIndex)
                                    {
                                        dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[0].Value = "";
                                        dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[1].Value = "";
                                        dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[2].Value = "";
                                        dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[3].Value = "";
                                        dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[4].Value = "";
                                        dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[5].Value = "";
                                        dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[6].Value = "";

                                    }
                                }

                            }
                            pnsearch.Visible = false;
                            getsino();
                            return true;
                        }

                        if (dgvOrders.Rows.Count == 0)
                        {
                            dgvOrders.Rows.Add();
                        }
                       

                    }
                }
                catch
                {

                }

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


            if (cmbcustomernames.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    cmdcitys.Focus();
                    return true;
                }
            }
            if (cmbstatus3.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    txtSearchProducts.Focus();
                    return true;
                }
            }


            if (txtSearchProducts.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    textSearchQtys.Focus();
                    return true;
                }
            }

            if (textSearchQtys.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    btnmerge.Focus();
                    return true;
                }
            }
            if (btnmerge.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    btnSearch.Focus();
                    return true;
                }
            }
            if (btnSearch.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    cmbcustomernames.Focus();
                    return true;
                }
            }

            if (keyData == Keys.F3)
            {
                pnlprodsearchs.Visible = true;
                txtprodsearchs.SelectionStart = 0;
                txtprodsearchs.SelectionLength = txtprodsearchs.Text.Length;
                txtprodsearchs.Text = "";
                txtprodsearchs.Focus();
                return true;
            }
            if (txtprodsearchs.Focused)
            {
                if (keyData == (Keys.Enter))
                {
                   
                        GetSuppliersearch();
                        pnlprodsearchs.Visible = false;
                   
                }
            }
            if (cmbassistbys.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    comboBox1.Focus();
                    return true;
                }
            }

            if (comboBox1.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    dgvOrders.Focus();
                    if (dgvOrders.Rows.Count == 0)
                    {
                        dgvOrders.Rows.Add();
                    }
                    dgvOrders.CurrentCell = dgvOrders[1, 0];

                    return true;
                }
            }
            if (keyData == Keys.Escape)
            {
                if (pnsearch.Visible)
                {
                    pnsearch.Visible = false;
                    dgvOrders.Focus();
                    dgvOrders.CurrentCell = dgvOrders[2, dgvOrders.CurrentCell.RowIndex];
                }
                else
                {
                    if (dgvOrders.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dgvOrders.Rows[0].Cells[1].Value)))
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
            if (cmbreferences.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = cmbassistbys;
                    return true;
                }
            }
        

            if (dates.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = dgvOrders;
                    if (dgvOrders.Rows.Count == 0)
                    {
                        dgvOrders.Rows.Add();
                    }
                    return true;
                }

            }

            if (cmbloaction.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = Txtitem;
                    return true;
                }

            }
            try
            {
                if (keyData == Keys.Tab)
                {
                    if (dgvOrders.CurrentCell.ColumnIndex == 5)
                    {
                        dgvOrders.Focus();
                        //edit = true;
                        dgvOrders.CurrentCell = dgvOrders[1, dgvOrders.CurrentCell.RowIndex + 1];
                    }
                }
            }
            catch
            {

            }

         
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public  void GetSuppliersearch()
        {
            DataTable dt = objQuotationbal.GetCustomerNamesearch(txtprodsearchs.Text);
            dgvSearch.Rows.Clear();
            DataTable SearchResult = new DataTable();
            if (dt.Rows.Count > 0)
            {
                try
                {
                    if (textSearchQtys.Text == "")
                        SearchResult = dt.Select("Products like '%" + txtSearchProducts.Text + "%'").CopyToDataTable();

                    else
                        SearchResult = dt.Select("Products like '%" + txtSearchProducts.Text + "%' and QTY like '%|" + textSearchQtys.Text + "|%'").CopyToDataTable();
                }
                catch (Exception ex)
                {
                    SearchResult = new DataTable();
                }

            }

            lblItemCounts.Text = SearchResult.Rows.Count.ToString();


            try
            {
                AlphanumComparator<string> comparer = new AlphanumComparator<string>();
                //DataTable dtNew = dv.Table;
                DataTable dtNew = SearchResult.AsEnumerable().OrderBy(x => x.Field<string>("Quotationid"), comparer).CopyToDataTable();
                //dtNew.TableName = "NaturalSort";
                SearchResult = dtNew;
            }
            catch (Exception ex)
            {

            }


            int SearchResultCount = 0;
            for (int i = 0; i < SearchResult.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(SearchResult.Rows[i]["Quotationid"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(SearchResult.Rows[i]["Customer"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(SearchResult.Rows[i]["Reference"]);
                SearchResultCount++;
            }



        }
        private void SearchPurchaseOrders()
        {
            dgvSearch.Rows.Clear();
            dgvSearch.Columns.Clear();
            dgvSearch.ColumnCount = 3;
            //dgvSearch.RowCount = 16;

            dgvSearch.Columns[0].Name = "Order No";
            dgvSearch.Columns[1].Name = "CustomerName ";
            dgvSearch.Columns[2].Name = "Reference";


            this.dgvSearch.Columns[0].Width = 60;
            this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvSearch.Columns[1].Width = 120;
            //this.dgvSearch.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvSearch.Columns[2].Width = 30;
            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.HeaderText = "";
            checkBoxColumn.Width = 30;
            checkBoxColumn.Name = "checkBoxColumn";
            checkBoxColumn.FlatStyle = FlatStyle.Popup;
            dgvSearch.Columns.Insert(3, checkBoxColumn);
            this.dgvSearch.Columns[3].Width = 10;
            dgvSearch.Columns[0].ReadOnly = true;
            dgvSearch.Columns[1].ReadOnly = true;
            dgvSearch.Columns[2].ReadOnly = true;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn c in dgvSearch.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
            this.dgvSearch.Columns[1].Visible = false;
            this.dgvSearch.Columns[2].Visible = false;
            this.dgvSearch.Columns[3].Visible = false;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        }
        public  void AutoCompleteLoad(string s, int t)
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
                DgvAutoRefNo.Rows[0].Cells[0].Selected = true;
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
                lbldemo.Text = "0";
                lblservice.Text = "0";
                lbldamage.Text = "0";
                lblprice.Text = "0";
                Locationpanal.Controls.Clear();
            }


        

        }    
        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrls => GetAll(ctrls, type)).Concat(controls).Where(c => c.GetType() == type);
        }
        private void Btnsubmit_Click(object sender, EventArgs e)
        {
       }
        public  void itemdetails(string s)
        {

            try
            {
                dtitems = new DataTable();
                string s1 = s.Trim();
                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s1.Replace("'", "''");             
                dtitems = objQuotationbal.itemdetails(name, s2);
                Program.dtitems = dtitems;
            }
            catch (Exception e)
            {

            }

        }
        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            ProdSelRowvalue = 0;

        }
        private void transactionclose_Click(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
            lblproductid.Text = string.Empty;          
            lblitemcode.Text = "0";
            lblrack.Text = "0";
            lbldisplay.Text = "0";
            lbldemo.Text = "0";
            lblservice.Text = "0";
            lbldamage.Text = "0";
            lblprice.Text = "0";         
            DgvAutoRefNo.DataSource = null;
            DgvAutoRefNo.Visible = false;
        }   
        private void dgvOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvOrders.CurrentCell.ColumnIndex;
            string headerText = dgvOrders.Columns[column].HeaderText;

            if (headerText.Equals("Quantity"))
            {
                tb = e.Control as TextBox;


                if (tb != null)
                {
                    ///tb.TextChanged += new EventHandler(textbox_Change);
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.MaxLength = 10;
                }
            }
            else if (headerText.Equals("Rate"))
            {
                tbrate = e.Control as TextBox;


                if (tbrate != null)
                {
                    tbrate.TextChanged += new EventHandler(textbox_Change);
                    tbrate.KeyPress += new KeyPressEventHandler(txtch);
                }


            }
        }
        private void textbox_Change(object sender, EventArgs e)
        {
            int column = dgvOrders.CurrentCell.ColumnIndex;
            string headerText = dgvOrders.Columns[column].HeaderText;
            if (headerText.Equals("Rate"))
            {
                if (!string.IsNullOrEmpty(tbrate.Text))
                {
                    if (tbrate.Text.Contains("-"))
                    {
                        tbrate.Text = tbrate.Text.Replace("-", "");
                    }
                }
            }
        }
        public  void txtch(object sender, KeyPressEventArgs e)
        {
            //if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            //    e.Handled = true;
            if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar))
                                && (e.KeyChar != '.'))
                e.Handled = true;


            //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            //{
            //    e.Handled = true;
            //}

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }


        }
        Regex reg = new Regex(@"^-?\d+[.]?\d*$");
        Regex reg1 = new Regex(@"^-?[.]?\d*$");
        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
         
            try
            {
                if (char.IsControl(e.KeyChar)) return;
                if ((reg.IsMatch(tb.Text.Insert(tb.SelectionStart, e.KeyChar.ToString()) + "1")) || reg1.IsMatch(tb.Text.Insert(tb.SelectionStart, e.KeyChar.ToString()) + "1"))
                {

                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {

            }
        }
        private void button_click(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;

            if (btn.Checked)
            {
                getquotaion(btn.Text);
                total();
                pnenabletrue();
            }

        }
        public  void getquotaion(string s)
        {
            this.dgvOrders.Columns["Amount"].ReadOnly = true;
            this.dgvOrders.Columns[0].ReadOnly = true;
            this.dgvOrders.Columns["Items"].ReadOnly = true;
            this.dgvOrders.Columns["UOM"].ReadOnly = true;
            this.dgvOrders.Columns["productid"].ReadOnly = true;
            this.dgvOrders.Columns["Rate"].ReadOnly = true;
            DataSet ds = objQuotationbal.getquotation(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbcustomernames.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                cmdcitys.Text = Convert.ToString(ds.Tables[0].Rows[0]["city"]);
                cmbreferences.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Referenceid"]);
                cmbassistbys.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Assist"]);
                lblperare.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);
                txtorders.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);
                cmbstatuss.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                dates.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);
                comboBox1.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["AssistName"]);
                string final = Convert.ToString(ds.Tables[0].Rows[0]["Final"]);
                if (final == "Open")
                {
                    pnenabletrue();


                    btnSavePending.Enabled = true;
                    btnSave.Enabled = true;
                    btnPrint.Enabled = false;
                }
                else
                {
                    pnenablefalse();

                    pnenablefalse();
                    btnSavePending.Enabled = false;
                    btnSave.Enabled = false;
                    btnPrint.Enabled = false;
                }

            }
            else
            {
                pnenablefalse();
                btnSavePending.Enabled = false;
                btnSave.Enabled = false;
                btnPrint.Enabled = false;
                //clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                string s1 = string.Empty;
                double qty;
                dgvOrders.Rows.Clear();
                this.dgvOrders.Columns["Rate"].ReadOnly = true;
                btnLesss.Enabled = true;
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvOrders.Rows.Add();
                    dgvOrders.Rows[i].Cells[0].Value = i + 1;
                    dgvOrders.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvOrders.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvOrders.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    s1 = Convert.ToString(ds.Tables[1].Rows[i]["Rate"]);
                    if (string.IsNullOrEmpty(s1))
                    {
                        qty = 0;
                    }
                    else
                    {
                        qty = Convert.ToDouble(s1);
                    }

                    dgvOrders.Rows[i].Cells[4].Value = qty;
                    dgvOrders.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvOrders.Rows[i].Cells[6].Value = amt;
                }
                //panel2.Enabled = false;
            }
            else
            {
                btnLesss.Enabled = false;
                dgvOrders.Rows.Clear();
                pnenablefalse();
                btnSavePending.Enabled = false;
                btnSave.Enabled = false;
                btnPrint.Enabled = false;
            }

        }
        public  void pnenabletrue()
        {

            cmbcustomernames.Enabled = true;
            cmdcitys.Enabled = true;
            cmbreferences.Enabled = true;
            cmbassistbys.Enabled = true;
            panel3.Enabled = true;
            dgvOrders.ReadOnly = false;
            comboBox1.Enabled = true;
        }
        public  void pnenablefalse()
        {

            cmbcustomernames.Enabled = false;
            cmdcitys.Enabled = false;
            cmbreferences.Enabled = false;
            cmbassistbys.Enabled = false;
            panel3.Enabled = false;
            dgvOrders.ReadOnly = true;
            comboBox1.Enabled = false;
        }
        public  void total()
        {
            try
            {
                double totalamount = 0.00, totalquantity = 0.00;
                double value = 0.0, value1 = 0.0;

                for (int i = 0; i < dgvOrders.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrders.Rows[i].Cells[6].Value)))
                    {
                        value = 0.0;
                    }
                    else
                    {
                        value = Convert.ToDouble(dgvOrders.Rows[i].Cells[6].Value);
                    }

                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrders.Rows[i].Cells[5].Value)))
                    {
                        value1 = 0.0;
                    }
                    else
                    {
                        value1 = Convert.ToDouble(dgvOrders.Rows[i].Cells[5].Value);
                    }

                    totalamount = totalamount + value;
                    totalquantity = totalquantity + value1;
                }

                // totalquantity = Math.Round(totalquantity);
                // totalamount = Math.Round(totalamount);


                string[] str = totalamount.ToString().Split('.');
                if (str.Length > 1)
                {
                    double num1 = Convert.ToDouble("0." + str[1]);

                    if (num1 >= 0.5)
                    {
                        totalamount = Math.Ceiling(totalamount);
                    }
                    else
                    {
                        totalamount = Math.Floor(totalamount);
                    }

                }


                lbltotalquantity.Text = Convert.ToString(totalquantity);
                lbltotalamount.Text = String.Format("{0:0,0.00}", totalamount);
            }
            catch
            {

            }
        }
        public  void bindorderno(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = objQuotationbal.ordernoval();
            cmb.DataSource = dt;
            cmb.DisplayMember = "Quotationid";
            cmb.ValueMember = "Quotationid";
        }
        public  void bindcustomer(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = objQuotationbal.Getcustomer();
            cmb.DataSource = dt;
            cmb.DisplayMember = "Name";
            cmb.ValueMember = "CustomerID";
        }
        public  void bindreference(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = objQuotationbal.Getreference();
            cmb.DataSource = dt;
            cmb.DisplayMember = "Name";
            cmb.ValueMember = "ReferencesID";
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
        public void search(string Customer, string OrderNumber, DateTime FromDate, DateTime ToDate, string Product, string Qty, string UserId)
        {

            DataTable dt = objQuotationbal.searchQuotation(Customer, OrderNumber, FromDate, ToDate, Product, Qty, Program.userid);


            dgvSearch.DataSource = null;


            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Order Number"]);
                
            }

            lblItemCounts.Text = Convert.ToString(dt.Rows.Count);

        }
        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string dd = label27.Text;
            if (dd != "1")
            {
                if (e.RowIndex >= 0)
                {
                    string s = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getquotaion(s);
                        total();


                    }
                    else
                    {
                        //clear();
                    }

                }
            }
        }
        private void dgvOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                try
                {
                    e.SuppressKeyPress = true;
                    if (dgvOrders.CurrentCell.ColumnIndex == 0)
                    {
                        dgvOrders.Focus();
                        dgvOrders.CurrentCell = dgvOrders[1, dgvOrders.CurrentCell.RowIndex];

                    }
                    else if (dgvOrders.CurrentCell.ColumnIndex == 1)
                    {
                        dgvOrders.Focus();
                        dgvOrders.CurrentCell = dgvOrders[2, dgvOrders.CurrentCell.RowIndex];

                    }
                    else if (dgvOrders.CurrentCell.ColumnIndex == 2)
                    {
                        dgvOrders.Focus();
                        dgvOrders.CurrentCell = dgvOrders[4, dgvOrders.CurrentCell.RowIndex];

                    }
                    else if (dgvOrders.CurrentCell.ColumnIndex == 4)
                    {
                        dgvOrders.Focus();
                        dgvOrders.CurrentCell = dgvOrders[5, dgvOrders.CurrentCell.RowIndex];

                    }


                    else if (dgvOrders.CurrentCell.ColumnIndex == 6)
                    {
                        dgvOrders.Focus();
                        dgvOrders.CurrentCell = dgvOrders[1, dgvOrders.CurrentCell.RowIndex + 1];

                    }
                }
                catch
                {

                }

            }
        }
        private void dgvOrder_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (edit == true)
                {
                    if (dgvOrders.CurrentCell.RowIndex >= 1)
                    {
                        dgvOrders.CurrentCell = dgvOrders[dgvOrders.CurrentCell.ColumnIndex, dgvOrders.CurrentCell.RowIndex - 1];
                        edit = false;
                    }
                    else if (dgvOrders.CurrentCell.RowIndex == 0)
                    {
                        dgvOrders.CurrentCell = dgvOrders[dgvOrders.CurrentCell.ColumnIndex, dgvOrders.CurrentCell.RowIndex - 1];
                    }

                }
            }
            catch
            {

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
        private void SalesQuotation1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        private void Txtitem_KeyUp(object sender, KeyEventArgs e)
        {
            
            string word;
            int typr = 0;
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
                    if (word.Trim() != "")
                        AutoCompleteLoad(word, typr);

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
                    

                }

              
            }
            catch (Exception efd)
            {

            }
        }
        private void DgvAutoRefNo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (Convert.ToInt32(lblproductid.Text) != 0)
                {
                    //dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
                    int rowindex = Convert.ToInt32(lblrowindex.Text);
                    dgvOrders.CurrentCell = dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[5];

                    //itemdetails(Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value));

                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                    getitems(sa);

                    dgvOrders.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    btnLesss.Enabled = true;
                    dgvOrders.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();

                    //dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    //dgvOrder.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                    dgvOrders.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                    double val = Convert.ToDouble(lblprice.Text);
                    dgvOrders.Rows[rowindex].Cells[7].Value = category.Text;
                    dgvOrders.Rows[rowindex].Cells[4].Value = val;
                    dgvOrders.Rows[rowindex].Cells[0].Value = rowindex + 1;
                    DgvAutoRefNo.Visible = false;

                    pnsearch.Visible = false;
                    lblproductid.Text = string.Empty;
                    //  Txtitem.Text = string.Empty;
                    lblitemcode.Text = "0";
                    lblrack.Text = "0";
                    lbldisplay.Text = "0";
                    lbldemo.Text = "0";
                    lblservice.Text = "0";
                    lbldamage.Text = "0";
                    lblprice.Text = "0";
                    dgvOrders.Focus();
                    dgvOrders.CurrentCell = dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[5];
                }
            }
            //else
            //{
            //    MessageBox.Show("Please Enter Correct Product Name");
            //}
        }
        public  void getsino()
        {
            for (int i = 0; i < dgvOrders.Rows.Count; i++)
            {
                dgvOrders.Rows[i].Cells[0].Value = i + 1;
            }
        }
        private void DgvAutoRefNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(Txtitem.Text))
                {

                    if (Convert.ToInt32(lblproductid.Text) != 0)
                    {

                        //dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
                        int rowindex = Convert.ToInt32(lblrowindex.Text);
                        dgvOrders.CurrentCell = dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[5];
                        dgvOrders.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        btnLesss.Enabled = true;
                        dgvOrders.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                        dgvOrders.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                        double val = Convert.ToDouble(lblprice.Text);
                        dgvOrders.Rows[rowindex].Cells[4].Value = val;
                        dgvOrders.Rows[rowindex].Cells[7].Value = category.Text;
                        dgvOrders.Rows[rowindex].Cells[0].Value = rowindex + 1;
                        DgvAutoRefNo.Visible = false;

                        pnsearch.Visible = false;
                        lblproductid.Text = string.Empty;
                        // Txtitem.Text = string.Empty;
                        lblitemcode.Text = "0";
                        lblrack.Text = "0";
                        lbldisplay.Text = "0";
                        lbldemo.Text = "0";
                        lblservice.Text = "0";
                        lbldamage.Text = "0";
                        lblprice.Text = "0";
                        Locationpanal.Controls.Clear();
                        dgvOrders.Focus();
                        dgvOrders.CurrentCell = dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[5];
                    }
                    else
                    {
                        MessageBox.Show("Please Enter Correct Product Name");
                    }
                }
                else
                {
                    this.ActiveControl = btnSave;
                    pnsearch.Visible = false;
                  
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
             else if(e.KeyData == Keys.End)
            {
                //if (DgvAutoRefNo.CurrentCell.RowIndex + 1 != DgvAutoRefNo.Rows.Count)
               // {
                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                    getitemdetails(sa);
                //}

            }



        }
        public  void getitemdetails(string sa)
        {

            Locationpanal.Controls.Clear();
            dtitems = Program.dtitems;
            var rows = from row in dtitems.AsEnumerable()
                       where row.Field<string>("ProductName").Trim() == sa.Trim()
                       select row;
            DataTable st = rows.CopyToDataTable();
            string productname = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);


            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.CommandText = "LocationstockinPanal";
                cmd.Parameters.AddWithValue("@Productname", sa);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                con.Close();
            }


            Label lbl;
            int y = 0, z = 0;
            Point p1 = new Point();
            p1.X = 0;
            p1.Y = 0;
            int lblcount = 0;

            if (dt.Rows.Count != 0)
            {
                string lbl_Caption = "";
                bool caption = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string yyy = dt.Rows[i][0].ToString();

                    if (yyy != "0")
                    {
                        caption = true;
                        if (lbl_Caption != "")
                        {
                            lbl_Caption = lbl_Caption + "     " + dt.Rows[i][1].ToString() + " = " + dt.Rows[i][0].ToString();
                        }
                        else
                        {
                            lbl_Caption = dt.Rows[i][1].ToString() + " = " + dt.Rows[i][0].ToString();
                        }
                    }



                }
                if (caption == true)
                {
                    lbl = new Label();
                    lbl.Width = 400;
                    lbl.Text = lbl_Caption;
                    Locationpanal.Controls.Add(lbl);
                    lblcount = 1;
                }


                if (lblcount == 0)
                {
                    lbl = new Label();
                    lbl.Text = "No stock";
                    Locationpanal.Controls.Add(lbl);
                }
            }
            else
            {
                lbl = new Label();
                lbl.Text = "No stock";
                Locationpanal.Controls.Add(lbl);
            }
            //string xx=getitems(items);


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
                category.Text = Convert.ToString(st.Rows[0]["Types"]);

                if (category.Text == "")
                {
                    category.Text = "0";
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
        public  void getitems(string sa)
        {

            Locationpanal.Controls.Clear();
            dtitems = Program.dtitems;
            var rows = from row in dtitems.AsEnumerable()
                       where row.Field<string>("ProductName").Trim() == sa.Trim()
                       select row;
            DataTable st = rows.CopyToDataTable();
            string productname = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);

          
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



                category.Text = Convert.ToString(st.Rows[0]["Types"]);

                if (category.Text == "")
                {
                    category.Text = "0";
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
        private void DgvAutoRefNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Up) || e.KeyChar != Convert.ToChar(Keys.Down))
            {
                Txtitem.Focus();
                SendKeys.Send(e.KeyChar.ToString());
                lblhiddenproduct.Text = Txtitem.Text;
                if (DgvAutoRefNo.Rows.Count > 0)
                {
                    DgvAutoRefNo.Rows[0].Cells[0].Selected = false;
                }
            }
        }
        private void ListSearchDate1_Click(object sender, EventArgs e)
        {
            
        }
        private void ListSearchDate2_Click(object sender, EventArgs e)
        {
           
        }
        private void ListSearchDate3_Click(object sender, EventArgs e)
        {
           
        }
        private void Calender()
        {
           
        }
        private void label40_Click(object sender, EventArgs e)
        {
           
        }
        private void lblToday_Click(object sender, EventArgs e)
        {
           
        }
        private void lblThisWeek_Click(object sender, EventArgs e)
        {
           
        }
        private void lblThisMonth_Click(object sender, EventArgs e)
        {
           
        }
        private void lblThisYear_Click(object sender, EventArgs e)
        {
           
        }
        private void lblYesterday_Click(object sender, EventArgs e)
        {
          
        }
        private void lblLastWeek_Click(object sender, EventArgs e)
        {
           
        }
        private void lblLastMonth_Click(object sender, EventArgs e)
        {

           
        }
        private void lblLastYear_Click(object sender, EventArgs e)
        {
           
        }
        private void SearchFrmDate_ValueChanged(object sender, EventArgs e)
        {
           
        }
        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            GetSearchOrder();
        }  
  
        private void GetSearchOrder()
        {
            string OrderNo = txtOrderNo.Text.Trim();
            string order = string.Empty;
               order = Convert.ToString(cmbstatus3.SelectedValue);
               if (order != "-Select-")
            {
                order = Convert.ToString(cmbstatus3.SelectedValue);
            }

            else
            {
                order = null;
            }
            DateTime FromDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
            DateTime ToDate = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);
          
            string Product = txtSearchProducts.Text.Trim();
            string Quty =string.Empty;

            Quty = textSearchQtys.Text.Trim();
            if (Quty!="")
            {
                Quty = textSearchQtys.Text.Trim();
            }
            else
            {
                Quty = null;
            }
          
            search(OrderNo, order, FromDate, ToDate, Product, Quty, Program.userid);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.dgvOrders.Columns["Rate"].ReadOnly = true;
            pnllesss.Visible = false;
            txtlesspwds.Clear();
        }     
        private void pcupdate_Click(object sender, EventArgs e)
        {
            Updated u = new Updated();
            u.ShowDialog();
        }
        private void dgvSearch_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyData == Keys.Down)
                {
                    if (dgvSearch.CurrentCell.RowIndex >= 0)
                    {
                        string s = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex + 1].Cells[0].Value);
                        if (!string.IsNullOrEmpty(s))
                        {


                            getquotaion(s);
                            total();

                        }
                        else
                        {
                            //clear();
                        }

                    }
                }

                if (e.KeyData == Keys.Up)
                {
                    if (dgvSearch.CurrentCell.RowIndex >= 0)
                    {
                        string s = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex - 1].Cells[0].Value);
                        if (!string.IsNullOrEmpty(s))
                        {
                            getquotaion(s);
                            total();
                        }
                        else
                        {
                          
                        }

                    }
                }
            }
            catch
            {

            }
        }
        private void btnmerge_Click(object sender, EventArgs e)
        {

        }
        public  void getquotaionmerge(string data)
        {
            this.dgvOrders.Columns["Amount"].ReadOnly = true;
            this.dgvOrders.Columns[0].ReadOnly = true;
            this.dgvOrders.Columns["Items"].ReadOnly = true;
            this.dgvOrders.Columns["UOM"].ReadOnly = true;
            this.dgvOrders.Columns["productid"].ReadOnly = true;
            this.dgvOrders.Columns["Rate"].ReadOnly = true;
            DataTable ds = objQuotationbal.getquotaionmerge(data);

            string s1 = string.Empty;
            double qty;
            dgvOrders.Rows.Clear();
            this.dgvOrders.Columns["Rate"].ReadOnly = true;
            btnLesss.Enabled = true;
            for (int i = 0; i < ds.Rows.Count; i++)
            {
                dgvOrders.Rows.Add();
                dgvOrders.Rows[i].Cells[0].Value = i + 1;
                dgvOrders.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["DisplayName"]);
                dgvOrders.Rows[i].Cells[2].Value = Convert.ToString(ds.Rows[i]["UOM"]);
                dgvOrders.Rows[i].Cells[3].Value = Convert.ToString(ds.Rows[i]["Productid"]);
                s1 = Convert.ToString(ds.Rows[i]["Rate"]);
                if (string.IsNullOrEmpty(s1))
                {
                    qty = 0;
                }
                else
                {
                    qty = Convert.ToDouble(s1);
                }

                dgvOrders.Rows[i].Cells[4].Value = qty;
                dgvOrders.Rows[i].Cells[5].Value = Convert.ToString(ds.Rows[i]["Quantity"]);
                double amt = Convert.ToDouble(ds.Rows[i]["Amount"]);
                dgvOrders.Rows[i].Cells[6].Value = amt;
            }
        }    
        private void dgvSearch_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == dgvSearch.Columns[3].Index)
            {
                dgvSearch.EndEdit();  //Stop editing of cell.


                var data = "";

                List<DataGridViewRow> selectedRows = (from row in dgvSearch.Rows.Cast<DataGridViewRow>()
                                                      where Convert.ToBoolean(row.Cells["checkBoxColumn"].Value) == true
                                                      select row).ToList();



                foreach (DataGridViewRow row in selectedRows)
                {

                    data += ",";
                    data += row.Cells[0].Value;

                }
                getquotaionmerge(data);


            }
        }
        private void btnmerge_Click_1(object sender, EventArgs e)
        {
            clear1();
            MergeGetSearchOrder();
            label27.Text = "1";
            this.dgvSearch.Columns[3].Visible = true;
            btnmerge.Visible = false;
            
        }
        private void MergeGetSearchOrder()
        {
            string OrderNo = txtOrderNo.Text.Trim();
            string order = string.Empty;
            order = Convert.ToString(cmbstatus3.SelectedValue);
            if (order != "")
            {
                order = Convert.ToString(cmbstatus3.SelectedValue);
            }

            else
            {
                order = null;
            }
            DateTime FromDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
            DateTime ToDate = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);

            string Product = txtSearchProducts.Text.Trim();
            string Quty = string.Empty;

            Quty = textSearchQtys.Text.Trim();
            if (Quty != "")
            {
                Quty = textSearchQtys.Text.Trim();
            }
            else
            {
                Quty = null;
            }

            Mergesearch(order, FromDate, ToDate, Product, Quty, Program.userid);
        }
        public void Mergesearch(string OrderNumber, DateTime FromDate, DateTime ToDate, string Product, string Qty, string UserId)
        {

            DataTable dt = objQuotationbal.searchMergeQuotation(OrderNumber, FromDate, ToDate, Product, Qty, Program.userid);


            dgvSearch.DataSource = null;


            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Order Number"]);

            }

            lblItemCounts.Text = Convert.ToString(dt.Rows.Count);

        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }   
        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearchs.Visible = false;
        }
        private void cmbcustomernames_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = "";
            if (cmbcustomernames.SelectedIndex > 0)
            {
                s = Convert.ToString(cmbcustomernames.SelectedValue);
            }

            cmdcitys.Text = objQuotationbal.bindcity(s);
        }      
        private void textSearchQtys_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && (e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
        private void btnLesss_Click(object sender, EventArgs e)
        {
            pnllesss.Visible = true;
            txtlesspwds.Focus();
        }
        private void txtlesspwds_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnLogins.PerformClick();
            }
        }
        private void btnLogins_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtlesspwds.Text))
            {
                MessageBox.Show("Password should not be empty");
                this.ActiveControl = txtlesspwds;
                return;
            }
            DataTable dt = LoginBAL.GetLesserDetials(txtlesspwds.Text, "PRICE");
            if (dt.Rows.Count > 0)
            {
                txtlesspwds.Text = string.Empty;
                pnllesss.Visible = false;
                this.dgvOrders.Columns["Rate"].ReadOnly = false;
                dgvOrders.Focus();
                dgvOrders.CurrentCell = dgvOrders[4, 0];
                dgvOrders.BeginEdit(true);
            }
            else
            {
                MessageBox.Show("Authentication Failed");
                this.dgvOrders.Columns["Rate"].ReadOnly = true;
                txtlesspwds.Focus();

            }
        }
        private void dgvOrders_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5)
                {
                    dgvOrders.Focus();
                    //edit = true;
                    dgvOrders.CurrentCell = dgvOrders[1, e.RowIndex + 1];
                }

                if (e.ColumnIndex == 4)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[4].Value)))
                    {
                        dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[4].Value = 0;
                    }
                }
            }
            catch
            {

            }
        }
        private void dgvOrders_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            total();
            if (e.ColumnIndex == 1)
            {
                rdbStartsWith.Checked = true;
                if (dgvOrders.ReadOnly == false)
                {
                    pnsearch.Visible = true;
                }
                //pnsearch.Visible = true;
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
        }
        private void dgvOrders_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {


                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[5].Value)))
                {
                    decimal rate = Convert.ToDecimal(dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[4].Value);
                    //decimal amt = rate * Convert.ToDecimal(tb.Text);

                    decimal amt = rate * Convert.ToDecimal(dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[5].Value);

                    if (amt > 0)
                    {
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
                    }
                    else
                    {
                        string[] str = amt.ToString().Split('.');
                        if (str.Length > 1)
                        {
                            double num1 = Convert.ToDouble("0." + str[1]);

                            if (num1 >= 0.5)
                            {
                                amt = Math.Floor(amt);
                            }
                            else
                            {
                                amt = Math.Ceiling(amt);
                            }

                        }
                    }
                    dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[6].Value = amt;
                }


            }
            catch
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[1].Value)))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[6].Value)))
                    {
                        dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[6].Value = 0.00;
                    }

                }

            }
            total();
        }
        private void dgvOrders_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (savevads == false)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[5].Value)))
                    {
                        decimal rate = Convert.ToDecimal(dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[4].Value);
                        //decimal amt = rate * Convert.ToDecimal(tb.Text);

                        decimal amt = rate * Convert.ToDecimal(dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[5].Value);

                        if (amt > 0)
                        {
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
                        }
                        else
                        {
                            string[] str = amt.ToString().Split('.');
                            if (str.Length > 1)
                            {
                                double num1 = Convert.ToDouble("0." + str[1]);

                                if (num1 >= 0.5)
                                {
                                    amt = Math.Floor(amt);
                                }
                                else
                                {
                                    amt = Math.Ceiling(amt);
                                }

                            }
                        }
                        dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[6].Value = amt;
                    }


                }
                catch (Exception sa)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[1].Value)))
                    {
                        dgvOrders.Rows[dgvOrders.CurrentCell.RowIndex].Cells[6].Value = 0.00;
                    }
                }
                total();

            }
            else
            {
                savevads = false;
            }
        }
        private void dgvOrders_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvOrders.CurrentCell.ColumnIndex;
            string headerText = dgvOrders.Columns[column].HeaderText;

            if (headerText.Equals("Quantity"))
            {
                tb = e.Control as TextBox;


                if (tb != null)
                {
                    ///tb.TextChanged += new EventHandler(textbox_Change);
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.MaxLength = 10;
                }
            }
            else if (headerText.Equals("Rate"))
            {
                tbrate = e.Control as TextBox;


                if (tbrate != null)
                {
                    tbrate.TextChanged += new EventHandler(textbox_Change);
                    tbrate.KeyPress += new KeyPressEventHandler(txtch);
                }


            }
        }
        private void dgvOrders_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (edit == true)
                {
                    if (dgvOrders.CurrentCell.RowIndex >= 1)
                    {
                        dgvOrders.CurrentCell = dgvOrders[dgvOrders.CurrentCell.ColumnIndex, dgvOrders.CurrentCell.RowIndex - 1];
                        edit = false;
                    }
                    else if (dgvOrders.CurrentCell.RowIndex == 0)
                    {
                        dgvOrders.CurrentCell = dgvOrders[dgvOrders.CurrentCell.ColumnIndex, dgvOrders.CurrentCell.RowIndex - 1];
                    }

                }
            }
            catch
            {

            }
        }
        private void dgvOrders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                try
                {
                    e.SuppressKeyPress = true;
                    if (dgvOrders.CurrentCell.ColumnIndex == 0)
                    {
                        dgvOrders.Focus();
                        dgvOrders.CurrentCell = dgvOrders[1, dgvOrders.CurrentCell.RowIndex];

                    }
                    else if (dgvOrders.CurrentCell.ColumnIndex == 1)
                    {
                        dgvOrders.Focus();
                        dgvOrders.CurrentCell = dgvOrders[2, dgvOrders.CurrentCell.RowIndex];

                    }
                    else if (dgvOrders.CurrentCell.ColumnIndex == 2)
                    {
                        dgvOrders.Focus();
                        dgvOrders.CurrentCell = dgvOrders[4, dgvOrders.CurrentCell.RowIndex];

                    }
                    else if (dgvOrders.CurrentCell.ColumnIndex == 4)
                    {
                        dgvOrders.Focus();
                        dgvOrders.CurrentCell = dgvOrders[5, dgvOrders.CurrentCell.RowIndex];

                    }


                    else if (dgvOrders.CurrentCell.ColumnIndex == 6)
                    {
                        dgvOrders.Focus();
                        dgvOrders.CurrentCell = dgvOrders[1, dgvOrders.CurrentCell.RowIndex + 1];

                    }
                }
                catch
                {

                }

            }
        }
        private void clear1()
        {


            btnLesss.Enabled = false;
            pnsearch.Visible = false;
            btnSavePending.Enabled = true;
            btnSave.Enabled = true;
            btnPrint.Enabled = true;
            cmbcustomernames.Text = "--Select--";
            cmdcitys.Text = string.Empty;
            cmbassistbys.SelectedIndex = 0;
            cmbreferences.SelectedIndex = 0;
            txtorders.Clear();
            dgvOrders.Rows.Clear();
            lblperare.Text = Program.Userfullname;
            lbltotalquantity.Text = "0";
            lbltotalamount.Text = "0";
            cmbcustomernames.Focus();
            cmbloaction.SelectedIndex = 0;
            cmbstatuss.Text = "Open";
            pnenabletrue();
            var cntls = GetAll(this, typeof(RadioButton));
            foreach (Control cntrl in cntls)
            {
                RadioButton _rb = (RadioButton)cntrl;
                if (_rb.Text != "New")
                {
                    if (_rb.Checked)
                    {
                        _rb.Checked = false;
                    }
                }
                else
                {
                    _rb.Checked = true;
                }
            }
            this.dgvOrders.Columns["Rate"].ReadOnly = true;
        }
        public DataTable bindEstimation()
        {
            DataTable dt = new DataTable();
            dt = objQuotationbal.itemestimationorderno();
            DataRow dr = dt.NewRow();
            dr["Quotationid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);

            return dt;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool val = Validation();
            if (val)
            {

                save(2);

            }
        }
        private bool Validation()
        {
            StockCheck = new DataTable();
            DataColumn ItemsLessStock = new DataColumn("ItemsLessStock", typeof(string));
            DataColumn Avalavbe = new DataColumn("Avalavbe", typeof(string));
            DataColumn Order = new DataColumn("Order", typeof(string));
            DataColumn Need = new DataColumn("Need", typeof(string));
            StockCheck.Columns.Add(ItemsLessStock);
            StockCheck.Columns.Add(Avalavbe);
            StockCheck.Columns.Add(Order);
            StockCheck.Columns.Add(Need);
            bool status = true;
            string message = "";
            int i = 0;

            if (cmbcustomernames.Text == "--Select--")
            {
                i++;
                message = message + "* Please Enter Customer Name" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbcustomernames;
            }


            //if (comboBox1.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please Select Assist By Type " + "\n";
            //    if (i == 1)
            //        this.ActiveControl = comboBox1;
            //}

            //if (string.IsNullOrEmpty(cmdcity.Text))
            //{
            //    i++;
            //    message = message + "* Please Select city" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmdcity;
            //}

          

            //if (cmbreference.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please select Reference" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmbreference;
            //}


            if (dgvOrders.Rows.Count > 0)
            {
                i++;
                string Items = Convert.ToString(dgvOrders.Rows[0].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvOrders.Rows[0].Cells["Items"].Value);
                if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                {
                    message = message + "* Please Select Product" + "\n";
                }

                if (i == 1)
                    this.ActiveControl = dgvOrders;
            }
            else if (dgvOrders.Rows.Count == 0)
            {
                i++;
                message = message + "* Please Select Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrders;
            }

            bool sas = false;
            bool LightProduct = false;
            bool NormalProduct = false;
            for (int k = 0; k < dgvOrders.RowCount; k++)
            {
                string Items = Convert.ToString(dgvOrders.Rows[k].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvOrders.Rows[k].Cells["Items"].Value);
                string rate = Convert.ToString(dgvOrders.Rows[k].Cells["Rate"].Value);
                string Types = Convert.ToString(dgvOrders.Rows[k].Cells["Types"].Value);


                if (Types == "Yes")
                {
                    LightProduct = true;


                }
                else if (Types == "No")
                {
                    NormalProduct = true;


                }



                if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))) || Items == "." || Items == "-" || Items == ".-" || Items == "-." || Items == "0" || rate == ".")
                {
                    sas = true;
                    break;
                }
                else
                {
                    if (!string.IsNullOrEmpty(Received))
                    {
                        DataTable StockList = new DataTable();
                        using (SqlConnection con = new SqlConnection(Conn))
                        {
                            con.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = con;
                            cmd.CommandText = "LocationstockinPanal";
                            cmd.Parameters.AddWithValue("@Productname", Received);
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(StockList);
                            con.Close();
                        }
                        double valstock = 0.00;

                        for (int s = 0; s < StockList.Rows.Count; s++)
                        {
                            valstock = valstock + (Convert.ToDouble(StockList.Rows[s][0].ToString()));
                        }

                        if (Convert.ToDouble(Items) > valstock)
                        {
                            double diff = Convert.ToDouble(Items) - Convert.ToDouble(valstock);
                            StockCheck.Rows.Add(Received, valstock, Items, diff);
                        }
                    }







                }

            }
            if (LightProduct == true)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    i++;
                    message = message + "* Please Select Assist By Light " + "\n";
                    if (i == 1)
                        this.ActiveControl = comboBox1;
                }
            }

            if (NormalProduct == true)
            {
                if (cmbassistbys.SelectedIndex == 0)
                {
                    i++;
                    message = message + "* Please Select Assist By " + "\n";
                    if (i == 1)
                        this.ActiveControl = cmbassistbys;
                }
            }
            if (sas == true)
            {
                i++;
                message = message + "* Product or Rate or Quantity should not be empty." + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrders;
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
        public void RemoveNullColumnFromDataTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][0])))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }
        public bool RemoveDuplicateRows(DataTable dTable, string colName)
        {
            bool sa = true;
            int index = 0;
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
            {
                index = dTable.Rows.IndexOf(dRow);
                dgvOrders.Rows[index].DefaultCellStyle.ForeColor = Color.Red;
                sa = false;
            }
            //dTable.Rows.Remove(dRow);


            //Datatable which contains unique records will be return as output.
            return sa;
        }
        public void GetReport(string QuotationId)
        {
            try
            {
                if (!string.IsNullOrEmpty(QuotationId))
                {
                    DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Quotationreport rpt = new Quotationreport(txtorder.Text);
                        //rpt.ShowDialog();

                        using (SqlConnection con = new SqlConnection(Program.connection))
                        {
                            DataSet ds = new DataSet();
                            con.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.AddWithValue("@id", QuotationId);
                            cmd.Parameters.AddWithValue("@companyname", Program.Company);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "GetQuotationreport_Print_RackOrder";
                            cmd.Connection = con;
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(ds);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                try
                                {

                                    RREPrint objRREPrint = new RREPrint();
                                    objRREPrint.dsMain = ds;
                                    objRREPrint.pagenumber = 1;
                                    objRREPrint.status = true;
                                    objRREPrint._strRefText = "Qtn:";
                                    objRREPrint._strRef = QuotationId;

                                    objRREPrint.RREPrintQuotation();
                                }

                                catch (Exception ex)
                                {

                                }
                            }


                        }

                        //System.Diagnostics.Process myProc = new System.Diagnostics.Process();
                        //myProc.StartInfo.FileName = "type " + Obj.fileName + " >prn";  //Attempting to start a non-existing executable
                        //myProc.Start();    //Start the application and assign it to the process component.    
                        //ExecuteCommandSync("type " + Obj.fileName + " >prn");



                    }
                }
            }

            catch (Exception ex)
            {

            }

        }
        private void clear()
        {
            foreach (DataGridViewRow dr in dgvSearch.Rows)
            {
                dr.Cells[3].Value = false;//sıfırın
            }
            btnmerge.Visible = true;
            label27.Text = "";
            btnmerge.Visible = true;
            comboBox1.SelectedIndex = 0;
            this.dgvSearch.Columns[3].Visible = false;
            btnLesss.Enabled = false;
            pnsearch.Visible = false;
            btnSavePending.Enabled = true;
            btnSave.Enabled = true;
            btnPrint.Enabled = true;
            cmbcustomernames.Text = "--Select--";
            cmdcitys.Text = string.Empty;
            cmbassistbys.SelectedIndex = 0;
            cmbreferences.SelectedIndex = 0;
            txtorders.Clear();
            dgvOrders.Rows.Clear();
            lblperare.Text = Program.Userfullname;
            lbltotalquantity.Text = "0";
            lbltotalamount.Text = "0";
            cmbcustomernames.Focus();
            cmbloaction.SelectedIndex = 0;
            cmbstatuss.Text = "Open";
            pnenabletrue();
            var cntls = GetAll(this, typeof(RadioButton));
            foreach (Control cntrl in cntls)
            {
                RadioButton _rb = (RadioButton)cntrl;
                if (_rb.Text != "New")
                {
                    if (_rb.Checked)
                    {
                        _rb.Checked = false;
                    }
                }
                else
                {
                    _rb.Checked = true;
                }
            }
            this.dgvOrders.Columns["Rate"].ReadOnly = true;
            btnSearch.PerformClick();
        }
        public void GetStockLessReport(DataTable QuotationId, string output)
        {
            try
            {
                if (QuotationId.Rows.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Do you Want Less Stock Details ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Quotationreport rpt = new Quotationreport(txtorder.Text);
                        //rpt.ShowDialog();




                        QuotationStockReport Obj = new QuotationStockReport();
                        Obj.dsMain = QuotationId;
                        Obj.IdQutoastion = output;
                        //Obj.ShowDialog();
                        if (Obj.GenerateQuoationid())
                        {
                            frmPrintPreview objfrmpreview = new frmPrintPreview();
                            objfrmpreview.fileName = Obj.fileName;
                            objfrmpreview.Show();

                        }






                    }
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        public void bindpending()
        {
            flowLayoutPanel1.Controls.Clear();
            DataTable dt = objQuotationbal.pending(userid);
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    RadioButton button = new RadioButton();
            //    button.Tag = i;
            //    button.Width = 200;
            //    button.FlatStyle = FlatStyle.Popup;
            //    button.Appearance = Appearance.Button;
            //    button.Cursor = Cursors.Hand;
            //    button.CheckedChanged += new EventHandler(button_click);
            //    button.Text = Convert.ToString(dt.Rows[i]["Quotationid"]);
            //    flowLayoutPanel1.Controls.Add(button);
            //}


            RadioButton btn = new RadioButton();
            btn.Tag = 0;
            btn.Width = 50;
            btn.FlatStyle = FlatStyle.Popup;
            btn.Appearance = Appearance.Button;
            btn.Cursor = Cursors.Hand;
            btn.Checked = true;
            btn.CheckedChanged += new EventHandler(button_click);
            btn.Text = "New";
            flowLayoutPanel1.Controls.Add(btn);


        }
        public void save(int v)
        {
            panel1.Enabled = false;
            pnenablefalse();

            Pnloading.Visible = true;


            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(txtorders.Text))
            {
                objQuotationbal.isnew = 0;
            }
            else
            {
                objQuotationbal.isnew = 1;
            }
            objQuotationbal.Quotationid = txtorders.Text;
            objQuotationbal.Customerid = Convert.ToString(cmbcustomernames.SelectedValue);
            objQuotationbal.date = dates.Value;
            objQuotationbal.Referenceid = Convert.ToString(cmbreferences.SelectedValue);
            objQuotationbal.Assist = Convert.ToString(cmbassistbys.SelectedValue);
            objQuotationbal.Customername = Convert.ToString(cmbcustomernames.Text.Trim());
            objQuotationbal.City = Convert.ToString(cmdcitys.Text);
            objQuotationbal.Assistnames = Convert.ToString(comboBox1.SelectedValue);

            if (v == 1)
            {
                objQuotationbal.status = "Open";
            }
            else if (v == 2)
            {
                objQuotationbal.status = "Quote Completed";
            }
            objQuotationbal.Updatedby = Program.userid;
            dt = DataGridView2DataTable(dgvOrders);
            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            RemoveNullColumnFromDataTable(dt);
            dt.Columns.RemoveAt(4);
            bool dtval = RemoveDuplicateRows(dt, "ProductId");


            if (dtval)
            {

                string output = objQuotationbal.SaveQuotation(objQuotationbal, dt);
                if (!string.IsNullOrEmpty(output) && string.IsNullOrEmpty(txtorders.Text))
                {
                    panel1.Enabled = true;
                    pnenabletrue();
                    Pnloading.Visible = false;

                    //MessageBox.Show("save successfully");
                    txtorders.Text = output;
                    GetStockLessReport(StockCheck, output);
                    if (v == 2)
                    {
                        //GetReport(output);

                        //DialogResult Res = MessageBox.Show("Do you Want Stock Less Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        //if (Res == DialogResult.Yes)
                        //{



                        //}

                        //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        //if (result == DialogResult.Yes)
                        //{
                        //    Quotationreport rpt = new Quotationreport(output);
                        //    rpt.ShowDialog();

                        GetReport(output);
                        //}
                    }

                }
                else if (!string.IsNullOrEmpty(output) && !string.IsNullOrEmpty(txtorders.Text))
                {
                    //MessageBox.Show("Update successfully");
                    panel1.Enabled = true;
                    pnenabletrue();
                    Pnloading.Visible = false;

                    txtorders.Text = output;
                    //DialogResult Res = MessageBox.Show("Do you Want Stock Less Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //if (Res == DialogResult.Yes)
                    //{


                    GetStockLessReport(StockCheck, output);
                    //}
                    if (v == 2)
                    {
                        //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                        //if (result == DialogResult.Yes)
                        //{
                        //    Quotationreport rpt = new Quotationreport(output);
                        //    rpt.ShowDialog();
                        //}






                        GetReport(output);
                    }
                    else if (v == 1)
                    {
                        GetReport(output);
                    }
                }
                bindpending();
                btnSearch.PerformClick();
                // search("Quotationid", "", "q.Updatedon", "Today", "customername", "", role1, Program.userid);
                clear();
            }
            else
            {
                MessageBox.Show("Please Remove Duplication Product");
                panel1.Enabled = true;
                btnSavePending.Enabled = true;
                btnSave.Enabled = true;
                btnPrint.Enabled = true;
                Pnloading.Visible = false;

                cmbcustomernames.Enabled = true;
                cmdcitys.Enabled = true;
                cmbreferences.Enabled = true;
                cmbassistbys.Enabled = true;
                dgvOrders.ReadOnly = false;
                this.dgvOrders.Columns["Rate"].ReadOnly = true;
                this.dgvOrders.Columns[0].ReadOnly = true;
                this.dgvOrders.Columns["Items"].ReadOnly = true;
                this.dgvOrders.Columns["UOM"].ReadOnly = true;
                this.dgvOrders.Columns["Amount"].ReadOnly = true;
            }
        }

        private void dgvSearch_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            string dd = label27.Text;
            if (dd != "1")
            {
                if (e.RowIndex >= 0)
                {
                    string s = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getquotaion(s);
                        total();


                    }
                    else
                    {
                        clear();
                    }

                }
            }
        }

        private void dgvSearch_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == dgvSearch.Columns[3].Index)
            {
                dgvSearch.EndEdit();  //Stop editing of cell.


                var data = "";

                List<DataGridViewRow> selectedRows = (from row in dgvSearch.Rows.Cast<DataGridViewRow>()
                                                      where Convert.ToBoolean(row.Cells["checkBoxColumn"].Value) == true
                                                      select row).ToList();



                foreach (DataGridViewRow row in selectedRows)
                {

                    data += ",";
                    data += row.Cells[0].Value;

                }
                getquotaionmerge(data);


            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            clear();
            this.dgvOrders.Columns["Rate"].ReadOnly = true;
            this.dgvOrders.Columns[0].ReadOnly = true;
            this.dgvOrders.Columns["Items"].ReadOnly = true;
            this.dgvOrders.Columns["UOM"].ReadOnly = true;
            this.dgvOrders.Columns["Amount"].ReadOnly = true;
        }

        private void btnSavePending_Click(object sender, EventArgs e)
        {
            bool val = Validation();
            if (val)
            {
                save(1);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void cmbassistbys_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pntab_Paint(object sender, PaintEventArgs e)
        {

        }
    }
      
}
