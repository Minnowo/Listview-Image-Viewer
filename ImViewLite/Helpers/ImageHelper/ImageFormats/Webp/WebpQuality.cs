using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Helpers
{
    
    public struct WebPQuality
    {
        public static readonly WebPQuality Empty;
        public static readonly WebPQuality Default = new WebPQuality(WebpEncodingFormat.EncodeLossy, 74, 6);

        /// <summary>
        /// The encoding format of the webp.
        /// </summary>
        [Description("The encoding format."), DisplayName("Encoding Format")]
        public WebpEncodingFormat Format { get; set; }

        /// <summary>
        /// Between 0 (lower quality, lowest file size) and 100 (highest quality, higher file size)
        /// </summary>
        [Description("The quality level."), DisplayName("Quality 0 - 100")]
        public int Quality
        {
            get
            {
                return quality;
            }
            set
            {
                quality = value.Clamp(0, 100);
            }
        }
        private int quality;

        /// <summary>
        /// Between 0 (fastest, lowest compression) and 9 (slower, best compression)
        /// </summary>
        [Description("The speed."), DisplayName("Speed 0 - 9 (fast - slow)")]
        public int Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value.Clamp(0, 9);
            }
        }
        private int speed;

        public WebPQuality(WebpEncodingFormat fmt, int quality, int speed) : this()
        {
            Format = fmt;
            Speed = speed;
            Quality = quality;
        }

        public static bool operator ==(WebPQuality left, WebPQuality right)
        {
            return (left.Format == right.Format) && (left.Speed == right.Speed) && (left.Quality == right.Quality);
        }

        public static bool operator !=(WebPQuality left, WebPQuality right)
        {
            return !(left == right);
        }

        public int ToDecimal()
        {
            return (int)Format << 16 | quality << 8 | Speed;
        }

        public static WebPQuality FromDecimal(int dec)
        {
            return new WebPQuality((WebpEncodingFormat)((dec >> 16) & 0xFF).Clamp(0, 2), (dec >> 8) & 0xFF, dec & 0xFF);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", Format, quality, speed);
        }
    }
}
