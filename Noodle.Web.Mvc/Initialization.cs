using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Noodle.Web.Mvc.Routes;

namespace Noodle.Web.Mvc
{
    /// <summary>
    /// Initialize the things related to noodle/mvc
    /// </summary>
    public class Initialization : IStartupTask
    {
        private readonly TinyIoCContainer _container;
        private readonly IRoutePublisher _routePublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="routePublisher">The route publisher.</param>
        public Initialization(TinyIoCContainer container, IRoutePublisher routePublisher)
        {
            _container = container;
            _routePublisher = routePublisher;
        }

        /// <summary>
        /// Excute is run once on startup of the application
        /// </summary>
        public void Execute()
        {
            DependencyResolver.SetResolver(new NoodleDependencyResolver(_container));
            _routePublisher.RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// The order at which the startup task will run. Smaller numbers run first.
        /// </summary>
        public int Order
        {
            get { return 0; }
        }
    }
}
