using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace Inventory.Report
{
    public class OpenAiHsnLookupService
    {
        private const string Endpoint = "https://api.openai.com/v1/responses";
        private const string DefaultModel = "gpt-5-mini";
        private const int TimeoutMilliseconds = 20000;
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
        private int requestCount;

        public int RequestCount
        {
            get { return requestCount; }
        }

        public HsnSuggestion Lookup(HsnLookupRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Unavailable("Online HSN suggestion is currently unavailable. Please enter the HSN manually.");
                }

                Debug.WriteLine("HSN lookup started for ProductId " + request.ProductId);

                if (!IsEnabled())
                {
                    return Unavailable("Online HSN suggestion is disabled. Please enter the HSN manually.");
                }

                string apiKey = GetApiKey();
                if (string.IsNullOrEmpty(apiKey))
                {
                    return Unavailable("Online HSN suggestion is not configured. Please enter the HSN manually.");
                }

                Interlocked.Increment(ref requestCount);
                string responseJson = PostRequest(apiKey, BuildRequestBody(request));
                HsnSuggestion suggestion = ParseResponse(responseJson);
                Debug.WriteLine("OpenAI lookup completed for ProductId " + request.ProductId);
                return suggestion;
            }
            catch (WebException ex)
            {
                Debug.WriteLine("OpenAI lookup failed: " + ex.Status);
                return Unavailable("Unable to get an OpenAI HSN suggestion. You can try again or enter/search the HSN manually.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OpenAI lookup failed: " + ex.GetType().Name);
                return Unavailable("Unable to get an OpenAI HSN suggestion. You can try again or enter/search the HSN manually.");
            }
        }

        private string PostRequest(string apiKey, string requestBody)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Endpoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + apiKey;
            request.Timeout = TimeoutMilliseconds;
            request.ReadWriteTimeout = TimeoutMilliseconds;

            byte[] payload = Encoding.UTF8.GetBytes(requestBody);
            request.ContentLength = payload.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(payload, 0, payload.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private string BuildRequestBody(HsnLookupRequest request)
        {
            Dictionary<string, object> schemaProperties = new Dictionary<string, object>();
            schemaProperties["hsn_code"] = new Dictionary<string, object> { { "type", "string" } };
            schemaProperties["category"] = new Dictionary<string, object> { { "type", "string" } };
            schemaProperties["confidence"] = new Dictionary<string, object> { { "type", "string" }, { "enum", new string[] { "high", "medium", "low", "no_match" } } };
            schemaProperties["reason"] = new Dictionary<string, object> { { "type", "string" } };
            schemaProperties["needs_review"] = new Dictionary<string, object> { { "type", "boolean" } };

            Dictionary<string, object> jsonSchema = new Dictionary<string, object>();
            jsonSchema["name"] = "hsn_suggestion";
            jsonSchema["strict"] = true;
            jsonSchema["schema"] = new Dictionary<string, object>
            {
                { "type", "object" },
                { "additionalProperties", false },
                { "required", new string[] { "hsn_code", "category", "confidence", "reason", "needs_review" } },
                { "properties", schemaProperties }
            };

            Dictionary<string, object> text = new Dictionary<string, object>();
            text["format"] = new Dictionary<string, object> { { "type", "json_schema" }, { "name", "hsn_suggestion" }, { "strict", true }, { "schema", jsonSchema["schema"] } };

            Dictionary<string, object> body = new Dictionary<string, object>();
            body["model"] = GetModel();
            body["instructions"] = GetInstructions();
            body["input"] = BuildInput(request);
            body["text"] = text;

            return serializer.Serialize(body);
        }

        private string BuildInput(HsnLookupRequest request)
        {
            StringBuilder input = new StringBuilder();
            AppendLine(input, "Product Name", request.ProductName);
            AppendLine(input, "Current HSN", request.CurrentHsn);
            AppendLine(input, "GST", request.GstPercent);
            AppendLine(input, "Product Category", request.Category);
            AppendLine(input, "Unit", request.Uom);
            return input.ToString();
        }

        private static void AppendLine(StringBuilder builder, string name, string value)
        {
            string cleaned = (value ?? string.Empty).Trim();
            if (cleaned.Length > 0)
            {
                builder.AppendLine(name + ": " + cleaned);
            }
        }

        private static string GetInstructions()
        {
            return @"Classify this product under Indian GST HSN.
Return the most likely 8-digit HSN when reasonably determinable.
Consider the product's actual function and type, not brand/model text.
If the description is insufficient, do not guess.
Return hsn_code, category, confidence, reason, needs_review.
Use confidence: high, medium, low, no_match.
Keep reason short.";
        }

        private HsnSuggestion ParseResponse(string responseJson)
        {
            Dictionary<string, object> response = serializer.DeserializeObject(responseJson) as Dictionary<string, object>;
            string outputText = FindOutputText(response);
            if (string.IsNullOrEmpty(outputText))
            {
                return InvalidResponse();
            }

            Dictionary<string, object> result = serializer.DeserializeObject(outputText) as Dictionary<string, object>;
            if (result == null)
            {
                return InvalidResponse();
            }

            string hsn = GetString(result, "hsn_code").Trim();
            string confidence = NormalizeConfidence(GetString(result, "confidence"));
            bool hasValidHsn = hsn.Length == 0 || HsnSuggestionService.IsValidHsn(hsn);
            if (!hasValidHsn)
            {
                return InvalidResponse();
            }

            if (hsn.Length == 0 && confidence != "No Match")
            {
                confidence = "No Match";
            }

            return new HsnSuggestion
            {
                HsnCode = hsn,
                Category = GetString(result, "category"),
                Confidence = confidence,
                Reason = GetString(result, "reason"),
                Source = "OpenAI",
                NeedsReview = GetBool(result, "needs_review") || confidence == "Low" || confidence == "No Match"
            };
        }

        private static string FindOutputText(Dictionary<string, object> response)
        {
            if (response == null) return string.Empty;
            object output;
            if (response.TryGetValue("output_text", out output))
            {
                return Convert.ToString(output) ?? string.Empty;
            }

            object outputObj;
            if (!response.TryGetValue("output", out outputObj)) return string.Empty;
            object[] outputItems = outputObj as object[];
            if (outputItems == null) return string.Empty;

            foreach (object item in outputItems)
            {
                Dictionary<string, object> itemDict = item as Dictionary<string, object>;
                if (itemDict == null || !itemDict.ContainsKey("content")) continue;
                object[] contentItems = itemDict["content"] as object[];
                if (contentItems == null) continue;

                foreach (object content in contentItems)
                {
                    Dictionary<string, object> contentDict = content as Dictionary<string, object>;
                    if (contentDict == null || !contentDict.ContainsKey("text")) continue;
                    string text = Convert.ToString(contentDict["text"]);
                    if (!string.IsNullOrEmpty(text)) return text;
                }
            }

            return string.Empty;
        }

        private static string GetString(Dictionary<string, object> values, string key)
        {
            object value;
            return values != null && values.TryGetValue(key, out value) && value != null ? Convert.ToString(value) ?? string.Empty : string.Empty;
        }

        private static bool GetBool(Dictionary<string, object> values, string key)
        {
            object value;
            if (values == null || !values.TryGetValue(key, out value) || value == null) return false;
            if (value is bool) return (bool)value;
            bool parsed;
            return bool.TryParse(Convert.ToString(value), out parsed) && parsed;
        }

        private static string NormalizeConfidence(string value)
        {
            string confidence = (value ?? string.Empty).Trim().ToLowerInvariant();
            if (confidence == "high") return "High";
            if (confidence == "medium") return "Medium";
            if (confidence == "low") return "Low";
            return "No Match";
        }

        private static HsnSuggestion InvalidResponse()
        {
            return new HsnSuggestion
            {
                HsnCode = string.Empty,
                Category = string.Empty,
                Confidence = "No Match",
                Reason = "OpenAI returned an invalid HSN suggestion.",
                Source = "OpenAI",
                NeedsReview = true
            };
        }

        private static HsnSuggestion Unavailable(string reason)
        {
            return new HsnSuggestion
            {
                HsnCode = string.Empty,
                Category = string.Empty,
                Confidence = "No Match",
                Reason = reason,
                Source = "OpenAI",
                NeedsReview = true
            };
        }

        private static bool IsEnabled()
        {
            string value = ConfigurationManager.AppSettings["OpenAI.Enabled"];
            if (IsBlank(value)) return true;
            bool enabled;
            return bool.TryParse(value, out enabled) ? enabled : true;
        }

        private static string GetApiKey()
        {
            string env = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (!IsBlank(env)) return env.Trim();
            string configured = ConfigurationManager.AppSettings["OpenAI.ApiKey"];
            return IsBlank(configured) ? string.Empty : configured.Trim();
        }

        private static string GetModel()
        {
            string configured = ConfigurationManager.AppSettings["OpenAI.Model"];
            return IsBlank(configured) ? DefaultModel : configured.Trim();
        }

        private static bool IsBlank(string value)
        {
            return value == null || value.Trim().Length == 0;
        }
    }
}
