using System;
using System.Runtime.Serialization;

namespace Runic.Agent.Core.AssemblyManagement
{
    [Serializable]
    public class AssemblyLoadException : Exception
    {
        public AssemblyLoadException()
        {
        }

        public AssemblyLoadException(string message) : base(message)
        {
        }

        public AssemblyLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AssemblyLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}