using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Noodle.Imaging
{
    /// <summary>
    /// Represents the settings which will be used to process the image. 
    /// Extends NameValueCollection to provide friendly property names for commonly used settings.
    /// Replaced by the Instructions class. Will be removed in V4.0
    /// </summary>
    [Serializable]
    public class ResizeSettings
    {
        /// <summary>
        /// Creates an empty settings collection. 
        /// </summary>
        public ResizeSettings()
        {
            Anchor = ContentAlignment.MiddleCenter;
            Width = -1;
            Height = -1;
            MaxWidth = -1;
            MaxHeight = -1;
            Quality = 90;
            Mode = FitMode.None;
            Rotate = 0.0d;
            Scale = ScaleMode.DownscaleOnly;
            CropValues = new double[] { 0, 0, 0, 0 };
        }

        /// <summary>
        /// Creates a new resize settings object with the specified resizing settings
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        public ResizeSettings(int width, int height, FitMode mode)
        {
            Width = width;
            Height = height;
            Mode = mode;
        }

        /// <summary>
        /// ["width"]: Sets the desired width of the image. (minus padding, borders, margins, effects, and rotation). 
        /// The only instance the resulting image will be smaller is if the original source image is smaller. 
        /// Set Scale=Both to upscale these images and ensure the output always matches 'width' and 'height'. 
        /// If both width and height are specified, the image will be 'letterboxed' to match the desired aspect ratio. 
        /// Change the Mode property to adjust this behavior.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// ["height"]: Sets the desired height of the image.  (minus padding, borders, margins, effects, and rotation)
        /// The only instance the resulting image will be smaller is if the original source image is smaller. 
        /// Set Scale=Both to upscale these images and ensure the output always matches 'width' and 'height'. 
        /// If both width and height are specified, the image will be 'letterboxed' to match the desired aspect ratio. 
        /// Change the Mode property to adjust this behavior.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// ["maxwidth"]: Sets the maximum desired width of the image.  (minus padding, borders, margins, effects, and rotation). 
        /// The image may be smaller than this value to maintain aspect ratio when both maxwidth and maxheight are specified.
        /// </summary>
        public int MaxWidth { get; set; }

        /// <summary>
        /// ["quality"]: The jpeg encoding quality to use. (10..100). 90 is the default and best value, you should leave it.
        /// </summary>
        public int Quality { get; set; }

        /// <summary>
        /// ["maxheight"]: Sets the maximum desired height of the image.  (minus padding, borders, margins, effects, and rotation). 
        /// The image may be smaller than this value to maintain aspect ratio when both maxwidth and maxheight are specified.
        /// </summary>
        public int MaxHeight { get; set; }

        /// <summary>
        /// ["mode"]: Sets the fit mode for the image. max, min, pad, crop, carve, stretch
        /// </summary>
        public FitMode Mode { get; set; }

        /// <summary>
        /// ["rotate"] The degress to rotate the image clockwise. -360 to 360.
        /// </summary>
        public double Rotate { get; set; }

        /// <summary>
        /// How to anchor the image when cropping or adding whitespace to meet sizing requirements.
        /// </summary>
        public ContentAlignment Anchor { get; set; }

        /// <summary>
        /// Allows you to flip the entire resulting image vertically, horizontally, or both. Rotation is not supported.
        /// </summary>
        public RotateFlipType Flip { get; set; }

        /// <summary>
        /// ["scale"] Whether to downscale, upscale, upscale the canvas, or both upscale or downscale the image as needed. Defaults to
        /// DownscaleOnly. See the DefaultSettings plugin to adjust the default.
        /// </summary>
        public ScaleMode Scale { get; set; }

        /// <summary>
        /// 4 values specify x1,y1,x2,y2 values for the crop rectangle.
        /// Negative values are relative to the bottom right - on a 100x100 picture, (10,10,90,90) is equivalent to (10,10,-10,-10). And (0,0,0,0) is equivalent to (0,0,100,100).
        /// </summary>
        protected double[] CropValues { get; set; }

        /// <summary>
        /// ["crop"]=([x1],[y1],x2,y2). Sets x1 and y21, the top-right corner of the crop rectangle. If 0 or greater, the coordinate is relative to the top-left corner of the image.
        /// If less than 0, the value is relative to the bottom-right corner. This allows for easy trimming: crop=(10,10,-10,-10).
        /// Set ["cropxunits"] and ["cropyunits"] to the width/height of the rectangle your coordinates are relative to, if different from the original image size.
        /// </summary>
        public PointF CropTopLeft
        {
            get
            {
                return new PointF((float)CropValues[0], (float)CropValues[1]);
            }
            set
            {
                CropValues = new[] { value.X, value.Y, CropValues[2], CropValues[3] };
            }
        }

        /// <summary>
        /// ["crop"]=(x1,y1,[x2],[y2]). Sets x2 and y2, the bottom-right corner of the crop rectangle. If 1 or greater, the coordinate is relative to the top-left corner of the image.
        /// If 0 or less, the value is relative to the bottom-right corner. This allows for easy trimming: crop=(10,10,-10,-10).
        /// Set ["cropxunits"] and ["cropyunits"] to the width/height of the rectangle your coordinates are relative to, if different from the original image size.
        /// </summary>
        public PointF CropBottomRight
        {
            get
            {
                return new PointF((float)CropValues[2], (float)CropValues[3]);
            }
            set
            {
                CropValues = new[] { CropValues[0], CropValues[1], value.X, value.Y };
            }
        }

        /// <summary>
        /// The width which the X and X2 crop values should be applied. For example, a value of '100' makes X and X2 percentages of the original image width.
        /// This can be set to any non-negative value. Very useful for performing cropping when the original image size is unknown.
        /// 0 indicates that the crop values are relative to the original size of the image.
        /// </summary>
        public double CropXUnits { get; set; }

        /// <summary>
        /// The width which the Y and Y2 crop values should be applied. For example, a value of '100' makes Y and Y2 percentages of the original image height.
        /// This can be set to any non-negative  value. Very useful for performing cropping when the original image size is unknown.
        /// 0 indicates that the crop values are relative to the original size of the image.
        /// </summary>        
        public double CropYUnits { get; set; }

        public RectangleF GetCustomCropSourceRect(SizeF imageSize)
        {
            var xunits = CropXUnits;
            var yunits = CropYUnits;
            return PolygonMath.GetCroppingRectangle(CropValues, xunits, yunits, imageSize);
        } 
    }
}
