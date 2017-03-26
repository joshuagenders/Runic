using System;

namespace Runic.Agent.Harness
{
    internal class FunctionWithAttributeNotFoundException : Exception
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