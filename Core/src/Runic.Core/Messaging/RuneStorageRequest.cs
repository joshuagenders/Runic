using System.Collections.Generic;

namespace Runic.Core.Messaging
{
    public class RuneStorageRequest<T>
    {
        public List<T> Runes { get; set; }
    }
}
