using System.Collections.Generic;

namespace Runic.Core.Models
{
    public class Flow
    {
        public string Name { get; set; }
        public List<Step> Steps { get; set; }
    }

    public class Step
    {
        public string FunctionName { get; set; }
        public string FunctionAssemblyName { get; set; }
        public string InputDatasource { get; set; }
        public Dictionary<string, string> DatasourceMapping { get; set; }
        public int Repeat { get; set; }
        public double DistributionPercentage { get; set; }
    }
}