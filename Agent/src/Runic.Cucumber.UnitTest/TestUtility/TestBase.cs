namespace Runic.Cucumber.UnitTest.TestUtility
{
    public class TestBase
    {
        protected TestEnvironment TestEnvironment { get; set; }

        public TestBase()
        {
            TestEnvironment = new TestEnvironment();
        }
    }
}
