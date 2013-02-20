using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Noodle.Documentation;

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
            _documentationService = new DocumentationService();
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
    }
}
