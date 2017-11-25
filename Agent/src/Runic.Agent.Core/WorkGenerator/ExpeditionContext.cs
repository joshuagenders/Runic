using System.Threading;

namespace Runic.Agent.Core.WorkGenerator
{
    public class ExpeditionContext
    { 
        public CancellationToken Ctx { get; set; }
        public Expedition TestPlan { get; set; }
    }
}
