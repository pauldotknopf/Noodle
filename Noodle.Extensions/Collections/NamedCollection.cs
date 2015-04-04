using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Noodle.Collections
{
    /// <summary>
    /// A collection of INameable objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NamedCollection<T> : INamedCollection<T>, IList where T : class, INameable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCollection{T}"/> class.
        /// </summary>
        public NamedCollection()
		{
			_inner = new List<T>();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCollection{T}"/> class with an existing collection
        /// </summary>
        /// <param name="inner">The inner.</param>
        public NamedCollection(IEnumerable<T> inner)
		{
			_inner = inner.ToList();
		}

		private List<T> _inner;

		protected List<T> Inner
		{
			get { return _inner; }
			set { _inner = value; }
		}

		private static void EnsureName(string key, T value)
		{
			if (value.Name != key)
				throw new InvalidOperationException("Cannot add value with differnet name (" + key + " != " + value.Name + ")");
		}

		#region IList Members

		public int IndexOf(T item)
		{
			return _inner.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_inner.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_inner.RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				return _inner[index];
			}
			set
			{
				_inner[index] = value;
			}
		}

		public void Add(T item)
		{
			_inner.Add(item);
		}

		public void Clear()
		{
			_inner.Clear();
		}

		public bool Contains(T item)
		{
			return _inner.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_inner.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _inner.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			return _inner.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _inner.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _inner.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			T[] arr = new T[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				arr[i] = (T)array.GetValue(i);
			}

			_inner.CopyTo(arr, index);
		}

		// The IsSynchronized Boolean property returns True if the 
		// collection is designed to be thread safe; otherwise, it returns False.
		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		#endregion

		#region INamedList<T> Members

		/// <summary>Adds an element with the provided key and value to the list.</summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value of the element to add.</param>
		public void Add(string key, T value)
		{
			EnsureName(key, value);

			_inner.Add(value);
		}

		/// <summary>Determines whether the list contains an element with the specified key.</summary>
		/// <param name="key">The key to locate in the list.</param>
		/// <returns>true if the System.Collections.Generic.IDictionary<TKey,TValue> contains an element with the key; otherwise, false.</returns>
		public bool ContainsKey(string key)
		{
			return _inner.Any(i => i.Name == key);
		}

		/// <summary>Gets an System.Collections.Generic.ICollection<T> containing the keys of the System.Collections.Generic.IDictionary<TKey,TValue>.</summary>
		public ICollection<string> Keys
		{
			get { return _inner.Select(i => i.Name).ToList(); }
		}

		/// <summary>Removes the element with the specified key from the System.Collections.Generic.IDictionary<TKey,TValue>.</summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if key was not found in the original list.</returns>
		public bool Remove(string key)
		{
			var index = _inner.FindIndex(i => i.Name == key);
			if (index >= 0)
				_inner.RemoveAt(index);
			return index >= 0;
		}

		/// <summary>Gets the value associated with the specified key.</summary>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
		/// <returns>true if the list contains an element with the specified key; otherwise, false.</returns>
		public bool TryGetValue(string key, out T value)
		{
			value = _inner.FirstOrDefault(i => i.Name == key);
			return value != null;
		}

		/// <summary>Gets an System.Collections.Generic.ICollection<T> containing the values in the System.Collections.Generic.IDictionary<TKey,TValue>.</summary>
		public ICollection<T> Values
		{
			get { return _inner.ToList(); }
		}

		/// <summary>Gets or sets the element with the specified key.</summary>
		/// <param name="name">The key of the element to get or set.</param>
		/// <returns>The element with the specified key.</returns>
		public T this[string name]
		{
			get
			{
				return FindNamed(name);
			}
			set
			{
				EnsureName(name, value);

				int index = _inner.FindIndex(i => i.Name == name);
				if (index < 0)
					Add(name, value);
				else
					_inner[index] = value;
			}
		}

		/// <summary>Finds an item with the given name.</summary>
		/// <param name="name">The name of the item to find.</param>
		/// <returns>The item with the given name or null if no item was found.</returns>
		public T FindNamed(string name)
		{
			return _inner.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.InvariantCultureIgnoreCase));
		}

		#endregion

		#region IList Members

		int IList.Add(object value)
		{
			Add(value as T);
			return IndexOf(value as T);
		}

		bool IList.Contains(object value)
		{
			return Contains(value as T);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf(value as T);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, value as T);
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		void IList.Remove(object value)
		{
			Remove(value as T);
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = value as T; }
		}

		#endregion

		#region IPageableList<T> Members

		public IQueryable<T> FindRange(int skip, int take)
		{
			return _inner.Skip(skip).Take(take).AsQueryable();
		}

		#endregion

		#region IQueryableList<T> Members

		public virtual IQueryable<T> Query()
		{
			return _inner.AsQueryable();
		}

		#endregion
    }
}
