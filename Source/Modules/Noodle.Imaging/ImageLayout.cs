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
        public RectangleF Image { get; set; }
        public RectangleF ImageArea { get; set; }

        public ImageLayout(RectangleF image, RectangleF imageArea)
        {
            Image = image;
            ImageArea = imageArea;
        }
    }
}
