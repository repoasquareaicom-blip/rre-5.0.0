using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace Inventory
{
    class ReciptReport
    {
        public DataSet dsMain;
        private decimal Total = 0.0m;
        private StreamWriter rdr;
        private string Bold_On = "_G";
        private string Bold_Off = "_H";
        private string Width_On = "_W1";
        private string Width_Off = "_W0";
        private string ELITE_PITCH = "_M";
        private string Compress_Off = "_";
        private int ColWidth = 60;
        public string BillType = "RECEIPT";
        public string BillNo;
        public string BillDt;
        public string Clerk;
        public string ClientName = "";
        public decimal Discount;
        public decimal TotalAmt;
        public decimal NetAmount;
        public decimal MRPTotal = 0m;
        public decimal SavedTotal = 0m;
        public DataTable dt;
        public string fileName;
        private int slno = 1;
        private int pagenumber = 1;
        private int pagerecno = 0;
        private bool headerflag = false;
        private bool showprevsum = false;
        public ReciptReport()
        {
            this.fileName = "d:\\bill.txt";
            FileStream fileStream = File.Create(this.fileName);
            fileStream.Close();
        }
        public bool GenerateQuoation()
        {
            bool result = true;
            using (this.rdr = new StreamWriter(this.fileName))
            {
                this.PrintHeader();
                this.PrintBuffer();
                this.Close();
                this.rdr.Close();
            }
            return result;
        }


        public bool GenerateQuoationReport()
        {
            bool result = true;
            using (this.rdr = new StreamWriter(this.fileName))
            {
                this.PrintHeaders();
                //this.PrintBuffer();
                this.Close();
                this.rdr.Close();
            }
            return result;
        }
        public void Close()
        {
            this.rdr.Close();
        }
        public void GetDataSet(DataSet Ds)
        {
            this.dsMain = Ds;
        }
        public void PrintHeader()
        {
            if (this.dsMain.Tables[0].Rows.Count > 0)
            {

                this.rdr.WriteLine("\u001bjl" + "\u001bjl" + "\u001bjl");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine(this.GetCenterdFormatedText("RECEIPT", 40));
                this.rdr.WriteLine("");

                this.PrintLine();

                this.rdr.Write("RECEIPT NO : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["ReceiptId"]), 48));
                this.rdr.Write("DATE : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Transdate"]), 25));
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.PrintLine();
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine(this.GetFormatedText("Received with thanks from M/S.., ", 55));
                this.rdr.WriteLine(this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["CustomerName"]), 68));
                this.rdr.WriteLine("");
                this.rdr.WriteLine(this.GetFormatedText("the sum Of Rupees " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Numberinwords"]), 60));
                this.rdr.WriteLine("");
                if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Mode"])))
                {
                    this.rdr.WriteLine(this.GetFormatedText("through by  " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Mode"]), 60));
                    this.rdr.WriteLine("");
                }
                this.rdr.WriteLine(this.GetFormatedText("toward your account.", 80));
                this.rdr.WriteLine("");
                this.PrintLine();
                this.rdr.WriteLine(this.GetFormatedText("Rs. " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Amount"]), 30));
                this.rdr.WriteLine("");
                if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["EntityId"])))
                {
                    this.rdr.Write(this.GetFormatedText("BILL DETAILS:" + Convert.ToString(this.dsMain.Tables[0].Rows[0]["EntityId"]), 60));
                }
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.PrintLine();

                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.Write(this.GetFormatedText("Signature", 54));
                this.rdr.Write(this.GetRightFormatedText("Authorized Signatory", 25));
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");

            }
        }

        public void PrintHeaders()
        {
            if (this.dsMain.Tables[0].Rows.Count > 0)
            {

                this.rdr.WriteLine("\u001bjl" + "\u001bjl" + "\u001bjl");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine(this.GetCenterdFormatedText("RECEIPT", 40));
                this.rdr.WriteLine("");

                this.PrintLine();

                this.rdr.Write("RECEIPT NO : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["ReceiptId"]), 48));
                this.rdr.Write("DATE : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Transdate"]), 25));
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.PrintLine();
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine(this.GetFormatedText("Received with thanks from M/S.., ", 55));
                this.rdr.WriteLine(this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["CustomerName"]), 68));
                this.rdr.WriteLine("");
                this.rdr.WriteLine(this.GetFormatedText("the sum Of Rupees " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Numberinwords"]), 60));
                this.rdr.WriteLine("");
                if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Mode"])))
                {
                    this.rdr.WriteLine(this.GetFormatedText("through by  " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Mode"]), 60));
                    this.rdr.WriteLine("");
                }
                this.rdr.WriteLine(this.GetFormatedText("toward your account.", 80));
                this.rdr.WriteLine("");
                this.PrintLine();
                this.rdr.WriteLine(this.GetFormatedText("Rs. " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Amount"]), 30));
                this.rdr.WriteLine("");
                if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["EntityId"])))
                {
                    this.rdr.Write(this.GetFormatedText("BILL DETAILS:" + Convert.ToString(this.dsMain.Tables[0].Rows[0]["EntityId"]), 60));
                }
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.PrintLine();

                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.Write(this.GetFormatedText("Signature", 54));
                this.rdr.Write(this.GetRightFormatedText("Authorized Signatory", 25));
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");

            }
        }
        public void PrintDetails2()
        {
            this.rdr.WriteLine("SLNo |Name                 |Qty|MRP   |Rate  |T.Amt  ");
            this.PrintLine();
            for (int i = 1; i <= 30; i++)
            {
                this.rdr.Write("{0,-4}", this.GetFormatedText(i.ToString(), 5) + " ");
                this.rdr.Write("{0,-35}", this.GetFormatedText("Product 1", 35) + " ");
                this.rdr.Write("{0,-2}", this.GetFormatedText("2", 3) + " ");
                this.rdr.Write("{0,-6}", this.GetFormatedText("125.00", 6) + " ");
                this.rdr.Write("{0,-6}", this.GetFormatedText("250.00", 6) + " ");
                this.rdr.Write("{0,-6}", this.GetFormatedText("250.00", 6) + " ");
                this.rdr.WriteLine("");
            }
        }
        private string GetFormatedText(string Cont, int Length)
        {
            int num = Length - Cont.Length;
            if (num < 0)
            {
                Cont = Cont.Substring(0, Length);
            }
            else
            {
                for (int i = 0; i < num; i++)
                {
                    Cont += " ";
                }
            }
            return Cont;
        }
        private string GetRightFormatedText(string Cont, int Length)
        {
            int num = Length - Cont.Length;
            if (num < 0)
            {
                Cont = Cont.Substring(0, Length);
            }
            else
            {
                string str = "";
                for (int i = 0; i < num; i++)
                {
                    str += " ";
                }
                Cont = str + Cont;
            }
            return Cont;
        }
        private string GetCenterdFormatedText(string Cont, int Length)
        {
            int num = Length - Cont.Length;
            if (num < 0)
            {
                Cont = Cont.Substring(0, Length);
            }
            else
            {
                string str = "";
                for (int i = 0; i < num / 2; i++)
                {
                    str += " ";
                }
                Cont = str + Cont;
            }
            return Cont;
        }
        public void PrintLine()
        {
            string text = "";
            for (int i = 1; i <= 80; i++)
            {
                text += "-";
            }
            this.rdr.WriteLine(text);
        }
        public void PrintBuffer()
        {
            new Process
            {
                StartInfo =
                {
                    FileName = "d:\\bill.bat",
                    WindowStyle = ProcessWindowStyle.Maximized
                }
            }.Start();
        }
    }

    class CashReciptReport
    {
        public DataSet dsMain;
        private decimal Total = 0.0m;
        private StreamWriter rdr;
        private string Bold_On = "_G";
        private string Bold_Off = "_H";
        private string Width_On = "_W1";
        private string Width_Off = "_W0";
        private string ELITE_PITCH = "_M";
        private string Compress_Off = "_";
        private int ColWidth = 60;
        public string BillType = "RECEIPT";
        public string BillNo;
        public string BillDt;
        public string Clerk;
        public string ClientName = "";
        public decimal Discount;
        public decimal TotalAmt;
        public decimal NetAmount;
        public decimal MRPTotal = 0m;
        public decimal SavedTotal = 0m;
        public DataTable dt;
        public string fileName;
        private int slno = 1;
        private int pagenumber = 1;
        private int pagerecno = 0;
        private bool headerflag = false;
        private bool showprevsum = false;
        public CashReciptReport()
        {
            this.fileName = "d:\\bill.txt";
            FileStream fileStream = File.Create(this.fileName);
            fileStream.Close();
        }
        public bool GenerateQuoation()
        {
            bool result = true;
            using (this.rdr = new StreamWriter(this.fileName))
            {
                this.PrintHeader();
                this.PrintBuffer();
                this.Close();
                this.rdr.Close();
            }
            return result;
        }
        public void Close()
        {
            this.rdr.Close();
        }
        public void GetDataSet(DataSet Ds)
        {
            this.dsMain = Ds;
        }
        public void PrintHeader()
        {
            if (this.dsMain.Tables[0].Rows.Count > 0)
            {
                this.rdr.WriteLine("\u001bjl" + "\u001bjl" + "\u001bjl");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine(this.GetCenterdFormatedText("RECEIPT", 40));
                this.rdr.WriteLine("");

                this.PrintLine();
                this.rdr.Write("RECEIPT NO : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["ReceiptId"]), 48));
                this.rdr.Write("DATE : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Transdate"]), 25));
                this.rdr.WriteLine("");
                this.PrintLine();
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine(this.GetFormatedText("Received with thanks from M/S.., ", 55));
                this.rdr.WriteLine(this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["CustomerName"]), 68));
                this.rdr.WriteLine("");
                this.rdr.WriteLine(this.GetFormatedText("the sum Of Rupees " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Numberinwords"]), 60));
                this.rdr.WriteLine("");
                if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Mode"])))
                {
                    this.rdr.WriteLine(this.GetFormatedText("through by  " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Mode"]), 60));
                    this.rdr.WriteLine("");
                }
                this.rdr.WriteLine(this.GetFormatedText("toward your account.", 80));
                this.rdr.WriteLine("");
                this.PrintLine();
                this.rdr.WriteLine(this.GetFormatedText("Rs. " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Amount"]), 30));
                //if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["EntityId"])))
                //{
                //    this.rdr.Write(this.GetFormatedText("BILL DETAILS:" + Convert.ToString(this.dsMain.Tables[0].Rows[0]["EntityId"]), 60));
                //}

                this.PrintLine();

                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.Write(this.GetFormatedText("Signature", 54));
                this.rdr.Write(this.GetRightFormatedText("Authorized Signatory", 25));
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
                this.rdr.WriteLine("");
            }
        }
        public void PrintDetails2()
        {
            this.rdr.WriteLine("SLNo |Name                 |Qty|MRP   |Rate  |T.Amt  ");
            this.PrintLine();
            for (int i = 1; i <= 30; i++)
            {
                this.rdr.Write("{0,-4}", this.GetFormatedText(i.ToString(), 5) + " ");
                this.rdr.Write("{0,-35}", this.GetFormatedText("Product 1", 35) + " ");
                this.rdr.Write("{0,-2}", this.GetFormatedText("2", 3) + " ");
                this.rdr.Write("{0,-6}", this.GetFormatedText("125.00", 6) + " ");
                this.rdr.Write("{0,-6}", this.GetFormatedText("250.00", 6) + " ");
                this.rdr.Write("{0,-6}", this.GetFormatedText("250.00", 6) + " ");
                this.rdr.WriteLine("");
            }
        }
        private string GetFormatedText(string Cont, int Length)
        {
            int num = Length - Cont.Length;
            if (num < 0)
            {
                Cont = Cont.Substring(0, Length);
            }
            else
            {
                for (int i = 0; i < num; i++)
                {
                    Cont += " ";
                }
            }
            return Cont;
        }
        private string GetRightFormatedText(string Cont, int Length)
        {
            int num = Length - Cont.Length;
            if (num < 0)
            {
                Cont = Cont.Substring(0, Length);
            }
            else
            {
                string str = "";
                for (int i = 0; i < num; i++)
                {
                    str += " ";
                }
                Cont = str + Cont;
            }
            return Cont;
        }
        private string GetCenterdFormatedText(string Cont, int Length)
        {
            int num = Length - Cont.Length;
            if (num < 0)
            {
                Cont = Cont.Substring(0, Length);
            }
            else
            {
                string str = "";
                for (int i = 0; i < num / 2; i++)
                {
                    str += " ";
                }
                Cont = str + Cont;
            }
            return Cont;
        }
        public void PrintLine()
        {
            string text = "";
            for (int i = 1; i <= 80; i++)
            {
                text += "-";
            }
            this.rdr.WriteLine(text);
        }
        public void PrintBuffer()
        {
            new Process
            {
                StartInfo =
                {
                    FileName = "d:\\bill.bat",
                    WindowStyle = ProcessWindowStyle.Maximized
                }
            }.Start();
        }
    }
}
