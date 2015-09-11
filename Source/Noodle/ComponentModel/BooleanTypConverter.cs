using System;
using System.ComponentModel;

namespace Noodle.ComponentModel
{
    /// <summary>
    /// This converter is more friendlier in terms of what gets parsed. "0", "1", "yes", "no", etc.
    /// </summary>
    public class BooleanTypCeonverter : BooleanConverter
    {
        /// <summary>
        /// Converts the given value object to a Boolean object.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that specifies the culture to which to convert.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted <paramref name="value" />.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if(value is string && !string.IsNullOrEmpty((string)value))
            {
                var stringValue = value.ToString();

                if ("true".Equals(stringValue, StringComparison.OrdinalIgnoreCase) ||
                     "1".Equals(stringValue, StringComparison.OrdinalIgnoreCase) ||
                     "yes".Equals(stringValue, StringComparison.OrdinalIgnoreCase) ||
                     "on".Equals(stringValue, StringComparison.OrdinalIgnoreCase))
                {
                    return base.ConvertFrom(context, culture, "true");
                }

                if ("false".Equals(stringValue, StringComparison.OrdinalIgnoreCase) ||
                    "0".Equals(stringValue, StringComparison.OrdinalIgnoreCase) ||
                    "no".Equals(stringValue, StringComparison.OrdinalIgnoreCase) ||
                    "off".Equals(stringValue, StringComparison.OrdinalIgnoreCase))
                {
                    return base.ConvertFrom(context, culture, "false");
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
