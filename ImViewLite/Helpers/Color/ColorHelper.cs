using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;

namespace ImViewLite.Helpers
{
    public static class ColorHelper
    {
        public static int DecimalPlaces { get; set; } = 2;

        public static Color AskChooseColor()
        {
            return AskChooseColor(Color.Empty);
        }

        public static Color AskChooseColor(Color initColor)
        {
            using (ColorPickerForm f = new ColorPickerForm())
            {
                f.TopMost = true;
                f.UpdateColors(initColor);
                f.ShowDialog();

                return f.GetCurrentColor();
            }
        }

        public static string ColorToHex(Color color, ColorFormat format = ColorFormat.RGB)
        {
            return ColorToHex(color.R, color.G, color.B, color.A, format);
        }

        public static string ColorToHex(int r, int g, int b, int a = 255, ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
                case ColorFormat.ARGB:
                    return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", a, r, g, b);
            }
        }

        public static int ColorToDecimal(Color color, ColorFormat format = ColorFormat.RGB)
        {
            return ColorToDecimal(color.R, color.G, color.B, color.A, format);
        }

        public static int ColorToDecimal(int r, int g, int b, int a = 255, ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return r << 16 | g << 8 | b;
                case ColorFormat.ARGB:
                    return a << 24 | r << 16 | g << 8 | b;
            }
        }

        public static Color DecimalToColor(int dec, ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return Color.FromArgb((dec >> 16) & 0xFF, (dec >> 8) & 0xFF, dec & 0xFF);
                case ColorFormat.ARGB:
                    return Color.FromArgb((dec >> 24) & 0xFF, (dec >> 16) & 0xFF, (dec >> 8) & 0xFF, dec & 0xFF);
            }
        }

        public static bool ParseCMYK(string input, out CMYK color)
        {
            Match matchCMYK = Regex.Match(input, @"^([0-9]?[0-9](?:[.][0-9]?[0-9]?[0-9]|)|100)(?:\s|,)+([0-9]?[0-9](?:[.][0-9]?[0-9]?[0-9]|)|100)(?:\s|,)+([0-9]?[0-9](?:[.][0-9]?[0-9]?[0-9]|)|100)(?:\s|,)+([0-9]?[0-9](?:[.][0-9]?[0-9]?[0-9]|)|100)(?:\s)?$");
            if (matchCMYK.Success)
            {
                color = new CMYK(float.Parse(matchCMYK.Groups[1].Value), float.Parse(matchCMYK.Groups[2].Value), float.Parse(matchCMYK.Groups[3].Value), float.Parse(matchCMYK.Groups[4].Value));
                return true;
            }
            color = Color.Empty;
            return false;
        }

        public static bool ParseRGB(string input, out Color color)
        {
            Match matchRGB = Regex.Match(input, @"^([1]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])(?:\s|,)+([1]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])(?:\s|,)+([1]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])(?:\s)?$");
            if (matchRGB.Success)
            {
                color = Color.FromArgb(int.Parse(matchRGB.Groups[1].Value), int.Parse(matchRGB.Groups[2].Value), int.Parse(matchRGB.Groups[3].Value));
                return true;
            }
            color = Color.Empty;
            return false;
        }

        public static bool ParseHex(string input, out Color color)
        {
            Match matchHex = Regex.Match(input, @"^(?:#|0x)?((?:[0-9A-Fa-f]{2}){3})$");
            if (matchHex.Success)
            {
                color = HexToColor(matchHex.Groups[1].Value);
                return true;
            }
            color = Color.Empty;
            return false;
        }

        public static bool ParseHSB(string input, out HSB color)
        {
            Match matchHSB = Regex.Match(input, @"^([1-2]?[0-9]?[0-9](?:[.][0-9]?[0-9]?[0-9]|)|3[0-5][0-9](?:[.][0-9]?[0-9]?[0-9]|)|360)(?:\s|,)+([0-9]?[0-9](?:[.][0-9]?[0-9]?[0-9]|)|100)(?:\s|,)+([0-9]?[0-9](?:[.][0-9]?[0-9]?[0-9]|)|100)(?:\s)?$");
            if (matchHSB.Success)
            {
                color = new HSB(float.Parse(matchHSB.Groups[1].Value), float.Parse(matchHSB.Groups[2].Value), float.Parse(matchHSB.Groups[3].Value));
                return true;
            }
            color = HSB.Empty;
            return false;
        }

        public static bool ParseHSL(string input, out HSL color)
        {
            Match matchHSL = Regex.Match(input, @"^([1-2]?[0-9]?[0-9](?:[.][0-9]?[0-9]?[0-9]|)|3[0-5][0-9](?:[.][0-9]?[0-9]?[0-9]|)|360)(?:\s|,)+([0-9]?[0-9](?:[.][0-9]?[0-9]?[0-9]|)|100)(?:\s|,)+([0-9]?[0-9](?:[.][0-9]?[0-9]?[0-9]|)|100)(?:\s)?$");
            if (matchHSL.Success)
            {
                color = new HSL(float.Parse(matchHSL.Groups[1].Value), float.Parse(matchHSL.Groups[2].Value), float.Parse(matchHSL.Groups[3].Value));
                return true;
            }
            color = HSL.Empty;
            return false;
        }


        public static Color HexToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return Color.Empty;
            }

            if (hex[0] == '#')
            {
                hex = hex.Remove(0, 1);
            }
            else if (hex.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                hex = hex.Remove(0, 2);
            }
            try
            {
                return ColorTranslator.FromHtml("#" + hex);
            }
            catch
            {
                return Color.Empty;
            }
        }

        public static Color Subtract(Color a, Color b)
        {
            return Color.FromArgb(Math.Abs(a.R - b.R), Math.Abs(a.G - b.G), Math.Abs(a.B - b.B));
        }

        public static Color Invert(Color a)
        {
            if (a.A < 255)
            {
                return Color.FromArgb(Math.Abs(a.A - a.R), Math.Abs(a.A - a.G), Math.Abs(a.A - a.B));
            }
            else
            {
                return Color.FromArgb(255 - a.R, 255 - a.G, 255 - a.B);
            }
        }
    }
}
