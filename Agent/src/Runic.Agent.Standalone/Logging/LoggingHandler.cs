using Runic.Agent.Core.ExternalInterfaces;
using Microsoft.Extensions.Logging;

namespace Runic.Agent.Standalone.Logging
{
    public class LoggingHandler : ILoggingHandler
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

        public void Error(string message)
        {
            _log.LogError(message);
        }

        public void Error(string message, object obj)
        {
            _log.LogError(message, obj);
        }

        public void Info(string message)
        {
            _log.LogInformation(message);
        }

        public void Info(string message, object obj)
        {
            _log.LogInformation(message, obj);
        }

        public void Warning(string message)
        {
            _log.LogWarning(message);
        }

        public void Warning(string message, object obj)
        {
            _log.LogWarning(message, obj);
        }
    }
}
