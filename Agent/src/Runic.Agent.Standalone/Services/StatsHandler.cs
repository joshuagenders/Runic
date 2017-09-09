using Runic.Agent.Core.Services;
using Runic.Agent.Core.StepController;
using Runic.Framework.Models;
using StatsN;
using System;

namespace Runic.Agent.Standalone.Services
{
    public class StatsHandler : IEventHandler
    {
        private readonly IStatsd _statsd;

        public StatsHandler(IStatsd statsd)
        {
            _statsd = statsd;
        }

        public void Debug(string message, Exception ex = null){ }
        public void Error(string message, Exception ex = null) { }
        public void Info(string message, Exception ex = null) { }
        public void Warning(string message, Exception ex = null) { }

        public void OnFlowComplete(Flow flow)
        {
            _statsd.Count($"flows.{flow.Name}.Complete");
        }

        public void OnFlowStart(Flow flow)
        {
            _statsd.Count($"flows.{flow.Name}.Start");
        }

        public void OnTestResult(Result result)
        {
            if (result.Success)
            {
                _statsd.Count($"functions.{result.Step.Function.FunctionName}.Success");
                _statsd.Count($"Steps.{result.Step.StepName}.Success");
                _statsd.Timing($"functions.{result.Step.Function.FunctionName}.Success", result.ExecutionTimeMilliseconds);
            }
            else
            {
                _statsd.Count($"functions.{result.Step.Function.FunctionName}.Error");
                _statsd.Count($"Steps.{result.Step.StepName}.Error");
                _statsd.Timing($"functions.{result.Step.Function.FunctionName}.Error", result.ExecutionTimeMilliseconds);
            }
        }

        public void OnThreadChange(Flow flow, int threadLevel)
        {
            _statsd.Gauge($"flows.{flow.Name}.threadLevel", threadLevel);
        }
    }
}
