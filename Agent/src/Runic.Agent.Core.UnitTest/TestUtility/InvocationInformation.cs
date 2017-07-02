using System;

namespace Runic.Agent.Core.UnitTest.TestUtility
{
    public class InvocationInformation
    {
        public DateTime InvocationTime { get; set; }
        public string StackTrace { get; set; }
        public string InvocationTarget { get; set; }
        public string AdditionalData { get; set; }
    }
}
