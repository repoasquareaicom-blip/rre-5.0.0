using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace RRE_To_Tally;

public sealed class TallyDataRepository
{
    public const string SalesExportSql = @"
SELECT
    S.sino,
    S.Salesid,
    S.Referenceid,
    COALESCE(S.EnteredOn, S.Updatedon) AS TransactionDate,
    S.Customerid,
    COALESCE(NULLIF(C.Name, ''), NULLIF(S.customername, ''), 'CASH CUSTOMER') AS CustomerName,
    COALESCE(NULLIF(C.Address1, ''), NULLIF(S.Address1, '')) AS CustomerAddress1,
    COALESCE(NULLIF(C.Address2, ''), NULLIF(S.Address2, '')) AS CustomerAddress2,
    COALESCE(NULLIF(C.City, ''), NULLIF(S.City, '')) AS CustomerCity,
    COALESCE(NULLIF(C.State, ''), NULLIF(S.State, ''), 'Tamil Nadu') AS CustomerState,
    C.District,
    C.Pincode,
    COALESCE(NULLIF(C.Phone, ''), NULLIF(S.Mobile, '')) AS CustomerPhone,
    C.Email,
    COALESCE(NULLIF(C.Tin, ''), NULLIF(S.Tin, '')) AS CustomerGSTIN,
    S.Paymentmode,
    S.TotalAmount,
    S.LessAmount,
    S.GrandTotal,
    S.others AS OtherCharges,
    S.GstText,
    SD.Productid,
    PM.ItemCode,
    COALESCE(NULLIF(PM.DisplayName, ''), NULLIF(PM.ItemName, ''), PM.ItemCode) AS ProductName,
    PM.Category,
    PM.Brand,
    COALESCE(NULLIF(U.UOM, ''), NULLIF(PM.UOM, ''), 'NOS') AS UOM,
    PM.HSN,
    PM.GST AS ProductGST,
    PM.SGST,
    PM.IGST,
    PM.Tax,
    SD.Rate,
    SD.Quantity,
    SD.Amount,
    SD.gst AS SalesDetailGST,
    CASE WHEN PM.id IS NULL THEN 1 ELSE 0 END AS MissingProductMaster
FROM Sales S
INNER JOIN SalesDetails SD
    ON SD.Salesid = S.Salesid
LEFT JOIN Customers C
    ON C.CustomerID = TRY_CONVERT(int, S.Customerid)
LEFT JOIN ProductMaster PM
    ON PM.id = TRY_CONVERT(int, SD.Productid)
LEFT JOIN UOM U
    ON U.Uomid = TRY_CONVERT(int, PM.UOM)
WHERE COALESCE(S.EnteredOn, S.Updatedon) >= @FromDate
  AND COALESCE(S.EnteredOn, S.Updatedon) < DATEADD(DAY, 1, @ToDate)
  AND (@BillNumber = '' OR S.Salesid LIKE '%' + @BillNumber + '%')
  AND (PM.id IS NULL OR (ISNULL(PM.IsArchived, 0) = 0 AND ISNULL(PM.IsDeleted, 'No') NOT IN ('Yes', '1', 'True')))
ORDER BY COALESCE(S.EnteredOn, S.Updatedon), S.Salesid, PM.ItemName;";

    public Task<List<SalesExportRow>> LoadSalesRowsAsync(DateTime fromDate, DateTime toDate, string billNumber)
    {
        return Task.Run(delegate
        {
            ConnectionStringSettings? setting = ConfigurationManager.ConnectionStrings["con"];
            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
            {
                throw new ConfigurationErrorsException("Connection string 'con' was not found.");
            }

            List<SalesExportRow> rows = new List<SalesExportRow>();
            using (SqlConnection connection = new SqlConnection(setting.ConnectionString))
            using (SqlCommand command = new SqlCommand(SalesExportSql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate.Date;
                command.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate.Date;
                command.Parameters.Add("@BillNumber", SqlDbType.VarChar, 100).Value = (billNumber ?? "").Trim();
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(new SalesExportRow
                        {
                            Sino = GetInt(reader, "sino"),
                            SalesId = GetString(reader, "Salesid"),
                            ReferenceId = GetString(reader, "Referenceid"),
                            TransactionDate = GetDate(reader, "TransactionDate"),
                            CustomerId = GetString(reader, "Customerid"),
                            CustomerName = GetString(reader, "CustomerName"),
                            CustomerAddress1 = GetString(reader, "CustomerAddress1"),
                            CustomerAddress2 = GetString(reader, "CustomerAddress2"),
                            CustomerCity = GetString(reader, "CustomerCity"),
                            CustomerState = GetString(reader, "CustomerState"),
                            District = GetString(reader, "District"),
                            Pincode = GetString(reader, "Pincode"),
                            CustomerPhone = GetString(reader, "CustomerPhone"),
                            Email = GetString(reader, "Email"),
                            CustomerGSTIN = GetString(reader, "CustomerGSTIN"),
                            PaymentMode = GetString(reader, "Paymentmode"),
                            TotalAmount = GetString(reader, "TotalAmount"),
                            LessAmount = GetString(reader, "LessAmount"),
                            GrandTotal = GetString(reader, "GrandTotal"),
                            OtherCharges = GetString(reader, "OtherCharges"),
                            GstText = GetString(reader, "GstText"),
                            ProductId = GetString(reader, "Productid"),
                            ItemCode = GetString(reader, "ItemCode"),
                            ProductName = GetString(reader, "ProductName"),
                            Category = GetString(reader, "Category"),
                            Brand = GetString(reader, "Brand"),
                            Uom = GetString(reader, "UOM"),
                            Hsn = GetString(reader, "HSN"),
                            ProductGst = GetString(reader, "ProductGST"),
                            Sgst = GetString(reader, "SGST"),
                            Igst = GetString(reader, "IGST"),
                            Tax = GetString(reader, "Tax"),
                            Rate = GetString(reader, "Rate"),
                            Quantity = GetString(reader, "Quantity"),
                            Amount = GetString(reader, "Amount"),
                            SalesDetailGst = GetDecimal(reader, "SalesDetailGST"),
                            MissingProductMaster = GetInt(reader, "MissingProductMaster") == 1
                        });
                    }
                }
            }

            return rows;
        });
    }

    private static string GetString(SqlDataReader reader, string name)
    {
        object value = reader[name];
        return value == DBNull.Value ? "" : Convert.ToString(value) ?? "";
    }

    private static int GetInt(SqlDataReader reader, string name)
    {
        object value = reader[name];
        return value == DBNull.Value ? 0 : Convert.ToInt32(value);
    }

    private static DateTime GetDate(SqlDataReader reader, string name)
    {
        object value = reader[name];
        return value == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(value);
    }

    private static decimal GetDecimal(SqlDataReader reader, string name)
    {
        object value = reader[name];
        return value == DBNull.Value ? 0m : Convert.ToDecimal(value);
    }
}
