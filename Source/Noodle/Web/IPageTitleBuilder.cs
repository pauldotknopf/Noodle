namespace Noodle.Web
{
    /// <summary>
    /// Interface that is per request that build page titles and other related seo stuff
    /// </summary>
    public interface IPageTitleBuilder
    {
        void AddTitleParts(params string[] parts);
        void AppendTitleParts(params string[] parts);
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
