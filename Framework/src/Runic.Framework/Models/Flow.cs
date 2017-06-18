using System;
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
        public string StepName { get; set; }
        public bool GetNextStepFromFunctionResult { get; set; }
        public int Repeat { get; set; }
        public DataInput DataInput { get; set; }
        public Distribution Distribution { get; set; }
        public FunctionInformation Function { get; set; }
        
    }

    public class FunctionInformation
    {
        public string AssemblyName { get; set; }
        public string AssemblyQualifiedClassName { get; set; }
        public string FunctionName { get; set; }
        public Dictionary<string, Type> Parameters { get; set; }
        public List<string> RequiredRunes { get; set; }
    }

    public class DataInput
    {
        public string InputDatasource { get; set; }
        public Dictionary<string, string> DatasourceMapping { get; set; }
    }

    public class Distribution
    {
        //global, or local key
        public string DistributionGroup { get; set; }
        //the amount of units distrubuted as a percentage of a group
        //eg. 1 group with two allocations of 1 means 50% each function in group
        public double DistributionAllocation { get; set; }
    }
}