using System.Threading;
using Runic.Framework.Models;
using Runic.Agent.ThreadPatterns;
using Runic.Agent.ThreadManagement;
using System;

namespace Runic.Agent.Services
{
    public class GradualFlowService
    {
        private IThreadOrchestrator _threadOrchestrator { get; set; }

        public GradualFlowService(IThreadOrchestrator threadOrchestrator)
        {
            _threadOrchestrator = threadOrchestrator;
        }

        public void ExecuteFlow(GradualFlowExecutionRequest request, CancellationToken ct)
        {
            if (request == null)
                throw new NullReferenceException("Gradual Flow Execution Request passed to ExecuteFlow was null");
            if (_threadOrchestrator == null)
                throw new NullReferenceException("No IThreadOrchestrator registered with GradualFlowService on ExecuteFlow");
            var pattern = new GradualThreadPattern()
            {
                DurationSeconds = request.ThreadPattern.DurationSeconds,
                Points = request.ThreadPattern.Points,
                RampDownSeconds = request.ThreadPattern.RampDownSeconds,
                RampUpSeconds = request.ThreadPattern.RampUpSeconds,
                StepIntervalSeconds = request.ThreadPattern.StepIntervalSeconds,
                ThreadCount = request.ThreadPattern.ThreadCount
            };

            _threadOrchestrator.AddNewPattern(request.PatternExecutionId, request.Flow, pattern);
        }
    }
}
