using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using Noodle.Collections;

namespace Noodle.Imaging
{
    /// <summary>
    /// Represents the settings which will be used to process the image. 
    /// Extends NameValueCollection to provide friendly property names for commonly used settings.
    /// Replaced by the Instructions class. Will be removed in V4.0
    /// </summary>
    [Serializable]
    public class ResizeSettings : QuerystringBase<ResizeSettings>
    {
        /// <summary>
        /// Creates an empty settings collection. 
        /// </summary>
        public ResizeSettings() : base() { }
        /// <summary>
        /// Copies the specified collection into a new ResizeSettings instance.
        /// </summary>
        /// <param name="col"></param>
        public ResizeSettings(NameValueCollection col) : base(col) { }
        /// <summary>
        /// Parses the specified querystring into name/value pairs. leading ? not required.
        /// </summary>
        /// <param name="queryString"></param>
        public ResizeSettings(string queryString) : base(Web.Url.ParseQueryStringAsNameValueCollection(queryString)) { }

        /// <summary>
        /// Creates a new resize settings object with the specified resizing settings
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        public ResizeSettings(int width, int height, FitMode mode)
        {
            this.Width = width;
            this.Height = height;
            this.Mode = mode;
        }


        protected int get(string name, int defaultValue) { return this.Get<int>(name, defaultValue); }
        protected void set(string name, int value) { this.Set<int>(name, value); }

        protected double get(string name, double defaultValue) { return this.Get<double>(name, defaultValue); }
        protected void set(string name, double value) { this.Set<double>(name, value); }

        /// <summary>
        /// ["width"]: Sets the desired width of the image. (minus padding, borders, margins, effects, and rotation). 
        /// The only instance the resulting image will be smaller is if the original source image is smaller. 
        /// Set Scale=Both to upscale these images and ensure the output always matches 'width' and 'height'. 
        /// If both width and height are specified, the image will be 'letterboxed' to match the desired aspect ratio. 
        /// Change the Mode property to adjust this behavior.
        /// </summary>
        public int Width
        {
            get
            {
                return get("width", get("w", -1));
            }
            set
            {
                set("width", value); this.Remove("w");
            }
        }

        /// <summary>
        /// ["height"]: Sets the desired height of the image.  (minus padding, borders, margins, effects, and rotation)
        /// The only instance the resulting image will be smaller is if the original source image is smaller. 
        /// Set Scale=Both to upscale these images and ensure the output always matches 'width' and 'height'. 
        /// If both width and height are specified, the image will be 'letterboxed' to match the desired aspect ratio. 
        /// Change the Mode property to adjust this behavior.
        /// </summary>
        public int Height
        {
            get
            {
                return get("height", get("h", -1));
            }
            set
            {
                set("height", value); this.Remove("h");
            }
        }

        /// <summary>
        /// ["maxwidth"]: Sets the maximum desired width of the image.  (minus padding, borders, margins, effects, and rotation). 
        /// The image may be smaller than this value to maintain aspect ratio when both maxwidth and maxheight are specified.
        /// </summary>
        public int MaxWidth
        {
            get
            {
                return get("maxwidth", -1);
            }
            set
            {
                set("maxwidth", value);
            }
        }


        /// <summary>
        /// ["quality"]: The jpeg encoding quality to use. (10..100). 90 is the default and best value, you should leave it.
        /// </summary>
        public int Quality
        {
            get
            {
                return get("quality", 90);
            }
            set
            {
                set("quality", value);
            }
        }


        /// <summary>
        /// ["maxheight"]: Sets the maximum desired height of the image.  (minus padding, borders, margins, effects, and rotation). 
        /// The image may be smaller than this value to maintain aspect ratio when both maxwidth and maxheight are specified.
        /// </summary>
        public int MaxHeight
        {
            get
            {
                return get("maxheight", -1);
            }
            set
            {
                set("maxheight", value);
            }
        }

        /// <summary>
        /// ["mode"]: Sets the fit mode for the image. max, min, pad, crop, carve, stretch
        /// </summary>
        public FitMode Mode
        {
            get
            {
                return Get("mode", FitMode.None);
            }
            set
            {
                Set<FitMode>("mode", value);
            }
        }

        /// <summary>
        /// Returns true if any of the specified keys are present in this NameValueCollection
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public bool WasOneSpecified(params string[] keys)
        {
            return NameValueCollectionExtensions.IsOneSpecified(this, keys);
        }

        /// <summary>
        /// How to anchor the image when cropping or adding whitespace to meet sizing requirements.
        /// </summary>
        public ContentAlignment Anchor
        {
            get
            {
                return this.Get<ContentAlignment>("anchor", ContentAlignment.MiddleCenter);
            }
            set
            {
                this.Set<ContentAlignment>("anchor", value);
            }
        }



        /// <summary>
        /// Allows you to flip the entire resulting image vertically, horizontally, or both. Rotation is not supported.
        /// </summary>
        public RotateFlipType Flip
        {
            get
            {
                return (RotateFlipType)this.Get<FlipMode>("flip", FlipMode.None);
            }
            set
            {
                this.Set<FlipMode>("flip", (FlipMode)value);
            }
        }

        /// <summary>
        /// ["sFlip"] Allows you to flip the source image vertically, horizontally, or both. Rotation is not supported.
        /// </summary>
        public RotateFlipType SourceFlip
        {
            get
            {
                return (RotateFlipType)this.Get<FlipMode>("sFlip", this.Get<FlipMode>("sourceFlip", FlipMode.None));
            }
            set
            {
                this.Set<FlipMode>("sflip", (FlipMode)value);
            }
        }

        /// <summary>
        /// ["scale"] Whether to downscale, upscale, upscale the canvas, or both upscale or downscale the image as needed. Defaults to
        /// DownscaleOnly. See the DefaultSettings plugin to adjust the default.
        /// </summary>
        public ScaleMode Scale
        {
            get
            {
                return this.Get<ScaleMode>("scale", ScaleMode.DownscaleOnly);
            }
            set
            {
                this.Set<ScaleMode>("scale", value);
            }
        }

        /// <summary>
        /// 4 values specify x1,y1,x2,y2 values for the crop rectangle.
        /// Negative values are relative to the bottom right - on a 100x100 picture, (10,10,90,90) is equivalent to (10,10,-10,-10). And (0,0,0,0) is equivalent to (0,0,100,100).
        /// </summary>
        protected double[] CropValues
        {
            get
            {
                //Return (0,0,0,0) when null.
                double[] vals = NameValueCollectionExtensions.GetList<double>(this, "crop", 0, 4);
                return vals ?? new double[] { 0, 0, 0, 0 };
            }
            set
            {
                NameValueCollectionExtensions.SetList(this, "crop", value, true, 4);
            }
        }

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
        public double CropXUnits { get { return this.Get<double>("cropxunits", 0); } set { this.Set<double>("cropxunits", value <= 0 ? null : (double?)value); } }
        /// <summary>
        /// The width which the Y and Y2 crop values should be applied. For example, a value of '100' makes Y and Y2 percentages of the original image height.
        /// This can be set to any non-negative  value. Very useful for performing cropping when the original image size is unknown.
        /// 0 indicates that the crop values are relative to the original size of the image.
        /// </summary>        
        public double CropYUnits { get { return this.Get<double>("cropyunits", 0); } set { this.Set<double>("cropyunits", value <= 0 ? null : (double?)value); } }


        public RectangleF getCustomCropSourceRect(SizeF imageSize)
        {
            double xunits = this.Get<double>("cropxunits", 0);
            double yunits = this.Get<double>("cropyunits", 0);

            return PolygonMath.GetCroppingRectangle(CropValues, xunits, yunits, imageSize);
        }

        /// <summary>
        /// Returns a string containing all the settings in the class, in querystring form. Use ToStringEncoded() to get a URL-safe querystring. 
        /// This method does not encode commas, spaces, etc.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // TODO: return something meaning ful
            return base.ToString();
        }

        /// <summary>
        /// This method will 'normalize' command aliases to the primary key name and resolve duplicates. 
        /// w->width, h->height, sourceFlip->sFlip, thumbnail->format
        /// </summary>
        public void Normalize()
        {
            Normalize("width", "w")
                .Normalize("height", "h");
        }

        /// <summary>
        /// Normalizes a command that has two possible names. 
        /// If either of the commands has a null or empty value, those keys are removed. 
        /// If both the the primary and secondary are present, the secondary is removed. 
        /// Otherwise, the secondary is renamed to the primary name.
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        public ResizeSettings Normalize(string primary, string secondary)
        {
            return (ResizeSettings)NameValueCollectionExtensions.Normalize(this, primary, secondary);
        }
    }
}
