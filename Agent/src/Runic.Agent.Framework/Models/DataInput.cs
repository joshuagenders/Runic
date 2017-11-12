using System.Collections.Generic;

namespace Runic.Agent.Framework.Models
{
    public class DataInput
    {
        public string InputDatasourceKey { get; set; }
        public string InputDatasourceType { get; set; }
        public Dictionary<string, string> DatasourceMapping { get; set; }
    }
}
