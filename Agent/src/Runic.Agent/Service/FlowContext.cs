using System.Threading;
using System.Threading.Tasks;
using Runic.Core.Models;

namespace Runic.Agent.Service
{
    public class FlowContext
    {
        public string FlowName { get; set; }
        public Flow Flow { get; set; }
        public int ThreadCount { get; set; }
        public Task Task { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}