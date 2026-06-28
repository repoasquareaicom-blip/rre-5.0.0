using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Configuration;
using System.Threading;

namespace BranchSync
{
    class Program
    {
        static void Main(string[] args)
        {
            // Simple scheduler: run every 1 minute
            while (true)
            {
                try
                {
                    RunSyncOnce();
                }
                catch
                {
                    // ignore errors
                }
                Thread.Sleep(60000); // 1 minute
            }
        }

        static void RunSyncOnce()
        {
            try
            {
                string apiCursor = ConfigurationManager.AppSettings["PresenceCursorUrl"];
                string apiPush = ConfigurationManager.AppSettings["PresencePushUrl"];
                string branch = ConfigurationManager.AppSettings["BranchCode"];
                string apiKey = ConfigurationManager.AppSettings["ApiKey"];

                if (string.IsNullOrEmpty(apiCursor) || string.IsNullOrEmpty(apiPush) || string.IsNullOrEmpty(branch) || string.IsNullOrEmpty(apiKey))
                    return;

                string lastCursor = GetCursor(apiCursor, branch, apiKey);

                // TODO: fetch records from local DB based on lastCursor/Updatedon and post in batches
                // For now this program will just call the push endpoint with a placeholder payload

                string payload = "{\"last_synced_at\":\"2026-06-02T15:00:00\",\"records\":[]}";
                PostJson(apiPush, branch, apiKey, payload);
            }
            catch
            {
                // swallow
            }
        }

        static string GetCursor(string url, string branch, string apiKey)
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
            catch
            {
                return null;
            }
        }

        static void PostJson(string url, string branch, string apiKey, string json)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers["X-Branch-Code"] = branch;
                request.Headers["X-Api-Key"] = apiKey;

                using (Stream reqStream = request.GetRequestStream())
                using (StreamWriter sw = new StreamWriter(reqStream))
                {
                    sw.Write(json);
                }

                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    string respText = sr.ReadToEnd();
                }
            }
            catch
            {
                // ignore
            }
        }
    }
}
