using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Noodle.Tests
{
    [TestClass]
    public class StringTests
    {
        [TestMethod]
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
