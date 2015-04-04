using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Noodle.Documentation.BuiltIn;

namespace Noodle.Documentation
{
    public class DocumentationMember
    {
        private readonly XmlNode _memberNode;

        public DocumentationMember(XmlNode memberNode, IDocumentationService documentationService)
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
                    Type = DocumentationMemberType.Event;
                    break;
                case "F:":
                    Type = DocumentationMemberType.Field;
                    break;
                default:
                    throw new InvalidOperationException("Unknown member type " + nameAttribute.Value.Substring(0, 2));
            }

            FullMemberName = nameAttribute.Value.Substring(2);
            var memberNameMatch = Regex.Match(FullMemberName, @"\.[\w\`\+\#\@]+(.#ctor)?(\([\w\.\,\`\+\{\}\[\]\#\@]*\))?$").Value;
            if (string.IsNullOrEmpty(memberNameMatch))
                throw new Exception("Couldn't get member name");
            MemberName = memberNameMatch.Substring(1);
            if (MemberName.IndexOf(".#ctor", StringComparison.Ordinal) > 0)
                MemberName = MemberName.Substring(0, MemberName.IndexOf(".#ctor", StringComparison.Ordinal));
            if (MemberName.IndexOf("(", StringComparison.Ordinal) > 0)
                MemberName = MemberName.Substring(0, MemberName.IndexOf("(", StringComparison.Ordinal));

            IsConstructor = FullMemberName.Contains(".#ctor");

            ParameterTypes = new List<string>();
            if(Type == DocumentationMemberType.Member)
            {
                var parameterTypesMatch = Regex.Match(FullMemberName, @"\([\w\.\,]*\)$");
                if (parameterTypesMatch.Success)
                {
                    var parameterTypesRaw = parameterTypesMatch.Value.Substring(1);
                    parameterTypesRaw = parameterTypesRaw.Substring(0, parameterTypesRaw.Length - 1);
                    foreach(var parameterType in parameterTypesRaw.Split(Convert.ToChar(",")))
                    {
                        ParameterTypes.Add(parameterType);
                    }
                }  
            }

            MemberInfos = new List<DocumentationMemberInfo>();
            MemberInfos.AddRange(documentationService.GetInfosForMember(memberNode));
            MemberSummary = MemberInfos.OfType<DocumentationMemberSummary>().FirstOrDefault();
            MemberReturns = MemberInfos.OfType<DocumentationMemberReturns>().FirstOrDefault();
            MemberValue = MemberInfos.OfType<DocumentationMemberValue>().FirstOrDefault();
            MemberParameters = MemberInfos.OfType<DocumentationMemberParameter>().ToList();

            foreach (var memberParameter in MemberParameters)
            {
                var memberParameterIndex = MemberParameters.IndexOf(memberParameter);
                if((ParameterTypes.Count - 1) >= memberParameterIndex)
                {
                    memberParameter.ParameterType = memberParameter.CleanText(ParameterTypes[memberParameterIndex]);
                }
            }
        }

        public string MemberName { get; protected set; }

        public string FullMemberName { get; protected set; }

        public DocumentationMemberType Type { get; protected set; }

        public bool IsConstructor { get; protected set; }

        public List<DocumentationMemberInfo> MemberInfos { get; protected set; }

        public DocumentationMemberSummary MemberSummary { get; protected set; }

        public DocumentationMemberValue MemberValue { get; protected set; }

        public DocumentationMemberReturns MemberReturns { get; protected set; }

        public List<string> ParameterTypes { get; protected set; }

        public List<DocumentationMemberParameter> MemberParameters { get; protected set; }
    }
}
