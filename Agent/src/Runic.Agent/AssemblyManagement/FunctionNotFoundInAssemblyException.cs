using System;

namespace Runic.Agent.AssemblyManagement
{
    internal class FunctionNotFoundInAssemblyException : Exception
    {
        private object function;

        public FunctionNotFoundInAssemblyException()
        {
        }

        public FunctionNotFoundInAssemblyException(object function)
        {
            this.function = function;
        }

        public FunctionNotFoundInAssemblyException(string message) : base(message)
        {
        }

        public FunctionNotFoundInAssemblyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}