using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InvBal;
namespace Inventory.Accounts
{
    public partial class CustomerAccount : Form
    {
        QuotationBal objQuotationBal = new QuotationBal();
        public CustomerAccount()
        {
            InitializeComponent();



            textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox1.AutoCompleteCustomSource = AutoCompleteLoads();
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }


        public AutoCompleteStringCollection AutoCompleteLoads()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationBal.GetAccountCustomer();
            string[] arr = new string[st.Rows.Count];
            for (int i = 0; i < st.Rows.Count; i++)
            {
                arr[i] = st.Rows[i]["Name"].ToString();
            }

            for (int i = 0; i < arr.Length; i++)
            {
                //var combined = string.Join(", ", arr);
                var combined = arr[i];
                str.Add(combined);
            }

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}

            return str;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                DataTable dt = objQuotationBal.GetCustomerDetail(textBox1.Text);

                dataGridView1.DataSource = dt;
                total();
                
            }
            else
            {
                MessageBox.Show("Please Select the Customer Name");
            }
        }

        public void total()
        {
            try
            {
                double totalamount = 0.0D, TotalPips = 0.0D;
                double value = 0.0, value1 = 0.0;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {

                    if (string.IsNullOrEmpty(Convert.ToString(dataGridView1.Rows[i].Cells[2].Value)))
                    {
                        value = 0.0;
                    }
                    else
                    {
                        value = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value);
                    }


                    totalamount = totalamount + value;



                    if (string.IsNullOrEmpty(Convert.ToString(dataGridView1.Rows[i].Cells[3].Value)))
                    {
                        value1 = 0.0;
                    }
                    else
                    {
                        value1 = Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value);
                    }


                    TotalPips = TotalPips + value1;

                    //totalquantity = totalquantity + Convert.ToInt32(dgvNew.Rows[i].Cells[5].Value);
                }

                //  totalquantity = Math.Round(totalquantity);

                //totalamount = Math.Round(totalamount);

                string[] str = totalamount.ToString().Split('.');
                if (str.Length > 1)
                {
                    double num1 = Convert.ToDouble("0." + str[1]);

                    if (num1 >= 0.5)
                    {
                        totalamount = Math.Ceiling(totalamount);
                    }
                    else
                    {
                        totalamount = Math.Floor(totalamount);
                    }

                }




                string[] strs = TotalPips.ToString().Split('.');
                if (strs.Length > 1)
                {
                    double num1 = Convert.ToDouble("0." + strs[1]);

                    if (num1 >= 0.5)
                    {
                        TotalPips = Math.Ceiling(TotalPips);
                    }
                    else
                    {
                        TotalPips = Math.Floor(TotalPips);
                    }

                }

                double Total = TotalPips - totalamount;

                debit.Text = String.Format("{0:00.00}", TotalPips);
                //lbltotalquantity.Text = Convert.ToString(totalquantity);
                credit.Text = String.Format("{0:00.00}", totalamount);
                label4.Text = String.Format("{0:00.00}", Total);

            }
            catch
            {

            }
        }

     
    }
}
