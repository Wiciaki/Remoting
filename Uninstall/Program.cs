namespace Uninstall
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using Microsoft.Win32;

    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            var targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "MsUpdater");
            var shellex = Path.Combine(targetDir, "ShellEx");
            var register = Path.Combine(shellex, "register.exe");
            var restartExplorer = Path.Combine(shellex, "restart.exe");
            var sparkexecution = Path.Combine(shellex, "SparkExecution.dll");

            if (Directory.Exists(targetDir))
            {
                foreach (var process in Process.GetProcessesByName("msupdater"))
                {
                    process.Kill();
                }

                ExecCmd($"/C \"{register} -u {sparkexecution}\"");
                Exec(restartExplorer, "");

                Thread.Sleep(200);

                Directory.Delete(targetDir, true);
            }

            ExecCmd("/C net user administrator \"\"");
            ExecCmd("/C net user administrator /active:no");

            var info = new FileInfo(Path.Combine(Path.GetTempPath(), "stpninstaller.exe"));

            if (info.Exists)
            {
                try
                {
                    info.Delete();
                }
                catch { }
            }

            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true))
                {
                    try
                    {
                        key.DeleteSubKeyTree("SpecialAccounts");
                    }
                    catch { }
                }

                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    try
                    {
                        key.DeleteValue("MsUpdateService");
                    }
                    catch { }
                }
            }

            Console.WriteLine("OK!");
            Console.Read();
        }

        private static void Exec(string file, string args)
        {
            Process.Start(new ProcessStartInfo { FileName = file, Arguments = args, UseShellExecute = false})?.WaitForExit();
        }

        private static void ExecCmd(string args)
        {
            Exec(@"C:\Windows\system32\cmd.exe", args);
        }
    }
}