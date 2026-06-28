using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using InvBal;
using System.Data.SqlClient;
using System.Configuration;
namespace Inventory.Report
{
    public partial class Customerwiseledger : Form
    {
        QuotationBal objQuotationbal = new QuotationBal();
        int userid = 0;
        static string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        public Customerwiseledger()
        {
          
            InitializeComponent();
            userid = Convert.ToInt32(Program.userid);
          
            bindcustomer();
        }

        public void bindcustomer()
        {

            Txtcustomername.DataSource = objQuotationbal.Getcustomer();
            Txtcustomername.DisplayMember = "Name";
            Txtcustomername.ValueMember = "CustomerID";
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

        public void searchGSTWithDate()
        {
            string datestatus = string.Empty;
            DataSet dt = new DataSet();


            string CustomerName = Txtcustomername.SelectedValue.ToString();
          
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@CustomerId", CustomerName);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Proc_CustomerLedger";  
                cmd.Connection = con;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                con.Close();
                dgvStockrpt.DataSource = dt.Tables[0];
               cr.Text=dt.Tables[1].Rows[0]["cr"].ToString();
               dr.Text = dt.Tables[1].Rows[0]["dr"].ToString();
               cl.Text = dt.Tables[1].Rows[0]["cl"].ToString();
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
        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (Txtcustomername.Text == "--Select--")
            {
                i++;
                message = message + "* Please Select Customer Name" + "\n";
                if (i == 1)
                    this.ActiveControl = Txtcustomername;
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mandatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

        private void lbltotalamount_Click(object sender, EventArgs e)
        {

        }
    }
}
