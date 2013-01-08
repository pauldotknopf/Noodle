using System;

namespace Noodle.Caching
{
    /// <summary>
    /// This attrbute is used to mark methods as cacheable.
    /// Using this attribute does nothing by itself, but combined with Noodle.Extensions,
    /// provides aspect oriented caching that is easy to set up.
    /// This attribute is stored in this library to keep the dependencies down.
    /// Noodle.Extensions has the required to make this attribute useful
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheAttribute : Attribute
    {
        public int TimeoutMinutes { get; set; }
    }
}
