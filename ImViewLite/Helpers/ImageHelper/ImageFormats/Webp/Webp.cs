#region License Information (GPL v3)

/*
    Nyan.Imaging - A library with a bunch of helpers and other stuff
    Copyright (c) 2007-2020 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImViewLite.Helpers
{
    // https://github.com/JosePineiro/WebP-wrapper

    public sealed class Webp : IMAGE
    {
        #region Readonly / Const / Static 

        public const string libwebP_x64 = "plugins\\libwebp_x64.dll";
        public const string libwebP_x86 = "plugins\\libwebp_x86.dll";

        public const int WEBP_MAX_DIMENSION = 16383;

        /// <summary>
        /// The leading bytes to identify the webp format.
        /// </summary>
        public static readonly byte[] IdentifierBytes_1 = new byte[4] { 0x52, 0x49, 0x46, 0x46 };


        /// <summary>
        /// The file extensions used for a WORM image.
        /// </summary>
        public static readonly new string[] FileExtensions = new[]
        {
            "webp"
        };


        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains.
        /// </summary>
        public new const string MimeType = "image/webp";


        /// <summary>
        /// Gets the default file extension.
        /// </summary>
        public new const string DefaultExtension = "webp";


        /// <summary>
        /// Gets the WORM iamge format.
        /// </summary>
        public static readonly new ImgFormat ImageFormat = ImgFormat.webp;

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
        /// Gets or Sets How should the webp should be encoded / what quality and speed should be used.
        /// </summary>
        public WebPQuality EncodingFormat { get; set; }


        private delegate int MyWriterDelegate([InAttribute()] IntPtr data, UIntPtr data_size, ref WebPPicture picture);


        public Webp()
        {
        }

        public Webp(Image bmp) : this((Bitmap)bmp)
        {
        }

        public Webp(Bitmap bmp)
        {
            if (bmp.Width > WEBP_MAX_DIMENSION || bmp.Height > WEBP_MAX_DIMENSION)
                throw new Exception($"WORM images do not support width or height larger than {WEBP_MAX_DIMENSION}");

            this.Image = bmp;
            this.EncodingFormat = WebPQuality.Default;
        }


        #region Static Functions 

        /// <summary>
        /// Save a webp image with the given quality.
        /// </summary>
        /// <param name="img">The image to save.</param>
        /// <param name="Path">The path to the file.</param>
        /// <param name="quality">The encoding settings.</param>
        public static void Save(Image img, string Path, WebPQuality quality)
        {
            try
            {
                using (Webp webp = new Webp())
                {
                    byte[] rawWebP;

                    switch (quality.Format)
                    {
                        default:
                        case WebpEncodingFormat.EncodeLossless:
                            rawWebP = webp.EncodeLossless((Bitmap)img, quality.Speed);
                            break;
                        case WebpEncodingFormat.EncodeNearLossless:
                            rawWebP = webp.EncodeNearLossless((Bitmap)img, quality.Quality, quality.Speed);
                            break;
                        case WebpEncodingFormat.EncodeLossy:
                            rawWebP = webp.EncodeLossy((Bitmap)img, quality.Quality, quality.Speed);
                            break;
                    }

                    File.WriteAllBytes(Path, rawWebP);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn Webp.Save(Bitmap, string, WebPQuality)");
            }
        }

        /// <summary>
        /// Read a WebP file
        /// </summary>
        /// <param name="path">WebP file to load</param>
        /// <returns>Bitmap with the WebP image</returns>
        public static Bitmap FromFileAsBitmap(string path)
        {
            try
            {
                using (Webp webp = new Webp())
                {
                    byte[] rawWebP = File.ReadAllBytes(path);
                    Bitmap image = webp.Decode(rawWebP);
                    ImageHelper.RotateImageByExifOrientationData(image);
                    image.Tag = ImgFormat.webp;
                    return image;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn WebP.Load");
            }
        }

        #endregion

        #region | Public Decode Functions |


        public override void Load(string path)
        {
            try
            {
                Clear();
                byte[] rawWebP = File.ReadAllBytes(path);
                this.Image = Decode(rawWebP);
                ImageHelper.RotateImageByExifOrientationData(this.Image);
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message + "\r\nIn WebP.Load"); 
            }
        }

        /// <summary>
        /// Decode a WebP image
        /// </summary>
        /// <param name="rawWebP">The data to uncompress</param>
        /// <returns>Bitmap with the WebP image</returns>
        public Bitmap Decode(byte[] rawWebP)
        {
            int size;

            GCHandle pinnedWebP = GCHandle.Alloc(rawWebP, GCHandleType.Pinned);

            Bitmap bmp = null;
            BitmapData bmpData = null;

            try
            {
                //Get image width and height
                GetInfo(rawWebP, out int imgWidth, out int imgHeight, out bool hasAlpha, out bool hasAnimation, out string format);

                //Create a BitmapData and Lock all pixels to be written
                if (hasAlpha)
                {
                    bmp = new Bitmap(imgWidth, imgHeight, PixelFormat.Format32bppArgb);
                }
                else
                { 
                    bmp = new Bitmap(imgWidth, imgHeight, PixelFormat.Format24bppRgb); 
                }

                bmpData = bmp.LockBits(new Rectangle(0, 0, imgWidth, imgHeight), ImageLockMode.WriteOnly, bmp.PixelFormat);

                //Uncompress the image
                int outputSize = bmpData.Stride * imgHeight;
                IntPtr ptrData = pinnedWebP.AddrOfPinnedObject();

                if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    size = UnsafeNativeMethods.WebPDecodeBGRInto(ptrData, rawWebP.Length, bmpData.Scan0, outputSize, bmpData.Stride);
                }
                else
                {
                    size = UnsafeNativeMethods.WebPDecodeBGRAInto(ptrData, rawWebP.Length, bmpData.Scan0, outputSize, bmpData.Stride);
                }

                if (size == 0)
                {
                    throw new Exception("Can´t encode WebP");
                }

                return bmp;
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message + "\r\nIn WebP.Decode"); 
            }
            finally
            {
                //Unlock the pixels
                if (bmpData != null)
                {
                    bmp.UnlockBits(bmpData);
                }

                //Free memory
                if (pinnedWebP.IsAllocated)
                {
                    pinnedWebP.Free();
                }
            }
        }

        /// <summary>
        /// Decode a WebP image
        /// </summary>
        /// <param name="rawWebP">the data to uncompress</param>
        /// <param name="options">Options for advanced decode</param>
        /// <returns>Bitmap with the WebP image</returns>
        public Bitmap Decode(byte[] rawWebP, WebPDecoderOptions options)
        {
            GCHandle pinnedWebP = GCHandle.Alloc(rawWebP, GCHandleType.Pinned);
            Bitmap bmp = null;
            BitmapData bmpData = null;
            VP8StatusCode result;

            try
            {
                WebPDecoderConfig config = new WebPDecoderConfig();
                if (UnsafeNativeMethods.WebPInitDecoderConfig(ref config) == 0)
                {
                    throw new Exception("WebPInitDecoderConfig failed. Wrong version?");
                }

                // Read the .webp input file information
                IntPtr ptrRawWebP = pinnedWebP.AddrOfPinnedObject();
                int height;
                int width;

                if (options.use_scaling == 0)
                {
                    result = UnsafeNativeMethods.WebPGetFeatures(ptrRawWebP, rawWebP.Length, ref config.input);
                    if (result != VP8StatusCode.VP8_STATUS_OK)
                    {
                        throw new Exception("Failed WebPGetFeatures with error " + result);
                    }

                    //Test cropping values
                    if (options.use_cropping == 1)
                    {
                        if (options.crop_left + options.crop_width > config.input.Width ||
                            options.crop_top + options.crop_height > config.input.Height)
                        {
                            throw new Exception("Crop options exceded WebP image dimensions");
                        }
                        width = options.crop_width;
                        height = options.crop_height;
                    }
                }
                else
                {
                    width = options.scaled_width;
                    height = options.scaled_height;
                }

                config.options.bypass_filtering = options.bypass_filtering;
                config.options.no_fancy_upsampling = options.no_fancy_upsampling;
                config.options.use_cropping = options.use_cropping;
                config.options.crop_left = options.crop_left;
                config.options.crop_top = options.crop_top;
                config.options.crop_width = options.crop_width;
                config.options.crop_height = options.crop_height;
                config.options.use_scaling = options.use_scaling;
                config.options.scaled_width = options.scaled_width;
                config.options.scaled_height = options.scaled_height;
                config.options.use_threads = options.use_threads;
                config.options.dithering_strength = options.dithering_strength;
                config.options.flip = options.flip;
                config.options.alpha_dithering_strength = options.alpha_dithering_strength;

                //Create a BitmapData and Lock all pixels to be written
                if (config.input.Has_alpha == 1)
                {
                    config.output.colorspace = WEBP_CSP_MODE.MODE_bgrA;
                    bmp = new Bitmap(config.input.Width, config.input.Height, PixelFormat.Format32bppArgb);
                }
                else
                {
                    config.output.colorspace = WEBP_CSP_MODE.MODE_BGR;
                    bmp = new Bitmap(config.input.Width, config.input.Height, PixelFormat.Format24bppRgb);
                }
                bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);

                // Specify the output format
                config.output.u.RGBA.rgba = bmpData.Scan0;
                config.output.u.RGBA.stride = bmpData.Stride;
                config.output.u.RGBA.size = (UIntPtr)(bmp.Height * bmpData.Stride);
                config.output.height = bmp.Height;
                config.output.width = bmp.Width;
                config.output.is_external_memory = 1;

                // Decode
                result = UnsafeNativeMethods.WebPDecode(ptrRawWebP, rawWebP.Length, ref config);
                if (result != VP8StatusCode.VP8_STATUS_OK)
                {
                    throw new Exception("Failed WebPDecode with error " + result);
                }
                UnsafeNativeMethods.WebPFreeDecBuffer(ref config.output);

                return bmp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn WebP.Decode");
            }
            finally
            {
                //Unlock the pixels
                if (bmpData != null)
                {
                    bmp.UnlockBits(bmpData);
                }

                //Free memory
                if (pinnedWebP.IsAllocated)
                {
                    pinnedWebP.Free();
                }
            }
        }

        /// <summary>Get Thumbnail from webP in mode faster/low quality</summary>
        /// <param name="rawWebP">The data to uncompress</param>
        /// <param name="width">Wanted width of thumbnail</param>
        /// <param name="height">Wanted height of thumbnail</param>
        /// <returns>Bitmap with the WebP thumbnail in 24bpp</returns>
        public Bitmap GetThumbnailFast(byte[] rawWebP, int width, int height)
        {
            GCHandle pinnedWebP = GCHandle.Alloc(rawWebP, GCHandleType.Pinned);
            Bitmap bmp = null;
            BitmapData bmpData = null;

            try
            {
                WebPDecoderConfig config = new WebPDecoderConfig();
                if (UnsafeNativeMethods.WebPInitDecoderConfig(ref config) == 0)
                {
                    throw new Exception("WebPInitDecoderConfig failed. Wrong version?");
                }

                // Set up decode options
                config.options.bypass_filtering = 1;
                config.options.no_fancy_upsampling = 1;
                config.options.use_threads = 1;
                config.options.use_scaling = 1;
                config.options.scaled_width = width;
                config.options.scaled_height = height;

                // Create a BitmapData and Lock all pixels to be written
                bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);

                // Specify the output format
                config.output.colorspace = WEBP_CSP_MODE.MODE_BGR;
                config.output.u.RGBA.rgba = bmpData.Scan0;
                config.output.u.RGBA.stride = bmpData.Stride;
                config.output.u.RGBA.size = (UIntPtr)(height * bmpData.Stride);
                config.output.height = height;
                config.output.width = width;
                config.output.is_external_memory = 1;

                // Decode
                IntPtr ptrRawWebP = pinnedWebP.AddrOfPinnedObject();
                VP8StatusCode result = UnsafeNativeMethods.WebPDecode(ptrRawWebP, rawWebP.Length, ref config);
                if (result != VP8StatusCode.VP8_STATUS_OK)
                {
                    throw new Exception("Failed WebPDecode with error " + result);
                }

                UnsafeNativeMethods.WebPFreeDecBuffer(ref config.output);

                return bmp;
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message + "\r\nIn WebP.Thumbnail"); 
            }
            finally
            {
                //Unlock the pixels
                if (bmpData != null)
                {
                    bmp.UnlockBits(bmpData);
                }

                //Free memory
                if (pinnedWebP.IsAllocated)
                {
                    pinnedWebP.Free();
                }
            }
        }

        /// <summary>Thumbnail from webP in mode slow/high quality</summary>
        /// <param name="rawWebP">The data to uncompress</param>
        /// <param name="width">Wanted width of thumbnail</param>
        /// <param name="height">Wanted height of thumbnail</param>
        /// <returns>Bitmap with the WebP thumbnail</returns>
        public Bitmap GetThumbnailQuality(byte[] rawWebP, int width, int height)
        {
            GCHandle pinnedWebP = GCHandle.Alloc(rawWebP, GCHandleType.Pinned);
            Bitmap bmp = null;
            BitmapData bmpData = null;

            try
            {
                WebPDecoderConfig config = new WebPDecoderConfig();
                if (UnsafeNativeMethods.WebPInitDecoderConfig(ref config) == 0)
                {
                    throw new Exception("WebPInitDecoderConfig failed. Wrong version?");
                }

                IntPtr ptrRawWebP = pinnedWebP.AddrOfPinnedObject();
                VP8StatusCode result = UnsafeNativeMethods.WebPGetFeatures(ptrRawWebP, rawWebP.Length, ref config.input);
                
                if (result != VP8StatusCode.VP8_STATUS_OK)
                {
                    throw new Exception("Failed WebPGetFeatures with error " + result);
                }

                // Set up decode options
                config.options.bypass_filtering = 0;
                config.options.no_fancy_upsampling = 0;
                config.options.use_threads = 1;
                config.options.use_scaling = 1;
                config.options.scaled_width = width;
                config.options.scaled_height = height;

                //Create a BitmapData and Lock all pixels to be written
                if (config.input.Has_alpha == 1)
                {
                    config.output.colorspace = WEBP_CSP_MODE.MODE_bgrA;
                    bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                }
                else
                {
                    config.output.colorspace = WEBP_CSP_MODE.MODE_BGR;
                    bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                }

                bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);

                // Specify the output format
                config.output.u.RGBA.rgba = bmpData.Scan0;
                config.output.u.RGBA.stride = bmpData.Stride;
                config.output.u.RGBA.size = (UIntPtr)(height * bmpData.Stride);
                config.output.height = height;
                config.output.width = width;
                config.output.is_external_memory = 1;

                // Decode
                result = UnsafeNativeMethods.WebPDecode(ptrRawWebP, rawWebP.Length, ref config);
                
                if (result != VP8StatusCode.VP8_STATUS_OK)
                {
                    throw new Exception("Failed WebPDecode with error " + result);
                }

                UnsafeNativeMethods.WebPFreeDecBuffer(ref config.output);

                return bmp;
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message + "\r\nIn WebP.Thumbnail"); 
            }
            finally
            {
                //Unlock the pixels
                if (bmpData != null)
                {
                    bmp.UnlockBits(bmpData);
                }

                //Free memory
                if (pinnedWebP.IsAllocated)
                {
                    pinnedWebP.Free();
                }
            }
        }

        #endregion

        #region | Public Encode Functions |

        public override void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Webp.Save(string)\n\tPath cannot be null or empty");
            if (this.Image == null)
                throw new ArgumentException("Webp.Save(string)\n\tImage cannot be null");

            byte[] rawWebP;

            switch (EncodingFormat.Format)
            {
                default:
                case WebpEncodingFormat.EncodeLossless:
                    rawWebP = EncodeLossless(this.Image, EncodingFormat.Speed);
                    break;
                case WebpEncodingFormat.EncodeNearLossless:
                    rawWebP = EncodeNearLossless(this.Image, EncodingFormat.Quality, EncodingFormat.Speed);
                    break;
                case WebpEncodingFormat.EncodeLossy:
                    rawWebP = EncodeLossy(this.Image, EncodingFormat.Quality, EncodingFormat.Speed);
                    break;
            }

            File.WriteAllBytes(path, rawWebP);
        }

        /// <summary>
        /// Save bitmap to file in WebP format
        /// </summary>
        /// <param name="bmp">Bitmap with the WebP image</param>
        /// <param name="pathFileName">The file to write</param>
        /// <param name="quality">Between 0 (lower quality, lowest file size) and 100 (highest quality, higher file size)</param>
        public void Save(Bitmap bmp, string pathFileName, int quality = 75)
        {
            byte[] rawWebP;

            try
            {
                //Encode in webP format
                rawWebP = EncodeLossy(bmp, quality);

                //Write webP file
                File.WriteAllBytes(pathFileName, rawWebP);
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message + "\r\nIn WebP.Save"); 
            }
        }

        /// <summary>
        /// Lossy encoding bitmap to WebP (Simple encoding API)
        /// </summary>
        /// <param name="bmp">Bitmap with the image</param>
        /// <param name="quality">Between 0 (lower quality, lowest file size) and 100 (highest quality, higher file size)</param>
        /// <returns>Compressed data</returns>
        public byte[] EncodeLossy(Bitmap bmp, int quality = 75)
        {
            //test bmp
            if (bmp.Width == 0 || bmp.Height == 0)
            {
                throw new ArgumentException("Bitmap contains no data.", "bmp");
            }
            
            if (bmp.Width > WEBP_MAX_DIMENSION || bmp.Height > WEBP_MAX_DIMENSION)
            {
                throw new NotSupportedException("Bitmap's dimension is too large. Max is " + WEBP_MAX_DIMENSION + "x" + WEBP_MAX_DIMENSION + " pixels.");
            }

            PixelFormat pf = bmp.PixelFormat;
            if (bmp.PixelFormat != PixelFormat.Format24bppRgb && bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                pf = PixelFormat.Format24bppRgb;
            }

            BitmapData bmpData = null;
            IntPtr unmanagedData = IntPtr.Zero;
            int size;

            try
            {
                //Get bmp data
                bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, pf);

                //Compress the bmp data
                if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    size = UnsafeNativeMethods.WebPEncodeBGR(bmpData.Scan0, bmp.Width, bmp.Height, bmpData.Stride, quality, out unmanagedData);
                }
                else
                {
                    size = UnsafeNativeMethods.WebPEncodeBGRA(bmpData.Scan0, bmp.Width, bmp.Height, bmpData.Stride, quality, out unmanagedData);
                }

                if (size == 0)
                {
                    throw new Exception("Can´t encode WebP");
                }

                //Copy image compress data to output array
                byte[] rawWebP = new byte[size];
                Marshal.Copy(unmanagedData, rawWebP, 0, size);

                return rawWebP;
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message + "\r\nIn WebP.EncodeLossly"); 
            }
            finally
            {
                //Unlock the pixels
                if (bmpData != null)
                {
                    bmp.UnlockBits(bmpData);
                }

                //Free memory
                if (unmanagedData != IntPtr.Zero)
                {
                    UnsafeNativeMethods.WebPFree(unmanagedData);
                }
            }
        }

        /// <summary>
        /// Lossy encoding bitmap to WebP (Advanced encoding API)
        /// </summary>
        /// <param name="bmp">Bitmap with the image</param>
        /// <param name="quality">Between 0 (lower quality, lowest file size) and 100 (highest quality, higher file size)</param>
        /// <param name="speed">Between 0 (fastest, lowest compression) and 9 (slower, best compression)</param>
        /// <returns>Compressed data</returns>
        public byte[] EncodeLossy(Bitmap bmp, int quality, int speed, bool info = false)
        {
            //Inicialize config struct
            WebPConfig config = new WebPConfig();

            //Set compresion parameters
            if (UnsafeNativeMethods.WebPConfigInit(ref config, WebPPreset.WEBP_PRESET_DEFAULT, 75) == 0)
            {
                throw new Exception("Can´t config preset");
            }

            // Add additional tuning:
            config.method = speed;

            if (config.method > 6)
            {
                config.method = 6;
            }
            
            config.quality = quality;
            config.autofilter = 1;
            config.pass = speed + 1;
            config.segments = 4;
            config.partitions = 3;
            config.thread_level = 1;
            config.alpha_quality = quality;
            config.alpha_filtering = 2;
            config.use_sharp_yuv = 1;

            //Old version don´t suport preprocessing 4
            if (UnsafeNativeMethods.WebPGetDecoderVersion() > 1082)
            {
                config.preprocessing = 4;
                config.use_sharp_yuv = 1;
            }
            else
            {
                config.preprocessing = 3;
            }

            return AdvancedEncode(bmp, config, info);
        }

        /// <summary>Lossless encoding bitmap to WebP (Simple encoding API)</summary>
        /// <param name="bmp">Bitmap with the image</param>
        /// <returns>Compressed data</returns>
        public byte[] EncodeLossless(Bitmap bmp)
        {
            //test bmp
            if (bmp.Width == 0 || bmp.Height == 0)
            {
                throw new ArgumentException("Bitmap contains no data.", "bmp");
            }

            if (bmp.Width > WEBP_MAX_DIMENSION || bmp.Height > WEBP_MAX_DIMENSION)
            {
                throw new NotSupportedException("Bitmap's dimension is too large. Max is " + WEBP_MAX_DIMENSION + "x" + WEBP_MAX_DIMENSION + " pixels.");
            }

            PixelFormat pf = bmp.PixelFormat;
            if (bmp.PixelFormat != PixelFormat.Format24bppRgb && bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                pf = PixelFormat.Format24bppRgb;
            }

            BitmapData bmpData = null;
            IntPtr unmanagedData = IntPtr.Zero;

            try
            {
                //Get bmp data
                bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, pf);

                //Compress the bmp data
                int size;
                if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    size = UnsafeNativeMethods.WebPEncodeLosslessBGR(bmpData.Scan0, bmp.Width, bmp.Height, bmpData.Stride, out unmanagedData);
                }
                else
                {
                    size = UnsafeNativeMethods.WebPEncodeLosslessBGRA(bmpData.Scan0, bmp.Width, bmp.Height, bmpData.Stride, out unmanagedData);
                }

                //Copy image compress data to output array
                byte[] rawWebP = new byte[size];
                Marshal.Copy(unmanagedData, rawWebP, 0, size);

                return rawWebP;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nIn WebP.EncodeLossless (Simple)");
            }
            finally
            {
                //Unlock the pixels
                if (bmpData != null) 
                { 
                    bmp.UnlockBits(bmpData);
                }

                //Free memory
                if (unmanagedData != IntPtr.Zero)
                {
                    UnsafeNativeMethods.WebPFree(unmanagedData);
                }
            }
        }

        /// <summary>
        /// Lossless encoding image in bitmap (Advanced encoding API)
        /// </summary>
        /// <param name="bmp">Bitmap with the image</param>
        /// <param name="speed">Between 0 (fastest, lowest compression) and 9 (slower, best compression)</param>
        /// <returns>Compressed data</returns>
        public byte[] EncodeLossless(Bitmap bmp, int speed)
        {
            //Inicialize config struct
            WebPConfig config = new WebPConfig();

            //Set compresion parameters
            if (UnsafeNativeMethods.WebPConfigInit(ref config, WebPPreset.WEBP_PRESET_DEFAULT, (speed + 1) * 10) == 0)
            {
                throw new Exception("Can´t config preset");
            }

            //Old version of dll not suport info and WebPConfigLosslessPreset
            if (UnsafeNativeMethods.WebPGetDecoderVersion() > 1082)
            {
                if (UnsafeNativeMethods.WebPConfigLosslessPreset(ref config, speed) == 0)
                    throw new Exception("Can´t config lossless preset");
            }
            else
            {
                config.lossless = 1;
                config.method = speed;

                if (config.method > 6)
                    config.method = 6;

                config.quality = (speed + 1) * 10;
            }

            config.pass = speed + 1;
            config.thread_level = 1;
            config.alpha_filtering = 2;
            config.use_sharp_yuv = 1;
            config.exact = 0;

            return AdvancedEncode(bmp, config, false);
        }

        /// <summary>
        /// Near lossless encoding image in bitmap
        /// </summary>
        /// <param name="bmp">Bitmap with the image</param>
        /// <param name="quality">Between 0 (lower quality, lowest file size) and 100 (highest quality, higher file size)</param>
        /// <param name="speed">Between 0 (fastest, lowest compression) and 9 (slower, best compression)</param>
        /// <returns>Compress data</returns>
        public byte[] EncodeNearLossless(Bitmap bmp, int quality, int speed = 9)
        {
            //test dll version
            if (UnsafeNativeMethods.WebPGetDecoderVersion() <= 1082)
            {
                throw new Exception("This dll version not suport EncodeNearLossless");
            }

            //Inicialize config struct
            WebPConfig config = new WebPConfig();

            //Set compresion parameters
            if (UnsafeNativeMethods.WebPConfigInit(ref config, WebPPreset.WEBP_PRESET_DEFAULT, (speed + 1) * 10) == 0)
            {
                throw new Exception("Can´t config preset");
            }

            if (UnsafeNativeMethods.WebPConfigLosslessPreset(ref config, speed) == 0)
            {
                throw new Exception("Can´t config lossless preset");
            }

            config.pass = speed + 1;
            config.near_lossless = quality;
            config.thread_level = 1;
            config.alpha_filtering = 2;
            config.use_sharp_yuv = 1;
            config.exact = 0;

            return AdvancedEncode(bmp, config, false);
        }

        #endregion

        #region | Another Public Functions |

        /// <summary>
        /// Get the libwebp version
        /// </summary>
        /// <returns>Version of library</returns>
        public string GetVersion()
        {
            try
            {
                uint v = (uint)UnsafeNativeMethods.WebPGetDecoderVersion();
                var revision = v % 256;
                var minor = (v >> 8) % 256;
                var major = (v >> 16) % 256;
                return major + "." + minor + "." + revision;
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message + "\r\nIn WebP.GetVersion"); 
            }
        }

        /// <summary>
        /// Get info of WEBP data
        /// </summary>
        /// <param name="rawWebP">The data of WebP</param>
        /// <param name="width">width of image</param>
        /// <param name="height">height of image</param>
        /// <param name="has_alpha">Image has alpha channel</param>
        /// <param name="has_animation">Image is a animation</param>
        /// <param name="format">Format of image: 0 = undefined (/mixed), 1 = lossy, 2 = lossless</param>
        public void GetInfo(byte[] rawWebP, out int width, out int height, out bool has_alpha, out bool has_animation, out string format)
        {
            VP8StatusCode result;
            GCHandle pinnedWebP = GCHandle.Alloc(rawWebP, GCHandleType.Pinned);

            try
            {
                IntPtr ptrRawWebP = pinnedWebP.AddrOfPinnedObject();

                WebPBitstreamFeatures features = new WebPBitstreamFeatures();
                result = UnsafeNativeMethods.WebPGetFeatures(ptrRawWebP, rawWebP.Length, ref features);

                if (result != 0)
                {
                    throw new Exception(result.ToString());
                }

                width = features.Width;
                height = features.Height;

                if (features.Has_alpha == 1) has_alpha = true; else has_alpha = false;
                if (features.Has_animation == 1) has_animation = true; else has_animation = false;

                switch (features.Format)
                {
                    case 1:
                        format = "lossy";
                        break;
                    case 2:
                        format = "lossless";
                        break;
                    default:
                        format = "undefined";
                        break;
                }
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message + "\r\nIn WebP.GetInfo"); 
            }
            finally
            {
                //Free memory
                if (pinnedWebP.IsAllocated)
                {
                    pinnedWebP.Free();
                }
            }
        }

        /// <summary>
        /// Compute PSNR, SSIM or LSIM distortion metric between two pictures. Warning: this function is rather CPU-intensive.
        /// </summary>
        /// <param name="source">Picture to measure</param>
        /// <param name="reference">Reference picture</param>
        /// <param name="metric_type">0 = PSNR, 1 = SSIM, 2 = LSIM</param>
        /// <returns>dB in the Y/U/V/Alpha/All order</returns>
        public float[] GetPictureDistortion(Bitmap source, Bitmap reference, int metric_type)
        {
            float[] result = new float[5];

            BitmapData sourceBmpData = null;
            BitmapData referenceBmpData = null;

            WebPPicture wpicSource = new WebPPicture();
            WebPPicture wpicReference = new WebPPicture();
            GCHandle pinnedResult = GCHandle.Alloc(result, GCHandleType.Pinned);

            try
            {
                if (source == null)
                {
                    throw new Exception("Source picture is void");
                }

                if (reference == null)
                {
                    throw new Exception("Reference picture is void");
                }

                if (metric_type > 2)
                {
                    throw new Exception("Bad metric_type. Use 0 = PSNR, 1 = SSIM, 2 = LSIM");
                }

                if (source.Width != reference.Width || source.Height != reference.Height)
                {
                    throw new Exception("Source and Reference pictures have diferent dimensions");
                }

                // Setup the source picture data, allocating the bitmap, width and height
                sourceBmpData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
                wpicSource = new WebPPicture();

                if (UnsafeNativeMethods.WebPPictureInitInternal(ref wpicSource) != 1)
                {
                    throw new Exception("Can´t init WebPPictureInit");
                }

                wpicSource.width = (int)source.Width;
                wpicSource.height = (int)source.Height;

                //Put the source bitmap componets in wpic
                if (sourceBmpData.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    wpicSource.use_argb = 1;
                    if (UnsafeNativeMethods.WebPPictureImportBGRA(ref wpicSource, sourceBmpData.Scan0, sourceBmpData.Stride) != 1)
                    {
                        throw new Exception("Can´t allocate memory in WebPPictureImportBGR");
                    }
                }
                else
                {
                    wpicSource.use_argb = 0;
                    if (UnsafeNativeMethods.WebPPictureImportBGR(ref wpicSource, sourceBmpData.Scan0, sourceBmpData.Stride) != 1)
                    {
                        throw new Exception("Can´t allocate memory in WebPPictureImportBGR");
                    }
                }

                // Setup the reference picture data, allocating the bitmap, width and height
                referenceBmpData = reference.LockBits(new Rectangle(0, 0, reference.Width, reference.Height), ImageLockMode.ReadOnly, reference.PixelFormat);
                wpicReference = new WebPPicture();

                if (UnsafeNativeMethods.WebPPictureInitInternal(ref wpicReference) != 1)
                {
                    throw new Exception("Can´t init WebPPictureInit");
                }

                wpicReference.width = (int)reference.Width;
                wpicReference.height = (int)reference.Height;
                wpicReference.use_argb = 1;

                //Put the source bitmap componets in wpic
                if (sourceBmpData.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    wpicSource.use_argb = 1;
                    if (UnsafeNativeMethods.WebPPictureImportBGRA(ref wpicReference, referenceBmpData.Scan0, referenceBmpData.Stride) != 1)
                    {
                        throw new Exception("Can´t allocate memory in WebPPictureImportBGR");
                    }
                }
                else
                {
                    wpicSource.use_argb = 0;
                    if (UnsafeNativeMethods.WebPPictureImportBGR(ref wpicReference, referenceBmpData.Scan0, referenceBmpData.Stride) != 1)
                    {
                        throw new Exception("Can´t allocate memory in WebPPictureImportBGR");
                    }
                }

                //Measure
                IntPtr ptrResult = pinnedResult.AddrOfPinnedObject();
                if (UnsafeNativeMethods.WebPPictureDistortion(ref wpicSource, ref wpicReference, metric_type, ptrResult) != 1)
                {
                    throw new Exception("Can´t measure.");
                }
                return result;
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message + "\r\nIn WebP.GetPictureDistortion"); 
            }
            finally
            {
                //Unlock the pixels
                if (sourceBmpData != null)
                {
                    source.UnlockBits(sourceBmpData);
                }

                if (referenceBmpData != null)
                {
                    reference.UnlockBits(referenceBmpData);
                }

                //Free memory
                if (wpicSource.argb != IntPtr.Zero)
                {
                    UnsafeNativeMethods.WebPPictureFree(ref wpicSource);
                }

                if (wpicReference.argb != IntPtr.Zero)
                {
                    UnsafeNativeMethods.WebPPictureFree(ref wpicReference);
                }

                //Free memory
                if (pinnedResult.IsAllocated)
                {
                    pinnedResult.Free();
                }
            }
        }

        #endregion

        #region | Private Methods |

        /// <summary>
        /// Encoding image using Advanced encoding API
        /// </summary>
        /// <param name="bmp">Bitmap with the image</param>
        /// <param name="config">Config for encode</param>
        /// <param name="info">True if need encode info.</param>
        /// <returns>Compressed data</returns>
        private byte[] AdvancedEncode(Bitmap bmp, WebPConfig config, bool info)
        {
            int dataWebpSize;

            byte[] rawWebP = null;
            byte[] dataWebp = null;

            BitmapData bmpData = null;

            WebPPicture wpic = new WebPPicture();
            WebPAuxStats stats = new WebPAuxStats();
            GCHandle pinnedArrayHandle = new GCHandle();

            IntPtr ptrStats = IntPtr.Zero;

            try
            {
                //Validate the config
                if (UnsafeNativeMethods.WebPValidateConfig(ref config) != 1)
                {
                    throw new Exception("Bad config parameters");
                }

                //test bmp
                if (bmp.Width == 0 || bmp.Height == 0)
                {
                    throw new ArgumentException("Bitmap contains no data.", "bmp");
                }

                if (bmp.Width > WEBP_MAX_DIMENSION || bmp.Height > WEBP_MAX_DIMENSION)
                {
                    throw new NotSupportedException("Bitmap's dimension is too large. Max is " + WEBP_MAX_DIMENSION + "x" + WEBP_MAX_DIMENSION + " pixels.");
                }

                PixelFormat pf = bmp.PixelFormat;

                if (pf != PixelFormat.Format24bppRgb && pf != PixelFormat.Format32bppArgb)
                {
                    pf = PixelFormat.Format24bppRgb;
                }

                // Setup the input data, allocating a the bitmap, width and height
                bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, pf);
                
                if (UnsafeNativeMethods.WebPPictureInitInternal(ref wpic) != 1)
                {
                    throw new Exception("Can´t init WebPPictureInit");
                }
                
                wpic.width = (int)bmp.Width;
                wpic.height = (int)bmp.Height;
                wpic.use_argb = 1;

                if (bmp.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    //Put the bitmap componets in wpic
                    int result = UnsafeNativeMethods.WebPPictureImportBGRA(ref wpic, bmpData.Scan0, bmpData.Stride);
                    
                    if (result != 1)
                    {
                        throw new Exception("Can´t allocate memory in WebPPictureImportBGRA");
                    }

                    wpic.colorspace =   (uint)WEBP_CSP_MODE.MODE_bgrA;
                    dataWebpSize =      bmp.Width * bmp.Height * 32;
                    dataWebp =          new byte[bmp.Width * bmp.Height * 32];      //Memory for WebP output
                }
                else
                {
                    //Put the bitmap componets in wpic
                    int result = UnsafeNativeMethods.WebPPictureImportBGR(ref wpic, bmpData.Scan0, bmpData.Stride);
                    
                    if (result != 1)
                    {
                        throw new Exception("Can´t allocate memory in WebPPictureImportBGR");
                    }
                    
                    dataWebpSize = bmp.Width * bmp.Height * 24;
                }

                //Set up statistics of compresion
                if (info)
                {
                    stats = new WebPAuxStats();
                    ptrStats = Marshal.AllocHGlobal(Marshal.SizeOf(stats));
                    Marshal.StructureToPtr(stats, ptrStats, false);
                    wpic.stats = ptrStats;
                }

                //Memory for WebP output
                if (dataWebpSize > 2147483591)
                {
                    dataWebpSize = 2147483591;
                }

                dataWebp = new byte[bmp.Width * bmp.Height * 32];
                pinnedArrayHandle = GCHandle.Alloc(dataWebp, GCHandleType.Pinned);
                IntPtr initPtr = pinnedArrayHandle.AddrOfPinnedObject();
                wpic.custom_ptr = initPtr;

                // Set up a byte-writing method (write-to-memory, in this case)
                UnsafeNativeMethods.OnCallback = new UnsafeNativeMethods.WebPMemoryWrite(MyWriter);
                wpic.writer = Marshal.GetFunctionPointerForDelegate(UnsafeNativeMethods.OnCallback);

                //compress the input samples
                if (UnsafeNativeMethods.WebPEncode(ref config, ref wpic) != 1)
                {
                    throw new Exception("Encoding error: " + ((WebPEncodingError)wpic.error_code).ToString());
                }

                //Remove OnCallback
                UnsafeNativeMethods.OnCallback = null;

                //Unlock the pixels
                bmp.UnlockBits(bmpData);
                bmpData = null;

                //Copy webpData to rawWebP
                int size = (int)((long)wpic.custom_ptr - (long)initPtr);
                rawWebP = new byte[size];
                Array.Copy(dataWebp, rawWebP, size);

                //Remove compression data
                pinnedArrayHandle.Free();
                dataWebp = null;

                //Show statistics
                if (info)
                {
                    stats = (WebPAuxStats)Marshal.PtrToStructure(ptrStats, typeof(WebPAuxStats));
                    MessageBox.Show("Dimension: " + wpic.width + " x " + wpic.height + " pixels\n" +
                                    "Output:    " + stats.coded_size + " bytes\n" +
                                    "PSNR Y:    " + stats.PSNRY + " db\n" +
                                    "PSNR u:    " + stats.PSNRU + " db\n" +
                                    "PSNR v:    " + stats.PSNRV + " db\n" +
                                    "PSNR ALL:  " + stats.PSNRALL + " db\n" +
                                    "Block intra4:  " + stats.block_count_intra4 + "\n" +
                                    "Block intra16: " + stats.block_count_intra16 + "\n" +
                                    "Block skipped: " + stats.block_count_skipped + "\n" +
                                    "Header size:    " + stats.header_bytes + " bytes\n" +
                                    "Mode-partition: " + stats.mode_partition_0 + " bytes\n" +
                                    "Macroblocks 0:  " + stats.segment_size_segments0 + " residuals bytes\n" +
                                    "Macroblocks 1:  " + stats.segment_size_segments1 + " residuals bytes\n" +
                                    "Macroblocks 2:  " + stats.segment_size_segments2 + " residuals bytes\n" +
                                    "Macroblocks 3:  " + stats.segment_size_segments3 + " residuals bytes\n" +
                                    "Quantizer   0:  " + stats.segment_quant_segments0 + " residuals bytes\n" +
                                    "Quantizer   1:  " + stats.segment_quant_segments1 + " residuals bytes\n" +
                                    "Quantizer   2:  " + stats.segment_quant_segments2 + " residuals bytes\n" +
                                    "Quantizer   3:  " + stats.segment_quant_segments3 + " residuals bytes\n" +
                                    "Filter level 0: " + stats.segment_level_segments0 + " residuals bytes\n" +
                                    "Filter level 1: " + stats.segment_level_segments1 + " residuals bytes\n" +
                                    "Filter level 2: " + stats.segment_level_segments2 + " residuals bytes\n" +
                                    "Filter level 3: " + stats.segment_level_segments3 + " residuals bytes\n", "Compression statistics");
                }

                return rawWebP;
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message + "\r\nIn WebP.AdvancedEncode"); 
            }
            finally
            {
                //Free temporal compress memory
                if (pinnedArrayHandle.IsAllocated)
                {
                    pinnedArrayHandle.Free();
                }

                //Free statistics memory
                if (ptrStats != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrStats);
                }

                //Unlock the pixels
                if (bmpData != null)
                {
                    bmp.UnlockBits(bmpData);
                }

                //Free memory
                if (wpic.argb != IntPtr.Zero)
                {
                    UnsafeNativeMethods.WebPPictureFree(ref wpic);
                }
            }
        }

        private int MyWriter([InAttribute()] IntPtr data, UIntPtr data_size, ref WebPPicture picture)
        {
            UnsafeNativeMethods.CopyMemory(picture.custom_ptr, data, (uint)data_size);
            picture.custom_ptr = new IntPtr(picture.custom_ptr.ToInt64() + (int)data_size);
            return 1;
        }


        #endregion


        public static implicit operator Bitmap(Webp webp)
        {
            return webp.Image;
        }

        public static implicit operator Webp(Bitmap bitmap)
        {
            return new Webp(bitmap);
        }

        public static implicit operator Image(Webp webp)
        {
            return webp.Image;
        }

        public static implicit operator Webp(Image bitmap)
        {
            return new Webp(bitmap);
        }


        public override ImgFormat GetImageFormat()
        {
            return Webp.ImageFormat;
        }

        public override string GetMimeType()
        {
            return Webp.MimeType;
        }

        /// <summary>
        /// Dispose of the image.
        /// </summary>
        public new void Clear()
        {
            if (Image != null)
                Image.Dispose();

            Image = null;
        }

        /// <summary>
        /// Free memory
        /// </summary>
        public new void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }
    }
}
