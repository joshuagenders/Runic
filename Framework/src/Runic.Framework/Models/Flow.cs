using System.Collections.Generic;

namespace Runic.Framework.Models
{
    public class Flow
    {
        public string Name { get; set; }
        public int StepDelayMilliseconds { get; set; }
        public List<Step> Steps { get; set; }
    }

    public class Step
    {
        public FunctionInformation Function { get; set; }
        public string InputDatasource { get; set; }
        public Dictionary<string, string> DatasourceMapping { get; set; }
        public int Repeat { get; set; }
        public double DistributionPercentage { get; set; }
    }
}