using System;
using System.IO;

namespace RREInventoryLauncher.Services
{
    public class LogService
    {
        private readonly object syncRoot = new object();
        private string logDirectory;

        public LogService(string logDirectory)
        {
            SetLogDirectory(logDirectory);
        }

        public void SetLogDirectory(string path)
        {
            logDirectory = string.IsNullOrWhiteSpace(path)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "RRE", "Logs")
                : path;
        }

        public void Info(string message)
        {
            Write("INFO", message, null);
        }

        public void Error(string message, Exception ex)
        {
            Write("ERROR", message, ex);
        }

        private void Write(string level, string message, Exception ex)
        {
            try
            {
                Directory.CreateDirectory(logDirectory);
                string file = Path.Combine(logDirectory, "launcher-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [" + level + "] " + message;
                if (ex != null)
                {
                    line += Environment.NewLine + ex.GetType().Name + ": " + ex.Message;
                }

                lock (syncRoot)
                {
                    File.AppendAllText(file, line + Environment.NewLine);
                }
            }
            catch
            {
                // Logging must never prevent Inventory from launching.
            }
        }
    }
}
