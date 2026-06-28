using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Inventory.Adjustment
{
    public partial class StockAdjustmentView : Form
    {
        TextBox tb;
        PurchaseReceiptBAL ObjPurchaseReceiptBAL = new PurchaseReceiptBAL();
        public StockAdjustmentView()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.ActiveControl = cmbloaction;
            bindLocation();
            Loadstock();
        }

        public void bindLocation()
        {
            cmbloaction.DataSource = ObjPurchaseReceiptBAL.GetLocation();
            cmbloaction.DisplayMember = "LocationName";
            cmbloaction.ValueMember = "LocationID";
        }

        public void getstockcount()
        {
            lblout.Text = "0";
            double val = 0.00;
            for (int i = 0; i < dgvinvoice.Rows.Count;i++ )
            {
                val = val + (Convert.ToDouble(dgvinvoice.Rows[i].Cells["Stock"].Value));
            }
            lblout.Text = Convert.ToString(val);
        }

        private void Loadstock()
        {

            dgvinvoice.Rows.Clear();
            dgvinvoice.Columns.Clear();
            dgvinvoice.ColumnCount = 9;

            dgvinvoice.Columns[0].Name = "Sno";
            dgvinvoice.Columns[1].Name = "Product";
            dgvinvoice.Columns[2].Name = "Rack";
            dgvinvoice.Columns[3].Name = "Stock";
            dgvinvoice.Columns[4].Name = "Adjust";
            dgvinvoice.Columns[5].Name = "New Stock";
            dgvinvoice.Columns[6].Name = "Productid";
            dgvinvoice.Columns[7].Name = "Rate";
            dgvinvoice.Columns[8].Name = "TotalAmount";
            



            this.dgvinvoice.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[0].Width = 40;
            // this.dgvOrder.Columns[0].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[0].ReadOnly = true;

            this.dgvinvoice.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[1].Width = 350;
            // this.dgvOrder.Columns[0].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[1].ReadOnly = true;


            this.dgvinvoice.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[2].Width = 80;
            //this.dgvOrder.Columns[1].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            this.dgvinvoice.Columns[2].ReadOnly = true;


            this.dgvinvoice.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[3].Width = 90;
            //this.dgvOrder.Columns[2].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvinvoice.Columns[3].ReadOnly = true;

            this.dgvinvoice.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[4].Width = 90;
            //this.dgvOrder.Columns[3].DefaultCellStyle.BackColor = Color.Beige;
            this.dgvinvoice.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
           


            this.dgvinvoice.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[5].Width = 90;
            this.dgvinvoice.Columns[5].ReadOnly = true;
            this.dgvinvoice.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //this.dgvOrder.Columns[4].DefaultCellStyle.BackColor = Color.Beige;
            dgvinvoice.Columns[6].Visible = false;
            dgvinvoice.Columns[4].Visible = false; 
            dgvinvoice.Columns[5].Visible = false;

            this.dgvinvoice.Columns[6].ReadOnly = true;
            this.dgvinvoice.Columns[7].ReadOnly = true;
            this.dgvinvoice.Columns[8].ReadOnly = true;

            this.dgvinvoice.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[7].Width = 90;
            this.dgvinvoice.Columns[7].ReadOnly = true;
            this.dgvinvoice.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



            this.dgvinvoice.Columns[8].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvinvoice.Columns[8].Width = 90;
            this.dgvinvoice.Columns[8].ReadOnly = true;
            this.dgvinvoice.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;




            dgvinvoice.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvinvoice.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }
        }




        public void bindstock()
        {
            DataTable ds = ObjPurchaseReceiptBAL.Getstockadjustment(Convert.ToInt32(cmbloaction.SelectedValue));
            if (ds.Rows.Count > 0)
            {
                dgvinvoice.ReadOnly = true;
                dgvinvoice.Rows.Clear();
                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    dgvinvoice.Rows.Add();
                    dgvinvoice.Rows[i].Cells[0].Value = i + 1;
                    dgvinvoice.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["Displayname"]);
                    dgvinvoice.Rows[i].Cells[3].Value = Convert.ToString(ds.Rows[i]["qty"]);
                    dgvinvoice.Rows[i].Cells[6].Value = Convert.ToString(ds.Rows[i]["id"]);
                    dgvinvoice.Rows[i].Cells[2].Value = Convert.ToString(ds.Rows[i]["Rack"]);
                    dgvinvoice.Rows[i].Cells["Rate"].Value = Convert.ToString(ds.Rows[i]["Price"]);
                    dgvinvoice.Rows[i].Cells["TotalAmount"].Value = Convert.ToString(ds.Rows[i]["TotalAmount"]);


                }
            }
            else
            {
                dgvinvoice.Rows.Clear();
                dgvinvoice.ReadOnly = true;
            }
        }


        public void bindstockbyproduct()
        {
            DataTable ds = ObjPurchaseReceiptBAL.Getstockadjustmentbyproduct(Convert.ToInt32(cmbloaction.SelectedValue), txtprodsearch.Text);
            if (ds.Rows.Count > 0)
            {
                dgvinvoice.ReadOnly = true;
                dgvinvoice.Rows.Clear();
                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    dgvinvoice.Rows.Add();
                    dgvinvoice.Rows[i].Cells[0].Value = i + 1;
                    dgvinvoice.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["Displayname"]);
                    dgvinvoice.Rows[i].Cells[3].Value = Convert.ToString(ds.Rows[i]["qty"]);
                    dgvinvoice.Rows[i].Cells[6].Value = Convert.ToString(ds.Rows[i]["id"]);
                    dgvinvoice.Rows[i].Cells[2].Value = Convert.ToString(ds.Rows[i]["Rack"]);

                }

                getstockcount();
            }
            else
            {
                dgvinvoice.Rows.Clear();
                dgvinvoice.ReadOnly = true;
            }
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            if (cmbloaction.SelectedIndex > 0)
            {
                cmbloaction.Enabled = false;
                bindstock();
                dgvinvoice.Focus();
                getstockcount();
                //dgvinvoice.CurrentCell = dgvinvoice[4, 0];
                //dgvinvoice.BeginEdit(true);
                //Application.Idle += new EventHandler(Application_Idle);
            }
            else
            {
                MessageBox.Show("Please Select Location");
                cmbloaction.Focus();
            }
        }
        //public void Application_Idle(object sender, EventArgs e)
        //{
        //    try
        //    {
              
        //            dgvinvoice.CurrentCell = dgvinvoice[4, dgvinvoice.CurrentCell.RowIndex];
             
       
        //    }
        //    catch { }
        //}

        private void dgvinvoice_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 4)
                {
                    double stock = Convert.ToDouble(dgvinvoice.Rows[e.RowIndex].Cells["Stock"].Value);
                    double less = Convert.ToDouble(dgvinvoice.Rows[e.RowIndex].Cells["Adjust"].Value);
                    double val = stock + less;
                    dgvinvoice.Rows[e.RowIndex].Cells["New Stock"].Value = val;
                }
            }
            catch
            {

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Pnloading.Visible = true;
            DataTable dt = DataGridView2DataTable(dgvinvoice);
            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            dt.Columns.RemoveAt(0);
            dt.Columns.RemoveAt(1);
            RemoveNullColumnFromDataTable(dt);
            if (dt.Rows.Count <= 0)
            {
                Pnloading.Visible = false;
                MessageBox.Show("Plase Enter Correct Product");
            }
            else
            {
                int s = ObjPurchaseReceiptBAL.savestockadjustment(Convert.ToString(cmbloaction.SelectedValue), Convert.ToInt32(Program.userid), dt);
                if (s == 1)
                {
                    Pnloading.Visible = false;
                    MessageBox.Show("Updated Succesfully");
                    clear();
                }
                else
                {
                    Pnloading.Visible = false;
                    MessageBox.Show("Update Failed");
                }
            }
        }

        public void RemoveNullColumnFromDataTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {


                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][0])) || (Convert.ToString(dt.Rows[i][0])=="."))
                        dt.Rows[i].Delete();
               
            }
            dt.AcceptChanges();
        }

        public DataTable DataGridView2DataTable(DataGridView dgv, int minRow = 0)
        {

            DataTable dt = new DataTable();

            // Header columns
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                DataColumn dc = new DataColumn(column.Name.ToString());
                dt.Columns.Add(dc);
            }

            // Data cells
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                DataGridViewRow row = dgv.Rows[i];
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    dr[j] = (row.Cells[j].Value == null) ? "" : row.Cells[j].Value.ToString();
                }
                dt.Rows.Add(dr);
            }

            // Related to the bug arround min size when using ExcelLibrary for export
            for (int i = dgv.Rows.Count; i < minRow; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dr[j] = "  ";
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private void Btnclear_Click(object sender, EventArgs e)
        {
            clear();
        }
        public void clear()
        {
            cmbloaction.Enabled = true;
            dgvinvoice.Rows.Clear();
            cmbloaction.SelectedIndex = 0;
            dgvinvoice.ReadOnly = true;
            lblout.Text = "0";
        }

        private void dgvinvoice_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvinvoice.CurrentCell.ColumnIndex;
            string headerText = dgvinvoice.Columns[column].HeaderText;

            if (headerText.Equals("Adjust"))
            {
                tb = e.Control as TextBox;


                if (tb != null)
                {
                    
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.MaxLength = 10;
                }
            }
        }

        Regex reg = new Regex(@"^-?\d+[.]?\d*$");
        Regex reg1 = new Regex(@"^-?[.]?\d*$");
        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {

            try
            {
                if (char.IsControl(e.KeyChar)) return;
                if ((reg.IsMatch(tb.Text.Insert(tb.SelectionStart, e.KeyChar.ToString()) + "1")) || reg1.IsMatch(tb.Text.Insert(tb.SelectionStart, e.KeyChar.ToString()) + "1"))
                {

                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {

            }
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData==Keys.F3)
            {
                pnlprodsearch.Visible = true;
                txtprodsearch.SelectionStart = 0;
                txtprodsearch.SelectionLength = txtprodsearch.Text.Length;
                txtprodsearch.Focus();
                return true;
            }

            if(keyData==Keys.Escape)
            {
                if (pnlprodsearch.Visible)
                {
                     pnlprodsearch.Visible = false;
                     return true;
                }
                else
                {
                    this.Close();
                    return true;
                   
                }
            }

            if (txtprodsearch.Focused)
            {
                if (keyData == (Keys.Enter))
                {
                    if (!string.IsNullOrEmpty(txtprodsearch.Text))
                    {
                        if (cmbloaction.SelectedIndex>0)
                        {
                        bindstockbyproduct();
                        }
                        else
                        {
                            MessageBox.Show("Please Select Location");
                        }
                    }
                }
            }
           
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
        }

    }
}
