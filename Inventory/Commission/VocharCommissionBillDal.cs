using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Inventory.Commission
{
    public class VocharCommissionBillDal
    {
        public DataSet dsMain;
        decimal Total = 0.0M;
        System.IO.StreamWriter rdr;

        private string Bold_On = "_G";
        private string Bold_Off = "_H";
        private string Width_On = "_W1"; //Chr(27) + Chr(87) + Chr(49) 'W1
        private string Width_Off = "_W0";

        //Public Const Compress_On = "¤"       'Chr(15)    '¤"
        //Public Const Compress_Off = "_" 'Chr(18)   '_
        private string ELITE_PITCH = "_M";
        //private string Compress_On = "_ð ;      
        private string Compress_Off = "_";

        private int ColWidth = 60;
        public string BillType = "VOUCHER";
        public string BillNo;
        public string BillDt;
        public string Clerk;
        public string ClientName = "";
        public decimal Discount;
        public decimal TotalAmt;
        public decimal NetAmount;
        public decimal MRPTotal = 0, SavedTotal = 0;
        public System.Data.DataTable dt;
        public string fileName;
        public VocharCommissionBillDal()
        {
            fileName = "d:\\bill.txt";
            var myFile = File.Create(fileName);
            myFile.Close();

        }
        public bool GenerateQuoation()
        {
            bool result = true;

            bool loop = true;
            using (rdr = new System.IO.StreamWriter(fileName))
            {
                PrintHeader();
                PrintBuffer();
                Close();
                rdr.Close();
            }
            return result;
        }
        int slno = 1;
        int pagenumber = 1;
        int pagerecno = 0;
        bool headerflag = false;
        bool showprevsum = false;

        public void Close()
        {
            rdr.Close();
        }
        public void GetDataSet(DataSet Ds)
        {
            dsMain = Ds;
        }
        public void PrintHeader()
        {
            if (dsMain.Tables[0].Rows.Count > 0)
            {
                rdr.WriteLine("");
                //PrintLine();
                //rdr.WriteLine((char)14 + GetCenterdFormatedText(Convert.ToString(dsMain.Tables[0].Rows[0]["CompanyName"]), 40));
                //rdr.WriteLine();
                //rdr.WriteLine((char)18 + GetCenterdFormatedText(Convert.ToString(dsMain.Tables[0].Rows[0]["DoorNo"]), 80));
                //rdr.WriteLine(GetCenterdFormatedText(Convert.ToString(dsMain.Tables[0].Rows[0]["Address1"]), 80));
                //rdr.WriteLine(GetCenterdFormatedText(Convert.ToString(dsMain.Tables[0].Rows[0]["Phone1"]), 80));
                rdr.WriteLine((char)14 + GetCenterdFormatedText("VOUCHER", 40));
                rdr.WriteLine("");
                PrintLine();
                rdr.Write("Rose Id : " + GetFormatedText(Convert.ToString(dsMain.Tables[0].Rows[0]["RequestId"]), 40));
                rdr.Write("DATE : " + GetFormatedText(DateTime.Now.ToString("dd/MM/yyyy"), 25));
                PrintLine();

                rdr.WriteLine(GetFormatedText("Rose Mode :" + Convert.ToString(dsMain.Tables[0].Rows[0]["PaymentMode"]), 65));
                rdr.WriteLine("");
                rdr.WriteLine(GetFormatedText("the sum Of Rupees " + Convert.ToString(dsMain.Tables[0].Rows[0]["Words"]), 60));
                rdr.WriteLine("");



                rdr.WriteLine(GetFormatedText("Paid to :" + Convert.ToString(dsMain.Tables[0].Rows[0]["RequestedBy"]), 60));
                rdr.WriteLine("");

                PrintLine();
                rdr.Write(GetFormatedText("Rs. " + Convert.ToString(dsMain.Tables[0].Rows[0]["Amount"]), 40));
                rdr.Write(GetFormatedText("Total amount :" + Convert.ToString(dsMain.Tables[0].Rows[0]["totalamount"]), 40));

                PrintLine();
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.Write(GetFormatedText("Signature", 54));
                rdr.Write(GetRightFormatedText("Authorized Signatory", 25));
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");

                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine("");
                rdr.WriteLine(".");
            }

        }


        private string GetFormatedText(string Cont, int Length)
        {
            int rLoc = Length - Cont.Length;
            if (rLoc < 0)
            {
                Cont = Cont.Substring(0, Length);
            }
            else
            {
                int nos;
                for (nos = 0; nos < rLoc; nos++) Cont = Cont + " ";
            }
            return (Cont);
        }
        private string GetRightFormatedText(string Cont, int Length)
        {
            int rLoc = Length - Cont.Length;
            if (rLoc < 0)
            {
                Cont = Cont.Substring(0, Length);
            }
            else
            {
                int nos;
                string space = "";
                for (nos = 0; nos < rLoc; nos++)
                    space += " ";
                Cont = space + Cont;
            }
            return (Cont);
        }
        private string GetCenterdFormatedText(string Cont, int Length)
        {
            int rLoc = Length - Cont.Length;
            if (rLoc < 0)
            {
                Cont = Cont.Substring(0, Length);
            }
            else
            {
                int nos;
                string space = "";
                for (nos = 0; nos < rLoc / 2; nos++) space += " ";
                Cont = space + Cont;
            }
            return (Cont);
        }

        public void PrintLine()
        {
            int i;
            string Lstr = "";
            for (i = 1; i <= 80; i++)
            {
                Lstr = Lstr + "-";
            }
            rdr.WriteLine(Lstr);

        }

        public void PrintBuffer()
        {

            Process cmd = new Process();
            cmd.StartInfo.FileName = "d:\\bill.bat";

            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            cmd.Start();

        }


    }
}