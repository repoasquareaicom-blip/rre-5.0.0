using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Inventory.Movement
{
    public partial class FrmReceived : Form
    {
        public bool edit = false;
        public TextBox tb;
        public TextBox Quantitytomove1;
        string clickstatus = string.Empty;
        IssuedReceivedBAL objIssuedReceivedBAL = new IssuedReceivedBAL();
        string role1 = string.Empty;
        string srole = string.Empty;
        string mianid;

        public FrmReceived()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            srole = Program.userid;
            if (srole == "1")
            {
                role1 = "Admin";
            }
            else
            {
                role1 = "Emp";
            }
            SearchCreteria2();
            SearchCreteria1();
            GetLocation();
            GetCustomers();
            LoadPorts();
            LoadPortsFloorCheckIN();
            // BindsearchGrid();
            Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            Txtitem.AutoCompleteCustomSource = AutoCompleteLoad();
            Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;

            RBReceived.Checked = true;
            ddlcustomers.Focus();
            this.ActiveControl = ddlcustomers;
        }

        #region bind
        private void LoadPortsFloorCheckIN()
        {
            //dgvFloorCheckOut.Rows.Clear();
            //dgvFloorCheckOut.ColumnCount = 5;


            //dgvFloorCheckOut.Columns[0].Name = "S.NO";
            //dgvFloorCheckOut.Columns[1].Name = "Items";
            //dgvFloorCheckOut.Columns[2].Name = "Quantity";
            //dgvFloorCheckOut.Columns[4].Name = "Productid";
            ////dgvFloorCheckOut.Columns[6].Name = "Productid";
            //dgvFloorCheckOut.Columns[3].Name = "Rack";


            //this.dgvFloorCheckOut.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvFloorCheckOut.Columns[0].Width = 50;

            //this.dgvFloorCheckOut.Columns[4].Visible = false;

            //this.dgvFloorCheckOut.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvFloorCheckOut.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvFloorCheckOut.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvFloorCheckOut.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            //this.dgvFloorCheckOut.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dgvFloorCheckOut.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvFloorCheckOut.Columns[1].Width = 400;


            //this.dgvFloorCheckOut.Columns[0].ReadOnly = true;
            //this.dgvFloorCheckOut.Columns[1].ReadOnly = true;
            //this.dgvFloorCheckOut.Columns[2].ReadOnly = true;
            //this.dgvFloorCheckOut.Columns[3].ReadOnly = true;

            //this.dgvFloorCheckOut.Columns[3].Width = 80;

            //this.dgvFloorCheckOut.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvFloorCheckOut.Columns[2].Width = 110;


            //DataGridViewComboBoxColumn name = new DataGridViewComboBoxColumn();
            //name.HeaderText = "Location";
            //name.DataPropertyName = "Location";
            //name.FlatStyle = FlatStyle.Popup;
            //dgvFloorCheckOut.Columns.Insert(3, name);
            ////this.dgvFloorCheckOut.Columns[3].Width = 150;


            //DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            //dgvCmb.ValueType = typeof(bool);
            //dgvCmb.Name = "ChkFloorCheckOut";
            //dgvCmb.HeaderText = "IsCheckOut";
            //dgvCmb.FlatStyle = FlatStyle.Popup;

            //dgvFloorCheckOut.Columns.Insert(6, dgvCmb);
            //this.dgvFloorCheckOut.Columns[6].Width = 115;


            //dgvFloorCheckOut.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //foreach (DataGridViewColumn c in dgvFloorCheckOut.Columns)
            //{
            //    c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            //}
            dgvFloorCheckOut.Rows.Clear();
            dgvFloorCheckOut.ColumnCount = 5;


            dgvFloorCheckOut.Columns[0].Name = "S.NO";
            dgvFloorCheckOut.Columns[1].Name = "Items";
            dgvFloorCheckOut.Columns[2].Name = "Quantity";
            dgvFloorCheckOut.Columns[4].Name = "Productid";
            //dgvFloorCheckOut.Columns[6].Name = "Productid";
            dgvFloorCheckOut.Columns[3].Name = "Rack";


            this.dgvFloorCheckOut.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvFloorCheckOut.Columns[0].Width = 50;

            this.dgvFloorCheckOut.Columns[4].Visible = false;

            this.dgvFloorCheckOut.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvFloorCheckOut.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvFloorCheckOut.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvFloorCheckOut.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvFloorCheckOut.Columns[1].Width = 400;


            this.dgvFloorCheckOut.Columns[0].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[1].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[2].ReadOnly = true;
            this.dgvFloorCheckOut.Columns[3].ReadOnly = true;

            this.dgvFloorCheckOut.Columns[3].Width = 100;

            this.dgvFloorCheckOut.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dgvFloorCheckOut.Columns[2].Width = 100;


            DataGridViewComboBoxColumn name = new DataGridViewComboBoxColumn();
            name.HeaderText = "Location";
            name.DataPropertyName = "Location";
            name.FlatStyle = FlatStyle.Popup;
            dgvFloorCheckOut.Columns.Insert(3, name);
            //this.dgvFloorCheckOut.Columns[3].Width = 150;


            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "ChkFloorCheckOut";
            dgvCmb.HeaderText = "IsCheckOut";
            dgvCmb.FlatStyle = FlatStyle.Popup;

            dgvFloorCheckOut.Columns.Insert(6, dgvCmb);
            this.dgvFloorCheckOut.Columns[6].Width = 90;


            dgvFloorCheckOut.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvFloorCheckOut.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
        }
        public void bindsearchgrid()
        {
            Dgvmovementsearch.DataSource = IssuedReceivedBAL.BindsearchGrid();
        }
        private void LoadPorts()
        {
            dgvOrder.Rows.Clear();

            dgvOrder.Rows.Add(1);


            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[0].Width = 100;

            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[1].Width = 400;
            this.dgvOrder.Columns[1].ReadOnly = true;


            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvOrder.Columns[2].Width = 50;




            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvOrder.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvOrder.AlternatingRowsDefaultCellStyle.BackColor = Color.White;




            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }



        }
        public void GetLocation()
        {
            DataTable dtlocation = ProductMovementBal.GetLocation();
            DataRow row = dtlocation.NewRow();
            row["LocationID"] = "0";
            row["LocationName"] = "--Select--";
            dtlocation.Rows.InsertAt(row, 0);
            ddlLocation.DataSource = dtlocation;
            ddlLocation.ValueMember = "LocationID";
            ddlLocation.DisplayMember = "LocationName";
            ddlLocation.SelectedIndex = 1;

            cmbflrRDloc.DataSource = dtlocation;
            cmbflrRDloc.ValueMember = "LocationID";
            cmbflrRDloc.DisplayMember = "LocationName";
            cmbflrRDloc.SelectedIndex = 1;

        }

        public void GetCustomers()
        {
            DataTable dtlocation = ProductMovementBal.GetCustomers();
            DataRow row = dtlocation.NewRow();

            row["CustomerID"] = "0";
            row["Name"] = "--Select--";
            dtlocation.Rows.InsertAt(row, 0);
            ddlcustomers.DataSource = dtlocation;
            ddlcustomers.ValueMember = "CustomerID";
            ddlcustomers.DisplayMember = "Name";
            ddlcustomers.SelectedIndex = 0;

            cmbflrRDcus.DataSource = dtlocation;
            cmbflrRDcus.ValueMember = "CustomerID";
            cmbflrRDcus.DisplayMember = "Name";
            cmbflrRDcus.SelectedIndex = 0;

        }

        #endregion
        #region search
        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("EnteredBy");
            search.Add("EnteredOn");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchMovedBy.DataSource = bs.DataSource;
            cbxSearchMovedBy.SelectedIndex = 0;
            cmbstatus1.Visible = false;
        }
        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("EnteredBy");
            search.Add("EnteredOn");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderDate.DataSource = bs.DataSource;
            cbxSearchOrderDate.SelectedIndex = 1;
            cmbstatus2.Visible = false;
         
        }
        private void cbxSearchMovedBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchMovedBy.SelectedIndex == 2)
            {
                cmbstatus1.Visible = true;
                txtsearch1.Visible = false;
                ListSearchDate1.Visible = false;

            }
            else if (cbxSearchMovedBy.SelectedIndex == 1)
            {
                cmbstatus1.Visible = false;
                txtsearch1.Visible = true;
                txtsearch1.Text = "Today";
                ListSearchDate1.Visible = true;
            }
            else
            {
                cmbstatus1.Visible = false;
                txtsearch1.Visible = true;
                txtsearch1.Text = string.Empty;
                ListSearchDate1.Visible = false;
            }
            pnlCalender.Visible = false;
        }
        private void cbxSearchOrderDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchOrderDate.SelectedIndex == 2)
            {
                cmbstatus2.Visible = true;
                txtsearch2.Visible = false;
                ListSearchDate2.Visible = false;

            }
            else if (cbxSearchOrderDate.SelectedIndex == 1)
            {
                cmbstatus2.Visible = false;
                txtsearch2.Visible = true;
                txtsearch2.Text = "Today";
                ListSearchDate2.Visible = true;
            }
            else
            {
                cmbstatus2.Visible = false;
                txtsearch2.Visible = true;
                txtsearch2.Text = string.Empty;
                ListSearchDate2.Visible = false;
            }

            pnlCalender.Visible = false;
        }
        private void ListSearchDate1_Click(object sender, EventArgs e)
        {

            clickstatus = "search1";
            Calender();
            pnlCalender.Location = new Point(143, 54);
        }
        private void ListSearchDate2_Click(object sender, EventArgs e)
        {
            clickstatus = "search2";
            Calender();
            pnlCalender.Location = new Point(143, 79);
        }
        private void Calender()
        {
            if (pnlCalender.Visible)
            {
                pnlCalender.Visible = false;
            }
            else
            {
                pnlCalender.BringToFront();
                pnlCalender.Visible = true;
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            
            
            
            if ((cbxSearchMovedBy.SelectedIndex == cbxSearchOrderDate.SelectedIndex))
            {
                MessageBox.Show("*Search a item Should Not Be Same");
            }
            else
            {
                string firstname = string.Empty, firstvalue = string.Empty, secondname = string.Empty, secondvalue = string.Empty;
                string firstname1 = string.Empty, firstvalue1 = string.Empty, secondname1 = string.Empty, secondvalue1 = string.Empty;
                firstname = cbxSearchMovedBy.Text.Trim();
                if (firstname == "EnteredBy")
                {
                    firstname = "EnteredBy";
                    //firstvalue = Convert.ToString(cmbstatus1.SelectedValue);
                    if (!string.IsNullOrEmpty(txtsearch1.Text))
                    {
                        firstvalue = txtsearch1.Text;
                    }
                }
                else
                {
                    //firstvalue = txtsearch1.Text.Trim();
                    string part1 = txtsearch1.Text.Trim();
                    if (txtsearch1.Text.Trim() != "All")
                    {
                        part1 = txtsearch1.Text.Trim();
                    }
                    else
                    {
                        part1 = string.Empty;
                        firstvalue = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(part1))
                    {
                        string part = part1.Substring(0, 1);
                        if (Char.IsDigit(part, 0))
                        {
                            string[] rr = txtsearch1.Text.Split('-');
                            firstvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                        }
                        else
                        {
                            firstvalue = txtsearch1.Text.Trim();
                        }
                    }
                }

                secondname = cbxSearchOrderDate.Text.Trim();
                if (secondname == "EnteredBy")
                {
                    secondname = "EnteredBy";
                    //secondvalue = Convert.ToString(cmbstatus2.SelectedValue);
                    if (!string.IsNullOrEmpty(cmbstatus2.Text))
                    {
                        secondvalue = cmbstatus2.Text;
                    }
                }
                else
                {
                    //secondvalue = txtsearch2.Text.Trim();
                    string part1 = txtsearch2.Text.Trim();
                    if (txtsearch2.Text.Trim() != "All")
                    {
                        part1 = txtsearch2.Text.Trim();
                    }
                    else
                    {
                        part1 = string.Empty;
                        secondvalue = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(part1))
                    {
                        string part = part1.Substring(0, 2);
                        if (Char.IsDigit(part, 0))
                        {
                            string[] rr = txtsearch2.Text.Split('-');
                            secondvalue = rr[2] + "-" + rr[1] + "-" + rr[0];
                        }
                        else
                        {
                            secondvalue = txtsearch2.Text.Trim();
                        }
                    }
                }


                if (firstname == "EnteredBy")
                {
                    firstname1 = "EnteredBy";
                    firstvalue1 = firstvalue;
                }
                else if (firstname == "EnteredOn")
                {
                    secondname1 = "EnteredOn";
                    secondvalue1 = firstvalue;
                }


                if (secondname == "EnteredBy")
                {
                    firstname1 = "EnteredBy";
                    firstvalue1 = secondvalue;
                }
                else if (secondname == "EnteredOn")
                {
                    secondname1 = "EnteredOn";
                    secondvalue1 = secondvalue;
                }




                search("EnteredBy", firstvalue1, "EnteredOn", secondvalue1, role1, Program.userid);
            }
        }
        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string role, string UserId)
        {
            IssuedReceivedBAL obj = new IssuedReceivedBAL();
            DataTable dt = obj.SearchFrmReceived(firstname, firstvalue, secondname, secondvalue, role1, Program.UserName);
            Dgvmovementsearch.Columns.Clear();
            Dgvmovementsearch.DataSource = dt;

            Dgvmovementsearch.Columns["EnteredOn"].HeaderText = "Entered On";
            Dgvmovementsearch.Columns["EnteredBy"].HeaderText = "Entered By";
            Dgvmovementsearch.Columns["ReferenceNo"].HeaderText = "Reference No";
            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            Dgvmovementsearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            Dgvmovementsearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        private void txtsearch1_Click(object sender, EventArgs e)
        {
            string selecteditem = cbxSearchMovedBy.Text.ToString();
            if (selecteditem == "Date")
            {
                clickstatus = "search1";
                pnlCalender.BringToFront();
                pnlCalender.Visible = true;
                pnlCalender.Location = new Point(133, 54);
            }
            else
            {
                pnlCalender.Visible = false;
            }
        }
        private void txtsearch2_Click(object sender, EventArgs e)
        {
            if (cbxSearchOrderDate.Text.ToString() == "Date")
            {
                clickstatus = "search2";
                pnlCalender.BringToFront();
                pnlCalender.Visible = true;
                pnlCalender.Location = new Point(133, 79);
            }
            else
            {
                pnlCalender.Visible = false;
            }
        }
        private void panel3_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblAll.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblAll.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblAll_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblAll.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblAll.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblToday_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblToday.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblToday.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblThisWeek_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblThisWeek.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblThisWeek.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblThisMonth_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblThisMonth.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblThisMonth.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblThisYear_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblThisYear.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblThisYear.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblYesterday_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblYesterday.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblYesterday.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblLastWeek_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblLastWeek.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblLastWeek.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblLastMonth_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblLastMonth.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblLastMonth.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void lblLastYear_Click(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = lblLastYear.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = lblLastYear.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        private void SearchFrmDate_ValueChanged(object sender, EventArgs e)
        {
            if (clickstatus == "search1")
            {
                txtsearch1.Text = SearchFrmDate.Text.Trim();
            }
            else if (clickstatus == "search2")
            {
                txtsearch2.Text = SearchFrmDate.Text.Trim();
            }

            pnlCalender.Visible = false;
        }
        #endregion
        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (ddlLocation.Text == "--Select--")
            {
                i++;
                message = message + "* Please Select from location" + "\n";
                if (i == 1)
                    this.ActiveControl = ddlLocation;
            }
            if (ddlcustomers.Text == "--Select--")
            {
                i++;
                message = message + "* Please Select from customers" + "\n";
                if (i == 1)
                    this.ActiveControl = ddlcustomers;
            }

            string val = Convert.ToString(dgvOrder[1, 0].Value);
            if (string.IsNullOrEmpty(val))
            {
                i++;
                message = message + "* Please move atleast one product" + "\n";
                if (i == 1)
                    dgvOrder.CurrentCell = dgvOrder.Rows[0].Cells[1];
            }





            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }
        #region Buttons
        private void btnsave_Click(object sender, EventArgs e)
        {
            if (MainTabSalesBill.SelectedIndex == 0)
            {
                if (Validation())
                {
                    pnsearch.Visible = false;
                    save();
                    search("EnteredBy", "", "EnteredOn", "", role1, Program.UserName);
                    clear();
                }
            }
            else if (MainTabSalesBill.SelectedIndex == 1)
            {
                if (Validationcheckout())
                {
                    pnsearch.Visible = false;
                    SavecheckFloorCheckIn();

                    search("EnteredBy", "", "EnteredOn", "Today", role1, Program.userid);
                    clear();
                }
                else
                {
                    MessageBox.Show("Please Check All Items");
                }

            }
        }
        public void SavecheckFloorCheckIn()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Productid", typeof(int));
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("Rack", typeof(int));
            for (int i = 0; i < dgvFloorCheckOut.Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvFloorCheckOut.Rows[i].Cells["Items"].Value)))
                {
                    DataRow dr = dt.NewRow();
                    dr["Productid"] = dgvFloorCheckOut.Rows[i].Cells["Productid"].Value.ToString();
                    dr["Quantity"] = dgvFloorCheckOut.Rows[i].Cells["Quantity"].Value.ToString();
                    dr["Rack"] = dgvFloorCheckOut.Rows[i].Cells[3].Value.ToString(); ;
                    dt.Rows.Add(dr);
                }

            }
            string res = IssuedReceivedBAL.saveReceivedFloorcheckin(txtflrRDrefno.Text, dt);


        }
        private bool Validationcheckout()
        {
            bool status = true;
            string message = "";
            int i = 0;

            foreach (DataGridViewRow row in dgvFloorCheckOut.Rows)
            {
                DataGridViewCheckBoxCell CbxCell = (DataGridViewCheckBoxCell)row.Cells[6];
                if (Convert.ToBoolean(CbxCell.Value) == false)
                {
                    status = false;
                    break;
                }
            }
            return status;
        }
        private void btnclear_Click(object sender, EventArgs e)
        {

          
                clear();
           
        }
        public void clear()
        {
            if (MainTabSalesBill.SelectedIndex == 0)
            {
                panel2.Enabled = true;
                ddlLocation.SelectedIndex = 1;
                GetCustomers();
                ddlcustomers.SelectedIndex = 0;
                txtrefno.Text = string.Empty;
                txtRemarks.Text = string.Empty;
                Rbissued.Checked = true;
                LoadPorts();
                this.ActiveControl = ddlLocation;
                HidLblMain.Text = string.Empty;
            }
            else if (MainTabSalesBill.SelectedIndex == 1)
            {
                clearFloor();
            }

        }

        public void clearFloor()
        {
            
            cmbflrRDloc.SelectedIndex = 1;
            GetCustomers();
            cmbflrRDcus.SelectedIndex = 0;
            txtflrRDrefno.Text = string.Empty;
            dgvFloorCheckOut.Rows.Clear();
            txtRecRemarks.Text = string.Empty;
           // dgvFloorCheckOut.Columns.Clear();
           // LoadPortsFloorCheckIN();
            this.ActiveControl = cmbflrRDloc;
            HidLblMain.Text = string.Empty;

        }
        private void btnnew_Click(object sender, EventArgs e)
        {
            clear();
        }
        #endregion
        public void save()
        {
            IssuedReceivedBAL objIssuedReceivedBAL = new IssuedReceivedBAL();
            objIssuedReceivedBAL.cusid = Convert.ToString(ddlcustomers.Text);
            objIssuedReceivedBAL.location = Convert.ToString(ddlLocation.SelectedValue);
            objIssuedReceivedBAL.Movedby = Program.UserName;
            objIssuedReceivedBAL.Remarks = txtRemarks.Text;
            if (string.IsNullOrEmpty(HidLblMain.Text))
            {
                objIssuedReceivedBAL.MainID = String.Empty;
            }
            else
            {
                objIssuedReceivedBAL.MainID = HidLblMain.Text;
            }
            HidLblMain.Text = IssuedReceivedBAL.SaveGoodsReceivedHeader(objIssuedReceivedBAL);

            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[1].Value)))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[4].Value)))
                    {
                        objIssuedReceivedBAL.Subid = Convert.ToString(dgvOrder.Rows[i].Cells[4].Value);
                    }
                    else
                    {
                        objIssuedReceivedBAL.Subid = string.Empty;
                    }


                    objIssuedReceivedBAL.ProductID = Convert.ToString(dgvOrder.Rows[i].Cells[3].Value);
                    objIssuedReceivedBAL.Quantity = Convert.ToString(dgvOrder.Rows[i].Cells[2].Value);
                    if (Rbissued.Checked)
                        objIssuedReceivedBAL.type = "Issued";
                    else
                        objIssuedReceivedBAL.type = "Received";

                    objIssuedReceivedBAL.MainID = HidLblMain.Text;
                    string subid = IssuedReceivedBAL.SaveGoodsReceivedDetails(objIssuedReceivedBAL);
                    dgvOrder.Rows[i].Cells[4].Value = subid;
                }
            }

            //for (int i = 0; i < dgvOrder.Rows.Count; i++)
            //{
            //    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[1].Value)))
            //    {
            //        if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[5].Value)))
            //        {
            //            objIssuedReceivedBAL.status = 1;
            //        }
            //        else
            //        {
            //            objIssuedReceivedBAL.status = 2;
            //        }
            //        objIssuedReceivedBAL.cusid = Convert.ToString(ddlcustomers.SelectedValue);
            //        objIssuedReceivedBAL.Subid = Convert.ToString(dgvOrder.Rows[i].Cells[4].Value);
            //        objIssuedReceivedBAL.ProductID = Convert.ToString(dgvOrder.Rows[i].Cells[3].Value);
            //        objIssuedReceivedBAL.Quantity = Convert.ToString(dgvOrder.Rows[i].Cells[2].Value);
            //        if (Rbissued.Checked)
            //            objIssuedReceivedBAL.type = "IN";
            //        else
            //            objIssuedReceivedBAL.type = "OUT";

            //        objIssuedReceivedBAL.location = Convert.ToString(ddlLocation.SelectedValue);


            //        string transid = IssuedReceivedBAL.SaveIssuedReceivedTranscation(objIssuedReceivedBAL);
            //        dgvOrder.Rows[i].Cells[5].Value = transid;
            //    }
            //}

        }
        private void Txtitem_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyData == Keys.Enter)
            {

                if (!string.IsNullOrEmpty(lblproductid.Text) && lblproductid.Text != "0" && !string.IsNullOrEmpty(lblitemname.Text) && lblitemname.Text != "0")
                {
                    int rowindex = Convert.ToInt32(lblrowindex.Text);

                    dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    dgvOrder.Rows[rowindex].Cells[1].Value = lblitemname.Text.ToUpper();
                    dgvOrder.Rows[rowindex].Cells[2].Value = 1;

                    dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                    pnsearch.Visible = false;
                    lblproductid.Text = string.Empty;
                    Txtitem.Text = string.Empty;
                    lblitemcode.Text = "0";

                    lbldisplay.Text = "0";
                    lbldemo.Text = "0";
                    lblservice.Text = "0";
                    lblitemname.Text = string.Empty;
                    lblrack.Text = "0";
                    edit = false;
                    dgvOrder.Focus();
                    dgvOrder.ClearSelection();
                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Selected = true;
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
                    dgvOrder.BeginEdit(true);
                    edit = true;


                }
                else if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value)))
                {
                    MessageBox.Show("Please Enter Product Name");
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];
                    Txtitem.Focus();
                }
                else
                {
                    dgvOrder.Focus();
                    dgvOrder.ClearSelection();
                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Selected = true;
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2];
                    dgvOrder.BeginEdit(true);
                    edit = true;

                }



            }
        }
        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            itemdetails1();
        }
        public void itemdetails1()
        {

            try
            {

                string s1 = Txtitem.Text;
                string name = s1.Replace("'", "''");
                string loc = Convert.ToString(ddlLocation.SelectedValue);
                DataTable st = ProductMovementBal.itemdetails(name, loc);


                if (st.Rows.Count > 0)
                {

                    lblitemcode.Text = Convert.ToString(st.Rows[0]["ItemCode"]);
                    if (lblitemcode.Text == "")
                    {
                        lblitemcode.Text = "";
                    }
                    lblitemname.Text = Convert.ToString(st.Rows[0]["ItemName"]);
                    if (lblitemname.Text == "")
                    {
                        lblitemname.Text = string.Empty;
                    }

                    lblproductid.Text = Convert.ToString(st.Rows[0]["id"]);
                    if (lblproductid.Text == "")
                    {
                        lblproductid.Text = "";
                    }

                    lbldemo.Text = "0";
                    lblrack.Text = Convert.ToString(st.Rows[0]["Stock"]);
                    if (lblrack.Text == "")
                    {
                        lblrack.Text = "0";
                    }
                    lbldisplay.Text = "0";

                    lblservice.Text = "0";
                }

                else
                {
                    lblproductid.Text = "";
                    lblrack.Text = "0";


                }




            }
            catch (Exception)
            {

            }

        }

        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string headerText = dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].HeaderText;
            try
            {



                if (headerText == "Quantity")
                {
                    int quantity1;
                    if (Convert.ToString(Quantitytomove1.Text) == "" || Convert.ToInt32(Quantitytomove1.Text) == 0)
                    {
                        quantity1 = 1;
                    }
                    else
                    {

                        quantity1 = Convert.ToInt32(Quantitytomove1.Text);
                    }
                    dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex].Value = quantity1;
                }
                if (e.ColumnIndex == 2)
                {

                    dgvOrder.Focus();

                    dgvOrder.Rows.Add(1);
                    dgvOrder.Focus();

                    edit = false;
                    dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                    dgvOrder.Focus();
                    dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1].Selected = true;

                }
            }
            catch
            {

            }
        }

        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    pnsearch.Visible = true;
                    this.ActiveControl = Txtitem;
                    lblrowindex.Text = e.RowIndex.ToString();
                    lblpageno.Text = Convert.ToString(Convert.ToInt32(lblpageno.Text) + 1);
                }
                else
                {
                    pnsearch.Visible = false;
                }



            }
            catch
            {

            }

        }
        #region Panel
        protected void Quantitytomove1_press(object sender, KeyPressEventArgs e)
        {
            string headerText = dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].HeaderText;
            if (headerText == "Quantity")
            {



                if (headerText == "Quantity")
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
                    {
                        e.Handled = true;
                    }

                    // only allow one decimal point
                    if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                    {
                        e.Handled = true;
                    }
                }
            }
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
           // var combined = string.Join(", ", arr);
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

        #endregion
        #region Processcmd
        [DllImport("user32.dll")]
        internal static extern IntPtr GetFocus();
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.S | Keys.Control))
            {
                btnsave.PerformClick();


                return true;
            }

            else if (keyData == (Keys.C | Keys.Alt))
            {

                clear();

                return true;
            }

            if (keyData == Keys.Escape)
            {
                if (pnsearch.Visible)
                {
                    pnsearch.Visible = false;
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
                }
                else
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
                }




                return true;
            }

            if (ddlcustomers.Focused)
            {
                if (keyData == (Keys.Tab))
                {

                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[1, 0];

                    return true;
                }
            }
            if (keyData == Keys.Enter)
            {

                try
                {
                    IntPtr wndHandle = GetFocus();
                    Control focusedControl = FromChildHandle(wndHandle);

                    if (focusedControl.Name == "ddlLocation")
                    {
                        this.ActiveControl = ddlcustomers;
                    }
                    if (focusedControl.Name == "ddlcustomers")
                    {
                        dgvOrder.CurrentCell = dgvOrder[1, 0];
                    }



                }
                catch
                {

                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
        private void textbox_Change(object sender, EventArgs e)
        {

            if (dgvOrder.CurrentCell.ColumnIndex == 0)
            {

            }
        }
        private void dgvOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvOrder.CurrentCell.ColumnIndex;
            string headerText = dgvOrder.Columns[column].HeaderText;

            if (headerText.Equals("Items"))
            {
                tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;


                    tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    tb.TextChanged += new EventHandler(textbox_Change);
                }
            }

            else if (dgvOrder.CurrentCell.ColumnIndex == 2)
            {


                if (e.Control is TextBox)
                {
                    Quantitytomove1 = e.Control as TextBox;

                    Quantitytomove1.MaxLength = 4;
                    Quantitytomove1.KeyPress += new KeyPressEventHandler(Quantitytomove1_press);
                }

            }
        }

        private void dgvOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {

                e.SuppressKeyPress = true;
                if (dgvOrder.CurrentCell.ColumnIndex == 0)
                {
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex];

                }
                else if (dgvOrder.CurrentCell.ColumnIndex == 1)
                {
                    dgvOrder.Focus();
                    this.ActiveControl = Quantitytomove1;
                    dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];

                }
                else if (dgvOrder.CurrentCell.ColumnIndex == 2)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value)))
                        {
                            dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                        }
                        else
                        {
                            dgvOrder.Focus();
                            dgvOrder.Rows.Add(1);
                            dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                        }
                    }
                    catch
                    {
                        dgvOrder.Focus();
                        dgvOrder.Rows.Add(1);
                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                    }

                }
            }
            else
            {
                MessageBox.Show("Enter product to move");
            }
        }

        private void dgvOrder_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (edit == true)
                {
                    if (dgvOrder.CurrentCell.RowIndex >= 1)
                    {
                        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex - 1];
                        edit = false;
                    }
                    else if (dgvOrder.CurrentCell.RowIndex == 0)
                    {
                        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex - 1];
                    }

                }
            }
            catch
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
            lblproductid.Text = string.Empty;
            Txtitem.Text = string.Empty;
            lblitemcode.Text = "0";

            lbldisplay.Text = "0";
            lbldemo.Text = "0";
            lblservice.Text = "0";
            lblitemname.Text = "0";
            lblrack.Text = "0";
        }
        private void Dgvmovementsearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    string s = Convert.ToString(Dgvmovementsearch.Rows[Dgvmovementsearch.CurrentCell.RowIndex].Cells[0].Value);
                    if (MainTabSalesBill.SelectedIndex == 0)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            BindMaterialTranscationMain(s);
                            ddlcustomers.Focus();
                        }
                        else
                        {
                            clear();
                        }
                    }
                    else if (MainTabSalesBill.SelectedIndex == 1)
                    {


                        if (!string.IsNullOrEmpty(s))
                        {
                            getCheckout(s);
                            dgvFloorCheckOut.Focus();

                            //dgvFloorCheckOut.CurrentCell = dgvFloorCheckOut[6, 0];
                        }
                        else
                        {
                            clear();
                        }
                    }
                }
            }
            catch
            {

            }
        }
        public void getCheckout(string s)
        {
            DataTable ds = objIssuedReceivedBAL.BindMaterialTranscationMain(s);
            if (ds.Rows.Count > 0)
            {
                
                cmbflrRDloc.SelectedValue = ds.Rows[0]["LocationId"];
                cmbflrRDcus.Text = Convert.ToString(ds.Rows[0]["CustomerId"]);
                txtflrRDrefno.Text = Convert.ToString(ds.Rows[0]["Headid"]);
                HidLblMain.Text = Convert.ToString(ds.Rows[0]["Headid"]);

                txtRecRemarks.Text = Convert.ToString(ds.Rows[0]["Remarks"]);

                panel2.Enabled = false;
            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            if (ds.Rows.Count > 0)
            {
                dgvFloorCheckOut.Rows.Clear();
                for (int i = 0; i < ds.Rows.Count; i++)
                {



                    dgvFloorCheckOut.Rows.Add();
                    dgvFloorCheckOut.Rows[i].Cells[0].Value = i + 1;
                    dgvFloorCheckOut.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["Item"]);
                    dgvFloorCheckOut.Rows[i].Cells[2].Value = Convert.ToString(ds.Rows[i]["Quantity"]);
                    //dgvFloorCheckOut.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Rack"]);
                    dgvFloorCheckOut.Rows[i].Cells[5].Value = Convert.ToString(ds.Rows[i]["Productid"]);
                    //dgvFloorCheckOut.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["LocationID"]);



                    string vals = Convert.ToString(ds.Rows[i]["Location"]);
                    DataTable dt = getdatatable(vals);

                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).DataSource = dt;
                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).ValueMember = "id";
                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).DisplayMember = "location";
                    (dgvFloorCheckOut.Rows[i].Cells[3] as DataGridViewComboBoxCell).Value = dt.Rows[0][0];
                    string val = Convert.ToString(dt.Rows[0][0]);
                    string val1 = Convert.ToString(ds.Rows[i]["Productid"]);
                    string v = getrack(val, val1);
                    dgvFloorCheckOut.Rows[i].Cells["Rack"].Value = v;

                    dgvFloorCheckOut.Columns[3].ReadOnly=true;
                }
                panel2.Enabled = false;
            }
            else
            {
                dgvFloorCheckOut.Rows.Clear();
                panel2.Enabled = true;
            }

        }
        public DataTable getdatatable(string itemsToAdd)
        {
            DataTable table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("location", typeof(string));


            string[] item = itemsToAdd.Split(';');

            for (int i = 0; i < item.Length; i++)
            {
                string[] val = item[i].Split(',');
                table.Rows.Add(val[0], val[1]);
            }
            return table;
        }
        public string getrack(string s, string s1)
        {
            QuotationBal objQuotationbal = new QuotationBal();
            string v = objQuotationbal.getrack(s, s1);
            return v;
        }

        public void BindMaterialTranscationMain(string s)
        {

            DataTable ds = objIssuedReceivedBAL.BindMaterialTranscationMain(s);

            if (ds.Rows.Count > 0)
            {
                string typr=Convert.ToString(ds.Rows[0]["Type"]);
                if (typr == "Issued")
                {
                    Rbissued.Checked = true;
                  //  RBReceived.Checked = true;
                }
                else
                {
                    RBReceived.Checked = true;
                }
                   
                ddlLocation.SelectedValue = ds.Rows[0]["LocationId"];
                ddlcustomers.Text =Convert.ToString( ds.Rows[0]["CustomerId"]);
                txtrefno.Text = Convert.ToString(ds.Rows[0]["Headid"]);
                HidLblMain.Text = Convert.ToString(ds.Rows[0]["Headid"]);
                txtRemarks.Text = Convert.ToString(ds.Rows[0]["Remarks"]);
                dgvOrder.Rows.Clear();

                dgvOrder.Rows.Add(ds.Rows.Count);

                for (int i = 0; i < ds.Rows.Count; i++)
                {

                    dgvOrder.Rows[i].Cells[0].Value = i + 1;
                    dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["Item"]);
                    dgvOrder.Rows[i].Cells[2].Value = ds.Rows[i]["Quantity"];
                    dgvOrder.Rows[i].Cells[3].Value = ds.Rows[i]["ProductId"];
                    dgvOrder.Rows[i].Cells[4].Value = ds.Rows[i]["SubId"];
                    dgvOrder.Rows[i].Cells[5].Value = ds.Rows[i]["SubId"];


                }
                this.ActiveControl = ddlLocation;
            }
            else
            {
                dgvOrder.Rows.Clear();
                panel2.Enabled = true;
                pnsearch.Visible = true;
            }

        }
        #region Load
        private void FrmIssuedReceived_Load(object sender, EventArgs e)
        {
            search("EnteredBy", "", "EnteredOn", "Today", role1, Program.userid);

            this.ActiveControl = ddlLocation;
        }

        #endregion

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

        private void dgvFloorCheckOut_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex != 6)
                {
                    if (dgvFloorCheckOut[e.ColumnIndex, e.RowIndex].EditType.ToString() == "System.Windows.Forms.DataGridViewComboBoxEditingControl")
                    {
                        DataGridViewColumn column = dgvFloorCheckOut.Columns[e.ColumnIndex];
                        if (column is DataGridViewComboBoxColumn)
                        {
                            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dgvFloorCheckOut[e.ColumnIndex, e.RowIndex];
                            dgvFloorCheckOut.CurrentCell = cell;
                            dgvFloorCheckOut.BeginEdit(true);
                            DataGridViewComboBoxEditingControl editingControl = (DataGridViewComboBoxEditingControl)dgvFloorCheckOut.EditingControl;
                            editingControl.DroppedDown = true;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void dgvFloorCheckOut_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void MainTabSalesBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MainTabSalesBill.SelectedIndex == 0)
            {
                search("EnteredBy", "", "EnteredOn", "Today", role1, Program.userid);
                clear();
            }
            else if (MainTabSalesBill.SelectedIndex == 1)
            {
                search("EnteredBy", "", "EnteredOn", "Today", role1, Program.userid);
                clear();
            }
        }

        private void FrmReceived_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int j = 0; j < MainTabSalesBill.TabPages.Count; j++)
                {

                    for (int i = 0; i < MainTabSalesBill.TabPages[j].Controls.Count; i++)
                    {
                        MainTabSalesBill.TabPages[j].Controls[i].Dispose();
                    }
                }

                MainTabSalesBill.Dispose();

                this.Dispose();
            }
            catch
            {

            }
        }
    }
}
