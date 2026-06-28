using InvBal;
using Inventory.Report_Purchase;
using Inventory.Sales;
using PurchaseOrderReport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Purchase
{
    public partial class SearchPurchaseOrder : Form
    {
        bool all = false;
        PurchaseOrderBAL OblPurchaseOrderBAL = new PurchaseOrderBAL();
        public SearchPurchaseOrder()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            LoadPorts();
            //SearchPurchaseOrder1();
            checkBox1.Checked=true;
        }
        private void LoadPorts()
        {
            dgvOrder.Rows.Clear();
            dgvOrder.Columns.Clear();
            dgvOrder.ColumnCount = 8;
            dgvOrder.RowCount = 1;
            dgvOrder.Columns[0].Name = "S.No";
            dgvOrder.Columns[1].Name = "Items";
            dgvOrder.Columns[2].Name = "Price";
            dgvOrder.Columns[3].Name = "Ordered Qty";
            dgvOrder.Columns[4].Name = "Receipt";
            dgvOrder.Columns[5].Name = "Checking";
            dgvOrder.Columns[6].Name = "Floor Check In";
            dgvOrder.Columns[7].Name = "Balance";

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvOrder.Columns[0].Width = 60;
                this.dgvOrder.Columns[1].Width = 180;
                this.dgvOrder.Columns[2].Width = 100;
                this.dgvOrder.Columns[3].Width = 120;
                this.dgvOrder.Columns[4].Width = 90;
                this.dgvOrder.Columns[5].Width = 90;
                this.dgvOrder.Columns[6].Width = 220;
                this.dgvOrder.Columns[7].Width = 100;

            }
            else
            {
                this.dgvOrder.Columns[0].Width = 60;
                this.dgvOrder.Columns[1].Width = 380;
                this.dgvOrder.Columns[2].Width = 80;
                this.dgvOrder.Columns[3].Width = 100;
                this.dgvOrder.Columns[4].Width = 90;
                this.dgvOrder.Columns[5].Width = 90;
                this.dgvOrder.Columns[6].Width = 170;
                this.dgvOrder.Columns[7].Width = 100;
            }


            //this.dgvOrder.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;


            //this.dgvOrder.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;            
            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvOrder.Columns[1].ReadOnly = true;

            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            this.dgvOrder.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            this.dgvOrder.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            this.dgvOrder.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            this.dgvOrder.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

          //  dgvOrder.Columns[3].Visible = false;

            //this.dgvOrder.Columns[4].ReadOnly = true;
            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }

        }
        private void SearchPurchaseOrder_Load(object sender, EventArgs e)
        {
            GetSuppliers1();
            GetSuppliers2();
            GetSearchPurchaseOrder();
            
           // GetSearchPurchaseOrder();
        }

        private void pbxCollapse_Click(object sender, EventArgs e)
        {
            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;
                vLabel1.Enabled = true;
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
                //this.dgvSearch.Columns[1].Visible = true;
                //this.dgvSearch.Columns[2].Visible = true;
                //this.dgvSearch.Columns[3].Visible = true;
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

                //this.dgvSearch.Columns[1].Visible = false;
                //this.dgvSearch.Columns[2].Visible = false;
                //this.dgvSearch.Columns[3].Visible = false;
            }
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
        private void pbxCollapse_Click_1(object sender, EventArgs e)
        {
            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;
                vLabel1.Enabled = true;
            }
        }

        private void pbxRightCollapse_Click_1(object sender, EventArgs e)
        {
            if (pnlCollapse2.Visible == true)
            {
                pnlOrder.Visible = true;
                vLabel2.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;
                this.dgvSearch.Columns[1].Visible = true;
                this.dgvSearch.Columns[2].Visible = true;
                this.dgvSearch.Columns[3].Visible = true;
            }
        }
        public void GetSuppliers2()
        {
            DataTable dtsup = OblPurchaseOrderBAL.GetSuppliers(null);
            cbVendor.DataSource = dtsup;
            cbVendor.ValueMember = "SuppliersID";
            cbVendor.DisplayMember = "Name";
            cbVendor.SelectedIndex = 0;
        }
        public void GetSuppliers1()
        {
            DataTable dtsup = OblPurchaseOrderBAL.GetSuppliers(null);
            cmbstatus3.DataSource = dtsup;
            cmbstatus3.ValueMember = "SuppliersID";
            cmbstatus3.DisplayMember = "Name";
            cmbstatus3.SelectedIndex = 0;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {





            if (keyData == (Keys.F3))
            {
                txtOrderNo.Focus();
                this.ActiveControl = txtOrderNo;
                return true;
            }
            if (keyData == (Keys.A | Keys.Alt))
            {
                all = true;
                GetSearchPurchaseOrder();

                return true;
            }


         
            //if (txtorder.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = date;
            //        return true;
            //    }

            //}

           
            //if (cmbloaction.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = Txtitem;
            //        return true;
            //    }

            //}
           

            //if (btnPrint.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = cmbcustomername;
            //        return true;
            //    }
            //}



            if (keyData == Keys.Escape)
            {
              
                    if (dgvOrder.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[0].Cells[1].Value)))
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
        private void SearchPurchaseOrder1()
        {
            dgvSearch.Rows.Clear();
            dgvSearch.Columns.Clear();
            dgvSearch.ColumnCount = 4;
            dgvSearch.RowCount = 16;

            dgvSearch.Columns[0].Name = "Order No";
            dgvSearch.Columns[1].Name = "Vendor Name";
            dgvSearch.Columns[2].Name = "Date";
            dgvSearch.Columns[3].Name = "Status ";

            this.dgvSearch.Columns[0].Width = 60;
            this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvSearch.Columns[1].Width = 120;

            this.dgvSearch.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvSearch.Columns[2].Width = 40;

            this.dgvSearch.Columns[3].Width = 40;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn c in dgvSearch.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }

            //this.dgvSearch.Columns[1].Visible = false;
            //this.dgvSearch.Columns[2].Visible = false;
            //this.dgvSearch.Columns[3].Visible = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            GetSearchPurchaseOrder();
        }

        private void GetSearchPurchaseOrder()
        {
            //if(dateTimePicker1.Value >dateTimePicker2.Value)
            //{
            //    MessageBox.Show("Order Date From should not be greater than Order Date To");
            //    return;
            //}

            string OrderNo = txtOrderNo.Text.Trim();
            DateTime FromDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
            DateTime ToDate = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);

            string Vendorid = Convert.ToString(cmbstatus3.SelectedValue);
            string Iscombined = string.Empty;
            if(all==true)
            {
                Iscombined = null;
            }
            else
            {
            if (checkBox1.Checked)
            {
                Iscombined = "1";
            }
            else
            {
                Iscombined = "0";
            }
            }
            string ProductName = txtprodname.Text.Trim();
            DataTable dt = OblPurchaseOrderBAL.GetSearchPurchaseOrder(OrderNo, FromDate, ToDate, Iscombined, Vendorid, ProductName);
            dgvSearch.DataSource = dt;
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            dgvSearch.Columns["PurchaseId"].Visible = false;
           // dgvSearch.Columns["PurchaseId"].Visible = false;
            dgvSearch.Columns["OrderNumber"].HeaderText = "Order Number";
            dgvSearch.Columns["status"].HeaderText = "Status";
            dgvSearch.Columns["OrderDate"].Visible = false;
            dgvSearch.Columns["ExpectedDeliveryDate"].Visible = false;
            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            all = false;

        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string id = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[0].Value);
                string val = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells[1].Value);
                GetPurchaseOrderByOrderNo(id, val);
                total();
            }
        }
        public void total()
        {
            double totalamount = 0, totalquantity = 0;
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                //totalamount = totalamount + Convert.ToInt32(dgvOrder.Rows[i].Cells[6].Value);
                totalquantity += Convert.ToDouble(dgvOrder.Rows[i].Cells[3].Value);
            }
            lbltotalquantity.Text = Convert.ToString(totalquantity);
            //lbltotalamount.Text = Convert.ToString(totalamount);
        }
        public void GetPurchaseOrderByOrderNo(string s, string val)
        {
            DataSet ds = OblPurchaseOrderBAL.GetPurchaseOrderByOrderNo(val, "New");
            DataSet ds1 = OblPurchaseOrderBAL.Proc_SearchPurchaseReceipt(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                dateTimePicker3.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderDate"]);
                dateTimePicker4.Text = Convert.ToString(ds.Tables[0].Rows[0]["ExpectedDeliveryDate"]);
                cbVendor.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"]);
                TxtOrdNO.Text = Convert.ToString(ds.Tables[0].Rows[0]["OrderNumber"]);
                cbxStatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                txtRemarks.Text = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
              //  cbxStatus.Enabled = true;
                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
             //   clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                dgvOrder.Rows.Clear();
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    dgvOrder.Rows.Add();
                    dgvOrder.Rows[i].Cells[0].Value = i + 1;
                    dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(ds1.Tables[0].Rows[i]["ProductName"]);
                    dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(ds1.Tables[0].Rows[i]["Price"]);
                    dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds1.Tables[0].Rows[i]["OrderedQty"]);
                    dgvOrder.Rows[i].Cells[4].Value = Convert.ToString(ds1.Tables[0].Rows[i]["Receipt"]);
                    dgvOrder.Rows[i].Cells[5].Value = Convert.ToString(ds1.Tables[0].Rows[i]["Checking"]);
                    dgvOrder.Rows[i].Cells[6].Value = Convert.ToString(ds1.Tables[0].Rows[i]["FloorCheckIn"]);
                    dgvOrder.Rows[i].Cells[7].Value = Convert.ToString(ds1.Tables[0].Rows[i]["Balance"]);

                }
                panel2.Enabled = false;

            }
            else
            {
                dgvOrder.Rows.Clear();
                panel2.Enabled = true;
            }

        }

        private void cbVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindSupplierCity();
        }
        public void bindSupplierCity()
        {
            int SupplierId = 0;
            if (cbVendor.SelectedIndex != 0)
            {
                SupplierId = Convert.ToInt32(cbVendor.SelectedValue);
                DataTable dtCity = OblPurchaseOrderBAL.GetSuppliers(SupplierId);
                if (dtCity.Rows.Count > 0)
                {
                    txtCity.Text = Convert.ToString(dtCity.Rows[1]["City"]);
                }
                else
                {
                    txtCity.Clear();
                }
            }
            else
            {
                txtCity.Clear();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }
        private void clear()
        {
            cbVendor.SelectedIndex = 0;
            txtCity.Clear();
            txtOrderNo.Clear();
            dgvOrder.Rows.Clear();
            dgvOrder.Rows.Add(1);
            //dgvOrder.Columns.Clear();
            //LoadPorts();
            cbxStatus.Text = ""; ;
            this.dateTimePicker3.Value = DateTime.Now;
            txtRemarks.Clear();
            this.dateTimePicker4.Value = DateTime.Now;
            lbltotalquantity.Text = "0";
            cbxStatus.Enabled = false;
          
           // cbVendor.Enabled = true;
           // dateTimePicker2.Enabled = true;
            cbxStatus.Enabled = true;
            TxtOrdNO.Clear();
           
        }


        public void GetReport(string QuotationId)
        {
            //try
            //{
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

                        
                        SqlCommand cmd = new SqlCommand("Report_PurchaseOrder_print_Direct", con);
                        cmd.Parameters.AddWithValue("@OrderNumber", QuotationId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            try
                            {

                                PurchaseorderPrint objRREPrint = new PurchaseorderPrint();
                                objRREPrint.dsMain = ds;
                                objRREPrint.pagenumber = 1;
                                objRREPrint.status = true;
                                objRREPrint._strRefText = "Pur:";
                                objRREPrint._strRef = QuotationId;

                                objRREPrint.RREPrintPurchaseOrderQuotation();
                            }

                            catch (Exception ex)
                            {

                            }
                        }



                        //System.Diagnostics.Process myProc = new System.Diagnostics.Process();
                        //myProc.StartInfo.FileName = "type " + Obj.fileName + " >prn";  //Attempting to start a non-existing executable
                        //myProc.Start();    //Start the application and assign it to the process component.    
                        //ExecuteCommandSync("type " + Obj.fileName + " >prn");



                    }
                }
            }


            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message.ToString());
            //}
        }
        public void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }

        private void Btnprint_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtOrdNO.Text))
            {
                GetReport(TxtOrdNO.Text);

                //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //if (result == DialogResult.Yes)
                //{
                //    PurchaseOrderRV rpt = new PurchaseOrderRV(TxtOrdNO.Text);
                //    rpt.ShowDialog();
                //}
            }
            else
            {
                MessageBox.Show("Please Select Item");
            }
        }

        private void dgvSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Down)
                {
                    if (dgvSearch.CurrentCell.RowIndex >= 0)
                    {
                        string id = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex + 1].Cells[0].Value);
                        string val = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex + 1].Cells[1].Value);
                        GetPurchaseOrderByOrderNo(id, val);
                        total();
                    }
                }
                else if (e.KeyData == Keys.Up)
                {
                    if (dgvSearch.CurrentCell.RowIndex >= 0)
                    {
                        string id = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex - 1].Cells[0].Value);
                        string val = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex - 1].Cells[1].Value);
                        GetPurchaseOrderByOrderNo(id, val);
                        total();
                    }
                }
            }
            catch
            {

            }

        }
    }
}
