using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Noodle.FluentDateTime;

namespace Noodle.Tests
{
    [TestClass]
    public class FluentTimeSpanTests
    {
        [TestMethod]
        public void Subtract()
        {
            Assert.AreEqual(3, 3.5.Days().Subtract(.5.Days()).Days);
        }

        [TestMethod]
        public void GetHashCodeTest()
        {
            Assert.AreEqual(343024320, 3.5.Days().GetHashCode());
        }

        [TestMethod]
        public void CompareToFluentTimeSpan()
        {
            Assert.AreEqual(0, 3.Days().CompareTo(3.Days()));
            Assert.AreEqual(-1, 3.Days().CompareTo(4.Days()));
            Assert.AreEqual(1, 4.Days().CompareTo(3.Days()));
        }
        [TestMethod]
        public void CompareToTimeSpan()
        {
            Assert.AreEqual(0, 3.Days().CompareTo(TimeSpan.FromDays(3)));
            Assert.AreEqual(-1, 3.Days().CompareTo(TimeSpan.FromDays(4)));
            Assert.AreEqual(1, 4.Days().CompareTo(TimeSpan.FromDays(3)));
        }
        [TestMethod]
        public void CompareToObject()
        {
            Assert.AreEqual(0, 3.Days().CompareTo((object)TimeSpan.FromDays(3)));
            Assert.AreEqual(-1, 3.Days().CompareTo((object)TimeSpan.FromDays(4)));
            Assert.AreEqual(1, 4.Days().CompareTo((object)TimeSpan.FromDays(3)));
        }
        [TestMethod]
        public void EqualsFluentTimeSpan()
        {
            Assert.IsTrue(3.Days().Equals(3.Days()));
            Assert.IsFalse(4.Days().Equals(3.Days()));
        }
        [TestMethod]
        public void EqualsTimeSpan()
        {
            Assert.IsTrue(3.Days().Equals(TimeSpan.FromDays(3)));
            Assert.IsFalse(4.Days().Equals(TimeSpan.FromDays(3)));
        }
        [TestMethod]
        public void Equals()
        {
            Assert.IsFalse(3.Days().Equals(null));
        }
        [TestMethod]
        public void EqualsTimeSpanAsObject()
        {
            Assert.IsTrue(3.Days().Equals((object)TimeSpan.FromDays(3)));
        }
        [TestMethod]
        public void EqualsObject()
        {
            Assert.IsFalse(3.Days().Equals(1));
        }
        [TestMethod]
        public void Add()
        {
            Assert.AreEqual(4, 3.5.Days().Add(.5.Days()).Days);
        }
        [TestMethod]
        public void ToStringTest()
        {
            Assert.AreEqual("3.12:00:00", 3.5.Days().ToString());
        }
        [TestMethod]
        public void Clone()
        {
            var timeSpan = 3.Milliseconds();
            var clone = timeSpan.Clone();
            Assert.AreNotSame(timeSpan, clone);
            Assert.AreEqual(timeSpan, clone);
        }

        [TestMethod]
        public void Ticks()
        {
            Assert.AreEqual(30000, 3.Milliseconds().Ticks);
        }

        [TestMethod]
        public void Milliseconds()
        {
            Assert.AreEqual(100, 1100.Milliseconds().Milliseconds);
        }
        [TestMethod]
        public void TotalMilliseconds()
        {
            Assert.AreEqual(1100, 1100.Milliseconds().TotalMilliseconds);
        }
        [TestMethod]
        public void Seconds()
        {
            Assert.AreEqual(1, 61.Seconds().Seconds);
        }
        [TestMethod]
        public void TotalSeconds()
        {
            Assert.AreEqual(61, 61.Seconds().TotalSeconds);
        }
        [TestMethod]
        public void Minutes()
        {
            Assert.AreEqual(1, 61.Minutes().Minutes);
        }
        [TestMethod]
        public void TotalMinutes()
        {
            Assert.AreEqual(61, 61.Minutes().TotalMinutes);
        }
        [TestMethod]
        public void Hours()
        {
            Assert.AreEqual(1, 25.Hours().Hours);
        }
        [TestMethod]
        public void TotalHours()
        {
            Assert.AreEqual(25, 25.Hours().TotalHours);
        }
        [TestMethod]
        public void Days()
        {
            Assert.AreEqual(366, 366.Days().Days);
        }
        [TestMethod]
        public void TotalDays()
        {
            Assert.AreEqual(366, 366.Days().TotalDays);
        }
        [TestMethod]
        public void Years()
        {
            var fluentTimeSpan = 3.Years();
            Assert.AreEqual(3, fluentTimeSpan.Years);
        }

        [TestMethod]
        public void EnsureWhenConvertedIsCorrect()
        {
            TimeSpan timeSpan = 10.Years();
            Assert.AreEqual(3650d, timeSpan.TotalDays);
        }

        [TestClass]
        public class OperatorOverloads
        {
            [TestMethod]
            public void LessThan()
            {
                Assert.IsTrue(1.Seconds() < 2.Seconds());
                Assert.IsTrue(1.Seconds() < TimeSpan.FromSeconds(2));
                Assert.IsTrue(TimeSpan.FromSeconds(1) < 2.Seconds());
            }
            [TestMethod]
            public void LessThanOrEqualTo()
            {
                Assert.IsTrue(1.Seconds() <= 2.Seconds());
                Assert.IsTrue(1.Seconds() <= TimeSpan.FromSeconds(2));
                Assert.IsTrue(TimeSpan.FromSeconds(1) <= 2.Seconds());
            }
            [TestMethod]
            public void GreaterThan()
            {
                Assert.IsTrue(2.Seconds() > 1.Seconds());
                Assert.IsTrue(2.Seconds() > TimeSpan.FromSeconds(1));
                Assert.IsTrue(TimeSpan.FromSeconds(2) > 1.Seconds());
            }
            [TestMethod]
            public void GreaterThanOrEqualTo()
            {
                Assert.IsTrue(2.Seconds() >= 1.Seconds());
                Assert.IsTrue(2.Seconds() >= TimeSpan.FromSeconds(1));
                Assert.IsTrue(TimeSpan.FromSeconds(2) >= 1.Seconds());
            }
            [TestMethod]
            public void Equals()
            {
                Assert.IsTrue(2.Seconds() == 2.Seconds());
                Assert.IsTrue(2.Seconds() == TimeSpan.FromSeconds(2));
                Assert.IsTrue(TimeSpan.FromSeconds(2) == 2.Seconds());
            }
            [TestMethod]
            public void NotEquals()
            {
                Assert.IsTrue(2.Seconds() != 1.Seconds());
                Assert.IsTrue(2.Seconds() != TimeSpan.FromSeconds(1));
                Assert.IsTrue(TimeSpan.FromSeconds(2) != 1.Seconds());
            }
            [TestMethod]
            public void Add()
            {
                Assert.AreEqual(1.Seconds() + 2.Seconds(), 3.Seconds());
                Assert.AreEqual(1.Seconds() + TimeSpan.FromSeconds(2), 3.Seconds());
                Assert.AreEqual(TimeSpan.FromSeconds(1) + 2.Seconds(), 3.Seconds());
            }
            [TestMethod]
            public void Subtract()
            {
                Assert.AreEqual(1.Seconds() - 2.Seconds(), -1.Seconds());
                Assert.AreEqual(1.Seconds() - TimeSpan.FromSeconds(2), -1.Seconds());
                Assert.AreEqual(TimeSpan.FromSeconds(1) - 2.Seconds(), -1.Seconds());
            }
        }
        [TestClass]
        public class ToDisplayStringTests
        {

            [TestMethod]
            public void DaysHours()
            {
                var timeSpan = 2.Days() + 3.Hours();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("2 days and 3 hours", displayString);
            }
            [TestMethod]
            public void DaysHoursRoundUp()
            {
                var timeSpan = 2.Days() + 3.Hours() + 30.Minutes();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("2 days and 4 hours", displayString);
            }
            [TestMethod]
            public void DaysHoursRoundDown()
            {
                var timeSpan = 2.Days() + 3.Hours() + 9.Minutes();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("2 days and 3 hours", displayString);
            }
            [TestMethod]
            public void HoursMinutes()
            {
                var timeSpan = 2.Hours() + 9.Minutes();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("2 hours and 9 minutes", displayString);
            }
            [TestMethod]
            public void HoursMinutesRoundUp()
            {
                var timeSpan = 2.Hours() + 9.Minutes() + 30.Seconds();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("2 hours and 10 minutes", displayString);
            }
            [TestMethod]
            public void HoursMinutesRoundDown()
            {
                var timeSpan = 2.Hours() + 9.Minutes() + 10.Seconds();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("2 hours and 9 minutes", displayString);
            }
            [TestMethod]
            public void MinutesSeconds()
            {
                var timeSpan = 9.Minutes();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("9 minutes and 0 seconds", displayString);
            }
            [TestMethod]
            public void MinutesSecondsRoundUp()
            {
                var timeSpan = 9.Minutes() + 30.5.Seconds();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("9 minutes and 31 seconds", displayString);
            }
            [TestMethod]
            public void MinutesSecondsRoundDown()
            {
                var timeSpan = 9.Minutes() + 30.4.Seconds();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("9 minutes and 30 seconds", displayString);
            }

            [TestMethod]
            public void SecondsMilliseconds()
            {
                var timeSpan = 9.Seconds();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("9 seconds", displayString);
            }
            [TestMethod]
            public void SecondsMillisecondsRoundUp()
            {
                var timeSpan = 9.Seconds() + 500.Milliseconds();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual(9.5 + " seconds", displayString);
            }
            [TestMethod]
            public void SecondsMillisecondsRoundDown()
            {
                var timeSpan = 9.Seconds() + 300.Milliseconds();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual(9.3 + " seconds", displayString);
            }

            [TestMethod]
            public void Milliseconds()
            {
                var timeSpan = 9.Milliseconds();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("9 milliseconds", displayString);
            }
            [TestMethod]
            public void ABitOverADay()
            {
                var timeSpan = 46.2.Hours();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("1 days and 22 hours", displayString);
            }
            [TestMethod]
            public void ABitOverADay2()
            {
                var timeSpan = 46.9.Hours();
                var displayString = timeSpan.ToDisplayString();
                Assert.AreEqual("1 days and 23 hours", displayString);
            }
        }
    }
}
