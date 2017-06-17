using System.Threading;
using Runic.Framework.Models;
using Runic.Agent.ThreadPatterns;
using Runic.Agent.ThreadManagement;

namespace Runic.Agent.Services
{
    public class GraphFlowService
    {
        private IThreadOrchestrator _threadOrchestrator { get; set; }

        public GraphFlowService(IThreadOrchestrator threadOrchestrator)
        {
            _threadOrchestrator = threadOrchestrator;
        }

        public void ExecuteFlow(GraphFlowExecutionRequest request, CancellationToken ct)
        {
            var pattern = new GraphThreadPattern()
            {
                DurationSeconds = request.ThreadPattern.DurationSeconds,
                Points = request.ThreadPattern.Points
            };
            _threadOrchestrator.AddNewPattern(request.PatternExecutionId, request.Flow, pattern);
        }
    }
}
