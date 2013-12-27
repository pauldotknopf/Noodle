using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Install languages from an xml file
    /// </summary>
    public interface ILanguageInstaller
    {
        /// <summary>
        /// Install languages from the given xml file
        /// </summary>
        /// <param name="languagesXmlFileLocation"></param>
        void Install(string languagesXmlFileLocation);
    }
}
