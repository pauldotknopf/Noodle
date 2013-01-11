using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;
using Noodle.Caching;
using Noodle.Configuration;
using Noodle.Data;
using Noodle.Email;
using Noodle.Plugins;
using Noodle.Scheduling;
using Noodle.Security;
using Noodle.Serialization;
using Noodle.Web;

namespace Noodle.Engine
{
    public static class CoreDependencyRegistrar
    {
        public static void Register(IKernel kernel)
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
                kernel.Bind<ConnectionStringSettingsCollection>().ToConstant(WebConfigurationManager.ConnectionStrings);
                kernel.Bind<AppSettings>().ToConstant(new AppSettings(WebConfigurationManager.AppSettings));
            }
            else
            {
                kernel.Bind<ConnectionStringSettingsCollection>().ToConstant(ConfigurationManager.ConnectionStrings);
                kernel.Bind<AppSettings>().ToConstant(new AppSettings(ConfigurationManager.AppSettings));
            }
            kernel.Components.Add<IBindingResolver, AutoStartBindingResolver>(); //hack to get IStartupTasks configuired.
            kernel.Bind<NoodleCoreConfiguration>().ToConstant(coreConfig);
            kernel.Bind<ConfigurationManagerWrapper>().ToConstant(configuration);
            kernel.Bind<ServiceRegistrator>().ToSelf().InSingletonScope();
            kernel.Bind<ITypeFinder>().To<AppDomainTypeFinder>().InSingletonScope();
            kernel.Bind<ISerializer>().To<BinaryStringSerializer>().InSingletonScope();
            kernel.Bind<IEncryptionService>().To<EncryptionService>().InSingletonScope();
            // Adaptive cache will auto switch between http and in-memory cache
            kernel.Bind<ICacheManager>().To<AdaptiveCache>().InSingletonScope();
            kernel.Bind<IRequestContext>().To<AdaptiveContext>();//.InRequestScrope(); TODO
            kernel.Bind<IDateTimeHelper>().To<DateTimeHelper>().InSingletonScope();
            kernel.Bind<IDatabaseService>().To<DatabaseService>().InSingletonScope();
            kernel.Bind<IEmailSender>().To<EmailSender>().InSingletonScope();
            kernel.Bind<IPageTitleBuilder>().To<PageTitleBuilder>();//.InRequestScope TODO
            kernel.Bind<ISecurityManager>().To<DefaultSecurityManager>().InSingletonScope();
            kernel.Bind<IWorker>().To<AsyncWorker>().InSingletonScope();
            kernel.Bind<IPluginBootstrapper>().To<PluginBootstrapper>().InSingletonScope();
            kernel.Bind<IPluginFinder>().To<PluginFinder>().InSingletonScope();
            kernel.Bind<IHeart>().To<Heart>().InSingletonScope();
            kernel.Bind<IErrorNotifier>().To<ErrorNotifier>().InSingletonScope();
            kernel.Bind<IConnectionProvider>().To<ConnectionProvider>().InSingletonScope();
            kernel.Bind<IWebHelper>().ToMethod(context =>
            {
                if (HttpContext.Current == null)
                    throw new InvalidOperationException(
                        "IWebHelper is attempting to be activated by there is no web context (HttpContext.Current == null).");
                return new WebHelper(context.Kernel.Get<IRequestContext>());
            });
            kernel.Bind<HttpContextWrapper>().ToMethod(context =>
            {
                if (HttpContext.Current == null)
                    throw new InvalidOperationException(
                        "HttpContextWrapper is attempting to be activated by there is no web context (HttpContext.Current == null).");

                return new HttpContextWrapper(HttpContext.Current);
            });
        }
    }
}
