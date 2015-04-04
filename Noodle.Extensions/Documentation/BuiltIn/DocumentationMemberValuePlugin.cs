using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Documentation.BuiltIn
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
