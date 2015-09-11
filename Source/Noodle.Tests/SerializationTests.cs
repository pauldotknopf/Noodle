using System;
using Noodle.Extensions.Serialization;
using NUnit.Framework;

namespace Noodle.Tests
{
    [TestFixture]
    public class SerializationTests
    {
        ISerializer _serializer = null;

        public SerializationTests()
        {
            _serializer = new BinaryStringSerializer();
        }

        [Test]
        public void Can_serialize()
        {
            var testDate = DateTime.Now;

            var testObject = new TestObject {TestDate = testDate};

            var serialized = _serializer.Serialize(testObject);

            var deserialize = _serializer.Deserialize<TestObject>(serialized);

            Assert.AreEqual(deserialize.TestDate, testDate);
        }

        [Serializable]
        public class TestObject
        {
            public DateTime TestDate { get; set; }
        }
    }
}
