using System.Collections.Generic;

namespace Noodle.Extensions.Collections
{
    /// <summary>
    /// A collection of INameable objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INamedCollection<T> : IList<T>, INamedList<T>, IPageableList<T>, IQueryableList<T> where T : class, INameable
    {
    }
}
