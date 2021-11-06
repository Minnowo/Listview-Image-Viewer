using System;
using System.Drawing;

namespace ImViewLite.Helpers
{
    public struct HSB
    {
        public static readonly HSB Empty;


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
            set { saturation = (value / 100f).Clamp<float>(0, 1); }
        }
        private float saturation;



        /// <summary>
        /// A value from 0 - 1 representing the Brightness of this color.
        /// </summary>
        public float Brightness
        {
            get { return brightness; }
            set { brightness = value.Clamp<float>(0, 1); }
        }

        /// <summary>
        /// A value from 0 - 100 representing the Brightness of this color.
        /// </summary>
        public float Brightness100
        {
            get { return brightness * 100f; }
            set { brightness = (value / 100f).Clamp<float>(0, 1); }
        }
        private float brightness;


        /// <summary>
        /// Creates a new instance of the HSB struct with the given H S B A values.
        /// <para>Where H is a value from 0 - 360f</para>
        /// <para>Where S is a value from 0 - 100f</para>
        /// <para>Where B is a value from 0 - 100f</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="H">A value from 0 - 360f</param>
        /// <param name="S">a value from 0 - 100f</param>
        /// <param name="B">a value from 0 - 100f</param>
        /// <param name="A">a value from 0 - 255</param>
        public HSB(float H, float S, float B, int A = 255) : this()
        {
            Hue360 = H;
            Saturation100 = S;
            Brightness100 = B;
            this.A = (byte)A;
        }

        /// <summary>
        /// Creates a new instance of the HSB struct with the given H S B A values.
        /// <para>Where H is a value from 0 - 360</para>
        /// <para>Where S is a value from 0 - 100</para>
        /// <para>Where B is a value from 0 - 100</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="H">A value from 0 - 360</param>
        /// <param name="S">a value from 0 - 100</param>
        /// <param name="B">a value from 0 - 100</param>
        /// <param name="A">a value from 0 - 255</param>
        public HSB(int H, int S, int B, int A = 255) : this()
        {
            Hue360 = H;
            Saturation100 = S;
            Brightness100 = B;
            this.A = (byte)A;
        }

        /// <summary>
        /// Creates a new instance of the HSB struct with the given System.Drawing.Color.
        /// </summary>
        /// <param name="color">A System.Drawing.Color color.</param>
        public HSB(Color color) : this(color.R, color.G, color.B, color.A)
        {
        }

        /// <summary>
        /// Creates a new instance of the HSB struct with the given R G B A values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        /// <param name="A">a value from 0 - 255</param>
        public HSB(byte R, byte G, byte B, byte A = 255)
        {
            this.a = (byte)A;

            float newR = R / 255f;
            float newG = G / 255f;
            float newB = B / 255f;
            float min = newR;

            foreach(float i in new float[] { newG, newB })
                if (i < min)
                    min = i;
            

            if (newR >= newB && newR >= newG) // newR > than both
            {
                if ((newR - min) != 0) // cannot divide by 0 
                {
                    // divide by 6 because if you don't hue * 60 = 0-360, but we want hue * 360 = 0-360
                    hue = (((newG - newB) / (newR - min)) % 6) / 6;

                    // if its negative add 360 because 360/360 = 1
                    if (hue < 0)
                    {
                        hue += 1;
                    }
                }
                else
                {
                    hue = 0;
                }

                if (newR == 0)
                {
                    saturation = 0f;
                }
                else
                {
                    saturation = (newR - min) / newR;
                }

                brightness = newR;
            }
            else if (newB > newG) // newB > both
            {
                // don't have to worry about dividing by 0 because if max == min the if statement above is true
                hue = (4.0f + (newR - newG) / (newB - min)) / 6; // still dividing by 6

                if (newB == 0)
                {
                    saturation = 0f;
                }
                else
                {
                    saturation = (newB - min) / newB;
                }

                brightness = newB;
            }
            else // newG > both
            {
                hue = (2.0f + (newB - newR) / (newG - min)) / 6;
                
                if (newG == 0)
                {
                    saturation = 0f;
                }
                else
                {
                    saturation = (newG - min) / newG;
                }
                
                brightness = newG;
            }
        }

        

        public static implicit operator HSB(Color color)
        {
            return new HSB(color);
        }

        public static implicit operator Color(HSB color)
        {
            return color.ToColor();
        }

        public static implicit operator ARGB(HSB color)
        {
            return color.ToColor();
        }

        public static implicit operator HSL(HSB color)
        {
            return color.ToHSL();
        }

        public static implicit operator CMYK(HSB color)
        {
            return color.ToCMYK();
        }

        public static bool operator ==(HSB left, HSB right)
        {
            return  (left.Hue == right.Hue) && 
                    (left.Saturation == right.Saturation) && 
                    (left.Brightness == right.Brightness) && 
                    (left.A == right.A);
        }

        public static bool operator !=(HSB left, HSB right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Converts the current color to a System.Drawing.Color.
        /// </summary>
        /// <returns>A System.Drawing.Color representation of this color.</returns>
        public Color ToColor()
        {
            float c, x, m, r = 0, g = 0, b = 0;
            c = brightness * saturation;
            x = c * (1 - Math.Abs(Hue360 / 60 % 2 - 1));
            m = brightness - c;

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
        /// Converts the current color to a HSL color.
        /// </summary>
        /// <returns>An HSL representation of this color.</returns>
        public HSL ToHSL()
        {
            return new HSL(this.ToColor());
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
        /// Inverts the current color.
        /// </summary>
        /// <returns>The inverted color.</returns>
        public HSB GetInverted()
        {
            return HSB.GetInverted(this);
        }

        /// <summary>
        /// Creates a new HSB color from the given HSBA.
        /// <para>Where H is a value from 0 - 360f</para>
        /// <para>Where S is a value from 0 - 100f</para>
        /// <para>Where B is a value from 0 - 100f</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="H">A value from 0 - 360f</param>
        /// <param name="S">A value from 0 - 360f</param>
        /// <param name="B">A value from 0 - 360f</param>
        /// <param name="A">A value from 0 - 255</param>
        /// <returns>An HSB color struct.</returns>
        public static HSB FromHSB360(float H, float S, float B, int A = 255)
        {
            return new HSB(H, S, B, A);
        }

        /// <summary>
        /// Creates a new HSB color from the given HSBA.
        /// <para>Where H is a value from 0 - 1f</para>
        /// <para>Where S is a value from 0 - 1f</para>
        /// <para>Where B is a value from 0 - 1f</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="H">A value from 0 - 1f</param>
        /// <param name="S">A value from 0 - 1f</param>
        /// <param name="B">A value from 0 - 1f</param>
        /// <param name="A">A value from 0 - 255</param>
        /// <returns>An HSB color struct.</returns>
        public static HSB FromHSB0(float H, float S, float B, int A = 255)
        {
            return new HSB(H * 360f, S * 100, B * 100, A);
        }

        /// <summary>
        /// Creates a new instance of the HSB struct with the given R G B A values.
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
        public static HSB FromARGB(int A, int R, int G, int B)
        {
            return new HSB((byte)R, (byte)G, (byte)B, (byte)A);
        }

        /// <summary>
        /// Creates a new instance of the HSB struct with the given R G B A values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        /// <returns>An HSB color struct.</returns>
        public static HSB FromARGB(int R, int G, int B)
        {
            return new HSB((byte)R, (byte)G, (byte)B, (byte)255);
        }

        /// <summary>
        /// Gets the inverted HSB color.
        /// </summary>
        /// <param name="input">The color to invert.</param>
        /// <returns>An inverted HSB struct.</returns>
        public static HSB GetInverted(HSB input)
        {
            float hue = input.Hue;

            if (hue < 0.5f)
            {
                hue += 0.5f;
            }
            else
            {
                hue -= 0.5f;
            }

            return new HSB(hue * 360, input.Saturation100, input.Brightness100, input.A);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}",
                Math.Round(Hue360, ColorHelper.DecimalPlaces),
                Math.Round(Saturation100, ColorHelper.DecimalPlaces),
                Math.Round(Brightness100, ColorHelper.DecimalPlaces));
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
