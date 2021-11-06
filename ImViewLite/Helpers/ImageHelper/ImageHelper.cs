using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

using ImViewLite.Settings;

namespace ImViewLite.Helpers
{
    public static class ImageHelper
    {
        public static bool ShowExceptions = false;
        /// <summary>
        /// Reads the <see cref="ImgFormat"/> of an image using the identifier bytes.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>
        /// The <see cref="ImgFormat"/> of the image.
        /// </returns>
        public static ImgFormat GetImageFormat(string path)
        {
            return ImageBinaryReader.GetImageFormat(path);
        }


        /// <summary>
        /// Reads the <see cref="ImgFormat"/> using the file extension of the given string.
        /// </summary>
        /// <param name="path">The path or string.</param>
        /// <returns>
        /// The <see cref="ImgFormat"/> of the given string.
        /// </returns>
        public static ImgFormat GetImageFormatFromPath(string path)
        {
            string ext = Helper.GetFilenameExtension(path);

            if (string.IsNullOrEmpty(ext))
                return ImgFormat.nil;

            switch (ext)
            {
                case "png":
                    return ImgFormat.png;
                case "jpg":
                case "jpeg":
                case "jpe":
                case "jfif":
                    return ImgFormat.jpg;
                case "gif":
                    return ImgFormat.gif;
                case "bmp":
                    return ImgFormat.bmp;
                case "tif":
                case "tiff":
                    return ImgFormat.tif;
                case "wrm":
                case "dwrm":
                    return ImgFormat.wrm;
                case "webp":
                    return ImgFormat.webp;
            }
            return ImgFormat.nil;
        }


        /// <summary>
        /// Gets the default pixel format for the given bit depth.
        /// </summary>
        /// <param name="bitDepth">The color depth in bits per pixel.</param>
        /// <returns>
        /// The <see cref="PixelFormat"/>.
        /// </returns>
        public static PixelFormat GetPixelFormatForBitDepth(BitDepth bitDepth)
        {
            switch (bitDepth)
            {
                case BitDepth.Bit1:
                    return PixelFormat.Format1bppIndexed;
                case BitDepth.Bit4:
                    return PixelFormat.Format4bppIndexed;
                case BitDepth.Bit8:
                    return PixelFormat.Format8bppIndexed;
                case BitDepth.Bit16:
                    return PixelFormat.Format16bppRgb565;
                case BitDepth.Bit24:
                    return PixelFormat.Format24bppRgb;
                default:
                    return PixelFormat.Format32bppArgb;
            }
        }


        /// <summary>
        /// Reads the <see cref="System.Drawing.Size"/> of an image from the file.
        /// </summary>
        /// <param name="imagePath"> Path to the image. </param>
        /// <returns> 
        /// The <see cref="System.Drawing.Size"/> of the image; otherwise <see cref="Size.Empty"/>.
        /// </returns>
        public static Size GetImageDimensionsFromFile(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                return Size.Empty;

            Size s = ImageBinaryReader.GetDimensions(imagePath);
            if (s != Size.Empty)
                return s;

            try
            {
                using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (Image image = Image.FromStream(fileStream, false, false))
                {
                    return new Size(image.Width, image.Height);
                }
            }
            catch
            {
                return Size.Empty;
            }
        }


        /// <summary>
        /// Gets the mime type of the image.
        /// </summary>
        /// <param name="image"> The image. </param>
        /// <returns> 
        /// The mime type of the image. 
        /// </returns>
        public static string GetMimeType(Image image)
        {
            // https://stackoverflow.com/a/6336453

            try
            {
                Guid imgguid = image.RawFormat.Guid;
                foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
                {
                    if (codec.FormatID == imgguid)
                        return codec.MimeType;
                }

                if ((ImgFormat)image.Tag == ImgFormat.wrm)
                    return WORM.MimeType;
                if ((ImgFormat)image.Tag == ImgFormat.webp)
                    return Webp.MimeType;
                if ((ImgFormat)image.Tag == ImgFormat.ico)
                    return ICO.MimeType;
            }
            catch { }
            return "image/unknown";
        }


        /// <summary>
        /// Returns a value indicating whether the given pixel format is indexed.
        /// </summary>
        /// <param name="format">The <see cref="PixelFormat"/> to test.</param>
        /// <returns>
        /// The true if the image is indexed; otherwise, false.
        /// </returns>
        public static bool IsIndexed(PixelFormat format)
        {
            return format == PixelFormat.Indexed
                || format == PixelFormat.Format1bppIndexed
                || format == PixelFormat.Format4bppIndexed
                || format == PixelFormat.Format8bppIndexed;
        }


        /// <summary>
        /// Copies the metadata from the source image to the target.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <param name="target">The target image.</param>
        public static void CopyMetadata(Image source, Image target)
        {
            foreach (PropertyItem item in source.PropertyItems)
            {
                try
                {
                    target.SetPropertyItem(item);
                }
                catch
                {
                    // Handle issue https://github.com/JimBobSquarePants/ImageProcessor/issues/571
                    // SetPropertyItem throws a native error if the property item is invalid for that format
                    // but there's no way to handle individual formats so we do a dumb try...catch...
                }
            }
        }


        /// <summary>
        /// Rotates the given bitmap based off the orientation exif property tag.
        /// </summary>
        /// <param name="bmp">The bitmap to rotate flip.</param>
        /// <param name="removeExifOrientationData">Should the orientation tag be removed.</param>
        public static void RotateImageByExifOrientationData(Bitmap bmp, bool removeExifOrientationData = true)
        {
            const int orientationId = (int)ExifPropertyTag.Orientation;

            if (Array.IndexOf(bmp.PropertyIdList, orientationId) == -1)
                return;
           
            PropertyItem propertyItem = bmp.GetPropertyItem(orientationId);
            RotateFlipType rotateType = GetRotateFlipTypeByExifOrientationData(propertyItem.Value[0]);

            if (rotateType == RotateFlipType.RotateNoneFlipNone)
                return;
            
            bmp.RotateFlip(rotateType);

            if (removeExifOrientationData)
            {
                bmp.RemovePropertyItem(orientationId);
            }   
        }


        /// <summary>
        /// Gets the roatefliptype from the exif property tag orientation vlaue.
        /// </summary>
        /// <param name="orientation">The int value of the orientation property tag.</param>
        /// <returns>A <see cref="RotateFlipType"/> for the given orientation.</returns>
        private static RotateFlipType GetRotateFlipTypeByExifOrientationData(int orientation)
        {
            switch (orientation)
            {
                default:
                case 1:
                    return RotateFlipType.RotateNoneFlipNone;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
            }
        }



        /// <summary>
        /// Save a bitmap as a webp file. (Requires the libwebp_x64.dll or libwebp_x86.dll)
        /// </summary>
        /// <param name="img"> The bitmap to encode. </param>
        /// <param name="Path"> The path to save the bitmap. </param>
        /// <param name="q"> The webp quality args. </param>
        /// <param name="collectGarbage"> A bool indicating if GC.Collect should be called after saving. </param>
        /// <returns> true if the bitmap was saved successfully, else false </returns>
        public static bool SaveWebp(Bitmap img, string path, WebPQuality q, bool collectGarbage = true)
        {
            if (!InternalSettings.WebP_Plugin_Exists || string.IsNullOrEmpty(path) || img == null)
                return false;
    
            q = WebPQuality.Default;

            try
            {
                Webp.Save(img, path, q);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (collectGarbage)
                {
                    GC.Collect();
                }
            }
        }

        /// <summary>
        /// Save an image as a webp file. (Requires the libwebp_x64.dll or libwebp_x86.dll)
        /// </summary>
        /// <param name="img"> The image to encode. </param>
        /// <param name="filePath"> The path to save the image. </param>
        /// <param name="q"> The webp quality args. </param>
        /// <param name="collectGarbage"> A bool indicating if GC.Collect should be called after saving. </param>
        /// <returns> true if the image was saved successfully, else false </returns>
        public static bool SaveWebp(Image img, string path, WebPQuality q, bool collectGarbage = true)
        {
            return SaveWebp((Bitmap)img, path, q, collectGarbage);
        }


        /// <summary>
        /// Save an image as a wrm file. (a custom image format i made)
        /// </summary>
        /// <param name="img">The image to encode.</param>
        /// <param name="filePath">The path to save the image.</param>
        /// <returns>True if the image was saved, else false.</returns>
        public static bool SaveWrm(Image img, string path)
        {
            try
            {
                WORM wrm = new WORM(img);
                
                wrm.Save(path);
                return true;
            }
            catch
            {
            }
            return false;
        }


        /// <summary>
        /// Saves an image.
        /// </summary>
        /// <param name="img"> The image to save. </param>
        /// <param name="path"> The path to save the image. </param>
        /// <param name="collectGarbage"> A bool indicating if GC.Collect should be called after saving. </param>
        /// <returns> true if the image was saved successfully, else false </returns>
        public static bool SaveImage(Image img, string path, bool collectGarbage = true)
        {
            if (img == null || string.IsNullOrEmpty(path))
                return false;

            PathHelper.CreateDirectoryFromFilePath(path);

            try
            {
                switch (GetImageFormatFromPath(path))
                {
                    default:
                    case ImgFormat.png:
                        PNG.Save(img, path);
                        return true;
                    case ImgFormat.jpg:
                        JPEG.Save(img, path, JPEG.DefaultQuality);
                        return true;
                    case ImgFormat.bmp:
                        BMP.Save(img, path);
                        return true;
                    case ImgFormat.gif:
                        Gif.Save(img, path);
                        return true;
                    case ImgFormat.tif:
                        TIFF.Save(img, path);
                        return true;
                    case ImgFormat.wrm:
                        WORM.Save(img, path);
                        return true;
                    case ImgFormat.webp:
                        Webp.Save(img, path, WebPQuality.Default);
                        return true;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                if (collectGarbage)
                {
                    GC.Collect();
                }
            }
        }

        /// <summary>
        /// Saves an image.
        /// </summary>
        /// <param name="img"> The image to save. </param>
        /// <param name="path"> The path to save the image. </param>
        /// <param name="collectGarbage"> A bool indicating if GC.Collect should be called after saving. </param>
        /// <returns> true if the image was saved successfully, else false </returns>
        public static bool SaveImage(Bitmap img, string path, bool collectGarbage = true)
        {
            return SaveImage((Image)img, path, collectGarbage);
        }

        /// <summary>
        /// Opens a save file dialog asking where to save an image.
        /// </summary>
        /// <param name="img"> The image to save. </param>
        /// <param name="path"> The path to open. </param>
        /// <param name="collectGarbage"> A bool indicating if GC.Collect should be called after saving. </param>
        /// <returns> The filename of the saved image, null if failed to save or canceled. </returns>
        public static string SaveImageFileDialog(Image img, string path = "", bool collectGarbage = true)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = InternalSettings.Save_File_Dialog_Title;
                sfd.Filter = InternalSettings.Image_Dialog_Filters;
                sfd.DefaultExt = "png";

                if (!string.IsNullOrEmpty(path))
                {
                    sfd.FileName = Path.GetFileName(path);

                    ImgFormat fmt = GetImageFormatFromPath(path);

                    if (fmt != ImgFormat.nil)
                    {
                        switch (fmt)
                        {
                            case ImgFormat.png:
                                sfd.FilterIndex = 1;
                                break;
                            case ImgFormat.jpg:
                                sfd.FilterIndex = 2;
                                break;
                            case ImgFormat.bmp:
                                sfd.FilterIndex = 3;
                                break;
                            case ImgFormat.tif:
                                sfd.FilterIndex = 4;
                                break;
                            case ImgFormat.gif:
                                sfd.FilterIndex = 5;
                                break;
                            case ImgFormat.wrm:
                                sfd.FilterIndex = 6;
                                break;
                            case ImgFormat.webp:
                                if (InternalSettings.WebP_Plugin_Exists)
                                {
                                    sfd.FilterIndex = 7;
                                    break;
                                }
                                sfd.FilterIndex = 2;
                                break;
                        }
                    }
                }

                if (sfd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(sfd.FileName))
                {
                    SaveImage(img, sfd.FileName, collectGarbage);
                    return sfd.FileName;
                }
            }

            return null;
        }




        /// <summary>
        /// Opens a file dialog to select an image.
        /// </summary>
        /// <param name="multiselect"> A bool indicating if multiple files should be allowed. </param>
        /// <param name="form"> The form to be the owner of the dialog. </param>
        /// <param name="initialDirectory"> The initial directory to open. </param>
        /// <returns> A string[] containing the file paths of the images, null if cancel. </returns>
        public static string[] OpenImageFileDialog(bool multiselect, Form form = null, string initialDirectory = null)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = string.Format("{0}|{1}", InternalSettings.All_Image_Files_File_Dialog, InternalSettings.Image_Dialog_Filters);

                ofd.Multiselect = multiselect;

                if (!string.IsNullOrEmpty(initialDirectory))
                {
                    ofd.InitialDirectory = initialDirectory;
                }

                if (ofd.ShowDialog(form) == DialogResult.OK)
                {
                    return ofd.FileNames;
                }
            }

            return null;
        }



        /// <summary>
        /// Loads an image.
        /// </summary>
        /// <param name="path"> The path to the image. </param>
        /// <returns> A bitmap object if the image is loaded, otherwise null. </returns>
        public static Bitmap LoadImageAsBitmap(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return null;

            ImgFormat fmt = GetImageFormat(path);

            if (fmt == ImgFormat.nil)
                fmt = GetImageFormatFromPath(path);

            try
            {
                Bitmap result;

                switch (fmt)
                {
                    case ImgFormat.png:
                        result = PNG.FromFileAsBitmap(path);
                        result.Tag = ImgFormat.png;
                        return result;

                    case ImgFormat.bmp:
                        result = BMP.FromFileAsBitmap(path);
                        result.Tag = ImgFormat.bmp;
                        return result;

                    case ImgFormat.gif:
                        result = Gif.FromFileAsBitmap(path);
                        result.Tag = ImgFormat.gif;
                        return result;

                    case ImgFormat.jpg:
                        result = JPEG.FromFileAsBitmap(path);
                        result.Tag = ImgFormat.jpg;
                        return result;

                    case ImgFormat.tif:
                        result = TIFF.FromFileAsBitmap(path);
                        result.Tag = ImgFormat.tif;
                        return result;

                    case ImgFormat.webp:
                        result = Webp.FromFileAsBitmap(path);
                        result.Tag = ImgFormat.webp;
                        return result;

                    case ImgFormat.wrm:
                        result = WORM.FromFileAsBitmap(path);
                        result.Tag = ImgFormat.wrm;
                        return result;

                    case ImgFormat.ico:
                        result = ICO.FromFileAsBitmap(path);
                        result.Tag = ImgFormat.ico;
                        return result;
                }

            }
            catch
            {
            }
            return null;
        }

        public static IMAGE LoadImage(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return null;

            ImgFormat fmt = GetImageFormat(path);

            try
            {
                switch (fmt)
                {
                    case ImgFormat.png:
                        PNG png = new PNG();
                        png.Load(path);
                        return png;

                    case ImgFormat.bmp:
                        BMP bmp = new BMP();
                        bmp.Load(path);
                        return bmp;

                    case ImgFormat.gif:
                        Gif gif = new Gif();
                        gif.Load(path);
                        return gif;

                    case ImgFormat.jpg:
                        JPEG jpeg = new JPEG();
                        jpeg.Load(path);
                        return jpeg;

                    case ImgFormat.tif:
                        TIFF tiff = new TIFF();
                        tiff.Load(path);
                        return tiff;

                    case ImgFormat.webp:
                        Webp webp = new Webp();
                        webp.Load(path);
                        return webp;

                    case ImgFormat.wrm:
                        WORM worm = new WORM();
                        worm.Load(path);
                        return worm;

                    case ImgFormat.ico:
                        ICO ico = new ICO();
                        ico.Load(path);
                        return ico;
                }

            }
            catch
            {
            }
            return null;
        }
    }
}
