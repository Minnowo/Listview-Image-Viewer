using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Helpers
{
    /// <summary>
    /// Provides enumeration for the available bit depths.
    /// </summary>
    public enum BitDepth : long
    {
        /// <summary>
        /// 1 bit per pixel
        /// </summary>
        Bit1 = 1L,

        /// <summary>
        /// 4 bits per pixel
        /// </summary>
        Bit4 = 4L,

        /// <summary>
        /// 8 bits per pixel
        /// </summary>
        Bit8 = 8L,

        /// <summary>
        /// 16 bits per pixel
        /// </summary>
        Bit16 = 16L,

        /// <summary>
        /// 24 bits per pixel
        /// </summary>
        Bit24 = 24L,

        /// <summary>
        /// 32 bits per pixel
        /// </summary>
        Bit32 = 32L
    }
}
