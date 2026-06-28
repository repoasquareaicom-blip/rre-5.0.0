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
using System.Xml.Linq;

namespace Inventory.Report
{
    public partial class PeriodWiseClosingStock : Form
    {
        public string cs = System.Configuration.ConfigurationManager.ConnectionStrings["con"].ToString();

        public PeriodWiseClosingStock()
        {
            InitializeComponent();
        }

        private void PeriodWiseClosingStock_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            processingImage.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {  
            processingImage.Visible = true;
            
          
            DataSet objDs = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Proc_ClosingStocks_ByDates";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("FromDate", dateFrom.Value);
                    cmd.Parameters.AddWithValue("ToDate", dateTo.Value);

                    conn.Open();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(objDs);
                }

                dataGridView1.DataSource = objDs.Tables[0];

                //if (objDs.Tables[0].Rows.Count > 0)
                //{
                //    TotalStockVal.Text = objDs.Tables[1].Rows[0]["TotalStock"].ToString();
                //    StockVal.Text = objDs.Tables[1].Rows[0]["TotalStockValue"].ToString();
                //}

            }
            catch (Exception ex)
            {

            }

            processingImage.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            processingImage.Visible = true;
            DataSet objDs = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Proc_ClosingStocks_ByDates";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("FromDate", dateFrom.Value);
                    cmd.Parameters.AddWithValue("ToDate", dateTo.Value);

                    conn.Open();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(objDs);
                }

                dataGridView1.DataSource = objDs.Tables[0];

                //if (objDs.Tables[0].Rows.Count > 0)
                //{
                //    TotalStockVal.Text = objDs.Tables[1].Rows[0]["TotalStock"].ToString();
                //    StockVal.Text = objDs.Tables[1].Rows[0]["TotalStockValue"].ToString();
                //}


                saveFileDialog1.ShowDialog();
                StringBuilder outExcel = new StringBuilder();
                string OutFileName = saveFileDialog1.FileName;
                OutFileName = OutFileName.Replace(".xls", "");
                OutFileName = OutFileName.Replace(".XLS", "");
                OutFileName = OutFileName + ".xls";
                outExcel.Append("S.No\tProduct Name\tPrice\tOpening Stock\tIN\tOUT\tStock\tStockValue");
                outExcel.Append("\r\n");
                for (int i = 0; i < objDs.Tables[0].Rows.Count; i++)
                {
                    outExcel.Append((i + 1) + "\t");
                    for (int j = 0; j < objDs.Tables[0].Columns.Count; j++)
                    {
                        outExcel.Append(objDs.Tables[0].Rows[i][j].ToString() + "\t");
                    }
                    outExcel.Append("\r\n");
                }

               // outExcel.Append("\r\n\r\n\t\tTotal Stock" + "\t" + objDs.Tables[1].Rows[0]["TotalStock"].ToString() + "\t" + "Total Stock Value" + "\t" + objDs.Tables[1].Rows[0]["TotalStockValue"].ToString());




                Encoding utf16 = Encoding.GetEncoding(1254);
                byte[] output = utf16.GetBytes(outExcel.ToString());
                FileStream fs = new FileStream(OutFileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(output, 0, output.Length); //write the encoded file
                bw.Flush();
                bw.Close();
                fs.Close();

                MessageBox.Show("Report saved in " + OutFileName);
                processingImage.Visible = false;
            }
            catch (Exception ex)
            {

            }
        }

        public static XDocument GenerateXMLSpreadSheet(DataTable tbl)
        {
            XDocument xmlssDoc =
                new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XProcessingInstruction("mso-application", "Excel.Sheet"),
                new XElement("Workbook",
                    new XAttribute("xmlns", "urn:schemas-microsoft-com:office:spreadsheet"),
                    new XAttribute("xmlns:ss", "urn:schemas-microsoft-com:office:spreadsheet"),
                        new XElement("Worksheet", new XAttribute("ss:Name", tbl.TableName),
                            new XElement("Table", GetRows(tbl)
                            )
                        )
                    )
                );
            return xmlssDoc;
        }

        public static Object[] GetRows(DataTable tbl)
        {
            // generate XElement rows for each row in the database.
            // generate from the bottom-up.
            // start with the cells.
            XElement[] rows = new XElement[tbl.Rows.Count];

            int r = 0;
            foreach (DataRow row in tbl.Rows)
            {
                // create the array of cells to add to the row:
                XElement[] cells = new XElement[tbl.Columns.Count];
                int c = 0;
                foreach (DataColumn col in tbl.Columns)
                {
                    cells[c++] =
                        new XElement("Cell",
                            new XElement("Data", new XAttribute("ss:Type", "String"),
                                new XText(row[col].ToString())));
                }
                rows[r++] = new XElement("Row", cells);
            }
            // return the array of rows.
            return rows;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
        }


    }
}
