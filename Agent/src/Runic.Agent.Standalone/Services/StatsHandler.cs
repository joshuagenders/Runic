using Runic.Agent.Core.Services;
using Runic.Agent.TestHarness.StepController;
using Runic.Agent.Framework.Models;
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

        public void Debug(string message, Exception ex = null)
        {
            // Method intentionally left empty.
        }
        public void Error(string message, Exception ex = null)
        {
            // Method intentionally left empty.
        }
        public void Info(string message, Exception ex = null)
        {
            // Method intentionally left empty.
        }
        public void Warning(string message, Exception ex = null)
        {
            // Method intentionally left empty.
        }

        public void OnFlowComplete(Journey flow)
        {
            _statsd.Count($"flows.{flow.Name}.Complete");
        }

        public void OnFlowStart(Journey flow)
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

        public void OnThreadChange(Journey flow, int threadLevel)
        {
            _statsd.Gauge($"flows.{flow.Name}.threadLevel", threadLevel);
        }
    }
}
