using System;
using System.Configuration;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Configuration;
using Noodle.Caching;
using Noodle.Configuration;
using Noodle.Data;
using Noodle.Data.Deploy;
using Noodle.Email;
using Noodle.Imaging;
using Noodle.Plugins;
using Noodle.Scheduling;
using Noodle.Security;
using Noodle.Serialization;
using Noodle.Web;
using Noodle.TinyIoC;

namespace Noodle.Engine
{
    public static class CoreDependencyRegistrar
    {
        public static void Register(TinyIoC.TinyIoCContainer kernel)
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
                kernel.Register(WebConfigurationManager.ConnectionStrings);
                kernel.Register(new AppSettings(WebConfigurationManager.AppSettings));
            }
            else
            {
                kernel.Register(ConfigurationManager.ConnectionStrings);
                kernel.Register(new AppSettings(ConfigurationManager.AppSettings));
            }
            kernel.Register<IEmbeddedSchemaPlanner, EmbeddedSchemaPlanner>().AsSingleton();
            kernel.Register(coreConfig);
            kernel.Register(configuration);
            kernel.Register<ServiceRegistrator>().AsSingleton();
            kernel.Register<ITypeFinder,AppDomainTypeFinder>().AsSingleton();
            kernel.Register<ISerializer, BinaryStringSerializer>().AsSingleton();
            kernel.Register<IEncryptionService, EncryptionService>();
            // Adaptive cache will auto switch between http and in-memory cache
            kernel.Register<ICacheManager, AdaptiveCache>().AsSingleton();
            kernel.Register<IImageManipulator, ImageManipulator>().AsSingleton();
            kernel.Register<IRequestContext, AdaptiveContext>().AsMultiInstance();
            kernel.Register<IDateTimeHelper, DateTimeHelper>().AsSingleton();
            kernel.Register<IDatabaseService, DatabaseService>().AsMultiInstance();
            kernel.Register<IEmailSender, EmailSender>().AsSingleton();
            kernel.Register<IPageTitleBuilder, PageTitleBuilder>().AsPerRequestSingleton();
            kernel.Register<ISecurityManager, DefaultSecurityManager>().AsSingleton();
            kernel.Register<IWorker, AsyncWorker>().AsSingleton();
            kernel.Register<IPluginBootstrapper, PluginBootstrapper>().AsSingleton();
            kernel.Register<IPluginFinder, PluginFinder>().AsSingleton();
            kernel.Register<IHeart, Heart>().AsSingleton();
            kernel.Register<IErrorNotifier, ErrorNotifier>().AsSingleton();
            kernel.Register<IDeployService, DeployService>().AsSingleton();
            kernel.Register<IConnectionProvider, ConnectionProvider>().AsSingleton();
            kernel.Register((context, namedParams)=>
            {
                if (HttpContext.Current == null)
                    throw new InvalidOperationException(
                        "IWebHelper is attempting to be activated by there is no web context (HttpContext.Current == null).");
                return new WebHelper(context.Resolve<IRequestContext>()) as IWebHelper;
            });
            kernel.Register((context, namedParams) =>
            {
                if (HttpContext.Current == null)
                    throw new InvalidOperationException(
                        "HttpContextWrapper is attempting to be activated by there is no web context (HttpContext.Current == null).");

                return new HttpContextWrapper(HttpContext.Current);
            });
        }
    }
}
