using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runic.Cucumber.UnitTest.TestUtility
{
    public class TestBase
    {
        protected TestEnvironment TestEnvironment { get; set; }

        [TestInitialize]
        public virtual void Init()
        {
            TestEnvironment = new TestEnvironment();
        }
    }
}
