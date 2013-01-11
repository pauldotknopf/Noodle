using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Noodle.Configuration;
using Noodle.Engine;
using Noodle.Plugins;
using Noodle.Security;

namespace Noodle.Tests.Plugins
{
    public class TestPluginAttribute : Attribute, IPlugin, ISecurable, IPermittable
    {
        public string Name { get; set; }

        public Type Decorates { get; set; }

        public int SortOrder { get; set; }

        public int CompareTo(IPlugin other)
        {
            if (other == null)
                return 1;
            var result = SortOrder.CompareTo(other.SortOrder) * 2 + String.CompareOrdinal(Name, other.Name);
            return result;
        }

        public string[] RequiredRoles { get; set; }
        public string[] RequiredPermissions { get; set; }
    }

    [TestPlugin(SortOrder = 30, Name = "Plugin 1")]
    public class TestPlugin1
    {
    }

    [TestPlugin(SortOrder = 20, Name = "Plugin 2")]
    public class TestPlugin2
    {
    }

    [TestPlugin(SortOrder = 40, Name = "Plugin 3", RequiredRoles = new[] { "Admin" })]
    public class TestPlugin3Securable
    {
    }

    [TestPlugin(SortOrder = 50, Name = "Plugin 4", RequiredPermissions = new[] { "Edit" })]
    public class TestPlugin4Securable
    {
    }

    [TestClass]
    public class PluginFinderTests
    {
        readonly PluginFinder _finder;

        public PluginFinderTests()
        {
            var edit = new NoodleCoreConfiguration();
            var kernel = new StandardKernel();
            kernel.Bind<ISecurityManager>().To<FakeSecurityManager>();
            _finder = new PluginFinder(new AppDomainTypeFinder(), edit, kernel);
        }

        [TestMethod]
        public void Can_get_plugins()
        {
            var plugins = _finder.GetPlugins<TestPluginAttribute>();
            plugins.Count().ShouldEqual(4);
        }

        [TestMethod]
        public void Can_sort_plugins()
        {
            var plugins = _finder.GetPlugins<TestPluginAttribute>().ToList();
            plugins.Count.ShouldEqual(4);

            plugins[0].Name.ShouldEqual("Plugin 2");
            plugins[1].Name.ShouldEqual("Plugin 1");
            plugins[2].Name.ShouldEqual("Plugin 3");
            plugins[3].Name.ShouldEqual("Plugin 4");
        }

        [TestMethod]
        public void Can_get_securable_plugins_hidden_for_anonymous()
        {
            var plugins = _finder.GetPlugins<TestPluginAttribute>(new FakePrincipal(false)).ToList();

            plugins.Count.ShouldEqual(2);
            plugins[0].Name.ShouldEqual("Plugin 2");
            plugins[1].Name.ShouldEqual("Plugin 1");
        }

        [TestMethod]
        public void Can_get_securable_plugins_hidden_for_registered()
        {
            var plugins = _finder.GetPlugins<TestPluginAttribute>(new FakePrincipal(true)).ToList();

            plugins.Count.ShouldEqual(2);
            plugins[0].Name.ShouldEqual("Plugin 2");
            plugins[1].Name.ShouldEqual("Plugin 1");
        }

        [TestMethod]
        public void Can_get_securable_plugins_hidden_for_registered_with_permission()
        {
            var plugins = _finder.GetPlugins<TestPluginAttribute>(new FakePrincipal(true, permissions: new[] { "Edit" })).ToList();

            plugins.Count.ShouldEqual(3);

            plugins[0].Name.ShouldEqual("Plugin 2");
            plugins[1].Name.ShouldEqual("Plugin 1");
            plugins[2].Name.ShouldEqual("Plugin 4");
        }

        [TestMethod]
        public void Can_get_securable_plugins_hidden_for_registered_with_role()
        {
            var plugins = _finder.GetPlugins<TestPluginAttribute>(new FakePrincipal(true, roles: new[] { "Admin" })).ToList();

            plugins.Count.ShouldEqual(3);

            plugins[0].Name.ShouldEqual("Plugin 2");
            plugins[1].Name.ShouldEqual("Plugin 1");
            plugins[2].Name.ShouldEqual("Plugin 3");
        }


        [TestMethod]
        public void Can_remove_plugins_through_configuration()
        {
            int initialCount = _finder.GetPlugins<TestPluginAttribute>().Count();
            var configuration = new NoodleCoreConfiguration() { Plugins = new PluginCollection() };
            configuration.Plugins.Remove(new PluginElement { Name = "Plugin 2" });
            var newfinder = new PluginFinder(new AppDomainTypeFinder(), configuration, null);
            
            var plugins = newfinder.GetPlugins<TestPluginAttribute>().ToList();

            plugins.Count.ShouldEqual(initialCount - 1);
        }
    }
}
