using System;
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

        public decimal GetRackBalance(int productId, int rackId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT CAST(ISNULL(SUM(
    CASE
        WHEN UPPER(LTRIM(RTRIM(Type))) = 'IN' THEN ISNULL(Quantity, 0)
        WHEN UPPER(LTRIM(RTRIM(Type))) = 'OUT' THEN -ISNULL(Quantity, 0)
        ELSE 0
    END
), 0) AS DECIMAL(18,3))
FROM dbo.MaterialTranscation
WHERE MaterailId = @ProductId
  AND RackId = @RackId", con))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                cmd.Parameters.Add("@RackId", SqlDbType.Int).Value = rackId;
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return 0m;
                }

                return Convert.ToDecimal(result);
            }
        }

        public void ReplaceMapping(int productId, int oldRackId, int newRackId, int updatedBy)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteCmd = new SqlCommand("sp_ProductRackMapping_Delete", con, transaction))
                        {
                            deleteCmd.CommandType = CommandType.StoredProcedure;
                            deleteCmd.Parameters.AddWithValue("@ProductId", productId);
                            deleteCmd.Parameters.AddWithValue("@RackId", oldRackId);
                            deleteCmd.ExecuteNonQuery();
                        }

                        using (SqlCommand saveCmd = new SqlCommand("sp_ProductRackMapping_Save", con, transaction))
                        {
                            saveCmd.CommandType = CommandType.StoredProcedure;
                            saveCmd.Parameters.AddWithValue("@ProductId", productId);
                            saveCmd.Parameters.AddWithValue("@RackId", newRackId);
                            saveCmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                            saveCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
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
