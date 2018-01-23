using Runic.Agent.Core.Models;
using Runic.Agent.FunctionalTest.TestUtility;
using System.Collections.Generic;
using System.Reflection;

namespace Runic.Agent.FunctionalTest.Repositories
{
    public class JourneyRepository : KeyRepository<Journey>
    {
        public JourneyRepository()
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            _values = new Dictionary<string, Journey>()
            {
                {
                    "Google", new Journey()
                    {
                        AssemblyName = assembly.FullName,
                        Name = "Google Journey",
                        StepDelayMilliseconds = 50,
                        Steps = new List<Step>()
                        {
                            new Step()
                            {
                                StepName = "Step 1",
                                Function = new MethodStepInformation()
                                {
                                    AssemblyName = assembly.FullName,
                                    AssemblyQualifiedClassName = typeof(FakeTest).AssemblyQualifiedName,
                                    MethodName = "GoogleSomething",
                                    PositionalMethodParameterValues = new List<string>() { "Software Testing" },
                                    GetNextStepFromMethodResult = false
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
