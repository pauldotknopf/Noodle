using System.Collections.Specialized;

namespace Noodle.Imaging
{
    /// <summary>
    /// Represents the settings to resize/manipulator an image
    /// </summary>
    public class ImageManipulationSettings : ResizeSettings
    {
        /// <summary>
        /// Creates an empty settings collection. 
        /// </summary>
        public ImageManipulationSettings() { }
        /// <summary>
        /// Copies the specified collection into a new ResizeSettings instance.
        /// </summary>
        /// <param name="col"></param>
        public ImageManipulationSettings(NameValueCollection col) : base(col) { }
        /// <summary>
        /// Parses the specified querystring into name/value pairs. leading ? not required.
        /// </summary>
        /// <param name="queryString"></param>
        public ImageManipulationSettings(string queryString) : base(Web.Url.ParseQueryStringAsNameValueCollection(queryString)) { }

        public double? Brightness
        {
            get { return Get<double>("brightness", null); }
            set { Set("brightness", value); }
        }

        public double? Contrast
        {
            get { return Get<double>("contrast", null); }
            set { Set("contrast", value); }
        }

        public double? Gamma
        {
            get { return Get<double>("gamma", null); }
            set { Set("gamma", value); }
        }

        public double? Hue
        {
            get { return Get<double>("hue", null); }
            set { Set("hue", value); }
        }

        public double? Saturation
        {
            get { return Get<double>("saturation", null); }
            set { Set("saturation", value); }
        }

        public double? Sharpen
        {
            get { return Get<double>("sharpen", null); }
            set { Set("sharpen", value); }
        }
    }
}
