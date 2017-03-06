using System;

namespace Runic.Agent.Harness
{
    internal class FunctionTypeNotFoundException : Exception
    {
        public FunctionTypeNotFoundException()
        {
        }

        public FunctionTypeNotFoundException(string message) : base(message)
        {
        }

        public FunctionTypeNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}