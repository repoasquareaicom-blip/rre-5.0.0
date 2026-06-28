using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Report
{
    public partial class NotInEstimation : Form
    {
        public string cs = System.Configuration.ConfigurationManager.ConnectionStrings["con"].ToString();

        public NotInEstimation()
        {
            InitializeComponent();
        }

        private void NotInEstimation_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            DateTime date = DateTime.Now.Date;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            dateFrom.Value = firstDayOfMonth;
            BindInGrif();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BindInGrif();
        }

        private void BindInGrif()
        {
            DataSet objDs = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "getNotInEsitmationQuotation";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("FromDate", dateFrom.Value);
                    cmd.Parameters.AddWithValue("ToDate", dateTo.Value);

                    conn.Open();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(objDs);
                }

                dataGridView1.DataSource = objDs.Tables[0];

            }
            catch (Exception ex)
            {

            }
        }
    }
}
