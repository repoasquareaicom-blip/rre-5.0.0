using System.Diagnostics;
using System.IO;
using RREInventoryLauncher.Models;

namespace RREInventoryLauncher.Services
{
    public class ApplicationLauncher
    {
        public bool IsInventoryRunning(string inventoryExe)
        {
            string processName = Path.GetFileNameWithoutExtension(inventoryExe);
            return Process.GetProcessesByName(processName).Length > 0;
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
