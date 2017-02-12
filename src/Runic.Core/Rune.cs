using System;
using System.Collections.Generic;

namespace Runic.Core
{
    public class Rune
    {
        public string Name { get; set; }
        public Dictionary<string,string> IndexedProperties { get; set; }
        public object Detail { get; set; }
        public DateTime RuneExpiry { get; set; }
    }
}
