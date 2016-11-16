namespace Updater
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            const string BootstrapName = "MsUpdater.exe";

            var target = Path.Combine(Directory.GetCurrentDirectory(), BootstrapName);

            if (File.Exists(target))
            {
                File.Delete(target);
            }

            using (var client = new WebClient())
            {
                client.DownloadFile("https://raw.githubusercontent.com/Wiciaki/Remoting/master/Download/" + BootstrapName, target);
            }

            Process.Start(target);
        }
    }
}