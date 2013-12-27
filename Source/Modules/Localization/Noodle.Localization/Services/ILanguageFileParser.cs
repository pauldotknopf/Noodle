using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// The service that parses files into a workable set of languages/resources.
    /// </summary>
    public interface ILanguageFileParser
    {
        /// <summary>
        /// Deserialize the xml language file to an in memory collection for modification (and maybe inserting?)
        /// </summary>
        /// <param name="languagesXmlFileLocation">The languages XML file location.</param>
        /// <returns></returns>
        List<Pair<Language, List<LocaleStringResource>>> DeserializeLanguagesFile(string languagesXmlFileLocation);
    }
}
