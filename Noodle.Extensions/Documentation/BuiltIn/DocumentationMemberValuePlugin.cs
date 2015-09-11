namespace Noodle.Extensions.Documentation.BuiltIn
{
    [DocumentationMemberInfoPlugin]
    public class DocumentationMemberValuePlugin : DocumentationMemberInfoPlugin
    {
        public override DocumentationMemberInfo TryGetMemberInfo(System.Xml.XmlNode node)
        {
            return node.Name == "value" ? new DocumentationMemberValue(node) : null;
        }
    }
}
