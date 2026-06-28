using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory
{
    public partial class Mainform : Form
    {
        public Mainform()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Dispose();
                Application.Exit();
            }

            else if (keyData == Keys.Enter)
            {
                this.Hide();
                Login l = new Login();
                l.Show();
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
