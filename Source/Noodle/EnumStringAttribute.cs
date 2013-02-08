using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class EnumStringAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Default { get; set; }

        public EnumStringAttribute(string name, bool defaultForSerialization)
        {
            Name = name;
            Default = defaultForSerialization;
        }

        public EnumStringAttribute(string name) : this(name, false) { }
    }
}
