using System;

namespace Runic.Core
{
    public class Rune
    {
        public string Name { get; set; }
        public DateTimeOffset RuneExpiry { get; set; }

        public Rune(string name)
        {
            RuneExpiry = DateTime.Now.AddDays(1);
            Name = name;
        }
    }
}
