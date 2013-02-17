using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Noodle.Reflection;

namespace Noodle.Configuration
{
    /// <summary>
    /// Builds a configuration section from xml
    /// </summary>
    public class ConfigurationSectionBuilder : IConfigurationSectionBuilder
    {
        /// <summary>
        /// Build a configuration section from the given xml. This is useful for storing configuration in a database somewhere.
        /// </summary>
        /// <typeparam name="TSection"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public TSection BuildSection<TSection>(string xml) where TSection : System.Configuration.ConfigurationSection
        {
            var myEncoder = new ASCIIEncoding();
            byte[] bytes = myEncoder.GetBytes(xml);
            var ms = new MemoryStream(bytes);
            var xmlReader = XmlReader.Create(ms);

            var section = Activator.CreateInstance(typeof(TSection)) as TSection;
            var deserializeMethod = Private.Method<TSection>("DeserializeSection");
            try
            {
                deserializeMethod.Invoke(section, new object[] { xmlReader });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            return section;
        }
    }
}
