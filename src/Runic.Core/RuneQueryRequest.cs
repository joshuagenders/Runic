using System.Collections.Generic;

namespace Runic.Core
{
    public class RuneQueryRequest
    {
        public string Id { get; set; }
        public List<RuneQuery> RuneQueries { get; set;}
    }
}
