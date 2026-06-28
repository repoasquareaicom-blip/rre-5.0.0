using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InvBal;
namespace Inventory.Report
{
    public partial class AccountHeadLedger : Form
    {
        string conn = Program.connection;
        StockReportBAL objStockReportBAL = new StockReportBAL();
        public AccountHeadLedger()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            AccountMainheade();
        }

        private void AccountHeadLedger_Load(object sender, EventArgs e)
        {

        }
        public void AccountMainheade()
        {
            comboAccountheadValue.DataSource = GetDataTableFromSQl("select MainHeadId, MeanHeadName as Value from AccountMailHeadMaster  order by MeanHeadName");
            comboAccountheadValue.ValueMember = "MainHeadId";
            comboAccountheadValue.DisplayMember = "Value";
        }

        private void radioCustomers_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCustomers.Checked == true)
            {
                comboAccountheadValue.DataSource = GetDataTableFromSQl("select CustomerID as Id, Name as Value from Customers where CustomerID in (select Customerid from QuotationEstimation where Paymentmode like'%credit%') order by Name");
                comboAccountheadValue.ValueMember = "Id";
                comboAccountheadValue.DisplayMember = "Value";
            }
        }

        private void radioSupliers_CheckedChanged(object sender, EventArgs e)
        {
            //if (radioSupliers.Checked == true)
            //{
            //    comboAccountheadValue.DataSource = GetDataTableFromSQl("select SuppliersID as id, Name as Value from Suppliers order by Name");
            //    comboAccountheadValue.ValueMember = "Id";
            //    comboAccountheadValue.DisplayMember = "Value";
            //}
        }

        private void radioAcHead_CheckedChanged(object sender, EventArgs e)
        {
            if (radioAcHead.Checked == true)
            {
                comboAccountheadValue.DataSource = GetDataTableFromSQl("select MainHeadId, MeanHeadName as Value from AccountMailHeadMaster  order by MeanHeadName");
                comboAccountheadValue.ValueMember = "MainHeadId";
                comboAccountheadValue.DisplayMember = "Value";
            }
        }

        private void radioRose_CheckedChanged(object sender, EventArgs e)
        {
            if (radioRose.Checked == true)
            {
                comboAccountheadValue.DataSource = GetDataTableFromSQl("select ReferencesID as id, Name as Value from [References]  order by Name");
                comboAccountheadValue.ValueMember = "Id";
                comboAccountheadValue.DisplayMember = "Value";
            }
        }


        public DataTable GetDataTableFromSQl(string QueryText)
        {
            DataTable dt = new DataTable();
            DataSet objDs = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            using (SqlConnection sqlconn = new SqlConnection(conn))
            using (SqlCommand cmd = sqlconn.CreateCommand())
            {
                cmd.CommandText = QueryText;
                cmd.CommandType = System.Data.CommandType.Text;
                sqlconn.Open();
                da = new SqlDataAdapter(cmd);
                da.Fill(objDs);
            }
            if (objDs.Tables.Count > 0)
            {
                dt = objDs.Tables[0];
            }
            return dt;
        }

        private void btnsearch_Click(object sender, EventArgs e)
        {
            DateTime Fromdates = new DateTime(dtpFromDate.Value.Year, dtpFromDate.Value.Month, dtpFromDate.Value.Day);
            DateTime Todates = new DateTime(dtpToDate.Value.Year, dtpToDate.Value.Month, dtpToDate.Value.Day);
            DataTable dt = objStockReportBAL.GetAccountHeaderreport(comboAccountheadValue.Text, Fromdates, Todates);
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
