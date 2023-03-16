using System;
using System.Collections.Generic;
using System.Drawing;

namespace Bot.Detection
{
    internal class RoundDetector
    {
        private readonly IntPtr _pokerHandle;

        public RoundDetector(IntPtr pokerHandle)
        {
            _pokerHandle = pokerHandle;
        }

        public bool MyTurn()
        {
            Resizer.ResizeAndSwitch(_pokerHandle);
            return CheckPixel(Positions._relativeMyTurnPos, Positions._MyTurnSize);
        }

        public bool OnlyCallOrFold()
        {
            Resizer.ResizeAndSwitch(_pokerHandle);
            return CheckPixel(Positions._relativeMyCallPos, Positions._MyCallSize);
        }


        private bool CheckPixel(Point point, Size size)
        {
            var bitmapPixel = Screenshot.GetRelativeScreenshot(point, size, _pokerHandle);

            //Total Colors red found.
            Dictionary<Color, int> frequency = new Dictionary<Color, int>();
            for (int i = 0; i < bitmapPixel.Width; i++)
            {
                for (int j = 0; j < bitmapPixel.Height; j++)
                {
                    var pixel = bitmapPixel.GetPixel(i, j);
                    if (frequency.ContainsKey(pixel)) frequency[pixel]++;
                    else frequency.Add(pixel, 1);
                }
            }
            int highestColorFound = 0;
            Color highestColor = Color.FromArgb(0);

            foreach (var kvp in frequency)
            {
                if (kvp.Value > highestColorFound)
                {
                    highestColor = kvp.Key;
                    highestColorFound = kvp.Value;
                }
            }

            //Console.WriteLine("Color (R={0},G={1},B={2})", highestColor.R, highestColor.G, highestColor.B);

            return highestColor.R > 50;
        }
    }
}