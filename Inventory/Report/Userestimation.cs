using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Report
{
    public partial class Userestimation : Form
    {
        UserCreationBAL ObjUserCreationBAL = new UserCreationBAL();
        QuotationBal objQuotationbal = new QuotationBal();
        public Userestimation()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            Getuser();
            lblbill.Text = "Total Bill:" +"0";
            lblamount.Text = "Total Amount:" + "0";
        }

        public void Getuser()
        {
            ddlRole.DataSource = null;
            DataTable dt = UserCreationBAL.GetUserCreation();
            DataRow ds = dt.NewRow();
            ds["UserId"] = "0";
            ds["UserName"] = "--Select--";
            dt.Rows.InsertAt(ds, 0);
            ddlRole.DataSource = dt;
            ddlRole.DisplayMember = "UserName";
            ddlRole.ValueMember = "UserId";


        }


        public void getemployee()
        {
            ddlRole.DataSource = null;
            DataTable dt = objQuotationbal.getemployee();
            DataRow ds = dt.NewRow();
            ds["Employeeid"] = "0";
            ds["Name"] = "--Select--";
            dt.Rows.InsertAt(ds, 0);
            ddlRole.DataSource = dt;
            ddlRole.DisplayMember = "Name";
            ddlRole.ValueMember = "Employeeid";


        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchEstimationuserWithDate();
        }

        private void searchEstimationuserWithDate()
        {
            string status = string.Empty;

            if (radioButton1.Checked)
            {
                status = radioButton1.Text;
            }
            if (radioButton2.Checked)
            {
                status = radioButton2.Text;
            }

            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);

            DataTable dt = objQuotationbal.searchEstimationuserWithDate(FromDate, ToDate, Convert.ToString(ddlRole.SelectedValue), status);
            dgvSearch.DataSource = null;
            dgvSearch.DataSource = dt;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvSearch.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            getamount();
            lblbill.Text = "Total Bill:" + Convert.ToString(dgvSearch.Rows.Count);
        }


        public void getamount()
        {
            double value = 0.00;
            for (int i = 0; i < dgvSearch.Rows.Count;i++ )
            {
                value = value + Convert.ToDouble(dgvSearch.Rows[i].Cells["Amount"].Value);
            }

            lblamount.Text = "Total Amount:" + Convert.ToString(value);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            getemployee();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Getuser();
        }


    }
}
