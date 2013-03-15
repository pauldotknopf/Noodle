using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Noodle.Configuration;
using Noodle.Documentation;
using Noodle.Engine;
using Noodle.Plugins;
using Noodle.Tests.Helpers;

namespace Noodle.Tests
{
    [TestFixture]
    public class DocumentationTests : TestBase
    {
        private string _xmlDocumentation;
        private IDocumentationService _documentationService;

        public override void SetUp()
        {
            base.SetUp();

            _xmlDocumentation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/SimpleInjector.xml");
            _documentationService = new DocumentationService(
                new PluginFinder(new AppDomainTypeFinder(new FakeAssemblyFinder(new List<Assembly>{typeof(DocumentationMember).Assembly})), 
                new NoodleCoreConfiguration(),
                new TinyIoCContainer()));
        }

        [Test]
        public void Can_get_assembly_name()
        {
            var result = _documentationService.DeserializeDocumentation(_xmlDocumentation);
            result.AssemblyName.ShouldEqual("SimpleInjector");
        }

        [Test]
        public void Can_get_members()
        {
            var result = _documentationService.DeserializeDocumentation(_xmlDocumentation);
            result.Members.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_get_full_name()
        {
            var result = _documentationService.DeserializeDocumentation(_xmlDocumentation);
            var member = result.Members[3];
            member.FullMemberName.ShouldEqual("SimpleInjector.ActivationException.#ctor(System.String,System.Exception)");
            member.MemberName.ShouldEqual("ActivationException");
        }

        [Test]
        public void Can_get_is_constructor()
        {
            var result = _documentationService.DeserializeDocumentation(_xmlDocumentation);
            result.Members[0].IsConstructor.ShouldBeFalse();
            result.Members[1].IsConstructor.ShouldBeTrue();
        }

        [Test]
        public void Can_get_member_infos()
        {
            var result = _documentationService.DeserializeDocumentation(_xmlDocumentation);
            result.Members[0].MemberInfos.Any().ShouldBeTrue();
            result.Members[0].MemberSummary.ShouldNotBeNull();
            result.Members[0].MemberSummary.Summary.ShouldNotBeNull();
        }

        [Test]
        public void Can_get_parameter_types()
        {
            var result = _documentationService.DeserializeDocumentation(_xmlDocumentation);
            result.Members[3].ParameterTypes.Count.ShouldEqual(2);
            result.Members[3].ParameterTypes[0].ShouldEqual("System.String");
            result.Members[3].ParameterTypes[1].ShouldEqual("System.Exception");
        }

        [Test]
        public void Can_get_member_parameters()
        {
            var result = _documentationService.DeserializeDocumentation(_xmlDocumentation);
            result.Members[3].MemberParameters.Count.ShouldEqual(2);
            result.Members[3].MemberParameters[0].ParameterName.ShouldEqual("message");
            result.Members[3].MemberParameters[0].ParameterType.ShouldEqual("System.String");
            result.Members[3].MemberParameters[1].ParameterName.ShouldEqual("innerException");
            result.Members[3].MemberParameters[1].ParameterType.ShouldEqual("System.Exception");
        }

        [Test]
        public void Can_clean_text()
        {
            var result = _documentationService.DeserializeDocumentation(_xmlDocumentation);
            result.Members[0].MemberSummary.Summary.ShouldEqual("The standard exception thrown when a container has an error in resolving an object.");
        }
    }
}
