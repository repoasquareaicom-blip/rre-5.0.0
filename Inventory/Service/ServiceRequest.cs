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

namespace Inventory.Service
{
    public partial class ServiceRequest : Form
    {
        public DateTimePicker cellDateTimePicker;
        string chequeno_currentrow;
        DateTimePicker datetimepicker;
        DataTable dtAdd = new DataTable();
        bool first = true;
        ServiceRequestBAL objServiceRequestBAL = new ServiceRequestBAL();
        string Service = string.Empty;
        public ServiceRequest()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
           // pnlLabelSearch.Visible = true;
            // vLabel1.Visible = true;
           // pnlSearch.Visible = false;
           // splitContainer1.Panel1Collapsed = true;
            this.ActiveControl = TxtInvoiceOrdNo;
            Searchresult();
            DateTimePickerObj();
        }
        public DateTimePicker DateTimePickerObj()
        {
           
            datetimepicker = new DateTimePicker();
            datetimepicker.Format = DateTimePickerFormat.Custom;
            datetimepicker.CustomFormat = "dd-MM-yyyy";
            datetimepicker.ValueChanged += new EventHandler(cellDateTimePickerValueChanged);            
            datetimepicker.Visible = false;

            return datetimepicker;
        }
        public void cellDateTimePickerValueChanged(object sender, EventArgs e)
        {
            int row = dgvProductqty.CurrentCell.RowIndex;
            dgvProductqty.Rows[row].Cells["Date"].Value = cellDateTimePicker.Text;//convert the date as per your format
            //this.DGVChequeRealization.Focus();
        }
        private void ServiceRequest_Load(object sender, EventArgs e)
        {
            Service = Program.Service;
            if (Program.Service != "Service Request")
            {
                TxtInvoiceOrdNo.Enabled = false;
                txtcustname.Enabled = false;
                cbxProduct.Enabled = false;
                txtqty.Enabled = false;
                txtcomplients.Enabled = false;
                btnAdd.Enabled = false;
                btnclear.Enabled = false;
                btnProductSave.Enabled = false;
                btnnew.Enabled = false;
                TxtInvoiceOrdNo.Enabled = false;
            }

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

            Searchresult();
            txtsearch1.Text = string.Empty;
            txtsearch2.Text = string.Empty;
        }
        public void Searchresult()
        {
            dgvSearch.DataSource = null;
            string service = Program.Service;
            DataTable dt = objServiceRequestBAL.SearchData(txtsearch1.Text, txtsearch2.Text, service);
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

            if (string.IsNullOrEmpty(txtcustname.Text))
            {
                i++;
                message = message + "* Customer Name Should Not Be Empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtcustname;
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

            DataTable dtsa = (DataTable)dgvProductqty.DataSource;
            bool dtval = RemoveDuplicateRows(dtsa, "ProductId");
            if (dtval==false)
            {
                i++;
                message = message + "* Please Remove Duplicate  Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvProductqty;
                status = false;
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

        //public DataTable DataGridView2DataTable(DataGridView dgv, int minRow = 0)
        //{

        //    DataTable dt = new DataTable();

        //    // Header columns
        //    foreach (DataGridViewColumn column in dgv.Columns)
        //    {
        //        DataColumn dc = new DataColumn(column.Name.ToString());
        //        dt.Columns.Add(dc);
        //    }

        //    // Data cells
        //    for (int i = 0; i < dgv.Rows.Count; i++)
        //    {
        //        DataGridViewRow row = dgv.Rows[i];
        //        DataRow dr = dt.NewRow();
        //        for (int j = 0; j < dgv.Columns.Count; j++)
        //        {
        //            dr[j] = (row.Cells[j].Value == null) ? "" : row.Cells[j].Value.ToString();
        //        }
        //        dt.Rows.Add(dr);
        //    }

        //    // Related to the bug arround min size when using ExcelLibrary for export
        //    for (int i = dgv.Rows.Count; i < minRow; i++)
        //    {
        //        DataRow dr = dt.NewRow();
        //        for (int j = 0; j < dt.Columns.Count; j++)
        //        {
        //            dr[j] = "  ";
        //        }
        //        dt.Rows.Add(dr);
        //    }
        //    return dt;
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
                dgvProductqty.Rows[index].DefaultCellStyle.ForeColor = Color.Red;
                sa = false;
            }
            //dTable.Rows.Remove(dRow);


            //Datatable which contains unique records will be return as output.
            return sa;
        }

        private void btnProductSave_Click(object sender, EventArgs e)
        {
            bool Val = SaveValidation();
            if (Val == true)
            {
                SaveServicerequest();
                clear();

            }
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
            lblsalesorderno.Text = string.Empty;
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

            if (string.IsNullOrEmpty(Convert.ToString(lblproductid.Text)))
            {
                i++;
                message = message + "* Please Enter Correct Product" + "\n";
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
                if (!string.IsNullOrEmpty(lblhide.Text))
                {
                    int v = Convert.ToInt32(lblhide.Text);
                    dgvProductqty.Rows.RemoveAt(v);
                    
                }
                if (dtAdd.Rows.Count == 0)
                {
                    


                    dtAdd.Columns.Clear();
                    dtAdd.Columns.Add("ServiceProductid", typeof(string));
                    dtAdd.Columns.Add("ProductId", typeof(string));
                    dtAdd.Columns.Add("Product", typeof(string));
                    dtAdd.Columns.Add("QTY", typeof(string));
                    dtAdd.Columns.Add("Complients", typeof(string));
                    dtAdd.Columns.Add("Status", typeof(string));
                    DataRow dr = dtAdd.NewRow();
                    if(string.IsNullOrEmpty(lblproductid.Text))
                    {
                        dr["ServiceProductid"] =0;
                    }
                    else
                    {
                        dr["ServiceProductid"] = lblproductid.Text;
                    }
                    
                    dr["ProductId"] = cbxProduct.SelectedValue;
                    dr["Product"] = cbxProduct.Text;
                    dr["QTY"] = txtqty.Text;
                    dr["Complients"] = txtcomplients.Text;
                    if (string.IsNullOrEmpty(lblstatus.Text))
                    {
                        dr["Status"] = "New";
                    }
                    else
                    {
                        dr["Status"] = lblstatus.Text;
                    }

                    dtAdd.Rows.Add(dr);
                    lblhide.Text = "";
                }
                else
                {
                    DataRow dr = dtAdd.NewRow();
                    if (string.IsNullOrEmpty(lblproductid.Text))
                    {
                        dr["ServiceProductid"] = 0;
                    }
                    else
                    {
                        dr["ServiceProductid"] = lblproductid.Text;
                    }
                    dr["ProductId"] = cbxProduct.SelectedValue;
                    dr["Product"] = cbxProduct.Text;
                    dr["QTY"] = txtqty.Text;
                    dr["Complients"] = txtcomplients.Text;
                    if (string.IsNullOrEmpty(lblstatus.Text))
                    {
                        dr["Status"] = "New";
                    }
                    else
                    {
                        dr["Status"] = lblstatus.Text;
                    }
                    dtAdd.Rows.Add(dr);
                    lblhide.Text = "";
                }
                dtAdd.AcceptChanges();
                gridaddbind();
                cbxProduct.Focus();
                cbxProduct.SelectionStart = 0;
                cbxProduct.SelectionLength = cbxProduct.Text.Length;
            }
        }
        public void gridaddbind()
        {
            dgvProductqty.DataSource = dtAdd;
            try
            {
                if (dgvProductqty.Rows.Count == 0)
                {
                    cellDateTimePicker.Dispose();
                }
            }
            catch
            {

            }
            if (first)
            {
                DataGridViewImageColumn img1 = new DataGridViewImageColumn();
                img1.Image = Inventory.Properties.Resources.user_edit;
                dgvProductqty.Columns.Insert(6, img1);
                img1.HeaderText = "Edit";
                img1.Name = "Edit";
                dgvProductqty.Columns["Edit"].Width = 50;
                DataGridViewImageColumn img2 = new DataGridViewImageColumn();
                img2.Image = Inventory.Properties.Resources.trash;
                dgvProductqty.Columns.Insert(7, img2);
                img2.HeaderText = "Delete";
                img2.Name = "Delete";
                dgvProductqty.Columns["Delete"].Width = 50;
                dgvProductqty.Columns["ProductId"].Visible = false;
                dgvProductqty.Columns["ServiceProductid"].Visible = false;
                dgvProductqty.Columns["Status"].ReadOnly = true;
                dgvProductqty.Columns["Product"].ReadOnly = true;
                dgvProductqty.Columns["QTY"].ReadOnly = true;
                dgvProductqty.Columns["Complients"].ReadOnly = true; 
                first = false;
            }
            txtqty.Text = string.Empty;
            txtcomplients.Text = string.Empty;
            try
            {
                cbxProduct.SelectedIndex = 0;
            }
            catch(Exception sa) 
            { 
            }
            
        }
        private void TxtInvoiceOrdNo_Leave(object sender, EventArgs e)
        {
            serviceordernumberchange();
        }
        public void serviceordernumberchange()
        {
            if (Program.Service == "Service Request")
            {
                if (objServiceRequestBAL.checksalesbillnumber(TxtInvoiceOrdNo.Text.Trim()))
                {
                    if (!string.IsNullOrEmpty(lblsalesorderno.Text))
                    {
                        if (lblsalesorderno.Text != TxtInvoiceOrdNo.Text.Trim())
                        {
                            dgvProductqty.Columns.Clear();
                            dgvProductqty.DataSource = null;
                            dtAdd.Rows.Clear();
                            first = true;
                            //gridaddbind();
                        }
                    }
                    cbxProduct.Enabled = true;
                    txtqty.Enabled = true;
                    txtcomplients.Enabled = true;
                    btnAdd.Enabled = true;
                    btnproductClear.Enabled = true;
                    DataTable dt = objServiceRequestBAL.GetProduct(TxtInvoiceOrdNo.Text.Trim());

                    DataRow row = dt.NewRow();
                    row["Productid"] = "0";
                    row["DisplayName"] = "-Select-";
                    dt.Rows.InsertAt(row, 0);
                    cbxProduct.DataSource = dt;
                    cbxProduct.ValueMember = "Productid";
                    cbxProduct.DisplayMember = "DisplayName";
                    cbxProduct.SelectedIndex = 0;
                    lblsalesorderno.Text = TxtInvoiceOrdNo.Text.Trim();
                }
                else
                {
                    cbxProduct.Enabled = false;
                    txtqty.Enabled = false;
                    txtcomplients.Enabled = false;
                    btnAdd.Enabled = false;
                    lblsalesorderno.Text = string.Empty;
                    dgvProductqty.Columns.Clear();
                    dgvProductqty.DataSource = null;
                    dtAdd.Rows.Clear();
                    first = true;

                    // MessageBox.Show("* Error" + "\n" + "----------------------------------------" + "\n" + "Invalid Sales Order Number");               
                }
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
            if (btnproductClear.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnProductSave;
                    return true;
                }
            }
            if (btnclear.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = TxtInvoiceOrdNo;
                    return true;
                }
            }
            if (cbxProduct.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = txtqty;
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
        

        private void dgvProductqty_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvProductqty.Columns[e.ColumnIndex].HeaderText == "Date")
                {
                    datetimepicker.Visible = true;
                    Rectangle tempRect = dgvProductqty.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    cellDateTimePicker.Location = tempRect.Location;
                    cellDateTimePicker.Width = tempRect.Width;     
                    //cellDateTimePicker
                    this.ActiveControl = cellDateTimePicker;
                    dgvProductqty.Visible = true;

                }
                if (dgvProductqty.Columns[e.ColumnIndex].HeaderText == "Delete")
                {
                    string val = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["Status"].Value);
                    if (val == "New")
                    {
                        dtAdd.Rows.Remove(dtAdd.Rows[e.RowIndex]);
                        gridaddbind();
                    }
                }
                if (dgvProductqty.Columns[e.ColumnIndex].HeaderText == "Edit")
                {
                    lblproductid.Text = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["ServiceProductid"].Value);
                    cbxProduct.SelectedValue = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["ProductId"].Value);
                    txtqty.Text = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["QTY"].Value);
                    txtcomplients.Text = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["Complients"].Value);
                    lblstatus.Text = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["Status"].Value);
                    lblhide.Text = e.RowIndex.ToString();
                }
                if (dgvProductqty.Columns[e.ColumnIndex].HeaderText == "Status Update")
                {
                    if (Program.Service == "Service Completed")
                    {
                        if (dgvProductqty.Rows.Count>0)
                        {
                        DateTime date;
                        string ServiceProductId = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["ServiceProductid"].Value);
                        string Status = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["cmb"].Value);
                        string Remark = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["ReceviedCustomer"].Value);

                       
                        string Date = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["Date"].Value);
                        if (string.IsNullOrEmpty(Date))
                        {
                            date = new DateTime(cellDateTimePicker.Value.Year, cellDateTimePicker.Value.Month, cellDateTimePicker.Value.Day);
                        }
                        else
                        {
                        string[] day=Date.Split('-');
                         date = new DateTime(Convert.ToInt32(day[2]), Convert.ToInt32(day[1]), Convert.ToInt32(day[0]));
                        }

                        if (!string.IsNullOrEmpty(Status) && Status != "-Select-")
                        {
                            objServiceRequestBAL.StatusUpdatePagecomplete(ServiceProductId, Status, Remark, date);
                        }
                        else
                        {
                            MessageBox.Show("Please Select the status of the Product.");
                        }
                        }
                    }
                    else
                    {
                        if (dgvProductqty.Rows.Count > 0)
                        {
                            string ServiceProductId = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["ServiceProductid"].Value);
                            string Status = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["cmb"].Value);
                            string Remark = Convert.ToString(dgvProductqty.Rows[e.RowIndex].Cells["Remark"].Value);
                            if (!string.IsNullOrEmpty(Status) && Status != "-Select-")
                            {
                                objServiceRequestBAL.StatusUpdatePage(ServiceProductId, Status, Remark);
                            }
                            else
                            {
                                MessageBox.Show("Please Select the status of the Product.");
                            }
                        }
                    }
                    dtAdd = objServiceRequestBAL.GetserviceProduct(lblservicerequestid.Text.Trim(), Service);
                    gridaddbind();
                }
            }
        }
        private void datebind()
        {
            DateTime dt = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            for (int i = 0; i < dgvProductqty.Rows.Count; i++)
            {
                // DGVChequeRealization.Rows[i].Cells[0].Value = i + 1;
                dgvProductqty.Rows[i].Cells["Date"].Value = Convert.ToString(dt.Date.AddMonths(i).ToString("dd-MM-yyyy"));
                dgvProductqty.CellClick += new DataGridViewCellEventHandler(dgvProductqty_CellClick);
            }
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (Program.Service == "Service Request")
                {
                    lblservicerequestid.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ServiceReqId"].Value);
                    TxtInvoiceOrdNo.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["SalesOrderno"].Value);
                    lblsalesorderno.Text = TxtInvoiceOrdNo.Text;
                    txtcustname.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Customer"].Value);
                    dtAdd = objServiceRequestBAL.GetserviceProduct(lblservicerequestid.Text.Trim(), Service);
                    
                    dgvProductqty.DataSource = dtAdd;
                    if (first)
                    {
                        DataGridViewImageColumn img1 = new DataGridViewImageColumn();
                        img1.Image = Inventory.Properties.Resources.user_edit;
                        dgvProductqty.Columns.Insert(6, img1);
                        img1.HeaderText = "Edit";
                        img1.Name = "Edit";
                        dgvProductqty.Columns["Edit"].Width = 50;
                        DataGridViewImageColumn img2 = new DataGridViewImageColumn();
                        img2.Image = Inventory.Properties.Resources.trash;
                        dgvProductqty.Columns.Insert(7, img2);
                        img2.HeaderText = "Delete";
                        img2.Name = "Delete";
                        dgvProductqty.Columns["Delete"].Width = 50;
                        dgvProductqty.Columns["ProductId"].Visible = false;
                        dgvProductqty.Columns["ServiceProductid"].Visible = false;
                        dgvProductqty.Columns["Status"].Visible = false;
                        dgvProductqty.Columns["Status"].ReadOnly = true;
                        dgvProductqty.Columns["Product"].ReadOnly = true;
                        dgvProductqty.Columns["QTY"].ReadOnly = true;
                        dgvProductqty.Columns["Complients"].ReadOnly = true; 
                        first = false;
                    }
                }
                else if (Program.Service == "Service Update")
                {
                    lblservicerequestid.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ServiceReqId"].Value);
                    TxtInvoiceOrdNo.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["SalesOrderno"].Value);
                    txtcustname.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Customer"].Value);
                    dtAdd = objServiceRequestBAL.GetserviceProduct(lblservicerequestid.Text.Trim(), Service);
                    
                    dgvProductqty.DataSource = dtAdd;
                    if (first)
                    {
                        DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
                        cmb.HeaderText = "Product Status";
                        cmb.Name = "cmb";
                        cmb.MaxDropDownItems = 4;
                        cmb.Items.Add("-Select-");
                        cmb.Items.Add("Completed");
                        cmb.Items.Add("Inprogress");
                        cmb.Items.Add("Cancel");
                        cmb.DefaultCellStyle.NullValue = "-Select-";
                        cmb.FlatStyle = FlatStyle.Popup;
                        dgvProductqty.Columns.Insert(6, cmb);
                        DataGridViewTextBoxColumn txt = new DataGridViewTextBoxColumn();
                        txt.HeaderText = "Remark";
                        txt.Name = "Remark";
                        dgvProductqty.Columns.Insert(7, txt);
                        DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
                        btn.HeaderText = "Status Update";
                        btn.Text = "Update";
                        btn.Name = "btn";                    
                        btn.UseColumnTextForButtonValue = true;
                        btn.FlatStyle = FlatStyle.Popup;
                        dgvProductqty.Columns.Insert(8, btn);
                        dgvProductqty.Columns[8].Width = 100;
                        dgvProductqty.Columns["ProductId"].Visible = false;
                        dgvProductqty.Columns["ServiceProductid"].Visible = false;
                        dgvProductqty.Columns["Status"].Visible = false;

                        dgvProductqty.Columns["Status"].ReadOnly = true;
                        dgvProductqty.Columns["Product"].ReadOnly = true;
                        dgvProductqty.Columns["QTY"].ReadOnly = true;
                        dgvProductqty.Columns["Complients"].ReadOnly = true; 
                        first = false;
                    }
                    dgvProductqty.Focus();
                    if (dgvProductqty.Rows.Count>0)
                    {
                   // dgvProductqty.CurrentCell = dgvProductqty[6, 0];
                        dgvProductqty.CurrentCell = dgvProductqty.Rows[0].Cells["cmb"];
                    }
                }
                else if (Program.Service == "Service Completed")
                {
                    lblservicerequestid.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ServiceReqId"].Value);
                    TxtInvoiceOrdNo.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["SalesOrderno"].Value);
                    txtcustname.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Customer"].Value);
                    dtAdd = objServiceRequestBAL.GetserviceProduct(lblservicerequestid.Text.Trim(), Service);
                    
                    dgvProductqty.DataSource = dtAdd;                   
                    if (first)
                    {
                        DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
                        cmb.HeaderText = "Select Data";
                        cmb.Name = "cmb";
                        cmb.MaxDropDownItems = 4;
                        cmb.Items.Add("-Select-");
                        cmb.Items.Add("Delivered");
                        cmb.Items.Add("Cancel");
                        cmb.DefaultCellStyle.NullValue = "-Select-";
                        cmb.FlatStyle = FlatStyle.Popup;
                        dgvProductqty.Columns.Insert(6, cmb);

                        DataGridViewTextBoxColumn txt = new DataGridViewTextBoxColumn();
                        txt.HeaderText = "Recevied Customer";
                        txt.Name = "ReceviedCustomer";
                        dgvProductqty.Columns.Insert(7, txt);
                        
                    
                        cellDateTimePicker = DateTimePickerObj();
                        if (dtAdd.Rows.Count <= 0)
                        {
                            cellDateTimePicker.Visible = false;
                        }
                        this.dgvProductqty.Controls.Add(cellDateTimePicker);
                        dgvProductqty_CellClick(dgvProductqty, new DataGridViewCellEventArgs(6, 0));
                        DataGridViewTextBoxColumn txtdate = new DataGridViewTextBoxColumn();
                        txtdate.HeaderText = "Date";
                        txtdate.Name = "Date";
                        dgvProductqty.Columns.Insert(8, txtdate);
                        DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
                        btn.HeaderText = "Status Update";
                        btn.Text = "Update";
                        btn.Name = "btn";
                        btn.UseColumnTextForButtonValue = true;
                        btn.FlatStyle = FlatStyle.Popup;
                        dgvProductqty.Columns.Insert(9, btn);
                        dgvProductqty.Columns[9].Width = 100;
                        dgvProductqty.Columns["ProductId"].Visible = false;

                        dgvProductqty.Columns["ServiceProductid"].Visible = false;
                        dgvProductqty.Columns["Status"].Visible = false;
                        dgvProductqty.Columns["Status"].ReadOnly = true;
                        dgvProductqty.Columns["Product"].ReadOnly = true;
                        dgvProductqty.Columns["QTY"].ReadOnly = true;
                        dgvProductqty.Columns["Complients"].ReadOnly = true; 
                        first = false;
                    }
                    datebind();
                    dgvProductqty.Focus();
                    if (dgvProductqty.Rows.Count > 0)
                    {
                        dgvProductqty.CurrentCell = dgvProductqty.Rows[0].Cells["cmb"];
                    }
                }
                else if (Program.Service == "Service Issued")
                {
                    lblservicerequestid.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ServiceReqId"].Value);
                    TxtInvoiceOrdNo.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["SalesOrderno"].Value);
                    txtcustname.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Customer"].Value);
                    dtAdd = objServiceRequestBAL.GetserviceProduct(lblservicerequestid.Text.Trim(), Service);
                    
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
                }
                else if (Program.Service == "Service Cancelled")
                {
                    lblservicerequestid.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ServiceReqId"].Value);
                    TxtInvoiceOrdNo.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["SalesOrderno"].Value);
                    txtcustname.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Customer"].Value);
                    dtAdd = objServiceRequestBAL.GetserviceProduct(lblservicerequestid.Text.Trim(), Service);
                    
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
                }
                serviceordernumberchange();
            }
        }

        private void btnproductClear_Click(object sender, EventArgs e)
        {
            lblproductid.Text = string.Empty;
            cbxProduct.SelectedValue = 0;
            cbxProduct.Text = string.Empty;
            txtqty.Text = string.Empty;
            txtcomplients.Text = string.Empty;
            lblstatus.Text = string.Empty;
        }

        private void cbxProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbxProduct.SelectedIndex>0)
            {
                lblproductid.Text = Convert.ToString(cbxProduct.SelectedValue);
            }
            else
            {
                lblproductid.Text = "";
            }
             
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvProductqty_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvProductqty.Rows.Count > 0)
            {

                if (dgvProductqty.Columns[dgvProductqty.CurrentCell.ColumnIndex].HeaderText == "Recevied Customer")
                {
                    if (e.KeyData == Keys.Tab)
                    {
                        dgvProductqty_CellClick(dgvProductqty, new DataGridViewCellEventArgs(dgvProductqty.CurrentCell.ColumnIndex + 1, dgvProductqty.CurrentCell.RowIndex));
                        // dgvProductqty.Rows[dgvProductqty.CurrentCell.RowIndex].Cells["Date"].
                    }
                }
                if (e.KeyData == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    if (dgvProductqty.Columns[dgvProductqty.CurrentCell.ColumnIndex].HeaderText == "Status Update")
                    {
                        if (Program.Service == "Service Completed")
                        {
                            string ServiceProductId = Convert.ToString(dgvProductqty.Rows[dgvProductqty.CurrentCell.RowIndex].Cells["ServiceProductid"].Value);
                            string Status = Convert.ToString(dgvProductqty.Rows[dgvProductqty.CurrentCell.RowIndex].Cells["cmb"].Value);
                            string Remark = Convert.ToString(dgvProductqty.Rows[dgvProductqty.CurrentCell.RowIndex].Cells["ReceviedCustomer"].Value);


                            string Date = Convert.ToString(dgvProductqty.Rows[dgvProductqty.CurrentCell.RowIndex].Cells["Date"].Value);
                            string[] day = Date.Split('-');
                            DateTime date = new DateTime(Convert.ToInt32(day[2]), Convert.ToInt32(day[1]), Convert.ToInt32(day[0]));

                            if (!string.IsNullOrEmpty(Status) && Status != "-Select-")
                            {
                                objServiceRequestBAL.StatusUpdatePagecomplete(ServiceProductId, Status, Remark, date);
                            }
                            else
                            {
                                MessageBox.Show("Please Select the status of the Product.");
                            }
                            //objServiceRequestBAL.StatusUpdatePagecomplete(ServiceProductId, Status, Remark);
                        }
                        else
                        {
                            string ServiceProductId = Convert.ToString(dgvProductqty.Rows[dgvProductqty.CurrentCell.RowIndex].Cells["ServiceProductid"].Value);
                            string Status = Convert.ToString(dgvProductqty.Rows[dgvProductqty.CurrentCell.RowIndex].Cells["cmb"].Value);
                            string Remark = Convert.ToString(dgvProductqty.Rows[dgvProductqty.CurrentCell.RowIndex].Cells["Remark"].Value);
                            if (!string.IsNullOrEmpty(Status) && Status != "-Select-")
                            {
                                objServiceRequestBAL.StatusUpdatePage(ServiceProductId, Status, Remark);
                            }
                            else
                            {
                                MessageBox.Show("Please Select the status of the Product.");
                            }
                        }
                        dtAdd = objServiceRequestBAL.GetserviceProduct(lblservicerequestid.Text.Trim(), Service);
                        gridaddbind();
                    }
                }
            }
        }

        private void txtqty_KeyPress(object sender, KeyPressEventArgs e)
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

        private void dgvProductqty_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

//Procedure

//checksalesbillno
//SaveServiceRequest
//serviceproduct
//DeleteServiceRequest
//SaveServiceRequestProduct
//GetserviceProduct
//SearchServiceRequest
//UpdateServiceRefStatus
//SearchServiceRequestupdate

//table
//ServiceRequest
//ServiceRequestProduct