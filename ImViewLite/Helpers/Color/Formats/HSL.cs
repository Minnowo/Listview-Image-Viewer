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
    public struct HSL
    {
        public static readonly HSL Empty;


        /// <summary>
        /// A bool indicating if the color is transparent.
        /// </summary>
        public bool isTransparent
        {
            get
            {
                return A < 255;
            }
        }


        /// <summary>
        /// A value from 0 - 255 representing the Alpha value of this color.
        /// </summary>
        public byte A
        {
            get { return a; }
            set { a = value.Clamp<byte>(0, 255); }
        }
        private byte a;



        /// <summary>
        /// A value from 0 - 1 representing the Hue of this color.
        /// </summary>
        public float Hue
        {
            get { return hue; }
            set { hue = value.Clamp<float>(0, 1); }
        }

        /// <summary>
        /// A value from 0 - 360 representing the Hue of this color.
        /// </summary>
        public float Hue360
        {
            get { return hue * 360f; }
            set { hue = (value / 360f).Clamp<float>(0, 1); }
        }
        private float hue;



        /// <summary>
        /// A value from 0 - 1 representing the Saturation of this color.
        /// </summary>
        public float Saturation
        {
            get { return saturation; }
            set { saturation = value.Clamp<float>(0, 1); }
        }

        /// <summary>
        /// A value from 0 - 100 representing the Saturation of this color.
        /// </summary>
        public float Saturation100
        {
            get { return saturation * 100f; }
            set { saturation = (value / 100f).Clamp<float>(0, 1); ; }
        }
        private float saturation;



        /// <summary>
        /// A value from 0 - 1 representing the Lightness of this color.
        /// </summary>
        public float Lightness
        {
            get { return lightness; }
            set { lightness = value.Clamp<float>(0, 1); }
        }

        /// <summary>
        /// A value from 0 - 100 representing the Lightness of this color.
        /// </summary>
        public float Lightness100
        {
            get { return lightness * 100f; }
            set { lightness = (value / 100f).Clamp<float>(0, 1); ; }
        }
        private float lightness;


        /// <summary>
        /// Creates a new instance of the HSL struct with the given System.Drawing.Color.
        /// </summary>
        /// <param name="color">A System.Drawing.Color color.</param>
        public HSL(Color color) : this()
        {
            Hue360 = color.GetHue();
            Saturation = color.GetSaturation();
            Lightness = color.GetBrightness();
            A = color.A;
        }

        /// <summary>
        /// Creates a new instance of the HSL struct with the given R G B A values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        /// <param name="A">a value from 0 - 255</param>
        public HSL(byte R, byte G, byte B, byte A = 255) : this(Color.FromArgb(A, R, G, B))
        {
        }

        /// <summary>
        /// Creates a new instance of the HSL struct with the given H S L A values.
        /// <para>Where H is a value from 0 - 360</para>
        /// <para>Where S is a value from 0 - 100</para>
        /// <para>Where B is a value from 0 - 100</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="H">A value from 0 - 360</param>
        /// <param name="S">a value from 0 - 100</param>
        /// <param name="L">a value from 0 - 100</param>
        /// <param name="A">a value from 0 - 255</param>
        public HSL(int H, int S, int L, int A = 255) : this()
        {
            Hue360 = H;
            Saturation100 = S;
            Lightness100 = L;
            this.A = (byte)A;
        }

        /// <summary>
        /// Creates a new instance of the HSL struct with the given H S L A values.
        /// <para>Where H is a value from 0 - 360f</para>
        /// <para>Where S is a value from 0 - 100f</para>
        /// <para>Where B is a value from 0 - 100f</para>
        /// <para>Where A is a value from 0 - 255f</para>
        /// </summary>
        /// <param name="H">A value from 0 - 360f</param>
        /// <param name="S">a value from 0 - 100f</param>
        /// <param name="L">a value from 0 - 100f</param>
        /// <param name="A">a value from 0 - 255</param>
        public HSL(float H, float S, float L, int A = 255) : this()
        {
            Hue360 = H;
            Saturation100 = S;
            Lightness100 = L;
            this.A = (byte)A;
        }

        

        public static implicit operator HSL(Color color)
        {
            return new HSL(color);
        }

        public static implicit operator Color(HSL color)
        {
            return color.ToColor();
        }

        public static implicit operator ARGB(HSL color)
        {
            return color.ToColor();
        }

        public static implicit operator CMYK(HSL color)
        {
            return color.ToCMYK();
        }

        public static implicit operator HSB(HSL color)
        {
            return color.ToHSB();
        }

        public static bool operator ==(HSL left, HSL right)
        {
            return  (left.Hue == right.Hue) && 
                    (left.Saturation == right.Saturation) && 
                    (left.Lightness == right.Lightness) && 
                    (left.A == right.A);
        }

        public static bool operator !=(HSL left, HSL right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Converts the current color to a System.Drawing.Color.
        /// </summary>
        /// <returns>A System.Drawing.Color representation of this color.</returns>
        public Color ToColor()
        {
            double c, x, m, r = 0, g = 0, b = 0;
            c = (1.0 - Math.Abs(2 * lightness - 1.0)) * saturation;
            x = c * (1.0 - Math.Abs(Hue360 / 60 % 2 - 1.0));
            m = lightness - c / 2.0;

            if (Hue360 <= 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (Hue360 <= 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (Hue360 <= 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (Hue360 <= 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (Hue360 <= 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else if (Hue360 <= 360)
            {
                r = c;
                g = 0;
                b = x;
            }

            return Color.FromArgb(A,
                (int)Math.Round((r + m) * 255),
                (int)Math.Round((g + m) * 255),
                (int)Math.Round((b + m) * 255));
        }

        /// <summary>
        /// Converts the current color to an ARGB color.
        /// </summary>
        /// <returns>An ARGB representation of this color.</returns>
        public ARGB ToARGB()
        {
            return new ARGB(this.ToColor());
        }

        /// <summary>
        /// Converts the current color to a HSB color.
        /// </summary>
        /// <returns>An HSB representation of this color.</returns>
        public HSB ToHSB()
        {
            return new HSB(this.ToColor());
        }

        /// <summary>
        /// Converts the current color to a CMYK color.
        /// </summary>
        /// <returns>An CMYK representation of this color.</returns>
        public CMYK ToCMYK()
        {
            return new CMYK(this.ToColor());
        }

        /// <summary>
        /// Creates a new HSB color from the given HSBA.
        /// <para>Where H is a value from 0 - 360f</para>
        /// <para>Where S is a value from 0 - 100f</para>
        /// <para>Where L is a value from 0 - 100f</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="H">A value from 0 - 360f</param>
        /// <param name="S">A value from 0 - 360f</param>
        /// <param name="B">A value from 0 - 360f</param>
        /// <param name="A">A value from 0 - 255</param>
        /// <returns>An HSB color struct.</returns>
        public static HSB FromHSL360(float H, float S, float L, int A = 255)
        {
            return new HSB(H, S, L, A);
        }

        /// <summary>
        /// Creates a new HSB color from the given HSBA.
        /// <para>Where H is a value from 0 - 1f</para>
        /// <para>Where S is a value from 0 - 1f</para>
        /// <para>Where L is a value from 0 - 1f</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="H">A value from 0 - 1f</param>
        /// <param name="S">A value from 0 - 1f</param>
        /// <param name="L">A value from 0 - 1f</param>
        /// <param name="A">A value from 0 - 255</param>
        /// <returns>An HSB color struct.</returns>
        public static HSB FromHSL0(float H, float S, float L, int A = 255)
        {
            return new HSB(H * 360f, S * 100, L * 100, A);
        }

        /// <summary>
        /// Creates a new instance of the HSL struct with the given R G B A values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        /// <param name="A">a value from 0 - 255</param>
        /// <returns>An HSB color struct.</returns>
        public static HSL FromARGB(int A, int R, int G, int B)
        {
            return new HSL((byte)R, (byte)G, (byte)B, (byte)A);
        }

        /// <summary>
        /// Creates a new instance of the HSL struct with the given R G B A values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        /// <returns>An HSB color struct.</returns>
        public static HSL FromARGB(int R, int G, int B)
        {
            return new HSL((byte)R, (byte)G, (byte)B, (byte)255);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}",
                Math.Round(Hue360, ColorHelper.DecimalPlaces),
                Math.Round(Saturation100, ColorHelper.DecimalPlaces),
                Math.Round(Lightness100, ColorHelper.DecimalPlaces));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
