using InvBal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory.Service
{
    public partial class CancelledTray : Form
    {
        DataTable dtAdd = new DataTable();
        bool first = true;
        ServiceRequestBAL objServiceRequestBAL = new ServiceRequestBAL();
        string Service = string.Empty;
        public CancelledTray()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            pnlLabelSearch.Visible = true;
            // vLabel1.Visible = true;
            pnlSearch.Visible = false;
            splitContainer1.Panel1Collapsed = true;
            Searchresultload();
            Searchresult();

        }
        private void ServiceRequest_Load(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Searchresult();
        }
        public void Searchresultload()
        {
            DataTable dt = objServiceRequestBAL.SearchDataissuedTray("Cancel");
            dgvProductqty.DataSource = dt;

        }
        public void Searchresult()
        {
            dgvSearch.DataSource = null;
            string service = Program.Service;
            DataTable dt = objServiceRequestBAL.SearchData(txtsearch1.Text, txtsearch2.Text, service);
            dgvSearch.DataSource = dt;
            lblItemCount.Text = Convert.ToString(dt.Rows.Count);
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lblservicerequestid.Text = Convert.ToString(dgvSearch.Rows[e.RowIndex].Cells["ServiceReqId"].Value);
                dtAdd = objServiceRequestBAL.GetserviceProductTray("Cancel", lblservicerequestid.Text.Trim());
                dgvProductqty.DataSource = dtAdd;
            }
        }

        private void vLabel3_Click(object sender, EventArgs e)
        {
            if (pnlLabelSearch.Visible == true)
            {
                pnlLabelSearch.Visible = false;
                //vLabel1.Visible = false;
                pnlSearch.Visible = true;
                splitContainer1.Panel1Collapsed = false;
                //cbxSearchOrderNo.Focus();
            }
        }

        private void vLabel2_Click(object sender, EventArgs e)
        {
            if (pnlOrder.Visible == true)
            {
                pnlOrder.Visible = false;
                vLabel2.Visible = false;
                pnlCollapse2.Visible = true;
                splitContainer1.Panel2Collapsed = false;
                pbxCollapse.Visible = true;
                pbxRightCollapse.Visible = true;

                pnlLabelSearch.Visible = true;
                // vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;

                //this.dgvSearch.Columns["ItemCode"].Visible = false;
                //this.dgvSearch.Columns["BarCode"].Visible = false;
                //this.dgvSearch.Columns["Brandname"].Visible = false;
            }
        }

        private void pbxCollapse_Click(object sender, EventArgs e)
        {
            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                //vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void pbxRightCollapse_Click(object sender, EventArgs e)
        {
            if (pnlCollapse2.Visible == true)
            {
                pnlOrder.Visible = true;
                vLabel2.Visible = true;
                pnlCollapse2.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                pbxCollapse.Visible = false;
                pbxRightCollapse.Visible = false;

                //this.dgvSearch.Columns["ItemCode"].Visible = true;
                //this.dgvSearch.Columns["BarCode"].Visible = true;
                //this.dgvSearch.Columns["Brandname"].Visible = true;
                //dgvSearch.Columns["BarCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Searchresultload();
        }
    }
}

//Procedure

//checksalesbillno
//SaveServiceRequest
//serviceproduct
//DeleteServiceRequest
//SaveServiceRequestProduct
//GetserviceProduct
//SearchServiceRequest
//UpdateServiceRefStatus
//SearchServiceRequestupdate

//table
//ServiceRequest
//ServiceRequestProduct