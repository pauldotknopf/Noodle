using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Noodle.Plugins;

namespace Noodle.Documentation
{
    /// <summary>
    /// Manages all things involved in deserializing/consuming an xml documentation file
    /// </summary>
    public class DocumentationService : IDocumentationService
    {
        private readonly IPluginFinder _pluginFinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationService"/> class.
        /// </summary>
        /// <param name="pluginFinder">The plugin finder.</param>
        public DocumentationService(IPluginFinder pluginFinder)
        {
            _pluginFinder = pluginFinder;
        }

        /// <summary>
        /// Deserialize the given xml file to a DocumentationAssembly for usage
        /// </summary>
        /// <param name="xmlDocumentationFile"></param>
        /// <returns></returns>
        public DocumentationAssembly DeserializeDocumentation(string xmlDocumentationFile)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlDocumentationFile);

            var rootNode = xmlDocument.SelectSingleNode("doc");
            if(rootNode == null) throw new InvalidOperationException("The root node must be 'doc'");

            var assemblyNode = rootNode.SelectSingleNode("assembly");
            if(assemblyNode == null) throw new InvalidOperationException("There is no doc/assembly node");

            var assemblyNameNode = assemblyNode.SelectSingleNode("name");
            if(assemblyNameNode == null) throw new InvalidOperationException("There is no name node for the assembly node");

            var membersNode = rootNode.SelectSingleNode("members");
            if(membersNode == null) throw new InvalidOperationException("There is no members node");

            var result = new DocumentationAssembly();

            foreach (XmlNode memberNode in membersNode.SelectNodes("member"))
            {
                result.Members.Add(new DocumentationMember(memberNode, this));
            }

            result.AssemblyName = assemblyNameNode.InnerText;

            return result;
        }

        /// <summary>
        /// Retuns a list of member infos (summary, example, etc) for a member in the documentation file
        /// </summary>
        /// <param name="xmlMember"></param>
        /// <returns></returns>
        public List<DocumentationMemberInfo> GetInfosForMember(XmlNode xmlMember)
        {
            var infoPlugins = _pluginFinder.GetPlugins<DocumentationMemberInfoPluginAttribute, DocumentationMemberInfoPlugin>().ToList();
            var result = new List<DocumentationMemberInfo>();

            foreach(XmlNode xmlMembeInfoNode in xmlMember.ChildNodes)
            {
                DocumentationMemberInfo infoPluginResult = null;
                foreach(var infoPlugin in infoPlugins)
                {
                    if (infoPluginResult == null)
                    {
                        infoPluginResult = infoPlugin.TryGetMemberInfo(xmlMembeInfoNode);
                    }
                }
                if(infoPluginResult == null)
                    infoPluginResult = new DocumentationMemberInfoUnknown(xmlMembeInfoNode);
                result.Add(infoPluginResult);
            }

            return result;
        }
    }
}
