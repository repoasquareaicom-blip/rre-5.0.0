using System.Configuration;

namespace Inventory
{
    public static class BranchAccess
    {
        public const string MainOfficeBranchCode = "RR-SALEM";

        public static string CurrentBranchCode
        {
            get
            {
                string branchCode = ConfigurationManager.AppSettings["BranchCode"];
                return string.IsNullOrEmpty(branchCode) ? string.Empty : branchCode.Trim().ToUpperInvariant();
            }
        }

        public static bool IsMainOffice
        {
            get { return CurrentBranchCode == MainOfficeBranchCode; }
        }

        public static string MainOfficeOnlyMessage
        {
            get { return "Product master and price changes are allowed only in main office (Salem)."; }
        }
    }
}
