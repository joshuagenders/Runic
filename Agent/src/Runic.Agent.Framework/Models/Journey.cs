using System;
using System.Collections.Generic;

namespace Runic.Agent.Framework.Models
{
    public class Journey
    {
        public string Name { get; set; }
        public int StepDelayMilliseconds { get; set; }
        public List<Step> Steps { get; set; }
        public JourneyFrequency Frequency { get; set; }
    }

    public class JourneyFrequency
    {
        public int JourneysPerMinute { get; set; }
        public int JourneysPerHour { get; set; }
    }

    public class Step
    {
        public string StepName { get; set; }
        public int RepeatCount { get; set; }
        public DataInput DataInput { get; set; }
        public Distribution Distribution { get; set; }
        public FunctionInformation Function { get; set; }
        public CucmberInformation Cucumber { get; set; }
    }

    public class CucmberInformation
    {
        public string Document { get; set; }
        public string AssemblyName { get; set; }
    }

    public class FunctionInformation
    {
        public string AssemblyName { get; set; }
        public string AssemblyQualifiedClassName { get; set; }
        public string FunctionName { get; set; }
        public Dictionary<string, Type> MethodParameters { get; set; }
        public List<string> InputParameters { get; set; }
        public List<string> RequiredRunes { get; set; }
        public bool GetNextStepFromFunctionResult { get; set; }
    }

    public class DataInput
    {
        public List<string> PersonAttributeKeys { get; set; }
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