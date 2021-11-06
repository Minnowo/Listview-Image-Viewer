using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ImViewLite.Helpers
{
    public struct ResizeImage
    {
        public Size NewSize;

        public InterpolationMode InterpolationMode;
        public GraphicsUnit GraphicsUnit;
        public SmoothingMode SmoothingMode;
        public CompositingMode CompositingMode;
        public CompositingQuality CompositingQuality;
        public PixelOffsetMode PixelOffsetMode;

        public ResizeImage(Size newSize)
        {
            NewSize = newSize;
            InterpolationMode = InterpolationMode.NearestNeighbor;
            GraphicsUnit = GraphicsUnit.Pixel;
            SmoothingMode = SmoothingMode.None;
            CompositingMode = CompositingMode.SourceOver;
            CompositingQuality = CompositingQuality.HighSpeed;
            PixelOffsetMode = PixelOffsetMode.None;
        }
    }
}
