using System.Linq;

namespace Noodle.Extensions.Collections
{
    /// <summary>
    /// Query items in the list (possibly by querying the database).
    /// </summary>
    /// <typeparam name="T">The type of item to query.</typeparam>
    public interface IQueryableList<T>
    {
        /// <summary>Gets the queryable used to query a subset of the list.</summary>
        /// <returns>A queryable enumeration.</returns>
        IQueryable<T> Query();
    }
}
