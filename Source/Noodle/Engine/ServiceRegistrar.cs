using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noodle.TinyIoC;

namespace Noodle.Engine
{
    /// <summary>
    /// Registers services in the Noodle container (ninject) upon start.
    /// </summary>
    public class ServiceRegistrator
    {
        readonly ITypeFinder finder;

        public ServiceRegistrator(ITypeFinder finder)
        {
            this.finder = finder;
        }

        public virtual IEnumerable<AttributeInfo<ServiceAttribute>> FindServices()
        {
            foreach (var type in finder.Find(typeof(object)))
            {
                var attributes = type.GetCustomAttributes(typeof(ServiceAttribute), false);
                foreach (ServiceAttribute attribute in attributes)
                {
                    yield return new AttributeInfo<ServiceAttribute> { Attribute = attribute, DecoratedType = type };
                }
            }
        }

        public virtual void RegisterServices(TinyIoC.TinyIoCContainer kernel)
        {
            var allServices = FindServices().ToList();
            var replacementServices = allServices.Where(s => s.Attribute.Replaces != null).Select(s => s.Attribute.Replaces).ToList();
            foreach (var info in allServices.Where(s => !replacementServices.Contains(s.DecoratedType)))
            {
                Type serviceType = info.Attribute.ServiceType ?? info.DecoratedType;
                string key = info.Attribute.Key ?? info.DecoratedType.FullName;
                if (string.IsNullOrEmpty(info.Attribute.StaticAccessor))
                {
                    ConfigureScope(kernel.Register(serviceType, info.DecoratedType, key), info.Attribute.ContainerScope);
                }
                else
                {
                    var pi = info.DecoratedType.GetProperty(info.Attribute.StaticAccessor, BindingFlags.Public | BindingFlags.Static);
                    if (pi == null) throw new InvalidOperationException("[Service(StaticAccessor = \"" + info.Attribute.StaticAccessor + "\")] on " + info.DecoratedType + " doesn't match an existing static property on that type. Add a static property or remove the static accessor declaration.");
                    var instance = pi.GetValue(null, null);
                    if (instance == null) new InvalidOperationException("[Service(StaticAccessor = \"" + info.Attribute.StaticAccessor + "\")] on " + info.DecoratedType + " defines a property that returned null. Make sure this static property returns a value.");
                    if (!serviceType.IsInstanceOfType(instance)) new InvalidOperationException("[Service(StaticAccessor = \"" + info.Attribute.StaticAccessor + "\")] on " + info.DecoratedType + " defines a property that returned an invalid type. The returned object must be assignable to " + serviceType);
                    kernel.Register(serviceType, instance, key);
                }
                
            }
        }

        private void ConfigureScope(TinyIoCContainer.RegisterOptions binding, ContainerScopeEnum scope)
        {
            switch(scope)
            {
                case ContainerScopeEnum.PerRequest:
                    binding.AsPerRequestSingleton();
                    break;
                case ContainerScopeEnum.Singleton:
                    binding.AsSingleton();
                    break;
                case ContainerScopeEnum.Thread:
                    // todo
                    //binding.AsPerThreadSingleton();
                    break;
                case ContainerScopeEnum.Transient:
                    binding.AsMultiInstance();
                    break;
            }
        }
    }
}
