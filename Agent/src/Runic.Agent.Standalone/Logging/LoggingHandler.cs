using Microsoft.Extensions.Logging;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.StepController;
using Runic.Framework.Models;
using System;

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void OnFlowAdded(Flow flow)
        {
            throw new NotImplementedException();
        }

        public void OnFlowComplete(Flow flow)
        {
            throw new NotImplementedException();
        }

        public void OnFlowStart(Flow flow)
        {
            throw new NotImplementedException();
        }

        public void OnTestResult(Result result)
        {
            throw new NotImplementedException();
        }

        public void OnThreadChange(Flow flow, int threadLevel)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
