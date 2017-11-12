﻿using System;

namespace Runic.Agent.Core.AssemblyManagement
{
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
    }
}