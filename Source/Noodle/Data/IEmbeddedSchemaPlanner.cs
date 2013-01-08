using System.Collections.Generic;

namespace Noodle.Data
{
    /// <summary>
    /// Gets plan for installing an embedded schema
    /// </summary>
    public interface IEmbeddedSchemaPlanner
    {
        /// <summary>
        /// Get plans for an instance of a provideer
        /// </summary>
        /// <param name="schemaProvider"></param>
        /// <returns></returns>
        List<AbstraceEmbeddedSchemaProvider> GetPlansFor(AbstraceEmbeddedSchemaProvider schemaProvider);

        /// <summary>
        /// Get plans for an attibute decorated on a provider.
        /// </summary>
        /// <param name="schemaProviderAttribute"></param>
        /// <returns></returns>
        List<EmbeddedSchemaProviderAttribute> GetPlansFor(EmbeddedSchemaProviderAttribute schemaProviderAttribute);
    }
}
