using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using RREInventoryLauncher.Models;

namespace RREInventoryLauncher.Services
{
    public class VersionService
    {
        public Version GetLocalVersion(LauncherConfig config)
        {
            string localVersionFile = Path.Combine(config.LocalApplicationPath, "version.txt");
            if (File.Exists(localVersionFile))
            {
                Version version;
                if (TryParseVersion(File.ReadAllText(localVersionFile), out version))
                {
                    return version;
                }
            }

            string exePath = GetInventoryExePath(config);
            if (!File.Exists(exePath))
            {
                return null;
            }

            FileVersionInfo info = FileVersionInfo.GetVersionInfo(exePath);
            Version fileVersion;
            if (TryParseVersion(info.FileVersion, out fileVersion))
            {
                return fileVersion;
            }

            Version productVersion;
            if (TryParseVersion(info.ProductVersion, out productVersion))
            {
                return productVersion;
            }

            return null;
        }

        public Version GetServerVersion(LauncherConfig config)
        {
            if (!File.Exists(config.ServerVersionFile))
            {
                throw new FileNotFoundException(
                    "Server version file was not found: [" + config.ServerVersionFile + "]",
                    config.ServerVersionFile);
            }

            Version version;
            if (!TryParseVersion(File.ReadAllText(config.ServerVersionFile), out version))
            {
                throw new InvalidOperationException("Server version file is not a valid version.");
            }

            return version;
        }

        public bool LocalInventoryExists(LauncherConfig config)
        {
            return File.Exists(GetInventoryExePath(config));
        }

        public string GetInventoryExePath(LauncherConfig config)
        {
            return Path.Combine(config.LocalApplicationPath, config.InventoryExe);
        }

        public string GetLocalVersionFilePath(LauncherConfig config)
        {
            return Path.Combine(config.LocalApplicationPath, "version.txt");
        }

        public void WriteLocalVersion(LauncherConfig config, Version serverVersion)
        {
            if (serverVersion == null)
            {
                throw new ArgumentNullException("serverVersion");
            }

            Directory.CreateDirectory(config.LocalApplicationPath);
            File.WriteAllText(GetLocalVersionFilePath(config), FormatVersion(serverVersion));
        }

        private static bool TryParseVersion(string value, out Version version)
        {
            version = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string clean = value.Trim();
            int space = clean.IndexOfAny(new[] { ' ', '\r', '\n', '\t' });
            if (space > 0)
            {
                clean = clean.Substring(0, space);
            }

            Version parsed;
            if (Version.TryParse(clean, out parsed))
            {
                version = Normalize(parsed);
                return true;
            }

            return false;
        }

        private static Version Normalize(Version version)
        {
            int build = version.Build < 0 ? 0 : version.Build;
            int revision = version.Revision < 0 ? 0 : version.Revision;
            return new Version(version.Major, version.Minor, build, revision);
        }

        public string FormatVersion(Version version)
        {
            return version == null ? "Not installed" : version.ToString();
        }
    }
}
