namespace RRE_To_Tally;

public sealed class FrmLogin : Form
{
    private readonly TallyAuthRepository _authRepository = new TallyAuthRepository();
    private TextBox txtUserName = new TextBox();
    private TextBox txtPassword = new TextBox();
    private Button btnLogin = new Button();
    private Button btnCancel = new Button();
    private Label lblStatus = new Label();
    private Panel pnlStatus = new Panel();

    public UserSession? AuthenticatedUser { get; private set; }

    public FrmLogin()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        BackColor = Color.FromArgb(248, 250, 252);
        ClientSize = new Size(760, 440);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "RRE Tally Export Login";

        Panel leftPanel = new Panel { BackColor = Color.FromArgb(12, 74, 110), Dock = DockStyle.Left, Width = 300 };
        Label brand = new Label { AutoSize = true, Text = "RRE", Font = new Font("Segoe UI", 30F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(36, 54) };
        Label product = new Label { AutoSize = true, Text = "Tally Sales XML Export", Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(224, 242, 254), Location = new Point(40, 122) };
        Label line1 = new Label { Text = "Secure export access for approved users.", Font = new Font("Segoe UI", 10F), ForeColor = Color.FromArgb(186, 230, 253), Location = new Point(42, 170), Size = new Size(220, 48) };
        Panel accent = new Panel { BackColor = Color.FromArgb(20, 184, 166), Location = new Point(42, 240), Size = new Size(86, 5) };
        Label version = new Label { AutoSize = true, Text = "Standalone utility", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(186, 230, 253), Location = new Point(42, 364) };
        leftPanel.Controls.Add(brand);
        leftPanel.Controls.Add(product);
        leftPanel.Controls.Add(line1);
        leftPanel.Controls.Add(accent);
        leftPanel.Controls.Add(version);

        Label title = new Label { AutoSize = true, Text = "Sign in", Font = new Font("Segoe UI", 24F, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(356, 54) };
        Label subtitle = new Label { AutoSize = true, Text = "Use your existing Inventory user credentials.", Font = new Font("Segoe UI", 10F), ForeColor = Color.FromArgb(71, 85, 105), Location = new Point(360, 102) };

        Label lblUser = MakeLabel("User Name", 362, 148);
        txtUserName.Location = new Point(362, 174);
        txtUserName.Size = new Size(330, 29);
        txtUserName.Name = "txtUserName";
        txtUserName.BorderStyle = BorderStyle.FixedSingle;
        txtUserName.Font = new Font("Segoe UI", 11F);

        Label lblPassword = MakeLabel("Password", 362, 224);
        txtPassword.Location = new Point(362, 250);
        txtPassword.Size = new Size(330, 29);
        txtPassword.Name = "txtPassword";
        txtPassword.BorderStyle = BorderStyle.FixedSingle;
        txtPassword.Font = new Font("Segoe UI", 11F);
        txtPassword.PasswordChar = '*';
        txtPassword.KeyDown += TxtPassword_KeyDown;

        btnLogin = MakeButton("Login", 362, 314, 156);
        btnLogin.Click += BtnLogin_Click;
        btnCancel = MakeButton("Cancel", 536, 314, 156);
        btnCancel.BackColor = Color.FromArgb(100, 116, 139);
        btnCancel.Click += delegate { DialogResult = DialogResult.Cancel; Close(); };

        pnlStatus.BackColor = Color.FromArgb(241, 245, 249);
        pnlStatus.Location = new Point(362, 366);
        pnlStatus.Size = new Size(330, 38);
        pnlStatus.Visible = false;
        lblStatus.AutoEllipsis = true;
        lblStatus.ForeColor = Color.FromArgb(30, 41, 59);
        lblStatus.Location = new Point(12, 9);
        lblStatus.Size = new Size(306, 20);
        pnlStatus.Controls.Add(lblStatus);

        Controls.Add(leftPanel);
        Controls.Add(title);
        Controls.Add(subtitle);
        Controls.Add(lblUser);
        Controls.Add(txtUserName);
        Controls.Add(lblPassword);
        Controls.Add(txtPassword);
        Controls.Add(btnLogin);
        Controls.Add(btnCancel);
        Controls.Add(pnlStatus);
        AcceptButton = btnLogin;
        CancelButton = btnCancel;
        ResumeLayout(false);
        PerformLayout();
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            ShowStatus("Enter user name and password.", true);
            return;
        }

        SetBusy(true);
        ShowStatus("Signing in...", false);
        try
        {
            UserSession? user = await _authRepository.LoginAsync(txtUserName.Text, txtPassword.Text).ConfigureAwait(true);
            if (user == null)
            {
                ShowStatus("Authentication failed.", true);
                return;
            }

            bool hasAccess = await _authRepository.HasTallyAccessAsync(user).ConfigureAwait(true);
            if (!hasAccess)
            {
                ShowStatus("Access not assigned for this user.", true);
                return;
            }

            AuthenticatedUser = user;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            ShowStatus(ex.Message, true);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void TxtPassword_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            btnLogin.PerformClick();
        }
    }

    private void SetBusy(bool busy)
    {
        btnLogin.Enabled = !busy;
        btnCancel.Enabled = !busy;
        txtUserName.Enabled = !busy;
        txtPassword.Enabled = !busy;
    }

    private void ShowStatus(string message, bool isError)
    {
        pnlStatus.Visible = true;
        pnlStatus.BackColor = isError ? Color.FromArgb(254, 226, 226) : Color.FromArgb(224, 242, 254);
        lblStatus.ForeColor = isError ? Color.FromArgb(127, 29, 29) : Color.FromArgb(12, 74, 110);
        lblStatus.Text = message;
    }

    private static Label MakeLabel(string text, int x, int y)
    {
        return new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(30, 41, 59), Location = new Point(x, y), Text = text };
    }

    private static Button MakeButton(string text, int x, int y, int width)
    {
        Button button = new Button { Text = text, Location = new Point(x, y), Size = new Size(width, 36), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(20, 184, 166), ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }
}
