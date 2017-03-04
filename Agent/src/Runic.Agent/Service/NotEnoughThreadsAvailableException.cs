﻿using System;

namespace Runic.Agent.Service
{
    internal class NotEnoughThreadsAvailableException : Exception
    {
        public NotEnoughThreadsAvailableException()
        {
        }

        public NotEnoughThreadsAvailableException(string message) : base(message)
        {
        }

        public NotEnoughThreadsAvailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}