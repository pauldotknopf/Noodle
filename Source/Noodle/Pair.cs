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
        /// Creates a new instance with null values for First and Second
        /// </summary>
        public Pair()
        {
            
        }

        /// <summary>
        /// Create a new instance with the given for and second item
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

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
