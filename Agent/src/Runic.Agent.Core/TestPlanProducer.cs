using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.Patterns;
using Runic.Agent.Services;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public class TestPlanProducer : IProducer<TestPlan>
    {
        private readonly IConsumer<TestPlan> _consumer;
        private readonly IDatetimeService _datetimeService;
        private readonly ICoreConfiguration _config;
        
        private ConcurrentDictionary<string, TestPlan> TestPlans { get; set; }
        private DateTime? _lastPollTime { get; set; }

        public TestPlanProducer(IConsumer<TestPlan> consumer, IDatetimeService datetimeService, ICoreConfiguration config)
        {
            _consumer = consumer;
            _datetimeService = datetimeService;
            _config = config;
            TestPlans = new ConcurrentDictionary<string, TestPlan>();
        }

        public TestPlan GetWorkItem(string id) => (TestPlans.TryGetValue(id, out TestPlan plan)) ? plan : null;
        public void AddUpdateWorkItem(string id, TestPlan item)
        {
            item.StartTime = _datetimeService.Now;
            TestPlans.AddOrUpdate(id, item, (x, y) => y);
        }

        public async Task ProduceWorkItems(CancellationToken ctx)
        {
            while (!ctx.IsCancellationRequested)
            {
                PopulateWorkQueue(ctx);
                await _datetimeService.WaitMilliseconds(_config.TaskCreationPollingIntervalSeconds * 1000);
            }
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

        public void PopulateWorkQueue(CancellationToken ctx)
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

                for (int i = 0; i < count; i++)
                    _consumer.EnqueueTask(testPlan);

                if (ctx.IsCancellationRequested)
                    break;
            }
        }
    }
}
