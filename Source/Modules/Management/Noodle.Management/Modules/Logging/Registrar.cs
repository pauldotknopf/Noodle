using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Noodle.Engine;
using Noodle.Management.Library.Navigation;
using Noodle.Management.Logging.Controllers;

namespace Noodle.Management.Logging
{
    public class Registrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register your services with the container. You are given a type finder to help you find anything you need.
        /// </summary>
        /// <param name="container"></param>
        public void Register(TinyIoCContainer container)
        {
            container.Register<INoodleSiteMapNodeProvider, SiteMap>();
            container.Register<LogController>();
        }

        /// <summary>
        /// The lower numbers will be registered first. Higher numbers the latest.
        /// If you are registering fakes, give a high integer (int.Max ?) so that that it will be registered last,
        /// and the container will use it instead of the previously registered services.
        /// </summary>
        public int Importance
        {
            get { return 0; }
        }
    }
}