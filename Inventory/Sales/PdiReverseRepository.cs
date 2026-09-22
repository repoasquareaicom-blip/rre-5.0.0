using System;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Sales
{
    public class PdiReverseRepository
    {
        private readonly string connectionString = Program.connection;

        public int GetReversiblePdiCount()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.QuotationHeader q
WHERE ISNULL(q.IsPDI, 0) = 1
  AND EXISTS
  (
      SELECT 1
      FROM dbo.MaterialTranscation mt
      WHERE mt.TransId = q.Quotationid
        AND mt.TranscationType = 'PDI'
        AND mt.Type = 'OUT'
  )
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.MaterialTranscation rev
      WHERE rev.TransId = q.Quotationid
        AND rev.TranscationType = 'PDI-REVERSE'
        AND rev.Type = 'IN'
  )
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.QuotationEstimation qe
      WHERE qe.Quotationid = q.Quotationid
        AND ISNULL(qe.IsBilled, 0) = 1
  );", con))
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
        }

        public DataTable GetReversiblePdis()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(GetReversiblePdiSql(@"
    q.Quotationid AS [Quotation No],
    q.[date] AS [Date],
    q.customername AS [Customer],
    ISNULL(r.Name, '') AS [Reference],
    CAST(SUM(ISNULL(mt.Quantity, 0)) AS DECIMAL(18,3)) AS [PDI Qty],
    COUNT(1) AS [PDI Item Count]"), con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        public DataTable GetOriginalPdiOut(string quotationId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    mt.MaterailId AS ProductId,
    ISNULL(pm.ItemName, '') AS ProductName,
    mt.LocationId,
    mt.RackId,
    CAST(SUM(ISNULL(mt.Quantity, 0)) AS DECIMAL(18,3)) AS Quantity,
    CAST(ISNULL(pm.RackWiseStockMovement, 0) AS BIT) AS RackWiseStockMovement
FROM dbo.MaterialTranscation mt
LEFT JOIN dbo.ProductMaster pm
    ON pm.id = mt.MaterailId
WHERE mt.TransId = @QuotationId
  AND mt.TranscationType = 'PDI'
  AND mt.Type = 'OUT'
GROUP BY
    mt.MaterailId,
    ISNULL(pm.ItemName, ''),
    mt.LocationId,
    mt.RackId,
    CAST(ISNULL(pm.RackWiseStockMovement, 0) AS BIT)
ORDER BY ISNULL(pm.ItemName, ''), mt.MaterailId, mt.LocationId, mt.RackId;", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@QuotationId", SqlDbType.VarChar, 100).Value = quotationId;
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        public DataTable GetReturnRacksForProduct(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    prm.ProductId,
    lm.LocationId,
    rm.RackId,
    lm.LocationName,
    rm.RackCaption,
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
    ISNULL(rm.DisplayOrder, 0),
    rm.RackId;", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        public PdiReverseResult ReversePdi(PdiReverseRequest request)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.ReverseQuotationPdi_RackWise", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.Add("@QuotationId", SqlDbType.VarChar, 100).Value = NullString(request.QuotationId);
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.VarChar, 100).Value = NullString(request.UpdatedBy);

                SqlParameter rackDetails = cmd.Parameters.Add("@RackDetails", SqlDbType.Structured);
                rackDetails.TypeName = "dbo.PdiReverseRackDetailType";
                rackDetails.Value = BuildRackDetailsTvp(request.RackDetails);

                SqlParameter output = cmd.Parameters.Add("@output", SqlDbType.Int);
                output.Direction = ParameterDirection.Output;

                SqlParameter result = cmd.Parameters.Add("@result", SqlDbType.VarChar, 200);
                result.Direction = ParameterDirection.Output;

                con.Open();
                cmd.ExecuteNonQuery();

                PdiReverseResult reverseResult = new PdiReverseResult();
                reverseResult.OutputCode = output.Value == DBNull.Value ? 0 : Convert.ToInt32(output.Value);
                reverseResult.Result = Convert.ToString(result.Value);
                return reverseResult;
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

        private string GetReversiblePdiSql(string selectExpression)
        {
            return @"
SELECT " + selectExpression + @"
FROM dbo.QuotationHeader q
INNER JOIN dbo.MaterialTranscation mt
    ON mt.TransId = q.Quotationid
   AND mt.TranscationType = 'PDI'
   AND mt.Type = 'OUT'
LEFT JOIN dbo.[References] r
    ON r.ReferencesID = q.Referenceid
WHERE ISNULL(q.IsPDI, 0) = 1
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.MaterialTranscation rev
      WHERE rev.TransId = q.Quotationid
        AND rev.TranscationType = 'PDI-REVERSE'
        AND rev.Type = 'IN'
  )
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.QuotationEstimation qe
      WHERE qe.Quotationid = q.Quotationid
        AND ISNULL(qe.IsBilled, 0) = 1
  )
GROUP BY
    q.Quotationid,
    q.[date],
    q.customername,
    ISNULL(r.Name, '')
ORDER BY q.[date] DESC, q.Quotationid DESC;";
        }

        private DataTable BuildRackDetailsTvp(DataTable source)
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
                row["ProductId"] = ToInt(sourceRow["ProductId"]);
                row["RackId"] = ToInt(sourceRow["RackId"]);
                row["Quantity"] = ToDecimal(sourceRow["Quantity"]);
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
    }

    public class PdiReverseRequest
    {
        public string QuotationId;
        public string UpdatedBy;
        public DataTable RackDetails;
    }

    public class PdiReverseResult
    {
        public int OutputCode;
        public string Result;
    }
}
