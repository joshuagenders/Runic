using FluentAssertions;
using Moq;
using Runic.Agent.Core.Patterns;
using Runic.Agent.Core.WorkGenerator;
using Runic.Agent.FunctionalTest.Repositories;
using Runic.Agent.FunctionalTest.TestUtility;
using Runic.Agent.Standalone;
using System;
using System.Threading;

namespace Runic.Agent.FunctionalTest.Steps
{
    public class Steps
    {
        private readonly TestEnvironment _sut;
        private Mock<IWorkLoader> MockWorkLoader { get; } = new Mock<IWorkLoader>();

        private Work _work { get; set; }
        public Steps()
        {
            _sut = new TestEnvironmentBuilder().Build(testEnvironment: new TestEnvironment()
            {
                AssemblyManager = new MockAssemblyManager().Object,
                WorkLoader = MockWorkLoader.Object
            });
        }

        public Steps GivenIHaveAJourneyFor(string journeyKey)
        {
            _work = new Work(new JourneyRepository().Get(journeyKey));
            return this;
        }

        public Steps GivenIHaveAFrequencyPattern(string patternType)
        {
            switch (patternType.ToLower())
            {
                case "constant":
                    _work.Frequency = new ConstantFrequencyPattern();
                    break;
                case "graph":
                    _work.Frequency = new GraphFrequencyPattern();
                    break;
                case "gradual":
                    _work.Frequency = new GradualFrequencyPattern();
                    break;
            }
            return this;
        }

        public Steps GivenTheJourneyLastsForSeconds(int journeyLengthSeconds)
        {
            _work.Frequency.DurationSeconds = journeyLengthSeconds;
            return this;
        }

        public Steps GivenTheFrequencyIsJourneysPerMinute(int journeysPerMinute)
        {
            var gradual = _work.Frequency as GradualFrequencyPattern;
            if (gradual != null)
            {
                gradual.JourneysPerMinute = journeysPerMinute;
                return this;
            }

            var constant =_work.Frequency as ConstantFrequencyPattern;
            if (constant != null)
            {
                constant.JourneysPerMinute = journeysPerMinute;
                return this;
            }

            return this;
        }

        public Steps WhenIEmbarkOnTheJourney()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(20000);
            _sut.WorkProducer.AddUpdateWorkItem(Guid.NewGuid().ToString("N"), _work);
            var task = _sut.Runner.Start(cts.Token);
            task.GetAwaiter().GetResult();
            if (task.Exception != null)
                throw task.Exception;
            return this;
        }

        public Steps ThenTheGooglePageIsReturnedAtleastTimes(int times)
        {
            FakeTest.GoogleSomethingCounter.Should().BeGreaterThan(times);
            return this;
        }
    }
}
