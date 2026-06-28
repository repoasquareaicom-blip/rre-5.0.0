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
    public partial class EstimationAging : Form
    {
        QuotationBal objQuotationbal = new QuotationBal();
        public EstimationAging()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            GetSearchSalesOrder();
            total();
            this.ActiveControl = dtfromdate;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            GetSearchSalesOrder();
            total();
        }

        public void total()
        {
            double totalamount = 0.0D;
            double value = 0.0;
            for (int i = 0; i < dgvSearch.Rows.Count; i++)
            {

                if (string.IsNullOrEmpty(Convert.ToString(dgvSearch.Rows[i].Cells["Amount"].Value)))
                {
                    value = 0.0;
                }
                else
                {
                    value = Convert.ToDouble(dgvSearch.Rows[i].Cells["Amount"].Value);
                }


                totalamount = totalamount + value;
                //totalquantity = totalquantity + Convert.ToInt32(dgvNew.Rows[i].Cells[5].Value);
            }

            //lbltotalquantity.Text = Convert.ToString(totalquantity);
            lbltotalamount.Text = String.Format("{0:0.00}", totalamount);

        }
        private void GetSearchSalesOrder()
        {
            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);

            DataTable dt = objQuotationbal.EstimationAging(FromDate, ToDate);
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


        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                DialogResult result = MessageBox.Show("Do you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                try
                {
                    if (result == DialogResult.Yes)
                    {
                        this.Dispose();

                    }
                }
                catch
                {
                    this.Dispose();
                }
            }

            if(keyData==Keys.Tab)
            {
                if (dtfromdate.Focused)
                {
                    DTPTodate.Focus();
                }
                else if (DTPTodate.Focused)
                {
                    btnSearch.Focus();
                }
                else if (btnSearch.Focused)
                {
                    dtfromdate.Focus();
                }
                return true;
            }




            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
