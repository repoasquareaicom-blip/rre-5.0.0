
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
using System.Configuration;


namespace Inventory.Sales
{
    public partial class SalesNewreport : Form
    {
        string role1 = string.Empty;
        string srole = string.Empty;
        QuotationBal objQuotationbal = new QuotationBal();
        DataTable dtreceivedbalance, dtpaidbalance;
        ComboBox cmblocation;
        TextBox tb, tbamount, tbbaalanceanount, tborderquantoty;
        public bool edit = false;
        string clickstatus = string.Empty;
        string selectedtab = string.Empty;
        string reportname=string.Empty;
        static string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        public SalesNewreport()
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

            search();
            Bindcompany();
            //SearchCreteria1();
            //SearchCreteria2();
            //SearchCreteria3();

            //SearchPurchaseOrder();
            //searchsalebyid("");
            //txtsearch1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //txtsearch1.AutoCompleteCustomSource = Autocusomer();
            //txtsearch1.AutoCompleteSource = AutoCompleteSource.CustomSource;


            //txtsearch2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //txtsearch2.AutoCompleteCustomSource = Autocusomer();
            //txtsearch2.AutoCompleteSource = AutoCompleteSource.CustomSource;


            //txtsearch3.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //txtsearch3.AutoCompleteCustomSource = Autocusomer();
            //txtsearch3.AutoCompleteSource = AutoCompleteSource.CustomSource;




            //search("Salesid", "", "sr.Updatedon", "Today", "Name", "", role1, Program.userid);
                DataTable dt = bindEstimation();
                //cmbstatus1.DataSource = dt;
                //cmbstatus1.DisplayMember = "Salesid";
                //cmbstatus1.ValueMember = "Salesid";

                //cmbstatus2.DataSource = dt;
                //cmbstatus2.DisplayMember = "Salesid";
                //cmbstatus2.ValueMember = "Salesid";


                //cmbstatus3.DataSource = dt;
                //cmbstatus3.DisplayMember = "Salesid";
                //cmbstatus3.ValueMember = "Salesid";
             
    



            
        }

    

        #region Search
        private void pbxCollapse_Click(object sender, EventArgs e)
        {
           
        }
        private void vLabel2_Click(object sender, EventArgs e)
        {
           
        }

      






        //private void lblAll_Click(object sender, EventArgs e)
        //{
        //    if (clickstatus == "search1")
        //    {
        //        txtsearch1.Text = lblAll.Text.Trim();
        //    }
        //    else if (clickstatus == "search2")
        //    {
        //        txtsearch2.Text = lblAll.Text.Trim();
        //    }
        //    else if (clickstatus == "search3")
        //    {
        //        txtsearch3.Text = lblAll.Text.Trim();
        //    }

        //    pnlCalender.Visible = false;
        //}
       
        #endregion

        #region SalesBillTabPage
        




        public DataTable paymentDeno()
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
            paymentdo.Rows.Add("Coins", 0, 0.00);


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





     

        
        #endregion


        //public AutoCompleteStringCollection AutoCompleteLoad()
        //{
        //    AutoCompleteStringCollection str = new AutoCompleteStringCollection();
        //    DataTable st = objQuotationbal.itemauto();
        //    string[] arr = new string[st.Rows.Count];
        //    for (int i = 0; i < st.Rows.Count; i++)
        //    {
        //        arr[i] = st.Rows[i]["DisplayName"].ToString();
        //    }

        //    //for (int i = 0; i < arr.Length; i++)
        //    //{
        //    var combined = string.Join(", ", arr);
        //    //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
        //    str.Add(combined);
        //    //}

        //    //for (int i = 0; i < st.Rows.Count; i++)
        //    //{
        //    //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
        //    //    str.Add(combined);
        //    //}

        //    return str;
        //}
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
            }
            catch(Exception ex)
            {

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

            //for (int i = 0; i < arr.Length; i++)
            //{
            var combined = string.Join(", ", arr);
            //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            str.Add(combined);
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

        //private void SearchPurchaseOrder()
        //{

        //    dgvSearch.Rows.Clear();
        //    dgvSearch.Columns.Clear();
        //    dgvSearch.ColumnCount = 3;


        //    dgvSearch.Columns[0].Name = "Order No";
        //    dgvSearch.Columns[1].Name = "Customer Name";
        //    dgvSearch.Columns[2].Name = "Date";




        //    this.dgvSearch.Columns[0].Width = 60;

        //    this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


        //    this.dgvSearch.Columns[1].Width = 60;




        //    this.dgvSearch.Columns[2].Width = 60;






        //    dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //    dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    foreach (DataGridViewColumn c in dgvSearch.Columns)
        //    {
        //        c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
        //    }

        //    this.dgvSearch.Columns[1].Visible = false;
        //    this.dgvSearch.Columns[2].Visible = false;


        //    dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //    dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
        //    dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        //}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData==Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //searchsalebyid("");
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

        private void btnNew_Click(object sender, EventArgs e)
        {
          
        }

        public DataTable bindEstimation()
        {
            DataTable dt = new DataTable();
            dt = objQuotationbal.getSales();
            DataRow dr = dt.NewRow();
            dr["Salesid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);

            return dt;
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            //getsearchdata();
        }

      
        private void cbxSearchOrderNo_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void ListSearchDate1_Click_1(object sender, EventArgs e)
        {

        }

        private void pbxRightCollapse_Click(object sender, EventArgs e)
        {
            
        }

        
        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
               // searchsalebyid(Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value));
            }
        }

        private void Btnsubmit_Click(object sender, EventArgs e)
        {
           

        }

       
        private void label40_Click(object sender, EventArgs e)
        {
            
        }

       
            //dgvSearch.Columns["Assist"].Visible = false;
        
        public string getrack(string s, string s1)
        {
            string v = objQuotationbal.getrack(s, s1);
            return v;
        }

        private void dgvChecking_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

     

        private void SalesBillNew_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void dgvChecking_DataError_1(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

      
        private void btnsave_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnsearch_Click(object sender, EventArgs e)
        {

            try
            {
                searchWithDate();
            }
            catch(Exception ex)
            {
                dgvStockrpt.DataSource = null;
                LbelPipesTotal.Text = "0.00";
                lbltotalamount.Text = "0.00";
            }
        }


        public void search()
        {
           
            DataTable dt = objQuotationbal.GetSalesReport();
            //  Convert.ToDateTime(dt.Columns["Date"].ToString());
            //dt.Columns[5].DateTimeMode;// DateTime);
            dgvStockrpt.DataSource = dt;
            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);




            foreach (DataGridViewColumn c in dgvStockrpt.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvStockrpt.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvStockrpt.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            total();

        }
        public void total()
        {
            try
            {
                double totalamount = 0.0D, TotalPips = 0.0D;
                double value = 0.0, value1 = 0.0;
                for (int i = 0; i < dgvStockrpt.Rows.Count; i++)
                {

                    if (string.IsNullOrEmpty(Convert.ToString(dgvStockrpt.Rows[i].Cells[1].Value)))
                    {
                        value = 0.0;
                    }
                    else
                    {
                        value = Convert.ToDouble(dgvStockrpt.Rows[i].Cells[1].Value);
                    }


                    totalamount = totalamount + value;



                    if (string.IsNullOrEmpty(Convert.ToString(dgvStockrpt.Rows[i].Cells[2].Value)))
                    {
                        value1 = 0.0;
                    }
                    else
                    {
                        value1 = Convert.ToDouble(dgvStockrpt.Rows[i].Cells[2].Value);
                    }


                    TotalPips = TotalPips + value1;

                    //totalquantity = totalquantity + Convert.ToInt32(dgvNew.Rows[i].Cells[5].Value);
                }

                //  totalquantity = Math.Round(totalquantity);

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




                string[] strs = TotalPips.ToString().Split('.');
                if (strs.Length > 1)
                {
                    double num1 = Convert.ToDouble("0." + strs[1]);

                    if (num1 >= 0.5)
                    {
                        TotalPips = Math.Ceiling(TotalPips);
                    }
                    else
                    {
                        TotalPips = Math.Floor(TotalPips);
                    }

                }

                LbelPipesTotal.Text = String.Format("{0:00.00}", TotalPips);
                //lbltotalquantity.Text = Convert.ToString(totalquantity);
                lbltotalamount.Text = String.Format("{0:00.00}", totalamount);
               
            }
            catch
            {

            }
        }

        public void searchWithDate()
        {
            string datestatus = string.Empty;


            DateTime Fromdate = new DateTime(Frommtdate.Value.Year, Frommtdate.Value.Month, Frommtdate.Value.Day);
            DateTime Todate = new DateTime(Tomtdate.Value.Year, Tomtdate.Value.Month, Tomtdate.Value.Day);
            DataTable dt = objQuotationbal.GetSalesReportWithDate(Fromdate, Todate);
            //  Convert.ToDateTime(dt.Columns["Date"].ToString());
            //dt.Columns[5].DateTimeMode;// DateTime);
            dgvStockrpt.DataSource = dt;
            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);




            foreach (DataGridViewColumn c in dgvStockrpt.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvStockrpt.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvStockrpt.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            total();

        }
    }
}

