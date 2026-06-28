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
    public partial class FrmRoles : Form
    {
        UserCreationBAL ObjUserCreationBAL = new UserCreationBAL();
        QuotationBal objQuotationbal = new QuotationBal();
        string SelectedRigits = string.Empty;
        string filepath = string.Empty;
        string imgpath = string.Empty;
        public FrmRoles()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            pnlLabelSearch.Visible = true;
            vLabel1.Visible = true;
            pnlSearch.Visible = false;
            splitContainer1.Panel1Collapsed = true;
            vLabel1.Enabled = false;
            Getuser();
            Getmenu();
            ddlRole.SelectedIndex = 0;
            GetUserCreation();
            ddlRole.Focus();

        }

        public void Getuser()
        {
            DataTable dt = UserCreationBAL.GetUserCreation();
            DataRow ds = dt.NewRow();
            ds["UserId"] = "0";
            ds["UserName"] = "--Select--";
            dt.Rows.InsertAt(ds, 0);
            ddlRole.DataSource = dt;
            ddlRole.DisplayMember = "UserName";
            ddlRole.ValueMember = "UserId";


        }
      

        public void Getmenu()
        {
            DataTable dt = objQuotationbal.Getmenu();

            Chkmenu.DataSource = dt;
            Chkmenu.DisplayMember = "MenuName";
            Chkmenu.ValueMember = "Menuid";


        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void InsertUpdateRightToInformation()
        {
            foreach (DataRowView view in Chkmenu.CheckedItems)
            {
                if (!String.IsNullOrEmpty(SelectedRigits))
                {
                    SelectedRigits = SelectedRigits + "," + view[Chkmenu.ValueMember].ToString();
                }
                else
                {
                    SelectedRigits = view[Chkmenu.ValueMember].ToString();
                }
            }
            int Status = 0;
            if (string.IsNullOrEmpty(Convert.ToString(lblhidden.Text)))
            {

                ObjUserCreationBAL.Menuid = "";
            }
            else
            {
                ObjUserCreationBAL.Menuid = lblhidden.Text;
            }


            ObjUserCreationBAL.UserId = Convert.ToString(ddlRole.SelectedValue);
            ObjUserCreationBAL.Menu = SelectedRigits;



            Status = UserCreationBAL.SaveUserMenu(ObjUserCreationBAL);

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
                MessageBox.Show("Role already exist ");
                ddlRole.Focus();
            }
        }
        private void btnUOMSave_Click(object sender, EventArgs e)
        {

        }
        private void clear()
        {

            ddlRole.SelectedIndex = 0;
            SelectedRigits = "";
            foreach (int i in Chkmenu.CheckedIndices)
            {
                Chkmenu.SetItemCheckState(i, CheckState.Unchecked);
            }
            lblhidden.Text = "";

        }
        private bool Validationmenu()
        {
            bool status = true;
            string message = "";
            int i = 0;

            int count = Chkmenu.CheckedItems.Count;
            if (count <= 0)
            {
                i++;
                message = message + "*  Please select atleast menu" + "\n";
                if (i == 1)
                {
                    Chkmenu.Focus();
                }
            }

            if (ddlRole.SelectedIndex == 0)
            {
                //if (count <= 0)
                //{
                i++;
                message = message + "*  Please select user name" + "\n";
                if (i == 1)
                {
                    ddlRole.Focus();
                }
                //}
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




                bool val = Validationmenu();
                if (val)
                {
                    InsertUpdateRightToInformation();
                    GetUserCreation();
                }
            }
        }

        public void GetUserCreation()
        {
            DataTable dt = UserCreationBAL.GetUserMenu();
            dgvUserCreation.Columns.Clear();



            DataTable dt1 = new DataTable();
            dt1.Columns.Add("id", typeof(int));
            dt1.Columns.Add("UserName", typeof(string));
            dt1.Columns.Add("Menu", typeof(string));
            dt1.Columns.Add("RightsId", typeof(string));

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr1 = dt1.NewRow();
                dr1["id"] = dt.Rows[i]["id"].ToString().Trim();
                dr1["UserName"] = dt.Rows[i]["UserName"].ToString().Trim();
                dr1["RightsId"] = dt.Rows[i]["Menu"].ToString().Trim();

                string strRoles = dt.Rows[i]["Menu"].ToString();
                string[] strArr = strRoles.Split(',');
                string items = string.Empty;
                //foreach (DataRowView view in (DataRowView)strArr)
                //{
                //    if (!String.IsNullOrEmpty(SelectedRigits))
                //    {
                //        SelectedRigits = SelectedRigits + "," + view[Chkmenu.ValueMember].ToString();
                //    }
                //    else
                //    {
                //        SelectedRigits = view[Chkmenu.ValueMember].ToString();
                //    }
                //}
                for (int j = 0; j < strArr.Length; j++)
                {
                    int k = Convert.ToInt32(strArr[j]);
                    DataRowView row = (DataRowView)Chkmenu.Items[k - 1];



                    if (!string.IsNullOrEmpty(items))
                    {
                        items = items + "," + row[Chkmenu.DisplayMember].ToString();
                    }
                    else
                    {
                        items = row[Chkmenu.DisplayMember].ToString();
                    }
                }
                dr1["Menu"] = items.Trim();
                dt1.Rows.Add(dr1);
                dt1.AcceptChanges();
            }
            dgvUserCreation.DataSource = dt1;
            dgvUserCreation.Columns["Id"].Visible = false;
            dgvUserCreation.Columns["RightsId"].Visible = false;
            dgvUserCreation.Columns["UserName"].HeaderText = "UserName";


            DataGridViewImageColumn img1 = new DataGridViewImageColumn();
            img1.Image = Inventory.Properties.Resources.user_edit;
            dgvUserCreation.Columns.Insert(3, img1);
            img1.HeaderText = "Edit";
            img1.Name = "Edit";


            dgvUserCreation.Columns["Edit"].Width = 40;
            dgvUserCreation.Columns["UserName"].Width = 80;


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
                    lblhidden.Text = Convert.ToString(dgvUserCreation.Rows[e.RowIndex].Cells["id"].Value);
                    ddlRole.Text = Convert.ToString(dgvUserCreation.Rows[e.RowIndex].Cells["UserName"].Value);


                    string getRigths = dgvUserCreation.Rows[e.RowIndex].Cells["RightsId"].Value.ToString();

                    for (int s = 0; s < Chkmenu.Items.Count; s++)
                    {
                        Chkmenu.SetItemChecked(s, false);
                    }

                    string[] strArrUser = getRigths.Split(',');
                    int j = 0;
                    for (int k = 0; k < Chkmenu.Items.Count; k++)
                    {
                        DataRowView row = (DataRowView)Chkmenu.Items[k];
                        if (j < strArrUser.Length)
                        {
                            if (row[Chkmenu.ValueMember].ToString() == strArrUser[j].ToString())
                            {
                                Chkmenu.SetItemChecked(k, true);
                                j++;
                            }
                        }
                    }

                }
            }

        }


        private void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRole.SelectedIndex == 5)
            {
                //ClbFloor.Enabled = true;
                label2.Enabled = true;
            }
            else
            {
                //ClbFloor.Enabled = false;
                label2.Enabled = true;
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
    }
}
