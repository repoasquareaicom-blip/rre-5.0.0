using InvBal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Inventory.Purchase
{
    public partial class PurchaseReceipt : Form
    {
        PurchaseOrderBAL OblPurchaseOrderBAL = new PurchaseOrderBAL();
        PurchaseReceiptBAL ObjPurchaseReceiptBAL = new PurchaseReceiptBAL();
        QuotationBal objQuotationbal = new QuotationBal();
        string clickstatus = string.Empty;
        public TextBox tb, tbreceipt;
        public ComboBox combo;
        public bool edit = false;
        bool CellQtyValidation = false;
        bool CellCheckingQtyValidation = false;
        bool IsNewPO = false;
        int userid = 0;
        DataTable dtitems;
        string role = "";
        string UserId = "";
        string UserName = "";
        bool ProdNotFoundMSg = false;
        DataTable dtpartial = new DataTable();
        TextBox goodquantity, demagequantity;
        double GoodQty = 0;
        double DamageQty = 0;
        double totQty = 0;
        string cas = string.Empty;
        int ProdSelRowvalue = 0;
        bool res = false;
        string selectedtab = string.Empty;
        int InsDel = 1;
        string test;
        public PurchaseReceipt()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
            UserId = Convert.ToString(Program.userid);
            role = Program.Userrole;
            UserName = Program.UserName;
            LoadReceipt();
            LoadReceiptmod();
            LoadProductChecking();
            LoadBarcode();
            LoadFloorIncharge();
            LoadDamage();
            LoadInvoice();
           
            SearchPurchaseOrder();
            bindLocation();
            cmbloaction.SelectedIndex = 1;
            cbxStatus.SelectedIndex = 0;
            comstatuss.SelectedIndex = 0;
            combostatus.SelectedIndex = 0;
            comboBoxstatus.SelectedIndex = 0;
            combobxstatus.SelectedIndex = 0;

            CmbInvROS.SelectedIndex = 0;
            CmbInvOCS.SelectedIndex = 0;
            btnReject.Enabled = false;


            GetSuppliers();
            BindSupplierSearch();
            DataTable dtitems = Program.dtitems;
            if (dtitems == null)
            {
                itemdetails("");
            }

            GetsearchPurchasegoods();

            DataTable dt = Program.Dtmenu;

            selectedtab = tabControl1.SelectedTab.Name;
            tabControl1.TabPages.Remove(TabReceipt);

            // Remove Delivery,Packing
            tabControl1.TabPages.Remove(TabInvoice);
            tabControl1.TabPages.Remove(TabChecking);
            tabControl1.TabPages.Remove(TabBarCode);
            tabControl1.TabPages.Remove(TabFloorCheckIn);
            tabControl1.TabPages.Remove(TabDamage);
//            bool contains5 = dt.AsEnumerable()
//.Any(row => "ReceiptModification" == row.Field<String>("Data"));


//            if (contains5 == false)
//            {
//                tabControl1.TabPages.Remove(TabReceipt);
//            }

//            bool contains = dt.AsEnumerable()
//             .Any(row => "PurchaseReceiptInvoice" == row.Field<String>("Data"));


//            if (contains == false)
//            {
//                tabControl1.TabPages.Remove(TabInvoice);
//            }


//            bool contains1 = dt.AsEnumerable()
//           .Any(row => "PurchaseReceiptChecking" == row.Field<String>("Data"));


//            if (contains1 == false)
//            {
//                tabControl1.TabPages.Remove(TabChecking);
//            }


//            bool contains2 = dt.AsEnumerable()
//          .Any(row => "PurchaseReceiptBarcode" == row.Field<String>("Data"));


//            if (contains2 == false)
//            {
//                tabControl1.TabPages.Remove(TabBarCode);
//            }


//            bool contains3 = dt.AsEnumerable()
//       .Any(row => "PurchaseReceiptFloorCheckin" == row.Field<String>("Data"));


//            if (contains3 == false)
//            {
//                tabControl1.TabPages.Remove(TabFloorCheckIn);
//            }

//            bool contains4 = dt.AsEnumerable()
// .Any(row => "PurchaseReceiptDemage" == row.Field<String>("Data"));
            

//            if (contains4 == false)
//            {
//                tabControl1.TabPages.Remove(TabDamage);
//            }




        }

        public void GetSuppliers()
        {
            DataTable dtsup = OblPurchaseOrderBAL.GetSuppliers(null);
            cbVendor.DataSource = dtsup;
            cbVendor.ValueMember = "SuppliersID";
            cbVendor.DisplayMember = "Name";
            cbVendor.SelectedIndex = 0;

        }

        public void BindSupplierSearch()
        {
            //DataTable dtsup = OblPurchaseOrderBAL.GetSuppliers(null);
            //comboBox1.DataSource = dtsup;
            //comboBox1.ValueMember = "SuppliersID";
            //comboBox1.DisplayMember = "Name";
            //comboBox1.SelectedIndex = 0;

            //comboBox1.DataSource = dtsup;
            //comboBox1.ValueMember = "SuppliersID";
            //comboBox1.DisplayMember = "Name";
            //comboBox1.SelectedIndex = 0;

            //comboBox1.DataSource = dtsup;
            //comboBox1.ValueMember = "SuppliersID";
            //comboBox1.DisplayMember = "Name";
            //comboBox1.SelectedIndex = 0;
        }

        //public void AutoCompleteLoad(string s,int t)
        //{
        //    AutoCompleteStringCollection str = new AutoCompleteStringCollection();
        //    DataTable st = objQuotationbal.itemauto(s,t);

        //    if (st.Rows.Count > 0)
        //    {
        //        DgvAutoRefNo.Visible = true;
        //        DgvAutoRefNo.DataSource = st;
        //        res = false;
        //        itemdetails(Convert.ToString(DgvAutoRefNo.Rows[0].Cells[0].Value));
        //        cas = Convert.ToString(DgvAutoRefNo.Rows[0].Cells[0].Value);
        //    }
        //    else
        //    {
        //        DgvAutoRefNo.Visible = false;
        //    }


        //    //string[] arr = new string[st.Rows.Count];
        //    //for (int i = 0; i < st.Rows.Count; i++)
        //    //{
        //    //    arr[i] = st.Rows[i]["DisplayName"].ToString();
        //    //}
        //    //for (int i = 0; i < arr.Length; i++)
        //    //{
        //    //    //var combined = string.Join(", ", arr);
        //    //    var combined = arr[i];
        //    //    str.Add(combined);
        //    //}

        //    //Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        //    //Txtitem.AutoCompleteCustomSource = str;
        //    //Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;

        //    //for (int i = 0; i < arr.Length; i++)
        //    //{
        //    //  var combined = string.Join(", ", arr);
        //    //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
        //    //str.Add(combined);
        //    //}

        //    //for (int i = 0; i < st.Rows.Count; i++)
        //    //{
        //    //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
        //    //    str.Add(combined);
        //    //}


        //}


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
        public void bindLocation()
        {
            cmbloaction.DataSource = ObjPurchaseReceiptBAL.GetLocation();
            cmbloaction.DisplayMember = "LocationName";
            cmbloaction.ValueMember = "LocationID";
        }

        private void LoadInvoice()
        {

            dgvinvoice.Rows.Clear();
            dgvinvoice.Columns.Clear();
            dgvinvoice.RowCount = 1;
            dgvinvoice.ColumnCount = 8;

            dgvinvoice.Columns[0].Name = "Sno";
            dgvinvoice.Columns[1].Name = "Items";
            dgvinvoice.Columns[2].Name = "UOM";
            dgvinvoice.Columns[3].Name = "Quantity";
            dgvinvoice.Columns[4].Name = "Price";
            //dgvinvoice.Columns[5].Name = "Tax";
            dgvinvoice.Columns[5].Name = "Amount";
            dgvinvoice.Columns[6].Name = "ProductId";
            dgvinvoice.Columns[7].Name = "Sum";
            //dgvinvoice.Columns[4].Name = "Discount";
            //dgvinvoice.Columns[5].Name = "Tax";
            //dgvinvoice.Columns[6].Name = "Sub Total";

            this.dgvinvoice.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[0].Width = 60;
            // this.dgvOrder.Columns[0].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[0].ReadOnly = true;

            this.dgvinvoice.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[1].Width = 300;
            // this.dgvOrder.Columns[0].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[1].ReadOnly = true;


            this.dgvinvoice.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[2].Width = 80;
            //this.dgvOrder.Columns[1].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            this.dgvinvoice.Columns[2].ReadOnly = true;


            this.dgvinvoice.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[3].Width = 90;
            //this.dgvOrder.Columns[2].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvinvoice.Columns[3].ReadOnly = true;


            this.dgvinvoice.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[4].Width = 90;
            //this.dgvOrder.Columns[3].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            this.dgvinvoice.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[5].Width = 90;

            this.dgvinvoice.Columns[5].ReadOnly = true;
            //this.dgvOrder.Columns[4].DefaultCellStyle.BackColor = Color.Beige;
            dgvinvoice.Columns[6].Visible = false;
            dgvinvoice.Columns[7].Visible = false;

            DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
            cmb.HeaderText = "Tax";
            cmb.Name = "cmbtax";
            cmb.MaxDropDownItems = 4;
            cmb.Items.Add("14.50");
            cmb.Items.Add("5.00");
            cmb.Items.Add("No Tax");
            cmb.DefaultCellStyle.NullValue = "Select";
            dgvinvoice.Columns.Insert(5, cmb);
            this.dgvinvoice.Columns[5].Width = 80;
            this.dgvinvoice.Columns[6].Width = 100;
            this.dgvinvoice.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            //this.dgvinvoice.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvinvoice.Columns[5].Width = 50;
            ////this.dgvOrder.Columns[5].DefaultCellStyle.BackColor = Color.Beige;


            //this.dgvinvoice.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvinvoice.Columns[6].Width = 100;
            // this.dgvOrder.Columns[6].DefaultCellStyle.BackColor = Color.Beige;


            dgvinvoice.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvinvoice.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }
        }

        private void LoadReceipt()
        {
            dgvOrder.Rows.Clear();
            dgvOrder.Columns.Clear();
            dgvOrder.RowCount = 1;
            dgvOrder.ColumnCount = 7;

            dgvOrder.Columns[0].Name = "S.No";
            dgvOrder.Columns[1].Name = "Items";
            dgvOrder.Columns[2].Name = "Price";
            dgvOrder.Columns[3].Name = "Ordered Qty";
            dgvOrder.Columns[4].Name = "Received Qty";
            dgvOrder.Columns[5].Name = "ProductId";
            dgvOrder.Columns[6].Name = "Status";
            //dgvOrder.Columns[5].Name = "Tax";
            //dgvOrder.Columns[6].Name = "Sub Total";

            dgvOrder.Columns[3].Visible = false;
            dgvOrder.Columns[5].Visible = false;
            this.dgvOrder.Columns[0].ReadOnly = true;
            //dgvOrder.Columns[4].Visible = false;
            //dgvOrder.Columns[5].Visible = false;
            //dgvOrder.Columns[6].Visible = false;


            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[0].Width = 100;

            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[1].Width = 780;
            // this.dgvOrder.Columns[0].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvOrder.Columns[1].ReadOnly = true;
            this.dgvOrder.Columns[6].Visible = false;




            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].Width = 40;
            //this.dgvOrder.Columns[1].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            this.dgvOrder.Columns[2].ReadOnly = true;
            this.dgvOrder.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvOrder.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[3].Width = 100;
            this.dgvOrder.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dgvOrder.Columns[2].DefaultCellStyle.BackColor = Color.Beige;


            this.dgvOrder.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[4].Width = 90;
            //this.dgvOrder.Columns[3].DefaultCellStyle.BackColor = Color.Beige;


            //this.dgvOrder.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvOrder.Columns[4].Width = 50;
            ////this.dgvOrder.Columns[4].DefaultCellStyle.BackColor = Color.Beige;


            //this.dgvOrder.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvOrder.Columns[5].Width = 50;
            ////this.dgvOrder.Columns[5].DefaultCellStyle.BackColor = Color.Beige;


            //this.dgvOrder.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvOrder.Columns[6].Width = 100;
            //// this.dgvOrder.Columns[6].DefaultCellStyle.BackColor = Color.Beige;


            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }
        private void LoadReceiptmod()
        {
            dgvrepreceipt.Rows.Clear();
            dgvrepreceipt.Columns.Clear();
            dgvrepreceipt.RowCount = 1;
            dgvrepreceipt.ColumnCount = 4;

            dgvrepreceipt.Columns[0].Name = "S.No";
            dgvrepreceipt.Columns[1].Name = "Items";
            dgvrepreceipt.Columns[2].Name = "Ordered Qty";

            dgvrepreceipt.Columns[3].Name = "ProductId";

            //dgvOrder.Columns[5].Name = "Tax";
            //dgvOrder.Columns[6].Name = "Sub Total";

            dgvrepreceipt.Columns[3].Visible = false;

            this.dgvrepreceipt.Columns[0].ReadOnly = true;
            //dgvOrder.Columns[4].Visible = false;
            //dgvOrder.Columns[5].Visible = false;
            //dgvOrder.Columns[6].Visible = false;


            this.dgvrepreceipt.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvrepreceipt.Columns[0].Width = 100;

            this.dgvrepreceipt.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvrepreceipt.Columns[1].Width = 800;
            // this.dgvOrder.Columns[0].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvrepreceipt.Columns[1].ReadOnly = true;





            this.dgvrepreceipt.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvOrder.Columns[2].Width = 40;
            //this.dgvOrder.Columns[1].DefaultCellStyle.BackColor = Color.LightSkyBlue;

            this.dgvrepreceipt.Columns[2].Width = 100;
            this.dgvrepreceipt.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvrepreceipt.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvrepreceipt.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dgvOrder.Columns[2].DefaultCellStyle.BackColor = Color.Beige;



            //this.dgvOrder.Columns[4].Width = 70;
            //this.dgvOrder.Columns[3].DefaultCellStyle.BackColor = Color.Beige;


            //this.dgvOrder.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvOrder.Columns[4].Width = 50;
            ////this.dgvOrder.Columns[4].DefaultCellStyle.BackColor = Color.Beige;


            //this.dgvOrder.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvOrder.Columns[5].Width = 50;
            ////this.dgvOrder.Columns[5].DefaultCellStyle.BackColor = Color.Beige;


            //this.dgvOrder.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvOrder.Columns[6].Width = 100;
            //// this.dgvOrder.Columns[6].DefaultCellStyle.BackColor = Color.Beige;


            dgvrepreceipt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvrepreceipt.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }

        private void LoadProductChecking()
        {
            dgvChecking.Columns.Clear();
            dgvChecking.Rows.Clear();
            dgvChecking.ColumnCount = 6;
            dgvChecking.RowCount = 1;

            dgvChecking.Columns[0].Name = "Sno";
            this.dgvChecking.Columns[0].Width = 40;
            //this.dgvChecking.Columns[0].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvChecking.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[0].ReadOnly = true;

            dgvChecking.Columns[1].Name = "Items";

            //this.dgvChecking.Columns[0].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvChecking.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[1].ReadOnly = true;

            dgvChecking.Columns[2].Name = "Quantity";
            this.dgvChecking.Columns[2].Width = 100;
            // this.dgvChecking.Columns[1].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvChecking.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[2].ReadOnly = true;
            dgvChecking.Columns[2].Visible = false;

            dgvChecking.Columns[3].Name = "Good Qty";
            this.dgvChecking.Columns[3].Width = 80;
            // this.dgvChecking.Columns[1].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvChecking.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvChecking.Columns[3].ReadOnly = true;
            //dgvChecking.Columns[2].Visible = false;


            //dgvChecking.Columns[2].Name = "Is Checked";
            //this.dgvChecking.Columns[2].Width = 100;
            //// this.dgvChecking.Columns[1].DefaultCellStyle.BackColor = Color.Beige;
            //this.dgvChecking.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvChecking.Columns[2].ReadOnly = true;

            //dgvChecking.Columns[3].Name = "Is Damaged";
            //this.dgvChecking.Columns[3].Width = 100;
            //// this.dgvChecking.Columns[1].DefaultCellStyle.BackColor = Color.Beige;
            //this.dgvChecking.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvChecking.Columns[3].ReadOnly = true;


            dgvChecking.Columns[4].Name = "Damage Qty";
            this.dgvChecking.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvChecking.Columns[4].Width = 100;
            this.dgvChecking.Columns[4].ReadOnly = true;


            dgvChecking.Columns[5].Name = "ProductId";
            this.dgvChecking.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[5].Visible = false;

            this.dgvChecking.Columns[1].Width = 400;

            dgvChecking.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvChecking.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }
        }
        private void LoadBarcode()
        {
            dgvBarcode.Rows.Clear();
            dgvBarcode.Columns.Clear();
            dgvBarcode.ColumnCount = 4;

            dgvBarcode.Columns[0].Name = "Items";
            dgvBarcode.Columns[1].Name = "Quantity";
            dgvBarcode.Columns[2].Name = "ProductId";
            dgvBarcode.Columns[3].Name = "ISBarCodeable";

            this.dgvBarcode.Columns[0].ReadOnly = true;
            this.dgvBarcode.Columns[1].ReadOnly = true;
            this.dgvBarcode.Columns[2].Visible = false;
            this.dgvBarcode.Columns[3].Visible = false;

            this.dgvBarcode.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvBarcode.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvBarcode.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvBarcode.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "ChkIsBarcoded";
            dgvCmb.HeaderText = "Action";
            dgvBarcode.Columns.Insert(0, dgvCmb);

            //Rectangle resolution = Screen.PrimaryScreen.Bounds;
            //int w = resolution.Width;
            //int h = resolution.Height;
            //if (w == 1024 && h == 768)
            //{
            //    this.dgvBarcode.Columns[0].Width = 100;

            //    this.dgvBarcode.Columns[2].Width = 100;
            //    this.dgvBarcode.Columns[1].Width = 300;

            //}
            //else
            //{
            this.dgvBarcode.Columns[0].Width = 100;
            this.dgvBarcode.Columns[1].Width = 400;
            this.dgvBarcode.Columns[2].Width = 100;
            // }
            dgvBarcode.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvBarcode.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }
        }

        private void LoadDamage()
        {
            dgvDamage.Rows.Clear();
            dgvDamage.ColumnCount = 4;
            //dgvDamage.RowCount = 24;

            dgvDamage.Columns[0].Name = "Sno";
            //   this.dgvDamage.Columns[0].Width = 70;
            //this.dgvChecking.Columns[0].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvDamage.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvDamage.Columns[0].ReadOnly = true;

            dgvDamage.Columns[1].Name = "Items";
            dgvDamage.Columns[2].Name = "Quantity";
            dgvDamage.Columns[3].Name = "ProductId";



            //  this.dgvDamage.Columns[0].DefaultCellStyle.BackColor = Color.Beige;

            this.dgvDamage.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvDamage.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


            //  this.dgvDamage.Columns[1].DefaultCellStyle.BackColor = Color.LightSkyBlue;

            this.dgvDamage.Columns[1].ReadOnly = true;
            this.dgvDamage.Columns[2].ReadOnly = true;
            this.dgvDamage.Columns[3].Visible = false;

            //this.dgvDamage.Columns[2].Width = 120;
            ////  this.dgvDamage.Columns[1].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            //this.dgvDamage.Columns[2].ReadOnly = true;

            //DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            //dgvCmb.ValueType = typeof(bool);
            //dgvCmb.Name = "ChkMovedToDamageStorage";
            //dgvCmb.HeaderText = "To Storage";
            //dgvDamage.Columns.Add(dgvCmb);
            //this.dgvDamage.Columns[2].Width = 95;

            //Rectangle resolution = Screen.PrimaryScreen.Bounds;
            //int w = resolution.Width;
            //int h = resolution.Height;
            //if (w == 1024 && h == 768)
            //{
            //    this.dgvDamage.Columns[0].Width = 50;

            //    this.dgvDamage.Columns[2].Width = 70;
            //    this.dgvDamage.Columns[1].Width = 400;
            //}
            //else
            //{
            this.dgvDamage.Columns[0].Width = 70;
            this.dgvDamage.Columns[1].Width = 600;
            this.dgvDamage.Columns[2].Width = 100;
            // }

            dgvDamage.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvDamage.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }
        }


        private void LoadFloorIncharge()
        {
            dgvFloorCheckIn.Rows.Clear();
            dgvFloorCheckIn.ColumnCount = 4;
            //dgvFloorCheckIn.RowCount = 23;

            dgvFloorCheckIn.Columns[0].Name = "Sno";

            this.dgvFloorCheckIn.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckIn.Columns[0].ReadOnly = true;

            dgvFloorCheckIn.Columns[1].Name = "Items";

            this.dgvFloorCheckIn.Columns[1].ReadOnly = true;

            dgvFloorCheckIn.Columns[2].Name = "Quantity";

            this.dgvFloorCheckIn.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckIn.Columns[2].ReadOnly = true;


            //dgvFloorCheckIn.Columns[2].Name = "To Rack";
            //this.dgvFloorCheckIn.Columns[2].Width = 100;
            ////  this.dgvFloorCheckIn.Columns[3].DefaultCellStyle.BackColor = Color.Beige;
            //this.dgvFloorCheckIn.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvFloorCheckIn.Columns[3].Name = "ProductId";
            this.dgvFloorCheckIn.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckIn.Columns[3].Visible = false;

            DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
            cmb.HeaderText = "Location";
            cmb.Name = "cmbLocation";
            //cmb.DataSource = ObjPurchaseReceiptBAL.GetLocation();
            //cmb.ValueMember = "LocationID";
            //cmb.DisplayMember = "LocationName";
            cmb.DefaultCellStyle.NullValue = "Select";
            dgvFloorCheckIn.Columns.Add(cmb);


            DataGridViewTextBoxColumn txt = new DataGridViewTextBoxColumn();
            txt.HeaderText = "Rack";
            txt.Name = "Rack";
            dgvFloorCheckIn.Columns.Add(txt);
            this.dgvFloorCheckIn.Columns[5].Width = 100;
            this.dgvFloorCheckIn.Columns[5].ReadOnly = true;


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;
            if (w == 1024 && h == 768)
            {
                this.dgvFloorCheckIn.Columns[0].Width = 80;

                this.dgvFloorCheckIn.Columns[2].Width = 100;
                this.dgvFloorCheckIn.Columns[4].Width = 150;
                this.dgvFloorCheckIn.Columns[1].Width = 200;

            }
            else
            {
                this.dgvFloorCheckIn.Columns[0].Width = 80;
                this.dgvFloorCheckIn.Columns[1].Width = 250;
                this.dgvFloorCheckIn.Columns[2].Width = 100;
                this.dgvFloorCheckIn.Columns[4].Width = 150;

            }

            dgvFloorCheckIn.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvFloorCheckIn.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
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
                vLabel1.Enabled = true;
            }
        }

       
      

       
        private void dgvBarcode_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dgvBarcode.Rows[e.RowIndex].Cells[e.ColumnIndex];
                bool achecked = Convert.ToBoolean(checkCell.Value);
                if (achecked)
                {
                    dgvBarcode.Rows[e.RowIndex].Cells[2].ReadOnly = false;
                }
                else
                {
                    dgvBarcode.Rows[e.RowIndex].Cells[2].ReadOnly = true;
                }
            }
        }

        private void dgvBarcode_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvBarcode.IsCurrentCellDirty && dgvBarcode.CurrentCell.ColumnIndex == 1)
            {
                dgvBarcode.EndEdit();
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
                //this.dgvSearch.Columns[1].Visible = true;
                //this.dgvSearch.Columns[2].Visible = true;
                //this.dgvSearch.Columns[3].Visible = true;
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
                //this.dgvSearch.Columns[1].Visible = false;
                //this.dgvSearch.Columns[2].Visible = false;
                //this.dgvSearch.Columns[3].Visible = false;
            }
        }

        private void dgvChecking_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void SearchPurchaseOrder()
        {

            dgvSearch.Rows.Clear();
            dgvSearch.Columns.Clear();
            dgvSearch.ColumnCount = 4;
            dgvSearch.RowCount = 16;

            dgvSearch.Columns[0].Name = "Order No";
            dgvSearch.Columns[1].Name = "Vendor Name";
            dgvSearch.Columns[2].Name = "Date";
            dgvSearch.Columns[3].Name = "Status ";



            this.dgvSearch.Columns[0].Width = 60;

            this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvSearch.Columns[1].Width = 120;


            this.dgvSearch.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvSearch.Columns[2].Width = 30;



            this.dgvSearch.Columns[3].Width = 30;


            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn c in dgvSearch.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }

            this.dgvSearch.Columns[1].Visible = false;
            this.dgvSearch.Columns[2].Visible = false;
            this.dgvSearch.Columns[3].Visible = false;
        }

        private void PurchaseReceipt_Load(object sender, EventArgs e)
        {

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (IsNewPO == true)
            {

                if (keyData == (Keys.Alt | Keys.Insert))
                {
                    if (dgvOrder.Rows.Count <= 0)
                    {
                        dgvOrder.Rows.Add();
                    }
                    else
                    {
                        try
                        {
                            int rowindex = dgvOrder.CurrentRow.Index;
                            int colindex = dgvOrder.CurrentCell.ColumnIndex;
                            //dgvOrder.Rows.Insert(rowindex, dgvOrder.Rows.Add(1));
                            dgvOrder.Rows.Insert(rowindex, 1);
                            getsino();
                        }
                        catch
                        { }
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

            if (cbVendor.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = dgvOrder;
                    if (dgvOrder.Rows.Count == 0)
                    {
                        dgvOrder.Rows.Add();
                    }
                    dgvOrder.CurrentCell = dgvOrder[1, 0];
                    return true;
                }
            }

            //}
            //if (dgvOrder.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        int col = dgvOrder.CurrentCell.ColumnIndex;
            //        int row = dgvOrder.CurrentCell.RowIndex;
            //        if (row == (dgvOrder.Rows.Count - 1))
            //        {
            //            if (col == 3)
            //            {
            //                if (IsNewPO == false)
            //                {
            //                    this.ActiveControl = txtinvoice;
            //                }
            //                else
            //                {
            //                    this.ActiveControl = txtRemarks;
            //                }
            //                return true;
            //            }
            //        }
            //    }
            //}

            //if (keyData == (Keys.Escape))
            //{
            //    pnsearch.Visible = false;
            //    dgvOrder.Focus();
            //    dgvOrder.CurrentCell = dgvOrder[3, dgvOrder.CurrentCell.RowIndex];
            //    dgvOrder.BeginEdit(true);
            //    return true;
            //}


            if (keyData == Keys.Escape)
            {
                if (pnsearch.Visible)
                {
                    pnsearch.Visible = false;
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[3, dgvOrder.CurrentCell.RowIndex];
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



            if (dateTimePicker1.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = dgvOrder;
                    if (dgvOrder.Rows.Count == 0)
                    {
                        dgvOrder.Rows.Add();
                    }
                    dgvOrder.CurrentCell = dgvOrder[1, 0];
                    return true;
                }
            }

            if (transactionclose.Focused)
            {
                if (keyData == (Keys.Tab))
                {

                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex + 2, dgvOrder.CurrentCell.RowIndex];
                    this.ActiveControl = dgvOrder;
                    //dgvOrder.BeginEdit(true);
                    pnsearch.Visible = false;
                    return true;
                }

            }
            if (txtinvoice.Focused)
            {
                //if (keyData == (Keys.Tab))
                //{

                //    this.ActiveControl = txtRemarks;
                //    pnsearch.Visible = false;
                //    return true;
                //}

            }
            //if (txtRemarks.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = butsave;
            //        return true;
            //    }
            //}

            ////if (butprint.Focused)
            ////{
            ////    if (keyData == (Keys.Tab))
            ////    {
            ////        this.ActiveControl = txtorder;
            ////        return true;
            ////    }
            ////}

            //if (txtsearch.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtordernos;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (txtreceipt.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtinvoiceno;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (txtremark.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = butsave;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (butprint.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txorderno;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (bttnsearch.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = ttorderno;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (ttreceiptno.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = ttinvoiceno;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            if (dateTimePicker9.Focused)
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
                    txtRemarks.Focus();
                    return true;
                }

            }

            if (butclear.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    textBox2.Focus();
                    return true;
                }

            }
            //if (butprint.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = txtordernoss;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (buttsearch.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = texorderno;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (textreceiptno.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = textinvoiceno;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (textremarks.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = butsave;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (butprint.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = textorderno;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (buttnsearch.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = textBxorderno;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (textBxreceiptno.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = textBxinvoiceno;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            //if (textBxremarks.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = butsave;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}

            //if (butclear.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = butprint;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}

            //if (butprint.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = textBorderno;
            //        pnsearch.Visible = false;
            //        return true;
            //    }

            //}
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tabname = tabControl1.SelectedTab.Name;


            if (tabname == "TabNew")
            {
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "", "", "");

            }
            if (tabname == "TabReceipt")
            {

                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsEntryCompleted", "IsCheckingCompleted");
                IsNewPO = false;
            }

            if (tabname == "TabInvoice")
            {
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsCheckingCompleted", "IsInvoiceCompleted");
                IsNewPO = false;
            }
            if (tabname == "TabChecking")
            {
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsEntryCompleted", "IsCheckingCompleted");
                IsNewPO = false;
            }
            if (tabname == "TabBarCode")
            {
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsCheckingCompleted", "IsBarcodedCompleted");
                IsNewPO = false;
            }
            if (tabname == "TabFloorCheckIn")
            {
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsCheckingCompleted", "IsFloorCheckedCompleted");
                IsNewPO = false;
            }
            if (tabname == "TabDamage")
            {
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsCheckingCompleted", "IsDamagedCompleted");
                IsNewPO = false;
            }
        }

        private void butprint_Click(object sender, EventArgs e)
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
        private void butsave_Click(object sender, EventArgs e)
        {
            string tabname = tabControl1.SelectedTab.Name;


            if (tabname == "TabNew")
            {
                DataTable dt = DataGridView2DataTable(dgvOrder);
                for (int i = 0; i < 3; i++)
                {
                    dt.Columns.RemoveAt(0);
                }
                dt.Columns.RemoveAt(3);
                //dt.Columns.RemoveAt(0);
                RemoveNullColumnFromDataTable(dt);

                //foreach (DataRow row in dt.Rows)
                //{
                //    if (row["Quantity"].ToString() == "0")
                //    {
                //        //MessageBox.Show("Quanityy");

                //        test = "1";
                //    }
                //    else
                //    {
                //        test = "0";
                //    }
                //}

                //if (test == "1")
                //{
                //    MessageBox.Show("Quantity Should not be Zero");
                //}
                //else
                //{
                    bool dtval = RemoveDuplicateRows(dt, "ProductId");

                    if (dtval)
                    {
                        if (Validation())
                        {
                            //if (getnewcheck())
                            //{
                            SaveNew();
                            GetsearchPurchasegoods();
                            //}
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please Remove Duplication Product");
                    }
               //}
            }
            else if (tabname == "TabReceipt")
            {
                if (!string.IsNullOrEmpty(txtreporderno.Text))
                {
                    Updatepurchasereceipt();
                }
                else
                {
                    MessageBox.Show("Please Select Receipt");
                }


            }
            else if (tabname == "TabInvoice")
            {
                if (Validation1())
                {
                    SaveInvoice();
                }
            }
            else if (tabname == "TabChecking")
            {
                if (Validation2())
                {
                    if (getvalcheck())
                    {
                        SaveChecking();
                    }
                    else
                    {
                        string message = "Good Qty and Damage Qty is not matching with Recieved Qty";
                        MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                    }
                }
            }
            else if (tabname == "TabBarCode")
            {
                if (Validation3())
                {
                    SaveBarcode();
                }
            }
            else if (tabname == "TabFloorCheckIn")
            {
                if (Validation4())
                {
                    SaveFloorCheckIn();
                }
            }
            else if (tabname == "TabDamage")
            {
                SaveDamage();
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

        public void SaveNew()
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(txtOrderNo.Text))
            {
                ObjPurchaseReceiptBAL.isnew = 0;
            }
            else
            {
                ObjPurchaseReceiptBAL.isnew = 1;
            }
            ObjPurchaseReceiptBAL.OrderNo = txtOrderNo.Text;
            DateTime date = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
            ObjPurchaseReceiptBAL.OrderDate = date;
            //DateTime date1 = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);
            //ObjPurchaseReceiptBAL.ExpectedDelieveryDate = date1;
            ObjPurchaseReceiptBAL.Supplierid = Convert.ToString(cbVendor.SelectedValue);
            ObjPurchaseReceiptBAL.Remarks = Convert.ToString(txtRemarks.Text);
            ObjPurchaseReceiptBAL.status = "Approve";
            ObjPurchaseReceiptBAL.Enteredby = Program.userid;
            dt = DataGridView2DataTable(dgvOrder);
            bool contains1 = dt.AsEnumerable()
              .Any(row => "Partial" == row.Field<String>("Status"));

            if (contains1 == true)
            {
                ObjPurchaseReceiptBAL.partial = "Partial";
            }
            else
            {
                ObjPurchaseReceiptBAL.partial = "Full";
            }
            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            dt.Columns.RemoveAt(3);
            RemoveNullColumnFromDataTable(dt);
            dt.Columns["Ordered Qty"].ColumnName = "OrderQuantity";
            dt.Columns["Received Qty"].ColumnName = "RecievedQuantity";





            string output = ObjPurchaseReceiptBAL.SavePurchaseReceipt(ObjPurchaseReceiptBAL, dt);
            if (!string.IsNullOrEmpty(output))
            {
                DataTable dtval = ObjPurchaseReceiptBAL.getstatus(txtOrderNo.Text);
                if (dtval.Rows.Count > 0)
                {
                    bool contains3 = dtval.AsEnumerable()
                 .Any(row => "Partial" == row.Field<String>("Status"));

                    if (contains3 == true)
                    {
                        int s = ObjPurchaseReceiptBAL.updatestatus(txtOrderNo.Text);
                    }
                }

                clear();
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "", "", "");
            }
        }

        public void Updatepurchasereceipt()
        {

            for (int i = 0; i < dgvrepreceipt.Rows.Count; i++)
            {
                string productid = Convert.ToString(dgvrepreceipt.Rows[i].Cells[3].Value);
                string Quantity = Convert.ToString(dgvrepreceipt.Rows[i].Cells[2].Value);
                string user = Convert.ToString(Program.userid);

                int output = ObjPurchaseReceiptBAL.Updatepurchasereceipt(txtrepreceiptid.Text, user, productid, Txtrepremarks.Text, Quantity);



            }


            if (!string.IsNullOrEmpty(txtrepreceiptid.Text))
            {
                DataTable dtval = ObjPurchaseReceiptBAL.getstatus(txtreporderno.Text);
                if (dtval.Rows.Count > 0)
                {
                    bool contains3 = dtval.AsEnumerable()
                 .Any(row => "Partial" == row.Field<String>("Status"));

                    if (contains3 == true)
                    {
                        int s = ObjPurchaseReceiptBAL.updatestatus(txtreporderno.Text);
                    }
                    else
                    {
                        int s = ObjPurchaseReceiptBAL.updatestatusful(txtreporderno.Text);
                    }
                }
            }


            //if (output == 1)
            //{
            clear6();
            search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsEntryCompleted", "IsCheckingCompleted");
            //}
        }

        public void SaveInvoice()
        {
            DataTable dt = new DataTable();

            if (string.IsNullOrEmpty(lblinvoiceTotals.Text))
            {
                lblinvoiceTotals.Text = "0.0";
            }

            if (string.IsNullOrEmpty(txttax1.Text))
            {
                txttax1.Text = "0.0";
            }
            if (string.IsNullOrEmpty(txttax2.Text))
            {
                txttax2.Text = "0.0";
            }

            if (string.IsNullOrEmpty(txtInvoiceDis.Text))
            {
                txtInvoiceDis.Text = "0.0";
            }

            if (string.IsNullOrEmpty(txtinvoiceRoundOff.Text))
            {
                txtinvoiceRoundOff.Text = "0.0";
            }
            if (string.IsNullOrEmpty(txtinvioceOtherCharges.Text))
            {
                txtinvioceOtherCharges.Text = "0.0";
            }

            ObjPurchaseReceiptBAL.ReceiptNo = Convert.ToString(txtInvoiceReceiptNo.Text);
            ObjPurchaseReceiptBAL.InvoiceNo = TxtInInvoiceNo.Text;
            DateTime date = new DateTime(dtpInInvoicedate.Value.Year, dtpInInvoicedate.Value.Month, dtpInInvoicedate.Value.Day);
            ObjPurchaseReceiptBAL.InvoiceDate = date.ToString();
            ObjPurchaseReceiptBAL.Total = lblinvoiceTotals.Text;
            ObjPurchaseReceiptBAL.Tax1 = txttax1.Text;
            ObjPurchaseReceiptBAL.Tax2 = txttax2.Text;
            ObjPurchaseReceiptBAL.Discount = txtInvoiceDis.Text;
            ObjPurchaseReceiptBAL.RoundOffSimbol = CmbInvROS.Text;
            ObjPurchaseReceiptBAL.RoundOff = txtinvoiceRoundOff.Text;
            ObjPurchaseReceiptBAL.OtherChargesSimbol = CmbInvOCS.Text;
            ObjPurchaseReceiptBAL.OtherCharges = txtinvioceOtherCharges.Text;
            ObjPurchaseReceiptBAL.NetAmount = lblinvoiceNet.Text;
            ObjPurchaseReceiptBAL.Invoiceby = Program.userid;
            dt = DataGridView2DataTable(dgvinvoice);
            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            dt.Columns.RemoveAt(dt.Columns.Count - 1);

            RemoveNullColumnFromDataTable(dt);

            int output = ObjPurchaseReceiptBAL.SavePurchaseInvoice(ObjPurchaseReceiptBAL, dt);
            if (output == 1)
            {
                clear1();
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsCheckingCompleted", "IsInvoiceCompleted");
            }

        }

        public void SaveChecking()
        {
            DataTable dt = new DataTable();
            ObjPurchaseReceiptBAL.ReceiptNo = Convert.ToString(txtreceipt.Text);
            ObjPurchaseReceiptBAL.Checkedby = Program.userid;
            dt = DataGridView2DataTable(dgvChecking);
            for (int i = 0; i < 2; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            dt.Columns["Quantity"].ColumnName = "ReceivedQuantity";
            dt.Columns["Good Qty"].ColumnName = "GoodQuantity";
            dt.Columns["Damage Qty"].ColumnName = "DamageQuantity";
            RemoveNullColumnFromDataTable(dt);

          

            int output = ObjPurchaseReceiptBAL.SavePurchaseChecking(ObjPurchaseReceiptBAL, dt);
            if (output == 1)
            {
                clear2();
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsEntryCompleted", "IsCheckingCompleted");
            }
        }

        public void SaveBarcode()
        {
            DataTable dt = new DataTable();

            //for(int i=0;i<dgvBarcode.Rows.Count;i++)
            //{
            ObjPurchaseReceiptBAL.ProductId = Convert.ToString(dgvBarcode.Rows[0].Cells[3].Value);
            ObjPurchaseReceiptBAL.ReceiptNo = Convert.ToString(ttreceiptno.Text);
            ObjPurchaseReceiptBAL.Barcodedby = Program.userid;

            int output = ObjPurchaseReceiptBAL.SavePurchaseBarcode(ObjPurchaseReceiptBAL);
            //}
            if (output == 1)
            {
                clear3();
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsCheckingCompleted", "IsBarcodedCompleted");
            }
        }

        public void SaveFloorCheckIn()
        {
            DataTable dt = new DataTable();
            ObjPurchaseReceiptBAL.ReceiptNo = Convert.ToString(textreceiptno.Text);
            ObjPurchaseReceiptBAL.FloorCheckinby = Program.userid;
            dt = DataGridView2DataTable(dgvFloorCheckIn);
            for (int i = 0; i < 2; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            RemoveNullColumnFromDataTable(dt);

            int output = ObjPurchaseReceiptBAL.SavePurchaseFloorCheckin(ObjPurchaseReceiptBAL, dt);
            if (output == 1)
            {
                itemdetailsval("");
                clear4();
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsBarcodedCompleted", "IsFloorCheckedCompleted");
                //search("OrderNumber", "", "OrderDate", "", "Name", "", role, UserId, "EnteredBy", "IsCheckingCompleted", "IsFloorCheckedCompleted");
            }
        }

        public void SaveDamage()
        {
            DataTable dt = new DataTable();
            ObjPurchaseReceiptBAL.ReceiptNo = Convert.ToString(textBxreceiptno.Text);
            ObjPurchaseReceiptBAL.Damagedby = Program.userid;
            dt = DataGridView2DataTable(dgvDamage);
            for (int i = 0; i < 2; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            RemoveNullColumnFromDataTable(dt);

            int output = ObjPurchaseReceiptBAL.SavePurchaseDamage(ObjPurchaseReceiptBAL);
            if (output == 1)
            {
                clear5();
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId, "EnteredBy", "IsFloorCheckedCompleted", "IsDamagedCompleted");
            }
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
                if (dt.Columns.Count>=3)
                {
                if(string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][2])))
                {
                    dt.Rows[i][2] = 0;
                }
                }

                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][1])))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }

        private void butsaveaspending_Click(object sender, EventArgs e)
        {
            string tabname = tabControl1.SelectedTab.Name;
            if (tabname == "TabNew")
            {
                Validation();
            }
            else if (tabname == "TabInvoice")
            {
                Validation1();
            }
            else if (tabname == "TabChecking")
            {
                Validation2();
            }
            else if (tabname == "TabBarCode")
            {
                Validation3();
            }
            else if (tabname == "TabFloorCheckIn")
            {
                Validation4();
            }
        }

        private void butclear_Click(object sender, EventArgs e)
        {
            string tabname = tabControl1.SelectedTab.Name;
            if (tabname == "TabNew")
            {
                clear();
            }
            else if (tabname == "TabReceipt")
            {
                clear6();
            }
            else if (tabname == "TabInvoice")
            {
                clear1();
            }
            else if (tabname == "TabChecking")
            {
                clear2();
            }
            else if (tabname == "TabBarCode")
            {
                clear3();
            }
            else if (tabname == "TabFloorCheckIn")
            {
                clear4();
            }
            else if (tabname == "TabDamage")
            {
                clear5();
            }


        }

        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;


            int val = 0;

            foreach (DataGridViewRow row in dgvOrder.Rows)
            {
                if (Convert.ToString(row.Cells["Received Qty"].Value)!=".")
                {
                double Received = Convert.ToDouble(row.Cells["Received Qty"].Value);
                if (Received != 0)
                {
                    val = val + 1;
                }
                }
                else
                {
                    val = 0;
                }
               
            }

            if (val==0)
            {
                i++;
                message = message + "* Please Enter Correct Quantity ." + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;

            }

            //if (string.IsNullOrEmpty(txtorder.Text))
            //{
            //    i++;
            //    message = message + "*Please Search Order No" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = butsearch;
            //}
            if (IsNewPO == false)
            {
                if (string.IsNullOrEmpty(txtOrderNo.Text))
                {
                    i++;
                    message = message + "* Please Enter Order No." + "\n";
                    if (i == 1)
                        this.ActiveControl = txtOrderNo;
                }

                //if (!string.IsNullOrEmpty(message))
                //{
                //    MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                //    status = false;
                //}
            }
            else
            {
                if (cbVendor.Text == "--Select--")
                {
                    i++;
                    message = message + "* Should select vendor" + "\n";
                    if (i == 1)
                        this.ActiveControl = cbVendor;
                }

                //if (!string.IsNullOrEmpty(txtCity.Text))
                //{
                //    i++;
                //    message = message + "* City Should Not Be Empty" + "\n";
                //    if (i == 1)
                //        this.ActiveControl = cbVendor;
                //}
              


                foreach (DataGridViewRow row in dgvOrder.Rows)
                {
                    string Items = Convert.ToString(row.Cells["Items"].Value);
                    string Received = Convert.ToString(row.Cells["Received Qty"].Value);

                    if (string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received)))
                    {

                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Items))
                        {
                            i++;
                            message = message + "* Please Enter Items." + "\n";
                            if (i == 1)
                                this.ActiveControl = dgvOrder;
                            break;
                        }

                        if ((string.IsNullOrEmpty(Received)) || Received == ".")
                        {
                            i++;
                            message = message + "* Please Enter  Quantity." + "\n";
                            if (i == 1)
                                this.ActiveControl = dgvOrder;
                            break;
                        }
                    }
                }


                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                    status = false;
                }



                //if (cbxStatus.SelectedIndex == 0)
                //{
                //    i++;
                //    message += "* Choose Status." + "\n";
                //    if (i == 1)
                //    {
                //        this.ActiveControl = cbxStatus;
                //    }
                //}

                //if (string.IsNullOrEmpty(txtRemarks.Text))
                //{
                //    i++;
                //    message = message + "*Please Enter Remarks." + "\n";
                //    if (i == 1)
                //        this.ActiveControl = txtRemarks;
                //}





            }

            clearGrid(dgvOrder);

            if (IsNewPO == false)
            {
                foreach (DataGridViewRow row in dgvOrder.Rows)
                {
                    string Items = Convert.ToString(row.Cells["Items"].Value);
                    string Received = Convert.ToString(row.Cells["Received Qty"].Value);

                    if (string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received)))
                    {
                        i++;
                        message = message + "*Please Enter Items and Quantity." + "\n";
                        if (i == 1)
                            this.ActiveControl = dgvOrder;
                        break;
                    }
                }

                if (dgvOrder.Rows.Count > 0)
                {
                    i++;
                    string Items = Convert.ToString(dgvOrder.Rows[0].Cells["Received Qty"].Value);
                    string Received = Convert.ToString(dgvOrder.Rows[0].Cells["Items"].Value);
                    if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                    {
                        message = message + "*.Please Select Product" + "\n";
                    }

                    if (i == 1)
                        this.ActiveControl = dgvOrder;
                }
                else if (dgvOrder.Rows.Count == 0)
                {
                    i++;
                    message = message + "*.Please Select Product" + "\n";
                    if (i == 1)
                        this.ActiveControl = dgvOrder;
                }

                bool sas = false;

                for (int k = 0; k < dgvOrder.RowCount; k++)
                {
                    string Items = Convert.ToString(dgvOrder.Rows[k].Cells["Received Qty"].Value);
                    string Received = Convert.ToString(dgvOrder.Rows[k].Cells["Items"].Value);

                    if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))))
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
            }
            return status;
        }



        private void clear()
        {
            //txtorder.Clear();
            txtOrderNo.Clear();
            cbxStatus.SelectedIndex = 0;
            cbVendor.SelectedIndex = 0;
            txtreceiptno.Clear();
            txtinvoice.Clear();
            txtRemarks.Clear();
            txtCity.Clear();
            dgvOrder.Rows.Clear();
            //dgvOrder.Rows.Add(1);
            PnlVendor.Visible = false;
            panel2.Enabled = false;
            pnsearch.Visible = false;
            lbltotalquantity.Text = "0";
            dgvOrder.AllowUserToAddRows = false;
        }

        private void clear6()
        {
            //txtorder.Clear();
            txtreporderno.Clear();
            cmrepstatus.SelectedIndex = 0;

            txtrepreceiptid.Clear();

            Txtrepremarks.Clear();
            dgvrepreceipt.Rows.Clear();


            lblreptotal.Text = "0";
        }
        private bool Validation1()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(TxtInvoiceOrdNo.Text))
            {
                i++;
                message = message + "*Please Search Order No" + "\n";
                if (i == 1)
                    this.ActiveControl = TxtInvoiceOrdNo;
            }

            if (string.IsNullOrEmpty(TxtInInvoiceNo.Text))
            {
                i++;
                message = message + "*Please Enter Invoice No" + "\n";
                if (i == 1)
                    this.ActiveControl = TxtInInvoiceNo;
            }
            if (RoundOFF.Text==".")
            {
                i++;
                message = message + "*Please Enter Correct Round Off" + "\n";
                if (i == 1)
                    this.ActiveControl = RoundOFF;
            }
            if (txtdiscount.Text == ".")
            {
                i++;
                message = message + "*Please Enter Correct Discound" + "\n";
                if (i == 1)
                    this.ActiveControl = txtdiscount;
            }
            if (textBox26.Text == ".")
            {
                i++;
                message = message + "*Please Enter Correct Other Changes" + "\n";
                if (i == 1)
                    this.ActiveControl = textBox26;
            }
           

            foreach (DataGridViewRow row in dgvinvoice.Rows)
            {
                string price = Convert.ToString(row.Cells["Price"].Value);
                string Tax = Convert.ToString(row.Cells["Sum"].Value);

                if (string.IsNullOrEmpty(price) || (string.IsNullOrEmpty(Tax)))
                {
                    i++;
                    message = message + "*Please Enter Price and Tax" + "\n";
                    if (i == 1)
                        this.ActiveControl = dgvinvoice;
                    break;
                }

                if (price=="." )
                {
                    i++;
                    message = message + "*Please Enter Correct Price" + "\n";
                    if (i == 1)
                        this.ActiveControl = dgvinvoice;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }
        private void clear1()
        {
            TxtInvoiceOrdNo.Clear();
            txtInvoiceReceiptNo.Clear();
            cmbInvoiceStatus.SelectedIndex = 0;
            var today = DateTime.Today;
            dateTimePicker6.Value = today;

            TxtInInvoiceNo.Clear();
            dtpInInvoicedate.Value = today;
            txtInvoiceRemarks.Clear();

            lblinvoiceTotals.Text = "0.00";
            txttax1.Text = "0.00";
            txttax2.Text = "0.00";
            txtInvoiceDis.Text = "0.00";
            CmbInvROS.SelectedIndex = 0;
            txtinvoiceRoundOff.Text = "0.00";
            CmbInvOCS.SelectedIndex = 0;
            txtinvioceOtherCharges.Text = "0.00";
            lblinvoiceNet.Text = "0.00";

            dgvinvoice.Rows.Clear();
            dgvinvoice.Rows.Add(1);

        }
        private bool Validation2()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(txtordernos.Text))
            {
                i++;
                message = message + "*Please Search Order No" + "\n";
                if (i == 1)
                    this.ActiveControl = txtordernos;
            }

            //if (CellCheckingQtyValidation)
            //{
            //    i++;
            //    message = message + "*Good Qty and Damage Qty is not matching with Recieved Qty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtreceipt;
            //}

            foreach (DataGridViewRow row in dgvChecking.Rows)
            {
                string GoodQuantity = Convert.ToString(row.Cells["Good Qty"].Value);
                string DamageQuantity = Convert.ToString(row.Cells["Damage Qty"].Value);

                if (string.IsNullOrEmpty(GoodQuantity) && (string.IsNullOrEmpty(DamageQuantity)))
                {
                    i++;
                    message = message + "*Please Enter Good Qty and Damage Qty" + "\n";
                    if (i == 1)
                        this.ActiveControl = dgvChecking;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }
        private void clear2()
        {
            txtordernos.Clear();
            comstatuss.SelectedIndex = 0;
            var today = DateTime.Today;
            dateTimePicker2.Value = today;
            txtreceipt.Clear();
            txorderno.Clear();
            txtremark.Clear();
            dgvChecking.Rows.Clear();
            dgvChecking.Rows.Add(1);
            GoodQty = 0.0;
            DamageQty = 0.0;
            totQty = 0.0;
        }
        private bool Validation3()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(ttreceiptno.Text))
            {
                i++;
                message = message + "*Please Search Order No" + "\n";
                if (i == 1)
                    this.ActiveControl = ttreceiptno;
            }

            bool err = false;
            foreach (DataGridViewRow row in dgvBarcode.Rows)
            {
                if (Convert.ToBoolean(((DataGridViewCheckBoxCell)row.Cells["ChkIsBarcoded"]).Value) == false)
                {
                    err = true;
                }
            }

            if (err)
            {
                i++;
                message = message + "*Please check all items shown in Green Colour" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvBarcode;
            }

            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }
        private void clear3()
        {
            ttorderno.Clear();
            combostatus.SelectedIndex = 0;
            var date = DateTime.Today;
            dateTimePicker3.Value = date;
            ttreceiptno.Clear();
            dgvBarcode.Rows.Clear();
            ttremarks.Clear();
        }
        private bool Validation4()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(textreceiptno.Text))
            {
                i++;
                message = message + "*Please Search Order No" + "\n";
                if (i == 1)
                    this.ActiveControl = textreceiptno;
            }

            foreach (DataGridViewRow row in dgvFloorCheckIn.Rows)
            {
                string price = Convert.ToString(row.Cells["Rack"].Value);

                if (string.IsNullOrEmpty(price))
                {
                    i++;
                    message = message + "*Please Select Location" + "\n";
                    if (i == 1)
                        this.ActiveControl = dgvFloorCheckIn;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }
        private void clear4()
        {
            texorderno.Clear();
            textBxorderno.Clear();
            comboBoxstatus.SelectedIndex = 0;
            textreceiptno.Clear();
            textremarks.Clear();
            dgvFloorCheckIn.Rows.Clear();
            var date = DateTime.Today;
            dateTimePicker4.Value = date;
        }

        private void clear5()
        {
            textBxorderno.Clear();
            combobxstatus.SelectedIndex = 0;
            textBxreceiptno.Clear();
            textBxremarks.Clear();
            dgvDamage.Rows.Clear();
            var date = DateTime.Today;
            dateTimePicker5.Value = date;
        }

        private void clearGrid(DataGridView view)
        {
            try
            {
                for (int row = 0; row < view.Rows.Count; ++row)
                {
                    bool isEmpty = true;
                    for (int col = 0; col < view.Columns.Count; ++col)
                    {
                        object value = view.Rows[row].Cells[col].Value;
                        if (value != null && value.ToString().Length > 0)
                        {
                            isEmpty = false;
                            break;
                        }
                    }
                    if (isEmpty)
                    {
                        // deincrement (after the call) since we are removing the row
                        view.Rows.RemoveAt(row--);
                    }
                }
            }
            catch
            {

            }
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            if (!PnlVendor.Visible)
            {
                PnlVendor.Visible = true;
            }
            IsNewPO = true;
            InsDel = 2;
            //clear();
            dgvOrder.Rows.Clear();
            dgvOrder.Rows.Add(1);
            panel2.Enabled = true;
            txtreceiptno.Enabled = false;
            txtOrderNo.Enabled = false;
            cbxStatus.Enabled = false;
            dateTimePicker1.Enabled = false;
            cbVendor.Focus();
            dgvOrder.Rows.Clear();
            dgvOrder.AllowUserToAddRows = true;
            dgvOrder.Columns[3].ReadOnly = true;
        }

        /* Left Side Content */

        private void lblAll_Click(object sender, EventArgs e)
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

        private void txtsearch1_Click(object sender, EventArgs e)
        {
           
        }

        private void txtsearch2_Click(object sender, EventArgs e)
        {

            
        }

        private void txtsearch3_Click(object sender, EventArgs e)
        {
            
        }

        private void SearchFrmDate_ValueChanged(object sender, EventArgs e)
        {
            
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

        private void cbxSearchOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string ordno = Convert.ToString(cbxSearchOrderNo.SelectedValue);

            //if (ordno == "RequestedBy")

           
        }

        private void cbxSearchOrderDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cbxVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cbVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindSupplierCity();
        }

        public void bindSupplierCity()
        {
            int SupplierId = 0;
            if (cbVendor.SelectedIndex != 0)
            {
                SupplierId = Convert.ToInt32(cbVendor.SelectedValue);
                DataTable dtCity = OblPurchaseOrderBAL.GetSuppliers(SupplierId);
                if (dtCity.Rows.Count > 0)
                {
                    txtCity.Text = Convert.ToString(dtCity.Rows[1]["City"]);
                }
                else
                {
                    txtCity.Clear();
                }
            }
            else
            {
                txtCity.Clear();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            GetsearchPurchasegoods();
        }

        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId, string UserBy, string Complete, string ChkComplete)
        {
            ////try
            ////{
            //string tabname = tabControl1.SelectedTab.Name;
            //DataTable dts;
            //if (tabname == "TabNew")
            //{
               
            //}
            //else if (tabname == "TabReceipt")
            //{
            //    dts = ObjPurchaseReceiptBAL.SearchReceipt(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role, UserId, UserBy);
            //}
            //else if (tabname == "TabInvoice")
            //{
            //    dts = ObjPurchaseReceiptBAL.SearchPurchaseReceiptval(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role, UserId, UserBy, Complete, ChkComplete);
            //}
            //else if (tabname == "TabChecking" || tabname == "TabBarCode")
            //{

            //    dts = ObjPurchaseReceiptBAL.SearchPurchaseReceipt(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role, UserId, UserBy, Complete, ChkComplete);
            //}
            //else
            //{
            //    dts = ObjPurchaseReceiptBAL.SearchPurchaseReceiptcheck(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role, UserId, UserBy, Complete, ChkComplete);
            //}
            //dgvSearch.Columns.Clear();
            //dgvSearch.DataSource = dts;

            //lblItemCount.Text = Convert.ToString(dts.Rows.Count);


            //if (tabname != "TabNew")
            //{
            //    if (tabname == "TabReceipt")
            //    {
            //        dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //        dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //        dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            //        dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //    }

            //    else
            //    {

            //        if (dgvSearch.Columns.Count > 0)
            //        {
            //            dgvSearch.Columns["PurchaseId"].Visible = false;
            //            dgvSearch.Columns["OrderDate"].Visible = false;

            //            // dgvSearch.Columns["OrderNumber"].Visible = false;
            //            if (tabname == "TabNew")
            //            {
            //                dgvSearch.Columns["OrderNumber"].Visible = true;
            //                dgvSearch.Columns["OrderNumber"].ReadOnly = true;
            //                //dgvSearch.Columns["ExpectedDeliveryDate"].Visible = false;
            //            }
            //            else
            //            {
            //                dgvSearch.Columns["PurchaseOrderNo"].Visible = false;
            //                dgvSearch.Columns["OrderNumber"].Visible = true;
            //            }
            //            dgvSearch.Columns["VendorId"].Visible = false;
            //            dgvSearch.Columns["Vendor"].Visible = false;
            //            dgvSearch.Columns["Status"].Visible = false;
            //            dgvSearch.Columns["Remarks"].Visible = false;
            //            dgvSearch.Columns["ApprovedBy"].Visible = false;
            //            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            //            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //        }
            //    }
            //}
            //}
            ////catch(Exception e)
            ////{

            ////}
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string tabname = tabControl1.SelectedTab.Name;
                string id = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);

                if (tabname == "TabNew")
                {
                    string status = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[1].Value);
                    GetPurchaseOrderByOrderNo(id, status);
                    btnReject.Enabled = true;
                }
                else if (tabname == "TabReceipt")
                {
                    string ids = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["OrderNumber"].Value);
                    GetPurchaseReceipt(ids);
                }
                else if (tabname == "TabInvoice")
                {

                    string ids = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["OrderNumber"].Value);
                    GetPurchaseReceiptInvoiceByOrderNo(ids);
                }
                else if (tabname == "TabChecking")
                {
                    string ids = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["OrderNumber"].Value);
                    GetPurchaseReceiptChkByOrderNo(ids);
                    for (int i = 0; i < dgvChecking.Rows.Count; i++)
                    {
                        dgvChecking.Rows[i].Cells[3].ReadOnly = false;
                    }
                }
                else if (tabname == "TabBarCode")
                {
                    string ids = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["OrderNumber"].Value);
                    GetPRBarcodeByOrderNo(ids);
                }
                else if (tabname == "TabFloorCheckIn")
                {
                    string ids = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["OrderNumber"].Value);
                    GetPRFloorcheckinByOrderNo(ids);
                }
                else if (tabname == "TabDamage")
                {
                    string ids = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["OrderNumber"].Value);
                    GetPRDamageByOrderNo(ids);
                }
                IsNewPO = false;
                PnlVendor.Visible = false;
            }
        }

        public void GetPurchaseOrderByOrderNo(string s, string s1)
        {
            DataSet ds = OblPurchaseOrderBAL.GetPurchaseOrderByOrderNo(s, s1);
            if (ds.Tables[0].Rows.Count > 0)
            {
                dateTimePicker1.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderDate"]);
                //dateTimePicker1.Text = Convert.ToString(ds.Tables[0].Rows[0]["ExpectedDeliveryDate"]);
                cbVendor.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"]);
                txtOrderNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderNumber"]);
                cbxStatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                txtRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            //if (s1 == "New")
            //{
            if (ds.Tables[1].Rows.Count > 0)
            {
                dgvOrder.AllowUserToAddRows = false;
                dgvOrder.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {

                    dgvOrder.Rows.Add();
                    dgvOrder.Rows[i].Cells[0].Value = i + 1;
                    dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["SalesPrice"]);
                    dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    //dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["ReceivedQuantity"]);
                    dgvOrder.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    //if (Convert.ToInt32(ds.Tables[1].Rows[i]["Balance Quantity"]) <= 0)
                    //{
                    //    dgvOrder.Rows[i].Cells[3].ReadOnly = true;
                    //}
                }

                dgvOrder.Columns[5].ReadOnly = false;

                // dgvOrder.CurrentCell = dgvOrder[2, 0];
                panel2.Enabled = false;
                pnsearch.Visible = false;
                dgvOrder.Focus();
                dgvOrder.CurrentCell = dgvOrder[4, 0];
                //dgvOrder.BeginEdit(true);
            }
            else
            {
                dgvOrder.Rows.Clear();
                panel2.Enabled = true;
            }
            //}
            //else
            //{
            //    dtpartial = new DataTable();
            //    dtpartial = ds.Tables[1];
            //    dgvOrder.Rows.Clear();
            //    dgvOrder.Rows.Add();
            //}
        }


        public void GetPurchaseReceiptInvoiceByOrderNo(string s)
        {
            DataSet ds = ObjPurchaseReceiptBAL.GetPurchaseReceiptByOrderNo(s);
            txttax1.Text = string.Empty;
            txttax2.Text = string.Empty;
            txtInvoiceDis.Text = string.Empty;
            txtinvoiceRoundOff.Text = string.Empty;
            txtinvioceOtherCharges.Text = string.Empty;
            lblinvoiceNet.Text = string.Empty;
            if (ds.Tables[0].Rows.Count > 0)
            {
                dateTimePicker6.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderDate"]);
                //dateTimePicker1.Text = Convert.ToString(ds.Tables[0].Rows[0]["ExpectedDeliveryDate"]);
                //cbVendor.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"]);
                TxtInvoiceOrdNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["POOrdno"]);
                txtInvoiceReceiptNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["PROrdno"]);
                cmbInvoiceStatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                txtInvoiceRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);

                panel4.Enabled = false;
            }
            else
            {
                panel4.Enabled = false;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                dgvinvoice.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvinvoice.Rows.Add();
                    dgvinvoice.Rows[i].Cells[0].Value = i + 1;
                    dgvinvoice.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvinvoice.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["ReceivedQuantity"]);
                    dgvinvoice.Rows[i].Cells[7].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    string uom = ObjPurchaseReceiptBAL.GetUOMByProductId(Convert.ToInt32(ds.Tables[1].Rows[i]["Productid"]));
                    dgvinvoice.Rows[i].Cells[2].Value = uom;
                }
                panel2.Enabled = false;
                dgvinvoice.CurrentCell = dgvinvoice[4, 0];
                dgvinvoice.BeginEdit(true);
            }
            else
            {
                dgvinvoice.Rows.Clear();
                panel2.Enabled = true;
            }
        }

        public void GetPurchaseReceiptChkByOrderNo(string s)
        {
            DataSet ds = ObjPurchaseReceiptBAL.GetPurchaseReceiptByOrderNo(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                dateTimePicker2.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderDate"]);
                //dateTimePicker1.Text = Convert.ToString(ds.Tables[0].Rows[0]["ExpectedDeliveryDate"]);
                //cbVendor.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"]);
                txtordernos.Text = Convert.ToString(ds.Tables[0].Rows[0]["POOrdno"]);
                txtreceipt.Text = Convert.ToString(ds.Tables[0].Rows[0]["PROrdno"]);
                comstatuss.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                txtremark.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
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
                    dgvChecking.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["ReceivedQuantity"]);
                    dgvChecking.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                }
                panel2.Enabled = false;
                dgvChecking.Focus();
                dgvChecking.CurrentCell = dgvChecking[3, 0];
                dgvChecking.BeginEdit(true);
            }
            else
            {
                dgvChecking.Rows.Clear();
                panel2.Enabled = true;
            }

        }

        public void GetPurchaseReceipt(string s)
        {
            DataSet ds = ObjPurchaseReceiptBAL.GetPurchaseReceipt(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                dateTimePicker7.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderDate"]);
                //dateTimePicker1.Text = Convert.ToString(ds.Tables[0].Rows[0]["ExpectedDeliveryDate"]);
                //cbVendor.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"]);
                txtreporderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["POOrdno"]);
                txtrepreceiptid.Text = Convert.ToString(ds.Tables[0].Rows[0]["PROrdno"]);
                cmrepstatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                Txtrepremarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                if (Convert.ToInt32(ds.Tables[0].Rows[0]["Checked"]) == 1)
                {
                    dgvrepreceipt.ReadOnly = true;
                }
                else
                {
                    dgvrepreceipt.ReadOnly = false;
                }
                //panel2.Enabled = false;
            }
            else
            {
                //panel2.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                dgvrepreceipt.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvrepreceipt.Rows.Add();
                    dgvrepreceipt.Rows[i].Cells[0].Value = i + 1;
                    dgvrepreceipt.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvrepreceipt.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["ReceivedQuantity"]);
                    dgvrepreceipt.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

                }
                //panel2.Enabled = false;
                dgvrepreceipt.CurrentCell = dgvrepreceipt[2, 0];
                dgvrepreceipt.BeginEdit(true);
            }
            else
            {
                dgvChecking.Rows.Clear();
                //panel2.Enabled = true;
            }

        }

        public void GetPRBarcodeByOrderNo(string s)
        {
            DataSet ds = ObjPurchaseReceiptBAL.GetPurchaseReceiptByOrderNo(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                dateTimePicker3.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderDate"]);
                //dateTimePicker1.Text = Convert.ToString(ds.Tables[0].Rows[0]["ExpectedDeliveryDate"]);
                //cbVendor.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"]);
                ttorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["POOrdno"]);
                ttreceiptno.Text = Convert.ToString(ds.Tables[0].Rows[0]["PROrdno"]);
                combostatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                ttremarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                dgvBarcode.Rows.Clear();

                //if (role == "Admin")
                //{
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvBarcode.Rows.Add();
                    dgvBarcode.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvBarcode.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["GoodQuantity"]);
                    dgvBarcode.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    dgvBarcode.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[i]["ISBarCodeable"]);
                    if (Convert.ToString(ds.Tables[1].Rows[i]["ISBarCodeable"]) == "True")
                    {
                        dgvBarcode.Rows[i].Cells[1].Style.BackColor = Color.Green;
                        dgvBarcode.Rows[i].Cells[1].Style.ForeColor = Color.White;
                    }
                }
                //}
                //else
                //{
                //    if (!string.IsNullOrEmpty(Program.Floor))
                //    {
                //        int flr = Convert.ToInt32(Program.Floor);
                //        int k = 0;
                //        DataTable dtfloor = ObjPurchaseReceiptBAL.GetProductByFloor(flr, s);

                //        for (int i = 0; i < dtfloor.Rows.Count; i++)
                //        {
                //            string prodid = Convert.ToString(dtfloor.Rows[i]["Productid"]);
                //            int Location = Convert.ToInt32(dtfloor.Rows[i]["Barcode"]);
                //            for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                //            {
                //                string prodid1 = Convert.ToString(ds.Tables[1].Rows[j]["Productid"]);

                //                if (prodid == prodid1)
                //                {
                //                    if (Location == 0)
                //                    {
                //                        dgvBarcode.Rows.Add();
                //                        dgvBarcode.Rows[k].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[j]["DisplayName"]);
                //                        dgvBarcode.Rows[k].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[j]["GoodQuantity"]);
                //                        dgvBarcode.Rows[k].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[j]["Productid"]);
                //                        dgvBarcode.Rows[k].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[j]["ISBarCodeable"]);
                //                        if (Convert.ToString(ds.Tables[1].Rows[j]["ISBarCodeable"]) == "True")
                //                        {
                //                            dgvBarcode.Rows[k].Cells[1].Style.BackColor = Color.Green;
                //                            dgvBarcode.Rows[k].Cells[1].Style.ForeColor = Color.White;
                //                        }
                //                        k++;
                //                    }
                //                    else
                //                    {

                //                        ttorderno.Text = string.Empty;
                //                        ttreceiptno.Text = string.Empty;
                //                        combostatus.Text = string.Empty;
                //                        ttremarks.Text = string.Empty;
                //                        //MessageBox.Show("Barcode Completed");
                //                        break;
                //                    }
                //                }
                //            }
                //        }
                //        if (dtfloor.Rows.Count==0)
                //        {
                //            ttorderno.Text = string.Empty;
                //            ttreceiptno.Text = string.Empty;
                //            combostatus.Text = string.Empty;
                //            ttremarks.Text = string.Empty;
                //        }
                //    }
                //}

                try
                {
                    panel9.Enabled = false;
                    dgvBarcode.Focus();
                    dgvBarcode.CurrentCell = dgvBarcode[0, 0];
                    dgvBarcode.BeginEdit(true);
                }
                catch
                {

                }
            }
            else
            {
                dgvBarcode.Rows.Clear();
                panel9.Enabled = false;
            }

        }

        public void GetPRFloorcheckinByOrderNo(string s)
        {
            DataSet ds = ObjPurchaseReceiptBAL.GetPurchaseReceiptByOrderNo(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                dateTimePicker4.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderDate"]);
                //dateTimePicker1.Text = Convert.ToString(ds.Tables[0].Rows[0]["ExpectedDeliveryDate"]);
                //cbVendor.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"]);
                texorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["POOrdno"]);
                textreceiptno.Text = Convert.ToString(ds.Tables[0].Rows[0]["PROrdno"]);
                comboBoxstatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                textremarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                dgvFloorCheckIn.Rows.Clear();

                if (role == "Admin" || UserName.ToLower() == "devi" || UserName.ToLower() == "gokulnath")
                {
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        dgvFloorCheckIn.Rows.Add();
                        dgvFloorCheckIn.Rows[i].Cells[0].Value = i + 1;
                        dgvFloorCheckIn.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                        dgvFloorCheckIn.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["GoodQuantity"]);
                        dgvFloorCheckIn.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                        DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)(dgvFloorCheckIn.Rows[i].Cells["cmbLocation"]);
                        cell.DataSource = ObjPurchaseReceiptBAL.GetLocationByProductId(Convert.ToInt32(ds.Tables[1].Rows[i]["Productid"]));
                        cell.ValueMember = "LocationID";
                        cell.DisplayMember = "LocationName";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Program.Floor))
                    {
                        int flr = Convert.ToInt32(Program.Floor);
                        int k = 0;
                        DataTable dtfloor = ObjPurchaseReceiptBAL.GetProductByFloor(flr, s);

                        for (int i = 0; i < dtfloor.Rows.Count; i++)
                        {
                            string prodid = Convert.ToString(dtfloor.Rows[i]["Productid"]);
                            int Location = Convert.ToInt32(dtfloor.Rows[i]["Location"]);
                            for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                            {
                                string prodid1 = Convert.ToString(ds.Tables[1].Rows[j]["Productid"]);

                                if (prodid == prodid1)
                                {
                                    if (Location == 0)
                                    {
                                        dgvFloorCheckIn.Rows.Add();
                                        dgvFloorCheckIn.Rows[k].Cells[0].Value = k + 1;
                                        dgvFloorCheckIn.Rows[k].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[j]["DisplayName"]);
                                        dgvFloorCheckIn.Rows[k].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[j]["GoodQuantity"]);
                                        dgvFloorCheckIn.Rows[k].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[j]["Productid"]);
                                        DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)(dgvFloorCheckIn.Rows[k].Cells["cmbLocation"]);
                                        cell.DataSource = ObjPurchaseReceiptBAL.GetLocationByProductId(Convert.ToInt32(ds.Tables[1].Rows[j]["Productid"]));
                                        cell.ValueMember = "LocationID";
                                        cell.DisplayMember = "LocationName";
                                        k++;
                                    }
                                    else
                                    {
                                        texorderno.Text = string.Empty;
                                        textreceiptno.Text = string.Empty;
                                        comboBoxstatus.Text = string.Empty;
                                        textremarks.Text = string.Empty;
                                        //MessageBox.Show("FloorCheckIn Completed");
                                        break;
                                    }
                                }
                            }
                        }

                        if (dtfloor.Rows.Count == 0)
                        {
                            texorderno.Text = string.Empty;
                            textreceiptno.Text = string.Empty;
                            comboBoxstatus.Text = string.Empty;
                            textremarks.Text = string.Empty;
                        }
                    }
                }
                try
                {
                    panel2.Enabled = false;
                    dgvFloorCheckIn.CurrentCell = dgvFloorCheckIn[0, 0];
                    dgvFloorCheckIn.BeginEdit(true);
                }
                catch
                {

                }
            }
            else
            {
                dgvFloorCheckIn.Rows.Clear();
                panel2.Enabled = false;
            }

        }

        public void GetPRDamageByOrderNo(string s)
        {
            DataSet ds = ObjPurchaseReceiptBAL.GetPurchaseReceiptByOrderNo(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                dateTimePicker5.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderDate"]);
                //dateTimePicker1.Text = Convert.ToString(ds.Tables[0].Rows[0]["ExpectedDeliveryDate"]);
                //cbVendor.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"]);
                textBxorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["POOrdno"]);
                textBxreceiptno.Text = Convert.ToString(ds.Tables[0].Rows[0]["PROrdno"]);
                combobxstatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                textBxremarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                panel11.Enabled = false;
            }
            else
            {
                panel11.Enabled = true;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                if (role == "Admin")
                {
                    dgvDamage.Rows.Clear();
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        dgvDamage.Rows.Add();
                        dgvDamage.Rows[i].Cells[0].Value = i + 1;
                        dgvDamage.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                        dgvDamage.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["DamageQuantity"]);
                        dgvDamage.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    }
                    panel11.Enabled = false;
                    dgvDamage.CurrentCell = dgvDamage[0, 0];
                    dgvDamage.BeginEdit(true);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Program.Floor))
                    {
                        string flr = Program.Floor;
                        int k = 0;
                        string dtfloor = ObjPurchaseReceiptBAL.getdamagelocation(flr);
                        if (dtfloor == "Damage")
                        {
                            dgvDamage.Rows.Clear();
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {
                                dgvDamage.Rows.Add();
                                dgvDamage.Rows[i].Cells[0].Value = i + 1;
                                dgvDamage.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                                dgvDamage.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["DamageQuantity"]);
                                dgvDamage.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                            }
                            panel11.Enabled = false;
                            dgvDamage.CurrentCell = dgvDamage[0, 0];
                            dgvDamage.BeginEdit(true);
                        }
                    }
                }
            }
            else
            {
                dgvDamage.Rows.Clear();
                panel11.Enabled = false;
            }

        }

        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            bool sas = false;
            try
            {
                total();
                if (e.ColumnIndex == 4)
                {
                    double OrderedQty;
                    int Quantity = 0;
                    try
                    {
                        if (IsNewPO)
                        {
                            dgvOrder.CurrentCell = dgvOrder[1, e.RowIndex + 1];
                            dgvOrder.BeginEdit(true);
                        }
                        string itemval = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value);
                        if (!string.IsNullOrEmpty(itemval))
                        {
                            if (dtpartial.Rows.Count > 0)
                            {
                                string v = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value);
                                var rowColl = dtpartial.AsEnumerable();
                                try
                                {
                                    Quantity = (from r in rowColl
                                                where r.Field<string>("Productid") == v
                                                select r.Field<int>("Quantity")).First<int>();
                                }
                                catch (Exception eds)
                                {

                                }
                            }
                            if (Quantity <= 0)
                            {
                                OrderedQty = Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value);
                            }
                            else
                            {
                                OrderedQty = Convert.ToDouble(Quantity);
                            }
                            double RecieveQty = Convert.ToDouble(tb.Text);



                            if (IsNewPO == false)
                            {
                                //if (OrderedQty != 0)
                                //{
                                if (RecieveQty < OrderedQty)
                                {
                                    //DialogResult confirmResult = MessageBox.Show("Received Qty is lower than Requested Qty." + Environment.NewLine + "Update us partial recieve?", "Matching Quantity", MessageBoxButtons.YesNo);
                                    //if (confirmResult == DialogResult.Yes)
                                    //{

                                    //    // if 'Yes' do something here 
                                    //    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value = "Partial";

                                    //    CellQtyValidation = true;
                                    //    sas = true;

                                    //}
                                    //else
                                    //{
                                    // if 'No' do something here 
                                    //dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value;
                                    //dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value = "";
                                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = "Partial";
                                    dgvOrder.CurrentCell = dgvOrder[4, dgvOrder.CurrentCell.RowIndex];
                                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                                    dgvOrder.RefreshEdit();



                                    //}
                                }
                                else if (RecieveQty > OrderedQty)
                                {
                                    //DialogResult confirmResult = MessageBox.Show("Received Qty is higher than Requested Qty." + Environment.NewLine + "Update the requested qty to " + RecieveQty + "?", "Matching Quantity", MessageBoxButtons.YesNo);
                                    //if (confirmResult == DialogResult.Yes)
                                    //{
                                    //    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value = "Full";
                                    //    // if 'Yes' do something here 
                                    //    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value = RecieveQty;
                                    //    //dgvOrder.RefreshEdit();
                                    //    // MessageBox.Show(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value));
                                    //}
                                    //else
                                    //{
                                    //    // if 'No' do something here 
                                    //    //dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value = "";
                                    //    //CellQtyValidation = true;
                                    //    //sas = true;

                                    //    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value = "";
                                    //    dgvOrder.CurrentCell = dgvOrder[3, dgvOrder.CurrentCell.RowIndex];
                                    //    //dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value;
                                    //    dgvOrder.RefreshEdit();
                                    //}

                                  
                                    dgvOrder.CurrentCell = dgvOrder[4, dgvOrder.CurrentCell.RowIndex];
                                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                                    dgvOrder.RefreshEdit();

                                }
                                else if (RecieveQty == OrderedQty)
                                {
                                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = "Full";
                                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.White;
                                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                                }
                                //}
                            }
                            else
                            {
                                dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value = RecieveQty;
                            }
                        }
                        else
                        {
                            //dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value = "";
                            dgvOrder.RefreshEdit();
                        }
                        
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[4, dgvOrder.CurrentCell.RowIndex + 1];
                        dgvOrder.BeginEdit(true);

                       
                    }
                    catch (Exception es)
                    {
                        //dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = 0;
                    }


                    if (sas == false)
                    {
                        dgvOrder.Focus();
                        edit = true;
                        //dgvOrder.CurrentCell = dgvOrder[1, e.RowIndex + 1];

                        string val = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value);
                        string qtyval = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
                        if (!string.IsNullOrEmpty(val) && !string.IsNullOrEmpty(qtyval))
                        {
                            //dgvOrder.Rows.Add(1);
                            //dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                            //dgvOrder.BeginEdit(true);
                        }

                        //if (e.RowIndex == (dgvOrder.Rows.Count - 1))
                        //{
                        //    this.ActiveControl = txtRemarks;
                        //}
                        //else
                        //{
                        //    dgvOrder.BeginEdit(true);
                        //}
                    }
                }
            }
            catch
            {

            }



        }

        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                if (txtCity.Visible)
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
                }
                else
                {
                    pnsearch.Visible = false;
                }
                this.ActiveControl = Txtitem;
                lblrowindex.Text = e.RowIndex.ToString();
                lblpageno.Text = Convert.ToString(Convert.ToInt32(lblpageno.Text) + 1);
            }
            else
            {
                pnsearch.Visible = false;
            }
        }

        private void dgvOrder_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                total();
            }
        }

        private void dgvOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvOrder.CurrentCell.ColumnIndex;
            string headerText = dgvOrder.Columns[column].HeaderText;

            if (headerText.Equals("Received Qty"))
            {
                tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.TextChanged += new EventHandler(textbox_Change);
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.MaxLength = 10;
                }
            }
        }

        private void textbox_Change(object sender, EventArgs e)
        {
            //try
            //{
            //    decimal totalamount = 0, totalquantity = 0;
            //    for (int i = 0; i < dgvOrder.Rows.Count; i++)
            //    {
            //        //totalamount = totalamount + Convert.ToInt32(dgvOrder.Rows[i].Cells[6].Value);
            //        totalquantity = totalquantity + Convert.ToDecimal(dgvOrder.Rows[i].Cells[3].Value);
            //    }
            //    if (!string.IsNullOrEmpty(tb.Text))
            //    {
            //        lbltotalquantity.Text = Convert.ToString(totalquantity + Convert.ToDecimal(tb.Text));
            //    }
            //    else
            //    {
            //        lbltotalquantity.Text = Convert.ToString(totalquantity);
            //    }
            //}
            //catch
            //{

            //}

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

        public void total()
        {
            try
            {
                double totalamount = 0, totalquantity = 0;
                for (int i = 0; i < dgvOrder.Rows.Count; i++)
                {
                    //totalamount = totalamount + Convert.ToInt32(dgvOrder.Rows[i].Cells[6].Value);
                    totalquantity = totalquantity + Convert.ToDouble(dgvOrder.Rows[i].Cells[4].Value);
                }

                lbltotalquantity.Text = Convert.ToString(totalquantity);
                //lbltotalamount.Text = Convert.ToString(totalamount);
            }
            catch(Exception e)
            {

            }

        }

        private void dgvOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                //e.SuppressKeyPress = true;
                //if (dgvOrder.CurrentCell.ColumnIndex == 3)
                //{
                //    dgvOrder.Focus();
                //    dgvOrder.CurrentCell = dgvOrder[3, dgvOrder.CurrentCell.RowIndex+1];
                //    dgvOrder.BeginEdit(true);

                //}
                //else if (dgvOrder.CurrentCell.ColumnIndex == 1)
                //{
                //    dgvOrder.Focus();
                //    dgvOrder.CurrentCell = dgvOrder[3, dgvOrder.CurrentCell.RowIndex];
                //    dgvOrder.BeginEdit(true);
                //}


                try
                {
                    if (dgvOrder.CurrentCell.ColumnIndex == 3)
                    {
                        if (IsNewPO)
                        {
                            dgvOrder.Focus();
                            string val = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value);
                            string qtyval = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value);
                            if (!string.IsNullOrEmpty(val) && !string.IsNullOrEmpty(qtyval))
                            {
                                dgvOrder.Rows.Add(1);
                                dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                            }
                        }
                        else
                        {
                            if (dgvOrder.CurrentCell.RowIndex == dgvOrder.Rows.Count - 1)
                            {
                                butsave.Focus();
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            // ProdSelRowvalue = 0;
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

        private void Btnsubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Txtitem.Text))
            {
                int rowindex = Convert.ToInt32(lblrowindex.Text);
                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];
                dgvOrder.Rows[rowindex].Cells[4].Value = lblproductid.Text;
                dgvOrder.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                //dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                //dgvOrder.Rows[rowindex].Cells[4].Value = lblprice.Text;
                dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                pnsearch.Visible = false;
                DgvAutoRefNo.Visible = false;

                lblproductid.Text = string.Empty;
                //Txtitem.Text = string.Empty;
                lblitemcode.Text = "0";
                lblprice.Text = "0";
                lbldisplay.Text = "0";
                //lbldemo.Text = "0";
                //lblservice.Text = "0";
                //lbldamage.Text = "0";
                lblrack.Text = "0";
                dgvOrder.Focus();
                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3];
                dgvOrder.BeginEdit(true);
            }
            else
            {
                this.ActiveControl = butsave;
                pnsearch.Visible = false;
                //MessageBox.Show("Please Enter Product Name");
                //Txtitem.Focus();
            }
        }

        private void transactionclose_Click(object sender, EventArgs e)
        {
            transactionclose1();
        }

        public void transactionclose1()
        {
            pnsearch.Visible = false;
            lblproductid.Text = string.Empty;
            //Txtitem.Text = string.Empty;
            lblitemcode.Text = "0";
            lblprice.Text = "0";
            lbldisplay.Text = "0";
            //lbldemo.Text = "0";
            //lblservice.Text = "0";
            //lbldamage.Text = "0";
            lblrack.Text = "0";
            cmbloaction.SelectedIndex = 0;
            LblProdNotFoundMSg.Visible = false;
        }


        private void dgvChecking_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //int column = dgvChecking.CurrentCell.ColumnIndex;
            //string headerText = dgvChecking.Columns[column].HeaderText;



            tb = e.Control as TextBox;
            if (tb != null)
            {
                tb.TextChanged += new EventHandler(Checking_Change);
                tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                tb.MaxLength = 6;
            }

        }


        private void Checking_Change(object sender, EventArgs e)
        {
            try
            {
                int column = dgvChecking.CurrentCell.ColumnIndex;
                int row = dgvChecking.CurrentCell.RowIndex;
                string headerText = dgvChecking.Columns[column].HeaderText;

                totQty = Convert.ToDouble(dgvChecking.Rows[row].Cells[2].Value);
                if (headerText.Equals("Good Qty"))
                {
                    GoodQty = Convert.ToDouble(tb.Text);
                    //DamageQty = Convert.ToInt32(dgvChecking.Rows[row].Cells[3].Value);
                    //int actQty = GoodQty + DamageQty;
                    //if (totQty != actQty)
                    //{
                    //    MessageBox.Show("Good Qty and Damage Qty is not matched with Recieved Qty");
                    //}
                }
                else if (headerText.Equals("Damage Qty"))
                {
                    DamageQty = Convert.ToDouble(tb.Text);
                    //GoodQty = Convert.ToInt32(dgvChecking.Rows[row].Cells[4].Value);


                    //int actQty = GoodQty + DamageQty;
                    //if (totQty != actQty)
                    //{
                    //    MessageBox.Show("Good Qty and Damage Qty is not matched with Recieved Qty");
                    //}

                }




            }
            catch
            {
                //dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = 0;
            }
        }

        private void dgvChecking_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //int actQty = GoodQty + DamageQty;
                //if (dgvChecking.Columns[e.ColumnIndex].HeaderText == "Good Qty")
                //{
                //    dgvChecking.Rows[e.RowIndex].Cells[4].ReadOnly = false;
                ////    if (totQty != actQty)
                ////    {
                ////        MessageBox.Show("Good Qty and Damage Qty is not matched with Recieved Qty");
                ////        CellCheckingQtyValidation = true;
                ////    }
                ////    else
                ////    {
                ////        CellCheckingQtyValidation = false;
                ////    }

                //}

                //if (dgvChecking.Columns[e.ColumnIndex].HeaderText == "Damage Qty")
                //{
                //    if (totQty != actQty)
                //    {
                //        MessageBox.Show("Good Qty and Damage Qty is not matched with Recieved Qty");
                //        dgvChecking.Rows[e.RowIndex].Cells["Damage Qty"].Value = "";
                //        dgvChecking.Rows[e.RowIndex].Cells["Good Qty"].Value = "";
                //        dgvChecking.Focus();
                //        dgvChecking.CurrentCell = dgvChecking[3, e.RowIndex];
                //        CellCheckingQtyValidation = true;
                //    }
                //    else
                //    {
                //        CellCheckingQtyValidation = false;
                //    }
                //}
            }
        }

        private void dgvFloorCheckIn_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvFloorCheckIn[e.ColumnIndex, e.RowIndex].EditType.ToString() == "System.Windows.Forms.DataGridViewComboBoxEditingControl")
                {
                    DataGridViewColumn column = dgvFloorCheckIn.Columns[e.ColumnIndex];
                    if (column is DataGridViewComboBoxColumn)
                    {
                        DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dgvFloorCheckIn[e.ColumnIndex, e.RowIndex];
                        dgvFloorCheckIn.CurrentCell = cell;
                        dgvFloorCheckIn.BeginEdit(true);
                        DataGridViewComboBoxEditingControl editingControl = (DataGridViewComboBoxEditingControl)dgvFloorCheckIn.EditingControl;
                        editingControl.DroppedDown = true;
                    }
                }
            }
        }

        private void dgvinvoice_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvinvoice[e.ColumnIndex, e.RowIndex].EditType.ToString() == "System.Windows.Forms.DataGridViewComboBoxEditingControl")
                {
                    DataGridViewColumn column = dgvinvoice.Columns[e.ColumnIndex];
                    if (column is DataGridViewComboBoxColumn)
                    {
                        DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dgvinvoice[e.ColumnIndex, e.RowIndex];
                        dgvinvoice.CurrentCell = cell;
                        dgvinvoice.BeginEdit(true);
                        DataGridViewComboBoxEditingControl editingControl = (DataGridViewComboBoxEditingControl)dgvinvoice.EditingControl;
                        editingControl.DroppedDown = true;
                    }
                }
            }
        }

        private void txtRemarks_Enter(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
        }

        private void dgvFloorCheckIn_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Check box column
            ComboBox comboBox = e.Control as ComboBox;
            if (comboBox != null)
            {
                comboBox.SelectedIndexChanged -= new EventHandler(comboBox_SelectedIndexChanged);
                // Add the event handler. 
                comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
                //comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            }
        }

        void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int row = dgvFloorCheckIn.CurrentCell.RowIndex;
                if (((ComboBox)sender).SelectedIndex > 0)
                {

                    int selectedIndex = Convert.ToInt32(((ComboBox)sender).SelectedValue);
                    if (selectedIndex > 0)
                    {

                        int prodid = Convert.ToInt32(dgvFloorCheckIn.Rows[row].Cells[3].Value);
                        string rack = ObjPurchaseReceiptBAL.GetProductLocationById(selectedIndex, prodid);
                        dgvFloorCheckIn.Rows[row].Cells["Rack"].Value = rack;
                    }

                }
                else
                {
                    dgvFloorCheckIn.Rows[row].Cells["Rack"].Value = "";
                }
            }
            catch
            {

            }
            //MessageBox.Show("Selected Index = " + selectedIndex);
        }

        private void dgvChecking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {

            }
        }

        private void dgvinvoice_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvinvoice.CurrentCell.ColumnIndex;
            string headerText = dgvinvoice.Columns[column].HeaderText;

            if (headerText.Equals("Price"))
            {
                tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(IntOnly_KeyPress);
                    tb.TextChanged += new EventHandler(txtinvoiceprice_Change);
                }
            }
            if (headerText.Equals("Tax"))
            {
                combo = e.Control as ComboBox;
                if (combo != null)
                {
                    // Remove an existing event-handler, if present, to avoid  
                    // adding multiple handlers when the editing control is reused.
                    combo.SelectedIndexChanged -=
                        new EventHandler(ComboBox_SelectedIndexChanged);

                    // Add the event handler. 
                    combo.SelectedIndexChanged +=
                        new EventHandler(ComboBox_SelectedIndexChanged);
                }
            }
        }



        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            double tax1 = 0;
            double tax2 = 0;
            string strVal = Convert.ToString(combo.Text);
            dgvinvoice.Rows[dgvinvoice.CurrentCell.RowIndex].Cells["Sum"].Value = strVal;

            for (int i = 0; i <= dgvinvoice.Rows.Count - 1; i++)
            {
                string tax = Convert.ToString(dgvinvoice.Rows[i].Cells["Sum"].Value);

                if (tax == "14.50")
                {
                    tax1 = tax1 + (14.5 / 100) * Convert.ToDouble(dgvinvoice.Rows[i].Cells["Amount"].Value);
                    //tax1 = (14.5 / 100) * Convert.ToDouble(lblinvoiceTotals.Text);
                    //tax1 = tax1 + Convert.ToDouble(tax);
                }
                else if (tax == "5.00")
                {
                    tax2 = tax2 + (5.0 / 100) * Convert.ToDouble(dgvinvoice.Rows[i].Cells["Amount"].Value);
                    //tax2 = (4.5 / 100) * Convert.ToDouble(lblinvoiceTotals.Text);
                    //tax2 = tax2 + Convert.ToDouble(tax);
                }
            }

            txttax1.Text = Convert.ToString(tax1);
            txttax2.Text = Convert.ToString(tax2);
        }

        private void txtinvoiceprice_Change(object sender, EventArgs e)
        {
            try
            {
                double total = 0;

                if (!string.IsNullOrEmpty(tb.Text))
                {
                    try
                    {
                        double Qty = Convert.ToDouble(dgvinvoice.Rows[dgvinvoice.CurrentCell.RowIndex].Cells[3].Value);
                        double price = Convert.ToDouble(tb.Text);

                        double amount = Convert.ToDouble(Qty) * price;
                        dgvinvoice.Rows[dgvinvoice.CurrentCell.RowIndex].Cells["Amount"].Value = amount;



                        double tax1 = 0;
                        double tax2 = 0;
                        //string strVal = Convert.ToString(combo.Text);
                        //dgvinvoice.Rows[dgvinvoice.CurrentCell.RowIndex].Cells["Sum"].Value = strVal;

                        for (int i = 0; i <= dgvinvoice.Rows.Count - 1; i++)
                        {
                            string tax = Convert.ToString(dgvinvoice.Rows[i].Cells["Sum"].Value);

                            if (tax == "14.50")
                            {
                                tax1 = tax1 + (14.5 / 100) * Convert.ToDouble(dgvinvoice.Rows[i].Cells["Amount"].Value);
                                //tax1 = (14.5 / 100) * Convert.ToDouble(lblinvoiceTotals.Text);
                                //tax1 = tax1 + Convert.ToDouble(tax);
                            }
                            else if (tax == "5.00")
                            {
                                tax2 = tax2 + (5.0 / 100) * Convert.ToDouble(dgvinvoice.Rows[i].Cells["Amount"].Value);
                                //tax2 = (4.5 / 100) * Convert.ToDouble(lblinvoiceTotals.Text);
                                //tax2 = tax2 + Convert.ToDouble(tax);
                            }
                        }

                        txttax1.Text = Convert.ToString(tax1);
                        txttax2.Text = Convert.ToString(tax2);



                    }
                    catch
                    {

                    }

                }
                else
                {
                    dgvinvoice.Rows[dgvinvoice.CurrentCell.RowIndex].Cells["Amount"].Value = "0";

                }

                for (int i = 0; i <= dgvinvoice.Rows.Count - 1; i++)
                {
                    total = total + Convert.ToDouble(dgvinvoice.Rows[i].Cells["Amount"].Value);

                }

                lblinvoiceTotals.Text = Convert.ToString(total);
                lblinvoiceNet.Text = Convert.ToString(total);

                lblinvoiceNet.Text = Convert.ToString(Convert.ToDouble(txttax1.Text) + Convert.ToDouble(txttax2.Text) + Convert.ToDouble(lblinvoiceTotals.Text));
                if (!string.IsNullOrEmpty(txtInvoiceDis.Text))
                {
                    lblinvoiceNet.Text = Convert.ToString(Convert.ToDouble(lblinvoiceNet.Text) + Convert.ToDouble(txtInvoiceDis.Text));
                }

                if (!string.IsNullOrEmpty(txtinvoiceRoundOff.Text))
                {
                    lblinvoiceNet.Text = Convert.ToString(Convert.ToDouble(lblinvoiceNet.Text) + Convert.ToDouble(txtinvoiceRoundOff.Text));
                }

                if (!string.IsNullOrEmpty(txtinvioceOtherCharges.Text))
                {
                    lblinvoiceNet.Text = Convert.ToString(Convert.ToDouble(lblinvoiceNet.Text) + Convert.ToDouble(txtinvioceOtherCharges.Text));
                }
            }
            catch
            {

            }


        }

        private void IntOnly_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // characters 48-57 are 0-9, 8 is backspace, 46 is decimal
            // if it's not a 0-9, not a backspace, and not a decimal,
            //    don't allow the keystroke
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46)
                e.Handled = true;
            // if keystroke was a decimal, check to see if a decimal already exists
            //    in the textbox.  if so, don't allow it.
            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.Contains("."))
                    e.Handled = true;
            }

        }

        private void dgvinvoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                int Rowindex = dgvinvoice.CurrentCell.RowIndex;
                int Columnindex = dgvinvoice.CurrentCell.ColumnIndex;
                if (Rowindex != (dgvinvoice.Rows.Count - 1))
                {
                    dgvinvoice.CurrentCell = dgvinvoice[4, Rowindex + 1];
                }
                else
                {
                    txttax1.Focus();
                }
            }
        }

        private void CalculateInvoiceNetAmount_Change(object sender, EventArgs e)
        {
            try
            {
                double total = 0;
                double tax1 = 0;
                double tax2 = 0;
                double discount = 0;
                double NetValue = 0;
                if (!string.IsNullOrEmpty(lblinvoiceTotals.Text))
                {
                    total = Convert.ToDouble(lblinvoiceTotals.Text);
                }
                //else
                //{
                //    lblinvoiceTotal.Text = "0";
                //}

                if (!string.IsNullOrEmpty(txttax1.Text))
                {

                    tax1 = Convert.ToDouble(txttax1.Text);
                }
                //else
                //{
                //    txttax1.Text = "0";
                //}
                if (!string.IsNullOrEmpty(txttax2.Text))
                {
                    tax2 = Convert.ToDouble(txttax2.Text);
                }
                //else
                //{
                //    txttax2.Text = "0";
                //}

                if (!string.IsNullOrEmpty(txtInvoiceDis.Text))
                {
                    discount = Convert.ToDouble(txtInvoiceDis.Text);
                }
                //else
                //{
                //    txtInvoiceDis.Text = "0";
                //}
                string ROS = CmbInvROS.Text;
                try
                {
                    if (ROS == "+")
                    {

                        if (!string.IsNullOrEmpty(txtinvoiceRoundOff.Text))
                        {
                            double RoundOff = Convert.ToDouble(txtinvoiceRoundOff.Text);
                            NetValue = ((total + tax1 + tax2) - discount) + RoundOff;
                        }
                        else
                        {
                            NetValue = ((total + tax1 + tax2) - discount);
                            lblinvoiceNet.Text = Convert.ToString(NetValue);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(txtinvoiceRoundOff.Text))
                        {
                            double RoundOff = Convert.ToDouble(txtinvoiceRoundOff.Text);
                            NetValue = ((total + tax1 + tax2) - discount) - RoundOff;
                        }
                        else
                        {
                            NetValue = ((total + tax1 + tax2) - discount);
                            lblinvoiceNet.Text = Convert.ToString(NetValue);
                        }
                    }
                }
                catch
                {

                }

                string OCS = CmbInvOCS.Text;
                if (OCS == "+")
                {
                    if (!string.IsNullOrEmpty(txtinvioceOtherCharges.Text))
                    {
                        double OtherCharges = Convert.ToDouble(txtinvioceOtherCharges.Text);
                        NetValue = NetValue + OtherCharges;
                        lblinvoiceNet.Text = Convert.ToString(NetValue);
                    }
                    else
                    {
                        lblinvoiceNet.Text = Convert.ToString(NetValue);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtinvioceOtherCharges.Text))
                    {
                        double OtherCharges = Convert.ToDouble(txtinvioceOtherCharges.Text);
                        NetValue = NetValue - OtherCharges;
                        lblinvoiceNet.Text = Convert.ToString(NetValue);
                    }
                    else
                    {
                        lblinvoiceNet.Text = Convert.ToString(NetValue);
                    }
                }
            }
            catch
            {

            }
        }
        bool ROSstat = false;
        bool OCSstat = false;
        private void CmbInvROS_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string ROS = CmbInvROS.Text;

                if (ROS == "+")
                {
                    if (!string.IsNullOrEmpty(txtinvoiceRoundOff.Text))
                    {
                        if (ROSstat == false)
                        {
                            double RoundOff = Convert.ToDouble(txtinvoiceRoundOff.Text);
                            double NetValue = Convert.ToDouble(lblinvoiceNet.Text);
                            NetValue = NetValue + RoundOff;
                            lblinvoiceNet.Text = Convert.ToString(NetValue);
                            ROSstat = true;
                        }
                    }
                    //else
                    //{
                    //    txtinvoiceRoundOff.Text = "0";
                    //}
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtinvoiceRoundOff.Text))
                    {
                        if (ROSstat)
                        {
                            double RoundOff = Convert.ToDouble(txtinvoiceRoundOff.Text);
                            double NetValue = Convert.ToDouble(lblinvoiceNet.Text);
                            NetValue = NetValue - RoundOff;
                            lblinvoiceNet.Text = Convert.ToString(NetValue);
                            ROSstat = false;
                        }
                    }
                    //else
                    //{
                    //    txtinvoiceRoundOff.Text = "0";
                    //}
                }
            }
            catch
            {

            }
        }

        private void CmbInvOCS_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string OCS = CmbInvOCS.Text;
                if (OCS == "+")
                {
                    if (!string.IsNullOrEmpty(txtinvioceOtherCharges.Text))
                    {
                        if (OCSstat == false)
                        {
                            double OtherCharges = Convert.ToDouble(txtinvioceOtherCharges.Text);
                            double NetValue = Convert.ToDouble(lblinvoiceNet.Text);
                            NetValue = NetValue + OtherCharges;
                            lblinvoiceNet.Text = Convert.ToString(NetValue);
                            OCSstat = true;
                        }
                    }
                    //else
                    //{
                    //    txtinvioceOtherCharges.Text = "0";
                    //}
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtinvioceOtherCharges.Text))
                    {
                        if (OCSstat)
                        {
                            double OtherCharges = Convert.ToDouble(txtinvioceOtherCharges.Text);
                            double NetValue = Convert.ToDouble(lblinvoiceNet.Text);
                            NetValue = NetValue - OtherCharges;
                            lblinvoiceNet.Text = Convert.ToString(NetValue);
                            OCSstat = false;
                        }
                    }
                    //else
                    //{
                    //    txtinvioceOtherCharges.Text = "0";
                    //}
                }
            }
            catch
            {

            }
        }

        private void PurchaseReceipt_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int j = 0; j < tabControl1.TabPages.Count; j++)
            {

                for (int i = 0; i < tabControl1.TabPages[j].Controls.Count; i++)
                {
                    tabControl1.TabPages[j].Controls[i].Dispose();
                }
            }

            tabControl1.Dispose();

            this.Dispose();

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

        private void dgvChecking_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            double actQty = GoodQty + DamageQty;
            if (dgvChecking.Columns[dgvChecking.CurrentCell.ColumnIndex].HeaderText == "Good Qty")
            {
                dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].Cells[4].ReadOnly = false;
                if (totQty != actQty)
                {
                    //MessageBox.Show("Good Qty and Damage Qty is not matching with Recieved Qty");
                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].Cells["Damage Qty"].Value = "";
                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].Cells["Good Qty"].Value = "";

                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                    dgvChecking.Focus();
                    //dgvChecking.CurrentCell = dgvChecking[3, dgvChecking.CurrentCell.RowIndex];
                    CellCheckingQtyValidation = true;
                }
                else
                {
                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    CellCheckingQtyValidation = false;
                }

            }

            if (dgvChecking.Columns[dgvChecking.CurrentCell.ColumnIndex].HeaderText == "Damage Qty")
            {
                if (totQty != actQty)
                {
                    //MessageBox.Show("Good Qty and Damage Qty is not matching with Recieved Qty");
                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].Cells["Damage Qty"].Value = "";
                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].Cells["Good Qty"].Value = "";


                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.White;


                    dgvChecking.Focus();
                    //dgvChecking.CurrentCell = dgvChecking[3, dgvChecking.CurrentCell.RowIndex];
                    CellCheckingQtyValidation = true;
                }
                else
                {
                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    //dgvChecking.Rows[dgvChecking.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.Black;


                    CellCheckingQtyValidation = false;
                }
            }
        }

        public bool getvalcheck()
        {
            bool vas = true;
            for (int i = 0; i < dgvChecking.Rows.Count; i++)
            {
                double val = Convert.ToDouble(dgvChecking.Rows[i].Cells["Good Qty"].Value);
                double val1 = Convert.ToDouble(dgvChecking.Rows[i].Cells["Damage Qty"].Value);
                double act = Convert.ToDouble(dgvChecking.Rows[i].Cells[2].Value);

                double actQty = val + val1;
                if (act != actQty)
                {
                    dgvChecking.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    dgvChecking.Rows[i].DefaultCellStyle.ForeColor = Color.White;

                    //string message = "Good Qty and Damage Qty is not matching with Recieved Qty";
                    //MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);

                    vas = false;
                    //break;
                }
                else
                {
                    dgvChecking.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    dgvChecking.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }

            }
            return vas;
        }

        public bool getnewcheck()
        {
            bool vas = true;
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                string OrderedQty = Convert.ToString(dgvOrder.Rows[i].Cells[3].Value);
                string received = Convert.ToString(dgvOrder.Rows[i].Cells[4].Value);

                if (OrderedQty != received)
                {
                    string message = "Order Qty is not matching with Recieved Qty";
                    MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);

                    vas = false;
                    break;
                }
            }
            return vas;
        }

        private void dgvrepreceipt_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvrepreceipt.CurrentCell.ColumnIndex;
            string headerText = dgvrepreceipt.Columns[column].HeaderText;

            if (headerText.Equals("Ordered Qty"))
            {
                tbreceipt = e.Control as TextBox;
                if (tbreceipt != null)
                {
                    //tbreceipt.TextChanged += new EventHandler(textbox_Change);
                    tbreceipt.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tbreceipt.MaxLength = 6;
                }
            }
        }

        private void RefScrollGrid()
        {
            if (DgvAutoRefNo.Rows.Count - 1 >= ProdSelRowvalue)
            {
                DgvAutoRefNo.FirstDisplayedScrollingRowIndex = ProdSelRowvalue;
            }
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

        public void getsino()
        {
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                dgvOrder.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void DgvAutoRefNo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (!string.IsNullOrEmpty(Txtitem.Text))
            //{
            //dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
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
                    itemdetails(Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value));

                    //dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    //dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();
                    //dgvOrder.Rows[rowindex].Cells[3].ReadOnly = false;

                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                    getitems(sa);

                    dgvOrder.Rows[rowindex].Cells[4].Value = lblproductid.Text;
                    dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();
                    dgvOrder.Rows[rowindex].Cells[3].ReadOnly = false;

                    //dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                    //double val = Convert.ToDouble(lblprice.Text);
                    //dgvOrder.Rows[rowindex].Cells[4].Value = val;
                    dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                    DgvAutoRefNo.Visible = false;


                    //dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                    //double val = Convert.ToDouble(lblprice.Text);
                    //dgvOrder.Rows[rowindex].Cells[4].Value = val;
                    dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                    DgvAutoRefNo.Visible = false;


                    pnsearch.Visible = false;
                    lblproductid.Text = string.Empty;
                    //Txtitem.Text = string.Empty;
                    lblitemcode.Text = "0";
                    lblrack.Text = "0";
                    lbldisplay.Text = "0";
                    //lbldemo.Text = "0";
                    //lblservice.Text = "0";
                    //lbldamage.Text = "0";
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

        private void TabNew_Click(object sender, EventArgs e)
        {

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
                    //dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
                    if (ProdNotFoundMSg)
                    {
                        LblProdNotFoundMSg.Visible = true;
                    }
                    else
                    {
                        int rowindex = Convert.ToInt32(lblrowindex.Text);
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];
                        dgvOrder.Rows[rowindex].Cells[4].Value = lblproductid.Text;
                        dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                        dgvOrder.Rows[rowindex].Cells[3].ReadOnly = false;

                        //dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                        //double val = Convert.ToDouble(lblprice.Text);
                        //dgvOrder.Rows[rowindex].Cells[4].Value = val;
                        dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                        DgvAutoRefNo.Visible = false;


                        pnsearch.Visible = false;
                        lblproductid.Text = string.Empty;
                       // Txtitem.Text = string.Empty;
                        lblitemcode.Text = "0";
                        lblrack.Text = "0";
                        lbldisplay.Text = "0";
                        //lbldemo.Text = "0";
                        //lblservice.Text = "0";
                        //lbldamage.Text = "0";
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
            if (e.ColumnIndex == 4)
            {
                total();
            }
        }

        private void textBxorderno_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            DeleteQuotation(txtOrderNo.Text);
        }
        public void DeleteQuotation(string QuotationId)
        {
           
            if (!string.IsNullOrEmpty(QuotationId))
            {
                DialogResult result = MessageBox.Show("Do you want to Reject " + QuotationId + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string val = ObjPurchaseReceiptBAL.DeletePurchaseRecipt(QuotationId);
                    GetsearchPurchasegoods();
                    clear();

                }
            }
        }



         public void GetsearchPurchasegoods()
        {
            string tabname = tabControl1.SelectedTab.Name;
             string ordernumber= string.Empty;
             ordernumber = textBox2.Text.Trim();
             if(ordernumber!="")
             {
                 ordernumber = textBox2.Text.Trim();
             }
             else
             {
                 ordernumber = null;
             }

            // string productname = string.Empty;
            // productname = textBox3.Text.Trim();
            // if (productname != "")
            // {
            //     productname = textBox3.Text.Trim();
            // }
            // else
            // {
            //     productname = null;
            // }
                
            //string vendor =  string.Empty;
            //vendor = Convert.ToString(comboBox1.SelectedValue);
            //if (vendor!="0")
            //{
            //    vendor = Convert.ToString(comboBox1.SelectedValue);
            //}
            //else
            //{
            //    vendor = null;
            //}

            DateTime FromDate = new DateTime(dateTimePicker8.Value.Year, dateTimePicker8.Value.Month, dateTimePicker8.Value.Day);
            DateTime ToDate = new DateTime(dateTimePicker9.Value.Year, dateTimePicker9.Value.Month, dateTimePicker9.Value.Day);
            DataTable dt;
            dt = ObjPurchaseReceiptBAL.GetPurchaseOrderforsearchReceipt(ordernumber, FromDate, ToDate, null, null, Program.userid);
            dgvSearch.Columns.Clear();
            dgvSearch.DataSource = dt;
            if (tabname != "TabNew")
            {
                if (tabname == "TabReceipt")
                {
                    dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                    dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
                    dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                    dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                else
                {

                    if (dgvSearch.Columns.Count > 0)
                    {
                        dgvSearch.Columns["PurchaseId"].Visible = false;
                        dgvSearch.Columns["OrderDate"].Visible = false;

                        // dgvSearch.Columns["OrderNumber"].Visible = false;
                        if (tabname == "TabNew")
                        {
                            dgvSearch.Columns["OrderNumber"].Visible = true;
                            dgvSearch.Columns["OrderNumber"].ReadOnly = true;
                            dgvSearch.Columns["OrderNumber"].HeaderText = "Order Number";
                            //dgvSearch.Columns["ExpectedDeliveryDate"].Visible = false;
                        }
                        else
                        {
                            dgvSearch.Columns["PurchaseOrderNo"].Visible = false;
                            dgvSearch.Columns["OrderNumber"].Visible = true;
                            dgvSearch.Columns["OrderNumber"].HeaderText = "Order Number";
                        }
                        dgvSearch.Columns["VendorId"].Visible = false;
                        dgvSearch.Columns["Vendor"].Visible = false;
                        dgvSearch.Columns["Status"].Visible = false;
                        dgvSearch.Columns["Remarks"].Visible = false;
                        dgvSearch.Columns["ApprovedBy"].Visible = false;
                        dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                        dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
                        dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                        dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }
            }

            dgvSearch.Columns["OrderNumber"].HeaderText = "Order Number";
            dgvSearch.Columns["Partialval"].HeaderText = "Partial Val";
        }
    }
}

