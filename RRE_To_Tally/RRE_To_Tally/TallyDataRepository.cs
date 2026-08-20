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
    C.Name AS MasterCustomerName,
    C.Address1 AS MasterAddress1,
    C.Address2 AS MasterAddress2,
    C.City AS MasterCity,
    C.District AS MasterDistrict,
    C.State AS MasterState,
    {3} AS MasterStateResolved,
    C.Pincode AS MasterPincode,
    C.ContactName AS MasterContactName,
    C.Phone AS MasterPhone,
    C.Email AS MasterEmail,
    C.Tin AS MasterGSTIN,
    S.customername AS SalesCustomerName,
    S.Address1 AS SalesAddress1,
    S.Address2 AS SalesAddress2,
    S.City AS SalesCity,
    S.State AS SalesState,
    {4} AS SalesStateResolved,
    S.Mobile AS SalesMobile,
    S.Tin AS SalesGSTIN,
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
    PM.vat AS ProductVAT,
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
{5}
WHERE {2} >= @FromDate
  AND {2} < DATEADD(DAY, 1, @ToDate)
  AND (@BillNumber = '' OR S.Salesid LIKE '%' + @BillNumber + '%')
  AND (PM.id IS NULL OR (ISNULL(PM.IsArchived, 0) = 0 AND ISNULL(PM.IsDeleted, 'No') NOT IN ('Yes', '1', 'True')))
ORDER BY {2}, S.Salesid, PM.ItemName;";

    public Task<List<SalesDivisionConfig>> LoadCompanyConfigsAsync()
    {
        return Task.Run(delegate
        {
            ConnectionStringSettings? setting = ConfigurationManager.ConnectionStrings["con"];
            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
            {
                throw new ConfigurationErrorsException("Connection string 'con' was not found.");
            }

            List<SalesDivisionConfig> configs = new List<SalesDivisionConfig>();
            HashSet<string> added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (SqlConnection connection = new SqlConnection(setting.ConnectionString))
            {
                connection.Open();
                if (!ColumnExists(connection, "ReportAddressDetails", "CompanyName"))
                {
                    throw new InvalidOperationException("ReportAddressDetails.CompanyName was not found.");
                }

                using (SqlCommand command = new SqlCommand("SELECT DISTINCT CompanyName FROM ReportAddressDetails WHERE LTRIM(RTRIM(ISNULL(CompanyName, ''))) <> '' ORDER BY CompanyName", connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string companyName = GetString(reader, "CompanyName");
                        SalesDivisionConfig? config = SalesDivisionConfig.FindByCompanyName(companyName);
                        if (config == null || !added.Add(config.Key)) continue;
                        configs.Add(config);
                    }
                }
            }

            return configs;
        });
    }

    public Task<List<SalesExportRow>> LoadSalesRowsAsync(DateTime fromDate, DateTime toDate, string billNumber, SalesDivisionConfig division)
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
                                MasterCustomerName = GetString(reader, "MasterCustomerName"),
                                MasterAddress1 = GetString(reader, "MasterAddress1"),
                                MasterAddress2 = GetString(reader, "MasterAddress2"),
                                MasterCity = GetString(reader, "MasterCity"),
                                MasterDistrict = GetString(reader, "MasterDistrict"),
                                MasterState = GetString(reader, "MasterState"),
                                MasterStateResolved = GetString(reader, "MasterStateResolved"),
                                MasterPincode = GetString(reader, "MasterPincode"),
                                MasterContactName = GetString(reader, "MasterContactName"),
                                MasterPhone = GetString(reader, "MasterPhone"),
                                MasterEmail = GetString(reader, "MasterEmail"),
                                MasterGSTIN = GetString(reader, "MasterGSTIN"),
                                SalesCustomerName = GetString(reader, "SalesCustomerName"),
                                SalesAddress1 = GetString(reader, "SalesAddress1"),
                                SalesAddress2 = GetString(reader, "SalesAddress2"),
                                SalesCity = GetString(reader, "SalesCity"),
                                SalesState = GetString(reader, "SalesState"),
                                SalesStateResolved = GetString(reader, "SalesStateResolved"),
                                SalesMobile = GetString(reader, "SalesMobile"),
                                SalesGSTIN = GetString(reader, "SalesGSTIN"),
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
                                ProductVat = GetString(reader, "ProductVAT"),
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

            return rows;
        });
    }

    private static string BuildSalesExportSql(SqlConnection connection, SalesDivisionConfig division)
    {
        StateLookupDefinition? stateLookup = FindStateLookup(connection);
        string masterStateExpression = "CONVERT(nvarchar(100), C.State)";
        string salesStateExpression = "CONVERT(nvarchar(100), S.State)";
        string stateJoins = "";
        if (stateLookup != null)
        {
            masterStateExpression = "COALESCE(NULLIF(CONVERT(nvarchar(100), MS." + stateLookup.NameColumn + "), ''), CONVERT(nvarchar(100), C.State))";
            salesStateExpression = "COALESCE(NULLIF(CONVERT(nvarchar(100), SS." + stateLookup.NameColumn + "), ''), CONVERT(nvarchar(100), S.State))";
            stateJoins =
                "LEFT JOIN " + stateLookup.TableName + " MS" + Environment.NewLine +
                "    ON MS." + stateLookup.IdColumn + " = TRY_CONVERT(int, C.State)" + Environment.NewLine +
                "LEFT JOIN " + stateLookup.TableName + " SS" + Environment.NewLine +
                "    ON SS." + stateLookup.IdColumn + " = TRY_CONVERT(int, S.State)";
        }

        return string.Format(SalesExportSqlTemplate, QuoteName(division.HeaderTable), QuoteName(division.DetailTable), GetSalesDateExpression(connection, division.HeaderTable), masterStateExpression, salesStateExpression, stateJoins);
    }

    private sealed class StateLookupDefinition
    {
        public string TableName { get; set; } = "";
        public string IdColumn { get; set; } = "";
        public string NameColumn { get; set; } = "";
    }

    private static StateLookupDefinition? FindStateLookup(SqlConnection connection)
    {
        string sql = @"
SELECT TOP 1
    QUOTENAME(SCHEMA_NAME(t.schema_id)) + '.' + QUOTENAME(t.name) AS TableName,
    QUOTENAME(idc.name) AS IdColumn,
    QUOTENAME(namec.name) AS NameColumn
FROM sys.tables t
INNER JOIN sys.columns idc
    ON idc.object_id = t.object_id
    AND idc.name IN ('StateId', 'StateID', 'StateCode')
INNER JOIN sys.columns namec
    ON namec.object_id = t.object_id
    AND namec.name IN ('State', 'StateName', 'Name')
WHERE t.name IN ('State', 'States', 'StateMaster', 'StateMasters', 'MstState', 'MasterState')
ORDER BY
    CASE t.name
        WHEN 'State' THEN 0
        WHEN 'States' THEN 1
        WHEN 'StateMaster' THEN 2
        ELSE 3
    END,
    CASE namec.name WHEN 'State' THEN 0 WHEN 'StateName' THEN 1 ELSE 2 END;";

        using (SqlCommand command = new SqlCommand(sql, connection))
        using (SqlDataReader reader = command.ExecuteReader())
        {
            if (!reader.Read()) return null;
            return new StateLookupDefinition
            {
                TableName = GetString(reader, "TableName"),
                IdColumn = GetString(reader, "IdColumn"),
                NameColumn = GetString(reader, "NameColumn")
            };
        }
    }

    private static string GetSalesDateExpression(SqlConnection connection, string tableName)
    {
        foreach (string column in new[] { "UpdatedOn", "Updatedon" })
        {
            if (ColumnExists(connection, tableName, column))
            {
                return "S." + QuoteColumnName(column);
            }
        }

        throw new InvalidOperationException("UpdatedOn column was not found in table " + tableName + ".");
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
