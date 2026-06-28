using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Sales
{
    public partial class frmPrintPreview : Form
    {
        public string fileName;
        public frmPrintPreview()
        {
            InitializeComponent();
        }

        private void frmPrintPreview_Load(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(fileName))
                rtc.LoadFile(fileName, RichTextBoxStreamType.PlainText);
            else
                rtc.Text = "Data file not found";
        }
    }
}
