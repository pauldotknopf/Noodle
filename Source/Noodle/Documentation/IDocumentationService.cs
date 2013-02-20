using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Documentation
{
    /// <summary>
    /// Manages all things involved in deserializing/consuming an xml documentation file
    /// </summary>
    public interface IDocumentationService
    {
        /// <summary>
        /// Deserialize the given xml file to a DocumentationAssembly for usage
        /// </summary>
        /// <param name="xmlDocumentationFile"></param>
        /// <returns></returns>
        DocumentationAssembly DeserializeDocumentation(string xmlDocumentationFile);
    }
}
