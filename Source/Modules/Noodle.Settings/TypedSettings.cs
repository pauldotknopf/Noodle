using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Settings
{
    public class TypedSettings<T> : Setting
        where T : ISettings, new()
    {
        public TypedSettings()
        {
            Settings = new T();
        }

        public T Settings { get; set; }
    }
}
