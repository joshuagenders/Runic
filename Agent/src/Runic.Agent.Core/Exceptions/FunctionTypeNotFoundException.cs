using System;

namespace Runic.Agent.Core.Exceptions
{
    public class FunctionTypeNotFoundException : Exception
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