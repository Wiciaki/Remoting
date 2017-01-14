namespace Uninstall
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

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

                foreach (var process in Process.GetProcessesByName("msupdater"))
                {
                    process.Kill();
                }

                if (File.Exists(SparkExecution))
                {
                    ExecCmd("/C net user administrator \"\"");
                    ExecCmd("/C net user administrator /active:no");
                    ExecCmd($"/C \"{Register} -u {SparkExecution}\"");

                    Console.WriteLine();
                    Exec(RestartExplorer, null);

                    Thread.Sleep(200);

                    Directory.Delete(TargetDir, true);
                }
                else
                {
                    try
                    {
                        Directory.Delete(TargetDir, true);
                    }
                    catch
                    {
                        MoveFileEx(TargetDir, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);

                        const string Wall = "-------------------------";
                        Console.WriteLine(Wall);
                        Console.WriteLine("Serwis chronienia plików jest wciąż aktywny.");
                        Console.WriteLine("Niektóre pliki zostaną usunięte po ponownym uruchomieniu...");
                        Console.WriteLine(Wall);
                        Console.WriteLine();
                    }
                }
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

        private static bool DeleteTempFile(string name)
        {
            var info = new FileInfo(Path.Combine(Path.GetTempPath(), name));

            if (info.Exists)
            {
                try
                {
                    info.Delete();
                }
                catch { }

                return true;
            }

            return false;
        }
    }
}