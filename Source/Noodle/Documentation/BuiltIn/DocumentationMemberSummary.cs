using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Noodle.Documentation.BuiltIn
{
    public class DocumentationMemberSummary : DocumentationMemberInfo
    {
        public DocumentationMemberSummary(XmlNode node)
            :base(node)
        {
            Summary = XmlNode.InnerText;
        }

        public string Summary { get; protected set; }
    }
}
