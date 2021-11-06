using System;
using System.Drawing;


namespace ImViewLite.Helpers
{
    public struct CMYK
    {
        public static readonly CMYK Empty;


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
        /// A value from 0 - 1 representing the C value of this color.
        /// </summary>
        public float C
        {
            get { return c; }
            set { c = value.Clamp<float>(0, 1); }
        }

        /// <summary>
        /// A value from 0 - 100 representing the C value of this color.
        /// </summary>
        public float C100
        {
            get { return c * 100f; }
            set { c = (value / 100f).Clamp<float>(0, 1); }
        }
        private float c;



        /// <summary>
        /// A value from 0 - 1 representing the M value of this color.
        /// </summary>
        public float M
        {
            get { return m; }
            set { m = value.Clamp<float>(0, 1); }
        }

        /// <summary>
        /// A value from 0 - 100 representing the M value of this color.
        /// </summary>
        public float M100
        {
            get { return m * 100f; }
            set { m = (value / 100f).Clamp<float>(0, 1); }
        }
        private float m;



        /// <summary>
        /// A value from 0 - 1 representing the Y value of this color.
        /// </summary>
        public float Y
        {
            get { return y; }
            set { y = value.Clamp<float>(0, 1); }
        }

        /// <summary>
        /// A value from 0 - 100 representing the Y value of this color.
        /// </summary>
        public float Y100
        {
            get { return y * 100f; }
            set { y = (value / 100f).Clamp<float>(0, 1); }
        }
        private float y;



        /// <summary>
        /// A value from 0 - 1 representing the K value of this color.
        /// </summary>
        public float K
        {
            get { return k; }
            set { k = value.Clamp<float>(0, 1); }
        }

        /// <summary>
        /// A value from 0 - 100 representing the K value of this color.
        /// </summary>
        public float K100
        {
            get { return k * 100f; }
            set { k = (value / 100f).Clamp<float>(0, 1); }
        }
        private float k;


        /// <summary>
        /// Creates a new instance of the CMYK struct with the given System.Drawing.Color.
        /// </summary>
        /// <param name="color">A System.Drawing.Color</param>
        public CMYK(Color color) : this(color.R, color.G, color.B, color.A)
        {
        }

        /// <summary>
        /// Creates a new instance of the CMYK struct with the given R G B A values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        /// <param name="A">a value from 0 - 255</param>
        /// <returns>An CMYK color struct.</returns>
        public CMYK(byte R, byte G, byte B, byte A = 255)
        {
            if (R == 0 && G == 0 && B == 0)
            {
                c = 0;
                m = 0;
                y = 0;
                k = 1;
                this.a = (byte)A;
                return;
            }
            

            float modifiedR, modifiedG, modifiedB;

            modifiedR = R / 255f;
            modifiedG = G / 255f;
            modifiedB = B / 255f;

            float max = 0;
            foreach (float i in new float[] { modifiedR, modifiedG, modifiedB })
                if (i > max)
                    max = i;

            k = 1f - max; // - new List<float>() { modifiedR, modifiedG, modifiedB }.Max();
            c = (1f - modifiedR - k) / (1f - k);
            m = (1f - modifiedG - k) / (1f - k);
            y = (1f - modifiedB - k) / (1f - k);
            
            this.a = (byte)A;
        }

        /// <summary>
        /// Creates a new instance of the CMYK struct with the given C M Y K A values.
        /// <para>Where C is a value from 0 - 100</para>
        /// <para>Where M is a value from 0 - 100</para>
        /// <para>Where Y is a value from 0 - 100</para>
        /// <para>Where K is a value from 0 - 100</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="C">a value from 0 - 100</param>
        /// <param name="M">a value from 0 - 100</param>
        /// <param name="Y">a value from 0 - 100</param>
        /// <param name="K">a value from 0 - 100</param>
        /// <param name="A">a value from 0 - 255</param>
        public CMYK(int C, int M, int Y, int K, int A = 255) : this()
        {
            C100 = C;
            M100 = M;
            Y100 = Y;
            K100 = K;
            this.A = (byte)A;
        }

        /// <summary>
        /// Creates a new instance of the CMYK struct with the given C M Y K A values.
        /// <para>Where C is a value from 0 - 100f</para>
        /// <para>Where M is a value from 0 - 100f</para>
        /// <para>Where Y is a value from 0 - 100f</para>
        /// <para>Where K is a value from 0 - 100f</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="C">a value from 0 - 100f</param>
        /// <param name="M">a value from 0 - 100f</param>
        /// <param name="Y">a value from 0 - 100f</param>
        /// <param name="K">a value from 0 - 100f</param>
        /// <param name="A">a value from 0 - 255</param>
        public CMYK(float C, float M, float Y, float K, float A = 255) : this()
        {
            C100 = C;
            M100 = M;
            Y100 = Y;
            K100 = K;
            this.A = (byte)A;
        }

        

        public static implicit operator CMYK(Color color)
        {
            return new CMYK(color);
        }

        public static implicit operator Color(CMYK color)
        {
            return color.ToColor();
        }

        public static implicit operator ARGB(CMYK color)
        {
            return color.ToColor();
        }

        public static implicit operator HSB(CMYK color)
        {
            return color.ToHSB();
        }

        public static implicit operator HSL(CMYK color)
        {
            return color.ToHSL();
        }

        public static bool operator ==(CMYK left, CMYK right)
        {
            return  (left.C == right.C) && 
                    (left.M == right.M) && 
                    (left.Y == right.Y) && 
                    (left.K == right.K) && 
                    (left.A == right.A);
        }

        public static bool operator !=(CMYK left, CMYK right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Converts the current color to a System.Drawing.Color.
        /// </summary>
        /// <returns>A System.Drawing.Color representation of this color.</returns>
        public Color ToColor()
        {
            double r, g, b;
            r = Math.Round(255 * (1 - C) * (1 - k));
            g = Math.Round(255 * (1 - M) * (1 - K));
            b = Math.Round(255 * (1 - Y) * (1 - K));
            return Color.FromArgb(A, (int)r, (int)g, (int)b);
        }

        /// <summary>
        /// Converts the current color to an ARGB color.
        /// </summary>
        /// <returns>An ARGB representation of this color.</returns>
        public ARGB ToARGB()
        {
            return this.ToColor();
        }

        /// <summary>
        /// Converts the current color to an HSB color.
        /// </summary>
        /// <returns>An HSB representation of this color.</returns>
        public HSB ToHSB()
        {
            return new HSB(this.ToColor());
        }

        /// <summary>
        /// Converts the current color to an HSL color.
        /// </summary>
        /// <returns>An HSL representation of this color.</returns>
        public HSL ToHSL()
        {
            return new HSL(this.ToColor());
        }

        /// <summary>
        /// Creates a new instance of the CMYK struct with the given R G B A values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        /// <param name="A">a value from 0 - 255</param>
        /// <returns>An CMYK color struct.</returns>
        public CMYK FromArgb(int A, int R, int G, int B)
        {
            return new CMYK((byte)R, (byte)G, (byte)B, (byte)A);
        }

        /// <summary>
        /// Creates a new instance of the CMYK struct with the given R G B A values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        /// <returns>An CMYK color struct.</returns>
        public CMYK FromArgb(int R, int G, int B)
        {
            return new CMYK((byte)R, (byte)G, (byte)B);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}",
                Math.Round(C100, ColorHelper.DecimalPlaces),
                Math.Round(M100, ColorHelper.DecimalPlaces),
                Math.Round(Y100, ColorHelper.DecimalPlaces),
                Math.Round(K100, ColorHelper.DecimalPlaces));
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
