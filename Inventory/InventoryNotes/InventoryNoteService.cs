using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Inventory.InventoryNotes
{
    internal sealed class InventoryNoteLine
    {
        public int MaterialId;
        public string ItemCode;
        public string ProductName;
        public string Brand;
        public string Size;
        public string UOM;
        public decimal AvailableStock;
        public decimal Quantity;
        public string Remarks;
    }

    internal sealed class InventoryNoteService
    {
        private readonly string connectionString;

        public InventoryNoteService()
        {
            connectionString = Program.connection;
        }

        public DataTable GetLocations()
        {
            const string sql = @"
SELECT LocationId, LocationCode, LocationName, LocationType,
       LocationName + CASE WHEN ISNULL(LocationCode,'') = '' THEN '' ELSE ' (' + LocationCode + ')' END AS DisplayName
FROM InventoryLocationMaster
WHERE ISNULL(IsActive, 1) = 1
ORDER BY LocationName";
            return ExecuteTable(sql, null);
        }

        public DataTable SearchProducts(string searchText, int locationId)
        {
            string like = "%" + (searchText == null ? "" : searchText.Trim()) + "%";
            const string sql = @"
SELECT TOP 75
       p.id,
       ISNULL(p.ItemCode, '') AS ItemCode,
       ISNULL(NULLIF(p.DisplayName, ''), p.ItemName) AS DisplayName,
       ISNULL(p.ItemName, '') AS ItemName,
       ISNULL(CONVERT(varchar(100), p.Brand), '') AS Brand,
       ISNULL(p.Size, '') AS Size,
       ISNULL(p.UOM, '') AS UOM,
       CAST(ISNULL(SUM(CASE WHEN mt.Type = 'IN' THEN mt.Quantity WHEN mt.Type = 'OUT' THEN -mt.Quantity ELSE 0 END), 0) AS decimal(18,3)) AS AvailableStock
FROM ProductMaster p
LEFT JOIN MaterialTransaction mt ON mt.MaterialId = p.id AND mt.LocationId = @LocationId
WHERE ISNULL(p.IsDeleted, 0) = 0
  AND (
      ISNULL(p.ItemCode, '') LIKE @Search
      OR ISNULL(p.DisplayName, '') LIKE @Search
      OR ISNULL(p.ItemName, '') LIKE @Search
      OR ISNULL(CONVERT(varchar(100), p.Brand), '') LIKE @Search
      OR ISNULL(p.BarCode, '') LIKE @Search
  )
GROUP BY p.id, p.ItemCode, p.DisplayName, p.ItemName, CONVERT(varchar(100), p.Brand), p.Size, p.UOM
ORDER BY ISNULL(NULLIF(p.DisplayName, ''), p.ItemName)";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@LocationId", locationId));
            parameters.Add(new SqlParameter("@Search", like));
            return ExecuteTable(sql, parameters);
        }

        public DataTable SearchNotes(string noteKind, DateTime fromDate, DateTime toDate, string noteNo, int fromLocationId, int toLocationId, string status)
        {
            string headerTable = HeaderTable(noteKind);
            string idColumn = HeaderIdColumn(noteKind);
            string noColumn = NoteNoColumn(noteKind);
            string dateColumn = NoteDateColumn(noteKind);
            string detailTable = DetailTable(noteKind);

            string sql = @"
SELECT h." + idColumn + @" AS NoteId,
       h." + noColumn + @" AS NoteNo,
       h." + dateColumn + @" AS NoteDate,
       fl.LocationName AS FromLocation,
       tl.LocationName AS ToLocation,
       COUNT(d." + DetailIdColumn(noteKind) + @") AS TotalItems,
       h.EnteredBy,
       h.EnteredOn,
       h.Status
FROM " + headerTable + @" h
INNER JOIN InventoryLocationMaster fl ON fl.LocationId = h.FromLocationId
INNER JOIN InventoryLocationMaster tl ON tl.LocationId = h.ToLocationId
LEFT JOIN " + detailTable + @" d ON d." + idColumn + @" = h." + idColumn + @"
WHERE ISNULL(h.IsDeleted, 0) = 0
  AND h." + dateColumn + @" >= @FromDate
  AND h." + dateColumn + @" < @ToDate
  AND (@NoteNo = '' OR h." + noColumn + @" LIKE @NoteNoLike)
  AND (@FromLocationId = 0 OR h.FromLocationId = @FromLocationId)
  AND (@ToLocationId = 0 OR h.ToLocationId = @ToLocationId)
  AND (@Status = 'ALL' OR h.Status = @Status)
GROUP BY h." + idColumn + @", h." + noColumn + @", h." + dateColumn + @", fl.LocationName, tl.LocationName, h.EnteredBy, h.EnteredOn, h.Status
ORDER BY h." + dateColumn + @" DESC, h." + idColumn + @" DESC";

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@FromDate", fromDate.Date));
            parameters.Add(new SqlParameter("@ToDate", toDate.Date.AddDays(1)));
            parameters.Add(new SqlParameter("@NoteNo", noteNo == null ? "" : noteNo.Trim()));
            parameters.Add(new SqlParameter("@NoteNoLike", "%" + (noteNo == null ? "" : noteNo.Trim()) + "%"));
            parameters.Add(new SqlParameter("@FromLocationId", fromLocationId));
            parameters.Add(new SqlParameter("@ToLocationId", toLocationId));
            parameters.Add(new SqlParameter("@Status", status == null || status.Trim() == "" ? "PENDING" : status.Trim().ToUpper()));
            return ExecuteTable(sql, parameters);
        }

        public DataTable GetNoteDetails(string noteKind, int noteId)
        {
            string detailTable = DetailTable(noteKind);
            string idColumn = HeaderIdColumn(noteKind);
            const string productSelect = @"
SELECT d.MaterialId,
       ISNULL(p.ItemCode, '') AS ItemCode,
       ISNULL(NULLIF(p.DisplayName, ''), p.ItemName) AS ProductName,
       ISNULL(CONVERT(varchar(100), p.Brand), '') AS Brand,
       ISNULL(p.Size, '') AS Size,
       ISNULL(p.UOM, '') AS UOM,
       d.Quantity,
       ISNULL(d.Remarks, '') AS Remarks
";
            string sql = productSelect + @"
FROM " + detailTable + @" d
INNER JOIN ProductMaster p ON p.id = d.MaterialId
WHERE d." + idColumn + @" = @NoteId
ORDER BY d." + DetailIdColumn(noteKind);
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@NoteId", noteId));
            return ExecuteTable(sql, parameters);
        }

        public DataRow GetNoteHeader(string noteKind, int noteId)
        {
            string sql = "SELECT * FROM " + HeaderTable(noteKind) + " WHERE " + HeaderIdColumn(noteKind) + " = @NoteId";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@NoteId", noteId));
            DataTable table = ExecuteTable(sql, parameters);
            return table.Rows.Count == 0 ? null : table.Rows[0];
        }

        public DataRow GetDeliveryNotePrintHeader(int deliveryNoteId)
        {
            const string sql = @"
SELECT dn.DeliveryNoteId,
       dn.DeliveryNoteNo,
       dn.DeliveryNoteDate,
       fl.LocationName AS FromLocation,
       tl.LocationName AS ToLocation,
       ISNULL(dn.ReferenceNo, '') AS ReferenceNo,
       dn.ReferenceDate,
       ISNULL(dn.Remarks, '') AS Remarks,
       dn.Status,
       ISNULL(dn.EnteredBy, '') AS EnteredBy,
       dn.EnteredOn,
       ISNULL(dn.ApprovedBy, '') AS ApprovedBy,
       dn.ApprovedOn
FROM DeliveryNote dn
INNER JOIN InventoryLocationMaster fl ON fl.LocationId = dn.FromLocationId
INNER JOIN InventoryLocationMaster tl ON tl.LocationId = dn.ToLocationId
WHERE dn.DeliveryNoteId = @DeliveryNoteId";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@DeliveryNoteId", deliveryNoteId));
            DataTable table = ExecuteTable(sql, parameters);
            return table.Rows.Count == 0 ? null : table.Rows[0];
        }

        public string SaveNote(string noteKind, DateTime noteDate, int fromLocationId, int toLocationId, string referenceNo, object referenceDate, string remarks, List<InventoryNoteLine> lines)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    string noteNo = GenerateNoteNo(connection, transaction, noteKind, noteDate);
                    string headerTable = HeaderTable(noteKind);
                    string idColumn = HeaderIdColumn(noteKind);
                    string noColumn = NoteNoColumn(noteKind);
                    string dateColumn = NoteDateColumn(noteKind);
                    string insertHeader = @"
INSERT INTO " + headerTable + @" (" + noColumn + @", " + dateColumn + @", FromLocationId, ToLocationId, ReferenceNo, ReferenceDate, Remarks, Status, EnteredBy, EnteredOn, IsDeleted)
VALUES (@NoteNo, @NoteDate, @FromLocationId, @ToLocationId, @ReferenceNo, @ReferenceDate, @Remarks, 'PENDING', @EnteredBy, GETDATE(), 0);
SELECT CAST(SCOPE_IDENTITY() AS int);";
                    int noteId;
                    using (SqlCommand cmd = new SqlCommand(insertHeader, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@NoteNo", noteNo);
                        cmd.Parameters.AddWithValue("@NoteDate", noteDate.Date);
                        cmd.Parameters.AddWithValue("@FromLocationId", fromLocationId);
                        cmd.Parameters.AddWithValue("@ToLocationId", toLocationId);
                        cmd.Parameters.AddWithValue("@ReferenceNo", NullIfEmpty(referenceNo));
                        cmd.Parameters.AddWithValue("@ReferenceDate", referenceDate == null ? DBNull.Value : referenceDate);
                        cmd.Parameters.AddWithValue("@Remarks", NullIfEmpty(remarks));
                        cmd.Parameters.AddWithValue("@EnteredBy", CurrentUser());
                        noteId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    foreach (InventoryNoteLine line in lines)
                    {
                        string insertDetail = @"
INSERT INTO " + DetailTable(noteKind) + @" (" + idColumn + @", MaterialId, Quantity, Remarks)
VALUES (@NoteId, @MaterialId, @Quantity, @Remarks)";
                        using (SqlCommand cmd = new SqlCommand(insertDetail, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@NoteId", noteId);
                            cmd.Parameters.AddWithValue("@MaterialId", line.MaterialId);
                            cmd.Parameters.AddWithValue("@Quantity", line.Quantity);
                            cmd.Parameters.AddWithValue("@Remarks", NullIfEmpty(line.Remarks));
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return noteNo;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void ApproveNote(string noteKind, int noteId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable);
                try
                {
                    DataRow header = LoadHeaderForUpdate(connection, transaction, noteKind, noteId);
                    if (header == null)
                        throw new ApplicationException("Selected document was not found.");
                    if (Convert.ToString(header["Status"]).ToUpper() != "PENDING")
                        throw new ApplicationException("Selected document is already " + Convert.ToString(header["Status"]) + ".");

                    string transactionType = noteKind == "DN" ? "DELIVERY NOTE" : "RECEIPT NOTE";
                    if (HasPostedLedger(connection, transaction, transactionType, noteId))
                        throw new ApplicationException("This document is already posted in MaterialTransaction.");

                    DataTable details = LoadDetailsForUpdate(connection, transaction, noteKind, noteId);
                    if (details.Rows.Count == 0)
                        throw new ApplicationException("Cannot approve a document without product details.");

                    int locationId = noteKind == "DN" ? Convert.ToInt32(header["FromLocationId"]) : Convert.ToInt32(header["ToLocationId"]);
                    DateTime transactionDate = Convert.ToDateTime(header[NoteDateColumn(noteKind)]).Date;

                    if (noteKind == "DN")
                    {
                        foreach (DataRow detail in details.Rows)
                        {
                            int materialId = Convert.ToInt32(detail["MaterialId"]);
                            decimal requested = Convert.ToDecimal(detail["Quantity"]);
                            decimal available = GetAvailableStock(connection, transaction, materialId, locationId);
                            if (requested > available)
                            {
                                string product = Convert.ToString(detail["ProductName"]);
                                throw new ApplicationException("Insufficient stock for " + product + "." + Environment.NewLine +
                                    "Available: " + available.ToString("0.###") + Environment.NewLine +
                                    "Requested: " + requested.ToString("0.###"));
                            }
                        }
                    }

                    foreach (DataRow detail in details.Rows)
                    {
                        InsertMaterialTransaction(connection, transaction, noteId, transactionType, transactionDate,
                            Convert.ToInt32(detail["MaterialId"]), Convert.ToDecimal(detail["Quantity"]), locationId,
                            noteKind == "DN" ? "OUT" : "IN");
                    }

                    string sql = "UPDATE " + HeaderTable(noteKind) + " SET Status = 'APPROVED', ApprovedBy = @User, ApprovedOn = GETDATE() WHERE " + HeaderIdColumn(noteKind) + " = @NoteId AND Status = 'PENDING'";
                    using (SqlCommand cmd = new SqlCommand(sql, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@User", CurrentUser());
                        cmd.Parameters.AddWithValue("@NoteId", noteId);
                        if (cmd.ExecuteNonQuery() != 1)
                            throw new ApplicationException("Approval failed because the document status changed.");
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void RejectNote(string noteKind, int noteId, string rejectionRemarks)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable);
                try
                {
                    DataRow header = LoadHeaderForUpdate(connection, transaction, noteKind, noteId);
                    if (header == null)
                        throw new ApplicationException("Selected document was not found.");
                    if (Convert.ToString(header["Status"]).ToUpper() != "PENDING")
                        throw new ApplicationException("Selected document is already " + Convert.ToString(header["Status"]) + ".");

                    string sql = "UPDATE " + HeaderTable(noteKind) + " SET Status = 'REJECTED', RejectedBy = @User, RejectedOn = GETDATE(), RejectionRemarks = @Remarks WHERE " + HeaderIdColumn(noteKind) + " = @NoteId AND Status = 'PENDING'";
                    using (SqlCommand cmd = new SqlCommand(sql, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@User", CurrentUser());
                        cmd.Parameters.AddWithValue("@Remarks", rejectionRemarks);
                        cmd.Parameters.AddWithValue("@NoteId", noteId);
                        if (cmd.ExecuteNonQuery() != 1)
                            throw new ApplicationException("Reject failed because the document status changed.");
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private DataRow LoadHeaderForUpdate(SqlConnection connection, SqlTransaction transaction, string noteKind, int noteId)
        {
            string sql = "SELECT * FROM " + HeaderTable(noteKind) + " WITH (UPDLOCK, HOLDLOCK) WHERE " + HeaderIdColumn(noteKind) + " = @NoteId AND ISNULL(IsDeleted, 0) = 0";
            DataTable table = ExecuteTable(connection, transaction, sql, new SqlParameter("@NoteId", noteId));
            return table.Rows.Count == 0 ? null : table.Rows[0];
        }

        private DataTable LoadDetailsForUpdate(SqlConnection connection, SqlTransaction transaction, string noteKind, int noteId)
        {
            string sql = @"
SELECT d.MaterialId,
       d.Quantity,
       ISNULL(NULLIF(p.DisplayName, ''), p.ItemName) AS ProductName
FROM " + DetailTable(noteKind) + @" d WITH (UPDLOCK, HOLDLOCK)
INNER JOIN ProductMaster p ON p.id = d.MaterialId
WHERE d." + HeaderIdColumn(noteKind) + " = @NoteId";
            return ExecuteTable(connection, transaction, sql, new SqlParameter("@NoteId", noteId));
        }

        private bool HasPostedLedger(SqlConnection connection, SqlTransaction transaction, string transactionType, int noteId)
        {
            const string sql = "SELECT COUNT(1) FROM MaterialTransaction WITH (UPDLOCK, HOLDLOCK) WHERE TransactionType = @TransactionType AND TransId = @TransId";
            using (SqlCommand cmd = new SqlCommand(sql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@TransactionType", transactionType);
                cmd.Parameters.AddWithValue("@TransId", noteId);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private decimal GetAvailableStock(SqlConnection connection, SqlTransaction transaction, int materialId, int locationId)
        {
            const string sql = @"
SELECT CAST(ISNULL(SUM(CASE WHEN Type = 'IN' THEN Quantity WHEN Type = 'OUT' THEN -Quantity ELSE 0 END), 0) AS decimal(18,3))
FROM MaterialTransaction WITH (UPDLOCK, HOLDLOCK)
WHERE MaterialId = @MaterialId AND LocationId = @LocationId";
            using (SqlCommand cmd = new SqlCommand(sql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@MaterialId", materialId);
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                object value = cmd.ExecuteScalar();
                return value == DBNull.Value ? 0 : Convert.ToDecimal(value);
            }
        }

        private void InsertMaterialTransaction(SqlConnection connection, SqlTransaction transaction, int transId, string transactionType, DateTime transactionDate, int materialId, decimal quantity, int locationId, string type)
        {
            const string sql = @"
INSERT INTO MaterialTransaction (TransId, TransactionType, TransactionDate, MaterialId, Quantity, LocationId, Type, Updatedby)
VALUES (@TransId, @TransactionType, @TransactionDate, @MaterialId, @Quantity, @LocationId, @Type, @Updatedby)";
            using (SqlCommand cmd = new SqlCommand(sql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@TransId", transId);
                cmd.Parameters.AddWithValue("@TransactionType", transactionType);
                cmd.Parameters.AddWithValue("@TransactionDate", transactionDate);
                cmd.Parameters.AddWithValue("@MaterialId", materialId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Updatedby", CurrentUser());
                cmd.ExecuteNonQuery();
            }
        }

        private string GenerateNoteNo(SqlConnection connection, SqlTransaction transaction, string noteKind, DateTime noteDate)
        {
            string prefix = noteKind == "DN" ? "DN" : "RN";
            string fy = FinancialYear(noteDate);
            string noColumn = NoteNoColumn(noteKind);
            string sql = "SELECT ISNULL(MAX(CAST(RIGHT(" + noColumn + ", 6) AS int)), 0) + 1 FROM " + HeaderTable(noteKind) + " WITH (UPDLOCK, HOLDLOCK) WHERE " + noColumn + " LIKE @Prefix";
            using (SqlCommand cmd = new SqlCommand(sql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Prefix", prefix + "/" + fy + "/%");
                int next = Convert.ToInt32(cmd.ExecuteScalar());
                return prefix + "/" + fy + "/" + next.ToString("000000");
            }
        }

        private static string FinancialYear(DateTime date)
        {
            int startYear = date.Month >= 4 ? date.Year : date.Year - 1;
            int endYear = startYear + 1;
            return (startYear % 100).ToString("00") + "-" + (endYear % 100).ToString("00");
        }

        private DataTable ExecuteTable(string sql, List<SqlParameter> parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                if (parameters != null)
                {
                    foreach (SqlParameter parameter in parameters)
                        cmd.Parameters.Add(parameter);
                }
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        private DataTable ExecuteTable(SqlConnection connection, SqlTransaction transaction, string sql, SqlParameter parameter)
        {
            using (SqlCommand cmd = new SqlCommand(sql, connection, transaction))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(parameter);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        private static object NullIfEmpty(string value)
        {
            if (value == null || value.Trim() == "")
                return DBNull.Value;
            return value.Trim();
        }

        private static string CurrentUser()
        {
            if (!string.IsNullOrEmpty(Program.UserName))
                return Program.UserName;
            if (!string.IsNullOrEmpty(Program.userid))
                return Program.userid;
            return Environment.UserName;
        }

        private static string HeaderTable(string noteKind)
        {
            return noteKind == "DN" ? "DeliveryNote" : "ReceiptNote";
        }

        private static string DetailTable(string noteKind)
        {
            return noteKind == "DN" ? "DeliveryNoteDetail" : "ReceiptNoteDetail";
        }

        private static string HeaderIdColumn(string noteKind)
        {
            return noteKind == "DN" ? "DeliveryNoteId" : "ReceiptNoteId";
        }

        private static string DetailIdColumn(string noteKind)
        {
            return noteKind == "DN" ? "DeliveryNoteDetailId" : "ReceiptNoteDetailId";
        }

        private static string NoteNoColumn(string noteKind)
        {
            return noteKind == "DN" ? "DeliveryNoteNo" : "ReceiptNoteNo";
        }

        private static string NoteDateColumn(string noteKind)
        {
            return noteKind == "DN" ? "DeliveryNoteDate" : "ReceiptNoteDate";
        }
    }
}
