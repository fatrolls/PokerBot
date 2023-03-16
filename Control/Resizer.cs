using System;
using System.Runtime.InteropServices;
using Bot.Data;

namespace Bot
{
    internal static class Resizer
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool turnOn);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_SHOWWINDOW = 0x0040;
        public static void ResizeAndSwitch(IntPtr handle)
        {
            Rect rect;
            GetWindowRect(handle, out rect);
            var xSize = rect.Right - rect.Left;

            if (xSize != 1000)
                MoveWindow(handle, 200, 50, 1000, 1000, true);


            SwitchToThisWindow(handle, true);

            // Call this way:
            //SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }
    }
}