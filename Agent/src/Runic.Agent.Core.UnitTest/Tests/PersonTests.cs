using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Models;
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
            var mockAssemblyManager = new Mock<IAssemblyManager>();
            mockAssemblyManager.Setup(m => m.GetLoadAssembly(It.IsAny<string>()))
                               .Returns(GetType().GetTypeInfo().Assembly);
            var person = new Person(mockAssemblyManager.Object);
            var assemblyPath = "myassemblypath";
            var steps = ImmutableList.Create (
                    new Step("Step 1", new MethodStepInformation(assemblyPath, "class", "method"), null)
                );
            var journey = new Journey("TestJourney", 300, steps);
            await person.PerformJourneyAsync(journey);
            mockAssemblyManager.Verify(m => m.GetLoadAssembly(assemblyPath));
        }
    }
}
