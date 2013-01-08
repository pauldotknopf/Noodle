using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace Noodle.Configuration
{
    /// <summary>
    /// Allws to "remove" items not in the collection. The reader of the configuration must 
    /// implement "remove" semantics reading from the RemovedElements collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LazyRemovableCollection<T> : ConfigurationElementCollection, IEnumerable<T>
        where T : ConfigurationElement, IIdentifiable, new()
    {
        List<T> _removedElements = new List<T>();
        List<T> _defaults = new List<T>();
        bool _isCleared;

        /// <summary>Elements that were "removed".</summary>
        public IEnumerable<T> RemovedElements
        {
            get { return _removedElements.AsReadOnly(); }
            set { _removedElements = new List<T>(value ?? new T[0]); }
        }

        /// <summary>Elements that were "removed".</summary>
        public IEnumerable<T> Defaults
        {
            get { return _defaults.AsReadOnly(); }
            set { _defaults = new List<T>(value ?? new T[0]); }
        }

        /// <summary>All added elements except those that have been removed but not default elements.</summary>
        public IEnumerable<T> AddedElements
        {
            get
            {
                object[] removedKeys = RemovedElements.Select(e => e.ElementKey).ToArray();
                return from key in BaseGetAllKeys() where !removedKeys.Contains(key) select BaseGet(key) as T;
            }
        }

        /// <summary>All added elements except those that have been removed.</summary>
        public IEnumerable<T> AllElements
        {
            get
            {
                var removedKeys = RemovedElements.Select(e => e.ElementKey).ToArray();
                foreach (T element in from element in Defaults let key = element.ElementKey where !removedKeys.Contains(key) select element)
                {
                    yield return element;
                }
                foreach (var element in AddedElements)
                {
                    yield return element;
                }
            }
            set
            {
                BaseClear();
                _defaults.Clear();
                if (value == null) return;

                foreach (var element in value)
                    BaseAdd(element);
            }
        }

        public bool IsCleared
        {
            get { return _isCleared; }
        }

        /// <summary>Adds an element to the collection of defaults.</summary>
        /// <param name="element">The element to add.</param>
        public void AddDefault(T element)
        {
            _defaults.Add(element);
        }

        /// <summary>Adds an element to the collection.</summary>
        /// <param name="element">The element to add.</param>
        public void Add(T element)
        {
            BaseAdd(element);
        }

        /// <summary>"Removes" an element from the collection.</summary>
        /// <param name="element">The element to "remove".</param>
        public void Remove(T element)
        {
            _removedElements.Add(element);
        }

        /// <summary>Clears all the elements in this collection.</summary>
        public void Clear()
        {
            _isCleared = true;
            _defaults.Clear();
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IIdentifiable)element).ElementKey;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            if (elementName == "remove")
            {
                T element = new T {ElementKey = reader.GetAttribute("name")};

                OnDeserializeRemoveElement(element, reader);

                Remove(element);
                return true;
            }
            if (elementName == "clear")
            {
                Clear();
                return true;
            }
            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        protected virtual void OnDeserializeRemoveElement(T element, XmlReader reader)
        {
        }

        public new int Count
        {
            get { return AllElements.Count(); }
        }

        #region IEnumerable<T> Members

        public new IEnumerator<T> GetEnumerator()
        {
            return AllElements.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AllElements.GetEnumerator();
        }

        #endregion
    }
}
