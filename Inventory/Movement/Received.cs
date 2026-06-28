
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
using System.Collections;
using IssuedReceived;


namespace Inventory.Sales
{
    public partial class Received : Form
    {
        bool all = false;
        bool load = false;
        string role1 = string.Empty;
        string srole = string.Empty;
        QuotationBal objQuotationbal = new QuotationBal();
        DataTable dtreceivedbalance, dtpaidbalance;
        ComboBox cmblocation;
        DataTable dtitems;
        TextBox tb, tbamount, tbbaalanceanount, tborderquantoty;
        DataGridViewComboBoxColumn name = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn name1 = new DataGridViewComboBoxColumn();
        public bool edit = false;
        string clickstatus = string.Empty;
        string selectedtab = string.Empty;
        string cas = string.Empty;
        int ProdSelRowvalue = 0;
        bool res = false;
        bool ProdNotFoundMSg = false;
        public Received()
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
            LoadPortsNew();

            Getapproval("");



            bindLocation();

            cmbloaction.SelectedIndex = 0;
            Globeimage();

            ddlpaymenttype.SelectedIndex = 0;

            DataTable dtitems = Program.dtitems;
            if (dtitems == null)
            {
                itemdetails("");
            }







            SearchPurchaseOrder();

            GetSearchSalesOrder();
            bind();

            bindcustomer();
            LoadPortsApproval();

            selectedtab = MainTabSalesBill.SelectedTab.Name;

            if (selectedtab == "TabNew")
            {

                btnsave.Enabled = true;
                btnSavePending.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = true;
            }



            else if (selectedtab == "TabChecking")
            {

                btnsave.Enabled = false;
                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
            }

            if (role1 != "Admin")
            {
                DataTable dt1 = Program.Dtmenu;
                bool contains = dt1.AsEnumerable()
                    .Any(row => "ReceivedApproval" == row.Field<String>("Data"));
                if (contains == false)
                {
                    MainTabSalesBill.TabPages.Remove(TabPayment);
                }

            }




        }

        public void Globeimage()
        {
            ////string pathname = Path.Combine(Environment.CurrentDirectory);
            //string pathname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
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



            //    pictureBox4.Image = Image.FromStream(ms);
            //    pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;

            //    pictureBox5.Image = Image.FromStream(ms);
            //    pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;


            //}
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
                //this.dgvSearch.Columns[1].Visible = false;
                //this.dgvSearch.Columns[2].Visible = false;

            }
        }


        #endregion

        #region SalesBillTabPage
        private void LoadPortsNew()
        {
            DataTable dt = new DataTable();
            dgvNew.Rows.Clear();
            dgvNew.ColumnCount = 7;
            //dgvOrder.RowCount = 16;

            dgvNew.Columns[0].Name = "S.NO";
            dgvNew.Columns[1].Name = "Items";
            dgvNew.Columns[2].Name = "UOM";
            dgvNew.Columns[5].Name = "Quantity";
            dgvNew.Columns[3].Name = "productid";
            dgvNew.Columns[4].Name = "Rate";
            dgvNew.Columns[6].Name = "Amount";

            dgvNew.Columns[3].Visible = false;

            this.dgvNew.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvNew.Columns[0].Width = 15;
            this.dgvNew.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvNew.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvNew.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvNew.Columns[1].Width = 100;
            this.dgvNew.Columns[2].Width = 15;
            this.dgvNew.Columns[0].ReadOnly = true;
            this.dgvNew.Columns[1].ReadOnly = true;
            this.dgvNew.Columns[2].ReadOnly = true;
            this.dgvNew.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvNew.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvNew.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            this.dgvNew.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvNew.Columns[4].Width = 15;
            this.dgvNew.Columns[4].ReadOnly = true;

            this.dgvNew.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvNew.Columns[5].Width = 20;

            dgvNew.Columns[4].DefaultCellStyle.Format = "N2";
            dgvNew.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvNew.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvNew.Columns[6].Width = 100;

            this.dgvNew.Columns[6].ReadOnly = true;
            dgvNew.Columns[6].Visible = false;
            dgvNew.Columns[4].Visible = false;


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
            dgvNew.Columns.Insert(6, name);
            dgvNew.Columns["Location"].ReadOnly = true;
            dt = objQuotationbal.GetFloor();
            if (dt.Rows.Count > 0)
            {
                name.DataSource = dt;
            }

        }


        private void LoadPortsApproval()
        {
            DataTable dt = new DataTable();
            dgvPayment.Rows.Clear();
            dgvPayment.ColumnCount = 7;
            //dgvOrder.RowCount = 16;

            dgvPayment.Columns[0].Name = "S.NO";
            dgvPayment.Columns[1].Name = "Items";
            dgvPayment.Columns[2].Name = "UOM";
            dgvPayment.Columns[5].Name = "Quantity";
            dgvPayment.Columns[3].Name = "productid";
            dgvPayment.Columns[4].Name = "Rate";
            dgvPayment.Columns[6].Name = "Amount";

            dgvPayment.Columns[3].Visible = false;

            this.dgvPayment.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvPayment.Columns[0].Width = 15;
            this.dgvPayment.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPayment.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvPayment.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvPayment.Columns[1].Width = 100;
            this.dgvPayment.Columns[2].Width = 15;
            this.dgvPayment.Columns[0].ReadOnly = true;
            this.dgvPayment.Columns[1].ReadOnly = true;
            this.dgvPayment.Columns[2].ReadOnly = true;
            this.dgvPayment.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayment.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvPayment.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            this.dgvPayment.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvPayment.Columns[4].Width = 15;
            this.dgvPayment.Columns[4].ReadOnly = true;

            this.dgvPayment.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvPayment.Columns[5].Width = 20;

            dgvPayment.Columns[4].DefaultCellStyle.Format = "N2";
            dgvPayment.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvPayment.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvPayment.Columns[6].Width = 100;

            // this.dgvPayment.Columns[6].ReadOnly = true;
            dgvPayment.Columns[6].Visible = false;
            dgvPayment.Columns[4].Visible = false;


            dgvPayment.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvPayment.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvPayment.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvPayment.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


            name1.HeaderText = "Location";
            name1.Name = "Location";
            name1.DataPropertyName = "LocationID";
            name1.DisplayMember = "LocationName";
            name1.ValueMember = "LocationID";
            name1.FlatStyle = FlatStyle.Popup;
            dgvPayment.Columns.Insert(6, name1);


            dgvPayment.Columns["Location"].ReadOnly = true;
            dt = objQuotationbal.GetFloor();
            if (dt.Rows.Count > 0)
            {
                name1.DataSource = dt;
            }

        }







        private void MainTabSalesBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedtab = MainTabSalesBill.SelectedTab.Name;

            selectedtab = MainTabSalesBill.SelectedTab.Name;

            if (selectedtab == "TabNew")
            {

                btnsave.Enabled = true;
                btnSavePending.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = true;


                txtreceivedid.Text = string.Empty;
                Txtappcutomer.Text = string.Empty;
                Txtestimation.Text = string.Empty;
                dgvPayment.Rows.Clear();
                btnsave.Enabled = true;
            }


            else if (selectedtab == "TabPayment")
            {
                btnsave.Enabled = false;
                btnSavePending.Enabled = false;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;



                pnsearch.Visible = false;
                Txtcustomername.SelectedIndex = 0;
                dgvNew.Rows.Clear();
                //lbltotalamount.Text = "0.00";
                //txtless.Text = "0.00";
                //lblTotal.Text = "0.00";
                Lblhidden.Text = string.Empty;
                CmbEstimationid.DataSource = null;
                txtRemarks.Text = string.Empty;
                cmbloaction.SelectedIndex = 0;
                panel9.Enabled = true;
                btnsave.Enabled = true;
                Txtcustomername.Text = string.Empty;
                CmbEstimationid.Text = string.Empty;
                Txtcustomername.Enabled = true;
                CmbEstimationid.Enabled = true;
                Txtcustomername.Focus();
                btnsave.Enabled = false;

            }







        }
        #endregion


        public void AutoCompleteLoad(string s, int t)
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.getproductautocomplteestimation(s, t, CmbEstimationid.Text);

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
                // lblrack.Text = "0";
                lbldisplay.Text = "0";
                lbldemo.Text = "0";
                lblservice.Text = "0";
                lbldamage.Text = "0";
                lblprice.Text = "0";
            }

        }

        public AutoCompleteStringCollection Autocusomer()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.Getcustomer();
            string[] arr = new string[st.Rows.Count];
            for (int i = 0; i < st.Rows.Count; i++)
            {
                arr[i] = st.Rows[i]["Name"].ToString();
            }
            for (int i = 0; i < arr.Length; i++)
            {
                //var combined = string.Join(", ", arr);
                var combined = arr[i];
                str.Add(combined);
            }
            //for (int i = 0; i < arr.Length; i++)
            //{
            // var combined = string.Join(", ", arr);
            //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            // str.Add(combined);
            //}

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}

            return str;
        }

        private void TabPaymentReceived_Click(object sender, EventArgs e)
        {

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

        public bool validationPaymentCheque()
        {
            bool Status = true;
            string msg = "";
            int i = 0;


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

            this.ActiveControl = Txtcustomername;

            Txtcustomername.Text = string.Empty;
            load = true;
            //if(string.IsNullOrEmpty(Txtcustomername.Text))
            //{
            //    load = false;
            //}

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {



                if (keyData == (Keys.Alt | Keys.Insert))
                {
                    if (dgvNew.Rows.Count <= 0)
                    {
                        dgvNew.Rows.Add();
                    }
                    else
                    {
                        int rowindex = dgvNew.CurrentRow.Index;
                        int colindex = dgvNew.CurrentCell.ColumnIndex;
                        //dgvOrder.Rows.Insert(rowindex, dgvOrder.Rows.Add(1));
                        dgvNew.Rows.Insert(rowindex, 1);
                        getsino();
                        return true;
                    }

                   

                }

                if (keyData == (Keys.Alt | Keys.Delete))
                {
                    DialogResult result = MessageBox.Show("Do you want to Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (dgvNew.Rows.Count > 0)
                        {
                            try
                            {
                                int rowindex = dgvNew.CurrentRow.Index;
                                int colindex = dgvNew.CurrentCell.ColumnIndex;
                                dgvNew.Rows.RemoveAt(rowindex);
                            }
                            catch
                            {

                            }
                            pnsearch.Visible = false;
                            getsino();
                            return true;
                        }

                        if (dgvNew.Rows.Count == 0)
                        {
                            dgvNew.Rows.Add();
                            getsino();
                        }
                       
                    }

                }


                if (Txtcustomername.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = CmbEstimationid;
                        //dgvNew.Focus();
                        //dgvNew.CurrentCell = dgvNew[1, dgvNew.CurrentCell.RowIndex];
                        //this.ActiveControl = dgvNew;
                        //dgvOrder.BeginEdit(true);
                        //pnsearch.Visible = false;
                        return true;
                    }
                }

                if (CmbEstimationid.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        //this.ActiveControl = CmbEstimationid;
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[1, dgvNew.CurrentCell.RowIndex];
                        this.ActiveControl = dgvNew;
                        //dgvOrder.BeginEdit(true);
                        //pnsearch.Visible = false;
                        return true;
                    }
                }



                if (button2.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        //this.ActiveControl = txtRemarks;
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[dgvNew.CurrentCell.ColumnIndex + 1, dgvNew.CurrentCell.RowIndex];
                        //this.ActiveControl = dgvNew;
                        this.ActiveControl = txtRemarks;
                        //dgvOrder.BeginEdit(true);
                        pnsearch.Visible = false;
                        return true;
                    }
                }


                if (txtRemarks.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        this.ActiveControl = btnsave;
                        btnsave.Focus();
                        return true;
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


                if (keyData == Keys.Escape)
                {
                    if (pnsearch.Visible)
                    {
                        pnsearch.Visible = false;
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew[2, dgvNew.CurrentCell.RowIndex];
                       
                        return true;
                    }
                    else
                    {
                        if (dgvNew.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[0].Cells[1].Value)))
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
                   
                }




                if (txtRemarks.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        btnsave.Focus();


                        return true;
                    }
                }

                if (keyData == Keys.Escape)
                {
                    pnsearch.Visible = false;
                    dgvNew.Focus();
                    dgvNew.CurrentCell = dgvNew[2, dgvNew.CurrentCell.RowIndex];
                    return true;
                }




                if (cmbloaction.Focused)
                {
                    try
                    {
                        if (keyData == (Keys.Tab))
                        {
                            this.ActiveControl = Txtitem;
                            return true;
                        }
                    }
                    catch
                    {

                    }

                }


                if (cmbloaction.Focused)
                {
                    try
                    {
                        if (keyData == (Keys.Tab))
                        {
                            this.ActiveControl = Txtitem;
                            return true;
                        }
                    }
                    catch
                    {

                    }

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


        private void btncreditsave_Click(object sender, EventArgs e)
        {
            if (selectedtab == "TabNew")
            {
                bool vali = Validation1();
                
                if (vali)
                {
                    save();
                    GetSearchSalesOrder();
                    //search("Returnid", "", "sr.Updatedon", "Today", "CustomerName", "", role1, Program.userid);
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



        private bool Validation1()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (Txtcustomername.Text == "--Select--" || string.IsNullOrEmpty(Txtcustomername.Text))
            {
                i++;
                message = message + "* Please Enter Customer Name" + "\n";
                if (i == 1)
                    this.ActiveControl = Txtcustomername;
            }

            if (CmbEstimationid.Text == "--Select--" || string.IsNullOrEmpty(CmbEstimationid.Text))
            {
                i++;
                message = message + "* Please Enter Estimationid" + "\n";
                if (i == 1)
                    this.ActiveControl = CmbEstimationid;
            }




            //if (string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[0].Cells[1].Value)) && string.IsNullOrEmpty(Convert.ToString(dgvNew.Rows[0].Cells["Quantity"].Value)))
            //{
            //    i++;
            //    message = message + "* Please Enter Product" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = dgvNew;
            //}




            if (dgvNew.Rows.Count > 0)
            {
                i++;
                string Items = Convert.ToString(dgvNew.Rows[0].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvNew.Rows[0].Cells["Items"].Value);
                if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                {
                    message = message + "* Please Enter Product" + "\n";
                }

                if (i == 1)
                    this.ActiveControl = dgvNew;
            }
            else if (dgvNew.Rows.Count == 0)
            {
                i++;
                message = message + "* Please Enter Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvNew;
            }

            bool sas = false;

            for (int k = 0; k < dgvNew.RowCount; k++)
            {
                string Items = Convert.ToString(dgvNew.Rows[k].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvNew.Rows[k].Cells["Items"].Value);

                if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))) || Items == ".")
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

            try
            {
                for (int g = 0; g < dgvNew.RowCount; g++)
                {
                    double qty = Convert.ToDouble(dgvNew.Rows[g].Cells[5].Value);
                    double orgqty = Convert.ToDouble(dgvNew.Rows[g].Cells[4].Value);

                    if (orgqty < qty)
                    {
                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                        message = message + "* Please Enter Correct Quantity." + "\n";
                        // MessageBox.Show("Please Enter Correct Quantity");
                        status = false;
                        break;
                    }
                    else
                    {
                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                        dgvNew.Rows[dgvNew.CurrentCell.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
            catch
            {

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
                pnsearch.Visible = false;
                Txtcustomername.SelectedIndex = 0;
                dgvNew.Rows.Clear();
                //lbltotalamount.Text = "0.00";
                //txtless.Text = "0.00";
                //lblTotal.Text = "0.00";
                Lblhidden.Text = string.Empty;
                CmbEstimationid.DataSource = null;
                txtRemarks.Text = string.Empty;
                cmbloaction.SelectedIndex = 0;
                panel9.Enabled = true;
                btnsave.Enabled = true;
                Txtcustomername.Text = string.Empty;
                CmbEstimationid.Text = string.Empty;
                Txtcustomername.Enabled = true;
                CmbEstimationid.Enabled = true;
                Txtcustomername.Focus();
            }
            else
            {
                txtreceivedid.Text = string.Empty;
                Txtappcutomer.Text = string.Empty;
                Txtestimation.Text = string.Empty;
                dgvPayment.Rows.Clear();
            }




        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            clear();
        }

        public void bindcustomer()
        {
            DataTable dt = objQuotationbal.getestimationcustomer();
            Txtcustomername.DataSource = dt;
            Txtcustomername.DisplayMember = "CustomerName";
            Txtcustomername.ValueMember = "Customerid";

            
        }

        public void bind()
        {
            //DataTable dt = objQuotationbal.getestimationcustomer();
            //cmbcustomer.DataSource = dt;
            //cmbcustomer.DisplayMember = "CustomerName";
            //cmbcustomer.ValueMember = "Customerid";
        }

        public DataTable bindEstimation()
        {
            DataTable dt = new DataTable();
            dt = objQuotationbal.itemSalesreturnorderno();
            DataRow dr = dt.NewRow();
            dr["Returnid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);

            return dt;
        }

        public DataTable bindcheckout()
        {
            DataTable dt = new DataTable();
            dt = objQuotationbal.getSalesreturncheckorderno();
            DataRow dr = dt.NewRow();
            dr["Returnid"] = "-Select-";
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

        private void dgvNew_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 1)
            {
                pnsearch.Visible = true;
                Txtcustomername.Enabled = false;
                CmbEstimationid.Enabled = false;
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
                else
                {
                    Txtitem.Text = string.Empty;
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
            //else
            //{
            //    pnsearch.Visible = false; ;
            //}
        }

        private void dgvNew_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double qty = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value);
                double orgqty = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value);

                if (orgqty < qty)
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
            catch
            {

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
                if (e.KeyData == Keys.Enter)
                {
                    e.SuppressKeyPress = true;

                    if (dgvNew.Rows.Count > 0)
                    {
                        if (dgvNew.CurrentCell.ColumnIndex == 0)
                        {
                            dgvNew.Focus();
                            dgvNew.CurrentCell = dgvNew[1, dgvNew.CurrentCell.RowIndex];

                        }
                        else if (dgvNew.CurrentCell.ColumnIndex == 1)
                        {
                            dgvNew.Focus();
                            dgvNew.CurrentCell = dgvNew[2, dgvNew.CurrentCell.RowIndex];

                        }
                        else if (dgvNew.CurrentCell.ColumnIndex == 2)
                        {
                            dgvNew.Focus();
                            dgvNew.CurrentCell = dgvNew[4, dgvNew.CurrentCell.RowIndex];

                        }
                        else if (dgvNew.CurrentCell.ColumnIndex == 4)
                        {
                            dgvNew.Focus();
                            dgvNew.CurrentCell = dgvNew[5, dgvNew.CurrentCell.RowIndex];

                        }


                        else if (dgvNew.CurrentCell.ColumnIndex == 6)
                        {
                            dgvNew.Focus();
                            dgvNew.CurrentCell = dgvNew[1, dgvNew.CurrentCell.RowIndex + 1];

                        }
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

        public void RemoveNullColumnFromDataTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][0])))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }
        public void save()
        {

            //Pnloading.Visible = true;
            DataTable dt = new DataTable();
            if (Txtcustomername.SelectedIndex != 0)
            {
                objQuotationbal.Customerid = Convert.ToString(Txtcustomername.SelectedValue);
            }
            else
            {
                objQuotationbal.Customerid = "";
            }
            objQuotationbal.Customername = Txtcustomername.Text;
            objQuotationbal.Estinationid = CmbEstimationid.Text;
            objQuotationbal.Quotationid = Lblhidden.Text;
            if (string.IsNullOrEmpty(Lblhidden.Text))
            {
                objQuotationbal.isnew = 0;
            }
            else
            {
                objQuotationbal.isnew = 1;
            }



            objQuotationbal.Updatedby = Program.userid;
            dt = DataGridView2DataTable(dgvNew);

            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }

            dt.Columns.RemoveAt(1);
            dt.Columns.RemoveAt(2);

            dt.Columns["Amount"].ColumnName = "Location";
            dt.AcceptChanges();

            RemoveNullColumnFromDataTable(dt);
            bool dtval = RemoveDuplicateRows(dt, "ProductId");

            
            if (dtval)
            {
                string output = objQuotationbal.SaveIssuedreceived(objQuotationbal, dt);
                if (!string.IsNullOrEmpty(output))
                {
                    GetReport(output);
                    clear();
                }
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
                dgvNew.Rows[index].DefaultCellStyle.ForeColor = Color.Red;
                sa = false;
            }
            //dTable.Rows.Remove(dRow);


            //Datatable which contains unique records will be return as output.
            return sa;
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

        private void button2_Click(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
            lblproductid.Text = string.Empty;
            // Txtitem.Text = string.Empty;
            lblitemcode.Text = "0";
            lblprice.Text = "0";
            lbldisplay.Text = "0";
            lbldemo.Text = "0";
            lblservice.Text = "0";
            lbldamage.Text = "0";
            pnsearch.Visible = false;
            dgvNew.Focus();
            dgvNew.CurrentCell = dgvNew[2, dgvNew.CurrentCell.RowIndex];
        }

        public void getEstimation(string s)
        {
            DataSet ds = objQuotationbal.GetIssuedreceived(s);
            if (ds.Tables[0].Rows.Count > 0)
            {

                Txtcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["CustomerName"]);
                Lblhidden.Text = Convert.ToString(ds.Tables[0].Rows[0]["ReceivedID"]);
                CmbEstimationid.Text = Convert.ToString(ds.Tables[0].Rows[0]["RefNo"]);

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
                    dgvNew.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvNew.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvNew.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);


                    string sval1 = objQuotationbal.getestimationqty(CmbEstimationid.Text, Convert.ToString(ds.Tables[1].Rows[i]["Productid"]));

                    dgvNew.Rows[i].Cells[4].Value = sval1;

                    dgvNew.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Receiveqty"]);

                    dgvNew.Rows[i].Cells["Location"].Value = Convert.ToString(ds.Tables[1].Rows[i]["LocationName"]);
                    dgvNew.Rows[i].Cells["Amount"].Value = Convert.ToString(ds.Tables[1].Rows[i]["Location"]);
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




        public void Getapproval(string s)
        {
            DataSet ds = objQuotationbal.GetIssuedreceived(s);
            if (ds.Tables[0].Rows.Count > 0)
            {

                Txtappcutomer.Text = Convert.ToString(ds.Tables[0].Rows[0]["CustomerName"]);
                txtreceivedid.Text = Convert.ToString(ds.Tables[0].Rows[0]["ReceivedID"]);
                Txtestimation.Text = Convert.ToString(ds.Tables[0].Rows[0]["RefNo"]);

            }
            else
            {

                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                dgvPayment.Rows.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvPayment.Rows.Add();
                    dgvPayment.Rows[i].Cells[0].Value = i + 1;
                    dgvPayment.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvPayment.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvPayment.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);


                    string sval1 = objQuotationbal.getestimationqty(CmbEstimationid.Text, Convert.ToString(ds.Tables[1].Rows[i]["Productid"]));

                    dgvPayment.Rows[i].Cells[4].Value = sval1;

                    dgvPayment.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Receiveqty"]);

                    dgvPayment.Rows[i].Cells["Location"].Value = Convert.ToString(ds.Tables[1].Rows[i]["LocationName"]);
                    dgvPayment.Rows[i].Cells["Amount"].Value = Convert.ToString(ds.Tables[1].Rows[i]["Location"]);
                }
                //panel9.Enabled = false;
                //btnsave.Enabled = false;
            }
            else
            {
                dgvPayment.Rows.Clear();
                //panel9.Enabled = true;
                //btnsave.Enabled = true;
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

                            Txtcustomername.Focus();
                        }
                        else
                        {
                            clear();
                        }

                    }
                }





                else if (selectedtab == "TabPayment")
                {

                    if (e.RowIndex >= 0)
                    {
                        string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                        if (!string.IsNullOrEmpty(s))
                        {
                            Getapproval(s);

                            btnApproved.Focus();
                        }
                        else
                        {
                            clear();
                        }

                    }


                }


            }
        }

        private void Btnsubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Txtitem.Text))
            {
                int rowindex = Convert.ToInt32(lblrowindex.Text);

                if (selectedtab == "TabNew")
                {
                    dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
                    dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    dgvNew.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                    dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                    //dgvNew.Rows[rowindex].Cells[4].Value = lblprice.Text;
                    dgvNew.Rows[rowindex].Cells[0].Value = rowindex + 1;


                    DataTable s = objQuotationbal.getlocbytransid(CmbEstimationid.Text, lblproductid.Text);
                    string sval = objQuotationbal.getestimationqty(CmbEstimationid.Text, lblproductid.Text);
                    dgvNew.Rows[rowindex].Cells[4].Value = sval;
                    if (s.Rows.Count > 0)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(s.Rows[0]["LocationName"])))
                        {
                            MessageBox.Show("In correct Product");
                            dgvNew.Rows[rowindex].Cells[3].Value = "";
                            dgvNew.Rows[rowindex].Cells[1].Value = "";
                            dgvNew.Rows[rowindex].Cells[2].Value = "";
                            dgvNew.Rows[rowindex].Cells[4].Value = "";
                            dgvNew.Rows[rowindex].Cells[0].Value = "";
                            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[2];

                        }
                        else
                        {
                            dgvNew.Rows[rowindex].Cells["Location"].Value = Convert.ToString(s.Rows[0]["LocationName"]);
                            dgvNew.Rows[rowindex].Cells["Amount"].Value = Convert.ToString(s.Rows[0]["LocationId"]);
                        }

                    }
                    else
                    {
                        MessageBox.Show("In correct Product");
                        dgvNew.Rows[rowindex].Cells[3].Value = "";
                        dgvNew.Rows[rowindex].Cells[1].Value = "";
                        dgvNew.Rows[rowindex].Cells[2].Value = "";
                        dgvNew.Rows[rowindex].Cells[4].Value = "";
                        dgvNew.Rows[rowindex].Cells[0].Value = "";
                        dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[2];
                    }
                    pnsearch.Visible = false;
                    DgvAutoRefNo.Visible = false;

                    lblproductid.Text = string.Empty;
                    // Txtitem.Text = string.Empty;
                    lblitemcode.Text = "0";
                    lblprice.Text = "0";
                    lbldisplay.Text = "0";
                    lbldemo.Text = "0";
                    lblservice.Text = "0";
                    lbldamage.Text = "0";

                    dgvNew.Focus();
                    dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];


                }
            }
            else
            {
                MessageBox.Show("Please Enter Product Name");
                Txtitem.Focus();
            }

        }

        public void bindLocation()
        {
            cmbloaction.DataSource = objQuotationbal.getLocation();
            cmbloaction.DisplayMember = "LocationName";
            cmbloaction.ValueMember = "LocationID";
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

        public void searchcheckout(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchCheckout(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }


        public void searchPaymentmode(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchPaymentmode(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }




        private void Txtitem_KeyDown_1(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.Enter)
            //{
            //    if (!string.IsNullOrEmpty(Txtitem.Text))
            //    {
            //        if (ProdNotFoundMSg)
            //        {
            //            //LblProdNotFoundMSg.Visible = true;
            //        }
            //        else
            //        {
            //            int rowindex = Convert.ToInt32(lblrowindex.Text);
            //            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
            //            dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
            //            dgvNew.Rows[rowindex].Cells[1].Value = cas.ToUpper();
            //            dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
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

            //            dgvNew.Focus();
            //            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
            //            dgvNew.BeginEdit(true);
            //            // LblProdNotFoundMSg.Visible = false;
            //        }
            //    }
            //    else
            //    {
            //        this.ActiveControl = btnsave;
            //        pnsearch.Visible = false;
            //        //MessageBox.Show("Please Enter Product Name");
            //        //Txtitem.Focus();
            //    }

            //}
        }

        public void searchcreditapproved(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchcreditapproved(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }


            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }





        public void searchpay(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchwindowpay(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }






        public void searchcheck(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchsalesreturncheck(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);


            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }












        public void searchdelivery(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchWindowdelivery(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

            //dgvSearch.Columns["Assist"].Visible = false;
            //dgvSearch.Columns["Referenceid"].Visible = false;
            //dgvSearch.Columns["customername"].Width = 175;
            //dgvSearch.Columns["date"].Width = 175;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

        private void dgvChecking_DataError(object sender, DataGridViewDataErrorEventArgs e)
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

                    for (int i = 0; i < MainTabSalesBill.TabPages[j].Controls.Count; )
                    {
                        MainTabSalesBill.TabPages[j].Controls[i].Dispose();
                    }
                    MainTabSalesBill.TabPages[j].Dispose();
                }

                MainTabSalesBill.Dispose();

                this.Dispose();
            }
            catch
            {

            }
        }


        //private void dgvPayment_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex >= 0)
        //    {
        //        string va = Convert.ToString(dgvPayment.Rows[e.RowIndex].Cells["Returnid"].Value);
        //        if (Convert.ToString(dgvPayment.Columns[e.ColumnIndex].HeaderText) == "Approval")
        //        {
        //            int v = objQuotationbal.salesreturnapproval("SalesApproved", va);
        //        }
        //        else if (Convert.ToString(dgvPayment.Columns[e.ColumnIndex].HeaderText) == "Reject")
        //        {
        //            int v = objQuotationbal.salesreturnapproval("Rejected", va);
        //        }
        //        Getapproval("");
        //    }
        //}



        private void dgvChecking_DataError_1(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
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
                    //            Txtitem.Text = Convert.ToString(DgvAutoRefNo[0, ProdSelRowvalue].Value);
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
            for (int i = 0; i < dgvNew.Rows.Count; i++)
            {
                dgvNew.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void DgvAutoRefNo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (!string.IsNullOrEmpty(lblproductid.Text))
                {
                if (Convert.ToInt32(lblproductid.Text) != 0)
                {
                    int rowindex = Convert.ToInt32(lblrowindex.Text);

                    if (selectedtab == "TabNew")
                    {
                        dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
                        string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                        getitems(sa);

                        //itemdetails(Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value));

                        dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        dgvNew.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();


                        //dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        //dgvNew.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                        dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                        double val = Convert.ToDouble(lblprice.Text);

                        DataTable s = objQuotationbal.getlocbytransid(CmbEstimationid.Text, lblproductid.Text);
                        string sval = objQuotationbal.getestimationqty(CmbEstimationid.Text, lblproductid.Text);
                        dgvNew.Rows[rowindex].Cells[4].Value = sval;

                        dgvNew.Rows[rowindex].Cells[0].Value = rowindex + 1;

                        if (s.Rows.Count > 0)
                        {
                            if (string.IsNullOrEmpty(Convert.ToString(s.Rows[0]["LocationName"])))
                            {
                                MessageBox.Show("In correct Product");
                                dgvNew.Rows[rowindex].Cells[3].Value = "";
                                dgvNew.Rows[rowindex].Cells[1].Value = "";
                                dgvNew.Rows[rowindex].Cells[2].Value = "";
                                dgvNew.Rows[rowindex].Cells[4].Value = "";
                                dgvNew.Rows[rowindex].Cells[0].Value = "";
                                dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[2];

                            }
                            else
                            {
                                dgvNew.Rows[rowindex].Cells["Location"].Value = Convert.ToString(s.Rows[0]["LocationName"]);
                                dgvNew.Rows[rowindex].Cells["Amount"].Value = Convert.ToString(s.Rows[0]["LocationId"]);
                            }

                        }
                        else
                        {
                            MessageBox.Show("In correct Product");
                            dgvNew.Rows[rowindex].Cells[3].Value = "";
                            dgvNew.Rows[rowindex].Cells[1].Value = "";
                            dgvNew.Rows[rowindex].Cells[2].Value = "";
                            dgvNew.Rows[rowindex].Cells[4].Value = "";
                            dgvNew.Rows[rowindex].Cells[0].Value = "";
                            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[2];
                        }
                        pnsearch.Visible = false;
                        DgvAutoRefNo.Visible = false;

                        lblproductid.Text = string.Empty;
                        // Txtitem.Text = string.Empty;
                        lblitemcode.Text = "0";
                        lblprice.Text = "0";
                        lbldisplay.Text = "0";
                        lbldemo.Text = "0";
                        lblservice.Text = "0";
                        lbldamage.Text = "0";

                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
                    }
                }
                }
            }
        }

        private void dgvNew_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double qty = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5].Value);
                double orgqty = Convert.ToDouble(dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[4].Value);

                if (orgqty < qty)
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
            catch
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
                        dgvNew.Columns[4].DefaultCellStyle.Format = "N2";
                        int rowindex = Convert.ToInt32(lblrowindex.Text);
                        dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
                        dgvNew.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        dgvNew.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                        dgvNew.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                        double val = Convert.ToDouble(lblprice.Text);
                        //dgvNew.Rows[rowindex].Cells[4].Value = val;
                        dgvNew.Rows[rowindex].Cells[0].Value = rowindex + 1;
                        DgvAutoRefNo.Visible = false;

                        DataTable s = objQuotationbal.getlocbytransid(CmbEstimationid.Text, lblproductid.Text);
                        string sval = objQuotationbal.getestimationqty(CmbEstimationid.Text, lblproductid.Text);
                        dgvNew.Rows[rowindex].Cells[4].Value = sval;
                        if (s.Rows.Count > 0)
                        {
                            if (string.IsNullOrEmpty(Convert.ToString(s.Rows[0]["LocationName"])))
                            {
                                MessageBox.Show("In correct Product");
                                dgvNew.Rows[rowindex].Cells[3].Value = "";
                                dgvNew.Rows[rowindex].Cells[1].Value = "";
                                dgvNew.Rows[rowindex].Cells[2].Value = "";
                                dgvNew.Rows[rowindex].Cells[4].Value = "";
                                dgvNew.Rows[rowindex].Cells[0].Value = "";
                                dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[2];

                            }
                            else
                            {
                                dgvNew.Rows[rowindex].Cells["Location"].Value = Convert.ToString(s.Rows[0]["LocationName"]);
                                dgvNew.Rows[rowindex].Cells["Amount"].Value = Convert.ToString(s.Rows[0]["LocationId"]);
                            }

                        }
                        else
                        {
                            MessageBox.Show("In correct Product");
                            dgvNew.Rows[rowindex].Cells[3].Value = "";
                            dgvNew.Rows[rowindex].Cells[1].Value = "";
                            dgvNew.Rows[rowindex].Cells[2].Value = "";
                            dgvNew.Rows[rowindex].Cells[4].Value = "";
                            dgvNew.Rows[rowindex].Cells[0].Value = "";
                            dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[2];
                        }
                        pnsearch.Visible = false;
                        lblproductid.Text = string.Empty;
                        //Txtitem.Text = string.Empty;
                        lblitemcode.Text = "0";
                        //lblrack.Text = "0";
                        lbldisplay.Text = "0";
                        lbldemo.Text = "0";
                        lblservice.Text = "0";
                        lbldamage.Text = "0";
                        lblprice.Text = "0";
                        dgvNew.Focus();
                        dgvNew.CurrentCell = dgvNew.Rows[dgvNew.CurrentCell.RowIndex].Cells[5];
                    }
                    else
                    {
                        MessageBox.Show("Please Enter Correct Product Name");
                    }
                }
                else
                {
                    //this.ActiveControl = btnSave;
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
                //lblitem.Text = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);

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

                //  lblrack.Text = "0";
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

        private void Txtcustomername_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvNew_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void Txtcustomername_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Txtcustomername.Text))
            {
                DataTable dt = objQuotationbal.getestimationnumber(Txtcustomername.Text);
                if (dt.Rows.Count > 0)
                {
                    CmbEstimationid.DataSource = dt;
                    CmbEstimationid.DisplayMember = "Estimationid";
                    CmbEstimationid.ValueMember = "Estimationid";
                }
                else
                {
                    CmbEstimationid.DataSource = null;
                }
            }
            else
            {
                CmbEstimationid.DataSource = null;
            }
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {      
            GetSearchSalesOrder();
        }

        private void GetSearchSalesOrder()
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
            DataTable dt = objQuotationbal.searchIssuedreceived(OrderNo, FromDate, ToDate, Vendorid, Convert.ToInt32(Iscombined));


            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Order No"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Customer"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Order Date"]);
                dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i]["RefNo"]);
            }

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

        private void dgvPayment_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void btnApproved_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtreceivedid.Text))
            {
                saveapproval("Approved");
            }
            else
            {
                MessageBox.Show("Please Select Item");
            }
           
        }


        public void saveapproval(string status)
        {
            DataTable dt = new DataTable();
            dt = DataGridView2DataTable(dgvPayment);

            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }

            dt.Columns.RemoveAt(1);
            dt.Columns.RemoveAt(2);

            dt.Columns["Amount"].ColumnName = "Location";
            dt.AcceptChanges();
            RemoveNullColumnFromDataTable(dt);
            objQuotationbal.Quotationid = txtreceivedid.Text;
            objQuotationbal.Estinationid = Txtestimation.Text;
            objQuotationbal.status = status;
            int s = objQuotationbal.saveapproval(objQuotationbal, dt);
            if (s == 1)
            {
                GetSearchSalesOrder();
                clear();
            }

        }

        private void btnRejected_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtreceivedid.Text))
            {
                saveapproval("Reject");
            }
            else
            {
                MessageBox.Show("Please Select Item");
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
                            cmd.CommandText = "GetReceived_Print";
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

        private void cmbcustomer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

