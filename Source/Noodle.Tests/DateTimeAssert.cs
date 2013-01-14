using System;
using NUnit.Framework;

namespace Noodle.Tests
{
    public static class DateTimeAssert
    {
        public static void ShouldBeSameTimeAs(this DateTime dateTime, DateTime other)
        {
            Assert.IsTrue(dateTime.Tollerable() == other.Tollerable(), "dateTimes aren't the same");
        }
    }
}
