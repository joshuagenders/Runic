using System.Threading;

namespace Runic.Agent.Core
{
    public class TestPlanContext
    { 
        public CancellationToken Ctx { get; set; }
        public TestPlan TestPlan { get; set; }
    }
}
