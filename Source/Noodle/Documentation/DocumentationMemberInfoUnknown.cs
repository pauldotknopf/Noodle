using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Noodle.Documentation
{
    /// <summary>
    /// These member infos are used if no plugins can build one for the node.
    /// This node could be anything. Maybe custom nodes you created? Who knows? Not me.
    /// </summary>
    public class DocumentationMemberInfoUnknown : DocumentationMemberInfo
    {
        public DocumentationMemberInfoUnknown(XmlNode node)
            :base(node)
        {
            
        }
    }
}
