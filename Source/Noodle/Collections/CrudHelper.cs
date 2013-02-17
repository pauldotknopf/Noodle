using System;
using System.Collections.Generic;
using System.Linq;

namespace Noodle.Collections
{
    /// <summary>
    /// CRUD helper for determining which items need to be created/updated/deleted
    /// </summary>
    public static class CrudHelper
    {
        /// <summary>
        /// Compare two collections and determine which ones are deleted/updated/new
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="existingCollection">The existing collection</param>
        /// <param name="newCollection">The new collection (with inserts/deletes)</param>
        /// <param name="keySelector">The key of the T parameter that is the primary key</param>
        /// <param name="create">The items that have been created (new)</param>
        /// <param name="update">The items that exist in both collections that need to be updated</param>
        /// <param name="delete">The items that don't exist in the new collection and should be deleted</param>
        public static void Crud<T>(IList<T> existingCollection, IList<T> newCollection,  Func<T, object> keySelector,  ref IList<T> create, ref IList<T> update, ref IList<T> delete)
        {
            var existingDictionary = existingCollection.ToDictionary(keySelector);
            var newDictionary = newCollection.ToDictionary(keySelector);
            
            foreach(var newItemKey in newDictionary.Keys)
            {
                if (!existingDictionary.ContainsKey(newItemKey))
                {
                    // create
                    create.Add(newDictionary[newItemKey]);
                }
                else
                {
                    // update
                    update.Add(newDictionary[newItemKey]);
                }
            }

            foreach (var existingItemKey in existingDictionary.Keys.Where(existingItemKey => !newDictionary.ContainsKey(existingItemKey)))
            {
                delete.Add(existingDictionary[existingItemKey]);
            }
        }
    }
}
