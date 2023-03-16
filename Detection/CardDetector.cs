using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Bot.Exceptions;
using Bot.Properties;

namespace Bot.Detection
{
    internal class CardDetector
    {
        private readonly IntPtr _pokerHandle;


        public CardDetector(IntPtr handle)
        {
            _pokerHandle = handle;
        }

        private static Bitmap CardCrop(Bitmap source)
        {
            int lowestX = 1000;
            int lowestY = 1000;
            bool whiteFound = false;
            for (var x = 0; x < source.Width; x++)
            {
                for (var y = 0; y < source.Height; y++)
                {
                    var pixel = source.GetPixel(x, y);

                    //white color to start it up
                    if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                    {
                        whiteFound = true;
                    }
                    //Red Or Black
                    if (whiteFound && (pixel.R > 180 && pixel.G < 20 && pixel.B < 20) || (pixel.R <= 50 && pixel.G <= 50 && pixel.B <= 50))
                    {
                        if (lowestX > x) lowestX = x;
                        if (lowestY > y) lowestY = y;
                    }
                }
            }
            if (lowestX == 1000 || lowestY == 1000)
            {
                lowestX = 0;
                lowestY = 0;
            }

            Point position = new Point(lowestX, lowestY);
            var size = new Size(Positions._relativeCheckerCardPos.X, Positions._relativeCheckerCardPos.Y);
            var rect = new Rectangle(position, size);
            return source.Clone(rect, source.PixelFormat);
        }


        private static Bitmap SecondCrop(Bitmap source)
        {
            Point position;
            for (var y = 0; y < source.Height; y++)
            for (var x = 0; x < source.Width; x++)
            {
                var pixel = source.GetPixel(x, y);
                if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255) continue;
                position = new Point(x, y);
                goto found;
            }
            throw new NoCardException();
            found:
            var rect = new Rectangle(position, new Size(Positions._relativeCheckerCardPos.X, Positions._relativeCheckerCardPos.Y));
            return source.Clone(rect, source.PixelFormat);
        }


        public List<Data.Card> GetCards(bool playerCards)
        {
            Resizer.ResizeAndSwitch(_pokerHandle);

            int cardAmount;
            int margin;
            Point pos;

            if (playerCards)
            {
                cardAmount = 2;
                margin = Positions.PlayerCardMargin;
                pos = Positions._relativePlayerCardPos;
            }
            else
            {
                cardAmount = GetTableCardAmount();
                margin = Positions.TableCardMargin;
                pos = Positions._relativeTableCardPos;
            }

            var cards = new List<Data.Card>();

            for (var i = 0; i < cardAmount; i++)
            {
                try
                {
                    var bitmap = Screenshot.GetRelativeScreenshot(new Point(pos.X + (i * margin), pos.Y),
                        Positions._cropSize, _pokerHandle);

                    var name = "";
                    if (playerCards)
                        name = CompareWithResourcesPlayer(bitmap);
                    else
                        name = CompareWithResources(bitmap);

                    cards.Add(new Data.Card(name));
                }
                catch (NoCardException)
                {
                    return new List<Data.Card>();
                }
            }
            return cards;
        }

        private static string CompareWithResourcesPlayer(Bitmap source)
        {
            String playerCard = "not found";
            var resSet = Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            float highestSimilarity = 0.0f;
            string bestCardFound = "not found";
            string bestSuitFound = "not found";

            foreach (DictionaryEntry entry in resSet)
            {
                var value = entry.Value as Bitmap;
                if (value == null) continue;
                var cmp = value;
                var entryStr = entry.Key.ToString();

                //Get Player Card
                if (entryStr == "AR" || entryStr == "2R" || entryStr == "3R" || entryStr == "4R" ||
                    entryStr == "5R" || entryStr == "6R" || entryStr == "7R" || entryStr == "8R" ||
                    entryStr == "9R" || entryStr == "TR" || entryStr == "JR" || entryStr == "QR" ||
                    entryStr == "KR" ||
                    entryStr == "AB" || entryStr == "2B" || entryStr == "3B" || entryStr == "4B" ||
                    entryStr == "5B" || entryStr == "6B" || entryStr == "7B" || entryStr == "8B" ||
                    entryStr == "9B" || entryStr == "TB" || entryStr == "JB" || entryStr == "QB" ||
                    entryStr == "KB")
                {
                    float curSimilarity = FindBitmap(source, cmp);
                    if (curSimilarity > highestSimilarity && curSimilarity > 0.80f)
                    {
                        highestSimilarity = curSimilarity;
                        bestCardFound = entry.Key.ToString().Replace("R", "").Replace("B", "");
                    }
                }
            }
            
            highestSimilarity = 0.0f;

            foreach (DictionaryEntry entry in resSet)
            {
                var value = entry.Value as Bitmap;
                if (value == null) continue;
                var cmp = value;
                var entryStr = entry.Key.ToString();

                //Get Player Card Suit.
                if( entryStr == "H1" || entryStr == "H2" ||
                    entryStr == "D1" || entryStr == "D2" || entryStr == "D3" ||
                    entryStr == "C1" || entryStr == "C2" || entryStr == "C3" ||
                    entryStr == "S1" || entryStr == "S2" )
                {
                    float curSimilarity = FindBitmap(source, cmp);
                    if (curSimilarity > highestSimilarity && curSimilarity > 0.80f)
                    {
                        highestSimilarity = curSimilarity;
                        bestSuitFound = entry.Key.ToString().Replace("1", "").Replace("2", "").Replace("3", "");
                    }
                }
            }

            return bestCardFound + bestSuitFound;
        }

        private static string CompareWithResources(Bitmap source)
        {
            var resSet = Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            float highestSimilarity = 0.0f;
            string bestCardFound = "not found";

            foreach (DictionaryEntry entry in resSet)
            {
                var value = entry.Value as Bitmap;
                if (value == null) continue;
                var cmp = value;
                var entryStr = entry.Key.ToString();

                //Only do table cards.
                if (entryStr == "AH" || entryStr == "2H" || entryStr == "3H" || entryStr == "4H" || entryStr == "5H" || entryStr == "6H" || entryStr == "7H" || entryStr == "8H" || entryStr == "9H" || entryStr == "TH" || entryStr == "JH" || entryStr == "QH" || entryStr == "KH" ||
                    entryStr == "AC" || entryStr == "2C" || entryStr == "3C" || entryStr == "4C" || entryStr == "5C" || entryStr == "6C" || entryStr == "7C" || entryStr == "8C" || entryStr == "9C" || entryStr == "TC" || entryStr == "JC" || entryStr == "QC" || entryStr == "KC" ||
                    entryStr == "AS" || entryStr == "2S" || entryStr == "3S" || entryStr == "4S" || entryStr == "5S" || entryStr == "6S" || entryStr == "7S" || entryStr == "8S" || entryStr == "9S" || entryStr == "TS" || entryStr == "JS" || entryStr == "QS" || entryStr == "KS" ||
                    entryStr == "AD" || entryStr == "2D" || entryStr == "3D" || entryStr == "4D" || entryStr == "5D" || entryStr == "6D" || entryStr == "7D" || entryStr == "8D" || entryStr == "9D" || entryStr == "TD" || entryStr == "JD" || entryStr == "QD" || entryStr == "KD")
                {
                    float curSimilarity = FindBitmap(source, cmp);
                    if (curSimilarity > highestSimilarity && curSimilarity > 0.90f)
                    {
                        highestSimilarity = curSimilarity;
                        bestCardFound = entry.Key.ToString();
                    }
                }
            }

            return bestCardFound;
        }


        public int GetTableCardAmount()
        {
            //HighGamer FIXED this 100%
            var checkBitmap = Screenshot.GetRelativeScreenshot(Positions._relativeTableCardPos,
                new Size(5 * Positions.TableCardMargin, Positions._cropSize.Height), _pokerHandle);

            var checkX = Positions._relativeCheckCountCardPos.X;
            var checkCount = 0;
            var pixel = checkBitmap.GetPixel(checkX, Positions._relativeCheckCountCardPos.Y);
            while (pixel.R == 255 && pixel.G == 255 && pixel.B == 255 && checkCount < 5)
            {
                checkCount++;

                if (checkCount < 5)
                {
                    checkX += Positions.TableCardMargin;
                    pixel = checkBitmap.GetPixel(checkX, Positions._relativeCheckCountCardPos.Y);
                }
            }
            return checkCount;
        }

        private static float FindBitmap(Bitmap bmpNeedle, Bitmap bmpHaystack, bool isLowQuality = false)
        {

            //if (bmpNeedle.Height != bmpHaystack.Height || bmpNeedle.Width != bmpHaystack.Width)
            //    throw new Exception("Can't compare two bitmaps with different sizes!");

            Bitmap sourceImage = (Bitmap)bmpNeedle;
            Bitmap template = (Bitmap)bmpHaystack;

            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            // apply the filter
            sourceImage = filter.Apply(sourceImage);
            template = filter.Apply(template);

            // create template matching algorithm's instance
            // (set similarity threshold to 0.50%)

            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.5f);
            // find all matchings with specified above similarity

            TemplateMatch[] matchings = tm.ProcessImage(sourceImage, template);
            // highlight found matchings

            BitmapData data = sourceImage.LockBits(
                 new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                 ImageLockMode.ReadWrite, sourceImage.PixelFormat);
            foreach (TemplateMatch m in matchings)
            {
                //MessageBox.Show(m.Rectangle.Location.ToString());
                return m.Similarity;
            }
            sourceImage.UnlockBits(data);
            return 0.0f;

            /*

            var pixelAmount = bmpHaystack.Width * bmpHaystack.Height;
            var pixelCount = 0;

            for (var x = 0; x < bmpNeedle.Width; x++)
            for (var y = 0; y < bmpNeedle.Height; y++)
            {
                var cNeedle = bmpNeedle.GetPixel(x, y);
                var cHaystack = bmpHaystack.GetPixel(x, y);

                if (cNeedle.R == cHaystack.R && cNeedle.G == cHaystack.G && cNeedle.B == cHaystack.B)
                    pixelCount++;
            }
            if (pixelAmount == pixelCount)
                return true;
            return false;
            */
        }
    }
}