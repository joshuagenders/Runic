using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public class TestPlanConsumer : IConsumer<TestPlan>
    {
        private readonly IPersonFactory _personFactory;
        private readonly BlockingCollection<TestPlan> _workQueue;
        public bool Closed => _workQueue.IsAddingCompleted;
        public int Count => _workQueue?.Count ?? 0;

        private IDatetimeService _datetimeService { get; set; }

        public TestPlanConsumer(
            IProducerConsumerCollection<TestPlan> testPlanTaskCollection,
            IPersonFactory personFactory,
            IDatetimeService datetimeService)
        {
            _personFactory = personFactory;
            _workQueue = new BlockingCollection<TestPlan>(testPlanTaskCollection);
            _datetimeService = datetimeService;
        }

        public void EnqueueTask(TestPlan workItem)
        {
            _workQueue.Add(workItem);
        }

        private async Task ProcessPlan(TestPlanContext stateInfo)
        {
            if (stateInfo == null)
                return;
            var person = _personFactory.GetPerson(stateInfo.TestPlan.Journey);
            await person.PerformJourneyAsync(stateInfo.TestPlan.Journey, stateInfo.Ctx);
        }

        public async Task ProcessCallback(object stateInfo)
        {
            await ProcessPlan((TestPlanContext)stateInfo);
        }

        public void ProcessQueue(CancellationToken ctx)
        {
            foreach (var testPlan in _workQueue.GetConsumingEnumerable())
            {
                var context = new TestPlanContext()
                {
                    TestPlan = testPlan,
                    Ctx = ctx
                };
                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(
                        async delegate {
                            await ProcessCallback(context);
                        }));

                if (ctx.IsCancellationRequested || _workQueue.Count == 0)
                {
                    Close();
                    break;
                }
            }
        }

        public void Close()
        {
            _workQueue.CompleteAdding();
        }
    }
}
