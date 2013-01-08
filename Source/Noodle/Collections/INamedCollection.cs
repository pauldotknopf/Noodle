using System.Collections.Generic;

namespace Noodle.Collections
{
    public interface INamedCollection<T> : IList<T>, INamedList<T>, IPageableList<T>, IQueryableList<T> where T : class, INameable
    {
    }
}
