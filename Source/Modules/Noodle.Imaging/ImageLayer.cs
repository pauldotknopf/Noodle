using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Noodle.Imaging
{
    class ImageLayer
    {
        public delegate Size CalculateLayerContentSize(double maxwidth, double maxheight);
        /// <summary>
        /// Returns a rectangle with canvas-relative coordinates. A callback is required to calculate the actual size of the content based on the specified bounds.
        /// The callback may be passed double.NaN for one or more paramters to indicate that they are not specified.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="actualSizeCalculator">The actual size calculator.</param>
        /// <param name="forceInsideCanvas">if set to <c>true</c> [force inside canvas].</param>
        /// <returns></returns>
        public RectangleF CalculateLayerCoordinates(ImageState s, CalculateLayerContentSize actualSizeCalculator, bool forceInsideCanvas)
        {
            //Find container 
            RectangleF cont;
            if (s.layout.ContainsRing(RelativeTo))
                cont = PolygonMath.GetBoundingBox(s.layout[RelativeTo]);
            else if ("canvas".Equals(RelativeTo, StringComparison.OrdinalIgnoreCase))
                cont = new RectangleF(new PointF(), s.destSize);
            else
                cont = PolygonMath.GetBoundingBox(s.layout["image"]);

            //Calculate layer coords
            RectangleF rect = new RectangleF();

            //Resolve all values to the same coordinate plane, null values will be transformed to NaN
            double left = Resolve(Left, cont.X, cont.Width, false);
            double top = Resolve(Top, cont.Y, cont.Height, false);
            double right = Resolve(Right, cont.Right, cont.Width, true);
            double bottom = Resolve(Bottom, cont.Bottom, cont.Height, true);
            double width = Resolve(Width, 0, cont.Width, false);
            double height = Resolve(Height, 0, cont.Height, false);

            //Force all values to be within the canvas area.
            if (forceInsideCanvas)
            {
                SizeF canvas = s.destSize;
                if (!double.IsNaN(left)) left = Math.Min(Math.Max(0, left), canvas.Width);
                if (!double.IsNaN(right)) right = Math.Min(Math.Max(0, right), canvas.Width);
                if (!double.IsNaN(width)) width = Math.Min(Math.Max(0, width), canvas.Width);
                if (!double.IsNaN(bottom)) bottom = Math.Min(Math.Max(0, bottom), canvas.Height);
                if (!double.IsNaN(top)) top = Math.Min(Math.Max(0, top), canvas.Height);
                if (!double.IsNaN(height)) height = Math.Min(Math.Max(0, height), canvas.Height);
            }

            //If right and left (or top and bottom) are inverted, avg them and set them equal.
            if (!double.IsNaN(left) && !double.IsNaN(right) && right < left) left = right = ((left + right) / 2);
            if (!double.IsNaN(top) && !double.IsNaN(bottom) && bottom < top) bottom = top = ((bottom + top) / 2);


            //Fill in width/height if enough stuff is specified
            if (!double.IsNaN(left) && !double.IsNaN(right) && double.IsNaN(width)) width = Math.Max(right - left, 0);
            if (!double.IsNaN(top) && !double.IsNaN(bottom) && double.IsNaN(height)) height = Math.Max(bottom - top, 0);


            //Execute the callback to get the actual size. Update the width and height values if the actual size is smaller. 
            SizeF normalSize = actualSizeCalculator((double.IsNaN(width) && Fill) ? cont.Width : width, (double.IsNaN(height) && Fill) ? cont.Height : height);
            if (double.IsNaN(width) || width > normalSize.Width) width = normalSize.Width;
            if (double.IsNaN(height) || height > normalSize.Height) height = normalSize.Height;



            //If only width and height are specified, set the other values to match the container, and let alignment sort it out.
            if (double.IsNaN(left) && double.IsNaN(right)) { left = cont.X; right = cont.Right; }//Handle situations where neither left nor right is specified, pretend left=0
            if (double.IsNaN(top) && double.IsNaN(bottom)) { top = cont.X; bottom = cont.Bottom; } //Handle situations where neither top nor bottom is specified, pretend top=0


            //When all 3 values are specified in either direction, we must use the alignment setting to determine which direction to snap to.
            if (!double.IsNaN(left) && !double.IsNaN(right) && !double.IsNaN(width))
            {
                if (width > right - left) width = right - left; //Use the smaller value in this case, no need to align.
                else
                {
                    if (Align == ContentAlignment.BottomLeft || Align == ContentAlignment.MiddleLeft || Align == ContentAlignment.TopLeft)
                        right = left + width;
                    if (Align == ContentAlignment.BottomCenter || Align == ContentAlignment.MiddleCenter || Align == ContentAlignment.TopCenter)
                    {
                        left += (right - left - width) / 2;
                        right = left + width;
                    }
                    if (Align == ContentAlignment.BottomRight || Align == ContentAlignment.MiddleRight || Align == ContentAlignment.TopRight)
                        left = right - width;
                }
            }

            //When all 3 values are specified in either direction, we must use the alignment setting to determine which direction to snap to.
            if (!double.IsNaN(top) && !double.IsNaN(bottom) && !double.IsNaN(height))
            {
                if (height > bottom - top) height = bottom - top; //Use the smaller value in this case, no need to align.
                else
                {
                    if (Align == ContentAlignment.TopLeft || Align == ContentAlignment.TopCenter || Align == ContentAlignment.TopRight)
                        bottom = top + height;
                    if (Align == ContentAlignment.MiddleLeft || Align == ContentAlignment.MiddleCenter || Align == ContentAlignment.MiddleRight)
                    {
                        top += (bottom - top - height) / 2;
                        bottom = top + height;
                    }
                    if (Align == ContentAlignment.BottomLeft || Align == ContentAlignment.BottomCenter || Align == ContentAlignment.BottomRight)
                        top = bottom - height;
                }
            }


            //Calculate values for top and left based off bottom and right
            if (double.IsNaN(left)) left = right - width;
            if (double.IsNaN(top)) top = bottom - height;

            //Calculate values for bottom and right based off top and left
            if (double.IsNaN(right)) right = left + width;
            if (double.IsNaN(bottom)) bottom = top + height;


            return new RectangleF((float)left, (float)top, (float)width, (float)height);

        }
    }

}
