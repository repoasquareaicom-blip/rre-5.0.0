using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Masters
{
    public partial class FrmPasswordSetup : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        private DataTable dataTable;
        public FrmPasswordSetup()
        {
            InitializeComponent();
        }
        private void FrmPasswordSetup_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id, Password, Flag FROM LessProoferPwd";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridView1.DataSource = dataTable;

                // Set the DataGridView properties
                dataGridView1.Columns["id"].ReadOnly = true; // Make id column read-only
                dataGridView1.Columns["Flag"].ReadOnly = true; // Make Flag column read-only
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row.RowState == DataRowState.Modified) // Only update modified rows
                    {
                        string query = "UPDATE LessProoferPwd SET Password = @Password WHERE id = @id";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Password", row["Password"]);
                            command.Parameters.AddWithValue("@id", row["id"]);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Records updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh data
                LoadData();
            }
        }

     
    }
}
