using System;

namespace Runic.Agent.Core.AssemblyManagement
{
    public class AssemblyNotFoundException : Exception
    {
        public AssemblyNotFoundException()
        {
        }

        public AssemblyNotFoundException(string message) : base(message)
        {
        }

        public AssemblyNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}