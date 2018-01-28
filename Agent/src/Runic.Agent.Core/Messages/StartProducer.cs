using Runic.Agent.Core.Models;

namespace Runic.Agent.Core.Messages
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