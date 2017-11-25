namespace Runic.Agent.Core.Models
{
    public class Distribution
    {
        //global, or local key
        public string DistributionGroup { get; set; }
        //the amount of units distrubuted as a percentage of a group
        //eg. 1 group with two allocations of 1 means 50% each function in group
        public double DistributionAllocation { get; set; }
    }
}
