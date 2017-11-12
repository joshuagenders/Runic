using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.Patterns;
using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public class TestPlanProducer
    {
        private readonly TestPlanConsumer _taskQueue;
        private readonly IDatetimeService _datetimeService;
        private readonly ICoreConfiguration _config;
        
        private ConcurrentDictionary<string, TestPlan> TestPlans { get; set; }
        private DateTime? _lastPollTime { get; set; }

        public TestPlanProducer(TestPlanConsumer taskQueue, IDatetimeService datetimeService, ICoreConfiguration config)
        {
            _taskQueue = taskQueue;
            _datetimeService = datetimeService;
            _config = config;
            TestPlans = new ConcurrentDictionary<string, TestPlan>();
        }

        public TestPlan GetPlan(string id)
        {
            if (TestPlans.TryGetValue(id, out TestPlan plan))
                return plan;
            else
                return null;
        }

        public async Task ProduceWorkItems(CancellationToken ctx = default(CancellationToken))
        {
            while (!ctx.IsCancellationRequested)
            {
                PopulateQueue(ctx);
                await _datetimeService.WaitMilliseconds(_config.TaskCreationPollingIntervalSeconds * 1000);
            }
        }

        public void AddUpdatePlan(string id, TestPlan plan)
        {
            plan.StartTime = _datetimeService.Now;
            TestPlans.AddOrUpdate(id, plan, (x, y) => plan);
        }

        private int GetJourneyCountInPeriod(IFrequencyPattern frequencyPattern, DateTime startTime, DateTime endTime)
        {
            var totalSeconds = endTime.Subtract(startTime).TotalSeconds;
            double total = 0;
            for (int i = 0; i < totalSeconds; i++)
            {
                total += frequencyPattern.GetCurrentFrequencyPerMinute(startTime, startTime.AddSeconds(i)) / 60;
            }

            return (int)total;
        }

        public void PopulateQueue(CancellationToken ctx)
        {
            var lastTime = _lastPollTime;
            var now = _datetimeService.Now;
            _lastPollTime = now;

            foreach (var testPlan in TestPlans.Values)
            {
                if (!lastTime.HasValue)
                {
                    continue;
                }
                var count = 
                    GetJourneyCountInPeriod(testPlan.Frequency, testPlan.StartTime, now) 
                        - GetJourneyCountInPeriod(testPlan.Frequency, testPlan.StartTime, lastTime.Value);

                Parallel.For(0, (int)count, i =>
                {
                    _taskQueue.EnqueueTask(testPlan, ctx);
                });
                if (ctx.IsCancellationRequested)
                    break;
            }
        }
    }
}
