namespace Noodle.Web
{
    /// <summary>
    /// The settings used for SEO
    /// </summary>
    public class SeoSettings : ISettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeoSettings"/> class.
        /// </summary>
        public SeoSettings()
        {
            PageTitleSeparator = " - ";
            DefaultTitle = "Noodle";
            PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterSitename;
        }

        /// <summary>
        /// The seperator of all the page-title parts
        /// </summary>
        public string PageTitleSeparator { get; set; }

        /// <summary>
        /// Should the default page title be inserted at the end or the beginning of the page title parts?
        /// </summary>
        public PageTitleSeoAdjustment PageTitleSeoAdjustment { get; set; }

        /// <summary>
        /// The default page title that will be displayed on every page
        /// </summary>
        public string DefaultTitle { get; set; }

        /// <summary>
        /// The default meta keywords to be used if none are given
        /// </summary>
        public string DefaultMetaKeywords { get; set; }

        /// <summary>
        /// The default meta description to be used if none are given
        /// </summary>
        public string DefaultMetaDescription { get; set; }
    }
}
