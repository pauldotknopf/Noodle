using NUnit.Framework;

namespace Noodle.Tests
{
    [TestFixture]
    public class StringTests
    {
        [Test]
        public void IsNullOrWhiteSpace()
        {
            " ".IsNullOrWhiteSpace().ShouldBeTrue();
            "".IsNullOrWhiteSpace().ShouldBeTrue();
            ((string)null).IsNullOrWhiteSpace().ShouldBeTrue();
            "  d  ".IsNullOrWhiteSpace().ShouldBeFalse();
            "d".IsNullOrWhiteSpace().ShouldBeFalse();
        }
    }
}
