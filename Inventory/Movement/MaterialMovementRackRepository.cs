using System;
using System.Data;
using System.Data.SqlClient;

namespace Inventory
{
    public class MaterialMovementRackRepository
    {
        private readonly string connectionString = Program.connection;

        public ProductMovementInfo GetProductForMovement(string displayName)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1
    prod.id AS ProductId,
    prod.DisplayName AS ProductName,
    prod.ItemCode,
    prod.SalesPrice AS Price,
    CAST(ISNULL(prod.RackWiseStockMovement, 0) AS BIT) AS RackWiseStockMovement,
    CAST(ISNULL(stock.TotalStock, 0) AS DECIMAL(18,3)) AS TotalStock
FROM dbo.ProductMaster prod
OUTER APPLY
(
    SELECT SUM(CASE
        WHEN UPPER(ISNULL(mt.Type, '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
        WHEN UPPER(ISNULL(mt.Type, '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
        ELSE 0
    END) AS TotalStock
    FROM dbo.MaterialTranscation mt
    WHERE mt.MaterailId = prod.id
) stock
WHERE prod.DisplayName = @DisplayName
  AND ISNULL(prod.IsDeleted, '0') <> '1'", con))
            {
                cmd.Parameters.Add("@DisplayName", SqlDbType.VarChar).Value = displayName;
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    ProductMovementInfo info = new ProductMovementInfo();
                    info.ProductId = Convert.ToInt32(reader["ProductId"]);
                    info.ProductName = Convert.ToString(reader["ProductName"]);
                    info.ItemCode = Convert.ToString(reader["ItemCode"]);
                    info.Price = ToDecimal(reader["Price"]);
                    info.RackWiseStockMovement = Convert.ToBoolean(reader["RackWiseStockMovement"]);
                    info.TotalStock = ToDecimal(reader["TotalStock"]);
                    return info;
                }
            }
        }

        public DataTable GetRackAvailability(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    prm.ProductId,
    lm.LocationId,
    lm.LocationName,
    rm.RackId,
    rm.RackCaption,
    ISNULL(lm.DisplayOrder, 0) AS LocationDisplayOrder,
    ISNULL(rm.DisplayOrder, 0) AS RackDisplayOrder,
    CAST(ISNULL(stock.AvailableQuantity, 0) AS DECIMAL(18,3)) AS AvailableQuantity
FROM dbo.ProductRackMapping prm
INNER JOIN dbo.RackMaster rm
    ON rm.RackId = prm.RackId
INNER JOIN dbo.LocationMaster lm
    ON lm.LocationId = rm.LocationId
OUTER APPLY
(
    SELECT SUM(CASE
        WHEN UPPER(ISNULL(mt.Type, '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
        WHEN UPPER(ISNULL(mt.Type, '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
        ELSE 0
    END) AS AvailableQuantity
    FROM dbo.MaterialTranscation mt
    WHERE mt.MaterailId = prm.ProductId
      AND mt.RackId = prm.RackId
) stock
WHERE prm.ProductId = @ProductId
  AND ISNULL(rm.IsActive, 0) = 1
  AND ISNULL(lm.IsActive, 0) = 1
ORDER BY
    ISNULL(lm.DisplayOrder, 0),
    ISNULL(rm.DisplayOrder, 0),
    rm.RackId", con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public DataTable CreateSimpleRackDetailTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("LineNo", typeof(int));
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("FromRackId", typeof(int));
            table.Columns.Add("ToRackId", typeof(int));
            table.Columns.Add("Quantity", typeof(decimal));
            return table;
        }

        public decimal GetCurrentRackStock(int productId, int rackId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT CAST(ISNULL(SUM(CASE
    WHEN UPPER(ISNULL(mt.Type, '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
    WHEN UPPER(ISNULL(mt.Type, '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
    ELSE 0
END), 0) AS DECIMAL(18,3)) AS AvailableQuantity
FROM dbo.MaterialTranscation mt
WHERE mt.MaterailId = @ProductId
  AND mt.RackId = @RackId", con))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                cmd.Parameters.Add("@RackId", SqlDbType.Int).Value = rackId;
                con.Open();
                return ToDecimal(cmd.ExecuteScalar());
            }
        }

        public decimal GetCurrentProductStock(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT CAST(ISNULL(SUM(CASE
    WHEN UPPER(ISNULL(mt.Type, '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
    WHEN UPPER(ISNULL(mt.Type, '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
    ELSE 0
END), 0) AS DECIMAL(18,3)) AS AvailableQuantity
FROM dbo.MaterialTranscation mt
WHERE mt.MaterailId = @ProductId", con))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                con.Open();
                return ToDecimal(cmd.ExecuteScalar());
            }
        }

        public string SaveRackWiseMovement(string movedBy, string estimationId, DataTable rackDetails)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.SaveMaterialMovement_RackWise", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.Add("@MoveBy", SqlDbType.VarChar, 20).Value = NullIfEmpty(movedBy);
                cmd.Parameters.Add("@Estid", SqlDbType.VarChar, 100).Value = NullIfEmpty(estimationId);

                SqlParameter detailParam = cmd.Parameters.Add("@RackDetails", SqlDbType.Structured);
                detailParam.TypeName = "dbo.MaterialMovementSimpleRackDetailType";
                detailParam.Value = rackDetails;

                SqlParameter messageParam = cmd.Parameters.Add("@Message", SqlDbType.VarChar, 500);
                messageParam.Direction = ParameterDirection.Output;

                con.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToString(messageParam.Value);
            }
        }

        private object NullIfEmpty(string value)
        {
            if (value == null || value.Trim().Length == 0)
            {
                return DBNull.Value;
            }

            return value;
        }

        private decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value || Convert.ToString(value).Trim().Length == 0)
            {
                return 0;
            }

            return Convert.ToDecimal(value);
        }
    }

    public class ProductMovementInfo
    {
        public int ProductId;
        public string ProductName;
        public string ItemCode;
        public decimal Price;
        public bool RackWiseStockMovement;
        public decimal TotalStock;
    }
}
