namespace MsUpdater
{
    using System;
    using System.Windows.Forms;

    internal static class Service
    {
        static Service()
        {
            if (Environment.MachineName == "WICIAKI")
            {
                MessageBox.Show(@"Bootstrap");
            }
        }
    }
}