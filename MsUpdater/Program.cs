namespace MsUpdater
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal static class Program
    {
        private struct ResourceData
        {
            public string CloudPath;
            
            public string LocalPath;
        }

        private const string Downloads = "";

        private static readonly List<ResourceData> Resources;

        static Program()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var resourceNames = new List<string>();

            Resources = resourceNames.ConvertAll(name => new ResourceData { CloudPath = Path.Combine(Downloads, name), LocalPath = Path.Combine(currentPath, name) });
        }

        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }


        }
    }
}