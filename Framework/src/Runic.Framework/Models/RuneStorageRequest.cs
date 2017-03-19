using System.Collections.Generic;
using Runic.Framework.Models;

namespace Runic.Framework.Models
{
    internal class RuneStorageRequest<T> where T : Rune
    {
        public List<T> Runes { get; set; }
    }
}