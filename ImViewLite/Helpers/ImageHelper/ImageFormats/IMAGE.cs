using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;

namespace ImViewLite.Helpers
{

    public interface IImage : IDisposable
    {
        /// <summary>
        /// The image / images.
        /// </summary>
        Bitmap Image { get; }

        /// <summary>
        /// Loads an image into memory.
        /// <para>Can throw just about any exception and should be used in a try catch</para>
        /// </summary>
        /// <param name="path">The path of the image to load.</param>
        /// <exception cref="Exception"></exception>
        void Load(string path);

        /// <summary>
        /// Saves an image to disk.
        /// <para>Can throw just about any exception and should be used in a try catch</para>
        /// </summary>
        /// <param name="path">The path to save.</param>
        /// <exception cref="Exception"></exception>
        void Save(string path);

        /// <summary>
        /// Gets the image format of the derived class.
        /// </summary>
        /// <returns>The <see cref="ImgFormat"/> of the image.</returns>
        ImgFormat GetImageFormat();

        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        /// <returns>The MimeType of the image.</returns>
        string GetMimeType();

        /// <summary>
        /// Rotate the image 90 degrees to the left.
        /// </summary>
        void RotateLeft90();

        /// <summary>
        /// Rotate the image 90 degrees to the right.
        /// </summary>
        void RotateRight90();

        /// <summary>
        /// Flip the image horizontally.
        /// </summary>
        void FlipHorizontal();

        /// <summary>
        /// Flip the image vertically.
        /// </summary>
        void FlipVertical();

        /// <summary>
        /// Inverts the image colors.
        /// </summary>
        void InvertColor();

        /// <summary>
        /// Covnerts the image to gray.
        /// </summary>
        void ConvertGrayscale();

        /// <summary>
        /// Reiszes the image using the given interpolation mode.
        /// </summary>
        /// <param name="newSize">The new <see cref="Size"/>.</param>
        /// <param name="interp">The <see cref="InterpolationMode"/>.</param>
        void Reisze(Size newSize, InterpolationMode interp);

        /// <summary>
        /// Resizes the image using the given graphics unit.
        /// </summary>
        /// <param name="newSize">The new <see cref="Size"/>.</param>
        /// <param name="units">The <see cref="GraphicsUnit"/>.</param>
        void Reisze(Size newSize, GraphicsUnit units);

        /// <summary>
        /// Resizes the image using the given interpolation mode and graphics unit.
        /// </summary>
        /// <param name="newSize">The new <see cref="Size"/>.</param>
        /// <param name="interp">The <see cref="InterpolationMode"/>.</param>
        /// <param name="units">The <see cref="GraphicsUnit"/>.</param>
        void Reisze(Size newSize, InterpolationMode interp, GraphicsUnit units);
    }


    /// <summary>
    /// The supported format base. Implement this class when building a supported format.
    /// </summary>
    public abstract class IMAGE : IImage
    {
        #region Readonly / Const / Static 

        /// <summary>
        /// The leading bytes to identify a WORM image.
        /// </summary>
        public static readonly byte[][] FileIdentifiers;

        /// <summary>
        /// The file extensions used for a WORM image.
        /// </summary>
        public static readonly string[] FileExtensions;


        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        public const string MimeType = "image/unknown";


        /// <summary>
        /// Gets the default file extension.
        /// </summary>
        public const string DefaultExtension = "";


        /// <summary>
        /// Gets the WORM iamge format.
        /// </summary>
        public static ImgFormat ImageFormat { get; } = ImgFormat.nil;

        #endregion

        /// <summary>
        /// Gets the image.
        /// </summary>
        public abstract Bitmap Image { get; protected set; }

        /// <summary>
        /// Gets the width of the image.
        /// </summary>
        public abstract int Width { get; protected set; }

        /// <summary>
        /// Gets the height of the image.
        /// </summary>
        public abstract int Height { get; protected set; }

        /// <summary>
        /// Gets the <see cref="Size"/> of the image.
        /// </summary>
        public abstract Size Size { get; protected set; }

        #region Static Functions 


        /// <summary>
        /// Loads an image using the standard method.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A <see cref="Bitmap"/> object.</returns>
        public static Bitmap StandardLoad(string path)
        {
            Bitmap bmp = (Bitmap)System.Drawing.Image.FromStream(new MemoryStream(File.ReadAllBytes(path)));
            ImageHelper.RotateImageByExifOrientationData(bmp);
            return bmp;
        }


        /// <summary>
        /// Casts an image to its given image type and returns its base class.
        /// </summary>
        /// <param name="image">The image to cast.</param>
        /// <param name="format">The format to cast.</param>
        /// <returns>The image as <see cref="IMAGE"/> of its proper format.</returns>
        public static IMAGE ProperCast(Image image, ImgFormat format)
        {
            switch (format)
            {
                case ImgFormat.png:
                    return new PNG(image);

                case ImgFormat.bmp:
                    return new BMP(image);

                case ImgFormat.gif:
                    return new Gif(image);

                case ImgFormat.jpg:
                    return new JPEG(image);

                case ImgFormat.tif:
                    return new TIFF(image);

                case ImgFormat.webp:
                    return new Webp(image);

                case ImgFormat.wrm:
                    return new WORM(image);

                case ImgFormat.ico:
                    return new ICO(image);
            }
            return null;
        }
        #endregion

        public virtual void Load(string path)
        {
            if (this.Image != null)
                this.Image.Dispose();
            this.Image = ImageHelper.LoadImageAsBitmap(path);
        }



        public virtual void Save(string path)
        {
            ImageHelper.SaveImage(this.Image, path, false);
        }

        /// <summary>
        /// Makes a copy of the image and returns a <see cref="Bitmap"/>.
        /// </summary>
        /// <returns>A <see cref="Bitmap"/> copy of the image.</returns>
        public virtual Bitmap DeepClone()
        {
            if (this.Image == null)
                return null;

            Bitmap copy = ImageProcessor.DeepClone(this.Image, this.Image.PixelFormat, true);
            /*if (ImageHelper.IsIndexed(targetFormat))
            {
                Bitmap quantized = this.Quantizer.Quantize(copy);
                copy.Dispose();
                copy = quantized;
            }*/

            return copy;
        }

        /// <summary>
        /// Uses the built in Clone to copy the image.
        /// </summary>
        /// <returns>A <see cref="Bitmap"/> copy of the image.</returns>
        public virtual Bitmap CloneSafe()
        {
            if (this.Image == null)
                return null;
            return this.Image.CloneSafe();
        }

        public virtual ImgFormat GetImageFormat()
        {
            return IMAGE.ImageFormat;
        }

        public virtual string GetMimeType()
        {
            return IMAGE.MimeType;
        }

        public virtual void RotateRight90()
        {
            if (this.Image == null)
                return;
            this.Image.RotateFlip(RotateFlipType.Rotate90FlipNone); 
        }

        public virtual void RotateLeft90()
        {
            if (this.Image == null)
                return;
            this.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
        }

        public virtual void FlipHorizontal()
        {
            if (this.Image == null)
                return;
            this.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
        }

        public virtual void FlipVertical()
        {
            if (this.Image == null)
                return;
            this.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        public virtual void InvertColor()
        {
            if (this.Image == null)
                return;
            ImageProcessor.InvertBitmapSafe(this.Image);
        }

        public virtual void ConvertGrayscale()
        {
            if (this.Image == null)
                return;
            ImageProcessor.GrayscaleBitmapSafe(this.Image);
        }

        public virtual void Reisze(Size newSize, InterpolationMode interp)
        {
            if (this.Image == null)
                return;

            UpdateImage(ImageProcessor.ResizeImage(this.Image, newSize, interp, GraphicsUnit.Pixel));
        }

        public virtual void Reisze(Size newSize, GraphicsUnit units)
        {
            if (this.Image == null)
                return;

            if ((newSize.Width + newSize.Height) >> 1 > (this.Image.Width + this.Image.Height) >> 1)
            {
                UpdateImage(ImageProcessor.ResizeImage(this.Image, newSize, InterpolationMode.NearestNeighbor, units));
            }
            else
            {
                UpdateImage(ImageProcessor.ResizeImage(this.Image, newSize, InterpolationMode.HighQualityBicubic, units));
            }
        }

        public virtual void Reisze(Size newSize, InterpolationMode interp, GraphicsUnit units)
        {
            if (this.Image == null)
                return;

            UpdateImage(ImageProcessor.ResizeImage(this.Image, newSize, interp, units));
        }

        /// <summary>
        /// Loads an iamge using the standard method.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        protected void LoadSafe(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            Clear();
            this.Image = IMAGE.StandardLoad(path);
        }

        /// <summary>
        /// Dispose of the previous image and replace it with the given image.
        /// </summary>
        /// <param name="image"></param>
        public virtual void UpdateImage(Image image)
        {
            this.Clear();
            this.Image = (Bitmap)image;
        }


        /// <summary>
        /// Dispose of the image.
        /// </summary>
        public virtual void Clear()
        {
            if (this.Image == null)
                return;

            this.Image.Dispose();
            this.Image = null;
        }

        /// <summary>
        /// Dispose of the image.
        /// </summary>
        public void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }

        public static implicit operator Image(IMAGE bas)
        {
            if (bas == null)
                return null;
            return bas.Image;
        }

        public static implicit operator Bitmap(IMAGE bas)
        {
            if (bas == null)
                return null;
            return bas.Image;
        }


        public override bool Equals(object obj) => obj is IImage format && this.Equals(format);



        public override int GetHashCode()
        {
            int hashCode = -116763541;
            hashCode = (hashCode * -1521134295) + EqualityComparer<int>.Default.GetHashCode(this.Height);
            hashCode = (hashCode * -1521134295) + EqualityComparer<int>.Default.GetHashCode(this.Width);
            hashCode = (hashCode * -1521134295) + EqualityComparer<Bitmap>.Default.GetHashCode(this.Image);
            hashCode = (hashCode * -1521134295) + EqualityComparer<ImgFormat>.Default.GetHashCode(this.GetImageFormat());
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.GetMimeType());
            return hashCode;
        }
    }
}
