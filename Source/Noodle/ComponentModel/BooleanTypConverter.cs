using System;
using System.ComponentModel;

namespace Noodle.ComponentModel
{
    /// <summary>
    /// This converter is more friendlier in terms of what gets parsed. "0", "1", "yes", "no", etc.
    /// </summary>
    public class BooleanTypCeonverter : BooleanConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if(value is string && !string.IsNullOrEmpty((string)value))
            {
                var stringValue = value.ToString();
                if(stringValue.Equals("0")
                    || stringValue.Equals("no", StringComparison.InvariantCultureIgnoreCase))
                {
                    return base.ConvertFrom(context, culture, "false");
                }else if(stringValue.Equals("1")
                    || stringValue.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                {
                    return base.ConvertFrom(context, culture, "true");
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
