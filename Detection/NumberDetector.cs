using System;
using System.Drawing;
using Patagames.Ocr;
using System.Text.RegularExpressions;

namespace Bot.Detection
{
    internal class NumberDetector
    {
        private readonly OcrApi _api = OcrApi.Create();
        private readonly IntPtr _pokerHandle;

        private readonly RoundDetector _turnDetect;


        public NumberDetector(IntPtr pokerHandle)
        {
            _pokerHandle = pokerHandle;
            _turnDetect = new RoundDetector(pokerHandle);
            _api.Init(Patagames.Ocr.Enums.Languages.English);
        }

        public int GetPot()
        {
            Resizer.ResizeAndSwitch(_pokerHandle);
            return GetNumber(Positions._relativePotPos, Positions._potSize);
            //, CutPotBitmap);
        }


        public int MinimalContribution()
        {
            Resizer.ResizeAndSwitch(_pokerHandle);
            if (!_turnDetect.MyTurn()) return 0;
            return GetCall() < GetMinRaise() ? GetCall() : GetMinRaise();
        }

        public int MinimalRaise()
        {
            Resizer.ResizeAndSwitch(_pokerHandle);
            if (!_turnDetect.MyTurn()) return 0;
            return _turnDetect.OnlyCallOrFold() ? 0 : GetMinRaise();
        }

        public int GetMoney()
        {
            //var bitmap = Screenshot.GetRelativeScreenshot(Positions._moneyCheckPoint, Positions._moneySize, _pokerHandle);
            //var pos = rgbSum(bitmap.GetPixel(0, 0)) > 150 ? Positions._relativeMoneyPosRight : Positions._relativeMoneyPos;

            return GetNumber(Positions._moneyCheckPoint, Positions._moneySize); //, CutMoneyBitmap);
        }


        private int GetCall()
        {
            return GetNumber(Positions._relativeCallPos, Positions._callSize);//, CutCallBitmap);
        }

        private int GetMinRaise()
        {
            return GetNumber(Positions._relativeRaisePos, Positions._RaiseSize); //, CutCallBitmap);
        }


        private int GetNumber(Point pos, Size size)
        {
            var bitmap = Screenshot.GetRelativeScreenshot(pos, size, _pokerHandle);

            /*try
            {
                bitmap = cut(bitmap);
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }*/

            string d = _api.GetTextFromImage(bitmap);

            var regex = new Regex(@"([0-9\,]+)");
            var match = regex.Match(d);
            var matchString = match.Value;

            matchString = matchString.Replace(",", "").Replace(".", "");
            matchString = matchString.Length == 0 ? "0" : matchString;
            return int.Parse(matchString);
        }

        private static Bitmap CutPotBitmap(Bitmap potBitmap)
        {
            var y = potBitmap.Height / 2;
            var xLeft = 0;

            while (potBitmap.GetPixel(xLeft, y).G > 70)
                xLeft++;

            var xRight = potBitmap.Width - 1;

            while (potBitmap.GetPixel(xRight, y).G > 70)
                xRight--;

            var crop = new Rectangle(xLeft, 0, xRight - xLeft, potBitmap.Height);

            return potBitmap.Clone(crop, potBitmap.PixelFormat);
        }

        private Bitmap CutCallBitmap(Bitmap callBitmap)
        {
            var y = callBitmap.Height / 2;
            var xLeft = 0;

            while (rgbSum(callBitmap.GetPixel(xLeft, y)) < 400)
                xLeft++;

            var xRight = callBitmap.Width - 1;

            while (rgbSum(callBitmap.GetPixel(xRight, y)) < 400)
                xRight--;

            var crop = new Rectangle(Math.Max(xLeft - 20, 0), 0, Math.Min(xRight - xLeft + 30, callBitmap.Width),
                callBitmap.Height);

            return callBitmap.Clone(crop, callBitmap.PixelFormat);
        }

        private Bitmap CutMoneyBitmap(Bitmap moneyBitmap)
        {
            var y = moneyBitmap.Height / 2;
            var xLeft = 0;

            while (moneyCutCheck(moneyBitmap.GetPixel(xLeft, y)))
                xLeft++;

            var xRight = moneyBitmap.Width - 1;

            while (moneyCutCheck(moneyBitmap.GetPixel(xRight, y)))
                xRight--;

            var crop = new Rectangle(Math.Max(0, xLeft - 20), 0, Math.Min(xRight - xLeft + 30, moneyBitmap.Width),
                moneyBitmap.Height);

            return moneyBitmap.Clone(crop, moneyBitmap.PixelFormat);
        }

        private int rgbSum(Color pixel)
        {
            return pixel.R + pixel.G + pixel.B;
        }

        private bool moneyCutCheck(Color pixel)
        {
            if (pixel.R > 100 && pixel.G > 100 && pixel.B > 100)
                return false;
            return true;
        }
    }
}