using System.Drawing;


namespace ImViewLite.Helpers
{
    public struct COLOR
    {
        public static readonly COLOR Empty;

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
            get
            {
                return a;
            }
            set
            {
                a = value.Clamp<byte>(0, 255);
                ARGB.A = a;
                HSB.A = a;
                HSL.A = a;
                CMYK.A = a;
            }
        }
        private byte a;

        /// <summary>
        /// The ARGB format of this color.
        /// </summary>
        public ARGB ARGB;

        /// <summary>
        /// The HSB format of this color.
        /// </summary>
        public HSB HSB;

        /// <summary>
        /// The HSL format of this color.
        /// </summary>
        public HSL HSL;

        /// <summary>
        /// The CMYK format of this color.
        /// </summary>
        public CMYK CMYK;


        /// <summary>
        /// Creates a new instnace of the COLOR struct from the given System.Drawing.Color.
        /// </summary>
        /// <param name="color">A System.Drawing.Color color.</param>
        public COLOR(Color color)
        {
            ARGB = color;
            HSB = color;
            HSL = color;
            CMYK = color;
            a = color.A;
        }

        public COLOR(int A, int R, int G, int B) : this(Color.FromArgb(A, R, G, B))
        {
            this.A = (byte)A.Clamp(0, 255);
        }

        public COLOR(int R, int G, int B) : this(Color.FromArgb(R, G, B))
        {
            A = 255;
        }

        public static bool operator ==(COLOR left, COLOR right)
        {
            return  (left.ARGB.R == right.ARGB.R) && 
                    (left.ARGB.G == right.ARGB.G) && 
                    (left.ARGB.B == right.ARGB.B) && 
                    (left.A == right.A);
        }

        public static bool operator !=(COLOR left, COLOR right)
        {
            return !(left == right);
        }

        public static implicit operator COLOR(Color color)
        {
            return new COLOR(color);
        }

        public static implicit operator Color(COLOR color)
        {
            return Color.FromArgb(color.a, color.ARGB.R, color.ARGB.G, color.ARGB.B);
        }

        public static implicit operator ARGB(COLOR color)
        {
            return color.ARGB;
        }

        public static implicit operator HSB(COLOR color)
        {
            return color.HSB;
        }

        public static implicit operator HSL(COLOR color)
        {
            return color.HSL;
        }

        public static implicit operator CMYK(COLOR color)
        {
            return color.CMYK;
        }

        public string ToHex(ColorFormat format = ColorFormat.RGB)
        {
            return ColorHelper.ColorToHex(ARGB.R, ARGB.G, ARGB.B, ARGB.A, format);
        }

        public int ToDecimal(ColorFormat format = ColorFormat.RGB)
        {
            return ColorHelper.ColorToDecimal(ARGB.R, ARGB.G, ARGB.B, ARGB.A, format);
        }


        /// <summary>
        /// Updates the ARGB, HSL and CMYK color to represent the HSB current color.
        /// <para>This SHOULD be called after changing any values of the HSB color</para>
        /// </summary>
        public void UpdateHSB()
        {
            this.ARGB = HSB;
            this.HSL = HSB;
            this.CMYK = HSB;
            this.a = HSB.A;
        }

        /// <summary>
        /// Updates the ARGB, HSB and CMYK color to represent the HSL current color.
        /// <para>This SHOULD be called after changing any values of the HSL color</para>
        /// </summary>
        public void UpdateHSL()
        {
            this.ARGB = HSL;
            this.HSB = HSL;
            this.CMYK = HSL;
            this.a = HSL.A;
        }

        /// <summary>
        /// Updates the ARGB, HSL and HSB color to represent the CMYK current color.
        /// <para>This SHOULD be called after changing any values of the CMYK color</para>
        /// </summary>
        public void UpdateCMYK()
        {
            this.ARGB = CMYK;
            this.HSL = CMYK;
            this.HSB = CMYK;
            this.a = CMYK.A;
        }

        /// <summary>
        /// Updates the HSB, HSL and CMYK color to represent the ARGB current color.
        /// <para>This SHOULD be called after changing any values of the ARGB color</para>
        /// </summary>
        public void UpdateARGB()
        {
            this.HSB = ARGB;
            this.CMYK = ARGB;
            this.HSB = ARGB;
            this.a = ARGB.A;
        }

        public COLOR GetInverted()
        {
            return new COLOR(ARGB.GetInverted());
        }

        public static COLOR FromARGB(int A, int R, int G, int B)
        {
            return new COLOR(A, R, G, B);
        }

        public static COLOR FromARGB(int R, int G, int B)
        {
            return new COLOR(R, G, B);
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
