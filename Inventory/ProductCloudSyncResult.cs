namespace Inventory
{
    public class ProductCloudSyncResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public static ProductCloudSyncResult Ok()
        {
            return new ProductCloudSyncResult { Success = true, ErrorMessage = string.Empty };
        }

        public static ProductCloudSyncResult Failed(string message)
        {
            return new ProductCloudSyncResult { Success = false, ErrorMessage = message ?? string.Empty };
        }
    }
}
