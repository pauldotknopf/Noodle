using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Noodle.Caching;
using Noodle.Engine;
using Noodle.Resources;
using Noodle.Web.Caching;
using Noodle.Web.Resources;

namespace Noodle.Web
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(TinyIoCContainer container)
        {
            container.Register<ICacheManager, AdaptiveCache>();
            container.Register<IPageTitleBuilder, PageTitleBuilder>().AsPerRequestSingleton();
            container.Register<HttpContextWrapper>().AsPerRequestSingleton();
            container.Register<EmbeddedResourceHandler>().AsSingleton();
            container.Register<RegisterStartup>().AsSingleton();
            container.Register((context, p) => HttpContext.Current);
            container.Register((context, p) => EventBroker.Instance);
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
