using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using InvDal;
using InvBal;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Inventory.Masters
{
    public partial class FrmUserCreation : Form
    {
        UserCreationBAL ObjUserCreationBAL = new UserCreationBAL();
        string SelectedRigits = string.Empty;
        string filepath = string.Empty;
        string imgpath = string.Empty;
        public FrmUserCreation()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            pnlLabelSearch.Visible = true;
            vLabel1.Visible = true;
            pnlSearch.Visible = false;
            splitContainer1.Panel1Collapsed = true;
            vLabel1.Enabled = false;
            ddlRole.SelectedIndex = 0;
            txtUserName.Focus();
            bindfloor();
            GetUserCreation();
        }

        public void bindfloor()
        {
            ClbFloor.DataSource = UserCreationBAL.GetFloor();
            ClbFloor.DisplayMember = "LocationName";
            ClbFloor.ValueMember = "LocationID";
        }

        private void InsertUpdateRightToInformation()
        {
            int Status = 0;

            foreach (DataRowView view in ClbFloor.CheckedItems)
            {
                if (!String.IsNullOrEmpty(SelectedRigits))
                {
                    SelectedRigits = SelectedRigits + "," + view[ClbFloor.ValueMember].ToString();
                }
                else
                {
                    SelectedRigits = view[ClbFloor.ValueMember].ToString();
                }
            }

            if (string.IsNullOrEmpty(Convert.ToString(lblhidden.Text)))
            {

                ObjUserCreationBAL.id = "";
            }
            else
            {
                ObjUserCreationBAL.id = lblhidden.Text;
            }


            ObjUserCreationBAL.UserName = txtUserName.Text;
            ObjUserCreationBAL.Password = txtPassword.Text;
            ObjUserCreationBAL.Role = ddlRole.Text;
            ObjUserCreationBAL.FloorName = SelectedRigits;
            ObjUserCreationBAL.userfullname = TxtuserfullName.Text;

           Status = UserCreationBAL.SaveUserCreation(ObjUserCreationBAL);

            if (Status == 1 && lblhidden.Text == string.Empty)
            {
                MessageBox.Show("Inserted successfully");
                
                GetUserCreation();
                
                clear();
            }

            else if (Status == 1 && lblhidden.Text != string.Empty)
            {
                MessageBox.Show("Updated Succesfully");
                
                GetUserCreation();
                
                clear();

            }
            else if (Status == 2)
            {
                MessageBox.Show("UserName already exist ");
                txtUserName.Focus();
            }
        }
        private void btnUOMSave_Click(object sender, EventArgs e)
        {

        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
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
                    
                        GetUserCreationSearch();
                   
                }
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }
        public void GetUserCreationSearch()
        {
            DataTable dt = UserCreationBAL.GetUserCreationSearch(txtprodsearch.Text);
            dgvUserCreation.Columns.Clear();
            dgvUserCreation.DataSource = dt;
            dgvUserCreation.Columns["IsActive"].Visible = false;
            dgvUserCreation.Columns["EnteredBy"].Visible = false;
            dgvUserCreation.Columns["IsDeleted"].Visible = false;
            dgvUserCreation.Columns["Encrypt"].Visible = false;
            dgvUserCreation.Columns["Mobile"].Visible = false;
            dgvUserCreation.Columns["FloorName"].Visible = false;
            dgvUserCreation.Columns["UserId"].Visible = false;
            dgvUserCreation.Columns["EnteredOn"].Visible = false;
            dgvUserCreation.Columns["URole"].HeaderText = "Role";

            DataGridViewImageColumn img1 = new DataGridViewImageColumn();
            img1.Image = Inventory.Properties.Resources.user_edit;
            dgvUserCreation.Columns.Insert(11, img1);
            img1.HeaderText = "Edit";
            img1.Name = "Edit";

            DataGridViewImageColumn img2 = new DataGridViewImageColumn();
            img2.Image = Inventory.Properties.Resources.trash;
            dgvUserCreation.Columns.Insert(12, img2);
            img2.HeaderText = "Delete";
            img2.Name = "Delete";

            dgvUserCreation.Columns["Edit"].Width = 70;
            dgvUserCreation.Columns["Delete"].Width = 70;
            dgvUserCreation.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvUserCreation.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvUserCreation.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            pnlprodsearch.Visible = false;

        }
        private void clear()
        {
            txtUserName.Clear();
            txtPassword.Clear();
            ddlRole.SelectedIndex = 0;
            TxtuserfullName.Clear();
            SelectedRigits = "";
            foreach (int i in ClbFloor.CheckedIndices)
            {
                ClbFloor.SetItemCheckState(i, CheckState.Unchecked);
            }

        }
        private bool Validationmenu()
        {
            bool status = true;
            string message = "";
            int i = 0;

            int count = ClbFloor.CheckedItems.Count;
            //if (count <= 0)
            //{
            //    i++;
            //    message = message + "*  Please select atleast one keyword" + "\n";
            //    if (i == 1)
            //    {
            //        ClbFloor.Focus();
            //    }
            //}

            if (ddlRole.SelectedIndex == 5)
            {
                if (count <= 0)
                {
                    i++;
                    message = message + "*  Please select atleast one keyword" + "\n";
                    if (i == 1)
                    {
                        ClbFloor.Focus();
                    }
                }
            }

            if (string.IsNullOrEmpty(Convert.ToString(txtUserName.Text)))
            {
                i++;
                message = message + "* UserName should not empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtUserName;
            }


            if (string.IsNullOrEmpty(Convert.ToString(txtPassword.Text)))
            {
                i++;
                message = message + "* Password should not empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtPassword;
            }

            if (string.IsNullOrEmpty(Convert.ToString(TxtuserfullName.Text)))
            {
                i++;
                message = message + "* UserFullName should not empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtUserName;
            }

            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message);
                status = false;
            }
            return status;
        }

       

        private void btnUOMSave_Click_1(object sender, EventArgs e)
        {
            if (Validationmenu())
            {



                //foreach (object Roles in ClbFloor.CheckedItems)
                //{
                //    if (!string.IsNullOrEmpty(SelectedRigits))
                //    {
                //        SelectedRigits = SelectedRigits + "," + ClbFloor.Items.IndexOf(Roles).ToString();
                //    }
                //    else
                //    {
                //        SelectedRigits = ClbFloor.Items.IndexOf(Roles).ToString();
                //    }
                //}
                bool val = Validationmenu();
                if (val)
                {
                    InsertUpdateRightToInformation();
                }
            }
        }

        public void GetUserCreation()
       {
           DataTable dt = UserCreationBAL.GetUserCreation();
           dgvUserCreation.Columns.Clear();
           dgvUserCreation.DataSource = dt;
           dgvUserCreation.Columns["IsActive"].Visible = false;
           dgvUserCreation.Columns["EnteredBy"].Visible = false;
           dgvUserCreation.Columns["IsDeleted"].Visible = false;
           dgvUserCreation.Columns["Encrypt"].Visible = false;
           dgvUserCreation.Columns["Mobile"].Visible = false;
           dgvUserCreation.Columns["FloorName"].Visible = false;
           dgvUserCreation.Columns["UserId"].Visible = false;
           dgvUserCreation.Columns["EnteredOn"].Visible = false;
           dgvUserCreation.Columns["URole"].HeaderText = "Role";
         
           DataGridViewImageColumn img1 = new DataGridViewImageColumn();
           img1.Image = Inventory.Properties.Resources.user_edit;
           dgvUserCreation.Columns.Insert(11, img1);
           img1.HeaderText = "Edit";
           img1.Name = "Edit";

           DataGridViewImageColumn img2 = new DataGridViewImageColumn();
           img2.Image = Inventory.Properties.Resources.trash;
           dgvUserCreation.Columns.Insert(12, img2);
           img2.HeaderText = "Delete";
           img2.Name = "Delete";

           dgvUserCreation.Columns["Edit"].Width = 70;
           dgvUserCreation.Columns["Delete"].Width = 70;
           dgvUserCreation.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
           dgvUserCreation.DefaultCellStyle.BackColor = Color.Gainsboro;
           dgvUserCreation.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
          
    }

        private void dgvUserCreation_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvUserCreation.Columns[e.ColumnIndex].HeaderText == "Edit")
                {
                    lblhidden.Text = Convert.ToString(dgvUserCreation.Rows[e.RowIndex].Cells["UserId"].Value);
                    txtUserName.Text = Convert.ToString(dgvUserCreation.Rows[e.RowIndex].Cells["UserName"].Value);
                    txtPassword.Text = Convert.ToString(dgvUserCreation.Rows[e.RowIndex].Cells["Encrypt"].Value);
                    ddlRole.Text = Convert.ToString(dgvUserCreation.Rows[e.RowIndex].Cells["URole"].Value);
                    ClbFloor.Text = Convert.ToString(dgvUserCreation.Rows[e.RowIndex].Cells["FloorName"].Value);
                    TxtuserfullName.Text = Convert.ToString(dgvUserCreation.Rows[e.RowIndex].Cells["UserFullName"].Value);

                    string getRigths = dgvUserCreation.Rows[e.RowIndex].Cells["FloorName"].Value.ToString();

                    for (int s = 0; s < ClbFloor.Items.Count; s++)
                    {
                        ClbFloor.SetItemChecked(s, false);
                    }

                    string[] strArrUser = getRigths.Split(',');
                    int j = 0;
                    for (int k = 0; k < ClbFloor.Items.Count; k++)
                    {
                        DataRowView row = (DataRowView)ClbFloor.Items[k];
                        if (j < strArrUser.Length)
                        {
                            if (row[ClbFloor.ValueMember].ToString() == strArrUser[j].ToString())
                            {
                                ClbFloor.SetItemChecked(k, true);
                                j++;
                            }
                        }
                    }

                    //for (int s = 0; s < ClbFloor.Items.Count; s++)
                    //{
                    //    ClbFloor.SetItemChecked(s, false);
                    //}
                    //string[] strArr = getRigths.Split(',');
                    //string FlrID = Convert.ToString(ClbFloor.SelectedIndex);
                    //if (!string.IsNullOrEmpty(getRigths))
                    //{
                    //for (int k = 0; k < strArr.Length; k++)
                    //{
                    //    int ss = Convert.ToInt32(strArr[k]);
                    //    ClbFloor.SetItemChecked(ss, true);
                    //}
                    //}
                  
                }
                else if (dgvUserCreation.Columns[e.ColumnIndex].HeaderText == "Delete")
                {
                    string namestatus = Convert.ToString(dgvUserCreation.Rows[e.RowIndex].Cells["UserName"].Value);
                    DialogResult result = MessageBox.Show("Are you sure delete UserName " + namestatus + "?", "delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.Yes))
                    {
                        lblhidden.Text = Convert.ToString(dgvUserCreation.Rows[e.RowIndex].Cells["UserId"].Value);
                        int i = Delete();
                        if (i == 1)
                        {
                            MessageBox.Show("Delete Successfully");
                            clear();
                            GetUserCreation();
                        }
                    }
                }
            }
        }
        public int Delete()
        {
            int result = 0;
            result = UserCreationBAL.deleteUserCreation(Convert.ToInt32(lblhidden.Text));
            return result;
        }

        private void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ddlRole.SelectedIndex==5)
            {
                ClbFloor.Enabled = true;
                label2.Enabled = true;
            }
            else
            {
                ClbFloor.Enabled = false;
                label2.Enabled = false;
            }
        }

        private void btnclaer_Click_1(object sender, EventArgs e)
        {
            clear();
        }

        private void btnnew_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void vLabel1_Click(object sender, EventArgs e)
        {

        }

        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
            GetUserCreationSearch();
        }
    }
}
