using System.Collections.Generic;

namespace Runic.Core.Models
{
    public class RuneStorageRequest<T>
    {
        public List<T> Runes { get; set; }
    }
}