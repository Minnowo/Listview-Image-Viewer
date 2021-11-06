using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Helpers
{
    public static class GraphicsExtensions
    {

        /// <summary>
        /// A wrapper for <see cref="Graphics.DrawRectangle(Pen, Rectangle)"/> which offsets the rectangle width and height if the pen width is 1.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object.</param>
        /// <param name="pen">The <see cref="Pen"/> to draw with.</param>
        /// <param name="rect">The <see cref="Rectangle"/> to draw.</param>
        public static void DrawRectangleProper(this Graphics g, Pen pen, Rectangle rect)
        {
            if (pen.Width == 1)
            {
                rect = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }

            if (rect.Width > 0 && rect.Height > 0)
            {
                g.DrawRectangle(pen, rect);
            }
        }
    }
}
