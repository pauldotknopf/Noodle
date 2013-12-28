using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Noodle.Web.Mvc
{
    /// <summary>
    /// Initialize the things related to noodle/mvc
    /// </summary>
    public class Initialization : IStartupTask
    {
        private readonly TinyIoCContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public Initialization(TinyIoCContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Excute is run once on startup of the application
        /// </summary>
        public void Execute()
        {
            DependencyResolver.SetResolver(new NoodleDependencyResolver(_container));
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
