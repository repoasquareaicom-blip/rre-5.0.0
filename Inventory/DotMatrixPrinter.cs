using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Inventory
{
    public class DotMatrixPrinterSettings
    {
        public string PrintMode;
        public string LptPort;
        public string PrinterName;
    }

    public static class DotMatrixPrinter
    {
        public const string PrintModeLpt = "LPT";
        public const string PrintModeUsb = "USB";

        public static string ConfigFilePath
        {
            get
            {
                return Path.Combine(ConfigDirectory, "DotMatrixPrinter.ini");
            }
        }

        private static string ConfigDirectory
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RR Inventory");
            }
        }

        public static void Print(string printOutput)
        {
            try
            {
                PrintCore(printOutput);
            }
            catch (Exception ex)
            {
                ShowError("Quotation printing failed." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        public static bool ShowPrinterSetup()
        {
            using (PrinterSetupForm form = new PrinterSetupForm())
            {
                Form owner = Form.ActiveForm;
                if (owner != null && owner.Visible && !owner.IsDisposed)
                {
                    return form.ShowDialog(owner) == DialogResult.OK;
                }

                return form.ShowDialog() == DialogResult.OK;
            }
        }

        internal static bool TryLoadSettings(out DotMatrixPrinterSettings settings, out string error)
        {
            settings = null;
            error = null;

            if (!File.Exists(ConfigFilePath))
            {
                error = "Printer configuration was not found." + Environment.NewLine + ConfigFilePath;
                return false;
            }

            try
            {
                string mode = null;
                string port = null;
                string printerName = null;
                bool hasMode = false;
                bool hasPort = false;
                bool hasPrinter = false;

                using (StreamReader reader = new StreamReader(ConfigFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.Length == 0 || line.StartsWith(";") || line.StartsWith("#"))
                        {
                            continue;
                        }

                        int split = line.IndexOf('=');
                        if (split <= 0)
                        {
                            continue;
                        }

                        string key = line.Substring(0, split).Trim();
                        string value = line.Substring(split + 1).Trim();
                        if (string.Compare(key, "PrintMode", true) == 0)
                        {
                            mode = value;
                            hasMode = true;
                        }
                        else if (string.Compare(key, "LptPort", true) == 0)
                        {
                            port = value;
                            hasPort = true;
                        }
                        else if (string.Compare(key, "PrinterName", true) == 0)
                        {
                            printerName = value;
                            hasPrinter = true;
                        }
                    }
                }

                if (!hasMode || !hasPort || !hasPrinter)
                {
                    error = "Printer configuration is incomplete." + Environment.NewLine + ConfigFilePath;
                    return false;
                }

                settings = new DotMatrixPrinterSettings();
                settings.PrintMode = NormalizeMode(mode);
                settings.LptPort = string.IsNullOrEmpty(port) ? "LPT1" : port;
                settings.PrinterName = printerName ?? "";
                return true;
            }
            catch (Exception ex)
            {
                error = "The local printer configuration could not be read."
                    + Environment.NewLine + ConfigFilePath
                    + Environment.NewLine + Environment.NewLine + ex.Message;
                return false;
            }
        }

        internal static bool TrySaveSettings(DotMatrixPrinterSettings settings, out string error)
        {
            error = null;
            if (settings == null)
            {
                error = "Printer settings are missing.";
                return false;
            }

            try
            {
                Directory.CreateDirectory(ConfigDirectory);
                using (StreamWriter writer = new StreamWriter(ConfigFilePath, false))
                {
                    writer.WriteLine("PrintMode=" + (settings.PrintMode ?? ""));
                    writer.WriteLine("LptPort=" + (settings.LptPort ?? ""));
                    writer.WriteLine("PrinterName=" + (settings.PrinterName ?? ""));
                }

                return true;
            }
            catch (Exception ex)
            {
                error = "The local printer configuration could not be saved."
                    + Environment.NewLine + ConfigFilePath
                    + Environment.NewLine + Environment.NewLine + ex.Message;
                return false;
            }
        }

        internal static string BuildTestPage()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((char)27);
            sb.Append('j');
            sb.Append((char)1);
            sb.Append("\r\n");
            sb.Append((char)18);
            sb.Append(Fit("DOT-MATRIX RAW TEST", 80));
            sb.Append("\r\n");
            sb.Append((char)15);
            sb.Append(Fit("CONDENSED 0123456789 ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz", 80));
            sb.Append("\r\n");
            sb.Append((char)18);
            sb.Append(Fit("NORMAL MODE RESTORED", 80));
            sb.Append("\r\n\r\n\r\n");
            return sb.ToString();
        }

        internal static bool TrySend(DotMatrixPrinterSettings settings, string printOutput, string documentName)
        {
            if (settings == null)
            {
                ShowError("Printer settings are missing.");
                return false;
            }

            if (printOutput == null)
            {
                ShowError("There is no text to print.");
                return false;
            }

            if (settings.PrintMode == PrintModeLpt)
            {
                return SendLpt(printOutput, settings.LptPort);
            }

            if (settings.PrintMode == PrintModeUsb)
            {
                return SendUsb(printOutput, settings.PrinterName, documentName);
            }

            ShowError("Printer configuration has an unknown print mode: " + (settings.PrintMode ?? ""));
            return false;
        }

        private static void PrintCore(string printOutput)
        {
            if (printOutput == null)
            {
                ShowError("There is no quotation text to print.");
                return;
            }

            if (!File.Exists(ConfigFilePath))
            {
                if (!ShowPrinterSetup())
                {
                    ShowError("Printing was not sent. No local printer configuration is saved on this computer.");
                    return;
                }
            }

            DotMatrixPrinterSettings settings;
            string error;
            if (!TryLoadSettings(out settings, out error))
            {
                ShowError(error);
                if (!ShowPrinterSetup())
                {
                    ShowError("Printing was not sent. Printer setup was cancelled.");
                    return;
                }

                if (!TryLoadSettings(out settings, out error))
                {
                    ShowError(error);
                    return;
                }
            }

            string validationError = ValidateForPrint(settings);
            if (validationError != null)
            {
                ShowError(validationError);
                if (!ShowPrinterSetup())
                {
                    ShowError("Printing was not sent. Printer setup was cancelled.");
                    return;
                }

                if (!TryLoadSettings(out settings, out error))
                {
                    ShowError(error);
                    return;
                }

                validationError = ValidateForPrint(settings);
                if (validationError != null)
                {
                    ShowError(validationError);
                    return;
                }
            }

            TrySend(settings, printOutput, "Quotation");
        }

        private static string ValidateForPrint(DotMatrixPrinterSettings settings)
        {
            if (settings == null || (settings.PrintMode != PrintModeLpt && settings.PrintMode != PrintModeUsb))
            {
                return "Printer configuration has an unknown print mode."
                    + Environment.NewLine + ConfigFilePath;
            }

            if (settings.PrintMode == PrintModeUsb)
            {
                string installed = FindInstalledPrinter(settings.PrinterName);
                if (installed == null)
                {
                    if (string.IsNullOrEmpty(settings.PrinterName))
                    {
                        return "No USB printer is selected in the local printer configuration.";
                    }

                    return "The configured USB printer is not installed on this computer:"
                        + Environment.NewLine + settings.PrinterName;
                }

                settings.PrinterName = installed;
            }

            return null;
        }

        private static bool SendLpt(string printOutput, string lptPort)
        {
            try
            {
                StreamWriter sr = new StreamWriter("d:\\bill.txt");
                try
                {
                    sr.Write(printOutput);
                }
                finally
                {
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                ShowError("Could not write the LPT print file d:\\bill.txt."
                    + Environment.NewLine + Environment.NewLine + ex.Message);
                return false;
            }

            if (!File.Exists("d:\\Bill.bat"))
            {
                string port = string.IsNullOrEmpty(lptPort) ? "LPT1" : lptPort;
                ShowError("LPT printing could not start because d:\\Bill.bat was not found."
                    + Environment.NewLine + Environment.NewLine
                    + "The quotation text was written to d:\\bill.txt."
                    + Environment.NewLine
                    + "The saved LPT port (" + port + ") is not used yet. Bill.bat still chooses the printer port.");
                return false;
            }

            try
            {
                new System.Diagnostics.Process
                {
                    StartInfo =
                    {
                        FileName = "d:\\Bill.bat",
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                    }
                }.Start();
                return true;
            }
            catch (Exception ex)
            {
                ShowError("Could not start d:\\Bill.bat."
                    + Environment.NewLine + Environment.NewLine + ex.Message);
                return false;
            }
        }

        private static bool SendUsb(string printOutput, string printerName, string documentName)
        {
            string installed = FindInstalledPrinter(printerName);
            if (installed == null)
            {
                if (string.IsNullOrEmpty(printerName))
                {
                    ShowError("No USB printer is selected.");
                }
                else
                {
                    ShowError("The selected USB printer is not installed on this computer:"
                        + Environment.NewLine + printerName);
                }

                return false;
            }

            byte[] bytes;
            try
            {
                bytes = ToRawBytes(printOutput);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return false;
            }

            IntPtr printer = IntPtr.Zero;
            IntPtr buffer = IntPtr.Zero;
            bool documentStarted = false;
            bool pageStarted = false;

            try
            {
                if (!OpenPrinter(installed, out printer, IntPtr.Zero))
                {
                    int win32Error = Marshal.GetLastWin32Error();
                    printer = IntPtr.Zero;
                    ShowError("The printer could not be opened:"
                        + Environment.NewLine + installed
                        + Environment.NewLine + Environment.NewLine + Win32Message(win32Error));
                    return false;
                }

                DOCINFO docInfo = new DOCINFO();
                docInfo.pDocName = string.IsNullOrEmpty(documentName) ? "Dot Matrix Print" : documentName;
                docInfo.pOutputFile = null;
                docInfo.pDataType = "RAW";

                if (!StartDocPrinter(printer, 1, ref docInfo))
                {
                    int win32Error = Marshal.GetLastWin32Error();
                    ShowError("RAW printing could not start for:"
                        + Environment.NewLine + installed
                        + Environment.NewLine + Environment.NewLine + Win32Message(win32Error));
                    return false;
                }

                documentStarted = true;
                if (!StartPagePrinter(printer))
                {
                    int win32Error = Marshal.GetLastWin32Error();
                    ShowError("RAW printing could not start a page for:"
                        + Environment.NewLine + installed
                        + Environment.NewLine + Environment.NewLine + Win32Message(win32Error));
                    return false;
                }

                pageStarted = true;
                buffer = Marshal.AllocCoTaskMem(bytes.Length == 0 ? 1 : bytes.Length);
                if (bytes.Length > 0)
                {
                    Marshal.Copy(bytes, 0, buffer, bytes.Length);
                }

                int written;
                if (!WritePrinter(printer, buffer, bytes.Length, out written) || written != bytes.Length)
                {
                    int win32Error = Marshal.GetLastWin32Error();
                    ShowError("RAW printing failed while sending data to:"
                        + Environment.NewLine + installed
                        + Environment.NewLine + Environment.NewLine + Win32Message(win32Error));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowError("RAW printing failed." + Environment.NewLine + Environment.NewLine + ex.Message);
                return false;
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(buffer);
                }

                if (pageStarted)
                {
                    EndPagePrinter(printer);
                }

                if (documentStarted)
                {
                    EndDocPrinter(printer);
                }

                if (printer != IntPtr.Zero)
                {
                    ClosePrinter(printer);
                }
            }
        }

        internal static string FindInstalledPrinter(string printerName)
        {
            if (string.IsNullOrEmpty(printerName))
            {
                return null;
            }

            System.Drawing.Printing.PrinterSettings.StringCollection installed =
                System.Drawing.Printing.PrinterSettings.InstalledPrinters;

            for (int i = 0; i < installed.Count; i++)
            {
                string name = installed[i];
                if (string.Compare(name, printerName, true) == 0)
                {
                    return name;
                }
            }

            return null;
        }

        internal static string NormalizeMode(string mode)
        {
            if (string.Compare(mode, PrintModeUsb, true) == 0)
            {
                return PrintModeUsb;
            }

            if (string.Compare(mode, PrintModeLpt, true) == 0)
            {
                return PrintModeLpt;
            }

            return mode == null ? "" : mode.Trim();
        }

        private static byte[] ToRawBytes(string text)
        {
            byte[] bytes = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c > 255)
                {
                    throw new InvalidOperationException(
                        "Print text contains a character that cannot be sent as one printer byte (U+"
                        + ((int)c).ToString("X4") + ").");
                }

                bytes[i] = (byte)c;
            }

            return bytes;
        }

        private static string Fit(string text, int width)
        {
            if (text == null)
            {
                text = "";
            }

            if (text.Length > width)
            {
                return text.Substring(0, width);
            }

            return text.PadRight(width);
        }

        private static string Win32Message(int error)
        {
            return "Windows error " + error.ToString() + ": " + new System.ComponentModel.Win32Exception(error).Message;
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Dot Matrix Printer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DOCINFO
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDocName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pOutputFile;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDataType;
        }

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFO pDocInfo);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);
    }
}
