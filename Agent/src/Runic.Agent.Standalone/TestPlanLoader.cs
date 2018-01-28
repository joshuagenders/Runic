using Newtonsoft.Json;
using Runic.Agent.Core.Models;
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
            
            return files.Select(File.ReadAllText)
                        .Select(DeserializeTestPlan)
                        .ToList();
        }

        public static TestPlan DeserializeTestPlan(string fileText)
        {
            return JsonConvert.DeserializeObject<TestPlan>(fileText);
        }
    }
}
