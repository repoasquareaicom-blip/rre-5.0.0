using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Inventory
{
    public partial class BranchStockWidget : UserControl
    {
        private const int ApiTimeoutMilliseconds = 5000;
        private readonly Dictionary<string, int> branchRows = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public BranchStockWidget()
        {
            InitializeComponent();
            Load += new EventHandler(BranchStockWidget_Load);
        }

        public event EventHandler CloseRequested;

        private void BranchStockWidget_Load(object sender, EventArgs e)
        {
            LoadProducts();
            ResetResults();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (CloseRequested != null)
            {
                CloseRequested(this, EventArgs.Empty);
            }
        }

        private void btnGetStock_Click(object sender, EventArgs e)
        {
            int productId;
            string itemName;
            if (!TryGetSelectedProduct(out productId, out itemName))
            {
                MessageBox.Show("Please select a product.", "Branch Stock", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbProduct.Focus();
                return;
            }

            btnGetStock.Enabled = false;
            cmbProduct.Enabled = false;
            lblDetails.Text = "";
            ResetResults();

            List<BranchEndpoint> branches = GetOrderedBranches();
            foreach (BranchEndpoint branch in branches)
            {
                AddBranchRow(branch.BranchCode, "-", branch.IsCurrent ? "Loading..." : "Loading...", "");
            }

            foreach (BranchEndpoint branch in branches)
            {
                if (branch.IsCurrent)
                {
                    BranchStockResult localResult = GetLocalStock(branch.BranchCode, productId, itemName);
                    UpdateBranchRow(localResult);
                    continue;
                }

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(remoteWorker_DoWork);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(remoteWorker_RunWorkerCompleted);
                worker.RunWorkerAsync(new BranchStockRequest(branch, productId, itemName));
            }

            if (branches.Count <= 1)
            {
                btnGetStock.Enabled = true;
                cmbProduct.Enabled = true;
            }
        }

        private void remoteWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BranchStockRequest request = (BranchStockRequest)e.Argument;
            e.Result = GetRemoteStock(request);
        }

        private void remoteWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BranchStockResult result = e.Result as BranchStockResult;
            if (e.Error != null)
            {
                Trace.WriteLine(e.Error.ToString());
                result = BranchStockResult.Offline("Remote Branch", "Unable to connect", e.Error.ToString());
            }

            if (result != null)
            {
                UpdateBranchRow(result);
            }

            if (!HasLoadingRows())
            {
                btnGetStock.Enabled = true;
                cmbProduct.Enabled = true;
            }
        }

        private void LoadProducts()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("ItemName", typeof(string));
            dt.Rows.Add(0, "-Select-");

            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("SELECT id AS ProductId, ItemName FROM ProductMaster WHERE ISNULL(ItemName, '') <> '' ORDER BY ItemName", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                con.Open();
                da.Fill(dt);
            }

            cmbProduct.DataSource = dt;
            cmbProduct.DisplayMember = "ItemName";
            cmbProduct.ValueMember = "ProductId";
        }

        private bool TryGetSelectedProduct(out int productId, out string itemName)
        {
            productId = 0;
            itemName = string.Empty;

            DataRowView row = cmbProduct.SelectedItem as DataRowView;
            if (row == null)
            {
                return false;
            }

            productId = Convert.ToInt32(row["ProductId"]);
            itemName = Convert.ToString(row["ItemName"]);
            return productId > 0 && !string.IsNullOrEmpty(itemName) && itemName != "-Select-";
        }

        private List<BranchEndpoint> GetOrderedBranches()
        {
            string currentBranch = ConfigurationManager.AppSettings["BranchCode"];
            if (string.IsNullOrEmpty(currentBranch))
            {
                currentBranch = "BRANCH NOT SET";
            }
            currentBranch = currentBranch.Trim();

            List<BranchEndpoint> endpoints = new List<BranchEndpoint>();
            endpoints.Add(new BranchEndpoint(currentBranch, null, true));

            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                if (key == null || !key.StartsWith("BranchApi_", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string branchCode = key.Substring("BranchApi_".Length).Trim();
                if (string.IsNullOrEmpty(branchCode) || string.Equals(branchCode, currentBranch, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string url = ConfigurationManager.AppSettings[key];
                endpoints.Add(new BranchEndpoint(branchCode, url, false));
            }

            return endpoints;
        }

        private BranchStockResult GetLocalStock(string branchCode, int productId, string itemName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Program.connection))
                using (SqlCommand cmd = new SqlCommand("GetAvailableStockByProductId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    cmd.Parameters.Add("@ItemName", SqlDbType.VarChar, 500).Value = itemName;

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        con.Open();
                        da.Fill(dt);
                    }

                    if (dt.Rows.Count == 0)
                    {
                        return BranchStockResult.Error(branchCode, "No result returned from local stock procedure", null);
                    }

                    DataRow row = dt.Rows[0];
                    string returnCode = GetString(row, "ReturnCode");
                    string message = GetString(row, "Message");
                    decimal? stock = GetDecimal(row, "AvailableStock");

                    if (string.IsNullOrEmpty(returnCode) || string.Equals(returnCode, "YES", StringComparison.OrdinalIgnoreCase))
                    {
                        return BranchStockResult.Local(branchCode, stock, NullIfEmpty(message));
                    }

                    return BranchStockResult.Error(branchCode, NullIfEmpty(message), null);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return BranchStockResult.Error(branchCode, "Local stock check failed", ex.ToString());
            }
        }

        private BranchStockResult GetRemoteStock(BranchStockRequest request)
        {
            if (string.IsNullOrEmpty(request.Branch.Url))
            {
                return BranchStockResult.Offline(request.Branch.BranchCode, "Branch API URL not configured", null);
            }

            try
            {
                Program.ConfigureApiSecurity();
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                UriBuilder uriBuilder = new UriBuilder(request.Branch.Url);
                uriBuilder.Path = "api/stock/available";
                System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
                query["productId"] = Convert.ToString(request.ProductId);
                query["itemName"] = request.ItemName;
                uriBuilder.Query = query.ToString();

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
                webRequest.Method = "GET";
                webRequest.Accept = "application/json";
                webRequest.Timeout = ApiTimeoutMilliseconds;
                webRequest.ReadWriteTimeout = ApiTimeoutMilliseconds;

                using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    ApiStockResponse apiResponse = serializer.Deserialize<ApiStockResponse>(json);
                    if (apiResponse == null)
                    {
                        return BranchStockResult.Offline(request.Branch.BranchCode, "Invalid API response", json);
                    }

                    string responseBranch = string.IsNullOrEmpty(apiResponse.branchCode) ? request.Branch.BranchCode : apiResponse.branchCode;
                    if (string.Equals(apiResponse.returnCode, "YES", StringComparison.OrdinalIgnoreCase))
                    {
                        return BranchStockResult.Online(responseBranch, apiResponse.availableStock, NullIfEmpty(apiResponse.message));
                    }

                    return BranchStockResult.Error(responseBranch, NullIfEmpty(apiResponse.message), json);
                }
            }
            catch (WebException ex)
            {
                string detail = BuildTechnicalDetail(ex);
                Trace.WriteLine(detail);
                return BranchStockResult.Offline(request.Branch.BranchCode, ShortWebError(ex), detail);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return BranchStockResult.Offline(request.Branch.BranchCode, "Unable to connect", ex.ToString());
            }
        }

        private void ResetResults()
        {
            dgvBranchStock.Rows.Clear();
            branchRows.Clear();
            lblDetails.Text = "";
        }

        private void AddBranchRow(string branchCode, string stock, string status, string detail)
        {
            int rowIndex = dgvBranchStock.Rows.Add(branchCode, stock, status, detail);
            branchRows[branchCode] = rowIndex;
            StyleRow(rowIndex, status);
        }

        private void UpdateBranchRow(BranchStockResult result)
        {
            if (!branchRows.ContainsKey(result.BranchCode))
            {
                AddBranchRow(result.BranchCode, FormatStock(result.AvailableStock), result.Status, result.Detail);
            }
            else
            {
                int rowIndex = branchRows[result.BranchCode];
                DataGridViewRow row = dgvBranchStock.Rows[rowIndex];
                row.Cells["colStock"].Value = FormatStock(result.AvailableStock);
                row.Cells["colStatus"].Value = result.Status;
                row.Cells["colDetail"].Value = result.Detail;
                StyleRow(rowIndex, result.Status);
            }

            UpdateDetails();
        }

        private void StyleRow(int rowIndex, string status)
        {
            DataGridViewRow row = dgvBranchStock.Rows[rowIndex];
            Color foreColor = Color.FromArgb(30, 41, 59);

            if (string.Equals(status, "Local", StringComparison.OrdinalIgnoreCase))
            {
                foreColor = Color.FromArgb(12, 74, 110);
            }
            else if (string.Equals(status, "Online", StringComparison.OrdinalIgnoreCase))
            {
                foreColor = Color.Green;
            }
            else if (string.Equals(status, "Offline", StringComparison.OrdinalIgnoreCase) || string.Equals(status, "Error", StringComparison.OrdinalIgnoreCase))
            {
                foreColor = Color.Firebrick;
            }

            row.DefaultCellStyle.ForeColor = foreColor;
        }

        private void UpdateDetails()
        {
            List<string> details = new List<string>();
            foreach (DataGridViewRow row in dgvBranchStock.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                string branch = Convert.ToString(row.Cells["colBranch"].Value);
                string detail = Convert.ToString(row.Cells["colDetail"].Value);
                if (!string.IsNullOrEmpty(detail))
                {
                    details.Add(branch + " - " + detail);
                }
            }

            lblDetails.Text = details.Count == 0 ? "" : "Details: " + string.Join(" | ", details.ToArray());
        }

        private bool HasLoadingRows()
        {
            foreach (DataGridViewRow row in dgvBranchStock.Rows)
            {
                if (!row.IsNewRow && string.Equals(Convert.ToString(row.Cells["colStatus"].Value), "Loading...", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private string FormatStock(decimal? stock)
        {
            return stock.HasValue ? stock.Value.ToString("0.000") : "-";
        }

        private string GetString(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
            {
                return Convert.ToString(row[columnName]);
            }

            return string.Empty;
        }

        private decimal? GetDecimal(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
            {
                return Convert.ToDecimal(row[columnName]);
            }

            return null;
        }

        private string NullIfEmpty(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : value;
        }

        private string ShortWebError(WebException ex)
        {
            if (ex.Status == WebExceptionStatus.Timeout)
            {
                return "Request timed out";
            }

            if (ex.Status == WebExceptionStatus.NameResolutionFailure)
            {
                return "DNS lookup failed";
            }

            return "Unable to connect";
        }

        private string BuildTechnicalDetail(WebException ex)
        {
            StringBuilder detail = new StringBuilder();
            detail.AppendLine("WebException Status: " + ex.Status);
            detail.AppendLine("SecurityProtocol: " + ServicePointManager.SecurityProtocol);

            HttpWebResponse httpResponse = ex.Response as HttpWebResponse;
            if (httpResponse != null)
            {
                detail.AppendLine("HTTP Status: " + ((int)httpResponse.StatusCode) + " " + httpResponse.StatusDescription);
            }

            detail.AppendLine(ex.ToString());
            return detail.ToString();
        }

        private class BranchEndpoint
        {
            public readonly string BranchCode;
            public readonly string Url;
            public readonly bool IsCurrent;

            public BranchEndpoint(string branchCode, string url, bool isCurrent)
            {
                BranchCode = branchCode;
                Url = url;
                IsCurrent = isCurrent;
            }
        }

        private class BranchStockRequest
        {
            public readonly BranchEndpoint Branch;
            public readonly int ProductId;
            public readonly string ItemName;

            public BranchStockRequest(BranchEndpoint branch, int productId, string itemName)
            {
                Branch = branch;
                ProductId = productId;
                ItemName = itemName;
            }
        }

        private class BranchStockResult
        {
            public string BranchCode;
            public decimal? AvailableStock;
            public string Status;
            public string Detail;

            public static BranchStockResult Local(string branchCode, decimal? stock, string detail)
            {
                return Create(branchCode, stock, "Local", detail);
            }

            public static BranchStockResult Online(string branchCode, decimal? stock, string detail)
            {
                return Create(branchCode, stock, "Online", detail);
            }

            public static BranchStockResult Offline(string branchCode, string detail, string technicalDetail)
            {
                if (!string.IsNullOrEmpty(technicalDetail))
                {
                    Trace.WriteLine(technicalDetail);
                }
                return Create(branchCode, null, "Offline", detail);
            }

            public static BranchStockResult Error(string branchCode, string detail, string technicalDetail)
            {
                if (!string.IsNullOrEmpty(technicalDetail))
                {
                    Trace.WriteLine(technicalDetail);
                }
                return Create(branchCode, null, "Error", detail);
            }

            private static BranchStockResult Create(string branchCode, decimal? stock, string status, string detail)
            {
                BranchStockResult result = new BranchStockResult();
                result.BranchCode = branchCode;
                result.AvailableStock = stock;
                result.Status = status;
                result.Detail = detail;
                return result;
            }
        }

        private class ApiStockResponse
        {
            public string branchCode { get; set; }
            public string returnCode { get; set; }
            public string message { get; set; }
            public int? productId { get; set; }
            public decimal? availableStock { get; set; }
        }
    }
}
