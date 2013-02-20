using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Documentation.BuiltIn
{
    [DocumentationMemberInfoPlugin]
    public class DocumentationMemberReturnsPlugin : DocumentationMemberInfoPlugin
    {
        public override DocumentationMemberInfo TryGetMemberInfo(System.Xml.XmlNode node)
        {
            return node.Name == "returns" ? new DocumentationMemberReturns(node) : null;
        }
    }
}
