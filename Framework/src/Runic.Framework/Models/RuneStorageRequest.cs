using System.Collections.Generic;

namespace Runic.Framework.Models
{
    public class RuneStorageRequest<T>
    {
        public List<T> Runes { get; set; }
    }
}