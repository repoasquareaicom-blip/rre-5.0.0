using InvBal;
using Inventory.Accounts;
using Inventory.Adjustment;
using Inventory.Commission;
using Inventory.Masters;
using Inventory.Movement;
using Inventory.Purchase;
using Inventory.Report;
using Inventory.Sales;
using Inventory.Service;
using Inventory.Tool;
using Inventory.Transactions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory
{
    static class Program
    {
        public static string connection = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        public static int userlevel;
        public static string PrintInvoiceNumber;

        public static string State;
        public static string AppVersion = "5.0.0";
        public static string CopyText;
        public static DataTable dtitems;
        public static string Company;
        public static string userid;
        public static string path = System.Configuration.ConfigurationManager.AppSettings["ImagePath"];
        public static string UserName;
        public static string Userrole;
        public static string Userfullname;
        public static string ShopName;
        public static string Floor;
        public static DataTable Dtmenu;
        public static PurchaseOrder objPurchaseOrder;
        // public static Product objProduct;
        public static PurchaseReceipt objPurchaseReceipt;
        public static SalesBillNew objSalesBillNew;
        public static SalesNewQuotation objsalesnewquotation;

        //public static class.Inventory.Sales.SalesN
        public static SalesQuotation1 objSalesQuotation1;
        public static FrmIssuedReceivedReport objFrmIssuedReceivedReport;
        public static Purchase_Return.FrmPurchaseReturn objFrmPurchaseReturn;
        public static UploadHistory UploadHistory;

        public static RackUpload objrack;

        //Masters
        public static Masters.Product objProduct;
        public static Category objCategory;
        public static Customers objCustomers;
        public static Reference objReference;
        public static Suppliers objSuppliers;
        public static UOM objUOM;
        public static Brand objBrand;

        public static CustomerAccount CustAccount;

        public static GSTReport GSTReports;

        //public static Report.StockReport objStockReport;
        public static Location objLocation;
        public static FrmUserCreation objFrmUserCreation;
        public static FrmPasswordSetup objFrmPasswordSetup;
        public static Employee objEmployee;
        //public static UOM objUOM;

        //Reports
        public static  CashTransactionReportRV ObjCashTransactionReportRV;
        public static FrmStockFinalRpt ObjFrmStockFinalRpt;

        public static Inventory.Report_Transaction.CashTransaction Objcashtransaction;

        //Tool
        public static CashEndClose ObjCashEndClose;
        public static CashEndCloseVerification ObjCashEndCloseVerification;
        //Accounts
        public static AccountsReceivable ObjAccountsReceivable;
        public static AccountPayable ObjAccountPayable;
       //commission 
        public static CommissionBill objCommissionBill;
       // public static CashRequestList objCashRequestList;
        //FrmMain
        public static FrmMain objFrmMain;
        //public static Transaction objTransaction;

        //Transaction
        public static CashRequest objCashRequest;
        public static CashRequestApproval objCashRequestApproval;
        public static CashPayment objCashPayment;
        public static MaterialMovement objMaterialMovement;
        public static FrmIssuedReceived objFrmIssuedReceived;

        public static SalesReturn objSalesReturn;
        public static WindowSales objWindowSales;
        public static SalesReport objSalesReport;
        public static SalesNewreport objSalesNewreport;
        public static CommissionPayment objCommissionPayment;
        //public static FrmStockReport objFrmStockReport;
        public static FrmRoles objFrmRole;
        public static FrmReceived objFrmReceived;
        //SearchPurchse&Sales

        public static SearchPurchaseOrder objSearchPurchaseOrder;
        public static SearchQuatation objSearchQuatation;
        public static SearchQuatationEstimation objSearchQuatationEstimation;
        public static FollowupQuotationReport objFollowupQuotationReport;
        //Adjustment
        public static AdjustmentforSalesBill ObjAdjustmentforSalesBill;
        public static AdjustmentforPurchaseBill ObjAdjustmentforPurchaseBill;
        //service
        public static ServiceRequest objServiceRequest;
        public static CancelledTray objCancelledTray;
        public static IssuedTray objIssuedTray;
        public static SalesReturnList objSalesReturnList;
        public static CashReceipt objCashReceipt;

        public static Customerwiseledger objCustomerledger;
      
        public static IncentiveReport objIncentiveReport;
        public static Requestlist objRequestlist;
        public static Cashreceiptlist objCashreceiptlist;

        public static StockAdjustment objStockAdjustment;
        public static StockAdjustmentView objStockAdjustmentView;
        public static SalesReportForm objSalesReportForm;

        public static WarrantyList objWarrantyList;
        public static Issued objIssued;
        public static Received objReceived;
        public static Stockreport objStockreport;

        public static EstimationAging objEstimationAging;
        public static MaterialTranscationreport objMaterialTranscationreport;
        public static SALES_TRANSACTION_REPORT objSALES_TRANSACTION_REPORT;
        

        public static Purchase_Ageing objPurchase_Ageing;
        public static ExpressSales objExpressSales;
        public static SalesPDI objSalesPDI;


        public static Userestimation objUserestimation;

        public static AccountHeadLedger frmAccountHeadLedger;
        public static PeriodWiseClosingStock frmPeriodWiseClosingStock;
        public static NotInEstimation frmNotInEstimation;
        public static CashRequestlist objCashRequestlist;

        public static HsnStockReport ObjHsnstockreport;

        public static UploadFile UploadFile;

        public static ReUpload ReUpload;

        public static PriceUpload PriceUpload;
        public static ProductSyncQueue ProductSyncQueue;


        public static GstFormReport GstFormReports;

        public static string Service;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            itemdetails("");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Mainform());
        }
        public static void itemdetails(string s)
        {

            try
            {
                dtitems = new DataTable();
                string s1 = s.Trim();
                string s2 = "";
                string name = s1.Replace("'", "''");
                QuotationBal objQuotationbal = new QuotationBal();
                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                dtitems =objQuotationbal.itemdetails(name, s2);


            }
            catch (Exception e)
            {

            }

        }
    }
    public class AlphanumComparator<T> : IComparer<T>
    {
        private enum ChunkType { Alphanumeric, Numeric };
        private bool InChunk(char ch, char otherCh)
        {
            ChunkType type = ChunkType.Alphanumeric;

            if (char.IsDigit(otherCh))
            {
                type = ChunkType.Numeric;
            }

            if ((type == ChunkType.Alphanumeric && char.IsDigit(ch))
                || (type == ChunkType.Numeric && !char.IsDigit(ch)))
            {
                return false;
            }

            return true;
        }

        public int Compare(T x, T y)
        {
            String s1 = x as string;
            String s2 = y as string;
            if (s1 == null || s2 == null)
            {
                return 0;
            }

            int thisMarker = 0, thisNumericChunk = 0;
            int thatMarker = 0, thatNumericChunk = 0;

            while ((thisMarker < s1.Length) || (thatMarker < s2.Length))
            {
                if (thisMarker >= s1.Length)
                {
                    return -1;
                }
                else if (thatMarker >= s2.Length)
                {
                    return 1;
                }
                char thisCh = s1[thisMarker];
                char thatCh = s2[thatMarker];

                StringBuilder thisChunk = new StringBuilder();
                StringBuilder thatChunk = new StringBuilder();

                while ((thisMarker < s1.Length) && (thisChunk.Length == 0 || InChunk(thisCh, thisChunk[0])))
                {
                    thisChunk.Append(thisCh);
                    thisMarker++;

                    if (thisMarker < s1.Length)
                    {
                        thisCh = s1[thisMarker];
                    }
                }

                while ((thatMarker < s2.Length) && (thatChunk.Length == 0 || InChunk(thatCh, thatChunk[0])))
                {
                    thatChunk.Append(thatCh);
                    thatMarker++;

                    if (thatMarker < s2.Length)
                    {
                        thatCh = s2[thatMarker];
                    }
                }

                int result = 0;
                // If both chunks contain numeric characters, sort them numerically
                if (char.IsDigit(thisChunk[0]) && char.IsDigit(thatChunk[0]))
                {
                    thisNumericChunk = Convert.ToInt32(thisChunk.ToString());
                    thatNumericChunk = Convert.ToInt32(thatChunk.ToString());

                    if (thisNumericChunk < thatNumericChunk)
                    {
                        result = -1;
                    }

                    if (thisNumericChunk > thatNumericChunk)
                    {
                        result = 1;
                    }
                }
                else
                {
                    result = thisChunk.ToString().CompareTo(thatChunk.ToString());
                }

                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }


    }
}


