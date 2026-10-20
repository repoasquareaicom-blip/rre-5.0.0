using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Sales
{
    public class NoteRackRepository
    {
        private readonly string connectionString = Program.connection;
        private readonly PdiRackRepository pdiRackRepository = new PdiRackRepository();
        private readonly Dictionary<int, bool> rackWiseByProduct = new Dictionary<int, bool>();

        public bool IsRackWise(int productId)
        {
            if (productId <= 0)
            {
                return false;
            }

            bool rackWise;
            if (!rackWiseByProduct.TryGetValue(productId, out rackWise))
            {
                rackWise = pdiRackRepository.GetRackWiseStockMovement(productId);
                rackWiseByProduct[productId] = rackWise;
            }
            return rackWise;
        }

        public void ClearCache()
        {
            rackWiseByProduct.Clear();
        }

        public bool HasRackAllocation(int productId)
        {
            return pdiRackRepository.HasRackAllocation(productId);
        }

        public DataTable GetRacksForProduct(int productId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    prm.ProductId,
    lm.LocationId,
    rm.RackId,
    lm.LocationName,
    rm.RackCaption,
    CAST(ISNULL(stock.AvailableQuantity, 0) AS DECIMAL(18,3)) AS AvailableQty
FROM dbo.ProductRackMapping prm
INNER JOIN dbo.RackMaster rm
    ON rm.RackId = prm.RackId
INNER JOIN dbo.LocationMaster lm
    ON lm.LocationId = rm.LocationId
OUTER APPLY
(
    SELECT SUM(
        CASE
            WHEN UPPER(ISNULL(mt.Type, '')) = 'IN' THEN ISNULL(mt.Quantity, 0)
            WHEN UPPER(ISNULL(mt.Type, '')) = 'OUT' THEN -ISNULL(mt.Quantity, 0)
            ELSE 0
        END
    ) AS AvailableQuantity
    FROM dbo.MaterialTranscation mt
    WHERE mt.MaterailId = prm.ProductId
      AND mt.RackId = prm.RackId
) stock
WHERE prm.ProductId = @ProductId
  AND ISNULL(rm.IsActive, 0) = 1
  AND ISNULL(lm.IsActive, 0) = 1
ORDER BY ISNULL(lm.DisplayOrder, 0), ISNULL(rm.DisplayOrder, 0), rm.RackId", con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public DataTable GetAllocations(bool receiptNote, int noteId)
        {
            string table = receiptNote ? "ReceiptNoteRackAllocation" : "DeliveryNoteRackAllocation";
            string noteColumn = receiptNote ? "ReceiptNoteId" : "DeliveryNoteId";
            string detailColumn = receiptNote ? "ReceiptNoteDetailId" : "DeliveryNoteDetailId";
            string idColumn = receiptNote ? "ReceiptNoteRackAllocationId" : "DeliveryNoteRackAllocationId";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT r." + detailColumn + @" AS DetailId, r.ProductId, r.RackId, r.Quantity,
       ISNULL(lm.LocationName, '') AS LocationName, ISNULL(rm.RackCaption, CONVERT(varchar(20), r.RackId)) AS RackCaption
FROM dbo." + table + @" r
LEFT JOIN dbo.RackMaster rm ON rm.RackId = r.RackId
LEFT JOIN dbo.LocationMaster lm ON lm.LocationId = rm.LocationId
WHERE r." + noteColumn + @" = @NoteId
ORDER BY r." + detailColumn + ", r." + idColumn, con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@NoteId", SqlDbType.Int).Value = noteId;
                DataTable result = new DataTable();
                adapter.Fill(result);
                return result;
            }
        }
    }

    public class NoteRackAllocation
    {
        public int ProductId;
        public Dictionary<int, decimal> Quantities = new Dictionary<int, decimal>();
        public Dictionary<int, string> Captions = new Dictionary<int, string>();
        public Dictionary<int, string> Locations = new Dictionary<int, string>();

        public decimal Total
        {
            get
            {
                decimal total = 0;
                foreach (decimal quantity in Quantities.Values)
                {
                    total += quantity;
                }
                return total;
            }
        }
    }
}
