using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using Noodle.Caching;
using Noodle.Configuration;
using Noodle.Data;
using Noodle.Documentation;
using Noodle.Email;
using Noodle.Imaging;
using Noodle.Plugins;
using Noodle.Scheduling;
using Noodle.Security;
using Noodle.Serialization;
using Noodle.Web;

namespace Noodle.Engine
{
    public static class CoreDependencyRegistrar
    {
        public static void Register(TinyIoCContainer container)
        {
            if (WebConfigurationManager.ConnectionStrings != null)
            {
                container.Register(WebConfigurationManager.ConnectionStrings);
                container.Register(new AppSettings(WebConfigurationManager.AppSettings));
            }
            else
            {
                container.Register(ConfigurationManager.ConnectionStrings);
                container.Register(new AppSettings(ConfigurationManager.AppSettings));
            }
            container.Register(EngineContext.ConfigurationGroupManager().GetSection<NoodleCoreConfiguration>("core", true));

            container.Register(EngineContext.ConfigurationGroupManager());
            container.Register<IWorker, AsyncWorker>();
            container.Register<ITypeFinder, AppDomainTypeFinder>();
            container.Register<IAssemblyFinder, AssemblyFinder>();
            container.Register<ISerializer, BinaryStringSerializer>();
            container.Register<IEncryptionService, EncryptionService>();
            container.Register<ICacheManager, AdaptiveCache>();
            container.Register<IRequestContext, AdaptiveContext>();
            container.Register<IDateTimeHelper, DateTimeHelper>();
            container.Register<IDatabaseService, DatabaseService>();
            container.Register<IEmailSender, EmailSender>();
            container.Register<IPageTitleBuilder, PageTitleBuilder>().AsPerRequestSingleton();
            container.Register<IImageManipulator, NoodleImageManipulator>();
            container.Register<IImageLayoutBuilder, ImageLayoutBuilder>();
            container.Register<ISecurityManager, DefaultSecurityManager>();
            container.Register<IPluginBootstrapper, PluginBootstrapper>();
            container.Register<IPluginFinder, PluginFinder>();
            container.Register<IHeart, Heart>();
            container.Register<IErrorNotifier, ErrorNotifier>();
            container.Register<IConnectionProvider, ConnectionProvider>();
            container.Register<Resources.EmbeddedResourceHandler>().AsSingleton();
            container.Register<Resources.RegisterStartup>().AsSingleton();
            container.Register((context, p) => EventBroker.Instance);
            container.Register<Scheduler>();
            container.Register<IDocumentationService, DocumentationService>();
            container.Register((context, p) => HttpContext.Current);
            container.Register<IWebHelper, WebHelper>().AsPerRequestSingleton();
            container.Register<HttpContextWrapper>().AsPerRequestSingleton();
        }
    }
}
