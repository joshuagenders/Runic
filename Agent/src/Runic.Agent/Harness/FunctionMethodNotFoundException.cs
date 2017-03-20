using System;

namespace Runic.Agent.Harness
{
    internal class FunctionMethodNotFoundException : Exception
    {
        public FunctionMethodNotFoundException()
        {
        }

        public FunctionMethodNotFoundException(string message) : base(message)
        {
        }

        public FunctionMethodNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}