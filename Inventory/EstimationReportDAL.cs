using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Inventory
{
    class EstimationReportDAL
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
		public string BillType = "ESTIMATION";
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
		private int pagerecno = 0;
		private bool headerflag = false;
		private bool showprevsum = false;
        private decimal postoatl = 0.0m;
        private decimal nagtoatl = 0.0m;
		public EstimationReportDAL()
		{
			this.fileName = "d:\\bill.txt";
			FileStream fileStream = File.Create(this.fileName);
			fileStream.Close();
		}


        public bool GenerateQuoationReport()
        {
            bool result = true;
            using (this.rdr = new StreamWriter(this.fileName))
            {
                this.PrintHeader();
                this.PrintDetails();
                this.Close();
                this.rdr.Close();
            }
            return result;
        }






        public string PrintQuotationHeader()
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
                strPrintHeader += PrintLines() + "\n";
                strPrintHeader += this.GetFormatedText("SNO", 3) + " ";
                strPrintHeader += this.GetFormatedText("PRODUCT NAME", 30) + " ";
                strPrintHeader += this.GetFormatedText("QUANTITY", 12) + " ";
                strPrintHeader += this.GetRightFormatedText("RATE", 9) + " ";
                strPrintHeader += this.GetRightFormatedText("AMOUNT", 9) + " ";
                strPrintHeader += this.GetFormatedText("", 12);
                strPrintHeader += " " + "\n";
                strPrintHeader += PrintLines();
            }
            return strPrintHeader;
        }
        public string PrintQuotationDetails()
        {
            string PrintDetails = "";
            while (this.dsMain.Tables[1].Rows.Count >0)
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
            return PrintDetails + "\n";
        }
        public string PrintQuotationFooter()
        {
            string strPrintFooter = "";
            char[] trimChars = new char[]
			{
				'-',
				' '
			};
            string value = Convert.ToString(this.nagtoatl).TrimStart(trimChars);
            strPrintFooter += PrintLines() + "\n";
            strPrintFooter += this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Assist"].ToString()), 15);
            strPrintFooter += this.GetFormatedText(Convert.ToString("Pur:"), 4);
            strPrintFooter += this.GetFormatedText(Convert.ToString(this.postoatl), 13);
            strPrintFooter += this.GetFormatedText(Convert.ToString(" Rtn:"), 5);
            strPrintFooter += this.GetFormatedText(Convert.ToString(value), 13);
            strPrintFooter += this.GetRightFormatedText(Convert.ToString("ToT:"), 4);
            strPrintFooter += this.GetFormatedText(Convert.ToString(this.Total), 13) + "\n"; ;

            return strPrintFooter;

        }


        public string PrintLines()
        {
            string str = "";
            for (int i = 1; i <= 80; i++)
            {
                str += "-";
            }
            return str;
        }

        public bool GenerateQuoationPrintViewReport()
        {
            bool result = true;
            using (this.rdr = new StreamWriter(this.fileName))
            {
                this.PrintQuotationHeader();
                this.PrintQuotationDetails();
                this.PrintQuotationFooter();
                this.Close();
                this.rdr.Close();
            }
            return result;
        }

		public bool GenerateQuoation()
		{
			bool result = true;
			using (this.rdr = new StreamWriter(this.fileName))
			{
				if (this.status)
				{
                    string _ReverseFeed = '\u001b' + "j" + 'l';
                    this.rdr.WriteLine("");
                    this.rdr.WriteLine("");
                    this.rdr.WriteLine(_ReverseFeed);
                    this.rdr.WriteLine(_ReverseFeed);
                    this.rdr.WriteLine(_ReverseFeed);

					this.PrintHeader();
					this.PrintDetails();
				}
				else
				{
					this.PrintDetailspage();
				}
				this.Close();
			}
			return result;
		}
        public void PrintDetails()
        {
            while (this.dsMain.Tables[1].Rows.Count >= this.slno)
            {
                if (this.headerflag)
                {
                    this.rdr.WriteLine("");
                    this.PrintHeader();
                    this.headerflag = false;
                    this.showprevsum = true;
                }
                if (this.showprevsum)
                {
                    this.rdr.Write("{0,-4}", this.GetFormatedText(" ", 4));
                    this.rdr.Write("{0,-30}", this.GetFormatedText("B/F", 30));
                    this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 43));
                    this.rdr.WriteLine("");
                    this.pagerecno++;
                    this.showprevsum = false;
                }
                this.rdr.Write("{0,-3}", this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["sino"].ToString(), 3) + " ");
                this.rdr.Write("{0,-40}", this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["ItemName"].ToString(), 40) + " ");
                if (this.dsMain.Tables[1].Rows[this.slno - 1]["Quantity"].ToString() == "0")
                    this.rdr.Write("{0,-8}", this.GetRightFormatedText(" ", 8) + " ");
                else
                    this.rdr.Write("{0,-8}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Quantity"].ToString(), 8) + " ");

                this.rdr.Write("{0,-3}", this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["UOM"].ToString(), 3) + " ");

                if (this.dsMain.Tables[1].Rows[this.slno - 1]["Rate"].ToString() == "0.00")
                    this.rdr.Write("{0,-9}", this.GetRightFormatedText(" ", 9) + " ");
                else
                    this.rdr.Write("{0,-9}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Rate"].ToString(), 9) + " ");

                if (this.dsMain.Tables[1].Rows[this.slno - 1]["Amount"].ToString() == "0.00")
                    this.rdr.Write("{0,-9}", this.GetRightFormatedText(" ", 9) + " ");
                else
                    this.rdr.Write("{0,-9}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Amount"].ToString(), 9) + " ");
                this.rdr.WriteLine("");
                this.Total += Convert.ToDecimal(this.dsMain.Tables[1].Rows[this.slno - 1]["Amount"]);
                this.pagerecno++;
                if (this.slno % 9 == 0)
                {
                    int num = this.dsMain.Tables[1].Rows.Count / 9;
                    if (num == this.pagenumber)
                    {
                        if (this.dsMain.Tables[1].Rows.Count % 9 == 0)
                        {
                            this.PrintFooter(false);
                            this.pagerecno = 10;
                        }
                        else
                        {
                            this.PrintFooter(true);
                        }
                    }
                    else
                    {
                        this.PrintFooter(true);
                    }
                    this.pagenumber++;
                    int num2 = this.dsMain.Tables[1].Rows.Count / 9;
                    if (num2 + 1 == this.pagenumber)
                    {
                        if (this.dsMain.Tables[1].Rows.Count % 9 != 0)
                        {
                            this.rdr.WriteLine("");
                            this.rdr.WriteLine("");
                            this.rdr.WriteLine("");
                        }
                    }
                    else
                    {
                        this.rdr.WriteLine("");
                        this.rdr.WriteLine("");
                        this.rdr.WriteLine("");
                    }
                    if (this.dsMain.Tables[1].Rows.Count > this.slno)
                    {
                        this.rdr.Write(this.GetRightFormatedText("Contd..." + this.pagenumber.ToString(), 80));
                        this.rdr.WriteLine("");
                        this.rdr.WriteLine("");
                        this.rdr.WriteLine("");
                        this.rdr.WriteLine("");
                        this.rdr.WriteLine("");
                        this.rdr.WriteLine("");
                    }
                    else
                    {
                        if (this.dsMain.Tables[1].Rows.Count % 9 != 0)
                        {
                            this.rdr.WriteLine("");
                            this.rdr.WriteLine("");
                        }
                    }
                    this.pagerecno = 0;
                    this.headerflag = true;
                }
                this.slno++;
            }
            int num3 = this.dsMain.Tables[1].Rows.Count / 9;
            if (num3 + 1 == this.pagenumber)
            {
                if (this.dsMain.Tables[1].Rows.Count % 9 == 0)
                {
                    this.headerflag = false;
                    this.pagerecno = 10;
                }
                else
                {
                    this.headerflag = false;
                }
            }
            if (!this.headerflag)
            {
                for (int i = this.pagerecno; i <= 9; i++)
                {
                    this.rdr.WriteLine("");
                }
                this.rdr.WriteLine(this.GetRightFormatedText("-------------------", 80) + " ");
                if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["ReturnSales"])))
                {
                    this.rdr.Write(this.GetFormatedText("     " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["ReturnSales"]), 50));
                    this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 27));
                    this.rdr.WriteLine("");
                }
                else
                {
                    this.rdr.Write(this.GetFormatedText("     TOTAL", 30));
                    this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 47));
                    this.rdr.WriteLine("");
                }
                if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["LessAmount"])))
                {
                    if (Convert.ToDouble(this.dsMain.Tables[0].Rows[0]["LessAmount"]) > 0.0)
                    {
                        this.rdr.Write(this.GetFormatedText("     LESS AMOUNT", 30));
                        this.rdr.Write(this.GetRightFormatedText("-" + Convert.ToString(this.dsMain.Tables[0].Rows[0]["LessAmount"]), 47));
                        this.rdr.WriteLine("");
                    }
                }
                if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["OtherCharges"])))
                {
                    if (Convert.ToDouble(this.dsMain.Tables[0].Rows[0]["OtherCharges"]) > 0.0)
                    {
                        this.rdr.Write(this.GetFormatedText("     OTHERS", 30));
                        this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["OtherCharges"]), 47));
                        this.rdr.WriteLine("");
                    }
                }
                this.rdr.Write(this.GetFormatedText("     GRAND TOTAL", 30));
                this.rdr.Write((char)15 + this.GetRightFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["GrnandTotal"]), 50));
                this.rdr.WriteLine("");
                this.rdr.WriteLine(this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Assist"]), 30));
                this.rdr.WriteLine(this.GetCenterdFormatedText("Check the goods carefully before taking delivery", 80));
                this.rdr.WriteLine(this.GetCenterdFormatedText("No guarantee for glass fittings & bulbs. Company service guarantee only", 80));
                this.rdr.WriteLine(this.GetCenterdFormatedText("for guarantee items. Goods once sold cannot be taken back or exchanged.", 80));
                if (this.pagerecno <= 9)
                {
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
        }
		public void PrintDetailspage()
		{
			if (this.pagenumber >= this.fpage && this.pagenumber <= this.lpage)
			{
				this.PrintHeader();
			}
			while (this.dsMain.Tables[1].Rows.Count >= this.slno)
			{
				if (this.headerflag)
				{
					if (this.pagenumber >= this.fpage && this.pagenumber <= this.lpage)
					{
						if (this.pagenumber != this.fpage)
						{
							this.rdr.WriteLine("");
						}
						this.PrintHeader();
						this.headerflag = false;
						this.showprevsum = true;
					}
				}
				if (this.showprevsum)
				{
					if (this.pagenumber >= this.fpage && this.pagenumber <= this.lpage)
					{
						this.rdr.Write("{0,-4}", this.GetFormatedText(" ", 4));
						this.rdr.Write("{0,-30}", this.GetFormatedText("B/F", 30));
						this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 43));
						this.rdr.WriteLine("");
					}
					this.pagerecno++;
					this.showprevsum = false;
				}
				if (this.pagenumber >= this.fpage && this.pagenumber <= this.lpage)
				{
					this.rdr.Write("{0,-3}", this.GetFormatedText(this.slno.ToString(), 3) + " ");
					this.rdr.Write("{0,-40}", this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["ItemName"].ToString(), 40) + " ");
					this.rdr.Write("{0,-8}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Quantity"].ToString(), 8) + " ");
					this.rdr.Write("{0,-3}", this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["UOM"].ToString(), 3) + " ");
					this.rdr.Write("{0,-9}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Rate"].ToString(), 9) + " ");
					this.rdr.Write("{0,-9}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Amount"].ToString(), 9) + " ");
					this.rdr.WriteLine("");
				}
				this.Total += Convert.ToDecimal(this.dsMain.Tables[1].Rows[this.slno - 1]["Amount"]);
				this.pagerecno++;
				if (this.slno % 9 == 0)
				{
					int num = this.dsMain.Tables[1].Rows.Count / 9;
					if (num == this.pagenumber)
					{
						if (this.dsMain.Tables[1].Rows.Count % 9 == 0)
						{
							if (this.pagenumber >= this.fpage && this.pagenumber <= this.lpage)
							{
								this.PrintFooter(false);
								this.pagerecno = 10;
							}
						}
						else
						{
							if (this.pagenumber >= this.fpage && this.pagenumber <= this.lpage)
							{
								this.PrintFooter(true);
							}
						}
					}
					else
					{
						if (this.pagenumber >= this.fpage && this.pagenumber <= this.lpage)
						{
							this.PrintFooter(true);
						}
					}
					int num2 = this.dsMain.Tables[1].Rows.Count / 9;
					if (num2 + 1 == this.pagenumber)
					{
						if (this.dsMain.Tables[1].Rows.Count % 9 != 0)
						{
							if (this.pagenumber >= this.fpage && this.pagenumber <= this.lpage)
							{
								this.rdr.WriteLine("");
								this.rdr.WriteLine("");
								this.rdr.WriteLine("");
							}
						}
					}
					else
					{
						if (this.pagenumber >= this.fpage && this.pagenumber <= this.lpage)
						{
							this.rdr.WriteLine("");
							this.rdr.WriteLine("");
							this.rdr.WriteLine("");
						}
					}
					if (this.dsMain.Tables[1].Rows.Count > this.slno)
					{
						if (this.pagenumber >= this.fpage && this.pagenumber <= this.lpage)
						{
							this.rdr.Write(this.GetRightFormatedText("Contd..." + this.pagenumber.ToString(), 80));
							this.rdr.WriteLine("");
							this.rdr.WriteLine("");
							this.rdr.WriteLine("");
							this.rdr.WriteLine("");
							this.rdr.WriteLine("");
							this.rdr.WriteLine("");
						}
					}
					else
					{
						if (this.dsMain.Tables[1].Rows.Count % 9 != 0)
						{
							if (this.pagenumber >= this.fpage && this.pagenumber <= this.lpage)
							{
								this.rdr.WriteLine("");
								this.rdr.WriteLine("");
							}
						}
					}
					this.pagerecno = 0;
					this.pagenumber++;
					this.headerflag = true;
				}
				this.slno++;
			}
			int num3 = this.dsMain.Tables[1].Rows.Count / 9;
			if (num3 + 1 == this.pagenumber)
			{
				if (this.dsMain.Tables[1].Rows.Count % 9 == 0)
				{
					this.headerflag = false;
					this.pagerecno = 10;
				}
				else
				{
					this.headerflag = false;
				}
			}
			if (!this.headerflag)
			{
				if (this.pagenumber == this.lpage)
				{
					for (int i = this.pagerecno; i <= 9; i++)
					{
						this.rdr.WriteLine("");
					}
					this.rdr.WriteLine(this.GetRightFormatedText("-------------------", 80) + " ");
					if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["ReturnSales"])))
					{
						this.rdr.Write(this.GetFormatedText("     " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["ReturnSales"]), 50));
						this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 27));
						this.rdr.WriteLine("");
					}
					else
					{
						this.rdr.Write(this.GetFormatedText("     TOTAL", 30));
						this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 47));
						this.rdr.WriteLine("");
					}
					if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["LessAmount"])))
					{
						if (Convert.ToDouble(this.dsMain.Tables[0].Rows[0]["LessAmount"]) > 0.0)
						{
							this.rdr.Write(this.GetFormatedText("     LESS AMOUNT", 30));
							this.rdr.Write(this.GetRightFormatedText("-" + Convert.ToString(this.dsMain.Tables[0].Rows[0]["LessAmount"]), 47));
							this.rdr.WriteLine("");
						}
					}
					if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["OtherCharges"])))
					{
						if (Convert.ToDouble(this.dsMain.Tables[0].Rows[0]["OtherCharges"]) > 0.0)
						{
							this.rdr.Write(this.GetFormatedText("     OTHERS", 30));
							this.rdr.Write(this.GetRightFormatedText("-" + Convert.ToString(this.dsMain.Tables[0].Rows[0]["OtherCharges"]), 47));
							this.rdr.WriteLine("");
						}
					}
					this.rdr.Write(this.GetFormatedText("     GRAND TOTAL", 30));
					this.rdr.Write('\u000e' + this.GetRightFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["GrnandTotal"]), 24));
					this.rdr.WriteLine("");
					this.rdr.Write(this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Assist"]), 30));
					this.rdr.WriteLine("");
					this.rdr.Write(this.GetCenterdFormatedText("Check the goods carefully before taking delivery", 80));
					this.rdr.WriteLine("");
					this.rdr.Write(this.GetFormatedText("    No guarantee for glass fittings & bulbs. Company service guarantee only", 80));
					this.rdr.WriteLine("");
					this.rdr.Write(this.GetFormatedText("    for guarantee items. Goods once sold cannot be taken back or exchanged.", 80));
				}
				if (this.pagerecno <= 9)
				{
					this.rdr.WriteLine("");
					this.rdr.WriteLine("");
					this.rdr.WriteLine("");
				}
			}
		}
		public void PrintFooter(bool val)
		{
			if (val)
			{
				this.rdr.WriteLine(this.GetRightFormatedText("-------------------", 80) + " ");
				this.rdr.Write(this.GetFormatedText("     TOTAL", 30));
				this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 47));
				this.rdr.WriteLine("");
			}
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
              
                this.rdr.WriteLine((char)18 + this.GetCenterdFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["CompanyName"]), 40));
				this.rdr.WriteLine();
                this.rdr.WriteLine((char)(15) + this.GetCenterdFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["DoorNo"]), 80));
				this.rdr.WriteLine(this.GetCenterdFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Address1"]), 80));
				this.rdr.WriteLine(this.GetCenterdFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Phone1"]), 80));
                this.rdr.WriteLine((char)18 + this.GetCenterdFormatedText("ESTIMATION", 40)+ "\n" + (char)15 );
				this.rdr.WriteLine("");
				this.rdr.WriteLine("Date : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Date"]), 50));
				this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Estimationid"]), 80));
				this.rdr.WriteLine("");
				this.rdr.WriteLine("To : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["customername"]), 50));
				this.PrintLine();
				this.rdr.Write("{0,-3}", this.GetFormatedText("SNo", 3) + " ");
				this.rdr.Write("{0,-40}", this.GetFormatedText("PRODUCT NAME", 40) + " ");
				this.rdr.Write("{0,-12}", this.GetFormatedText("QUANTITY", 12) + " ");
				this.rdr.Write("{0,-9}", this.GetRightFormatedText("RATE", 9) + " ");
				this.rdr.Write("{0,-9}", this.GetRightFormatedText("AMOUNT", 9) + " ");
                this.rdr.WriteLine("");
				this.PrintLine();
			}
		}
		public void PritPageHeader()
		{
			this.rdr.WriteLine("SLNo  Name                            Qty         Rate       T.Amt         ");
			this.PrintLine();
		}
		public void PrintDetails2()
		{
			this.rdr.WriteLine("SLNo |Name                 |Qty|MRP   |Rate  |T.Amt  ");
			this.PrintLine();
			for (int i = 1; i <= 30; i++)
			{
				this.rdr.Write("{0,-4}", this.GetFormatedText(i.ToString(), 5) + " ");
				this.rdr.Write("{0,-20}", this.GetFormatedText("Product 1", 21) + " ");
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
