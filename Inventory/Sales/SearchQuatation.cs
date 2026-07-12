
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
using QuotationReport;
using System.Diagnostics;

namespace Inventory
{
    public partial class SearchQuatation : Form
    {
        QuotationBal objQuotationbal = new QuotationBal();
        TextBox tb;
        public bool edit = false;
        int userid = 0;
        string cas = string.Empty;
        int ProdSelRowvalue = 0;
        bool res = false;
        bool all = false;
        int page;
        string fileName;
        string firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue;
        public SearchQuatation()
        {
            InitializeComponent();
            userid = Convert.ToInt32(Program.userid);
            this.WindowState = FormWindowState.Maximized;
            LoadPorts();


            //  bindLocation();  
            //comboBox1.SelectedIndex = 0;
            cmbstatus.SelectedIndex = 0;
            SearchPurchaseOrder();
            lblperare.Text = Program.UserName; ;
            bindAssist();
            bindreference();
            bindcustomer();

            //   bindpending();
            pnlprodsearch.Visible = false;
            cmbIscombined.Checked = true;
            Globeimage();
            GetSearchSalesOrder();
            //  panel2.Enabled = false;


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

            //cmbcustomer.DataSource = objQuotationbal.Getcustomer();
            //cmbcustomer.DisplayMember = "Name";
            //cmbcustomer.ValueMember = "CustomerID";




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
            dgvOrder.ColumnCount = 7;
            //dgvOrder.RowCount = 16;

            dgvOrder.Columns[0].Name = "S.NO";
            dgvOrder.Columns[1].Name = "Items";
            dgvOrder.Columns[2].Name = "UOM";
            dgvOrder.Columns[5].Name = "Quantity";
            dgvOrder.Columns[3].Name = "productid";
            dgvOrder.Columns[4].Name = "Rate";
            dgvOrder.Columns[6].Name = "Amount";

            dgvOrder.Columns[3].Visible = false;

            this.dgvOrder.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvOrder.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns[1].ReadOnly = true;
            this.dgvOrder.Columns[2].ReadOnly = true;
            this.dgvOrder.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrder.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrder.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
            dgvOrder.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvOrder.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[4].ReadOnly = true;

            this.dgvOrder.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;




            this.dgvOrder.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvOrder.Columns[6].ReadOnly = true;



            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvOrder.Columns[0].Width = 12;
                this.dgvOrder.Columns[1].Width = 100;
                this.dgvOrder.Columns[2].Width = 15;
                this.dgvOrder.Columns[4].Width = 15;
                this.dgvOrder.Columns[5].Width = 20;
                this.dgvOrder.Columns[6].Width = 100;

            }
            else
            {
                this.dgvOrder.Columns[0].Width = 12;
                this.dgvOrder.Columns[1].Width = 100;
                this.dgvOrder.Columns[2].Width = 15;
                this.dgvOrder.Columns[4].Width = 15;
                this.dgvOrder.Columns[5].Width = 15;
                this.dgvOrder.Columns[6].Width = 100;

            }

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

        private void SalesQuotation1_Load(object sender, EventArgs e)
        {
            this.ActiveControl = cmbcustomername;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {





            if (keyData == (Keys.F3))
            {
                txtOrderNo.Focus();
                this.ActiveControl = txtOrderNo;
                pnlprodsearch.Visible = true;
                txtprodsearch.SelectionStart = 0;
                txtprodsearch.SelectionLength = txtprodsearch.Text.Length;
                txtprodsearch.Text = "";
                txtprodsearch.Focus();
                return true;
            }
            if (txtprodsearch.Focused)
            {
                if (keyData == (Keys.Enter))
                {
                    if (!string.IsNullOrEmpty(txtprodsearch.Text))
                    {
                        GetSuppliersearch();
                        pnlprodsearch.Visible = true;
                    }
                }
            }
            if (keyData == (Keys.A | Keys.Alt))
            {
                all = true;
                GetSearchSalesOrder();

                return true;
            }


            if (cmbassistby.Focused)
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
                // pnsearch.Visible = false;
                //dgvOrder.Focus();
                //dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
                //dgvOrder.BeginEdit(true);

                this.Close();


                return true;
            }



            if (keyData == Keys.Escape)
            {

                this.Close();

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

            //if (cmbloaction.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = Txtitem;
            //        return true;
            //    }

            //}
            try
            {
                if (keyData == Keys.Tab)
                {
                    if (dgvOrder.CurrentCell.ColumnIndex == 5)
                    {
                        dgvOrder.Focus();
                        //edit = true;
                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
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

        public void GetSuppliersearch()
        {
            DataTable dt = objQuotationbal.GetCustomerNamesearch_Qutation(txtprodsearch.Text);
            DataTable SearchResult = FilterQuotationSearchResult(dt);

            lblItemCount.Text = SearchResult.Rows.Count.ToString();
            try
            {
                AlphanumComparator<string> comparer = new AlphanumComparator<string>();
                //DataTable dtNew = dv.Table;
                DataTable dtNew = SearchResult.AsEnumerable().OrderBy(x => x.Field<string>("Order No"), comparer).CopyToDataTable();
                //dtNew.TableName = "NaturalSort";
                SearchResult = dtNew;
            }
            catch (Exception ex)
            {

            }


            dgvSearch.Rows.Clear();
            for (int i = 0; i < SearchResult.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(SearchResult.Rows[i]["Order No"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(SearchResult.Rows[i]["Customer"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(SearchResult.Rows[i]["Order Date"]);
            }

            //lblItemCount.Text = Convert.ToString(dt.Rows.Count);


            dgvSearch.Columns["Order No"].Visible = true;
            //dgvSearch.Columns["CustomerName"].Visible = false;
            //dgvSearch.Columns["Order Date"].Visible = false;
            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            all = false;



        }
        private void SearchPurchaseOrder()
        {
            dgvSearch.Rows.Clear();
            dgvSearch.Columns.Clear();
            dgvSearch.ColumnCount = 3;
            //dgvSearch.RowCount = 16;

            dgvSearch.Columns[0].Name = "Order No";
            dgvSearch.Columns[1].Name = "CustomerName";
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

        public void AutoCompleteLoad(string s, int t)
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.itemauto(s, t);

            if (st.Rows.Count > 0)
            {

            }
            else
            {

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
                //  save(2);
                clear();
            }
        }

        private void btnSavePending_Click(object sender, EventArgs e)
        {
            bool val = Validation();
            if (val)
            {
                //save(1);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
            // panel2.Enabled = false;
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




            if (string.IsNullOrEmpty(cmdcity.Text))
            {
                i++;
                message = message + "* Please Select city" + "\n";
                if (i == 1)
                    this.ActiveControl = cmdcity;
            }

            if (cmbassistby.SelectedIndex == 0)
            {
                i++;
                message = message + "* Please Select Assist " + "\n";
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
                string Items = Convert.ToString(dgvOrder.Rows[k].Cells["Quantity"].Value);
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
            //       pnsearch.Visible = false;
            //btnSavePending.Enabled = true;
            //btnSave.Enabled = true;
            btnPrint.Enabled = true;
            cmbcustomername.Text = "--Select--";
            cmdcity.Text = string.Empty;
            cmbassistby.SelectedIndex = 0;
            cmbreference.SelectedIndex = 0;
            txtorder.Clear();
            dgvOrder.Rows.Clear();
            lblperare.Text = Program.UserName;
            lbltotalquantity.Text = "0";
            lbltotalamount.Text = "0";
            cmbcustomername.Focus();
            cmbIscombined.Checked = true;

            // cmbloaction.SelectedIndex = 0;
            // panel2.Enabled = true;
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
        public void GetReport(string QuotationId)
        {
            //try
            //{
            if (!string.IsNullOrEmpty(QuotationId))
            {
                DialogResult result = MessageBox.Show("Do you want to PrintView" + QuotationId + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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


                        //EstimationReportDAL objRREPrint = new EstimationReportDAL();
                        //objRREPrint.dsMain = ds;
                        //if (objRREPrint.GenerateQuoationPrintViewReport())
                        //{
                        //    try
                        //    {

                        //        frmPrintPreview objfrmpreview = new frmPrintPreview();
                        //        objfrmpreview.fileName = objRREPrint.fileName;

                        //        objfrmpreview.Show();
                        //    }

                        //    catch (Exception ex)
                        //    {

                        //    }
                        //}


                        QuotationReportDAL Obj = new QuotationReportDAL();
                        Obj.dsMain = ds;
                        Obj.pagenumber = 1;
                        Obj.status = true;
                        if (Obj.GenerateQuoation())
                        {
                            //page = Obj.pagenumber;

                            //lblqid.Text = txtorder.Text;
                            //panel4.Visible = true;
                            //rdrange.Checked = false;
                            //rdall.Checked = true;
                            //rdrange.Text = "Range(" + page + ")";
                            //fileName = Obj.fileName;

                            //textBox2.Text = string.Empty;
                            //textBox2.Text = string.Empty;

                            //// print(QuotationId,page);

                            frmPrintPreview objfrmpreview = new frmPrintPreview();
                            objfrmpreview.fileName = Obj.fileName;
                            objfrmpreview.Show();

                        }
                    }
                }



                //System.Diagnostics.Process myProc = new System.Diagnostics.Process();
                //myProc.StartInfo.FileName = "type " + Obj.fileName + " >prn";  //Attempting to start a non-existing executable
                //myProc.Start();    //Start the application and assign it to the process component.    
                //ExecuteCommandSync("type " + Obj.fileName + " >prn");







            }


            else
            {
                MessageBox.Show("Please Select the Quotation");
            }
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message.ToString());
            //}
        }



        public void print()
        {

            if (rdall.Checked)
            {
                PrintBuffer();
                frmPrintPreview objfrmpreview = new frmPrintPreview();
                objfrmpreview.fileName = fileName;
                objfrmpreview.Show();
            }

            else if (rdrange.Checked)
            {

                using (SqlConnection con = new SqlConnection(Program.connection))
                {
                    DataSet ds = new DataSet();
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.AddWithValue("@id", lblqid.Text);
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
                            objRREPrint._strRef = lblqid.Text;

                            objRREPrint.RREPrintQuotation();
                        }

                        catch (Exception ex)
                        {

                        }
                    }

                }
            }
        }

        public void PrintBuffer()
        {

            Process cmd = new Process();
            cmd.StartInfo.FileName = "d:\\bill.bat";

            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            cmd.Start();


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

        public void preview()
        {
            if (!string.IsNullOrEmpty(txtorder.Text))
            {


                Pnloading.Visible = false;



                DialogResult result = MessageBox.Show("Do you want to Print" + txtorder.Text + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                if (result == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Program.connection))
                    {
                        DataSet ds = new DataSet();
                        con.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Parameters.AddWithValue("@id", txtorder.Text);
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
                                objRREPrint.dsMain1 = ds;
                                objRREPrint.pagenumber = 1;
                                objRREPrint.status = true;
                                objRREPrint._strRefText = "Qtn:";
                                objRREPrint._strRef = txtorder.Text;

                                objRREPrint.RREPrintQuotation();
                            }

                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString());
                            }
                        }
                    }
                }
            }
        }




        private void cmbcustomername_SelectedIndexChanged(object sender, EventArgs e)
        {


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



        }




        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            ProdSelRowvalue = 0;

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
                    tb.MaxLength = 6;
                }
            }
        }

        private void textbox_Change(object sender, EventArgs e)
        {

        }

        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void button_click(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;

            if (btn.Checked)
            {
                getquotaion(btn.Text);
                total();
                // panel2.Enabled = true;
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
                date.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);
                //panel2.Enabled = false;
            }
            else
            {
                //panel2.Enabled = false;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                string s1 = string.Empty;
                double qty;
                dgvOrder.Rows.Clear();
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

                    dgvOrder[6, i].Value = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);

                }
                //  panel2.Enabled = false;
            }
            else
            {
                dgvOrder.Rows.Clear();
                //   panel2.Enabled = false;
            }

        }

        public void total()
        {
            double totalamount = 0.0D, totalquantity = 0.0D;
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

            lbltotalquantity.Text = Convert.ToString(totalquantity);
            lbltotalamount.Text = String.Format("{0:0,0.00}", totalamount);

        }

        private void dgvOrder_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.ColumnIndex == 6)
            //{
            //try
            //{

            //    double rate = Convert.ToInt32(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
            //    double amt = rate * Convert.ToDouble(tb.Text);
            //    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = amt;

            //}
            //catch
            //{
            //    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
            //    {
            //        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = 0.00;
            //    }
            //}
            //total();

            //}
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

        }

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





        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, int userid)
        {

            DataTable dt = objQuotationbal.search(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, userid);
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
                    // btnSavePending.Enabled = false;
                    // btnSave.Enabled = false;
                    btnPrint.Visible = true;


                }
                else
                {
                    clear();
                }

            }
        }

        private void dgvOrder_KeyDown(object sender, KeyEventArgs e)
        {
            //    if (e.KeyData == Keys.Enter)
            //    {
            //        e.SuppressKeyPress = true;
            //        if (dgvOrder.Rows.Count > 0)
            //        {
            //            if (dgvOrder.CurrentCell.ColumnIndex == 0)
            //            {
            //                dgvOrder.Focus();
            //                dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex];

            //            }
            //            else if (dgvOrder.CurrentCell.ColumnIndex == 1)
            //            {
            //                dgvOrder.Focus();
            //                dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];

            //            }
            //            else if (dgvOrder.CurrentCell.ColumnIndex == 2)
            //            {
            //                dgvOrder.Focus();
            //                dgvOrder.CurrentCell = dgvOrder[4, dgvOrder.CurrentCell.RowIndex];

            //            }
            //            else if (dgvOrder.CurrentCell.ColumnIndex == 4)
            //            {
            //                dgvOrder.Focus();
            //                dgvOrder.CurrentCell = dgvOrder[5, dgvOrder.CurrentCell.RowIndex];

            //            }


            //            else if (dgvOrder.CurrentCell.ColumnIndex == 6)
            //            {
            //                dgvOrder.Focus();
            //                dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];

            //            }
            //        }

            //    }
        }

        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{
            //    if (e.ColumnIndex == 5)
            //    {
            //        dgvOrder.Focus();
            //        //edit = true;
            //        dgvOrder.CurrentCell = dgvOrder[1, e.RowIndex + 1];
            //    }
            //}
            //catch
            //{

            //}
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



        private void DgvAutoRefNo_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvOrder_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{

            //    double rate = Convert.ToInt32(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
            //    double amt = rate * Convert.ToDouble(tb.Text);
            //    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = amt;

            //}
            //catch
            //{
            //    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
            //    {
            //        if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value)))
            //        {
            //            dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = 0.00;
            //       }

            //    }

            //}
            //total();
        }


        public void getsino()
        {
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                dgvOrder.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
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
            DataTable dt = objQuotationbal.searchsalesQuotation(OrderNo, FromDate, ToDate, Vendorid, Convert.ToInt32(Iscombined));


            DataTable SearchResult = FilterQuotationSearchResult(dt);

            lblItemCount.Text = SearchResult.Rows.Count.ToString();
            try
            {
                AlphanumComparator<string> comparer = new AlphanumComparator<string>();
                //DataTable dtNew = dv.Table;
                DataTable dtNew = SearchResult.AsEnumerable().OrderBy(x => x.Field<string>("Order No"), comparer).CopyToDataTable();
                //dtNew.TableName = "NaturalSort";
                SearchResult = dtNew;
            }
            catch (Exception ex)
            {

            }


            dgvSearch.Rows.Clear();
            for (int i = 0; i < SearchResult.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(SearchResult.Rows[i]["Order No"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(SearchResult.Rows[i]["Customer"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(SearchResult.Rows[i]["Order Date"]);
            }

            //lblItemCount.Text = Convert.ToString(dt.Rows.Count);


            dgvSearch.Columns["Order No"].Visible = true;
            //dgvSearch.Columns["CustomerName"].Visible = false;
            //dgvSearch.Columns["Order Date"].Visible = false;
            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            all = false;


        }

        private DataTable FilterQuotationSearchResult(DataTable source)
        {
            DataTable result = source.Clone();
            string productName = txtSearchProduct.Text.Trim();
            string qty = textSearchQty.Text.Trim();
            bool hasDetailFilter = !string.IsNullOrEmpty(productName) || !string.IsNullOrEmpty(qty);

            foreach (DataRow row in source.Rows)
            {
                if (hasDetailFilter)
                {
                    string orderNo = Convert.ToString(row["Order No"]);
                    if (QuotationHasMatchingProduct(orderNo, productName, qty))
                    {
                        result.ImportRow(row);
                    }

                    continue;
                }
                else
                {
                    result.ImportRow(row);
                }
            }

            return result;
        }

        private bool QuotationHasMatchingProduct(string quotationId, string productName, string qty)
        {
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT TOP 1 1
                    FROM QuotationDetails qd
                    INNER JOIN ProductMaster pm ON qd.Productid = pm.id
                    WHERE qd.Quotationid = @QuotationId
                      AND (@ProductName = ''
                           OR pm.DisplayName LIKE @ProductLike
                           OR pm.ItemName LIKE @ProductLike)
                      AND (@Qty = '' OR CONVERT(varchar(50), qd.Quantity) = @Qty)";

                cmd.Parameters.AddWithValue("@QuotationId", quotationId);
                cmd.Parameters.AddWithValue("@ProductName", productName);
                cmd.Parameters.AddWithValue("@ProductLike", "%" + productName + "%");
                cmd.Parameters.AddWithValue("@Qty", qty);

                con.Open();
                return cmd.ExecuteScalar() != null;
            }
        }

        private void dgvOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetReport(txtorder.Text);
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
                            // btnSavePending.Enabled = false;
                            // btnSave.Enabled = false;
                            btnPrint.Visible = true;


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
                            // btnSavePending.Enabled = false;
                            // btnSave.Enabled = false;
                            btnPrint.Visible = true;


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

        private void button3_Click(object sender, EventArgs e)
        {
            clearprint();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (rdrange.Checked)
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please Enter From Page");
                }
                else if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("Please Enter To Page");
                }

                else if (Convert.ToInt32(textBox1.Text) > Convert.ToInt32(textBox2.Text))
                {
                    MessageBox.Show("From Page Should Not Be Greater Than To Page");
                }
                else
                {
                    print();
                    clearprint();
                }
            }
            else
            {
                print();
                clearprint();
            }
        }
        public void clearprint()
        {
            panel4.Visible = false;
            rdall.Checked = false;
            rdrange.Checked = false;
            //lblfrom.Visible = false;
            //lblto.Visible = false;
            textBox2.Text = string.Empty;
            textBox2.Text = string.Empty;
            //textBox1.Visible = false;
            //textBox2.Visible = false;

        }

        private void textSearchQty_KeyPress(object sender, KeyPressEventArgs e)
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

        private void button4_Click(object sender, EventArgs e)
        {
           
        }

        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
        }
    }
   
}
