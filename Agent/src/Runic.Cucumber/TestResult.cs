using System;
using System.Collections.Generic;

namespace Runic.Cucumber
{
    public class TestResult
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
        public string FailedStep { get; set; }
        public List<string> Steps { get; set; }
    }
}
