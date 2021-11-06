using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ImViewLite.Helpers
{
    public static class ByteHelperExtensions
    {
        /// <summary>
        /// Checks if param 2 starts with param 1.
        /// </summary>
        /// <returns>True if param 2 starts with param 1.</returns>
        public static bool StartsWith(this byte[] thisBytes, byte[] thatBytes)
        {
            return ByteHelper.StartsWith(thatBytes, thisBytes);
        }
    }

    public static class ByteHelper
    {
        /// <summary>
        /// Checks if param 2 starts with param 1.
        /// </summary>
        /// <returns>True if param 2 starts with param 1.</returns>
        public static bool StartsWith(byte[] thatBytes, byte[] thisBytes)
        {
            for (int i = 0; i < thisBytes.Length; i += 1)
                if (thatBytes[i] != thisBytes[i])
                    return false;

            return true;
        }




        #region Little Endian

        /// <summary>
        /// Reads a 32bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="binaryReader">The stream to read the data from.</param>
        /// <returns>The unsigned 32bit integer cast to an <c>Int32</c>.</returns>
        public static int ReadInt32LE(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(4);
            return ((bytes[0]) | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24));
        }

        /// <summary>
        /// Reads a 32bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>The unsigned 32bit integer cast to an <c>Int32</c>.</returns>
        public static int ReadInt32LE(Stream stream)
        {
            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, bytes.Length);
            return ((bytes[0]) | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24));
        }

        /// <summary>
        /// Reads a 32bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <returns>The unsigned 32bit integer cast to an <c>Int32</c>.</returns>
        public static int ReadInt32LE(byte[] bytes)
        {
            return ((bytes[0]) | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24));
        }

        /// <summary>
        /// Reads a 32bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <param name="offset">The offset to read from the buffer.</param>
        /// <returns>The unsigned 32bit integer cast to an <c>Int32</c>.</returns>
        public static int ReadInt32LE(byte[] bytes, long offset)
        {
            return ((bytes[offset]) | (bytes[offset + 1] << 8) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 24));
        }



        /// <summary>
        /// Reads a 24bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="binaryReader">The stream to read the data from.</param>
        /// <returns>The unsigned 24bit integer cast to an <c>Int24</c>.</returns>
        public static int ReadInt24LE(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(3);
            return ((bytes[0]) | (bytes[1] << 8) | (bytes[2] << 16));
        }

        /// <summary>
        /// Reads a 24bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>The unsigned 24bit integer cast to an <c>Int24</c>.</returns>
        public static int ReadInt24LE(Stream stream)
        {
            byte[] bytes = new byte[3];
            stream.Read(bytes, 0, bytes.Length);
            return ((bytes[0]) | (bytes[1] << 8) | (bytes[2] << 16));
        }

        /// <summary>
        /// Reads a 24bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <returns>The unsigned 24bit integer cast to an <c>Int24</c>.</returns>
        public static int ReadInt24LE(byte[] bytes)
        {
            if (bytes.Length < 3)
                return -1;
            return ((bytes[0]) | (bytes[1] << 8) | (bytes[2] << 16));
        }

        /// <summary>
        /// Reads a 24bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <param name="offset">The offset to read from the buffer.</param>
        /// <returns>The unsigned 24bit integer cast to an <c>Int24</c>.</returns>
        public static int ReadInt24LE(byte[] bytes, long offset)
        {
            if (bytes.Length < 3)
                return -1;
            return ((bytes[offset]) | (bytes[offset + 1] << 8) | (bytes[offset + 2] << 16));
        }



        /// <summary>
        /// Reads a 16bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="binaryReader">The stream to read the data from.</param>
        /// <returns>The unsigned 16bit integer cast to an <c>Int16</c>.</returns>
        public static short ReadInt16LE(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(2);
            return (short)((bytes[0]) | (bytes[1] << 8));
        }

        /// <summary>
        /// Reads a 16bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>The unsigned 16bit integer cast to an <c>Int16</c>.</returns>
        public static short ReadInt16LE(Stream stream)
        {
            byte[] bytes = new byte[2];
            stream.Read(bytes, 0, bytes.Length);
            return (short)((bytes[0]) | (bytes[1] << 8));
        }

        /// <summary>
        /// Reads a 16bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <returns>The unsigned 16bit integer cast to an <c>Int16</c>.</returns>
        public static short ReadInt16LE(byte[] bytes)
        {
            return (short)((bytes[0]) | (bytes[1] << 8));
        }

        /// <summary>
        /// Reads a 16bit unsigned integer in little-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <param name="offset">The offset to read from the buffer.</param>
        /// <returns>The unsigned 16bit integer cast to an <c>Int16</c>.</returns>
        public static short ReadInt16LE(byte[] bytes, long offset)
        {
            return (short)((bytes[offset]) | (bytes[offset + 1] << 8));
        }


        #endregion

        #region Big Endian

        /// <summary>
        /// Reads a 32bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>The unsigned 32bit integer cast to an <c>Int32</c>.</returns>
        public static int ReadInt32BE(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(4);
            return ((bytes[3]) | (bytes[2] << 8) | (bytes[1] << 16) | (bytes[0] << 24));
        }

        /// <summary>
        /// Reads a 32bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>The unsigned 32bit integer cast to an <c>Int32</c>.</returns>
        public static int ReadInt32BE(Stream stream)
        {
            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, bytes.Length);
            return ((bytes[3]) | (bytes[2] << 8) | (bytes[1] << 16) | (bytes[0] << 24));
        }

        /// <summary>
        /// Reads a 32bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <returns>The unsigned 32bit integer cast to an <c>Int32</c>.</returns>
        public static int ReadInt32BE(byte[] bytes)
        {
            return ((bytes[3]) | (bytes[2] << 8) | (bytes[1] << 16) | (bytes[0] << 24));
        }

        /// <summary>
        /// Reads a 32bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <param name="offset">The offset to read from the buffer.</param>
        /// <returns>The unsigned 32bit integer cast to an <c>Int32</c>.</returns>
        public static int ReadInt32BE(byte[] bytes, long offset)
        {
            return ((bytes[offset + 3]) | (bytes[offset + 2] << 8) | (bytes[offset + 1] << 16) | (bytes[offset] << 24));
        }



        /// <summary>
        /// Reads a 24bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="binaryReader">The stream to read the data from.</param>
        /// <returns>The unsigned 24bit integer cast to an <c>Int24</c>.</returns>
        public static int ReadInt24BE(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(3);
            return ((bytes[2]) | (bytes[1] << 8) | (bytes[0] << 16));
        }

        /// <summary>
        /// Reads a 24bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>The unsigned 24bit integer cast to an <c>Int24</c>.</returns>
        public static int ReadInt24BE(Stream stream)
        {
            byte[] bytes = new byte[3];
            stream.Read(bytes, 0, bytes.Length);
            return ((bytes[2]) | (bytes[1] << 8) | (bytes[0] << 16));
        }

        /// <summary>
        /// Reads a 24bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <returns>The unsigned 24bit integer cast to an <c>Int24</c>.</returns>
        public static int ReadInt24BE(byte[] bytes)
        {
            return ((bytes[2]) | (bytes[1] << 8) | (bytes[0] << 16));
        }

        /// <summary>
        /// Reads a 24bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <param name="offset">The offset to read from the buffer.</param>
        /// <returns>The unsigned 24bit integer cast to an <c>Int24</c>.</returns>
        public static int ReadInt24BE(byte[] bytes, long offset)
        {
            return ((bytes[offset + 2]) | (bytes[offset + 1] << 8) | (bytes[offset] << 16));
        }



        /// <summary>
        /// Reads a 16bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="binaryReader">The stream to read the data from.</param>
        /// <returns>The unsigned 16bit integer cast to an <c>Int16</c>.</returns>
        public static short ReadInt16BE(BinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(2);
            return (short)((bytes[1]) | (bytes[0] << 8));
        }

        /// <summary>
        /// Reads a 16bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>The unsigned 16bit integer cast to an <c>Int16</c>.</returns>
        public static short ReadInt16BE(Stream stream)
        {
            byte[] bytes = new byte[2];
            stream.Read(bytes, 0, bytes.Length);
            return (short)((bytes[1]) | (bytes[0] << 8));
        }

        /// <summary>
        /// Reads a 16bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <returns>The unsigned 16bit integer cast to an <c>Int16</c>.</returns>
        public static short ReadInt16BE(byte[] bytes)
        {
            return (short)((bytes[1]) | (bytes[0] << 8));
        }

        /// <summary>
        /// Reads a 16bit unsigned integer in big-endian format.
        /// </summary>
        /// <param name="bytes">The buffer to read the data from.</param>
        /// <param name="offset">The offset to read from the buffer.</param>
        /// <returns>The unsigned 16bit integer cast to an <c>Int16</c>.</returns>
        public static short ReadInt16BE(byte[] bytes, long offset)
        {
            return (short)((bytes[offset + 1]) | (bytes[offset] << 8));
        }

        #endregion
    }
}
