﻿using System;

namespace Runic.Agent.Core.Exceptions
{
    public class ClassNotFoundInAssemblyException : Exception
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