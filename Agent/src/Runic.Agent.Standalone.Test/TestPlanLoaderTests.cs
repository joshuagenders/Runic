using Newtonsoft.Json;
using Runic.Agent.Core.Models;
using Shouldly;
using System;
using System.Collections.Immutable;
using Xunit;

namespace Runic.Agent.Standalone.Tests
{
    public class TestPlanLoaderTests
    {
        [Fact]
        public void WhenSerializingAndDeserializing_ThenNoExceptions()
        { 
            var steps = ImmutableList.Create(new Step[] { });
            var journey = new Journey("test journey", 1, steps, "assembly name");
            var testPlan = new TestPlan(journey, 2, new FrequencyPattern(PatternType.Constant, 2, 3, 4, 5, null));

            var jsonString = JsonConvert.SerializeObject(testPlan);
            Action action = () => TestPlanLoader.DeserializeTestPlan(jsonString);
            action.ShouldNotThrow();
        }
    }
}
