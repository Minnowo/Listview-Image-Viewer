using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.IO.Compression;

namespace ImViewLite.Helpers
{
    public enum WormFormat
    {
        wrm,
        dwrm,
        nil = -1
    }

    /// <summary>
    /// Provides the necessary information to support WORM images.
    /// </summary>
    public class WORM : IMAGE
    {
        #region Readonly / Const / Static 

        /// <summary>
        /// The maximum width and height of a WORM iamge.
        /// </summary>
        public const int MAX_SIZE = 65535; 


        /// <summary>
        /// The leading bytes to identify the wrm format.
        /// </summary>
        public static readonly byte[] IdentifierBytes_1 = new byte[5] { 0x57, 0x4F, 0x52, 0x4D, 0x2E };


        /// <summary>
        /// The leading bytes to identify the dwrm format.
        /// </summary>
        public static readonly byte[] IdentifierBytes_2 = new byte[5] { 0x44, 0x57, 0x4F, 0x52, 0x4D };


        /// <summary>
        /// The leading bytes to identify a WORM image.
        /// </summary>
        public static readonly new byte[][] FileIdentifiers = new byte[][]
        {
            IdentifierBytes_1,
            IdentifierBytes_2
        };


        /// <summary>
        /// The file extensions used for a WORM image.
        /// </summary>
        public static readonly new string[] FileExtensions = new[]
        {
            "wrm", "dwrm"
        };


        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        public new const string MimeType = "image/worm";


        /// <summary>
        /// Gets the default file extension.
        /// </summary>
        public new const string DefaultExtension = "wrm";


        /// <summary>
        /// Gets the WORM iamge format.
        /// </summary>
        public static readonly new ImgFormat ImageFormat = ImgFormat.wrm;

        #endregion


        public override Bitmap Image { get; protected set; }

        public override int Width
        {
            get
            {
                if (this.Image == null)
                    return 0;
                return this.Image.Width;
            }
            protected set
            {
            }
        }

        public override int Height
        {
            get
            {
                if (this.Image == null)
                    return 0;
                return this.Image.Height;
            }
            protected set
            {
            }
        }

        public override Size Size
        {
            get
            {
                if (this.Image == null)
                    return Size.Empty;
                return this.Image.Size;
            }
            protected set
            {
            }
        }

        /// <summary>
        /// Get the <see cref="WormFormat"/> of the image.
        /// </summary>
        public WormFormat Format { get; private set; }


        private const double MAX_DEC = 16777215d;
        private const double MAX_DEC_TO_USHORT = 256.00389d;
        private const byte HUE_OFFSET = 60;


        public WORM()
        {
        }

        public WORM(Image bmp) : this((Bitmap)bmp)
        {
        }

        public WORM(Bitmap bmp)
        {
            if (bmp.Width > MAX_SIZE || bmp.Height > MAX_SIZE)
                throw new Exception($"WORM images do not support width or height larger than {MAX_SIZE}");

            this.Image = bmp;
            this.Format = WormFormat.nil;
        }


        #region Static Functions

        /// <summary>
        /// Load a wrm image.
        /// </summary>
        /// <param name="file">The path of the image.</param>
        /// <returns>A new instance of the WORM class.</returns>
        public static WORM FromFile(string file)
        {
            try
            {
                WORM wrm = new WORM();
                wrm.Load(file);
                return wrm;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn WORM.FromFile(string)");
            }
        }


        /// <summary>
        /// Loads a WORM image and returns it as a bitmap object.
        /// </summary>
        /// <param name="file">The path of the image.</param>
        /// <returns>A <see cref="Bitmap"/> object.</returns>
        public static Bitmap FromFileAsBitmap(string file)
        {
            try
            {
                return WORM.FromFile(file).Image;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn WORM.FromFileAsBitmap(string)");
            }
        }


        /// <summary>
        /// Gets the dimensions of a WORM image from a file.
        /// </summary>
        /// <param name="path">The path to the image.</param>
        /// <returns>The width and height of the image.</returns>
        public static Size GetDimensionsFromFile(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return Size.Empty;

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                return GetDimensions(binaryReader);
            }
        }


        /// <summary>
        /// Gets the dimensions of a WORM image from a binary reader.
        /// <para>If the Identifier has already been read call this with False as the second argument.</para>
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="checkIdentifier">Should the WRM_IDENTIFIER or DWRM_IDENTIFIER be read from the stream.</param>
        /// <returns>The width and height of the image.</returns>
        public static Size GetDimensions(BinaryReader binaryReader, bool checkIdentifier = true)
        {
            try
            {
                if (checkIdentifier)
                {
                    WormFormat format = GetWormFormat(binaryReader);

                    if (format == WormFormat.nil)
                        return Size.Empty;
                }

                int Width = binaryReader.ReadUInt16();
                int Height = binaryReader.ReadUInt16();
                return new Size(Width, Height);
            }
            catch 
            { 
            }
            return Size.Empty;
        }

        
        /// <summary>
        /// Reads the first 5 bytes from the reader to identify a WORM image.
        /// </summary>
        /// <param name="binaryReader">The reader.</param>
        /// <returns>The format of a worm image. nil for invalid</returns>
        public static WormFormat GetWormFormat(BinaryReader binaryReader)
        {
            byte[] identifier = binaryReader.ReadBytes(5);

            if (ByteHelper.StartsWith(identifier, IdentifierBytes_1))
                return WormFormat.wrm;
            if (ByteHelper.StartsWith(identifier, IdentifierBytes_2))
                return WormFormat.dwrm;

            return WormFormat.nil;
        }


        public static void Save(Image image, string path)
        {
            try
            {
                WORM wrm = new WORM(image);
                wrm.Save(path);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message + "\r\tIn WORM.Save(Image, string)");
            }
        }

        #endregion



        public override unsafe void Load(string file)
        {
            if(string.IsNullOrEmpty(file))
                throw new ArgumentException("WORM.Load(string)\n\tThe path cannot be null or empty");

            Clear();

            using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            using(GZipStream decompressor = new GZipStream(fileStream, CompressionMode.Decompress))
            {
                Format = GetWormFormat(binaryReader);
                
                if(Format == WormFormat.nil)
                    throw new Exception("WORM.Load(string)\n\tInvalid WORM file cannot read");

                int width = binaryReader.ReadUInt16();
                int height = binaryReader.ReadUInt16();

                Image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                Image.Tag = ImgFormat.wrm;

                BitmapData dstBD = null;
                byte[] decompressedData = new byte[width * height * 2];
                decompressor.Read(decompressedData, 0, decompressedData.Length);

                dstBD = Image.LockBits(
                        new Rectangle(0, 0, Image.Width, Image.Height),
                        ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                try
                {
                    byte* pDst = (byte*)(void*)dstBD.Scan0;

                    for (int i = 0; i < width * height; i += 1)
                    {
                        int index = i << 1;
                        int dec = (ushort)((decompressedData[index]) | (decompressedData[index + 1] << 8));
                        double percent = (double)dec / (double)ushort.MaxValue;
                        int COLOR = (int)Math.Round(percent * MAX_DEC);
                        Color c = ReadWORMPixel(COLOR);

                        *(pDst++) = c.B;
                        *(pDst++) = c.G;
                        *(pDst++) = c.R;
                        pDst++; // skip alpha					 
                    }
                }
                finally
                {
                    Image.UnlockBits(dstBD);
                }
            }
        }



        public override unsafe void Save(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentException("WORM.Save(string)\n\tThe path cannot be null or empty");

            switch (Path.GetExtension(file))
            {
                case ".wrm":
                    this.Format = WormFormat.wrm;
                    break;
                case ".dwrm":
                    this.Format = WormFormat.dwrm;
                    break;
            }

            if (this.Format != WormFormat.nil) 
            {
                Save(file, this.Format); 
            }
            else
            {
                Save(file, WormFormat.wrm);
            }
        }

        /// <summary>
        /// Saves an image to disk.
        /// <para>Can throw just about any exception and should be used in a try catch</para>
        /// </summary>
        /// <param name="file">The path to save.</param>
        /// <param name="format">The <see cref="WormFormat"/> to encode.</param>
        /// <exception cref="Exception"></exception>
        public virtual unsafe void Save(string file, WormFormat format)
        {
            if(string.IsNullOrEmpty(file))
                throw new ArgumentException("WORM.Save(string, WormFormat)\n\tThe path cannot be null or empty");
            if (this.Image == null)
                throw new ArgumentException("WORM.Save(string, WormFormat)\n\tThe image cannot be null");

            using (Stream stream = new FileStream(file, FileMode.OpenOrCreate))
            using (BinaryWriter binaryWriter = new BinaryWriter(stream))
            using(GZipStream compressor  = new GZipStream(stream, CompressionLevel.Optimal))
            {
                switch (format)
                {
                    case WormFormat.nil:
                        throw new Exception("WORM.Save(string, WormFormat)\n\tInvalid worm format.");
                    case WormFormat.wrm:
                        binaryWriter.Write(IdentifierBytes_1, 0, IdentifierBytes_1.Length); 
                        break;
                    case WormFormat.dwrm:
                        binaryWriter.Write(IdentifierBytes_2, 0, IdentifierBytes_2.Length);
                        break;
                }
                
                binaryWriter.Write((ushort)this.Image.Width);
                binaryWriter.Write((ushort)this.Image.Height);

                BitmapData dstBD = null;
                byte[] data = new byte[this.Image.Width * this.Image.Height* 2];

                dstBD = Image.LockBits(
                        new Rectangle(0, 0, this.Image.Width, this.Image.Height),
                        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                try
                {
                    byte* pDst = (byte*)(void*)dstBD.Scan0;

                    for (int i = 0; i < this.Image.Width * this.Image.Height; i++)
                    {
                        int Decimal = ColorToDecimal(*(pDst + 2), *(pDst + 1), *(pDst));
                        ushort StorageValue = (ushort)Math.Round(ushort.MaxValue * (Decimal / MAX_DEC));

                        int index = i << 1;

                        data[index] = (byte)StorageValue;
                        data[index + 1] = (byte)(StorageValue >> 8);

                        pDst += 4; // advance				 
                    }
                    compressor.Write(data, 0, data.Length);
                }
                finally
                {
                    Image.UnlockBits(dstBD);
                }
            }
        }


        public override ImgFormat GetImageFormat()
        {
            return WORM.ImageFormat;
        }

        public override string GetMimeType()
        {
            return WORM.MimeType;
        }

        public override void Clear()
        {
            if (Image != null)
                Image.Dispose();
            Format = WormFormat.nil;
            Image = null;
        }

        /// <summary>
        /// Dispose of the image.
        /// </summary>
        public new void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }


        public static implicit operator Bitmap(WORM worm)
        {
            return worm.Image;
        }

        public static implicit operator WORM(Bitmap bitmap)
        {
            return new WORM(bitmap);
        }

        public static implicit operator Image(WORM worm)
        {
            return worm.Image;
        }

        public static implicit operator WORM(Image bitmap)
        {
            return new WORM(bitmap);
        }


        private static int ColorToDecimal(int r, int g, int b, int a = 255)
        {
            return r << 16 | g << 8 | b;
        }

        private static Color DecimalToColor(int dec)
        {
            return Color.FromArgb((dec >> 16) & 0xFF, (dec >> 8) & 0xFF, dec & 0xFF);
        }

        private Color ReadWORMPixel(int dec)
        {
            Color c = DecimalToColor(dec);

            float Hue;
            float Sat;
            float Lig;

            switch (Format)
            {
                default:
                case WormFormat.wrm:
                    float newR = c.R / 255f;
                    float newG = c.G / 255f;
                    float newB = c.B / 255f;
                    float min = new List<float> { newR, newB, newG }.Min();

                    if (newR >= newB && newR >= newG) // newR > than both
                    {
                        if ((newR - min) != 0) // cannot divide by 0 
                        {
                            // divide by 6 because if you don't hue * 60 = 0-360, but we want hue * 360 = 0-360
                            Hue = (((newG - newB) / (newR - min)) % 6) / 6;
                            if (Hue < 0) // if its negative add 360. 360/360 = 1
                                Hue += 1;
                        }
                        else
                            Hue = 0;

                        if (newR == 0)
                            Sat = 0f;
                        else
                            Sat = (newR - min) / newR;
                        Lig = newR;
                    }
                    else if (newB > newG) // newB > both
                    {
                        // don't have to worry about dividing by 0 because if max == min the if statement above is true
                        Hue = (4.0f + (newR - newG) / (newB - min)) / 6;
                        if (newB == 0)
                            Sat = 0f;
                        else
                            Sat = (newB - min) / newB;
                        Lig = newB;
                    }
                    else // newG > both
                    {
                        Hue = (2.0f + (newB - newR) / (newG - min)) / 6;
                        if (newG == 0)
                            Sat = 0f;
                        else
                            Sat = (newG - min) / newG;
                        Lig = newG;
                    }

                    Hue = Math.Abs(360 - (Hue * 360 + HUE_OFFSET));
                    Sat = (Sat + 0.05f);
                    if (Sat > 1)
                        Sat = Math.Abs(1f - Sat);
                    return HSVToColor(Hue, Sat, Lig);

                case WormFormat.dwrm:
                    Hue = c.GetHue();
                    Sat = c.GetSaturation();
                    Lig = c.GetBrightness();

                    Hue = Math.Abs(360f - (Hue + HUE_OFFSET));

                    return HSLToColor(Hue, Sat, Lig);
            }

        }

        private static Color HSVToColor(float Hue360, float saturation, float brightness)
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

            return Color.FromArgb(
                (int)Math.Round((r + m) * 255),
                (int)Math.Round((g + m) * 255),
                (int)Math.Round((b + m) * 255));
        }

        private static Color HSLToColor(float Hue360, float lightness, float saturation)
        {
            double c, x, m, r = 0, g = 0, b = 0;
            c = (1.0 - Math.Abs(2 * lightness - 1.0d)) * saturation;
            x = c * (1.0d - Math.Abs(Hue360 / 60 % 2 - 1.0d));
            m = lightness - c / 2.0d;

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

            return Color.FromArgb(
                (int)Math.Round((r + m) * 255),
                (int)Math.Round((g + m) * 255),
                (int)Math.Round((b + m) * 255));

        }
    }
}
