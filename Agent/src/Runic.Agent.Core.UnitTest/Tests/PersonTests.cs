using Moq;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class PersonTests
    {
        [Fact]
        public async Task WhenJourneyIsExecuted_ThenCorrectAssemblyIsLoaded()
        {
            var mockAssemblyService = new Mock<AssemblyService>();
            mockAssemblyService.Setup(m => m.GetLoadAssembly(It.IsAny<string>()))
                               .Returns(GetType().GetTypeInfo().Assembly);
            var person = new Person(mockAssemblyService.Object);
            var assemblyPath = "myassemblypath";
            var steps = ImmutableList.Create (
                    new Step("Step 1", new MethodStepInformation(assemblyPath, "class", "method"), null)
                );
            var journey = new Journey("TestJourney", 300, steps);
            await person.PerformJourneyAsync(journey);
            mockAssemblyService.Verify(m => m.GetLoadAssembly(assemblyPath));
        }
    }
}
