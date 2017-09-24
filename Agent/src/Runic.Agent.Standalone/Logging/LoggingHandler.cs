using Microsoft.Extensions.Logging;
using Runic.Agent.Core.Services;
using Runic.Agent.TestHarness.StepController;
using System;
using Runic.Agent.Framework.Models;

namespace Runic.Agent.Standalone.Logging
{
    public class LoggingHandler : IEventHandler
    {
        private readonly ILogger _log;
        public LoggingHandler(ILoggerFactory loggingFactory)
        {
            _log = loggingFactory.CreateLogger("Runic.Agent");
        }
        public void Debug(string message)
        {
            _log.LogDebug(message);
        }

        public void Debug(string message, object obj)
        {
            _log.LogDebug(message, obj);
        }

        public void Debug(string message, Exception ex = null)
        {
            throw new NotImplementedException();
        }

        public void Error(string message)
        {
            _log.LogError(message);
        }

        public void Error(string message, object obj)
        {
            _log.LogError(message, obj);
        }

        public void Error(string message, Exception ex = null)
        {
            _log.LogError(message, ex);
        }

        public void Info(string message)
        {
            _log.LogInformation(message);
        }

        public void Info(string message, object obj)
        {
            _log.LogInformation(message, obj);
        }

        public void Info(string message, Exception ex = null)
        {
            _log.LogInformation(message, ex);
        }

        public void OnFlowAdded(Journey flow)
        {
            _log.LogInformation("Flow added", flow);
        }

        public void OnFlowComplete(Journey flow)
        {
            _log.LogInformation("Flow complete", flow);
        }

        public void OnFlowStart(Journey flow)
        {
            _log.LogInformation("Flow started", flow);
        }

        public void OnTestResult(Result result)
        {
            _log.LogInformation("Test Result", result);
        }

        public void OnThreadChange(Journey flow, int threadLevel)
        {
            _log.LogInformation($"Thread Change on flow {flow.Name} to {threadLevel}");
        }

        public void Warning(string message)
        {
            _log.LogWarning(message);
        }

        public void Warning(string message, object obj)
        {
            _log.LogWarning(message, obj);
        }

        public void Warning(string message, Exception ex = null)
        {
            _log.LogWarning(message, ex);
        }
    }
}
