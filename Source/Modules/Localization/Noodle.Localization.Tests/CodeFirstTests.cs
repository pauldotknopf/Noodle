using NUnit.Framework;
using Noodle.Localization.CodeFirst;
using Noodle.Tests;

namespace Noodle.Localization.Tests
{
    [TestFixture]
    public class CodeFirstTests
    {
        [Test]
        public void Can_resolve_resource()
        {
            var expressionVisitor = new LocalizationNodeExpressionVisitor(() => Products.Value);
            expressionVisitor.ResourceName.ShouldEqual("Products");
            expressionVisitor.DefaultValue.ShouldEqual("Products value");

            expressionVisitor = new LocalizationNodeExpressionVisitor(() => Products.Categories.Edit);
            expressionVisitor.ResourceName.ShouldEqual("Products.Categories.Edit");
            expressionVisitor.DefaultValue.ShouldEqual("Edit category value");

            expressionVisitor = new LocalizationNodeExpressionVisitor(() => LoginAdditional.RememberMe);
            expressionVisitor.ResourceName.ShouldEqual("Login.Additional.RememberMe");
            expressionVisitor.DefaultValue.ShouldEqual("Remember me");

            expressionVisitor = new LocalizationNodeExpressionVisitor(() => Login.RememberMe);
            expressionVisitor.ResourceName.ShouldEqual("Resources.Security.Login.RememberMe");
            expressionVisitor.DefaultValue.ShouldEqual("Remember me");
        }
    }

    [ResourceNamespace("Login.Additional")]
    public class LoginAdditional
    {
        public static string RememberMe = "Remember me";
    }

    [ResourceNamespace("Resources.Security.Login")]
    public class Login
    {
        public static string RememberMe = "Remember me";
    }

    public class Products
    {
        public static string Value = "Products value";
        
        public class Categories
        {
            public static string Value = "Categories value";
            public static string Edit = "Edit category value";
        }
    }
}
