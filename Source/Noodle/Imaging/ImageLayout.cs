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
        /// Ctor
        /// </summary>
        /// <param name="image"></param>
        /// <param name="canvasSize"></param>
        public ImageLayout(RectangleF image, SizeF canvasSize)
        {
            Image = image;
            CanvasSize = canvasSize;
        }

        /// <summary>
        /// Relative to the canvas, this is where the image will be drawn
        /// </summary>
        public RectangleF Image { get; set; }

        /// <summary>
        /// This is the size of the canvas (width/height).
        /// Origin (0,0) is implied.
        /// </summary>
        public SizeF CanvasSize { get; set; }
    }
}
