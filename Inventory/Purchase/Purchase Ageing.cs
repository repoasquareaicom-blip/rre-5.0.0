using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Purchase
{
    public partial class Purchase_Ageing : Form
    {
        PurchaseOrderBAL OblPurchaseOrderBAL = new PurchaseOrderBAL();
        QuotationBal objQuotationbal = new QuotationBal();
        public Purchase_Ageing()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            txtvendor.AutoCompleteCustomSource = Autovendor();
            txtvendor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtvendor.AutoCompleteSource = AutoCompleteSource.CustomSource;
            this.ActiveControl = txtvendor;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            GetSearchSalesOrder();
        }


        public AutoCompleteStringCollection Autovendor()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = OblPurchaseOrderBAL.GetSuppliers(null);
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
            return str;
        }

        private void GetSearchSalesOrder()
        {
            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);

            string Vendorid;
            if (string.IsNullOrEmpty(txtvendor.Text))
            {
                Vendorid = null;
            }
            else
            {
                Vendorid = txtvendor.Text;
            }

            DataTable dt = objQuotationbal.Proc_SearchPurchaseReceiptval(FromDate, ToDate, Vendorid);
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
         


            dgvSearch.Columns["ProductName"].Width = 400;
            dgvSearch.Columns["Status"].Width = 150;
            dgvSearch.Columns["OrderNo"].Width = 120;

            dgvSearch.Columns["OrderedQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSearch.Columns["Receipt"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSearch.Columns["Checking"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSearch.Columns["FloorCheckIn"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSearch.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

           // dgvSearch.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSearch.Columns["Date"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

            if (keyData == Keys.Tab)
            {
                if (txtvendor.Focused)
                {
                    dtfromdate.Focus();
                }
               else if (dtfromdate.Focused)
                {
                    DTPTodate.Focus();
                }
                else if (DTPTodate.Focused)
                {
                    btnSearch.Focus();
                }
                else if (btnSearch.Focused)
                {
                    txtvendor.Focus();
                }
                return true;
            }




            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
