using System.ComponentModel;

namespace RRE_To_Tally;

public sealed class FrmUserAccess : Form
{
    private readonly TallyAuthRepository _authRepository = new TallyAuthRepository();
    private readonly UserSession _adminUser;
    private BindingList<UserAccessRow> _users = new BindingList<UserAccessRow>();
    private DataGridView dgvUsers = new DataGridView();
    private Button btnSave = new Button();
    private Button btnClose = new Button();
    private Label lblStatus = new Label();

    public FrmUserAccess(UserSession adminUser)
    {
        _adminUser = adminUser;
        InitializeComponent();
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await LoadUsersAsync().ConfigureAwait(true);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        BackColor = Color.FromArgb(236, 240, 245);
        ClientSize = new Size(820, 520);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(700, 450);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Tally Export User Access";

        Panel header = new Panel { BackColor = Color.FromArgb(12, 74, 110), Dock = DockStyle.Top, Height = 56 };
        Label title = new Label { AutoSize = true, Text = "User Access", Font = new Font("Segoe UI", 15F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(18, 14) };
        header.Controls.Add(title);

        dgvUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvUsers.AutoGenerateColumns = false;
        dgvUsers.BackgroundColor = Color.White;
        dgvUsers.BorderStyle = BorderStyle.None;
        dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvUsers.Location = new Point(18, 76);
        dgvUsers.RowHeadersVisible = false;
        dgvUsers.Size = new Size(784, 365);
        dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Access", DataPropertyName = "HasAccess", Width = 70 });
        dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "User ID", DataPropertyName = "UserId", Width = 70, ReadOnly = true });
        dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "User Name", DataPropertyName = "UserName", Width = 170, ReadOnly = true });
        dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Full Name", DataPropertyName = "UserFullName", Width = 260, ReadOnly = true });
        dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Role", DataPropertyName = "Role", Width = 150, ReadOnly = true });

        btnSave = MakeButton("Save Access", 590, 458, 105);
        btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnSave.Click += BtnSave_Click;
        btnClose = MakeButton("Close", 704, 458, 98);
        btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnClose.BackColor = Color.FromArgb(100, 116, 139);
        btnClose.Click += delegate { Close(); };
        lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblStatus.AutoEllipsis = true;
        lblStatus.Location = new Point(18, 463);
        lblStatus.Size = new Size(540, 23);
        lblStatus.ForeColor = Color.FromArgb(30, 41, 59);

        Controls.Add(header);
        Controls.Add(dgvUsers);
        Controls.Add(lblStatus);
        Controls.Add(btnSave);
        Controls.Add(btnClose);
        ResumeLayout(false);
    }

    private async Task LoadUsersAsync()
    {
        SetBusy(true);
        lblStatus.Text = "Loading users...";
        try
        {
            List<UserAccessRow> rows = await _authRepository.LoadUsersForAccessAsync().ConfigureAwait(true);
            foreach (UserAccessRow row in rows)
            {
                if (string.Equals(row.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    row.HasAccess = true;
                }
            }
            _users = new BindingList<UserAccessRow>(rows);
            dgvUsers.DataSource = _users;
            lblStatus.Text = "Loaded " + rows.Count + " users.";
        }
        catch (Exception ex)
        {
            lblStatus.Text = ex.Message;
            MessageBox.Show(this, ex.Message, "User Access", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        dgvUsers.EndEdit();
        SetBusy(true);
        lblStatus.Text = "Saving access...";
        try
        {
            await _authRepository.SaveUserAccessAsync(_users.ToList(), _adminUser).ConfigureAwait(true);
            lblStatus.Text = "Access saved.";
            MessageBox.Show(this, "User access saved.", "User Access", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            lblStatus.Text = ex.Message;
            MessageBox.Show(this, ex.Message, "User Access", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool busy)
    {
        btnSave.Enabled = !busy;
        btnClose.Enabled = !busy;
        dgvUsers.Enabled = !busy;
    }

    private static Button MakeButton(string text, int x, int y, int width)
    {
        Button button = new Button { Text = text, Location = new Point(x, y), Size = new Size(width, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(20, 184, 166), ForeColor = Color.White };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }
}
