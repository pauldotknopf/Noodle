using System.Xml;

namespace Noodle.Extensions.Documentation
{
    /// <summary>
    /// Derived classes have the ability to provide specialized classes for certain (or custom) xml documentation nodes
    /// </summary>
    public abstract class DocumentationMemberInfoPlugin
    {
        /// <summary>
        /// If the node is supported by this pugin, return an instance of an already build member info
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public abstract DocumentationMemberInfo TryGetMemberInfo(XmlNode node);
    }
}
