using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Noodle.Imaging
{
    class PolygonMath
    {
        /// <summary>
        /// Used for converting custom crop rectangle coordinates into a valid cropping rectangle. Positive values are relative to 0,0, negative values relative to width, height.
        /// X2 and Y2 values of 0 become width and height respectively. 
        /// </summary>
        /// <param name="cropValues">An array of 4 elements defining x1, y1, x2, and y2 of the cropping rectangle</param>
        /// <param name="xunits">The width x1 and x2 are relative to</param>
        /// <param name="yunits">The height y1 and y2 are relative to</param>
        /// <param name="imageSize">The size of the uncropped image</param>
        /// <returns></returns>
        public static RectangleF GetCroppingRectangle(double[] cropValues, double xunits, double yunits, SizeF imageSize)
        {
            var defValue = new RectangleF(new PointF(0, 0), imageSize);
            var c = cropValues;

            //Step 2, Apply units to values, resolving against imageSize
            for (int i = 0; i < c.Length; i++)
            {
                bool xvalue = i % 2 == 0;
                if (xvalue && xunits != 0) c[i] *= (imageSize.Width / xunits);
                if (!xvalue && xunits != 0) c[i] *= (imageSize.Height / yunits);

                //Prohibit values larger than imageSize
                if (xvalue && c[i] > imageSize.Width) c[i] = imageSize.Width;
                if (!xvalue && c[i] > imageSize.Height) c[i] = imageSize.Height;
            }

            //Step 3, expand width/height crop to 4-value crop (not currently used)
            if (c.Length == 2)
            {
                if (c[0] < 1 || c[1] < 1) return defValue; //We can't do anything with negative values here
                //Center horizontally and vertically.
                double x = (imageSize.Width - c[0]) / 2;
                double y = (imageSize.Height - c[1]) / 2;

                c = new double[] { x, y, x + c[0], y + c[1] };
            }

            double x1 = c[0], y1 = c[1], x2 = c[2], y2 = c[3];

            //allow negative offsets 
            if (x1 < 0) x1 += imageSize.Width;
            if (y1 < 0) y1 += imageSize.Height;
            if (x2 <= 0) x2 += imageSize.Width;
            if (y2 <= 0) y2 += imageSize.Height;


            //Require box stay in bounds.
            if (x1 < 0) x1 = 0; if (x2 < 0) x2 = 0;
            if (y1 < 0) y1 = 0; if (y2 < 0) y2 = 0;
            if (x1 > imageSize.Width) x1 = imageSize.Width;
            if (x2 > imageSize.Width) x2 = imageSize.Width;
            if (y1 > imageSize.Height) y1 = imageSize.Height;
            if (y2 > imageSize.Height) y2 = imageSize.Height;

            //Require positive width and height.
            if (x2 <= x1 || y2 <= y1)
            {
                //Use original dimensions - can't recover from negative width or height in cropping rectangle
                return new RectangleF(new PointF(0, 0), imageSize);
            }

            return new RectangleF((float)x1, (float)y1, (float)(x2 - x1), (float)(y2 - y1));
        }
    }
}
