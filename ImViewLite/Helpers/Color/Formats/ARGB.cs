using System.Drawing;

namespace ImViewLite.Helpers
{

    public struct ARGB
    {
        public static readonly ARGB Empty;


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
        /// A value from 0 - 255 representing the Red value of this color.
        /// </summary>
        public byte R
        {
            get { return r; }
            set { r = value.Clamp<byte>(0, 255); }
        }
        private byte r;


        /// <summary>
        /// A value from 0 - 255 representing the Green value of this color.
        /// </summary>
        public byte G
        {
            get { return g; }
            set { g = value.Clamp<byte>(0, 255); }
        }
        private byte g;


        /// <summary>
        /// A value from 0 - 255 representing the Blue value of this color.
        /// </summary>
        public byte B
        {
            get { return b; } 
            set { b = value.Clamp<byte>(0, 255); }
        }
        private byte b;


        /// <summary>
        /// Creates a new instance of the ARGB struct from the given RGB values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        public ARGB(int R, int G, int B) : this(255, R, G, B)
        { 
        }

        /// <summary>
        /// Creates a new instance of the ARGB struct from the given RGB values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        /// <param name="A">a value from 0 - 255</param>
        public ARGB(int A, int R, int G, int B)
        {
            this.a = (byte)A;
            this.r = (byte)R;
            this.g = (byte)G;
            this.b = (byte)B;
        }

        /// <summary>
        /// Creates a new instance of the ARGB struct from the given System.Drawing.Color.
        /// </summary>
        /// <param name="argb"></param>
        public ARGB(Color argb) : this(argb.A, argb.R, argb.G, argb.B)
        {
        }

        public static implicit operator ARGB(Color color)
        {
            return new ARGB(color);
        }

        public static implicit operator Color(ARGB color)
        {
            return color.ToColor();
        }
        public static implicit operator HSB(ARGB color)
        {
            return color.ToHSB();
        }

        public static implicit operator HSL(ARGB color)
        {
            return color.ToHSL();
        }

        public static implicit operator CMYK(ARGB color)
        {
            return color.ToCMYK();
        }

        public static bool operator ==(ARGB left, ARGB right)
        {
            return  (left.R == right.R) && 
                    (left.G == right.G) && 
                    (left.B == right.B) && 
                    (left.A == right.A);
        }

        public static bool operator !=(ARGB left, ARGB right)
        {
            return !(left == right);
        }


        /// <summary>
        /// Convert the current color to a System.Drawing.Color.
        /// </summary>
        /// <returns>A System.Drawing.Color representation of this color.</returns>
        public Color ToColor()
        {
            return Color.FromArgb(A, R, G, B);
        }

        /// <summary>
        /// Convert the current color to a HSB color.
        /// </summary>
        /// <returns>An HSB representation of this color.</returns>
        public HSB ToHSB()
        {
            return new HSB(r, g, b, a);
        }

        /// <summary>
        /// Convert the current color to a HSL color.
        /// </summary>
        /// <returns>An HSL representation of this color.</returns>
        public HSL ToHSL()
        {
            return new HSL(r, g, b, a);
        }

        /// <summary>
        /// Convert the current color to a CMYK color.
        /// </summary>
        /// <returns>An CMYK representation of this color.</returns>
        public CMYK ToCMYK()
        {
            return new CMYK(r, g, b, a);
        }

        /// <summary>
        /// Inverts the current color.
        /// </summary>
        /// <returns>The inverted color.</returns>
        public ARGB GetInverted()
        {
            return new ARGB(a, 255 - r, 255 - b, 255 - b);
        }

        /// <summary>
        /// Gets the inverted ARGB color.
        /// </summary>
        /// <param name="input">The color to invert.</param>
        /// <returns>An inverted ARGB struct.</returns>
        public static ARGB GetInverted(ARGB input)
        {
            return new ARGB(input.A, 255 - input.R, 255 - input.G, 255 - input.B);
        }

        /// <summary>
        /// Creates a new instance of the ARGB struct from the given RGB values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// <para>Where A is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        /// <param name="A">a value from 0 - 255</param>
        /// <returns>A ARGB color.</returns>
        public static ARGB FromARGB(int A, int R, int G, int B)
        {
            return new ARGB(A, R, G, B);
        }

        /// <summary>
        /// Creates a new instance of the ARGB struct from the given RGB values.
        /// <para>Where R is a value from 0 - 255</para>
        /// <para>Where G is a value from 0 - 255</para>
        /// <para>Where B is a value from 0 - 255</para>
        /// </summary>
        /// <param name="R">a value from 0 - 255</param>
        /// <param name="G">a value from 0 - 255</param>
        /// <param name="B">a value from 0 - 255</param>
        /// <returns>A ARGB color.</returns>
        public static ARGB FromARGB(int R, int G, int B)
        {
            return new ARGB(R, G, B);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", A, R, G, B);
        }

        public string ToString(ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return string.Format("{0}, {1}, {2}", R, G, B);
                case ColorFormat.ARGB:
                    return string.Format("{0}, {1}, {2}, {3}", A, R, G, B);
            }
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
