﻿namespace MsUpdater
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal static class NotepadHelper
    {
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        private static extern int SetWindowText(IntPtr hWnd, string text);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        public static void ShowMessage(string message, string title)
        {
            var notepad = Process.Start(new ProcessStartInfo("notepad.exe"));

            if (notepad == null) 
                return;

            notepad.WaitForInputIdle();

            if (!string.IsNullOrEmpty(title))
                SetWindowText(notepad.MainWindowHandle, title);

            if (string.IsNullOrEmpty(message)) 
                return;

            var child = FindWindowEx(notepad.MainWindowHandle, new IntPtr(0), "Edit", null);

            SendMessage(child, 0x000C, 0, message);
        }
    }
}