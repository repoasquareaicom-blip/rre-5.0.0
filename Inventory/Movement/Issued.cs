
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
using System.Reflection;
using System.Text.RegularExpressions;
using Inventory.Report;
using EstimationReport;
using Receipt;
using IssuedReceived;



namespace Inventory.Sales
{
    public partial class Issued : Form
    {
        DataTable dtDelivered;
        bool all = false;
        string role1 = string.Empty;
        string srole = string.Empty;
        string ReceiptId = string.Empty;
        PurchaseReceiptBAL ObjPurchaseReceiptBAL = new PurchaseReceiptBAL();
        DataGridViewComboBoxColumn name = new DataGridViewComboBoxColumn();
        QuotationBal objQuotationbal = new QuotationBal();
        DataTable dtreceivedbalance, dtpaidbalance;
        ComboBox cmblocation;
        string cas = string.Empty;

        int ProdSelRowvalue = 0;
        DataTable dtitems;
        bool res = false;

        TextBox tb, tbamount, tbbaalanceanount, tborderquantoty, tbdeliveredqty;
        public bool edit = false;
        string clickstatus = string.Empty;
        string selectedtab = string.Empty;
        public Issued()
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

            this.WindowState = FormWindowState.Maximized;
            //panel9.Enabled = false;
            LoadPortsNew();
           
            LoadPortsChecking();
            LoadPortsDelivery();
            LoadPortsFloorCheckOut();
           
            ddlpaymenttype.SelectedIndex = 0;




            //SearchPurchaseOrder();
           
           
            selectedtab = MainTabSalesBill.SelectedTab.Name;

            if (selectedtab == "TabNew")
            {
                GetSearchissue();
                DataTable dt = bindEstimation();
       
                btnSavePending.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = true;
                checkBox1.Visible = false;
            }


            else if (selectedtab == "TabChecking")
            {

                GetSearchissuechecking();

                DataTable dt = bindcheckout();

                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
                checkBox1.Visible = false;
            }
            else if (selectedtab == "TabDelivery")
            {
                GetSearchissueDelivery();
                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
                checkBox1.Visible = true;
            }



            //if (role1 != "Admin")
            //{
            //    DataTable dt1 = Program.Dtmenu;




            //    bool contains6 = dt1.AsEnumerable()
            //    .Any(row => "IssuedPDI" == row.Field<String>("Data"));
            //    if (contains6 == false)
            //    {
            //        MainTabSalesBill.TabPages.Remove(TabChecking);
            //        btnPrint.Enabled = false;
            //        btnNew.Enabled = false;
            //        btnsave.Enabled = true;
            //        btnSavePending.Enabled = false;
            //        btnClear.Enabled = true;
            //    }

            //    bool contains3 = dt1.AsEnumerable()
            //     .Any(row => "IssuedCheckout" == row.Field<String>("Data"));
            //    if (contains3 == false)
            //    {
            //        MainTabSalesBill.TabPages.Remove(Tabcheckout);
            //        btnPrint.Enabled = false;
            //        btnNew.Enabled = false;
            //        btnsave.Enabled = true;
            //        btnSavePending.Enabled = false;
            //        btnClear.Enabled = true;
            //    }

            //    bool contains4 = dt1.AsEnumerable()
            //    .Any(row => "IssuedDelivery" == row.Field<String>("Data"));
            //    if (contains4 == false)
            //    {
            //        MainTabSalesBill.TabPages.Remove(TabDelivery);
            //        btnPrint.Enabled = false;
            //        btnNew.Enabled = false;
            //        btnsave.Enabled = true;
            //        btnSavePending.Enabled = false;
            //        btnClear.Enabled = true;
            //    }
            //}

          
            

        }

        public bool getreturns()
        {
            bool returns = false;
            for (int i = 0; i < dgvNew.Rows.Count; i++)
            {
                double v = Convert.ToDouble(dgvNew.Rows[i].Cells["Quantity"].Value);
                if (v > 0)
                {
                    returns = true;
                    break;
                }
                else
                {
                    returns = false;
                }
            }
            return returns;
        }

       
        #region Search
        private void pbxCollapse_Click(object sender, EventArgs e)
        {
            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;
                vLabel4.Enabled = true;
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
               

            }
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

            //    pictureBox1.Image = Image.FromStream(ms);
            //    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            //    pictureBox3.Image = Image.FromStream(ms);
            //    pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;

            //    pictureBox4.Image = Image.FromStream(ms);
            //    pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;

            //    pictureBox5.Image = Image.FromStream(ms);
            //    pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;

            //    pictureBox6.Image = Image.FromStream(ms);
            //    pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
            //}
        }




        #endregion

        #region SalesBillTabPage
        private void LoadPortsNew()
        {
            DataTable dt = new DataTable();
            dgvNew.Rows.Clear();
            dgvNew.ColumnCount = 8;
            //dgvOrder.RowCount = 16;

            dgvNew.Columns[0].Name = "S.NO";
            dgvNew.Columns[1].Name = "Items";
            dgvNew.Columns[2].Name = "Total Quantity";//Yet to Issued
            dgvNew.Columns[3].Name = "Pending Qty";//Issued Quantity
            dgvNew.Columns[6].Name = "Productid";
            dgvNew.Columns[4].Name = "Issued";
            dgvNew.Columns[5].Name = "Quantity";
            dgvNew.Columns[7].Name = "Locationid";


            this.dgvNew.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            
            this.dgvNew.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvNew.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

           
            this.dgvNew.Columns[0].ReadOnly = true;
            this.dgvNew.Columns[1].ReadOnly = true;
            this.dgvNew.Columns[2].ReadOnly = true;
            this.dgvNew.Columns[3].ReadOnly = true;
            this.dgvNew.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvNew.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvNew.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvNew.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            this.dgvNew.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
           
            //this.dgvNew.Columns[4].ReadOnly = true;

            this.dgvNew.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;




            

            //dgvNew.Columns[4].DefaultCellStyle.Format = "N2";
            //dgvNew.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvNew.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
           

            this.dgvNew.Columns[7].ReadOnly = true;
            dgvNew.Columns[6].Visible = false;
            dgvNew.Columns[7].Visible = false;


            this.dgvNew.Columns[0].Width = 15;
            this.dgvNew.Columns[1].Width = 60;
            this.dgvNew.Columns[2].Width = 30;
            this.dgvNew.Columns[3].Width = 35;
            this.dgvNew.Columns[4].Width = 30;
            this.dgvNew.Columns[5].Width = 30;
            this.dgvNew.Columns[6].Width = 20;
            this.dgvNew.Columns[7].Width = 30;
            

            dgvNew.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvNew.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvNew.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvNew.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


            name.HeaderText = "Location";
            name.Name = "Location";
            name.DataPropertyName = "LocationID";
            name.DisplayMember = "LocationName";
            name.ValueMember = "LocationID";
            name.FlatStyle = FlatStyle.Popup;
            dgvNew.Columns.Insert(5, name);
            dgvNew.Columns["Location"].Width = 53;
            dgvNew.Columns["Location"].ReadOnly = true;
            dt = objQuotationbal.GetFloor();
            if (dt.Rows.Count > 0)
            {
                name.DataSource = dt;
            }

        }


        private void LoadPortsFloorCheckOut()
        {
            dgvFloorCheckOut.Rows.Clear();
            dgvFloorCheckOut.ColumnCount = 6;


            dgvFloorCheckOut.Columns[0].Name = "S.NO";
            dgvFloorCheckOut.Columns[1].Name = "Items";
            dgvFloorCheckOut.Columns[2].Name = "Quantity";
            dgvFloorCheckOut.Columns[4].Name = "Productid";
            dgvFloorCheckOut.Columns[3].Name = "Location";
            dgvFloorCheckOut.Columns[5].Name = "Locationid";


            this.dgvFloorCheckOut.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvFloorCheckOut.Columns[4].Visible = false;
            this.dgvFloorCheckOut.Columns[5].Visible = false;

            this.dgvFloorCheckOut.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvFloorCheckOut.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvFloorCheckOut.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            this.dgvFloorCheckOut.Columns[0].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[1].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[2].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[3].ReadOnly = true;



            this.dgvFloorCheckOut.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;






            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "ChkFloorCheckOut";
            dgvCmb.HeaderText = "IsCheckOut";
            dgvCmb.FlatStyle = FlatStyle.Popup;

            dgvFloorCheckOut.Columns.Insert(6, dgvCmb);


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvFloorCheckOut.Columns[0].Width = 45;
                this.dgvFloorCheckOut.Columns[1].Width = 300;
                this.dgvFloorCheckOut.Columns[2].Width = 100;
                this.dgvFloorCheckOut.Columns[3].Width = 100;
                this.dgvFloorCheckOut.Columns[6].Width = 80;

            }
            else
            {
                this.dgvFloorCheckOut.Columns[0].Width = 45;
                this.dgvFloorCheckOut.Columns[1].Width = 300;
                this.dgvFloorCheckOut.Columns[2].Width = 110;
                this.dgvFloorCheckOut.Columns[3].Width = 110;

                this.dgvFloorCheckOut.Columns[6].Width = 80;

            }
            dgvFloorCheckOut.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvFloorCheckOut.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }

        private void SearchPurchaseOrder()
        {

            dgvSearch.Rows.Clear();
            dgvSearch.Columns.Clear();
            dgvSearch.ColumnCount = 4;


            dgvSearch.Columns[0].Name = "Order No";
            dgvSearch.Columns[1].Name = "Customer Name";
            dgvSearch.Columns[2].Name = "Date";
            dgvSearch.Columns[3].Name = "RefNO";




            this.dgvSearch.Columns[0].Width = 60;

            this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvSearch.Columns[1].Width = 60;




            this.dgvSearch.Columns[2].Width = 60;






            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn c in dgvSearch.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            //this.dgvSearch.Columns[1].Visible = false;
            //this.dgvSearch.Columns[2].Visible = false;


            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        }
     
     

       
        public DataTable paymentDenotoCustomer()
        {
            DataTable paymentdo = new DataTable();
            paymentdo.Columns.Add("Denomination", typeof(string));
            paymentdo.Columns.Add("Count", typeof(int));
            paymentdo.Columns.Add("Amount", typeof(decimal));

            //paymentdo.Columns.Add("DenominationID", typeof(int));
            //paymentdo.Columns.Add("unique", typeof(int));
            paymentdo.Rows.Add("1000", 0, 0.00);
            paymentdo.Rows.Add("500", 0, 0.00);
            paymentdo.Rows.Add("100", 0, 0.00);
            paymentdo.Rows.Add("50", 0, 0.00);
            paymentdo.Rows.Add("20", 0, 0.00);
            paymentdo.Rows.Add("10", 0, 0.00);
            paymentdo.Rows.Add("5", 0, 0.00);
            paymentdo.Rows.Add("2", 0, 0.00);
            paymentdo.Rows.Add("1", 0, 0.00);

            //List<SalesBal> paymodedeno = new List<SalesBal>();
            //paymodedeno = salesbal.Getdenamination().ToList();

            //SqlDataAdapter da = new SqlDataAdapter("SELECT  Id,PaymentMethod FROM MASTER_PaymentMethod", ObjConn);
            //DataTable dtp = new DataTable();
            //da.Fill(dtp);

            //for (int i = 0; i < paymodedeno.Count(); i++)
            //{

            //    string paydeno = paymodedeno.ToList()[i].denoType;
            //    int paydenoid = paymodedeno.ToList()[i].denoId;
            //    paymentdo.Rows.Add(paydeno, 0, 0.00, paydenoid, 0);
            //}
            return paymentdo;

        }


    
        private void LoadPortsChecking()
        {
            dgvChecking.Rows.Clear();
            dgvChecking.ColumnCount = 5;


            dgvChecking.Columns[0].Name = "S.NO";
            dgvChecking.Columns[1].Name = "Items";

            dgvChecking.Columns[2].Name = "originalQuantity";
            dgvChecking.Columns[3].Name = "Quantity";
            dgvChecking.Columns[4].Name = "Productid";

            this.dgvChecking.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            this.dgvChecking.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvChecking.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvChecking.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvChecking.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvChecking.Columns[0].ReadOnly = true;
            this.dgvChecking.Columns[1].ReadOnly = true;


            this.dgvChecking.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvChecking.Columns[2].Visible = false;
            this.dgvChecking.Columns[4].Visible = false;

            this.dgvChecking.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            //DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            //dgvCmb.ValueType = typeof(bool);
            //dgvCmb.Name = "ChkIsCheckOut";
            //dgvCmb.HeaderText = "Is CheckOut";
            //dgvChecking.Columns.Insert(4, dgvCmb);
            //this.dgvChecking.Columns[4].Width = 110;

            //DataGridViewCheckBoxColumn dgvCm = new DataGridViewCheckBoxColumn();
            //dgvCm.ValueType = typeof(bool);
            //dgvCm.Name = "ChkIsChecking";
            //dgvCm.HeaderText = "Checked";
            //dgvChecking.Columns.Insert(5, dgvCm);
            //this.dgvChecking.Columns[5].Width = 110;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvChecking.Columns[0].Width = 10;
                this.dgvChecking.Columns[1].Width = 120;
                this.dgvChecking.Columns[3].Width = 80;
                //this.dgvChecking.Columns[4].Width = 80;
                //this.dgvChecking.Columns[5].Width = 80;
            }
            else
            {
                this.dgvChecking.Columns[0].Width = 10;
                this.dgvChecking.Columns[1].Width = 120;
                this.dgvChecking.Columns[3].Width = 80;
                //this.dgvChecking.Columns[4].Width = 80;
                //this.dgvChecking.Columns[5].Width = 80;

            }
            dgvChecking.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvChecking.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }
        private void LoadPortsDelivery()
        {
            dgvDelivery.Rows.Clear();
            dgvDelivery.ColumnCount = 6;


            dgvDelivery.Columns[0].Name = "S.NO";
            dgvDelivery.Columns[1].Name = "Items";
            dgvDelivery.Columns[2].Name = "Quantity";

            dgvDelivery.Columns[3].Name = "Productid";
            dgvDelivery.Columns[4].Name = "Delivered Qty";
            dgvDelivery.Columns[5].Name = "Balance Qty";


            this.dgvDelivery.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            dgvDelivery.Columns[3].Visible = false;
            this.dgvDelivery.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvDelivery.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvDelivery.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvDelivery.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvDelivery.Columns[0].ReadOnly = true;
            this.dgvDelivery.Columns[1].ReadOnly = true;
            this.dgvDelivery.Columns[2].ReadOnly = true;
            this.dgvDelivery.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvDelivery.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvDelivery.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvDelivery.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;




            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "ChkDelivery";
            dgvCmb.HeaderText = "Delivered";
            dgvDelivery.Columns.Insert(6, dgvCmb);
            this.dgvDelivery.Columns[6].Width = 60;
            this.dgvDelivery.Columns[5].ReadOnly = true;
            this.dgvDelivery.Columns[6].ReadOnly = true;

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvDelivery.Columns[0].Width = 40;
                this.dgvDelivery.Columns[1].Width = 250;
                this.dgvDelivery.Columns[2].Width = 70;
                this.dgvDelivery.Columns[3].Width = 120;
                this.dgvDelivery.Columns[4].Width = 120;
                this.dgvDelivery.Columns[5].Width = 120;
            }
            else
            {
                this.dgvDelivery.Columns[0].Width = 30;
                this.dgvDelivery.Columns[1].Width = 200;
                this.dgvDelivery.Columns[2].Width = 80;
                this.dgvDelivery.Columns[3].Width = 110;
                this.dgvDelivery.Columns[4].Width = 110;
                this.dgvDelivery.Columns[5].Width = 110;

            }
            dgvDelivery.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvDelivery.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }
        private void MainTabSalesBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedtab = MainTabSalesBill.SelectedTab.Name;
            if (selectedtab == "TabNew")
            {
                GetSearchissue();
                btnPrint.Enabled = true;
                btnNew.Enabled = true;
                btnsave.Enabled = true;
                btnSavePending.Enabled = true;
                btnClear.Enabled = true;
                checkBox1.Visible = false;

            }
            else if (selectedtab == "Tabcheckout")
            {
                GetSearchissuecheck();
                btnsave.Enabled = true;
                btnClear.Enabled = true;
                checkBox1.Visible = false;
            }


            else if (selectedtab == "TabChecking")
            {

                GetSearchissuechecking();
                btnPrint.Enabled = false;
                btnNew.Enabled = false;
                btnsave.Enabled = true;
                btnSavePending.Enabled = false;
                btnClear.Enabled = true;
                checkBox1.Visible = false;
            }
            else if (selectedtab == "TabDelivery")
            {

                GetSearchissueDelivery();
                btnPrint.Enabled = false;
                btnNew.Enabled = false;
                btnsave.Enabled = true;
                btnSavePending.Enabled = false;
                btnClear.Enabled = true;
                checkBox1.Visible = true;
            }


            

        }
        #endregion

        private void TabPaymentReceived_Click(object sender, EventArgs e)
        {

        }

        private void MainTabSalesBill_Click(object sender, EventArgs e)
        {

        }

        //private void SearchPurchaseOrder()
        //{

        //    dgvSearch.Rows.Clear();
        //    dgvSearch.Columns.Clear();
        //    dgvSearch.ColumnCount = 2;


        //    dgvSearch.Columns[0].Name = "Order No";
        //    dgvSearch.Columns[1].Name = "Quotation No";
        //    //dgvSearch.Columns[3].Name = "Customer Name";
        //    //dgvSearch.Columns[4].Name = "Date";




        //    this.dgvSearch.Columns[0].Width = 60;

        //    this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        //  //  this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


        //    this.dgvSearch.Columns[1].Width = 60;




        //   // this.dgvSearch.Columns[2].Width = 60;


        //    dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //    dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
        //    dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;




        //    dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    foreach (DataGridViewColumn c in dgvSearch.Columns)
        //    {
        //        c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
        //    }

        //    //this.dgvSearch.Columns[3].Visible = false;
        //    //this.dgvSearch.Columns[4].Visible = false;

        //}

        private void btnGenerateBill_Click(object sender, EventArgs e)
        {
            if (ddlpaymenttype.SelectedIndex == 0)
            {
                MessageBox.Show("Select payment type");
                this.ActiveControl = ddlpaymenttype;
                return;
            }


        }


        #region Validation
        public bool validationNEW()
        {
            bool Status = true;
            string msg = "";
            int i = 0;
            if (ddlpaymenttype.SelectedIndex == 0)
            {
                i++;
                msg = msg + "*Select Payment type" + "\n";
                if (i == 1)
                    this.ActiveControl = ddlpaymenttype;

            }
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + msg);
                Status = false;

            }
            return Status;


        }

      
        #endregion

        private void SalesBillNew_Load(object sender, EventArgs e)
        {
           
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                //if (keyData == (Keys.Alt | Keys.Insert))
                //{
                //    if (dgvNew.Rows.Count <= 0)
                //    {
                //        dgvNew.Rows.Add();
                //    }
                //    else
                //    {
                //        int rowindex = dgvNew.CurrentRow.Index;
                //        int colindex = dgvNew.CurrentCell.ColumnIndex;
                //        //dgvOrder.Rows.Insert(rowindex, dgvOrder.Rows.Add(1));
                //        dgvNew.Rows.Insert(rowindex, 1);
                //        return true;
                //    }

                //    getsino();

                //}

                //if (keyData == (Keys.Alt | Keys.Delete))
                //{
                //    DialogResult result = MessageBox.Show("Do you want to Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //    if (result == DialogResult.Yes)
                //    {
                //        if (dgvNew.Rows.Count > 0)
                //        {
                //            try
                //            {
                //                int rowindex = dgvNew.CurrentRow.Index;
                //                int colindex = dgvNew.CurrentCell.ColumnIndex;
                //                dgvNew.Rows.RemoveAt(rowindex);
                //            }
                //            catch
                //            {

                //                if (dgvNew.Rows.Count - 1 == dgvNew.CurrentCell.RowIndex)
                //                {
                //                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[0].Value = "";
                //                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[1].Value = "";
                //                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[2].Value = "";
                //                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[3].Value = "";
                //                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value = "";
                //                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value = "";
                //                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[6].Value = "";

                //                }
                //            }
                //            getsino();
                           
                //            return true;
                //        }

                //        if (dgvNew.Rows.Count == 0)
                //        {
                //            dgvNew.Rows.Add();
                //        }

                //    }

                //}
      

                


                if (keyData == Keys.Escape)
                {

                    selectedtab = MainTabSalesBill.SelectedTab.Name;

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
                    
                    return true;
                }



               

                

                try
                {
                    if (keyData == Keys.Tab)
                    {
                        if (dgvNew.CurrentCell.ColumnIndex == 5)
                        {
                            dgvNew.Focus();
                            //edit = true;
                            dgvNew.CurrentCell = dgvNew[1, dgvNew.CurrentCell.RowIndex + 1];
                        }
                    }
                }
                catch
                {

                }

              

            }
            catch
            {

            }

            return base.ProcessCmdKey(ref msg, keyData);
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


        private bool Validationcheckout()
        {
            bool status = true;
            string message = "";
            int i = 0;

            foreach (DataGridViewRow row in dgvFloorCheckOut.Rows)
            {
                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];

                if (Convert.ToBoolean(CbxCell.Value) == false && Convert.ToDecimal(row.Cells[2].Value) >= 0)
                {
                    status = false;
                    break;
                }

            }
            return status;
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

        private void btncreditsave_Click(object sender, EventArgs e)
        {
            if (selectedtab == "TabNew")
            {
                bool vali = Validation1();
            
                if (vali)
                {
                 save();
                 clear();
                 GetSearchissue();
                }
            }
            
             if (selectedtab == "Tabcheckout")
            {
                bool vali = Validationcheckout();
            
                if (vali)
                {
                 savecheckout();
                 clear();
                 GetSearchissuecheck();
                 itemdetailsval("");
                }
                else
                {
                    MessageBox.Show("Please Check All Items");
                }
            }
            
            if (selectedtab == "TabChecking")
            {
                if (valcke())
                {
                    bool vali = Validationchecking();
                    if (vali)
                    {
                        DataTable dt = DataGridView2DataTable(dgvChecking);
                       // RemoveNullColumnFromDataTable(dt);
                        dt.Columns.RemoveAt(0);
                        dt.Columns.RemoveAt(0);
                        dt.Columns["Quantity"].ColumnName = "Rack";
                        dt.AcceptChanges();
                        dt.Columns["originalQuantity"].ColumnName = "Quantity";

                        dt.AcceptChanges();

                        savechecking(dt);
                        GetSearchissuechecking();

                    }
                    else
                    {
                        MessageBox.Show("Quantity Should Not Be Empty");
                    }
                }

            }

            if (selectedtab == "TabDelivery")
            {
                bool vas = Validationdelivery1();
                bool vali = Validationdelivery();
                if (vas == true)
                {
                    if (vali)
                    {
                        DataTable dt = DataGridView2DataTable(dgvDelivery);
                        //RemoveNullColumnFromDataTable(dt);
                        dt.Columns.RemoveAt(0);
                        dt.Columns.RemoveAt(0);
                        dt.Columns.RemoveAt(2);
                        dt.Columns.RemoveAt(2);
                        dt.Columns.RemoveAt(2);
                        dt.Columns.Add("Rack").SetOrdinal(1);

                        deletedelivered();
                        savedelivered("", 2, dt, "", "", 0);
                        //string s = getcheckedvalue();
                        GetSearchissueDelivery();
                        //Pnloading6.Visible = false;
                        clear();
                    }
                    else
                    {
                        //DialogResult result = MessageBox.Show("Do you want to Partial Deliverd?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        //if (result == DialogResult.Yes)
                        //{
                        deletedelivered();
                        string s = getcheckedvalue();
                        GetSearchissueDelivery();
                        // Pnloading6.Visible = false;
                        clear();

                        //}
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Txtdeliveryorderno.Text))
                    {
                        MessageBox.Show("Please select Estimation");
                    }
                    else
                    {
                        MessageBox.Show("Please Enter Correct Quantity");
                    }
                }

            }

         


        }

        private void btnSavePending_Click(object sender, EventArgs e)
        {
            if (selectedtab == "TabNew")
            {
                bool vali = Validation1();
                if (vali)
                {
                    save();
                }
            }
          
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }

        private bool Validationchecking()
        {
            bool status = true;
            string message = "";
            for (int i = 0; i < dgvChecking.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dgvChecking.Rows[i].Cells[3].Value)) && (Convert.ToDecimal(dgvChecking.Rows[i].Cells[2].Value) >= 0))
                {
                    status = false;
                }

            }

            return status;
        }

        public bool valcke()
        {
            bool status = true;
            try
            {

                for (int i = 0; i < dgvChecking.Rows.Count; i++)
                {
                    if (Convert.ToDouble(dgvChecking.Rows[i].Cells[2].Value) > 0)
                    {
                        if (Convert.ToDouble(dgvChecking.Rows[i].Cells[2].Value) != Convert.ToDouble(dgvChecking.Rows[i].Cells[3].Value))
                        {

                            MessageBox.Show("Please Enter Correct Quantity");

                            dgvChecking.Focus();
                            edit = true;
                            dgvChecking.CurrentCell = dgvChecking[3, i];
                            dgvChecking.Rows[i].Cells[3].Value = "";
                            status = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {

                MessageBox.Show("Please Enter Correct Quantity");
                status = false;


            }
            return status;
        }

        private bool Validation1()
        {
            bool status = true;
            string message = "";
            int i = 0;

           


            


     


            //if (cmbreference.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please select Reference" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmbreference;
            //}


            



            if (dgvNew.Rows.Count > 0)
            {
                i++;
                string Items = Convert.ToString(dgvNew.Rows[0].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvNew.Rows[0].Cells["Items"].Value);
                if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                {
                    message = message + "* Please Select Product" + "\n";
                }

                if (i == 1)
                    this.ActiveControl = dgvNew;
            }
            else if (dgvNew.Rows.Count == 0)
            {
                i++;
                message = message + "* Please Select Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvNew;
            }

            bool sas = false;

            for (int k = 0; k < dgvNew.RowCount; k++)
            {
                string Items = Convert.ToString(dgvNew.Rows[k].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvNew.Rows[k].Cells["Items"].Value);

                if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))) || Items == "." || Items == "-")
                {
                    sas = true;
                    break;
                }
            }
            if (sas == true)
            {
                i++;
                message = message + "* Product or quantity should not be empty." + "\n";
                if (i == 1)
                    this.ActiveControl = dgvNew;
            }



            for (int h = 0; h < dgvNew.RowCount; h++)
            {
                if (Convert.ToString(dgvNew.Rows[h].Cells["Quantity"].Value) != ".")
                {
                    double qty = Convert.ToDouble(dgvNew.Rows[h].Cells["Quantity"].Value);
                    double orgqty = Convert.ToDouble(dgvNew.Rows[h].Cells["Pending Qty"].Value);
                    double rec = Convert.ToDouble(dgvNew.Rows[h].Cells["Issued"].Value);
                    double balance = orgqty - rec;

                    if (balance < qty)
                    {
                        dgvNew.Rows[h].DefaultCellStyle.ForeColor = Color.White;
                        dgvNew.Rows[h].DefaultCellStyle.BackColor = Color.Red;
                        message = message + "* Quantity should not be greater than original quantity." + "\n";
                        status = false;
                        break;

                    }
                    else
                    {
                        dgvNew.Rows[h].DefaultCellStyle.ForeColor = Color.Black;
                        dgvNew.Rows[h].DefaultCellStyle.BackColor = Color.White;
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

            if (selectedtab == "TabNew")
            {
                dgvNew.Rows.Clear();
                txtOrderNo.Text = string.Empty;
                txtissucustomer.Text = string.Empty;
                txtreceiveno.Text = string.Empty;
            }

            if (selectedtab == "Tabcheckout")
            {
                dgvFloorCheckOut.Rows.Clear();
                Txtcheckrefno.Text = string.Empty;
                Txtcheckoutcustomername.Text = string.Empty;
                Txtcheckestno.Text = string.Empty;
            }

            else if (selectedtab == "TabChecking")
            {
                Txtcheckcustmername.Text = string.Empty;
                txtcheckingrefno.Text = string.Empty;
          

                dgvChecking.Rows.Clear();

                txtcheckingorderno.Text = string.Empty;
              

            }
            else if (selectedtab == "TabDelivery")
            {
                txtdeliverycustomername.Text = string.Empty;
                Txtdeviveryrefno.Text = string.Empty;
              

                dgvDelivery.Rows.Clear();

                Txtdeliveryorderno.Text = string.Empty;
              

            }


           


        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            preview();
        }

        public void preview()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtOrderNo.Text))
                {
                    DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Quotationreport rpt = new Quotationreport(txtOrderNo.Text);
                        rpt.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("Please Select The Item");
                }
            }
            catch (Exception e)
            {

            }
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

        public DataTable bindcheckout()
        {
            DataTable dt = new DataTable();
            dt = objQuotationbal.itemcheckoutorderno();
            DataRow dr = dt.NewRow();
            dr["Estimationid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);
            return dt;
        }

        private void dgvNew_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5)
                {
                    dgvNew.Focus();
                    //edit = true;
                    dgvNew.CurrentCell = dgvNew[1, e.RowIndex + 1];
                }
            }
            catch
            {

            }
        }

        private void dgvNew_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

            if (Convert.ToString(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells["Quantity"].Value) != ".")
            {
                double qty = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells["Quantity"].Value);
                double orgqty = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells["Pending Qty"].Value);
                double rec = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells["Issued"].Value);
                double balance = orgqty - rec;

                if (balance < qty)
                {
                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.White;
                }
            }
           
         
        }

        private void dgvNew_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvNew.CurrentCell.ColumnIndex;
            string headerText = dgvNew.Columns[column].HeaderText;

            if (headerText.Equals("Quantity"))
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

        }

        private void dgvNew_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dgvNew.CurrentCell.RowIndex == dgvNew.Rows.Count - 1)
                {
                    if (e.KeyData == Keys.Enter)
                    {
                        btnsave.Focus();
                    }
                }
            }
            catch
            {

            }

        }

        private void dgvNew_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (edit == true)
                {
                    if (dgvNew.CurrentCell.RowIndex >= 1)
                    {
                        dgvNew.CurrentCell = dgvNew[dgvNew.CurrentCell.ColumnIndex, dgvNew.CurrentCell.RowIndex - 1];
                        edit = false;
                    }
                    else if (dgvNew.CurrentCell.RowIndex == 0)
                    {
                        dgvNew.CurrentCell = dgvNew[dgvNew.CurrentCell.ColumnIndex, dgvNew.CurrentCell.RowIndex - 1];
                    }

                }
            }
            catch
            {

            }
        }

    

        //Regex reg = new Regex(@"^-?\d+[.]?\d*$");
        //Regex reg1 = new Regex(@"^-?[.]?\d*$");
        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
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

        public void RemoveNullColumnFromDataTable1(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {

                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][1])) || string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][3])))
                    dt.Rows[i].Delete();

            }
            dt.AcceptChanges();
        }

     

        public void save()
        {
          
            DataTable dt = new DataTable();
            dt = DataGridView2DataTable(dgvNew);

            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }

            dt.Columns.RemoveAt(0);
            dt.Columns.RemoveAt(1);
            dt.Columns.RemoveAt(0);

            string s = objQuotationbal.SaveIssued(txtreceiveno.Text,Program.userid,dt);
            
             if(!string.IsNullOrEmpty(s))
             {
                 GetReport(s);
                 clear();
             }

            //for (int i = 0; i < dgvNew.Rows.Count; i++)
            //{
            //    string productid = Convert.ToString(dgvNew.Rows[i].Cells["Productid"].Value);
            //    string Quantity = Convert.ToString(dgvNew.Rows[i].Cells["Quantity"].Value);
            //    string Locationid = Convert.ToString(dgvNew.Rows[i].Cells["Locationid"].Value);

            //   string s= objQuotationbal.SaveIssued(txtreceiveno.Text, productid, Quantity, Locationid);
            //}

      
        
          

        }

        public void savecheckout()
        {

            DataTable dt = new DataTable();
            dt = DataGridView2DataTable(dgvFloorCheckOut);

            for (int i = 0; i <2; i++)
            {
                dt.Columns.RemoveAt(0);
            }

            dt.Columns.RemoveAt(1);
            dt.Columns.RemoveAt(3);

            string s = objQuotationbal.saveIssuedcheckout(Txtcheckrefno.Text, dt);

            if(string.IsNullOrEmpty(s))
            {
                clear();
            }


           


        }

        public void searchReturnapproved(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchReturnapproved(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;


            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }
        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchestimation(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i][2]);
            //    dgvSearch.Rows[i].Cells[4].Value = Convert.ToString(dt.Rows[i][2]);

            //    //dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i][2]);
            //}
            //dgvSearch.Columns[1].Visible = false;

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void cbxSearchOrderNo_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void ListSearchDate1_Click_1(object sender, EventArgs e)
        {

        }

        private void pbxRightCollapse_Click(object sender, EventArgs e)
        {
            if (pnlCollapse2.Visible == true)
            {
                pnlOrder.Visible = true;
                vLabel5.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;
                //this.dgvSearch.Columns[1].Visible = true;
                //this.dgvSearch.Columns[2].Visible = true;

            }
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (selectedtab == "TabNew")
                {
                    if (e.RowIndex >= 0)
                    {
                        string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                        if (!string.IsNullOrEmpty(s))
                        {
                            getEstimation(s);
                        }
                        else
                        {
                            clear();
                        }

                    }
                }


                else if (selectedtab == "Tabcheckout")
                    {
                        if (e.RowIndex >= 0)
                        {
                            string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                            if (!string.IsNullOrEmpty(s))
                            {
                                getcheckout(s);
                                dgvFloorCheckOut.Focus();
                                if (dgvFloorCheckOut.Rows.Count > 0)
                                {
                                    dgvFloorCheckOut.CurrentCell = dgvFloorCheckOut[6, 0];
                                }
                            }
                            else
                            {
                                clear();
                            }

                        }
                    }
                
                
                else if (selectedtab == "TabChecking")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getcheck(s);
                        if (dgvChecking.Rows.Count > 0)
                        {
                            dgvChecking.Focus();
                            dgvChecking.CurrentCell = dgvChecking[3, 0];
                        }
                    }
                    else
                    {
                        clear();
                    }


                }

                else if (selectedtab == "TabDelivery")
                {

                    string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        getDelivery(s);
                        if (dgvDelivery.Rows.Count > 0)
                        {
                            dgvDelivery.Focus();
                            dgvDelivery.CurrentCell = dgvDelivery[4, 0];
                        }
                    }
                    else
                    {
                        clear();
                    }


                }

               


            

            }
        }
       
        public void searchcheckout(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchCheckout(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //    dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i][3]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }


        public void searchPaymentmode(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchPaymentmode(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        public void savechecking(DataTable dt)
        {

            // Pnloading5.Visible = true;
            string output = objQuotationbal.saveissuechecking(txtcheckingrefno.Text, Program.userid, dt);
            if (output == "1")
            {

                //GetReport(txtcheckingorderno.Text);
         
                clear();

            }


        }

        private void Txtitem_KeyDown_1(object sender, KeyEventArgs e)
        {


            //if (!string.IsNullOrEmpty(Txtitem.Text))
            //{
            //    if (Convert.ToInt32(lblproductid.Text) != 0)
            //    {
            //        int rowindex = Convert.ToInt32(lblrowindex.Text);

            //        if (selectedtab == "TabNew")
            //        {
            //            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
            //            dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
            //            dgvNew.Rows[rowindex].Cells[1].Value = cas.ToUpper();
            //            dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;

            //            dgvNew.Rows[rowindex].Cells[5].ReadOnly = false;

            //            double val = Convert.ToDouble(lblprice.Text);

            //            dgvNew.Rows[rowindex].Cells[4].Value = val;
            //            dgvNew.Rows[rowindex].Cells[0].Value = rowindex + 1;
            //            pnsearch.Visible = false;
            //            DgvAutoRefNo.Visible = false;

            //            lblproductid.Text = string.Empty;
            //            Txtitem.Text = string.Empty;
            //            lblitemcode.Text = "0";
            //            lblprice.Text = "0";
            //            lbldisplay.Text = "0";
            //            lbldemo.Text = "0";
            //            lblservice.Text = "0";
            //            lbldamage.Text = "0";
            //            lblrack.Text = "0";
            //            dgvNew.Focus();
            //            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please Enter Correct Product Name");
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please Enter Product Name");
            //    Txtitem.Focus();
            //}
        }

        public void searchcreditapproved(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchcreditapproved(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;

            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        public void searchpay(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchpay(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void textbox1_keypress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void textbox2_keypress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        public void searchcheck(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchcheck(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        public void getcheck(string s)
        {
            DataSet ds = objQuotationbal.bindIssuedchecking(s);

            if (ds.Tables[0].Rows.Count > 0)
            {

                txtcheckingrefno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Issuedid"]);
                Txtcheckcustmername.Text = Convert.ToString(ds.Tables[0].Rows[0]["CustomerName"]);
                txtcheckingorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["RefNo"]);
                

               
            }
            else
            {
               
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
                    dgvChecking.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);

                    if (Convert.ToDecimal(dgvChecking.Rows[i].Cells[2].Value) < 0)
                    {
                        dgvChecking.Rows[i].ReadOnly = true;
                        dgvChecking.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    }
                    //double qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    //dgvChecking.Rows[i].Cells[4].Value = qty;
                    //dgvChecking.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);

                    //double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    //dgvChecking.Rows[i].Cells[6].Value = amt;


                    //dgvPaymentReceived.Rows.Add();
                    //dgvPaymentReceived.Rows[i].Cells[0].Value = i + 1;
                    //dgvPaymentReceived.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["ItemName"]);
                    //dgvPaymentReceived.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    //dgvPaymentReceived.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
                    //dgvPaymentReceived.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                }
               
            }
            else
            {
                dgvChecking.Rows.Clear();
              
            }

        }

        public void getDelivery(string s)
        {
            DataSet ds;
            if(checkBox1.Checked)
            {
                ds = objQuotationbal.bindissuedeliveredCompleted(s);
            }
            else
            {
                 ds = objQuotationbal.bindissuedelivered(s);
            }
           

            if (ds.Tables[0].Rows.Count > 0)
            {
                dgvDelivery.Enabled = true;
                Txtdeviveryrefno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Issuedid"]);
                txtdeliverycustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["CustomerName"]);
                Txtdeliveryorderno.Text = Convert.ToString(ds.Tables[0].Rows[0]["RefNo"]);
               
               
            }
            else
            {
                
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {

                dgvDelivery.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvDelivery.Rows.Add();
                    dgvDelivery.Rows[i].Cells[0].Value = i + 1;
                    dgvDelivery.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvDelivery.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    dgvDelivery.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    //dgvDelivery.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    if (checkBox1.Checked)
                    {
                        dgvDelivery.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                        dgvDelivery.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[i]["Deliveredqty"]);
                    }
                    if (Convert.ToDecimal(dgvDelivery.Rows[i].Cells[2].Value) < 0)
                    {
                        dgvDelivery.Rows[i].ReadOnly = true;
                        dgvDelivery.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    }
                    if (Convert.ToBoolean(ds.Tables[1].Rows[i]["isdeleivered"]) == true)
                    {
                        dgvDelivery.Rows[i].Cells[6].Value = Convert.ToBoolean(ds.Tables[1].Rows[i]["isdeleivered"]);
                        dgvDelivery.Rows[i].ReadOnly = true;
                    }
                    else
                    {
                        dgvDelivery.Rows[i].Cells[6].Value = Convert.ToBoolean(ds.Tables[1].Rows[i]["isdeleivered"]);
                    }


                }
              
            }
            else
            {
                dgvChecking.Rows.Clear();
              
            }

        }

        private void dgvChecking_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                try
                {
                    if (Convert.ToDouble(dgvChecking.Rows[e.RowIndex].Cells[2].Value) != Convert.ToDouble(tborderquantoty.Text))
                    {
                        MessageBox.Show("Please Enter Correct Quantity");
                        dgvChecking.Focus();
                        edit = true;
                        dgvChecking.CurrentCell = dgvChecking[3, e.RowIndex];
                        dgvChecking.Rows[e.RowIndex].Cells[3].Value = "";
                    }
                }
                catch
                {

                }
            }
        }

        private void dgvChecking_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvChecking.CurrentCell.ColumnIndex;
            string headerText = dgvChecking.Columns[column].HeaderText;

            if (headerText.Equals("Quantity"))
            {
                tborderquantoty = e.Control as TextBox;
                tborderquantoty.KeyPress += new KeyPressEventHandler(txtless_KeyPress);
                tborderquantoty.MaxLength = 6;
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

        public void searchdelivery(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchdelivery(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.DataSource = null;
            if (dt.Rows.Count > 0)
            {
                dgvSearch.DataSource = dt;
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            //}

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private bool Validationdelivery()
        {
            bool status = true;
            string message = "";
            int i = 0;

            foreach (DataGridViewRow row in dgvDelivery.Rows)
            {
                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];
                if (Convert.ToBoolean(CbxCell.Value) == false && Convert.ToDecimal(row.Cells[2].Value) >= 0)
                {
                    status = false;
                    break;
                }

            }
            return status;
        }

        private bool Validationdelivery1()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (dgvDelivery.Rows.Count > 0)
            {

                foreach (DataGridViewRow row in dgvDelivery.Rows)
                {
                    if (Convert.ToDouble(row.Cells["quantity"].Value) > 0)
                    {
                        if (Convert.ToString(row.Cells["Delivered Qty"].Value) != "." && !string.IsNullOrEmpty(Convert.ToString(row.Cells["Delivered Qty"].Value)) && row.DefaultCellStyle.BackColor != Color.LightSteelBlue)
                        {
                            if ((string.IsNullOrEmpty(Convert.ToString(row.Cells[5].Value)) && (Convert.ToDecimal(row.Cells["Delivered Qty"].Value) > 0)) || (Convert.ToDecimal(row.Cells["Quantity"].Value) < Convert.ToDecimal(row.Cells["Delivered Qty"].Value)))
                            {
                                status = false;
                            }

                        }
                        else
                        {
                            status = false;
                        }
                    }
                }
            }
            else
            {
                status = false;
            }

            return status;
        }

        public string getcheckedvalue()
        {
            string val = string.Empty;
            DataTable dt = DataGridView2DataTable(dgvDelivery);
            dt.Columns.RemoveAt(0);
            dt.Columns.RemoveAt(0);
            dt.Columns.RemoveAt(2);
            dt.Columns.RemoveAt(2);
            dt.Columns.RemoveAt(2);
            dt.Columns.Add("Rack").SetOrdinal(1);
            foreach (DataGridViewRow row in dgvDelivery.Rows)
            {
                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];
                if (Convert.ToBoolean(CbxCell.Value) == true)
                {

                    string s = Convert.ToString(row.Cells[2].Value);
                    string delivered = Convert.ToString(row.Cells[2].Value);
                    savedelivered(Convert.ToString(row.Cells[3].Value), 1, dt, s, delivered, 1);
                }
                else
                {
                    string s = Convert.ToString(row.Cells[2].Value);
                    string delivered = Convert.ToString(row.Cells[4].Value);
                    savedelivered(Convert.ToString(row.Cells[3].Value), 1, dt, s, delivered, 0);
                }

                //if(string.IsNullOrEmpty(val))
                //{
                //    val ="'"+Convert.ToString(row.Cells[3].Value)+"'";
                //}
                //else
                //{
                //    val =val+ ",'" + Convert.ToString(row.Cells[3].Value) + "'";
                //}

            }
            //string[] vasa = val.Split(',');
            //if (vasa.Length==1)
            //{
            //    val = val.Replace("'", "");
            //}

            return val;
        }

        public void savedelivered(string pid, int status, DataTable dt, string Quanity, string deliveredQty, int val)
        {
            //  Pnloading6.Visible = true;
            string s = objQuotationbal.saveissuedelivered(Txtdeviveryrefno.Text, pid, status, dt, Quanity, deliveredQty, val);

            if (s == "1")
            {
                //if (status == 2)
                //{
                //Pnloading6.Visible = false;
                //clear();
                //}
            }

        }

        public void deletedelivered()
        {

            objQuotationbal.updateIssueddelivered(Txtdeviveryrefno.Text);


        }

        private void dgvChecking_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvChecking.Rows.Count > 0)
            {
                if (dgvChecking.CurrentCell.RowIndex == dgvChecking.Rows.Count - 1)
                {
                    if (e.KeyData == Keys.Enter)
                    {
                        btnsave.Focus();
                    }
                }
            }
        }

        private void dgvDelivery_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvDelivery.Rows.Count > 0)
            {
                if (dgvDelivery.CurrentCell.RowIndex == dgvDelivery.Rows.Count - 1)
                {
                    if (e.KeyData == Keys.Enter)
                    {
                        btnsave.Focus();
                    }
                }
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
            string v = objQuotationbal.getrack(s, s1);
            return v;
        }

        private void dgvFloorCheckOut_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void vLabel4_Click(object sender, EventArgs e)
        {

            if (pnlLabelSearch.Visible == true)
            {
                pnlLabelSearch.Visible = false;
                vLabel1.Visible = false;
                pnlSearch.Visible = true;
                splitContainer1.Panel1Collapsed = false;
            }
        }

        

        private void SalesBillNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int j = 0; j < MainTabSalesBill.TabPages.Count; j++)
                {

                    for (int i = 0; i < MainTabSalesBill.TabPages[j].Controls.Count; i++)
                    {
                        MainTabSalesBill.TabPages[j].Controls[i].Dispose();
                    }
                }

                MainTabSalesBill.Dispose();

                this.Dispose();
            }
            catch
            {

            }
        }

        private void dgvDelivery_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvDelivery.CurrentCell.ColumnIndex;
            string headerText = dgvDelivery.Columns[column].HeaderText;

            if (headerText.Equals("Delivered Qty"))
            {
                tbdeliveredqty = e.Control as TextBox;
                tbdeliveredqty.KeyPress += new KeyPressEventHandler(txtless_KeyPress);
                tbdeliveredqty.MaxLength = 6;

            }
        }

        private void dgvDelivery_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    if (e.ColumnIndex == 4)
                    {
                        double sas = Convert.ToDouble(dgvDelivery.Rows[e.RowIndex].Cells[2].Value);
                        double s = sas - Convert.ToDouble(tbdeliveredqty.Text);
                        dgvDelivery.Rows[e.RowIndex].Cells[5].Value = s;
                        if (s >= 0)
                        {
                            dgvDelivery.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                            dgvDelivery.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;

                            if (s == 0)
                            {
                                DataGridViewRow row = dgvDelivery.Rows[e.RowIndex];
                                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];
                                CbxCell.Value = 1;

                            }
                            else
                            {
                                DataGridViewRow row = dgvDelivery.Rows[e.RowIndex];
                                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];
                                CbxCell.Value = 0;
                            }
                        }
                        else
                        {
                            dgvDelivery.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                            dgvDelivery.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                        }
                    }
                }
                catch
                {

                }


            }
        }

        private void dgvNew_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (Convert.ToString(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells["Quantity"].Value) != ".")
            {
                double qty = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells["Quantity"].Value);
                //double totalqty = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells["Total Quantity"].Value);
                double orgqty = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells["Pending Qty"].Value);
                double rec = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells["Issued"].Value);
                double balance = orgqty - rec;

                if (balance < qty)
                {
                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.White;
                }
            }
           
        }

        public void getsino()
        {
            for (int i = 0; i < dgvNew.Rows.Count; i++)
            {
                dgvNew.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void dgvFloorCheckOut_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void txtless_KeyPress(object sender, KeyPressEventArgs e)
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

        

 

        private void GetSearchissue()
        {
            string OrderNo = txtOrderNo.Text.Trim();
            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);
            string Vendorid;
            if (string.IsNullOrEmpty(cmbcustomer.Text))
            {
                Vendorid = null;
            }
            else
            {
                Vendorid = cmbcustomer.Text;
            }

            string Iscombined = string.Empty;
            if (all == true)
            {
                Iscombined = null;
            }
            else
            {
                if (cmbIscombined.Checked)
                {
                    Iscombined = "1";
                }
                else
                {
                    Iscombined = "0";
                }
            }
            DataTable dt = objQuotationbal.searchIssued(OrderNo, FromDate, ToDate, Vendorid, Convert.ToInt32(Iscombined));
            dgvSearch.DataSource = null;
            dgvSearch.DataSource = dt;
            //dgvSearch.Rows.Clear();
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Order No"]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Customer"]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Order Date"]);
            //    dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i]["RefNo"]);
            //}

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);


            //dgvSearch.Columns["Order No"].Visible = true;
            //dgvSearch.Columns["CustomerName"].Visible = false;
            //dgvSearch.Columns["Order Date"].Visible = false;
            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            all = false;


        }

        private void GetSearchissuecheck()
        {
            string OrderNo = txtOrderNo.Text.Trim();
            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);
            string Vendorid;
            if (string.IsNullOrEmpty(cmbcustomer.Text))
            {
                Vendorid = null;
            }
            else
            {
                Vendorid = cmbcustomer.Text;
            }

            string Iscombined = string.Empty;
            if (all == true)
            {
                Iscombined = null;
            }
            else
            {
                if (cmbIscombined.Checked)
                {
                    Iscombined = "1";
                }
                else
                {
                    Iscombined = "0";
                }
            }
            DataTable dt = objQuotationbal.searchIssuedcheck(OrderNo, FromDate, ToDate, Vendorid, Convert.ToInt32(Iscombined));
            dgvSearch.DataSource = null;
            dgvSearch.DataSource = dt;
            //dgvSearch.Rows.Clear();
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Order No"]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Customer"]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Order Date"]);
            //    dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i]["RefNo"]);
            //}

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);


            //dgvSearch.Columns["Order No"].Visible = true;
            //dgvSearch.Columns["CustomerName"].Visible = false;
            //dgvSearch.Columns["Order Date"].Visible = false;
            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            all = false;


        }


        private void GetSearchissuechecking()
        {
            string OrderNo = txtOrderNo.Text.Trim();
            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);
            string Vendorid;
            if (string.IsNullOrEmpty(cmbcustomer.Text))
            {
                Vendorid = null;
            }
            else
            {
                Vendorid = cmbcustomer.Text;
            }

            string Iscombined = string.Empty;
            if (all == true)
            {
                Iscombined = null;
            }
            else
            {
                if (cmbIscombined.Checked)
                {
                    Iscombined = "1";
                }
                else
                {
                    Iscombined = "0";
                }
            }
            DataTable dt = objQuotationbal.searchIssuedchecking(OrderNo, FromDate, ToDate, Vendorid, Convert.ToInt32(Iscombined));
            dgvSearch.DataSource = null;
            dgvSearch.DataSource = dt;
            //dgvSearch.Rows.Clear();
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Order No"]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Customer"]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Order Date"]);
            //    dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i]["RefNo"]);
            //}

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);


            //dgvSearch.Columns["Order No"].Visible = true;
            //dgvSearch.Columns["CustomerName"].Visible = false;
            //dgvSearch.Columns["Order Date"].Visible = false;
            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            all = false;


        }


        private void GetSearchissueDelivery()
        {
            
            string OrderNo = txtOrderNo.Text.Trim();
            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);
            string Vendorid;
            if (string.IsNullOrEmpty(cmbcustomer.Text))
            {
                Vendorid = null;
            }
            else
            {
                Vendorid = cmbcustomer.Text;
            }

            string Iscombined = string.Empty;
            if (all == true)
            {
                Iscombined = null;
            }
            else
            {
                if (cmbIscombined.Checked)
                {
                    Iscombined = "1";
                }
                else
                {
                    Iscombined = "0";
                }
            }
          
            string IsDelivered = string.Empty;
            if(checkBox1.Checked)
            {
                dtDelivered = objQuotationbal.searchIssuedDeliveryDirect(OrderNo, FromDate, ToDate, Vendorid, Convert.ToInt32(Iscombined));
                btnsave.Visible = false;
            }
            else
            {
                dtDelivered = objQuotationbal.searchIssuedDelivery(OrderNo, FromDate, ToDate, Vendorid, Convert.ToInt32(Iscombined));
                btnsave.Visible = true;
            }
            dgvNew.DataSource = null;
          
            dgvSearch.DataSource = null;
            dgvSearch.DataSource = dtDelivered;
            //dgvSearch.Rows.Clear();
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    dgvSearch.Rows.Add();
            //    dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Order No"]);
            //    dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Customer"]);
            //    dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Order Date"]);
            //    dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i]["RefNo"]);
            //}

            lblItemCount.Text = Convert.ToString(dtDelivered.Rows.Count);


            //dgvSearch.Columns["Order No"].Visible = true;
            //dgvSearch.Columns["CustomerName"].Visible = false;
            //dgvSearch.Columns["Order Date"].Visible = false;
            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            all = false;


        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            string s = MainTabSalesBill.SelectedTab.Name;
            if (s == "TabNew")
            {
                GetSearchissue();
            }
           else  if (s == "Tabcheckout")
            {
                GetSearchissuecheck();
            }
            else if (s == "TabChecking")
            {
                GetSearchissuechecking();
                
            }
            else  if (s == "TabDelivery")
            {
                GetSearchissueDelivery();
            }

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn c in dgvSearch.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            //this.dgvSearch.Columns[1].Visible = false;
            //this.dgvSearch.Columns[2].Visible = false;


            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            
        }


        public void getEstimation(string s)
        {
            DataSet ds = objQuotationbal.GetIssued(s, Program.Floor, Program.Userrole);
            if (ds.Tables[0].Rows.Count > 0)
            {

              txtissucustomer.Text = Convert.ToString(ds.Tables[0].Rows[0]["CustomerName"]);
                txtreceiveno.Text = Convert.ToString(ds.Tables[0].Rows[0]["ReceivedID"]);
                txtOrderNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["RefNo"]);

            }
            else
            {

                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {

                dgvNew.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {

                    dgvNew.Rows.Add();
                    dgvNew.Rows[i].Cells[0].Value = i + 1;
                    dgvNew.Rows[i].Cells["Items"].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvNew.Rows[i].Cells["Total Quantity"].Value = Convert.ToString(ds.Tables[1].Rows[i]["TotalQty"]);
                    dgvNew.Rows[i].Cells["Pending Qty"].Value = Convert.ToString(ds.Tables[1].Rows[i]["Receiveqty"]);
                    dgvNew.Rows[i].Cells["Productid"].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);


                    //string sval1 = objQuotationbal.getestimationqty(txtOrderNo.Text, Convert.ToString(ds.Tables[1].Rows[i]["Productid"]));

                    //dgvNew.Rows[i].Cells[4].Value = sval1;

                    dgvNew.Rows[i].Cells["Issued"].Value = Convert.ToString(ds.Tables[1].Rows[i]["IssueQty"]);

                    dgvNew.Rows[i].Cells["Location"].Value = Convert.ToString(ds.Tables[1].Rows[i]["LocationName"]);
                    dgvNew.Rows[i].Cells["Locationid"].Value = Convert.ToString(ds.Tables[1].Rows[i]["Location"]);
                    if (Convert.ToString(ds.Tables[1].Rows[i]["IssueQty"]) == Convert.ToString(ds.Tables[1].Rows[i]["Receiveqty"]))
                   {
                       dgvNew.Rows[i].Cells["Quantity"].Value = "0";
                       dgvNew.Rows[i].Cells["Quantity"].ReadOnly = true;
                   }

                }
               
                //panel9.Enabled = false;
                //btnsave.Enabled = false;
            }
            else
            {
                dgvNew.Rows.Clear();
                //panel9.Enabled = true;
                //btnsave.Enabled = true;
            }

        }


        public void getcheckout(string s)
        {
            DataSet ds = objQuotationbal.GetIssuedCheckout(s, Program.Floor, Program.Userrole);
            if (ds.Tables[0].Rows.Count > 0)
            {

                Txtcheckoutcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["CustomerName"]);
                Txtcheckrefno.Text = Convert.ToString(ds.Tables[0].Rows[0]["Issuedid"]);
                Txtcheckestno.Text = Convert.ToString(ds.Tables[0].Rows[0]["RefNo"]);

            }
            else
            {

                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {

                dgvFloorCheckOut.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {

                    //dgvFloorCheckOut.Columns[0].Name = "S.NO";
                    //dgvFloorCheckOut.Columns[1].Name = "Items";
                    //dgvFloorCheckOut.Columns[2].Name = "Quantity";
                    //dgvFloorCheckOut.Columns[4].Name = "Productid";
                    //dgvFloorCheckOut.Columns[3].Name = "Location";
                    //dgvFloorCheckOut.Columns[5].Name = "Locationid";

                    dgvFloorCheckOut.Rows.Add();
                    dgvFloorCheckOut.Rows[i].Cells[0].Value = i + 1;
                    dgvFloorCheckOut.Rows[i].Cells["Items"].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvFloorCheckOut.Rows[i].Cells["Quantity"].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    dgvFloorCheckOut.Rows[i].Cells["Productid"].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);


         

                    dgvFloorCheckOut.Rows[i].Cells["Location"].Value = Convert.ToString(ds.Tables[1].Rows[i]["LocationName"]);
                    dgvFloorCheckOut.Rows[i].Cells["Locationid"].Value = Convert.ToString(ds.Tables[1].Rows[i]["Location"]);

                }

                //panel9.Enabled = false;
                //btnsave.Enabled = false;
            }
            else
            {
                dgvFloorCheckOut.Rows.Clear();
                Txtcheckoutcustomername.Text = string.Empty;
                Txtcheckrefno.Text = string.Empty;
                Txtcheckestno.Text = string.Empty;
            }

        }

        private void dgvNew_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvFloorCheckOut_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dgvFloorCheckOut.CurrentCell.RowIndex == dgvFloorCheckOut.Rows.Count - 1)
                {
                    if (e.KeyData == Keys.Enter)
                    {
                        btnsave.Focus();
                    }
                }
            }
            catch
            {

            }

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
                            cmd.CommandText = "GetIssued_Print";
                            cmd.Connection = con;
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(ds);

                            IssuedReceivedDal Obj = new IssuedReceivedDal();
                            Obj.dsMain = ds;
                            if (Obj.GenerateQuoation())
                            {
                                //frmPrintPreview objfrmpreview = new frmPrintPreview();
                                //objfrmpreview.fileName = Obj.fileName;
                                //objfrmpreview.Show();

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
        
    }
}

