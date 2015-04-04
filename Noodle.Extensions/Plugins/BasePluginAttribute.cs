using System;

namespace Noodle.Plugins
{
    public class BasePluginAttribute : Attribute, IPlugin
    {
        public virtual string Name { get; set; }

        public virtual Type Decorates { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual int CompareTo(IPlugin other)
        {
            if (other == null)
                return 1;
            var result = SortOrder.CompareTo(other.SortOrder) * 2 + String.CompareOrdinal(Name, other.Name);
            return result;
        }
    }
}
