using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InvBal;
using System.Runtime.InteropServices;
namespace Inventory.Report
{
    public partial class FrmStockFinalRpt : Form
    {
        public FrmStockFinalRpt()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            txtproductname.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtproductname.AutoCompleteCustomSource = AutoCompleteLoad();
            txtproductname.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //Bind(null);
           
        }
        public AutoCompleteStringCollection AutoCompleteLoad()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = ProductMovementBal.itemauto();
            string[] arr = new string[st.Rows.Count];
            for (int i = 0; i < st.Rows.Count; i++)
            {
                arr[i] = st.Rows[i]["DisplayName"].ToString();
            }
            for (int i = 0; i < arr.Length; i++)
            {
                //var combined = string.Join(", ", arr);
                var combined = arr[i];
                str.Add(combined);
            }
            //for (int i = 0; i < arr.Length; i++)
            //{
            //  var combined = string.Join(", ", arr);
            //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //   str.Add(combined);
            //}

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}

            return str;
        }

        string category = null;
        string product = null;
        private void btnsearch_Click(object sender, EventArgs e)
        {

          

            if (!string.IsNullOrEmpty(txtproductname.Text))
            {
                product = txtproductname.Text;
            }
            else
            {
                product = null;
            }



            Bind(product);
        }
        public void Bind(string product)
        {
            dgvStockrpt.DataSource = StockReportBAL.GetStockReportFinal(product);
           dgvStockrpt.Columns["ProductId"].Visible = false;

           dgvStockrpt.Columns["SNo"].Width = 50;
           dgvStockrpt.Columns["Price"].Width = 70;
           dgvStockrpt.Columns["PDate"].Width = 90;
           dgvStockrpt.Columns["SDate"].Width = 90;

            //dgvStockrpt.Columns["ItemCode"].Width = 100; dgvStockrpt.Columns["Sno"].Width = 50;

             Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;
            if (w == 1024 && h == 768)
            {

                dgvStockrpt.Columns["ProductName"].Width = 350;

                
            }
            else
            {
                dgvStockrpt.Columns["ProductName"].Width = 450;
            }

            //dgvStockrpt.Columns["Delivery"].Width = 80;
            //dgvStockrpt.Columns["Checking"].Width = 85;


            this.dgvStockrpt.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            
            dgvStockrpt.Columns["CHECKING"].Visible = false;
            dgvStockrpt.Columns["DELIVER"].Visible = false;
            dgvStockrpt.Columns["F"].Visible = false;
            dgvStockrpt.Columns["S"].Visible = false;
            dgvStockrpt.Columns["T"].Visible = false;
            dgvStockrpt.Columns["M"].Visible = false;
            dgvStockrpt.Columns["I"].Visible = false;
            dgvStockrpt.Columns["GL"].Visible = false;
            dgvStockrpt.Columns["FL"].Visible = false;
            dgvStockrpt.Columns["SL"].Visible = false;
            dgvStockrpt.Columns["TL"].Visible = false;
            dgvStockrpt.Columns["PG"].Visible = false;
            dgvStockrpt.Columns["P"].Visible = false;
            dgvStockrpt.Columns["MD"].Visible = false;
            dgvStockrpt.Columns["RETURN_CHECK"].Visible = false;
            //this.dgvStockrpt.Columns["Damage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dgvStockrpt.Columns["Display"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dgvStockrpt.Columns["Delivery"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dgvStockrpt.Columns["Checking"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

          [DllImport("user32.dll")]
        internal static extern IntPtr GetFocus();
          protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
          {
              if (keyData == Keys.Enter)
              {

                  IntPtr wndHandle = GetFocus();
                  Control focusedControl = FromChildHandle(wndHandle);
                  if (focusedControl.Name == "textBox1")
                  {
                      try
                      {
                          int text = Convert.ToInt32(textBox1.Text);
                          foreach (DataGridViewRow row in dgvStockrpt.Rows)
                          {
                              if (Convert.ToInt32(row.Cells["Aging"].Value) > text)

                                  row.DefaultCellStyle.BackColor = Color.Red;

                              else
                                  row.DefaultCellStyle.BackColor = Color.White;

                          }
                      }
                      catch
                      {

                      }
                  }
              }


              if (keyData == Keys.Escape)
              {
                  this.Close();
                  return true;
              }

              return base.ProcessCmdKey(ref msg, keyData);
          }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
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
    }
}
