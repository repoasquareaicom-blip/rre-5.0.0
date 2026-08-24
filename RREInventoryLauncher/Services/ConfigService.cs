using System;
using System.IO;
using System.Runtime.Serialization.Json;
using RREInventoryLauncher.Models;

namespace RREInventoryLauncher.Services
{
    public class ConfigService
    {
        public string ConfigPath { get; private set; }

        public ConfigService()
        {
            ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "launcher.config.json");
        }

        public LauncherConfig Load()
        {
            if (!File.Exists(ConfigPath))
            {
                throw new FileNotFoundException("Launcher config file was not found.", ConfigPath);
            }

            using (FileStream stream = File.OpenRead(ConfigPath))
            {
                var serializer = new DataContractJsonSerializer(typeof(LauncherConfig));
                var config = (LauncherConfig)serializer.ReadObject(stream);
                Validate(config);
                return config;
            }
        }

        private static void Validate(LauncherConfig config)
        {
            if (config == null)
            {
                throw new InvalidOperationException("Launcher configuration is empty.");
            }

            Require(config.BranchCode, "BranchCode");
            Require(config.ServerDeploymentPath, "ServerDeploymentPath");
            Require(config.ServerVersionFile, "ServerVersionFile");
            Require(config.LocalApplicationPath, "LocalApplicationPath");
            Require(config.InventoryExe, "InventoryExe");
            Require(config.LogPath, "LogPath");
        }

        private static void Require(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException("Missing launcher configuration value: " + name);
            }
        }
    }
}
