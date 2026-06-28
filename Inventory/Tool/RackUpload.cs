using InvBal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
namespace Inventory.Tool
{
    public partial class RackUpload : Form
    {
        string UserId = Program.userid;
        string validationError = "";

        System.Data.DataTable monthData = new System.Data.DataTable();
        CashEndCloseBAL ObjCashEndCloseBAL = new CashEndCloseBAL();
        public RackUpload()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void RackUpload_Load(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            label3.Text = "0";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            openFileDialog1.Filter = "*.xls|*.xls|*.xlsx|*.xlsx";
            DialogResult oResult = openFileDialog1.ShowDialog();
            if (oResult == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                string fileExtension = Path.GetExtension(openFileDialog1.FileName);
                Random rnd = new Random();
                string strConn = "";
                string fileLocation = openFileDialog1.FileName;
                switch (fileExtension)
                {
                    case ".xls":
                        //Excel 1997-2003  
                        strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + fileLocation + ";" + "Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
                        break;
                }
                BindData(strConn);
            }
        }

        private System.Data.DataTable getSheetData(string strConn, string sheet)
        {
            OleDbConnection objConn;
            OleDbDataAdapter oleDA;
            System.Data.DataTable dt = new System.Data.DataTable();
            objConn = new OleDbConnection(strConn);
            objConn.Open();
            DataTable sheetTable = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string query = "select * from [" + sheetTable.Rows[0]["TABLE_NAME"].ToString() + "]";
            oleDA = new OleDbDataAdapter(query, objConn);
            oleDA.Fill(dt);
            objConn.Close();
            oleDA.Dispose();
            objConn.Dispose();
            return dt;
        }

        public DataTable GetPriceData(string PriceDataId)
        {

            DataTable GetData = ObjCashEndCloseBAL.GetPriceDataId(PriceDataId);
            if (GetData.Rows.Count > 0)
            {

            }
            else
            {

            }
            return GetData;
        }
        private void BindData(string strConn)
        {
            try
            {

                string sheetName = openFileDialog1.FileName;
                System.Data.DataTable dataSheets = getSheetData(strConn, sheetName);
                ProcessMonthData(dataSheets);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }


        private void ProcessMonthData(DataTable sheetData)
        {
            DataTable filterDataPrice = sheetData;
            //Validate Fields
            if (ValidatePriceFields(filterDataPrice) == true)
            {
                string SortByVal = string.Empty;
                SortByVal = "Id";
                string SortOrderVal = "asc";

                filterDataPrice.Columns.Add("Comments");
                bool validation = true;
                foreach (DataRow r in filterDataPrice.Rows)
                {



                    if (r["ItemName"].ToString() != "")
                    {



                    }
                    else
                    {
                        string Id = r["Id"].ToString();
                        string test = r["ItemName"].ToString();
                        string Check = "";

                        if (test != "")
                        {




                        }
                        else
                        {
                            Check += "ItemName Not empty,";
                            r["Comments"] = Check;
                            validation = false;

                            button2.Enabled = false;
                        }

                    }

                }

                dataGridView1.DataSource = filterDataPrice;
                dataGridView1.Columns["Id"].Width = 50;

                dataGridView1.Columns["ItemName"].Width = 300;
                dataGridView1.Columns["Rack"].Width = 300;
                dataGridView1.Columns["Comments"].Width = 450;

                dataGridView1.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Rack"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Comments"].SortMode = DataGridViewColumnSortMode.NotSortable;


                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    //string value = Convert.ToString(row.Cells[1].Value);
                    //if (Convert.ToInt16(row.Cells[1].Value) < 0)
                    //{
                    //    row.DefaultCellStyle.BackColor = Color.IndianRed;
                    //}
                    //else if (Convert.ToInt16(row.Cells[1].Value) == 0)
                    //{
                    //    row.DefaultCellStyle.BackColor = Color.Orange;
                    //}


                    //else if (value != "")
                    //{
                    //    row.DefaultCellStyle.BackColor = Color.Green;
                    //}
                    //else
                    //{
                    //    row.DefaultCellStyle.BackColor = Color.IndianRed;
                    //}




                    if (row.Cells[3].Value.ToString() != "")
                    {
                        row.DefaultCellStyle.BackColor = Color.IndianRed;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.Green;
                    }

                }
                if (validation == false)
                {
                    MessageBox.Show("Please check the comments");
                }
                else
                {
                    button2.Enabled = true;
                }
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }


        private bool ValidatePriceFields(DataTable filterData)
        {

            string actionResults = "";
            bool actionResult = true;
            if (filterData.Columns.Count == 3)
            {
                if (filterData.Columns[0].ToString().Trim().ToLower() != "id") { actionResult = false; actionResults += "Id not found or In-valid column Name,"; }
                if (filterData.Columns[1].ToString().Trim().ToLower() != "itemname") { actionResult = false; actionResults += "ItemName not found or In-valid column Name,"; }
                if (filterData.Columns[2].ToString().Trim().ToLower() != "rack") { actionResult = false; actionResults += "Rack not found or In-valid column Name,"; }


                if (actionResults != "")
                {

                    MessageBox.Show(actionResults.ToString().Trim());
                    actionResult = false;
                    button2.Enabled = false;
                }
                else
                {

                }
            }
            else
            {
                MessageBox.Show("Invalid Excel Please select correct excel");
                dataGridView1.DataSource = null;
                actionResult = false;
                button2.Enabled = false;
            }

            return actionResult;

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            DataTable rsDatacheck = (DataTable)(dataGridView1.DataSource);
            var tables = (DataTable)(dataGridView1.DataSource);
            if (tables != null)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to Update the Rack?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        DataTable subData = null;
                        DataTable rsData1 = new DataTable();
                        DataTable rsData = (DataTable)(dataGridView1.DataSource);
                        var table = (DataTable)(dataGridView1.DataSource);
                        var rows = table.Select("Comments <> '" + "" + "'");
                        var dt = rows.Any() ? rows.CopyToDataTable() : table.Clone();
                        int filteredCount = dt.Rows.Count;
                        if (filteredCount <= 0)
                        {
                            rsData.Columns.Remove("ItemName");

                            rsData.Columns.Remove("Comments");


                            string[] str = new string[] { "1", "2", "3" };
                            DataTable tvp = new DataTable();


                            tvp.Columns.Add("id");
                            tvp.Columns.Add("Price");
                            foreach (string item in str)
                            {
                                tvp.Rows.Add(item);
                            }

                            foreach (DataRow r in tvp.Rows)
                            {
                                Console.WriteLine(r["id"].ToString());
                                Console.WriteLine(r["Price"].ToString());
                            }



                            rsData1 = CashEndCloseBAL.UpdateSalesRack(tvp, rsData, Convert.ToInt16(UserId));
                            MessageBox.Show(rsData1.Rows[0]["Result"].ToString());
                            rsData.Columns.Add("Comments");

                            rsData.Columns.Add("ItemName");


                            textBox1.Text = "";
                            this.dataGridView1.DataSource = null;
                        }
                        else
                        {
                            MessageBox.Show("Please Check the Comments");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Select the Excel File");
            }
        }

    }
}
