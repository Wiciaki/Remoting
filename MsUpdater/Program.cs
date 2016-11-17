namespace MsUpdater
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            
            var updater = Path.Combine(Directory.GetCurrentDirectory(), "Updater.exe");

            if (File.Exists(updater))
            {
                File.Delete(updater);
            }
            
            string webVersion;

            using (var client = new WebClient())
            {
                webVersion = client.DownloadString("https://raw.githubusercontent.com/Wiciaki/Remoting/master/Downloads/Version.txt");
            }

            if (typeof(Program).Assembly.GetName().Version < new Version(webVersion))
            {
                File.WriteAllBytes(updater, Properties.Resources.Updater);
                Process.Start(updater);
                return;
            }

            RuntimeHelpers.RunClassConstructor(typeof(Service).TypeHandle);
        }
    }
}