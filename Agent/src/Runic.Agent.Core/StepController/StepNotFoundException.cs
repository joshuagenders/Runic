﻿using System;

namespace Runic.Agent.Core.StepController
{
    public class StepNotFoundException : Exception
    {
        public StepNotFoundException()
        {
        }

        public StepNotFoundException(string message) : base(message)
        {
        }

        public StepNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}