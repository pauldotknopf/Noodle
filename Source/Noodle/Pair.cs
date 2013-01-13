using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle
{
    /// <summary>
    /// Used to store to related objects
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Pair<T1,T2>
    {
        /// <summary>
        /// The first item
        /// </summary>
        public T1 First { get; set; }

        /// <summary>
        /// And then the second
        /// </summary>
        public T2 Second { get; set; }
    }
}
