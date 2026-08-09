namespace RRE_To_Tally
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 2 && string.Equals(args[0], "--debug-tally-sales-test", StringComparison.OrdinalIgnoreCase))
            {
                DebugTallySalesXmlTest.WriteLocalGstSplitTest(args[1]);
                return;
            }
            if (args.Length == 2 && string.Equals(args[0], "--debug-tally-masters-test", StringComparison.OrdinalIgnoreCase))
            {
                DebugTallySalesXmlTest.WriteMastersStructureTest(args[1]);
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            using (FrmLogin login = new FrmLogin())
            {
                if (login.ShowDialog() == DialogResult.OK && login.AuthenticatedUser != null)
                {
                    Application.Run(new FrmTallySalesExport(login.AuthenticatedUser));
                }
            }
        }
    }
}
