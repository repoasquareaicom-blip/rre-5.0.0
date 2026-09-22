using System;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Sales
{
    public class PendingIssuedRackRepository
    {
        private readonly string connectionString = Program.connection;

        public bool GetRackWiseStockMovement(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Product_RackWiseStockMovement_Get", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
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

        public DataTable GetEligibleRackAvailability(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    prm.ProductId,
    lm.LocationId,
    rm.RackId,
    lm.LocationName,
    rm.RackCaption,
    CAST(ISNULL(stock.AvailableQuantity, 0) AS DECIMAL(18,3)) AS AvailableQuantity,
    CAST(ISNULL(stock.AvailableQuantity, 0) AS DECIMAL(18,3)) AS AvailableQty,
    ISNULL(lm.DisplayOrder, 0) AS LocationDisplayOrder,
    ISNULL(rm.DisplayOrder, 0) AS RackDisplayOrder
FROM dbo.ProductRackMapping prm
INNER JOIN dbo.RackMaster rm
    ON rm.RackId = prm.RackId
INNER JOIN dbo.LocationMaster lm
    ON lm.LocationId = rm.LocationId
OUTER APPLY
(
    SELECT SUM(
        CASE
            WHEN UPPER(ISNULL(mt.Type, '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
            WHEN UPPER(ISNULL(mt.Type, '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
            ELSE 0
        END
    ) AS AvailableQuantity
    FROM dbo.MaterialTranscation mt
    WHERE mt.MaterailId = prm.ProductId
      AND mt.RackId = prm.RackId
) stock
WHERE prm.ProductId = @ProductId
  AND ISNULL(rm.IsActive, 0) = 1
  AND ISNULL(lm.IsActive, 0) = 1
  AND ISNULL(lm.AllowForSales, 0) = 1
  AND ISNULL(stock.AvailableQuantity, 0) > 0
ORDER BY
    ISNULL(lm.DisplayOrder, 0),
    ISNULL(rm.DisplayOrder, 0),
    rm.RackId", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        public DataTable CreateRackDetailsTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("RackId", typeof(int));
            dt.Columns.Add("Quantity", typeof(decimal));
            return dt;
        }

        public string SaveIssuedRackWise(string receiveId, string updatedBy, DataTable issueDetails, DataTable rackDetails)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.SaveIssued_RackWise", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.Add("@receiveid", SqlDbType.VarChar, 50).Value = NullString(receiveId);

                SqlParameter issueParam = cmd.Parameters.Add("@QuotationDetails", SqlDbType.Structured);
                issueParam.TypeName = "dbo.Issuedtype";
                issueParam.Value = BuildIssuedType(issueDetails);

                SqlParameter rackParam = cmd.Parameters.Add("@RackDetails", SqlDbType.Structured);
                rackParam.TypeName = "dbo.PendingIssuedRackDetailType";
                rackParam.Value = BuildRackType(rackDetails);

                cmd.Parameters.Add("@Updatedby", SqlDbType.VarChar, 10).Value = NullString(updatedBy);

                SqlParameter output = cmd.Parameters.Add("@outid", SqlDbType.VarChar, 100);
                output.Direction = ParameterDirection.Output;

                con.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToString(output.Value);
            }
        }

        private DataTable BuildIssuedType(DataTable source)
        {
            DataTable tvp = new DataTable();
            tvp.Columns.Add("Quantity", typeof(string));
            tvp.Columns.Add("Productid", typeof(string));
            tvp.Columns.Add("Location", typeof(string));

            if (source == null)
            {
                return tvp;
            }

            foreach (DataRow sourceRow in source.Rows)
            {
                if (sourceRow.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                DataRow row = tvp.NewRow();
                row["Quantity"] = ToStringValue(GetSourceValue(sourceRow, "Quantity"));
                row["Productid"] = ToStringValue(GetSourceValue(sourceRow, "Productid"));
                row["Location"] = ToStringValue(GetSourceValue(sourceRow, "Location"));
                tvp.Rows.Add(row);
            }

            return tvp;
        }

        private DataTable BuildRackType(DataTable source)
        {
            DataTable tvp = CreateRackDetailsTable();
            if (source == null)
            {
                return tvp;
            }

            foreach (DataRow sourceRow in source.Rows)
            {
                if (sourceRow.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                DataRow row = tvp.NewRow();
                row["ProductId"] = ToInt(GetSourceValue(sourceRow, "ProductId"));
                row["RackId"] = ToInt(GetSourceValue(sourceRow, "RackId"));
                row["Quantity"] = ToDecimal(GetSourceValue(sourceRow, "Quantity"));
                tvp.Rows.Add(row);
            }

            return tvp;
        }

        private object NullString(string value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            return value;
        }

        private object GetSourceValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                return row[columnName];
            }

            return DBNull.Value;
        }

        private int ToInt(object value)
        {
            if (value == null || value == DBNull.Value || Convert.ToString(value).Trim().Length == 0)
            {
                return 0;
            }

            return Convert.ToInt32(value);
        }

        private decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value || Convert.ToString(value).Trim().Length == 0)
            {
                return 0;
            }

            return Convert.ToDecimal(value);
        }

        private object ToStringValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return DBNull.Value;
            }

            return Convert.ToString(value);
        }
    }
}
