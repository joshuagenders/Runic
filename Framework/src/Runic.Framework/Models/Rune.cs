using System;

namespace Runic.Framework.Models
{
    public class Rune
    {
        public Rune(string name)
        {
            RuneExpiry = DateTime.Now.AddDays(30);
            Name = name;
        }

        public string Name { get; set; }
        public DateTimeOffset RuneExpiry { get; set; }
    }
}