using System;

namespace Runic.Agent.Core.Exceptions
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