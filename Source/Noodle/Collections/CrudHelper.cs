using System;
using System.Collections.Generic;
using System.Linq;

namespace Noodle.Collections
{
    public static class CrudHelper
    {
        public static void Crud<T>(IList<T> existingCollection, IList<T> newCollection,  Func<T, object> keySelector,  ref IList<T> create, ref IList<T> update, ref IList<T> delete)
        {
            var existingDictionary = existingCollection.ToDictionary(keySelector);
            var newDictionary = newCollection.ToDictionary(keySelector);
            
            foreach(var newItemKey in newDictionary.Keys)
            {

                if (!existingDictionary.ContainsKey(newItemKey))
                {
                    // Create
                    create.Add(newDictionary[newItemKey]);
                }
                else
                {
                    // Update
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
