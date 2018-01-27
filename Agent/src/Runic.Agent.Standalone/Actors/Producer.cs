using Akka.Actor;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.WorkGenerator;
using Runic.Agent.Standalone.Messages;
using System;
using System.Linq;

namespace Runic.Agent.Standalone.Actors
{
    public class Producer : ReceiveActor
    {
        private DateTime? _lastPollTime { get; set; }
        private DateTime _startTime { get; set; }
        private readonly TestPlan _testPlan;
        private readonly IActorRef _consumerSupervisor;
        public Producer(IActorRef consumerSupervisor, TestPlan testPlan)
        {
            _startTime = DateTime.Now;
            _testPlan = testPlan;
            _consumerSupervisor = consumerSupervisor;
            Receive<Produce>(_ => PopulateWorkQueue());
        }

        //todo terminate when duration passes

        private int GetJourneyCountInPeriod(FrequencyPattern frequencyPattern, DateTime startTime, DateTime endTime)
        {            
            int totalSeconds = Convert.ToInt32(endTime.Subtract(startTime).TotalSeconds);
            return Convert.ToInt32(
                        Enumerable.Range(0, totalSeconds)
                                  .Select(s => new FrequencyService().GetCurrentFrequencyPerMinute(
                                                frequencyPattern, s) / 60)
                                  .Sum());
        }

        private int GetJourneyCount()
        {
            var lastTime = _lastPollTime;
            var now = DateTime.Now;
            _lastPollTime = now;

            return GetJourneyCountInPeriod(_testPlan.Frequency, _startTime, now)
                    - GetJourneyCountInPeriod(_testPlan.Frequency, _startTime, lastTime ?? now);
        }

        private void PopulateWorkQueue()
        {
            var count = GetJourneyCount();
            for (int i = 0; i < count; i++)
            {
                _consumerSupervisor.Tell(new ExecuteTestPlan() { TestPlan = _testPlan });
            }
        }
    }
}
