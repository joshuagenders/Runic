using System;

namespace Runic.Framework.Orchestration
{
    public class FuncResult<T>
    {
        public long ElapsedMilliseconds { get; set; }
        public T ExecutionResult { get; set; }
        public Exception ExecutionException { get; set; }
    }
}