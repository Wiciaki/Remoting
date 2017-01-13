namespace MsUpdater
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var directory = Directory.GetCurrentDirectory();
            
            /*
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

            RuntimeHelpers.RunClassConstructor(typeof(Service).TypeHandle);

            Survival:

            using (File.Open(Path.Combine(directory, "Bootstrap.exe"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Process.GetCurrentProcess().WaitForExit();
            }
        }
    }
}