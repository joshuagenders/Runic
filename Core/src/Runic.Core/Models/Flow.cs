using System.Collections.Generic;

namespace Runic.Core.Models
{
    public class Flow
    {
        public List<Step> Steps { get; set; }
    }

    public class Step
    {
        /// <summary>
        /// </summary>
        public string TestName { get; set; }

        public string TestAssemblyName { get; set; }
        public string InputDatasource { get; set; }
        public Dictionary<string, string> DatasourceMapping { get; set; }
        public int Repeat { get; set; }
        public double DistributionPercentage { get; set; }
    }
}