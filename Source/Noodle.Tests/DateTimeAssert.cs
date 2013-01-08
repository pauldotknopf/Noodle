using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Noodle.Tests
{
    public static class DateTimeAssert
    {
        public static DateTime Tollerable(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute,
                                dateTime.Second);
        }

        public static DateTime? Tollerable(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.Tollerable() : (DateTime?)null;
        }

        public static void ShouldBeSameTimeAs(this DateTime dateTime, DateTime other)
        {
            Assert.IsTrue(dateTime.Tollerable() == other.Tollerable(), "dateTimes aren't the same");
        }
    }
}
