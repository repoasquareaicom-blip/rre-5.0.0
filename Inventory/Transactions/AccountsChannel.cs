using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory.Transactions
{
    internal static class AccountsChannel
    {
        public static string NoteCount(DataGridView grid, int face)
        {
            if (grid == null)
            {
                return "0";
            }
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (Convert.ToString(grid.Rows[i].Cells[0].Value) == face.ToString())
                {
                    string count = Convert.ToString(grid.Rows[i].Cells[1].Value);
                    return string.IsNullOrEmpty(count) ? "0" : count;
                }
            }
            return "0";
        }

        public static string SignedChangeCount(DataGridView grid, int face)
        {
            string count = NoteCount(grid, face);
            if (count != "0")
            {
                return "-" + count;
            }
            return count;
        }

        public static bool TryUpiReference(string text, out string reference)
        {
            reference = text == null ? "" : text.Trim();
            if (reference.Length == 0)
            {
                MessageBox.Show("Enter UPI Payment Reference");
                return false;
            }
            if (reference.Length > 30)
            {
                MessageBox.Show("UPI Payment Reference cannot be more than 30 characters");
                return false;
            }
            return true;
        }

        public static void SaveTwoHundred(string transId, string twoHundred, string twoHundredReturn)
        {
            if (string.IsNullOrEmpty(transId))
            {
                return;
            }
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.SaveAccountsTwoHundred", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransId", transId);
                cmd.Parameters.AddWithValue("@TwoHundred", (object)twoHundred ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TwoHundredReturn", (object)twoHundredReturn ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void SaveAccountReceivableTwoHundred(int enteredBy, string paidAmount, string twoHundred, string twoHundredReturn)
        {
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.SaveAccountReceivableTwoHundred", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EnteredBy", enteredBy);
                cmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
                cmd.Parameters.AddWithValue("@TwoHundred", (object)twoHundred ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TwoHundredReturn", (object)twoHundredReturn ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static int SaveAccountReceivableUpi(string billId, string customerId, bool isRecord, string paidAmount, string reference, int enteredBy)
        {
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.SaveAccountReceivableUpi", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BillId", billId);
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                cmd.Parameters.AddWithValue("@IsRecord", isRecord);
                cmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
                cmd.Parameters.AddWithValue("@Reference", reference);
                cmd.Parameters.AddWithValue("@EnteredBy", enteredBy);
                SqlParameter result = cmd.Parameters.Add("@Result", SqlDbType.Int);
                result.Direction = ParameterDirection.Output;
                con.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToInt32(result.Value);
            }
        }

        public static string SavePaymentRequestUpi(string requestId, decimal amount, string reference, int paidBy, string accountHead)
        {
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.SavePaymentRequestUpi", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RequestId", requestId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Reference", reference);
                cmd.Parameters.AddWithValue("@CashPaidBy", paidBy);
                cmd.Parameters.AddWithValue("@AccountHead", accountHead ?? "");
                SqlParameter result = cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
                result.Direction = ParameterDirection.Output;
                con.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToString(result.Value);
            }
        }

        public static string SaveRoseRequestUpi(string commissionId, decimal amount, string reference)
        {
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.SaveRoseRequestUpi", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CommissionId", commissionId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Reference", reference);
                SqlParameter result = cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
                result.Direction = ParameterDirection.Output;
                con.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToString(result.Value);
            }
        }

        public static string SaveCashReceiptUpi(string requestId, decimal amount, string reference)
        {
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.SaveCashReceiptUpi", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RequestId", requestId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Reference", reference);
                SqlParameter result = cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
                result.Direction = ParameterDirection.Output;
                con.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToString(result.Value);
            }
        }

        public static void SaveCashReceiptReason(string receiptId, string reason)
        {
            if (string.IsNullOrEmpty(receiptId))
            {
                return;
            }
            using (SqlConnection con = new SqlConnection(Program.connection))
            using (SqlCommand cmd = new SqlCommand("dbo.SaveCashReceiptReason", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ReceiptId", receiptId);
                cmd.Parameters.AddWithValue("@Reason", reason ?? "");
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
