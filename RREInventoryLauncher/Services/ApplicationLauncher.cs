using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using RREInventoryLauncher.Models;

namespace RREInventoryLauncher.Services
{
    public class ApplicationLauncher
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SwRestore = 9;

        public bool BringRunningInventoryToFront(string inventoryExe)
        {
            string processName = Path.GetFileNameWithoutExtension(inventoryExe);
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                if (process.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }

                ShowWindow(process.MainWindowHandle, SwRestore);
                SetForegroundWindow(process.MainWindowHandle);
                return true;
            }

            return processes.Length > 0;
        }

        public void LaunchInventory(LauncherConfig config)
        {
            string exePath = Path.Combine(config.LocalApplicationPath, config.InventoryExe);
            if (!File.Exists(exePath))
            {
                throw new FileNotFoundException("Inventory executable was not found.", exePath);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = config.LocalApplicationPath,
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }
    }
}
