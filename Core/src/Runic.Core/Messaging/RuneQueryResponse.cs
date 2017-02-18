﻿using System.Collections.Generic;

namespace Runic.Core.Messaging
{
    public class RuneQueryResponse
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public RuneQuery RuneQueryResult { get; set; }
    }
}