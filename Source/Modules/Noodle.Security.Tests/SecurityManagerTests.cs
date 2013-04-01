namespace Noodle.Security.Tests
{
    [TestClass]
    public class SecurityManagerTests : SecurityTestBase
    {
        #region Database

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DataTestBaseHelper.DropCreateDatabase<SecurityManagerTests>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DataTestBaseHelper.DropDatabase<SecurityManagerTests>();
        }

        #endregion
    }
}
