using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using Noodle.Caching;
using Noodle.Configuration;
using Noodle.Data;
using Noodle.Email;
using Noodle.Plugins;
using Noodle.Scheduling;
using Noodle.Security;
using Noodle.Serialization;
using Noodle.Web;
using SimpleInjector;

namespace Noodle.Engine
{
    public static class CoreDependencyRegistrar
    {
        public static void Register(Container container)
        {
            var configuration = new ConfigurationManagerWrapper();
            NoodleCoreConfiguration coreConfig = null;
            try
            {
                coreConfig = configuration.GetSection<NoodleCoreConfiguration>("core");
            }
            catch (ConfigurationErrorsException ex)
            {
                if (ex.Message.Contains("Missing configuration section at 'core'"))
                {
                    coreConfig = new NoodleCoreConfiguration();
                }
                else
                {
                    throw;
                }
            }
            if (WebConfigurationManager.ConnectionStrings != null)
            {
                container.RegisterSingle(WebConfigurationManager.ConnectionStrings);
                container.RegisterSingle(new AppSettings(WebConfigurationManager.AppSettings));
            }
            else
            {
                container.RegisterSingle(ConfigurationManager.ConnectionStrings);
                container.RegisterSingle(new AppSettings(ConfigurationManager.AppSettings));
            }
            container.RegisterSingle(coreConfig);
            container.RegisterSingle(configuration);
            container.RegisterSingle<ITypeFinder, AppDomainTypeFinder>();
            container.RegisterSingle<IAssemblyFinder, AssemblyFinder>();
            container.RegisterSingle<ISerializer, BinaryStringSerializer>();
            container.RegisterSingle<IEncryptionService, EncryptionService>();
            container.RegisterSingle<ICacheManager, AdaptiveCache>();
            container.RegisterSingle<IRequestContext, AdaptiveContext>();
            container.RegisterSingle<IDateTimeHelper, DateTimeHelper>();
            container.RegisterSingle<IDatabaseService, DatabaseService>();
            container.RegisterSingle<IEmailSender, EmailSender>();
            container.RegisterPerWebRequest<IPageTitleBuilder, PageTitleBuilder>();
            container.RegisterSingle<ISecurityManager, DefaultSecurityManager>();
            container.RegisterSingle<IWorker, AsyncWorker>();
            container.RegisterSingle<IPluginBootstrapper, PluginBootstrapper>();
            container.RegisterSingle<IPluginFinder, PluginFinder>();
            container.RegisterSingle<IHeart, Heart>();
            container.RegisterSingle<IErrorNotifier, ErrorNotifier>();
            container.RegisterSingle<IConnectionProvider, ConnectionProvider>();
            container.RegisterSingle<Resources.EmbeddedResourceHandler>();
            container.RegisterSingle<Resources.RegisterStartup>();
            container.Register(() => EventBroker.Instance);
            container.RegisterSingle<Scheduler>();
            container.RegisterInitializer<IStartupTask>(task => task.Execute());
            container.RegisterPerWebRequest(() =>
            {
                if (HttpContext.Current == null)
                    throw new InvalidOperationException(
                        "IWebHelper is attempting to be activated by there is no web context (HttpContext.Current == null).");
                return new WebHelper(container.GetInstance<IRequestContext>());
            });
            container.RegisterPerWebRequest(() =>
            {
                if (HttpContext.Current == null)
                    throw new InvalidOperationException(
                        "HttpContextWrapper is attempting to be activated by there is no web context (HttpContext.Current == null).");

                return new HttpContextWrapper(HttpContext.Current);
            });
        }
    }
}
