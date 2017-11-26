using System;
using System.Runtime.Serialization;

namespace Runic.Agent.Standalone
{
    [Serializable]
    public class NoWorkException : Exception
    {
        public NoWorkException()
        {
        }

        public NoWorkException(string message) : base(message)
        {
        }

        public NoWorkException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoWorkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}