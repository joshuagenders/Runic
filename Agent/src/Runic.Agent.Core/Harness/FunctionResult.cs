using System;

namespace Runic.Agent.Core.Harness
{
    public class FunctionResult
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
        public string FunctionName { get; set; }
        public object[] FunctionParameters { get; set; }
        public long ExecutionTimeMilliseconds { get; set; }
    }
}
