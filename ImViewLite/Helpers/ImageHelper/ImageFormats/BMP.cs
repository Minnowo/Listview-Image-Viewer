using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Helpers
{
    public class BMP : IMAGE
    {
        #region Readonly / Const / Static 


        /// <summary>
        /// The leading bytes to identify the bmp format.
        /// </summary>
        public static readonly byte[] IdentifierBytes_1 = new byte[2] { 0x42, 0x4D };



        /// <summary>
        /// The leading bytes to identify a bitmap image.
        /// </summary>
        public static readonly new byte[][] FileIdentifiers = new byte[][]
        {
            IdentifierBytes_1
        };


        /// <summary>
        /// The file extensions used for a bitmap image.
        /// </summary>
        public static readonly new string[] FileExtensions = new[]
        {
            "bmp"
        };


        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        public new const string MimeType = "image/bmp";


        /// <summary>
        /// Gets the default file extension.
        /// </summary>
        public new const string DefaultExtension = "bmp";


        /// <summary>
        /// Gets the bitmap iamge format.
        /// </summary>
        public static readonly new ImgFormat ImageFormat = ImgFormat.bmp;

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

        public BMP()
        {
        }

        public BMP(Image bmp) : this((Bitmap)bmp)
        {
        }

        public BMP(Bitmap bmp)
        {
            this.Image = bmp;
        }

        #region Static Functions 

        /// <summary>
        /// Loads a BMP image and returns it as a bitmap object.
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
                throw new Exception(ex.Message + "\r\nIn BMP.FromFileAsBitmap(string)");
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
                image.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn BMP.Save(Image, string)");
            }
        }

        #endregion


        public override void Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("BMP.Load(string)\n\tPath cannot be null or empty");

            base.LoadSafe(path);
        }


        public override void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("BMP.Save(string)\n\tPath cannot be null or empty");
            if (this.Image == null)
                throw new ArgumentException("BMP.Save(string)\n\tImage cannot be null");

            BMP.Save(this.Image, path);
        }


        public override ImgFormat GetImageFormat()
        {
            return BMP.ImageFormat;
        }

        public override string GetMimeType()
        {
            return BMP.MimeType;
        }

        public override void Clear()
        {
            if (Image != null)
                Image.Dispose();

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


        public static implicit operator Bitmap(BMP bmp)
        {
            return bmp.Image;
        }

        public static implicit operator BMP(Bitmap bitmap)
        {
            return new BMP(bitmap);
        }

        public static implicit operator Image(BMP bmp)
        {
            return bmp.Image;
        }

        public static implicit operator BMP(Image bitmap)
        {
            return new BMP(bitmap);
        }
    }
}
