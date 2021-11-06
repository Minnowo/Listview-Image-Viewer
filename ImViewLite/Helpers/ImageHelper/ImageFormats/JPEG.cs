using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImViewLite.Helpers
{
    public class JPEG : IMAGE
    {
        #region Readonly / Const / Static 


        /// <summary>
        /// The leading bytes to identify the jpeg format.
        /// </summary>
        public static readonly byte[] IdentifierBytes_1 = new byte[3] { 0xFF, 0xD8, 0xFF };


        /// <summary>
        /// The leading bytes to identify a jpeg image.
        /// </summary>
        public static readonly new byte[][] FileIdentifiers = new byte[][]
        {
            IdentifierBytes_1
        };


        /// <summary>
        /// The file extensions used for a jpeg image.
        /// </summary>
        public static readonly new string[] FileExtensions = new[]
        {
            "jpg", "jpeg", "jpe", "jfif"
        };


        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        public new const string MimeType = "image/jpeg";


        /// <summary>
        /// Gets the default file extension.
        /// </summary>
        public new const string DefaultExtension = "jpg";


        /// <summary>
        /// Gets the jpeg iamge format.
        /// </summary>
        public static readonly new ImgFormat ImageFormat = ImgFormat.jpg;

        private static ImageCodecInfo imageCodecInfo = Array.Find(
                                ImageCodecInfo.GetImageEncoders(),
                                ici => ici.MimeType.Equals(JPEG.MimeType, StringComparison.OrdinalIgnoreCase));

        public static int DefaultQuality = 75;
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
        /// Get or Set the quality to encode the jpeg.
        /// </summary>
        public virtual long Quality { get; set; }

        public JPEG()
        {
            this.Quality = JPEG.DefaultQuality;
        }

        public JPEG(Image bmp) : this((Bitmap)bmp)
        {
        }

        public JPEG(Bitmap bmp)
        {
            this.Image = bmp;
            this.Quality = 75;
        }


        #region Static Functions 

        /// <summary>
        /// Loads a Jpeg image and returns it as a bitmap object.
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
                throw new Exception(ex.Message + "\r\nIn JPEG.FromFileAsBitmap(string)");
            }
        }

        /// <summary>
        /// Save an image using the bitmap format.
        /// </summary>
        /// <param name="image">The image to save.</param>
        /// <param name="path">The path to save the image.</param>
        public static void Save(Image image, string path, long quality = 75L)
        {
            try
            {
                PathHelper.CreateDirectoryFromFilePath(path);
                JPEG t = new JPEG(image);
                t.Quality = quality;
                t.Save(path);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn JPEG.Save(Image, string)");
            }
        }

        #endregion


        public override void Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("JPEG.Load(string)\n\tPath cannot be null or empty");

            base.LoadSafe(path);
        }

        public override void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("JPEG.Save(string)\n\tPath cannot be null or empty");
            if (this.Image == null)
                throw new ArgumentException("JPEG.Save(string)\n\tImage cannot be null");

            this.Save(path, this.Quality);
        }

        /// <summary>
        /// Saves an image to disk.
        /// <para>Can throw just about any exception and should be used in a try catch</para>
        /// </summary>
        /// <param name="path">The path to save.</param>
        /// <param name="quality">The quality of the jpeg.</param>
        /// <exception cref="Exception"></exception>
        public void Save(string path, long quality)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("JPEG.Save(string)\n\tPath cannot be null or empty");
            if (this.Image == null)
                throw new ArgumentException("JPEG.Save(string)\n\tImage cannot be null");

            this.Quality = quality;
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                this.Save(stream, this.Image, quality);
            }
        }

        /// <summary>
        /// Saves an image to the given stream.
        /// <para>Can throw just about any exception and should be used in a try catch</para>
        /// </summary>
        /// <param name="stream">The stream to save.</param>
        /// <param name="image">The image to save.</param>
        /// <param name="quality">The quality of the jpeg.</param>
        /// <exception cref="Exception"></exception>
        public void Save(Stream stream, Image image, long quality)
        {
            // Jpegs can be saved with different settings to include a quality setting for the JPEG compression.
            // This improves output compression and quality.
            using (EncoderParameters encoderParameters = GetEncoderParameters(quality))
            {
                image.Save(stream, JPEG.imageCodecInfo, encoderParameters);
            }
        }

        public override ImgFormat GetImageFormat()
        {
            return JPEG.ImageFormat;
        }

        public override string GetMimeType()
        {
            return JPEG.MimeType;
        }

        public override void Clear()
        {
            if (Image != null)
                Image.Dispose();

            Image = null;
            Quality = 75;
        }

        /// <summary>
        /// Dispose of the image.
        /// </summary>
        public new void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }


        public static implicit operator Bitmap(JPEG jpeg)
        {
            return jpeg.Image;
        }

        public static implicit operator JPEG(Bitmap bitmap)
        {
            return new JPEG(bitmap);
        }

        public static implicit operator Image(JPEG jpeg)
        {
            return jpeg.Image;
        }

        public static implicit operator JPEG(Image bitmap)
        {
            return new JPEG(bitmap);
        }

        private static EncoderParameters GetEncoderParameters(long quality)
        {
            return new EncoderParameters(1)
            {
                // Set the quality.
                Param = { [0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality) }
            };
        }
    }
}
