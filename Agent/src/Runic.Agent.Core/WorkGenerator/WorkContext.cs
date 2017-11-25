using System.Threading;

namespace Runic.Agent.Core.WorkGenerator
{
    public class WorkContext
    { 
        public CancellationToken Ctx { get; set; }
        public Work Work { get; set; }
    }
}
