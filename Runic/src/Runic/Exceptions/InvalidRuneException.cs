using System;

namespace Runic.Exceptions
{
    public class InvalidRuneException : Exception
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