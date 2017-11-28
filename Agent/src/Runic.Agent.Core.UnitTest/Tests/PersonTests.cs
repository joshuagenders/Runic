﻿using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using Runic.Agent.UnitTest.TestUtility;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class PersonTests
    {
        [Fact]
        public async Task WhenPerformingJourney_ThenAssemblyIsLoadedAndFunctionIsCreated()
        {
            var ff = new Mock<IFunctionFactory>();
            var dts = new Mock<IDatetimeService>();
            var am = new Mock<IAssemblyManager>();
            var testClass = new FakeFunction();
            var fakeFunction = new FunctionHarness(testClass, new Step()
            {
                StepName = "Step1",
                Function = new MethodInformation()
                {
                    MethodName = "NoInputs",
                    PositionalMethodParameterValues = new List<string>()
                }
            });
            ff.Setup(f => f.CreateFunction(It.IsAny<Step>(), It.IsAny<TestContext>())).Returns(fakeFunction);
            var person = new Person(ff.Object, dts.Object, am.Object);
            var journey = new Journey()
            {
                Name = "Test Journey",
                AssemblyName = "someassembly",
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        StepName = "Step1",
                        Function = new MethodInformation()
                        {
                            AssemblyName = "someassembly"
                        }
                    }
                }
,
            };
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            await person.PerformJourneyAsync(journey, cts.Token);
            am.Verify(a => a.LoadAssembly("someassembly"), Times.AtLeastOnce);
            ff.Verify(f => f.CreateFunction(It.IsAny<Step>(), It.IsAny<TestContext>()), Times.AtLeastOnce);
        }   
    }
}
