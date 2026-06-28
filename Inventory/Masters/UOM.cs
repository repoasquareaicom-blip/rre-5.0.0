
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
//using Vendor.PurchaseOrder;

namespace Inventory.Masters
{
    public partial class UOM : Form
    {
        public TextBox tb;
        UomBAL ObjUomBAL = new UomBAL();
        string connectionString = Program.connection;
        public UOM()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            getuom();
            pnlLabelSearch.Visible = true;
            vLabel1.Visible = true;
            pnlSearch.Visible = false;
            splitContainer1.Panel1Collapsed = true;
            vLabel1.Enabled = false;
        }
        public void clear()
        {
            txtuom.Text = string.Empty;
            lblhidden.Text = string.Empty;
        }

        private void btnUOMSave_Click(object sender, EventArgs e)
        {
            if (Validation())
            {
                InsertUpdateRightToInformation();
            }


        }
        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (string.IsNullOrEmpty(txtuom.Text))
            {
                i++;
                message = message + "*UOM Should Not Be Empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtuom;
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

        private void txtuom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
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
            if (string.IsNullOrEmpty(Convert.ToString(lblhidden.Text)))
            {
                ObjUomBAL.Id ="";
                ObjUomBAL.Uom = txtuom.Text.Trim();
                ObjUomBAL.UserId = Convert.ToString(Program.userlevel);
            }
            else
            {
                ObjUomBAL.Id = Convert.ToString(lblhidden.Text);
                ObjUomBAL.Uom = txtuom.Text.Trim();
                ObjUomBAL.UserId = Convert.ToString(Program.userlevel);
            }

            


            int Status = UomBAL.SaveUom(ObjUomBAL);

            if (Status == 1 && lblhidden.Text == string.Empty)
            {
                MessageBox.Show("Inserted successfully");
                getuom();
                clear();

            }

            else if (Status == 1 && lblhidden.Text != string.Empty)
            {
                MessageBox.Show("Updated Succesfully");
                getuom();
                clear();

            }
            else if (Status == 2)
            {
                MessageBox.Show("Right To Information already exist ");
                txtuom.Focus();
            }
        }

        public void getuom()
        {
                DataTable dt = UomBAL.GetUOM();
                dgvuom.Columns.Clear();
                dgvuom.DataSource = dt;
                dgvuom.Columns["uomid"].Visible = false;
                dgvuom.Columns["Updatedon"].Visible = false;
                dgvuom.Columns["Updatedby"].Visible = false;
                dgvuom.Columns["IsDeleted"].Visible = false;


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

                dgvuom.Columns["Edit"].Width = 30;
                dgvuom.Columns["Delete"].Width = 50;

                dgvuom.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
                dgvuom.DefaultCellStyle.BackColor = Color.Gainsboro;
                dgvuom.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            
        }

        private void dgvuom_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvuom.Columns[e.ColumnIndex].HeaderText == "Edit")
                {
                    lblhidden.Text = Convert.ToString(dgvuom.Rows[e.RowIndex].Cells["uomid"].Value);
                    txtuom.Text = Convert.ToString(dgvuom.Rows[e.RowIndex].Cells["UOM"].Value);
                }

                else if (dgvuom.Columns[e.ColumnIndex].HeaderText == "Delete")
                {
                    string namestatus = Convert.ToString(dgvuom.Rows[e.RowIndex].Cells["UOM"].Value);
                    DialogResult result = MessageBox.Show("Are you sure delete UOM " + namestatus + "?", "Delete UOM?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.Yes))
                    {
                        lblhidden.Text = Convert.ToString(dgvuom.Rows[e.RowIndex].Cells["uomid"].Value);
                        int i = Delete();
                        if (i == 1)
                        {
                            MessageBox.Show("Delete Successfully");
                            clear();
                            getuom();
                        }
                    }

                }
            }
        }


        public int Delete()
        {
            int result = 0;
            result = UomBAL.DeleteUom(Convert.ToInt32(lblhidden.Text));
            return result;
        }

        private void dgvuom_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            //Skip the Column and Row headers
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                return;
            }
            var dataGridView = (sender as DataGridView);
            //Check the condition as per the requirement casting the cell value to the appropriate type
            if (dgvuom.Columns[e.ColumnIndex].HeaderText == "Edit" || dgvuom.Columns[e.ColumnIndex].HeaderText == "Delete")
                dataGridView.Cursor = Cursors.Hand;
            else
                dataGridView.Cursor = Cursors.Default;
        }

        private void btnnew_Click(object sender, EventArgs e)
        {
            txtuom.Text = string.Empty;
            lblhidden.Text = string.Empty;
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            txtuom.Text = string.Empty;
            lblhidden.Text = string.Empty;
        }

        private void UOM_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtuom;
            lblhidden.Text = string.Empty;
        }
    }
}





















































