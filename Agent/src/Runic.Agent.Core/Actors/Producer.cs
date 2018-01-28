using Akka.Actor;
using Akka.Event;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.Messages;
using System;
using System.Linq;

namespace Runic.Agent.Core.Actors
{
    public class Producer : ReceiveActor
    {
        private DateTime? _lastPollTime { get; set; }
        private DateTime _startTime { get; set; }
        private readonly TestPlan _testPlan;

        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void PreStart() => Log.Info("Producer started");
        protected override void PostStop() => Log.Info("Producer stopped");


        public Producer(TestPlan testPlan)
        {
            _startTime = DateTime.Now;
            _testPlan = testPlan;
            Receive<Produce>(_ => PopulateWorkQueue());
        }

        private int GetJourneyCountInPeriod(FrequencyPattern frequencyPattern, DateTime startTime, DateTime endTime)
        {            
            int totalSeconds = Convert.ToInt32(endTime.Subtract(startTime).TotalSeconds);
            return Convert.ToInt32(
                        Enumerable.Range(0, totalSeconds)
                                  .Select(s => 
                                    new FrequencyService().GetCurrentFrequencyPerMinute(frequencyPattern, s) / 60)
                                  .Sum());
        }

        private int GetJourneyCount()
        {
            var lastTime = _lastPollTime;
            var now = DateTime.Now;
            
            var currentTotal = GetJourneyCountInPeriod(_testPlan.Frequency, _startTime, now);
            var lastJourneyCount = GetJourneyCountInPeriod(_testPlan.Frequency, _startTime, lastTime ?? now);

            _lastPollTime = now;
            return currentTotal - lastJourneyCount;
        }

        private void PopulateWorkQueue()
        {
            var count = GetJourneyCount();
            for (int i = 0; i < count; i++)
            {
                Context.ActorSelection("../../consumer-supervisor")
                       .Tell(new ExecuteTestPlan(_testPlan));
            }
            if (_startTime.AddSeconds(_testPlan.Frequency.DurationSeconds) > DateTime.Now)
            {
                Context.Stop(Self);
            }
        }
    }
}
