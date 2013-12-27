using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MongoDB.Driver;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Class LanguageInstaller
    /// </summary>
    public class LanguageInstaller : ILanguageInstaller
    {
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageFileParser _languageFileParser;
        private readonly IErrorNotifier _errorNotifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageInstaller"/> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="localizationService">The localization service.</param>
        /// <param name="languageFileParser">The language file parser.</param>
        /// <param name="errorNotifier">The error notifier.</param>
        public LanguageInstaller(ILanguageService languageService,
            ILocalizationService localizationService,
            ILanguageFileParser languageFileParser,
            IErrorNotifier errorNotifier)
        {
            _languageService = languageService;
            _localizationService = localizationService;
            _languageFileParser = languageFileParser;
            _errorNotifier = errorNotifier;
        }

        /// <summary>
        /// Install languages from the given xml file
        /// </summary>
        /// <param name="languagesXmlFileLocation">The languages XML file location.</param>
        public void Install(string languagesXmlFileLocation)
        {
            // Let's first delete everything
            _languageService.DeleteAll();

            var languages = _languageFileParser.DeserializeLanguagesFile(languagesXmlFileLocation);

            try
            {
                foreach (var language in languages)
                {
                    // insert the language
                    _languageService.InsertLanguage(language.First);

                    // update all the resources with the new id of the language
                    Array.ForEach(language.Second.ToArray(),
                        (resource) => resource.LanguageId = language.First.Id);

                    // batch insert them!
                    _localizationService.InsertLocaleStringResources(language.Second.ToArray());
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Problem installing languages. " + ex.Message);
                _errorNotifier.Notify(ex);
            }
        }

    }
}
