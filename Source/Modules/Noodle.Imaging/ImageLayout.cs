using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Noodle.Imaging
{
    /// <summary>
    /// The layout of the image relative to its canvas
    /// </summary>
    public class ImageLayout
    {
        /// <summary>
        /// Create an image layout with a canvas size and the destination location of the source image
        /// </summary>
        /// <param name="canvasSize"></param>
        /// <param name="drawTo"></param>
        public ImageLayout(SizeF canvasSize, RectangleF drawTo)
        {
            CanvasSize = canvasSize;
            DrawTo = drawTo;
        }

        /// <summary>
        /// The size of the canvas that the source image will be drawn on
        /// </summary>
        public SizeF CanvasSize { get; protected set; }

        /// <summary>
        /// The X/Y/width/height that the source image will be drawn to 
        /// </summary>
        public RectangleF DrawTo { get; protected set; }
    }
}
