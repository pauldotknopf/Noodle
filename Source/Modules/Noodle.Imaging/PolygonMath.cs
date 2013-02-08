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

        /// <summary>
        /// Rounds a floating-point rectangle to an integer rectangle using System.Round
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Rectangle ToRectangle(RectangleF r)
        {
            return new Rectangle((int)Math.Round(r.X), (int)Math.Round(r.Y), (int)Math.Round(r.Width), (int)Math.Round(r.Height));
        }

        /// <summary>
        /// Scales 'inner' to fit inside 'bounding' while maintaining aspect ratio. Upscales and downscales.
        /// </summary>
        /// <param name="inner"></param>
        /// <param name="bounding"></param>
        /// <returns></returns>
        public static SizeF ScaleInside(SizeF inner, SizeF bounding)
        {
            double innerRatio = inner.Width / inner.Height;
            double outerRatio = bounding.Width / bounding.Height;

            if (outerRatio > innerRatio)
            {
                //Width is wider - so bound by height.
                return new SizeF((float)(innerRatio * bounding.Height), (float)(bounding.Height));
            }
            else
            {
                //Height is higher, or aspect ratios are identical.
                return new SizeF((float)(bounding.Width), (float)(bounding.Width / innerRatio));
            }
        }

        /// <summary>
        /// Rounds the elements of the specified array [not used]
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static PointF[] RoundPoints(PointF[] a)
        {

            ForEach(a, delegate(object o)
            {
                PointF p = (PointF)o;
                p.X = (float)Math.Round(p.X);
                p.Y = (float)Math.Round(p.Y);
                return p;
            });
            return a;
        }
        /// <summary>
        /// Rounds the elements of the specified array [not used]
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static PointF[,] RoundPoints(PointF[,] a)
        {

            ForEach(a, delegate(object o)
            {
                PointF p = (PointF)o;
                p.X = (float)Math.Round(p.X);
                p.Y = (float)Math.Round(p.Y);
                return p;
            });
            return a;
        }

        public static Size RoundPoints(SizeF sizeF)
        {
            return new Size((int)Math.Round(sizeF.Width), (int)Math.Round(sizeF.Height));
        }

        public delegate object ForEachFunction(object o);
        /// <summary>
        /// Modifies the specified array by applying the specified function to each element.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="func">object delegate(object o){}</param>
        /// <returns></returns>
        public static void ForEach(Array a, ForEachFunction func)
        {
            long[] ix = new long[a.Rank];
            //Init index
            for (int i = 0; i < ix.Length; i++) ix[i] = a.GetLowerBound(i);

            //Loop through all items
            for (long i = 0; i < a.LongLength; i++)
            {
                a.SetValue(func(a.GetValue(ix)), ix);

                //Increment ix, the index
                for (int j = 0; j < ix.Length; j++)
                {
                    if (ix[j] < a.GetUpperBound(j))
                    {
                        ix[j]++;
                        break; //We're done incrementing.
                    }
                    else
                    {
                        //Ok, reset this one and increment the next.
                        ix[j] = a.GetLowerBound(j);
                        //If this is the last dimension, assert
                        //that we are at the last element
                        if (j == ix.Length - 1)
                        {
                            if (i < a.LongLength - 1) throw new Exception();
                        }
                        continue;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Aligns the specified rectangle object with its reference ('container') rectangle using the specified alignment. The container can be smaller than 'obj'.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="container"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        public static RectangleF AlignWith(RectangleF obj, RectangleF container, ContentAlignment align)
        {
            if (align == ContentAlignment.BottomLeft || align == ContentAlignment.MiddleLeft || align == ContentAlignment.TopLeft)
                obj.X = container.X;
            if (align == ContentAlignment.BottomCenter || align == ContentAlignment.MiddleCenter || align == ContentAlignment.TopCenter)
                obj.X = container.X + (container.Width - obj.Width) / 2;
            if (align == ContentAlignment.BottomRight || align == ContentAlignment.MiddleRight || align == ContentAlignment.TopRight)
                obj.X = container.X + (container.Width - obj.Width);
            if (align == ContentAlignment.TopLeft || align == ContentAlignment.TopCenter || align == ContentAlignment.TopRight)
                obj.Y = container.Y;
            if (align == ContentAlignment.MiddleLeft || align == ContentAlignment.MiddleCenter || align == ContentAlignment.MiddleRight)
                obj.Y = container.Y + (container.Height - obj.Height) / 2;
            if (align == ContentAlignment.BottomLeft || align == ContentAlignment.BottomCenter || align == ContentAlignment.BottomRight)
                obj.Y = container.Y + (container.Height - obj.Height);

            return obj;
        }
        /// <summary>
        /// Aligns the specified polygon with its container (reference) polygon using the specified alignment. The container can be smaller than 'obj'.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="container"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        public static PointF[] AlignWith(PointF[] obj, PointF[] container, ContentAlignment align)
        {
            RectangleF origBounds = PolygonMath.GetBoundingBox(obj);
            RectangleF newBounds = AlignWith(PolygonMath.GetBoundingBox(obj), PolygonMath.GetBoundingBox(container), align);
            return PolygonMath.MovePoly(obj, new PointF(newBounds.X - origBounds.X, newBounds.Y - origBounds.Y));
        }

        /// <summary>
        /// Returns a bounding box for the specified set of points.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static RectangleF GetBoundingBox(PointF[] points)
        {
            float left = float.MaxValue;
            float top = float.MaxValue;
            float right = float.MinValue;
            float bottom = float.MinValue;
            foreach (PointF f in points)
            {
                if (f.X < left) left = f.X;
                if (f.X > right) right = f.X;
                if (f.Y < top) top = f.Y;
                if (f.Y > bottom) bottom = f.Y;
            }
            return new RectangleF(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Returns a modified version of the array, with each element being offset by the specified amount.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static PointF[] MovePoly(PointF[] points, PointF offset)
        {
            PointF[] pts = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                pts[i].X = points[i].X + offset.X;
                pts[i].Y = points[i].Y + offset.Y;
            }
            return pts;
        }

        /// <summary>
        /// Returns a bounding box for the specified set of points. Odd points are Y values, even points are X values
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static RectangleF GetBoundingBox(double[] flattenedPoints)
        {
            double? minx = null, maxx = null, miny = null, maxy = null;
            for (var i = 0; i < flattenedPoints.Length; i++)
            {
                var v = flattenedPoints[i];
                if (i % 2 == 0)
                {
                    if (minx == null || v < minx.Value) minx = v;
                    if (maxx == null || v > maxx.Value) maxx = v;
                }
                else
                {
                    if (miny == null || v < miny.Value) miny = v;
                    if (maxy == null || v > maxy.Value) maxy = v;
                }
            }
            return new RectangleF((float)minx, (float)miny, (float)(maxx - minx), (float)(maxy - miny));
        }

        /// <summary>
        /// Returns true if 'inner' fits inside or equals 'outer'
        /// </summary>
        /// <param name="inner"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        public static bool FitsInside(SizeF inner, SizeF outer)
        {
            if (inner.Width > outer.Width) return false;
            if (inner.Height > outer.Height) return false;
            return true;
        }

        /// <summary>
        /// Returns a clockwise array of points on the rectangle.
        /// Point 0 is top-left.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static PointF[] ToPoly(RectangleF rect)
        {
            PointF[] r = new PointF[4];
            r[0] = rect.Location;
            r[1] = new PointF(rect.Right, rect.Top);
            r[2] = new PointF(rect.Right, rect.Bottom);
            r[3] = new PointF(rect.Left, rect.Bottom);
            return r;
        }
    }
}
