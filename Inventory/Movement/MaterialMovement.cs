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
using System.Runtime.InteropServices;

namespace Inventory
{
    public partial class MaterialMovement : Form
    {
        public bool edit = false;
        public TextBox tb;
        bool all = false;
        DataTable dtitems = new DataTable();
        public TextBox Quantitytomove1;
        string headid;
        string conn = Program.connection;
        string clickstatus = string.Empty;
        SqlConnection ObjConn = new SqlConnection(Program.connection);
        QuotationBal objQuotationbal = new QuotationBal();
        ProductMovementBal objProductMovementBal = new ProductMovementBal();
        private readonly MaterialMovementRackRepository rackRepository = new MaterialMovementRackRepository();
        string role1 = string.Empty;
        string srole = string.Empty;
        DataTable datatableitem;
        DataTable item3;
        public MaterialMovement()
        {
            InitializeComponent();


            pnlOrder.Visible = false;
            vLabel2.Visible = false;
            label23.Visible = false;
            lblrack.Visible = false;

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
            GetLacation();
            // ddlLocation.SelectedIndex = 0;
            LoadPorts();
            ConfigureRackColumns();
            dgvOrder.CellValueChanged += dgvOrder_CellValueChanged;
            dgvOrder.CurrentCellDirtyStateChanged += dgvOrder_CurrentCellDirtyStateChanged;
            LocationBind();
            //LoadPortsFloorCheckIN();
            Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            Txtitem.AutoCompleteCustomSource = AutoCompleteLoad();
            Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;
            bindsearchgrid();
            GetSearchSalesOrder();

            //dgvOrder.Rows.Add(1);
        }
        #region binddatas
        public void bindsearchgrid()
        {
            DataTable dt = ProductMovementBal.BindsearchGrid(); ;
            Dgvmovementsearch.DataSource = dt;
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            Dgvmovementsearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            Dgvmovementsearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvOrder.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvOrder.AlternatingRowsDefaultCellStyle.BackColor = Color.White;


        }
        //private void LoadPortsFloorCheckIN()
        //{
        //    dgvFloorCheckOut.Rows.Clear();
        //    dgvFloorCheckOut.ColumnCount = 7;

        //    dgvFloorCheckOut.Columns[6].Name = "MovedBy";
        //    dgvFloorCheckOut.Columns[0].Name = "S.NO";
        //    dgvFloorCheckOut.Columns[1].Name = "Items";

        //    dgvFloorCheckOut.Columns[3].Name = "Quantity";
        //    dgvFloorCheckOut.Columns[4].Name = "Productid";
        //    dgvFloorCheckOut.Columns[2].Name = "FromLocation";
        //    dgvFloorCheckOut.Columns[5].Name = "TransID";


        //    this.dgvFloorCheckOut.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


        //    // this.dgvFloorCheckOut.Columns[4].Visible = false;

        //    this.dgvFloorCheckOut.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    this.dgvFloorCheckOut.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    this.dgvFloorCheckOut.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    this.dgvFloorCheckOut.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    this.dgvFloorCheckOut.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    this.dgvFloorCheckOut.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //    this.dgvFloorCheckOut.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



        //    this.dgvFloorCheckOut.Columns[0].ReadOnly = true;
        //    this.dgvFloorCheckOut.Columns[1].ReadOnly = true;
        //    this.dgvFloorCheckOut.Columns[2].ReadOnly = true;
        //    this.dgvFloorCheckOut.Columns[3].ReadOnly = true;
        //    this.dgvFloorCheckOut.Columns[4].ReadOnly = true;
        //    this.dgvFloorCheckOut.Columns[5].ReadOnly = true;
        //    this.dgvFloorCheckOut.Columns[6].ReadOnly = true;
        //    this.dgvFloorCheckOut.Columns["TransID"].Visible = false;
        //    this.dgvFloorCheckOut.Columns["Productid"].Visible = false;
        //    this.dgvFloorCheckOut.Columns[4].Visible = false;
        //    this.dgvFloorCheckOut.Columns[5].ReadOnly = false;


        //    // this.dgvFloorCheckOut.Columns[6].ReadOnly = true;




        //    this.dgvFloorCheckOut.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;





        //    DataGridViewComboBoxColumn toloc = new DataGridViewComboBoxColumn();
        //    toloc.HeaderText = "ToLocation";
        //    toloc.DataPropertyName = "Location";
        //    toloc.FlatStyle = FlatStyle.Popup;
        //    dgvFloorCheckOut.Columns.Insert(3, toloc);
        //    //this.dgvFloorCheckOut.Columns[3].Width = 150;


        //    DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
        //    dgvCmb.ValueType = typeof(bool);
        //    dgvCmb.Name = "ChkFloorCheckIn";
        //    dgvCmb.HeaderText = "IsCheckOut";
        //    dgvCmb.FlatStyle = FlatStyle.Popup;

        //    dgvFloorCheckOut.Columns.Insert(5, dgvCmb);



        //    dgvFloorCheckOut.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //    foreach (DataGridViewColumn c in dgvFloorCheckOut.Columns)
        //    {
        //        c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
        //    }

        //    Rectangle resolution = Screen.PrimaryScreen.Bounds;
        //    int w = resolution.Width;
        //    int h = resolution.Height;

        //    if (w == 1024 && h == 768)
        //    {
        //        this.dgvFloorCheckOut.Columns[0].Width = 20;
        //        this.dgvFloorCheckOut.Columns[1].Width = 150;
        //        this.dgvFloorCheckOut.Columns[2].Width = 60;
        //        this.dgvFloorCheckOut.Columns[3].Width = 60;
        //        this.dgvFloorCheckOut.Columns[4].Width = 40;
        //        this.dgvFloorCheckOut.Columns[5].Width = 80;
        //        this.dgvFloorCheckOut.Columns[7].Width = 90;
        //    }
        //    else
        //    {
        //        dgvFloorCheckOut.Columns[0].Width = 50;
        //        dgvFloorCheckOut.Columns[1].Width = 150;
        //        dgvFloorCheckOut.Columns[2].Width = 100;
        //        dgvFloorCheckOut.Columns[3].Width = 100;
        //        dgvFloorCheckOut.Columns[4].Width = 70;
        //        dgvFloorCheckOut.Columns[5].Width = 90;
        //        dgvFloorCheckOut.Columns[7].Width = 90;


        //    }
        //}
        public void bindgridlocation()
        {

        }
        public void clear()
        {
            ddlLocation.Enabled = true;
            if (!string.IsNullOrEmpty(Program.Floor))
            {
                ddlLocation.SelectedValue = Program.Floor;
            }
            else
            {
                ddlLocation.SelectedIndex = 0;
            }


            dgvOrder.Rows.Clear();

            LoadPorts();
            LocationBind();

            for (int l = 0; l < dgvOrder.Rows.Count; l++)
            {
                dgvOrder["cmblocation", l].Value = "";
            }
            lblhidden.Text = string.Empty;
            this.ActiveControl = ddlLocation;


        }

        public DataTable GetUserMapedLocations()
        {
            DataTable dtLocation = new DataTable();
            DataSet objDs = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            using (SqlConnection sqlconn = new SqlConnection(conn))
            using (SqlCommand cmd = sqlconn.CreateCommand())
            {
                //cmd.CommandText = "select LocationName, LocationId, '" + Program.UserName + "' as InchargeBy, " + Program.userid + " as USerId, '' as RackName,'' as RackId from Location where LocationID in (select data from dbo.split((select FloorName from Users where UserId = " + Program.userid + "),',')) ";
                cmd.CommandText = "select LocationName, LocationId from location where locationid in(2,3,6)" ;

                cmd.CommandType = System.Data.CommandType.Text;
                sqlconn.Open();
                da = new SqlDataAdapter(cmd);
                da.Fill(objDs);
            }
            if (objDs.Tables.Count > 0)
            {
                dtLocation = objDs.Tables[0];
            }
            return dtLocation;
        }

        public void GetLacation()
        {

            //DataTable dtlocation = ProductMovementBal.GetLocation(); Function Override for Getting Location mapped with users
            DataTable dtlocation = GetUserMapedLocations();

            DataRow row = dtlocation.NewRow();
           
            ddlLocation.DataSource = dtlocation;
            ddlLocation.ValueMember = "LocationID";
            ddlLocation.DisplayMember = "LocationName";

            if (dtlocation.Rows.Count > 0)
            {
                ddlLocation.Enabled = true;
            }

            /*if (Program.Userrole == "Admin" || Program.UserName.ToLower() == "mythili" || Program.UserName.ToLower() == "devi" || Program.UserName.ToLower() == "mohanapriya")
            {
                ddlLocation.SelectedIndex = 0;
                ddlLocation.Enabled = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(Program.Floor))
                {
                    ddlLocation.SelectedValue = Convert.ToInt32(Program.Floor);
                    ddlLocation.Enabled = false;
                }
            }*/

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

        private void LocationBind()
        {

            cmblocation.DataSource = ProductMovementBal.GetLocation();
            cmblocation.ValueMember = "LocationID";
            cmblocation.DisplayMember = "LocationName";
        }
        public void GetDetialsBasedUser()
        {
            DataTable ds;
            if (Program.Userrole == "Admin")
            {
                ds = ProductMovementBal.BindMaterialTransMove(null);
            }
            else
            {
                ds = ProductMovementBal.BindMaterialTransMove(Program.Floor);
            }

            //  dgvFloorCheckOut.DataSource = dtlocation;




            //if (ds.Rows.Count > 0)
            //{

            //    dgvFloorCheckOut.Rows.Clear();
            //    int RowIndex = 0;
            //    for (int i = 0; i < ds.Rows.Count; i++)
            //    {
            //        if (Program.Floor != null)
            //        {
            //            if (ds.Rows[i]["ToLocation"].ToString() == Program.Floor.ToString())
            //            {
            //                dgvFloorCheckOut.Rows.Add();
            //                dgvFloorCheckOut.Rows[RowIndex].Cells[0].Value = Convert.ToString(ds.Rows[i]["Sino"]);
            //                dgvFloorCheckOut.Rows[RowIndex].Cells[1].Value = Convert.ToString(ds.Rows[i]["DisplayName"]);
            //                dgvFloorCheckOut.Rows[RowIndex].Cells["Quantity"].Value = Convert.ToString(ds.Rows[i]["Quantity"]);
            //                dgvFloorCheckOut.Rows[RowIndex].Cells["MovedBy"].Value = Convert.ToString(ds.Rows[i]["MovedBy"]);
            //                dgvFloorCheckOut.Rows[RowIndex].Cells["Productid"].Value = Convert.ToString(ds.Rows[i]["MaterailId"]);
            //                dgvFloorCheckOut.Rows[RowIndex].Cells["FromLocation"].Value = Convert.ToString(ds.Rows[i]["FromLocation"]);
            //                dgvFloorCheckOut.Rows[RowIndex].Cells["TransID"].Value = Convert.ToString(ds.Rows[i]["TransID"]);

            //                string vals = Convert.ToString(ds.Rows[i]["ToLocation"]);
            //                DataTable dt = ProductMovementBal.GetLocation();

            //                (dgvFloorCheckOut.Rows[RowIndex].Cells[3] as DataGridViewComboBoxCell).DataSource = dt;
            //                (dgvFloorCheckOut.Rows[RowIndex].Cells[3] as DataGridViewComboBoxCell).ValueMember = "LocationID";
            //                (dgvFloorCheckOut.Rows[RowIndex].Cells[3] as DataGridViewComboBoxCell).DisplayMember = "LocationName";
            //                (dgvFloorCheckOut.Rows[RowIndex].Cells[3] as DataGridViewComboBoxCell).Value = ds.Rows[i]["ToLocation"];
            //                //string val = Convert.ToString(dt.Rows[0][0]);
            //                //string val1 = Convert.ToString(dsRows[i]["Productid"]);
            //                //string v = getrack(val, val1);
            //                //dgvChecking.Rows[i].Cells["Rack"].Value = v;

            //                RowIndex++;

            //                this.dgvFloorCheckOut.Columns["TransID"].Visible = false;
            //                this.dgvFloorCheckOut.Columns[3].ReadOnly = true;

            //                Rectangle resolution = Screen.PrimaryScreen.Bounds;
            //                int w = resolution.Width;
            //                int h = resolution.Height;
            //                if (w == 1024 && h == 768)
            //                {
            //                    this.dgvFloorCheckOut.Columns[0].Width = 20;
            //                    this.dgvFloorCheckOut.Columns[1].Width = 150;
            //                    this.dgvFloorCheckOut.Columns[2].Width = 60;
            //                    this.dgvFloorCheckOut.Columns[3].Width = 60;
            //                    this.dgvFloorCheckOut.Columns[4].Width = 40;
            //                    this.dgvFloorCheckOut.Columns[5].Width = 80;
            //                    this.dgvFloorCheckOut.Columns[7].Width = 90;
            //                }
            //                else
            //                {
            //                    dgvFloorCheckOut.Columns[0].Width = 50;
            //                    dgvFloorCheckOut.Columns[1].Width = 250;
            //                    dgvFloorCheckOut.Columns[2].Width = 150;
            //                    dgvFloorCheckOut.Columns[3].Width = 150;
            //                    dgvFloorCheckOut.Columns[4].Width = 70;
            //                    dgvFloorCheckOut.Columns[5].Width = 90;
            //                    dgvFloorCheckOut.Columns[7].Width = 90;

            //                }





            //                dgvFloorCheckOut.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //                dgvFloorCheckOut.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //                dgvFloorCheckOut.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //                //dgvFloorCheckOut.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //                //dgvFloorCheckOut.DefaultCellStyle.BackColor = Color.Gainsboro;
            //                //dgvFloorCheckOut.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            //            }
            //        }
            //    }

            //}
            //else
            //{
            //    if (dgvFloorCheckOut.Rows.Count > 0)
            //    {
            //        dgvFloorCheckOut.Rows.Clear();
            //    }

            //}





        }
        public void itemdetails()
        {
            try
            {


                string s1 = Txtitem.Text;
                string s2 = Convert.ToString(ddlLocation.SelectedValue);
                string name = s1.Replace("'", "''");
                DataTable st = objQuotationbal.itemdetails(name, s2);


                if (st.Rows.Count > 0)
                {

                    lblitemcode.Text = Convert.ToString(st.Rows[0]["ItemCode"]);
                    if (lblitemcode.Text == "")
                    {
                        lblitemcode.Text = "0";
                    }


                    lblproductid.Text = Convert.ToString(st.Rows[0]["id"]);
                    if (lblproductid.Text == "")
                    {
                        lblproductid.Text = "0";
                    }

                    lblprice.Text = Convert.ToString(st.Rows[0]["Price"]);
                    if (lblprice.Text == "")
                    {
                        lblprice.Text = "0";
                    }


                    lbldemo.Text = "0";


                    lblrack.Text = Convert.ToString(st.Rows[0]["Stock"]);
                    if (lblrack.Text == "")
                    {
                        lblrack.Text = "0";
                    }
                    lbldisplay.Text = "0";
                    lblitemname.Text = "0";
                    lblservice.Text = "0";
                }

            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region grid
        private void LoadPorts()
        {
            dgvOrder.Rows.Clear();

            dgvOrder.Rows.Add(1);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvOrder.Columns[0].Width = 100;
                this.dgvOrder.Columns[1].Width = 250;
                this.dgvOrder.Columns[2].Width = 200;
                this.dgvOrder.Columns[3].Width = 150;
            }
            else
            {
                this.dgvOrder.Columns[0].Width = 100;
                this.dgvOrder.Columns[1].Width = 400;
                this.dgvOrder.Columns[2].Width = 50;
                this.dgvOrder.Columns[3].Width = 150;
            }

            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvOrder.Columns[1].ReadOnly = true;


            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



            this.dgvOrder.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvOrder.Columns[3].ReadOnly = false;



            ConfigureRackColumns();




            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Verdana", 12F, GraphicsUnit.Pixel);
            }



        }

        private void ConfigureRackColumns()
        {
            if (dgvOrder.Columns.Contains("cmblocation"))
            {
                dgvOrder.Columns["cmblocation"].Visible = false;
            }

            if (dgvOrder.Columns.Contains("FromRack"))
            {
                dgvOrder.Columns["FromRack"].DisplayIndex = 4;
                dgvOrder.Columns["FromRack"].Width = 170;
                dgvOrder.Columns["FromRack"].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            if (dgvOrder.Columns.Contains("ToRack"))
            {
                dgvOrder.Columns["ToRack"].DisplayIndex = 5;
                dgvOrder.Columns["ToRack"].Width = 170;
                dgvOrder.Columns["ToRack"].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            if (dgvOrder.Columns.Contains("ProductId"))
            {
                dgvOrder.Columns["ProductId"].Visible = false;
            }

            if (dgvOrder.Columns.Contains("TransId"))
            {
                dgvOrder.Columns["TransId"].Visible = false;
            }
        }

        private DataTable GetRackComboSource(int productId, bool sourceRack)
        {
            DataTable source = new DataTable();
            source.Columns.Add("RackId", typeof(int));
            source.Columns.Add("RackDisplayName", typeof(string));
            source.Columns.Add("LocationId", typeof(int));
            source.Columns.Add("AvailableQuantity", typeof(decimal));

            if (productId <= 0)
            {
                return source;
            }

            DataTable racks = rackRepository.GetRackAvailability(productId);
            foreach (DataRow rack in racks.Rows)
            {
                decimal availableQuantity = ToDecimal(rack["AvailableQuantity"]);
                if (sourceRack && availableQuantity <= 0)
                {
                    continue;
                }

                string displayName = Convert.ToString(rack["LocationName"]) + " / " + Convert.ToString(rack["RackCaption"]);
                if (sourceRack)
                {
                    displayName = displayName + " (" + availableQuantity.ToString("0.###") + ")";
                }

                source.Rows.Add(Convert.ToInt32(rack["RackId"]), displayName, Convert.ToInt32(rack["LocationId"]), availableQuantity);
            }

            return source;
        }

        private void PrepareRackCells(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvOrder.Rows.Count)
            {
                return;
            }

            int productId;
            if (!int.TryParse(Convert.ToString(dgvOrder.Rows[rowIndex].Cells["ProductId"].Value), out productId))
            {
                productId = 0;
            }

            DataTable fromSource = GetRackComboSource(productId, true);
            DataTable toSource = GetRackComboSource(productId, false);
            BindRackCell(rowIndex, "FromRack", fromSource);
            BindRackCell(rowIndex, "ToRack", toSource);
        }

        private void BindRackCell(int rowIndex, string columnName, DataTable source)
        {
            DataGridViewComboBoxCell cell = dgvOrder.Rows[rowIndex].Cells[columnName] as DataGridViewComboBoxCell;
            if (cell == null)
            {
                return;
            }

            object currentValue = cell.Value;
            cell.DataSource = source;
            cell.ValueMember = "RackId";
            cell.DisplayMember = "RackDisplayName";

            if (currentValue != null && currentValue != DBNull.Value && source.Select("RackId = " + Convert.ToString(currentValue)).Length > 0)
            {
                cell.Value = currentValue;
            }
            else
            {
                cell.Value = null;
            }
        }

        private void MoveToRackCell(string columnName)
        {
            if (dgvOrder.CurrentCell == null)
            {
                return;
            }

            PrepareRackCells(dgvOrder.CurrentCell.RowIndex);
            dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[columnName];
        }

        private void MoveToNextMovementRow()
        {
            int rowIndex = dgvOrder.CurrentCell.RowIndex;
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[rowIndex + 1].Cells[5].Value)))
                {
                    dgvOrder.CurrentCell = dgvOrder[1, rowIndex + 1];
                    return;
                }
            }
            catch
            {
            }

            dgvOrder.Rows.Add(1);
            LocationBind();
            dgvOrder.CurrentCell = dgvOrder[1, rowIndex + 1];
            if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["cmblocation"].Value)))
            {
                dgvOrder["cmblocation", dgvOrder.CurrentCell.RowIndex].Value = "";
            }
        }

        private void SetHiddenDestinationLocationFromRack(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvOrder.Rows.Count)
            {
                return;
            }

            DataGridViewComboBoxCell toRackCell = dgvOrder.Rows[rowIndex].Cells["ToRack"] as DataGridViewComboBoxCell;
            if (toRackCell == null || toRackCell.Value == null || toRackCell.DataSource == null)
            {
                return;
            }

            DataTable source = toRackCell.DataSource as DataTable;
            if (source == null)
            {
                return;
            }

            DataRow[] selectedRows = source.Select("RackId = " + Convert.ToString(toRackCell.Value));
            if (selectedRows.Length > 0)
            {
                dgvOrder.Rows[rowIndex].Cells["cmblocation"].Value = selectedRows[0]["LocationId"];
            }
        }

        private decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value || Convert.ToString(value).Trim().Length == 0)
            {
                return 0;
            }

            decimal result;
            if (decimal.TryParse(Convert.ToString(value), out result))
            {
                return result;
            }

            return 0;
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

            else if (dgvOrder.CurrentCell.ColumnIndex == 3)
            {


                if (e.Control is TextBox)
                {
                    Quantitytomove1 = e.Control as TextBox;

                    Quantitytomove1.MaxLength = 6;
                    Quantitytomove1.KeyPress += new KeyPressEventHandler(Quantitytomove1_press);
                }

            }
        }
        protected void Quantitytomove1_press(object sender, KeyPressEventArgs e)
        {
            string headerText = dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].HeaderText;
            if (headerText == "Quantity to move")
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
        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string headerText = dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].HeaderText;
            try
            {
                if (e.ColumnIndex == 2)
                {
                    dgvOrder.Focus();
                    edit = true;
                    //  dgvOrder.CurrentCell = dgvOrder[1,];
                }


                if (headerText == "Quantity to move")
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
                    pnsearch.Visible = false; ;
                }

                if (dgvOrder.Columns[e.ColumnIndex].Name == "FromRack" || dgvOrder.Columns[e.ColumnIndex].Name == "ToRack")
                {
                    PrepareRackCells(e.RowIndex);
                }

                if (e.ColumnIndex == 4)
                {
                    dgvOrder.Focus();

                    int toloc = Convert.ToInt32(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
                    int fromloc = Convert.ToInt32(ddlLocation.SelectedValue);
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                    {
                        if (toloc == fromloc)
                        {
                            //MessageBox.Show("From and to location should not be same");
                            dgvOrder.CurrentCell = dgvOrder[4, dgvOrder.CurrentCell.RowIndex];
                        }
                        else
                        {

                            dgvOrder.Rows.Add(1);
                            LocationBind();
                            dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                            if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["cmblocation"].Value)))
                            {
                                dgvOrder["cmblocation", dgvOrder.CurrentCell.RowIndex].Value = 0;
                            }


                        }

                    }

                    else
                    {
                        MessageBox.Show("Enter product to move");
                    }

                }



            }
            catch
            {

            }

        }

        private void dgvOrder_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvOrder.IsCurrentCellDirty && dgvOrder.CurrentCell != null)
            {
                string columnName = dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].Name;
                if (columnName == "FromRack" || columnName == "ToRack")
                {
                    dgvOrder.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }

        private void dgvOrder_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (dgvOrder.Columns[e.ColumnIndex].Name == "ToRack")
            {
                SetHiddenDestinationLocationFromRack(e.RowIndex);
            }
        }

        private void dgvOrder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyData == Keys.Down || e.KeyData == Keys.Up)
                {

                    string headerText = dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].HeaderText;

                    if (headerText == "Location" || headerText == "From Rack" || headerText == "To Rack")
                    {
                        SendKeys.Send("{F4}");
                        e.SuppressKeyPress = true;
                    }
                }

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
                        dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 2)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[3, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 3)
                    {
                        dgvOrder.Focus();
                        if (string.IsNullOrEmpty(Convert.ToString(dgvOrder[3, dgvOrder.CurrentCell.RowIndex].Value)))
                        {
                            MessageBox.Show("Quantity Should not be empty");

                            dgvOrder.CurrentCell = dgvOrder[3, dgvOrder.CurrentCell.RowIndex];
                        }
                        else
                        {

                            MoveToRackCell("FromRack");
                        }


                    }
                    else if (dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].Name == "FromRack")
                    {
                        MoveToRackCell("ToRack");
                    }
                    else if (dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].Name == "ToRack")
                    {
                        SetHiddenDestinationLocationFromRack(dgvOrder.CurrentCell.RowIndex);
                        MoveToNextMovementRow();
                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 4)
                    {
                        dgvOrder.Focus();

                        int toloc = Convert.ToInt32(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
                        int fromloc = Convert.ToInt32(ddlLocation.SelectedValue);
                        if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                        {
                            if (toloc == fromloc)
                            {
                                MessageBox.Show("From and to location should not be same");
                                dgvOrder.CurrentCell = dgvOrder[4, dgvOrder.CurrentCell.RowIndex];
                            }
                            else
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value)))
                                    {
                                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                                    }
                                    else
                                    {
                                        dgvOrder.Rows.Add(1);
                                        LocationBind();
                                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                                        if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["cmblocation"].Value)))
                                        {
                                            dgvOrder["cmblocation", dgvOrder.CurrentCell.RowIndex].Value = "";
                                        }
                                    }
                                }
                                catch
                                {
                                    dgvOrder.Rows.Add(1);
                                    LocationBind();
                                    dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["cmblocation"].Value)))
                                    {
                                        dgvOrder["cmblocation", dgvOrder.CurrentCell.RowIndex].Value = "";
                                    }
                                }
                            }

                        }
                        else
                        {
                            MessageBox.Show("Enter product to move");
                        }

                    }

                }

                if (e.KeyData == Keys.Tab)
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
                        dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 2)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[3, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 3)
                    {
                        dgvOrder.Focus();
                        if (string.IsNullOrEmpty(Convert.ToString(dgvOrder[3, dgvOrder.CurrentCell.RowIndex].Value)))
                        {
                            MessageBox.Show("Quantity Should not be empty");

                            dgvOrder.CurrentCell = dgvOrder[3, dgvOrder.CurrentCell.RowIndex];
                        }
                        else
                        {

                            MoveToRackCell("FromRack");
                        }


                    }
                    else if (dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].Name == "FromRack")
                    {
                        MoveToRackCell("ToRack");
                    }
                    else if (dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].Name == "ToRack")
                    {
                        SetHiddenDestinationLocationFromRack(dgvOrder.CurrentCell.RowIndex);
                        MoveToNextMovementRow();
                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 4)
                    {
                        dgvOrder.Focus();

                        int toloc = Convert.ToInt32(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
                        int fromloc = Convert.ToInt32(ddlLocation.SelectedValue);
                        if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                        {
                            if (toloc == fromloc)
                            {
                                MessageBox.Show("From and to location should not be same");
                                dgvOrder.CurrentCell = dgvOrder[4, dgvOrder.CurrentCell.RowIndex];
                            }
                            else
                            {
                                //dgvOrder.Rows.Add(1);
                                LocationBind();
                                dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                                if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["cmblocation"].Value)))
                                {
                                    dgvOrder["cmblocation", dgvOrder.CurrentCell.RowIndex].Value = "";
                                }
                            }

                        }
                        else
                        {
                            MessageBox.Show("Enter product to move");
                        }

                    }
                }
            }
            catch
            {

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
        private void Dgvmovementsearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (MainTabSalesBill.SelectedIndex == 0)
                {
                    string s = Convert.ToString(Dgvmovementsearch.Rows[Dgvmovementsearch.CurrentCell.RowIndex].Cells[0].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        BindMaterialTranscationMain(s);
                    }
                    else
                    {
                        clear();
                    }

                }
            }
        }
        public void BindMaterialTranscationMain(string s)
        {

            DataTable ds = objProductMovementBal.BindMaterialTranscationMain1(s);
            if (ds.Rows.Count > 0)
            {
                lblhidden.Text = s;
                if (string.IsNullOrEmpty(Convert.ToString(ds.Rows[0]["FromLocation"])))
                {
                    ddlLocation.SelectedValue = ds.Rows[0]["FromLocation"];
                    ddlLocation.Enabled = true;
                }
                else
                {
                    ddlLocation.SelectedValue = ds.Rows[0]["FromLocation"];
                    ddlLocation.Enabled = false;
                }



            }
            else
            {
                panel2.Enabled = true;
                clear();
            }
            if (ds.Rows.Count > 0)
            {
                dgvOrder.Rows.Clear();

                dgvOrder.Rows.Add(ds.Rows.Count);

                for (int i = 0; i < ds.Rows.Count; i++)
                {

                    dgvOrder.Rows[i].Cells[0].Value = i + 1;
                    dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["ItemName"]);
                    dgvOrder.Rows[i].Cells[2].Value = ds.Rows[i]["Stock"];
                    dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds.Rows[i]["Quantity"]);
                    dgvOrder.Rows[i].Cells[4].Value = ds.Rows[i]["ToLocation"];
                    dgvOrder.Rows[i].Cells[5].Value = Convert.ToString(ds.Rows[i]["MaterailId"]);
                    dgvOrder.Rows[i].Cells[6].Value = Convert.ToString(ds.Rows[i]["TransID"]);
                    lblproductid.Text = Convert.ToString(ds.Rows[i]["MaterailId"]);
                    headid = Convert.ToString(ds.Rows[i]["HeadID"]);
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
        #endregion

        #region Panel
        private void textbox_Change(object sender, EventArgs e)
        {

            if (dgvOrder.CurrentCell.ColumnIndex == 0)
            {

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

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}

            return str;
        }
        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            itemdetails();
        }
        #endregion

        #region processcmd
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
            if (ddlLocation.Focused)
            {
                if (keyData == (Keys.Tab))
                {

                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[1, 0];

                    return true;
                }
            }
            if (productsearchbttn.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    btnsave.Focus();
                    this.ActiveControl = btnsave;
                    btnsave.PerformClick();
                }
            }
            if (keyData == Keys.Enter)
            {

                IntPtr wndHandle = GetFocus();
                Control focusedControl = FromChildHandle(wndHandle);
                if (focusedControl.Name == "ddlLocation")
                {
                    dgvOrder.CurrentCell = dgvOrder[1, 0];
                }

                if (dgvOrder.CurrentCell.ColumnIndex == 1)
                {
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex];

                }
                if (dgvOrder.CurrentCell.ColumnIndex == 3)
                {
                    dgvOrder.Focus();
                    dgvOrder.Select();
                    MoveToRackCell("FromRack");

                }


            }

            else if (keyData == Keys.Tab)
            {
                try
                {
                    if (dgvOrder.CurrentCell.ColumnIndex == 1 && tb.Focused)
                    {
                        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex + 1, dgvOrder.CurrentCell.RowIndex];

                        if (datatableitem.Rows.Count <= 0 && item3.Rows.Count <= 0)
                        {
                            dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex];
                        }
                        else
                        {
                            dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex];
                        }
                    }
                    if (dgvOrder.CurrentCell.ColumnIndex == 4)
                    {
                        dgvOrder.Focus();

                        int toloc = Convert.ToInt32(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
                        int fromloc = Convert.ToInt32(ddlLocation.SelectedValue);
                        if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                        {
                            if (toloc == fromloc)
                            {
                                MessageBox.Show("From and to location should not be same");
                                dgvOrder.CurrentCell = dgvOrder[4, dgvOrder.CurrentCell.RowIndex];
                            }
                            else
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex + 1].Cells[5].Value)))
                                    {
                                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                                    }
                                    else
                                    {
                                        dgvOrder.Rows.Add(1);
                                        LocationBind();
                                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                                        if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["cmblocation"].Value)))
                                        {
                                            dgvOrder["cmblocation", dgvOrder.CurrentCell.RowIndex].Value = "";
                                        }
                                    }
                                }
                                catch
                                {
                                    dgvOrder.Rows.Add(1);
                                    LocationBind();
                                    dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells["cmblocation"].Value)))
                                    {
                                        dgvOrder["cmblocation", dgvOrder.CurrentCell.RowIndex].Value = "";
                                    }
                                }
                            }

                        }
                        else
                        {
                            MessageBox.Show("Enter product to move");
                        }

                    }
                    else if (dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].Name == "FromRack")
                    {
                        MoveToRackCell("ToRack");
                    }

                    else if (dgvOrder.Columns[dgvOrder.CurrentCell.ColumnIndex].Name == "ToRack")
                    {
                        SetHiddenDestinationLocationFromRack(dgvOrder.CurrentCell.RowIndex);
                        MoveToNextMovementRow();
                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 2)
                    {
                        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex + 1, dgvOrder.CurrentCell.RowIndex];
                    }

                    else if (dgvOrder.CurrentCell.ColumnIndex == 3)
                    {
                        MoveToRackCell("FromRack");
                    }
                }
                catch
                {
                    //  btnsave.Focus();
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region click
        private void Btnsubmit_Click(object sender, EventArgs e)
        {

        }

        private void Txtitem_TextChanged_1(object sender, EventArgs e)
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
                    lblprice.Text = Convert.ToString(st.Rows[0]["Price"]);
                    if (lblprice.Text == "")
                    {
                        lblprice.Text = "0";
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
                    lblprice.Text = "0";

                }




            }
            catch (Exception e)
            {

            }

        }

        private void Txtitem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {

                if (!string.IsNullOrEmpty(lblproductid.Text) && lblproductid.Text != "0" && !string.IsNullOrEmpty(lblitemname.Text) && lblitemname.Text != "0")
                {
                    int rowindex = Convert.ToInt32(lblrowindex.Text);
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3];
                    dgvOrder.Rows[rowindex].Cells[5].Value = lblproductid.Text;
                    dgvOrder.Rows[rowindex].Cells[1].Value = lblitemname.Text.ToUpper();
                    int selectedProductId = Convert.ToInt32(lblproductid.Text);
                    dgvOrder.Rows[rowindex].Cells[2].Value = rackRepository.GetCurrentProductStock(selectedProductId).ToString("0.###");
                    //dgvOrder.Rows[rowindex].Cells[3].Value = 1;
                    double val = Convert.ToDouble(lblprice.Text);
                    dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                    PrepareRackCells(rowindex);
                    pnsearch.Visible = false;
                    lblproductid.Text = string.Empty;
                    Txtitem.Text = string.Empty;
                    lblitemcode.Text = "0";
                    lblprice.Text = "0";
                    lbldisplay.Text = "0";
                    lbldemo.Text = "0";
                    lblservice.Text = "0";
                    lblitemname.Text = string.Empty;
                    lblrack.Text = "0";
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3];

                }
                else if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value)))
                {
                    MessageBox.Show("Please Enter Product Name");
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1];
                    Txtitem.Focus();
                }
                else
                {
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3];
                }



            }
        }

        private bool Validation()
        {
            bool status = true;
            string message = "";
            int i = 0;

            if (ddlLocation.Text == "--Select--" || string.IsNullOrEmpty(ddlLocation.Text))
            {
                i++;
                message = message + "* Please Select from location" + "\n";
                if (i == 1)
                    this.ActiveControl = ddlLocation;
            }
            string val = Convert.ToString(dgvOrder[1, 0].Value);
            if (string.IsNullOrEmpty(val))
            {
                i++;
                message = message + "* Please move atleast one product" + "\n";
                if (i == 1)
                    dgvOrder.CurrentCell = dgvOrder.Rows[0].Cells[1];
            }


            for (int j = 0; j < dgvOrder.Rows.Count; j++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[j].Cells[1].Value)))
                {
                    int productId;
                    int fromRackId;
                    int toRackId;
                    decimal quantity;

                    if (!int.TryParse(Convert.ToString(dgvOrder.Rows[j].Cells["ProductId"].Value), out productId) || productId <= 0)
                    {
                        i++;
                        message = message + "* Please select a valid product in row " + (j + 1).ToString() + "\n";
                        if (i == 1)
                            dgvOrder.CurrentCell = dgvOrder.Rows[j].Cells["Items"];
                        continue;
                    }

                    if (!decimal.TryParse(Convert.ToString(dgvOrder.Rows[j].Cells["Quantitytomove"].Value), out quantity) || quantity <= 0)
                    {
                        i++;
                        message = message + "* Please enter valid quantity in row " + (j + 1).ToString() + "\n";
                        if (i == 1)
                            dgvOrder.CurrentCell = dgvOrder.Rows[j].Cells["Quantitytomove"];
                        continue;
                    }

                    if (!int.TryParse(Convert.ToString(dgvOrder.Rows[j].Cells["FromRack"].Value), out fromRackId) || fromRackId <= 0)
                    {
                        i++;
                        message = message + "* Please select From Rack in row " + (j + 1).ToString() + "\n";
                        if (i == 1)
                            dgvOrder.CurrentCell = dgvOrder.Rows[j].Cells["FromRack"];
                        continue;
                    }

                    if (!int.TryParse(Convert.ToString(dgvOrder.Rows[j].Cells["ToRack"].Value), out toRackId) || toRackId <= 0)
                    {
                        i++;
                        message = message + "* Please select To Rack in row " + (j + 1).ToString() + "\n";
                        if (i == 1)
                            dgvOrder.CurrentCell = dgvOrder.Rows[j].Cells["ToRack"];
                        continue;
                    }

                    if (fromRackId == toRackId)
                    {
                        i++;
                        message = message + "* From Rack and To Rack should not be same in row " + (j + 1).ToString() + "\n";
                        if (i == 1)
                            dgvOrder.CurrentCell = dgvOrder.Rows[j].Cells["ToRack"];
                        continue;
                    }

                    decimal availableQuantity = rackRepository.GetCurrentRackStock(productId, fromRackId);
                    if (quantity > availableQuantity)
                    {
                        i++;
                        message = message + "* Quantity exceeds From Rack stock in row " + (j + 1).ToString() + "\n";
                        if (i == 1)
                            dgvOrder.CurrentCell = dgvOrder.Rows[j].Cells["Quantitytomove"];
                    }
                }
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

        //private bool ValidationIN()
        //{
        //    bool status = true;
        //    string message = "";
        //    int i = 0;


        //    bool st = true;

        //    for (int j = 0; j < dgvFloorCheckOut.Rows.Count; j++)
        //    {


        //        if (Convert.ToBoolean(dgvFloorCheckOut.Rows[j].Cells[5].Value) != true)
        //        {
        //            st = false;
        //        }
        //    }
        //    if (st == false)
        //    {
        //        i++;
        //        message = message + "* Please Verify The Quantity" + "\n";
        //        if (i == 1)
        //            dgvFloorCheckOut.CurrentCell = dgvFloorCheckOut.Rows[0].Cells[1];
        //    }


        //    if (!string.IsNullOrEmpty(message))
        //    {
        //        MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
        //        status = false;
        //    }
        //    return status;
        //}

        private void btnsave_Click(object sender, EventArgs e)
        {
            string s = MainTabSalesBill.SelectedTab.Name;
            if (s == "TabNew")
            {
                if (Validation())
                {
                    save();
                    bindsearchgrid();
                    GetSearchSalesOrder();
                    // search("mn.MovedBy", "", "mn.MovedOn", "Today", role1, Program.UserName);
                    clear();
                }
            }
            //else if (s == "TabFloorceckin")
            //{
            //    if (ValidationIN())
            //    {
            //        saveIn();
            //        GetDetialsBasedUser();
            //        GetSearchSalesOrder();

            //        //  search("mn.MovedBy", "", "mn.MovedOn", "Today", role1, Program.UserName);


            //    }

            //}


        }


        public void itemdetailsval(string s)
        {

            try
            {
                dtitems = new DataTable();
                string s1 = s.Trim();
                string s2 = Convert.ToString("");
                string name = s1.Replace("'", "''");


                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                dtitems = objQuotationbal.itemdetails(name, s2);
                Program.dtitems = dtitems;


            }
            catch (Exception e)
            {

            }

        }
        public void save()
        {

            try
            {
                DataTable rackDetails = rackRepository.CreateSimpleRackDetailTable();
                int lineNo = 1;
                for (int i = 0; i < dgvOrder.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[1].Value)))
                    {
                        DataRow row = rackDetails.NewRow();
                        row["LineNo"] = lineNo;
                        row["ProductId"] = Convert.ToInt32(dgvOrder.Rows[i].Cells["ProductId"].Value);
                        row["FromRackId"] = Convert.ToInt32(dgvOrder.Rows[i].Cells["FromRack"].Value);
                        row["ToRackId"] = Convert.ToInt32(dgvOrder.Rows[i].Cells["ToRack"].Value);
                        row["Quantity"] = Convert.ToDecimal(dgvOrder.Rows[i].Cells["Quantitytomove"].Value);
                        rackDetails.Rows.Add(row);
                        lineNo++;
                    }
                }

                string res = rackRepository.SaveRackWiseMovement(Program.UserName, lblhidden.Text, rackDetails);
                if (!string.IsNullOrEmpty(res))
                {
                    MessageBox.Show(res);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }
        //public void saveIn()
        //{
        //    string res1 = string.Empty;
        //    DateTime myDateTime = DateTime.Now;
        //    string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd");
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("TransId", typeof(int));
        //    dt.Columns.Add("TranscationType", typeof(string));
        //    dt.Columns.Add("TranscationDate", typeof(DateTime));
        //    dt.Columns.Add("MaterailId", typeof(int));
        //    dt.Columns.Add("Quantity", typeof(decimal));
        //    dt.Columns.Add("LocationId", typeof(int));
        //    dt.Columns.Add("Type", typeof(string));
        //    if (dt.Rows.Count > 0)
        //    {
        //        dt.Rows.Clear();
        //    }

        //    for (int i = 0; i < dgvFloorCheckOut.Rows.Count; i++)
        //    {

        //        DataRow dr = dt.NewRow();
        //        dr["TransId"] = dgvFloorCheckOut.Rows[i].Cells["TransID"].Value.ToString();
        //        dr["TranscationType"] = "Product Movement";
        //        dr["TranscationDate"] = myDateTime.ToString("yyyy-MM-dd");
        //        dr["MaterailId"] = dgvFloorCheckOut.Rows[i].Cells["Productid"].Value.ToString();
        //        dr["Quantity"] = dgvFloorCheckOut.Rows[i].Cells["Quantity"].Value.ToString();
        //        dr["LocationId"] = Convert.ToInt32(dgvFloorCheckOut.Rows[i].Cells[3].Value);
        //        dr["Type"] = "IN";

        //        dt.Rows.Add(dr);
        //        int st = ProductMovementBal.updateTransMoveDetails(dgvFloorCheckOut.Rows[i].Cells["TransID"].Value.ToString());
        //    }

        //    if (dt.Rows.Count > 0)
        //    {
        //        res1 = ProductMovementBal.SaveMaterialTranscation(dt);
        //    }
        //    if (res1 == "1")
        //    {
        //        itemdetailsval("");
        //        MessageBox.Show("Successfully Product MovedIn");
        //    }
        //}
        private void btnnew_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region load
        private void MaterialMovement_Load(object sender, EventArgs e)
        {
            //  search("mn.MovedBy", "", "mn.MovedOn", "Today", role1, Program.UserName);

            this.ActiveControl = ddlLocation;
            LocationBind();

            for (int l = 0; l < dgvOrder.Rows.Count; l++)
            {
                dgvOrder["cmblocation", l].Value = "";
            }
        }
        #endregion




        //search("mn.MovedBy", firstvalue1, "mn.MovedOn", secondvalue1, role1, Program.userid);



        //public void search(string firstname, string firstvalue, string secondname, string secondvalue, string role, string UserId)
        //{
        //    ProductMovementBal obj = new ProductMovementBal();

        //    DataTable dt = obj.searchcashPayment(firstname, firstvalue, secondname, secondvalue, role1, Program.UserName);
        //    Dgvmovementsearch.Columns.Clear();
        //    Dgvmovementsearch.DataSource = dt;
        //    lblItemCount.Text = Convert.ToString(dt.Rows.Count);
        //    Dgvmovementsearch.Columns["Id"].Visible =false;
        //    Dgvmovementsearch.Columns["MovedBy"].HeaderText = "Moved By";
        //    Dgvmovementsearch.Columns["MovedOn"].HeaderText = "Moved On";
        //    //Dgvmovementsearch.Columns["ItemCode"].HeaderText = "ItemCode";



        //    Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
        //    Dgvmovementsearch.DefaultCellStyle.BackColor = Color.Gainsboro;
        //    Dgvmovementsearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        //    Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //}

        // #endregion

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

        private void productsearchbttn_Click(object sender, EventArgs e)
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

        private void MainTabSalesBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MainTabSalesBill.SelectedIndex == 0)
            {
                GetLacation();
                btnclear.Enabled = true;
                btnnew.Enabled = true;
            }
            else if (MainTabSalesBill.SelectedIndex == 1)
            {
                GetDetialsBasedUser();
                btnclear.Enabled = false;
                btnnew.Enabled = false;
            }
        }

        private void MaterialMovement_FormClosing(object sender, FormClosingEventArgs e)
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

        private void pnlCalender_Paint(object sender, PaintEventArgs e)
        {

        }

        private void GetSearchSalesOrder()
        {

            DateTime FromDate = new DateTime(dtfromdate.Value.Year, dtfromdate.Value.Month, dtfromdate.Value.Day);
            DateTime ToDate = new DateTime(DTPTodate.Value.Year, DTPTodate.Value.Month, DTPTodate.Value.Day);

            string Iscombined = string.Empty;
            if (all == true)
            {
                Iscombined = null;
            }
            else
            {
                if (cmbIscombined.Checked)
                {
                    Iscombined = "1";
                }
                else
                {
                    Iscombined = "0";
                }
            }
            DataTable dt = objQuotationbal.searchMove(FromDate, ToDate, Convert.ToInt32(Iscombined));
            Dgvmovementsearch.DataSource = dt;
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
            Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            Dgvmovementsearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            Dgvmovementsearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvOrder.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvOrder.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //Dgvmovementsearch.Rows.Clear();
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    Dgvmovementsearch.Rows.Add();
            //    Dgvmovementsearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["Order No"]);
            //    Dgvmovementsearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Customer"]);
            //    Dgvmovementsearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Order Date"]);
            //}

            //lblItemCount.Text = Convert.ToString(dt.Rows.Count);


            //Dgvmovementsearch.Columns["Order No"].Visible = true;
            ////Dgvmovementsearch.Columns["CustomerName"].Visible = false;
            ////Dgvmovementsearch.Columns["Order Date"].Visible = false;
            //Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            //Dgvmovementsearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            //Dgvmovementsearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            //Dgvmovementsearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            all = false;


        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            GetSearchSalesOrder();
        }

        //private void dgvFloorCheckOut_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (dgvFloorCheckOut.CurrentCell.RowIndex == dgvFloorCheckOut.Rows.Count - 1)
        //        {
        //            if (e.KeyData == Keys.Enter)
        //            {
        //                btnsave.Focus();
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}

        private void dgvOrder_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvFloorCheckOut_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvOrder_KeyPress(object sender, KeyPressEventArgs e)
        {


        }

    }
}
