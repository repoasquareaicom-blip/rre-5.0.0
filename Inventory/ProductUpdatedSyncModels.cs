using System.Collections.Generic;

namespace Inventory
{
    public class ProductUpdatedPendingItem
    {
        public int QueueId { get; set; }
        public string ProductId { get; set; }
        public string DisplayName { get; set; }
        public object SalesPrice { get; set; }
        public object MRP { get; set; }
        public object GST { get; set; }
        public string Status { get; set; }
        public string ChangeType { get; set; }
        public int AttemptCount { get; set; }
        public string LastError { get; set; }
        public object LastTriedOn { get; set; }
        public string TargetBranchCode { get; set; }
        public string LocalStatus { get; set; }
    }

    public class ProductUpdatedApplyResult
    {
        public bool LocalUpdated { get; set; }
        public bool Acknowledged { get; set; }
        public bool Conflict { get; set; }
        public string Message { get; set; }
    }

    public class ProductUpdatedAcceptSummary
    {
        public int Updated { get; set; }
        public int Failed { get; set; }
        public int AcknowledgementPending { get; set; }
    }

    public class ProductUpdatedReadResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ProductUpdatedPendingItem> Items { get; set; }
    }
}
