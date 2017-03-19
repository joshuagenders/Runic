using System;

namespace Runic.Framework.Models
{
    public class Rune
    {
        public Rune()
        {
            //todo config
            RuneExpiry = DateTime.Now.AddDays(30);
        }

        public string Name { get; set; }
        public DateTimeOffset RuneExpiry { get; set; }
    }
}