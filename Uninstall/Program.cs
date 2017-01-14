namespace Uninstall
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading;

    using Microsoft.Win32;

    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
        private static void Main(string[] args)
        {
            const string TargetDir = @"C:\Windows\MsUpdater";
            const string Register = @"C:\Windows\MsUpdater\ShellEx\register.exe";
            const string RestartExplorer = @"C:\Windows\MsUpdater\ShellEx\restart.exe";
            const string SparkExecution = @"C:\Windows\MsUpdater\ShellEx\SparkExecution.dll";

            var flag = false;

            if (Directory.Exists(TargetDir))
            {
                flag = true;

                Array.ForEach(Process.GetProcessesByName("msupdater"), process => process.Kill());
                
                ExecCmd("/C net user administrator \"\"");
                ExecCmd("/C net user administrator /active:no");
                ExecCmd($"/C \"{Register} -u {SparkExecution}\"");

                Console.WriteLine();
                Exec(RestartExplorer, null);

                Thread.Sleep(200);

                Directory.Delete(TargetDir, true);
            }

            if (DeleteTempFile("Expander.exe") || DeleteTempFile("stpninstaller.exe"))
            {
                flag = true;
            }

            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                // ReSharper disable PossibleNullReferenceException

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

                // ReSharper restore PossibleNullReferenceException
            }

            Console.WriteLine(flag ? "Usunięcie pomyślne" : "Nic nie znaleziono...");
            Thread.Sleep(10 * 1000);
        }

        private static void Exec(string file, string args)
        {
            Process.Start(new ProcessStartInfo { FileName = file, Arguments = args, UseShellExecute = false})?.WaitForExit();
        }

        private static void ExecCmd(string args)
        {
            Exec(@"C:\Windows\system32\cmd.exe", args);
        }

        private static bool DeleteTempFile(string name)
        {
            var info = new FileInfo(Path.Combine(Path.GetTempPath(), name));

            if (info.Exists)
            {
                try
                {
                    info.Delete();
                }
                catch
                {
                    // ignored
                }
            }

            return info.Exists;
        }
    }
}