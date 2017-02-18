using System;

namespace Runic.Agent.AssemblyManagement
{
    internal class AssemblyNotFoundException : Exception
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