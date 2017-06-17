using System.Threading;
using Runic.Framework.Models;
using Runic.Agent.ThreadPatterns;
using Runic.Agent.ThreadManagement;
using System;

namespace Runic.Agent.Services
{
    public class ConstantFlowService 
    {
        private IThreadOrchestrator _threadOrchestrator { get; set; }

        public ConstantFlowService(IThreadOrchestrator threadOrchestrator)
        {
            _threadOrchestrator = threadOrchestrator;
        }

        public void ExecuteFlow(ConstantFlowExecutionRequest request, CancellationToken ct)
        {
            var pattern = new ConstantThreadPattern()
            {
                ThreadCount = request.ThreadPattern.ThreadCount,
                DurationSeconds = request.ThreadPattern.DurationSeconds
            };

            _threadOrchestrator.AddNewPattern(request.PatternExecutionId, request.Flow, pattern);            
        }
    }
}
