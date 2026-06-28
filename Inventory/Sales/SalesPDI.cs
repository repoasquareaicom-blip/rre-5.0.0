
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
using IssuedReceived;
//using QuotationReport;

namespace Inventory
{
    public partial class SalesPDI : Form
    {
        QuotationBal objQuotationbal = new QuotationBal();
        TextBox tb, tbrate;
        bool load = false;
        public bool edit = false;
        int userid = 0;
        string cas = string.Empty;
        string role1 = string.Empty;
        string srole = string.Empty;
        int ProdSelRowvalue = 0;
        bool res = false;
        ComboBox cmbActionstatus;
        DataTable dtitems;
        string clickstatus = string.Empty;
        bool savevads = false;
        string firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue;
        public SalesPDI()
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
            userid = Convert.ToInt32(Program.userid);
            this.WindowState = FormWindowState.Maximized;
            LoadPorts();
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            bindLocation();
            cmbloaction.SelectedIndex = 0;
            //comboBox1.SelectedIndex = 0;
            cmbstatus.SelectedIndex = 0;
            SearchPurchaseOrder();
            lblperare.Text = Program.Userfullname;
            bindAssist();
            bindreference();
            bindcustomer();
            bindAssistName();
            DataTable dtitems = Program.dtitems;
            if (dtitems == null)
            {
                itemdetails("");
            }
            //Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //Txtitem.AutoCompleteCustomSource = AutoCompleteLoad();
            //Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;

            search("Quotationid", "", "q.Updatedon", "Today", "customername", "", role1, Program.userid);
            //search("Quotationid", "", "customername", "", "r.Name", "", userid);
            Globeimage();
            DataTable dt = bindEstimation();
            cmbstatus1.DataSource = dt;
            cmbstatus1.DisplayMember = "Quotationid";
            cmbstatus1.ValueMember = "Quotationid";

            cmbstatus2.DataSource = dt;
            cmbstatus2.DisplayMember = "Quotationid";
            cmbstatus2.ValueMember = "Quotationid";


            cmbstatus3.DataSource = dt;
            cmbstatus3.DisplayMember = "Quotationid";
            cmbstatus3.ValueMember = "Quotationid";



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
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Yellow, 0, 0, 100, 100);
        }
        public void Globeimage()
        {
            //string pathname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            ////string pathname = Path.Combine(Environment.CurrentDirectory);
            ////string a = pathname.Replace("\\bin\\Debug", "");
            ////string path = a + "\\Resources\\Light Globe.gif";
            //string path = pathname + "\\Loading.gif";
            //using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            //{
            //    var ms = new System.IO.MemoryStream();
            //    fs.CopyTo(ms);
            //    ms.Position = 0;                               // <=== here
            //    if (pcloading.Image != null) pcloading.Image.Dispose();
            //    pcloading.Image = Image.FromStream(ms);
            //    pcloading.SizeMode = PictureBoxSizeMode.Zoom;
            //}
        }

        public void bindcustomer()
        {
            cmbcustomername.DataSource = objQuotationbal.Getcustomer();
            cmbcustomername.DisplayMember = "Name";
            cmbcustomername.ValueMember = "CustomerID";
        }

        public void bindLocation()
        {
            cmbloaction.DataSource = objQuotationbal.getLocation();
            cmbloaction.DisplayMember = "LocationName";
            cmbloaction.ValueMember = "LocationID";
        }

        public void bindreference()
        {
            cmbreference.DataSource = objQuotationbal.Getreference();
            cmbreference.DisplayMember = "Name";
            cmbreference.ValueMember = "ReferencesID";
        }

        public void bindAssist()
        {
            cmbassistby.DataSource = objQuotationbal.GetProductsusername();
            cmbassistby.DisplayMember = "Name";
            cmbassistby.ValueMember = "employeeid";
        }
        public void bindAssistName()
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
            dgvOrder.Rows.Clear();
            dgvOrder.ColumnCount = 9;
            //dgvOrder.RowCount = 16;

            dgvOrder.Columns[0].Name = "S.NO";
            dgvOrder.Columns[1].Name = "Items";
            dgvOrder.Columns[2].Name = "UOM";
            dgvOrder.Columns[5].Name = "Quantity";
            dgvOrder.Columns[3].Name = "productid";
            dgvOrder.Columns[4].Name = "Rate";
            dgvOrder.Columns[6].Name = "Amount";

            dgvOrder.Columns[7].Name = "Pending Quantity";
            dgvOrder.Columns[8].Name = "Product Serial.No";
            dgvOrder.Columns[3].Visible = false;

            this.dgvOrder.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvOrder.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns["S.NO"].ReadOnly = true;
            this.dgvOrder.Columns["Items"].ReadOnly = true;
            this.dgvOrder.Columns["UOM"].ReadOnly = true;
            this.dgvOrder.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrder.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrder.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvOrder.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
            dgvOrder.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvOrder.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvOrder.Columns["Rate"].ReadOnly = true;

            this.dgvOrder.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;




            this.dgvOrder.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvOrder.Columns["Amount"].ReadOnly = true;

            this.dgvOrder.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvOrder.Columns[0].Width = 12;
                this.dgvOrder.Columns[1].Width = 100;
                this.dgvOrder.Columns[2].Width = 10;
                this.dgvOrder.Columns[4].Width = 15;
                this.dgvOrder.Columns[5].Width = 20;
                this.dgvOrder.Columns[6].Width = 100;
                this.dgvOrder.Columns[7].Width = 100;
                this.dgvOrder.Columns[8].Width = 100;

            }
            else
            {
                this.dgvOrder.Columns[0].Width = 30;
                this.dgvOrder.Columns[1].Width = 200;
                this.dgvOrder.Columns[2].Width = 30;
                this.dgvOrder.Columns[4].Width = 60;
                this.dgvOrder.Columns[5].Width = 60;
                this.dgvOrder.Columns[6].Width = 100;
                this.dgvOrder.Columns[7].Width = 100;
                this.dgvOrder.Columns[8].Width = 100;

            }

            this.dgvOrder.Columns["Pending Quantity"].ReadOnly = true;

            DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
            cmb.HeaderText = "Status";
            cmb.Name = "cmbAction";
            cmb.MaxDropDownItems = 4;
            cmb.Items.Add("-Select-");
            cmb.Items.Add("Verified");
            cmb.Items.Add("Pending");
            cmb.DisplayIndex = 7;
            cmb.FlatStyle = FlatStyle.Popup;
            dgvOrder.Columns.Add(cmb);




            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);



            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvOrder.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvOrder.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


            Gridviewreadonly();
        }


        public void Gridviewreadonly()
        {

            for (int i = 0; i < dgvOrder.Columns.Count; i++)
            {
                if (dgvOrder.Columns[i].HeaderText == "S.NO" || dgvOrder.Columns[i].HeaderText == "Items" || dgvOrder.Columns[i].HeaderText == "UOM" || dgvOrder.Columns[i].HeaderText == "productid" || dgvOrder.Columns[i].HeaderText == "Rate" || dgvOrder.Columns[i].HeaderText == "Amount" || dgvOrder.Columns[i].HeaderText == "Pending Quantity")
                {
                    this.dgvOrder.Columns[i].ReadOnly = true;
                }

            }
        }

        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("Customer");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderNo.DataSource = bs.DataSource;
            cbxSearchOrderNo.SelectedIndex = 0;
            cmbstatus1.Visible = false;

        }

        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("Customer");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderDate.DataSource = bs.DataSource;
            cbxSearchOrderDate.SelectedIndex = 1;
            cmbstatus2.Visible = false;
        }

        private void SearchCreteria3()
        {
            List<string> search = new List<string>();
            search.Add("Customer");
            search.Add("Order Date");
            search.Add("Order Number");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxVendor.DataSource = bs.DataSource;
            cbxVendor.SelectedIndex = 2;
            txtsearch3.Visible = false;
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

        private void SalesPDI_Load(object sender, EventArgs e)
        {
            this.ActiveControl = cmbcustomername;
            clear();

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (dgvOrder.Focused)
            {
                try
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

                            return true;
                        }
                        getsino();

                    }

                    if (keyData == (Keys.Alt | Keys.Delete))
                    {
                        DialogResult result = MessageBox.Show("Do you want to Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            if (dgvOrder.Rows.Count > 0)
                            {
                                try
                                {
                                    int rowindex = dgvOrder.CurrentRow.Index;
                                    int colindex = dgvOrder.CurrentCell.ColumnIndex;
                                    dgvOrder.Rows.RemoveAt(rowindex);
                                }
                                catch
                                {
                                    if (dgvOrder.Rows.Count - 1 == dgvOrder.CurrentCell.RowIndex)
                                    {
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[0].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = "";

                                    }
                                }

                            }
                            pnsearch.Visible = false;
                            getsino();
                            return true;
                        }

                        if (dgvOrder.Rows.Count == 0)
                        {
                            dgvOrder.Rows.Add();
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
          
            if (cbxSearchOrderNo.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    cmbstatus1.Focus();
                    return true;
                }
            }
            if (cmbstatus1.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    cbxSearchOrderDate.Focus();
                    return true;
                }
            }
            if (cbxSearchOrderDate.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    cmbstatus2.Focus();
                    return true;
                }
            }
            if (cmbstatus2.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    cbxVendor.Focus();
                    return true;
                }
            }
            if (cbxVendor.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    cmbstatus3.Focus();
                    return true;
                }
            }
            if (cmbstatus3.Focused)
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
                    cmbassistby.Focus();
                    return true;
                }
            }


            if (cmbassistby.Focused)
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

                    dgvOrder.Focus();
                    if (dgvOrder.Rows.Count == 0)
                    {
                        dgvOrder.Rows.Add();
                    }
                    dgvOrder.CurrentCell = dgvOrder[1, 0];

                    return true;
                }
            }
            if (keyData == Keys.Escape)
            {
                if (pnsearch.Visible)
                {
                    pnsearch.Visible = false;
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
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
            if (cmbreference.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = cmbassistby;
                    return true;
                }
            }
            //if (txtorder.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = date;
            //        return true;
            //    }

            //}

            if (date.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = dgvOrder;
                    if (dgvOrder.Rows.Count == 0)
                    {
                        dgvOrder.Rows.Add();
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
                    if (dgvOrder.CurrentCell.ColumnIndex == 8)
                    {

                        if (Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value) == "Pending")
                        {
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder[7, dgvOrder.CurrentCell.RowIndex];
                            return true;
                        }
                        else
                        {
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder[8, dgvOrder.CurrentCell.RowIndex + 1];
                            return true;
                        }


                    }
                }
            }
            catch
            {

            }

            //if (btnPrint.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = cmbcustomername;
            //        return true;
            //    }
            //}
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SearchPurchaseOrder()
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
            }


            //string[] arr = new string[st.Rows.Count];
            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    arr[i] = st.Rows[i]["DisplayName"].ToString();
            //}
            //for (int i = 0; i < arr.Length; i++)
            //{
            //    //var combined = string.Join(", ", arr);
            //    var combined = arr[i];
            //    str.Add(combined);
            //}

            //Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //Txtitem.AutoCompleteCustomSource = str;
            //Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;

            //for (int i = 0; i < arr.Length; i++)
            //{
            //  var combined = string.Join(", ", arr);
            //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //str.Add(combined);
            //}

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}


        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool val = Validation();
            if (val)
            {
                save(2);

            }
        }

        private void btnSavePending_Click(object sender, EventArgs e)
        {

          

            bool val = Validationpending();
            if (val)
            {
                pendingsave();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }

        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (cmbcustomername.Text == "--Select--")
            {
                i++;
                message = message + "* Please Enter Customer Name" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbcustomername;
            }




            //if (string.IsNullOrEmpty(cmdcity.Text))
            //{
            //    i++;
            //    message = message + "* Please Select city" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmdcity;
            //}

            if (cmbassistby.SelectedIndex == 0)
            {
                i++;
                message = message + "* Please Select Assist By " + "\n";
                if (i == 1)
                    this.ActiveControl = cmbassistby;
            }

            //if (comboBox1.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please Select Assist by Name " + "\n";
            //    if (i == 1)
            //        this.ActiveControl = comboBox1;
            //}



            //if (cmbreference.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please select Reference" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmbreference;
            //}


            if (dgvOrder.Rows.Count > 0)
            {
                i++;
                string Items = Convert.ToString(dgvOrder.Rows[0].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvOrder.Rows[0].Cells["Items"].Value);
                if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                {
                    message = message + "* Please Select Product" + "\n";
                }

                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }
            else if (dgvOrder.Rows.Count == 0)
            {
                i++;
                message = message + "* Please Select Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }

            bool sas = false;

            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                string Items = Convert.ToString(dgvOrder.Rows[k].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvOrder.Rows[k].Cells["Items"].Value);
                string rate = Convert.ToString(dgvOrder.Rows[k].Cells["Rate"].Value);


                if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))) || Items == "." || Items == "-" || Items == ".-" || Items == "-." || Items == "0" || rate == ".")
                {
                    sas = true;
                    break;
                }
            }
            if (sas == true)
            {
                i++;
                message = message + "* Product or Rate or Quantity should not be empty." + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }


            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                if (Convert.ToString(dgvOrder.Rows[k].Cells["Rate"].Value) == "")
                {
                }
            }


            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[k].Cells[1].Value)))
                {
                    DataGridViewComboBoxCell thisCbCell = (DataGridViewComboBoxCell)dgvOrder.Rows[k].Cells["cmbAction"];
                    //DataGridViewComboBoxCell thisCbCell = (DataGridViewComboBoxCell)dgvOrder.Rows[k].Cells["cmbAction"];
                    if (string.IsNullOrEmpty(Convert.ToString(thisCbCell.Value)))
                    {
                        i++;
                        message = message + "* Please Select Status." + "\n";
                        if (i == 1)
                            this.ActiveControl = dgvOrder;
                        dgvOrder.CurrentCell = dgvOrder["cmbAction", dgvOrder.CurrentCell.RowIndex];
                        break;
                    }


                    else if (Convert.ToString(thisCbCell.Value) == "-Select-")
                    {
                        i++;
                        message = message + "* Please Select Status." + "\n";
                        if (i == 1)
                            this.ActiveControl = dgvOrder;
                        dgvOrder.CurrentCell = dgvOrder["cmbAction", dgvOrder.CurrentCell.RowIndex];
                        break;
                    }


                    else if (Convert.ToString(thisCbCell.Value) == "Pending")
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[k].Cells["Pending Quantity"].Value)) || Convert.ToString(dgvOrder.Rows[k].Cells["Pending Quantity"].Value) == "." || Convert.ToInt32(dgvOrder.Rows[k].Cells["Pending Quantity"].Value) == 0)
                        {
                            i++;
                            message = message + "* Please Enter Pending Quantity." + "\n";
                            if (i == 1)
                                this.ActiveControl = dgvOrder;
                            dgvOrder.CurrentCell = dgvOrder["Pending Quantity", dgvOrder.CurrentCell.RowIndex];
                            break;
                        }

                        else if (Convert.ToDouble(dgvOrder.Rows[k].Cells["Quantity"].Value) < Convert.ToDouble(dgvOrder.Rows[k].Cells["Pending Quantity"].Value))
                        {
                            i++;
                            message = message + "* Pending Quantity Should Not Be Greater Than Quantity." + "\n";
                            if (i == 1)
                                this.ActiveControl = dgvOrder;
                            dgvOrder.CurrentCell = dgvOrder["Pending Quantity", dgvOrder.CurrentCell.RowIndex];
                            status = false;
                            break;
                        }
                        else if (Convert.ToDouble(dgvOrder.Rows[k].Cells["Pending Quantity"].Value) < 0)
                        {
                            message = message + "* Pending Quantity Should Not Be Negitive." + "\n";
                            status = false;
                            break;
                        }
                        else if (Convert.ToDouble(dgvOrder.Rows[k].Cells["Quantity"].Value) < 0)
                        {
                            message = message + "* Quantity is Negitive should not be pending." + "\n";
                            status = false;
                            break;
                        }

                    }
                }
            }


            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }


        private bool Validationpending()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (cmbcustomername.Text == "--Select--")
            {
                i++;
                message = message + "* Please Enter Customer Name" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbcustomername;
            }




            //if (string.IsNullOrEmpty(cmdcity.Text))
            //{
            //    i++;
            //    message = message + "* Please Select city" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmdcity;
            //}

            if (cmbassistby.SelectedIndex == 0)
            {
                i++;
                message = message + "* Please Select Assist By " + "\n";
                if (i == 1)
                    this.ActiveControl = cmbassistby;
            }



            //if (cmbreference.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please select Reference" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmbreference;
            //}


            if (dgvOrder.Rows.Count > 0)
            {
                i++;
                string Items = Convert.ToString(dgvOrder.Rows[0].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvOrder.Rows[0].Cells["Items"].Value);
                if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                {
                    message = message + "* Please Select Product" + "\n";
                }

                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }
            else if (dgvOrder.Rows.Count == 0)
            {
                i++;
                message = message + "* Please Select Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }

            bool sas = false;

            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                string Items = Convert.ToString(dgvOrder.Rows[k].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvOrder.Rows[k].Cells["Items"].Value);
                string rate = Convert.ToString(dgvOrder.Rows[k].Cells["Rate"].Value);


                if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))) || Items == "." || Items == "-" || Items == ".-" || Items == "-." || Items == "0" || rate == ".")
                {
                    sas = true;
                    break;
                }
            }
            if (sas == true)
            {
                i++;
                message = message + "* Product or Rate or Quantity should not be empty." + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }


            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                if (Convert.ToString(dgvOrder.Rows[k].Cells["Rate"].Value) == "")
                {
                }
            }


            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[k].Cells[1].Value)))
                {
                    DataGridViewComboBoxCell thisCbCell = (DataGridViewComboBoxCell)dgvOrder.Rows[k].Cells["cmbAction"];
                    //DataGridViewComboBoxCell thisCbCell = (DataGridViewComboBoxCell)dgvOrder.Rows[k].Cells["cmbAction"];
                    //if (string.IsNullOrEmpty(Convert.ToString(thisCbCell.Value)))
                    //{
                    //    i++;
                    //    message = message + "* Please Select Status." + "\n";
                    //    if (i == 1)
                    //        this.ActiveControl = dgvOrder;
                    //    dgvOrder.CurrentCell = dgvOrder["cmbAction", dgvOrder.CurrentCell.RowIndex];
                    //    break;
                    //}


                    //else if (Convert.ToString(thisCbCell.Value) == "-Select-")
                    //{
                    //    i++;
                    //    message = message + "* Please Select Status." + "\n";
                    //    if (i == 1)
                    //        this.ActiveControl = dgvOrder;
                    //    dgvOrder.CurrentCell = dgvOrder["cmbAction", dgvOrder.CurrentCell.RowIndex];
                    //    break;
                    //}


                     if (Convert.ToString(thisCbCell.Value) == "Pending")
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[k].Cells["Pending Quantity"].Value)) || Convert.ToString(dgvOrder.Rows[k].Cells["Pending Quantity"].Value) == "." || Convert.ToInt32(dgvOrder.Rows[k].Cells["Pending Quantity"].Value) == 0)
                        {
                            i++;
                            message = message + "* Please Enter Pending Quantity." + "\n";
                            if (i == 1)
                                this.ActiveControl = dgvOrder;
                            dgvOrder.CurrentCell = dgvOrder["Pending Quantity", dgvOrder.CurrentCell.RowIndex];
                            break;
                        }

                        else if (Convert.ToDouble(dgvOrder.Rows[k].Cells["Quantity"].Value) < Convert.ToDouble(dgvOrder.Rows[k].Cells["Pending Quantity"].Value))
                        {
                            i++;
                            message = message + "* Pending Quantity Shold Not Be Greater Than Quantity." + "\n";
                            if (i == 1)
                                this.ActiveControl = dgvOrder;
                            dgvOrder.CurrentCell = dgvOrder["Pending Quantity", dgvOrder.CurrentCell.RowIndex];
                            break;
                        }

                    }
                }
            }


            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

        private void txtCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;

        }

        private void txtContactNo_KeyPress(object sender, KeyPressEventArgs e)
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

        private void clear()
        {
            dgvOrder.ReadOnly = true;
            pnsearch.Visible = false;
            btnSavePending.Enabled = false;
            btnSave.Enabled = false;
            btnPrint.Enabled = true;
            cmbcustomername.Text = "--Select--";
            cmdcity.Text = string.Empty;
            cmbassistby.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            cmbreference.SelectedIndex = 0;
            txtorder.Clear();
            dgvOrder.Rows.Clear();
            lblperare.Text = Program.Userfullname;
            lbltotalquantity.Text = "0";
            lbltotalamount.Text = "0";
            cmbcustomername.Focus();
            cmbloaction.SelectedIndex = 0;
            cmbstatus.Text = "Open";
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
            this.dgvOrder.Columns["Rate"].ReadOnly = true;
        }


        private void btnNew_Click(object sender, EventArgs e)
        {
            clear();
            this.dgvOrder.Columns["Rate"].ReadOnly = true;
            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns["Items"].ReadOnly = true;
            this.dgvOrder.Columns["UOM"].ReadOnly = true;
            this.dgvOrder.Columns["Amount"].ReadOnly = true;

        }

        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrls => GetAll(ctrls, type)).Concat(controls).Where(c => c.GetType() == type);
        }

        private void btnPrint_Click(object sender, EventArgs e)
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


        public void save(int v)
        {
            panel1.Enabled = false;
            //pnenablefalse();

            Pnloading.Visible = true;


            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(txtorder.Text))
            {
                objQuotationbal.isnew = 0;
            }
            else
            {
                objQuotationbal.isnew = 1;
            }
            objQuotationbal.Quotationid = txtorder.Text;
            objQuotationbal.Customerid = Convert.ToString(cmbcustomername.SelectedValue);
            objQuotationbal.date = date.Value;
            objQuotationbal.Referenceid = Convert.ToString(cmbreference.SelectedValue);
            objQuotationbal.Assist = Convert.ToString(cmbassistby.SelectedValue);
            objQuotationbal.Assistnames = Convert.ToString(comboBox1.SelectedValue);
            objQuotationbal.Customername = Convert.ToString(cmbcustomername.Text.Trim());
            objQuotationbal.City = Convert.ToString(cmdcity.Text);



            if (v == 1)
            {
                objQuotationbal.status = "Open";
            }
            else if (v == 2)
            {
                objQuotationbal.status = "Quote Completed";
            }
            objQuotationbal.Updatedby = Program.userid;
            dt = DataGridView2DataTable(dgvOrder);
            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }

            //dt.Columns.RemoveAt(4);

            dt.Columns[4].ColumnName = "PQuantity";

            dt.Columns["cmbAction"].ColumnName = "Status";



            RemoveNullColumnFromDataTable(dt);

            bool dtval = RemoveDuplicateRows(dt, "ProductId");


            if (dtval)
            {

                string output = objQuotationbal.SaveQuotationPdi(objQuotationbal, dt);
                if (!string.IsNullOrEmpty(output) && string.IsNullOrEmpty(txtorder.Text))
                {
                    panel1.Enabled = true;
                    pnenabletrue();
                    Pnloading.Visible = false;

                    //MessageBox.Show("save successfully");
                    txtorder.Text = output;
                    if (v == 2)
                    {
                        savereceived();
                        // GetReport(output);


                        //    Quotationreport rpt = new Quotationreport(output);
                        //    rpt.ShowDialog();

                        //    //GetReport(output);
                        //}
                    }

                }
                else if (!string.IsNullOrEmpty(output) && !string.IsNullOrEmpty(txtorder.Text))
                {
                    //MessageBox.Show("Update successfully");
                    panel1.Enabled = true;
                    pnenabletrue();
                    Pnloading.Visible = false;

                    txtorder.Text = output;
                    if (v == 2)
                    {

                        savereceived();
                        //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                        //if (result == DialogResult.Yes)
                        //{
                        //    Quotationreport rpt = new Quotationreport(output);
                        //    rpt.ShowDialog();
                        //}

                        //GetReport(output);
                    }
                }

                search("Quotationid", "", "q.Updatedon", "Today", "customername", "", role1, Program.userid);
                clear();
            }
            else
            {
                MessageBox.Show("Please Remove Duplication Product");
                panel1.Enabled = true;
                btnSavePending.Enabled = true;
                btnSave.Enabled = true;
                btnPrint.Enabled = true;

                //cmbcustomername.Enabled = true;
                //cmdcity.Enabled = true;
                //cmbreference.Enabled = true;
                //cmbassistby.Enabled = true;
                //  dgvOrder.ReadOnly = false;
                this.dgvOrder.Columns["Rate"].ReadOnly = true;
                this.dgvOrder.Columns[0].ReadOnly = true;
                this.dgvOrder.Columns["Items"].ReadOnly = true;
                this.dgvOrder.Columns["UOM"].ReadOnly = true;
                this.dgvOrder.Columns["Amount"].ReadOnly = true;
            }
        }



        public void pendingsave()
        {
            panel1.Enabled = true;
            //pnenablefalse();

            Pnloading.Visible = true;


            DataTable dt = new DataTable();
            
            objQuotationbal.Quotationid = txtorder.Text;
            
           
            dt = DataGridView2DataTable(dgvOrder);
            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }

            //dt.Columns.RemoveAt(4);

            dt.Columns["cmbAction"].ColumnName = "Status";

            dt.Columns[4].ColumnName = "PQuantity";



            RemoveNullColumnFromDataTable(dt);

            bool dtval = RemoveDuplicateRows(dt, "ProductId");


            if (dtval)
            {

                string output = objQuotationbal.pendingpdi(txtorder.Text, dt);
                if (!string.IsNullOrEmpty(output) )
                {
                    
                }
                
                search("Quotationid", "", "q.Updatedon", "Today", "customername", "", role1, Program.userid);
                clear();
                btnClear.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please Remove Duplication Product");
                panel1.Enabled = true;
                btnSavePending.Enabled = true;
                btnSave.Enabled = true;
                btnPrint.Enabled = true;

                //cmbcustomername.Enabled = true;
                //cmdcity.Enabled = true;
                //cmbreference.Enabled = true;
                //cmbassistby.Enabled = true;
                //  dgvOrder.ReadOnly = false;
                this.dgvOrder.Columns["Rate"].ReadOnly = true;
                this.dgvOrder.Columns[0].ReadOnly = true;
                this.dgvOrder.Columns["Items"].ReadOnly = true;
                this.dgvOrder.Columns["UOM"].ReadOnly = true;
                this.dgvOrder.Columns["Amount"].ReadOnly = true;
            }
        }

        public void savereceived()
        {

            //Pnloading.Visible = true;
            DataTable dt = new DataTable();
            if (cmbcustomername.SelectedIndex != 0)
            {
                objQuotationbal.Customerid = Convert.ToString(cmbcustomername.SelectedValue);
            }
            else
            {
                objQuotationbal.Customerid = "";
            }
            objQuotationbal.Customername = cmbcustomername.Text;
            objQuotationbal.Estinationid = txtorder.Text;
            objQuotationbal.Quotationid = "";
            //if (string.IsNullOrEmpty(Lblhidden.Text))
            //{
            objQuotationbal.isnew = 0;
            //}
            //else
            //{
            //    objQuotationbal.isnew = 1;
            //}



            objQuotationbal.Updatedby = Program.userid;
            dt = DataGridView2DataTable(dgvOrder);

            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }

            dt.Columns.RemoveAt(1);
            dt.Columns.RemoveAt(2);
            dt.Columns.RemoveAt(3);

            dt.Columns["cmbAction"].ColumnName = "Location";
            dt.Columns["Quantity"].ColumnName = "Total_Quantity";
            dt.Columns["Pending Quantity"].ColumnName = "Quantity";
            
            dt.AcceptChanges();

            RemoveNullColumnFromDataTableremove(dt);

            for (int s = 0; s < dt.Rows.Count; s++)
            {
                dt.Rows[s]["Location"] = "6";
            }


            //bool dtval = RemoveDuplicateRows(dt, "ProductId");


            //if (dtval)
            //{
            if (dt.Rows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Some Items Are Pending, Do You Want To Move Received?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string output = objQuotationbal.SaveIssuedreceivedwin(objQuotationbal, dt);
                    if (!string.IsNullOrEmpty(output))
                    {
                        GetReport(output);
                        clear();
                    }
                }

            }
            //}
            //else
            //{
            //    MessageBox.Show("Please Remove Duplication Product");
            //}

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
                dgvOrder.Rows[index].DefaultCellStyle.ForeColor = Color.Red;
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
                            cmd.CommandText = "GetReceived_Print";
                            cmd.Connection = con;
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(ds);

                            IssuedReceivedDal Obj = new IssuedReceivedDal();
                            Obj.dsMain = ds;
                            if (Obj.GenerateQuoation())
                            {
                                frmPrintPreview objfrmpreview = new frmPrintPreview();
                                objfrmpreview.fileName = Obj.fileName;
                                objfrmpreview.Show();

                            }



                            //System.Diagnostics.Process myProc = new System.Diagnostics.Process();
                            //myProc.StartInfo.FileName = "type " + Obj.fileName + " >prn";  //Attempting to start a non-existing executable
                            //myProc.Start();    //Start the application and assign it to the process component.    
                            //ExecuteCommandSync("type " + Obj.fileName + " >prn");



                        }
                    }
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }


        private void cmbcustomername_SelectedIndexChanged(object sender, EventArgs e)
        {

            string s = "";
            if (cmbcustomername.SelectedIndex > 0)
            {
                s = Convert.ToString(cmbcustomername.SelectedValue);
            }

            cmdcity.Text = objQuotationbal.bindcity(s);


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



        private void Btnsubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Txtitem.Text))
            {
                int rowindex = Convert.ToInt32(lblrowindex.Text);
                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
                dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                dgvOrder.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                dgvOrder.Rows[rowindex].Cells[4].Value = lblrack.Text;
                dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                DgvAutoRefNo.Visible = false;
                //  btnLess.Enabled = true;
                pnsearch.Visible = false;
                lblproductid.Text = string.Empty;
                //Txtitem.Text = string.Empty;
                lblitemcode.Text = "0";
                lblrack.Text = "0";
                lbldisplay.Text = "0";
                lbldemo.Text = "0";
                lblservice.Text = "0";
                lbldamage.Text = "0";
                lblprice.Text = "0";
                dgvOrder.Focus();
                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
            }
            else
            {
                MessageBox.Show("Please Enter Product Name");
                Txtitem.Focus();
            }


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

        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            ProdSelRowvalue = 0;

        }

        private void transactionclose_Click(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
            lblproductid.Text = string.Empty;
            //Txtitem.Text = string.Empty;
            lblitemcode.Text = "0";
            lblrack.Text = "0";
            lbldisplay.Text = "0";
            lbldemo.Text = "0";
            lblservice.Text = "0";
            lbldamage.Text = "0";
            lblprice.Text = "0";
            //Txtitem.Text = string.Empty;
            DgvAutoRefNo.DataSource = null;

            DgvAutoRefNo.Visible = false;
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


        public void RemoveNullColumnFromDataTableremove(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Quantity"])))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }

        private void dgvOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvOrder.CurrentCell.ColumnIndex;
            string headerText = dgvOrder.Columns[column].HeaderText;


            if (headerText.Equals("Status"))
            {
                cmbActionstatus = e.Control as ComboBox;
                cmbActionstatus.SelectedIndexChanged += cmbActionstatus_SelectedIndexChanged;
            }

            if (headerText.Equals("Pending Quantity"))
            {
                tb = e.Control as TextBox;


                if (tb != null)
                {
                    //tb.TextChanged += new EventHandler(tb_Change);
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.MaxLength = 10;
                }
            }

            if (headerText.Equals("Quantity"))
            {
                tb = e.Control as TextBox;
                //DataGridViewComboBoxCell cbCell = (DataGridViewComboBoxCell)dgvOrder.Rows[0].Cells[2];
                // dgvOrder.Rows[rowindex].Cells["cmbAction"].Value 
                if (tb != null)
                {
                    //tb.TextChanged += new EventHandler(textbox_Change);
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

        private void cmbActionstatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = cmbActionstatus.Text;
            if (s == "Pending" && !string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["Items"].Value)))
            {
                //Gridviewreadonly();
                dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["Pending Quantity"].ReadOnly = false;
            }
            else
            {
                dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["Pending Quantity"].Value = "";
                dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["Pending Quantity"].ReadOnly = true;
            }
        }


        //private void tb_Change(object sender, EventArgs e)
        //{
        //    int column = dgvOrder.CurrentCell.ColumnIndex;
        //    string headerText = dgvOrder.Columns[column].HeaderText;
        //    if (headerText.Equals("Pending Quantity"))
        //    {
        //        double sa = Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["Quantity"].Value);
        //        if (!string.IsNullOrEmpty(tb.Text))
        //        {
        //            if (Convert.ToDouble(tb.Text) > sa)
        //            {
        //                MessageBox.Show("Pending Quantity Shold Not Be Greater Than Quantity.");
        //               // tb.Text = string.Empty;
        //                dgvOrder.Focus();
        //                dgvOrder.CurrentCell = dgvOrder[7, dgvOrder.CurrentCell.RowIndex];
        //            }
        //        }
        //    }
        //}

        private void textbox_Change(object sender, EventArgs e)
        {
            int column = dgvOrder.CurrentCell.ColumnIndex;
            string headerText = dgvOrder.Columns[column].HeaderText;
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

        public void txtch(object sender, KeyPressEventArgs e)
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
            ////if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            ////    e.Handled = true;
            //if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar))
            //                    && (e.KeyChar != '.') && (e.KeyChar != '-'))
            //    e.Handled = true;


            ////if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            ////{
            ////    e.Handled = true;
            ////}

            //// only allow one decimal point
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = true;
            //}

            //// only allow minus sign at the beginning
            //if (e.KeyChar == '-' && (sender as TextBox).Text.Length > 0)
            //{
            //    e.Handled = true;
            //}
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

        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            total();
            if (e.ColumnIndex == 1)
            {
                rdbStartsWith.Checked = true;
                if (dgvOrder.ReadOnly == false)
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
            //else if (e.ColumnIndex == 4)
            //{
            //    if (dgvOrder.ReadOnly == false)
            //    {
            //        if (dgvOrder.Columns[4].ReadOnly==true)
            //        {
            //                dgvOrder.Rows[e.RowIndex].Cells[4].ReadOnly = true;

            //        }

            //    }
            //}
            //    else
            //    {
            //        pnsearch.Visible = false; ;
            //    }
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
        public void getquotaion(string s)
        {
            this.dgvOrder.Columns["Amount"].ReadOnly = true;
            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns["Items"].ReadOnly = true;
            this.dgvOrder.Columns["UOM"].ReadOnly = true;
            this.dgvOrder.Columns["productid"].ReadOnly = true;
            this.dgvOrder.Columns["Rate"].ReadOnly = true;
            DataSet ds = objQuotationbal.getquotation(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                cmdcity.Text = Convert.ToString(ds.Tables[0].Rows[0]["city"]);
                cmbreference.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Referenceid"]);
                cmbassistby.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Assist"]);
                lblperare.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);
                txtorder.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);
                cmbstatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                date.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);
                string final = Convert.ToString(ds.Tables[0].Rows[0]["Final"]);
                comboBox1.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["AssistName"]);
                pnenablefalse();
                btnSavePending.Enabled = true;
                btnSave.Enabled = true;
                btnPrint.Enabled = false;

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


                    btnSavePending.Enabled = false;
                    btnSave.Enabled = false;
                    btnPrint.Enabled = false;
                    dgvOrder.ReadOnly = false;
                }

            }
            else
            {
                pnenablefalse();
                btnSavePending.Enabled = false;
                btnSave.Enabled = false;
                btnPrint.Enabled = false;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                string s1 = string.Empty;
                double qty;
                dgvOrder.Rows.Clear();
                this.dgvOrder.Columns["Rate"].ReadOnly = true;
                btnSavePending.Enabled = true;
                btnSave.Enabled = true;
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvOrder.Rows.Add();
                    dgvOrder.Rows[i].Cells[0].Value = i + 1;
                    dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    s1 = Convert.ToString(ds.Tables[1].Rows[i]["Rate"]);
                    if (string.IsNullOrEmpty(s1))
                    {
                        qty = 0;
                    }
                    else
                    {
                        qty = Convert.ToDouble(s1);
                    }

                    dgvOrder.Rows[i].Cells[4].Value = qty;
                    dgvOrder.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvOrder.Rows[i].Cells[6].Value = amt;

                    if (!string.IsNullOrEmpty( Convert.ToString(ds.Tables[1].Rows[i]["pqty"])))
                    {
                        dgvOrder.Rows[i].Cells[7].Value = Convert.ToString(ds.Tables[1].Rows[i]["pqty"]);
                        dgvOrder.Rows[i].Cells["Pending Quantity"].ReadOnly = false;
                    }
                   

                    if (string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[i]["Status"])))
                    {
                        dgvOrder.Rows[i].Cells["cmbAction"].Value = "-Select-";
                    }
                   else
                    {
                        dgvOrder.Rows[i].Cells["cmbAction"].Value = Convert.ToString(ds.Tables[1].Rows[i]["Status"]);
                    }
                }


                dgvOrder.Focus();
                dgvOrder.CurrentCell = dgvOrder[8, 0];


                //for (int i = 0; i < dgvOrder.Rows.Count; i++)
                //{
                //    dgvOrder.Rows[i].Cells["cmbAction"].Value = "Select";
                //}
                //panel2.Enabled = false;
            }
            else
            {
                // btnLess.Enabled = false;
                dgvOrder.Rows.Clear();
                //pnenablefalse();
                btnSavePending.Enabled = false;
                btnSave.Enabled = false;
                btnPrint.Enabled = false;
            }

        }
        public void pnenabletrue()
        {

            //cmbcustomername.Enabled = true;
            //cmdcity.Enabled = true;
            //cmbreference.Enabled = true;
            //cmbassistby.Enabled = true;
            //panel3.Enabled = true;

        }
        public void pnenablefalse()
        {

            cmbcustomername.Enabled = false;
            cmdcity.Enabled = false;
            cmbreference.Enabled = false;
            //cmbassistby.Enabled = false;
            panel3.Enabled = false;

        }

        public void total()
        {
            try
            {
                double totalamount = 0.00, totalquantity = 0.00;
                double value = 0.0, value1 = 0.0;

                for (int i = 0; i < dgvOrder.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[6].Value)))
                    {
                        value = 0.0;
                    }
                    else
                    {
                        value = Convert.ToDouble(dgvOrder.Rows[i].Cells[6].Value);
                    }

                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[5].Value)))
                    {
                        value1 = 0.0;
                    }
                    else
                    {
                        value1 = Convert.ToDouble(dgvOrder.Rows[i].Cells[5].Value);
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

        private void dgvOrder_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (savevads == false)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value)))
                    {
                        decimal rate = Convert.ToDecimal(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
                        //decimal amt = rate * Convert.ToDecimal(tb.Text);

                        decimal amt = rate * Convert.ToDecimal(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value);

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
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = amt;
                    }


                }
                catch (Exception sa)
                {

                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                    {
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = 0.00;
                    }
                }
                total();

            }
            else
            {
                savevads = false;
            }
        }

        //private void btnSearch_Click(object sender, EventArgs e)
        //{
        //    if ((cbxSearchOrderNo.SelectedIndex == cbxSearchOrderDate.SelectedIndex) || cbxSearchOrderNo.SelectedIndex == cbxVendor.SelectedIndex || cbxSearchOrderDate.SelectedIndex == cbxVendor.SelectedIndex)
        //    {
        //        MessageBox.Show("Search a item Should Not Be Same");
        //    }
        //    else
        //    {


        //        firstname = cbxSearchOrderNo.Text.Trim();
        //        if (firstname == "Order Number")
        //        {
        //            firstname = "Quotationid";
        //            if (cmbstatus1.SelectedIndex != 0)
        //            {
        //                firstvalue = cmbstatus1.Text.Trim();
        //            }
        //            else
        //            {
        //                firstvalue = "";
        //            }
        //        }
        //        else if (firstname == "Customer")
        //        {
        //            firstname = "customername";
        //            if (cmbstatus1.SelectedIndex != 0)
        //            {
        //                firstvalue = cmbstatus1.Text.Trim();
        //            }
        //            else
        //            {
        //                firstvalue = "";
        //            }
        //        }
        //        else if (firstname == "Reference")
        //        {
        //            firstname = "r.Name";
        //            if (cmbstatus1.SelectedIndex != 0)
        //            {
        //                firstvalue = cmbstatus1.Text.Trim();
        //            }
        //            else
        //            {
        //                firstvalue = "";
        //            }
        //        }


        //        secondname = cbxSearchOrderDate.Text.Trim();
        //        if (secondname == "Order Number")
        //        {
        //            secondname = "Quotationid";
        //            if (cmbstatus2.SelectedIndex != 0)
        //            {
        //                secondvalue = cmbstatus2.Text.Trim();
        //            }
        //            else
        //            {
        //                secondvalue = "";
        //            }

        //        }
        //        else if (secondname == "Customer")
        //        {
        //            secondname = "customername";
        //            if (cmbstatus2.SelectedIndex != 0)
        //            {
        //                secondvalue = cmbstatus2.Text.Trim();
        //            }
        //            else
        //            {
        //                secondvalue = "";
        //            }
        //        }
        //        else if (secondname == "Reference")
        //        {
        //            secondname = "r.Name";
        //            if (cmbstatus2.SelectedIndex != 0)
        //            {
        //                secondvalue = cmbstatus2.Text.Trim();
        //            }
        //            else
        //            {
        //                secondvalue = "";
        //            }
        //        }


        //        thirdname = cbxVendor.Text.Trim();
        //        if (thirdname == "Order Number")
        //        {
        //            thirdname = "Quotationid";
        //            if (cmbstatus3.SelectedIndex != 0)
        //            {
        //                thirdvalue = cmbstatus3.Text.Trim();
        //            }
        //            else
        //            {
        //                thirdvalue = "";
        //            }


        //        }
        //        else if (thirdname == "Customer")
        //        {
        //            thirdname = "customername";
        //            if (cmbstatus3.SelectedIndex != 0)
        //            {
        //                thirdvalue = cmbstatus3.Text.Trim();
        //            }
        //            else
        //            {
        //                thirdvalue = "";
        //            }
        //        }
        //        else if (thirdname == "Reference")
        //        {
        //            thirdname = "r.Name";
        //            if (cmbstatus3.SelectedIndex != 0)
        //            {
        //                thirdvalue = cmbstatus3.Text.Trim();
        //            }
        //            else
        //            {
        //                thirdvalue = "";
        //            }
        //        }

        //        search(firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
        //    }
        //}

        public void bindorderno(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = objQuotationbal.ordernoval();
            cmb.DataSource = dt;
            cmb.DisplayMember = "Quotationid";
            cmb.ValueMember = "Quotationid";
        }

        public void bindcustomer(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = objQuotationbal.Getcustomer();
            cmb.DataSource = dt;
            cmb.DisplayMember = "Name";
            cmb.ValueMember = "CustomerID";
        }

        public void bindreference(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = objQuotationbal.Getreference();
            cmb.DataSource = dt;
            cmb.DisplayMember = "Name";
            cmb.ValueMember = "ReferencesID";
        }

        private void cbxSearchOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchOrderNo.SelectedIndex == 2)
            {
                cmbstatus1.Visible = true;
                txtsearch1.Visible = false;
                ListSearchDate1.Visible = false;

            }
            else if (cbxSearchOrderNo.SelectedIndex == 1)
            {
                cmbstatus1.Visible = false;
                txtsearch1.Visible = true;
                txtsearch1.Text = "Today";
                ListSearchDate1.Visible = true;
            }
            else
            {
                cmbstatus1.Visible = false;
                txtsearch1.Visible = true;
                txtsearch1.Text = string.Empty;
                ListSearchDate1.Visible = false;
            }
            pnlCalender.Visible = false;
        }
        private void cbxSearchOrderDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchOrderDate.SelectedIndex == 2)
            {
                cmbstatus2.Visible = true;
                txtsearch2.Visible = false;
                ListSearchDate2.Visible = false;

            }
            else if (cbxSearchOrderDate.SelectedIndex == 1)
            {
                cmbstatus2.Visible = false;
                txtsearch2.Visible = true;
                txtsearch2.Text = "Today";
                ListSearchDate2.Visible = true;
            }
            else
            {
                cmbstatus2.Visible = false;
                txtsearch2.Visible = true;
                txtsearch2.Text = string.Empty;
                ListSearchDate2.Visible = false;
            }

            pnlCalender.Visible = false;
        }
        private void cbxVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxVendor.SelectedIndex == 2)
            {
                cmbstatus3.Visible = true;
                txtsearch3.Visible = false;
                ListSearchDate3.Visible = false;
            }
            else if (cbxVendor.SelectedIndex == 1)
            {
                cmbstatus3.Visible = false;
                txtsearch3.Visible = true;
                txtsearch3.Text = "Today";
                ListSearchDate3.Visible = true;
            }
            else
            {
                cmbstatus3.Visible = false;
                txtsearch3.Visible = true;
                txtsearch3.Text = string.Empty;
                ListSearchDate3.Visible = false;
            }

            pnlCalender.Visible = false;
        }


        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {

            DataTable dt = objQuotationbal.searchnewpdi(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Quotationid"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Customer"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Reference"]);
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string s = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex].Cells[0].Value);
                if (!string.IsNullOrEmpty(s))
                {
                    getquotaion(s);
                    total();
                    Gridviewreadonly();
                    btnClear.Enabled = true;

                }
                else
                {
                    clear();
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
                    if (dgvOrder.CurrentCell.ColumnIndex == 0)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 1)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 2)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[4, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 4)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[5, dgvOrder.CurrentCell.RowIndex];

                    }


                    else if (dgvOrder.CurrentCell.ColumnIndex == 6)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];

                    }
                }
                catch
                {

                }

            }


        }

        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5)
                {
                    dgvOrder.Focus();
                    //edit = true;
                    dgvOrder.CurrentCell = dgvOrder[1, e.RowIndex + 1];
                }

                if (e.ColumnIndex == 4)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value)))
                    {
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = 0;
                    }
                }
            }
            catch
            {

            }
        }

        private void dgvOrder_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (edit == true)
                {
                    if (dgvOrder.CurrentCell.RowIndex >= 1)
                    {
                        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex - 1];
                        edit = false;
                    }
                    else if (dgvOrder.CurrentCell.RowIndex == 0)
                    {
                        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex - 1];
                    }

                }
            }
            catch
            {

            }
        }

        private void Txtitem_KeyDown(object sender, KeyEventArgs e)
        {
            //if(e.KeyData==Keys.Down)
            //{
            //    DgvAutoRefNo.Focus();
            //    DgvAutoRefNo.CurrentCell = DgvAutoRefNo[0, 0];
            //    DgvAutoRefNo.Rows[0].Cells[0].Selected = true;
            //}
            //try
            //{
            //    if (e.KeyData == Keys.Enter)
            //    {
            //        if (!string.IsNullOrEmpty(Txtitem.Text))
            //        {
            //            if (Convert.ToInt32(lblproductid.Text) != 0)
            //            {
            //                dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
            //                int rowindex = Convert.ToInt32(lblrowindex.Text);
            //                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
            //                dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
            //                dgvOrder.Rows[rowindex].Cells[1].Value = cas.ToUpper();
            //                dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
            //                double val = Convert.ToDouble(lblprice.Text);
            //                dgvOrder.Rows[rowindex].Cells[4].Value = val;
            //                dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
            //                DgvAutoRefNo.Visible = false;

            //                pnsearch.Visible = false;
            //                lblproductid.Text = string.Empty;
            //                Txtitem.Text = string.Empty;
            //                lblitemcode.Text = "0";
            //                lblrack.Text = "0";
            //                lbldisplay.Text = "0";
            //                lbldemo.Text = "0";
            //                lblservice.Text = "0";
            //                lbldamage.Text = "0";
            //                lblprice.Text = "0";
            //                dgvOrder.Focus();
            //                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
            //            }
            //            else
            //            {
            //                MessageBox.Show("Please Enter Correct Product Name");
            //            }
            //        }
            //        else
            //        {
            //            this.ActiveControl = btnSave;
            //            pnsearch.Visible = false;
            //            //MessageBox.Show("Please Enter Product Name");
            //            //Txtitem.Focus();
            //        }
            //    }
            //}
            //catch
            //{

            //}
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

        private void SalesPDI_FormClosing(object sender, FormClosingEventArgs e)
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
            if (e.RowIndex >= 0)
            {
                if (Convert.ToInt32(lblproductid.Text) != 0)
                {
                    //dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
                    int rowindex = Convert.ToInt32(lblrowindex.Text);
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];

                    //itemdetails(Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value));

                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                    getitems(sa);

                    dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    //btnLess.Enabled = true;
                    dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();

                    //dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    //dgvOrder.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                    dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                    double val = Convert.ToDouble(lblprice.Text);
                    dgvOrder.Rows[rowindex].Cells[4].Value = val;





                    decimal rate = Convert.ToDecimal(dgvOrder.Rows[rowindex].Cells[4].Value);
                    //decimal amt = rate * Convert.ToDecimal(tb.Text);

                    decimal amt = rate * Convert.ToDecimal(dgvOrder.Rows[rowindex].Cells[5].Value);


                    dgvOrder.Rows[rowindex].Cells[6].Value = amt;






                    dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                    DgvAutoRefNo.Visible = false;
                    dgvOrder.Rows[rowindex].Cells["cmbAction"].Value = "-Select-";


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
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
                }
            }
            //else
            //{
            //    MessageBox.Show("Please Enter Correct Product Name");
            //}
        }

        private void dgvOrder_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {


                double sa = Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["Quantity"].Value);
                double pq = Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["Pending Quantity"].Value);
                if (!string.IsNullOrEmpty(pq.ToString()))
                {
                    if (Convert.ToDouble(pq) > sa)
                    {
                        MessageBox.Show("Pending Quantity Shold Not Be Greater Than Quantity.");
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["Pending Quantity"].Value = "";
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[7, dgvOrder.CurrentCell.RowIndex];
                        edit = true;
                    }
                    else
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[8, dgvOrder.CurrentCell.RowIndex+1];
                    }
                }


                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value)))
                {
                    decimal rate = Convert.ToDecimal(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
                    //decimal amt = rate * Convert.ToDecimal(tb.Text);

                    decimal amt = rate * Convert.ToDecimal(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value);

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
                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = amt;
                }


            }
            catch
            {
                //MessageBox.Show(e1.Message.ToString());
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value)))
                    {
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = 0.00;
                    }

                }

            }


            //if (e.ColumnIndex == 4)
            //{
            //    decimal rate = Convert.ToDecimal(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
            //    //decimal amt = rate * Convert.ToDecimal(tb.Text);

            //    decimal amt = rate * Convert.ToDecimal(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value);

            //    if (amt > 0)
            //    {
            //        string[] str = amt.ToString().Split('.');
            //        if (str.Length > 1)
            //        {
            //            double num1 = Convert.ToDouble("0." + str[1]);

            //            if (num1 >= 0.5)
            //            {
            //                amt = Math.Ceiling(amt);
            //            }
            //            else
            //            {
            //                amt = Math.Floor(amt);
            //            }

            //        }
            //    }
            //    else
            //    {
            //        string[] str = amt.ToString().Split('.');
            //        if (str.Length > 1)
            //        {
            //            double num1 = Convert.ToDouble("0." + str[1]);

            //            if (num1 >= 0.5)
            //            {
            //                amt = Math.Floor(amt);
            //            }
            //            else
            //            {
            //                amt = Math.Ceiling(amt);
            //            }

            //        }
            //    }
            //    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = amt;
            //}

            total();
        }


        public void getsino()
        {
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                dgvOrder.Rows[i].Cells[0].Value = i + 1;
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
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
                        dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        // btnLess.Enabled = true;
                        dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                        dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                        double val = Convert.ToDouble(lblprice.Text);
                        dgvOrder.Rows[rowindex].Cells[4].Value = val;



                        decimal rate = Convert.ToDecimal(dgvOrder.Rows[rowindex].Cells[4].Value);
                        //decimal amt = rate * Convert.ToDecimal(tb.Text);

                        decimal amt = rate * Convert.ToDecimal(dgvOrder.Rows[rowindex].Cells[5].Value);


                        dgvOrder.Rows[rowindex].Cells[6].Value = amt;



                        dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                        DgvAutoRefNo.Visible = false;
                        dgvOrder.Rows[rowindex].Cells["cmbAction"].Value = "-Select-";
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
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
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
            clickstatus = "search1";
            Calender();
            pnlCalender.Location = new Point(133, 54);
        }

        private void ListSearchDate2_Click(object sender, EventArgs e)
        {
            clickstatus = "search2";
            Calender();
            pnlCalender.Location = new Point(133, 79);
        }

        private void ListSearchDate3_Click(object sender, EventArgs e)
        {
            clickstatus = "search3";
            Calender();
            pnlCalender.Location = new Point(133, 103);
        }


        private void Calender()
        {
            if (pnlCalender.Visible)
            {
                pnlCalender.Visible = false;
            }
            else
            {
                pnlCalender.BringToFront();
                pnlCalender.Visible = true;
            }
        }

        private void label40_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = label40.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = label40.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = label40.Text.Trim();
            }
            pnlCalender.Visible = false;
        }

        private void lblToday_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblToday.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblToday.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblToday.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblThisWeek_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblThisWeek.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblThisWeek.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblThisWeek.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblThisMonth_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblThisMonth.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblThisMonth.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblThisMonth.Text.Trim();
            }
            pnlCalender.Visible = false;
        }

        private void lblThisYear_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblThisYear.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblThisYear.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblThisYear.Text.Trim();
            }
            pnlCalender.Visible = false;
        }
        private void lblYesterday_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblYesterday.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblYesterday.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblYesterday.Text.Trim();
            }

            pnlCalender.Visible = false;
        }

        private void lblLastWeek_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblLastWeek.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblLastWeek.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblLastWeek.Text.Trim();
            }

            pnlCalender.Visible = false;
        }

        private void lblLastMonth_Click(object sender, EventArgs e)
        {

            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblLastMonth.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblLastMonth.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblLastMonth.Text.Trim();
            }
            pnlCalender.Visible = false;
        }

        private void lblLastYear_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblLastYear.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblLastYear.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblLastYear.Text.Trim();
            }
            pnlCalender.Visible = false;
        }

        private void SearchFrmDate_ValueChanged(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = SearchFrmDate.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = SearchFrmDate.Text.Trim();
            }
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = SearchFrmDate.Text.Trim();
            }
            pnlCalender.Visible = false;
        }


        private void btnSearch_Click_1(object sender, EventArgs e)
        {

            try
            {
                if ((cbxSearchOrderNo.SelectedIndex == cbxSearchOrderDate.SelectedIndex) || cbxSearchOrderNo.SelectedIndex == cbxVendor.SelectedIndex || cbxSearchOrderDate.SelectedIndex == cbxVendor.SelectedIndex)
                {
                    MessageBox.Show("* Search a item Should Not Be Same");
                }
                else
                {
                    string firstname = string.Empty, firstvalue = string.Empty, secondname = string.Empty, secondvalue = string.Empty, thirdname = string.Empty, thirdvalue = string.Empty;
                    string firstname1 = string.Empty, firstvalue1 = string.Empty, secondname1 = string.Empty, secondvalue1 = string.Empty, thirdname1 = string.Empty, thirdvalue1 = string.Empty;

                    firstname = cbxSearchOrderNo.Text.Trim();
                    if (firstname == "Order Number")
                    {
                        firstname = "Order Number";
                        //firstvalue = Convert.ToString(cmbstatus1.SelectedValue);
                        if (cmbstatus1.SelectedIndex > 0)
                        {
                            firstvalue = cmbstatus1.Text;
                        }
                        else
                        {
                            firstvalue = "";
                        }
                    }
                    else
                    {
                        //firstvalue = txtsearch1.Text.Trim();
                        string part1 = string.Empty;
                        //secondvalue = txtsearch2.Text.Trim();
                        if (txtsearch1.Text.Trim() != "ALL")
                        {
                            part1 = txtsearch1.Text.Trim();
                        }
                        else if (txtsearch1.Text.Trim() == "ALL")
                        {
                            part1 = string.Empty;
                            firstvalue = "All";
                        }
                        else
                        {
                            part1 = string.Empty;
                            firstvalue = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(part1))
                        {
                            string part = part1.Substring(0, 2);
                            if (Char.IsDigit(part, 0))
                            {
                                string date = txtsearch1.Text;
                                DateTime dt = Convert.ToDateTime(date);
                                string formatted = dt.ToString("dd-MM-yyyy");
                                string[] rr = formatted.Split('-');
                                //  string[] rr = txtsearch1.Text.Split('-');
                                firstvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                            }
                            else
                            {
                                firstvalue = txtsearch1.Text.Trim();
                            }
                        }
                    }

                    secondname = cbxSearchOrderDate.Text.Trim();
                    if (secondname == "Order Number")
                    {
                        secondname = "Order Number";
                        //secondvalue = Convert.ToString(cmbstatus2.SelectedValue);
                        if (cmbstatus2.SelectedIndex > 0)
                        {
                            secondvalue = cmbstatus1.Text;
                        }
                        else
                        {
                            secondvalue = "";
                        }



                    }
                    else
                    {
                        string part1 = string.Empty;
                        //secondvalue = txtsearch2.Text.Trim();
                        if (txtsearch2.Text.Trim() != "ALL")
                        {
                            part1 = txtsearch2.Text.Trim();
                        }
                        else if (txtsearch2.Text.Trim() == "ALL")
                        {
                            part1 = string.Empty;
                            secondvalue = "All";
                        }
                        else
                        {
                            part1 = string.Empty;
                            secondvalue = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(part1))
                        {
                            string part = part1.Substring(0, 2);
                            if (Char.IsDigit(part, 0))
                            {
                                string date = txtsearch2.Text;
                                DateTime dt = Convert.ToDateTime(date);
                                string formatted = dt.ToString("dd-MM-yyyy");
                                string[] rr = formatted.Split('-');
                                // string[] rr = txtsearch2.Text.Split('-');
                                secondvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                            }
                            else
                            {
                                secondvalue = txtsearch2.Text.Trim();
                            }
                        }
                    }


                    thirdname = cbxVendor.Text.Trim();
                    if (thirdname == "Order Number")
                    {
                        thirdname = "Order Number";

                        if (cmbstatus3.SelectedIndex > 0)
                        {
                            thirdvalue = cmbstatus3.Text;
                        }
                        else
                        {
                            thirdvalue = "";
                        }


                    }
                    else
                    {
                        //thirdvalue = txtsearch3.Text.Trim();
                        string part1 = string.Empty;
                        //secondvalue = txtsearch2.Text.Trim();
                        if (txtsearch3.Text.Trim() != "ALL")
                        {
                            part1 = txtsearch3.Text.Trim();
                        }
                        else if (txtsearch3.Text.Trim() == "ALL")
                        {
                            part1 = string.Empty;
                            thirdvalue = "All";
                        }
                        else
                        {
                            part1 = string.Empty;
                            thirdvalue = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(part1))
                        {
                            string part = part1.Substring(0, 2);
                            if (Char.IsDigit(part, 0))
                            {
                                string date = txtsearch3.Text;
                                DateTime dt = Convert.ToDateTime(date);
                                string formatted = dt.ToString("dd-MM-yyyy");
                                string[] rr = formatted.Split('-');

                                // string[] rr = txtsearch3.Text.Split('-');
                                thirdvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                            }
                            else
                            {
                                thirdvalue = txtsearch3.Text.Trim();
                            }
                        }
                    }

                    if (firstname == "Customer")
                    {
                        firstname1 = "customername";
                        firstvalue1 = firstvalue;
                    }
                    else if (firstname == "Order Date")
                    {
                        secondname1 = "q.Updatedon";
                        secondvalue1 = firstvalue;
                    }
                    else if (firstname == "Order Number")
                    {
                        //if (selectedtab == "TabNew")

                        //    thirdname1 = "Quotationid";

                        //else if (selectedtab == "TabFloorCheckOut")

                        //    thirdname1 = "Estimationid";
                        thirdvalue1 = firstvalue;
                    }

                    if (secondname == "Customer")
                    {
                        firstname1 = "customername";
                        firstvalue1 = secondvalue;
                    }
                    else if (secondname == "Order Date")
                    {
                        secondname1 = "q.Updatedon";
                        secondvalue1 = secondvalue;
                    }
                    else if (secondname == "Order Number")
                    {
                        //if (selectedtab == "TabNew")

                        //    thirdname1 = "Quotationid";

                        //else if (selectedtab == "TabFloorCheckOut")

                        //    thirdname1 = "Estimationid";

                        thirdvalue1 = secondvalue;
                    }

                    if (thirdname == "Customer")
                    {
                        firstname1 = "customername";
                        firstvalue1 = thirdvalue;
                    }
                    else if (thirdname == "Order Date")
                    {
                        secondname1 = "q.Updatedon";
                        secondvalue1 = thirdvalue;
                    }
                    else if (thirdname == "Order Number")
                    {
                        //if (selectedtab == "TabNew")

                        //    thirdname1 = "Quotationid";

                        //else if (selectedtab == "TabFloorCheckOut")
                        //    thirdname1 = "Estimationid";

                        thirdname1 = "Quotationid";
                        thirdvalue1 = thirdvalue;
                    }




                    search(firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);


                }
            }
            catch(Exception ex)
            {

            }
           
        }

        private void dgvOrder_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if(e.ColumnIndex==4)
            //{
            //    if (dgvOrder.ReadOnly == false)
            //    {
            //        if (dgvOrder.Columns[4].ReadOnly == true)
            //        {
            //            pnlless.Visible = true;
            //            txtlesspwd.Focus();
            //        }
            //    }
            //}
        }







        private void Btnmobilenumber_Click(object sender, EventArgs e)
        {

        }


        private void btnReport_Click(object sender, EventArgs e)
        {
            // GetReport(QuotationId);
        }

        private void dgvOrder_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    if (dgvOrder[e.ColumnIndex, e.RowIndex].EditType.ToString() == "System.Windows.Forms.DataGridViewComboBoxEditingControl")
                    {
                        DataGridViewColumn column = dgvOrder.Columns[e.ColumnIndex];
                        if (column is DataGridViewComboBoxColumn)
                        {
                            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dgvOrder[e.ColumnIndex, e.RowIndex];
                            dgvOrder.CurrentCell = cell;
                            dgvOrder.BeginEdit(true);
                            DataGridViewComboBoxEditingControl editingControl = (DataGridViewComboBoxEditingControl)dgvOrder.EditingControl;
                            editingControl.DroppedDown = true;
                        }
                    }
                }
                catch
                {
                    //MessageBox.Show("Please Select Status");
                }
            }
        }

        private void dgvOrder_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
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
                           
                            btnClear.Enabled = true;
                        }
                        else
                        {
                            clear();
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
                            
                            btnClear.Enabled = true;
                        }
                        else
                        {
                            clear();
                        }

                    }
                }
            }
            catch
            {

            }
        }

        //public void GetReport(string QuotationId)
        // {
        //     try
        //     {
        //         if (!string.IsNullOrEmpty(QuotationId))
        //         {
        //             DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        //             if (result == DialogResult.Yes)
        //             {
        //                 // Quotationreport rpt = new Quotationreport(txtorder.Text);
        //                 //rpt.ShowDialog();

        //                 using (SqlConnection con = new SqlConnection(Program.connection))
        //                 {
        //                     DataSet ds = new DataSet();
        //                     con.Open();
        //                     SqlCommand cmd = new SqlCommand();
        //                     cmd.Parameters.AddWithValue("@id", QuotationId);
        //                     cmd.Parameters.AddWithValue("@companyname", Program.Company);
        //                     cmd.CommandType = CommandType.StoredProcedure;
        //                     cmd.CommandText = "GetQuotationreport";
        //                     cmd.Connection = con;
        //                     SqlDataAdapter ad = new SqlDataAdapter(cmd);
        //                     ad.Fill(ds);



        //                     if (ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
        //                     {

        //                         QuotationReportDAL Obj = new QuotationReportDAL();
        //                         Obj.GetDataSet(ds);
        //                         Obj.PrintHeader();
        //                         Obj.PrintDetails();
        //                         Obj.PrintFooter();
        //                         Obj.PrintBuffer();
        //                         Obj.Close();

        //                     }
        //                 }
        //             }
        //         }

        //     }
        //     catch
        //     {

        //     }
        // }






    }
}
