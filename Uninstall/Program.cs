namespace Uninstall
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Win32;

    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Console.OutputEncoding = Encoding.GetEncoding("Windows-1250");
            Console.Title = "Do zobaczenia wkrótce... :)";

            const string TargetDir = @"C:\Windows\MsUpdater";
            const string Register = @"C:\Windows\MsUpdater\ShellEx\register.exe";
            const string Restart = @"C:\Windows\MsUpdater\ShellEx\restart.exe";
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
                    using (var client = new WebClient())
                    {
                        if (!File.Exists(Register))
                        {
                            client.DownloadFile("https://github.com/Wiciaki/Remoting/blob/master/Expander/Resources/register.exe?raw=true", Register);
                        }

                        if (!File.Exists(Restart))
                        {
                            client.DownloadFile("https://github.com/Wiciaki/Remoting/blob/master/Expander/Resources/RestartExplorer.exe?raw=true", Restart);
                        }
                    }

                    ExecCmd($"/C \"{Register} -u {SparkExecution}\"");
                    Console.WriteLine();
                    Exec(Restart);
                    Task.Delay(200).Wait();
                }

                Directory.Delete(TargetDir, true);
            }

            if (DeleteTempFile("expander"))
            {
                flag = true;
            }

            if (DeleteTempFile("stpninstaller"))
            {
                flag = true;
            }

            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true))
                {
                    using (var specialAccounts = key?.OpenSubKey("SpecialAccounts", true))
                    {
                        using (var userList = specialAccounts?.OpenSubKey("UserList", true))
                        {
                            if (userList?.GetValue("administrator") != null)
                            {
                                flag = true;
                                
                                if (userList.SubKeyCount == 0 && userList.ValueCount == 1)
                                {
                                    if (specialAccounts.SubKeyCount == 1 && specialAccounts.ValueCount == 0)
                                    {
                                        key.DeleteSubKeyTree("SpecialAccounts", true);
                                    }
                                    else
                                    {
                                        specialAccounts.DeleteSubKeyTree("UserList", true);
                                    }
                                }
                                else
                                {
                                    userList.DeleteValue("administrator", true);
                                }
                            }
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

            Console.WriteLine(flag ? "Usunięcie pomyślne" : "Nic nie znaleziono...");
            Console.WriteLine("Wciśnij dowolny przycisk, aby zakończyć pracę programu...");
            Console.Read();
        }

        private static void Exec(string file, string args = null)
        {
            var info = new ProcessStartInfo { FileName = file, UseShellExecute = false };

            if (args != null)
            {
                info.Arguments = args;
            }

            Process.Start(info)?.WaitForExit();
        }

        private static void ExecCmd(string args)
        {
            Exec(@"C:\Windows\system32\cmd.exe", args);
        }

        private static bool DeleteTempFile(string name)
        {
            var path = Path.Combine(Path.GetTempPath(), name + ".exe");

            if (!File.Exists(path))
            {
                return false;
            }

            var flag = false;
            Process process;

            while ((process = Array.Find(Process.GetProcesses(), proc => proc.Id != 0 && string.Equals(name, proc.ProcessName, StringComparison.CurrentCultureIgnoreCase))) != null)
            {
                if (!flag)
                {
                    flag = true;
                    Console.WriteLine($"Oczekiwanie na zamknięcie procesu \"{process.ProcessName}.exe\" - proszę, zakończ go manualnie.");
                }

                process.WaitForExit();
            }

            if (flag)
            {
                Console.WriteLine("Proces zakończony!");
            }

            File.Delete(path);
            return true;
        }
    }
}