using Runic.Agent.Core.Configuration;
using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public class TestPlanProducer
    {
        private readonly Population _workQueue;
        private readonly IDatetimeService _datetimeService;
        private readonly ICoreConfiguration _config;
        private List<TestPlan> TestPlans { get; set; }
        private DateTime? _lastPollTime { get; set; }

        public TestPlanProducer(Population workQueue, IDatetimeService datetimeService, ICoreConfiguration config)
        {
            _workQueue = workQueue;
            _datetimeService = datetimeService;
            _config = config;
            TestPlans = new List<TestPlan>();
        }

        public async Task ProduceWorkItems(CancellationToken ctx = default(CancellationToken))
        {
            while (!ctx.IsCancellationRequested)
            {
                PopulateQueue();
                await _datetimeService.WaitMilliseconds(_config.PopulationPollingIntervalSeconds * 1000);
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
                if (!lastTime.HasValue)
                {
                    //first population, start 1 journey of all
                    _workQueue.AddTask(testPlan);
                    continue;
                }
                //count is the number of journeys that should be enqueued by now,
                //minus the number of journeys that should have been enqueued by last time
                var count =
                    (lastTime.GetValueOrDefault().Subtract(testPlan.StartTime).Seconds / testPlan.JourneyFrequencySeconds)
                    - (now.Subtract(testPlan.StartTime).Seconds / testPlan.JourneyFrequencySeconds);
                Parallel.For(0, count, i =>
                {
                    _workQueue.AddTask(testPlan);
                });
            }
        }
    }
}
