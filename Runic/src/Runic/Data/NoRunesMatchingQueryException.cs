using System;

namespace Runic.Data
{
    internal class NoRunesMatchingQueryException : Exception
    {
        public NoRunesMatchingQueryException()
        {
        }

        public NoRunesMatchingQueryException(string message) : base(message)
        {
        }

        public NoRunesMatchingQueryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}