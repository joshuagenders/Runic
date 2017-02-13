using System.Collections.Generic;

namespace Runic.Core
{
    public class RuneQueryResponse
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public List<Rune> Runes { get; set; }
    }
}
