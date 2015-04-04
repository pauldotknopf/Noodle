using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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

        /// <summary>
        /// Retuns a list of member infos (summary, example, etc) for a member in the documentation file
        /// </summary>
        /// <param name="xmlMember"></param>
        /// <returns></returns>
        List<DocumentationMemberInfo> GetInfosForMember(XmlNode xmlMember); 
    }
}
