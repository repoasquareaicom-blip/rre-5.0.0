using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Sales
{
    public class PdiRackRepository
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

        public DataTable GetSalesRacksForProduct(int productId)
        {
            return GetEligibleRackAvailability(productId);
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

        public Dictionary<int, Dictionary<int, decimal>> GetExistingRackAllocations(string quotationId)
        {
            Dictionary<int, Dictionary<int, decimal>> result = new Dictionary<int, Dictionary<int, decimal>>();
            if (quotationId == null || quotationId.Trim().Length == 0)
            {
                return result;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
IF OBJECT_ID('dbo.PdiRackAllocation', 'U') IS NOT NULL
BEGIN
    SELECT ProductId, RackId, Quantity
    FROM dbo.PdiRackAllocation
    WHERE QuotationId = @QuotationId
      AND Quantity > 0
    ORDER BY ProductId, RackId;
END", con))
            {
                cmd.Parameters.Add("@QuotationId", SqlDbType.VarChar, 100).Value = quotationId;
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int productId = Convert.ToInt32(reader["ProductId"]);
                        int rackId = Convert.ToInt32(reader["RackId"]);
                        decimal quantity = Convert.ToDecimal(reader["Quantity"]);
                        if (!result.ContainsKey(productId))
                        {
                            result.Add(productId, new Dictionary<int, decimal>());
                        }
                        result[productId][rackId] = quantity;
                    }
                }
            }

            return result;
        }

        public PdiRackSaveResult SaveQuotationPdi(PdiRackSaveRequest request)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.SaveQuotationPdi_RackWise", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.Add("@isnew", SqlDbType.Int).Value = request.IsNew;
                cmd.Parameters.Add("@Quotationid", SqlDbType.VarChar, 100).Value = NullString(request.QuotationId);
                cmd.Parameters.Add("@Customerid", SqlDbType.VarChar, 100).Value = NullString(request.CustomerId);
                cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = request.Date;
                cmd.Parameters.Add("@Referenceid", SqlDbType.VarChar, 100).Value = NullString(request.ReferenceId);
                cmd.Parameters.Add("@Assist", SqlDbType.VarChar, 100).Value = NullString(request.Assist);
                cmd.Parameters.Add("@status", SqlDbType.VarChar, 100).Value = NullString(request.Status);
                cmd.Parameters.Add("@Updatedby", SqlDbType.VarChar, 100).Value = NullString(request.UpdatedBy);
                cmd.Parameters.Add("@AssistName", SqlDbType.VarChar, 100).Value = NullString(request.AssistName);
                cmd.Parameters.Add("@Customername", SqlDbType.VarChar, 250).Value = NullString(request.CustomerName);
                cmd.Parameters.Add("@City", SqlDbType.VarChar, 100).Value = NullString(request.City);

                SqlParameter quotationDetails = cmd.Parameters.Add("@QuotationDetails", SqlDbType.Structured);
                quotationDetails.TypeName = "dbo.QuotationPditype_2";
                quotationDetails.Value = BuildQuotationPdiTvp(request.QuotationDetails);

                SqlParameter rackDetails = cmd.Parameters.Add("@RackDetails", SqlDbType.Structured);
                rackDetails.TypeName = "dbo.PdiRackAllocationType";
                rackDetails.Value = BuildPdiRackAllocationTvp(request.RackDetails);

                SqlParameter output = cmd.Parameters.Add("@output", SqlDbType.Int);
                output.Direction = ParameterDirection.Output;

                SqlParameter result = cmd.Parameters.Add("@result", SqlDbType.VarChar, 100);
                result.Direction = ParameterDirection.Output;

                con.Open();
                cmd.ExecuteNonQuery();

                PdiRackSaveResult saveResult = new PdiRackSaveResult();
                saveResult.OutputCode = output.Value == DBNull.Value ? 0 : Convert.ToInt32(output.Value);
                saveResult.Result = Convert.ToString(result.Value);
                return saveResult;
            }
        }

        private object NullString(string value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            return value;
        }

        private DataTable BuildPdiRackAllocationTvp(DataTable source)
        {
            DataTable tvp = new DataTable();
            tvp.Columns.Add("ProductId", typeof(int));
            tvp.Columns.Add("RackId", typeof(int));
            tvp.Columns.Add("Quantity", typeof(decimal));

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

        private DataTable BuildQuotationPdiTvp(DataTable source)
        {
            DataTable tvp = new DataTable();
            tvp.Columns.Add("Productid", typeof(string));
            tvp.Columns.Add("Rate", typeof(string));
            tvp.Columns.Add("Quantity", typeof(string));
            tvp.Columns.Add("Amount", typeof(string));
            tvp.Columns.Add("PQuantity", typeof(string));
            tvp.Columns.Add("Product", typeof(string));
            tvp.Columns.Add("Status", typeof(string));

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
                row["Productid"] = ToStringValue(GetSourceValue(sourceRow, "ProductId"));
                row["Rate"] = ToStringValue(GetSourceValue(sourceRow, "Rate"));
                row["Quantity"] = ToStringValue(GetSourceValue(sourceRow, "Quantity"));
                row["Amount"] = ToStringValue(GetSourceValue(sourceRow, "Amount"));
                row["PQuantity"] = ToStringValue(GetSourceValue(sourceRow, "PQuantity"));
                row["Product"] = ToStringValue(GetSourceValue(sourceRow, "Product Serial.No"));
                row["Status"] = ToStringValue(GetSourceValue(sourceRow, "Status"));
                tvp.Rows.Add(row);
            }

            return tvp;
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

    public class PdiRackSaveRequest
    {
        public int IsNew;
        public string QuotationId;
        public string CustomerId;
        public DateTime Date;
        public string ReferenceId;
        public string Assist;
        public string Status;
        public string UpdatedBy;
        public string AssistName;
        public string CustomerName;
        public string City;
        public DataTable QuotationDetails;
        public DataTable RackDetails;
    }

    public class PdiRackSaveResult
    {
        public int OutputCode;
        public string Result;
    }
}
