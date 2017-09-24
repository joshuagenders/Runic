using Runic.Agent.Core.Configuration;
using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public class PopulationScheduler
    {
        private readonly IDatetimeService _datetimeService;
        private readonly IPersonFactory _personFactory;
        private readonly ICoreConfiguration _config;
        private DateTime _lastPollTime { get; set; }
        private List<TestPlan> TestPlans { get; set; }
        private ConcurrentQueue<TestPlan> PendingJourneys { get; set; }

        public PopulationScheduler(IDatetimeService datetimeService, IPersonFactory personFactory, ICoreConfiguration config)
        {
            _datetimeService = datetimeService;
            _personFactory = personFactory;
            _config = config;
            TestPlans = new List<TestPlan>();
            PendingJourneys = new ConcurrentQueue<TestPlan>();
        }

        public async Task ProcessQueue(CancellationToken ctx)
        {
            while (!ctx.IsCancellationRequested)
            {
                if (PendingJourneys.TryDequeue(out TestPlan pendingJourney))
                {
                    var person = _personFactory.GetPerson(pendingJourney.Journey);
                    //todo process all tasks concurrently
                    await pendingJourney.Population.ActivatePerson(person, pendingJourney.Journey, ctx);
                }
            }
        }

        public void AddPlan(TestPlan plan)
        {
            plan.StartTime = _datetimeService.Now;
            TestPlans.Add(plan);
        }

        public void PopulateQueue()
        {
            var lastTime = _lastPollTime;
            var now = _datetimeService.Now;
            _lastPollTime = now;

            foreach (var testPlan in TestPlans)
            {
                //count is the number of journeys that should be enqueued by now,
                //minus the number of journeys that should have been enqueued by last time
                var count = 
                    (lastTime.Subtract(testPlan.StartTime).Seconds / testPlan.JourneyFrequencySeconds) 
                    - (now.Subtract(testPlan.StartTime).Seconds / testPlan.JourneyFrequencySeconds);
                Parallel.For(0, count, i =>
                {
                    PendingJourneys.Enqueue(testPlan);
                });
            }
        }
    }
}
