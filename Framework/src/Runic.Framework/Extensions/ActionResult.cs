using System;

namespace Runic.Framework.Extensions
{
    public class ActionResult
    {
        public long ElapsedMilliseconds { get; set; }
        public Exception ExecutionException { get; set; }
    }
}
