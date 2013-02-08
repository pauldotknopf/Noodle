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
            // we, by default, are taking the entire image (x=0,y=0,h=source.Height,w=source.Width
            var cropRectangle = new RectangleF(new PointF(0, 0), imageSourceSize);

            // use the crop size if present.
            if (NameValueCollectionExtensions.GetList<double>(resizeSettings, "crop", 0, 4) != null)
            {
                cropRectangle = PolygonMath.ToRectangle(resizeSettings.getCustomCropSourceRect(imageSourceSize)); //Round the custom crop rectangle coordinates
                if (cropRectangle.Size.IsEmpty) throw new Exception("You must specify a custom crop rectange if crop=custom");
            }

            var fit = resizeSettings.Mode;
            // determine fit mode to use if both vertical and horizontal limits are used.
            if (fit == FitMode.None)
            {
                if (resizeSettings.Width != -1 || resizeSettings.Height != -1)
                {
                    if ("fill".Equals(resizeSettings["stretch"], StringComparison.OrdinalIgnoreCase)) 
                        fit = FitMode.Stretch;
                    else if ("auto".Equals(resizeSettings["crop"], StringComparison.OrdinalIgnoreCase)) 
                        fit = FitMode.Crop;
                    else 
                        fit = FitMode.Pad;
                }
                else
                {
                    fit = FitMode.Max;
                }
            }

            // Aspect ratio of the image
            var imageRatio = cropRectangle.Size.Width / cropRectangle.Size.Height;

            // zoom factor
            var zoom = resizeSettings.Get<double>("zoom", 1);
            //The target size for the image 
            var targetSize = new SizeF(-1, -1);
            //Target area for the image
            var areaSize = new SizeF(-1, -1);
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
                    areaSize = targetSize = PolygonMath.ScaleInside(cropRectangle.Size, new SizeF((float)width, (float)height));
                }
                else if (fit == FitMode.Pad)
                {
                    areaSize = new SizeF((float)width, (float)height);
                    targetSize = PolygonMath.ScaleInside(cropRectangle.Size, areaSize);
                }
                else if (fit == FitMode.Crop)
                {
                    // we autocrop - so both target and area match the requested size
                    areaSize = targetSize = new SizeF((float)width, (float)height);
                    // determine the size of the area we are copying
                    var sourceSize = PolygonMath.RoundPoints(PolygonMath.ScaleInside(areaSize, cropRectangle.Size));
                    // center the portion we are copying within the manualCropSize
                    cropRectangle = PolygonMath.ToRectangle(PolygonMath.AlignWith(new RectangleF(0, 0, sourceSize.Width, sourceSize.Height), cropRectangle, resizeSettings.Anchor));
                }
                else
                { 
                    //Stretch and carve both act like stretching, so do that:
                    areaSize = targetSize = new SizeF((float)width, (float)height);
                }
            }
            else
            {
                // no dimensions specified, no fit mode needed. Use manual crop dimensions
                areaSize = targetSize = cropRectangle.Size;
            }

            //Multiply both areaSize and targetSize by zoom. 
            areaSize.Width *= (float)zoom;
            areaSize.Height *= (float)zoom;
            targetSize.Width *= (float)zoom;
            targetSize.Height *= (float)zoom;

            // NOTE: automatic crop is permitted to break the scaling rules.

            //Now do upscale/downscale checks. If they take effect, set targetSize to imageSize
            if (resizeSettings.Scale == ScaleMode.DownscaleOnly)
            {
                if (PolygonMath.FitsInside(cropRectangle.Size, targetSize))
                {
                    // the image is smaller or equal to its target polygon. Use original image coordinates instead.
                    areaSize = targetSize = cropRectangle.Size;
                }
            }
            else if (resizeSettings.Scale == ScaleMode.UpscaleOnly)
            {
                if (!PolygonMath.FitsInside(cropRectangle.Size, targetSize))
                {
                    // the image is larger than its target. Use original image coordintes instead
                    areaSize = targetSize = cropRectangle.Size;
                }
            }
            else if (resizeSettings.Scale == ScaleMode.UpscaleCanvas)
            {
                // same as downscaleonly, except areaSize isn't changed.
                if (PolygonMath.FitsInside(cropRectangle.Size, targetSize))
                {
                    //The image is smaller or equal to its target polygon. 
                    targetSize = cropRectangle.Size;
                }
            }

            // require max dimension and round values to minimize rounding differences.
            areaSize.Width = Math.Max(1, (float)Math.Round(areaSize.Width));
            areaSize.Height = Math.Max(1, (float)Math.Round(areaSize.Height));
            targetSize.Width = Math.Max(1, (float)Math.Round(targetSize.Width));
            targetSize.Height = Math.Max(1, (float)Math.Round(targetSize.Height));


            //Translate and scale all existing rings
        
            var image = PolygonMath.ToPoly(new RectangleF(new PointF(0, 0), targetSize));

            var imageArea = PolygonMath.ToPoly(new RectangleF(new PointF(0, 0), areaSize));

            //Center imageArea around 'image'
            imageArea = PolygonMath.AlignWith(imageArea, image, resizeSettings.Anchor);

            return new ImageLayout(PolygonMath.GetBoundingBox(image), PolygonMath.GetCroppingRectangle(imageArea));
        }
    }
}
