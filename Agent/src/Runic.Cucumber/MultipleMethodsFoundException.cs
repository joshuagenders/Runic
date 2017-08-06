using System;

namespace Runic.Cucumber
{
    public class MultipleMethodsFoundException : Exception
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