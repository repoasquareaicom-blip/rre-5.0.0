using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Runtime.InteropServices;
using WarrantyDal;
using Inventory.Sales;
using System.Collections;

namespace Inventory.Movement
{
    public partial class FrmIssuedReceived : Form
    {
        public bool edit = false;
        DataTable dtitems;
        DataTable dtEstDet=new DataTable();
        public TextBox tb;
        public TextBox Quantitytomove1;
        string clickstatus = string.Empty;
        ComboBox cmblocation;
        string cas = string.Empty;
        int ProdSelRowvalue = 0;
        bool res = false;
        TextBox  tbamount, tbbaalanceanount, tborderquantoty, tbdeliveredqty;
        IssuedReceivedBAL objIssuedReceivedBAL = new IssuedReceivedBAL();
        string role1 = string.Empty;
        string srole = string.Empty;
        string mianid;
        string selectedtab = string.Empty;

        string IDFloorCheckout = string.Empty, IDPDI = string.Empty, IDDELIVERY=string.Empty;

        public FrmIssuedReceived()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            srole = Program.userid;
            if (srole == "1")
            {
                role1 = "Admin";
            }
            else
            {
                role1 = "Emp";
            }
            SearchCreteria2();
            SearchCreteria1();
            GetLocation();
            GetCustomers();
            LoadPorts();
           // LoadPortsFloorCheckOut();
            // BindsearchGrid();
            //Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //Txtitem.AutoCompleteCustomSource = AutoCompleteLoad();
            //Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;
           // Rbissued.Checked = true;
            ddlcustomers.Focus();
            itemdetails("");
            this.ActiveControl = ddlcustomers;
           // LoadPortsFloorCheckOut();
            LoadPortsChecking();
            LoadPortsDelivery();

            DataTable dt = Program.Dtmenu;
            bool contains = dt.AsEnumerable()
                .Any(row => "warrantyApproval" == row.Field<String>("Data"));

            //if(Program.userlevel!=1)
            //{
            if (contains == false)
            {
                MainTabSalesBill.TabPages.Remove(TabfloorApproval);
            }

            bool contains1 = dt.AsEnumerable()
                .Any(row => "warrantyFloorCheckout" == row.Field<String>("Data"));

            //if(Program.userlevel!=1)
            //{
            if (contains1 == false)
            {
                MainTabSalesBill.TabPages.Remove(TabFloorCheckOut);
            }



            bool contains2 = dt.AsEnumerable()
                .Any(row => "warrantyChecking" == row.Field<String>("Data"));

            //if(Program.userlevel!=1)
            //{
            if (contains2 == false)
            {
                MainTabSalesBill.TabPages.Remove(TabPDI);
            }

            bool contains3 = dt.AsEnumerable()
               .Any(row => "warrantyChecking" == row.Field<String>("Data"));

            //if(Program.userlevel!=1)
            //{
            if (contains3 == false)
            {
                MainTabSalesBill.TabPages.Remove(TabDelivery);
            }

        }
        private void LoadApproval()
        {
            DgvApproval.Rows.Clear();
            DgvApproval.ColumnCount = 6;


            DgvApproval.Columns[0].Name = "S.NO";
            DgvApproval.Columns[1].Name = "Items";
            DgvApproval.Columns[2].Name = "Quantity";
            DgvApproval.Columns[3].Name = "Remarks";
            DgvApproval.Columns[4].Name = "Status";
            DgvApproval.Columns[5].Name = "ProductID";





            this.DgvApproval.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      
            this.DgvApproval.Columns[5].Visible = false;


            this.DgvApproval.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.DgvApproval.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.DgvApproval.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.DgvApproval.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.DgvApproval.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.DgvApproval.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
          


            this.DgvApproval.Columns[0].ReadOnly = true;
            this.DgvApproval.Columns[1].ReadOnly = true;
            this.DgvApproval.Columns[2].ReadOnly = true;
            this.DgvApproval.Columns[3].ReadOnly = true;

         

            this.DgvApproval.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.DgvApproval.Columns[0].Width = 50;

            this.DgvApproval.Columns[3].Width = 100;
            this.DgvApproval.Columns[4].Width = 150;
            this.DgvApproval.Columns[1].Width = 200;
            this.DgvApproval.Columns[2].Width = 100;




            DgvApproval.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in DgvApproval.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }

        #region FloorCheckOut
        //private void LoadPortsFloorCheckOut()
        //{
        //    dgvFloorCheckOut.Rows.Clear();
        //    dgvFloorCheckOut.ColumnCount = 6;


        //    dgvFloorCheckOut.Columns[0].Name = "S.NO";
        //    dgvFloorCheckOut.Columns[1].Name = "Items";
        //    dgvFloorCheckOut.Columns[2].Name = "Quantity";
        //    dgvFloorCheckOut.Columns[5].Name = "Productid";
        //    dgvFloorCheckOut.Columns[4].Name = "Location";          
        //    dgvFloorCheckOut.Columns[3].Name = "Rack";

        //    dgvFloorCheckOut.Columns[5].Visible = false;
        //    this.dgvFloorCheckOut.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


         

        //    this.dgvFloorCheckOut.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    this.dgvFloorCheckOut.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    this.dgvFloorCheckOut.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    this.dgvFloorCheckOut.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    this.dgvFloorCheckOut.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //    this.dgvFloorCheckOut.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



        //    this.dgvFloorCheckOut.Columns[0].ReadOnly = true;
        //    this.dgvFloorCheckOut.Columns[1].ReadOnly = true;
        //    this.dgvFloorCheckOut.Columns[2].ReadOnly = true;
        //    this.dgvFloorCheckOut.Columns[3].ReadOnly = true;



        //    this.dgvFloorCheckOut.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



        //    //DataGridViewComboBoxColumn name = new DataGridViewComboBoxColumn();
        //    //name.HeaderText = "Location";
        //    //name.DataPropertyName = "Location";
        //    //name.FlatStyle = FlatStyle.Popup;
        //    //dgvFloorCheckOut.Columns.Insert(3, name);
        //    ////this.dgvFloorCheckOut.Columns[3].Width = 150;


        //    DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
        //    dgvCmb.ValueType = typeof(bool);
        //    dgvCmb.Name = "ChkFloorCheckOut";
        //    dgvCmb.HeaderText = "IsCheckOut";
        //    dgvCmb.FlatStyle = FlatStyle.Popup;

        //    dgvFloorCheckOut.Columns.Insert(6, dgvCmb);


        //    Rectangle resolution = Screen.PrimaryScreen.Bounds;
        //    int w = resolution.Width;
        //    int h = resolution.Height;

        //    if (w == 1024 && h == 768)
        //    {
        //        this.dgvFloorCheckOut.Columns[0].Width = 45;
        //        this.dgvFloorCheckOut.Columns[1].Width = 300;
        //        this.dgvFloorCheckOut.Columns[2].Width = 100;
        //        this.dgvFloorCheckOut.Columns[3].Width = 100;
        //        this.dgvFloorCheckOut.Columns["Rack"].Width = 250;
        //        this.dgvFloorCheckOut.Columns[5].Width = 80;

        //    }
        //    else
        //    {
        //        this.dgvFloorCheckOut.Columns[0].Width = 45;
        //        this.dgvFloorCheckOut.Columns[1].Width = 300;
        //        this.dgvFloorCheckOut.Columns[2].Width = 110;
        //        this.dgvFloorCheckOut.Columns[3].Width = 110;

        //        this.dgvFloorCheckOut.Columns[5].Width = 80;

        //    }
        //    dgvFloorCheckOut.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //    foreach (DataGridViewColumn c in dgvFloorCheckOut.Columns)
        //    {
        //        c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
        //    }
        //}



        private void LoadPortsFloorCheckOut()
        {
            dgvFloorCheckOut.Rows.Clear();
            dgvFloorCheckOut.ColumnCount = 5;


            dgvFloorCheckOut.Columns[0].Name = "S.NO";
            dgvFloorCheckOut.Columns[1].Name = "Items";
            dgvFloorCheckOut.Columns[2].Name = "Quantity";
            dgvFloorCheckOut.Columns[4].Name = "Productid";
            //dgvFloorCheckOut.Columns[6].Name = "Productid";
            dgvFloorCheckOut.Columns[3].Name = "Rack";


            this.dgvFloorCheckOut.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvFloorCheckOut.Columns[0].Width = 50;

            this.dgvFloorCheckOut.Columns[4].Visible = false;

            this.dgvFloorCheckOut.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvFloorCheckOut.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvFloorCheckOut.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvFloorCheckOut.Columns[1].Width = 400;


            this.dgvFloorCheckOut.Columns[0].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[1].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[2].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[3].ReadOnly = true;

            this.dgvFloorCheckOut.Columns[3].Width = 100;

            this.dgvFloorCheckOut.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvFloorCheckOut.Columns[2].Width = 100;


            DataGridViewComboBoxColumn name = new DataGridViewComboBoxColumn();
            name.HeaderText = "Location";
            name.DataPropertyName = "Location";
            name.FlatStyle = FlatStyle.Popup;



           
            
            dgvFloorCheckOut.Columns.Insert(3, name);
            //this.dgvFloorCheckOut.Columns[3].Width = 150;


            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
           
            dgvCmb.Name = "ChkFloorCheckOut";
            dgvCmb.HeaderText = "IsCheckOut";
            dgvCmb.FlatStyle = FlatStyle.Popup;

            dgvFloorCheckOut.Columns.Insert(6, dgvCmb);
            this.dgvFloorCheckOut.Columns[6].Width = 90;
            


            dgvFloorCheckOut.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvFloorCheckOut.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }
        #endregion

        #region PDI
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
        #endregion

        #region Delivery
        private void LoadPortsDelivery()
        {
            dgvDelivery.Rows.Clear();
            dgvDelivery.ColumnCount = 4;


            dgvDelivery.Columns[0].Name = "S.NO";
            dgvDelivery.Columns[1].Name = "Items";
            dgvDelivery.Columns[2].Name = "Quantity";
            dgvDelivery.Columns[3].Name = "Productid";          


            this.dgvDelivery.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            
            dgvDelivery.Columns[3].Visible = false;
           // dgvDelivery.Columns[5].Visible = false;
            this.dgvDelivery.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvDelivery.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvDelivery.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvDelivery.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvDelivery.Columns[0].ReadOnly = true;
            this.dgvDelivery.Columns[1].ReadOnly = true;
            this.dgvDelivery.Columns[2].ReadOnly = true;
            this.dgvDelivery.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvDelivery.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;     




            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "ChkDelivery";
            dgvCmb.HeaderText = "Delivered";
            dgvDelivery.Columns.Insert(3, dgvCmb);
            this.dgvDelivery.Columns[3].Width = 60;
           

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvDelivery.Columns[0].Width = 40;
                this.dgvDelivery.Columns[1].Width = 250;
                this.dgvDelivery.Columns[2].Width = 70;
                this.dgvDelivery.Columns[3].Width = 120;
          
            }
            else
            {
                this.dgvDelivery.Columns[0].Width = 30;
                this.dgvDelivery.Columns[1].Width = 200;
                this.dgvDelivery.Columns[2].Width = 80;
                this.dgvDelivery.Columns[3].Width = 110;          

            }
            dgvDelivery.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvDelivery.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }
        #endregion

        #region bind
        public void bindsearchgrid()
        {
            DataTable dt=IssuedReceivedBAL.BindsearchGrid();
            Dgvmovementsearch.DataSource = dt;


            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

        }
        private void LoadPorts()
        {
            dgvOrder.Rows.Clear();

            //dgvOrder.Rows.Add(1);


            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
       

            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
       
            this.dgvOrder.Columns[1].ReadOnly = true;

            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
           // this.dgvOrder.Columns[2].Width = 50;
             Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvOrder.Columns[0].Width = 60;
                this.dgvOrder.Columns[2].Width = 120;
                this.dgvOrder.Columns[3].Width = 150;
                this.dgvOrder.Columns[4].Width = 200;
                this.dgvOrder.Columns[1].Width = 190;
            }
            else
            {
                this.dgvOrder.Columns[0].Width = 60;
                this.dgvOrder.Columns[2].Width = 60;
                this.dgvOrder.Columns[3].Width = 150;
                this.dgvOrder.Columns[4].Width = 200;
                this.dgvOrder.Columns[1].Width = 250;
            }
            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvOrder.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvOrder.AlternatingRowsDefaultCellStyle.BackColor = Color.White;




            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }



        }
        public void GetLocation()
        {
            //DataTable dtlocation = ProductMovementBal.GetLocation();
            //DataRow row = dtlocation.NewRow();
            //row["LocationID"] = "0";
            //row["LocationName"] = "--Select--";
            //dtlocation.Rows.InsertAt(row, 0);
            //ddlLocation.DataSource = dtlocation;
            //ddlLocation.ValueMember = "LocationID";
            //ddlLocation.DisplayMember = "LocationName";
            //ddlLocation.SelectedIndex = 1;

            //cmbflrlocation.DataSource = dtlocation;
            //cmbflrlocation.ValueMember = "LocationID";
            //cmbflrlocation.DisplayMember = "LocationName";
            //cmbflrlocation.SelectedIndex = 1;

        }

        public void GetCustomers()
        {
            DataTable dtlocation = ProductMovementBal.GetCustomers();
            DataRow row = dtlocation.NewRow();

            row["CustomerID"] = "0";
            row["Name"] = "--Select--";
            dtlocation.Rows.InsertAt(row, 0);
            ddlcustomers.DataSource = dtlocation;
            ddlcustomers.ValueMember = "CustomerID";
            ddlcustomers.DisplayMember = "Name";
            ddlcustomers.SelectedIndex = 0;

            //cmbflrcustomer.DataSource = dtlocation;
            //cmbflrcustomer.ValueMember = "CustomerID";
            //cmbflrcustomer.DisplayMember = "Name";
            //cmbflrcustomer.SelectedIndex = 0;

        }

        #endregion

        #region search
        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("EnteredBy");
            search.Add("EnteredOn");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchMovedBy.DataSource = bs.DataSource;
            cbxSearchMovedBy.SelectedIndex = 0;
            cmbstatus1.Visible = false;
        }
        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("EnteredBy");
            search.Add("EnteredOn");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderDate.DataSource = bs.DataSource;
            cbxSearchOrderDate.SelectedIndex = 1;
            cmbstatus2.Visible = false;
        }
        private void cbxSearchMovedBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchMovedBy.SelectedIndex == 2)
            {
                cmbstatus1.Visible = true;
                txtsearch1.Visible = false;
                ListSearchDate1.Visible = false;

            }
            else if (cbxSearchMovedBy.SelectedIndex == 1)
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
        private void btnSearch_Click(object sender, EventArgs e)
        {   
              if ((cbxSearchMovedBy.SelectedIndex == cbxSearchOrderDate.SelectedIndex))
            {
                MessageBox.Show("*Search a item Should Not Be Same");
            }
            else
            {
                string firstname = string.Empty, firstvalue = string.Empty, secondname = string.Empty, secondvalue = string.Empty;
                string firstname1 = string.Empty, firstvalue1 = string.Empty, secondname1 = string.Empty, secondvalue1 = string.Empty;
                firstname = cbxSearchMovedBy.Text.Trim();
                if (firstname == "EnteredBy")
                {
                    firstname = "EnteredBy";
                    //firstvalue = Convert.ToString(cmbstatus1.SelectedValue);
                    if (!string.IsNullOrEmpty(txtsearch1.Text))
                    {
                        firstvalue = txtsearch1.Text;
                    }
                }
                else
                {
                    //firstvalue = txtsearch1.Text.Trim();
                    string part1 = txtsearch1.Text.Trim();
                    if (txtsearch1.Text.Trim() != "All")
                    {
                        part1 = txtsearch1.Text.Trim();
                    }
                    else
                    {
                        part1 = string.Empty;
                        firstvalue = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(part1))
                    {
                        string part = part1.Substring(0, 1);
                        if (Char.IsDigit(part, 0))
                        {
                            string[] rr = txtsearch1.Text.Split('-');
                            firstvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                        }
                        else
                        {
                            firstvalue = txtsearch1.Text.Trim();
                        }
                    }
                }

                secondname = cbxSearchOrderDate.Text.Trim();
                if (secondname == "EnteredBy")
                {
                    secondname = "EnteredBy";
                    //secondvalue = Convert.ToString(cmbstatus2.SelectedValue);
                    if (!string.IsNullOrEmpty(cmbstatus2.Text))
                    {
                        secondvalue = cmbstatus2.Text;
                    }
                }
                else
                {
                    //secondvalue = txtsearch2.Text.Trim();
                    string part1 = txtsearch2.Text.Trim();
                    if (txtsearch2.Text.Trim() != "All")
                    {
                        part1 = txtsearch2.Text.Trim();
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
                            string[] rr = txtsearch2.Text.Split('-');
                            secondvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                        }
                        else
                        {
                            secondvalue = txtsearch2.Text.Trim();
                        }
                    }
                }


                if (firstname == "EnteredBy")
                {
                    firstname1 = "us.UserFullName";
                    firstvalue1 = firstvalue;
                }
                else if (firstname == "EnteredOn")
                {
                    secondname1 = "h.EnteredOn";
                    secondvalue1 = firstvalue;
                }


                if (secondname == "EnteredBy")
                {
                    firstname1 = "us.UserFullName";
                    firstvalue1 = secondvalue;
                }
                else if (secondname == "EnteredOn")
                {
                    secondname1 = "h.EnteredOn";
                    secondvalue1 = secondvalue;
                }

                string select = MainTabSalesBill.SelectedTab.Name;

               

                if (selectedtab == "TabNew" || selectedtab == "TabfloorApproval")
                {
                search(firstname1, firstvalue1, secondname1, secondvalue1, role1, Program.userid);
                }
                if (selectedtab == "TabFloorCheckOut")
                {
                    searchFloorCheckOut(firstname1, firstvalue1, secondname1, secondvalue1, role1, Program.userid);
                }
                if (selectedtab == "TabPDI")
                {
                    searchPDI(firstname1, firstvalue1, secondname1, secondvalue1, role1, Program.userid);
                }
                if (selectedtab == "TabDelivery")
                {
                    searchDelivery(firstname1, firstvalue1, secondname1, secondvalue1, role1, Program.userid);
                }
            }
        
        }


        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string role, string UserId)
        {
            IssuedReceivedBAL obj = new IssuedReceivedBAL();
            DataTable dt = obj.searchFrmIssuedReceived(firstname, firstvalue, secondname, secondvalue, role1, Program.UserName);
            Dgvmovementsearch.Columns.Clear();
            Dgvmovementsearch.DataSource = dt;
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            Dgvmovementsearch.Columns["EnteredOn"].HeaderText = "Entered On";
            Dgvmovementsearch.Columns["EnteredBy"].HeaderText = "Entered By";
            Dgvmovementsearch.Columns["ReferenceNo"].HeaderText = "Reference No";
            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            Dgvmovementsearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            Dgvmovementsearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public void searchFloorCheckOut(string firstname, string firstvalue, string secondname, string secondvalue, string role, string UserId)
        {
            IssuedReceivedBAL obj = new IssuedReceivedBAL();
            DataTable dt = obj.searchFrmIssuedReceivedApproved(firstname, firstvalue, secondname, secondvalue, role1, Program.UserName);
            Dgvmovementsearch.Columns.Clear();
            Dgvmovementsearch.DataSource = dt;
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            Dgvmovementsearch.Columns["EnteredOn"].HeaderText = "Entered On";
            Dgvmovementsearch.Columns["EnteredBy"].HeaderText = "Entered By";
            Dgvmovementsearch.Columns["ReferenceNo"].HeaderText = "Reference No";
            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            Dgvmovementsearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            Dgvmovementsearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public void searchPDI(string firstname, string firstvalue, string secondname, string secondvalue, string role, string UserId)
        {
            IssuedReceivedBAL obj = new IssuedReceivedBAL();
            DataTable dt = obj.searchFrmIssuedFloorcheckout(firstname, firstvalue, secondname, secondvalue, role1, Program.UserName);
            Dgvmovementsearch.Columns.Clear();
            Dgvmovementsearch.DataSource = dt;
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            Dgvmovementsearch.Columns["EnteredOn"].HeaderText = "Entered On";
            Dgvmovementsearch.Columns["EnteredBy"].HeaderText = "Entered By";
            Dgvmovementsearch.Columns["ReferenceNo"].HeaderText = "Reference No";
            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            Dgvmovementsearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            Dgvmovementsearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public void searchDelivery(string firstname, string firstvalue, string secondname, string secondvalue, string role, string UserId)
        {
            IssuedReceivedBAL obj = new IssuedReceivedBAL();
            DataTable dt = obj.SearchFrmIssuedPDI(firstname, firstvalue, secondname, secondvalue, role1, Program.UserName);
            Dgvmovementsearch.Columns.Clear();
            Dgvmovementsearch.DataSource = dt;
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            Dgvmovementsearch.Columns["EnteredOn"].HeaderText = "Entered On";
            Dgvmovementsearch.Columns["EnteredBy"].HeaderText = "Entered By";
            Dgvmovementsearch.Columns["ReferenceNo"].HeaderText = "Reference No";
            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            Dgvmovementsearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            Dgvmovementsearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
       


        private void txtsearch1_Click(object sender, EventArgs e)
        {
            string selecteditem = cbxSearchMovedBy.Text.ToString();
            if (selecteditem == "Date")
            {
                clickstatus = "search1";
                pnlCalender.BringToFront();
                pnlCalender.Visible = true;
                pnlCalender.Location = new Point(133, 54);
            }
            else
            {
                pnlCalender.Visible = false;
            }
        }
        private void txtsearch2_Click(object sender, EventArgs e)
        {
            if (cbxSearchOrderDate.Text.ToString() == "Date")
            {
                clickstatus = "search2";
                pnlCalender.BringToFront();
                pnlCalender.Visible = true;
                pnlCalender.Location = new Point(133, 79);
            }
            else
            {
                pnlCalender.Visible = false;
            }
        }
        private void panel3_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblAll.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblAll.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblAll_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblAll.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblAll.Text.Trim();
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

            pnlCalender.Visible = false;
        }
        #endregion
        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

          
            if (ddlcustomers.Text == "--Select--")
            {
                i++;
                message = message + "* Please Select from customers" + "\n";
                if (i == 1)
                    this.ActiveControl = ddlcustomers;
            }

            //string val = Convert.ToString(dgvOrder[1, 0].Value);
            //if (string.IsNullOrEmpty(val))
            //{
            //    i++;
            //    message = message + "* Product Should Not Be Empty" + "\n";
            //    if (i == 1)
            //        dgvOrder.CurrentCell = dgvOrder.Rows[0].Cells[1];
            //}


            if (dgvOrder.Rows.Count == 0)
            {
                i++;
                message = message + "* Please Select Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }
            else if (dgvOrder.Rows.Count == 1)
            {
                string Items = Convert.ToString(dgvOrder.Rows[0].Cells["Items"].Value);
                string Received = Convert.ToString(dgvOrder.Rows[0].Cells["Quantity"].Value);
                if (string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received)))
                {
                    i++;
                    message = message + "* Please Select Product" + "\n";
                    if (i == 1)
                        this.ActiveControl = dgvOrder;
                }
            }


            foreach (DataGridViewRow row in dgvOrder.Rows)
            {
                string Items = Convert.ToString(row.Cells["Items"].Value);
                string Received = Convert.ToString(row.Cells["Quantity"].Value);

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

                    if ((string.IsNullOrEmpty(Received)) || Received == "." || Convert.ToDouble(Received) == 0)
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
            return status;
        }
        #region Buttons
        private void btnsave_Click(object sender, EventArgs e)
        {
          

            string select = MainTabSalesBill.SelectedTab.Name;

            if (select == "TabNew")
            {
                if (Validation())
                {
                    save();
                   
                }
            }
            if (select == "TabfloorApproval")
            {
                if (ValidationApprovel())
                {
                    SaveApproval();
                    search("us.UserFullName", "", "h.EnteredOn", "Today", role1, Program.userid);
                    clear();
                }


            }
            if (select == "TabFloorCheckOut")
            {
                
                if (Validationcheckout())
                {
                    SavecheckFloorCheckout();
                    searchFloorCheckOut("us.UserFullName", "", "h.EnteredOn", "Today", role1, Program.userid);
                    clear();
                }
                else
                {
                    MessageBox.Show("Please Check All Items");
                }

            }
            if (select == "TabPDI")
            {
                if (Validationchecking())
                {
                    SavePDI();
                    searchPDI("us.UserFullName", "", "h.EnteredOn", "Today", role1, Program.userid);
                    clear();
                }
                

            }
            if (select == "TabDelivery")
            {
                if (ValidationDelivery())
                {
                    SavecheckDelivery();
                    searchDelivery("us.UserFullName", "", "h.EnteredOn", "Today", role1, Program.userid);
                    clear();
                }
                else
                {
                    MessageBox.Show("Please Check All Items");
                }


            }
           
            
        }
        public void SaveApproval()
        {


            DataTable dt = new DataTable();
            dt.Columns.Add("Productid", typeof(int));
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("Rack", typeof(int));
            for (int i = 0; i < DgvApproval.Rows.Count; i++)
            {

                if (!string.IsNullOrEmpty(Convert.ToString(DgvApproval.Rows[i].Cells["Items"].Value)))
                {
                    DataRow dr = dt.NewRow();
                    dr["Productid"] = DgvApproval.Rows[i].Cells["ProductID"].Value.ToString();
                    dr["Quantity"] = DgvApproval.Rows[i].Cells["Quantity"].Value.ToString();
                    dr["Rack"] =0;
                    dt.Rows.Add(dr);
                }

            }
            IssuedReceivedBAL objIssuedReceivedBAL = new IssuedReceivedBAL();
            objIssuedReceivedBAL.MainID = lblWRHIDAvl.Text;
            if (cbxStatus.SelectedIndex == 1)
            {
                objIssuedReceivedBAL.status = "Issued Approved";
            }
            else
            {
                objIssuedReceivedBAL.status = "Issued Rejected";
            }
            string res = IssuedReceivedBAL.SaveWRApproval(objIssuedReceivedBAL,dt);


        }
        private bool ValidationApprovel()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (DgvApproval.Rows.Count > 0)
            {



                string val = Convert.ToString(DgvApproval[1, 0].Value);
                if (string.IsNullOrEmpty(val))
                {
                    i++;
                    message = message + "* Please Select Request To Approve" + "\n";
                    if (i == 1)
                        Dgvmovementsearch.Focus();
                }
            }
            else
            {
                i++;
                message = message + "* Please Select Request To Approve" + "\n";
                if (i == 1)
                    Dgvmovementsearch.Focus();
            }

            if ((cbxStatus.SelectedIndex == 0))
            {
                i++;
                message = message + "* Please Select Status" + "\n";
                if (i == 1)
                    cbxStatus.Focus();
            }





            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }
        public void SavecheckFloorCheckout()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Productid", typeof(int));
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("Rack", typeof(int));
            for (int i = 0; i < dgvFloorCheckOut.Rows.Count; i++)
            {

                if (!string.IsNullOrEmpty(Convert.ToString(dgvFloorCheckOut.Rows[i].Cells["Items"].Value)))
                {
                    DataRow dr = dt.NewRow();
                    dr["Productid"] = dgvFloorCheckOut.Rows[i].Cells["Productid"].Value.ToString();
                    dr["Quantity"] = dgvFloorCheckOut.Rows[i].Cells["Quantity"].Value.ToString();
                    dr["Rack"] = dgvFloorCheckOut.Rows[i].Cells[3].Value.ToString(); 
                    dt.Rows.Add(dr);
                }

            }
            string res = IssuedReceivedBAL.saveIssuedFloorcheckout(IDFloorCheckout,dt);


        }

        public void SavecheckDelivery()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Productid", typeof(int));
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("Rack", typeof(string));

            for (int i = 0; i < dgvDelivery.Rows.Count; i++)
            {

                if (!string.IsNullOrEmpty(Convert.ToString(dgvDelivery.Rows[i].Cells["Items"].Value)))
                {
                    DataRow dr = dt.NewRow();
                    dr["Productid"] = dgvDelivery.Rows[i].Cells["Productid"].Value.ToString();
                    dr["Quantity"] = dgvDelivery.Rows[i].Cells["Quantity"].Value.ToString();
                    dr["Rack"] = "0";
                    dt.Rows.Add(dr);
                }

            }

            string res = IssuedReceivedBAL.saveIssuedFloorDelivery(IDDELIVERY, dt);


        }

        public void SavePDI()
        {
            DataTable dt = DataGridView2DataTable(dgvChecking);
            dt.Columns.RemoveAt(0);
            dt.Columns.RemoveAt(0);
            dt.Columns["Quantity"].ColumnName = "Rack";
            dt.AcceptChanges();
            dt.Columns["originalQuantity"].ColumnName = "Quantity";

            dt.AcceptChanges();

           string res=IssuedReceivedBAL.saveIssuedPDI(IDPDI,Program.userid, dt);
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
                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][1])) || string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][3])))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }

        //private bool Validationcheckout()
        //{
        //    bool status = true;
        //    string message = "";
        //    int i = 0;

        //    foreach (DataGridViewRow row in DgvApproval.Rows)
        //    {
        //        DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];
        //        if (Convert.ToBoolean(CbxCell.Value) == false)
        //        {
        //            status = false;
        //            break;
        //        }
        //    }
        //    return status;
        //}
        private void btnclear_Click(object sender, EventArgs e)
        {
            clear();
        }
        public void clear()
        {
            string select = MainTabSalesBill.SelectedTab.Name;


             if (select == "TabNew")
            {
                //ddlLocation.SelectedIndex = 1;
                GetCustomers();
                ddlcustomers.SelectedIndex = 0;
                cmbEstmationID.Text = "";
                txtphoneno.Text = "";

                //   txtrefno.Text = string.Empty;
                //  txtRemarks.Text = string.Empty;
                //  Rbissued.Checked = true;
                LoadPorts();
                //   this.ActiveControl = ddlLocation;
                HidLblMain.Text = string.Empty;
                ddlcustomers.Focus();
                try
                {
                    cmbEstmationID.DataSource = "";
                    txtphoneno.Text = "";
                }
                catch
                {
                    cmbEstmationID.Text = "";
                    txtphoneno.Text = "";
                }
            }
            if (select == "TabfloorApproval")
            {
                clearFloor();
            }
            if (select == "TabFloorCheckOut")
            {
                txtFlrChkOutCustomerName.Text = "";
                txtFlrChkOutEstimationNo.Text = "";
                txtFlrChkOutPhoneNo.Text = "";
                dgvFloorCheckOut.Rows.Clear();
                dgvFloorCheckOut.Columns.Clear();
                LoadPortsFloorCheckOut();
            }
            if (select == "TabPDI")
            {
                txtPDICustomerName.Text = "";
                txtPDIEstimationNo.Text = "";
                txtPDIPhoneNo.Text = "";
                dgvChecking.Rows.Clear();
                dgvChecking.Columns.Clear();
                
                LoadPortsChecking();
            }
            if (select == "TabDelivery")
            {
                txtDeliveryCustomerName.Text = "";
                txtDeliveryEstimationNo.Text = "";
                txtDeliveryPhoneNo.Text = "";
                dgvDelivery.Rows.Clear();
              //  dgvDelivery.Columns.Clear();
                
                LoadPortsDelivery();
            }
           

        }
        public void clearFloor()
        {

            GetCustomers();
            txtAvlCusname.Text = "";
            txtAvlEstNo.Text = "";
            txtAvlPhNo.Text = "";
            lblWRHIDAvl.Text = "";
            DgvApproval.Rows.Clear();
            DgvApproval.Columns.Clear();
            LoadApproval();
            HidLblMain.Text = string.Empty;
            cbxStatus.SelectedIndex = 0;
        }
        private void btnnew_Click(object sender, EventArgs e)
        {
            clear();
        }
        #endregion
        public void save()
        {
            IssuedReceivedBAL objIssuedReceivedBAL = new IssuedReceivedBAL();
            if (Convert.ToInt32(ddlcustomers.SelectedValue) > 0)
            {
                objIssuedReceivedBAL.cusid = Convert.ToString(ddlcustomers.SelectedValue);
            }
            else
            {
                objIssuedReceivedBAL.cusid = null;
            }
            objIssuedReceivedBAL.Cusname = Convert.ToString(ddlcustomers.Text);

            if (Convert.ToInt32(cmbEstmationID.SelectedIndex) > 0)
            {
                objIssuedReceivedBAL.EstimationID = Convert.ToString(cmbEstmationID.Text);
            }
            else
            {
                objIssuedReceivedBAL.EstimationID = null;
            }

            objIssuedReceivedBAL.Phno = txtphoneno.Text;


            if (string.IsNullOrEmpty(HidLblMain.Text))
            {
                objIssuedReceivedBAL.MainID = String.Empty;
            }
            else
            {
                objIssuedReceivedBAL.MainID = HidLblMain.Text;
            }
            objIssuedReceivedBAL.Movedby = Program.userid;

            DataTable dt = DataGridView2DataTable(dgvOrder);

        

             bool dtval = RemoveDuplicateRows(dt, "ProductId");


             if (dtval)
             {

                 HidLblMain.Text = IssuedReceivedBAL.SaveWRHeaderNew(objIssuedReceivedBAL);



                 for (int i = 0; i < dgvOrder.Rows.Count; i++)
                 {
                     if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[1].Value)))
                     {
                         if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[7].Value)))
                         {
                             objIssuedReceivedBAL.Subid = Convert.ToString(dgvOrder.Rows[i].Cells[7].Value);
                         }
                         else
                         {
                             objIssuedReceivedBAL.Subid = string.Empty;
                         }


                         objIssuedReceivedBAL.ProductID = Convert.ToString(dgvOrder.Rows[i].Cells[6].Value);
                         objIssuedReceivedBAL.Quantity = Convert.ToString(dgvOrder.Rows[i].Cells[2].Value);
                         objIssuedReceivedBAL.Remarks = Convert.ToString(dgvOrder.Rows[i].Cells[3].Value);
                         objIssuedReceivedBAL.status = Convert.ToString(dgvOrder.Rows[i].Cells[4].Value);


                         objIssuedReceivedBAL.MainID = HidLblMain.Text;
                         string subid = IssuedReceivedBAL.SaveWRDetails(objIssuedReceivedBAL);

                         dgvOrder.Rows[i].Cells[7].Value = subid;
                     }
                 }

                 GetReport(HidLblMain.Text);
                 search("us.UserFullName", "", "h.EnteredOn", "Today", role1, Program.UserName);
                 clear();

             }
             else
             {
                 MessageBox.Show("Please Remove Duplication Product");
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
        private void Txtitem_KeyDown(object sender, KeyEventArgs e)
        {

            //if (e.KeyData == Keys.Enter)
            //{

            //    if (!string.IsNullOrEmpty(lblproductid.Text) && lblproductid.Text != "0" && !string.IsNullOrEmpty(lblitemname.Text) && lblitemname.Text != "0")
            //    {
            //        int rowindex = Convert.ToInt32(lblrowindex.Text);

            //        dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
            //        dgvOrder.Rows[rowindex].Cells[1].Value = lblitemname.Text.ToUpper();
            //        dgvOrder.Rows[rowindex].Cells[2].Value = 1;

            //        dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
            //        pnsearch.Visible = false;
            //        lblproductid.Text = string.Empty;
            //        Txtitem.Text = string.Empty;
            //        lblitemcode.Text = "0";

            //        lbldisplay.Text = "0";
            //        lbldemo.Text = "0";
            //        lblservice.Text = "0";
            //        lblitemname.Text = string.Empty;
            //        lblrack.Text = "0";
            //        edit = false;
            //        dgvOrder.Focus();
            //        dgvOrder.ClearSelection();
            //        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Selected = true;
            //        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
            //        dgvOrder.BeginEdit(true);
            //        edit = true;


            //    }
            //    else if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value)))
            //    {
            //        MessageBox.Show("Please Enter Product Name");
            //        dgvOrder.Focus();
            //        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];
            //        Txtitem.Focus();
            //    }
            //    else
            //    {
            //        dgvOrder.Focus();
            //        dgvOrder.ClearSelection();
            //        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Selected = true;
            //        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
            //        dgvOrder.BeginEdit(true);
            //        edit = true;

            //    }



            //}
        }
        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            
        }


        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string headerText = dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].HeaderText;
            try
            {



                if (headerText == "Quantity")
                {

                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[e.RowIndex].Cells["adyqty"].Value)))
                    {
                        if (Convert.ToDecimal(dgvOrder.Rows[e.RowIndex].Cells["adyqty"].Value) < Convert.ToDecimal(Quantitytomove1.Text))
                        {
                            MessageBox.Show("Quantity Is Greater Than Estimation");
                            dgvOrder.Focus();
                            edit = true;
                            dgvOrder.Rows[e.RowIndex].Cells[2].Value = "";
                            dgvOrder.CurrentCell = dgvOrder[2, e.RowIndex];
                            Quantitytomove1.Text = "0";
                            decimal quantity1;
                            if (Convert.ToString(Quantitytomove1.Text) == "" || Convert.ToDecimal(Quantitytomove1.Text) == 0)
                            {
                                quantity1 = 0;
                            }
                            else
                            {

                                quantity1 = Convert.ToDecimal(Quantitytomove1.Text);
                            }
                            dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex].Value = quantity1;
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
                            //dgvOrder.BeginEdit(true);
                        }
                        else
                        {
                            decimal quantity1;
                            if (Convert.ToString(Quantitytomove1.Text) == "" || Convert.ToDecimal(Quantitytomove1.Text) == 0)
                            {
                                quantity1 = 0;
                            }
                            else
                            {

                                quantity1 = Convert.ToDecimal(Quantitytomove1.Text);
                            }
                            dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex].Value = quantity1;
                            edit = true;
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3];
                            //dgvOrder.BeginEdit(true);
                        }

                    }
                    else
                    {
                        decimal quantity1;
                        if (Convert.ToString(Quantitytomove1.Text) == "" || Convert.ToDecimal(Quantitytomove1.Text) == 0)
                        {
                            quantity1 = 0;
                        }
                        else
                        {

                            quantity1 = Convert.ToDecimal(Quantitytomove1.Text);
                        }

                        string dsa = Convert.ToString(dgvOrder.Rows[e.RowIndex].Cells[1].Value);
                        if (!string.IsNullOrEmpty(dsa))
                        {
                            dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex].Value = quantity1;
                        }
                       
                        edit = true;
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3];
                        //dgvOrder.BeginEdit(true);


                        //edit = true;
                    }


                }

                else if (e.ColumnIndex == 3)
                {

                    dgvOrder.Focus();

                   // dgvOrder.Rows.Add(1);
                    dgvOrder.Focus();

                    edit = false;
                    dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                    dgvOrder.Focus();
                    dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1].Selected = true;

                }
            }
            catch
            {

            }
        }

        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
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
                else
                {
                    pnsearch.Visible = false;
                }



            }
            catch
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

        #region Panel
        protected void Quantitytomove1_press(object sender, KeyPressEventArgs e)
        {
            string headerText = dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].HeaderText;
            if (headerText == "Quantity")
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
                //if (!(char.IsDigit(e.KeyChar)))
                //{
                //    if (e.KeyChar != '\b')
                //    {
                //        e.Handled = true;
                //    }
                //}
            }
        }
        //public AutoCompleteStringCollection AutoCompleteLoad()
        //{
        //    AutoCompleteStringCollection str = new AutoCompleteStringCollection();
        //    DataTable st = ProductMovementBal.itemauto();
        //    string[] arr = new string[st.Rows.Count];
        //    for (int i = 0; i < st.Rows.Count; i++)
        //    {
        //        arr[i] = st.Rows[i]["DisplayName"].ToString();
        //    }
        //    for (int i = 0; i < arr.Length; i++)
        //    {
        //        //var combined = string.Join(", ", arr);
        //        var combined = arr[i];
        //        str.Add(combined);
        //    }
        //    //for (int i = 0; i < arr.Length; i++)
        //    //{
        //   // var combined = string.Join(", ", arr);
        //    //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
        // //   str.Add(combined);
        //    //}

        //    //for (int i = 0; i < st.Rows.Count; i++)
        //    //{
        //    //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
        //    //    str.Add(combined);
        //    //}

        //    return str;
        //}

        #endregion
        #region Processcmd
        [DllImport("user32.dll")]
        internal static extern IntPtr GetFocus();
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
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




            if(keyData==Keys.Tab)
            {
                if (ddlcustomers.Focused)
                {
                    txtphoneno.Focus();
                    return true;
                }
            }
            if (keyData == (Keys.S | Keys.Control))
            {
                btnsave.PerformClick();


                return true;
            }

            else if (keyData == (Keys.C | Keys.Alt))
            {

                clear();

                return true;
            }
            if (keyData == Keys.Escape)
            {
                if (pnsearch.Visible)
                {
                    pnsearch.Visible = false;
                    //dgvOrder.Focus();
                    //dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
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


   

               

            if (cmbEstmationID.Focused)
            {
                if (keyData == (Keys.Tab))
                {

                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[1, 0];

                    return true;
                }
            }
           
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
        private void textbox_Change(object sender, EventArgs e)
        {

            if (dgvOrder.CurrentCell.ColumnIndex == 0)
            {

            }
        }
        private void dgvOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvOrder.CurrentCell.ColumnIndex;
            string headerText = dgvOrder.Columns[column].HeaderText;

            if (headerText.Equals("Items"))
            {
                tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;


                    tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    tb.TextChanged += new EventHandler(textbox_Change);
                }
            }

            else if (dgvOrder.CurrentCell.ColumnIndex == 2)
            {


                if (e.Control is TextBox)
                {
                    Quantitytomove1 = e.Control as TextBox;

                    Quantitytomove1.MaxLength = 7;
                    Quantitytomove1.KeyPress += new KeyPressEventHandler(Quantitytomove1_press);
                }

            }
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

                }
                else if (dgvOrder.CurrentCell.ColumnIndex == 1)
                {
                    dgvOrder.Focus();
                    this.ActiveControl = Quantitytomove1;
                    dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];

                }
                else if (dgvOrder.CurrentCell.ColumnIndex == 2)
                {
                    // dgvOrder.Focus();
                    //// this.ActiveControl = Quantitytomove1;
                    // dgvOrder.CurrentCell = dgvOrder[3, dgvOrder.CurrentCell.RowIndex];

                }
                else if (dgvOrder.CurrentCell.ColumnIndex == 3)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value)))
                        {
                            dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                        }
                        else
                        {
                            dgvOrder.Focus();
                            //dgvOrder.Rows.Add(1);
                            dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                        }
                    }
                    catch
                    {
                        dgvOrder.Focus();
                       // dgvOrder.Rows.Add(1);
                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                    }

                }
            }
            else
            {
               // MessageBox.Show("Enter product to move");
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

        private void button2_Click(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
            lblproductid.Text = string.Empty;
            //Txtitem.Text = string.Empty;
            lblitemcode.Text = "0";

            lbldisplay.Text = "0";
            lbldemo.Text = "0";
            lblservice.Text = "0";
           
            lblrack.Text = "0";
        }
        private void Dgvmovementsearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
            if (e.RowIndex >= 0)
            {
                string s = Convert.ToString(Dgvmovementsearch.Rows[Dgvmovementsearch.CurrentCell.RowIndex].Cells[0].Value);
                string select = MainTabSalesBill.SelectedTab.Name;

                

                 if (select == "TabNew")
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        BindMaterialTranscationMain(s);
                    }
                    else
                    {
                        clear();
                    }
                }
                if (select == "TabfloorApproval")
                {


                    if (!string.IsNullOrEmpty(s))
                    {
                        DgvApproval.Columns.Clear();
                        DgvApproval.Rows.Clear();
                        LoadApproval();
                        getCheckout(s);
                        // DgvApproval.Focus();
                        cbxStatus.Focus();



                    }
                    else
                    {
                        clear();
                    }
                }
                if (select == "TabFloorCheckOut")
                {
                    dgvFloorCheckOut.Columns.Clear();
                    dgvFloorCheckOut.Rows.Clear();
                    LoadPortsFloorCheckOut();
                    IDFloorCheckout = s;
                    GetWarrenty(s);
                }

                if (select == "TabPDI")
                {
                    IDPDI = s;
                    GetWarrentyPDI(s);
                }
                if (select == "TabDelivery")
                {
                    IDDELIVERY = s;
                    GetWarrentyDelivery(s);
                }

            }
            
        }
        public void getCheckout(string s)
        {
            DataTable ds = IssuedReceivedBAL.BindIssedWR(s);

            if (ds.Rows.Count > 0)
            {




                txtAvlCusname.Text = Convert.ToString(ds.Rows[0]["CustomerName"]);
                txtAvlPhNo.Text = Convert.ToString(ds.Rows[0]["PhoneNO"]);
                txtAvlEstNo.Text = Convert.ToString(ds.Rows[0]["EstimationId"]);

                lblWRHIDAvl.Text = Convert.ToString(ds.Rows[0]["Headid"]);

                DgvApproval.Rows.Clear();

                DgvApproval.Rows.Add(ds.Rows.Count);

                for (int i = 0; i < ds.Rows.Count; i++)
                {

                    DgvApproval.Rows[i].Cells[0].Value = i + 1;
                    DgvApproval.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["Item"]);
                    DgvApproval.Rows[i].Cells[2].Value = ds.Rows[i]["Quantity"];
                    DgvApproval.Rows[i].Cells[3].Value = ds.Rows[i]["Remarks"];
                    DgvApproval.Rows[i].Cells[4].Value = ds.Rows[i]["QuotationStatus"];
                    DgvApproval.Rows[i].Cells[5].Value = ds.Rows[i]["ProductId"];

                }

            }
            else
            {
                DgvApproval.Rows.Clear();
                panel2.Enabled = true;
                pnsearch.Visible = true;
            }


        }
        public void GetWarrenty(string ReferenceNo)
        {
         DataSet ds= IssuedReceivedBAL.GetWarrentyFloorcheckout(ReferenceNo);

         if (ds.Tables[0].Rows.Count>0)
         {
             txtFlrChkOutCustomerName.Text = Convert.ToString(ds.Tables[0].Rows[0]["CustomerName"]);
             txtFlrChkOutEstimationNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["EstimationId"]);
             txtFlrChkOutPhoneNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["PhoneNO"]);
         }
            if (ds.Tables[0].Rows.Count>0)
         {
               dgvFloorCheckOut.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {



                    dgvFloorCheckOut.Rows.Add();
                    dgvFloorCheckOut.Rows[i].Cells[0].Value = i + 1;
                    dgvFloorCheckOut.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["Items"]);
                    dgvFloorCheckOut.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                   // string vals = "3,Damage";
                    string vals = Convert.ToString(ds.Tables[1].Rows[i]["Location"]);
                    
                    //for (int k = 0; k < dgvFloorCheckOut.Rows.Count; k++)
                    //{
                    //    dgvFloorCheckOut.Rows[k].Cells["Location"].Value = (name.Items[0] as
                    //    DataRowView).Row[1].ToString();
                    //}
                  //  dgvFloorCheckOut.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
                    dgvFloorCheckOut.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["ProductId"]);

                    //DgvApproval.Rows.Add();
                    //DgvApproval.Rows[i].Cells[0].Value = i + 1;
                    //DgvApproval.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["Item"]);
                    //DgvApproval.Rows[i].Cells[2].Value = Convert.ToString(ds.Rows[i]["Quantity"]);
                    ////dgvFloorCheckOut.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
                    //DgvApproval.Rows[i].Cells[5].Value = Convert.ToString(ds.Rows[i]["Productid"]);
                    ////dgvFloorCheckOut.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["LocationID"]);



                    //string vals = Convert.ToString(ds.Rows[i]["Location"]);
                    DataTable dt = getdatatable(vals);
                    
                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).DataSource = dt;
                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).ValueMember = "id";
                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).DisplayMember = "location";
                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).Value = dt.Rows[0][0];
                    string val = Convert.ToString(dt.Rows[0][0]);
                    
                    string val1 = Convert.ToString(ds.Tables[1].Rows[i]["ProductId"]);
                    string v = getrack(val, val1);
                    dgvFloorCheckOut.Rows[i].Cells["Rack"].Value = v;


                   // dgvFloorCheckOut.Columns[3].ReadOnly = true;




                    dgvFloorCheckOut.Rows[i].Cells[0].ReadOnly=true;
                    dgvFloorCheckOut.Rows[i].Cells[1].ReadOnly=true;
                    dgvFloorCheckOut.Rows[i].Cells[2].ReadOnly=true;
                    //dgvFloorCheckOut.Rows[i].Cells[3].ReadOnly=true;
                    dgvFloorCheckOut.Rows[i].Cells[4].ReadOnly = true;


                


                }
                dgvFloorCheckOut.Focus();
                dgvFloorCheckOut.CurrentCell = dgvFloorCheckOut[6,0];
            }
            else
            {
                dgvFloorCheckOut.Rows.Clear();
                //panel2.Enabled = true;
            }

            dgvFloorCheckOut.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvFloorCheckOut.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }

        public void GetWarrentyPDI(string ReferenceNo)
        {
            DataSet ds = IssuedReceivedBAL.GetWarrentyPDI(ReferenceNo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                txtPDICustomerName.Text = Convert.ToString(ds.Tables[0].Rows[0]["CustomerName"]);
                txtPDIEstimationNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["EstimationId"]);
                txtPDIPhoneNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["PhoneNO"]);
            }
            if (ds.Tables[0].Rows.Count > 0)
            {

                dgvChecking.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvChecking.Rows.Add();
                    dgvChecking.Rows[i].Cells[0].Value = i + 1;
                    dgvChecking.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["Items"]);
                    dgvChecking.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    //dgvChecking.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[i]["Location"]);
                    //dgvChecking.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
                    dgvChecking.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[i]["ProductId"]);
                   

                    dgvChecking.Rows[i].Cells[0].ReadOnly = true;
                    dgvChecking.Rows[i].Cells[1].ReadOnly = true;
                    //dgvChecking.Rows[i].Cells[2].ReadOnly = true;
                    //dgvChecking.Rows[i].Cells[3].ReadOnly = true;
                    //dgvChecking.Rows[i].Cells[4].ReadOnly = true;

                }
                dgvChecking.Focus();
                dgvChecking.CurrentCell=dgvChecking[3,0];
            }
            else
            {
                dgvChecking.Rows.Clear();
            }

            dgvChecking.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvChecking.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }

        public void GetWarrentyDelivery(string ReferenceNo)
        {
            DataSet ds = IssuedReceivedBAL.GetWarrentyDelivery(ReferenceNo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                txtDeliveryCustomerName.Text = Convert.ToString(ds.Tables[0].Rows[0]["CustomerName"]);
                txtDeliveryEstimationNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["EstimationId"]);
                txtDeliveryPhoneNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["PhoneNO"]);
            }
            if (ds.Tables[0].Rows.Count > 0)
            {

                dgvDelivery.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvDelivery.Rows.Add();
                    dgvDelivery.Rows[i].Cells[0].Value = i + 1;
                    dgvDelivery.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["Items"]);
                    dgvDelivery.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    dgvDelivery.Rows[i].Cells["ProductId"].Value = Convert.ToString(ds.Tables[1].Rows[i]["ProductId"]);
                    //dgvChecking.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[i]["Location"]);
                    //dgvChecking.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);


                    dgvDelivery.Rows[i].Cells[0].ReadOnly = true;
                    dgvDelivery.Rows[i].Cells[1].ReadOnly = true;
                   
                    //dgvChecking.Rows[i].Cells[3].ReadOnly = true;
                    //dgvChecking.Rows[i].Cells[4].ReadOnly = true;

                }
                dgvDelivery.Focus();
               dgvDelivery.CurrentCell = dgvDelivery[3,0];
            }
            else
            {
                dgvDelivery.Rows.Clear();
            }

            dgvDelivery.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvDelivery.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
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
                if (Convert.ToBoolean(CbxCell.Value) == false)
                {
                    status = false;
                    break;
                }
            }
            return status;
        }
        private bool Validationchecking()
        {
            bool status = true;
            string message = "";
            for (int i = 0; i < dgvChecking.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dgvChecking.Rows[i].Cells[3].Value)))
                {
                    status = false;
                }

            }

            return status;
        }
         private bool ValidationDelivery()
        {
            bool status = true;
            string message = "";
            for (int i = 0; i < dgvDelivery.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dgvDelivery.Rows[i].Cells[3].Value)))
                {
                    status = false;
                }

            }

            return status;
        }

        //public void getCheckout(string s)
        //{
        //    DataTable ds = objIssuedReceivedBAL.BindMaterialTranscationMain(s);
        //    if (ds.Rows.Count > 0)
        //    {

               
        //        //cmbflrcustomer.Text =  Convert.ToString(ds.Rows[0]["CustomerId"]);
               
        //        //txtFlrOutRemarks.Text = Convert.ToString(ds.Rows[0]["Remarks"]);
        //        HidLblMain.Text = Convert.ToString(ds.Rows[0]["Headid"]);

        //        panel2.Enabled = false;
        //    }
        //    else
        //    {
        //        panel2.Enabled = true;
        //        clear();
        //    }
        //    if (ds.Rows.Count > 0)
        //    {
        //        DgvApproval.Rows.Clear();
        //        for (int i = 0; i < ds.Rows.Count; i++)
        //        {



        //            DgvApproval.Rows.Add();
        //            DgvApproval.Rows[i].Cells[0].Value = i + 1;
        //            DgvApproval.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["Item"]);
        //            DgvApproval.Rows[i].Cells[2].Value = Convert.ToString(ds.Rows[i]["Quantity"]);
        //            //dgvFloorCheckOut.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
        //            DgvApproval.Rows[i].Cells[5].Value = Convert.ToString(ds.Rows[i]["Productid"]);
        //            //dgvFloorCheckOut.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["LocationID"]);



        //            string vals = Convert.ToString(ds.Rows[i]["Location"]);
        //            DataTable dt = getdatatable(vals);

        //            (DgvApproval.Rows[i].Cells[3] as DataGridViewComboBoxCell).DataSource = dt;
        //            (DgvApproval.Rows[i].Cells[3] as DataGridViewComboBoxCell).ValueMember = "id";
        //            (DgvApproval.Rows[i].Cells[3] as DataGridViewComboBoxCell).DisplayMember = "location";
        //            (DgvApproval.Rows[i].Cells[3] as DataGridViewComboBoxCell).Value = dt.Rows[0][0];
        //            string val = Convert.ToString(dt.Rows[0][0]);
        //            string val1 = Convert.ToString(ds.Rows[i]["Productid"]);
        //            string v = getrack(val, val1);
        //            DgvApproval.Rows[i].Cells["Rack"].Value = v;
        //            DgvApproval.Columns[3].ReadOnly=true;

        //        }
        //        panel2.Enabled = false;
        //    }
        //    else
        //    {
        //        DgvApproval.Rows.Clear();
        //        panel2.Enabled = true;
        //    }

        //}
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
            QuotationBal objQuotationbal = new QuotationBal();
            string v = objQuotationbal.getrack(s, s1);
            return v;
        }
        public void BindMaterialTranscationMain(string s)
        {

            DataTable ds = IssuedReceivedBAL.BindIssedWR(s);

            if (ds.Rows.Count > 0)
            {




                ddlcustomers.Text = Convert.ToString(ds.Rows[0]["CustomerName"]);
                txtphoneno.Text = Convert.ToString(ds.Rows[0]["PhoneNO"]);
                cmbEstmationID.Text = Convert.ToString(ds.Rows[0]["EstimationId"]);

                HidLblMain.Text = Convert.ToString(ds.Rows[0]["Headid"]);

                dgvOrder.Rows.Clear();

                //dgvOrder.Rows.Add(ds.Rows.Count + 1);

                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    dgvOrder.Rows.Add();
                    dgvOrder.Rows[i].Cells[0].Value = i + 1;
                    dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["Item"]);
                    dgvOrder.Rows[i].Cells[2].Value = ds.Rows[i]["Quantity"];
                    dgvOrder.Rows[i].Cells[3].Value = ds.Rows[i]["Remarks"];
                    dgvOrder.Rows[i].Cells[4].Value = ds.Rows[i]["QuotationStatus"];
                    dgvOrder.Rows[i].Cells[6].Value = ds.Rows[i]["ProductId"];

                    dgvOrder.Rows[i].Cells[7].Value = ds.Rows[i]["SubId"];
                    dgvOrder.Rows[i].Cells[8].Value = ds.Rows[i]["SubId"];


                }

            }
            else
            {
                dgvOrder.Rows.Clear();
                panel2.Enabled = true;
                pnsearch.Visible = true;
            }

        }
        #region Load
        private void FrmIssuedReceived_Load(object sender, EventArgs e)
        {
            search("us.UserFullName", "", "h.EnteredOn", "Today", role1, Program.userid);

          
        }

        #endregion

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
                pnlOrder.Visible = true;
                vLabel2.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;

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

        private void MainTabSalesBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedtab = MainTabSalesBill.SelectedTab.Name;

          

             if (selectedtab == "TabNew")
            {
                search("us.UserFullName", "", "h.EnteredOn", "Today", role1, Program.userid);
                clear();
            }
            if (selectedtab == "TabfloorApproval")
            {
                search("us.UserFullName", "", "h.EnteredOn", "Today", role1, Program.userid);
                clear();
            }

            else if (selectedtab == "TabFloorCheckOut")
            {
                searchFloorCheckOut("us.UserFullName", "", "h.EnteredOn", "Today", role1, Program.userid);
                clear();
            }

            else if (selectedtab == "TabPDI")
            {
                searchPDI("us.UserFullName", "", "h.EnteredOn", "Today", role1, Program.userid);
                clear();                
            }
            else if (selectedtab == "TabDelivery")
            {

                searchDelivery("us.UserFullName", "", "h.EnteredOn", "Today", role1, Program.userid);
                clear();
            }
                
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvFloorCheckOut_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex != 6)
                {
                    if (DgvApproval[e.ColumnIndex, e.RowIndex].EditType.ToString() == "System.Windows.Forms.DataGridViewComboBoxEditingControl")
                    {
                        DataGridViewColumn column = DgvApproval.Columns[e.ColumnIndex];
                        if (column is DataGridViewComboBoxColumn)
                        {
                            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)DgvApproval[e.ColumnIndex, e.RowIndex];
                            DgvApproval.CurrentCell = cell;
                            DgvApproval.BeginEdit(true);
                            DataGridViewComboBoxEditingControl editingControl = (DataGridViewComboBoxEditingControl)DgvApproval.EditingControl;
                            editingControl.DroppedDown = true;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void dgvFloorCheckOut_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
        }

        private void FrmIssuedReceived_FormClosing(object sender, FormClosingEventArgs e)
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Txtitem_TextChanged_1(object sender, EventArgs e)
        {
            ProdSelRowvalue = 0;
        }
        public void AutoCompleteLoad(string s, int t)
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            QuotationBal objQuotationbal = new QuotationBal();
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

        public void itemdetails(string s)
        {

            try
            {
                dtitems = new DataTable();
                string s1 = s.Trim();
                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s1.Replace("'", "''");


                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                QuotationBal objQuotationbal = new QuotationBal();
                dtitems = objQuotationbal.itemdetails(name, s2);



            }
            catch (Exception e)
            {

            }

        }

        public void getitems(string sa)
        {
            var rows = from row in dtitems.AsEnumerable()
                       where row.Field<string>("ProductName").Trim() == sa
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

        private void DgvAutoRefNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(Txtitem.Text))
                {

                    if (Convert.ToInt32(lblproductid.Text) != 0)
                    {
                        dgvOrder.Columns[2].DefaultCellStyle.Format = "N2";
                        int rowindex = Convert.ToInt32(lblrowindex.Text);
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
                        dgvOrder.Rows[rowindex].Cells[6].Value = lblproductid.Text;
                        dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                        // dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                        double val = Convert.ToDouble(lblprice.Text);
                        //dgvOrder.Rows[rowindex].Cells[4].Value = val;
                        dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                        if (cmbEstmationID.SelectedIndex > 0)
                        {
                            if (dtEstDet.Rows.Count > 0)
                            {

                                DataRow[] qty = dtEstDet.Select("Productid='" + lblproductid.Text + "'");
                                if (qty.Length > 0)
                                {
                                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["adyqty"].Value = qty[0]["Quantity"].ToString();
                                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = "Found In Quotation";
                                }
                                else
                                {
                                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = "Not Found In Quotation";
                                }

                            }
                            else
                            {
                                dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = "Not Found In Quotation";
                            }
                        }
                        else
                        {
                            dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = "Not Found In Quotation";
                        }

                        DgvAutoRefNo.Visible = false;

                        pnsearch.Visible = false;
                        dgvOrder.Rows[rowindex].Cells["ProductId"].Value = lblproductid.Text;
                        lblproductid.Text = string.Empty;
                      //  Txtitem.Text = string.Empty;
                        lblitemcode.Text = "0";
                        lblrack.Text = "0";
                        lbldisplay.Text = "0";
                        lbldemo.Text = "0";
                        lblservice.Text = "0";
                        lbldamage.Text = "0";
                        lblprice.Text = "0";
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
                        dgvOrder.Focus();



                        dgvOrder.Focus();




                    }
                    else
                    {
                        MessageBox.Show("Please Enter Correct Product Name");
                    }
                }
                else
                {
                    this.ActiveControl = btnsave;
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

        private void DgvAutoRefNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Up) || e.KeyChar != Convert.ToChar(Keys.Down))
            {
                Txtitem.Focus();
                SendKeys.Send(e.KeyChar.ToString());
                lblhiddenproduct.Text = Txtitem.Text;

            }
        }

        private void DgvAutoRefNo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (Convert.ToInt32(lblproductid.Text) != 0)
                {
                    dgvOrder.Columns[2].DefaultCellStyle.Format = "N2";
                    int rowindex = Convert.ToInt32(lblrowindex.Text);
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];

                    //itemdetails(Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value));

                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                    getitems(sa);
                 
                    dgvOrder.Rows[rowindex].Cells[6].Value = lblproductid.Text;
                    dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();

                    //dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    //dgvOrder.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                    //dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                    double val = Convert.ToDouble(lblprice.Text);
                    // dgvOrder.Rows[rowindex].Cells[4].Value = val;
                    dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                    if (dtEstDet.Rows.Count > 0)
                    {

                        DataRow[] qty = dtEstDet.Select("Productid='" + lblproductid.Text + "'");
                        if (qty.Length > 0)
                        {
                            dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["adyqty"].Value = qty[0]["Quantity"].ToString();
                            dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = "Found In Quotation";
                        }
                        else
                        {
                            dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = "Not Found In Quotation";
                        }

                    }
                    else
                    {
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = "Not Found In Quotation";
                    }
                    DgvAutoRefNo.Visible = false;

                    pnsearch.Visible = false;
                    dgvOrder.Rows[rowindex].Cells["ProductId"].Value = lblproductid.Text;
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
                    dgvOrder.BeginEdit(true);
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];



                }
            }
            //else
            //{
            //    MessageBox.Show("Please Enter Correct Product Name");
            //}
        }

        private void transactionclose_Click(object sender, EventArgs e)
        {
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
            //Txtitem.Text = string.Empty;
            DgvAutoRefNo.DataSource = null;

            DgvAutoRefNo.Visible = false;
        }

        private void Dgvmovementsearch_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChecking_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                if (!string.IsNullOrEmpty(tborderquantoty.Text))
                {
                if (Convert.ToDecimal(dgvChecking.Rows[e.RowIndex].Cells[2].Value) != Convert.ToDecimal(tborderquantoty.Text))
                {
                    MessageBox.Show("Please Enter Correct Quantity");
                    dgvChecking.Focus();
                    edit = true;
                    dgvChecking.CurrentCell = dgvChecking[3, e.RowIndex];
                    dgvChecking.Rows[e.RowIndex].Cells[3].Value = "";
                }
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
                tborderquantoty.KeyPress += new KeyPressEventHandler(textbox_keypress);
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

        private void dgvFloorCheckOut_CellMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex != 6)
                {
                    if (dgvFloorCheckOut[e.ColumnIndex, e.RowIndex].EditType.ToString() == "System.Windows.Forms.DataGridViewComboBoxEditingControl")
                    {
                        DataGridViewColumn column = dgvFloorCheckOut.Columns[e.ColumnIndex];
                        if (column is DataGridViewComboBoxColumn)
                        {
                            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dgvFloorCheckOut[e.ColumnIndex, e.RowIndex];
                            dgvFloorCheckOut.CurrentCell = cell;
                            dgvFloorCheckOut.BeginEdit(true);
                            DataGridViewComboBoxEditingControl editingControl = (DataGridViewComboBoxEditingControl)dgvFloorCheckOut.EditingControl;
                            editingControl.DroppedDown = true;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void dgvFloorCheckOut_DataError_1(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void ddlcustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            {
                string s = "";
                if (ddlcustomers.SelectedIndex > 0)
                {
                    s = Convert.ToString(ddlcustomers.SelectedValue);
                    txtphoneno.Text = IssuedReceivedBAL.bindPhoneNo(s);

                    DataTable dtEstID = IssuedReceivedBAL.GetCusEstimationId(s);
                    if (dtEstID.Rows.Count <= 0)
                    {
                        LblEstStatus.Text = "Estimation Not Found";
                        DataRow row = dtEstID.NewRow();
                        row["Estimationid"] = "0";
                        row["Estimationid"] = "--Select--";
                        dtEstID.Rows.InsertAt(row, 0);
                        cmbEstmationID.DataSource = dtEstID;
                        cmbEstmationID.ValueMember = "Estimationid";
                        cmbEstmationID.DisplayMember = "Estimationid";
                        cmbEstmationID.SelectedIndex = 0;
                    }
                    else
                    {
                        LblEstStatus.Text = "";
                        DataRow row = dtEstID.NewRow();
                        row["Estimationid"] = "0";
                        row["Estimationid"] = "--Select--";
                        dtEstID.Rows.InsertAt(row, 0);
                        cmbEstmationID.DataSource = dtEstID;
                        cmbEstmationID.ValueMember = "Estimationid";
                        cmbEstmationID.DisplayMember = "Estimationid";
                        cmbEstmationID.SelectedIndex = 0;
                    }


                }



            }
        }

        private void txtphoneno_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtphoneno_KeyPress(object sender, KeyPressEventArgs e)
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

        private void cmbEstmationID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEstmationID.SelectedIndex > 0)
            {
                dtEstDet = IssuedReceivedBAL.GetEstBasedCusId(Convert.ToString(cmbEstmationID.SelectedValue), Convert.ToString(ddlcustomers.SelectedValue));
  

            }
        }

        private void ddlcustomers_TextChanged(object sender, EventArgs e)
        {
          
            try
            {
                cmbEstmationID.DataSource=null;
                txtphoneno.Text = "";
            }
            catch
            {
                cmbEstmationID.Text = "";
                txtphoneno.Text = "";
            }
            
        }

        private void dgvFloorCheckOut_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvFloorCheckOut.Rows.Count > 0)
            {
                if (dgvFloorCheckOut.CurrentCell.RowIndex == dgvFloorCheckOut.Rows.Count - 1)
                {
                    if (e.KeyData == Keys.Enter)
                    {
                        btnsave.Focus();
                    }
                }
            }
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


        public void GetReport(string QuotationId)
        {
            //try
            //{
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
                        cmd.CommandText = "GetWarranty_Print";
                        cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.Fill(ds);

                        Warranty Obj = new Warranty();
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


            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message.ToString());
            //}
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
   
        

       
       
    }
}
