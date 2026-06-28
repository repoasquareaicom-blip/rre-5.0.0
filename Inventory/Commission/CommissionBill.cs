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
    public partial class CommissionBill : Form
    {
        
        QuotationBal objQuotationBal = new QuotationBal();
        DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
        string amountmode = string.Empty;
        public CommissionBill()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();

            amountmode = "Percentage";
            LoadPortsChecking();
            pnlLabelSearch.Visible = true;
            vLabel1.Visible = true;
            pnlSearch.Visible = false;
            splitContainer1.Panel1Collapsed = true;

            bindregfernce();
            referenceperson.SelectedIndex = 0;
            bindgrid();
        }
        //dgvCommissionBill
        public Button btn;

        private void LoadPortsChecking()
        {
            dgvCommissionBill.Rows.Clear();
            dgvCommissionBill.ColumnCount = 6;


            dgvCommissionBill.Columns[0].Name = "S.NO";
            dgvCommissionBill.Columns[1].Name = "Estimationid";

            dgvCommissionBill.Columns[2].Name = "Date";
            dgvCommissionBill.Columns[5].Name = "Reference Name";

            dgvCommissionBill.Columns[3].Name = "Amount";
            dgvCommissionBill.Columns[4].Name = "Balance";

            this.dgvCommissionBill.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            this.dgvCommissionBill.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCommissionBill.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCommissionBill.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCommissionBill.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvCommissionBill.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvCommissionBill.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvCommissionBill.Columns[0].ReadOnly = true;
            this.dgvCommissionBill.Columns[1].ReadOnly = true;
            this.dgvCommissionBill.Columns[3].ReadOnly = true;
            this.dgvCommissionBill.Columns[4].ReadOnly = true;
            this.dgvCommissionBill.Columns[5].ReadOnly = true;
            this.dgvCommissionBill.Columns[2].ReadOnly = true;


            this.dgvCommissionBill.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            this.dgvCommissionBill.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "ChkIsCheckOut";
            dgvCmb.HeaderText = "Action";
            dgvCmb.FlatStyle = FlatStyle.Popup;
            dgvCommissionBill.Columns.Insert(6, dgvCmb);



            foreach (DataGridViewColumn c in dgvCommissionBill.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvCommissionBill.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvCommissionBill.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvCommissionBill.AlternatingRowsDefaultCellStyle.BackColor = Color.White;



            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {

                this.dgvCommissionBill.Columns[0].Width = 15;
                this.dgvCommissionBill.Columns[1].Width = 30;
                this.dgvCommissionBill.Columns[2].Width = 30;
                this.dgvCommissionBill.Columns[3].Width = 50;
                this.dgvCommissionBill.Columns[4].Width = 60;
                this.dgvCommissionBill.Columns[5].Width = 60;
                this.dgvCommissionBill.Columns[6].Width = 100;
            }
            else
            {
                this.dgvCommissionBill.Columns[0].Width = 20;
                this.dgvCommissionBill.Columns[1].Width = 80;
                this.dgvCommissionBill.Columns[2].Width = 80;
                this.dgvCommissionBill.Columns[3].Width = 60;
                this.dgvCommissionBill.Columns[4].Width = 100;
                this.dgvCommissionBill.Columns[5].Width = 100;
                this.dgvCommissionBill.Columns[6].Width = 40;
            }
        }
        private void pbxCollapse_Click(object sender, EventArgs e)
        {
            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;
            }
        }
        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("Order Number");
            search.Add("Order Date");
            search.Add("Vendor");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderNo.DataSource = bs.DataSource;
            cbxSearchOrderNo.SelectedIndex = 0;

        }

        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("Order Number");
            search.Add("Order Date");
            search.Add("Vendor");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderDate.DataSource = bs.DataSource;
            cbxSearchOrderDate.SelectedIndex = 1;
        }

        private void SearchCreteria3()
        {
            List<string> search = new List<string>();
            search.Add("Order Number");
            search.Add("Order Date");
            search.Add("Vendor");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxVendor.DataSource = bs.DataSource;
            cbxVendor.SelectedIndex = 2;
        }

        public void bindregfernce()
        {
            DataTable dt = ReferenceBAL.GetReference();
            referenceperson.DataSource = dt;
            referenceperson.DisplayMember = "Name";
            referenceperson.ValueMember = "ReferencesID";
            DataRow dr = dt.NewRow();
            dr["Name"] = "-Select-";
            dr["ReferencesID"] = "0";
            dt.Rows.InsertAt(dr, 0);
        }

        private void vLabel1_Click(object sender, EventArgs e)
        {
            if (pnlLabelSearch.Visible == true)
            {
                pnlLabelSearch.Visible = false;
                vLabel1.Visible = false;
                pnlSearch.Visible = true;
                splitContainer1.Panel1Collapsed = false;
            }
        }

        private void pbxRightCollapse_Click(object sender, EventArgs e)
        {
            if (pnlCollapse2.Visible == true)
            {
                pnlOrder.Visible = true;
                vLabel2.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;
            }
        }

        private void vLabel2_Click(object sender, EventArgs e)
        {
            if (pnlOrder.Visible == true)
            {
                pnlOrder.Visible = false;
                vLabel2.Visible = false;
                pnlCollapse2.Visible = true;
                splitContainer1.Panel2Collapsed = false;
                pbxCollapse.Visible = true;
                pbxRightCollapse.Visible = true;
            }
        }


        private void panelright_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblcommall_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommall.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommtoday_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommtoday.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommthisweek_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommthisweek.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommthismonth_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommthismonth.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommthisyear_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommthisyear.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommyest_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommyest.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommlastweek_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommlastweek.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommlastmonth_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommlastmonth.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void lblcommlastyear_Click(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommlastyear.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void ListSearchDate_Click(object sender, EventArgs e)
        {
            if (lblcommPanel.Visible == true)
            {
                lblcommPanel.Visible = false;
            }
            else
            {
                lblcommPanel.Visible = true;
                lblcommPanel.BringToFront();
            }
        }

        private void txtPercentage_TextChanged(object sender, EventArgs e)
        {
            try
            {
                geamount();
            }
            catch
            {

            }
        }

        private void txtPercentage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
    && !char.IsDigit(e.KeyChar)
    && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            // only allow one decimal point
            if (e.KeyChar == '.'
                && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtPercentage_KeyDown(object sender, KeyEventArgs e)
        {

        }

        public bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;
            if (!string.IsNullOrEmpty(txtListDate.Text))
            {
                if (referenceperson.SelectedIndex == 0)
                {
                    message += "* Please Select Reference Person" + "\n";
                    i++;
                    //if (i == 1)
                    //{
                    //this.ActiveControl = txtListDate;

                    // }
                }
            }
            if (string.IsNullOrEmpty(message))
            {
                status = true;
            }
            else
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            {
                bool Val = Validation();
                if (Val == true)
                {
                    bindgrid();
                }

            }
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            string message = "Please Select Atleast One Person";
            int gridval = valid();
            if (gridval <= 0)
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);

            }
        }


        public int valid()
        {
            int val = 0;

            foreach (DataGridViewRow row in dgvCommissionBill.Rows)
            {
                if (Convert.ToBoolean(row.Cells[0].Value))
                {
                    val = val + 1;
                }

            }
            return val;
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            clear();
        }
        public void clear()
        {
            referenceperson.SelectedIndex = 0;
            txtListDate.Text = string.Empty;
            checkBox1.Checked = false;
            dgvCommissionBill.Rows.Clear();

            label5.Text = string.Empty;
            label4.Text = string.Empty;
            lblBillAmt.Text = string.Empty;
            label8.Text = string.Empty;
            txtPercentage.Text = string.Empty;
            txtPercentage.ReadOnly = true;

            pictureBox1.Image = Inventory.Properties.Resources.images1;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void CommissionBill_Load(object sender, EventArgs e)
        {
            this.ActiveControl = referenceperson;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (txtListDate.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = ListSearchDate;

                    return true;
                }

            }

            if (ListSearchDate.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnsech;

                    return true;
                }

            }
            if (btnsech.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = dgvCommissionBill;

                    return true;
                }

            }

            if (dgvCommissionBill.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = txtPercentage;

                    return true;
                }

            }

            if (btnupdatecommissionaccountpayable.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnsave;

                    return true;
                }

            }
            if (btnprint.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = referenceperson;

                    return true;
                }

            }



            if (keyData == Keys.Escape)
            {

                if (dgvCommissionBill.Rows.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Do you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        this.Close();
                    }
                }
                else
                {
                    this.Close();
                }
                return true;

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnnew_Click(object sender, EventArgs e)
        {

        }

        private void btnsaveaspending_Click(object sender, EventArgs e)
        {

        }

        public void bindgrid()
        {
            string s = string.Empty;
            if (referenceperson.SelectedIndex > 0)
            {
                s = Convert.ToString(referenceperson.SelectedValue);
            }
            else
            {
                s = "";
            }
            DataSet ds = objQuotationBal.GetTable(txtListDate.Text, s);


            if (dgvCommissionBill.Rows.Count > 0)
            {
                dgvCommissionBill.Rows.Clear();
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                dgvCommissionBill.Rows.Add();
                dgvCommissionBill.Rows[i].Cells[0].Value = i + 1;
                dgvCommissionBill.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[0].Rows[i]["Estimationid"]);
                dgvCommissionBill.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[0].Rows[i]["Date"]);
                dgvCommissionBill.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[0].Rows[i]["Amount"]);
                dgvCommissionBill.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[0].Rows[i]["Balance"]);
                dgvCommissionBill.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[0].Rows[i]["Reference Name"]);

                try
                {
                    if (Convert.ToDouble(dgvCommissionBill.Rows[i].Cells[4].Value) > 0)
                    {
                        DataGridViewRow cRow = dgvCommissionBill.Rows[i];
                        cRow.DefaultCellStyle.BackColor = Color.Orange; 
                    }
                }
                catch (Exception exp)
                {

                }


            }



            if (ds.Tables[1].Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[0]["Name"])))
                {
                    label5.Text = Convert.ToString(ds.Tables[1].Rows[0]["Name"]);
                }
                else
                {
                    label5.Text = string.Empty;
                }

                if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[0]["Phone"])))
                {
                    label4.Text = Convert.ToString(ds.Tables[1].Rows[0]["Phone"]);
                }
                else
                {
                    label4.Text = string.Empty;
                }

                if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[0]["path"])))
                {
                    pictureBox1.ImageLocation = Convert.ToString(ds.Tables[1].Rows[0]["path"]);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else
                {
                    pictureBox1.Image = Inventory.Properties.Resources.images1;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            else
            {
                label5.Text = string.Empty;
                label4.Text = string.Empty;
                pictureBox1.Image = Inventory.Properties.Resources.images1;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }

        }

        private void btnproceed_Click(object sender, EventArgs e)
        {
            double val = 0.0;
            int vas = 0;


            foreach (DataGridViewRow dataGridRow in dgvCommissionBill.Rows)
            {
                if (dataGridRow.Cells["ChkIsCheckOut"].Value != null && (bool)dataGridRow.Cells["ChkIsCheckOut"].Value)
                {
                    val += Convert.ToDouble(dataGridRow.Cells["Amount"].Value);
                    vas = vas + 1;
                }

            }
            if (vas == 0)
            {
                MessageBox.Show("Please Check Item");
                txtPercentage.ReadOnly = true;
                lblBillAmt.Text = Convert.ToString(0.00);
                label8.Text = Convert.ToString(0.00);
            }
            else
            {

                txtPercentage.ReadOnly = false;
                lblBillAmt.Text = String.Format("{0:00.00}", Convert.ToString(val));
                label8.Text = Convert.ToString(vas);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (dgvCommissionBill.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvCommissionBill.Rows)
                {
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[5];
                    chk.Value = !(chk.Value == null ? false : (bool)chk.Value); //because chk.Value is initialy null

                }
            }
            else
            {

                checkBox1.Checked = false;

            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            amountmode = "Percentage";
            geamount();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            amountmode = "Net";
            geamount();
        }


        public void geamount()
        {
            if (!string.IsNullOrEmpty(txtPercentage.Text.Trim()))
            {
                double P = Convert.ToDouble(txtPercentage.Text.Trim());


                if (amountmode == "Percentage")
                {
                    if (P > 100)
                    {
                        MessageBox.Show("Percentege should be greater than 100");
                        txtPercentage.Text = "";
                        lblAmt.Text = "0";
                        txtPercentage.Focus();
                        return;
                    }
                    double BillAmt = Convert.ToDouble(lblBillAmt.Text.Trim());
                    double Amt = Convert.ToDouble((P / 100) * BillAmt);
                    lblAmt.Text = Convert.ToString(Amt);
                }
                else if (amountmode == "Net")
                {
                    if (P > Convert.ToDouble(lblBillAmt.Text.Trim()))
                    {
                        MessageBox.Show("Amount should be greater than BillAmount");
                        txtPercentage.Text = "";
                        lblAmt.Text = "0";
                        txtPercentage.Focus();
                        return;
                    }
                    int BillAmt = Convert.ToInt32(lblBillAmt.Text.Trim());
                    //decimal Amt = BillAmt - Convert.ToDecimal((P));
                    //lblAmt.Text = Convert.ToString(Amt);
                    lblAmt.Text = Convert.ToString((P));
                }
            }
            else
            {
                lblAmt.Text = string.Empty;
            }
        }

        private void btnupdatecommissionaccountpayable_Click(object sender, EventArgs e)
        {
            string message = string.Empty;

            bool das = valgrid();
            if (das == false)
            {
                message = "Please Select Atleast One Person";
            }
            else if (string.IsNullOrEmpty(txtPercentage.Text))
            {
                message = message + "Please Enter Percentage / Amount";
            }
            else if (txtPercentage.Text == ".")
            {
                message = message + "Please Enter  Correct Percentage / Amount";
            }
            else if (!string.IsNullOrEmpty(txtPercentage.Text))
            {
                if (radioButton1.Checked)
                {
                    if (txtPercentage.Text=="100")
                    {
                        message = message + "Percentage Should Not equal To 100";
                    }
                    
                }
                if (radioButton2.Checked)
                {

                    if (Convert.ToDouble(txtPercentage.Text) == Convert.ToDouble(lblBillAmt.Text))
                    {
                        message = message + "Amount Should Not Equal Bill Amount";
                    }
                }
            }
          

            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
            }
            else
            {
                save();
            }
        }


        public bool valgrid()
        {
            bool vas = false;
            foreach (DataGridViewRow dataGridRow in dgvCommissionBill.Rows)
            {
                if (dataGridRow.Cells["ChkIsCheckOut"].Value != null && (bool)dataGridRow.Cells["ChkIsCheckOut"].Value)
                {
                    vas = true;
                }

            }
            return vas;
        }
        public void save()
        {
            DataTable dt = new DataTable();
            objQuotationBal.totalammount = lblBillAmt.Text;
            objQuotationBal.Customerid = label5.Text;
            objQuotationBal.commisionamount = Convert.ToString(Math.Round(Convert.ToDecimal(lblAmt.Text)));
            objQuotationBal.commisionpercentage = txtPercentage.Text;
            if (radioButton1.Checked)
            {
                objQuotationBal.commisionMode = "Percentage";
            }
            else if (radioButton2.Checked)
            {
                objQuotationBal.commisionMode = "Amount";
            }

            objQuotationBal.Updatedby = Program.userid;


            dt = DataGridView2DataTable();


            string output = objQuotationBal.SaveCommision(objQuotationBal, dt);
            if (output == "1")
            {
                clear();
            }
        }

        public DataTable DataGridView2DataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[2] { new DataColumn("Estimationid", typeof(string)),
                            new DataColumn("Amount", typeof(string))
                            });
            foreach (DataGridViewRow dataGridRow in dgvCommissionBill.Rows)
            {
                if (dataGridRow.Cells["ChkIsCheckOut"].Value != null && (bool)dataGridRow.Cells["ChkIsCheckOut"].Value)
                {
                    dt.Rows.Add(Convert.ToString(dataGridRow.Cells["Estimationid"].Value), Convert.ToString(dataGridRow.Cells["Amount"].Value));
                }

            }
            return dt;

        }

        private void lblcommfromdate_ValueChanged(object sender, EventArgs e)
        {
            txtListDate.Text = lblcommfromdate.Text.Trim();
            lblcommPanel.Visible = false;
        }

        private void btnprint_Click(object sender, EventArgs e)
        {

        }

        private void dgvCommissionBill_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}





















































