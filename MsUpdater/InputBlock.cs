namespace MsUpdater
{
    using System;
    using System.Runtime.InteropServices;

    internal class InputBlock : IDisposable
    {
        [DllImport("user32.dll")]
        public static extern bool BlockInput(bool block);

        public InputBlock()
        {
            BlockInput(true);
        }

        public void Dispose()
        {
            BlockInput(false);
        }
    }
}