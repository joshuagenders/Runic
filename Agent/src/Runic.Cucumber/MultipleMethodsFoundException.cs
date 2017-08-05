using System;

namespace Runic.Cucumber
{
    internal class MultipleMethodsFoundException : Exception
    {
        public MultipleMethodsFoundException()
        {
        }

        public MultipleMethodsFoundException(string message) : base(message)
        {
        }

        public MultipleMethodsFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}