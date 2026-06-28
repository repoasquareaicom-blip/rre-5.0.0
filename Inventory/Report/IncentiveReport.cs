using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Report
{
    public partial class IncentiveReport : Form
    {
        UserCreationBAL ObjUserCreationBAL = new UserCreationBAL();
        QuotationBal objQuotationbal = new QuotationBal();
        public IncentiveReport()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            Getuser();
        }

        private void btnsearch_Click(object sender, EventArgs e)
        {
        //    if (ddlRole.SelectedValue == 0)
        //    { 

        //    }
        //    else
        //    {
            IncentiveReports();
                total();
            //}
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

        public void total()
        {
            double totalamount = 0.0D;
            double value = 0.0;
            for (int i = 0; i < dgvStockrpt.Rows.Count; i++)
            {

                if (string.IsNullOrEmpty(Convert.ToString(dgvStockrpt.Rows[i].Cells["Total Amount"].Value)))
                {
                    value = 0.0;
                }
                else
                {
                    value = Convert.ToDouble(dgvStockrpt.Rows[i].Cells["Total Amount"].Value);
                }


                totalamount = totalamount + value;
                //totalquantity = totalquantity + Convert.ToInt32(dgvNew.Rows[i].Cells[5].Value);
            }

            //lbltotalquantity.Text = Convert.ToString(totalquantity);
            lbltotalamount.Text = String.Format("{0:0.00}", totalamount);

        }
        public void IncentiveReports()
        {

            try
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
                if (radioButton3.Checked)
                {
                    status = radioButton3.Text;
                }
                string Onlyincentive = string.Empty;
                string name = string.Empty;
                string userName = string.Empty;
                DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
                DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);
                if (chkOnlyincentive.Checked)
                {
                    Onlyincentive = "true";
                }
                else
                {
                    Onlyincentive = "false";

                }
                if (!string.IsNullOrEmpty(txtname.Text))
                {
                    name = txtname.Text;
                }
                else
                {
                    name = null;
                }
                string xx = Convert.ToString(ddlRole.SelectedIndex);
                if (xx != "0")
                {
                    userName = Convert.ToString(ddlRole.SelectedValue);
                }
                else
                {
                    userName = null;

                }

                dgvStockrpt.DataSource = StockReportBAL.Getincentive(FromDate, ToDate, Onlyincentive, name, userName, status);
                dgvStockrpt.Columns["Product"].Width = 250;
                dgvStockrpt.Columns["Quantity"].Width = 70;
                dgvStockrpt.Columns["Customer Name"].Width = 150;
                ////dgvStockrpt.Columns["Helper Name"].Width = 100;

                //if (dgvStockrpt.Columns["Helper Name"].ToString() == "Helper Name")
                //{
                //    dgvStockrpt.Columns["Helper Name"].Width = 100;
                //}

                dgvStockrpt.Columns["Incentive"].Width = 80;
                dgvStockrpt.Columns["UserName"].Visible = false;

                dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                dgvStockrpt.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvStockrpt.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
                dgvStockrpt.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            }
            catch(Exception ex)
            {
                dgvStockrpt.DataSource = null;
            }
        }
            //if (dtfromdate.Value > DTPTodate.Value)
            //{
            //    MessageBox.Show("*Expected delivery date should not be lesser than  order date");
               
                  
            //}

            //else
            //{
              

       // }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            getemployee();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

           
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            Getuser();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            getemployee();
        }
       

     
    }
}
