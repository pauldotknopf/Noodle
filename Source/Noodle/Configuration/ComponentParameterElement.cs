using System.Configuration;

namespace Noodle.Configuration
{
    public class ComponentParameterElement : NamedElement
    {
        /// <summary>The value of the property to set.</summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }

        [ConfigurationProperty("parameterType", IsRequired = false, DefaultValue = ParameterTypeEnum.Constructor)]
        public ParameterTypeEnum ParameterType
        {
            get { return (ParameterTypeEnum)base["parameterType"]; }
            set { base["parameterType"] = value; }
        }

        public enum ParameterTypeEnum
        {
            Constructor,
            Property
        }
    }
}
