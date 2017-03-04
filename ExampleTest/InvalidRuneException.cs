using System;

namespace Runic.ExampleTest.Functions
{
    internal class InvalidRuneException : Exception
    {
        public InvalidRuneException()
        {
        }

        public InvalidRuneException(string message) : base(message)
        {
        }

        public InvalidRuneException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}