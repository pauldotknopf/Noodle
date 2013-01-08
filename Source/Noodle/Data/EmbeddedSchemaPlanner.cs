using System;
using System.Collections.Generic;
using System.Linq;

namespace Noodle.Data
{
    /// <summary>
    /// Gets plan for installing an embedded schema
    /// </summary>
    public class EmbeddedSchemaPlanner : IEmbeddedSchemaPlanner, IEqualityComparer<AbstraceEmbeddedSchemaProvider>
    {
        /// <summary>
        /// Get plans for an instance of a provideer
        /// </summary>
        /// <param name="schemaProvider"></param>
        /// <returns></returns>
        public List<AbstraceEmbeddedSchemaProvider> GetPlansFor(AbstraceEmbeddedSchemaProvider schemaProvider)
        {
            var schemaProviders = new List<AbstraceEmbeddedSchemaProvider>();
            try
            {
                RecursivelyResolveDependencies(schemaProvider, schemaProviders);
            }
            catch(StackOverflowException ex)
            {
                throw new Exception("Stack overflow has happened in the schema planner. This may be because of a cyclical embedded schema provider reference.", ex);
            }
            schemaProviders = schemaProviders.Distinct().ToList();
            return schemaProviders;
        }

        /// <summary>
        /// Get plans for an attibute decorated on a provider.
        /// </summary>
        /// <param name="schemaProviderAttribute"></param>
        /// <returns></returns>
        private void RecursivelyResolveDependencies(AbstraceEmbeddedSchemaProvider schemaProvider, List<AbstraceEmbeddedSchemaProvider> schemaProviders)
        {
            schemaProviders.Insert(0, schemaProvider);
            var dependencies = schemaProvider.GetDependentSchemaProviders();
            dependencies.Reverse();
            foreach (var dependency in dependencies)
            {
                RecursivelyResolveDependencies(dependency, schemaProviders);
            }
        }

        public List<EmbeddedSchemaProviderAttribute> GetPlansFor(EmbeddedSchemaProviderAttribute schemaProviderAttribute)
        {
            var schemaProvider = Activator.CreateInstance(schemaProviderAttribute.Decorates) as AbstraceEmbeddedSchemaProvider;
            var plans = GetPlansFor(schemaProvider);
            var planAttributes = new List<EmbeddedSchemaProviderAttribute>();
            foreach(var plan in plans)
            {
                var planAttribute = plan.GetType().GetCustomAttributes(true).OfType<EmbeddedSchemaProviderAttribute>().First();
                planAttribute.Decorates = plan.GetType();
                if (planAttribute.Name == null)
                    planAttribute.Name = planAttribute.Decorates.Name;
                planAttributes.Add(planAttribute);
            }
            return planAttributes;
        }

        public bool Equals(AbstraceEmbeddedSchemaProvider x, AbstraceEmbeddedSchemaProvider y)
        {
            return x.GetType().FullName.Equals(y.GetType().FullName, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(AbstraceEmbeddedSchemaProvider obj)
        {
            return obj.GetType().FullName.GetHashCode();
        }
    }
}
