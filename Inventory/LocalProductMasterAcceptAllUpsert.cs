using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inventory
{
    public static class LocalProductMasterAcceptAllUpsert
    {
        public static void Apply(Dictionary<string, object> payload)
        {
            if (payload == null || !payload.ContainsKey("id") || payload["id"] == null)
            {
                throw new InvalidOperationException("ProductMaster id is missing.");
            }

            using (SqlConnection conn = new SqlConnection(Program.connection))
            {
                conn.Open();

                List<string> columns = GetTableColumns(conn);
                if (columns.Count == 0)
                {
                    throw new InvalidOperationException("ProductMaster table was not found.");
                }

                Dictionary<string, string> columnByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (string column in columns)
                {
                    columnByName[column] = column;
                }

                Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (KeyValuePair<string, object> item in payload)
                {
                    string actualColumn;
                    if (columnByName.TryGetValue(item.Key, out actualColumn))
                    {
                        data[actualColumn] = item.Value;
                    }
                }

                if (!data.ContainsKey("id"))
                {
                    throw new InvalidOperationException("ProductMaster id is not a valid target column.");
                }

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        bool exists = ProductIdExists(conn, transaction, data["id"]);
                        if (!exists)
                        {
                            ValidateDisplayNameForNewProduct(conn, transaction, data);
                        }

                        int updated = UpdateProductMaster(conn, transaction, data);
                        if (updated == 0)
                        {
                            InsertProductMaster(conn, transaction, data);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                        }

                        throw;
                    }
                }
            }
        }

        private static List<string> GetTableColumns(SqlConnection conn)
        {
            List<string> columns = new List<string>();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = @table
ORDER BY ORDINAL_POSITION";
                cmd.Parameters.Add("@table", SqlDbType.VarChar, 128).Value = "ProductMaster";

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columns.Add(reader.GetString(0));
                    }
                }
            }

            return columns;
        }

        private static bool ProductIdExists(SqlConnection conn, SqlTransaction transaction, object productId)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = "SELECT COUNT(1) FROM ProductMaster WITH (UPDLOCK, HOLDLOCK) WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", productId ?? DBNull.Value);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static void ValidateDisplayNameForNewProduct(SqlConnection conn, SqlTransaction transaction, Dictionary<string, object> data)
        {
            string displayName = NormalizeBusinessString(GetValue(data, "DisplayName"));
            if (string.IsNullOrEmpty(displayName))
            {
                throw new ProductUpdatedConflictException("Product validation failed. DisplayName is missing.");
            }

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = @"
SELECT TOP 1 id
FROM ProductMaster WITH (UPDLOCK, HOLDLOCK)
WHERE UPPER(LTRIM(RTRIM(CONVERT(nvarchar(255), DisplayName)))) = @DisplayName
  AND CONVERT(varchar(50), id) <> CONVERT(varchar(50), @id)";
                cmd.Parameters.AddWithValue("@DisplayName", displayName.ToUpperInvariant());
                cmd.Parameters.AddWithValue("@id", data["id"] ?? DBNull.Value);

                object conflictingId = cmd.ExecuteScalar();
                if (conflictingId != null && conflictingId != DBNull.Value)
                {
                    throw new ProductUpdatedConflictException("Product validation failed. DisplayName already exists with a different ProductId.");
                }
            }
        }

        private static object GetValue(Dictionary<string, object> data, string key)
        {
            object value;
            return data.TryGetValue(key, out value) ? value : null;
        }

        private static string NormalizeBusinessString(object value)
        {
            return Convert.ToString(value).Trim();
        }

        private static int UpdateProductMaster(SqlConnection conn, SqlTransaction transaction, Dictionary<string, object> data)
        {
            List<string> sets = new List<string>();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;

                foreach (KeyValuePair<string, object> item in data)
                {
                    if (string.Equals(item.Key, "id", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string parameter = "@p_" + item.Key.Replace(" ", "_");
                    sets.Add("[" + item.Key.Replace("]", "]]") + "] = " + parameter);
                    cmd.Parameters.AddWithValue(parameter, item.Value ?? DBNull.Value);
                }

                if (sets.Count == 0)
                {
                    return 0;
                }

                cmd.CommandText = "UPDATE ProductMaster SET " + string.Join(",", sets.ToArray()) + " WHERE [id] = @id";
                cmd.Parameters.AddWithValue("@id", data["id"] ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
        }

        private static void InsertProductMaster(SqlConnection conn, SqlTransaction transaction, Dictionary<string, object> data)
        {
            bool idIsIdentity = IsIdentityColumn(conn, transaction, "ProductMaster", "id");
            List<string> columns = new List<string>();
            List<string> parameters = new List<string>();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;

                foreach (KeyValuePair<string, object> item in data)
                {
                    string parameter = "@p_" + item.Key.Replace(" ", "_");
                    columns.Add("[" + item.Key.Replace("]", "]]") + "]");
                    parameters.Add(parameter);
                    cmd.Parameters.AddWithValue(parameter, item.Value ?? DBNull.Value);
                }

                string insertSql = "INSERT INTO ProductMaster (" + string.Join(",", columns.ToArray()) + ") VALUES (" + string.Join(",", parameters.ToArray()) + ")";
                if (idIsIdentity)
                {
                    insertSql = "SET IDENTITY_INSERT ProductMaster ON; " + insertSql + "; SET IDENTITY_INSERT ProductMaster OFF;";
                }

                cmd.CommandText = insertSql;
                cmd.ExecuteNonQuery();
            }
        }

        private static bool IsIdentityColumn(SqlConnection conn, SqlTransaction transaction, string tableName, string columnName)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = "SELECT COLUMNPROPERTY(OBJECT_ID(@table), @column, 'IsIdentity')";
                cmd.Parameters.Add("@table", SqlDbType.VarChar, 128).Value = tableName;
                cmd.Parameters.Add("@column", SqlDbType.VarChar, 128).Value = columnName;
                object result = cmd.ExecuteScalar();
                return result != null && result != DBNull.Value && Convert.ToInt32(result) == 1;
            }
        }
    }
}
