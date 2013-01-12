using System.Collections.Generic;
using NUnit.Framework;
using Noodle.Collections;

namespace Noodle.Tests
{
    [TestFixture]
    public class CrudCollectionTests
    {
        [Test]
        public void Can_delete()
        {
            var existingItems = new List<string> {"Value1", "Value2"};
            var newItems = new List<string> {"Value1"};

            IList<string> create = new List<string>();
            IList<string> update = new List<string>();
            IList<string> delete = new List<string>();

            CrudHelper.Crud(existingItems, newItems, x => x, ref create, ref update, ref delete);

            // Created
            Assert.AreEqual(0, create.Count);

            // Updated
            Assert.AreEqual(1, update.Count);
            Assert.AreEqual("Value1", update[0]);

            // Deleted
            Assert.AreEqual(1, delete.Count);
            Assert.AreEqual("Value2", delete[0]);
        }

        [Test]
        public void Can_update()
        {
            var existingItems = new List<string> { "Value1" };
            var newItems = new List<string> { "Value1" };

            IList<string> create = new List<string>();
            IList<string> update = new List<string>();
            IList<string> delete = new List<string>();

            CrudHelper.Crud(existingItems, newItems, x => x, ref create, ref update, ref delete);

            // Created
            Assert.AreEqual(0, create.Count);

            // Deleted
            Assert.AreEqual(0, delete.Count);

            // Updated
            Assert.AreEqual(1, update.Count);
            Assert.AreEqual("Value1", update[0]);
        }

        [Test]
        public void Can_create()
        {
            var existingItems = new List<string> { "Value1" };
            var newItems = new List<string> { "Value1", "Value2" };

            IList<string> create = new List<string>();
            IList<string> update = new List<string>();
            IList<string> delete = new List<string>();

            CrudHelper.Crud(existingItems, newItems, x => x, ref create, ref update, ref delete);

            // Created
            Assert.AreEqual(1, create.Count);
            Assert.AreEqual("Value2", create[0]);
        }
    }
}
