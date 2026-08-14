
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.IO;
using System.Text;

using System.Windows.Forms;


using InvBal;
using Inventory.Sales;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using QuotationReport;
using System.Collections;
using System.Configuration;
//using QuotationReport;

namespace Inventory
{

    public partial class DeliveryNote : Form
    {
        public static string Conn = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        QuotationBal objQuotationbal = new QuotationBal();
        TextBox tb, tbrate;
        bool load = false;
        public bool edit = false;
        int userid = 0;
        string cas = string.Empty;
        string role1 = string.Empty;
        string srole = string.Empty;
        int ProdSelRowvalue = 0;
        bool res = false;
        DataTable dtitems;
        string clickstatus = string.Empty;
        DataTable StockCheck;
        bool followupDateStorageReady = false;

        
        bool savevads = false;
        string firstname, firstvalue, secondname, secondvalue, thirdname, thirdvalue;
        private readonly bool approvalMode;
        private readonly bool receiptMode;
        private int currentDeliveryNoteId;
        private TextBox txtReference;
        private Label lblReference;
        private TextBox txtEWayBillNo;
        private TextBox txtModeTermsOfPayment;
        private TextBox txtOtherReferences;
        private TextBox txtBuyerOrderNo;
        private TextBox txtBuyerBillNo;
        private TextBox txtDestinationStateName;
        private DateTimePicker dtBuyerOrderDate;
        private TextBox txtDispatchDocNo;
        private TextBox txtDispatchedThrough;
        private TextBox txtBillOfLadingNo;
        private TextBox txtMotorVehicleNo;
        private TextBox txtTermsOfDelivery;
        private TableLayoutPanel tblDeliveryDetails;
        public DeliveryNote()
            : this(false, false)
        {
        }

        public DeliveryNote(bool approvalMode)
            : this(approvalMode, false)
        {
        }

        public DeliveryNote(bool approvalMode, bool receiptMode)
        {
            this.approvalMode = approvalMode;
            this.receiptMode = receiptMode;
            InitializeComponent();
            RemoveDeliveryNoteUnusedControls();
            ConfigureDeliveryNoteHeader();
            EnsureDeliveryNoteBackendSchema();
            EnsureFollowupDateStorage();
            srole = Program.Userrole;
            if (srole != "Admin")
            {
                role1 = "Emp";
            }
            else
            {
                role1 = "Admin";
            }
            pnlprodsearch.Visible = false;
            userid = Convert.ToInt32(Program.userid);
            //this.WindowState = FormWindowState.Maximized;
            LoadPorts();
            SearchCreteria1();
            SearchCreteria2();
            SearchCreteria3();
            bindLocation();
            SafeResetCombo(cmbloaction);
            //comboBox1.SelectedIndex = 0;
            SafeResetCombo(cmbstatus);
            SearchPurchaseOrder();
            lblperare.Text = Program.Userfullname;
            bindAssist();
            bindreference();
            bindAssistName();
            bindcustomer();
            DataTable dtitems = Program.dtitems;
            if (dtitems == null)
            {
                itemdetails("");
            }
            //Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //Txtitem.AutoCompleteCustomSource = AutoCompleteLoad();
            //Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //bindpending();
           
            Globeimage();
            DataTable dt = GetBranches();
            cmbstatus3.DataSource = dt;
            cmbstatus3.ValueMember = "BranchId";
            cmbstatus3.DisplayMember = "BranchName";
            SafeResetCombo(cmbstatus3);


            GetSearchOrder();
           




        }

        private void RemoveDeliveryNoteUnusedControls()
        {
            RemoveControl(pcupdate);
            RemoveControl(RollPaper);
            RemoveControl(btnLess);
            RemoveControl(labelFollowupDate);
            RemoveControl(followupdate);
            RemoveControl(label30);
            RemoveControl(comboBox1);
            if (receiptMode)
            {
                RemoveControl(btnPrint);
            }
            else
            {
                btnPrint.Visible = true;
                btnPrint.Enabled = false;
                btnPrint.Text = "Print";
            }
            RemoveControl(btnSavePending);
            RemoveControl(btnNew);
            RemoveControl(cmdcity);
            RemoveControl(label12);
            RemoveControl(cmbassistby);
            RemoveControl(label9);
            RemoveControl(label31);

            btnSave.Location = new Point(btnClear.Left - btnSave.Width - 4, btnSave.Top);
            if (!receiptMode)
            {
                btnPrint.Location = new Point(btnSave.Left - btnPrint.Width - 4, btnSave.Top);
            }
        }

        private void ConfigureDeliveryNoteHeader()
        {
            label2.Text = "From Location";
            label7.Text = "To Location";
            label7.Visible = true;
            label17.Text = approvalMode ? NoteTitle() + " Approval" : NoteTitle();
            btnSave.Text = approvalMode ? "Approve" : "Save";
            lblOrderNumber.Text = NoteTitle();
            lblVendor.Text = "To";
            ConfigureReferenceField();
            ConfigureDeliveryNoteFields();

            cmbcustomername.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbcustomername.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbcustomername.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbcustomername.SelectedIndexChanged -= new EventHandler(cmbcustomername_SelectedIndexChanged);

            dgvOrder.Top = receiptMode ? 128 : 240;
            dgvOrder.Height = pntab.Height - dgvOrder.Top - 42;
        }

        private void ConfigureReferenceField()
        {
            if (!receiptMode)
            {
                RemoveControl(cmbreference);
                RemoveControl(label14);
                return;
            }

            RemoveControl(cmbreference);
            label14.Text = "Reference";
            label14.Visible = true;
            label14.Location = new Point(label15.Left + 105, label15.Top);
            label14.AutoSize = true;
            if (label14.Parent != pntab)
            {
                label14.Parent.Controls.Remove(label14);
                pntab.Controls.Add(label14);
            }

            txtReference = new TextBox();
            txtReference.Font = cmbToLocation.Font;
            txtReference.Location = new Point(label14.Left + 70, label15.Top - 3);
            txtReference.MaxLength = 100;
            txtReference.Name = "txtReference";
            txtReference.Size = new Size(180, cmbToLocation.Height);
            txtReference.TabIndex = 4;
            pntab.Controls.Add(txtReference);
            txtReference.BringToFront();
            label14.BringToFront();

            lblReference = label14;
        }

        private void ConfigureDeliveryNoteFields()
        {
            if (receiptMode)
                return;

            tblDeliveryDetails = new TableLayoutPanel();
            tblDeliveryDetails.ColumnCount = 4;
            tblDeliveryDetails.RowCount = 7;
            tblDeliveryDetails.Location = new Point(270, 38);
            tblDeliveryDetails.Margin = new Padding(0);
            tblDeliveryDetails.Name = "tblDeliveryDetails";
            tblDeliveryDetails.Size = new Size(445, 188);
            tblDeliveryDetails.TabIndex = 5;

            tblDeliveryDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108F));
            tblDeliveryDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblDeliveryDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
            tblDeliveryDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            for (int i = 0; i < tblDeliveryDetails.RowCount; i++)
                tblDeliveryDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));

            pntab.Controls.Add(tblDeliveryDetails);
            tblDeliveryDetails.BringToFront();

            txtEWayBillNo = AddDeliveryTableTextBox("txtEWayBillNo", "e-Way Bill No", 0, 0, 5);
            txtModeTermsOfPayment = AddDeliveryTableTextBox("txtModeTermsOfPayment", "Mode/Terms", 2, 0, 6);
            txtReference = AddDeliveryTableTextBox("txtReference", "Reference No/Date", 0, 1, 7);
            txtOtherReferences = AddDeliveryTableTextBox("txtOtherReferences", "Other Reference", 2, 1, 8);
            txtBuyerOrderNo = AddDeliveryTableTextBox("txtBuyerOrderNo", "Buyer Order No", 0, 2, 9);
            dtBuyerOrderDate = AddDeliveryTableDate("dtBuyerOrderDate", "Order Date", 2, 2, 10);
            txtBuyerBillNo = AddDeliveryTableTextBox("txtBuyerBillNo", "Buyer Bill No", 0, 3, 11);
            txtDestinationStateName = AddDeliveryTableTextBox("txtDestinationStateName", "State Name", 2, 3, 12);
            txtDispatchDocNo = AddDeliveryTableTextBox("txtDispatchDocNo", "Dispatch Doc No", 0, 4, 13);
            txtDispatchedThrough = AddDeliveryTableTextBox("txtDispatchedThrough", "Dispatched Through", 2, 4, 14);
            txtBillOfLadingNo = AddDeliveryTableTextBox("txtBillOfLadingNo", "LR-RR No", 0, 5, 15);
            txtMotorVehicleNo = AddDeliveryTableTextBox("txtMotorVehicleNo", "Vehicle No", 2, 5, 16);
            txtTermsOfDelivery = AddDeliveryTableTextBox("txtTermsOfDelivery", "Terms", 0, 6, 17);
            tblDeliveryDetails.SetColumnSpan(txtTermsOfDelivery, 3);
        }

        private TextBox AddDeliveryTableTextBox(string name, string labelText, int labelColumn, int row, int tabIndex)
        {
            AddDeliveryTableLabel(labelText, labelColumn, row);
            TextBox textBox = new TextBox();
            textBox.Dock = DockStyle.Fill;
            textBox.Font = cmbToLocation.Font;
            textBox.Margin = new Padding(3, 2, 3, 1);
            textBox.MaxLength = 100;
            textBox.Name = name;
            textBox.TabIndex = tabIndex;
            tblDeliveryDetails.Controls.Add(textBox, labelColumn + 1, row);
            return textBox;
        }

        private DateTimePicker AddDeliveryTableDate(string name, string labelText, int labelColumn, int row, int tabIndex)
        {
            AddDeliveryTableLabel(labelText, labelColumn, row);
            DateTimePicker picker = new DateTimePicker();
            picker.Checked = false;
            picker.CustomFormat = "dd-MM-yyyy";
            picker.Dock = DockStyle.Fill;
            picker.Font = cmbToLocation.Font;
            picker.Format = DateTimePickerFormat.Custom;
            picker.Margin = new Padding(3, 2, 3, 1);
            picker.Name = name;
            picker.ShowCheckBox = true;
            picker.TabIndex = tabIndex;
            tblDeliveryDetails.Controls.Add(picker, labelColumn + 1, row);
            return picker;
        }

        private void AddDeliveryTableLabel(string text, int column, int row)
        {
            Label label = new Label();
            label.Dock = DockStyle.Fill;
            label.Font = label2.Font;
            label.Margin = new Padding(0, 0, 4, 0);
            label.Text = text;
            label.TextAlign = ContentAlignment.MiddleRight;
            tblDeliveryDetails.Controls.Add(label, column, row);
        }

        private TextBox AddDeliveryTextBox(string name, string labelText, int labelX, int inputX, int y, Size size, int tabIndex)
        {
            AddDeliveryLabel(labelText, labelX, y + 3);
            TextBox textBox = new TextBox();
            textBox.Font = cmbToLocation.Font;
            textBox.Location = new Point(inputX, y);
            textBox.MaxLength = 100;
            textBox.Name = name;
            textBox.Size = size;
            textBox.TabIndex = tabIndex;
            pntab.Controls.Add(textBox);
            textBox.BringToFront();
            return textBox;
        }

        private DateTimePicker AddDeliveryDate(string name, string labelText, int labelX, int inputX, int y, Size size, int tabIndex)
        {
            AddDeliveryLabel(labelText, labelX, y + 3);
            DateTimePicker picker = new DateTimePicker();
            picker.Checked = false;
            picker.CustomFormat = "dd-MM-yyyy";
            picker.Font = cmbToLocation.Font;
            picker.Format = DateTimePickerFormat.Custom;
            picker.Location = new Point(inputX, y);
            picker.Name = name;
            picker.ShowCheckBox = true;
            picker.Size = size;
            picker.TabIndex = tabIndex;
            pntab.Controls.Add(picker);
            picker.BringToFront();
            return picker;
        }

        private void AddDeliveryLabel(string text, int x, int y)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Font = label2.Font;
            label.Location = new Point(x, y);
            label.Text = text;
            pntab.Controls.Add(label);
            label.BringToFront();
        }

        private string NoteTitle()
        {
            return receiptMode ? "Receipt Note" : "Delivery Note";
        }

        private string HeaderTable()
        {
            return receiptMode ? "ReceiptNote" : "DeliveryNote";
        }

        private string DetailTable()
        {
            return receiptMode ? "ReceiptNoteDetail" : "DeliveryNoteDetail";
        }

        private string HeaderIdColumn()
        {
            return receiptMode ? "ReceiptNoteId" : "DeliveryNoteId";
        }

        private string DetailIdColumn()
        {
            return receiptMode ? "ReceiptNoteDetailId" : "DeliveryNoteDetailId";
        }

        private string NoteNoColumn()
        {
            return receiptMode ? "ReceiptNoteNo" : "DeliveryNoteNo";
        }

        private string NoteDateColumn()
        {
            return receiptMode ? "ReceiptNoteDate" : "DeliveryNoteDate";
        }

        private string NotePrefix()
        {
            return receiptMode ? "RN" : "DN";
        }

        private string MaterialTransactionType()
        {
            return receiptMode ? "RECEIPT NOTE" : "DELIVERY NOTE";
        }

        private string MaterialInOutType()
        {
            return receiptMode ? "IN" : "OUT";
        }

        private object GetReferenceValue()
        {
            if (txtReference == null || string.IsNullOrEmpty(txtReference.Text.Trim()))
                return DBNull.Value;
            return txtReference.Text.Trim();
        }

        private object DeliveryTextValue(TextBox textBox)
        {
            if (textBox == null || string.IsNullOrEmpty(textBox.Text.Trim()))
                return DBNull.Value;
            return textBox.Text.Trim();
        }

        private object DeliveryDateValue(DateTimePicker picker)
        {
            if (picker == null || !picker.Checked)
                return DBNull.Value;
            return picker.Value.Date;
        }

        private string DeliveryText(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
                return string.Empty;
            return Convert.ToString(row[columnName]);
        }

        private DateTime? DeliveryDate(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
                return null;
            return Convert.ToDateTime(row[columnName]);
        }

        private int GetSelectedBranchId(ComboBox comboBox)
        {
            if (comboBox == null || comboBox.SelectedValue == null)
                return 0;

            int branchId = 0;
            int.TryParse(Convert.ToString(comboBox.SelectedValue), out branchId);
            return branchId;
        }

        private DataTable GetBranches()
        {
            string idColumn = GetFirstColumn("Branches", new string[] { "id", "BranchId", "BranchID", "ID", "Id" });
            string nameColumn = GetFirstColumn("Branches", new string[] { "location_name", "BranchName", "Name", "Branch", "LocationName" });
            string codeColumn = GetFirstColumn("Branches", new string[] { "BranchCode", "Code" });
            string activeColumn = GetFirstColumn("Branches", new string[] { "IsActive", "Active" });

            if (string.IsNullOrEmpty(idColumn) || string.IsNullOrEmpty(nameColumn))
                throw new ApplicationException("Branches table should have branch id and branch name columns.");

            string display = "ISNULL(CONVERT(varchar(150), " + SqlName(nameColumn) + "), '')";
            if (!string.IsNullOrEmpty(codeColumn))
                display = "ISNULL(CONVERT(varchar(50), " + SqlName(codeColumn) + "), '') + ' - ' + ISNULL(CONVERT(varchar(150), " + SqlName(nameColumn) + "), '')";

            string sql = "SELECT CAST(" + SqlName(idColumn) + " AS int) AS BranchId, " + display + " AS BranchName FROM dbo.Branches";
            if (!string.IsNullOrEmpty(activeColumn))
                sql += " WHERE ISNULL(" + SqlName(activeColumn) + ", 1) = 1";
            sql += " ORDER BY " + SqlName(nameColumn);

            DataTable table = ExecuteDeliveryTable(sql);
            DataRow row = table.NewRow();
            row["BranchId"] = 0;
            row["BranchName"] = "--Select--";
            table.Rows.InsertAt(row, 0);
            return table;
        }

        private void EnsureDeliveryNoteBackendSchema()
        {
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                ExecuteDeliveryNonQuery(con, null, @"
IF OBJECT_ID('dbo.DeliveryNote', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DeliveryNote
    (
        DeliveryNoteId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DeliveryNote PRIMARY KEY,
        DeliveryNoteNo VARCHAR(30) NOT NULL,
        DeliveryNoteDate DATETIME NOT NULL,
        FromBranchId INT NULL,
        ToBranchId INT NULL,
        Status VARCHAR(20) NOT NULL CONSTRAINT DF_DeliveryNote_Status DEFAULT ('PENDING'),
        EnteredBy VARCHAR(20) NULL,
        EnteredOn DATETIME NOT NULL CONSTRAINT DF_DeliveryNote_EnteredOn DEFAULT (GETDATE()),
        ApprovedBy VARCHAR(20) NULL,
        ApprovedOn DATETIME NULL,
        IsDeleted BIT NOT NULL CONSTRAINT DF_DeliveryNote_IsDeleted DEFAULT (0)
    );
END
IF COL_LENGTH('dbo.DeliveryNote', 'FromBranchId') IS NULL ALTER TABLE dbo.DeliveryNote ADD FromBranchId INT NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'ToBranchId') IS NULL ALTER TABLE dbo.DeliveryNote ADD ToBranchId INT NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'Status') IS NULL ALTER TABLE dbo.DeliveryNote ADD Status VARCHAR(20) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'ApprovedBy') IS NULL ALTER TABLE dbo.DeliveryNote ADD ApprovedBy VARCHAR(20) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'ApprovedOn') IS NULL ALTER TABLE dbo.DeliveryNote ADD ApprovedOn DATETIME NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'IsDeleted') IS NULL ALTER TABLE dbo.DeliveryNote ADD IsDeleted BIT NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'FromLocationId') IS NOT NULL ALTER TABLE dbo.DeliveryNote ALTER COLUMN FromLocationId INT NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'ToLocationId') IS NOT NULL ALTER TABLE dbo.DeliveryNote ALTER COLUMN ToLocationId INT NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'ReferenceNo') IS NULL ALTER TABLE dbo.DeliveryNote ADD ReferenceNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'EWayBillNo') IS NULL ALTER TABLE dbo.DeliveryNote ADD EWayBillNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'ModeTermsOfPayment') IS NULL ALTER TABLE dbo.DeliveryNote ADD ModeTermsOfPayment VARCHAR(100) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'OtherReferences') IS NULL ALTER TABLE dbo.DeliveryNote ADD OtherReferences VARCHAR(100) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'BuyerOrderNo') IS NULL ALTER TABLE dbo.DeliveryNote ADD BuyerOrderNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'BuyerBillNo') IS NULL ALTER TABLE dbo.DeliveryNote ADD BuyerBillNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'DestinationStateName') IS NULL ALTER TABLE dbo.DeliveryNote ADD DestinationStateName VARCHAR(100) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'BuyerOrderDate') IS NULL ALTER TABLE dbo.DeliveryNote ADD BuyerOrderDate DATETIME NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'DispatchDocNo') IS NULL ALTER TABLE dbo.DeliveryNote ADD DispatchDocNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'DispatchedThrough') IS NULL ALTER TABLE dbo.DeliveryNote ADD DispatchedThrough VARCHAR(100) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'BillOfLadingNo') IS NULL ALTER TABLE dbo.DeliveryNote ADD BillOfLadingNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'MotorVehicleNo') IS NULL ALTER TABLE dbo.DeliveryNote ADD MotorVehicleNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo.DeliveryNote', 'TermsOfDelivery') IS NULL ALTER TABLE dbo.DeliveryNote ADD TermsOfDelivery VARCHAR(250) NULL;
IF OBJECT_ID('dbo.DeliveryNoteDetail', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DeliveryNoteDetail
    (
        DeliveryNoteDetailId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DeliveryNoteDetail PRIMARY KEY,
        DeliveryNoteId INT NOT NULL,
        MaterialId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL,
        Remarks VARCHAR(250) NULL
    );
END");
                EnsureNoteTables(con, "Receipt");
                EnsureAvailableStockProcedure(con);
            }
        }

        private void EnsureNoteTables(SqlConnection con, string noteName)
        {
            string headerTable = noteName + "Note";
            string detailTable = noteName + "NoteDetail";
            string headerId = noteName + "NoteId";
            string detailId = noteName + "NoteDetailId";
            string noteNo = noteName + "NoteNo";
            string noteDate = noteName + "NoteDate";

            ExecuteDeliveryNonQuery(con, null, @"
IF OBJECT_ID('dbo." + headerTable + @"', 'U') IS NULL
BEGIN
    CREATE TABLE dbo." + headerTable + @"
    (
        " + headerId + @" INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_" + headerTable + @" PRIMARY KEY,
        " + noteNo + @" VARCHAR(30) NOT NULL,
        " + noteDate + @" DATETIME NOT NULL,
        FromBranchId INT NULL,
        ToBranchId INT NULL,
        Status VARCHAR(20) NOT NULL CONSTRAINT DF_" + headerTable + @"_Status DEFAULT ('PENDING'),
        EnteredBy VARCHAR(20) NULL,
        EnteredOn DATETIME NOT NULL CONSTRAINT DF_" + headerTable + @"_EnteredOn DEFAULT (GETDATE()),
        ApprovedBy VARCHAR(20) NULL,
        ApprovedOn DATETIME NULL,
        IsDeleted BIT NOT NULL CONSTRAINT DF_" + headerTable + @"_IsDeleted DEFAULT (0)
    );
END
IF COL_LENGTH('dbo." + headerTable + @"', 'FromBranchId') IS NULL ALTER TABLE dbo." + headerTable + @" ADD FromBranchId INT NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'ToBranchId') IS NULL ALTER TABLE dbo." + headerTable + @" ADD ToBranchId INT NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'Status') IS NULL ALTER TABLE dbo." + headerTable + @" ADD Status VARCHAR(20) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'ApprovedBy') IS NULL ALTER TABLE dbo." + headerTable + @" ADD ApprovedBy VARCHAR(20) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'ApprovedOn') IS NULL ALTER TABLE dbo." + headerTable + @" ADD ApprovedOn DATETIME NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'IsDeleted') IS NULL ALTER TABLE dbo." + headerTable + @" ADD IsDeleted BIT NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'ReferenceNo') IS NULL ALTER TABLE dbo." + headerTable + @" ADD ReferenceNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'EWayBillNo') IS NULL ALTER TABLE dbo." + headerTable + @" ADD EWayBillNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'ModeTermsOfPayment') IS NULL ALTER TABLE dbo." + headerTable + @" ADD ModeTermsOfPayment VARCHAR(100) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'OtherReferences') IS NULL ALTER TABLE dbo." + headerTable + @" ADD OtherReferences VARCHAR(100) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'BuyerOrderNo') IS NULL ALTER TABLE dbo." + headerTable + @" ADD BuyerOrderNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'BuyerBillNo') IS NULL ALTER TABLE dbo." + headerTable + @" ADD BuyerBillNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'DestinationStateName') IS NULL ALTER TABLE dbo." + headerTable + @" ADD DestinationStateName VARCHAR(100) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'BuyerOrderDate') IS NULL ALTER TABLE dbo." + headerTable + @" ADD BuyerOrderDate DATETIME NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'DispatchDocNo') IS NULL ALTER TABLE dbo." + headerTable + @" ADD DispatchDocNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'DispatchedThrough') IS NULL ALTER TABLE dbo." + headerTable + @" ADD DispatchedThrough VARCHAR(100) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'BillOfLadingNo') IS NULL ALTER TABLE dbo." + headerTable + @" ADD BillOfLadingNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'MotorVehicleNo') IS NULL ALTER TABLE dbo." + headerTable + @" ADD MotorVehicleNo VARCHAR(100) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'TermsOfDelivery') IS NULL ALTER TABLE dbo." + headerTable + @" ADD TermsOfDelivery VARCHAR(250) NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'FromLocationId') IS NOT NULL ALTER TABLE dbo." + headerTable + @" ALTER COLUMN FromLocationId INT NULL;
IF COL_LENGTH('dbo." + headerTable + @"', 'ToLocationId') IS NOT NULL ALTER TABLE dbo." + headerTable + @" ALTER COLUMN ToLocationId INT NULL;
IF OBJECT_ID('dbo." + detailTable + @"', 'U') IS NULL
BEGIN
    CREATE TABLE dbo." + detailTable + @"
    (
        " + detailId + @" INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_" + detailTable + @" PRIMARY KEY,
        " + headerId + @" INT NOT NULL,
        MaterialId INT NOT NULL,
        Quantity DECIMAL(18,3) NOT NULL,
        Remarks VARCHAR(250) NULL
    );
END");
        }

        private void EnsureAvailableStockProcedure(SqlConnection con)
        {
            ExecuteDeliveryNonQuery(con, null, @"
IF OBJECT_ID('dbo.GetAvailableStockByProductId', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.GetAvailableStockByProductId AS BEGIN SET NOCOUNT ON; END');
END");

            ExecuteDeliveryNonQuery(con, null, @"
ALTER PROCEDURE dbo.GetAvailableStockByProductId
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        @ProductId AS ProductId,
        CAST(
            ISNULL(SUM(
                CASE
                    WHEN UPPER(ISNULL([Type], '')) = 'IN'
                        THEN ISNULL(Quantity, 0)
                    WHEN UPPER(ISNULL([Type], '')) = 'OUT'
                        THEN -ISNULL(Quantity, 0)
                    ELSE 0
                END
            ), 0)
        AS DECIMAL(18, 3)) AS AvailableStock
    FROM dbo.MaterialTranscation
    WHERE MaterailId = @ProductId
      AND LocationId = 6;
END");
        }

        private string SaveDeliveryNotePending()
        {
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    string noteNo = GenerateDeliveryNoteNo(con, tran, date.Value);
                    int noteId;
                    using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo." + HeaderTable() + @" (" + NoteNoColumn() + @", " + NoteDateColumn() + @", FromBranchId, ToBranchId, ReferenceNo, EWayBillNo, ModeTermsOfPayment, OtherReferences, BuyerOrderNo, BuyerBillNo, DestinationStateName, BuyerOrderDate, DispatchDocNo, DispatchedThrough, BillOfLadingNo, MotorVehicleNo, TermsOfDelivery, Status, EnteredBy, EnteredOn, IsDeleted)
VALUES (@No, @Date, @FromBranchId, @ToBranchId, @ReferenceNo, @EWayBillNo, @ModeTermsOfPayment, @OtherReferences, @BuyerOrderNo, @BuyerBillNo, @DestinationStateName, @BuyerOrderDate, @DispatchDocNo, @DispatchedThrough, @BillOfLadingNo, @MotorVehicleNo, @TermsOfDelivery, 'PENDING', @User, GETDATE(), 0);
SELECT CAST(SCOPE_IDENTITY() AS int);", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@No", noteNo);
                        cmd.Parameters.AddWithValue("@Date", date.Value.Date);
                        cmd.Parameters.AddWithValue("@FromBranchId", Convert.ToInt32(cmbcustomername.SelectedValue));
                        cmd.Parameters.AddWithValue("@ToBranchId", Convert.ToInt32(cmbToLocation.SelectedValue));
                        cmd.Parameters.AddWithValue("@ReferenceNo", GetReferenceValue());
                        cmd.Parameters.AddWithValue("@EWayBillNo", DeliveryTextValue(txtEWayBillNo));
                        cmd.Parameters.AddWithValue("@ModeTermsOfPayment", DeliveryTextValue(txtModeTermsOfPayment));
                        cmd.Parameters.AddWithValue("@OtherReferences", DeliveryTextValue(txtOtherReferences));
                        cmd.Parameters.AddWithValue("@BuyerOrderNo", DeliveryTextValue(txtBuyerOrderNo));
                        cmd.Parameters.AddWithValue("@BuyerBillNo", DeliveryTextValue(txtBuyerBillNo));
                        cmd.Parameters.AddWithValue("@DestinationStateName", DeliveryTextValue(txtDestinationStateName));
                        cmd.Parameters.AddWithValue("@BuyerOrderDate", DeliveryDateValue(dtBuyerOrderDate));
                        cmd.Parameters.AddWithValue("@DispatchDocNo", DeliveryTextValue(txtDispatchDocNo));
                        cmd.Parameters.AddWithValue("@DispatchedThrough", DeliveryTextValue(txtDispatchedThrough));
                        cmd.Parameters.AddWithValue("@BillOfLadingNo", DeliveryTextValue(txtBillOfLadingNo));
                        cmd.Parameters.AddWithValue("@MotorVehicleNo", DeliveryTextValue(txtMotorVehicleNo));
                        cmd.Parameters.AddWithValue("@TermsOfDelivery", DeliveryTextValue(txtTermsOfDelivery));
                        cmd.Parameters.AddWithValue("@User", CurrentUser());
                        noteId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    InsertDeliveryNoteDetailRows(con, tran, noteId);
                    tran.Commit();
                    currentDeliveryNoteId = noteId;
                    return noteNo;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        private void ApproveDeliveryNote()
        {
            if (currentDeliveryNoteId <= 0)
                throw new ApplicationException("Please select a delivery note for approval.");

            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction(IsolationLevel.Serializable);
                try
                {
                    string status = Convert.ToString(ExecuteDeliveryScalar(con, tran, "SELECT Status FROM dbo." + HeaderTable() + " WITH (UPDLOCK, HOLDLOCK) WHERE " + HeaderIdColumn() + " = @Id", new SqlParameter("@Id", currentDeliveryNoteId)));
                    if (status.ToUpper() != "PENDING")
                        throw new ApplicationException("This delivery note is already " + status + ".");

                    ExecuteDeliveryNonQuery(con, tran, "DELETE FROM dbo." + DetailTable() + " WHERE " + HeaderIdColumn() + " = @Id", new SqlParameter("@Id", currentDeliveryNoteId));
                    InsertDeliveryNoteDetailRows(con, tran, currentDeliveryNoteId);
                    string deliveryNoteNo = Convert.ToString(ExecuteDeliveryScalar(con, tran, "SELECT " + NoteNoColumn() + " FROM dbo." + HeaderTable() + " WHERE " + HeaderIdColumn() + " = @Id", new SqlParameter("@Id", currentDeliveryNoteId)));
                    DeleteMaterialTransactionRows(con, tran, deliveryNoteNo);
                    InsertMaterialTransactionRows(con, tran, currentDeliveryNoteId, deliveryNoteNo);
                    ExecuteDeliveryNonQuery(con, tran, "UPDATE dbo." + HeaderTable() + " SET FromBranchId = @FromBranchId, ToBranchId = @ToBranchId, ReferenceNo = @ReferenceNo, EWayBillNo = @EWayBillNo, ModeTermsOfPayment = @ModeTermsOfPayment, OtherReferences = @OtherReferences, BuyerOrderNo = @BuyerOrderNo, BuyerBillNo = @BuyerBillNo, DestinationStateName = @DestinationStateName, BuyerOrderDate = @BuyerOrderDate, DispatchDocNo = @DispatchDocNo, DispatchedThrough = @DispatchedThrough, BillOfLadingNo = @BillOfLadingNo, MotorVehicleNo = @MotorVehicleNo, TermsOfDelivery = @TermsOfDelivery, Status = 'APPROVED', ApprovedBy = @User, ApprovedOn = GETDATE() WHERE " + HeaderIdColumn() + " = @Id",
                        new SqlParameter("@FromBranchId", Convert.ToInt32(cmbcustomername.SelectedValue)),
                        new SqlParameter("@ToBranchId", Convert.ToInt32(cmbToLocation.SelectedValue)),
                        new SqlParameter("@ReferenceNo", GetReferenceValue()),
                        new SqlParameter("@EWayBillNo", DeliveryTextValue(txtEWayBillNo)),
                        new SqlParameter("@ModeTermsOfPayment", DeliveryTextValue(txtModeTermsOfPayment)),
                        new SqlParameter("@OtherReferences", DeliveryTextValue(txtOtherReferences)),
                        new SqlParameter("@BuyerOrderNo", DeliveryTextValue(txtBuyerOrderNo)),
                        new SqlParameter("@BuyerBillNo", DeliveryTextValue(txtBuyerBillNo)),
                        new SqlParameter("@DestinationStateName", DeliveryTextValue(txtDestinationStateName)),
                        new SqlParameter("@BuyerOrderDate", DeliveryDateValue(dtBuyerOrderDate)),
                        new SqlParameter("@DispatchDocNo", DeliveryTextValue(txtDispatchDocNo)),
                        new SqlParameter("@DispatchedThrough", DeliveryTextValue(txtDispatchedThrough)),
                        new SqlParameter("@BillOfLadingNo", DeliveryTextValue(txtBillOfLadingNo)),
                        new SqlParameter("@MotorVehicleNo", DeliveryTextValue(txtMotorVehicleNo)),
                        new SqlParameter("@TermsOfDelivery", DeliveryTextValue(txtTermsOfDelivery)),
                        new SqlParameter("@User", CurrentUser()),
                        new SqlParameter("@Id", currentDeliveryNoteId));
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        private void InsertDeliveryNoteDetailRows(SqlConnection con, SqlTransaction tran, int noteId)
        {
            foreach (DataGridViewRow row in dgvOrder.Rows)
            {
                if (row.IsNewRow)
                    continue;
                string productText = Convert.ToString(row.Cells["productid"].Value);
                string qtyText = Convert.ToString(row.Cells["Quantity"].Value);
                if (string.IsNullOrEmpty(productText) || string.IsNullOrEmpty(qtyText))
                    continue;

                using (SqlCommand cmd = new SqlCommand("INSERT INTO dbo." + DetailTable() + " (" + HeaderIdColumn() + ", MaterialId, Quantity, Remarks) VALUES (@Id, @MaterialId, @Quantity, NULL)", con, tran))
                {
                    cmd.Parameters.AddWithValue("@Id", noteId);
                    cmd.Parameters.AddWithValue("@MaterialId", Convert.ToInt32(productText));
                    cmd.Parameters.AddWithValue("@Quantity", Convert.ToDecimal(qtyText));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertMaterialTransactionRows(SqlConnection con, SqlTransaction tran, int noteId, string deliveryNoteNo)
        {
            string transactionTypeColumn = GetFirstColumn("MaterialTranscation", new string[] { "TransactionType", "TranscationType" });
            string transactionDateColumn = GetFirstColumn("MaterialTranscation", new string[] { "TransactionDate", "TranscationDate" });
            string materialIdColumn = GetFirstColumn("MaterialTranscation", new string[] { "MaterialId", "MaterailId" });

            if (string.IsNullOrEmpty(transactionTypeColumn) || string.IsNullOrEmpty(transactionDateColumn) || string.IsNullOrEmpty(materialIdColumn))
                throw new ApplicationException("MaterialTranscation table does not have the expected transaction/product columns.");

            foreach (DataGridViewRow row in dgvOrder.Rows)
            {
                if (row.IsNewRow)
                    continue;
                string productText = Convert.ToString(row.Cells["productid"].Value);
                string qtyText = Convert.ToString(row.Cells["Quantity"].Value);
                if (string.IsNullOrEmpty(productText) || string.IsNullOrEmpty(qtyText))
                    continue;

                string sql = @"INSERT INTO dbo.MaterialTranscation
(TransId, " + SqlName(transactionTypeColumn) + @", " + SqlName(transactionDateColumn) + @", " + SqlName(materialIdColumn) + @", Quantity, LocationId, Type, Updatedby)
VALUES (@TransId, @TransactionType, @Date, @MaterialId, @Quantity, @LocationId, @Type, @User)";
                using (SqlCommand cmd = new SqlCommand(sql, con, tran))
                {
                    cmd.Parameters.AddWithValue("@TransId", deliveryNoteNo);
                    cmd.Parameters.AddWithValue("@TransactionType", MaterialTransactionType());
                    cmd.Parameters.AddWithValue("@Date", date.Value.Date);
                    cmd.Parameters.AddWithValue("@MaterialId", Convert.ToInt32(productText));
                    cmd.Parameters.AddWithValue("@Quantity", Convert.ToDecimal(qtyText));
                    cmd.Parameters.AddWithValue("@LocationId", 6);
                    cmd.Parameters.AddWithValue("@Type", MaterialInOutType());
                    cmd.Parameters.AddWithValue("@User", Convert.ToInt32(Program.userid));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeleteMaterialTransactionRows(SqlConnection con, SqlTransaction tran, string noteNo)
        {
            string transactionTypeColumn = GetFirstColumn("MaterialTranscation", new string[] { "TransactionType", "TranscationType" });
            if (string.IsNullOrEmpty(transactionTypeColumn))
                throw new ApplicationException("MaterialTranscation table does not have a transaction type column.");

            string sql = "DELETE FROM dbo.MaterialTranscation WHERE " + SqlName(transactionTypeColumn) + " = @TransactionType AND TransId = @TransId";
            ExecuteDeliveryNonQuery(con, tran, sql,
                new SqlParameter("@TransactionType", MaterialTransactionType()),
                new SqlParameter("@TransId", noteNo));
        }

        private string GenerateDeliveryNoteNo(SqlConnection con, SqlTransaction tran, DateTime noteDate)
        {
            if (!receiptMode)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(CASE WHEN " + NoteNoColumn() + " NOT LIKE '%[^0-9]%' THEN CAST(" + NoteNoColumn() + " AS int) END), 0) + 1 FROM dbo." + HeaderTable() + " WITH (UPDLOCK, HOLDLOCK)", con, tran))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                }
            }

            string prefix = NotePrefix() + "/" + FinancialYear(noteDate) + "/";
            using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(RIGHT(" + NoteNoColumn() + ", 6) AS int)), 0) + 1 FROM dbo." + HeaderTable() + " WITH (UPDLOCK, HOLDLOCK) WHERE " + NoteNoColumn() + " LIKE @Prefix", con, tran))
            {
                cmd.Parameters.AddWithValue("@Prefix", prefix + "%");
                return prefix + Convert.ToInt32(cmd.ExecuteScalar()).ToString("000000");
            }
        }

        private static string FinancialYear(DateTime value)
        {
            int start = value.Month >= 4 ? value.Year : value.Year - 1;
            int end = start + 1;
            return (start % 100).ToString("00") + "-" + (end % 100).ToString("00");
        }

        private DataTable SearchDeliveryNotes(DateTime fromDate, DateTime toDate, string noteNo, int toBranchId)
        {
            string branchIdColumn = GetFirstColumn("Branches", new string[] { "id", "BranchId", "BranchID", "ID", "Id" });
            string branchNameColumn = GetFirstColumn("Branches", new string[] { "location_name", "BranchName", "Name", "Branch", "LocationName" });
            string sql = @"
SELECT dn." + HeaderIdColumn() + @" AS NoteId, dn." + NoteNoColumn() + @" AS NoteNo, dn." + NoteDateColumn() + @" AS NoteDate,
       ISNULL(CONVERT(varchar(150), fb." + SqlName(branchNameColumn) + @"), CONVERT(varchar(20), dn.FromBranchId)) AS FromBranch,
       ISNULL(CONVERT(varchar(150), tb." + SqlName(branchNameColumn) + @"), CONVERT(varchar(20), dn.ToBranchId)) AS ToBranch,
       ISNULL(dn.ReferenceNo, '') AS ReferenceNo,
       dn.Status
FROM dbo." + HeaderTable() + @" dn
LEFT JOIN dbo.Branches fb ON fb." + SqlName(branchIdColumn) + @" = dn.FromBranchId
LEFT JOIN dbo.Branches tb ON tb." + SqlName(branchIdColumn) + @" = dn.ToBranchId
WHERE ISNULL(dn.IsDeleted, 0) = 0
  AND dn." + NoteDateColumn() + @" >= @FromDate
  AND dn." + NoteDateColumn() + @" < @ToDate
  AND (@NoteNo = '' OR dn." + NoteNoColumn() + @" LIKE @NoteNoLike)
  AND (@ToBranchId = 0 OR dn.ToBranchId = @ToBranchId)
ORDER BY dn." + NoteDateColumn() + @" DESC, dn." + HeaderIdColumn() + @" DESC";
            return ExecuteDeliveryTable(sql,
                new SqlParameter("@FromDate", fromDate.Date),
                new SqlParameter("@ToDate", toDate.Date.AddDays(1)),
                new SqlParameter("@NoteNo", noteNo == null ? "" : noteNo.Trim()),
                new SqlParameter("@NoteNoLike", "%" + (noteNo == null ? "" : noteNo.Trim()) + "%"),
                new SqlParameter("@ToBranchId", toBranchId));
        }

        private void LoadDeliveryNote(int noteId)
        {
            DataTable header = ExecuteDeliveryTable("SELECT * FROM dbo." + HeaderTable() + " WHERE " + HeaderIdColumn() + " = @Id", new SqlParameter("@Id", noteId));
            if (header.Rows.Count == 0)
                return;
            currentDeliveryNoteId = noteId;
            txtorder.Text = Convert.ToString(header.Rows[0][NoteNoColumn()]);
            date.Value = Convert.ToDateTime(header.Rows[0][NoteDateColumn()]);
            cmbcustomername.SelectedValue = Convert.ToInt32(header.Rows[0]["FromBranchId"]);
            cmbToLocation.SelectedValue = Convert.ToInt32(header.Rows[0]["ToBranchId"]);
            cmbstatus.Text = Convert.ToString(header.Rows[0]["Status"]);
            if (txtReference != null && header.Columns.Contains("ReferenceNo"))
                txtReference.Text = Convert.ToString(header.Rows[0]["ReferenceNo"]);
            LoadDeliveryHeaderFields(header.Rows[0]);

            DataTable details = ExecuteDeliveryTable(@"
SELECT d.MaterialId, ISNULL(NULLIF(p.DisplayName, ''), p.ItemName) AS DisplayName, ISNULL(p.UOM, '') AS UOM, d.Quantity
FROM dbo." + DetailTable() + @" d
INNER JOIN dbo.ProductMaster p ON p.id = d.MaterialId
WHERE d." + HeaderIdColumn() + @" = @Id
ORDER BY d." + DetailIdColumn(), new SqlParameter("@Id", noteId));
            dgvOrder.Rows.Clear();
            for (int i = 0; i < details.Rows.Count; i++)
            {
                dgvOrder.Rows.Add();
                dgvOrder.Rows[i].Cells["S.NO"].Value = i + 1;
                dgvOrder.Rows[i].Cells["Items"].Value = Convert.ToString(details.Rows[i]["DisplayName"]);
                dgvOrder.Rows[i].Cells["UOM"].Value = Convert.ToString(details.Rows[i]["UOM"]);
                dgvOrder.Rows[i].Cells["productid"].Value = Convert.ToString(details.Rows[i]["MaterialId"]);
                dgvOrder.Rows[i].Cells["Quantity"].Value = Convert.ToString(details.Rows[i]["Quantity"]);
                dgvOrder.Rows[i].Cells["Rate"].Value = "0";
                dgvOrder.Rows[i].Cells["Amount"].Value = "0";
                dgvOrder.Rows[i].Cells["Types"].Value = "No";
            }

            if (!approvalMode)
            {
                pnenablefalse();
                btnSave.Enabled = false;
            }
            else
            {
                string status = Convert.ToString(header.Rows[0]["Status"]).ToUpper();
                if (status == "APPROVED")
                {
                    pnenablefalse();
                    btnSave.Text = "Approved";
                    btnSave.Enabled = false;
                }
                else
                {
                    pnenabletrue();
                    btnSave.Text = "Approve";
                    btnSave.Enabled = true;
                }
            }
            if (!receiptMode)
            {
                btnPrint.Enabled = true;
            }
        }

        private DataRow GetBranchPrintDetails(int branchId)
        {
            string idColumn = GetFirstColumn("Branches", new string[] { "id", "BranchId", "BranchID", "ID", "Id" });
            if (string.IsNullOrEmpty(idColumn))
                return null;

            DataTable table = ExecuteDeliveryTable("SELECT TOP 1 * FROM dbo.Branches WHERE " + SqlName(idColumn) + " = @BranchId",
                new SqlParameter("@BranchId", branchId));
            if (table.Rows.Count == 0)
                return null;
            return table.Rows[0];
        }

        private static string BranchValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
                return string.Empty;
            return Convert.ToString(row[columnName]).Trim();
        }

        private static string BranchDisplayName(DataRow row, ComboBox fallback)
        {
            string name = BranchValue(row, "location_name");
            if (string.IsNullOrEmpty(name))
                name = BranchValue(row, "BranchName");
            if (string.IsNullOrEmpty(name) && fallback != null)
                name = fallback.Text;
            return name;
        }

        private void LoadDeliveryHeaderFields(DataRow row)
        {
            SetTextBox(txtEWayBillNo, DeliveryText(row, "EWayBillNo"));
            SetTextBox(txtModeTermsOfPayment, DeliveryText(row, "ModeTermsOfPayment"));
            SetTextBox(txtOtherReferences, DeliveryText(row, "OtherReferences"));
            SetTextBox(txtBuyerOrderNo, DeliveryText(row, "BuyerOrderNo"));
            SetTextBox(txtBuyerBillNo, DeliveryText(row, "BuyerBillNo"));
            SetTextBox(txtDestinationStateName, DeliveryText(row, "DestinationStateName"));
            SetDatePicker(dtBuyerOrderDate, DeliveryDate(row, "BuyerOrderDate"));
            SetTextBox(txtDispatchDocNo, DeliveryText(row, "DispatchDocNo"));
            SetTextBox(txtDispatchedThrough, DeliveryText(row, "DispatchedThrough"));
            SetTextBox(txtBillOfLadingNo, DeliveryText(row, "BillOfLadingNo"));
            SetTextBox(txtMotorVehicleNo, DeliveryText(row, "MotorVehicleNo"));
            SetTextBox(txtTermsOfDelivery, DeliveryText(row, "TermsOfDelivery"));
        }

        private void ClearDeliveryHeaderFields()
        {
            SetTextBox(txtEWayBillNo, string.Empty);
            SetTextBox(txtModeTermsOfPayment, string.Empty);
            SetTextBox(txtOtherReferences, string.Empty);
            SetTextBox(txtBuyerOrderNo, string.Empty);
            SetTextBox(txtBuyerBillNo, string.Empty);
            SetTextBox(txtDestinationStateName, string.Empty);
            SetDatePicker(dtBuyerOrderDate, null);
            SetTextBox(txtDispatchDocNo, string.Empty);
            SetTextBox(txtDispatchedThrough, string.Empty);
            SetTextBox(txtBillOfLadingNo, string.Empty);
            SetTextBox(txtMotorVehicleNo, string.Empty);
            SetTextBox(txtTermsOfDelivery, string.Empty);
        }

        private void SetTextBox(TextBox textBox, string value)
        {
            if (textBox != null)
                textBox.Text = value;
        }

        private void SetDatePicker(DateTimePicker picker, DateTime? value)
        {
            if (picker == null)
                return;
            picker.Checked = value.HasValue;
            if (value.HasValue)
                picker.Value = value.Value;
        }

        private void RequireDeliveryText(TextBox textBox, string label, ref int count, ref string message)
        {
            if (textBox != null && string.IsNullOrEmpty(textBox.Text.Trim()))
            {
                count++;
                message = message + "* Please Enter " + label + "\n";
                if (count == 1)
                    this.ActiveControl = textBox;
            }
        }

        private DataTable ExecuteDeliveryTable(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        private object ExecuteDeliveryScalar(SqlConnection con, SqlTransaction tran, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = new SqlCommand(sql, con, tran))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }

        private void ExecuteDeliveryNonQuery(SqlConnection con, SqlTransaction tran, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = tran == null ? new SqlCommand(sql, con) : new SqlCommand(sql, con, tran))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
            }
        }

        private decimal GetAvailableStockByProductId(int productId)
        {
            DataTable stock = new DataTable();
            using (SqlConnection con = new SqlConnection(Conn))
            using (SqlCommand cmd = new SqlCommand("GetAvailableStockByProductId", con))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", productId);
                adapter.Fill(stock);
            }

            if (stock.Rows.Count == 0)
                return 0;

            if (stock.Columns.Contains("AvailableStock"))
                return Convert.ToDecimal(stock.Rows[0]["AvailableStock"]);

            if (stock.Columns.Count > 1)
                return Convert.ToDecimal(stock.Rows[0][1]);

            return Convert.ToDecimal(stock.Rows[0][0]);
        }

        private string GetFirstColumn(string tableName, string[] candidates)
        {
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                foreach (string candidate in candidates)
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM sys.columns WHERE object_id = OBJECT_ID(@TableName) AND name = @ColumnName", con))
                    {
                        cmd.Parameters.AddWithValue("@TableName", "dbo." + tableName);
                        cmd.Parameters.AddWithValue("@ColumnName", candidate);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            return candidate;
                    }
                }
            }
            return string.Empty;
        }

        private static string SqlName(string name)
        {
            return "[" + name.Replace("]", "]]") + "]";
        }

        private static string CurrentUser()
        {
            if (!string.IsNullOrEmpty(Program.UserName))
                return Program.UserName;
            if (!string.IsNullOrEmpty(Program.userid))
                return Program.userid;
            return Environment.UserName;
        }

        private void RemoveControl(Control control)
        {
            if (control == null)
            {
                return;
            }

            if (control.Parent != null)
            {
                control.Parent.Controls.Remove(control);
            }

            control.Visible = false;
            control.TabStop = false;
        }

        private void SafeResetCombo(ComboBox comboBox)
        {
            if (comboBox != null && comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }
        public DataTable bindEstimation()
        {
            DataTable dt = ExecuteDeliveryTable("SELECT " + NoteNoColumn() + " AS Quotationid FROM dbo." + HeaderTable() + " WHERE ISNULL(IsDeleted, 0) = 0 ORDER BY " + HeaderIdColumn() + " DESC");
            DataRow dr = dt.NewRow();
            dr["Quotationid"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);

            return dt;
        }

        private void EnsureFollowupDateStorage()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conn))
                using (SqlCommand cmd = new SqlCommand(@"
DECLARE @schema sysname;
DECLARE @sql nvarchar(max);

SELECT TOP 1 @schema = SCHEMA_NAME(schema_id)
FROM sys.tables
WHERE name = 'QuotationHeader';

IF @schema IS NOT NULL
   AND COL_LENGTH(QUOTENAME(@schema) + '.QuotationHeader', 'FollowupDate') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + QUOTENAME(@schema) + N'.QuotationHeader ADD FollowupDate datetime NULL';
    EXEC sp_executesql @sql;
END

IF @schema IS NOT NULL
   AND COL_LENGTH(QUOTENAME(@schema) + '.QuotationHeader', 'FollowupClosed') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + QUOTENAME(@schema) + N'.QuotationHeader ADD FollowupClosed bit NULL';
    EXEC sp_executesql @sql;
END

IF @schema IS NOT NULL
   AND COL_LENGTH(QUOTENAME(@schema) + '.QuotationHeader', 'FollowupPhone') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + QUOTENAME(@schema) + N'.QuotationHeader ADD FollowupPhone varchar(50) NULL';
    EXEC sp_executesql @sql;
END", con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    followupDateStorageReady = true;
                }
            }
            catch (Exception ex)
            {
                followupDateStorageReady = false;
                MessageBox.Show("Unable to initialize Follow Up date storage: " + ex.Message);
            }
        }

        private void SaveFollowupDate(string quotationId)
        {
            if (!followupDateStorageReady || string.IsNullOrEmpty(quotationId))
            {
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(Conn))
                using (SqlCommand cmd = new SqlCommand("UPDATE QuotationHeader SET FollowupDate = @FollowupDate, FollowupPhone = @FollowupPhone WHERE Quotationid = @Quotationid", con))
                {
                    cmd.Parameters.Add("@FollowupDate", SqlDbType.DateTime).Value = followupdate.Checked ? (object)followupdate.Value.Date : DBNull.Value;
                    cmd.Parameters.Add("@FollowupPhone", SqlDbType.VarChar, 50).Value = string.IsNullOrEmpty(txtFollowupPhone.Text.Trim()) ? (object)DBNull.Value : txtFollowupPhone.Text.Trim();
                    cmd.Parameters.Add("@Quotationid", SqlDbType.VarChar).Value = quotationId;
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save Follow Up date: " + ex.Message);
            }
        }

        private void LoadFollowupDate(string quotationId)
        {
            followupdate.Checked = false;
            followupdate.Value = DateTime.Today;
            txtFollowupPhone.Text = string.Empty;
            if (!followupDateStorageReady || string.IsNullOrEmpty(quotationId))
            {
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(Conn))
                using (SqlCommand cmd = new SqlCommand("SELECT FollowupDate, FollowupPhone FROM QuotationHeader WHERE Quotationid = @Quotationid", con))
                {
                    cmd.Parameters.Add("@Quotationid", SqlDbType.VarChar).Value = quotationId;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            object followupDate = reader["FollowupDate"];
                            if (followupDate != null && followupDate != DBNull.Value)
                            {
                                followupdate.Checked = true;
                                followupdate.Value = Convert.ToDateTime(followupDate);
                            }

                            if (reader["FollowupPhone"] != null && reader["FollowupPhone"] != DBNull.Value)
                            {
                                txtFollowupPhone.Text = Convert.ToString(reader["FollowupPhone"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load Follow Up date: " + ex.Message);
            }
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    e.Graphics.DrawLine(Pens.Yellow, 0, 0, 100, 100);
        //}
        public void Globeimage()
        {
            //string pathname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            ////string pathname = Path.Combine(Environment.CurrentDirectory);
            ////string a = pathname.Replace("\\bin\\Debug", "");
            ////string path = a + "\\Resources\\Light Globe.gif";
            //string path = pathname + "\\Loading.gif";
            //using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            //{
            //    var ms = new System.IO.MemoryStream();
            //    fs.CopyTo(ms);
            //    ms.Position = 0;                               // <=== here
            //    if (pcloading.Image != null) pcloading.Image.Dispose();
            //    pcloading.Image = Image.FromStream(ms);
            //    pcloading.SizeMode = PictureBoxSizeMode.Zoom;
            //}
        }

        public void bindcustomer()
        {
            DataTable location = GetBranches();
            int defaultBranchId = GetDefaultBranchId(location);

            DataTable fromLocation = location.Copy();
            if (receiptMode && defaultBranchId > 0)
            {
                RemoveBranch(fromLocation, defaultBranchId);
            }

            cmbcustomername.DataSource = fromLocation;
            cmbcustomername.DisplayMember = "BranchName";
            cmbcustomername.ValueMember = "BranchId";

            if (cmbToLocation != null)
            {
                DataTable toLocation = location.Copy();
                if (!receiptMode && defaultBranchId > 0)
                {
                    RemoveBranch(toLocation, defaultBranchId);
                }
                cmbToLocation.DataSource = toLocation;
                cmbToLocation.DisplayMember = "BranchName";
                cmbToLocation.ValueMember = "BranchId";
            }

            if (!receiptMode && defaultBranchId > 0)
                cmbcustomername.SelectedValue = defaultBranchId;
            if (receiptMode && cmbToLocation != null && defaultBranchId > 0)
                cmbToLocation.SelectedValue = defaultBranchId;
            ApplyDefaultBranchSelection();
        }

        private void RemoveBranch(DataTable table, int branchId)
        {
            for (int i = table.Rows.Count - 1; i >= 0; i--)
            {
                if (Convert.ToInt32(table.Rows[i]["BranchId"]) == branchId)
                    table.Rows.RemoveAt(i);
            }
        }

        private int GetDefaultBranchId(DataTable branches)
        {
            string branchCode = ConfigurationManager.AppSettings["BranchCode"];
            if (string.IsNullOrEmpty(branchCode))
                return 0;

            foreach (DataRow row in branches.Rows)
            {
                string branchName = Convert.ToString(row["BranchName"]);
                if (string.Compare(branchName, branchCode, true) == 0)
                    return Convert.ToInt32(row["BranchId"]);
            }

            foreach (DataRow row in branches.Rows)
            {
                string branchName = Convert.ToString(row["BranchName"]);
                if (branchName.IndexOf(branchCode, StringComparison.OrdinalIgnoreCase) >= 0)
                    return Convert.ToInt32(row["BranchId"]);
            }

            return 0;
        }

        private void ApplyDefaultBranchSelection()
        {
            if (!receiptMode)
            {
                if (cmbcustomername.DataSource is DataTable)
                {
                    int defaultBranchId = GetDefaultBranchId((DataTable)cmbcustomername.DataSource);
                    if (defaultBranchId > 0)
                        cmbcustomername.SelectedValue = defaultBranchId;
                }
                cmbcustomername.Enabled = false;
                cmbToLocation.Enabled = true;
                SafeResetCombo(cmbToLocation);
                return;
            }

            if (cmbToLocation != null && cmbToLocation.DataSource is DataTable)
            {
                int defaultBranchId = GetDefaultBranchId((DataTable)cmbToLocation.DataSource);
                if (defaultBranchId > 0)
                    cmbToLocation.SelectedValue = defaultBranchId;
            }
            cmbcustomername.Enabled = true;
            cmbToLocation.Enabled = false;
            SafeResetCombo(cmbcustomername);
        }

        public void bindLocation()
        {
            cmbloaction.DataSource = objQuotationbal.getLocation();
            cmbloaction.DisplayMember = "LocationName";
            cmbloaction.ValueMember = "LocationID";
        }

        public void bindreference()
        {
            cmbreference.DataSource = objQuotationbal.Getreference();
            cmbreference.DisplayMember = "Name";
            cmbreference.ValueMember = "ReferencesID";
        }

        public void bindAssist()
        {
            cmbassistby.DataSource = objQuotationbal.GetProductsusername();
            cmbassistby.DisplayMember = "Name";
            cmbassistby.ValueMember = "employeeid";
        }
        public void bindAssistName()
        {
            comboBox1.DataSource = objQuotationbal.GetProductsusername();
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "employeeid";
        }

        private void pbxCollapse_Click(object sender, EventArgs e)
        {

            if (pnlSearch.Visible == true)
            {
                pnlLabelSearch.Visible = true;
                vLabel1.Visible = true;
                pnlSearch.Visible = false;
                splitContainer1.Panel1Collapsed = true;

            }
        }
        private void RefScrollGrid()
        {
            if (DgvAutoRefNo.Rows.Count - 1 >= ProdSelRowvalue)
            {
                DgvAutoRefNo.FirstDisplayedScrollingRowIndex = ProdSelRowvalue;
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
                this.dgvSearch.Columns[1].Visible = false;
                this.dgvSearch.Columns[2].Visible = false;
                this.dgvSearch.Columns[3].Visible = false;

            }
        }
        private void LoadPorts()
        {
            dgvOrder.Rows.Clear();
            dgvOrder.ColumnCount =8;
            //dgvOrder.RowCount = 16;

            dgvOrder.Columns[0].Name = "S.NO";
            dgvOrder.Columns[1].Name = "Items";
            dgvOrder.Columns[2].Name = "UOM";
            dgvOrder.Columns[5].Name = "Quantity";
            dgvOrder.Columns[3].Name = "productid";
            dgvOrder.Columns[4].Name = "Rate";
            dgvOrder.Columns[6].Name = "Amount";
            dgvOrder.Columns[7].Name = "Types";
            dgvOrder.Columns[3].Visible = false;
            dgvOrder.Columns[7].Visible = false;
            this.dgvOrder.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvOrder.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvOrder.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgvOrder.Columns["S.NO"].ReadOnly = true;
            this.dgvOrder.Columns["Items"].ReadOnly = true;
            this.dgvOrder.Columns["UOM"].ReadOnly = true;
            this.dgvOrder.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrder.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvOrder.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
            dgvOrder.Columns[6].DefaultCellStyle.Format = "N2";

            this.dgvOrder.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvOrder.Columns["Rate"].ReadOnly = true;

            this.dgvOrder.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;




            this.dgvOrder.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            this.dgvOrder.Columns["Amount"].ReadOnly = true;




            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int w = resolution.Width;
            int h = resolution.Height;

            if (w == 1024 && h == 768)
            {
                this.dgvOrder.Columns[0].Width = 12;
                this.dgvOrder.Columns[1].Width = 100;
                this.dgvOrder.Columns[2].Width = 15;
                this.dgvOrder.Columns[4].Width = 15;
                this.dgvOrder.Columns[5].Width = 20;
                this.dgvOrder.Columns[6].Width = 100;
              
               

            }
            else
            {
                this.dgvOrder.Columns[0].Width = 12;
                this.dgvOrder.Columns[1].Width = 100;
                this.dgvOrder.Columns[2].Width = 15;
                this.dgvOrder.Columns[4].Width = 15;
                this.dgvOrder.Columns[5].Width = 15;
                this.dgvOrder.Columns[6].Width = 100;
               

            }

            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);



            foreach (DataGridViewColumn c in dgvOrder.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }

            dgvOrder.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvOrder.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvOrder.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }

        private void SearchCreteria1()
        {
            List<string> search = new List<string>();
            search.Add("To");
            search.Add("Date");
            search.Add("Delivery Note");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
           

        }

        private void SearchCreteria2()
        {
            List<string> search = new List<string>();
            search.Add("To");
            search.Add("Date");
            search.Add("Delivery Note");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
           
        }

        private void SearchCreteria3()
        {
            List<string> search = new List<string>();
            search.Add("To");
            search.Add("Date");
            search.Add("Delivery Note");
            BindingSource bs = new BindingSource();
            bs.DataSource = search;
          
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
                this.dgvSearch.Columns[1].Visible = true;
                this.dgvSearch.Columns[2].Visible = true;
                this.dgvSearch.Columns[3].Visible = false;

            }
        }

        private void DeliveryNote_Load(object sender, EventArgs e)
        {
            this.ActiveControl = cmbcustomername;
            clear();

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (dgvOrder.Focused)
            {
                try
                {
                    if (keyData == (Keys.Alt | Keys.Insert))
                    {

                        if (dgvOrder.Rows.Count <= 0)
                        {
                            dgvOrder.Rows.Add();
                        }
                        else
                        {
                            int rowindex = dgvOrder.CurrentRow.Index;
                            int colindex = dgvOrder.CurrentCell.ColumnIndex;
                            //dgvOrder.Rows.Insert(rowindex, dgvOrder.Rows.Add(1));
                            dgvOrder.Rows.Insert(rowindex, 1);

                            return true;
                        }
                        getsino();

                    }

                    if (keyData == (Keys.Alt | Keys.Delete))
                    {
                        DialogResult result = MessageBox.Show("Do you want to Delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            if (dgvOrder.Rows.Count > 0)
                            {
                                try
                                {
                                    int rowindex = dgvOrder.CurrentRow.Index;
                                    int colindex = dgvOrder.CurrentCell.ColumnIndex;
                                    dgvOrder.Rows.RemoveAt(rowindex);
                                }
                                catch
                                {
                                    if (dgvOrder.Rows.Count - 1 == dgvOrder.CurrentCell.RowIndex)
                                    {
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[0].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[2].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[3].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value = "";
                                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = "";

                                    }
                                }

                            }
                            pnsearch.Visible = false;
                            getsino();
                            return true;
                        }

                        if (dgvOrder.Rows.Count == 0)
                        {
                            dgvOrder.Rows.Add();
                        }
                       

                    }
                }
                catch
                {

                }

            }



            if (keyData == (Keys.Alt | Keys.S))
            {
                rdbStartsWith.Checked = true;
                return true;
            }
            if (keyData == (Keys.Alt | Keys.C))
            {
                rdbContains.Checked = true;
                return true;
            }

            //cmbstatus3
            if (cmbstatus3.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    txtSearchProduct.Focus();
                    return true;
                }
            }


            if (txtSearchProduct.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    textSearchQty.Focus();
                    return true;
                }
            }

            if (textSearchQty.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    btnmerge.Focus();
                    return true;
                }
            }
            if (btnmerge.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    btnSearch.Focus();
                    return true;
                }
            }
            if (btnSearch.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    cmbcustomername.Focus();
                    return true;
                }
            }

            if (cmbcustomername.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    cmbToLocation.Focus();
                    return true;
                }
            }
            if (cmbToLocation != null && cmbToLocation.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    dgvOrder.Focus();
                    if (dgvOrder.Rows.Count == 0)
                    {
                        dgvOrder.Rows.Add();
                    }
                    dgvOrder.CurrentCell = dgvOrder.Rows[0].Cells["Items"];
                    return true;
                }
            }

            if (keyData == Keys.F3)
            {
                pnlprodsearch.Visible = true;
                txtprodsearch.SelectionStart = 0;
                txtprodsearch.SelectionLength = txtprodsearch.Text.Length;
                txtprodsearch.Text = "";
                txtprodsearch.Focus();
                return true;
            }
            if (txtprodsearch.Focused)
            {
                if (keyData == (Keys.Enter))
                {
                    //if (!string.IsNullOrEmpty(txtprodsearch.Text))
                    //{
                        GetSuppliersearch();
                        pnlprodsearch.Visible = false;
                    //}
                }
            }
            if (cmbassistby.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    dgvOrder.Focus();
                    if (dgvOrder.Rows.Count == 0)
                    {
                        dgvOrder.Rows.Add();
                    }
                    dgvOrder.CurrentCell = dgvOrder[1, 0];
                    return true;
                }
            }
            if (keyData == Keys.Escape)
            {
                if (pnsearch.Visible)
                {
                    pnsearch.Visible = false;
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];
                }
                else
                {
                    if (dgvOrder.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[0].Cells[1].Value)))
                    {
                        DialogResult result = MessageBox.Show("Do you want to Exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            this.Close();
                        }
                    }
                    else
                    {
                        this.Close();
                    }
                }
                return true;
            }
            if (cmbreference.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = cmbassistby;
                    return true;
                }
            }
            //if (txtorder.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = date;
            //        return true;
            //    }

            //}

            if (date.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = dgvOrder;
                    if (dgvOrder.Rows.Count == 0)
                    {
                        dgvOrder.Rows.Add();
                    }
                    return true;
                }

            }

            if (cmbloaction.Focused)
            {
                if (keyData == (Keys.Tab))
                {
                    this.ActiveControl = Txtitem;
                    return true;
                }

            }
            try
            {
                if (keyData == Keys.Tab)
                {
                    if (dgvOrder.CurrentCell.ColumnIndex == 5)
                    {
                        dgvOrder.Focus();
                        //edit = true;
                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];
                    }
                }
            }
            catch
            {

            }

            //if (btnPrint.Focused)
            //{
            //    if (keyData == (Keys.Tab))
            //    {
            //        this.ActiveControl = cmbcustomername;
            //        return true;
            //    }
            //}
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void GetSuppliersearch()
        {
            DataTable dt = objQuotationbal.GetCustomerNamesearch(txtprodsearch.Text);
            dgvSearch.Rows.Clear();
            DataTable SearchResult = new DataTable();
            if (dt.Rows.Count > 0)
            {
                try
                {
                    if (textSearchQty.Text == "")
                        SearchResult = dt.Select("Products like '%" + txtSearchProduct.Text + "%'").CopyToDataTable();

                    else
                        SearchResult = dt.Select("Products like '%" + txtSearchProduct.Text + "%' and QTY like '%|" + textSearchQty.Text + "|%'").CopyToDataTable();
                }
                catch (Exception ex)
                {
                    SearchResult = new DataTable();
                }

            }

            lblItemCount.Text = SearchResult.Rows.Count.ToString();


            try
            {
                AlphanumComparator<string> comparer = new AlphanumComparator<string>();
                //DataTable dtNew = dv.Table;
                DataTable dtNew = SearchResult.AsEnumerable().OrderBy(x => x.Field<string>("Quotationid"), comparer).CopyToDataTable();
                //dtNew.TableName = "NaturalSort";
                SearchResult = dtNew;
            }
            catch (Exception ex)
            {

            }


            int SearchResultCount = 0;
            for (int i = 0; i < SearchResult.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(SearchResult.Rows[i]["Quotationid"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(SearchResult.Rows[i]["Customer"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(SearchResult.Rows[i]["Reference"]);
                SearchResultCount++;
            }



        }
        private void SearchPurchaseOrder()
        {
            dgvSearch.Rows.Clear();
            dgvSearch.Columns.Clear();
            dgvSearch.ColumnCount = 4;
            //dgvSearch.RowCount = 16;

            dgvSearch.Columns[0].Name = NoteTitle();
            dgvSearch.Columns[1].Name = "To";
            dgvSearch.Columns[2].Name = "Status";
            dgvSearch.Columns[3].Name = "NoteId";


            this.dgvSearch.Columns[0].Width = 120;
            this.dgvSearch.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvSearch.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvSearch.Columns[1].Width = 120;
            //this.dgvSearch.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvSearch.Columns[2].Width = 70;
            this.dgvSearch.Columns[3].Visible = false;
            dgvSearch.Columns[0].ReadOnly = true;
            dgvSearch.Columns[1].ReadOnly = true;
            dgvSearch.Columns[2].ReadOnly = true;

            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9.1F, FontStyle.Bold);
            dgvSearch.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn c in dgvSearch.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            }
            dgvSearch.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.1F, FontStyle.Bold);
            dgvSearch.DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvSearch.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

        }

        public void AutoCompleteLoad(string s, int t)
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            DataTable st = objQuotationbal.itemauto(s, t);

            if (st.Rows.Count > 0)
            {
                DgvAutoRefNo.Visible = true;
                DgvAutoRefNo.DataSource = st;
                res = false;
                cas = Convert.ToString(DgvAutoRefNo.Rows[0].Cells[0].Value);

                DgvAutoRefNo.Focus();
                DgvAutoRefNo.CurrentCell = DgvAutoRefNo[0, 0];
                DgvAutoRefNo.Rows[0].Cells[0].Selected = true;
                string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                getitems(sa);
            }
            else
            {
                DgvAutoRefNo.Visible = false;
                lblproductid.Text = string.Empty;
                //Txtitem.Text = string.Empty;
                lblitemcode.Text = "0";
                lblrack.Text = "0";
                lbldisplay.Text = "0";
                lbldemo.Text = "0";
                lblservice.Text = "0";
                lbldamage.Text = "0";
                lblprice.Text = "0";
                Locationpanal.Controls.Clear();
            }


            //string[] arr = new string[st.Rows.Count];
            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    arr[i] = st.Rows[i]["DisplayName"].ToString();
            //}
            //for (int i = 0; i < arr.Length; i++)
            //{
            //    //var combined = string.Join(", ", arr);
            //    var combined = arr[i];
            //    str.Add(combined);
            //}

            //Txtitem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //Txtitem.AutoCompleteCustomSource = str;
            //Txtitem.AutoCompleteSource = AutoCompleteSource.CustomSource;

            //for (int i = 0; i < arr.Length; i++)
            //{
            //  var combined = string.Join(", ", arr);
            //var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //str.Add(combined);
            //}

            //for (int i = 0; i < st.Rows.Count; i++)
            //{
            //    var combined = string.Join(", ", st.Rows[i]["DisplayName"]);
            //    str.Add(combined);
            //}


        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool val = Validation();
            if (val)
            {
                if (approvalMode)
                {
                    DialogResult result = MessageBox.Show("Do you want to approve this " + NoteTitle().ToLower() + "?", NoteTitle() + " Approval", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            ApproveDeliveryNote();
                            MessageBox.Show(NoteTitle() + " approved successfully.");
                            btnSave.Enabled = false;
                            GetSearchOrder();
                            clear();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("Do you want to save this " + NoteTitle().ToLower() + "?", NoteTitle(), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        save(1);
                    }
                }

            }
        }

        private void btnSavePending_Click(object sender, EventArgs e)
        {
            bool val = Validation();
            if (val)
            {
                save(1);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }

        private bool Validation()
        {
            StockCheck = new DataTable();
            DataColumn ItemsLessStock = new DataColumn("ItemsLessStock", typeof(string));
            DataColumn Avalavbe = new DataColumn("Avalavbe", typeof(string));
            DataColumn Order = new DataColumn("Order", typeof(string));
            DataColumn Need = new DataColumn("Need", typeof(string));
            StockCheck.Columns.Add(ItemsLessStock);
            StockCheck.Columns.Add(Avalavbe);
            StockCheck.Columns.Add(Order);
            StockCheck.Columns.Add(Need);
            bool status = true;
            string message = "";
            int i = 0;

            if (cmbcustomername.SelectedIndex <= 0 || cmbcustomername.Text == "--Select--")
            {
                i++;
                message = message + "* Please Select From Location" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbcustomername;
            }

            if (cmbToLocation == null || cmbToLocation.SelectedIndex <= 0 || cmbToLocation.Text == "--Select--")
            {
                i++;
                message = message + "* Please Select To Location" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbToLocation;
            }

            if (cmbToLocation != null &&
                cmbcustomername.SelectedIndex > 0 &&
                cmbToLocation.SelectedIndex > 0 &&
                Convert.ToString(cmbcustomername.SelectedValue) == Convert.ToString(cmbToLocation.SelectedValue))
            {
                i++;
                message = message + "* From Location and To Location should be different" + "\n";
                if (i == 1)
                    this.ActiveControl = cmbToLocation;
            }

            if (!receiptMode)
            {
                RequireDeliveryText(txtEWayBillNo, "e-Way Bill No", ref i, ref message);
                RequireDeliveryText(txtModeTermsOfPayment, "Mode/Terms of Payment", ref i, ref message);
                RequireDeliveryText(txtOtherReferences, "Other References", ref i, ref message);
                RequireDeliveryText(txtBuyerOrderNo, "Buyer Order No", ref i, ref message);
                RequireDeliveryText(txtBuyerBillNo, "Buyer Bill No", ref i, ref message);
                RequireDeliveryText(txtDestinationStateName, "State Name", ref i, ref message);
                if (dtBuyerOrderDate == null || !dtBuyerOrderDate.Checked)
                {
                    i++;
                    message = message + "* Please Enter Buyer Order Date" + "\n";
                    if (i == 1)
                        this.ActiveControl = dtBuyerOrderDate;
                }
                RequireDeliveryText(txtDispatchDocNo, "Dispatch Doc No", ref i, ref message);
                RequireDeliveryText(txtDispatchedThrough, "Dispatched Through", ref i, ref message);
                RequireDeliveryText(txtBillOfLadingNo, "LR-RR No", ref i, ref message);
                RequireDeliveryText(txtMotorVehicleNo, "Motor Vehicle No", ref i, ref message);
                RequireDeliveryText(txtReference, "Reference No/Date", ref i, ref message);
                RequireDeliveryText(txtTermsOfDelivery, "Terms of Delivery", ref i, ref message);
            }




            //if (string.IsNullOrEmpty(cmdcity.Text))
            //{
            //    i++;
            //    message = message + "* Please Select city" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmdcity;
            //}

            //if (comboBox1.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please Select Assist By Type " + "\n";
            //    if (i == 1)
            //        this.ActiveControl = comboBox1;
            //}
           


            //if (cmbreference.SelectedIndex == 0)
            //{
            //    i++;
            //    message = message + "* Please select Reference" + "\n";
            //    if (i == 1)
            //        this.ActiveControl = cmbreference;
            //}
          

            if (dgvOrder.Rows.Count > 0)
            {
                i++;
                string Items = Convert.ToString(dgvOrder.Rows[0].Cells["Quantity"].Value);
                string Received = Convert.ToString(dgvOrder.Rows[0].Cells["Items"].Value);

                


               

                if ((string.IsNullOrEmpty(Items) && string.IsNullOrEmpty(Received)))
                {
                    message = message + "* Please Select Product" + "\n";
                }

                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }
            else if (dgvOrder.Rows.Count == 0)
            {
                i++;
                message = message + "* Please Select Product" + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }

           
            bool sas = false;
            bool LightProduct = false;
            bool NormalProduct = false;
            for (int k = 0; k < dgvOrder.RowCount; k++)
            {
                string Items = Convert.ToString(dgvOrder.Rows[k].Cells["Quantity"].Value);

                string Received = Convert.ToString(dgvOrder.Rows[k].Cells["Items"].Value);
                string productText = Convert.ToString(dgvOrder.Rows[k].Cells["productid"].Value);
                string rate = Convert.ToString(dgvOrder.Rows[k].Cells["Rate"].Value);


                string Types = Convert.ToString(dgvOrder.Rows[k].Cells["Types"].Value);


                if (Types == "Yes")
                {
                    LightProduct = true;
                   

                }
                else if (Types == "No")
                {
                    NormalProduct = true;

                  
                }

                if ((!string.IsNullOrEmpty(Items) && (string.IsNullOrEmpty(Received))) || (string.IsNullOrEmpty(Items) && (!string.IsNullOrEmpty(Received))) || Items == "." || Items == "-" || Items == ".-" || Items == "-." || Items == "0" || rate == ".")
                {
                    sas = true;
                    break;
                }
                else
                {
                    if (!receiptMode && !string.IsNullOrEmpty(Received))
                    {
                        int productId = 0;
                        if (!int.TryParse(productText, out productId) || productId <= 0)
                        {
                            sas = true;
                            break;
                        }

                        decimal requestedQty = Convert.ToDecimal(Items);
                        decimal availableStock = GetAvailableStockByProductId(productId);

                        if (requestedQty > availableStock)
                        {
                            decimal diff = requestedQty - availableStock;
                            StockCheck.Rows.Add(Received, availableStock, requestedQty, diff);
                        }
                    }







                }

            }


            if (sas == true)
            {
                i++;
                message = message + "* Product or Rate or Quantity should not be empty." + "\n";
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }

            if (!receiptMode && StockCheck.Rows.Count > 0)
            {
                i++;
                StringBuilder stockMessage = new StringBuilder();
                foreach (DataRow row in StockCheck.Rows)
                {
                    stockMessage.Append("* Insufficient stock for ");
                    stockMessage.Append(Convert.ToString(row["ItemsLessStock"]));
                    stockMessage.Append(". Available: ");
                    stockMessage.Append(Convert.ToString(row["Avalavbe"]));
                    stockMessage.Append(", Requested: ");
                    stockMessage.Append(Convert.ToString(row["Order"]));
                    stockMessage.Append(", Need to add: ");
                    stockMessage.Append(Convert.ToString(row["Need"]));
                    stockMessage.Append("\n");
                }
                message = message + stockMessage.ToString();
                if (i == 1)
                    this.ActiveControl = dgvOrder;
            }



            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show("* Mantatory Fields" + "\n" + "----------------------------------------" + "\n" + message);
                status = false;
            }
            return status;
        }

        private void txtCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;

        }

        private void txtContactNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void clear()
        {
            foreach (DataGridViewRow dr in dgvSearch.Rows)
            {
                dr.Cells[3].Value = false;//sifirin
            }
            btnmerge.Visible = true;
            label27.Text = "";
            btnmerge.Visible = true;
            this.dgvSearch.Columns[3].Visible = false;
            btnLess.Enabled = false;
            pnsearch.Visible = false;
            btnSavePending.Enabled = true;
            btnSave.Enabled = true;
            btnPrint.Enabled = false;
            currentDeliveryNoteId = 0;
            ApplyDefaultBranchSelection();
            cmdcity.Text = string.Empty;
            if (txtReference != null)
                txtReference.Text = string.Empty;
            ClearDeliveryHeaderFields();
            SafeResetCombo(cmbassistby);
            SafeResetCombo(cmbreference);
            SafeResetCombo(comboBox1);
            txtorder.Clear();
            followupdate.Checked = false;
            followupdate.Value = DateTime.Today;
            txtFollowupPhone.Text = string.Empty;
            dgvOrder.Rows.Clear();
            lblperare.Text = Program.Userfullname;
            lbltotalquantity.Text = "0";
            lbltotalamount.Text = "0";
            cmbcustomername.Focus();
            SafeResetCombo(cmbloaction);
            cmbstatus.Text = "Open";
            pnenabletrue();
            var cntls = GetAll(this, typeof(RadioButton));
            foreach (Control cntrl in cntls)
            {
                RadioButton _rb = (RadioButton)cntrl;
                if (_rb.Text != "New")
                {
                    if (_rb.Checked)
                    {
                        _rb.Checked = false;
                    }
                }
                else
                {
                    _rb.Checked = true;
                }
            }
            this.dgvOrder.Columns["Rate"].ReadOnly = true;
            btnSearch.PerformClick();
        }


        private void btnNew_Click(object sender, EventArgs e)
        {
            clear();
            this.dgvOrder.Columns["Rate"].ReadOnly = true;
            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns["Items"].ReadOnly = true;
            this.dgvOrder.Columns["UOM"].ReadOnly = true;
            this.dgvOrder.Columns["Amount"].ReadOnly = true;

        }

        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrls => GetAll(ctrls, type)).Concat(controls).Where(c => c.GetType() == type);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            preview();
        }

        public void preview()
        {
            try
            {
                if (receiptMode)
                {
                    MessageBox.Show("Print option is available for Delivery Note only.");
                    return;
                }

                if (currentDeliveryNoteId <= 0 || string.IsNullOrEmpty(txtorder.Text))
                {
                    MessageBox.Show("Please select a Delivery Note to print.");
                    return;
                }

                PrintDocument document = new PrintDocument();
                document.DocumentName = txtorder.Text;
                document.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);
                document.PrintPage += new PrintPageEventHandler(DeliveryNote_PrintPage);

                PrintPreviewDialog previewDialog = new PrintPreviewDialog();
                previewDialog.Document = document;
                previewDialog.WindowState = FormWindowState.Maximized;
                previewDialog.Text = "Delivery Note Print";
                previewDialog.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeliveryNote_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = e.MarginBounds;
            int y = bounds.Top;
            int left = bounds.Left;
            int right = bounds.Right;

            DataRow fromBranch = GetBranchPrintDetails(GetSelectedBranchId(cmbcustomername));
            DataRow toBranch = GetBranchPrintDetails(GetSelectedBranchId(cmbToLocation));

            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font duplicateFont = new Font("Arial", 9, FontStyle.Italic);
            Font normalFont = new Font("Arial", 9);
            Pen linePen = Pens.Black;

            string companyName = BranchValue(fromBranch, "AddressLine1");
            if (string.IsNullOrEmpty(companyName))
                companyName = BranchDisplayName(fromBranch, cmbcustomername);

            DrawCentered(g, "DELIVERY NOTE", titleFont, left, y, bounds.Width);
            string copyText = "(DUPLICATE FOR TRANSPORTER)";
            SizeF copySize = g.MeasureString(copyText, duplicateFont);
            g.DrawString(copyText, duplicateFont, Brushes.Black, right - copySize.Width, y + 4);
            y += 28;
            g.DrawLine(linePen, left, y, right, y);
            y += 12;

            int bodyTop = y;
            int customerLeft = left;
            int detailsLeft = left + (bounds.Width / 2);
            int detailsWidth = bounds.Width - (bounds.Width / 2);
            int customerWidth = detailsLeft - left - 12;

            g.DrawString("From", headerFont, Brushes.Black, left, y);
            y += 18;
            g.DrawString(companyName, headerFont, Brushes.Black, left, y);
            y += 26;
            y = DrawBranchAddress(g, fromBranch, normalFont, left, y, true);
            g.DrawLine(linePen, left, y, left + customerWidth, y);
            y += 16;

            g.DrawString("To", headerFont, Brushes.Black, customerLeft, y);
            y += 18;
            string buyerBillNo = DeliveryControlText(txtBuyerBillNo);
            if (!string.IsNullOrEmpty(buyerBillNo))
            {
                g.DrawString("Buyer Bill No : " + buyerBillNo, normalFont, Brushes.Black, customerLeft, y);
                y += 16;
            }
            string toName = BranchDisplayName(toBranch, cmbToLocation);
            if (!string.IsNullOrEmpty(toName))
            {
                g.DrawString(toName, headerFont, Brushes.Black, customerLeft, y);
                y += 18;
            }
            int customerBottom = DrawBranchAddress(g, toBranch, normalFont, customerLeft, y, false);
            string stateName = DeliveryControlText(txtDestinationStateName);
            if (!string.IsNullOrEmpty(stateName))
            {
                g.DrawString("State Name : " + stateName, normalFont, Brushes.Black, customerLeft, customerBottom);
                customerBottom += 16;
            }
            int detailsBottom = DrawDeliveryParameters(g, new Rectangle(detailsLeft, bodyTop, detailsWidth, bounds.Height), bodyTop, headerFont, normalFont);

            y = Math.Max(customerBottom, detailsBottom) + 12;
            y = DrawDeliveryProductGrid(g, bounds, y, headerFont, normalFont);

            int footerY = bounds.Bottom - 70;
            g.DrawLine(linePen, left, footerY, right, footerY);
            footerY += 14;
            g.DrawString("Prepared By", normalFont, Brushes.Black, left, footerY);
            g.DrawString(lblperare.Text, headerFont, Brushes.Black, left, footerY + 18);
            g.DrawString("Received By", normalFont, Brushes.Black, left + 260, footerY);
            g.DrawString("Authorized Signatory", normalFont, Brushes.Black, right - 160, footerY);

            e.HasMorePages = false;
        }

        private void DrawCentered(Graphics g, string text, Font font, int x, int y, int width)
        {
            SizeF size = g.MeasureString(text, font);
            g.DrawString(text, font, Brushes.Black, x + ((width - size.Width) / 2), y);
        }

        private int DrawBranchAddress(Graphics g, DataRow branch, Font font, int x, int y, bool skipAddressLine1)
        {
            string[] columns = skipAddressLine1
                ? new string[] { "AddressLine2", "AddressLine3", "AddressLine4", "City", "Pincode" }
                : new string[] { "AddressLine1", "AddressLine2", "AddressLine3", "AddressLine4", "City", "Pincode" };

            for (int i = 0; i < columns.Length; i++)
            {
                string value = BranchValue(branch, columns[i]);
                if (!string.IsNullOrEmpty(value))
                {
                    g.DrawString(value, font, Brushes.Black, x, y);
                    y += 16;
                }
            }

            string gst = FirstBranchValue(branch, new string[] { "GSTIN", "GSTNo", "GST", "Tin", "TinNumber", "CustomerTin" });
            if (!string.IsNullOrEmpty(gst))
            {
                g.DrawString("GSTIN/UIN : " + gst, font, Brushes.Black, x, y);
                y += 16;
            }
            return y;
        }

        private string FirstBranchValue(DataRow branch, string[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                string value = BranchValue(branch, columns[i]);
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return string.Empty;
        }

        private void DrawLabelValue(Graphics g, string label, string value, Font font, int x, int y)
        {
            g.DrawString(label + " :", font, Brushes.Black, x, y);
            g.DrawString(value, font, Brushes.Black, x + 55, y);
        }

        private string DeliveryControlText(TextBox textBox)
        {
            return textBox == null ? string.Empty : textBox.Text.Trim();
        }

        private string DeliveryControlDate(DateTimePicker picker)
        {
            if (picker == null || !picker.Checked)
                return string.Empty;
            return picker.Value.ToString("dd-MM-yyyy");
        }

        private int DrawDeliveryParameters(Graphics g, Rectangle bounds, int y, Font headerFont, Font normalFont)
        {
            int left = bounds.Left;
            int half = bounds.Width / 2;

            y = DrawThreeColumnParameterRow(g, left, y, bounds.Width, normalFont,
                "Delivery Note No", txtorder.Text,
                "e-Way Bill.no", DeliveryControlText(txtEWayBillNo),
                "Dated", date.Value.ToString("dd-MM-yyyy"));

            y = DrawCompactLabelValueRow(g, left, y, half, bounds.Width - half, normalFont,
                "Mode/Terms Of Payment", DeliveryControlText(txtModeTermsOfPayment));

            y = DrawTwoColumnParameterRow(g, left, y, half, bounds.Width - half, normalFont,
                "Reference No & Date", DeliveryControlText(txtReference),
                "Other Reference", DeliveryControlText(txtOtherReferences));

            y = DrawCompactBuyerOrderRow(g, left, y, half, bounds.Width - half, normalFont,
                "Buyer's Order No / Dated", DeliveryControlText(txtBuyerOrderNo), DeliveryControlDate(dtBuyerOrderDate));

            y = DrawCompactLabelValueRow(g, left, y, half, bounds.Width - half, normalFont,
                "Despatch Doc No", DeliveryControlText(txtDispatchDocNo));

            y = DrawTwoColumnParameterRow(g, left, y, half, bounds.Width - half, normalFont,
                "Despatched Through", DeliveryControlText(txtDispatchedThrough),
                "Destination", cmbToLocation.Text);

            y = DrawTwoColumnParameterRow(g, left, y, half, bounds.Width - half, normalFont,
                "Bill of Landing/LR-RR No", DeliveryControlText(txtBillOfLadingNo),
                "Motor Vehicle No", DeliveryControlText(txtMotorVehicleNo));

            y = DrawMergedParameterRow(g, left, y, bounds.Width, normalFont,
                "Terms of Delivery", DeliveryControlText(txtTermsOfDelivery));
            return y;
        }

        private int DrawThreeColumnParameterRow(Graphics g, int x, int y, int width, Font font, string label1, string value1, string label2, string value2, string label3, string value3)
        {
            int firstWidth = width / 3;
            int secondWidth = width / 3;
            int thirdWidth = width - firstWidth - secondWidth;
            int height = Math.Max(28, Math.Max(
                MeasureParameterCellHeight(g, font, firstWidth, label1, value1),
                Math.Max(
                    MeasureParameterCellHeight(g, font, secondWidth, label2, value2),
                    MeasureParameterCellHeight(g, font, thirdWidth, label3, value3))));
            DrawParameterCell(g, x, y, firstWidth, height, font, label1, value1);
            DrawParameterCell(g, x + firstWidth, y, secondWidth, height, font, label2, value2);
            DrawParameterCell(g, x + firstWidth + secondWidth, y, thirdWidth, height, font, label3, value3);
            return y + height;
        }

        private int DrawTwoColumnParameterRow(Graphics g, int x, int y, int leftWidth, int rightWidth, Font font, string leftLabel, string leftValue, string rightLabel, string rightValue)
        {
            int height = Math.Max(28, Math.Max(
                MeasureParameterCellHeight(g, font, leftWidth, leftLabel, leftValue),
                MeasureParameterCellHeight(g, font, rightWidth, rightLabel, rightValue)));
            DrawParameterCell(g, x, y, leftWidth, height, font, leftLabel, leftValue);
            DrawParameterCell(g, x + leftWidth, y, rightWidth, height, font, rightLabel, rightValue);
            return y + height;
        }

        private int DrawCompactLabelValueRow(Graphics g, int x, int y, int leftWidth, int rightWidth, Font font, string label, string value)
        {
            int height = Math.Max(18, Math.Max(
                MeasureTopAlignedCellHeight(g, font, leftWidth, label),
                MeasureTopAlignedCellHeight(g, font, rightWidth, value)));
            DrawTopAlignedCell(g, x, y, leftWidth, height, font, label);
            DrawTopAlignedCell(g, x + leftWidth, y, rightWidth, height, font, value);
            return y + height;
        }

        private int DrawCompactBuyerOrderRow(Graphics g, int x, int y, int leftWidth, int rightWidth, Font font, string label, string orderNo, string orderDate)
        {
            int orderWidth = rightWidth / 2;
            int height = Math.Max(18, Math.Max(
                MeasureTopAlignedCellHeight(g, font, leftWidth, label),
                Math.Max(
                    MeasureTopAlignedCellHeight(g, font, orderWidth, orderNo),
                    MeasureTopAlignedCellHeight(g, font, rightWidth - orderWidth, orderDate))));
            DrawTopAlignedCell(g, x, y, leftWidth, height, font, label);
            g.DrawRectangle(Pens.Black, x + leftWidth, y, rightWidth, height);
            DrawTopAlignedText(g, orderNo, font, x + leftWidth, y, orderWidth, height);
            DrawTopAlignedText(g, orderDate, font, x + leftWidth + orderWidth, y, rightWidth - orderWidth, height);
            return y + height;
        }

        private int DrawMergedParameterRow(Graphics g, int x, int y, int width, Font font, string label, string value)
        {
            int height = Math.Max(28, MeasureParameterCellHeight(g, font, width, label, value));
            DrawParameterCell(g, x, y, width, height, font, label, value);
            return y + height;
        }

        private void DrawParameterCell(Graphics g, int x, int y, int width, int height, Font font, string label, string value)
        {
            g.DrawRectangle(Pens.Black, x, y, width, height);
            float textY = y + 2;
            if (!string.IsNullOrEmpty(label))
            {
                int labelHeight = MeasureWrappedTextHeight(g, font, width, label);
                g.DrawString(label, font, Brushes.Black, new RectangleF(x + 3, textY, width - 6, labelHeight));
                textY += labelHeight + 1;
            }
            if (!string.IsNullOrEmpty(value))
            {
                float valueHeight = Math.Max(0, y + height - textY - 2);
                g.DrawString(value, font, Brushes.Black, new RectangleF(x + 3, textY, width - 6, valueHeight));
            }
        }

        private void DrawTopAlignedCell(Graphics g, int x, int y, int width, int height, Font font, string text)
        {
            g.DrawRectangle(Pens.Black, x, y, width, height);
            DrawTopAlignedText(g, text, font, x, y, width, height);
        }

        private void DrawTopAlignedText(Graphics g, string text, Font font, int x, int y, int width, int height)
        {
            if (!string.IsNullOrEmpty(text))
                g.DrawString(text, font, Brushes.Black, new RectangleF(x + 3, y + 2, width - 6, height - 4));
        }

        private int MeasureParameterCellHeight(Graphics g, Font font, int width, string label, string value)
        {
            int height = 4;
            if (!string.IsNullOrEmpty(label))
                height += MeasureWrappedTextHeight(g, font, width, label) + 1;
            if (!string.IsNullOrEmpty(value))
                height += MeasureWrappedTextHeight(g, font, width, value) + 1;
            return height + 2;
        }

        private int MeasureTopAlignedCellHeight(Graphics g, Font font, int width, string text)
        {
            if (string.IsNullOrEmpty(text))
                return 18;
            return MeasureWrappedTextHeight(g, font, width, text) + 4;
        }

        private int MeasureWrappedTextHeight(Graphics g, Font font, int width, string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            int textWidth = Math.Max(1, width - 6);
            return (int)Math.Ceiling(g.MeasureString(text, font, textWidth).Height);
        }

        private void DrawPrintPair(Graphics g, string label, string value, Font font, int x, int y, int width)
        {
            string text = label + " : " + value;
            g.DrawString(text, font, Brushes.Black, new RectangleF(x, y, width, 18));
        }

        private string ProductPriceIncludingTax(DataGridViewRow row)
        {
            string amount = Convert.ToString(row.Cells["Amount"].Value);
            if (!string.IsNullOrEmpty(amount) && amount != "0" && amount != "0.00")
                return FormatPrintAmount(amount);

            string rate = Convert.ToString(row.Cells["Rate"].Value);
            return FormatPrintAmount(rate);
        }

        private string FormatPrintAmount(string value)
        {
            decimal amount;
            if (decimal.TryParse(value, out amount))
                return amount.ToString("0.00");
            return value;
        }

        private int DrawDeliveryProductGrid(Graphics g, Rectangle bounds, int y, Font headerFont, Font normalFont)
        {
            int[] widths = new int[] { 45, 280, 70, 80, 100 };
            int rowHeight = 24;
            int x = bounds.Left;
            string[] headers = new string[] { "S.No", "Items", "UOM", "Quantity", "Price Incl. Tax" };

            for (int i = 0; i < headers.Length; i++)
            {
                g.FillRectangle(Brushes.Gainsboro, x, y, widths[i], rowHeight);
                g.DrawRectangle(Pens.Black, x, y, widths[i], rowHeight);
                g.DrawString(headers[i], headerFont, Brushes.Black, x + 4, y + 5);
                x += widths[i];
            }
            y += rowHeight;

            int serialNo = 1;
            foreach (DataGridViewRow row in dgvOrder.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string item = Convert.ToString(row.Cells["Items"].Value);
                string quantity = Convert.ToString(row.Cells["Quantity"].Value);
                if (string.IsNullOrEmpty(item) && string.IsNullOrEmpty(quantity))
                    continue;

                x = bounds.Left;
                string[] values = new string[]
                {
                    serialNo.ToString(),
                    item,
                    Convert.ToString(row.Cells["UOM"].Value),
                    quantity,
                    ProductPriceIncludingTax(row)
                };

                for (int i = 0; i < values.Length; i++)
                {
                    g.DrawRectangle(Pens.Black, x, y, widths[i], rowHeight);
                    g.DrawString(values[i], normalFont, Brushes.Black, new RectangleF(x + 4, y + 5, widths[i] - 8, rowHeight - 8));
                    x += widths[i];
                }

                y += rowHeight;
                serialNo++;
            }

            return y + 10;
        }


        public void save(int v)
        {
            panel1.Enabled = false;
            pnenablefalse();

            Pnloading.Visible = true;
            try
            {
                string output = SaveDeliveryNotePending();
                txtorder.Text = output;
                cmbstatus.Text = "PENDING";
                Pnloading.Visible = false;
                panel1.Enabled = true;
                MessageBox.Show(NoteTitle() + " saved successfully. " + output);
                GetSearchOrder();
                clear();
                return;
            }
            catch (Exception ex)
            {
                Pnloading.Visible = false;
                panel1.Enabled = true;
                pnenabletrue();
                MessageBox.Show(ex.Message);
                return;
            }


            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(txtorder.Text))
            {
                objQuotationbal.isnew = 0;
            }
            else
            {
                objQuotationbal.isnew = 1;
            }
            objQuotationbal.Quotationid = txtorder.Text;
            objQuotationbal.Customerid = Convert.ToString(cmbcustomername.SelectedValue);
            objQuotationbal.date = date.Value;
            objQuotationbal.Referenceid = Convert.ToString(cmbreference.SelectedValue);
            objQuotationbal.Assist = Convert.ToString(cmbassistby.SelectedValue);
            objQuotationbal.Assistnames = string.Empty;
                           
            objQuotationbal.Customername = Convert.ToString(cmbcustomername.Text.Trim());
            objQuotationbal.City = Convert.ToString(cmbToLocation.Text.Trim());


            if (v == 1)
            {
                objQuotationbal.status = "Open";
            }
            else if (v == 2)
            {
                objQuotationbal.status = "Quote Completed";
            }
            objQuotationbal.Updatedby = Program.userid;
            dt = DataGridView2DataTable(dgvOrder);
            for (int i = 0; i < 3; i++)
            {
                dt.Columns.RemoveAt(0);
            }
            RemoveNullColumnFromDataTable(dt);
            dt.Columns.RemoveAt(4);
            bool dtval = RemoveDuplicateRows(dt, "ProductId");


            if (dtval)
            {

                string output = objQuotationbal.SaveQuotation(objQuotationbal, dt);
                SaveFollowupDate(output);
                if (!string.IsNullOrEmpty(output) && string.IsNullOrEmpty(txtorder.Text))
                {
                    panel1.Enabled = true;
                    pnenabletrue();
                    Pnloading.Visible = false;

                    //MessageBox.Show("save successfully");
                    txtorder.Text = output;
                    GetStockLessReport(StockCheck, output);
                    if (v == 2)
                    {
                        //GetReport(output);

                        //DialogResult Res = MessageBox.Show("Do you Want Stock Less Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        //if (Res == DialogResult.Yes)
                        //{



                        //}

                        //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        //if (result == DialogResult.Yes)
                        //{
                        //    Quotationreport rpt = new Quotationreport(output);
                        //    rpt.ShowDialog();

                        GetReport(output);
                        //}
                    }
                    else if (v == 2)
                    {
                        GetReport(output);
                    }

                }
                else if (!string.IsNullOrEmpty(output) && !string.IsNullOrEmpty(txtorder.Text))
                {
                    //MessageBox.Show("Update successfully");
                    panel1.Enabled = true;
                    pnenabletrue();
                    Pnloading.Visible = false;

                    txtorder.Text = output;
                    //DialogResult Res = MessageBox.Show("Do you Want Stock Less Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //if (Res == DialogResult.Yes)
                    //{


                    GetStockLessReport(StockCheck, output);
                    //}
                    if (v == 2)
                    {
                        //DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                        //if (result == DialogResult.Yes)
                        //{
                        //    Quotationreport rpt = new Quotationreport(output);
                        //    rpt.ShowDialog();
                        //}






                        GetReport(output);
                    }
                    else if (v == 2)
                    {
                        GetReport(output);
                    }
                    else if (v == 1)
                    {
                        GetReport(output);
                    }
                }
                bindpending();
                btnSearch.PerformClick();
                // search("Quotationid", "", "q.Updatedon", "Today", "customername", "", role1, Program.userid);
                clear();
            }
            else
            {
                MessageBox.Show("Please Remove Duplication Product");
                panel1.Enabled = true;
                btnSavePending.Enabled = true;
                btnSave.Enabled = true;
                btnPrint.Enabled = !receiptMode && currentDeliveryNoteId > 0;
                Pnloading.Visible = false;

                cmbcustomername.Enabled = receiptMode;
                cmbToLocation.Enabled = !receiptMode;
                cmbreference.Enabled = true;
                cmbassistby.Enabled = true;
                dgvOrder.ReadOnly = false;
                this.dgvOrder.Columns["Rate"].ReadOnly = true;
                this.dgvOrder.Columns[0].ReadOnly = true;
                this.dgvOrder.Columns["Items"].ReadOnly = true;
                this.dgvOrder.Columns["UOM"].ReadOnly = true;
                this.dgvOrder.Columns["Amount"].ReadOnly = true;
            }
        }


        public void GetStockLessReport(DataTable QuotationId, string output)
        {
            try
            {
                if (QuotationId.Rows.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Do you Want Less Stock Details ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Quotationreport rpt = new Quotationreport(txtorder.Text);
                        //rpt.ShowDialog();




                        QuotationStockReport Obj = new QuotationStockReport();
                        Obj.dsMain = QuotationId;
                        Obj.IdQutoastion = output;
                        //Obj.ShowDialog();
                        if (Obj.GenerateQuoationid())
                        {
                            frmPrintPreview objfrmpreview = new frmPrintPreview();
                            objfrmpreview.fileName = Obj.fileName;
                            objfrmpreview.Show();

                        }






                    }
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public bool RemoveDuplicateRows(DataTable dTable, string colName)
        {
            bool sa = true;
            int index = 0;
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
            {
                index = dTable.Rows.IndexOf(dRow);
                dgvOrder.Rows[index].DefaultCellStyle.ForeColor = Color.Red;
                sa = false;
            }
            //dTable.Rows.Remove(dRow);


            //Datatable which contains unique records will be return as output.
            return sa;
        }
        public void GetReport(string QuotationId)
        {
            try
            {
                if (!string.IsNullOrEmpty(QuotationId))
                {
                    DialogResult result = MessageBox.Show("Do you want to Print?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Quotationreport rpt = new Quotationreport(txtorder.Text);
                        //rpt.ShowDialog();

                        using (SqlConnection con = new SqlConnection(Program.connection))
                        {
                            DataSet ds = new DataSet();
                            con.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.AddWithValue("@id", QuotationId);
                            cmd.Parameters.AddWithValue("@companyname", Program.Company);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "GetQuotationreport_Print_RackOrder";
                            cmd.Connection = con;
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(ds);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                try
                                {

                                    RREPrint objRREPrint = new RREPrint();
                                    objRREPrint.dsMain = ds;
                                    objRREPrint.pagenumber = 1;
                                    objRREPrint.status = true;
                                    objRREPrint._strRefText = "Qtn:";
                                    objRREPrint._strRef = QuotationId;

                                    objRREPrint.RREPrintQuotation();
                                }

                                catch (Exception ex)
                                {

                                }
                            }


                        }

                        //System.Diagnostics.Process myProc = new System.Diagnostics.Process();
                        //myProc.StartInfo.FileName = "type " + Obj.fileName + " >prn";  //Attempting to start a non-existing executable
                        //myProc.Start();    //Start the application and assign it to the process component.    
                        //ExecuteCommandSync("type " + Obj.fileName + " >prn");



                    }
                }
            }

            catch(Exception ex)
            {

            }

        }
    


    

        public void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }


        private void cmbcustomername_SelectedIndexChanged(object sender, EventArgs e)
        {

            string s = "";
            if (cmbcustomername.SelectedIndex > 0)
            {
                s = Convert.ToString(cmbcustomername.SelectedValue);
            }

            cmdcity.Text = objQuotationbal.bindcity(s);


        }

        public DataTable DataGridView2DataTable(DataGridView dgv, int minRow = 0)
        {

            DataTable dt = new DataTable();

            // Header columns
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                DataColumn dc = new DataColumn(column.Name.ToString());
                dt.Columns.Add(dc);
            }

            // Data cells
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                DataGridViewRow row = dgv.Rows[i];
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    dr[j] = (row.Cells[j].Value == null) ? "" : row.Cells[j].Value.ToString();
                }
                dt.Rows.Add(dr);
            }

            // Related to the bug arround min size when using ExcelLibrary for export
            for (int i = dgv.Rows.Count; i < minRow; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dr[j] = "  ";
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }



        private void Btnsubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Txtitem.Text))
            {
                int rowindex = Convert.ToInt32(lblrowindex.Text);
                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
                dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                dgvOrder.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                dgvOrder.Rows[rowindex].Cells[4].Value = lblrack.Text;
                dgvOrder.Rows[rowindex].Cells[7].Value = category.Text;
                dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                DgvAutoRefNo.Visible = false;
                btnLess.Enabled = true;
                pnsearch.Visible = false;
                lblproductid.Text = string.Empty;
                //Txtitem.Text = string.Empty;
                lblitemcode.Text = "0";
                lblrack.Text = "0";
                lbldisplay.Text = "0";
                lbldemo.Text = "0";
                lblservice.Text = "0";
                lbldamage.Text = "0";
                lblprice.Text = "0";
                dgvOrder.Focus();
                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
            }
            else
            {
                MessageBox.Show("Please Enter Product Name");
                Txtitem.Focus();
            }


        }


        public void itemdetails(string s)
        {

            try
            {
                dtitems = new DataTable();
                string s1 = s.Trim();
                string s2 = Convert.ToString(cmbloaction.SelectedValue);
                string name = s1.Replace("'", "''");

                // DataTable st = StockReportBAL.GetStockReportFinal(name);
                dtitems = objQuotationbal.itemdetails(name, s2);

                Program.dtitems = dtitems;

            }
            catch (Exception e)
            {

            }

        }

        private void Txtitem_TextChanged(object sender, EventArgs e)
        {
            ProdSelRowvalue = 0;

        }

        private void transactionclose_Click(object sender, EventArgs e)
        {
            pnsearch.Visible = false;
            lblproductid.Text = string.Empty;
            //Txtitem.Text = string.Empty;
            lblitemcode.Text = "0";
            lblrack.Text = "0";
            lbldisplay.Text = "0";
            lbldemo.Text = "0";
            lblservice.Text = "0";
            lbldamage.Text = "0";
            lblprice.Text = "0";
            //Txtitem.Text = string.Empty;
            DgvAutoRefNo.DataSource = null;

            DgvAutoRefNo.Visible = false;
        }

        public void RemoveNullColumnFromDataTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][0])))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }

        private void dgvOrder_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dgvOrder.CurrentCell.ColumnIndex;
            string headerText = dgvOrder.Columns[column].HeaderText;

            if (headerText.Equals("Quantity"))
            {
                tb = e.Control as TextBox;


                if (tb != null)
                {
                    ///tb.TextChanged += new EventHandler(textbox_Change);
                    tb.KeyPress += new KeyPressEventHandler(textbox_keypress);
                    tb.MaxLength = 10;
                }
            }
            else if (headerText.Equals("Rate"))
            {
                tbrate = e.Control as TextBox;


                if (tbrate != null)
                {
                    tbrate.TextChanged += new EventHandler(textbox_Change);
                    tbrate.KeyPress += new KeyPressEventHandler(txtch);
                }


            }
        }

        private void textbox_Change(object sender, EventArgs e)
        {
            int column = dgvOrder.CurrentCell.ColumnIndex;
            string headerText = dgvOrder.Columns[column].HeaderText;
            if (headerText.Equals("Rate"))
            {
                if (!string.IsNullOrEmpty(tbrate.Text))
                {
                    if (tbrate.Text.Contains("-"))
                    {
                        tbrate.Text = tbrate.Text.Replace("-", "");
                    }
                }
            }
        }

        public void txtch(object sender, KeyPressEventArgs e)
        {
            //if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            //    e.Handled = true;
            if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar))
                                && (e.KeyChar != '.'))
                e.Handled = true;


            //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            //{
            //    e.Handled = true;
            //}

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }


        }

        Regex reg = new Regex(@"^-?\d+[.]?\d*$");
        Regex reg1 = new Regex(@"^-?[.]?\d*$");
        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
            ////if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            ////    e.Handled = true;
            //if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar))
            //                    && (e.KeyChar != '.') && (e.KeyChar != '-'))
            //    e.Handled = true;


            ////if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            ////{
            ////    e.Handled = true;
            ////}

            //// only allow one decimal point
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = true;
            //}

            //// only allow minus sign at the beginning
            //if (e.KeyChar == '-' && (sender as TextBox).Text.Length > 0)
            //{
            //    e.Handled = true;
            //}
            try
            {
                if (char.IsControl(e.KeyChar)) return;
                if ((reg.IsMatch(tb.Text.Insert(tb.SelectionStart, e.KeyChar.ToString()) + "1")) || reg1.IsMatch(tb.Text.Insert(tb.SelectionStart, e.KeyChar.ToString()) + "1"))
                {

                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {

            }
        }

        private void dgvOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            total();
            if (e.ColumnIndex == 1)
            {
                rdbStartsWith.Checked = true;
                if (dgvOrder.ReadOnly == false)
                {
                    pnsearch.Visible = true;
                }
                //pnsearch.Visible = true;
                if (!string.IsNullOrEmpty(lblhiddenproduct.Text))
                {
                    Txtitem.Text = lblhiddenproduct.Text;
                    AutoCompleteLoad(Txtitem.Text, 1);
                    if (DgvAutoRefNo.Rows.Count > 0)
                    {
                        DgvAutoRefNo.Rows[0].Cells[0].Selected = false;
                        DefaultFloor.Text = "0";
                        Display.Text = "0";
                        Damage.Text = "0";
                        Checking.Text = "0";
                        Delivery.Text = "0";
                        lblprice.Text = "0";
                    }

                }

                Txtitem.SelectionStart = 0;
                Txtitem.SelectionLength = Txtitem.Text.Length;
                this.ActiveControl = Txtitem;
                lblrowindex.Text = e.RowIndex.ToString();
                lblpageno.Text = Convert.ToString(Convert.ToInt32(lblpageno.Text) + 1);
            }
            //else if (e.ColumnIndex == 4)
            //{
            //    if (dgvOrder.ReadOnly == false)
            //    {
            //        if (dgvOrder.Columns[4].ReadOnly==true)
            //        {
            //                dgvOrder.Rows[e.RowIndex].Cells[4].ReadOnly = true;

            //        }

            //    }
            //}
            //    else
            //    {
            //        pnsearch.Visible = false; ;
            //    }
        }

        public void bindpending()
        {
            flowLayoutPanel1.Controls.Clear();
            DataTable dt = objQuotationbal.pending(userid);
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    RadioButton button = new RadioButton();
            //    button.Tag = i;
            //    button.Width = 200;
            //    button.FlatStyle = FlatStyle.Popup;
            //    button.Appearance = Appearance.Button;
            //    button.Cursor = Cursors.Hand;
            //    button.CheckedChanged += new EventHandler(button_click);
            //    button.Text = Convert.ToString(dt.Rows[i]["Quotationid"]);
            //    flowLayoutPanel1.Controls.Add(button);
            //}


            RadioButton btn = new RadioButton();
            btn.Tag = 0;
            btn.Width = 50;
            btn.FlatStyle = FlatStyle.Popup;
            btn.Appearance = Appearance.Button;
            btn.Cursor = Cursors.Hand;
            btn.Checked = true;
            btn.CheckedChanged += new EventHandler(button_click);
            btn.Text = "New";
            flowLayoutPanel1.Controls.Add(btn);


        }
        private void button_click(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;

            if (btn.Checked)
            {
                getquotaion(btn.Text);
                total();
                pnenabletrue();
            }

        }
        public void getquotaion(string s)
        {
            this.dgvOrder.Columns["Amount"].ReadOnly = true;
            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns["Items"].ReadOnly = true;
            this.dgvOrder.Columns["UOM"].ReadOnly = true;
            this.dgvOrder.Columns["productid"].ReadOnly = true;
            this.dgvOrder.Columns["Rate"].ReadOnly = true;
            DataSet ds = objQuotationbal.getquotation(s);
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbcustomername.Text = Convert.ToString(ds.Tables[0].Rows[0]["customername"]);
                cmbToLocation.Text = Convert.ToString(ds.Tables[0].Rows[0]["city"]);
                cmbreference.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Referenceid"]);
                cmbassistby.SelectedValue = Convert.ToInt32(ds.Tables[0].Rows[0]["Assist"]);
                lblperare.Text = Convert.ToString(ds.Tables[0].Rows[0]["Updatedby"]);
                txtorder.Text = Convert.ToString(ds.Tables[0].Rows[0]["Quotationid"]);
                cmbstatus.Text = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                date.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["date"]);
                LoadFollowupDate(txtorder.Text);
                string final = Convert.ToString(ds.Tables[0].Rows[0]["Final"]);
                if (final == "Open")
                {
                    pnenabletrue();


                    btnSavePending.Enabled = true;
                    btnSave.Enabled = true;
                    btnPrint.Enabled = false;
                }
                else
                {
                    pnenablefalse();

                    pnenablefalse();
                    btnSavePending.Enabled = false;
                    btnSave.Enabled = false;
                    btnPrint.Enabled = false;
                }

            }
            else
            {
                pnenablefalse();
                btnSavePending.Enabled = false;
                btnSave.Enabled = false;
                btnPrint.Enabled = false;
                clear();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                string s1 = string.Empty;
                double qty;
                dgvOrder.Rows.Clear();
                this.dgvOrder.Columns["Rate"].ReadOnly = true;
                btnLess.Enabled = true;
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    dgvOrder.Rows.Add();
                    dgvOrder.Rows[i].Cells[0].Value = i + 1;
                    dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(ds.Tables[1].Rows[i]["DisplayName"]);
                    dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(ds.Tables[1].Rows[i]["UOM"]);
                    dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds.Tables[1].Rows[i]["Productid"]);
                    s1 = Convert.ToString(ds.Tables[1].Rows[i]["Rate"]);
                    if (string.IsNullOrEmpty(s1))
                    {
                        qty = 0;
                    }
                    else
                    {
                        qty = Convert.ToDouble(s1);
                    }

                    dgvOrder.Rows[i].Cells[4].Value = qty;
                    dgvOrder.Rows[i].Cells[5].Value = Convert.ToString(ds.Tables[1].Rows[i]["Quantity"]);
                    double amt = Convert.ToDouble(ds.Tables[1].Rows[i]["Amount"]);
                    dgvOrder.Rows[i].Cells[6].Value = amt;
                }
                //panel2.Enabled = false;
            }
            else
            {
                btnLess.Enabled = false;
                dgvOrder.Rows.Clear();
                pnenablefalse();
                btnSavePending.Enabled = false;
                btnSave.Enabled = false;
                btnPrint.Enabled = false;
            }

        }
        public void pnenabletrue()
        {

            cmbcustomername.Enabled = receiptMode;
            cmbToLocation.Enabled = !receiptMode;
            cmbreference.Enabled = true;
            cmbassistby.Enabled = true;
            comboBox1.Enabled = true;
            panel3.Enabled = true;
            followupdate.Enabled = true;
            txtFollowupPhone.Enabled = true;
            dgvOrder.ReadOnly = false;
        }
        public void pnenablefalse()
        {

            cmbcustomername.Enabled = false;
            cmbToLocation.Enabled = false;
            comboBox1.Enabled = false;
            cmbreference.Enabled = false;
            cmbassistby.Enabled = false;
            panel3.Enabled = false;
            followupdate.Enabled = false;
            txtFollowupPhone.Enabled = false;
            dgvOrder.ReadOnly = true;
        }

        public void total()
        {
            try
            {
                double totalamount = 0.00, totalquantity = 0.00;
                double value = 0.0, value1 = 0.0;

                for (int i = 0; i < dgvOrder.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[6].Value)))
                    {
                        value = 0.0;
                    }
                    else
                    {
                        value = Convert.ToDouble(dgvOrder.Rows[i].Cells[6].Value);
                    }

                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[i].Cells[5].Value)))
                    {
                        value1 = 0.0;
                    }
                    else
                    {
                        value1 = Convert.ToDouble(dgvOrder.Rows[i].Cells[5].Value);
                    }

                    totalamount = totalamount + value;
                    totalquantity = totalquantity + value1;
                }

                // totalquantity = Math.Round(totalquantity);
                // totalamount = Math.Round(totalamount);


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


                lbltotalquantity.Text = Convert.ToString(totalquantity);
                lbltotalamount.Text = String.Format("{0:0,0.00}", totalamount);
            }
            catch
            {

            }
        }

        private void dgvOrder_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (savevads == false)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value)))
                    {
                        decimal rate = Convert.ToDecimal(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
                        //decimal amt = rate * Convert.ToDecimal(tb.Text);

                        decimal amt = rate * Convert.ToDecimal(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value);

                        if (amt > 0)
                        {
                            string[] str = amt.ToString().Split('.');
                            if (str.Length > 1)
                            {
                                double num1 = Convert.ToDouble("0." + str[1]);

                                if (num1 >= 0.5)
                                {
                                    amt = Math.Ceiling(amt);
                                }
                                else
                                {
                                    amt = Math.Floor(amt);
                                }

                            }
                        }
                        else
                        {
                            string[] str = amt.ToString().Split('.');
                            if (str.Length > 1)
                            {
                                double num1 = Convert.ToDouble("0." + str[1]);

                                if (num1 >= 0.5)
                                {
                                    amt = Math.Floor(amt);
                                }
                                else
                                {
                                    amt = Math.Ceiling(amt);
                                }

                            }
                        }
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = amt;
                    }


                }
                catch (Exception sa)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                    {
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = 0.00;
                    }
                }
                total();

            }
            else
            {
                savevads = false;
            }
        }

        //private void btnSearch_Click(object sender, EventArgs e)
        //{
        //    if ((cbxSearchOrderNo.SelectedIndex == cbxSearchOrderDate.SelectedIndex) || cbxSearchOrderNo.SelectedIndex == cbxVendor.SelectedIndex || cbxSearchOrderDate.SelectedIndex == cbxVendor.SelectedIndex)
        //    {
        //        MessageBox.Show("Search a item Should Not Be Same");
        //    }
        //    else
        //    {


        //        firstname = cbxSearchOrderNo.Text.Trim();
        //        if (firstname == "Order Number")
        //        {
        //            firstname = "Quotationid";
        //            if (cmbstatus1.SelectedIndex != 0)
        //            {
        //                firstvalue = cmbstatus1.Text.Trim();
        //            }
        //            else
        //            {
        //                firstvalue = "";
        //            }
        //        }
        //        else if (firstname == "Customer")
        //        {
        //            firstname = "customername";
        //            if (cmbstatus1.SelectedIndex != 0)
        //            {
        //                firstvalue = cmbstatus1.Text.Trim();
        //            }
        //            else
        //            {
        //                firstvalue = "";
        //            }
        //        }
        //        else if (firstname == "Reference")
        //        {
        //            firstname = "r.Name";
        //            if (cmbstatus1.SelectedIndex != 0)
        //            {
        //                firstvalue = cmbstatus1.Text.Trim();
        //            }
        //            else
        //            {
        //                firstvalue = "";
        //            }
        //        }


        //        secondname = cbxSearchOrderDate.Text.Trim();
        //        if (secondname == "Order Number")
        //        {
        //            secondname = "Quotationid";
        //            if (cmbstatus2.SelectedIndex != 0)
        //            {
        //                secondvalue = cmbstatus2.Text.Trim();
        //            }
        //            else
        //            {
        //                secondvalue = "";
        //            }

        //        }
        //        else if (secondname == "Customer")
        //        {
        //            secondname = "customername";
        //            if (cmbstatus2.SelectedIndex != 0)
        //            {
        //                secondvalue = cmbstatus2.Text.Trim();
        //            }
        //            else
        //            {
        //                secondvalue = "";
        //            }
        //        }
        //        else if (secondname == "Reference")
        //        {
        //            secondname = "r.Name";
        //            if (cmbstatus2.SelectedIndex != 0)
        //            {
        //                secondvalue = cmbstatus2.Text.Trim();
        //            }
        //            else
        //            {
        //                secondvalue = "";
        //            }
        //        }


        //        thirdname = cbxVendor.Text.Trim();
        //        if (thirdname == "Order Number")
        //        {
        //            thirdname = "Quotationid";
        //            if (cmbstatus3.SelectedIndex != 0)
        //            {
        //                thirdvalue = cmbstatus3.Text.Trim();
        //            }
        //            else
        //            {
        //                thirdvalue = "";
        //            }


        //        }
        //        else if (thirdname == "Customer")
        //        {
        //            thirdname = "customername";
        //            if (cmbstatus3.SelectedIndex != 0)
        //            {
        //                thirdvalue = cmbstatus3.Text.Trim();
        //            }
        //            else
        //            {
        //                thirdvalue = "";
        //            }
        //        }
        //        else if (thirdname == "Reference")
        //        {
        //            thirdname = "r.Name";
        //            if (cmbstatus3.SelectedIndex != 0)
        //            {
        //                thirdvalue = cmbstatus3.Text.Trim();
        //            }
        //            else
        //            {
        //                thirdvalue = "";
        //            }
        //        }

        //        search(firstname1, firstvalue1, secondname1, secondvalue1, thirdname1, thirdvalue1, role1, Program.userid);
        //    }
        //}

        public void bindorderno(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = ExecuteDeliveryTable("SELECT 0 AS NoteId, '-Select-' AS NoteNo UNION ALL SELECT " + HeaderIdColumn() + " AS NoteId, " + NoteNoColumn() + " AS NoteNo FROM dbo." + HeaderTable() + " WHERE ISNULL(IsDeleted, 0) = 0 ORDER BY NoteNo");
            cmb.DataSource = dt;
            cmb.DisplayMember = "NoteNo";
            cmb.ValueMember = "NoteNo";
        }

        public void bindcustomer(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = GetBranches();
            cmb.DataSource = dt;
            cmb.DisplayMember = "BranchName";
            cmb.ValueMember = "BranchId";
        }

        public void bindreference(ComboBox cmb)
        {
            cmb.DataSource = null;
            DataTable dt = objQuotationbal.Getreference();
            cmb.DataSource = dt;
            cmb.DisplayMember = "Name";
            cmb.ValueMember = "ReferencesID";
        }

        private void cbxSearchOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        private void cbxSearchOrderDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        private void cbxVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }


        public void search(string Customer, string OrderNumber, DateTime FromDate, DateTime ToDate, string Product, string Qty, string UserId)
        {

            int toBranchId = 0;
            int.TryParse(Convert.ToString(OrderNumber), out toBranchId);
            DataTable dt = SearchDeliveryNotes(FromDate, ToDate, Customer, toBranchId);


            dgvSearch.DataSource = null;


            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["NoteNo"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["ToBranch"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Status"]);
                dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i]["NoteId"]);

            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

        }

        public void Mergesearch(string OrderNumber, DateTime FromDate, DateTime ToDate, string Product, string Qty, string UserId)
        {

            int toBranchId = 0;
            int.TryParse(Convert.ToString(OrderNumber), out toBranchId);
            DataTable dt = SearchDeliveryNotes(FromDate, ToDate, string.Empty, toBranchId);


            dgvSearch.DataSource = null;


            dgvSearch.Rows.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgvSearch.Rows.Add();
                dgvSearch.Rows[i].Cells[0].Value = Convert.ToString(dt.Rows[i]["NoteNo"]);
                dgvSearch.Rows[i].Cells[1].Value = Convert.ToString(dt.Rows[i]["ToBranch"]);
                dgvSearch.Rows[i].Cells[2].Value = Convert.ToString(dt.Rows[i]["Status"]);
                dgvSearch.Rows[i].Cells[3].Value = Convert.ToString(dt.Rows[i]["NoteId"]);

            }

            lblItemCount.Text = Convert.ToString(dt.Rows.Count);

        }
        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string dd = label27.Text;
            if (dd != "1")
            {
                if (e.RowIndex >= 0)
                {
                    string s = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex].Cells[0].Value);
                    string id = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex].Cells[3].Value);
                    if (!string.IsNullOrEmpty(s))
                    {
                        LoadDeliveryNote(Convert.ToInt32(id));
                        total();


                    }
                    else
                    {
                        clear();
                    }

                }
            }
        }

        private void dgvOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                try
                {
                    e.SuppressKeyPress = true;
                    if (dgvOrder.CurrentCell.ColumnIndex == 0)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 1)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[2, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 2)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[4, dgvOrder.CurrentCell.RowIndex];

                    }
                    else if (dgvOrder.CurrentCell.ColumnIndex == 4)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[5, dgvOrder.CurrentCell.RowIndex];

                    }


                    else if (dgvOrder.CurrentCell.ColumnIndex == 6)
                    {
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder[1, dgvOrder.CurrentCell.RowIndex + 1];

                    }
                }
                catch
                {

                }

            }
        }

        private void dgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5)
                {
                    dgvOrder.Focus();
                    //edit = true;
                    dgvOrder.CurrentCell = dgvOrder[1, e.RowIndex + 1];
                }

                if (e.ColumnIndex == 4)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value)))
                    {
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value = 0;
                    }
                }
            }
            catch
            {

            }
        }

        private void dgvOrder_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (edit == true)
                {
                    if (dgvOrder.CurrentCell.RowIndex >= 1)
                    {
                        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex - 1];
                        edit = false;
                    }
                    else if (dgvOrder.CurrentCell.RowIndex == 0)
                    {
                        dgvOrder.CurrentCell = dgvOrder[dgvOrder.CurrentCell.ColumnIndex, dgvOrder.CurrentCell.RowIndex - 1];
                    }

                }
            }
            catch
            {

            }
        }

        private void Txtitem_KeyDown(object sender, KeyEventArgs e)
        {
            //if(e.KeyData==Keys.Down)
            //{
            //    DgvAutoRefNo.Focus();
            //    DgvAutoRefNo.CurrentCell = DgvAutoRefNo[0, 0];
            //    DgvAutoRefNo.Rows[0].Cells[0].Selected = true;
            //}
            //try
            //{
            //    if (e.KeyData == Keys.Enter)
            //    {
            //        if (!string.IsNullOrEmpty(Txtitem.Text))
            //        {
            //            if (Convert.ToInt32(lblproductid.Text) != 0)
            //            {
            //                dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
            //                int rowindex = Convert.ToInt32(lblrowindex.Text);
            //                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
            //                dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
            //                dgvOrder.Rows[rowindex].Cells[1].Value = cas.ToUpper();
            //                dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
            //                double val = Convert.ToDouble(lblprice.Text);
            //                dgvOrder.Rows[rowindex].Cells[4].Value = val;
            //                dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
            //                DgvAutoRefNo.Visible = false;

            //                pnsearch.Visible = false;
            //                lblproductid.Text = string.Empty;
            //                Txtitem.Text = string.Empty;
            //                lblitemcode.Text = "0";
            //                lblrack.Text = "0";
            //                lbldisplay.Text = "0";
            //                lbldemo.Text = "0";
            //                lblservice.Text = "0";
            //                lbldamage.Text = "0";
            //                lblprice.Text = "0";
            //                dgvOrder.Focus();
            //                dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
            //            }
            //            else
            //            {
            //                MessageBox.Show("Please Enter Correct Product Name");
            //            }
            //        }
            //        else
            //        {
            //            this.ActiveControl = btnSave;
            //            pnsearch.Visible = false;
            //            //MessageBox.Show("Please Enter Product Name");
            //            //Txtitem.Focus();
            //        }
            //    }
            //}
            //catch
            //{

            //}
        }

        private void vLabel1_Click(object sender, EventArgs e)
        {
            if (pnlLabelSearch.Visible == true)
            {
                pnlLabelSearch.Visible = false;
                vLabel1.Visible = false;
                pnlSearch.Visible = true;
                splitContainer1.Panel1Collapsed = false;
            }
        }

        private void DeliveryNote_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Txtitem_KeyUp(object sender, KeyEventArgs e)
        {

            string word;
            int typr = 0;
            try
            {

                if (e.KeyData != Keys.Enter && e.KeyData != Keys.Tab && e.KeyData != Keys.Down && e.KeyData != Keys.Up && e.KeyData != Keys.Left && e.KeyData != Keys.Right && e.KeyData != Keys.Escape && e.KeyData != Keys.F2 && e.KeyData != (Keys.S | Keys.Alt) && e.KeyData != (Keys.C | Keys.Alt) && e.KeyData != (Keys.N | Keys.Alt) && e.KeyData != (Keys.V | Keys.Alt) && e.KeyData != (Keys.M | Keys.Alt) && e.KeyData != (Keys.D | Keys.Alt) && e.KeyData != (Keys.X | Keys.Alt))
                {
                    word = Txtitem.Text;
                    if (rdbStartsWith.Checked)
                    {
                        typr = 1;
                    }
                    else if (rdbContains.Checked)
                    {
                        typr = 2;
                    }
                    if (word.Trim() != "")
                        AutoCompleteLoad(word, typr);

                }
                if (e.KeyData == Keys.Up)
                {

                    //try
                    //{
                    //    DataGridViewRow theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue - 1];
                    //    if (theRow3.Index != DgvAutoRefNo.RowCount)
                    //    {

                    //        theRow3.DefaultCellStyle.BackColor = Color.LightGray;

                    //        theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue];
                    //        theRow3.DefaultCellStyle.BackColor = Color.White;

                    //        ProdSelRowvalue--;
                    //        cas = Convert.ToString(DgvAutoRefNo[0, ProdSelRowvalue].Value);
                    //        itemdetails(cas);
                    //        RefScrollGrid();
                    //    }
                    //}
                    //catch
                    //{
                    //    //ProdSelRowvalue = 0;
                    //}

                }
                if (e.KeyData == Keys.Down)
                {
                    if (DgvAutoRefNo.Rows.Count > 0)
                    {
                        DgvAutoRefNo.Focus();
                        DgvAutoRefNo.CurrentCell = DgvAutoRefNo[0, 0];
                        DgvAutoRefNo.Rows[0].Cells[0].Selected = true;
                        string sa = Convert.ToString(DgvAutoRefNo.Rows[0].Cells[0].Value);
                        getitems(sa);
                    }
                    //try
                    //{
                    //    if (DgvAutoRefNo.Rows.Count - 1 != ProdSelRowvalue)
                    //    {
                    //        DataGridViewRow theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue + 1];
                    //        if (theRow3.Index != DgvAutoRefNo.RowCount)
                    //        {

                    //            theRow3.DefaultCellStyle.BackColor = Color.LightGray;

                    //            theRow3 = DgvAutoRefNo.Rows[ProdSelRowvalue];
                    //            theRow3.DefaultCellStyle.BackColor = Color.White;

                    //            ProdSelRowvalue++;
                    //            cas = Convert.ToString(DgvAutoRefNo[0, ProdSelRowvalue].Value);
                    //            itemdetails(cas);
                    //            RefScrollGrid();
                    //        }
                    //    }
                    //}
                    //catch
                    //{
                    //    //ProdSelRowvalue = 0;
                    //}

                }

                if (e.KeyData == Keys.Enter)
                {
                    //if (!string.IsNullOrEmpty(Txtitem.Text))
                    //{
                    //    if (res == false)
                    //    {
                    //        if (DgvAutoRefNo.Visible == false)
                    //        {
                    //            DgvAutoRefNo.Visible = false;

                    //        }
                    //        else
                    //        {
                    //            Txtitem.Text = Convert.ToString(DgvAutoRefNo[0, DgvAutoRefNo.CurrentCell.RowIndex].Value);
                    //            DgvAutoRefNo.Visible = false;
                    //            DgvAutoRefNo.Rows[0].Selected = false;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    //if (status6 == false && v == false)
                    //    //{
                    //    //    MessageBox.Show("No records found");
                    //    //    txtRegNo.Focus();
                    //    //    status6 = true;
                    //    //}
                    //    //else
                    //    //{
                    //    //    status6 = false;
                    //    //    v = false;


                    //    //}
                    //}

                }
            }
            catch (Exception efd)
            {

            }
        }

        private void DgvAutoRefNo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (Convert.ToInt32(lblproductid.Text) != 0)
                {
                    //dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
                    int rowindex = Convert.ToInt32(lblrowindex.Text);
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];

                    //itemdetails(Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value));

                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                    getitems(sa);

                    dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    btnLess.Enabled = true;
                    dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[e.RowIndex].Cells[0].Value).ToUpper();

                    //dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                    //dgvOrder.Rows[rowindex].Cells[1].Value = cas.ToUpper();
                    dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                    double val = Convert.ToDouble(lblprice.Text);
                    dgvOrder.Rows[rowindex].Cells[4].Value = val;
                    dgvOrder.Rows[rowindex].Cells[7].Value = category.Text;
                    dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                    DgvAutoRefNo.Visible = false;

                    pnsearch.Visible = false;
                    lblproductid.Text = string.Empty;
                    //  Txtitem.Text = string.Empty;
                    lblitemcode.Text = "0";
                    lblrack.Text = "0";
                    lbldisplay.Text = "0";
                    lbldemo.Text = "0";
                    lblservice.Text = "0";
                    lbldamage.Text = "0";
                    lblprice.Text = "0";
                    dgvOrder.Focus();
                    dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
                }
            }
            //else
            //{
            //    MessageBox.Show("Please Enter Correct Product Name");
            //}
        }

        private void dgvOrder_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {


                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value)))
                {
                    decimal rate = Convert.ToDecimal(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[4].Value);
                    //decimal amt = rate * Convert.ToDecimal(tb.Text);

                    decimal amt = rate * Convert.ToDecimal(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5].Value);

                    if (amt > 0)
                    {
                        string[] str = amt.ToString().Split('.');
                        if (str.Length > 1)
                        {
                            double num1 = Convert.ToDouble("0." + str[1]);

                            if (num1 >= 0.5)
                            {
                                amt = Math.Ceiling(amt);
                            }
                            else
                            {
                                amt = Math.Floor(amt);
                            }

                        }
                    }
                    else
                    {
                        string[] str = amt.ToString().Split('.');
                        if (str.Length > 1)
                        {
                            double num1 = Convert.ToDouble("0." + str[1]);

                            if (num1 >= 0.5)
                            {
                                amt = Math.Floor(amt);
                            }
                            else
                            {
                                amt = Math.Ceiling(amt);
                            }

                        }
                    }
                    dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = amt;
                }


            }
            catch
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[1].Value)))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value)))
                    {
                        dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[6].Value = 0.00;
                    }

                }

            }
            total();
        }


        public void getsino()
        {
            for (int i = 0; i < dgvOrder.Rows.Count; i++)
            {
                dgvOrder.Rows[i].Cells[0].Value = i + 1;
            }
        }


        private void DgvAutoRefNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(Txtitem.Text))
                {

                    if (Convert.ToInt32(lblproductid.Text) != 0)
                    {

                        //dgvOrder.Columns[4].DefaultCellStyle.Format = "N2";
                        int rowindex = Convert.ToInt32(lblrowindex.Text);
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
                        dgvOrder.Rows[rowindex].Cells[3].Value = lblproductid.Text;
                        btnLess.Enabled = true;
                        dgvOrder.Rows[rowindex].Cells[1].Value = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                        dgvOrder.Rows[rowindex].Cells[2].Value = lblitemcode.Text;
                        double val = Convert.ToDouble(lblprice.Text);
                        dgvOrder.Rows[rowindex].Cells[4].Value = val;
                        dgvOrder.Rows[rowindex].Cells[7].Value = category.Text;
                        dgvOrder.Rows[rowindex].Cells[0].Value = rowindex + 1;
                        DgvAutoRefNo.Visible = false;

                        pnsearch.Visible = false;
                        lblproductid.Text = string.Empty;
                        // Txtitem.Text = string.Empty;
                        lblitemcode.Text = "0";
                        lblrack.Text = "0";
                        lbldisplay.Text = "0";
                        lbldemo.Text = "0";
                        lblservice.Text = "0";
                        lbldamage.Text = "0";
                        lblprice.Text = "0";
                        Locationpanal.Controls.Clear();
                        dgvOrder.Focus();
                        dgvOrder.CurrentCell = dgvOrder.Rows[dgvOrder.CurrentCell.RowIndex].Cells[5];
                    }
                    else
                    {
                        MessageBox.Show("Please Enter Correct Product Name");
                    }
                }
                else
                {
                    this.ActiveControl = btnSave;
                    pnsearch.Visible = false;
                    //MessageBox.Show("Please Enter Product Name");
                    //Txtitem.Focus();
                }
            }







            else if (e.KeyData == Keys.Up)
            {
                if (DgvAutoRefNo.CurrentCell.RowIndex != 0)
                {
                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex - 1].Cells[0].Value);
                    getitems(sa);
                }



            }
            else if (e.KeyData == Keys.Down)
            {
                if (DgvAutoRefNo.CurrentCell.RowIndex + 1 != DgvAutoRefNo.Rows.Count)
                {
                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex + 1].Cells[0].Value);
                    getitems(sa);
                }
            }
             else if(e.KeyData == Keys.End)
            {
                //if (DgvAutoRefNo.CurrentCell.RowIndex + 1 != DgvAutoRefNo.Rows.Count)
               // {
                    string sa = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);
                    getitemdetails(sa);
                //}

            }



        }
        public void getitemdetails(string sa)
        {

            Locationpanal.Controls.Clear();
            dtitems = Program.dtitems;
            var rows = from row in dtitems.AsEnumerable()
                       where row.Field<string>("ProductName").Trim() == sa.Trim()
                       select row;
            DataTable st = rows.CopyToDataTable();
            string productname = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);


            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.CommandText = "LocationstockinPanal";
                cmd.Parameters.AddWithValue("@Productname", sa);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                con.Close();
            }


            Label lbl;
            int y = 0, z = 0;
            Point p1 = new Point();
            p1.X = 0;
            p1.Y = 0;
            int lblcount = 0;

            if (dt.Rows.Count != 0)
            {
                string lbl_Caption = "";
                bool caption = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string yyy = dt.Rows[i][0].ToString();

                    if (yyy != "0")
                    {
                        caption = true;
                        if (lbl_Caption != "")
                        {
                            lbl_Caption = lbl_Caption + "     " + dt.Rows[i][1].ToString() + " = " + dt.Rows[i][0].ToString();
                        }
                        else
                        {
                            lbl_Caption = dt.Rows[i][1].ToString() + " = " + dt.Rows[i][0].ToString();
                        }
                    }



                }
                if (caption == true)
                {
                    lbl = new Label();
                    lbl.Width = 400;
                    lbl.Text = lbl_Caption;
                    Locationpanal.Controls.Add(lbl);
                    lblcount = 1;
                }


                if (lblcount == 0)
                {
                    lbl = new Label();
                    lbl.Text = "No stock";
                    Locationpanal.Controls.Add(lbl);
                }
            }
            else
            {
                lbl = new Label();
                lbl.Text = "No stock";
                Locationpanal.Controls.Add(lbl);
            }
            //string xx=getitems(items);


            if (st.Rows.Count > 0)
            {
                lblitem.Text = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);



                lblitemcode.Text = Convert.ToString(st.Rows[0]["UOM"]);


                if (lblitemcode.Text == "")
                {
                    lblitemcode.Text = "0";
                }


                lblproductid.Text = Convert.ToString(st.Rows[0]["ProductId"]);
                if (lblproductid.Text == "")
                {
                    lblproductid.Text = "0";
                }

                lblprice.Text = Convert.ToString(st.Rows[0]["Price"]);
                if (lblprice.Text == "")
                {
                    lblprice.Text = "0";
                }

                DefaultFloor.Text = Convert.ToString(st.Rows[0]["DefaultFloor"]);
                if (DefaultFloor.Text == "")
                {
                    DefaultFloor.Text = "0";
                }

                Checking.Text = Convert.ToString(st.Rows[0]["Checking"]);
                if (Checking.Text == "")
                {
                    Checking.Text = "0";
                }


                Display.Text = Convert.ToString(st.Rows[0]["Display"]);

                if (Display.Text == "")
                {
                    Display.Text = "0";
                }


                Damage.Text = Convert.ToString(st.Rows[0]["Damage"]);
                if (Damage.Text == "")
                {
                    Damage.Text = "0";
                }

                Delivery.Text = Convert.ToString(st.Rows[0]["Delivery"]);
                if (Delivery.Text == "")
                {
                    Delivery.Text = "0";
                }


                category.Text = Convert.ToString(st.Rows[0]["Types"]);

                if (category.Text == "")
                {
                    category.Text = "0";
                }



                //pictureBox1.ImageLocation = Path.GetFullPath("131353W24J150-43D6.jpg");
                //pictureBox1.ImageLocation = itemdetails.ToList()[0].imagepath;




            }
            else
            {

                lblitemcode.Text = "0";
                lblproductid.Text = "0";
                lblprice.Text = "0";

                lblrack.Text = "0";
                lbldisplay.Text = "0";


            }
        }


        public void getitems(string sa)
        {

            Locationpanal.Controls.Clear();
            dtitems = Program.dtitems;
            var rows = from row in dtitems.AsEnumerable()
                       where row.Field<string>("ProductName").Trim() == sa.Trim()
                       select row;
            DataTable st = rows.CopyToDataTable();
            string productname = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);

            // * Commented due to performance issue
            /*
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.CommandText = "LocationstockinPanal";
                cmd.Parameters.AddWithValue("@Productname", sa);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                con.Close();
            }
             
            
            Label lbl;
            int y = 0, z = 0;
            Point p1 = new Point();
            p1.X = 0;
            p1.Y = 0;
            int lblcount = 0;

            if (dt.Rows.Count != 0)
            {
                string lbl_Caption = "";
                bool caption = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string yyy = dt.Rows[i][0].ToString();

                    if (yyy != "0")
                    {
                        caption = true;
                        if (lbl_Caption != "")
                        {
                            lbl_Caption = lbl_Caption + "     " + dt.Rows[i][1].ToString() + " = " + dt.Rows[i][0].ToString();
                        }
                        else
                        {
                            lbl_Caption = dt.Rows[i][1].ToString() + " = " + dt.Rows[i][0].ToString();
                        }
                    }



                }
                if (caption == true)
                {
                    lbl = new Label();
                    lbl.Width = 400;
                    lbl.Text = lbl_Caption;
                    Locationpanal.Controls.Add(lbl);
                    lblcount = 1;
                }


                if (lblcount == 0)
                {
                    lbl = new Label();
                    lbl.Text = "No stock";
                    Locationpanal.Controls.Add(lbl);
                }
            }
            else
            {
                lbl = new Label();
                lbl.Text = "No stock";
                Locationpanal.Controls.Add(lbl);
            }
            //string xx=getitems(items);

            */
            if (st.Rows.Count > 0)
            {
                lblitem.Text = Convert.ToString(DgvAutoRefNo.Rows[DgvAutoRefNo.CurrentCell.RowIndex].Cells[0].Value);



                lblitemcode.Text = Convert.ToString(st.Rows[0]["UOM"]);


                if (lblitemcode.Text == "")
                {
                    lblitemcode.Text = "0";
                }


                lblproductid.Text = Convert.ToString(st.Rows[0]["ProductId"]);
                if (lblproductid.Text == "")
                {
                    lblproductid.Text = "0";
                }

                lblprice.Text = Convert.ToString(st.Rows[0]["Price"]);
                if (lblprice.Text == "")
                {
                    lblprice.Text = "0";
                }

                DefaultFloor.Text = Convert.ToString(st.Rows[0]["DefaultFloor"]);
                if (DefaultFloor.Text == "")
                {
                    DefaultFloor.Text = "0";
                }

                Checking.Text = Convert.ToString(st.Rows[0]["Checking"]);
                if (Checking.Text == "")
                {
                    Checking.Text = "0";
                }


                Display.Text = Convert.ToString(st.Rows[0]["Display"]);

                if (Display.Text == "")
                {
                    Display.Text = "0";
                }


                Damage.Text = Convert.ToString(st.Rows[0]["Damage"]);
                if (Damage.Text == "")
                {
                    Damage.Text = "0";
                }

                Delivery.Text = Convert.ToString(st.Rows[0]["Delivery"]);
                if (Delivery.Text == "")
                {
                    Delivery.Text = "0";
                }

                category.Text = Convert.ToString(st.Rows[0]["Types"]);

                if (category.Text == "")
                {
                    category.Text = "0";
                }


                //pictureBox1.ImageLocation = Path.GetFullPath("131353W24J150-43D6.jpg");
                //pictureBox1.ImageLocation = itemdetails.ToList()[0].imagepath;




            }
            else
            {

                lblitemcode.Text = "0";
                lblproductid.Text = "0";
                lblprice.Text = "0";

                lblrack.Text = "0";
                lbldisplay.Text = "0";


            }
        }

        private void DgvAutoRefNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Up) || e.KeyChar != Convert.ToChar(Keys.Down))
            {
                Txtitem.Focus();
                SendKeys.Send(e.KeyChar.ToString());
                lblhiddenproduct.Text = Txtitem.Text;
                if (DgvAutoRefNo.Rows.Count > 0)
                {
                    DgvAutoRefNo.Rows[0].Cells[0].Selected = false;
                }
            }
        }


        private void ListSearchDate1_Click(object sender, EventArgs e)
        {
            
        }

        private void ListSearchDate2_Click(object sender, EventArgs e)
        {
           
        }

        private void ListSearchDate3_Click(object sender, EventArgs e)
        {
           
        }


        private void Calender()
        {
            
        }

        private void label40_Click(object sender, EventArgs e)
        {
          
        }

        private void lblToday_Click(object sender, EventArgs e)
        {
           
        }
        private void lblThisWeek_Click(object sender, EventArgs e)
        {
            
        }
        private void lblThisMonth_Click(object sender, EventArgs e)
        {
           
        }

        private void lblThisYear_Click(object sender, EventArgs e)
        {
           
        }
        private void lblYesterday_Click(object sender, EventArgs e)
        {
            
        }

        private void lblLastWeek_Click(object sender, EventArgs e)
        {
           
        }

        private void lblLastMonth_Click(object sender, EventArgs e)
        {

        }

        private void lblLastYear_Click(object sender, EventArgs e)
        {
           
        }

        private void SearchFrmDate_ValueChanged(object sender, EventArgs e)
        {
            
        }


        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            GetSearchOrder();
        }
        private void GetSearchOrder()
        {
            string OrderNo = txtOrderNo.Text.Trim();
            string order = string.Empty;
            order = Convert.ToString(cmbstatus3.SelectedValue);
            if (order != "-Select-")
            {
                order = Convert.ToString(cmbstatus3.SelectedValue);
            }

            else
            {
                order = null;
            }
            DateTime FromDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
            DateTime ToDate = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);

            string Product = txtSearchProduct.Text.Trim();
            string Quty = string.Empty;

            Quty = textSearchQty.Text.Trim();
            if (Quty != "")
            {
                Quty = textSearchQty.Text.Trim();
            }
            else
            {
                Quty = null;
            }

            search(OrderNo, order, FromDate, ToDate, Product, Quty, Program.userid);
        }


        private void MergeGetSearchOrder()
        {
            string OrderNo = txtOrderNo.Text.Trim();
            string order = string.Empty;
            order = Convert.ToString(cmbstatus3.SelectedValue);
            if (order != "")
            {
                order = Convert.ToString(cmbstatus3.SelectedValue);
            }

            else
            {
                order = null;
            }
            DateTime FromDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day);
            DateTime ToDate = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day);

            string Product = txtSearchProduct.Text.Trim();
            string Quty = string.Empty;

            Quty = textSearchQty.Text.Trim();
            if (Quty != "")
            {
                Quty = textSearchQty.Text.Trim();
            }
            else
            {
                Quty = null;
            }

            Mergesearch(order, FromDate, ToDate, Product, Quty, Program.userid);
        }
        private void dgvOrder_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if(e.ColumnIndex==4)
            //{
            //    if (dgvOrder.ReadOnly == false)
            //    {
            //        if (dgvOrder.Columns[4].ReadOnly == true)
            //        {
            //            pnlless.Visible = true;
            //            txtlesspwd.Focus();
            //        }
            //    }
            //}
        }



        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtlesspwd.Text))
            {
                MessageBox.Show("Password should not be empty");
                this.ActiveControl = txtlesspwd;
                return;
            }
            DataTable dt = LoginBAL.GetLesserDetials(txtlesspwd.Text, "PRICE");
            if (dt.Rows.Count > 0)
            {
                txtlesspwd.Text = string.Empty;
                pnlless.Visible = false;
                this.dgvOrder.Columns["Rate"].ReadOnly = false;
                dgvOrder.Focus();
                dgvOrder.CurrentCell = dgvOrder[4, 0];
                dgvOrder.BeginEdit(true);
            }
            else
            {
                MessageBox.Show("Authentication Failed");
                this.dgvOrder.Columns["Rate"].ReadOnly = true;
                txtlesspwd.Focus();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.dgvOrder.Columns["Rate"].ReadOnly = true;
            pnlless.Visible = false;
            txtlesspwd.Clear();
        }

        private void Btnmobilenumber_Click(object sender, EventArgs e)
        {

        }

        private void txtlesspwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void btnLess_Click(object sender, EventArgs e)
        {
            pnlless.Visible = true;
            txtlesspwd.Focus();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            // GetReport(QuotationId);
        }

        private void pcupdate_Click(object sender, EventArgs e)
        {
            Updated u = new Updated();
            u.ShowDialog();
        }



        private void dgvSearch_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyData == Keys.Down)
                {
                    if (dgvSearch.CurrentCell.RowIndex >= 0)
                    {
                        string s = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex + 1].Cells[0].Value);
                        if (!string.IsNullOrEmpty(s))
                        {


                            getquotaion(s);
                            total();

                        }
                        else
                        {
                            clear();
                        }

                    }
                }

                if (e.KeyData == Keys.Up)
                {
                    if (dgvSearch.CurrentCell.RowIndex >= 0)
                    {
                        string s = Convert.ToString(dgvSearch.Rows[dgvSearch.CurrentCell.RowIndex - 1].Cells[0].Value);
                        if (!string.IsNullOrEmpty(s))
                        {
                            getquotaion(s);
                            total();
                        }
                        else
                        {
                            clear();
                        }

                    }
                }
            }
            catch
            {

            }
        }

        private void btnmerge_Click(object sender, EventArgs e)
        {

        }


        public void getquotaionmerge(string data)
        {
            this.dgvOrder.Columns["Amount"].ReadOnly = true;
            this.dgvOrder.Columns[0].ReadOnly = true;
            this.dgvOrder.Columns["Items"].ReadOnly = true;
            this.dgvOrder.Columns["UOM"].ReadOnly = true;
            this.dgvOrder.Columns["productid"].ReadOnly = true;
            this.dgvOrder.Columns["Rate"].ReadOnly = true;
            DataTable ds = objQuotationbal.getquotaionmerge(data);

            string s1 = string.Empty;
            double qty;
            dgvOrder.Rows.Clear();
            this.dgvOrder.Columns["Rate"].ReadOnly = true;
            btnLess.Enabled = true;
            for (int i = 0; i < ds.Rows.Count; i++)
            {
                dgvOrder.Rows.Add();
                dgvOrder.Rows[i].Cells[0].Value = i + 1;
                dgvOrder.Rows[i].Cells[1].Value = Convert.ToString(ds.Rows[i]["DisplayName"]);
                dgvOrder.Rows[i].Cells[2].Value = Convert.ToString(ds.Rows[i]["UOM"]);
                dgvOrder.Rows[i].Cells[3].Value = Convert.ToString(ds.Rows[i]["Productid"]);
                s1 = Convert.ToString(ds.Rows[i]["Rate"]);
                if (string.IsNullOrEmpty(s1))
                {
                    qty = 0;
                }
                else
                {
                    qty = Convert.ToDouble(s1);
                }

                dgvOrder.Rows[i].Cells[4].Value = qty;
                dgvOrder.Rows[i].Cells[5].Value = Convert.ToString(ds.Rows[i]["Quantity"]);
                double amt = Convert.ToDouble(ds.Rows[i]["Amount"]);
                dgvOrder.Rows[i].Cells[6].Value = amt;


            }


        }

        private void clear1()
        {


            btnLess.Enabled = false;
            pnsearch.Visible = false;
            btnSavePending.Enabled = true;
            btnSave.Enabled = true;
            btnPrint.Enabled = false;
            currentDeliveryNoteId = 0;
            ApplyDefaultBranchSelection();
            cmdcity.Text = string.Empty;
            if (txtReference != null)
                txtReference.Text = string.Empty;
            ClearDeliveryHeaderFields();
            SafeResetCombo(cmbassistby);
            SafeResetCombo(cmbreference);
            txtorder.Clear();
            followupdate.Checked = false;
            followupdate.Value = DateTime.Today;
            txtFollowupPhone.Text = string.Empty;
            dgvOrder.Rows.Clear();
            lblperare.Text = Program.Userfullname;
            lbltotalquantity.Text = "0";
            lbltotalamount.Text = "0";
            cmbcustomername.Focus();
            SafeResetCombo(cmbloaction);
            cmbstatus.Text = "Open";
            pnenabletrue();
            var cntls = GetAll(this, typeof(RadioButton));
            foreach (Control cntrl in cntls)
            {
                RadioButton _rb = (RadioButton)cntrl;
                if (_rb.Text != "New")
                {
                    if (_rb.Checked)
                    {
                        _rb.Checked = false;
                    }
                }
                else
                {
                    _rb.Checked = true;
                }
            }
            this.dgvOrder.Columns["Rate"].ReadOnly = true;
        }

        private void dgvSearch_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == dgvSearch.Columns[3].Index)
            {
                dgvSearch.EndEdit();  //Stop editing of cell.


                var data = "";

                List<DataGridViewRow> selectedRows = (from row in dgvSearch.Rows.Cast<DataGridViewRow>()
                                                      where Convert.ToBoolean(row.Cells["checkBoxColumn"].Value) == true
                                                      select row).ToList();



                foreach (DataGridViewRow row in selectedRows)
                {

                    data += ",";
                    data += row.Cells[0].Value;

                }
                getquotaionmerge(data);


            }
        }

        private void btnmerge_Click_1(object sender, EventArgs e)
        {
            clear1();
            MergeGetSearchOrder();
            label27.Text = "1";
            this.dgvSearch.Columns[3].Visible = false;
            btnmerge.Visible = false;

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textSearchQty_TextChanged(object sender, EventArgs e)
        {

        }

        private void textSearchQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && (e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void productsearchbttn_Click(object sender, EventArgs e)
        {
            pnlprodsearch.Visible = false;
        }
    }
}
