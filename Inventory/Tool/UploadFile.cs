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
    public partial class UploadFile : Form
    {
        string UserId = Program.userid;
        string validationError = "";
        System.Data.DataTable monthData = new System.Data.DataTable();
        public UploadFile()
        {
            InitializeComponent();
        }
        private void UploadFile_Load(object sender, EventArgs e)
        {

            textBox1.Enabled = false;
            label4.Text = "0";
        }
        private void button1_Click(object sender, EventArgs e)
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
        public bool checkItemExist(string ProductName, string Id)
        {

            DataTable GetData = CashEndCloseBAL.CheckStockUpload(ProductName, Id);
            if (GetData.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //public bool checkItemNameExist(DataTable CheckData)
        //{


        //    GetsData.Rows[0]["Result"].ToString();
        //}

        private void ProcessUploadData(DataTable sheetData)
        {
            DataTable filterData = sheetData;
            //Validate Fields
            if (ValidateUploadFields(filterData) == true)
            {
                string SortByVal = string.Empty;
                SortByVal = "Stock";
                string SortOrderVal = "asc";
                filterData.DefaultView.Sort = SortByVal + " " + SortOrderVal;
                filterData.Columns.Add("Comments");
                bool validation = true;
                foreach (DataRow r in filterData.Rows)
                {
                    string Check = "";

                    if (r["Stock"].ToString() != "")
                    {



                        double stockValue = Convert.ToDouble(r["Stock"].ToString());
                        string ProductName = r["ItemName"].ToString();
                        string Id = r["Id"].ToString();

                        if (stockValue < 0)
                        {
                            Check += "Negative stock not allowed,";
                            r["Comments"] = Check;
                            validation = false;

                            button2.Enabled = false;
                        }



                        if (checkItemExist(ProductName, Id) == false)
                        {
                            Check += "Item (or) Id  Invalid,";
                            r["Comments"] = Check;
                            button2.Enabled = false;
                            validation = false;
                        }



                        DataTable GetsData = CashEndCloseBAL.CheckStockItemNameUpload(ProductName);
                        if (GetsData.Rows.Count > 0)
                        {
                            string val = "";
                            val = GetsData.Rows[0]["Result"].ToString();
                            if (val != "")
                            {
                                Check += val;
                                r["Comments"] = Check;
                                button2.Enabled = false;
                                validation = false;
                            }
                            else
                            {
                                // button2.Enabled = true;
                            }

                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        string ProductName = r["ItemName"].ToString();
                        string Id = r["Id"].ToString();
                        string test = r["Stock"].ToString();
                        if (test != "")
                        {
                            double stockValue = Convert.ToDouble(r["Stock"].ToString());
                            if (stockValue < 0)
                            {
                                Check += "Negative stock not allowed,";
                                r["Comments"] = Check;
                                validation = false;

                                button2.Enabled = false;
                            }

                        }





                        if (checkItemExist(ProductName, Id) == false)
                        {
                            Check += "Item (or) Id  Invalid,";
                            r["Comments"] = Check;
                            button2.Enabled = false;
                            validation = false;
                        }





                        Check += "Stock Not empty,";
                        r["Comments"] = Check;
                        button2.Enabled = false;
                        validation = false;

                        DataTable GetsData = CashEndCloseBAL.CheckStockItemNameUpload(ProductName);
                        if (GetsData.Rows.Count > 0)
                        {
                            string val = "";
                            val = GetsData.Rows[0]["Result"].ToString();
                            if (val != "")
                            {
                                Check += val;
                                r["Comments"] = Check;
                                button2.Enabled = false;
                                validation = false;
                            }
                            else
                            {
                                // button2.Enabled = true;
                            }

                        }
                        else
                        {

                        }
                    }






                }

                dataGridView1.DataSource = filterData;
                dataGridView1.Columns["Id"].Width = 50;
                dataGridView1.Columns["ItemName"].Width = 300;
                dataGridView1.Columns["Rack"].Width = 300;
                dataGridView1.Columns["Comments"].Width = 450;

                dataGridView1.Columns["Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Rack"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Comments"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["UOM"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Stock"].SortMode = DataGridViewColumnSortMode.NotSortable;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    //string value = Convert.ToString(row.Cells[5].Value);

                    if (row.Cells[4].Value.ToString() == "")
                    {
                        //row.DefaultCellStyle.BackColor = Color.IndianRed;
                    }
                    else
                    {
                        if (Convert.ToDouble(row.Cells[4].Value) < 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.IndianRed;
                        }

                    }

                    if (row.Cells[5].Value.ToString() != "")
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
                DataTable subData = new DataTable();
                decimal StockValue = 0;
                DataTable dt = filterData;
                subData = dt.Select("Stock = '" + StockValue + "'").CopyToDataTable();
                int countTbl = subData.Rows.Count;
                label4.Text = countTbl.ToString();
                //button2.Enabled = false;
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }


        private bool ValidateUploadFields(DataTable filterData)
        {
            string actionResults = "";
            bool actionResult = true;

            if (filterData.Columns[0].ToString().Trim().ToLower() != "id") { actionResult = false; actionResults += "Id not found or In-valid column Name,"; }
            if (filterData.Columns[1].ToString().Trim().ToLower() != "itemname") { actionResult = false; actionResults += "ItemName not found or In-valid column Name,"; }
            if (filterData.Columns[2].ToString().Trim().ToLower() != "rack") { actionResult = false; actionResults += "Rack not found or In-valid column Name,"; }
            if (filterData.Columns[3].ToString().Trim().ToLower() != "uom") { actionResult = false; actionResults += "UOM not found or In-valid column Name,"; }
            if (filterData.Columns[4].ToString().Trim().ToLower() != "stock") { actionResult = false; actionResults += "Stock not found or In-valid column Name"; }

            if (actionResults != "")
            {
                MessageBox.Show(actionResults.ToString().Trim());
            }
            else
            {

            }
            return actionResult;
        }
        private void BindData(string strConn)
        {
            try
            {

                string sheetName = openFileDialog1.FileName;
                System.Data.DataTable dataSheet = getSheetData(strConn, sheetName);

                ProcessUploadData(dataSheet);


            }
            catch (Exception ex)
            {
                //button2.Enabled = false;
                //dataGridView1.DataSource = null;
                //MessageBox.Show("Please check the Excel");
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
        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                DataTable rsDatacheck = (DataTable)(dataGridView1.DataSource);
                var tables = (DataTable)(dataGridView1.DataSource);
                if (tables != null)
                {
                    DialogResult result = MessageBox.Show("Update Opening stock, are you sure?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                                rsData.Columns.Add("ModifiedStock");
                                rsData.Columns.Remove("Comments");
                                rsData1 = CashEndCloseBAL.StockInsertFilse(rsData, Convert.ToInt16(UserId));
                                MessageBox.Show(rsData1.Rows[0]["Result"].ToString());
                                rsData.Columns.Add("Comments");
                                rsData.Columns.Remove("ModifiedStock");
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataTable ExcelFile = CashEndCloseBAL.ExceluploadFile();

            if (ExcelFile != null)
            {
                genExcel(ExcelFile);
            }
            else
            {
                MessageBox.Show("ProductMaster Should not Empty");
            }

        }
        protected void genExcel(DataTable Dt)
        {

            StreamWriter wr = new StreamWriter(@"D:\\ProductMasterTemplate_Upload.xls");

            try
            {

                for (int i = 0; i < Dt.Columns.Count; i++)
                {
                    wr.Write(Dt.Columns[i].ToString().ToUpper() + "\t");
                }

                wr.WriteLine();

                //write rows to excel file
                for (int i = 0; i < (Dt.Rows.Count); i++)
                {
                    for (int j = 0; j < Dt.Columns.Count; j++)
                    {
                        if (Dt.Rows[i][j] != null)
                        {
                            wr.Write(Convert.ToString(Dt.Rows[i][j]) + "\t");
                        }
                        else
                        {
                            wr.Write("\t");
                        }
                    }
                    //go to next line
                    wr.WriteLine();
                }
                //close file
                wr.Close();
                MessageBox.Show("Excel Created Succesfully");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}

