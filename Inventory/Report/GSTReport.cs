using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Report
{
    public partial class GSTReport : Form
    {
        static string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        QuotationBal objQuotationbal = new QuotationBal();
        public GSTReport()
        {
            InitializeComponent();
            Bindcompany();
        }

        private void btnsearch_Click(object sender, EventArgs e)
        {
            try
            {
                bool val = Validation();
                if (val)
                {
                    searchGSTWithDate();
                }
            }
            catch (Exception ex)
            {
                dgvStockrpt.DataSource = null;

            }
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

        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (cmbcompanychange.Text == "--Select--")
            {
                i++;
                message = message + "* Please Select Company Name" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbcompanychange;
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mandatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }
        public void searchGSTWithDate()
        {
            string datestatus = string.Empty;
            DataTable dt = new DataTable();

            DateTime Fromdate = new DateTime(Frommtdate.Value.Year, Frommtdate.Value.Month, Frommtdate.Value.Day);
            DateTime Todate = new DateTime(Tomtdate.Value.Year, Tomtdate.Value.Month, Tomtdate.Value.Day);
            string CompanyName = cmbcompanychange.SelectedValue.ToString();

            //  DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();


                // SqlCommand cmd = new SqlCommand("SalesBillPrint", con);
                SqlCommand cmd = new SqlCommand();
                //cmd.CommandType = CommandType.StoredProcedure;
                //SqlDataAdapter ad = new SqlDataAdapter(cmd);
                cmd.Parameters.AddWithValue("@fromdate", Fromdate);
                cmd.Parameters.AddWithValue("@todate", Todate);
                cmd.Parameters.AddWithValue("@company", CompanyName);
                cmd.CommandType = CommandType.StoredProcedure;

                if (CompanyName == "R.R. ELECTRICAL AGENCIES")
                {
                    cmd.CommandText = "GetCGSTReport";
                }
                if (CompanyName == "R.R. PIPES")
                {
                    cmd.CommandText = "GetPipesCGSTReport";
                }
                cmd.Connection = con;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                con.Close();




                dgvStockrpt.DataSource = dt;
                dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);




                foreach (DataGridViewColumn c in dgvStockrpt.Columns)
                {
                    c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
                }
               
                dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                dgvStockrpt.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvStockrpt.AlternatingRowsDefaultCellStyle.BackColor = Color.White;



            }

        }
    }
}
