using InvBal;
using InvDal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using System.Windows.Forms;

namespace Inventory.Masters
{
    public partial class Location : Form
    {
        public TextBox tb;
        LocationBAL ObjLocationBAL = new LocationBAL();
        string connectionString = Program.connection;
        public void LoadRacks()
        {
            dgvRack.Rows.Clear();

            dgvRack.Rows.Add(1);


           
            this.dgvRack.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvRack.Columns[0].Width = 100;

            this.dgvRack.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvRack.Columns[1].Width = 400;
            this.dgvRack.Columns[1].Visible = false;








            dgvRack.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvRack.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }
            txtLocation.Focus();

        }

        public Location()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            getLocation();         
            pnlLabelSearch.Visible = true;
            vLabel1.Visible = true;
            pnlSearch.Visible = false;
            splitContainer1.Panel1Collapsed = true;
            vLabel1.Enabled = false;
            LoadRacks();
        }
        #region Processcmd
        [DllImport("user32.dll")]
        internal static extern IntPtr GetFocus();
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (dgvRack.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = btnUOMSave;
                    return true;
                }

            }
            
            if (keyData == (Keys.S | Keys.Control))
            {
                btnUOMSave.PerformClick();


                return true;
            }

            else if (keyData == (Keys.C | Keys.Alt))
            {

                clear();

                return true;
            }

            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
         
          
            if (keyData == Keys.Enter)
            {

                try
                {
                    IntPtr wndHandle = GetFocus();
                    Control focusedControl = FromChildHandle(wndHandle);

                    if (focusedControl.Name == "txtLocation")
                    {
                        this.ActiveControl = ddlIncharge;
                      
                    }
                    if (focusedControl.Name == "ddlIncharge")
                    {
                        dgvRack.Focus();
                        dgvRack.CurrentCell = dgvRack[0, 0];
                    }

                }
                catch
                {

                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion 
        public void clear()
        {
            txtLocation.Text = string.Empty;
            lblhidden.Text = string.Empty;
            ddlIncharge.SelectedIndex = 0;
            LoadRacks();

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

            if (string.IsNullOrEmpty(txtLocation.Text))
            {
                i++;
                message = message + "*Location Should Not Be Empty" + "\n";
                if (i == 1)
                    this.ActiveControl = txtLocation;
            }
            if (ddlIncharge.SelectedIndex==0)
            {
                i++;
                message = message + "* Select Inchargeby" + "\n";
                if (i == 1)
                    this.ActiveControl = txtLocation;
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

        private void txtLocation_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }


        private void InsertUpdateRightToInformation()
        {
            if (string.IsNullOrEmpty(Convert.ToString(lblhidden.Text)))
            {
                ObjLocationBAL.Id = "";
                ObjLocationBAL.LocationName = txtLocation.Text.Trim();
                ObjLocationBAL.inchargeby = Convert.ToInt32(ddlIncharge.SelectedValue);

                ObjLocationBAL.UserId = Convert.ToString(Program.userlevel);
            }
            else
            {
                ObjLocationBAL.Id = Convert.ToString(lblhidden.Text);
                ObjLocationBAL.LocationName = txtLocation.Text.Trim();
                ObjLocationBAL.inchargeby = Convert.ToInt32(ddlIncharge.SelectedValue);
                ObjLocationBAL.UserId = Convert.ToString(Program.userlevel);
            }




            int Status = LocationBAL.SaveLocation(ObjLocationBAL);

            for (int i = 0; i < dgvRack.Rows.Count;i++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvRack.Rows[i].Cells[0].Value)))
                {
                if (string.IsNullOrEmpty(Convert.ToString(dgvRack.Rows[i].Cells[1].Value)))
                {
                    ObjLocationBAL.rackid = "";
                }
                else
                {
                    ObjLocationBAL.rackid = Convert.ToString(dgvRack.Rows[i].Cells[1].Value);
                }

                ObjLocationBAL.Id = Convert.ToString(Status);
                ObjLocationBAL.rackname = Convert.ToString(dgvRack.Rows[i].Cells[0].Value);
                int res = LocationBAL.SaveLocationRack(ObjLocationBAL);
                }
            }



            if (lblhidden.Text == string.Empty)
            {
                MessageBox.Show("Inserted successfully");
                getLocation();
                clear();

            }

            else if (lblhidden.Text != string.Empty)
            {
                MessageBox.Show("Updated Succesfully");
                getLocation();
                clear();

            }
            else if (Status == 2)
            {
                MessageBox.Show("Right To Information already exist ");
                txtLocation.Focus();
            }
        }

        public void getLocation()
        {
            DataTable dt = LocationBAL.GetLocation();
            dgvLocation.Columns.Clear();
            dgvLocation.DataSource = dt;
            dgvLocation.Columns["LocationID"].Visible = false;
            dgvLocation.Columns["RackID"].Visible = false;
            dgvLocation.Columns["UserId"].Visible = false;

            dgvLocation.Columns["LocationName"].HeaderText = "Location Name";
            dgvLocation.Columns["InchargeBy"].HeaderText = "Incharge By";
            dgvLocation.Columns["RackName"].HeaderText = "Rack Name";

            DataGridViewImageColumn img1 = new DataGridViewImageColumn();
            img1.Image = Inventory.Properties.Resources.user_edit;
            dgvLocation.Columns.Insert(5, img1);
            img1.HeaderText = "Edit";
            img1.Name = "Edit";


            //DataGridViewImageColumn img2 = new DataGridViewImageColumn();
            //img2.Image = Inventory.Properties.Resources.trash;
            //dgvLocation.Columns.Insert(6, img2);
            //img2.HeaderText = "Delete";
            //img2.Name = "Delete";

            dgvLocation.Columns["Edit"].Width = 40;
            //dgvLocation.Columns["Delete"].Width = 60;

            dgvLocation.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvLocation.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvLocation.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


        }

        private void dgvLocation_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvLocation.Columns[e.ColumnIndex].HeaderText == "Edit")
                {
                    lblhidden.Text = Convert.ToString(dgvLocation.Rows[e.RowIndex].Cells["LocationID"].Value);
                    txtLocation.Text = Convert.ToString(dgvLocation.Rows[e.RowIndex].Cells["LocationName"].Value);
                    string rackname = Convert.ToString(dgvLocation.Rows[e.RowIndex].Cells["RackName"].Value);
                    string[] rack = rackname.Split(',');
                    string rackId = Convert.ToString(dgvLocation.Rows[e.RowIndex].Cells["RackID"].Value);
                    string[] racki = rackId.Split(',');
                    dgvRack.Rows.Clear();
                    for (int i = 0; i < rack.Count();i++ )
                    {
                        dgvRack.Rows.Add(1);
                        dgvRack.Rows[i].Cells[0].Value=rack[i];
                        dgvRack.Rows[i].Cells[1].Value = racki[i];

                    }
                    ddlIncharge.Text = Convert.ToString(dgvLocation.Rows[e.RowIndex].Cells["InchargeBy"].Value);

                    this.ActiveControl = txtLocation;

                }

                else if (dgvLocation.Columns[e.ColumnIndex].HeaderText == "Delete")
                {
                    string namestatus = Convert.ToString(dgvLocation.Rows[e.RowIndex].Cells["LocationName"].Value);
                    DialogResult result = MessageBox.Show("Are you sure delete Location " + namestatus + "?", "Delete Location?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.Yes))
                    {
                        lblhidden.Text = Convert.ToString(dgvLocation.Rows[e.RowIndex].Cells["Locationid"].Value);
                        int i = Delete();
                        if (i == 1)
                        {
                            MessageBox.Show("Delete Successfully");
                            clear();
                            getLocation();
                        }
                    }

                }
            }
        }


        public int Delete()
        {
            int result = 0;
            result = LocationBAL.DeleteLocation(Convert.ToInt32(lblhidden.Text));
            return result;
        }

        private void dgvLocation_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                return;
            }
            var dataGridView = (sender as DataGridView);
         
            if (dgvLocation.Columns[e.ColumnIndex].HeaderText == "Edit" || dgvLocation.Columns[e.ColumnIndex].HeaderText == "Delete")
                dataGridView.Cursor = Cursors.Hand;
            else
                dataGridView.Cursor = Cursors.Default;
        }

        private void btnnew_Click(object sender, EventArgs e)
        {
            txtLocation.Text = string.Empty;
            lblhidden.Text = string.Empty;
        }

        private void btnclaer_Click(object sender, EventArgs e)
        {
            txtLocation.Text = string.Empty;
            lblhidden.Text = string.Empty;
            ddlIncharge.SelectedIndex = 0;
            LoadRacks();
            
        }

        private void Location_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtLocation;
            lblhidden.Text = string.Empty;
            Bindusers();
            ddlIncharge.SelectedIndex = 0;
        }

        public void Bindusers()
        {
            DataTable dt = LocationBAL.Getuser();
            DataRow dr = dt.NewRow();
            dr["UserId"]="0";
            dr["UserName"] = "--Select--";
            dt.Rows.InsertAt(dr, 0);
            
            ddlIncharge.DataSource = dt;
            ddlIncharge.ValueMember = "UserId";
            ddlIncharge.DisplayMember = "UserName";
           

        }

        private void dgvRack_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dgvRack.CurrentCell.ColumnIndex == 0)
                {
                   
                        if (!string.IsNullOrEmpty(Convert.ToString(dgvRack[0, dgvRack.CurrentCell.RowIndex].Value)))
                        {
                            string cur = Convert.ToString(dgvRack[0, dgvRack.CurrentCell.RowIndex].Value);
                            int st = 0;
                            if (dgvRack.Rows.Count > 1)
                            {
                                for (int i = 0; i < dgvRack.Rows.Count; i++)
                                {

                                    if (Convert.ToString(dgvRack[0, i].Value) == cur && dgvRack.CurrentCell.RowIndex != i)
                                    {
                                        st = 1;
                                    }

                                }
                            }
                            if (st == 0)
                            {
                                dgvRack.Focus();
                                dgvRack.Rows.Add(1);
                                dgvRack.Focus();
                                dgvRack.CurrentCell = dgvRack[0, dgvRack.CurrentCell.RowIndex + 1];
                                dgvRack.BeginEdit(true);
                            }
                            else
                            {
                                MessageBox.Show("Rack name should not Same");
                                dgvRack.CurrentCell = dgvRack[0, dgvRack.CurrentCell.RowIndex];
                            }
                      }
                        else
                        {
                            MessageBox.Show("Please enter rack name");
                            dgvRack.CurrentCell = dgvRack[0, dgvRack.CurrentCell.RowIndex];
                        }
                      

                   

                }
            }
        }

      

        private void dgvRack_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string headerText = dgvRack.Columns[dgvRack.CurrentCell.ColumnIndex].HeaderText;
            try
            {
                if (e.ColumnIndex == 0)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvRack[0, dgvRack.CurrentCell.RowIndex].Value)))
                    {
                        string cur=Convert.ToString(dgvRack[0, dgvRack.CurrentCell.RowIndex].Value);
                        int st = 0;
                        if (dgvRack.Rows.Count>1)
                        {
                            for (int i = 0; i < dgvRack.Rows.Count; i++)
                            {

                                if (Convert.ToString(dgvRack[0, i].Value) == cur && dgvRack.CurrentCell.RowIndex!=i)
                                {
                                    st = 1;
                                }

                            }
                        }
                        if (st==0)
                        {
                            dgvRack.Focus();
                        dgvRack.Rows.Add(1);
                        dgvRack.Focus();
                        dgvRack.CurrentCell = dgvRack[0, dgvRack.CurrentCell.RowIndex + 1];
                        dgvRack.BeginEdit(true);
                        }
                        else
                        {
                            dgvRack.CurrentCell = dgvRack[0, dgvRack.CurrentCell.RowIndex];
                        }
                    }
                    else
                    {
                        dgvRack.CurrentCell = dgvRack[0, dgvRack.CurrentCell.RowIndex];
                    }


                }
            }
            catch
            {

            }
        }

        private void ddlIncharge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ddlIncharge.SelectedIndex!=0)
            {
                dgvRack.Focus();
            }
        }
       
        
       
    }
}
