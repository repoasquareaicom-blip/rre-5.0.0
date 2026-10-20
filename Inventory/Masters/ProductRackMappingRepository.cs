using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Masters
{
    public class ProductRackBulkAllocationResult
    {
        public int SelectedProductCount { get; set; }
        public int RackCount { get; set; }
        public int InsertedCount { get; set; }
        public int SkippedCount { get; set; }
    }

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

        public ProductRackBulkAllocationResult BulkAddMappings(IList<int> productIds, string rackCaptions, int updatedBy)
        {
            if (productIds == null || productIds.Count == 0)
            {
                throw new ArgumentException("Select one or more products.");
            }

            string productIdCsv = BuildProductIdCsv(productIds);

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ProductRackAllocation_BulkAddRacks", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;

                cmd.Parameters.Add("@ProductIds", SqlDbType.NVarChar, -1).Value = productIdCsv;
                cmd.Parameters.Add("@RackCaptions", SqlDbType.NVarChar, -1).Value = rackCaptions;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = updatedBy;

                SqlParameter selectedProductCount = cmd.Parameters.Add("@SelectedProductCount", SqlDbType.Int);
                selectedProductCount.Direction = ParameterDirection.Output;

                SqlParameter rackCount = cmd.Parameters.Add("@RackCount", SqlDbType.Int);
                rackCount.Direction = ParameterDirection.Output;

                SqlParameter insertedCount = cmd.Parameters.Add("@InsertedCount", SqlDbType.Int);
                insertedCount.Direction = ParameterDirection.Output;

                SqlParameter skippedCount = cmd.Parameters.Add("@SkippedCount", SqlDbType.Int);
                skippedCount.Direction = ParameterDirection.Output;

                con.Open();
                cmd.ExecuteNonQuery();

                return new ProductRackBulkAllocationResult
                {
                    SelectedProductCount = OutputInt(selectedProductCount),
                    RackCount = OutputInt(rackCount),
                    InsertedCount = OutputInt(insertedCount),
                    SkippedCount = OutputInt(skippedCount)
                };
            }
        }

        private static string BuildProductIdCsv(IList<int> productIds)
        {
            List<int> distinctProductIds = new List<int>();

            foreach (int productId in productIds)
            {
                if (productId <= 0 || distinctProductIds.Contains(productId))
                {
                    continue;
                }

                distinctProductIds.Add(productId);
            }

            if (distinctProductIds.Count == 0)
            {
                throw new ArgumentException("Selected products are invalid.");
            }

            return string.Join(",", distinctProductIds.ConvertAll(delegate(int productId)
            {
                return productId.ToString();
            }).ToArray());
        }

        private static int OutputInt(SqlParameter parameter)
        {
            if (parameter == null || parameter.Value == null || parameter.Value == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(parameter.Value);
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
