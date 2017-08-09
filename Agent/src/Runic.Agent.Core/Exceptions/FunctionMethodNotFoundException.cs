using System;

namespace Runic.Agent.Core.Exceptions
{
    public class FunctionWithAttributeNotFoundException : Exception
    {
        public FunctionWithAttributeNotFoundException()
        {
        }

        public FunctionWithAttributeNotFoundException(string message) : base(message)
        {
        }

        public FunctionWithAttributeNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}