using System;
using System.Reflection;
using Noodle.Collections;

namespace Noodle
{
    public class BaseDetailsEntity<T, TDetail> : BaseEntity, IBaseEntityWithDetails<T, TDetail>
        where T : class, IBaseEntityWithDetails<T, TDetail>
        where TDetail : EntityDetail<T, TDetail>, new()
    {
        private NamedCollection<TDetail> _details;
        //private NamedCollection<EntityDetailCollection<T>> _detailCollections;

        public virtual NamedCollection<TDetail> Details
        {
            get { return _details ?? (_details = new NamedCollection<TDetail>()); }
            set { _details = value; }
        }

        #region GetDetail & SetDetail<T> Methods
        /// <summary>Gets a detail from the details bag.</summary>
        /// <param name="detailName">The name of the value to get.</param>
        /// <returns>The value stored in the details bag or null if no item was found.</returns>
        public virtual object GetDetail(string detailName)
        {
            return Details.ContainsKey(detailName)
                ? Details[detailName].Value
                : null;
        }

        /// <summary>Gets a detail from the details bag.</summary>
        /// <param name="detailName">The name of the value to get.</param>
        /// <param name="defaultValue">The default value to return when no detail is found.</param>
        /// <returns>The value stored in the details bag or null if no item was found.</returns>
        public virtual T GetDetail<T>(string detailName, T defaultValue)
        {
            return Details.ContainsKey(detailName)
                ? (T)Details[detailName].Value
                : defaultValue;
        }

        /// <summary>Set a value into the <see cref="Details"/> bag. If a value with the same name already exists it is overwritten. If the value equals the default value it will be removed from the details bag.</summary>
        /// <param name="detailName">The name of the item to set.</param>
        /// <param name="value">The value to set. If this parameter is null or equal to defaultValue the detail is removed.</param>
        /// <param name="defaultValue">The default value. If the value is equal to this value the detail will be removed.</param>
        public virtual void SetDetail<T>(string detailName, T value, T defaultValue)
        {
            if (value == null || !value.Equals(defaultValue))
            {
                SetDetail<T>(detailName, value);
            }
            else if (Details.ContainsKey(detailName))
            {
                _details.Remove(detailName);
            }
        }

        /// <summary>Set a value into the <see cref="Details"/> bag. If a value with the same name already exists it is overwritten.</summary>
        /// <param name="detailName">The name of the item to set.</param>
        /// <param name="value">The value to set. If this parameter is null the detail is removed.</param>
        /// <typeparam name="T">The type of value to store in details.</typeparam>
        public virtual void SetDetail<T>(string detailName, T value)
        {
            SetDetail(detailName, value, typeof(T));
        }

        /// <summary>Set a value into the <see cref="Details"/> bag. If a value with the same name already exists it is overwritten.</summary>
        /// <param name="detailName">The name of the item to set.</param>
        /// <param name="value">The value to set. If this parameter is null the detail is removed.</param>
        /// <param name="valueType">The type of value to store in details.</param>
        public virtual void SetDetail(string detailName, object value, Type valueType)
        {
            TDetail detail = null;
            if (Details.TryGetValue(detailName, out detail))
            {
                if (value != null)
                {
                    // update an existing detail of same type
                    detail.Value = value;
                    return;
                }
            }

            if (detail != null)
                // delete detail or remove detail of wrong type
                Details.Remove(detailName);
            if (value != null)
                // add new detail
                Details.Add(detailName, new TDetail
                                            {
                                                Name = detailName,
                                                Value = value,
                                                EnclosingItem = this as T
                                            });
        }
        #endregion

        #region this[]

        /// <summary>Gets or sets the detail or property with the supplied name. If a property with the supplied name exists this is always returned in favour of any detail that might have the same name.</summary>
        /// <param name="detailName">The name of the propery or detail.</param>
        /// <returns>The value of the property or detail. If now property exists null is returned.</returns>
        public virtual object this[string detailName]
        {
            get
            {
                if (detailName == null)
                    throw new ArgumentNullException("detailName");

                switch (detailName)
                {
                    //TODO:The cases for typed properties
                    default:
                        return CommonHelper.Evaluate(this, detailName)
                               ?? GetDetail(detailName);
                        //?? GetDetailCollection(detailName, false);
                }
            }
            set
            {
                if (string.IsNullOrEmpty(detailName))
                    throw new ArgumentNullException("Parameter 'detailName' cannot be null or empty.", "detailName");

                switch (detailName)
                {
                    //TODO:Cases for the type properties
                    //case "ZoneName": ZoneName = Utility.Convert<string>(value); break;
                    default:
                        {
                            PropertyInfo info = GetType().GetProperty(detailName);
                            if (info != null && info.CanWrite)
                            {
                                if (value != null && info.PropertyType != value.GetType())
                                    value = CommonHelper.To(value, info.PropertyType);
                                info.SetValue(this, value, null);
                            }
                            else
                            {
                                SetDetail(detailName, value);
                            }
                        }
                        break;
                }
            }
        }
        #endregion
    }
}
