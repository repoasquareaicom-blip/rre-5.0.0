using System;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Masters
{
    public class ProductRackWiseStockMovementRepository
    {
        private readonly string connectionString = Program.connection;

        public bool GetRackWiseStockMovement(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Product_RackWiseStockMovement_Get", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", productId);
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return false;
                }

                bool boolValue;
                if (bool.TryParse(Convert.ToString(result), out boolValue))
                {
                    return boolValue;
                }

                int intValue;
                return int.TryParse(Convert.ToString(result), out intValue) && intValue != 0;
            }
        }

        public void SaveRackWiseStockMovement(int productId, bool enabled)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Product_RackWiseStockMovement_Save", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@RackWiseStockMovement", enabled);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
