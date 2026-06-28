using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Inventory
{
    class SalesReports
    {
        public DataSet dsMain;
		public int pagenumber = 0;
		public bool status;
		public int fpage;
		public int lpage;
		private decimal Total = 0.0m;
		private StreamWriter rdr;
		public string BillType = "Sales";
		public string BillNo;
		public string BillDt;
		public string Clerk;
		public string ClientName = "";
		public decimal Discount;
		public decimal TotalAmt;
		public decimal NetAmount;
		public decimal MRPTotal = 0m;
		public decimal SavedTotal = 0m;
		public int linecount = 0;
		public int value = 0;
		public DataTable dt;
		public string fileName;
		public bool Party = false;
		private int slno = 1;
		private int pagerecno = 0;
		private bool headerflag = false;
		private bool showprevsum = false;
		private bool firstpage = true;
		public SalesReports()
		{
			this.fileName = "d:\\bill.txt";
			FileStream fileStream = File.Create(this.fileName);
			fileStream.Close();
		}
		public void pagebreak()
		{
			int num = 1;
			int i = 6;
			while (i >= num)
			{
				if (num == 6)
				{
					this.rdr.WriteLine("");
				}
				else
				{
					this.rdr.WriteLine("");
				}
				num++;
			}
		}
		public bool GenerateQuoation()
		{
			bool result = true;
			using (this.rdr = new StreamWriter(this.fileName))
			{
				this.rdr.WriteLine('\u001b' + "j" + 'l');
				this.rdr.WriteLine('\u001b' + "j" + 'l');
				this.rdr.WriteLine('\u001b' + "j" + 'l');
                this.rdr.WriteLine(" ");
                this.rdr.WriteLine(" ");
				this.PrintHeader();
				this.PrintDetails();
				this.Close();
			}
			return result;
		}
		public void PrintDetails()
		{
			this.slno = 1;
			int count = this.dsMain.Tables[1].Rows.Count;
			int num = 0;
			while (this.dsMain.Tables[1].Rows.Count >= this.slno)
			{
				if (this.headerflag)
				{
					this.PrintHeader();
					this.headerflag = false;
					this.showprevsum = true;
				}
				if (this.showprevsum)
				{
					this.rdr.Write("{0,-4}", this.GetFormatedText(" ", 4));
					this.rdr.Write("{0,-29}", this.GetFormatedText("B/F", 29));
					this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 44));
					this.rdr.WriteLine("");
					this.showprevsum = false;
				}
				num++;
				if (this.firstpage)
				{
					this.rdr.WriteLine("");
					this.firstpage = false;
				}
				this.rdr.Write("{0,-3}", this.GetFormatedText(this.slno.ToString(), 3) + " ");
				this.rdr.Write("{0,-32}", this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["ItemName"].ToString(), 32) + " ");
				this.rdr.Write("{0,-8}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Quantity"].ToString(), 8) + " ");
				this.rdr.Write("{0,-3}", this.GetFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["UOM"].ToString(), 3) + " ");
				this.rdr.Write("{0,-9}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Rate"].ToString(), 9) + " ");
				this.rdr.Write("{0,-9}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Vat"].ToString(), 9) + " ");
				this.rdr.Write("{0,-9}", this.GetRightFormatedText(this.dsMain.Tables[1].Rows[this.slno - 1]["Amount"].ToString(), 9) + " ");
				this.rdr.WriteLine("");
				this.Total += Convert.ToDecimal(this.dsMain.Tables[1].Rows[this.slno - 1]["Amount"]);
				this.pagerecno++;
				this.value = 10;
				int num2 = count / 10;
				decimal num3 = Convert.ToDecimal(count) / 10m;
				if (this.slno % this.value == 0)
				{
					this.PrintFooter();
					this.pagenumber++;
					if (this.dsMain.Tables[1].Rows.Count >= this.slno)
					{
						this.rdr.WriteLine("");
						this.rdr.Write(this.GetRightFormatedText("Contd..." + this.pagenumber.ToString(), 80));
						this.rdr.WriteLine("");
						this.rdr.WriteLine("");
					}
					else
					{
						this.rdr.WriteLine("");
					}
					this.pagebreak();
					this.pagerecno = 0;
					this.headerflag = true;
					if (this.dsMain.Tables[1].Rows.Count == this.slno)
					{
						if (this.headerflag)
						{
							this.PrintHeader();
							this.headerflag = false;
							this.showprevsum = true;
						}
						if (this.showprevsum)
						{
							this.rdr.Write("{0,-4}", this.GetFormatedText(" ", 4));
							this.rdr.Write("{0,-29}", this.GetFormatedText("B/F", 29));
							this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 44));
							this.rdr.WriteLine("");
							this.showprevsum = false;
						}
						this.headerflag = false;
					}
				}
				else
				{
					if (this.slno == this.dsMain.Tables[1].Rows.Count)
					{
						int num4 = count - num2 * 10;
						if (4 <= num4)
						{
							for (int i = this.pagerecno; i < this.value; i++)
							{
								this.rdr.WriteLine("");
							}
							this.PrintFooter();
							this.pagenumber++;
							this.rdr.WriteLine("");
							if (this.dsMain.Tables[1].Rows.Count >= this.slno)
							{
								this.rdr.WriteLine("");
								this.rdr.Write(this.GetRightFormatedText("Contd..." + this.pagenumber.ToString(), 80));
								this.rdr.WriteLine("");
								this.rdr.WriteLine("");
							}
							else
							{
								this.rdr.WriteLine("");
							}
							this.pagebreak();
							this.pagerecno = 0;
							this.PrintHeader();
							this.rdr.Write("{0,-4}", this.GetFormatedText(" ", 4));
							this.rdr.Write("{0,-29}", this.GetFormatedText("B/F", 29));
							this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 44));
							this.rdr.WriteLine("");
						}
					}
				}
				this.slno++;
			}
			if (!this.headerflag)
			{
				if (num <= 4)
				{
					this.pagerecno = num;
				}
				for (int i = this.pagerecno; i < 4; i++)
				{
					this.rdr.WriteLine("");
				}
				this.PrintFooter();
				if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["LessAmount"])))
				{
					if (Convert.ToDouble(this.dsMain.Tables[0].Rows[0]["LessAmount"]) > 0.0)
					{
						this.rdr.Write(this.GetFormatedText("     LESS AMOUNT", 30));
						this.rdr.Write(this.GetRightFormatedText("-" + Convert.ToString(this.dsMain.Tables[0].Rows[0]["LessAmount"]), 47));
						this.rdr.WriteLine("");
					}
					else
					{
						this.rdr.WriteLine("");
					}
				}
				else
				{
					this.rdr.WriteLine("");
				}
				if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Others"])))
				{
					if (Convert.ToDouble(this.dsMain.Tables[0].Rows[0]["Others"]) > 0.0)
					{
						this.rdr.Write(this.GetFormatedText("     OTher AMOUNT", 30));
						this.rdr.Write(this.GetRightFormatedText(" " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Others"]), 47));
						this.rdr.WriteLine("");
					}
					else
					{
						this.rdr.WriteLine("");
					}
				}
				else
				{
					this.rdr.WriteLine("");
				}
				this.rdr.Write(this.GetFormatedText("     GRAND TOTAL", 30));
				this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["GrandTotal"]), 50));
				string str = string.Empty;
				if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Valdata1"])))
				{
					str = str + "     " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Valdata1"]), 35);
				}
				if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Valdata2"])))
				{
					str = str + "     " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Valdata2"]), 35);
				}
				this.rdr.WriteLine("");
				this.rdr.WriteLine(str);
				this.rdr.WriteLine("E.&O.E.(" + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["NumberInwords"]), 70));
				this.PrintLine();
				this.rdr.WriteLine("");
				this.rdr.Write(this.GetFormatedText("Customer's Signature", 30));
				this.rdr.Write(this.GetRightFormatedText("For " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["CompanyName"]), 50));
				this.rdr.WriteLine("");
				this.rdr.WriteLine("");
			}
			this.pagebreak();
			this.pageEnd();
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
		public void PrintFooter()
		{
			this.rdr.WriteLine(this.GetRightFormatedText("-------------------", 80));
			this.rdr.Write(this.GetFormatedText("     TOTAL", 30));
			this.rdr.Write(this.GetRightFormatedText(Convert.ToString(this.Total), 47));
			this.rdr.Write("   ", 3);
			if (!this.Party)
			{
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
				this.linecount = 0;
                this.rdr.WriteLine('\u000e' + this.GetCenterdFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["CompanyName"]), 40));
				string cont = Convert.ToString(this.dsMain.Tables[0].Rows[0]["DoorNo"]) + " " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["City1"]);
				this.rdr.WriteLine("");
                this.rdr.WriteLine((char)18 + this.GetCenterdFormatedText(cont, 80));
				this.rdr.WriteLine('\u000e' + this.GetCenterdFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["BillHeader"]), 40) + "\n" + (char)18);
				this.rdr.WriteLine("TIN : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Tin"]), 45) + Convert.ToString(this.dsMain.Tables[0].Rows[0]["Phone1"]), 35);
				this.rdr.WriteLine("CST : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Cst"]), 55) + "DATE : " + Convert.ToString(this.dsMain.Tables[0].Rows[0]["salesdate"]), 30);
				this.rdr.WriteLine("BILL NO : " + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Billno"]), 30));
				this.rdr.WriteLine("TO : "  + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["CustomerName"]), 37));
				string str = string.Empty;
				if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Customeraddress1"])))
				{
					str = Convert.ToString(this.dsMain.Tables[0].Rows[0]["Customeraddress1"]);
				}
				string text = string.Empty;
				if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Customercity"])))
				{
					text = Convert.ToString(this.dsMain.Tables[0].Rows[0]["Customercity"]);
				}
				string text2 = str + text;
				if (string.IsNullOrEmpty(text2))
				{
					this.rdr.WriteLine("");
				}
				else
				{
					string cont2 = str + "  " + text;
					this.rdr.WriteLine("     " + this.GetFormatedText(cont2, 74));
				}
				if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Customerphone"])))
				{
					this.rdr.WriteLine("     "  + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Customerphone"]), 37));
				}
				else
				{
					this.rdr.WriteLine("");
				}
				if (!string.IsNullOrEmpty(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Party"])))
				{
					this.rdr.WriteLine("Party TIN/CST :" + this.GetFormatedText(Convert.ToString(this.dsMain.Tables[0].Rows[0]["Party"]), 65));
					this.Party = true;
				}
				this.PrintLine();
				this.rdr.Write("{0,-3}", this.GetFormatedText("SNo", 3) + " ");
				this.rdr.Write("{0,-32}", this.GetFormatedText("PRODUCT NAME", 32) + " ");
				this.rdr.Write("{0,-10}", this.GetFormatedText("QUANTITY", 10) + " ");
				this.rdr.Write("{0,-9}", this.GetRightFormatedText("RATE", 9) + " ");
				this.rdr.Write("{0,-9}", this.GetRightFormatedText("VAT%", 9) + " ");
				this.rdr.Write("{0,-9}", this.GetRightFormatedText("AMOUNT", 9) + " ");
				this.rdr.WriteLine("");
				this.PrintLine();
			}
		}
		public void PritPageHeader()
		{
			this.rdr.WriteLine("SLNo  Name                         Qty       Rate       Vat      T.Amt         ");
			this.PrintLine();
		}
		public void PrintDetails2()
		{
			this.rdr.WriteLine("SLNo |Name                 |Qty|MRP   |Rate  |Vat  |T.Amt    ");
			this.PrintLine();
			for (int i = 1; i <= 30; i++)
			{
				this.rdr.Write("{0,-4}", this.GetFormatedText(i.ToString(), 5) + " ");
				this.rdr.Write("{0,-20}", this.GetFormatedText("Product 1", 21) + " ");
				this.rdr.Write("{0,-2}", this.GetFormatedText("2", 3) + " ");
				this.rdr.Write("{0,-6}", this.GetFormatedText("125.00", 6) + " ");
				this.rdr.Write("{0,-6}", this.GetFormatedText("250.00", 6) + " ");
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
			string str = "";
			for (int i = 1; i <= 80; i++)
			{
				str += "-";
			}
			this.rdr.WriteLine(str);
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

