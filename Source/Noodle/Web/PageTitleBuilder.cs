using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Web
{
    /// <summary>
    /// An implementation of pagetitle builder.
    /// This is designed to be stored/used per-request
    /// </summary>
    public class PageTitleBuilder : IPageTitleBuilder
    {
        private readonly SeoSettings _seoSettings;
        private readonly List<string> _titleParts;
        private readonly List<string> _metaDescriptionParts;
        private readonly List<string> _metaKeywordParts;
        private readonly List<string> _canonicalUrlParts;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageTitleBuilder"/> class.
        /// </summary>
        /// <param name="seoSettings">The seo settings.</param>
        public PageTitleBuilder(SeoSettings seoSettings)
        {
            _seoSettings = seoSettings;
            _titleParts = new List<string>();
            _metaDescriptionParts = new List<string>();
            _metaKeywordParts = new List<string>();
            _canonicalUrlParts = new List<string>();
        }

        /// <summary>
        /// Add title parts to the page to the end
        /// </summary>
        /// <param name="parts"></param>
        public void AddTitleParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _titleParts.Add(part);
        }

        /// <summary>
        /// Adds title parts to the begining
        /// </summary>
        /// <param name="parts"></param>
        public void AppendTitleParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _titleParts.Insert(0, part);
        }

        /// <summary>
        /// Generates the page title
        /// </summary>
        /// <param name="addDefaultTitle"></param>
        /// <returns></returns>
        public string GenerateTitle(bool addDefaultTitle)
        {
            string result;
            var specificTitle = string.Join(_seoSettings.PageTitleSeparator, _titleParts.AsEnumerable().Reverse().ToArray());
            if (!String.IsNullOrEmpty(specificTitle))
            {
                if (addDefaultTitle)
                {
                    // default title + page title
                    switch (_seoSettings.PageTitleSeoAdjustment)
                    {
                        case PageTitleSeoAdjustment.PagenameAfterSitename:
                            {
                                result = string.Join(_seoSettings.PageTitleSeparator,new[]{ _seoSettings.DefaultTitle, specificTitle});
                            }
                            break;
                        default:
                            {
                                result = string.Join(_seoSettings.PageTitleSeparator, new[] { specificTitle, _seoSettings.DefaultTitle });
                            }
                            break;

                    }
                }
                else
                {
                    // page title only
                    result = specificTitle;
                }
            }
            else
            {
                // default title only
                result = _seoSettings.DefaultTitle;
            }
            return result;
        }

        /// <summary>
        /// Add meta description parts to the end
        /// </summary>
        /// <param name="parts"></param>
        public void AddMetaDescriptionParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaDescriptionParts.Add(part);
        }

        /// <summary>
        /// Add meta description parts to the beggining
        /// </summary>
        /// <param name="parts"></param>
        public void AppendMetaDescriptionParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaDescriptionParts.Insert(0, part);
        }

        /// <summary>
        /// Generate the meta descrition
        /// </summary>
        /// <returns></returns>
        public string GenerateMetaDescription()
        {
            var metaDescription = string.Join(", ", _metaDescriptionParts.AsEnumerable().Reverse().ToArray());
            var result = !String.IsNullOrEmpty(metaDescription) ? metaDescription : _seoSettings.DefaultMetaDescription;
            return result;
        }

        /// <summary>
        /// Add meta keywords to the end
        /// </summary>
        /// <param name="parts"></param>
        public void AddMetaKeywordParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaKeywordParts.Add(part);
        }

        /// <summary>
        /// Add meta keywords to the beginning
        /// </summary>
        /// <param name="parts"></param>
        public void AppendMetaKeywordParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaKeywordParts.Insert(0, part);
        }

        /// <summary>
        /// Generate a string of meta keywords
        /// </summary>
        /// <returns></returns>
        public string GenerateMetaKeywords()
        {
            var metaKeyword = string.Join(", ", _metaKeywordParts.AsEnumerable().Reverse().ToArray());
            var result = !String.IsNullOrEmpty(metaKeyword) ? metaKeyword : _seoSettings.DefaultMetaKeywords;
            return result;
        }

        /// <summary>
        /// Add canonical urls to the end
        /// </summary>
        /// <param name="parts"></param>
        public void AddCanonicalUrlParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _canonicalUrlParts.Add(part);
        }

        /// <summary>
        /// Add canonical url parts to the begining
        /// </summary>
        /// <param name="parts"></param>
        public void AppendCanonicalUrlParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _canonicalUrlParts.Insert(0, part);
        }

        /// <summary>
        /// Generate a string of all the canonical urls
        /// </summary>
        /// <returns></returns>
        public string GenerateCanonicalUrls()
        {
            var result = new StringBuilder();
            foreach (var canonicalUrl in _canonicalUrlParts)
            {
                result.AppendFormat("<link rel=\"canonical\" href=\"{0}\" />", canonicalUrl);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }
    }
}
