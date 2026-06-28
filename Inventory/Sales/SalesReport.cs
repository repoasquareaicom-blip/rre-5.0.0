
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
using SalesReportDAL;
using System.Configuration;
using RRLightsSaleBill;

namespace Inventory
{
    public partial class SalesReport : Form
    {
        static string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        QuotationBal objQuotationbal = new QuotationBal();
        TextBox tb;
        public bool edit = false;
        string userid = "";
        string test;
        string firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue;
        public SalesReport()
        {
            InitializeComponent();
            userid = Convert.ToString(Program.userid);
            this.WindowState = FormWindowState.Maximized;
            LoadPorts();
            textBox1.Visible = false;
            lblHSNCode.Visible = true;
            ItemName.Visible = false;
            bindLocation();
            cmbloaction.SelectedIndex = 0;
            cmbpaymode.SelectedIndex = 0;
            cmbstatus.SelectedIndex = 0;
            SearchPurchaseOrder();
            lblperare.Text = Program.UserName;
            lblShop.Text = Program.ShopName;
            bindAssist();
            bindreference();
            label29.Visible = false;
            bindcustomer();
            Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            Txtitem.AutoCompleteCustomSource = AutoCompleteLoad();
            Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;


            textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox1.AutoCompleteCustomSource = AutoCompleteLoads();
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;

            btnSearch.PerformClick();
            Globeimage();
            int shop = Convert.ToInt32(lblShop.Text);
            getsaleamount(shop);
            Bindcompany();
            cmbcompanychange.SelectedIndex = Convert.ToInt32(lblShop.Text);
            BindDropDown();
            comboBox1.SelectedValue = 31;
          
            BindDropDown1();
            comboBox2.SelectedValue = 0;

        }


        public void recal()
        {
            lblShop.Text = Program.ShopName;
            cmbcompanychange.SelectedIndex = Convert.ToInt32(lblShop.Text);
            int shop = Convert.ToInt32(lblShop.Text);
            userid = Convert.ToString(Program.userid);
            this.WindowState = FormWindowState.Maximized;
            LoadPorts();

            bindLocation();
            cmbloaction.SelectedIndex = 0;
            cmbpaymode.SelectedIndex = 0;
            cmbstatus.SelectedIndex = 0;
            SearchPurchaseOrder();
            lblperare.Text = Program.UserName;

            bindAssist();
            bindreference();
            bindcustomer();
            Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            Txtitem.AutoCompleteCustomSource = AutoCompleteLoad();
            Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;


            textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox1.AutoCompleteCustomSource = AutoCompleteLoads();
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;

            btnSearch.PerformClick();
            Globeimage();

            getsaleamount(shop);

            cmbcompanychange.SelectedIndex = Convert.ToInt32(lblShop.Text);

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Yellow, 0, 0, 100, 100);
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
        public void Bindcompany()
        {
            try
            {
                DataTable dt = LoginBAL.Getcompanyname();
                DataRow dr = dt.NewRow();
                dr["CompanyName"] = "0";
                dr["CompanyName"] = "--Select--";
                dt.Rows.InsertAt(dr, 0);
                string com = dt.Rows[1][1].ToString();
                if (com == "R.R.LIGHTS")
                {
                    foreach (DataRow dr1 in dt.Rows)
                    {
                        if (dr1["CompanyName"].ToString() == "R.R. PIPES")
                            dr1.Delete();
                    }
                }
                else
                {

                }
                cmbcompanychange.DataSource = dt;

                cmbcompanychange.ValueMember = "CompanyName";
                cmbcompanychange.DisplayMember = "CompanyName";
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message.ToString());
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
            dgvOrder.ColumnCount = 10;
            //dgvOrder.RowCount = 16;

            dgvOrder.Columns[0].Name = "S.NO";
            dgvOrder.Columns[1].Name = "Items";
            dgvOrder.Columns[3].Name = "UOM";
            dgvOrder.Columns[2].Name = "HSN";
            dgvOrder.Columns[7].Name = "Quantity";
            dgvOrder.Columns[4].Name = "productid";
            dgvOrder.Columns[5].Name = "Rate";
            dgvOrder.Columns[6].Name = "GST";
            dgvOrder.Columns[8].Name = "Amount";
            dgvOrder.Columns[9].Name = "orderQty";

            dgvOrder.Columns[4].Visible = false;

            this.dgvOrder.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[0].Width = 25;
            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;

            // dgvOrder.Columns[5].HeaderText = "GST";
            this.dgvOrder.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[1].Width = 150;
            this.dgvOrder.Columns[2].Width = 30;
            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns[1].ReadOnly = true;
            this.dgvOrder.Columns[2].ReadOnly = true;
            this.dgvOrder.Columns[5].ReadOnly = false;
            this.dgvOrder.Columns[6].ReadOnly = true;
            this.dgvOrder.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrder.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrder.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvOrder.Columns[5].DefaultCellStyle.Format = "F2";
            dgvOrder.Columns[7].DefaultCellStyle.Format = "F2";

            this.dgvOrder.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[5].Width = 30;
            //this.dgvOrder.Columns[4].ReadOnly = true;

            this.dgvOrder.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[2].Width = 35;

            this.dgvOrder.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[3].Width = 30;
            this.dgvOrder.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[6].Width = 30;

            this.dgvOrder.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[7].Width = 35;



            this.dgvOrder.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[8].Width = 90;

            this.dgvOrder.Columns[8].ReadOnly = true;

            this.dgvOrder.Columns[9].Visible = false;



            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvOrder.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvOrder.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
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

        private void SalesReport_Load(object sender, EventArgs e)
        {
            int shop = Convert.ToInt32(lblShop.Text);
            this.ActiveControl = cmbcustomername;


            btnSearch.PerformClick();
            getsaleamount(shop);
        }


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
                if (keyData == (Keys.Alt | Keys.I))
                {
                    radioButton1.Checked = true;
                    return true;
                }
                if (keyData == (Keys.Alt | Keys.H))
                {
                    radioButton2.Checked = true;
                    return true;
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


            if (txtmobile.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = txtdate;
                    return true;


                    // return true;
                }
            }
            if (radioButton1.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = radioButton2;
                    return true;


                    // return true;
                }
            }

            if (radioButton2.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = Txtitem;
                    return true;


                    // return true;
                }
            }

            if (btnSave.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnSavePending;
                    return true;


                    // return true;
                }
            }

            if (btnSavePending.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnClear;
                    return true;


                    // return true;
                }
            }
            if (btnClear.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnNew;
                    return true;


                    // return true;
                }
            }
            if (btnNew.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = dtfromdate;
                    return true;


                    // return true;
                }
            }

            if (DTPTodate.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnSearch;
                    return true;


                    // return true;
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
            if (btnSearch.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    cmbcustomername.Focus();
                    return true;
                }
            }
            if (txtmobile.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    txtdate.Focus();
                    return true;
                }
            }

            if (transactionclose.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    cmbpaymode.Focus();
                    return true;
                }
            }
            if (cmbpaymode.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    btnSave.Focus();
                    return true;
                }
            }
            if (btnSave.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    btnPrint.Focus();
                    return true;
                }
            }
            if (btnPrint.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    button1.Focus();
                    return true;
                }
            }
            if (button1.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    dtfromdate.Focus();
                    return true;
                }
            }

            try
            {
                if (keyData == Keys.Tab)
                {

                    if (radioButton1.Checked)
                    {
                        if (dgvOrder.CurrentCell.ColumnIndex == 8)
                        {
                            dgvOrder.Focus();
                            //edit = true;
                            dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                            Txtitem.Focus();
                            //textBox1.Focus();
                        }
                    }
                    else if (radioButton2.Checked)
                    {
                        if (dgvOrder.CurrentCell.ColumnIndex == 8)
                        {
                            dgvOrder.Focus();
                            //edit = true;
                            dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                            //Txtitem.Focus();
                            textBox1.Focus();
                        }
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (txtdate.Focused)
                {
                    if (keyData == (Keys.Tab))
                    {
                        if (radioButton1.Checked)
                        {
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex + 1, dgvOrder.CurrentCell.RowIndex];
                            this.ActiveControl = dgvOrder;
                            //dgvOrder.BeginEdit(true);
                            Txtitem.Focus();
                            //textBox1.Focus();
                            pnsearch.Visible = true;

                            return true;
                        }

                        else if (radioButton2.Checked)
                        {
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex + 1, dgvOrder.CurrentCell.RowIndex];
                            this.ActiveControl = dgvOrder;
                            //dgvOrder.BeginEdit(true);
                            //Txtitem.Focus();
                            textBox1.Focus();
                            pnsearch.Visible = true;

                            return true;
                        }

                        //}
                    }
                }
            }
            catch (Exception ex)
            {

            }
            if (cmbloaction.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = Txtitem;
                    return true;
                }

            }




            return base.ProcessCmdKey(ref msg, keyData);
        }


        public void getsino()
        {
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                dgvOrder.Rows[i].Cells[0].Value = i + 1;
            }
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

        public AutoCompleteStringCollection AutoCompleteLoad()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.getproductautocomplteonly();
            string[] arr = new string[st.Rows.Count];
            for (int i = 0; i < st.Rows.Count; i++)
            {
                arr[i] = st.Rows[i]["DisplayName"].ToString();
            }

            for (int i = 0; i < arr.Length; i++)
            {
                //var combined = string.Join(", ", arr);
                var combined = arr[i];
                str.Add(combined);
            }

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}

            return str;
        }


        public AutoCompleteStringCollection AutoCompleteLoads()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.getHSNautocomplteonly();
            string[] arr = new string[st.Rows.Count];
            for (int i = 0; i < st.Rows.Count; i++)
            {
                arr[i] = st.Rows[i]["HSN"].ToString();
            }

            for (int i = 0; i < arr.Length; i++)
            {
                //var combined = string.Join(", ", arr);
                var combined = arr[i];
                str.Add(combined);
            }

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}

            return str;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool val = Validation();



            if (val)
            {
                save(2);
                if (test == "1")
                {
                    panel1.Enabled = true;
                    panel2.Enabled = true;

                    Pnloading.Visible = false;
                }
                else
                {
                    clear();

                }

            }
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


            bool sas = false;

            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                string Items = Convert.ToString(dgvOrder.Rows[k].Cells["Quantity"].Value);

                //string Received = Convert.ToString(dgvOrder.Rows[k].Cells["Items"].Value);
                //string rate = Convert.ToString(dgvOrder.Rows[k].Cells["Rate"].Value);






                if ((!string.IsNullOrEmpty(Items) && Items == "." || Items == "-" || Items == ".-" || Items == "-." || Items == "0"))
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
                message = message + "*Quantity should not be empty." + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }


            //if (string.IsNullOrEmpty(cmdcity.Text))
            //{
            //    i++;
            //    message = message + "* Please Select city" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmdcity;
            //}

            //if (cmbassistby.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please Select Assist " + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmbassistby;
            //}



            //if (cmbreference.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please select Reference" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmbreference;
            //}

            if (cmbpaymode.SelectedIndex <= 0)
            {
                i++;
                message = message + "* Please select Paymentmode" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbpaymode;
            }


            if (dgvOrder.Rows.Count > 0)
            {
                i++;
                string Items = Convert.ToString(dgvOrder.Rows[0].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvOrder.Rows[0].Cells["Items"].Value);
                if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                {
                    message = message + "* Please Enter Product" + "\n";
                }

                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }
            else if (dgvOrder.Rows.Count == 0)
            {
                i++;
                message = message + "* Please Enter Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }

            bool sass = false;

            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                string Items = Convert.ToString(dgvOrder.Rows[k].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvOrder.Rows[k].Cells["Items"].Value);

                if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))) || Items == "." || Items == "-")
                {
                    sass = true;
                    break;
                }
            }
            if (sass == true)
            {
                i++;
                message = message + "* Product or quantity should not be empty." + "\n";
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
            pnsearch.Visible = false;
            btnSavePending.Enabled = true;
            btnSave.Enabled = true;
            btnPrint.Enabled = true;
            cmbcustomername.Text = "--Select--";
            cmdcity.Text = string.Empty;
            cmbassistby.SelectedIndex = 0;
            cmbreference.SelectedIndex = 0;
            txtorder.Clear();
            dgvOrder.Rows.Clear();
            lblperare.Text = Program.UserName;
            lbltotalquantity.Text = "0";
            lbltotalamount.Text = "0.00";
            cmbcustomername.Focus();
            cmbloaction.SelectedIndex = 0;
            panel2.Enabled = true;
            cmbpaymode.SelectedIndex = 0;
            lblgrandtotal.Text = "0.00";
            txtless.Text = "0.00";
            txtAddressLine1.Clear();
            txtAddressLine2.Clear();
            txttin.Clear();
            txtmobile.Clear();
            txtdate.Value = DateTime.Now.Date;
            Txtothers.Text = "0";

        }


        private void btnNew_Click(object sender, EventArgs e)
        {
            clear();
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
            panel2.Enabled = false;

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

            objQuotationbal.Customerid = Convert.ToString(cmbcustomername.SelectedValue);
            objQuotationbal.Customername = Convert.ToString(cmbcustomername.Text);
            objQuotationbal.City = Convert.ToString(cmdcity.Text);
            objQuotationbal.Referenceid = Convert.ToString(cmbreference.SelectedValue);
            objQuotationbal.Assist = Convert.ToString(cmbassistby.SelectedValue);
            objQuotationbal.Paymentmode = Convert.ToString(cmbpaymode.Text);
            objQuotationbal.Total = Convert.ToString(lbltotalamount.Text);


            if (string.IsNullOrEmpty(txtless.Text))
            {
                objQuotationbal.Lessamount = "0";
            }
            else
            {
                objQuotationbal.Lessamount = Convert.ToString(txtless.Text);
            }
            if (string.IsNullOrEmpty(Txtothers.Text))
            {
                objQuotationbal.others = "0";

            }
            else
            {
                objQuotationbal.others = Convert.ToString(Txtothers.Text);
            }

            //objQuotationbal.Lessamount = Convert.ToString(txtless.Text);
            objQuotationbal.Grandtotal = Convert.ToString(Convert.ToDouble(lbltotalamount.Text) + Convert.ToDouble(Txtothers.Text) - Convert.ToDouble(objQuotationbal.Lessamount));
            objQuotationbal.Updatedby = Program.userid;
            objQuotationbal.Address1 = txtAddressLine1.Text;
            objQuotationbal.Address2 = txtAddressLine2.Text;
            objQuotationbal.Tin = txttin.Text;
            objQuotationbal.Mobile = txtmobile.Text;




            DateTime date = new DateTime(txtdate.Value.Year, txtdate.Value.Month, txtdate.Value.Day);
            objQuotationbal.date = date;
            dt = DataGridView2DataTable(dgvOrder);
            for (int i = 0; i < 4; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            dt.Columns.RemoveAt(2);
            dt.Columns.RemoveAt(4);
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

            // if (test == "1")
            // {
            //     MessageBox.Show("Quantity Should not be Zero");
            // }
            // else
            // {
            int shop = Convert.ToInt32(lblShop.Text);

            string value;


            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlCommand cmd = null;

                if (shop == 1)
                {

                    cmd = new SqlCommand("SaveQuotationsales_GST", con);
                }

                if (shop == 2)
                {
                    cmd = new SqlCommand("SaveQuotationsalesPipes_Direct_GST", con);

                }


                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Customerid", Convert.ToString(cmbcustomername.SelectedValue));
                cmd.Parameters.AddWithValue("@Customername", Convert.ToString(cmbcustomername.Text));
                cmd.Parameters.AddWithValue("@City", Convert.ToString(cmdcity.Text));
                cmd.Parameters.AddWithValue("@Referenceid", Convert.ToString(cmbreference.SelectedValue));
                cmd.Parameters.AddWithValue("@Assist", Convert.ToString(cmbassistby.SelectedValue));
                cmd.Parameters.AddWithValue("@Updatedby", Program.userid);
                cmd.Parameters.AddWithValue("@paymentmode", Convert.ToString(cmbpaymode.Text));
                cmd.Parameters.AddWithValue("@TotalAmount", Convert.ToString(lbltotalamount.Text));
                if (string.IsNullOrEmpty(txtless.Text))
                {
                    cmd.Parameters.AddWithValue("@LessAmount", "0");

                }
                else
                {
                    cmd.Parameters.AddWithValue("@LessAmount", Convert.ToString(txtless.Text));
                    //objQuotationbal.Lessamount = Convert.ToString(txtless.Text);
                }

                cmd.Parameters.AddWithValue("@GrandTotal", Convert.ToString(Convert.ToDouble(lbltotalamount.Text) + Convert.ToDouble(Txtothers.Text) - Convert.ToDouble(objQuotationbal.Lessamount)));
                cmd.Parameters.AddWithValue("@address1", txtAddressLine1.Text);
                if (string.IsNullOrEmpty(Txtothers.Text))
                {

                    cmd.Parameters.AddWithValue("@others", "0");

                }
                else
                {
                    cmd.Parameters.AddWithValue("@others", Convert.ToString(Txtothers.Text));
                    //objQuotationbal.others = ;
                }


                cmd.Parameters.AddWithValue("@date", date);
                if (string.IsNullOrEmpty(txtAddressLine2.Text))
                {
                    cmd.Parameters.AddWithValue("@address2", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@address2", txtAddressLine2.Text);
                }
                cmd.Parameters.AddWithValue("@tin", txttin.Text);
                cmd.Parameters.AddWithValue("@mobile", txtmobile.Text);



                cmd.Parameters.AddWithValue("@QuotationDetails", dt);
                cmd.Parameters.Add("@result", SqlDbType.VarChar, 100);
                cmd.Parameters["@result"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                value = Convert.ToString(cmd.Parameters["@result"].Value);


                con.Close();
            }
            string output = value;
            //string output = objQuotationbal.SaveQuotationsales1(objQuotationbal, dt,shop);
            if (!string.IsNullOrEmpty(output))
            {
                panel1.Enabled = true;
                panel2.Enabled = true;
                Pnloading.Visible = false;
                getsaleamount(shop);
                //MessageBox.Show("save successfully");
                //    txtorder.Text = output;


                string configvalue2 = ConfigurationManager.AppSettings["PrePrinted"];

                if (configvalue2 == "Yes")
                {
                    GetrrlightsReport(output);
                }
                else
                {

                    GetReport(output, shop);
                    clear();
                }

                // GetReport(output);

                //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //if (result == DialogResult.Yes)
                //{

                //    Salesbillreport rpt = new Salesbillreport(output);
                //    rpt.ShowDialog();
                //}



                //else if (!string.IsNullOrEmpty(output) && !string.IsNullOrEmpty(txtorder.Text))
                //{
                //    //MessageBox.Show("Update successfully");
                //    panel1.Enabled = true;
                //    panel2.Enabled = true;
                //    Pnloading.Visible = false;

                //    txtorder.Text = output;
                //    if (v == 2)
                //    {
                //        DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                //        if (result == DialogResult.Yes)
                //        {
                //            Quotationreport rpt = new Quotationreport(output);
                //            rpt.ShowDialog();
                //        }
                //    }
                // }
                btnSearch.PerformClick();
            }
        }



        public void GetReport(string QuotationId, int shop)
        {
            try
            {
                if (!string.IsNullOrEmpty(QuotationId))
                {
                    DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {

                        //if (QuotationId.IndexOf("GST") >= 0)
                        //{
                        if (SGSTval.Checked)
                        {
                            Program.State = comboBox2.Text;
                        }
                        else if (IGSTval.Checked)
                        {
                            Program.State = comboBox1.Text;
                        }
                        Program.PrintInvoiceNumber = QuotationId;
                       
                       
                        frmInvoice frmInvoice = new frmInvoice();
                        frmInvoice.Show();
                        //}
                        //else
                        //{
                        //    using (SqlConnection con = new SqlConnection(Program.connection))
                        //    {
                        //        DataSet ds = new DataSet();
                        //        con.Open();
                        //        SqlCommand cmd = new SqlCommand();
                        //        cmd.Parameters.AddWithValue("@id", QuotationId);
                        //        cmd.Parameters.AddWithValue("@company", Program.Company);
                        //        cmd.CommandType = CommandType.StoredProcedure;
                        //        if (shop == 1)
                        //        {

                        //            cmd.CommandText = "GetQuotationsalesreport_print";
                        //        }

                        //        if (shop == 2)
                        //        {
                        //            cmd.CommandText = "GetQuotationsalesreport_printPipes_Direct";

                        //        }
                        //        //cmd.CommandText = "GetQuotationsalesreport_print";
                        //        cmd.Connection = con;
                        //        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        //        ad.Fill(ds);
                        //        SalesReports Obj = new SalesReports();
                        //        Obj.dsMain = ds;
                        //        Obj.pagenumber = 1;
                        //        Obj.status = true;
                        //        if (Obj.GenerateQuoation())
                        //        {


                        //        }
                        //}


                        //}
                    }
                }


            }
            catch (Exception e)
            {

            }
        }

        public void GetrrlightsReport(string QuotationId)
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
                            cmd.Parameters.AddWithValue("@company", Program.Company);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "GetRRLightsQuotationsalesreport_print";
                            cmd.Connection = con;
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(ds);

                            DalRRlightsSalesBill Obj = new DalRRlightsSalesBill();
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

        private void cmbcustomername_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = "";
            if (cmbcustomername.SelectedIndex > 0)
            {
                s = Convert.ToString(cmbcustomername.SelectedValue);
            }

            DataTable dt = objQuotationbal.getcityvalbyCustomerid(s);
            if (dt.Rows.Count > 0)
            {
                txtAddressLine1.Text = Convert.ToString(dt.Rows[0]["Address1"]);
                txtAddressLine2.Text = Convert.ToString(dt.Rows[0]["Address2"]);
                txtmobile.Text = Convert.ToString(dt.Rows[0]["Phone"]);
                cmdcity.Text = Convert.ToString(dt.Rows[0]["CITY"]);
                txttin.Text = Convert.ToString(dt.Rows[0]["Tin"]);
            }
            else
            {
                txtAddressLine1.Clear();
                txtAddressLine2.Clear();
                txtmobile.Clear();
                cmdcity.Clear();
                txttin.Clear();


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


        private void BindDropDown()
        {
            DataTable dtblDataSource = new DataTable();
            dtblDataSource.Columns.Add("Text");
            dtblDataSource.Columns.Add("Value");

            dtblDataSource.Rows.Add("--Select--", 0);
            dtblDataSource.Rows.Add("Andaman and Nicobar Islands", 1);
            dtblDataSource.Rows.Add("Andhra Pradesh", 2);
            dtblDataSource.Rows.Add("Arunachal Pradesh", 3);
            dtblDataSource.Rows.Add("Assam", 4);
            dtblDataSource.Rows.Add("Bihar", 5);
            dtblDataSource.Rows.Add("Chandigarh", 6);
            dtblDataSource.Rows.Add("Chhattisgarh", 7);
            dtblDataSource.Rows.Add("Dadar and Nagar Haveli", 8);
            dtblDataSource.Rows.Add("Daman and Diu", 9);
            dtblDataSource.Rows.Add("Delhi", 10);
            dtblDataSource.Rows.Add("Goa", 11);
            dtblDataSource.Rows.Add("Gujarat", 12);
            dtblDataSource.Rows.Add("Haryana", 13);
            dtblDataSource.Rows.Add("Himachal Pradesh", 14);
            dtblDataSource.Rows.Add("Jammu and Kashmir", 15);
            dtblDataSource.Rows.Add("Jharkhand", 16);
            dtblDataSource.Rows.Add("Karnataka", 17);
            dtblDataSource.Rows.Add("Kerala", 18);
            dtblDataSource.Rows.Add("Lakshadweep", 19);
            dtblDataSource.Rows.Add("Madhya Pradesh", 20);
            dtblDataSource.Rows.Add("Maharashtra", 21);
            dtblDataSource.Rows.Add("Manipur", 22);
            dtblDataSource.Rows.Add("Meghalaya", 23);
            dtblDataSource.Rows.Add("Mizoram", 24);
            dtblDataSource.Rows.Add("Nagaland", 25);
            dtblDataSource.Rows.Add("Odisha", 26);
            dtblDataSource.Rows.Add("Puducherry", 27);
            dtblDataSource.Rows.Add("Punjab", 28);
            dtblDataSource.Rows.Add("Rajasthan", 29);
            dtblDataSource.Rows.Add("Sikkim", 30);
            dtblDataSource.Rows.Add("Tamil Nadu", 31);
            dtblDataSource.Rows.Add("Telangana", 32);
            dtblDataSource.Rows.Add("Tripura", 33);
            dtblDataSource.Rows.Add("Uttar Pradesh", 34);
            dtblDataSource.Rows.Add("Uttarakhand", 35);
            dtblDataSource.Rows.Add("West Bengal", 36);

            comboBox1.Items.Clear();
            comboBox1.DataSource = dtblDataSource;
            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";
        }
        private void BindDropDown1()
        {
            DataTable dtblDataSource = new DataTable();
            dtblDataSource.Columns.Add("Text");
            dtblDataSource.Columns.Add("Value");

            dtblDataSource.Rows.Add("--Select--", 0);
            dtblDataSource.Rows.Add("Andaman and Nicobar Islands", 1);
            dtblDataSource.Rows.Add("Andhra Pradesh", 2);
            dtblDataSource.Rows.Add("Arunachal Pradesh", 3);
            dtblDataSource.Rows.Add("Assam", 4);
            dtblDataSource.Rows.Add("Bihar", 5);
            dtblDataSource.Rows.Add("Chandigarh", 6);
            dtblDataSource.Rows.Add("Chhattisgarh", 7);
            dtblDataSource.Rows.Add("Dadar and Nagar Haveli", 8);
            dtblDataSource.Rows.Add("Daman and Diu", 9);
            dtblDataSource.Rows.Add("Delhi", 10);
            dtblDataSource.Rows.Add("Goa", 11);
            dtblDataSource.Rows.Add("Gujarat", 12);
            dtblDataSource.Rows.Add("Haryana", 13);
            dtblDataSource.Rows.Add("Himachal Pradesh", 14);
            dtblDataSource.Rows.Add("Jammu and Kashmir", 15);
            dtblDataSource.Rows.Add("Jharkhand", 16);
            dtblDataSource.Rows.Add("Karnataka", 17);
            dtblDataSource.Rows.Add("Kerala", 18);
            dtblDataSource.Rows.Add("Lakshadweep", 19);
            dtblDataSource.Rows.Add("Madhya Pradesh", 20);
            dtblDataSource.Rows.Add("Maharashtra", 21);
            dtblDataSource.Rows.Add("Manipur", 22);
            dtblDataSource.Rows.Add("Meghalaya", 23);
            dtblDataSource.Rows.Add("Mizoram", 24);
            dtblDataSource.Rows.Add("Nagaland", 25);
            dtblDataSource.Rows.Add("Odisha", 26);
            dtblDataSource.Rows.Add("Puducherry", 27);
            dtblDataSource.Rows.Add("Punjab", 28);
            dtblDataSource.Rows.Add("Rajasthan", 29);
            dtblDataSource.Rows.Add("Sikkim", 30);

            dtblDataSource.Rows.Add("Telangana", 31);
            dtblDataSource.Rows.Add("Tripura", 32);
            dtblDataSource.Rows.Add("Uttar Pradesh", 33);
            dtblDataSource.Rows.Add("Uttarakhand", 34);
            dtblDataSource.Rows.Add("West Bengal", 35);

            comboBox2.Items.Clear();
            comboBox2.DataSource = dtblDataSource;
            comboBox2.DisplayMember = "Text";
            comboBox2.ValueMember = "Value";
        }

        private void Btnsubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Txtitem.Text))
            {
                int rowindex = Convert.ToInt32(lblrowindex.Text);
                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6];
                dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                dgvOrder.Rows[rowindex].Cells[1].Value = Txtitem.Text.ToUpper();
                dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                dgvOrder.Rows[rowindex].Cells[5].Value = "1";
                dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                pnsearch.Visible = false;
                lblproductid.Text = string.Empty;
                Txtitem.Text = string.Empty;
                lblitemcode.Text = "0";

                lbldisplay.Text = "0";
                lbldemo.Text = "0";
                lblservice.Text = "0";
                lbldamage.Text = "0";
                lblprices.Text = "0";
                lblVat.Text = "0";
                dgvOrder.Focus();
                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6];
            }
            else
            {
                MessageBox.Show("Please Enter Product Name");
                Txtitem.Focus();
            }


        }


        public void itemdetails()
        {

            try
            {
                lblproductid.Text = "";
                string s1 = Txtitem.Text.Trim();
                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s1.Replace("'", "''");


                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                //DataTable st = objQuotationbal.itemdetails1(name, s2);
                DataTable st = objQuotationbal.itemdetailssales(name, s2);

                if (st.Rows.Count > 0)
                {
                    lblitem.Text = name;
                    //pnitemdetails.Visible = true;
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
                    lblHSNCode.Text = Convert.ToString(st.Rows[0]["HSN"]);
                    if (lblHSNCode.Text == "")
                    {
                        lblHSNCode.Text = "0";
                    }
                    lblprices.Text = Convert.ToString(st.Rows[0]["Price"]);
                    if (lblprices.Text == "")
                    {
                        lblprices.Text = "0";
                    }
                    lblprice.Text = Convert.ToString(st.Rows[0]["Price"]);
                    if (lblprice.Text == "")
                    {
                        lblprice.Text = "0";
                    }
                    lblVat.Text = Convert.ToString(st.Rows[0]["GST"]);
                    if (lblVat.Text == "")
                    {
                        lblVat.Text = "0";
                    }

                    lbelhsncode.Text = Convert.ToString(st.Rows[0]["HSN"]);
                    if (lbelhsncode.Text == "")
                    {
                        lbelhsncode.Text = "0";
                    }
                    //DefaultFloor.Text = Convert.ToString(st.Rows[0]["DefaultFloor"]);
                    //if (DefaultFloor.Text == "")
                    //{
                    //    DefaultFloor.Text = "0";
                    //}

                    //Checking.Text = Convert.ToString(st.Rows[0]["Checking"]);
                    //if (Checking.Text == "")
                    //{
                    //    Checking.Text = "0";
                    //}


                    //Display.Text = Convert.ToString(st.Rows[0]["Display"]);

                    //if (Display.Text == "")
                    //{
                    //    Display.Text = "0";
                    //}


                    //Damage.Text = Convert.ToString(st.Rows[0]["Damage"]);
                    //if (Damage.Text == "")
                    //{
                    //    Damage.Text = "0";
                    //}

                    //Delivery.Text = Convert.ToString(st.Rows[0]["Delivery"]);
                    //if (Delivery.Text == "")
                    //{
                    //    Delivery.Text = "0";
                    //}





                    //pictureBox1.ImageLocation = Path.GetFullPath("131353W24J150-43D6.jpg");
                    //pictureBox1.ImageLocation = itemdetails.ToList()[0].imagepath;




                }
                else
                {
                    //pnitemdetails.Visible = false;
                    //lblitemcode.Text = "0";
                    //lblproductid.Text = "0";
                    lblprice.Text = "0";
                    lblprices.Text = "0";
                    lblHSNCode.Text = "0";
                    //lbldisplay.Text = "0";


                }

            }
            catch (Exception e)
            {

            }

        }




        public void itemdetailsHSN()
        {

            try
            {


                string s1 = textBox1.Text.Trim();
                string[] s = s1.Split('-');
                HSNPrice.Text = "";

                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s[0].ToString();

                string name1 = s[1].ToString();
                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                //DataTable st = objQuotationbal.itemdetails1(name, s2);
                DataTable st = objQuotationbal.itemdetailssalesHSN(name, s2, name1);

                if (st.Rows.Count > 0)
                {
                    lblitem.Text = name;
                    //pnitemdetails.Visible = true;
                    lblitemcode.Text = Convert.ToString(st.Rows[0]["UOM"]);
                    if (lblitemcode.Text == "")
                    {
                        lblitemcode.Text = "0";
                    }


                    HSNPrice.Text = Convert.ToString(st.Rows[0]["ProductId"]);
                    if (HSNPrice.Text == "")
                    {
                        HSNPrice.Text = "0";
                    }
                    //lblHSNCode.Text = Convert.ToString(st.Rows[0]["HSN"]);
                    //if (lblHSNCode.Text == "")
                    //{
                    //    lblHSNCode.Text = "0";
                    //}
                    lblprice.Text = Convert.ToString(st.Rows[0]["Price"]);
                    if (lblprice.Text == "")
                    {
                        lblprice.Text = "0";
                    }


                    lblprices.Text = Convert.ToString(st.Rows[0]["Price"]);
                    if (lblprices.Text == "")
                    {
                        lblprices.Text = "0";
                    }

                    lblVat.Text = Convert.ToString(st.Rows[0]["GST"]);
                    if (lblVat.Text == "")
                    {
                        lblVat.Text = "0";
                    }

                    lbelhsncode.Text = Convert.ToString(st.Rows[0]["HSN1"]);
                    if (lbelhsncode.Text == "")
                    {
                        lbelhsncode.Text = "0";
                    }


                    ItemName.Text = Convert.ToString(st.Rows[0]["ItemName"]);
                    if (ItemName.Text == "")
                    {
                        ItemName.Text = "";
                    }
                    //DefaultFloor.Text = Convert.ToString(st.Rows[0]["DefaultFloor"]);
                    //if (DefaultFloor.Text == "")
                    //{
                    //    DefaultFloor.Text = "0";
                    //}

                    //Checking.Text = Convert.ToString(st.Rows[0]["Checking"]);
                    //if (Checking.Text == "")
                    //{
                    //    Checking.Text = "0";
                    //}


                    //Display.Text = Convert.ToString(st.Rows[0]["Display"]);

                    //if (Display.Text == "")
                    //{
                    //    Display.Text = "0";
                    //}


                    //Damage.Text = Convert.ToString(st.Rows[0]["Damage"]);
                    //if (Damage.Text == "")
                    //{
                    //    Damage.Text = "0";
                    //}

                    //Delivery.Text = Convert.ToString(st.Rows[0]["Delivery"]);
                    //if (Delivery.Text == "")
                    //{
                    //    Delivery.Text = "0";
                    //}





                    //pictureBox1.ImageLocation = Path.GetFullPath("131353W24J150-43D6.jpg");
                    //pictureBox1.ImageLocation = itemdetails.ToList()[0].imagepath;




                }
                else
                {
                    //pnitemdetails.Visible = false;
                    //lblitemcode.Text = "0";
                    //lblproductid.Text = "0";
                    //lblprice.Text = "0";
                    lblprices.Text = "0";
                    //lbldisplay.Text = "0";


                }

            }
            catch (Exception e)
            {
                ItemName.Text = "";
                lblprices.Text = "0";
            }

        }

        private void Txtitem_TextChanged(object sender, EventArgs e)
        {

            if (radioButton1.Checked)
            {
                textBox1.Visible = false;
                Txtitem.Visible = true;
                label29.Visible = false;
                label34.Visible = true;
                textBox1.Text = "";
                // Txtitem.Text = "";
                itemdetails();
            }
            else if (radioButton2.Checked)
            {
                textBox1.Visible = true;
                //textBox1.Text = "";
                label29.Visible = true;
                Txtitem.Text = "";
                label34.Visible = false;
                Txtitem.Visible = false;
                itemdetailsHSN();
            }

        }

        private void transactionclose_Click(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
            lblproductid.Text = string.Empty;
            Txtitem.Text = string.Empty;
            lblitemcode.Text = "0";
            HSNPrice.Text = string.Empty;
            lbldisplay.Text = "0";
            lbldemo.Text = "0";
            lblservice.Text = "0";
            lbldamage.Text = "0";
            lblprices.Text = "0";
            lblVat.Text = "0";
            radioButton1.Checked = true;
            radioButton1.Checked = false;
            cmbpaymode.Focus();
        }

        public void RemoveNullColumnFromDataTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][3])))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
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
                    tb.TextChanged += new EventHandler(textbox_Change);
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.MaxLength = 10;
                }
            }
        }

        private void textbox_Change(object sender, EventArgs e)
        {
            try
            {

                //double rate = Convert.ToInt32(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
                //double amt = rate * Convert.ToDouble(tb.Text);
                //dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value = amt;

            }
            catch
            {
                //dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value = 0.00;
            }
        }
        Regex reg = new Regex(@"^-?\d+[.]?\d*$");
        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (!reg.IsMatch(tb.Text.Insert(tb.SelectionStart, e.KeyChar.ToString()) + "1")) e.Handled = true;
        }

        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //total();
            //if (e.ColumnIndex == 1)
            //{
            //    pnsearch.Visible = true;
            //    this.ActiveControl = Txtitem;
            //    lblrowindex.Text = e.RowIndex.ToString();
            //    lblpageno.Text = Convert.ToString(Convert.ToInt32(lblpageno.Text) + 1);
            //}
            //else
            //{
            //    pnsearch.Visible = false; ;
            //}


            total();
            if (e.ColumnIndex == 1)
            {
                //rdbStartsWith.Checked = true;
                if (dgvOrder.ReadOnly == false)
                {
                    pnsearch.Visible = true;
                }
                //pnsearch.Visible = true;
                //if (!string.IsNullOrEmpty(lblhiddenproduct.Text))
                //{
                //    Txtitem.Text = lblhiddenproduct.Text;
                //    AutoCompleteLoad(Txtitem.Text, 1);
                //    if (DgvAutoRefNo.Rows.Count > 0)
                //    {
                //        DgvAutoRefNo.Rows[0].Cells[0].Selected = false;
                //        DefaultFloor.Text = "0";
                //        Display.Text = "0";
                //        Damage.Text = "0";
                //        Checking.Text = "0";
                //        Delivery.Text = "0";
                //        lblprice.Text = "0";
                //    }

                //}

                Txtitem.SelectionStart = 0;
                Txtitem.SelectionLength = Txtitem.Text.Length;
                dgvOrder.Focus();

                if (radioButton1.Checked)
                {
                    this.ActiveControl = Txtitem;
                }
                else
                {
                    this.ActiveControl = textBox1;
                }
                // dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];


                //textBox1.SelectionStart = 0;
                //textBox1.SelectionLength = textBox1.Text.Length;
                //dgvOrder.Focus();
                //// dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                //this.ActiveControl = textBox1;
                lblrowindex.Text = e.RowIndex.ToString();
                lblpageno.Text = Convert.ToString(Convert.ToInt32(lblpageno.Text) + 1);
            }
        }


        private void button_click(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;

            if (btn.Checked)
            {
                getquotaion(btn.Text);
                total();
                panel2.Enabled = true;
            }

        }

        public void getquotaion(string s)
        {
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
                dateTimePicker1.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);

            }
            else
            {

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
                    dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["HSN"]);
                    dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvOrder.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    double qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    dgvOrder.Rows[i].Cells[5].Value = qty;
                    dgvOrder.Rows[i].Cells[6].Value = Convert.ToString(ds.Tables[1].Rows[i]["GST"]);
                    dgvOrder.Rows[i].Cells[7].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvOrder.Rows[i].Cells[8].Value = amt;
                    dgvOrder.Rows[i].Cells[9].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                }

            }
            else
            {
                dgvOrder.Rows.Clear();

            }

        }

        public void Getestimate(string s)
        {
            DataSet ds = objQuotationbal.Getestimate(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                cmdcity.Text = Convert.ToString(ds.Tables[0].Rows[0]["city"]);
                cmbreference.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Referenceid"]);
                cmbassistby.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Assist"]);
                lblperare.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);
                txtorder.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);
                cmbstatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                // date.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);
                lbltotalamount.Text = Convert.ToString(ds.Tables[0].Rows[0]["Total"]);
                txtless.Text = Convert.ToString(ds.Tables[0].Rows[0]["LessAmount"]);
                Txtothers.Text = Convert.ToString(ds.Tables[0].Rows[0]["Others"]);

            }
            else
            {

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
                    dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["HSN"]);
                    dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvOrder.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    double qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    dgvOrder.Rows[i].Cells[5].Value = qty;
                    dgvOrder.Rows[i].Cells[6].Value = Convert.ToString(ds.Tables[1].Rows[i]["GST"]);
                    dgvOrder.Rows[i].Cells[7].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvOrder.Rows[i].Cells[8].Value = amt;
                    dgvOrder.Rows[i].Cells[9].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                }

            }
            else
            {
                dgvOrder.Rows.Clear();

            }

        }

        public void total()
        {
            try
            {
                double totalamount = 0, totalquantity = 0;
                for (int i = 0; i < dgvOrder.Rows.Count; i++)
                {
                    totalamount = totalamount + Convert.ToDouble(dgvOrder.Rows[i].Cells[8].Value);
                    totalquantity = totalquantity + Convert.ToDouble(dgvOrder.Rows[i].Cells[7].Value);
                }


                lbltotalquantity.Text = Convert.ToString(totalquantity);

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


                // totalamount = Math.Round(totalamount);

                lbltotalamount.Text = totalamount.ToString("F2");
                totalamount = totalamount + Convert.ToDouble(Txtothers.Text) - Convert.ToDouble(txtless.Text);


                totalamount = totalamount - Convert.ToDouble(Txtothers.Text);

                lblgrandtotal.Text = totalamount.ToString("F2");
            }
            catch
            {

            }

        }

        private void dgvOrder_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double rate = Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value);
                if (!String.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value)))
                {
                    double amt = 0;
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value)))
                    {
                        amt = rate * Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value);
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

                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value = amt;
                    }
                }

            }
            catch
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value)))
                    {
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value = 0.00;
                    }
                }
            }
            total();

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {



            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);

            if (rdquotation.Checked)
            {
                search(FromDate, ToDate);
            }
            else
            {
                searchEstimationsales(FromDate, ToDate);
            }

        }

        public void bindorderno(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = objQuotationbal.ordernoval();
            cmb.DataSource = dt;
            cmb.DisplayMember = "Quotationid";
            cmb.ValueMember = "Quotationid";
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






        public void search(DateTime f, DateTime t)
        {

            DataTable dt = objQuotationbal.searchQuotationsale(f, t);
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


        public void searchEstimationsales(DateTime f, DateTime t)
        {

            DataTable dt = objQuotationbal.searchEstimationsales(f, t);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Estimationid"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Amount"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["customername"]);
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
                    if (rdquotation.Checked)
                    {
                        getquotaion(s);
                    }
                    else if (rdestimation.Checked)
                    {
                        Getestimate(s);
                    }
                    total();
                    btnSavePending.Enabled = false;
                    btnSave.Enabled = true;
                    btnPrint.Enabled = false;

                }
                else
                {
                    clear();
                }

            }
        }

        private void dgvOrder_KeyDown(object sender, KeyEventArgs e)
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
                else if (dgvOrder.CurrentCell.ColumnIndex == 5)
                {
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[6, dgvOrder.CurrentCell.RowIndex];

                }

                //else if (dgvOrder.CurrentCell.ColumnIndex == 6)
                //{
                //    dgvOrder.Focus();
                //    dgvOrder.CurrentCell = dgvOrder[7, dgvOrder.CurrentCell.RowIndex];

                //}
                else if (dgvOrder.CurrentCell.ColumnIndex == 8)
                {
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];

                }



            }


            catch
            {

            }
        }

        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            int aa = e.ColumnIndex;

            try
            {
                if (e.ColumnIndex == 7)
                {

                    double ordqty = Convert.ToDouble(dgvOrder.Rows[e.RowIndex].Cells[7].Value);
                    double qty = Convert.ToDouble(tb.Text);
                    if (ordqty > 0)
                    {
                        if (ordqty < qty)
                        {
                            MessageBox.Show("Quantity should  not be greater than original quantity");
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder[6, e.RowIndex];
                            dgvOrder.Rows[e.RowIndex].Cells[6].Value = ordqty;

                        }
                        else
                        {
                            dgvOrder.Focus();
                            //edit = true;
                            dgvOrder.CurrentCell = dgvOrder[6, e.RowIndex];
                        }
                    }
                }
                dgvOrder.CurrentCell = dgvOrder[1, e.RowIndex];
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


            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(Txtitem.Text))
                    {
                        if (Convert.ToInt32(lblproductid.Text) != 0)
                        {
                            dgvOrder.Columns[6].DefaultCellStyle.Format = "F2";
                            int rowindex = Convert.ToInt32(lblrowindex.Text);
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7];
                            dgvOrder.Rows[rowindex].Cells[2].Value = lbelhsncode.Text;
                            dgvOrder.Rows[rowindex].Cells[4].Value = lblproductid.Text;
                            dgvOrder.Rows[rowindex].Cells[1].Value = Txtitem.Text.ToUpper();
                            dgvOrder.Rows[rowindex].Cells[3].Value = lblitemcode.Text;
                            double val = Convert.ToDouble(lblprice.Text);
                            dgvOrder.Rows[rowindex].Cells[5].Value = val;
                            dgvOrder.Rows[rowindex].Cells[6].Value = lblVat.Text;
                            dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                            pnsearch.Visible = false;
                            lblproductid.Text = string.Empty;
                            Txtitem.Text = string.Empty;
                            lblitemcode.Text = "0";
                            lblHSNCode.Text = "0";
                            lbldisplay.Text = "0";
                            lbldemo.Text = "0";
                            lblservice.Text = "0";
                            lbldamage.Text = "0";
                            lblprice.Text = "0";
                            lbelhsncode.Text = "0";
                            lblVat.Text = "0";
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7];
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
            }

            catch (Exception ex)
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



        private void rdquotation_CheckedChanged(object sender, EventArgs e)
        {
            btnSearch.PerformClick();
        }

        private void rdestimation_CheckedChanged(object sender, EventArgs e)
        {


        }

        public void getsaleamount(int shop)
        {

            string values = string.Empty;
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlCommand cmd = null;

                if (shop == 1)
                {

                    cmd = new SqlCommand("getsaleamount", con);
                }

                if (shop == 2)
                {
                    cmd = new SqlCommand("getsaleamountPipes_Direct1", con);

                }
                //SqlCommand cmd = new SqlCommand("getsaleamountPipes", con);
                cmd.CommandType = CommandType.StoredProcedure;
                values = Convert.ToString(cmd.ExecuteScalar());
                con.Close();
            }
            string s = values;
            //string s = objQuotationbal.getsaleamount(shop);
            if (!string.IsNullOrEmpty(s))
            {
                lbltodaysales.Text = s;
            }
            else
            {
                lbltodaysales.Text = "0";
            }
        }

        private void txtless_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtless.Text))
                {

                    double total = Convert.ToDouble(lbltotalamount.Text);
                    if (string.IsNullOrEmpty(txtless.Text))
                    {
                        txtless.Text = "0";
                    }
                    if (string.IsNullOrEmpty(Txtothers.Text))
                    {
                        Txtothers.Text = "0";
                    }
                    double less = Convert.ToDouble(txtless.Text);

                    if (total < less)
                    {
                        MessageBox.Show("Less amount shold not be greater than actual amount");
                        txtless.Text = "0";
                        txtless.SelectionStart = 0;
                        txtless.SelectionLength = 1;
                        txtless.Focus();
                        lblgrandtotal.Text = Convert.ToDouble(lbltotalamount.Text).ToString("F2");

                    }

                    else
                    {
                        double others = Convert.ToDouble(Txtothers.Text);
                        double grandtotal = total + others - less;
                        lblgrandtotal.Text = grandtotal.ToString("F2");
                    }
                }
                else
                {
                    lblgrandtotal.Text = Convert.ToDouble(lbltotalamount.Text).ToString("F2");
                }
            }
            catch
            {

            }

        }

        private void dgvOrder_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double rate = Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value);
                if (!String.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value)))
                {
                    double amt = 0;
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value)))
                    {
                        amt = rate * Convert.ToDouble(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7].Value);
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

                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value = amt;
                    }
                }

            }
            catch
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value)))
                    {
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[8].Value = 0.00;
                    }
                }
            }
            total();
        }

        private void txtless_KeyPress(object sender, KeyPressEventArgs e)
        {
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

            // only allow minus sign at the beginning
            if (e.KeyChar == '-' && (sender as TextBox).Text.Length > 0)
            {
                e.Handled = true;
            }
        }

        private void txtmobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Txtothers_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtless.Text))
                {

                    double total = Convert.ToDouble(lbltotalamount.Text);
                    if (string.IsNullOrEmpty(txtless.Text))
                    {
                        txtless.Text = "0";
                    }
                    if (string.IsNullOrEmpty(Txtothers.Text))
                    {
                        Txtothers.Text = "0";
                    }

                    double less = Convert.ToDouble(txtless.Text);
                    double others = Convert.ToDouble(Txtothers.Text);
                    double grandtotal = total + others - less;
                    lblgrandtotal.Text = grandtotal.ToString("F2");
                }
                else
                {
                    lblgrandtotal.Text = Convert.ToDouble(lbltotalamount.Text).ToString("F2");
                }
            }
            catch
            {

            }
        }

        private void cmbreference_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public void chages()
        {
            Program.ShopName = Convert.ToString(cmbcompanychange.SelectedIndex);

            Program.Company = Convert.ToString(cmbcompanychange.Text);
            recal();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cmbcompanychange.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Company name");
            }
            else
            {
                // Program.Company = cmbcompanychange.Text;
                chages();
                chages();
                clear();
                MessageBox.Show("Shop Changed Successfully");
            }




        }

        private void dgvOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Visible = false;
            label34.Visible = true;
            lblHSNCode.Visible = true;
            label29.Visible = false;
            ItemName.Visible = false;
            Txtitem.Visible = true;
            textBox1.Text = "";
            lblprices.Text = "0";
            ItemName.Visible = false;
            lblHSNCode.Text = "0";
            Txtitem.Focus();

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Txtitem.Visible = false;
            Txtitem.Text = "";
            lblprices.Text = "0";
            label34.Visible = false;
            lblHSNCode.Visible = false;
            label29.Visible = true;
            ItemName.Visible = true;


            textBox1.Visible = true;
            // lblHSNCode.Visible = false;
            ItemName.Visible = true;
            ItemName.Text = "";
            textBox1.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox1.Visible = false;
                textBox1.Text = "";
                lblprice.Text = "0";
                Txtitem.Visible = true;
                Txtitem.Focus();

                lblHSNCode.Visible = true;
                ItemName.Visible = false;
                itemdetails();
            }
            else if (radioButton2.Checked)
            {
                textBox1.Visible = true;
                Txtitem.Text = "";
                Txtitem.Visible = false;
                lblHSNCode.Visible = false;
                ItemName.Visible = true;
                lblprice.Text = "0";
                textBox1.Focus();
                itemdetailsHSN();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(textBox1.Text))
                    {
                        if (Convert.ToInt32(HSNPrice.Text) != 0)
                        {
                            dgvOrder.Columns[6].DefaultCellStyle.Format = "F2";
                            int rowindex = Convert.ToInt32(lblrowindex.Text);
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7];
                            dgvOrder.Rows[rowindex].Cells[2].Value = lbelhsncode.Text;
                            dgvOrder.Rows[rowindex].Cells[4].Value = HSNPrice.Text;
                            dgvOrder.Rows[rowindex].Cells[1].Value = ItemName.Text.ToUpper();
                            dgvOrder.Rows[rowindex].Cells[3].Value = lblitemcode.Text;
                            double val = Convert.ToDouble(lblprice.Text);
                            dgvOrder.Rows[rowindex].Cells[5].Value = val;
                            dgvOrder.Rows[rowindex].Cells[6].Value = lblVat.Text;
                            dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                            pnsearch.Visible = false;
                            HSNPrice.Text = string.Empty;
                            textBox1.Text = string.Empty;
                            ItemName.Text = "";
                            lblitemcode.Text = "0";
                            //lblHSNCode.Text = "0";
                            lbldisplay.Text = "0";
                            lbldemo.Text = "0";
                            lblservice.Text = "0";
                            lbldamage.Text = "0";
                            lblprice.Text = "0";
                            lbelhsncode.Text = "0";
                            lblVat.Text = "0";
                            dgvOrder.Focus();
                            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[7];
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
            }

            catch (Exception ex)
            {

            }
        }

        private void SGST_CheckedChanged(object sender, EventArgs e)
        {

            comboBox2.Visible = true;
            comboBox1.Visible = true;
            if (SGSTval.Checked)
            {
                comboBox2.Visible = true;
                comboBox1.Visible = false;

            }

            else if (IGSTval.Checked)
            {
                comboBox2.Visible = false;
                comboBox1.Visible = true;

            }
        }


    }
}
