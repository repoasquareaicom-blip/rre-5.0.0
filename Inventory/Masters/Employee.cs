using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using System.Windows.Forms;


namespace Inventory.Masters
{
    public partial class Employee : Form
    {
        public TextBox tb;
        ReferenceBAL ObjReferenceBal = new ReferenceBAL();
        public Button btn;
        int StateId, DistId;
        string conn = Program.connection;
        string filename = null;
        string newnameimage;
        string sourcepath;
        static string imagepath;
        string filepath = string.Empty;
        string imgpath = string.Empty;
       
        public Employee()
        {
            
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;         
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            //DataTable dt = GetTable();
            //dgvCustomerinfo.DataSource = dt;
            bindState();
            bindDistrict();
            GetProduct();
            cmbbxState.SelectedIndex = 0;
            cmbbxDistrict.SelectedIndex = 0;
           // cmbbxCity.SelectedIndex = 0;
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

            search("Name", "", "Phone", "", "ispending", "");
            this.dgvSearch.Columns["employeeid"].Visible = false;
            this.dgvSearch.Columns["Email"].Visible = false;
            this.dgvSearch.Columns["Fax"].Visible = false;
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
                this.dgvSearch.Columns["employeeid"].Visible = true;
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
                this.dgvSearch.Columns["employeeid"].Visible = false;
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

        private void btnclear_Click(object sender, EventArgs e)//clear
        {
            clear();
        }

        private void btnsave_Click(object sender, EventArgs e)//save
        {
            bool val= Validation();
            if(val)
            {
                InsertUpdateRightToInformation();
            }
        }

        private void InsertUpdateRightToInformation()
        {
            int Status = 0;
            if (string.IsNullOrEmpty(Convert.ToString(lblhidden.Text)))
            {
                ObjReferenceBal.Id = "";
            }
            else
            {
                ObjReferenceBal.Id = lblhidden.Text;
            }


            ObjReferenceBal.Name = txtCustomerName.Text.Trim();
            ObjReferenceBal.Address1 = txtAddressLine1.Text;
            ObjReferenceBal.Address2 = txtAddressLine2.Text;
            ObjReferenceBal.CityId = Convert.ToString(cmbbxCity.Text);
            ObjReferenceBal.StateId = Convert.ToString(cmbbxState.SelectedValue);
            ObjReferenceBal.DistrictId = Convert.ToString(cmbbxDistrict.SelectedValue);
            ObjReferenceBal.Pincode = txtPincode.Text;
            ObjReferenceBal.Phone = txtPhone.Text;
            ObjReferenceBal.Email = txtEmail.Text;
            ObjReferenceBal.Fax = txtFax.Text;
            ObjReferenceBal.Path = filename;

            Status = ReferenceBAL.SaveEmpoyee(ObjReferenceBal);

            if (Status == 1 && lblhidden.Text == string.Empty)
            {
                MessageBox.Show("Inserted successfully");
                if (!string.IsNullOrEmpty(filepath))
                {
                    File.Copy(filepath, imgpath);
                }
                GetProduct();
                search("Name", "", "Phone", "", "ispending", "");
                clear();

            }
            else if (Status == 1 && lblhidden.Text != string.Empty)
            {
                MessageBox.Show("Updated Succesfully");
                if (!string.IsNullOrEmpty(filepath))
                {
                    try{
                    File.Copy(filepath, imgpath);
                }
                    catch
                    {

                    }
                }
                GetProduct();
                search("Name", "", "Phone", "", "ispending", "");
                clear();
            }
            else if (Status == 2)
            {
                MessageBox.Show("Already exist ");
                txtCustomerName.Focus();
            }
            

        }
        private void Insertpending()
        {
            int Status = 0;
            if (string.IsNullOrEmpty(Convert.ToString(lblhidden.Text)))
            {
                ObjReferenceBal.Id = "";
            }
            else
            {
                ObjReferenceBal.Id = lblhidden.Text;
            }


            ObjReferenceBal.Name = txtCustomerName.Text;
            ObjReferenceBal.Address1 = txtAddressLine1.Text;
            ObjReferenceBal.Address2 = txtAddressLine2.Text;
            ObjReferenceBal.CityId = Convert.ToString(cmbbxCity.Text);
            ObjReferenceBal.StateId = Convert.ToString(cmbbxState.SelectedValue);
            ObjReferenceBal.DistrictId = Convert.ToString(cmbbxDistrict.SelectedValue);
            ObjReferenceBal.Pincode = txtPincode.Text;
            ObjReferenceBal.Phone = txtPhone.Text;
            ObjReferenceBal.Email = txtEmail.Text;
            ObjReferenceBal.Fax = txtFax.Text;
            ObjReferenceBal.Path = filename;

            Status = ReferenceBAL.SaveEmpoyeePending(ObjReferenceBal);

            if (Status == 1 && lblhidden.Text == string.Empty)
            {
                MessageBox.Show("Inserted successfully");
                if (!string.IsNullOrEmpty(filepath))
                {
                    File.Copy(filepath, imgpath);
                }
                GetProduct();
                search("Name", "", "Phone", "", "ispending", "");
                clear();
            }

            else if (Status == 1 && lblhidden.Text != string.Empty)
            {
                MessageBox.Show("Updated Succesfully");
                if (!string.IsNullOrEmpty(filepath))
                {
                    File.Copy(filepath, imgpath);
                }
                GetProduct();
                search("Name", "", "Phone", "", "ispending", "");
                clear();

            }
            else if (Status == 2)
            {
                MessageBox.Show("Already exist ");
                txtCustomerName.Focus();
            }
        }


        public void GetProduct()
        {
            DataTable dt = ReferenceBAL.GetEmpoyee();

            dgvCustomerinfo.Columns.Clear();
            dgvCustomerinfo.DataSource = dt;
            dgvCustomerinfo.Columns["employeeid"].Visible = false;

            DataGridViewImageColumn img1 = new DataGridViewImageColumn();
            img1.Image = Inventory.Properties.Resources.user_edit;
            dgvCustomerinfo.Columns.Insert(12, img1);
            img1.HeaderText = "Edit";
            img1.Name = "Edit";

            DataGridViewImageColumn img2 = new DataGridViewImageColumn();
            img2.Image = Inventory.Properties.Resources.trash;
            dgvCustomerinfo.Columns.Insert(13, img2);
            img2.HeaderText = "Delete";
            img2.Name = "Delete";

            //dgvCustomerinfo.Columns["Name"].Width = 20;
            dgvCustomerinfo.Columns["Edit"].Width = 40;
            dgvCustomerinfo.Columns["Delete"].Width = 60;



            dgvCustomerinfo.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvCustomerinfo.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvCustomerinfo.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }

        private void btnsaveaspending_Click(object sender, EventArgs e)//save Bending
        {
            bool Val = Validationpending();
            if (Val == true)
            {
                Insertpending();
            }
        }

        public bool 
            Validationpending()
        {
            bool status = true;
            string message = "";
            int i = 0;


            if (string.IsNullOrEmpty(Convert.ToString(txtCustomerName.Text)))
            {
                    i++;
                    message += "* Employee Should Not Be Empty" + "\n";
                if (i == 1)
                {
                    this.ActiveControl = txtCustomerName;
                   
                }
            }
            if (!string.IsNullOrEmpty(txtPhone.Text))
            {
                string ph = txtPhone.Text;
                if (ph.Length != 10)
                {
                    i++;
                    message = message + "* Contact No Is Not Valid" + "\n";
                    if (i == 1)
                        this.ActiveControl = txtPhone;
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
            //if (!string.IsNullOrEmpty(txtCustomerName.Text))
            //{
            //    Insertpending();
            //}
            //else
            //{
            //    MessageBox.Show("CustomerName Should Not Be Empty");
            //}
        }

        //private bool savevalidation()
        //{
        //    bool status = true;
        //    string mgs = "";
        //    int i = 0;

           
        //    if (txtCustomerName.Text == "")
        //    {
        //        i++;
        //        MessageBox.Show("Name Field cannnot be blank");
        //        if(i==1)
        //        txtCustomerName.Focus();
        //    }
        //    if (txtAddressLine1.Text == "")
        //    {
        //        MessageBox.Show("Address Field cannnot be blank");
        //        txtAddressLine1.Focus();
        //    }
        //    if (txtcity.Text == "")
        //    {
        //        MessageBox.Show("City Field cannnot be blank");
        //        txtcity.Focus();
        //    }
        //    if (txtPincode.Text == "")
        //    {
        //        MessageBox.Show("Pincode Field cannnot be blank");
        //        txtPincode.Focus();
        //    }
        //    if (txtEmail.Text == "")
        //    {
        //        MessageBox.Show("Email Field cannnot be blank");
        //        txtEmail.Focus();
        //    }
        //    if (txtPhone.Text == "")
        //    {
        //        MessageBox.Show("Mobile Field cannnot be blank!!");
        //        txtPhone.Focus();
        //    }
        //    Regex re = new Regex("^[0-9]{9}");
        //    if (re.IsMatch(txtPhone.Text.Trim()) == false || txtPhone.Text.Length > 10)
        //    {
        //        MessageBox.Show("Invalid Indian Mobile Number !!");
        //        txtPhone.Focus();
        //    }
        //    Regex MYRE = new Regex(@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
        //    if (MYRE.IsMatch(txtEmail.Text))
        //    {
        //        MessageBox.Show("Email Field not Correct");
        //        txtEmail.Focus();

        //    }
        //    return status;
 
        //}

        private void clear()
        {
            txtCustomerName.Clear();
            txtAddressLine1.Clear();
            txtAddressLine2.Clear();
            cmbbxState.SelectedIndex = 0;
            cmbbxDistrict.SelectedIndex = 0;
            cmbbxCity.Text = string.Empty;
            //txtcity.Clear();
            //txtdistrict.Clear();
            //txtstate.Clear();
            txtPincode.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtFax.Clear();
            pcphoto.Image = null;
            filename = null;
            lblhidden.Text = string.Empty;
            
        }

        private void button2_Click(object sender, EventArgs e)// New Button click
        {
            clear();
        }

        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                i++;
                message = message + "* Employee Name Should Not Be Empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtCustomerName;
            }

            //if (string.IsNullOrEmpty(txtAddressLine1.Text))
            //{
            //    i++;
            //    message = message + "* Address Line1 Should Not Be Empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtAddressLine1;
            //}

            //if (string.IsNullOrEmpty(txtAddressLine2.Text))
            //{
            //    i++;
            //    message = message + "* Address Line2 Should Not Be Empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtAddressLine2;
            //}


            //if (string.IsNullOrEmpty(txtcity.Text))
            //{
            //    i++;
            //    message = message + "*Enter city" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtcity;
            //}


            //if (string.IsNullOrEmpty(txtdistrict.Text))
            //{
            //    i++;
            //    message = message + "*Enter district" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtdistrict;
            //}
            //if (string.IsNullOrEmpty(txtstate.Text))
            //{
            //    i++;
            //    message = message + "*Enter state" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtstate;
            //}
            //if (string.IsNullOrEmpty(txtEmail.Text))
            //{
            //    i++;
            //    message = message + "*Enter Email" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtEmail;
            //}

            //if (cmbbxState.SelectedIndex == 0)
            //{ 
            //    i++;
            //    message += "* Choose State" + "\n";
            //    if (i == 1)
            //    {
            //        this.ActiveControl = cmbbxState;
                    
            //    }
            //}

            //if (cmbbxDistrict.SelectedIndex == 0)
            //{
            //    i++;
            //    message += "* Choose District" + "\n";
            //    if (i == 1)
            //    {
            //        this.ActiveControl = cmbbxDistrict;
                   
            //    }
            //}

            //if (cmbbxCity.SelectedIndex == 0)
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
            //    message = message + "* Pincode Should Not Be Empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtPincode;
            //}
            //if (string.IsNullOrEmpty(txtEmail.Text))
            //{
            //    i++;
            //    message = message + "* Plesae Enter Email Id" + "\n";
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
            if (!string.IsNullOrEmpty(txtPhone.Text))
            {
                string ph = txtPhone.Text;
                if (ph.Length != 10)
                {
                    i++;
                    message = message + "* Contact No Is Not Valid" + "\n";
                    if (i == 1)
                        this.ActiveControl = txtPhone;
                }
            }
            //if (string.IsNullOrEmpty(txtPhone.Text))
            //{
            //    i++;
            //    message = message + "* Contact No Should Not Be Empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtPhone;
            //}
            //if (string.IsNullOrEmpty(txtFax.Text))
            //{
            //    i++;
            //    message += "* Fax Number Should Not Be Empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = txtFax;
            //}
            //if (string.IsNullOrEmpty(Convert.ToString(filename)))
            //{
            //    i++;
            //    message = message + "* Image should not empty" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = butbrowse;
            //}


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

            //if (!string.IsNullOrEmpty(message))
            //{
            //    MessageBox.Show(message);
            //    status = false;
            //}
            //return status;
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

        private void txtCustomerName_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void txtPhone_KeyPress_1(object sender, KeyPressEventArgs e)
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

       
        private void butbrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            string appPath = Program.path;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
             try
              {
                  string iName = openFileDialog.SafeFileName;
                  filepath = openFileDialog.FileName;
                  imgpath = appPath + DateTime.Now.Ticks.ToString() + iName;
                  //File.Copy(filepath, imgpath);
                  filename = imgpath;
                  pcphoto.Image = new Bitmap(openFileDialog.OpenFile());
                  pcphoto.SizeMode = PictureBoxSizeMode.StretchImage;
              }
              catch (Exception exp)
              {
                  MessageBox.Show("Unable to open file " + exp.Message);
              }
          }
          else
          {
              openFileDialog.Dispose();
          }
        }

        private void butclear_Click(object sender, EventArgs e)
        {
            pcphoto.Image = null;
            filename = null;
        }

        private void dgvCustomerinfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                if (dgvCustomerinfo.Columns[e.ColumnIndex].HeaderText == "Edit")
                {
                    lblhidden.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["employeeid"].Value);
                    txtCustomerName.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Name"].Value);
                    txtAddressLine1.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Address1"].Value);
                    txtAddressLine2.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Address2"].Value);
                    cmbbxState.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["State"].Value);
                    cmbbxDistrict.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["District"].Value);
                    cmbbxCity.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["City"].Value);
                    //txtcity.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["City"].Value);
                    //txtstate.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["State"].Value);
                    //txtdistrict.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["District"].Value);
                    txtPincode.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Pincode"].Value);
                    txtEmail.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Email"].Value);
                    txtPhone.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Phone"].Value);
                    txtFax.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Fax"].Value);
                    filename = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["Path"].Value);

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
                    DialogResult result = MessageBox.Show("Are you sure delete Employee " + namestatus + "?", "deleteEmployee?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.Yes))
                    {
                        lblhidden.Text = Convert.ToString(dgvCustomerinfo.Rows[e.RowIndex].Cells["ReferencesID"].Value);
                        int i = Delete();
                        if (i == 1)
                        {
                            MessageBox.Show("Delete Successfully");
                            clear();
                            GetProduct();
                        }
                    }
                }
            }
        }
        public int Delete()
        {
            int result = 0;
            result = ReferenceBAL.deleteEmployee(Convert.ToInt32(lblhidden.Text));
            return result;
        }

        public void bindState()
        {
            DataTable dtState = ReferenceBAL.GetState();

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

            DataTable dtDistrict = ReferenceBAL.GetDistrict(StateId);
            DataRow row = dtDistrict.NewRow();
            row["DistrictId"] = "0";
            row["DISTRICT"] = "--Select--";
            dtDistrict.Rows.InsertAt(row, 0);
            cmbbxDistrict.DataSource = dtDistrict;
            cmbbxDistrict.ValueMember = "DistrictId";
            cmbbxDistrict.DisplayMember = "DISTRICT";
            cmbbxDistrict.SelectedIndex = 0;

        }

        private void cmbbxState_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindDistrict();
        }

        private void cmbbxDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            //bindcity();
        }

       
        //public void bindcity()
        //{
        //    DistId = 0;
        //    if (cmbbxDistrict.SelectedIndex != 0)
        //    {
        //        DistId = Convert.ToInt32(cmbbxDistrict.SelectedValue);
        //    }

        //    DataTable dtCity = ReferenceBAL.GetCity(StateId, DistId);
        //    DataRow row = dtCity.NewRow();
        //    row["CityId"] = "0";
        //    row["City"] = "--Select--";
        //    dtCity.Rows.InsertAt(row, 0);
        //    cmbbxCity.DataSource = dtCity;
        //    cmbbxCity.ValueMember = "CityId";
        //    cmbbxCity.DisplayMember = "City";
        //    cmbbxCity.SelectedIndex = 0;

        //}

        private void Reference_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtCustomerName;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (butclear.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnsave;
                    return true;
                }

            }
            if (Btnprint.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = txtCustomerName;
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
                    
                        GetProductSearch();
                   
                }
            }
         
            return base.ProcessCmdKey(ref msg, keyData);
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
                MessageBox.Show("* Search a item Should Not Be Same");
            }
            else
            {
                string firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue;

                firstname = cbxSearchOrderNo.Text.Trim();
                if (firstname == "status")
                {
                    firstname = "ispending";
                    firstvalue = Convert.ToString(cmbstatus1.SelectedIndex);
                }
                else
                {
                    firstvalue = txtsearch1.Text.Trim();
                }

                secondname = cbxSearchOrderDate.Text.Trim();
                if (secondname == "status")
                {
                    secondname = "ispending";
                    secondvalue = Convert.ToString(cmbstatus2.SelectedIndex);
                }
                else
                {
                    secondvalue = txtsearch2.Text.Trim();
                }


                thirdname = cbxVendor.Text.Trim();
                if (thirdname == "status")
                {
                    thirdname = "ispending";
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
            DataTable dt = ReferenceBAL.searchEmployee(firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue);
            dgvSearch.Columns.Clear();
            dgvSearch.DataSource = dt;

            dgvSearch.Columns["Address1"].Visible = false;
            dgvSearch.Columns["Address2"].Visible = false;
            dgvSearch.Columns["State"].Visible = false;
            dgvSearch.Columns["District"].Visible = false;
            dgvSearch.Columns["City"].Visible = false;
            dgvSearch.Columns["Pincode"].Visible = false;
            dgvSearch.Columns["Email"].Visible = false;
            dgvSearch.Columns["Phone"].Visible = false;
            dgvSearch.Columns["Fax"].Visible = false;
            dgvSearch.Columns["Path"].Visible = false;

            dgvSearch.Columns["employeeid"].Visible = false;

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

                lblhidden.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["employeeid"].Value);
                txtCustomerName.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Name"].Value);
                txtAddressLine1.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Address1"].Value);
                txtAddressLine2.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Address2"].Value);
                cmbbxState.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["State"].Value);
                cmbbxDistrict.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["District"].Value);
                cmbbxCity.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["City"].Value);
                txtPincode.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Pincode"].Value);
                txtEmail.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Email"].Value);
                txtPhone.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Phone"].Value);
                txtFax.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Fax"].Value);
                filename = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["Path"].Value);
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

        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
            GetProductSearch();
        }

        public void GetProductSearch()
        {
            DataTable dt = ReferenceBAL.GetEmpoyeeSearch(txtprodsearch.Text);

            dgvCustomerinfo.Columns.Clear();
            dgvCustomerinfo.DataSource = dt;
            dgvCustomerinfo.Columns["employeeid"].Visible = false;

            DataGridViewImageColumn img1 = new DataGridViewImageColumn();
            img1.Image = Inventory.Properties.Resources.user_edit;
            dgvCustomerinfo.Columns.Insert(12, img1);
            img1.HeaderText = "Edit";
            img1.Name = "Edit";

            DataGridViewImageColumn img2 = new DataGridViewImageColumn();
            img2.Image = Inventory.Properties.Resources.trash;
            dgvCustomerinfo.Columns.Insert(13, img2);
            img2.HeaderText = "Delete";
            img2.Name = "Delete";

            //dgvCustomerinfo.Columns["Name"].Width = 20;
            dgvCustomerinfo.Columns["Edit"].Width = 40;
            dgvCustomerinfo.Columns["Delete"].Width = 60;



            dgvCustomerinfo.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvCustomerinfo.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvCustomerinfo.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            pnlprodsearch.Visible = false;
        }

    }
}





















































