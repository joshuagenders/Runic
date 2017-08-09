using Runic.Framework.Models;
using System;

namespace Runic.Agent.Core.Services
{
    public abstract class Result
    {
        public bool Success { get; set; }
        public long ExecutionTimeMilliseconds { get; set; }
        public Exception Exception { get; set; }
        public Step Step { get; set; }
        public string NextStep { get; set; }
    }
}
