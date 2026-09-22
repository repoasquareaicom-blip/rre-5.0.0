using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Inventory.Purchase
{
    public class PurchaseReceiptRepository
    {
        private readonly string connectionString = Program.connection;

        public DataTable SearchPurchaseOrders(string orderNumber)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP (100)
    poh.PurchaseId,
    poh.OrderNumber,
    poh.OrderDate,
    poh.VendorId,
    ISNULL(s.Name, '') AS VendorName,
    ISNULL(poh.Status, '') AS Status,
    ISNULL(poh.Remarks, '') AS Remarks
FROM dbo.PurchaseOrderHeader poh
LEFT JOIN dbo.Suppliers s
    ON s.SuppliersID = poh.VendorId
WHERE ISNULL(poh.IsDeleted, 0) = 0
  AND ISNULL(poh.Status, '') = 'Approve'
  AND (@OrderNumber = '' OR poh.OrderNumber LIKE '%' + @OrderNumber + '%')
  AND EXISTS
  (
      SELECT 1
      FROM dbo.PurchaseOrderDetails pod
      WHERE pod.PurchaseId = poh.PurchaseId
        AND ISNULL(pod.Quantity, 0) >
            ISNULL((
                SELECT SUM(prd.ReceivedQuantity)
                FROM dbo.PurchaseReceiptHeader prh
                INNER JOIN dbo.PurchaseReceiptDetails prd
                    ON prd.PurchaseId = prh.PurchaseId
                WHERE ISNULL(prh.IsDeleted, 0) = 0
                  AND prh.PurchaseOrderId = poh.PurchaseId
                  AND prd.ProductId = pod.Productid
            ), 0)
  )
ORDER BY poh.OrderDate DESC, poh.PurchaseId DESC", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@OrderNumber", SqlDbType.VarChar, 50).Value = orderNumber == null ? "" : orderNumber;
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        public PurchaseOrderReceiptData GetPurchaseOrder(string orderNumber)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP (1)
    poh.PurchaseId,
    poh.OrderNumber,
    poh.OrderDate,
    poh.VendorId,
    ISNULL(s.Name, '') AS VendorName,
    ISNULL(poh.Remarks, '') AS Remarks
FROM dbo.PurchaseOrderHeader poh
LEFT JOIN dbo.Suppliers s
    ON s.SuppliersID = poh.VendorId
WHERE ISNULL(poh.IsDeleted, 0) = 0
  AND poh.OrderNumber = @OrderNumber;

SELECT
    pod.Productid AS ProductId,
    ISNULL(pm.ItemName, '') AS ProductName,
    CAST(ISNULL(pod.Quantity, 0) AS DECIMAL(18,3)) AS OrderedQty,
    CAST(ISNULL((
        SELECT SUM(prd.ReceivedQuantity)
        FROM dbo.PurchaseReceiptHeader prh
        INNER JOIN dbo.PurchaseReceiptDetails prd
            ON prd.PurchaseId = prh.PurchaseId
        WHERE ISNULL(prh.IsDeleted, 0) = 0
          AND prh.PurchaseOrderId = pod.PurchaseId
          AND prd.ProductId = pod.Productid
    ), 0) AS DECIMAL(18,3)) AS AlreadyReceivedQty,
    CAST(ISNULL(pod.Quantity, 0) - ISNULL((
        SELECT SUM(prd.ReceivedQuantity)
        FROM dbo.PurchaseReceiptHeader prh
        INNER JOIN dbo.PurchaseReceiptDetails prd
            ON prd.PurchaseId = prh.PurchaseId
        WHERE ISNULL(prh.IsDeleted, 0) = 0
          AND prh.PurchaseOrderId = pod.PurchaseId
          AND prd.ProductId = pod.Productid
    ), 0) AS DECIMAL(18,3)) AS RemainingQty
FROM dbo.PurchaseOrderDetails pod
LEFT JOIN dbo.ProductMaster pm
    ON pm.id = pod.Productid
INNER JOIN dbo.PurchaseOrderHeader poh
    ON poh.PurchaseId = pod.PurchaseId
WHERE poh.OrderNumber = @OrderNumber
  AND ISNULL(pod.Quantity, 0) > ISNULL((
        SELECT SUM(prd.ReceivedQuantity)
        FROM dbo.PurchaseReceiptHeader prh
        INNER JOIN dbo.PurchaseReceiptDetails prd
            ON prd.PurchaseId = prh.PurchaseId
        WHERE ISNULL(prh.IsDeleted, 0) = 0
          AND prh.PurchaseOrderId = pod.PurchaseId
          AND prd.ProductId = pod.Productid
    ), 0)
ORDER BY ISNULL(pm.ItemName, ''), pod.Productid;", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@OrderNumber", SqlDbType.VarChar, 50).Value = orderNumber;
                DataSet ds = new DataSet();
                ad.Fill(ds);

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    throw new ApplicationException("Purchase order not found.");
                }

                DataRow header = ds.Tables[0].Rows[0];
                PurchaseOrderReceiptData data = new PurchaseOrderReceiptData();
                data.PurchaseId = Convert.ToInt32(header["PurchaseId"]);
                data.OrderNumber = Convert.ToString(header["OrderNumber"]);
                data.OrderDate = Convert.ToDateTime(header["OrderDate"]);
                data.VendorId = Convert.ToInt32(header["VendorId"]);
                data.VendorName = Convert.ToString(header["VendorName"]);
                data.Remarks = Convert.ToString(header["Remarks"]);
                data.Lines = ds.Tables.Count > 1 ? ds.Tables[1] : new DataTable();
                return data;
            }
        }

        public DataTable GetActiveRacksForProducts(List<int> productIds)
        {
            DataTable empty = new DataTable();
            empty.Columns.Add("ProductId", typeof(int));
            empty.Columns.Add("RackId", typeof(int));
            empty.Columns.Add("RackCaption", typeof(string));
            empty.Columns.Add("LocationId", typeof(int));
            empty.Columns.Add("LocationName", typeof(string));

            if (productIds == null || productIds.Count == 0)
            {
                return empty;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(@"
SELECT DISTINCT
    prm.ProductId,
    rm.RackId,
    rm.RackCaption,
    lm.LocationId,
    lm.LocationName,
    ISNULL(lm.DisplayOrder, 0) AS LocationDisplayOrder,
    ISNULL(rm.DisplayOrder, 0) AS RackDisplayOrder
FROM dbo.ProductRackMapping prm
INNER JOIN dbo.RackMaster rm
    ON rm.RackId = prm.RackId
INNER JOIN dbo.LocationMaster lm
    ON lm.LocationId = rm.LocationId
WHERE rm.IsActive = 1
  AND lm.IsActive = 1
  AND prm.ProductId IN (");

            for (int i = 0; i < productIds.Count; i++)
            {
                if (i > 0)
                {
                    sql.Append(",");
                }
                sql.Append("@p");
                sql.Append(i.ToString());
            }

            sql.Append(@")
ORDER BY
    ISNULL(lm.DisplayOrder, 0),
    lm.LocationName,
    ISNULL(rm.DisplayOrder, 0),
    rm.RackCaption,
    rm.RackId");

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql.ToString(), con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                for (int i = 0; i < productIds.Count; i++)
                {
                    cmd.Parameters.Add("@p" + i.ToString(), SqlDbType.Int).Value = productIds[i];
                }

                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        public PurchaseReceiptSaveResult SavePurchaseReceipt(PurchaseReceiptSaveRequest request)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.SavePurchaseReceipt_direct", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.Add("@isnew", SqlDbType.Int).Value = request.IsNew;
                cmd.Parameters.Add("@OrderNumber", SqlDbType.VarChar, 100).Value = request.OrderNumber;
                cmd.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = request.OrderDate;
                cmd.Parameters.Add("@VendorId", SqlDbType.Int).Value = request.VendorId;
                cmd.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = request.Status;
                cmd.Parameters.Add("@EnteredBy", SqlDbType.VarChar, 50).Value = request.EnteredBy;

                SqlParameter purchaseDetails = cmd.Parameters.Add("@PurchaseDetails", SqlDbType.Structured);
                purchaseDetails.TypeName = "dbo.PurchaseReceiptDetailstype";
                purchaseDetails.Value = request.PurchaseDetails;

                cmd.Parameters.Add("@Remarks", SqlDbType.VarChar, -1).Value = request.Remarks;
                cmd.Parameters.Add("@Partial", SqlDbType.VarChar, -1).Value = request.Partial;

                SqlParameter rackDetails = cmd.Parameters.Add("@RackDetails", SqlDbType.Structured);
                rackDetails.TypeName = "dbo.PurchaseReceiptRackDetailType";
                rackDetails.Value = request.RackDetails;

                SqlParameter outParam = cmd.Parameters.Add("@out", SqlDbType.Int);
                outParam.Direction = ParameterDirection.Output;
                SqlParameter resultParam = cmd.Parameters.Add("@result", SqlDbType.VarChar, 50);
                resultParam.Direction = ParameterDirection.Output;

                con.Open();
                cmd.ExecuteNonQuery();

                PurchaseReceiptSaveResult result = new PurchaseReceiptSaveResult();
                result.OutputMessage = Convert.ToString(resultParam.Value);
                result.ResultCode = outParam.Value == DBNull.Value ? 0 : Convert.ToInt32(outParam.Value);
                if (result.OutputMessage.Length == 0)
                {
                    result.OutputMessage = "Purchase receipt saved.";
                }
                return result;
            }
        }
    }

    public class PurchaseOrderReceiptData
    {
        public int PurchaseId;
        public string OrderNumber;
        public DateTime OrderDate;
        public int VendorId;
        public string VendorName;
        public string Remarks;
        public DataTable Lines;
    }

    public class PurchaseReceiptSaveRequest
    {
        public int IsNew;
        public string OrderNumber;
        public DateTime OrderDate;
        public int VendorId;
        public string Status;
        public string EnteredBy;
        public string Remarks;
        public string Partial;
        public DataTable PurchaseDetails;
        public DataTable RackDetails;
    }

    public class PurchaseReceiptSaveResult
    {
        public string OutputMessage;
        public int ResultCode;
    }
}
