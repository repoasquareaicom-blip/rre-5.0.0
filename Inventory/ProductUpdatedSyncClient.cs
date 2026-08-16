using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Inventory
{
    public static class ProductUpdatedSyncClient
    {
        private const string PendingProcedureName = "dbo.sp_product_sync_pending_by_branch";
        private const string FullProductProcedureName = "dbo.sp_product_sync_full_product";

        public static ProductUpdatedReadResult FetchPendingForCurrentBranch()
        {
            ProductUpdatedReadResult result = new ProductUpdatedReadResult();
            result.Items = new List<ProductUpdatedPendingItem>();

            string branchCode = BranchAccess.CurrentBranchCode;
            if (branchCode != BranchAccess.MainOfficeBranchCode && branchCode != "RR-NAMAKKAL" && branchCode != "RR-KOLATHUR")
            {
                result.Success = true;
                return result;
            }

            try
            {
                List<string> targetBranches = new List<string>();
                if (BranchAccess.IsMainOffice)
                {
                    targetBranches.Add("RR-NAMAKKAL");
                    targetBranches.Add("RR-KOLATHUR");
                }
                else
                {
                    targetBranches.Add(branchCode);
                }

                foreach (string targetBranch in targetBranches)
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters["TargetBranchCode"] = targetBranch;

                    IList rows = RunSalemGetData(PendingProcedureName, parameters);
                    AddPendingRows(result.Items, rows, targetBranch);
                }

                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public static ProductUpdatedApplyResult AcceptOne(ProductUpdatedPendingItem item)
        {
            ProductUpdatedApplyResult result = new ProductUpdatedApplyResult();

            if (item == null || item.QueueId <= 0 || string.IsNullOrEmpty(item.ProductId))
            {
                result.Message = "Invalid pending item.";
                return result;
            }

            if (!string.Equals(item.TargetBranchCode, BranchAccess.CurrentBranchCode, StringComparison.Ordinal))
            {
                result.Message = "Queue row does not belong to this branch.";
                return result;
            }

            try
            {
                Dictionary<string, object> fullRow = FetchFullProduct(item.ProductId);
                if (fullRow == null)
                {
                    result.Message = "Salem ProductMaster row was not found.";
                    return result;
                }

                LocalProductMasterAcceptAllUpsert.Apply(fullRow);
                result.LocalUpdated = true;
            }
            catch (ProductUpdatedConflictException ex)
            {
                result.Conflict = true;
                result.Message = ex.Message;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }

            try
            {
                AcknowledgeSynced(item.QueueId, item.ProductId, item.TargetBranchCode);
                result.Acknowledged = true;
                result.Message = "Synced";
            }
            catch (Exception ex)
            {
                result.Message = "Updated locally, acknowledgement pending. " + ex.Message;
            }

            return result;
        }

        private static void AddPendingRows(List<ProductUpdatedPendingItem> items, IList rows, string targetBranchCode)
        {
            foreach (object rowObject in rows)
            {
                Dictionary<string, object> row = rowObject as Dictionary<string, object>;
                if (row == null)
                {
                    continue;
                }

                ProductUpdatedPendingItem item = new ProductUpdatedPendingItem();
                item.QueueId = ToInt(GetValue(row, "QueueId"));
                item.ProductId = ToText(GetValue(row, "ProductId"));
                item.DisplayName = ToText(GetValue(row, "DisplayName"));
                item.SalesPrice = GetValue(row, "SalesPrice");
                item.MRP = GetValue(row, "MRP");
                item.GST = GetValue(row, "GST");
                item.Status = ToText(GetValue(row, "Status"));
                item.ChangeType = ToText(GetValue(row, "ChangeType"));
                item.AttemptCount = ToInt(GetValue(row, "AttemptCount"));
                item.LastError = ToText(GetValue(row, "LastError"));
                item.LastTriedOn = GetValue(row, "LastTriedOn");
                item.TargetBranchCode = ToText(GetValue(row, "TargetBranchCode"));
                item.LocalStatus = string.Empty;

                if (string.Equals(item.TargetBranchCode, targetBranchCode, StringComparison.Ordinal))
                {
                    items.Add(item);
                }
            }
        }

        private static Dictionary<string, object> FetchFullProduct(string productId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["ProductId"] = productId;

            IList rows = RunSalemGetData(FullProductProcedureName, parameters);
            if (rows == null || rows.Count == 0)
            {
                return null;
            }

            return rows[0] as Dictionary<string, object>;
        }

        private static IList RunSalemGetData(string procedureName, Dictionary<string, object> parameters)
        {
            string url = GetSalemBaseUrl().TrimEnd('/') + "/api/getdata";
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload["queryText"] = procedureName;
            payload["parameters"] = parameters;

            Dictionary<string, object> response = PostJson(url, payload);
            if (response.ContainsKey("success") && response["success"] is bool && !((bool)response["success"]))
            {
                throw new InvalidOperationException(ToText(GetValue(response, "message")));
            }

            object data = GetValue(response, "data");
            IList rows = data as IList;
            if (rows == null)
            {
                throw new InvalidOperationException("Invalid Salem product sync response.");
            }

            return rows;
        }

        private static void AcknowledgeSynced(int queueId, string productId, string targetBranchCode)
        {
            string url = GetSalemBaseUrl().TrimEnd('/') + "/api/productmaster/queue/ack-synced";
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload["queueId"] = queueId;
            payload["productId"] = productId;
            payload["targetBranchCode"] = targetBranchCode;

            Dictionary<string, object> response = PostJson(url, payload);
            if (response.ContainsKey("success") && response["success"] is bool && !((bool)response["success"]))
            {
                throw new InvalidOperationException(ToText(GetValue(response, "message")));
            }
        }

        private static Dictionary<string, object> PostJson(string url, Dictionary<string, object> payload)
        {
            string apiKey = ConfigurationManager.AppSettings["BranchApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("BranchApiKey is missing in app config.");
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(payload);

            Program.ConfigureApiSecurity();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers["X-Api-Key"] = apiKey;
            request.Timeout = 30000;
            request.ReadWriteTimeout = 30000;

            byte[] bytes = Encoding.UTF8.GetBytes(json);
            request.ContentLength = bytes.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            string responseText;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                responseText = reader.ReadToEnd();
            }

            object responseObject = serializer.DeserializeObject(responseText);
            Dictionary<string, object> responseDictionary = responseObject as Dictionary<string, object>;
            if (responseDictionary == null)
            {
                throw new InvalidOperationException("Invalid API response.");
            }

            return responseDictionary;
        }

        private static string GetSalemBaseUrl()
        {
            string baseUrl = ConfigurationManager.AppSettings["BranchApi_" + BranchAccess.MainOfficeBranchCode];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("Salem Branch API URL is missing.");
            }

            return baseUrl;
        }

        internal static object GetValue(Dictionary<string, object> row, string key)
        {
            if (row == null)
            {
                return null;
            }

            foreach (KeyValuePair<string, object> item in row)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Value;
                }
            }

            return null;
        }

        internal static string ToText(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static int ToInt(object value)
        {
            int result;
            if (value == null || !int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out result))
            {
                return 0;
            }

            return result;
        }
    }

    public class ProductUpdatedConflictException : Exception
    {
        public ProductUpdatedConflictException(string message) : base(message)
        {
        }
    }
}
