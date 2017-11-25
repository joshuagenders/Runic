using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.WorkGenerator
{
    public class ExpeditionConsumer : IConsumer<Expedition>
    {
        private readonly IPersonFactory _personFactory;
        private readonly BlockingCollection<Expedition> _workQueue;
        public bool Closed => _workQueue.IsAddingCompleted;
        public int Count => _workQueue?.Count ?? 0;

        private IDatetimeService _datetimeService { get; set; }

        public ExpeditionConsumer(
            IProducerConsumerCollection<Expedition> testPlanTaskCollection,
            IPersonFactory personFactory,
            IDatetimeService datetimeService)
        {
            _personFactory = personFactory;
            _workQueue = new BlockingCollection<Expedition>(testPlanTaskCollection);
            _datetimeService = datetimeService;
        }

        public void EnqueueTask(Expedition workItem)
        {
            _workQueue.Add(workItem);
        }

        private async Task ProcessPlan(ExpeditionContext stateInfo)
        {
            if (stateInfo == null)
                return;
            var person = _personFactory.GetPerson(stateInfo.TestPlan.Journey);
            await person.PerformJourneyAsync(stateInfo.TestPlan.Journey, stateInfo.Ctx);
        }

        public async Task ProcessCallback(object stateInfo)
        {
            await ProcessPlan((ExpeditionContext)stateInfo);
        }

        public void ProcessQueue(CancellationToken ctx)
        {
            foreach (var testPlan in _workQueue.GetConsumingEnumerable())
            {
                var context = new ExpeditionContext()
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
