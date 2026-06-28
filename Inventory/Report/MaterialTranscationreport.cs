using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Report
{
    public partial class MaterialTranscationreport : Form
    {
        StockReportBAL objStockReportBAL = new StockReportBAL();
        public MaterialTranscationreport()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.ActiveControl = txtproductname;
            lights.Checked = true;
            electiclas.Checked = true;
            search();
        }

        private void btnsearch_Click(object sender, EventArgs e)
        {
                search();
        }
        string Status = "";

        private void LoadPorts()
        {
            dgvStockrpt.Rows.Clear();
            dgvStockrpt.ColumnCount = 6;
            //dgvOrder.RowCount = 16;

            dgvStockrpt.Columns[0].Name = "Trans Id";
            dgvStockrpt.Columns[1].Name = "Product";
            dgvStockrpt.Columns[2].Name = "Trans Type";
            dgvStockrpt.Columns[3].Name = "Product Type";
            dgvStockrpt.Columns[4].Name = "Trans Date";
            dgvStockrpt.Columns[5].Name = "Qty";
           


            //this.dgvStockrpt.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvStockrpt.Columns[0].Width = 25;
            //this.dgvStockrpt.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvStockrpt.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvStockrpt.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvStockrpt.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvStockrpt.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvStockrpt.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;

            //// dgvOrder.Columns[5].HeaderText = "GST";

            //this.dgvStockrpt.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvStockrpt.Columns[5].Width = 30;
            ////this.dgvOrder.Columns[4].ReadOnly = true;

            //this.dgvStockrpt.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvStockrpt.Columns[2].Width = 35;

            //this.dgvStockrpt.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvStockrpt.Columns[3].Width = 30;
            //this.dgvStockrpt.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvStockrpt.Columns[6].Width = 30;

            //this.dgvStockrpt.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvStockrpt.Columns[7].Width = 35;



            //this.dgvStockrpt.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgvStockrpt.Columns[8].Width = 90;

          



            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);

            foreach (DataGridViewColumn c in dgvStockrpt.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvStockrpt.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvStockrpt.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }

        public void search()
        {
            string datestatus=string.Empty;

            if (lights.Checked == true && electiclas.Checked == true)
            {
                Status = "All";
            }
            else if (lights.Checked == false && electiclas.Checked == true)
            {
                Status = "No";
            }
            else if (lights.Checked == true && electiclas.Checked == false)
            {
                Status = "Yes";
            }
            else if (lights.Checked == false && electiclas.Checked == false)
            {
                Status = "All";
            }

            DateTime Fromdate = new DateTime(Frommtdate.Value.Year, Frommtdate.Value.Month, Frommtdate.Value.Day);
            DateTime Todate = new DateTime(Tomtdate.Value.Year, Tomtdate.Value.Month, Tomtdate.Value.Day);
            DataTable dt = objStockReportBAL.GetMaterialTranscationreport(txtproductname.Text, Fromdate, Todate, Status);



            LoadPorts();


            if (dt.Rows.Count > 0)
            {
                dgvStockrpt.Rows.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string Val;
                    dgvStockrpt.Rows.Add();
                    //dgvStockrpt.Rows[i].Cells[0].Value = i + 1;
                    dgvStockrpt.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["TransId"]);
                    dgvStockrpt.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["Product"]);
                    dgvStockrpt.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["TransType"]);
                    if(Convert.ToString(dt.Rows[i]["ProductType"])=="Yes")
                    {
                        Val = "Light Product";
                    }
                    else
                    {
                        Val = "Eletrical Product";
                    }
                    dgvStockrpt.Rows[i].Cells[3].Value = Val;                   
                    dgvStockrpt.Rows[i].Cells[4].Value = Convert.ToString(dt.Rows[i]["TransDate"]);
                    dgvStockrpt.Rows[i].Cells[5].Value = Convert.ToString(dt.Rows[i]["Qty"]);
                }

            }
            else
            {
                dgvStockrpt.Rows.Clear();

            }




          //  Convert.ToDateTime(dt.Columns["Date"].ToString());
            //dt.Columns[5].DateTimeMode;// DateTime);
            //dgvStockrpt.DataSource = dt;
            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);
            



            foreach (DataGridViewColumn c in dgvStockrpt.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvStockrpt.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvStockrpt.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvStockrpt.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            //dgvStockrpt.Columns[0].Visible = false;
            //dgvStockrpt.Columns[1].Width = 100;
            //dgvStockrpt.Columns[2].Width = 400;
            //dgvStockrpt.Columns[3].Width = 100;
            //dgvStockrpt.Columns[4].Width = 80;
            
            //dgvStockrpt.Columns[5].Width = 60;
            
            ////dgvStockrpt.Columns[5].ValueType = typeof();
            //dgvStockrpt.Columns[6].Width = 60;
            //dgvStockrpt.Columns[7].Width = 50;
            //dgvStockrpt.Columns[8].Visible = false;
            //dgvStockrpt.Columns[9].Visible = false; 
            //dgvStockrpt.Columns[10].Visible = false;

            //if (dgvStockrpt.Rows.Count>0)
            //{
            //    lblin.Text = Convert.ToString(dgvStockrpt.Rows[0].Cells[9].Value);
            //    lblout.Text = Convert.ToString(dgvStockrpt.Rows[0].Cells[10].Value);
            //    lblstock.Text = Convert.ToString(Convert.ToDouble(lblin.Text) - Convert.ToDouble(lblout.Text));
            //}
            //else
            //{
            //    lblin.Text = "0";
            //    lblout.Text = "0";
            //    lblstock.Text = "0";
            //}
           
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Dispose();
            }
            if(keyData==Keys.Tab)
            {
               
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        public AutoCompleteStringCollection AutoCompleteLoad()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objStockReportBAL.getprobymaterial();
            
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
            //for (int i = 0; i < arr.Length; i++)
            //{
            //  var combined = string.Join(", ", arr);
            //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //   str.Add(combined);
            //}

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}

            return str;
        }

        private void MaterialTranscationreport_Load(object sender, EventArgs e)
        {
            //search();
            //txtproductname.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //txtproductname.AutoCompleteCustomSource = AutoCompleteLoad();
            //txtproductname.AutoCompleteSource = AutoCompleteSource.CustomSource;

        }
    }
}
