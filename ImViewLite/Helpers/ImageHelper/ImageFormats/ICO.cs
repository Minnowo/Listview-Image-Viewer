using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ImViewLite.Helpers
{
    public class ICO : IMAGE
    {
        #region Readonly / Const / Static 


        /// <summary>
        /// The leading bytes to identify the wrm format.
        /// </summary>
        public static readonly byte[] IdentifierBytes_1 = new byte[] { 0x00, 0x00, 0x01, 0x00 };



        /// <summary>
        /// The leading bytes to identify a WORM image.
        /// </summary>
        public static readonly new byte[][] FileIdentifiers = new byte[][]
        {
            IdentifierBytes_1,
        };


        /// <summary>
        /// The file extensions used for a WORM image.
        /// </summary>
        public static readonly new string[] FileExtensions = new[]
        {
            "ico"
        };


        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        public new const string MimeType = "image/x-icon";


        /// <summary>
        /// Gets the default file extension.
        /// </summary>
        public new const string DefaultExtension = "ico";


        /// <summary>
        /// Gets the WORM iamge format.
        /// </summary>
        public static readonly new ImgFormat ImageFormat = ImgFormat.ico;

        #endregion

        public virtual Bitmap[] Images { get; protected set; }

        public virtual Bitmap this[int Index]
        {
            get
            {
                if (Images == null)
                    return null;
                else
                    return Images[Index];
            }
        }

        public override Bitmap Image 
        {
            get 
            { 
                if (Images == null || selectedImageIndex < 0 || Images.Length == 0) 
                    return null; 
                else 
                    return Images[selectedImageIndex]; 
            }
            protected set { }
        }


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

        /// <summary>
        /// Gets the number of images.
        /// </summary>
        public int Count
        {
            get 
            {
                if (Images == null)
                    return 0;
                return Images.Length; 
            }
        }

        /// <summary>
        /// Get or Set the selected image index.
        /// <para><see cref="ICO.Image"/> returns <see cref="ICO.Images"/> at the selected index.</para>
        /// </summary>
        public int SelectedImageIndex 
        { 
            get 
            { 
                if(selectedImageIndex < 0 && this.Images != null)
                {
                    selectedImageIndex = 0;
                }
                return selectedImageIndex; 
            }
            set 
            {
                if (Images == null)
                    selectedImageIndex = 0;
                else 
                    selectedImageIndex = value.Clamp(0, Images.Length); 
            }
        }
        private int selectedImageIndex = -1;

        


        public ICO()
        {
        }

        public ICO(Image bmp) : this((Bitmap)bmp)
        {
        }

        public ICO(Bitmap bmp)
        {
            this.Images = new Bitmap[1];
            this.Images[0] = bmp;
            this.selectedImageIndex = 0;
            this.Width = bmp.Width;
            this.Height = bmp.Height;
        }

        public ICO(Bitmap[] bitmaps)
        {
            this.Images = bitmaps;
            this.selectedImageIndex = 0;
            this.Width = bitmaps[0].Width;
            this.Height = bitmaps[0].Height;
        }



        #region Static Functions

        /// <summary>
        /// Loads a ICO image and returns it as a bitmap object.
        /// </summary>
        /// <param name="path">The path of the image.</param>
        /// <returns>A <see cref="Bitmap"/> object.</returns>
        public static Bitmap FromFileAsBitmap(string path)
        {
            try
            {
                Bitmap bmp = IMAGE.StandardLoad(path);
                ImageHelper.RotateImageByExifOrientationData(bmp);
                return bmp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn ICO.FromFileAsBitmap(string)");
            }
        }

        /// <summary>
        /// Gets the number of images stored in a .ico file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The number of images in the file, OR -1 if error.</returns>
        public static int GetImageCount(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return -1;

            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] bytes = new byte[4];
                    fileStream.Read(bytes, 0, bytes.Length);

                    if (!ByteHelper.StartsWith(bytes, IdentifierBytes_1))
                    {
                        return -1;
                    }
                    return ByteHelper.ReadInt16LE(fileStream);
                }
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the width and height of the first image directory of a .ico file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The <see cref="Size"/> of the first image directory, OR Size.Empty if error.</returns>
        public static Size GetDimensionOfFirstImageFromFile(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return Size.Empty;

            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] bytes = new byte[6];
                    fileStream.Read(bytes, 0, bytes.Length);

                    if (!ByteHelper.StartsWith(bytes, IdentifierBytes_1))
                    {
                        return Size.Empty;
                    }

                    byte[] wh = new byte[2];
                    fileStream.Read(wh, 0, wh.Length);

                    int width = 0;
                    int height = 0;

                    if (wh[0] == 0x00)
                        width = 256;
                    else
                        width = (byte)wh[0];

                    if (wh[1] == 0x00)
                        height = 256;
                    else
                        height = (byte)wh[0];

                    return new Size(width, height);
                }
            }
            catch
            {
                return Size.Empty;
            }
        }

        #endregion

        public override unsafe void Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("ICO.Load(string)\n\tPath cannot be null or empty");
            if(!File.Exists(path))
                throw new ArgumentException("ICO.Load(string)\n\tFile does not exist");

            this.Clear();

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] bytes = new byte[6];
                fileStream.Read(bytes, 0, bytes.Length);

                if (!ByteHelper.StartsWith(bytes, IdentifierBytes_1))
                    throw new Exception("ICO.Load(string)\n\tInvalid .ico file");


                int imageCount = ByteHelper.ReadInt16LE(bytes, 4);

                this.Images = new Bitmap[imageCount];
                this.selectedImageIndex = 0;

                long streamPosition = -1;
                for (int i = 0; i < imageCount; i++)
                {
                    try
                    {
                        byte[] imageDirectory = new byte[8];
                        fileStream.Read(imageDirectory, 0, imageDirectory.Length);

                        int width = 0;
                        int height = 0;

                        // if the width is 256 its saved as 0x00
                        if (imageDirectory[0] == 0x00)
                            width = 256;
                        else
                            width = (byte)imageDirectory[0];

                        // if the height is 256 its saved as 0x00
                        if (imageDirectory[1] == 0x00)
                            height = 256;
                        else
                            height = (byte)imageDirectory[1];

                        // reserved should be 0
                        if (imageDirectory[3] != 0x00)
                            continue;

                        int colorPlanes = ByteHelper.ReadInt16LE(imageDirectory, 4);

                        // should be 0 or 1
                        if (colorPlanes != 0 && colorPlanes != 1)
                            continue;

                        int bitsPerPixel = ByteHelper.ReadInt16LE(imageDirectory, 6);
                        int imageDataSize = ByteHelper.ReadInt32LE(fileStream);
                        int imageDataOffset = ByteHelper.ReadInt32LE(fileStream);

                        streamPosition = fileStream.Position;

                        // seek to the image data start
                        fileStream.Seek(imageDataOffset, SeekOrigin.Begin);

                        // get the image data
                        byte[] imageData = new byte[imageDataSize];
                        fileStream.Read(imageData, 0, imageData.Length);

                        // reset the position
                        fileStream.Seek(streamPosition, SeekOrigin.Begin);

                        // if the image is stored as a png, use a memory stream to read the image
                        if (ByteHelper.StartsWith(imageData, PNG.IdentifierBytes_1))
                        {
                            MemoryStream mem = new MemoryStream();
                            mem.Write(imageData, 0, imageData.Length);
                            this.Images[i] = (Bitmap)System.Drawing.Image.FromStream(mem);
                            continue;
                        }

                        int dipHeaderSize = ByteHelper.ReadInt32LE(imageData, 0);
                        int index = dipHeaderSize;

                        Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                        this.Images[i] = bmp;

                        BitmapData dstBD = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height),
                            ImageLockMode.ReadWrite,
                            PixelFormat.Format32bppArgb);

                        byte* pDst = (byte*)(void*)dstBD.Scan0;

                        for (int ii = 0; ii < bmp.Width * bmp.Height; ii++)
                        {
                            *pDst = (byte)(imageData[index]); //  B
                            pDst++;
                            index++;

                            *pDst = (byte)(imageData[index]); // G
                            pDst++;
                            index++;

                            *pDst = (byte)(imageData[index]); //  R
                            pDst++;
                            index++;

                            if (bitsPerPixel == 32)
                            {
                                *pDst = (byte)(imageData[index]); //  a
                                index++;
                            }
                            else
                            {
                                *pDst = (byte)255; //  a
                            }
                            pDst++;
                        }
                        bmp.UnlockBits(dstBD);

                        // since the ico stores the bitmap data upside down 
                        // flip the image 
                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    }
                    catch
                    {
                        // chances are the error was thrown when building the image
                        // so at least try and get the others 
                        if (streamPosition != -1)
                            fileStream.Seek(streamPosition, SeekOrigin.Begin);
                        continue;
                    }
                }
            }
        }

        public override void Save(string path)
        {
            throw new Exception("ICO.Save(string)\n\tCurrently doesn't support the saving of .ico files");
        }

        public override void RotateRight90()
        {
            if (this.Images == null)
                return;

            for (int i = 0; i < this.Count; i++)
            {
                if (this.Images[i] != null)
                    this.Images[i].RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
        }

        public override void RotateLeft90()
        {
            if (this.Images == null)
                return;

            for (int i = 0; i < this.Count; i++)
            {
                if (this.Images[i] != null)
                    this.Images[i].RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
        }

        public override void FlipHorizontal()
        {
            if (this.Images == null)
                return;

            for (int i = 0; i < this.Count; i++)
            {
                if (this.Images[i] != null)
                    this.Images[i].RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
        }

        public override void FlipVertical()
        {
            if (this.Images == null)
                return;

            for (int i = 0; i < this.Count; i++)
            {
                if(this.Images[i] != null)
                    this.Images[i].RotateFlip(RotateFlipType.RotateNoneFlipY);
            }
        }

        public override void ConvertGrayscale()
        {
            if (this.Images == null)
                return;

            for(int i = 0; i < this.Count; i++)
            {
                if (this.Images[i] != null)
                    ImageProcessor.GrayscaleBitmapSafe(this.Images[i]);
            }
        }

        public override void InvertColor()
        {
            if (this.Images == null)
                return;

            for (int i = 0; i < this.Count; i++)
            {
                if (this.Images[i] != null)
                    ImageProcessor.InvertBitmapSafe(this.Images[i]);
            }
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

        public override void Clear()
        {
            if (this.Images == null)
                return;

            foreach (Bitmap b in this.Images)
            {
                b?.Dispose();
            }
            this.Images = null;
            this.selectedImageIndex = -1;
        }

        /// <summary>
        /// Dispose of all the images.
        /// </summary>
        public new void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }

        public override ImgFormat GetImageFormat()
        {
            return ICO.ImageFormat;
        }

        public override string GetMimeType()
        {
            return ICO.MimeType;
        }


        public static implicit operator Bitmap(ICO ico)
        {
            return ico.Image;
        }

        public static implicit operator ICO(Bitmap bitmap)
        {
            return new ICO(bitmap);
        }

        public static implicit operator Image(ICO ico)
        {
            return ico.Image;
        }

        public static implicit operator ICO(Image bitmap)
        {
            return new ICO(bitmap);
        }

        public static implicit operator ICO(Bitmap[] bitmaps)
        {
            return new ICO(bitmaps);
        }

        public static implicit operator Bitmap[](ICO ico)
        {
            return ico.Images;
        }
    }
}
