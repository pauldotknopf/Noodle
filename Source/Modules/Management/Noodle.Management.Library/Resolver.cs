using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noodle.Engine;
using Noodle.Management.Library.Controllers;
using Noodle.Web;

namespace Noodle.Management.Library
{
    public class Resolver : IDependencyRegistrar
    {
        public void Register(TinyIoCContainer container)
        {
            container.Register<Initialization>();

            container.Register<DefaultController>().AsPerRequestSingleton();
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
