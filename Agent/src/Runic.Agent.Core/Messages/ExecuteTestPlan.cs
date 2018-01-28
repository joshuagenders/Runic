using Runic.Agent.Core.Models;

namespace Runic.Agent.Core.Messages
{
    public class ExecuteTestPlan
    {
        public ExecuteTestPlan(TestPlan testPlan)
        {
            TestPlan = testPlan;
        }
        public TestPlan TestPlan { get; }
    }
}
