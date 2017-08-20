﻿using Runic.Agent.Core.Services;
using System.Collections.Generic;

namespace Runic.Agent.Core.CucumberHarness
{
    public class CucumberResult : Result
    {
        public string FailedStep { get; set; }
        public List<string> Steps { get; set; }
    }
}