namespace Noodle.Extensions.Documentation.BuiltIn
{
    [DocumentationMemberInfoPlugin]
    public class DocumentationMemberSummaryPlugin : DocumentationMemberInfoPlugin
    {
        public override DocumentationMemberInfo TryGetMemberInfo(System.Xml.XmlNode node)
        {
            return node.Name == "summary" ? new DocumentationMemberSummary(node) : null;
        }
    }
}
