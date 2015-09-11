using System;
using System.Linq;
using System.Xml;

namespace Noodle.Extensions.Documentation
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
        public XmlNode XmlNode
        {
            get { return _node; }
        }

        /// <summary>
        /// Get the raw text from the node
        /// </summary>
        public string NodeText
        {
            get { return _node.InnerText; }
        }

        /// <summary>
        /// Gets the InnerText cleaned up of whitespace characters
        /// </summary>
        public string CleanNodeText
        {
            get { return CleanText(_node.InnerText); }
        }

        /// <summary>
        /// Trims the text and removes any beginning or ending tabs or new line breaks
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string CleanText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var trim = Environment.NewLine.ToCharArray().ToList();
            trim.Add(Convert.ToChar(" "));
            return text.TrimEnd(trim.ToArray()).TrimStart(trim.ToArray());
        }
    }
}
