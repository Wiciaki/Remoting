namespace Uninstall
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;

    using Microsoft.Win32;

    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum MoveFileFlags
    {
        MOVEFILE_REPLACE_EXISTING = 0x00000001,
        MOVEFILE_COPY_ALLOWED = 0x00000002,
        MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004,
        MOVEFILE_WRITE_THROUGH = 0x00000008,
        MOVEFILE_CREATE_HARDLINK = 0x00000010,
        MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x00000020
    }

    internal class Program
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            var targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "MsUpdater");
            var shellex = Path.Combine(targetDir, "ShellEx");
            var register = Path.Combine(shellex, "register.exe");
            var restartExplorer = Path.Combine(shellex, "restart.exe");
            var sparkexecution = Path.Combine(shellex, "SparkExecution.dll");
            var flag = false;

            if (Directory.Exists(targetDir))
            {
                flag = true;

                foreach (var process in Process.GetProcessesByName("msupdater"))
                {
                    process.Kill();
                }

                if (File.Exists(sparkexecution))
                {
                    ExecCmd("/C net user administrator \"\"");
                    ExecCmd("/C net user administrator /active:no");
                    ExecCmd($"/C \"{register} -u {sparkexecution}\"");

                    Console.WriteLine();
                    Exec(restartExplorer, null);

                    Directory.Delete(targetDir, true);
                }
                else
                {
                    try
                    {
                        Directory.Delete(targetDir, true);
                    }
                    catch
                    {
                        MoveFileEx(targetDir, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);

                        const string Wall = "-------------------------";
                        Console.WriteLine(Wall);
                        Console.WriteLine("Serwis chronienia plików jest wciąż aktywny.");
                        Console.WriteLine("Niektóre pliki zostaną usunięte po ponownym uruchomieniu...");
                        Console.WriteLine(Wall);
                        Console.WriteLine();
                    }
                }
            }

            var info = new FileInfo(Path.Combine(Path.GetTempPath(), "stpninstaller.exe"));

            if (info.Exists)
            {
                flag = true;

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

            Console.WriteLine(!flag ? "Nic nie znaleziono..." : "Usunięcie pomyślne");
            Console.WriteLine("Naciśnij dowolny klawisz, aby zamknąć narzędzie.");
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