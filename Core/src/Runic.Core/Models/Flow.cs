using System.Collections.Generic;

namespace Runic.Core.Models
{
    public class Flow
    {
        public List<Step> Steps { get; set; }
    }

    public class Step
    {
        public string TestName { get; set; }
        public string TestAssemblyName { get; set; }
        public Dictionary<string,string> InputOverrides { get; set; }
        public int Repeat { get; set; }
        public double DistributionPercentage { get; set; }
    }
}
