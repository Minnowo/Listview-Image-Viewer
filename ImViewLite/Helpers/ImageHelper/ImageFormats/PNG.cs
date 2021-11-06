using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Helpers
{
    public class PNG : IMAGE
    {
        #region Readonly / Const / Static 


        /// <summary>
        /// The leading bytes to identify the png format.
        /// </summary>
        public static readonly byte[] IdentifierBytes_1 = new byte[8] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };



        /// <summary>
        /// The leading bytes to identify a png image.
        /// </summary>
        public static readonly new byte[][] FileIdentifiers = new byte[][]
        {
            IdentifierBytes_1
        };


        /// <summary>
        /// The file extensions used for a png image.
        /// </summary>
        public static readonly new string[] FileExtensions = new[]
        {
            "png"
        };


        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        public new const string MimeType = "image/png";


        /// <summary>
        /// Gets the default file extension.
        /// </summary>
        public new const string DefaultExtension = "png";


        /// <summary>
        /// Gets the png iamge format.
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


        public PNG()
        {
        }

        public PNG(Image bmp) : this((Bitmap)bmp)
        {
        }

        public PNG(Bitmap bmp)
        {
            this.Image = bmp;
        }


        #region Static Functions 

        /// <summary>
        /// Loads a PNG image and returns it as a bitmap object.
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
                throw new Exception(ex.Message + "\r\nIn PNG.FromFileAsBitmap(string)");
            }
        }

        /// <summary>
        /// Save an image using the png format.
        /// </summary>
        /// <param name="image">The image to save.</param>
        /// <param name="path">The path to save the image.</param>
        public static void Save(Image image, string path)
        {
            try
            {
                PathHelper.CreateDirectoryFromFilePath(path);
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn PNG.Save(Image, string)");
            }
        }

        #endregion


        public override void Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("PNG.Load(string)\n\tPath cannot be null or empty");

            base.LoadSafe(path);
        }


        public override void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("PNG.Save(string)\n\tPath cannot be null or empty");
            if (this.Image == null)
                throw new ArgumentException("PNG.Save(string)\n\tImage cannot be null");

            PNG.Save(this.Image, path);
        }


        public override ImgFormat GetImageFormat()
        {
            return PNG.ImageFormat;
        }

        public override string GetMimeType()
        {
            return PNG.MimeType;
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


        public static implicit operator Bitmap(PNG png)
        {
            return png.Image;
        }

        public static implicit operator PNG(Bitmap bitmap)
        {
            return new PNG(bitmap);
        }

        public static implicit operator Image(PNG png)
        {
            return png.Image;
        }

        public static implicit operator PNG(Image bitmap)
        {
            return new PNG(bitmap);
        }


    }
}
