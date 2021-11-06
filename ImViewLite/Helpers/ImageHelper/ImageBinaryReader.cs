using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ImViewLite.Helpers
{
    public static class ImageBinaryReader
    {
        const byte MAX_MAGIC_BYTE_LENGTH = 8;


        public static readonly Dictionary<byte[], ImgFormat> Image_Byte_Identifiers = new Dictionary<byte[], ImgFormat>()
        {
            { PNG.IdentifierBytes_1, ImgFormat.png },
            { JPEG.IdentifierBytes_1, ImgFormat.jpg },
            { Webp.IdentifierBytes_1, ImgFormat.webp },
            { Gif.IdentifierBytes_1, ImgFormat.gif },
            { Gif.IdentifierBytes_2, ImgFormat.gif },
            { TIFF.IdentifierBytes_2, ImgFormat.tif },
            { TIFF.IdentifierBytes_1, ImgFormat.tif },
            { BMP.IdentifierBytes_1, ImgFormat.bmp },
            { ICO.IdentifierBytes_1, ImgFormat.ico },
            { WORM.IdentifierBytes_1, ImgFormat.wrm },
            { WORM.IdentifierBytes_2, ImgFormat.wrm }
        };

        static readonly Dictionary<byte[], Func<BinaryReader, Size>> Image_Format_Decoders = new Dictionary<byte[], Func<BinaryReader, Size>>()
        {
            { BMP.IdentifierBytes_1, DecodeBitmap },
            { Gif.IdentifierBytes_1, DecodeGif },
            { Gif.IdentifierBytes_2, DecodeGif },
            { PNG.IdentifierBytes_1, DecodePng },
            { JPEG.IdentifierBytes_1, DecodeJfif },
            { Webp.IdentifierBytes_1, DecodeWebP },
            { TIFF.IdentifierBytes_1,  DecodeTiffLE },
            { TIFF.IdentifierBytes_2,  DecodeTiffBE },
            { WORM.IdentifierBytes_1, DecodeWORM },
            { WORM.IdentifierBytes_2, DecodeDWORM }
        };



        #region Get / Read Image Format 

        /// <summary>
        /// Gets the image mime type from the leading byte identifiers.
        /// </summary>
        /// <param name="path">The path to the image file.</param>
        /// <returns>The mime type as a string.</returns>
        public static string GetMimeType(string path)
        {
            switch (GetImageFormat(path))
            {
                case ImgFormat.png:
                    return PNG.MimeType;
                case ImgFormat.jpg:
                    return JPEG.MimeType;
                case ImgFormat.tif:
                    return TIFF.MimeType;
                case ImgFormat.bmp:
                    return BMP.MimeType;
                case ImgFormat.gif:
                    return Gif.MimeType;
                case ImgFormat.webp:
                    return Webp.MimeType;
                case ImgFormat.wrm:
                    return WORM.MimeType;
                case ImgFormat.ico:
                    return ICO.MimeType;
            }
            return IMAGE.MimeType;
        }

        /// <summary>
        /// Gets the image format from the leading byte identifiers.
        /// </summary>
        /// <param name="path">The path to the image file.</param>
        /// <returns>The image format.</returns>
        public static ImgFormat GetImageFormat(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return ImgFormat.nil;


            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    byte[] magicBytes = new byte[MAX_MAGIC_BYTE_LENGTH];

                    for (int i = 0; i < MAX_MAGIC_BYTE_LENGTH; i += 1)
                    {
                        magicBytes[i] = binaryReader.ReadByte();

                        foreach (KeyValuePair<byte[], ImgFormat> kvPair in Image_Byte_Identifiers)
                        {
                            if (ByteHelper.StartsWith(magicBytes, kvPair.Key))
                            {
                                return kvPair.Value;
                            }
                        }
                    }
                    return ImgFormat.nil;
                }
            }
            catch
            {
                return ImgFormat.nil;
            }
        }


        #endregion


        #region Get / Read Image Dimensions 


        /// <summary>        
        /// Gets the dimensions of an image.        
        /// </summary>        
        /// <param name="path">The path of the image to get the dimensions of.</param>        
        /// <returns>The dimensions of the specified image.</returns>        
        /// <exception cref="ArgumentException">The image was of an unrecognised format.</exception>            
        public static Size GetDimensions(BinaryReader binaryReader)
        {
            byte[] magicBytes = new byte[MAX_MAGIC_BYTE_LENGTH];

            for (int i = 0; i < MAX_MAGIC_BYTE_LENGTH; i += 1)
            {
                magicBytes[i] = binaryReader.ReadByte();

                foreach (KeyValuePair<byte[], Func<BinaryReader, Size>> kvPair in Image_Format_Decoders)
                {
                    if (ByteHelper.StartsWith(magicBytes, kvPair.Key))
                    {
                        return kvPair.Value(binaryReader);
                    }
                }
            }

            return Size.Empty;
        }

        /// <summary>
        /// Gets the dimensions of an image.
        /// </summary>
        /// <param name="path">The path of the image to get the dimensions of.</param>
        /// <returns>The dimensions of the specified image.</returns>
        /// <exception cref="ArgumentException">The image was of an unrecognized format.</exception>
        public static Size GetDimensions(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return Size.Empty;

            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    return GetDimensions(binaryReader);
                }
            }
            catch
            {
                return Size.Empty;
            }
        }

        private static Size DecodeWORM(BinaryReader binaryReader)
        {
            return WORM.GetDimensions(binaryReader, false);
        }

        private static Size DecodeDWORM(BinaryReader binaryReader)
        {
            return WORM.GetDimensions(binaryReader, false);
        }

        private static Size DecodeTiffLE(BinaryReader binaryReader)
        {
            if (binaryReader.ReadByte() != 0)
                return Size.Empty;

            int idfStart = ByteHelper.ReadInt32LE(binaryReader);

            binaryReader.BaseStream.Seek(idfStart, SeekOrigin.Begin);

            int numberOfIDF = ByteHelper.ReadInt16LE(binaryReader);

            int width = -1;
            int height = -1;
            for (int i = 0; i < numberOfIDF; i++)
            {
                short field = ByteHelper.ReadInt16LE(binaryReader);

                switch (field)
                {
                    // https://www.awaresystems.be/imaging/tiff/tifftags/baseline.html
                    default:
                        binaryReader.ReadBytes(10);
                        break;
                    case 256: // image width
                        binaryReader.ReadBytes(6);
                        width = ByteHelper.ReadInt32LE(binaryReader);
                        break;
                    case 257: // image length
                        binaryReader.ReadBytes(6);
                        height = ByteHelper.ReadInt32LE(binaryReader);
                        break;
                }
                if (width != -1 && height != -1)
                    return new Size(width, height);
            }
            return Size.Empty;
        }

        private static Size DecodeTiffBE(BinaryReader binaryReader)
        {
            int idfStart = ByteHelper.ReadInt32BE(binaryReader);

            binaryReader.BaseStream.Seek(idfStart, SeekOrigin.Begin);

            int numberOfIDF = ByteHelper.ReadInt16BE(binaryReader);

            int width = -1;
            int height = -1;
            for (int i = 0; i < numberOfIDF; i++)
            {
                short field = ByteHelper.ReadInt16BE(binaryReader);

                switch (field)
                {
                    // https://www.awaresystems.be/imaging/tiff/tifftags/baseline.html
                    default:
                        binaryReader.ReadBytes(10);
                        break;
                    case 256: // image width
                        binaryReader.ReadBytes(6);
                        width = ByteHelper.ReadInt32BE(binaryReader);
                        break;
                    case 257: // image length
                        binaryReader.ReadBytes(6);
                        height = ByteHelper.ReadInt32BE(binaryReader);
                        break;
                }
                if (width != -1 && height != -1)
                    return new Size(width, height);
            }
            return Size.Empty;
        }

        private static Size DecodeBitmap(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(16);
            int width = binaryReader.ReadInt32();
            int height = binaryReader.ReadInt32();
            return new Size(width, height);
        }

        private static Size DecodeGif(BinaryReader binaryReader)
        {
            int width = binaryReader.ReadInt16();
            int height = binaryReader.ReadInt16();
            return new Size(width, height);
        }

        private static Size DecodePng(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(8);
            int width = ByteHelper.ReadInt32BE(binaryReader);
            int height = ByteHelper.ReadInt32BE(binaryReader);
            return new Size(width, height);
        }

        private static Size DecodeJfif(BinaryReader binaryReader)
        {
            while (binaryReader.ReadByte() == 0xff)
            {
                byte marker = binaryReader.ReadByte();
                short chunkLength = ByteHelper.ReadInt16BE(binaryReader);
                if (marker == 0xc0 || marker == 0xc2) // c2: progressive
                {
                    binaryReader.ReadByte();
                    int height = ByteHelper.ReadInt16BE(binaryReader);
                    int width = ByteHelper.ReadInt16BE(binaryReader);
                    return new Size(width, height);
                }

                if (chunkLength < 0)
                {
                    ushort uchunkLength = (ushort)chunkLength;
                    binaryReader.ReadBytes(uchunkLength - 2);
                }
                else
                {
                    binaryReader.ReadBytes(chunkLength - 2);
                }
            }

            return Size.Empty;
        }

        private static Size DecodeWebP(BinaryReader binaryReader)
        {
            // 'RIFF' already read   
            binaryReader.ReadBytes(4);

            if (ByteHelper.ReadInt32LE(binaryReader) != 1346520407)// 1346520407 : 'WEBP'
                return Size.Empty;

            switch (ByteHelper.ReadInt32LE(binaryReader))
            {
                case 540561494: // 'VP8 ' : lossy
                                // skip stuff we don't need
                    binaryReader.ReadBytes(7);

                    if (ByteHelper.ReadInt24LE(binaryReader) != 2752925) // invalid webp file
                        return Size.Empty;

                    return new Size(ByteHelper.ReadInt16LE(binaryReader), ByteHelper.ReadInt16LE(binaryReader));

                case 1278758998:// 'VP8L' : lossless
                                // skip stuff we don't need
                    binaryReader.ReadBytes(4);

                    if (binaryReader.ReadByte() != 47)// 0x2f : 47 1 byte signature
                        return Size.Empty;

                    byte[] b = binaryReader.ReadBytes(4);

                    return new Size(
                        1 + (((b[1] & 0x3F) << 8) | b[0]),
                        1 + ((b[3] << 10) | (b[2] << 2) | ((b[1] & 0xC0) >> 6)));
                // if something breaks put in the '& 0xF' but & oxf should do nothing in theory
                // because inclusive & with 1111 will leave the binary untouched
                //  1 + (((wh[3] & 0xF) << 10) | (wh[2] << 2) | ((wh[1] & 0xC0) >> 6))

                case 1480085590:// 'VP8X' : extended
                                // skip stuff we don't need
                    binaryReader.ReadBytes(8);
                    return new Size(1 + ByteHelper.ReadInt24LE(binaryReader), 1 + ByteHelper.ReadInt24LE(binaryReader));
            }

            return Size.Empty;
        }

        #endregion


    }
}
