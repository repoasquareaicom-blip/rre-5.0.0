using System;
using System.IO;
using System.Text;
using System.Net;
using System.Configuration;
using System.Threading;

namespace BranchSync
{
    class Program
    {
        static int Main(string[] args)
        {
            // ensure any unhandled exceptions are logged
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                try { Log("Unhandled exception: " + (e.ExceptionObject ?? "(null)").ToString()); } catch { }
            };

            // create log directory immediately and write startup entry
            try { var d = LogDirectory; if (!Directory.Exists(d)) Directory.CreateDirectory(d); Log("=== BranchSync process starting ==="); } catch { }
            // identify which source file built into this exe
            try
            {
                Log("Assembly location: " + System.Reflection.Assembly.GetExecutingAssembly().Location);
                Log("Assembly last write time: " + File.GetLastWriteTimeUtc(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString("o"));
            }
            catch { }

            Console.WriteLine("BranchSync started"); // startup
            Thread.Sleep(1000);
            return 0;
        }

        private static readonly string LogDirectory = "logs";

        static void RunSyncOnce(bool consoleOutput)
        {
            Log("=== Run START ===");
            if (consoleOutput) Console.WriteLine("Run START");
            // heartbeat so we can see that RunSyncOnce actually started
            Log("RunSyncOnce heartbeat");
        }

        static void Log(string message)
        {
            try
            {
                var file = Path.Combine(LogDirectory, DateTime.UtcNow.ToString("yyyy-MM-dd") + ".log");
                var dir = Path.GetDirectoryName(file);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var line = DateTime.UtcNow.ToString("o") + " " + message + Environment.NewLine;
                File.AppendAllText(file, line, Encoding.UTF8);
            }
            catch
            {
                // ignore logging failures
            }
        }
    }
}
