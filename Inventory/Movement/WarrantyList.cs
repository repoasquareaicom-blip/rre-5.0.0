using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace Inventory.Commission
{
    public partial class WarrantyList : Form
    {
        QuotationBal objQuotationBal = new QuotationBal();
      
        string amountmode = string.Empty;
        public WarrantyList()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            pnlLabelSearch.Visible = true;
            vLabel1.Visible = true;
            pnlSearch.Visible = false;
            splitContainer1.Panel1Collapsed = true;
            bindgrid();
        }
       

        public void bindgrid()
        {
            DateTime Fromdate = new DateTime(Frommtdate.Value.Year, Frommtdate.Value.Month, Frommtdate.Value.Day);
            DateTime Todate = new DateTime(Tomtdate.Value.Year, Tomtdate.Value.Month, Tomtdate.Value.Day);
            DataTable dt = objQuotationBal.getwarrantylist(Fromdate, Todate);

            dgvStockrpt.DataSource = dt;
            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);



            foreach (DataGridViewColumn c in dgvStockrpt.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvStockrpt.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvStockrpt.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvStockrpt.Columns[0].Width = 100;
            //dgvStockrpt.Columns[1].Width = 300;
            //dgvStockrpt.Columns[2].Width = 100;
            //dgvStockrpt.Columns[3].Width = 100;
            //dgvStockrpt.Columns[3].Width = 100;

        }

        private void btnsech_Click(object sender, EventArgs e)
        {
            bindgrid();
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            dgvStockrpt.DataSource = null;
        }

        private void dgvStockrpt_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvStockrpt.Columns[dgvStockrpt.CurrentCell.ColumnIndex].HeaderText == "CustomerName")
                {
                    if (e.RowIndex >= 0)
                    {
                        var coordinates = this.PointToClient(Cursor.Position);
                        int x = coordinates.X;
                        int y = coordinates.Y;
                        x = x - 200;
                        y = y - 100;
                        panelIn.Visible = false;
                        dgvInBills.DataSource = null;
                        string productname = Convert.ToString(dgvStockrpt.Rows[dgvStockrpt.CurrentCell.RowIndex].Cells["ReferenceNo"].Value);
                        dgvInBills.DataSource = StockReportBAL.getwarrentylistitem(productname);

                        dgvInBills.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);



                        foreach (DataGridViewColumn c in dgvInBills.Columns)
                        {
                            c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
                        }

                        dgvInBills.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                        dgvInBills.DefaultCellStyle.BackColor = Color.Gainsboro;
                        dgvInBills.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
                        panelIn.Visible = true;
                        panelIn.Location = new Point(x, y);
                    }
                }
            }
            catch(Exception ex)
            {

            }
           
        }

        private void InClose_Click(object sender, EventArgs e)
        {
            panelIn.Visible = false;
            dgvInBills.DataSource = null;
        }
    
    }
}





















































