using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Install languages from an xml filel
    /// </summary>
    public interface ILanguageInstaller
    {
        /// <summary>
        /// Install languages from the given xml file
        /// </summary>
        /// <param name="languagesXmlFileLocation"></param>
        void Install(string languagesXmlFileLocation);

        /// <summary>
        /// Deserialize the xml language file to an inmemoy collection for modification (and maybe inserting?)
        /// </summary>
        /// <param name="languagesXmlFileLocation"></param>
        /// <returns></returns>
        Dictionary<Language, List<LocaleStringResource>> DeserializeLanguagesFile(string languagesXmlFileLocation);
    }
}
