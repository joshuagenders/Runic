using System;

namespace Runic.Agent.TestHarness.StepController
{
    public class StepDequeueFailedException : Exception
    {
        public StepDequeueFailedException()
        {
        }

        public StepDequeueFailedException(string message) : base(message)
        {
        }

        public StepDequeueFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}