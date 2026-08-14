using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Inventory
{
    public static class ProductCloudSyncClient
    {
        public static void PushProductById(string productId)
        {
            TryPushProductById(productId);
        }

        public static ProductCloudSyncResult TryPushProductById(string productId)
        {
            return TryPushProductById(productId, BranchAccess.MainOfficeBranchCode);
        }

        public static ProductCloudSyncResult TryPushProductById(string productId, string targetBranchCode)
        {
            if (!BranchAccess.IsMainOffice || string.IsNullOrEmpty(productId))
            {
                return ProductCloudSyncResult.Failed("Invalid branch or product id.");
            }

            if (string.IsNullOrEmpty(targetBranchCode))
            {
                return ProductCloudSyncResult.Failed("Target branch code is missing.");
            }

            try
            {
                using (SqlConnection con = new SqlConnection(Program.connection))
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM ProductMaster WHERE id = @id", con))
                {
                    cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = productId;
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PushProductRecord(reader, targetBranchCode);
                            return ProductCloudSyncResult.Ok();
                        }
                    }
                }

                return ProductCloudSyncResult.Failed("Product not found in local ProductMaster.");
            }
            catch (Exception ex)
            {
                return ProductCloudSyncResult.Failed(GetExceptionMessage(ex));
            }
        }

        public static void PushProductsByIds(DataTable products)
        {
            if (!BranchAccess.IsMainOffice || products == null || products.Rows.Count == 0)
            {
                return;
            }

            foreach (DataRow row in products.Rows)
            {
                if (products.Columns.Contains("Id"))
                {
                    ProductMasterCloudQueue.EnqueueAndTryPush(Convert.ToString(row["Id"]), "Product", false);
                }
                else if (products.Columns.Contains("id"))
                {
                    ProductMasterCloudQueue.EnqueueAndTryPush(Convert.ToString(row["id"]), "Product", false);
                }
            }
        }

        private static void PushProductRecord(IDataRecord record, string targetBranchCode)
        {
            string apiUrl = GetTargetProductUpsertUrl(targetBranchCode);
            string branch = ConfigurationManager.AppSettings["BranchCode"];
            string apiKey = ConfigurationManager.AppSettings["ApiKey"];

            if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(branch) || string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Product sync API URL, BranchCode, or ApiKey is missing in app config for " + targetBranchCode + ".");
            }

            string json = "{\"records\":[" + RowToJson(record) + "]}";

            Program.ConfigureApiSecurity();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers["X-Branch-Code"] = branch;
            request.Headers["X-Api-Key"] = apiKey;
            request.Timeout = 30000;
            request.ReadWriteTimeout = 30000;

            byte[] bytes = Encoding.UTF8.GetBytes(json);
            request.ContentLength = bytes.Length;

            using (Stream req = request.GetRequestStream())
            {
                req.Write(bytes, 0, bytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if ((int)response.StatusCode < 200 || (int)response.StatusCode >= 300)
                {
                    throw new InvalidOperationException("Branch API returned HTTP " + (int)response.StatusCode + " " + response.StatusDescription);
                }

                string responseText = string.Empty;
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                }

                if (!string.IsNullOrEmpty(responseText) && responseText.ToLowerInvariant().IndexOf("\"success\":false") >= 0)
                {
                    throw new InvalidOperationException("Branch API rejected product sync: " + responseText);
                }
                response.Close();
            }
        }

        private static string GetTargetProductUpsertUrl(string targetBranchCode)
        {
            string baseUrl = ConfigurationManager.AppSettings["BranchApi_" + targetBranchCode];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("Branch API URL missing for " + targetBranchCode + ".");
            }

            return baseUrl.TrimEnd('/') + "/api/productmaster/upsert";
        }

        private static string GetExceptionMessage(Exception ex)
        {
            WebException webException = ex as WebException;
            if (webException != null && webException.Response != null)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(webException.Response.GetResponseStream()))
                    {
                        string body = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(body))
                        {
                            return webException.Message + " " + body;
                        }
                    }
                }
                catch
                {
                }
            }

            if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
            {
                return ex.Message + " Inner: " + ex.InnerException.Message;
            }

            return ex.Message;
        }

        private static string RowToJson(IDataRecord record)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            for (int i = 0; i < record.FieldCount; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }

                sb.Append("\"");
                sb.Append(EscapeJson(record.GetName(i)));
                sb.Append("\":");
                sb.Append(ValueToJson(record.GetValue(i)));
            }

            sb.Append("}");
            return sb.ToString();
        }

        private static string ValueToJson(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "null";
            }

            if (value is bool)
            {
                return ((bool)value) ? "true" : "false";
            }

            if (value is DateTime)
            {
                return "\"" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "\"";
            }

            if (value is byte || value is short || value is int || value is long || value is decimal || value is double || value is float)
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }

            return "\"" + EscapeJson(Convert.ToString(value)) + "\"";
        }

        private static string EscapeJson(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }
    }
}
