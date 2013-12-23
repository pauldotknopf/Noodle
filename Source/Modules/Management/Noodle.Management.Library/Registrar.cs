using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Builder;
using MvcSiteMapProvider.Caching;
using MvcSiteMapProvider.Collections.Specialized;
using MvcSiteMapProvider.Globalization;
using MvcSiteMapProvider.Loader;
using MvcSiteMapProvider.Security;
using MvcSiteMapProvider.Visitor;
using MvcSiteMapProvider.Web.Compilation;
using MvcSiteMapProvider.Web.Mvc;
using MvcSiteMapProvider.Web.Mvc.Filters;
using MvcSiteMapProvider.Xml;
using Noodle.Engine;
using Noodle.Management.Library.Controllers;
using Noodle.Management.Library.Navigation;
using Noodle.Web;

namespace Noodle.Management.Library
{
    public class Registrar : IDependencyRegistrar
    {
        public void Register(TinyIoCContainer container)
        {
            container.Register<IAclModule>((c, overloads) => new CompositeAclModule(new AuthorizeAttributeAclModule(c.Resolve<IMvcContextFactory>(),
                c.Resolve<IControllerDescriptorFactory>(),
                c.Resolve<IControllerBuilder>(),
                c.Resolve<IGlobalFilterProvider>()),
                new XmlRolesAclModule(c.Resolve<IMvcContextFactory>())));
            container.Register<IMvcContextFactory, MvcContextFactory>();
            container.Register<IControllerDescriptorFactory, ControllerDescriptorFactory>();
            container.Register((c, overloads) => ControllerBuilder.Current);
            container.Register<IControllerBuilder>((c, overloads) => new ControllerBuilderAdaptor(c.Resolve<ControllerBuilder>()));
            container.Register<IGlobalFilterProvider, GlobalFilterProvider>();
            container.Register<ISiteMapNodeVisibilityProviderStrategy>((c, overloads) =>
                                                                       new SiteMapNodeVisibilityProviderStrategy(
                                                                           c.ResolveAll<ISiteMapNodeVisibilityProvider>().ToArray(), 
                                                                           string.Empty));
            container.Register<IBuildManager, BuildManagerAdaptor>();
            container.Register<IControllerTypeResolverFactory>((c, overloads) =>
                                                              new ControllerTypeResolverFactory(new string[0],
                                                                                                c.Resolve<IControllerBuilder>(),
                                                                                                c.Resolve<IBuildManager>()));
            container.Register<System.Runtime.Caching.ObjectCache>((c, overloads) => System.Runtime.Caching.MemoryCache.Default);
            container.Register<ICacheDetails>((c, overloads) => new CacheDetails(TimeSpan.FromMinutes(5), TimeSpan.MinValue, new NullCacheDependency()));
            container.Register(typeof (ICacheProvider<>), typeof (RuntimeCacheProvider<>));
            container.Register<FluentSiteMapNodeProvider>().AsSingleton();
            container.Register((c, overloads) =>
                    c.Resolve<SiteMapBuilderFactory>()
                        .Create(new CompositeSiteMapNodeProvider(c.Resolve<FluentSiteMapNodeProvider>())));
            container.Register<ISiteMapLoader, SiteMapLoader>();
            container.Register<ISiteMapCache, SiteMapCache>();
            container.Register<ISiteMapCacheKeyGenerator, SiteMapCacheKeyGenerator>();
            container.Register<ISiteMapCreator, SiteMapCreator>();
            container.Register<ISiteMapCacheKeyToBuilderSetMapper, SiteMapCacheKeyToBuilderSetMapper>();
            container.Register<ISiteMapBuilderSet>((c, overloads) => new SiteMapBuilderSet("Noodle", false, false, c.Resolve<ISiteMapBuilder>(), null));
            container.Register<ISiteMapBuilderSetStrategy>((c, overloads) => new SiteMapBuilderSetStrategy(c.ResolveAll<ISiteMapBuilderSet>().ToArray()));
            container.Register<ISiteMapNodeVisitor, UrlResolvingSiteMapNodeVisitor>();
            container.Register<ISiteMapHierarchyBuilder, SiteMapHierarchyBuilder>();
            container.Register<ISiteMapNodeHelperFactory, SiteMapNodeHelperFactory>();
            container.Register<ISiteMapNodeCreatorFactory, SiteMapNodeCreatorFactory>();
            container.Register<ISiteMapNodeChildStateFactory, SiteMapNodeChildStateFactory>();
            container.Register<ISiteMapNodeFactory, SiteMapNodeFactory>();
            container.Register<IAttributeDictionaryFactory, AttributeDictionaryFactory>();
            container.Register<IRequestCache, RequestCache>();
            container.Register<IRouteValueDictionaryFactory, RouteValueDictionaryFactory>();
            container.Register<ILocalizationServiceFactory, LocalizationServiceFactory>();
            container.Register<IExplicitResourceKeyParser, ExplicitResourceKeyParser>();
            container.Register<ISiteMapNodePluginProvider, SiteMapNodePluginProvider>();
            container.Register<IDynamicNodeProviderStrategy, DynamicNodeProviderStrategy>();
            container.Register<IStringLocalizer, StringLocalizer>();
            container.Register<Initialization>();
            container.Register<DefaultController>().AsPerRequestSingleton();
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
