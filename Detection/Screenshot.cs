using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Bot.Data;
using Point = System.Drawing.Point;

namespace Bot.Detection
{
    internal class Screenshot
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        private static Bitmap GetScreen(IntPtr handle)
        {
            Rect rect;
            GetWindowRect(handle, out rect);

            Bitmap bmp = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            PrintWindow(handle, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }

        public static Bitmap GetRelativeScreenshot(Point position, Size size, IntPtr handle)
        {
            Bitmap screenArea = GetScreen(handle);
            if(position.X > screenArea.Width || position.Y > screenArea.Height ||
              size.Width > screenArea.Width || size.Height > screenArea.Height) return screenArea;
            Image cropped = screenArea.Clone(new Rectangle(position, size), screenArea.PixelFormat);

            return (Bitmap)cropped;
        }
    }
}