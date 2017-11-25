using System.Collections.Generic;

namespace Runic.Agent.Core.Models
{
    public class DataInput
    {
        public string InputDatasourceKey { get; set; }
        public string InputDatasourceType { get; set; }
        public Dictionary<string, string> DatasourceMapping { get; set; }
    }
}
