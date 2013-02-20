using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Noodle.Documentation
{
    public class DocumentationMember
    {
        private readonly XmlNode _memberNode;

        public DocumentationMember(XmlNode memberNode)
        {
            _memberNode = memberNode;

            var nameAttribute = _memberNode.Attributes["name"];
            if(nameAttribute == null) throw new InvalidOperationException("The member doesn't have a name attribute");

            switch (nameAttribute.Value.Substring(0, 2))
            {
                case "T:":
                    Type = DocumentationMemberType.Type;
                    break;
                case "M:":
                    Type = DocumentationMemberType.Member;
                    break;
                case "P:":
                    Type = DocumentationMemberType.Property;
                    break;
                case "E:":
                    Type = DocumentationMemberType.Exception;
                    break;
                default:
                    throw new InvalidOperationException("Unknown member type " + nameAttribute.Value.Substring(0, 2));
            }

            FullMemberName = nameAttribute.Value.Substring(2);
            MemberName = Regex.Match(FullMemberName, @"\.[\w]+(.#ctor)?(\([\w\.\,]*\))?").Value.Substring(1);
            if (MemberName.IndexOf(".#ctor", StringComparison.Ordinal) > 0)
                MemberName = MemberName.Substring(0, MemberName.IndexOf(".#ctor", StringComparison.Ordinal));
            if (MemberName.IndexOf("(", StringComparison.Ordinal) > 0)
                MemberName = MemberName.Substring(0, MemberName.IndexOf("(", StringComparison.Ordinal));
        }

        public string MemberName { get; protected set; }

        public string FullMemberName { get; protected set; }

        public DocumentationMemberType Type { get; protected set; }
    }
}
