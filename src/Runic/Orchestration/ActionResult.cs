using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Runic.Orchestration
{
    public class ActionResult
    {
        public long ElapsedMilliseconds { get; set; }
        public object ExecutionResult { get; set; }

    }
}
