using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Noodle.Imaging
{
    /// <summary>
    /// A service responsible for building a layout used for image resizing
    /// </summary>
    public class ImageLayoutBuilder : IImageLayoutBuilder
    {
        /// <summary>
        /// Build a layout for the source image given the resize settings
        /// </summary>
        /// <param name="imageSourceSize"></param>
        /// <param name="resizeSettings"></param>
        public ImageLayout BuildLayout(Size imageSourceSize, ResizeSettings resizeSettings)
        {
            // by default, we are taking the entire image
            var sourceRectangle = new RectangleF(new PointF(0, 0), imageSourceSize);

            var fit = resizeSettings.Mode;
            // determine fit mode to use if both vertical and horizontal limits are used.
            if (fit == FitMode.None)
            {
                if (resizeSettings.Width != -1 || resizeSettings.Height != -1)
                {
                    // we specified both width and height
                    // we are looking for exact width and height then
                    // use pad to ensure height and width but ensure aspect ratio
                    // by adding padding where needed
                    fit = FitMode.Pad;
                }
                else
                {
                    fit = FitMode.Max;
                }
            }

            // Aspect source ratio of the image
            var imageRatio = sourceRectangle.Size.Width / sourceRectangle.Size.Height;

            // zoom factor
            var zoom = resizeSettings.Get<double>("zoom", 1);

            // The target size for the image 
            var imageSize = new SizeF(-1, -1);

            // Target canvas size for the image
            var canvasSize = new SizeF(-1, -1);

            //If any dimensions are specified, calculate. Otherwise, use original image dimensions
            if (resizeSettings.Width != -1 || resizeSettings.Height != -1 || resizeSettings.MaxHeight != -1 || resizeSettings.MaxWidth != -1)
            {
                // a dimension was specified. 
                // we first calculate the largest size the image can be under the width/height/maxwidth/maxheight restrictions.
                // pretending stretch=fill and scale=both

                // temp vars - results stored in targetSize and areaSize
                double width = resizeSettings.Width;
                double height = resizeSettings.Height;
                double maxwidth = resizeSettings.MaxWidth;
                double maxheight = resizeSettings.MaxHeight;

                // eliminate cases where both a value and a max value are specified: use the smaller value for the width/height 
                if (maxwidth > 0 && width > 0) { width = Math.Min(maxwidth, width); maxwidth = -1; }
                if (maxheight > 0 && height > 0) { height = Math.Min(maxheight, height); maxheight = -1; }

                // handle cases of width/maxheight and height/maxwidth. 
                if (width != -1 && maxheight != -1) maxheight = Math.Min(maxheight, (width / imageRatio));
                if (height != -1 && maxwidth != -1) maxwidth = Math.Min(maxwidth, (height * imageRatio));


                //Move max values to width/height. FitMode should already reflect the mode we are using, and we've already resolved mixed modes above.
                width = Math.Max(width, maxwidth);
                height = Math.Max(height, maxheight);

                // calculate missing value (a missing value is handled the same everywhere). 
                if (width > 0 && height <= 0) 
                    height = width / imageRatio;
                else if (height > 0 && width <= 0) 
                    width = height * imageRatio;

                // we now have width & height, our target size. It will only be a different aspect ratio from the image if both 'width' and 'height' are specified.

                if (fit == FitMode.Max)
                {
                    canvasSize = imageSize = PolygonMath.ScaleInside(sourceRectangle.Size, new SizeF((float)width, (float)height));
                }
                else if (fit == FitMode.Pad)
                {
                    canvasSize = new SizeF((float)width, (float)height);
                    imageSize = PolygonMath.ScaleInside(sourceRectangle.Size, canvasSize);
                }
                else if (fit == FitMode.Crop)
                {
                    canvasSize = new SizeF((float)width, (float)height);
                    imageSize = PolygonMath.RoundPoints(PolygonMath.ScaleOutside(canvasSize, sourceRectangle.Size));

                }
                else
                { 
                    // stretch
                    canvasSize = imageSize = new SizeF((float)width, (float)height);
                }
            }
            else
            {
                // no dimensions specified, no fit mode needed.
                canvasSize = imageSize = sourceRectangle.Size;
            }

            //Multiply both areaSize and targetSize by zoom. 
            canvasSize.Width *= (float)zoom;
            canvasSize.Height *= (float)zoom;
            imageSize.Width *= (float)zoom;
            imageSize.Height *= (float)zoom;

            // NOTE: automatic crop is permitted to break the scaling rules.

            //Now do upscale/downscale checks. If they take effect, set targetSize to imageSize
            if (resizeSettings.Scale == ScaleMode.DownscaleOnly)
            {
                if (PolygonMath.FitsInside(sourceRectangle.Size, imageSize))
                {
                    // the image is smaller or equal to its target polygon. Use original image coordinates instead.
                    canvasSize = imageSize = sourceRectangle.Size;
                }
            }
            else if (resizeSettings.Scale == ScaleMode.UpscaleOnly)
            {
                if (!PolygonMath.FitsInside(sourceRectangle.Size, imageSize))
                {
                    // the image is larger than its target. Use original image coordintes instead
                    canvasSize = imageSize = sourceRectangle.Size;
                }
            }
            else if (resizeSettings.Scale == ScaleMode.UpscaleCanvas)
            {
                // same as downscaleonly, except areaSize isn't changed.
                if (PolygonMath.FitsInside(sourceRectangle.Size, imageSize))
                {
                    //The image is smaller or equal to its target polygon. 
                    imageSize = sourceRectangle.Size;
                }
            }

            // require max dimension and round values to minimize rounding differences.
            canvasSize.Width = Math.Max(1, (float)Math.Round(canvasSize.Width));
            canvasSize.Height = Math.Max(1, (float)Math.Round(canvasSize.Height));
            imageSize.Width = Math.Max(1, (float)Math.Round(imageSize.Width));
            imageSize.Height = Math.Max(1, (float)Math.Round(imageSize.Height));


            //Translate and scale all existing rings
        
            var image = PolygonMath.ToPoly(new RectangleF(new PointF(0, 0), imageSize));

            var imageArea = PolygonMath.ToPoly(new RectangleF(new PointF(0, 0), canvasSize));

            // center canvas around the image
            image = PolygonMath.AlignWith(image, imageArea, resizeSettings.Anchor);

            return new ImageLayout(PolygonMath.GetBoundingBox(image), canvasSize);
        }
    }
}
