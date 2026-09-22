using System.Data;
using System.Data.SqlClient;

namespace Inventory.Masters
{
    public class ProductRackMappingRepository
    {
        private readonly string connectionString = Program.connection;

        public DataTable GetMappings(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ProductRackMapping_ListByProduct", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", productId);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        public void AddMapping(int productId, int rackId, int updatedBy)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ProductRackMapping_Save", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@RackId", rackId);
                cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteMapping(int productId, int rackId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ProductRackMapping_Delete", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@RackId", rackId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteAllMappings(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ProductRackMapping_DeleteByProduct", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", productId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
