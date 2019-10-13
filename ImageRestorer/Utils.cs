using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageRestorer
{
    public static class Utils
    {
        public enum ConnectDirection
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3,
        }
        public static readonly Random random = new Random();
        public static Int64 GetColorDistance(Color color1, Color color2)
        {
            //return (Int64)(Math.Pow(Math.Abs(color1.R - color2.R), 2.0) + Math.Pow(Math.Abs(color1.G - color2.G), 2.0) + Math.Pow(Math.Abs(color1.B - color2.B), 2.0));
            //return Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B);
            return Math.Max(Math.Max(Math.Abs(color1.R - color2.R), Math.Abs(color1.G - color2.G)), Math.Abs(color1.B - color2.B)) * 256 + Math.Min(Math.Min(Math.Abs(color1.R - color2.R), Math.Abs(color1.G - color2.G)), Math.Abs(color1.B - color2.B));
            //return Math.Max(Math.Max(Math.Abs(color1.R - color2.R), Math.Abs(color1.G - color2.G)), Math.Abs(color1.B - color2.B));
            //return (Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B)) / 16;
            //return Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B) + random.Next(0, 500);
            //return Math.Abs(color1.R + color1.G + color1.B - color2.R - color2.G - color2.B);
        }
        public static Int64 GetScoreBetween(ConnectDirection direction, Bitmap tile1, Bitmap tile2)
        {
            Int64 score = 0;
            int size = tile1.Width;
            switch (direction)
            {
                case ConnectDirection.Right:
                    for (int i = 0; i < size; i++)
                        score += Utils.GetColorDistance(tile1.GetPixel(size - 1, i), tile2.GetPixel(0, i));
                    break;
                case ConnectDirection.Left:
                    for (int i = 0; i < size; i++)
                        score += Utils.GetColorDistance(tile1.GetPixel(0, i), tile2.GetPixel(size - 1, i));
                    break;
                case ConnectDirection.Bottom:
                    for (int i = 0; i < size; i++)
                        score += Utils.GetColorDistance(tile1.GetPixel(i, size - 1), tile2.GetPixel(i, 0));
                    break;
                case ConnectDirection.Top:
                    for (int i = 0; i < size; i++)
                        score += Utils.GetColorDistance(tile1.GetPixel(i, 0), tile2.GetPixel(i, size - 1));
                    break;
            }
            return score;
        }
    }
}
