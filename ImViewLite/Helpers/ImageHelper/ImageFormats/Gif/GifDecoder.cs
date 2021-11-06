using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Helpers
{
    /// <summary>
    /// Allows the decoding of gifs into individual frames.
    /// </summary>
    public class GifDecoder
    {
        private readonly IMAGE image;
        private readonly byte[] times = new byte[4];


        /// <summary>
        /// Initializes a new instance of the <see cref="GifDecoder"/> class.
        /// </summary>
        /// <param name="image">The image to decode.</param>
        public GifDecoder(Gif image)
        {
            this.image = image;
            this.ActiveFrameIndex = 0;

            if (ImageAnimator.CanAnimate(image))
            {
                this.IsAnimated = true;
                this.FrameCount = image.Image.GetFrameCount(FrameDimension.Time);
                const int LoopCount = (int)ExifPropertyTag.LoopCount;
                const int FrameDelay = (int)ExifPropertyTag.FrameDelay;

                // Loop info is stored at byte 20737. Default to infinite loop if not found.
                this.LoopCount = Array.IndexOf(image.Image.PropertyIdList, LoopCount) != -1
                    ? BitConverter.ToInt16(image.Image.GetPropertyItem(LoopCount).Value, 0)
                    : 0;

                // Get the times stored in the gif. Default to 0 if not found.
                if (Array.IndexOf(this.image.Image.PropertyIdList, FrameDelay) != -1)
                {
                    this.times = this.image.Image.GetPropertyItem(FrameDelay).Value;
                }
            }
            else
            {
                this.FrameCount = 1;
            }
        }


        /// <summary>
        /// Gets the input image.
        /// </summary>
        public Image Image { get { return this.image.Image; } }

        /// <summary>
        /// Gets a value indicating whether the image is animated.
        /// </summary>
        public bool IsAnimated { get; }

        /// <summary>
        /// Gets the loop count.
        /// </summary>
        public int LoopCount { get; }

        /// <summary>
        /// Gets the frame count.
        /// </summary>
        public int FrameCount { get; }

        /// <summary>
        /// Gets the active frame index.
        /// </summary>
        public int ActiveFrameIndex { get; private set; }

        /// <summary>
        /// Gets the frame at the specified index.
        /// <remarks>
        /// Image frames are returned in <see cref="PixelFormat.Format32bppArgb"/> format to allow processing
        /// using the <see cref="Graphics"/> canvas.
        /// </remarks>
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The <see cref="GifFrame"/>.
        /// </returns>
        public GifFrame GetFrame(int index)
        {
            if (index > FrameCount - 1)
                return null;

            // Convert each 4-byte chunk into an integer.
            // GDI returns a single array with all delays, while Mono returns a different array for each frame.
            TimeSpan delay = TimeSpan.FromMilliseconds(BitConverter.ToInt32(this.times, (4 * index) % this.times.Length) * 10);

            // Find the frame
            this.image.Image.SelectActiveFrame(FrameDimension.Time, index);

            GifFrame frame = new GifFrame(this.image, delay);

            // Reset the image
            this.image.Image.SelectActiveFrame(FrameDimension.Time, 0);
            this.ActiveFrameIndex = 0;

            return frame;
        }


        /// <summary>
        /// Sets the active frame at the specific index.
        /// </summary>
        /// <param name="index">The index of the frame.</param>
        public void SetFrame(int index)
        {
            if (index > FrameCount - 1 || index < 0)
                return;

            // Convert each 4-byte chunk into an integer.
            // GDI returns a single array with all delays, while Mono returns a different array for each frame.
            TimeSpan delay = TimeSpan.FromMilliseconds(BitConverter.ToInt32(this.times, (4 * index) % this.times.Length) * 10);

            // Find the frame
            this.image.Image.SelectActiveFrame(FrameDimension.Time, index);
            this.ActiveFrameIndex = index;
        }
    }
}
