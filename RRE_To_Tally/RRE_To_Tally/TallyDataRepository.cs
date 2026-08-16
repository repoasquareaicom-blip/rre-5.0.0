using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace RRE_To_Tally;

public sealed class TallyDataRepository
{
    private const string SalesExportSqlTemplate = @"
SELECT
    @DivisionKey AS DivisionKey,
    @DivisionName AS DivisionName,
    @DivisionCompanyName AS DivisionCompanyName,
    S.sino,
    S.Salesid,
    S.Referenceid,
    {2} AS TransactionDate,
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
FROM {0} S
INNER JOIN {1} SD
    ON SD.Salesid = S.Salesid
LEFT JOIN Customers C
    ON C.CustomerID = TRY_CONVERT(int, S.Customerid)
LEFT JOIN ProductMaster PM
    ON PM.id = TRY_CONVERT(int, SD.Productid)
LEFT JOIN UOM U
    ON U.Uomid = TRY_CONVERT(int, PM.UOM)
WHERE {2} >= @FromDate
  AND {2} < DATEADD(DAY, 1, @ToDate)
  AND (@BillNumber = '' OR S.Salesid LIKE '%' + @BillNumber + '%')
  AND (PM.id IS NULL OR (ISNULL(PM.IsArchived, 0) = 0 AND ISNULL(PM.IsDeleted, 'No') NOT IN ('Yes', '1', 'True')))
ORDER BY {2}, S.Salesid, PM.ItemName;";

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
            {
                connection.Open();

                foreach (SalesDivisionConfig division in SalesDivisionConfig.All)
                {
                    using (SqlCommand command = new SqlCommand(BuildSalesExportSql(connection, division), connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add("@DivisionKey", SqlDbType.VarChar, 40).Value = division.Key;
                        command.Parameters.Add("@DivisionName", SqlDbType.VarChar, 80).Value = division.DisplayName;
                        command.Parameters.Add("@DivisionCompanyName", SqlDbType.VarChar, 120).Value = division.CompanyName;
                        command.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate.Date;
                        command.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate.Date;
                        command.Parameters.Add("@BillNumber", SqlDbType.VarChar, 100).Value = (billNumber ?? "").Trim();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rows.Add(new SalesExportRow
                                {
                                    DivisionKey = GetString(reader, "DivisionKey"),
                                    DivisionName = GetString(reader, "DivisionName"),
                                    DivisionCompanyName = GetString(reader, "DivisionCompanyName"),
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
                }
            }

            return rows;
        });
    }

    private static string BuildSalesExportSql(SqlConnection connection, SalesDivisionConfig division)
    {
        return string.Format(SalesExportSqlTemplate, QuoteName(division.HeaderTable), QuoteName(division.DetailTable), GetSalesDateExpression(connection, division.HeaderTable));
    }

    private static string GetSalesDateExpression(SqlConnection connection, string tableName)
    {
        List<string> dateColumns = new List<string>();
        foreach (string column in new[] { "date", "Date", "BillDate", "SalesDate", "InvoiceDate", "TransactionDate", "EnteredOn", "Updatedon" })
        {
            if (ColumnExists(connection, tableName, column))
            {
                dateColumns.Add("S." + QuoteColumnName(column));
            }
        }

        if (dateColumns.Count == 0)
        {
            throw new InvalidOperationException("No usable sales date column found in table " + tableName + ".");
        }

        return "COALESCE(" + string.Join(", ", dateColumns.ToArray()) + ")";
    }

    private static string QuoteColumnName(string columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName) || columnName.Any(ch => !(char.IsLetterOrDigit(ch) || ch == '_')))
        {
            throw new InvalidOperationException("Unsafe column name: " + columnName);
        }

        return "[" + columnName.Replace("]", "]]") + "]";
    }

    private static bool ColumnExists(SqlConnection connection, string tableName, string columnName)
    {
        using (SqlCommand command = new SqlCommand("SELECT 1 FROM sys.columns c INNER JOIN sys.tables t ON t.object_id = c.object_id WHERE t.name = @TableName AND c.name = @ColumnName", connection))
        {
            command.Parameters.Add("@TableName", SqlDbType.NVarChar, 128).Value = tableName;
            command.Parameters.Add("@ColumnName", SqlDbType.NVarChar, 128).Value = columnName;
            object? result = command.ExecuteScalar();
            return result != null && result != DBNull.Value;
        }
    }

    private static string QuoteName(string tableName)
    {
        if (SalesDivisionConfig.All.All(d => !string.Equals(d.HeaderTable, tableName, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(d.DetailTable, tableName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Unsafe table name: " + tableName);
        }

        return "[" + tableName.Replace("]", "]]") + "]";
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
