using System;

namespace Runic.Agent.Core.AssemblyManagement
{
    internal class ClassNotFoundInAssemblyException : Exception
    {
        public ClassNotFoundInAssemblyException()
        {
        }

        public ClassNotFoundInAssemblyException(string message) : base(message)
        {
        }

        public ClassNotFoundInAssemblyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}