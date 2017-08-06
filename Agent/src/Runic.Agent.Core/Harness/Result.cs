using System;

namespace Runic.Agent.Core.Harness
{
    public abstract class Result
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
        public long ExecutionTimeMilliseconds { get; set; }
    }
}
