using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Helpers
{
    public class TIFF : IMAGE
    {
        #region Readonly / Const / Static 


        /// <summary>
        /// The leading bytes to identify the tiff format. These bytes specify the tiff is using the little-endian format.
        /// </summary>
        public static readonly byte[] IdentifierBytes_1 = new byte[4] { 0x49, 0x49, 0x2A, 0x00 };


        /// <summary>
        /// The leading bytes to identify the tiff format. These bytes specify the tiff is using the big-endian format.
        /// </summary>
        public static readonly byte[] IdentifierBytes_2 = new byte[4] { 0x4D, 0x4D, 0x00, 0x2A };


        /// <summary>
        /// The leading bytes to identify a tiff image.
        /// </summary>
        public static readonly new byte[][] FileIdentifiers = new byte[][]
        {
            IdentifierBytes_1,
            IdentifierBytes_2
        };


        /// <summary>
        /// The file extensions used for a tiff image.
        /// </summary>
        public static readonly new string[] FileExtensions = new[]
        {
            "tif", "tiff"
        };


        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        public new const string MimeType = "image/tiff";


        /// <summary>
        /// Gets the default file extension.
        /// </summary>
        public new const string DefaultExtension = "tif";


        /// <summary>
        /// Gets the tiff iamge format.
        /// </summary>
        public static readonly new ImgFormat ImageFormat = ImgFormat.tif;

        private static ImageCodecInfo imageCodecInfo = Array.Find(
                                                    ImageCodecInfo.GetImageEncoders(),
                                                    ici => ici.MimeType.Equals(TIFF.MimeType, StringComparison.OrdinalIgnoreCase));

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
        /// Get or Set the tiff bit depth;
        /// </summary>
        public BitDepth BitDepth { get; set; }

        public TIFF()
        {
            this.BitDepth = BitDepth.Bit32;
        }

        public TIFF(Image bmp) : this((Bitmap)bmp)
        {
        }

        public TIFF(Bitmap bmp)
        {
            this.Image = bmp;
            this.BitDepth = BitDepth.Bit32;
        }

        #region Static Functions 

        /// <summary>
        /// Loads a TIFF image and returns it as a bitmap object.
        /// </summary>
        /// <param name="path">The path of the image.</param>
        /// <returns>A <see cref="Bitmap"/> object.</returns>
        public static Bitmap FromFileAsBitmap(string path)
        {
            try
            {
                return IMAGE.StandardLoad(path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn TIFF.FromFileAsBitmap(string)");
            }
        }

        /// <summary>
        /// Save an image using the bitmap format.
        /// </summary>
        /// <param name="image">The image to save.</param>
        /// <param name="path">The path to save the image.</param>
        public static void Save(Image image, string path)
        {
            try
            {
                PathHelper.CreateDirectoryFromFilePath(path);
                TIFF t = new TIFF(image);
                t.Save(path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn TIFF.Save(Image, string)");
            }
        }

        #endregion


        public override void Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("TIFF.Load(string)\n\tPath cannot be null or empty");

            base.LoadSafe(path);
        }


        public override void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("TIFF.Save(string)\n\tPath cannot be null or empty");
            if (this.Image == null)
                throw new ArgumentException("TIFF.Save(string)\n\tImage cannot be null");

            this.Save(path, this.BitDepth);
        }

        /// <summary>
        /// Saves an image to disk.
        /// <para>Can throw just about any exception and should be used in a try catch</para>
        /// </summary>
        /// <param name="path">The path to save.</param>
        /// <param name="bitDepth">The bit depth of the Tiff.</param>
        /// <exception cref="Exception"></exception>
        public void Save(string path, BitDepth bitDepth)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("TIFF.Save(string, BitDepth)\n\tPath cannot be null or empty");
            if (this.Image == null)
                throw new ArgumentException("TIFF.Save(string, BitDepth)\n\tImage cannot be null");

            this.BitDepth = BitDepth;
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                this.Save(stream, this.Image, bitDepth);
            }
        }

        /// <summary>
        /// Saves an image to the given stream.
        /// <para>Can throw just about any exception and should be used in a try catch</para>
        /// </summary>
        /// <param name="stream">The stream to save.</param>
        /// <param name="image">The image to save.</param>
        /// <param name="bitDepth">The bit depth of the Tiff.</param>
        /// <exception cref="Exception"></exception>
        public void Save(Stream stream, Image image, BitDepth bitDepth)
        {
            // Tiffs can be saved with different bit depths but throws if we use 16 bits.
            if (bitDepth == BitDepth.Bit16)
            {
                bitDepth = BitDepth.Bit24;
            }

            using (EncoderParameters encoderParameters = GetEncoderParameters(bitDepth))
            {
                switch (bitDepth)
                {
                    case BitDepth.Bit4:
                    case BitDepth.Bit8:
                        // Save as 8 bit quantized image.
                        //using (Bitmap quantized = this.Quantizer.Quantize(image))
                        //{
                        //.CopyMetadata(image, quantized);
                        //quantized.Save(stream, this.GetCodecInfo(), encoderParameters);
                        image.Save(stream, TIFF.imageCodecInfo, encoderParameters);
                        //}

                        return;

                    case BitDepth.Bit24:
                    case BitDepth.Bit32:

                        PixelFormat pixelFormat = ImageHelper.GetPixelFormatForBitDepth(bitDepth);

                        if (pixelFormat != image.PixelFormat)
                        {
                            using (Image copy = ImageProcessor.DeepClone(image, pixelFormat, true))
                            {
                                copy.Save(stream, TIFF.imageCodecInfo, encoderParameters);
                            }
                        }
                        else
                        {
                            image.Save(stream, TIFF.imageCodecInfo, encoderParameters);
                        }

                        break;
                    default:

                        // Encoding is handled by the encoding parameters.
                        image.Save(stream, TIFF.imageCodecInfo, encoderParameters);
                        break;
                }
            }
        }



        public override ImgFormat GetImageFormat()
        {
            return TIFF.ImageFormat;
        }

        public override string GetMimeType()
        {
            return TIFF.MimeType;
        }

        public override void Clear()
        {
            if (Image != null)
                Image.Dispose();

            Image = null;
            BitDepth = BitDepth.Bit32;
        }

        /// <summary>
        /// Dispose of the image.
        /// </summary>
        public new void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }


        public static implicit operator Bitmap(TIFF tiff)
        {
            return tiff.Image;
        }

        public static implicit operator TIFF(Bitmap bitmap)
        {
            return new TIFF(bitmap);
        }

        public static implicit operator Image(TIFF tiff)
        {
            return tiff.Image;
        }

        public static implicit operator TIFF(Image bitmap)
        {
            return new TIFF(bitmap);
        }


        private static EncoderParameters GetEncoderParameters(BitDepth bitDepth)
        {
            long colorDepth = (long)bitDepth;

            // CompressionCCITT4 provides 1 bit diffusion.
            long compression = (long)(bitDepth == BitDepth.Bit1
                ? EncoderValue.CompressionCCITT4
                : EncoderValue.CompressionLZW);

            EncoderParameters encoderParameters = new EncoderParameters(2);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, compression);
            encoderParameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, colorDepth);

            return encoderParameters;
        }
    }
}
