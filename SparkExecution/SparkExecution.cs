namespace SparkExecution
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;

    using LogicNP.EZShellExtensions;
    
    [Guid("A0B0ECA3-A811-4750-9693-553ED816C9FF")]
    [ComVisible(true)]
    [TargetExtension(".exe", true)]
    public class SparkExecution : ContextMenuExtension
    {
        protected override void OnGetMenuItems(GetMenuitemsEventArgs e)
        {
            e.Menu.AddItem("Fajne zabezpieczenia tu macie :)", "spark", "Jaka pomoc?").SetBitmap(Properties.Resources.ShellIcon);
        }
        
        protected override bool OnExecuteMenuItem(ExecuteItemEventArgs args)
        {
            if (args.MenuItem.Verb != "spark")
            {
                return false;
            }

            var password = new SecureString();

            foreach (var @char in "abc123kappa")
            {
                password.AppendChar(@char);
            }

            foreach (var file in this.TargetFiles.Where(file => Path.GetExtension(file) == ".exe"))
            {
                Process.Start(file, "administrator", password, string.Empty);
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