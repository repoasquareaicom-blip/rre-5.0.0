using System;
using InvBal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Data.SqlClient;
using Inventory.Sales;

namespace Inventory.Commission
{
    public partial class CashRequestlist : Form
    {
        QuotationBal objQuotationBal = new QuotationBal();
        public CashRequestlist()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            //DataTable dt = GetTable();
            LoadPortsChecking();
            bindgrid();
            //dgvCommissionBill.DataSource = dt;
           
            pnlLabelSearch.Visible = true;
            vLabel1.Visible = true;
            pnlSearch.Visible = false;
            bindregfernce();
            splitContainer1.Panel1Collapsed = true;
            comreferenceperson.SelectedIndex = 0;
        }
        //dgvCommissionBill
        private void LoadPortsChecking()
        {
            dgvCommissionBill.Rows.Clear();
            dgvCommissionBill.ColumnCount =7;


            dgvCommissionBill.Columns[0].Name = "S.NO";
            dgvCommissionBill.Columns[1].Name = "referenceid";

            dgvCommissionBill.Columns[2].Name = "CommissionAmount";
            dgvCommissionBill.Columns[3].Name = "Totalbalance";

            dgvCommissionBill.Columns[4].Name = "Updatedon";
            dgvCommissionBill.Columns[5].Name = "Phone";

            dgvCommissionBill.Columns[6].Name = "Commissionid";


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
            this.dgvCommissionBill.Columns[2].ReadOnly = true;
            this.dgvCommissionBill.Columns[5].ReadOnly= true;
            
           
            


            this.dgvCommissionBill.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            this.dgvCommissionBill.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            dgvCommissionBill.Columns[0].HeaderText = "S.NO";
            dgvCommissionBill.Columns[1].HeaderText = "REFERENCE NAME";

            dgvCommissionBill.Columns[2].HeaderText = "REFERENCE MOBILE NUMBER";
            dgvCommissionBill.Columns[3].HeaderText = "ROSE AMOUNT";

            dgvCommissionBill.Columns[4].HeaderText = "BILL AMOUNT";
            dgvCommissionBill.Columns[5].HeaderText = "ROSE DATE";

            dgvCommissionBill.Columns[6].HeaderText = "Commissionid";
            foreach (DataGridViewColumn c in dgvCommissionBill.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvCommissionBill.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvCommissionBill.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvCommissionBill.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            this.dgvCommissionBill.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvCommissionBill.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvCommissionBill.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvCommissionBill.Columns[0].Width = 20;
            this.dgvCommissionBill.Columns[1].Width = 100;
            this.dgvCommissionBill.Columns[2].Width = 70;
            this.dgvCommissionBill.Columns[3].Width = 40;
            this.dgvCommissionBill.Columns[4].Width = 40;
            this.dgvCommissionBill.Columns[5].Width = 65;
            //this.dgvCommissionBill.Columns[6].Width = 65;
            

            DataGridViewButtonColumn Print = new DataGridViewButtonColumn();
            
            Print.HeaderText = "Print";
            Print.Text = "Print";
            Print.Name = "Print";
            Print.FlatStyle = FlatStyle.Popup;
            Print.UseColumnTextForButtonValue = true;
            dgvCommissionBill.Columns.Add(Print);
            dgvCommissionBill.Columns["Print"].Width = 20;
            dgvCommissionBill.Columns[6].Visible = false;

        }
        public Button btn;
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
            comreferenceperson.DataSource = dt;
            comreferenceperson.DisplayMember = "Name";
            comreferenceperson.ValueMember = "ReferencesID";
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
        private void bindgrid()
        {
            string s = string.Empty;
            if (comreferenceperson.SelectedIndex > 0)
            {
                s = Convert.ToString(comreferenceperson.Text);
            }
            else
            {
                s = "";
            }
            DateTime Fromdates = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
            DateTime Todates = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);
            DataSet ds = objQuotationBal.GetTables(Fromdates, Todates, s);

            if (dgvCommissionBill.Rows.Count > 0)
            {
                dgvCommissionBill.Rows.Clear();
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {


                dgvCommissionBill.Rows.Add();
                dgvCommissionBill.Rows[i].Cells[0].Value = i + 1;
                dgvCommissionBill.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[0].Rows[i]["referenceid"]);
                dgvCommissionBill.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[0].Rows[i]["Phone"]);
                dgvCommissionBill.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[0].Rows[i]["CommissionAmount"]);
                dgvCommissionBill.Rows[i].Cells[4].Value = Convert.ToString(ds.Tables[0].Rows[i]["Totalbalance"]);
             
                dgvCommissionBill.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[0].Rows[i]["Updatedon"]);
                dgvCommissionBill.Rows[i].Cells[6].Value = Convert.ToString(ds.Tables[0].Rows[i]["Commissionid"]);

                //dgvCommissionBill.Rows[i].Cells[8].Value = dgvCommissionBill.Columns.Add(btn);
                //btn.Text = "Delete";
            }
            ////DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
            //dgvCommissionBill.Columns.Add(btn);
            //btn.HeaderText = "";
            //btn.Text = "Delete";
            //btn.Name = "btn";
            //btn.UseColumnTextForButtonValue = true;
        }
        //static DataTable GetTable()
        //{
        //    // Here we create a DataTable with four columns.
        //    DataTable table = new DataTable();
        //    table.Columns.Add("SlNo", typeof(int));
        //    table.Columns.Add("Reference", typeof(string));
        //    table.Columns.Add("Amount to be paid", typeof(string));
        //    table.Columns.Add("Paid", typeof(string));
        //    table.Columns.Add("Balance to be Paid", typeof(string));            
        //    // Here we add five DataRows.
        //    table.Rows.Add(1, "Mohan", "2000", "1500", "500");
        //    table.Rows.Add(2, "ARIVU", "2000", "1500", "500");
        //    table.Rows.Add(3, "Bharath", "2000", "1500", "500");
        //    table.Rows.Add(4, "Bala", "2000", "1500", "500");
        //    table.Rows.Add(5, "Prabhu", "2000", "1500", "500");
        //    return table;
        //}

      

        private void dgvCommissionBill_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCommissionBill.Columns[e.ColumnIndex].Name == "Action")
            {
            }
        }     

        private void panelright_Paint(object sender, PaintEventArgs e)
        {

        }


        private void lblcommtoday_Click(object sender, EventArgs e)
        {
           
        }

        private void lblcommthisweek_Click(object sender, EventArgs e)
        {
          
        }

        private void lblcommthismonth_Click(object sender, EventArgs e)
        {
          
        }

        private void lblcommthisyear_Click(object sender, EventArgs e)
        {
           
        }

        private void lblcommyest_Click(object sender, EventArgs e)
        {
           
        }

        private void lblcommlastweek_Click(object sender, EventArgs e)
        {
           
        }

        private void lblcommlastmonth_Click(object sender, EventArgs e)
        {
           
        }

        private void lblcommlastyear_Click(object sender, EventArgs e)
        {
           
        }

        private void ListSearchDate_Click(object sender, EventArgs e)
        {
         
        }

        private void CashRequestlist_Load(object sender, EventArgs e)
        {
            this.ActiveControl = comreferenceperson;
        }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
      {

          if (comreferenceperson.Focused)
          {
              if (keyData == (Keys.Tab))
              {
                  this.ActiveControl = dateTimePicker1;
                  return true;
              }

          }

          if (dateTimePicker1.Focused)
          {
              if (keyData == (Keys.Tab))
              {
                  this.ActiveControl = dateTimePicker2;
                  return true;
              }

          }
          if (dateTimePicker2.Focused)
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
                      this.ActiveControl = btnclear;
                      return true;
                  }

              }

              if (btnclear.Focused)
              {
                  if (keyData == (Keys.Tab))
                  {
                      this.ActiveControl = comreferenceperson;
                      return true;
                  }

              }

              //if (dgvCommissionBill.Focused)
              //{
              //    if (keyData == (Keys.Tab))
              //    {
              //        this.ActiveControl = btnclear;
              //        return true;
              //    }

              //}

              if (keyData == Keys.Escape)
              {
                  this.Close();
                  return true;

              }
          return base.ProcessCmdKey(ref msg, keyData);
      }

    private void btnprint_Click(object sender, EventArgs e)
    {

    }

    private void btnclear_Click(object sender, EventArgs e)
    {
      clear();
    }

    private void clear()
    {
        if (comreferenceperson.SelectedIndex>0)
        {
        comreferenceperson.SelectedIndex = 0;
        }

       

    }

    private void btnsech_Click(object sender, EventArgs e)
    {
        bindgrid();
    }

    private void label3_Click(object sender, EventArgs e)
    {
       
    }

    private void dgvCommissionBill_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            if (dgvCommissionBill.Columns[e.ColumnIndex].HeaderText == "Print")
            {
                //dgvCommissionBill.Rows[i].Cells[8].Value = Convert.ToString(ds.Tables[0].Rows[i]["Commissionid"]);
                
                string ReceiptId = Convert.ToString(dgvCommissionBill.Rows[e.RowIndex].Cells["Commissionid"].Value);
                //MessageBox.Show(ReceiptId);
                GetReport1(ReceiptId);

               
            }
        }



    }

    public void GetReport1(string QuotationId)
    {

        if (!string.IsNullOrEmpty(QuotationId))
        {
            DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Quotationreport rpt = new Quotationreport(txtorder.Text);
                //rpt.ShowDialog();

                using (SqlConnection con = new SqlConnection(Program.connection))
                {
                    DataSet ds = new DataSet();
                    con.Open();
                    SqlCommand cmd = new SqlCommand("GetComissionPaymentId", con);
                    cmd.Parameters.AddWithValue("@RequestId", QuotationId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        try
                        {

                            RoseBillReport objRREPrint = new RoseBillReport();
                            objRREPrint.dsMain = ds;
                            objRREPrint.pagenumber = 1;
                            objRREPrint.status = true;
                            objRREPrint._strRefText = "Ro:";
                            objRREPrint._strRef = QuotationId;

                            objRREPrint.RREPrintRoseBill();
                        }

                        catch (Exception ex)
                        {

                        }
                    }



                }
            }
        }


    }

   

    }
}





















































