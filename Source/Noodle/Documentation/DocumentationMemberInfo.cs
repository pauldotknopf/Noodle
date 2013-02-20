using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Noodle.Documentation
{
    public abstract class DocumentationMemberInfo
    {
        private readonly XmlNode _node;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationMemberInfo"/> class.
        /// </summary>
        protected DocumentationMemberInfo(XmlNode node)
        {
            _node = node;
            NodeName = _node.Name;
        }

        /// <summary>
        /// summary? parameter? value? example?
        /// </summary>
        public string NodeName { get; protected set; }

        /// <summary>
        /// The raw xmlnode for the member info
        /// </summary>
        public XmlNode XmlNode { get { return _node; } }

        /// <summary>
        /// Get the raw text from the node
        /// </summary>
        public string NodeText { get { return _node.InnerText; } }
    }
}
