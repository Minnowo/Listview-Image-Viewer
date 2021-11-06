using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ImViewLite.Helpers
{
    /// <summary>
    /// A class for processing images.
    /// These functions work 100% for <see cref="PixelFormat.Format32bppArgb"/>
    /// Seemingly work fine for <see cref="PixelFormat.Format32bppRgb"/>, <see cref="PixelFormat.Format24bppRgb"/> and most other formats.
    /// <para>i'v had no issues using these functions on any image i'v tried. So use at own risk ig.</para>
    /// </summary>
    public static unsafe class ImageProcessor
    {
        #region Graphics 


        /// <summary>
        /// Creates a deep copy of the source image frame allowing you to set the pixel format.
        /// <remarks>
        /// Images with an indexed <see cref="PixelFormat"/> cannot deep copied using a <see cref="Graphics"/>
        /// surface so have to be copied to <see cref="PixelFormat.Format32bppArgb"/> instead.
        /// </remarks>
        /// </summary>
        /// <param name="source">The source image frame.</param>
        /// <param name="targetFormat">The target pixel format.</param>
        /// <returns>The <see cref="Bitmap"/>.</returns>
        public static Bitmap DeepClone(Image source, PixelFormat targetFormat, bool preserveMetaData = true)
        {
            // Create a new image and draw the original pixel data over the top.
            // This will automatically remap the pixel layout.
            PixelFormat pixelFormat = ImageHelper.IsIndexed(targetFormat) ? PixelFormat.Format32bppArgb : targetFormat;

            Bitmap copy = new Bitmap(source.Width, source.Height, pixelFormat);

            copy.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            using (Graphics g = Graphics.FromImage(copy))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.PageUnit = GraphicsUnit.Pixel;

                g.Clear(Color.Transparent);
                g.DrawImageUnscaled(source, 0, 0);
            }

            if (preserveMetaData)
            {
                ImageHelper.CopyMetadata(source, copy);
            }

            return copy;
        }

        public static Bitmap DeepCloneGif(Image source, bool preserveMetaData = true)
        {
            GifDecoder decoder = new GifDecoder(source);
            GifEncoder encoder = new GifEncoder(decoder.LoopCount);

            for (int i = 0; i < decoder.FrameCount; i++)
            {
                using (GifFrame frame = decoder.GetFrame(i))
                {
                    encoder.EncodeFrame(frame);
                }
            }

            Image copy = encoder.Encode();

            if (preserveMetaData)
            {
                ImageHelper.CopyMetadata(source, copy);
            }

            return (Bitmap)copy;
        }

        /// <summary>
        /// Creates a solid color bitmap.
        /// </summary>
        /// <param name="bmpSize"> The size of the bitmap to create. </param>
        /// <param name="fillColor"> The color to fill the bitmap. </param>
        /// <returns>a <see cref="System.Drawing.Bitmap"/> of the given <see cref="System.Drawing.Color"/> with the given <see cref="System.Drawing.Size"/>.</returns>
        public static Bitmap CreateSolidColorBitmap(Size bmpSize, Color fillColor)
        {
            Bitmap b = new Bitmap(bmpSize.Width, bmpSize.Height);

            using (Graphics g = Graphics.FromImage(b))
            using (SolidBrush brush = new SolidBrush(fillColor))
            {
                g.FillRectangle(brush, new Rectangle(0, 0, bmpSize.Width, bmpSize.Height));
            }
            return b;
        }


        /// <summary>
        /// Gets a checkered <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        /// <param name="width">The width of the resultant image.</param>
        /// <param name="height">The height of the resultent image.</param>
        /// <param name="checkerSize">The size of each checkered square.</param>
        /// <param name="color1">Color 1 of the checker pattern.</param>
        /// <param name="color2">Color 2 of the checker pattern.</param>
        /// <returns>A chekcered <see cref="System.Drawing.Bitmap"/> with the given size and colors.</returns>
        public static Bitmap GetCheckeredBitmap(int width, int height, int checkerSize, Color color1, Color color2)
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            DrawCheckers(bmp, checkerSize, color1, color2);
            return bmp;
        }


        /// <summary>
        /// Draws a checker pattern on the given image.
        /// </summary>
        /// <param name="bmp">The <see cref="System.Drawing.Bitmap"/> to draw on.</param>
        /// <param name="checkerSize">The size of the checker pattern.</param>
        /// <param name="color1">Color 1 of the checker pattern.</param>
        /// <param name="color2">Color 2 of the checker pattern.</param>
        public static void DrawCheckers(Bitmap bmp, int checkerSize, Color color1, Color color2)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            using (Image checker = CreateCheckerPattern(checkerSize, color1, color2))
            using (Brush checkerBrush = new TextureBrush(checker, WrapMode.Tile))
            {
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }
        }


        private static Bitmap CreateCheckerPattern(int cellSize, Color color1, Color color2)
        {
            Bitmap resultBMP = new Bitmap(cellSize << 1, cellSize << 1, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(resultBMP))
            {
                using (Brush brush = new SolidBrush(color1))
                {
                    g.FillRectangle(brush, new Rectangle(cellSize, 0, cellSize, cellSize));
                    g.FillRectangle(brush, new Rectangle(0, cellSize, cellSize, cellSize));
                }

                using (Brush brush = new SolidBrush(color2))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, cellSize, cellSize));
                    g.FillRectangle(brush, new Rectangle(cellSize, cellSize, cellSize, cellSize));
                }
            }

            return resultBMP;
        }


        /// <summary>
        /// Resizes the given image.
        /// </summary>
        /// <param name="im"> The image to resize. </param>
        /// <param name="s"> The new size. </param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Image im, Size s)
        {
            Bitmap newIm = new Bitmap(s.Width, s.Height);

            using (Graphics g = Graphics.FromImage(newIm))
            {
                g.DrawImage(im,
                    new Rectangle(new Point(0, 0), s),
                    new Rectangle(0, 0, im.Width, im.Height),
                    GraphicsUnit.Pixel);
            }
            return newIm;
        }


        /// <summary>
        /// Resizes the given image. This returns a new image and the caller should be responsible for disposing of the old image.
        /// </summary>
        /// <param name="im"> The image to resize. </param>
        /// <param name="ri"> The graphics settings and new image size data. </param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Image im, ResizeImage ri)
        {
            Bitmap newIm = new Bitmap(ri.NewSize.Width, ri.NewSize.Height);

            using (Graphics g = Graphics.FromImage(newIm))
            {
                g.InterpolationMode = ri.InterpolationMode;
                g.SmoothingMode = ri.SmoothingMode;
                g.CompositingMode = ri.CompositingMode;
                g.CompositingQuality = ri.CompositingQuality;
                g.PixelOffsetMode = ri.PixelOffsetMode;

                g.DrawImage(im,
                    new Rectangle(new Point(0, 0), ri.NewSize),
                    new Rectangle(0, 0, im.Width, im.Height),
                    ri.GraphicsUnit);
            }
            return newIm;
        }

        
        public static Bitmap ResizeImage(Image image, Size newSize, InterpolationMode interp, GraphicsUnit units)
        {
            if (image == null)
                return null;

            Bitmap newIm = new Bitmap(newSize.Width, newSize.Height, PixelFormat.Format32bppArgb);

            newIm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(newIm))
            {
                g.InterpolationMode = interp;
                
                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);

                    g.DrawImage(image,
                        new Rectangle(Point.Empty, newSize),
                        0, 0, image.Width, image.Height,
                        units,
                        wrapMode);
                }
            }
            return newIm;
        }


        #endregion


        #region Grayscale


        /// <summary>
        /// A value from 0-1 which is used to convert a color to grayscale.
        /// <para>Default: 0.3</para>
        /// <para>Gray = (Red * GrayscaleRedMultiplier) + (Green * GrayscaleGreenMultiplier) + (Blue * GrayscaleBlueMultiplier)</para> 
        /// </summary>
        public static double GrayscaleRedMultiplier
        {
            get { return gsrm; }
            set { gsrm = value.Clamp(0, 1); }
        }
        private static double gsrm = 0.3; // 0.21

        /// <summary>
        /// A value from 0-1 which is used to convert a color to grayscale.
        /// <para>Default: 0.59</para>
        /// <para>Gray = (Red * GrayscaleRedMultiplier) + (Green * GrayscaleGreenMultiplier) + (Blue * GrayscaleBlueMultiplier)</para> 
        /// </summary>
        public static double GrayscaleGreenMultiplier
        {
            get { return gsgm; }
            set { gsgm = value.Clamp(0, 1); }
        }
        private static double gsgm = 0.59; // 0.71

        /// <summary>
        /// A value from 0-1 which is used to convert a color to grayscale.
        /// <para>Default: 0.11</para>
        /// <para>Gray = (Red * GrayscaleRedMultiplier) + (Green * GrayscaleGreenMultiplier) + (Blue * GrayscaleBlueMultiplier)</para> 
        /// </summary>
        public static double GrayscaleBlueMultiplier
        {
            get { return gsbm; }
            set { gsbm = value.Clamp(0, 1); }
        }
        private static double gsbm = 0.11; // 0.071


        
        /// <summary>
        /// Convert the given image to grayscale.
        /// </summary>
        /// <param name="srcImg">The image to convert.</param>
        public static unsafe void GrayscaleBitmap(Bitmap srcImg)
        {
            BitmapData dstBD = srcImg.LockBits(
                new Rectangle(0, 0, srcImg.Width, srcImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            try
            {
                byte* pDst = (byte*)(void*)dstBD.Scan0;

                for (int i = 0; i < dstBD.Stride * dstBD.Height; i += 4)
                {
                    byte gray = (byte)(
                        (gsbm * *(pDst)) +      // B
                        (gsgm * *(pDst + 1)) +  // G
                        (gsrm * *(pDst + 2)));  // R

                    *pDst = gray; // B
                    pDst++;
                    *pDst = gray; // G
                    pDst++;
                    *pDst = gray; // R
                    pDst += 2;    // Skip alpha

                    //*pDst = grey; // A
                    //pDst++;
                }
            }
            finally
            {
                srcImg.UnlockBits(dstBD);
            }
        }


        /// <summary>
		/// Convert the given image to grayscale.
		/// </summary>
		/// <param name="srcImg">The image to convert.</param>
		public static void GrayscaleBitmap(Image srcImg)
        {
            GrayscaleBitmap((Bitmap)srcImg);
        }


        /// <summary>
        /// Convert a bitmap to greyscale.
        /// </summary>
        /// <param name="image"> The bitmap to convert </param>
        /// <returns>True if there were no errors processing the image, else False.</returns>
        public static bool GrayscaleBitmapSafe(Image image)
        {
            return GrayscaleBitmapSafe((Bitmap)image);
        }


        /// <summary>
        /// Convert a bitmap to greyscale.
        /// </summary>
        /// <param name="image"> The bitmap to convert </param>
        /// <returns>True if there were no errors processing the image, else False.</returns>
        public static bool GrayscaleBitmapSafe(Bitmap image)
        {
            if (image == null)
                return false;

            try
            {
                GrayscaleBitmap(image);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Creates a deep clone of the given image and returns it as grayscale. The original image is left untouched.
        /// </summary>
        /// <param name="srcImg">The image to invert clone.</param>
        /// <returns>an gray scaled <see cref="System.Drawing.Imaging.PixelFormat.Format32bppArgb"/> clone of the given image.</returns>
        public static Bitmap GetGrayscaledBitmap(Image srcImg)
        {
            Bitmap newBitmap = ImageProcessor.DeepClone(srcImg, PixelFormat.Format32bppArgb);
            GrayscaleBitmapSafe(newBitmap);
            return newBitmap;
        }


        /// <summary>
        /// Convert the colors of every frame of a Gif object  with animated frames to greyscale.
        /// </summary>
        /// <param name="bmp"> The bitmap to convert. </param>
        /// <returns></returns>
        public static Bitmap GrayscaleGif(Bitmap bmp)
        {
            try
            {
                GifDecoder d = new GifDecoder(bmp);
                GifEncoder e = new GifEncoder(d.LoopCount);

                for (int i = 0; i < d.FrameCount; i++)
                {
                    using (GifFrame frame = d.GetFrame(i))
                    {
                        GrayscaleBitmapSafe(frame.Image);
                        e.EncodeFrame(frame);
                    }
                }
                return (Bitmap)e.Encode();
            }
            catch
            {
                return null;
            }
        }


        #endregion


        #region Inverting


        /// <summary>
        /// Invert the color of the given image.
        /// </summary>
        /// <param name="srcImg">The image to invert.</param>
        public static unsafe void InvertBitmap(Bitmap srcImg)
        {
            BitmapData dstBD = srcImg.LockBits(
                new Rectangle(0, 0, srcImg.Width, srcImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            try
            {
                byte* pDst = (byte*)(void*)dstBD.Scan0;

                for (int i = 0; i < dstBD.Stride * dstBD.Height; i += 4)
                {
                    *pDst = (byte)(255 - *pDst); // invert B
                    pDst++;
                    *pDst = (byte)(255 - *pDst); // invert G
                    pDst++;
                    *pDst = (byte)(255 - *pDst); // invert R
                    pDst += 2; // skip alpha

                    //*pDst = (byte)(255 - *pDst); // invert A
                    //pDst++;						 
                }
            }
            finally
            {
                srcImg.UnlockBits(dstBD);
            }
        }


        /// <summary>
		/// Invert the color of the given image.
		/// </summary>
		/// <param name="srcImg">The image to invert.</param>
		public static void InvertBitmap(Image srcImg)
        {
            InvertBitmap((Bitmap)srcImg);
        }


        /// <summary>
        /// Inverts the colors of a bitmap.
        /// </summary>
        /// <param name="image"> The bitmap to invert </param>
        /// <returns>True if there were no errors processing the image, else False.</returns>
        public static bool InvertBitmapSafe(Image image)
        {
            return InvertBitmapSafe((Bitmap)image);
        }


        /// <summary>
        /// Inverts the colors of a bitmap.
        /// </summary>
        /// <param name="image"> The bitmap to invert </param>
        /// <returns>True if there were no errors processing the image, else False.</returns>
        public static bool InvertBitmapSafe(Bitmap image)
        {
            if (image == null)
                return false;

            try
            {
                InvertBitmap(image);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Creates a deep clone of the given image and returns it with inverted color. The original image is left untouched.
        /// </summary>
        /// <param name="srcImg">The image to invert clone.</param>
        /// <returns>an inverted <see cref="System.Drawing.Imaging.PixelFormat.Format32bppArgb"/> clone of the given image.</returns>
        public static Bitmap GetInvertedBitmap(Image srcImg)
        {
            Bitmap newBitmap = ImageProcessor.DeepClone(srcImg, PixelFormat.Format32bppArgb);
            InvertBitmapSafe(newBitmap);
            return newBitmap;
        }


        /// <summary>
        /// Invert the colors of every frame of a bitmap with animated frames.
        /// </summary>
        /// <param name="bmp"> The bitmap to invert. </param>
        /// <returns></returns>
        public static Bitmap InvertGif(Bitmap bmp)
        {
            try
            {
                GifDecoder d = new GifDecoder(bmp);
                GifEncoder e = new GifEncoder(d.LoopCount);

                for (int i = 0; i < d.FrameCount; i++)
                {
                    using (GifFrame frame = d.GetFrame(i))
                    {
                        InvertBitmapSafe(frame.Image);
                        e.EncodeFrame(frame);
                    }
                }
                return (Bitmap)e.Encode();
            }
            catch
            {
                return null;
            }
        }


        #endregion


        #region Replace Transparent Pixels

        /// <summary>
        /// Replace all pixels with an alpha value less than the given amount.
        /// </summary>
        /// <param name="srcImg">The image to process.</param>
        /// <param name="fill">The color to set transparent pixels.</param>
        /// <param name="alpha">The alpha value to fill colors less than.</param>
        public static unsafe void ReplaceTransparentPixels(Bitmap srcImg, Color fill, byte alpha)
        {
            BitmapData dstBD = srcImg.LockBits(
                new Rectangle(0, 0, srcImg.Width, srcImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            try
            {
                byte* pDst = (byte*)(void*)dstBD.Scan0;

                for (int i = 0; i < dstBD.Stride * dstBD.Height; i += 4)
                {
                    if (*(pDst + 3) < alpha)
                    {
                        *pDst = fill.B; // B
                        pDst++;
                        *pDst = fill.G; // G
                        pDst++;
                        *pDst = fill.R; // R
                        pDst++;
                        *pDst = fill.A; // A
                        pDst++;
                    }
                }
            }
            finally
            {
                srcImg.UnlockBits(dstBD);
            }
        }


        /// <summary>
        /// Replace all pixels with an alpha value less than the given amount.
        /// </summary>
        /// <param name="srcImg">The image to process.</param>
        /// <param name="fill">The color to set transparent pixels.</param>
        /// <param name="alpha">The alpha value to fill colors less than.</param>
		public static void ReplaceTransparentPixels(Image srcImg, Color fill, byte alpha)
        {
            ReplaceTransparentPixels((Bitmap)srcImg, fill, alpha);
        }


        /// <summary>
        /// Replace all pixels with an alpha value less than the given amount.
        /// </summary>
        /// <param name="srcImg">The image to process.</param>
        /// <param name="fill">The color to set transparent pixels.</param>
        /// <param name="alpha">The alpha value to fill colors less than.</param>
        /// <returns>True if there were no errors processing the image, else False.</returns>
        public static bool ReplaceTransparentPixelsSafe(Bitmap srcImg, Color fill, byte alpha)
        {
            if (srcImg == null)
                return false;

            try
            {
                ReplaceTransparentPixels(srcImg, fill, alpha);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Replace all pixels with an alpha value less than the given amount.
        /// </summary>
        /// <param name="srcImg">The image to process.</param>
        /// <param name="fill">The color to set transparent pixels.</param>
        /// <param name="alpha">The alpha value to fill colors less than.</param>
        /// <returns>True if there were no errors processing the image, else False.</returns>
        public static bool ReplaceTransparentPixelsSafe(Image srcImg, Color fill, byte alpha)
        {
            return ReplaceTransparentPixelsSafe((Bitmap)srcImg, fill, alpha);
        }


        #endregion


        #region Copy Pixels

        /// <summary>
        /// Copies the pixels of image 2 onto image 1.
        /// <para>The 2 images MUST have the same width and height.</para>
        /// </summary>
        /// <param name="toUpdate">The image recieving the pixels.</param>
        /// <param name="dataBitmap">The image to copy.</param>
        public static unsafe void CopyPixels(Bitmap toUpdate, Bitmap dataBitmap)
        {
            if (toUpdate.Width != dataBitmap.Width || toUpdate.Height != dataBitmap.Height)
                throw new ArgumentException("CopyPixels(Bitmap, Bitmap)\n\tTHe 2 given images must be the same size");

            Color[] data = GetBitmapColors(dataBitmap);

            BitmapData dstBD = toUpdate.LockBits(
                new Rectangle(0, 0, toUpdate.Width, toUpdate.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            try
            {
                byte* pDst = (byte*)(void*)dstBD.Scan0;

                for (int i = 0; i < data.Length; i++)
                {
                    *(pDst++) = data[i].B; // B
                    *(pDst++) = data[i].G; // G
                    *(pDst++) = data[i].R; // R
                    *(pDst++) = data[i].A; // A		 
                }
            }
            finally
            {
                toUpdate.UnlockBits(dstBD);
            }
        }


        /// <summary>
        /// Copies the pixels of image 2 onto image 1.
        /// <para>The 2 images MUST have the same width and height.</para>
        /// </summary>
        /// <param name="toUpdate">The image recieving the pixels.</param>
        /// <param name="dataBitmap">The image to copy.</param>
        public static unsafe void CopyPixels(Image toUpdate, Bitmap dataBitmap)
        {
            CopyPixels((Bitmap)toUpdate, dataBitmap);
        }


        /// <summary>
        /// Copies the pixels of image 2 onto image 1.
        /// <para>The 2 images MUST have the same width and height.</para>
        /// </summary>
        /// <param name="toUpdate">The image recieving the pixels.</param>
        /// <param name="dataBitmap">The image to copy.</param>
        public static unsafe void CopyPixels(Bitmap toUpdate, Image dataBitmap)
        {
            CopyPixels(toUpdate, (Bitmap)dataBitmap);
        }


        /// <summary>
        /// Copies the pixels of image 2 onto image 1.
        /// <para>The 2 images MUST have the same width and height.</para>
        /// </summary>
        /// <param name="toUpdate">The image recieving the pixels.</param>
        /// <param name="dataBitmap">The image to copy.</param>
        public static unsafe void CopyPixels(Image toUpdate, Image dataBitmap)
        {
            CopyPixels((Bitmap)toUpdate, (Bitmap)dataBitmap);
        }


        /// <summary>
        /// Updates a bitmap's pixel data using pointers.
        /// </summary>
        /// <param name="toUpdate"> The bitmap that is going to be written on. </param>
        /// <param name="dataBitmap"> The bitmap that the data comes from. </param>
        /// <returns>True if there were no errors processing the image, else False.</returns>
        public static bool CopyPixelsSafe(Image toUpdate, Image dataBitmap)
        {
            if (toUpdate.Width != dataBitmap.Width || toUpdate.Height != dataBitmap.Height)
                return false;

            try
            {
                CopyPixels(toUpdate, dataBitmap);
                return true;
            }
            catch
            {
                return false;
            }
        }


        #endregion


        #region Image -> Color[], Color[] -> Image


        /// <summary>
        /// Gets a <see cref="System.Drawing.Color"/>[] from the given image.
        /// </summary>
        /// <param name="srcImg">The image.</param>
        /// <returns>A <see cref="System.Drawing.Color"/>[] of pixels from the image. OR null if the image is null.</returns>
        public static unsafe Color[] GetBitmapColors(Bitmap srcImg)
        {
            if (srcImg == null)
                return null;

            Color[] result = new Color[srcImg.Width * srcImg.Height];

            BitmapData dstBD = srcImg.LockBits(
                new Rectangle(0, 0, srcImg.Width, srcImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            
            try
            {
                byte* pDst = (byte*)(void*)dstBD.Scan0;

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = Color.FromArgb(*(pDst + 3), *(pDst + 2), *(pDst + 1), *pDst);
                    pDst += 4;
                }
            }
            finally
            {
                srcImg.UnlockBits(dstBD);
            }
            return result;
        }


        /// <summary>
        /// Gets a <see cref="System.Drawing.Color"/>[] from the given image.
        /// </summary>
        /// <param name="srcImg">The image.</param>
        /// <returns>A <see cref="System.Drawing.Color"/>[] of pixels from the image.. OR null if the image is null.</returns>
        public static Color[] GetBitmapColors(Image srcImg)
        {
            return GetBitmapColors((Bitmap)srcImg);
        }



        /// <summary>
        /// Gets a <see cref="System.Drawing.Bitmap"/> from the given <see cref="System.Drawing.Color"/>[].
        /// </summary>
        /// <param name="srcAry">The <see cref="System.Drawing.Color"/>[].</param>
        /// <param name="size">The dimensions of the bitmap.</param>
        /// <returns>A <see cref="System.Drawing.Bitmap"/> built from the given array. OR null if the array is null or empty.</returns>
        public static unsafe Bitmap GetBitmapFromArray(Color[] srcAry, Size size)
        {
            if (srcAry == null || srcAry.Length < 1)
                return null;

            Bitmap resultBmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
            BitmapData dstBD = resultBmp.LockBits(
                new Rectangle(0, 0, resultBmp.Width, resultBmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            try
            {
                byte* pDst = (byte*)(void*)dstBD.Scan0;

                for (int i = 0; i < srcAry.Length; i++)
                {
                    *(pDst++) = srcAry[i].B; // B
                    *(pDst++) = srcAry[i].G; // G
                    *(pDst++) = srcAry[i].R; // R
                    *(pDst++) = srcAry[i].A; // A		 
                }
            }
            finally
            {
                resultBmp.UnlockBits(dstBD);
            }
            return resultBmp;
        }


        #endregion


        #region Rotation / Flips


        /// <summary>
        /// Decode a gif, apply the given <see cref="RotateFlipType"/> and return the re-encoded the gif.
        /// </summary>
        /// <param name="bmp">The gif to process.</param>
        /// <param name="rotateFlip">The <see cref="RotateFlipType"/> to apply.</param>
        /// <returns>A rotated flipped gif based on the <see cref="RotateFlipType"/>; otherwise null.</returns>
        public static Bitmap RotateFlipGif(Bitmap bmp, RotateFlipType rotateFlip)
        {
            try
            {
                GifDecoder d = new GifDecoder(bmp);
                GifEncoder e = new GifEncoder(d.LoopCount);

                for (int i = 0; i < d.FrameCount; i++)
                {
                    using (GifFrame frame = d.GetFrame(i))
                    {
                        frame.Image.RotateFlip(rotateFlip);
                        e.EncodeFrame(frame);
                    }
                }
                return (Bitmap)e.Encode();
            }
            catch
            {
                return null;
            }
        }


        #endregion

    }
}
