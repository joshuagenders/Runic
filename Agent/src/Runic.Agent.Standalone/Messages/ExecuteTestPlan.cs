using Runic.Agent.Core.WorkGenerator;

namespace Runic.Agent.Standalone.Messages
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
