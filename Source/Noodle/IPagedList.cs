using System.Collections.Generic;

namespace Noodle
{
    /// <summary>
    /// Paged list interface
    /// </summary>
    public interface IPagedList<T> : IList<T>
    {
        /// <summary>
        /// The current page index
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// The page size
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// The total number of items
        /// </summary>
        int TotalCount { get; }
        
        /// <summary>
        /// The total number of pages
        /// </summary>
        int TotalPages { get; }

        /// <summary>
        /// Is there a previous page?
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// Is there a next page?
        /// </summary>
        bool HasNextPage { get; }
    }
}
