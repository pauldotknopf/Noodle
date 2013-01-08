using System;
using Noodle.Collections;

namespace Noodle
{
    /// <summary>
    /// A content detail. A number of content details can be associated with one content item.
    /// </summary>
    /// <remarks>Usually content details are created below the hood when working with primitive .NET types against a contnet item.</remarks>
    [Serializable]
    public class EntityDetail<T, TDetail> : ICloneable, INameable, IMultipleValue<T, TDetail> 
        where T:class, IBaseEntityWithDetails<T,TDetail>
        where TDetail:EntityDetail<T, TDetail>, new()
    {
        #region TypeKeys
        public static class TypeKeys
        {
            public const string BoolType = "Bool";
            public const string IntType = "Int";
            public const string LinkType = "Link";
            public const string DoubleType = "Double";
            public const string DateTimeType = "DateTime";
            public const string StringType = "String";
            public const string ObjectType = "Object";
            public const string MultiType = "Multi";
        }
        #endregion

        #region Constuctors
        /// <summary>Creates a new (empty) instance of the content detail.</summary>
        public EntityDetail()
        {
            id = 0;
            enclosingItem = null;
            name = string.Empty;
            ValueTypeKey = TypeKeys.StringType;
            Value = null;
        }

        public EntityDetail(T enclosingItem, string name, object value)
        {
            Id = 0;
            EnclosingItem = enclosingItem;
            Name = name;
            Value = value;
        }
        #endregion

        #region Private Fields
        private int id;
        private T enclosingItem;
        private int enclosingItemId;
        private string name;
        //private EntityDetailCollection<T> collection;
        private string valueTypeKey;

        private string stringValue;
        private T linkedItem;
        private int? linkValue;
        private double? doubleValue;
        private DateTime? dateTimeValue;
        private int? intValue;
        private bool? boolValue;
        private object objectValue;
        #endregion

        #region Public Properties

        /// <summary>Gets or sets the detil's primary key.</summary>
        public virtual int Id
        {
            get { return id; }
            set { id = value; }

        }

        /// <summary>Gets or sets the name of the detail.</summary>
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>Gets or sets this details' value.</summary>
        public virtual object Value
        {
            get
            {
                switch (ValueTypeKey)
                {
                    case TypeKeys.BoolType:
                        return boolValue;
                    case TypeKeys.DateTimeType:
                        return dateTimeValue;
                    case TypeKeys.DoubleType:
                        return doubleValue;
                    case TypeKeys.IntType:
                        return intValue;
                    case TypeKeys.LinkType:
                        return linkedItem;
                    case TypeKeys.StringType:
                        return stringValue;
                    case TypeKeys.MultiType:
                        return new MultipleValueHolder<T, TDetail> { BoolValue = BoolValue, DateTimeValue = DateTimeValue, DoubleValue = DoubleValue, IntValue = IntValue, LinkedItem = LinkedItem, ObjectValue = ObjectValue, StringValue = StringValue };
                    default:
                        return objectValue;
                }
            }
            set
            {
                valueTypeKey = SetValue(value);
            }
        }

        #region class MultipleValueHolder
        class MultipleValueHolder<T, TMultiDetail> : IMultipleValue<T, TMultiDetail>
            where T : class, IBaseEntityWithDetails<T, TMultiDetail>
            where TMultiDetail: EntityDetail<T, TMultiDetail>, new()
        {
            #region IMultipleValue Members
            public bool? BoolValue { get; set; }
            public DateTime? DateTimeValue { get; set; }
            public double? DoubleValue { get; set; }
            public int? IntValue { get; set; }
            public T LinkedItem { get; set; }
            public object ObjectValue { get; set; }
            public string StringValue { get; set; }
            #endregion
        }
        #endregion

        private string SetValue(object value)
        {
            if (value == null)
            {
                EmptyValue();
                return valueTypeKey;
            }

            Type t = value.GetType();
            EmptyValue();
            switch (t.FullName)
            {
                case "System.Boolean":
                    boolValue = (bool)value;
                    return TypeKeys.BoolType;
                case "System.Int32":
                    intValue = (int)value;
                    return TypeKeys.IntType;
                case "System.Double":
                    doubleValue = (double)value;
                    return TypeKeys.DoubleType;
                case "System.DateTime":
                    dateTimeValue = (DateTime)value;
                    return TypeKeys.DateTimeType;
                case "System.String":
                    stringValue = (string)value;
                    return TypeKeys.StringType;
                default:
                    if (t.IsSubclassOf(typeof(T)))
                    {
                        LinkedItem = (T)value;
                        return TypeKeys.LinkType;
                    }
                    else
                    {
                        objectValue = value;
                        return TypeKeys.ObjectType;
                    }
            }
        }

        private void EmptyValue()
        {
            switch (ValueTypeKey)
            {
                case TypeKeys.BoolType:
                    boolValue = false;
                    return;
                case TypeKeys.DateTimeType:
                    dateTimeValue = DateTime.MinValue;
                    return;
                case TypeKeys.DoubleType:
                    doubleValue = 0;
                    return;
                case TypeKeys.IntType:
                    intValue = 0;
                    return;
                case TypeKeys.LinkType:
                    linkedItem = null;
                    return;
                case TypeKeys.StringType:
                    stringValue = null;
                    return;
                default:
                    objectValue = null;
                    return;
            }
        }

        /// <summary>Gets the type of value associated with this item.</summary>
        public virtual Type ValueType
        {
            get
            {
                switch (ValueTypeKey)
                {
                    case TypeKeys.BoolType:
                        return typeof(bool);
                    case TypeKeys.DateTimeType:
                        return typeof(DateTime);
                    case TypeKeys.DoubleType:
                        return typeof(double);
                    case TypeKeys.IntType:
                        return typeof(int);
                    case TypeKeys.StringType:
                        return typeof(string);
                    case TypeKeys.LinkType:
                        return typeof(T);
                    case TypeKeys.MultiType:
                        return typeof(IMultipleValue<T, TDetail>);
                    default:
                        return typeof(object);
                }
            }
        }

        public virtual string ValueTypeKey
        {
            get { return valueTypeKey; }
            set { valueTypeKey = value; }
        }

        public virtual string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }

        public virtual T LinkedItem
        {
            get { return linkedItem; }
            set
            {
                linkedItem = value;
                if (value != null)
                    LinkValue = value.Id;
                else
                    LinkValue = null;
            }
        }

        protected internal virtual int? LinkValue
        {
            get { return linkValue; }
            set { linkValue = value; }
        }

        public virtual double? DoubleValue
        {
            get { return doubleValue; }
            set { doubleValue = value; }
        }

        public virtual DateTime? DateTimeValue
        {
            get { return dateTimeValue; }
            set { dateTimeValue = value; }
        }

        public virtual bool? BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        public virtual int? IntValue
        {
            get { return intValue; }
            set { intValue = value; }
        }

        public virtual object ObjectValue
        {
            get { return objectValue; }
            set { objectValue = value; }
        }

        /// <summary>Gets whether this items belongs to an <see cref="N2.Details.DetailCollection"/>.</summary>
        //public virtual bool IsInCollection
        //{
        //    get { return EnclosingCollection != null; }
        //}

        /// <summary>Gets or sets the content item that this detail belong to.</summary>
        /// <remarks>Usually this is assigned by a content item which encapsulates the usage of details</remarks>
        public virtual T EnclosingItem
        {
            get { return enclosingItem; }
            set { enclosingItem = value; }
        }

        public virtual int EnclosingItemId
        {
            get { return enclosingItemId; }
            set { enclosingItemId = value; }
        }

        /// <summary>Gets or sets the <see cref="N2.Details.DetailCollection"/> associated with this detail. This value can be null which means it's a named detail directly on the item.</summary>
        //public virtual EntityDetailCollection<T> EnclosingCollection
        //{
        //    get { return collection; }
        //    set { collection = value; }
        //}
        #endregion

        #region Static Methods
        /// <summary>Creates a new content detail of the appropriated type based on the given value.</summary>
        /// <param name="item">The item that will enclose the new detail.</param>
        /// <param name="name">The name of the detail.</param>
        /// <param name="value">The value of the detail. This will determine what type of content detail will be returned.</param>
        /// <returns>A new content detail whose type depends on the type of value.</returns>
        public static EntityDetail<T, TDetail> New(T item, string name, object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return new EntityDetail<T, TDetail>(item, name, value);
        }

        /// <summary>Creates a new content detail of the appropriated type based on the given value.</summary>
        /// <param name="name">The name of the detail.</param>
        /// <param name="value">The value of the detail. This will determine what type of content detail will be returned.</param>
        /// <returns>A new content detail whose type depends on the type of value.</returns>
        public static TDetail New(string name, object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return new TDetail()
                       {
                           Name = name,
                           Value = value
                       };
        }

        /// <summary>Creates a new content detail with multiple values.</summary>
        /// <param name="enclosingItem">The item that will enclose the new detail.</param>
        /// <param name="name">The name of the detail.</param>
        /// <param name="booleanValue">Boolean value.</param>
        /// <param name="dateTimeValue">Date time value.</param>
        /// <param name="doubleValue">Double value.</param>
        /// <param name="integerValue">Integer value.</param>
        /// <param name="linkedValue">Linked item.</param>
        /// <param name="objectValue">Object value.</param>
        /// <param name="stringValue">String value.</param>
        /// <returns>A new content detail whose type depends on the type of value.</returns>
        public static EntityDetail<T, TDetail> Multi(string name, bool? booleanValue = null, int? integerValue = null, double? doubleValue = null, DateTime? dateTimeValue = null, string stringValue = null, T linkedValue = null, object objectValue = null)
        {
            return new EntityDetail<T, TDetail>
            {
                Name = name,
                ValueTypeKey = TypeKeys.MultiType,
                BoolValue = booleanValue,
                IntValue = integerValue,
                DoubleValue = doubleValue,
                DateTimeValue = dateTimeValue,
                LinkedItem = linkedValue,
                ObjectValue = objectValue,
                StringValue = stringValue
            };
        }

        /// <summary>Gets the name of the property on the detail class that can encapsulate the given value.</summary>
        /// <param name="value">The value for which the to retrieve the associated property.</param>
        /// <returns>The name of the property on the detail class that can encapsulate the given value.</returns>
        public static string GetAssociatedPropertyName(object value)
        {
            if (value is bool)
                return "BoolValue";
            else if (value is int)
                return "IntValue";
            else if (value is double)
                return "DoubleValue";
            else if (value is DateTime)
                return "DateTimeValue";
            else if (value is string)
                return "StringValue";
            else if (value is T)
                return "LinkedItem";
            else
                return "Value";
        }

        /// <summary>Gets the name of the property on the detail class that can encapsulate the given value type.</summary>
        /// <typeparam name="T">The value type for which the to retrieve the associated property.</typeparam>
        /// <returns>The name of the property on the detail class that can encapsulate the given value.</returns>
        public static string GetAssociatedPropertyName<T>()
        {
            return GetAssociatedPropertyName(typeof(T));
        }

        /// <summary>Gets the name of the property on the detail class that can encapsulate the given value type.</summary>
        /// <param name="valueType">The value type for which the to retrieve the associated property.</param>
        /// <returns>The name of the property on the detail class that can encapsulate the given value.</returns>
        public static string GetAssociatedPropertyName(Type valueType)
        {
            if (valueType == typeof(bool))
                return "BoolValue";
            else if (valueType == typeof(int))
                return "IntValue";
            else if (valueType == typeof(double))
                return "DoubleValue";
            else if (valueType == typeof(DateTime))
                return "DateTimeValue";
            else if (valueType == typeof(string))
                return "StringValue";
            else if (typeof(T).IsAssignableFrom(valueType))
                return "LinkedItem";
            else
                return "Value";
        }
        #endregion

        #region Equals, HashCode and ToString Overrides

        /// <summary>Checks details for equality.</summary>
        /// <returns>True if details have the same ID.</returns>
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var other = obj as EntityDetail<T, TDetail>;
            return other != null && id != 0 && id == other.id;
        }

        int? hashCode;
        /// <summary>Gets a hash code based on the ID.</summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            if (!hashCode.HasValue)
                hashCode = (id > 0 ? id.GetHashCode() : base.GetHashCode());
            return hashCode.Value;
        }

        /// <summary>Returns this details value's ToString result.</summary>
        /// <returns>The value to string.</returns>
        public override string ToString()
        {
            return null == this.Value
                ? base.ToString()
                : this.Value.ToString();
        }
        #endregion

        #region ICloneable Members

        /// <summary>Creates a cloned object with the id set to 0.</summary>
        /// <returns>A new ContentDetail with the same Name and Value.</returns>
        public virtual EntityDetail<T, TDetail> Clone()
        {
            var cloned = new EntityDetail<T, TDetail>();
            cloned.Id = 0;
            cloned.Name = this.Name;
            cloned.BoolValue = this.BoolValue;
            cloned.DateTimeValue = this.DateTimeValue;
            cloned.DoubleValue = this.DoubleValue;
            cloned.IntValue = this.IntValue;
            cloned.LinkedItem = this.LinkedItem;
            cloned.ObjectValue = this.ObjectValue;
            cloned.StringValue = this.StringValue;
            cloned.ValueTypeKey = this.ValueTypeKey;
            return cloned;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        public virtual void AddTo(T newEnclosingItem)
        {
            //AddTo((EntityDetailCollection<T>)null);

            if (newEnclosingItem == EnclosingItem)
                return;

            RemoveFromEnclosingItem();

            if (newEnclosingItem != null)
            {
                EnclosingItem = newEnclosingItem;
                newEnclosingItem.Details.Add(Name, (TDetail)this);
            }
        }

        protected internal virtual void RemoveFromEnclosingItem()
        {
            if (EnclosingItem != null)
                EnclosingItem.Details.Remove(Name);
        }

        //public virtual void AddTo(EntityDetailCollection<T> newEnclosingCollection)
        //{
        //    RemoveFromEnclosingCollection();

        //    if (newEnclosingCollection != null)
        //        newEnclosingCollection.Add(this);
        //}

        //protected internal virtual void RemoveFromEnclosingCollection()
        //{
        //    if (EnclosingCollection != null)
        //        EnclosingCollection.Remove(this);
        //}
    }
}
