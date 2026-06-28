using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Sales
{
    public partial class SalesReturnList : Form
    {
        bool load = false;
        string role1 = string.Empty;
        string srole = string.Empty;
        QuotationBal objQuotationbal = new QuotationBal();
        DataTable dtreceivedbalance, dtpaidbalance;
        ComboBox cmblocation;
        DataTable dtitems;
        TextBox tb, tbamount, tbbaalanceanount, tborderquantoty;
        public bool edit = false;
        string clickstatus = string.Empty;
        string selectedtab = string.Empty;
        string cas = string.Empty;
        int ProdSelRowvalue = 0;
        bool res = false;
        bool ProdNotFoundMSg = false;
        public SalesReturnList()
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
            
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            SearchPurchaseOrder();
            bindcustomer();

            txtsearch1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtsearch1.AutoCompleteCustomSource = Autocusomer();
            txtsearch1.AutoCompleteSource = AutoCompleteSource.CustomSource;


           // selectedtab = MainTabSalesBill.SelectedTab.Name;

            //if (selectedtab == "TabNew")
            //{
                search("Returnid", "", "sr.Updatedon", "Today", "CustomerName", "", role1, Program.userid);
                DataTable dt = bindEstimation();
                cmbstatus1.DataSource = dt;
                cmbstatus1.DisplayMember = "Returnid";
                cmbstatus1.ValueMember = "Returnid";

                cmbstatus2.DataSource = dt;
                cmbstatus2.DisplayMember = "Returnid";
                cmbstatus2.ValueMember = "Returnid";


                cmbstatus3.DataSource = dt;
                cmbstatus3.DisplayMember = "Returnid";
                cmbstatus3.ValueMember = "Returnid";
                
                btnPrint.Enabled = true;
           // }



            //else if (selectedtab == "TabChecking")
            //{

            //    searchcheck("Returnid", "", "sr.Updatedon", "Today", "CustomerName", "", role1, Program.userid);

            //    DataTable dt = bindcheckout();
            //    cmbstatus1.DataSource = dt;
            //    cmbstatus1.DisplayMember = "Estimationid";
            //    cmbstatus1.ValueMember = "Estimationid";

            //    cmbstatus2.DataSource = dt;
            //    cmbstatus2.DisplayMember = "Estimationid";
            //    cmbstatus2.ValueMember = "Estimationid";


            //    cmbstatus3.DataSource = dt;
            //    cmbstatus3.DisplayMember = "Estimationid";
            //    cmbstatus3.ValueMember = "Estimationid";

               
            //    btnPrint.Enabled = false;
            //}

        }
        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue, string role, string UserId)
        {
            DataTable dt = objQuotationbal.searchsalesreturnList(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue, role1, Program.userid);
            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i][0]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i][1]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i][2]);
                dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i][3]);
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void LoadPortsNew()
        {

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


           

            

            dgvNew.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvNew.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvNew.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
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
        public DataTable bindEstimation()
        {
            DataTable dt = new DataTable();
            dt = objQuotationbal.itemSalesreturnorderno();
            DataRow dr = dt.NewRow();
            dr["Returnid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);

            return dt;
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

        private void SearchPurchaseOrder()
        {

            dgvSearch.Rows.Clear();
            dgvSearch.Columns.Clear();
            dgvSearch.ColumnCount = 4;


            dgvSearch.Columns[0].Name = "Order No";
            dgvSearch.Columns[1].Name = "Customer Name";
            dgvSearch.Columns[2].Name = "Date";
            dgvSearch.Columns[3].Name = "Status";




            this.dgvSearch.Columns[0].Width = 60;

            this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvSearch.Columns[1].Width = 60;




            this.dgvSearch.Columns[2].Width = 60;
            this.dgvSearch.Columns[3].Width = 60;





            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
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
        public void bindcustomer()
        {
            Txtcustomername.DataSource = objQuotationbal.Getcustomer();
            Txtcustomername.DisplayMember = "Name";
            Txtcustomername.ValueMember = "CustomerID";
        }
        private void Txtcustomername_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (load)
            {
                Txtcustomername.Text = string.Empty;
                load = false;
            }
            else
            {
                load = false;
            }
            string s = "";
            if (Txtcustomername.SelectedIndex > 0)
            {
                s = Convert.ToString(Txtcustomername.SelectedValue);
            }

            DataTable dt = objQuotationbal.Getpaidamunt(s);

            lblpaid.Text = Convert.ToString(dt.Rows[0]["Paid"]);
            lblbalance.Text = Convert.ToString(dt.Rows[0]["Balance"]);
            txtless.Text = Convert.ToString(dt.Rows[0]["Balance"]);
            if (string.IsNullOrEmpty(lblpaid.Text))
            {
                lblpaid.Text = "0.00";
            }
            if (string.IsNullOrEmpty(lblbalance.Text))
            {
                lblbalance.Text = "0.00";
                txtless.Text = "0.00";
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
                vLabel4.Enabled = true;
            }
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
                this.dgvSearch.Columns[1].Visible = true;
                this.dgvSearch.Columns[2].Visible = true;
                this.dgvSearch.Columns[3].Visible = true;

            }
        }

        private void vLabel5_Click(object sender, EventArgs e)
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

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string s = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                if (!string.IsNullOrEmpty(s))
                {
                    getEstimation(s);
                    total();
                    Txtcustomername.Focus();
                }
                else
                {
                    clear();
                }

            }
        }
        public void total()
        {
            double totalamount = 0, totalquantity = 0;
            for (int i = 0; i < dgvNew.Rows.Count; i++)
            {
                totalamount = totalamount + Convert.ToDouble(dgvNew.Rows[i].Cells[6].Value);
                //totalquantity = totalquantity + Convert.ToInt32(dgvNew.Rows[i].Cells[5].Value);
            }

            //lbltotalquantity.Text = Convert.ToString(totalquantity);

            //totalamount = Math.Round(totalamount);


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

            lbltotalamount.Text = String.Format("{0:00.00}", totalamount);

            double total = Convert.ToDouble(lbltotalamount.Text);
            double less = Convert.ToDouble(txtless.Text);
            double grandtotal = less - total;



            lblTotal.Text = String.Format("{0:00.00}", grandtotal);



        }

        public void getEstimation(string s)
        {
            DataSet ds = objQuotationbal.getSalesreturn(s);
            if (ds.Tables[0].Rows.Count > 0)
            {

                Txtcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["Customerid"]);
                lblpaid.Text = Convert.ToString(ds.Tables[0].Rows[0]["Paid"]);
                lblbalance.Text = Convert.ToString(ds.Tables[0].Rows[0]["Balance"]);
                txtless.Text = lblbalance.Text;
                txtRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                //panel2.Enabled = false;
            }
            else
            {
               // panel2.Enabled = true;
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

                    double qty = Convert.ToDouble(ds.Tables[1].Rows[i]["Rate"]);
                    dgvNew.Rows[i].Cells[4].Value = qty;
                    dgvNew.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);

                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvNew.Rows[i].Cells[6].Value = amt;
                }
               // panel9.Enabled = false;
               // btnsave.Enabled = false;
            }
            else
            {
                dgvNew.Rows.Clear();
                //panel9.Enabled = true;
                //btnsave.Enabled = true;
            }

        }

         private void clear()
        {
           
               // pnsearch.Visible = false;
                Txtcustomername.SelectedIndex = 0;
                dgvNew.Rows.Clear();
                lbltotalamount.Text = "0.00";
                txtless.Text = "0.00";
                lblTotal.Text = "0.00";
                lblpaid.Text = "0.00";
                lblbalance.Text = "0.00";
                Txtcustomername.Focus();
                txtRemarks.Text = string.Empty;
               // cmbloaction.SelectedIndex = 0;
                panel9.Enabled = true;
               // btnsave.Enabled = true;
                Txtcustomername.Text = string.Empty;
            }

         private void btnPrint_Click(object sender, EventArgs e)
         {

         }

         private void btnClear_Click(object sender, EventArgs e)
         {

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
                             getEstimation(s);
                             total();
                             Txtcustomername.Focus();
                         }
                         else
                         {
                             clear();
                         }

                     }
                 }
                 else if (e.KeyData == Keys.Up)
                 {
                     if (dgvSearch.CurrentCell.RowIndex >= 0)
                     {
                         string s = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex - 1].Cells[0].Value);
                         if (!string.IsNullOrEmpty(s))
                         {
                             getEstimation(s);
                             total();
                             Txtcustomername.Focus();
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



    }
}
