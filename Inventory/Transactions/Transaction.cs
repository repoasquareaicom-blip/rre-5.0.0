using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventory.Transactions
{
    public partial class Transaction : Form
    {
        public Transaction()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            LoadCRA();
            LoadCRP();
            LoadDenomination();
        }

        private void LoadCRA()
        {
            dgvCRA.Rows.Clear();
            dgvCRA.Columns.Clear();
            dgvCRA.RowCount = 4;
            dgvCRA.ColumnCount = 4;

            dgvCRA.Columns[0].Name = "Date";
            dgvCRA.Columns[1].Name = "Requested By";
            dgvCRA.Columns[2].Name = "Reason";
            dgvCRA.Columns[3].Name = "Amount";
            
            this.dgvCRA.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRA.Columns[0].Width = 150;         
            this.dgvCRA.Columns[0].ReadOnly = true;
            
            this.dgvCRA.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRA.Columns[1].Width = 180;            
            this.dgvCRA.Columns[1].ReadOnly = true;

            this.dgvCRA.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRA.Columns[2].Width = 450;
            this.dgvCRA.Columns[2].ReadOnly = true;

            this.dgvCRA.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRA.Columns[3].Width = 130;
            this.dgvCRA.Columns[3].ReadOnly = true;
            this.dgvCRA.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
            cmb.HeaderText = "";
            cmb.Name = "cmb";
            cmb.MaxDropDownItems = 4;
            cmb.Items.Add("Approve");
            cmb.Items.Add("Reject");
            dgvCRA.Columns.Add(cmb);
           
            this.dgvCRA.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRA.Columns[4].Width = 110;

            //this.dgvCRA.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvCRA.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvCRA.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvCRA.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvCRA.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            
            dgvCRA.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvCRA.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }

        private void LoadCRP()
        {
            dgvCRP.Rows.Clear();
            dgvCRP.Columns.Clear();
            dgvCRP.RowCount = 6;
            dgvCRP.ColumnCount = 6;

            dgvCRP.Columns[0].Name = "Date";
            dgvCRP.Columns[1].Name = "Requested By";
            dgvCRP.Columns[2].Name = "Reason";
            dgvCRP.Columns[3].Name = "Amount";
            dgvCRP.Columns[4].Name = "Approved By";
            dgvCRP.Columns[5].Name = "Approved On";

            this.dgvCRP.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[0].Width = 150;
            this.dgvCRP.Columns[0].ReadOnly = true;

            this.dgvCRP.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[1].Width = 150;
            this.dgvCRP.Columns[1].ReadOnly = true;

            this.dgvCRP.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[2].Width = 350;
            this.dgvCRP.Columns[2].ReadOnly = true;

            this.dgvCRP.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[3].Width = 100;
            this.dgvCRP.Columns[3].ReadOnly = true;
            this.dgvCRP.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvCRP.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[4].Width = 100;
            this.dgvCRP.Columns[4].ReadOnly = true;

            this.dgvCRP.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[5].Width = 130;
            this.dgvCRP.Columns[5].ReadOnly = true;

            DataGridViewButtonColumn btn = new DataGridViewButtonColumn();

            btn.Text = "Pay Cash";
            btn.HeaderText = "PayCash";
            btn.Name = "PayCash";
            btn.UseColumnTextForButtonValue = true;
            btn.AutoSizeMode =
            DataGridViewAutoSizeColumnMode.AllCells;
            btn.FlatStyle = FlatStyle.Standard;
            btn.CellTemplate.Style.BackColor = Color.Honeydew;
            dgvCRP.Columns.Add(btn);

            this.dgvCRP.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCRP.Columns[4].Width = 110;

            dgvCRP.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvCRP.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }

        private void LoadDenomination()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Denomination",typeof(string));
            dt.Columns.Add("NoOfCurrency", typeof(string));
            dt.Rows.Add("2000", "");
            dt.Rows.Add("1000","");
            dt.Rows.Add("500", "");
            dt.Rows.Add("100", "");
            dt.Rows.Add("50", "");
            dt.Rows.Add("20", "");
            dt.Rows.Add("10", "");
            dt.Rows.Add("5", "");
            dt.Rows.Add("Coins", "");
            dgvDenomination.DataSource = dt;
            this.dgvDenomination.Columns[0].ReadOnly = true;

            //this.dgvCRP.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvCRP.Columns[0].Width = 150;
            //

            //this.dgvCRP.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvCRP.Columns[1].Width = 150;
            
            //dgvCRP.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            //foreach (DataGridViewColumn c in dgvCRP.Columns)
            //{
            //    c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            //}
        }

        private void dgvCRA_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                e.Value = "Approve";
            }
        }

        private void dgvCRP_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6)
            {
                a1Panel1.Visible = true;
            }
        }

        private void a1Panel1_Click(object sender, EventArgs e)
        {
            a1Panel1.Visible = false;
            a1Panel2.Visible = false;
        }

        private void rbCash_CheckedChanged(object sender, EventArgs e)
        {
            if(rbCash.Checked)
            {
                a1Panel2.Visible = true;
                
            }
            else
            {
                a1Panel2.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Validation())
            {

            }
      }
        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(txtrequest.Text))
            {
                i++;
                message = message + "*Enter request" + "\n";
                if (i == 1)
                    this.ActiveControl = txtrequest;
            }
            if (string.IsNullOrEmpty(txtamount.Text))
            {
                i++;
                message = message + "*Enter  Amount" + "\n";
                if (i == 1)
                    this.ActiveControl = txtamount;
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message);
                status = false;
            }
            return status;
        }

        private void txtamount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtamount1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtamountrupe_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)// save au bending
        {
            if (Validation())
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtamount.Clear();
            txtrequest.Clear();
            txtreson.Clear();
        }

        private void Transaction_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtrequest;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (btnprint.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = txtrequest;
                    return true;
                }

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
