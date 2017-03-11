﻿using System.Collections.Generic;

namespace Runic.Framework.Models
{
    public class RuneQuery
    {
        public Dictionary<string, string> RequiredProperties { get; set; }
        public string RuneName { get; set; }
        public List<RuneQuery> RequiredLinks { get; set; }
        public Rune Result { get; set; }
    }
}