using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Noodle.Documentation.BuiltIn
{
    public class DocumentationMemberParameter : DocumentationMemberInfo
    {
        public DocumentationMemberParameter(XmlNode node)
            :base(node)
        {
            var nameAttribute = XmlNode.Attributes != null ? XmlNode.Attributes["name"] : null;
            if(nameAttribute != null)
                ParameterName = CleanText(nameAttribute.Value);
        }

        public string ParameterName { get; protected set; }

        public string ParameterType { get; set; }
    }
}
