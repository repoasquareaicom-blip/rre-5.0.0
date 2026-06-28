using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace Inventory
{
    public static class ProductMasterCloudQueue
    {
        public const string StatusPending = "Pending";
        public const string StatusSynced = "Synced";
        public const string StatusFailed = "Failed";

        public static void EnqueueAndTryPush(string productId, string changeType, bool showFailureMessage)
        {
            if (!BranchAccess.IsMainOffice || string.IsNullOrEmpty(productId))
            {
                return;
            }

            ProductCloudSyncResult result;
            try
            {
                EnqueueProduct(productId, changeType);
                result = PushProduct(productId);
            }
            catch (Exception ex)
            {
                result = ProductCloudSyncResult.Failed("Unable to save product sync queue: " + ex.Message);
            }

            if (!result.Success && showFailureMessage)
            {
                MessageBox.Show(
                    "Product saved successfully in Salem." + Environment.NewLine +
                    "Cloud sync failed, so this product is pending for branch update." + Environment.NewLine +
                    "Please push it from Tools > Product Sync Queue when internet/API is available." + Environment.NewLine +
                    Environment.NewLine + "Error: " + result.ErrorMessage);
            }
        }

        public static void EnqueueAndTryPushProducts(DataTable products, string changeType)
        {
            if (!BranchAccess.IsMainOffice || products == null || products.Rows.Count == 0)
            {
                return;
            }

            int failed = 0;
            StringBuilder errors = new StringBuilder();

            foreach (DataRow row in products.Rows)
            {
                string productId = GetProductId(row, products);
                if (string.IsNullOrEmpty(productId))
                {
                    continue;
                }

                ProductCloudSyncResult result;
                try
                {
                    EnqueueProduct(productId, changeType);
                    result = PushProduct(productId);
                }
                catch (Exception ex)
                {
                    result = ProductCloudSyncResult.Failed("Unable to save product sync queue: " + ex.Message);
                }

                if (!result.Success)
                {
                    failed++;
                    if (errors.Length < 500)
                    {
                        errors.AppendLine(productId + ": " + result.ErrorMessage);
                    }
                }
            }

            if (failed > 0)
            {
                MessageBox.Show(
                    "Price changes were saved successfully in Salem." + Environment.NewLine +
                    failed + " product(s) are pending for branch update." + Environment.NewLine +
                    "Please push them from Tools > Product Sync Queue when internet/API is available." + Environment.NewLine +
                    Environment.NewLine + errors.ToString());
            }
        }

        public static DataTable GetQueue(string status)
        {
            EnsureTable();

            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"
SELECT QueueId, ProductId, ItemCode, ItemName, SalesPrice, ChangeType, Status, AttemptCount,
       LastError, CreatedOn, ModifiedOn, LastTriedOn, SyncedOn
FROM ProductMasterCloudQueue
WHERE (@status = 'All' OR Status = @status)
ORDER BY CASE WHEN Status = 'Pending' THEN 0 WHEN Status = 'Failed' THEN 1 ELSE 2 END,
         ModifiedOn DESC";
                cmd.Parameters.Add("@status", SqlDbType.VarChar, 20).Value = string.IsNullOrEmpty(status) ? "All" : status;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public static ProductCloudSyncResult PushQueueId(int queueId)
        {
            EnsureTable();

            string productId = null;
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT ProductId FROM ProductMasterCloudQueue WHERE QueueId = @queueId";
                cmd.Parameters.Add("@queueId", SqlDbType.Int).Value = queueId;
                con.Open();
                object value = cmd.ExecuteScalar();
                if (value != null && value != DBNull.Value)
                {
                    productId = Convert.ToString(value);
                }
            }

            if (string.IsNullOrEmpty(productId))
            {
                return ProductCloudSyncResult.Failed("Queue item not found.");
            }

            return PushProduct(productId);
        }

        public static int PushPending()
        {
            EnsureTable();

            DataTable pending = GetQueue("All");
            int success = 0;

            foreach (DataRow row in pending.Rows)
            {
                string status = Convert.ToString(row["Status"]);
                if (status == StatusSynced)
                {
                    continue;
                }

                int queueId = Convert.ToInt32(row["QueueId"]);
                ProductCloudSyncResult result = PushQueueId(queueId);
                if (result.Success)
                {
                    success++;
                }
            }

            return success;
        }

        public static ProductCloudSyncResult PushProduct(string productId)
        {
            if (!BranchAccess.IsMainOffice)
            {
                return ProductCloudSyncResult.Failed(BranchAccess.MainOfficeOnlyMessage);
            }

            ProductCloudSyncResult result = ProductCloudSyncClient.TryPushProductById(productId);
            if (result.Success)
            {
                MarkSynced(productId);
            }
            else
            {
                MarkFailed(productId, result.ErrorMessage);
            }

            return result;
        }

        private static void EnqueueProduct(string productId, string changeType)
        {
            EnsureTable();

            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"
DECLARE @ItemCode varchar(100), @ItemName varchar(255), @SalesPrice decimal(18,2), @SalesPriceText varchar(50);

SELECT @ItemCode = CONVERT(varchar(100), ItemCode),
       @ItemName = CONVERT(varchar(255), ItemName),
       @SalesPriceText = CONVERT(varchar(50), SalesPrice)
FROM ProductMaster
WHERE id = @ProductId;

IF ISNUMERIC(@SalesPriceText) = 1
    SET @SalesPrice = CONVERT(decimal(18,2), @SalesPriceText);

IF EXISTS (SELECT 1 FROM ProductMasterCloudQueue WHERE ProductId = @ProductId)
BEGIN
    UPDATE ProductMasterCloudQueue
    SET ItemCode = @ItemCode,
        ItemName = @ItemName,
        SalesPrice = @SalesPrice,
        ChangeType = @ChangeType,
        Status = @PendingStatus,
        LastError = NULL,
        ModifiedOn = GETDATE()
    WHERE ProductId = @ProductId;
END
ELSE
BEGIN
    INSERT INTO ProductMasterCloudQueue
        (ProductId, ItemCode, ItemName, SalesPrice, ChangeType, Status, AttemptCount, CreatedOn, ModifiedOn)
    VALUES
        (@ProductId, @ItemCode, @ItemName, @SalesPrice, @ChangeType, @PendingStatus, 0, GETDATE(), GETDATE());
END";

                cmd.Parameters.Add("@ProductId", SqlDbType.VarChar, 50).Value = productId;
                cmd.Parameters.Add("@ChangeType", SqlDbType.VarChar, 30).Value = string.IsNullOrEmpty(changeType) ? "Product" : changeType;
                cmd.Parameters.Add("@PendingStatus", SqlDbType.VarChar, 20).Value = StatusPending;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static void MarkSynced(string productId)
        {
            UpdateStatus(productId, StatusSynced, null);
        }

        private static void MarkFailed(string productId, string errorMessage)
        {
            UpdateStatus(productId, StatusFailed, errorMessage);
        }

        private static void UpdateStatus(string productId, string status, string errorMessage)
        {
            EnsureTable();

            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE ProductMasterCloudQueue
SET Status = @Status,
    AttemptCount = AttemptCount + 1,
    LastError = @LastError,
    LastTriedOn = GETDATE(),
    SyncedOn = CASE WHEN @Status = @SyncedStatus THEN GETDATE() ELSE SyncedOn END
WHERE ProductId = @ProductId";
                cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = status;
                cmd.Parameters.Add("@SyncedStatus", SqlDbType.VarChar, 20).Value = StatusSynced;
                cmd.Parameters.Add("@LastError", SqlDbType.VarChar, 1000).Value = string.IsNullOrEmpty(errorMessage) ? (object)DBNull.Value : errorMessage;
                cmd.Parameters.Add("@ProductId", SqlDbType.VarChar, 50).Value = productId;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static string GetProductId(DataRow row, DataTable table)
        {
            if (table.Columns.Contains("Id"))
            {
                return Convert.ToString(row["Id"]);
            }

            if (table.Columns.Contains("id"))
            {
                return Convert.ToString(row["id"]);
            }

            if (table.Columns.Contains("ProductId"))
            {
                return Convert.ToString(row["ProductId"]);
            }

            return string.Empty;
        }

        private static void EnsureTable()
        {
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"
IF OBJECT_ID('dbo.ProductMasterCloudQueue', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductMasterCloudQueue
    (
        QueueId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ProductId varchar(50) NOT NULL UNIQUE,
        ItemCode varchar(100) NULL,
        ItemName varchar(255) NULL,
        SalesPrice decimal(18,2) NULL,
        ChangeType varchar(30) NOT NULL,
        Status varchar(20) NOT NULL,
        AttemptCount int NOT NULL CONSTRAINT DF_ProductMasterCloudQueue_AttemptCount DEFAULT(0),
        LastError varchar(1000) NULL,
        CreatedOn datetime NOT NULL CONSTRAINT DF_ProductMasterCloudQueue_CreatedOn DEFAULT(GETDATE()),
        ModifiedOn datetime NOT NULL CONSTRAINT DF_ProductMasterCloudQueue_ModifiedOn DEFAULT(GETDATE()),
        LastTriedOn datetime NULL,
        SyncedOn datetime NULL
    );
END";
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
