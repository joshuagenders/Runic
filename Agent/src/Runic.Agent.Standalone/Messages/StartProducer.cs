using Runic.Agent.Core.WorkGenerator;

namespace Runic.Agent.Standalone.Messages
{
    public class StartProducer
    {
        public StartProducer(TestPlan testPlan)
        {
            TestPlan = testPlan;
        }
        public TestPlan TestPlan { get; }
    }
}