﻿using System;

namespace Runic.Framework.Orchestration
{
    public class ActionResult
    {
        public long ElapsedMilliseconds { get; set; }
        public Exception ExecutionException { get; set; }
    }
}
