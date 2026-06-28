using System;
using InvBal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Report_Transaction
{
    public partial class CashTransaction : Form
    {
        TransactionBAL objTransactionBal = new TransactionBAL();
        public CashTransaction()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void btnsearch_Click(object sender, EventArgs e)
        {
            search();
        }
        public void search()
        {

            try
            {
                string datestatus = string.Empty;


                DateTime Fromdate = new DateTime(dtpFromDate.Value.Year, dtpFromDate.Value.Month, dtpFromDate.Value.Day);
                DateTime Todate = new DateTime(dtpToDate.Value.Year, dtpToDate.Value.Month, dtpToDate.Value.Day);
                DataSet dt = objTransactionBal.GetCashTransactionReport(Fromdate, Todate);
                //  Convert.ToDateTime(dt.Columns["Date"].ToString());
                //dt.Columns[5].DateTimeMode;// DateTime);
                DataTable ds = dt.Tables[0];
                dgvStockrpt.DataSource = ds;
                dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);

                dgvStockrpt.Columns["Db"].HeaderText = "Dr";

                dgvStockrpt.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvStockrpt.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvStockrpt.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                foreach (DataGridViewColumn c in dgvStockrpt.Columns)
                {
                    c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
                }

                dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                dgvStockrpt.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvStockrpt.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

                this.dgvStockrpt.Columns[0].Width = 100;
                this.dgvStockrpt.Columns[1].Width = 700;
                this.dgvStockrpt.Columns[2].Width = 200;
                this.dgvStockrpt.Columns[3].Width = 200;
                this.dgvStockrpt.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dgvStockrpt.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvStockrpt.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvStockrpt.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvStockrpt.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvStockrpt.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

                if (dt.Tables[1].Rows.Count > 0)
                {
                    Debit.Text = Convert.ToString(dt.Tables[1].Rows[0]["Dr"].ToString());
                    Credit.Text = Convert.ToString(dt.Tables[1].Rows[0]["cr"].ToString());
                    Closing.Text = Convert.ToString(dt.Tables[1].Rows[0]["cb"].ToString());
                }
                else
                {
                    Debit.Text = "0.00";
                    Credit.Text = "0.00";
                    Closing.Text = "0.00";
                }
            }
            catch(Exception ex)
            {
                dgvStockrpt.DataSource = null;
                Debit.Text = "0.00";
                Credit.Text = "0.00";
                Closing.Text = "0.00";
            }
         

        }

        private void CashTransaction_Load(object sender, EventArgs e)
        {

        }

        private void Closing_Click(object sender, EventArgs e)
        {

        }

        private void Credit_Click(object sender, EventArgs e)
        {

        }
    }
}
