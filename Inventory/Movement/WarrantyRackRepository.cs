using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Inventory.Movement
{
    public class WarrantyRackRepository
    {
        private readonly string connectionString = Program.connection;

        public DataTable GetActiveRacksForProduct(int productId)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("RackId", typeof(int));
            table.Columns.Add("RackCaption", typeof(string));
            table.Columns.Add("LocationId", typeof(int));
            table.Columns.Add("LocationName", typeof(string));

            if (productId <= 0)
            {
                return table;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT DISTINCT
    prm.ProductId,
    rm.RackId,
    rm.RackCaption,
    lm.LocationId,
    lm.LocationName,
    ISNULL(lm.DisplayOrder, 0) AS LocationDisplayOrder,
    ISNULL(rm.DisplayOrder, 0) AS RackDisplayOrder
FROM dbo.ProductRackMapping prm
INNER JOIN dbo.RackMaster rm
    ON rm.RackId = prm.RackId
INNER JOIN dbo.LocationMaster lm
    ON lm.LocationId = rm.LocationId
WHERE prm.ProductId = @ProductId
  AND ISNULL(rm.IsActive, 0) = 1
  AND ISNULL(lm.IsActive, 0) = 1
ORDER BY
    ISNULL(lm.DisplayOrder, 0),
    lm.LocationName,
    ISNULL(rm.DisplayOrder, 0),
    rm.RackCaption,
    rm.RackId", con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                adapter.Fill(table);
                return table;
            }
        }

        public DataTable CreateRackDetailsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("TransId", typeof(string));
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("RackId", typeof(int));
            table.Columns.Add("Quantity", typeof(decimal));
            return table;
        }

        public void SaveWarrantyRackStock(DataTable rackDetails, string updatedBy)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.SaveWarrantyReplacement_RackWiseStock", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;

                SqlParameter detailParam = cmd.Parameters.Add("@RackDetails", SqlDbType.Structured);
                detailParam.TypeName = "dbo.WarrantyReplacementRackDetailType";
                detailParam.Value = rackDetails;

                cmd.Parameters.Add("@UpdatedBy", SqlDbType.VarChar, 50).Value = updatedBy == null ? "" : updatedBy;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public decimal AllocationTotal(Dictionary<int, decimal> allocations)
        {
            decimal total = 0;
            if (allocations == null)
            {
                return total;
            }

            foreach (KeyValuePair<int, decimal> allocation in allocations)
            {
                total += allocation.Value;
            }

            return total;
        }
    }
}
