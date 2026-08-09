
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

            cmbcustomername.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbcustomername.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbcustomername.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbcustomername.SelectedIndexChanged -= new EventHandler(cmbcustomername_SelectedIndexChanged);

            dgvOrder.Top = 128;
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
            if (!receiptMode || txtReference == null || string.IsNullOrEmpty(txtReference.Text.Trim()))
                return DBNull.Value;
            return txtReference.Text.Trim();
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
INSERT INTO dbo." + HeaderTable() + @" (" + NoteNoColumn() + @", " + NoteDateColumn() + @", FromBranchId, ToBranchId, ReferenceNo, Status, EnteredBy, EnteredOn, IsDeleted)
VALUES (@No, @Date, @FromBranchId, @ToBranchId, @ReferenceNo, 'PENDING', @User, GETDATE(), 0);
SELECT CAST(SCOPE_IDENTITY() AS int);", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@No", noteNo);
                        cmd.Parameters.AddWithValue("@Date", date.Value.Date);
                        cmd.Parameters.AddWithValue("@FromBranchId", Convert.ToInt32(cmbcustomername.SelectedValue));
                        cmd.Parameters.AddWithValue("@ToBranchId", Convert.ToInt32(cmbToLocation.SelectedValue));
                        cmd.Parameters.AddWithValue("@ReferenceNo", GetReferenceValue());
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
                    ExecuteDeliveryNonQuery(con, tran, "UPDATE dbo." + HeaderTable() + " SET FromBranchId = @FromBranchId, ToBranchId = @ToBranchId, ReferenceNo = @ReferenceNo, Status = 'APPROVED', ApprovedBy = @User, ApprovedOn = GETDATE() WHERE " + HeaderIdColumn() + " = @Id",
                        new SqlParameter("@FromBranchId", Convert.ToInt32(cmbcustomername.SelectedValue)),
                        new SqlParameter("@ToBranchId", Convert.ToInt32(cmbToLocation.SelectedValue)),
                        new SqlParameter("@ReferenceNo", GetReferenceValue()),
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
            if (receiptMode && txtReference != null && header.Columns.Contains("ReferenceNo"))
                txtReference.Text = Convert.ToString(header.Rows[0]["ReferenceNo"]);

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
                    if (!string.IsNullOrEmpty(Received))
                    {
                        DataTable StockList = new DataTable();
                        using (SqlConnection con = new SqlConnection(Conn))
                        {
                            con.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = con;
                            cmd.CommandText = "LocationstockinPanal";
                            cmd.Parameters.AddWithValue("@Productname", Received);
                            SqlDataAdapter ad = new SqlDataAdapter(cmd);
                            ad.Fill(StockList);
                            con.Close();
                        }
                        double valstock = 0.00;

                        for (int s = 0; s < StockList.Rows.Count; s++)
                        {
                            valstock = valstock + (Convert.ToDouble(StockList.Rows[s][0].ToString()));
                        }

                        if (Convert.ToDouble(Items) > valstock)
                        {
                            double diff = Convert.ToDouble(Items) - Convert.ToDouble(valstock);
                            StockCheck.Rows.Add(Received, valstock, Items, diff);
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
            Font normalFont = new Font("Arial", 9);
            Font smallFont = new Font("Arial", 8);
            Pen linePen = Pens.Black;

            string companyName = BranchValue(fromBranch, "AddressLine1");
            if (string.IsNullOrEmpty(companyName))
                companyName = BranchDisplayName(fromBranch, cmbcustomername);

            g.DrawString(companyName, titleFont, Brushes.Black, left, y);
            y += 26;
            y = DrawBranchAddress(g, fromBranch, normalFont, left, y, true);

            int infoLeft = right - 220;
            int infoY = bounds.Top;
            g.DrawString("DELIVERY NOTE", headerFont, Brushes.Black, infoLeft, infoY);
            infoY += 24;
            DrawLabelValue(g, "No", txtorder.Text, normalFont, infoLeft, infoY);
            infoY += 20;
            DrawLabelValue(g, "Date", date.Value.ToString("dd-MM-yyyy"), normalFont, infoLeft, infoY);
            infoY += 20;
            DrawLabelValue(g, "Status", cmbstatus.Text, normalFont, infoLeft, infoY);

            y = Math.Max(y, infoY + 18);
            g.DrawLine(linePen, left, y, right, y);
            y += 16;

            g.DrawString("To", headerFont, Brushes.Black, left, y);
            y += 18;
            string toName = BranchDisplayName(toBranch, cmbToLocation);
            if (!string.IsNullOrEmpty(toName))
            {
                g.DrawString(toName, headerFont, Brushes.Black, left, y);
                y += 18;
            }
            y = DrawBranchAddress(g, toBranch, normalFont, left, y, false);
            y += 12;

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
            return y;
        }

        private void DrawLabelValue(Graphics g, string label, string value, Font font, int x, int y)
        {
            g.DrawString(label + " :", font, Brushes.Black, x, y);
            g.DrawString(value, font, Brushes.Black, x + 55, y);
        }

        private int DrawDeliveryProductGrid(Graphics g, Rectangle bounds, int y, Font headerFont, Font normalFont)
        {
            int[] widths = new int[] { 45, 360, 80, 90 };
            int rowHeight = 24;
            int x = bounds.Left;
            string[] headers = new string[] { "S.No", "Items", "UOM", "Quantity" };

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
                    quantity
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
