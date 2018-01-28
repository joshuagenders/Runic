using System;

namespace Runic.Agent.Core.Models
{
    public abstract class Result
    {
        //todo immutable?
        public bool Success { get; set; }
        public long ExecutionTimeMilliseconds { get; set; }
        public Exception Exception { get; set; }
        public Step Step { get; set; }
    }
}
