using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using RREInventoryLauncher.Models;
using RREInventoryLauncher.Services;

namespace RREInventoryLauncher.Forms
{
    public class LauncherForm : Form
    {
        private readonly ConfigService configService = new ConfigService();
        private readonly VersionService versionService = new VersionService();
        private readonly ApplicationLauncher applicationLauncher = new ApplicationLauncher();

        private LogService logService = new LogService(null);

        private Label statusLabel;
        private Label localVersionValue;
        private Label serverVersionValue;
        private Label branchValue;
        private ProgressBar progressBar;
        private Button continueButton;

        private LauncherConfig config;

        public LauncherForm()
        {
            InitializeComponent();
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await StartLauncherFlowAsync();
        }

        private void InitializeComponent()
        {
            Text = "RRE Inventory Launcher";
            StartPosition = FormStartPosition.CenterScreen;

            Size = new Size(520, 260);
            MinimumSize = new Size(520, 260);
            MaximumSize = new Size(520, 260);

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var title = new Label
            {
                Text = "R.R. ELECTRICAL AGENCIES",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 14),
                Size = new Size(470, 28)
            };

            var subtitle = new Label
            {
                Text = "Inventory System",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 42),
                Size = new Size(470, 22)
            };

            branchValue = AddValueRow(
                "Branch",
                "Loading...",
                76);

            localVersionValue = AddValueRow(
                "Local Version",
                "-",
                102);

            serverVersionValue = AddValueRow(
                "Server Version",
                "-",
                128);

            var statusCaption = new Label
            {
                Text = "Status:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(62, 158),
                Size = new Size(110, 20)
            };

            statusLabel = new Label
            {
                Text = "Checking application...",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Location = new Point(176, 158),
                Size = new Size(290, 20)
            };

            progressBar = new ProgressBar
            {
                Location = new Point(64, 184),
                Size = new Size(345, 18),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 20
            };

            continueButton = new Button
            {
                Text = "Continue With Current Version",
                Location = new Point(240, 178),
                Size = new Size(190, 28),
                Visible = false
            };

            continueButton.Click += ContinueButton_Click;

            Controls.Add(title);
            Controls.Add(subtitle);
            Controls.Add(statusCaption);
            Controls.Add(statusLabel);
            Controls.Add(progressBar);
            Controls.Add(continueButton);
        }

        private Label AddValueRow(
            string caption,
            string value,
            int top)
        {
            Controls.Add(
                new Label
                {
                    Text = caption + " :",
                    Font = new Font(
                        "Segoe UI",
                        9F,
                        FontStyle.Regular),

                    Location = new Point(
                        112,
                        top),

                    Size = new Size(
                        120,
                        20)
                });

            var valueLabel = new Label
            {
                Text = value,

                Font = new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Bold),

                Location = new Point(
                    240,
                    top),

                Size = new Size(
                    220,
                    20)
            };

            Controls.Add(valueLabel);

            return valueLabel;
        }

        private async Task StartLauncherFlowAsync()
        {
            try
            {
                SetStatus(
                    "Loading configuration...");

                config =
                    configService.Load();

                logService.SetLogDirectory(
                    config.LogPath);

                logService.Info(
                    "Launcher started");

                logService.Info(
                    "BranchCode: " +
                    config.BranchCode);

                logService.Info(
                    "Local application path: " +
                    config.LocalApplicationPath);

                logService.Info(
                    "Server deployment path: " +
                    config.ServerDeploymentPath);

                logService.Info(
                    "Server version file path: " +
                    config.ServerVersionFile);

                branchValue.Text =
                    config.BranchCode;

                await CheckVersionsAndLaunchAsync();
            }
            catch (Exception ex)
            {
                logService.Error(
                    "Launcher configuration is missing or invalid.",
                    ex);

                ShowFinalMessage(
                    "Launcher configuration is missing or invalid.\r\n\r\n" +
                    "Please contact support.");
            }
        }

        private async Task CheckVersionsAndLaunchAsync()
        {
            // ---------------------------------------------------------
            // CHECK LOCAL INVENTORY
            // ---------------------------------------------------------

            SetStatus(
                "Checking local Inventory...");

            bool localExists =
                versionService.LocalInventoryExists(
                    config);

            Version localVersion = null;

            if (localExists)
            {
                localVersion =
                    versionService.GetLocalVersion(
                        config);
            }

            localVersionValue.Text =
                versionService.FormatVersion(
                    localVersion);

            logService.Info(
                "Local version: " +
                localVersionValue.Text);

            // ---------------------------------------------------------
            // READ SERVER VERSION
            // ---------------------------------------------------------

            SetStatus(
                "Checking update server...");

            Version serverVersion = null;

            try
            {
                logService.Info(
                    "Reading server version file: " +
                    config.ServerVersionFile);

                serverVersion =
                    versionService.GetServerVersion(
                        config);

                serverVersionValue.Text =
                    versionService.FormatVersion(
                        serverVersion);

                logService.Info(
                    "Server version: " +
                    serverVersionValue.Text);
            }
            catch (Exception ex)
            {
                serverVersionValue.Text =
                    "Unavailable";

                logService.Error(
                    "Update server unavailable.",
                    ex);

                // -----------------------------------------------------
                // SERVER OFFLINE BUT LOCAL APP EXISTS
                // -----------------------------------------------------

                if (localExists)
                {
                    SetStatus(
                        "Update server unavailable. Starting installed Inventory...");

                    await LaunchAndCloseAsync();

                    return;
                }

                // -----------------------------------------------------
                // NO LOCAL APP + NO SERVER
                // -----------------------------------------------------

                ShowFinalMessage(
                    "Inventory application could not be installed or started.\r\n\r\n" +
                    "Please contact support.");

                return;
            }

            // ---------------------------------------------------------
            // FIRST-TIME INSTALLATION
            // ---------------------------------------------------------

            if (!localExists)
            {
                logService.Info(
                    "Local Inventory not found. Starting first-time installation.");

                SetStatus(
                    "Installing Inventory...");

                try
                {
                    await Task.Run(
                        () =>
                            SafeUpdateApplication(
                                serverVersion,
                                true));
                }
                catch (Exception ex)
                {
                    logService.Error(
                        "Inventory initial installation failed.",
                        ex);

                    ShowFinalMessage(
                        "Inventory application could not be installed.\r\n\r\n" +
                        "Please contact support.");

                    return;
                }

                // Refresh local version after successful installation
                localVersion =
                    versionService.GetLocalVersion(
                        config);

                localVersionValue.Text =
                    versionService.FormatVersion(
                        localVersion);

                SetStatus(
                    "Installation completed. Starting Inventory...");

                await LaunchAndCloseAsync();

                return;
            }

            // ---------------------------------------------------------
            // LOCAL APP EXISTS BUT VERSION NOT DETECTED
            // ---------------------------------------------------------

            if (localVersion == null)
            {
                logService.Info(
                    "Local Inventory version could not be detected.");

                ShowFinalMessage(
                    "Installed Inventory version could not be detected.\r\n\r\n" +
                    "Please contact support.");

                return;
            }

            // ---------------------------------------------------------
            // UPDATE REQUIRED
            // ---------------------------------------------------------

            if (
                serverVersion.CompareTo(
                    localVersion) > 0)
            {
                logService.Info(
                    "New version detected. Current: " +
                    localVersion +
                    ", Available: " +
                    serverVersion);

                if (
                    applicationLauncher
                    .IsInventoryRunning(
                        config.InventoryExe))
                {
                    logService.Info(
                        "Inventory update blocked because Inventory is currently running.");

                    ShowFinalMessage(
                        "A new version of Inventory is available.\r\n\r\n" +
                        "Please close all Inventory windows and start the launcher again to complete the update.");

                    await CloseAfterDelayAsync();

                    return;
                }

                SetStatus(
                    "Updating Inventory to " +
                    versionService.FormatVersion(
                        serverVersion) +
                    "...");

                try
                {
                    await Task.Run(
                        () =>
                            SafeUpdateApplication(
                                serverVersion,
                                false));
                }
                catch (Exception ex)
                {
                    logService.Error(
                        "Inventory update failed.",
                        ex);

                    ShowFinalMessage(
                        "Inventory update failed.\r\n\r\n" +
                        "Please contact support.");

                    return;
                }

                // Refresh local version after successful update
                localVersion =
                    versionService.GetLocalVersion(
                        config);

                localVersionValue.Text =
                    versionService.FormatVersion(
                        localVersion);

                SetStatus(
                    "Update completed. Starting Inventory...");

                await LaunchAndCloseAsync();

                return;
            }

            // ---------------------------------------------------------
            // ALREADY UP TO DATE
            // ---------------------------------------------------------

            SetStatus(
                "Application is up to date. Starting Inventory...");

            await LaunchAndCloseAsync();
        }

        private async void ContinueButton_Click(
            object sender,
            EventArgs e)
        {
            continueButton.Enabled =
                false;

            progressBar.Visible =
                true;

            SetStatus(
                "Starting current Inventory version...");

            await LaunchAndCloseAsync();
        }

        private void SafeUpdateApplication(
            Version serverVersion,
            bool firstInstall)
        {
            if (firstInstall)
            {
                logService.Info(
                    "Creating local application directory.");
            }
            else
            {
                logService.Info(
                    "Updating existing Inventory application.");
            }

            logService.Info(
                "Installing application files.");

            CopyDirectory(
                config.ServerDeploymentPath,
                config.LocalApplicationPath);

            ValidateInstalledInventoryExists();

            UpdateLocalVersionFileAfterSuccessfulUpdate(
                serverVersion);

            if (firstInstall)
            {
                logService.Info(
                    "Initial installation completed successfully.");
            }
            else
            {
                logService.Info(
                    "Inventory update completed successfully.");
            }
        }

        private void CopyDirectory(
            string sourceDirectory,
            string destinationDirectory)
        {
            if (
                string.IsNullOrWhiteSpace(
                    sourceDirectory))
            {
                throw new InvalidOperationException(
                    "Server deployment path is empty.");
            }

            if (
                string.IsNullOrWhiteSpace(
                    destinationDirectory))
            {
                throw new InvalidOperationException(
                    "Local Inventory path is empty.");
            }

            if (
                !Directory.Exists(
                    sourceDirectory))
            {
                throw new DirectoryNotFoundException(
                    "Server deployment path was not found: " +
                    sourceDirectory);
            }

            logService.Info(
                "Copying application files from: " +
                sourceDirectory);

            logService.Info(
                "Copying application files to: " +
                destinationDirectory);

            Directory.CreateDirectory(
                destinationDirectory);

            // ---------------------------------------------------------
            // CREATE ALL SUBDIRECTORIES
            // ---------------------------------------------------------

            foreach (
                string directory
                in Directory.GetDirectories(
                    sourceDirectory,
                    "*",
                    SearchOption.AllDirectories))
            {
                string relativePath =
                    directory
                    .Substring(
                        sourceDirectory.Length)
                    .TrimStart(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar);

                string destinationSubDirectory =
                    Path.Combine(
                        destinationDirectory,
                        relativePath);

                Directory.CreateDirectory(
                    destinationSubDirectory);
            }

            // ---------------------------------------------------------
            // COPY ALL FILES
            // ---------------------------------------------------------

            foreach (
                string sourceFile
                in Directory.GetFiles(
                    sourceDirectory,
                    "*",
                    SearchOption.AllDirectories))
            {
                string relativePath =
                    sourceFile
                    .Substring(
                        sourceDirectory.Length)
                    .TrimStart(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar);

                // version.txt is written separately after successful copy
                if (
                    string.Equals(
                        relativePath,
                        "version.txt",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string destinationFile =
                    Path.Combine(
                        destinationDirectory,
                        relativePath);

                string destinationFileDirectory =
                    Path.GetDirectoryName(
                        destinationFile);

                if (
                    !string.IsNullOrEmpty(
                        destinationFileDirectory))
                {
                    Directory.CreateDirectory(
                        destinationFileDirectory);
                }

                File.Copy(
                    sourceFile,
                    destinationFile,
                    true);
            }

            logService.Info(
                "Application files copied successfully.");
        }

        private void ValidateInstalledInventoryExists()
        {
            string inventoryExe =
                versionService.GetInventoryExePath(
                    config);

            if (
                !File.Exists(
                    inventoryExe))
            {
                throw new FileNotFoundException(
                    "Inventory.exe was not found after installation.",
                    inventoryExe);
            }

            logService.Info(
                "Inventory.exe validated successfully: " +
                inventoryExe);
        }

        private void UpdateLocalVersionFileAfterSuccessfulUpdate(
            Version serverVersion)
        {
            string localVersionFile =
                versionService
                .GetLocalVersionFilePath(
                    config);

            versionService.WriteLocalVersion(
                config,
                serverVersion);

            logService.Info(
                "Local version file updated to: " +
                versionService.FormatVersion(
                    serverVersion));

            logService.Info(
                "Local version file: " +
                localVersionFile);
        }

        private async Task LaunchAndCloseAsync()
        {
            await Task.Run(
                () =>
                    applicationLauncher
                    .LaunchInventory(
                        config));

            logService.Info(
                "Inventory launched successfully");

            await CloseAfterDelayAsync();
        }

        private async Task CloseAfterDelayAsync()
        {
            await Task.Delay(
                700);

            Close();
        }

        private void SetStatus(
            string message)
        {
            if (
                InvokeRequired)
            {
                Invoke(
                    new Action<string>(
                        SetStatus),
                    message);

                return;
            }

            statusLabel.Text =
                message;

            statusLabel.Refresh();
        }

        private void ShowFinalMessage(
            string message)
        {
            progressBar.Visible =
                false;

            continueButton.Visible =
                false;

            SetStatus(
                message.Replace(
                    "\r\n",
                    " "));

            MessageBox.Show(
                this,
                message,
                "RRE Inventory Launcher",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
