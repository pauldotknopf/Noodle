using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Noodle.Documentation.BuiltIn
{
    public class DocumentationMemberValue : DocumentationMemberInfo
    {
        public DocumentationMemberValue(XmlNode node)
            :base(node)
        {
            
        }
    }
}
