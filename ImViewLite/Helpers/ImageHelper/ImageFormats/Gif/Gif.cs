using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Helpers
{
    /// <summary>
    /// Provides the necessary information to support gif images.
    /// </summary>
    public class Gif : IMAGE
    {
        #region Readonly / Const / Static 


        /// <summary>
        /// The leading bytes to identify the wrm format.
        /// </summary>
        public static readonly byte[] IdentifierBytes_1 = new byte[6] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 };


        /// <summary>
        /// The leading bytes to identify the dwrm format.
        /// </summary>
        public static readonly byte[] IdentifierBytes_2 = new byte[6] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 };


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
            "gif"
        };


        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        public new const string MimeType = "image/gif";


        /// <summary>
        /// Gets the default file extension.
        /// </summary>
        public new const string DefaultExtension = "gif";


        /// <summary>
        /// Gets the WORM iamge format.
        /// </summary>
        public static readonly new ImgFormat ImageFormat = ImgFormat.gif;

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
            protected set { }
        }

        public virtual bool IsAnimating { get; protected set; }

        private List<EventHandler> FrameChangedHandlerCallbacks = new List<EventHandler>();


        public Gif()
        {
        }

        public Gif(Image bmp) : this((Bitmap)bmp)
        {
        }

        public Gif(Bitmap bmp)
        {
            this.Image = bmp;
        }



        #region Static Functions

        /// <summary>
        /// Loads a Gif image and returns it as a bitmap object.
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
                throw new Exception(ex.Message + "\r\nIn Gif.FromFileAsBitmap(string)");
            }
        }

        public static void Save(Image image, string path)
        {
            try
            {
                PathHelper.CreateDirectoryFromFilePath(path);
                Gif g = new Gif(image);
                g.Save(path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn Gif.Save(Image, string)");
            }
        }

        #endregion

        /// <summary>
        /// Get a bool indicating if the current gif can be animated.
        /// </summary>
        /// <returns>True if this gif can be animated; otherwise False;</returns>
        public bool CanAnimate()
        {
            if (this.Image == null)
                return false;
            return ImageAnimator.CanAnimate(this.Image);
        }

        /// <summary>
        /// Animate the current gif if possible.
        /// </summary>
        /// <param name="onFrameChangedHandler">The even handler for on frame changing.</param>
        /// <returns>True if the image animation has been started; otherwise False.</returns>
        public bool Animate(EventHandler onFrameChangedHandler)
        {
            if (this.Image == null)
                return false;

            this.IsAnimating = CanAnimate();
            if (this.IsAnimating)
            {
                FrameChangedHandlerCallbacks.Add(onFrameChangedHandler);
                ImageAnimator.Animate(this.Image, onFrameChangedHandler);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Stop animating the current gif if possible.
        /// </summary>
        /// <returns>True if the animation was stopped; otherwise False.</returns>
        public bool StopAnimate()
        {
            return StopAnimate(false);
        }

        public override void Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Gif.Load(string)\n\tPath cannot be null or empty");

            base.LoadSafe(path);
        }



        /// <summary>
        /// Saves an image to disk.
        /// <para>Can throw just about any exception and should be used in a try catch</para>
        /// </summary>
        /// <param name="path">The path to save.</param>
        /// <param name="encodeGif">Should the Gif be encoded before saving.</param>
        /// <exception cref="Exception"></exception>
        public void Save(string path, bool encodeGif)
        {
            if (encodeGif)
            {
                Save(path);
            }
            
            PathHelper.CreateDirectoryFromFilePath(path);
            this.Image.Save(path, System.Drawing.Imaging.ImageFormat.Gif);
        }

        public override void Save(string path)
        {
            if (this.Image == null)
                throw new System.ArgumentException("Gif.Save(string, bool)\n\tThe image cannot be null");
            if (string.IsNullOrEmpty(path))
                throw new System.ArgumentException("Gif.Save(string, bool)\n\tThe path cannot be null or empty");

            PathHelper.CreateDirectoryFromFilePath(path);

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                this.Save(fs);
            }
        }

        /// <summary>
        /// Saves an image to disk.
        /// <para>Can throw just about any exception and should be used in a try catch</para>
        /// </summary>
        /// <param name="stream">The stream to copy to.</param>
        /// <exception cref="Exception"></exception>
        public void Save(Stream stream)
        {
            GifDecoder decoder = new GifDecoder(this.Image);
            GifEncoder encoder = new GifEncoder(decoder.LoopCount);

            for (int i = 0; i < decoder.FrameCount; i++)
            {
                using (GifFrame frame = decoder.GetFrame(i))
                {
                    encoder.EncodeFrame(frame);
                }
            }

            encoder.EncodeToStream(stream);
        }


        public override Bitmap DeepClone()
        {
            if (this.Image == null)
                return null;
            return ImageProcessor.DeepCloneGif(this.Image, false);
        }

        public override void RotateRight90()
        {
            if (this.Image == null)
                return;

            Bitmap gray = ImageProcessor.RotateFlipGif(this.Image, RotateFlipType.Rotate90FlipNone);

            if (gray == null)
                return;

            bool wasAnimating = StopAnimate(true);
            this.Image.Dispose();
            this.Image = gray;

            if (wasAnimating)
                _StartAnimate();
        }

        public override void RotateLeft90()
        {
            if (this.Image == null)
                return;

            Bitmap gray = ImageProcessor.RotateFlipGif(this.Image, RotateFlipType.Rotate270FlipNone);

            if (gray == null)
                return;

            bool wasAnimating = StopAnimate(true);
            this.Image.Dispose();
            this.Image = gray;

            if (wasAnimating)
                _StartAnimate();
        }

        public override void FlipHorizontal()
        {
            if (this.Image == null)
                return;

            Bitmap gray = ImageProcessor.RotateFlipGif(this.Image, RotateFlipType.RotateNoneFlipX);

            if (gray == null)
                return;

            bool wasAnimating = StopAnimate(true);
            this.Image.Dispose();
            this.Image = gray;

            if (wasAnimating)
                _StartAnimate();
        }

        public override void FlipVertical()
        {
            if (this.Image == null)
                return;

            Bitmap gray = ImageProcessor.RotateFlipGif(this.Image, RotateFlipType.RotateNoneFlipY);

            if (gray == null)
                return;

            bool wasAnimating = StopAnimate(true);
            this.Image.Dispose();
            this.Image = gray;

            if (wasAnimating)
                _StartAnimate();
        }

        public override void ConvertGrayscale()
        {
            if (this.Image == null)
                return;

            Bitmap gray = ImageProcessor.GrayscaleGif(this.Image);
            
            if (gray == null)
                return;

            bool wasAnimating = StopAnimate(true);
            this.Image.Dispose();
            this.Image = gray;

            if (wasAnimating)
                _StartAnimate();
        }

        public override void InvertColor()
        {
            if (this.Image == null)
                return;

            Bitmap inverted = ImageProcessor.InvertGif(this.Image);

            if (inverted == null)
                return;

            bool wasAnimating = StopAnimate(true);
            this.Image.Dispose();
            this.Image = inverted;

            if (wasAnimating)
                _StartAnimate();
        }

        public override void Reisze(Size newSize, InterpolationMode interp)
        {
            if (this.Image == null)
                return;
        }

        public override void Reisze(Size newSize, GraphicsUnit units)
        {
            if (this.Image == null)
                return;
        }

        public override void Reisze(Size newSize, InterpolationMode interp, GraphicsUnit units)
        {
            if (this.Image == null)
                return;
        }

        public override ImgFormat GetImageFormat()
        {
            return Gif.ImageFormat;
        }

        public override string GetMimeType()
        {
            return Gif.MimeType;
        }

        /// <summary>
        /// Dispose of the image.
        /// </summary>
        public new void Dispose()
        {
            if (this.IsAnimating)
                StopAnimate();

            if (Image != null)
                Image.Dispose();

            Image = null;
        }


        public static implicit operator Bitmap(Gif gif)
        {
            return gif.Image;
        }

        public static implicit operator Gif(Bitmap bitmap)
        {
            return new Gif(bitmap);
        }

        public static implicit operator Image(Gif gif)
        {
            return gif.Image;
        }

        public static implicit operator Gif(Image bitmap)
        {
            return new Gif(bitmap);
        }


        private bool StopAnimate(bool rememberCallbacks)
        {
            if (this.Image == null)
                return false;

            if (!this.IsAnimating)
                return false;

            for (int i = 0; i < FrameChangedHandlerCallbacks.Count; i++)
            {
                try
                {
                    ImageAnimator.StopAnimate(this.Image, FrameChangedHandlerCallbacks[i]);
                }
                catch { }
            }
            this.IsAnimating = false;
            
            if(!rememberCallbacks)
                FrameChangedHandlerCallbacks.Clear();

            return true;
        }

        private bool _StartAnimate()
        {
            if (this.Image == null)
                return false;
            if (this.FrameChangedHandlerCallbacks == null || this.FrameChangedHandlerCallbacks.Count == 0)
                return false;

            this.IsAnimating = CanAnimate();
            if (this.IsAnimating)
            {
                foreach (EventHandler h in FrameChangedHandlerCallbacks)
                {
                    try
                    {
                        ImageAnimator.Animate(this.Image, h);
                    }
                    catch { }
                }
                return true;
            }
            return false;
        }
    }
}
