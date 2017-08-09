using System;

namespace Runic.Agent.Core.Exceptions
{
    public class MaxErrorCountExceededException : Exception
    {
        public MaxErrorCountExceededException()
        {
        }

        public MaxErrorCountExceededException(string message) : base(message)
        {
        }

        public MaxErrorCountExceededException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}