namespace Noodle.Web
{
    /// <summary>
    /// Interface that is per request that build page titles and other related seo stuff
    /// </summary>
    public interface IPageTitleBuilder
    {
        /// <summary>
        /// Add title parts to the page to the end
        /// </summary>
        /// <param name="parts"></param>
        void AddTitleParts(params string[] parts);

        /// <summary>
        /// Adds title parts to the begining
        /// </summary>
        /// <param name="parts"></param>
        void AppendTitleParts(params string[] parts);

        /// <summary>
        /// Generates the page title
        /// </summary>
        /// <param name="addDefaultTitle"></param>
        /// <returns></returns>
        string GenerateTitle(bool addDefaultTitle);

        void AddMetaDescriptionParts(params string[] parts);
        void AppendMetaDescriptionParts(params string[] parts);
        string GenerateMetaDescription();

        void AddMetaKeywordParts(params string[] parts);
        void AppendMetaKeywordParts(params string[] parts);
        string GenerateMetaKeywords();

        void AddCanonicalUrlParts(params string[] parts);
        void AppendCanonicalUrlParts(params string[] parts);
        string GenerateCanonicalUrls();
    }
}
