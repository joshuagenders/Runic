﻿using System;
using System.Collections.Generic;

namespace Runic.Query
{
    public class RuneQuery
    {
        public bool EnableRegex { get; set; }
        public Dictionary<string, string> RequiredProperties { get; set; }
        public string RuneName { get; set; }
        public string[] LinkedProperties { get; set; }
    }
}
