using System;

namespace Noodle
{
    /// <summary>
    /// Represents the value type of a content details using multiple value types.
    /// </summary>
    public interface IMultipleValue<T, TDetail> 
        where T:class, IBaseEntityWithDetails<T, TDetail>
        where TDetail: EntityDetail<T, TDetail>, new()
    {
        bool? BoolValue { get; set; }
        DateTime? DateTimeValue { get; set; }
        double? DoubleValue { get; set; }
        int? IntValue { get; set; }
        T LinkedItem { get; set; }
        object ObjectValue { get; set; }
        string StringValue { get; set; }
    }
}
