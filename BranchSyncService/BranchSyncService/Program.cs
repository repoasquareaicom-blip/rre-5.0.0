using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Configuration;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Web.Script.Serialization;

namespace BranchSync
{
    class Program
    {
        // Log helper: writes to a daily log file in the exe folder
        static string LogDirectory
        {
            get
            {
                try
                {
                    var loc = Assembly.GetExecutingAssembly().Location;
                    return Path.GetDirectoryName(loc) ?? AppDomain.CurrentDomain.BaseDirectory;
                }
                catch
                {
                    return AppDomain.CurrentDomain.BaseDirectory;
                }
            }
        }

        // Concise log: one line per entry with ISO timestamp and level. Keep messages short to reduce log size.
        static void Log(string message, string level = "INFO")
        {
            try
            {
                var file = Path.Combine(LogDirectory, DateTime.UtcNow.ToString("yyyy-MM-dd") + ".log");
                var line = DateTime.UtcNow.ToString("o") + " " + level + " " + message + Environment.NewLine;
                File.AppendAllText(file, line, Encoding.UTF8);
            }
            catch
            {
                // ignore logging failures
            }
        }

        // Log an exception in compact form: show type and message only (no stacktrace) to keep logs small.
        static void LogError(string context, Exception ex)
        {
            try
            {
                if (ex == null) { Log(context, "ERROR"); return; }
                string msg = string.Format("{0}: {1}: {2}", context, ex.GetType().Name, ex.Message);
                Log(msg, "ERROR");
            }
            catch
            {
                // ignore logging failures
            }
        }

        static int Main(string[] args)
        {
            bool once = true;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--loop" || args[i] == "--daemon") { once = false; break; }
                if (args[i] == "--once") { once = true; break; }
            }

            Log("=== Service started ===");

            if (once)
            {
                Console.WriteLine("Run once mode: starting sync...");
                RunSyncOnce(true);
                Console.WriteLine("Run once mode: finished.");
                Log("=== Service stopped (once) ===");
                return 0;
            }

            Log("Entering loop mode");
            while (true)
            {
                try
                {
                    RunSyncOnce(false);
                }
                catch (Exception ex)
                {
                    LogError("Run loop exception", ex);
                }
                Thread.Sleep(60000);
            }
        }

        static void RunSyncOnce(bool consoleOutput)
        {
            Log("=== Run START ===");
            if (consoleOutput) Console.WriteLine("Run START");

            try
            {
                string apiCursor = ConfigurationManager.AppSettings["PresenceCursorUrl"];
                string quotationPushUrl = ConfigurationManager.AppSettings["PresencePushUrl"];
                string estimationPushUrl = ConfigurationManager.AppSettings["EstimationPushUrl"];
                string salesPushUrl = ConfigurationManager.AppSettings["SalesPushUrl"];
                string salesPipesPushUrl = ConfigurationManager.AppSettings["SalesPipesPushUrl"];
                string salesTradersPushUrl = ConfigurationManager.AppSettings["SalesTradersPushUrl"];
                string productChangesUrl = ConfigurationManager.AppSettings["ProductChangesUrl"];

                string branch = ConfigurationManager.AppSettings["BranchCode"];
                string apiKey = ConfigurationManager.AppSettings["ApiKey"];
                string connStr = ConfigurationManager.AppSettings["ConnectionString"];

                if (string.IsNullOrEmpty(apiCursor) || string.IsNullOrEmpty(quotationPushUrl) ||
                    string.IsNullOrEmpty(branch) || string.IsNullOrEmpty(apiKey))
                {
                    Log("Missing required app settings; aborting run");
                    if (consoleOutput) Console.WriteLine("Missing required app settings; aborting run");
                    Log("=== Run END ===");
                    if (consoleOutput) Console.WriteLine("Run END");
                    return;
                }

                if (string.IsNullOrEmpty(connStr))
                {
                    Log("Missing ConnectionString; aborting run");
                    if (consoleOutput) Console.WriteLine("Missing ConnectionString; aborting run");
                    Log("=== Run END ===");
                    if (consoleOutput) Console.WriteLine("Run END");
                    return;
                }

                int batchSize = 5;
                int parsedBatchSize;
                if (int.TryParse(ConfigurationManager.AppSettings["BatchSize"], out parsedBatchSize) && parsedBatchSize > 0)
                {
                    batchSize = parsedBatchSize;
                }

                string cursorJson = GetCursor(apiCursor, branch, apiKey);
                Log("Cursor " + (string.IsNullOrEmpty(cursorJson) ? "empty" : "present,len=" + cursorJson.Length));
                if (consoleOutput) Console.WriteLine("Cursor: " + (string.IsNullOrEmpty(cursorJson) ? "<empty>" : "present,len=" + cursorJson.Length));

                SyncModule(
                    moduleName: "Quotations",
                    cursorJson: cursorJson,
                    cursorFieldName: "quotation_last_sync_on",
                    pushUrl: quotationPushUrl,
                    connStr: connStr,
                    branch: branch,
                    apiKey: apiKey,
                    headerTable: "QuotationHeader",
                    detailTable: "QuotationDetails",
                    headerIdColumn: "Quotationid",
                    detailIdColumn: "Quotationid",
                    dateColumn: "Updatedon",
                    batchSize: batchSize,
                    consoleOutput: consoleOutput);

                SyncModule(
                    moduleName: "Estimations",
                    cursorJson: cursorJson,
                    cursorFieldName: "estimation_last_sync_on",
                    pushUrl: estimationPushUrl,
                    connStr: connStr,
                    branch: branch,
                    apiKey: apiKey,
                    headerTable: "QuotationEstimation",
                    detailTable: "QuotationEstimationDetails",
                    headerIdColumn: "Estimationid",
                    detailIdColumn: "Estimationid",
                    dateColumn: "Updatedon",
                    batchSize: batchSize,
                    consoleOutput: consoleOutput);

                SyncModule(
                    moduleName: "Sales",
                    cursorJson: cursorJson,
                    cursorFieldName: "sales_last_sync_on",
                    pushUrl: salesPushUrl,
                    connStr: connStr,
                    branch: branch,
                    apiKey: apiKey,
                    headerTable: "Sales",
                    detailTable: "SalesDetails",
                    headerIdColumn: "Salesid",
                    detailIdColumn: "Salesid",
                    dateColumn: "EnteredOn",
                    batchSize: batchSize,
                    consoleOutput: consoleOutput);

                SyncModule(
                    moduleName: "Sales Pipes",
                    cursorJson: cursorJson,
                    cursorFieldName: "sales_pipes_last_sync_on",
                    pushUrl: salesPipesPushUrl,
                    connStr: connStr,
                    branch: branch,
                    apiKey: apiKey,
                    headerTable: "SalesPipes",
                    detailTable: "SalesPipesDetails",
                    headerIdColumn: "Salesid",
                    detailIdColumn: "Salesid",
                    dateColumn: "EnteredOn",
                    batchSize: batchSize,
                    consoleOutput: consoleOutput);

                SyncModule(
                    moduleName: "Sales Traders",
                    cursorJson: cursorJson,
                    cursorFieldName: "sales_traders_last_sync_on",
                    pushUrl: salesTradersPushUrl,
                    connStr: connStr,
                    branch: branch,
                    apiKey: apiKey,
                    headerTable: "SalesTraders",
                    detailTable: "SalesTradersDetails",
                    headerIdColumn: "Salesid",
                    detailIdColumn: "Salesid",
                    dateColumn: "EnteredOn",
                    batchSize: batchSize,
                    consoleOutput: consoleOutput);

                PullProductChanges(productChangesUrl, connStr, branch, apiKey, consoleOutput);

                Log("=== Run END ===");
                if (consoleOutput) Console.WriteLine("Run END");
            }
            catch (Exception ex)
            {
                LogError("RunSyncOnce exception", ex);
                if (consoleOutput) Console.WriteLine("RunSyncOnce exception: " + ex.Message);

                Log("=== Run END ===", "INFO");
                if (consoleOutput) Console.WriteLine("Run END");
            }
        }

        static void SyncModule(
            string moduleName,
            string cursorJson,
            string cursorFieldName,
            string pushUrl,
            string connStr,
            string branch,
            string apiKey,
            string headerTable,
            string detailTable,
            string headerIdColumn,
            string detailIdColumn,
            string dateColumn,
            int batchSize,
            bool consoleOutput)
        {
            if (string.IsNullOrEmpty(pushUrl))
            {
                Log(moduleName + ": Push URL missing. Skipping.");
                if (consoleOutput) Console.WriteLine(moduleName + ": Push URL missing. Skipping.");
                return;
            }

            try
            {
                Log(moduleName + " START");
                if (consoleOutput) Console.WriteLine(moduleName + " START");

                string lastSyncedAt = ParseCursorValue(cursorJson, cursorFieldName);

                if (!string.IsNullOrEmpty(lastSyncedAt))
                {
                    Log(moduleName + " cursor=" + lastSyncedAt);
                    if (consoleOutput) Console.WriteLine(moduleName + " cursor=" + lastSyncedAt);
                }
                else
                {
                    Log(moduleName + " cursor empty, using latest record");
                    if (consoleOutput) Console.WriteLine(moduleName + " cursor empty, using latest record");
                }

                DateTime since = DateTime.MinValue;
                if (!string.IsNullOrEmpty(lastSyncedAt))
                {
                    DateTime parsed;
                    if (DateTime.TryParse(
                        lastSyncedAt,
                        null,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out parsed))
                    {
                        since = parsed;
                    }
                }

                var records = new List<string>();
                int fetchedHeaders = 0;

                using (var conn = new SqlConnection(connStr))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();

                    if (since == DateTime.MinValue)
                    {
                        cmd.CommandText = string.Format(@"
                            SELECT TOP (1) *
                            FROM {0}
                            ORDER BY {1} DESC", headerTable, dateColumn);
                    }
                    else
                    {
                        cmd.CommandText = string.Format(@"
                            SELECT TOP ({0}) *
                            FROM {1}
                            WHERE {2} > @since
                            ORDER BY {2} ASC", batchSize, headerTable, dateColumn);

                        cmd.Parameters.Add("@since", SqlDbType.DateTime).Value = since;
                    }

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            fetchedHeaders++;

                            string headerJson = RowToJson(rdr);

                            string mainId = rdr[headerIdColumn] != DBNull.Value
                                ? rdr[headerIdColumn].ToString()
                                : null;

                            var details = new List<string>();

                            if (!string.IsNullOrEmpty(mainId))
                            {
                                using (var dconn = new SqlConnection(connStr))
                                using (var dcmd = dconn.CreateCommand())
                                {
                                    dconn.Open();

                                    dcmd.CommandText = string.Format(@"
                                        SELECT *
                                        FROM {0}
                                        WHERE {1} = @id", detailTable, detailIdColumn);

                                    dcmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = mainId;

                                    using (var dr = dcmd.ExecuteReader())
                                    {
                                        while (dr.Read())
                                        {
                                            details.Add(RowToJson(dr));
                                        }
                                    }
                                }
                            }

                            var sb = new StringBuilder();
                            sb.Append("{\"header\":");
                            sb.Append(headerJson);
                            sb.Append(",\"details\":[");
                            sb.Append(string.Join(",", details));
                            sb.Append("]}");

                            records.Add(sb.ToString());
                        }
                    }
                }

                Log(moduleName + " fetched=" + fetchedHeaders);
                if (consoleOutput) Console.WriteLine(moduleName + " fetched=" + fetchedHeaders);

                if (records.Count > 0)
                {
                    string payload = BuildPayload(records);

                    Log(moduleName + " posting=" + records.Count + " to=" + pushUrl);
                    if (consoleOutput) Console.WriteLine(moduleName + " posting=" + records.Count);

                    PostJson(pushUrl, branch, apiKey, payload);

                    Log(moduleName + " completed=" + records.Count);
                    if (consoleOutput) Console.WriteLine(moduleName + " completed=" + records.Count);
                }
                else
                {
                    Log(moduleName + " completed=0");
                    if (consoleOutput) Console.WriteLine(moduleName + " completed=0");
                }

                Log(moduleName + " END");
                if (consoleOutput) Console.WriteLine(moduleName + " END");
            }
            catch (Exception ex)
            {
                LogError(moduleName + " sync exception", ex);
                if (consoleOutput) Console.WriteLine(moduleName + " sync exception: " + ex.Message);
            }
        }

        static string BuildPayload(List<string> records)
        {
            var sb = new StringBuilder();
            sb.Append('{');
            sb.AppendFormat("\"last_synced_at\":\"{0}\",", DateTime.UtcNow.ToString("o"));
            sb.Append("\"records\":[");
            sb.Append(string.Join(",", records));
            sb.Append("]}");
            return sb.ToString();
        }

        static string RowToJson(IDataRecord rdr)
        {
            var parts = new List<string>();
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                string name = rdr.GetName(i);
                object val = rdr.GetValue(i);
                string jsonVal;

                if (val == DBNull.Value)
                {
                    jsonVal = "null";
                }
                else if (val is DateTime)
                {
                    jsonVal = "\"" + ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "\"";
                }
                else if (val is int || val is long || val is decimal || val is double || val is float || val is short || val is byte)
                {
                    jsonVal = Convert.ToString(val, CultureInfo.InvariantCulture);
                }
                else if (val is bool)
                {
                    jsonVal = ((bool)val) ? "true" : "false";
                }
                else
                {
                    jsonVal = "\"" + EscapeJson(val.ToString()) + "\"";
                }

                parts.Add("\"" + EscapeJson(name) + "\":" + jsonVal);
            }
            return "{" + string.Join(",", parts) + "}";
        }

        sealed class ProductChangesResponse
        {
            public bool ok { get; set; }
            public List<ProductChangeRecord>? records { get; set; }
        }

        sealed class ProductChangeRecord
        {
            public long change_id { get; set; }
            public Dictionary<string, object>? payload { get; set; }
        }

        static void PullProductChanges(string url, string connStr, string branch, string apiKey, bool consoleOutput)
        {
            if (string.IsNullOrEmpty(url))
            {
                Log("Products: Changes URL missing. Skipping.");
                if (consoleOutput) Console.WriteLine("Products: Changes URL missing. Skipping.");
                return;
            }

            if (string.Equals(branch, "RR-SALEM", StringComparison.OrdinalIgnoreCase))
            {
                Log("Products: Salem source branch. Skipping cloud pull.");
                if (consoleOutput) Console.WriteLine("Products: Salem source branch. Skipping cloud pull.");
                return;
            }

            try
            {
                long afterId = ReadProductCursor();
                string separator = url.IndexOf('?') >= 0 ? "&" : "?";
                string fetchUrl = url + separator + "after_id=" + afterId.ToString(CultureInfo.InvariantCulture) + "&limit=100";

                Log("Products START cursor=" + afterId);
                if (consoleOutput) Console.WriteLine("Products START cursor=" + afterId);

                string json = GetJson(fetchUrl, branch, apiKey);
                if (string.IsNullOrEmpty(json))
                {
                    Log("Products empty response");
                    if (consoleOutput) Console.WriteLine("Products empty response");
                    return;
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                ProductChangesResponse response = serializer.Deserialize<ProductChangesResponse>(json);
                if (response == null || response.records == null || response.records.Count == 0)
                {
                    Log("Products completed=0");
                    if (consoleOutput) Console.WriteLine("Products completed=0");
                    return;
                }

                int applied = 0;
                long maxChangeId = afterId;
                foreach (ProductChangeRecord record in response.records)
                {
                    if (record == null || record.payload == null)
                    {
                        continue;
                    }

                    UpsertProductMaster(connStr, record.payload);
                    applied++;

                    if (record.change_id > maxChangeId)
                    {
                        maxChangeId = record.change_id;
                    }
                }

                if (maxChangeId > afterId)
                {
                    WriteProductCursor(maxChangeId);
                }

                Log("Products completed=" + applied);
                if (consoleOutput) Console.WriteLine("Products completed=" + applied);
            }
            catch (Exception ex)
            {
                LogError("Products sync exception", ex);
                if (consoleOutput) Console.WriteLine("Products sync exception: " + ex.Message);
            }
        }

        static void UpsertProductMaster(string connStr, Dictionary<string, object> payload)
        {
            if (!payload.ContainsKey("id") || payload["id"] == null)
            {
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                List<string> columns = GetTableColumns(conn, "ProductMaster");
                if (columns.Count == 0)
                {
                    return;
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
                    return;
                }

                int updated = UpdateProductMaster(conn, data);
                if (updated == 0)
                {
                    InsertProductMaster(conn, data);
                }
            }
        }

        static List<string> GetTableColumns(SqlConnection conn, string tableName)
        {
            List<string> columns = new List<string>();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = @table
                    ORDER BY ORDINAL_POSITION";
                cmd.Parameters.Add("@table", SqlDbType.VarChar, 128).Value = tableName;

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

        static int UpdateProductMaster(SqlConnection conn, Dictionary<string, object> data)
        {
            List<string> sets = new List<string>();
            using (SqlCommand cmd = conn.CreateCommand())
            {
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

                cmd.CommandText = "UPDATE ProductMaster SET " + string.Join(",", sets) + " WHERE [id] = @id";
                cmd.Parameters.AddWithValue("@id", data["id"] ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
        }

        static void InsertProductMaster(SqlConnection conn, Dictionary<string, object> data)
        {
            bool idIsIdentity = IsIdentityColumn(conn, "ProductMaster", "id");
            List<string> columns = new List<string>();
            List<string> parameters = new List<string>();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                foreach (KeyValuePair<string, object> item in data)
                {
                    string parameter = "@p_" + item.Key.Replace(" ", "_");
                    columns.Add("[" + item.Key.Replace("]", "]]") + "]");
                    parameters.Add(parameter);
                    cmd.Parameters.AddWithValue(parameter, item.Value ?? DBNull.Value);
                }

                string insertSql = "INSERT INTO ProductMaster (" + string.Join(",", columns) + ") VALUES (" + string.Join(",", parameters) + ")";
                if (idIsIdentity)
                {
                    insertSql = "SET IDENTITY_INSERT ProductMaster ON; " + insertSql + "; SET IDENTITY_INSERT ProductMaster OFF;";
                }

                cmd.CommandText = insertSql;
                cmd.ExecuteNonQuery();
            }
        }

        static bool IsIdentityColumn(SqlConnection conn, string tableName, string columnName)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COLUMNPROPERTY(OBJECT_ID(@table), @column, 'IsIdentity')";
                cmd.Parameters.Add("@table", SqlDbType.VarChar, 128).Value = tableName;
                cmd.Parameters.Add("@column", SqlDbType.VarChar, 128).Value = columnName;
                object result = cmd.ExecuteScalar();
                return result != null && result != DBNull.Value && Convert.ToInt32(result, CultureInfo.InvariantCulture) == 1;
            }
        }

        static long ReadProductCursor()
        {
            try
            {
                string path = ProductCursorPath();
                if (!File.Exists(path))
                {
                    return 0;
                }

                long value;
                if (long.TryParse(File.ReadAllText(path).Trim(), out value))
                {
                    return value;
                }
            }
            catch
            {
            }

            return 0;
        }

        static void WriteProductCursor(long changeId)
        {
            try
            {
                File.WriteAllText(ProductCursorPath(), changeId.ToString(CultureInfo.InvariantCulture), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                LogError("Write product cursor failed", ex);
            }
        }

        static string ProductCursorPath()
        {
            return Path.Combine(LogDirectory, "product-master.cursor");
        }

        static string GetField(IDataRecord r, string name)
        {
            try { var v = r[name]; return v == DBNull.Value ? null : v.ToString(); } catch { return null; }
        }

        // Kept for old quotation-specific code compatibility. New sync uses RowToJson.
        static string BuildHeaderJson(IDataRecord rdr)
        {
            return RowToJson(rdr);
        }

        // Kept for old quotation-specific code compatibility. New sync uses RowToJson.
        static string BuildDetailJson(IDataRecord dr)
        {
            return RowToJson(dr);
        }

        static string EscapeJson(string s)
        {
            if (s == null) return string.Empty;
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
        }

        static string GetCursor(string url, string branch, string apiKey)
        {
            return GetJson(url, branch, apiKey);
        }

        static string GetJson(string url, string branch, string apiKey)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Accept = "application/json";
                request.Headers["X-Branch-Code"] = branch;
                request.Headers["X-Api-Key"] = apiKey;

                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                LogError("GetCursor failed", ex);
                return null;
            }
        }

        static string ParseLastSyncedAt(string json)
        {
            return ParseCursorValue(json, "quotation_last_sync_on");
        }

        static string ParseCursorValue(string json, string fieldName)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(fieldName)) return null;

            string key = "\"" + fieldName + "\"";
            int foundIdx = json.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (foundIdx < 0) return null;

            int idxColon = json.IndexOf(':', foundIdx);
            if (idxColon < 0) return null;
            int cur = idxColon + 1;

            while (cur < json.Length && char.IsWhiteSpace(json[cur])) cur++;
            if (cur >= json.Length) return null;

            if (json.Length >= cur + 4 && string.Compare(json, cur, "null", 0, 4, StringComparison.OrdinalIgnoreCase) == 0)
                return null;

            if (json[cur] == '"')
            {
                int end = json.IndexOf('"', cur + 1);
                if (end <= cur) return null;
                return json.Substring(cur + 1, end - cur - 1);
            }

            int endIdx = cur;
            while (endIdx < json.Length && !char.IsWhiteSpace(json[endIdx]) && json[endIdx] != ',' && json[endIdx] != '}') endIdx++;
            if (endIdx <= cur) return null;
            return json.Substring(cur, endIdx - cur);
        }

        static void PostJson(string url, string branch, string apiKey, string json)
        {
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers["X-Branch-Code"] = branch;
                request.Headers["X-Api-Key"] = apiKey;

                byte[] bytes = Encoding.UTF8.GetBytes(json ?? string.Empty);
                request.ContentLength = bytes.Length;

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    string respText = sr.ReadToEnd();
                    Log($"POST {url} status={(int)resp.StatusCode} len={(respText != null ? respText.Length : 0)}");
                }
            }
            catch (WebException wex)
            {
                try
                {
                    var resp = wex.Response as HttpWebResponse;
                    string body = string.Empty;
                    if (resp != null)
                    {
                        using (var sr = new StreamReader(resp.GetResponseStream()))
                        {
                            body = sr.ReadToEnd();
                        }
                        Log($"PostJson failed: HTTP {(int)resp.StatusCode} {resp.StatusDescription} bodyLen={(body != null ? body.Length : 0)}", "ERROR");
                    }
                    else
                    {
                        LogError("PostJson WebException", wex);
                    }
                }
                catch (Exception ex2)
                {
                    LogError("PostJson failed reading error response", ex2);
                }
            }
            catch (Exception ex)
            {
                LogError("PostJson failed", ex);
            }
        }
    }
}
