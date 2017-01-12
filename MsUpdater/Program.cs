namespace MsUpdater
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            RuntimeHelpers.RunClassConstructor(typeof(Service).TypeHandle);
            Process.GetCurrentProcess().WaitForExit();
        }
    }
}