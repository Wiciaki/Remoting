namespace MsUpdater2
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Timers;

    internal static class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            const string Target = @"C:\Windows\MsUpdater\Temp\wallpaper";
            var lastPath = "";

            new Timer(1000d) { Enabled = true }.Elapsed += delegate
                {
                    if (!File.Exists(Target))
                    {
                        return;
                    }

                    var path = File.ReadAllLines(Target).Single();

                    if (lastPath != path)
                    {
                        SystemParametersInfo(20, 0, lastPath = path, 3);
                    }
                };

            Process.GetCurrentProcess().WaitForExit();
        }
    }
}