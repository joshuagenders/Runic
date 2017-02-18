using System;

namespace Runic.Agent.AssemblyManagement
{
    internal class TestNotFoundInAssemblyException : Exception
    {
        public TestNotFoundInAssemblyException()
        {
        }

        public TestNotFoundInAssemblyException(string message) : base(message)
        {
        }

        public TestNotFoundInAssemblyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}