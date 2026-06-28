using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Inventory.Tool
{
    public partial class UploadHistory : Form
    {
        public UploadHistory()
        {
            InitializeComponent();
        }

        private void UploadHistory_Load(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
            DataTable rsData1 = new DataTable();
            rsData1 = CashEndCloseBAL.StockUpload_History();
            dataGridView1.DataSource = rsData1;
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {





            if (keyData == (Keys.F3))
            {
                pnlprodsearch.Visible = true;
                txtprodsearch.SelectionStart = 0;
                txtprodsearch.SelectionLength = txtprodsearch.Text.Length;
                txtprodsearch.Text = "";
                txtprodsearch.Focus();
                return true;
            }

            if (txtprodsearch.Focused)
            {
                if (keyData == (Keys.Enter))
                {
                    if (!string.IsNullOrEmpty(txtprodsearch.Text))
                    {
                        GetSuppliersearch();
                        pnlprodsearch.Visible = false;
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void GetSuppliersearch()
        {
            DataTable dt = CashEndCloseBAL.GetHistory_StockUpload(txtprodsearch.Text);
            dataGridView1.DataSource = dt;
        }

        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
        }
    }
}
