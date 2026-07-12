
using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using System.Windows.Forms;


namespace Inventory.Masters
{
    public partial class Customers : Form
    {
        CustomerBAL ObjCustomerBal = new CustomerBAL();

        //string sourcepath;
        string filename = null;
        //string newnameimage;
        //string aaa;
        //static string imagepath;
        public TextBox tb;
        string filepath = string.Empty;
        string imgpath = string.Empty;
        int StateId, DistId;
        string conn = Program.connection;
        DateTimePicker dtpDOB;
        DateTimePicker dtpAnniversary;
        public Customers()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            AddCustomerDateFields();
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            //DataTable dt = GetTable();
            //dgvCustomerinfo.DataSource = dt;
            GetCustomer();
            bindState();
            bindDistrict();
            cmbbxState.SelectedIndex = 0;
            cmbbxDistrict.SelectedIndex = 0;
            cmbbxCity.SelectedIndex = 0;
            cmbstatus1.SelectedIndex = 0;
           cmbstatus2.SelectedIndex = 0;
            cmbstatus3.SelectedIndex = 0;
            //pnlOrder.Visible = false;
            //vLabel2.Visible = false;
            //pnlCollapse2.Visible = true;
            //splitContainer1.Panel2Collapsed = false;
            //pbxCollapse.Visible = true;
            //pbxRightCollapse.Visible = true;

            pnlLabelSearch.Visible = true;
            vLabel1.Visible = true;
            pnlSearch.Visible = false;
            splitContainer1.Panel1Collapsed = true;
            search("Name", "", "Phone", "", "pending", "");
            this.dgvSearch.Columns["CustomerID"].Visible = false;
            this.dgvSearch.Columns["Email"].Visible = false;
            this.dgvSearch.Columns["Fax"].Visible = false;
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
            search.Add("Name");
            search.Add("Phone");
            search.Add("status");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderNo.DataSource = bs.DataSource;
            cbxSearchOrderNo.SelectedIndex = 0;
            cmbstatus1.Visible = false;
        }

        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("Name");
            search.Add("Phone");
            search.Add("status");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxSearchOrderDate.DataSource = bs.DataSource;
            cbxSearchOrderDate.SelectedIndex = 1;
            cmbstatus2.Visible = false;
        }

        private void SearchCreteria3()
        {
            List<string> search = new List<string>();
            search.Add("Name");
            search.Add("Phone");
            search.Add("status");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
            cbxVendor.DataSource = bs.DataSource;
            cbxVendor.SelectedIndex = 2;
            txtsearch3.Visible = false;
        }



        private void vLabel1_Click(object sender, EventArgs e)
        {
            if (pnlLabelSearch.Visible == true)
            {
                pnlLabelSearch.Visible = false;
                vLabel1.Visible = false;
                pnlSearch.Visible = true;
                splitContainer1.Panel1Collapsed = false;
                cbxSearchOrderNo.Focus();
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


                this.dgvSearch.Columns["CustomerID"].Visible = true;
                this.dgvSearch.Columns["Email"].Visible = true;
                this.dgvSearch.Columns["Fax"].Visible = true;
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

                pnlLabelSearch.Visible = true;
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;

                this.dgvSearch.Columns["CustomerID"].Visible = false;
                this.dgvSearch.Columns["Email"].Visible = false;
                this.dgvSearch.Columns["Fax"].Visible = false;
            }
        }

        private void Btnprint_Click(object sender, EventArgs e)
        {
            preview();
        }

        public void preview()
        {
            //try
            //{
            //    int c = Convert.ToInt32(1);
            //    Purchasereport pos = new Purchasereport(c);
            //    pos.Show();
            //}
            //catch
            //{
            //    MessageBox.Show("Please Select The Item");
            //}
        }



        //private void SearchPurchaseOrder()
        //{

        //    dgvSearch.Rows.Clear();
        //    dgvSearch.Columns.Clear();
        //    dgvSearch.ColumnCount = 4;
        //    dgvSearch.RowCount = 16;

        //    dgvSearch.Columns[0].Name = "Customer Name";
        //    dgvSearch.Columns[1].Name = "Pnone No";
        //    dgvSearch.Columns[2].Name = "Email";
        //    dgvSearch.Columns[3].Name = "Contact Person ";



        //    this.dgvSearch.Columns[0].Width = 60;

        //    this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    this.dgvSearch.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    this.dgvSearch.Columns[1].Width = 60;


        //    this.dgvSearch.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        //    this.dgvSearch.Columns[2].Width = 60;



        //    this.dgvSearch.Columns[3].Width = 60;


        //    dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //    dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    foreach (DataGridViewColumn c in dgvSearch.Columns)
        //    {
        //        c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
        //    }

        //    this.dgvSearch.Columns[1].Visible = false;
        //    this.dgvSearch.Columns[2].Visible = false;
        //    this.dgvSearch.Columns[3].Visible = false;
        //}

        private void txtCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (char.IsLetter(e.KeyChar) || char.IsControl(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
            //{
            //    e.Handled = false;

            //}
            //else
            //{
            //    e.Handled = true;
            //}


        }

        private void txtAddressLine1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = false;

            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtAddressLine2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = false;

            }
            else
            {
                e.Handled = true;
            }
        }


        //private void txtCity_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (Char.IsLetter(e.KeyChar) || Char.IsControl(e.KeyChar) )
        //    {
        //        e.Handled = false;
        //    }
        //    else
        //    {
        //        e.Handled = true;
        //    }
        //}

        //private void txtdistrict_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (Char.IsLetter(e.KeyChar) || Char.IsControl(e.KeyChar))
        //    {
        //        e.Handled = false;
        //    }
        //    else
        //    {
        //        e.Handled = true;
        //    }

        //}

        private void txtPincode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = true;

            }
            else
            {
                e.Handled = false;
            }

        }

        private void txtContactName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar) || Char.IsControl(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }


        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtFax_KeyPress(object sender, KeyPressEventArgs e)
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



        private bool validation()
        {
            bool status = true;
            string message = "";
            int i = 0;
            if (string.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                message += "*Customer Name Should Not Be Empty " + "\n";
                i++;
                if (i == 1)
                    this.ActiveControl = txtCustomerName;

            }

            //if (string.IsNullOrEmpty(Convert.ToString(txtAddressLine1.Text)))
            //{
            //    message += "*Address Line1 Should Not Be Empty" + "\n";
            //    i++;
            //    if (i==1)
            //    {
            //        this.ActiveControl = txtAddressLine1;

            //    }
            //}
            //if (string.IsNullOrEmpty(txtAddressLine2.Text))
            //{
            //    i++;
            //    message += "*Address Line2 Should Not Be Empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtAddressLine2;
            //}
            //if (cmbbxState.SelectedIndex==0)
            //{
            //    message += "* Enter city name" + "\n";
            //    i++;
            //    if (i == 1)
            //        this.ActiveControl = cmbbxDistrict;
            //    cmbbxDistrict.Focus();

            //}
            ////if (cmbbxState.SelectedIndex != 0)
            ////{
            ////    if (cmbbxDistrict.SelectedIndex == 0)
            ////    {
            ////        message += "* Enter district name" + "\n";
            ////        i++;
            ////        if (i == 1)
            ////            this.ActiveControl = cmbbxDistrict;
            ////        cmbbxDistrict.Focus();

            ////    }
            ////}

            ////if (cmbbxDistrict.SelectedIndex != 0)
            ////{
            ////    if (cmbbxCity.SelectedIndex == 0)
            ////    {
            ////        message += "* Enter City name" + "\n";
            ////        i++;
            ////        if (i == 1)
            ////            this.ActiveControl = cmbbxCity;
            ////        cmbbxCity.Focus();

            ////    }
            ////}
            //if (string.IsNullOrEmpty(cmbbxCity.Text))
            //{
            //    message += "* Enter state" + "\n";
            //    i++;
            //    if (i == 1)
            //        this.ActiveControl = txtPincode;
            //    txtPincode.Focus();

            //}

            //if (cmbbxState.SelectedIndex==0)
            //{
            //    i++;
            //    message += "*Choose State" + "\n";
            //    if (i == 1)
            //    {
            //        this.ActiveControl = cmbbxState;

            //    }
            //}

            //if (cmbbxDistrict.SelectedIndex==0)
            //{
            //    i++;
            //    message += "*Choose District" + "\n";
            //    if (i == 1)
            //    {
            //        this.ActiveControl = cmbbxDistrict;

            //    }
            //}

            //if (cmbbxCity.SelectedIndex==0)
            //{
            //    i++;
            //    message += "* Choose City" + "\n";
            //    if (i == 1)
            //    {
            //        this.ActiveControl = cmbbxCity;

            //    }
            //}

            //if (string.IsNullOrEmpty(txtPincode.Text))
            //{
            //    i++;
            //    message += "*Pincode Should Not Be Empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtPincode;

            //}
            //if (string.IsNullOrEmpty(txtContactName.Text))
            //{
            //    i++;
            //    message += "*Contact Name Should Not Be Empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtContactName;

            //}
            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                i++;
                message += "*Phone Number Should Not Be Empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtPhone;
            }
            if (!string.IsNullOrEmpty(txtPhone.Text))
            {
                string ph = txtPhone.Text;
                if (ph.Length != 10)
                {
                    i++;
                    message += "*Phone Number Is Not Valid" + "\n";
                    if (i == 1)
                        this.ActiveControl = txtPhone;
                }
            }
            //if (string.IsNullOrEmpty(txtFax.Text))
            //{
            //    i++;
            //    message += "*Fax Number Should Not Be Empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtFax;
            //}
            //if (string.IsNullOrEmpty(txtEmail.Text))
            //{
            //    i++;
            //    message = message + "*Plesae Enter Email Id" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtEmail;
            //}

            System.Text.RegularExpressions.Regex rEMail = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
            if (!string.IsNullOrEmpty(txtEmail.Text))
            {
                if (!rEMail.IsMatch(txtEmail.Text))
                {
                    i++;
                    message += "* Enter Valid Email Id" + "\n";
                    if (i == 1)
                        this.ActiveControl = txtEmail;
                }
            }
            //if (string.IsNullOrEmpty(Convert.ToString(filename)))
            //{
            //    i++;
            //    message = message + "*Image should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = butbrowse;
            //}
            if (string.IsNullOrEmpty(Convert.ToString(message)))
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

        private void btnCustomerSave_Click(object sender, EventArgs e)
        {
            bool val = validation();
            if (val)
            {
                InsertUpdateRightToInformation();

            }
        }

        private void InsertUpdateRightToInformation()
        {
            int Status = 0;
            if (string.IsNullOrEmpty(Convert.ToString(lblhidden.Text)))
            {
                ObjCustomerBal.Id = "";
            }
            else
            {
                ObjCustomerBal.Id = lblhidden.Text;
            }


            ObjCustomerBal.Name = txtCustomerName.Text.Trim();
            ObjCustomerBal.Address1 = txtAddressLine1.Text;
            ObjCustomerBal.Address2 = txtAddressLine2.Text;
            if (cmbbxCity.SelectedIndex == 0)
            {
                ObjCustomerBal.CityId = string.Empty;

            }
            else
            {
                ObjCustomerBal.CityId = Convert.ToString(cmbbxCity.Text);
            }

            if (cmbbxState.SelectedIndex == 0)
            {
                ObjCustomerBal.StateId = string.Empty;

            }
            else
            {
                ObjCustomerBal.StateId = Convert.ToString(cmbbxState.SelectedValue);
            }

            if (cmbbxDistrict.SelectedIndex == 0)
            {
                ObjCustomerBal.DistrictId = string.Empty;

            }
            else
            {
                ObjCustomerBal.DistrictId = Convert.ToString(cmbbxDistrict.SelectedValue);
            }


            //ObjCustomerBal.StateId=Convert.ToString(cmbbxState.SelectedValue);
            // ObjCustomerBal.DistrictId=Convert.ToString(cmbbxDistrict.SelectedValue);
            ObjCustomerBal.Pincode = txtPincode.Text;
            ObjCustomerBal.ContactName = txtContactName.Text.Trim();
            ObjCustomerBal.Phone = txtPhone.Text;
            ObjCustomerBal.Email = txtEmail.Text;
            ObjCustomerBal.Fax = txtFax.Text;
            ObjCustomerBal.Path = filename;
            ObjCustomerBal.tin = Txttin.Text;
            ObjCustomerBal.reff = Txttin.Text;

            Status = CustomerBAL.SaveCustomer(ObjCustomerBal);
            if (Status == 1)
            {
                UpdateCustomerExtraFields(GetCurrentCustomerId());
            }

            if (Status == 1 && lblhidden.Text == string.Empty)
            {
                MessageBox.Show("Inserted successfully");
                if (!string.IsNullOrEmpty(filepath))
                {
                    try
                    {
                        File.Copy(filepath, imgpath);
                    }
                    catch
                    {

                    }
                }
                GetCustomer();
                search("Name", "", "Phone", "", "pending", "");
                clear();
            }

            else if (Status == 1 && lblhidden.Text != string.Empty)
            {
                MessageBox.Show("Updated Succesfully");
                if (!string.IsNullOrEmpty(filepath))
                {
                    try
                    {
                        File.Copy(filepath, imgpath);
                    }
                    catch
                    {

                    }
                }
                GetCustomer();
                search("Name", "", "Phone", "", "pending", "");
                clear();

            }
            else if (Status == 2)
            {
                MessageBox.Show("Customer already exist ");
                txtCustomerName.Focus();
            }

        }

        private void Insertpending()
        {
            int Status = 0;
            if (string.IsNullOrEmpty(Convert.ToString(lblhidden.Text)))
            {
                ObjCustomerBal.Id = "";
            }
            else
            {
                ObjCustomerBal.Id = lblhidden.Text;
            }


            ObjCustomerBal.Name = txtCustomerName.Text;
            ObjCustomerBal.Address1 = txtAddressLine1.Text;
            ObjCustomerBal.Address2 = txtAddressLine2.Text;
            //ObjCustomerBal.CityId=Convert.ToString(cmbbxCity.SelectedValue);
            //ObjCustomerBal.StateId=Convert.ToString(cmbbxState.SelectedValue);
            //ObjCustomerBal.DistrictId=Convert.ToString(cmbbxDistrict.SelectedValue);
            if (cmbbxCity.SelectedIndex == 0)
            {
                ObjCustomerBal.CityId = string.Empty;

            }
            else
            {
                ObjCustomerBal.CityId = Convert.ToString(cmbbxCity.SelectedValue);
            }

            if (cmbbxState.SelectedIndex == 0)
            {
                ObjCustomerBal.StateId = string.Empty;

            }
            else
            {
                ObjCustomerBal.StateId = Convert.ToString(cmbbxCity.SelectedValue);
            }

            if (cmbbxDistrict.SelectedIndex == 0)
            {
                ObjCustomerBal.DistrictId = string.Empty;

            }
            else
            {
                ObjCustomerBal.DistrictId = Convert.ToString(cmbbxCity.SelectedValue);
            }
            ObjCustomerBal.Pincode = txtPincode.Text;
            ObjCustomerBal.ContactName = txtContactName.Text;
            ObjCustomerBal.Phone = txtPhone.Text;
            ObjCustomerBal.Email = txtEmail.Text;
            ObjCustomerBal.Fax = txtFax.Text;
            ObjCustomerBal.Path = filename;

            Status = CustomerBAL.SaveCustomerPending(ObjCustomerBal);
            if (Status == 1)
            {
                UpdateCustomerExtraFields(GetCurrentCustomerId());
            }

            if (Status == 1 && lblhidden.Text == string.Empty)
            {
                MessageBox.Show("Inserted successfully");
                if (!string.IsNullOrEmpty(filepath))
                {
                    try
                    {
                        File.Copy(filepath, imgpath);
                    }
                    catch
                    {

                    }
                }
                GetCustomer();
                search("Name", "", "Phone", "", "pending", "");
                clear();
            }

            else if (Status == 1 && lblhidden.Text != string.Empty)
            {
                MessageBox.Show("Updated Succesfully");
                if (!string.IsNullOrEmpty(filepath))
                {
                    try
                    {
                        File.Copy(filepath, imgpath);
                    }
                    catch
                    {

                    }
                }
                GetCustomer();
                search("Name", "", "Phone", "", "pending", "");
                clear();

            }
            else if (Status == 2)
            {
                MessageBox.Show("Customer already exist ");
                txtCustomerName.Focus();
            }
        }

        public void GetCustomer()
        {

            DataTable dt = CustomerBAL.GetCustomer();


            dgvCustomerinfo.Columns.Clear();
            dgvCustomerinfo.DataSource = dt;
            dgvCustomerinfo.Columns["CustomerID"].Visible = false;

            //dgvCustomerinfo.Columns["Amount"].Visible = false;
            //dgvCustomerinfo.Columns["EnteredOn"].Visible = false;
            //dgvCustomerinfo.Columns["UpdatedOn"].Visible = false;
            //dgvCustomerinfo.Columns["UpdatedBy"].Visible = false;
            //dgvCustomerinfo.Columns["IsDelete"].Visible = false;


            DataGridViewImageColumn img1 = new DataGridViewImageColumn();
            img1.Image = Inventory.Properties.Resources.user_edit;
            dgvCustomerinfo.Columns.Insert(14, img1);
            img1.HeaderText = "Edit";
            img1.Name = "Edit";


            DataGridViewImageColumn img2 = new DataGridViewImageColumn();
            img2.Image = Inventory.Properties.Resources.trash;
            dgvCustomerinfo.Columns.Insert(15, img2);
            img2.HeaderText = "Delete";
            img2.Name = "Delete";

            dgvCustomerinfo.Columns["Edit"].Width = 40;
            dgvCustomerinfo.Columns["Delete"].Width = 60;
            dgvCustomerinfo.Columns["Path"].Visible = false;
            dgvCustomerinfo.Columns["Fax"].Visible = false;
            dgvCustomerinfo.Columns["Pincode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCustomerinfo.Columns["Phone"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCustomerinfo.Columns["Fax"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            dgvCustomerinfo.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvCustomerinfo.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvCustomerinfo.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


        }

        private string GetCurrentCustomerId()
        {
            if (!string.IsNullOrEmpty(lblhidden.Text))
                return lblhidden.Text;

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 CustomerID FROM Customers WHERE Name = @Name ORDER BY CustomerID DESC", con))
                {
                    cmd.Parameters.AddWithValue("@Name", txtCustomerName.Text.Trim());
                    return Convert.ToString(cmd.ExecuteScalar());
                }
            }
        }

        private void UpdateCustomerExtraFields(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                return;

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                EnsureCustomerExtraColumns(con);
                using (SqlCommand cmd = new SqlCommand("UPDATE Customers SET DOB = @DOB, AnniversaryDate = @AnniversaryDate WHERE CustomerID = @Id", con))
                {
                    AddDateParameter(cmd, "@DOB", dtpDOB);
                    AddDateParameter(cmd, "@AnniversaryDate", dtpAnniversary);
                    cmd.Parameters.AddWithValue("@Id", customerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadCustomerExtraFields(string customerId)
        {
            ClearDatePicker(dtpDOB);
            ClearDatePicker(dtpAnniversary);
            if (string.IsNullOrEmpty(customerId))
                return;

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                if (!ColumnExists(con, "Customers", "DOB") || !ColumnExists(con, "Customers", "AnniversaryDate"))
                    return;

                using (SqlCommand cmd = new SqlCommand("SELECT DOB, AnniversaryDate FROM Customers WHERE CustomerID = @Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", customerId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SetDatePicker(dtpDOB, reader["DOB"]);
                            SetDatePicker(dtpAnniversary, reader["AnniversaryDate"]);
                        }
                    }
                }
            }
        }

        private void EnsureCustomerExtraColumns(SqlConnection con)
        {
            EnsureColumn(con, "Customers", "DOB", "datetime NULL");
            EnsureColumn(con, "Customers", "AnniversaryDate", "datetime NULL");
        }

        private void AddDateParameter(SqlCommand cmd, string parameterName, DateTimePicker picker)
        {
            cmd.Parameters.AddWithValue(parameterName, picker.Checked ? (object)picker.Value.Date : DBNull.Value);
        }

        private void SetDatePicker(DateTimePicker picker, object value)
        {
            if (value == DBNull.Value || value == null)
            {
                ClearDatePicker(picker);
                return;
            }

            picker.Value = Convert.ToDateTime(value);
            picker.Checked = true;
            picker.Format = DateTimePickerFormat.Short;
        }

        private void ClearDatePicker(DateTimePicker picker)
        {
            picker.Checked = false;
            picker.Value = DateTime.Today;
            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = " ";
        }

        private void DatePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker picker = sender as DateTimePicker;
            if (picker == null)
                return;

            if (picker.Checked)
            {
                picker.Format = DateTimePickerFormat.Short;
            }
            else
            {
                picker.Format = DateTimePickerFormat.Custom;
                picker.CustomFormat = " ";
            }
        }

        private void EnsureColumn(SqlConnection con, string tableName, string columnName, string columnDefinition)
        {
            if (ColumnExists(con, tableName, columnName))
                return;

            using (SqlCommand cmd = new SqlCommand("ALTER TABLE dbo." + tableName + " ADD " + columnName + " " + columnDefinition, con))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private bool ColumnExists(SqlConnection con, string tableName, string columnName)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT COL_LENGTH('dbo." + tableName + "', '" + columnName + "')", con))
            {
                return cmd.ExecuteScalar() != DBNull.Value;
            }
        }

        private void txtstate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar) || Char.IsControl(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }

        private void btnclear_Click(object sender, EventArgs e) //clear
        {
            clear();
        }

        private void btnnew_Click(object sender, EventArgs e) //new
        {
            clear();
        }

        protected void clear()
        {
            txtCustomerName.Clear();
            txtAddressLine1.Clear();
            txtAddressLine2.Clear();
            cmbbxState.SelectedIndex = 0;
            cmbbxDistrict.SelectedIndex = 0;
            cmbbxCity.SelectedIndex = 0;

            //txtCity.Clear();
            //txtdistrict.Clear();
            //txtstate.Clear();

            txtPincode.Clear();
            txtContactName.Clear();
            txtPhone.Clear();
            txtFax.Clear();
            txtEmail.Clear();
            ClearDatePicker(dtpDOB);
            ClearDatePicker(dtpAnniversary);
            pcphoto.Image = null;
            filename = null;
            lblhidden.Text = string.Empty;
            Txttin.Text = string.Empty;
        }

        private void btnsaveaspending_Click(object sender, EventArgs e)
        {
            bool Val = Validationpending();
            if (Val == true)
            {
                Insertpending();
            }
        }

        public bool Validationpending()
        {
            bool status = true;
            string message = "";
            int i = 0;


            if (string.IsNullOrEmpty(Convert.ToString(txtCustomerName.Text)))
            {
                i++;
                message += "* CustomerName Should Not Be Empty" + "\n";
                if (i == 1)
                {
                    this.ActiveControl = txtCustomerName;

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

        private void butbrowse_Click(object sender, EventArgs e)
        {

            OpenFileDialog opFile = new OpenFileDialog();
            opFile.Title = "Select a Image";
            opFile.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";

            //string appPath = Path.GetPathRoot(@"/192.168.1.250./Hrms");
            string appPath = Program.path;
            //if (Directory.Exists(appPath) == false)
            //{
            //    Directory.CreateDirectory(appPath);
            //}
            if (opFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string iName = opFile.SafeFileName;
                    filepath = opFile.FileName;
                    imgpath = appPath + DateTime.Now.Ticks.ToString() + iName;
                    //File.Copy(filepath, imgpath);
                    filename = imgpath;
                    //filename = iName;
                    //lblimagepath.Text = imagePath;
                    pcphoto.Image = new Bitmap(opFile.OpenFile());
                    pcphoto.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Unable to open file " + exp.Message);
                }
            }
            else
            {
                opFile.Dispose();
            }


            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    Bitmap image1 = new Bitmap(openFileDialog.FileName);
            //    //sourcePath = openFileDialog.FileName;
            //    //fileName = Path.GetFileName(sourcePath);
            //    pcphoto.ImageLocation = openFileDialog.FileName;
            //    pcphoto.SizeMode = PictureBoxSizeMode.StretchImage;
            //    //imagePath = sourcePath;
            //}
        }

        private void butclear_Click(object sender, EventArgs e)
        {
            //pcphoto.ImageLocation = "";
            pcphoto.Image = null;
            filename = null;
        }

        private void dgvCustomerinfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "Edit")
                {
                    lblhidden.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["CustomerID"].Value);
                    txtCustomerName.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Name"].Value);
                    txtAddressLine1.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Address1"].Value);
                    txtAddressLine2.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Address2"].Value);
                    cmbbxState.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["State"].Value);
                    cmbbxDistrict.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["District"].Value);
                    cmbbxCity.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["City"].Value);


                    //txtCity.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["City"].Value);
                    //txtstate.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["State"].Value);
                    //txtdistrict.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["District"].Value);
                    txtContactName.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["ContactName"].Value);
                    txtPhone.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Phone"].Value);
                    txtPincode.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Pincode"].Value);
                    txtFax.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Fax"].Value);
                    txtEmail.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Email"].Value);
                    Txttin.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["TIN/CST"].Value);
                    filename = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Path"].Value);
                    LoadCustomerExtraFields(lblhidden.Text);


                    if (!string.IsNullOrEmpty(filename))
                    {
                        //pcphoto.Image = new Bitmap(imgpath);
                        pcphoto.ImageLocation = filename;
                        pcphoto.SizeMode = PictureBoxSizeMode.StretchImage;
                    }

                }

                else if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "Delete")
                {
                    string namestatus = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Name"].Value);
                    DialogResult result = MessageBox.Show("Are you sure delete Customer " + namestatus + "?", "Delete UOM?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.Yes))
                    {
                        lblhidden.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["CustomerID"].Value);
                        int i = Delete();
                        if (i == 1)
                        {
                            MessageBox.Show("Delete Successfully");
                            clear();
                            GetCustomer();

                        }
                    }

                }
            }
        }
        public int Delete()
        {
            int result = 0;
            result = CustomerBAL.DeleteCustomer(Convert.ToInt32(lblhidden.Text));
            return result;
        }

        public void bindState()
        {
            DataTable dtState = CustomerBAL.GetState();

            DataRow row = dtState.NewRow();
            row["StateId"] = "0";
            row["State"] = "--Select--";
            dtState.Rows.InsertAt(row, 0);
            cmbbxState.DataSource = dtState;
            cmbbxState.ValueMember = "StateId";
            cmbbxState.DisplayMember = "State";
            cmbbxState.SelectedIndex = 0;

        }

        public void bindDistrict()
        {
            StateId = 0;

            if (cmbbxState.SelectedIndex != 0)
            {
                StateId = Convert.ToInt32(cmbbxState.SelectedValue);

            }


            DataTable dtDistrict = CustomerBAL.GetDistrict(StateId);
            DataRow row = dtDistrict.NewRow();
            row["DistrictId"] = "0";
            row["DISTRICT"] = "--Select--";
            dtDistrict.Rows.InsertAt(row, 0);
            cmbbxDistrict.DataSource = dtDistrict;
            cmbbxDistrict.ValueMember = "DistrictId";
            cmbbxDistrict.DisplayMember = "DISTRICT";
            cmbbxDistrict.SelectedIndex = 0;

        }

        private void AddCustomerDateFields()
        {
            AddDateField("lblDOB", "Date Of Birth", "dtpDOB", 18, 210, out dtpDOB);
            AddDateField("lblAnniversary", "Anniversary Date", "dtpAnniversary", 330, 210, out dtpAnniversary);
            dgvCustomerinfo.Location = new Point(dgvCustomerinfo.Location.X, dgvCustomerinfo.Location.Y + 30);
            dgvCustomerinfo.Height = Math.Max(100, dgvCustomerinfo.Height - 30);
        }

        private void AddDateField(string labelName, string labelText, string pickerName, int x, int y, out DateTimePicker picker)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Font = new Font("Calibri", 9.75F);
            label.Location = new Point(x, y + 3);
            label.Name = labelName;
            label.Text = labelText;

            picker = new DateTimePicker();
            picker.Checked = false;
            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = " ";
            picker.Location = new Point(x + 120, y);
            picker.Name = pickerName;
            picker.ShowCheckBox = true;
            picker.Size = new Size(120, 20);
            picker.ValueChanged += new EventHandler(DatePicker_ValueChanged);

            pnlmain.Controls.Add(label);
            pnlmain.Controls.Add(picker);
        }

        private void Customers_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtCustomerName;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (butclear.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnCustomerSave;
                    return true;
                }

            }
            if (cbxSearchOrderNo.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    if (cmbstatus1.Visible == true)
                    {
                        this.ActiveControl = cmbstatus1;
                        return true;
                    }
                    else
                    {
                        this.ActiveControl = txtsearch1;
                        return true;
                    }

                }

            }
            if (cmbstatus1.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = cbxSearchOrderDate;
                    return true;
                }
            }
            if (txtsearch1.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = cbxSearchOrderDate;
                    return true;
                }
            }
            //cmbstatus2
            if (cbxSearchOrderDate.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    if (cmbstatus2.Visible == true)
                    {
                        this.ActiveControl = cmbstatus2;
                        return true;
                    }
                    else
                    {
                        this.ActiveControl = txtsearch2;
                        return true;
                    }

                }

            }
            if (cmbstatus2.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = cbxVendor;
                    return true;
                }
            }
            if (txtsearch2.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = cbxVendor;
                    return true;
                }
            }
            //cmbstatus2
            if (cbxVendor.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    if (cmbstatus3.Visible == true)
                    {
                        this.ActiveControl = cmbstatus3;
                        return true;
                    }
                    else
                    {
                        this.ActiveControl = txtsearch3;
                        return true;
                    }

                }

            }
            if (cmbstatus3.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnSearch;
                    return true;
                }
            }
            if (txtsearch3.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnSearch;
                    return true;
                }
            }

            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            if (keyData == Keys.F3)
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
                    //if (!string.IsNullOrEmpty(txtprodsearch.Text))
                    //{
                        GetCustomerSearch();
                   // }
                }
            }
         
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void cmbbxState_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindDistrict();
        }

        private void cmbbxDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindcity();
        }

        public void bindcity()
        {
            DistId = 0;
            if (cmbbxDistrict.SelectedIndex != 0)
            {
                DistId = Convert.ToInt32(cmbbxDistrict.SelectedValue);
            }
            DataTable dtCity = CustomerBAL.GetCity(StateId, DistId);
            DataRow row = dtCity.NewRow();
            row["CityId"] = "0";
            row["City"] = "--Select--";
            dtCity.Rows.InsertAt(row, 0);
            cmbbxCity.DataSource = dtCity;
            cmbbxCity.ValueMember = "CityId";
            cmbbxCity.DisplayMember = "City";
            cmbbxCity.SelectedIndex = 0;

        }

        private void dgvCustomerinfo_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                return;
            }
            var dataGridView = (sender as DataGridView);
            //Check the condition as per the requirement casting the cell value to the appropriate type
            if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
                dataGridView.Cursor = Cursors.Hand;
            else
                dataGridView.Cursor = Cursors.Default;
        }

        private void dgvCustomerinfo_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "Name")
            {
                dgvCustomerinfo.Columns[e.ColumnIndex].Width = 200;
            }
            if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "Address1")
            {
                dgvCustomerinfo.Columns[e.ColumnIndex].Width = 250;
            }
            if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "Address2")
            {
                dgvCustomerinfo.Columns[e.ColumnIndex].Width = 250;
            }
            if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "City")
            {
                dgvCustomerinfo.Columns[e.ColumnIndex].Width = 150;
            }
            if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "District")
            {
                dgvCustomerinfo.Columns[e.ColumnIndex].Width = 150;
            }
            if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "State")
            {
                dgvCustomerinfo.Columns[e.ColumnIndex].Width = 150;
            }
            if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "Pincode")
            {
                dgvCustomerinfo.Columns[e.ColumnIndex].Width = 70;
            }
            if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "ContactName")
            {
                dgvCustomerinfo.Columns[e.ColumnIndex].Width = 100;
            }
            if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "Phone")
            {
                dgvCustomerinfo.Columns[e.ColumnIndex].Width = 90;
            }
            if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "Email")
            {
                dgvCustomerinfo.Columns[e.ColumnIndex].Width = 200;
            }
            if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "Fax")
            {
                dgvCustomerinfo.Columns[e.ColumnIndex].Width = 100;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if ((cbxSearchOrderNo.SelectedIndex == cbxSearchOrderDate.SelectedIndex) || cbxSearchOrderNo.SelectedIndex == cbxVendor.SelectedIndex || cbxSearchOrderDate.SelectedIndex == cbxVendor.SelectedIndex)
            {
                MessageBox.Show("Search item Should Not Be Same");
            }
            else
            {
                string firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue;

                firstname = cbxSearchOrderNo.Text.Trim();
                if (firstname == "status")
                {
                    firstname = "pending";
                    firstvalue = Convert.ToString(cmbstatus1.SelectedIndex);
                }
                else
                {
                    firstvalue = txtsearch1.Text.Trim();
                }

                secondname = cbxSearchOrderDate.Text.Trim();
                if (secondname == "status")
                {
                    secondname = "pending";
                    secondvalue = Convert.ToString(cmbstatus2.SelectedIndex);
                }
                else
                {
                    secondvalue = txtsearch2.Text.Trim();
                }


                thirdname = cbxVendor.Text.Trim();
                if (thirdname == "status")
                {
                    thirdname = "pending";
                    thirdvalue = Convert.ToString(cmbstatus3.SelectedIndex);
                }
                else
                {
                    thirdvalue = txtsearch3.Text.Trim();
                }

                search(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue);
            }
        }

        public void search(string firstname, string firstvalue, string secondname, string secondvalue, string thirdname, string thirdvalue)
        {
            DataTable dt = CustomerBAL.SearchCustomer(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue);
            dgvSearch.Columns.Clear();
            dgvSearch.DataSource = dt;

            dgvSearch.Columns["Address1"].Visible = false;
            dgvSearch.Columns["Address2"].Visible = false;
            dgvSearch.Columns["State"].Visible = false;
            dgvSearch.Columns["District"].Visible = false;
            dgvSearch.Columns["City"].Visible = false;
            dgvSearch.Columns["Pincode"].Visible = false;
            dgvSearch.Columns["ContactName"].Visible = false;
            dgvSearch.Columns["Email"].Visible = false;
            dgvSearch.Columns["Phone"].Visible = false;
            dgvSearch.Columns["Fax"].Visible = false;
            dgvSearch.Columns["Path"].Visible = false;
            dgvSearch.Columns["tin"].Visible = false;


            dgvSearch.Columns["CustomerID"].Visible = false;




            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void dgvSearch_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                return;
            }
            var dataGridView = (sender as DataGridView);
            //Check the condition as per the requirement casting the cell value to the appropriate type

            dataGridView.Cursor = Cursors.Hand;

        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {

                lblhidden.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["CustomerID"].Value);
                txtCustomerName.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Name"].Value);
                txtAddressLine1.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Address1"].Value);
                txtAddressLine2.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Address2"].Value);
                //cmbbxState.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["State"].Value);
                //cmbbxDistrict.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["District"].Value);
                //cmbbxCity.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["City"].Value);
                if (string.IsNullOrEmpty(Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["State"].Value)))
                {
                    cmbbxState.SelectedIndex = 0;
                }
                else
                {
                    cmbbxState.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["State"].Value);
                }
                if (string.IsNullOrEmpty(Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["District"].Value)))
                {
                    cmbbxDistrict.SelectedIndex = 0;
                }
                else
                {
                    cmbbxDistrict.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["District"].Value);
                }
                if (string.IsNullOrEmpty(Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["City"].Value)))
                {
                    cmbbxCity.SelectedIndex = 0;
                }
                else
                {
                    cmbbxCity.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["City"].Value);
                }

                txtPincode.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Pincode"].Value);
                txtContactName.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ContactName"].Value);
                txtEmail.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Email"].Value);
                txtPhone.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Phone"].Value);
                txtFax.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Fax"].Value);
                Txttin.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["tin"].Value);
                filename = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Path"].Value);
                LoadCustomerExtraFields(lblhidden.Text);
                if (!string.IsNullOrEmpty(filename))
                {
                    //pcphoto.Image = new Bitmap(imgpath);
                    pcphoto.ImageLocation = filename;
                    pcphoto.SizeMode = PictureBoxSizeMode.StretchImage;
                }


            }
        }

        private void cbxSearchOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSearchOrderNo.SelectedIndex == 2)
            {
                cmbstatus1.Visible = true;
                txtsearch1.Visible = false;
            }
            else
            {
                cmbstatus1.Visible = false;
                txtsearch1.Visible = true;
            }
        }

        private void cbxSearchOrderDate_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbxSearchOrderDate.SelectedIndex == 2)
            {
                cmbstatus2.Visible = true;
                txtsearch2.Visible = false;
            }
            else
            {
                cmbstatus2.Visible = false;
                txtsearch2.Visible = true;
            }
        }

        private void cbxVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxVendor.SelectedIndex == 2)
            {
                cmbstatus3.Visible = true;
                txtsearch3.Visible = false;
            }
            else
            {
                cmbstatus3.Visible = false;
                txtsearch3.Visible = true;
            }
        }

        private void txtPincode_KeyPress_1(object sender, KeyPressEventArgs e)
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

        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
            GetCustomerSearch();
        }
        public void GetCustomerSearch()
        {

            DataTable dt = CustomerBAL.GetCustomerSearch(txtprodsearch.Text);


            dgvCustomerinfo.Columns.Clear();
            dgvCustomerinfo.DataSource = dt;
            dgvCustomerinfo.Columns["CustomerID"].Visible = false;

            //dgvCustomerinfo.Columns["Amount"].Visible = false;
            //dgvCustomerinfo.Columns["EnteredOn"].Visible = false;
            //dgvCustomerinfo.Columns["UpdatedOn"].Visible = false;
            //dgvCustomerinfo.Columns["UpdatedBy"].Visible = false;
            //dgvCustomerinfo.Columns["IsDelete"].Visible = false;


            DataGridViewImageColumn img1 = new DataGridViewImageColumn();
            img1.Image = Inventory.Properties.Resources.user_edit;
            dgvCustomerinfo.Columns.Insert(14, img1);
            img1.HeaderText = "Edit";
            img1.Name = "Edit";


            DataGridViewImageColumn img2 = new DataGridViewImageColumn();
            img2.Image = Inventory.Properties.Resources.trash;
            dgvCustomerinfo.Columns.Insert(15, img2);
            img2.HeaderText = "Delete";
            img2.Name = "Delete";

            dgvCustomerinfo.Columns["Edit"].Width = 40;
            dgvCustomerinfo.Columns["Delete"].Width = 60;
            dgvCustomerinfo.Columns["Path"].Visible = false;
            dgvCustomerinfo.Columns["Fax"].Visible = false;
            dgvCustomerinfo.Columns["Pincode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCustomerinfo.Columns["Phone"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCustomerinfo.Columns["Fax"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            dgvCustomerinfo.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvCustomerinfo.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvCustomerinfo.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            pnlprodsearch.Visible = false;
        }

        private void label25_Click(object sender, EventArgs e)
        {

        }
    }
}
























































