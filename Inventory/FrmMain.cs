using InvBal;
using Inventory.Accounts;
using Inventory.Adjustment;
using Inventory.Commission;
using Inventory.Masters;
using Inventory.Movement;
using Inventory.Purchase;
using Inventory.Report;
using Inventory.Sales;
using Inventory.Service;
using Inventory.Tool;
using Inventory.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Inventory
{
    public partial class FrmMain : Form
    {
        QuotationBal objQuotationbal = new QuotationBal();
        private ToolStripMenuItem productSyncQueueToolStripMenuItem;
        public FrmMain(string userid)
        {
            InitializeComponent();
            AddProductSyncQueueMenuItem();
            ApplyCloudEyeDelightMainTheme();
            ApplyMainOfficeProductRestrictions();
            FixMdiClientArea();
            this.WindowState = FormWindowState.Maximized;
            Btndelete.Cursor = Cursors.Hand;
            getmenu(userid);
            this.ActiveControl = menuStrip1;
            string configvalue2 = ConfigurationManager.AppSettings["Delete"];
            if (configvalue2 == "Yes")
            {
                Btndelete.Visible = true;
            }
            else
            {
                Btndelete.Visible = false;
            }
            if (Program.Userrole == "Admin")
            {
                passwordSetupToolStripMenuItem.Visible = true;
            }
            else
            {
                passwordSetupToolStripMenuItem.Visible = false;
            }
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.menuStrip1.Dock = DockStyle.Top;
        }

        private void ApplyCloudEyeDelightMainTheme()
        {
            string versionNo = GetApplicationVersion();
            string branchCode = ConfigurationManager.AppSettings["BranchCode"];
            if (branchCode == null || branchCode.Trim() == "")
            {
                branchCode = "BRANCH NOT SET";
            }
            branchCode = branchCode.Trim();

            this.Text = "RR Electricals - " + branchCode + " | Version " + versionNo;
            this.BackColor = Color.FromArgb(236, 240, 245);

            foreach (Control ctl in this.Controls)
            {
                if (ctl is MdiClient)
                {
                    ctl.BackColor = Color.FromArgb(236, 240, 245);
                }
            }

            // IMPORTANT:
            // Do not add a new top/bottom panel here. This form is an MDI parent.
            // Extra panels reduce/hide the inner child pages. So all branding is shown
            // inside the existing menu strip blue bar only.
            Control oldHeader = this.Controls["pnlCloudEyeMainHeader"];
            if (oldHeader != null)
            {
                this.Controls.Remove(oldHeader);
                oldHeader.Dispose();
            }

            Control oldFooter = this.Controls["pnlCloudEyeMainFooter"];
            if (oldFooter != null)
            {
                this.Controls.Remove(oldFooter);
                oldFooter.Dispose();
            }

            menuStrip1.SuspendLayout();

            menuStrip1.AutoSize = false;
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Height = 37; // keep original row height, so inner pages will not hide
            menuStrip1.Width = this.ClientSize.Width;
            menuStrip1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            menuStrip1.BackColor = Color.FromArgb(12, 74, 110);
            menuStrip1.BackgroundImage = null;
            menuStrip1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            menuStrip1.RenderMode = ToolStripRenderMode.Professional;
            menuStrip1.Renderer = new CloudEyeMenuRenderer(new CloudEyeMenuColorTable());
            menuStrip1.ShowItemToolTips = true;
            menuStrip1.Padding = new Padding(6, 2, 6, 2);

            // Remove old branding items if the theme method is called again.
            for (int i = menuStrip1.Items.Count - 1; i >= 0; i--)
            {
                if (menuStrip1.Items[i].Name != null && menuStrip1.Items[i].Name.StartsWith("ce_"))
                {
                    menuStrip1.Items.RemoveAt(i);
                }
            }

            // No left-side RR Electricals label here. Keep existing menu area width and height unchanged.

            ToolStripLabel poweredLabel = new ToolStripLabel("CloudEye Delight");
            poweredLabel.Name = "ce_lblPowered";
            poweredLabel.Alignment = ToolStripItemAlignment.Right;
            poweredLabel.ForeColor = Color.FromArgb(224, 242, 254);
            poweredLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            poweredLabel.Margin = new Padding(6, 0, 2, 0);
            poweredLabel.Padding = new Padding(2, 0, 2, 0);

            ToolStripLabel versionLabel = new ToolStripLabel("v" + versionNo);
            versionLabel.Name = "ce_lblVersion";
            versionLabel.Alignment = ToolStripItemAlignment.Right;
            versionLabel.ForeColor = Color.FromArgb(31, 41, 55);
            versionLabel.BackColor = Color.FromArgb(250, 204, 21);
            versionLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            versionLabel.Margin = new Padding(4, 0, 4, 0);
            versionLabel.Padding = new Padding(6, 1, 6, 1);

            ToolStripLabel branchLabel = new ToolStripLabel(branchCode);
            branchLabel.Name = "ce_lblBranch";
            branchLabel.Alignment = ToolStripItemAlignment.Right;
            branchLabel.ForeColor = Color.White;
            branchLabel.BackColor = Color.FromArgb(20, 184, 166);
            branchLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            branchLabel.Margin = new Padding(4, 0, 4, 0);
            branchLabel.Padding = new Padding(6, 1, 6, 1);

            menuStrip1.Items.Add(poweredLabel);
            menuStrip1.Items.Add(versionLabel);
            menuStrip1.Items.Add(branchLabel);

            foreach (ToolStripItem item in menuStrip1.Items)
            {
                if (item.Name != null && item.Name.StartsWith("ce_"))
                {
                    continue;
                }

                item.ForeColor = Color.White;
                item.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                item.Padding = new Padding(7, 0, 7, 0);
                item.Margin = new Padding(1, 0, 1, 0);
                item.BackColor = Color.Transparent;

                ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                if (menuItem != null)
                {
                    menuItem.ShowShortcutKeys = false;
                    menuItem.DropDown.BackColor = Color.White;
                    menuItem.DropDown.Padding = new Padding(2, 4, 2, 4);

                    foreach (ToolStripItem dropItem in menuItem.DropDownItems)
                    {
                        dropItem.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
                        dropItem.ForeColor = Color.FromArgb(30, 41, 59);
                        dropItem.Padding = new Padding(8, 3, 8, 3);
                        dropItem.Margin = new Padding(1, 1, 1, 1);
                    }
                }
            }

            Btndelete.Top = menuStrip1.Bottom + 5;
            Btndelete.BackColor = Color.FromArgb(220, 38, 38);
            Btndelete.ForeColor = Color.White;
            Btndelete.FlatStyle = FlatStyle.Flat;
            Btndelete.FlatAppearance.BorderSize = 0;

            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            menuStrip1.BringToFront();
        }

        private string GetApplicationVersion()
        {
            try
            {
                System.Reflection.FieldInfo versionField = typeof(Program).GetField("AppVersion");
                if (versionField == null)
                {
                    versionField = typeof(Program).GetField("ApplicationVersion");
                }
                if (versionField == null)
                {
                    versionField = typeof(Program).GetField("CloudEyeVersion");
                }

                if (versionField != null)
                {
                    object versionValue = versionField.GetValue(null);
                    if (versionValue != null && versionValue.ToString().Trim() != "")
                    {
                        return versionValue.ToString().Trim();
                    }
                }
            }
            catch
            {
            }

            return "5.0.0";
        }

        private void purchaseOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (Program.objPurchaseOrder == null)
            {
                PurchaseOrder PurchaseOrder = new PurchaseOrder();
                Program.objPurchaseOrder = PurchaseOrder;
            }
            else
            {
                Program.objPurchaseOrder.Close();
                PurchaseOrder PurchaseOrder = new PurchaseOrder();
                Program.objPurchaseOrder = PurchaseOrder;
            }

            Program.objPurchaseOrder.MdiParent = this;
            Program.objPurchaseOrder.Show();

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                DialogResult result = MessageBox.Show("Do you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                try
                {
                    if (result == DialogResult.Yes)
                    {
                        this.Dispose();
                        Application.Exit();
                    }
                }
                catch
                {
                    Application.Exit();
                }
            }




            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void productToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void purchaseReceiptToolStripMenuItem_Click(object sender, EventArgs e)
        {



            if (Program.objPurchaseReceipt == null)
            {
                PurchaseReceipt objPurchaseReceipt = new PurchaseReceipt();
                Program.objPurchaseReceipt = objPurchaseReceipt;

            }
            else
            {
                Program.objPurchaseReceipt.Close();
                PurchaseReceipt objPurchaseReceipt = new PurchaseReceipt();
                Program.objPurchaseReceipt = objPurchaseReceipt;
            }

            Program.objPurchaseReceipt.MdiParent = this;
            Program.objPurchaseReceipt.Show();

        }

        private void ApplyMainOfficeProductRestrictions()
        {
            if (BranchAccess.IsMainOffice)
            {
                return;
            }

            productsToolStripMenuItem.Enabled = false;
            productsToolStripMenuItem.ToolTipText = BranchAccess.MainOfficeOnlyMessage;
            priceUploadToolStripMenuItem.Enabled = false;
            priceUploadToolStripMenuItem.ToolTipText = BranchAccess.MainOfficeOnlyMessage;
            if (productSyncQueueToolStripMenuItem != null)
            {
                productSyncQueueToolStripMenuItem.Enabled = false;
                productSyncQueueToolStripMenuItem.ToolTipText = BranchAccess.MainOfficeOnlyMessage;
            }
        }

        private void AddProductSyncQueueMenuItem()
        {
            productSyncQueueToolStripMenuItem = new ToolStripMenuItem();
            productSyncQueueToolStripMenuItem.Name = "productSyncQueueToolStripMenuItem";
            productSyncQueueToolStripMenuItem.Size = new Size(206, 22);
            productSyncQueueToolStripMenuItem.Text = "Product Sync Queue";
            productSyncQueueToolStripMenuItem.Click += new EventHandler(productSyncQueueToolStripMenuItem_Click);

            int insertIndex = toolsToolStripMenuItem.DropDownItems.IndexOf(priceUploadToolStripMenuItem);
            if (insertIndex >= 0)
            {
                toolsToolStripMenuItem.DropDownItems.Insert(insertIndex + 1, productSyncQueueToolStripMenuItem);
            }
            else
            {
                toolsToolStripMenuItem.DropDownItems.Add(productSyncQueueToolStripMenuItem);
            }
        }

        private void receiptOfGoodsReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReceiptOfGoodsReport report = new ReceiptOfGoodsReport();
            report.MdiParent = this;
            report.Show();
        }

        private void salesBillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSalesBillNew == null)
            {
                SalesBillNew SalesBillNew = new SalesBillNew();
                Program.objSalesBillNew = SalesBillNew;
            }
            else
            {
                Program.objSalesBillNew.Close();
                SalesBillNew SalesBillNew = new SalesBillNew();
                Program.objSalesBillNew = SalesBillNew;
            }
            Program.objSalesBillNew.MdiParent = this;
            Program.objSalesBillNew.Show();
        }

        private void salesQuotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSalesQuotation1 != null)
            {
                Program.objSalesQuotation1.Dispose();
                Program.objSalesQuotation1 = null;
            }

            Program.objSalesQuotation1 = new SalesQuotation1();
            Program.objSalesQuotation1.MdiParent = this;
            Program.objSalesQuotation1.Show();
            Program.objSalesQuotation1.WindowState = FormWindowState.Maximized;

        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!BranchAccess.IsMainOffice)
            {
                MessageBox.Show(BranchAccess.MainOfficeOnlyMessage);
                return;
            }

            if (Program.objProduct == null)
            {
                Masters.Product Product = new Masters.Product();
                Program.objProduct = Product;
            }
            else
            {
                Program.objProduct.Dispose();
                Masters.Product Product = new Masters.Product();
                Program.objProduct = Product;
            }
            Program.objProduct.MdiParent = this;
            Program.objProduct.Show();
        }

        private void commissionBillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objCommissionBill == null)
            {
                CommissionBill SalesQuotation1 = new CommissionBill();
                Program.objCommissionBill = SalesQuotation1;
            }
            else
            {
                Program.objCommissionBill.Dispose();
                CommissionBill SalesQuotation1 = new CommissionBill();
                Program.objCommissionBill = SalesQuotation1;
            }
            Program.objCommissionBill.MdiParent = this;
            Program.objCommissionBill.Show();
        }

        private void transactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (Program.objTransaction == null)
            //{
            //    TransactionRequest Transaction = new TransactionRequest();
            //    Program.objTransaction = Transaction;
            //}
            //else
            //{
            //    Program.objTransaction.Dispose();
            //    TransactionRequest Transaction = new TransactionRequest();
            //    Program.objTransaction = Transaction;
            //}
            //Program.objTransaction.MdiParent = this;
            //Program.objTransaction.Show();      
        }

        private void suppliersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSuppliers == null)
            {
                Suppliers sup = new Suppliers();
                Program.objSuppliers = sup;
            }
            else
            {
                Program.objSuppliers.Dispose();
                Suppliers sup = new Suppliers();
                Program.objSuppliers = sup;
            }
            Program.objSuppliers.MdiParent = this;
            Program.objSuppliers.Show();
        }

        private void customersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objCustomers == null)
            {
                Customers cus = new Customers();
                Program.objCustomers = cus;
            }
            else
            {
                Program.objCustomers.Dispose();
                Customers cus = new Customers();
                Program.objCustomers = cus;
            }
            Program.objCustomers.MdiParent = this;
            Program.objCustomers.Show();
        }

        private void referenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objReference == null)
            {
                Reference refe = new Reference();
                Program.objReference = refe;
            }
            else
            {
                Program.objReference.Dispose();
                Reference refe = new Reference();
                Program.objReference = refe;
            }
            Program.objReference.MdiParent = this;
            Program.objReference.Show();
        }

        private void uOMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objUOM == null)
            {
                UOM uom = new UOM();
                Program.objUOM = uom;
            }
            else
            {
                Program.objUOM.Dispose();
                UOM uom = new UOM();
                Program.objUOM = uom;
            }
            Program.objUOM.MdiParent = this;
            Program.objUOM.Show();
        }

        private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objCategory == null)
            {
                Category cat = new Category();
                Program.objCategory = cat;
            }
            else
            {
                Program.objCategory.Dispose();
                Category cat = new Category();
                Program.objCategory = cat;
            }
            Program.objCategory.MdiParent = this;
            Program.objCategory.Show();
        }

        private void commisionReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objCommissionPayment == null)
            {
                CommissionPayment objCommissionPayment = new CommissionPayment();
                Program.objCommissionPayment = objCommissionPayment;
            }
            else
            {
                Program.objCommissionPayment.Dispose();
                CommissionPayment objCommissionPayment = new CommissionPayment();
                Program.objCommissionPayment = objCommissionPayment;
            }
            Program.objCommissionPayment.MdiParent = this;
            Program.objCommissionPayment.Show();

        }



        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (System.Diagnostics.Process p in
                 System.Diagnostics.Process.GetProcesses())
                {
                    string sProcess = "Inventory.vshost";
                    if (p.ProcessName == sProcess)
                    {
                        p.Kill();

                    }

                    string sProcess1 = "Inventory.exe";
                    if (p.ProcessName == sProcess1)
                    {
                        p.Kill();

                    }
                }
                this.Dispose();
                Application.Exit();
            }
            catch
            {
                Application.Exit();
            }
        }

        private void cashRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objCashRequest == null)
            {
                CashRequest cashrequest = new CashRequest();
                Program.objCashRequest = cashrequest;
            }
            else
            {
                Program.objCashRequest.Dispose();
                CashRequest cashrequest = new CashRequest();
                Program.objCashRequest = cashrequest;
            }
            Program.objCashRequest.MdiParent = this;
            Program.objCashRequest.Show();
        }

        private void cashApprovalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objCashRequestApproval == null)
            {
                CashRequestApproval cashrequestapproval = new CashRequestApproval();
                Program.objCashRequestApproval = cashrequestapproval;
            }
            else
            {
                Program.objCashRequestApproval.Dispose();
                CashRequestApproval cashrequestapproval = new CashRequestApproval();
                Program.objCashRequestApproval = cashrequestapproval;
            }
            Program.objCashRequestApproval.MdiParent = this;
            Program.objCashRequestApproval.Show();
        }

        private void cashPaymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objCashPayment == null)
            {
                CashPayment objCashPayment = new CashPayment();
                Program.objCashPayment = objCashPayment;
            }
            else
            {
                Program.objCashPayment.Dispose();
                CashPayment objCashPayment = new CashPayment();
                Program.objCashPayment = objCashPayment;
            }
            Program.objCashPayment.MdiParent = this;
            Program.objCashPayment.Show();
        }

        private void materialMovementToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (Program.objWarrantyList == null)
            {
                WarrantyList objWarrantyList = new WarrantyList();
                Program.objWarrantyList = objWarrantyList;
            }
            else
            {
                Program.objWarrantyList.Close();
                WarrantyList objWarrantyList = new WarrantyList();
                Program.objWarrantyList = objWarrantyList;
            }
            Program.objWarrantyList.MdiParent = this;
            Program.objWarrantyList.Show();
        }

        private void issuedReceivedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objFrmIssuedReceived == null)
            {


                FrmIssuedReceived FrmIssuedReceived = new FrmIssuedReceived();
                Program.objFrmIssuedReceived = FrmIssuedReceived;
            }
            else
            {
                Program.objFrmIssuedReceived.Close();
                FrmIssuedReceived FrmIssuedReceived = new FrmIssuedReceived();
                Program.objFrmIssuedReceived = FrmIssuedReceived;
            }
            Program.objFrmIssuedReceived.MdiParent = this;
            Program.objFrmIssuedReceived.Show();
        }

        private void issuedReceivedReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objFrmReceived == null)
            {


                FrmReceived objFrmReceived = new FrmReceived();
                Program.objFrmReceived = objFrmReceived;
            }
            else
            {
                Program.objFrmReceived.Close();
                FrmReceived objFrmReceived = new FrmReceived();
                Program.objFrmReceived = objFrmReceived;
            }
            Program.objFrmReceived.MdiParent = this;
            Program.objFrmReceived.Show();
        }

        private void locationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objLocation == null)
            {
                Location Location = new Location();
                Program.objLocation = Location;
            }
            else
            {
                Program.objLocation.Dispose();
                Location Location = new Location();
                Program.objLocation = Location;
            }
            Program.objLocation.MdiParent = this;
            Program.objLocation.Show();
        }

        private void stockReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objStockreport == null)
            {

                Stockreport objStockreport = new Stockreport();
                Program.objStockreport = objStockreport;
            }
            else
            {
                Program.objStockreport.Dispose();
                Stockreport objStockreport = new Stockreport();
                Program.objStockreport = objStockreport;
            }
            Program.objStockreport.MdiParent = this;
            Program.objStockreport.Show();
        }

        private void userCreationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objFrmUserCreation == null)
            {

                FrmUserCreation FrmUserCreation = new FrmUserCreation();
                Program.objFrmUserCreation = FrmUserCreation;
            }
            else
            {
                Program.objFrmUserCreation.Dispose();
                FrmUserCreation FrmUserCreation = new FrmUserCreation();
                Program.objFrmUserCreation = FrmUserCreation;
            }
            Program.objFrmUserCreation.MdiParent = this;
            Program.objFrmUserCreation.Show();
        }

        private void cashEndCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ObjCashEndClose == null)
            {
                CashEndClose objcashendclose = new CashEndClose();
                Program.ObjCashEndClose = objcashendclose;
            }
            else
            {
                Program.ObjCashEndClose.Dispose();
                CashEndClose objcashendclose = new CashEndClose();
                Program.ObjCashEndClose = objcashendclose;
            }
            Program.ObjCashEndClose.MdiParent = this;
            Program.ObjCashEndClose.Show();
        }

        private void cashEndVerificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ObjCashEndCloseVerification == null)
            {
                CashEndCloseVerification objcashendcloseVerification = new CashEndCloseVerification();
                Program.ObjCashEndCloseVerification = objcashendcloseVerification;
            }
            else
            {
                Program.ObjCashEndCloseVerification.Dispose();
                CashEndCloseVerification objcashendcloseVerification = new CashEndCloseVerification();
                Program.ObjCashEndCloseVerification = objcashendcloseVerification;
            }
            Program.ObjCashEndCloseVerification.MdiParent = this;
            Program.ObjCashEndCloseVerification.Show();
        }

        private void accountReceiveBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ObjAccountsReceivable == null)
            {
                AccountsReceivable objaccountsreceivable = new AccountsReceivable();
                Program.ObjAccountsReceivable = objaccountsreceivable;
            }
            else
            {
                Program.ObjAccountsReceivable.Dispose();
                AccountsReceivable objaccountsreceivable = new AccountsReceivable();
                Program.ObjAccountsReceivable = objaccountsreceivable;
            }
            Program.ObjAccountsReceivable.MdiParent = this;
            Program.ObjAccountsReceivable.Show();
        }

        private void accountPayableBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ObjAccountPayable == null)
            {
                AccountPayable objaccountspayable = new AccountPayable();
                Program.ObjAccountPayable = objaccountspayable;
            }
            else
            {
                Program.ObjAccountPayable.Dispose();
                AccountPayable objaccountspayable = new AccountPayable();
                Program.ObjAccountPayable = objaccountspayable;
            }
            Program.ObjAccountPayable.MdiParent = this;
            Program.ObjAccountPayable.Show();
        }

        private void windowSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objWindowSales == null)
            {
                WindowSales objWindowSales = new WindowSales();
                Program.objWindowSales = objWindowSales;
            }
            else
            {
                Program.objWindowSales.Close();
                WindowSales objWindowSales = new WindowSales();
                Program.objWindowSales = objWindowSales;
            }
            Program.objWindowSales.MdiParent = this;
            Program.objWindowSales.Show();
        }

        private void salesReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSalesReturn == null)
            {
                SalesReturn objSalesReturn = new SalesReturn();
                Program.objSalesReturn = objSalesReturn;
            }
            else
            {
                Program.objSalesReturn.Close();
                SalesReturn objSalesReturn = new SalesReturn();
                Program.objSalesReturn = objSalesReturn;
            }
            Program.objSalesReturn.MdiParent = this;
            Program.objSalesReturn.Show();
        }

        private void salesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSalesReport == null)
            {
                SalesReport objSalesReport = new SalesReport();
                Program.objSalesReport = objSalesReport;
            }
            else
            {
                Program.objSalesReport.Dispose();
                SalesReport objSalesReport = new SalesReport();
                Program.objSalesReport = objSalesReport;
            }
            Program.objSalesReport.MdiParent = this;
            Program.objSalesReport.Show();
        }

        private void slesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSalesNewreport == null)
            {
                SalesNewreport objSalesNewreport = new SalesNewreport();
                Program.objSalesNewreport = objSalesNewreport;
            }
            else
            {
                Program.objSalesNewreport.Dispose();
                SalesNewreport objSalesNewreport = new SalesNewreport();
                Program.objSalesNewreport = objSalesNewreport;
            }
            Program.objSalesNewreport.MdiParent = this;
            Program.objSalesNewreport.Show();
        }

        private void purchaseReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objFrmPurchaseReturn == null)
            {
                Purchase_Return.FrmPurchaseReturn objFrmPurchaseReturn = new Purchase_Return.FrmPurchaseReturn();
                Program.objFrmPurchaseReturn = objFrmPurchaseReturn;
            }
            else
            {
                Program.objFrmPurchaseReturn.Close();
                Purchase_Return.FrmPurchaseReturn objFrmPurchaseReturn = new Purchase_Return.FrmPurchaseReturn();
                Program.objFrmPurchaseReturn = objFrmPurchaseReturn;
            }
            Program.objFrmPurchaseReturn.MdiParent = this;
            Program.objFrmPurchaseReturn.Show();
        }

        private void commissionReportToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (Program.objCashRequestlist == null)
            {
                CashRequestlist objCashRequestlist = new CashRequestlist();
                Program.objCashRequestlist = objCashRequestlist;
            }
            else
            {
                Program.objCashRequestlist.Dispose();
                CashRequestlist objCashRequestlist = new CashRequestlist();
                Program.objCashRequestlist = objCashRequestlist;
            }
            Program.objCashRequestlist.MdiParent = this;
            Program.objCashRequestlist.Show();
        }

        private void stockDetailReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // public static FrmStockFinalRpt ObjFrmStockFinalRpt;
            if (Program.ObjFrmStockFinalRpt == null)
            {
                FrmStockFinalRpt objFrmStockReport = new FrmStockFinalRpt();
                Program.ObjFrmStockFinalRpt = objFrmStockReport;
            }
            else
            {
                Program.ObjFrmStockFinalRpt.Dispose();
                FrmStockFinalRpt objFrmStockReport = new FrmStockFinalRpt();
                Program.ObjFrmStockFinalRpt = objFrmStockReport;
            }
            Program.ObjFrmStockFinalRpt.MdiParent = this;
            Program.ObjFrmStockFinalRpt.Show();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

            //try
            //{
            //    foreach (Control control in this.Controls)
            //    {
            //        if (control is MdiClient)
            //        {
            //            control.BackgroundImage = this.BackgroundImage;
            //            break;
            //        }
            //    }
            //}
            //catch
            //{

            //}
        }


        private void rolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objFrmRole == null)
            {
                FrmRoles objFrmRole = new FrmRoles();
                Program.objFrmRole = objFrmRole;
            }
            else
            {
                Program.objFrmRole.Dispose();
                FrmRoles objFrmRole = new FrmRoles();
                Program.objFrmRole = objFrmRole;
            }
            Program.objFrmRole.MdiParent = this;
            Program.objFrmRole.Show();
        }

        public void getmenu(string id)
        {
            DataTable dt = objQuotationbal.Getmenuiem(id);
            Program.Dtmenu = dt;
            invisibleall();
            if (Program.Userrole == "Admin")
            {
                visibleall();
            }
            else
            {
                visibleMenu(dt);
            }

        }

        public void invisibleall()
        {
            //Purchase
            inventoryToolStripMenuItem.Visible = false;
            purchaseOrderToolStripMenuItem.Visible = false;
            purchaseReceiptToolStripMenuItem.Visible = false;
            receiptOfGoodsReportToolStripMenuItem.Visible = false;
            purchaseReturnToolStripMenuItem.Visible = false;
            purchaseReturnToolStripMenuItem.Visible = false;
            purchaseOrderListToolStripMenuItem.Visible = false;
            //purchaseAgeingToolStripMenuItem.Visible = false;

            //Sales
            productToolStripMenuItem.Visible = false;
            salesQuotationToolStripMenuItem.Visible = false;
            salesBillToolStripMenuItem.Visible = false;
            windowSaleToolStripMenuItem.Visible = false;
            // salesReturnToolStripMenuItem.Visible = false;
            salesToolStripMenuItem.Visible = false;
            quotationListToolStripMenuItem.Visible = false;
            followupQuotationReportToolStripMenuItem.Visible = false;
            estimationListToolStripMenuItem.Visible = false;
            // salesReturnListToolStripMenuItem.Visible = false;
            saleListToolStripMenuItem.Visible = false;
            // estimationAgeingToolStripMenuItem.Visible = false;
            expressSalesToolStripMenuItem.Visible = false;
            salesPDiToolStripMenuItem.Visible = false;


            //Master
            mastersToolStripMenuItem.Visible = false;
            productsToolStripMenuItem.Visible = false;
            suppliersToolStripMenuItem.Visible = false;
            customersToolStripMenuItem.Visible = false;
            referenceToolStripMenuItem.Visible = false;
            uOMToolStripMenuItem.Visible = false;
            categoryToolStripMenuItem.Visible = false;
            locationToolStripMenuItem.Visible = false;
            userCreationToolStripMenuItem.Visible = false;
            rolesToolStripMenuItem.Visible = false;
            employeeToolStripMenuItem.Visible = false;
            brandToolStripMenuItem.Visible = false;

            //Commission
            //commissionToolStripMenuItem.Visible = false;
            //commissionBillToolStripMenuItem.Visible = false;
            //commisionPaymentToolStripMenuItem.Visible = false;
            //commissionReportToolStripMenuItem.Visible = false;


            //Transaction
            transactionToolStripMenuItem.Visible = false;
            cashRequestToolStripMenuItem.Visible = false;
            //cashApprovalToolStripMenuItem.Visible = false;
            //cashPaymentToolStripMenuItem.Visible = false;
            //cashReceiptToolStripMenuItem.Visible = false;
            //cashRequestListToolStripMenuItem.Visible = false;
            //cashReceiptToolStripMenuItem1.Visible = false;


            //Report
            reportsToolStripMenuItem.Visible = false;
            followupQuotationReportToolStripMenuItem.Visible = false;
            slesReportToolStripMenuItem.Visible = false;
            // stockDetailReportToolStripMenuItem.Visible = false;
            cashTransactionReportToolStripMenuItem.Visible = false;
            issuedReceivedReportToolStripMenuItem1.Visible = false;
            userBillReportToolStripMenuItem.Visible = false;
            incentiveReportToolStripMenuItem.Visible = false;

            //Movement
            movementToolStripMenuItem.Visible = false;
            materialMovementToolStripMenuItem.Visible = false;
            issuedReceivedToolStripMenuItem.Visible = false;
            materialMovementToolStripMenuItem.Visible = false;

            // issuedReceivedReportToolStripMenuItem.Visible = false;



            //Tool
            toolsToolStripMenuItem.Visible = false;
            cashEndCloseToolStripMenuItem.Visible = false;
            cashEndVerificationToolStripMenuItem.Visible = false;


            //Account
            accountsToolStripMenuItem.Visible = false;
            accountReceiveBalanceToolStripMenuItem.Visible = false;
            // accountPayableBalanceToolStripMenuItem.Visible = false;


            //Adjudment
            adjustmentToolStripMenuItem.Visible = false;
            adjustmentForSalesToolStripMenuItem.Visible = false;
            adjustmentForPurchaseToolStripMenuItem.Visible = false;


            //Serveice
            servicesToolStripMenuItem.Visible = false;
            serviceRequestToolStripMenuItem.Visible = false;
            completedToolStripMenuItem.Visible = false;
            servicesToolStripMenuItem.Visible = false;
            issuedTrayToolStripMenuItem.Visible = false;
            cancelledTrayToolStripMenuItem.Visible = false;

            //Issued/Received

            //issuedReceivedToolStripMenuItem1.Visible = false;
            //issuedToolStripMenuItem.Visible = false;
            //receivedToolStripMenuItem.Visible = false;

            if (Program.UserName == "ARUN")
            {
                stockAjustmentToolStripMenuItem.Visible = true;
                viewStockAdjustmentToolStripMenuItem.Visible = true;
                materialTransactionReportToolStripMenuItem.Visible = true;
            }
            else
            {
                stockAjustmentToolStripMenuItem.Visible = false;
                viewStockAdjustmentToolStripMenuItem.Visible = false;
                materialTransactionReportToolStripMenuItem.Visible = false;
            }

        }

        public void visibleall()
        {
            //Purchase
            inventoryToolStripMenuItem.Visible = true;
            purchaseOrderToolStripMenuItem.Visible = true;
            purchaseReceiptToolStripMenuItem.Visible = true;
            receiptOfGoodsReportToolStripMenuItem.Visible = true;
            purchaseReturnToolStripMenuItem.Visible = true;
            purchaseReturnToolStripMenuItem.Visible = true;
            purchaseOrderListToolStripMenuItem.Visible = true;
            //purchaseAgeingToolStripMenuItem.Visible = true;

            //Sales
            productToolStripMenuItem.Visible = true;
            salesQuotationToolStripMenuItem.Visible = true;
            salesBillToolStripMenuItem.Visible = true;
            windowSaleToolStripMenuItem.Visible = false;
            // salesReturnToolStripMenuItem.Visible = true;
            salesToolStripMenuItem.Visible = true;
            quotationListToolStripMenuItem.Visible = true;
            estimationListToolStripMenuItem.Visible = true;
            //salesReturnListToolStripMenuItem.Visible = true;
            saleListToolStripMenuItem.Visible = true;
            //estimationAgeingToolStripMenuItem.Visible = true;
            //expressSalesToolStripMenuItem.Visible = true;
            salesPDiToolStripMenuItem.Visible = true;

            //Master
            mastersToolStripMenuItem.Visible = true;
            productsToolStripMenuItem.Visible = true;
            suppliersToolStripMenuItem.Visible = true;
            customersToolStripMenuItem.Visible = true;
            referenceToolStripMenuItem.Visible = true;
            uOMToolStripMenuItem.Visible = true;
            categoryToolStripMenuItem.Visible = true;
            locationToolStripMenuItem.Visible = true;
            userCreationToolStripMenuItem.Visible = true;
            rolesToolStripMenuItem.Visible = true;
            employeeToolStripMenuItem.Visible = true;
            brandToolStripMenuItem.Visible = true;

            //Commission
            //commissionToolStripMenuItem.Visible = true;
            //commissionBillToolStripMenuItem.Visible = true;
            //commisionPaymentToolStripMenuItem.Visible = true;
            //commissionReportToolStripMenuItem.Visible = true;


            //Transaction
            transactionToolStripMenuItem.Visible = true;
            cashRequestToolStripMenuItem.Visible = true;
            //cashApprovalToolStripMenuItem.Visible = true;
            //cashPaymentToolStripMenuItem.Visible = true;
            //cashReceiptToolStripMenuItem.Visible = true;
            //cashRequestListToolStripMenuItem.Visible = true;
            //cashReceiptToolStripMenuItem1.Visible = false;


            //Report
            reportsToolStripMenuItem.Visible = true;
            followupQuotationReportToolStripMenuItem.Visible = true;
            slesReportToolStripMenuItem.Visible = true;
            // stockDetailReportToolStripMenuItem.Visible = true;
            cashTransactionReportToolStripMenuItem.Visible = true;
            issuedReceivedReportToolStripMenuItem1.Visible = true;
            //stockAjustmentToolStripMenuItem.Visible = true;
            viewStockAdjustmentToolStripMenuItem.Visible = false;
            materialTransactionReportToolStripMenuItem.Visible = true;
            userBillReportToolStripMenuItem.Visible = false;
            incentiveReportToolStripMenuItem.Visible = true;

            //Movement
            movementToolStripMenuItem.Visible = true;
            materialMovementToolStripMenuItem.Visible = true;
            issuedReceivedToolStripMenuItem.Visible = true;
            materialMovementToolStripMenuItem.Visible = true;
            // issuedReceivedReportToolStripMenuItem.Visible = true;



            //Tool
            toolsToolStripMenuItem.Visible = true;
            cashEndCloseToolStripMenuItem.Visible = true;
            cashEndVerificationToolStripMenuItem.Visible = true;


            //Account
            accountsToolStripMenuItem.Visible = true;
            accountReceiveBalanceToolStripMenuItem.Visible = true;
            //accountPayableBalanceToolStripMenuItem.Visible = true;


            //Adjudment
            adjustmentToolStripMenuItem.Visible = false;
            adjustmentForSalesToolStripMenuItem.Visible = true;
            adjustmentForPurchaseToolStripMenuItem.Visible = true;


            //Serveice
            servicesToolStripMenuItem.Visible = true;
            serviceRequestToolStripMenuItem.Visible = true;
            completedToolStripMenuItem.Visible = true;
            servicesToolStripMenuItem.Visible = true;
            issuedTrayToolStripMenuItem.Visible = true;
            cancelledTrayToolStripMenuItem.Visible = true;


            //Issued/Received

            //issuedReceivedToolStripMenuItem1.Visible = true;
            //issuedToolStripMenuItem.Visible = true;
            //receivedToolStripMenuItem.Visible = true;



            issuedReceivedReportToolStripMenuItem1.Visible = false;

        }

        public void visibleMenu(DataTable dt)
        {
            string menu = string.Empty;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                menu = Convert.ToString(dt.Rows[i]["Data"]).Trim();

                //Purchase
                if (menu == "Purchase")
                {
                    inventoryToolStripMenuItem.Visible = true;
                }
                else if (menu == "PurchaseOrder")
                {
                    purchaseOrderToolStripMenuItem.Visible = true;
                }
                else if (menu == "PurchaseReceipt")
                {
                    purchaseReceiptToolStripMenuItem.Visible = true;
                    receiptOfGoodsReportToolStripMenuItem.Visible = true;
                }
                else if (menu == "ReceiptOfGoodsReport")
                {
                    receiptOfGoodsReportToolStripMenuItem.Visible = true;
                }
                else if (menu == "PurchaseReturn")
                {
                    purchaseReturnToolStripMenuItem.Visible = true;
                }

                else if (menu == "Purchase List")
                {
                    purchaseOrderListToolStripMenuItem.Visible = true;
                }

                //else if (menu == "PurchaseAgingReport")
                //{
                //    purchaseAgeingToolStripMenuItem.Visible = true;
                //}



                //Sales
                else if (menu == "Estimation")
                {
                    productToolStripMenuItem.Visible = true;
                }
                else if (menu == "SalesQuotation")
                {
                    salesQuotationToolStripMenuItem.Visible = true;
                }
                else if (menu == "SalesEstimation")
                {
                    salesBillToolStripMenuItem.Visible = true;
                }

                else if (menu == "windowsale")
                {
                    windowSaleToolStripMenuItem.Visible = true;
                }
                //else if (menu == "SalesReturn")
                //{
                //    salesReturnToolStripMenuItem.Visible = true;
                //}

                else if (menu == "SalesPage")
                {
                    salesToolStripMenuItem.Visible = true;
                }

                else if (menu == "Quotation List")
                {
                    quotationListToolStripMenuItem.Visible = true;
                }


                else if (menu == "Estimation List")
                {
                    estimationListToolStripMenuItem.Visible = true;
                }

                //else if (menu == "Sales Return List")
                //{
                //    salesReturnListToolStripMenuItem.Visible = true;
                //}

                else if (menu == "Sales List")
                {
                    saleListToolStripMenuItem.Visible = true;
                }

                //else if (menu == "EstimationAging")
                //{
                //    estimationAgeingToolStripMenuItem.Visible = true;
                //}

                else if (menu == "ExpressSales")
                {
                    expressSalesToolStripMenuItem.Visible = true;
                }
                else if (menu == "SalesQuotationPDI")
                {
                    salesPDiToolStripMenuItem.Visible = true;
                }



                //Master
                else if (menu == "Setting")
                {
                    mastersToolStripMenuItem.Visible = true;
                }
                else if (menu == "ProductMaster")
                {
                    productsToolStripMenuItem.Visible = true;
                }
                else if (menu == "SuppliersMaster")
                {
                    suppliersToolStripMenuItem.Visible = true;
                }
                else if (menu == "CustomerMaster")
                {
                    customersToolStripMenuItem.Visible = true;
                }
                else if (menu == "ReferenceMaster")
                {
                    referenceToolStripMenuItem.Visible = true;
                }
                else if (menu == "UOMMaster")
                {
                    uOMToolStripMenuItem.Visible = true;
                }
                else if (menu == "CategoryMaster")
                {
                    categoryToolStripMenuItem.Visible = true;
                }
                else if (menu == "LocationMaster")
                {
                    locationToolStripMenuItem.Visible = true;
                }
                else if (menu == "UserCreation")
                {
                    userCreationToolStripMenuItem.Visible = true;
                }
                else if (menu == "RoleMaster")
                {
                    rolesToolStripMenuItem.Visible = true;
                }
                else if (menu == "EmployeeMaster")
                {
                    employeeToolStripMenuItem.Visible = true;
                }
                else if (menu == "BrandMaster")
                {
                    brandToolStripMenuItem.Visible = true;
                }



                //Commission


                //else if (menu == "Commission")
                //{
                //    commissionToolStripMenuItem.Visible = true;
                //}
                //else if (menu == "CommissionBill")
                //{
                //    commissionBillToolStripMenuItem.Visible = true;
                //}
                //else if (menu == "CommissionPayment")
                //{
                //    commisionPaymentToolStripMenuItem.Visible = true;
                //}
                //else if (menu == "CommissionReport")
                //{
                //    commissionReportToolStripMenuItem.Visible = true;
                //}





                //Transaction

                else if (menu == "Transaction")
                {
                    transactionToolStripMenuItem.Visible = true;
                }
                else if (menu == "CashRequest")
                {
                    cashRequestToolStripMenuItem.Visible = true;
                }
                //else if (menu == "CashApproval")
                //{
                //    cashApprovalToolStripMenuItem.Visible = true;
                //}
                //else if (menu == "CashPayment")
                //{
                //    cashPaymentToolStripMenuItem.Visible = true;
                //}
                else if (menu == "Cash Receipt")
                {
                    cashReceiptToolStripMenuItem.Visible = true;
                }
                //else if (menu == "Cash Request List")
                //{
                //    cashRequestListToolStripMenuItem.Visible = true;
                //}
                //else if (menu == "Cash Receipt List")
                //{
                //    cashReceiptToolStripMenuItem1.Visible = true;
                //}




                //Report
                else if (menu == "Report")
                {
                    reportsToolStripMenuItem.Visible = true;
                    followupQuotationReportToolStripMenuItem.Visible = true;
                }
                else if (menu == "SalesReport")
                {
                    slesReportToolStripMenuItem.Visible = true;
                }
                //else if (menu == "StockDetailReport")
                //{
                //    stockDetailReportToolStripMenuItem.Visible = true;
                //}
                else if (menu == "StockReport")
                {
                    stockReportToolStripMenuItem.Visible = true;
                }

                else if (menu == "CashTransactionReport")
                {
                    cashTransactionReportToolStripMenuItem.Visible = true;
                }

                else if (menu == "StockAdjustment")
                {
                    stockAjustmentToolStripMenuItem.Visible = true;
                }
                else if (menu == "ViewStockAdjustment")
                {
                    viewStockAdjustmentToolStripMenuItem.Visible = false;
                }
                else if (menu == "MaterialTransactionReport")
                {
                    materialTransactionReportToolStripMenuItem.Visible = true;
                }

                else if (menu == "UserBill")
                {
                    userBillReportToolStripMenuItem.Visible = false;
                }
                else if (menu == "IncentiveReport")
                {
                    incentiveReportToolStripMenuItem.Visible = true;
                }





                //else if (menu == "Issued/Received Report")
                //{
                //    issuedReceivedReportToolStripMenuItem1.Visible = true;
                //}



                //Movement

                else if (menu == "Movement")
                {
                    movementToolStripMenuItem.Visible = true;
                }
                else if (menu == "MaterialMovement")
                {
                    materialMovementToolStripMenuItem.Visible = true;
                }
                else if (menu == "warranty")
                {
                    issuedReceivedToolStripMenuItem.Visible = true;
                }
                else if (menu == "warrantyList")
                {
                    materialMovementToolStripMenuItem.Visible = true;
                }


                //else if (menu == "Issued/Received Report")
                //{
                //    issuedReceivedReportToolStripMenuItem1.Visible = true;
                //}





                //Tool

                else if (menu == "Tools")
                {
                    toolsToolStripMenuItem.Visible = true;
                }
                else if (menu == "CashEndClose")
                {
                    cashEndCloseToolStripMenuItem.Visible = true;
                }
                else if (menu == "CashEndVerification")
                {
                    cashEndVerificationToolStripMenuItem.Visible = true;
                }




                //Account

                else if (menu == "Account")
                {
                    accountsToolStripMenuItem.Visible = true;
                }
                else if (menu == "AccountReceiveBalance")
                {
                    accountReceiveBalanceToolStripMenuItem.Visible = true;
                }
                //else if (menu == "AccountPayableBalance")
                //{
                //    accountPayableBalanceToolStripMenuItem.Visible = true;
                //}

                //Adjustment

                else if (menu == "Adjustment")
                {
                    adjustmentToolStripMenuItem.Visible = false;
                }
                else if (menu == "AdjustmentForPurchase")
                {
                    adjustmentForPurchaseToolStripMenuItem.Visible = false;
                }
                else if (menu == "AdjustmentForSales")
                {
                    adjustmentForSalesToolStripMenuItem.Visible = false;
                }



                //Service

                else if (menu == "Service")
                {
                    servicesToolStripMenuItem.Visible = true;
                }
                else if (menu == "Service Request")
                {
                    serviceRequestToolStripMenuItem.Visible = true;
                }
                else if (menu == "Service Update Status")
                {
                    updateStatusToolStripMenuItem.Visible = true;
                }

                else if (menu == "Service Completed Tray")
                {
                    completedToolStripMenuItem.Visible = true;
                }

                else if (menu == "Service Issued Tray")
                {
                    issuedTrayToolStripMenuItem.Visible = true;
                }

                else if (menu == "Service Cancelled Tray")
                {
                    cancelledTrayToolStripMenuItem.Visible = true;
                }



                //Issued/Received

                //else if (menu == "Issued/Received")
                //{
                //    issuedReceivedToolStripMenuItem1.Visible = true;
                //}
                //else if (menu == "Issued")
                //{
                //    receivedToolStripMenuItem.Visible = true;
                //}
                //else if (menu == "Received")
                //{
                //    issuedToolStripMenuItem.Visible = true;
                //}



            }

        }

        private void cashTransactionReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //  CashTransactionReportRV ObjCashTransactionReportRV;

        }

        private void employeeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objEmployee == null)
            {
                Employee objEmployee = new Employee();
                Program.objEmployee = objEmployee;
            }
            else
            {
                Program.objEmployee.Dispose();
                Employee objEmployee = new Employee();
                Program.objEmployee = objEmployee;
            }
            Program.objEmployee.MdiParent = this;
            Program.objEmployee.Show();
        }

        private void issuedReceivedReportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //if (Program.objFrmIssuedReceivedReport == null)
            //{


            //    FrmIssuedReceivedReport FrmIssuedReceivedReport = new FrmIssuedReceivedReport();
            //    Program.objFrmIssuedReceivedReport = FrmIssuedReceivedReport;
            //}
            //else
            //{
            //    Program.objFrmIssuedReceivedReport.Dispose();
            //    FrmIssuedReceivedReport FrmIssuedReceivedReport = new FrmIssuedReceivedReport();
            //    Program.objFrmIssuedReceivedReport = FrmIssuedReceivedReport;
            //}
            //Program.objFrmIssuedReceivedReport.MdiParent = this;
            //Program.objFrmIssuedReceivedReport.Show();
        }

        private void adjustmentForSalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AdjustmentforSalesBill ObjAdjustmentforSalesBill
            if (Program.ObjAdjustmentforSalesBill == null)
            {
                AdjustmentforSalesBill objAdjustmentforSalesBill = new AdjustmentforSalesBill();
                Program.ObjAdjustmentforSalesBill = objAdjustmentforSalesBill;
            }
            else
            {
                Program.ObjAdjustmentforSalesBill.Dispose();
                AdjustmentforSalesBill objAdjustmentforSalesBill = new AdjustmentforSalesBill();
                Program.ObjAdjustmentforSalesBill = objAdjustmentforSalesBill;
            }
            Program.ObjAdjustmentforSalesBill.MdiParent = this;
            Program.ObjAdjustmentforSalesBill.Show();
        }

        private void adjustmentForPurchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AdjustmentforPurchaseBill ObjAdjustmentforPurchaseBill;
            if (Program.ObjAdjustmentforPurchaseBill == null)
            {
                AdjustmentforPurchaseBill objAdjustmentforPurchaseBill = new AdjustmentforPurchaseBill();
                Program.ObjAdjustmentforPurchaseBill = objAdjustmentforPurchaseBill;
            }
            else
            {
                Program.ObjAdjustmentforPurchaseBill.Dispose();
                AdjustmentforPurchaseBill objAdjustmentforPurchaseBill = new AdjustmentforPurchaseBill();
                Program.ObjAdjustmentforPurchaseBill = objAdjustmentforPurchaseBill;
            }
            Program.ObjAdjustmentforPurchaseBill.MdiParent = this;
            Program.ObjAdjustmentforPurchaseBill.Show();
        }

        private void inventoryToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.Black;
            productToolStripMenuItem.ForeColor = Color.White;
            transactionToolStripMenuItem.ForeColor = Color.White;
            // commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;

        }

        private void inventoryToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            leave();
        }

        private void purchaseOrderToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.Black;
        }

        private void purchaseReceiptToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.Black;
        }

        private void purchaseReturnToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.Black;
        }

        private void purchaseReceiptToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
        }

        private void purchaseReturnToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
        }

        private void productToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.Black;
            transactionToolStripMenuItem.ForeColor = Color.White;
            //commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void transactionToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.White;
            transactionToolStripMenuItem.ForeColor = Color.Black;
            //commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void commissionToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.White;
            transactionToolStripMenuItem.ForeColor = Color.White;
            //commissionToolStripMenuItem.ForeColor = Color.Black;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void adjustmentToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.White;
            transactionToolStripMenuItem.ForeColor = Color.White;
            // commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.Black;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void accountsToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.White;
            transactionToolStripMenuItem.ForeColor = Color.White;
            // commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.Black;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void movementToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.White;
            transactionToolStripMenuItem.ForeColor = Color.White;
            // commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.Black;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void toolsToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.White;
            transactionToolStripMenuItem.ForeColor = Color.White;
            // commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.Black;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void mastersToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.White;
            transactionToolStripMenuItem.ForeColor = Color.White;
            //commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.Black;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void reportsToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.White;
            transactionToolStripMenuItem.ForeColor = Color.White;
            //commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.Black;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        public void leave()
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.White;
            transactionToolStripMenuItem.ForeColor = Color.White;
            //commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void productToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            leave();
        }

        private void transactionToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            leave();
        }

        private void commissionToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            leave();
        }

        private void adjustmentToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            leave();
        }

        private void accountsToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            leave();
        }

        private void movementToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            leave();
        }

        private void toolsToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            leave();
        }

        private void mastersToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            leave();
        }

        private void reportsToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            leave();
        }

        private void salesQuotationToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.Black;
        }

        private void salesBillToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.Black;
        }

        private void windowSaleToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.Black;
        }

        private void salesReturnToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.Black;
        }

        private void salesToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.Black;
        }

        private void salesQuotationToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.White;
        }

        private void salesBillToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.White;
        }

        private void windowSaleToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.White;
        }

        private void salesReturnToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.White;
        }

        private void salesToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.White;
        }

        private void cashRequestToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            transactionToolStripMenuItem.ForeColor = Color.Black;
        }

        private void cashApprovalToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            transactionToolStripMenuItem.ForeColor = Color.Black;
        }

        private void cashPaymentToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            transactionToolStripMenuItem.ForeColor = Color.Black;
        }

        private void cashRequestToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            transactionToolStripMenuItem.ForeColor = Color.White;
        }

        private void cashApprovalToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            transactionToolStripMenuItem.ForeColor = Color.White;
        }

        private void cashPaymentToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            transactionToolStripMenuItem.ForeColor = Color.White;
        }

        //private void commissionBillToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        //{
        //    commissionToolStripMenuItem.ForeColor = Color.Black;
        //}

        //private void commisionPaymentToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        //{
        //    commissionToolStripMenuItem.ForeColor = Color.Black;
        //}

        //private void commissionReportToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        //{
        //    commissionToolStripMenuItem.ForeColor = Color.Black;
        //}

        //private void commissionBillToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        //{
        //    commissionToolStripMenuItem.ForeColor = Color.White;
        //}

        //private void commisionPaymentToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        //{
        //    commissionToolStripMenuItem.ForeColor = Color.White;
        //}

        //private void commissionReportToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        //{
        //    commissionToolStripMenuItem.ForeColor = Color.White;
        //}

        private void adjustmentForSalesToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            adjustmentToolStripMenuItem.ForeColor = Color.Black;
        }

        private void adjustmentForPurchaseToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            adjustmentToolStripMenuItem.ForeColor = Color.Black;
        }

        private void adjustmentForSalesToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            adjustmentToolStripMenuItem.ForeColor = Color.White;
        }

        private void adjustmentForPurchaseToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            adjustmentToolStripMenuItem.ForeColor = Color.White;
        }

        private void accountReceiveBalanceToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            accountsToolStripMenuItem.ForeColor = Color.Black;
        }

        private void accountPayableBalanceToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            accountsToolStripMenuItem.ForeColor = Color.Black;
        }

        private void accountReceiveBalanceToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            accountsToolStripMenuItem.ForeColor = Color.White;
        }

        private void accountPayableBalanceToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            accountsToolStripMenuItem.ForeColor = Color.White;
        }

        private void materialMovementToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            movementToolStripMenuItem.ForeColor = Color.Black;
        }

        private void issuedReceivedToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            movementToolStripMenuItem.ForeColor = Color.Black;
        }

        private void issuedReceivedReportToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            movementToolStripMenuItem.ForeColor = Color.Black;
        }

        private void materialMovementToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            movementToolStripMenuItem.ForeColor = Color.White;
        }

        private void issuedReceivedToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            movementToolStripMenuItem.ForeColor = Color.White;
        }

        private void issuedReceivedReportToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            movementToolStripMenuItem.ForeColor = Color.White;
        }

        private void cashEndCloseToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            toolsToolStripMenuItem.ForeColor = Color.Black;
        }

        private void cashEndVerificationToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            toolsToolStripMenuItem.ForeColor = Color.Black;
        }

        private void cashEndCloseToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            toolsToolStripMenuItem.ForeColor = Color.White;
        }

        private void cashEndVerificationToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            toolsToolStripMenuItem.ForeColor = Color.White;
        }

        private void productsToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.Black;
        }

        private void suppliersToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.Black;
        }

        private void customersToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.Black;
        }

        private void referenceToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.Black;
        }

        private void uOMToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.Black;
        }

        private void categoryToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.Black;
        }

        private void locationToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.Black;
        }

        private void userCreationToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.Black;
        }

        private void rolesToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.Black;
        }

        private void employeeToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.Black;
        }

        private void productsToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.White;
        }

        private void suppliersToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.White;
        }

        private void customersToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.White;
        }

        private void referenceToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.White;
        }

        private void uOMToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.White;
        }

        private void categoryToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.White;
        }

        private void locationToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.White;
        }

        private void userCreationToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.White;
        }

        private void rolesToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.White;
        }

        private void employeeToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            mastersToolStripMenuItem.ForeColor = Color.White;
        }

        private void slesReportToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            reportsToolStripMenuItem.ForeColor = Color.Black;
        }

        private void stockDetailReportToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            reportsToolStripMenuItem.ForeColor = Color.Black;
        }

        private void cashTransactionReportToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            reportsToolStripMenuItem.ForeColor = Color.Black;
        }

        private void issuedReceivedReportToolStripMenuItem1_MouseEnter(object sender, EventArgs e)
        {
            reportsToolStripMenuItem.ForeColor = Color.Black;
        }

        private void slesReportToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            reportsToolStripMenuItem.ForeColor = Color.White;
        }

        private void stockDetailReportToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            reportsToolStripMenuItem.ForeColor = Color.White;
        }

        private void cashTransactionReportToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            reportsToolStripMenuItem.ForeColor = Color.White;
        }

        private void issuedReceivedReportToolStripMenuItem1_MouseLeave(object sender, EventArgs e)
        {
            reportsToolStripMenuItem.ForeColor = Color.White;
        }

        private void purchaseOrderToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void newSalesQuotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSalesQuotation1 == null)
            {
                SalesQuotation1 SalesQuotation1 = new SalesQuotation1();
                Program.objSalesQuotation1 = SalesQuotation1;
            }
            else
            {
                Program.objSalesQuotation1.Dispose();
                SalesQuotation1 SalesQuotation1 = new SalesQuotation1();
                Program.objSalesQuotation1 = SalesQuotation1;
            }
            Program.objSalesQuotation1.MdiParent = this;
            Program.objSalesQuotation1.Show();
        }

        private void newSaleEstimationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void purchaseOrderListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSearchPurchaseOrder == null)
            {
                SearchPurchaseOrder objSearchPurchaseOrder = new SearchPurchaseOrder();
                Program.objSearchPurchaseOrder = objSearchPurchaseOrder;
            }
            else
            {
                Program.objSearchPurchaseOrder.Dispose();
                SearchPurchaseOrder objSearchPurchaseOrder = new SearchPurchaseOrder();
                Program.objSearchPurchaseOrder = objSearchPurchaseOrder;
            }
            Program.objSearchPurchaseOrder.MdiParent = this;
            Program.objSearchPurchaseOrder.Show();
        }

        private void quotationListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSearchQuatation == null)
            {
                SearchQuatation objSearchQuatation = new SearchQuatation();
                Program.objSearchQuatation = objSearchQuatation;
            }
            else
            {
                Program.objSearchQuatation.Dispose();
                SearchQuatation objSearchQuatation = new SearchQuatation();
                Program.objSearchQuatation = objSearchQuatation;
            }
            Program.objSearchQuatation.MdiParent = this;
            Program.objSearchQuatation.Show();
        }

        private void followupQuotationReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objFollowupQuotationReport == null)
            {
                FollowupQuotationReport objFollowupQuotationReport = new FollowupQuotationReport();
                Program.objFollowupQuotationReport = objFollowupQuotationReport;
            }
            else
            {
                Program.objFollowupQuotationReport.Dispose();
                FollowupQuotationReport objFollowupQuotationReport = new FollowupQuotationReport();
                Program.objFollowupQuotationReport = objFollowupQuotationReport;
            }
            Program.objFollowupQuotationReport.MdiParent = this;
            Program.objFollowupQuotationReport.Show();
        }

        private void estimationListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSearchQuatationEstimation == null)
            {
                SearchQuatationEstimation objSearchQuatationEstimation = new SearchQuatationEstimation();
                Program.objSearchQuatationEstimation = objSearchQuatationEstimation;
            }
            else
            {
                Program.objSearchQuatationEstimation.Dispose();
                SearchQuatationEstimation objSearchQuatationEstimation = new SearchQuatationEstimation();
                Program.objSearchQuatationEstimation = objSearchQuatationEstimation;
            }
            Program.objSearchQuatationEstimation.MdiParent = this;
            Program.objSearchQuatationEstimation.Show();
        }

        private void purchaseOrderListToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.Black;
        }

        private void purchaseOrderListToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
        }

        private void quotationListToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.Black;
            transactionToolStripMenuItem.ForeColor = Color.White;
            //commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void quotationListToolStripMenuItem_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void quotationListToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            //inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.White;
        }

        private void estimationListToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.Black;
            transactionToolStripMenuItem.ForeColor = Color.White;
            //commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void estimationListToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.White;
        }

        private void serviceRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Service = "Service Request";
            if (Program.objServiceRequest == null)
            {
                ServiceRequest FrmServiceRequest = new ServiceRequest();
                Program.objServiceRequest = FrmServiceRequest;
            }
            else
            {
                Program.objServiceRequest.Dispose();
                ServiceRequest FrmServiceRequest = new ServiceRequest();
                Program.objServiceRequest = FrmServiceRequest;
            }
            Program.objServiceRequest.MdiParent = this;
            Program.objServiceRequest.Show();
        }

        private void updateStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Service = "Service Update";
            if (Program.objServiceRequest == null)
            {
                ServiceRequest FrmServiceRequest = new ServiceRequest();
                Program.objServiceRequest = FrmServiceRequest;
            }
            else
            {
                Program.objServiceRequest.Dispose();
                ServiceRequest FrmServiceRequest = new ServiceRequest();
                Program.objServiceRequest = FrmServiceRequest;
            }
            Program.objServiceRequest.MdiParent = this;
            Program.objServiceRequest.Show();
        }

        private void completedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Service = "Service Completed";
            if (Program.objServiceRequest == null)
            {
                ServiceRequest FrmServiceRequest = new ServiceRequest();
                Program.objServiceRequest = FrmServiceRequest;
            }
            else
            {
                Program.objServiceRequest.Dispose();
                ServiceRequest FrmServiceRequest = new ServiceRequest();
                Program.objServiceRequest = FrmServiceRequest;
            }
            Program.objServiceRequest.MdiParent = this;
            Program.objServiceRequest.Show();
        }

        private void issuedTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Service = "Service Issued";
            if (Program.objIssuedTray == null)
            {
                IssuedTray FrmServiceRequest = new IssuedTray();
                Program.objIssuedTray = FrmServiceRequest;
            }
            else
            {
                Program.objIssuedTray.Dispose();
                IssuedTray FrmServiceRequest = new IssuedTray();
                Program.objIssuedTray = FrmServiceRequest;
            }
            Program.objIssuedTray.MdiParent = this;
            Program.objIssuedTray.Show();
        }

        private void cancelledTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Program.Service = "Service Cancelled";
            if (Program.objCancelledTray == null)
            {
                CancelledTray FrmServiceRequest = new CancelledTray();
                Program.objCancelledTray = FrmServiceRequest;
            }
            else
            {
                Program.objCancelledTray.Dispose();
                CancelledTray FrmServiceRequest = new CancelledTray();
                Program.objCancelledTray = FrmServiceRequest;
            }
            Program.objCancelledTray.MdiParent = this;
            Program.objCancelledTray.Show();
        }

        private void serviceRequestToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            servicesToolStripMenuItem.ForeColor = Color.Black;
        }

        private void serviceRequestToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void servicesToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.White;
            transactionToolStripMenuItem.ForeColor = Color.White;
            //commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.Black;
        }

        private void servicesToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            leave();
        }

        private void salesReturnListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Program.Service = "Service Update";
            if (Program.objSalesReturnList == null)
            {
                SalesReturnList objSalesReturnList = new SalesReturnList();
                Program.objSalesReturnList = objSalesReturnList;
            }
            else
            {
                Program.objSalesReturnList.Dispose();
                SalesReturnList objSalesReturnList = new SalesReturnList();
                Program.objSalesReturnList = objSalesReturnList;
            }
            Program.objSalesReturnList.MdiParent = this;
            Program.objSalesReturnList.Show();
        }

        private void salesReturnListToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.Black;
            transactionToolStripMenuItem.ForeColor = Color.White;
            //commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void salesReturnListToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.White;
        }

        private void cashReceiptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Program.Service = "Service Update";
            if (Program.objCashReceipt == null)
            {
                CashReceipt CashReceipt = new CashReceipt();
                Program.objCashReceipt = CashReceipt;
            }
            else
            {
                Program.objCashReceipt.Dispose();
                CashReceipt CashReceipt = new CashReceipt();
                Program.objCashReceipt = CashReceipt;
            }
            Program.objCashReceipt.MdiParent = this;
            Program.objCashReceipt.Show();

        }

        private void cashRequestListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objRequestlist == null)
            {
                Requestlist objRequestlist = new Requestlist();
                Program.objRequestlist = objRequestlist;
            }
            else
            {
                Program.objRequestlist.Dispose();
                Requestlist objRequestlist = new Requestlist();
                Program.objRequestlist = objRequestlist;
            }
            Program.objRequestlist.MdiParent = this;
            Program.objRequestlist.Show();
        }

        private void cashReceiptToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Program.objCashreceiptlist == null)
            {
                Cashreceiptlist objCashreceiptlist = new Cashreceiptlist();
                Program.objCashreceiptlist = objCashreceiptlist;
            }
            else
            {
                Program.objCashreceiptlist.Dispose();
                Cashreceiptlist objCashreceiptlist = new Cashreceiptlist();
                Program.objCashreceiptlist = objCashreceiptlist;
            }
            Program.objCashreceiptlist.MdiParent = this;
            Program.objCashreceiptlist.Show();
        }

        private void stockAjustmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objStockAdjustment == null)
            {
                StockAdjustment objStockAdjustment = new StockAdjustment();
                Program.objStockAdjustment = objStockAdjustment;
            }
            else
            {
                Program.objStockAdjustment.Dispose();
                StockAdjustment objStockAdjustment = new StockAdjustment();
                Program.objStockAdjustment = objStockAdjustment;
            }
            Program.objStockAdjustment.MdiParent = this;
            Program.objStockAdjustment.Show();
        }

        private void saleListToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (Program.objSalesReportForm == null)
            {
                SalesReportForm objSalesReportForm = new SalesReportForm();
                Program.objSalesReportForm = objSalesReportForm;
            }
            else
            {
                Program.objSalesReportForm.Dispose();
                SalesReportForm objSalesReportForm = new SalesReportForm();
                Program.objSalesReportForm = objSalesReportForm;
            }
            Program.objSalesReportForm.MdiParent = this;
            Program.objSalesReportForm.Show();
        }

        private void brandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objBrand == null)
            {
                Brand objBrand = new Brand();
                Program.objBrand = objBrand;
            }
            else
            {
                Program.objBrand.Dispose();
                Brand objBrand = new Brand();
                Program.objBrand = objBrand;
            }
            Program.objBrand.MdiParent = this;
            Program.objBrand.Show();
        }

        private void viewStockAdjustmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objStockAdjustmentView == null)
            {
                StockAdjustmentView objStockAdjustmentView = new StockAdjustmentView();
                Program.objStockAdjustmentView = objStockAdjustmentView;
            }
            else
            {
                Program.objStockAdjustmentView.Dispose();
                StockAdjustmentView objStockAdjustmentView = new StockAdjustmentView();
                Program.objStockAdjustmentView = objStockAdjustmentView;
            }
            Program.objStockAdjustmentView.MdiParent = this;
            Program.objStockAdjustmentView.Show();
        }


        public void WriteDataToFile(DataTable submittedDataTable, string submittedFilePath)
        {
            int i = 0;
            StreamWriter sw = null;
            sw = File.CreateText(submittedFilePath);
            sw.Flush();
            sw.Close();
            sw = new StreamWriter(submittedFilePath, false);

            for (i = 0; i < submittedDataTable.Columns.Count - 1; i++)
            {
                //sw.Write("{0,-20}", GetFormatedText(submittedDataTable.Columns[i].ColumnName, 20) + "|" );
                sw.Write(submittedDataTable.Columns[i].ColumnName + "|");
            }

            sw.Write(submittedDataTable.Columns[i].ColumnName);
            sw.WriteLine(" ");


            //if (submittedDataTable.Columns.Count == 10)
            //{
            //    int i1;
            //    string Lstr = "";
            //    for (i1 = 1; i1 <= 205; i1++)
            //    {
            //        Lstr = Lstr + "-";
            //    }
            //    sw.WriteLine(Lstr);
            //}
            //else
            //{
            //    int i1;
            //    string Lstr = "";
            //    for (i1 = 1; i1 <= 600; i1++)
            //    {
            //        Lstr = Lstr + "-";
            //    }
            //    sw.WriteLine(Lstr);
            //}


            foreach (DataRow row in submittedDataTable.Rows)
            {
                object[] array = row.ItemArray;

                for (i = 0; i < array.Length - 1; i++)
                {

                    //sw.Write("{0,-20}", GetFormatedText(array[i].ToString(), 20) + "|");
                    sw.Write(array[i].ToString() + "|");
                }
                sw.Write(array[i].ToString());
                sw.WriteLine();

            }
            sw.Flush();
            sw.Close();
        }


        private void Btndelete_Click(object sender, EventArgs e)
        {

            pnlless.Visible = true;
            txtlesspwd.Focus();


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

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtlesspwd.Text))
            {
                MessageBox.Show("Password should not be empty");
                this.ActiveControl = txtlesspwd;
                return;
            }
            DataTable dt = LoginBAL.GetLesserDetials(txtlesspwd.Text, "ESTIMATIONDELETE");
            if (dt.Rows.Count > 0)
            {
                txtlesspwd.Text = string.Empty;
                pnlless.Visible = false;
                panel1.Visible = true;

            }
            else
            {
                MessageBox.Show("Authentication Failed");
                txtlesspwd.Focus();

            }
        }



        public void deletedate()
        {
            DataSet dt = new DataSet();
            string path = string.Empty;
            int vsa = 0;
            DateTime fromdate = new DateTime(Fromdate.Value.Year, Fromdate.Value.Month, Fromdate.Value.Day);
            DateTime Todatedate = new DateTime(Todate.Value.Year, Todate.Value.Month, Todate.Value.Day);
            try
            {

                if (rdall.Checked)
                {
                    dt = objQuotationbal.getestimatedelete(fromdate, Todatedate, "All");
                }
                else if (rdrange.Checked)
                {

                    dt = objQuotationbal.getestimatedelete(fromdate, Todatedate, "Range");
                }


                //DataTable dtdetails = objQuotationbal.getestimatedeletedetial();
                path = objQuotationbal.Estimationpath();
                string date = Regex.Replace(DateTime.Now.ToString(), @"[^0-9a-zA-Z]+", "-");
                path = path + date;
                WriteDataToFile(dt.Tables[1], path + "Estmationdetails.txt");
                WriteDataToFile(dt.Tables[0], path + "Estmation.txt");

                if (rdall.Checked)
                {
                    vsa = objQuotationbal.DeleteAllEstmation(fromdate, Todatedate, "All");
                }
                else if (rdrange.Checked)
                {

                    vsa = objQuotationbal.DeleteAllEstmation(fromdate, Todatedate, "Range");
                }



                if (vsa == 1)
                {
                    MessageBox.Show("Deleted Successfully");
                }
                else
                {
                    MessageBox.Show("Failed");
                }
            }
            catch (Exception e1)
            {
                DialogResult result = MessageBox.Show("File Can't Updated.  Do you want to  Continue to Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (rdall.Checked)
                    {
                        vsa = objQuotationbal.DeleteAllEstmation(fromdate, Todatedate, "All");
                    }
                    else if (rdrange.Checked)
                    {

                        vsa = objQuotationbal.DeleteAllEstmation(fromdate, Todatedate, "Range");
                    }
                    if (vsa == 1)
                    {
                        MessageBox.Show("Deleted Successfully");
                    }
                    else
                    {
                        MessageBox.Show("Failed");
                    }
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            pnlless.Visible = false;
            txtlesspwd.Clear();
        }

        private void txtlesspwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void rdrange_CheckedChanged(object sender, EventArgs e)
        {
            lblfrom.Visible = true;
            lblto.Visible = true;
            Fromdate.Visible = true;
            Todate.Visible = true;
            button2.Enabled = true;
        }

        private void rdall_CheckedChanged(object sender, EventArgs e)
        {
            lblfrom.Visible = false;
            lblto.Visible = false;
            Fromdate.Visible = false;
            Todate.Visible = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            deletedate();
            panel1.Visible = false;
            rdall.Checked = false;
            rdrange.Checked = false;
            lblfrom.Visible = false;
            lblto.Visible = false;
            Fromdate.Visible = false;
            Todate.Visible = false;
            button2.Enabled = false;

        }

        private void button3_Click(object sender, EventArgs e)
        {

            panel1.Visible = false;
            rdall.Checked = false;
            rdrange.Checked = false;
            lblfrom.Visible = false;
            lblto.Visible = false;
            Fromdate.Visible = false;
            Todate.Visible = false;
            button2.Enabled = false;
        }

        private void materialTransactionReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objMaterialTranscationreport == null)
            {
                MaterialTranscationreport objMaterialTranscationreport = new MaterialTranscationreport();
                Program.objMaterialTranscationreport = objMaterialTranscationreport;
            }
            else
            {
                Program.objMaterialTranscationreport.Dispose();
                MaterialTranscationreport objMaterialTranscationreport = new MaterialTranscationreport();
                Program.objMaterialTranscationreport = objMaterialTranscationreport;
            }
            Program.objMaterialTranscationreport.MdiParent = this;
            Program.objMaterialTranscationreport.Show();
        }

        private void materialMovementToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Program.objMaterialMovement == null)
            {


                MaterialMovement MaterialMovement = new MaterialMovement();
                Program.objMaterialMovement = MaterialMovement;
            }
            else
            {
                Program.objMaterialMovement.Close();
                MaterialMovement MaterialMovement = new MaterialMovement();
                Program.objMaterialMovement = MaterialMovement;
            }
            Program.objMaterialMovement.MdiParent = this;
            Program.objMaterialMovement.Show();
        }

        private void issuedToolStripMenuItem_Click(object sender, EventArgs e)
        {



            if (Program.objReceived == null)
            {
                Received objReceived = new Received();
                Program.objReceived = objReceived;
            }
            else
            {
                Program.objReceived.Close();
                Received objReceived = new Received();
                Program.objReceived = objReceived;
            }
            Program.objReceived.MdiParent = this;
            Program.objReceived.Show();



        }

        private void receivedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objIssued == null)
            {
                Issued objIssued = new Issued();
                Program.objIssued = objIssued;
            }
            else
            {
                Program.objIssued.Close();
                Issued objIssued = new Issued();
                Program.objIssued = objIssued;
            }
            Program.objIssued.MdiParent = this;
            Program.objIssued.Show();
        }

        private void materialMovementToolStripMenuItem1_MouseEnter(object sender, EventArgs e)
        {
            movementToolStripMenuItem.ForeColor = Color.Black;
        }

        private void materialMovementToolStripMenuItem1_MouseLeave(object sender, EventArgs e)
        {
            movementToolStripMenuItem.ForeColor = Color.White;
        }

        private void issuedToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            // issuedReceivedToolStripMenuItem1.ForeColor = Color.Black;
        }

        private void issuedToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            // issuedReceivedToolStripMenuItem1.ForeColor = Color.White;
        }

        private void estimationAgeingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objEstimationAging == null)
            {
                EstimationAging objEstimationAging = new EstimationAging();
                Program.objEstimationAging = objEstimationAging;
            }
            else
            {
                Program.objEstimationAging.Dispose();
                EstimationAging objEstimationAging = new EstimationAging();
                Program.objEstimationAging = objEstimationAging;
            }
            Program.objEstimationAging.MdiParent = this;
            Program.objEstimationAging.Show();
        }

        private void estimationAgeingToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
            productToolStripMenuItem.ForeColor = Color.Black;
            transactionToolStripMenuItem.ForeColor = Color.White;
            //commissionToolStripMenuItem.ForeColor = Color.White;
            adjustmentToolStripMenuItem.ForeColor = Color.White;
            accountsToolStripMenuItem.ForeColor = Color.White;
            movementToolStripMenuItem.ForeColor = Color.White;
            toolsToolStripMenuItem.ForeColor = Color.White;
            mastersToolStripMenuItem.ForeColor = Color.White;
            reportsToolStripMenuItem.ForeColor = Color.White;
            servicesToolStripMenuItem.ForeColor = Color.White;
        }

        private void estimationAgeingToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            productToolStripMenuItem.ForeColor = Color.White;
        }

        private void purchaseAgeingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objPurchase_Ageing == null)
            {
                Purchase_Ageing objPurchase_Ageing = new Purchase_Ageing();
                Program.objPurchase_Ageing = objPurchase_Ageing;
            }
            else
            {
                Program.objPurchase_Ageing.Dispose();
                Purchase_Ageing objPurchase_Ageing = new Purchase_Ageing();
                Program.objPurchase_Ageing = objPurchase_Ageing;
            }
            Program.objPurchase_Ageing.MdiParent = this;
            Program.objPurchase_Ageing.Show();
        }

        private void purchaseAgeingToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.Black;
        }

        private void purchaseAgeingToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            inventoryToolStripMenuItem.ForeColor = Color.White;
        }

        private void expressSalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objExpressSales == null)
            {
                ExpressSales objExpressSales = new ExpressSales();
                Program.objExpressSales = objExpressSales;
            }
            else
            {
                Program.objExpressSales.Close();
                ExpressSales objExpressSales = new ExpressSales();
                Program.objExpressSales = objExpressSales;
            }
            Program.objExpressSales.MdiParent = this;
            Program.objExpressSales.Show();
        }

        private void salesPDiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSalesPDI == null)
            {
                SalesPDI objSalesPDI = new SalesPDI();
                Program.objSalesPDI = objSalesPDI;
            }
            else
            {
                Program.objSalesPDI.Close();
                SalesPDI objSalesPDI = new SalesPDI();
                Program.objSalesPDI = objSalesPDI;
            }
            Program.objSalesPDI.MdiParent = this;
            Program.objSalesPDI.Show();

        }

        private void userBillReportToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (Program.objUserestimation == null)
            {
                Userestimation objUserestimation = new Userestimation();
                Program.objUserestimation = objUserestimation;
            }
            else
            {
                Program.objUserestimation.Close();
                Userestimation objUserestimation = new Userestimation();
                Program.objUserestimation = objUserestimation;
            }
            Program.objUserestimation.MdiParent = this;
            Program.objUserestimation.Show();
        }

        private void incentiveReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objIncentiveReport == null)
            {

                IncentiveReport objIncentiveReport = new IncentiveReport();
                Program.objIncentiveReport = objIncentiveReport;
            }
            else
            {
                Program.objIncentiveReport.Dispose();
                IncentiveReport objIncentiveReport = new IncentiveReport();
                Program.objIncentiveReport = objIncentiveReport;
            }
            Program.objIncentiveReport.MdiParent = this;
            Program.objIncentiveReport.Show();
        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void reportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void commissionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void salesTransactionReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objSALES_TRANSACTION_REPORT == null)
            {
                SALES_TRANSACTION_REPORT objSALES_TRANSACTION_REPORT = new SALES_TRANSACTION_REPORT();
                Program.objSALES_TRANSACTION_REPORT = objSALES_TRANSACTION_REPORT;
            }
            else
            {
                Program.objSALES_TRANSACTION_REPORT.Dispose();
                SALES_TRANSACTION_REPORT objSALES_TRANSACTION_REPORT = new SALES_TRANSACTION_REPORT();
                Program.objSALES_TRANSACTION_REPORT = objSALES_TRANSACTION_REPORT;
            }
            Program.objSALES_TRANSACTION_REPORT.MdiParent = this;
            Program.objSALES_TRANSACTION_REPORT.Show();
        }

        private void periodWiseClosingStockReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.frmPeriodWiseClosingStock != null)
            {
                Program.frmPeriodWiseClosingStock.Dispose();
            }
            PeriodWiseClosingStock objFrmPeriodWiseClosingStock = new PeriodWiseClosingStock();
            Program.frmPeriodWiseClosingStock = objFrmPeriodWiseClosingStock;
            Program.frmPeriodWiseClosingStock.MdiParent = this;
            Program.frmPeriodWiseClosingStock.Show();
        }

        private void accountHeadWiseStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.frmAccountHeadLedger != null)
            {
                Program.frmAccountHeadLedger.Dispose();
            }
            AccountHeadLedger objFrmAccountHeadLedger = new AccountHeadLedger();
            Program.frmAccountHeadLedger = objFrmAccountHeadLedger;
            Program.frmAccountHeadLedger.MdiParent = this;
            Program.frmAccountHeadLedger.Show();
        }

        private void quotaionListNotInEstimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.frmNotInEstimation != null)
            {
                Program.frmNotInEstimation.Dispose();
            }
            NotInEstimation objNotInEstimation = new NotInEstimation();
            Program.frmNotInEstimation = objNotInEstimation;
            Program.frmNotInEstimation.MdiParent = this;
            Program.frmNotInEstimation.Show();
        }

        private void roseReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objCashRequestlist == null)
            {
                CashRequestlist objCashRequestlist = new CashRequestlist();
                Program.objCashRequestlist = objCashRequestlist;
            }
            else
            {
                Program.objCashRequestlist.Dispose();
                CashRequestlist objCashRequestlist = new CashRequestlist();
                Program.objCashRequestlist = objCashRequestlist;
            }
            Program.objCashRequestlist.MdiParent = this;
            Program.objCashRequestlist.Show();
        }

        private void stockUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.UploadFile == null)
            {
                UploadFile ObjUploadFile = new UploadFile();
                Program.UploadFile = ObjUploadFile;
            }
            else
            {
                Program.UploadFile.Dispose();
                UploadFile ObjUploadFile = new UploadFile();
                Program.UploadFile = ObjUploadFile;
            }
            Program.UploadFile.MdiParent = this;
            Program.UploadFile.Show();
        }

        private void stockReUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.UploadFile == null)
            {
                ReUpload ObjReUpload = new ReUpload();
                Program.ReUpload = ObjReUpload;
            }
            else
            {
                Program.UploadFile.Dispose();
                ReUpload ObjUploadFile = new ReUpload();
                Program.ReUpload = ObjUploadFile;
            }
            Program.ReUpload.MdiParent = this;
            Program.ReUpload.Show();
        }

        private void cashTransactionReportToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (Program.Objcashtransaction == null)
            {
                Inventory.Report_Transaction.CashTransaction Objcashtransaction = new Inventory.Report_Transaction.CashTransaction();
                Program.Objcashtransaction = Objcashtransaction;
            }
            else
            {
                Program.Objcashtransaction.Dispose();
                Inventory.Report_Transaction.CashTransaction Objcashtransaction = new Inventory.Report_Transaction.CashTransaction();
                Program.Objcashtransaction = Objcashtransaction;
            }
            Program.Objcashtransaction.MdiParent = this;
            Program.Objcashtransaction.Show();
        }

        private void stockUploadHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.UploadFile == null)
            {
                UploadHistory ObjReUpload = new UploadHistory();
                Program.UploadHistory = ObjReUpload;
            }
            else
            {
                Program.UploadFile.Dispose();
                UploadHistory ObjUploadFile = new UploadHistory();
                Program.UploadHistory = ObjUploadFile;
            }
            Program.UploadHistory.MdiParent = this;
            Program.UploadHistory.Show();
        }

        private void pendingIssuedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objIssued == null)
            {
                Issued objIssued = new Issued();
                Program.objIssued = objIssued;
            }
            else
            {
                Program.objIssued.Dispose();
                Issued objIssued = new Issued();
                Program.objIssued = objIssued;
            }
            Program.objIssued.MdiParent = this;
            Program.objIssued.Show();
        }

        private void salesTransactionReportToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (Program.objSALES_TRANSACTION_REPORT == null)
            {
                SALES_TRANSACTION_REPORT objSALES_TRANSACTION_REPORT = new SALES_TRANSACTION_REPORT();
                Program.objSALES_TRANSACTION_REPORT = objSALES_TRANSACTION_REPORT;
            }
            else
            {
                Program.objSALES_TRANSACTION_REPORT.Dispose();
                SALES_TRANSACTION_REPORT objSALES_TRANSACTION_REPORT = new SALES_TRANSACTION_REPORT();
                Program.objSALES_TRANSACTION_REPORT = objSALES_TRANSACTION_REPORT;
            }
            Program.objSALES_TRANSACTION_REPORT.MdiParent = this;
            Program.objSALES_TRANSACTION_REPORT.Show();
        }

        private void salesNewQuotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.objIssued == null)
                {
                    SalesNewQuotation objsalesnewquotations = new SalesNewQuotation();
                    Program.objsalesnewquotation = objsalesnewquotations;
                }
                else
                {
                    Program.objsalesnewquotation.Dispose();
                    SalesNewQuotation objsalesnewquotations = new SalesNewQuotation();
                    Program.objsalesnewquotation = objsalesnewquotations;
                }
                Program.objsalesnewquotation.MdiParent = this;
                Program.objsalesnewquotation.Show();
            }

            catch (Exception ex)
            {

            }
        }

        private void priceUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!BranchAccess.IsMainOffice)
            {
                MessageBox.Show(BranchAccess.MainOfficeOnlyMessage);
                return;
            }

            if (Program.PriceUpload == null)
            {
                PriceUpload objpriceupload = new PriceUpload();
                Program.PriceUpload = objpriceupload;
            }
            else
            {
                Program.PriceUpload.Dispose();
                PriceUpload objpriceupload = new PriceUpload();
                Program.PriceUpload = objpriceupload;
            }
            Program.PriceUpload.MdiParent = this;
            Program.PriceUpload.Show();
        }

        private void productSyncQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!BranchAccess.IsMainOffice)
            {
                MessageBox.Show(BranchAccess.MainOfficeOnlyMessage);
                return;
            }

            if (Program.ProductSyncQueue == null || Program.ProductSyncQueue.IsDisposed)
            {
                Program.ProductSyncQueue = new ProductSyncQueue();
            }
            else
            {
                Program.ProductSyncQueue.Dispose();
                Program.ProductSyncQueue = new ProductSyncQueue();
            }

            Program.ProductSyncQueue.MdiParent = this;
            Program.ProductSyncQueue.Show();
        }

        private void accountCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.CustAccount == null)
            {
                CustomerAccount objpriceupload = new CustomerAccount();
                Program.CustAccount = objpriceupload;
            }
            else
            {
                Program.CustAccount.Dispose();
                CustomerAccount objpriceupload = new CustomerAccount();
                Program.CustAccount = objpriceupload;
            }
            Program.CustAccount.MdiParent = this;
            Program.CustAccount.Show();
        }

        private void productGSTReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.GSTReports == null)
            {
                GSTReport objpriceuploads = new GSTReport();
                Program.GSTReports = objpriceuploads;
            }
            else
            {
                Program.GSTReports.Dispose();
                GSTReport objpriceuploads = new GSTReport();
                Program.GSTReports = objpriceuploads;
            }
            Program.GSTReports.MdiParent = this;
            Program.GSTReports.Show();
        }

        private void hSNStockReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ObjHsnstockreport == null)
            {
                HsnStockReport ObjHsnstockreport = new HsnStockReport();
                Program.ObjHsnstockreport = ObjHsnstockreport;
            }
            else
            {
                Program.ObjHsnstockreport.Dispose();
                HsnStockReport ObjHsnstockreport = new HsnStockReport();
                Program.ObjHsnstockreport = ObjHsnstockreport;
            }
            Program.ObjHsnstockreport.MdiParent = this;
            Program.ObjHsnstockreport.Show();
        }

        private void customerWiseLedgerReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objCustomerledger == null)
            {
                Customerwiseledger objCustomerledger = new Customerwiseledger();
                Program.objCustomerledger = objCustomerledger;
            }
            else
            {
                Program.objCustomerledger.Dispose();
                Customerwiseledger objCustomerledger = new Customerwiseledger();
                Program.objCustomerledger = objCustomerledger;
            }
            Program.objCustomerledger.MdiParent = this;
            Program.objCustomerledger.Show();
        }

        private void rackUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objrack == null)
            {
                RackUpload objrack = new RackUpload();
                Program.objrack = objrack;
            }
            else
            {
                Program.objrack.Dispose();
                RackUpload objrack = new RackUpload();
                Program.objrack = objrack;
            }
            Program.objrack.MdiParent = this;
            Program.objrack.Show();
        }

        private void gSTNewReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.GstFormReports == null)
            {
                GstFormReport GstFormReports = new GstFormReport();
                Program.GstFormReports = GstFormReports;
            }
            else
            {
                Program.GstFormReports.Dispose();
                GstFormReport GstFormReports = new GstFormReport();
                Program.GstFormReports = GstFormReports;
            }
            Program.GstFormReports.MdiParent = this;
            Program.GstFormReports.Show();
        }

        private void passwordSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.objFrmPasswordSetup == null)
            {
                FrmPasswordSetup objFrmPasswordSetup = new FrmPasswordSetup();
                Program.objFrmPasswordSetup = objFrmPasswordSetup;
            }
            else
            {
                Program.objFrmPasswordSetup.Dispose();
                FrmPasswordSetup objFrmPasswordSetup = new FrmPasswordSetup();
                Program.objFrmPasswordSetup = objFrmPasswordSetup;
            }
            Program.objFrmPasswordSetup.MdiParent = this;
            Program.objFrmPasswordSetup.Show();
        }

        private class CloudEyeMenuRenderer : ToolStripProfessionalRenderer
        {
            public CloudEyeMenuRenderer(ProfessionalColorTable colorTable)
                : base(colorTable)
            {
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
                bool isTopMenu = e.Item.Owner is MenuStrip;
                bool selected = e.Item.Selected;
                bool pressed = false;

                ToolStripMenuItem menuItem = e.Item as ToolStripMenuItem;
                if (menuItem != null)
                {
                    pressed = menuItem.Pressed || menuItem.DropDown.Visible;
                }

                if (selected || pressed)
                {
                    Color startColor;
                    Color endColor;
                    Color borderColor;

                    if (isTopMenu)
                    {
                        if (pressed)
                        {
                            startColor = Color.FromArgb(249, 115, 22);
                            endColor = Color.FromArgb(251, 146, 60);
                            borderColor = Color.FromArgb(255, 237, 213);
                        }
                        else
                        {
                            startColor = Color.FromArgb(37, 99, 235);
                            endColor = Color.FromArgb(14, 165, 233);
                            borderColor = Color.FromArgb(250, 204, 21);
                        }

                        Rectangle fillRect = new Rectangle(rect.X + 1, rect.Y + 2, rect.Width - 2, rect.Height - 4);
                        using (GraphicsPath path = GetRoundRect(fillRect, 8))
                        using (LinearGradientBrush brush = new LinearGradientBrush(fillRect, startColor, endColor, LinearGradientMode.Horizontal))
                        using (Pen pen = new Pen(borderColor))
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            e.Graphics.FillPath(brush, path);
                            e.Graphics.DrawPath(pen, path);
                        }

                        using (SolidBrush indicator = new SolidBrush(Color.FromArgb(250, 204, 21)))
                        {
                            e.Graphics.FillRectangle(indicator, rect.X + 6, rect.Bottom - 3, Math.Max(10, rect.Width - 12), 2);
                        }
                    }
                    else
                    {
                        Rectangle fillRect = new Rectangle(rect.X + 2, rect.Y + 1, rect.Width - 4, rect.Height - 2);
                        using (GraphicsPath path = GetRoundRect(fillRect, 6))
                        using (LinearGradientBrush brush = new LinearGradientBrush(fillRect, Color.FromArgb(239, 246, 255), Color.FromArgb(219, 234, 254), LinearGradientMode.Horizontal))
                        using (Pen pen = new Pen(Color.FromArgb(147, 197, 253)))
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            e.Graphics.FillPath(brush, path);
                            e.Graphics.DrawPath(pen, path);
                        }

                        using (SolidBrush leftLine = new SolidBrush(Color.FromArgb(249, 115, 22)))
                        {
                            e.Graphics.FillRectangle(leftLine, rect.X + 3, rect.Y + 4, 3, rect.Height - 8);
                        }
                    }
                }
                else
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                bool isTopMenu = e.Item.Owner is MenuStrip;
                if (isTopMenu)
                {
                    e.TextColor = Color.White;
                }
                else if (e.Item.Selected)
                {
                    e.TextColor = Color.FromArgb(15, 23, 42);
                }
                else
                {
                    e.TextColor = Color.FromArgb(30, 41, 59);
                }
                base.OnRenderItemText(e);
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                if (e.ToolStrip is MenuStrip)
                {
                    using (Pen pen = new Pen(Color.FromArgb(56, 189, 248)))
                    {
                        e.Graphics.DrawLine(pen, 0, e.ToolStrip.Height - 1, e.ToolStrip.Width, e.ToolStrip.Height - 1);
                    }
                }
                else
                {
                    using (Pen pen = new Pen(Color.FromArgb(203, 213, 225)))
                    {
                        Rectangle rect = new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
                        e.Graphics.DrawRectangle(pen, rect);
                    }
                }
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                Rectangle rect = new Rectangle(28, e.Item.Height / 2, e.Item.Width - 32, 1);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(226, 232, 240)))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            }

            private GraphicsPath GetRoundRect(Rectangle rect, int radius)
            {
                GraphicsPath path = new GraphicsPath();
                int diameter = radius * 2;

                if (diameter > rect.Width) diameter = rect.Width;
                if (diameter > rect.Height) diameter = rect.Height;

                Rectangle arc = new Rectangle(rect.Location, new Size(diameter, diameter));
                path.AddArc(arc, 180, 90);

                arc.X = rect.Right - diameter;
                path.AddArc(arc, 270, 90);

                arc.Y = rect.Bottom - diameter;
                path.AddArc(arc, 0, 90);

                arc.X = rect.Left;
                path.AddArc(arc, 90, 90);

                path.CloseFigure();
                return path;
            }
        }

        private class CloudEyeMenuColorTable : ProfessionalColorTable
        {
            public override Color MenuStripGradientBegin { get { return Color.FromArgb(12, 74, 110); } }
            public override Color MenuStripGradientEnd { get { return Color.FromArgb(14, 116, 144); } }
            public override Color ToolStripDropDownBackground { get { return Color.White; } }
            public override Color MenuItemSelected { get { return Color.FromArgb(37, 99, 235); } }
            public override Color MenuItemSelectedGradientBegin { get { return Color.FromArgb(37, 99, 235); } }
            public override Color MenuItemSelectedGradientEnd { get { return Color.FromArgb(14, 165, 233); } }
            public override Color MenuItemPressedGradientBegin { get { return Color.FromArgb(249, 115, 22); } }
            public override Color MenuItemPressedGradientEnd { get { return Color.FromArgb(251, 146, 60); } }
            public override Color MenuItemBorder { get { return Color.FromArgb(250, 204, 21); } }
            public override Color ImageMarginGradientBegin { get { return Color.FromArgb(248, 250, 252); } }
            public override Color ImageMarginGradientMiddle { get { return Color.FromArgb(241, 245, 249); } }
            public override Color ImageMarginGradientEnd { get { return Color.FromArgb(226, 232, 240); } }
            public override Color SeparatorDark { get { return Color.FromArgb(203, 213, 225); } }
            public override Color SeparatorLight { get { return Color.White; } }
        }
        private void FixMdiClientArea()
        {
            foreach (Control ctl in this.Controls)
            {
                if (ctl is MdiClient)
                {
                    ctl.Location = new Point(0, menuStrip1.Height);
                    ctl.Size = new Size(
                        this.ClientSize.Width,
                        this.ClientSize.Height - menuStrip1.Height
                    );
                    break;
                }
            }
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            FixMdiClientArea();
        }
    }
}
