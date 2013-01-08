using System.Collections.Generic;

namespace Noodle
{
    /// <summary>
    /// Useful linq extensions.
    /// </summary>
    /// <remarks></remarks>
    public static class LinqExtensions
    {
        /// <summary>
        /// Batches the specified collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns>A list of lists (batched)</returns>
        /// <remarks></remarks>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            var nextbatch = new List<T>(batchSize);
            foreach (var item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSize)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(batchSize);
                }
            }
            if (nextbatch.Count > 0)
                yield return nextbatch;
        }
    }
}
