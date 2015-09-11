using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noodle.Engine;

namespace Noodle.Extensions
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(TinyIoCContainer container)
        {
            container.Register(typeof(Serialization.ISerializer), typeof(Serialization.BinaryStringSerializer));
            container.Register(typeof(Security.IEncryptionService), typeof(Security.EncryptionService));
            container.Register(typeof(Data.IDatabaseService), typeof(Data.DatabaseService));
            container.Register(typeof(Email.IEmailSender), typeof(Email.EmailSender));
            container.Register(typeof(Security.ISecurityManager), typeof(Security.DefaultSecurityManager));
            container.Register(typeof(Plugins.IPluginBootstrapper), typeof(Plugins.PluginBootstrapper));
            container.Register(typeof(Plugins.IPluginFinder), typeof(Plugins.PluginFinder));
            container.Register(typeof(Scheduling.IHeart), typeof(Scheduling.Heart));
            container.Register(typeof(Data.IConnectionProvider), typeof(Data.NoConnectionProvider));
            container.Register(typeof(Caching.ICacheManager), typeof(Caching.InMemoryCache));
            container.Register(typeof(Scheduling.Scheduler));
            container.Register(typeof(Documentation.IDocumentationService), typeof(Documentation.DocumentationService));
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
