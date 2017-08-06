using System.Collections.Generic;

namespace Runic.Framework.Models
{
    public class RuneStorageRequest<T> where T : Rune
    {
        public List<T> Runes { get; set; }
    }
}