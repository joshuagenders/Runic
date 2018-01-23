using Newtonsoft.Json;
using Runic.Agent.Core.WorkGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Runic.Agent.Standalone
{
    public static class TestPlanLoader
    {
        public static IEnumerable<TestPlan> GetTestPlans (Configuration configuration)
        {
            var files = Directory.GetFiles(configuration.TestPlanPath)
                                 .Where(f => f.EndsWith(".testplan.json"));
            if (!files.Any())
            {
                throw new ArgumentException(
                    $"No work files (.work.json) were located in the configured directory: {configuration.TestPlanPath}");
            }
            
            var work = files.Select(File.ReadAllText)
                            .Select(JsonConvert.DeserializeObject<TestPlan>)
                            .ToList();
            return work;
        }
    }
}
