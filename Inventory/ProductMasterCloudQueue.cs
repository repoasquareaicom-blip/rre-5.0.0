using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;

namespace Inventory
{
    public static class ProductMasterCloudQueue
    {
        public const string StatusPending = "Pending";
        public const string StatusSynced = "Synced";
        public const string StatusFailed = "Failed";
        private static readonly string[] TargetBranches = new string[] { "RR-NAMAKKAL", "RR-KOLATHUR" };

        public static void EnqueueAndTryPush(string productId, string changeType, bool showFailureMessage)
        {
            if (!BranchAccess.IsMainOffice || string.IsNullOrEmpty(productId))
            {
                return;
            }

            foreach (string targetBranchCode in TargetBranches)
            {
                try
                {
                    EnqueueProduct(productId, targetBranchCode, changeType);
                }
                catch (Exception ex)
                {
                    if (showFailureMessage)
                    {
                        MessageBox.Show("Unable to save product sync queue: " + ex.Message);
                    }
                    return;
                }
            }

            BeginPushPendingInBackground();
        }

        public static void EnqueueAndTryPushProducts(DataTable products, string changeType)
        {
            if (!BranchAccess.IsMainOffice || products == null || products.Rows.Count == 0)
            {
                return;
            }

            int queued = 0;

            foreach (DataRow row in products.Rows)
            {
                string productId = GetProductId(row, products);
                if (string.IsNullOrEmpty(productId))
                {
                    continue;
                }

                foreach (string targetBranchCode in TargetBranches)
                {
                    try
                    {
                        EnqueueProduct(productId, targetBranchCode, changeType);
                        queued++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to save product sync queue: " + ex.Message);
                        return;
                    }
                }
            }

            if (queued > 0)
            {
                BeginPushPendingInBackground();
            }
        }

        public static DataTable GetQueue(string status)
        {
            EnsureTable();

            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"
SELECT QueueId, ProductId, ItemCode, ItemName, SalesPrice, ChangeType, TargetBranchCode, Status, AttemptCount,
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
            string targetBranchCode = null;
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT ProductId, TargetBranchCode FROM ProductMasterCloudQueue WHERE QueueId = @queueId";
                cmd.Parameters.Add("@queueId", SqlDbType.Int).Value = queueId;
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        productId = Convert.ToString(reader["ProductId"]);
                        targetBranchCode = Convert.ToString(reader["TargetBranchCode"]);
                    }
                }
            }

            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(targetBranchCode))
            {
                return ProductCloudSyncResult.Failed("Queue item not found.");
            }

            return PushProduct(productId, targetBranchCode);
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

        public static ProductCloudSyncResult PushProduct(string productId, string targetBranchCode)
        {
            if (!BranchAccess.IsMainOffice)
            {
                return ProductCloudSyncResult.Failed(BranchAccess.MainOfficeOnlyMessage);
            }

            ProductCloudSyncResult result = ProductCloudSyncClient.TryPushProductById(productId, targetBranchCode);
            if (result.Success)
            {
                MarkSynced(productId, targetBranchCode);
            }
            else
            {
                MarkFailed(productId, targetBranchCode, result.ErrorMessage);
            }

            return result;
        }

        private static void EnqueueProduct(string productId, string targetBranchCode, string changeType)
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

IF EXISTS (SELECT 1 FROM ProductMasterCloudQueue WHERE ProductId = @ProductId AND TargetBranchCode = @TargetBranchCode)
BEGIN
    UPDATE ProductMasterCloudQueue
    SET ItemCode = @ItemCode,
        ItemName = @ItemName,
        SalesPrice = @SalesPrice,
        ChangeType = @ChangeType,
        Status = @PendingStatus,
        LastError = NULL,
        ModifiedOn = GETDATE()
    WHERE ProductId = @ProductId AND TargetBranchCode = @TargetBranchCode;
END
ELSE
BEGIN
    INSERT INTO ProductMasterCloudQueue
        (ProductId, TargetBranchCode, ItemCode, ItemName, SalesPrice, ChangeType, Status, AttemptCount, CreatedOn, ModifiedOn)
    VALUES
        (@ProductId, @TargetBranchCode, @ItemCode, @ItemName, @SalesPrice, @ChangeType, @PendingStatus, 0, GETDATE(), GETDATE());
END";

                cmd.Parameters.Add("@ProductId", SqlDbType.VarChar, 50).Value = productId;
                cmd.Parameters.Add("@TargetBranchCode", SqlDbType.VarChar, 30).Value = targetBranchCode;
                cmd.Parameters.Add("@ChangeType", SqlDbType.VarChar, 30).Value = string.IsNullOrEmpty(changeType) ? "Product" : changeType;
                cmd.Parameters.Add("@PendingStatus", SqlDbType.VarChar, 20).Value = StatusPending;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static void MarkSynced(string productId, string targetBranchCode)
        {
            UpdateStatus(productId, targetBranchCode, StatusSynced, null);
        }

        private static void MarkFailed(string productId, string targetBranchCode, string errorMessage)
        {
            UpdateStatus(productId, targetBranchCode, StatusFailed, errorMessage);
        }

        private static void UpdateStatus(string productId, string targetBranchCode, string status, string errorMessage)
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
WHERE ProductId = @ProductId AND TargetBranchCode = @TargetBranchCode";
                cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = status;
                cmd.Parameters.Add("@SyncedStatus", SqlDbType.VarChar, 20).Value = StatusSynced;
                cmd.Parameters.Add("@LastError", SqlDbType.VarChar, 1000).Value = string.IsNullOrEmpty(errorMessage) ? (object)DBNull.Value : errorMessage;
                cmd.Parameters.Add("@ProductId", SqlDbType.VarChar, 50).Value = productId;
                cmd.Parameters.Add("@TargetBranchCode", SqlDbType.VarChar, 30).Value = targetBranchCode;
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

        private static void BeginPushPendingInBackground()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    PushPending();
                }
                catch
                {
                    // The queue screen exposes retry and LastError for failed rows.
                }
            });
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
        TargetBranchCode varchar(30) NOT NULL,
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

            EnsureTargetBranchSchema();
        }

        private static void EnsureTargetBranchSchema()
        {
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"
IF OBJECT_ID('dbo.ProductMasterCloudQueue', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.ProductMasterCloudQueue', 'TargetBranchCode') IS NULL
    BEGIN
        DELETE FROM dbo.ProductMasterCloudQueue;
        ALTER TABLE dbo.ProductMasterCloudQueue ADD TargetBranchCode varchar(30) NULL;
        ALTER TABLE dbo.ProductMasterCloudQueue ALTER COLUMN TargetBranchCode varchar(30) NOT NULL;
    END;

    DECLARE @sql nvarchar(max);

    SELECT TOP 1 @sql = 'ALTER TABLE dbo.ProductMasterCloudQueue DROP CONSTRAINT [' + kc.name + ']'
    FROM sys.key_constraints kc
    INNER JOIN sys.indexes i ON kc.parent_object_id = i.object_id AND kc.unique_index_id = i.index_id
    INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE kc.parent_object_id = OBJECT_ID('dbo.ProductMasterCloudQueue')
      AND kc.type = 'UQ'
      AND c.name = 'ProductId'
      AND NOT EXISTS (
          SELECT 1
          FROM sys.index_columns ic2
          INNER JOIN sys.columns c2 ON ic2.object_id = c2.object_id AND ic2.column_id = c2.column_id
          WHERE ic2.object_id = i.object_id AND ic2.index_id = i.index_id AND c2.name <> 'ProductId'
      );

    IF @sql IS NOT NULL
        EXEC(@sql);

    SET @sql = NULL;

    SELECT TOP 1 @sql = 'DROP INDEX [' + i.name + '] ON dbo.ProductMasterCloudQueue'
    FROM sys.indexes i
    INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE i.object_id = OBJECT_ID('dbo.ProductMasterCloudQueue')
      AND i.is_unique = 1
      AND i.is_primary_key = 0
      AND i.is_unique_constraint = 0
      AND c.name = 'ProductId'
      AND NOT EXISTS (
          SELECT 1
          FROM sys.index_columns ic2
          INNER JOIN sys.columns c2 ON ic2.object_id = c2.object_id AND ic2.column_id = c2.column_id
          WHERE ic2.object_id = i.object_id AND ic2.index_id = i.index_id AND c2.name <> 'ProductId'
      );

    IF @sql IS NOT NULL
        EXEC(@sql);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.ProductMasterCloudQueue') AND name = 'UX_ProductMasterCloudQueue_Product_TargetBranch')
    BEGIN
        CREATE UNIQUE INDEX UX_ProductMasterCloudQueue_Product_TargetBranch
        ON dbo.ProductMasterCloudQueue(ProductId, TargetBranchCode);
    END;
END";
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
