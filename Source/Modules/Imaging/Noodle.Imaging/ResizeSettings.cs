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
        public ResizeSettings() { }
        /// <summary>
        /// Copies the specified collection into a new ResizeSettings instance.
        /// </summary>
        /// <param name="col"></param>
        public ResizeSettings(NameValueCollection col) : base(col) { }
        /// <summary>
        /// Parses the specified querystring into name/value pairs. leading ? not required.
        /// </summary>
        /// <param name="queryString"></param>
        public ResizeSettings(string queryString) : base(CommonHelper.ParseQueryStringAsNameValueCollection(queryString)) { }

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


        protected int Get(string name, int defaultValue) { return this.Get<int>(name, defaultValue); }
        protected void Set(string name, int value) { this.Set<int>(name, value); }

        protected double Get(string name, double defaultValue) { return this.Get<double>(name, defaultValue); }
        protected void Set(string name, double value) { this.Set<double>(name, value); }

        /// <summary>
        /// ["width"]: Sets the desired width of the image.
        /// The only instance the resulting image will be smaller is if the original source image is smaller. 
        /// Set Scale=Both to upscale these images and ensure the output always matches 'width' and 'height'. 
        /// If both width and height are specified, the image will be 'letterboxed' to match the desired aspect ratio. 
        /// Change the Mode property to adjust this behavior.
        /// </summary>
        public int Width
        {
            get
            {
                return Get("width", Get("w", -1));
            }
            set
            {
                Set("width", value); this.Remove("w");
            }
        }

        /// <summary>
        /// ["height"]: Sets the desired height of the image.
        /// The only instance the resulting image will be smaller is if the original source image is smaller. 
        /// Set Scale=Both to upscale these images and ensure the output always matches 'width' and 'height'. 
        /// If both width and height are specified, the image will be 'letterboxed' to match the desired aspect ratio. 
        /// Change the Mode property to adjust this behavior.
        /// </summary>
        public int Height
        {
            get { return Get("height", Get("h", -1)); }
            set { Set("height", value); this.Remove("h"); }
        }

        /// <summary>
        /// ["maxwidth"]: Sets the maximum desired width of the image.
        /// The image may be smaller than this value to maintain aspect ratio when both maxwidth and maxheight are specified.
        /// </summary>
        public int MaxWidth
        {
            get { return Get("maxwidth", -1); }
            set { Set("maxwidth", value); }
        }

        /// <summary>
        /// ["maxheight"]: Sets the maximum desired height of the image.
        /// The image may be smaller than this value to maintain aspect ratio when both maxwidth and maxheight are specified.
        /// </summary>
        public int MaxHeight
        {
            get { return Get("maxheight", -1); }
            set { Set("maxheight", value); }
        }

        /// <summary>
        /// ["mode"]: Sets the fit mode for the image. max, min, pad, crop, carve, stretch
        /// </summary>
        public FitMode Mode
        {
            get { return Get("mode", FitMode.None); }
            set { Set<FitMode>("mode", value); }
        }

        /// <summary>
        /// How to anchor the image when cropping or adding whitespace to meet sizing requirements.
        /// </summary>
        public ContentAlignment Anchor
        {
            get { return Get("anchor", ContentAlignment.MiddleCenter); }
            set { Set<ContentAlignment>("anchor", value); }
        }

        /// <summary>
        /// ["scale"] Whether to downscale, upscale, upscale the canvas, or both upscale or downscale the image as needed. Defaults to
        /// DownscaleOnly.
        /// </summary>
        public ScaleMode Scale
        {
            get { return Get("scale", ScaleMode.DownscaleOnly); }
            set { Set<ScaleMode>("scale", value); }
        }

        /// <summary>
        /// Returns a string containing all the settings in the class, in querystring form. Use ToStringEncoded() to get a URL-safe querystring. 
        /// This method does not encode commas, spaces, etc.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(";", Keys.Cast<string>().Select(x => x + ":" + this[x]));
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
