using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Inventory.Sales
{
    class QuotationStockReport
    {
    
        // Printing commands are depends on the Dotmatrix printer that we are using
        public DataTable dsMain;
        public string IdQutoastion;
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
        public string BillType = "QuotationSTOCK";
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

        public int pagenumber;

        public bool status=true;
        public int fpage;
        public int lpage;


        public QuotationStockReport()
        {
            //fileName = "d:\\Temp\\Quotation_" + Guid.NewGuid().ToString() + ".txt";
            fileName = "d:\\bill.txt";

            var myFile = File.Create(fileName);
            myFile.Close();

            //rdr=new System.IO.StreamWriter(fileName);
            //rdr.AutoFlush();


        }
        public bool GenerateQuoationid()
        {
            bool result = true;

            bool loop = true;
            
            using (rdr = new System.IO.StreamWriter(fileName))
            {
                //PrintHeader();
                //PrintDetails();

                if (status == true)
                {
                    PrintHeader();
                    PrintDetails();
                }
                else
                {
                    PrintDetailspage();
                }


                PrintBuffer();
                Close();
            }

            return result;
        }
        int slno = 1;
        
        int pagerecno = 0;
        bool headerflag = false;
        bool showprevsum = false;
        public void PrintDetails()
        {
            while (dsMain.Rows.Count >= slno)
            {
                if (headerflag)
                {
                    rdr.WriteLine("");
                    //rdr.WriteLine("");
                    //rdr.WriteLine("");
                    //rdr.WriteLine("");
                    //rdr.WriteLine("");
                    PrintHeader();
                    headerflag = false;
                    showprevsum = true;
                }
                if (showprevsum)
                {
                    rdr.Write("{0,-4}", GetFormatedText(" ", 4));
                    rdr.Write("{0,-30}", GetFormatedText("B/F", 30));
                    rdr.Write(GetRightFormatedText("", 43));
                    rdr.WriteLine("");
                    pagerecno++;
                    showprevsum = false;

                }

                rdr.Write("{0,-3}", GetFormatedText(slno.ToString(), 3) + " ");
                rdr.Write("{0,-44}", GetFormatedText(dsMain.Rows[slno - 1]["ItemsLessStock"].ToString(), 44) + " ");
                rdr.Write("{0,-11}", GetCenterdFormatedText(dsMain.Rows[slno - 1]["Avalavbe"].ToString(), 11) + " ");
                rdr.Write("{0,-11}", GetCenterdFormatedText(dsMain.Rows[slno - 1]["Order"].ToString(), 11) + " ");
                rdr.Write("{0,-11}", GetCenterdFormatedText(dsMain.Rows[slno - 1]["Need"].ToString(), 11) + " ");
               
                rdr.WriteLine("");
                
                pagerecno++;


                if (slno % 9 == 0)
                {
                    // Print Footer Line
                    int s1 = dsMain.Rows.Count / 9;
                    if (s1 == pagenumber)
                    {
                        if (dsMain.Rows.Count % 9 == 0)
                        {
                            PrintFooter(false);
                            pagerecno = 10;
                        }
                        else
                        {
                            PrintFooter(true);
                        }
                    }
                    else
                    {
                        PrintFooter(true);

                    }

                    pagenumber++;
                    int s2 = dsMain.Rows.Count / 9;
                    if (s2 + 1 == pagenumber)
                    {
                        if (dsMain.Rows.Count % 9 != 0)
                        {
                            rdr.WriteLine("");
                            rdr.WriteLine("");
                            rdr.WriteLine("");
                        }
                    }
                    else
                    {
                        rdr.WriteLine("");
                        rdr.WriteLine("");
                        rdr.WriteLine("");
                    }

                    if (dsMain.Rows.Count > slno)
                    {
                        rdr.Write(GetRightFormatedText("Contd..." + pagenumber.ToString(), 80));
                        rdr.WriteLine("");
                        rdr.WriteLine("");
                        rdr.WriteLine("");
                        rdr.WriteLine("");
                        rdr.WriteLine("");
                        rdr.WriteLine("");
                       

                    }
                    else
                    {
                        if (dsMain.Rows.Count % 9 != 0)
                        {
                            rdr.WriteLine("");
                            rdr.WriteLine("");
                        }
                    }
                    pagerecno = 0;

                    headerflag = true;
                }
                slno++;
            }


            int s = dsMain.Rows.Count / 9;
            if (s + 1 == pagenumber)
            {
                if (dsMain.Rows.Count % 9 == 0)
                {
                    headerflag = false;
                    pagerecno = 10;
                }
                else
                {
                    headerflag = false;
                }
            }




            if (!headerflag)
            {
                for (int i = pagerecno; i <= 9; i++)
                    rdr.WriteLine("");

                //  rdr.WriteLine("");
                if (pagerecno <= 9)
                {
                    rdr.WriteLine("");
                    rdr.WriteLine("");
                    rdr.WriteLine("");
                }
                //rdr.WriteLine("");
            }
        }

        public void PrintDetailspage()
        {
            if (pagenumber >= fpage && pagenumber <= lpage)
            {
                PrintHeader();
            }
            while (dsMain.Rows.Count >= slno)
            {
              

                if (headerflag)
                {
                    if (pagenumber >= fpage && pagenumber <= lpage)
                    {
                        if (pagenumber != fpage)
                        {
                            rdr.WriteLine("");
                            //rdr.WriteLine("");
                            //rdr.WriteLine("");
                            //rdr.WriteLine("");
                            //rdr.WriteLine("");
                        }
                        PrintHeader();
                        headerflag = false;
                        showprevsum = true;
                    }
                }
                if (showprevsum)
                {
                    if (pagenumber >= fpage && pagenumber <= lpage)
                    {
                        
                    }
                        pagerecno++;
                        showprevsum = false;
                   

                }
                if (pagenumber >= fpage && pagenumber <= lpage)
                {
                    rdr.Write("{0,-3}", GetFormatedText(slno.ToString(), 3) + " ");
                    rdr.Write("{0,-44}", GetFormatedText(dsMain.Rows[slno - 1]["ItemsLessStock"].ToString(), 44) + " ");
                    rdr.Write("{0,-11}", GetCenterdFormatedText(dsMain.Rows[slno - 1]["Avalavbe"].ToString(), 11) + " ");
                    rdr.Write("{0,-11}", GetCenterdFormatedText(dsMain.Rows[slno - 1]["Order"].ToString(), 11) + " ");
                    rdr.Write("{0,-11}", GetCenterdFormatedText(dsMain.Rows[slno - 1]["Need"].ToString(), 11) + " ");
                    rdr.WriteLine("");
                }
                
                pagerecno++;


                if (slno % 9 == 0)
                {
                    // Print Footer Line
                    int s1 = dsMain.Rows.Count / 9;
                    if (s1 == pagenumber)
                    {
                        if (dsMain.Rows.Count % 9 == 0)
                        {
                            if (pagenumber >= fpage && pagenumber <= lpage)
                            {
                                PrintFooter(false);
                                pagerecno = 10;
                            }
                        }
                        else
                        {
                            if (pagenumber >= fpage && pagenumber <= lpage)
                            {
                                PrintFooter(true);
                            }
                        }
                    }
                    else
                    {
                        if (pagenumber >= fpage && pagenumber <= lpage)
                        {
                            PrintFooter(true);
                        }

                    }

                   
                    int s2 = dsMain.Rows.Count / 9;
                    if (s2 + 1 == pagenumber)
                    {
                        if (dsMain.Rows.Count % 9 != 0)
                        {
                            if (pagenumber >= fpage && pagenumber <= lpage)
                            {
                                rdr.WriteLine("");
                                rdr.WriteLine("");
                                rdr.WriteLine("");
                            }
                        }
                    }
                    else
                    {
                        if (pagenumber >= fpage && pagenumber <= lpage)
                        {
                            rdr.WriteLine("");
                            rdr.WriteLine("");
                            rdr.WriteLine("");
                        }
                    }

                    if (dsMain.Rows.Count > slno)
                    {
                        if (pagenumber >= fpage && pagenumber <= lpage)
                        {
                            rdr.Write(GetRightFormatedText("Contd..." + pagenumber.ToString(), 80));
                            rdr.WriteLine("");
                            rdr.WriteLine("");
                            rdr.WriteLine("");
                            rdr.WriteLine("");
                            rdr.WriteLine("");
                            rdr.WriteLine("");
                        }


                    }
                    else
                    {
                        if (dsMain.Rows.Count % 9 != 0)
                        {
                            if (pagenumber >= fpage && pagenumber <= lpage)
                            {
                                rdr.WriteLine("");
                                rdr.WriteLine("");
                            }
                        }
                    }
                    pagerecno = 0;
                    pagenumber++;
                    headerflag = true;
                }
                slno++;
            }


            int s = dsMain.Rows.Count / 9;
            if (s + 1 == pagenumber)
            {
                if (dsMain.Rows.Count % 9 == 0)
                {
                    headerflag = false;
                    pagerecno = 10;
                }
                else
                {
                    headerflag = false;
                }
            }




            if (!headerflag)
            {
                if (pagenumber == lpage)
                {

                    for (int i = pagerecno; i <= 9; i++)
                        rdr.WriteLine("");


                }

                //  rdr.WriteLine("");
                if (pagerecno <= 9)
                {
                    rdr.WriteLine("");
                    rdr.WriteLine("");
                    rdr.WriteLine("");
                }
                //rdr.WriteLine("");
            }
        }

        public void PrintFooter(bool val)
        {
            if (val)
            {
               
                rdr.WriteLine("");
            }
       
        }
        public void Close()
        {
            rdr.Close();
        }
        public void GetDataSet(DataTable Ds)
        {
            dsMain = Ds;
        }
        public void PrintHeader()
        {
            if (dsMain.Rows.Count > 0)
            {

               
                rdr.WriteLine((char)14 + GetCenterdFormatedText("Quotation Less Stock List", 40));
                rdr.WriteLine("");
                rdr.Write(GetRightFormatedText(IdQutoastion, 80));
                
                rdr.WriteLine("");
                

                PrintLine();
                rdr.Write("{0,-3}", GetFormatedText("SNo", 3) + " ");
                rdr.Write("{0,-44}", GetFormatedText("PRODUCT NAME", 44) + " ");
                rdr.Write("{0,-11}", GetCenterdFormatedText("Available", 11) + " ");
                rdr.Write("{0,-11}", GetCenterdFormatedText("Ordered", 11) + " ");
                rdr.Write("{0,-11}", GetCenterdFormatedText("Need", 11) + " ");
               
                PrintLine();
               
            }

        }
        public void PritPageHeader()
        {

            rdr.WriteLine("SLNo  PRODUCT NAME                                       Available Ordered  Need");
            PrintLine();
        }
        

        public void PrintDetails2()
        {

            rdr.WriteLine("SLNo |Name                                           ");
            PrintLine();
            for (int ln = 1; ln <= 30; ln++)
            {
             
               
                rdr.WriteLine("");
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
