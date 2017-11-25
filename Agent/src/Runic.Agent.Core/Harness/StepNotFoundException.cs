using System;

namespace Runic.Agent.Core.Harness
{
    public class StepNotFoundException : Exception
    {
        public StepNotFoundException()
        {
        }

        public StepNotFoundException(string message) : base(message)
        {
        }

        public StepNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}