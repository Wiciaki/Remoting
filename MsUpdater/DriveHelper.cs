namespace MsUpdater
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Timers;

    internal static class DriveHelper
    {
        public static bool Opened { get; private set; }

        public static void EnsureStatus(bool desiredStatus)
        {
            if (Opened == desiredStatus || Environment.TickCount - lastTick < 5000)
            {
                order = desiredStatus;
            }
            else
            {
                lastTick = Environment.TickCount;

                SetDriveStatusImpl(Opened = desiredStatus);
            }
        }

        private static int lastTick;

        private static  bool? order;

        static DriveHelper()
        {
            new Timer(500d) { Enabled = true }.Elapsed += delegate
                {
                    if (order.HasValue)
                    {
                        EnsureStatus(order.Value);
                    }
                };
        }

        [DllImport("winmm.dll")]
        private static extern int mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

        private static void SetDriveStatusImpl(bool status)
        {
            mciSendString("set CDAudio door " + (status ? "open" : "closed"), null, 0, IntPtr.Zero);
        }
    }
}