using System.Xml;

namespace Noodle.Extensions.Documentation.BuiltIn
{
    public class DocumentationMemberSummary : DocumentationMemberInfo
    {
        public DocumentationMemberSummary(XmlNode node)
            :base(node)
        {
            Summary = CleanText(node.InnerText);
        }

        public string Summary { get; protected set; }
    }
}
