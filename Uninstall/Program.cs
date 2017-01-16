namespace Uninstall
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Windows.Forms;

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

                Array.ForEach(Process.GetProcesses(), process =>
                    {
                        if (process.ProcessName.StartsWith("msupdater", StringComparison.CurrentCultureIgnoreCase))
                        {
                            process.Kill();
                            process.WaitForExit();
                        }
                    });
                
                ExecCmd("/C net user administrator \"\"");
                ExecCmd("/C net user administrator /active:no");

                if (File.Exists(SparkExecution))
                {
                    if (!File.Exists(Register))
                    {
                        var webFile = GetWebFile("https://github.com/Wiciaki/Remoting/blob/master/Expander/Resources/register.exe?raw=true");
                        ExecCmd($"/C \"{webFile} -u {SparkExecution}\"");
                        Thread.Sleep(200);
                        File.Delete(webFile);
                    }
                    else
                    {
                        ExecCmd($"/C \"{Register} -u {SparkExecution}\"");
                    }

                    Console.WriteLine();

                    if (!File.Exists(RestartExplorer))
                    {
                        var webFile = GetWebFile("https://github.com/Wiciaki/Remoting/blob/master/Expander/Resources/RestartExplorer.exe?raw=true");
                        Exec(webFile, null);
                        Thread.Sleep(200);
                        File.Delete(webFile);
                    }
                    else
                    {
                        Exec(RestartExplorer, null);
                    }

                    Thread.Sleep(200);
                }

                try
                {
                    Directory.Delete(TargetDir, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie można usunąć folderu!\n{TargetDir}\n\nPowód:\n{ex}");
                }
            }

            if (DeleteTempFile("Expander.exe"))
            {
                flag = true;
            }

            if (DeleteTempFile("stpninstaller.exe"))
            {
                flag = true;
            }

            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true))
                {
                    var specialAccounts = key?.OpenSubKey("SpecialAccounts", true);
                    var userList = specialAccounts?.OpenSubKey("UserList", true);

                    if (userList?.GetValue("administrator") != null)
                    {
                        flag = true;

                        if (specialAccounts.SubKeyCount == 1 && specialAccounts.ValueCount == 0 && userList.SubKeyCount == 0 && userList.ValueCount == 1)
                        {
                            key.DeleteSubKeyTree("SpecialAccounts");
                        }
                        else
                        {
                            userList.DeleteValue("administrator");
                        }
                    }
                }

                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key?.GetValue("MsUpdateService") != null)
                    {
                        flag = true;
                        key.DeleteValue("MsUpdateService");
                    }
                }
            }

            MessageBox.Show(flag ? "Usunięcie pomyślne" : "Nic nie znaleziono...");
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
            var path = Path.Combine(Path.GetTempPath(), name);
            var info = new FileInfo(path);

            if (info.Exists)
            {
                try
                {
                    info.Delete();
                }
                catch
                {
                    MessageBox.Show($"Nie można usunąć pliku!\n{path}");
                }

                return true;
            }

            return false;
        }

        private static string GetWebFile(string link)
        {
            var path = Path.Combine(Path.GetTempPath(), "sparkTemp.exe");

            using (var client = new WebClient())
            {
                client.DownloadFile(link, path);
            }

            return path;
        }
    }
}