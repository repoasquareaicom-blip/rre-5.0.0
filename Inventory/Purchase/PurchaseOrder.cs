
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
using Inventory.Report_Purchase;
using System.Collections;
using PurchaseOrderReport;
using Inventory.Sales;
using System.Configuration;
//using Inventory.Report_Purchase;


namespace Inventory.Purchase
{
    public partial class PurchaseOrder : Form
    {
        public static string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        public TextBox tb;
        public bool edit = false;
        string UserId = "";
        string role = "";
        string cas = string.Empty;
        int ProdSelRowvalue = 0;
        DataTable dtitems = new DataTable();
        string selectedtab = string.Empty;
        bool res = false;
        string test;
        //string UserId = "1";
        bool ProdNotFoundMSg = false;
        //   bool isnew = false;
        //string conn = Program.connection;
        PurchaseOrderBAL OblPurchaseOrderBAL = new PurchaseOrderBAL();
        string conn = Program.connection;
        SqlConnection ObjConn = new SqlConnection(Program.connection);
        QuotationBal objQuotationbal = new QuotationBal();
        string clickstatus = string.Empty;
        public PurchaseOrder()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            UserId = Convert.ToString(Program.userid);
            role = Program.Userrole;
            lblPreparedBy.Text = Program.UserName;
            cbxStatus.SelectedIndex = 0;
            LoadPorts();
            bindLocation();
            // cmbloaction.SelectedIndex = 0;
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            SearchPurchaseOrder();
            GetSuppliers();
            bindpending();
            BindSupplierSearch();
            label3.Visible = false;
            cbxStatus.Visible = false;
            search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId);

            // GetProducts();
            DataTable dt = Program.Dtmenu;
            bool contains = dt.AsEnumerable()
                .Any(row => "PurchaseApproval" == row.Field<String>("Data"));

            //if(Program.userlevel!=1)
            //{
            if (contains == false)
            {
                TabPurchaseOrder.TabPages.Remove(Tabapproval);
            }

            bool contains1 = dt.AsEnumerable()
                .Any(row => "PurchaseReject" == row.Field<String>("Data"));

            //if(Program.userlevel!=1)
            //{
            if (contains1 == false)
            {
                TabPurchaseOrder.TabPages.Remove(TabReject);
            }
            selectedtab = TabPurchaseOrder.SelectedTab.Name;
            TabPurchaseOrder.TabPages.Remove(Tabapproval);
            TabPurchaseOrder.TabPages.Remove(TabReject);
            //Rectangle resolution = Screen.PrimaryScreen.Bounds;
            //   int w = resolution.Width;
            //   int h = resolution.Height;

            //   if (w == 1366 && h == 768)
            //   {
            //       splitContainer1.SplitterDistance = splitContainer1.Width - splitContainer1.SplitterWidth;
            //       //splitContainer1.Panel1.Width = 300;

            //   }

            //}
        }

        public Button btn;
        private void Txtitem_KeyUp(object sender, KeyEventArgs e)
        {
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
                Locationpanal.Controls.Clear();
            }




        }



        private void pbxCollapse_Click(object sender, EventArgs e)
        {
            //splitContainer1.Panel1.Controls.Remove(btn);
            //btn = new Button();
            //splitContainer1.Panel1Collapsed = true;
            //System.Windows.Forms.ToolTip ToolTip6 = new System.Windows.Forms.ToolTip();
            //ToolTip6.SetToolTip(this.btn, "Search");
            //btn.Dock = DockStyle.Left;
            //btn.BackColor = SystemColors.ActiveBorder;
            //btn.Width = 20;
            //btn.FlatStyle = FlatStyle.Flat;
            //btn.TabStop = false;
            //btn.FlatAppearance.BorderSize = 0;
            //btn.Parent = splitContainer1.Panel2;
            //btn.Click += (s2, es) =>
            //{
            //    splitContainer1.Panel1Collapsed = false;
            //    splitContainer1.Panel2.Controls.Remove(btn);
            //};
            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;
                vLabel1.Enabled = true;
            }
        }

        public void bindLocation()
        {
            cmbloaction.DataSource = OblPurchaseOrderBAL.getLocation();
            cmbloaction.DisplayMember = "LocationName";
            cmbloaction.ValueMember = "LocationID";
        }

        private void LoadPorts()
        {
            dgvOrder.Rows.Clear();
            dgvOrder.Columns.Clear();
            dgvOrder.ColumnCount = 4;
            dgvOrder.RowCount = 1;

            dgvOrder.Columns[0].Name = "S.No";
            dgvOrder.Columns[1].Name = "Items";
            dgvOrder.Columns[2].Name = "Quantity";
            dgvOrder.Columns[3].Name = "ProductId";

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvOrder.Columns[0].Width = 100;
                this.dgvOrder.Columns[1].Width = 500;
                this.dgvOrder.Columns[2].Width = 100;

            }
            else
            {
                this.dgvOrder.Columns[0].Width = 100;
                this.dgvOrder.Columns[1].Width = 700;
                this.dgvOrder.Columns[2].Width = 100;
            }
            //this.dgvOrder.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;


            //this.dgvOrder.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;            
            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvOrder.Columns[1].ReadOnly = true;

            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvOrder.Columns[3].Visible = true;

            //this.dgvOrder.Columns[4].ReadOnly = true;
            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }

        }



        public void itemdetails(string s)
        {

            try
            {
                //dtitems = new DataTable();
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
        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("Order Number");
            search.Add("Order Date");
            search.Add("Vendor");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderNo.DataSource = bs.DataSource;
            cbxSearchOrderNo.SelectedIndex = 0;
        }

        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("Order Number");
            search.Add("Order Date");
            search.Add("Vendor");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderDate.DataSource = bs.DataSource;
            cbxSearchOrderDate.SelectedIndex = 1;
        }

        private void SearchCreteria3()
        {
            List<string> search = new List<string>();
            search.Add("Order Number");
            search.Add("Order Date");
            search.Add("Vendor");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxVendor.DataSource = bs.DataSource;
            cbxVendor.SelectedIndex = 2;
        }





        private void RefScrollGrid()
        {
            if (DgvAutoRefNo.Rows.Count - 1 >= ProdSelRowvalue)
            {
                DgvAutoRefNo.FirstDisplayedScrollingRowIndex = ProdSelRowvalue;
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
                this.dgvSearch.Columns[1].Visible = true;
                this.dgvSearch.Columns[2].Visible = true;
                this.dgvSearch.Columns[3].Visible = true;


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
                this.dgvSearch.Columns[3].Visible = false;
            }
        }

        private void Btnprint_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOrderNo.Text))
            {

                GetReport(txtOrderNo.Text);


                //DialogResult result = MessageBox.Show("Do you want to print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //if (result == DialogResult.Yes)
                //{
                //    PurchaseOrderRV rpt = new PurchaseOrderRV(txtOrderNo.Text);
                //    rpt.ShowDialog();
                //}
            }
            else
            {
                MessageBox.Show("Please Select Item");
            }
        }

        //public void preview()
        //{
        //    try
        //    {
        //        int c = Convert.ToInt32(1);
        //        Purchasereport pos = new Purchasereport(c);
        //        pos.ShowDialog();
        //        PurchaseOrderRV rpt = new PurchaseOrderRV(txtOrderNo.Text);
        //        rpt.ShowDialog();
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Please Select The Item");
        //    }
        //}  






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

            this.dgvSearch.Columns[2].Width = 40;

            this.dgvSearch.Columns[3].Width = 40;

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

        private void txtPhNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }


        private void btnSave_Click(object sender, EventArgs e)
        {

            bool val = Validation();

            if (val)
            {
                DataTable dt = DataGridView2DataTable(dgvOrder);
                for (int i = 0; i < 2; i++)
                {
                    dt.Columns.RemoveAt(0);
                }
                RemoveNullColumnFromDataTable(dt);
                // foreach (DataRow row in dt.Rows)
                //{
                //    if (row["Quantity"].ToString() == "0")
                //    {
                //        //MessageBox.Show("Quanityy");

                //       test = "1";
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

                    if (Validationsss())
                    {
                        if (val)
                        {
                            save(dt);
                            clear();
                        }
                    }
                    else
                    {
                        if (dgvOrder.Rows.Count == 0)
                        {
                            dgvOrder.Rows.Add();
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder[1, 0];
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please Remove Duplication Product");
                }
            }

            //}
           

        }



        private bool Validationsss()
        {
            bool sas = false;
            bool status = true;
            string message = "";
            int i = 0;
            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                string Items = Convert.ToString(dgvOrder.Rows[k].Cells["Quantity"].Value);






                if (Items == "0" || Items == "00" || Items == "000" || Items == "000" || Items == "0000" || Items == "00000" || Items == "00000" || Items == "0000000" || Items == "00000000" || Items == "000000000" || Items == "0000000000")
                {
                    sas = true;
                    break;
                }
                else
                {





                }

            }
            if (sas == true)
            {
                i++;
                message = message + "* Quantity should not be zero." + "\n";
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

        //}


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


        public bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;


            if (cbVendor.SelectedIndex == 0)
            {
                i++;
                message = message + "* Should select vendor" + "\n";
                if (i == 1)
                    this.ActiveControl = cbVendor;
            }
            //if (!string.IsNullOrEmpty(txtOrderNo.Text))
            //{
            //    if (cbxStatus.SelectedIndex == 0)
            //    {
            //        i++;
            //        message = message + "* Should select Status" + "\n";
            //        if (i == 1)
            //            this.ActiveControl = cbxStatus;
            //    }
            //}

            //if (string.IsNullOrEmpty(txtRemarks.Text))
            //{
            //    i++;
            //    message = message + "*Please Enter Remarks." + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtRemarks;
            //}

            //DateTime d1 = Convert.ToDateTime(dateTimePicker1.Value.Month + "/" + dateTimePicker1.Value.Day + "/" + dateTimePicker1.Value.Year);
            //DateTime d2 = Convert.ToDateTime(dateTimePicker2.Value.Month + "/" + dateTimePicker2.Value.Day + "/" + dateTimePicker2.Value.Year);

            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                i++;
                message = message + "*Expected delivery date should not be lesser than order date" + "\n";
                if (i == 1)
                    this.ActiveControl = dateTimePicker2;
            }





            //if (dgvOrder.Rows.Count > 0)
            //{
            //    i++;
            //    string Items = Convert.ToString(dgvOrder.Rows[0].Cells["Quantity"].Value);
            //    string Received = Convert.ToString(dgvOrder.Rows[0].Cells["ProductId"].Value);
            //    if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
            //    {
            //        message = message + "*.Please Select Product" + "\n";

            //    }

            //    if (i == 1)
            //        this.ActiveControl = dgvOrder;
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




            //bool sas = false;

            //for (int k = 0; k < dgvOrder.RowCount; k++)
            //{
            //    string Items = Convert.ToString(dgvOrder.Rows[k].Cells["Quantity"].Value);
            //    string Received = Convert.ToString(dgvOrder.Rows[k].Cells["ProductId"].Value);

            //    if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))))
            //    {
            //        sas = true;
            //        break;
            //    }
            //}
            //if (sas == true)
            //{
            //    i++;
            //    message = message + "*Product or quantity should not be empty." + "\n";
            //    if (i == 1)
            //        this.ActiveControl = dgvOrder;
            //}

            //clearGrid(dgvOrder);

            //foreach (DataGridViewRow row in dgvOrder.Rows)
            //{
            //    string Items = Convert.ToString(row.Cells["Items"].Value);
            //    string Received = Convert.ToString(row.Cells["Quantity"].Value);

            //    if (string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received)))
            //    {
            //        message = message + "*Please Enter Items and Quantity." + "\n";
            //        break;
            //    }
            //}


            //dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];

            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }


        private void clearGrid(DataGridView view)
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


        private void btnPendingSave_Click(object sender, EventArgs e)
        {
            if (cbVendor.SelectedIndex <= 0)
            {
                MessageBox.Show("Should select vendor");
                this.ActiveControl = cbVendor;
                return;
            }
            if ((cbVendor.SelectedIndex > 0))
            {
                bool val = Validation();
                DataTable dt = DataGridView2DataTable(dgvOrder);
                for (int i = 0; i < 2; i++)
                {
                    dt.Columns.RemoveAt(0);
                }
                RemoveNullColumnFromDataTable(dt);
                bool dtval = RemoveDuplicateRows(dt, "ProductId");

                if (dtval)
                {
                    if (val)
                    {
                        save(dt);
                        clear();
                    }
                    else
                    {
                        if (dgvOrder.Rows.Count == 0)
                        {
                            dgvOrder.Rows.Add();
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder[1, 0];
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please Remove Duplication Product");
                }
            }
        }


        public void save(DataTable dt)
        {
            panel1.Enabled = false;
            panel2.Enabled = false;

            //DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(txtOrderNo.Text))
            {
                OblPurchaseOrderBAL.isnew = 0;
            }
            else
            {
                OblPurchaseOrderBAL.isnew = 1;
            }
            OblPurchaseOrderBAL.OrderNo = txtOrderNo.Text;
            DateTime date = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
            OblPurchaseOrderBAL.OrderDate = date;
            DateTime date1 = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);
            OblPurchaseOrderBAL.ExpectedDelieveryDate = date1;
            OblPurchaseOrderBAL.Supplierid = Convert.ToString(cbVendor.SelectedValue);
            OblPurchaseOrderBAL.Remarks = Convert.ToString(txtRemarks.Text);
            if (cbxStatus.Text == "Approve")
            {
                if (lbedit.Text == "1")
                    OblPurchaseOrderBAL.status = "";
                else
                    OblPurchaseOrderBAL.status = "Approve";
            }
            //else if (cbxStatus.Text == "Reject")
            //{
            //    if (lbedit.Text == "1")
            //        OblPurchaseOrderBAL.status = "";
            //    else
            //        OblPurchaseOrderBAL.status = "Reject";
            //}
            //else if (cbxStatus.Text == "Closed")
            //{
            //    if (lbedit.Text == "1")
            //        OblPurchaseOrderBAL.status = "";
            //    else
            //        OblPurchaseOrderBAL.status = "Closed";
            //}
            else
            {
                OblPurchaseOrderBAL.status = "";

            }
            OblPurchaseOrderBAL.Updatedby = Program.userid;



            string output = OblPurchaseOrderBAL.SavePurchaseOrder(OblPurchaseOrderBAL, dt);
            if (!string.IsNullOrEmpty(output) && string.IsNullOrEmpty(txtOrderNo.Text))
            {
                panel1.Enabled = true;
                panel2.Enabled = true;
                //Pnloading.Visible = false;

                //MessageBox.Show("save successfully");
                txtOrderNo.Text = output;
                if (cbxStatus.Text == "Approve")
                {
                    GetReport(output);
                    //DialogResult result = MessageBox.Show("Do you want to print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    //if (result == DialogResult.Yes)
                    //{
                    //    PurchaseOrderRV rpt = new PurchaseOrderRV(output);
                    //    rpt.ShowDialog();
                    //}
                }
                GetPurchaseOrderByOrderNo(output);
                label3.Visible = true;
                cbxStatus.Visible = true;
                total();

            }
            else if (!string.IsNullOrEmpty(output) && !string.IsNullOrEmpty(txtOrderNo.Text))
            {
                //MessageBox.Show("Update successfully");
                panel1.Enabled = true;
                panel2.Enabled = true;
                //Pnloading.Visible = false;

                txtOrderNo.Text = output;
                if (cbxStatus.Text == "Approve")
                {
                    GetReport(output);
                    //DialogResult result = MessageBox.Show("Do you want to print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //if (result == DialogResult.Yes)
                    //{
                    //    PurchaseOrderRV rpt = new PurchaseOrderRV(output);
                    //    rpt.ShowDialog();
                    //}
                }
                GetPurchaseOrderByOrderNo(output);
                label3.Visible = true;
                cbxStatus.Visible = true;
                total();
            }
            
            search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId);

            //bindpending();
            //search("Quotationid", "", "customername", "", "r.Name", "", userid);
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
                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][1])))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }

        public void bindpending()
        {
            //flowLayoutPanel1.Controls.Clear();
            //DataTable dt = OblPurchaseOrderBAL.GetpendingPurchaseOrder(userid);
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    RadioButton button = new RadioButton();
            //    button.Tag = i;
            //    button.Width = 100;
            //    button.FlatStyle = FlatStyle.Popup;
            //    button.Appearance = Appearance.Button;
            //    button.Cursor = Cursors.Hand;
            //    button.CheckedChanged += new EventHandler(button_click);
            //    button.Text = Convert.ToString(dt.Rows[i]["OrderNumber"]);
            //    flowLayoutPanel1.Controls.Add(button);
            //}


            //RadioButton btn = new RadioButton();
            //btn.Tag = 0;
            //btn.Width = 50;
            //btn.FlatStyle = FlatStyle.Popup;
            //btn.Appearance = Appearance.Button;
            //btn.Cursor = Cursors.Hand;
            //btn.Checked = true;
            //btn.CheckedChanged += new EventHandler(button_click);
            //btn.Text = "New";
            //flowLayoutPanel1.Controls.Add(btn);


        }

        private void button_click(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;

            if (btn.Checked)
            {
                GetPurchaseOrderByOrderNo(btn.Text);
                total();
                panel2.Enabled = true;
            }

        }

        public void GetPurchaseOrderByOrderNo(string s)
        {
            DataSet ds = OblPurchaseOrderBAL.GetPurchaseOrderByOrderNo(s, "New");
            if (ds.Tables[0].Rows.Count > 0)
            {
                dateTimePicker1.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderDate"]);
                dateTimePicker2.Text = Convert.ToString(ds.Tables[0].Rows[0]["ExpectedDeliveryDate"]);
                cbVendor.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"]);
                txtOrderNo.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderNumber"]);
                //cbxStatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                txtRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                cbxStatus.Enabled = true;
                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
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
                    dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                }
                panel2.Enabled = false;

            }
            else
            {
                dgvOrder.Rows.Clear();
                panel2.Enabled = true;
            }

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void clear()
        {
            cbVendor.SelectedIndex = 0;
            txtCity.Clear();
            txtOrderNo.Clear();
            dgvOrder.Rows.Clear();
            dgvOrder.Rows.Add(1);
            //dgvOrder.Columns.Clear();
            //LoadPorts();
            cbxStatus.SelectedIndex = 0;
            this.dateTimePicker1.Value = DateTime.Now;
            txtRemarks.Clear();
            this.dateTimePicker2.Value = DateTime.Now;
            lbltotalquantity.Text = "0";
            cbxStatus.Enabled = false;
            lbedit.Text = "0";
            cbVendor.Enabled = true;
            dateTimePicker2.Enabled = true;
            cbxStatus.Enabled = true;
            cbxStatus.SelectedIndex = 0;
            label3.Visible = false;
            cbxStatus.Visible = false;
        }

        private void TabPurchaseOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tabname = TabPurchaseOrder.SelectedTab.Name;

            if (tabname == "Tabapproval")
            {
                Btnprint.Enabled = true;
                Btnprint.Visible = true;
                btnNew.Enabled = true;
                btnSave.Enabled = true;
                btnPendingSave.Enabled = true;
                btnClear.Enabled = true;
                // isnew = false;
                BindPurchaseComplete();

                if (pnlSearch.Visible == true)
                {
                    pnlLabelSearch.Visible = false;
                    vLabel1.Visible = false;
                    pnlSearch.Visible = true;
                    splitContainer1.Panel1Collapsed = false;
                    vLabel1.Enabled = true;
                }

            }

            if (tabname == "TabNew")
            {
                //Btnprint.Enabled = true;
                //Btnprint.Visible = false;
                btnNew.Enabled = true;
                btnSave.Enabled = true;
                btnPendingSave.Enabled = true;
                btnClear.Enabled = true;
                // isnew = true;
                search("OrderNumber", "", "OrderDate", "Today", "Name", "", role, UserId);

                if (pnlLabelSearch.Visible == true)
                {
                    pnlLabelSearch.Visible = false;
                    vLabel1.Visible = false;
                    pnlSearch.Visible = true;
                    splitContainer1.Panel1Collapsed = false;
                    vLabel1.Enabled = true;
                    this.ActiveControl = cbVendor;
                }
            }
            else if (tabname == "TabReject")
            {
                Btnprint.Enabled = true;
                Btnprint.Visible = true;
                btnNew.Enabled = true;
                btnSave.Enabled = true;
                btnPendingSave.Enabled = true;
                btnClear.Enabled = true;
                //isnew = false;
                GetpendingPurchaseOrder();

                if (pnlSearch.Visible == true)
                {
                    pnlLabelSearch.Visible = false;
                    vLabel1.Visible = false;
                    pnlSearch.Visible = true;
                    splitContainer1.Panel1Collapsed = false;
                    vLabel1.Enabled = true;
                }
            }
        }

        public void BindPurchaseComplete()
        {
            gdvapproval.Columns.Clear();
            DataTable dtpCom = OblPurchaseOrderBAL.BindPurchaseComplete(Convert.ToInt32(UserId));
            gdvapproval.DataSource = dtpCom;
            gdvapproval.Columns[0].Visible = false;
            gdvapproval.Columns[4].Visible = false;
            gdvapproval.Columns[6].Visible = false;

            DataGridViewButtonColumn uninstallButtonColumn = new DataGridViewButtonColumn();
            uninstallButtonColumn.Name = "Action";
            uninstallButtonColumn.Text = "Approve";
            uninstallButtonColumn.FlatStyle = FlatStyle.Popup;
            uninstallButtonColumn.UseColumnTextForButtonValue = true;

            if (gdvapproval.Columns["Approve"] == null)
            {
                gdvapproval.Columns.Insert(8, uninstallButtonColumn);
                gdvapproval.Columns["Action"].Width = 80;
            }
        }

        public void GetpendingPurchaseOrder()
        {
            DataTable dtpCom = OblPurchaseOrderBAL.GetpendingPurchaseOrder(Convert.ToInt32(UserId));
            dgvReject.Columns.Clear();
            dgvReject.DataSource = null;
            dgvReject.DataSource = dtpCom;
            dgvReject.Columns[0].Visible = false;
            dgvReject.Columns[4].Visible = false;
            dgvReject.Columns[6].Visible = false;


            DataGridViewButtonColumn uninstallButtonColumn = new DataGridViewButtonColumn();
            uninstallButtonColumn.Name = "Action";
            uninstallButtonColumn.Text = "Close";
            uninstallButtonColumn.FlatStyle = FlatStyle.Popup;
            uninstallButtonColumn.UseColumnTextForButtonValue = true;

            if (dgvReject.Columns["Action"] == null)
            {
                dgvReject.Columns.Insert(9, uninstallButtonColumn);
                dgvReject.Columns["Action"].Width = 80;
            }


        }

        //public void GetPurChaseOrder()
        //{
        //    using (SqlConnection con = new SqlConnection(conn))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("[PurchaseOrderID]", con);
        //        SqlDataAdapter ad = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        ad.Fill(dt);
        //        dgvOrder.Columns.Clear();
        //        dgvOrder.DataSource = dt;
        //        dgvOrder.Columns[""].Visible = false;

        //        DataGridViewImageColumn img1 = new DataGridViewImageColumn();
        //        img1.Image = Inventory.Properties.Resources.user_edit;
        //        dgvOrder.Columns.Insert(13, img1);
        //        img1.HeaderText = "Edit";
        //        img1.Name = "Edit";


        //        DataGridViewImageColumn img2 = new DataGridViewImageColumn();
        //        img2.Image = Inventory.Properties.Resources.trash;
        //        dgvOrder.Columns.Insert(14, img2);
        //        img2.HeaderText = "Delete";
        //        img2.Name = "Delete";

        //        //dgvOrder.Columns["Edit"].Width = 40;
        //        //dgvOrder.Columns["Delete"].Width = 60;
        //        //dgvOrder.Columns["Path"].Visible = false;
        //        //dgvOrder.Columns["Pincode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //        //dgvOrder.Columns["Phone"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //        //dgvOrder.Columns["Fax"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //        //dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //        //dgvOrder.DefaultCellStyle.BackColor = Color.Gainsboro;
        //        //dgvOrder.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        //    }
        //}

        private void PurchaseOrder_Load(object sender, EventArgs e)
        {
            this.ActiveControl = cbVendor;
            DataTable dtitems = Program.dtitems;
            if (dtitems == null)
            {
                itemdetails("");
            }

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {


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
                    cbVendor.Focus();
                    return true;
                }

            }

            //if (cbVendor.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        txtCity.Focus();
            //        return true;
            //    }

            //}
            //if(keyData==Keys.Up)
            //{
            //    if (dgvOrder.Focused)
            //    {
            //        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex - 1];
            //        return true;
            //    }
            //}

            //if (keyData == Keys.Down)
            //{
            //    if (dgvOrder.Focused)
            //    {
            //        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex+1];
            //        return true;
            //    }
            //}

            //if (keyData == Keys.Right)
            //{
            //    if (dgvOrder.Focused)
            //    {
            //        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex+1, dgvOrder.CurrentCell.RowIndex];
            //        return true;
            //    }
            //}
            //if (keyData == Keys.Left)
            //{
            //    if (dgvOrder.Focused)
            //    {
            //        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex -1, dgvOrder.CurrentCell.RowIndex];
            //        return true;
            //    }
            //}
            //   if(isnew==true)
            // {
            if (keyData == (Keys.Alt | Keys.Insert))
            {
                if (dgvOrder.Rows.Count <= 0)
                {
                    dgvOrder.Rows.Add();
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[2, 0];
                }
                else
                {
                    int rowindex = dgvOrder.CurrentRow.Index;
                    int colindex = dgvOrder.CurrentCell.ColumnIndex;
                    //dgvOrder.Rows.Insert(rowindex, dgvOrder.Rows.Add(1));
                    dgvOrder.Rows.Insert(rowindex, 1);
                    dgvOrder.Focus();
                    try
                    {
                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex - 1];
                    }
                    catch
                    {

                    }

                    return true;
                }
                getsino();
            }

            // }

            if (keyData == (Keys.Alt | Keys.Delete))
            {
                if (TabPurchaseOrder.SelectedTab.Text == "New")
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

            //if (dgvOrder.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        try
            //        {
            //            int col = dgvOrder.CurrentCell.ColumnIndex;
            //            int row = dgvOrder.CurrentCell.RowIndex;
            //            if (row == (dgvOrder.Rows.Count - 1))
            //            {
            //                if (col == 2)
            //                {
            //                    this.ActiveControl = txtRemarks;
            //                    return true;
            //                }
            //            }
            //        }
            //        catch
            //        {

            //        }
            //    }
            //}

            if (txtRemarks.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = dateTimePicker2;
                    return true;
                }
            }




            if (cbVendor.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = dgvOrder;
                    if (dgvOrder.Rows.Count > 0)
                    {
                        dgvOrder.CurrentCell = dgvOrder[1, 0];
                    }
                    else
                    {
                        dgvOrder.Rows.Add(1);
                        dgvOrder.CurrentCell = dgvOrder[1, 0];
                    }
                    return true;
                }
            }


            //if (dateTimePicker1.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = dgvOrder;
            //        dgvOrder.CurrentCell = dgvOrder[1, 0];
            //        //dgvOrder.BeginEdit(true);
            //        return true;
            //    }
            //}

            //if (keyData == (Keys.Tab))
            //{
            //    pnsearch.Visible = false;
            //    return true;
            //}


            //if (txtRemarks.Focused)
            //{
            //    //if (keyData == (Keys.Tab))
            //    //{
            //        pnsearch.Visible = false;
            //        return false;
            //    //}
            //}

            //if (dateTimePicker2.Focused)
            //{
            //   if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = btnSave;
            //      return true;
            //   }
            //}

            //if (keyData == Keys.Escape)
            //{
            //    pnsearch.Visible = false;
            //    dgvOrder.Focus();
            //    dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
            //    dgvOrder.BeginEdit(true);
            //}


            if (keyData == Keys.Escape)
            {
                if (pnsearch.Visible)
                {
                    pnsearch.Visible = false;
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
                    dgvOrder.BeginEdit(true);
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
            DataTable dtsup = OblPurchaseOrderBAL.GetSuppliers(null);
            cmbstatus1.DataSource = dtsup;
            cmbstatus1.ValueMember = "SuppliersID";
            cmbstatus1.DisplayMember = "Name";
            cmbstatus1.SelectedIndex = 0;

            cmbstatus2.DataSource = dtsup;
            cmbstatus2.ValueMember = "SuppliersID";
            cmbstatus2.DisplayMember = "Name";
            cmbstatus2.SelectedIndex = 0;

            cmbstatus3.DataSource = dtsup;
            cmbstatus3.ValueMember = "SuppliersID";
            cmbstatus3.DisplayMember = "Name";
            cmbstatus3.SelectedIndex = 0;
        }

        //public void GetProducts()
        //{
        //    using (SqlConnection con = new SqlConnection(conn))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("[GetPurChaseOrder]", con);
        //        SqlDataAdapter ad = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        ad.Fill(dt);
        //        dgvOrder.Columns.Clear();
        //        dgvOrder.DataSource = dt;
        //        dgvOrder.Columns["id"].Visible = false;
        //        dgvOrder.Columns["OrderNumber"].Visible = false;
        //        dgvOrder.Columns["ProductId"].Visible = false;


        //        dgvOrder.Columns["Item"].Width = 500;
        //        dgvOrder.Columns["Quantity"].Width = 513;

        //        //DataGridViewImageColumn img1 = new DataGridViewImageColumn();
        //        //img1.Image = Inventory.Properties.Resources.user_edit;
        //        //dgvOrder.Columns.Insert(3, img1);
        //        //img1.HeaderText = "Edit";
        //        //img1.Name = "Edit";

        //        //DataGridViewImageColumn img2 = new DataGridViewImageColumn();
        //        //img2.Image = Inventory.Properties.Resources.trash;
        //        //dgvOrder.Columns.Insert(14, img2);
        //        //img2.HeaderText = "Delete";
        //        //img2.Name = "Delete";

        //        //dgvOrder.Columns["product"].Width = 20;

        //        //dgvOrder.Columns["Edit"].Width = 40;
        //        //dgvOrder.Columns["Delete"].Width = 60;

        //        dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //        dgvOrder.DefaultCellStyle.BackColor = Color.Gainsboro;
        //        dgvOrder.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        //    }
        //}

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

        private void dgvOrder_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
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

        private void dgvOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvOrder.CurrentCell.ColumnIndex;
            string headerText = dgvOrder.Columns[column].HeaderText;

            //if (headerText.Equals("Items"))
            //{
            //    tb = e.Control as TextBox;
            //    if (tb != null)
            //    {
            //        tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;


            //        tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //        //tb.TextChanged += new EventHandler(textbox_Change);
            //    }
            //}
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

            try
            {
                string itemval = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value);
                if (string.IsNullOrEmpty(itemval))
                {
                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value = "";
                    dgvOrder.RefreshEdit();

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

        private void dgvOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // MessageBox.Show(e.ColumnIndex.ToString());
        }

        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                pnsearch.Visible = true;
                //Txtitem.Text = "A";
                //AutoCompleteLoad("A", 1);
                //Txtitem.SelectionStart = 1;

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
                pnsearch.Visible = false; ;
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
                    totalquantity = totalquantity + Convert.ToDouble(dgvOrder.Rows[i].Cells[2].Value);
                }

                lbltotalquantity.Text = Convert.ToString(totalquantity);
                //lbltotalamount.Text = Convert.ToString(totalamount);
            }
            catch
            {

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
                    dgvOrder.BeginEdit(true);

                }
                else if (dgvOrder.CurrentCell.ColumnIndex == 1)
                {
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
                    dgvOrder.BeginEdit(true);
                }
                //else if (dgvOrder.CurrentCell.ColumnIndex == 2)
                //{
                //    dgvOrder.Focus();
                //    //string val = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value);
                //    //string qtyval = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value);
                //    //if (!string.IsNullOrEmpty(val) && !string.IsNullOrEmpty(qtyval))
                //    //{
                //    //    dgvOrder.Rows.Add(1);
                //    //    dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                //    //    dgvOrder.BeginEdit(true);
                //    //}
                //    dgvOrder.BeginEdit(true);
                //}          

            }
        }

        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2)
                {
                    dgvOrder.Focus();
                    edit = true;
                    string val = Convert.ToString(dgvOrder.Rows[e.RowIndex].Cells[1].Value);
                    string qtyval = Convert.ToString(dgvOrder.Rows[e.RowIndex].Cells[2].Value);
                    if (!string.IsNullOrEmpty(val) && !string.IsNullOrEmpty(qtyval))
                    {
                        dgvOrder.Rows.Add(1);
                        dgvOrder.CurrentCell = dgvOrder[1, e.RowIndex + 1];
                        dgvOrder.BeginEdit(true);
                    }
                    //if(e.RowIndex == (dgvOrder.Rows.Count - 1))
                    //{
                    //    this.ActiveControl = txtRemarks;
                    //}
                    //else
                    //{
                    //    dgvOrder.BeginEdit(false);
                    //}
                }
            }
            catch
            {

            }
        }

        private void dgvOrder_SelectionChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (edit == true)
            //    {
            //        if (dgvOrder.CurrentCell.RowIndex >= 1)
            //        {
            //            dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex - 1];
            //            edit = false;
            //        }
            //        else if (dgvOrder.CurrentCell.RowIndex == 0)
            //        {
            //            dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex - 1];
            //        }

            //    }
            //}
            //catch
            //{

            //}
        }

        private void dgvOrder_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                string itemval = Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value);
                if (!string.IsNullOrEmpty(itemval))
                {
                    total();
                }
            }
        }

        private void Txtitem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(Txtitem.Text))
                {
                    if (Convert.ToInt32(lblproductid.Text) != 0)
                    {
                        if (ProdNotFoundMSg)
                        {
                            // LblProdNotFoundMSg.Visible = true;
                        }
                        else
                        {
                            int rowindex = Convert.ToInt32(lblrowindex.Text);
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];

                            //if (CheckDuplicate(lblproductid.Text) == false)
                            //{

                                dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                                dgvOrder.Rows[rowindex].Cells[1].Value = Txtitem.Text.ToUpper();
                                dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                                pnsearch.Visible = false;
                                lblproductid.Text = string.Empty;
                                Txtitem.Text = string.Empty;
                                lblitemcode.Text = "0";
                                lblrack.Text = "0";
                                lbldisplay.Text = "0";
                                lblprice.Text = "0";
                                dgvOrder.Focus();
                                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
                                dgvOrder.BeginEdit(true);
                                //  LblProdNotFoundMSg.Visible = false;
                            //}


                            //else
                            //{
                            //    MessageBox.Show("Product already added.");
                            //}
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please Enter Product Name");
                        Txtitem.Focus();
                    }
                }
            }
        }


        private bool CheckDuplicate(string ProductId)
        {
            bool returnAction = false;


            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                if (dgvOrder.Rows[i].Cells[3].Value != null)
                {
                    if (dgvOrder.Rows[i].Cells[3].Value.ToString() == ProductId)
                    {
                        returnAction = true;
                    }
                }

            }



            return returnAction;
        }
        //private void Txtitem_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyData == Keys.Enter)
        //    {
        //        if (!string.IsNullOrEmpty(Txtitem.Text))
        //        {
        //            if (ProdNotFoundMSg)
        //            {
        //                LblProdNotFoundMSg.Visible = true;
        //            }
        //            else
        //            {
        //                int rowindex = Convert.ToInt32(lblrowindex.Text);
        //                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];
        //                dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
        //                dgvOrder.Rows[rowindex].Cells[1].Value = Txtitem.Text.ToUpper();
        //                dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
        //                pnsearch.Visible = false;
        //                lblproductid.Text = string.Empty;
        //                Txtitem.Text = string.Empty;
        //                lblitemcode.Text = "0";
        //                lblrack.Text = "0";
        //                lbldisplay.Text = "0";   
        //                lblprice.Text = "0";
        //                dgvOrder.Focus();
        //                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
        //                dgvOrder.BeginEdit(true);
        //                LblProdNotFoundMSg.Visible = false;
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Please Enter Product Name");
        //            Txtitem.Focus();
        //        }
        //    }
        //}

        private void Btnsubmit_Click(object sender, EventArgs e)
        {

        }

        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            //itemdetails();
        }

        public void itemdetails()
        {

            try
            {

                string s1 = Txtitem.Text.Trim();
                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s1.Replace("'", "''");


                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                DataTable st = objQuotationbal.itemdetailssales(name, s2);


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
            //LblProdNotFoundMSg.Visible = false;
            DgvAutoRefNo.Visible = false;
        }

        /* Left Side Content */

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
            else if (clickstatus == "search3")
            {
                txtsearch3.Text = lblAll.Text.Trim();
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

            //txtListDate.Text = lblToday.Text.Trim();

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

            //txtListDate.Text = lblThisWeek.Text.Trim();
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

            //txtListDate.Text = lblThisMonth.Text.Trim();
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

            //txtListDate.Text = lblThisYear.Text.Trim();
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

            //txtListDate.Text = lblYesterday.Text.Trim();
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

            //txtListDate.Text = lblLastWeek.Text.Trim();
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

            //txtListDate.Text = lblLastMonth.Text.Trim();
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

            //txtListDate.Text = lblLastYear.Text.Trim();
            pnlCalender.Visible = false;
        }

        private void txtsearch1_Click(object sender, EventArgs e)
        {
            string selecteditem = cbxSearchOrderNo.Text.ToString();
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

        private void txtsearch3_Click(object sender, EventArgs e)
        {
            if (cbxVendor.Text.ToString() == "Date")
            {
                clickstatus = "search3";
                pnlCalender.BringToFront();
                pnlCalender.Visible = true;
                pnlCalender.Location = new Point(133, 103);
            }
            else
            {
                pnlCalender.Visible = false;

            }
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
            //txtListDate.Text = lblLastMonth.Text.Trim();
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

        private void cbxSearchOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string ordno = Convert.ToString(cbxSearchOrderNo.SelectedValue);

            //if (ordno == "RequestedBy")
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
            //string orddate = Convert.ToString(cbxSearchOrderDate.SelectedValue);
            //if (orddate == "RequestedBy")
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
            //string ordvender = Convert.ToString(cbxVendor.SelectedValue);
            //if (ordvender == "RequestedBy")
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

        private void btnSearch_Click(object sender, EventArgs e)
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
                    if (firstname == "Vendor")
                    {
                        firstname = "Vendor";
                        //firstvalue = Convert.ToString(cmbstatus1.SelectedValue);
                        if (!string.IsNullOrEmpty(cmbstatus1.Text) && cmbstatus1.Text != "--Select--")
                        {
                            firstvalue = cmbstatus1.Text;
                        }
                    }
                    else
                    {
                        //firstvalue = txtsearch1.Text.Trim();
                        string part1 = txtsearch1.Text.Trim();
                        if (!string.IsNullOrEmpty(part1))
                        {
                            string part = part1.Substring(0, 2);
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
                    if (secondname == "Vendor")
                    {
                        secondname = "Vendor";
                        //secondvalue = Convert.ToString(cmbstatus2.SelectedValue);
                        if (!string.IsNullOrEmpty(cmbstatus2.Text) && cmbstatus2.Text != "--Select--")
                        {
                            secondvalue = cmbstatus2.Text;
                        }
                    }
                    else
                    {
                        //secondvalue = txtsearch2.Text.Trim();
                        string part1 = txtsearch2.Text.Trim();
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


                    thirdname = cbxVendor.Text.Trim();
                    if (thirdname == "Vendor")
                    {
                        thirdname = "Vendor";
                        if (!string.IsNullOrEmpty(cmbstatus3.Text) && cmbstatus3.Text != "--Select--")
                        {
                            thirdvalue = cmbstatus3.Text;
                        }
                    }
                    else
                    {
                        //thirdvalue = txtsearch3.Text.Trim();
                        string part1 = txtsearch3.Text.Trim();
                        if (!string.IsNullOrEmpty(part1))
                        {
                            string part = part1.Substring(0, 2);
                            if (Char.IsDigit(part, 0))
                            {
                                string[] rr = txtsearch3.Text.Split('-');
                                thirdvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                            }
                            else
                            {
                                thirdvalue = txtsearch3.Text.Trim();
                            }
                        }
                    }

                    if (firstname == "Order Number")
                    {
                        firstname1 = firstname;
                        firstvalue1 = firstvalue;
                    }
                    else if (firstname == "Order Date")
                    {
                        secondname1 = firstname;
                        secondvalue1 = firstvalue;
                    }
                    else if (firstname == "Vendor")
                    {
                        thirdname1 = firstname;
                        thirdvalue1 = firstvalue;
                    }

                    if (secondname == "Order Number")
                    {
                        firstname1 = secondname;
                        firstvalue1 = secondvalue;
                    }
                    else if (secondname == "Order Date")
                    {
                        secondname1 = secondname;
                        secondvalue1 = secondvalue;
                    }
                    else if (secondname == "Vendor")
                    {
                        thirdname1 = secondname;
                        thirdvalue1 = secondvalue;
                    }

                    if (thirdname == "Order Number")
                    {
                        firstname1 = thirdname;
                        firstvalue1 = thirdvalue;
                    }
                    else if (thirdname == "Order Date")
                    {
                        secondname1 = thirdname;
                        secondvalue1 = thirdvalue;
                    }
                    else if (thirdname == "Vendor")
                    {
                        thirdname1 = thirdname;
                        thirdvalue1 = thirdvalue;
                    }
                    firstname1 = "OrderNumber";
                    secondname1 = "OrderDate";
                    thirdname1 = "Name";
                    try
                    {
                        search(firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1, role, UserId);
                    }
                    catch
                    {
                        dgvSearch.DataSource = null;
                    }
                }
            }
            catch
            {

            }
        }

        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = OblPurchaseOrderBAL.SearchPurchaseOrder(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role, UserId);
            dgvSearch.Columns.Clear();
            dgvSearch.DataSource = dt;

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            //if (dt.Rows.Count > 0)
            //{
            dgvSearch.Columns["PurchaseId"].Visible = false;
            dgvSearch.Columns["OrderDate"].HeaderText = "Date";
            //dgvSearch.Columns["OrderDate"].Visible = false;
            dgvSearch.Columns["ExpectedDeliveryDate"].Visible = false;
            dgvSearch.Columns["VendorId"].Visible = false;
            dgvSearch.Columns["Vendor"].Visible = false;
            dgvSearch.Columns["Status"].Visible = false;
            dgvSearch.Columns["Remarks"].Visible = false;
            dgvSearch.Columns["ApprovedBy"].Visible = false;
            dgvSearch.Columns["OrderNumber"].ReadOnly = true;
            dgvSearch.Columns["OrderNumber"].HeaderText = "Order Number";

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //}

        }




        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string id = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[1].Value);
                TabPurchaseOrder.SelectedIndex = 0;
                cbVendor.Enabled = true;
                GetPurchaseOrderByOrderNo(id);
                total();
                cbVendor.Focus();
                lbedit.Text = "1";
                //cbxStatus.Enabled = false;
            }
        }

        private void PurchaseOrder_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int j = 0; j < TabPurchaseOrder.TabPages.Count; j++)
                {

                    for (int i = 0; i < TabPurchaseOrder.TabPages[j].Controls.Count; i++)
                    {
                        TabPurchaseOrder.TabPages[j].Controls[i].Dispose();
                    }
                }

                TabPurchaseOrder.Dispose();

                this.Dispose();
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

        private void dgvReject_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string id = Convert.ToString(dgvReject.Rows[e.RowIndex].Cells[1].Value);
                TabPurchaseOrder.SelectedIndex = 0;
                GetPurchaseOrderByOrderNo(id);
                total();
                dateTimePicker2.Enabled = false;
                cbxStatus.Enabled = false;
                label3.Visible = false;
                //cbxStatus.SelectedIndex = 3;

                //cbxStatus.Visible = true;
            }
        }

        private void txtRemarks_Enter(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
        }

        private void DgvAutoRefNo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (!string.IsNullOrEmpty(Txtitem.Text))
            //{
            if (e.RowIndex >= 0)
            {
                if (ProdNotFoundMSg)
                {
                    // LblProdNotFoundMSg.Visible = true;
                }
                else
                {
                    //if (CheckDuplicate(lblproductid.Text) == false)
                    //{

                        int rowindex = Convert.ToInt32(lblrowindex.Text);
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];

                        //itemdetails(Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value));

                        string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                        getitems(sa);

                        dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();
                        //dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                        //double val = Convert.ToDouble(lblprice.Text);
                        //dgvOrder.Rows[rowindex].Cells[4].Value = val;
                        dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                        DgvAutoRefNo.Visible = false;

                        getsino();
                        // Txtitem.Text = string.Empty;
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
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
                        dgvOrder.BeginEdit(true);
                        //LblProdNotFoundMSg.Visible = false;
                    
                    // }
                    //else
                    //{
                    //    MessageBox.Show("Product already added.");
                    //}
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

        private void gdvapproval_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (gdvapproval.Columns[e.ColumnIndex].HeaderText == "Action")
                {
                    //string id = Convert.ToString(gdvapproval.Rows[e.RowIndex].Cells[1].Value);
                    //TabPurchaseOrder.SelectedIndex = 0;
                    //cbVendor.Enabled = false;
                    //GetPurchaseOrderByOrderNo(id);
                    //label3.Visible = true;
                    //cbxStatus.Visible = true;
                    //total();

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
            // Txtitem.Text= SendKeys.Send(e.KeyChar.ToString());
        }

        private void DgvAutoRefNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(Txtitem.Text))
                {
                    if (ProdNotFoundMSg)
                    {
                        // LblProdNotFoundMSg.Visible = true;
                    }
                    else
                    {
                        //if (CheckDuplicate(lblproductid.Text) == false)
                        //{
                            int rowindex = Convert.ToInt32(lblrowindex.Text);
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];
                            dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                            dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                            //dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                            //double val = Convert.ToDouble(lblprice.Text);
                            //dgvOrder.Rows[rowindex].Cells[4].Value = val;
                            dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                            DgvAutoRefNo.Visible = false;
                            getsino();
                            //Txtitem.Text = string.Empty;
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
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
                            dgvOrder.BeginEdit(true);
                        
                    //     }
                    //else
                    //{
                    //    MessageBox.Show("Product already added.");
                    //}
                        //LblProdNotFoundMSg.Visible = false;
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
            total();
        }


        public void GetReport(string QuotationId)
        {
            //try
            //{
            if (!string.IsNullOrEmpty(QuotationId))
            {
                DialogResult result = MessageBox.Show("Do you want to Print " + QuotationId + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Quotationreport rpt = new Quotationreport(txtorder.Text);
                    //rpt.ShowDialog();

                    using (SqlConnection con = new SqlConnection(Program.connection))
                    {
                        DataSet ds = new DataSet();
                        con.Open();


                        SqlCommand cmd = new SqlCommand("Report_PurchaseOrder_print", con);
                        cmd.Parameters.AddWithValue("@OrderNumber", QuotationId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);




                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            try
                            {

                                PurchaseOrderReport objRREPrint = new PurchaseOrderReport();
                                objRREPrint.dsMain = ds;
                                objRREPrint.pagenumber = 1;
                                objRREPrint.status = true;
                                objRREPrint.strRefTexts = "Puch:";
                                objRREPrint.strRefs = QuotationId;
                                //try
                                //{
                                //    objRREPrint.Copies = int.Parse(txtCopies.Text);
                                //}
                                //catch
                                //{
                                //    objRREPrint.Copies = 1;
                                //}
                                objRREPrint.RREPrintPurchaseOrder();
                            }

                            catch (Exception ex)
                            {

                            }
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

        private void DgvAutoRefNo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            QuotationId.Text = "";
            QuotationSearch.Visible = true;
            QuotationId.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            QuotationSearch.Visible = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            if (QuotationId.Text != "")
            {
                DataSet Quotation = OblPurchaseOrderBAL.SearchQuotationData(QuotationId.Text);
                if (Quotation.Tables[0].Rows.Count > 0)
                {
                    dgvOrder.Rows.Clear();
                    for (int i = 0; i < Quotation.Tables[0].Rows.Count; i++)
                    {
                        dgvOrder.Rows.Add();
                        dgvOrder.Rows[i].Cells[0].Value = i + 1;
                        dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(Quotation.Tables[0].Rows[i]["DisplayName"]);
                        dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(Quotation.Tables[0].Rows[i]["Quantity"]);
                        dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(Quotation.Tables[0].Rows[i]["Productid"]);
                    }

                    //panel2.Enabled = false;
                    QuotationSearch.Visible = false;
                    QuotationId.Text = "";
                }
                else
                {
                    //dgvOrder.Rows.Clear();
                    //panel2.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("QuotationId Should not be empty");
                QuotationId.Focus();
            }
        }

        private void Txtitem_KeyUp_1(object sender, KeyEventArgs e)
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

        private void Txtitem_TextChanged_1(object sender, EventArgs e)
        {
            ProdSelRowvalue = 0;
        }

        private void DgvAutoRefNo_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (ProdNotFoundMSg)
                {
                    // LblProdNotFoundMSg.Visible = true;
                }
                else
                {

                    int rowindex = Convert.ToInt32(lblrowindex.Text);
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];

                    //itemdetails(Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value));

                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                    getitems(sa);
                    //if (CheckDuplicate(lblproductid.Text) == false)
                    //{
                        dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();
                        //dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                        //double val = Convert.ToDouble(lblprice.Text);
                        //dgvOrder.Rows[rowindex].Cells[4].Value = val;
                        dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                        DgvAutoRefNo.Visible = false;

                        getsino();
                        // Txtitem.Text = string.Empty;
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
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
                        dgvOrder.BeginEdit(true);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Product already added.");
                    //}

                    //LblProdNotFoundMSg.Visible = false;
                }
            }
        }

        private void DgvAutoRefNo_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(Txtitem.Text))
                {
                    if (ProdNotFoundMSg)
                    {
                        // LblProdNotFoundMSg.Visible = true;
                    }
                    else
                    {
                        //if (CheckDuplicate(lblproductid.Text) == false)
                        //{
                            int rowindex = Convert.ToInt32(lblrowindex.Text);
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];
                            dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                            dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                            //dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                            //double val = Convert.ToDouble(lblprice.Text);
                            //dgvOrder.Rows[rowindex].Cells[4].Value = val;
                            dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                            DgvAutoRefNo.Visible = false;
                            getsino();
                            //Txtitem.Text = string.Empty;
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
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
                            dgvOrder.BeginEdit(true);
                        //}

                        //else
                        //{
                        //    MessageBox.Show("Product already added.");
                        //}
                        //LblProdNotFoundMSg.Visible = false;
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
            else if (e.KeyData == Keys.End)
            {
                //if (DgvAutoRefNo.CurrentCell.RowIndex + 1 != DgvAutoRefNo.Rows.Count)
                // {
                string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                getitemdetails(sa);
                //}

            }
        }




        public void getitemdetails(string sa)
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


        private void DgvAutoRefNo_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Up) || e.KeyChar != Convert.ToChar(Keys.Down))
            {
                Txtitem.Focus();
                SendKeys.Send(e.KeyChar.ToString());
                lblhiddenproduct.Text = Txtitem.Text;

            }
        }

        private void transactionclose_Click_1(object sender, EventArgs e)
        {
            transactionclose1();
        }
    }

}



