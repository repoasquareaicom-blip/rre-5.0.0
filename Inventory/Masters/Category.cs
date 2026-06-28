
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;

using InvBal;
namespace Inventory.Masters
{
    public partial class Category : Form
    {
        public TextBox tb;
        CategoryBAL ObjCategoryBAL = new CategoryBAL();
        string connectionString = Program.connection;
       
        public Category()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            getcategory();
            pnlLabelSearch.Visible = true;
            vLabel1.Visible = true;
            pnlSearch.Visible = false;
            splitContainer1.Panel1Collapsed = true;
            vLabel1.Enabled = false;
           
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            if(validation())
            {
                InsertUpdateRightToInformation();
            }
        }

        public bool validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(txtcategory.Text.Trim()))
            {
                i++;
                message = message + "* Category Should Not Be Empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtcategory;
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;

            }
            return status;
        }

        private void txtcategory_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
       
        private void clear()
        {
            txtcategory.Text = "";
            lblhidden.Text = string.Empty;
        }

        public void getcategory()
        {
            DataTable dt = CategoryBAL.objCategoryDAL.GetCategory();
           
                dgvuom.Columns.Clear();
                dgvuom.DataSource = dt;
                dgvuom.Columns["id"].Visible = false;
                dgvuom.Columns["Updatedon"].Visible = false;
                dgvuom.Columns["Updatedby"].Visible = false;
                dgvuom.Columns["IsDeleted"].Visible = false;

                dgvuom.Columns["categoryname"].HeaderText = "Category Name";


                DataGridViewImageColumn img1 = new DataGridViewImageColumn();
                img1.Image = Inventory.Properties.Resources.user_edit;
                dgvuom.Columns.Insert(4, img1);
                img1.HeaderText = "Edit";
                img1.Name = "Edit";


                DataGridViewImageColumn img2 = new DataGridViewImageColumn();
                img2.Image = Inventory.Properties.Resources.trash;
                dgvuom.Columns.Insert(5, img2);
                img2.HeaderText = "Delete";
                img2.Name = "Delete";

                dgvuom.Columns["Edit"].Width = 24;
                dgvuom.Columns["Delete"].Width = 40;
                dgvuom.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                dgvuom.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvuom.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            }
        

        private void InsertUpdateRightToInformation()
        {

            if (string.IsNullOrEmpty(Convert.ToString(lblhidden.Text)))
            {
                ObjCategoryBAL.id = "";
                ObjCategoryBAL.categoryname = txtcategory.Text.Trim();
                ObjCategoryBAL.Updatedby = Convert.ToString(Program.userlevel);
            }
            else
            {
                ObjCategoryBAL.id = Convert.ToString(lblhidden.Text);
                ObjCategoryBAL.categoryname = txtcategory.Text.Trim();
                ObjCategoryBAL.Updatedby = Convert.ToString(Program.userlevel);
            }




            int Status = CategoryBAL.objCategoryDAL.SaveCategory(ObjCategoryBAL);
      
                if (Status == 1 && lblhidden.Text == string.Empty)
                {
                    MessageBox.Show("Inserted successfully");
                    getcategory();
                    clear();

                }

                else if (Status == 1 && lblhidden.Text != string.Empty)
                {
                    MessageBox.Show("Updated Succesfully");
                    getcategory();
                    clear();

                }
                else if (Status == 2)
                {
                    MessageBox.Show("Category already exist ");
                    txtcategory.Focus();
                }
            
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData==Keys.Escape)
            {
                this.Close();
                return true;
            }
         
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void dgvuom_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvuom.Columns[e.ColumnIndex].HeaderText == "Edit")
                {
                    lblhidden.Text = Convert.ToString(dgvuom.Rows[e.RowIndex].Cells["id"].Value);
                    txtcategory.Text = Convert.ToString(dgvuom.Rows[e.RowIndex].Cells["categoryname"].Value);
                }

                else if (dgvuom.Columns[e.ColumnIndex].HeaderText == "Delete")
                {
                    string namestatus = Convert.ToString(dgvuom.Rows[e.RowIndex].Cells["categoryname"].Value);
                    DialogResult result = MessageBox.Show("Are you sure delete category " + namestatus + "?", "Delete category?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.Yes))
                    {
                        lblhidden.Text = Convert.ToString(dgvuom.Rows[e.RowIndex].Cells["id"].Value);
                        int i = Delete();
                        if (i == 1)
                        {
                            MessageBox.Show("Delete Successfully");
                            clear();
                            getcategory();
                        }
                    }

                }
            }
        }

        public int Delete()
        {
            int result = 0;
            result = CategoryBAL.objCategoryDAL.DeleteCategory(Convert.ToInt32(lblhidden.Text));
          
            return result;
        }

        private void btnnew_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void Category_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtcategory;
        }

        private void vLabel1_Click(object sender, EventArgs e)
        {

        }

       
        
    }
}





















































