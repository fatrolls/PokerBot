using System;
using System.Collections.Generic;
using System.Drawing;

namespace Bot.Detection
{
    internal class PlayerCount
    {
        private readonly IntPtr _pokerHandle;

        public PlayerCount(IntPtr pokerHandle)
        {
            this._pokerHandle = pokerHandle;
        }

        private bool CheckPlayerState(int player)
        {
            var playerPos = Positions._relativePlayerPoints[player];

            var checkPixel = Screenshot.GetRelativeScreenshot(playerPos, new Size(99, 25), _pokerHandle);

            //Total Colors red found.
            Dictionary<Color, int> frequency = new Dictionary<Color, int>();
            for (int i = 0; i < checkPixel.Width; i++)
            {
                for (int j = 0; j < checkPixel.Height; j++)
                {
                    var pixel = checkPixel.GetPixel(i, j);
                    if (frequency.ContainsKey(pixel)) frequency[pixel]++;
                    else frequency.Add(pixel, 1);
                }
            }
            int highestColorFound = 0;
            Color highestColor = Color.FromArgb(0);

            foreach (var kvp in frequency)
            {
                if (kvp.Value > highestColorFound) { 
                    highestColor = kvp.Key;
                    highestColorFound = kvp.Value;
                }
            }

            //Console.WriteLine("Color (R={0},G={1},B={2})", highestColor.R, highestColor.G, highestColor.B);

            return highestColor.R > 100;
        }

        public int playerCount()
        {
            var count = 0;

            Resizer.ResizeAndSwitch(_pokerHandle);

            for (var i = 0; i <= 5; i++)
                if (CheckPlayerState(i))
                    count++;

            return count;
        }
    }
}