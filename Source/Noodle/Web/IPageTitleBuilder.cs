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

        /// <summary>
        /// Add meta description parts to the end
        /// </summary>
        /// <param name="parts"></param>
        void AddMetaDescriptionParts(params string[] parts);

        /// <summary>
        /// Add meta description parts to the beggining
        /// </summary>
        /// <param name="parts"></param>
        void AppendMetaDescriptionParts(params string[] parts);

        /// <summary>
        /// Generate the meta descrition
        /// </summary>
        /// <returns></returns>
        string GenerateMetaDescription();

        /// <summary>
        /// Add meta keywords to the end
        /// </summary>
        /// <param name="parts"></param>
        void AddMetaKeywordParts(params string[] parts);

        /// <summary>
        /// Add meta keywords to the beginning
        /// </summary>
        /// <param name="parts"></param>
        void AppendMetaKeywordParts(params string[] parts);

        /// <summary>
        /// Generate a string of meta keywords
        /// </summary>
        /// <returns></returns>
        string GenerateMetaKeywords();

        /// <summary>
        /// Add canonical urls to the end
        /// </summary>
        /// <param name="parts"></param>
        void AddCanonicalUrlParts(params string[] parts);

        /// <summary>
        /// Add canonical url parts to the begining
        /// </summary>
        /// <param name="parts"></param>
        void AppendCanonicalUrlParts(params string[] parts);

        /// <summary>
        /// Generate a string of all the canonical urls
        /// </summary>
        /// <returns></returns>
        string GenerateCanonicalUrls();
    }
}
