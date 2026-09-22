using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory
{
    public class PrinterSetupForm : Form
    {
        private RadioButton radioLpt;
        private RadioButton radioUsb;
        private ComboBox cboPort;
        private ComboBox cboPrinter;
        private Label lblNote;

        public PrinterSetupForm()
        {
            InitializeComponent();
            LoadPrinters();
            LoadCurrentSettings();
            ApplyMode();
        }

        private void InitializeComponent()
        {
            this.Text = "Printer Setup";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.ClientSize = new Size(480, 248);
            this.Font = new Font("Microsoft Sans Serif", 9F);

            Label lblMode = new Label();
            lblMode.Text = "Print Mode";
            lblMode.Location = new Point(12, 12);
            lblMode.AutoSize = true;

            radioLpt = new RadioButton();
            radioLpt.Text = "LPT";
            radioLpt.Location = new Point(12, 34);
            radioLpt.AutoSize = true;
            radioLpt.Checked = true;
            radioLpt.CheckedChanged += new EventHandler(ModeChanged);

            radioUsb = new RadioButton();
            radioUsb.Text = "USB";
            radioUsb.Location = new Point(90, 34);
            radioUsb.AutoSize = true;
            radioUsb.CheckedChanged += new EventHandler(ModeChanged);

            Label lblPort = new Label();
            lblPort.Text = "LPT Port";
            lblPort.Location = new Point(12, 68);
            lblPort.AutoSize = true;

            cboPort = new ComboBox();
            cboPort.DropDownStyle = ComboBoxStyle.DropDown;
            cboPort.Location = new Point(12, 88);
            cboPort.Size = new Size(120, 23);
            cboPort.Items.Add("LPT1");
            cboPort.Items.Add("LPT2");
            cboPort.Items.Add("LPT3");
            cboPort.Text = "LPT1";

            Label lblPrinter = new Label();
            lblPrinter.Text = "Windows Printer";
            lblPrinter.Location = new Point(160, 68);
            lblPrinter.AutoSize = true;

            cboPrinter = new ComboBox();
            cboPrinter.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPrinter.Location = new Point(160, 88);
            cboPrinter.Size = new Size(304, 23);

            lblNote = new Label();
            lblNote.Location = new Point(12, 124);
            lblNote.Size = new Size(452, 48);
            lblNote.Text = "LPT still prints through d:\\bill.txt and d:\\Bill.bat. The port saved here is kept for later. Bill.bat chooses the port.";

            Button btnTest = new Button();
            btnTest.Text = "Test Print";
            btnTest.Location = new Point(12, 204);
            btnTest.Size = new Size(90, 28);
            btnTest.Click += new EventHandler(TestClicked);

            Button btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Location = new Point(286, 204);
            btnSave.Size = new Size(84, 28);
            btnSave.Click += new EventHandler(SaveClicked);

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(378, 204);
            btnCancel.Size = new Size(84, 28);
            btnCancel.DialogResult = DialogResult.Cancel;

            this.CancelButton = btnCancel;
            this.Controls.Add(lblMode);
            this.Controls.Add(radioLpt);
            this.Controls.Add(radioUsb);
            this.Controls.Add(lblPort);
            this.Controls.Add(cboPort);
            this.Controls.Add(lblPrinter);
            this.Controls.Add(cboPrinter);
            this.Controls.Add(lblNote);
            this.Controls.Add(btnTest);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void LoadPrinters()
        {
            cboPrinter.Items.Clear();
            System.Drawing.Printing.PrinterSettings.StringCollection installed =
                System.Drawing.Printing.PrinterSettings.InstalledPrinters;

            for (int i = 0; i < installed.Count; i++)
            {
                cboPrinter.Items.Add(installed[i]);
            }
        }

        private void LoadCurrentSettings()
        {
            DotMatrixPrinterSettings settings;
            string error;
            if (!DotMatrixPrinter.TryLoadSettings(out settings, out error) || settings == null)
            {
                radioLpt.Checked = true;
                cboPort.Text = "LPT1";
                return;
            }

            if (settings.PrintMode == DotMatrixPrinter.PrintModeUsb)
            {
                radioUsb.Checked = true;
            }
            else
            {
                radioLpt.Checked = true;
            }

            if (!string.IsNullOrEmpty(settings.LptPort))
            {
                cboPort.Text = settings.LptPort;
            }

            if (!string.IsNullOrEmpty(settings.PrinterName))
            {
                string installed = DotMatrixPrinter.FindInstalledPrinter(settings.PrinterName);
                if (installed != null)
                {
                    cboPrinter.SelectedItem = installed;
                }
            }
        }

        private void ModeChanged(object sender, EventArgs e)
        {
            ApplyMode();
        }

        private void ApplyMode()
        {
            bool lpt = radioLpt.Checked;
            cboPort.Enabled = lpt;
            cboPrinter.Enabled = !lpt;
            if (lpt)
            {
                lblNote.Text = "LPT still prints through d:\\bill.txt and d:\\Bill.bat. The port saved here is kept for later. Bill.bat chooses the port.";
            }
            else
            {
                lblNote.Text = "USB sends the same raw text and control codes to the selected Windows printer.";
            }
        }

        private void TestClicked(object sender, EventArgs e)
        {
            string error;
            DotMatrixPrinterSettings settings = ReadForm(out error);
            if (settings == null)
            {
                MessageBox.Show(this, error, "Printer Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DotMatrixPrinter.TrySend(settings, DotMatrixPrinter.BuildTestPage(), "Dot Matrix Test"))
            {
                MessageBox.Show(this, "Test print was sent.", "Printer Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SaveClicked(object sender, EventArgs e)
        {
            string error;
            DotMatrixPrinterSettings settings = ReadForm(out error);
            if (settings == null)
            {
                MessageBox.Show(this, error, "Printer Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!DotMatrixPrinter.TrySaveSettings(settings, out error))
            {
                MessageBox.Show(this, error, "Printer Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private DotMatrixPrinterSettings ReadForm(out string error)
        {
            error = null;
            DotMatrixPrinterSettings settings = new DotMatrixPrinterSettings();
            if (radioUsb.Checked)
            {
                settings.PrintMode = DotMatrixPrinter.PrintModeUsb;
                settings.LptPort = string.IsNullOrEmpty(cboPort.Text) ? "LPT1" : cboPort.Text.Trim();
                if (cboPrinter.SelectedItem == null || cboPrinter.SelectedItem.ToString().Trim().Length == 0)
                {
                    error = "Select a Windows printer for USB printing.";
                    return null;
                }

                settings.PrinterName = cboPrinter.SelectedItem.ToString();
                return settings;
            }

            settings.PrintMode = DotMatrixPrinter.PrintModeLpt;
            settings.PrinterName = cboPrinter.SelectedItem == null ? "" : cboPrinter.SelectedItem.ToString();
            string port = cboPort.Text == null ? "" : cboPort.Text.Trim();
            if (port.Length == 0)
            {
                error = "Enter an LPT port, for example LPT1.";
                return null;
            }

            settings.LptPort = port;
            return settings;
        }
    }
}
