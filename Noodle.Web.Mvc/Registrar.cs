using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noodle.Engine;
using Noodle.Web.Mvc.Routes;

namespace Noodle.Web.Mvc
{
    public class Registrar : IDependencyRegistrar
    {
        public void Register(TinyIoCContainer container)
        {
            container.Register<Initialization>();
            container.Register<IRoutePublisher, RoutePublisher>();
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
