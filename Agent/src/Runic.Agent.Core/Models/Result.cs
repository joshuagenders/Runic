using System;

namespace Runic.Agent.Core.Models
{
    public class Result
    {
        public Result(
            bool success,
            long executionTimeMilliseconds,
            string exceptionMessage,
            Step step)
        {
            Success = success;
            ExecutionTimeMilliseconds = executionTimeMilliseconds;
            ExceptionMessage = exceptionMessage;
            Step = step;
        }

        public bool Success { get; }

        public long ExecutionTimeMilliseconds { get; }

        public string ExceptionMessage { get; }

        public Step Step { get; }
    }
}
