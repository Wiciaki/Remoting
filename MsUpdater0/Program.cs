namespace MsUpdater0
{
    using System;
    using System.Security.Principal;
    using System.Windows.Forms;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            MessageBox.Show($"Hello world! {new WindowsPrincipal(WindowsIdentity.GetCurrent()).Identity.Name}");
        }
    }
}