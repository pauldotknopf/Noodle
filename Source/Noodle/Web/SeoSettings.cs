namespace Noodle.Web
{
    public class SeoSettings : ISettings
    {
        public SeoSettings()
        {
            PageTitleSeparator = " - ";
            DefaultTitle = "Noodle";
            PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterSitename;
        }

        public string PageTitleSeparator { get; set; }
        public PageTitleSeoAdjustment PageTitleSeoAdjustment { get; set; }
        public string DefaultTitle { get; set; }
        public string DefaultMetaKeywords { get; set; }
        public string DefaultMetaDescription { get; set; }
    }
}
