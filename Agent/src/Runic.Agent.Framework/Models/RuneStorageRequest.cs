using System.Collections.Generic;

namespace Runic.Agent.Framework.Models
{
    public class RuneStorageRequest<T> where T : Rune
    {
        public List<T> Runes { get; set; }
    }
}