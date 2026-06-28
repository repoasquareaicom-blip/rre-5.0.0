using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Diagnostics;


namespace Inventory
{
    class RREPrint
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
        public string BillType = "QUOTATION";
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
        public int pagenumber;
        public bool status;
        public int fpage;
        public int lpage;
        private int slno = 1;
        private int slno1 = 1;
        public int Copies = 1;
        private decimal postoatl = 0.0m;
        private decimal nagtoatl = 0.0m;
        private int pagerecno = 0;
        private bool headerflag = false;
        private bool showprevsum = false;
        public string _strRefText;
        public string _strRef;
        public DataSet dsMain1;



        public int linecount = 0;
        public int value = 0;


        public bool Party = false;

        private bool firstpage = true;

        public RREPrint()
        {

        }






        public void RREPrintEstimation()
        {
            bool result = true;
            this.fileName = "d:\\Bill.txt";
            FileStream fileStream = File.Create(this.fileName);
            fileStream.Close();
            using (this.rdr = new StreamWriter(this.fileName))
            {
                for (int i = 1; i <= Copies; i++)
                {
                    PrintEstimationHeader();
                    PrintEstimationDetails();
                    PrintEstimationFooter();
                }

            }
            this.rdr.Close();
            new Process
            {
                StartInfo =
                {
                    FileName = "d:\\Bill.bat",
                    WindowStyle = ProcessWindowStyle.Hidden

                }
            }.Start();

        }
        public void PrintEstimationHeader()
        {
            this.rdr.WriteLine((char)27 + "jn");
            this.rdr.WriteLine((char)27 + "jn");
            this.rdr.WriteLine((char)27 + "jn");
            this.rdr.WriteLine((char)27 + "F" + this.GetCenterdFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["CompanyName"]), 50));
            this.rdr.WriteLine();
            this.rdr.WriteLine((char)(15) + this.GetCenterdFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["DoorNo"]), 90));
            this.rdr.WriteLine(this.GetCenterdFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Address1"]), 90));
            this.rdr.WriteLine(this.GetCenterdFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Phone1"]), 90) + (char)(18));
            this.rdr.WriteLine((char)27 + "F" + this.GetCenterdFormatedText("ESTIMATION", 50));
            this.rdr.WriteLine((char)15 + "");
            this.rdr.Write("Date : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Date"]), 43));
            this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Estimationid"]), 40));
            this.rdr.WriteLine("");
            this.rdr.WriteLine("To : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["customername"]), 50));
            this.PrintLine();
            this.rdr.Write("{0,-3}", this.GetFormatedText("SNo", 3) + " ");
            this.rdr.Write("{0,-50}", this.GetFormatedText("PRODUCT NAME", 50) + " ");
            this.rdr.Write("{0,-12}", this.GetFormatedText("QUANTITY", 12) + " ");
            this.rdr.Write("{0,-9}", this.GetRightFormatedText("RATE", 9) + " ");
            this.rdr.Write("{0,-9}", this.GetRightFormatedText("AMOUNT", 9) + " ");
            this.rdr.WriteLine("");
            this.PrintLine();
        }
        public void PrintEstimationDetails()
        {

            while (this.dsMain.Tables[1].Rows.Count > 0)
            {

                this.rdr.Write("{0,-3}", this.GetFormatedText(this.slno.ToString(), 3) + " ");
                this.rdr.Write("{0,-50}", this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["ItemName"].ToString(), 50) + " ");
                this.rdr.Write("{0,-8}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Quantity"].ToString(), 8) + " ");
                this.rdr.Write("{0,-3}", this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["UOM"].ToString(), 3) + " ");
                this.rdr.Write("{0,-9}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Rate"].ToString(), 9) + " ");
                this.rdr.Write("{0,-9}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Amount"].ToString(), 9) + " ");
                this.rdr.WriteLine("");
                this.Total += Convert.ToDecimal(this.dsMain.Tables[1].Rows[this.slno - 1]["Amount"]);
                this.slno++;
            }

        }
        public void PrintEstimationFooter()
        {
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine(this.GetRightFormatedText("-------------------", 90) + " ");
            if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["ReturnSales"])))
            {
                this.rdr.Write(this.GetFormatedText("     " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["ReturnSales"]), 60));
                this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 27));
                this.rdr.WriteLine("");
            }
            else
            {
                this.rdr.Write(this.GetFormatedText("     TOTAL", 30));
                this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 57));
                this.rdr.WriteLine("");
            }
            if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["LessAmount"])))
            {
                if (Convert.ToDouble(this.dsMain.Tables[0].Rows[0]["LessAmount"]) > 0.0)
                {
                    this.rdr.Write(this.GetFormatedText("     LESS AMOUNT", 30));
                    this.rdr.Write(this.GetRightFormatedText("-" + Convert.ToString(this.dsMain.Tables[0].Rows[0]["LessAmount"]), 57));
                    this.rdr.WriteLine("");
                }
            }
            if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["OtherCharges"])))
            {
                if (Convert.ToDouble(this.dsMain.Tables[0].Rows[0]["OtherCharges"]) > 0.0)
                {
                    this.rdr.Write(this.GetFormatedText("     OTHERS", 30));
                    this.rdr.Write(this.GetRightFormatedText("-" + Convert.ToString(this.dsMain.Tables[0].Rows[0]["OtherCharges"]), 57));
                    this.rdr.WriteLine("");
                }
            }
            this.rdr.Write(this.GetFormatedText("     GRAND TOTAL", 30));
            this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["GrnandTotal"]), 47));
            this.rdr.WriteLine("");
            this.rdr.Write(this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Assist"]), 40));
            this.rdr.WriteLine("");
            this.rdr.Write(this.GetCenterdFormatedText("Check the goods carefully before taking delivery", 90));
            this.rdr.WriteLine("");
            this.rdr.Write(this.GetFormatedText("             No guarantee for glass fittings & bulbs. Company service guarantee only", 90));
            this.rdr.WriteLine("");
            this.rdr.Write(this.GetFormatedText("               for guarantee items. Goods once sold cannot be taken back or exchanged.", 90));
            this.rdr.WriteLine("" + (char)18);
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
        public void RREPrintQuotation()
        {
            bool result = true;

            for (int i = 1; i <= Copies; i++)
            {
                string StrOutPut = GetPrintOut(_strRefText, _strRef, PrintHeader(), PrintDetails(), PrintFooter(), PrintReturn());
                //dsMain.Tables[1].Clear();
                //string StrOutPutCashOut = GetPrintOut1(_strRefText, _strRef, PrintHeader1(), PrintDetails1(), PrintFooter1());
                //dsMain1.Clear();

                StreamWriter sr = new StreamWriter("d:\\bill.txt");
                sr.Write(StrOutPut);
                sr.Close();

            }



            new Process
            {
                StartInfo =
                {
                    FileName = "d:\\Bill.bat",
                    WindowStyle = ProcessWindowStyle.Hidden

                }
            }.Start();
        }
        private string GetPrintOut(string strRefText, string strRef, string Header, string Detail, string Footer, string Return)
        {
            int InitialReverseFeed = 3;
            int PageLength = 36;
            int TopMargin = 3;
            int BottomMargin = 6;
            //string _ReverseFeed = '\u001b' + "j" + 'l';
            string _ReverseFeed = '\u001b' + "j" + 'l';
            int i = 0;
            int LinesCount;

            string[] sHeader = Header.Split('\n');
            string[] sFooter = Footer.Split('\n');
            string[] sDetail = Detail.Split('\n');
            string[] sReturn = Return.Split('\n');
            StringBuilder sb = new StringBuilder();

            // Initial Reverse Feed Characters

            int PreviousPage = 0;
            int CurrentPage = 1;
            int CurrentLine = 1;

            for (i = 0; i < InitialReverseFeed; i++)
            {
                sb.AppendLine(_ReverseFeed);

            }
            sb.AppendLine("");
            CurrentLine = 5;
            // First Header 
            for (i = 0; i < sHeader.Length; i++)
            {
                sb.AppendLine(sHeader[i]);
            }
            CurrentLine += sHeader.Length;

            bool flag = true;
            for (i = 0; i < sDetail.Length; i++)
            {
                if (CurrentLine == TopMargin + 1)
                {

                    sb.AppendLine(GetRightFormatedText("", 80));
                    CurrentLine++;
                }
                sb.AppendLine(sDetail[i]);
                CurrentLine++;
                // Identify the end of current page
                if (CurrentLine > PageLength - BottomMargin)
                {
                    // add bottom margin lines

                    sb.AppendLine(GetRightFormatedText("Contd.." + (CurrentPage + 1).ToString(), 80));

                    for (int j = 1; j < BottomMargin; j++)
                    {
                        //sb.AppendLine("b"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentPage++;
                    CurrentLine = 0; // intialize the current line number
                    // add top margin to the next page
                    for (int j = 1; j <= TopMargin; j++)
                    {
                        //sb.AppendLine("t"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentLine = TopMargin + 1;
                }

            }

            // Print return product

            for (i = 0; i < sReturn.Length; i++)
            {
                if (CurrentLine == TopMargin + 1)
                {

                    sb.AppendLine(GetRightFormatedText("", 80));
                    CurrentLine++;
                }
                sb.AppendLine(sReturn[i]);
                CurrentLine++;
                // Identify the end of current page
                if (CurrentLine > PageLength - BottomMargin)
                {
                    // add bottom margin lines

                    sb.AppendLine(GetRightFormatedText("Contd.." + (CurrentPage + 1).ToString(), 80));

                    for (int j = 1; j < BottomMargin; j++)
                    {
                        //sb.AppendLine("b"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentPage++;
                    CurrentLine = 0; // intialize the current line number
                    // add top margin to the next page
                    for (int j = 1; j <= TopMargin; j++)
                    {
                        //sb.AppendLine("t"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentLine = TopMargin + 1;
                }

            }

            // Print footer 
            if ((PageLength - CurrentLine - BottomMargin) < sFooter.Length)
            {
                // eject current page
                for (i = PageLength - CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");

                }
                // add top margin
                // eject current page
                for (i = 1; i <= TopMargin; i++)
                {
                    sb.AppendLine(" ");
                }
                sb.AppendLine(GetRightFormatedText(strRefText + strRef, 80));
                for (i = 0; i < sFooter.Length - 1; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 7; i++)
                {
                    sb.AppendLine(" ");
                }
            }
            else
            {
                LinesCount = PageLength - CurrentLine - BottomMargin - sFooter.Length;
                for (i = 1; i <= LinesCount + 2; i++)
                {
                    sb.AppendLine(" ");
                    CurrentLine++;
                }
                for (i = 0; i < sFooter.Length; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                    CurrentLine++;
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = 1; i <= BottomMargin; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 9; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }

            }

            CurrentLine = TopMargin;
            return sb.ToString();
        }


        private string GetPrintOut1(string strRefText, string strRef, string Header, string Detail, string Footer)
        {
            int InitialReverseFeed = 3;
            int PageLength = 36;
            int TopMargin = 0;
            int BottomMargin = 6;
            //string _ReverseFeed = '\u001b' + "j" + 'l';
            string _ReverseFeed = '\u001b' + "j" + 'l';
            int i = 0;
            int LinesCount;

            string[] sHeader = Header.Split('\n');
            string[] sFooter = Footer.Split('\n');
            string[] sDetail = Detail.Split('\n');
            StringBuilder sb = new StringBuilder();

            // Initial Reverse Feed Characters

            int PreviousPage = 0;
            int CurrentPage = 1;
            int CurrentLine = 1;

            for (i = 0; i < InitialReverseFeed; i++)
            {
                sb.AppendLine(_ReverseFeed);

            }
            sb.AppendLine("");
            CurrentLine = 5;
            // First Header 
            for (i = 0; i < sHeader.Length; i++)
            {
                sb.AppendLine(sHeader[i]);
            }
            CurrentLine += sHeader.Length;

            bool flag = true;
            for (i = 0; i < sDetail.Length; i++)
            {
                if (CurrentLine == TopMargin + 1)
                {

                    sb.AppendLine(GetRightFormatedText("", 80));
                    CurrentLine++;
                }
                sb.AppendLine(sDetail[i]);
                CurrentLine++;
                // Identify the end of current page
                if (CurrentLine > PageLength - BottomMargin)
                {
                    // add bottom margin lines

                    sb.AppendLine(GetRightFormatedText("Contd.." + (CurrentPage + 1).ToString(), 80));

                    for (int j = 1; j < BottomMargin; j++)
                    {
                        //sb.AppendLine("b"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentPage++;
                    CurrentLine = 0; // intialize the current line number
                    // add top margin to the next page
                    for (int j = 1; j <= TopMargin; j++)
                    {
                        //sb.AppendLine("t"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentLine = TopMargin + 1;
                }

            }
            // Print footer 
            if ((PageLength - CurrentLine - BottomMargin) < sFooter.Length)
            {
                // eject current page
                for (i = PageLength - CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");

                }
                // add top margin
                // eject current page
                for (i = 1; i <= TopMargin; i++)
                {
                    sb.AppendLine(" ");
                }
                sb.AppendLine(GetRightFormatedText(strRefText + strRef, 80));
                for (i = 0; i < sFooter.Length - 1; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 7; i++)
                {
                    sb.AppendLine(" ");
                }
            }
            else
            {
                LinesCount = PageLength - CurrentLine - BottomMargin - sFooter.Length;
                for (i = 1; i <= LinesCount + 2; i++)
                {
                    sb.AppendLine(" ");
                    CurrentLine++;
                }
                for (i = 0; i < sFooter.Length; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                    CurrentLine++;
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = 1; i <= BottomMargin; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 9; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }

            }

            CurrentLine = TopMargin;
            return sb.ToString();
        }
        public void pagebreak()
        {
            int i = 1;
            int j = 6;
            while (j >= i)
            {
                if (i == 6)
                {
                    this.rdr.WriteLine("");
                }
                else
                {
                    this.rdr.WriteLine("");
                }
                i++;
            }
        }

        public void pageEnd()
        {
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine(".");
        }

        public string PrintHeader()
        {
            string strPrintHeader = "";
            if (this.dsMain.Tables[0].Rows.Count > 0)
            {
                strPrintHeader = (char)18 + this.GetCenterdFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["CompanyName"]), 50) + "\n";
                strPrintHeader += (char)(15) + this.GetCenterdFormatedText(this.dsMain.Tables[0].Rows[0]["DoorNo"].ToString() + this.dsMain.Tables[0].Rows[0]["Address1"].ToString() + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Phone1"].ToString()), 80) + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += (char)18 + this.GetCenterdFormatedText("QUOTATION", 50) + "\n" + (char)15;
                strPrintHeader += "Date : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Date"]), 33);
                strPrintHeader += this.GetRightFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["QuotationId"]), 40) + "\n";
                strPrintHeader += "Customer Name : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["customername"]), 50) + "\n";
                strPrintHeader += PrintLine() + "\n";
                strPrintHeader += this.GetFormatedText("SNO", 3) + " ";
                strPrintHeader += this.GetFormatedText("PRODUCT NAME", 30) + " ";
                strPrintHeader += this.GetFormatedText("QUANTITY", 12) + " ";
                strPrintHeader += this.GetRightFormatedText("RATE", 9) + " ";
                strPrintHeader += this.GetRightFormatedText("AMOUNT", 9) + " ";
                strPrintHeader += this.GetFormatedText("", 12);
                strPrintHeader += " " + "\n";
                strPrintHeader += PrintLine();
            }
         
            return strPrintHeader;
        }

         public string PrintHeader1()
        {
            string strPrintHeader = "";
            if (this.dsMain1.Tables[2].Rows.Count > 0)
            {
                strPrintHeader += (char)18 + this.GetFormatedText("Return products", 50) + "\n" + (char)15;
                strPrintHeader += this.GetFormatedText("SNO", 3) + " ";
                strPrintHeader += this.GetFormatedText("PRODUCT NAME", 30) + " ";
                strPrintHeader += this.GetFormatedText("QUANTITY", 12) + " ";
                strPrintHeader += this.GetFormatedText("", 12);
                strPrintHeader += " " + "\n";
                strPrintHeader += PrintLine();
            }
         
            return strPrintHeader;
        }

         

        public string PrintDetails()
        {
            string PrintDetails = "";
            if (this.dsMain.Tables[1].Rows.Count > 0)
            {
                while (this.dsMain.Tables[1].Rows.Count >= this.slno)
                {
                    PrintDetails += this.GetFormatedText(this.slno.ToString(), 3) + " ";
                    PrintDetails += this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["ItemName"].ToString(), 30) + " ";
                    PrintDetails += this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Quantity"].ToString(), 8) + " ";
                    PrintDetails += this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["UOM"].ToString(), 3) + " ";
                    PrintDetails += this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Rate"].ToString(), 9) + " ";
                    PrintDetails += this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Amount"].ToString(), 9) + " ";
                    PrintDetails += this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Rack"].ToString(), 15);
                    PrintDetails += " \n";
                    this.Total += Convert.ToDecimal(this.dsMain.Tables[1].Rows[this.slno - 1]["Amount"]);
                    this.slno++;
                }
            }
           
            return PrintDetails + "\n";
        }

        public string PrintReturn()
        {
            string PrintDetails = "";
            if (this.dsMain.Tables[2].Rows.Count > 0)
            {
                this.slno1 = 1;
                PrintDetails = this.GetFormatedText("RETURN PRODUCTS", 77) + " ";
                PrintDetails += " \n";
                PrintDetails += this.GetFormatedText("---------------", 77) + " ";
                PrintDetails += " \n";

                while (this.dsMain.Tables[2].Rows.Count >= this.slno1)
                {
                    PrintDetails += this.GetFormatedText(this.slno1.ToString(), 3) + " ";
                    PrintDetails += this.GetFormatedText(this.dsMain.Tables[2].Rows[this.slno1 - 1]["ItemName"].ToString(), 30) + " ";
                    PrintDetails += this.GetRightFormatedText(this.dsMain.Tables[2].Rows[this.slno1 - 1]["Quantity"].ToString(), 8) + " ";
                    PrintDetails += this.GetFormatedText(this.dsMain.Tables[2].Rows[this.slno1 - 1]["UOM"].ToString(), 3) + " ";
                    PrintDetails += this.GetRightFormatedText(this.dsMain.Tables[2].Rows[this.slno1 - 1]["Rate"].ToString(), 9) + " ";
                    PrintDetails += this.GetRightFormatedText(this.dsMain.Tables[2].Rows[this.slno1 - 1]["Amount"].ToString(), 9) + " ";
                    PrintDetails += this.GetFormatedText(this.dsMain.Tables[2].Rows[this.slno1 - 1]["Rack"].ToString(), 15);
                    PrintDetails += " \n";
                    this.Total += Convert.ToDecimal(this.dsMain.Tables[2].Rows[this.slno1 - 1]["Amount"]);
                    this.slno1++;
                }
            }

            return PrintDetails + "\n";
        }

        public string PrintDetails1()
        {
            string PrintDetails = "";
           
            if (this.dsMain1.Tables[2].Rows.Count > 0)
            {
                while (this.dsMain1.Tables[2].Rows.Count >= this.slno1)
                {
                    PrintDetails += this.GetFormatedText(this.slno1.ToString(), 3) + " ";
                    PrintDetails += this.GetFormatedText(this.dsMain1.Tables[2].Rows[this.slno1 - 1]["ItemName"].ToString(), 30) + "";
                    PrintDetails += this.GetRightFormatedText(this.dsMain1.Tables[2].Rows[this.slno1 - 1]["Quantity"].ToString(), 8) + " \n ";
                    this.slno1++;
                }
            }
            return PrintDetails + "\n";
        }
        public string PrintFooter()
        {
            string strPrintFooter = "";
            char[] trimChars = new char[]
			{
				'-',
				' '
			};
            if (this.dsMain.Tables[1].Rows.Count > 0)
            {
                string value = Convert.ToString(this.nagtoatl).TrimStart(trimChars);
                strPrintFooter += PrintLine() + "\n";
                strPrintFooter += this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Assist"].ToString()), 15);
                strPrintFooter += this.GetFormatedText(Convert.ToString("Pur:"), 4);
                strPrintFooter += this.GetFormatedText(Convert.ToString(this.postoatl), 13);
                strPrintFooter += this.GetFormatedText(Convert.ToString(" Rtn:"), 5);
                strPrintFooter += this.GetFormatedText(Convert.ToString(value), 13);
                strPrintFooter += this.GetRightFormatedText(Convert.ToString("ToT:"), 4);
                strPrintFooter += this.GetFormatedText(Convert.ToString(this.Total), 13) + "\n"; 
            }

            return strPrintFooter;

        }

        public string PrintFooter1()
        {
            string strPrintFooter = "";
            char[] trimChars = new char[]
			{
				'-',
				' '
			};
            
                string value = Convert.ToString(this.nagtoatl).TrimStart(trimChars);
                strPrintFooter += "\n\n\n\n\n\n";
              
           

            return strPrintFooter;

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
        public string PrintLine()
        {
            string str = "";
            for (int i = 1; i <= 80; i++)
            {
                str += "-";
            }
            return str;
        }

    }


    class CashEndCloseReport
    {
        public DataTable dsMain;
        public DataTable dsMain1;
        public DataTable dsMain2;
        private decimal Total = 0.0m;
        private decimal Total1 = 0.0m;
        private StreamWriter rdr;
        private string Bold_On = "_G";
        private string Bold_Off = "_H";
        private string Width_On = "_W1";
        private string Width_Off = "_W0";
        private string ELITE_PITCH = "_M";
        private string Compress_Off = "_";
        private int ColWidth = 60;
        public string BillType = "Cash-In";
        public string BillType1 = "Cash-Out";
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
        public int pagenumber;
        public bool status;
        public int fpage;
        public int lpage;
        private int slno = 1;
        private int slno1 = 1;
        public int Copies = 1;
        private decimal postoatl = 0.0m;
        private decimal nagtoatl = 0.0m;
        private int pagerecno = 0;
        private bool headerflag = false;
        private bool showprevsum = false;
        public string _strRefText;
        public string _strRef;
        public DataTable dsa;
        public string strTypevalue;
        public string strTypevalue1;


        public int linecount = 0;
        public int value = 0;


        public bool Party = false;

        private bool firstpage = true;

        public CashEndCloseReport()
        {
            dsa = dsMain2;
        }






        public void CashEndClosePrint()
        {
            bool result = true;
            this.fileName = "d:\\Bill.txt";
            FileStream fileStream = File.Create(this.fileName);
            fileStream.Close();
            using (this.rdr = new StreamWriter(this.fileName))
            {
                for (int i = 1; i <= Copies; i++)
                {

                    PrintCloseEndHeader();
                    PrintCloseEndDetails();
                    PrintCloseEndFooter();
                }

            }
            this.rdr.Close();
            new Process
            {
                StartInfo =
                {
                    FileName = "d:\\Bill.bat",
                    WindowStyle = ProcessWindowStyle.Hidden

                }
            }.Start();

        }
        public void PrintCloseEndHeader()
        {
            this.rdr.WriteLine((char)27 + "jn");
            this.rdr.WriteLine((char)27 + "jn");
            this.rdr.WriteLine((char)27 + "jn");

            this.rdr.WriteLine((char)27 + "F" + this.GetCenterdFormatedText(BillType, 50));


            this.PrintLine();
            this.rdr.Write("{0,-3}", this.GetFormatedText("Reference", 3) + " ");
            this.rdr.Write("{0,-50}", this.GetFormatedText("Particulars", 50) + " ");
            this.rdr.Write("{0,-12}", this.GetFormatedText("Amount", 12) + " ");

            this.rdr.WriteLine("");
            this.PrintLine();
        }
        public void PrintCloseEndDetails()
        {

            while (this.dsMain.Rows.Count > 0)
            {

                this.rdr.Write("{0,-3}", this.GetFormatedText(this.slno.ToString(), 3) + " ");
                this.rdr.Write("{0,-50}", this.GetFormatedText(this.dsMain.Rows[this.slno - 1]["ItemName"].ToString(), 50) + " ");
                this.rdr.Write("{0,-8}", this.GetRightFormatedText(this.dsMain.Rows[this.slno - 1]["Quantity"].ToString(), 8) + " ");
                this.rdr.Write("{0,-3}", this.GetFormatedText(this.dsMain.Rows[this.slno - 1]["UOM"].ToString(), 3) + " ");
                this.rdr.Write("{0,-9}", this.GetRightFormatedText(this.dsMain.Rows[this.slno - 1]["Rate"].ToString(), 9) + " ");
                this.rdr.Write("{0,-9}", this.GetRightFormatedText(this.dsMain.Rows[this.slno - 1]["Amount"].ToString(), 9) + " ");
                this.rdr.WriteLine("");
                this.Total += Convert.ToDecimal(this.dsMain.Rows[this.slno - 1]["Amount"]);
                this.slno++;
            }

        }
        public void PrintCloseEndFooter()
        {
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine(this.GetRightFormatedText("-------------------", 90) + " ");
            if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Rows[0]["ReturnSales"])))
            {
                this.rdr.Write(this.GetFormatedText("     " + Convert.ToString(this.dsMain.Rows[0]["ReturnSales"]), 60));
                this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 27));
                this.rdr.WriteLine("");
            }
            else
            {
                this.rdr.Write(this.GetFormatedText("     TOTAL", 30));
                this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 57));
                this.rdr.WriteLine("");
            }
            if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Rows[0]["LessAmount"])))
            {
                if (Convert.ToDouble(this.dsMain.Rows[0]["LessAmount"]) > 0.0)
                {
                    this.rdr.Write(this.GetFormatedText("     LESS AMOUNT", 30));
                    this.rdr.Write(this.GetRightFormatedText("-" + Convert.ToString(this.dsMain.Rows[0]["LessAmount"]), 57));
                    this.rdr.WriteLine("");
                }
            }
            if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Rows[0]["OtherCharges"])))
            {
                if (Convert.ToDouble(this.dsMain.Rows[0]["OtherCharges"]) > 0.0)
                {
                    this.rdr.Write(this.GetFormatedText("     OTHERS", 30));
                    this.rdr.Write(this.GetRightFormatedText("-" + Convert.ToString(this.dsMain.Rows[0]["OtherCharges"]), 57));
                    this.rdr.WriteLine("");
                }
            }
            this.rdr.Write(this.GetFormatedText("     GRAND TOTAL", 30));
            this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.dsMain.Rows[0]["GrnandTotal"]), 47));
            this.rdr.WriteLine("");
            this.rdr.Write(this.GetFormatedText(Convert.ToString(this.dsMain.Rows[0]["Assist"]), 40));
            this.rdr.WriteLine("");
            this.rdr.Write(this.GetCenterdFormatedText("Check the goods carefully before taking delivery", 90));
            this.rdr.WriteLine("");
            this.rdr.Write(this.GetFormatedText("             No guarantee for glass fittings & bulbs. Company service guarantee only", 90));
            this.rdr.WriteLine("");
            this.rdr.Write(this.GetFormatedText("               for guarantee items. Goods once sold cannot be taken back or exchanged.", 90));
            this.rdr.WriteLine("" + (char)18);
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
        public void CashEndClosePrintStatemnet()
        {
            bool result = true;

            for (int i = 1; i <= Copies; i++)
            {


                string StrOutPutCashIn = GetPrintOut(_strRefText, PrintHeader(), PrintDetails(), PrintFooter());
                dsMain.Clear();
                string StrOutPutCashOut = GetPrintOut(_strRefText, PrintHeader(), PrintDetails(), PrintFooter());
                dsMain1.Clear();
                StreamWriter sr = new StreamWriter("d:\\bill.txt");
                sr.Write(StrOutPutCashIn + "\n\n" + StrOutPutCashOut);
                sr.Close();

            }



            new Process
            {
                StartInfo =
                {
                    FileName = "d:\\Bill.bat",
                    WindowStyle = ProcessWindowStyle.Hidden

                }
            }.Start();
        }
        private string GetPrintOut(string strRefText, string Header, string Detail, string Footer)
        {
            int InitialReverseFeed = 3;
            int PageLength = 36;
            int TopMargin = 1;
            int BottomMargin = 3;
            //string _ReverseFeed = '\u001b' + "j" + 'l';
            string _ReverseFeed = '\u001b' + "j" + 'l';
            int i = 0;
            int LinesCount;

            string[] sHeader = Header.Split('\n');
            string[] sFooter = Footer.Split('\n');
            string[] sDetail = Detail.Split('\n');
            StringBuilder sb = new StringBuilder();

            // Initial Reverse Feed Characters

            int PreviousPage = 0;
            int CurrentPage = 1;
            int CurrentLine = 1;

            for (i = 0; i < InitialReverseFeed; i++)
            {
                sb.AppendLine(_ReverseFeed);

            }
            sb.AppendLine("");
            CurrentLine = 5;
            // First Header 
            for (i = 0; i < sHeader.Length; i++)
            {
                sb.AppendLine(sHeader[i]);
            }
            CurrentLine += sHeader.Length;

            bool flag = true;
            for (i = 0; i < sDetail.Length; i++)
            {
                if (CurrentLine == TopMargin + 1)
                {

                    sb.AppendLine(GetRightFormatedText("", 80));
                    CurrentLine++;
                }
                sb.AppendLine(sDetail[i]);
                CurrentLine++;
                // Identify the end of current page
                if (CurrentLine > PageLength - BottomMargin)
                {
                    // add bottom margin lines

                    sb.AppendLine(GetRightFormatedText("Contd.." + (CurrentPage + 1).ToString(), 80));

                    for (int j = 1; j < BottomMargin; j++)
                    {
                        //sb.AppendLine("b"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentPage++;
                    CurrentLine = 0; // intialize the current line number
                    // add top margin to the next page
                    for (int j = 1; j <= TopMargin; j++)
                    {
                        //sb.AppendLine("t"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentLine = TopMargin + 1;
                }

            }
            // Print footer 
            if ((PageLength - CurrentLine - BottomMargin) < sFooter.Length)
            {
                // eject current page
                for (i = PageLength - CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");

                }
                // add top margin
                // eject current page
                for (i = 1; i <= TopMargin; i++)
                {
                    sb.AppendLine(" ");
                }
                sb.AppendLine(GetRightFormatedText(strRefText, 80));
                for (i = 0; i < sFooter.Length - 1; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 7; i++)
                {
                    sb.AppendLine(" ");
                }
            }
            else
            {
                //LinesCount = PageLength - CurrentLine - BottomMargin - sFooter.Length;
                //for (i = 1; i <= LinesCount + 2; i++)
                //{
                //    sb.AppendLine(" ");
                //    CurrentLine++;
                //}
                for (i = 0; i < sFooter.Length; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                    CurrentLine++;
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = 1; i <= BottomMargin; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 9; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }

            }

            CurrentLine = TopMargin;
            return sb.ToString();
        }
        public void pagebreak()
        {
            int i = 1;
            int j = 6;
            while (j >= i)
            {
                if (i == 6)
                {
                    this.rdr.WriteLine("");
                }
                else
                {
                    this.rdr.WriteLine("");
                }
                i++;
            }
        }

        public void pageEnd()
        {
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine(".");
        }

        public string PrintHeader()
        {
            string strPrintHeader = "";
            if (this.dsMain.Rows.Count > 0)
            {

                strPrintHeader += (char)18 + this.GetCenterdFormatedText("Cash-In", 50) + "\n" + (char)15;

                strPrintHeader += this.GetFormatedText("", 12);
                strPrintHeader += " " + "\n";
                strPrintHeader += PrintLine() + "\n";

                strPrintHeader += this.GetFormatedText("SNO", 3) + " ";
                strPrintHeader += this.GetFormatedText("Reference", 30) + " ";
                strPrintHeader += this.GetFormatedText("Particulars", 30) + " ";
                strPrintHeader += this.GetRightFormatedText("Amount", 9) + " ";

                strPrintHeader += this.GetFormatedText("", 12);
                strPrintHeader += " " + "\n";
                strPrintHeader += PrintLine();
            }
            else
            {

                strPrintHeader += (char)18 + this.GetCenterdFormatedText("Cash-Out", 50) + "\n" + (char)15;

                strPrintHeader += PrintLine() + "\n";

                strPrintHeader += this.GetFormatedText("SNO", 3) + " ";
                strPrintHeader += this.GetFormatedText("Reference", 30) + " ";
                strPrintHeader += this.GetFormatedText("Particulars", 30) + " ";
                strPrintHeader += this.GetRightFormatedText("Amount", 9) + " " + "\n";

                strPrintHeader += " " + "\n";

                strPrintHeader += PrintLine();



            }
            return strPrintHeader;
        }
        public string PrintDetails()
        {
            string PrintDetails = "";
            if (this.dsMain.Rows.Count > 0)
            {
                while (this.dsMain.Rows.Count >= this.slno)
                {
                    PrintDetails += this.GetFormatedText(this.slno.ToString(), 3) + " ";
                    PrintDetails += this.GetFormatedText(this.dsMain.Rows[this.slno - 1]["EntityId"].ToString(), 30) + " ";
                    PrintDetails += this.GetFormatedText(this.dsMain.Rows[this.slno - 1]["Type"].ToString(), 30) + " ";

                    PrintDetails += this.GetRightFormatedText(this.dsMain.Rows[this.slno - 1]["Amount"].ToString(), 8) + " ";

                    PrintDetails += " \n";
                    this.Total += Convert.ToDecimal(this.dsMain.Rows[this.slno - 1]["Amount"]);
                    this.slno++;
                }
            }
            else
            {

                while (this.dsMain1.Rows.Count >= this.slno1)
                {
                    PrintDetails += this.GetFormatedText(this.slno1.ToString(), 3) + " ";
                    PrintDetails += this.GetFormatedText(this.dsMain1.Rows[this.slno1 - 1]["EntityId"].ToString(), 30) + " ";
                    PrintDetails += this.GetFormatedText(this.dsMain1.Rows[this.slno1 - 1]["Type"].ToString(), 30) + " ";

                    PrintDetails += this.GetRightFormatedText(this.dsMain1.Rows[this.slno1 - 1]["Amount"].ToString(), 8) + " ";

                    PrintDetails += " \n";
                    this.Total1 += Convert.ToDecimal(this.dsMain1.Rows[this.slno1 - 1]["Amount"]);
                    this.slno1++;
                }
            }
            return PrintDetails + "\n";
        }
        public string PrintFooter()
        {
            string strPrintFooter = "";
            char[] trimChars = new char[]
			{
				'-',
				' '
			};
            if (this.dsMain.Rows.Count > 0)
            {
                string value = Convert.ToString(this.nagtoatl).TrimStart(trimChars);
                strPrintFooter += PrintLine() + "\n";
                strPrintFooter += this.GetFormatedText(Convert.ToString("Total"), 30);

                strPrintFooter += this.GetRightFormatedText(Convert.ToString(this.Total), 44) + "\n";

                strPrintFooter += " " + "\n";
                strPrintFooter += PrintLine();

            }
            else
            {
                string value = Convert.ToString(this.nagtoatl).TrimStart(trimChars);
                strPrintFooter += PrintLine() + "\n";
                strPrintFooter += this.GetFormatedText(Convert.ToString("Total"), 30);

                strPrintFooter += this.GetRightFormatedText(Convert.ToString(this.Total1), 44) + "\n";

                strPrintFooter += " " + "\n";
                strPrintFooter += PrintLine();
                strPrintFooter += " " + "\n";
                strPrintFooter += this.GetFormatedText(Convert.ToString("Total cash-in"), 30);
                strPrintFooter += this.GetRightFormatedText(Convert.ToString(this.Total), 44) + "\n";
                strPrintFooter += this.GetFormatedText(Convert.ToString("Total cash-out"), 30);
                strPrintFooter += this.GetRightFormatedText(Convert.ToString(this.Total1), 44) + "\n";
                strPrintFooter += this.GetFormatedText(Convert.ToString("Closing balance"), 30);
                strPrintFooter += this.GetRightFormatedText(Convert.ToString(this.Total - this.Total1), 44) + "\n";
            }

            return strPrintFooter;

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
        public string PrintLine()
        {
            string str = "";
            for (int i = 1; i <= 80; i++)
            {
                str += "-";
            }
            return str;
        }

    }

    class PurchaseOrderReport
    {
        public DataSet dsMain;
        private decimal Total = 0.0m;
        private decimal Total1 = 0.0m;
        private StreamWriter rdr;
        private string Bold_On = "_G";
        private string Bold_Off = "_H";
        private string Width_On = "_W1";
        private string Width_Off = "_W0";
        private string ELITE_PITCH = "_M";
        private string Compress_Off = "_";
        private int ColWidth = 60;
        public string BillType = "PURCHASEORDER";
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
        public int pagenumber;
        public bool status;
        public int fpage;
        public int lpage;
        private int slno = 1;
        private int slno1 = 1;
        public int Copies = 1;
        private decimal postoatl = 0.0m;
        private decimal nagtoatl = 0.0m;
        private int pagerecno = 0;
        private bool headerflag = false;
        private bool showprevsum = false;
        public string strRefTexts;
        public string strRefs;




        public int linecount = 0;
        public int value = 0;


        public bool Party = false;

        private bool firstpage = true;

        public PurchaseOrderReport()
        {

        }

        public void RREPrintPurchaseOrder()
        {
            bool result = true;

            for (int i = 1; i <= Copies; i++)
            {
                string StrOutPuts = GetPrintOuts(strRefTexts, strRefs, PrintPurchaseorderHeader(), PrintPurchaseorderDetails(), PrintPurchaseOrderFooter());
                StreamWriter sr = new StreamWriter("d:\\bill.txt");
                sr.Write(StrOutPuts);
                sr.Close();

            }



            new Process
            {
                StartInfo =
                {
                    FileName = "d:\\Bill.bat",
                    WindowStyle = ProcessWindowStyle.Hidden

                }
            }.Start();


        }
        public string PrintPurchaseorderHeader()
        {
            string strPrintHeader = "";
            if (this.dsMain.Tables[0].Rows.Count > 0)
            {
                strPrintHeader += (char)18 + this.GetCenterdFormatedText("PURCHASE  ORDER", 40) + "\n\n" + (char)15;
                strPrintHeader += "ORDER NO : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["OrderNumber"]), 50);
                strPrintHeader += "DATE : " + this.GetFormatedText(DateTime.Now.ToString("dd/MM/yyyy"), 25) + "\n";
                strPrintHeader += PrintLine() + "\n";
                strPrintHeader += this.GetFormatedText("SNo", 3) + " ";
                strPrintHeader += this.GetFormatedText("PRODUCT NAME", 40) + " ";
                strPrintHeader += this.GetFormatedText("QUANTITY", 10) + " " + "\n";
                // strPrintHeader += PrintLine() + "\n";

                strPrintHeader += PrintLine();
            }
            return strPrintHeader;
        }
        public string PrintPurchaseorderDetails()
        {
            string PrintDetails = "";
            while (this.dsMain.Tables[0].Rows.Count >= this.slno)
            {
                PrintDetails += this.GetFormatedText(this.slno.ToString(), 3) + " ";
                PrintDetails += this.GetFormatedText(this.dsMain.Tables[0].Rows[this.slno - 1]["Item"].ToString(), 30) + " ";
                PrintDetails += this.GetRightFormatedText(this.dsMain.Tables[0].Rows[this.slno - 1]["Quantity"].ToString(), 8) + " ";
                PrintDetails += this.GetFormatedText(this.dsMain.Tables[0].Rows[this.slno - 1]["UOM"].ToString(), 3) + " ";


                PrintDetails += " \n";

                this.slno++;
            }
            return PrintDetails + "\n";

        }
        public string PrintPurchaseOrderFooter()
        {
            string strPrintFooter = "";

            for (int i = this.pagerecno; i <= 16; i++)
            {
                strPrintFooter = " \n";
            }
            strPrintFooter += PrintLine() + "\n\n";
            // this.PrintFooter();
            // strPrintFooter + = this.GetRightFormatedText("Sign :------------------------", 80);
            strPrintFooter += this.GetRightFormatedText("Sign :------------------------", 80);
            return strPrintFooter;
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
        public string PrintLine()
        {
            string str = "";
            for (int i = 1; i <= 80; i++)
            {
                str += "-";
            }
            return str;
        }
        private string GetPrintOuts(string strRefTexts, string strRefs, string Header, string Detail, string Footer)
        {
            int InitialReverseFeed = 3;
            int PageLength = 36;
            int TopMargin = 3;
            int BottomMargin = 4;
            //string _ReverseFeed = '\u001b' + "j" + 'l';
            string _ReverseFeed = '\u001b' + "j" + 'l';
            int i = 0;
            int LinesCount;

            string[] sHeader = Header.Split('\n');
            string[] sFooter = Footer.Split('\n');
            string[] sDetail = Detail.Split('\n');
            StringBuilder sb = new StringBuilder();

            // Initial Reverse Feed Characters

            int PreviousPage = 0;
            int CurrentPage = 1;
            int CurrentLine = 1;

            for (i = 0; i < InitialReverseFeed; i++)
            {
                sb.AppendLine(_ReverseFeed);

            }
            sb.AppendLine("");
            CurrentLine = 5;
            // First Header 
            for (i = 0; i < sHeader.Length; i++)
            {
                sb.AppendLine(sHeader[i]);
            }
            CurrentLine += sHeader.Length;

            bool flag = true;
            for (i = 0; i < sDetail.Length; i++)
            {
                if (CurrentLine == TopMargin + 1)
                {
                    sb.AppendLine(GetRightFormatedText(strRefTexts + strRefs, 80));
                    CurrentLine++;
                }
                sb.AppendLine(sDetail[i]);
                CurrentLine++;
                // Identify the end of current page
                if (CurrentLine > PageLength - BottomMargin)
                {
                    // add bottom margin lines
                    sb.AppendLine(GetRightFormatedText("Contd.." + (CurrentPage + 1).ToString(), 80));

                    for (int j = 1; j < BottomMargin; j++)
                    {
                        //sb.AppendLine("b"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentPage++;
                    CurrentLine = 0; // intialize the current line number
                    // add top margin to the next page
                    for (int j = 1; j <= TopMargin; j++)
                    {
                        //sb.AppendLine("t"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentLine = TopMargin + 1;
                }

            }
            // Print footer 
            if ((PageLength - CurrentLine - BottomMargin) <= sFooter.Length)
            {
                // eject current page
                for (i = CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");

                }
                // add top margin
                // eject current page
                for (i = 1; i <= TopMargin; i++)
                {
                    sb.AppendLine(" ");
                }
                CurrentLine = TopMargin + 1;
                sb.AppendLine(GetRightFormatedText(strRefTexts + strRefs, 80));
                sb.AppendLine(" ");
                sb.AppendLine(" ");
                CurrentLine++;
                CurrentLine++;
                for (i = 0; i < sFooter.Length - 1; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 8; i++)
                {
                    sb.AppendLine(" ");
                }
            }
            else
            {
                LinesCount = PageLength - CurrentLine - BottomMargin - sFooter.Length;
                for (i = 1; i <= LinesCount + 2; i++)
                {
                    sb.AppendLine(" ");
                    CurrentLine++;
                }
                for (i = 0; i < sFooter.Length; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                    CurrentLine++;
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = 1; i <= BottomMargin; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 10; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }

            }

            CurrentLine = TopMargin;
            return sb.ToString();
        }
    }
    class VoucherReport
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
        public string BillType = "VOCHER";
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
        public int pagenumber;
        public bool status;
        public int fpage;
        public int lpage;
        private int slno = 1;
        public int Copies = 1;
        private decimal postoatl = 0.0m;
        private decimal nagtoatl = 0.0m;
        private int pagerecno = 0;
        private bool headerflag = false;
        private bool showprevsum = false;
        public string _strRefText;
        public string _strRef;




        public int linecount = 0;
        public int value = 0;


        public bool Party = false;

        private bool firstpage = true;

        public VoucherReport()
        {
        }
        public void RREPrintReuest()
        {
            bool result = true;


            string StrOutPut = GetPrintOuts(_strRefText, _strRef, PrintHeaders(), Printdetails(), PrintFooter());
            //string StrOutPut = '\u001b' + "j" + 'l' + '\u001b' + "j" + 'l' + '\u001b' + "j" + 'l';
            //StrOutPut += "test";
            //StrOutPut+= (char)15 + "print 2\n";
            //StrOutPut+= (char)18 + "print 2\n";

            //StrOutPut = (char)(15) + StrOutPut + (char)27 + "F";
            //StrOutPut = StrOutPut + (char)18;

            StreamWriter sr = new StreamWriter("d:\\bill.txt");
            sr.Write(StrOutPut);
            sr.Close();





            new Process
            {
                StartInfo =
                {
                    FileName = "d:\\Bill.bat",
                    WindowStyle = ProcessWindowStyle.Hidden

                }
            }.Start();
        }
        private string GetPrintOuts(string strRefTexts, string strRefs, string Headers, string Details, string Footer)
        {
            int InitialReverseFeed = 3;
            int PageLength = 36;
            int TopMargin = 3;
            int BottomMargin = 4;
            //string _ReverseFeed = '\u001b' + "j" + 'l';
            string _ReverseFeed = '\u001b' + "j" + 'l';
            int i = 0;
            int LinesCount;

            string[] sHeader = Headers.Split('\n');
            string[] sFooter = Footer.Split('\n');
            string[] sDetail = Details.Split('\n');
            StringBuilder sb = new StringBuilder();

            // Initial Reverse Feed Characters

            int PreviousPage = 0;
            int CurrentPage = 1;
            int CurrentLine = 1;

            for (i = 0; i < InitialReverseFeed; i++)
            {
                sb.AppendLine(_ReverseFeed);

            }
            sb.AppendLine("");
            CurrentLine = 5;
            // First Header 
            for (i = 0; i < sHeader.Length; i++)
            {
                sb.AppendLine(sHeader[i]);
            }
            CurrentLine += sHeader.Length;

            bool flag = true;
            for (i = 0; i < sDetail.Length; i++)
            {
                if (CurrentLine == TopMargin + 1)
                {
                    sb.AppendLine(GetRightFormatedText(strRefTexts + strRefs, 80));
                    CurrentLine++;
                }
                sb.AppendLine(sDetail[i]);
                CurrentLine++;
                // Identify the end of current page
                if (CurrentLine > PageLength - BottomMargin)
                {
                    // add bottom margin lines
                    sb.AppendLine(GetRightFormatedText("Contd.." + (CurrentPage + 1).ToString(), 80));

                    for (int j = 1; j < BottomMargin; j++)
                    {
                        //sb.AppendLine("b"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentPage++;
                    CurrentLine = 0; // intialize the current line number
                    // add top margin to the next page
                    for (int j = 1; j <= TopMargin; j++)
                    {
                        //sb.AppendLine("t"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentLine = TopMargin + 1;
                }

            }
            // Print footer 
            if ((PageLength - CurrentLine - BottomMargin) <= sFooter.Length)
            {
                // eject current page
                for (i = CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");

                }
                // add top margin
                // eject current page
                for (i = 1; i <= TopMargin; i++)
                {
                    sb.AppendLine(" ");
                }
                CurrentLine = TopMargin + 1;
                sb.AppendLine(GetRightFormatedText(strRefTexts + strRefs, 80));
                sb.AppendLine(" ");
                sb.AppendLine(" ");
                CurrentLine++;
                CurrentLine++;
                for (i = 0; i < sFooter.Length - 1; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 8; i++)
                {
                    sb.AppendLine(" ");
                }
            }
            else
            {
                LinesCount = PageLength - CurrentLine - BottomMargin - sFooter.Length;
                for (i = 1; i <= LinesCount + 2; i++)
                {
                    sb.AppendLine(" ");
                    CurrentLine++;
                }
                for (i = 0; i < sFooter.Length; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                    CurrentLine++;
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = 1; i <= BottomMargin; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 10; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }

            }

            CurrentLine = TopMargin;
            return sb.ToString();
        }

        public string PrintHeaders()
        {
            string strPrintHeader = "";
            if (this.dsMain.Tables[0].Rows.Count > 0)
            {

                strPrintHeader += (char)18 + this.GetCenterdFormatedText("VOUCHER", 50) + "\n" + (char)15;
                strPrintHeader += PrintLine() + "\n";
                strPrintHeader += "VOUCHER NO  : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["RequestId"]), 48);
                strPrintHeader += "Date : " + this.GetFormatedText(DateTime.Now.ToString("dd/MM/yyyy"), 25) + "\n";

                strPrintHeader += PrintLine() + "\n";

                strPrintHeader += "Cash paid to  : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Reason"]), 65) + "\n";

                strPrintHeader += this.GetFormatedText("the sum Of Rupees " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Words"]), 60) + "\n";
                strPrintHeader += this.GetFormatedText("toward your " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["RequestedBy"]), 60) + "\n";
                strPrintHeader += PrintLine() + "\n";

                strPrintHeader += this.GetFormatedText("Rs." + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Amount"]), 60) + "\n";

                strPrintHeader += PrintLine() + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += " " + "\n";

                strPrintHeader += this.GetFormatedText("Signature", 54);
                strPrintHeader += this.GetRightFormatedText("Authorized Signatory", 25);
            }
            return strPrintHeader;
        }
        public string Printdetails()
        {
            string strPrintDetails = "";
            strPrintDetails = " " + "\n";

            strPrintDetails = " " + "\n";

            return strPrintDetails;

        }
        public string PrintFooter()
        {
            string strPrintFooter = "";


            strPrintFooter += " " + "\n";
            strPrintFooter += " " + "\n";
            strPrintFooter += " " + "\n";
            strPrintFooter += " " + "\n";
            return strPrintFooter;
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
        public string PrintLine()
        {
            string str = "";
            for (int i = 1; i <= 80; i++)
            {
                str += "-";
            }
            return str;
        }
    }


    class RoseBillReport
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
        public string BillType = "VOCHER";
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
        public int pagenumber;
        public bool status;
        public int fpage;
        public int lpage;
        private int slno = 1;
        public int Copies = 1;
        private decimal postoatl = 0.0m;
        private decimal nagtoatl = 0.0m;
        private int pagerecno = 0;
        private bool headerflag = false;
        private bool showprevsum = false;
        public string _strRefText;
        public string _strRef;




        public int linecount = 0;
        public int value = 0;


        public bool Party = false;

        private bool firstpage = true;

        public RoseBillReport()
        {

        }


        public void RREPrintRoseBill()
        {
            bool result = true;

            for (int i = 1; i <= Copies; i++)
            {
                string StrOutPuts = GetPrintOuts(_strRefText, _strRef, PrintRoseBillHeader(), PrintRosedetails(), PrintRoseFooter());
                StreamWriter sr = new StreamWriter("d:\\bill.txt");
                sr.Write(StrOutPuts);
                sr.Close();

            }



            new Process
            {
                StartInfo =
                {
                    FileName = "d:\\Bill.bat",
                    WindowStyle = ProcessWindowStyle.Hidden

                }
            }.Start();


        }
        public string PrintRoseBillHeader()
        {
            string strPrintHeader = "";
            if (this.dsMain.Tables[0].Rows.Count > 0)
            {
                strPrintHeader += (char)18 + this.GetCenterdFormatedText("VOUCHER", 40) + "\n\n" + (char)15;
                strPrintHeader += "Rose Id : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["RequestId"]), 50);
                strPrintHeader += "DATE : " + this.GetFormatedText(DateTime.Now.ToString("dd/MM/yyyy"), 25) + "\n";
                strPrintHeader += PrintLine() + "\n";
                strPrintHeader += "Rose Mode : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["PaymentMode"]), 50);
                strPrintHeader += " " + "\n";
                strPrintHeader += GetFormatedText("the sum Of Rupees " + Convert.ToString(dsMain.Tables[0].Rows[0]["Words"]), 60);

                strPrintHeader += " " + "\n";
                strPrintHeader += GetFormatedText("Paid to :" + Convert.ToString(dsMain.Tables[0].Rows[0]["RequestedBy"]), 60);
                strPrintHeader += " " + "\n";


                strPrintHeader += PrintLine() + "\n";
                strPrintHeader += GetFormatedText("Rs. " + Convert.ToString(dsMain.Tables[0].Rows[0]["Amount"]), 60);
                strPrintHeader += GetFormatedText("Total amount :" + Convert.ToString(dsMain.Tables[0].Rows[0]["totalamount"]), 60) + "\n";
                strPrintHeader += PrintLine();
                strPrintHeader += " " + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += " " + "\n";
                strPrintHeader += GetFormatedText("Signature", 54);
                strPrintHeader += GetRightFormatedText("Authorized Signatory", 25);



            }
            return strPrintHeader;
        }
        public string PrintRosedetails()
        {
            string strPrintDetails = "";
            strPrintDetails = " " + "\n";

            strPrintDetails = " " + "\n";

            return strPrintDetails;

        }
        public string PrintRoseFooter()
        {
            string strPrintFooter = "";


            strPrintFooter += " " + "\n";
            strPrintFooter += " " + "\n";
            strPrintFooter += " " + "\n";
            strPrintFooter += " " + "\n";
            return strPrintFooter;
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
        public string PrintLine()
        {
            string str = "";
            for (int i = 1; i <= 80; i++)
            {
                str += "-";
            }
            return str;
        }
        private string GetPrintOuts(string strRefTexts, string strRefs, string Header, string Details, string Footer)
        {
            int InitialReverseFeed = 3;
            int PageLength = 36;
            int TopMargin = 3;
            int BottomMargin = 4;
            //string _ReverseFeed = '\u001b' + "j" + 'l';
            string _ReverseFeed = '\u001b' + "j" + 'l';
            int i = 0;
            int LinesCount;

            string[] sHeader = Header.Split('\n');
            string[] sFooter = Footer.Split('\n');
            string[] sDetail = Details.Split('\n');
            StringBuilder sb = new StringBuilder();

            // Initial Reverse Feed Characters

            int PreviousPage = 0;
            int CurrentPage = 1;
            int CurrentLine = 1;

            for (i = 0; i < InitialReverseFeed; i++)
            {
                sb.AppendLine(_ReverseFeed);

            }
            sb.AppendLine("");
            CurrentLine = 5;
            // First Header 
            for (i = 0; i < sHeader.Length; i++)
            {
                sb.AppendLine(sHeader[i]);
            }
            CurrentLine += sHeader.Length;

            bool flag = true;
            for (i = 0; i < sDetail.Length; i++)
            {
                if (CurrentLine == TopMargin + 1)
                {
                    sb.AppendLine(GetRightFormatedText(strRefTexts + strRefs, 80));
                    CurrentLine++;
                }
                sb.AppendLine(sDetail[i]);
                CurrentLine++;
                // Identify the end of current page
                if (CurrentLine > PageLength - BottomMargin)
                {
                    // add bottom margin lines
                    sb.AppendLine(GetRightFormatedText("Contd.." + (CurrentPage + 1).ToString(), 80));

                    for (int j = 1; j < BottomMargin; j++)
                    {
                        //sb.AppendLine("b"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentPage++;
                    CurrentLine = 0; // intialize the current line number
                    // add top margin to the next page
                    for (int j = 1; j <= TopMargin; j++)
                    {
                        //sb.AppendLine("t"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentLine = TopMargin + 1;
                }

            }
            // Print footer 
            if ((PageLength - CurrentLine - BottomMargin) <= sFooter.Length)
            {
                // eject current page
                for (i = CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");

                }
                // add top margin
                // eject current page
                for (i = 1; i <= TopMargin; i++)
                {
                    sb.AppendLine(" ");
                }
                CurrentLine = TopMargin + 1;
                sb.AppendLine(GetRightFormatedText(strRefTexts + strRefs, 80));
                sb.AppendLine(" ");
                sb.AppendLine(" ");
                CurrentLine++;
                CurrentLine++;
                for (i = 0; i < sFooter.Length - 1; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 8; i++)
                {
                    sb.AppendLine(" ");
                }
            }
            else
            {
                LinesCount = PageLength - CurrentLine - BottomMargin - sFooter.Length;
                for (i = 1; i <= LinesCount + 2; i++)
                {
                    sb.AppendLine(" ");
                    CurrentLine++;
                }
                for (i = 0; i < sFooter.Length; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                    CurrentLine++;
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = 1; i <= BottomMargin; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 10; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }

            }

            CurrentLine = TopMargin;
            return sb.ToString();

        }

        class QuotationStockReports
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
            public string BillType = "QuotationSTOCK";
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
            public int pagenumber;
            public bool status;
            public int fpage;
            public int lpage;
            private int slno = 1;
            public int Copies = 1;
            private decimal postoatl = 0.0m;
            private decimal nagtoatl = 0.0m;
            private int pagerecno = 0;
            private bool headerflag = false;
            private bool showprevsum = false;
            public string _strRefText;
            public string _strRef;




            public int linecount = 0;
            public int value = 0;


            public bool Party = false;

            private bool firstpage = true;

            public QuotationStockReports()
            {

            }


            public void RREPrintStockReport()
            {
                bool result = true;

                for (int i = 1; i <= Copies; i++)
                {
                    string StrOutPuts = GetPrintOuts(_strRefText, _strRef, PrintStockReportHeader(), PrintStockReportDetails());
                    StreamWriter sr = new StreamWriter("d:\\bill.txt");
                    sr.Write(StrOutPuts);
                    sr.Close();

                }



                new Process
                {
                    StartInfo =
                    {
                        FileName = "d:\\Bill.bat",
                        WindowStyle = ProcessWindowStyle.Hidden

                    }
                }.Start();


            }
            public string PrintStockReportHeader()
            {
                string strPrintHeader = "";
                if (this.dsMain.Tables[0].Rows.Count > 0)
                {
                    strPrintHeader += (char)18 + this.GetCenterdFormatedText("Quotation Less Stock List", 40) + "\n\n" + (char)15;
                    strPrintHeader += this.GetFormatedText("SNo", 3) + " ";
                    strPrintHeader += this.GetFormatedText("PRODUCT NAME", 44) + " ";
                    strPrintHeader += this.GetFormatedText("Available", 11) + " ";
                    strPrintHeader += this.GetRightFormatedText("Ordered", 11) + " ";
                    strPrintHeader += this.GetRightFormatedText("Need", 11) + " ";
                    strPrintHeader += PrintLine() + "\n";


                }
                return strPrintHeader;
            }
            public string PrintStockReportDetails()
            {
                string strPrintHeader = "";
                if (this.dsMain.Tables[0].Rows.Count > 0)
                {
                    strPrintHeader += this.GetFormatedText(slno.ToString(), 3) + " ";
                    strPrintHeader += this.GetFormatedText(dsMain.Tables[0].Rows[slno - 1]["ItemsLessStock"].ToString(), 44) + " ";
                    strPrintHeader += this.GetCenterdFormatedText(dsMain.Tables[0].Rows[slno - 1]["Avalavbe"].ToString(), 11) + " ";
                    strPrintHeader += this.GetCenterdFormatedText(dsMain.Tables[0].Rows[slno - 1]["Order"].ToString(), 11) + " ";
                    strPrintHeader += this.GetCenterdFormatedText(dsMain.Tables[0].Rows[slno - 1]["Need"].ToString(), 11) + " ";
                    // strPrintHeader += this.GetRightFormatedText("Need", 11) + " ";
                    strPrintHeader += PrintLine() + "\n";


                }
                return strPrintHeader;
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
            public string PrintLine()
            {
                string str = "";
                for (int i = 1; i <= 80; i++)
                {
                    str += "-";
                }
                return str;
            }
            private string GetPrintOuts(string strRefTexts, string strRefs, string Header, string Details)
            {
                int InitialReverseFeed = 3;
                int PageLength = 36;
                int TopMargin = 3;
                int BottomMargin = 4;
                //string _ReverseFeed = '\u001b' + "j" + 'l';
                string _ReverseFeed = '\u001b' + "j" + 'l';
                int i = 0;
                int LinesCount;

                string[] sHeader = Header.Split('\n');
                //string[] sFooter = Footer.Split('\n');
                string[] sDetail = Details.Split('\n');
                StringBuilder sb = new StringBuilder();

                // Initial Reverse Feed Characters

                int PreviousPage = 0;
                int CurrentPage = 1;
                int CurrentLine = 1;

                for (i = 0; i < InitialReverseFeed; i++)
                {
                    sb.AppendLine(_ReverseFeed);

                }
                sb.AppendLine("");
                CurrentLine = 3;
                // First Header 
                for (i = 0; i < sHeader.Length; i++)
                {
                    sb.AppendLine(sHeader[i]);
                }
                CurrentLine += sHeader.Length;

                bool flag = true;
                for (i = 0; i < sDetail.Length; i++)
                {
                    if (CurrentLine == TopMargin + 1)
                    {
                        sb.AppendLine(GetRightFormatedText(strRefTexts + strRefs, 80));
                        CurrentLine++;
                    }
                    sb.AppendLine(sDetail[i]);
                    CurrentLine++;
                    // Identify the end of current page
                    if (CurrentLine > PageLength - BottomMargin)
                    {
                        // add bottom margin lines
                        sb.AppendLine(GetRightFormatedText("Contd.." + (CurrentPage + 1).ToString(), 80));

                        for (int j = 1; j < BottomMargin; j++)
                        {
                            //sb.AppendLine("b"+j.ToString());
                            sb.AppendLine("");
                        }
                        CurrentPage++;
                        CurrentLine = 0; // intialize the current line number
                        // add top margin to the next page
                        for (int j = 1; j <= TopMargin; j++)
                        {
                            //sb.AppendLine("t"+j.ToString());
                            sb.AppendLine("");
                        }
                        CurrentLine = TopMargin + 1;
                    }

                }
                //// Print footer 
                //if ((PageLength - CurrentLine - BottomMargin) <= sFooter.Length)
                //{
                //    // eject current page
                //    for (i = CurrentLine; i <= PageLength; i++)
                //    {
                //        sb.AppendLine(" ");

                //    }
                //    // add top margin
                //    // eject current page
                //    for (i = 1; i <= TopMargin; i++)
                //    {
                //        sb.AppendLine(" ");
                //    }
                //    CurrentLine = TopMargin + 1;
                //    sb.AppendLine(GetRightFormatedText(strRefTexts + strRefs, 80));
                //    sb.AppendLine(" ");
                //    sb.AppendLine(" ");
                //    CurrentLine++;
                //    CurrentLine++;
                //    for (i = 0; i < sFooter.Length - 1; i++)
                //    {
                //        sb.AppendLine(sFooter[i].ToString());
                //    }
                //    CurrentLine = CurrentLine + sFooter.Length;
                //    // eject current page
                //    for (i = CurrentLine; i <= PageLength; i++)
                //    {
                //        sb.AppendLine(" ");
                //    }
                //    // bill complete

                //    // insert intial for feed for reverse
                //    for (i = 1; i <= 8; i++)
                //    {
                //        sb.AppendLine(" ");
                //    }
                //}
                //else
                //{
                //    LinesCount = PageLength - CurrentLine - BottomMargin - sFooter.Length;
                //    for (i = 1; i <= LinesCount + 2; i++)
                //    {
                //        sb.AppendLine(" ");
                //        CurrentLine++;
                //    }
                //    for (i = 0; i < sFooter.Length; i++)
                //    {
                //        sb.AppendLine(sFooter[i].ToString());
                //        CurrentLine++;
                //    }
                //    CurrentLine = CurrentLine + sFooter.Length;
                // eject current page

                for (i = 1; i <= BottomMargin; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 10; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }
                CurrentLine = TopMargin;
                return sb.ToString();
            }


        }
    }




    class PurchaseorderPrint
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
        public string BillType = "PURCHASE  ORDER";
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
        public int pagenumber;
        public bool status;
        public int fpage;
        public int lpage;
        private int slno = 1;
        public int Copies = 1;
        private decimal postoatl = 0.0m;
        private decimal nagtoatl = 0.0m;
        private int pagerecno = 0;
        private bool headerflag = false;
        private bool showprevsum = false;
        public string _strRefText;
        public string _strRef;




        public int linecount = 0;
        public int value = 0;


        public bool Party = false;

        private bool firstpage = true;

        public PurchaseorderPrint()
        {

        }



        public void RREPrintPurchaseOrderQuotation()
        {
            bool result = true;

            for (int i = 1; i <= Copies; i++)
            {
                string StrOutPut = GetPrintOut(_strRefText, _strRef, PrintPurchaseHeader(), PrintPurchaseDetails(), PrintPurchaseFooter());
                //string StrOutPut = '\u001b' + "j" + 'l' + '\u001b' + "j" + 'l' + '\u001b' + "j" + 'l';
                //StrOutPut += "test";
                //StrOutPut+= (char)15 + "print 2\n";
                //StrOutPut+= (char)18 + "print 2\n";

                //StrOutPut = (char)(15) + StrOutPut + (char)27 + "F";
                //StrOutPut = StrOutPut + (char)18;

                StreamWriter sr = new StreamWriter("d:\\bill.txt");
                sr.Write(StrOutPut);
                sr.Close();

            }



            new Process
            {
                StartInfo =
                {
                    FileName = "d:\\Bill.bat",
                    WindowStyle = ProcessWindowStyle.Hidden

                }
            }.Start();
        }
        private string GetPrintOut(string strRefText, string strRef, string Header, string Detail, string Footer)
        {
            int InitialReverseFeed = 3;
            int PageLength = 36;
            int TopMargin = 3;
            int BottomMargin = 6;
            //string _ReverseFeed = '\u001b' + "j" + 'l';
            string _ReverseFeed = '\u001b' + "j" + 'l';
            int i = 0;
            int LinesCount;

            string[] sHeader = Header.Split('\n');
            string[] sFooter = Footer.Split('\n');
            string[] sDetail = Detail.Split('\n');
            StringBuilder sb = new StringBuilder();

            // Initial Reverse Feed Characters

            int PreviousPage = 0;
            int CurrentPage = 1;
            int CurrentLine = 1;

            for (i = 0; i < InitialReverseFeed; i++)
            {
                sb.AppendLine(_ReverseFeed);

            }
            sb.AppendLine("");
            CurrentLine = 5;
            // First Header 
            for (i = 0; i < sHeader.Length; i++)
            {
                sb.AppendLine(sHeader[i]);
            }
            CurrentLine += sHeader.Length;

            bool flag = true;
            for (i = 0; i < sDetail.Length; i++)
            {
                if (CurrentLine == TopMargin + 1)
                {

                    sb.AppendLine(GetRightFormatedText("", 80));
                    CurrentLine++;
                }
                sb.AppendLine(sDetail[i]);
                CurrentLine++;
                // Identify the end of current page
                if (CurrentLine > PageLength - BottomMargin)
                {
                    // add bottom margin lines

                    sb.AppendLine(GetRightFormatedText("Contd.." + (CurrentPage + 1).ToString(), 80));

                    for (int j = 1; j < BottomMargin; j++)
                    {
                        //sb.AppendLine("b"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentPage++;
                    CurrentLine = 0; // intialize the current line number
                    // add top margin to the next page
                    for (int j = 1; j <= TopMargin; j++)
                    {
                        //sb.AppendLine("t"+j.ToString());
                        sb.AppendLine("");
                    }
                    CurrentLine = TopMargin + 1;
                }

            }
            // Print footer 
            if ((PageLength - CurrentLine - BottomMargin) < sFooter.Length)
            {
                // eject current page
                for (i = PageLength - CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");

                }
                // add top margin
                // eject current page
                for (i = 1; i <= TopMargin; i++)
                {
                    sb.AppendLine(" ");
                }
                sb.AppendLine(GetRightFormatedText(strRefText + strRef, 80));
                for (i = 0; i < sFooter.Length - 1; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = CurrentLine; i <= PageLength; i++)
                {
                    sb.AppendLine(" ");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 7; i++)
                {
                    sb.AppendLine(" ");
                }
            }
            else
            {
                LinesCount = PageLength - CurrentLine - BottomMargin - sFooter.Length;
                for (i = 1; i <= LinesCount + 2; i++)
                {
                    sb.AppendLine(" ");
                    CurrentLine++;
                }
                for (i = 0; i < sFooter.Length; i++)
                {
                    sb.AppendLine(sFooter[i].ToString());
                    CurrentLine++;
                }
                CurrentLine = CurrentLine + sFooter.Length;
                // eject current page
                for (i = 1; i <= BottomMargin; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }
                // bill complete

                // insert intial for feed for reverse
                for (i = 1; i <= 9; i++)
                {
                    //sb.AppendLine(i.ToString());
                    sb.AppendLine("");
                }

            }

            CurrentLine = TopMargin;
            return sb.ToString();
        }
        public void pagebreak()
        {
            int i = 1;
            int j = 6;
            while (j >= i)
            {
                if (i == 6)
                {
                    this.rdr.WriteLine("");
                }
                else
                {
                    this.rdr.WriteLine("");
                }
                i++;
            }
        }

        public void pageEnd()
        {
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine("");
            this.rdr.WriteLine(".");
        }

        public string PrintPurchaseHeader()
        {
            string strPrintPurchaseHeader = "";
            if (this.dsMain.Tables[0].Rows.Count > 0)
            {

                strPrintPurchaseHeader += (char)18 + this.GetCenterdFormatedText("PURCHASE  ORDER", 50) + "\n" + (char)15;
                strPrintPurchaseHeader += "ORDER NO :" + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["OrderNumber"]), 33);
                strPrintPurchaseHeader += "DATE : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Date"]), 33) + "\n";


                strPrintPurchaseHeader += PrintLine() + "\n";
                strPrintPurchaseHeader += this.GetFormatedText("SNo", 3) + " ";
                strPrintPurchaseHeader += this.GetFormatedText("PRODUCT NAME", 50) + " ";
                strPrintPurchaseHeader += this.GetFormatedText("QUANTITY", 12) + " ";

                strPrintPurchaseHeader += this.GetFormatedText("", 12);
                strPrintPurchaseHeader += " " + "\n";
                strPrintPurchaseHeader += PrintLine();
            }
            return strPrintPurchaseHeader;
        }
        public string PrintPurchaseDetails()
        {
            string PrintPurchaseDetails = "";
            while (this.dsMain.Tables[0].Rows.Count >= this.slno)
            {
                PrintPurchaseDetails += this.GetFormatedText(this.slno.ToString(), 3) + " ";
                PrintPurchaseDetails += this.GetFormatedText(this.dsMain.Tables[0].Rows[this.slno - 1]["Item"].ToString(), 50) + " ";
                PrintPurchaseDetails += this.GetRightFormatedText(this.dsMain.Tables[0].Rows[this.slno - 1]["Quantity"].ToString(), 8) + " " + "" + this.dsMain.Tables[0].Rows[this.slno - 1]["UOM"].ToString() + " ";
                ;

                PrintPurchaseDetails += " \n";

                this.slno++;
            }
            return PrintPurchaseDetails + "\n";
        }
        public string PrintPurchaseFooter()
        {
            string strPrintPurchaseFooter = "";
            char[] trimChars = new char[]
			{
				'-',
				' '
			};
            string value = Convert.ToString(this.nagtoatl).TrimStart(trimChars);
            strPrintPurchaseFooter += PrintLine() + "\n\n";

            strPrintPurchaseFooter += this.GetRightFormatedText(Convert.ToString("Sign :"), 54) + "-------------------------";


            return strPrintPurchaseFooter;

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
        public string PrintLine()
        {
            string str = "";
            for (int i = 1; i <= 80; i++)
            {
                str += "-";
            }
            return str;
        }
    }
}


