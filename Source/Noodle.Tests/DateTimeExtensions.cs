using System;

namespace Noodle.Tests
{
    public static class DateTimeExtensions
    {
        public static DateTime Tollerable(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute,
                                dateTime.Second);
        }

        public static DateTime? Tollerable(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;

            return new DateTime(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day, dateTime.Value.Hour, dateTime.Value.Minute,
                                dateTime.Value.Second);
        }
    }
}
