using Runic.Agent.TestUtility;

namespace Runic.Agent.Core.UnitTest.TestUtility
{
    public class UnitEnvironment : TestEnvironmentBuilder
    {

        public override TestEnvironment StartApplication()
        {
            return this;
        }
    }
}
