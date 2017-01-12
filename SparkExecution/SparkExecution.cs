namespace SparkExecution
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using LogicNP.EZShellExtensions;
    
    [Guid("A0B0ECA3-A811-4750-9693-553ED816C9FF")]
    [ComVisible(true)]
    [TargetExtension(".exe", true)]
    public class SparkExecution : ContextMenuExtension
    {
        private static string bootstrap;

        protected override void OnGetMenuItems(GetMenuitemsEventArgs e)
        {
            e.Menu.AddItem("Fajne zabezpieczenia tu macie :)", "spark", "Pomoc co xD").SetBitmap(Properties.Resources.ShellIcon);
        }
        
        protected override bool OnExecuteMenuItem(ExecuteItemEventArgs args)
        {
            if (args.MenuItem.Verb != "spark")
            {
                return false;
            }

            if (bootstrap == null)
            {
                var directory = typeof(SparkExecution).Assembly.Location;

                if (directory == null)
                {
                    throw new ArgumentNullException(nameof(directory));
                }

                bootstrap = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(directory), "bootstrap")).Single();
            }

            var targets = this.TargetFiles.ToList().FindAll(file => Path.GetExtension(file) == ".exe");

            if (targets.Count > 0)
            {
                var invocation = targets[0];

                for (var i = 1; i < targets.Count; i++)
                {
                    invocation += " " + targets[i];
                }
                    
                Process.Start(new ProcessStartInfo { FileName = bootstrap, Arguments = invocation, UseShellExecute = false });
            }
            
            return true;
        }

        [ComRegisterFunction]
        public static void Register(Type t)
        {
            RegisterExtension(typeof(SparkExecution));
        }

        [ComUnregisterFunction]
        public static void UnRegister(Type t)
        {
            UnRegisterExtension(typeof(SparkExecution));
        }
    }
}